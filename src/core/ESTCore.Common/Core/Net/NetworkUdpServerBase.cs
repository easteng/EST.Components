// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Core.Net.NetworkUdpServerBase
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ESTCore.Common.Core.Net
{
    /// <summary>
    /// Udp服务器程序的基础类，提供了启动服务器的基本实现，方便后续的扩展操作。<br />
    /// The basic class of the udp server program provides the basic implementation of starting the server to facilitate subsequent expansion operations.
    /// </summary>
    public class NetworkUdpServerBase : NetworkXBase
    {
        private Thread threadReceive;

        /// <summary>
        /// 实例化一个默认的对象<br />
        /// Instantiate a default object
        /// </summary>
        public NetworkUdpServerBase()
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

        /// <summary>后台接收数据的线程</summary>
        protected virtual void ThreadReceiveCycle()
        {
            EndPoint remoteEP = (EndPoint)new IPEndPoint(IPAddress.Any, 0);
            while (this.IsStarted)
            {
                byte[] buffer = new byte[1024];
                int num = 0;
                try
                {
                    num = this.CoreSocket.ReceiveFrom(buffer, ref remoteEP);
                }
                catch (Exception ex)
                {
                    this.LogNet?.WriteException(nameof(ThreadReceiveCycle), ex);
                }
                Console.WriteLine(DateTime.Now.ToString() + " :ReceiveData");
            }
        }

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
            this.CoreSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.CoreSocket.Bind((EndPoint)new IPEndPoint(IPAddress.Any, port));
            this.threadReceive = new Thread(new ThreadStart(this.ThreadReceiveCycle))
            {
                IsBackground = true
            };
            this.threadReceive.Start();
            this.IsStarted = true;
            this.Port = port;
            this.LogNet?.WriteNewLine();
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
            this.CloseAction();
            this.CoreSocket?.Close();
            this.IsStarted = false;
            this.LogNet?.WriteInfo(this.ToString(), StringResources.Language.NetEngineClose);
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("NetworkUdpServerBase[{0}]", (object)this.Port);
    }
}
