// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Fuji.FujiSPHServer
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
using System.Net;
using System.Net.Sockets;

namespace ESTCore.Common.Profinet.Fuji
{
    /// <summary>
    /// <b>[商业授权]</b> 富士的SPH虚拟的PLC，支持M1.0，M3.0，M10.0，I0，Q0的位与字的读写操作。<br />
    /// </summary>
    public class FujiSPHServer : NetworkDataServerBase
    {
        private SoftBuffer m1Buffer;
        private SoftBuffer m3Buffer;
        private SoftBuffer m10Buffer;
        private SoftBuffer iqBuffer;
        private const int DataPoolLength = 65536;

        /// <summary>
        /// 实例化一个基于SPH协议的虚拟的富士PLC对象，可以用来和<see cref="T:ESTCore.Common.Profinet.Fuji.FujiSPHNet" />进行通信测试。
        /// </summary>
        public FujiSPHServer()
        {
            this.m1Buffer = new SoftBuffer(131072);
            this.m3Buffer = new SoftBuffer(131072);
            this.m10Buffer = new SoftBuffer(131072);
            this.iqBuffer = new SoftBuffer(131072);
            this.ByteTransform = (IByteTransform)new RegularByteTransform();
            this.WordLength = (ushort)1;
        }

        /// <inheritdoc />
        protected override byte[] SaveToBytes()
        {
            byte[] numArray = new byte[524288];
            this.m1Buffer.GetBytes().CopyTo((Array)numArray, 0);
            this.m3Buffer.GetBytes().CopyTo((Array)numArray, 131072);
            this.m10Buffer.GetBytes().CopyTo((Array)numArray, 262144);
            this.iqBuffer.GetBytes().CopyTo((Array)numArray, 393216);
            return numArray;
        }

        /// <inheritdoc />
        protected override void LoadFromBytes(byte[] content)
        {
            if (content.Length < 524288)
                throw new Exception("File is not correct");
            this.m1Buffer.SetBytes(content, 0, 131072);
            this.m3Buffer.SetBytes(content, 131072, 131072);
            this.m10Buffer.SetBytes(content, 262144, 131072);
            this.iqBuffer.SetBytes(content, 393216, 131072);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Fuji.FujiSPHNet.Read(System.String,System.UInt16)" />
        [EstMqttApi("ReadByteArray", "")]
        public override OperateResult<byte[]> Read(string address, ushort length)
        {
            OperateResult<FujiSPHAddress> from = FujiSPHAddress.ParseFrom(address);
            if (!from.IsSuccess)
                return from.ConvertFailed<byte[]>();
            switch (from.Content.TypeCode)
            {
                case 1:
                    return OperateResult.CreateSuccessResult<byte[]>(this.iqBuffer.GetBytes(from.Content.AddressStart * 2, (int)length * 2));
                case 2:
                    return OperateResult.CreateSuccessResult<byte[]>(this.m1Buffer.GetBytes(from.Content.AddressStart * 2, (int)length * 2));
                case 4:
                    return OperateResult.CreateSuccessResult<byte[]>(this.m3Buffer.GetBytes(from.Content.AddressStart * 2, (int)length * 2));
                case 8:
                    return OperateResult.CreateSuccessResult<byte[]>(this.m10Buffer.GetBytes(from.Content.AddressStart * 2, (int)length * 2));
                default:
                    return new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType);
            }
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Fuji.FujiSPHNet.Write(System.String,System.Byte[])" />
        [EstMqttApi("WriteByteArray", "")]
        public override OperateResult Write(string address, byte[] value)
        {
            OperateResult<FujiSPHAddress> from = FujiSPHAddress.ParseFrom(address);
            if (!from.IsSuccess)
                return (OperateResult)from.ConvertFailed<byte[]>();
            switch (from.Content.TypeCode)
            {
                case 1:
                    this.iqBuffer.SetBytes(value, from.Content.AddressStart * 2);
                    break;
                case 2:
                    this.m1Buffer.SetBytes(value, from.Content.AddressStart * 2);
                    break;
                case 4:
                    this.m3Buffer.SetBytes(value, from.Content.AddressStart * 2);
                    break;
                case 8:
                    this.m10Buffer.SetBytes(value, from.Content.AddressStart * 2);
                    break;
                default:
                    return (OperateResult)new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType);
            }
            return OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadBool(System.String,System.UInt16)" />
        [EstMqttApi("ReadBoolArray", "")]
        public override OperateResult<bool[]> ReadBool(string address, ushort length)
        {
            OperateResult<FujiSPHAddress> from = FujiSPHAddress.ParseFrom(address);
            if (!from.IsSuccess)
                return from.ConvertFailed<bool[]>();
            int num1 = from.Content.BitIndex + (int)length;
            int num2 = num1 % 16 == 0 ? num1 / 16 : num1 / 16 + 1;
            OperateResult<byte[]> operateResult = this.Read(address, (ushort)num2);
            return !operateResult.IsSuccess ? operateResult.ConvertFailed<bool[]>() : OperateResult.CreateSuccessResult<bool[]>(operateResult.Content.ToBoolArray().SelectMiddle<bool>(from.Content.BitIndex, (int)length));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Write(System.String,System.Boolean[])" />
        [EstMqttApi("WriteBoolArray", "")]
        public override OperateResult Write(string address, bool[] value)
        {
            OperateResult<FujiSPHAddress> from = FujiSPHAddress.ParseFrom(address);
            if (!from.IsSuccess)
                return (OperateResult)from.ConvertFailed<bool[]>();
            switch (from.Content.TypeCode)
            {
                case 1:
                    this.iqBuffer.SetBool(value, from.Content.AddressStart * 16 + from.Content.BitIndex);
                    break;
                case 2:
                    this.m1Buffer.SetBool(value, from.Content.AddressStart * 16 + from.Content.BitIndex);
                    break;
                case 4:
                    this.m3Buffer.SetBool(value, from.Content.AddressStart * 16 + from.Content.BitIndex);
                    break;
                case 8:
                    this.m10Buffer.SetBool(value, from.Content.AddressStart * 16 + from.Content.BitIndex);
                    break;
                default:
                    return new OperateResult(StringResources.Language.NotSupportedDataType);
            }
            return OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc />
        protected override void ThreadPoolLoginAfterClientCheck(Socket socket, IPEndPoint endPoint)
        {
            AppSession session = new AppSession();
            session.IpEndPoint = endPoint;
            session.WorkSocket = socket;
            if (socket.BeginReceiveResult(new AsyncCallback(this.SocketAsyncCallBack), (object)session).IsSuccess)
                this.AddClient(session);
            else
                this.LogNet?.WriteDebug(this.ToString(), string.Format(StringResources.Language.ClientOfflineInfo, (object)endPoint));
        }

        private async void SocketAsyncCallBack(IAsyncResult ar)
        {
            if (!(ar.AsyncState is AppSession session))
                session = (AppSession)null;
            else if (!session.WorkSocket.EndReceiveResult(ar).IsSuccess)
            {
                this.RemoveClient(session);
                session = (AppSession)null;
            }
            else
            {
                OperateResult<byte[]> read1 = await this.ReceiveByMessageAsync(session.WorkSocket, 2000, (INetMessage)new FujiSPHMessage());
                if (!read1.IsSuccess)
                {
                    this.RemoveClient(session);
                    session = (AppSession)null;
                }
                else if (!ESTCore.Common.Authorization.asdniasnfaksndiqwhawfskhfaiw())
                {
                    this.RemoveClient(session);
                    session = (AppSession)null;
                }
                else if (read1.Content[0] != (byte)251 || read1.Content[1] != (byte)128)
                {
                    this.RemoveClient(session);
                    session = (AppSession)null;
                }
                else
                {
                    this.LogNet?.WriteDebug(this.ToString(), string.Format("[{0}] Tcp {1}：{2}", (object)session.IpEndPoint, (object)StringResources.Language.Receive, (object)read1.Content.ToHexString(' ')));
                    byte[] back = this.ReadFromSPBCore(read1.Content);
                    if (back == null)
                    {
                        this.RemoveClient(session);
                        session = (AppSession)null;
                    }
                    else if (!this.Send(session.WorkSocket, back).IsSuccess)
                    {
                        this.RemoveClient(session);
                        session = (AppSession)null;
                    }
                    else
                    {
                        this.LogNet?.WriteDebug(this.ToString(), string.Format("[{0}] Tcp {1}：{2}", (object)session.IpEndPoint, (object)StringResources.Language.Send, (object)back.ToHexString(' ')));
                        session.HeartTime = DateTime.Now;
                        this.RaiseDataReceived((object)session, read1.Content);
                        if (!session.WorkSocket.BeginReceiveResult(new AsyncCallback(this.SocketAsyncCallBack), (object)session).IsSuccess)
                            this.RemoveClient(session);
                        read1 = (OperateResult<byte[]>)null;
                        back = (byte[])null;
                        session = (AppSession)null;
                    }
                }
            }
        }

        private byte[] PackCommand(byte[] cmd, byte err, byte[] data)
        {
            if (data == null)
                data = new byte[0];
            byte[] numArray = new byte[26 + data.Length];
            numArray[0] = (byte)251;
            numArray[1] = (byte)128;
            numArray[2] = (byte)128;
            numArray[3] = (byte)0;
            numArray[4] = err;
            numArray[5] = (byte)123;
            numArray[6] = cmd[6];
            numArray[7] = (byte)0;
            numArray[8] = (byte)17;
            numArray[9] = (byte)0;
            numArray[10] = (byte)0;
            numArray[11] = (byte)0;
            numArray[12] = (byte)0;
            numArray[13] = (byte)0;
            numArray[14] = cmd[14];
            numArray[15] = cmd[15];
            numArray[16] = (byte)0;
            numArray[17] = (byte)1;
            numArray[18] = BitConverter.GetBytes(data.Length + 6)[0];
            numArray[19] = BitConverter.GetBytes(data.Length + 6)[1];
            Array.Copy((Array)cmd, 20, (Array)numArray, 20, 6);
            if ((uint)data.Length > 0U)
                data.CopyTo((Array)numArray, 26);
            return numArray;
        }

        private byte[] ReadFromSPBCore(byte[] receive)
        {
            if (receive.Length < 20)
                return this.PackCommand(receive, (byte)16, (byte[])null);
            if (receive[14] == (byte)0 && receive[15] == (byte)0)
                return this.ReadByCommand(receive);
            return receive[14] == (byte)1 && receive[15] == (byte)0 ? this.WriteByCommand(receive) : this.PackCommand(receive, (byte)32, (byte[])null);
        }

        private byte[] ReadByCommand(byte[] command)
        {
            try
            {
                byte num1 = command[20];
                int num2 = (int)command[23] * 256 * 256 + (int)command[22] * 256 + (int)command[21];
                int num3 = (int)command[25] * 256 + (int)command[24];
                if (num2 + num3 > (int)ushort.MaxValue)
                    return this.PackCommand(command, (byte)69, (byte[])null);
                switch (num1)
                {
                    case 1:
                        return this.PackCommand(command, (byte)0, this.iqBuffer.GetBytes(num2 * 2, num3 * 2));
                    case 2:
                        return this.PackCommand(command, (byte)0, this.m1Buffer.GetBytes(num2 * 2, num3 * 2));
                    case 4:
                        return this.PackCommand(command, (byte)0, this.m3Buffer.GetBytes(num2 * 2, num3 * 2));
                    case 8:
                        return this.PackCommand(command, (byte)0, this.m10Buffer.GetBytes(num2 * 2, num3 * 2));
                    default:
                        return this.PackCommand(command, (byte)64, (byte[])null);
                }
            }
            catch
            {
                return this.PackCommand(command, (byte)164, (byte[])null);
            }
        }

        private byte[] WriteByCommand(byte[] command)
        {
            if (!this.EnableWrite)
                return this.PackCommand(command, (byte)16, (byte[])null);
            try
            {
                byte num1 = command[20];
                int num2 = (int)command[23] * 256 * 256 + (int)command[22] * 256 + (int)command[21];
                int num3 = (int)command[25] * 256 + (int)command[24];
                byte[] data = command.RemoveBegin<byte>(26);
                if (num2 + num3 > (int)ushort.MaxValue)
                    return this.PackCommand(command, (byte)69, (byte[])null);
                if (num3 * 2 != data.Length)
                    return this.PackCommand(command, (byte)69, (byte[])null);
                switch (num1)
                {
                    case 1:
                        this.iqBuffer.SetBytes(data, num2 * 2);
                        return this.PackCommand(command, (byte)0, (byte[])null);
                    case 2:
                        this.m1Buffer.SetBytes(data, num2 * 2);
                        return this.PackCommand(command, (byte)0, (byte[])null);
                    case 4:
                        this.m3Buffer.SetBytes(data, num2 * 2);
                        return this.PackCommand(command, (byte)0, (byte[])null);
                    case 8:
                        this.m10Buffer.SetBytes(data, num2 * 2);
                        return this.PackCommand(command, (byte)0, (byte[])null);
                    default:
                        return this.PackCommand(command, (byte)64, (byte[])null);
                }
            }
            catch
            {
                return this.PackCommand(command, (byte)164, (byte[])null);
            }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.m1Buffer.Dispose();
                this.m3Buffer.Dispose();
                this.m10Buffer.Dispose();
                this.iqBuffer.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("FujiSPHServer[{0}]", (object)this.Port);
    }
}
