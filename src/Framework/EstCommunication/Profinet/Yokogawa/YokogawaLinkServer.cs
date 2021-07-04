// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.Yokogawa.YokogawaLinkServer
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
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EstCommunication.Profinet.Yokogawa
{
  /// <summary>
  /// <b>[商业授权]</b> 横河PLC的虚拟服务器，支持X,Y,I,E,M,T,C,L继电器类型的数据读写，支持D,B,F,R,V,Z,W,TN,CN寄存器类型的数据读写，可以用来测试横河PLC的二进制通信类型<br />
  /// <b>[Authorization]</b> Yokogawa PLC's virtual server, supports X, Y, I, E, M, T, C, L relay type data read and write,
  /// supports D, B, F, R, V, Z, W, TN, CN register types The data read and write can be used to test the binary communication type of Yokogawa PLC
  /// </summary>
  /// <remarks>
  /// 其中的X继电器可以在服务器进行读写操作，但是远程的PLC只能进行读取，所有的数据读写的最大的范围按照协议进行了限制。
  /// </remarks>
  /// <example>
  /// 地址示例如下：
  /// <list type="table">
  ///   <listheader>
  ///     <term>地址名称</term>
  ///     <term>地址代号</term>
  ///     <term>示例</term>
  ///     <term>字操作</term>
  ///     <term>位操作</term>
  ///     <term>备注</term>
  ///   </listheader>
  ///   <item>
  ///     <term>Input relay</term>
  ///     <term>X</term>
  ///     <term>X100,X200</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term>服务器端可读可写</term>
  ///   </item>
  ///   <item>
  ///     <term>Output relay</term>
  ///     <term>Y</term>
  ///     <term>Y100,Y200</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>Internal relay</term>
  ///     <term>I</term>
  ///     <term>I100,I200</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>Share relay</term>
  ///     <term>E</term>
  ///     <term>E100,E200</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>Special relay</term>
  ///     <term>M</term>
  ///     <term>M100,M200</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>Time relay</term>
  ///     <term>T</term>
  ///     <term>T100,T200</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>Counter relay</term>
  ///     <term>C</term>
  ///     <term>C100,C200</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>link relay</term>
  ///     <term>L</term>
  ///     <term>L100, L200</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>Data register</term>
  ///     <term>D</term>
  ///     <term>D100,D200</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>File register</term>
  ///     <term>B</term>
  ///     <term>B100,B200</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>Cache register</term>
  ///     <term>F</term>
  ///     <term>F100,F200</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>Shared register</term>
  ///     <term>R</term>
  ///     <term>R100,R200</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>Index register</term>
  ///     <term>V</term>
  ///     <term>V100,V200</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>Special register</term>
  ///     <term>Z</term>
  ///     <term>Z100,Z200</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>Link register</term>
  ///     <term>W</term>
  ///     <term>W100,W200</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>Timer current value</term>
  ///     <term>TN</term>
  ///     <term>TN100,TN200</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>Counter current value</term>
  ///     <term>CN</term>
  ///     <term>CN100,CN200</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term></term>
  ///   </item>
  /// </list>
  /// 你可以很快速并且简单的创建一个虚拟的横河服务器
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\YokogawaLinkServerSample.cs" region="UseExample1" title="简单的创建服务器" />
  /// 当然如果需要高级的服务器，指定日志，限制客户端的IP地址，获取客户端发送的信息，在服务器初始化的时候就要参照下面的代码：
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\YokogawaLinkServerSample.cs" region="UseExample4" title="定制服务器" />
  /// 服务器创建好之后，我们就可以对服务器进行一些读写的操作了，下面的代码是基础的BCL类型的读写操作。
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\YokogawaLinkServerSample.cs" region="ReadWriteExample" title="基础的读写示例" />
  /// 高级的对于byte数组类型的数据进行批量化的读写操作如下：
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\YokogawaLinkServerSample.cs" region="BytesReadWrite" title="字节的读写示例" />
  /// 更高级操作请参见源代码。
  /// </example>
  public class YokogawaLinkServer : NetworkDataServerBase
  {
    private SoftBuffer xBuffer;
    private SoftBuffer yBuffer;
    private SoftBuffer iBuffer;
    private SoftBuffer eBuffer;
    private SoftBuffer mBuffer;
    private SoftBuffer lBuffer;
    private SoftBuffer dBuffer;
    private SoftBuffer bBuffer;
    private SoftBuffer fBuffer;
    private SoftBuffer rBuffer;
    private SoftBuffer vBuffer;
    private SoftBuffer zBuffer;
    private SoftBuffer wBuffer;
    private SoftBuffer specialBuffer;
    private const int DataPoolLength = 65536;
    private IByteTransform transform;
    private bool isProgramStarted = false;

    /// <summary>
    /// 实例化一个横河PLC的服务器，支持X,Y,I,E,M,T,C,L继电器类型的数据读写，支持D,B,F,R,V,Z,W,TN,CN寄存器类型的数据读写<br />
    /// Instantiate a Yokogawa PLC server, support X, Y, I, E, M, T, C, L relay type data read and write,
    /// support D, B, F, R, V, Z, W, TN, CN Register type data reading and writing
    /// </summary>
    public YokogawaLinkServer()
    {
      this.xBuffer = new SoftBuffer(65536);
      this.yBuffer = new SoftBuffer(65536);
      this.iBuffer = new SoftBuffer(65536);
      this.eBuffer = new SoftBuffer(65536);
      this.mBuffer = new SoftBuffer(65536);
      this.lBuffer = new SoftBuffer(65536);
      this.dBuffer = new SoftBuffer(131072);
      this.bBuffer = new SoftBuffer(131072);
      this.fBuffer = new SoftBuffer(131072);
      this.rBuffer = new SoftBuffer(131072);
      this.vBuffer = new SoftBuffer(131072);
      this.zBuffer = new SoftBuffer(131072);
      this.wBuffer = new SoftBuffer(131072);
      this.specialBuffer = new SoftBuffer(131072);
      this.WordLength = (ushort) 2;
      this.ByteTransform = (IByteTransform) new ReverseWordTransform();
      this.ByteTransform.DataFormat = DataFormat.CDAB;
      this.transform = (IByteTransform) new ReverseBytesTransform();
    }

    private OperateResult<SoftBuffer> GetDataAreaFromYokogawaAddress(
      YokogawaLinkAddress yokogawaAddress,
      bool isBit)
    {
      if (isBit)
      {
        switch (yokogawaAddress.DataCode)
        {
          case 5:
            return OperateResult.CreateSuccessResult<SoftBuffer>(this.eBuffer);
          case 9:
            return OperateResult.CreateSuccessResult<SoftBuffer>(this.iBuffer);
          case 12:
            return OperateResult.CreateSuccessResult<SoftBuffer>(this.lBuffer);
          case 13:
            return OperateResult.CreateSuccessResult<SoftBuffer>(this.mBuffer);
          case 24:
            return OperateResult.CreateSuccessResult<SoftBuffer>(this.xBuffer);
          case 25:
            return OperateResult.CreateSuccessResult<SoftBuffer>(this.yBuffer);
          default:
            return new OperateResult<SoftBuffer>(StringResources.Language.NotSupportedDataType);
        }
      }
      else
      {
        switch (yokogawaAddress.DataCode)
        {
          case 2:
            return OperateResult.CreateSuccessResult<SoftBuffer>(this.bBuffer);
          case 4:
            return OperateResult.CreateSuccessResult<SoftBuffer>(this.dBuffer);
          case 5:
            return OperateResult.CreateSuccessResult<SoftBuffer>(this.eBuffer);
          case 6:
            return OperateResult.CreateSuccessResult<SoftBuffer>(this.fBuffer);
          case 9:
            return OperateResult.CreateSuccessResult<SoftBuffer>(this.iBuffer);
          case 12:
            return OperateResult.CreateSuccessResult<SoftBuffer>(this.lBuffer);
          case 13:
            return OperateResult.CreateSuccessResult<SoftBuffer>(this.mBuffer);
          case 18:
            return OperateResult.CreateSuccessResult<SoftBuffer>(this.rBuffer);
          case 22:
            return OperateResult.CreateSuccessResult<SoftBuffer>(this.vBuffer);
          case 23:
            return OperateResult.CreateSuccessResult<SoftBuffer>(this.wBuffer);
          case 24:
            return OperateResult.CreateSuccessResult<SoftBuffer>(this.xBuffer);
          case 25:
            return OperateResult.CreateSuccessResult<SoftBuffer>(this.yBuffer);
          case 26:
            return OperateResult.CreateSuccessResult<SoftBuffer>(this.zBuffer);
          default:
            return new OperateResult<SoftBuffer>(StringResources.Language.NotSupportedDataType);
        }
      }
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Yokogawa.YokogawaLinkTcp.Read(System.String,System.UInt16)" />
    [EstMqttApi("ReadByteArray", "")]
    public override OperateResult<byte[]> Read(string address, ushort length)
    {
      if (address.StartsWith("Special:") || address.StartsWith("special:"))
      {
        address = address.Substring(8);
        EstHelper.ExtractParameter(ref address, "unit");
        EstHelper.ExtractParameter(ref address, "slot");
        try
        {
          return OperateResult.CreateSuccessResult<byte[]>(this.specialBuffer.GetBytes((int) ushort.Parse(address) * 2, (int) length * 2));
        }
        catch (Exception ex)
        {
          return new OperateResult<byte[]>("Address format wrong: " + ex.Message);
        }
      }
      else
      {
        OperateResult<YokogawaLinkAddress> from = YokogawaLinkAddress.ParseFrom(address, length);
        if (!from.IsSuccess)
          return OperateResult.CreateFailedResult<byte[]>((OperateResult) from);
        OperateResult<SoftBuffer> fromYokogawaAddress = this.GetDataAreaFromYokogawaAddress(from.Content, false);
        if (!fromYokogawaAddress.IsSuccess)
          return OperateResult.CreateFailedResult<byte[]>((OperateResult) fromYokogawaAddress);
        return from.Content.DataCode == 24 || from.Content.DataCode == 25 || (from.Content.DataCode == 9 || from.Content.DataCode == 5) || from.Content.DataCode == 13 || from.Content.DataCode == 12 ? OperateResult.CreateSuccessResult<byte[]>(((IEnumerable<byte>) fromYokogawaAddress.Content.GetBytes(from.Content.AddressStart, (int) length * 16)).Select<byte, bool>((Func<byte, bool>) (m => m > (byte) 0)).ToArray<bool>().ToByteArray()) : OperateResult.CreateSuccessResult<byte[]>(fromYokogawaAddress.Content.GetBytes(from.Content.AddressStart * 2, (int) length * 2));
      }
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Yokogawa.YokogawaLinkTcp.Write(System.String,System.Byte[])" />
    [EstMqttApi("WriteByteArray", "")]
    public override OperateResult Write(string address, byte[] value)
    {
      if (address.StartsWith("Special:") || address.StartsWith("special:"))
      {
        address = address.Substring(8);
        EstHelper.ExtractParameter(ref address, "unit");
        EstHelper.ExtractParameter(ref address, "slot");
        try
        {
          this.specialBuffer.SetBytes(value, (int) ushort.Parse(address) * 2);
          return OperateResult.CreateSuccessResult();
        }
        catch (Exception ex)
        {
          return new OperateResult("Address format wrong: " + ex.Message);
        }
      }
      else
      {
        OperateResult<YokogawaLinkAddress> from = YokogawaLinkAddress.ParseFrom(address, (ushort) 0);
        if (!from.IsSuccess)
          return (OperateResult) OperateResult.CreateFailedResult<byte[]>((OperateResult) from);
        OperateResult<SoftBuffer> fromYokogawaAddress = this.GetDataAreaFromYokogawaAddress(from.Content, false);
        if (!fromYokogawaAddress.IsSuccess)
          return (OperateResult) OperateResult.CreateFailedResult<byte[]>((OperateResult) fromYokogawaAddress);
        if (from.Content.DataCode == 24 || from.Content.DataCode == 25 || (from.Content.DataCode == 9 || from.Content.DataCode == 5) || from.Content.DataCode == 13 || from.Content.DataCode == 12)
          fromYokogawaAddress.Content.SetBytes(((IEnumerable<bool>) value.ToBoolArray()).Select<bool, byte>((Func<bool, byte>) (m => !m ? (byte) 0 : (byte) 1)).ToArray<byte>(), from.Content.AddressStart);
        else
          fromYokogawaAddress.Content.SetBytes(value, from.Content.AddressStart * 2);
        return OperateResult.CreateSuccessResult();
      }
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Yokogawa.YokogawaLinkTcp.ReadBool(System.String,System.UInt16)" />
    [EstMqttApi("ReadBoolArray", "")]
    public override OperateResult<bool[]> ReadBool(string address, ushort length)
    {
      OperateResult<YokogawaLinkAddress> from = YokogawaLinkAddress.ParseFrom(address, length);
      if (!from.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) from);
      OperateResult<SoftBuffer> fromYokogawaAddress = this.GetDataAreaFromYokogawaAddress(from.Content, true);
      return !fromYokogawaAddress.IsSuccess ? OperateResult.CreateFailedResult<bool[]>((OperateResult) fromYokogawaAddress) : OperateResult.CreateSuccessResult<bool[]>(((IEnumerable<byte>) this.mBuffer.GetBytes(from.Content.AddressStart, (int) length)).Select<byte, bool>((Func<byte, bool>) (m => m > (byte) 0)).ToArray<bool>());
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Yokogawa.YokogawaLinkTcp.Write(System.String,System.Boolean[])" />
    [EstMqttApi("WriteBoolArray", "")]
    public override OperateResult Write(string address, bool[] value)
    {
      OperateResult<YokogawaLinkAddress> from = YokogawaLinkAddress.ParseFrom(address, (ushort) 0);
      if (!from.IsSuccess)
        return (OperateResult) from;
      OperateResult<SoftBuffer> fromYokogawaAddress = this.GetDataAreaFromYokogawaAddress(from.Content, true);
      if (!fromYokogawaAddress.IsSuccess)
        return (OperateResult) fromYokogawaAddress;
      fromYokogawaAddress.Content.SetBytes(((IEnumerable<bool>) value).Select<bool, byte>((Func<bool, byte>) (m => !m ? (byte) 0 : (byte) 1)).ToArray<byte>(), from.Content.AddressStart);
      return OperateResult.CreateSuccessResult();
    }

    /// <summary>
    /// 如果未执行程序，则开始执行程序<br />
    /// Starts executing a program if it is not being executed
    /// </summary>
    [EstMqttApi(Description = "Starts executing a program if it is not being executed")]
    public void StartProgram() => this.isProgramStarted = true;

    /// <summary>
    /// 停止当前正在执行程序<br />
    /// Stops the executing program.
    /// </summary>
    [EstMqttApi(Description = "Stops the executing program.")]
    public void StopProgram() => this.isProgramStarted = false;

    /// <inheritdoc />
    protected override void ThreadPoolLoginAfterClientCheck(Socket socket, IPEndPoint endPoint)
    {
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
          OperateResult<byte[]> read1 = await this.ReceiveByMessageAsync(session.WorkSocket, 5000, (INetMessage) new YokogawaLinkBinaryMessage());
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
          if (receive[0] == (byte) 1)
            back = this.ReadBoolByCommand(receive);
          else if (receive[0] == (byte) 2)
            back = this.WriteBoolByCommand(receive);
          else if (receive[0] == (byte) 4)
            back = this.ReadRandomBoolByCommand(receive);
          else if (receive[0] == (byte) 5)
            back = this.WriteRandomBoolByCommand(receive);
          else if (receive[0] == (byte) 17)
            back = this.ReadWordByCommand(receive);
          else if (receive[0] == (byte) 18)
            back = this.WriteWordByCommand(receive);
          else if (receive[0] == (byte) 20)
            back = this.ReadRandomWordByCommand(receive);
          else if (receive[0] == (byte) 21)
            back = this.WriteRandomWordByCommand(receive);
          else if (receive[0] == (byte) 49)
            back = this.ReadSpecialModule(receive);
          else if (receive[0] == (byte) 50)
            back = this.WriteSpecialModule(receive);
          else if (receive[0] == (byte) 69)
            back = this.StartByCommand(receive);
          else if (receive[0] == (byte) 70)
          {
            back = this.StopByCommand(receive);
          }
          else
          {
            if (receive[0] == (byte) 97)
              throw new RemoteCloseException();
            back = receive[0] != (byte) 98 ? (receive[0] != (byte) 99 ? this.PackCommandBack(receive[0], (byte) 3, (byte[]) null) : this.ReadSystemDateTime(receive)) : this.ReadSystemByCommand(receive);
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

    private byte[] ReadBoolByCommand(byte[] command)
    {
      int index = this.transform.TransInt32(command, 6);
      int length = (int) this.transform.TransUInt16(command, 10);
      if (index > (int) ushort.MaxValue || index < 0)
        return this.PackCommandBack(command[0], (byte) 4, (byte[]) null);
      if (length > 256)
        return this.PackCommandBack(command[0], (byte) 5, (byte[]) null);
      if (index + length > (int) ushort.MaxValue)
        return this.PackCommandBack(command[0], (byte) 5, (byte[]) null);
      switch (command[5])
      {
        case 5:
          return this.PackCommandBack(command[0], (byte) 0, this.eBuffer.GetBytes(index, length));
        case 9:
          return this.PackCommandBack(command[0], (byte) 0, this.iBuffer.GetBytes(index, length));
        case 12:
          return this.PackCommandBack(command[0], (byte) 0, this.lBuffer.GetBytes(index, length));
        case 13:
          return this.PackCommandBack(command[0], (byte) 0, this.mBuffer.GetBytes(index, length));
        case 24:
          return this.PackCommandBack(command[0], (byte) 0, this.xBuffer.GetBytes(index, length));
        case 25:
          return this.PackCommandBack(command[0], (byte) 0, this.yBuffer.GetBytes(index, length));
        default:
          return this.PackCommandBack(command[0], (byte) 3, (byte[]) null);
      }
    }

    private byte[] WriteBoolByCommand(byte[] command)
    {
      if (!this.EnableWrite)
        return this.PackCommandBack(command[0], (byte) 3, (byte[]) null);
      int destIndex = this.transform.TransInt32(command, 6);
      int num = (int) this.transform.TransUInt16(command, 10);
      if (destIndex > (int) ushort.MaxValue || destIndex < 0)
        return this.PackCommandBack(command[0], (byte) 4, (byte[]) null);
      if (num > 256)
        return this.PackCommandBack(command[0], (byte) 5, (byte[]) null);
      if (destIndex + num > (int) ushort.MaxValue)
        return this.PackCommandBack(command[0], (byte) 5, (byte[]) null);
      if (num != command.Length - 12)
        return this.PackCommandBack(command[0], (byte) 5, (byte[]) null);
      switch (command[5])
      {
        case 5:
          this.eBuffer.SetBytes(command.RemoveBegin<byte>(12), destIndex);
          return this.PackCommandBack(command[0], (byte) 0, (byte[]) null);
        case 9:
          this.iBuffer.SetBytes(command.RemoveBegin<byte>(12), destIndex);
          return this.PackCommandBack(command[0], (byte) 0, (byte[]) null);
        case 12:
          this.lBuffer.SetBytes(command.RemoveBegin<byte>(12), destIndex);
          return this.PackCommandBack(command[0], (byte) 0, (byte[]) null);
        case 13:
          this.mBuffer.SetBytes(command.RemoveBegin<byte>(12), destIndex);
          return this.PackCommandBack(command[0], (byte) 0, (byte[]) null);
        case 24:
          return this.PackCommandBack(command[0], (byte) 3, (byte[]) null);
        case 25:
          this.yBuffer.SetBytes(command.RemoveBegin<byte>(12), destIndex);
          return this.PackCommandBack(command[0], (byte) 0, (byte[]) null);
        default:
          return this.PackCommandBack(command[0], (byte) 3, (byte[]) null);
      }
    }

    private byte[] ReadRandomBoolByCommand(byte[] command)
    {
      int length = (int) this.transform.TransUInt16(command, 4);
      if (length > 32)
        return this.PackCommandBack(command[0], (byte) 5, (byte[]) null);
      if (length * 6 != command.Length - 6)
        return this.PackCommandBack(command[0], (byte) 5, (byte[]) null);
      byte[] result = new byte[length];
      for (int index1 = 0; index1 < length; ++index1)
      {
        int index2 = this.transform.TransInt32(command, 8 + 6 * index1);
        if (index2 > (int) ushort.MaxValue || index2 < 0)
          return this.PackCommandBack(command[0], (byte) 4, (byte[]) null);
        switch (command[7 + index1 * 6])
        {
          case 5:
            result[index1] = this.eBuffer.GetBytes(index2, 1)[0];
            break;
          case 9:
            result[index1] = this.iBuffer.GetBytes(index2, 1)[0];
            break;
          case 12:
            result[index1] = this.lBuffer.GetBytes(index2, 1)[0];
            break;
          case 13:
            result[index1] = this.mBuffer.GetBytes(index2, 1)[0];
            break;
          case 24:
            result[index1] = this.xBuffer.GetBytes(index2, 1)[0];
            break;
          case 25:
            result[index1] = this.yBuffer.GetBytes(index2, 1)[0];
            break;
          default:
            return this.PackCommandBack(command[0], (byte) 3, (byte[]) null);
        }
      }
      return this.PackCommandBack(command[0], (byte) 0, result);
    }

    private byte[] WriteRandomBoolByCommand(byte[] command)
    {
      if (!this.EnableWrite)
        return this.PackCommandBack(command[0], (byte) 3, (byte[]) null);
      int num = (int) this.transform.TransUInt16(command, 4);
      if (num > 32)
        return this.PackCommandBack(command[0], (byte) 5, (byte[]) null);
      if (num * 8 - 1 != command.Length - 6)
        return this.PackCommandBack(command[0], (byte) 5, (byte[]) null);
      for (int index1 = 0; index1 < num; ++index1)
      {
        int index2 = this.transform.TransInt32(command, 8 + 8 * index1);
        if (index2 > (int) ushort.MaxValue || index2 < 0)
          return this.PackCommandBack(command[0], (byte) 4, (byte[]) null);
        switch (command[7 + index1 * 8])
        {
          case 5:
            this.eBuffer.SetValue(command[12 + 8 * index1], index2);
            break;
          case 9:
            this.iBuffer.SetValue(command[12 + 8 * index1], index2);
            break;
          case 12:
            this.lBuffer.SetValue(command[12 + 8 * index1], index2);
            break;
          case 13:
            this.mBuffer.SetValue(command[12 + 8 * index1], index2);
            break;
          case 24:
            return this.PackCommandBack(command[0], (byte) 3, (byte[]) null);
          case 25:
            this.yBuffer.SetValue(command[12 + 8 * index1], index2);
            break;
          default:
            return this.PackCommandBack(command[0], (byte) 3, (byte[]) null);
        }
      }
      return this.PackCommandBack(command[0], (byte) 0, (byte[]) null);
    }

    private byte[] ReadWordByCommand(byte[] command)
    {
      int index = this.transform.TransInt32(command, 6);
      int num = (int) this.transform.TransUInt16(command, 10);
      if (index > (int) ushort.MaxValue || index < 0)
        return this.PackCommandBack(command[0], (byte) 4, (byte[]) null);
      if (num > 64)
        return this.PackCommandBack(command[0], (byte) 5, (byte[]) null);
      if (index + num > (int) ushort.MaxValue)
        return this.PackCommandBack(command[0], (byte) 5, (byte[]) null);
      switch (command[5])
      {
        case 2:
          return this.PackCommandBack(command[0], (byte) 0, this.bBuffer.GetBytes(index * 2, num * 2));
        case 4:
          return this.PackCommandBack(command[0], (byte) 0, this.dBuffer.GetBytes(index * 2, num * 2));
        case 5:
          return this.PackCommandBack(command[0], (byte) 0, ((IEnumerable<byte>) this.eBuffer.GetBytes(index, num * 16)).Select<byte, bool>((Func<byte, bool>) (m => m > (byte) 0)).ToArray<bool>().ToByteArray());
        case 6:
          return this.PackCommandBack(command[0], (byte) 0, this.fBuffer.GetBytes(index * 2, num * 2));
        case 9:
          return this.PackCommandBack(command[0], (byte) 0, ((IEnumerable<byte>) this.iBuffer.GetBytes(index, num * 16)).Select<byte, bool>((Func<byte, bool>) (m => m > (byte) 0)).ToArray<bool>().ToByteArray());
        case 12:
          return this.PackCommandBack(command[0], (byte) 0, ((IEnumerable<byte>) this.lBuffer.GetBytes(index, num * 16)).Select<byte, bool>((Func<byte, bool>) (m => m > (byte) 0)).ToArray<bool>().ToByteArray());
        case 13:
          return this.PackCommandBack(command[0], (byte) 0, ((IEnumerable<byte>) this.mBuffer.GetBytes(index, num * 16)).Select<byte, bool>((Func<byte, bool>) (m => m > (byte) 0)).ToArray<bool>().ToByteArray());
        case 18:
          return this.PackCommandBack(command[0], (byte) 0, this.rBuffer.GetBytes(index * 2, num * 2));
        case 22:
          return this.PackCommandBack(command[0], (byte) 0, this.vBuffer.GetBytes(index * 2, num * 2));
        case 23:
          return this.PackCommandBack(command[0], (byte) 0, this.wBuffer.GetBytes(index * 2, num * 2));
        case 24:
          return this.PackCommandBack(command[0], (byte) 0, ((IEnumerable<byte>) this.xBuffer.GetBytes(index, num * 16)).Select<byte, bool>((Func<byte, bool>) (m => m > (byte) 0)).ToArray<bool>().ToByteArray());
        case 25:
          return this.PackCommandBack(command[0], (byte) 0, ((IEnumerable<byte>) this.yBuffer.GetBytes(index, num * 16)).Select<byte, bool>((Func<byte, bool>) (m => m > (byte) 0)).ToArray<bool>().ToByteArray());
        case 26:
          return this.PackCommandBack(command[0], (byte) 0, this.zBuffer.GetBytes(index * 2, num * 2));
        default:
          return this.PackCommandBack(command[0], (byte) 3, (byte[]) null);
      }
    }

    private byte[] WriteWordByCommand(byte[] command)
    {
      if (!this.EnableWrite)
        return this.PackCommandBack(command[0], (byte) 3, (byte[]) null);
      int destIndex = this.transform.TransInt32(command, 6);
      int num = (int) this.transform.TransUInt16(command, 10);
      if (destIndex > (int) ushort.MaxValue || destIndex < 0)
        return this.PackCommandBack(command[0], (byte) 4, (byte[]) null);
      if (num > 64)
        return this.PackCommandBack(command[0], (byte) 5, (byte[]) null);
      if (destIndex + num > (int) ushort.MaxValue)
        return this.PackCommandBack(command[0], (byte) 5, (byte[]) null);
      if (num * 2 != command.Length - 12)
        return this.PackCommandBack(command[0], (byte) 5, (byte[]) null);
      switch (command[5])
      {
        case 2:
          this.bBuffer.SetBytes(command.RemoveBegin<byte>(12), destIndex * 2);
          return this.PackCommandBack(command[0], (byte) 0, (byte[]) null);
        case 4:
          this.dBuffer.SetBytes(command.RemoveBegin<byte>(12), destIndex * 2);
          return this.PackCommandBack(command[0], (byte) 0, (byte[]) null);
        case 5:
          this.eBuffer.SetBytes(((IEnumerable<bool>) command.RemoveBegin<byte>(12).ToBoolArray()).Select<bool, byte>((Func<bool, byte>) (m => !m ? (byte) 0 : (byte) 1)).ToArray<byte>(), destIndex);
          return this.PackCommandBack(command[0], (byte) 0, (byte[]) null);
        case 6:
          this.fBuffer.SetBytes(command.RemoveBegin<byte>(12), destIndex * 2);
          return this.PackCommandBack(command[0], (byte) 0, (byte[]) null);
        case 9:
          this.iBuffer.SetBytes(((IEnumerable<bool>) command.RemoveBegin<byte>(12).ToBoolArray()).Select<bool, byte>((Func<bool, byte>) (m => !m ? (byte) 0 : (byte) 1)).ToArray<byte>(), destIndex);
          return this.PackCommandBack(command[0], (byte) 0, (byte[]) null);
        case 12:
          this.lBuffer.SetBytes(((IEnumerable<bool>) command.RemoveBegin<byte>(12).ToBoolArray()).Select<bool, byte>((Func<bool, byte>) (m => !m ? (byte) 0 : (byte) 1)).ToArray<byte>(), destIndex);
          return this.PackCommandBack(command[0], (byte) 0, (byte[]) null);
        case 13:
          this.mBuffer.SetBytes(((IEnumerable<bool>) command.RemoveBegin<byte>(12).ToBoolArray()).Select<bool, byte>((Func<bool, byte>) (m => !m ? (byte) 0 : (byte) 1)).ToArray<byte>(), destIndex);
          return this.PackCommandBack(command[0], (byte) 0, (byte[]) null);
        case 18:
          this.rBuffer.SetBytes(command.RemoveBegin<byte>(12), destIndex * 2);
          return this.PackCommandBack(command[0], (byte) 0, (byte[]) null);
        case 22:
          this.vBuffer.SetBytes(command.RemoveBegin<byte>(12), destIndex * 2);
          return this.PackCommandBack(command[0], (byte) 0, (byte[]) null);
        case 23:
          this.wBuffer.SetBytes(command.RemoveBegin<byte>(12), destIndex * 2);
          return this.PackCommandBack(command[0], (byte) 0, (byte[]) null);
        case 24:
          return this.PackCommandBack(command[0], (byte) 3, (byte[]) null);
        case 25:
          this.yBuffer.SetBytes(((IEnumerable<bool>) command.RemoveBegin<byte>(12).ToBoolArray()).Select<bool, byte>((Func<bool, byte>) (m => !m ? (byte) 0 : (byte) 1)).ToArray<byte>(), destIndex);
          return this.PackCommandBack(command[0], (byte) 0, (byte[]) null);
        case 26:
          this.zBuffer.SetBytes(command.RemoveBegin<byte>(12), destIndex * 2);
          return this.PackCommandBack(command[0], (byte) 0, (byte[]) null);
        default:
          return this.PackCommandBack(command[0], (byte) 3, (byte[]) null);
      }
    }

    private byte[] ReadRandomWordByCommand(byte[] command)
    {
      int num = (int) this.transform.TransUInt16(command, 4);
      if (num > 32)
        return this.PackCommandBack(command[0], (byte) 5, (byte[]) null);
      if (num * 6 != command.Length - 6)
        return this.PackCommandBack(command[0], (byte) 5, (byte[]) null);
      byte[] result = new byte[num * 2];
      for (int index1 = 0; index1 < num; ++index1)
      {
        int index2 = this.transform.TransInt32(command, 8 + 6 * index1);
        if (index2 > (int) ushort.MaxValue || index2 < 0)
          return this.PackCommandBack(command[0], (byte) 4, (byte[]) null);
        switch (command[7 + index1 * 6])
        {
          case 2:
            this.bBuffer.GetBytes(index2 * 2, 2).CopyTo((Array) result, index1 * 2);
            break;
          case 4:
            this.dBuffer.GetBytes(index2 * 2, 2).CopyTo((Array) result, index1 * 2);
            break;
          case 5:
            ((IEnumerable<byte>) this.eBuffer.GetBytes(index2, 16)).Select<byte, bool>((Func<byte, bool>) (m => m > (byte) 0)).ToArray<bool>().ToByteArray().CopyTo((Array) result, index1 * 2);
            break;
          case 6:
            this.fBuffer.GetBytes(index2 * 2, 2).CopyTo((Array) result, index1 * 2);
            break;
          case 9:
            ((IEnumerable<byte>) this.iBuffer.GetBytes(index2, 16)).Select<byte, bool>((Func<byte, bool>) (m => m > (byte) 0)).ToArray<bool>().ToByteArray().CopyTo((Array) result, index1 * 2);
            break;
          case 12:
            ((IEnumerable<byte>) this.lBuffer.GetBytes(index2, 16)).Select<byte, bool>((Func<byte, bool>) (m => m > (byte) 0)).ToArray<bool>().ToByteArray().CopyTo((Array) result, index1 * 2);
            break;
          case 13:
            ((IEnumerable<byte>) this.mBuffer.GetBytes(index2, 16)).Select<byte, bool>((Func<byte, bool>) (m => m > (byte) 0)).ToArray<bool>().ToByteArray().CopyTo((Array) result, index1 * 2);
            break;
          case 18:
            this.rBuffer.GetBytes(index2 * 2, 2).CopyTo((Array) result, index1 * 2);
            break;
          case 22:
            this.vBuffer.GetBytes(index2 * 2, 2).CopyTo((Array) result, index1 * 2);
            break;
          case 23:
            this.wBuffer.GetBytes(index2 * 2, 2).CopyTo((Array) result, index1 * 2);
            break;
          case 24:
            ((IEnumerable<byte>) this.xBuffer.GetBytes(index2, 16)).Select<byte, bool>((Func<byte, bool>) (m => m > (byte) 0)).ToArray<bool>().ToByteArray().CopyTo((Array) result, index1 * 2);
            break;
          case 25:
            ((IEnumerable<byte>) this.yBuffer.GetBytes(index2, 16)).Select<byte, bool>((Func<byte, bool>) (m => m > (byte) 0)).ToArray<bool>().ToByteArray().CopyTo((Array) result, index1 * 2);
            break;
          case 26:
            this.zBuffer.GetBytes(index2 * 2, 2).CopyTo((Array) result, index1 * 2);
            break;
          default:
            return this.PackCommandBack(command[0], (byte) 3, (byte[]) null);
        }
      }
      return this.PackCommandBack(command[0], (byte) 0, result);
    }

    private byte[] WriteRandomWordByCommand(byte[] command)
    {
      if (!this.EnableWrite)
        return this.PackCommandBack(command[0], (byte) 3, (byte[]) null);
      int num = (int) this.transform.TransUInt16(command, 4);
      if (num > 32)
        return this.PackCommandBack(command[0], (byte) 5, (byte[]) null);
      if (num * 8 != command.Length - 6)
        return this.PackCommandBack(command[0], (byte) 5, (byte[]) null);
      for (int index = 0; index < num; ++index)
      {
        int destIndex = this.transform.TransInt32(command, 8 + 8 * index);
        if (destIndex > (int) ushort.MaxValue || destIndex < 0)
          return this.PackCommandBack(command[0], (byte) 4, (byte[]) null);
        switch (command[7 + index * 8])
        {
          case 2:
            this.bBuffer.SetBytes(command.SelectMiddle<byte>(12 + 8 * index, 2), destIndex * 2);
            break;
          case 4:
            this.dBuffer.SetBytes(command.SelectMiddle<byte>(12 + 8 * index, 2), destIndex * 2);
            break;
          case 5:
            this.eBuffer.SetBytes(((IEnumerable<bool>) command.SelectMiddle<byte>(12 + 8 * index, 2).ToBoolArray()).Select<bool, byte>((Func<bool, byte>) (m => !m ? (byte) 0 : (byte) 1)).ToArray<byte>(), destIndex);
            break;
          case 6:
            this.fBuffer.SetBytes(command.SelectMiddle<byte>(12 + 8 * index, 2), destIndex * 2);
            break;
          case 9:
            this.iBuffer.SetBytes(((IEnumerable<bool>) command.SelectMiddle<byte>(12 + 8 * index, 2).ToBoolArray()).Select<bool, byte>((Func<bool, byte>) (m => !m ? (byte) 0 : (byte) 1)).ToArray<byte>(), destIndex);
            break;
          case 12:
            this.lBuffer.SetBytes(((IEnumerable<bool>) command.SelectMiddle<byte>(12 + 8 * index, 2).ToBoolArray()).Select<bool, byte>((Func<bool, byte>) (m => !m ? (byte) 0 : (byte) 1)).ToArray<byte>(), destIndex);
            break;
          case 13:
            this.mBuffer.SetBytes(((IEnumerable<bool>) command.SelectMiddle<byte>(12 + 8 * index, 2).ToBoolArray()).Select<bool, byte>((Func<bool, byte>) (m => !m ? (byte) 0 : (byte) 1)).ToArray<byte>(), destIndex);
            break;
          case 18:
            this.rBuffer.SetBytes(command.SelectMiddle<byte>(12 + 8 * index, 2), destIndex * 2);
            break;
          case 22:
            this.vBuffer.SetBytes(command.SelectMiddle<byte>(12 + 8 * index, 2), destIndex * 2);
            break;
          case 23:
            this.wBuffer.SetBytes(command.SelectMiddle<byte>(12 + 8 * index, 2), destIndex * 2);
            break;
          case 24:
            return this.PackCommandBack(command[0], (byte) 3, (byte[]) null);
          case 25:
            this.yBuffer.SetBytes(((IEnumerable<bool>) command.SelectMiddle<byte>(12 + 8 * index, 2).ToBoolArray()).Select<bool, byte>((Func<bool, byte>) (m => !m ? (byte) 0 : (byte) 1)).ToArray<byte>(), destIndex);
            break;
          case 26:
            this.zBuffer.SetBytes(command.SelectMiddle<byte>(12 + 8 * index, 2), destIndex * 2);
            break;
          default:
            return this.PackCommandBack(command[0], (byte) 3, (byte[]) null);
        }
      }
      return this.PackCommandBack(command[0], (byte) 0, (byte[]) null);
    }

    private byte[] StartByCommand(byte[] command)
    {
      this.isProgramStarted = true;
      return this.PackCommandBack(command[0], (byte) 0, (byte[]) null);
    }

    private byte[] StopByCommand(byte[] command)
    {
      this.isProgramStarted = false;
      return this.PackCommandBack(command[0], (byte) 0, (byte[]) null);
    }

    private byte[] ReadSystemByCommand(byte[] command)
    {
      if (command[5] == (byte) 1)
      {
        byte[] result = new byte[2]
        {
          (byte) 0,
          this.isProgramStarted ? (byte) 1 : (byte) 2
        };
        return this.PackCommandBack(command[0], (byte) 0, result);
      }
      if (command[5] != (byte) 2)
        return this.PackCommandBack(command[0], (byte) 3, (byte[]) null);
      byte[] result1 = new byte[28];
      Encoding.ASCII.GetBytes("F3SP38-6N").CopyTo((Array) result1, 0);
      Encoding.ASCII.GetBytes("12345").CopyTo((Array) result1, 16);
      result1[25] = (byte) 17;
      result1[26] = (byte) 2;
      result1[27] = (byte) 3;
      return this.PackCommandBack(command[0], (byte) 0, result1);
    }

    private byte[] ReadSystemDateTime(byte[] command)
    {
      byte[] result = new byte[16];
      DateTime now = DateTime.Now;
      result[0] = BitConverter.GetBytes(now.Year - 2000)[1];
      result[1] = BitConverter.GetBytes(now.Year - 2000)[0];
      result[2] = BitConverter.GetBytes(now.Month)[1];
      result[3] = BitConverter.GetBytes(now.Month)[0];
      result[4] = BitConverter.GetBytes(now.Day)[1];
      result[5] = BitConverter.GetBytes(now.Day)[0];
      result[6] = BitConverter.GetBytes(now.Hour)[1];
      result[7] = BitConverter.GetBytes(now.Hour)[0];
      result[8] = BitConverter.GetBytes(now.Minute)[1];
      result[9] = BitConverter.GetBytes(now.Minute)[0];
      result[10] = BitConverter.GetBytes(now.Second)[1];
      result[11] = BitConverter.GetBytes(now.Second)[0];
      uint totalSeconds = (uint) (now - new DateTime(now.Year, 1, 1)).TotalSeconds;
      result[12] = BitConverter.GetBytes(totalSeconds)[3];
      result[13] = BitConverter.GetBytes(totalSeconds)[2];
      result[14] = BitConverter.GetBytes(totalSeconds)[1];
      result[15] = BitConverter.GetBytes(totalSeconds)[0];
      return this.PackCommandBack(command[0], (byte) 0, result);
    }

    private byte[] ReadSpecialModule(byte[] command)
    {
      if (command[4] != (byte) 0 || command[5] != (byte) 1)
        return this.PackCommandBack(command[0], (byte) 3, (byte[]) null);
      ushort num1 = this.transform.TransUInt16(command, 6);
      ushort num2 = this.transform.TransUInt16(command, 8);
      return this.PackCommandBack(command[0], (byte) 0, this.specialBuffer.GetBytes((int) num1 * 2, (int) num2 * 2));
    }

    private byte[] WriteSpecialModule(byte[] command)
    {
      if (!this.EnableWrite)
        return this.PackCommandBack(command[0], (byte) 3, (byte[]) null);
      if (command[4] != (byte) 0 || command[5] != (byte) 1)
        return this.PackCommandBack(command[0], (byte) 3, (byte[]) null);
      ushort num = this.transform.TransUInt16(command, 6);
      if ((int) this.transform.TransUInt16(command, 8) * 2 != command.Length - 10)
        return this.PackCommandBack(command[0], (byte) 5, (byte[]) null);
      this.specialBuffer.SetBytes(command.RemoveBegin<byte>(10), (int) num * 2);
      return this.PackCommandBack(command[0], (byte) 0, (byte[]) null);
    }

    private byte[] PackCommandBack(byte cmd, byte err, byte[] result)
    {
      if (result == null)
        result = new byte[0];
      byte[] numArray = new byte[4 + result.Length];
      numArray[0] = (byte) ((uint) cmd + 128U);
      numArray[1] = err;
      numArray[2] = BitConverter.GetBytes(result.Length)[1];
      numArray[3] = BitConverter.GetBytes(result.Length)[0];
      result.CopyTo((Array) numArray, 4);
      return numArray;
    }

    /// <inheritdoc />
    protected override void LoadFromBytes(byte[] content)
    {
      if (content.Length < 1310720)
        throw new Exception("File is not correct");
      this.xBuffer.SetBytes(content, 0, 0, 65536);
      this.yBuffer.SetBytes(content, 65536, 0, 65536);
      this.iBuffer.SetBytes(content, 131072, 0, 65536);
      this.eBuffer.SetBytes(content, 196608, 0, 65536);
      this.mBuffer.SetBytes(content, 262144, 0, 65536);
      this.lBuffer.SetBytes(content, 327680, 0, 65536);
      this.dBuffer.SetBytes(content, 393216, 0, 65536);
      this.bBuffer.SetBytes(content, 524288, 0, 65536);
      this.fBuffer.SetBytes(content, 655360, 0, 65536);
      this.rBuffer.SetBytes(content, 786432, 0, 65536);
      this.vBuffer.SetBytes(content, 917504, 0, 65536);
      this.zBuffer.SetBytes(content, 1048576, 0, 65536);
      this.wBuffer.SetBytes(content, 1179648, 0, 65536);
    }

    /// <inheritdoc />
    protected override byte[] SaveToBytes()
    {
      byte[] numArray = new byte[1310720];
      Array.Copy((Array) this.xBuffer.GetBytes(), 0, (Array) numArray, 0, 65536);
      Array.Copy((Array) this.yBuffer.GetBytes(), 0, (Array) numArray, 65536, 65536);
      Array.Copy((Array) this.iBuffer.GetBytes(), 0, (Array) numArray, 131072, 65536);
      Array.Copy((Array) this.eBuffer.GetBytes(), 0, (Array) numArray, 196608, 65536);
      Array.Copy((Array) this.mBuffer.GetBytes(), 0, (Array) numArray, 262144, 65536);
      Array.Copy((Array) this.lBuffer.GetBytes(), 0, (Array) numArray, 327680, 65536);
      Array.Copy((Array) this.dBuffer.GetBytes(), 0, (Array) numArray, 393216, 65536);
      Array.Copy((Array) this.bBuffer.GetBytes(), 0, (Array) numArray, 524288, 65536);
      Array.Copy((Array) this.fBuffer.GetBytes(), 0, (Array) numArray, 655360, 65536);
      Array.Copy((Array) this.rBuffer.GetBytes(), 0, (Array) numArray, 786432, 65536);
      Array.Copy((Array) this.vBuffer.GetBytes(), 0, (Array) numArray, 917504, 65536);
      Array.Copy((Array) this.zBuffer.GetBytes(), 0, (Array) numArray, 1048576, 65536);
      Array.Copy((Array) this.wBuffer.GetBytes(), 0, (Array) numArray, 1179648, 65536);
      return numArray;
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.xBuffer?.Dispose();
        this.yBuffer?.Dispose();
        this.iBuffer?.Dispose();
        this.eBuffer?.Dispose();
        this.mBuffer?.Dispose();
        this.lBuffer?.Dispose();
        this.dBuffer?.Dispose();
        this.bBuffer?.Dispose();
        this.fBuffer?.Dispose();
        this.rBuffer?.Dispose();
        this.vBuffer?.Dispose();
        this.zBuffer?.Dispose();
        this.wBuffer?.Dispose();
        this.specialBuffer?.Dispose();
      }
      base.Dispose(disposing);
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("YokogawaLinkServer[{0}]", (object) this.Port);
  }
}
