// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Melsec.MelsecMcUdpServer
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Core.Net;

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ESTCore.Common.Profinet.Melsec
{
    /// <inheritdoc />
    public class MelsecMcUdpServer : MelsecMcServer
    {
        /// <summary>
        /// 实例化一个默认参数的mc协议的服务器<br />
        /// Instantiate a mc protocol server with default parameters
        /// </summary>
        /// <param name="isBinary">是否是二进制，默认是二进制，否则是ASCII格式</param>
        public MelsecMcUdpServer(bool isBinary = true)
          : base(isBinary)
        {
        }

        /// <summary>获取或设置一次接收时的数据长度，默认2KB数据长度</summary>
        public int ReceiveCacheLength { get; set; } = 2048;

        /// <inheritdoc />
        public override void ServerStart(int port)
        {
            if (this.IsStarted)
                return;
            this.StartInitialization();
            this.CoreSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.CoreSocket.Bind((EndPoint)new IPEndPoint(IPAddress.Any, port));
            this.RefreshReceive();
            this.IsStarted = true;
            this.Port = port;
            this.LogNet?.WriteInfo(this.ToString(), StringResources.Language.NetEngineStart);
        }

        /// <summary>重新开始接收数据</summary>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        private void RefreshReceive()
        {
            AppSession appSession = new AppSession();
            appSession.WorkSocket = this.CoreSocket;
            appSession.UdpEndPoint = (EndPoint)new IPEndPoint(IPAddress.Any, 0);
            appSession.BytesContent = new byte[this.ReceiveCacheLength];
            this.CoreSocket.BeginReceiveFrom(appSession.BytesContent, 0, this.ReceiveCacheLength, SocketFlags.None, ref appSession.UdpEndPoint, new System.AsyncCallback(this.AsyncCallback), (object)appSession);
        }

        private void AsyncCallback(IAsyncResult ar)
        {
            if (!(ar.AsyncState is AppSession asyncState))
                return;
            try
            {
                int from = asyncState.WorkSocket.EndReceiveFrom(ar, ref asyncState.UdpEndPoint);
                if (!ESTCore.Common.Authorization.asdniasnfaksndiqwhawfskhfaiw())
                {
                    this.RemoveClient(asyncState);
                }
                else
                {
                    this.RefreshReceive();
                    byte[] numArray1 = new byte[from];
                    Array.Copy((Array)asyncState.BytesContent, 0, (Array)numArray1, 0, from);
                    byte[] numArray2 = !this.IsBinary ? this.ReadFromMcAsciiCore(numArray1.RemoveBegin<byte>(22)) : this.ReadFromMcCore(numArray1.RemoveBegin<byte>(11));
                    this.LogNet?.WriteDebug(this.ToString(), "Udp " + StringResources.Language.Receive + "：" + (this.IsBinary ? numArray1.ToHexString(' ') : Encoding.ASCII.GetString(numArray1)));
                    if (numArray2 != null)
                    {
                        asyncState.WorkSocket.SendTo(numArray2, numArray2.Length, SocketFlags.None, asyncState.UdpEndPoint);
                        this.LogNet?.WriteDebug(this.ToString(), "Udp " + StringResources.Language.Send + "：" + (this.IsBinary ? numArray2.ToHexString(' ') : Encoding.ASCII.GetString(numArray2)));
                        this.RaiseDataReceived((object)asyncState, numArray1);
                    }
                    else
                        this.RemoveClient(asyncState);
                }
            }
            catch (ObjectDisposedException ex)
            {
            }
            catch (Exception ex)
            {
                this.LogNet?.WriteException(this.ToString(), StringResources.Language.SocketEndReceiveException, ex);
                this.RefreshReceive();
            }
            finally
            {
            }
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("MelsecMcUdpServer[{0}]", (object)this.Port);
    }
}
