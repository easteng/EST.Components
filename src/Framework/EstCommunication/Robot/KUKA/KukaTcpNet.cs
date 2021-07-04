// Decompiled with JetBrains decompiler
// Type: EstCommunication.Robot.KUKA.KukaTcpNet
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.Core;
using EstCommunication.Core.Net;
using EstCommunication.Reflection;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EstCommunication.Robot.KUKA
{
  /// <summary>Kuka机器人的数据交互类，通讯支持的条件为KUKA 的 TCP通讯</summary>
  public class KukaTcpNet : NetworkDoubleBase, IRobotNet
  {
    /// <summary>
    /// 实例化一个默认的对象<br />
    /// Instantiate a default object
    /// </summary>
    public KukaTcpNet()
    {
      this.ByteTransform = (IByteTransform) new ReverseWordTransform();
      this.LogMsgFormatBinary = false;
    }

    /// <summary>
    /// 实例化一个默认的Kuka机器人对象，并指定IP地址和端口号，端口号通常为9999<br />
    /// Instantiate a default Kuka robot object and specify the IP address and port number, usually 9999
    /// </summary>
    /// <param name="ipAddress">Ip地址</param>
    /// <param name="port">端口号</param>
    public KukaTcpNet(string ipAddress, int port)
    {
      this.IpAddress = ipAddress;
      this.Port = port;
      this.ByteTransform = (IByteTransform) new ReverseWordTransform();
      this.LogMsgFormatBinary = false;
    }

    /// <inheritdoc />
    public override OperateResult<byte[]> ReadFromCoreServer(
      Socket socket,
      byte[] send,
      bool hasResponseData = true,
      bool usePackHeader = true)
    {
      this.LogNet?.WriteDebug(this.ToString(), StringResources.Language.Send + " : " + (this.LogMsgFormatBinary ? send.ToHexString(' ') : Encoding.ASCII.GetString(send)));
      OperateResult result = this.Send(socket, send);
      if (!result.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>(result);
      if (this.receiveTimeOut < 0)
        return OperateResult.CreateSuccessResult<byte[]>(new byte[0]);
      OperateResult<byte[]> operateResult = this.Receive(socket, -1, this.receiveTimeOut);
      if (!operateResult.IsSuccess)
        return operateResult;
      this.LogNet?.WriteDebug(this.ToString(), StringResources.Language.Receive + " : " + (this.LogMsgFormatBinary ? operateResult.Content.ToHexString(' ') : Encoding.ASCII.GetString(operateResult.Content)));
      return OperateResult.CreateSuccessResult<byte[]>(operateResult.Content);
    }

    /// <inheritdoc />
    public override async Task<OperateResult<byte[]>> ReadFromCoreServerAsync(
      Socket socket,
      byte[] send,
      bool hasResponseData = true,
      bool usePackHeader = true)
    {
      this.LogNet?.WriteDebug(this.ToString(), StringResources.Language.Send + " : " + (this.LogMsgFormatBinary ? send.ToHexString(' ') : Encoding.ASCII.GetString(send)));
      OperateResult sendResult = await this.SendAsync(socket, send);
      if (!sendResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>(sendResult);
      if (this.receiveTimeOut < 0)
        return OperateResult.CreateSuccessResult<byte[]>(new byte[0]);
      OperateResult<byte[]> resultReceive = await this.ReceiveAsync(socket, -1, this.receiveTimeOut);
      if (!resultReceive.IsSuccess)
        return resultReceive;
      this.LogNet?.WriteDebug(this.ToString(), StringResources.Language.Receive + " : " + (this.LogMsgFormatBinary ? resultReceive.Content.ToHexString(' ') : Encoding.ASCII.GetString(resultReceive.Content)));
      return OperateResult.CreateSuccessResult<byte[]>(resultReceive.Content);
    }

    /// <summary>
    /// 读取Kuka机器人的数据内容，根据输入的变量名称来读取<br />
    /// Read the data content of the Kuka robot according to the input variable name
    /// </summary>
    /// <param name="address">地址数据</param>
    /// <returns>带有成功标识的byte[]数组</returns>
    [EstMqttApi(ApiTopic = "ReadRobotByte", Description = "Read the data content of the Kuka robot according to the input variable name")]
    public OperateResult<byte[]> Read(string address) => ByteTransformHelper.GetResultFromOther<byte[], byte[]>(this.ReadFromCoreServer(Encoding.UTF8.GetBytes(KukaTcpNet.BuildReadCommands(address))), new Func<byte[], OperateResult<byte[]>>(this.ExtractActualData));

    /// <summary>
    /// 读取Kuka机器人的所有的数据信息，返回字符串信息，解码方式为UTF8，需要指定变量名称<br />
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
    /// 根据Kuka机器人的变量名称，写入UTF8编码的字符串数据信息<br />
    /// Writes ansi-encoded string data information based on the variable name of the Kuka robot
    /// </summary>
    /// <param name="address">变量名称</param>
    /// <param name="value">ANSI编码的字符串</param>
    /// <returns>是否成功的写入</returns>
    [EstMqttApi(ApiTopic = "WriteRobotString", Description = "Writes ansi-encoded string data information based on the variable name of the Kuka robot")]
    public OperateResult Write(string address, string value) => this.Write(new string[1]
    {
      address
    }, new string[1]{ value });

    /// <summary>
    /// 根据Kuka机器人的变量名称，写入多个UTF8编码的字符串数据信息<br />
    /// Write multiple UTF8 encoded string data information according to the variable name of the Kuka robot
    /// </summary>
    /// <param name="address">变量名称</param>
    /// <param name="value">ANSI编码的字符串</param>
    /// <returns>是否成功的写入</returns>
    [EstMqttApi(ApiTopic = "WriteRobotStrings", Description = "Write multiple UTF8 encoded string data information according to the variable name of the Kuka robot")]
    public OperateResult Write(string[] address, string[] value) => this.ReadCmd(KukaTcpNet.BuildWriteCommands(address, value));

    private OperateResult ReadCmd(string cmd)
    {
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(Encoding.UTF8.GetBytes(cmd));
      if (!operateResult.IsSuccess)
        return (OperateResult) operateResult;
      string str = Encoding.UTF8.GetString(operateResult.Content);
      return str.Contains("err") ? new OperateResult("Result contains err: " + str) : OperateResult.CreateSuccessResult();
    }

    /// <summary>
    /// 启动机器人的指定的程序<br />
    /// Start the specified program of the robot
    /// </summary>
    /// <param name="program">程序的名字</param>
    /// <returns>是否启动成功</returns>
    [EstMqttApi(Description = "Start the specified program of the robot")]
    public OperateResult StartProgram(string program) => this.ReadCmd("03" + program);

    /// <summary>
    /// 复位当前的程序<br />
    /// Reset current program
    /// </summary>
    /// <returns>复位结果</returns>
    [EstMqttApi(Description = "Reset current program")]
    public OperateResult ResetProgram() => this.ReadCmd("0601");

    /// <summary>
    /// 停止当前的程序<br />
    /// Stop current program
    /// </summary>
    /// <returns>复位结果</returns>
    [EstMqttApi(Description = "Stop current program")]
    public OperateResult StopProgram() => this.ReadCmd("0621");

    /// <inheritdoc cref="M:EstCommunication.Robot.KUKA.KukaTcpNet.Read(System.String)" />
    public async Task<OperateResult<byte[]>> ReadAsync(string address)
    {
      OperateResult<byte[]> result = await this.ReadFromCoreServerAsync(Encoding.UTF8.GetBytes(KukaTcpNet.BuildReadCommands(address)));
      return ByteTransformHelper.GetResultFromOther<byte[], byte[]>(result, new Func<byte[], OperateResult<byte[]>>(this.ExtractActualData));
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.KUKA.KukaTcpNet.ReadString(System.String)" />
    public async Task<OperateResult<string>> ReadStringAsync(string address)
    {
      OperateResult<byte[]> result = await this.ReadAsync(address);
      return ByteTransformHelper.GetSuccessResultFromOther<string, byte[]>(result, new Func<byte[], string>(Encoding.Default.GetString));
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.KUKA.KukaTcpNet.Write(System.String,System.Byte[])" />
    public async Task<OperateResult> WriteAsync(string address, byte[] value)
    {
      OperateResult operateResult = await this.WriteAsync(address, Encoding.Default.GetString(value));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.KUKA.KukaTcpNet.Write(System.String,System.String)" />
    public async Task<OperateResult> WriteAsync(string address, string value)
    {
      OperateResult operateResult = await this.WriteAsync(new string[1]
      {
        address
      }, new string[1]{ value });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.KUKA.KukaTcpNet.Write(System.String[],System.String[])" />
    public async Task<OperateResult> WriteAsync(string[] address, string[] value)
    {
      OperateResult operateResult = await this.ReadCmdAsync(KukaTcpNet.BuildWriteCommands(address, value));
      return operateResult;
    }

    private async Task<OperateResult> ReadCmdAsync(string cmd)
    {
      OperateResult<byte[]> write = await this.ReadFromCoreServerAsync(Encoding.UTF8.GetBytes(cmd));
      if (!write.IsSuccess)
        return (OperateResult) write;
      string msg = Encoding.UTF8.GetString(write.Content);
      return !msg.Contains("err") ? OperateResult.CreateSuccessResult() : new OperateResult("Result contains err: " + msg);
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.KUKA.KukaTcpNet.StartProgram(System.String)" />
    public async Task<OperateResult> StartProgramAsync(string program)
    {
      OperateResult operateResult = await this.ReadCmdAsync("03" + program);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.KUKA.KukaTcpNet.ResetProgram" />
    public async Task<OperateResult> ResetProgramAsync()
    {
      OperateResult operateResult = await this.ReadCmdAsync("0601");
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.KUKA.KukaTcpNet.StopProgram" />
    public async Task<OperateResult> StopProgramAsync()
    {
      OperateResult operateResult = await this.ReadCmdAsync("0621");
      return operateResult;
    }

    private OperateResult<byte[]> ExtractActualData(byte[] response) => OperateResult.CreateSuccessResult<byte[]>(response);

    /// <inheritdoc />
    public override string ToString() => string.Format("KukaTcpNet[{0}:{1}]", (object) this.IpAddress, (object) this.Port);

    /// <summary>构建读取变量的报文命令</summary>
    /// <param name="address">地址信息</param>
    /// <returns>报文内容</returns>
    public static string BuildReadCommands(string[] address)
    {
      if (address == null)
        return string.Empty;
      StringBuilder stringBuilder = new StringBuilder("00");
      for (int index = 0; index < address.Length; ++index)
      {
        stringBuilder.Append(address[index] ?? "");
        if (index != address.Length - 1)
          stringBuilder.Append(",");
      }
      return stringBuilder.ToString();
    }

    /// <summary>构建读取变量的报文命令</summary>
    /// <param name="address">地址信息</param>
    /// <returns>报文内容</returns>
    public static string BuildReadCommands(string address) => KukaTcpNet.BuildReadCommands(new string[1]
    {
      address
    });

    /// <summary>构建写入变量的报文命令</summary>
    /// <param name="address">地址信息</param>
    /// <param name="values">数据信息</param>
    /// <returns>字符串信息</returns>
    public static string BuildWriteCommands(string[] address, string[] values)
    {
      if (address == null || values == null)
        return string.Empty;
      if (address.Length != values.Length)
        throw new Exception(StringResources.Language.TwoParametersLengthIsNotSame);
      StringBuilder stringBuilder = new StringBuilder("01");
      for (int index = 0; index < address.Length; ++index)
      {
        stringBuilder.Append(address[index] + "=");
        stringBuilder.Append(values[index] ?? "");
        if (index != address.Length - 1)
          stringBuilder.Append(",");
      }
      return stringBuilder.ToString();
    }

    /// <summary>构建写入变量的报文命令</summary>
    /// <param name="address">地址信息</param>
    /// <param name="value">数据信息</param>
    /// <returns>字符串信息</returns>
    public static string BuildWriteCommands(string address, string value) => KukaTcpNet.BuildWriteCommands(new string[1]
    {
      address
    }, new string[1]{ value });
  }
}
