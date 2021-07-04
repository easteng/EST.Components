// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.Melsec.MelsecMcUdp
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using EstCommunication.Core;
using EstCommunication.Core.Address;
using EstCommunication.Core.Net;
using EstCommunication.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EstCommunication.Profinet.Melsec
{
  /// <summary>
  /// 三菱PLC通讯类，采用UDP的协议实现，采用Qna兼容3E帧协议实现，需要在PLC侧先的以太网模块先进行配置，必须为二进制通讯<br />
  /// Mitsubishi PLC communication class is implemented using UDP protocol and Qna compatible 3E frame protocol.
  /// The Ethernet module needs to be configured first on the PLC side, and it must be binary communication.
  /// </summary>
  /// <remarks>
  /// <inheritdoc cref="T:EstCommunication.Profinet.Melsec.MelsecMcNet" path="remarks" />
  /// </remarks>
  /// <example>
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\melsecTest.cs" region="Usage" title="简单的短连接使用" />
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\melsecTest.cs" region="Usage2" title="简单的长连接使用" />
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\melsecTest.cs" region="ReadExample1" title="基本的读取示例" />
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\melsecTest.cs" region="ReadExample2" title="批量读取示例" />
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\melsecTest.cs" region="ReadExample3" title="随机字读取示例" />
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\melsecTest.cs" region="ReadExample4" title="随机批量字读取示例" />
  /// </example>
  public class MelsecMcUdp : NetworkUdpDeviceBase
  {
    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecMcNet.#ctor" />
    public MelsecMcUdp()
    {
      this.WordLength = (ushort) 1;
      this.ByteTransform = (IByteTransform) new RegularByteTransform();
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecMcNet.#ctor(System.String,System.Int32)" />
    public MelsecMcUdp(string ipAddress, int port)
    {
      this.WordLength = (ushort) 1;
      this.IpAddress = ipAddress;
      this.Port = port;
      this.ByteTransform = (IByteTransform) new RegularByteTransform();
    }

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
        ushort num2 = (ushort) Math.Min((int) length - (int) num1, 900);
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
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(MelsecMcNet.PackMcCommand(MelsecHelper.BuildReadMcCoreCommand(addressData, false), this.NetworkNumber, this.NetworkStationNumber));
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult);
      OperateResult result = MelsecMcNet.CheckResponseContent(operateResult.Content);
      return !result.IsSuccess ? OperateResult.CreateFailedResult<byte[]>(result) : MelsecMcNet.ExtractActualData(SoftBasic.ArrayRemoveBegin<byte>(operateResult.Content, 11), false);
    }

    /// <inheritdoc />
    [EstMqttApi("WriteByteArray", "")]
    public override OperateResult Write(string address, byte[] value)
    {
      OperateResult<McAddressData> operateResult = this.McAnalysisAddress(address, (ushort) 0);
      return !operateResult.IsSuccess ? (OperateResult) OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult) : this.WriteAddressData(operateResult.Content, value);
    }

    private OperateResult WriteAddressData(McAddressData addressData, byte[] value)
    {
      OperateResult<byte[]> operateResult1 = this.ReadFromCoreServer(MelsecMcNet.PackMcCommand(MelsecHelper.BuildWriteWordCoreCommand(addressData, value), this.NetworkNumber, this.NetworkStationNumber));
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult operateResult2 = MelsecMcNet.CheckResponseContent(operateResult1.Content);
      return !operateResult2.IsSuccess ? operateResult2 : OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecMcNet.ReadRandom(System.String[])" />
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
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(MelsecMcNet.PackMcCommand(MelsecHelper.BuildReadRandomWordCommand(address1), this.NetworkNumber, this.NetworkStationNumber));
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult);
      OperateResult result = MelsecMcNet.CheckResponseContent(operateResult.Content);
      return !result.IsSuccess ? OperateResult.CreateFailedResult<byte[]>(result) : MelsecMcNet.ExtractActualData(SoftBasic.ArrayRemoveBegin<byte>(operateResult.Content, 11), false);
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
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(MelsecMcNet.PackMcCommand(MelsecHelper.BuildReadRandomCommand(address1), this.NetworkNumber, this.NetworkStationNumber));
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult);
      OperateResult result = MelsecMcNet.CheckResponseContent(operateResult.Content);
      return !result.IsSuccess ? OperateResult.CreateFailedResult<byte[]>(result) : MelsecMcNet.ExtractActualData(SoftBasic.ArrayRemoveBegin<byte>(operateResult.Content, 11), false);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecMcNet.ReadRandomInt16(System.String[])" />
    public OperateResult<short[]> ReadRandomInt16(string[] address)
    {
      OperateResult<byte[]> operateResult = this.ReadRandom(address);
      return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<short[]>((OperateResult) operateResult) : OperateResult.CreateSuccessResult<short[]>(this.ByteTransform.TransInt16(operateResult.Content, 0, address.Length));
    }

    /// <inheritdoc />
    [EstMqttApi("ReadBoolArray", "")]
    public override OperateResult<bool[]> ReadBool(string address, ushort length)
    {
      OperateResult<McAddressData> operateResult1 = this.McAnalysisAddress(address, length);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult1);
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(MelsecMcNet.PackMcCommand(MelsecHelper.BuildReadMcCoreCommand(operateResult1.Content, true), this.NetworkNumber, this.NetworkStationNumber));
      if (!operateResult2.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult2);
      OperateResult result = MelsecMcNet.CheckResponseContent(operateResult2.Content);
      if (!result.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>(result);
      OperateResult<byte[]> actualData = MelsecMcNet.ExtractActualData(SoftBasic.ArrayRemoveBegin<byte>(operateResult2.Content, 11), true);
      return !actualData.IsSuccess ? OperateResult.CreateFailedResult<bool[]>((OperateResult) actualData) : OperateResult.CreateSuccessResult<bool[]>(((IEnumerable<byte>) actualData.Content).Select<byte, bool>((Func<byte, bool>) (m => m == (byte) 1)).Take<bool>((int) length).ToArray<bool>());
    }

    /// <inheritdoc />
    [EstMqttApi("WriteBoolArray", "")]
    public override OperateResult Write(string address, bool[] values)
    {
      OperateResult<McAddressData> operateResult1 = this.McAnalysisAddress(address, (ushort) 0);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(MelsecMcNet.PackMcCommand(MelsecHelper.BuildWriteBitCoreCommand(operateResult1.Content, values), this.NetworkNumber, this.NetworkStationNumber));
      if (!operateResult2.IsSuccess)
        return (OperateResult) operateResult2;
      OperateResult operateResult3 = MelsecMcNet.CheckResponseContent(operateResult2.Content);
      return !operateResult3.IsSuccess ? operateResult3 : OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecMcNet.RemoteRun(System.Boolean)" />
    [EstMqttApi]
    public OperateResult RemoteRun()
    {
      OperateResult<byte[]> operateResult1 = this.ReadFromCoreServer(MelsecMcNet.PackMcCommand(new byte[8]
      {
        (byte) 1,
        (byte) 16,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0
      }, this.NetworkNumber, this.NetworkStationNumber));
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult operateResult2 = MelsecMcNet.CheckResponseContent(operateResult1.Content);
      return !operateResult2.IsSuccess ? operateResult2 : OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecMcNet.RemoteStop" />
    [EstMqttApi]
    public OperateResult RemoteStop()
    {
      OperateResult<byte[]> operateResult1 = this.ReadFromCoreServer(MelsecMcNet.PackMcCommand(new byte[6]
      {
        (byte) 2,
        (byte) 16,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0
      }, this.NetworkNumber, this.NetworkStationNumber));
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult operateResult2 = MelsecMcNet.CheckResponseContent(operateResult1.Content);
      return !operateResult2.IsSuccess ? operateResult2 : OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecMcNet.RemoteReset" />
    [EstMqttApi]
    public OperateResult RemoteReset()
    {
      OperateResult<byte[]> operateResult1 = this.ReadFromCoreServer(MelsecMcNet.PackMcCommand(new byte[6]
      {
        (byte) 6,
        (byte) 16,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0
      }, this.NetworkNumber, this.NetworkStationNumber));
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult operateResult2 = MelsecMcNet.CheckResponseContent(operateResult1.Content);
      return !operateResult2.IsSuccess ? operateResult2 : OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecMcNet.ReadPlcType" />
    [EstMqttApi]
    public OperateResult<string> ReadPlcType()
    {
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(MelsecMcNet.PackMcCommand(new byte[4]
      {
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 0
      }, this.NetworkNumber, this.NetworkStationNumber));
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<string>((OperateResult) operateResult);
      OperateResult result = MelsecMcNet.CheckResponseContent(operateResult.Content);
      return !result.IsSuccess ? OperateResult.CreateFailedResult<string>(result) : OperateResult.CreateSuccessResult<string>(Encoding.ASCII.GetString(operateResult.Content, 11, 16).TrimEnd());
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecMcNet.ErrorStateReset" />
    [EstMqttApi]
    public OperateResult ErrorStateReset()
    {
      OperateResult<byte[]> operateResult1 = this.ReadFromCoreServer(MelsecMcNet.PackMcCommand(new byte[4]
      {
        (byte) 23,
        (byte) 22,
        (byte) 0,
        (byte) 0
      }, this.NetworkNumber, this.NetworkStationNumber));
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult operateResult2 = MelsecMcNet.CheckResponseContent(operateResult1.Content);
      return !operateResult2.IsSuccess ? operateResult2 : OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("MelsecMcUdp[{0}:{1}]", (object) this.IpAddress, (object) this.Port);
  }
}
