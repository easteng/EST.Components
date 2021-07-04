// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.LSIS.XGBCnet
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using EstCommunication.Core;
using EstCommunication.Reflection;
using EstCommunication.Serial;
using System.Threading.Tasks;

namespace EstCommunication.Profinet.LSIS
{
  /// <summary>
  /// XGB Cnet I/F module supports Serial Port. The address can carry station number information, for example: s=2;D100
  /// </summary>
  /// <remarks>
  /// <inheritdoc cref="T:EstCommunication.Profinet.LSIS.XGBCnetOverTcp" path="remarks" />
  /// </remarks>
  public class XGBCnet : SerialDeviceBase
  {
    /// <summary>Instantiate a Default object</summary>
    public XGBCnet()
    {
      this.ByteTransform = (IByteTransform) new RegularByteTransform();
      this.WordLength = (ushort) 2;
    }

    /// <inheritdoc cref="P:EstCommunication.Profinet.LSIS.XGBCnetOverTcp.Station" />
    public byte Station { get; set; } = 5;

    /// <inheritdoc cref="M:EstCommunication.Profinet.LSIS.XGBCnetOverTcp.ReadByte(System.String)" />
    [EstMqttApi("ReadByte", "")]
    public OperateResult<byte> ReadByte(string address) => ByteTransformHelper.GetResultFromArray<byte>(this.Read(address, (ushort) 2));

    /// <inheritdoc cref="M:EstCommunication.Profinet.LSIS.XGBCnetOverTcp.Write(System.String,System.Byte)" />
    [EstMqttApi("WriteByte", "")]
    public OperateResult Write(string address, byte value) => this.Write(address, new byte[1]
    {
      value
    });

    /// <inheritdoc />
    [EstMqttApi("ReadBoolArray", "")]
    public override OperateResult<bool[]> ReadBool(string address, ushort length)
    {
      OperateResult<byte[]> operateResult1 = XGBCnetOverTcp.BuildReadOneCommand((byte) EstHelper.ExtractParameter(ref address, "s", (int) this.Station), address, length);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult1);
      OperateResult<byte[]> operateResult2 = this.ReadBase(operateResult1.Content);
      return !operateResult2.IsSuccess ? OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult2) : OperateResult.CreateSuccessResult<bool[]>(SoftBasic.ByteToBoolArray(XGBCnetOverTcp.ExtractActualData(operateResult2.Content, true).Content, (int) length));
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.LSIS.XGBCnetOverTcp.ReadCoil(System.String)" />
    public OperateResult<bool> ReadCoil(string address) => this.ReadBool(address);

    /// <inheritdoc cref="M:EstCommunication.Profinet.LSIS.XGBCnetOverTcp.ReadCoil(System.String,System.UInt16)" />
    public OperateResult<bool[]> ReadCoil(string address, ushort length) => this.ReadBool(address, length);

    /// <inheritdoc cref="M:EstCommunication.Profinet.LSIS.XGBCnet.WriteCoil(System.String,System.Boolean)" />
    public OperateResult WriteCoil(string address, bool value) => this.Write(address, value);

    /// <inheritdoc />
    [EstMqttApi("WriteBool", "")]
    public override OperateResult Write(string address, bool value) => this.Write(address, new byte[1]
    {
      value ? (byte) 1 : (byte) 0
    });

    /// <inheritdoc />
    public override async Task<OperateResult> WriteAsync(
      string address,
      bool value)
    {
      OperateResult operateResult = await this.WriteAsync(address, new byte[1]
      {
        value ? (byte) 1 : (byte) 0
      });
      return operateResult;
    }

    /// <inheritdoc />
    [EstMqttApi("ReadByteArray", "")]
    public override OperateResult<byte[]> Read(string address, ushort length)
    {
      OperateResult<byte[]> operateResult1 = XGBCnetOverTcp.BuildReadCommand((byte) EstHelper.ExtractParameter(ref address, "s", (int) this.Station), address, length);
      if (!operateResult1.IsSuccess)
        return operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadBase(operateResult1.Content);
      return !operateResult2.IsSuccess ? operateResult2 : XGBCnetOverTcp.ExtractActualData(operateResult2.Content, true);
    }

    /// <inheritdoc />
    [EstMqttApi("WriteByteArray", "")]
    public override OperateResult Write(string address, byte[] value)
    {
      OperateResult<byte[]> operateResult1 = XGBCnetOverTcp.BuildWriteCommand((byte) EstHelper.ExtractParameter(ref address, "s", (int) this.Station), address, value);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadBase(operateResult1.Content);
      return !operateResult2.IsSuccess ? (OperateResult) operateResult2 : (OperateResult) XGBCnetOverTcp.ExtractActualData(operateResult2.Content, false);
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("XGBCnet[{0}:{1}]", (object) this.PortName, (object) this.BaudRate);
  }
}
