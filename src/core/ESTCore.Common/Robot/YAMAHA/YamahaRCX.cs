// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Robot.YAMAHA.YamahaRCX
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;
using ESTCore.Common.Core;
using ESTCore.Common.Core.Net;
using ESTCore.Common.Reflection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.Common.Robot.YAMAHA
{
    /// <summary>雅马哈机器人的数据访问类</summary>
    public class YamahaRCX : NetworkDoubleBase
    {
        /// <summary>实例化一个默认的对象</summary>
        public YamahaRCX()
        {
            this.ByteTransform = (IByteTransform)new RegularByteTransform();
            this.ReceiveTimeOut = 30000;
        }

        /// <summary>指定IP地址和端口来实例化一个对象</summary>
        /// <param name="ipAddress">IP地址</param>
        /// <param name="port">端口号</param>
        public YamahaRCX(string ipAddress, int port)
        {
            this.ByteTransform = (IByteTransform)new RegularByteTransform();
            this.IpAddress = ipAddress;
            this.Port = port;
            this.ReceiveTimeOut = 30000;
        }

        /// <inheritdoc />
        public override OperateResult<byte[]> ReadFromCoreServer(
          Socket socket,
          byte[] send,
          bool hasResponseData = true,
          bool usePackHeader = true)
        {
            OperateResult result = this.Send(socket, send);
            if (!result.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>(result);
            return this.ReceiveTimeOut < 0 ? OperateResult.CreateSuccessResult<byte[]>(new byte[0]) : this.ReceiveCommandLineFromSocket(socket, (byte)13, (byte)10, 60000);
        }

        /// <summary>发送命令行到socket, 并从机器人读取指定的命令行</summary>
        /// <param name="send">等待发送的数据</param>
        /// <param name="lines">接收的行数</param>
        /// <returns>结果的结果数据内容</returns>
        public OperateResult<string[]> ReadFromServer(byte[] send, int lines)
        {
            OperateResult<string[]> operateResult = new OperateResult<string[]>();
            this.InteractiveLock.Enter();
            OperateResult<Socket> availableSocket;
            try
            {
                availableSocket = this.GetAvailableSocket();
                if (!availableSocket.IsSuccess)
                {
                    this.IsSocketError = true;
                    this.AlienSession?.Offline();
                    this.InteractiveLock.Leave();
                    operateResult.CopyErrorFromOther<OperateResult<Socket>>(availableSocket);
                    return operateResult;
                }
                List<string> stringList = new List<string>();
                bool flag = false;
                for (int index = 0; index < lines; ++index)
                {
                    OperateResult<byte[]> result = this.ReadFromCoreServer(availableSocket.Content, send, true, true);
                    if (!result.IsSuccess)
                    {
                        flag = true;
                        this.IsSocketError = true;
                        this.AlienSession?.Offline();
                        operateResult.CopyErrorFromOther<OperateResult<byte[]>>(result);
                        break;
                    }
                    stringList.Add(Encoding.ASCII.GetString(result.Content.RemoveLast<byte>(2)));
                }
                if (!flag)
                {
                    this.IsSocketError = false;
                    operateResult.IsSuccess = true;
                    operateResult.Content = stringList.ToArray();
                    operateResult.Message = StringResources.Language.SuccessText;
                }
                this.ExtraAfterReadFromCoreServer(new OperateResult()
                {
                    IsSuccess = !flag
                });
                this.InteractiveLock.Leave();
            }
            catch
            {
                this.InteractiveLock.Leave();
                throw;
            }
            if (!this.isPersistentConn && availableSocket != null)
                availableSocket.Content?.Close();
            return operateResult;
        }

        /// <inheritdoc />
        public override async Task<OperateResult<byte[]>> ReadFromCoreServerAsync(
          Socket socket,
          byte[] send,
          bool hasResponseData = true,
          bool usePackHeader = true)
        {
            OperateResult sendResult = await this.SendAsync(socket, send);
            if (!sendResult.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>(sendResult);
            if (this.ReceiveTimeOut < 0)
                return OperateResult.CreateSuccessResult<byte[]>(new byte[0]);
            OperateResult<byte[]> lineFromSocketAsync = await this.ReceiveCommandLineFromSocketAsync(socket, (byte)13, (byte)10, 60000);
            return lineFromSocketAsync;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Robot.YAMAHA.YamahaRCX.ReadFromServer(System.Byte[],System.Int32)" />
        public async Task<OperateResult<string[]>> ReadFromServerAsync(
          byte[] send,
          int lines)
        {
            OperateResult<string[]> result = new OperateResult<string[]>();
            OperateResult<Socket> resultSocket = (OperateResult<Socket>)null;
            this.InteractiveLock.Enter();
            try
            {
                resultSocket = await this.GetAvailableSocketAsync();
                if (!resultSocket.IsSuccess)
                {
                    this.IsSocketError = true;
                    this.AlienSession?.Offline();
                    this.InteractiveLock.Leave();
                    result.CopyErrorFromOther<OperateResult<Socket>>(resultSocket);
                    return result;
                }
                List<string> buffers = new List<string>();
                bool isError = false;
                for (int i = 0; i < lines; ++i)
                {
                    OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(resultSocket.Content, send, true, true);
                    if (!read.IsSuccess)
                    {
                        isError = true;
                        this.IsSocketError = true;
                        this.AlienSession?.Offline();
                        result.CopyErrorFromOther<OperateResult<byte[]>>(read);
                        break;
                    }
                    buffers.Add(Encoding.ASCII.GetString(read.Content.RemoveLast<byte>(2)));
                    read = (OperateResult<byte[]>)null;
                }
                if (!isError)
                {
                    this.IsSocketError = false;
                    result.IsSuccess = true;
                    result.Content = buffers.ToArray();
                    result.Message = StringResources.Language.SuccessText;
                }
                this.ExtraAfterReadFromCoreServer(new OperateResult()
                {
                    IsSuccess = !isError
                });
                this.InteractiveLock.Leave();
                buffers = (List<string>)null;
            }
            catch
            {
                this.InteractiveLock.Leave();
                throw;
            }
            if (!this.isPersistentConn)
                resultSocket?.Content?.Close();
            return result;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Robot.YAMAHA.YamahaRCX.ReadCommand(System.String,System.Int32)" />
        public async Task<OperateResult<string[]>> ReadCommandAsync(
          string command,
          int lines)
        {
            byte[] buffer = SoftBasic.SpliceArray<byte>(Encoding.ASCII.GetBytes(command), new byte[2]
            {
        (byte) 13,
        (byte) 10
            });
            OperateResult<string[]> operateResult = await this.ReadFromServerAsync(buffer, lines);
            buffer = (byte[])null;
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Robot.YAMAHA.YamahaRCX.Reset" />
        public async Task<OperateResult> ResetAsync()
        {
            OperateResult<string[]> read = await this.ReadCommandAsync("@ RESET ", 1);
            OperateResult operateResult = read.IsSuccess ? this.CheckResponseOk(read.Content[0]) : (OperateResult)read;
            read = (OperateResult<string[]>)null;
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Robot.YAMAHA.YamahaRCX.Run" />
        public async Task<OperateResult> RunAsync()
        {
            OperateResult<string[]> read = await this.ReadCommandAsync("@ RUN ", 1);
            OperateResult operateResult = read.IsSuccess ? this.CheckResponseOk(read.Content[0]) : (OperateResult)read;
            read = (OperateResult<string[]>)null;
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Robot.YAMAHA.YamahaRCX.Stop" />
        public async Task<OperateResult> StopAsync()
        {
            OperateResult<string[]> read = await this.ReadCommandAsync("@ STOP ", 1);
            OperateResult operateResult = read.IsSuccess ? this.CheckResponseOk(read.Content[0]) : (OperateResult)read;
            read = (OperateResult<string[]>)null;
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Robot.YAMAHA.YamahaRCX.ReadMotorStatus" />
        public async Task<OperateResult<int>> ReadMotorStatusAsync()
        {
            OperateResult<string[]> read = await this.ReadCommandAsync("@?MOTOR ", 2);
            if (!read.IsSuccess)
                return OperateResult.CreateFailedResult<int>((OperateResult)read);
            OperateResult check = this.CheckResponseOk(read.Content[1]);
            return check.IsSuccess ? OperateResult.CreateSuccessResult<int>(Convert.ToInt32(read.Content[0])) : OperateResult.CreateFailedResult<int>(check);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Robot.YAMAHA.YamahaRCX.ReadModeStatus" />
        public async Task<OperateResult<int>> ReadModeStatusAsync()
        {
            OperateResult<string[]> read = await this.ReadCommandAsync("@?MODE ", 2);
            if (!read.IsSuccess)
                return OperateResult.CreateFailedResult<int>((OperateResult)read);
            OperateResult check = this.CheckResponseOk(read.Content[1]);
            return check.IsSuccess ? OperateResult.CreateSuccessResult<int>(Convert.ToInt32(read.Content[0])) : OperateResult.CreateFailedResult<int>(check);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Robot.YAMAHA.YamahaRCX.ReadJoints" />
        public async Task<OperateResult<float[]>> ReadJointsAsync()
        {
            OperateResult<string[]> read = await this.ReadCommandAsync("@?WHERE ", 1);
            if (!read.IsSuccess)
                return OperateResult.CreateFailedResult<float[]>((OperateResult)read);
            return OperateResult.CreateSuccessResult<float[]>(((IEnumerable<string>)read.Content[0].Split(new char[1]
            {
        ' '
            }, StringSplitOptions.RemoveEmptyEntries)).Select<string, float>((Func<string, float>)(m => Convert.ToSingle(m))).ToArray<float>());
        }

        /// <inheritdoc cref="M:ESTCore.Common.Robot.YAMAHA.YamahaRCX.ReadEmergencyStatus" />
        public async Task<OperateResult<int>> ReadEmergencyStatusAsync()
        {
            OperateResult<string[]> read = await this.ReadCommandAsync("@?EMG ", 2);
            if (!read.IsSuccess)
                return OperateResult.CreateFailedResult<int>((OperateResult)read);
            OperateResult check = this.CheckResponseOk(read.Content[1]);
            return check.IsSuccess ? OperateResult.CreateSuccessResult<int>(Convert.ToInt32(read.Content[0])) : OperateResult.CreateFailedResult<int>(check);
        }

        /// <summary>
        /// 读取指定的命令的方法，需要指定命令，和接收命令的行数信息<br />
        /// The method of reading the specified command requires the specified command and the line number information of the received command
        /// </summary>
        /// <param name="command">命令</param>
        /// <param name="lines">接收的行数信息</param>
        /// <returns>接收的命令</returns>
        [EstMqttApi(Description = "The method of reading the specified command requires the specified command and the line number information of the received command")]
        public OperateResult<string[]> ReadCommand(string command, int lines) => this.ReadFromServer(SoftBasic.SpliceArray<byte>(Encoding.ASCII.GetBytes(command), new byte[2]
        {
      (byte) 13,
      (byte) 10
        }), lines);

        private OperateResult CheckResponseOk(string msg) => msg.StartsWith("OK") ? OperateResult.CreateSuccessResult() : new OperateResult(msg);

        /// <summary>
        /// 指定程序复位信息，对所有的程序进行复位。当重新启动了程序时，从主程序或者任务 1 中最后执行的程序开头开始执行。<br />
        /// Specify the program reset information to reset all programs. When the program is restarted,
        /// execution starts from the beginning of the main program or the last executed program in task 1.
        /// </summary>
        /// <returns>执行结果是否成功</returns>
        [EstMqttApi(Description = "Specify the program reset information to reset all programs. When the program is restarted")]
        public OperateResult Reset()
        {
            OperateResult<string[]> operateResult = this.ReadCommand("@ RESET ", 1);
            return !operateResult.IsSuccess ? (OperateResult)operateResult : this.CheckResponseOk(operateResult.Content[0]);
        }

        /// <summary>
        /// 执行程序运行。执行所有的 RUN 状态程序。<br />
        /// Execute the program to run. Execute all RUN state programs.
        /// </summary>
        /// <returns>执行结果是否成功</returns>
        [EstMqttApi(Description = "Execute the program to run. Execute all RUN state programs.")]
        public OperateResult Run()
        {
            OperateResult<string[]> operateResult = this.ReadCommand("@ RUN ", 1);
            return !operateResult.IsSuccess ? (OperateResult)operateResult : this.CheckResponseOk(operateResult.Content[0]);
        }

        /// <summary>
        /// 执行程序停止。执行所有的 STOP 状态程序。<br />
        /// The execution program stops. Execute all STOP state programs.
        /// </summary>
        /// <returns>执行结果是否成功</returns>
        [EstMqttApi(Description = "The execution program stops. Execute all STOP state programs.")]
        public OperateResult Stop()
        {
            OperateResult<string[]> operateResult = this.ReadCommand("@ STOP ", 1);
            return !operateResult.IsSuccess ? (OperateResult)operateResult : this.CheckResponseOk(operateResult.Content[0]);
        }

        /// <summary>
        /// 获取马达电源状态，返回的0:马达电源关闭; 1:马达电源开启; 2:马达电源开启＋所有机器人伺服开启<br />
        /// Get the motor power status, return 0: motor power off; 1: motor power on; 2: motor power on + all robot servos on
        /// </summary>
        /// <returns>返回的0:马达电源关闭; 1:马达电源开启; 2:马达电源开启＋所有机器人伺服开启</returns>
        [EstMqttApi(Description = "Get the motor power status, return 0: motor power off; 1: motor power on; 2: motor power on + all robot servos on")]
        public OperateResult<int> ReadMotorStatus()
        {
            OperateResult<string[]> operateResult = this.ReadCommand("@?MOTOR ", 2);
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<int>((OperateResult)operateResult);
            OperateResult result = this.CheckResponseOk(operateResult.Content[1]);
            return !result.IsSuccess ? OperateResult.CreateFailedResult<int>(result) : OperateResult.CreateSuccessResult<int>(Convert.ToInt32(operateResult.Content[0]));
        }

        /// <summary>
        /// 读取模式状态<br />
        /// Read mode status
        /// </summary>
        /// <returns>模式的状态信息</returns>
        [EstMqttApi(Description = "Read mode status")]
        public OperateResult<int> ReadModeStatus()
        {
            OperateResult<string[]> operateResult = this.ReadCommand("@?MODE ", 2);
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<int>((OperateResult)operateResult);
            OperateResult result = this.CheckResponseOk(operateResult.Content[1]);
            return !result.IsSuccess ? OperateResult.CreateFailedResult<int>(result) : OperateResult.CreateSuccessResult<int>(Convert.ToInt32(operateResult.Content[0]));
        }

        /// <summary>
        /// 读取关节的基本数据信息<br />
        /// Read the basic data information of the joint
        /// </summary>
        /// <returns>关节信息</returns>
        [EstMqttApi(Description = "Read the basic data information of the joint")]
        public OperateResult<float[]> ReadJoints()
        {
            OperateResult<string[]> operateResult = this.ReadCommand("@?WHERE ", 1);
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<float[]>((OperateResult)operateResult);
            return OperateResult.CreateSuccessResult<float[]>(((IEnumerable<string>)operateResult.Content[0].Split(new char[1]
            {
        ' '
            }, StringSplitOptions.RemoveEmptyEntries)).Select<string, float>((Func<string, float>)(m => Convert.ToSingle(m))).ToArray<float>());
        }

        /// <summary>
        /// 读取紧急停止状态，0 ：正常状态、1 ：紧急停止状态<br />
        /// Read emergency stop state, 0: normal state, 1: emergency stop state
        /// </summary>
        /// <returns>0 ：正常状态、1 ：紧急停止状态</returns>
        [EstMqttApi(Description = "Read emergency stop state, 0: normal state, 1: emergency stop state")]
        public OperateResult<int> ReadEmergencyStatus()
        {
            OperateResult<string[]> operateResult = this.ReadCommand("@?EMG ", 2);
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<int>((OperateResult)operateResult);
            OperateResult result = this.CheckResponseOk(operateResult.Content[1]);
            return !result.IsSuccess ? OperateResult.CreateFailedResult<int>(result) : OperateResult.CreateSuccessResult<int>(Convert.ToInt32(operateResult.Content[0]));
        }
    }
}
