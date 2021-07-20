// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.WebSocket.WebSocketQANet
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;
using ESTCore.Common.Core;
using ESTCore.Common.Core.Net;

using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.Common.WebSocket
{
    /// <summary>
    /// WebSocket的问答机制的客户端，本客户端将会在请求头上追加 RequestAndAnswer: true，本客户端将会请求服务器的信息，然后等待服务器的返回<br />
    /// Client of WebSocket Q &amp; A mechanism, this client will append RequestAndAnswer: true to the request header, this client will request the server information, and then wait for the server to return
    /// </summary>
    public class WebSocketQANet : NetworkDoubleBase
    {
        /// <summary>
        /// 根据指定的ip地址及端口号，实例化一个默认的对象<br />
        /// Instantiates a default object based on the specified IP address and port number
        /// </summary>
        /// <param name="ipAddress">远程服务器的ip地址</param>
        /// <param name="port">端口号信息</param>
        public WebSocketQANet(string ipAddress, int port)
        {
            this.IpAddress = EstHelper.GetIpAddressFromInput(ipAddress);
            this.Port = port;
            this.ByteTransform = (IByteTransform)new RegularByteTransform();
        }

        /// <inheritdoc />
        protected override OperateResult InitializationOnConnect(Socket socket)
        {
            byte[] data = WebSocketHelper.BuildWsQARequest(this.IpAddress, this.Port);
            OperateResult operateResult1 = this.Send(socket, data);
            if (!operateResult1.IsSuccess)
                return operateResult1;
            OperateResult<byte[]> operateResult2 = this.Receive(socket, -1, 10000);
            return !operateResult2.IsSuccess ? (OperateResult)operateResult2 : OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc />
        public override OperateResult<byte[]> ReadFromCoreServer(
          Socket socket,
          byte[] send,
          bool hasResponseData = true,
          bool usePackHeader = true)
        {
            this.LogNet?.WriteDebug(this.ToString(), StringResources.Language.Send + " : " + SoftBasic.ByteToHexString(send, ' '));
            OperateResult result = this.Send(socket, send);
            if (!result.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>(result);
            if (this.ReceiveTimeOut < 0)
                return OperateResult.CreateSuccessResult<byte[]>(new byte[0]);
            OperateResult<WebSocketMessage> webSocketPayload = this.ReceiveWebSocketPayload(socket);
            if (!webSocketPayload.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)webSocketPayload);
            this.LogNet?.WriteDebug(this.ToString(), string.Format("{0} : OpCode[{1}] Mask[{2}] {3}", (object)StringResources.Language.Receive, (object)webSocketPayload.Content.OpCode, (object)webSocketPayload.Content.HasMask, (object)SoftBasic.ByteToHexString(webSocketPayload.Content.Payload, ' ')));
            return OperateResult.CreateSuccessResult<byte[]>(webSocketPayload.Content.Payload);
        }

        /// <inheritdoc />
        protected override async Task<OperateResult> InitializationOnConnectAsync(
          Socket socket)
        {
            byte[] command = WebSocketHelper.BuildWsQARequest(this.IpAddress, this.Port);
            OperateResult send = await this.SendAsync(socket, command);
            if (!send.IsSuccess)
                return send;
            OperateResult<byte[]> rece = await this.ReceiveAsync(socket, -1, 10000);
            return rece.IsSuccess ? OperateResult.CreateSuccessResult() : (OperateResult)rece;
        }

        /// <inheritdoc />
        public override async Task<OperateResult<byte[]>> ReadFromCoreServerAsync(
          Socket socket,
          byte[] send,
          bool hasResponseData = true,
          bool usePackHeader = true)
        {
            this.LogNet?.WriteDebug(this.ToString(), StringResources.Language.Send + " : " + SoftBasic.ByteToHexString(send, ' '));
            OperateResult sendResult = await this.SendAsync(socket, send);
            if (!sendResult.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>(sendResult);
            if (this.ReceiveTimeOut < 0)
                return OperateResult.CreateSuccessResult<byte[]>(new byte[0]);
            OperateResult<WebSocketMessage> read = await this.ReceiveWebSocketPayloadAsync(socket);
            if (!read.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)read);
            this.LogNet?.WriteDebug(this.ToString(), string.Format("{0} : OpCode[{1}] Mask[{2}] {3}", (object)StringResources.Language.Receive, (object)read.Content.OpCode, (object)read.Content.HasMask, (object)SoftBasic.ByteToHexString(read.Content.Payload, ' ')));
            return OperateResult.CreateSuccessResult<byte[]>(read.Content.Payload);
        }

        /// <summary>
        /// 和websocket的服务器交互，将负载数据发送到服务器端，然后等待接收服务器的数据<br />
        /// Interact with the websocket server, send the load data to the server, and then wait to receive data from the server
        /// </summary>
        /// <param name="payload">数据负载</param>
        /// <returns>返回的结果数据</returns>
        public OperateResult<string> ReadFromServer(string payload) => ByteTransformHelper.GetSuccessResultFromOther<string, byte[]>(this.ReadFromCoreServer(WebSocketHelper.WebScoketPackData(1, true, payload)), new Func<byte[], string>(Encoding.UTF8.GetString));

        /// <inheritdoc cref="M:ESTCore.Common.WebSocket.WebSocketQANet.ReadFromServer(System.String)" />
        public async Task<OperateResult<string>> ReadFromServerAsync(string payload)
        {
            OperateResult<byte[]> result = await this.ReadFromCoreServerAsync(WebSocketHelper.WebScoketPackData(1, true, payload));
            return ByteTransformHelper.GetSuccessResultFromOther<string, byte[]>(result, new Func<byte[], string>(Encoding.UTF8.GetString));
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("WebSocketQANet[{0}:{1}]", (object)this.IpAddress, (object)this.Port);
    }
}
