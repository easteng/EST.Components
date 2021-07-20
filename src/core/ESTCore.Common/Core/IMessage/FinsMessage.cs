// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Core.IMessage.FinsMessage
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using System;

namespace ESTCore.Common.Core.IMessage
{
    /// <summary>用于欧姆龙通信的Fins协议的消息解析规则</summary>
    public class FinsMessage : INetMessage
    {
        /// <inheritdoc cref="P:ESTCore.Common.Core.IMessage.INetMessage.ProtocolHeadBytesLength" />
        public int ProtocolHeadBytesLength => 8;

        /// <inheritdoc cref="M:ESTCore.Common.Core.IMessage.INetMessage.GetContentLengthByHeadBytes" />
        public int GetContentLengthByHeadBytes() => BitConverter.ToInt32(new byte[4]
        {
      this.HeadBytes[7],
      this.HeadBytes[6],
      this.HeadBytes[5],
      this.HeadBytes[4]
        }, 0);

        /// <inheritdoc cref="M:ESTCore.Common.Core.IMessage.INetMessage.CheckHeadBytesLegal(System.Byte[])" />
        public bool CheckHeadBytesLegal(byte[] token) => this.HeadBytes != null && (this.HeadBytes[0] == (byte)70 && this.HeadBytes[1] == (byte)73 && this.HeadBytes[2] == (byte)78 && this.HeadBytes[3] == (byte)83);

        /// <inheritdoc cref="M:ESTCore.Common.Core.IMessage.INetMessage.GetHeadBytesIdentity" />
        public int GetHeadBytesIdentity() => 0;

        /// <inheritdoc cref="P:ESTCore.Common.Core.IMessage.INetMessage.HeadBytes" />
        public byte[] HeadBytes { get; set; }

        /// <inheritdoc cref="P:ESTCore.Common.Core.IMessage.INetMessage.ContentBytes" />
        public byte[] ContentBytes { get; set; }

        /// <inheritdoc cref="P:ESTCore.Common.Core.IMessage.INetMessage.SendBytes" />
        public byte[] SendBytes { get; set; }
    }
}
