// Decompiled with JetBrains decompiler
// Type: EstCommunication.ModBus.ModbusTcpNet
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
using System.Net.Sockets;
using System.Threading.Tasks;

namespace EstCommunication.ModBus
{
  /// <summary>
  /// Modbus-Tcp协议的客户端通讯类，方便的和服务器进行数据交互，支持标准的功能码，也支持扩展的功能码实现，地址采用富文本的形式，详细见备注说明<br />
  /// The client communication class of Modbus-Tcp protocol is convenient for data interaction with the server. It supports standard function codes and also supports extended function codes.
  /// The address is in rich text. For details, see the remarks.
  /// </summary>
  /// <remarks>
  /// 本客户端支持的标准的modbus协议，Modbus-Tcp及Modbus-Udp内置的消息号会进行自增，地址支持富文本格式，具体参考示例代码。<br />
  /// 读取线圈，输入线圈，寄存器，输入寄存器的方法中的读取长度对商业授权用户不限制，内部自动切割读取，结果合并。
  /// </remarks>
  /// <example>
  /// 本客户端支持的标准的modbus协议，Modbus-Tcp及Modbus-Udp内置的消息号会进行自增，比如我们想要控制消息号在0-1000之间自增，不能超过一千，可以写如下的代码：
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Modbus\Modbus.cs" region="Sample1" title="序号示例" />
  /// <note type="important">
  /// 地址共可以携带3个信息，最完整的表示方式"s=2;x=3;100"，对应的modbus报文是 02 03 00 64 00 01 的前四个字节，站号，功能码，起始地址，下面举例
  /// </note>
  /// 当读写int, uint, float, double, long, ulong类型的时候，支持动态指定数据格式，也就是 DataFormat 信息，本部分内容为商业授权用户专有，感谢支持。<br />
  /// ReadInt32("format=BADC;100") 指示使用BADC的格式来解析byte数组，从而获得int数据，同时支持和站号信息叠加，例如：ReadInt32("format=BADC;s=2;100")
  /// <list type="definition">
  /// <item>
  ///     <term>读取线圈</term>
  ///     <description>ReadCoil("100")表示读取线圈100的值，ReadCoil("s=2;100")表示读取站号为2，线圈地址为100的值</description>
  /// </item>
  /// <item>
  ///     <term>读取离散输入</term>
  ///     <description>ReadDiscrete("100")表示读取离散输入100的值，ReadDiscrete("s=2;100")表示读取站号为2，离散地址为100的值</description>
  /// </item>
  /// <item>
  ///     <term>读取寄存器</term>
  ///     <description>ReadInt16("100")表示读取寄存器100的值，ReadInt16("s=2;100")表示读取站号为2，寄存器100的值</description>
  /// </item>
  /// <item>
  ///     <term>读取输入寄存器</term>
  ///     <description>ReadInt16("x=4;100")表示读取输入寄存器100的值，ReadInt16("s=2;x=4;100")表示读取站号为2，输入寄存器100的值</description>
  /// </item>
  /// </list>
  /// 对于写入来说也是一致的
  /// <list type="definition">
  /// <item>
  ///     <term>写入线圈</term>
  ///     <description>WriteCoil("100",true)表示读取线圈100的值，WriteCoil("s=2;100",true)表示读取站号为2，线圈地址为100的值</description>
  /// </item>
  /// <item>
  ///     <term>写入寄存器</term>
  ///     <description>Write("100",(short)123)表示写寄存器100的值123，Write("s=2;100",(short)123)表示写入站号为2，寄存器100的值123</description>
  /// </item>
  /// </list>
  /// 特殊说明部分：
  ///  <list type="definition">
  /// <item>
  ///     <term>01功能码</term>
  ///     <description>ReadBool("100")</description>
  /// </item>
  /// <item>
  ///     <term>02功能码</term>
  ///     <description>ReadBool("x=2;100")</description>
  /// </item>
  /// <item>
  ///     <term>03功能码</term>
  ///     <description>Read("100")</description>
  /// </item>
  /// <item>
  ///     <term>04功能码</term>
  ///     <description>Read("x=4;100")</description>
  /// </item>
  /// <item>
  ///     <term>05功能码</term>
  ///     <description>Write("100", True)</description>
  /// </item>
  /// <item>
  ///     <term>06功能码</term>
  ///     <description>Write("100", (short)100);Write("100", (ushort)100)</description>
  /// </item>
  /// <item>
  ///     <term>0F功能码</term>
  ///     <description>Write("100", new bool[]{True})</description>
  /// </item>
  /// <item>
  ///     <term>10功能码</term>
  ///     <description>写入寄存器的方法出去上述06功能码的示例，如果写一个short想用10功能码：Write("100", new short[]{100})</description>
  /// </item>
  /// <item>
  ///     <term>16功能码</term>
  ///     <description>Write("100.2", True) 当写入bool值的方法里，地址格式变为字地址时，就使用16功能码，通过掩码的方式来修改寄存器的某一位，需要Modbus服务器支持，否则写入无效。</description>
  /// </item>
  /// </list>
  /// 基本的用法请参照下面的代码示例
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Modbus\Modbus.cs" region="Example1" title="Modbus示例" />
  /// </example>
  public class ModbusTcpNet : NetworkDeviceBase
  {
    private byte station = 1;
    private readonly SoftIncrementCount softIncrementCount;
    private bool isAddressStartWithZero = true;

    /// <summary>
    /// 实例化一个Modbus-Tcp协议的客户端对象<br />
    /// Instantiate a client object of the Modbus-Tcp protocol
    /// </summary>
    public ModbusTcpNet()
    {
      this.softIncrementCount = new SoftIncrementCount((long) ushort.MaxValue);
      this.WordLength = (ushort) 1;
      this.station = (byte) 1;
      this.ByteTransform = (IByteTransform) new ReverseWordTransform();
    }

    /// <summary>
    /// 指定服务器地址，端口号，客户端自己的站号来初始化<br />
    /// Specify the server address, port number, and client's own station number to initialize
    /// </summary>
    /// <param name="ipAddress">服务器的Ip地址</param>
    /// <param name="port">服务器的端口号</param>
    /// <param name="station">客户端自身的站号</param>
    public ModbusTcpNet(string ipAddress, int port = 502, byte station = 1)
    {
      this.softIncrementCount = new SoftIncrementCount((long) ushort.MaxValue);
      this.IpAddress = ipAddress;
      this.Port = port;
      this.WordLength = (ushort) 1;
      this.station = station;
      this.ByteTransform = (IByteTransform) new ReverseWordTransform();
    }

    /// <inheritdoc />
    protected override INetMessage GetNewNetMessage() => (INetMessage) new ModbusTcpMessage();

    /// <inheritdoc />
    protected override OperateResult InitializationOnConnect(Socket socket) => this.isUseAccountCertificate ? this.AccountCertificate(socket) : base.InitializationOnConnect(socket);

    /// <inheritdoc />
    protected override async Task<OperateResult> InitializationOnConnectAsync(
      Socket socket)
    {
      if (this.isUseAccountCertificate)
      {
        OperateResult operateResult = await this.AccountCertificateAsync(socket);
        return operateResult;
      }
      OperateResult operateResult1 = await base.InitializationOnConnectAsync(socket);
      return operateResult1;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadInt32(System.String,System.UInt16)" />
    [EstMqttApi("ReadInt32Array", "")]
    public override OperateResult<int[]> ReadInt32(string address, ushort length)
    {
      IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      return ByteTransformHelper.GetResultFromBytes<int[]>(this.Read(address, (ushort) ((int) length * (int) this.WordLength * 2)), (Func<byte[], int[]>) (m => transform.TransInt32(m, 0, (int) length)));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadUInt32(System.String,System.UInt16)" />
    [EstMqttApi("ReadUInt32Array", "")]
    public override OperateResult<uint[]> ReadUInt32(string address, ushort length)
    {
      IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      return ByteTransformHelper.GetResultFromBytes<uint[]>(this.Read(address, (ushort) ((int) length * (int) this.WordLength * 2)), (Func<byte[], uint[]>) (m => transform.TransUInt32(m, 0, (int) length)));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadFloat(System.String,System.UInt16)" />
    [EstMqttApi("ReadFloatArray", "")]
    public override OperateResult<float[]> ReadFloat(string address, ushort length)
    {
      IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      return ByteTransformHelper.GetResultFromBytes<float[]>(this.Read(address, (ushort) ((int) length * (int) this.WordLength * 2)), (Func<byte[], float[]>) (m => transform.TransSingle(m, 0, (int) length)));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadInt64(System.String,System.UInt16)" />
    [EstMqttApi("ReadInt64Array", "")]
    public override OperateResult<long[]> ReadInt64(string address, ushort length)
    {
      IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      return ByteTransformHelper.GetResultFromBytes<long[]>(this.Read(address, (ushort) ((int) length * (int) this.WordLength * 4)), (Func<byte[], long[]>) (m => transform.TransInt64(m, 0, (int) length)));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadUInt64(System.String,System.UInt16)" />
    [EstMqttApi("ReadUInt64Array", "")]
    public override OperateResult<ulong[]> ReadUInt64(string address, ushort length)
    {
      IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      return ByteTransformHelper.GetResultFromBytes<ulong[]>(this.Read(address, (ushort) ((int) length * (int) this.WordLength * 4)), (Func<byte[], ulong[]>) (m => transform.TransUInt64(m, 0, (int) length)));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadDouble(System.String,System.UInt16)" />
    [EstMqttApi("ReadDoubleArray", "")]
    public override OperateResult<double[]> ReadDouble(string address, ushort length)
    {
      IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      return ByteTransformHelper.GetResultFromBytes<double[]>(this.Read(address, (ushort) ((int) length * (int) this.WordLength * 4)), (Func<byte[], double[]>) (m => transform.TransDouble(m, 0, (int) length)));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.Int32[])" />
    [EstMqttApi("WriteInt32Array", "")]
    public override OperateResult Write(string address, int[] values)
    {
      IByteTransform transformParameter = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      return this.Write(address, transformParameter.TransByte(values));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.UInt32[])" />
    [EstMqttApi("WriteUInt32Array", "")]
    public override OperateResult Write(string address, uint[] values)
    {
      IByteTransform transformParameter = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      return this.Write(address, transformParameter.TransByte(values));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.Single[])" />
    [EstMqttApi("WriteFloatArray", "")]
    public override OperateResult Write(string address, float[] values)
    {
      IByteTransform transformParameter = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      return this.Write(address, transformParameter.TransByte(values));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.Int64[])" />
    [EstMqttApi("WriteInt64Array", "")]
    public override OperateResult Write(string address, long[] values)
    {
      IByteTransform transformParameter = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      return this.Write(address, transformParameter.TransByte(values));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.UInt64[])" />
    [EstMqttApi("WriteUInt64Array", "")]
    public override OperateResult Write(string address, ulong[] values)
    {
      IByteTransform transformParameter = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      return this.Write(address, transformParameter.TransByte(values));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.Double[])" />
    [EstMqttApi("WriteDoubleArray", "")]
    public override OperateResult Write(string address, double[] values)
    {
      IByteTransform transformParameter = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      return this.Write(address, transformParameter.TransByte(values));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadInt32Async(System.String,System.UInt16)" />
    public override async Task<OperateResult<int[]>> ReadInt32Async(
      string address,
      ushort length)
    {
      IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      OperateResult<byte[]> result = await this.ReadAsync(address, (ushort) ((int) length * (int) this.WordLength * 2));
      return ByteTransformHelper.GetResultFromBytes<int[]>(result, (Func<byte[], int[]>) (m => transform.TransInt32(m, 0, (int) length)));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadUInt32Async(System.String,System.UInt16)" />
    public override async Task<OperateResult<uint[]>> ReadUInt32Async(
      string address,
      ushort length)
    {
      IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      OperateResult<byte[]> result = await this.ReadAsync(address, (ushort) ((int) length * (int) this.WordLength * 2));
      return ByteTransformHelper.GetResultFromBytes<uint[]>(result, (Func<byte[], uint[]>) (m => transform.TransUInt32(m, 0, (int) length)));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadFloatAsync(System.String,System.UInt16)" />
    public override async Task<OperateResult<float[]>> ReadFloatAsync(
      string address,
      ushort length)
    {
      IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      OperateResult<byte[]> result = await this.ReadAsync(address, (ushort) ((int) length * (int) this.WordLength * 2));
      return ByteTransformHelper.GetResultFromBytes<float[]>(result, (Func<byte[], float[]>) (m => transform.TransSingle(m, 0, (int) length)));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadInt64Async(System.String,System.UInt16)" />
    public override async Task<OperateResult<long[]>> ReadInt64Async(
      string address,
      ushort length)
    {
      IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      OperateResult<byte[]> result = await this.ReadAsync(address, (ushort) ((int) length * (int) this.WordLength * 4));
      return ByteTransformHelper.GetResultFromBytes<long[]>(result, (Func<byte[], long[]>) (m => transform.TransInt64(m, 0, (int) length)));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadUInt64Async(System.String,System.UInt16)" />
    public override async Task<OperateResult<ulong[]>> ReadUInt64Async(
      string address,
      ushort length)
    {
      IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      OperateResult<byte[]> result = await this.ReadAsync(address, (ushort) ((int) length * (int) this.WordLength * 4));
      return ByteTransformHelper.GetResultFromBytes<ulong[]>(result, (Func<byte[], ulong[]>) (m => transform.TransUInt64(m, 0, (int) length)));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadDoubleAsync(System.String,System.UInt16)" />
    public override async Task<OperateResult<double[]>> ReadDoubleAsync(
      string address,
      ushort length)
    {
      IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      OperateResult<byte[]> result = await this.ReadAsync(address, (ushort) ((int) length * (int) this.WordLength * 4));
      return ByteTransformHelper.GetResultFromBytes<double[]>(result, (Func<byte[], double[]>) (m => transform.TransDouble(m, 0, (int) length)));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteAsync(System.String,System.Int32[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      int[] values)
    {
      IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      OperateResult operateResult = await this.WriteAsync(address, transform.TransByte(values));
      transform = (IByteTransform) null;
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteAsync(System.String,System.UInt32[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      uint[] values)
    {
      IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      OperateResult operateResult = await this.WriteAsync(address, transform.TransByte(values));
      transform = (IByteTransform) null;
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteAsync(System.String,System.Single[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      float[] values)
    {
      IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      OperateResult operateResult = await this.WriteAsync(address, transform.TransByte(values));
      transform = (IByteTransform) null;
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteAsync(System.String,System.Int64[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      long[] values)
    {
      IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      OperateResult operateResult = await this.WriteAsync(address, transform.TransByte(values));
      transform = (IByteTransform) null;
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteAsync(System.String,System.UInt64[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      ulong[] values)
    {
      IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      OperateResult operateResult = await this.WriteAsync(address, transform.TransByte(values));
      transform = (IByteTransform) null;
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteAsync(System.String,System.Double[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      double[] values)
    {
      IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      OperateResult operateResult = await this.WriteAsync(address, transform.TransByte(values));
      transform = (IByteTransform) null;
      return operateResult;
    }

    /// <summary>
    /// 获取或设置起始的地址是否从0开始，默认为True<br />
    /// Gets or sets whether the starting address starts from 0. The default is True
    /// </summary>
    /// <remarks>
    /// <note type="warning">因为有些设备的起始地址是从1开始的，就要设置本属性为<c>False</c></note>
    /// </remarks>
    public bool AddressStartWithZero
    {
      get => this.isAddressStartWithZero;
      set => this.isAddressStartWithZero = value;
    }

    /// <summary>
    /// 获取或者重新修改服务器的默认站号信息，当然，你可以再读写的时候动态指定，参见备注<br />
    /// Get or modify the default station number information of the server. Of course, you can specify it dynamically when reading and writing, see note
    /// </summary>
    /// <remarks>
    /// 当你调用 ReadCoil("100") 时，对应的站号就是本属性的值，当你调用 ReadCoil("s=2;100") 时，就忽略本属性的值，读写寄存器的时候同理
    /// </remarks>
    public byte Station
    {
      get => this.station;
      set => this.station = value;
    }

    /// <inheritdoc cref="P:EstCommunication.Core.ByteTransformBase.DataFormat" />
    public DataFormat DataFormat
    {
      get => this.ByteTransform.DataFormat;
      set => this.ByteTransform.DataFormat = value;
    }

    /// <summary>
    /// 字符串数据是否按照字来反转，默认为False<br />
    /// Whether the string data is reversed according to words. The default is False.
    /// </summary>
    /// <remarks>字符串按照2个字节的排列进行颠倒，根据实际情况进行设置</remarks>
    public bool IsStringReverse
    {
      get => this.ByteTransform.IsStringReverseByteWord;
      set => this.ByteTransform.IsStringReverseByteWord = value;
    }

    /// <summary>
    /// 获取modbus协议自增的消息号，你可以自定义modbus的消息号的规则，详细参见<see cref="T:EstCommunication.ModBus.ModbusTcpNet" />说明，也可以查找<see cref="T:EstCommunication.BasicFramework.SoftIncrementCount" />说明。<br />
    /// Get the message number incremented by the modbus protocol. You can customize the rules of the message number of the modbus. For details, please refer to the description of <see cref="T:EstCommunication.ModBus.ModbusTcpNet" />, or you can find the description of <see cref="T:EstCommunication.BasicFramework.SoftIncrementCount" />
    /// </summary>
    public SoftIncrementCount MessageId => this.softIncrementCount;

    /// <summary>
    /// 读取线圈，需要指定起始地址，如果富文本地址不指定，默认使用的功能码是 0x01<br />
    /// To read the coil, you need to specify the start address. If the rich text address is not specified, the default function code is 0x01.
    /// </summary>
    /// <param name="address">起始地址，格式为"1234"</param>
    /// <returns>带有成功标志的bool对象</returns>
    public OperateResult<bool> ReadCoil(string address) => this.ReadBool(address);

    /// <summary>
    /// 批量的读取线圈，需要指定起始地址，读取长度，如果富文本地址不指定，默认使用的功能码是 0x01<br />
    /// For batch reading coils, you need to specify the start address and read length. If the rich text address is not specified, the default function code is 0x01.
    /// </summary>
    /// <param name="address">起始地址，格式为"1234"</param>
    /// <param name="length">读取长度</param>
    /// <returns>带有成功标志的bool数组对象</returns>
    public OperateResult<bool[]> ReadCoil(string address, ushort length) => this.ReadBool(address, length);

    /// <summary>
    /// 读取输入线圈，需要指定起始地址，如果富文本地址不指定，默认使用的功能码是 0x02<br />
    /// To read the input coil, you need to specify the start address. If the rich text address is not specified, the default function code is 0x02.
    /// </summary>
    /// <param name="address">起始地址，格式为"1234"</param>
    /// <returns>带有成功标志的bool对象</returns>
    public OperateResult<bool> ReadDiscrete(string address) => ByteTransformHelper.GetResultFromArray<bool>(this.ReadDiscrete(address, (ushort) 1));

    /// <summary>
    /// 批量的读取输入点，需要指定起始地址，读取长度，如果富文本地址不指定，默认使用的功能码是 0x02<br />
    /// To read input points in batches, you need to specify the start address and read length. If the rich text address is not specified, the default function code is 0x02
    /// </summary>
    /// <param name="address">起始地址，格式为"1234"</param>
    /// <param name="length">读取长度</param>
    /// <returns>带有成功标志的bool数组对象</returns>
    public OperateResult<bool[]> ReadDiscrete(string address, ushort length) => this.ReadBoolHelper(address, length, (byte) 2);

    /// <summary>
    /// 从Modbus服务器批量读取寄存器的信息，需要指定起始地址，读取长度，如果富文本地址不指定，默认使用的功能码是 0x03，如果需要使用04功能码，那么地址就写成 x=4;100<br />
    /// To read the register information from the Modbus server in batches, you need to specify the start address and read length. If the rich text address is not specified,
    /// the default function code is 0x03. If you need to use the 04 function code, the address is written as x = 4; 100
    /// </summary>
    /// <param name="address">起始地址，比如"100"，"x=4;100"，"s=1;100","s=1;x=4;100"</param>
    /// <param name="length">读取的数量</param>
    /// <returns>带有成功标志的字节信息</returns>
    /// <remarks>富地址格式，支持携带站号信息，功能码信息，具体参照类的示例代码</remarks>
    /// <example>
    /// 此处演示批量读取的示例
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Modbus\Modbus.cs" region="ReadExample1" title="Read示例" />
    /// </example>
    [EstMqttApi("ReadByteArray", "")]
    public override OperateResult<byte[]> Read(string address, ushort length)
    {
      OperateResult<byte[][]> operateResult1 = ModbusInfo.BuildReadModbusCommand(address, length, this.Station, this.AddressStartWithZero, (byte) 3);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult1);
      List<byte> byteList = new List<byte>();
      for (int index = 0; index < operateResult1.Content.Length; ++index)
      {
        OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(ModbusInfo.PackCommandToTcp(operateResult1.Content[index], (ushort) this.softIncrementCount.GetCurrentValue()));
        if (!operateResult2.IsSuccess)
          return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult2);
        OperateResult<byte[]> actualData = ModbusInfo.ExtractActualData(ModbusInfo.ExplodeTcpCommandToCore(operateResult2.Content));
        if (!actualData.IsSuccess)
          return OperateResult.CreateFailedResult<byte[]>((OperateResult) actualData);
        byteList.AddRange((IEnumerable<byte>) actualData.Content);
      }
      return OperateResult.CreateSuccessResult<byte[]>(byteList.ToArray());
    }

    /// <summary>
    /// 将数据写入到Modbus的寄存器上去，需要指定起始地址和数据内容，如果富文本地址不指定，默认使用的功能码是 0x10<br />
    /// To write data to Modbus registers, you need to specify the start address and data content. If the rich text address is not specified, the default function code is 0x10
    /// </summary>
    /// <param name="address">起始地址，比如"100"，"x=4;100"，"s=1;100","s=1;x=4;100"</param>
    /// <param name="value">写入的数据，长度根据data的长度来指示</param>
    /// <returns>返回写入结果</returns>
    /// <remarks>富地址格式，支持携带站号信息，功能码信息，具体参照类的示例代码</remarks>
    /// <example>
    /// 此处演示批量写入的示例
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Modbus\Modbus.cs" region="WriteExample1" title="Write示例" />
    /// </example>
    [EstMqttApi("WriteByteArray", "")]
    public override OperateResult Write(string address, byte[] value)
    {
      OperateResult<byte[]> operateResult1 = ModbusInfo.BuildWriteWordModbusCommand(address, value, this.Station, this.AddressStartWithZero, (byte) 16);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(ModbusInfo.PackCommandToTcp(operateResult1.Content, (ushort) this.softIncrementCount.GetCurrentValue()));
      return !operateResult2.IsSuccess ? (OperateResult) operateResult2 : (OperateResult) ModbusInfo.ExtractActualData(ModbusInfo.ExplodeTcpCommandToCore(operateResult2.Content));
    }

    /// <summary>
    /// 将数据写入到Modbus的单个寄存器上去，需要指定起始地址和数据值，如果富文本地址不指定，默认使用的功能码是 0x06<br />
    /// To write data to a single register of Modbus, you need to specify the start address and data value. If the rich text address is not specified, the default function code is 0x06.
    /// </summary>
    /// <param name="address">起始地址，比如"100"，"x=4;100"，"s=1;100","s=1;x=4;100"</param>
    /// <param name="value">写入的short数据</param>
    /// <returns>是否写入成功</returns>
    [EstMqttApi("WriteInt16", "")]
    public override OperateResult Write(string address, short value)
    {
      OperateResult<byte[]> operateResult1 = ModbusInfo.BuildWriteWordModbusCommand(address, value, this.Station, this.AddressStartWithZero, (byte) 6);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(ModbusInfo.PackCommandToTcp(operateResult1.Content, (ushort) this.softIncrementCount.GetCurrentValue()));
      return !operateResult2.IsSuccess ? (OperateResult) operateResult2 : (OperateResult) ModbusInfo.ExtractActualData(ModbusInfo.ExplodeTcpCommandToCore(operateResult2.Content));
    }

    /// <summary>
    /// 将数据写入到Modbus的单个寄存器上去，需要指定起始地址和数据值，如果富文本地址不指定，默认使用的功能码是 0x06<br />
    /// To write data to a single register of Modbus, you need to specify the start address and data value. If the rich text address is not specified, the default function code is 0x06.
    /// </summary>
    /// <param name="address">起始地址，比如"100"，"x=4;100"，"s=1;100","s=1;x=4;100"</param>
    /// <param name="value">写入的ushort数据</param>
    /// <returns>是否写入成功</returns>
    [EstMqttApi("WriteUInt16", "")]
    public override OperateResult Write(string address, ushort value)
    {
      OperateResult<byte[]> operateResult1 = ModbusInfo.BuildWriteWordModbusCommand(address, value, this.Station, this.AddressStartWithZero, (byte) 6);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(ModbusInfo.PackCommandToTcp(operateResult1.Content, (ushort) this.softIncrementCount.GetCurrentValue()));
      return !operateResult2.IsSuccess ? (OperateResult) operateResult2 : (OperateResult) ModbusInfo.ExtractActualData(ModbusInfo.ExplodeTcpCommandToCore(operateResult2.Content));
    }

    /// <summary>
    /// 向设备写入掩码数据，使用0x16功能码，需要确认对方是否支持相关的操作，掩码数据的操作主要针对寄存器。<br />
    /// To write mask data to the server, using the 0x16 function code, you need to confirm whether the other party supports related operations.
    /// The operation of mask data is mainly directed to the register.
    /// </summary>
    /// <param name="address">起始地址，起始地址，比如"100"，"x=4;100"，"s=1;100","s=1;x=4;100"</param>
    /// <param name="andMask">等待与操作的掩码数据</param>
    /// <param name="orMask">等待或操作的掩码数据</param>
    /// <returns>是否写入成功</returns>
    [EstMqttApi("WriteMask", "")]
    public OperateResult WriteMask(string address, ushort andMask, ushort orMask)
    {
      OperateResult<byte[]> operateResult1 = ModbusInfo.BuildWriteMaskModbusCommand(address, andMask, orMask, this.Station, this.AddressStartWithZero, (byte) 22);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(ModbusInfo.PackCommandToTcp(operateResult1.Content, (ushort) this.softIncrementCount.GetCurrentValue()));
      return !operateResult2.IsSuccess ? (OperateResult) operateResult2 : (OperateResult) ModbusInfo.ExtractActualData(ModbusInfo.ExplodeTcpCommandToCore(operateResult2.Content));
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.Write(System.String,System.Int16)" />
    public OperateResult WriteOneRegister(string address, short value) => this.Write(address, value);

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.Write(System.String,System.UInt16)" />
    public OperateResult WriteOneRegister(string address, ushort value) => this.Write(address, value);

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.ReadCoil(System.String)" />
    public async Task<OperateResult<bool>> ReadCoilAsync(string address)
    {
      OperateResult<bool> operateResult = await this.ReadBoolAsync(address);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.ReadCoil(System.String,System.UInt16)" />
    public async Task<OperateResult<bool[]>> ReadCoilAsync(
      string address,
      ushort length)
    {
      OperateResult<bool[]> operateResult = await this.ReadBoolAsync(address, length);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.ReadDiscrete(System.String)" />
    public async Task<OperateResult<bool>> ReadDiscreteAsync(string address)
    {
      OperateResult<bool[]> result = await this.ReadDiscreteAsync(address, (ushort) 1);
      return ByteTransformHelper.GetResultFromArray<bool>(result);
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.ReadDiscrete(System.String,System.UInt16)" />
    public async Task<OperateResult<bool[]>> ReadDiscreteAsync(
      string address,
      ushort length)
    {
      OperateResult<bool[]> operateResult = await this.ReadBoolHelperAsync(address, length, (byte) 2);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.Read(System.String,System.UInt16)" />
    public override async Task<OperateResult<byte[]>> ReadAsync(
      string address,
      ushort length)
    {
      OperateResult<byte[][]> command = ModbusInfo.BuildReadModbusCommand(address, length, this.Station, this.AddressStartWithZero, (byte) 3);
      if (!command.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) command);
      List<byte> resultArray = new List<byte>();
      for (int i = 0; i < command.Content.Length; ++i)
      {
        OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(ModbusInfo.PackCommandToTcp(command.Content[i], (ushort) this.softIncrementCount.GetCurrentValue()));
        if (!read.IsSuccess)
          return OperateResult.CreateFailedResult<byte[]>((OperateResult) read);
        OperateResult<byte[]> extract = ModbusInfo.ExtractActualData(ModbusInfo.ExplodeTcpCommandToCore(read.Content));
        if (!extract.IsSuccess)
          return OperateResult.CreateFailedResult<byte[]>((OperateResult) extract);
        resultArray.AddRange((IEnumerable<byte>) extract.Content);
        read = (OperateResult<byte[]>) null;
        extract = (OperateResult<byte[]>) null;
      }
      return OperateResult.CreateSuccessResult<byte[]>(resultArray.ToArray());
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.Write(System.String,System.Byte[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      byte[] value)
    {
      OperateResult<byte[]> command = ModbusInfo.BuildWriteWordModbusCommand(address, value, this.Station, this.AddressStartWithZero, (byte) 16);
      if (!command.IsSuccess)
        return (OperateResult) command;
      OperateResult<byte[]> write = await this.ReadFromCoreServerAsync(ModbusInfo.PackCommandToTcp(command.Content, (ushort) this.softIncrementCount.GetCurrentValue()));
      return write.IsSuccess ? (OperateResult) ModbusInfo.ExtractActualData(ModbusInfo.ExplodeTcpCommandToCore(write.Content)) : (OperateResult) write;
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.Write(System.String,System.Int16)" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      short value)
    {
      OperateResult<byte[]> command = ModbusInfo.BuildWriteWordModbusCommand(address, value, this.Station, this.AddressStartWithZero, (byte) 6);
      if (!command.IsSuccess)
        return (OperateResult) command;
      OperateResult<byte[]> write = await this.ReadFromCoreServerAsync(ModbusInfo.PackCommandToTcp(command.Content, (ushort) this.softIncrementCount.GetCurrentValue()));
      return write.IsSuccess ? (OperateResult) ModbusInfo.ExtractActualData(ModbusInfo.ExplodeTcpCommandToCore(write.Content)) : (OperateResult) write;
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.Write(System.String,System.UInt16)" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      ushort value)
    {
      OperateResult<byte[]> command = ModbusInfo.BuildWriteWordModbusCommand(address, value, this.Station, this.AddressStartWithZero, (byte) 6);
      if (!command.IsSuccess)
        return (OperateResult) command;
      OperateResult<byte[]> write = await this.ReadFromCoreServerAsync(ModbusInfo.PackCommandToTcp(command.Content, (ushort) this.softIncrementCount.GetCurrentValue()));
      return write.IsSuccess ? (OperateResult) ModbusInfo.ExtractActualData(ModbusInfo.ExplodeTcpCommandToCore(write.Content)) : (OperateResult) write;
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.WriteMask(System.String,System.UInt16,System.UInt16)" />
    public async Task<OperateResult> WriteMaskAsync(
      string address,
      ushort andMask,
      ushort orMask)
    {
      OperateResult<byte[]> command = ModbusInfo.BuildWriteMaskModbusCommand(address, andMask, orMask, this.Station, this.AddressStartWithZero, (byte) 22);
      if (!command.IsSuccess)
        return (OperateResult) command;
      OperateResult<byte[]> write = await this.ReadFromCoreServerAsync(ModbusInfo.PackCommandToTcp(command.Content, (ushort) this.softIncrementCount.GetCurrentValue()));
      return write.IsSuccess ? (OperateResult) ModbusInfo.ExtractActualData(ModbusInfo.ExplodeTcpCommandToCore(write.Content)) : (OperateResult) write;
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.Write(System.String,System.Int16)" />
    public virtual async Task<OperateResult> WriteOneRegisterAsync(
      string address,
      short value)
    {
      OperateResult operateResult = await this.WriteAsync(address, value);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.Write(System.String,System.UInt16)" />
    public virtual async Task<OperateResult> WriteOneRegisterAsync(
      string address,
      ushort value)
    {
      OperateResult operateResult = await this.WriteAsync(address, value);
      return operateResult;
    }

    private OperateResult<bool[]> ReadBoolHelper(
      string address,
      ushort length,
      byte function)
    {
      OperateResult<byte[][]> operateResult1 = ModbusInfo.BuildReadModbusCommand(address, length, this.Station, this.AddressStartWithZero, function);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult1);
      List<bool> boolList = new List<bool>();
      for (int index = 0; index < operateResult1.Content.Length; ++index)
      {
        OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(ModbusInfo.PackCommandToTcp(operateResult1.Content[index], (ushort) this.softIncrementCount.GetCurrentValue()));
        if (!operateResult2.IsSuccess)
          return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult2);
        OperateResult<byte[]> actualData = ModbusInfo.ExtractActualData(ModbusInfo.ExplodeTcpCommandToCore(operateResult2.Content));
        if (!actualData.IsSuccess)
          return OperateResult.CreateFailedResult<bool[]>((OperateResult) actualData);
        int length1 = (int) operateResult1.Content[index][4] * 256 + (int) operateResult1.Content[index][5];
        boolList.AddRange((IEnumerable<bool>) SoftBasic.ByteToBoolArray(actualData.Content, length1));
      }
      return OperateResult.CreateSuccessResult<bool[]>(boolList.ToArray());
    }

    /// <summary>
    /// 批量读取线圈或是离散的数据信息，需要指定地址和长度，具体的结果取决于实现，如果富文本地址不指定，默认使用的功能码是 0x01<br />
    /// To read coils or discrete data in batches, you need to specify the address and length. The specific result depends on the implementation. If the rich text address is not specified, the default function code is 0x01.
    /// </summary>
    /// <param name="address">数据地址，比如 "1234" </param>
    /// <param name="length">数据长度</param>
    /// <returns>带有成功标识的bool[]数组</returns>
    [EstMqttApi("ReadBoolArray", "")]
    public override OperateResult<bool[]> ReadBool(string address, ushort length) => this.ReadBoolHelper(address, length, (byte) 1);

    /// <summary>
    /// 向线圈中写入bool数组，返回是否写入成功，如果富文本地址不指定，默认使用的功能码是 0x0F<br />
    /// Write the bool array to the coil, and return whether the writing is successful. If the rich text address is not specified, the default function code is 0x0F.
    /// </summary>
    /// <param name="address">要写入的数据地址，比如"1234"</param>
    /// <param name="values">要写入的实际数组</param>
    /// <returns>返回写入结果</returns>
    [EstMqttApi("WriteBoolArray", "")]
    public override OperateResult Write(string address, bool[] values)
    {
      OperateResult<byte[]> operateResult1 = ModbusInfo.BuildWriteBoolModbusCommand(address, values, this.Station, this.AddressStartWithZero, (byte) 15);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(ModbusInfo.PackCommandToTcp(operateResult1.Content, (ushort) this.softIncrementCount.GetCurrentValue()));
      return !operateResult2.IsSuccess ? (OperateResult) operateResult2 : (OperateResult) ModbusInfo.ExtractActualData(ModbusInfo.ExplodeTcpCommandToCore(operateResult2.Content));
    }

    /// <summary>
    /// 向线圈中写入bool数值，返回是否写入成功，如果富文本地址不指定，默认使用的功能码是 0x05，
    /// 如果你的地址为字地址，例如100.2，那么将使用0x16的功能码，通过掩码的方式来修改寄存器的某一位，需要Modbus服务器支持，否则写入无效。<br />
    /// Write bool value to the coil and return whether the writing is successful. If the rich text address is not specified, the default function code is 0x05.
    /// If your address is a word address, such as 100.2, then you will use the function code of 0x16 to modify a bit of the register through a mask.
    /// It needs Modbus server support, otherwise the writing is invalid.
    /// </summary>
    /// <param name="address">要写入的数据地址，比如"12345"</param>
    /// <param name="value">要写入的实际数据</param>
    /// <returns>返回写入结果</returns>
    [EstMqttApi("WriteBool", "")]
    public override OperateResult Write(string address, bool value)
    {
      OperateResult<byte[]> operateResult1 = ModbusInfo.BuildWriteBoolModbusCommand(address, value, this.Station, this.AddressStartWithZero, (byte) 5);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(ModbusInfo.PackCommandToTcp(operateResult1.Content, (ushort) this.softIncrementCount.GetCurrentValue()));
      return !operateResult2.IsSuccess ? (OperateResult) operateResult2 : (OperateResult) ModbusInfo.ExtractActualData(ModbusInfo.ExplodeTcpCommandToCore(operateResult2.Content));
    }

    private async Task<OperateResult<bool[]>> ReadBoolHelperAsync(
      string address,
      ushort length,
      byte function)
    {
      OperateResult<byte[][]> command = ModbusInfo.BuildReadModbusCommand(address, length, this.Station, this.AddressStartWithZero, function);
      if (!command.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) command);
      List<bool> resultArray = new List<bool>();
      for (int i = 0; i < command.Content.Length; ++i)
      {
        OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(ModbusInfo.PackCommandToTcp(command.Content[i], (ushort) this.softIncrementCount.GetCurrentValue()));
        if (!read.IsSuccess)
          return OperateResult.CreateFailedResult<bool[]>((OperateResult) read);
        OperateResult<byte[]> extract = ModbusInfo.ExtractActualData(ModbusInfo.ExplodeTcpCommandToCore(read.Content));
        if (!extract.IsSuccess)
          return OperateResult.CreateFailedResult<bool[]>((OperateResult) extract);
        int bitLength = (int) command.Content[i][4] * 256 + (int) command.Content[i][5];
        resultArray.AddRange((IEnumerable<bool>) SoftBasic.ByteToBoolArray(extract.Content, bitLength));
        read = (OperateResult<byte[]>) null;
        extract = (OperateResult<byte[]>) null;
      }
      return OperateResult.CreateSuccessResult<bool[]>(resultArray.ToArray());
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.ReadBool(System.String,System.UInt16)" />
    public override async Task<OperateResult<bool[]>> ReadBoolAsync(
      string address,
      ushort length)
    {
      OperateResult<bool[]> operateResult = await this.ReadBoolHelperAsync(address, length, (byte) 1);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.Write(System.String,System.Boolean[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      bool[] values)
    {
      OperateResult<byte[]> command = ModbusInfo.BuildWriteBoolModbusCommand(address, values, this.Station, this.AddressStartWithZero, (byte) 15);
      if (!command.IsSuccess)
        return (OperateResult) command;
      OperateResult<byte[]> write = await this.ReadFromCoreServerAsync(ModbusInfo.PackCommandToTcp(command.Content, (ushort) this.softIncrementCount.GetCurrentValue()));
      return write.IsSuccess ? (OperateResult) ModbusInfo.ExtractActualData(ModbusInfo.ExplodeTcpCommandToCore(write.Content)) : (OperateResult) write;
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.Write(System.String,System.Boolean)" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      bool value)
    {
      OperateResult<byte[]> command = ModbusInfo.BuildWriteBoolModbusCommand(address, value, this.Station, this.AddressStartWithZero, (byte) 5);
      if (!command.IsSuccess)
        return (OperateResult) command;
      OperateResult<byte[]> write = await this.ReadFromCoreServerAsync(ModbusInfo.PackCommandToTcp(command.Content, (ushort) this.softIncrementCount.GetCurrentValue()));
      return write.IsSuccess ? (OperateResult) ModbusInfo.ExtractActualData(ModbusInfo.ExplodeTcpCommandToCore(write.Content)) : (OperateResult) write;
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("ModbusTcpNet[{0}:{1}]", (object) this.IpAddress, (object) this.Port);
  }
}
