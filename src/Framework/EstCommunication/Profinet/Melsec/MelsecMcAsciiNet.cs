// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.Melsec.MelsecMcAsciiNet
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using EstCommunication.Core;
using EstCommunication.Core.Address;
using EstCommunication.Core.IMessage;
using EstCommunication.Core.Net;
using EstCommunication.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstCommunication.Profinet.Melsec
{
  /// <summary>
  /// 三菱PLC通讯类，采用Qna兼容3E帧协议实现，需要在PLC侧先的以太网模块先进行配置，必须为ASCII通讯格式<br />
  /// Mitsubishi PLC communication class is implemented using Qna compatible 3E frame protocol.
  /// The Ethernet module on the PLC side needs to be configured first. It must be ascii communication.
  /// </summary>
  /// <remarks>
  /// <inheritdoc cref="T:EstCommunication.Profinet.Melsec.MelsecMcNet" path="remarks" />
  /// </remarks>
  /// <example>
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecAscii.cs" region="Usage" title="简单的短连接使用" />
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\MelsecAscii.cs" region="Usage2" title="简单的长连接使用" />
  /// </example>
  public class MelsecMcAsciiNet : NetworkDeviceBase
  {
    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecMcNet.#ctor" />
    public MelsecMcAsciiNet()
    {
      this.WordLength = (ushort) 1;
      this.LogMsgFormatBinary = false;
      this.ByteTransform = (IByteTransform) new RegularByteTransform();
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecMcNet.#ctor(System.String,System.Int32)" />
    public MelsecMcAsciiNet(string ipAddress, int port)
    {
      this.WordLength = (ushort) 1;
      this.IpAddress = ipAddress;
      this.Port = port;
      this.LogMsgFormatBinary = false;
      this.ByteTransform = (IByteTransform) new RegularByteTransform();
    }

    /// <inheritdoc />
    protected override INetMessage GetNewNetMessage() => (INetMessage) new MelsecQnA3EAsciiMessage();

    /// <inheritdoc cref="P:EstCommunication.Profinet.Melsec.MelsecMcNet.NetworkNumber" />
    public byte NetworkNumber { get; set; } = 0;

    /// <inheritdoc cref="P:EstCommunication.Profinet.Melsec.MelsecMcNet.NetworkStationNumber" />
    public byte NetworkStationNumber { get; set; } = 0;

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecMcNet.McAnalysisAddress(System.String,System.UInt16)" />
    protected virtual OperateResult<McAddressData> McAnalysisAddress(
      string address,
      ushort length)
    {
      return McAddressData.ParseMelsecFrom(address, length);
    }

    /// <inheritdoc />
    [EstMqttApi("ReadByteArray", "")]
    public override OperateResult<byte[]> Read(string address, ushort length)
    {
      OperateResult<McAddressData> operateResult1 = this.McAnalysisAddress(address, length);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult1);
      List<byte> byteList = new List<byte>();
      ushort num1 = 0;
      while ((int) num1 < (int) length)
      {
        ushort num2 = (ushort) Math.Min((int) length - (int) num1, 450);
        operateResult1.Content.Length = num2;
        OperateResult<byte[]> operateResult2 = this.ReadAddressData(operateResult1.Content);
        if (!operateResult2.IsSuccess)
          return operateResult2;
        byteList.AddRange((IEnumerable<byte>) operateResult2.Content);
        num1 += num2;
        if (operateResult1.Content.McDataType.DataType == (byte) 0)
          operateResult1.Content.AddressStart += (int) num2;
        else
          operateResult1.Content.AddressStart += (int) num2 * 16;
      }
      return OperateResult.CreateSuccessResult<byte[]>(byteList.ToArray());
    }

    private OperateResult<byte[]> ReadAddressData(McAddressData addressData)
    {
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(MelsecMcAsciiNet.PackMcCommand(MelsecHelper.BuildAsciiReadMcCoreCommand(addressData, false), this.NetworkNumber, this.NetworkStationNumber));
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult);
      OperateResult result = MelsecMcAsciiNet.CheckResponseContent(operateResult.Content);
      return !result.IsSuccess ? OperateResult.CreateFailedResult<byte[]>(result) : MelsecMcAsciiNet.ExtractActualData(operateResult.Content, false);
    }

    /// <inheritdoc />
    [EstMqttApi("WriteByteArray", "")]
    public override OperateResult Write(string address, byte[] value)
    {
      OperateResult<McAddressData> operateResult1 = this.McAnalysisAddress(address, (ushort) 0);
      if (!operateResult1.IsSuccess)
        return (OperateResult) OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult1);
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(MelsecMcAsciiNet.PackMcCommand(MelsecHelper.BuildAsciiWriteWordCoreCommand(operateResult1.Content, value), this.NetworkNumber, this.NetworkStationNumber));
      if (!operateResult2.IsSuccess)
        return (OperateResult) operateResult2;
      OperateResult operateResult3 = MelsecMcAsciiNet.CheckResponseContent(operateResult2.Content);
      return !operateResult3.IsSuccess ? operateResult3 : OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc />
    public override async Task<OperateResult<byte[]>> ReadAsync(
      string address,
      ushort length)
    {
      OperateResult<McAddressData> addressResult = this.McAnalysisAddress(address, length);
      if (!addressResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) addressResult);
      List<byte> bytesContent = new List<byte>();
      ushort alreadyFinished = 0;
      while ((int) alreadyFinished < (int) length)
      {
        ushort readLength = (ushort) Math.Min((int) length - (int) alreadyFinished, 450);
        addressResult.Content.Length = readLength;
        OperateResult<byte[]> read = await this.ReadAddressDataAsync(addressResult.Content);
        if (!read.IsSuccess)
          return read;
        bytesContent.AddRange((IEnumerable<byte>) read.Content);
        alreadyFinished += readLength;
        if (addressResult.Content.McDataType.DataType == (byte) 0)
          addressResult.Content.AddressStart += (int) readLength;
        else
          addressResult.Content.AddressStart += (int) readLength * 16;
        read = (OperateResult<byte[]>) null;
      }
      return OperateResult.CreateSuccessResult<byte[]>(bytesContent.ToArray());
    }

    private async Task<OperateResult<byte[]>> ReadAddressDataAsync(
      McAddressData addressData)
    {
      byte[] coreResult = MelsecHelper.BuildAsciiReadMcCoreCommand(addressData, false);
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(MelsecMcAsciiNet.PackMcCommand(coreResult, this.NetworkNumber, this.NetworkStationNumber));
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) read);
      OperateResult check = MelsecMcAsciiNet.CheckResponseContent(read.Content);
      return check.IsSuccess ? MelsecMcAsciiNet.ExtractActualData(read.Content, false) : OperateResult.CreateFailedResult<byte[]>(check);
    }

    /// <inheritdoc />
    public override async Task<OperateResult> WriteAsync(
      string address,
      byte[] value)
    {
      OperateResult<McAddressData> addressResult = this.McAnalysisAddress(address, (ushort) 0);
      if (!addressResult.IsSuccess)
        return (OperateResult) OperateResult.CreateFailedResult<byte[]>((OperateResult) addressResult);
      byte[] coreResult = MelsecHelper.BuildAsciiWriteWordCoreCommand(addressResult.Content, value);
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(MelsecMcAsciiNet.PackMcCommand(coreResult, this.NetworkNumber, this.NetworkStationNumber));
      if (!read.IsSuccess)
        return (OperateResult) read;
      OperateResult check = MelsecMcAsciiNet.CheckResponseContent(read.Content);
      return check.IsSuccess ? OperateResult.CreateSuccessResult() : check;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecMcNet.ReadRandom(System.String[])" />
    [EstMqttApi]
    public OperateResult<byte[]> ReadRandom(string[] address)
    {
      McAddressData[] address1 = new McAddressData[address.Length];
      for (int index = 0; index < address.Length; ++index)
      {
        OperateResult<McAddressData> melsecFrom = McAddressData.ParseMelsecFrom(address[index], (ushort) 1);
        if (!melsecFrom.IsSuccess)
          return OperateResult.CreateFailedResult<byte[]>((OperateResult) melsecFrom);
        address1[index] = melsecFrom.Content;
      }
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(MelsecMcAsciiNet.PackMcCommand(MelsecHelper.BuildAsciiReadRandomWordCommand(address1), this.NetworkNumber, this.NetworkStationNumber));
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult);
      OperateResult result = MelsecMcAsciiNet.CheckResponseContent(operateResult.Content);
      return !result.IsSuccess ? OperateResult.CreateFailedResult<byte[]>(result) : MelsecMcAsciiNet.ExtractActualData(operateResult.Content, false);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecMcNet.ReadRandom(System.String[],System.UInt16[])" />
    public OperateResult<byte[]> ReadRandom(string[] address, ushort[] length)
    {
      if (length.Length != address.Length)
        return new OperateResult<byte[]>(StringResources.Language.TwoParametersLengthIsNotSame);
      McAddressData[] address1 = new McAddressData[address.Length];
      for (int index = 0; index < address.Length; ++index)
      {
        OperateResult<McAddressData> melsecFrom = McAddressData.ParseMelsecFrom(address[index], length[index]);
        if (!melsecFrom.IsSuccess)
          return OperateResult.CreateFailedResult<byte[]>((OperateResult) melsecFrom);
        address1[index] = melsecFrom.Content;
      }
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(MelsecMcAsciiNet.PackMcCommand(MelsecHelper.BuildAsciiReadRandomCommand(address1), this.NetworkNumber, this.NetworkStationNumber));
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult);
      OperateResult result = MelsecMcAsciiNet.CheckResponseContent(operateResult.Content);
      return !result.IsSuccess ? OperateResult.CreateFailedResult<byte[]>(result) : MelsecMcAsciiNet.ExtractActualData(operateResult.Content, false);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecMcNet.ReadRandomInt16(System.String[])" />
    public OperateResult<short[]> ReadRandomInt16(string[] address)
    {
      OperateResult<byte[]> operateResult = this.ReadRandom(address);
      return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<short[]>((OperateResult) operateResult) : OperateResult.CreateSuccessResult<short[]>(this.ByteTransform.TransInt16(operateResult.Content, 0, address.Length));
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecMcAsciiNet.ReadRandom(System.String[])" />
    public async Task<OperateResult<byte[]>> ReadRandomAsync(string[] address)
    {
      McAddressData[] mcAddressDatas = new McAddressData[address.Length];
      for (int i = 0; i < address.Length; ++i)
      {
        OperateResult<McAddressData> addressResult = McAddressData.ParseMelsecFrom(address[i], (ushort) 1);
        if (!addressResult.IsSuccess)
          return OperateResult.CreateFailedResult<byte[]>((OperateResult) addressResult);
        mcAddressDatas[i] = addressResult.Content;
        addressResult = (OperateResult<McAddressData>) null;
      }
      byte[] coreResult = MelsecHelper.BuildAsciiReadRandomWordCommand(mcAddressDatas);
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(MelsecMcAsciiNet.PackMcCommand(coreResult, this.NetworkNumber, this.NetworkStationNumber));
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) read);
      OperateResult check = MelsecMcAsciiNet.CheckResponseContent(read.Content);
      return check.IsSuccess ? MelsecMcAsciiNet.ExtractActualData(read.Content, false) : OperateResult.CreateFailedResult<byte[]>(check);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecMcAsciiNet.ReadRandom(System.String[],System.UInt16[])" />
    public async Task<OperateResult<byte[]>> ReadRandomAsync(
      string[] address,
      ushort[] length)
    {
      if (length.Length != address.Length)
        return new OperateResult<byte[]>(StringResources.Language.TwoParametersLengthIsNotSame);
      McAddressData[] mcAddressDatas = new McAddressData[address.Length];
      for (int i = 0; i < address.Length; ++i)
      {
        OperateResult<McAddressData> addressResult = McAddressData.ParseMelsecFrom(address[i], length[i]);
        if (!addressResult.IsSuccess)
          return OperateResult.CreateFailedResult<byte[]>((OperateResult) addressResult);
        mcAddressDatas[i] = addressResult.Content;
        addressResult = (OperateResult<McAddressData>) null;
      }
      byte[] coreResult = MelsecHelper.BuildAsciiReadRandomCommand(mcAddressDatas);
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(MelsecMcAsciiNet.PackMcCommand(coreResult, this.NetworkNumber, this.NetworkStationNumber));
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) read);
      OperateResult check = MelsecMcAsciiNet.CheckResponseContent(read.Content);
      return check.IsSuccess ? MelsecMcAsciiNet.ExtractActualData(read.Content, false) : OperateResult.CreateFailedResult<byte[]>(check);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecMcAsciiNet.ReadRandomInt16(System.String[])" />
    public async Task<OperateResult<short[]>> ReadRandomInt16Async(string[] address)
    {
      OperateResult<byte[]> read = await this.ReadRandomAsync(address);
      OperateResult<short[]> operateResult = read.IsSuccess ? OperateResult.CreateSuccessResult<short[]>(this.ByteTransform.TransInt16(read.Content, 0, address.Length)) : OperateResult.CreateFailedResult<short[]>((OperateResult) read);
      read = (OperateResult<byte[]>) null;
      return operateResult;
    }

    /// <inheritdoc />
    [EstMqttApi("ReadBoolArray", "")]
    public override OperateResult<bool[]> ReadBool(string address, ushort length)
    {
      OperateResult<McAddressData> operateResult1 = this.McAnalysisAddress(address, length);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult1);
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(MelsecMcAsciiNet.PackMcCommand(MelsecHelper.BuildAsciiReadMcCoreCommand(operateResult1.Content, true), this.NetworkNumber, this.NetworkStationNumber));
      if (!operateResult2.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult2);
      OperateResult result = MelsecMcAsciiNet.CheckResponseContent(operateResult2.Content);
      if (!result.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>(result);
      OperateResult<byte[]> actualData = MelsecMcAsciiNet.ExtractActualData(operateResult2.Content, true);
      return !actualData.IsSuccess ? OperateResult.CreateFailedResult<bool[]>((OperateResult) actualData) : OperateResult.CreateSuccessResult<bool[]>(((IEnumerable<byte>) actualData.Content).Select<byte, bool>((Func<byte, bool>) (m => m == (byte) 1)).Take<bool>((int) length).ToArray<bool>());
    }

    /// <inheritdoc />
    [EstMqttApi("WriteBoolArray", "")]
    public override OperateResult Write(string address, bool[] values)
    {
      OperateResult<McAddressData> operateResult1 = this.McAnalysisAddress(address, (ushort) 0);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(MelsecMcAsciiNet.PackMcCommand(MelsecHelper.BuildAsciiWriteBitCoreCommand(operateResult1.Content, values), this.NetworkNumber, this.NetworkStationNumber));
      if (!operateResult2.IsSuccess)
        return (OperateResult) operateResult2;
      OperateResult operateResult3 = MelsecMcAsciiNet.CheckResponseContent(operateResult2.Content);
      return !operateResult3.IsSuccess ? operateResult3 : OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc />
    public override async Task<OperateResult<bool[]>> ReadBoolAsync(
      string address,
      ushort length)
    {
      OperateResult<McAddressData> addressResult = this.McAnalysisAddress(address, length);
      if (!addressResult.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) addressResult);
      byte[] coreResult = MelsecHelper.BuildAsciiReadMcCoreCommand(addressResult.Content, true);
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(MelsecMcAsciiNet.PackMcCommand(coreResult, this.NetworkNumber, this.NetworkStationNumber));
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) read);
      OperateResult check = MelsecMcAsciiNet.CheckResponseContent(read.Content);
      if (!check.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>(check);
      OperateResult<byte[]> extract = MelsecMcAsciiNet.ExtractActualData(read.Content, true);
      return extract.IsSuccess ? OperateResult.CreateSuccessResult<bool[]>(((IEnumerable<byte>) extract.Content).Select<byte, bool>((Func<byte, bool>) (m => m == (byte) 1)).Take<bool>((int) length).ToArray<bool>()) : OperateResult.CreateFailedResult<bool[]>((OperateResult) extract);
    }

    /// <inheritdoc />
    public override async Task<OperateResult> WriteAsync(
      string address,
      bool[] values)
    {
      OperateResult<McAddressData> addressResult = this.McAnalysisAddress(address, (ushort) 0);
      if (!addressResult.IsSuccess)
        return (OperateResult) addressResult;
      byte[] coreResult = MelsecHelper.BuildAsciiWriteBitCoreCommand(addressResult.Content, values);
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(MelsecMcAsciiNet.PackMcCommand(coreResult, this.NetworkNumber, this.NetworkStationNumber));
      if (!read.IsSuccess)
        return (OperateResult) read;
      OperateResult check = MelsecMcAsciiNet.CheckResponseContent(read.Content);
      return check.IsSuccess ? OperateResult.CreateSuccessResult() : check;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecMcNet.ReadMemory(System.String,System.UInt16)" />
    [EstMqttApi(ApiTopic = "ReadMemory", Description = "读取缓冲寄存器的数据信息，地址直接为偏移地址")]
    public OperateResult<byte[]> ReadMemory(string address, ushort length)
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
      OperateResult<byte[]> operateResult1 = MelsecHelper.BuildAsciiReadMemoryCommand(address, length);
      if (!operateResult1.IsSuccess)
        return operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(MelsecMcAsciiNet.PackMcCommand(operateResult1.Content, this.NetworkNumber, this.NetworkStationNumber));
      if (!operateResult2.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult2);
      OperateResult result = MelsecMcAsciiNet.CheckResponseContent(operateResult2.Content);
      return !result.IsSuccess ? OperateResult.CreateFailedResult<byte[]>(result) : MelsecMcAsciiNet.ExtractActualData(operateResult2.Content, false);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecMcAsciiNet.ReadMemory(System.String,System.UInt16)" />
    public async Task<OperateResult<byte[]>> ReadMemoryAsync(
      string address,
      ushort length)
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
      OperateResult<byte[]> coreResult = MelsecHelper.BuildAsciiReadMemoryCommand(address, length);
      if (!coreResult.IsSuccess)
        return coreResult;
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(MelsecMcAsciiNet.PackMcCommand(coreResult.Content, this.NetworkNumber, this.NetworkStationNumber));
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) read);
      OperateResult check = MelsecMcAsciiNet.CheckResponseContent(read.Content);
      return check.IsSuccess ? MelsecMcAsciiNet.ExtractActualData(read.Content, false) : OperateResult.CreateFailedResult<byte[]>(check);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecMcNet.ReadSmartModule(System.UInt16,System.String,System.UInt16)" />
    [EstMqttApi(ApiTopic = "ReadSmartModule", Description = "读取智能模块的数据信息，需要指定模块地址，偏移地址，读取的字节长度")]
    public OperateResult<byte[]> ReadSmartModule(
      ushort module,
      string address,
      ushort length)
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
      OperateResult<byte[]> operateResult1 = MelsecHelper.BuildAsciiReadSmartModule(module, address, length);
      if (!operateResult1.IsSuccess)
        return operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(MelsecMcAsciiNet.PackMcCommand(operateResult1.Content, this.NetworkNumber, this.NetworkStationNumber));
      if (!operateResult2.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult2);
      OperateResult result = MelsecMcAsciiNet.CheckResponseContent(operateResult2.Content);
      return !result.IsSuccess ? OperateResult.CreateFailedResult<byte[]>(result) : MelsecMcAsciiNet.ExtractActualData(operateResult2.Content, false);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecMcAsciiNet.ReadSmartModule(System.UInt16,System.String,System.UInt16)" />
    public async Task<OperateResult<byte[]>> ReadSmartModuleAsync(
      ushort module,
      string address,
      ushort length)
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
      OperateResult<byte[]> coreResult = MelsecHelper.BuildAsciiReadSmartModule(module, address, length);
      if (!coreResult.IsSuccess)
        return coreResult;
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(MelsecMcAsciiNet.PackMcCommand(coreResult.Content, this.NetworkNumber, this.NetworkStationNumber));
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) read);
      OperateResult check = MelsecMcAsciiNet.CheckResponseContent(read.Content);
      return check.IsSuccess ? MelsecMcAsciiNet.ExtractActualData(read.Content, false) : OperateResult.CreateFailedResult<byte[]>(check);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecMcNet.RemoteRun(System.Boolean)" />
    [EstMqttApi]
    public OperateResult RemoteRun()
    {
      OperateResult<byte[]> operateResult1 = this.ReadFromCoreServer(MelsecMcAsciiNet.PackMcCommand(Encoding.ASCII.GetBytes("1001000000010000"), this.NetworkNumber, this.NetworkStationNumber));
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult operateResult2 = MelsecMcAsciiNet.CheckResponseContent(operateResult1.Content);
      return !operateResult2.IsSuccess ? operateResult2 : OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecMcNet.RemoteStop" />
    [EstMqttApi]
    public OperateResult RemoteStop()
    {
      OperateResult<byte[]> operateResult1 = this.ReadFromCoreServer(MelsecMcAsciiNet.PackMcCommand(Encoding.ASCII.GetBytes("100200000001"), this.NetworkNumber, this.NetworkStationNumber));
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult operateResult2 = MelsecMcAsciiNet.CheckResponseContent(operateResult1.Content);
      return !operateResult2.IsSuccess ? operateResult2 : OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecMcNet.RemoteReset" />
    [EstMqttApi]
    public OperateResult RemoteReset()
    {
      OperateResult<byte[]> operateResult1 = this.ReadFromCoreServer(MelsecMcAsciiNet.PackMcCommand(Encoding.ASCII.GetBytes("100600000001"), this.NetworkNumber, this.NetworkStationNumber));
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult operateResult2 = MelsecMcAsciiNet.CheckResponseContent(operateResult1.Content);
      return !operateResult2.IsSuccess ? operateResult2 : OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecMcNet.ReadPlcType" />
    [EstMqttApi]
    public OperateResult<string> ReadPlcType()
    {
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(MelsecMcAsciiNet.PackMcCommand(Encoding.ASCII.GetBytes("01010000"), this.NetworkNumber, this.NetworkStationNumber));
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<string>((OperateResult) operateResult);
      OperateResult result = MelsecMcAsciiNet.CheckResponseContent(operateResult.Content);
      return !result.IsSuccess ? OperateResult.CreateFailedResult<string>(result) : OperateResult.CreateSuccessResult<string>(Encoding.ASCII.GetString(operateResult.Content, 22, 16).TrimEnd());
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecMcNet.ErrorStateReset" />
    [EstMqttApi]
    public OperateResult ErrorStateReset()
    {
      OperateResult<byte[]> operateResult1 = this.ReadFromCoreServer(MelsecMcAsciiNet.PackMcCommand(Encoding.ASCII.GetBytes("01010000"), this.NetworkNumber, this.NetworkStationNumber));
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult operateResult2 = MelsecMcAsciiNet.CheckResponseContent(operateResult1.Content);
      return !operateResult2.IsSuccess ? operateResult2 : OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecMcAsciiNet.RemoteRun" />
    public async Task<OperateResult> RemoteRunAsync()
    {
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(MelsecMcAsciiNet.PackMcCommand(Encoding.ASCII.GetBytes("1001000000010000"), this.NetworkNumber, this.NetworkStationNumber));
      if (!read.IsSuccess)
        return (OperateResult) read;
      OperateResult check = MelsecMcAsciiNet.CheckResponseContent(read.Content);
      return check.IsSuccess ? OperateResult.CreateSuccessResult() : check;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecMcAsciiNet.RemoteStop" />
    public async Task<OperateResult> RemoteStopAsync()
    {
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(MelsecMcAsciiNet.PackMcCommand(Encoding.ASCII.GetBytes("100200000001"), this.NetworkNumber, this.NetworkStationNumber));
      if (!read.IsSuccess)
        return (OperateResult) read;
      OperateResult check = MelsecMcAsciiNet.CheckResponseContent(read.Content);
      return check.IsSuccess ? OperateResult.CreateSuccessResult() : check;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecMcAsciiNet.RemoteReset" />
    public async Task<OperateResult> RemoteResetAsync()
    {
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(MelsecMcAsciiNet.PackMcCommand(Encoding.ASCII.GetBytes("100600000001"), this.NetworkNumber, this.NetworkStationNumber));
      if (!read.IsSuccess)
        return (OperateResult) read;
      OperateResult check = MelsecMcAsciiNet.CheckResponseContent(read.Content);
      return check.IsSuccess ? OperateResult.CreateSuccessResult() : check;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecMcAsciiNet.ReadPlcType" />
    public async Task<OperateResult<string>> ReadPlcTypeAsync()
    {
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(MelsecMcAsciiNet.PackMcCommand(Encoding.ASCII.GetBytes("01010000"), this.NetworkNumber, this.NetworkStationNumber));
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<string>((OperateResult) read);
      OperateResult check = MelsecMcAsciiNet.CheckResponseContent(read.Content);
      return check.IsSuccess ? OperateResult.CreateSuccessResult<string>(Encoding.ASCII.GetString(read.Content, 22, 16).TrimEnd()) : OperateResult.CreateFailedResult<string>(check);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecMcAsciiNet.ErrorStateReset" />
    public async Task<OperateResult> ErrorStateResetAsync()
    {
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(MelsecMcAsciiNet.PackMcCommand(Encoding.ASCII.GetBytes("01010000"), this.NetworkNumber, this.NetworkStationNumber));
      if (!read.IsSuccess)
        return (OperateResult) read;
      OperateResult check = MelsecMcAsciiNet.CheckResponseContent(read.Content);
      return check.IsSuccess ? OperateResult.CreateSuccessResult() : check;
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("MelsecMcAsciiNet[{0}:{1}]", (object) this.IpAddress, (object) this.Port);

    /// <summary>将MC协议的核心报文打包成一个可以直接对PLC进行发送的原始报文</summary>
    /// <param name="mcCore">MC协议的核心报文</param>
    /// <param name="networkNumber">网络号</param>
    /// <param name="networkStationNumber">网络站号</param>
    /// <returns>原始报文信息</returns>
    public static byte[] PackMcCommand(
      byte[] mcCore,
      byte networkNumber = 0,
      byte networkStationNumber = 0)
    {
      byte[] numArray = new byte[22 + mcCore.Length];
      numArray[0] = (byte) 53;
      numArray[1] = (byte) 48;
      numArray[2] = (byte) 48;
      numArray[3] = (byte) 48;
      numArray[4] = SoftBasic.BuildAsciiBytesFrom(networkNumber)[0];
      numArray[5] = SoftBasic.BuildAsciiBytesFrom(networkNumber)[1];
      numArray[6] = (byte) 70;
      numArray[7] = (byte) 70;
      numArray[8] = (byte) 48;
      numArray[9] = (byte) 51;
      numArray[10] = (byte) 70;
      numArray[11] = (byte) 70;
      numArray[12] = SoftBasic.BuildAsciiBytesFrom(networkStationNumber)[0];
      numArray[13] = SoftBasic.BuildAsciiBytesFrom(networkStationNumber)[1];
      numArray[14] = SoftBasic.BuildAsciiBytesFrom((ushort) (numArray.Length - 18))[0];
      numArray[15] = SoftBasic.BuildAsciiBytesFrom((ushort) (numArray.Length - 18))[1];
      numArray[16] = SoftBasic.BuildAsciiBytesFrom((ushort) (numArray.Length - 18))[2];
      numArray[17] = SoftBasic.BuildAsciiBytesFrom((ushort) (numArray.Length - 18))[3];
      numArray[18] = (byte) 48;
      numArray[19] = (byte) 48;
      numArray[20] = (byte) 49;
      numArray[21] = (byte) 48;
      mcCore.CopyTo((Array) numArray, 22);
      return numArray;
    }

    /// <summary>从PLC反馈的数据中提取出实际的数据内容，需要传入反馈数据，是否位读取</summary>
    /// <param name="response">反馈的数据内容</param>
    /// <param name="isBit">是否位读取</param>
    /// <returns>解析后的结果对象</returns>
    public static OperateResult<byte[]> ExtractActualData(byte[] response, bool isBit) => isBit ? OperateResult.CreateSuccessResult<byte[]>(((IEnumerable<byte>) response.RemoveBegin<byte>(22)).Select<byte, byte>((Func<byte, byte>) (m => m != (byte) 48 ? (byte) 1 : (byte) 0)).ToArray<byte>()) : OperateResult.CreateSuccessResult<byte[]>(MelsecHelper.TransAsciiByteArrayToByteArray(response.RemoveBegin<byte>(22)));

    /// <summary>检查反馈的内容是否正确的</summary>
    /// <param name="content">MC的反馈的内容</param>
    /// <returns>是否正确</returns>
    public static OperateResult CheckResponseContent(byte[] content)
    {
      ushort uint16 = Convert.ToUInt16(Encoding.ASCII.GetString(content, 18, 4), 16);
      return uint16 > (ushort) 0 ? new OperateResult((int) uint16, MelsecHelper.GetErrorDescription((int) uint16)) : OperateResult.CreateSuccessResult();
    }
  }
}
