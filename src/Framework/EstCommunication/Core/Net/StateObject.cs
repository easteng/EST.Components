// Decompiled with JetBrains decompiler
// Type: EstCommunication.Core.Net.StateObject
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using System.Net.Sockets;

namespace EstCommunication.Core.Net
{
  /// <summary>网络中的异步对象</summary>
  internal class StateObject : StateOneBase
  {
    /// <summary>实例化一个对象</summary>
    public StateObject()
    {
    }

    /// <summary>实例化一个对象，指定接收或是发送的数据长度</summary>
    /// <param name="length">数据长度</param>
    public StateObject(int length)
    {
      this.DataLength = length;
      this.Buffer = new byte[length];
    }

    /// <summary>唯一的一串信息</summary>
    public string UniqueId { get; set; }

    /// <summary>网络套接字</summary>
    public Socket WorkSocket { get; set; }

    /// <summary>是否关闭了通道</summary>
    public bool IsClose { get; set; }

    /// <summary>清空旧的数据</summary>
    public void Clear()
    {
      this.IsError = false;
      this.IsClose = false;
      this.AlreadyDealLength = 0;
      this.Buffer = (byte[]) null;
    }
  }
}
