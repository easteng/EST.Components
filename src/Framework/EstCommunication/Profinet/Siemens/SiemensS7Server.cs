// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.Siemens.SiemensS7Server
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using EstCommunication.Core;
using EstCommunication.Core.Address;
using EstCommunication.Core.IMessage;
using EstCommunication.Core.Net;
using EstCommunication.Reflection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace EstCommunication.Profinet.Siemens
{
  /// <summary>
  /// <b>[商业授权]</b> 西门子S7协议的虚拟服务器，支持TCP协议，模拟的是1200的PLC进行通信，在客户端进行操作操作的时候，最好是选择1200的客户端对象进行通信。<br />
  /// <b>[Authorization]</b> The virtual server of Siemens S7 protocol supports TCP protocol. It simulates 1200 PLC for communication. When the client is operating, it is best to select the 1200 client object for communication.
  /// </summary>
  /// <remarks>
  /// 本西门子的虚拟PLC仅限商业授权用户使用，感谢支持。
  /// <note type="important">对于200smartPLC的V区，就是DB1.X，例如，V100=DB1.100</note>
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
  ///   <item>
  ///     <term>定时器的值</term>
  ///     <term>T</term>
  ///     <term>T100,T200</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term>未测试通过</term>
  ///   </item>
  ///   <item>
  ///     <term>计数器的值</term>
  ///     <term>C</term>
  ///     <term>C100,C200</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term>未测试通过</term>
  ///   </item>
  /// </list>
  /// 你可以很快速并且简单的创建一个虚拟的s7服务器
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\SiemensS7ServerExample.cs" region="UseExample1" title="简单的创建服务器" />
  /// 当然如果需要高级的服务器，指定日志，限制客户端的IP地址，获取客户端发送的信息，在服务器初始化的时候就要参照下面的代码：
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\SiemensS7ServerExample.cs" region="UseExample4" title="定制服务器" />
  /// 服务器创建好之后，我们就可以对服务器进行一些读写的操作了，下面的代码是基础的BCL类型的读写操作。
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\SiemensS7ServerExample.cs" region="ReadWriteExample" title="基础的读写示例" />
  /// 高级的对于byte数组类型的数据进行批量化的读写操作如下：
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\SiemensS7ServerExample.cs" region="BytesReadWrite" title="字节的读写示例" />
  /// 更高级操作请参见源代码。
  /// </example>
  public class SiemensS7Server : NetworkDataServerBase
  {
    private SoftBuffer inputBuffer;
    private SoftBuffer outputBuffer;
    private SoftBuffer memeryBuffer;
    private SoftBuffer countBuffer;
    private SoftBuffer timerBuffer;
    private SoftBuffer db1BlockBuffer;
    private SoftBuffer db2BlockBuffer;
    private SoftBuffer db3BlockBuffer;
    private SoftBuffer dbOtherBlockBuffer;
    private SoftBuffer aiBuffer;
    private SoftBuffer aqBuffer;
    private const int DataPoolLength = 65536;

    /// <summary>
    /// 实例化一个S7协议的服务器，支持I，Q，M，DB1.X, DB2.X, DB3.X 数据区块的读写操作<br />
    /// Instantiate a server with S7 protocol, support I, Q, M, DB1.X data block read and write operations
    /// </summary>
    public SiemensS7Server()
    {
      this.inputBuffer = new SoftBuffer(65536);
      this.outputBuffer = new SoftBuffer(65536);
      this.memeryBuffer = new SoftBuffer(65536);
      this.db1BlockBuffer = new SoftBuffer(65536);
      this.db2BlockBuffer = new SoftBuffer(65536);
      this.db3BlockBuffer = new SoftBuffer(65536);
      this.dbOtherBlockBuffer = new SoftBuffer(65536);
      this.countBuffer = new SoftBuffer(131072);
      this.timerBuffer = new SoftBuffer(131072);
      this.aiBuffer = new SoftBuffer(65536);
      this.aqBuffer = new SoftBuffer(65536);
      this.WordLength = (ushort) 2;
      this.ByteTransform = (IByteTransform) new ReverseBytesTransform();
    }

    private OperateResult<SoftBuffer> GetDataAreaFromS7Address(
      S7AddressData s7Address)
    {
      switch (s7Address.DataCode)
      {
        case 6:
          return OperateResult.CreateSuccessResult<SoftBuffer>(this.aiBuffer);
        case 7:
          return OperateResult.CreateSuccessResult<SoftBuffer>(this.aqBuffer);
        case 30:
          return OperateResult.CreateSuccessResult<SoftBuffer>(this.countBuffer);
        case 31:
          return OperateResult.CreateSuccessResult<SoftBuffer>(this.timerBuffer);
        case 129:
          return OperateResult.CreateSuccessResult<SoftBuffer>(this.inputBuffer);
        case 130:
          return OperateResult.CreateSuccessResult<SoftBuffer>(this.outputBuffer);
        case 131:
          return OperateResult.CreateSuccessResult<SoftBuffer>(this.memeryBuffer);
        case 132:
          if (s7Address.DbBlock == (ushort) 1)
            return OperateResult.CreateSuccessResult<SoftBuffer>(this.db1BlockBuffer);
          if (s7Address.DbBlock == (ushort) 2)
            return OperateResult.CreateSuccessResult<SoftBuffer>(this.db2BlockBuffer);
          return s7Address.DbBlock == (ushort) 3 ? OperateResult.CreateSuccessResult<SoftBuffer>(this.db3BlockBuffer) : OperateResult.CreateSuccessResult<SoftBuffer>(this.dbOtherBlockBuffer);
        default:
          return new OperateResult<SoftBuffer>(StringResources.Language.NotSupportedDataType);
      }
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Siemens.SiemensS7Net.Read(System.String,System.UInt16)" />
    [EstMqttApi("ReadByteArray", "")]
    public override OperateResult<byte[]> Read(string address, ushort length)
    {
      OperateResult<S7AddressData> from = S7AddressData.ParseFrom(address, length);
      if (!from.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) from);
      OperateResult<SoftBuffer> areaFromS7Address = this.GetDataAreaFromS7Address(from.Content);
      if (!areaFromS7Address.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) areaFromS7Address);
      return from.Content.DataCode == (byte) 30 || from.Content.DataCode == (byte) 31 ? OperateResult.CreateSuccessResult<byte[]>(areaFromS7Address.Content.GetBytes(from.Content.AddressStart * 2, (int) length * 2)) : OperateResult.CreateSuccessResult<byte[]>(areaFromS7Address.Content.GetBytes(from.Content.AddressStart / 8, (int) length));
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Siemens.SiemensS7Net.Write(System.String,System.Byte[])" />
    [EstMqttApi("WriteByteArray", "")]
    public override OperateResult Write(string address, byte[] value)
    {
      OperateResult<S7AddressData> from = S7AddressData.ParseFrom(address);
      if (!from.IsSuccess)
        return (OperateResult) OperateResult.CreateFailedResult<byte[]>((OperateResult) from);
      OperateResult<SoftBuffer> areaFromS7Address = this.GetDataAreaFromS7Address(from.Content);
      if (!areaFromS7Address.IsSuccess)
        return (OperateResult) OperateResult.CreateFailedResult<byte[]>((OperateResult) areaFromS7Address);
      if (from.Content.DataCode == (byte) 30 || from.Content.DataCode == (byte) 31)
        areaFromS7Address.Content.SetBytes(value, from.Content.AddressStart * 2);
      else
        areaFromS7Address.Content.SetBytes(value, from.Content.AddressStart / 8);
      return OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Siemens.SiemensS7Net.ReadByte(System.String)" />
    [EstMqttApi("ReadByte", "")]
    public OperateResult<byte> ReadByte(string address) => ByteTransformHelper.GetResultFromArray<byte>(this.Read(address, (ushort) 1));

    /// <inheritdoc cref="M:EstCommunication.Profinet.Siemens.SiemensS7Net.Write(System.String,System.Byte)" />
    [EstMqttApi("WriteByte", "")]
    public OperateResult Write(string address, byte value) => this.Write(address, new byte[1]
    {
      value
    });

    /// <inheritdoc cref="M:EstCommunication.Profinet.Siemens.SiemensS7Net.ReadBool(System.String)" />
    [EstMqttApi("ReadBool", "")]
    public override OperateResult<bool> ReadBool(string address)
    {
      OperateResult<S7AddressData> from = S7AddressData.ParseFrom(address);
      if (!from.IsSuccess)
        return OperateResult.CreateFailedResult<bool>((OperateResult) from);
      OperateResult<SoftBuffer> areaFromS7Address = this.GetDataAreaFromS7Address(from.Content);
      return !areaFromS7Address.IsSuccess ? OperateResult.CreateFailedResult<bool>((OperateResult) areaFromS7Address) : OperateResult.CreateSuccessResult<bool>(areaFromS7Address.Content.GetBool(from.Content.AddressStart));
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Siemens.SiemensS7Net.Write(System.String,System.Boolean)" />
    [EstMqttApi("WriteBool", "")]
    public override OperateResult Write(string address, bool value)
    {
      OperateResult<S7AddressData> from = S7AddressData.ParseFrom(address);
      if (!from.IsSuccess)
        return (OperateResult) from;
      OperateResult<SoftBuffer> areaFromS7Address = this.GetDataAreaFromS7Address(from.Content);
      if (!areaFromS7Address.IsSuccess)
        return (OperateResult) areaFromS7Address;
      areaFromS7Address.Content.SetBool(value, from.Content.AddressStart);
      return OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc />
    protected override void ThreadPoolLoginAfterClientCheck(Socket socket, IPEndPoint endPoint)
    {
      S7Message s7Message = new S7Message();
      if (!this.ReceiveByMessage(socket, 5000, (INetMessage) s7Message).IsSuccess || !this.Send(socket, SoftBasic.HexStringToBytes("03 00 00 16 02 D0 80 32 01 00 00 02 00 00 08 00 00 f0 00 00 01 00")).IsSuccess || (!this.ReceiveByMessage(socket, 5000, (INetMessage) s7Message).IsSuccess || !this.Send(socket, SoftBasic.HexStringToBytes("03 00 00 1B 02 f0 80 32 01 00 00 02 00 00 08 00 00 00 00 00 01 00 01 00 f0 00 f0")).IsSuccess))
        return;
      AppSession session = new AppSession();
      session.IpEndPoint = endPoint;
      session.WorkSocket = socket;
      try
      {
        socket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, new AsyncCallback(this.SocketAsyncCallBack), (object) session);
        this.AddClient(session);
      }
      catch
      {
        socket.Close();
        this.LogNet?.WriteDebug(this.ToString(), string.Format(StringResources.Language.ClientOfflineInfo, (object) endPoint));
      }
    }

    private async void SocketAsyncCallBack(IAsyncResult ar)
    {
      if (!(ar.AsyncState is AppSession session))
      {
        session = (AppSession) null;
      }
      else
      {
        try
        {
          int receiveCount = session.WorkSocket.EndReceive(ar);
          OperateResult<byte[]> read1 = await this.ReceiveByMessageAsync(session.WorkSocket, 5000, (INetMessage) new S7Message());
          if (!read1.IsSuccess)
          {
            this.RemoveClient(session);
            session = (AppSession) null;
            return;
          }
          if (!EstCommunication.Authorization.asdniasnfaksndiqwhawfskhfaiw())
          {
            this.RemoveClient(session);
            session = (AppSession) null;
            return;
          }
          this.LogNet?.WriteDebug(this.ToString(), string.Format("[{0}] Tcp {1}：{2}", (object) session.IpEndPoint, (object) StringResources.Language.Receive, (object) read1.Content.ToHexString(' ')));
          byte[] receive = read1.Content;
          byte[] back = (byte[]) null;
          if (receive[17] == (byte) 4)
            back = this.ReadByMessage(receive);
          else if (receive[17] == (byte) 5)
            back = this.WriteByMessage(receive);
          else if (receive[17] == (byte) 0)
          {
            back = SoftBasic.HexStringToBytes("03 00 00 7D 02 F0 80 32 07 00 00 00 01 00 0C 00 60 00 01 12 08 12 84 01 01 00 00 00 00 FF 09 00 5C 00 11 00 00 00 1C 00 03 00 01 36 45 53 37 20 32 31 35 2D 31 41 47 34 30 2D 30 58 42 30 20 00 00 00 06 20 20 00 06 36 45 53 37 20 32 31 35 2D 31 41 47 34 30 2D 30 58 42 30 20 00 00 00 06 20 20 00 07 36 45 53 37 20 32 31 35 2D 31 41 47 34 30 2D 30 58 42 30 20 00 00 56 04 02 01");
          }
          else
          {
            this.RemoveClient(session);
            session = (AppSession) null;
            return;
          }
          session.WorkSocket.Send(back);
          this.LogNet?.WriteDebug(this.ToString(), string.Format("[{0}] Tcp {1}：{2}", (object) session.IpEndPoint, (object) StringResources.Language.Send, (object) back.ToHexString(' ')));
          session.HeartTime = DateTime.Now;
          this.RaiseDataReceived((object) session, receive);
          session.WorkSocket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, new AsyncCallback(this.SocketAsyncCallBack), (object) session);
          read1 = (OperateResult<byte[]>) null;
          receive = (byte[]) null;
          back = (byte[]) null;
        }
        catch
        {
          this.RemoveClient(session);
        }
        session = (AppSession) null;
      }
    }

    private byte[] ReadByMessage(byte[] packCommand)
    {
      List<byte> byteList = new List<byte>();
      int num1 = (int) packCommand[18];
      int index1 = 19;
      for (int index2 = 0; index2 < num1; ++index2)
      {
        byte num2 = packCommand[index1 + 1];
        byte[] command = packCommand.SelectMiddle<byte>(index1, (int) num2 + 2);
        index1 += (int) num2 + 2;
        byteList.AddRange((IEnumerable<byte>) this.ReadByCommand(command));
      }
      byte[] array = new byte[21 + byteList.Count];
      SoftBasic.HexStringToBytes("03 00 00 1A 02 F0 80 32 03 00 00 00 01 00 02 00 05 00 00 04 01").CopyTo((Array) array, 0);
      array[2] = (byte) (array.Length / 256);
      array[3] = (byte) (array.Length % 256);
      array[15] = (byte) (byteList.Count / 256);
      array[16] = (byte) (byteList.Count % 256);
      array[20] = packCommand[18];
      byteList.CopyTo(array, 21);
      return array;
    }

    private byte[] ReadByCommand(byte[] command)
    {
      if (command[3] == (byte) 1)
      {
        int destIndex = (int) command[9] * 65536 + (int) command[10] * 256 + (int) command[11];
        ushort num = this.ByteTransform.TransUInt16(command, 6);
        S7AddressData s7Address = new S7AddressData();
        s7Address.AddressStart = destIndex;
        s7Address.DataCode = command[8];
        s7Address.DbBlock = num;
        s7Address.Length = (ushort) 1;
        OperateResult<SoftBuffer> areaFromS7Address = this.GetDataAreaFromS7Address(s7Address);
        if (!areaFromS7Address.IsSuccess)
          throw new Exception(areaFromS7Address.Message);
        return this.PackReadBitCommandBack(areaFromS7Address.Content.GetBool(destIndex));
      }
      if (command[3] == (byte) 30 || command[3] == (byte) 31)
      {
        ushort num1 = this.ByteTransform.TransUInt16(command, 4);
        int num2 = (int) command[9] * 65536 + (int) command[10] * 256 + (int) command[11];
        S7AddressData s7Address = new S7AddressData();
        s7Address.AddressStart = num2;
        s7Address.DataCode = command[8];
        s7Address.DbBlock = (ushort) 0;
        s7Address.Length = num1;
        OperateResult<SoftBuffer> areaFromS7Address = this.GetDataAreaFromS7Address(s7Address);
        if (!areaFromS7Address.IsSuccess)
          throw new Exception(areaFromS7Address.Message);
        return this.PackReadCTCommandBack(areaFromS7Address.Content.GetBytes(num2 * 2, (int) num1 * 2), command[3] == (byte) 30 ? 3 : 5);
      }
      ushort num3 = this.ByteTransform.TransUInt16(command, 4);
      if (command[3] == (byte) 4)
        num3 *= (ushort) 2;
      ushort num4 = this.ByteTransform.TransUInt16(command, 6);
      int index = ((int) command[9] * 65536 + (int) command[10] * 256 + (int) command[11]) / 8;
      S7AddressData s7Address1 = new S7AddressData();
      s7Address1.AddressStart = index;
      s7Address1.DataCode = command[8];
      s7Address1.DbBlock = num4;
      s7Address1.Length = num3;
      OperateResult<SoftBuffer> areaFromS7Address1 = this.GetDataAreaFromS7Address(s7Address1);
      if (!areaFromS7Address1.IsSuccess)
        throw new Exception(areaFromS7Address1.Message);
      return this.PackReadWordCommandBack(areaFromS7Address1.Content.GetBytes(index, (int) num3));
    }

    private byte[] PackReadWordCommandBack(byte[] result)
    {
      byte[] numArray = new byte[4 + result.Length];
      numArray[0] = byte.MaxValue;
      numArray[1] = (byte) 4;
      this.ByteTransform.TransByte((ushort) result.Length).CopyTo((Array) numArray, 2);
      result.CopyTo((Array) numArray, 4);
      return numArray;
    }

    private byte[] PackReadCTCommandBack(byte[] result, int dataLength)
    {
      byte[] numArray = new byte[4 + result.Length * dataLength / 2];
      numArray[0] = byte.MaxValue;
      numArray[1] = (byte) 9;
      this.ByteTransform.TransByte((ushort) (numArray.Length - 4)).CopyTo((Array) numArray, 2);
      for (int index = 0; index < result.Length / 2; ++index)
        result.SelectMiddle<byte>(index * 2, 2).CopyTo((Array) numArray, 4 + dataLength - 2 + index * dataLength);
      return numArray;
    }

    private byte[] PackReadBitCommandBack(bool value) => new byte[5]
    {
      byte.MaxValue,
      (byte) 3,
      (byte) 0,
      (byte) 1,
      value ? (byte) 1 : (byte) 0
    };

    private byte[] WriteByMessage(byte[] packCommand)
    {
      if (!this.EnableWrite)
        return SoftBasic.HexStringToBytes("03 00 00 16 02 F0 80 32 03 00 00 00 01 00 02 00 01 00 00 05 01 04");
      if (packCommand[22] == (byte) 2 || packCommand[22] == (byte) 4)
      {
        ushort num = this.ByteTransform.TransUInt16(packCommand, 25);
        int length = (int) this.ByteTransform.TransInt16(packCommand, 23);
        if (packCommand[22] == (byte) 4)
          length *= 2;
        int destIndex = ((int) packCommand[28] * 65536 + (int) packCommand[29] * 256 + (int) packCommand[30]) / 8;
        byte[] data = this.ByteTransform.TransByte(packCommand, 35, length);
        S7AddressData s7Address = new S7AddressData();
        s7Address.DataCode = packCommand[27];
        s7Address.DbBlock = num;
        s7Address.Length = (ushort) 1;
        OperateResult<SoftBuffer> areaFromS7Address = this.GetDataAreaFromS7Address(s7Address);
        if (!areaFromS7Address.IsSuccess)
          throw new Exception(areaFromS7Address.Message);
        areaFromS7Address.Content.SetBytes(data, destIndex);
        return SoftBasic.HexStringToBytes("03 00 00 16 02 F0 80 32 03 00 00 00 01 00 02 00 01 00 00 05 01 FF");
      }
      ushort num1 = this.ByteTransform.TransUInt16(packCommand, 25);
      int destIndex1 = (int) packCommand[28] * 65536 + (int) packCommand[29] * 256 + (int) packCommand[30];
      bool flag = packCommand[35] > (byte) 0;
      S7AddressData s7Address1 = new S7AddressData();
      s7Address1.DataCode = packCommand[27];
      s7Address1.DbBlock = num1;
      s7Address1.Length = (ushort) 1;
      OperateResult<SoftBuffer> areaFromS7Address1 = this.GetDataAreaFromS7Address(s7Address1);
      if (!areaFromS7Address1.IsSuccess)
        throw new Exception(areaFromS7Address1.Message);
      areaFromS7Address1.Content.SetBool(flag, destIndex1);
      return SoftBasic.HexStringToBytes("03 00 00 16 02 F0 80 32 03 00 00 00 01 00 02 00 01 00 00 05 01 FF");
    }

    /// <inheritdoc />
    protected override void LoadFromBytes(byte[] content)
    {
      if (content.Length < 458752)
        throw new Exception("File is not correct");
      this.inputBuffer.SetBytes(content, 0, 0, 65536);
      this.outputBuffer.SetBytes(content, 65536, 0, 65536);
      this.memeryBuffer.SetBytes(content, 131072, 0, 65536);
      this.db1BlockBuffer.SetBytes(content, 196608, 0, 65536);
      this.db2BlockBuffer.SetBytes(content, 262144, 0, 65536);
      this.db3BlockBuffer.SetBytes(content, 327680, 0, 65536);
      this.dbOtherBlockBuffer.SetBytes(content, 393216, 0, 65536);
      if (content.Length < 720896)
        return;
      this.countBuffer.SetBytes(content, 458752, 0, 131072);
      this.timerBuffer.SetBytes(content, 589824, 0, 131072);
    }

    /// <inheritdoc />
    protected override byte[] SaveToBytes()
    {
      byte[] numArray = new byte[720896];
      Array.Copy((Array) this.inputBuffer.GetBytes(), 0, (Array) numArray, 0, 65536);
      Array.Copy((Array) this.outputBuffer.GetBytes(), 0, (Array) numArray, 65536, 65536);
      Array.Copy((Array) this.memeryBuffer.GetBytes(), 0, (Array) numArray, 131072, 65536);
      Array.Copy((Array) this.db1BlockBuffer.GetBytes(), 0, (Array) numArray, 196608, 65536);
      Array.Copy((Array) this.db2BlockBuffer.GetBytes(), 0, (Array) numArray, 262144, 65536);
      Array.Copy((Array) this.db3BlockBuffer.GetBytes(), 0, (Array) numArray, 327680, 65536);
      Array.Copy((Array) this.dbOtherBlockBuffer.GetBytes(), 0, (Array) numArray, 393216, 65536);
      Array.Copy((Array) this.countBuffer.GetBytes(), 0, (Array) numArray, 458752, 131072);
      Array.Copy((Array) this.timerBuffer.GetBytes(), 0, (Array) numArray, 589824, 131072);
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
    public override string ToString() => string.Format("SiemensS7Server[{0}]", (object) this.Port);
  }
}
