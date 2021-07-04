// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.Omron.OmronFinsNet
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.Core;
using EstCommunication.Core.IMessage;
using EstCommunication.Core.Net;
using EstCommunication.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace EstCommunication.Profinet.Omron
{
  /// <summary>
  /// 欧姆龙PLC通讯类，采用Fins-Tcp通信协议实现，支持的地址信息参见api文档信息。<br />
  /// Omron PLC communication class is implemented using Fins-Tcp communication protocol. For the supported address information, please refer to the api document information.
  /// </summary>
  /// <remarks>
  /// <note type="important">PLC的IP地址的要求，最后一个整数的范围应该小于250，否则会发生连接不上的情况。</note>
  /// <br />
  /// <note type="warning">如果在测试的时候报错误码64，经网友 上海-Lex 指点，是因为PLC中产生了报警，如伺服报警，模块错误等产生的，但是数据还是能正常读到的，屏蔽64报警或清除plc错误可解决</note>
  /// <br />
  /// <note type="warning">如果碰到NX系列连接失败，或是无法读取的，需要使用网口2，配置ip地址，网线连接网口2，配置FINSTCP，把UDP的端口改成9601的，这样就可以读写了。</note><br />
  /// 需要特别注意<see cref="P:EstCommunication.Profinet.Omron.OmronFinsNet.ReadSplits" />属性，在超长数据读取时，规定了切割读取的长度，在不是CP1H及扩展模块的时候，可以设置为999，提高一倍的通信速度。
  /// </remarks>
  /// <example>
  /// 地址列表：
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
  ///     <term>DM Area</term>
  ///     <term>D</term>
  ///     <term>D100,D200</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>CIO Area</term>
  ///     <term>C</term>
  ///     <term>C100,C200</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>Work Area</term>
  ///     <term>W</term>
  ///     <term>W100,W200</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>Holding Bit Area</term>
  ///     <term>H</term>
  ///     <term>H100,H200</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>Auxiliary Bit Area</term>
  ///     <term>A</term>
  ///     <term>A100,A200</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>EM Area</term>
  ///     <term>E</term>
  ///     <term>E0.0,EF.200,E10.100</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  /// </list>
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\OmronFinsNet.cs" region="Usage" title="简单的短连接使用" />
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\OmronFinsNet.cs" region="Usage2" title="简单的长连接使用" />
  /// </example>
  public class OmronFinsNet : NetworkDeviceBase
  {
    private readonly byte[] handSingle = new byte[20]
    {
      (byte) 70,
      (byte) 73,
      (byte) 78,
      (byte) 83,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 12,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0
    };

    /// <summary>
    /// 实例化一个欧姆龙PLC Fins帧协议的通讯对象<br />
    /// Instantiate a communication object of Omron PLC Fins frame protocol
    /// </summary>
    public OmronFinsNet()
    {
      this.WordLength = (ushort) 1;
      this.ByteTransform = (IByteTransform) new ReverseWordTransform();
      this.ByteTransform.DataFormat = DataFormat.CDAB;
      this.ByteTransform.IsStringReverseByteWord = true;
    }

    /// <summary>
    /// 指定ip地址和端口号来实例化一个欧姆龙PLC Fins帧协议的通讯对象<br />
    /// Specify the IP address and port number to instantiate a communication object of the Omron PLC Fins frame protocol
    /// </summary>
    /// <param name="ipAddress">PLCd的Ip地址</param>
    /// <param name="port">PLC的端口</param>
    public OmronFinsNet(string ipAddress, int port)
      : this()
    {
      this.IpAddress = ipAddress;
      this.Port = port;
    }

    /// <inheritdoc />
    protected override INetMessage GetNewNetMessage() => (INetMessage) new FinsMessage();

    /// <inheritdoc />
    [EstMqttApi(Description = "Get or set the IP address of the remote server. If it is a local test, then it needs to be set to 127.0.0.1", HttpMethod = "GET")]
    public override string IpAddress
    {
      get => base.IpAddress;
      set
      {
        base.IpAddress = value;
        this.DA1 = Convert.ToByte(base.IpAddress.Substring(base.IpAddress.LastIndexOf(".") + 1));
      }
    }

    /// <summary>
    /// 信息控制字段，默认0x80<br />
    /// Information control field, default 0x80
    /// </summary>
    public byte ICF { get; set; } = 128;

    /// <summary>
    /// 系统使用的内部信息<br />
    /// Internal information used by the system
    /// </summary>
    public byte RSV { get; private set; } = 0;

    /// <summary>
    /// 网络层信息，默认0x02，如果有八层消息，就设置为0x07<br />
    /// Network layer information, default is 0x02, if there are eight layers of messages, set to 0x07
    /// </summary>
    public byte GCT { get; set; } = 2;

    /// <summary>
    /// PLC的网络号地址，默认0x00<br />
    /// PLC network number address, default 0x00
    /// </summary>
    public byte DNA { get; set; } = 0;

    /// <summary>
    /// PLC的节点地址，这个值在配置了ip地址之后是默认赋值的，默认为Ip地址的最后一位<br />
    /// PLC node address. This value is assigned by default after the IP address is configured. The default is the last bit of the IP address.
    /// </summary>
    /// <remarks>
    /// <note type="important">假如你的PLC的Ip地址为192.168.0.10，那么这个值就是10</note>
    /// </remarks>
    [EstMqttApi(Description = "PLC node address. This value is assigned by default after the IP address is configured. The default is the last bit of the IP address.", HttpMethod = "GET")]
    public byte DA1 { get; set; } = 19;

    /// <summary>
    /// PLC的单元号地址，通常都为0<br />
    /// PLC unit number address, usually 0
    /// </summary>
    /// <remarks>
    /// <note type="important">通常都为0</note>
    /// </remarks>
    public byte DA2 { get; set; } = 0;

    /// <summary>
    /// 上位机的网络号地址<br />
    /// Network number and address of the computer
    /// </summary>
    public byte SNA { get; set; } = 0;

    /// <summary>
    /// 上位机的节点地址，默认是0x01，当连接PLC之后，将由PLC来设定当前的值。<br />
    /// The node address of the host computer is 0x01 by default. After connecting to the PLC, the PLC will set the current value.
    /// </summary>
    /// <remarks>
    /// <note type="important">v9.6.5版本及之前的版本都需要手动设置，如果是多连接，相同的节点是连接不上PLC的。</note>
    /// </remarks>
    [EstMqttApi(Description = "The node address of the host computer is 0x01 by default. After connecting to the PLC, the PLC will set the current value.", HttpMethod = "GET")]
    public byte SA1 { get; set; } = 1;

    /// <summary>
    /// 上位机的单元号地址<br />
    /// Unit number and address of the computer
    /// </summary>
    public byte SA2 { get; set; }

    /// <summary>
    /// 设备的标识号<br />
    /// Device identification number
    /// </summary>
    public byte SID { get; set; } = 0;

    /// <summary>
    /// 进行字读取的时候对于超长的情况按照本属性进行切割，默认500，如果不是CP1H及扩展模块的，可以设置为999，可以提高一倍的通信速度。<br />
    /// When reading words, it is cut according to this attribute for the case of overlength. The default is 500.
    /// If it is not for CP1H and expansion modules, it can be set to 999, which can double the communication speed.
    /// </summary>
    public int ReadSplits { get; set; } = 500;

    /// <summary>将普通的指令打包成完整的指令</summary>
    /// <param name="cmd">FINS的核心指令</param>
    /// <returns>完整的可用于发送PLC的命令</returns>
    private byte[] PackCommand(byte[] cmd)
    {
      byte[] numArray = new byte[26 + cmd.Length];
      Array.Copy((Array) this.handSingle, 0, (Array) numArray, 0, 4);
      byte[] bytes = BitConverter.GetBytes(numArray.Length - 8);
      Array.Reverse((Array) bytes);
      bytes.CopyTo((Array) numArray, 4);
      numArray[11] = (byte) 2;
      numArray[16] = this.ICF;
      numArray[17] = this.RSV;
      numArray[18] = this.GCT;
      numArray[19] = this.DNA;
      numArray[20] = this.DA1;
      numArray[21] = this.DA2;
      numArray[22] = this.SNA;
      numArray[23] = this.SA1;
      numArray[24] = this.SA2;
      numArray[25] = this.SID;
      cmd.CopyTo((Array) numArray, 26);
      return numArray;
    }

    /// <summary>
    /// 根据类型地址长度确认需要读取的指令头<br />
    /// Confirm the instruction header to be read according to the type address length
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="length">长度</param>
    /// <param name="isBit">是否是位读取</param>
    /// <returns>带有成功标志的报文数据</returns>
    public OperateResult<List<byte[]>> BuildReadCommand(
      string address,
      ushort length,
      bool isBit)
    {
      OperateResult<List<byte[]>> operateResult = OmronFinsNetHelper.BuildReadCommand(address, length, isBit, this.ReadSplits);
      return !operateResult.IsSuccess ? operateResult : OperateResult.CreateSuccessResult<List<byte[]>>(operateResult.Content.Select<byte[], byte[]>((Func<byte[], byte[]>) (m => this.PackCommand(m))).ToList<byte[]>());
    }

    /// <summary>
    /// 根据类型地址以及需要写入的数据来生成指令头<br />
    /// Generate instruction header based on type address and data to be writtens
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="value">真实的数据值信息</param>
    /// <param name="isBit">是否是位操作</param>
    /// <returns>带有成功标志的报文数据</returns>
    public OperateResult<byte[]> BuildWriteCommand(
      string address,
      byte[] value,
      bool isBit)
    {
      OperateResult<byte[]> operateResult = OmronFinsNetHelper.BuildWriteWordCommand(address, value, isBit);
      return !operateResult.IsSuccess ? operateResult : OperateResult.CreateSuccessResult<byte[]>(this.PackCommand(operateResult.Content));
    }

    /// <inheritdoc />
    protected override OperateResult InitializationOnConnect(Socket socket)
    {
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(socket, this.handSingle);
      if (!operateResult.IsSuccess)
        return (OperateResult) operateResult;
      int int32 = BitConverter.ToInt32(new byte[4]
      {
        operateResult.Content[15],
        operateResult.Content[14],
        operateResult.Content[13],
        operateResult.Content[12]
      }, 0);
      if ((uint) int32 > 0U)
        return new OperateResult(int32, OmronFinsNetHelper.GetStatusDescription(int32));
      if (operateResult.Content.Length >= 20)
        this.SA1 = operateResult.Content[19];
      if (operateResult.Content.Length >= 24)
        this.DA1 = operateResult.Content[23];
      return OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc />
    protected override async Task<OperateResult> InitializationOnConnectAsync(
      Socket socket)
    {
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(socket, this.handSingle);
      if (!read.IsSuccess)
        return (OperateResult) read;
      byte[] buffer = new byte[4]
      {
        read.Content[15],
        read.Content[14],
        read.Content[13],
        read.Content[12]
      };
      int status = BitConverter.ToInt32(buffer, 0);
      if ((uint) status > 0U)
        return new OperateResult(status, OmronFinsNetHelper.GetStatusDescription(status));
      if (read.Content.Length >= 20)
        this.SA1 = read.Content[19];
      if (read.Content.Length >= 24)
        this.DA1 = read.Content[23];
      return OperateResult.CreateSuccessResult();
    }

    /// <summary>
    /// 从欧姆龙PLC中读取想要的数据，返回读取结果，读取长度的单位为字，地址格式为"D100","C100","W100","H100","A100"<br />
    /// Read the desired data from the Omron PLC and return the read result. The unit of the read length is word. The address format is "D100", "C100", "W100", "H100", "A100"
    /// </summary>
    /// <param name="address">读取地址，格式为"D100","C100","W100","H100","A100"</param>
    /// <param name="length">读取的数据长度</param>
    /// <returns>带成功标志的结果数据对象</returns>
    /// <example>
    /// 假设起始地址为D100，D100存储了温度，100.6℃值为1006，D101存储了压力，1.23Mpa值为123，D102,D103存储了产量计数，读取如下：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\OmronFinsNet.cs" region="ReadExample2" title="Read示例" />
    /// 以下是读取不同类型数据的示例
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\OmronFinsNet.cs" region="ReadExample1" title="Read示例" />
    /// </example>
    [EstMqttApi("ReadByteArray", "")]
    public override OperateResult<byte[]> Read(string address, ushort length)
    {
      OperateResult<List<byte[]>> operateResult1 = this.BuildReadCommand(address, length, false);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult1);
      List<byte> byteList = new List<byte>();
      for (int index = 0; index < operateResult1.Content.Count; ++index)
      {
        OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content[index]);
        if (!operateResult2.IsSuccess)
          return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult2);
        OperateResult<byte[]> operateResult3 = OmronFinsNetHelper.ResponseValidAnalysis(operateResult2.Content, true);
        if (!operateResult3.IsSuccess)
          return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult3);
        byteList.AddRange((IEnumerable<byte>) operateResult3.Content);
      }
      return OperateResult.CreateSuccessResult<byte[]>(byteList.ToArray());
    }

    /// <summary>
    /// 向PLC写入数据，数据格式为原始的字节类型，地址格式为"D100","C100","W100","H100","A100"<br />
    /// Write data to PLC, the data format is the original byte type, and the address format is "D100", "C100", "W100", "H100", "A100"
    /// </summary>
    /// <param name="address">初始地址</param>
    /// <param name="value">原始的字节数据</param>
    /// <returns>结果</returns>
    /// <example>
    /// 假设起始地址为D100，D100存储了温度，100.6℃值为1006，D101存储了压力，1.23Mpa值为123，D102,D103存储了产量计数，读取如下：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\OmronFinsNet.cs" region="WriteExample2" title="Write示例" />
    /// 以下是写入不同类型数据的示例
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\OmronFinsNet.cs" region="WriteExample1" title="Write示例" />
    /// </example>
    [EstMqttApi("WriteByteArray", "")]
    public override OperateResult Write(string address, byte[] value)
    {
      OperateResult<byte[]> operateResult1 = this.BuildWriteCommand(address, value, false);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return (OperateResult) operateResult2;
      OperateResult<byte[]> operateResult3 = OmronFinsNetHelper.ResponseValidAnalysis(operateResult2.Content, false);
      return !operateResult3.IsSuccess ? (OperateResult) operateResult3 : OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Omron.OmronFinsNet.Read(System.String,System.UInt16)" />
    public override async Task<OperateResult<byte[]>> ReadAsync(
      string address,
      ushort length)
    {
      OperateResult<List<byte[]>> command = this.BuildReadCommand(address, length, false);
      if (!command.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) command);
      List<byte> contentArray = new List<byte>();
      for (int i = 0; i < command.Content.Count; ++i)
      {
        OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content[i]);
        if (!read.IsSuccess)
          return OperateResult.CreateFailedResult<byte[]>((OperateResult) read);
        OperateResult<byte[]> valid = OmronFinsNetHelper.ResponseValidAnalysis(read.Content, true);
        if (!valid.IsSuccess)
          return OperateResult.CreateFailedResult<byte[]>((OperateResult) valid);
        contentArray.AddRange((IEnumerable<byte>) valid.Content);
        read = (OperateResult<byte[]>) null;
        valid = (OperateResult<byte[]>) null;
      }
      return OperateResult.CreateSuccessResult<byte[]>(contentArray.ToArray());
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Omron.OmronFinsNet.Write(System.String,System.Byte[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      byte[] value)
    {
      OperateResult<byte[]> command = this.BuildWriteCommand(address, value, false);
      if (!command.IsSuccess)
        return (OperateResult) command;
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      if (!read.IsSuccess)
        return (OperateResult) read;
      OperateResult<byte[]> valid = OmronFinsNetHelper.ResponseValidAnalysis(read.Content, false);
      return valid.IsSuccess ? OperateResult.CreateSuccessResult() : (OperateResult) valid;
    }

    /// <summary>
    /// 从欧姆龙PLC中批量读取位软元件，地址格式为"D100.0","C100.0","W100.0","H100.0","A100.0"<br />
    /// Read bit devices in batches from Omron PLC with address format "D100.0", "C100.0", "W100.0", "H100.0", "A100.0"
    /// </summary>
    /// <param name="address">读取地址，格式为"D100","C100","W100","H100","A100"</param>
    /// <param name="length">读取的长度</param>
    /// <returns>带成功标志的结果数据对象</returns>
    /// <example>
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\OmronFinsNet.cs" region="ReadBool" title="ReadBool示例" />
    /// </example>
    [EstMqttApi("ReadBoolArray", "")]
    public override OperateResult<bool[]> ReadBool(string address, ushort length)
    {
      OperateResult<List<byte[]>> operateResult1 = this.BuildReadCommand(address, length, true);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult1);
      List<bool> boolList = new List<bool>();
      for (int index = 0; index < operateResult1.Content.Count; ++index)
      {
        OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content[index]);
        if (!operateResult2.IsSuccess)
          return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult2);
        OperateResult<byte[]> operateResult3 = OmronFinsNetHelper.ResponseValidAnalysis(operateResult2.Content, true);
        if (!operateResult3.IsSuccess)
          return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult3);
        boolList.AddRange(((IEnumerable<byte>) operateResult3.Content).Select<byte, bool>((Func<byte, bool>) (m => m > (byte) 0)));
      }
      return OperateResult.CreateSuccessResult<bool[]>(boolList.ToArray());
    }

    /// <summary>
    /// 向PLC中位软元件写入bool数组，返回是否写入成功，比如你写入D100,values[0]对应D100.0，地址格式为"D100.0","C100.0","W100.0","H100.0","A100.0"<br />
    /// Write the bool array to the PLC's median device and return whether the write was successful. For example, if you write D100, values [0] corresponds to D100.0
    /// and the address format is "D100.0", "C100.0", "W100. 0 "," H100.0 "," A100.0 "
    /// </summary>
    /// <param name="address">要写入的数据地址</param>
    /// <param name="values">要写入的实际数据，可以指定任意的长度</param>
    /// <returns>返回写入结果</returns>
    /// <example>
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\OmronFinsNet.cs" region="WriteBool" title="WriteBool示例" />
    /// </example>
    [EstMqttApi("WriteBoolArray", "")]
    public override OperateResult Write(string address, bool[] values)
    {
      OperateResult<byte[]> operateResult1 = this.BuildWriteCommand(address, ((IEnumerable<bool>) values).Select<bool, byte>((Func<bool, byte>) (m => !m ? (byte) 0 : (byte) 1)).ToArray<byte>(), true);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return (OperateResult) operateResult2;
      OperateResult<byte[]> operateResult3 = OmronFinsNetHelper.ResponseValidAnalysis(operateResult2.Content, false);
      return !operateResult3.IsSuccess ? (OperateResult) operateResult3 : OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Omron.OmronFinsNet.ReadBool(System.String,System.UInt16)" />
    public override async Task<OperateResult<bool[]>> ReadBoolAsync(
      string address,
      ushort length)
    {
      OperateResult<List<byte[]>> command = this.BuildReadCommand(address, length, true);
      if (!command.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) command);
      List<bool> contentArray = new List<bool>();
      for (int i = 0; i < command.Content.Count; ++i)
      {
        OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content[i]);
        if (!read.IsSuccess)
          return OperateResult.CreateFailedResult<bool[]>((OperateResult) read);
        OperateResult<byte[]> valid = OmronFinsNetHelper.ResponseValidAnalysis(read.Content, true);
        if (!valid.IsSuccess)
          return OperateResult.CreateFailedResult<bool[]>((OperateResult) valid);
        contentArray.AddRange(((IEnumerable<byte>) valid.Content).Select<byte, bool>((Func<byte, bool>) (m => m > (byte) 0)));
        read = (OperateResult<byte[]>) null;
        valid = (OperateResult<byte[]>) null;
      }
      return OperateResult.CreateSuccessResult<bool[]>(contentArray.ToArray());
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Omron.OmronFinsNet.Write(System.String,System.Boolean[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      bool[] values)
    {
      OperateResult<byte[]> command = this.BuildWriteCommand(address, ((IEnumerable<bool>) values).Select<bool, byte>((Func<bool, byte>) (m => !m ? (byte) 0 : (byte) 1)).ToArray<byte>(), true);
      if (!command.IsSuccess)
        return (OperateResult) command;
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      if (!read.IsSuccess)
        return (OperateResult) read;
      OperateResult<byte[]> valid = OmronFinsNetHelper.ResponseValidAnalysis(read.Content, false);
      return valid.IsSuccess ? OperateResult.CreateSuccessResult() : (OperateResult) valid;
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("OmronFinsNet[{0}:{1}]", (object) this.IpAddress, (object) this.Port);
  }
}
