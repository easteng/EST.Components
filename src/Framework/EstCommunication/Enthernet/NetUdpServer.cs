// Decompiled with JetBrains decompiler
// Type: EstCommunication.Enthernet.NetUdpServer
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.Core.Net;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EstCommunication.Enthernet
{
  /// <summary>
  /// Udp网络的服务器端类，您可以使用本类构建一个简单的，高性能的udp服务器，接收来自其他客户端的数据，当然，您也可以自定义返回你要返回的数据<br />
  /// Server-side class of Udp network. You can use this class to build a simple, high-performance udp server that receives data from other clients. Of course, you can also customize the data you want to return.
  /// </summary>
  public class NetUdpServer : NetworkServerBase
  {
    /// <summary>获取或设置一次接收时的数据长度，默认2KB数据长度</summary>
    public int ReceiveCacheLength { get; set; } = 2048;

    /// <inheritdoc />
    public override void ServerStart(int port)
    {
      if (this.IsStarted)
        return;
      this.CoreSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
      this.CoreSocket.Bind((EndPoint) new IPEndPoint(IPAddress.Any, port));
      this.RefreshReceive();
      this.LogNet?.WriteInfo(this.ToString(), StringResources.Language.NetEngineStart);
      this.IsStarted = true;
    }

    /// <inheritdoc />
    protected override void CloseAction()
    {
      this.AcceptString = (Action<AppSession, NetHandle, string>) null;
      this.AcceptByte = (Action<AppSession, NetHandle, byte[]>) null;
      base.CloseAction();
    }

    /// <summary>重新开始接收数据</summary>
    /// <exception cref="T:System.ArgumentNullException"></exception>
    private void RefreshReceive()
    {
      AppSession appSession = new AppSession();
      appSession.WorkSocket = this.CoreSocket;
      appSession.UdpEndPoint = (EndPoint) new IPEndPoint(IPAddress.Any, 0);
      appSession.BytesContent = new byte[this.ReceiveCacheLength];
      this.CoreSocket.BeginReceiveFrom(appSession.BytesContent, 0, this.ReceiveCacheLength, SocketFlags.None, ref appSession.UdpEndPoint, new System.AsyncCallback(this.AsyncCallback), (object) appSession);
    }

    private void AsyncCallback(IAsyncResult ar)
    {
      if (!(ar.AsyncState is AppSession asyncState))
        return;
      try
      {
        int from = asyncState.WorkSocket.EndReceiveFrom(ar, ref asyncState.UdpEndPoint);
        this.RefreshReceive();
        if (from >= 32)
        {
          if (this.CheckRemoteToken(asyncState.BytesContent))
          {
            asyncState.IpEndPoint = (IPEndPoint) asyncState.UdpEndPoint;
            int int32_1 = BitConverter.ToInt32(asyncState.BytesContent, 28);
            if (int32_1 == from - 32)
            {
              byte[] head = new byte[32];
              byte[] content1 = new byte[int32_1];
              Array.Copy((Array) asyncState.BytesContent, 0, (Array) head, 0, 32);
              if (int32_1 > 0)
                Array.Copy((Array) asyncState.BytesContent, 32, (Array) content1, 0, int32_1);
              byte[] content2 = EstProtocol.CommandAnalysis(head, content1);
              int int32_2 = BitConverter.ToInt32(head, 0);
              int int32_3 = BitConverter.ToInt32(head, 4);
              this.DataProcessingCenter(asyncState, int32_2, int32_3, content2);
            }
            else
              this.LogNet?.WriteWarn(this.ToString(), string.Format("Should Rece：{0} Actual：{1}", (object) (BitConverter.ToInt32(asyncState.BytesContent, 4) + 8), (object) from));
          }
          else
            this.LogNet?.WriteWarn(this.ToString(), StringResources.Language.TokenCheckFailed);
        }
        else
          this.LogNet?.WriteWarn(this.ToString(), string.Format("Receive error, Actual：{0}", (object) from));
      }
      catch (ObjectDisposedException ex)
      {
      }
      catch (Exception ex)
      {
        this.LogNet?.WriteException(this.ToString(), StringResources.Language.SocketEndReceiveException, ex);
        this.RefreshReceive();
      }
      finally
      {
      }
    }

    /// <summary>数据处理中心</summary>
    /// <param name="session">会话信息</param>
    /// <param name="protocol">暗号</param>
    /// <param name="customer"></param>
    /// <param name="content"></param>
    private void DataProcessingCenter(
      AppSession session,
      int protocol,
      int customer,
      byte[] content)
    {
      switch (protocol)
      {
        case 1001:
          string str = Encoding.Unicode.GetString(content);
          Action<AppSession, NetHandle, string> acceptString = this.AcceptString;
          if (acceptString != null)
            acceptString(session, (NetHandle) customer, str);
          break;
        case 1002:
          Action<AppSession, NetHandle, byte[]> acceptByte = this.AcceptByte;
          if (acceptByte == null)
            break;
          acceptByte(session, (NetHandle) customer, content);
          break;
      }
    }

    /// <summary>向指定的通信对象发送字符串数据</summary>
    /// <param name="session">通信对象</param>
    /// <param name="customer">用户的指令头</param>
    /// <param name="str">实际发送的字符串数据</param>
    public void SendMessage(AppSession session, int customer, string str) => this.SendBytesAsync(session, EstProtocol.CommandBytes(customer, this.Token, str));

    /// <summary>向指定的通信对象发送字节数据</summary>
    /// <param name="session">连接对象</param>
    /// <param name="customer">用户的指令头</param>
    /// <param name="bytes">实际的数据</param>
    public void SendMessage(AppSession session, int customer, byte[] bytes) => this.SendBytesAsync(session, EstProtocol.CommandBytes(customer, this.Token, bytes));

    private void SendBytesAsync(AppSession session, byte[] data)
    {
      try
      {
        session.WorkSocket.SendTo(data, data.Length, SocketFlags.None, session.UdpEndPoint);
      }
      catch (Exception ex)
      {
        this.LogNet?.WriteException("SendMessage", ex);
      }
    }

    /// <summary>当接收到文本数据的时候,触发此事件</summary>
    public event Action<AppSession, NetHandle, string> AcceptString;

    /// <summary>当接收到字节数据的时候,触发此事件</summary>
    public event Action<AppSession, NetHandle, byte[]> AcceptByte;

    /// <inheritdoc />
    public override string ToString() => string.Format("NetUdpServer[{0}]", (object) this.Port);
  }
}
