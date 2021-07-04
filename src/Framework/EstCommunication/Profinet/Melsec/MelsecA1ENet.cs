// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.Melsec.MelsecA1ENet
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.Core;
using EstCommunication.Core.IMessage;
using EstCommunication.Core.Net;
using EstCommunication.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EstCommunication.Profinet.Melsec
{
  /// <summary>
  /// 三菱PLC通讯协议，采用A兼容1E帧协议实现，使用二进制码通讯，请根据实际型号来进行选取<br />
  /// Mitsubishi PLC communication protocol, implemented using A compatible 1E frame protocol, using binary code communication, please choose according to the actual model
  /// </summary>
  /// <remarks>
  /// 本类适用于的PLC列表
  /// <list type="number">
  /// <item>FX3U(C) PLC   测试人sandy_liao</item>
  /// </list>
  /// 数据地址支持的格式如下：
  /// <list type="table">
  ///   <listheader>
  ///     <term>地址名称</term>
  ///     <term>地址代号</term>
  ///     <term>示例</term>
  ///     <term>地址进制</term>
  ///     <term>字操作</term>
  ///     <term>位操作</term>
  ///     <term>备注</term>
  ///   </listheader>
  ///   <item>
  ///     <term>内部继电器</term>
  ///     <term>M</term>
  ///     <term>M100,M200</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>输入继电器</term>
  ///     <term>X</term>
  ///     <term>X10,X20</term>
  ///     <term>动态</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term>地址前面带0就是8进制比如X010，不带则是16进制，X40</term>
  ///   </item>
  ///   <item>
  ///     <term>输出继电器</term>
  ///     <term>Y</term>
  ///     <term>Y10,Y20</term>
  ///     <term>动态</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term>地址前面带0就是8进制比如Y020，不带则是16进制，Y40</term>
  ///   </item>
  ///   <item>
  ///     <term>步进继电器</term>
  ///     <term>S</term>
  ///     <term>S100,S200</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>报警器</term>
  ///     <term>F</term>
  ///     <term>F100,F200</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>链接继电器</term>
  ///     <term>B</term>
  ///     <term>B1A0,B2A0</term>
  ///     <term>16</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>定时器触点</term>
  ///     <term>TS</term>
  ///     <term>TS0,TS100</term>
  ///     <term>10</term>
  ///     <term>×</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>定时器线圈</term>
  ///     <term>TC</term>
  ///     <term>TC0,TC100</term>
  ///     <term>10</term>
  ///     <term>×</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>定时器当前值</term>
  ///     <term>TN</term>
  ///     <term>TN0,TN100</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>计数器器触点</term>
  ///     <term>CS</term>
  ///     <term>CS0,CS100</term>
  ///     <term>10</term>
  ///     <term>×</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>计数器线圈</term>
  ///     <term>CC</term>
  ///     <term>CC0,CC100</term>
  ///     <term>10</term>
  ///     <term>×</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>计数器当前值</term>
  ///     <term>CN</term>
  ///     <term>CN0,CN100</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>数据寄存器</term>
  ///     <term>D</term>
  ///     <term>D1000,D2000</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>链接寄存器</term>
  ///     <term>W</term>
  ///     <term>W0,W1A0</term>
  ///     <term>16</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>文件寄存器</term>
  ///     <term>R</term>
  ///     <term>R100,R200</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term></term>
  ///   </item>
  /// </list>
  /// <see cref="M:EstCommunication.Profinet.Melsec.MelsecA1ENet.ReadBool(System.String,System.UInt16)" /> 方法一次读取的最多点数是256点。
  /// <note type="important">本通讯类由CKernal推送，感谢</note>
  /// </remarks>
  public class MelsecA1ENet : NetworkDeviceBase
  {
    /// <summary>
    /// 实例化一个默认的对象<br />
    /// Instantiate a default object
    /// </summary>
    public MelsecA1ENet()
    {
      this.WordLength = (ushort) 1;
      this.ByteTransform = (IByteTransform) new RegularByteTransform();
    }

    /// <summary>
    /// 指定ip地址和端口来来实例化一个默认的对象<br />
    /// Specify the IP address and port to instantiate a default object
    /// </summary>
    /// <param name="ipAddress">PLC的Ip地址</param>
    /// <param name="port">PLC的端口</param>
    public MelsecA1ENet(string ipAddress, int port)
    {
      this.WordLength = (ushort) 1;
      this.IpAddress = ipAddress;
      this.Port = port;
      this.ByteTransform = (IByteTransform) new RegularByteTransform();
    }

    /// <inheritdoc />
    protected override INetMessage GetNewNetMessage() => (INetMessage) new MelsecA1EBinaryMessage();

    /// <summary>
    /// PLC编号，默认为0xFF<br />
    /// PLC number, default is 0xFF
    /// </summary>
    public byte PLCNumber { get; set; } = byte.MaxValue;

    /// <inheritdoc />
    [EstMqttApi("ReadByteArray", "")]
    public override OperateResult<byte[]> Read(string address, ushort length)
    {
      OperateResult<byte[]> operateResult1 = MelsecA1ENet.BuildReadCommand(address, length, false, this.PLCNumber);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult1);
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult2);
      OperateResult result = MelsecA1ENet.CheckResponseLegal(operateResult2.Content);
      return !result.IsSuccess ? OperateResult.CreateFailedResult<byte[]>(result) : MelsecA1ENet.ExtractActualData(operateResult2.Content, false);
    }

    /// <inheritdoc />
    [EstMqttApi("WriteByteArray", "")]
    public override OperateResult Write(string address, byte[] value)
    {
      OperateResult<byte[]> operateResult1 = MelsecA1ENet.BuildWriteWordCommand(address, value, this.PLCNumber);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return (OperateResult) operateResult2;
      OperateResult result = MelsecA1ENet.CheckResponseLegal(operateResult2.Content);
      return !result.IsSuccess ? (OperateResult) OperateResult.CreateFailedResult<byte[]>(result) : OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc />
    public override async Task<OperateResult<byte[]>> ReadAsync(
      string address,
      ushort length)
    {
      OperateResult<byte[]> command = MelsecA1ENet.BuildReadCommand(address, length, false, this.PLCNumber);
      if (!command.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) command);
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) read);
      OperateResult check = MelsecA1ENet.CheckResponseLegal(read.Content);
      return check.IsSuccess ? MelsecA1ENet.ExtractActualData(read.Content, false) : OperateResult.CreateFailedResult<byte[]>(check);
    }

    /// <inheritdoc />
    public override async Task<OperateResult> WriteAsync(
      string address,
      byte[] value)
    {
      OperateResult<byte[]> command = MelsecA1ENet.BuildWriteWordCommand(address, value, this.PLCNumber);
      if (!command.IsSuccess)
        return (OperateResult) command;
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      if (!read.IsSuccess)
        return (OperateResult) read;
      OperateResult check = MelsecA1ENet.CheckResponseLegal(read.Content);
      return check.IsSuccess ? OperateResult.CreateSuccessResult() : (OperateResult) OperateResult.CreateFailedResult<byte[]>(check);
    }

    /// <summary>
    /// 批量读取<see cref="T:System.Boolean" />数组信息，需要指定地址和长度，地址示例M100，S100，B1A，如果是X,Y, X017就是8进制地址，Y10就是16进制地址。<br />
    /// Batch read <see cref="T:System.Boolean" /> array information, need to specify the address and length, return <see cref="T:System.Boolean" /> array.
    /// Examples of addresses M100, S100, B1A, if it is X, Y, X017 is an octal address, Y10 is a hexadecimal address.
    /// </summary>
    /// <remarks>
    /// 根据协议的规范，最多读取256长度的bool数组信息，如果需要读取更长的bool信息，需要按字为单位进行读取的操作。
    /// </remarks>
    /// <param name="address">数据地址</param>
    /// <param name="length">数据长度</param>
    /// <returns>带有成功标识的byte[]数组</returns>
    [EstMqttApi("ReadBoolArray", "")]
    public override OperateResult<bool[]> ReadBool(string address, ushort length)
    {
      OperateResult<byte[]> operateResult1 = MelsecA1ENet.BuildReadCommand(address, length, true, this.PLCNumber);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult1);
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult2);
      OperateResult result = MelsecA1ENet.CheckResponseLegal(operateResult2.Content);
      if (!result.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>(result);
      OperateResult<byte[]> actualData = MelsecA1ENet.ExtractActualData(operateResult2.Content, true);
      return !actualData.IsSuccess ? OperateResult.CreateFailedResult<bool[]>((OperateResult) actualData) : OperateResult.CreateSuccessResult<bool[]>(((IEnumerable<byte>) actualData.Content).Select<byte, bool>((Func<byte, bool>) (m => m == (byte) 1)).Take<bool>((int) length).ToArray<bool>());
    }

    /// <summary>
    /// 批量写入<see cref="T:System.Boolean" />数组数据，返回是否成功，地址示例M100，S100，B1A，如果是X,Y, X017就是8进制地址，Y10就是16进制地址。<br />
    /// Batch write <see cref="T:System.Boolean" /> array data, return whether the write was successful.
    /// Examples of addresses M100, S100, B1A, if it is X, Y, X017 is an octal address, Y10 is a hexadecimal address.
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="value">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    [EstMqttApi("WriteBoolArray", "")]
    public override OperateResult Write(string address, bool[] value)
    {
      OperateResult<byte[]> operateResult1 = MelsecA1ENet.BuildWriteBoolCommand(address, value, this.PLCNumber);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      return !operateResult2.IsSuccess ? (OperateResult) operateResult2 : MelsecA1ENet.CheckResponseLegal(operateResult2.Content);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecA1ENet.ReadBool(System.String,System.UInt16)" />
    public override async Task<OperateResult<bool[]>> ReadBoolAsync(
      string address,
      ushort length)
    {
      OperateResult<byte[]> command = MelsecA1ENet.BuildReadCommand(address, length, true, this.PLCNumber);
      if (!command.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) command);
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) read);
      OperateResult check = MelsecA1ENet.CheckResponseLegal(read.Content);
      if (!check.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>(check);
      OperateResult<byte[]> extract = MelsecA1ENet.ExtractActualData(read.Content, true);
      return extract.IsSuccess ? OperateResult.CreateSuccessResult<bool[]>(((IEnumerable<byte>) extract.Content).Select<byte, bool>((Func<byte, bool>) (m => m == (byte) 1)).Take<bool>((int) length).ToArray<bool>()) : OperateResult.CreateFailedResult<bool[]>((OperateResult) extract);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecA1ENet.Write(System.String,System.Boolean[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      bool[] values)
    {
      OperateResult<byte[]> command = MelsecA1ENet.BuildWriteBoolCommand(address, values, this.PLCNumber);
      if (!command.IsSuccess)
        return (OperateResult) command;
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      return read.IsSuccess ? MelsecA1ENet.CheckResponseLegal(read.Content) : (OperateResult) read;
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("MelsecA1ENet[{0}:{1}]", (object) this.IpAddress, (object) this.Port);

    /// <summary>根据类型地址长度确认需要读取的指令头</summary>
    /// <param name="address">起始地址</param>
    /// <param name="length">长度</param>
    /// <param name="isBit">指示是否按照位成批的读出</param>
    /// <param name="plcNumber">PLC编号</param>
    /// <returns>带有成功标志的指令数据</returns>
    public static OperateResult<byte[]> BuildReadCommand(
      string address,
      ushort length,
      bool isBit,
      byte plcNumber)
    {
      OperateResult<MelsecA1EDataType, int> operateResult = MelsecHelper.McA1EAnalysisAddress(address);
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult);
      return OperateResult.CreateSuccessResult<byte[]>(new byte[12]
      {
        isBit ? (byte) 0 : (byte) 1,
        plcNumber,
        (byte) 10,
        (byte) 0,
        BitConverter.GetBytes(operateResult.Content2)[0],
        BitConverter.GetBytes(operateResult.Content2)[1],
        BitConverter.GetBytes(operateResult.Content2)[2],
        BitConverter.GetBytes(operateResult.Content2)[3],
        operateResult.Content1.DataCode[1],
        operateResult.Content1.DataCode[0],
        (byte) ((uint) length % 256U),
        (byte) 0
      });
    }

    /// <summary>根据类型地址以及需要写入的数据来生成指令头</summary>
    /// <param name="address">起始地址</param>
    /// <param name="value">数据值</param>
    /// <param name="plcNumber">PLC编号</param>
    /// <returns>带有成功标志的指令数据</returns>
    public static OperateResult<byte[]> BuildWriteWordCommand(
      string address,
      byte[] value,
      byte plcNumber)
    {
      OperateResult<MelsecA1EDataType, int> operateResult = MelsecHelper.McA1EAnalysisAddress(address);
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult);
      byte[] numArray = new byte[12 + value.Length];
      numArray[0] = (byte) 3;
      numArray[1] = plcNumber;
      numArray[2] = (byte) 10;
      numArray[3] = (byte) 0;
      numArray[4] = BitConverter.GetBytes(operateResult.Content2)[0];
      numArray[5] = BitConverter.GetBytes(operateResult.Content2)[1];
      numArray[6] = BitConverter.GetBytes(operateResult.Content2)[2];
      numArray[7] = BitConverter.GetBytes(operateResult.Content2)[3];
      numArray[8] = operateResult.Content1.DataCode[1];
      numArray[9] = operateResult.Content1.DataCode[0];
      numArray[10] = BitConverter.GetBytes(value.Length / 2)[0];
      numArray[11] = BitConverter.GetBytes(value.Length / 2)[1];
      Array.Copy((Array) value, 0, (Array) numArray, 12, value.Length);
      return OperateResult.CreateSuccessResult<byte[]>(numArray);
    }

    /// <summary>根据类型地址以及需要写入的数据来生成指令头</summary>
    /// <param name="address">起始地址</param>
    /// <param name="value">数据值</param>
    /// <param name="plcNumber">PLC编号</param>
    /// <returns>带有成功标志的指令数据</returns>
    public static OperateResult<byte[]> BuildWriteBoolCommand(
      string address,
      bool[] value,
      byte plcNumber)
    {
      OperateResult<MelsecA1EDataType, int> operateResult = MelsecHelper.McA1EAnalysisAddress(address);
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult);
      byte[] byteData = MelsecHelper.TransBoolArrayToByteData(value);
      byte[] numArray = new byte[12 + byteData.Length];
      numArray[0] = (byte) 2;
      numArray[1] = plcNumber;
      numArray[2] = (byte) 10;
      numArray[3] = (byte) 0;
      numArray[4] = BitConverter.GetBytes(operateResult.Content2)[0];
      numArray[5] = BitConverter.GetBytes(operateResult.Content2)[1];
      numArray[6] = BitConverter.GetBytes(operateResult.Content2)[2];
      numArray[7] = BitConverter.GetBytes(operateResult.Content2)[3];
      numArray[8] = operateResult.Content1.DataCode[1];
      numArray[9] = operateResult.Content1.DataCode[0];
      numArray[10] = BitConverter.GetBytes(value.Length)[0];
      numArray[11] = BitConverter.GetBytes(value.Length)[1];
      Array.Copy((Array) byteData, 0, (Array) numArray, 12, byteData.Length);
      return OperateResult.CreateSuccessResult<byte[]>(numArray);
    }

    /// <summary>检测反馈的消息是否合法</summary>
    /// <param name="response">接收的报文</param>
    /// <returns>是否成功</returns>
    public static OperateResult CheckResponseLegal(byte[] response)
    {
      if (response.Length < 2)
        return new OperateResult(StringResources.Language.ReceiveDataLengthTooShort);
      if (response[1] == (byte) 0)
        return OperateResult.CreateSuccessResult();
      return response[1] == (byte) 91 ? new OperateResult((int) response[2], StringResources.Language.MelsecPleaseReferToManualDocument) : new OperateResult((int) response[1], StringResources.Language.MelsecPleaseReferToManualDocument);
    }

    /// <summary>从PLC反馈的数据中提取出实际的数据内容，需要传入反馈数据，是否位读取</summary>
    /// <param name="response">反馈的数据内容</param>
    /// <param name="isBit">是否位读取</param>
    /// <returns>解析后的结果对象</returns>
    public static OperateResult<byte[]> ExtractActualData(byte[] response, bool isBit)
    {
      if (!isBit)
        return OperateResult.CreateSuccessResult<byte[]>(response.RemoveBegin<byte>(2));
      byte[] numArray = new byte[(response.Length - 2) * 2];
      for (int index = 2; index < response.Length; ++index)
      {
        if (((int) response[index] & 16) == 16)
          numArray[(index - 2) * 2] = (byte) 1;
        if (((int) response[index] & 1) == 1)
          numArray[(index - 2) * 2 + 1] = (byte) 1;
      }
      return OperateResult.CreateSuccessResult<byte[]>(numArray);
    }
  }
}
