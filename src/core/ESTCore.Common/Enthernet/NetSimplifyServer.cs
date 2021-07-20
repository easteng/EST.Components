// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Enthernet.NetSimplifyServer
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Core.Net;
using ESTCore.Common.LogNet;

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ESTCore.Common.Enthernet
{
    /// <summary>
    /// 消息处理服务器，主要用来实现接收客户端信息并进行消息反馈的操作，适用于客户端进行远程的调用，要求服务器反馈数据。<br />
    /// The message processing server is mainly used to implement the operation of receiving client information and performing message feedback. It is applicable to remote calls made by clients and requires the server to feedback data.
    /// </summary>
    /// <remarks>
    /// 详细的使用说明，请参照博客<a href="http://www.cnblogs.com/dathlin/p/7697782.html">http://www.cnblogs.com/dathlin/p/7697782.html</a>
    /// </remarks>
    /// <example>
    /// 此处贴上了Demo项目的服务器配置的示例代码
    /// <code lang="cs" source="TestProject\SimplifyNetTest\FormServer.cs" region="Simplify Net" title="NetSimplifyServer示例" />
    /// </example>
    public class NetSimplifyServer : NetworkAuthenticationServerBase
    {
        private int clientCount = 0;

        /// <summary>接收字符串信息的事件</summary>
        public event Action<AppSession, NetHandle, string> ReceiveStringEvent;

        /// <summary>接收字符串数组信息的事件</summary>
        public event Action<AppSession, NetHandle, string[]> ReceiveStringArrayEvent;

        /// <summary>接收字节信息的事件</summary>
        public event Action<AppSession, NetHandle, byte[]> ReceivedBytesEvent;

        /// <summary>向指定的通信对象发送字符串数据</summary>
        /// <param name="session">通信对象</param>
        /// <param name="customer">用户的指令头</param>
        /// <param name="str">实际发送的字符串数据</param>
        public void SendMessage(AppSession session, int customer, string str) => this.Send(session.WorkSocket, EstProtocol.CommandBytes(customer, this.Token, str));

        /// <summary>向指定的通信对象发送字符串数组</summary>
        /// <param name="session">通信对象</param>
        /// <param name="customer">用户的指令头</param>
        /// <param name="str">实际发送的字符串数组</param>
        public void SendMessage(AppSession session, int customer, string[] str) => this.Send(session.WorkSocket, EstProtocol.CommandBytes(customer, this.Token, str));

        /// <summary>向指定的通信对象发送字节数据</summary>
        /// <param name="session">连接对象</param>
        /// <param name="customer">用户的指令头</param>
        /// <param name="bytes">实际的数据</param>
        public void SendMessage(AppSession session, int customer, byte[] bytes) => this.Send(session.WorkSocket, EstProtocol.CommandBytes(customer, this.Token, bytes));

        /// <summary>关闭网络的操作</summary>
        protected override void CloseAction()
        {
            this.ReceivedBytesEvent = (Action<AppSession, NetHandle, byte[]>)null;
            this.ReceiveStringEvent = (Action<AppSession, NetHandle, string>)null;
            base.CloseAction();
        }

        /// <summary>当接收到了新的请求的时候执行的操作</summary>
        /// <param name="socket">异步对象</param>
        /// <param name="endPoint">终结点</param>
        protected override void ThreadPoolLogin(Socket socket, IPEndPoint endPoint)
        {
            AppSession appSession = new AppSession();
            appSession.WorkSocket = socket;
            try
            {
                appSession.IpEndPoint = endPoint;
                appSession.IpAddress = appSession.IpEndPoint.Address.ToString();
            }
            catch (Exception ex)
            {
                this.LogNet?.WriteException(this.ToString(), StringResources.Language.GetClientIpAddressFailed, ex);
            }
            this.LogNet?.WriteDebug(this.ToString(), string.Format(StringResources.Language.ClientOnlineInfo, (object)appSession.IpEndPoint));
            try
            {
                appSession.WorkSocket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, new AsyncCallback(this.ReceiveCallback), (object)appSession);
                Interlocked.Increment(ref this.clientCount);
            }
            catch (Exception ex)
            {
                appSession.WorkSocket?.Close();
                this.LogNet?.WriteException(this.ToString(), StringResources.Language.NetClientLoginFailed, ex);
            }
        }

        private async void ReceiveCallback(IAsyncResult ar)
        {
            if (!(ar.AsyncState is AppSession appSession))
                appSession = (AppSession)null;
            else if (!appSession.WorkSocket.EndReceiveResult(ar).IsSuccess)
            {
                this.AppSessionRemoteClose(appSession);
                appSession = (AppSession)null;
            }
            else
            {
                OperateResult<int, int, byte[]> read = await this.ReceiveEstMessageAsync(appSession.WorkSocket);
                if (!read.IsSuccess)
                {
                    this.AppSessionRemoteClose(appSession);
                    appSession = (AppSession)null;
                }
                else
                {
                    int protocol = read.Content1;
                    int customer = read.Content2;
                    byte[] content = read.Content3;
                    switch (protocol)
                    {
                        case 1:
                            appSession.HeartTime = DateTime.Now;
                            this.SendMessage(appSession, customer, content);
                            ILogNet logNet = this.LogNet;
                            if (logNet != null)
                            {
                                logNet.WriteDebug(this.ToString(), string.Format("Heart Check From {0}", (object)appSession.IpEndPoint));
                                break;
                            }
                            break;
                        case 2:
                            this.AppSessionRemoteClose(appSession);
                            appSession = (AppSession)null;
                            return;
                        case 1001:
                            Action<AppSession, NetHandle, string> receiveStringEvent = this.ReceiveStringEvent;
                            if (receiveStringEvent != null)
                            {
                                receiveStringEvent(appSession, (NetHandle)customer, Encoding.Unicode.GetString(content));
                                break;
                            }
                            break;
                        case 1002:
                            Action<AppSession, NetHandle, byte[]> receivedBytesEvent = this.ReceivedBytesEvent;
                            if (receivedBytesEvent != null)
                            {
                                receivedBytesEvent(appSession, (NetHandle)customer, content);
                                break;
                            }
                            break;
                        case 1005:
                            Action<AppSession, NetHandle, string[]> stringArrayEvent = this.ReceiveStringArrayEvent;
                            if (stringArrayEvent != null)
                            {
                                stringArrayEvent(appSession, (NetHandle)customer, EstProtocol.UnPackStringArrayFromByte(content));
                                break;
                            }
                            break;
                        default:
                            this.AppSessionRemoteClose(appSession);
                            appSession = (AppSession)null;
                            return;
                    }
                    if (!appSession.WorkSocket.BeginReceiveResult(new AsyncCallback(this.ReceiveCallback), (object)appSession).IsSuccess)
                        this.AppSessionRemoteClose(appSession);
                    read = (OperateResult<int, int, byte[]>)null;
                    content = (byte[])null;
                    appSession = (AppSession)null;
                }
            }
        }

        /// <summary>让客户端正常下线，调用本方法即可自由控制会话客户端强制下线操作。</summary>
        /// <param name="session">会话对象</param>
        public void AppSessionRemoteClose(AppSession session)
        {
            session.WorkSocket?.Close();
            Interlocked.Decrement(ref this.clientCount);
            this.LogNet?.WriteDebug(this.ToString(), string.Format(StringResources.Language.ClientOfflineInfo, (object)session.IpEndPoint));
        }

        /// <summary>当前在线的客户端数量</summary>
        public int ClientCount => this.clientCount;

        /// <inheritdoc />
        public override string ToString() => string.Format("NetSimplifyServer[{0}]", (object)this.Port);
    }
}
