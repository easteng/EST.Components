// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.DCS.DcsNanJingAuto
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;
using ESTCore.Common.Core.IMessage;
using ESTCore.Common.ModBus;

using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESTCore.Common.DCS
{
    /// <summary>南京自动化研究所的DCS系统，基于modbus实现，但是不是标准的实现</summary>
    public class DcsNanJingAuto : ModbusTcpNet
    {
        private byte[] headCommand = new byte[12]
        {
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 6,
      (byte) 1,
      (byte) 3,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 1
        };

        /// <summary>
        /// 实例化一个默认的对象<br />
        /// Instantiate a default object
        /// </summary>
        public DcsNanJingAuto()
        {
        }

        /// <inheritdoc />
        public DcsNanJingAuto(string ipAddress, int port = 502, byte station = 1)
          : base(ipAddress, port, station)
        {
        }

        /// <inheritdoc />
        protected override INetMessage GetNewNetMessage() => (INetMessage)new DcsNanJingAutoMessage();

        /// <inheritdoc />
        protected override OperateResult InitializationOnConnect(Socket socket)
        {
            this.MessageId.ResetCurrentValue(0L);
            this.headCommand[6] = this.Station;
            OperateResult operateResult1 = this.Send(socket, this.headCommand);
            if (!operateResult1.IsSuccess)
                return operateResult1;
            OperateResult<byte[]> operateResult2 = this.Receive(socket, -1, 3000);
            return !operateResult2.IsSuccess ? (OperateResult)operateResult2 : (this.CheckResponseStatus(operateResult2.Content) ? base.InitializationOnConnect(socket) : new OperateResult("Check Status Response failed: " + operateResult2.Content.ToHexString(' ')));
        }

        /// <inheritdoc />
        protected override async Task<OperateResult> InitializationOnConnectAsync(
          Socket socket)
        {
            this.MessageId.ResetCurrentValue(0L);
            this.headCommand[6] = this.Station;
            OperateResult send = await this.SendAsync(socket, this.headCommand);
            if (!send.IsSuccess)
                return send;
            OperateResult<byte[]> receive = await this.ReceiveAsync(socket, -1, 3000);
            return receive.IsSuccess ? (this.CheckResponseStatus(receive.Content) ? OperateResult.CreateSuccessResult() : new OperateResult("Check Status Response failed: " + receive.Content.ToHexString(' '))) : (OperateResult)receive;
        }

        /// <inheritdoc />
        public override OperateResult<byte[]> ReadFromCoreServer(
          Socket socket,
          byte[] send,
          bool hasResponseData = true,
          bool usePackHeader = true)
        {
            this.LogNet?.WriteDebug(this.ToString(), StringResources.Language.Send + " : " + (this.LogMsgFormatBinary ? send.ToHexString(' ') : Encoding.ASCII.GetString(send)));
            INetMessage newNetMessage = this.GetNewNetMessage();
            if (newNetMessage != null)
                newNetMessage.SendBytes = send;
            OperateResult result = this.Send(socket, send);
            if (!result.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>(result);
            if (this.receiveTimeOut < 0)
                return OperateResult.CreateSuccessResult<byte[]>(new byte[0]);
            if (!hasResponseData)
                return OperateResult.CreateSuccessResult<byte[]>(new byte[0]);
            if (this.SleepTime > 0)
                Thread.Sleep(this.SleepTime);
            OperateResult<byte[]> byMessage = this.ReceiveByMessage(socket, this.receiveTimeOut, newNetMessage);
            if (!byMessage.IsSuccess)
                return byMessage;
            this.LogNet?.WriteDebug(this.ToString(), StringResources.Language.Receive + " : " + (this.LogMsgFormatBinary ? byMessage.Content.ToHexString(' ') : Encoding.ASCII.GetString(byMessage.Content)));
            if (byMessage.Content.Length == 6 && this.CheckResponseStatus(byMessage.Content))
                byMessage = this.ReceiveByMessage(socket, this.receiveTimeOut, newNetMessage);
            if (newNetMessage == null || newNetMessage.CheckHeadBytesLegal(this.Token.ToByteArray()))
                return OperateResult.CreateSuccessResult<byte[]>(byMessage.Content);
            socket?.Close();
            return new OperateResult<byte[]>(StringResources.Language.CommandHeadCodeCheckFailed + Environment.NewLine + StringResources.Language.Send + ": " + SoftBasic.ByteToHexString(send, ' ') + Environment.NewLine + StringResources.Language.Receive + ": " + SoftBasic.ByteToHexString(byMessage.Content, ' '));
        }

        /// <inheritdoc />
        public override async Task<OperateResult<byte[]>> ReadFromCoreServerAsync(
          Socket socket,
          byte[] send,
          bool hasResponseData = true,
          bool usePackHeader = true)
        {
            this.LogNet?.WriteDebug(this.ToString(), StringResources.Language.Send + " : " + (this.LogMsgFormatBinary ? send.ToHexString(' ') : Encoding.ASCII.GetString(send)));
            INetMessage netMessage = this.GetNewNetMessage();
            if (netMessage != null)
                netMessage.SendBytes = send;
            OperateResult sendResult = await this.SendAsync(socket, send);
            if (!sendResult.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>(sendResult);
            if (this.receiveTimeOut < 0)
                return OperateResult.CreateSuccessResult<byte[]>(new byte[0]);
            if (!hasResponseData)
                return OperateResult.CreateSuccessResult<byte[]>(new byte[0]);
            if (this.SleepTime > 0)
                await Task.Delay(this.SleepTime);
            OperateResult<byte[]> resultReceive = await this.ReceiveByMessageAsync(socket, this.receiveTimeOut, netMessage);
            if (!resultReceive.IsSuccess)
                return resultReceive;
            this.LogNet?.WriteDebug(this.ToString(), StringResources.Language.Receive + " : " + (this.LogMsgFormatBinary ? resultReceive.Content.ToHexString(' ') : Encoding.ASCII.GetString(resultReceive.Content)));
            if (resultReceive.Content.Length == 6 && this.CheckResponseStatus(resultReceive.Content))
                resultReceive = await this.ReceiveByMessageAsync(socket, this.receiveTimeOut, netMessage);
            if (netMessage == null || netMessage.CheckHeadBytesLegal(this.Token.ToByteArray()))
                return OperateResult.CreateSuccessResult<byte[]>(resultReceive.Content);
            socket?.Close();
            return new OperateResult<byte[]>(StringResources.Language.CommandHeadCodeCheckFailed + Environment.NewLine + StringResources.Language.Send + ": " + SoftBasic.ByteToHexString(send, ' ') + Environment.NewLine + StringResources.Language.Receive + ": " + SoftBasic.ByteToHexString(resultReceive.Content, ' '));
        }

        private bool CheckResponseStatus(byte[] content)
        {
            if (content.Length < 6)
                return false;
            for (int index = content.Length - 4; index < content.Length; ++index)
            {
                if (content[index] > (byte)0)
                    return false;
            }
            return true;
        }
    }
}
