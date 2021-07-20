// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.GE.GeSRTPServer
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;
using ESTCore.Common.Core;
using ESTCore.Common.Core.Address;
using ESTCore.Common.Core.IMessage;
using ESTCore.Common.Core.Net;
using ESTCore.Common.LogNet;
using ESTCore.Common.Reflection;

using System;
using System.Net;
using System.Net.Sockets;

namespace ESTCore.Common.Profinet.GE
{
    /// <summary>
    /// <b>[商业授权]</b> Ge的SRTP协议实现的虚拟PLC，支持I,Q,M,T,SA,SB,SC,S,G的位和字节读写，支持AI,AQ,R的字读写操作，支持读取当前时间及程序名称。<br />
    /// <b>[Authorization]</b> Virtual PLC implemented by Ge's SRTP protocol, supports bit and byte read and write of I, Q, M, T, SA, SB, SC, S, G,
    /// supports word read and write operations of AI, AQ, R, and supports reading Current time and program name.
    /// </summary>
    /// <remarks>
    /// 实例化之后，直接调用 <see cref="M:ESTCore.Common.Core.Net.NetworkServerBase.ServerStart(System.Int32)" /> 方法就可以通信及交互，所有的地址都是从1开始的，地址示例：M1,M100, R1，
    /// 具体的用法参考 ESTCore.CommonDemo 相关界面的源代码。
    /// </remarks>
    /// <example>
    /// 地址的示例，参考 <see cref="T:ESTCore.Common.Profinet.GE.GeSRTPNet" /> 相关的示例说明
    /// </example>
    public class GeSRTPServer : NetworkDataServerBase
    {
        private SoftBuffer iBuffer;
        private SoftBuffer qBuffer;
        private SoftBuffer mBuffer;
        private SoftBuffer tBuffer;
        private SoftBuffer saBuffer;
        private SoftBuffer sbBuffer;
        private SoftBuffer scBuffer;
        private SoftBuffer sBuffer;
        private SoftBuffer gBuffer;
        private SoftBuffer aiBuffer;
        private SoftBuffer aqBuffer;
        private SoftBuffer rBuffer;
        private const int DataPoolLength = 65536;

        /// <summary>
        /// 实例化一个默认的对象<br />
        /// Instantiate a default object
        /// </summary>
        public GeSRTPServer()
        {
            this.iBuffer = new SoftBuffer(65536);
            this.qBuffer = new SoftBuffer(65536);
            this.mBuffer = new SoftBuffer(65536);
            this.tBuffer = new SoftBuffer(65536);
            this.saBuffer = new SoftBuffer(65536);
            this.sbBuffer = new SoftBuffer(65536);
            this.scBuffer = new SoftBuffer(65536);
            this.sBuffer = new SoftBuffer(65536);
            this.gBuffer = new SoftBuffer(65536);
            this.aiBuffer = new SoftBuffer(131072);
            this.aqBuffer = new SoftBuffer(131072);
            this.rBuffer = new SoftBuffer(131072);
            this.WordLength = (ushort)2;
            this.ByteTransform = (IByteTransform)new RegularByteTransform();
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.GE.GeSRTPNet.Read(System.String,System.UInt16)" />
        [EstMqttApi("ReadByteArray", "")]
        public override OperateResult<byte[]> Read(string address, ushort length)
        {
            OperateResult<GeSRTPAddress> from = GeSRTPAddress.ParseFrom(address, length, false);
            return !from.IsSuccess ? OperateResult.CreateFailedResult<byte[]>((OperateResult)from) : OperateResult.CreateSuccessResult<byte[]>(this.ReadByCommand(from.Content.DataCode, (ushort)from.Content.AddressStart, from.Content.Length));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.GE.GeSRTPNet.Write(System.String,System.Byte[])" />
        [EstMqttApi("WriteByteArray", "")]
        public override OperateResult Write(string address, byte[] value)
        {
            OperateResult<GeSRTPAddress> from = GeSRTPAddress.ParseFrom(address, (ushort)value.Length, false);
            return !from.IsSuccess ? (OperateResult)OperateResult.CreateFailedResult<byte[]>((OperateResult)from) : this.WriteByCommand(from.Content.DataCode, (ushort)from.Content.AddressStart, from.Content.Length, value);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.GE.GeSRTPNet.ReadBool(System.String,System.UInt16)" />
        [EstMqttApi("ReadBoolArray", "")]
        public override OperateResult<bool[]> ReadBool(string address, ushort length)
        {
            OperateResult<GeSRTPAddress> from = GeSRTPAddress.ParseFrom(address, length, true);
            return !from.IsSuccess ? OperateResult.CreateFailedResult<bool[]>((OperateResult)from) : OperateResult.CreateSuccessResult<bool[]>(this.GetSoftBufferFromDataCode(from.Content.DataCode, out bool _).GetBool(from.Content.AddressStart, (int)length));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.GE.GeSRTPNet.Write(System.String,System.Boolean[])" />
        [EstMqttApi("WriteBoolArray", "")]
        public override OperateResult Write(string address, bool[] value)
        {
            OperateResult<GeSRTPAddress> from = GeSRTPAddress.ParseFrom(address, (ushort)value.Length, true);
            if (!from.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<bool[]>((OperateResult)from);
            this.GetSoftBufferFromDataCode(from.Content.DataCode, out bool _).SetBool(value, from.Content.AddressStart);
            return OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc />
        protected override async void ThreadPoolLoginAfterClientCheck(
          Socket socket,
          IPEndPoint endPoint)
        {
            OperateResult<byte[]> read = await this.ReceiveByMessageAsync(socket, 5000, (INetMessage)new GeSRTPMessage());
            byte[] back;
            OperateResult send;
            AppSession appSession;
            if (!read.IsSuccess)
            {
                Socket socket1 = socket;
                if (socket1 == null)
                {
                    read = (OperateResult<byte[]>)null;
                    back = (byte[])null;
                    send = (OperateResult)null;
                    appSession = (AppSession)null;
                }
                else
                {
                    socket1.Close();
                    read = (OperateResult<byte[]>)null;
                    back = (byte[])null;
                    send = (OperateResult)null;
                    appSession = (AppSession)null;
                }
            }
            else
            {
                back = new byte[56];
                back[0] = (byte)1;
                back[8] = (byte)15;
                send = this.Send(socket, back);
                if (!send.IsSuccess)
                {
                    Socket socket1 = socket;
                    if (socket1 == null)
                    {
                        read = (OperateResult<byte[]>)null;
                        back = (byte[])null;
                        send = (OperateResult)null;
                        appSession = (AppSession)null;
                    }
                    else
                    {
                        socket1.Close();
                        read = (OperateResult<byte[]>)null;
                        back = (byte[])null;
                        send = (OperateResult)null;
                        appSession = (AppSession)null;
                    }
                }
                else
                {
                    appSession = new AppSession();
                    appSession.IpEndPoint = endPoint;
                    appSession.WorkSocket = socket;
                    try
                    {
                        socket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, new AsyncCallback(this.SocketAsyncCallBack), (object)appSession);
                        this.AddClient(appSession);
                        read = (OperateResult<byte[]>)null;
                        back = (byte[])null;
                        send = (OperateResult)null;
                        appSession = (AppSession)null;
                    }
                    catch
                    {
                        socket.Close();
                        ILogNet logNet = this.LogNet;
                        if (logNet == null)
                        {
                            read = (OperateResult<byte[]>)null;
                            back = (byte[])null;
                            send = (OperateResult)null;
                            appSession = (AppSession)null;
                        }
                        else
                        {
                            logNet.WriteDebug(this.ToString(), string.Format(StringResources.Language.ClientOfflineInfo, (object)endPoint));
                            read = (OperateResult<byte[]>)null;
                            back = (byte[])null;
                            send = (OperateResult)null;
                            appSession = (AppSession)null;
                        }
                    }
                }
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
                    OperateResult<byte[]> read1 = await this.ReceiveByMessageAsync(session.WorkSocket, 5000, (INetMessage)new GeSRTPMessage());
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
                    byte[] receive = read1.Content;
                    byte[] back = (byte[])null;
                    back = receive[42] != (byte)4 ? (receive[42] != (byte)1 ? (receive[42] != (byte)37 ? (receive[50] != (byte)7 ? (byte[])null : this.WriteByCommand(receive)) : this.ReadDateTimeByCommand(receive)) : this.ReadProgramNameByCommand(receive)) : this.ReadByCommand(receive);
                    if (back == null)
                    {
                        this.RemoveClient(session);
                        session = (AppSession)null;
                        return;
                    }
                    session.WorkSocket.Send(back);
                    this.LogNet?.WriteDebug(this.ToString(), string.Format("[{0}] Tcp {1}：{2}", (object)session.IpEndPoint, (object)StringResources.Language.Send, (object)back.ToHexString(' ')));
                    session.HeartTime = DateTime.Now;
                    this.RaiseDataReceived((object)session, receive);
                    session.WorkSocket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, new AsyncCallback(this.SocketAsyncCallBack), (object)session);
                    read1 = (OperateResult<byte[]>)null;
                    receive = (byte[])null;
                    back = (byte[])null;
                }
                catch
                {
                    this.RemoveClient(session);
                }
                session = (AppSession)null;
            }
        }

        private SoftBuffer GetSoftBufferFromDataCode(byte code, out bool isBit)
        {
            switch (code)
            {
                case 8:
                    isBit = false;
                    return this.rBuffer;
                case 10:
                    isBit = false;
                    return this.aiBuffer;
                case 12:
                    isBit = false;
                    return this.aqBuffer;
                case 16:
                    isBit = false;
                    return this.iBuffer;
                case 20:
                    isBit = false;
                    return this.tBuffer;
                case 22:
                    isBit = false;
                    return this.mBuffer;
                case 24:
                    isBit = false;
                    return this.saBuffer;
                case 26:
                    isBit = false;
                    return this.sbBuffer;
                case 28:
                    isBit = false;
                    return this.scBuffer;
                case 30:
                    isBit = false;
                    return this.tBuffer;
                case 56:
                    isBit = false;
                    return this.gBuffer;
                case 66:
                    isBit = false;
                    return this.qBuffer;
                case 70:
                    isBit = true;
                    return this.iBuffer;
                case 72:
                    isBit = true;
                    return this.qBuffer;
                case 74:
                    isBit = true;
                    return this.tBuffer;
                case 76:
                    isBit = true;
                    return this.mBuffer;
                case 78:
                    isBit = true;
                    return this.saBuffer;
                case 80:
                    isBit = true;
                    return this.sbBuffer;
                case 82:
                    isBit = true;
                    return this.scBuffer;
                case 84:
                    isBit = true;
                    return this.tBuffer;
                case 86:
                    isBit = true;
                    return this.gBuffer;
                default:
                    isBit = false;
                    return (SoftBuffer)null;
            }
        }

        private byte[] ReadByCommand(byte dataCode, ushort address, ushort length)
        {
            bool isBit;
            SoftBuffer bufferFromDataCode = this.GetSoftBufferFromDataCode(dataCode, out isBit);
            if (bufferFromDataCode == null)
                return (byte[])null;
            if (isBit)
            {
                int newStart;
                ushort byteLength;
                EstHelper.CalculateStartBitIndexAndLength((int)address, length, out newStart, out byteLength, out int _);
                return bufferFromDataCode.GetBytes(newStart / 8, (int)byteLength);
            }
            return dataCode == (byte)10 || dataCode == (byte)12 || dataCode == (byte)8 ? bufferFromDataCode.GetBytes((int)address * 2, (int)length * 2) : bufferFromDataCode.GetBytes((int)address, (int)length);
        }

        private byte[] ReadByCommand(byte[] command)
        {
            byte[] numArray1 = this.ReadByCommand(command[43], BitConverter.ToUInt16(command, 44), BitConverter.ToUInt16(command, 46));
            if (numArray1 == null)
                return (byte[])null;
            if (numArray1.Length < 7)
            {
                byte[] hexBytes = "\r\n03 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00\r\n00 01 00 00 00 00 00 00 00 00 00 00 00 00 06 d4\r\n00 0e 00 00 00 60 01 a0 01 01 00 00 00 00 00 00\r\n00 00 ff 02 03 00 5c 01".ToHexBytes();
                numArray1.CopyTo((Array)hexBytes, 44);
                command.SelectMiddle<byte>(2, 2).CopyTo((Array)hexBytes, 2);
                return hexBytes;
            }
            byte[] numArray2 = new byte[56 + numArray1.Length];
            "03 00 03 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 06 94\r\n\t\t\t\t00 0e 00 00 00 60 01 a0 00 00 0c 00 00 18 00 00 01 01 ff 02 03 00 5c 01".ToHexBytes().CopyTo((Array)numArray2, 0);
            command.SelectMiddle<byte>(2, 2).CopyTo((Array)numArray2, 2);
            numArray1.CopyTo((Array)numArray2, 56);
            BitConverter.GetBytes((ushort)numArray1.Length).CopyTo((Array)numArray2, 4);
            return numArray2;
        }

        private OperateResult WriteByCommand(
          byte dataCode,
          ushort address,
          ushort length,
          byte[] value)
        {
            bool isBit;
            SoftBuffer bufferFromDataCode = this.GetSoftBufferFromDataCode(dataCode, out isBit);
            if (bufferFromDataCode == null)
                return new OperateResult(StringResources.Language.NotSupportedDataType);
            if (isBit)
            {
                EstHelper.CalculateStartBitIndexAndLength((int)address, length, out int _, out ushort _, out int _);
                bufferFromDataCode.SetBool(value.ToBoolArray().SelectMiddle<bool>((int)address % 8, (int)length), (int)address);
            }
            else if (dataCode == (byte)10 || dataCode == (byte)12 || dataCode == (byte)8)
            {
                if (value.Length % 2 == 1)
                    return new OperateResult(StringResources.Language.GeSRTPWriteLengthMustBeEven);
                bufferFromDataCode.SetBytes(value, (int)address * 2);
            }
            else
                bufferFromDataCode.SetBytes(value, (int)address);
            return OperateResult.CreateSuccessResult();
        }

        private byte[] WriteByCommand(byte[] command)
        {
            if (!this.EnableWrite || !this.WriteByCommand(command[51], BitConverter.ToUInt16(command, 52), BitConverter.ToUInt16(command, 54), command.RemoveBegin<byte>(56)).IsSuccess)
                return (byte[])null;
            byte[] hexBytes = "03 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00\r\n00 02 00 00 00 00 00 00 00 00 00 00 00 00 09 d4\r\n00 0e 00 00 00 60 01 a0 01 01 00 00 00 00 00 00\r\n00 00 ff 02 03 00 5c 01".ToHexBytes();
            command.SelectMiddle<byte>(2, 2).CopyTo((Array)hexBytes, 2);
            return hexBytes;
        }

        private byte[] ReadDateTimeByCommand(byte[] command)
        {
            byte[] hexBytes = "03 00 03 00 07 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 06 94\r\n\t\t\t\t00 0e 00 00 00 60 01 a0 00 00 0c 00 00 18 00 00 01 01 ff 02 03 00 5c 01 00 00 00 00 00 00 03".ToHexBytes();
            DateTime now = DateTime.Now;
            now.Second.ToString("D2").ToHexBytes().CopyTo((Array)hexBytes, 56);
            now.Minute.ToString("D2").ToHexBytes().CopyTo((Array)hexBytes, 57);
            now.Hour.ToString("D2").ToHexBytes().CopyTo((Array)hexBytes, 58);
            now.Day.ToString("D2").ToHexBytes().CopyTo((Array)hexBytes, 59);
            now.Month.ToString("D2").ToHexBytes().CopyTo((Array)hexBytes, 60);
            (now.Year - 2000).ToString("D2").ToHexBytes().CopyTo((Array)hexBytes, 61);
            command.SelectMiddle<byte>(2, 2).CopyTo((Array)hexBytes, 2);
            return hexBytes;
        }

        private byte[] ReadProgramNameByCommand(byte[] command)
        {
            byte[] hexBytes = "\r\n03 00 07 00 2a 00 00 00 00 00 00 00 00 00 00 00 \r\n00 01 00 00 00 00 00 00 00 00 00 00 00 00 06 94 \r\n00 0e 00 00 00 62 01 a0 00 00 2a 00 00 18 00 00 \r\n01 01 ff 02 03 00 5c 01 00 00 00 00 00 00 00 00 \r\n01 00 00 00 00 00 00 00 00 00 50 41 43 34 30 30 \r\n00 00 00 00 00 00 00 00 00 00 03 00 01 50 05 18 \r\n01 21".ToHexBytes();
            command.SelectMiddle<byte>(2, 2).CopyTo((Array)hexBytes, 2);
            return hexBytes;
        }

        /// <inheritdoc />
        protected override void LoadFromBytes(byte[] content)
        {
            if (content.Length < 983040)
                throw new Exception("File is not correct");
            this.iBuffer.SetBytes(content, 0, 0, 65536);
            this.qBuffer.SetBytes(content, 65536, 0, 65536);
            this.mBuffer.SetBytes(content, 131072, 0, 65536);
            this.tBuffer.SetBytes(content, 196608, 0, 65536);
            this.saBuffer.SetBytes(content, 262144, 0, 65536);
            this.sbBuffer.SetBytes(content, 327680, 0, 65536);
            this.scBuffer.SetBytes(content, 393216, 0, 65536);
            this.sBuffer.SetBytes(content, 458752, 0, 65536);
            this.gBuffer.SetBytes(content, 524288, 0, 65536);
            this.aiBuffer.SetBytes(content, 589824, 0, 131072);
            this.aqBuffer.SetBytes(content, 720896, 0, 131072);
            this.rBuffer.SetBytes(content, 851968, 0, 131072);
        }

        /// <inheritdoc />
        protected override byte[] SaveToBytes()
        {
            byte[] numArray = new byte[983040];
            Array.Copy((Array)this.iBuffer.GetBytes(), 0, (Array)numArray, 0, 65536);
            Array.Copy((Array)this.qBuffer.GetBytes(), 0, (Array)numArray, 65536, 65536);
            Array.Copy((Array)this.mBuffer.GetBytes(), 0, (Array)numArray, 131072, 65536);
            Array.Copy((Array)this.tBuffer.GetBytes(), 0, (Array)numArray, 196608, 65536);
            Array.Copy((Array)this.saBuffer.GetBytes(), 0, (Array)numArray, 262144, 65536);
            Array.Copy((Array)this.sbBuffer.GetBytes(), 0, (Array)numArray, 327680, 65536);
            Array.Copy((Array)this.scBuffer.GetBytes(), 0, (Array)numArray, 393216, 65536);
            Array.Copy((Array)this.sBuffer.GetBytes(), 0, (Array)numArray, 458752, 65536);
            Array.Copy((Array)this.gBuffer.GetBytes(), 0, (Array)numArray, 524288, 65536);
            Array.Copy((Array)this.aiBuffer.GetBytes(), 0, (Array)numArray, 589824, 131072);
            Array.Copy((Array)this.aqBuffer.GetBytes(), 0, (Array)numArray, 720896, 131072);
            Array.Copy((Array)this.rBuffer.GetBytes(), 0, (Array)numArray, 851968, 131072);
            return numArray;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.iBuffer.Dispose();
                this.qBuffer.Dispose();
                this.mBuffer.Dispose();
                this.tBuffer.Dispose();
                this.saBuffer.Dispose();
                this.sbBuffer.Dispose();
                this.scBuffer.Dispose();
                this.sBuffer.Dispose();
                this.gBuffer.Dispose();
                this.aiBuffer.Dispose();
                this.aqBuffer.Dispose();
                this.rBuffer.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("GeSRTPServer[{0}]", (object)this.Port);
    }
}
