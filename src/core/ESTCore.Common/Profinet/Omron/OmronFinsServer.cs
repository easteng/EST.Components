// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Omron.OmronFinsServer
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;
using ESTCore.Common.Core;
using ESTCore.Common.Core.Address;
using ESTCore.Common.Core.IMessage;
using ESTCore.Common.Core.Net;
using ESTCore.Common.Reflection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace ESTCore.Common.Profinet.Omron
{
    /// <summary>
    /// <b>[商业授权]</b> 欧姆龙的虚拟服务器，支持DM区，CIO区，Work区，Hold区，Auxiliary区，可以方便的进行测试<br />
    /// <b>[Authorization]</b> Omron's virtual server supports DM area, CIO area, Work area, Hold area, and Auxiliary area, which can be easily tested
    /// </summary>
    public class OmronFinsServer : NetworkDataServerBase
    {
        private SoftBuffer dBuffer;
        private SoftBuffer cioBuffer;
        private SoftBuffer wBuffer;
        private SoftBuffer hBuffer;
        private SoftBuffer arBuffer;
        private const int DataPoolLength = 65536;
        /// <summary>头部命令的长度，用来在tcp协议和udp协议传输的时候区别</summary>
        protected int headLength = 16;

        /// <summary>
        /// 实例化一个Fins协议的服务器<br />
        /// Instantiate a Fins protocol server
        /// </summary>
        public OmronFinsServer()
        {
            this.dBuffer = new SoftBuffer(131072);
            this.cioBuffer = new SoftBuffer(131072);
            this.wBuffer = new SoftBuffer(131072);
            this.hBuffer = new SoftBuffer(131072);
            this.arBuffer = new SoftBuffer(131072);
            this.dBuffer.IsBoolReverseByWord = true;
            this.cioBuffer.IsBoolReverseByWord = true;
            this.wBuffer.IsBoolReverseByWord = true;
            this.hBuffer.IsBoolReverseByWord = true;
            this.arBuffer.IsBoolReverseByWord = true;
            this.WordLength = (ushort)1;
            this.ByteTransform = (IByteTransform)new ReverseWordTransform();
            this.ByteTransform.DataFormat = DataFormat.CDAB;
        }

        /// <inheritdoc cref="P:ESTCore.Common.Core.ByteTransformBase.DataFormat" />
        public DataFormat DataFormat
        {
            get => this.ByteTransform.DataFormat;
            set => this.ByteTransform.DataFormat = value;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronFinsNet.Read(System.String,System.UInt16)" />
        [EstMqttApi("ReadByteArray", "")]
        public override OperateResult<byte[]> Read(string address, ushort length)
        {
            OperateResult<OmronFinsAddress> from = OmronFinsAddress.ParseFrom(address);
            if (!from.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)from);
            if ((int)from.Content.WordCode == (int)OmronFinsDataType.DM.WordCode)
                return OperateResult.CreateSuccessResult<byte[]>(this.dBuffer.GetBytes(from.Content.AddressStart / 16 * 2, (int)length * 2));
            if ((int)from.Content.WordCode == (int)OmronFinsDataType.CIO.WordCode)
                return OperateResult.CreateSuccessResult<byte[]>(this.cioBuffer.GetBytes(from.Content.AddressStart / 16 * 2, (int)length * 2));
            if ((int)from.Content.WordCode == (int)OmronFinsDataType.WR.WordCode)
                return OperateResult.CreateSuccessResult<byte[]>(this.wBuffer.GetBytes(from.Content.AddressStart / 16 * 2, (int)length * 2));
            if ((int)from.Content.WordCode == (int)OmronFinsDataType.HR.WordCode)
                return OperateResult.CreateSuccessResult<byte[]>(this.hBuffer.GetBytes(from.Content.AddressStart / 16 * 2, (int)length * 2));
            return (int)from.Content.WordCode == (int)OmronFinsDataType.AR.WordCode ? OperateResult.CreateSuccessResult<byte[]>(this.arBuffer.GetBytes(from.Content.AddressStart / 16 * 2, (int)length * 2)) : new OperateResult<byte[]>(StringResources.Language.NotSupportedFunction);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronFinsNet.Write(System.String,System.Byte[])" />
        [EstMqttApi("WriteByteArray", "")]
        public override OperateResult Write(string address, byte[] value)
        {
            OperateResult<OmronFinsAddress> from = OmronFinsAddress.ParseFrom(address);
            if (!from.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<byte[]>((OperateResult)from);
            if ((int)from.Content.WordCode == (int)OmronFinsDataType.DM.WordCode)
                this.dBuffer.SetBytes(value, from.Content.AddressStart / 16 * 2);
            else if ((int)from.Content.WordCode == (int)OmronFinsDataType.CIO.WordCode)
                this.cioBuffer.SetBytes(value, from.Content.AddressStart / 16 * 2);
            else if ((int)from.Content.WordCode == (int)OmronFinsDataType.WR.WordCode)
                this.wBuffer.SetBytes(value, from.Content.AddressStart / 16 * 2);
            else if ((int)from.Content.WordCode == (int)OmronFinsDataType.HR.WordCode)
            {
                this.hBuffer.SetBytes(value, from.Content.AddressStart / 16 * 2);
            }
            else
            {
                if ((int)from.Content.WordCode != (int)OmronFinsDataType.AR.WordCode)
                    return new OperateResult(StringResources.Language.NotSupportedFunction);
                this.arBuffer.SetBytes(value, from.Content.AddressStart / 16 * 2);
            }
            return OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronFinsNet.ReadBool(System.String,System.UInt16)" />
        [EstMqttApi("ReadBoolArray", "")]
        public override OperateResult<bool[]> ReadBool(string address, ushort length)
        {
            OperateResult<OmronFinsAddress> from = OmronFinsAddress.ParseFrom(address);
            if (!from.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)from);
            if ((int)from.Content.BitCode == (int)OmronFinsDataType.DM.BitCode)
                return OperateResult.CreateSuccessResult<bool[]>(this.dBuffer.GetBool(from.Content.AddressStart, (int)length));
            if ((int)from.Content.BitCode == (int)OmronFinsDataType.CIO.BitCode)
                return OperateResult.CreateSuccessResult<bool[]>(this.cioBuffer.GetBool(from.Content.AddressStart, (int)length));
            if ((int)from.Content.BitCode == (int)OmronFinsDataType.WR.BitCode)
                return OperateResult.CreateSuccessResult<bool[]>(this.wBuffer.GetBool(from.Content.AddressStart, (int)length));
            if ((int)from.Content.BitCode == (int)OmronFinsDataType.HR.BitCode)
                return OperateResult.CreateSuccessResult<bool[]>(this.hBuffer.GetBool(from.Content.AddressStart, (int)length));
            return (int)from.Content.BitCode == (int)OmronFinsDataType.AR.BitCode ? OperateResult.CreateSuccessResult<bool[]>(this.arBuffer.GetBool(from.Content.AddressStart, (int)length)) : new OperateResult<bool[]>(StringResources.Language.NotSupportedFunction);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronFinsNet.Write(System.String,System.Boolean[])" />
        [EstMqttApi("WriteBoolArray", "")]
        public override OperateResult Write(string address, bool[] value)
        {
            OperateResult<OmronFinsAddress> from = OmronFinsAddress.ParseFrom(address);
            if (!from.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<bool[]>((OperateResult)from);
            if ((int)from.Content.BitCode == (int)OmronFinsDataType.DM.BitCode)
                this.dBuffer.SetBool(value, from.Content.AddressStart);
            else if ((int)from.Content.BitCode == (int)OmronFinsDataType.CIO.BitCode)
                this.cioBuffer.SetBool(value, from.Content.AddressStart);
            else if ((int)from.Content.BitCode == (int)OmronFinsDataType.WR.BitCode)
                this.wBuffer.SetBool(value, from.Content.AddressStart);
            else if ((int)from.Content.BitCode == (int)OmronFinsDataType.HR.BitCode)
            {
                this.hBuffer.SetBool(value, from.Content.AddressStart);
            }
            else
            {
                if ((int)from.Content.BitCode != (int)OmronFinsDataType.AR.BitCode)
                    return new OperateResult(StringResources.Language.NotSupportedFunction);
                this.arBuffer.SetBool(value, from.Content.AddressStart);
            }
            return OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc />
        protected override void ThreadPoolLoginAfterClientCheck(Socket socket, IPEndPoint endPoint)
        {
            FinsMessage finsMessage = new FinsMessage();
            if (!this.ReceiveByMessage(socket, 5000, (INetMessage)finsMessage).IsSuccess || !this.Send(socket, SoftBasic.HexStringToBytes("46 49 4E 53 00 00 00 10 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 01")).IsSuccess)
                return;
            AppSession session = new AppSession();
            session.IpEndPoint = endPoint;
            session.WorkSocket = socket;
            try
            {
                socket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, new AsyncCallback(this.SocketAsyncCallBack), (object)session);
                this.AddClient(session);
            }
            catch
            {
                socket.Close();
                this.LogNet?.WriteDebug(this.ToString(), string.Format(StringResources.Language.ClientOfflineInfo, (object)endPoint));
            }
        }

        private async void SocketAsyncCallBack(IAsyncResult ar)
        {
            if (!(ar.AsyncState is AppSession session))
            {
                session = (AppSession)null;
            }
            else
            {
                try
                {
                    int receiveCount = session.WorkSocket.EndReceive(ar);
                    OperateResult<byte[]> read1 = await this.ReceiveByMessageAsync(session.WorkSocket, 5000, (INetMessage)new FinsMessage());
                    if (!read1.IsSuccess)
                    {
                        this.RemoveClient(session);
                        session = (AppSession)null;
                        return;
                    }
                    if (!ESTCore.Common.Authorization.asdniasnfaksndiqwhawfskhfaiw())
                    {
                        this.RemoveClient(session);
                        session = (AppSession)null;
                        return;
                    }
                    this.LogNet?.WriteDebug(this.ToString(), string.Format("[{0}] Tcp {1}：{2}", (object)session.IpEndPoint, (object)StringResources.Language.Receive, (object)read1.Content.ToHexString(' ')));
                    byte[] back = this.ReadFromFinsCore(read1.Content);
                    if (back != null)
                    {
                        session.WorkSocket.Send(back);
                        this.LogNet?.WriteDebug(this.ToString(), string.Format("[{0}] Tcp {1}：{2}", (object)session.IpEndPoint, (object)StringResources.Language.Send, (object)back.ToHexString(' ')));
                        session.HeartTime = DateTime.Now;
                        this.RaiseDataReceived((object)session, read1.Content);
                        session.WorkSocket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, new AsyncCallback(this.SocketAsyncCallBack), (object)session);
                        read1 = (OperateResult<byte[]>)null;
                        back = (byte[])null;
                    }
                    else
                    {
                        this.RemoveClient(session);
                        session = (AppSession)null;
                        return;
                    }
                }
                catch
                {
                    this.RemoveClient(session);
                }
                session = (AppSession)null;
            }
        }

        /// <summary>
        /// 当收到mc协议的报文的时候应该触发的方法，允许继承重写，来实现自定义的返回，或是数据监听。<br />
        /// The method that should be triggered when a message of the mc protocol is received is allowed to be inherited and rewritten to achieve a custom return or data monitoring.
        /// </summary>
        /// <param name="finsCore">mc报文</param>
        /// <returns>返回的报文信息</returns>
        protected virtual byte[] ReadFromFinsCore(byte[] finsCore)
        {
            if (finsCore[this.headLength + 10] == (byte)1 && finsCore[this.headLength + 11] == (byte)1)
            {
                byte[] data = this.ReadByCommand(SoftBasic.ArrayRemoveBegin<byte>(finsCore, this.headLength + 10));
                return this.PackCommand(data == null ? 2 : 0, data);
            }
            if (finsCore[this.headLength + 10] != (byte)1 || finsCore[this.headLength + 11] != (byte)2)
                return (byte[])null;
            return !this.EnableWrite ? this.PackCommand(3, (byte[])null) : this.PackCommand(0, this.WriteByMessage(SoftBasic.ArrayRemoveBegin<byte>(finsCore, this.headLength + 10)));
        }

        /// <summary>
        /// 将核心报文打包的方法，追加报文头<br />
        /// The method of packing the core message, adding the message header
        /// </summary>
        /// <param name="status">错误码</param>
        /// <param name="data">核心的内容</param>
        /// <returns>完整的报文信息</returns>
        protected virtual byte[] PackCommand(int status, byte[] data)
        {
            if (data == null)
                data = new byte[0];
            byte[] numArray = new byte[30 + data.Length];
            SoftBasic.HexStringToBytes("46 49 4E 53 00 00 00 0000 00 00 00 00 00 00 0000 00 00 00 00 00 00 00 00 00 00 00 00 00").CopyTo((Array)numArray, 0);
            if ((uint)data.Length > 0U)
                data.CopyTo((Array)numArray, 30);
            BitConverter.GetBytes(numArray.Length - 8).ReverseNew<byte>().CopyTo((Array)numArray, 4);
            BitConverter.GetBytes(status).ReverseNew<byte>().CopyTo((Array)numArray, 12);
            return numArray;
        }

        private byte[] ReadByCommand(byte[] command)
        {
            if ((int)command[2] == (int)OmronFinsDataType.DM.BitCode || (int)command[2] == (int)OmronFinsDataType.CIO.BitCode || ((int)command[2] == (int)OmronFinsDataType.WR.BitCode || (int)command[2] == (int)OmronFinsDataType.HR.BitCode) || (int)command[2] == (int)OmronFinsDataType.AR.BitCode)
            {
                ushort num = (ushort)((uint)command[6] * 256U + (uint)command[7]);
                int destIndex = ((int)command[3] * 256 + (int)command[4]) * 16 + (int)command[5];
                if ((int)command[2] == (int)OmronFinsDataType.DM.BitCode)
                    return ((IEnumerable<bool>)this.dBuffer.GetBool(destIndex, (int)num)).Select<bool, byte>((Func<bool, byte>)(m => !m ? (byte)0 : (byte)1)).ToArray<byte>();
                if ((int)command[2] == (int)OmronFinsDataType.CIO.BitCode)
                    return ((IEnumerable<bool>)this.cioBuffer.GetBool(destIndex, (int)num)).Select<bool, byte>((Func<bool, byte>)(m => !m ? (byte)0 : (byte)1)).ToArray<byte>();
                if ((int)command[2] == (int)OmronFinsDataType.WR.BitCode)
                    return ((IEnumerable<bool>)this.wBuffer.GetBool(destIndex, (int)num)).Select<bool, byte>((Func<bool, byte>)(m => !m ? (byte)0 : (byte)1)).ToArray<byte>();
                if ((int)command[2] == (int)OmronFinsDataType.HR.BitCode)
                    return ((IEnumerable<bool>)this.hBuffer.GetBool(destIndex, (int)num)).Select<bool, byte>((Func<bool, byte>)(m => !m ? (byte)0 : (byte)1)).ToArray<byte>();
                if ((int)command[2] == (int)OmronFinsDataType.AR.BitCode)
                    return ((IEnumerable<bool>)this.arBuffer.GetBool(destIndex, (int)num)).Select<bool, byte>((Func<bool, byte>)(m => !m ? (byte)0 : (byte)1)).ToArray<byte>();
                throw new Exception(StringResources.Language.NotSupportedDataType);
            }
            if ((int)command[2] != (int)OmronFinsDataType.DM.WordCode && (int)command[2] != (int)OmronFinsDataType.CIO.WordCode && ((int)command[2] != (int)OmronFinsDataType.WR.WordCode && (int)command[2] != (int)OmronFinsDataType.HR.WordCode) && (int)command[2] != (int)OmronFinsDataType.AR.WordCode)
                return new byte[0];
            ushort num1 = (ushort)((uint)command[6] * 256U + (uint)command[7]);
            int num2 = (int)command[3] * 256 + (int)command[4];
            if (num1 > (ushort)999)
                return (byte[])null;
            if ((int)command[2] == (int)OmronFinsDataType.DM.WordCode)
                return this.dBuffer.GetBytes(num2 * 2, (int)num1 * 2);
            if ((int)command[2] == (int)OmronFinsDataType.CIO.WordCode)
                return this.cioBuffer.GetBytes(num2 * 2, (int)num1 * 2);
            if ((int)command[2] == (int)OmronFinsDataType.WR.WordCode)
                return this.wBuffer.GetBytes(num2 * 2, (int)num1 * 2);
            if ((int)command[2] == (int)OmronFinsDataType.HR.WordCode)
                return this.hBuffer.GetBytes(num2 * 2, (int)num1 * 2);
            if ((int)command[2] == (int)OmronFinsDataType.AR.WordCode)
                return this.arBuffer.GetBytes(num2 * 2, (int)num1 * 2);
            throw new Exception(StringResources.Language.NotSupportedDataType);
        }

        private byte[] WriteByMessage(byte[] command)
        {
            if ((int)command[2] == (int)OmronFinsDataType.DM.BitCode || (int)command[2] == (int)OmronFinsDataType.CIO.BitCode || ((int)command[2] == (int)OmronFinsDataType.WR.BitCode || (int)command[2] == (int)OmronFinsDataType.HR.BitCode) || (int)command[2] == (int)OmronFinsDataType.AR.BitCode)
            {
                ushort num = (ushort)((uint)command[6] * 256U + (uint)command[7]);
                int destIndex = ((int)command[3] * 256 + (int)command[4]) * 16 + (int)command[5];
                bool[] array = ((IEnumerable<byte>)SoftBasic.ArrayRemoveBegin<byte>(command, 8)).Select<byte, bool>((Func<byte, bool>)(m => m == (byte)1)).ToArray<bool>();
                if ((int)command[2] == (int)OmronFinsDataType.DM.BitCode)
                    this.dBuffer.SetBool(array, destIndex);
                else if ((int)command[2] == (int)OmronFinsDataType.CIO.BitCode)
                    this.cioBuffer.SetBool(array, destIndex);
                else if ((int)command[2] == (int)OmronFinsDataType.WR.BitCode)
                    this.wBuffer.SetBool(array, destIndex);
                else if ((int)command[2] == (int)OmronFinsDataType.HR.BitCode)
                {
                    this.hBuffer.SetBool(array, destIndex);
                }
                else
                {
                    if ((int)command[2] != (int)OmronFinsDataType.AR.BitCode)
                        throw new Exception(StringResources.Language.NotSupportedDataType);
                    this.arBuffer.SetBool(array, destIndex);
                }
                return new byte[0];
            }
            ushort num1 = (ushort)((uint)command[6] * 256U + (uint)command[7]);
            int num2 = (int)command[3] * 256 + (int)command[4];
            byte[] data = SoftBasic.ArrayRemoveBegin<byte>(command, 8);
            if ((int)command[2] == (int)OmronFinsDataType.DM.WordCode)
                this.dBuffer.SetBytes(data, num2 * 2);
            else if ((int)command[2] == (int)OmronFinsDataType.CIO.WordCode)
                this.cioBuffer.SetBytes(data, num2 * 2);
            else if ((int)command[2] == (int)OmronFinsDataType.WR.WordCode)
                this.wBuffer.SetBytes(data, num2 * 2);
            else if ((int)command[2] == (int)OmronFinsDataType.HR.WordCode)
            {
                this.hBuffer.SetBytes(data, num2 * 2);
            }
            else
            {
                if ((int)command[2] != (int)OmronFinsDataType.AR.WordCode)
                    throw new Exception(StringResources.Language.NotSupportedDataType);
                this.arBuffer.SetBytes(data, num2 * 2);
            }
            return new byte[0];
        }

        /// <inheritdoc />
        protected override void LoadFromBytes(byte[] content)
        {
            if (content.Length < 786432)
                throw new Exception("File is not correct");
            this.dBuffer.SetBytes(content, 0, 0, 131072);
            this.cioBuffer.SetBytes(content, 131072, 0, 131072);
            this.wBuffer.SetBytes(content, 262144, 0, 131072);
            this.hBuffer.SetBytes(content, 393216, 0, 131072);
            this.arBuffer.SetBytes(content, 524288, 0, 131072);
            this.arBuffer.SetBytes(content, 655360, 0, 131072);
        }

        /// <inheritdoc />
        protected override byte[] SaveToBytes()
        {
            byte[] numArray = new byte[786432];
            Array.Copy((Array)this.dBuffer.GetBytes(), 0, (Array)numArray, 0, 131072);
            Array.Copy((Array)this.cioBuffer.GetBytes(), 0, (Array)numArray, 131072, 131072);
            Array.Copy((Array)this.wBuffer.GetBytes(), 0, (Array)numArray, 262144, 131072);
            Array.Copy((Array)this.hBuffer.GetBytes(), 0, (Array)numArray, 393216, 131072);
            Array.Copy((Array)this.arBuffer.GetBytes(), 0, (Array)numArray, 524288, 131072);
            Array.Copy((Array)this.arBuffer.GetBytes(), 0, (Array)numArray, 655360, 131072);
            return numArray;
        }

        /// <inheritdoc cref="M:System.IDisposable.Dispose" />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dBuffer?.Dispose();
                this.cioBuffer?.Dispose();
                this.wBuffer?.Dispose();
                this.hBuffer?.Dispose();
                this.arBuffer?.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("OmronFinsServer[{0}]", (object)this.Port);
    }
}
