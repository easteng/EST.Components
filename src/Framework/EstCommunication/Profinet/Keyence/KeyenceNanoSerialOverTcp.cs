// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.Keyence.KeyenceNanoSerialOverTcp
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using EstCommunication.Core;
using EstCommunication.Core.Net;
using EstCommunication.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EstCommunication.Profinet.Keyence
{
  /// <summary>
  /// 基恩士KV上位链路协议的通信对象,适用于KV5000/5500/3000,KV1000,KV700,以及L20V通信模块，本类是基于tcp通信<br />
  /// The communication object of KEYENCE KV upper link protocol is suitable for KV5000/5500/3000, KV1000, KV700, and L20V communication modules. This type is based on tcp communication
  /// </summary>
  /// <remarks>
  /// 位读写的数据类型为 R,B,MR,LR,CR,VB,以及读定时器的计数器的触点，字读写的数据类型为 DM,EM,FM,ZF,W,TM,Z,AT,CM,VM 双字读写为T,C,TC,CC,TS,CS。
  /// 如果想要读写扩展的缓存器，地址示例：unit=2;1000  前面的是单元编号，后面的是偏移地址
  /// </remarks>
  /// <example>
  /// 地址示例如下：
  /// 当读取Bool的输入的格式说明如下：
  /// <list type="table">
  ///   <listheader>
  ///     <term>地址名称</term>
  ///     <term>地址代号</term>
  ///     <term>示例</term>
  ///     <term>地址范围</term>
  ///     <term>备注</term>
  ///   </listheader>
  ///   <item>
  ///     <term>继电器</term>
  ///     <term>R</term>
  ///     <term>R0,R100</term>
  ///     <term>0-59915</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>链路继电器</term>
  ///     <term>B</term>
  ///     <term>B0,B100</term>
  ///     <term>0-3FFF</term>
  ///     <term>KV5500/KV5000/KV3000</term>
  ///   </item>
  ///   <item>
  ///     <term>控制继电器</term>
  ///     <term>CR</term>
  ///     <term>CR0,CR100</term>
  ///     <term>0-3915</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>内部辅助继电器</term>
  ///     <term>MR</term>
  ///     <term>MR0,MR100</term>
  ///     <term>0-99915</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>锁存继电器</term>
  ///     <term>LR</term>
  ///     <term>LR0,LR100</term>
  ///     <term>0-99915</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>工作继电器</term>
  ///     <term>VB</term>
  ///     <term>VB0,VB100</term>
  ///     <term>0-3FFF</term>
  ///     <term>KV5500/KV5000/KV3000</term>
  ///   </item>
  ///   <item>
  ///     <term>定时器</term>
  ///     <term>T</term>
  ///     <term>T0,T100</term>
  ///     <term>0-3999</term>
  ///     <term>通断</term>
  ///   </item>
  ///   <item>
  ///     <term>计数器</term>
  ///     <term>C</term>
  ///     <term>C0,C100</term>
  ///     <term>0-3999</term>
  ///     <term>通断</term>
  ///   </item>
  ///   <item>
  ///     <term>高速计数器</term>
  ///     <term>CTH</term>
  ///     <term>CTH0,CTH1</term>
  ///     <term>0-1</term>
  ///     <term>通断</term>
  ///   </item>
  ///   <item>
  ///     <term>高速计数器比较器</term>
  ///     <term>CTC</term>
  ///     <term>CTC0,CTC1</term>
  ///     <term>0-1</term>
  ///     <term>通断</term>
  ///   </item>
  /// </list>
  /// 读取数据的地址如下：
  /// 
  /// <list type="table">
  ///   <listheader>
  ///     <term>地址名称</term>
  ///     <term>地址代号</term>
  ///     <term>示例</term>
  ///     <term>地址范围</term>
  ///     <term>备注</term>
  ///   </listheader>
  ///   <item>
  ///     <term>数据存储器</term>
  ///     <term>DM</term>
  ///     <term>DM0,DM100</term>
  ///     <term>0-65534</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>控制存储器</term>
  ///     <term>CM</term>
  ///     <term>CM0,CM100</term>
  ///     <term>0-11998</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>临时数据存储器</term>
  ///     <term>TM</term>
  ///     <term>TM0,TM100</term>
  ///     <term>0-511</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>扩展数据存储器</term>
  ///     <term>EM</term>
  ///     <term>EM0,EM100</term>
  ///     <term>0-65534</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>扩展数据存储器</term>
  ///     <term>FM</term>
  ///     <term>FM0,FM100</term>
  ///     <term>0-32766</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>变址寄存器</term>
  ///     <term>Z</term>
  ///     <term>Z1,Z5</term>
  ///     <term>1-12</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>数字微调器</term>
  ///     <term>AT</term>
  ///     <term>AT0,AT5</term>
  ///     <term>0-7</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>链路寄存器</term>
  ///     <term>W</term>
  ///     <term>W1,W5</term>
  ///     <term>0-3FFF</term>
  ///     <term>KV5500/KV5000/KV3000</term>
  ///   </item>
  ///   <item>
  ///     <term>工作寄存器</term>
  ///     <term>VM</term>
  ///     <term>VM1,VM5</term>
  ///     <term>0-59999</term>
  ///     <term>KV5500/KV5000/KV3000</term>
  ///   </item>
  ///   <item>
  ///     <term>定时器</term>
  ///     <term>T</term>
  ///     <term>T0,T100</term>
  ///     <term>0-3999</term>
  ///     <term>当前值(current value), 读int</term>
  ///   </item>
  ///   <item>
  ///     <term>计数器</term>
  ///     <term>C</term>
  ///     <term>C0,C100</term>
  ///     <term>0-3999</term>
  ///     <term>当前值(current value), 读int</term>
  ///   </item>
  /// </list>
  /// </example>
  public class KeyenceNanoSerialOverTcp : NetworkDeviceBase
  {
    /// <summary>
    /// 实例化一个默认的对象<br />
    /// Instantiate a default object
    /// </summary>
    public KeyenceNanoSerialOverTcp()
    {
      this.WordLength = (ushort) 1;
      this.ByteTransform = (IByteTransform) new RegularByteTransform();
    }

    /// <summary>
    /// 使用指定的ip地址和端口号来初始化对象<br />
    /// Initialize the object with the specified IP address and port number
    /// </summary>
    /// <param name="ipAddress">Ip地址数据</param>
    /// <param name="port">端口号</param>
    public KeyenceNanoSerialOverTcp(string ipAddress, int port = 8501)
      : this()
    {
      this.IpAddress = ipAddress;
      this.Port = port;
    }

    /// <inheritdoc />
    protected override OperateResult InitializationOnConnect(Socket socket)
    {
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(socket, KeyenceNanoSerialOverTcp.GetConnectCmd(this.Station), true, true);
      return !operateResult.IsSuccess ? (OperateResult) operateResult : OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc />
    protected override OperateResult ExtraOnDisconnect(Socket socket)
    {
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(socket, KeyenceNanoSerialOverTcp.GetDisConnectCmd(this.Station), true, true);
      return !operateResult.IsSuccess ? (OperateResult) operateResult : OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc />
    public override OperateResult<byte[]> ReadFromCoreServer(
      Socket socket,
      byte[] send,
      bool hasResponseData = true,
      bool usePackHeader = true)
    {
      this.LogNet?.WriteDebug(this.ToString(), StringResources.Language.Send + " : " + (this.LogMsgFormatBinary ? send.ToHexString(' ') : Encoding.ASCII.GetString(send)));
      OperateResult result = this.Send(socket, usePackHeader ? this.PackCommandWithHeader(send) : send);
      if (!result.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>(result);
      if (this.receiveTimeOut < 0)
        return OperateResult.CreateSuccessResult<byte[]>(new byte[0]);
      if (!hasResponseData)
        return OperateResult.CreateSuccessResult<byte[]>(new byte[0]);
      if (this.SleepTime > 0)
        Thread.Sleep(this.SleepTime);
      OperateResult<byte[]> commandLineFromSocket = this.ReceiveCommandLineFromSocket(socket, (byte) 13, (byte) 10, this.receiveTimeOut);
      if (!commandLineFromSocket.IsSuccess)
        return commandLineFromSocket;
      this.LogNet?.WriteDebug(this.ToString(), StringResources.Language.Receive + " : " + (this.LogMsgFormatBinary ? commandLineFromSocket.Content.ToHexString(' ') : Encoding.ASCII.GetString(commandLineFromSocket.Content)));
      return OperateResult.CreateSuccessResult<byte[]>(commandLineFromSocket.Content);
    }

    /// <inheritdoc />
    protected override async Task<OperateResult> InitializationOnConnectAsync(
      Socket socket)
    {
      OperateResult<byte[]> result = await this.ReadFromCoreServerAsync(socket, KeyenceNanoSerialOverTcp.GetConnectCmd(this.Station), true, true);
      OperateResult operateResult = result.IsSuccess ? OperateResult.CreateSuccessResult() : (OperateResult) result;
      result = (OperateResult<byte[]>) null;
      return operateResult;
    }

    /// <inheritdoc />
    protected override async Task<OperateResult> ExtraOnDisconnectAsync(
      Socket socket)
    {
      OperateResult<byte[]> result = await this.ReadFromCoreServerAsync(socket, KeyenceNanoSerialOverTcp.GetDisConnectCmd(this.Station), true, true);
      OperateResult operateResult = result.IsSuccess ? OperateResult.CreateSuccessResult() : (OperateResult) result;
      result = (OperateResult<byte[]>) null;
      return operateResult;
    }

    /// <inheritdoc />
    public override async Task<OperateResult<byte[]>> ReadFromCoreServerAsync(
      Socket socket,
      byte[] send,
      bool hasResponseData = true,
      bool usePackHeader = true)
    {
      this.LogNet?.WriteDebug(this.ToString(), StringResources.Language.Send + " : " + (this.LogMsgFormatBinary ? send.ToHexString(' ') : Encoding.ASCII.GetString(send)));
      OperateResult sendResult = this.Send(socket, usePackHeader ? this.PackCommandWithHeader(send) : send);
      if (!sendResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>(sendResult);
      if (this.receiveTimeOut < 0)
        return OperateResult.CreateSuccessResult<byte[]>(new byte[0]);
      if (!hasResponseData)
        return OperateResult.CreateSuccessResult<byte[]>(new byte[0]);
      if (this.SleepTime > 0)
        Thread.Sleep(this.SleepTime);
      OperateResult<byte[]> resultReceive = await this.ReceiveCommandLineFromSocketAsync(socket, (byte) 13, (byte) 10, this.receiveTimeOut);
      if (!resultReceive.IsSuccess)
        return resultReceive;
      this.LogNet?.WriteDebug(this.ToString(), StringResources.Language.Receive + " : " + (this.LogMsgFormatBinary ? resultReceive.Content.ToHexString(' ') : Encoding.ASCII.GetString(resultReceive.Content)));
      return OperateResult.CreateSuccessResult<byte[]>(resultReceive.Content);
    }

    /// <summary>
    /// 获取或设置当前的站号信息，在RS232连接模式下，设置为0，如果是RS485/RS422连接下，必须设置正确的站号<br />
    /// Get or set the current station number information. In RS232 connection mode, set it to 0.
    /// If it is RS485/RS422 connection, you must set the correct station number.
    /// </summary>
    public byte Station { get; set; }

    /// <inheritdoc />
    [EstMqttApi("ReadByteArray", "")]
    public override OperateResult<byte[]> Read(string address, ushort length)
    {
      if (address.StartsWith("unit="))
      {
        byte parameter = (byte) EstHelper.ExtractParameter(ref address, "unit", 0);
        return !ushort.TryParse(address, out ushort _) ? new OperateResult<byte[]>("Address is not right, convert ushort wrong!") : this.ReadExpansionMemory(parameter, ushort.Parse(address), length);
      }
      OperateResult<byte[]> operateResult1 = KeyenceNanoSerialOverTcp.BuildReadCommand(address, length);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult1);
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult2);
      OperateResult result = KeyenceNanoSerialOverTcp.CheckPlcReadResponse(operateResult2.Content);
      if (!result.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>(result);
      OperateResult<string, int> operateResult3 = KeyenceNanoSerialOverTcp.KvAnalysisAddress(address);
      return !operateResult3.IsSuccess ? OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult3) : KeyenceNanoSerialOverTcp.ExtractActualData(operateResult3.Content1, operateResult2.Content);
    }

    /// <inheritdoc />
    [EstMqttApi("WriteByteArray", "")]
    public override OperateResult Write(string address, byte[] value)
    {
      OperateResult<byte[]> operateResult1 = KeyenceNanoSerialOverTcp.BuildWriteCommand(address, value);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return (OperateResult) operateResult2;
      OperateResult operateResult3 = KeyenceNanoSerialOverTcp.CheckPlcWriteResponse(operateResult2.Content);
      return !operateResult3.IsSuccess ? operateResult3 : OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Keyence.KeyenceNanoSerialOverTcp.Read(System.String,System.UInt16)" />
    public override async Task<OperateResult<byte[]>> ReadAsync(
      string address,
      ushort length)
    {
      if (address.StartsWith("unit="))
      {
        byte unit = (byte) EstHelper.ExtractParameter(ref address, "unit", 0);
        if (!ushort.TryParse(address, out ushort _))
          return new OperateResult<byte[]>("Address is not right, convert ushort wrong!");
        OperateResult<byte[]> operateResult = await this.ReadExpansionMemoryAsync(unit, ushort.Parse(address), length);
        return operateResult;
      }
      OperateResult<byte[]> command = KeyenceNanoSerialOverTcp.BuildReadCommand(address, length);
      if (!command.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) command);
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) read);
      OperateResult ackResult = KeyenceNanoSerialOverTcp.CheckPlcReadResponse(read.Content);
      if (!ackResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>(ackResult);
      OperateResult<string, int> addressResult = KeyenceNanoSerialOverTcp.KvAnalysisAddress(address);
      return addressResult.IsSuccess ? KeyenceNanoSerialOverTcp.ExtractActualData(addressResult.Content1, read.Content) : OperateResult.CreateFailedResult<byte[]>((OperateResult) addressResult);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Keyence.KeyenceNanoSerialOverTcp.Write(System.String,System.Byte[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      byte[] value)
    {
      OperateResult<byte[]> command = KeyenceNanoSerialOverTcp.BuildWriteCommand(address, value);
      if (!command.IsSuccess)
        return (OperateResult) command;
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      if (!read.IsSuccess)
        return (OperateResult) read;
      OperateResult checkResult = KeyenceNanoSerialOverTcp.CheckPlcWriteResponse(read.Content);
      return checkResult.IsSuccess ? OperateResult.CreateSuccessResult() : checkResult;
    }

    /// <inheritdoc />
    [EstMqttApi("ReadBoolArray", "")]
    public override OperateResult<bool[]> ReadBool(string address, ushort length)
    {
      OperateResult<byte[]> operateResult1 = KeyenceNanoSerialOverTcp.BuildReadCommand(address, length);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult1);
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult2);
      OperateResult result = KeyenceNanoSerialOverTcp.CheckPlcReadResponse(operateResult2.Content);
      if (!result.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>(result);
      OperateResult<string, int> operateResult3 = KeyenceNanoSerialOverTcp.KvAnalysisAddress(address);
      return !operateResult3.IsSuccess ? OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult3) : KeyenceNanoSerialOverTcp.ExtractActualBoolData(operateResult3.Content1, operateResult2.Content);
    }

    /// <inheritdoc />
    [EstMqttApi("WriteBool", "")]
    public override OperateResult Write(string address, bool value)
    {
      OperateResult<byte[]> operateResult1 = KeyenceNanoSerialOverTcp.BuildWriteCommand(address, value);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return (OperateResult) operateResult2;
      OperateResult operateResult3 = KeyenceNanoSerialOverTcp.CheckPlcWriteResponse(operateResult2.Content);
      return !operateResult3.IsSuccess ? operateResult3 : OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc />
    [EstMqttApi("WriteBoolArray", "")]
    public override OperateResult Write(string address, bool[] value)
    {
      OperateResult<byte[]> operateResult1 = KeyenceNanoSerialOverTcp.BuildWriteCommand(address, value);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return (OperateResult) operateResult2;
      OperateResult operateResult3 = KeyenceNanoSerialOverTcp.CheckPlcWriteResponse(operateResult2.Content);
      return !operateResult3.IsSuccess ? operateResult3 : OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Keyence.KeyenceNanoSerialOverTcp.ReadBool(System.String,System.UInt16)" />
    public override async Task<OperateResult<bool[]>> ReadBoolAsync(
      string address,
      ushort length)
    {
      OperateResult<byte[]> command = KeyenceNanoSerialOverTcp.BuildReadCommand(address, length);
      if (!command.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) command);
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) read);
      OperateResult ackResult = KeyenceNanoSerialOverTcp.CheckPlcReadResponse(read.Content);
      if (!ackResult.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>(ackResult);
      OperateResult<string, int> addressResult = KeyenceNanoSerialOverTcp.KvAnalysisAddress(address);
      return addressResult.IsSuccess ? KeyenceNanoSerialOverTcp.ExtractActualBoolData(addressResult.Content1, read.Content) : OperateResult.CreateFailedResult<bool[]>((OperateResult) addressResult);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Keyence.KeyenceNanoSerialOverTcp.Write(System.String,System.Boolean)" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      bool value)
    {
      OperateResult<byte[]> command = KeyenceNanoSerialOverTcp.BuildWriteCommand(address, value);
      if (!command.IsSuccess)
        return (OperateResult) command;
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      if (!read.IsSuccess)
        return (OperateResult) read;
      OperateResult checkResult = KeyenceNanoSerialOverTcp.CheckPlcWriteResponse(read.Content);
      return checkResult.IsSuccess ? OperateResult.CreateSuccessResult() : checkResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Keyence.KeyenceNanoSerialOverTcp.Write(System.String,System.Boolean[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      bool[] value)
    {
      OperateResult<byte[]> command = KeyenceNanoSerialOverTcp.BuildWriteCommand(address, value);
      if (!command.IsSuccess)
        return (OperateResult) command;
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      if (!read.IsSuccess)
        return (OperateResult) read;
      OperateResult checkResult = KeyenceNanoSerialOverTcp.CheckPlcWriteResponse(read.Content);
      return checkResult.IsSuccess ? OperateResult.CreateSuccessResult() : checkResult;
    }

    /// <summary>
    /// 查询PLC的型号信息<br />
    /// Query PLC model information
    /// </summary>
    /// <returns>包含型号的结果对象</returns>
    [EstMqttApi("查询PLC的型号信息")]
    public OperateResult<KeyencePLCS> ReadPlcType() => KeyenceNanoSerialOverTcp.ReadPlcTypeHelper(new Func<byte[], OperateResult<byte[]>>(((NetworkDoubleBase) this).ReadFromCoreServer));

    /// <summary>
    /// 读取当前PLC的模式，如果是0，代表 PROG模式或者梯形图未登录，如果为1，代表RUN模式<br />
    /// Read the current PLC mode, if it is 0, it means PROG mode or the ladder diagram is not registered, if it is 1, it means RUN mode
    /// </summary>
    /// <returns>包含模式的结果对象</returns>
    [EstMqttApi("读取当前PLC的模式，如果是0，代表 PROG模式或者梯形图未登录，如果为1，代表RUN模式")]
    public OperateResult<int> ReadPlcMode() => KeyenceNanoSerialOverTcp.ReadPlcModeHelper(new Func<byte[], OperateResult<byte[]>>(((NetworkDoubleBase) this).ReadFromCoreServer));

    /// <summary>
    /// 设置PLC的时间<br />
    /// Set PLC time
    /// </summary>
    /// <param name="dateTime">时间数据</param>
    /// <returns>是否设置成功</returns>
    [EstMqttApi("设置PLC的时间")]
    public OperateResult SetPlcDateTime(DateTime dateTime) => KeyenceNanoSerialOverTcp.SetPlcDateTimeHelper(new Func<byte[], OperateResult<byte[]>>(((NetworkDoubleBase) this).ReadFromCoreServer), dateTime);

    /// <summary>读取指定软元件的注释信息</summary>
    /// <param name="address">软元件的地址</param>
    /// <returns>软元件的注释信息</returns>
    [EstMqttApi("读取指定软元件的注释信息")]
    public OperateResult<string> ReadAddressAnnotation(string address) => KeyenceNanoSerialOverTcp.ReadAddressAnnotationHelper(new Func<byte[], OperateResult<byte[]>>(((NetworkDoubleBase) this).ReadFromCoreServer), address);

    /// <summary>
    /// 从扩展单元缓冲存储器连续读取指定个数的数据，单位为字<br />
    /// Continuously read the specified number of data from the expansion unit buffer memory, the unit is word
    /// </summary>
    /// <param name="unit">单元编号</param>
    /// <param name="address">偏移地址</param>
    /// <param name="length">读取的长度，单位为字</param>
    /// <returns>包含是否成功的原始字节数组</returns>
    [EstMqttApi("从扩展单元缓冲存储器连续读取指定个数的数据，单位为字")]
    public OperateResult<byte[]> ReadExpansionMemory(
      byte unit,
      ushort address,
      ushort length)
    {
      return KeyenceNanoSerialOverTcp.ReadExpansionMemoryHelper(new Func<byte[], OperateResult<byte[]>>(((NetworkDoubleBase) this).ReadFromCoreServer), unit, address, length);
    }

    /// <summary>
    /// 将原始字节数据写入到扩展的缓冲存储器，需要指定单元编号，偏移地址，写入的数据<br />
    /// To write the original byte data to the extended buffer memory, you need to specify the unit number, offset address, and write data
    /// </summary>
    /// <param name="unit">单元编号</param>
    /// <param name="address">偏移地址</param>
    /// <param name="value">等待写入的原始字节数据</param>
    /// <returns>是否写入成功的结果对象</returns>
    [EstMqttApi("将原始字节数据写入到扩展的缓冲存储器，需要指定单元编号，偏移地址，写入的数据")]
    public OperateResult WriteExpansionMemory(byte unit, ushort address, byte[] value) => KeyenceNanoSerialOverTcp.WriteExpansionMemoryHelper(new Func<byte[], OperateResult<byte[]>>(((NetworkDoubleBase) this).ReadFromCoreServer), unit, address, value);

    /// <inheritdoc cref="M:EstCommunication.Profinet.Keyence.KeyenceNanoSerialOverTcp.ReadPlcType" />
    public async Task<OperateResult<KeyencePLCS>> ReadPlcTypeAsync()
    {
      OperateResult<KeyencePLCS> operateResult = await KeyenceNanoSerialOverTcp.ReadPlcTypeAsyncHelper(new Func<byte[], Task<OperateResult<byte[]>>>(((NetworkDoubleBase) this).ReadFromCoreServerAsync));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Keyence.KeyenceNanoSerialOverTcp.ReadPlcMode" />
    public async Task<OperateResult<int>> ReadPlcModeAsync()
    {
      OperateResult<int> operateResult = await KeyenceNanoSerialOverTcp.ReadPlcModeAsyncHelper(new Func<byte[], Task<OperateResult<byte[]>>>(((NetworkDoubleBase) this).ReadFromCoreServerAsync));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Keyence.KeyenceNanoSerialOverTcp.SetPlcDateTime(System.DateTime)" />
    public async Task<OperateResult> SetPlcDateTimeAsync(DateTime dateTime)
    {
      OperateResult operateResult = await KeyenceNanoSerialOverTcp.SetPlcDateTimeAsyncHelper(new Func<byte[], Task<OperateResult<byte[]>>>(((NetworkDoubleBase) this).ReadFromCoreServerAsync), dateTime);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Keyence.KeyenceNanoSerialOverTcp.ReadAddressAnnotation(System.String)" />
    public async Task<OperateResult<string>> ReadAddressAnnotationAsync(
      string address)
    {
      OperateResult<string> operateResult = await KeyenceNanoSerialOverTcp.ReadAddressAnnotationAsyncHelper(new Func<byte[], Task<OperateResult<byte[]>>>(((NetworkDoubleBase) this).ReadFromCoreServerAsync), address);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Keyence.KeyenceNanoSerialOverTcp.ReadExpansionMemory(System.Byte,System.UInt16,System.UInt16)" />
    public async Task<OperateResult<byte[]>> ReadExpansionMemoryAsync(
      byte unit,
      ushort address,
      ushort length)
    {
      OperateResult<byte[]> operateResult = await KeyenceNanoSerialOverTcp.ReadExpansionMemoryAsyncHelper(new Func<byte[], Task<OperateResult<byte[]>>>(((NetworkDoubleBase) this).ReadFromCoreServerAsync), unit, address, length);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Keyence.KeyenceNanoSerialOverTcp.WriteExpansionMemory(System.Byte,System.UInt16,System.Byte[])" />
    public async Task<OperateResult> WriteExpansionMemoryAsync(
      byte unit,
      ushort address,
      byte[] value)
    {
      OperateResult operateResult = await KeyenceNanoSerialOverTcp.WriteExpansionMemoryAsyncHelper(new Func<byte[], Task<OperateResult<byte[]>>>(((NetworkDoubleBase) this).ReadFromCoreServerAsync), unit, address, value);
      return operateResult;
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("KeyenceNanoSerialOverTcp[{0}:{1}]", (object) this.IpAddress, (object) this.Port);

    internal static OperateResult<KeyencePLCS> ReadPlcTypeHelper(
      Func<byte[], OperateResult<byte[]>> funRead)
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return new OperateResult<KeyencePLCS>(StringResources.Language.InsufficientPrivileges);
      OperateResult<byte[]> operateResult1 = funRead(Encoding.ASCII.GetBytes("?K\r"));
      if (!operateResult1.IsSuccess)
        return operateResult1.ConvertFailed<KeyencePLCS>();
      OperateResult operateResult2 = KeyenceNanoSerialOverTcp.CheckPlcReadResponse(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return operateResult2.ConvertFailed<KeyencePLCS>();
      string str1 = Encoding.ASCII.GetString(operateResult1.Content.RemoveLast<byte>(2));
      string str2 = str1;
      if (str2 == "48" || str2 == "49")
        return OperateResult.CreateSuccessResult<KeyencePLCS>(KeyencePLCS.KV700);
      if (str2 == "50")
        return OperateResult.CreateSuccessResult<KeyencePLCS>(KeyencePLCS.KV1000);
      if (str2 == "51")
        return OperateResult.CreateSuccessResult<KeyencePLCS>(KeyencePLCS.KV3000);
      if (str2 == "52")
        return OperateResult.CreateSuccessResult<KeyencePLCS>(KeyencePLCS.KV5000);
      return str2 == "53" ? OperateResult.CreateSuccessResult<KeyencePLCS>(KeyencePLCS.KV5500) : new OperateResult<KeyencePLCS>("Unknow type:" + str1);
    }

    internal static async Task<OperateResult<KeyencePLCS>> ReadPlcTypeAsyncHelper(
      Func<byte[], Task<OperateResult<byte[]>>> funRead)
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return new OperateResult<KeyencePLCS>(StringResources.Language.InsufficientPrivileges);
      OperateResult<byte[]> read = await funRead(Encoding.ASCII.GetBytes("?K\r"));
      if (!read.IsSuccess)
        return read.ConvertFailed<KeyencePLCS>();
      OperateResult check = KeyenceNanoSerialOverTcp.CheckPlcReadResponse(read.Content);
      if (!check.IsSuccess)
        return check.ConvertFailed<KeyencePLCS>();
      string type = Encoding.ASCII.GetString(read.Content.RemoveLast<byte>(2));
      string str = type;
      return str == "48" || str == "49" ? OperateResult.CreateSuccessResult<KeyencePLCS>(KeyencePLCS.KV700) : (str == "50" ? OperateResult.CreateSuccessResult<KeyencePLCS>(KeyencePLCS.KV1000) : (str == "51" ? OperateResult.CreateSuccessResult<KeyencePLCS>(KeyencePLCS.KV3000) : (str == "52" ? OperateResult.CreateSuccessResult<KeyencePLCS>(KeyencePLCS.KV5000) : (str == "53" ? OperateResult.CreateSuccessResult<KeyencePLCS>(KeyencePLCS.KV5500) : new OperateResult<KeyencePLCS>("Unknow type:" + type)))));
    }

    internal static OperateResult<int> ReadPlcModeHelper(
      Func<byte[], OperateResult<byte[]>> funRead)
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return new OperateResult<int>(StringResources.Language.InsufficientPrivileges);
      OperateResult<byte[]> operateResult1 = funRead(Encoding.ASCII.GetBytes("?M\r"));
      if (!operateResult1.IsSuccess)
        return operateResult1.ConvertFailed<int>();
      OperateResult operateResult2 = KeyenceNanoSerialOverTcp.CheckPlcReadResponse(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return operateResult2.ConvertFailed<int>();
      return Encoding.ASCII.GetString(operateResult1.Content.RemoveLast<byte>(2)) == "0" ? OperateResult.CreateSuccessResult<int>(0) : OperateResult.CreateSuccessResult<int>(1);
    }

    internal static async Task<OperateResult<int>> ReadPlcModeAsyncHelper(
      Func<byte[], Task<OperateResult<byte[]>>> funRead)
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return new OperateResult<int>(StringResources.Language.InsufficientPrivileges);
      OperateResult<byte[]> read = await funRead(Encoding.ASCII.GetBytes("?M\r"));
      if (!read.IsSuccess)
        return read.ConvertFailed<int>();
      OperateResult check = KeyenceNanoSerialOverTcp.CheckPlcReadResponse(read.Content);
      if (!check.IsSuccess)
        return check.ConvertFailed<int>();
      string type = Encoding.ASCII.GetString(read.Content.RemoveLast<byte>(2));
      return !(type == "0") ? OperateResult.CreateSuccessResult<int>(1) : OperateResult.CreateSuccessResult<int>(0);
    }

    internal static OperateResult SetPlcDateTimeHelper(
      Func<byte[], OperateResult<byte[]>> funRead,
      DateTime dateTime)
    {
      OperateResult<byte[]> operateResult = funRead(Encoding.ASCII.GetBytes(string.Format("WRT {0:D2} {1:D2} {2:D2} ", (object) (dateTime.Year - 2000), (object) dateTime.Month, (object) dateTime.Day) + string.Format("{0:D2} {1:D2} {2:D2} {3}\r", (object) dateTime.Hour, (object) dateTime.Minute, (object) dateTime.Second, (object) (int) dateTime.DayOfWeek)));
      return !operateResult.IsSuccess ? (OperateResult) operateResult.ConvertFailed<int>() : KeyenceNanoSerialOverTcp.CheckPlcWriteResponse(operateResult.Content);
    }

    internal static async Task<OperateResult> SetPlcDateTimeAsyncHelper(
      Func<byte[], Task<OperateResult<byte[]>>> funRead,
      DateTime dateTime)
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return (OperateResult) new OperateResult<int>(StringResources.Language.InsufficientPrivileges);
      OperateResult<byte[]> read = await funRead(Encoding.ASCII.GetBytes(string.Format("WRT {0:D2} {1:D2} {2:D2} ", (object) (dateTime.Year - 2000), (object) dateTime.Month, (object) dateTime.Day) + string.Format("{0:D2} {1:D2} {2:D2} {3}\r", (object) dateTime.Hour, (object) dateTime.Minute, (object) dateTime.Second, (object) (int) dateTime.DayOfWeek)));
      return read.IsSuccess ? KeyenceNanoSerialOverTcp.CheckPlcWriteResponse(read.Content) : (OperateResult) read;
    }

    internal static OperateResult<string> ReadAddressAnnotationHelper(
      Func<byte[], OperateResult<byte[]>> funRead,
      string address)
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return new OperateResult<string>(StringResources.Language.InsufficientPrivileges);
      OperateResult<byte[]> operateResult1 = funRead(Encoding.ASCII.GetBytes("RDC " + address + "\r"));
      if (!operateResult1.IsSuccess)
        return operateResult1.ConvertFailed<string>();
      OperateResult operateResult2 = KeyenceNanoSerialOverTcp.CheckPlcReadResponse(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return operateResult2.ConvertFailed<string>();
      return OperateResult.CreateSuccessResult<string>(Encoding.ASCII.GetString(operateResult1.Content.RemoveLast<byte>(2)).Trim(' '));
    }

    internal static async Task<OperateResult<string>> ReadAddressAnnotationAsyncHelper(
      Func<byte[], Task<OperateResult<byte[]>>> funRead,
      string address)
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return new OperateResult<string>(StringResources.Language.InsufficientPrivileges);
      OperateResult<byte[]> read = await funRead(Encoding.ASCII.GetBytes("RDC " + address + "\r"));
      if (!read.IsSuccess)
        return read.ConvertFailed<string>();
      OperateResult check = KeyenceNanoSerialOverTcp.CheckPlcReadResponse(read.Content);
      if (!check.IsSuccess)
        return check.ConvertFailed<string>();
      return OperateResult.CreateSuccessResult<string>(Encoding.ASCII.GetString(read.Content.RemoveLast<byte>(2)).Trim(' '));
    }

    internal static OperateResult<byte[]> ReadExpansionMemoryHelper(
      Func<byte[], OperateResult<byte[]>> funRead,
      byte unit,
      ushort address,
      ushort length)
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
      OperateResult<byte[]> operateResult1 = funRead(Encoding.ASCII.GetBytes(string.Format("URD {0} {1}.U {2}\r", (object) unit, (object) address, (object) length)));
      if (!operateResult1.IsSuccess)
        return operateResult1.ConvertFailed<byte[]>();
      OperateResult operateResult2 = KeyenceNanoSerialOverTcp.CheckPlcReadResponse(operateResult1.Content);
      return !operateResult2.IsSuccess ? operateResult2.ConvertFailed<byte[]>() : KeyenceNanoSerialOverTcp.ExtractActualData("DM", operateResult1.Content);
    }

    internal static async Task<OperateResult<byte[]>> ReadExpansionMemoryAsyncHelper(
      Func<byte[], Task<OperateResult<byte[]>>> funRead,
      byte unit,
      ushort address,
      ushort length)
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
      OperateResult<byte[]> read = await funRead(Encoding.ASCII.GetBytes(string.Format("URD {0} {1}.U {2}\r", (object) unit, (object) address, (object) length)));
      if (!read.IsSuccess)
        return read.ConvertFailed<byte[]>();
      OperateResult check = KeyenceNanoSerialOverTcp.CheckPlcReadResponse(read.Content);
      return check.IsSuccess ? KeyenceNanoSerialOverTcp.ExtractActualData("DM", read.Content) : check.ConvertFailed<byte[]>();
    }

    internal static OperateResult WriteExpansionMemoryHelper(
      Func<byte[], OperateResult<byte[]>> funRead,
      byte unit,
      ushort address,
      byte[] value)
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return (OperateResult) new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
      OperateResult<byte[]> operateResult = funRead(KeyenceNanoSerialOverTcp.BuildWriteExpansionMemoryCommand(unit, address, value).Content);
      return !operateResult.IsSuccess ? (OperateResult) operateResult.ConvertFailed<byte[]>() : KeyenceNanoSerialOverTcp.CheckPlcWriteResponse(operateResult.Content);
    }

    internal static async Task<OperateResult> WriteExpansionMemoryAsyncHelper(
      Func<byte[], Task<OperateResult<byte[]>>> funRead,
      byte unit,
      ushort address,
      byte[] value)
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return (OperateResult) new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
      OperateResult<byte[]> read = await funRead(KeyenceNanoSerialOverTcp.BuildWriteExpansionMemoryCommand(unit, address, value).Content);
      return read.IsSuccess ? KeyenceNanoSerialOverTcp.CheckPlcWriteResponse(read.Content) : (OperateResult) read.ConvertFailed<byte[]>();
    }

    /// <summary>
    /// 连接PLC的命令报文<br />
    /// Command message to connect to PLC
    /// </summary>
    public static byte[] GetConnectCmd(byte station) => Encoding.ASCII.GetBytes(string.Format("CR {0:D2}\r", (object) station));

    /// <summary>
    /// 断开PLC连接的命令报文<br />
    /// Command message to disconnect PLC
    /// </summary>
    public static byte[] GetDisConnectCmd(byte station) => Encoding.ASCII.GetBytes(string.Format("CQ {0:D2}\r", (object) station));

    /// <summary>获取当前的地址类型是字数据的倍数关系</summary>
    /// <param name="type">地址的类型</param>
    /// <returns>倍数关系</returns>
    public static int GetWordAddressMultiple(string type)
    {
      if (type == "CTH" || type == "CTC" || (type == "C" || type == "T") || (type == "TS" || type == "TC" || (type == "CS" || type == "CC")) || type == "AT")
        return 2;
      return type == "DM" || type == "CM" || (type == "TM" || type == "EM") || (type == "FM" || type == "Z" || (type == "W" || type == "ZF")) || type == "VM" ? 1 : 1;
    }

    /// <summary>
    /// 建立读取PLC数据的指令，需要传入地址数据，以及读取的长度，地址示例参照类的说明文档<br />
    /// To create a command to read PLC data, you need to pass in the address data, and the length of the read. For an example of the address, refer to the class documentation
    /// </summary>
    /// <param name="address">软元件地址</param>
    /// <param name="length">读取长度</param>
    /// <returns>是否建立成功</returns>
    public static OperateResult<byte[]> BuildReadCommand(string address, ushort length)
    {
      OperateResult<string, int> operateResult = KeyenceNanoSerialOverTcp.KvAnalysisAddress(address);
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult);
      if (length > (ushort) 1)
        length /= (ushort) KeyenceNanoSerialOverTcp.GetWordAddressMultiple(operateResult.Content1);
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("RDS");
      stringBuilder.Append(" ");
      stringBuilder.Append(operateResult.Content1);
      stringBuilder.Append(operateResult.Content2.ToString());
      stringBuilder.Append(" ");
      stringBuilder.Append(length.ToString());
      stringBuilder.Append("\r");
      return OperateResult.CreateSuccessResult<byte[]>(Encoding.ASCII.GetBytes(stringBuilder.ToString()));
    }

    /// <summary>
    /// 建立写入PLC数据的指令，需要传入地址数据，以及写入的数据信息，地址示例参照类的说明文档<br />
    /// To create a command to write PLC data, you need to pass in the address data and the written data information. For an example of the address, refer to the class documentation
    /// </summary>
    /// <param name="address">软元件地址</param>
    /// <param name="value">转换后的数据</param>
    /// <returns>是否成功的信息</returns>
    public static OperateResult<byte[]> BuildWriteCommand(string address, byte[] value)
    {
      OperateResult<string, int> operateResult = KeyenceNanoSerialOverTcp.KvAnalysisAddress(address);
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult);
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("WRS");
      stringBuilder.Append(" ");
      stringBuilder.Append(operateResult.Content1);
      stringBuilder.Append(operateResult.Content2);
      stringBuilder.Append(" ");
      int num = value.Length / (KeyenceNanoSerialOverTcp.GetWordAddressMultiple(operateResult.Content1) * 2);
      stringBuilder.Append(num.ToString());
      for (int index = 0; index < num; ++index)
      {
        stringBuilder.Append(" ");
        stringBuilder.Append(BitConverter.ToUInt16(value, index * KeyenceNanoSerialOverTcp.GetWordAddressMultiple(operateResult.Content1) * 2));
      }
      stringBuilder.Append("\r");
      return OperateResult.CreateSuccessResult<byte[]>(Encoding.ASCII.GetBytes(stringBuilder.ToString()));
    }

    /// <summary>
    /// 构建写入扩展单元缓冲寄存器的报文命令，需要传入单元编号，地址，写入的数据，实际写入的数据格式才有无符号的方式<br />
    /// To construct a message command to write to the buffer register of the expansion unit, the unit number, address,
    /// and data to be written need to be passed in, and the format of the actually written data is unsigned.
    /// </summary>
    /// <param name="unit">单元编号0~48</param>
    /// <param name="address">地址0~32767</param>
    /// <param name="value">写入的数据信息，单次交互最大256个字</param>
    /// <returns>包含是否成功的报文对象</returns>
    public static OperateResult<byte[]> BuildWriteExpansionMemoryCommand(
      byte unit,
      ushort address,
      byte[] value)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("UWR");
      stringBuilder.Append(" ");
      stringBuilder.Append(unit);
      stringBuilder.Append(" ");
      stringBuilder.Append(address);
      stringBuilder.Append(".U");
      stringBuilder.Append(" ");
      int num = value.Length / 2;
      stringBuilder.Append(num.ToString());
      for (int index = 0; index < num; ++index)
      {
        stringBuilder.Append(" ");
        stringBuilder.Append(BitConverter.ToUInt16(value, index * 2));
      }
      stringBuilder.Append("\r");
      return OperateResult.CreateSuccessResult<byte[]>(Encoding.ASCII.GetBytes(stringBuilder.ToString()));
    }

    /// <summary>
    /// 建立写入bool数据的指令，针对地址类型为 R,CR,MR,LR<br />
    /// Create instructions to write bool data, address type is R, CR, MR, LR
    /// </summary>
    /// <param name="address">软元件地址</param>
    /// <param name="value">转换后的数据</param>
    /// <returns>是否成功的信息</returns>
    public static OperateResult<byte[]> BuildWriteCommand(string address, bool value)
    {
      OperateResult<string, int> operateResult = KeyenceNanoSerialOverTcp.KvAnalysisAddress(address);
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult);
      StringBuilder stringBuilder = new StringBuilder();
      if (value)
        stringBuilder.Append("ST");
      else
        stringBuilder.Append("RS");
      stringBuilder.Append(" ");
      stringBuilder.Append(operateResult.Content1);
      stringBuilder.Append(operateResult.Content2);
      stringBuilder.Append("\r");
      return OperateResult.CreateSuccessResult<byte[]>(Encoding.ASCII.GetBytes(stringBuilder.ToString()));
    }

    /// <summary>
    /// 批量写入数据位到plc地址，针对地址格式为 R,B,CR,MR,LR,VB<br />
    /// Write data bits in batches to the plc address, and the address format is R, B, CR, MR, LR, VB
    /// </summary>
    /// <param name="address">PLC的地址</param>
    /// <param name="value">等待写入的bool数组</param>
    /// <returns>写入bool数组的命令报文</returns>
    public static OperateResult<byte[]> BuildWriteCommand(string address, bool[] value)
    {
      OperateResult<string, int> operateResult = KeyenceNanoSerialOverTcp.KvAnalysisAddress(address);
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult);
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("WRS");
      stringBuilder.Append(" ");
      stringBuilder.Append(operateResult.Content1);
      stringBuilder.Append(operateResult.Content2);
      stringBuilder.Append(" ");
      stringBuilder.Append(value.Length.ToString());
      for (int index = 0; index < value.Length; ++index)
      {
        stringBuilder.Append(" ");
        stringBuilder.Append(value[index] ? "1" : "0");
      }
      stringBuilder.Append("\r");
      return OperateResult.CreateSuccessResult<byte[]>(Encoding.ASCII.GetBytes(stringBuilder.ToString()));
    }

    private static string GetErrorText(string err)
    {
      if (err.StartsWith("E0"))
        return StringResources.Language.KeyenceNanoE0;
      if (err.StartsWith("E1"))
        return StringResources.Language.KeyenceNanoE1;
      if (err.StartsWith("E2"))
        return StringResources.Language.KeyenceNanoE2;
      if (err.StartsWith("E4"))
        return StringResources.Language.KeyenceNanoE4;
      if (err.StartsWith("E5"))
        return StringResources.Language.KeyenceNanoE5;
      return err.StartsWith("E6") ? StringResources.Language.KeyenceNanoE6 : StringResources.Language.UnknownError + " " + err;
    }

    /// <summary>
    /// 校验读取返回数据状态，主要返回的第一个字节是不是E<br />
    /// Check the status of the data returned from reading, whether the first byte returned is E
    /// </summary>
    /// <param name="ack">反馈信息</param>
    /// <returns>是否成功的信息</returns>
    public static OperateResult CheckPlcReadResponse(byte[] ack)
    {
      if (ack.Length == 0)
        return new OperateResult(StringResources.Language.MelsecFxReceiveZero);
      if (ack[0] == (byte) 69)
        return new OperateResult(KeyenceNanoSerialOverTcp.GetErrorText(Encoding.ASCII.GetString(ack)));
      return ack[ack.Length - 1] != (byte) 10 && ack[ack.Length - 2] != (byte) 13 ? new OperateResult(StringResources.Language.MelsecFxAckWrong + " Actual: " + SoftBasic.ByteToHexString(ack, ' ')) : OperateResult.CreateSuccessResult();
    }

    /// <summary>
    /// 校验写入返回数据状态，检测返回的数据是不是OK<br />
    /// Verify the status of the returned data written and check whether the returned data is OK
    /// </summary>
    /// <param name="ack">反馈信息</param>
    /// <returns>是否成功的信息</returns>
    public static OperateResult CheckPlcWriteResponse(byte[] ack)
    {
      if (ack.Length == 0)
        return new OperateResult(StringResources.Language.MelsecFxReceiveZero);
      return ack[0] == (byte) 79 && ack[1] == (byte) 75 ? OperateResult.CreateSuccessResult() : new OperateResult(KeyenceNanoSerialOverTcp.GetErrorText(Encoding.ASCII.GetString(ack)));
    }

    /// <summary>
    /// 从PLC反馈的数据进行提炼Bool操作<br />
    /// Refine Bool operation from data fed back from PLC
    /// </summary>
    /// <param name="addressType">地址的数据类型</param>
    /// <param name="response">PLC反馈的真实数据</param>
    /// <returns>数据提炼后的真实数据</returns>
    public static OperateResult<bool[]> ExtractActualBoolData(
      string addressType,
      byte[] response)
    {
      try
      {
        if (string.IsNullOrEmpty(addressType))
          addressType = "R";
        string str = Encoding.Default.GetString(response.RemoveLast<byte>(2));
        if (addressType == "R" || addressType == "CR" || (addressType == "MR" || addressType == "LR") || addressType == "B" || addressType == "VB")
          return OperateResult.CreateSuccessResult<bool[]>(((IEnumerable<string>) str.Split(new char[1]
          {
            ' '
          }, StringSplitOptions.RemoveEmptyEntries)).Select<string, bool>((Func<string, bool>) (m => m == "1")).ToArray<bool>());
        if (!(addressType == "T") && !(addressType == "C") && !(addressType == "CTH") && !(addressType == "CTC"))
          return new OperateResult<bool[]>(StringResources.Language.NotSupportedDataType);
        return OperateResult.CreateSuccessResult<bool[]>(((IEnumerable<string>) str.Split(new char[1]
        {
          ' '
        }, StringSplitOptions.RemoveEmptyEntries)).Select<string, bool>((Func<string, bool>) (m => m.StartsWith("1"))).ToArray<bool>());
      }
      catch (Exception ex)
      {
        OperateResult<bool[]> operateResult = new OperateResult<bool[]>();
        operateResult.Message = "Extract Msg：" + ex.Message + Environment.NewLine + "Data: " + SoftBasic.ByteToHexString(response);
        return operateResult;
      }
    }

    /// <summary>
    /// 从PLC反馈的数据进行提炼操作<br />
    /// Refining operation from data fed back from PLC
    /// </summary>
    /// <param name="addressType">地址的数据类型</param>
    /// <param name="response">PLC反馈的真实数据</param>
    /// <returns>数据提炼后的真实数据</returns>
    public static OperateResult<byte[]> ExtractActualData(
      string addressType,
      byte[] response)
    {
      try
      {
        if (string.IsNullOrEmpty(addressType))
          addressType = "R";
        string[] strArray = Encoding.Default.GetString(response.RemoveLast<byte>(2)).Split(new char[1]
        {
          ' '
        }, StringSplitOptions.RemoveEmptyEntries);
        if (addressType == "DM" || addressType == "EM" || (addressType == "FM" || addressType == "ZF") || (addressType == "W" || addressType == "TM" || (addressType == "Z" || addressType == "CM")) || addressType == "VM")
        {
          byte[] numArray = new byte[strArray.Length * 2];
          for (int index = 0; index < strArray.Length; ++index)
            BitConverter.GetBytes(ushort.Parse(strArray[index])).CopyTo((Array) numArray, index * 2);
          return OperateResult.CreateSuccessResult<byte[]>(numArray);
        }
        if (addressType == "AT" || addressType == "TC" || (addressType == "CC" || addressType == "TS") || addressType == "CS")
        {
          byte[] numArray = new byte[strArray.Length * 4];
          for (int index = 0; index < strArray.Length; ++index)
            BitConverter.GetBytes(uint.Parse(strArray[index])).CopyTo((Array) numArray, index * 4);
          return OperateResult.CreateSuccessResult<byte[]>(numArray);
        }
        if (!(addressType == "T") && !(addressType == "C") && !(addressType == "CTH") && !(addressType == "CTC"))
          return new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType);
        byte[] numArray1 = new byte[strArray.Length * 4];
        for (int index = 0; index < strArray.Length; ++index)
          BitConverter.GetBytes(uint.Parse(strArray[index].Split(new char[1]
          {
            ','
          }, StringSplitOptions.RemoveEmptyEntries)[1])).CopyTo((Array) numArray1, index * 4);
        return OperateResult.CreateSuccessResult<byte[]>(numArray1);
      }
      catch (Exception ex)
      {
        OperateResult<byte[]> operateResult = new OperateResult<byte[]>();
        operateResult.Message = "Extract Msg：" + ex.Message + Environment.NewLine + "Data: " + SoftBasic.ByteToHexString(response);
        return operateResult;
      }
    }

    /// <summary>
    /// 解析数据地址成不同的Keyence地址类型<br />
    /// Parse data addresses into different keyence address types
    /// </summary>
    /// <param name="address">数据地址</param>
    /// <returns>地址结果对象</returns>
    public static OperateResult<string, int> KvAnalysisAddress(string address)
    {
      try
      {
        if (address.StartsWith("CTH") || address.StartsWith("cth"))
          return OperateResult.CreateSuccessResult<string, int>("CTH", int.Parse(address.Substring(3)));
        if (address.StartsWith("CTC") || address.StartsWith("ctc"))
          return OperateResult.CreateSuccessResult<string, int>("CTC", int.Parse(address.Substring(3)));
        if (address.StartsWith("CR") || address.StartsWith("cr"))
          return OperateResult.CreateSuccessResult<string, int>("CR", int.Parse(address.Substring(2)));
        if (address.StartsWith("MR") || address.StartsWith("mr"))
          return OperateResult.CreateSuccessResult<string, int>("MR", int.Parse(address.Substring(2)));
        if (address.StartsWith("LR") || address.StartsWith("lr"))
          return OperateResult.CreateSuccessResult<string, int>("LR", int.Parse(address.Substring(2)));
        if (address.StartsWith("DM") || address.StartsWith("DM"))
          return OperateResult.CreateSuccessResult<string, int>("DM", int.Parse(address.Substring(2)));
        if (address.StartsWith("CM") || address.StartsWith("cm"))
          return OperateResult.CreateSuccessResult<string, int>("CM", int.Parse(address.Substring(2)));
        if (address.StartsWith("W") || address.StartsWith("w"))
          return OperateResult.CreateSuccessResult<string, int>("W", int.Parse(address.Substring(1)));
        if (address.StartsWith("TM") || address.StartsWith("tm"))
          return OperateResult.CreateSuccessResult<string, int>("TM", int.Parse(address.Substring(2)));
        if (address.StartsWith("VM") || address.StartsWith("vm"))
          return OperateResult.CreateSuccessResult<string, int>("VM", int.Parse(address.Substring(2)));
        if (address.StartsWith("EM") || address.StartsWith("em"))
          return OperateResult.CreateSuccessResult<string, int>("EM", int.Parse(address.Substring(2)));
        if (address.StartsWith("FM") || address.StartsWith("fm"))
          return OperateResult.CreateSuccessResult<string, int>("EM", int.Parse(address.Substring(2)));
        if (address.StartsWith("ZF") || address.StartsWith("zf"))
          return OperateResult.CreateSuccessResult<string, int>("ZF", int.Parse(address.Substring(2)));
        if (address.StartsWith("AT") || address.StartsWith("at"))
          return OperateResult.CreateSuccessResult<string, int>("AT", int.Parse(address.Substring(2)));
        if (address.StartsWith("TS") || address.StartsWith("ts"))
          return OperateResult.CreateSuccessResult<string, int>("TS", int.Parse(address.Substring(2)));
        if (address.StartsWith("TC") || address.StartsWith("tc"))
          return OperateResult.CreateSuccessResult<string, int>("TC", int.Parse(address.Substring(2)));
        if (address.StartsWith("CC") || address.StartsWith("cc"))
          return OperateResult.CreateSuccessResult<string, int>("CC", int.Parse(address.Substring(2)));
        if (address.StartsWith("CS") || address.StartsWith("cs"))
          return OperateResult.CreateSuccessResult<string, int>("CS", int.Parse(address.Substring(2)));
        if (address.StartsWith("Z") || address.StartsWith("z"))
          return OperateResult.CreateSuccessResult<string, int>("Z", int.Parse(address.Substring(1)));
        if (address.StartsWith("R") || address.StartsWith("r"))
          return OperateResult.CreateSuccessResult<string, int>("", int.Parse(address.Substring(1)));
        if (address.StartsWith("B") || address.StartsWith("b"))
          return OperateResult.CreateSuccessResult<string, int>("B", int.Parse(address.Substring(1)));
        if (address.StartsWith("T") || address.StartsWith("t"))
          return OperateResult.CreateSuccessResult<string, int>("T", int.Parse(address.Substring(1)));
        return address.StartsWith("C") || address.StartsWith("c") ? OperateResult.CreateSuccessResult<string, int>("C", int.Parse(address.Substring(1))) : throw new Exception(StringResources.Language.NotSupportedDataType);
      }
      catch (Exception ex)
      {
        return new OperateResult<string, int>(ex.Message);
      }
    }
  }
}
