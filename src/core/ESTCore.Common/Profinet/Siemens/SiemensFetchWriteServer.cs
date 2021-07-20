// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Siemens.SiemensFetchWriteServer
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

namespace ESTCore.Common.Profinet.Siemens
{
    /// <summary>
    /// <b>[商业授权]</b> 西门子的Fetch/Write协议的虚拟PLC，可以用来调试通讯，也可以实现一个虚拟的PLC功能，从而开发一套带虚拟环境的上位机系统，可以用来演示，测试。<br />
    /// <b>[Authorization]</b> The virtual PLC of Siemens Fetch/Write protocol can be used for debugging communication, and can also realize a virtual PLC function, so as to develop a set of upper computer system with virtual environment, which can be used for demonstration and testing.
    /// </summary>
    /// <remarks>
    /// 本虚拟服务器的使用需要企业商业授权，否则只能运行24小时。本协议实现的虚拟PLC服务器，主要支持I,Q,M,DB块的数据读写操作，例如 M100, DB1.100，服务器端也可以对位进行读写操作，例如M100.1，DB1.100.2；
    /// 但是不支持连接的远程客户端对位进行操作。
    /// </remarks>
    /// <example>
    /// 地址支持的列表如下：
    /// <list type="table">
    ///   <listheader>
    ///     <term>地址名称</term>
    ///     <term>地址代号</term>
    ///     <term>示例</term>
    ///     <term>地址进制</term>
    ///     <term>字操作</term>
    ///     <term>位操作</term>
    ///     <term>备注</term>
    ///   </listheader>
    ///   <item>
    ///     <term>中间寄存器</term>
    ///     <term>M</term>
    ///     <term>M100,M200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>输入寄存器</term>
    ///     <term>I</term>
    ///     <term>I100,I200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>输出寄存器</term>
    ///     <term>Q</term>
    ///     <term>Q100,Q200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>DB块寄存器</term>
    ///     <term>DB</term>
    ///     <term>DB1.100,DB1.200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>V寄存器</term>
    ///     <term>V</term>
    ///     <term>V100,V200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term>V寄存器本质就是DB块1</term>
    ///   </item>
    /// </list>
    /// 本虚拟的PLC共有4个DB块，DB1.X, DB2.X, DB3.X, 和其他DB块。对于远程客户端的读写长度，暂时没有限制。
    /// </example>
    public class SiemensFetchWriteServer : NetworkDataServerBase
    {
        private SoftBuffer inputBuffer;
        private SoftBuffer outputBuffer;
        private SoftBuffer memeryBuffer;
        private SoftBuffer counterBuffer;
        private SoftBuffer timerBuffer;
        private SoftBuffer db1BlockBuffer;
        private SoftBuffer db2BlockBuffer;
        private SoftBuffer db3BlockBuffer;
        private SoftBuffer dbOtherBlockBuffer;
        private const int DataPoolLength = 65536;

        /// <summary>
        /// 实例化一个S7协议的服务器，支持I，Q，M，DB1.X, DB2.X, DB3.X 数据区块的读写操作<br />
        /// Instantiate a server with S7 protocol, support I, Q, M, DB1.X data block read and write operations
        /// </summary>
        public SiemensFetchWriteServer()
        {
            this.inputBuffer = new SoftBuffer(65536);
            this.outputBuffer = new SoftBuffer(65536);
            this.memeryBuffer = new SoftBuffer(65536);
            this.db1BlockBuffer = new SoftBuffer(65536);
            this.db2BlockBuffer = new SoftBuffer(65536);
            this.db3BlockBuffer = new SoftBuffer(65536);
            this.dbOtherBlockBuffer = new SoftBuffer(65536);
            this.counterBuffer = new SoftBuffer(65536);
            this.timerBuffer = new SoftBuffer(65536);
            this.WordLength = (ushort)2;
            this.ByteTransform = (IByteTransform)new ReverseBytesTransform();
        }

        private OperateResult<SoftBuffer> GetDataAreaFromS7Address(
          S7AddressData s7Address)
        {
            switch (s7Address.DataCode)
            {
                case 129:
                    return OperateResult.CreateSuccessResult<SoftBuffer>(this.inputBuffer);
                case 130:
                    return OperateResult.CreateSuccessResult<SoftBuffer>(this.outputBuffer);
                case 131:
                    return OperateResult.CreateSuccessResult<SoftBuffer>(this.memeryBuffer);
                case 132:
                    if (s7Address.DbBlock == (ushort)1)
                        return OperateResult.CreateSuccessResult<SoftBuffer>(this.db1BlockBuffer);
                    if (s7Address.DbBlock == (ushort)2)
                        return OperateResult.CreateSuccessResult<SoftBuffer>(this.db2BlockBuffer);
                    return s7Address.DbBlock == (ushort)3 ? OperateResult.CreateSuccessResult<SoftBuffer>(this.db3BlockBuffer) : OperateResult.CreateSuccessResult<SoftBuffer>(this.dbOtherBlockBuffer);
                default:
                    return new OperateResult<SoftBuffer>(StringResources.Language.NotSupportedDataType);
            }
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Siemens.SiemensS7Net.Read(System.String,System.UInt16)" />
        [EstMqttApi("ReadByteArray", "")]
        public override OperateResult<byte[]> Read(string address, ushort length)
        {
            OperateResult<S7AddressData> from = S7AddressData.ParseFrom(address, length);
            if (!from.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)from);
            OperateResult<SoftBuffer> areaFromS7Address = this.GetDataAreaFromS7Address(from.Content);
            return !areaFromS7Address.IsSuccess ? OperateResult.CreateFailedResult<byte[]>((OperateResult)areaFromS7Address) : OperateResult.CreateSuccessResult<byte[]>(areaFromS7Address.Content.GetBytes(from.Content.AddressStart / 8, (int)length));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Siemens.SiemensS7Net.Write(System.String,System.Byte[])" />
        [EstMqttApi("WriteByteArray", "")]
        public override OperateResult Write(string address, byte[] value)
        {
            OperateResult<S7AddressData> from = S7AddressData.ParseFrom(address);
            if (!from.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<byte[]>((OperateResult)from);
            OperateResult<SoftBuffer> areaFromS7Address = this.GetDataAreaFromS7Address(from.Content);
            if (!areaFromS7Address.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<byte[]>((OperateResult)areaFromS7Address);
            areaFromS7Address.Content.SetBytes(value, from.Content.AddressStart / 8);
            return OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Siemens.SiemensS7Net.ReadByte(System.String)" />
        [EstMqttApi("ReadByte", "")]
        public OperateResult<byte> ReadByte(string address) => ByteTransformHelper.GetResultFromArray<byte>(this.Read(address, (ushort)1));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Siemens.SiemensS7Net.Write(System.String,System.Byte)" />
        [EstMqttApi("WriteByte", "")]
        public OperateResult Write(string address, byte value) => this.Write(address, new byte[1]
        {
      value
        });

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Siemens.SiemensS7Net.ReadBool(System.String)" />
        [EstMqttApi("ReadBool", "")]
        public override OperateResult<bool> ReadBool(string address)
        {
            OperateResult<S7AddressData> from = S7AddressData.ParseFrom(address);
            if (!from.IsSuccess)
                return OperateResult.CreateFailedResult<bool>((OperateResult)from);
            OperateResult<SoftBuffer> areaFromS7Address = this.GetDataAreaFromS7Address(from.Content);
            return !areaFromS7Address.IsSuccess ? OperateResult.CreateFailedResult<bool>((OperateResult)areaFromS7Address) : OperateResult.CreateSuccessResult<bool>(areaFromS7Address.Content.GetBool(from.Content.AddressStart));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Siemens.SiemensS7Net.Write(System.String,System.Boolean)" />
        [EstMqttApi("WriteBool", "")]
        public override OperateResult Write(string address, bool value)
        {
            OperateResult<S7AddressData> from = S7AddressData.ParseFrom(address);
            if (!from.IsSuccess)
                return (OperateResult)from;
            OperateResult<SoftBuffer> areaFromS7Address = this.GetDataAreaFromS7Address(from.Content);
            if (!areaFromS7Address.IsSuccess)
                return (OperateResult)areaFromS7Address;
            areaFromS7Address.Content.SetBool(value, from.Content.AddressStart);
            return OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc />
        protected override void ThreadPoolLoginAfterClientCheck(Socket socket, IPEndPoint endPoint)
        {
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
                    OperateResult<byte[]> read1 = await this.ReceiveByMessageAsync(session.WorkSocket, 5000, (INetMessage)new FetchWriteMessage());
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
                    if (receive[5] == (byte)3)
                        back = this.WriteByMessage(receive);
                    else if (receive[5] == (byte)5)
                    {
                        back = this.ReadByMessage(receive);
                    }
                    else
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

        private SoftBuffer GetBufferFromCommand(byte[] command)
        {
            if (command[8] == (byte)2)
                return this.memeryBuffer;
            if (command[8] == (byte)3)
                return this.inputBuffer;
            if (command[8] == (byte)4)
                return this.outputBuffer;
            if (command[8] == (byte)1)
            {
                if (command[9] == (byte)1)
                    return this.db1BlockBuffer;
                if (command[9] == (byte)2)
                    return this.db2BlockBuffer;
                return command[9] == (byte)3 ? this.db3BlockBuffer : this.dbOtherBlockBuffer;
            }
            if (command[8] == (byte)6)
                return this.counterBuffer;
            return command[8] == (byte)7 ? this.timerBuffer : (SoftBuffer)null;
        }

        private byte[] ReadByMessage(byte[] command)
        {
            SoftBuffer bufferFromCommand = this.GetBufferFromCommand(command);
            int index = (int)command[10] * 256 + (int)command[11];
            int length = (int)command[12] * 256 + (int)command[13];
            if (bufferFromCommand == null)
                return this.PackCommandResponse((byte)6, (byte)1, (byte[])null);
            return command[8] == (byte)1 || command[8] == (byte)6 || command[8] == (byte)7 ? this.PackCommandResponse((byte)6, (byte)0, bufferFromCommand.GetBytes(index, length * 2)) : this.PackCommandResponse((byte)6, (byte)0, bufferFromCommand.GetBytes(index, length));
        }

        private byte[] WriteByMessage(byte[] command)
        {
            if (!this.EnableWrite)
                return this.PackCommandResponse((byte)4, (byte)1, (byte[])null);
            SoftBuffer bufferFromCommand = this.GetBufferFromCommand(command);
            int destIndex = (int)command[10] * 256 + (int)command[11];
            int num = (int)command[12] * 256 + (int)command[13];
            if (bufferFromCommand == null)
                return this.PackCommandResponse((byte)4, (byte)1, (byte[])null);
            if (command[8] == (byte)1 || command[8] == (byte)6 || command[8] == (byte)7)
            {
                if (num != (command.Length - 16) / 2)
                    return this.PackCommandResponse((byte)4, (byte)1, (byte[])null);
                bufferFromCommand.SetBytes(command.RemoveBegin<byte>(16), destIndex);
                return this.PackCommandResponse((byte)4, (byte)0, (byte[])null);
            }
            if (num != command.Length - 16)
                return this.PackCommandResponse((byte)4, (byte)1, (byte[])null);
            bufferFromCommand.SetBytes(command.RemoveBegin<byte>(16), destIndex);
            return this.PackCommandResponse((byte)4, (byte)0, (byte[])null);
        }

        private byte[] PackCommandResponse(byte opCode, byte err, byte[] data)
        {
            if (data == null)
                data = new byte[0];
            byte[] numArray = new byte[16 + data.Length];
            numArray[0] = (byte)83;
            numArray[1] = (byte)53;
            numArray[2] = (byte)16;
            numArray[3] = (byte)1;
            numArray[4] = (byte)3;
            numArray[5] = opCode;
            numArray[6] = (byte)15;
            numArray[7] = (byte)3;
            numArray[8] = err;
            numArray[9] = byte.MaxValue;
            numArray[10] = (byte)7;
            if ((uint)data.Length > 0U)
                data.CopyTo((Array)numArray, 16);
            return numArray;
        }

        /// <inheritdoc />
        protected override void LoadFromBytes(byte[] content)
        {
            if (content.Length < 589824)
                throw new Exception("File is not correct");
            this.inputBuffer.SetBytes(content, 0, 0, 65536);
            this.outputBuffer.SetBytes(content, 65536, 0, 65536);
            this.memeryBuffer.SetBytes(content, 131072, 0, 65536);
            this.db1BlockBuffer.SetBytes(content, 196608, 0, 65536);
            this.db2BlockBuffer.SetBytes(content, 262144, 0, 65536);
            this.db3BlockBuffer.SetBytes(content, 327680, 0, 65536);
            this.dbOtherBlockBuffer.SetBytes(content, 393216, 0, 65536);
            this.counterBuffer.SetBytes(content, 458752, 0, 65536);
            this.timerBuffer.SetBytes(content, 524288, 0, 65536);
        }

        /// <inheritdoc />
        protected override byte[] SaveToBytes()
        {
            byte[] numArray = new byte[589824];
            Array.Copy((Array)this.inputBuffer.GetBytes(), 0, (Array)numArray, 0, 65536);
            Array.Copy((Array)this.outputBuffer.GetBytes(), 0, (Array)numArray, 65536, 65536);
            Array.Copy((Array)this.memeryBuffer.GetBytes(), 0, (Array)numArray, 131072, 65536);
            Array.Copy((Array)this.db1BlockBuffer.GetBytes(), 0, (Array)numArray, 196608, 65536);
            Array.Copy((Array)this.db2BlockBuffer.GetBytes(), 0, (Array)numArray, 262144, 65536);
            Array.Copy((Array)this.db3BlockBuffer.GetBytes(), 0, (Array)numArray, 327680, 65536);
            Array.Copy((Array)this.dbOtherBlockBuffer.GetBytes(), 0, (Array)numArray, 393216, 65536);
            Array.Copy((Array)this.counterBuffer.GetBytes(), 0, (Array)numArray, 458752, 65536);
            Array.Copy((Array)this.timerBuffer.GetBytes(), 0, (Array)numArray, 524288, 65536);
            return numArray;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.inputBuffer?.Dispose();
                this.outputBuffer?.Dispose();
                this.memeryBuffer?.Dispose();
                this.db1BlockBuffer?.Dispose();
                this.db2BlockBuffer?.Dispose();
                this.db3BlockBuffer?.Dispose();
                this.dbOtherBlockBuffer?.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("SiemensFetchWriteServer[{0}]", (object)this.Port);
    }
}
