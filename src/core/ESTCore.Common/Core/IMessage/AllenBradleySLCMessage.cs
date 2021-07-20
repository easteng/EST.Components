// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Core.IMessage.AllenBradleySLCMessage
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

namespace ESTCore.Common.Core.IMessage
{
    /// <summary>用于和 AllenBradley PLC 交互的消息协议类</summary>
    public class AllenBradleySLCMessage : INetMessage
    {
        /// <inheritdoc cref="P:ESTCore.Common.Core.IMessage.INetMessage.ProtocolHeadBytesLength" />
        public int ProtocolHeadBytesLength => 28;

        /// <inheritdoc cref="M:ESTCore.Common.Core.IMessage.INetMessage.GetContentLengthByHeadBytes" />
        public int GetContentLengthByHeadBytes() => this.HeadBytes == null ? 0 : (int)this.HeadBytes[2] * 256 + (int)this.HeadBytes[3];

        /// <inheritdoc cref="M:ESTCore.Common.Core.IMessage.INetMessage.CheckHeadBytesLegal(System.Byte[])" />
        public bool CheckHeadBytesLegal(byte[] token) => true;

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
