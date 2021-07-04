// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.AllenBradley.AllenBradleyMicroCip
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

namespace EstCommunication.Profinet.AllenBradley
{
  /// <summary>
  /// AB PLC的cip通信实现类，适用Micro800系列控制系统<br />
  /// AB PLC's cip communication implementation class, suitable for Micro800 series control system
  /// </summary>
  public class AllenBradleyMicroCip : AllenBradleyNet
  {
    /// <inheritdoc cref="M:EstCommunication.Profinet.AllenBradley.AllenBradleyNet.#ctor" />
    public AllenBradleyMicroCip()
    {
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.AllenBradley.AllenBradleyNet.#ctor(System.String,System.Int32)" />
    public AllenBradleyMicroCip(string ipAddress, int port = 44818)
      : base(ipAddress, port)
    {
    }

    /// <inheritdoc />
    protected override byte[] PackCommandService(byte[] portSlot, params byte[][] cips) => AllenBradleyHelper.PackCleanCommandService(portSlot, cips);

    /// <inheritdoc />
    public override string ToString() => string.Format("AllenBradleyMicroCip[{0}:{1}]", (object) this.IpAddress, (object) this.Port);
  }
}
