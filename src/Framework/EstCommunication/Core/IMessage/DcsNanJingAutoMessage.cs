// Decompiled with JetBrains decompiler
// Type: EstCommunication.Core.IMessage.DcsNanJingAutoMessage
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

namespace EstCommunication.Core.IMessage
{
  /// <summary>南京自动化研究所推出的DCS设备的消息类</summary>
  public class DcsNanJingAutoMessage : INetMessage
  {
    /// <inheritdoc cref="P:EstCommunication.Core.IMessage.INetMessage.ProtocolHeadBytesLength" />
    public int ProtocolHeadBytesLength => 6;

    /// <inheritdoc cref="M:EstCommunication.Core.IMessage.INetMessage.GetContentLengthByHeadBytes" />
    public int GetContentLengthByHeadBytes()
    {
      int? length = this.HeadBytes?.Length;
      int protocolHeadBytesLength = this.ProtocolHeadBytesLength;
      return length.GetValueOrDefault() >= protocolHeadBytesLength & length.HasValue ? (int) this.HeadBytes[4] * 256 + (int) this.HeadBytes[5] : 0;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IMessage.INetMessage.CheckHeadBytesLegal(System.Byte[])" />
    public bool CheckHeadBytesLegal(byte[] token) => true;

    /// <inheritdoc cref="M:EstCommunication.Core.IMessage.INetMessage.GetHeadBytesIdentity" />
    public int GetHeadBytesIdentity() => (int) this.HeadBytes[0] * 256 + (int) this.HeadBytes[1];

    /// <inheritdoc cref="P:EstCommunication.Core.IMessage.INetMessage.HeadBytes" />
    public byte[] HeadBytes { get; set; }

    /// <inheritdoc cref="P:EstCommunication.Core.IMessage.INetMessage.ContentBytes" />
    public byte[] ContentBytes { get; set; }

    /// <inheritdoc cref="P:EstCommunication.Core.IMessage.INetMessage.SendBytes" />
    public byte[] SendBytes { get; set; }
  }
}
