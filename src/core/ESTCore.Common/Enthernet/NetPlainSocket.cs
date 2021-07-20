// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Enthernet.NetPlainSocket
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Core.Net;

using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ESTCore.Common.Enthernet
{
    /// <summary>一个基于明文的socket中心</summary>
    public class NetPlainSocket : NetworkXBase
    {
        private Encoding encoding;
        private object connectLock = new object();
        private string ipAddress = "127.0.0.1";
        private int port = 10000;
        private int bufferLength = 2048;
        private byte[] buffer = (byte[])null;

        /// <summary>实例化一个默认的对象</summary>
        public NetPlainSocket()
        {
            this.buffer = new byte[this.bufferLength];
            this.encoding = Encoding.UTF8;
        }

        /// <summary>使用指定的ip地址和端口号来实例化这个对象</summary>
        /// <param name="ipAddress">Ip地址</param>
        /// <param name="port">端口号</param>
        public NetPlainSocket(string ipAddress, int port)
        {
            this.buffer = new byte[this.bufferLength];
            this.encoding = Encoding.UTF8;
            this.ipAddress = ipAddress;
            this.port = port;
        }

        /// <summary>连接服务器</summary>
        /// <returns>返回是否连接成功</returns>
        public OperateResult ConnectServer()
        {
            this.CoreSocket?.Close();
            OperateResult<Socket> socketAndConnect = this.CreateSocketAndConnect(this.ipAddress, this.port, 5000);
            if (!socketAndConnect.IsSuccess)
                return (OperateResult)socketAndConnect;
            try
            {
                this.CoreSocket = socketAndConnect.Content;
                this.CoreSocket.BeginReceive(this.buffer, 0, this.bufferLength, SocketFlags.None, new AsyncCallback(this.ReceiveCallBack), (object)this.CoreSocket);
                return OperateResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                return new OperateResult(ex.Message);
            }
        }

        /// <summary>关闭当前的连接对象</summary>
        /// <returns>错误信息</returns>
        public OperateResult ConnectClose()
        {
            try
            {
                this.CoreSocket?.Close();
                return OperateResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                return new OperateResult(ex.Message);
            }
        }

        /// <summary>发送字符串到网络上去</summary>
        /// <param name="text">文本信息</param>
        /// <returns>发送是否成功</returns>
        public OperateResult SendString(string text) => string.IsNullOrEmpty(text) ? OperateResult.CreateSuccessResult() : this.Send(this.CoreSocket, this.encoding.GetBytes(text));

        private void ReceiveCallBack(IAsyncResult ar)
        {
            if (!(ar.AsyncState is Socket asyncState))
                return;
            byte[] bytes = (byte[])null;
            try
            {
                int length = asyncState.EndReceive(ar);
                asyncState.BeginReceive(this.buffer, 0, this.bufferLength, SocketFlags.None, new AsyncCallback(this.ReceiveCallBack), (object)asyncState);
                if (length == 0)
                {
                    this.CoreSocket?.Close();
                    return;
                }
                bytes = new byte[length];
                Array.Copy((Array)this.buffer, 0, (Array)bytes, 0, length);
            }
            catch (ObjectDisposedException ex)
            {
            }
            catch (Exception ex)
            {
                this.LogNet?.WriteWarn(StringResources.Language.SocketContentReceiveException + ":" + ex.Message);
                ThreadPool.QueueUserWorkItem(new WaitCallback(this.ReConnectServer), (object)null);
            }
            if (bytes != null)
            {
                Action<string> receivedString = this.ReceivedString;
                if (receivedString != null)
                    receivedString(this.encoding.GetString(bytes));
            }
        }

        /// <summary>是否是处于重连的状态</summary>
        /// <param name="obj">无用的对象</param>
        private void ReConnectServer(object obj)
        {
            this.LogNet?.WriteWarn(StringResources.Language.ReConnectServerAfterTenSeconds);
            for (int index = 0; index < 10; ++index)
            {
                Thread.Sleep(1000);
                this.LogNet?.WriteWarn(string.Format("Wait for connecting server after {0} seconds", (object)(9 - index)));
            }
            OperateResult<Socket> socketAndConnect = this.CreateSocketAndConnect(this.ipAddress, this.port, 5000);
            if (!socketAndConnect.IsSuccess)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(this.ReConnectServer), obj);
            }
            else
            {
                lock (this.connectLock)
                {
                    try
                    {
                        this.CoreSocket?.Close();
                        this.CoreSocket = socketAndConnect.Content;
                        this.CoreSocket.BeginReceive(this.buffer, 0, this.bufferLength, SocketFlags.None, new AsyncCallback(this.ReceiveCallBack), (object)this.CoreSocket);
                        this.LogNet?.WriteWarn(StringResources.Language.ReConnectServerSuccess);
                    }
                    catch (Exception ex)
                    {
                        this.LogNet?.WriteWarn(StringResources.Language.RemoteClosedConnection + ":" + ex.Message);
                        ThreadPool.QueueUserWorkItem(new WaitCallback(this.ReConnectServer), obj);
                    }
                }
            }
        }

        /// <summary>当接收到字符串时候的触发事件</summary>
        public event Action<string> ReceivedString;

        /// <summary>当前的编码器</summary>
        public Encoding Encoding
        {
            get => this.encoding;
            set => this.encoding = value;
        }

        /// <summary>返回表示当前对象的字符串</summary>
        /// <returns>字符串</returns>
        public override string ToString() => string.Format("NetPlainSocket[{0}:{1}]", (object)this.ipAddress, (object)this.port);
    }
}
