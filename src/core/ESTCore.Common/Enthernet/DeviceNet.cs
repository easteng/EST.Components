// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Enthernet.DeviceNet
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Core;
using ESTCore.Common.Core.Net;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ESTCore.Common.Enthernet
{
    /// <summary>通用设备的基础网络信息</summary>
    public class DeviceNet : NetworkServerBase
    {
        private List<DeviceState> list;
        private SimpleHybirdLock lock_list;
        private readonly byte endByte = 13;

        /// <summary>实例化一个通用的设备类</summary>
        public DeviceNet()
        {
            this.list = new List<DeviceState>();
            this.lock_list = new SimpleHybirdLock();
        }

        private void AddClient(DeviceState device)
        {
            this.lock_list.Enter();
            this.list.Add(device);
            this.lock_list.Leave();
            Action<DeviceState> clientOnline = this.ClientOnline;
            if (clientOnline == null)
                return;
            clientOnline(device);
        }

        private void RemoveClient(DeviceState device)
        {
            this.lock_list.Enter();
            this.list.Remove(device);
            device.WorkSocket?.Close();
            this.lock_list.Leave();
            Action<DeviceState> clientOffline = this.ClientOffline;
            if (clientOffline == null)
                return;
            clientOffline(device);
        }

        /// <summary>当客户端上线的时候，触发此事件</summary>
        public event Action<DeviceState> ClientOnline;

        /// <summary>当客户端下线的时候，触发此事件</summary>
        public event Action<DeviceState> ClientOffline;

        /// <summary>按照ASCII文本的方式进行触发接收的数据</summary>
        public event Action<DeviceState, string> AcceptString;

        /// <summary>按照字节的方式进行触发接收的数据</summary>
        public event Action<DeviceState, byte[]> AcceptBytes;

        /// <summary>当接收到了新的请求的时候执行的操作</summary>
        /// <param name="socket">异步对象</param>
        /// <param name="endPoint">终结点</param>
        protected override void ThreadPoolLogin(Socket socket, IPEndPoint endPoint)
        {
            DeviceState device = new DeviceState()
            {
                WorkSocket = socket,
                DeviceEndPoint = (IPEndPoint)socket.RemoteEndPoint,
                IpAddress = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString(),
                ConnectTime = DateTime.Now
            };
            this.AddClient(device);
            try
            {
                device.WorkSocket.BeginReceive(device.Buffer, 0, device.Buffer.Length, SocketFlags.None, new AsyncCallback(this.ContentReceiveCallBack), (object)device);
            }
            catch (Exception ex)
            {
                this.RemoveClient(device);
                this.LogNet?.WriteException(this.ToString(), StringResources.Language.NetClientLoginFailed, ex);
            }
        }

        private void ContentReceiveCallBack(IAsyncResult ar)
        {
            if (!(ar.AsyncState is DeviceState asyncState))
                return;
            try
            {
                if (asyncState.WorkSocket.EndReceive(ar) > 0)
                {
                    MemoryStream memoryStream = new MemoryStream();
                    byte[] buffer;
                    for (byte index = asyncState.Buffer[0]; (int)index != (int)this.endByte; index = buffer[0])
                    {
                        memoryStream.WriteByte(index);
                        buffer = new byte[1];
                        asyncState.WorkSocket.Receive(buffer, 0, 1, SocketFlags.None);
                    }
                    asyncState.WorkSocket.BeginReceive(asyncState.Buffer, 0, asyncState.Buffer.Length, SocketFlags.None, new AsyncCallback(this.ContentReceiveCallBack), (object)asyncState);
                    byte[] array = memoryStream.ToArray();
                    memoryStream.Dispose();
                    this.lock_list.Enter();
                    asyncState.ReceiveTime = DateTime.Now;
                    this.lock_list.Leave();
                    Action<DeviceState, byte[]> acceptBytes = this.AcceptBytes;
                    if (acceptBytes != null)
                        acceptBytes(asyncState, array);
                    Action<DeviceState, string> acceptString = this.AcceptString;
                    if (acceptString != null)
                        acceptString(asyncState, Encoding.ASCII.GetString(array));
                }
                else
                {
                    this.RemoveClient(asyncState);
                    this.LogNet?.WriteInfo(this.ToString(), StringResources.Language.NetClientOffline);
                }
            }
            catch (Exception ex)
            {
                this.RemoveClient(asyncState);
                this.LogNet?.WriteException(this.ToString(), StringResources.Language.NetClientLoginFailed, ex);
            }
        }
    }
}
