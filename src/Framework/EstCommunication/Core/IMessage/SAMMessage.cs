// Decompiled with JetBrains decompiler
// Type: EstCommunication.Core.IMessage.SAMMessage
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

namespace EstCommunication.Core.IMessage
{
  /// <summary>SAM身份证通信协议的消息</summary>
  public class SAMMessage : INetMessage
  {
    /// <inheritdoc cref="P:EstCommunication.Core.IMessage.INetMessage.ProtocolHeadBytesLength" />
    public int ProtocolHeadBytesLength => 7;

    /// <inheritdoc cref="P:EstCommunication.Core.IMessage.INetMessage.HeadBytes" />
    public byte[] HeadBytes { get; set; }

    /// <inheritdoc cref="P:EstCommunication.Core.IMessage.INetMessage.ContentBytes" />
    public byte[] ContentBytes { get; set; }

    /// <inheritdoc cref="M:EstCommunication.Core.IMessage.INetMessage.CheckHeadBytesLegal(System.Byte[])" />
    public bool CheckHeadBytesLegal(byte[] token) => this.HeadBytes != null && (this.HeadBytes[0] == (byte) 170 && this.HeadBytes[1] == (byte) 170 && (this.HeadBytes[2] == (byte) 170 && this.HeadBytes[3] == (byte) 150) && this.HeadBytes[4] == (byte) 105);

    /// <inheritdoc cref="M:EstCommunication.Core.IMessage.SAMMessage.GetContentLengthByHeadBytes" />
    public int GetContentLengthByHeadBytes()
    {
      byte[] headBytes = this.HeadBytes;
      return headBytes != null && headBytes.Length >= 7 ? (int) this.HeadBytes[5] * 256 + (int) this.HeadBytes[6] : 0;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IMessage.INetMessage.GetHeadBytesIdentity" />
    public int GetHeadBytesIdentity() => 0;

    /// <inheritdoc cref="P:EstCommunication.Core.IMessage.INetMessage.SendBytes" />
    public byte[] SendBytes { get; set; }
  }
}
