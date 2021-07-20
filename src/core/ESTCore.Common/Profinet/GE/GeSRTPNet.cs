// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.GE.GeSRTPNet
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;
using ESTCore.Common.Core;
using ESTCore.Common.Core.Address;
using ESTCore.Common.Core.IMessage;
using ESTCore.Common.Core.Net;
using ESTCore.Common.Reflection;

using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ESTCore.Common.Profinet.GE
{
    /// <summary>
    /// Ge(通用电气)集团的SRTP协议实现的客户端，支持 I,Q,M,T,SA,SB,SC,S,G 的位和字节读写，支持 AI,AQ,R 的字读写操作，支持读取PLC时间，程序名操作<br />
    /// The client implemented by Ge (General Electric) Group's SRTP protocol, supports I, Q, M, T, SA, SB, SC, S, G bit and byte read and write,
    /// supports AI, AQ, R word read and write Operation, support reading PLC time, program name operation
    /// </summary>
    /// <remarks>
    /// PLC的端口号默认18245，其中读取R，AI，AQ寄存器的原始字节时，传入的长度参数为字节长度。<br />
    /// 对其他寄存器而言，M1-M8的位读取，相当于 M1的字节读取。写入也是同理。
    /// </remarks>
    /// <example>
    /// 地址支持的列表如下：
    /// <list type="table">
    ///   <listheader>
    ///     <term>地址名称</term>
    ///     <term>地址代号</term>
    ///     <term>示例</term>
    ///     <term>地址进制</term>
    ///     <term>位操作</term>
    ///     <term>字节操作</term>
    ///     <term>备注</term>
    ///   </listheader>
    ///   <item>
    ///     <term>Discrete Inputs</term>
    ///     <term>I</term>
    ///     <term>I1,I100</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term>注意：读位和字节，属于不同的地址</term>
    ///   </item>
    ///   <item>
    ///     <term>Discrete Outputs</term>
    ///     <term>Q</term>
    ///     <term>Q1,Q100</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term>注意：读位和字节，属于不同的地址</term>
    ///   </item>
    ///   <item>
    ///     <term>Discrete Internals</term>
    ///     <term>M</term>
    ///     <term>M1,M100</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term>注意：读位和字节，属于不同的地址</term>
    ///   </item>
    ///   <item>
    ///     <term>Discrete Temporaries</term>
    ///     <term>T</term>
    ///     <term>T1,T100</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term>注意：读位和字节，属于不同的地址</term>
    ///   </item>
    ///   <item>
    ///     <term>SA Discrete</term>
    ///     <term>SA</term>
    ///     <term>SA1,SA100</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term>注意：读位和字节，属于不同的地址</term>
    ///   </item>
    ///   <item>
    ///     <term>SB Discrete</term>
    ///     <term>SB</term>
    ///     <term>SB1,SB100</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term>注意：读位和字节，属于不同的地址</term>
    ///   </item>
    ///   <item>
    ///     <term>SC Discrete</term>
    ///     <term>SC</term>
    ///     <term>SC1,SC100</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term>注意：读位和字节，属于不同的地址</term>
    ///   </item>
    ///   <item>
    ///     <term>S Discrete</term>
    ///     <term>S</term>
    ///     <term>S1,S100</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term>注意：读位和字节，属于不同的地址</term>
    ///   </item>
    ///   <item>
    ///     <term>Genius Global Data</term>
    ///     <term>G</term>
    ///     <term>G1,G100</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term>注意：读位和字节，属于不同的地址</term>
    ///   </item>
    ///   <item>
    ///     <term>Analog Inputs </term>
    ///     <term>AI</term>
    ///     <term>AI1,AI100</term>
    ///     <term>10</term>
    ///     <term>×</term>
    ///     <term>√</term>
    ///     <term>虽然读取的时候，长度是字节，但是实际是字单位的，所以不支持 ReadByte</term>
    ///   </item>
    ///   <item>
    ///     <term>Analog Outputs </term>
    ///     <term>QI</term>
    ///     <term>QI1,QI100</term>
    ///     <term>10</term>
    ///     <term>×</term>
    ///     <term>√</term>
    ///     <term>虽然读取的时候，长度是字节，但是实际是字单位的，所以不支持 ReadByte</term>
    ///   </item>
    ///   <item>
    ///     <term>Registers </term>
    ///     <term>R</term>
    ///     <term>R1,R100</term>
    ///     <term>10</term>
    ///     <term>×</term>
    ///     <term>√</term>
    ///     <term>虽然读取的时候，长度是字节，但是实际是字单位的，所以不支持 ReadByte</term>
    ///   </item>
    /// </list>
    /// <note type="important">对于 AI,AQ,R 类型的数据来说，底层是以字为单位的，但是调用 <see cref="M:ESTCore.Common.Profinet.GE.GeSRTPNet.Read(System.String,System.UInt16)" /> 来读取原始数据时，需要传入2被的字长度，对于其他方法不影响</note>
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\GeSRTPNetSample.cs" region="Usage" title="简单的短连接使用" />
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\GeSRTPNetSample.cs" region="Usage2" title="简单的长连接使用" />
    /// 
    /// 假设起始地址为R1，R1存储了温度，100.6℃值为1006，R2存储了压力，1.23Mpa值为123，R3-R4存储了产量计数，读取如下：
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\GeSRTPNetSample.cs" region="ReadExample2" title="Read示例" />
    /// 以下是读取不同类型数据的示例
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\GeSRTPNetSample.cs" region="ReadExample1" title="Read示例" />
    /// </example>
    public class GeSRTPNet : NetworkDeviceBase
    {
        private SoftIncrementCount incrementCount = new SoftIncrementCount((long)ushort.MaxValue);

        /// <summary>
        /// 实例化一个默认的对象<br />
        /// Instantiate a default object
        /// </summary>
        public GeSRTPNet()
        {
            this.ByteTransform = (IByteTransform)new RegularByteTransform();
            this.WordLength = (ushort)2;
        }

        /// <summary>
        /// 指定IP地址和端口号来实例化一个对象<br />
        /// Specify the IP address and port number to instantiate an object
        /// </summary>
        /// <param name="ipAddress">Ip地址</param>
        /// <param name="port">端口号</param>
        public GeSRTPNet(string ipAddress, int port = 18245)
          : this()
        {
            this.IpAddress = ipAddress;
            this.Port = port;
        }

        /// <inheritdoc />
        protected override INetMessage GetNewNetMessage() => (INetMessage)new GeSRTPMessage();

        /// <inheritdoc />
        protected override OperateResult InitializationOnConnect(Socket socket)
        {
            OperateResult<byte[]> operateResult = this.ReadFromCoreServer(socket, new byte[56]);
            return !operateResult.IsSuccess ? (OperateResult)operateResult : OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc />
        protected override async Task<OperateResult> InitializationOnConnectAsync(
          Socket socket)
        {
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(socket, new byte[56]);
            OperateResult operateResult = read.IsSuccess ? OperateResult.CreateSuccessResult() : (OperateResult)read;
            read = (OperateResult<byte[]>)null;
            return operateResult;
        }

        /// <summary>
        /// 批量读取字节数组信息，需要指定地址和长度，返回原始的字节数组，支持 I,Q,M,T,SA,SB,SC,S,G 的位和字节读写，支持 AI,AQ,R 的字读写操作，地址示例：R1,M1<br />
        /// Batch read byte array information, need to specify the address and length, return the original byte array.
        /// Support I, Q, M, T, SA, SB, SC, S, G bit and byte read and write, support AI, AQ, R word read and write operations, address examples: R1, M1
        /// </summary>
        /// <remarks>
        /// 其中读取R，AI，AQ寄存器的原始字节时，传入的长度参数为字节长度。长度为10，返回10个字节数组信息，如果返回长度不满6个字节的，一律返回6个字节的数据
        /// </remarks>
        /// <param name="address">数据地址</param>
        /// <param name="length">数据长度</param>
        /// <returns>带有成功标识的byte[]数组</returns>
        [EstMqttApi("ReadByteArray", "")]
        public override OperateResult<byte[]> Read(string address, ushort length)
        {
            OperateResult<byte[]> operateResult1 = GeHelper.BuildReadCommand(this.incrementCount.GetCurrentValue(), address, length, false);
            if (!operateResult1.IsSuccess)
                return operateResult1;
            OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
            return !operateResult2.IsSuccess ? operateResult2 : GeHelper.ExtraResponseContent(operateResult2.Content);
        }

        /// <inheritdoc />
        [EstMqttApi("WriteByteArray", "")]
        public override OperateResult Write(string address, byte[] value)
        {
            OperateResult<byte[]> operateResult1 = GeHelper.BuildWriteCommand(this.incrementCount.GetCurrentValue(), address, value);
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
            return !operateResult2.IsSuccess ? (OperateResult)operateResult2 : (OperateResult)GeHelper.ExtraResponseContent(operateResult2.Content);
        }

        /// <summary>
        /// 根据指定的地址来读取一个字节的数据，按照字节为单位，例如 M1 字节，就是指 M1-M8 位组成的字节，M2 字节就是 M9-M16 组成的字节。不支持对 AI,AQ,R 寄存器的字节读取<br />
        /// A byte of data is read according to the specified address, and the unit is byte. For example, M1 byte refers to a byte composed of M1-M8 bits,
        /// and M2 byte is a byte composed of M9-M16. Does not support byte reading of AI, AQ, R registers
        /// </summary>
        /// <param name="address">地址信息</param>
        /// <returns>带有成功标识的 <see cref="T:System.Byte" /> 数据</returns>
        [EstMqttApi("ReadByte", "")]
        public OperateResult<byte> ReadByte(string address)
        {
            OperateResult<GeSRTPAddress> from = GeSRTPAddress.ParseFrom(address, (ushort)1, true);
            if (!from.IsSuccess)
                return OperateResult.CreateFailedResult<byte>((OperateResult)from);
            return from.Content.DataCode == (byte)10 || from.Content.DataCode == (byte)12 || from.Content.DataCode == (byte)8 ? new OperateResult<byte>(StringResources.Language.GeSRTPNotSupportByteReadWrite) : ByteTransformHelper.GetResultFromArray<byte>(this.Read(address, (ushort)1));
        }

        /// <summary>
        /// 向PLC中写入byte数据，返回是否写入成功<br />
        /// Write byte data to PLC and return whether the writing is successful
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 -&gt; Starting address, formatted as I100,mM100,Q100,DB20.100</param>
        /// <param name="value">byte数据 -&gt; Byte data</param>
        /// <returns>是否写入成功的结果对象 -&gt; Whether to write a successful result object</returns>
        [EstMqttApi("WriteByte", "")]
        public OperateResult Write(string address, byte value)
        {
            OperateResult<GeSRTPAddress> from = GeSRTPAddress.ParseFrom(address, (ushort)1, true);
            if (!from.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<byte>((OperateResult)from);
            if (from.Content.DataCode == (byte)10 || from.Content.DataCode == (byte)12 || from.Content.DataCode == (byte)8)
                return (OperateResult)new OperateResult<byte>(StringResources.Language.GeSRTPNotSupportByteReadWrite);
            return this.Write(address, new byte[1] { value });
        }

        /// <summary>
        /// 按照位为单位，批量从指定的地址里读取 bool 数组数据，不支持 AI，AQ，R 地址类型，地址比如从1开始，例如 I1,Q1,M1,T1,SA1,SB1,SC1,S1,G1<br />
        /// In units of bits, read bool array data from the specified address in batches. AI, AQ,
        /// R address types are not supported. For example, the address starts from 1, such as I1, Q1, M1, T1, SA1, SB1, SC1, S1, G1
        /// </summary>
        /// <param name="address">PLC的地址信息，例如 M1, G1</param>
        /// <param name="length">读取的长度信息，按照位为单位</param>
        /// <returns>包含是否读取成功的bool数组</returns>
        [EstMqttApi("ReadBoolArray", "")]
        public override OperateResult<bool[]> ReadBool(string address, ushort length)
        {
            OperateResult<GeSRTPAddress> from = GeSRTPAddress.ParseFrom(address, length, true);
            if (!from.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)from);
            OperateResult<byte[]> operateResult1 = GeHelper.BuildReadCommand(this.incrementCount.GetCurrentValue(), from.Content);
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult1);
            OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
            if (!operateResult2.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult2);
            OperateResult<byte[]> operateResult3 = GeHelper.ExtraResponseContent(operateResult2.Content);
            return !operateResult3.IsSuccess ? OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult3) : OperateResult.CreateSuccessResult<bool[]>(operateResult3.Content.ToBoolArray().SelectMiddle<bool>(from.Content.AddressStart % 8, (int)length));
        }

        /// <summary>
        /// 按照位为单位，批量写入 bool 数组到指定的地址里，不支持 AI，AQ，R 地址类型，地址比如从1开始，例如 I1,Q1,M1,T1,SA1,SB1,SC1,S1,G1<br />
        /// In units of bits, write bool arrays in batches to the specified addresses. AI, AQ, and R address types are not supported. For example,
        /// the address starts from 1, such as I1, Q1, M1, T1, SA1, SB1, SC1, S1, G1
        /// </summary>
        /// <param name="address">PLC的地址信息，例如 M1, G1</param>
        /// <param name="value">bool 数组</param>
        /// <returns>是否写入成功的结果对象</returns>
        [EstMqttApi(ApiTopic = "WriteBoolArray", Description = "In units of bits, write bool arrays in batches to the specified addresses")]
        public override OperateResult Write(string address, bool[] value)
        {
            OperateResult<byte[]> operateResult1 = GeHelper.BuildWriteCommand(this.incrementCount.GetCurrentValue(), address, value);
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
            return !operateResult2.IsSuccess ? (OperateResult)operateResult2 : (OperateResult)GeHelper.ExtraResponseContent(operateResult2.Content);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.GE.GeSRTPNet.Read(System.String,System.UInt16)" />
        public override async Task<OperateResult<byte[]>> ReadAsync(
          string address,
          ushort length)
        {
            OperateResult<byte[]> build = GeHelper.BuildReadCommand(this.incrementCount.GetCurrentValue(), address, length, false);
            if (!build.IsSuccess)
                return build;
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(build.Content);
            return read.IsSuccess ? GeHelper.ExtraResponseContent(read.Content) : read;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.GE.GeSRTPNet.Write(System.String,System.Byte[])" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          byte[] value)
        {
            OperateResult<byte[]> build = GeHelper.BuildWriteCommand(this.incrementCount.GetCurrentValue(), address, value);
            if (!build.IsSuccess)
                return (OperateResult)build;
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(build.Content);
            return read.IsSuccess ? (OperateResult)GeHelper.ExtraResponseContent(read.Content) : (OperateResult)read;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.GE.GeSRTPNet.ReadByte(System.String)" />
        public async Task<OperateResult<byte>> ReadByteAsync(string address)
        {
            OperateResult<GeSRTPAddress> analysis = GeSRTPAddress.ParseFrom(address, (ushort)1, true);
            if (!analysis.IsSuccess)
                return OperateResult.CreateFailedResult<byte>((OperateResult)analysis);
            if (analysis.Content.DataCode == (byte)10 || analysis.Content.DataCode == (byte)12 || analysis.Content.DataCode == (byte)8)
                return new OperateResult<byte>(StringResources.Language.GeSRTPNotSupportByteReadWrite);
            OperateResult<byte[]> result = await this.ReadAsync(address, (ushort)1);
            return ByteTransformHelper.GetResultFromArray<byte>(result);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.GE.GeSRTPNet.Write(System.String,System.Byte)" />
        public async Task<OperateResult> WriteAsync(string address, byte value)
        {
            OperateResult<GeSRTPAddress> analysis = GeSRTPAddress.ParseFrom(address, (ushort)1, true);
            if (!analysis.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<byte>((OperateResult)analysis);
            if (analysis.Content.DataCode == (byte)10 || analysis.Content.DataCode == (byte)12 || analysis.Content.DataCode == (byte)8)
                return (OperateResult)new OperateResult<byte>(StringResources.Language.GeSRTPNotSupportByteReadWrite);
            OperateResult operateResult = await this.WriteAsync(address, new byte[1]
            {
        value
            });
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.GE.GeSRTPNet.ReadBool(System.String,System.UInt16)" />
        public override async Task<OperateResult<bool[]>> ReadBoolAsync(
          string address,
          ushort length)
        {
            OperateResult<GeSRTPAddress> analysis = GeSRTPAddress.ParseFrom(address, length, true);
            if (!analysis.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)analysis);
            OperateResult<byte[]> build = GeHelper.BuildReadCommand(this.incrementCount.GetCurrentValue(), analysis.Content);
            if (!build.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)build);
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(build.Content);
            if (!read.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)read);
            OperateResult<byte[]> extra = GeHelper.ExtraResponseContent(read.Content);
            return extra.IsSuccess ? OperateResult.CreateSuccessResult<bool[]>(extra.Content.ToBoolArray().SelectMiddle<bool>(analysis.Content.AddressStart % 8, (int)length)) : OperateResult.CreateFailedResult<bool[]>((OperateResult)extra);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.GE.GeSRTPNet.Write(System.String,System.Boolean[])" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          bool[] value)
        {
            OperateResult<byte[]> build = GeHelper.BuildWriteCommand(this.incrementCount.GetCurrentValue(), address, value);
            if (!build.IsSuccess)
                return (OperateResult)build;
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(build.Content);
            return read.IsSuccess ? (OperateResult)GeHelper.ExtraResponseContent(read.Content) : (OperateResult)read;
        }

        /// <summary>
        /// <b>[商业授权]</b> 读取PLC当前的时间，这个时间可能是不包含时区的，需要自己转换成本地的时间。<br />
        /// <b>[Authorization]</b> Read the current time of the PLC, this time may not include the time zone, you need to convert the local time yourself.
        /// </summary>
        /// <returns>包含是否成功的时间信息</returns>
        [EstMqttApi(Description = "Read the current time of the PLC")]
        public OperateResult<DateTime> ReadPLCTime()
        {
            OperateResult<byte[]> operateResult1 = GeHelper.BuildReadCoreCommand(this.incrementCount.GetCurrentValue(), (byte)37, new byte[5]
            {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 2,
        (byte) 0
            });
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<DateTime>((OperateResult)operateResult1);
            OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
            if (!operateResult2.IsSuccess)
                return OperateResult.CreateFailedResult<DateTime>((OperateResult)operateResult2);
            OperateResult<byte[]> operateResult3 = GeHelper.ExtraResponseContent(operateResult2.Content);
            return !operateResult3.IsSuccess ? OperateResult.CreateFailedResult<DateTime>((OperateResult)operateResult3) : GeHelper.ExtraDateTime(operateResult3.Content);
        }

        /// <summary>
        /// <b>[商业授权]</b> 读取PLC当前的程序的名称<br />
        /// <b>[Authorization]</b> Read the name of the current program of the PLC
        /// </summary>
        /// <returns>包含是否成的程序名称信息</returns>
        [EstMqttApi(Description = "Read the name of the current program of the PLC")]
        public OperateResult<string> ReadProgramName()
        {
            OperateResult<byte[]> operateResult1 = GeHelper.BuildReadCoreCommand(this.incrementCount.GetCurrentValue(), (byte)1, new byte[5]
            {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 2,
        (byte) 0
            });
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<string>((OperateResult)operateResult1);
            OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
            if (!operateResult2.IsSuccess)
                return OperateResult.CreateFailedResult<string>((OperateResult)operateResult2);
            OperateResult<byte[]> operateResult3 = GeHelper.ExtraResponseContent(operateResult2.Content);
            return !operateResult3.IsSuccess ? OperateResult.CreateFailedResult<string>((OperateResult)operateResult3) : GeHelper.ExtraProgramName(operateResult3.Content);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.GE.GeSRTPNet.ReadPLCTime" />
        public async Task<OperateResult<DateTime>> ReadPLCTimeAsync()
        {
            OperateResult<byte[]> build = GeHelper.BuildReadCoreCommand(this.incrementCount.GetCurrentValue(), (byte)37, new byte[5]
            {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 2,
        (byte) 0
            });
            if (!build.IsSuccess)
                return OperateResult.CreateFailedResult<DateTime>((OperateResult)build);
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(build.Content);
            if (!read.IsSuccess)
                return OperateResult.CreateFailedResult<DateTime>((OperateResult)read);
            OperateResult<byte[]> extra = GeHelper.ExtraResponseContent(read.Content);
            return extra.IsSuccess ? GeHelper.ExtraDateTime(extra.Content) : OperateResult.CreateFailedResult<DateTime>((OperateResult)extra);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.GE.GeSRTPNet.ReadProgramName" />
        public async Task<OperateResult<string>> ReadProgramNameAsync()
        {
            OperateResult<byte[]> build = GeHelper.BuildReadCoreCommand(this.incrementCount.GetCurrentValue(), (byte)1, new byte[5]
            {
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 2,
        (byte) 0
            });
            if (!build.IsSuccess)
                return OperateResult.CreateFailedResult<string>((OperateResult)build);
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(build.Content);
            if (!read.IsSuccess)
                return OperateResult.CreateFailedResult<string>((OperateResult)read);
            OperateResult<byte[]> extra = GeHelper.ExtraResponseContent(read.Content);
            return extra.IsSuccess ? GeHelper.ExtraProgramName(extra.Content) : OperateResult.CreateFailedResult<string>((OperateResult)extra);
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("GeSRTPNet[{0}:{1}]", (object)this.IpAddress, (object)this.Port);
    }
}
