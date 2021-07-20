//// Decompiled with JetBrains decompiler
//// Type: ESTCore.Common.Profinet.LSIS.LSisServer
//// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
//// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
//// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

//using ESTCore.Common.BasicFramework;
//using ESTCore.Common.Core;
//using ESTCore.Common.Core.IMessage;
//using ESTCore.Common.Core.Net;
//using ESTCore.Common.Profinet.Panasonic;
//using ESTCore.Common.Reflection;
//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.IO.Ports;
//using System.Net;
//using System.Net.Sockets;
//using System.Text;
//using System.Threading;

//namespace ESTCore.Common.Profinet.LSIS
//{
//  /// <summary>
//  /// <b>[Authorization]</b> LSisServer
//  /// </summary>
//  public class LSisServer : NetworkDataServerBase
//  {
//    private SoftBuffer pBuffer;
//    private SoftBuffer qBuffer;
//    private SoftBuffer mBuffer;
//    private SoftBuffer iBuffer;
//    private SoftBuffer uBuffer;
//    private SoftBuffer dBuffer;
//    private SoftBuffer tBuffer;
//    private const int DataPoolLength = 65536;
//    private int station = 1;
//    private SerialPort serialPort;

//    /// <summary>LSisServer</summary>
//    public LSisServer(string CpuType)
//    {
//      this.pBuffer = new SoftBuffer(65536);
//      this.qBuffer = new SoftBuffer(65536);
//      this.iBuffer = new SoftBuffer(65536);
//      this.uBuffer = new SoftBuffer(65536);
//      this.mBuffer = new SoftBuffer(65536);
//      this.dBuffer = new SoftBuffer(131072);
//      this.tBuffer = new SoftBuffer(131072);
//      this.SetCpuType = CpuType;
//      this.WordLength = (ushort) 2;
//      this.ByteTransform = (IByteTransform) new RegularByteTransform();
//      this.serialPort = new SerialPort();
//    }

//    /// <summary>set plc</summary>
//    public string SetCpuType { get; set; }

//    /// <inheritdoc cref="M:ESTCore.Common.Profinet.LSIS.XGBFastEnet.Read(System.String,System.UInt16)" />
//    [EstMqttApi("ReadByteArray", "")]
//    public override OperateResult<byte[]> Read(string address, ushort length)
//    {
//      OperateResult<string> byteUnit = this.AnalysisAddressToByteUnit(address, false);
//      if (!byteUnit.IsSuccess)
//        return OperateResult.CreateFailedResult<byte[]>((OperateResult) byteUnit);
//      int index = int.Parse(byteUnit.Content.Substring(1));
//      switch (byteUnit.Content[0])
//      {
//        case 'D':
//          return OperateResult.CreateSuccessResult<byte[]>(this.dBuffer.GetBytes(index, (int) length));
//        case 'I':
//          return OperateResult.CreateSuccessResult<byte[]>(this.iBuffer.GetBytes(index, (int) length));
//        case 'M':
//          return OperateResult.CreateSuccessResult<byte[]>(this.mBuffer.GetBytes(index, (int) length));
//        case 'P':
//          return OperateResult.CreateSuccessResult<byte[]>(this.pBuffer.GetBytes(index, (int) length));
//        case 'Q':
//          return OperateResult.CreateSuccessResult<byte[]>(this.qBuffer.GetBytes(index, (int) length));
//        case 'T':
//          return OperateResult.CreateSuccessResult<byte[]>(this.tBuffer.GetBytes(index, (int) length));
//        case 'U':
//          return OperateResult.CreateSuccessResult<byte[]>(this.uBuffer.GetBytes(index, (int) length));
//        default:
//          return new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType);
//      }
//    }

//    /// <inheritdoc cref="M:ESTCore.Common.Profinet.LSIS.XGBFastEnet.Write(System.String,System.Byte[])" />
//    [EstMqttApi("WriteByteArray", "")]
//    public override OperateResult Write(string address, byte[] value)
//    {
//      OperateResult<string> byteUnit = this.AnalysisAddressToByteUnit(address, false);
//      if (!byteUnit.IsSuccess)
//        return (OperateResult) OperateResult.CreateFailedResult<byte[]>((OperateResult) byteUnit);
//      int destIndex = int.Parse(byteUnit.Content.Substring(1));
//      switch (byteUnit.Content[0])
//      {
//        case 'D':
//          this.dBuffer.SetBytes(value, destIndex);
//          break;
//        case 'I':
//          this.iBuffer.SetBytes(value, destIndex);
//          break;
//        case 'M':
//          this.mBuffer.SetBytes(value, destIndex);
//          break;
//        case 'P':
//          this.pBuffer.SetBytes(value, destIndex);
//          break;
//        case 'Q':
//          this.qBuffer.SetBytes(value, destIndex);
//          break;
//        case 'T':
//          this.tBuffer.SetBytes(value, destIndex);
//          break;
//        case 'U':
//          this.uBuffer.SetBytes(value, destIndex);
//          break;
//        default:
//          return (OperateResult) new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType);
//      }
//      return OperateResult.CreateSuccessResult();
//    }

//    /// <inheritdoc cref="M:ESTCore.Common.Profinet.LSIS.XGBFastEnet.ReadByte(System.String)" />
//    [EstMqttApi("ReadByte", "")]
//    public OperateResult<byte> ReadByte(string address) => ByteTransformHelper.GetResultFromArray<byte>(this.Read(address, (ushort) 1));

//    /// <inheritdoc cref="M:ESTCore.Common.Profinet.LSIS.XGBFastEnet.Write(System.String,System.Byte)" />
//    [EstMqttApi("WriteByte", "")]
//    public OperateResult Write(string address, byte value) => this.Write(address, new byte[1]
//    {
//      value
//    });

//    /// <inheritdoc />
//    [EstMqttApi("ReadBoolArray", "")]
//    public override OperateResult<bool[]> ReadBool(string address, ushort length)
//    {
//      OperateResult<string> byteUnit = this.AnalysisAddressToByteUnit(address, true);
//      if (!byteUnit.IsSuccess)
//        return OperateResult.CreateFailedResult<bool[]>((OperateResult) byteUnit);
//      int destIndex = int.Parse(byteUnit.Content.Substring(1));
//      switch (byteUnit.Content[0])
//      {
//        case 'I':
//          return OperateResult.CreateSuccessResult<bool[]>(this.iBuffer.GetBool(destIndex, (int) length));
//        case 'M':
//          return OperateResult.CreateSuccessResult<bool[]>(this.mBuffer.GetBool(destIndex, (int) length));
//        case 'P':
//          return OperateResult.CreateSuccessResult<bool[]>(this.pBuffer.GetBool(destIndex, (int) length));
//        case 'Q':
//          return OperateResult.CreateSuccessResult<bool[]>(this.qBuffer.GetBool(destIndex, (int) length));
//        case 'U':
//          return OperateResult.CreateSuccessResult<bool[]>(this.uBuffer.GetBool(destIndex, (int) length));
//        default:
//          return new OperateResult<bool[]>(StringResources.Language.NotSupportedDataType);
//      }
//    }

//    /// <inheritdoc />
//    [EstMqttApi("WriteBoolArray", "")]
//    public override OperateResult Write(string address, bool[] value)
//    {
//      OperateResult<string> byteUnit = this.AnalysisAddressToByteUnit(address, true);
//      if (!byteUnit.IsSuccess)
//        return (OperateResult) OperateResult.CreateFailedResult<bool[]>((OperateResult) byteUnit);
//      int destIndex = int.Parse(byteUnit.Content.Substring(1));
//      switch (byteUnit.Content[0])
//      {
//        case 'I':
//          this.iBuffer.SetBool(value, destIndex);
//          return OperateResult.CreateSuccessResult();
//        case 'M':
//          this.mBuffer.SetBool(value, destIndex);
//          return OperateResult.CreateSuccessResult();
//        case 'P':
//          this.pBuffer.SetBool(value, destIndex);
//          return OperateResult.CreateSuccessResult();
//        case 'Q':
//          this.qBuffer.SetBool(value, destIndex);
//          return OperateResult.CreateSuccessResult();
//        case 'U':
//          this.uBuffer.SetBool(value, destIndex);
//          return OperateResult.CreateSuccessResult();
//        default:
//          return new OperateResult(StringResources.Language.NotSupportedDataType);
//      }
//    }

//    /// <inheritdoc />
//    protected override void ThreadPoolLoginAfterClientCheck(Socket socket, IPEndPoint endPoint)
//    {
//      AppSession session = new AppSession();
//      session.IpEndPoint = endPoint;
//      session.WorkSocket = socket;
//      try
//      {
//        socket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, new AsyncCallback(this.SocketAsyncCallBack), (object) session);
//        this.AddClient(session);
//      }
//      catch
//      {
//        socket.Close();
//        this.LogNet?.WriteDebug(this.ToString(), string.Format(StringResources.Language.ClientOfflineInfo, (object) endPoint));
//      }
//    }

//    private async void SocketAsyncCallBack(IAsyncResult ar)
//    {
//      if (!(ar.AsyncState is AppSession session))
//      {
//        session = (AppSession) null;
//      }
//      else
//      {
//        try
//        {
//          int receiveCount = session.WorkSocket.EndReceive(ar);
//          OperateResult<byte[]> read1 = await this.ReceiveByMessageAsync(session.WorkSocket, 5000, (INetMessage) new LsisFastEnetMessage());
//          if (!read1.IsSuccess)
//          {
//            this.RemoveClient(session);
//            session = (AppSession) null;
//            return;
//          }
//          if (!ESTCore.Common.Authorization.asdniasnfaksndiqwhawfskhfaiw())
//          {
//            this.RemoveClient(session);
//            session = (AppSession) null;
//            return;
//          }
//          this.LogNet?.WriteDebug(this.ToString(), string.Format("[{0}] Tcp {1}：{2}", (object) session.IpEndPoint, (object) StringResources.Language.Receive, (object) read1.Content.ToHexString(' ')));
//          byte[] receive = read1.Content;
//          byte[] SendData = (byte[]) null;
//          if (receive[20] == (byte) 84)
//            SendData = this.ReadByMessage(receive);
//          else if (receive[20] == (byte) 88)
//          {
//            SendData = this.WriteByMessage(receive);
//          }
//          else
//          {
//            this.RaiseDataReceived((object) session, SendData);
//            this.RemoveClient(session);
//            session = (AppSession) null;
//            return;
//          }
//          if (SendData == null)
//          {
//            this.RemoveClient(session);
//            session = (AppSession) null;
//            return;
//          }
//          this.RaiseDataReceived((object) session, SendData);
//          session.WorkSocket.Send(SendData);
//          this.LogNet?.WriteDebug(this.ToString(), string.Format("[{0}] Tcp {1}：{2}", (object) session.IpEndPoint, (object) StringResources.Language.Send, (object) SendData.ToHexString(' ')));
//          session.HeartTime = DateTime.Now;
//          this.RaiseDataSend(receive);
//          session.WorkSocket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, new AsyncCallback(this.SocketAsyncCallBack), (object) session);
//          read1 = (OperateResult<byte[]>) null;
//          receive = (byte[]) null;
//          SendData = (byte[]) null;
//        }
//        catch (Exception ex)
//        {
//          this.RemoveClient(session, "SocketAsyncCallBack -> " + ex.Message);
//        }
//        session = (AppSession) null;
//      }
//    }

//    private byte[] ReadByMessage(byte[] packCommand)
//    {
//      List<byte> byteList = new List<byte>();
//      byteList.AddRange((IEnumerable<byte>) this.ReadByCommand(packCommand));
//      return byteList.ToArray();
//    }

//    private byte[] ReadByCommand(byte[] command)
//    {
//      List<byte> byteList = new List<byte>();
//      byteList.AddRange((IEnumerable<byte>) command.SelectBegin<byte>(20));
//      byteList[9] = (byte) 17;
//      byteList[10] = (byte) 1;
//      byteList[12] = (byte) 160;
//      byteList[13] = (byte) 17;
//      byteList[18] = (byte) 3;
//      byteList.AddRange((IEnumerable<byte>) new byte[10]
//      {
//        (byte) 85,
//        (byte) 0,
//        command[22],
//        command[23],
//        (byte) 8,
//        (byte) 1,
//        (byte) 0,
//        (byte) 0,
//        (byte) 1,
//        (byte) 0
//      });
//      int num = (int) command[28];
//      string address = Encoding.ASCII.GetString(command, 31, num - 1);
//      byte[] numArray1;
//      if (command[22] == (byte) 0)
//      {
//        int int32 = Convert.ToInt32(address.Substring(2));
//        byte[] numArray2;
//        if (!this.ReadBool(address.Substring(0, 2) + (int32 / 16).ToString() + (int32 % 16).ToString("X1")).Content)
//          numArray2 = new byte[1];
//        else
//          numArray2 = new byte[1]{ (byte) 1 };
//        numArray1 = numArray2;
//      }
//      else if (command[22] == (byte) 1)
//        numArray1 = this.Read(address, (ushort) 1).Content;
//      else if (command[22] == (byte) 2)
//        numArray1 = this.Read(address, (ushort) 2).Content;
//      else if (command[22] == (byte) 3)
//        numArray1 = this.Read(address, (ushort) 4).Content;
//      else if (command[22] == (byte) 4)
//        numArray1 = this.Read(address, (ushort) 8).Content;
//      else if (command[22] == (byte) 20)
//      {
//        ushort uint16 = BitConverter.ToUInt16(command, 30 + num);
//        numArray1 = this.Read(address, uint16).Content;
//      }
//      else
//        numArray1 = this.Read(address, (ushort) 1).Content;
//      byteList.AddRange((IEnumerable<byte>) BitConverter.GetBytes((ushort) numArray1.Length));
//      byteList.AddRange((IEnumerable<byte>) numArray1);
//      byteList[16] = (byte) (byteList.Count - 20);
//      return byteList.ToArray();
//    }

//    private byte[] WriteByMessage(byte[] packCommand)
//    {
//      if (!this.EnableWrite)
//        return (byte[]) null;
//      List<byte> byteList = new List<byte>();
//      byteList.AddRange((IEnumerable<byte>) packCommand.SelectBegin<byte>(20));
//      byteList[9] = (byte) 17;
//      byteList[10] = (byte) 1;
//      byteList[12] = (byte) 160;
//      byteList[13] = (byte) 17;
//      byteList[18] = (byte) 3;
//      byteList.AddRange((IEnumerable<byte>) new byte[10]
//      {
//        (byte) 89,
//        (byte) 0,
//        (byte) 20,
//        (byte) 0,
//        (byte) 8,
//        (byte) 1,
//        (byte) 0,
//        (byte) 0,
//        (byte) 1,
//        (byte) 0
//      });
//      int num = (int) packCommand[28];
//      string address = Encoding.ASCII.GetString(packCommand, 31, num - 1);
//      int uint16 = (int) BitConverter.ToUInt16(packCommand, 30 + num);
//      byte[] numArray = this.ByteTransform.TransByte(packCommand, 32 + num, uint16);
//      if (packCommand[22] == (byte) 0)
//      {
//        int int32 = Convert.ToInt32(address.Substring(2));
//        this.Write(address.Substring(0, 2) + (int32 / 16).ToString() + (int32 % 16).ToString("X1"), packCommand[38] > (byte) 0);
//      }
//      else
//        this.Write(address, numArray);
//      byteList[16] = (byte) (byteList.Count - 20);
//      return byteList.ToArray();
//    }

//    /// <inheritdoc />
//    protected override void LoadFromBytes(byte[] content)
//    {
//      if (content.Length < 262144)
//        throw new Exception("File is not correct");
//      this.pBuffer.SetBytes(content, 0, 0, 65536);
//      this.qBuffer.SetBytes(content, 65536, 0, 65536);
//      this.mBuffer.SetBytes(content, 131072, 0, 65536);
//      this.dBuffer.SetBytes(content, 196608, 0, 65536);
//    }

//    /// <inheritdoc />
//    protected override byte[] SaveToBytes()
//    {
//      byte[] numArray = new byte[262144];
//      Array.Copy((Array) this.pBuffer.GetBytes(), 0, (Array) numArray, 0, 65536);
//      Array.Copy((Array) this.qBuffer.GetBytes(), 0, (Array) numArray, 65536, 65536);
//      Array.Copy((Array) this.mBuffer.GetBytes(), 0, (Array) numArray, 131072, 65536);
//      Array.Copy((Array) this.dBuffer.GetBytes(), 0, (Array) numArray, 196608, 65536);
//      return numArray;
//    }

//    /// <summary>NumberStyles HexNumber</summary>
//    /// <param name="value"></param>
//    /// <returns></returns>
//    private static bool IsHex(string value)
//    {
//      if (string.IsNullOrEmpty(value))
//        return false;
//      bool flag = false;
//      for (int index = 0; index < value.Length; ++index)
//      {
//        switch (value[index])
//        {
//          case 'A':
//          case 'B':
//          case 'C':
//          case 'D':
//          case 'E':
//          case 'F':
//          case 'a':
//          case 'b':
//          case 'c':
//          case 'd':
//          case 'e':
//          case 'f':
//            flag = true;
//            break;
//        }
//      }
//      return flag;
//    }

//    /// <summary>Check the intput string address</summary>
//    /// <param name="address"></param>
//    /// <returns></returns>
//    public static int CheckAddress(string address)
//    {
//      int num = 0;
//      if (LSisServer.IsHex(address))
//      {
//        int result;
//        if (int.TryParse(address, NumberStyles.HexNumber, (IFormatProvider) CultureInfo.CurrentCulture, out result))
//          num = result;
//      }
//      else
//        num = int.Parse(address);
//      return num;
//    }

//    /// <summary>使用默认的参数进行初始化串口，9600波特率，8位数据位，无奇偶校验，1位停止位</summary>
//    /// <param name="com">串口信息</param>
//    public void StartSerialPort(string com) => this.StartSerialPort(com, 9600);

//    /// <summary>使用默认的参数进行初始化串口，8位数据位，无奇偶校验，1位停止位</summary>
//    /// <param name="com">串口信息</param>
//    /// <param name="baudRate">波特率</param>
//    public void StartSerialPort(string com, int baudRate) => this.StartSerialPort((Action<SerialPort>) (sp =>
//    {
//      sp.PortName = com;
//      sp.BaudRate = baudRate;
//      sp.DataBits = 8;
//      sp.Parity = Parity.None;
//      sp.StopBits = StopBits.One;
//    }));

//    /// <summary>使用自定义的初始化方法初始化串口的参数</summary>
//    /// <param name="inni">初始化信息的委托</param>
//    public void StartSerialPort(Action<SerialPort> inni)
//    {
//      if (this.serialPort.IsOpen)
//        return;
//      if (inni != null)
//        inni(this.serialPort);
//      this.serialPort.ReadBufferSize = 1024;
//      this.serialPort.ReceivedBytesThreshold = 1;
//      this.serialPort.Open();
//      this.serialPort.DataReceived += new SerialDataReceivedEventHandler(this.SerialPort_DataReceived);
//    }

//    /// <summary>关闭串口</summary>
//    public void CloseSerialPort()
//    {
//      if (!this.serialPort.IsOpen)
//        return;
//      this.serialPort.Close();
//    }

//    /// <summary>接收到串口数据的时候触发</summary>
//    /// <param name="sender">串口对象</param>
//    /// <param name="e">消息</param>
//    private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
//    {
//      SerialPort serialPort = (SerialPort) sender;
//      int offset = 0;
//      byte[] buffer = new byte[1024];
//      byte[] send = (byte[]) null;
//      while (true)
//      {
//        Thread.Sleep(20);
//        int length = serialPort.Read(buffer, offset, serialPort.BytesToRead);
//        offset += length;
//        if (length != 0)
//        {
//          send = new byte[offset];
//          Array.Copy((Array) buffer, 0, (Array) send, 0, length);
//        }
//        else
//          break;
//      }
//      if (send == null)
//        return;
//      byte[] numArray1 = SoftBasic.ArrayRemoveLast<byte>(send, 2);
//      if (numArray1[3] == (byte) 114)
//      {
//        byte[] numArray2 = this.ReadSerialByCommand(numArray1);
//        this.RaiseDataReceived(sender, numArray2);
//        this.serialPort.Write(numArray2, 0, numArray2.Length);
//      }
//      else if (numArray1[3] == (byte) 119)
//      {
//        byte[] numArray2 = this.WriteSerialByMessage(numArray1);
//        if (numArray2 != null)
//        {
//          this.RaiseDataReceived(sender, numArray2);
//          this.serialPort.Write(numArray2, 0, numArray2.Length);
//        }
//      }
//      else
//        this.serialPort.Close();
//      if (!this.IsStarted)
//        return;
//      this.RaiseDataSend(send);
//    }

//    private byte[] ReadSerialByCommand(byte[] command)
//    {
//      List<byte> byteList = new List<byte>();
//      byteList.Add((byte) 6);
//      byteList.AddRange((IEnumerable<byte>) SoftBasic.BuildAsciiBytesFrom((byte) this.station));
//      byteList.Add((byte) 114);
//      byteList.Add((byte) 83);
//      byteList.Add((byte) 66);
//      byteList.AddRange((IEnumerable<byte>) Encoding.ASCII.GetBytes("01"));
//      int num1 = int.Parse(Encoding.ASCII.GetString(command, 6, 2));
//      int int32 = Convert.ToInt32(Encoding.ASCII.GetString(command, 8 + num1, 2), 16);
//      byte[] content = this.Read(Encoding.ASCII.GetString(command, 9, num1 - 1), (ushort) int32).Content;
//      byteList.AddRange((IEnumerable<byte>) SoftBasic.BuildAsciiBytesFrom((byte) content.Length));
//      byteList.AddRange((IEnumerable<byte>) SoftBasic.BytesToAsciiBytes(content));
//      byteList.Add((byte) 3);
//      int num2 = 0;
//      for (int index = 0; index < byteList.Count; ++index)
//        num2 += (int) byteList[index];
//      byteList.AddRange((IEnumerable<byte>) SoftBasic.BuildAsciiBytesFrom((byte) num2));
//      return byteList.ToArray();
//    }

//    private byte[] WriteSerialByMessage(byte[] packCommand)
//    {
//      if (!this.EnableWrite)
//        return (byte[]) null;
//      List<byte> byteList = new List<byte>();
//      string str = Encoding.ASCII.GetString(packCommand, 3, 3);
//      byteList.Add((byte) 6);
//      byteList.AddRange((IEnumerable<byte>) SoftBasic.BuildAsciiBytesFrom((byte) this.station));
//      byteList.Add((byte) 119);
//      byteList.Add((byte) 83);
//      byteList.Add((byte) 66);
//      byteList.Add((byte) 3);
//      if (str == "wSS")
//      {
//        string s = Encoding.ASCII.GetString(packCommand, 8, 2);
//        this.Write(Encoding.ASCII.GetString(packCommand, 11, int.Parse(s) - 1), new byte[1]
//        {
//          Encoding.ASCII.GetString(packCommand, 10 + int.Parse(s), 2) == "01" ? (byte) 1 : (byte) 0
//        });
//      }
//      else
//      {
//        string s = Encoding.ASCII.GetString(packCommand, 6, 2);
//        string address = Encoding.ASCII.GetString(packCommand, 9, int.Parse(s) - 1);
//        int num = int.Parse(Encoding.ASCII.GetString(packCommand, 8 + int.Parse(s), 2));
//        byte[] bytes = SoftBasic.HexStringToBytes(Encoding.ASCII.GetString(packCommand, 8 + int.Parse(s) + num, num * 2));
//        this.Write(address, bytes);
//      }
//      return byteList.ToArray();
//    }

//    /// <inheritdoc />
//    public override string ToString() => string.Format("LSisServer[{0}]", (object) this.Port);

//    /// <summary>将带有数据类型的地址，转换成实际的byte数组的地址信息，例如 MW100 转成 M200</summary>
//    /// <param name="address">带有类型的地址</param>
//    /// <param name="isBit">是否是位操作</param>
//    /// <returns>最终的按照字节为单位的地址信息</returns>
//    public OperateResult<string> AnalysisAddressToByteUnit(string address, bool isBit)
//    {
//      if (!XGBFastEnet.AddressTypes.Contains(address.Substring(0, 1)))
//        return new OperateResult<string>(StringResources.Language.NotSupportedDataType);
//      try
//      {
//        int num;
//        if (address[0] == 'D' || address[0] == 'T')
//        {
//          switch (address[1])
//          {
//            case 'B':
//              num = Convert.ToInt32(address.Substring(2));
//              break;
//            case 'D':
//              num = Convert.ToInt32(address.Substring(2)) * 4;
//              break;
//            case 'L':
//              num = Convert.ToInt32(address.Substring(2)) * 8;
//              break;
//            case 'W':
//              num = Convert.ToInt32(address.Substring(2)) * 2;
//              break;
//            default:
//              num = Convert.ToInt32(address.Substring(1)) * 2;
//              break;
//          }
//        }
//        else if (isBit)
//        {
//          num = address[1] == 'X' ? PanasonicHelper.CalculateComplexAddress(address.Substring(2)) : PanasonicHelper.CalculateComplexAddress(address.Substring(1));
//        }
//        else
//        {
//          switch (address[1])
//          {
//            case 'B':
//              num = Convert.ToInt32(address.Substring(2));
//              break;
//            case 'D':
//              num = Convert.ToInt32(address.Substring(2)) * 4;
//              break;
//            case 'L':
//              num = Convert.ToInt32(address.Substring(2)) * 8;
//              break;
//            case 'W':
//              num = Convert.ToInt32(address.Substring(2)) * 2;
//              break;
//            case 'X':
//              num = Convert.ToInt32(address.Substring(2));
//              break;
//            default:
//              num = Convert.ToInt32(address.Substring(1)) * (isBit ? 1 : 2);
//              break;
//          }
//        }
//        return OperateResult.CreateSuccessResult<string>(address.Substring(0, 1) + num.ToString());
//      }
//      catch (Exception ex)
//      {
//        return new OperateResult<string>("AnalysisAddress Failed: " + ex.Message + " Source: " + address);
//      }
//    }
//  }
//}
