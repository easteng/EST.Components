// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Enthernet.Redis.RedisSubscribe
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Core.Net;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ESTCore.Common.Enthernet.Redis
{
    /// <summary>
    /// Redis协议的订阅操作，一个对象订阅一个或是多个频道的信息，当发生网络异常的时候，内部会进行自动重连，并恢复之前的订阅信息。<br />
    /// In the subscription operation of the Redis protocol, an object subscribes to the information of one or more channels.
    /// When a network abnormality occurs, the internal will automatically reconnect and restore the previous subscription information.
    /// </summary>
    public class RedisSubscribe : NetworkXBase
    {
        private IPEndPoint endPoint;
        private List<string> keyWords = (List<string>)null;
        private object listLock = new object();
        private int reconnectTime = 10000;
        private int connectTimeOut = 5000;

        /// <summary>
        /// 实例化一个发布订阅类的客户端，需要指定ip地址，端口。<br />
        /// To instantiate a publish and subscribe client, you need to specify the ip address and port.
        /// </summary>
        /// <param name="ipAddress">服务器的IP地址</param>
        /// <param name="port">服务器的端口号</param>
        public RedisSubscribe(string ipAddress, int port)
        {
            this.endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            this.keyWords = new List<string>();
        }

        /// <summary>
        /// 实例化一个发布订阅类的客户端，需要指定ip地址，端口，及订阅关键字。<br />
        /// To instantiate a publish-subscribe client, you need to specify the ip address, port, and subscription keyword.
        /// </summary>
        /// <param name="ipAddress">服务器的IP地址</param>
        /// <param name="port">服务器的端口号</param>
        /// <param name="keys">订阅关键字</param>
        public RedisSubscribe(string ipAddress, int port, string[] keys)
        {
            this.endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            this.keyWords = new List<string>((IEnumerable<string>)keys);
        }

        /// <summary>
        /// 实例化一个发布订阅类的客户端，需要指定ip地址，端口，及订阅关键字。<br />
        /// To instantiate a publish-subscribe client, you need to specify the ip address, port, and subscription keyword.
        /// </summary>
        /// <param name="ipAddress">服务器的IP地址</param>
        /// <param name="port">服务器的端口号</param>
        /// <param name="key">订阅关键字</param>
        public RedisSubscribe(string ipAddress, int port, string key)
        {
            this.endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            this.keyWords = new List<string>() { key };
        }

        private OperateResult CreatePush()
        {
            this.CoreSocket?.Close();
            OperateResult<Socket> socketAndConnect = this.CreateSocketAndConnect(this.endPoint, this.connectTimeOut);
            if (!socketAndConnect.IsSuccess)
                return (OperateResult)socketAndConnect;
            if (!string.IsNullOrEmpty(this.Password))
            {
                OperateResult operateResult = this.Send(socketAndConnect.Content, RedisHelper.PackStringCommand(new string[2]
                {
          "AUTH",
          this.Password
                }));
                if (!operateResult.IsSuccess)
                    return operateResult;
                OperateResult<byte[]> redisCommand = this.ReceiveRedisCommand(socketAndConnect.Content);
                if (!redisCommand.IsSuccess)
                    return (OperateResult)redisCommand;
                string msg = Encoding.UTF8.GetString(redisCommand.Content);
                if (!msg.StartsWith("+OK"))
                    return new OperateResult(msg);
            }
            List<string> keyWords = this.keyWords;
            // ISSUE: explicit non-virtual call
            if (keyWords != null && (keyWords.Count) > 0)
            {
                OperateResult operateResult = this.Send(socketAndConnect.Content, RedisHelper.PackSubscribeCommand(this.keyWords.ToArray()));
                if (!operateResult.IsSuccess)
                    return operateResult;
            }
            this.CoreSocket = socketAndConnect.Content;
            try
            {
                socketAndConnect.Content.BeginReceive(new byte[0], 0, 0, SocketFlags.None, new AsyncCallback(this.ReceiveCallBack), (object)socketAndConnect.Content);
            }
            catch (Exception ex)
            {
                return new OperateResult(ex.Message);
            }
            return OperateResult.CreateSuccessResult();
        }

        private void ReceiveCallBack(IAsyncResult ar)
        {
            if (!(ar.AsyncState is Socket asyncState))
                return;
            try
            {
                asyncState.EndReceive(ar);
            }
            catch (ObjectDisposedException ex)
            {
                this.LogNet?.WriteWarn("Socket Disposed!");
                return;
            }
            catch (Exception ex)
            {
                this.SocketReceiveException(ex);
                return;
            }
            OperateResult<byte[]> redisCommand = this.ReceiveRedisCommand(asyncState);
            if (!redisCommand.IsSuccess)
            {
                this.SocketReceiveException((Exception)null);
            }
            else
            {
                try
                {
                    asyncState.BeginReceive(new byte[0], 0, 0, SocketFlags.None, new AsyncCallback(this.ReceiveCallBack), (object)asyncState);
                }
                catch (Exception ex)
                {
                    this.SocketReceiveException(ex);
                    return;
                }
                OperateResult<string[]> stringsFromCommandLine = RedisHelper.GetStringsFromCommandLine(redisCommand.Content);
                if (!stringsFromCommandLine.IsSuccess)
                {
                    this.LogNet?.WriteWarn(stringsFromCommandLine.Message);
                }
                else
                {
                    if (stringsFromCommandLine.Content[0].ToUpper() == "SUBSCRIBE")
                        return;
                    if (stringsFromCommandLine.Content[0].ToUpper() == "MESSAGE")
                    {
                        RedisSubscribe.RedisMessageReceiveDelegate redisMessageReceived = this.OnRedisMessageReceived;
                        if (redisMessageReceived != null)
                            redisMessageReceived(stringsFromCommandLine.Content[1], stringsFromCommandLine.Content[2]);
                    }
                    else
                        this.LogNet?.WriteWarn(stringsFromCommandLine.Content[0]);
                }
            }
        }

        private void SocketReceiveException(Exception ex)
        {
            do
            {
                if (ex != null)
                    this.LogNet?.WriteException("Offline", ex);
                Console.WriteLine(StringResources.Language.ReConnectServerAfterTenSeconds);
                Thread.Sleep(this.reconnectTime);
            }
            while (!this.CreatePush().IsSuccess);
            Console.WriteLine(StringResources.Language.ReConnectServerSuccess);
        }

        private void AddSubTopics(string[] topics)
        {
            lock (this.listLock)
            {
                for (int index = 0; index < topics.Length; ++index)
                {
                    if (!this.keyWords.Contains(topics[index]))
                        this.keyWords.Add(topics[index]);
                }
            }
        }

        private void RemoveSubTopics(string[] topics)
        {
            lock (this.listLock)
            {
                for (int index = 0; index < topics.Length; ++index)
                {
                    if (this.keyWords.Contains(topics[index]))
                        this.keyWords.Remove(topics[index]);
                }
            }
        }

        /// <summary>
        /// 如果Redis服务器设置了密码，此处就需要进行设置。必须在 <see cref="M:ESTCore.Common.Enthernet.Redis.RedisSubscribe.ConnectServer" /> 方法调用前设置。<br />
        /// If the Redis server has set a password, it needs to be set here. Must be set before the <see cref="M:ESTCore.Common.Enthernet.Redis.RedisSubscribe.ConnectServer" /> method is called.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 获取或设置当前连接超时时间，主要对 <see cref="M:ESTCore.Common.Enthernet.Redis.RedisSubscribe.ConnectServer" /> 方法有影响，默认值为 5000，也即是5秒。<br />
        /// Get or set the current connection timeout period, which mainly affects the <see cref="M:ESTCore.Common.Enthernet.Redis.RedisSubscribe.ConnectServer" /> method. The default value is 5000, which is 5 seconds.
        /// </summary>
        public int ConnectTimeOut
        {
            get => this.connectTimeOut;
            set => this.connectTimeOut = value;
        }

        /// <summary>
        /// 从Redis服务器订阅一个或多个主题信息<br />
        /// Subscribe to one or more topics from the redis server
        /// </summary>
        /// <param name="topic">主题信息</param>
        /// <returns>订阅结果</returns>
        public OperateResult SubscribeMessage(string topic) => this.SubscribeMessage(new string[1]
        {
      topic
        });

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisSubscribe.SubscribeMessage(System.String)" />
        public OperateResult SubscribeMessage(string[] topics)
        {
            if (topics == null || topics.Length == 0)
                return OperateResult.CreateSuccessResult();
            if (this.CoreSocket == null)
            {
                OperateResult operateResult = this.ConnectServer();
                if (!operateResult.IsSuccess)
                    return operateResult;
            }
            OperateResult operateResult1 = this.Send(this.CoreSocket, RedisHelper.PackSubscribeCommand(topics));
            if (!operateResult1.IsSuccess)
                return operateResult1;
            this.AddSubTopics(topics);
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 取消订阅多个主题信息，取消之后，当前的订阅数据就不在接收到。<br />
        /// Unsubscribe from multiple topic information. After cancellation, the current subscription data will not be received.
        /// </summary>
        /// <param name="topics">主题信息</param>
        /// <returns>取消订阅结果</returns>
        public OperateResult UnSubscribeMessage(string[] topics)
        {
            if (this.CoreSocket == null)
            {
                OperateResult operateResult = this.ConnectServer();
                if (!operateResult.IsSuccess)
                    return operateResult;
            }
            OperateResult operateResult1 = this.Send(this.CoreSocket, RedisHelper.PackUnSubscribeCommand(topics));
            if (!operateResult1.IsSuccess)
                return operateResult1;
            this.RemoveSubTopics(topics);
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>取消已经订阅的主题信息</summary>
        /// <param name="topic">主题信息</param>
        /// <returns>取消订阅结果</returns>
        public OperateResult UnSubscribeMessage(string topic) => this.UnSubscribeMessage(new string[1]
        {
      topic
        });

        /// <summary>连接Redis的服务器，如果已经初始化了订阅的Topic信息，那么就会直接进行订阅操作。</summary>
        /// <returns>是否创建成功</returns>
        public OperateResult ConnectServer() => this.CreatePush();

        /// <summary>关闭消息推送的界面</summary>
        public void ConnectClose()
        {
            this.CoreSocket?.Close();
            lock (this.listLock)
                this.keyWords.Clear();
        }

        /// <summary>当接收到Redis订阅的信息的时候触发</summary>
        public event RedisSubscribe.RedisMessageReceiveDelegate OnRedisMessageReceived;

        /// <inheritdoc />
        public override string ToString() => string.Format("RedisSubscribe[{0}]", (object)this.endPoint);

        /// <summary>
        /// 当接收到Redis订阅的信息的时候触发<br />
        /// Triggered when receiving Redis subscription information
        /// </summary>
        /// <param name="topic">主题信息</param>
        /// <param name="message">数据信息</param>
        public delegate void RedisMessageReceiveDelegate(string topic, string message);
    }
}
