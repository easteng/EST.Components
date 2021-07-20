// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.WebSocket.WebSocketMessage
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using System.Text;

namespace ESTCore.Common.WebSocket
{
    /// <summary>
    /// websocket 协议下的单个消息的数据对象<br />
    /// Data object for a single message under the websocket protocol
    /// </summary>
    public class WebSocketMessage
    {
        /// <summary>
        /// 是否存在掩码<br />
        /// Whether a mask exists
        /// </summary>
        public bool HasMask { get; set; }

        /// <summary>
        /// 当前的websocket的操作码<br />
        /// The current websocket opcode
        /// </summary>
        public int OpCode { get; set; }

        /// <summary>负载数据</summary>
        public byte[] Payload { get; set; }

        /// <inheritdoc />
        public override string ToString() => string.Format("OpCode[{0}] HasMask[{1}] Payload: {2}", (object)this.OpCode, (object)this.HasMask, (object)Encoding.UTF8.GetString(this.Payload));
    }
}
