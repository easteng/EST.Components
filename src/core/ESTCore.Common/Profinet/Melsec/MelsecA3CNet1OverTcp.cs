// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Melsec.MelsecA3CNet1OverTcp
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;
using ESTCore.Common.Core;
using ESTCore.Common.Core.Address;
using ESTCore.Common.Core.Net;
using ESTCore.Common.Reflection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.Common.Profinet.Melsec
{
    /// <summary>
    /// 基于Qna 兼容3C帧的格式一的通讯，具体的地址需要参照三菱的基本地址，本类是基于tcp通讯的实现<br />
    /// Based on Qna-compatible 3C frame format one communication, the specific address needs to refer to the basic address of Mitsubishi. This class is based on TCP communication.
    /// </summary>
    /// <remarks>地址可以携带站号信息，例如：s=2;D100</remarks>
    /// <example>
    /// 地址的输入的格式说明如下：
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
    /// </example>
    public class MelsecA3CNet1OverTcp : NetworkDeviceBase
    {
        private byte station = 0;

        /// <summary>
        /// 实例化默认的对象<br />
        /// Instantiate the default object
        /// </summary>
        public MelsecA3CNet1OverTcp()
        {
            this.WordLength = (ushort)1;
            this.ByteTransform = (IByteTransform)new RegularByteTransform();
            this.SleepTime = 20;
        }

        /// <summary>
        /// 指定ip地址和端口号来实例化对象<br />
        /// Specify the IP address and port number to instantiate the object
        /// </summary>
        /// <param name="ipAddress">Ip地址信息</param>
        /// <param name="port">端口号信息</param>
        public MelsecA3CNet1OverTcp(string ipAddress, int port)
          : this()
        {
            this.IpAddress = ipAddress;
            this.Port = port;
        }

        /// <inheritdoc />
        public byte Station
        {
            get => this.station;
            set => this.station = value;
        }

        private OperateResult<byte[]> ReadWithPackCommand(byte[] command, byte station) => this.ReadFromCoreServer(MelsecA3CNet1OverTcp.PackCommand(command, station));

        private async Task<OperateResult<byte[]>> ReadWithPackCommandAsync(
          byte[] command)
        {
            OperateResult<byte[]> operateResult = await this.ReadFromCoreServerAsync(MelsecA3CNet1OverTcp.PackCommand(command, this.station));
            return operateResult;
        }

        /// <summary>
        /// 批量读取PLC的数据，以字为单位，支持读取X,Y,M,S,D,T,C，具体的地址范围需要根据PLC型号来确认<br />
        /// Read PLC data in batches, in units of words, supports reading X, Y, M, S, D, T, C. The specific address range needs to be confirmed according to the PLC model
        /// </summary>
        /// <param name="address">地址信息</param>
        /// <param name="length">数据长度</param>
        /// <returns>读取结果信息</returns>
        [EstMqttApi("ReadByteArray", "")]
        public override OperateResult<byte[]> Read(string address, ushort length)
        {
            byte parameter = (byte)EstHelper.ExtractParameter(ref address, "s", (int)this.station);
            return MelsecA3CNet1OverTcp.ReadHelper(address, length, parameter, new Func<byte[], byte, OperateResult<byte[]>>(this.ReadWithPackCommand));
        }

        /// <summary>
        /// 批量写入PLC的数据，以字为单位，也就是说最少2个字节信息，支持X,Y,M,S,D,T,C，具体的地址范围需要根据PLC型号来确认<br />
        /// The data written to the PLC in batches is in units of words, that is, at least 2 bytes of information. It supports X, Y, M, S, D, T, and C. The specific address range needs to be confirmed according to the PLC model.
        /// </summary>
        /// <param name="address">地址信息</param>
        /// <param name="value">数据值</param>
        /// <returns>是否写入成功</returns>
        [EstMqttApi("WriteByteArray", "")]
        public override OperateResult Write(string address, byte[] value)
        {
            byte parameter = (byte)EstHelper.ExtractParameter(ref address, "s", (int)this.station);
            return MelsecA3CNet1OverTcp.WriteHelper(address, value, parameter, new Func<byte[], byte, OperateResult<byte[]>>(this.ReadWithPackCommand));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecA3CNet1OverTcp.Read(System.String,System.UInt16)" />
        public override async Task<OperateResult<byte[]>> ReadAsync(
          string address,
          ushort length)
        {
            byte stat = (byte)EstHelper.ExtractParameter(ref address, "s", (int)this.station);
            OperateResult<McAddressData> addressResult = McAddressData.ParseMelsecFrom(address, length);
            if (!addressResult.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)addressResult);
            byte[] command = MelsecHelper.BuildAsciiReadMcCoreCommand(addressResult.Content, false);
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(MelsecA3CNet1OverTcp.PackCommand(command, stat));
            if (!read.IsSuccess)
                return read;
            if (read.Content[0] != (byte)2)
                return new OperateResult<byte[]>((int)read.Content[0], "Read Faild:" + Encoding.ASCII.GetString(read.Content, 1, read.Content.Length - 1));
            byte[] Content = new byte[(int)length * 2];
            for (int i = 0; i < Content.Length / 2; ++i)
            {
                ushort tmp = Convert.ToUInt16(Encoding.ASCII.GetString(read.Content, i * 4 + 11, 4), 16);
                BitConverter.GetBytes(tmp).CopyTo((Array)Content, i * 2);
            }
            return OperateResult.CreateSuccessResult<byte[]>(Content);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecA3CNet1OverTcp.Write(System.String,System.Byte[])" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          byte[] value)
        {
            byte stat = (byte)EstHelper.ExtractParameter(ref address, "s", (int)this.station);
            OperateResult<McAddressData> addressResult = McAddressData.ParseMelsecFrom(address, (ushort)0);
            if (!addressResult.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<byte[]>((OperateResult)addressResult);
            byte[] command = MelsecHelper.BuildAsciiWriteWordCoreCommand(addressResult.Content, value);
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(MelsecA3CNet1OverTcp.PackCommand(command, stat));
            return read.IsSuccess ? (read.Content[0] == (byte)6 ? OperateResult.CreateSuccessResult() : new OperateResult((int)read.Content[0], "Write Faild:" + Encoding.ASCII.GetString(read.Content, 1, read.Content.Length - 1))) : (OperateResult)read;
        }

        /// <summary>
        /// 批量读取bool类型数据，支持的类型为X,Y,S,T,C，具体的地址范围取决于PLC的类型<br />
        /// Read bool data in batches. The supported types are X, Y, S, T, C. The specific address range depends on the type of PLC.
        /// </summary>
        /// <param name="address">地址信息，比如X10,Y17，注意X，Y的地址是8进制的</param>
        /// <param name="length">读取的长度</param>
        /// <returns>读取结果信息</returns>
        [EstMqttApi("ReadBoolArray", "")]
        public override OperateResult<bool[]> ReadBool(string address, ushort length)
        {
            byte parameter = (byte)EstHelper.ExtractParameter(ref address, "s", (int)this.station);
            return MelsecA3CNet1OverTcp.ReadBoolHelper(address, length, parameter, new Func<byte[], byte, OperateResult<byte[]>>(this.ReadWithPackCommand));
        }

        /// <summary>
        /// 批量写入bool类型的数组，支持的类型为X,Y,S,T,C，具体的地址范围取决于PLC的类型<br />
        /// Write arrays of type bool in batches. The supported types are X, Y, S, T, C. The specific address range depends on the type of PLC.
        /// </summary>
        /// <param name="address">PLC的地址信息</param>
        /// <param name="value">数据信息</param>
        /// <returns>是否写入成功</returns>
        [EstMqttApi("WriteBoolArray", "")]
        public override OperateResult Write(string address, bool[] value)
        {
            byte parameter = (byte)EstHelper.ExtractParameter(ref address, "s", (int)this.station);
            return MelsecA3CNet1OverTcp.WriteHelper(address, value, parameter, new Func<byte[], byte, OperateResult<byte[]>>(this.ReadWithPackCommand));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecA3CNet1OverTcp.ReadBool(System.String,System.UInt16)" />
        public override async Task<OperateResult<bool[]>> ReadBoolAsync(
          string address,
          ushort length)
        {
            byte stat = (byte)EstHelper.ExtractParameter(ref address, "s", (int)this.station);
            OperateResult<McAddressData> addressResult = McAddressData.ParseMelsecFrom(address, length);
            if (!addressResult.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)addressResult);
            byte[] command = MelsecHelper.BuildAsciiReadMcCoreCommand(addressResult.Content, true);
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(MelsecA3CNet1OverTcp.PackCommand(command, stat));
            if (!read.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)read);
            if (read.Content[0] != (byte)2)
                return new OperateResult<bool[]>((int)read.Content[0], "Read Faild:" + Encoding.ASCII.GetString(read.Content, 1, read.Content.Length - 1));
            byte[] buffer = new byte[(int)length];
            Array.Copy((Array)read.Content, 11, (Array)buffer, 0, (int)length);
            return OperateResult.CreateSuccessResult<bool[]>(((IEnumerable<byte>)buffer).Select<byte, bool>((Func<byte, bool>)(m => m == (byte)49)).ToArray<bool>());
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecA3CNet1OverTcp.Write(System.String,System.Boolean[])" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          bool[] value)
        {
            byte stat = (byte)EstHelper.ExtractParameter(ref address, "s", (int)this.station);
            OperateResult<McAddressData> addressResult = McAddressData.ParseMelsecFrom(address, (ushort)0);
            if (!addressResult.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<bool[]>((OperateResult)addressResult);
            byte[] command = MelsecHelper.BuildAsciiWriteBitCoreCommand(addressResult.Content, value);
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(MelsecA3CNet1OverTcp.PackCommand(command, stat));
            return read.IsSuccess ? (read.Content[0] == (byte)6 ? OperateResult.CreateSuccessResult() : new OperateResult((int)read.Content[0], "Write Faild:" + Encoding.ASCII.GetString(read.Content, 1, read.Content.Length - 1))) : (OperateResult)read;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecFxLinksOverTcp.StartPLC(System.String)" />
        [EstMqttApi]
        public OperateResult RemoteRun() => MelsecA3CNet1OverTcp.RemoteRunHelper(this.station, new Func<byte[], byte, OperateResult<byte[]>>(this.ReadWithPackCommand));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecFxLinksOverTcp.StopPLC(System.String)" />
        [EstMqttApi]
        public OperateResult RemoteStop() => MelsecA3CNet1OverTcp.RemoteStopHelper(this.station, new Func<byte[], byte, OperateResult<byte[]>>(this.ReadWithPackCommand));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecMcNet.ReadPlcType" />
        [EstMqttApi]
        public OperateResult<string> ReadPlcType() => MelsecA3CNet1OverTcp.ReadPlcTypeHelper(this.station, new Func<byte[], byte, OperateResult<byte[]>>(this.ReadWithPackCommand));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecA3CNet1OverTcp.RemoteRun" />
        public async Task<OperateResult> RemoteRunAsync()
        {
            OperateResult<byte[]> read = await this.ReadWithPackCommandAsync(Encoding.ASCII.GetBytes("1001000000010000"));
            OperateResult operateResult = read.IsSuccess ? (read.Content[0] == (byte)6 || read.Content[0] == (byte)2 ? OperateResult.CreateSuccessResult() : new OperateResult((int)read.Content[0], "Faild:" + Encoding.ASCII.GetString(read.Content, 1, read.Content.Length - 1))) : (OperateResult)read;
            read = (OperateResult<byte[]>)null;
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecA3CNet1OverTcp.RemoteStop" />
        public async Task<OperateResult> RemoteStopAsync()
        {
            OperateResult<byte[]> read = await this.ReadWithPackCommandAsync(Encoding.ASCII.GetBytes("100200000001"));
            OperateResult operateResult = read.IsSuccess ? (read.Content[0] == (byte)6 || read.Content[0] == (byte)2 ? OperateResult.CreateSuccessResult() : new OperateResult((int)read.Content[0], "Faild:" + Encoding.ASCII.GetString(read.Content, 1, read.Content.Length - 1))) : (OperateResult)read;
            read = (OperateResult<byte[]>)null;
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecA3CNet1OverTcp.ReadPlcType" />
        public async Task<OperateResult<string>> ReadPlcTypeAsync()
        {
            OperateResult<byte[]> read = await this.ReadWithPackCommandAsync(Encoding.ASCII.GetBytes("01010000"));
            OperateResult<string> operateResult = read.IsSuccess ? (read.Content[0] == (byte)6 || read.Content[0] == (byte)2 ? OperateResult.CreateSuccessResult<string>(Encoding.ASCII.GetString(read.Content, 11, 16).TrimEnd()) : new OperateResult<string>((int)read.Content[0], "Faild:" + Encoding.ASCII.GetString(read.Content, 1, read.Content.Length - 1))) : OperateResult.CreateFailedResult<string>((OperateResult)read);
            read = (OperateResult<byte[]>)null;
            return operateResult;
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("MelsecA3CNet1OverTcp[{0}:{1}]", (object)this.IpAddress, (object)this.Port);

        /// <summary>
        /// 批量读取PLC的数据，以字为单位，支持读取X,Y,M,S,D,T,C，具体的地址范围需要根据PLC型号来确认
        /// </summary>
        /// <param name="address">地址信息</param>
        /// <param name="length">数据长度</param>
        /// <param name="station">当前PLC的站号信息</param>
        /// <param name="readCore">通信的载体信息</param>
        /// <returns>读取结果信息</returns>
        public static OperateResult<byte[]> ReadHelper(
          string address,
          ushort length,
          byte station,
          Func<byte[], byte, OperateResult<byte[]>> readCore)
        {
            OperateResult<McAddressData> melsecFrom = McAddressData.ParseMelsecFrom(address, length);
            if (!melsecFrom.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)melsecFrom);
            byte[] numArray1 = MelsecHelper.BuildAsciiReadMcCoreCommand(melsecFrom.Content, false);
            OperateResult<byte[]> operateResult = readCore(numArray1, station);
            if (!operateResult.IsSuccess)
                return operateResult;
            if (operateResult.Content[0] != (byte)2)
                return new OperateResult<byte[]>((int)operateResult.Content[0], "Read Faild:" + Encoding.ASCII.GetString(operateResult.Content, 1, operateResult.Content.Length - 1));
            byte[] numArray2 = new byte[(int)length * 2];
            for (int index = 0; index < numArray2.Length / 2; ++index)
                BitConverter.GetBytes(Convert.ToUInt16(Encoding.ASCII.GetString(operateResult.Content, index * 4 + 11, 4), 16)).CopyTo((Array)numArray2, index * 2);
            return OperateResult.CreateSuccessResult<byte[]>(numArray2);
        }

        /// <summary>
        /// 批量写入PLC的数据，以字为单位，也就是说最少2个字节信息，支持X,Y,M,S,D,T,C，具体的地址范围需要根据PLC型号来确认
        /// </summary>
        /// <param name="address">地址信息</param>
        /// <param name="value">数据值</param>
        /// <param name="station">当前PLC的站号信息</param>
        /// <param name="readCore">通信的载体信息</param>
        /// <returns>是否写入成功</returns>
        public static OperateResult WriteHelper(
          string address,
          byte[] value,
          byte station,
          Func<byte[], byte, OperateResult<byte[]>> readCore)
        {
            OperateResult<McAddressData> melsecFrom = McAddressData.ParseMelsecFrom(address, (ushort)0);
            if (!melsecFrom.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<byte[]>((OperateResult)melsecFrom);
            byte[] numArray = MelsecHelper.BuildAsciiWriteWordCoreCommand(melsecFrom.Content, value);
            OperateResult<byte[]> operateResult = readCore(numArray, station);
            if (!operateResult.IsSuccess)
                return (OperateResult)operateResult;
            return operateResult.Content[0] != (byte)6 ? new OperateResult((int)operateResult.Content[0], "Write Faild:" + Encoding.ASCII.GetString(operateResult.Content, 1, operateResult.Content.Length - 1)) : OperateResult.CreateSuccessResult();
        }

        /// <summary>批量读取bool类型数据，支持的类型为X,Y,S,T,C，具体的地址范围取决于PLC的类型</summary>
        /// <param name="address">地址信息，比如X10,Y17，注意X，Y的地址是8进制的</param>
        /// <param name="length">读取的长度</param>
        /// <param name="station">当前PLC的站号信息</param>
        /// <param name="readCore">通信的载体信息</param>
        /// <returns>读取结果信息</returns>
        public static OperateResult<bool[]> ReadBoolHelper(
          string address,
          ushort length,
          byte station,
          Func<byte[], byte, OperateResult<byte[]>> readCore)
        {
            OperateResult<McAddressData> melsecFrom = McAddressData.ParseMelsecFrom(address, length);
            if (!melsecFrom.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)melsecFrom);
            byte[] numArray1 = MelsecHelper.BuildAsciiReadMcCoreCommand(melsecFrom.Content, true);
            OperateResult<byte[]> operateResult = readCore(numArray1, station);
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult);
            if (operateResult.Content[0] != (byte)2)
                return new OperateResult<bool[]>((int)operateResult.Content[0], "Read Faild:" + Encoding.ASCII.GetString(operateResult.Content, 1, operateResult.Content.Length - 1));
            byte[] numArray2 = new byte[(int)length];
            Array.Copy((Array)operateResult.Content, 11, (Array)numArray2, 0, (int)length);
            return OperateResult.CreateSuccessResult<bool[]>(((IEnumerable<byte>)numArray2).Select<byte, bool>((Func<byte, bool>)(m => m == (byte)49)).ToArray<bool>());
        }

        /// <summary>批量写入bool类型的数组，支持的类型为X,Y,S,T,C，具体的地址范围取决于PLC的类型</summary>
        /// <param name="address">PLC的地址信息</param>
        /// <param name="value">数据信息</param>
        /// <param name="station">当前PLC的站号信息</param>
        /// <param name="readCore">通信的载体信息</param>
        /// <returns>是否写入成功</returns>
        public static OperateResult WriteHelper(
          string address,
          bool[] value,
          byte station,
          Func<byte[], byte, OperateResult<byte[]>> readCore)
        {
            OperateResult<McAddressData> melsecFrom = McAddressData.ParseMelsecFrom(address, (ushort)0);
            if (!melsecFrom.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<bool[]>((OperateResult)melsecFrom);
            byte[] numArray = MelsecHelper.BuildAsciiWriteBitCoreCommand(melsecFrom.Content, value);
            OperateResult<byte[]> operateResult = readCore(numArray, station);
            if (!operateResult.IsSuccess)
                return (OperateResult)operateResult;
            return operateResult.Content[0] != (byte)6 ? new OperateResult((int)operateResult.Content[0], "Write Faild:" + Encoding.ASCII.GetString(operateResult.Content, 1, operateResult.Content.Length - 1)) : OperateResult.CreateSuccessResult();
        }

        /// <summary>远程Run操作</summary>
        /// <param name="station">当前PLC的站号信息</param>
        /// <param name="readCore">通信的载体信息</param>
        /// <returns>是否成功</returns>
        public static OperateResult RemoteRunHelper(
          byte station,
          Func<byte[], byte, OperateResult<byte[]>> readCore)
        {
            OperateResult<byte[]> operateResult = readCore(Encoding.ASCII.GetBytes("1001000000010000"), station);
            if (!operateResult.IsSuccess)
                return (OperateResult)operateResult;
            return operateResult.Content[0] != (byte)6 && operateResult.Content[0] != (byte)2 ? new OperateResult((int)operateResult.Content[0], "Faild:" + Encoding.ASCII.GetString(operateResult.Content, 1, operateResult.Content.Length - 1)) : OperateResult.CreateSuccessResult();
        }

        /// <summary>远程Stop操作</summary>
        /// <param name="station">当前PLC的站号信息</param>
        /// <param name="readCore">通信的载体信息</param>
        /// <returns>是否成功</returns>
        public static OperateResult RemoteStopHelper(
          byte station,
          Func<byte[], byte, OperateResult<byte[]>> readCore)
        {
            OperateResult<byte[]> operateResult = readCore(Encoding.ASCII.GetBytes("100200000001"), station);
            if (!operateResult.IsSuccess)
                return (OperateResult)operateResult;
            return operateResult.Content[0] != (byte)6 && operateResult.Content[0] != (byte)2 ? new OperateResult((int)operateResult.Content[0], "Faild:" + Encoding.ASCII.GetString(operateResult.Content, 1, operateResult.Content.Length - 1)) : OperateResult.CreateSuccessResult();
        }

        /// <summary>读取PLC的型号信息</summary>
        /// <param name="station">当前PLC的站号信息</param>
        /// <param name="readCore">通信的载体信息</param>
        /// <returns>返回型号的结果对象</returns>
        public static OperateResult<string> ReadPlcTypeHelper(
          byte station,
          Func<byte[], byte, OperateResult<byte[]>> readCore)
        {
            OperateResult<byte[]> operateResult = readCore(Encoding.ASCII.GetBytes("01010000"), station);
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<string>((OperateResult)operateResult);
            return operateResult.Content[0] != (byte)6 && operateResult.Content[0] != (byte)2 ? new OperateResult<string>((int)operateResult.Content[0], "Faild:" + Encoding.ASCII.GetString(operateResult.Content, 1, operateResult.Content.Length - 1)) : OperateResult.CreateSuccessResult<string>(Encoding.ASCII.GetString(operateResult.Content, 11, 16).TrimEnd());
        }

        /// <summary>将命令进行打包传送</summary>
        /// <param name="mcCommand">mc协议的命令</param>
        /// <param name="station">PLC的站号</param>
        /// <returns>最终的原始报文信息</returns>
        public static byte[] PackCommand(byte[] mcCommand, byte station = 0)
        {
            byte[] numArray = new byte[13 + mcCommand.Length];
            numArray[0] = (byte)5;
            numArray[1] = (byte)70;
            numArray[2] = (byte)57;
            numArray[3] = SoftBasic.BuildAsciiBytesFrom(station)[0];
            numArray[4] = SoftBasic.BuildAsciiBytesFrom(station)[1];
            numArray[5] = (byte)48;
            numArray[6] = (byte)48;
            numArray[7] = (byte)70;
            numArray[8] = (byte)70;
            numArray[9] = (byte)48;
            numArray[10] = (byte)48;
            mcCommand.CopyTo((Array)numArray, 11);
            int num = 0;
            for (int index = 1; index < numArray.Length - 3; ++index)
                num += (int)numArray[index];
            numArray[numArray.Length - 2] = SoftBasic.BuildAsciiBytesFrom((byte)num)[0];
            numArray[numArray.Length - 1] = SoftBasic.BuildAsciiBytesFrom((byte)num)[1];
            return numArray;
        }
    }
}
