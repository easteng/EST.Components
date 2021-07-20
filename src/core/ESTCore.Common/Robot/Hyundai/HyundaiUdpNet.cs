// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Robot.Hyundai.HyundaiUdpNet
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;
using ESTCore.Common.Core.Net;
using ESTCore.Common.LogNet;
using ESTCore.Common.Reflection;

using System;
using System.Net;

namespace ESTCore.Common.Robot.Hyundai
{
    /// <summary>
    /// 现代机器人的UDP通讯类，注意本类是服务器，需要等待机器人先配置好ip地址及端口，然后连接到本服务器才能正确的进行操作。详细参见api文档注释<br />
    /// The UDP communication class of modern robots. Note that this class is a server. You need to wait for the robot to configure the IP address and port first,
    /// and then connect to this server to operate correctly. See api documentation for details
    /// </summary>
    /// <remarks>
    /// 为使用联机跟踪功能，通过JOB文件的 OnLTrack 命令激活本功能后对通信及位置增量命令 Filter 进行设置，必要时以 LIMIT 命令设置机器人的动作领域，速度限制项。
    /// 最后采用 OnLTrack 命令关闭联机跟踪功能以退出本功能。<br />
    /// 功能开始，通信及 Filter 设置，程序示例： OnLTrack ON,IP=192.168.1.254,PORT=7127,CRD=1,Bypass,Fn=10
    /// </remarks>
    public class HyundaiUdpNet : NetworkUdpServerBase
    {
        private HyundaiData hyundaiDataHistory;
        private SoftIncrementCount incrementCount;
        private EndPoint Remote;

        /// <summary>
        /// 实例化一个默认的对象<br />
        /// Instantiate a default object
        /// </summary>
        public HyundaiUdpNet() => this.incrementCount = new SoftIncrementCount((long)int.MaxValue);

        /// <inheritdoc />
        protected override void ThreadReceiveCycle()
        {
            while (this.IsStarted)
            {
                this.Remote = (EndPoint)new IPEndPoint(IPAddress.Any, 0);
                byte[] numArray = new byte[64];
                int from;
                try
                {
                    from = this.CoreSocket.ReceiveFrom(numArray, ref this.Remote);
                }
                catch (Exception ex)
                {
                    if (this.IsStarted)
                    {
                        ILogNet logNet = this.LogNet;
                        if (logNet != null)
                        {
                            logNet.WriteException(nameof(ThreadReceiveCycle), ex);
                            continue;
                        }
                        continue;
                    }
                    continue;
                }
                if (from != 64)
                {
                    this.LogNet?.WriteError(this.ToString(), string.Format("Receive Error Length[{0}]: {1}", (object)from, (object)numArray.ToHexString()));
                }
                else
                {
                    this.LogNet?.WriteDebug(this.ToString(), "Receive: " + numArray.ToHexString());
                    HyundaiData data = new HyundaiData(numArray);
                    if (data.Command == 'S')
                    {
                        this.Write(new HyundaiData(numArray)
                        {
                            Command = 'S',
                            Count = 0,
                            State = 1
                        });
                        this.LogNet?.WriteDebug(this.ToString(), "Send: " + data.ToBytes().ToHexString());
                        this.LogNet?.WriteDebug(this.ToString(), "Online tracking is started by Hi5 controller.");
                    }
                    else if (data.Command == 'P')
                    {
                        this.hyundaiDataHistory = data;
                        HyundaiUdpNet.OnHyundaiMessageReceiveDelegate hyundaiMessageReceive = this.OnHyundaiMessageReceive;
                        if (hyundaiMessageReceive != null)
                            hyundaiMessageReceive(data);
                    }
                    else if (data.Command == 'F')
                        this.LogNet?.WriteDebug(this.ToString(), "Online tracking is finished by Hi5 controller.");
                }
            }
        }

        /// <summary>
        /// 将指定的增量写入机器人，需要指定6个参数，位置和角度信息，其中位置单位为mm，角度单位为°<br />
        /// To write the specified increment to the robot, you need to specify 6 parameters,
        /// position and angle information, where the position unit is mm and the angle unit is °
        /// </summary>
        /// <param name="x">X轴增量信息，单位毫米</param>
        /// <param name="y">Y轴增量信息，单位毫米</param>
        /// <param name="z">Z轴增量信息，单位毫米</param>
        /// <param name="rx">X轴角度增量信息，单位角度</param>
        /// <param name="ry">Y轴角度增量信息，单位角度</param>
        /// <param name="rz">Z轴角度增量信息，单位角度</param>
        /// <returns>是否写入机器人成功</returns>
        [EstMqttApi]
        public OperateResult WriteIncrementPos(
          double x,
          double y,
          double z,
          double rx,
          double ry,
          double rz)
        {
            return this.WriteIncrementPos(new double[6]
            {
        x,
        y,
        z,
        rx,
        ry,
        rz
            });
        }

        /// <summary>
        /// 将指定的增量写入机器人，需要指定6个参数，位置和角度信息，其中位置单位为mm，角度单位为°<br />
        /// To write the specified increment to the robot, you need to specify 6 parameters, position and angle information, where the position unit is mm and the angle unit is °
        /// </summary>
        /// <param name="pos">增量的数组信息</param>
        /// <returns>是否写入机器人成功</returns>
        [EstMqttApi]
        public OperateResult WriteIncrementPos(double[] pos)
        {
            HyundaiData data = new HyundaiData()
            {
                Command = 'P',
                State = 2,
                Count = (int)this.incrementCount.GetCurrentValue()
            };
            for (int index = 0; index < data.Data.Length; ++index)
                data.Data[index] = pos[index];
            return this.Write(data);
        }

        /// <summary>
        /// 将指定的命令写入机器人，该命令是完全自定义的，需要遵循机器人的通讯协议，在写入之前，需要调用<see cref="M:ESTCore.Common.Core.Net.NetworkUdpServerBase.ServerStart(System.Int32)" /> 方法<br />
        /// Write the specified command to the robot. The command is completely customized and needs to follow the robot's communication protocol.
        /// Before writing, you need to call the <see cref="M:ESTCore.Common.Core.Net.NetworkUdpServerBase.ServerStart(System.Int32)" />
        /// </summary>
        /// <param name="data">机器人数据</param>
        /// <returns>是否写入成功</returns>
        [EstMqttApi]
        public OperateResult Write(HyundaiData data)
        {
            if (!this.IsStarted)
                return new OperateResult("Please Start Server First!");
            if (this.Remote == null)
                return new OperateResult("Please Wait Robot Connect!");
            try
            {
                this.CoreSocket.SendTo(data.ToBytes(), this.Remote);
                return OperateResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                return new OperateResult(ex.Message);
            }
        }

        /// <summary>
        /// 机器人在X轴上移动一小段距离，单位毫米<br />
        /// The robot moves a short distance on the X axis, in millimeters
        /// </summary>
        /// <param name="value">移动距离，单位毫米</param>
        /// <returns>是否写入成功</returns>
        [EstMqttApi]
        public OperateResult MoveX(double value) => this.WriteIncrementPos(value, 0.0, 0.0, 0.0, 0.0, 0.0);

        /// <summary>
        /// 机器人在Y轴上移动一小段距离，单位毫米<br />
        /// The robot moves a short distance on the Y axis, in millimeters
        /// </summary>
        /// <param name="value">移动距离，单位毫米</param>
        /// <returns>是否写入成功</returns>
        [EstMqttApi]
        public OperateResult MoveY(double value) => this.WriteIncrementPos(0.0, value, 0.0, 0.0, 0.0, 0.0);

        /// <summary>
        /// 机器人在Z轴上移动一小段距离，单位毫米<br />
        /// The robot moves a short distance on the Z axis, in millimeters
        /// </summary>
        /// <param name="value">移动距离，单位毫米</param>
        /// <returns>是否写入成功</returns>
        [EstMqttApi]
        public OperateResult MoveZ(double value) => this.WriteIncrementPos(0.0, 0.0, value, 0.0, 0.0, 0.0);

        /// <summary>
        /// 机器人在X轴方向上旋转指定角度，单位角度<br />
        /// The robot rotates the specified angle in the X axis direction, the unit angle
        /// </summary>
        /// <param name="value">旋转角度，单位角度</param>
        /// <returns>是否写入成功</returns>
        [EstMqttApi]
        public OperateResult RotateX(double value) => this.WriteIncrementPos(0.0, 0.0, 0.0, value, 0.0, 0.0);

        /// <summary>
        /// 机器人在Y轴方向上旋转指定角度，单位角度<br />
        /// The robot rotates the specified angle in the Y axis direction, the unit angle
        /// </summary>
        /// <param name="value">旋转角度，单位角度</param>
        /// <returns>是否写入成功</returns>
        [EstMqttApi]
        public OperateResult RotateY(double value) => this.WriteIncrementPos(0.0, 0.0, 0.0, 0.0, value, 0.0);

        /// <summary>
        /// 机器人在Z轴方向上旋转指定角度，单位角度<br />
        /// The robot rotates the specified angle in the Z axis direction, the unit angle
        /// </summary>
        /// <param name="value">旋转角度，单位角度</param>
        /// <returns>是否写入成功</returns>
        [EstMqttApi]
        public OperateResult RotateZ(double value) => this.WriteIncrementPos(0.0, 0.0, 0.0, 0.0, 0.0, value);

        /// <summary>当接收到机器人数据的时候触发的事件</summary>
        public event HyundaiUdpNet.OnHyundaiMessageReceiveDelegate OnHyundaiMessageReceive;

        /// <inheritdoc />
        public override string ToString() => string.Format("HyundaiUdpNet[{0}]", (object)this.Port);

        /// <summary>收到机器人消息的事件委托</summary>
        /// <param name="data">机器人消息</param>
        public delegate void OnHyundaiMessageReceiveDelegate(HyundaiData data);
    }
}
