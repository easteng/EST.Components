// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.Toledo.ToledoSerial
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.LogNet;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

namespace EstCommunication.Profinet.Toledo
{
  /// <summary>托利多电子秤的串口服务器对象</summary>
  public class ToledoSerial
  {
    private SerialPort serialPort;
    private ILogNet logNet;

    /// <summary>实例化一个默认的对象</summary>
    public ToledoSerial()
    {
      this.serialPort = new SerialPort();
      this.serialPort.RtsEnable = true;
      this.serialPort.DataReceived += new SerialDataReceivedEventHandler(this.SerialPort_DataReceived);
    }

    private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
      List<byte> byteList = new List<byte>();
      byte[] buffer = new byte[1024];
      while (true)
      {
        Thread.Sleep(20);
        if (this.serialPort.BytesToRead >= 1)
        {
          try
          {
            int length = this.serialPort.Read(buffer, 0, Math.Min(this.serialPort.BytesToRead, buffer.Length));
            byte[] numArray = new byte[length];
            Array.Copy((Array) buffer, 0, (Array) numArray, 0, length);
            byteList.AddRange((IEnumerable<byte>) numArray);
          }
          catch (Exception ex)
          {
            this.logNet?.WriteException(this.ToString(), nameof (SerialPort_DataReceived), ex);
            return;
          }
        }
        else
          break;
      }
      if (byteList.Count == 0)
        return;
      ToledoSerial.ToledoStandardDataReceivedDelegate standardDataReceived = this.OnToledoStandardDataReceived;
      if (standardDataReceived == null)
        return;
      standardDataReceived((object) this, new ToledoStandardData(byteList.ToArray()));
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
      if (this.serialPort.IsOpen)
        return;
      this.serialPort.PortName = portName;
      this.serialPort.BaudRate = baudRate;
      this.serialPort.DataBits = dataBits;
      this.serialPort.StopBits = stopBits;
      this.serialPort.Parity = parity;
      this.PortName = this.serialPort.PortName;
      this.BaudRate = this.serialPort.BaudRate;
    }

    /// <summary>
    /// 根据自定义初始化方法进行初始化串口信息<br />
    /// Initialize the serial port information according to the custom initialization method
    /// </summary>
    /// <param name="initi">初始化的委托方法</param>
    public void SerialPortInni(Action<SerialPort> initi)
    {
      if (this.serialPort.IsOpen)
        return;
      this.serialPort.PortName = "COM5";
      this.serialPort.BaudRate = 9600;
      this.serialPort.DataBits = 8;
      this.serialPort.StopBits = StopBits.One;
      this.serialPort.Parity = Parity.None;
      initi(this.serialPort);
      this.PortName = this.serialPort.PortName;
      this.BaudRate = this.serialPort.BaudRate;
    }

    /// <summary>
    /// 打开一个新的串行端口连接<br />
    /// Open a new serial port connection
    /// </summary>
    public void Open()
    {
      if (this.serialPort.IsOpen)
        return;
      this.serialPort.Open();
    }

    /// <summary>
    /// 获取一个值，指示串口是否处于打开状态<br />
    /// Gets a value indicating whether the serial port is open
    /// </summary>
    /// <returns>是或否</returns>
    public bool IsOpen() => this.serialPort.IsOpen;

    /// <summary>
    /// 关闭当前的串口连接<br />
    /// Close the current serial connection
    /// </summary>
    public void Close()
    {
      if (!this.serialPort.IsOpen)
        return;
      this.serialPort.Close();
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
    public bool RtsEnable
    {
      get => this.serialPort.RtsEnable;
      set => this.serialPort.RtsEnable = value;
    }

    /// <summary>
    /// 当前连接串口信息的端口号名称<br />
    /// The port name of the current connection serial port information
    /// </summary>
    public string PortName { get; private set; }

    /// <summary>
    /// 当前连接串口信息的波特率<br />
    /// Baud rate of current connection serial port information
    /// </summary>
    public int BaudRate { get; private set; }

    /// <summary>当接收到一条新的托利多的数据的时候触发</summary>
    public event ToledoSerial.ToledoStandardDataReceivedDelegate OnToledoStandardDataReceived;

    /// <inheritdoc />
    public override string ToString() => base.ToString();

    /// <summary>托利多数据接收时的委托</summary>
    /// <param name="sender">数据发送对象</param>
    /// <param name="toledoStandardData">数据对象</param>
    public delegate void ToledoStandardDataReceivedDelegate(
      object sender,
      ToledoStandardData toledoStandardData);
  }
}
