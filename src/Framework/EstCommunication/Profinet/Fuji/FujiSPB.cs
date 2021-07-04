// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.Fuji.FujiSPB
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.Core;
using EstCommunication.Core.Address;
using EstCommunication.Reflection;
using EstCommunication.Serial;
using System.Text;

namespace EstCommunication.Profinet.Fuji
{
  /// <summary>
  /// 富士PLC的SPB协议，详细的地址信息见api文档说明，地址可以携带站号信息，例如：s=2;D100，PLC侧需要配置无BCC计算，包含0D0A结束码<br />
  /// Fuji PLC's SPB protocol. For detailed address information, see the api documentation,
  /// The address can carry station number information, for example: s=2;D100, PLC side needs to be configured with no BCC calculation, including 0D0A end code
  /// </summary>
  /// <remarks>
  /// <inheritdoc cref="T:EstCommunication.Profinet.Fuji.FujiSPBOverTcp" path="remarks" />
  /// </remarks>
  public class FujiSPB : SerialDeviceBase
  {
    private byte station = 1;

    /// <inheritdoc cref="M:EstCommunication.Profinet.Fuji.FujiSPBOverTcp.#ctor" />
    public FujiSPB()
    {
      this.ByteTransform = (IByteTransform) new RegularByteTransform();
      this.WordLength = (ushort) 1;
      this.LogMsgFormatBinary = false;
    }

    /// <inheritdoc cref="P:EstCommunication.Profinet.Fuji.FujiSPBOverTcp.Station" />
    public byte Station
    {
      get => this.station;
      set => this.station = value;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Fuji.FujiSPBOverTcp.Read(System.String,System.UInt16)" />
    [EstMqttApi("ReadByteArray", "")]
    public override OperateResult<byte[]> Read(string address, ushort length)
    {
      OperateResult<byte[]> operateResult1 = FujiSPBOverTcp.BuildReadCommand(this.station, address, length);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult1);
      OperateResult<byte[]> operateResult2 = this.ReadBase(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult2);
      OperateResult<byte[]> operateResult3 = FujiSPBOverTcp.CheckResponseData(operateResult2.Content);
      return !operateResult3.IsSuccess ? OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult3) : OperateResult.CreateSuccessResult<byte[]>(Encoding.ASCII.GetString(operateResult3.Content.RemoveBegin<byte>(4)).ToHexBytes());
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Fuji.FujiSPBOverTcp.Write(System.String,System.Byte[])" />
    [EstMqttApi("WriteByteArray", "")]
    public override OperateResult Write(string address, byte[] value)
    {
      OperateResult<byte[]> operateResult1 = FujiSPBOverTcp.BuildWriteByteCommand(this.station, address, value);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadBase(operateResult1.Content);
      return !operateResult2.IsSuccess ? (OperateResult) operateResult2 : (OperateResult) FujiSPBOverTcp.CheckResponseData(operateResult2.Content);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Fuji.FujiSPBOverTcp.ReadBool(System.String,System.UInt16)" />
    [EstMqttApi("ReadBoolArray", "")]
    public override OperateResult<bool[]> ReadBool(string address, ushort length)
    {
      byte parameter = (byte) EstHelper.ExtractParameter(ref address, "s", (int) this.station);
      OperateResult<FujiSPBAddress> from = FujiSPBAddress.ParseFrom(address);
      if (!from.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) from);
      if ((address.StartsWith("X") || address.StartsWith("Y") || (address.StartsWith("M") || address.StartsWith("L")) || address.StartsWith("TC") || address.StartsWith("CC")) && address.IndexOf('.') < 0)
      {
        from.Content.BitIndex = (int) from.Content.Address % 16;
        from.Content.Address /= (ushort) 16;
      }
      ushort length1 = (ushort) ((from.Content.GetBitIndex() + (int) length - 1) / 16 - from.Content.GetBitIndex() / 16 + 1);
      OperateResult<byte[]> operateResult1 = FujiSPBOverTcp.BuildReadCommand(parameter, from.Content, length1);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult1);
      OperateResult<byte[]> operateResult2 = this.ReadBase(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult2);
      OperateResult<byte[]> operateResult3 = FujiSPBOverTcp.CheckResponseData(operateResult2.Content);
      return !operateResult3.IsSuccess ? OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult3) : OperateResult.CreateSuccessResult<bool[]>(Encoding.ASCII.GetString(operateResult3.Content.RemoveBegin<byte>(4)).ToHexBytes().ToBoolArray().SelectMiddle<bool>(from.Content.BitIndex, (int) length));
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Fuji.FujiSPBOverTcp.Write(System.String,System.Boolean)" />
    [EstMqttApi("WriteBool", "")]
    public override OperateResult Write(string address, bool value)
    {
      OperateResult<byte[]> operateResult1 = FujiSPBOverTcp.BuildWriteBoolCommand(this.station, address, value);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadBase(operateResult1.Content);
      return !operateResult2.IsSuccess ? (OperateResult) operateResult2 : (OperateResult) FujiSPBOverTcp.CheckResponseData(operateResult2.Content);
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("FujiSPB[{0}:{1}]", (object) this.PortName, (object) this.BaudRate);
  }
}
