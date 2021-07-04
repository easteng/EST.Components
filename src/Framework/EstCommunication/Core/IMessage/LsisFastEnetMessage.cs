// Decompiled with JetBrains decompiler
// Type: EstCommunication.Core.IMessage.LsisFastEnetMessage
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using System;

namespace EstCommunication.Core.IMessage
{
  /// <summary>LSIS的PLC的FastEnet的消息定义</summary>
  public class LsisFastEnetMessage : INetMessage
  {
    /// <inheritdoc cref="P:EstCommunication.Core.IMessage.INetMessage.ProtocolHeadBytesLength" />
    public int ProtocolHeadBytesLength => 20;

    /// <inheritdoc cref="P:EstCommunication.Core.IMessage.INetMessage.HeadBytes" />
    public byte[] HeadBytes { get; set; }

    /// <inheritdoc cref="P:EstCommunication.Core.IMessage.INetMessage.ContentBytes" />
    public byte[] ContentBytes { get; set; }

    /// <inheritdoc cref="M:EstCommunication.Core.IMessage.INetMessage.CheckHeadBytesLegal(System.Byte[])" />
    public bool CheckHeadBytesLegal(byte[] token) => this.HeadBytes != null && this.HeadBytes[0] == (byte) 76;

    /// <inheritdoc cref="M:EstCommunication.Core.IMessage.INetMessage.GetContentLengthByHeadBytes" />
    public int GetContentLengthByHeadBytes()
    {
      byte[] headBytes = this.HeadBytes;
      return headBytes != null && headBytes.Length >= 20 ? (int) BitConverter.ToUInt16(this.HeadBytes, 16) : 0;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IMessage.INetMessage.GetHeadBytesIdentity" />
    public int GetHeadBytesIdentity() => (int) BitConverter.ToUInt16(this.HeadBytes, 14);

    /// <inheritdoc cref="P:EstCommunication.Core.IMessage.INetMessage.SendBytes" />
    public byte[] SendBytes { get; set; }
  }
}
