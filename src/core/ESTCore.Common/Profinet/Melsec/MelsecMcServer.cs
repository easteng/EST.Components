// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Melsec.MelsecMcServer
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
using System.Text;

namespace ESTCore.Common.Profinet.Melsec
{
    /// <summary>
    /// <b>[商业授权]</b> 三菱MC协议的虚拟服务器，支持M,X,Y,D,W的数据池读写操作，支持二进制及ASCII格式进行读写操作，需要在实例化的时候指定。<br />
    /// <b>[Authorization]</b> The Mitsubishi MC protocol virtual server supports M, X, Y, D, W data pool read and write operations,
    /// and supports binary and ASCII format read and write operations, which need to be specified during instantiation.
    /// </summary>
    /// <remarks>
    /// 本三菱的虚拟PLC仅限商业授权用户使用，感谢支持。
    /// 如果你没有可以测试的三菱PLC，想要测试自己开发的上位机软件，或是想要在本机实现虚拟PLC，然后进行IO的输入输出练习，都可以使用本类来实现，先来说明下地址信息
    /// <br />
    /// 地址的输入的格式说明如下：
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
    ///     <term>内部继电器</term>
    ///     <term>M</term>
    ///     <term>M100,M200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>输入继电器</term>
    ///     <term>X</term>
    ///     <term>X100,X1A0</term>
    ///     <term>16</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>输出继电器</term>
    ///     <term>Y</term>
    ///     <term>Y100,Y1A0</term>
    ///     <term>16</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>数据寄存器</term>
    ///     <term>D</term>
    ///     <term>D1000,D2000</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>×</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>链接寄存器</term>
    ///     <term>W</term>
    ///     <term>W100,W1A0</term>
    ///     <term>16</term>
    ///     <term>√</term>
    ///     <term>×</term>
    ///     <term></term>
    ///   </item>
    /// </list>
    /// </remarks>
    public class MelsecMcServer : NetworkDataServerBase
    {
        private SoftBuffer xBuffer;
        private SoftBuffer yBuffer;
        private SoftBuffer mBuffer;
        private SoftBuffer dBuffer;
        private SoftBuffer wBuffer;
        private const int DataPoolLength = 65536;
        private bool isBinary = true;

        /// <summary>
        /// 实例化一个默认参数的mc协议的服务器<br />
        /// Instantiate a mc protocol server with default parameters
        /// </summary>
        /// <param name="isBinary">是否是二进制，默认是二进制，否则是ASCII格式</param>
        public MelsecMcServer(bool isBinary = true)
        {
            this.xBuffer = new SoftBuffer(65536);
            this.yBuffer = new SoftBuffer(65536);
            this.mBuffer = new SoftBuffer(65536);
            this.dBuffer = new SoftBuffer(131072);
            this.wBuffer = new SoftBuffer(131072);
            this.WordLength = (ushort)1;
            this.ByteTransform = (IByteTransform)new RegularByteTransform();
            this.isBinary = isBinary;
        }

        /// <inheritdoc />
        [EstMqttApi("ReadByteArray", "")]
        public override OperateResult<byte[]> Read(string address, ushort length)
        {
            OperateResult<McAddressData> melsecFrom = McAddressData.ParseMelsecFrom(address, length);
            if (!melsecFrom.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)melsecFrom);
            if ((int)melsecFrom.Content.McDataType.DataCode == (int)MelsecMcDataType.M.DataCode)
                return OperateResult.CreateSuccessResult<byte[]>(SoftBasic.BoolArrayToByte(((IEnumerable<byte>)this.mBuffer.GetBytes(melsecFrom.Content.AddressStart, (int)length * 16)).Select<byte, bool>((Func<byte, bool>)(m => m > (byte)0)).ToArray<bool>()));
            if ((int)melsecFrom.Content.McDataType.DataCode == (int)MelsecMcDataType.X.DataCode)
                return OperateResult.CreateSuccessResult<byte[]>(SoftBasic.BoolArrayToByte(((IEnumerable<byte>)this.xBuffer.GetBytes(melsecFrom.Content.AddressStart, (int)length * 16)).Select<byte, bool>((Func<byte, bool>)(m => m > (byte)0)).ToArray<bool>()));
            if ((int)melsecFrom.Content.McDataType.DataCode == (int)MelsecMcDataType.Y.DataCode)
                return OperateResult.CreateSuccessResult<byte[]>(SoftBasic.BoolArrayToByte(((IEnumerable<byte>)this.yBuffer.GetBytes(melsecFrom.Content.AddressStart, (int)length * 16)).Select<byte, bool>((Func<byte, bool>)(m => m > (byte)0)).ToArray<bool>()));
            if ((int)melsecFrom.Content.McDataType.DataCode == (int)MelsecMcDataType.D.DataCode)
                return OperateResult.CreateSuccessResult<byte[]>(this.dBuffer.GetBytes(melsecFrom.Content.AddressStart * 2, (int)length * 2));
            return (int)melsecFrom.Content.McDataType.DataCode == (int)MelsecMcDataType.W.DataCode ? OperateResult.CreateSuccessResult<byte[]>(this.wBuffer.GetBytes(melsecFrom.Content.AddressStart * 2, (int)length * 2)) : new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType);
        }

        /// <inheritdoc />
        [EstMqttApi("WriteByteArray", "")]
        public override OperateResult Write(string address, byte[] value)
        {
            OperateResult<McAddressData> melsecFrom = McAddressData.ParseMelsecFrom(address, (ushort)0);
            if (!melsecFrom.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<byte[]>((OperateResult)melsecFrom);
            if ((int)melsecFrom.Content.McDataType.DataCode == (int)MelsecMcDataType.M.DataCode)
            {
                this.mBuffer.SetBytes(((IEnumerable<bool>)SoftBasic.ByteToBoolArray(value)).Select<bool, byte>((Func<bool, byte>)(m => !m ? (byte)0 : (byte)1)).ToArray<byte>(), melsecFrom.Content.AddressStart);
                return OperateResult.CreateSuccessResult();
            }
            if ((int)melsecFrom.Content.McDataType.DataCode == (int)MelsecMcDataType.X.DataCode)
            {
                this.xBuffer.SetBytes(((IEnumerable<bool>)SoftBasic.ByteToBoolArray(value)).Select<bool, byte>((Func<bool, byte>)(m => !m ? (byte)0 : (byte)1)).ToArray<byte>(), melsecFrom.Content.AddressStart);
                return OperateResult.CreateSuccessResult();
            }
            if ((int)melsecFrom.Content.McDataType.DataCode == (int)MelsecMcDataType.Y.DataCode)
            {
                this.yBuffer.SetBytes(((IEnumerable<bool>)SoftBasic.ByteToBoolArray(value)).Select<bool, byte>((Func<bool, byte>)(m => !m ? (byte)0 : (byte)1)).ToArray<byte>(), melsecFrom.Content.AddressStart);
                return OperateResult.CreateSuccessResult();
            }
            if ((int)melsecFrom.Content.McDataType.DataCode == (int)MelsecMcDataType.D.DataCode)
            {
                this.dBuffer.SetBytes(value, melsecFrom.Content.AddressStart * 2);
                return OperateResult.CreateSuccessResult();
            }
            if ((int)melsecFrom.Content.McDataType.DataCode != (int)MelsecMcDataType.W.DataCode)
                return (OperateResult)new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType);
            this.wBuffer.SetBytes(value, melsecFrom.Content.AddressStart * 2);
            return OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc />
        [EstMqttApi("ReadBoolArray", "")]
        public override OperateResult<bool[]> ReadBool(string address, ushort length)
        {
            OperateResult<McAddressData> melsecFrom = McAddressData.ParseMelsecFrom(address, (ushort)0);
            if (!melsecFrom.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)melsecFrom);
            if (melsecFrom.Content.McDataType.DataType == (byte)0)
                return new OperateResult<bool[]>(StringResources.Language.MelsecCurrentTypeNotSupportedWordOperate);
            if ((int)melsecFrom.Content.McDataType.DataCode == (int)MelsecMcDataType.M.DataCode)
                return OperateResult.CreateSuccessResult<bool[]>(((IEnumerable<byte>)this.mBuffer.GetBytes(melsecFrom.Content.AddressStart, (int)length)).Select<byte, bool>((Func<byte, bool>)(m => m > (byte)0)).ToArray<bool>());
            if ((int)melsecFrom.Content.McDataType.DataCode == (int)MelsecMcDataType.X.DataCode)
                return OperateResult.CreateSuccessResult<bool[]>(((IEnumerable<byte>)this.xBuffer.GetBytes(melsecFrom.Content.AddressStart, (int)length)).Select<byte, bool>((Func<byte, bool>)(m => m > (byte)0)).ToArray<bool>());
            return (int)melsecFrom.Content.McDataType.DataCode == (int)MelsecMcDataType.Y.DataCode ? OperateResult.CreateSuccessResult<bool[]>(((IEnumerable<byte>)this.yBuffer.GetBytes(melsecFrom.Content.AddressStart, (int)length)).Select<byte, bool>((Func<byte, bool>)(m => m > (byte)0)).ToArray<bool>()) : new OperateResult<bool[]>(StringResources.Language.NotSupportedDataType);
        }

        /// <inheritdoc />
        [EstMqttApi("WriteBoolArray", "")]
        public override OperateResult Write(string address, bool[] value)
        {
            OperateResult<McAddressData> melsecFrom = McAddressData.ParseMelsecFrom(address, (ushort)0);
            if (!melsecFrom.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<bool[]>((OperateResult)melsecFrom);
            if (melsecFrom.Content.McDataType.DataType == (byte)0)
                return (OperateResult)new OperateResult<bool[]>(StringResources.Language.MelsecCurrentTypeNotSupportedWordOperate);
            if ((int)melsecFrom.Content.McDataType.DataCode == (int)MelsecMcDataType.M.DataCode)
            {
                this.mBuffer.SetBytes(((IEnumerable<bool>)value).Select<bool, byte>((Func<bool, byte>)(m => !m ? (byte)0 : (byte)1)).ToArray<byte>(), melsecFrom.Content.AddressStart);
                return OperateResult.CreateSuccessResult();
            }
            if ((int)melsecFrom.Content.McDataType.DataCode == (int)MelsecMcDataType.X.DataCode)
            {
                this.xBuffer.SetBytes(((IEnumerable<bool>)value).Select<bool, byte>((Func<bool, byte>)(m => !m ? (byte)0 : (byte)1)).ToArray<byte>(), melsecFrom.Content.AddressStart);
                return OperateResult.CreateSuccessResult();
            }
            if ((int)melsecFrom.Content.McDataType.DataCode != (int)MelsecMcDataType.Y.DataCode)
                return (OperateResult)new OperateResult<bool[]>(StringResources.Language.NotSupportedDataType);
            this.yBuffer.SetBytes(((IEnumerable<bool>)value).Select<bool, byte>((Func<bool, byte>)(m => !m ? (byte)0 : (byte)1)).ToArray<byte>(), melsecFrom.Content.AddressStart);
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
                    byte[] back = (byte[])null;
                    if (!ESTCore.Common.Authorization.asdniasnfaksndiqwhawfskhfaiw())
                    {
                        this.RemoveClient(session);
                        session = (AppSession)null;
                        return;
                    }
                    OperateResult<byte[]> read1;
                    if (this.isBinary)
                    {
                        read1 = await this.ReceiveByMessageAsync(session.WorkSocket, 5000, (INetMessage)new MelsecQnA3EBinaryMessage());
                        if (!read1.IsSuccess)
                        {
                            this.RemoveClient(session);
                            session = (AppSession)null;
                            return;
                        }
                        back = this.ReadFromMcCore(read1.Content.RemoveBegin<byte>(11));
                    }
                    else
                    {
                        read1 = await this.ReceiveByMessageAsync(session.WorkSocket, 5000, (INetMessage)new MelsecQnA3EAsciiMessage());
                        if (!read1.IsSuccess)
                        {
                            this.RemoveClient(session);
                            session = (AppSession)null;
                            return;
                        }
                        back = this.ReadFromMcAsciiCore(read1.Content.RemoveBegin<byte>(22));
                    }
                    this.LogNet?.WriteDebug(this.ToString(), string.Format("[{0}] Tcp {1}：{2}", (object)session.IpEndPoint, (object)StringResources.Language.Receive, this.isBinary ? (object)read1.Content.ToHexString(' ') : (object)Encoding.ASCII.GetString(read1.Content)));
                    if (back != null)
                    {
                        session.WorkSocket.Send(back);
                        this.LogNet?.WriteDebug(this.ToString(), string.Format("[{0}] Tcp {1}：{2}", (object)session.IpEndPoint, (object)StringResources.Language.Send, this.isBinary ? (object)back.ToHexString(' ') : (object)Encoding.ASCII.GetString(back)));
                        session.HeartTime = DateTime.Now;
                        this.RaiseDataReceived((object)session, read1.Content);
                        session.WorkSocket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, new AsyncCallback(this.SocketAsyncCallBack), (object)session);
                        back = (byte[])null;
                        read1 = (OperateResult<byte[]>)null;
                    }
                    else
                    {
                        this.RemoveClient(session);
                        session = (AppSession)null;
                        return;
                    }
                }
                catch (Exception ex)
                {
                    this.RemoveClient(session, "SocketAsyncCallBack -> " + ex.Message);
                }
                session = (AppSession)null;
            }
        }

        /// <summary>
        /// 当收到mc协议的报文的时候应该触发的方法，允许继承重写，来实现自定义的返回，或是数据监听。<br />
        /// The method that should be triggered when a message of the mc protocol is received,
        /// allowing inheritance to be rewritten to implement custom return or data monitoring.
        /// </summary>
        /// <param name="mcCore">mc报文</param>
        /// <returns>返回的报文信息</returns>
        protected virtual byte[] ReadFromMcCore(byte[] mcCore)
        {
            if (mcCore[0] == (byte)1 && mcCore[1] == (byte)4)
                return this.PackCommand((ushort)0, this.ReadByCommand(mcCore));
            if (mcCore[0] != (byte)1 || mcCore[1] != (byte)20)
                return (byte[])null;
            return !this.EnableWrite ? this.PackCommand((ushort)49250, (byte[])null) : this.PackCommand((ushort)0, this.WriteByMessage(mcCore));
        }

        /// <summary>
        /// 当收到mc协议的报文的时候应该触发的方法，允许继承重写，来实现自定义的返回，或是数据监听。<br />
        /// The method that should be triggered when a message of the mc protocol is received,
        /// allowing inheritance to be rewritten to implement custom return or data monitoring.
        /// </summary>
        /// <param name="mcCore">mc报文</param>
        /// <returns>返回的报文信息</returns>
        protected virtual byte[] ReadFromMcAsciiCore(byte[] mcCore)
        {
            if (mcCore[0] == (byte)48 && mcCore[1] == (byte)52 && mcCore[2] == (byte)48 && mcCore[3] == (byte)49)
                return this.PackCommand((ushort)0, this.ReadAsciiByCommand(mcCore));
            if (mcCore[0] != (byte)49 || mcCore[1] != (byte)52 || mcCore[2] != (byte)48 || mcCore[3] != (byte)49)
                return (byte[])null;
            return !this.EnableWrite ? this.PackCommand((ushort)49250, (byte[])null) : this.PackCommand((ushort)0, this.WriteAsciiByMessage(mcCore));
        }

        private byte[] PackCommand(ushort status, byte[] data)
        {
            if (data == null)
                data = new byte[0];
            if (this.isBinary)
            {
                byte[] numArray = new byte[11 + data.Length];
                SoftBasic.HexStringToBytes("D0 00 00 FF FF 03 00 00 00 00 00").CopyTo((Array)numArray, 0);
                if ((uint)data.Length > 0U)
                    data.CopyTo((Array)numArray, 11);
                BitConverter.GetBytes((short)(data.Length + 2)).CopyTo((Array)numArray, 7);
                BitConverter.GetBytes(status).CopyTo((Array)numArray, 9);
                return numArray;
            }
            byte[] numArray1 = new byte[22 + data.Length];
            Encoding.ASCII.GetBytes("D00000FF03FF0000000000").CopyTo((Array)numArray1, 0);
            if ((uint)data.Length > 0U)
                data.CopyTo((Array)numArray1, 22);
            Encoding.ASCII.GetBytes((data.Length + 4).ToString("X4")).CopyTo((Array)numArray1, 14);
            Encoding.ASCII.GetBytes(status.ToString("X4")).CopyTo((Array)numArray1, 18);
            return numArray1;
        }

        private byte[] ReadByCommand(byte[] command)
        {
            ushort num = this.ByteTransform.TransUInt16(command, 8);
            int index = (int)command[6] * 65536 + (int)command[5] * 256 + (int)command[4];
            if (command[2] == (byte)1)
            {
                if ((int)command[7] == (int)MelsecMcDataType.M.DataCode)
                    return MelsecHelper.TransBoolArrayToByteData(this.mBuffer.GetBytes(index, (int)num));
                if ((int)command[7] == (int)MelsecMcDataType.X.DataCode)
                    return MelsecHelper.TransBoolArrayToByteData(this.xBuffer.GetBytes(index, (int)num));
                if ((int)command[7] == (int)MelsecMcDataType.Y.DataCode)
                    return MelsecHelper.TransBoolArrayToByteData(this.yBuffer.GetBytes(index, (int)num));
                throw new Exception(StringResources.Language.NotSupportedDataType);
            }
            if ((int)command[7] == (int)MelsecMcDataType.M.DataCode)
                return ((IEnumerable<byte>)this.mBuffer.GetBytes(index, (int)num * 16)).Select<byte, bool>((Func<byte, bool>)(m => m > (byte)0)).ToArray<bool>().ToByteArray();
            if ((int)command[7] == (int)MelsecMcDataType.X.DataCode)
                return ((IEnumerable<byte>)this.xBuffer.GetBytes(index, (int)num * 16)).Select<byte, bool>((Func<byte, bool>)(m => m > (byte)0)).ToArray<bool>().ToByteArray();
            if ((int)command[7] == (int)MelsecMcDataType.Y.DataCode)
                return ((IEnumerable<byte>)this.yBuffer.GetBytes(index, (int)num * 16)).Select<byte, bool>((Func<byte, bool>)(m => m > (byte)0)).ToArray<bool>().ToByteArray();
            if ((int)command[7] == (int)MelsecMcDataType.D.DataCode)
                return this.dBuffer.GetBytes(index * 2, (int)num * 2);
            if ((int)command[7] == (int)MelsecMcDataType.W.DataCode)
                return this.wBuffer.GetBytes(index * 2, (int)num * 2);
            throw new Exception(StringResources.Language.NotSupportedDataType);
        }

        private byte[] ReadAsciiByCommand(byte[] command)
        {
            ushort uint16 = Convert.ToUInt16(Encoding.ASCII.GetString(command, 16, 4), 16);
            string str = Encoding.ASCII.GetString(command, 8, 2);
            int index = !(str == MelsecMcDataType.X.AsciiCode) && !(str == MelsecMcDataType.Y.AsciiCode) && !(str == MelsecMcDataType.W.AsciiCode) ? (int)Convert.ToUInt16(Encoding.ASCII.GetString(command, 10, 6)) : (int)Convert.ToUInt16(Encoding.ASCII.GetString(command, 10, 6), 16);
            if (command[7] == (byte)49)
            {
                if (str == MelsecMcDataType.M.AsciiCode)
                    return ((IEnumerable<byte>)this.mBuffer.GetBytes(index, (int)uint16)).Select<byte, byte>((Func<byte, byte>)(m => m == (byte)0 ? (byte)48 : (byte)49)).ToArray<byte>();
                if (str == MelsecMcDataType.X.AsciiCode)
                    return ((IEnumerable<byte>)this.xBuffer.GetBytes(index, (int)uint16)).Select<byte, byte>((Func<byte, byte>)(m => m == (byte)0 ? (byte)48 : (byte)49)).ToArray<byte>();
                if (str == MelsecMcDataType.Y.AsciiCode)
                    return ((IEnumerable<byte>)this.yBuffer.GetBytes(index, (int)uint16)).Select<byte, byte>((Func<byte, byte>)(m => m == (byte)0 ? (byte)48 : (byte)49)).ToArray<byte>();
                throw new Exception(StringResources.Language.NotSupportedDataType);
            }
            if (str == MelsecMcDataType.M.AsciiCode)
                return MelsecHelper.TransByteArrayToAsciiByteArray(SoftBasic.BoolArrayToByte(((IEnumerable<byte>)this.mBuffer.GetBytes(index, (int)uint16 * 16)).Select<byte, bool>((Func<byte, bool>)(m => m > (byte)0)).ToArray<bool>()));
            if (str == MelsecMcDataType.X.AsciiCode)
                return MelsecHelper.TransByteArrayToAsciiByteArray(SoftBasic.BoolArrayToByte(((IEnumerable<byte>)this.xBuffer.GetBytes(index, (int)uint16 * 16)).Select<byte, bool>((Func<byte, bool>)(m => m > (byte)0)).ToArray<bool>()));
            if (str == MelsecMcDataType.Y.AsciiCode)
                return MelsecHelper.TransByteArrayToAsciiByteArray(SoftBasic.BoolArrayToByte(((IEnumerable<byte>)this.yBuffer.GetBytes(index, (int)uint16 * 16)).Select<byte, bool>((Func<byte, bool>)(m => m > (byte)0)).ToArray<bool>()));
            if (str == MelsecMcDataType.D.AsciiCode)
                return MelsecHelper.TransByteArrayToAsciiByteArray(this.dBuffer.GetBytes(index * 2, (int)uint16 * 2));
            if (str == MelsecMcDataType.W.AsciiCode)
                return MelsecHelper.TransByteArrayToAsciiByteArray(this.wBuffer.GetBytes(index * 2, (int)uint16 * 2));
            throw new Exception(StringResources.Language.NotSupportedDataType);
        }

        private byte[] WriteByMessage(byte[] command)
        {
            if (!this.EnableWrite)
                return (byte[])null;
            ushort num = this.ByteTransform.TransUInt16(command, 8);
            int destIndex = (int)command[6] * 65536 + (int)command[5] * 256 + (int)command[4];
            if (command[2] == (byte)1)
            {
                byte[] content = MelsecMcNet.ExtractActualData(command.RemoveBegin<byte>(10), true).Content;
                if ((int)command[7] == (int)MelsecMcDataType.M.DataCode)
                    this.mBuffer.SetBytes(((IEnumerable<byte>)content).Take<byte>((int)num).ToArray<byte>(), destIndex);
                else if ((int)command[7] == (int)MelsecMcDataType.X.DataCode)
                {
                    this.xBuffer.SetBytes(((IEnumerable<byte>)content).Take<byte>((int)num).ToArray<byte>(), destIndex);
                }
                else
                {
                    if ((int)command[7] != (int)MelsecMcDataType.Y.DataCode)
                        throw new Exception(StringResources.Language.NotSupportedDataType);
                    this.yBuffer.SetBytes(((IEnumerable<byte>)content).Take<byte>((int)num).ToArray<byte>(), destIndex);
                }
                return new byte[0];
            }
            if ((int)command[7] == (int)MelsecMcDataType.M.DataCode)
            {
                this.mBuffer.SetBytes(((IEnumerable<bool>)SoftBasic.ByteToBoolArray(SoftBasic.ArrayRemoveBegin<byte>(command, 10))).Select<bool, byte>((Func<bool, byte>)(m => !m ? (byte)0 : (byte)1)).ToArray<byte>(), destIndex);
                return new byte[0];
            }
            if ((int)command[7] == (int)MelsecMcDataType.X.DataCode)
            {
                this.xBuffer.SetBytes(((IEnumerable<bool>)SoftBasic.ByteToBoolArray(SoftBasic.ArrayRemoveBegin<byte>(command, 10))).Select<bool, byte>((Func<bool, byte>)(m => !m ? (byte)0 : (byte)1)).ToArray<byte>(), destIndex);
                return new byte[0];
            }
            if ((int)command[7] == (int)MelsecMcDataType.Y.DataCode)
            {
                this.yBuffer.SetBytes(((IEnumerable<bool>)SoftBasic.ByteToBoolArray(SoftBasic.ArrayRemoveBegin<byte>(command, 10))).Select<bool, byte>((Func<bool, byte>)(m => !m ? (byte)0 : (byte)1)).ToArray<byte>(), destIndex);
                return new byte[0];
            }
            if ((int)command[7] == (int)MelsecMcDataType.D.DataCode)
            {
                this.dBuffer.SetBytes(SoftBasic.ArrayRemoveBegin<byte>(command, 10), destIndex * 2);
                return new byte[0];
            }
            if ((int)command[7] != (int)MelsecMcDataType.W.DataCode)
                throw new Exception(StringResources.Language.NotSupportedDataType);
            this.wBuffer.SetBytes(SoftBasic.ArrayRemoveBegin<byte>(command, 10), destIndex * 2);
            return new byte[0];
        }

        private byte[] WriteAsciiByMessage(byte[] command)
        {
            ushort uint16 = Convert.ToUInt16(Encoding.ASCII.GetString(command, 16, 4), 16);
            string str = Encoding.ASCII.GetString(command, 8, 2);
            int destIndex = !(str == MelsecMcDataType.X.AsciiCode) && !(str == MelsecMcDataType.Y.AsciiCode) && !(str == MelsecMcDataType.W.AsciiCode) ? (int)Convert.ToUInt16(Encoding.ASCII.GetString(command, 10, 6)) : (int)Convert.ToUInt16(Encoding.ASCII.GetString(command, 10, 6), 16);
            if (command[7] == (byte)49)
            {
                byte[] array = ((IEnumerable<byte>)command.RemoveBegin<byte>(20)).Select<byte, byte>((Func<byte, byte>)(m => m != (byte)49 ? (byte)0 : (byte)1)).ToArray<byte>();
                if (str == MelsecMcDataType.M.AsciiCode)
                    this.mBuffer.SetBytes(((IEnumerable<byte>)array).Take<byte>((int)uint16).ToArray<byte>(), destIndex);
                else if (str == MelsecMcDataType.X.AsciiCode)
                {
                    this.xBuffer.SetBytes(((IEnumerable<byte>)array).Take<byte>((int)uint16).ToArray<byte>(), destIndex);
                }
                else
                {
                    if (!(str == MelsecMcDataType.Y.AsciiCode))
                        throw new Exception(StringResources.Language.NotSupportedDataType);
                    this.yBuffer.SetBytes(((IEnumerable<byte>)array).Take<byte>((int)uint16).ToArray<byte>(), destIndex);
                }
                return new byte[0];
            }
            if (str == MelsecMcDataType.M.AsciiCode)
            {
                this.mBuffer.SetBytes(((IEnumerable<bool>)SoftBasic.ByteToBoolArray(MelsecHelper.TransAsciiByteArrayToByteArray(command.RemoveBegin<byte>(20)))).Select<bool, byte>((Func<bool, byte>)(m => !m ? (byte)0 : (byte)1)).ToArray<byte>(), destIndex);
                return new byte[0];
            }
            if (str == MelsecMcDataType.X.AsciiCode)
            {
                this.xBuffer.SetBytes(((IEnumerable<bool>)SoftBasic.ByteToBoolArray(MelsecHelper.TransAsciiByteArrayToByteArray(command.RemoveBegin<byte>(20)))).Select<bool, byte>((Func<bool, byte>)(m => !m ? (byte)0 : (byte)1)).ToArray<byte>(), destIndex);
                return new byte[0];
            }
            if (str == MelsecMcDataType.Y.AsciiCode)
            {
                this.yBuffer.SetBytes(((IEnumerable<bool>)SoftBasic.ByteToBoolArray(MelsecHelper.TransAsciiByteArrayToByteArray(command.RemoveBegin<byte>(20)))).Select<bool, byte>((Func<bool, byte>)(m => !m ? (byte)0 : (byte)1)).ToArray<byte>(), destIndex);
                return new byte[0];
            }
            if (str == MelsecMcDataType.D.AsciiCode)
            {
                this.dBuffer.SetBytes(MelsecHelper.TransAsciiByteArrayToByteArray(command.RemoveBegin<byte>(20)), destIndex * 2);
                return new byte[0];
            }
            if (!(str == MelsecMcDataType.W.AsciiCode))
                throw new Exception(StringResources.Language.NotSupportedDataType);
            this.wBuffer.SetBytes(MelsecHelper.TransAsciiByteArrayToByteArray(command.RemoveBegin<byte>(20)), destIndex * 2);
            return new byte[0];
        }

        /// <inheritdoc />
        protected override void LoadFromBytes(byte[] content)
        {
            if (content.Length < 458752)
                throw new Exception("File is not correct");
            this.mBuffer.SetBytes(content, 0, 0, 65536);
            this.xBuffer.SetBytes(content, 65536, 0, 65536);
            this.yBuffer.SetBytes(content, 131072, 0, 65536);
            this.dBuffer.SetBytes(content, 196608, 0, 131072);
            this.wBuffer.SetBytes(content, 327680, 0, 131072);
        }

        /// <inheritdoc />
        [EstMqttApi]
        protected override byte[] SaveToBytes()
        {
            byte[] numArray = new byte[458752];
            Array.Copy((Array)this.mBuffer.GetBytes(), 0, (Array)numArray, 0, 65536);
            Array.Copy((Array)this.xBuffer.GetBytes(), 0, (Array)numArray, 65536, 65536);
            Array.Copy((Array)this.yBuffer.GetBytes(), 0, (Array)numArray, 131072, 65536);
            Array.Copy((Array)this.dBuffer.GetBytes(), 0, (Array)numArray, 196608, 131072);
            Array.Copy((Array)this.wBuffer.GetBytes(), 0, (Array)numArray, 327680, 131072);
            return numArray;
        }

        /// <summary>释放当前的对象</summary>
        /// <param name="disposing">是否托管对象</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.xBuffer?.Dispose();
                this.yBuffer?.Dispose();
                this.mBuffer?.Dispose();
                this.dBuffer?.Dispose();
                this.wBuffer?.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// 获取或设置当前的通信格式是否是二进制<br />
        /// Get or set whether the current communication format is binary
        /// </summary>
        public bool IsBinary
        {
            get => this.isBinary;
            set => this.isBinary = value;
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("MelsecMcServer[{0}]", (object)this.Port);
    }
}
