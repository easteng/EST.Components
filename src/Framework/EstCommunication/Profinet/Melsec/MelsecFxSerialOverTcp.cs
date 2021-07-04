// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.Melsec.MelsecFxSerialOverTcp
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using EstCommunication.Core;
using EstCommunication.Core.Net;
using EstCommunication.Reflection;
using System;
using System.Text;
using System.Threading.Tasks;

namespace EstCommunication.Profinet.Melsec
{
  /// <summary>三菱串口协议的网络版</summary>
  /// <remarks>
  /// 字读写地址支持的列表如下：
  /// <list type="table">
  ///   <listheader>
  ///     <term>地址名称</term>
  ///     <term>地址代号</term>
  ///     <term>示例</term>
  ///     <term>地址范围</term>
  ///     <term>地址进制</term>
  ///     <term>备注</term>
  ///   </listheader>
  ///   <item>
  ///     <term>数据寄存器</term>
  ///     <term>D</term>
  ///     <term>D100,D200</term>
  ///     <term>D0-D511,D8000-D8255</term>
  ///     <term>10</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>定时器的值</term>
  ///     <term>TN</term>
  ///     <term>TN10,TN20</term>
  ///     <term>TN0-TN255</term>
  ///     <term>10</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>计数器的值</term>
  ///     <term>CN</term>
  ///     <term>CN10,CN20</term>
  ///     <term>CN0-CN199,CN200-CN255</term>
  ///     <term>10</term>
  ///     <term></term>
  ///   </item>
  /// </list>
  /// 位地址支持的列表如下：
  /// <list type="table">
  ///   <listheader>
  ///     <term>地址名称</term>
  ///     <term>地址代号</term>
  ///     <term>示例</term>
  ///     <term>地址范围</term>
  ///     <term>地址进制</term>
  ///     <term>备注</term>
  ///   </listheader>
  ///   <item>
  ///     <term>内部继电器</term>
  ///     <term>M</term>
  ///     <term>M100,M200</term>
  ///     <term>M0-M1023,M8000-M8255</term>
  ///     <term>10</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>输入继电器</term>
  ///     <term>X</term>
  ///     <term>X1,X20</term>
  ///     <term>X0-X177</term>
  ///     <term>8</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>输出继电器</term>
  ///     <term>Y</term>
  ///     <term>Y10,Y20</term>
  ///     <term>Y0-Y177</term>
  ///     <term>8</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>步进继电器</term>
  ///     <term>S</term>
  ///     <term>S100,S200</term>
  ///     <term>S0-S999</term>
  ///     <term>10</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>定时器触点</term>
  ///     <term>TS</term>
  ///     <term>TS10,TS20</term>
  ///     <term>TS0-TS255</term>
  ///     <term>10</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>定时器线圈</term>
  ///     <term>TC</term>
  ///     <term>TC10,TC20</term>
  ///     <term>TC0-TC255</term>
  ///     <term>10</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>计数器触点</term>
  ///     <term>CS</term>
  ///     <term>CS10,CS20</term>
  ///     <term>CS0-CS255</term>
  ///     <term>10</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>计数器线圈</term>
  ///     <term>CC</term>
  ///     <term>CC10,CC20</term>
  ///     <term>CC0-CC255</term>
  ///     <term>10</term>
  ///     <term></term>
  ///   </item>
  /// </list>
  /// </remarks>
  /// <example>
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecFxSerial.cs" region="Usage" title="简单的使用" />
  /// </example>
  public class MelsecFxSerialOverTcp : NetworkDeviceBase
  {
    /// <summary>
    /// 实例化网络版的三菱的串口协议的通讯对象<br />
    /// Instantiate the communication object of Mitsubishi's serial protocol on the network
    /// </summary>
    public MelsecFxSerialOverTcp()
    {
      this.WordLength = (ushort) 1;
      this.ByteTransform = (IByteTransform) new RegularByteTransform();
      this.IsNewVersion = true;
      this.ByteTransform.IsStringReverseByteWord = true;
      this.SleepTime = 20;
    }

    /// <summary>
    /// 指定ip地址及端口号来实例化三菱的串口协议的通讯对象<br />
    /// Specify the IP address and port number to instantiate the communication object of Mitsubishi's serial protocol
    /// </summary>
    /// <param name="ipAddress">Ip地址</param>
    /// <param name="port">端口号</param>
    public MelsecFxSerialOverTcp(string ipAddress, int port)
      : this()
    {
      this.IpAddress = ipAddress;
      this.Port = port;
    }

    /// <summary>
    /// 当前的编程口协议是否为新版，默认为新版，如果无法读取，切换旧版再次尝试<br />
    /// Whether the current programming port protocol is the new version, the default is the new version,
    /// if it cannot be read, switch to the old version and try again
    /// </summary>
    public bool IsNewVersion { get; set; }

    /// <inheritdoc />
    /// <example>
    /// 假设起始地址为D100，D100存储了温度，100.6℃值为1006，D101存储了压力，1.23Mpa值为123，D102，D103存储了产量计数，读取如下：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecFxSerial.cs" region="ReadExample2" title="Read示例" />
    /// 以下是读取不同类型数据的示例
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecFxSerial.cs" region="ReadExample1" title="Read示例" />
    /// </example>
    [EstMqttApi("ReadByteArray", "")]
    public override OperateResult<byte[]> Read(string address, ushort length) => MelsecFxSerialOverTcp.ReadHelper(address, length, new Func<byte[], OperateResult<byte[]>>(((NetworkDoubleBase) this).ReadFromCoreServer), this.IsNewVersion);

    /// <inheritdoc />
    /// <example>
    /// 假设起始地址为D100，D100存储了温度，100.6℃值为1006，D101存储了压力，1.23Mpa值为123，D102，D103存储了产量计数，写入如下：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecFxSerial.cs" region="WriteExample2" title="Write示例" />
    /// 以下是读取不同类型数据的示例
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecFxSerial.cs" region="WriteExample1" title="Write示例" />
    /// </example>
    [EstMqttApi("WriteByteArray", "")]
    public override OperateResult Write(string address, byte[] value) => MelsecFxSerialOverTcp.WriteHelper(address, value, new Func<byte[], OperateResult<byte[]>>(((NetworkDoubleBase) this).ReadFromCoreServer), this.IsNewVersion);

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecFxSerialOverTcp.Read(System.String,System.UInt16)" />
    public override async Task<OperateResult<byte[]>> ReadAsync(
      string address,
      ushort length)
    {
      OperateResult<byte[]> command = MelsecFxSerialOverTcp.BuildReadWordCommand(address, length, this.IsNewVersion);
      if (!command.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) command);
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) read);
      OperateResult ackResult = MelsecFxSerialOverTcp.CheckPlcReadResponse(read.Content);
      return ackResult.IsSuccess ? MelsecFxSerialOverTcp.ExtractActualData(read.Content) : OperateResult.CreateFailedResult<byte[]>(ackResult);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecFxSerialOverTcp.Write(System.String,System.Byte[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      byte[] value)
    {
      OperateResult<byte[]> command = MelsecFxSerialOverTcp.BuildWriteWordCommand(address, value, this.IsNewVersion);
      if (!command.IsSuccess)
        return (OperateResult) command;
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      if (!read.IsSuccess)
        return (OperateResult) read;
      OperateResult checkResult = MelsecFxSerialOverTcp.CheckPlcWriteResponse(read.Content);
      return checkResult.IsSuccess ? OperateResult.CreateSuccessResult() : checkResult;
    }

    /// <summary>
    /// 从三菱PLC中批量读取位软元件，返回读取结果，该读取地址最好从0，16，32...等开始读取，这样可以读取比较长得数据数组<br />
    /// Read bit devices from Mitsubishi PLC in batches and return the read result. The read address should preferably be read from 0, 16, 32, etc., so that you can read the longer data array.
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="length">读取的长度</param>
    /// <returns>带成功标志的结果数据对象</returns>
    /// <example>
    ///  <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecFxSerial.cs" region="ReadBool" title="Bool类型示例" />
    /// </example>
    [EstMqttApi("ReadBoolArray", "")]
    public override OperateResult<bool[]> ReadBool(string address, ushort length) => MelsecFxSerialOverTcp.ReadBoolHelper(address, length, new Func<byte[], OperateResult<byte[]>>(((NetworkDoubleBase) this).ReadFromCoreServer));

    /// <inheritdoc />
    [EstMqttApi("WriteBool", "")]
    public override OperateResult Write(string address, bool value) => MelsecFxSerialOverTcp.WriteHelper(address, value, new Func<byte[], OperateResult<byte[]>>(((NetworkDoubleBase) this).ReadFromCoreServer));

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecFxSerialOverTcp.ReadBool(System.String,System.UInt16)" />
    public override async Task<OperateResult<bool[]>> ReadBoolAsync(
      string address,
      ushort length)
    {
      OperateResult<byte[], int> command = MelsecFxSerialOverTcp.BuildReadBoolCommand(address, length);
      if (!command.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) command);
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content1);
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) read);
      OperateResult ackResult = MelsecFxSerialOverTcp.CheckPlcReadResponse(read.Content);
      return ackResult.IsSuccess ? MelsecFxSerialOverTcp.ExtractActualBoolData(read.Content, command.Content2, (int) length) : OperateResult.CreateFailedResult<bool[]>(ackResult);
    }

    /// <inheritdoc />
    public override async Task<OperateResult> WriteAsync(
      string address,
      bool value)
    {
      OperateResult<byte[]> command = MelsecFxSerialOverTcp.BuildWriteBoolPacket(address, value);
      if (!command.IsSuccess)
        return (OperateResult) command;
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      if (!read.IsSuccess)
        return (OperateResult) read;
      OperateResult checkResult = MelsecFxSerialOverTcp.CheckPlcWriteResponse(read.Content);
      return checkResult.IsSuccess ? OperateResult.CreateSuccessResult() : checkResult;
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("MelsecFxSerialOverTcp[{0}:{1}]", (object) this.IpAddress, (object) this.Port);

    /// <summary>从三菱PLC中读取想要的数据，返回读取结果</summary>
    /// <param name="address">读取地址，，支持的类型参考文档说明</param>
    /// <param name="length">读取的数据长度</param>
    /// <param name="readCore">指定的通道信息</param>
    /// <param name="isNewVersion">是否是新版的串口访问类</param>
    /// <returns>带成功标志的结果数据对象</returns>
    /// <example>
    /// 假设起始地址为D100，D100存储了温度，100.6℃值为1006，D101存储了压力，1.23Mpa值为123，D102，D103存储了产量计数，读取如下：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecFxSerial.cs" region="ReadExample2" title="Read示例" />
    /// 以下是读取不同类型数据的示例
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecFxSerial.cs" region="ReadExample1" title="Read示例" />
    /// </example>
    public static OperateResult<byte[]> ReadHelper(
      string address,
      ushort length,
      Func<byte[], OperateResult<byte[]>> readCore,
      bool isNewVersion)
    {
      OperateResult<byte[]> operateResult1 = MelsecFxSerialOverTcp.BuildReadWordCommand(address, length, isNewVersion);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult1);
      OperateResult<byte[]> operateResult2 = readCore(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult2);
      OperateResult result = MelsecFxSerialOverTcp.CheckPlcReadResponse(operateResult2.Content);
      return !result.IsSuccess ? OperateResult.CreateFailedResult<byte[]>(result) : MelsecFxSerialOverTcp.ExtractActualData(operateResult2.Content);
    }

    /// <summary>
    /// 从三菱PLC中批量读取位软元件，返回读取结果，该读取地址最好从0，16，32...等开始读取，这样可以读取比较长得数据数组
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="length">读取的长度</param>
    /// <param name="readCore">指定的通道信息</param>
    /// <returns>带成功标志的结果数据对象</returns>
    /// <example>
    ///  <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecFxSerial.cs" region="ReadBool" title="Bool类型示例" />
    /// </example>
    public static OperateResult<bool[]> ReadBoolHelper(
      string address,
      ushort length,
      Func<byte[], OperateResult<byte[]>> readCore)
    {
      OperateResult<byte[], int> operateResult1 = MelsecFxSerialOverTcp.BuildReadBoolCommand(address, length);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult1);
      OperateResult<byte[]> operateResult2 = readCore(operateResult1.Content1);
      if (!operateResult2.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult2);
      OperateResult result = MelsecFxSerialOverTcp.CheckPlcReadResponse(operateResult2.Content);
      return !result.IsSuccess ? OperateResult.CreateFailedResult<bool[]>(result) : MelsecFxSerialOverTcp.ExtractActualBoolData(operateResult2.Content, operateResult1.Content2, (int) length);
    }

    /// <summary>向PLC写入数据，数据格式为原始的字节类型</summary>
    /// <param name="address">初始地址，支持的类型参考文档说明</param>
    /// <param name="value">原始的字节数据</param>
    /// <param name="readCore">指定的通道信息</param>
    /// <param name="isNewVersion">是否是新版的串口访问类</param>
    /// <example>
    /// 假设起始地址为D100，D100存储了温度，100.6℃值为1006，D101存储了压力，1.23Mpa值为123，D102，D103存储了产量计数，写入如下：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecFxSerial.cs" region="WriteExample2" title="Write示例" />
    /// 以下是读取不同类型数据的示例
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecFxSerial.cs" region="WriteExample1" title="Write示例" />
    /// </example>
    /// <returns>是否写入成功的结果对象</returns>
    public static OperateResult WriteHelper(
      string address,
      byte[] value,
      Func<byte[], OperateResult<byte[]>> readCore,
      bool isNewVersion)
    {
      OperateResult<byte[]> operateResult1 = MelsecFxSerialOverTcp.BuildWriteWordCommand(address, value, isNewVersion);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = readCore(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return (OperateResult) operateResult2;
      OperateResult operateResult3 = MelsecFxSerialOverTcp.CheckPlcWriteResponse(operateResult2.Content);
      return !operateResult3.IsSuccess ? operateResult3 : OperateResult.CreateSuccessResult();
    }

    /// <summary>强制写入位数据的通断，支持的类型参考文档说明</summary>
    /// <param name="address">地址信息</param>
    /// <param name="value">是否为通</param>
    /// <param name="readCore">指定的通道信息</param>
    /// <returns>是否写入成功的结果对象</returns>
    public static OperateResult WriteHelper(
      string address,
      bool value,
      Func<byte[], OperateResult<byte[]>> readCore)
    {
      OperateResult<byte[]> operateResult1 = MelsecFxSerialOverTcp.BuildWriteBoolPacket(address, value);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = readCore(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return (OperateResult) operateResult2;
      OperateResult operateResult3 = MelsecFxSerialOverTcp.CheckPlcWriteResponse(operateResult2.Content);
      return !operateResult3.IsSuccess ? operateResult3 : OperateResult.CreateSuccessResult();
    }

    /// <summary>检查PLC返回的读取数据是否是正常的</summary>
    /// <param name="ack">Plc反馈的数据信息</param>
    /// <returns>检查结果</returns>
    public static OperateResult CheckPlcReadResponse(byte[] ack)
    {
      if (ack.Length == 0)
        return new OperateResult(StringResources.Language.MelsecFxReceiveZero);
      if (ack[0] == (byte) 21)
        return new OperateResult(StringResources.Language.MelsecFxAckNagative + " Actual: " + SoftBasic.ByteToHexString(ack, ' '));
      if (ack[0] != (byte) 2)
        return new OperateResult(StringResources.Language.MelsecFxAckWrong + ack[0].ToString() + " Actual: " + SoftBasic.ByteToHexString(ack, ' '));
      return !MelsecHelper.CheckCRC(ack) ? new OperateResult(StringResources.Language.MelsecFxCrcCheckFailed) : OperateResult.CreateSuccessResult();
    }

    /// <summary>检查PLC返回的写入的数据是否是正常的</summary>
    /// <param name="ack">Plc反馈的数据信息</param>
    /// <returns>检查结果</returns>
    public static OperateResult CheckPlcWriteResponse(byte[] ack)
    {
      if (ack.Length == 0)
        return new OperateResult(StringResources.Language.MelsecFxReceiveZero);
      if (ack[0] == (byte) 21)
        return new OperateResult(StringResources.Language.MelsecFxAckNagative + " Actual: " + SoftBasic.ByteToHexString(ack, ' '));
      return ack[0] != (byte) 6 ? new OperateResult(StringResources.Language.MelsecFxAckWrong + ack[0].ToString() + " Actual: " + SoftBasic.ByteToHexString(ack, ' ')) : OperateResult.CreateSuccessResult();
    }

    /// <summary>生成位写入的数据报文信息，该报文可直接用于发送串口给PLC</summary>
    /// <param name="address">地址信息，每个地址存在一定的范围，需要谨慎传入数据。举例：M10,S10,X5,Y10,C10,T10</param>
    /// <param name="value"><c>True</c>或是<c>False</c></param>
    /// <returns>带报文信息的结果对象</returns>
    public static OperateResult<byte[]> BuildWriteBoolPacket(
      string address,
      bool value)
    {
      OperateResult<MelsecMcDataType, ushort> operateResult = MelsecFxSerialOverTcp.FxAnalysisAddress(address);
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult);
      ushort content2 = operateResult.Content2;
      ushort num;
      if (operateResult.Content1 == MelsecMcDataType.M)
        num = content2 < (ushort) 8000 ? (ushort) ((uint) content2 + 2048U) : (ushort) ((int) content2 - 8000 + 3840);
      else if (operateResult.Content1 == MelsecMcDataType.S)
        num = content2;
      else if (operateResult.Content1 == MelsecMcDataType.X)
        num = (ushort) ((uint) content2 + 1024U);
      else if (operateResult.Content1 == MelsecMcDataType.Y)
        num = (ushort) ((uint) content2 + 1280U);
      else if (operateResult.Content1 == MelsecMcDataType.CS)
        num = (ushort) ((uint) content2 + 448U);
      else if (operateResult.Content1 == MelsecMcDataType.CC)
        num = (ushort) ((uint) content2 + 960U);
      else if (operateResult.Content1 == MelsecMcDataType.CN)
        num = (ushort) ((uint) content2 + 3584U);
      else if (operateResult.Content1 == MelsecMcDataType.TS)
        num = (ushort) ((uint) content2 + 192U);
      else if (operateResult.Content1 == MelsecMcDataType.TC)
      {
        num = (ushort) ((uint) content2 + 704U);
      }
      else
      {
        if (operateResult.Content1 != MelsecMcDataType.TN)
          return new OperateResult<byte[]>(StringResources.Language.MelsecCurrentTypeNotSupportedBitOperate);
        num = (ushort) ((uint) content2 + 1536U);
      }
      byte[] data = new byte[9]
      {
        (byte) 2,
        value ? (byte) 55 : (byte) 56,
        SoftBasic.BuildAsciiBytesFrom(num)[2],
        SoftBasic.BuildAsciiBytesFrom(num)[3],
        SoftBasic.BuildAsciiBytesFrom(num)[0],
        SoftBasic.BuildAsciiBytesFrom(num)[1],
        (byte) 3,
        (byte) 0,
        (byte) 0
      };
      MelsecHelper.FxCalculateCRC(data).CopyTo((Array) data, 7);
      return OperateResult.CreateSuccessResult<byte[]>(data);
    }

    /// <summary>根据类型地址长度确认需要读取的指令头</summary>
    /// <param name="address">起始地址</param>
    /// <param name="length">长度</param>
    /// <param name="isNewVersion">是否是新版的串口访问类</param>
    /// <returns>带有成功标志的指令数据</returns>
    public static OperateResult<byte[]> BuildReadWordCommand(
      string address,
      ushort length,
      bool isNewVersion)
    {
      OperateResult<ushort> wordStartAddress = MelsecFxSerialOverTcp.FxCalculateWordStartAddress(address, isNewVersion);
      if (!wordStartAddress.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) wordStartAddress);
      length *= (ushort) 2;
      ushort content = wordStartAddress.Content;
      if (isNewVersion)
      {
        byte[] data = new byte[13]
        {
          (byte) 2,
          (byte) 69,
          (byte) 48,
          (byte) 48,
          SoftBasic.BuildAsciiBytesFrom(content)[0],
          SoftBasic.BuildAsciiBytesFrom(content)[1],
          SoftBasic.BuildAsciiBytesFrom(content)[2],
          SoftBasic.BuildAsciiBytesFrom(content)[3],
          SoftBasic.BuildAsciiBytesFrom((byte) length)[0],
          SoftBasic.BuildAsciiBytesFrom((byte) length)[1],
          (byte) 3,
          (byte) 0,
          (byte) 0
        };
        MelsecHelper.FxCalculateCRC(data).CopyTo((Array) data, 11);
        return OperateResult.CreateSuccessResult<byte[]>(data);
      }
      byte[] data1 = new byte[11]
      {
        (byte) 2,
        (byte) 48,
        SoftBasic.BuildAsciiBytesFrom(content)[0],
        SoftBasic.BuildAsciiBytesFrom(content)[1],
        SoftBasic.BuildAsciiBytesFrom(content)[2],
        SoftBasic.BuildAsciiBytesFrom(content)[3],
        SoftBasic.BuildAsciiBytesFrom((byte) length)[0],
        SoftBasic.BuildAsciiBytesFrom((byte) length)[1],
        (byte) 3,
        (byte) 0,
        (byte) 0
      };
      MelsecHelper.FxCalculateCRC(data1).CopyTo((Array) data1, 9);
      return OperateResult.CreateSuccessResult<byte[]>(data1);
    }

    /// <summary>根据类型地址长度确认需要读取的指令头</summary>
    /// <param name="address">起始地址</param>
    /// <param name="length">bool数组长度</param>
    /// <returns>带有成功标志的指令数据</returns>
    public static OperateResult<byte[], int> BuildReadBoolCommand(
      string address,
      ushort length)
    {
      OperateResult<ushort, ushort, ushort> boolStartAddress = MelsecFxSerialOverTcp.FxCalculateBoolStartAddress(address);
      if (!boolStartAddress.IsSuccess)
        return OperateResult.CreateFailedResult<byte[], int>((OperateResult) boolStartAddress);
      ushort num = (ushort) (((int) boolStartAddress.Content2 + (int) length - 1) / 8 - (int) boolStartAddress.Content2 / 8 + 1);
      ushort content1 = boolStartAddress.Content1;
      byte[] data = new byte[11]
      {
        (byte) 2,
        (byte) 48,
        SoftBasic.BuildAsciiBytesFrom(content1)[0],
        SoftBasic.BuildAsciiBytesFrom(content1)[1],
        SoftBasic.BuildAsciiBytesFrom(content1)[2],
        SoftBasic.BuildAsciiBytesFrom(content1)[3],
        SoftBasic.BuildAsciiBytesFrom((byte) num)[0],
        SoftBasic.BuildAsciiBytesFrom((byte) num)[1],
        (byte) 3,
        (byte) 0,
        (byte) 0
      };
      MelsecHelper.FxCalculateCRC(data).CopyTo((Array) data, 9);
      return OperateResult.CreateSuccessResult<byte[], int>(data, (int) boolStartAddress.Content3);
    }

    /// <summary>根据类型地址以及需要写入的数据来生成指令头</summary>
    /// <param name="address">起始地址</param>
    /// <param name="value">实际的数据信息</param>
    /// <param name="isNewVersion">是否是新版的串口访问类</param>
    /// <returns>带有成功标志的指令数据</returns>
    public static OperateResult<byte[]> BuildWriteWordCommand(
      string address,
      byte[] value,
      bool isNewVersion)
    {
      OperateResult<ushort> wordStartAddress = MelsecFxSerialOverTcp.FxCalculateWordStartAddress(address, isNewVersion);
      if (!wordStartAddress.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) wordStartAddress);
      if (value != null)
        value = SoftBasic.BuildAsciiBytesFrom(value);
      ushort content = wordStartAddress.Content;
      if (isNewVersion)
      {
        byte[] data = new byte[13 + value.Length];
        data[0] = (byte) 2;
        data[1] = (byte) 69;
        data[2] = (byte) 49;
        data[3] = (byte) 48;
        data[4] = SoftBasic.BuildAsciiBytesFrom(content)[0];
        data[5] = SoftBasic.BuildAsciiBytesFrom(content)[1];
        data[6] = SoftBasic.BuildAsciiBytesFrom(content)[2];
        data[7] = SoftBasic.BuildAsciiBytesFrom(content)[3];
        data[8] = SoftBasic.BuildAsciiBytesFrom((byte) (value.Length / 2))[0];
        data[9] = SoftBasic.BuildAsciiBytesFrom((byte) (value.Length / 2))[1];
        Array.Copy((Array) value, 0, (Array) data, 10, value.Length);
        data[data.Length - 3] = (byte) 3;
        MelsecHelper.FxCalculateCRC(data).CopyTo((Array) data, data.Length - 2);
        return OperateResult.CreateSuccessResult<byte[]>(data);
      }
      byte[] data1 = new byte[11 + value.Length];
      data1[0] = (byte) 2;
      data1[1] = (byte) 49;
      data1[2] = SoftBasic.BuildAsciiBytesFrom(content)[0];
      data1[3] = SoftBasic.BuildAsciiBytesFrom(content)[1];
      data1[4] = SoftBasic.BuildAsciiBytesFrom(content)[2];
      data1[5] = SoftBasic.BuildAsciiBytesFrom(content)[3];
      data1[6] = SoftBasic.BuildAsciiBytesFrom((byte) (value.Length / 2))[0];
      data1[7] = SoftBasic.BuildAsciiBytesFrom((byte) (value.Length / 2))[1];
      Array.Copy((Array) value, 0, (Array) data1, 8, value.Length);
      data1[data1.Length - 3] = (byte) 3;
      MelsecHelper.FxCalculateCRC(data1).CopyTo((Array) data1, data1.Length - 2);
      return OperateResult.CreateSuccessResult<byte[]>(data1);
    }

    /// <summary>从PLC反馈的数据进行提炼操作</summary>
    /// <param name="response">PLC反馈的真实数据</param>
    /// <returns>数据提炼后的真实数据</returns>
    public static OperateResult<byte[]> ExtractActualData(byte[] response)
    {
      try
      {
        byte[] numArray = new byte[(response.Length - 4) / 2];
        for (int index = 0; index < numArray.Length; ++index)
        {
          byte[] bytes = new byte[2]
          {
            response[index * 2 + 1],
            response[index * 2 + 2]
          };
          numArray[index] = Convert.ToByte(Encoding.ASCII.GetString(bytes), 16);
        }
        return OperateResult.CreateSuccessResult<byte[]>(numArray);
      }
      catch (Exception ex)
      {
        OperateResult<byte[]> operateResult = new OperateResult<byte[]>();
        operateResult.Message = "Extract Msg：" + ex.Message + Environment.NewLine + "Data: " + SoftBasic.ByteToHexString(response);
        return operateResult;
      }
    }

    /// <summary>从PLC反馈的数据进行提炼bool数组操作</summary>
    /// <param name="response">PLC反馈的真实数据</param>
    /// <param name="start">起始提取的点信息</param>
    /// <param name="length">bool数组的长度</param>
    /// <returns>数据提炼后的真实数据</returns>
    public static OperateResult<bool[]> ExtractActualBoolData(
      byte[] response,
      int start,
      int length)
    {
      OperateResult<byte[]> actualData = MelsecFxSerialOverTcp.ExtractActualData(response);
      if (!actualData.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) actualData);
      try
      {
        bool[] flagArray = new bool[length];
        bool[] boolArray = SoftBasic.ByteToBoolArray(actualData.Content, actualData.Content.Length * 8);
        for (int index = 0; index < length; ++index)
          flagArray[index] = boolArray[index + start];
        return OperateResult.CreateSuccessResult<bool[]>(flagArray);
      }
      catch (Exception ex)
      {
        OperateResult<bool[]> operateResult = new OperateResult<bool[]>();
        operateResult.Message = "Extract Msg：" + ex.Message + Environment.NewLine + "Data: " + SoftBasic.ByteToHexString(response);
        return operateResult;
      }
    }

    /// <summary>解析数据地址成不同的三菱地址类型</summary>
    /// <param name="address">数据地址</param>
    /// <returns>地址结果对象</returns>
    private static OperateResult<MelsecMcDataType, ushort> FxAnalysisAddress(
      string address)
    {
      OperateResult<MelsecMcDataType, ushort> operateResult = new OperateResult<MelsecMcDataType, ushort>();
      try
      {
        switch (address[0])
        {
          case 'C':
          case 'c':
            if (address[1] == 'N' || address[1] == 'n')
            {
              operateResult.Content1 = MelsecMcDataType.CN;
              operateResult.Content2 = Convert.ToUInt16(address.Substring(2), MelsecMcDataType.CN.FromBase);
              break;
            }
            if (address[1] == 'S' || address[1] == 's')
            {
              operateResult.Content1 = MelsecMcDataType.CS;
              operateResult.Content2 = Convert.ToUInt16(address.Substring(2), MelsecMcDataType.CS.FromBase);
              break;
            }
            if (address[1] != 'C' && address[1] != 'c')
              throw new Exception(StringResources.Language.NotSupportedDataType);
            operateResult.Content1 = MelsecMcDataType.CC;
            operateResult.Content2 = Convert.ToUInt16(address.Substring(2), MelsecMcDataType.CC.FromBase);
            break;
          case 'D':
          case 'd':
            operateResult.Content1 = MelsecMcDataType.D;
            operateResult.Content2 = Convert.ToUInt16(address.Substring(1), MelsecMcDataType.D.FromBase);
            break;
          case 'M':
          case 'm':
            operateResult.Content1 = MelsecMcDataType.M;
            operateResult.Content2 = Convert.ToUInt16(address.Substring(1), MelsecMcDataType.M.FromBase);
            break;
          case 'S':
          case 's':
            operateResult.Content1 = MelsecMcDataType.S;
            operateResult.Content2 = Convert.ToUInt16(address.Substring(1), MelsecMcDataType.S.FromBase);
            break;
          case 'T':
          case 't':
            if (address[1] == 'N' || address[1] == 'n')
            {
              operateResult.Content1 = MelsecMcDataType.TN;
              operateResult.Content2 = Convert.ToUInt16(address.Substring(2), MelsecMcDataType.TN.FromBase);
              break;
            }
            if (address[1] == 'S' || address[1] == 's')
            {
              operateResult.Content1 = MelsecMcDataType.TS;
              operateResult.Content2 = Convert.ToUInt16(address.Substring(2), MelsecMcDataType.TS.FromBase);
              break;
            }
            if (address[1] != 'C' && address[1] != 'c')
              throw new Exception(StringResources.Language.NotSupportedDataType);
            operateResult.Content1 = MelsecMcDataType.TC;
            operateResult.Content2 = Convert.ToUInt16(address.Substring(2), MelsecMcDataType.TC.FromBase);
            break;
          case 'X':
          case 'x':
            operateResult.Content1 = MelsecMcDataType.X;
            operateResult.Content2 = Convert.ToUInt16(address.Substring(1), 8);
            break;
          case 'Y':
          case 'y':
            operateResult.Content1 = MelsecMcDataType.Y;
            operateResult.Content2 = Convert.ToUInt16(address.Substring(1), 8);
            break;
          default:
            throw new Exception(StringResources.Language.NotSupportedDataType);
        }
      }
      catch (Exception ex)
      {
        operateResult.Message = ex.Message;
        return operateResult;
      }
      operateResult.IsSuccess = true;
      return operateResult;
    }

    /// <summary>返回读取的地址及长度信息</summary>
    /// <param name="address">读取的地址信息</param>
    /// <param name="isNewVersion">是否是新版的串口访问类</param>
    /// <returns>带起始地址的结果对象</returns>
    private static OperateResult<ushort> FxCalculateWordStartAddress(
      string address,
      bool isNewVersion)
    {
      OperateResult<MelsecMcDataType, ushort> operateResult = MelsecFxSerialOverTcp.FxAnalysisAddress(address);
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<ushort>((OperateResult) operateResult);
      ushort content2 = operateResult.Content2;
      ushort num;
      if (operateResult.Content1 == MelsecMcDataType.D)
        num = content2 < (ushort) 8000 ? (isNewVersion ? (ushort) ((int) content2 * 2 + 16384) : (ushort) ((int) content2 * 2 + 4096)) : (ushort) (((int) content2 - 8000) * 2 + 3584);
      else if (operateResult.Content1 == MelsecMcDataType.CN)
      {
        num = content2 < (ushort) 200 ? (ushort) ((int) content2 * 2 + 2560) : (ushort) (((int) content2 - 200) * 4 + 3072);
      }
      else
      {
        if (operateResult.Content1 != MelsecMcDataType.TN)
          return new OperateResult<ushort>(StringResources.Language.MelsecCurrentTypeNotSupportedWordOperate);
        num = (ushort) ((int) content2 * 2 + 2048);
      }
      return OperateResult.CreateSuccessResult<ushort>(num);
    }

    /// <summary>返回读取的地址及长度信息，以及当前的偏置信息</summary>
    /// <param name="address">读取的地址信息</param>
    /// <returns>带起始地址的结果对象</returns>
    private static OperateResult<ushort, ushort, ushort> FxCalculateBoolStartAddress(
      string address)
    {
      OperateResult<MelsecMcDataType, ushort> operateResult = MelsecFxSerialOverTcp.FxAnalysisAddress(address);
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<ushort, ushort, ushort>((OperateResult) operateResult);
      ushort content2 = operateResult.Content2;
      ushort num;
      if (operateResult.Content1 == MelsecMcDataType.M)
        num = content2 < (ushort) 8000 ? (ushort) ((int) content2 / 8 + 256) : (ushort) (((int) content2 - 8000) / 8 + 480);
      else if (operateResult.Content1 == MelsecMcDataType.X)
        num = (ushort) ((int) content2 / 8 + 128);
      else if (operateResult.Content1 == MelsecMcDataType.Y)
        num = (ushort) ((int) content2 / 8 + 160);
      else if (operateResult.Content1 == MelsecMcDataType.S)
        num = (ushort) ((uint) content2 / 8U);
      else if (operateResult.Content1 == MelsecMcDataType.CS)
        num = (ushort) ((int) content2 / 8 + 448);
      else if (operateResult.Content1 == MelsecMcDataType.CC)
        num = (ushort) ((int) content2 / 8 + 960);
      else if (operateResult.Content1 == MelsecMcDataType.TS)
      {
        num = (ushort) ((int) content2 / 8 + 192);
      }
      else
      {
        if (operateResult.Content1 != MelsecMcDataType.TC)
          return new OperateResult<ushort, ushort, ushort>(StringResources.Language.MelsecCurrentTypeNotSupportedBitOperate);
        num = (ushort) ((int) content2 / 8 + 704);
      }
      return OperateResult.CreateSuccessResult<ushort, ushort, ushort>(num, operateResult.Content2, (ushort) ((uint) operateResult.Content2 % 8U));
    }
  }
}
