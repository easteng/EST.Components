// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Robot.YASKAWA.YRC1000TcpNet
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;
using ESTCore.Common.Core;
using ESTCore.Common.Core.Net;
using ESTCore.Common.Reflection;

using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.Common.Robot.YASKAWA
{
    /// <summary>
    /// 安川机器人的Ethernet 服务器功能的通讯类<br />
    /// Yaskawa robot's Ethernet server features a communication class
    /// </summary>
    public class YRC1000TcpNet : NetworkDoubleBase, IRobotNet
    {
        /// <summary>
        /// 指定机器人的ip地址及端口号来实例化对象<br />
        /// Specify the robot's IP address and port number to instantiate the object
        /// </summary>
        /// <param name="ipAddress">Ip地址</param>
        /// <param name="port">端口号</param>
        public YRC1000TcpNet(string ipAddress, int port)
        {
            this.IpAddress = ipAddress;
            this.Port = port;
            this.ByteTransform = (IByteTransform)new ReverseWordTransform();
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.IRobotNet.Read(System.String)" />
        [EstMqttApi(ApiTopic = "ReadRobotByte", Description = "Read the robot's original byte data information according to the address")]
        public OperateResult<byte[]> Read(string address)
        {
            OperateResult<string> operateResult = this.ReadString(address);
            return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult) : OperateResult.CreateSuccessResult<byte[]>(Encoding.ASCII.GetBytes(operateResult.Content));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.IRobotNet.ReadString(System.String)" />
        [EstMqttApi(ApiTopic = "ReadRobotString", Description = "Read the string data information of the robot based on the address")]
        public OperateResult<string> ReadString(string address)
        {
            if (address.IndexOf('.') < 0 && address.IndexOf(':') < 0 && address.IndexOf(';') < 0)
                return this.ReadByCommand(address, (string)null);
            string[] strArray = address.Split('.', ':', ';');
            return this.ReadByCommand(strArray[0], strArray[1]);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.IRobotNet.Write(System.String,System.Byte[])" />
        [EstMqttApi(ApiTopic = "WriteRobotByte", Description = "According to the address, to write the device related bytes data")]
        public OperateResult Write(string address, byte[] value) => this.Write(address, Encoding.ASCII.GetString(value));

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.IRobotNet.Write(System.String,System.String)" />
        [EstMqttApi(ApiTopic = "WriteRobotString", Description = "According to the address, to write the device related string data")]
        public OperateResult Write(string address, string value) => (OperateResult)this.ReadByCommand(address, value);

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.IRobotNet.ReadAsync(System.String)" />
        public async Task<OperateResult<byte[]>> ReadAsync(string address)
        {
            OperateResult<string> read = await this.ReadStringAsync(address);
            OperateResult<byte[]> operateResult = read.IsSuccess ? OperateResult.CreateSuccessResult<byte[]>(Encoding.ASCII.GetBytes(read.Content)) : OperateResult.CreateFailedResult<byte[]>((OperateResult)read);
            read = (OperateResult<string>)null;
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.IRobotNet.ReadStringAsync(System.String)" />
        public async Task<OperateResult<string>> ReadStringAsync(string address)
        {
            if (address.IndexOf('.') >= 0 || address.IndexOf(':') >= 0 || address.IndexOf(';') >= 0)
            {
                string[] commands = address.Split('.', ':', ';');
                OperateResult<string> operateResult = await this.ReadByCommandAsync(commands[0], commands[1]);
                return operateResult;
            }
            OperateResult<string> operateResult1 = await this.ReadByCommandAsync(address, (string)null);
            return operateResult1;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.IRobotNet.WriteAsync(System.String,System.Byte[])" />
        public async Task<OperateResult> WriteAsync(string address, byte[] value)
        {
            OperateResult operateResult = await this.WriteAsync(address, Encoding.ASCII.GetString(value));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.IRobotNet.WriteAsync(System.String,System.String)" />
        public async Task<OperateResult> WriteAsync(string address, string value)
        {
            OperateResult<string> operateResult = await this.ReadByCommandAsync(address, value);
            return (OperateResult)operateResult;
        }

        /// <summary>
        /// before read data , the connection should be Initialized
        /// </summary>
        /// <param name="socket">connected socket</param>
        /// <returns>whether is the Initialization is success.</returns>
        protected override OperateResult InitializationOnConnect(Socket socket)
        {
            OperateResult<string> operateResult = this.ReadFromCoreServer(socket, "CONNECT Robot_access KeepAlive:-1\r\n");
            if (!operateResult.IsSuccess)
                return (OperateResult)operateResult;
            if (operateResult.Content == "OK:YR Information Server(Ver) Keep-Alive:-1.\r\n")
                return OperateResult.CreateSuccessResult();
            if (!operateResult.Content.StartsWith("OK:"))
                return new OperateResult(operateResult.Content);
            this.isPersistentConn = false;
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// before read data , the connection should be Initialized
        /// </summary>
        /// <param name="socket">connected socket</param>
        /// <returns>whether is the Initialization is success.</returns>
        protected override async Task<OperateResult> InitializationOnConnectAsync(
          Socket socket)
        {
            OperateResult<string> read = await this.ReadFromCoreServerAsync(socket, "CONNECT Robot_access KeepAlive:-1\r\n");
            if (!read.IsSuccess)
                return (OperateResult)read;
            if (read.Content == "OK:YR Information Server(Ver) Keep-Alive:-1.\r\n")
                return OperateResult.CreateSuccessResult();
            if (!read.Content.StartsWith("OK:"))
                return new OperateResult(read.Content);
            this.isPersistentConn = false;
            return OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc />
        public override OperateResult<byte[]> ReadFromCoreServer(
          Socket socket,
          byte[] send,
          bool hasResponseData = true,
          bool usePackHeader = true)
        {
            this.LogNet?.WriteDebug(this.ToString(), StringResources.Language.Send + " : " + SoftBasic.ByteToHexString(send, ' '));
            OperateResult result = this.Send(socket, send);
            if (!result.IsSuccess)
            {
                socket?.Close();
                return OperateResult.CreateFailedResult<byte[]>(result);
            }
            if (this.ReceiveTimeOut < 0)
                return OperateResult.CreateSuccessResult<byte[]>(new byte[0]);
            OperateResult<byte[]> commandLineFromSocket = this.ReceiveCommandLineFromSocket(socket, (byte)13, (byte)10, this.ReceiveTimeOut);
            if (!commandLineFromSocket.IsSuccess)
                return new OperateResult<byte[]>(StringResources.Language.ReceiveDataTimeout + this.ReceiveTimeOut.ToString());
            this.LogNet?.WriteDebug(this.ToString(), StringResources.Language.Receive + " : " + SoftBasic.ByteToHexString(commandLineFromSocket.Content, ' '));
            return OperateResult.CreateSuccessResult<byte[]>(commandLineFromSocket.Content);
        }

        /// <summary>Read string value from socket</summary>
        /// <param name="socket">connected socket</param>
        /// <param name="send">string value</param>
        /// <returns>received string value with is successfully</returns>
        protected OperateResult<string> ReadFromCoreServer(Socket socket, string send)
        {
            OperateResult<byte[]> operateResult = this.ReadFromCoreServer(socket, Encoding.Default.GetBytes(send), true, true);
            return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<string>((OperateResult)operateResult) : OperateResult.CreateSuccessResult<string>(Encoding.Default.GetString(operateResult.Content));
        }

        /// <summary>
        /// 根据指令来读取设备的信息，如果命令数据为空，则传入null即可，注意，所有的命令不带换行符<br />
        /// Read the device information according to the instructions. If the command data is empty, pass in null. Note that all commands do not have a newline character
        /// </summary>
        /// <param name="command">命令的内容</param>
        /// <param name="commandData">命令数据内容</param>
        /// <returns>最终的结果内容，需要对IsSuccess进行验证</returns>
        [EstMqttApi(Description = "Read the device information according to the instructions. If the command data is empty, pass in null. Note that all commands do not have a newline character")]
        public OperateResult<string> ReadByCommand(string command, string commandData)
        {
            this.InteractiveLock.Enter();
            OperateResult<Socket> availableSocket = this.GetAvailableSocket();
            if (!availableSocket.IsSuccess)
            {
                this.IsSocketError = true;
                this.AlienSession?.Offline();
                this.InteractiveLock.Leave();
                return OperateResult.CreateFailedResult<string>((OperateResult)availableSocket);
            }
            string send = string.IsNullOrEmpty(commandData) ? "HOSTCTRL_REQUEST " + command + " 0\r\n" : string.Format("HOSTCTRL_REQUEST {0} {1}\r\n", (object)command, (object)(commandData.Length + 1));
            OperateResult<string> operateResult = this.ReadFromCoreServer(availableSocket.Content, send);
            if (!operateResult.IsSuccess)
            {
                this.IsSocketError = true;
                this.AlienSession?.Offline();
                this.InteractiveLock.Leave();
                return OperateResult.CreateFailedResult<string>((OperateResult)operateResult);
            }
            if (!operateResult.Content.StartsWith("OK:"))
            {
                if (!this.isPersistentConn)
                    availableSocket.Content?.Close();
                this.InteractiveLock.Leave();
                return new OperateResult<string>(operateResult.Content.Remove(operateResult.Content.Length - 2));
            }
            if (!string.IsNullOrEmpty(commandData))
            {
                byte[] bytes = Encoding.ASCII.GetBytes(commandData + "\r");
                this.LogNet?.WriteDebug(this.ToString(), StringResources.Language.Send + " : " + SoftBasic.ByteToHexString(bytes, ' '));
                OperateResult result = this.Send(availableSocket.Content, bytes);
                if (!result.IsSuccess)
                {
                    availableSocket.Content?.Close();
                    this.IsSocketError = true;
                    this.AlienSession?.Offline();
                    this.InteractiveLock.Leave();
                    return OperateResult.CreateFailedResult<string>(result);
                }
            }
            OperateResult<byte[]> commandLineFromSocket = this.ReceiveCommandLineFromSocket(availableSocket.Content, (byte)13, this.ReceiveTimeOut);
            if (!commandLineFromSocket.IsSuccess)
            {
                this.IsSocketError = true;
                this.AlienSession?.Offline();
                this.InteractiveLock.Leave();
                return OperateResult.CreateFailedResult<string>((OperateResult)commandLineFromSocket);
            }
            string msg = Encoding.ASCII.GetString(commandLineFromSocket.Content);
            if (msg.StartsWith("ERROR:"))
            {
                if (!this.isPersistentConn)
                    availableSocket.Content?.Close();
                this.InteractiveLock.Leave();
                this.Receive(availableSocket.Content, 1);
                return new OperateResult<string>(msg);
            }
            if (msg.StartsWith("0000\r"))
            {
                if (!this.isPersistentConn)
                    availableSocket.Content?.Close();
                this.Receive(availableSocket.Content, 1);
                this.InteractiveLock.Leave();
                return OperateResult.CreateSuccessResult<string>("0000");
            }
            if (!this.isPersistentConn)
                availableSocket.Content?.Close();
            this.InteractiveLock.Leave();
            return OperateResult.CreateSuccessResult<string>(msg.Remove(msg.Length - 1));
        }

        /// <inheritdoc />
        public override async Task<OperateResult<byte[]>> ReadFromCoreServerAsync(
          Socket socket,
          byte[] send,
          bool hasResponseData = true,
          bool usePackHeader = true)
        {
            this.LogNet?.WriteDebug(this.ToString(), StringResources.Language.Send + " : " + SoftBasic.ByteToHexString(send, ' '));
            OperateResult sendResult = await this.SendAsync(socket, send);
            if (!sendResult.IsSuccess)
            {
                socket?.Close();
                return OperateResult.CreateFailedResult<byte[]>(sendResult);
            }
            if (this.ReceiveTimeOut < 0)
                return OperateResult.CreateSuccessResult<byte[]>(new byte[0]);
            OperateResult<byte[]> resultReceive = await this.ReceiveCommandLineFromSocketAsync(socket, (byte)13, (byte)10, this.ReceiveTimeOut);
            if (!resultReceive.IsSuccess)
                return new OperateResult<byte[]>(StringResources.Language.ReceiveDataTimeout + this.ReceiveTimeOut.ToString());
            this.LogNet?.WriteDebug(this.ToString(), StringResources.Language.Receive + " : " + SoftBasic.ByteToHexString(resultReceive.Content, ' '));
            return OperateResult.CreateSuccessResult<byte[]>(resultReceive.Content);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Robot.YASKAWA.YRC1000TcpNet.ReadFromCoreServer(System.Net.Sockets.Socket,System.String)" />
        protected async Task<OperateResult<string>> ReadFromCoreServerAsync(
          Socket socket,
          string send)
        {
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(socket, Encoding.Default.GetBytes(send), true, true);
            OperateResult<string> operateResult = read.IsSuccess ? OperateResult.CreateSuccessResult<string>(Encoding.Default.GetString(read.Content)) : OperateResult.CreateFailedResult<string>((OperateResult)read);
            read = (OperateResult<byte[]>)null;
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Robot.YASKAWA.YRC1000TcpNet.ReadByCommand(System.String,System.String)" />
        public async Task<OperateResult<string>> ReadByCommandAsync(
          string command,
          string commandData)
        {
            this.InteractiveLock.Enter();
            OperateResult<Socket> resultSocket = await this.GetAvailableSocketAsync();
            if (!resultSocket.IsSuccess)
            {
                this.IsSocketError = true;
                this.AlienSession?.Offline();
                this.InteractiveLock.Leave();
                return OperateResult.CreateFailedResult<string>((OperateResult)resultSocket);
            }
            string sendCommand = string.IsNullOrEmpty(commandData) ? "HOSTCTRL_REQUEST " + command + " 0\r\n" : string.Format("HOSTCTRL_REQUEST {0} {1}\r\n", (object)command, (object)(commandData.Length + 1));
            OperateResult<string> readCommand = await this.ReadFromCoreServerAsync(resultSocket.Content, sendCommand);
            if (!readCommand.IsSuccess)
            {
                this.IsSocketError = true;
                this.AlienSession?.Offline();
                this.InteractiveLock.Leave();
                return OperateResult.CreateFailedResult<string>((OperateResult)readCommand);
            }
            if (!readCommand.Content.StartsWith("OK:"))
            {
                if (!this.isPersistentConn)
                    resultSocket.Content?.Close();
                this.InteractiveLock.Leave();
                return new OperateResult<string>(readCommand.Content.Remove(readCommand.Content.Length - 2));
            }
            if (!string.IsNullOrEmpty(commandData))
            {
                byte[] send2 = Encoding.ASCII.GetBytes(commandData + "\r");
                this.LogNet?.WriteDebug(this.ToString(), StringResources.Language.Send + " : " + SoftBasic.ByteToHexString(send2, ' '));
                OperateResult sendResult2 = await this.SendAsync(resultSocket.Content, send2);
                if (!sendResult2.IsSuccess)
                {
                    resultSocket.Content?.Close();
                    this.IsSocketError = true;
                    this.AlienSession?.Offline();
                    this.InteractiveLock.Leave();
                    return OperateResult.CreateFailedResult<string>(sendResult2);
                }
                send2 = (byte[])null;
                sendResult2 = (OperateResult)null;
            }
            OperateResult<byte[]> resultReceive2 = await this.ReceiveCommandLineFromSocketAsync(resultSocket.Content, (byte)13, this.ReceiveTimeOut);
            if (!resultReceive2.IsSuccess)
            {
                this.IsSocketError = true;
                this.AlienSession?.Offline();
                this.InteractiveLock.Leave();
                return OperateResult.CreateFailedResult<string>((OperateResult)resultReceive2);
            }
            string commandDataReturn = Encoding.ASCII.GetString(resultReceive2.Content);
            if (commandDataReturn.StartsWith("ERROR:"))
            {
                if (!this.isPersistentConn)
                    resultSocket.Content?.Close();
                this.InteractiveLock.Leave();
                OperateResult<byte[]> async = await this.ReceiveAsync(resultSocket.Content, 1);
                return new OperateResult<string>(commandDataReturn);
            }
            if (commandDataReturn.StartsWith("0000\r"))
            {
                if (!this.isPersistentConn)
                    resultSocket.Content?.Close();
                OperateResult<byte[]> async = await this.ReceiveAsync(resultSocket.Content, 1);
                this.InteractiveLock.Leave();
                return OperateResult.CreateSuccessResult<string>("0000");
            }
            if (!this.isPersistentConn)
                resultSocket.Content?.Close();
            this.InteractiveLock.Leave();
            return OperateResult.CreateSuccessResult<string>(commandDataReturn.Remove(commandDataReturn.Length - 1));
        }

        /// <summary>
        /// 读取机器人的报警信息<br />
        /// Read the alarm information of the robot
        /// </summary>
        /// <returns>原始的报警信息</returns>
        [EstMqttApi(Description = "Read the alarm information of the robot")]
        public OperateResult<string> ReadRALARM() => this.ReadByCommand("RALARM", (string)null);

        /// <summary>
        /// 读取机器人的坐标数据信息<br />
        /// Read the coordinate data information of the robot
        /// </summary>
        /// <returns>原始的报警信息</returns>
        [EstMqttApi(Description = "Read the coordinate data information of the robot")]
        public OperateResult<string> ReadRPOSJ() => this.ReadByCommand("RPOSJ", (string)null);

        /// <inheritdoc cref="M:ESTCore.Common.Robot.YASKAWA.YRC1000TcpNet.ReadRALARM" />
        public async Task<OperateResult<string>> ReadRALARMAsync()
        {
            OperateResult<string> operateResult = await this.ReadByCommandAsync("RALARM", (string)null);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Robot.YASKAWA.YRC1000TcpNet.ReadRPOSJ" />
        public async Task<OperateResult<string>> ReadRPOSJAsync()
        {
            OperateResult<string> operateResult = await this.ReadByCommandAsync("RPOSJ", (string)null);
            return operateResult;
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("YRC1000TcpNet Robot[{0}:{1}]", (object)this.IpAddress, (object)this.Port);
    }
}
