// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.Keyence.KeyenceMcAsciiNet
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.Core.Address;
using EstCommunication.Profinet.Melsec;

namespace EstCommunication.Profinet.Keyence
{
  /// <summary>
  /// 基恩士PLC的数据通信类，使用QnA兼容3E帧的通信协议实现，使用ASCII的格式，地址格式需要进行转换成三菱的格式，详细参照备注说明<br />
  /// Keyence PLC's data communication class is implemented using QnA compatible 3E frame communication protocol.
  /// It uses ascii format. The address format needs to be converted to Mitsubishi format.
  /// </summary>
  /// <remarks>
  /// <inheritdoc cref="T:EstCommunication.Profinet.Keyence.KeyenceMcNet" path="remarks" />
  /// </remarks>
  public class KeyenceMcAsciiNet : MelsecMcAsciiNet
  {
    /// <inheritdoc cref="M:EstCommunication.Profinet.Keyence.KeyenceMcNet.#ctor" />
    public KeyenceMcAsciiNet()
    {
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Keyence.KeyenceMcNet.#ctor(System.String,System.Int32)" />
    public KeyenceMcAsciiNet(string ipAddress, int port)
      : base(ipAddress, port)
    {
    }

    /// <inheritdoc />
    protected override OperateResult<McAddressData> McAnalysisAddress(
      string address,
      ushort length)
    {
      return McAddressData.ParseKeyenceFrom(address, length);
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("KeyenceMcAsciiNet[{0}:{1}]", (object) this.IpAddress, (object) this.Port);
  }
}
