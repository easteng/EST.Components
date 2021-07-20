// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Robot.KUKA.KukaAvarProxyNet
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;
using ESTCore.Common.Core;
using ESTCore.Common.Core.IMessage;
using ESTCore.Common.Core.Net;
using ESTCore.Common.Reflection;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.Common.Robot.KUKA
{
    /// <summary>
    /// Kuka机器人的数据交互类，通讯支持的条件为KUKA 的 KRC4 控制器中运行KUKAVARPROXY 这个第三方软件，端口通常为7000<br />
    /// The data interaction class of Kuka robot is supported by the third-party software KUKAVARPROXY running in the KRC4 controller of Kuka. The port is usually 7000
    /// </summary>
    /// <remarks>
    /// 非常感谢 昆山-LT 网友的测试和意见反馈。<br />
    /// 其中KUKAVARPROXY 这个第三方软件在来源地址：
    /// https://github.com/ImtsSrl/KUKAVARPROXY <br />
    /// 如果想要更详细的了解配置，连接，数据读取操作，请点击下面的连接：http://blog.davidrobot.com/2019/03/hsl_for_kuka.html?tdsourcetag=s_pctim_aiomsg
    /// </remarks>
    public class KukaAvarProxyNet : NetworkDoubleBase, IRobotNet
    {
        private SoftIncrementCount softIncrementCount;

        /// <summary>
        /// 实例化一个默认的对象<br />
        /// Instantiate a default object
        /// </summary>
        public KukaAvarProxyNet()
        {
            this.softIncrementCount = new SoftIncrementCount((long)ushort.MaxValue);
            this.ByteTransform = (IByteTransform)new ReverseWordTransform();
        }

        /// <summary>
        /// 实例化一个默认的Kuka机器人对象，并指定IP地址和端口号，端口号通常为7000<br />
        /// Instantiate a default Kuka robot object and specify the IP address and port number, usually 7000
        /// </summary>
        /// <param name="ipAddress">Ip地址</param>
        /// <param name="port">端口号</param>
        public KukaAvarProxyNet(string ipAddress, int port)
        {
            this.IpAddress = ipAddress;
            this.Port = port;
            this.softIncrementCount = new SoftIncrementCount((long)ushort.MaxValue);
            this.ByteTransform = (IByteTransform)new ReverseWordTransform();
        }

        /// <inheritdoc />
        protected override INetMessage GetNewNetMessage() => (INetMessage)new KukaVarProxyMessage();

        /// <summary>
        /// 读取Kuka机器人的数据内容，根据输入的变量名称来读取<br />
        /// Read the data content of the Kuka robot according to the input variable name
        /// </summary>
        /// <param name="address">地址数据</param>
        /// <returns>带有成功标识的byte[]数组</returns>
        [EstMqttApi(ApiTopic = "ReadRobotByte", Description = "Read the data content of the Kuka robot according to the input variable name")]
        public OperateResult<byte[]> Read(string address) => ByteTransformHelper.GetResultFromOther<byte[], byte[]>(this.ReadFromCoreServer(this.PackCommand(this.BuildReadValueCommand(address))), new Func<byte[], OperateResult<byte[]>>(this.ExtractActualData));

        /// <summary>
        /// 读取Kuka机器人的所有的数据信息，返回字符串信息，解码方式为ANSI，需要指定变量名称<br />
        /// Read all the data information of the Kuka robot, return the string information, decode by ANSI, need to specify the variable name
        /// </summary>
        /// <param name="address">地址信息</param>
        /// <returns>带有成功标识的字符串数据</returns>
        [EstMqttApi(ApiTopic = "ReadRobotString", Description = "Read all the data information of the Kuka robot, return the string information, decode by ANSI, need to specify the variable name")]
        public OperateResult<string> ReadString(string address) => ByteTransformHelper.GetSuccessResultFromOther<string, byte[]>(this.Read(address), new Func<byte[], string>(Encoding.Default.GetString));

        /// <summary>
        /// 根据Kuka机器人的变量名称，写入原始的数据内容<br />
        /// Write the original data content according to the variable name of the Kuka robot
        /// </summary>
        /// <param name="address">变量名称</param>
        /// <param name="value">原始的字节数据信息</param>
        /// <returns>是否成功的写入</returns>
        [EstMqttApi(ApiTopic = "WriteRobotByte", Description = "Write the original data content according to the variable name of the Kuka robot")]
        public OperateResult Write(string address, byte[] value) => this.Write(address, Encoding.Default.GetString(value));

        /// <summary>
        /// 根据Kuka机器人的变量名称，写入ANSI编码的字符串数据信息<br />
        /// Writes ansi-encoded string data information based on the variable name of the Kuka robot
        /// </summary>
        /// <param name="address">变量名称</param>
        /// <param name="value">ANSI编码的字符串</param>
        /// <returns>是否成功的写入</returns>
        [EstMqttApi(ApiTopic = "WriteRobotString", Description = "Writes ansi-encoded string data information based on the variable name of the Kuka robot")]
        public OperateResult Write(string address, string value) => (OperateResult)ByteTransformHelper.GetResultFromOther<byte[], byte[]>(this.ReadFromCoreServer(this.PackCommand(this.BuildWriteValueCommand(address, value))), new Func<byte[], OperateResult<byte[]>>(this.ExtractActualData));

        /// <inheritdoc cref="M:ESTCore.Common.Robot.KUKA.KukaAvarProxyNet.Read(System.String)" />
        public async Task<OperateResult<byte[]>> ReadAsync(string address)
        {
            OperateResult<byte[]> result = await this.ReadFromCoreServerAsync(this.PackCommand(this.BuildReadValueCommand(address)));
            return ByteTransformHelper.GetResultFromOther<byte[], byte[]>(result, new Func<byte[], OperateResult<byte[]>>(this.ExtractActualData));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Robot.KUKA.KukaAvarProxyNet.ReadString(System.String)" />
        public async Task<OperateResult<string>> ReadStringAsync(string address)
        {
            OperateResult<byte[]> result = await this.ReadAsync(address);
            return ByteTransformHelper.GetSuccessResultFromOther<string, byte[]>(result, new Func<byte[], string>(Encoding.Default.GetString));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Robot.KUKA.KukaAvarProxyNet.Write(System.String,System.Byte[])" />
        public async Task<OperateResult> WriteAsync(string address, byte[] value)
        {
            OperateResult operateResult = await this.WriteAsync(address, Encoding.Default.GetString(value));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Robot.KUKA.KukaAvarProxyNet.Write(System.String,System.String)" />
        public async Task<OperateResult> WriteAsync(string address, string value)
        {
            OperateResult<byte[]> result = await this.ReadFromCoreServerAsync(this.PackCommand(this.BuildWriteValueCommand(address, value)));
            return (OperateResult)ByteTransformHelper.GetResultFromOther<byte[], byte[]>(result, new Func<byte[], OperateResult<byte[]>>(this.ExtractActualData));
        }

        /// <summary>
        /// 将核心的指令打包成一个可用于发送的消息对象<br />
        /// Package the core instructions into a message object that can be sent
        /// </summary>
        /// <param name="commandCore">核心命令</param>
        /// <returns>最终实现的可以发送的机器人的字节数据</returns>
        private byte[] PackCommand(byte[] commandCore)
        {
            byte[] numArray = new byte[commandCore.Length + 4];
            this.ByteTransform.TransByte((ushort)this.softIncrementCount.GetCurrentValue()).CopyTo((Array)numArray, 0);
            this.ByteTransform.TransByte((ushort)commandCore.Length).CopyTo((Array)numArray, 2);
            commandCore.CopyTo((Array)numArray, 4);
            return numArray;
        }

        private OperateResult<byte[]> ExtractActualData(byte[] response)
        {
            try
            {
                if (response[response.Length - 1] != (byte)1)
                    return new OperateResult<byte[]>((int)response[response.Length - 1], "Wrong: " + SoftBasic.ByteToHexString(response, ' '));
                int length = (int)response[5] * 256 + (int)response[6];
                byte[] numArray = new byte[length];
                Array.Copy((Array)response, 7, (Array)numArray, 0, length);
                return OperateResult.CreateSuccessResult<byte[]>(numArray);
            }
            catch (Exception ex)
            {
                return new OperateResult<byte[]>("Wrong:" + ex.Message + " Code:" + SoftBasic.ByteToHexString(response, ' '));
            }
        }

        private byte[] BuildCommands(byte function, string[] commands)
        {
            List<byte> byteList = new List<byte>();
            byteList.Add(function);
            for (int index = 0; index < commands.Length; ++index)
            {
                byte[] bytes = Encoding.Default.GetBytes(commands[index]);
                byteList.AddRange((IEnumerable<byte>)this.ByteTransform.TransByte((ushort)bytes.Length));
                byteList.AddRange((IEnumerable<byte>)bytes);
            }
            return byteList.ToArray();
        }

        private byte[] BuildReadValueCommand(string address) => this.BuildCommands((byte)0, new string[1]
        {
      address
        });

        private byte[] BuildWriteValueCommand(string address, string value) => this.BuildCommands((byte)1, new string[2]
        {
      address,
      value
        });

        /// <inheritdoc />
        public override string ToString() => string.Format("KukaAvarProxyNet Robot[{0}:{1}]", (object)this.IpAddress, (object)this.Port);
    }
}
