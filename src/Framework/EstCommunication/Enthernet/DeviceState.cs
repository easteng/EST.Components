// Decompiled with JetBrains decompiler
// Type: EstCommunication.Enthernet.DeviceState
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using System;
using System.Net;
using System.Net.Sockets;

namespace EstCommunication.Enthernet
{
  /// <summary>通用设备的基础状态</summary>
  public class DeviceState
  {
    /// <summary>缓冲内存块</summary>
    internal byte[] Buffer = new byte[1];

    /// <summary>设备的连接地址</summary>
    public IPEndPoint DeviceEndPoint { get; set; }

    /// <summary>设备的连接时间</summary>
    public DateTime ConnectTime { get; set; }

    /// <summary>网络套接字</summary>
    internal Socket WorkSocket { get; set; }

    /// <summary>上次接收到信息的时间</summary>
    public DateTime ReceiveTime { get; set; }

    /// <summary>设备的ip地址</summary>
    public string IpAddress { get; set; }
  }
}
