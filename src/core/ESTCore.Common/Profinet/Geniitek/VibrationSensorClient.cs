// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Geniitek.VibrationSensorClient
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;
using ESTCore.Common.Core;
using ESTCore.Common.Core.Net;
using ESTCore.Common.LogNet;
using ESTCore.Common.Reflection;

using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ESTCore.Common.Profinet.Geniitek
{
    /// <summary>Geniitek-VB31 型号的智能无线振动传感器，来自苏州捷杰传感器技术有限公司</summary>
    public class VibrationSensorClient : NetworkXBase
    {
        private int isReConnectServer = 0;
        private bool closed = false;
        private string ipAddress = string.Empty;
        private int port = 1883;
        private int connectTimeOut = 10000;
        private Timer timerCheck;
        private DateTime activeTime = DateTime.Now;
        private int checkSeconds = 60;
        private int CheckTimeoutCount = 0;
        private ushort address = 1;
        private IByteTransform byteTransform;

        /// <summary>使用指定的ip，端口来实例化一个默认的对象</summary>
        /// <param name="ipAddress">Ip地址信息</param>
        /// <param name="port">端口号信息</param>
        public VibrationSensorClient(string ipAddress = "192.168.1.1", int port = 3001)
        {
            this.ipAddress = EstHelper.GetIpAddressFromInput(ipAddress);
            this.port = port;
            this.byteTransform = (IByteTransform)new ReverseBytesTransform();
        }

        /// <summary>
        /// 连接服务器，实例化客户端之后，至少要调用成功一次，如果返回失败，那些请过一段时间后重新调用本方法连接。<br />
        /// After connecting to the server, the client must be called at least once after instantiating the client.
        /// If the return fails, please call this method to connect again after a period of time.
        /// </summary>
        /// <returns>连接是否成功</returns>
        public OperateResult ConnectServer()
        {
            this.CoreSocket?.Close();
            OperateResult<Socket> socketAndConnect = this.CreateSocketAndConnect(this.ipAddress, this.port, this.connectTimeOut);
            if (!socketAndConnect.IsSuccess)
                return (OperateResult)socketAndConnect;
            this.CoreSocket = socketAndConnect.Content;
            try
            {
                this.CoreSocket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, new AsyncCallback(this.ReceiveAsyncCallback), (object)this.CoreSocket);
            }
            catch (Exception ex)
            {
                return new OperateResult(ex.Message);
            }
            this.closed = false;
            VibrationSensorClient.OnClientConnectedDelegate onClientConnected = this.OnClientConnected;
            if (onClientConnected != null)
                onClientConnected();
            this.timerCheck?.Dispose();
            this.timerCheck = new Timer(new TimerCallback(this.TimerCheckServer), (object)null, 2000, 5000);
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 关闭Mqtt服务器的连接。<br />
        /// Close the connection to the Mqtt server.
        /// </summary>
        public void ConnectClose()
        {
            if (this.closed)
                return;
            this.closed = true;
            Thread.Sleep(20);
            this.CoreSocket?.Close();
            this.timerCheck?.Dispose();
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Geniitek.VibrationSensorClient.ConnectServer" />
        public async Task<OperateResult> ConnectServerAsync()
        {
            this.CoreSocket?.Close();
            OperateResult<Socket> connect = await this.CreateSocketAndConnectAsync(this.ipAddress, this.port, this.connectTimeOut);
            if (!connect.IsSuccess)
                return (OperateResult)connect;
            this.CoreSocket = connect.Content;
            try
            {
                this.CoreSocket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, new AsyncCallback(this.ReceiveAsyncCallback), (object)this.CoreSocket);
            }
            catch (Exception ex)
            {
                return new OperateResult(ex.Message);
            }
            this.closed = false;
            VibrationSensorClient.OnClientConnectedDelegate onClientConnected = this.OnClientConnected;
            if (onClientConnected != null)
                onClientConnected();
            this.timerCheck?.Dispose();
            this.timerCheck = new Timer(new TimerCallback(this.TimerCheckServer), (object)null, 2000, 5000);
            return OperateResult.CreateSuccessResult();
        }

        private void OnVibrationSensorClientNetworkError()
        {
            if (this.closed)
                return;
            if (Interlocked.CompareExchange(ref this.isReConnectServer, 1, 0) != 0)
                return;
            try
            {
                if (this.OnNetworkError == null)
                {
                    this.LogNet?.WriteInfo("The network is abnormal, and the system is ready to automatically reconnect after 10 seconds.");
                    while (true)
                    {
                        for (int index = 0; index < 10; ++index)
                        {
                            Thread.Sleep(1000);
                            this.LogNet?.WriteInfo(string.Format("Wait for {0} second to connect to the server ...", (object)(10 - index)));
                        }
                        if (!this.ConnectServer().IsSuccess)
                            this.LogNet?.WriteInfo("The connection failed. Prepare to reconnect after 10 seconds.");
                        else
                            break;
                    }
                    this.LogNet?.WriteInfo("Successfully connected to the server!");
                }
                else
                {
                    EventHandler onNetworkError = this.OnNetworkError;
                    if (onNetworkError != null)
                        onNetworkError((object)this, new EventArgs());
                }
                this.activeTime = DateTime.Now;
                Interlocked.Exchange(ref this.isReConnectServer, 0);
            }
            catch
            {
                Interlocked.Exchange(ref this.isReConnectServer, 0);
                throw;
            }
        }

        private async void ReceiveAsyncCallback(IAsyncResult ar)
        {
            if (!(ar.AsyncState is Socket socket))
            {
                socket = (Socket)null;
            }
            else
            {
                try
                {
                    socket.EndReceive(ar);
                }
                catch (ObjectDisposedException ex)
                {
                    socket?.Close();
                    ILogNet logNet = this.LogNet;
                    if (logNet == null)
                    {
                        socket = (Socket)null;
                        return;
                    }
                    logNet.WriteDebug(this.ToString(), "Closed");
                    socket = (Socket)null;
                    return;
                }
                catch (Exception ex)
                {
                    socket?.Close();
                    this.LogNet?.WriteDebug(this.ToString(), "ReceiveCallback Failed:" + ex.Message);
                    this.OnVibrationSensorClientNetworkError();
                    socket = (Socket)null;
                    return;
                }
                if (this.closed)
                {
                    ILogNet logNet = this.LogNet;
                    if (logNet == null)
                    {
                        socket = (Socket)null;
                    }
                    else
                    {
                        logNet.WriteDebug(this.ToString(), "Closed");
                        socket = (Socket)null;
                    }
                }
                else
                {
                    OperateResult<byte[]> read = await this.ReceiveAsync(socket, 9);
                    if (!read.IsSuccess)
                    {
                        this.OnVibrationSensorClientNetworkError();
                        socket = (Socket)null;
                    }
                    else
                    {
                        if (read.Content[0] == (byte)170 && read.Content[1] == (byte)85 && read.Content[2] == (byte)127 && read.Content[7] == (byte)0)
                        {
                            OperateResult<byte[]> read2 = await this.ReceiveAsync(socket, 3);
                            if (!read2.IsSuccess)
                            {
                                this.OnVibrationSensorClientNetworkError();
                                socket = (Socket)null;
                                return;
                            }
                            int length = (int)read2.Content[1] * 256 + (int)read2.Content[2];
                            OperateResult<byte[]> read3 = await this.ReceiveAsync(socket, length + 4);
                            if (!read3.IsSuccess)
                            {
                                this.OnVibrationSensorClientNetworkError();
                                socket = (Socket)null;
                                return;
                            }
                            if (read.Content[5] == (byte)1)
                            {
                                this.Address = this.byteTransform.TransUInt16(read.Content, 3);
                                this.LogNet?.WriteDebug("Receive: " + SoftBasic.SpliceArray<byte>(read.Content, read2.Content, read3.Content).ToHexString(' '));
                                VibrationSensorPeekValue peekValue = new VibrationSensorPeekValue();
                                peekValue.AcceleratedSpeedX = (float)BitConverter.ToInt16(read3.Content, 0) / 100f;
                                peekValue.AcceleratedSpeedY = (float)BitConverter.ToInt16(read3.Content, 2) / 100f;
                                peekValue.AcceleratedSpeedZ = (float)BitConverter.ToInt16(read3.Content, 4) / 100f;
                                peekValue.SpeedX = (float)BitConverter.ToInt16(read3.Content, 6) / 100f;
                                peekValue.SpeedY = (float)BitConverter.ToInt16(read3.Content, 8) / 100f;
                                peekValue.SpeedZ = (float)BitConverter.ToInt16(read3.Content, 10) / 100f;
                                peekValue.OffsetX = (int)BitConverter.ToInt16(read3.Content, 12);
                                peekValue.OffsetY = (int)BitConverter.ToInt16(read3.Content, 14);
                                peekValue.OffsetZ = (int)BitConverter.ToInt16(read3.Content, 16);
                                peekValue.Temperature = (float)((double)BitConverter.ToInt16(read3.Content, 18) * 0.0199999995529652 - 273.149993896484);
                                peekValue.Voltage = (float)BitConverter.ToInt16(read3.Content, 20) / 100f;
                                peekValue.SendingInterval = BitConverter.ToInt32(read3.Content, 22);
                                VibrationSensorClient.OnPeekValueReceiveDelegate peekValueReceive = this.OnPeekValueReceive;
                                if (peekValueReceive != null)
                                    peekValueReceive(peekValue);
                                peekValue = (VibrationSensorPeekValue)null;
                            }
                            read2 = (OperateResult<byte[]>)null;
                            read3 = (OperateResult<byte[]>)null;
                        }
                        else if (read.Content[0] == (byte)170)
                        {
                            VibrationSensorActualValue actualValue = new VibrationSensorActualValue()
                            {
                                AcceleratedSpeedX = (float)this.byteTransform.TransInt16(read.Content, 1) / 100f,
                                AcceleratedSpeedY = (float)this.byteTransform.TransInt16(read.Content, 3) / 100f,
                                AcceleratedSpeedZ = (float)this.byteTransform.TransInt16(read.Content, 5) / 100f
                            };
                            VibrationSensorClient.OnActualValueReceiveDelegate actualValueReceive = this.OnActualValueReceive;
                            if (actualValueReceive != null)
                                actualValueReceive(actualValue);
                        }
                        else
                        {
                            OperateResult<byte[]> read2 = await this.ReceiveAsync(socket, 9);
                            if (!read2.IsSuccess)
                            {
                                this.OnVibrationSensorClientNetworkError();
                                socket = (Socket)null;
                                return;
                            }
                            byte[] array = SoftBasic.SpliceArray<byte>(read.Content, read2.Content);
                            for (int i = 0; i < array.Length; ++i)
                            {
                                if (array[i] == (byte)170)
                                {
                                    if (i < 9)
                                    {
                                        if (array[i + 9] == (byte)170)
                                        {
                                            OperateResult<byte[]> async = await this.ReceiveAsync(socket, i);
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        OperateResult<byte[]> async = await this.ReceiveAsync(socket, i - 9);
                                        break;
                                    }
                                }
                            }
                            read2 = (OperateResult<byte[]>)null;
                            array = (byte[])null;
                        }
                        this.activeTime = DateTime.Now;
                        try
                        {
                            socket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, new AsyncCallback(this.ReceiveAsyncCallback), (object)socket);
                        }
                        catch (Exception ex)
                        {
                            socket?.Close();
                            this.LogNet?.WriteDebug(this.ToString(), "BeginReceive Failed:" + ex.Message);
                            this.OnVibrationSensorClientNetworkError();
                        }
                        read = (OperateResult<byte[]>)null;
                        socket = (Socket)null;
                    }
                }
            }
        }

        private void TimerCheckServer(object obj)
        {
            if (this.CoreSocket == null || this.closed)
                return;
            if ((DateTime.Now - this.activeTime).TotalSeconds > (double)this.checkSeconds)
            {
                if (this.CheckTimeoutCount == 0)
                    this.LogNet?.WriteDebug(StringResources.Language.NetHeartCheckTimeout);
                this.CheckTimeoutCount = 1;
                this.OnVibrationSensorClientNetworkError();
            }
            else
                this.CheckTimeoutCount = 0;
        }

        private OperateResult SendPre(byte[] send)
        {
            this.LogNet?.WriteDebug("Send " + send.ToHexString(' '));
            return this.Send(this.CoreSocket, send);
        }

        /// <summary>
        /// 设置读取震动传感器的状态数据<br />
        /// Set to read the status data of the shock sensor
        /// </summary>
        /// <returns>是否发送成功</returns>
        [EstMqttApi]
        public OperateResult SetReadStatus() => this.SendPre(VibrationSensorClient.BulidLongMessage(this.address, (byte)1, (byte[])null));

        /// <summary>
        /// 设置读取震动传感器的实时加速度<br />
        /// Set the real-time acceleration of the vibration sensor
        /// </summary>
        /// <returns>是否发送成功</returns>
        [EstMqttApi]
        public OperateResult SetReadActual() => this.SendPre(VibrationSensorClient.BulidLongMessage(this.address, (byte)2, (byte[])null));

        /// <summary>
        /// 设置当前的震动传感器的数据发送间隔为指定的时间，单位为秒<br />
        /// Set the current vibration sensor data transmission interval to the specified time in seconds
        /// </summary>
        /// <param name="seconds">时间信息，单位为秒</param>
        /// <returns>是否发送成功</returns>
        [EstMqttApi]
        public OperateResult SetReadStatusInterval(int seconds)
        {
            byte[] data = new byte[6]
            {
        BitConverter.GetBytes(this.address)[0],
        BitConverter.GetBytes(this.address)[1],
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
            };
            BitConverter.GetBytes(seconds).CopyTo((Array)data, 2);
            return this.SendPre(VibrationSensorClient.BulidLongMessage(this.address, (byte)16, data));
        }

        /// <summary>
        ///  接收到震动传感器峰值数据时触发<br />
        ///  Triggered when peak data of vibration sensor is received
        /// </summary>
        public event VibrationSensorClient.OnPeekValueReceiveDelegate OnPeekValueReceive;

        /// <summary>
        ///  接收到震动传感器实时数据时触发<br />
        ///  Triggered when real-time data from shock sensor is received
        /// </summary>
        public event VibrationSensorClient.OnActualValueReceiveDelegate OnActualValueReceive;

        /// <summary>
        /// 当客户端连接成功触发事件，就算是重新连接服务器后，也是会触发的<br />
        /// The event is triggered when the client is connected successfully, even after reconnecting to the server.
        /// </summary>
        public event VibrationSensorClient.OnClientConnectedDelegate OnClientConnected;

        /// <summary>当网络发生异常的时候触发的事件，用户应该在事件里进行重连服务器</summary>
        public event EventHandler OnNetworkError;

        /// <summary>
        /// 获取或设置当前客户端的连接超时时间，默认10,000毫秒，单位ms<br />
        /// Gets or sets the connection timeout of the current client. The default is 10,000 milliseconds. The unit is ms.
        /// </summary>
        public int ConnectTimeOut
        {
            get => this.connectTimeOut;
            set => this.connectTimeOut = value;
        }

        /// <summary>获取或设置当前的客户端假死超时检查时间，单位为秒，默认60秒，60秒内没有接收到传感器的数据，则强制重连。</summary>
        public int CheckSeconds
        {
            get => this.checkSeconds;
            set => this.checkSeconds = value;
        }

        /// <summary>当前设备的地址信息</summary>
        public ushort Address
        {
            get => this.address;
            set => this.address = value;
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("VibrationSensorClient[{0}:{1}]", (object)this.ipAddress, (object)this.port);

        /// <summary>根据地址，命令，数据，创建向传感器发送的数据信息</summary>
        /// <param name="address">设备地址</param>
        /// <param name="cmd">命令</param>
        /// <param name="data">数据信息</param>
        /// <returns>原始的数据内容</returns>
        public static byte[] BulidLongMessage(ushort address, byte cmd, byte[] data)
        {
            if (data == null)
                data = new byte[0];
            byte[] numArray = new byte[16 + data.Length];
            numArray[0] = (byte)170;
            numArray[1] = (byte)85;
            numArray[2] = (byte)127;
            numArray[3] = BitConverter.GetBytes(address)[1];
            numArray[4] = BitConverter.GetBytes(address)[0];
            numArray[5] = cmd;
            numArray[6] = (byte)1;
            numArray[7] = (byte)0;
            numArray[8] = (byte)1;
            numArray[9] = (byte)1;
            numArray[10] = BitConverter.GetBytes(data.Length)[1];
            numArray[11] = BitConverter.GetBytes(data.Length)[0];
            data.CopyTo((Array)numArray, 12);
            int num = (int)numArray[3];
            for (int index = 4; index < numArray.Length - 4; ++index)
                num ^= (int)numArray[index];
            numArray[numArray.Length - 4] = (byte)num;
            numArray[numArray.Length - 3] = (byte)127;
            numArray[numArray.Length - 2] = (byte)170;
            numArray[numArray.Length - 1] = (byte)237;
            return numArray;
        }

        /// <summary>检查当前的数据是否XOR校验成功</summary>
        /// <param name="data">数据信息</param>
        /// <returns>校验结果</returns>
        public static bool CheckXor(byte[] data)
        {
            int num = (int)data[3];
            for (int index = 4; index < data.Length - 4; ++index)
                num ^= (int)data[index];
            return (int)BitConverter.GetBytes(num)[0] == (int)data[data.Length - 4];
        }

        /// <summary>
        /// 震动传感器峰值数据事件委托<br />
        /// Shock sensor peak data event delegation
        /// </summary>
        /// <param name="peekValue">峰值信息</param>
        public delegate void OnPeekValueReceiveDelegate(VibrationSensorPeekValue peekValue);

        /// <summary>
        /// 震动传感器实时数据事件委托<br />
        /// Vibration sensor real-time data event delegation
        /// </summary>
        /// <param name="actualValue">实际信息</param>
        public delegate void OnActualValueReceiveDelegate(VibrationSensorActualValue actualValue);

        /// <summary>
        /// 连接服务器成功的委托<br />
        /// Connection server successfully delegated
        /// </summary>
        public delegate void OnClientConnectedDelegate();
    }
}
