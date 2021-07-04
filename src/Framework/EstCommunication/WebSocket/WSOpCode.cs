// Decompiled with JetBrains decompiler
// Type: EstCommunication.WebSocket.WSOpCode
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

namespace EstCommunication.WebSocket
{
  /// <summary>websocket 协议的 op的枚举信息</summary>
  public enum WSOpCode
  {
    /// <summary>连续消息分片</summary>
    ContinuousMessageFragment = 0,
    /// <summary>文本消息分片</summary>
    TextMessageFragment = 1,
    /// <summary>二进制消息分片</summary>
    BinaryMessageFragment = 2,
    /// <summary>连接关闭</summary>
    ConnectionClose = 8,
    /// <summary>心跳检查</summary>
    HeartbeatPing = 9,
    /// <summary>心跳检查</summary>
    HeartbeatPong = 10, // 0x0000000A
  }
}
