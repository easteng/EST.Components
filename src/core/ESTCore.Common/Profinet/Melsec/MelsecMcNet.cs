// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Melsec.MelsecMcNet
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Core;
using ESTCore.Common.Core.Address;
using ESTCore.Common.Core.IMessage;
using ESTCore.Common.Core.Net;
using ESTCore.Common.Reflection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ESTCore.Common.Profinet.Melsec
{
    /// <summary>
    /// 三菱PLC通讯类，采用Qna兼容3E帧协议实现，需要在PLC侧先的以太网模块先进行配置，必须为二进制通讯<br />
    /// Mitsubishi PLC communication class is implemented using Qna compatible 3E frame protocol.
    /// The Ethernet module on the PLC side needs to be configured first. It must be binary communication.
    /// </summary>
    /// <remarks>
    /// 目前组件测试通过的PLC型号列表，有些来自于网友的测试
    /// <list type="number">
    /// <item>Q06UDV PLC  感谢hwdq0012</item>
    /// <item>fx5u PLC  感谢山楂</item>
    /// <item>Q02CPU PLC </item>
    /// <item>L02CPU PLC </item>
    /// </list>
    /// 地址的输入的格式支持多种复杂的地址表示方式：
    /// <list type="number">
    /// <item>[商业授权] 扩展的数据地址: 表示为 ext=1;W100  访问扩展区域为1的W100的地址信息</item>
    /// <item>[商业授权] 缓冲存储器地址: 表示为 mem=32  访问地址为32的本站缓冲存储器地址</item>
    /// <item>[商业授权] 智能模块地址：表示为 module=3;4106  访问模块号3，偏移地址是4106的数据，偏移地址需要根据模块的详细信息来确认。</item>
    /// <item>[商业授权] 基于标签的地址: 表示位 s=AAA  假如标签的名称为AAA，但是标签的读取是有条件的，详细参照<see cref="M:ESTCore.Common.Profinet.Melsec.MelsecMcNet.ReadTags(System.String,System.UInt16)" /></item>
    /// <item>普通的数据地址，参照下面的信息</item>
    /// </list>
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
    ///     <term>内部继电器</term>
    ///     <term>M</term>
    ///     <term>M100,M200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>输入继电器</term>
    ///     <term>X</term>
    ///     <term>X100,X1A0</term>
    ///     <term>16</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>输出继电器</term>
    ///     <term>Y</term>
    ///     <term>Y100,Y1A0</term>
    ///     <term>16</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///    <item>
    ///     <term>锁存继电器</term>
    ///     <term>L</term>
    ///     <term>L100,L200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>报警器</term>
    ///     <term>F</term>
    ///     <term>F100,F200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>边沿继电器</term>
    ///     <term>V</term>
    ///     <term>V100,V200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>链接继电器</term>
    ///     <term>B</term>
    ///     <term>B100,B1A0</term>
    ///     <term>16</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>步进继电器</term>
    ///     <term>S</term>
    ///     <term>S100,S200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>数据寄存器</term>
    ///     <term>D</term>
    ///     <term>D1000,D2000</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>×</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>链接寄存器</term>
    ///     <term>W</term>
    ///     <term>W100,W1A0</term>
    ///     <term>16</term>
    ///     <term>√</term>
    ///     <term>×</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>文件寄存器</term>
    ///     <term>R</term>
    ///     <term>R100,R200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>×</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>ZR文件寄存器</term>
    ///     <term>ZR</term>
    ///     <term>ZR100,ZR2A0</term>
    ///     <term>16</term>
    ///     <term>√</term>
    ///     <term>×</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>变址寄存器</term>
    ///     <term>Z</term>
    ///     <term>Z100,Z200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>×</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>定时器的触点</term>
    ///     <term>TS</term>
    ///     <term>TS100,TS200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>定时器的线圈</term>
    ///     <term>TC</term>
    ///     <term>TC100,TC200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>定时器的当前值</term>
    ///     <term>TN</term>
    ///     <term>TN100,TN200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>×</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>累计定时器的触点</term>
    ///     <term>SS</term>
    ///     <term>SS100,SS200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>累计定时器的线圈</term>
    ///     <term>SC</term>
    ///     <term>SC100,SC200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>累计定时器的当前值</term>
    ///     <term>SN</term>
    ///     <term>SN100,SN200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>×</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>计数器的触点</term>
    ///     <term>CS</term>
    ///     <term>CS100,CS200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>计数器的线圈</term>
    ///     <term>CC</term>
    ///     <term>CC100,CC200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>计数器的当前值</term>
    ///     <term>CN</term>
    ///     <term>CN100,CN200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>×</term>
    ///     <term></term>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\melsecTest.cs" region="Usage" title="简单的短连接使用" />
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\melsecTest.cs" region="Usage2" title="简单的长连接使用" />
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\melsecTest.cs" region="ReadExample1" title="基本的读取示例" />
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\melsecTest.cs" region="ReadExample2" title="批量读取示例" />
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\melsecTest.cs" region="ReadExample3" title="随机字读取示例" />
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\melsecTest.cs" region="ReadExample4" title="随机批量字读取示例" />
    /// </example>
    public class MelsecMcNet : NetworkDeviceBase
    {
        /// <summary>
        /// 实例化三菱的Qna兼容3E帧协议的通讯对象<br />
        /// Instantiate the communication object of Mitsubishi's Qna compatible 3E frame protocol
        /// </summary>
        public MelsecMcNet()
        {
            this.WordLength = (ushort)1;
            this.ByteTransform = (IByteTransform)new RegularByteTransform();
        }

        /// <summary>
        /// 指定ip地址和端口号来实例化一个默认的对象<br />
        /// Specify the IP address and port number to instantiate a default object
        /// </summary>
        /// <param name="ipAddress">PLC的Ip地址</param>
        /// <param name="port">PLC的端口</param>
        public MelsecMcNet(string ipAddress, int port)
        {
            this.WordLength = (ushort)1;
            this.IpAddress = ipAddress;
            this.Port = port;
            this.ByteTransform = (IByteTransform)new RegularByteTransform();
        }

        /// <inheritdoc />
        protected override INetMessage GetNewNetMessage() => (INetMessage)new MelsecQnA3EBinaryMessage();

        /// <summary>
        /// 网络号，通常为0<br />
        /// Network number, usually 0
        /// </summary>
        /// <remarks>
        /// 依据PLC的配置而配置，如果PLC配置了1，那么此处也填0，如果PLC配置了2，此处就填2，测试不通的话，继续测试0
        /// </remarks>
        public byte NetworkNumber { get; set; } = 0;

        /// <summary>
        /// 网络站号，通常为0<br />
        /// Network station number, usually 0
        /// </summary>
        /// <remarks>
        /// 依据PLC的配置而配置，如果PLC配置了1，那么此处也填0，如果PLC配置了2，此处就填2，测试不通的话，继续测试0
        /// </remarks>
        public byte NetworkStationNumber { get; set; } = 0;

        /// <summary>
        /// 当前MC协议的分析地址的方法，对传入的字符串格式的地址进行数据解析。<br />
        /// The current MC protocol's address analysis method performs data parsing on the address of the incoming string format.
        /// </summary>
        /// <param name="address">地址信息</param>
        /// <param name="length">数据长度</param>
        /// <returns>解析后的数据信息</returns>
        protected virtual OperateResult<McAddressData> McAnalysisAddress(
          string address,
          ushort length)
        {
            return McAddressData.ParseMelsecFrom(address, length);
        }

        /// <inheritdoc />
        [EstMqttApi("ReadByteArray", "")]
        public override OperateResult<byte[]> Read(string address, ushort length)
        {
            if (address.StartsWith("s=") || address.StartsWith("S="))
                return this.ReadTags(address.Substring(2), length);
            if (Regex.IsMatch(address, "ext=[0-9]+;", RegexOptions.IgnoreCase))
            {
                string input = Regex.Match(address, "ext=[0-9]+;").Value;
                return this.ReadExtend(ushort.Parse(Regex.Match(input, "[0-9]+").Value), address.Substring(input.Length), length);
            }
            if (Regex.IsMatch(address, "mem=", RegexOptions.IgnoreCase))
                return this.ReadMemory(address.Substring(4), length);
            if (Regex.IsMatch(address, "module=[0-9]+;", RegexOptions.IgnoreCase))
            {
                string input = Regex.Match(address, "module=[0-9]+;").Value;
                return this.ReadSmartModule(ushort.Parse(Regex.Match(input, "[0-9]+").Value), address.Substring(input.Length), (ushort)((uint)length * 2U));
            }
            OperateResult<McAddressData> operateResult1 = this.McAnalysisAddress(address, length);
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult1);
            List<byte> byteList = new List<byte>();
            ushort num1 = 0;
            while ((int)num1 < (int)length)
            {
                ushort num2 = (ushort)Math.Min((int)length - (int)num1, 900);
                operateResult1.Content.Length = num2;
                OperateResult<byte[]> operateResult2 = this.ReadAddressData(operateResult1.Content);
                if (!operateResult2.IsSuccess)
                    return operateResult2;
                byteList.AddRange((IEnumerable<byte>)operateResult2.Content);
                num1 += num2;
                if (operateResult1.Content.McDataType.DataType == (byte)0)
                    operateResult1.Content.AddressStart += (int)num2;
                else
                    operateResult1.Content.AddressStart += (int)num2 * 16;
            }
            return OperateResult.CreateSuccessResult<byte[]>(byteList.ToArray());
        }

        private OperateResult<byte[]> ReadAddressData(McAddressData addressData)
        {
            OperateResult<byte[]> operateResult = this.ReadFromCoreServer(MelsecMcNet.PackMcCommand(MelsecHelper.BuildReadMcCoreCommand(addressData, false), this.NetworkNumber, this.NetworkStationNumber));
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult);
            OperateResult result = MelsecMcNet.CheckResponseContent(operateResult.Content);
            return !result.IsSuccess ? OperateResult.CreateFailedResult<byte[]>(result) : MelsecMcNet.ExtractActualData(operateResult.Content.RemoveBegin<byte>(11), false);
        }

        /// <inheritdoc />
        [EstMqttApi("WriteByteArray", "")]
        public override OperateResult Write(string address, byte[] value)
        {
            OperateResult<McAddressData> operateResult = this.McAnalysisAddress(address, (ushort)0);
            return !operateResult.IsSuccess ? (OperateResult)OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult) : this.WriteAddressData(operateResult.Content, value);
        }

        private OperateResult WriteAddressData(McAddressData addressData, byte[] value)
        {
            OperateResult<byte[]> operateResult = this.ReadFromCoreServer(MelsecMcNet.PackMcCommand(MelsecHelper.BuildWriteWordCoreCommand(addressData, value), this.NetworkNumber, this.NetworkStationNumber));
            if (!operateResult.IsSuccess)
                return (OperateResult)operateResult;
            OperateResult result = MelsecMcNet.CheckResponseContent(operateResult.Content);
            return !result.IsSuccess ? (OperateResult)OperateResult.CreateFailedResult<byte[]>(result) : OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc />
        public override async Task<OperateResult<byte[]>> ReadAsync(
          string address,
          ushort length)
        {
            if (address.StartsWith("s="))
            {
                OperateResult<byte[]> operateResult = await this.ReadTagsAsync(address.Substring(2), length);
                return operateResult;
            }
            if (Regex.IsMatch(address, "ext=[0-9]+;"))
            {
                string extStr = Regex.Match(address, "ext=[0-9]+;").Value;
                ushort ext = ushort.Parse(Regex.Match(extStr, "[0-9]+").Value);
                OperateResult<byte[]> operateResult = await this.ReadExtendAsync(ext, address.Substring(extStr.Length), length);
                return operateResult;
            }
            if (Regex.IsMatch(address, "mem=", RegexOptions.IgnoreCase))
            {
                OperateResult<byte[]> operateResult = await this.ReadMemoryAsync(address.Substring(4), length);
                return operateResult;
            }
            OperateResult<McAddressData> addressResult = this.McAnalysisAddress(address, length);
            if (!addressResult.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)addressResult);
            List<byte> bytesContent = new List<byte>();
            ushort alreadyFinished = 0;
            while ((int)alreadyFinished < (int)length)
            {
                ushort readLength = (ushort)Math.Min((int)length - (int)alreadyFinished, 900);
                addressResult.Content.Length = readLength;
                OperateResult<byte[]> read = await this.ReadAddressDataAsync(addressResult.Content);
                if (!read.IsSuccess)
                    return read;
                bytesContent.AddRange((IEnumerable<byte>)read.Content);
                alreadyFinished += readLength;
                if (addressResult.Content.McDataType.DataType == (byte)0)
                    addressResult.Content.AddressStart += (int)readLength;
                else
                    addressResult.Content.AddressStart += (int)readLength * 16;
                read = (OperateResult<byte[]>)null;
            }
            return OperateResult.CreateSuccessResult<byte[]>(bytesContent.ToArray());
        }

        private async Task<OperateResult<byte[]>> ReadAddressDataAsync(
          McAddressData addressData)
        {
            byte[] coreResult = MelsecHelper.BuildReadMcCoreCommand(addressData, false);
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(MelsecMcNet.PackMcCommand(coreResult, this.NetworkNumber, this.NetworkStationNumber));
            if (!read.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)read);
            OperateResult check = MelsecMcNet.CheckResponseContent(read.Content);
            return check.IsSuccess ? MelsecMcNet.ExtractActualData(read.Content.RemoveBegin<byte>(11), false) : OperateResult.CreateFailedResult<byte[]>(check);
        }

        /// <inheritdoc />
        public override async Task<OperateResult> WriteAsync(
          string address,
          byte[] value)
        {
            OperateResult<McAddressData> addressResult = this.McAnalysisAddress(address, (ushort)0);
            if (!addressResult.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<byte[]>((OperateResult)addressResult);
            OperateResult operateResult = await this.WriteAddressDataAsync(addressResult.Content, value);
            return operateResult;
        }

        private async Task<OperateResult> WriteAddressDataAsync(
          McAddressData addressData,
          byte[] value)
        {
            byte[] coreResult = MelsecHelper.BuildWriteWordCoreCommand(addressData, value);
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(MelsecMcNet.PackMcCommand(coreResult, this.NetworkNumber, this.NetworkStationNumber));
            if (!read.IsSuccess)
                return (OperateResult)read;
            OperateResult check = MelsecMcNet.CheckResponseContent(read.Content);
            return check.IsSuccess ? OperateResult.CreateSuccessResult() : (OperateResult)OperateResult.CreateFailedResult<byte[]>(check);
        }

        /// <summary>
        /// 随机读取PLC的数据信息，可以跨地址，跨类型组合，但是每个地址只能读取一个word，也就是2个字节的内容。收到结果后，需要自行解析数据<br />
        /// Randomly read PLC data information, which can be combined across addresses and types, but each address can only read one word,
        /// which is the content of 2 bytes. After receiving the results, you need to parse the data yourself
        /// </summary>
        /// <param name="address">所有的地址的集合</param>
        /// <remarks>
        /// 访问安装有 Q 系列 C24/E71 的站 QCPU 上位站 经由 Q 系列兼容网络系统 MELSECNET/H MELSECNET/10 Ethernet 的 QCPU 其他站 时
        /// 访问点数········1≦ 字访问点数 双字访问点数 ≦192
        /// <br />
        /// 访问 QnACPU 其他站 经由 QnA 系列兼容网络系统 MELSECNET/10 Ethernet 的 Q/QnACPU 其他站 时访问点数········1≦ 字访问点数 双字访问点数 ≦96
        /// <br />
        /// 访问上述以外的 PLC CPU 其他站 时访问点数········1≦字访问点数≦10
        /// </remarks>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\melsecTest.cs" region="ReadExample3" title="随机字读取示例" /></example>
        /// <returns>结果</returns>
        public OperateResult<byte[]> ReadRandom(string[] address)
        {
            McAddressData[] address1 = new McAddressData[address.Length];
            for (int index = 0; index < address.Length; ++index)
            {
                OperateResult<McAddressData> melsecFrom = McAddressData.ParseMelsecFrom(address[index], (ushort)1);
                if (!melsecFrom.IsSuccess)
                    return OperateResult.CreateFailedResult<byte[]>((OperateResult)melsecFrom);
                address1[index] = melsecFrom.Content;
            }
            OperateResult<byte[]> operateResult = this.ReadFromCoreServer(MelsecMcNet.PackMcCommand(MelsecHelper.BuildReadRandomWordCommand(address1), this.NetworkNumber, this.NetworkStationNumber));
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult);
            OperateResult result = MelsecMcNet.CheckResponseContent(operateResult.Content);
            return !result.IsSuccess ? OperateResult.CreateFailedResult<byte[]>(result) : MelsecMcNet.ExtractActualData(operateResult.Content.RemoveBegin<byte>(11), false);
        }

        /// <summary>
        /// 随机读取PLC的数据信息，可以跨地址，跨类型组合，每个地址是任意的长度。收到结果后，需要自行解析数据，目前只支持字地址，比如D区，W区，R区，不支持X，Y，M，B，L等等<br />
        /// Read the data information of the PLC randomly. It can be combined across addresses and types. Each address is of any length. After receiving the results,
        /// you need to parse the data yourself. Currently, only word addresses are supported, such as D area, W area, R area. X, Y, M, B, L, etc
        /// </summary>
        /// <param name="address">所有的地址的集合</param>
        /// <param name="length">每个地址的长度信息</param>
        /// <remarks>
        /// 实际测试不一定所有的plc都可以读取成功，具体情况需要具体分析
        /// <br />
        /// 1 块数按照下列要求指定 120 ≧ 字软元件块数 + 位软元件块数
        /// <br />
        /// 2 各软元件点数按照下列要求指定 960 ≧ 字软元件各块的合计点数 + 位软元件各块的合计点数
        /// </remarks>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\melsecTest.cs" region="ReadExample4" title="随机批量字读取示例" />
        /// </example>
        /// <returns>结果</returns>
        public OperateResult<byte[]> ReadRandom(string[] address, ushort[] length)
        {
            if (length.Length != address.Length)
                return new OperateResult<byte[]>(StringResources.Language.TwoParametersLengthIsNotSame);
            McAddressData[] address1 = new McAddressData[address.Length];
            for (int index = 0; index < address.Length; ++index)
            {
                OperateResult<McAddressData> melsecFrom = McAddressData.ParseMelsecFrom(address[index], length[index]);
                if (!melsecFrom.IsSuccess)
                    return OperateResult.CreateFailedResult<byte[]>((OperateResult)melsecFrom);
                address1[index] = melsecFrom.Content;
            }
            OperateResult<byte[]> operateResult = this.ReadFromCoreServer(MelsecMcNet.PackMcCommand(MelsecHelper.BuildReadRandomCommand(address1), this.NetworkNumber, this.NetworkStationNumber));
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult);
            OperateResult result = MelsecMcNet.CheckResponseContent(operateResult.Content);
            return !result.IsSuccess ? OperateResult.CreateFailedResult<byte[]>(result) : MelsecMcNet.ExtractActualData(operateResult.Content.RemoveBegin<byte>(11), false);
        }

        /// <summary>
        /// 随机读取PLC的数据信息，可以跨地址，跨类型组合，但是每个地址只能读取一个word，也就是2个字节的内容。收到结果后，自动转换为了short类型的数组<br />
        /// Randomly read PLC data information, which can be combined across addresses and types, but each address can only read one word,
        /// which is the content of 2 bytes. After receiving the result, it is automatically converted to an array of type short.
        /// </summary>
        /// <param name="address">所有的地址的集合</param>
        /// <remarks>
        /// 访问安装有 Q 系列 C24/E71 的站 QCPU 上位站 经由 Q 系列兼容网络系统 MELSECNET/H MELSECNET/10 Ethernet 的 QCPU 其他站 时
        /// 访问点数········1≦ 字访问点数 双字访问点数 ≦192
        /// 
        /// 访问 QnACPU 其他站 经由 QnA 系列兼容网络系统 MELSECNET/10 Ethernet 的 Q/QnACPU 其他站 时访问点数········1≦ 字访问点数 双字访问点数 ≦96
        /// 
        /// 访问上述以外的 PLC CPU 其他站 时访问点数········1≦字访问点数≦10
        /// </remarks>
        /// <returns>结果</returns>
        public OperateResult<short[]> ReadRandomInt16(string[] address)
        {
            OperateResult<byte[]> operateResult = this.ReadRandom(address);
            return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<short[]>((OperateResult)operateResult) : OperateResult.CreateSuccessResult<short[]>(this.ByteTransform.TransInt16(operateResult.Content, 0, address.Length));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecMcNet.ReadRandom(System.String[])" />
        public async Task<OperateResult<byte[]>> ReadRandomAsync(string[] address)
        {
            McAddressData[] mcAddressDatas = new McAddressData[address.Length];
            for (int i = 0; i < address.Length; ++i)
            {
                OperateResult<McAddressData> addressResult = McAddressData.ParseMelsecFrom(address[i], (ushort)1);
                if (!addressResult.IsSuccess)
                    return OperateResult.CreateFailedResult<byte[]>((OperateResult)addressResult);
                mcAddressDatas[i] = addressResult.Content;
                addressResult = (OperateResult<McAddressData>)null;
            }
            byte[] coreResult = MelsecHelper.BuildReadRandomWordCommand(mcAddressDatas);
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(MelsecMcNet.PackMcCommand(coreResult, this.NetworkNumber, this.NetworkStationNumber));
            if (!read.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)read);
            OperateResult check = MelsecMcNet.CheckResponseContent(read.Content);
            return check.IsSuccess ? MelsecMcNet.ExtractActualData(read.Content.RemoveBegin<byte>(11), false) : OperateResult.CreateFailedResult<byte[]>(check);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecMcNet.ReadRandom(System.String[],System.UInt16[])" />
        public async Task<OperateResult<byte[]>> ReadRandomAsync(
          string[] address,
          ushort[] length)
        {
            if (length.Length != address.Length)
                return new OperateResult<byte[]>(StringResources.Language.TwoParametersLengthIsNotSame);
            McAddressData[] mcAddressDatas = new McAddressData[address.Length];
            for (int i = 0; i < address.Length; ++i)
            {
                OperateResult<McAddressData> addressResult = McAddressData.ParseMelsecFrom(address[i], length[i]);
                if (!addressResult.IsSuccess)
                    return OperateResult.CreateFailedResult<byte[]>((OperateResult)addressResult);
                mcAddressDatas[i] = addressResult.Content;
                addressResult = (OperateResult<McAddressData>)null;
            }
            byte[] coreResult = MelsecHelper.BuildReadRandomCommand(mcAddressDatas);
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(MelsecMcNet.PackMcCommand(coreResult, this.NetworkNumber, this.NetworkStationNumber));
            if (!read.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)read);
            OperateResult check = MelsecMcNet.CheckResponseContent(read.Content);
            return check.IsSuccess ? MelsecMcNet.ExtractActualData(read.Content.RemoveBegin<byte>(11), false) : OperateResult.CreateFailedResult<byte[]>(check);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecMcNet.ReadRandomInt16(System.String[])" />
        public async Task<OperateResult<short[]>> ReadRandomInt16Async(string[] address)
        {
            OperateResult<byte[]> read = await this.ReadRandomAsync(address);
            OperateResult<short[]> operateResult = read.IsSuccess ? OperateResult.CreateSuccessResult<short[]>(this.ByteTransform.TransInt16(read.Content, 0, address.Length)) : OperateResult.CreateFailedResult<short[]>((OperateResult)read);
            read = (OperateResult<byte[]>)null;
            return operateResult;
        }

        /// <inheritdoc />
        [EstMqttApi("ReadBoolArray", "")]
        public override OperateResult<bool[]> ReadBool(string address, ushort length)
        {
            OperateResult<McAddressData> operateResult1 = this.McAnalysisAddress(address, length);
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult1);
            OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(MelsecMcNet.PackMcCommand(MelsecHelper.BuildReadMcCoreCommand(operateResult1.Content, true), this.NetworkNumber, this.NetworkStationNumber));
            if (!operateResult2.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult2);
            OperateResult result = MelsecMcNet.CheckResponseContent(operateResult2.Content);
            if (!result.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>(result);
            OperateResult<byte[]> actualData = MelsecMcNet.ExtractActualData(operateResult2.Content.RemoveBegin<byte>(11), true);
            return !actualData.IsSuccess ? OperateResult.CreateFailedResult<bool[]>((OperateResult)actualData) : OperateResult.CreateSuccessResult<bool[]>(((IEnumerable<byte>)actualData.Content).Select<byte, bool>((Func<byte, bool>)(m => m == (byte)1)).Take<bool>((int)length).ToArray<bool>());
        }

        /// <inheritdoc />
        [EstMqttApi("WriteBoolArray", "")]
        public override OperateResult Write(string address, bool[] values)
        {
            OperateResult<McAddressData> operateResult1 = this.McAnalysisAddress(address, (ushort)0);
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(MelsecMcNet.PackMcCommand(MelsecHelper.BuildWriteBitCoreCommand(operateResult1.Content, values), this.NetworkNumber, this.NetworkStationNumber));
            if (!operateResult2.IsSuccess)
                return (OperateResult)operateResult2;
            OperateResult operateResult3 = MelsecMcNet.CheckResponseContent(operateResult2.Content);
            return !operateResult3.IsSuccess ? operateResult3 : OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecMcNet.ReadBool(System.String,System.UInt16)" />
        public override async Task<OperateResult<bool[]>> ReadBoolAsync(
          string address,
          ushort length)
        {
            OperateResult<McAddressData> addressResult = this.McAnalysisAddress(address, length);
            if (!addressResult.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)addressResult);
            byte[] coreResult = MelsecHelper.BuildReadMcCoreCommand(addressResult.Content, true);
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(MelsecMcNet.PackMcCommand(coreResult, this.NetworkNumber, this.NetworkStationNumber));
            if (!read.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)read);
            OperateResult check = MelsecMcNet.CheckResponseContent(read.Content);
            if (!check.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>(check);
            OperateResult<byte[]> extract = MelsecMcNet.ExtractActualData(read.Content.RemoveBegin<byte>(11), true);
            return extract.IsSuccess ? OperateResult.CreateSuccessResult<bool[]>(((IEnumerable<byte>)extract.Content).Select<byte, bool>((Func<byte, bool>)(m => m == (byte)1)).Take<bool>((int)length).ToArray<bool>()) : OperateResult.CreateFailedResult<bool[]>((OperateResult)extract);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecMcNet.Write(System.String,System.Boolean[])" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          bool[] values)
        {
            OperateResult<McAddressData> addressResult = this.McAnalysisAddress(address, (ushort)0);
            if (!addressResult.IsSuccess)
                return (OperateResult)addressResult;
            byte[] coreResult = MelsecHelper.BuildWriteBitCoreCommand(addressResult.Content, values);
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(MelsecMcNet.PackMcCommand(coreResult, this.NetworkNumber, this.NetworkStationNumber));
            if (!read.IsSuccess)
                return (OperateResult)read;
            OperateResult check = MelsecMcNet.CheckResponseContent(read.Content);
            return check.IsSuccess ? OperateResult.CreateSuccessResult() : check;
        }

        /// <summary>
        /// <b>[商业授权]</b> 读取PLC的标签信息，需要传入标签的名称，读取的字长度，标签举例：A; label[1]; bbb[10,10,10]<br />
        /// <b>[Authorization]</b> To read the label information of the PLC, you need to pass in the name of the label,
        /// the length of the word read, and an example of the label: A; label [1]; bbb [10,10,10]
        /// </summary>
        /// <param name="tag">标签名</param>
        /// <param name="length">读取长度</param>
        /// <returns>是否成功</returns>
        /// <remarks>
        ///  不可以访问局部标签。<br />
        ///  不可以访问通过GX Works2设置的全局标签。<br />
        ///  为了访问全局标签，需要通过GX Works3的全局标签设置编辑器将“来自于外部设备的访问”的设置项目置为有效。(默认为无效。)<br />
        ///  以ASCII代码进行数据通信时，由于需要从UTF-16将标签名转换为ASCII代码，因此报文容量将增加
        /// </remarks>
        [EstMqttApi(ApiTopic = "ReadTag", Description = "读取PLC的标签信息，需要传入标签的名称，读取的字长度，标签举例：A; label[1]; bbb[10,10,10]")]
        public OperateResult<byte[]> ReadTags(string tag, ushort length) => this.ReadTags(new string[1]
        {
      tag
        }, new ushort[1] { length });

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecMcNet.ReadTags(System.String,System.UInt16)" />
        [EstMqttApi(ApiTopic = "ReadTags", Description = "批量读取PLC的标签信息，需要传入标签的名称，读取的字长度，标签举例：A; label[1]; bbb[10,10,10]")]
        public OperateResult<byte[]> ReadTags(string[] tags, ushort[] length)
        {
            if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
                return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
            OperateResult<byte[]> operateResult = this.ReadFromCoreServer(MelsecMcNet.PackMcCommand(MelsecHelper.BuildReadTag(tags, length), this.NetworkNumber, this.NetworkStationNumber));
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult);
            OperateResult result = MelsecMcNet.CheckResponseContent(operateResult.Content);
            if (!result.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>(result);
            OperateResult<byte[]> actualData = MelsecMcNet.ExtractActualData(operateResult.Content.RemoveBegin<byte>(11), false);
            return !actualData.IsSuccess ? actualData : MelsecHelper.ExtraTagData(actualData.Content);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecMcNet.ReadTags(System.String,System.UInt16)" />
        public async Task<OperateResult<byte[]>> ReadTagsAsync(
          string tag,
          ushort length)
        {
            OperateResult<byte[]> operateResult = await this.ReadTagsAsync(new string[1]
            {
        tag
            }, new ushort[1] { length });
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecMcNet.ReadTags(System.String,System.UInt16)" />
        public async Task<OperateResult<byte[]>> ReadTagsAsync(
          string[] tags,
          ushort[] length)
        {
            if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
                return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
            byte[] coreResult = MelsecHelper.BuildReadTag(tags, length);
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(MelsecMcNet.PackMcCommand(coreResult, this.NetworkNumber, this.NetworkStationNumber));
            if (!read.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)read);
            OperateResult check = MelsecMcNet.CheckResponseContent(read.Content);
            if (!check.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>(check);
            OperateResult<byte[]> extract = MelsecMcNet.ExtractActualData(read.Content.RemoveBegin<byte>(11), false);
            return extract.IsSuccess ? MelsecHelper.ExtraTagData(extract.Content) : extract;
        }

        /// <summary>
        /// <b>[商业授权]</b> 读取扩展的数据信息，需要在原有的地址，长度信息之外，输入扩展值信息<br />
        /// <b>[Authorization]</b> To read the extended data information, you need to enter the extended value information in addition to the original address and length information
        /// </summary>
        /// <param name="extend">扩展信息</param>
        /// <param name="address">地址</param>
        /// <param name="length">数据长度</param>
        /// <returns>返回结果</returns>
        [EstMqttApi(ApiTopic = "ReadExtend", Description = "读取扩展的数据信息，需要在原有的地址，长度信息之外，输入扩展值信息")]
        public OperateResult<byte[]> ReadExtend(
          ushort extend,
          string address,
          ushort length)
        {
            if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
                return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
            OperateResult<McAddressData> operateResult1 = this.McAnalysisAddress(address, length);
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult1);
            OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(MelsecMcNet.PackMcCommand(MelsecHelper.BuildReadMcCoreExtendCommand(operateResult1.Content, extend, false), this.NetworkNumber, this.NetworkStationNumber));
            if (!operateResult2.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult2);
            OperateResult result = MelsecMcNet.CheckResponseContent(operateResult2.Content);
            if (!result.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>(result);
            OperateResult<byte[]> actualData = MelsecMcNet.ExtractActualData(operateResult2.Content.RemoveBegin<byte>(11), false);
            return !actualData.IsSuccess ? actualData : MelsecHelper.ExtraTagData(actualData.Content);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecMcNet.ReadExtend(System.UInt16,System.String,System.UInt16)" />
        public async Task<OperateResult<byte[]>> ReadExtendAsync(
          ushort extend,
          string address,
          ushort length)
        {
            if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
                return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
            OperateResult<McAddressData> addressResult = this.McAnalysisAddress(address, length);
            if (!addressResult.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)addressResult);
            byte[] coreResult = MelsecHelper.BuildReadMcCoreExtendCommand(addressResult.Content, extend, false);
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(MelsecMcNet.PackMcCommand(coreResult, this.NetworkNumber, this.NetworkStationNumber));
            if (!read.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)read);
            OperateResult check = MelsecMcNet.CheckResponseContent(read.Content);
            if (!check.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>(check);
            OperateResult<byte[]> extract = MelsecMcNet.ExtractActualData(read.Content.RemoveBegin<byte>(11), false);
            return extract.IsSuccess ? MelsecHelper.ExtraTagData(extract.Content) : extract;
        }

        /// <summary>
        /// <b>[商业授权]</b> 读取缓冲寄存器的数据信息，地址直接为偏移地址<br />
        /// <b>[Authorization]</b> Read the data information of the buffer register, the address is directly the offset address
        /// </summary>
        /// <remarks>
        /// 本指令不可以访问下述缓冲存储器:<br />
        /// 1. 本站(SLMP对应设备)上安装的智能功能模块<br />
        /// 2. 其它站缓冲存储器<br />
        /// </remarks>
        /// <param name="address">偏移地址</param>
        /// <param name="length">读取长度</param>
        /// <returns>读取的内容</returns>
        [EstMqttApi(ApiTopic = "ReadMemory", Description = "读取缓冲寄存器的数据信息，地址直接为偏移地址")]
        public OperateResult<byte[]> ReadMemory(string address, ushort length)
        {
            if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
                return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
            OperateResult<byte[]> operateResult1 = MelsecHelper.BuildReadMemoryCommand(address, length);
            if (!operateResult1.IsSuccess)
                return operateResult1;
            OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(MelsecMcNet.PackMcCommand(operateResult1.Content, this.NetworkNumber, this.NetworkStationNumber));
            if (!operateResult2.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult2);
            OperateResult result = MelsecMcNet.CheckResponseContent(operateResult2.Content);
            return !result.IsSuccess ? OperateResult.CreateFailedResult<byte[]>(result) : MelsecMcNet.ExtractActualData(operateResult2.Content.RemoveBegin<byte>(11), false);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecMcNet.ReadMemory(System.String,System.UInt16)" />
        public async Task<OperateResult<byte[]>> ReadMemoryAsync(
          string address,
          ushort length)
        {
            if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
                return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
            OperateResult<byte[]> coreResult = MelsecHelper.BuildReadMemoryCommand(address, length);
            if (!coreResult.IsSuccess)
                return coreResult;
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(MelsecMcNet.PackMcCommand(coreResult.Content, this.NetworkNumber, this.NetworkStationNumber));
            if (!read.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)read);
            OperateResult check = MelsecMcNet.CheckResponseContent(read.Content);
            return check.IsSuccess ? MelsecMcNet.ExtractActualData(read.Content.RemoveBegin<byte>(11), false) : OperateResult.CreateFailedResult<byte[]>(check);
        }

        /// <summary>
        /// <b>[商业授权]</b> 读取智能模块的数据信息，需要指定模块地址，偏移地址，读取的字节长度<br />
        /// <b>[Authorization]</b> To read the extended data information, you need to enter the extended value information in addition to the original address and length information
        /// </summary>
        /// <param name="module">模块地址</param>
        /// <param name="address">地址</param>
        /// <param name="length">数据长度</param>
        /// <returns>返回结果</returns>
        [EstMqttApi(ApiTopic = "ReadSmartModule", Description = "读取智能模块的数据信息，需要指定模块地址，偏移地址，读取的字节长度")]
        public OperateResult<byte[]> ReadSmartModule(
          ushort module,
          string address,
          ushort length)
        {
            if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
                return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
            OperateResult<byte[]> operateResult1 = MelsecHelper.BuildReadSmartModule(module, address, length);
            if (!operateResult1.IsSuccess)
                return operateResult1;
            OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(MelsecMcNet.PackMcCommand(operateResult1.Content, this.NetworkNumber, this.NetworkStationNumber));
            if (!operateResult2.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult2);
            OperateResult result = MelsecMcNet.CheckResponseContent(operateResult2.Content);
            return !result.IsSuccess ? OperateResult.CreateFailedResult<byte[]>(result) : MelsecMcNet.ExtractActualData(operateResult2.Content.RemoveBegin<byte>(11), false);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecMcNet.ReadSmartModule(System.UInt16,System.String,System.UInt16)" />
        public async Task<OperateResult<byte[]>> ReadSmartModuleAsync(
          ushort module,
          string address,
          ushort length)
        {
            if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
                return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
            OperateResult<byte[]> coreResult = MelsecHelper.BuildReadSmartModule(module, address, length);
            if (!coreResult.IsSuccess)
                return coreResult;
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(MelsecMcNet.PackMcCommand(coreResult.Content, this.NetworkNumber, this.NetworkStationNumber));
            if (!read.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)read);
            OperateResult check = MelsecMcNet.CheckResponseContent(read.Content);
            return check.IsSuccess ? MelsecMcNet.ExtractActualData(read.Content.RemoveBegin<byte>(11), false) : OperateResult.CreateFailedResult<byte[]>(check);
        }

        /// <summary>
        /// 远程Run操作<br />
        /// Remote Run Operation
        /// </summary>
        /// <param name="force">是否强制执行</param>
        /// <returns>是否成功</returns>
        [EstMqttApi]
        public OperateResult RemoteRun(bool force = false)
        {
            OperateResult<byte[]> operateResult1 = this.ReadFromCoreServer(MelsecMcNet.PackMcCommand(new byte[8]
            {
        (byte) 1,
        (byte) 16,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0
            }, this.NetworkNumber, this.NetworkStationNumber));
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            OperateResult operateResult2 = MelsecMcNet.CheckResponseContent(operateResult1.Content);
            return !operateResult2.IsSuccess ? operateResult2 : OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 远程Stop操作<br />
        /// Remote Stop operation
        /// </summary>
        /// <returns>是否成功</returns>
        [EstMqttApi]
        public OperateResult RemoteStop()
        {
            OperateResult<byte[]> operateResult1 = this.ReadFromCoreServer(MelsecMcNet.PackMcCommand(new byte[6]
            {
        (byte) 2,
        (byte) 16,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0
            }, this.NetworkNumber, this.NetworkStationNumber));
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            OperateResult operateResult2 = MelsecMcNet.CheckResponseContent(operateResult1.Content);
            return !operateResult2.IsSuccess ? operateResult2 : OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 远程Reset操作<br />
        /// Remote Reset Operation
        /// </summary>
        /// <returns>是否成功</returns>
        [EstMqttApi]
        public OperateResult RemoteReset()
        {
            OperateResult<byte[]> operateResult1 = this.ReadFromCoreServer(MelsecMcNet.PackMcCommand(new byte[6]
            {
        (byte) 6,
        (byte) 16,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0
            }, this.NetworkNumber, this.NetworkStationNumber));
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            OperateResult operateResult2 = MelsecMcNet.CheckResponseContent(operateResult1.Content);
            return !operateResult2.IsSuccess ? operateResult2 : OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 读取PLC的型号信息，例如 Q02HCPU<br />
        /// Read PLC model information, such as Q02HCPU
        /// </summary>
        /// <returns>返回型号的结果对象</returns>
        [EstMqttApi]
        public OperateResult<string> ReadPlcType()
        {
            OperateResult<byte[]> operateResult = this.ReadFromCoreServer(MelsecMcNet.PackMcCommand(new byte[4]
            {
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 0
            }, this.NetworkNumber, this.NetworkStationNumber));
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<string>((OperateResult)operateResult);
            OperateResult result = MelsecMcNet.CheckResponseContent(operateResult.Content);
            return !result.IsSuccess ? OperateResult.CreateFailedResult<string>(result) : OperateResult.CreateSuccessResult<string>(Encoding.ASCII.GetString(operateResult.Content, 11, 16).TrimEnd());
        }

        /// <summary>
        /// LED 熄灭 出错代码初始化<br />
        /// LED off Error code initialization
        /// </summary>
        /// <returns>是否成功</returns>
        [EstMqttApi]
        public OperateResult ErrorStateReset()
        {
            OperateResult<byte[]> operateResult1 = this.ReadFromCoreServer(MelsecMcNet.PackMcCommand(new byte[4]
            {
        (byte) 23,
        (byte) 22,
        (byte) 0,
        (byte) 0
            }, this.NetworkNumber, this.NetworkStationNumber));
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            OperateResult operateResult2 = MelsecMcNet.CheckResponseContent(operateResult1.Content);
            return !operateResult2.IsSuccess ? operateResult2 : OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecMcNet.RemoteRun(System.Boolean)" />
        public async Task<OperateResult> RemoteRunAsync()
        {
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(MelsecMcNet.PackMcCommand(new byte[8]
            {
        (byte) 1,
        (byte) 16,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0
            }, this.NetworkNumber, this.NetworkStationNumber));
            if (!read.IsSuccess)
                return (OperateResult)read;
            OperateResult check = MelsecMcNet.CheckResponseContent(read.Content);
            return check.IsSuccess ? OperateResult.CreateSuccessResult() : check;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecMcNet.RemoteStop" />
        public async Task<OperateResult> RemoteStopAsync()
        {
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(MelsecMcNet.PackMcCommand(new byte[6]
            {
        (byte) 2,
        (byte) 16,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0
            }, this.NetworkNumber, this.NetworkStationNumber));
            if (!read.IsSuccess)
                return (OperateResult)read;
            OperateResult check = MelsecMcNet.CheckResponseContent(read.Content);
            return check.IsSuccess ? OperateResult.CreateSuccessResult() : check;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecMcNet.RemoteReset" />
        public async Task<OperateResult> RemoteResetAsync()
        {
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(MelsecMcNet.PackMcCommand(new byte[6]
            {
        (byte) 6,
        (byte) 16,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0
            }, this.NetworkNumber, this.NetworkStationNumber));
            if (!read.IsSuccess)
                return (OperateResult)read;
            OperateResult check = MelsecMcNet.CheckResponseContent(read.Content);
            return check.IsSuccess ? OperateResult.CreateSuccessResult() : check;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecMcNet.ReadPlcType" />
        public async Task<OperateResult<string>> ReadPlcTypeAsync()
        {
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(MelsecMcNet.PackMcCommand(new byte[4]
            {
        (byte) 1,
        (byte) 1,
        (byte) 0,
        (byte) 0
            }, this.NetworkNumber, this.NetworkStationNumber));
            if (!read.IsSuccess)
                return OperateResult.CreateFailedResult<string>((OperateResult)read);
            OperateResult check = MelsecMcNet.CheckResponseContent(read.Content);
            return check.IsSuccess ? OperateResult.CreateSuccessResult<string>(Encoding.ASCII.GetString(read.Content, 11, 16).TrimEnd()) : OperateResult.CreateFailedResult<string>(check);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecMcNet.ErrorStateReset" />
        public async Task<OperateResult> ErrorStateResetAsync()
        {
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(MelsecMcNet.PackMcCommand(new byte[4]
            {
        (byte) 23,
        (byte) 22,
        (byte) 0,
        (byte) 0
            }, this.NetworkNumber, this.NetworkStationNumber));
            if (!read.IsSuccess)
                return (OperateResult)read;
            OperateResult check = MelsecMcNet.CheckResponseContent(read.Content);
            return check.IsSuccess ? OperateResult.CreateSuccessResult() : check;
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("MelsecMcNet[{0}:{1}]", (object)this.IpAddress, (object)this.Port);

        /// <summary>将MC协议的核心报文打包成一个可以直接对PLC进行发送的原始报文</summary>
        /// <param name="mcCore">MC协议的核心报文</param>
        /// <param name="networkNumber">网络号</param>
        /// <param name="networkStationNumber">网络站号</param>
        /// <returns>原始报文信息</returns>
        public static byte[] PackMcCommand(
          byte[] mcCore,
          byte networkNumber = 0,
          byte networkStationNumber = 0)
        {
            byte[] numArray = new byte[11 + mcCore.Length];
            numArray[0] = (byte)80;
            numArray[1] = (byte)0;
            numArray[2] = networkNumber;
            numArray[3] = byte.MaxValue;
            numArray[4] = byte.MaxValue;
            numArray[5] = (byte)3;
            numArray[6] = networkStationNumber;
            numArray[7] = (byte)((numArray.Length - 9) % 256);
            numArray[8] = (byte)((numArray.Length - 9) / 256);
            numArray[9] = (byte)10;
            numArray[10] = (byte)0;
            mcCore.CopyTo((Array)numArray, 11);
            return numArray;
        }

        /// <summary>从PLC反馈的数据中提取出实际的数据内容，需要传入反馈数据，是否位读取</summary>
        /// <param name="response">反馈的数据内容</param>
        /// <param name="isBit">是否位读取</param>
        /// <returns>解析后的结果对象</returns>
        public static OperateResult<byte[]> ExtractActualData(byte[] response, bool isBit)
        {
            if (!isBit)
                return OperateResult.CreateSuccessResult<byte[]>(response);
            byte[] numArray = new byte[response.Length * 2];
            for (int index = 0; index < response.Length; ++index)
            {
                if (((int)response[index] & 16) == 16)
                    numArray[index * 2] = (byte)1;
                if (((int)response[index] & 1) == 1)
                    numArray[index * 2 + 1] = (byte)1;
            }
            return OperateResult.CreateSuccessResult<byte[]>(numArray);
        }

        /// <summary>检查从MC返回的数据是否是合法的。</summary>
        /// <param name="content">数据内容</param>
        /// <returns>是否合法</returns>
        public static OperateResult CheckResponseContent(byte[] content)
        {
            ushort uint16 = BitConverter.ToUInt16(content, 9);
            return uint16 > (ushort)0 ? (OperateResult)new OperateResult<byte[]>((int)uint16, MelsecHelper.GetErrorDescription((int)uint16)) : OperateResult.CreateSuccessResult();
        }
    }
}
