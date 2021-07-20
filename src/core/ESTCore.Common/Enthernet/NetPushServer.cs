// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Enthernet.NetPushServer
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Core.Net;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ESTCore.Common.Enthernet
{
    /// <summary>发布订阅服务器的类，支持按照关键字进行数据信息的订阅</summary>
    /// <remarks>
    /// 详细的使用说明，请参照博客<a href="http://www.cnblogs.com/dathlin/p/8992315.html">http://www.cnblogs.com/dathlin/p/8992315.html</a>
    /// </remarks>
    /// <example>
    /// 此处贴上了Demo项目的服务器配置的示例代码
    /// <code lang="cs" source="TestProject\PushNetServer\FormServer.cs" region="NetPushServer" title="NetPushServer示例" />
    /// </example>
    public class NetPushServer : NetworkServerBase
    {
        private Dictionary<string, string> dictSendHistory;
        private Dictionary<string, PushGroupClient> dictPushClients;
        private readonly object dicHybirdLock;
        private readonly object dicSendCacheLock;
        private Action<AppSession, string> sendAction;
        private int onlineCount = 0;
        private List<NetPushClient> pushClients;
        private object pushClientsLock;
        private bool isPushCacheAfterConnect = true;

        /// <summary>实例化一个对象</summary>
        public NetPushServer()
        {
            this.dictPushClients = new Dictionary<string, PushGroupClient>();
            this.dictSendHistory = new Dictionary<string, string>();
            this.dicHybirdLock = new object();
            this.dicSendCacheLock = new object();
            this.sendAction = new Action<AppSession, string>(this.SendString);
            this.pushClientsLock = new object();
            this.pushClients = new List<NetPushClient>();
        }

        /// <inheritdoc />
        protected override void ThreadPoolLogin(Socket socket, IPEndPoint endPoint)
        {
            OperateResult<int, string> contentFromSocket = this.ReceiveStringContentFromSocket(socket);
            if (!contentFromSocket.IsSuccess || !this.SendStringAndCheckReceive(socket, 0, "").IsSuccess)
                return;
            AppSession appSession = new AppSession()
            {
                KeyGroup = contentFromSocket.Content2,
                WorkSocket = socket
            };
            appSession.IpEndPoint = endPoint;
            appSession.IpAddress = endPoint.Address.ToString();
            try
            {
                socket.BeginReceive(appSession.BytesHead, 0, appSession.BytesHead.Length, SocketFlags.None, new AsyncCallback(this.ReceiveCallback), (object)appSession);
            }
            catch (Exception ex)
            {
                this.LogNet?.WriteException(this.ToString(), StringResources.Language.SocketReceiveException, ex);
                return;
            }
            this.LogNet?.WriteDebug(this.ToString(), string.Format(StringResources.Language.ClientOnlineInfo, (object)appSession.IpEndPoint));
            PushGroupClient pushGroupClient = this.GetPushGroupClient(contentFromSocket.Content2);
            if (pushGroupClient == null)
                return;
            Interlocked.Increment(ref this.onlineCount);
            pushGroupClient.AddPushClient(appSession);
            lock (this.dicSendCacheLock)
            {
                if (this.dictSendHistory.ContainsKey(contentFromSocket.Content2) && this.isPushCacheAfterConnect)
                    this.SendString(appSession, this.dictSendHistory[contentFromSocket.Content2]);
            }
        }

        /// <inheritdoc />
        public override void ServerClose() => base.ServerClose();

        /// <summary>主动推送数据内容</summary>
        /// <param name="key">关键字</param>
        /// <param name="content">数据内容</param>
        public void PushString(string key, string content)
        {
            lock (this.dicSendCacheLock)
            {
                if (this.dictSendHistory.ContainsKey(key))
                    this.dictSendHistory[key] = content;
                else
                    this.dictSendHistory.Add(key, content);
            }
            this.AddPushKey(key);
            this.GetPushGroupClient(key)?.PushString(content, this.sendAction);
        }

        /// <summary>移除关键字信息，通常应用于一些特殊临时用途的关键字</summary>
        /// <param name="key">关键字</param>
        public void RemoveKey(string key)
        {
            lock (this.dicHybirdLock)
            {
                if (!this.dictPushClients.ContainsKey(key))
                    return;
                int num = this.dictPushClients[key].RemoveAllClient();
                for (int index = 0; index < num; ++index)
                    Interlocked.Decrement(ref this.onlineCount);
                this.dictPushClients.Remove(key);
            }
        }

        /// <summary>创建一个远程服务器的数据推送操作，以便推送给子客户端</summary>
        /// <param name="ipAddress">远程的IP地址</param>
        /// <param name="port">远程的端口号</param>
        /// <param name="key">订阅的关键字</param>
        public OperateResult CreatePushRemote(string ipAddress, int port, string key)
        {
            OperateResult operateResult;
            lock (this.pushClientsLock)
            {
                if (this.pushClients.Find((Predicate<NetPushClient>)(m => m.KeyWord == key)) == null)
                {
                    NetPushClient netPushClient = new NetPushClient(ipAddress, port, key);
                    operateResult = netPushClient.CreatePush(new Action<NetPushClient, string>(this.GetPushFromServer));
                    if (operateResult.IsSuccess)
                        this.pushClients.Add(netPushClient);
                }
                else
                    operateResult = new OperateResult(StringResources.Language.KeyIsExistAlready);
            }
            return operateResult;
        }

        /// <summary>在线客户端的数量</summary>
        public int OnlineCount => this.onlineCount;

        /// <summary>在客户端上线之后，是否推送缓存的数据，默认设置为true</summary>
        public bool PushCacheAfterConnect
        {
            get => this.isPushCacheAfterConnect;
            set => this.isPushCacheAfterConnect = value;
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            if (!(ar.AsyncState is AppSession asyncState))
                return;
            try
            {
                if (asyncState.WorkSocket.EndReceive(ar) <= 4)
                {
                    this.LogNet?.WriteDebug(this.ToString(), string.Format(StringResources.Language.ClientOfflineInfo, (object)asyncState.IpEndPoint));
                    this.RemoveGroupOnline(asyncState.KeyGroup, asyncState.ClientUniqueID);
                }
                else
                    asyncState.HeartTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains(StringResources.Language.SocketRemoteCloseException))
                {
                    this.LogNet?.WriteDebug(this.ToString(), string.Format(StringResources.Language.ClientOfflineInfo, (object)asyncState.IpEndPoint));
                    this.RemoveGroupOnline(asyncState.KeyGroup, asyncState.ClientUniqueID);
                }
                else
                {
                    this.LogNet?.WriteException(this.ToString(), string.Format(StringResources.Language.ClientOfflineInfo, (object)asyncState.IpEndPoint), ex);
                    this.RemoveGroupOnline(asyncState.KeyGroup, asyncState.ClientUniqueID);
                }
            }
        }

        private void AddPushKey(string key)
        {
            lock (this.dicHybirdLock)
            {
                if (this.dictPushClients.ContainsKey(key))
                    return;
                this.dictPushClients.Add(key, new PushGroupClient());
            }
        }

        private PushGroupClient GetPushGroupClient(string key)
        {
            PushGroupClient pushGroupClient = (PushGroupClient)null;
            lock (this.dicHybirdLock)
            {
                if (this.dictPushClients.ContainsKey(key))
                {
                    pushGroupClient = this.dictPushClients[key];
                }
                else
                {
                    pushGroupClient = new PushGroupClient();
                    this.dictPushClients.Add(key, pushGroupClient);
                }
            }
            return pushGroupClient;
        }

        /// <summary>移除客户端的数据信息</summary>
        /// <param name="key">指定的客户端</param>
        /// <param name="clientID">指定的客户端唯一的id信息</param>
        private void RemoveGroupOnline(string key, string clientID)
        {
            PushGroupClient pushGroupClient = this.GetPushGroupClient(key);
            if (pushGroupClient == null || !pushGroupClient.RemovePushClient(clientID))
                return;
            Interlocked.Decrement(ref this.onlineCount);
        }

        private void SendString(AppSession appSession, string content)
        {
            if (this.Send(appSession.WorkSocket, EstProtocol.CommandBytes(0, this.Token, content)).IsSuccess)
                return;
            this.RemoveGroupOnline(appSession.KeyGroup, appSession.ClientUniqueID);
        }

        private void GetPushFromServer(NetPushClient pushClient, string data) => this.PushString(pushClient.KeyWord, data);

        /// <inheritdoc />
        public override string ToString() => string.Format("NetPushServer[{0}]", (object)this.Port);
    }
}
