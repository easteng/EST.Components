// Decompiled with JetBrains decompiler
// Type: EstCommunication.Core.IMessage.FetchWriteMessage
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

namespace EstCommunication.Core.IMessage
{
  /// <summary>西门子Fetch/Write消息解析协议</summary>
  public class FetchWriteMessage : INetMessage
  {
    /// <inheritdoc cref="P:EstCommunication.Core.IMessage.INetMessage.ProtocolHeadBytesLength" />
    public int ProtocolHeadBytesLength => 16;

    /// <inheritdoc cref="M:EstCommunication.Core.IMessage.INetMessage.GetContentLengthByHeadBytes" />
    public int GetContentLengthByHeadBytes()
    {
      if (this.HeadBytes[5] == (byte) 5 || this.HeadBytes[5] == (byte) 4)
        return 0;
      if (this.HeadBytes[5] == (byte) 6)
      {
        if (this.SendBytes == null || this.HeadBytes[8] > (byte) 0)
          return 0;
        return this.SendBytes[8] == (byte) 1 || this.SendBytes[8] == (byte) 6 || this.SendBytes[8] == (byte) 7 ? ((int) this.SendBytes[12] * 256 + (int) this.SendBytes[13]) * 2 : (int) this.SendBytes[12] * 256 + (int) this.SendBytes[13];
      }
      if (this.HeadBytes[5] != (byte) 3)
        return 0;
      return this.HeadBytes[8] == (byte) 1 || this.HeadBytes[8] == (byte) 6 || this.HeadBytes[8] == (byte) 7 ? ((int) this.HeadBytes[12] * 256 + (int) this.HeadBytes[13]) * 2 : (int) this.HeadBytes[12] * 256 + (int) this.HeadBytes[13];
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IMessage.INetMessage.CheckHeadBytesLegal(System.Byte[])" />
    public bool CheckHeadBytesLegal(byte[] token) => this.HeadBytes != null && (this.HeadBytes[0] == (byte) 83 && this.HeadBytes[1] == (byte) 53);

    /// <inheritdoc cref="M:EstCommunication.Core.IMessage.INetMessage.GetHeadBytesIdentity" />
    public int GetHeadBytesIdentity() => (int) this.HeadBytes[3];

    /// <inheritdoc cref="P:EstCommunication.Core.IMessage.INetMessage.HeadBytes" />
    public byte[] HeadBytes { get; set; }

    /// <inheritdoc cref="P:EstCommunication.Core.IMessage.INetMessage.ContentBytes" />
    public byte[] ContentBytes { get; set; }

    /// <inheritdoc cref="P:EstCommunication.Core.IMessage.INetMessage.SendBytes" />
    public byte[] SendBytes { get; set; }
  }
}
