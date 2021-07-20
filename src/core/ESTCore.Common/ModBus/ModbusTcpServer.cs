// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.ModBus.ModbusTcpServer
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;
using ESTCore.Common.Core;
using ESTCore.Common.Core.Address;
using ESTCore.Common.Core.IMessage;
using ESTCore.Common.Core.Net;
using ESTCore.Common.Reflection;
using ESTCore.Common.Serial;

using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESTCore.Common.ModBus
{
    /// <summary>
    /// <b>[商业授权]</b> Modbus的虚拟服务器，同时支持Tcp和Rtu的机制，支持线圈，离散输入，寄存器和输入寄存器的读写操作，同时支持掩码写入功能，可以用来当做系统的数据交换池<br />
    /// <b>[Authorization]</b> Modbus virtual server supports Tcp and Rtu mechanisms at the same time, supports read and write operations of coils, discrete inputs, r
    /// egisters and input registers, and supports mask write function, which can be used as a system data exchange pool
    /// </summary>
    /// <remarks>
    /// 可以基于本类实现一个功能复杂的modbus服务器，支持Modbus-Tcp，启动串口后，还支持Modbus-Rtu和Modbus-ASCII，会根据报文进行动态的适配。
    /// <list type="number">
    /// <item>线圈，功能码对应01，05，15</item>
    /// <item>离散输入，功能码对应02</item>
    /// <item>寄存器，功能码对应03，06，16</item>
    /// <item>输入寄存器，功能码对应04，输入寄存器在服务器端可以实现读写的操作</item>
    /// <item>掩码写入，功能码对应22，可以对字寄存器进行位操作</item>
    /// </list>
    /// </remarks>
    /// <example>
    /// 读写的地址格式为富文本地址，具体请参照下面的示例代码。
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Modbus\ModbusTcpServer.cs" region="ModbusTcpServerExample" title="ModbusTcpServer示例" />
    /// </example>
    public class ModbusTcpServer : NetworkDataServerBase
    {
        private List<ModBusMonitorAddress> subscriptions;
        private SimpleHybirdLock subcriptionHybirdLock;
        private SerialPort serialPort;
        private SoftBuffer coilBuffer;
        private SoftBuffer inputBuffer;
        private SoftBuffer registerBuffer;
        private SoftBuffer inputRegisterBuffer;
        private const int DataPoolLength = 65536;
        private int station = 1;

        /// <summary>实例化一个Modbus Tcp及Rtu的服务器，支持数据读写操作</summary>
        public ModbusTcpServer()
        {
            this.coilBuffer = new SoftBuffer(65536);
            this.inputBuffer = new SoftBuffer(65536);
            this.registerBuffer = new SoftBuffer(131072);
            this.inputRegisterBuffer = new SoftBuffer(131072);
            this.registerBuffer.IsBoolReverseByWord = true;
            this.inputRegisterBuffer.IsBoolReverseByWord = true;
            this.subscriptions = new List<ModBusMonitorAddress>();
            this.subcriptionHybirdLock = new SimpleHybirdLock();
            this.ByteTransform = (IByteTransform)new ReverseWordTransform();
            this.WordLength = (ushort)1;
            this.serialPort = new SerialPort();
        }

        /// <inheritdoc cref="P:ESTCore.Common.ModBus.ModbusTcpNet.DataFormat" />
        public DataFormat DataFormat
        {
            get => this.ByteTransform.DataFormat;
            set => this.ByteTransform.DataFormat = value;
        }

        /// <inheritdoc cref="P:ESTCore.Common.ModBus.ModbusTcpNet.IsStringReverse" />
        public bool IsStringReverse
        {
            get => this.ByteTransform.IsStringReverseByteWord;
            set => this.ByteTransform.IsStringReverseByteWord = value;
        }

        /// <inheritdoc cref="P:ESTCore.Common.ModBus.ModbusTcpNet.Station" />
        public int Station
        {
            get => this.station;
            set => this.station = value;
        }

        /// <inheritdoc />
        protected override byte[] SaveToBytes()
        {
            byte[] numArray = new byte[393216];
            Array.Copy((Array)this.coilBuffer.GetBytes(), 0, (Array)numArray, 0, 65536);
            Array.Copy((Array)this.inputBuffer.GetBytes(), 0, (Array)numArray, 65536, 65536);
            Array.Copy((Array)this.registerBuffer.GetBytes(), 0, (Array)numArray, 131072, 131072);
            Array.Copy((Array)this.inputRegisterBuffer.GetBytes(), 0, (Array)numArray, 262144, 131072);
            return numArray;
        }

        /// <inheritdoc />
        protected override void LoadFromBytes(byte[] content)
        {
            if (content.Length < 393216)
                throw new Exception("File is not correct");
            this.coilBuffer.SetBytes(content, 0, 0, 65536);
            this.inputBuffer.SetBytes(content, 65536, 0, 65536);
            this.registerBuffer.SetBytes(content, 131072, 0, 131072);
            this.inputRegisterBuffer.SetBytes(content, 262144, 0, 131072);
        }

        /// <summary>读取地址的线圈的通断情况</summary>
        /// <param name="address">起始地址，示例："100"</param>
        /// <returns><c>True</c>或是<c>False</c></returns>
        /// <exception cref="T:System.IndexOutOfRangeException"></exception>
        public bool ReadCoil(string address) => this.coilBuffer.GetByte((int)ushort.Parse(address)) > (byte)0;

        /// <summary>批量读取地址的线圈的通断情况</summary>
        /// <param name="address">起始地址，示例："100"</param>
        /// <param name="length">读取长度</param>
        /// <returns><c>True</c>或是<c>False</c></returns>
        /// <exception cref="T:System.IndexOutOfRangeException"></exception>
        public bool[] ReadCoil(string address, ushort length) => ((IEnumerable<byte>)this.coilBuffer.GetBytes((int)ushort.Parse(address), (int)length)).Select<byte, bool>((Func<byte, bool>)(m => m > (byte)0)).ToArray<bool>();

        /// <summary>写入线圈的通断值</summary>
        /// <param name="address">起始地址，示例："100"</param>
        /// <param name="data">是否通断</param>
        /// <returns><c>True</c>或是<c>False</c></returns>
        /// <exception cref="T:System.IndexOutOfRangeException"></exception>
        public void WriteCoil(string address, bool data)
        {
            ushort num = ushort.Parse(address);
            this.coilBuffer.SetValue(data ? (byte)1 : (byte)0, (int)num);
        }

        /// <summary>写入线圈数组的通断值</summary>
        /// <param name="address">起始地址，示例："100"</param>
        /// <param name="data">是否通断</param>
        /// <returns><c>True</c>或是<c>False</c></returns>
        /// <exception cref="T:System.IndexOutOfRangeException"></exception>
        public void WriteCoil(string address, bool[] data)
        {
            if (data == null)
                return;
            ushort num = ushort.Parse(address);
            this.coilBuffer.SetBytes(((IEnumerable<bool>)data).Select<bool, byte>((Func<bool, byte>)(m => m ? (byte)1 : (byte)0)).ToArray<byte>(), (int)num);
        }

        /// <summary>读取地址的离散线圈的通断情况</summary>
        /// <param name="address">起始地址，示例："100"</param>
        /// <returns><c>True</c>或是<c>False</c></returns>
        /// <exception cref="T:System.IndexOutOfRangeException"></exception>
        public bool ReadDiscrete(string address) => this.inputBuffer.GetByte((int)ushort.Parse(address)) > (byte)0;

        /// <summary>批量读取地址的离散线圈的通断情况</summary>
        /// <param name="address">起始地址，示例："100"</param>
        /// <param name="length">读取长度</param>
        /// <returns><c>True</c>或是<c>False</c></returns>
        /// <exception cref="T:System.IndexOutOfRangeException"></exception>
        public bool[] ReadDiscrete(string address, ushort length) => ((IEnumerable<byte>)this.inputBuffer.GetBytes((int)ushort.Parse(address), (int)length)).Select<byte, bool>((Func<byte, bool>)(m => m > (byte)0)).ToArray<bool>();

        /// <summary>写入离散线圈的通断值</summary>
        /// <param name="address">起始地址，示例："100"</param>
        /// <param name="data">是否通断</param>
        /// <exception cref="T:System.IndexOutOfRangeException"></exception>
        public void WriteDiscrete(string address, bool data)
        {
            ushort num = ushort.Parse(address);
            this.inputBuffer.SetValue(data ? (byte)1 : (byte)0, (int)num);
        }

        /// <summary>写入离散线圈数组的通断值</summary>
        /// <param name="address">起始地址，示例："100"</param>
        /// <param name="data">是否通断</param>
        /// <exception cref="T:System.IndexOutOfRangeException"></exception>
        public void WriteDiscrete(string address, bool[] data)
        {
            if (data == null)
                return;
            ushort num = ushort.Parse(address);
            this.inputBuffer.SetBytes(((IEnumerable<bool>)data).Select<bool, byte>((Func<bool, byte>)(m => m ? (byte)1 : (byte)0)).ToArray<byte>(), (int)num);
        }

        /// <inheritdoc cref="M:ESTCore.Common.ModBus.ModbusTcpNet.Read(System.String,System.UInt16)" />
        [EstMqttApi("ReadByteArray", "")]
        public override OperateResult<byte[]> Read(string address, ushort length)
        {
            OperateResult<ModbusAddress> operateResult = ModbusInfo.AnalysisAddress(address, (byte)this.Station, true, (byte)3);
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult);
            if (operateResult.Content.Function == 3)
                return OperateResult.CreateSuccessResult<byte[]>(this.registerBuffer.GetBytes((int)operateResult.Content.Address * 2, (int)length * 2));
            return operateResult.Content.Function == 4 ? OperateResult.CreateSuccessResult<byte[]>(this.inputRegisterBuffer.GetBytes((int)operateResult.Content.Address * 2, (int)length * 2)) : new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType);
        }

        /// <inheritdoc cref="M:ESTCore.Common.ModBus.ModbusTcpNet.Write(System.String,System.Byte[])" />
        [EstMqttApi("WriteByteArray", "")]
        public override OperateResult Write(string address, byte[] value)
        {
            OperateResult<ModbusAddress> operateResult = ModbusInfo.AnalysisAddress(address, (byte)this.Station, true, (byte)3);
            if (!operateResult.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult);
            if (operateResult.Content.Function == 3)
            {
                this.registerBuffer.SetBytes(value, (int)operateResult.Content.Address * 2);
                return OperateResult.CreateSuccessResult();
            }
            if (operateResult.Content.Function != 4)
                return (OperateResult)new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType);
            this.inputRegisterBuffer.SetBytes(value, (int)operateResult.Content.Address * 2);
            return OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc cref="M:ESTCore.Common.ModBus.ModbusTcpNet.ReadBool(System.String,System.UInt16)" />
        [EstMqttApi("ReadBoolArray", "")]
        public override OperateResult<bool[]> ReadBool(string address, ushort length)
        {
            OperateResult<ModbusAddress> operateResult = ModbusInfo.AnalysisAddress(address, (byte)this.Station, true, (byte)1);
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult);
            if (operateResult.Content.Function == 1)
                return OperateResult.CreateSuccessResult<bool[]>(((IEnumerable<byte>)this.coilBuffer.GetBytes((int)operateResult.Content.Address, (int)length)).Select<byte, bool>((Func<byte, bool>)(m => m > (byte)0)).ToArray<bool>());
            return operateResult.Content.Function == 2 ? OperateResult.CreateSuccessResult<bool[]>(((IEnumerable<byte>)this.inputBuffer.GetBytes((int)operateResult.Content.Address, (int)length)).Select<byte, bool>((Func<byte, bool>)(m => m > (byte)0)).ToArray<bool>()) : new OperateResult<bool[]>(StringResources.Language.NotSupportedDataType);
        }

        /// <inheritdoc cref="M:ESTCore.Common.ModBus.ModbusTcpNet.Write(System.String,System.Boolean[])" />
        [EstMqttApi("WriteBoolArray", "")]
        public override OperateResult Write(string address, bool[] value)
        {
            OperateResult<ModbusAddress> operateResult = ModbusInfo.AnalysisAddress(address, (byte)this.Station, true, (byte)1);
            if (!operateResult.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult);
            if (operateResult.Content.Function == 1)
            {
                this.coilBuffer.SetBytes(((IEnumerable<bool>)value).Select<bool, byte>((Func<bool, byte>)(m => m ? (byte)1 : (byte)0)).ToArray<byte>(), (int)operateResult.Content.Address);
                return OperateResult.CreateSuccessResult();
            }
            if (operateResult.Content.Function != 2)
                return (OperateResult)new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType);
            this.inputBuffer.SetBytes(((IEnumerable<bool>)value).Select<bool, byte>((Func<bool, byte>)(m => m ? (byte)1 : (byte)0)).ToArray<byte>(), (int)operateResult.Content.Address);
            return OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc cref="M:ESTCore.Common.ModBus.ModbusTcpNet.Write(System.String,System.Boolean)" />
        [EstMqttApi("WriteBool", "")]
        public override OperateResult Write(string address, bool value)
        {
            if (address.IndexOf('.') < 0)
                return base.Write(address, value);
            try
            {
                int int32 = Convert.ToInt32(address.Substring(address.IndexOf('.') + 1));
                address = address.Substring(0, address.IndexOf('.'));
                OperateResult<ModbusAddress> operateResult = ModbusInfo.AnalysisAddress(address, (byte)this.Station, true, (byte)3);
                if (!operateResult.IsSuccess)
                    return (OperateResult)operateResult;
                int destIndex = (int)operateResult.Content.Address * 16 + int32;
                if (operateResult.Content.Function == 3)
                {
                    this.registerBuffer.SetBool(value, destIndex);
                    return OperateResult.CreateSuccessResult();
                }
                if (operateResult.Content.Function != 4)
                    return new OperateResult(StringResources.Language.NotSupportedDataType);
                this.inputRegisterBuffer.SetBool(value, destIndex);
                return OperateResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                return new OperateResult(ex.Message);
            }
        }

        /// <summary>写入寄存器数据，指定字节数据</summary>
        /// <param name="address">起始地址，示例："100"，如果是输入寄存器："x=4;100"</param>
        /// <param name="high">高位数据</param>
        /// <param name="low">地位数据</param>
        public void Write(string address, byte high, byte low) => this.Write(address, new byte[2]
        {
      high,
      low
        });

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
                OperateResult<byte[]> read1 = await this.ReceiveByMessageAsync(session.WorkSocket, 2000, (INetMessage)new ModbusTcpMessage());
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
                else if (!this.CheckModbusMessageLegal(read1.Content.RemoveBegin<byte>(6)))
                {
                    this.RemoveClient(session);
                    session = (AppSession)null;
                }
                else
                {
                    this.LogNet?.WriteDebug(this.ToString(), string.Format("[{0}] Tcp {1}：{2}", (object)session.IpEndPoint, (object)StringResources.Language.Receive, (object)read1.Content.ToHexString(' ')));
                    ushort id = (ushort)((uint)read1.Content[0] * 256U + (uint)read1.Content[1]);
                    byte[] back = ModbusInfo.PackCommandToTcp(this.ReadFromModbusCore(read1.Content.RemoveBegin<byte>(6)), id);
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

        /// <summary>
        /// 创建特殊的功能标识，然后返回该信息<br />
        /// Create a special feature ID and return this information
        /// </summary>
        /// <param name="modbusCore">modbus核心报文</param>
        /// <param name="error">错误码</param>
        /// <returns>携带错误码的modbus报文</returns>
        private byte[] CreateExceptionBack(byte[] modbusCore, byte error) => new byte[3]
        {
      modbusCore[0],
      (byte) ((uint) modbusCore[1] + 128U),
      error
        };

        /// <summary>
        /// 创建返回消息<br />
        /// Create return message
        /// </summary>
        /// <param name="modbusCore">modbus核心报文</param>
        /// <param name="content">返回的实际数据内容</param>
        /// <returns>携带内容的modbus报文</returns>
        private byte[] CreateReadBack(byte[] modbusCore, byte[] content) => SoftBasic.SpliceArray<byte>(new byte[3]
        {
      modbusCore[0],
      modbusCore[1],
      (byte) content.Length
        }, content);

        /// <summary>
        /// 创建写入成功的反馈信号<br />
        /// Create feedback signal for successful write
        /// </summary>
        /// <param name="modbus">modbus核心报文</param>
        /// <returns>携带成功写入的信息</returns>
        private byte[] CreateWriteBack(byte[] modbus) => modbus.SelectBegin<byte>(6);

        private byte[] ReadCoilBack(byte[] modbus, string addressHead)
        {
            try
            {
                ushort num = this.ByteTransform.TransUInt16(modbus, 2);
                ushort length = this.ByteTransform.TransUInt16(modbus, 4);
                if ((int)num + (int)length > 65536)
                    return this.CreateExceptionBack(modbus, (byte)2);
                if (length > (ushort)2040)
                    return this.CreateExceptionBack(modbus, (byte)3);
                bool[] content = this.ReadBool(addressHead + num.ToString(), length).Content;
                return this.CreateReadBack(modbus, SoftBasic.BoolArrayToByte(content));
            }
            catch (Exception ex)
            {
                this.LogNet?.WriteException(this.ToString(), StringResources.Language.ModbusTcpReadCoilException, ex);
                return this.CreateExceptionBack(modbus, (byte)4);
            }
        }

        private byte[] ReadRegisterBack(byte[] modbus, string addressHead)
        {
            try
            {
                ushort num = this.ByteTransform.TransUInt16(modbus, 2);
                ushort length = this.ByteTransform.TransUInt16(modbus, 4);
                if ((int)num + (int)length > 65536)
                    return this.CreateExceptionBack(modbus, (byte)2);
                if (length > (ushort)sbyte.MaxValue)
                    return this.CreateExceptionBack(modbus, (byte)3);
                byte[] content = this.Read(addressHead + num.ToString(), length).Content;
                return this.CreateReadBack(modbus, content);
            }
            catch (Exception ex)
            {
                this.LogNet?.WriteException(this.ToString(), StringResources.Language.ModbusTcpReadRegisterException, ex);
                return this.CreateExceptionBack(modbus, (byte)4);
            }
        }

        private byte[] WriteOneCoilBack(byte[] modbus)
        {
            try
            {
                if (!this.EnableWrite)
                    return this.CreateExceptionBack(modbus, (byte)4);
                ushort num = this.ByteTransform.TransUInt16(modbus, 2);
                if (modbus[4] == byte.MaxValue && modbus[5] == (byte)0)
                    this.Write(num.ToString(), true);
                else if (modbus[4] == (byte)0 && modbus[5] == (byte)0)
                    this.Write(num.ToString(), false);
                return this.CreateWriteBack(modbus);
            }
            catch (Exception ex)
            {
                this.LogNet?.WriteException(this.ToString(), StringResources.Language.ModbusTcpWriteCoilException, ex);
                return this.CreateExceptionBack(modbus, (byte)4);
            }
        }

        private byte[] WriteOneRegisterBack(byte[] modbus)
        {
            try
            {
                if (!this.EnableWrite)
                    return this.CreateExceptionBack(modbus, (byte)4);
                ushort address = this.ByteTransform.TransUInt16(modbus, 2);
                short content1 = this.ReadInt16(address.ToString()).Content;
                this.Write(address.ToString(), modbus[4], modbus[5]);
                short content2 = this.ReadInt16(address.ToString()).Content;
                this.OnRegisterBeforWrite(address, content1, content2);
                return this.CreateWriteBack(modbus);
            }
            catch (Exception ex)
            {
                this.LogNet?.WriteException(this.ToString(), StringResources.Language.ModbusTcpWriteRegisterException, ex);
                return this.CreateExceptionBack(modbus, (byte)4);
            }
        }

        private byte[] WriteCoilsBack(byte[] modbus)
        {
            try
            {
                if (!this.EnableWrite)
                    return this.CreateExceptionBack(modbus, (byte)4);
                ushort num1 = this.ByteTransform.TransUInt16(modbus, 2);
                ushort num2 = this.ByteTransform.TransUInt16(modbus, 4);
                if ((int)num1 + (int)num2 > 65536)
                    return this.CreateExceptionBack(modbus, (byte)2);
                if (num2 > (ushort)2040)
                    return this.CreateExceptionBack(modbus, (byte)3);
                this.Write(num1.ToString(), modbus.RemoveBegin<byte>(7).ToBoolArray((int)num2));
                return this.CreateWriteBack(modbus);
            }
            catch (Exception ex)
            {
                this.LogNet?.WriteException(this.ToString(), StringResources.Language.ModbusTcpWriteCoilException, ex);
                return this.CreateExceptionBack(modbus, (byte)4);
            }
        }

        private byte[] WriteRegisterBack(byte[] modbus)
        {
            try
            {
                if (!this.EnableWrite)
                    return this.CreateExceptionBack(modbus, (byte)4);
                ushort num1 = this.ByteTransform.TransUInt16(modbus, 2);
                ushort num2 = this.ByteTransform.TransUInt16(modbus, 4);
                if ((int)num1 + (int)num2 > 65536)
                    return this.CreateExceptionBack(modbus, (byte)2);
                if (num2 > (ushort)sbyte.MaxValue)
                    return this.CreateExceptionBack(modbus, (byte)3);
                MonitorAddress[] monitorAddressArray = new MonitorAddress[(int)num2];
                for (ushort index = 0; (int)index < (int)num2; ++index)
                {
                    int num3 = (int)num1 + (int)index;
                    short content1 = this.ReadInt16(num3.ToString()).Content;
                    num3 = (int)num1 + (int)index;
                    this.Write(num3.ToString(), modbus[2 * (int)index + 7], modbus[2 * (int)index + 8]);
                    num3 = (int)num1 + (int)index;
                    short content2 = this.ReadInt16(num3.ToString()).Content;
                    monitorAddressArray[(int)index] = new MonitorAddress()
                    {
                        Address = (ushort)((uint)num1 + (uint)index),
                        ValueOrigin = content1,
                        ValueNew = content2
                    };
                }
                for (int index = 0; index < monitorAddressArray.Length; ++index)
                    this.OnRegisterBeforWrite(monitorAddressArray[index].Address, monitorAddressArray[index].ValueOrigin, monitorAddressArray[index].ValueNew);
                return this.CreateWriteBack(modbus);
            }
            catch (Exception ex)
            {
                this.LogNet?.WriteException(this.ToString(), StringResources.Language.ModbusTcpWriteRegisterException, ex);
                return this.CreateExceptionBack(modbus, (byte)4);
            }
        }

        private byte[] WriteMaskRegisterBack(byte[] modbus)
        {
            try
            {
                if (!this.EnableWrite)
                    return this.CreateExceptionBack(modbus, (byte)4);
                ushort num1 = this.ByteTransform.TransUInt16(modbus, 2);
                int num2 = (int)this.ByteTransform.TransUInt16(modbus, 4);
                int num3 = (int)this.ByteTransform.TransUInt16(modbus, 6);
                int content = (int)this.ReadInt16(num1.ToString()).Content;
                short num4 = (short)(content & num2 | num3);
                this.Write(num1.ToString(), num4);
                MonitorAddress monitorAddress = new MonitorAddress()
                {
                    Address = num1,
                    ValueOrigin = (short)content,
                    ValueNew = num4
                };
                this.OnRegisterBeforWrite(monitorAddress.Address, monitorAddress.ValueOrigin, monitorAddress.ValueNew);
                return modbus;
            }
            catch (Exception ex)
            {
                this.LogNet?.WriteException(this.ToString(), StringResources.Language.ModbusTcpWriteRegisterException, ex);
                return this.CreateExceptionBack(modbus, (byte)4);
            }
        }

        /// <summary>
        /// 新增一个数据监视的任务，针对的是寄存器地址的数据<br />
        /// Added a data monitoring task for data at register addresses
        /// </summary>
        /// <param name="monitor">监视地址对象</param>
        public void AddSubcription(ModBusMonitorAddress monitor)
        {
            this.subcriptionHybirdLock.Enter();
            this.subscriptions.Add(monitor);
            this.subcriptionHybirdLock.Leave();
        }

        /// <summary>
        /// 移除一个数据监视的任务<br />
        /// Remove a data monitoring task
        /// </summary>
        /// <param name="monitor">监视地址对象</param>
        public void RemoveSubcrption(ModBusMonitorAddress monitor)
        {
            this.subcriptionHybirdLock.Enter();
            this.subscriptions.Remove(monitor);
            this.subcriptionHybirdLock.Leave();
        }

        /// <summary>
        /// 在数据变更后，进行触发是否产生订阅<br />
        /// Whether to generate a subscription after triggering data changes
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="before">修改之前的数</param>
        /// <param name="after">修改之后的数</param>
        private void OnRegisterBeforWrite(ushort address, short before, short after)
        {
            this.subcriptionHybirdLock.Enter();
            for (int index = 0; index < this.subscriptions.Count; ++index)
            {
                if ((int)this.subscriptions[index].Address == (int)address)
                {
                    this.subscriptions[index].SetValue(after);
                    if ((int)before != (int)after)
                        this.subscriptions[index].SetChangeValue(before, after);
                }
            }
            this.subcriptionHybirdLock.Leave();
        }

        /// <summary>
        /// 检测当前的Modbus接收的指定是否是合法的<br />
        /// Check if the current Modbus received designation is valid
        /// </summary>
        /// <param name="buffer">缓存数据</param>
        /// <returns>是否合格</returns>
        private bool CheckModbusMessageLegal(byte[] buffer)
        {
            bool flag;
            switch (buffer[1])
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    flag = buffer.Length == 6;
                    break;
                case 15:
                case 16:
                    flag = buffer.Length > 6 && (int)buffer[6] == buffer.Length - 7;
                    break;
                case 22:
                    flag = buffer.Length == 8;
                    break;
                default:
                    flag = true;
                    break;
            }
            if (!flag)
                this.LogNet?.WriteError(this.ToString(), "Receive Nosense Modbus-rtu : " + buffer.ToHexString(' '));
            return flag;
        }

        /// <summary>
        /// Modbus核心数据交互方法，允许重写自己来实现，报文只剩下核心的Modbus信息，去除了MPAB报头信息<br />
        /// The Modbus core data interaction method allows you to rewrite it to achieve the message.
        /// Only the core Modbus information is left in the message, and the MPAB header information is removed.
        /// </summary>
        /// <param name="modbusCore">核心的Modbus报文</param>
        /// <returns>进行数据交互之后的结果</returns>
        protected virtual byte[] ReadFromModbusCore(byte[] modbusCore)
        {
            byte[] numArray;
            switch (modbusCore[1])
            {
                case 1:
                    numArray = this.ReadCoilBack(modbusCore, string.Empty);
                    break;
                case 2:
                    numArray = this.ReadCoilBack(modbusCore, "x=2;");
                    break;
                case 3:
                    numArray = this.ReadRegisterBack(modbusCore, string.Empty);
                    break;
                case 4:
                    numArray = this.ReadRegisterBack(modbusCore, "x=4;");
                    break;
                case 5:
                    numArray = this.WriteOneCoilBack(modbusCore);
                    break;
                case 6:
                    numArray = this.WriteOneRegisterBack(modbusCore);
                    break;
                case 15:
                    numArray = this.WriteCoilsBack(modbusCore);
                    break;
                case 16:
                    numArray = this.WriteRegisterBack(modbusCore);
                    break;
                case 22:
                    numArray = this.WriteMaskRegisterBack(modbusCore);
                    break;
                default:
                    numArray = this.CreateExceptionBack(modbusCore, (byte)1);
                    break;
            }
            return numArray;
        }

        /// <summary>
        /// 启动modbus-rtu的从机服务，使用默认的参数进行初始化串口，9600波特率，8位数据位，无奇偶校验，1位停止位<br />
        /// Start the slave service of modbus-rtu, initialize the serial port with default parameters, 9600 baud rate, 8 data bits, no parity, 1 stop bit
        /// </summary>
        /// <param name="com">串口信息</param>
        public void StartModbusRtu(string com) => this.StartModbusRtu(com, 9600);

        /// <summary>
        /// 启动modbus-rtu的从机服务，使用默认的参数进行初始化串口，8位数据位，无奇偶校验，1位停止位<br />
        /// Start the slave service of modbus-rtu, initialize the serial port with default parameters, 8 data bits, no parity, 1 stop bit
        /// </summary>
        /// <param name="com">串口信息</param>
        /// <param name="baudRate">波特率</param>
        public void StartModbusRtu(string com, int baudRate) => this.StartModbusRtu((Action<System.IO.Ports.SerialPort>)(sp =>
       {
           sp.PortName = com;
           sp.BaudRate = baudRate;
           sp.DataBits = 8;
           sp.Parity = Parity.None;
           sp.StopBits = StopBits.One;
       }));

        /// <summary>
        /// 启动modbus-rtu的从机服务，使用自定义的初始化方法初始化串口的参数<br />
        /// Start the slave service of modbus-rtu and initialize the parameters of the serial port using a custom initialization method
        /// </summary>
        /// <param name="inni">初始化信息的委托</param>
        public void StartModbusRtu(Action<SerialPort> inni)
        {
            if (this.serialPort.IsOpen)
                return;
            if (inni != null)
                inni(this.serialPort);
            this.serialPort.ReadBufferSize = 1024;
            this.serialPort.ReceivedBytesThreshold = 1;
            this.serialPort.Open();
            this.serialPort.DataReceived += new SerialDataReceivedEventHandler(this.SerialPort_DataReceived);
        }

        /// <summary>
        /// 关闭modbus-rtu的串口对象<br />
        /// Close the serial port object of modbus-rtu
        /// </summary>
        public void CloseModbusRtu()
        {
            if (!this.serialPort.IsOpen)
                return;
            this.serialPort.Close();
        }

        /// <summary>接收到串口数据的时候触发</summary>
        /// <param name="sender">串口对象</param>
        /// <param name="e">消息</param>
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int num1 = 0;
            byte[] buffer = new byte[1024];
            int num2;
            do
            {
                Thread.Sleep(20);
                num2 = this.serialPort.Read(buffer, num1, this.serialPort.BytesToRead);
                num1 += num2;
            }
            while (num2 != 0);
            if (num1 == 0)
                return;
            byte[] numArray1 = buffer.SelectBegin<byte>(num1);
            if (numArray1.Length < 3)
                this.LogNet?.WriteError(this.ToString(), "[" + this.serialPort.PortName + "] Uknown Data：" + numArray1.ToHexString(' '));
            else if (numArray1[0] != (byte)58)
            {
                this.LogNet?.WriteDebug(this.ToString(), "[" + this.serialPort.PortName + "] Rtu " + StringResources.Language.Receive + "：" + numArray1.ToHexString(' '));
                if (SoftCRC16.CheckCRC16(numArray1))
                {
                    byte[] numArray2 = numArray1.RemoveLast<byte>(2);
                    if (!this.CheckModbusMessageLegal(numArray2))
                        return;
                    if (this.station >= 0 && this.station != (int)numArray2[0])
                    {
                        this.LogNet?.WriteError(this.ToString(), "[" + this.serialPort.PortName + "] Station not match Modbus-rtu : " + numArray1.ToHexString(' '));
                    }
                    else
                    {
                        byte[] rtu = ModbusInfo.PackCommandToRtu(this.ReadFromModbusCore(numArray2));
                        this.serialPort.Write(rtu, 0, rtu.Length);
                        this.LogNet?.WriteDebug(this.ToString(), "[" + this.serialPort.PortName + "] Rtu " + StringResources.Language.Send + "：" + rtu.ToHexString(' '));
                        if (!this.IsStarted)
                            return;
                        this.RaiseDataReceived(sender, numArray1);
                    }
                }
                else
                    this.LogNet?.WriteWarn("[" + this.serialPort.PortName + "] CRC Check Failed : " + numArray1.ToHexString(' '));
            }
            else
            {
                this.LogNet?.WriteDebug(this.ToString(), "[" + this.serialPort.PortName + "] Ascii " + StringResources.Language.Receive + "：" + Encoding.ASCII.GetString(numArray1.RemoveLast<byte>(2)));
                OperateResult<byte[]> core = ModbusInfo.TransAsciiPackCommandToCore(numArray1);
                if (!core.IsSuccess)
                {
                    this.LogNet?.WriteError(this.ToString(), core.Message);
                }
                else
                {
                    byte[] content = core.Content;
                    if (!this.CheckModbusMessageLegal(content))
                        return;
                    if (this.station >= 0 && this.station != (int)content[0])
                    {
                        this.LogNet?.WriteError(this.ToString(), "[" + this.serialPort.PortName + "] Station not match Modbus-Ascii : " + Encoding.ASCII.GetString(numArray1.RemoveLast<byte>(2)));
                    }
                    else
                    {
                        byte[] asciiPackCommand = ModbusInfo.TransModbusCoreToAsciiPackCommand(this.ReadFromModbusCore(content));
                        this.serialPort.Write(asciiPackCommand, 0, asciiPackCommand.Length);
                        this.LogNet?.WriteDebug(this.ToString(), "[" + this.serialPort.PortName + "] Ascii " + StringResources.Language.Send + "：" + Encoding.ASCII.GetString(asciiPackCommand.RemoveLast<byte>(2)));
                        if (this.IsStarted)
                            this.RaiseDataReceived(sender, numArray1);
                    }
                }
            }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.subcriptionHybirdLock?.Dispose();
                this.subscriptions?.Clear();
                this.coilBuffer?.Dispose();
                this.inputBuffer?.Dispose();
                this.registerBuffer?.Dispose();
                this.inputRegisterBuffer?.Dispose();
                this.serialPort?.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadInt32(System.String,System.UInt16)" />
        [EstMqttApi("ReadInt32Array", "")]
        public override OperateResult<int[]> ReadInt32(string address, ushort length)
        {
            IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
            return ByteTransformHelper.GetResultFromBytes<int[]>(this.Read(address, (ushort)((int)length * (int)this.WordLength * 2)), (Func<byte[], int[]>)(m => transform.TransInt32(m, 0, (int)length)));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadUInt32(System.String,System.UInt16)" />
        [EstMqttApi("ReadUInt32Array", "")]
        public override OperateResult<uint[]> ReadUInt32(string address, ushort length)
        {
            IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
            return ByteTransformHelper.GetResultFromBytes<uint[]>(this.Read(address, (ushort)((int)length * (int)this.WordLength * 2)), (Func<byte[], uint[]>)(m => transform.TransUInt32(m, 0, (int)length)));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadFloat(System.String,System.UInt16)" />
        [EstMqttApi("ReadFloatArray", "")]
        public override OperateResult<float[]> ReadFloat(string address, ushort length)
        {
            IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
            return ByteTransformHelper.GetResultFromBytes<float[]>(this.Read(address, (ushort)((int)length * (int)this.WordLength * 2)), (Func<byte[], float[]>)(m => transform.TransSingle(m, 0, (int)length)));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadInt64(System.String,System.UInt16)" />
        [EstMqttApi("ReadInt64Array", "")]
        public override OperateResult<long[]> ReadInt64(string address, ushort length)
        {
            IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
            return ByteTransformHelper.GetResultFromBytes<long[]>(this.Read(address, (ushort)((int)length * (int)this.WordLength * 4)), (Func<byte[], long[]>)(m => transform.TransInt64(m, 0, (int)length)));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadUInt64(System.String,System.UInt16)" />
        [EstMqttApi("ReadUInt64Array", "")]
        public override OperateResult<ulong[]> ReadUInt64(string address, ushort length)
        {
            IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
            return ByteTransformHelper.GetResultFromBytes<ulong[]>(this.Read(address, (ushort)((int)length * (int)this.WordLength * 4)), (Func<byte[], ulong[]>)(m => transform.TransUInt64(m, 0, (int)length)));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadDouble(System.String,System.UInt16)" />
        [EstMqttApi("ReadDoubleArray", "")]
        public override OperateResult<double[]> ReadDouble(string address, ushort length)
        {
            IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
            return ByteTransformHelper.GetResultFromBytes<double[]>(this.Read(address, (ushort)((int)length * (int)this.WordLength * 4)), (Func<byte[], double[]>)(m => transform.TransDouble(m, 0, (int)length)));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Write(System.String,System.Int32[])" />
        [EstMqttApi("WriteInt32Array", "")]
        public override OperateResult Write(string address, int[] values)
        {
            IByteTransform transformParameter = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
            return this.Write(address, transformParameter.TransByte(values));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Write(System.String,System.UInt32[])" />
        [EstMqttApi("WriteUInt32Array", "")]
        public override OperateResult Write(string address, uint[] values)
        {
            IByteTransform transformParameter = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
            return this.Write(address, transformParameter.TransByte(values));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Write(System.String,System.Single[])" />
        [EstMqttApi("WriteFloatArray", "")]
        public override OperateResult Write(string address, float[] values)
        {
            IByteTransform transformParameter = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
            return this.Write(address, transformParameter.TransByte(values));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Write(System.String,System.Int64[])" />
        [EstMqttApi("WriteInt64Array", "")]
        public override OperateResult Write(string address, long[] values)
        {
            IByteTransform transformParameter = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
            return this.Write(address, transformParameter.TransByte(values));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Write(System.String,System.UInt64[])" />
        [EstMqttApi("WriteUInt64Array", "")]
        public override OperateResult Write(string address, ulong[] values)
        {
            IByteTransform transformParameter = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
            return this.Write(address, transformParameter.TransByte(values));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Write(System.String,System.Double[])" />
        [EstMqttApi("WriteDoubleArray", "")]
        public override OperateResult Write(string address, double[] values)
        {
            IByteTransform transformParameter = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
            return this.Write(address, transformParameter.TransByte(values));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadInt32Async(System.String,System.UInt16)" />
        public override async Task<OperateResult<int[]>> ReadInt32Async(
          string address,
          ushort length)
        {
            IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
            OperateResult<byte[]> result = await this.ReadAsync(address, (ushort)((int)length * (int)this.WordLength * 2));
            return ByteTransformHelper.GetResultFromBytes<int[]>(result, (Func<byte[], int[]>)(m => transform.TransInt32(m, 0, (int)length)));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadUInt32Async(System.String,System.UInt16)" />
        public override async Task<OperateResult<uint[]>> ReadUInt32Async(
          string address,
          ushort length)
        {
            IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
            OperateResult<byte[]> result = await this.ReadAsync(address, (ushort)((int)length * (int)this.WordLength * 2));
            return ByteTransformHelper.GetResultFromBytes<uint[]>(result, (Func<byte[], uint[]>)(m => transform.TransUInt32(m, 0, (int)length)));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadFloatAsync(System.String,System.UInt16)" />
        public override async Task<OperateResult<float[]>> ReadFloatAsync(
          string address,
          ushort length)
        {
            IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
            OperateResult<byte[]> result = await this.ReadAsync(address, (ushort)((int)length * (int)this.WordLength * 2));
            return ByteTransformHelper.GetResultFromBytes<float[]>(result, (Func<byte[], float[]>)(m => transform.TransSingle(m, 0, (int)length)));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadInt64Async(System.String,System.UInt16)" />
        public override async Task<OperateResult<long[]>> ReadInt64Async(
          string address,
          ushort length)
        {
            IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
            OperateResult<byte[]> result = await this.ReadAsync(address, (ushort)((int)length * (int)this.WordLength * 4));
            return ByteTransformHelper.GetResultFromBytes<long[]>(result, (Func<byte[], long[]>)(m => transform.TransInt64(m, 0, (int)length)));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadUInt64Async(System.String,System.UInt16)" />
        public override async Task<OperateResult<ulong[]>> ReadUInt64Async(
          string address,
          ushort length)
        {
            IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
            OperateResult<byte[]> result = await this.ReadAsync(address, (ushort)((int)length * (int)this.WordLength * 4));
            return ByteTransformHelper.GetResultFromBytes<ulong[]>(result, (Func<byte[], ulong[]>)(m => transform.TransUInt64(m, 0, (int)length)));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadDoubleAsync(System.String,System.UInt16)" />
        public override async Task<OperateResult<double[]>> ReadDoubleAsync(
          string address,
          ushort length)
        {
            IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
            OperateResult<byte[]> result = await this.ReadAsync(address, (ushort)((int)length * (int)this.WordLength * 4));
            return ByteTransformHelper.GetResultFromBytes<double[]>(result, (Func<byte[], double[]>)(m => transform.TransDouble(m, 0, (int)length)));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.WriteAsync(System.String,System.Int32[])" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          int[] values)
        {
            IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
            OperateResult operateResult = await this.WriteAsync(address, transform.TransByte(values));
            transform = (IByteTransform)null;
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.WriteAsync(System.String,System.UInt32[])" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          uint[] values)
        {
            IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
            OperateResult operateResult = await this.WriteAsync(address, transform.TransByte(values));
            transform = (IByteTransform)null;
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.WriteAsync(System.String,System.Single[])" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          float[] values)
        {
            IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
            OperateResult operateResult = await this.WriteAsync(address, transform.TransByte(values));
            transform = (IByteTransform)null;
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.WriteAsync(System.String,System.Int64[])" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          long[] values)
        {
            IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
            OperateResult operateResult = await this.WriteAsync(address, transform.TransByte(values));
            transform = (IByteTransform)null;
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.WriteAsync(System.String,System.UInt64[])" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          ulong[] values)
        {
            IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
            OperateResult operateResult = await this.WriteAsync(address, transform.TransByte(values));
            transform = (IByteTransform)null;
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.WriteAsync(System.String,System.Double[])" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          double[] values)
        {
            IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
            OperateResult operateResult = await this.WriteAsync(address, transform.TransByte(values));
            transform = (IByteTransform)null;
            return operateResult;
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("ModbusTcpServer[{0}]", (object)this.Port);
    }
}
