// Decompiled with JetBrains decompiler
// Type: EstCommunication.Core.Net.AppSession
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using System;
using System.Net;
using System.Net.Sockets;

namespace EstCommunication.Core.Net
{
  /// <summary>
  /// 当前的网络会话信息，还包含了一些客户端相关的基本的参数信息<br />
  /// The current network session information also contains some basic parameter information related to the client
  /// </summary>
  public class AppSession
  {
    /// <summary>UDP通信中的远程端</summary>
    internal EndPoint UdpEndPoint = (EndPoint) null;

    /// <summary>实例化一个构造方法</summary>
    public AppSession()
    {
      this.ClientUniqueID = Guid.NewGuid().ToString("N");
      this.HybirdLockSend = new SimpleHybirdLock();
    }

    /// <summary>传输数据的对象</summary>
    internal Socket WorkSocket { get; set; }

    internal SimpleHybirdLock HybirdLockSend { get; set; }

    /// <summary>IP地址</summary>
    public string IpAddress { get; internal set; }

    /// <summary>此连接对象连接的远程客户端</summary>
    public IPEndPoint IpEndPoint { get; internal set; }

    /// <summary>远程对象的别名</summary>
    public string LoginAlias { get; set; }

    /// <summary>心跳验证的时间点</summary>
    public DateTime HeartTime { get; set; } = DateTime.Now;

    /// <summary>客户端的类型</summary>
    public string ClientType { get; set; }

    /// <summary>客户端唯一的标识</summary>
    public string ClientUniqueID { get; private set; }

    /// <summary>指令头缓存</summary>
    internal byte[] BytesHead { get; set; } = new byte[32];

    /// <summary>已经接收的指令头长度</summary>
    internal int AlreadyReceivedHead { get; set; }

    /// <summary>数据内容缓存</summary>
    internal byte[] BytesContent { get; set; }

    /// <summary>已经接收的数据内容长度</summary>
    internal int AlreadyReceivedContent { get; set; }

    /// <summary>用于关键字分类使用</summary>
    internal string KeyGroup { get; set; }

    /// <summary>清除本次的接收内容</summary>
    internal void Clear()
    {
      this.BytesHead = new byte[32];
      this.AlreadyReceivedHead = 0;
      this.BytesContent = (byte[]) null;
      this.AlreadyReceivedContent = 0;
    }

    /// <inheritdoc />
    public override string ToString() => string.IsNullOrEmpty(this.LoginAlias) ? string.Format("AppSession[{0}]", (object) this.IpEndPoint) : string.Format("AppSession[{0}] [{1}]", (object) this.IpEndPoint, (object) this.LoginAlias);
  }
}
