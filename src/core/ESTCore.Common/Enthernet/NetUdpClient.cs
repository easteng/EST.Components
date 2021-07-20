// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Enthernet.NetUdpClient
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Core.Net;

using System.Text;

namespace ESTCore.Common.Enthernet
{
    /// <summary>
    /// UDP客户端的类，负责发送数据到服务器，然后从服务器接收对应的数据信息，该数据经过HSL封装<br />
    /// UDP client class, responsible for sending data to the server, and then receiving the corresponding data information from the server, the data is encapsulated by HSL
    /// </summary>
    public class NetUdpClient : NetworkUdpBase
    {
        /// <summary>
        /// 实例化对象，指定发送的服务器地址和端口号<br />
        /// Instantiated object, specifying the server address and port number to send
        /// </summary>
        /// <param name="ipAddress">服务器的Ip地址</param>
        /// <param name="port">端口号</param>
        public NetUdpClient(string ipAddress, int port)
        {
            this.IpAddress = ipAddress;
            this.Port = port;
        }

        /// <summary>
        /// 客户端向服务器进行请求，请求字符串数据，忽略了自定义消息反馈<br />
        /// The client makes a request to the server, requesting string data, and ignoring custom message feedback
        /// </summary>
        /// <param name="customer">用户的指令头</param>
        /// <param name="send">发送数据</param>
        /// <returns>带返回消息的结果对象</returns>
        public OperateResult<string> ReadFromServer(NetHandle customer, string send = null)
        {
            OperateResult<byte[]> operateResult = this.ReadFromServerBase(EstProtocol.CommandBytes((int)customer, this.Token, send));
            return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<string>((OperateResult)operateResult) : OperateResult.CreateSuccessResult<string>(Encoding.Unicode.GetString(operateResult.Content));
        }

        /// <summary>
        /// 客户端向服务器进行请求，请求字节数据<br />
        /// The client makes a request to the server, requesting byte data
        /// </summary>
        /// <param name="customer">用户的指令头</param>
        /// <param name="send">发送的字节内容</param>
        /// <returns>带返回消息的结果对象</returns>
        public OperateResult<byte[]> ReadFromServer(NetHandle customer, byte[] send) => this.ReadFromServerBase(EstProtocol.CommandBytes((int)customer, this.Token, send));

        /// <summary>
        /// 客户端向服务器进行请求，请求字符串数据，并返回状态信息<br />
        /// The client makes a request to the server, requests string data, and returns status information
        /// </summary>
        /// <param name="customer">用户的指令头</param>
        /// <param name="send">发送数据</param>
        /// <returns>带返回消息的结果对象</returns>
        public OperateResult<NetHandle, string> ReadCustomerFromServer(
          NetHandle customer,
          string send = null)
        {
            OperateResult<NetHandle, byte[]> operateResult = this.ReadCustomerFromServerBase(EstProtocol.CommandBytes((int)customer, this.Token, send));
            return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<NetHandle, string>((OperateResult)operateResult) : OperateResult.CreateSuccessResult<NetHandle, string>(operateResult.Content1, Encoding.Unicode.GetString(operateResult.Content2));
        }

        /// <summary>
        /// 客户端向服务器进行请求，请求字节数据，并返回状态信息<br />
        /// The client makes a request to the server, requests byte data, and returns status information
        /// </summary>
        /// <param name="customer">用户的指令头</param>
        /// <param name="send">发送数据</param>
        /// <returns>带返回消息的结果对象</returns>
        public OperateResult<NetHandle, byte[]> ReadCustomerFromServer(
          NetHandle customer,
          byte[] send)
        {
            return this.ReadCustomerFromServerBase(EstProtocol.CommandBytes((int)customer, this.Token, send));
        }

        /// <summary>
        /// 发送的底层数据，然后返回结果数据<br />
        /// Send the underlying data and then return the result data
        /// </summary>
        /// <param name="send">需要发送的底层数据</param>
        /// <returns>带返回消息的结果对象</returns>
        private OperateResult<byte[]> ReadFromServerBase(byte[] send)
        {
            OperateResult<NetHandle, byte[]> operateResult = this.ReadCustomerFromServerBase(send);
            return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult) : OperateResult.CreateSuccessResult<byte[]>(operateResult.Content2);
        }

        /// <summary>
        /// 发送的底层数据，然后返回结果数据，该结果是带Handle信息的。<br />
        /// Send the underlying data, and then return the result data, the result is with Handle information.
        /// </summary>
        /// <param name="send">需要发送的底层数据</param>
        /// <returns>带返回消息的结果对象</returns>
        private OperateResult<NetHandle, byte[]> ReadCustomerFromServerBase(
          byte[] send)
        {
            OperateResult<byte[]> operateResult = this.ReadFromCoreServer(send);
            return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<NetHandle, byte[]>((OperateResult)operateResult) : EstProtocol.ExtractEstData(operateResult.Content);
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("NetUdpClient[{0}:{1}]", (object)this.IpAddress, (object)this.Port);
    }
}
