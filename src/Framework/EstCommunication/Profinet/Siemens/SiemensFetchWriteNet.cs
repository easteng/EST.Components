// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.Siemens.SiemensFetchWriteNet
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using EstCommunication.Core;
using EstCommunication.Core.IMessage;
using EstCommunication.Core.Net;
using EstCommunication.Reflection;
using System;
using System.Threading.Tasks;

namespace EstCommunication.Profinet.Siemens
{
  /// <summary>
  /// 使用了Fetch/Write协议来和西门子进行通讯，该种方法需要在PLC侧进行一些配置<br />
  /// Using the Fetch/write protocol to communicate with Siemens, this method requires some configuration on the PLC side
  /// </summary>
  /// <remarks>
  /// 配置的参考文章地址：https://www.cnblogs.com/dathlin/p/8685855.html
  /// <br />
  /// 与S7协议相比较而言，本协议不支持对单个的点位的读写操作。如果读取M100.0，需要读取M100的值，然后进行提取位数据。
  /// 
  /// 如果需要写入位地址的数据，可以读取plc的byte值，然后进行与或非，然后写入到plc之中。
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
  ///     <term>×</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>输入寄存器</term>
  ///     <term>I</term>
  ///     <term>I100,I200</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>输出寄存器</term>
  ///     <term>Q</term>
  ///     <term>Q100,Q200</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>DB块寄存器</term>
  ///     <term>DB</term>
  ///     <term>DB1.100,DB1.200</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>定时器的值</term>
  ///     <term>T</term>
  ///     <term>T100,T200</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>计数器的值</term>
  ///     <term>C</term>
  ///     <term>C100,C200</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term></term>
  ///   </item>
  /// </list>
  /// </remarks>
  /// <example>
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\SiemensFetchWriteNet.cs" region="Usage" title="简单的短连接使用" />
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\SiemensFetchWriteNet.cs" region="Usage2" title="简单的长连接使用" />
  /// </example>
  public class SiemensFetchWriteNet : NetworkDeviceBase
  {
    /// <summary>
    /// 实例化一个西门子的Fetch/Write协议的通讯对象<br />
    /// Instantiate a communication object for a Siemens Fetch/write protocol
    /// </summary>
    public SiemensFetchWriteNet()
    {
      this.WordLength = (ushort) 2;
      this.ByteTransform = (IByteTransform) new ReverseBytesTransform();
    }

    /// <summary>
    /// 实例化一个西门子的Fetch/Write协议的通讯对象<br />
    /// Instantiate a communication object for a Siemens Fetch/write protocol
    /// </summary>
    /// <param name="ipAddress">PLC的Ip地址 -&gt; Specify IP Address</param>
    /// <param name="port">PLC的端口 -&gt; Specify IP Port</param>
    public SiemensFetchWriteNet(string ipAddress, int port)
    {
      this.WordLength = (ushort) 2;
      this.IpAddress = ipAddress;
      this.Port = port;
      this.ByteTransform = (IByteTransform) new ReverseBytesTransform();
    }

    /// <inheritdoc />
    protected override INetMessage GetNewNetMessage() => (INetMessage) new FetchWriteMessage();

    /// <summary>
    /// 从PLC读取数据，地址格式为I100，Q100，DB20.100，M100，T100，C100，以字节为单位<br />
    /// Read data from PLC, address format I100,Q100,DB20.100,M100,T100,C100, in bytes
    /// </summary>
    /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100，T100，C100 -&gt;
    /// Starting address, formatted as I100,M100,Q100,DB20.100,T100,C100
    /// </param>
    /// <param name="length">读取的数量，以字节为单位 -&gt; The number of reads, in bytes</param>
    /// <returns>带有成功标志的字节信息 -&gt; Byte information with a success flag</returns>
    /// <example>
    /// 假设起始地址为M100，M100存储了温度，100.6℃值为1006，M102存储了压力，1.23Mpa值为123，M104，M105，M106，M107存储了产量计数，读取如下：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\SiemensFetchWriteNet.cs" region="ReadExample2" title="Read示例" />
    /// 以下是读取不同类型数据的示例
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\SiemensFetchWriteNet.cs" region="ReadExample1" title="Read示例" />
    /// </example>
    [EstMqttApi("ReadByteArray", "")]
    public override OperateResult<byte[]> Read(string address, ushort length)
    {
      OperateResult<byte[]> operateResult1 = SiemensFetchWriteNet.BuildReadCommand(address, length);
      if (!operateResult1.IsSuccess)
        return operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return operateResult2;
      OperateResult result = SiemensFetchWriteNet.CheckResponseContent(operateResult2.Content);
      return !result.IsSuccess ? OperateResult.CreateFailedResult<byte[]>(result) : OperateResult.CreateSuccessResult<byte[]>(SoftBasic.ArrayRemoveBegin<byte>(operateResult2.Content, 16));
    }

    /// <summary>
    /// 将数据写入到PLC数据，地址格式为I100，Q100，DB20.100，M100，以字节为单位<br />
    /// Writes data to the PLC data, in the address format i100,q100,db20.100,m100, in bytes
    /// </summary>
    /// <param name="address">起始地址，格式为M100,I100,Q100,DB1.100 -&gt; Starting address, formatted as M100,I100,Q100,DB1.100</param>
    /// <param name="value">要写入的实际数据 -&gt; The actual data to write</param>
    /// <returns>是否写入成功的结果对象 -&gt; Whether to write a successful result object</returns>
    /// <example>
    /// 假设起始地址为M100，M100,M101存储了温度，100.6℃值为1006，M102,M103存储了压力，1.23Mpa值为123，M104-M107存储了产量计数，写入如下：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\SiemensFetchWriteNet.cs" region="WriteExample2" title="Write示例" />
    /// 以下是写入不同类型数据的示例
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\SiemensFetchWriteNet.cs" region="WriteExample1" title="Write示例" />
    /// </example>
    [EstMqttApi("WriteByteArray", "")]
    public override OperateResult Write(string address, byte[] value)
    {
      OperateResult<byte[]> operateResult1 = SiemensFetchWriteNet.BuildWriteCommand(address, value);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return (OperateResult) operateResult2;
      OperateResult result = SiemensFetchWriteNet.CheckResponseContent(operateResult2.Content);
      return !result.IsSuccess ? (OperateResult) OperateResult.CreateFailedResult<byte[]>(result) : OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Siemens.SiemensFetchWriteNet.Read(System.String,System.UInt16)" />
    public override async Task<OperateResult<byte[]>> ReadAsync(
      string address,
      ushort length)
    {
      OperateResult<byte[]> command = SiemensFetchWriteNet.BuildReadCommand(address, length);
      if (!command.IsSuccess)
        return command;
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      if (!read.IsSuccess)
        return read;
      OperateResult check = SiemensFetchWriteNet.CheckResponseContent(read.Content);
      return check.IsSuccess ? OperateResult.CreateSuccessResult<byte[]>(SoftBasic.ArrayRemoveBegin<byte>(read.Content, 16)) : OperateResult.CreateFailedResult<byte[]>(check);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Siemens.SiemensFetchWriteNet.Write(System.String,System.Byte[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      byte[] value)
    {
      OperateResult<byte[]> command = SiemensFetchWriteNet.BuildWriteCommand(address, value);
      if (!command.IsSuccess)
        return (OperateResult) command;
      OperateResult<byte[]> write = await this.ReadFromCoreServerAsync(command.Content);
      if (!write.IsSuccess)
        return (OperateResult) write;
      OperateResult check = SiemensFetchWriteNet.CheckResponseContent(write.Content);
      return check.IsSuccess ? OperateResult.CreateSuccessResult() : (OperateResult) OperateResult.CreateFailedResult<byte[]>(check);
    }

    /// <summary>
    /// 读取指定地址的byte数据<br />
    /// Reads the byte data for the specified address
    /// </summary>
    /// <param name="address">起始地址，格式为M100,I100,Q100,DB1.100 -&gt; Starting address, formatted as M100,I100,Q100,DB1.100</param>
    /// <returns>byte类型的结果对象 -&gt; Result object of type Byte</returns>
    /// <remarks>
    /// <note type="warning">
    /// 不适用于DB块，定时器，计数器的数据读取，会提示相应的错误，读取长度必须为偶数
    /// </note>
    /// </remarks>
    [EstMqttApi("ReadByte", "")]
    public OperateResult<byte> ReadByte(string address) => ByteTransformHelper.GetResultFromArray<byte>(this.Read(address, (ushort) 1));

    /// <summary>
    /// 向PLC中写入byte数据，返回是否写入成功<br />
    /// Writes byte data to the PLC and returns whether the write succeeded
    /// </summary>
    /// <param name="address">起始地址，格式为M100,I100,Q100,DB1.100 -&gt; Starting address, formatted as M100,I100,Q100,DB1.100</param>
    /// <param name="value">要写入的实际数据 -&gt; The actual data to write</param>
    /// <returns>是否写入成功的结果对象 -&gt; Whether to write a successful result object</returns>
    [EstMqttApi("WriteByte", "")]
    public OperateResult Write(string address, byte value) => this.Write(address, new byte[1]
    {
      value
    });

    /// <inheritdoc cref="M:EstCommunication.Profinet.Siemens.SiemensFetchWriteNet.ReadByte(System.String)" />
    public async Task<OperateResult<byte>> ReadByteAsync(string address)
    {
      OperateResult<byte[]> result = await this.ReadAsync(address, (ushort) 1);
      return ByteTransformHelper.GetResultFromArray<byte>(result);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Siemens.SiemensFetchWriteNet.Write(System.String,System.Byte)" />
    public async Task<OperateResult> WriteAsync(string address, byte value)
    {
      OperateResult operateResult = await this.WriteAsync(address, new byte[1]
      {
        value
      });
      return operateResult;
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("SiemensFetchWriteNet[{0}:{1}]", (object) this.IpAddress, (object) this.Port);

    /// <summary>
    /// 计算特殊的地址信息<br />
    /// Calculate special address information
    /// </summary>
    /// <param name="address">字符串信息</param>
    /// <returns>实际值</returns>
    private static int CalculateAddressStarted(string address)
    {
      if (address.IndexOf('.') < 0)
        return Convert.ToInt32(address);
      return Convert.ToInt32(address.Split('.')[0]);
    }

    private static OperateResult CheckResponseContent(byte[] content) => content[8] > (byte) 0 ? new OperateResult((int) content[8], StringResources.Language.SiemensWriteError + content[8].ToString()) : OperateResult.CreateSuccessResult();

    /// <summary>
    /// 解析数据地址，解析出地址类型，起始地址，DB块的地址<br />
    /// Parse data address, parse out address type, start address, db block address
    /// </summary>
    /// <param name="address">起始地址，格式为M100,I100,Q100,DB1.100 -&gt; Starting address, formatted as M100,I100,Q100,DB1.100</param>
    /// <returns>解析出地址类型，起始地址，DB块的地址 -&gt; Resolves address type, start address, db block address</returns>
    private static OperateResult<byte, int, ushort> AnalysisAddress(string address)
    {
      OperateResult<byte, int, ushort> operateResult = new OperateResult<byte, int, ushort>();
      try
      {
        operateResult.Content3 = (ushort) 0;
        if (address[0] == 'I')
        {
          operateResult.Content1 = (byte) 3;
          operateResult.Content2 = SiemensFetchWriteNet.CalculateAddressStarted(address.Substring(1));
        }
        else if (address[0] == 'Q')
        {
          operateResult.Content1 = (byte) 4;
          operateResult.Content2 = SiemensFetchWriteNet.CalculateAddressStarted(address.Substring(1));
        }
        else if (address[0] == 'M')
        {
          operateResult.Content1 = (byte) 2;
          operateResult.Content2 = SiemensFetchWriteNet.CalculateAddressStarted(address.Substring(1));
        }
        else if (address[0] == 'D' || address.Substring(0, 2) == "DB")
        {
          operateResult.Content1 = (byte) 1;
          string[] strArray = address.Split('.');
          operateResult.Content3 = address[1] != 'B' ? Convert.ToUInt16(strArray[0].Substring(1)) : Convert.ToUInt16(strArray[0].Substring(2));
          if (operateResult.Content3 > (ushort) byte.MaxValue)
          {
            operateResult.Message = StringResources.Language.SiemensDBAddressNotAllowedLargerThan255;
            return operateResult;
          }
          operateResult.Content2 = SiemensFetchWriteNet.CalculateAddressStarted(address.Substring(address.IndexOf('.') + 1));
        }
        else if (address[0] == 'T')
        {
          operateResult.Content1 = (byte) 7;
          operateResult.Content2 = SiemensFetchWriteNet.CalculateAddressStarted(address.Substring(1));
        }
        else if (address[0] == 'C')
        {
          operateResult.Content1 = (byte) 6;
          operateResult.Content2 = SiemensFetchWriteNet.CalculateAddressStarted(address.Substring(1));
        }
        else
        {
          operateResult.Message = StringResources.Language.NotSupportedDataType;
          operateResult.Content1 = (byte) 0;
          operateResult.Content2 = 0;
          operateResult.Content3 = (ushort) 0;
          return operateResult;
        }
      }
      catch (Exception ex)
      {
        operateResult.Message = ex.Message;
        return operateResult;
      }
      operateResult.IsSuccess = true;
      return operateResult;
    }

    /// <summary>
    /// 生成一个读取字数据指令头的通用方法<br />
    /// A general method for generating a command header to read a Word data
    /// </summary>
    /// <param name="address">起始地址，格式为M100,I100,Q100,DB1.100 -&gt; Starting address, formatted as M100,I100,Q100,DB1.100</param>
    /// <param name="count">读取数据个数 -&gt; Number of Read data</param>
    /// <returns>带结果对象的报文数据 -&gt; Message data with a result object</returns>
    public static OperateResult<byte[]> BuildReadCommand(string address, ushort count)
    {
      OperateResult<byte, int, ushort> operateResult = SiemensFetchWriteNet.AnalysisAddress(address);
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult);
      byte[] numArray = new byte[16]
      {
        (byte) 83,
        (byte) 53,
        (byte) 16,
        (byte) 1,
        (byte) 3,
        (byte) 5,
        (byte) 3,
        (byte) 8,
        operateResult.Content1,
        (byte) operateResult.Content3,
        (byte) (operateResult.Content2 / 256),
        (byte) (operateResult.Content2 % 256),
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
      };
      if (operateResult.Content1 == (byte) 1 || operateResult.Content1 == (byte) 6 || operateResult.Content1 == (byte) 7)
      {
        if ((uint) count % 2U > 0U)
          return new OperateResult<byte[]>(StringResources.Language.SiemensReadLengthMustBeEvenNumber);
        numArray[12] = BitConverter.GetBytes((int) count / 2)[1];
        numArray[13] = BitConverter.GetBytes((int) count / 2)[0];
      }
      else
      {
        numArray[12] = BitConverter.GetBytes(count)[1];
        numArray[13] = BitConverter.GetBytes(count)[0];
      }
      numArray[14] = byte.MaxValue;
      numArray[15] = (byte) 2;
      return OperateResult.CreateSuccessResult<byte[]>(numArray);
    }

    /// <summary>
    /// 生成一个写入字节数据的指令<br />
    /// Generate an instruction to write byte data
    /// </summary>
    /// <param name="address">起始地址，格式为M100,I100,Q100,DB1.100 -&gt; Starting address, formatted as M100,I100,Q100,DB1.100</param>
    /// <param name="data">实际的写入的内容 -&gt; The actual content of the write</param>
    /// <returns>带结果对象的报文数据 -&gt; Message data with a result object</returns>
    public static OperateResult<byte[]> BuildWriteCommand(string address, byte[] data)
    {
      if (data == null)
        data = new byte[0];
      OperateResult<byte, int, ushort> operateResult = SiemensFetchWriteNet.AnalysisAddress(address);
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult);
      byte[] numArray = new byte[16 + data.Length];
      numArray[0] = (byte) 83;
      numArray[1] = (byte) 53;
      numArray[2] = (byte) 16;
      numArray[3] = (byte) 1;
      numArray[4] = (byte) 3;
      numArray[5] = (byte) 3;
      numArray[6] = (byte) 3;
      numArray[7] = (byte) 8;
      numArray[8] = operateResult.Content1;
      numArray[9] = (byte) operateResult.Content3;
      numArray[10] = (byte) (operateResult.Content2 / 256);
      numArray[11] = (byte) (operateResult.Content2 % 256);
      if (operateResult.Content1 == (byte) 1 || operateResult.Content1 == (byte) 6 || operateResult.Content1 == (byte) 7)
      {
        if ((uint) (data.Length % 2) > 0U)
          return new OperateResult<byte[]>(StringResources.Language.SiemensReadLengthMustBeEvenNumber);
        numArray[12] = BitConverter.GetBytes(data.Length / 2)[1];
        numArray[13] = BitConverter.GetBytes(data.Length / 2)[0];
      }
      else
      {
        numArray[12] = BitConverter.GetBytes(data.Length)[1];
        numArray[13] = BitConverter.GetBytes(data.Length)[0];
      }
      numArray[14] = byte.MaxValue;
      numArray[15] = (byte) 2;
      Array.Copy((Array) data, 0, (Array) numArray, 16, data.Length);
      return OperateResult.CreateSuccessResult<byte[]>(numArray);
    }
  }
}
