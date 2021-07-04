// Decompiled with JetBrains decompiler
// Type: EstCommunication.Core.IMessage.MelsecA1EBinaryMessage
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

namespace EstCommunication.Core.IMessage
{
  /// <summary>三菱的A兼容1E帧协议解析规则</summary>
  public class MelsecA1EBinaryMessage : INetMessage
  {
    /// <inheritdoc cref="P:EstCommunication.Core.IMessage.INetMessage.ProtocolHeadBytesLength" />
    public int ProtocolHeadBytesLength => 2;

    /// <inheritdoc cref="M:EstCommunication.Core.IMessage.INetMessage.GetContentLengthByHeadBytes" />
    public int GetContentLengthByHeadBytes()
    {
      if (this.HeadBytes[1] == (byte) 91)
        return 2;
      if (this.HeadBytes[1] != (byte) 0)
        return 0;
      switch (this.HeadBytes[0])
      {
        case 128:
          return this.SendBytes[10] != (byte) 0 ? ((int) this.SendBytes[10] + 1) / 2 : 128;
        case 129:
          return (int) this.SendBytes[10] * 2;
        case 130:
        case 131:
          return 0;
        default:
          return 0;
      }
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IMessage.INetMessage.CheckHeadBytesLegal(System.Byte[])" />
    public bool CheckHeadBytesLegal(byte[] token) => this.HeadBytes != null && (int) this.HeadBytes[0] - (int) this.SendBytes[0] == 128;

    /// <inheritdoc cref="M:EstCommunication.Core.IMessage.INetMessage.GetHeadBytesIdentity" />
    public int GetHeadBytesIdentity() => 0;

    /// <inheritdoc cref="P:EstCommunication.Core.IMessage.INetMessage.HeadBytes" />
    public byte[] HeadBytes { get; set; }

    /// <inheritdoc cref="P:EstCommunication.Core.IMessage.INetMessage.ContentBytes" />
    public byte[] ContentBytes { get; set; }

    /// <inheritdoc cref="P:EstCommunication.Core.IMessage.INetMessage.SendBytes" />
    public byte[] SendBytes { get; set; }
  }
}
