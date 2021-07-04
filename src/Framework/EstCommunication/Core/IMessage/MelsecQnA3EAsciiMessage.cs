// Decompiled with JetBrains decompiler
// Type: EstCommunication.Core.IMessage.MelsecQnA3EAsciiMessage
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using System;
using System.Text;

namespace EstCommunication.Core.IMessage
{
  /// <summary>基于MC协议的Qna兼容3E帧协议的ASCII通讯消息机制</summary>
  public class MelsecQnA3EAsciiMessage : INetMessage
  {
    /// <inheritdoc cref="P:EstCommunication.Core.IMessage.INetMessage.ProtocolHeadBytesLength" />
    public int ProtocolHeadBytesLength => 18;

    /// <inheritdoc cref="M:EstCommunication.Core.IMessage.INetMessage.GetContentLengthByHeadBytes" />
    public int GetContentLengthByHeadBytes() => Convert.ToInt32(Encoding.ASCII.GetString(new byte[4]
    {
      this.HeadBytes[14],
      this.HeadBytes[15],
      this.HeadBytes[16],
      this.HeadBytes[17]
    }), 16);

    /// <inheritdoc cref="M:EstCommunication.Core.IMessage.INetMessage.CheckHeadBytesLegal(System.Byte[])" />
    public bool CheckHeadBytesLegal(byte[] token) => this.HeadBytes != null && (this.HeadBytes[0] == (byte) 68 && this.HeadBytes[1] == (byte) 48 && this.HeadBytes[2] == (byte) 48 && this.HeadBytes[3] == (byte) 48);

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
