// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Enthernet.NetComplexClient
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Core.Net;

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ESTCore.Common.Enthernet
{
    /// <summary>一个基于异步高性能的客户端网络类，支持主动接收服务器的消息</summary>
    /// <remarks>
    /// 详细的使用说明，请参照博客<a href="http://www.cnblogs.com/dathlin/p/7697782.html">http://www.cnblogs.com/dathlin/p/7697782.html</a>
    /// </remarks>
    /// <example>
    /// 此处贴上了Demo项目的服务器配置的示例代码
    /// <code lang="cs" source="TestProject\ESTCore.CommonDemo\Est\FormComplexNet.cs" region="NetComplexClient" title="NetComplexClient示例" />
    /// </example>
    public class NetComplexClient : NetworkXBase
    {
        private AppSession session;
        private int isConnecting = 0;
        private bool closed = false;
        private Thread thread_heart_check = (Thread)null;

        /// <summary>实例化一个对象</summary>
        public NetComplexClient()
        {
            this.session = new AppSession();
            this.ServerTime = DateTime.Now;
            this.EndPointServer = new IPEndPoint(IPAddress.Any, 0);
        }

        /// <summary>客户端系统是否启动</summary>
        public bool IsClientStart { get; set; }

        /// <summary>重连接失败的次数</summary>
        public int ConnectFailedCount { get; private set; }

        /// <summary>客户端登录的标识名称，可以为ID号，也可以为登录名</summary>
        public string ClientAlias { get; set; } = string.Empty;

        /// <summary>远程服务器的IP地址和端口</summary>
        public IPEndPoint EndPointServer { get; set; }

        /// <summary>服务器的时间，自动实现和服务器同步</summary>
        public DateTime ServerTime { get; private set; }

        /// <summary>系统与服务器的延时时间，单位毫秒</summary>
        public int DelayTime { get; private set; }

        /// <summary>客户端启动成功的事件，重连成功也将触发此事件</summary>
        public event Action LoginSuccess;

        /// <summary>连接失败时触发的事件</summary>
        public event Action<int> LoginFailed;

        /// <summary>服务器的异常，启动，等等一般消息产生的时候，出发此事件</summary>
        public event Action<string> MessageAlerts;

        /// <summary>在客户端断开后并在重连服务器之前触发，用于清理系统资源</summary>
        public event Action BeforReConnected;

        /// <summary>当接收到文本数据的时候,触发此事件</summary>
        public event Action<AppSession, NetHandle, string> AcceptString;

        /// <summary>当接收到字节数据的时候,触发此事件</summary>
        public event Action<AppSession, NetHandle, byte[]> AcceptByte;

        /// <summary>关闭该客户端引擎</summary>
        public void ClientClose()
        {
            this.closed = true;
            if (this.IsClientStart)
                this.Send(this.session.WorkSocket, EstProtocol.CommandBytes(2, 0, this.Token, (byte[])null));
            this.IsClientStart = false;
            this.thread_heart_check = (Thread)null;
            this.LoginSuccess = (Action)null;
            this.LoginFailed = (Action<int>)null;
            this.MessageAlerts = (Action<string>)null;
            this.AcceptByte = (Action<AppSession, NetHandle, byte[]>)null;
            this.AcceptString = (Action<AppSession, NetHandle, string>)null;
            Thread.Sleep(20);
            this.session.WorkSocket?.Close();
            this.LogNet?.WriteDebug(this.ToString(), "Client Close.");
        }

        /// <summary>启动客户端引擎，连接服务器系统</summary>
        public void ClientStart()
        {
            if ((uint)Interlocked.CompareExchange(ref this.isConnecting, 1, 0) > 0U)
                return;
            new Thread(new ThreadStart(this.ThreadLogin))
            {
                IsBackground = true
            }.Start();
            if (this.thread_heart_check != null)
                return;
            this.thread_heart_check = new Thread(new ThreadStart(this.ThreadHeartCheck))
            {
                Priority = ThreadPriority.AboveNormal,
                IsBackground = true
            };
            this.thread_heart_check.Start();
        }

        /// <summary>连接服务器之前的消息提示，如果是重连的话，就提示10秒等待信息</summary>
        private void AwaitToConnect()
        {
            if (this.ConnectFailedCount == 0)
            {
                Action<string> messageAlerts = this.MessageAlerts;
                if (messageAlerts == null)
                    return;
                messageAlerts(StringResources.Language.ConnectingServer);
            }
            else
            {
                int num = 10;
                while (num > 0)
                {
                    if (this.closed)
                        return;
                    --num;
                    Action<string> messageAlerts = this.MessageAlerts;
                    if (messageAlerts != null)
                        messageAlerts(string.Format(StringResources.Language.ConnectFailedAndWait, (object)num));
                    Thread.Sleep(1000);
                }
                Action<string> messageAlerts1 = this.MessageAlerts;
                if (messageAlerts1 != null)
                    messageAlerts1(string.Format(StringResources.Language.AttemptConnectServer, (object)this.ConnectFailedCount));
            }
        }

        private void ConnectFailed()
        {
            ++this.ConnectFailedCount;
            Interlocked.Exchange(ref this.isConnecting, 0);
            Action<int> loginFailed = this.LoginFailed;
            if (loginFailed != null)
                loginFailed(this.ConnectFailedCount);
            this.LogNet?.WriteDebug(this.ToString(), "Connected Failed, Times: " + this.ConnectFailedCount.ToString());
        }

        private OperateResult<Socket> ConnectServer()
        {
            OperateResult<Socket> socketAndConnect = this.CreateSocketAndConnect(this.EndPointServer, 10000);
            if (!socketAndConnect.IsSuccess)
                return socketAndConnect;
            OperateResult result = this.SendStringAndCheckReceive(socketAndConnect.Content, 1, this.ClientAlias);
            if (!result.IsSuccess)
                return OperateResult.CreateFailedResult<Socket>(result);
            Action<string> messageAlerts = this.MessageAlerts;
            if (messageAlerts != null)
                messageAlerts(StringResources.Language.ConnectServerSuccess);
            return socketAndConnect;
        }

        private void LoginSuccessMethod(Socket socket)
        {
            this.ConnectFailedCount = 0;
            try
            {
                this.session.IpEndPoint = (IPEndPoint)socket.RemoteEndPoint;
                this.session.LoginAlias = this.ClientAlias;
                this.session.WorkSocket = socket;
                this.session.HeartTime = DateTime.Now;
                this.IsClientStart = true;
                this.session.WorkSocket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, new AsyncCallback(this.ReceiveCallback), (object)this.session);
            }
            catch
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(this.ReconnectServer), (object)null);
            }
        }

        private void ThreadLogin()
        {
            this.AwaitToConnect();
            OperateResult<Socket> operateResult = this.ConnectServer();
            if (!operateResult.IsSuccess)
            {
                this.ConnectFailed();
                ThreadPool.QueueUserWorkItem(new WaitCallback(this.ReconnectServer), (object)null);
            }
            else
            {
                this.LoginSuccessMethod(operateResult.Content);
                Action loginSuccess = this.LoginSuccess;
                if (loginSuccess != null)
                    loginSuccess();
                Interlocked.Exchange(ref this.isConnecting, 0);
                Thread.Sleep(200);
            }
        }

        private void ReconnectServer(object obj = null)
        {
            if (this.isConnecting == 1 || this.closed)
                return;
            Action beforReConnected = this.BeforReConnected;
            if (beforReConnected != null)
                beforReConnected();
            this.session?.WorkSocket?.Close();
            this.ClientStart();
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
                    ThreadPool.QueueUserWorkItem(new WaitCallback(this.ReconnectServer), (object)null);
                    appSession = (AppSession)null;
                    return;
                }
                OperateResult<int, int, byte[]> read = await this.ReceiveEstMessageAsync(appSession.WorkSocket);
                if (!read.IsSuccess)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(this.ReconnectServer), (object)null);
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
                        ThreadPool.QueueUserWorkItem(new WaitCallback(this.ReconnectServer), (object)null);
                        appSession = (AppSession)null;
                        return;
                    }
                    int protocol = read.Content1;
                    int customer = read.Content2;
                    byte[] content = read.Content3;
                    switch (protocol)
                    {
                        case 1:
                            DateTime dt = new DateTime(BitConverter.ToInt64(content, 0));
                            this.ServerTime = new DateTime(BitConverter.ToInt64(content, 8));
                            this.DelayTime = (int)(DateTime.Now - dt).TotalMilliseconds;
                            this.session.HeartTime = DateTime.Now;
                            break;
                        case 1001:
                            string str = Encoding.Unicode.GetString(content);
                            Action<AppSession, NetHandle, string> acceptString = this.AcceptString;
                            if (acceptString != null)
                                acceptString(this.session, (NetHandle)customer, str);
                            str = (string)null;
                            break;
                        case 1002:
                            Action<AppSession, NetHandle, byte[]> acceptByte = this.AcceptByte;
                            if (acceptByte != null)
                            {
                                acceptByte(this.session, (NetHandle)customer, content);
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
        /// <param name="customer">用户自定义的命令头</param>
        /// <param name="str">发送的文本</param>
        public void Send(NetHandle customer, string str)
        {
            if (!this.IsClientStart)
                return;
            this.Send(this.session.WorkSocket, EstProtocol.CommandBytes((int)customer, this.Token, str));
        }

        /// <summary>服务器端用于发送字节的方法</summary>
        /// <param name="customer">用户自定义的命令头</param>
        /// <param name="bytes">实际发送的数据</param>
        public void Send(NetHandle customer, byte[] bytes)
        {
            if (!this.IsClientStart)
                return;
            this.Send(this.session.WorkSocket, EstProtocol.CommandBytes((int)customer, this.Token, bytes));
        }

        /// <summary>心跳线程的方法</summary>
        private void ThreadHeartCheck()
        {
            Thread.Sleep(2000);
            while (true)
            {
                Thread.Sleep(10000);
                if (!this.closed)
                {
                    byte[] data = new byte[16];
                    BitConverter.GetBytes(DateTime.Now.Ticks).CopyTo((Array)data, 0);
                    this.Send(this.session.WorkSocket, EstProtocol.CommandBytes(1, 0, this.Token, data));
                    double totalSeconds = (DateTime.Now - this.session.HeartTime).TotalSeconds;
                    if (totalSeconds > 30.0)
                    {
                        if (this.isConnecting == 0)
                        {
                            this.LogNet?.WriteDebug(this.ToString(), string.Format("Heart Check Failed int {0} Seconds.", (object)totalSeconds));
                            this.ReconnectServer();
                        }
                        if (!this.closed)
                            Thread.Sleep(1000);
                    }
                }
                else
                    break;
            }
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("NetComplexClient[{0}]", (object)this.EndPointServer);
    }
}
