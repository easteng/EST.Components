// Decompiled with JetBrains decompiler
// Type: EstCommunication.CNC.Fanuc.CNCFanucSeriesMessage
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.Core.IMessage;

namespace EstCommunication.CNC.Fanuc
{
  /// <summary>Fanuc床子的消息对象</summary>
  public class CNCFanucSeriesMessage : INetMessage
  {
    /// <inheritdoc />
    public int ProtocolHeadBytesLength => 10;

    /// <inheritdoc />
    public byte[] HeadBytes { get; set; }

    /// <inheritdoc />
    public byte[] ContentBytes { get; set; }

    /// <inheritdoc />
    public byte[] SendBytes { get; set; }

    /// <inheritdoc />
    public bool CheckHeadBytesLegal(byte[] token) => true;

    /// <inheritdoc />
    public int GetContentLengthByHeadBytes() => (int) this.HeadBytes[8] * 256 + (int) this.HeadBytes[9];

    /// <inheritdoc />
    public int GetHeadBytesIdentity() => 0;

    /// <inheritdoc />
    public override string ToString() => nameof (CNCFanucSeriesMessage);
  }
}
