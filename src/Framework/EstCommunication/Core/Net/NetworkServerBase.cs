// Decompiled with JetBrains decompiler
// Type: EstCommunication.Core.Net.NetworkServerBase
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.Core.IMessage;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EstCommunication.Core.Net
{
  /// <summary>
  /// 服务器程序的基础类，提供了启动服务器的基本实现，方便后续的扩展操作。<br />
  /// The basic class of the server program provides the basic implementation of starting the server to facilitate subsequent expansion operations.
  /// </summary>
  public class NetworkServerBase : NetworkXBase
  {
    /// <summary>
    /// 实例化一个默认的对象<br />
    /// Instantiate a default object
    /// </summary>
    public NetworkServerBase()
    {
      this.IsStarted = false;
      this.Port = 0;
    }

    /// <summary>
    /// 服务器引擎是否启动<br />
    /// Whether the server engine is started
    /// </summary>
    public bool IsStarted { get; protected set; }

    /// <summary>
    /// 获取或设置服务器的端口号，如果是设置，需要在服务器启动前设置完成，才能生效。<br />
    /// Gets or sets the port number of the server. If it is set, it needs to be set before the server starts to take effect.
    /// </summary>
    /// <remarks>需要在服务器启动之前设置为有效</remarks>
    public int Port { get; set; }

    /// <summary>
    /// 异步传入的连接申请请求<br />
    /// Asynchronous incoming connection request
    /// </summary>
    /// <param name="iar">异步对象</param>
    protected void AsyncAcceptCallback(IAsyncResult iar)
    {
      if (!(iar.AsyncState is Socket asyncState))
        return;
      Socket socket = (Socket) null;
      try
      {
        socket = asyncState.EndAccept(iar);
        ThreadPool.QueueUserWorkItem(new WaitCallback(this.ThreadPoolLogin), (object) socket);
      }
      catch (ObjectDisposedException ex)
      {
        return;
      }
      catch (Exception ex)
      {
        socket?.Close();
        this.LogNet?.WriteException(this.ToString(), StringResources.Language.SocketAcceptCallbackException, ex);
      }
      int num = 0;
      while (num < 3)
      {
        try
        {
          asyncState.BeginAccept(new AsyncCallback(this.AsyncAcceptCallback), (object) asyncState);
          break;
        }
        catch (Exception ex)
        {
          Thread.Sleep(1000);
          this.LogNet?.WriteException(this.ToString(), StringResources.Language.SocketReAcceptCallbackException, ex);
          ++num;
        }
      }
      if (num >= 3)
      {
        this.LogNet?.WriteError(this.ToString(), StringResources.Language.SocketReAcceptCallbackException);
        throw new Exception(StringResources.Language.SocketReAcceptCallbackException);
      }
    }

    private void ThreadPoolLogin(object obj)
    {
      if (!(obj is Socket socket))
        return;
      IPEndPoint remoteEndPoint = (IPEndPoint) socket.RemoteEndPoint;
      OperateResult operateResult = this.SocketAcceptExtraCheck(socket, remoteEndPoint);
      if (!operateResult.IsSuccess)
      {
        this.LogNet?.WriteDebug(this.ToString(), string.Format("[{0}] Socket Accept Extra Check Failed : {1}", (object) remoteEndPoint, (object) operateResult.Message));
        socket?.Close();
      }
      else
        this.ThreadPoolLogin(socket, remoteEndPoint);
    }

    /// <summary>
    /// 当客户端连接到服务器，并听过额外的检查后，进行回调的方法<br />
    /// Callback method when the client connects to the server and has heard additional checks
    /// </summary>
    /// <param name="socket">socket对象</param>
    /// <param name="endPoint">远程的终结点</param>
    protected virtual void ThreadPoolLogin(Socket socket, IPEndPoint endPoint) => socket?.Close();

    /// <summary>
    /// 当客户端的socket登录的时候额外检查的操作，并返回操作的结果信息。<br />
    /// The operation is additionally checked when the client's socket logs in, and the result information of the operation is returned.
    /// </summary>
    /// <param name="socket">套接字</param>
    /// <param name="endPoint">终结点</param>
    /// <returns>验证的结果</returns>
    protected virtual OperateResult SocketAcceptExtraCheck(
      Socket socket,
      IPEndPoint endPoint)
    {
      return OperateResult.CreateSuccessResult();
    }

    /// <summary>
    /// 服务器启动时额外的初始化信息，可以用于启动一些额外的服务的操作。<br />
    /// The extra initialization information when the server starts can be used to start some additional service operations.
    /// </summary>
    /// <remarks>需要在派生类中重写</remarks>
    protected virtual void StartInitialization()
    {
    }

    /// <summary>
    /// 指定端口号来启动服务器的引擎<br />
    /// Specify the port number to start the server's engine
    /// </summary>
    /// <param name="port">指定一个端口号</param>
    public virtual void ServerStart(int port)
    {
      if (this.IsStarted)
        return;
      this.StartInitialization();
      this.CoreSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      this.CoreSocket.Bind((EndPoint) new IPEndPoint(IPAddress.Any, port));
      this.CoreSocket.Listen(500);
      this.CoreSocket.BeginAccept(new AsyncCallback(this.AsyncAcceptCallback), (object) this.CoreSocket);
      this.IsStarted = true;
      this.Port = port;
      this.LogNet?.WriteInfo(this.ToString(), StringResources.Language.NetEngineStart);
    }

    /// <summary>
    /// 使用已经配置好的端口启动服务器的引擎<br />
    /// Use the configured port to start the server's engine
    /// </summary>
    public void ServerStart() => this.ServerStart(this.Port);

    /// <summary>
    /// 服务器关闭的时候需要做的事情<br />
    /// Things to do when the server is down
    /// </summary>
    protected virtual void CloseAction()
    {
    }

    /// <summary>
    /// 关闭服务器的引擎<br />
    /// Shut down the server's engine
    /// </summary>
    public virtual void ServerClose()
    {
      if (!this.IsStarted)
        return;
      this.IsStarted = false;
      this.CloseAction();
      this.CoreSocket?.Close();
      this.LogNet?.WriteInfo(this.ToString(), StringResources.Language.NetEngineClose);
    }

    /// <summary>
    /// 创建一个指定的异形客户端连接，使用Est协议来发送注册包<br />
    /// Create a specified profiled client connection and use the Est protocol to send registration packets
    /// </summary>
    /// <param name="ipAddress">Ip地址</param>
    /// <param name="port">端口号</param>
    /// <param name="dtuId">设备唯一ID号，最长11</param>
    /// <returns>是否成功连接</returns>
    public OperateResult ConnectEstAlientClient(
      string ipAddress,
      int port,
      string dtuId)
    {
      if (dtuId.Length > 11)
        dtuId = dtuId.Substring(11);
      byte[] data = new byte[28];
      data[0] = (byte) 72;
      data[1] = (byte) 115;
      data[2] = (byte) 110;
      data[3] = (byte) 0;
      data[4] = (byte) 23;
      Encoding.ASCII.GetBytes(dtuId).CopyTo((Array) data, 5);
      OperateResult<Socket> socketAndConnect = this.CreateSocketAndConnect(ipAddress, port, 10000);
      if (!socketAndConnect.IsSuccess)
        return (OperateResult) socketAndConnect;
      OperateResult operateResult = this.Send(socketAndConnect.Content, data);
      if (!operateResult.IsSuccess)
        return operateResult;
      OperateResult<byte[]> byMessage = this.ReceiveByMessage(socketAndConnect.Content, 10000, (INetMessage) new AlienMessage());
      if (!byMessage.IsSuccess)
        return (OperateResult) byMessage;
      switch (byMessage.Content[5])
      {
        case 1:
          socketAndConnect.Content?.Close();
          return new OperateResult(StringResources.Language.DeviceCurrentIsLoginRepeat);
        case 2:
          socketAndConnect.Content?.Close();
          return new OperateResult(StringResources.Language.DeviceCurrentIsLoginForbidden);
        case 3:
          socketAndConnect.Content?.Close();
          return new OperateResult(StringResources.Language.PasswordCheckFailed);
        default:
          this.ThreadPoolLogin((object) socketAndConnect.Content);
          return OperateResult.CreateSuccessResult();
      }
    }

    /// <inheritdoc cref="M:EstCommunication.Core.Net.NetworkServerBase.ConnectEstAlientClient(System.String,System.Int32,System.String)" />
    public async Task<OperateResult> ConnectEstAlientClientAsync(
      string ipAddress,
      int port,
      string dtuId)
    {
      if (dtuId.Length > 11)
        dtuId = dtuId.Substring(11);
      byte[] sendBytes = new byte[28];
      sendBytes[0] = (byte) 72;
      sendBytes[1] = (byte) 115;
      sendBytes[2] = (byte) 110;
      sendBytes[3] = (byte) 0;
      sendBytes[4] = (byte) 23;
      Encoding.ASCII.GetBytes(dtuId).CopyTo((Array) sendBytes, 5);
      OperateResult<Socket> connect = await this.CreateSocketAndConnectAsync(ipAddress, port, 10000);
      if (!connect.IsSuccess)
        return (OperateResult) connect;
      OperateResult send = await this.SendAsync(connect.Content, sendBytes);
      if (!send.IsSuccess)
        return send;
      OperateResult<byte[]> receive = await this.ReceiveByMessageAsync(connect.Content, 10000, (INetMessage) new AlienMessage());
      if (!receive.IsSuccess)
        return (OperateResult) receive;
      byte num = receive.Content[5];
      switch (num)
      {
        case 1:
          connect.Content?.Close();
          return new OperateResult(StringResources.Language.DeviceCurrentIsLoginRepeat);
        case 2:
          connect.Content?.Close();
          return new OperateResult(StringResources.Language.DeviceCurrentIsLoginForbidden);
        case 3:
          connect.Content?.Close();
          return new OperateResult(StringResources.Language.PasswordCheckFailed);
        default:
          this.ThreadPoolLogin((object) connect.Content);
          return OperateResult.CreateSuccessResult();
      }
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("NetworkServerBase[{0}]", (object) this.Port);
  }
}
