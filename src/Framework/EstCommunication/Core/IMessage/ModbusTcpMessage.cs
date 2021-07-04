// Decompiled with JetBrains decompiler
// Type: EstCommunication.Core.IMessage.ModbusTcpMessage
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

namespace EstCommunication.Core.IMessage
{
  /// <summary>Modbus-Tcp协议支持的消息解析类</summary>
  public class ModbusTcpMessage : INetMessage
  {
    /// <inheritdoc cref="P:EstCommunication.Core.IMessage.INetMessage.ProtocolHeadBytesLength" />
    public int ProtocolHeadBytesLength => 8;

    /// <inheritdoc cref="M:EstCommunication.Core.IMessage.INetMessage.GetContentLengthByHeadBytes" />
    public int GetContentLengthByHeadBytes()
    {
      int? length = this.HeadBytes?.Length;
      int protocolHeadBytesLength = this.ProtocolHeadBytesLength;
      if (!(length.GetValueOrDefault() >= protocolHeadBytesLength & length.HasValue))
        return 0;
      int num = (int) this.HeadBytes[4] * 256 + (int) this.HeadBytes[5];
      if (num != 0)
        return num - 2;
      byte[] numArray = new byte[this.ProtocolHeadBytesLength - 1];
      for (int index = 0; index < numArray.Length; ++index)
        numArray[index] = this.HeadBytes[index + 1];
      this.HeadBytes = numArray;
      return (int) this.HeadBytes[5] * 256 + (int) this.HeadBytes[6] - 1;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IMessage.INetMessage.CheckHeadBytesLegal(System.Byte[])" />
    public bool CheckHeadBytesLegal(byte[] token)
    {
      if (!this.IsCheckMessageId)
        return true;
      return this.HeadBytes != null && ((int) this.SendBytes[0] == (int) this.HeadBytes[0] && (int) this.SendBytes[1] == (int) this.HeadBytes[1]) && (this.HeadBytes[2] == (byte) 0 && this.HeadBytes[3] == (byte) 0);
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IMessage.INetMessage.GetHeadBytesIdentity" />
    public int GetHeadBytesIdentity() => (int) this.HeadBytes[0] * 256 + (int) this.HeadBytes[1];

    /// <inheritdoc cref="P:EstCommunication.Core.IMessage.INetMessage.HeadBytes" />
    public byte[] HeadBytes { get; set; }

    /// <inheritdoc cref="P:EstCommunication.Core.IMessage.INetMessage.ContentBytes" />
    public byte[] ContentBytes { get; set; }

    /// <inheritdoc cref="P:EstCommunication.Core.IMessage.INetMessage.SendBytes" />
    public byte[] SendBytes { get; set; }

    /// <summary>
    /// 获取或设置是否进行检查返回的消息ID和发送的消息ID是否一致，默认为true，也就是检查<br />
    /// Get or set whether to check whether the returned message ID is consistent with the sent message ID, the default is true, that is, check
    /// </summary>
    public bool IsCheckMessageId { get; set; } = true;
  }
}
