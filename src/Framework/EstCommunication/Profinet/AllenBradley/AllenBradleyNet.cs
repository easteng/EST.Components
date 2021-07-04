// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.AllenBradley.AllenBradleyNet
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using EstCommunication.Core;
using EstCommunication.Core.IMessage;
using EstCommunication.Core.Net;
using EstCommunication.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EstCommunication.Profinet.AllenBradley
{
  /// <summary>
  /// AB PLC的数据通信类，使用CIP协议实现，适用1756，1769等型号，支持使用标签的形式进行读写操作，支持标量数据，一维数组，二维数组，三维数组等等。如果是局部变量，那么使用 Program:MainProgram.[变量名]。<br />
  /// The data communication class of AB PLC is implemented using the CIP protocol. It is suitable for 1756, 1769 and other models.
  /// It supports reading and writing in the form of tags, scalar data, one-dimensional array, two-dimensional array,
  /// three-dimensional array, and so on. If it is a local variable, use the Program:MainProgram.[Variable name].
  /// </summary>
  /// <remarks>
  /// thanks 江阴-  ∮溪风-⊙_⌒ help test the dll
  /// <br />
  /// thanks 上海-null 测试了这个dll
  /// <br />
  /// <br />
  /// 默认的地址就是PLC里的TAG名字，比如A，B，C；如果你需要读取的数据是一个数组，那么A就是默认的A[0]，如果想要读取偏移量为10的数据，那么地址为A[10]，
  /// 多维数组同理，使用A[10,10,10]的操作。
  /// <br />
  /// <br />
  /// 假设你读取的是局部变量，那么使用 Program:MainProgram.变量名<br />
  /// 目前适用的系列为1756 ControlLogix, 1756 GuardLogix, 1769 CompactLogix, 1769 Compact GuardLogix, 1789SoftLogix, 5069 CompactLogix, 5069 Compact GuardLogix, Studio 5000 Logix Emulate
  /// <br />
  /// <br />
  /// 如果你有个Bool数组要读取，变量名为 A, 那么读第0个位，可以通过 ReadBool("A")，但是第二个位需要使用<br />
  /// ReadBoolArray("A[0]")   // 返回32个bool长度，0-31的索引，如果我想读取32-63的位索引，就需要 ReadBoolArray("A[1]") ，以此类推。
  /// <br />
  /// <br />
  /// 地址可以携带站号信息，只要在前面加上slot=2;即可，这就是访问站号2的数据了，例如 slot=2;AAA
  /// </remarks>
  public class AllenBradleyNet : NetworkDeviceBase
  {
    /// <summary>
    /// Instantiate a communication object for a Allenbradley PLC protocol
    /// </summary>
    public AllenBradleyNet()
    {
      this.WordLength = (ushort) 2;
      this.ByteTransform = (IByteTransform) new RegularByteTransform();
    }

    /// <summary>
    /// Instantiate a communication object for a Allenbradley PLC protocol
    /// </summary>
    /// <param name="ipAddress">PLC IpAddress</param>
    /// <param name="port">PLC Port</param>
    public AllenBradleyNet(string ipAddress, int port = 44818)
    {
      this.WordLength = (ushort) 2;
      this.IpAddress = ipAddress;
      this.Port = port;
      this.ByteTransform = (IByteTransform) new RegularByteTransform();
    }

    /// <inheritdoc />
    protected override INetMessage GetNewNetMessage() => (INetMessage) new AllenBradleyMessage();

    /// <inheritdoc />
    protected override byte[] PackCommandWithHeader(byte[] command) => AllenBradleyHelper.PackRequestHeader(this.CipCommand, this.SessionHandle, command);

    /// <summary>
    /// The current session handle, which is determined by the PLC when communicating with the PLC handshake
    /// </summary>
    public uint SessionHandle { get; protected set; }

    /// <summary>
    /// Gets or sets the slot number information for the current plc, which should be set before connections
    /// </summary>
    public byte Slot { get; set; } = 0;

    /// <summary>port and slot information</summary>
    public byte[] PortSlot { get; set; }

    /// <summary>
    /// 获取或设置整个交互指令的控制码，默认为0x6F，通常不需要修改<br />
    /// Gets or sets the control code of the entire interactive instruction. The default is 0x6F, and usually does not need to be modified.
    /// </summary>
    public ushort CipCommand { get; set; } = 111;

    /// <inheritdoc />
    protected override OperateResult InitializationOnConnect(Socket socket)
    {
      OperateResult<byte[]> operateResult1 = this.ReadFromCoreServer(socket, AllenBradleyHelper.RegisterSessionHandle(), usePackHeader: false);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult operateResult2 = AllenBradleyHelper.CheckResponse(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return operateResult2;
      this.SessionHandle = this.ByteTransform.TransUInt32(operateResult1.Content, 4);
      return OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc />
    protected override OperateResult ExtraOnDisconnect(Socket socket)
    {
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(socket, AllenBradleyHelper.UnRegisterSessionHandle(this.SessionHandle), usePackHeader: false);
      return !operateResult.IsSuccess ? (OperateResult) operateResult : OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc />
    protected override async Task<OperateResult> InitializationOnConnectAsync(
      Socket socket)
    {
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(socket, AllenBradleyHelper.RegisterSessionHandle(), usePackHeader: false);
      if (!read.IsSuccess)
        return (OperateResult) read;
      OperateResult check = AllenBradleyHelper.CheckResponse(read.Content);
      if (!check.IsSuccess)
        return check;
      this.SessionHandle = this.ByteTransform.TransUInt32(read.Content, 4);
      return OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc />
    protected override async Task<OperateResult> ExtraOnDisconnectAsync(
      Socket socket)
    {
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(socket, AllenBradleyHelper.UnRegisterSessionHandle(this.SessionHandle), usePackHeader: false);
      OperateResult operateResult = read.IsSuccess ? OperateResult.CreateSuccessResult() : (OperateResult) read;
      read = (OperateResult<byte[]>) null;
      return operateResult;
    }

    /// <summary>
    /// 创建一个读取标签的报文指定，标签地址可以手动动态指定slot编号，例如 slot=2;AAA<br />
    /// Build a read command bytes, The label address can manually specify the slot number dynamically, for example slot=2;AAA
    /// </summary>
    /// <param name="address">the address of the tag name</param>
    /// <param name="length">Array information, if not arrays, is 1 </param>
    /// <returns>Message information that contains the result object </returns>
    public virtual OperateResult<byte[]> BuildReadCommand(
      string[] address,
      int[] length)
    {
      if (address == null || length == null)
        return new OperateResult<byte[]>("address or length is null");
      if (address.Length != length.Length)
        return new OperateResult<byte[]>("address and length is not same array");
      try
      {
        byte num = this.Slot;
        List<byte[]> numArrayList = new List<byte[]>();
        for (int index = 0; index < address.Length; ++index)
        {
          num = (byte) EstHelper.ExtractParameter(ref address[index], "slot", (int) this.Slot);
          numArrayList.Add(AllenBradleyHelper.PackRequsetRead(address[index], length[index]));
        }
        byte[][] numArray = new byte[2][]
        {
          new byte[4],
          null
        };
        byte[] portSlot = this.PortSlot;
        if (portSlot == null)
          portSlot = new byte[2]{ (byte) 1, num };
        numArray[1] = this.PackCommandService(portSlot, numArrayList.ToArray());
        return OperateResult.CreateSuccessResult<byte[]>(AllenBradleyHelper.PackCommandSpecificData(numArray));
      }
      catch (Exception ex)
      {
        return new OperateResult<byte[]>("Address Wrong:" + ex.Message);
      }
    }

    /// <summary>
    /// 创建一个读取多标签的报文<br />
    /// Build a read command bytes
    /// </summary>
    /// <param name="address">The address of the tag name </param>
    /// <returns>Message information that contains the result object </returns>
    public OperateResult<byte[]> BuildReadCommand(string[] address)
    {
      if (address == null)
        return new OperateResult<byte[]>("address or length is null");
      int[] length = new int[address.Length];
      for (int index = 0; index < address.Length; ++index)
        length[index] = 1;
      return this.BuildReadCommand(address, length);
    }

    /// <summary>Create a written message instruction</summary>
    /// <param name="address">The address of the tag name </param>
    /// <param name="typeCode">Data type</param>
    /// <param name="data">Source Data </param>
    /// <param name="length">In the case of arrays, the length of the array </param>
    /// <returns>Message information that contains the result object</returns>
    public OperateResult<byte[]> BuildWriteCommand(
      string address,
      ushort typeCode,
      byte[] data,
      int length = 1)
    {
      try
      {
        byte parameter = (byte) EstHelper.ExtractParameter(ref address, "slot", (int) this.Slot);
        byte[] numArray1 = AllenBradleyHelper.PackRequestWrite(address, typeCode, data, length);
        byte[][] numArray2 = new byte[2][]
        {
          new byte[4],
          null
        };
        byte[] portSlot = this.PortSlot;
        if (portSlot == null)
          portSlot = new byte[2]{ (byte) 1, parameter };
        numArray2[1] = this.PackCommandService(portSlot, numArray1);
        return OperateResult.CreateSuccessResult<byte[]>(AllenBradleyHelper.PackCommandSpecificData(numArray2));
      }
      catch (Exception ex)
      {
        return new OperateResult<byte[]>("Address Wrong:" + ex.Message);
      }
    }

    /// <summary>Create a written message instruction</summary>
    /// <param name="address">The address of the tag name </param>
    /// <param name="data">Bool Data </param>
    /// <returns>Message information that contains the result object</returns>
    public OperateResult<byte[]> BuildWriteCommand(string address, bool data)
    {
      try
      {
        byte parameter = (byte) EstHelper.ExtractParameter(ref address, "slot", (int) this.Slot);
        byte[] numArray1 = AllenBradleyHelper.PackRequestWrite(address, data);
        byte[][] numArray2 = new byte[2][]
        {
          new byte[4],
          null
        };
        byte[] portSlot = this.PortSlot;
        if (portSlot == null)
          portSlot = new byte[2]{ (byte) 1, parameter };
        numArray2[1] = this.PackCommandService(portSlot, numArray1);
        return OperateResult.CreateSuccessResult<byte[]>(AllenBradleyHelper.PackCommandSpecificData(numArray2));
      }
      catch (Exception ex)
      {
        return new OperateResult<byte[]>("Address Wrong:" + ex.Message);
      }
    }

    /// <summary>
    /// Read data information, data length for read array length information
    /// </summary>
    /// <param name="address">Address format of the node</param>
    /// <param name="length">In the case of arrays, the length of the array </param>
    /// <returns>Result data with result object </returns>
    [EstMqttApi("ReadByteArray", "")]
    public override OperateResult<byte[]> Read(string address, ushort length)
    {
      if (length > (ushort) 1)
        return this.ReadSegment(address, 0, (int) length);
      return this.Read(new string[1]{ address }, new int[1]
      {
        (int) length
      });
    }

    /// <summary>Bulk read Data information</summary>
    /// <param name="address">Name of the node </param>
    /// <returns>Result data with result object </returns>
    [EstMqttApi("ReadAddress", "")]
    public OperateResult<byte[]> Read(string[] address)
    {
      if (address == null)
        return new OperateResult<byte[]>("address can not be null");
      int[] length = new int[address.Length];
      for (int index = 0; index < length.Length; ++index)
        length[index] = 1;
      return this.Read(address, length);
    }

    /// <summary>
    /// <b>[商业授权]</b> 批量读取多地址的数据信息，例如我可以读取两个标签的数据 "A","B[0]"， 长度为 [1, 5]，返回的是一整个的字节数组，需要自行解析<br />
    /// <b>[Authorization]</b> Read the data information of multiple addresses in batches. For example, I can read the data "A", "B[0]" of two tags,
    /// the length is [1, 5], and the return is an entire byte array, and I need to do it myself Parsing
    /// </summary>
    /// <param name="address">节点的名称 -&gt; Name of the node </param>
    /// <param name="length">如果是数组，就为数组长度 -&gt; In the case of arrays, the length of the array </param>
    /// <returns>带有结果对象的结果数据 -&gt; Result data with result object </returns>
    public OperateResult<byte[]> Read(string[] address, int[] length)
    {
      if (address != null && address.Length > 1 && !Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
      OperateResult<byte[], ushort, bool> operateResult = this.ReadWithType(address, length);
      return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult) : OperateResult.CreateSuccessResult<byte[]>(operateResult.Content1);
    }

    private OperateResult<byte[], ushort, bool> ReadWithType(
      string[] address,
      int[] length)
    {
      OperateResult<byte[]> operateResult1 = this.BuildReadCommand(address, length);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<byte[], ushort, bool>((OperateResult) operateResult1);
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return OperateResult.CreateFailedResult<byte[], ushort, bool>((OperateResult) operateResult2);
      OperateResult result = AllenBradleyHelper.CheckResponse(operateResult2.Content);
      return !result.IsSuccess ? OperateResult.CreateFailedResult<byte[], ushort, bool>(result) : AllenBradleyHelper.ExtractActualData(operateResult2.Content, true);
    }

    /// <summary>
    /// Read Segment Data Array form plc, use address tag name
    /// </summary>
    /// <param name="address">Tag name in plc</param>
    /// <param name="startIndex">array start index, uint byte index</param>
    /// <param name="length">array length, data item length</param>
    /// <returns>Results Bytes</returns>
    [EstMqttApi("ReadSegment", "")]
    public OperateResult<byte[]> ReadSegment(
      string address,
      int startIndex,
      int length)
    {
      try
      {
        List<byte> byteList = new List<byte>();
        OperateResult<byte[]> operateResult;
        OperateResult<byte[], ushort, bool> actualData;
        do
        {
          operateResult = this.ReadCipFromServer(AllenBradleyHelper.PackRequestReadSegment(address, startIndex, length));
          if (operateResult.IsSuccess)
          {
            actualData = AllenBradleyHelper.ExtractActualData(operateResult.Content, true);
            if (actualData.IsSuccess)
            {
              startIndex += actualData.Content1.Length;
              byteList.AddRange((IEnumerable<byte>) actualData.Content1);
            }
            else
              goto label_4;
          }
          else
            goto label_2;
        }
        while (actualData.Content3);
        goto label_7;
label_2:
        return operateResult;
label_4:
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) actualData);
label_7:
        return OperateResult.CreateSuccessResult<byte[]>(byteList.ToArray());
      }
      catch (Exception ex)
      {
        return new OperateResult<byte[]>("Address Wrong:" + ex.Message);
      }
    }

    private OperateResult<byte[]> ReadByCips(params byte[][] cips)
    {
      OperateResult<byte[]> operateResult = this.ReadCipFromServer(cips);
      if (!operateResult.IsSuccess)
        return operateResult;
      OperateResult<byte[], ushort, bool> actualData = AllenBradleyHelper.ExtractActualData(operateResult.Content, true);
      return !actualData.IsSuccess ? OperateResult.CreateFailedResult<byte[]>((OperateResult) actualData) : OperateResult.CreateSuccessResult<byte[]>(actualData.Content1);
    }

    /// <summary>使用CIP报文和服务器进行核心的数据交换</summary>
    /// <param name="cips">Cip commands</param>
    /// <returns>Results Bytes</returns>
    public OperateResult<byte[]> ReadCipFromServer(params byte[][] cips)
    {
      byte[][] numArray = new byte[2][]{ new byte[4], null };
      byte[] portSlot = this.PortSlot;
      if (portSlot == null)
        portSlot = new byte[2]{ (byte) 1, this.Slot };
      numArray[1] = this.PackCommandService(portSlot, ((IEnumerable<byte[]>) cips).ToArray<byte[]>());
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(AllenBradleyHelper.PackCommandSpecificData(numArray));
      if (!operateResult.IsSuccess)
        return operateResult;
      OperateResult result = AllenBradleyHelper.CheckResponse(operateResult.Content);
      return !result.IsSuccess ? OperateResult.CreateFailedResult<byte[]>(result) : OperateResult.CreateSuccessResult<byte[]>(operateResult.Content);
    }

    /// <summary>使用EIP报文和服务器进行核心的数据交换</summary>
    /// <param name="eip">eip commands</param>
    /// <returns>Results Bytes</returns>
    public OperateResult<byte[]> ReadEipFromServer(params byte[][] eip)
    {
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(AllenBradleyHelper.PackCommandSpecificData(eip));
      if (!operateResult.IsSuccess)
        return operateResult;
      OperateResult result = AllenBradleyHelper.CheckResponse(operateResult.Content);
      return !result.IsSuccess ? OperateResult.CreateFailedResult<byte[]>(result) : OperateResult.CreateSuccessResult<byte[]>(operateResult.Content);
    }

    /// <summary>
    /// 读取单个的bool数据信息，如果读取的是单bool变量，就直接写变量名，如果是由int组成的bool数组的一个值，一律带"i="开头访问，例如"i=A[0]" <br />
    /// Read a single bool data information, if it is a single bool variable, write the variable name directly,
    /// if it is a value of a bool array composed of int, it is always accessed with "i=" at the beginning, for example, "i=A[0]"
    /// </summary>
    /// <param name="address">节点的名称 -&gt; Name of the node </param>
    /// <returns>带有结果对象的结果数据 -&gt; Result data with result info </returns>
    [EstMqttApi("ReadBool", "")]
    public override OperateResult<bool> ReadBool(string address)
    {
      if (address.StartsWith("i="))
      {
        address = address.Substring(2);
        int arrayIndex;
        address = AllenBradleyHelper.AnalysisArrayIndex(address, out arrayIndex);
        OperateResult<bool[]> operateResult = this.ReadBoolArray(address + string.Format("[{0}]", (object) (arrayIndex / 32)));
        return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<bool>((OperateResult) operateResult) : OperateResult.CreateSuccessResult<bool>(operateResult.Content[arrayIndex % 32]);
      }
      OperateResult<byte[]> operateResult1 = this.Read(address, (ushort) 1);
      return !operateResult1.IsSuccess ? OperateResult.CreateFailedResult<bool>((OperateResult) operateResult1) : OperateResult.CreateSuccessResult<bool>(this.ByteTransform.TransBool(operateResult1.Content, 0));
    }

    /// <summary>
    /// 批量读取的bool数组信息，如果你有个Bool数组变量名为 A, 那么读第0个位，可以通过 ReadBool("A")，但是第二个位需要使用
    /// ReadBoolArray("A[0]")   // 返回32个bool长度，0-31的索引，如果我想读取32-63的位索引，就需要 ReadBoolArray("A[1]") ，以此类推。<br />
    /// For batch read bool array information, if you have a Bool array variable named A, then you can read the 0th bit through ReadBool("A"),
    /// but the second bit needs to use ReadBoolArray("A[0]" ) // Returns the length of 32 bools, the index is 0-31,
    /// if I want to read the bit index of 32-63, I need ReadBoolArray("A[1]"), and so on.
    /// </summary>
    /// <param name="address">节点的名称 -&gt; Name of the node </param>
    /// <returns>带有结果对象的结果数据 -&gt; Result data with result info </returns>
    [EstMqttApi("ReadBoolArrayAddress", "")]
    public OperateResult<bool[]> ReadBoolArray(string address)
    {
      OperateResult<byte[]> operateResult = this.Read(address, (ushort) 1);
      return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult) : OperateResult.CreateSuccessResult<bool[]>(this.ByteTransform.TransBool(operateResult.Content, 0, operateResult.Content.Length));
    }

    /// <summary>
    /// 读取PLC的byte类型的数据<br />
    /// Read the byte type of PLC data
    /// </summary>
    /// <param name="address">节点的名称 -&gt; Name of the node </param>
    /// <returns>带有结果对象的结果数据 -&gt; Result data with result info </returns>
    [EstMqttApi("ReadByte", "")]
    public OperateResult<byte> ReadByte(string address) => ByteTransformHelper.GetResultFromArray<byte>(this.Read(address, (ushort) 1));

    /// <inheritdoc cref="M:EstCommunication.Profinet.AllenBradley.AllenBradleyNet.Read(System.String,System.UInt16)" />
    public override async Task<OperateResult<byte[]>> ReadAsync(
      string address,
      ushort length)
    {
      if (length > (ushort) 1)
      {
        OperateResult<byte[]> operateResult = await this.ReadSegmentAsync(address, 0, (int) length);
        return operateResult;
      }
      OperateResult<byte[]> operateResult1 = await this.ReadAsync(new string[1]
      {
        address
      }, new int[1]{ (int) length });
      return operateResult1;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.AllenBradley.AllenBradleyNet.Read(System.String[])" />
    public async Task<OperateResult<byte[]>> ReadAsync(string[] address)
    {
      if (address == null)
        return new OperateResult<byte[]>("address can not be null");
      int[] length = new int[address.Length];
      for (int i = 0; i < length.Length; ++i)
        length[i] = 1;
      OperateResult<byte[]> operateResult = await this.ReadAsync(address, length);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.AllenBradley.AllenBradleyNet.Read(System.String[],System.Int32[])" />
    public async Task<OperateResult<byte[]>> ReadAsync(
      string[] address,
      int[] length)
    {
      string[] strArray = address;
      if (strArray != null && strArray.Length > 1 && !Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
      OperateResult<byte[], ushort, bool> read = await this.ReadWithTypeAsync(address, length);
      return read.IsSuccess ? OperateResult.CreateSuccessResult<byte[]>(read.Content1) : OperateResult.CreateFailedResult<byte[]>((OperateResult) read);
    }

    private async Task<OperateResult<byte[], ushort, bool>> ReadWithTypeAsync(
      string[] address,
      int[] length)
    {
      OperateResult<byte[]> command = this.BuildReadCommand(address, length);
      if (!command.IsSuccess)
        return OperateResult.CreateFailedResult<byte[], ushort, bool>((OperateResult) command);
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<byte[], ushort, bool>((OperateResult) read);
      OperateResult check = AllenBradleyHelper.CheckResponse(read.Content);
      return check.IsSuccess ? AllenBradleyHelper.ExtractActualData(read.Content, true) : OperateResult.CreateFailedResult<byte[], ushort, bool>(check);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.AllenBradley.AllenBradleyNet.ReadSegment(System.String,System.Int32,System.Int32)" />
    public async Task<OperateResult<byte[]>> ReadSegmentAsync(
      string address,
      int startIndex,
      int length)
    {
      try
      {
        List<byte> bytesContent = new List<byte>();
        OperateResult<byte[]> read;
        OperateResult<byte[], ushort, bool> analysis;
        while (true)
        {
          read = await this.ReadCipFromServerAsync(AllenBradleyHelper.PackRequestReadSegment(address, startIndex, length));
          if (read.IsSuccess)
          {
            analysis = AllenBradleyHelper.ExtractActualData(read.Content, true);
            if (analysis.IsSuccess)
            {
              startIndex += analysis.Content1.Length;
              bytesContent.AddRange((IEnumerable<byte>) analysis.Content1);
              if (analysis.Content3)
              {
                read = (OperateResult<byte[]>) null;
                analysis = (OperateResult<byte[], ushort, bool>) null;
              }
              else
                goto label_9;
            }
            else
              goto label_5;
          }
          else
            break;
        }
        return read;
label_5:
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) analysis);
label_9:
        return OperateResult.CreateSuccessResult<byte[]>(bytesContent.ToArray());
      }
      catch (Exception ex)
      {
        return new OperateResult<byte[]>("Address Wrong:" + ex.Message);
      }
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.AllenBradley.AllenBradleyNet.ReadCipFromServer(System.Byte[][])" />
    public async Task<OperateResult<byte[]>> ReadCipFromServerAsync(
      params byte[][] cips)
    {
      byte[][] numArray = new byte[2][]{ new byte[4], null };
      byte[] portSlot = this.PortSlot;
      if (portSlot == null)
        portSlot = new byte[2]{ (byte) 1, this.Slot };
      numArray[1] = this.PackCommandService(portSlot, ((IEnumerable<byte[]>) cips).ToArray<byte[]>());
      byte[] commandSpecificData = AllenBradleyHelper.PackCommandSpecificData(numArray);
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(commandSpecificData);
      if (!read.IsSuccess)
        return read;
      OperateResult check = AllenBradleyHelper.CheckResponse(read.Content);
      return check.IsSuccess ? OperateResult.CreateSuccessResult<byte[]>(read.Content) : OperateResult.CreateFailedResult<byte[]>(check);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.AllenBradley.AllenBradleyNet.ReadEipFromServer(System.Byte[][])" />
    public async Task<OperateResult<byte[]>> ReadEipFromServerAsync(
      params byte[][] eip)
    {
      byte[] commandSpecificData = AllenBradleyHelper.PackCommandSpecificData(eip);
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(commandSpecificData);
      if (!read.IsSuccess)
        return read;
      OperateResult check = AllenBradleyHelper.CheckResponse(read.Content);
      return check.IsSuccess ? OperateResult.CreateSuccessResult<byte[]>(read.Content) : OperateResult.CreateFailedResult<byte[]>(check);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.AllenBradley.AllenBradleyNet.ReadBool(System.String)" />
    public override async Task<OperateResult<bool>> ReadBoolAsync(string address)
    {
      if (address.StartsWith("i="))
      {
        address = address.Substring(2);
        int bitIndex;
        address = AllenBradleyHelper.AnalysisArrayIndex(address, out bitIndex);
        OperateResult<bool[]> read = await this.ReadBoolArrayAsync(address + string.Format("[{0}]", (object) (bitIndex / 32)));
        return read.IsSuccess ? OperateResult.CreateSuccessResult<bool>(read.Content[bitIndex % 32]) : OperateResult.CreateFailedResult<bool>((OperateResult) read);
      }
      OperateResult<byte[]> read1 = await this.ReadAsync(address, (ushort) 1);
      return read1.IsSuccess ? OperateResult.CreateSuccessResult<bool>(this.ByteTransform.TransBool(read1.Content, 0)) : OperateResult.CreateFailedResult<bool>((OperateResult) read1);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.AllenBradley.AllenBradleyNet.ReadBoolArray(System.String)" />
    public async Task<OperateResult<bool[]>> ReadBoolArrayAsync(string address)
    {
      OperateResult<byte[]> read = await this.ReadAsync(address, (ushort) 1);
      OperateResult<bool[]> operateResult = read.IsSuccess ? OperateResult.CreateSuccessResult<bool[]>(this.ByteTransform.TransBool(read.Content, 0, read.Content.Length)) : OperateResult.CreateFailedResult<bool[]>((OperateResult) read);
      read = (OperateResult<byte[]>) null;
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.AllenBradley.AllenBradleyNet.ReadByte(System.String)" />
    public async Task<OperateResult<byte>> ReadByteAsync(string address)
    {
      OperateResult<byte[]> result = await this.ReadAsync(address, (ushort) 1);
      return ByteTransformHelper.GetResultFromArray<byte>(result);
    }

    /// <summary>
    /// 枚举当前的所有的变量名字，包含结构体信息，除去系统自带的名称数据信息<br />
    /// Enumerate all the current variable names, including structure information, except the name data information that comes with the system
    /// </summary>
    /// <returns>结果对象</returns>
    public OperateResult<AbTagItem[]> TagEnumerator()
    {
      List<AbTagItem> abTagItemList = new List<AbTagItem>();
      ushort startInstance = 0;
      OperateResult<byte[]> operateResult;
      OperateResult<byte[], ushort, bool> actualData;
      do
      {
        operateResult = this.ReadCipFromServer(AllenBradleyHelper.GetEnumeratorCommand(startInstance));
        if (operateResult.IsSuccess)
        {
          actualData = AllenBradleyHelper.ExtractActualData(operateResult.Content, true);
          if (actualData.IsSuccess)
          {
            if (operateResult.Content.Length >= 43 && BitConverter.ToUInt16(operateResult.Content, 40) == (ushort) 213)
            {
              int startIndex1 = 44;
              while (startIndex1 < operateResult.Content.Length)
              {
                AbTagItem abTagItem = new AbTagItem();
                abTagItem.InstanceID = BitConverter.ToUInt32(operateResult.Content, startIndex1);
                startInstance = (ushort) (abTagItem.InstanceID + 1U);
                int startIndex2 = startIndex1 + 4;
                ushort uint16 = BitConverter.ToUInt16(operateResult.Content, startIndex2);
                int index = startIndex2 + 2;
                abTagItem.Name = Encoding.ASCII.GetString(operateResult.Content, index, (int) uint16);
                int startIndex3 = index + (int) uint16;
                abTagItem.SymbolType = BitConverter.ToUInt16(operateResult.Content, startIndex3);
                startIndex1 = startIndex3 + 2;
                if (((int) abTagItem.SymbolType & 4096) != 4096 && !abTagItem.Name.StartsWith("__"))
                  abTagItemList.Add(abTagItem);
              }
            }
            else
              goto label_12;
          }
          else
            goto label_3;
        }
        else
          goto label_1;
      }
      while (actualData.Content3);
      goto label_11;
label_1:
      return OperateResult.CreateFailedResult<AbTagItem[]>((OperateResult) operateResult);
label_3:
      return OperateResult.CreateFailedResult<AbTagItem[]>((OperateResult) actualData);
label_11:
      return OperateResult.CreateSuccessResult<AbTagItem[]>(abTagItemList.ToArray());
label_12:
      return new OperateResult<AbTagItem[]>(StringResources.Language.UnknownError);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.AllenBradley.AllenBradleyNet.TagEnumerator" />
    public async Task<OperateResult<AbTagItem[]>> TagEnumeratorAsync()
    {
      List<AbTagItem> lists = new List<AbTagItem>();
      ushort instansAddress = 0;
      OperateResult<byte[]> readCip;
      OperateResult<byte[], ushort, bool> analysis;
      while (true)
      {
        readCip = await this.ReadCipFromServerAsync(AllenBradleyHelper.GetEnumeratorCommand(instansAddress));
        if (readCip.IsSuccess)
        {
          analysis = AllenBradleyHelper.ExtractActualData(readCip.Content, true);
          if (analysis.IsSuccess)
          {
            if (readCip.Content.Length >= 43 && BitConverter.ToUInt16(readCip.Content, 40) == (ushort) 213)
            {
              int index = 44;
              while (index < readCip.Content.Length)
              {
                AbTagItem td = new AbTagItem();
                td.InstanceID = BitConverter.ToUInt32(readCip.Content, index);
                instansAddress = (ushort) (td.InstanceID + 1U);
                index += 4;
                ushort nameLen = BitConverter.ToUInt16(readCip.Content, index);
                index += 2;
                td.Name = Encoding.ASCII.GetString(readCip.Content, index, (int) nameLen);
                index += (int) nameLen;
                td.SymbolType = BitConverter.ToUInt16(readCip.Content, index);
                index += 2;
                if (((int) td.SymbolType & 4096) != 4096 && !td.Name.StartsWith("__"))
                  lists.Add(td);
                td = (AbTagItem) null;
              }
              if (analysis.Content3)
              {
                readCip = (OperateResult<byte[]>) null;
                analysis = (OperateResult<byte[], ushort, bool>) null;
              }
              else
                goto label_12;
            }
            else
              goto label_13;
          }
          else
            goto label_4;
        }
        else
          break;
      }
      return OperateResult.CreateFailedResult<AbTagItem[]>((OperateResult) readCip);
label_4:
      return OperateResult.CreateFailedResult<AbTagItem[]>((OperateResult) analysis);
label_12:
      return OperateResult.CreateSuccessResult<AbTagItem[]>(lists.ToArray());
label_13:
      return new OperateResult<AbTagItem[]>(StringResources.Language.UnknownError);
    }

    /// <summary>枚举结构体的方法</summary>
    /// <param name="structTag">结构体的标签</param>
    /// <returns>是否成功</returns>
    [Obsolete("未测试通过")]
    public OperateResult<AbTagItem[]> StructTagEnumerator(AbTagItem structTag)
    {
      OperateResult<AbStructHandle> operateResult1 = this.ReadTagStructHandle(structTag);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<AbTagItem[]>((OperateResult) operateResult1);
      OperateResult<byte[]> operateResult2 = this.ReadCipFromServer(AllenBradleyHelper.GetStructItemNameType(structTag.SymbolType, operateResult1.Content));
      if (!operateResult2.IsSuccess)
        return OperateResult.CreateFailedResult<AbTagItem[]>((OperateResult) operateResult2);
      if (operateResult2.Content.Length < 43 || operateResult2.Content[40] != (byte) 204 || operateResult2.Content[41] != (byte) 0)
        return new OperateResult<AbTagItem[]>(StringResources.Language.UnknownError);
      byte[] bytes = BitConverter.GetBytes(structTag.SymbolType);
      bytes[1] = (byte) ((uint) bytes[1] & 15U);
      return bytes[1] >= (byte) 15 ? OperateResult.CreateSuccessResult<AbTagItem[]>(this.EnumSysStructItemType(operateResult2.Content, operateResult1.Content).ToArray()) : OperateResult.CreateSuccessResult<AbTagItem[]>(this.EnumUserStructItemType(operateResult2.Content, operateResult1.Content).ToArray());
    }

    private OperateResult<AbStructHandle> ReadTagStructHandle(
      AbTagItem structTag)
    {
      OperateResult<byte[]> operateResult = this.ReadByCips(AllenBradleyHelper.GetStructHandleCommand(structTag.SymbolType));
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<AbStructHandle>((OperateResult) operateResult);
      if (operateResult.Content.Length < 43 || BitConverter.ToInt32(operateResult.Content, 40) != 131)
        return new OperateResult<AbStructHandle>(StringResources.Language.UnknownError);
      return OperateResult.CreateSuccessResult<AbStructHandle>(new AbStructHandle()
      {
        Count = BitConverter.ToUInt16(operateResult.Content, 44),
        TemplateObjectDefinitionSize = BitConverter.ToUInt32(operateResult.Content, 50),
        TemplateStructureSize = BitConverter.ToUInt32(operateResult.Content, 58),
        MemberCount = BitConverter.ToUInt16(operateResult.Content, 66),
        StructureHandle = BitConverter.ToUInt16(operateResult.Content, 72)
      });
    }

    private List<AbTagItem> EnumSysStructItemType(
      byte[] Struct_Item_Type_buff,
      AbStructHandle structHandle)
    {
      List<AbTagItem> abTagItemList = new List<AbTagItem>();
      if (Struct_Item_Type_buff.Length > 41 && Struct_Item_Type_buff[40] == (byte) 204 && Struct_Item_Type_buff[41] == (byte) 0 && Struct_Item_Type_buff[42] == (byte) 0)
      {
        int num1 = Struct_Item_Type_buff.Length - 40;
        byte[] numArray1 = new byte[num1 - 4];
        Array.Copy((Array) Struct_Item_Type_buff, 44, (Array) numArray1, 0, num1 - 4);
        byte[] numArray2 = new byte[(int) structHandle.MemberCount * 8];
        Array.Copy((Array) numArray1, 0, (Array) numArray2, 0, (int) structHandle.MemberCount * 8);
        byte[] bytes = new byte[numArray1.Length - numArray2.Length + 1];
        Array.Copy((Array) numArray1, numArray2.Length - 1, (Array) bytes, 0, numArray1.Length - numArray2.Length + 1);
        ushort memberCount = structHandle.MemberCount;
        for (int index = 0; index < (int) memberCount; ++index)
        {
          int num2;
          abTagItemList.Add(new AbTagItem()
          {
            SymbolType = BitConverter.ToUInt16(numArray2, num2 = 8 * index + 2)
          });
        }
        List<int> intList = new List<int>();
        for (int index = 0; index < bytes.Length; ++index)
        {
          if (bytes[index] == (byte) 0)
            intList.Add(index);
        }
        intList.Add(bytes.Length);
        for (int index = 0; index < intList.Count; ++index)
        {
          if (index != 0)
          {
            int count = index + 1 >= intList.Count ? 0 : intList[index + 1] - intList[index] - 1;
            if (count > 0)
              abTagItemList[index - 1].Name = Encoding.ASCII.GetString(bytes, intList[index] + 1, count);
          }
        }
      }
      return abTagItemList;
    }

    private List<AbTagItem> EnumUserStructItemType(
      byte[] Struct_Item_Type_buff,
      AbStructHandle structHandle)
    {
      List<AbTagItem> abTagItemList = new List<AbTagItem>();
      bool flag = false;
      int num1 = 0;
      if (Struct_Item_Type_buff.Length > 41 & Struct_Item_Type_buff[40] == (byte) 204 & Struct_Item_Type_buff[41] == (byte) 0 & Struct_Item_Type_buff[42] == (byte) 0)
      {
        int num2 = Struct_Item_Type_buff.Length - 40;
        byte[] numArray1 = new byte[num2 - 4];
        Array.ConstrainedCopy((Array) Struct_Item_Type_buff, 44, (Array) numArray1, 0, num2 - 4);
        for (int index1 = 0; index1 < numArray1.Length; ++index1)
        {
          if (numArray1[index1] == (byte) 0 & !flag)
            num1 = index1;
          if (numArray1[index1] == (byte) 59 && numArray1[index1 + 1] == (byte) 110)
          {
            int length = index1 - num1 - 1;
            byte[] numArray2 = new byte[length];
            Array.Copy((Array) numArray1, num1 + 1, (Array) numArray2, 0, length);
            byte[] numArray3 = new byte[index1 + 1];
            Array.Copy((Array) numArray1, 0, (Array) numArray3, 0, index1 + 1);
            byte[] bytes = new byte[numArray1.Length - index1 - 1];
            Array.Copy((Array) numArray1, index1 + 1, (Array) bytes, 0, numArray1.Length - index1 - 1);
            if ((num1 + 1) % 8 == 0)
            {
              int num3 = (num1 + 1) / 8 - 1;
              for (int index2 = 0; index2 <= num3; ++index2)
              {
                int num4;
                abTagItemList.Add(new AbTagItem()
                {
                  SymbolType = BitConverter.ToUInt16(numArray3, num4 = 8 * index2 + 2)
                });
              }
              List<int> intList = new List<int>();
              for (int index2 = 0; index2 < bytes.Length; ++index2)
              {
                if (bytes[index2] == (byte) 0)
                  intList.Add(index2);
              }
              intList.Add(bytes.Length);
              for (int index2 = 0; index2 < intList.Count; ++index2)
              {
                int count = index2 + 1 >= intList.Count ? 0 : intList[index2 + 1] - intList[index2] - 1;
                if (count > 0)
                  abTagItemList[index2].Name = Encoding.ASCII.GetString(bytes, intList[index2] + 1, count);
              }
              break;
            }
            break;
          }
        }
      }
      return abTagItemList;
    }

    /// <inheritdoc />
    [EstMqttApi("ReadInt16Array", "")]
    public override OperateResult<short[]> ReadInt16(string address, ushort length) => ByteTransformHelper.GetResultFromBytes<short[]>(this.Read(address, length), (Func<byte[], short[]>) (m => this.ByteTransform.TransInt16(m, 0, (int) length)));

    /// <inheritdoc />
    [EstMqttApi("ReadUInt16Array", "")]
    public override OperateResult<ushort[]> ReadUInt16(string address, ushort length) => ByteTransformHelper.GetResultFromBytes<ushort[]>(this.Read(address, length), (Func<byte[], ushort[]>) (m => this.ByteTransform.TransUInt16(m, 0, (int) length)));

    /// <inheritdoc />
    [EstMqttApi("ReadInt32Array", "")]
    public override OperateResult<int[]> ReadInt32(string address, ushort length) => ByteTransformHelper.GetResultFromBytes<int[]>(this.Read(address, length), (Func<byte[], int[]>) (m => this.ByteTransform.TransInt32(m, 0, (int) length)));

    /// <inheritdoc />
    [EstMqttApi("ReadUInt32Array", "")]
    public override OperateResult<uint[]> ReadUInt32(string address, ushort length) => ByteTransformHelper.GetResultFromBytes<uint[]>(this.Read(address, length), (Func<byte[], uint[]>) (m => this.ByteTransform.TransUInt32(m, 0, (int) length)));

    /// <inheritdoc />
    [EstMqttApi("ReadFloatArray", "")]
    public override OperateResult<float[]> ReadFloat(string address, ushort length) => ByteTransformHelper.GetResultFromBytes<float[]>(this.Read(address, length), (Func<byte[], float[]>) (m => this.ByteTransform.TransSingle(m, 0, (int) length)));

    /// <inheritdoc />
    [EstMqttApi("ReadInt64Array", "")]
    public override OperateResult<long[]> ReadInt64(string address, ushort length) => ByteTransformHelper.GetResultFromBytes<long[]>(this.Read(address, length), (Func<byte[], long[]>) (m => this.ByteTransform.TransInt64(m, 0, (int) length)));

    /// <inheritdoc />
    [EstMqttApi("ReadUInt64Array", "")]
    public override OperateResult<ulong[]> ReadUInt64(string address, ushort length) => ByteTransformHelper.GetResultFromBytes<ulong[]>(this.Read(address, length), (Func<byte[], ulong[]>) (m => this.ByteTransform.TransUInt64(m, 0, (int) length)));

    /// <inheritdoc />
    [EstMqttApi("ReadDoubleArray", "")]
    public override OperateResult<double[]> ReadDouble(string address, ushort length) => ByteTransformHelper.GetResultFromBytes<double[]>(this.Read(address, length), (Func<byte[], double[]>) (m => this.ByteTransform.TransDouble(m, 0, (int) length)));

    /// <inheritdoc />
    public OperateResult<string> ReadString(string address) => this.ReadString(address, (ushort) 1, Encoding.ASCII);

    /// <inheritdoc />
    public override OperateResult<string> ReadString(
      string address,
      ushort length,
      Encoding encoding)
    {
      OperateResult<byte[]> operateResult = this.Read(address, length);
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<string>((OperateResult) operateResult);
      try
      {
        if (operateResult.Content.Length < 6)
          return OperateResult.CreateSuccessResult<string>(encoding.GetString(operateResult.Content));
        int count = this.ByteTransform.TransInt32(operateResult.Content, 2);
        return OperateResult.CreateSuccessResult<string>(encoding.GetString(operateResult.Content, 6, count));
      }
      catch (Exception ex)
      {
        return new OperateResult<string>(ex.Message + " Source: " + operateResult.Content.ToHexString(' '));
      }
    }

    /// <inheritdoc />
    public override async Task<OperateResult<short[]>> ReadInt16Async(
      string address,
      ushort length)
    {
      OperateResult<byte[]> result = await this.ReadAsync(address, length);
      return ByteTransformHelper.GetResultFromBytes<short[]>(result, (Func<byte[], short[]>) (m => this.ByteTransform.TransInt16(m, 0, (int) length)));
    }

    /// <inheritdoc />
    public override async Task<OperateResult<ushort[]>> ReadUInt16Async(
      string address,
      ushort length)
    {
      OperateResult<byte[]> result = await this.ReadAsync(address, length);
      return ByteTransformHelper.GetResultFromBytes<ushort[]>(result, (Func<byte[], ushort[]>) (m => this.ByteTransform.TransUInt16(m, 0, (int) length)));
    }

    /// <inheritdoc />
    public override async Task<OperateResult<int[]>> ReadInt32Async(
      string address,
      ushort length)
    {
      OperateResult<byte[]> result = await this.ReadAsync(address, length);
      return ByteTransformHelper.GetResultFromBytes<int[]>(result, (Func<byte[], int[]>) (m => this.ByteTransform.TransInt32(m, 0, (int) length)));
    }

    /// <inheritdoc />
    public override async Task<OperateResult<uint[]>> ReadUInt32Async(
      string address,
      ushort length)
    {
      OperateResult<byte[]> result = await this.ReadAsync(address, length);
      return ByteTransformHelper.GetResultFromBytes<uint[]>(result, (Func<byte[], uint[]>) (m => this.ByteTransform.TransUInt32(m, 0, (int) length)));
    }

    /// <inheritdoc />
    public override async Task<OperateResult<float[]>> ReadFloatAsync(
      string address,
      ushort length)
    {
      OperateResult<byte[]> result = await this.ReadAsync(address, length);
      return ByteTransformHelper.GetResultFromBytes<float[]>(result, (Func<byte[], float[]>) (m => this.ByteTransform.TransSingle(m, 0, (int) length)));
    }

    /// <inheritdoc />
    public override async Task<OperateResult<long[]>> ReadInt64Async(
      string address,
      ushort length)
    {
      OperateResult<byte[]> result = await this.ReadAsync(address, length);
      return ByteTransformHelper.GetResultFromBytes<long[]>(result, (Func<byte[], long[]>) (m => this.ByteTransform.TransInt64(m, 0, (int) length)));
    }

    /// <inheritdoc />
    public override async Task<OperateResult<ulong[]>> ReadUInt64Async(
      string address,
      ushort length)
    {
      OperateResult<byte[]> result = await this.ReadAsync(address, length);
      return ByteTransformHelper.GetResultFromBytes<ulong[]>(result, (Func<byte[], ulong[]>) (m => this.ByteTransform.TransUInt64(m, 0, (int) length)));
    }

    /// <inheritdoc />
    public override async Task<OperateResult<double[]>> ReadDoubleAsync(
      string address,
      ushort length)
    {
      OperateResult<byte[]> result = await this.ReadAsync(address, length);
      return ByteTransformHelper.GetResultFromBytes<double[]>(result, (Func<byte[], double[]>) (m => this.ByteTransform.TransDouble(m, 0, (int) length)));
    }

    /// <inheritdoc />
    public async Task<OperateResult<string>> ReadStringAsync(string address)
    {
      OperateResult<string> operateResult = await this.ReadStringAsync(address, (ushort) 1, Encoding.ASCII);
      return operateResult;
    }

    /// <inheritdoc />
    public override async Task<OperateResult<string>> ReadStringAsync(
      string address,
      ushort length,
      Encoding encoding)
    {
      OperateResult<byte[]> read = await this.ReadAsync(address, length);
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<string>((OperateResult) read);
      if (read.Content.Length < 6)
        return OperateResult.CreateSuccessResult<string>(encoding.GetString(read.Content));
      int strLength = this.ByteTransform.TransInt32(read.Content, 2);
      return OperateResult.CreateSuccessResult<string>(encoding.GetString(read.Content, 6, strLength));
    }

    /// <summary>
    /// 当前的PLC不支持该功能，需要调用 <see cref="M:EstCommunication.Profinet.AllenBradley.AllenBradleyNet.WriteTag(System.String,System.UInt16,System.Byte[],System.Int32)" /> 方法来实现。<br />
    /// The current PLC does not support this function, you need to call the <see cref="M:EstCommunication.Profinet.AllenBradley.AllenBradleyNet.WriteTag(System.String,System.UInt16,System.Byte[],System.Int32)" /> method to achieve it.
    /// </summary>
    /// <param name="address">地址</param>
    /// <param name="value">值</param>
    /// <returns>写入结果值</returns>
    [EstMqttApi("WriteByteArray", "")]
    public override OperateResult Write(string address, byte[] value) => new OperateResult(StringResources.Language.NotSupportedFunction + " Please refer to use WriteTag instead ");

    /// <summary>
    /// 使用指定的类型写入指定的节点数据<br />
    /// Writes the specified node data with the specified type
    /// </summary>
    /// <param name="address">节点的名称 -&gt; Name of the node </param>
    /// <param name="typeCode">类型代码，详细参见<see cref="T:EstCommunication.Profinet.AllenBradley.AllenBradleyHelper" />上的常用字段 -&gt;  Type code, see the commonly used Fields section on the <see cref="T:EstCommunication.Profinet.AllenBradley.AllenBradleyHelper" /> in detail</param>
    /// <param name="value">实际的数据值 -&gt; The actual data value </param>
    /// <param name="length">如果节点是数组，就是数组长度 -&gt; If the node is an array, it is the array length </param>
    /// <returns>是否写入成功 -&gt; Whether to write successfully</returns>
    public virtual OperateResult WriteTag(
      string address,
      ushort typeCode,
      byte[] value,
      int length = 1)
    {
      OperateResult<byte[]> operateResult1 = this.BuildWriteCommand(address, typeCode, value, length);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return (OperateResult) operateResult2;
      OperateResult result = AllenBradleyHelper.CheckResponse(operateResult2.Content);
      return !result.IsSuccess ? (OperateResult) OperateResult.CreateFailedResult<byte[]>(result) : (OperateResult) AllenBradleyHelper.ExtractActualData(operateResult2.Content, false);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.AllenBradley.AllenBradleyNet.Write(System.String,System.Byte[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      byte[] value)
    {
      OperateResult operateResult = await Task.Run<OperateResult>((Func<OperateResult>) (() => this.Write(address, value)));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.AllenBradley.AllenBradleyNet.WriteTag(System.String,System.UInt16,System.Byte[],System.Int32)" />
    public virtual async Task<OperateResult> WriteTagAsync(
      string address,
      ushort typeCode,
      byte[] value,
      int length = 1)
    {
      OperateResult<byte[]> command = this.BuildWriteCommand(address, typeCode, value, length);
      if (!command.IsSuccess)
        return (OperateResult) command;
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      if (!read.IsSuccess)
        return (OperateResult) read;
      OperateResult check = AllenBradleyHelper.CheckResponse(read.Content);
      return check.IsSuccess ? (OperateResult) AllenBradleyHelper.ExtractActualData(read.Content, false) : (OperateResult) OperateResult.CreateFailedResult<byte[]>(check);
    }

    /// <inheritdoc />
    [EstMqttApi("WriteInt16Array", "")]
    public override OperateResult Write(string address, short[] values) => this.WriteTag(address, (ushort) 195, this.ByteTransform.TransByte(values), values.Length);

    /// <inheritdoc />
    [EstMqttApi("WriteUInt16Array", "")]
    public override OperateResult Write(string address, ushort[] values) => this.WriteTag(address, (ushort) 199, this.ByteTransform.TransByte(values), values.Length);

    /// <inheritdoc />
    [EstMqttApi("WriteInt32Array", "")]
    public override OperateResult Write(string address, int[] values) => this.WriteTag(address, (ushort) 196, this.ByteTransform.TransByte(values), values.Length);

    /// <inheritdoc />
    [EstMqttApi("WriteUInt32Array", "")]
    public override OperateResult Write(string address, uint[] values) => this.WriteTag(address, (ushort) 200, this.ByteTransform.TransByte(values), values.Length);

    /// <inheritdoc />
    [EstMqttApi("WriteFloatArray", "")]
    public override OperateResult Write(string address, float[] values) => this.WriteTag(address, (ushort) 202, this.ByteTransform.TransByte(values), values.Length);

    /// <inheritdoc />
    [EstMqttApi("WriteInt64Array", "")]
    public override OperateResult Write(string address, long[] values) => this.WriteTag(address, (ushort) 197, this.ByteTransform.TransByte(values), values.Length);

    /// <inheritdoc />
    [EstMqttApi("WriteUInt64Array", "")]
    public override OperateResult Write(string address, ulong[] values) => this.WriteTag(address, (ushort) 201, this.ByteTransform.TransByte(values), values.Length);

    /// <inheritdoc />
    [EstMqttApi("WriteDoubleArray", "")]
    public override OperateResult Write(string address, double[] values) => this.WriteTag(address, (ushort) 203, this.ByteTransform.TransByte(values), values.Length);

    /// <inheritdoc />
    [EstMqttApi("WriteString", "")]
    public override OperateResult Write(string address, string value)
    {
      if (string.IsNullOrEmpty(value))
        value = string.Empty;
      byte[] bytes = Encoding.ASCII.GetBytes(value);
      OperateResult operateResult = this.Write(address + ".LEN", bytes.Length);
      if (!operateResult.IsSuccess)
        return operateResult;
      byte[] lengthEven = SoftBasic.ArrayExpandToLengthEven<byte>(bytes);
      return this.WriteTag(address + ".DATA[0]", (ushort) 194, lengthEven, bytes.Length);
    }

    /// <summary>
    /// 写入单个Bool的数据信息。如果读取的是单bool变量，就直接写变量名，如果是bool数组的一个值，一律带下标访问，例如a[0]<br />
    /// Write the data information of a single Bool. If the read is a single bool variable, write the variable name directly,
    /// if it is a value of the bool array, it will always be accessed with a subscript, such as a[0]
    /// </summary>
    /// <param name="address">标签的地址数据</param>
    /// <param name="value">bool数据值</param>
    /// <returns>是否写入成功</returns>
    [EstMqttApi("WriteBool", "")]
    public override OperateResult Write(string address, bool value)
    {
      if (Regex.IsMatch(address, "\\[[0-9]+\\]$"))
      {
        OperateResult<byte[]> operateResult1 = this.BuildWriteCommand(address, value);
        if (!operateResult1.IsSuccess)
          return (OperateResult) operateResult1;
        OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
        if (!operateResult2.IsSuccess)
          return (OperateResult) operateResult2;
        OperateResult result = AllenBradleyHelper.CheckResponse(operateResult2.Content);
        return !result.IsSuccess ? (OperateResult) OperateResult.CreateFailedResult<byte[]>(result) : (OperateResult) AllenBradleyHelper.ExtractActualData(operateResult2.Content, false);
      }
      string address1 = address;
      byte[] numArray;
      if (!value)
        numArray = new byte[2];
      else
        numArray = new byte[2]
        {
          byte.MaxValue,
          byte.MaxValue
        };
      return this.WriteTag(address1, (ushort) 193, numArray);
    }

    /// <summary>
    /// 写入Byte数据，返回是否写入成功<br />
    /// Write Byte data and return whether the writing is successful
    /// </summary>
    /// <param name="address">标签的地址数据</param>
    /// <param name="value">Byte数据</param>
    /// <returns>是否写入成功</returns>
    [EstMqttApi("WriteByte", "")]
    public virtual OperateResult Write(string address, byte value) => this.WriteTag(address, (ushort) 194, new byte[2]
    {
      value,
      (byte) 0
    });

    /// <inheritdoc cref="M:EstCommunication.Profinet.AllenBradley.AllenBradleyNet.Write(System.String,System.Int16[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      short[] values)
    {
      OperateResult operateResult = await this.WriteTagAsync(address, (ushort) 195, this.ByteTransform.TransByte(values), values.Length);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.AllenBradley.AllenBradleyNet.Write(System.String,System.UInt16[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      ushort[] values)
    {
      OperateResult operateResult = await this.WriteTagAsync(address, (ushort) 199, this.ByteTransform.TransByte(values), values.Length);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.AllenBradley.AllenBradleyNet.Write(System.String,System.Int32[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      int[] values)
    {
      OperateResult operateResult = await this.WriteTagAsync(address, (ushort) 196, this.ByteTransform.TransByte(values), values.Length);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.AllenBradley.AllenBradleyNet.Write(System.String,System.UInt32[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      uint[] values)
    {
      OperateResult operateResult = await this.WriteTagAsync(address, (ushort) 200, this.ByteTransform.TransByte(values), values.Length);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.AllenBradley.AllenBradleyNet.Write(System.String,System.Single[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      float[] values)
    {
      OperateResult operateResult = await this.WriteTagAsync(address, (ushort) 202, this.ByteTransform.TransByte(values), values.Length);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.AllenBradley.AllenBradleyNet.Write(System.String,System.Int64[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      long[] values)
    {
      OperateResult operateResult = await this.WriteTagAsync(address, (ushort) 197, this.ByteTransform.TransByte(values), values.Length);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.AllenBradley.AllenBradleyNet.Write(System.String,System.UInt64[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      ulong[] values)
    {
      OperateResult operateResult = await this.WriteTagAsync(address, (ushort) 201, this.ByteTransform.TransByte(values), values.Length);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.AllenBradley.AllenBradleyNet.Write(System.String,System.Double[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      double[] values)
    {
      OperateResult operateResult = await this.WriteTagAsync(address, (ushort) 203, this.ByteTransform.TransByte(values), values.Length);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.AllenBradley.AllenBradleyNet.Write(System.String,System.String)" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      string value)
    {
      if (string.IsNullOrEmpty(value))
        value = string.Empty;
      byte[] data = Encoding.ASCII.GetBytes(value);
      OperateResult write = await this.WriteAsync(address + ".LEN", data.Length);
      if (!write.IsSuccess)
        return write;
      byte[] buffer = SoftBasic.ArrayExpandToLengthEven<byte>(data);
      OperateResult operateResult = await this.WriteTagAsync(address + ".DATA[0]", (ushort) 194, buffer, data.Length);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.AllenBradley.AllenBradleyNet.Write(System.String,System.Boolean)" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      bool value)
    {
      if (Regex.IsMatch(address, "\\[[0-9]+\\]$"))
      {
        OperateResult<byte[]> command = this.BuildWriteCommand(address, value);
        if (!command.IsSuccess)
          return (OperateResult) command;
        OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
        if (!read.IsSuccess)
          return (OperateResult) read;
        OperateResult check = AllenBradleyHelper.CheckResponse(read.Content);
        return check.IsSuccess ? (OperateResult) AllenBradleyHelper.ExtractActualData(read.Content, false) : (OperateResult) OperateResult.CreateFailedResult<byte[]>(check);
      }
      string address1 = address;
      byte[] numArray;
      if (!value)
        numArray = new byte[2];
      else
        numArray = new byte[2]
        {
          byte.MaxValue,
          byte.MaxValue
        };
      OperateResult operateResult = await this.WriteTagAsync(address1, (ushort) 193, numArray);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.AllenBradley.AllenBradleyNet.Write(System.String,System.Byte)" />
    public virtual async Task<OperateResult> WriteAsync(string address, byte value)
    {
      OperateResult operateResult = await this.WriteTagAsync(address, (ushort) 194, new byte[2]
      {
        value,
        (byte) 0
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.AllenBradley.AllenBradleyHelper.PackCommandService(System.Byte[],System.Byte[][])" />
    protected virtual byte[] PackCommandService(byte[] portSlot, params byte[][] cips) => AllenBradleyHelper.PackCommandService(portSlot, cips);

    /// <inheritdoc />
    public override string ToString() => string.Format("AllenBradleyNet[{0}:{1}]", (object) this.IpAddress, (object) this.Port);
  }
}
