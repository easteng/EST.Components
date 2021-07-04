// Decompiled with JetBrains decompiler
// Type: EstCommunication.Serial.SerialBase
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.Core;
using EstCommunication.LogNet;
using EstCommunication.Reflection;
using System;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace EstCommunication.Serial
{
  /// <summary>
  /// 所有串行通信类的基类，提供了一些基础的服务，核心的通信实现<br />
  /// The base class of all serial communication classes provides some basic services for the core communication implementation
  /// </summary>
  public class SerialBase : IDisposable
  {
    /// <inheritdoc cref="F:EstCommunication.Core.Net.NetworkDoubleBase.LogMsgFormatBinary" />
    protected bool LogMsgFormatBinary = true;
    private bool disposedValue = false;
    /// <summary>串口交互的核心</summary>
    protected SerialPort sP_ReadData = (SerialPort) null;
    private SimpleHybirdLock hybirdLock;
    private ILogNet logNet;
    private int receiveTimeout = 5000;
    private int sleepTime = 20;
    private bool isClearCacheBeforeRead = false;
    private int connectErrorCount = 0;

    /// <summary>
    /// 实例化一个无参的构造方法<br />
    /// Instantiate a parameterless constructor
    /// </summary>
    public SerialBase()
    {
      this.sP_ReadData = new SerialPort();
      this.hybirdLock = new SimpleHybirdLock();
    }

    /// <summary>
    /// 初始化串口信息，9600波特率，8位数据位，1位停止位，无奇偶校验<br />
    /// Initial serial port information, 9600 baud rate, 8 data bits, 1 stop bit, no parity
    /// </summary>
    /// <param name="portName">端口号信息，例如"COM3"</param>
    public void SerialPortInni(string portName) => this.SerialPortInni(portName, 9600);

    /// <summary>
    /// 初始化串口信息，波特率，8位数据位，1位停止位，无奇偶校验<br />
    /// Initializes serial port information, baud rate, 8-bit data bit, 1-bit stop bit, no parity
    /// </summary>
    /// <param name="portName">端口号信息，例如"COM3"</param>
    /// <param name="baudRate">波特率</param>
    public void SerialPortInni(string portName, int baudRate) => this.SerialPortInni(portName, baudRate, 8, StopBits.One, Parity.None);

    /// <summary>
    /// 初始化串口信息，波特率，数据位，停止位，奇偶校验需要全部自己来指定<br />
    /// Start serial port information, baud rate, data bit, stop bit, parity all need to be specified
    /// </summary>
    /// <param name="portName">端口号信息，例如"COM3"</param>
    /// <param name="baudRate">波特率</param>
    /// <param name="dataBits">数据位</param>
    /// <param name="stopBits">停止位</param>
    /// <param name="parity">奇偶校验</param>
    public void SerialPortInni(
      string portName,
      int baudRate,
      int dataBits,
      StopBits stopBits,
      Parity parity)
    {
      if (this.sP_ReadData.IsOpen)
        return;
      this.sP_ReadData.PortName = portName;
      this.sP_ReadData.BaudRate = baudRate;
      this.sP_ReadData.DataBits = dataBits;
      this.sP_ReadData.StopBits = stopBits;
      this.sP_ReadData.Parity = parity;
      this.PortName = this.sP_ReadData.PortName;
      this.BaudRate = this.sP_ReadData.BaudRate;
    }

    /// <summary>
    /// 根据自定义初始化方法进行初始化串口信息<br />
    /// Initialize the serial port information according to the custom initialization method
    /// </summary>
    /// <param name="initi">初始化的委托方法</param>
    public void SerialPortInni(Action<SerialPort> initi)
    {
      if (this.sP_ReadData.IsOpen)
        return;
      this.sP_ReadData.PortName = "COM5";
      this.sP_ReadData.BaudRate = 9600;
      this.sP_ReadData.DataBits = 8;
      this.sP_ReadData.StopBits = StopBits.One;
      this.sP_ReadData.Parity = Parity.None;
      initi(this.sP_ReadData);
      this.PortName = this.sP_ReadData.PortName;
      this.BaudRate = this.sP_ReadData.BaudRate;
    }

    /// <summary>
    /// 打开一个新的串行端口连接<br />
    /// Open a new serial port connection
    /// </summary>
    public OperateResult Open()
    {
      try
      {
        if (this.sP_ReadData.IsOpen)
          return OperateResult.CreateSuccessResult();
        this.sP_ReadData.Open();
        return this.InitializationOnOpen();
      }
      catch (Exception ex)
      {
        if (this.connectErrorCount < 100000000)
          ++this.connectErrorCount;
        return new OperateResult(-this.connectErrorCount, ex.Message);
      }
    }

    /// <summary>
    /// 获取一个值，指示串口是否处于打开状态<br />
    /// Gets a value indicating whether the serial port is open
    /// </summary>
    /// <returns>是或否</returns>
    public bool IsOpen() => this.sP_ReadData.IsOpen;

    /// <summary>
    /// 关闭当前的串口连接<br />
    /// Close the current serial connection
    /// </summary>
    public void Close()
    {
      if (!this.sP_ReadData.IsOpen)
        return;
      this.ExtraOnClose();
      this.sP_ReadData.Close();
    }

    /// <summary>
    /// 将原始的字节数据发送到串口，然后从串口接收一条数据。<br />
    /// The raw byte data is sent to the serial port, and then a piece of data is received from the serial port.
    /// </summary>
    /// <param name="send">发送的原始字节数据</param>
    /// <returns>带接收字节的结果对象</returns>
    [EstMqttApi(Description = "The raw byte data is sent to the serial port, and then a piece of data is received from the serial port.")]
    public OperateResult<byte[]> ReadBase(byte[] send) => this.ReadBase(send, false);

    /// <summary>
    /// 将原始的字节数据发送到串口，然后从串口接收一条数据。<br />
    /// The raw byte data is sent to the serial port, and then a piece of data is received from the serial port.
    /// </summary>
    /// <param name="send">发送的原始字节数据</param>
    /// <param name="sendOnly">是否只是发送，如果为true, 不需要等待数据返回，如果为false, 需要等待数据返回</param>
    /// <returns>带接收字节的结果对象</returns>
    public OperateResult<byte[]> ReadBase(byte[] send, bool sendOnly)
    {
      this.LogNet?.WriteDebug(this.ToString(), StringResources.Language.Send + " : " + (this.LogMsgFormatBinary ? send.ToHexString(' ') : Encoding.ASCII.GetString(send)));
      this.hybirdLock.Enter();
      OperateResult result1 = this.Open();
      if (!result1.IsSuccess)
      {
        this.hybirdLock.Leave();
        return OperateResult.CreateFailedResult<byte[]>(result1);
      }
      if (this.IsClearCacheBeforeRead)
        this.ClearSerialCache();
      OperateResult result2 = this.SPSend(this.sP_ReadData, send);
      if (!result2.IsSuccess)
      {
        this.hybirdLock.Leave();
        return OperateResult.CreateFailedResult<byte[]>(result2);
      }
      if (sendOnly)
      {
        this.hybirdLock.Leave();
        return OperateResult.CreateSuccessResult<byte[]>(new byte[0]);
      }
      OperateResult<byte[]> operateResult = this.SPReceived(this.sP_ReadData, true);
      this.hybirdLock.Leave();
      this.LogNet?.WriteDebug(this.ToString(), StringResources.Language.Receive + " : " + (this.LogMsgFormatBinary ? operateResult.Content.ToHexString(' ') : Encoding.ASCII.GetString(operateResult.Content)));
      return operateResult;
    }

    /// <summary>
    /// 清除串口缓冲区的数据，并返回该数据，如果缓冲区没有数据，返回的字节数组长度为0<br />
    /// The number sent clears the data in the serial port buffer and returns that data, or if there is no data in the buffer, the length of the byte array returned is 0
    /// </summary>
    /// <returns>是否操作成功的方法</returns>
    public OperateResult<byte[]> ClearSerialCache() => this.SPReceived(this.sP_ReadData, false);

    /// <inheritdoc cref="M:EstCommunication.Core.Net.NetworkDoubleBase.InitializationOnConnect(System.Net.Sockets.Socket)" />
    protected virtual OperateResult InitializationOnOpen() => OperateResult.CreateSuccessResult();

    /// <inheritdoc cref="M:EstCommunication.Core.Net.NetworkDoubleBase.ExtraOnDisconnect(System.Net.Sockets.Socket)" />
    protected virtual OperateResult ExtraOnClose() => OperateResult.CreateSuccessResult();

    /// <summary>
    /// 发送数据到串口去。<br />
    /// Send data to serial port.
    /// </summary>
    /// <param name="serialPort">串口对象</param>
    /// <param name="data">字节数据</param>
    /// <returns>是否发送成功</returns>
    protected virtual OperateResult SPSend(SerialPort serialPort, byte[] data)
    {
      if (data == null || (uint) data.Length <= 0U)
        return OperateResult.CreateSuccessResult();
      if (!Authorization.nzugaydgwadawdibbas())
        return (OperateResult) new OperateResult<byte[]>(StringResources.Language.AuthorizationFailed);
      try
      {
        serialPort.Write(data, 0, data.Length);
        return OperateResult.CreateSuccessResult();
      }
      catch (Exception ex)
      {
        if (this.connectErrorCount < 100000000)
          ++this.connectErrorCount;
        return new OperateResult(-this.connectErrorCount, ex.Message);
      }
    }

    /// <summary>
    /// 从串口接收一串字节数据信息，直到没有数据为止，如果参数awaitData为false, 第一轮接收没有数据则返回<br />
    /// Receives a string of bytes of data information from the serial port until there is no data, and returns if the parameter awaitData is false
    /// </summary>
    /// <param name="serialPort">串口对象</param>
    /// <param name="awaitData">是否必须要等待数据返回</param>
    /// <returns>结果数据对象</returns>
    protected virtual OperateResult<byte[]> SPReceived(
      SerialPort serialPort,
      bool awaitData)
    {
      if (!Authorization.nzugaydgwadawdibbas())
        return new OperateResult<byte[]>(StringResources.Language.AuthorizationFailed);
      byte[] buffer = new byte[1024];
      MemoryStream memoryStream = new MemoryStream();
      DateTime now = DateTime.Now;
      while (true)
      {
        Thread.Sleep(this.sleepTime);
        try
        {
          if (serialPort.BytesToRead < 1)
          {
            if ((DateTime.Now - now).TotalMilliseconds > (double) this.ReceiveTimeout)
            {
              memoryStream.Dispose();
              if (this.connectErrorCount < 100000000)
                ++this.connectErrorCount;
              return new OperateResult<byte[]>(-this.connectErrorCount, string.Format("Time out: {0}", (object) this.ReceiveTimeout));
            }
            if (memoryStream.Length <= 0L)
            {
              if (!awaitData)
                break;
            }
            else
              break;
          }
          else
          {
            int count = serialPort.Read(buffer, 0, buffer.Length);
            memoryStream.Write(buffer, 0, count);
          }
        }
        catch (Exception ex)
        {
          memoryStream.Dispose();
          if (this.connectErrorCount < 100000000)
            ++this.connectErrorCount;
          return new OperateResult<byte[]>(-this.connectErrorCount, ex.Message);
        }
      }
      byte[] array = memoryStream.ToArray();
      memoryStream.Dispose();
      this.connectErrorCount = 0;
      return OperateResult.CreateSuccessResult<byte[]>(array);
    }

    /// <inheritdoc cref="P:EstCommunication.Core.Net.NetworkBase.LogNet" />
    public ILogNet LogNet
    {
      get => this.logNet;
      set => this.logNet = value;
    }

    /// <summary>
    /// 获取或设置一个值，该值指示在串行通信中是否启用请求发送 (RTS) 信号。<br />
    /// Gets or sets a value indicating whether the request sending (RTS) signal is enabled in serial communication.
    /// </summary>
    [EstMqttApi(Description = "Gets or sets a value indicating whether the request sending (RTS) signal is enabled in serial communication.")]
    public bool RtsEnable
    {
      get => this.sP_ReadData.RtsEnable;
      set => this.sP_ReadData.RtsEnable = value;
    }

    /// <summary>
    /// 接收数据的超时时间，默认5000ms<br />
    /// Timeout for receiving data, default is 5000ms
    /// </summary>
    [EstMqttApi(Description = "Timeout for receiving data, default is 5000ms")]
    public int ReceiveTimeout
    {
      get => this.receiveTimeout;
      set => this.receiveTimeout = value;
    }

    /// <summary>
    /// 连续串口缓冲数据检测的间隔时间，默认20ms，该值越小，通信速度越快，但是越不稳定。<br />
    /// Continuous serial port buffer data detection interval, the default 20ms, the smaller the value, the faster the communication, but the more unstable.
    /// </summary>
    [EstMqttApi(Description = "Continuous serial port buffer data detection interval, the default 20ms, the smaller the value, the faster the communication, but the more unstable.")]
    public int SleepTime
    {
      get => this.sleepTime;
      set
      {
        if (value <= 0)
          return;
        this.sleepTime = value;
      }
    }

    /// <summary>
    /// 是否在发送数据前清空缓冲数据，默认是false<br />
    /// Whether to empty the buffer before sending data, the default is false
    /// </summary>
    [EstMqttApi(Description = "Whether to empty the buffer before sending data, the default is false")]
    public bool IsClearCacheBeforeRead
    {
      get => this.isClearCacheBeforeRead;
      set => this.isClearCacheBeforeRead = value;
    }

    /// <summary>
    /// 当前连接串口信息的端口号名称<br />
    /// The port name of the current connection serial port information
    /// </summary>
    [EstMqttApi(Description = "The port name of the current connection serial port information")]
    public string PortName { get; private set; }

    /// <summary>
    /// 当前连接串口信息的波特率<br />
    /// Baud rate of current connection serial port information
    /// </summary>
    [EstMqttApi(Description = "Baud rate of current connection serial port information")]
    public int BaudRate { get; private set; }

    /// <summary>释放当前的对象</summary>
    /// <param name="disposing">是否在</param>
    protected virtual void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      if (disposing)
      {
        this.hybirdLock?.Dispose();
        this.sP_ReadData?.Dispose();
      }
      this.disposedValue = true;
    }

    /// <summary>释放当前的对象</summary>
    public void Dispose() => this.Dispose(true);

    /// <inheritdoc />
    public override string ToString() => string.Format("SerialBase[{0},{1},{2},{3},{4}]", (object) this.sP_ReadData.PortName, (object) this.sP_ReadData.BaudRate, (object) this.sP_ReadData.DataBits, (object) this.sP_ReadData.StopBits, (object) this.sP_ReadData.Parity);
  }
}
