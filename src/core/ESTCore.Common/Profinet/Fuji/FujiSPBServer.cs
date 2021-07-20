// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Fuji.FujiSPBServer
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;
using ESTCore.Common.Core;
using ESTCore.Common.Core.IMessage;
using ESTCore.Common.Core.Net;
using ESTCore.Common.Reflection;

using System;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ESTCore.Common.Profinet.Fuji
{
    /// <summary>
    /// <b>[商业授权]</b> 富士的SPB虚拟的PLC，线圈支持X,Y,M的读写，其中X只能远程读，寄存器支持D,R,W的读写操作。<br />
    /// <b>[Authorization]</b> Fuji's SPB virtual PLC, the coil supports X, Y, M read and write,
    /// X can only be read remotely, and the register supports D, R, W read and write operations.
    /// </summary>
    public class FujiSPBServer : NetworkDataServerBase
    {
        private SerialPort serialPort;
        private SoftBuffer xBuffer;
        private SoftBuffer yBuffer;
        private SoftBuffer mBuffer;
        private SoftBuffer dBuffer;
        private SoftBuffer rBuffer;
        private SoftBuffer wBuffer;
        private const int DataPoolLength = 65536;
        private int station = 1;

        /// <summary>实例化一个富士SPB的网口和串口服务器，支持数据读写操作</summary>
        public FujiSPBServer()
        {
            this.xBuffer = new SoftBuffer(65536);
            this.yBuffer = new SoftBuffer(65536);
            this.mBuffer = new SoftBuffer(65536);
            this.dBuffer = new SoftBuffer(131072);
            this.rBuffer = new SoftBuffer(131072);
            this.wBuffer = new SoftBuffer(131072);
            this.ByteTransform = (IByteTransform)new RegularByteTransform();
            this.ByteTransform.DataFormat = DataFormat.CDAB;
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

        /// <inheritdoc cref="P:ESTCore.Common.Profinet.Fuji.FujiSPBOverTcp.Station" />
        public int Station
        {
            get => this.station;
            set => this.station = value;
        }

        /// <inheritdoc />
        protected override byte[] SaveToBytes()
        {
            byte[] numArray = new byte[589824];
            this.xBuffer.GetBytes().CopyTo((Array)numArray, 0);
            this.yBuffer.GetBytes().CopyTo((Array)numArray, 65536);
            this.mBuffer.GetBytes().CopyTo((Array)numArray, 131072);
            this.dBuffer.GetBytes().CopyTo((Array)numArray, 196608);
            this.rBuffer.GetBytes().CopyTo((Array)numArray, 327680);
            this.wBuffer.GetBytes().CopyTo((Array)numArray, 458752);
            return numArray;
        }

        /// <inheritdoc />
        protected override void LoadFromBytes(byte[] content)
        {
            if (content.Length < 589824)
                throw new Exception("File is not correct");
            this.xBuffer.SetBytes(content, 0, 65536);
            this.yBuffer.SetBytes(content, 65536, 65536);
            this.mBuffer.SetBytes(content, 131072, 65536);
            this.dBuffer.SetBytes(content, 196608, 131072);
            this.rBuffer.SetBytes(content, 327680, 131072);
            this.wBuffer.SetBytes(content, 458752, 131072);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Fuji.FujiSPBOverTcp.Read(System.String,System.UInt16)" />
        [EstMqttApi("ReadByteArray", "")]
        public override OperateResult<byte[]> Read(string address, ushort length)
        {
            OperateResult<byte[]> operateResult = new OperateResult<byte[]>();
            try
            {
                switch (address[0])
                {
                    case 'D':
                    case 'd':
                        return OperateResult.CreateSuccessResult<byte[]>(this.dBuffer.GetBytes(Convert.ToInt32(address.Substring(1)) * 2, (int)length * 2));
                    case 'M':
                    case 'm':
                        return OperateResult.CreateSuccessResult<byte[]>(this.mBuffer.GetBytes(Convert.ToInt32(address.Substring(1)) * 2, (int)length * 2));
                    case 'R':
                    case 'r':
                        return OperateResult.CreateSuccessResult<byte[]>(this.rBuffer.GetBytes(Convert.ToInt32(address.Substring(1)) * 2, (int)length * 2));
                    case 'W':
                    case 'w':
                        return OperateResult.CreateSuccessResult<byte[]>(this.wBuffer.GetBytes(Convert.ToInt32(address.Substring(1)) * 2, (int)length * 2));
                    case 'X':
                    case 'x':
                        return OperateResult.CreateSuccessResult<byte[]>(this.xBuffer.GetBytes(Convert.ToInt32(address.Substring(1)) * 2, (int)length * 2));
                    case 'Y':
                    case 'y':
                        return OperateResult.CreateSuccessResult<byte[]>(this.yBuffer.GetBytes(Convert.ToInt32(address.Substring(1)) * 2, (int)length * 2));
                    default:
                        throw new Exception(StringResources.Language.NotSupportedDataType);
                }
            }
            catch (Exception ex)
            {
                operateResult.Message = ex.Message;
                return operateResult;
            }
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Fuji.FujiSPBOverTcp.Write(System.String,System.Byte[])" />
        [EstMqttApi("WriteByteArray", "")]
        public override OperateResult Write(string address, byte[] value)
        {
            OperateResult<byte[]> operateResult = new OperateResult<byte[]>();
            try
            {
                switch (address[0])
                {
                    case 'D':
                    case 'd':
                        this.dBuffer.SetBytes(value, Convert.ToInt32(address.Substring(1)) * 2);
                        return OperateResult.CreateSuccessResult();
                    case 'M':
                    case 'm':
                        this.mBuffer.SetBytes(value, Convert.ToInt32(address.Substring(1)) * 2);
                        return OperateResult.CreateSuccessResult();
                    case 'R':
                    case 'r':
                        this.rBuffer.SetBytes(value, Convert.ToInt32(address.Substring(1)) * 2);
                        return OperateResult.CreateSuccessResult();
                    case 'W':
                    case 'w':
                        this.wBuffer.SetBytes(value, Convert.ToInt32(address.Substring(1)) * 2);
                        return OperateResult.CreateSuccessResult();
                    case 'X':
                    case 'x':
                        this.xBuffer.SetBytes(value, Convert.ToInt32(address.Substring(1)) * 2);
                        return OperateResult.CreateSuccessResult();
                    case 'Y':
                    case 'y':
                        this.yBuffer.SetBytes(value, Convert.ToInt32(address.Substring(1)) * 2);
                        return OperateResult.CreateSuccessResult();
                    default:
                        throw new Exception(StringResources.Language.NotSupportedDataType);
                }
            }
            catch (Exception ex)
            {
                operateResult.Message = ex.Message;
                return (OperateResult)operateResult;
            }
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Fuji.FujiSPBOverTcp.ReadBool(System.String,System.UInt16)" />
        [EstMqttApi("ReadBoolArray", "")]
        public override OperateResult<bool[]> ReadBool(string address, ushort length)
        {
            try
            {
                int destIndex = 0;
                if (address.LastIndexOf('.') > 0)
                {
                    int indexInformation = EstHelper.GetBitIndexInformation(ref address);
                    destIndex = Convert.ToInt32(address.Substring(1)) * 16 + indexInformation;
                }
                else if (address[0] == 'X' || address[0] == 'x' || (address[0] == 'Y' || address[0] == 'y') || address[0] == 'M' || address[0] == 'm')
                    destIndex = Convert.ToInt32(address.Substring(1));
                switch (address[0])
                {
                    case 'D':
                    case 'd':
                        return OperateResult.CreateSuccessResult<bool[]>(this.dBuffer.GetBool(destIndex, (int)length));
                    case 'M':
                    case 'm':
                        return OperateResult.CreateSuccessResult<bool[]>(this.mBuffer.GetBool(destIndex, (int)length));
                    case 'R':
                    case 'r':
                        return OperateResult.CreateSuccessResult<bool[]>(this.rBuffer.GetBool(destIndex, (int)length));
                    case 'W':
                    case 'w':
                        return OperateResult.CreateSuccessResult<bool[]>(this.wBuffer.GetBool(destIndex, (int)length));
                    case 'X':
                    case 'x':
                        return OperateResult.CreateSuccessResult<bool[]>(this.xBuffer.GetBool(destIndex, (int)length));
                    case 'Y':
                    case 'y':
                        return OperateResult.CreateSuccessResult<bool[]>(this.yBuffer.GetBool(destIndex, (int)length));
                    default:
                        throw new Exception(StringResources.Language.NotSupportedDataType);
                }
            }
            catch (Exception ex)
            {
                return new OperateResult<bool[]>(ex.Message);
            }
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkDeviceBase.Write(System.String,System.Boolean[])" />
        [EstMqttApi("WriteBoolArray", "")]
        public override OperateResult Write(string address, bool[] value)
        {
            try
            {
                int destIndex = 0;
                if (address.LastIndexOf('.') > 0)
                {
                    EstHelper.GetBitIndexInformation(ref address);
                    destIndex = Convert.ToInt32(address.Substring(1)) * 16 + destIndex;
                }
                else if (address[0] == 'X' || address[0] == 'x' || (address[0] == 'Y' || address[0] == 'y') || address[0] == 'M' || address[0] == 'm')
                    destIndex = Convert.ToInt32(address.Substring(1));
                switch (address[0])
                {
                    case 'D':
                    case 'd':
                        this.dBuffer.SetBool(value, destIndex);
                        return OperateResult.CreateSuccessResult();
                    case 'M':
                    case 'm':
                        this.mBuffer.SetBool(value, destIndex);
                        return OperateResult.CreateSuccessResult();
                    case 'R':
                    case 'r':
                        this.rBuffer.SetBool(value, destIndex);
                        return OperateResult.CreateSuccessResult();
                    case 'W':
                    case 'w':
                        this.wBuffer.SetBool(value, destIndex);
                        return OperateResult.CreateSuccessResult();
                    case 'X':
                    case 'x':
                        this.xBuffer.SetBool(value, destIndex);
                        return OperateResult.CreateSuccessResult();
                    case 'Y':
                    case 'y':
                        this.yBuffer.SetBool(value, destIndex);
                        return OperateResult.CreateSuccessResult();
                    default:
                        throw new Exception(StringResources.Language.NotSupportedDataType);
                }
            }
            catch (Exception ex)
            {
                return (OperateResult)new OperateResult<bool[]>(ex.Message);
            }
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
                OperateResult<byte[]> read1 = await this.ReceiveByMessageAsync(session.WorkSocket, 2000, (INetMessage)new FujiSPBMessage());
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
                else if (read1.Content[0] != (byte)58)
                {
                    this.RemoveClient(session);
                    session = (AppSession)null;
                }
                else
                {
                    this.LogNet?.WriteDebug(this.ToString(), string.Format("[{0}] Tcp {1}：{2}", (object)session.IpEndPoint, (object)StringResources.Language.Receive, (object)Encoding.ASCII.GetString(read1.Content.RemoveLast<byte>(2))));
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
                        this.LogNet?.WriteDebug(this.ToString(), string.Format("[{0}] Tcp {1}：{2}", (object)session.IpEndPoint, (object)StringResources.Language.Send, (object)Encoding.ASCII.GetString(back.RemoveLast<byte>(2))));
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

        private byte[] CreateResponseBack(byte err, string command, byte[] data, bool addLength = true)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(':');
            stringBuilder.Append(this.Station.ToString("X2"));
            stringBuilder.Append("00");
            stringBuilder.Append(command.Substring(9, 4));
            stringBuilder.Append(err.ToString("X2"));
            if (err == (byte)0 && data != null)
            {
                if (addLength)
                    stringBuilder.Append(FujiSPBOverTcp.AnalysisIntegerAddress(data.Length / 2));
                stringBuilder.Append(data.ToHexString());
            }
            stringBuilder[3] = ((stringBuilder.Length - 5) / 2).ToString("X2")[0];
            stringBuilder[4] = ((stringBuilder.Length - 5) / 2).ToString("X2")[1];
            stringBuilder.Append("\r\n");
            return Encoding.ASCII.GetBytes(stringBuilder.ToString());
        }

        private int AnalysisAddress(string address) => Convert.ToInt32(address.Substring(2) + address.Substring(0, 2));

        private byte[] ReadFromSPBCore(byte[] receive)
        {
            if (receive.Length < 15)
                return (byte[])null;
            if (receive[receive.Length - 2] == (byte)13 && receive[receive.Length - 1] == (byte)10)
                receive = receive.RemoveLast<byte>(2);
            string command = Encoding.ASCII.GetString(receive);
            if (Convert.ToInt32(command.Substring(3, 2), 16) != (command.Length - 5) / 2)
                return this.CreateResponseBack((byte)3, command, (byte[])null);
            if (command.Substring(9, 4) == "0000")
                return this.ReadByCommand(command);
            if (command.Substring(9, 4) == "0100")
                return this.WriteByCommand(command);
            return command.Substring(9, 4) == "0102" ? this.WriteBitByCommand(command) : (byte[])null;
        }

        private byte[] ReadByCommand(string command)
        {
            string str = command.Substring(13, 2);
            int num1 = this.AnalysisAddress(command.Substring(15, 4));
            int num2 = this.AnalysisAddress(command.Substring(19, 4));
            if (num2 > 105)
                this.CreateResponseBack((byte)3, command, (byte[])null);
            if (str == "0C")
                return this.CreateResponseBack((byte)0, command, this.dBuffer.GetBytes(num1 * 2, num2 * 2));
            if (str == "0D")
                return this.CreateResponseBack((byte)0, command, this.rBuffer.GetBytes(num1 * 2, num2 * 2));
            if (str == "0E")
                return this.CreateResponseBack((byte)0, command, this.wBuffer.GetBytes(num1 * 2, num2 * 2));
            if (str == "01")
                return this.CreateResponseBack((byte)0, command, this.xBuffer.GetBytes(num1 * 2, num2 * 2));
            if (str == "00")
                return this.CreateResponseBack((byte)0, command, this.yBuffer.GetBytes(num1 * 2, num2 * 2));
            return str == "02" ? this.CreateResponseBack((byte)0, command, this.mBuffer.GetBytes(num1 * 2, num2 * 2)) : this.CreateResponseBack((byte)2, command, (byte[])null);
        }

        private byte[] WriteByCommand(string command)
        {
            if (!this.EnableWrite)
                return this.CreateResponseBack((byte)2, command, (byte[])null);
            string str = command.Substring(13, 2);
            int num = this.AnalysisAddress(command.Substring(15, 4));
            if (this.AnalysisAddress(command.Substring(19, 4)) * 4 != command.Length - 23)
                return this.CreateResponseBack((byte)3, command, (byte[])null);
            byte[] hexBytes = command.Substring(23).ToHexBytes();
            if (str == "0C")
            {
                this.dBuffer.SetBytes(hexBytes, num * 2);
                return this.CreateResponseBack((byte)0, command, (byte[])null);
            }
            if (str == "0D")
            {
                this.rBuffer.SetBytes(hexBytes, num * 2);
                return this.CreateResponseBack((byte)0, command, (byte[])null);
            }
            if (str == "0E")
            {
                this.wBuffer.SetBytes(hexBytes, num * 2);
                return this.CreateResponseBack((byte)0, command, (byte[])null);
            }
            if (str == "00")
            {
                this.yBuffer.SetBytes(hexBytes, num * 2);
                return this.CreateResponseBack((byte)0, command, (byte[])null);
            }
            if (!(str == "02"))
                return this.CreateResponseBack((byte)2, command, (byte[])null);
            this.mBuffer.SetBytes(hexBytes, num * 2);
            return this.CreateResponseBack((byte)0, command, (byte[])null);
        }

        private byte[] WriteBitByCommand(string command)
        {
            if (!this.EnableWrite)
                return this.CreateResponseBack((byte)2, command, (byte[])null);
            string str = command.Substring(13, 2);
            int num = this.AnalysisAddress(command.Substring(15, 4));
            int int32 = Convert.ToInt32(command.Substring(19, 2));
            bool flag = command.Substring(21, 2) != "00";
            if (str == "0C")
            {
                this.dBuffer.SetBool(flag, num * 8 + int32);
                return this.CreateResponseBack((byte)0, command, (byte[])null);
            }
            if (str == "0D")
            {
                this.rBuffer.SetBool(flag, num * 8 + int32);
                return this.CreateResponseBack((byte)0, command, (byte[])null);
            }
            if (str == "0E")
            {
                this.wBuffer.SetBool(flag, num * 8 + int32);
                return this.CreateResponseBack((byte)0, command, (byte[])null);
            }
            if (str == "00")
            {
                this.yBuffer.SetBool(flag, num * 8 + int32);
                return this.CreateResponseBack((byte)0, command, (byte[])null);
            }
            if (!(str == "02"))
                return this.CreateResponseBack((byte)2, command, (byte[])null);
            this.mBuffer.SetBool(flag, num * 8 + int32);
            return this.CreateResponseBack((byte)0, command, (byte[])null);
        }

        /// <summary>
        /// 启动SPB串口的从机服务，使用默认的参数进行初始化串口，9600波特率，8位数据位，无奇偶校验，1位停止位<br />
        /// Start the slave service of modbus-rtu, initialize the serial port with default parameters, 9600 baud rate, 8 data bits, no parity, 1 stop bit
        /// </summary>
        /// <param name="com">串口信息</param>
        public void StartSerial(string com) => this.StartSerial(com, 9600);

        /// <summary>
        /// 启动SPB串口的从机服务，使用默认的参数进行初始化串口，8位数据位，无奇偶校验，1位停止位<br />
        /// Start the slave service of modbus-rtu, initialize the serial port with default parameters, 8 data bits, no parity, 1 stop bit
        /// </summary>
        /// <param name="com">串口信息</param>
        /// <param name="baudRate">波特率</param>
        public void StartSerial(string com, int baudRate) => this.StartSerial((Action<SerialPort>)(sp =>
       {
           sp.PortName = com;
           sp.BaudRate = baudRate;
           sp.DataBits = 8;
           sp.Parity = Parity.None;
           sp.StopBits = StopBits.One;
       }));

        /// <summary>
        /// 启动SPB串口的从机服务，使用自定义的初始化方法初始化串口的参数<br />
        /// Start the slave service of modbus-rtu and initialize the parameters of the serial port using a custom initialization method
        /// </summary>
        /// <param name="inni">初始化信息的委托</param>
        public void StartSerial(Action<SerialPort> inni)
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
        /// 关闭SPB的串口对象<br />
        /// Close the serial port object of modbus-rtu
        /// </summary>
        public void CloseSerial()
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
            byte[] buffer1 = new byte[1024];
            int num2;
            do
            {
                Thread.Sleep(20);
                num2 = this.serialPort.Read(buffer1, num1, this.serialPort.BytesToRead);
                num1 += num2;
            }
            while (num2 != 0);
            if (num1 == 0)
                return;
            byte[] numArray = buffer1.SelectBegin<byte>(num1);
            if (numArray.Length < 5)
            {
                this.LogNet?.WriteError(this.ToString(), "[" + this.serialPort.PortName + "] Uknown Data：" + numArray.ToHexString(' '));
            }
            else
            {
                if (numArray[0] != (byte)58)
                    return;
                this.LogNet?.WriteDebug(this.ToString(), "[" + this.serialPort.PortName + "] Ascii " + StringResources.Language.Receive + "：" + Encoding.ASCII.GetString(numArray.RemoveLast<byte>(2)));
                if (Encoding.ASCII.GetString(numArray, 1, 2) != this.station.ToString("X2"))
                {
                    this.LogNet?.WriteDebug(this.ToString(), string.Format("[{0}] Station not match , Except: {1:X2} , Actual: {2}", (object)this.serialPort.PortName, (object)this.station, (object)Encoding.ASCII.GetString(numArray, 1, 2)));
                }
                else
                {
                    byte[] buffer2 = this.ReadFromSPBCore(numArray);
                    if (buffer2 == null)
                        return;
                    this.serialPort.Write(buffer2, 0, buffer2.Length);
                    this.LogNet?.WriteDebug(this.ToString(), "[" + this.serialPort.PortName + "] Ascii " + StringResources.Language.Send + "：" + Encoding.ASCII.GetString(buffer2.RemoveLast<byte>(2)));
                    if (!this.IsStarted)
                        return;
                    this.RaiseDataReceived(sender, numArray);
                }
            }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.xBuffer.Dispose();
                this.yBuffer.Dispose();
                this.mBuffer.Dispose();
                this.dBuffer.Dispose();
                this.rBuffer.Dispose();
                this.wBuffer.Dispose();
                this.serialPort?.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("FujiSPBServer[{0}]", (object)this.Port);
    }
}
