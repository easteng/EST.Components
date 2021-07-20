// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Enthernet.NetComplexServer
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

namespace ESTCore.Common.Enthernet
{
    /// <summary>高性能的异步网络服务器类，适合搭建局域网聊天程序，消息推送程序</summary>
    /// <remarks>
    /// 详细的使用说明，请参照博客<a href="http://www.cnblogs.com/dathlin/p/8097897.html">http://www.cnblogs.com/dathlin/p/8097897.html</a>
    /// </remarks>
    /// <example>
    /// 此处贴上了Demo项目的服务器配置的示例代码
    /// <code lang="cs" source="TestProject\ComplexNetServer\FormServer.cs" region="NetComplexServer" title="NetComplexServer示例" />
    /// </example>
    public class NetComplexServer : NetworkServerBase
    {
        private int connectMaxClient = 10000;
        private readonly List<AppSession> appSessions = (List<AppSession>)null;
        private readonly object lockSessions = (object)null;

        /// <summary>实例化一个网络服务器类对象</summary>
        public NetComplexServer()
        {
            this.appSessions = new List<AppSession>();
            this.lockSessions = new object();
        }

        /// <summary>所支持的同时在线客户端的最大数量，默认为10000个</summary>
        public int ConnectMax
        {
            get => this.connectMaxClient;
            set => this.connectMaxClient = value;
        }

        /// <summary>获取或设置服务器是否记录客户端上下线信息，默认为true</summary>
        public bool IsSaveLogClientLineChange { get; set; } = true;

        /// <summary>所有在线客户端的数量</summary>
        public int ClientCount => this.appSessions.Count;

        /// <summary>初始化操作</summary>
        protected override void StartInitialization()
        {
            this.Thread_heart_check = new Thread(new ThreadStart(this.ThreadHeartCheck))
            {
                IsBackground = true,
                Priority = ThreadPriority.AboveNormal
            };
            this.Thread_heart_check.Start();
            base.StartInitialization();
        }

        /// <summary>关闭网络时的操作</summary>
        protected override void CloseAction()
        {
            this.Thread_heart_check?.Abort();
            this.ClientOffline = (Action<AppSession, string>)null;
            this.ClientOnline = (Action<AppSession>)null;
            this.AcceptString = (Action<AppSession, NetHandle, string>)null;
            this.AcceptByte = (Action<AppSession, NetHandle, byte[]>)null;
            lock (this.lockSessions)
                this.appSessions.ForEach((Action<AppSession>)(m => m.WorkSocket?.Close()));
            base.CloseAction();
        }

        private void TcpStateUpLine(AppSession session)
        {
            lock (this.lockSessions)
                this.appSessions.Add(session);
            Action<AppSession> clientOnline = this.ClientOnline;
            if (clientOnline != null)
                clientOnline(session);
            Action<int> clientsStatusChange = this.AllClientsStatusChange;
            if (clientsStatusChange != null)
                clientsStatusChange(this.ClientCount);
            if (!this.IsSaveLogClientLineChange)
                return;
            this.LogNet?.WriteDebug(this.ToString(), string.Format("[{0}] Name:{1} {2}", (object)session.IpEndPoint, (object)session?.LoginAlias, (object)StringResources.Language.NetClientOnline));
        }

        private void TcpStateDownLine(AppSession session, bool regular, bool logSave = true)
        {
            lock (this.lockSessions)
            {
                if (!this.appSessions.Remove(session))
                    return;
            }
            session.WorkSocket?.Close();
            string str = regular ? StringResources.Language.NetClientOffline : StringResources.Language.NetClientBreak;
            Action<AppSession, string> clientOffline = this.ClientOffline;
            if (clientOffline != null)
                clientOffline(session, str);
            Action<int> clientsStatusChange = this.AllClientsStatusChange;
            if (clientsStatusChange != null)
                clientsStatusChange(this.ClientCount);
            if (!(this.IsSaveLogClientLineChange & logSave))
                return;
            this.LogNet?.WriteInfo(this.ToString(), string.Format("[{0}] Name:{1} {2}", (object)session.IpEndPoint, (object)session?.LoginAlias, (object)str));
        }

        /// <summary>让客户端正常下线，调用本方法即可自由控制会话客户端强制下线操作。</summary>
        /// <param name="session">会话对象</param>
        public void AppSessionRemoteClose(AppSession session) => this.TcpStateDownLine(session, true);

        /// <summary>客户端的上下限状态变更时触发，仅作为在线客户端识别</summary>
        public event Action<int> AllClientsStatusChange;

        /// <summary>当客户端上线的时候，触发此事件</summary>
        public event Action<AppSession> ClientOnline;

        /// <summary>当客户端下线的时候，触发此事件</summary>
        public event Action<AppSession, string> ClientOffline;

        /// <summary>当接收到文本数据的时候,触发此事件</summary>
        public event Action<AppSession, NetHandle, string> AcceptString;

        /// <summary>当接收到字节数据的时候,触发此事件</summary>
        public event Action<AppSession, NetHandle, byte[]> AcceptByte;

        /// <summary>当接收到了新的请求的时候执行的操作</summary>
        /// <param name="socket">异步对象</param>
        /// <param name="endPoint">终结点</param>
        protected override void ThreadPoolLogin(Socket socket, IPEndPoint endPoint)
        {
            if (this.appSessions.Count > this.ConnectMax)
            {
                socket?.Close();
                this.LogNet?.WriteWarn(this.ToString(), StringResources.Language.NetClientFull);
            }
            else
            {
                OperateResult<int, string> contentFromSocket = this.ReceiveStringContentFromSocket(socket);
                if (!contentFromSocket.IsSuccess)
                    return;
                AppSession session = new AppSession()
                {
                    WorkSocket = socket,
                    LoginAlias = contentFromSocket.Content2
                };
                session.IpEndPoint = endPoint;
                session.IpAddress = endPoint == null ? string.Empty : endPoint.Address.ToString();
                if (contentFromSocket.Content1 == 1)
                    session.ClientType = "Windows";
                else if (contentFromSocket.Content1 == 2)
                    session.ClientType = "Android";
                try
                {
                    session.WorkSocket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, new AsyncCallback(this.ReceiveCallback), (object)session);
                    this.TcpStateUpLine(session);
                    Thread.Sleep(20);
                }
                catch (Exception ex)
                {
                    session.WorkSocket?.Close();
                    this.LogNet?.WriteException(this.ToString(), StringResources.Language.NetClientLoginFailed, ex);
                }
            }
        }

        private async void ReceiveCallback(IAsyncResult ar)
        {
            if (!(ar.AsyncState is AppSession appSession))
            {
                appSession = (AppSession)null;
            }
            else
            {
                try
                {
                    appSession.WorkSocket.EndReceive(ar);
                }
                catch
                {
                    this.TcpStateDownLine(appSession, false);
                    appSession = (AppSession)null;
                    return;
                }
                OperateResult<int, int, byte[]> read = await this.ReceiveEstMessageAsync(appSession.WorkSocket);
                if (!read.IsSuccess)
                {
                    this.TcpStateDownLine(appSession, false);
                    appSession = (AppSession)null;
                }
                else
                {
                    try
                    {
                        appSession.WorkSocket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, new AsyncCallback(this.ReceiveCallback), (object)appSession);
                    }
                    catch
                    {
                        this.TcpStateDownLine(appSession, false);
                        appSession = (AppSession)null;
                        return;
                    }
                    int protocol = read.Content1;
                    int customer = read.Content2;
                    byte[] content = read.Content3;
                    switch (protocol)
                    {
                        case 1:
                            BitConverter.GetBytes(DateTime.Now.Ticks).CopyTo((Array)content, 8);
                            this.LogNet?.WriteDebug(this.ToString(), string.Format("Heart Check From {0}", (object)appSession.IpEndPoint));
                            if (this.Send(appSession.WorkSocket, EstProtocol.CommandBytes(1, customer, this.Token, content)).IsSuccess)
                            {
                                appSession.HeartTime = DateTime.Now;
                                break;
                            }
                            break;
                        case 2:
                            this.TcpStateDownLine(appSession, true);
                            appSession = (AppSession)null;
                            return;
                        case 1001:
                            string str = Encoding.Unicode.GetString(content);
                            Action<AppSession, NetHandle, string> acceptString = this.AcceptString;
                            if (acceptString != null)
                                acceptString(appSession, (NetHandle)customer, str);
                            str = (string)null;
                            break;
                        case 1002:
                            Action<AppSession, NetHandle, byte[]> acceptByte = this.AcceptByte;
                            if (acceptByte != null)
                            {
                                acceptByte(appSession, (NetHandle)customer, content);
                                break;
                            }
                            break;
                    }
                    read = (OperateResult<int, int, byte[]>)null;
                    content = (byte[])null;
                    appSession = (AppSession)null;
                }
            }
        }

        /// <summary>服务器端用于数据发送文本的方法</summary>
        /// <param name="session">数据发送对象</param>
        /// <param name="customer">用户自定义的数据对象，如不需要，赋值为0</param>
        /// <param name="str">发送的文本</param>
        public void Send(AppSession session, NetHandle customer, string str) => this.Send(session.WorkSocket, EstProtocol.CommandBytes((int)customer, this.Token, str));

        /// <summary>服务器端用于发送字节的方法</summary>
        /// <param name="session">数据发送对象</param>
        /// <param name="customer">用户自定义的数据对象，如不需要，赋值为0</param>
        /// <param name="bytes">实际发送的数据</param>
        public void Send(AppSession session, NetHandle customer, byte[] bytes) => this.Send(session.WorkSocket, EstProtocol.CommandBytes((int)customer, this.Token, bytes));

        /// <summary>服务端用于发送所有数据到所有的客户端</summary>
        /// <param name="customer">用户自定义的命令头</param>
        /// <param name="str">需要传送的实际的数据</param>
        public void SendAllClients(NetHandle customer, string str)
        {
            lock (this.lockSessions)
            {
                for (int index = 0; index < this.appSessions.Count; ++index)
                    this.Send(this.appSessions[index], customer, str);
            }
        }

        /// <summary>服务端用于发送所有数据到所有的客户端</summary>
        /// <param name="customer">用户自定义的命令头</param>
        /// <param name="data">需要群发客户端的字节数据</param>
        public void SendAllClients(NetHandle customer, byte[] data)
        {
            lock (this.lockSessions)
            {
                for (int index = 0; index < this.appSessions.Count; ++index)
                    this.Send(this.appSessions[index], customer, data);
            }
        }

        /// <summary>根据客户端设置的别名进行发送消息</summary>
        /// <param name="Alias">客户端上线的别名</param>
        /// <param name="customer">用户自定义的命令头</param>
        /// <param name="str">需要传送的实际的数据</param>
        public void SendClientByAlias(string Alias, NetHandle customer, string str)
        {
            lock (this.lockSessions)
            {
                for (int index = 0; index < this.appSessions.Count; ++index)
                {
                    if (this.appSessions[index].LoginAlias == Alias)
                        this.Send(this.appSessions[index], customer, str);
                }
            }
        }

        /// <summary>根据客户端设置的别名进行发送消息</summary>
        /// <param name="Alias">客户端上线的别名</param>
        /// <param name="customer">用户自定义的命令头</param>
        /// <param name="data">需要传送的实际的数据</param>
        public void SendClientByAlias(string Alias, NetHandle customer, byte[] data)
        {
            lock (this.lockSessions)
            {
                for (int index = 0; index < this.appSessions.Count; ++index)
                {
                    if (this.appSessions[index].LoginAlias == Alias)
                        this.Send(this.appSessions[index], customer, data);
                }
            }
        }

        private Thread Thread_heart_check { get; set; } = (Thread)null;

        private void ThreadHeartCheck()
        {
            do
            {
                Thread.Sleep(2000);
                try
                {
                    AppSession[] appSessionArray = (AppSession[])null;
                    lock (this.lockSessions)
                        appSessionArray = this.appSessions.ToArray();
                    for (int index = appSessionArray.Length - 1; index >= 0; --index)
                    {
                        if (appSessionArray[index] != null && (DateTime.Now - appSessionArray[index].HeartTime).TotalSeconds > 30.0)
                        {
                            this.LogNet?.WriteWarn(this.ToString(), StringResources.Language.NetHeartCheckTimeout + appSessionArray[index].IpAddress.ToString());
                            this.TcpStateDownLine(appSessionArray[index], false, false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.LogNet?.WriteException(this.ToString(), StringResources.Language.NetHeartCheckFailed, ex);
                }
            }
            while (this.IsStarted);
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("NetComplexServer[{0}]", (object)this.Port);
    }
}
