// Decompiled with JetBrains decompiler
// Type: EstCommunication.WebSocket.WebSocketServer
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using EstCommunication.Core.Net;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace EstCommunication.WebSocket
{
  /// <summary>
  /// WebSocket协议的实现，支持创建自定义的websocket服务器，直接给其他的网页端，客户端，手机端发送数据信息，详细看api文档说明<br />
  /// The implementation of the WebSocket protocol supports the creation of custom websocket servers and sends data information directly to other web pages, clients, and mobile phones. See the API documentation for details.
  /// </summary>
  /// <example>
  /// 使用本组件库可以非常简单方便的构造属于你自己的websocket服务器，从而实现和其他的客户端进行通信，尤其是和网页进行通讯，
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\WebSocket\WebSocketServerSample.cs" region="Sample1" title="简单的实例化" />
  /// 当客户端发送数据给服务器的时候，会发一个事件，并且把当前的会话暴露出来，下面举例打印消息，并且演示一个例子，发送数据给指定的会话。
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\WebSocket\WebSocketServerSample.cs" region="Sample2" title="接触数据" />
  /// 也可以在其他地方发送数据给所有的客户端，只要调用一个方法就可以了。
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\WebSocket\WebSocketServerSample.cs" region="Sample3" title="发送数据" />
  /// 当客户端上线之后也触发了当前的事件，我们可以手动捕获到
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\WebSocket\WebSocketServerSample.cs" region="Sample4" title="捕获上线事件" />
  /// 我们再来看看一个高级的操作，实现订阅，大多数的情况，websocket被设计成了订阅发布的操作。基本本服务器可以扩展出非常复杂功能的系统，我们来看一种最简单的操作。
  /// <br />
  /// 客户端给服务器发的数据都视为主题(topic)，这样服务器就可以辨认出主题信息，并追加主题。如下这么操作。
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\WebSocket\WebSocketServerSample.cs" region="Sample5" title="订阅实现" />
  /// 然后在发布的时候，调用下面的代码。
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\WebSocket\WebSocketServerSample.cs" region="Sample6" title="发布数据" />
  /// 可以看到，我们这里只有订阅操作，如果想要实现更为复杂的操作怎么办？丰富客户端发来的数据，携带命令，数据，就可以区分了。比如json数据。具体的实现需要看各位能力了。
  /// </example>
  public class WebSocketServer : NetworkServerBase
  {
    private readonly Dictionary<string, string> retainKeys;
    private readonly object keysLock;
    private bool isRetain = true;
    private readonly List<WebSocketSession> wsSessions = new List<WebSocketSession>();
    private readonly object sessionsLock = new object();
    private System.Threading.Timer timerHeart;

    /// <summary>
    /// 实例化一个默认的对象<br />
    /// Instantiate a default object
    /// </summary>
    public WebSocketServer()
    {
      this.retainKeys = new Dictionary<string, string>();
      this.keysLock = new object();
    }

    /// <inheritdoc />
    public override void ServerStart(int port)
    {
      base.ServerStart(port);
      if (this.KeepAliveSendInterval.TotalMilliseconds <= 0.0)
        return;
      this.timerHeart = new System.Threading.Timer(new TimerCallback(this.ThreadTimerHeartCheck), (object) null, 2000, (int) this.KeepAliveSendInterval.TotalMilliseconds);
    }

    private void ThreadTimerHeartCheck(object obj)
    {
      WebSocketSession[] webSocketSessionArray = (WebSocketSession[]) null;
      lock (this.sessionsLock)
        webSocketSessionArray = this.wsSessions.ToArray();
      if (webSocketSessionArray == null || (uint) webSocketSessionArray.Length <= 0U)
        return;
      for (int index = 0; index < webSocketSessionArray.Length; ++index)
      {
        if (DateTime.Now - webSocketSessionArray[index].ActiveTime > this.KeepAlivePeriod)
          this.RemoveAndCloseSession(webSocketSessionArray[index], "Heart check timeout[" + SoftBasic.GetTimeSpanDescription(DateTime.Now - webSocketSessionArray[index].ActiveTime) + "]");
        else
          this.Send(webSocketSessionArray[index].WsSocket, WebSocketHelper.WebScoketPackData(9, false, "Heart Check"));
      }
    }

    /// <inheritdoc />
    protected override async void ThreadPoolLogin(Socket socket, IPEndPoint endPoint)
    {
      OperateResult<byte[]> headResult = await this.ReceiveAsync(socket, -1, 5000);
      this.HandleWebsocketConnection(socket, endPoint, headResult);
      headResult = (OperateResult<byte[]>) null;
    }

    private async void ReceiveCallback(IAsyncResult ar)
    {
      if (!(ar.AsyncState is WebSocketSession session))
      {
        session = (WebSocketSession) null;
      }
      else
      {
        try
        {
          session.WsSocket.EndReceive(ar);
        }
        catch (Exception ex)
        {
          session.WsSocket?.Close();
          this.LogNet?.WriteDebug(this.ToString(), "ReceiveCallback Failed:" + ex.Message);
          this.RemoveAndCloseSession(session);
          session = (WebSocketSession) null;
          return;
        }
        OperateResult<WebSocketMessage> read = await this.ReceiveWebSocketPayloadAsync(session.WsSocket);
        this.HandleWebsocketMessage(session, read);
        read = (OperateResult<WebSocketMessage>) null;
        session = (WebSocketSession) null;
      }
    }

    private void HandleWebsocketConnection(
      Socket socket,
      IPEndPoint endPoint,
      OperateResult<byte[]> headResult)
    {
      if (!headResult.IsSuccess)
        return;
      string str = Encoding.UTF8.GetString(headResult.Content);
      OperateResult operateResult = WebSocketHelper.CheckWebSocketLegality(str);
      if (!operateResult.IsSuccess)
      {
        socket?.Close();
        this.LogNet?.WriteDebug(this.ToString(), string.Format("[{0}] WebScoket Check Failed:", (object) endPoint) + operateResult.Message + Environment.NewLine + str);
      }
      else
      {
        OperateResult<byte[]> response = WebSocketHelper.GetResponse(str);
        if (!response.IsSuccess)
        {
          socket?.Close();
          this.LogNet?.WriteDebug(this.ToString(), string.Format("[{0}] GetResponse Failed:", (object) endPoint) + response.Message);
        }
        else
        {
          if (!this.Send(socket, response.Content).IsSuccess)
            return;
          WebSocketSession session = new WebSocketSession()
          {
            ActiveTime = DateTime.Now,
            Remote = endPoint,
            WsSocket = socket,
            IsQASession = str.Contains("EstRequestAndAnswer: true") || str.Contains("EstRequestAndAnswer:true")
          };
          Match match = Regex.Match(str, "GET [\\S\\s]+ HTTP/1", RegexOptions.IgnoreCase);
          if (match.Success)
            session.Url = match.Value.Substring(4, match.Value.Length - 11);
          try
          {
            string[] socketSubscribes = WebSocketHelper.GetWebSocketSubscribes(str);
            if (socketSubscribes != null)
            {
              session.Topics = new List<string>((IEnumerable<string>) socketSubscribes);
              if (this.isRetain)
              {
                lock (this.keysLock)
                {
                  for (int index = 0; index < session.Topics.Count; ++index)
                  {
                    if (this.retainKeys.ContainsKey(session.Topics[index]) && !this.Send(socket, WebSocketHelper.WebScoketPackData(1, false, this.retainKeys[session.Topics[index]])).IsSuccess)
                      return;
                  }
                }
              }
            }
            socket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, new AsyncCallback(this.ReceiveCallback), (object) session);
            this.AddWsSession(session);
          }
          catch (Exception ex)
          {
            socket?.Close();
            this.LogNet?.WriteDebug(this.ToString(), string.Format("[{0}] BeginReceive Failed: {1}", (object) session.Remote, (object) ex.Message));
            return;
          }
          WebSocketServer.OnClientConnectedDelegate onClientConnected = this.OnClientConnected;
          if (onClientConnected == null)
            return;
          onClientConnected(session);
        }
      }
    }

    private void HandleWebsocketMessage(
      WebSocketSession session,
      OperateResult<WebSocketMessage> read)
    {
      if (!read.IsSuccess)
      {
        this.RemoveAndCloseSession(session);
      }
      else
      {
        session.ActiveTime = DateTime.Now;
        if (read.Content.OpCode == 8)
        {
          session.WsSocket?.Close();
          this.RemoveAndCloseSession(session, Encoding.UTF8.GetString(read.Content.Payload));
        }
        else
        {
          if (read.Content.OpCode == 9)
          {
            this.LogNet?.WriteDebug(this.ToString(), string.Format("[{0}] PING: {1}", (object) session.Remote, (object) read.Content));
            OperateResult operateResult = this.Send(session.WsSocket, WebSocketHelper.WebScoketPackData(10, false, read.Content.Payload));
            if (!operateResult.IsSuccess)
            {
              this.RemoveAndCloseSession(session, "HandleWebsocketMessage -> 09 opCode send back exception -> " + operateResult.Message);
              return;
            }
          }
          else if (read.Content.OpCode == 10)
          {
            this.LogNet?.WriteDebug(this.ToString(), string.Format("[{0}] PONG: {1}", (object) session.Remote, (object) read.Content));
          }
          else
          {
            WebSocketServer.OnClientApplicationMessageReceiveDelegate applicationMessageReceive = this.OnClientApplicationMessageReceive;
            if (applicationMessageReceive != null)
              applicationMessageReceive(session, read.Content);
          }
          try
          {
            session.WsSocket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, new AsyncCallback(this.ReceiveCallback), (object) session);
          }
          catch (Exception ex)
          {
            session.WsSocket?.Close();
            this.RemoveAndCloseSession(session, "BeginReceive Exception -> " + ex.Message);
          }
        }
      }
    }

    /// <summary>
    ///  websocket的消息收到时触发<br />
    ///  Triggered when a websocket message is received
    /// </summary>
    public event WebSocketServer.OnClientApplicationMessageReceiveDelegate OnClientApplicationMessageReceive;

    /// <summary>
    /// Websocket的客户端连接上来时触发<br />
    /// Triggered when a Websocket client connects
    /// </summary>
    public event WebSocketServer.OnClientConnectedDelegate OnClientConnected;

    /// <summary>
    /// Websocket的客户端下线时触发<br />
    /// Triggered when Websocket client connects
    /// </summary>
    public event WebSocketServer.OnClientConnectedDelegate OnClientDisConnected;

    /// <inheritdoc />
    protected override void StartInitialization()
    {
    }

    /// <inheritdoc />
    protected override void CloseAction()
    {
      base.CloseAction();
      this.CleanWsSession();
    }

    /// <summary>
    /// 向所有的客户端强制发送消息<br />
    /// Force message to all clients
    /// </summary>
    /// <param name="payload">消息内容</param>
    public void PublishAllClientPayload(string payload)
    {
      lock (this.sessionsLock)
      {
        for (int index = 0; index < this.wsSessions.Count; ++index)
        {
          if (!this.wsSessions[index].IsQASession)
          {
            OperateResult operateResult = this.Send(this.wsSessions[index].WsSocket, WebSocketHelper.WebScoketPackData(1, false, payload));
            if (!operateResult.IsSuccess)
              this.LogNet?.WriteError(this.ToString(), string.Format("[{0}] Send Failed: {1}", (object) this.wsSessions[index].Remote, (object) operateResult.Message));
          }
        }
      }
    }

    /// <summary>
    /// 向订阅了topic主题的客户端发送消息<br />
    /// Send messages to clients subscribed to topic
    /// </summary>
    /// <param name="topic">主题</param>
    /// <param name="payload">消息内容</param>
    public void PublishClientPayload(string topic, string payload)
    {
      lock (this.sessionsLock)
      {
        for (int index = 0; index < this.wsSessions.Count; ++index)
        {
          if (!this.wsSessions[index].IsQASession && this.wsSessions[index].IsClientSubscribe(topic))
          {
            OperateResult operateResult = this.Send(this.wsSessions[index].WsSocket, WebSocketHelper.WebScoketPackData(1, false, payload));
            if (!operateResult.IsSuccess)
              this.LogNet?.WriteError(this.ToString(), string.Format("[{0}] Send Failed: {1}", (object) this.wsSessions[index].Remote, (object) operateResult.Message));
          }
        }
      }
      if (!this.isRetain)
        return;
      this.AddTopicRetain(topic, payload);
    }

    /// <summary>
    /// 向指定的客户端发送数据<br />
    /// Send data to the specified client
    /// </summary>
    /// <param name="session">会话内容</param>
    /// <param name="payload">消息内容</param>
    public void SendClientPayload(WebSocketSession session, string payload) => this.Send(session.WsSocket, WebSocketHelper.WebScoketPackData(1, false, payload));

    /// <summary>
    /// 给一个当前的会话信息动态添加订阅的主题<br />
    /// Dynamically add subscribed topics to a current session message
    /// </summary>
    /// <param name="session">会话内容</param>
    /// <param name="topic">主题信息</param>
    public void AddSessionTopic(WebSocketSession session, string topic)
    {
      session.AddTopic(topic);
      this.PublishSessionTopic(session, topic);
    }

    /// <summary>
    /// 获取当前的在线的客户端数量<br />
    /// Get the current number of online clients
    /// </summary>
    public int OnlineCount => this.wsSessions.Count;

    /// <summary>
    /// 获取或设置当前的服务器是否对订阅主题信息缓存，方便订阅客户端立即收到结果，默认开启<br />
    /// Gets or sets whether the current server caches the topic information of the subscription, so that the subscription client can receive the results immediately. It is enabled by default.
    /// </summary>
    public bool IsTopicRetain
    {
      get => this.isRetain;
      set => this.isRetain = value;
    }

    /// <summary>获取当前的在线的客户端信息，可以用于额外的分析或是显示。</summary>
    public WebSocketSession[] OnlineSessions
    {
      get
      {
        WebSocketSession[] webSocketSessionArray = (WebSocketSession[]) null;
        lock (this.sessionsLock)
          webSocketSessionArray = this.wsSessions.ToArray();
        return webSocketSessionArray;
      }
    }

    /// <summary>
    /// 设置的参数，最小单位为1s，当超过设置的时间间隔必须回复PONG报文，否则服务器认定为掉线。默认120秒<br />
    /// Set the minimum unit of the parameter is 1s. When the set time interval is exceeded, the PONG packet must be returned, otherwise the server considers it to be offline. 120 seconds by default
    /// </summary>
    /// <remarks>
    /// 保持连接（Keep Alive）是一个以秒为单位的时间间隔，它是指客户端返回一个PONG报文到下一次返回PONG报文的时候，
    /// 两者之间允许空闲的最大时间间隔。客户端负责保证控制报文发送的时间间隔不超过保持连接的值。
    /// </remarks>
    public TimeSpan KeepAlivePeriod { get; set; } = TimeSpan.FromSeconds(120.0);

    /// <summary>
    /// 获取或是设置用于保持连接的心跳时间的发送间隔。默认30秒钟，需要在服务启动之前设置<br />
    /// Gets or sets the sending interval of the heartbeat time used to keep the connection. 30 seconds by default, need to be set before the service starts
    /// </summary>
    public TimeSpan KeepAliveSendInterval { get; set; } = TimeSpan.FromSeconds(30.0);

    private void CleanWsSession()
    {
      lock (this.sessionsLock)
      {
        for (int index = 0; index < this.wsSessions.Count; ++index)
          this.wsSessions[index].WsSocket?.Close();
        this.wsSessions.Clear();
      }
    }

    private void AddWsSession(WebSocketSession session)
    {
      lock (this.sessionsLock)
        this.wsSessions.Add(session);
      this.LogNet?.WriteDebug(this.ToString(), string.Format("Client[{0}] Online", (object) session.Remote));
    }

    /// <summary>
    /// 让Websocket客户端正常下线，调用本方法即可自由控制会话客户端强制下线操作。<br />
    /// Let the Websocket client go offline normally. Call this method to freely control the session client to force offline operation.
    /// </summary>
    /// <param name="session">当前的会话信息</param>
    /// <param name="reason">下线的原因，默认为空</param>
    public void RemoveAndCloseSession(WebSocketSession session, string reason = null)
    {
      lock (this.sessionsLock)
        this.wsSessions.Remove(session);
      session.WsSocket?.Close();
      this.LogNet?.WriteDebug(this.ToString(), string.Format("Client[{0}]  Offline {1}", (object) session.Remote, (object) reason));
      WebSocketServer.OnClientConnectedDelegate clientDisConnected = this.OnClientDisConnected;
      if (clientDisConnected == null)
        return;
      clientDisConnected(session);
    }

    private void AddTopicRetain(string topic, string payload)
    {
      lock (this.keysLock)
      {
        if (this.retainKeys.ContainsKey(topic))
          this.retainKeys[topic] = payload;
        else
          this.retainKeys.Add(topic, payload);
      }
    }

    private void PublishSessionTopic(WebSocketSession session, string topic)
    {
      bool flag = false;
      string message = string.Empty;
      lock (this.keysLock)
      {
        if (this.retainKeys.ContainsKey(topic))
        {
          flag = true;
          message = this.retainKeys[topic];
        }
      }
      if (!flag)
        return;
      this.Send(session.WsSocket, WebSocketHelper.WebScoketPackData(1, false, message));
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("WebSocketServer[{0}]", (object) this.Port);

    /// <summary>
    /// websocket的消息收到委托<br />
    /// websocket message received delegate
    /// </summary>
    /// <param name="session">当前的会话对象</param>
    /// <param name="message">websocket的消息</param>
    public delegate void OnClientApplicationMessageReceiveDelegate(
      WebSocketSession session,
      WebSocketMessage message);

    /// <summary>
    /// 当前websocket连接上服务器的事件委托<br />
    /// Event delegation of the server on the current websocket connection
    /// </summary>
    /// <param name="session">当前的会话对象</param>
    public delegate void OnClientConnectedDelegate(WebSocketSession session);
  }
}
