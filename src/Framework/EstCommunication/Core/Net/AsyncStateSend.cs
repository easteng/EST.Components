// Decompiled with JetBrains decompiler
// Type: EstCommunication.Core.Net.AsyncStateSend
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using System.Net.Sockets;

namespace EstCommunication.Core.Net
{
  internal class AsyncStateSend
  {
    /// <summary>传输数据的对象</summary>
    internal Socket WorkSocket { get; set; }

    /// <summary>发送的数据内容</summary>
    internal byte[] Content { get; set; }

    /// <summary>已经发送长度</summary>
    internal int AlreadySendLength { get; set; }

    internal SimpleHybirdLock HybirdLockSend { get; set; }

    /// <summary>关键字</summary>
    internal string Key { get; set; }

    /// <summary>客户端的标识</summary>
    internal string ClientId { get; set; }
  }
}
