// Decompiled with JetBrains decompiler
// Type: EstCommunication.Enthernet.NetPushClient
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.Core.Net;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace EstCommunication.Enthernet
{
  /// <summary>发布订阅类的客户端，使用指定的关键订阅相关的数据推送信息</summary>
  /// <remarks>
  /// 详细的使用说明，请参照博客<a href="http://www.cnblogs.com/dathlin/p/8992315.html">http://www.cnblogs.com/dathlin/p/8992315.html</a>
  /// </remarks>
  /// <example>
  /// 此处贴上了Demo项目的服务器配置的示例代码
  /// <code lang="cs" source="TestProject\EstCommunicationDemo\Est\FormPushNet.cs" region="FormPushNet" title="NetPushClient示例" />
  /// </example>
  public class NetPushClient : NetworkXBase
  {
    private readonly IPEndPoint endPoint;
    private readonly string keyWord = string.Empty;
    private Action<NetPushClient, string> action;
    private int reconnectTime = 10000;
    private bool closed = false;

    /// <summary>实例化一个发布订阅类的客户端，需要指定ip地址，端口，及订阅关键字</summary>
    /// <param name="ipAddress">服务器的IP地址</param>
    /// <param name="port">服务器的端口号</param>
    /// <param name="key">订阅关键字</param>
    public NetPushClient(string ipAddress, int port, string key)
    {
      this.endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
      this.keyWord = key;
      if (string.IsNullOrEmpty(key))
        throw new Exception(StringResources.Language.KeyIsNotAllowedNull);
    }

    /// <summary>创建数据推送服务</summary>
    /// <param name="pushCallBack">触发数据推送的委托</param>
    /// <returns>是否创建成功</returns>
    public OperateResult CreatePush(Action<NetPushClient, string> pushCallBack)
    {
      this.action = pushCallBack;
      return this.CreatePush();
    }

    /// <summary>创建数据推送服务，使用事件绑定的机制实现</summary>
    /// <returns>是否创建成功</returns>
    public OperateResult CreatePush()
    {
      this.CoreSocket?.Close();
      OperateResult<Socket> socketAndConnect = this.CreateSocketAndConnect(this.endPoint, 5000);
      if (!socketAndConnect.IsSuccess)
        return (OperateResult) socketAndConnect;
      OperateResult operateResult = this.SendStringAndCheckReceive(socketAndConnect.Content, 0, this.keyWord);
      if (!operateResult.IsSuccess)
        return operateResult;
      OperateResult<int, string> contentFromSocket = this.ReceiveStringContentFromSocket(socketAndConnect.Content);
      if (!contentFromSocket.IsSuccess)
        return (OperateResult) contentFromSocket;
      if ((uint) contentFromSocket.Content1 > 0U)
      {
        socketAndConnect.Content?.Close();
        return new OperateResult(contentFromSocket.Content2);
      }
      AppSession appSession = new AppSession();
      this.CoreSocket = socketAndConnect.Content;
      appSession.WorkSocket = socketAndConnect.Content;
      try
      {
        appSession.WorkSocket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, new AsyncCallback(this.ReceiveCallback), (object) appSession);
      }
      catch (Exception ex)
      {
        this.LogNet?.WriteException(this.ToString(), StringResources.Language.SocketReceiveException, ex);
        return new OperateResult(ex.Message);
      }
      this.closed = false;
      return OperateResult.CreateSuccessResult();
    }

    /// <summary>关闭消息推送的界面</summary>
    public void ClosePush()
    {
      this.action = (Action<NetPushClient, string>) null;
      this.closed = true;
      if (this.CoreSocket != null && this.CoreSocket.Connected)
        this.CoreSocket?.Send(BitConverter.GetBytes(100));
      Thread.Sleep(20);
      this.CoreSocket?.Close();
    }

    private void ReconnectServer(object obj)
    {
      do
      {
        if (!this.closed)
        {
          Console.WriteLine(StringResources.Language.ReConnectServerAfterTenSeconds);
          Thread.Sleep(this.reconnectTime);
          if (this.closed)
            goto label_6;
        }
        else
          goto label_1;
      }
      while (!this.CreatePush().IsSuccess);
      goto label_4;
label_1:
      return;
label_6:
      return;
label_4:
      Console.WriteLine(StringResources.Language.ReConnectServerSuccess);
    }

    private async void ReceiveCallback(IAsyncResult ar)
    {
      if (!(ar.AsyncState is AppSession appSession))
      {
        appSession = (AppSession) null;
      }
      else
      {
        try
        {
          appSession.WorkSocket.EndReceive(ar);
        }
        catch
        {
          ThreadPool.QueueUserWorkItem(new WaitCallback(this.ReconnectServer), (object) null);
          appSession = (AppSession) null;
          return;
        }
        OperateResult<int, int, byte[]> read = await this.ReceiveEstMessageAsync(appSession.WorkSocket);
        if (!read.IsSuccess)
        {
          ThreadPool.QueueUserWorkItem(new WaitCallback(this.ReconnectServer), (object) null);
          appSession = (AppSession) null;
        }
        else
        {
          int protocol = read.Content1;
          int customer = read.Content2;
          byte[] content = read.Content3;
          if (protocol == 1001)
          {
            Action<NetPushClient, string> action = this.action;
            if (action != null)
              action(this, Encoding.Unicode.GetString(content));
            Action<NetPushClient, string> onReceived = this.OnReceived;
            if (onReceived != null)
              onReceived(this, Encoding.Unicode.GetString(content));
          }
          else if (protocol == 1)
            this.Send(appSession.WorkSocket, EstProtocol.CommandBytes(1, 0, this.Token, new byte[0]));
          try
          {
            appSession.WorkSocket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, new AsyncCallback(this.ReceiveCallback), (object) appSession);
          }
          catch
          {
            ThreadPool.QueueUserWorkItem(new WaitCallback(this.ReconnectServer), (object) null);
          }
          read = (OperateResult<int, int, byte[]>) null;
          content = (byte[]) null;
          appSession = (AppSession) null;
        }
      }
    }

    /// <summary>本客户端的关键字</summary>
    public string KeyWord => this.keyWord;

    /// <summary>获取或设置重连服务器的间隔时间，单位：毫秒</summary>
    public int ReConnectTime
    {
      set => this.reconnectTime = value;
      get => this.reconnectTime;
    }

    /// <summary>当接收到数据的事件信息，接收到数据的时候触发。</summary>
    public event Action<NetPushClient, string> OnReceived;

    /// <inheritdoc />
    public override string ToString() => string.Format("NetPushClient[{0}]", (object) this.endPoint);
  }
}
