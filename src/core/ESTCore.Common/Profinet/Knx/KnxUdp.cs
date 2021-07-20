// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Knx.KnxUdp
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.LogNet;

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ESTCore.Common.Profinet.Knx
{
    /// <summary>Knx驱动，具体的用法参照demo</summary>
    /// <remarks>感谢上海NULL提供的技术支持</remarks>
    public class KnxUdp
    {
        private const int stateRequestTimerInterval = 60000;
        private IPEndPoint _localEndpoint;
        private IPEndPoint _rouEndpoint;
        private KnxCode KNX_CODE;
        private UdpClient udpClient;
        private ILogNet logNet;

        /// <summary>实例化一个默认的对象</summary>
        public KnxUdp() => this.KNX_CODE = new KnxCode();

        /// <summary>通道号（由设备发来）</summary>
        public byte Channel { get; set; }

        /// <summary>远程ip地址</summary>
        public IPEndPoint RouEndpoint
        {
            get => this._rouEndpoint;
            set => this._rouEndpoint = value;
        }

        /// <summary>本机IP地址</summary>
        public IPEndPoint LocalEndpoint
        {
            get => this._localEndpoint;
            set => this._localEndpoint = value;
        }

        /// <summary>系统的日志信息</summary>
        public ILogNet LogNet
        {
            get => this.logNet;
            set => this.logNet = value;
        }

        /// <summary>当前的状态是否连接中</summary>
        public bool IsConnect => this.KNX_CODE.IsConnect;

        /// <summary>通信指令类</summary>
        public KnxCode KnxCode => this.KNX_CODE;

        /// <summary>和KNX网络进行握手并开始监听</summary>
        public void ConnectKnx()
        {
            if (this.udpClient == null)
                this.udpClient = new UdpClient(this.LocalEndpoint)
                {
                    Client = {
            DontFragment = true,
            SendBufferSize = 0,
            ReceiveTimeout = 120000
          }
                };
            this.udpClient.Send(this.KNX_CODE.Handshake(this.LocalEndpoint), 26, this.RouEndpoint);
            this.udpClient.BeginReceive(new AsyncCallback(this.ReceiveCallback), (object)null);
            Thread.Sleep(1000);
            if (!this.KNX_CODE.IsConnect)
                return;
            this.KNX_CODE.Return_data_msg += new KnxCode.ReturnData(this.KNX_CODE_Return_data_msg);
            this.KNX_CODE.GetData_msg += new KnxCode.GetData(this.KNX_CODE_GetData_msg);
            this.KNX_CODE.Set_knx_data += new KnxCode.ReturnData(this.KNX_CODE_Set_knx_data);
            this.KNX_CODE.knx_server_is_real(this.LocalEndpoint);
        }

        /// <summary>保持KNX连接</summary>
        public void KeepConnection() => this.KNX_CODE.knx_server_is_real(this.LocalEndpoint);

        /// <summary>关闭连接</summary>
        public void DisConnectKnx()
        {
            if (this.KNX_CODE.Channel <= (byte)0)
                return;
            byte[] dgram = this.KNX_CODE.Disconnect_knx(this.KNX_CODE.Channel, this.LocalEndpoint);
            this.udpClient.Send(dgram, dgram.Length, this.RouEndpoint);
        }

        /// <summary>将报文写入KNX系统</summary>
        /// <param name="addr">地址</param>
        /// <param name="len">长度</param>
        /// <param name="data">数据</param>
        public void SetKnxData(short addr, byte len, byte[] data) => this.KNX_CODE.Knx_Write(addr, len, data);

        /// <summary>读取指定KNX组地址</summary>
        /// <param name="addr">地址</param>
        public void ReadKnxData(short addr)
        {
            this.KNX_CODE.knx_server_is_real(this.LocalEndpoint);
            this.KNX_CODE.Knx_Resd_step1(addr);
        }

        private void KNX_CODE_Set_knx_data(byte[] data) => this.udpClient.Send(data, data.Length, this.RouEndpoint);

        private void KNX_CODE_GetData_msg(short addr, byte len, byte[] data) => this.logNet?.WriteDebug("收到数据 地址：" + addr.ToString() + " 长度:" + len.ToString() + "数据：" + BitConverter.ToString(data));

        private void KNX_CODE_Return_data_msg(byte[] data) => this.udpClient.Send(data, data.Length, this.RouEndpoint);

        private void ReceiveCallback(IAsyncResult iar)
        {
            byte[] in_data = this.udpClient.EndReceive(iar, ref this._rouEndpoint);
            this.logNet?.WriteDebug("收到报文 {0}", BitConverter.ToString(in_data));
            this.KNX_CODE.KNX_check(in_data);
            if (!this.KNX_CODE.IsConnect)
                return;
            this.udpClient.BeginReceive(new AsyncCallback(this.ReceiveCallback), (object)null);
        }
    }
}
