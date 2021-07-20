// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Fuji.FujiSPBOverTcp
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
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.Common.Profinet.Fuji
{
    /// <summary>
    /// 富士PLC的SPB协议，详细的地址信息见api文档说明，地址可以携带站号信息，例如：s=2;D100，PLC侧需要配置无BCC计算，包含0D0A结束码<br />
    /// Fuji PLC's SPB protocol. For detailed address information, see the api documentation,
    /// The address can carry station number information, for example: s=2;D100, PLC side needs to be configured with no BCC calculation, including 0D0A end code
    /// </summary>
    /// <remarks>
    /// 其所支持的地址形式如下：
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
    ///     <term>读写字单位的时候，M2代表位的M32</term>
    ///   </item>
    ///   <item>
    ///     <term>输入继电器</term>
    ///     <term>X</term>
    ///     <term>X10,X20</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term>读取字单位的时候，X2代表位的X32</term>
    ///   </item>
    ///   <item>
    ///     <term>输出继电器</term>
    ///     <term>Y</term>
    ///     <term>Y10,Y20</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term>读写字单位的时候，Y2代表位的Y32</term>
    ///   </item>
    ///   <item>
    ///     <term>锁存继电器</term>
    ///     <term>L</term>
    ///     <term>L100,L200</term>
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
    ///     <term>计数器的线圈</term>
    ///     <term>CC</term>
    ///     <term>CC100,CC200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>计数器的当前</term>
    ///     <term>CN</term>
    ///     <term>CN100,CN200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>×</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>数据寄存器</term>
    ///     <term>D</term>
    ///     <term>D1000,D2000</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term>读位的时候，D10.15代表第10个字的第15位</term>
    ///   </item>
    ///   <item>
    ///     <term>文件寄存器</term>
    ///     <term>R</term>
    ///     <term>R100,R200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term>读位的时候，R10.15代表第10个字的第15位</term>
    ///   </item>
    ///   <item>
    ///     <term>链接寄存器</term>
    ///     <term>W</term>
    ///     <term>W100,W200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term>读位的时候，W10.15代表第10个字的第15位</term>
    ///   </item>
    /// </list>
    /// </remarks>
    public class FujiSPBOverTcp : NetworkDeviceBase
    {
        private byte station = 1;

        /// <summary>
        /// 使用默认的构造方法实例化对象<br />
        /// Instantiate the object using the default constructor
        /// </summary>
        public FujiSPBOverTcp()
        {
            this.WordLength = (ushort)1;
            this.LogMsgFormatBinary = false;
            this.ByteTransform = (IByteTransform)new RegularByteTransform();
            this.SleepTime = 20;
        }

        /// <summary>
        /// 使用指定的ip地址和端口来实例化一个对象<br />
        /// Instantiate an object with the specified IP address and port
        /// </summary>
        /// <param name="ipAddress">设备的Ip地址</param>
        /// <param name="port">设备的端口号</param>
        public FujiSPBOverTcp(string ipAddress, int port)
          : this()
        {
            this.IpAddress = ipAddress;
            this.Port = port;
        }

        /// <inheritdoc />
        protected override INetMessage GetNewNetMessage() => (INetMessage)new FujiSPBMessage();

        /// <summary>
        /// PLC的站号信息<br />
        /// PLC station number information
        /// </summary>
        public byte Station
        {
            get => this.station;
            set => this.station = value;
        }

        /// <summary>
        /// 批量读取PLC的数据，以字为单位，支持读取X,Y,L,M,D,TN,CN,TC,CC,R,W具体的地址范围需要根据PLC型号来确认，地址可以携带站号信息，例如：s=2;D100<br />
        /// Read PLC data in batches, in units of words. Supports reading X, Y, L, M, D, TN, CN, TC, CC, R, W.
        /// The specific address range needs to be confirmed according to the PLC model, The address can carry station number information, for example: s=2;D100
        /// </summary>
        /// <param name="address">地址信息</param>
        /// <param name="length">数据长度</param>
        /// <returns>读取结果信息</returns>
        /// <remarks>单次读取的最大的字数为105，如果读取的字数超过这个值，请分批次读取。</remarks>
        [EstMqttApi("ReadByteArray", "")]
        public override OperateResult<byte[]> Read(string address, ushort length)
        {
            OperateResult<byte[]> operateResult1 = FujiSPBOverTcp.BuildReadCommand(this.station, address, length);
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult1);
            OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
            if (!operateResult2.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult2);
            OperateResult<byte[]> operateResult3 = FujiSPBOverTcp.CheckResponseData(operateResult2.Content);
            return !operateResult3.IsSuccess ? OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult3) : OperateResult.CreateSuccessResult<byte[]>(Encoding.ASCII.GetString(operateResult3.Content.RemoveBegin<byte>(4)).ToHexBytes());
        }

        /// <summary>
        /// 批量写入PLC的数据，以字为单位，也就是说最少2个字节信息，支持读取X,Y,L,M,D,TN,CN,TC,CC,R具体的地址范围需要根据PLC型号来确认，地址可以携带站号信息，例如：s=2;D100<br />
        /// The data written to the PLC in batches, in units of words, that is, a minimum of 2 bytes of information. It supports reading X, Y, L, M, D, TN, CN, TC, CC, and R.
        /// The specific address range needs to be based on PLC model to confirm, The address can carry station number information, for example: s=2;D100
        /// </summary>
        /// <param name="address">地址信息，举例，D100，R200，TN100，CN200</param>
        /// <param name="value">数据值</param>
        /// <returns>是否写入成功</returns>
        /// <remarks>单次写入的最大的字数为103个字，如果写入的数据超过这个长度，请分批次写入</remarks>
        [EstMqttApi("WriteByteArray", "")]
        public override OperateResult Write(string address, byte[] value)
        {
            OperateResult<byte[]> operateResult1 = FujiSPBOverTcp.BuildWriteByteCommand(this.station, address, value);
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
            return !operateResult2.IsSuccess ? (OperateResult)operateResult2 : (OperateResult)FujiSPBOverTcp.CheckResponseData(operateResult2.Content);
        }

        /// <summary>
        /// 批量读取PLC的Bool数据，以位为单位，支持读取X,Y,L,M,D,TN,CN,TC,CC,R,W，例如 M100, 如果是寄存器地址，可以使用D10.12来访问第10个字的12位，地址可以携带站号信息，例如：s=2;M100<br />
        /// Read PLC's Bool data in batches, in units of bits, support reading X, Y, L, M, D, TN, CN, TC, CC, R, W, such as M100, if it is a register address,
        /// you can use D10. 12 to access the 12 bits of the 10th word, the address can carry station number information, for example: s=2;M100
        /// </summary>
        /// <param name="address">地址信息，举例：M100, D10.12</param>
        /// <param name="length">读取的bool长度信息</param>
        /// <returns>Bool[]的结果对象</returns>
        [EstMqttApi("ReadBoolArray", "")]
        public override OperateResult<bool[]> ReadBool(string address, ushort length)
        {
            byte parameter = (byte)EstHelper.ExtractParameter(ref address, "s", (int)this.station);
            OperateResult<FujiSPBAddress> from = FujiSPBAddress.ParseFrom(address);
            if (!from.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)from);
            if ((address.StartsWith("X") || address.StartsWith("Y") || (address.StartsWith("M") || address.StartsWith("L")) || address.StartsWith("TC") || address.StartsWith("CC")) && address.IndexOf('.') < 0)
            {
                from.Content.BitIndex = (int)from.Content.Address % 16;
                from.Content.Address /= (ushort)16;
            }
            ushort length1 = (ushort)((from.Content.GetBitIndex() + (int)length - 1) / 16 - from.Content.GetBitIndex() / 16 + 1);
            OperateResult<byte[]> operateResult1 = FujiSPBOverTcp.BuildReadCommand(parameter, from.Content, length1);
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult1);
            OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
            if (!operateResult2.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult2);
            OperateResult<byte[]> operateResult3 = FujiSPBOverTcp.CheckResponseData(operateResult2.Content);
            return !operateResult3.IsSuccess ? OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult3) : OperateResult.CreateSuccessResult<bool[]>(Encoding.ASCII.GetString(operateResult3.Content.RemoveBegin<byte>(4)).ToHexBytes().ToBoolArray().SelectMiddle<bool>(from.Content.BitIndex, (int)length));
        }

        /// <summary>
        /// 写入一个Bool值到一个地址里，地址可以是线圈地址，也可以是寄存器地址，例如：M100, D10.12，地址可以携带站号信息，例如：s=2;D10.12<br />
        /// Write a Bool value to an address. The address can be a coil address or a register address, for example: M100, D10.12.
        /// The address can carry station number information, for example: s=2;D10.12
        /// </summary>
        /// <param name="address">地址信息，举例：M100, D10.12</param>
        /// <param name="value">写入的bool值</param>
        /// <returns>是否写入成功的结果对象</returns>
        [EstMqttApi("WriteBool", "")]
        public override OperateResult Write(string address, bool value)
        {
            OperateResult<byte[]> operateResult1 = FujiSPBOverTcp.BuildWriteBoolCommand(this.station, address, value);
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
            return !operateResult2.IsSuccess ? (OperateResult)operateResult2 : (OperateResult)FujiSPBOverTcp.CheckResponseData(operateResult2.Content);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Fuji.FujiSPBOverTcp.Read(System.String,System.UInt16)" />
        public override async Task<OperateResult<byte[]>> ReadAsync(
          string address,
          ushort length)
        {
            OperateResult<byte[]> command = FujiSPBOverTcp.BuildReadCommand(this.station, address, length);
            if (!command.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)command);
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
            if (!read.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)read);
            OperateResult<byte[]> check = FujiSPBOverTcp.CheckResponseData(read.Content);
            return check.IsSuccess ? OperateResult.CreateSuccessResult<byte[]>(Encoding.ASCII.GetString(check.Content.RemoveBegin<byte>(4)).ToHexBytes()) : OperateResult.CreateFailedResult<byte[]>((OperateResult)check);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Fuji.FujiSPBOverTcp.Write(System.String,System.Byte[])" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          byte[] value)
        {
            OperateResult<byte[]> command = FujiSPBOverTcp.BuildWriteByteCommand(this.station, address, value);
            if (!command.IsSuccess)
                return (OperateResult)command;
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
            return read.IsSuccess ? (OperateResult)FujiSPBOverTcp.CheckResponseData(read.Content) : (OperateResult)read;
        }

        /// <inheritdoc />
        public override async Task<OperateResult<bool[]>> ReadBoolAsync(
          string address,
          ushort length)
        {
            byte stat = (byte)EstHelper.ExtractParameter(ref address, "s", (int)this.station);
            OperateResult<FujiSPBAddress> addressAnalysis = FujiSPBAddress.ParseFrom(address);
            if (!addressAnalysis.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)addressAnalysis);
            if ((address.StartsWith("X") || address.StartsWith("Y") || (address.StartsWith("M") || address.StartsWith("L")) || address.StartsWith("TC") || address.StartsWith("CC")) && address.IndexOf('.') < 0)
            {
                addressAnalysis.Content.BitIndex = (int)addressAnalysis.Content.Address % 16;
                addressAnalysis.Content.Address /= (ushort)16;
            }
            ushort len = (ushort)((addressAnalysis.Content.GetBitIndex() + (int)length - 1) / 16 - addressAnalysis.Content.GetBitIndex() / 16 + 1);
            OperateResult<byte[]> command = FujiSPBOverTcp.BuildReadCommand(stat, addressAnalysis.Content, len);
            if (!command.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)command);
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
            if (!read.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)read);
            OperateResult<byte[]> check = FujiSPBOverTcp.CheckResponseData(read.Content);
            return check.IsSuccess ? OperateResult.CreateSuccessResult<bool[]>(Encoding.ASCII.GetString(check.Content.RemoveBegin<byte>(4)).ToHexBytes().ToBoolArray().SelectMiddle<bool>(addressAnalysis.Content.BitIndex, (int)length)) : OperateResult.CreateFailedResult<bool[]>((OperateResult)check);
        }

        /// <inheritdoc />
        public override async Task<OperateResult> WriteAsync(
          string address,
          bool value)
        {
            OperateResult<byte[]> command = FujiSPBOverTcp.BuildWriteBoolCommand(this.station, address, value);
            if (!command.IsSuccess)
                return (OperateResult)command;
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
            return read.IsSuccess ? (OperateResult)FujiSPBOverTcp.CheckResponseData(read.Content) : (OperateResult)read;
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("FujiSPBOverTcp[{0}:{1}]", (object)this.IpAddress, (object)this.Port);

        /// <summary>将int数据转换成SPB可识别的标准的数据内容，例如 2转换为0200 , 200转换为0002</summary>
        /// <param name="address">等待转换的数据内容</param>
        /// <returns>转换之后的数据内容</returns>
        public static string AnalysisIntegerAddress(int address)
        {
            string str = address.ToString("D4");
            return str.Substring(2) + str.Substring(0, 2);
        }

        /// <summary>计算指令的和校验码</summary>
        /// <param name="data">指令</param>
        /// <returns>校验之后的信息</returns>
        public static string CalculateAcc(string data)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(data);
            int num = 0;
            for (int index = 0; index < bytes.Length; ++index)
                num += (int)bytes[index];
            return num.ToString("X4").Substring(2);
        }

        /// <summary>创建一条读取的指令信息，需要指定一些参数，单次读取最大105个字</summary>
        /// <param name="station">PLC的站号</param>
        /// <param name="address">地址信息</param>
        /// <param name="length">数据长度</param>
        /// <returns>是否成功的结果对象</returns>
        public static OperateResult<byte[]> BuildReadCommand(
          byte station,
          string address,
          ushort length)
        {
            station = (byte)EstHelper.ExtractParameter(ref address, "s", (int)station);
            OperateResult<FujiSPBAddress> from = FujiSPBAddress.ParseFrom(address);
            return !from.IsSuccess ? OperateResult.CreateFailedResult<byte[]>((OperateResult)from) : FujiSPBOverTcp.BuildReadCommand(station, from.Content, length);
        }

        /// <summary>创建一条读取的指令信息，需要指定一些参数，单次读取最大105个字</summary>
        /// <param name="station">PLC的站号</param>
        /// <param name="address">地址信息</param>
        /// <param name="length">数据长度</param>
        /// <returns>是否成功的结果对象</returns>
        public static OperateResult<byte[]> BuildReadCommand(
          byte station,
          FujiSPBAddress address,
          ushort length)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(':');
            stringBuilder.Append(station.ToString("X2"));
            stringBuilder.Append("09");
            stringBuilder.Append("FFFF");
            stringBuilder.Append("00");
            stringBuilder.Append("00");
            stringBuilder.Append(address.GetWordAddress());
            stringBuilder.Append(FujiSPBOverTcp.AnalysisIntegerAddress((int)length));
            stringBuilder.Append("\r\n");
            return OperateResult.CreateSuccessResult<byte[]>(Encoding.ASCII.GetBytes(stringBuilder.ToString()));
        }

        /// <summary>创建一条读取多个地址的指令信息，需要指定一些参数，单次读取最大105个字</summary>
        /// <param name="station">PLC的站号</param>
        /// <param name="address">地址信息</param>
        /// <param name="length">数据长度</param>
        /// <param name="isBool">是否位读取</param>
        /// <returns>是否成功的结果对象</returns>
        public static OperateResult<byte[]> BuildReadCommand(
          byte station,
          string[] address,
          ushort[] length,
          bool isBool)
        {
            if (address == null || length == null)
                return new OperateResult<byte[]>("Parameter address or length can't be null");
            if (address.Length != length.Length)
                return new OperateResult<byte[]>(StringResources.Language.TwoParametersLengthIsNotSame);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(':');
            stringBuilder.Append(station.ToString("X2"));
            stringBuilder.Append((6 + address.Length * 4).ToString("X2"));
            stringBuilder.Append("FFFF");
            stringBuilder.Append("00");
            stringBuilder.Append("04");
            stringBuilder.Append("00");
            stringBuilder.Append(address.Length.ToString("X2"));
            for (int index = 0; index < address.Length; ++index)
            {
                station = (byte)EstHelper.ExtractParameter(ref address[index], "s", (int)station);
                OperateResult<FujiSPBAddress> from = FujiSPBAddress.ParseFrom(address[index]);
                if (!from.IsSuccess)
                    return OperateResult.CreateFailedResult<byte[]>((OperateResult)from);
                stringBuilder.Append(from.Content.TypeCode);
                stringBuilder.Append(length[index].ToString("X2"));
                stringBuilder.Append(FujiSPBOverTcp.AnalysisIntegerAddress((int)from.Content.Address));
            }
            stringBuilder[1] = station.ToString("X2")[0];
            stringBuilder[2] = station.ToString("X2")[1];
            stringBuilder.Append("\r\n");
            return OperateResult.CreateSuccessResult<byte[]>(Encoding.ASCII.GetBytes(stringBuilder.ToString()));
        }

        /// <summary>创建一条别入byte数据的指令信息，需要指定一些参数，按照字单位，单次写入最大103个字</summary>
        /// <param name="station">站号</param>
        /// <param name="address">地址</param>
        /// <param name="value">数组值</param>
        /// <returns>是否创建成功</returns>
        public static OperateResult<byte[]> BuildWriteByteCommand(
          byte station,
          string address,
          byte[] value)
        {
            station = (byte)EstHelper.ExtractParameter(ref address, "s", (int)station);
            OperateResult<FujiSPBAddress> from = FujiSPBAddress.ParseFrom(address);
            if (!from.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)from);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(':');
            stringBuilder.Append(station.ToString("X2"));
            stringBuilder.Append("00");
            stringBuilder.Append("FFFF");
            stringBuilder.Append("01");
            stringBuilder.Append("00");
            stringBuilder.Append(from.Content.GetWordAddress());
            stringBuilder.Append(FujiSPBOverTcp.AnalysisIntegerAddress(value.Length / 2));
            stringBuilder.Append(value.ToHexString());
            stringBuilder[3] = ((stringBuilder.Length - 5) / 2).ToString("X2")[0];
            stringBuilder[4] = ((stringBuilder.Length - 5) / 2).ToString("X2")[1];
            stringBuilder.Append("\r\n");
            return OperateResult.CreateSuccessResult<byte[]>(Encoding.ASCII.GetBytes(stringBuilder.ToString()));
        }

        /// <summary>创建一条别入byte数据的指令信息，需要指定一些参数，按照字单位，单次写入最大103个字</summary>
        /// <param name="station">站号</param>
        /// <param name="address">地址</param>
        /// <param name="value">数组值</param>
        /// <returns>是否创建成功</returns>
        public static OperateResult<byte[]> BuildWriteBoolCommand(
          byte station,
          string address,
          bool value)
        {
            station = (byte)EstHelper.ExtractParameter(ref address, "s", (int)station);
            OperateResult<FujiSPBAddress> from = FujiSPBAddress.ParseFrom(address);
            if (!from.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)from);
            if ((address.StartsWith("X") || address.StartsWith("Y") || (address.StartsWith("M") || address.StartsWith("L")) || address.StartsWith("TC") || address.StartsWith("CC")) && address.IndexOf('.') < 0)
            {
                from.Content.BitIndex = (int)from.Content.Address % 16;
                from.Content.Address /= (ushort)16;
            }
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(':');
            stringBuilder.Append(station.ToString("X2"));
            stringBuilder.Append("00");
            stringBuilder.Append("FFFF");
            stringBuilder.Append("01");
            stringBuilder.Append("02");
            stringBuilder.Append(from.Content.GetWriteBoolAddress());
            stringBuilder.Append(value ? "01" : "00");
            stringBuilder[3] = ((stringBuilder.Length - 5) / 2).ToString("X2")[0];
            stringBuilder[4] = ((stringBuilder.Length - 5) / 2).ToString("X2")[1];
            stringBuilder.Append("\r\n");
            return OperateResult.CreateSuccessResult<byte[]>(Encoding.ASCII.GetBytes(stringBuilder.ToString()));
        }

        /// <summary>检查反馈的数据信息，是否包含了错误码，如果没有包含，则返回成功</summary>
        /// <param name="content">原始的报文返回</param>
        /// <returns>是否成功的结果对象</returns>
        public static OperateResult<byte[]> CheckResponseData(byte[] content)
        {
            if (content[0] != (byte)58)
                return new OperateResult<byte[]>((int)content[0], "Read Faild:" + SoftBasic.ByteToHexString(content, ' '));
            string code = Encoding.ASCII.GetString(content, 9, 2);
            if (code != "00")
                return new OperateResult<byte[]>(Convert.ToInt32(code, 16), FujiSPBOverTcp.GetErrorDescriptionFromCode(code));
            if (content[content.Length - 2] == (byte)13 && content[content.Length - 1] == (byte)10)
                content = content.RemoveLast<byte>(2);
            return OperateResult.CreateSuccessResult<byte[]>(content.RemoveBegin<byte>(11));
        }

        /// <summary>根据错误码获取到真实的文本信息</summary>
        /// <param name="code">错误码</param>
        /// <returns>错误的文本描述</returns>
        public static string GetErrorDescriptionFromCode(string code)
        {
            switch (code)
            {
                case "01":
                    return StringResources.Language.FujiSpbStatus01;
                case "02":
                    return StringResources.Language.FujiSpbStatus02;
                case "03":
                    return StringResources.Language.FujiSpbStatus03;
                case "04":
                    return StringResources.Language.FujiSpbStatus04;
                case "05":
                    return StringResources.Language.FujiSpbStatus05;
                case "06":
                    return StringResources.Language.FujiSpbStatus06;
                case "07":
                    return StringResources.Language.FujiSpbStatus07;
                case "09":
                    return StringResources.Language.FujiSpbStatus09;
                case "0C":
                    return StringResources.Language.FujiSpbStatus0C;
                default:
                    return StringResources.Language.UnknownError;
            }
        }
    }
}
