// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Core.IMessage.MelsecA1EAsciiMessage
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using System;
using System.Text;

namespace ESTCore.Common.Core.IMessage
{
    /// <summary>三菱的A兼容1E帧ASCII协议解析规则</summary>
    public class MelsecA1EAsciiMessage : INetMessage
    {
        /// <inheritdoc cref="P:ESTCore.Common.Core.IMessage.INetMessage.ProtocolHeadBytesLength" />
        public int ProtocolHeadBytesLength => 4;

        /// <inheritdoc cref="M:ESTCore.Common.Core.IMessage.INetMessage.GetContentLengthByHeadBytes" />
        public int GetContentLengthByHeadBytes()
        {
            if (this.HeadBytes[2] == (byte)53 && this.HeadBytes[3] == (byte)66)
                return 4;
            if (this.HeadBytes[2] != (byte)48 || this.HeadBytes[3] != (byte)48)
                return 0;
            int num = Convert.ToInt32(Encoding.ASCII.GetString(this.SendBytes, 20, 2), 16);
            if (num == 0)
                num = 256;
            switch (this.HeadBytes[1])
            {
                case 48:
                    return num % 2 == 1 ? num + 1 : num;
                case 49:
                    return num * 4;
                case 50:
                case 51:
                    return 0;
                default:
                    return 0;
            }
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IMessage.INetMessage.CheckHeadBytesLegal(System.Byte[])" />
        public bool CheckHeadBytesLegal(byte[] token) => this.HeadBytes != null && (int)this.HeadBytes[0] - (int)this.SendBytes[0] == 8;

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
