// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Omron.OmronConnectedCipNet
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;
using ESTCore.Common.Core;
using ESTCore.Common.Core.IMessage;
using ESTCore.Common.Core.Net;
using ESTCore.Common.Profinet.AllenBradley;
using ESTCore.Common.Reflection;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ESTCore.Common.Profinet.Omron
{
    /// <summary>
    /// 基于连接的对象访问的CIP协议的实现，用于对Omron PLC进行标签的数据读写，对数组，多维数组进行读写操作，支持的数据类型请参照API文档手册。<br />
    /// The implementation of the CIP protocol based on connected object access is used to read and write tag data to Omron PLC,
    /// and read and write arrays and multidimensional arrays. For the supported data types, please refer to the API documentation manual.
    /// </summary>
    /// <remarks>
    /// 支持普通标签的读写，类型要和标签对应上。如果标签是数组，例如 A 是 INT[0...9] 那么Read("A", 1)，返回的是10个short所有字节数组。
    /// 如果需要返回10个长度的short数组，请调用 ReadInt16("A[0], 10"); 地址必须写 "A[0]"，不能写 "A" , 如需要读取结构体，参考 <see cref="M:ESTCore.Common.Profinet.Omron.OmronConnectedCipNet.ReadStruct``1(System.String)" />
    /// </remarks>
    /// <example>
    /// 首先说明支持的类型地址，在PLC里支持了大量的类型，有些甚至在C#里是不存在的。现在做个统一的声明
    /// <list type="table">
    ///   <listheader>
    ///     <term>PLC类型</term>
    ///     <term>含义</term>
    ///     <term>代号</term>
    ///     <term>C# 类型</term>
    ///     <term>备注</term>
    ///   </listheader>
    ///   <item>
    ///     <term>bool</term>
    ///     <term>位类型数据</term>
    ///     <term>0xC1</term>
    ///     <term>bool</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>SINT</term>
    ///     <term>8位的整型</term>
    ///     <term>0xC2</term>
    ///     <term>sbyte</term>
    ///     <term>有符号8位很少用，HSL直接用byte</term>
    ///   </item>
    ///   <item>
    ///     <term>USINT</term>
    ///     <term>无符号8位的整型</term>
    ///     <term>0xC6</term>
    ///     <term>byte</term>
    ///     <term>如需要，使用<see cref="M:ESTCore.Common.Profinet.Omron.OmronConnectedCipNet.WriteTag(System.String,System.UInt16,System.Byte[],System.Int32)" />实现</term>
    ///   </item>
    ///   <item>
    ///     <term>BYTE</term>
    ///     <term>8位字符数据</term>
    ///     <term>0xD1</term>
    ///     <term>byte</term>
    ///     <term>如需要，使用<see cref="M:ESTCore.Common.Profinet.Omron.OmronConnectedCipNet.WriteTag(System.String,System.UInt16,System.Byte[],System.Int32)" />实现</term>
    ///   </item>
    ///   <item>
    ///     <term>INT</term>
    ///     <term>16位的整型</term>
    ///     <term>0xC3</term>
    ///     <term>short</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>UINT</term>
    ///     <term>无符号的16位整型</term>
    ///     <term>0xC7</term>
    ///     <term>ushort</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>DINT</term>
    ///     <term>32位的整型</term>
    ///     <term>0xC4</term>
    ///     <term>int</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>UDINT</term>
    ///     <term>无符号的32位整型</term>
    ///     <term>0xC8</term>
    ///     <term>uint</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>LINT</term>
    ///     <term>64位的整型</term>
    ///     <term>0xC5</term>
    ///     <term>long</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>ULINT</term>
    ///     <term>无符号的64位的整型</term>
    ///     <term>0xC9</term>
    ///     <term>ulong</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>REAL</term>
    ///     <term>单精度浮点数</term>
    ///     <term>0xCA</term>
    ///     <term>float</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>DOUBLE</term>
    ///     <term>双精度浮点数</term>
    ///     <term>0xCB</term>
    ///     <term>double</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>STRING</term>
    ///     <term>字符串数据</term>
    ///     <term>0xD0</term>
    ///     <term>string</term>
    ///     <term>前两个字节为字符长度</term>
    ///   </item>
    ///   <item>
    ///     <term>8bit string BYTE</term>
    ///     <term>8位的字符串</term>
    ///     <term>0xD1</term>
    ///     <term></term>
    ///     <term>本质是BYTE数组</term>
    ///   </item>
    ///   <item>
    ///     <term>16bit string WORD</term>
    ///     <term>16位的字符串</term>
    ///     <term>0xD2</term>
    ///     <term></term>
    ///     <term>本质是WORD数组，可存放中文</term>
    ///   </item>
    ///   <item>
    ///     <term>32bit string DWORD</term>
    ///     <term>32位的字符串</term>
    ///     <term>0xD2</term>
    ///     <term></term>
    ///     <term>本质是DWORD数组，可存放中文</term>
    ///   </item>
    /// </list>
    /// 在读写操作之前，先看看怎么实例化和连接PLC<br />
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\OmronConnectedCipNetSample.cs" region="Usage" title="实例化及连接示例" />
    /// 现在来说明以下具体的操作细节。我们假设有如下的变量：<br />
    /// CESHI_A       SINT<br />
    /// CESHI_B       BYTE<br />
    /// CESHI_C       INT<br />
    /// CESHI_D       UINT<br />
    /// CESHI_E       SINT[0..9]<br />
    /// CESHI_F       BYTE[0..9]<br />
    /// CESHI_G       INT[0..9]<br />
    /// CESHI_H       UINT[0..9]<br />
    /// CESHI_I       INT[0..511]<br />
    /// CESHI_J       STRING[12]<br />
    /// ToPc_ID1      ARRAY[0..99] OF STRING[20]<br />
    /// CESHI_O       BOOL<br />
    /// CESHI_P       BOOL[0..31]<br />
    /// 对 CESHI_A 来说，读写这么操作
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\OmronConnectedCipNetSample.cs" region="Usage2" title="读写示例" />
    /// 对于 CESHI_B 来说，写入的操作有点特殊
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\OmronConnectedCipNetSample.cs" region="Usage3" title="读写示例" />
    /// 对于 CESHI_C, CESHI_D 来说，就是 ReadInt16(string address) , Write( string address, short value ) 和 ReadUInt16(string address) 和 Write( string address, ushort value ) 差别不大。
    /// 所以我们着重来看看数组的情况，以 CESHI_G 标签为例子:<br />
    /// 情况一，我想一次读取这个标签所有的字节数组（当长度满足的情况下，会一次性返回数据）
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\OmronConnectedCipNetSample.cs" region="Usage4" title="读写示例" />
    /// 情况二，我想读取第3个数，或是第6个数开始，一共5个数
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\OmronConnectedCipNetSample.cs" region="Usage5" title="读写示例" />
    /// 其他的数组情况都是类似的，我们来看看字符串 CESHI_J 变量
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\OmronConnectedCipNetSample.cs" region="Usage6" title="读写示例" />
    /// 对于 bool 变量来说，就是 ReadBool("CESHI_O") 和 Write("CESHI_O", true) 操作，如果是bool数组，就不一样了
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\OmronConnectedCipNetSample.cs" region="Usage7" title="读写示例" />
    /// 最后我们来看看结构体的操作，假设我们有个结构体<br />
    /// MyData.Code     STRING(12)<br />
    /// MyData.Value1   INT<br />
    /// MyData.Value2   INT<br />
    /// MyData.Value3   REAL<br />
    /// MyData.Value4   INT<br />
    /// MyData.Value5   INT<br />
    /// MyData.Value6   INT[0..3]<br />
    /// 因为bool比较复杂，暂时不考虑。要读取上述的结构体，我们需要定义结构一样的数据
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\OmronConnectedCipNetSample.cs" region="Usage8" title="结构体" />
    /// 定义好后，我们再来读取就很简单了。
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\OmronConnectedCipNetSample.cs" region="Usage9" title="读写示例" />
    /// </example>
    public class OmronConnectedCipNet : NetworkDeviceBase
    {
        /// <summary>O -&gt; T Network Connection ID</summary>
        private uint OTConnectionId = 0;
        private SoftIncrementCount incrementCount = new SoftIncrementCount((long)ushort.MaxValue);
        private Random random = new Random();

        /// <summary>实例化一个默认的对象</summary>
        public OmronConnectedCipNet()
        {
            this.WordLength = (ushort)2;
            this.ByteTransform = (IByteTransform)new RegularByteTransform();
        }

        /// <summary>根据指定的IP及端口来实例化这个连接对象</summary>
        /// <param name="ipAddress">PLC的Ip地址</param>
        /// <param name="port">PLC的端口号信息</param>
        public OmronConnectedCipNet(string ipAddress, int port = 44818)
        {
            this.IpAddress = ipAddress;
            this.Port = port;
            this.WordLength = (ushort)2;
            this.ByteTransform = (IByteTransform)new RegularByteTransform();
        }

        /// <inheritdoc />
        protected override INetMessage GetNewNetMessage() => (INetMessage)new AllenBradleyMessage();

        /// <inheritdoc />
        protected override byte[] PackCommandWithHeader(byte[] command) => AllenBradleyHelper.PackRequestHeader((ushort)112, this.SessionHandle, AllenBradleyHelper.PackCommandSpecificData(this.GetOTConnectionIdService(), command));

        /// <inheritdoc />
        protected override OperateResult InitializationOnConnect(Socket socket)
        {
            OperateResult<byte[]> operateResult1 = this.ReadFromCoreServer(socket, AllenBradleyHelper.RegisterSessionHandle(), usePackHeader: false);
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            OperateResult operateResult2 = AllenBradleyHelper.CheckResponse(operateResult1.Content);
            if (!operateResult2.IsSuccess)
                return operateResult2;
            this.SessionHandle = this.ByteTransform.TransUInt32(operateResult1.Content, 4);
            OperateResult<byte[]> operateResult3 = this.ReadFromCoreServer(socket, AllenBradleyHelper.PackRequestHeader((ushort)111, this.SessionHandle, this.GetLargeForwardOpen()), usePackHeader: false);
            if (!operateResult3.IsSuccess)
                return (OperateResult)operateResult3;
            if (operateResult3.Content[42] > (byte)0)
                return this.ByteTransform.TransUInt16(operateResult3.Content, 44) == (ushort)256 ? new OperateResult("Connection in use or duplicate Forward Open") : new OperateResult("Forward Open failed, Code: " + this.ByteTransform.TransUInt16(operateResult3.Content, 44).ToString());
            this.OTConnectionId = this.ByteTransform.TransUInt32(operateResult3.Content, 44);
            this.incrementCount.ResetCurrentValue();
            OperateResult<byte[]> operateResult4 = this.ReadFromCoreServer(socket, AllenBradleyHelper.PackRequestHeader((ushort)111, this.SessionHandle, this.GetAttributeAll()), usePackHeader: false);
            if (!operateResult4.IsSuccess)
                return (OperateResult)operateResult4;
            if (operateResult4.Content.Length > 59)
                this.ProductName = Encoding.UTF8.GetString(operateResult4.Content, 59, (int)operateResult4.Content[58]);
            return OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc />
        protected override OperateResult ExtraOnDisconnect(Socket socket)
        {
            OperateResult<byte[]> operateResult = this.ReadFromCoreServer(socket, AllenBradleyHelper.UnRegisterSessionHandle(this.SessionHandle), usePackHeader: false);
            return !operateResult.IsSuccess ? (OperateResult)operateResult : OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc />
        protected override async Task<OperateResult> InitializationOnConnectAsync(
          Socket socket)
        {
            OperateResult<byte[]> read1 = await this.ReadFromCoreServerAsync(socket, AllenBradleyHelper.RegisterSessionHandle(), usePackHeader: false);
            if (!read1.IsSuccess)
                return (OperateResult)read1;
            OperateResult check = AllenBradleyHelper.CheckResponse(read1.Content);
            if (!check.IsSuccess)
                return check;
            this.SessionHandle = this.ByteTransform.TransUInt32(read1.Content, 4);
            OperateResult<byte[]> read2 = await this.ReadFromCoreServerAsync(socket, AllenBradleyHelper.PackRequestHeader((ushort)111, this.SessionHandle, this.GetLargeForwardOpen()), usePackHeader: false);
            if (!read2.IsSuccess)
                return (OperateResult)read2;
            if (read2.Content[42] > (byte)0)
                return this.ByteTransform.TransUInt16(read2.Content, 44) != (ushort)256 ? new OperateResult("Forward Open failed, Code: " + this.ByteTransform.TransUInt16(read2.Content, 44).ToString()) : new OperateResult("Connection in use or duplicate Forward Open");
            this.OTConnectionId = this.ByteTransform.TransUInt32(read2.Content, 44);
            this.incrementCount.ResetCurrentValue();
            OperateResult<byte[]> read3 = await this.ReadFromCoreServerAsync(socket, AllenBradleyHelper.PackRequestHeader((ushort)111, this.SessionHandle, this.GetAttributeAll()), usePackHeader: false);
            if (!read3.IsSuccess)
                return (OperateResult)read3;
            if (read3.Content.Length > 59)
                this.ProductName = Encoding.UTF8.GetString(read3.Content, 59, (int)read3.Content[58]);
            return OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc />
        protected override async Task<OperateResult> ExtraOnDisconnectAsync(
          Socket socket)
        {
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(socket, AllenBradleyHelper.UnRegisterSessionHandle(this.SessionHandle), usePackHeader: false);
            OperateResult operateResult = read.IsSuccess ? OperateResult.CreateSuccessResult() : (OperateResult)read;
            read = (OperateResult<byte[]>)null;
            return operateResult;
        }

        /// <inheritdoc cref="P:ESTCore.Common.Profinet.AllenBradley.AllenBradleyNet.SessionHandle" />
        public uint SessionHandle { get; protected set; }

        /// <summary>
        /// 当前产品的型号信息<br />
        /// Model information of the current product
        /// </summary>
        public string ProductName { get; private set; }

        private byte[] GetOTConnectionIdService()
        {
            byte[] numArray = new byte[8]
            {
        (byte) 161,
        (byte) 0,
        (byte) 4,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
            };
            this.ByteTransform.TransByte(this.OTConnectionId).CopyTo((Array)numArray, 4);
            return numArray;
        }

        private OperateResult<byte[]> BuildReadCommand(string[] address, ushort[] length)
        {
            try
            {
                List<byte[]> numArrayList = new List<byte[]>();
                for (int index = 0; index < address.Length; ++index)
                    numArrayList.Add(AllenBradleyHelper.PackRequsetRead(address[index], (int)length[index], true));
                return OperateResult.CreateSuccessResult<byte[]>(this.PackCommandService(numArrayList.ToArray()));
            }
            catch (Exception ex)
            {
                return new OperateResult<byte[]>("Address Wrong:" + ex.Message);
            }
        }

        private OperateResult<byte[]> BuildWriteCommand(
          string address,
          ushort typeCode,
          byte[] data,
          int length = 1)
        {
            try
            {
                return OperateResult.CreateSuccessResult<byte[]>(this.PackCommandService(AllenBradleyHelper.PackRequestWrite(address, typeCode, data, length, true)));
            }
            catch (Exception ex)
            {
                return new OperateResult<byte[]>("Address Wrong:" + ex.Message);
            }
        }

        private byte[] PackCommandService(params byte[][] cip)
        {
            MemoryStream memoryStream = new MemoryStream();
            memoryStream.WriteByte((byte)177);
            memoryStream.WriteByte((byte)0);
            memoryStream.WriteByte((byte)0);
            memoryStream.WriteByte((byte)0);
            long currentValue = this.incrementCount.GetCurrentValue();
            memoryStream.WriteByte(BitConverter.GetBytes(currentValue)[0]);
            memoryStream.WriteByte(BitConverter.GetBytes(currentValue)[1]);
            if (cip.Length == 1)
            {
                memoryStream.Write(cip[0], 0, cip[0].Length);
            }
            else
            {
                memoryStream.Write(new byte[6]
                {
          (byte) 10,
          (byte) 2,
          (byte) 32,
          (byte) 2,
          (byte) 36,
          (byte) 1
                }, 0, 6);
                memoryStream.WriteByte(BitConverter.GetBytes(cip.Length)[0]);
                memoryStream.WriteByte(BitConverter.GetBytes(cip.Length)[1]);
                int num = 2 + cip.Length * 2;
                for (int index = 0; index < cip.Length; ++index)
                {
                    memoryStream.WriteByte(BitConverter.GetBytes(num)[0]);
                    memoryStream.WriteByte(BitConverter.GetBytes(num)[1]);
                    num += cip[index].Length;
                }
                for (int index = 0; index < cip.Length; ++index)
                    memoryStream.Write(cip[index], 0, cip[index].Length);
            }
            byte[] array = memoryStream.ToArray();
            memoryStream.Dispose();
            BitConverter.GetBytes((ushort)(array.Length - 4)).CopyTo((Array)array, 2);
            return array;
        }

        private OperateResult<byte[], ushort, bool> ReadWithType(
          string[] address,
          ushort[] length)
        {
            OperateResult<byte[]> operateResult1 = this.BuildReadCommand(address, length);
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<byte[], ushort, bool>((OperateResult)operateResult1);
            OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
            if (!operateResult2.IsSuccess)
                return OperateResult.CreateFailedResult<byte[], ushort, bool>((OperateResult)operateResult2);
            OperateResult result = AllenBradleyHelper.CheckResponse(operateResult2.Content);
            return !result.IsSuccess ? OperateResult.CreateFailedResult<byte[], ushort, bool>(result) : OmronConnectedCipNet.ExtractActualData(operateResult2.Content, true);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.AllenBradley.AllenBradleyNet.ReadCipFromServer(System.Byte[][])" />
        public OperateResult<byte[]> ReadCipFromServer(params byte[][] cips)
        {
            OperateResult<byte[]> operateResult = this.ReadFromCoreServer(this.PackCommandService(((IEnumerable<byte[]>)cips).ToArray<byte[]>()));
            if (!operateResult.IsSuccess)
                return operateResult;
            OperateResult result = AllenBradleyHelper.CheckResponse(operateResult.Content);
            return !result.IsSuccess ? OperateResult.CreateFailedResult<byte[]>(result) : OperateResult.CreateSuccessResult<byte[]>(operateResult.Content);
        }

        /// <summary>
        /// <b>[商业授权]</b> 读取一个结构体的对象，需要事先根据实际的数据点位定义好结构体，然后使用本方法进行读取，当结构体定义不对时，本方法将会读取失败<br />
        /// <b>[Authorization]</b> To read a structure object, you need to define the structure in advance according to the actual data points,
        /// and then use this method to read. When the structure definition is incorrect, this method will fail to read
        /// </summary>
        /// <remarks>本方法需要商业授权支持，具体的使用方法，参考API文档的示例代码</remarks>
        /// <example>
        /// 我们来看看结构体的操作，假设我们有个结构体<br />
        /// MyData.Code     STRING(12)<br />
        /// MyData.Value1   INT<br />
        /// MyData.Value2   INT<br />
        /// MyData.Value3   REAL<br />
        /// MyData.Value4   INT<br />
        /// MyData.Value5   INT<br />
        /// MyData.Value6   INT[0..3]<br />
        /// 因为bool比较复杂，暂时不考虑。要读取上述的结构体，我们需要定义结构一样的数据
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\OmronConnectedCipNetSample.cs" region="Usage8" title="结构体" />
        /// 定义好后，我们再来读取就很简单了。
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\OmronConnectedCipNetSample.cs" region="Usage9" title="读写示例" />
        /// </example>
        /// <typeparam name="T">结构体的类型</typeparam>
        /// <param name="address">结构体对象的地址</param>
        /// <returns>是否读取成功的对象</returns>
        public OperateResult<T> ReadStruct<T>(string address) where T : struct
        {
            OperateResult<byte[]> operateResult = this.Read(address, (ushort)1);
            return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<T>((OperateResult)operateResult) : EstHelper.ByteArrayToStruct<T>(operateResult.Content.RemoveBegin<byte>(2));
        }

        private async Task<OperateResult<byte[], ushort, bool>> ReadWithTypeAsync(
          string[] address,
          ushort[] length)
        {
            OperateResult<byte[]> command = this.BuildReadCommand(address, length);
            if (!command.IsSuccess)
                return OperateResult.CreateFailedResult<byte[], ushort, bool>((OperateResult)command);
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
            if (!read.IsSuccess)
                return OperateResult.CreateFailedResult<byte[], ushort, bool>((OperateResult)read);
            OperateResult check = AllenBradleyHelper.CheckResponse(read.Content);
            return check.IsSuccess ? OmronConnectedCipNet.ExtractActualData(read.Content, true) : OperateResult.CreateFailedResult<byte[], ushort, bool>(check);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronConnectedCipNet.ReadCipFromServer(System.Byte[][])" />
        public async Task<OperateResult<byte[]>> ReadCipFromServerAsync(
          params byte[][] cips)
        {
            byte[] command = this.PackCommandService(((IEnumerable<byte[]>)cips).ToArray<byte[]>());
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command);
            if (!read.IsSuccess)
                return read;
            OperateResult check = AllenBradleyHelper.CheckResponse(read.Content);
            return check.IsSuccess ? OperateResult.CreateSuccessResult<byte[]>(read.Content) : OperateResult.CreateFailedResult<byte[]>(check);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronConnectedCipNet.ReadStruct``1(System.String)" />
        public async Task<OperateResult<T>> ReadStructAsync<T>(string address) where T : struct
        {
            OperateResult<byte[]> read = await this.ReadAsync(address, (ushort)1);
            OperateResult<T> operateResult = read.IsSuccess ? EstHelper.ByteArrayToStruct<T>(read.Content.RemoveBegin<byte>(2)) : OperateResult.CreateFailedResult<T>((OperateResult)read);
            read = (OperateResult<byte[]>)null;
            return operateResult;
        }

        /// <inheritdoc />
        [EstMqttApi("ReadByteArray", "")]
        public override OperateResult<byte[]> Read(string address, ushort length)
        {
            OperateResult<byte[], ushort, bool> operateResult = this.ReadWithType(new string[1]
            {
        address
            }, new ushort[1] { length });
            return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult) : OperateResult.CreateSuccessResult<byte[]>(operateResult.Content1);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.AllenBradley.AllenBradleyNet.Read(System.String[],System.Int32[])" />
        [EstMqttApi("ReadMultiAddress", "")]
        public OperateResult<byte[]> Read(string[] address, ushort[] length)
        {
            if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
                return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
            OperateResult<byte[], ushort, bool> operateResult = this.ReadWithType(address, length);
            return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult) : OperateResult.CreateSuccessResult<byte[]>(operateResult.Content1);
        }

        /// <summary>
        /// 读取bool数据信息，如果读取的是单bool变量，就直接写变量名，如果是 bool 数组，就 <br />
        /// Read a single bool data information, if it is a single bool variable, write the variable name directly,
        /// if it is a value of a bool array composed of int, it is always accessed with "i=" at the beginning, for example, "i=A[0]"
        /// </summary>
        /// <param name="address">节点的名称 -&gt; Name of the node </param>
        /// <param name="length">读取的数组长度信息</param>
        /// <returns>带有结果对象的结果数据 -&gt; Result data with result info </returns>
        [EstMqttApi("ReadBoolArray", "")]
        public override OperateResult<bool[]> ReadBool(string address, ushort length)
        {
            if (length == (ushort)1 && !Regex.IsMatch(address, "\\[[0-9]+\\]$"))
            {
                OperateResult<byte[]> operateResult = this.Read(address, length);
                return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult) : OperateResult.CreateSuccessResult<bool[]>(SoftBasic.ByteToBoolArray(operateResult.Content));
            }
            OperateResult<byte[]> operateResult1 = this.Read(address, length);
            return !operateResult1.IsSuccess ? OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult1) : OperateResult.CreateSuccessResult<bool[]>(((IEnumerable<byte>)operateResult1.Content).Select<byte, bool>((Func<byte, bool>)(m => m > (byte)0)).Take<bool>((int)length).ToArray<bool>());
        }

        /// <summary>
        /// 读取PLC的byte类型的数据<br />
        /// Read the byte type of PLC data
        /// </summary>
        /// <param name="address">节点的名称 -&gt; Name of the node </param>
        /// <returns>带有结果对象的结果数据 -&gt; Result data with result info </returns>
        [EstMqttApi("ReadByte", "")]
        public OperateResult<byte> ReadByte(string address) => ByteTransformHelper.GetResultFromArray<byte>(this.Read(address, (ushort)1));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronConnectedCipNet.ReadBool(System.String,System.UInt16)" />
        public override async Task<OperateResult<bool[]>> ReadBoolAsync(
          string address,
          ushort length)
        {
            if (length == (ushort)1 && !Regex.IsMatch(address, "\\[[0-9]+\\]$"))
            {
                OperateResult<byte[]> read = await this.ReadAsync(address, length);
                return read.IsSuccess ? OperateResult.CreateSuccessResult<bool[]>(SoftBasic.ByteToBoolArray(read.Content)) : OperateResult.CreateFailedResult<bool[]>((OperateResult)read);
            }
            OperateResult<byte[]> read1 = await this.ReadAsync(address, length);
            return read1.IsSuccess ? OperateResult.CreateSuccessResult<bool[]>(((IEnumerable<byte>)read1.Content).Select<byte, bool>((Func<byte, bool>)(m => m > (byte)0)).Take<bool>((int)length).ToArray<bool>()) : OperateResult.CreateFailedResult<bool[]>((OperateResult)read1);
        }

        /// <inheritdoc />
        public override async Task<OperateResult<byte[]>> ReadAsync(
          string address,
          ushort length)
        {
            OperateResult<byte[], ushort, bool> read = await this.ReadWithTypeAsync(new string[1]
            {
        address
            }, new ushort[1] { length });
            OperateResult<byte[]> operateResult = read.IsSuccess ? OperateResult.CreateSuccessResult<byte[]>(read.Content1) : OperateResult.CreateFailedResult<byte[]>((OperateResult)read);
            read = (OperateResult<byte[], ushort, bool>)null;
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronConnectedCipNet.Read(System.String[],System.UInt16[])" />
        public async Task<OperateResult<byte[]>> ReadAsync(
          string[] address,
          ushort[] length)
        {
            if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
                return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
            OperateResult<byte[], ushort, bool> read = await this.ReadWithTypeAsync(address, length);
            return read.IsSuccess ? OperateResult.CreateSuccessResult<byte[]>(read.Content1) : OperateResult.CreateFailedResult<byte[]>((OperateResult)read);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronConnectedCipNet.ReadByte(System.String)" />
        public async Task<OperateResult<byte>> ReadByteAsync(string address)
        {
            OperateResult<byte[]> result = await this.ReadAsync(address, (ushort)1);
            return ByteTransformHelper.GetResultFromArray<byte>(result);
        }

        /// <summary>
        /// 当前的PLC不支持该功能，需要调用 <see cref="M:ESTCore.Common.Profinet.Omron.OmronConnectedCipNet.WriteTag(System.String,System.UInt16,System.Byte[],System.Int32)" /> 方法来实现。<br />
        /// The current PLC does not support this function, you need to call the <see cref="M:ESTCore.Common.Profinet.Omron.OmronConnectedCipNet.WriteTag(System.String,System.UInt16,System.Byte[],System.Int32)" /> method to achieve it.
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
        /// <param name="typeCode">类型代码，详细参见<see cref="T:ESTCore.Common.Profinet.AllenBradley.AllenBradleyHelper" />上的常用字段 -&gt;  Type code, see the commonly used Fields section on the <see cref="T:ESTCore.Common.Profinet.AllenBradley.AllenBradleyHelper" /> in detail</param>
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
                return (OperateResult)operateResult1;
            OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
            if (!operateResult2.IsSuccess)
                return (OperateResult)operateResult2;
            OperateResult result = AllenBradleyHelper.CheckResponse(operateResult2.Content);
            return !result.IsSuccess ? (OperateResult)OperateResult.CreateFailedResult<byte[]>(result) : (OperateResult)AllenBradleyHelper.ExtractActualData(operateResult2.Content, false);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronConnectedCipNet.Write(System.String,System.Byte[])" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          byte[] value)
        {
            OperateResult operateResult = await Task.Run<OperateResult>((Func<OperateResult>)(() => this.Write(address, value)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronConnectedCipNet.WriteTag(System.String,System.UInt16,System.Byte[],System.Int32)" />
        public virtual async Task<OperateResult> WriteTagAsync(
          string address,
          ushort typeCode,
          byte[] value,
          int length = 1)
        {
            OperateResult<byte[]> command = this.BuildWriteCommand(address, typeCode, value, length);
            if (!command.IsSuccess)
                return (OperateResult)command;
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
            if (!read.IsSuccess)
                return (OperateResult)read;
            OperateResult check = AllenBradleyHelper.CheckResponse(read.Content);
            return check.IsSuccess ? (OperateResult)AllenBradleyHelper.ExtractActualData(read.Content, false) : (OperateResult)OperateResult.CreateFailedResult<byte[]>(check);
        }

        /// <inheritdoc />
        [EstMqttApi("ReadInt16Array", "")]
        public override OperateResult<short[]> ReadInt16(string address, ushort length) => ByteTransformHelper.GetResultFromBytes<short[]>(this.Read(address, length), (Func<byte[], short[]>)(m => this.ByteTransform.TransInt16(m, 0, (int)length)));

        /// <inheritdoc />
        [EstMqttApi("ReadUInt16Array", "")]
        public override OperateResult<ushort[]> ReadUInt16(string address, ushort length) => ByteTransformHelper.GetResultFromBytes<ushort[]>(this.Read(address, length), (Func<byte[], ushort[]>)(m => this.ByteTransform.TransUInt16(m, 0, (int)length)));

        /// <inheritdoc />
        [EstMqttApi("ReadInt32Array", "")]
        public override OperateResult<int[]> ReadInt32(string address, ushort length) => ByteTransformHelper.GetResultFromBytes<int[]>(this.Read(address, length), (Func<byte[], int[]>)(m => this.ByteTransform.TransInt32(m, 0, (int)length)));

        /// <inheritdoc />
        [EstMqttApi("ReadUInt32Array", "")]
        public override OperateResult<uint[]> ReadUInt32(string address, ushort length) => ByteTransformHelper.GetResultFromBytes<uint[]>(this.Read(address, length), (Func<byte[], uint[]>)(m => this.ByteTransform.TransUInt32(m, 0, (int)length)));

        /// <inheritdoc />
        [EstMqttApi("ReadFloatArray", "")]
        public override OperateResult<float[]> ReadFloat(string address, ushort length) => ByteTransformHelper.GetResultFromBytes<float[]>(this.Read(address, length), (Func<byte[], float[]>)(m => this.ByteTransform.TransSingle(m, 0, (int)length)));

        /// <inheritdoc />
        [EstMqttApi("ReadInt64Array", "")]
        public override OperateResult<long[]> ReadInt64(string address, ushort length) => ByteTransformHelper.GetResultFromBytes<long[]>(this.Read(address, length), (Func<byte[], long[]>)(m => this.ByteTransform.TransInt64(m, 0, (int)length)));

        /// <inheritdoc />
        [EstMqttApi("ReadUInt64Array", "")]
        public override OperateResult<ulong[]> ReadUInt64(string address, ushort length) => ByteTransformHelper.GetResultFromBytes<ulong[]>(this.Read(address, length), (Func<byte[], ulong[]>)(m => this.ByteTransform.TransUInt64(m, 0, (int)length)));

        /// <inheritdoc />
        [EstMqttApi("ReadDoubleArray", "")]
        public override OperateResult<double[]> ReadDouble(string address, ushort length) => ByteTransformHelper.GetResultFromBytes<double[]>(this.Read(address, length), (Func<byte[], double[]>)(m => this.ByteTransform.TransDouble(m, 0, (int)length)));

        /// <inheritdoc />
        public OperateResult<string> ReadString(string address) => this.ReadString(address, (ushort)1, Encoding.UTF8);

        /// <summary>
        /// 读取字符串数据，默认为UTF-8编码<br />
        /// Read string data, default is UTF-8 encoding
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="length">数据长度</param>
        /// <returns>带有成功标识的string数据</returns>
        /// <example>
        /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadString" title="String类型示例" />
        /// </example>
        [EstMqttApi("ReadString", "")]
        public override OperateResult<string> ReadString(string address, ushort length) => this.ReadString(address, length, Encoding.UTF8);

        /// <inheritdoc />
        public override OperateResult<string> ReadString(
          string address,
          ushort length,
          Encoding encoding)
        {
            OperateResult<byte[]> operateResult = this.Read(address, length);
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<string>((OperateResult)operateResult);
            if (operateResult.Content.Length < 2)
                return OperateResult.CreateSuccessResult<string>(encoding.GetString(operateResult.Content));
            int count = (int)this.ByteTransform.TransUInt16(operateResult.Content, 0);
            return OperateResult.CreateSuccessResult<string>(encoding.GetString(operateResult.Content, 2, count));
        }

        /// <inheritdoc />
        public override async Task<OperateResult<short[]>> ReadInt16Async(
          string address,
          ushort length)
        {
            OperateResult<byte[]> result = await this.ReadAsync(address, length);
            return ByteTransformHelper.GetResultFromBytes<short[]>(result, (Func<byte[], short[]>)(m => this.ByteTransform.TransInt16(m, 0, (int)length)));
        }

        /// <inheritdoc />
        public override async Task<OperateResult<ushort[]>> ReadUInt16Async(
          string address,
          ushort length)
        {
            OperateResult<byte[]> result = await this.ReadAsync(address, length);
            return ByteTransformHelper.GetResultFromBytes<ushort[]>(result, (Func<byte[], ushort[]>)(m => this.ByteTransform.TransUInt16(m, 0, (int)length)));
        }

        /// <inheritdoc />
        public override async Task<OperateResult<int[]>> ReadInt32Async(
          string address,
          ushort length)
        {
            OperateResult<byte[]> result = await this.ReadAsync(address, length);
            return ByteTransformHelper.GetResultFromBytes<int[]>(result, (Func<byte[], int[]>)(m => this.ByteTransform.TransInt32(m, 0, (int)length)));
        }

        /// <inheritdoc />
        public override async Task<OperateResult<uint[]>> ReadUInt32Async(
          string address,
          ushort length)
        {
            OperateResult<byte[]> result = await this.ReadAsync(address, length);
            return ByteTransformHelper.GetResultFromBytes<uint[]>(result, (Func<byte[], uint[]>)(m => this.ByteTransform.TransUInt32(m, 0, (int)length)));
        }

        /// <inheritdoc />
        public override async Task<OperateResult<float[]>> ReadFloatAsync(
          string address,
          ushort length)
        {
            OperateResult<byte[]> result = await this.ReadAsync(address, length);
            return ByteTransformHelper.GetResultFromBytes<float[]>(result, (Func<byte[], float[]>)(m => this.ByteTransform.TransSingle(m, 0, (int)length)));
        }

        /// <inheritdoc />
        public override async Task<OperateResult<long[]>> ReadInt64Async(
          string address,
          ushort length)
        {
            OperateResult<byte[]> result = await this.ReadAsync(address, length);
            return ByteTransformHelper.GetResultFromBytes<long[]>(result, (Func<byte[], long[]>)(m => this.ByteTransform.TransInt64(m, 0, (int)length)));
        }

        /// <inheritdoc />
        public override async Task<OperateResult<ulong[]>> ReadUInt64Async(
          string address,
          ushort length)
        {
            OperateResult<byte[]> result = await this.ReadAsync(address, length);
            return ByteTransformHelper.GetResultFromBytes<ulong[]>(result, (Func<byte[], ulong[]>)(m => this.ByteTransform.TransUInt64(m, 0, (int)length)));
        }

        /// <inheritdoc />
        public override async Task<OperateResult<double[]>> ReadDoubleAsync(
          string address,
          ushort length)
        {
            OperateResult<byte[]> result = await this.ReadAsync(address, length);
            return ByteTransformHelper.GetResultFromBytes<double[]>(result, (Func<byte[], double[]>)(m => this.ByteTransform.TransDouble(m, 0, (int)length)));
        }

        /// <inheritdoc />
        public async Task<OperateResult<string>> ReadStringAsync(string address)
        {
            OperateResult<string> operateResult = await this.ReadStringAsync(address, (ushort)1, Encoding.UTF8);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronConnectedCipNet.ReadString(System.String,System.UInt16)" />
        public override async Task<OperateResult<string>> ReadStringAsync(
          string address,
          ushort length)
        {
            OperateResult<string> operateResult = await this.ReadStringAsync(address, length, Encoding.UTF8);
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
                return OperateResult.CreateFailedResult<string>((OperateResult)read);
            if (read.Content.Length < 2)
                return OperateResult.CreateSuccessResult<string>(encoding.GetString(read.Content));
            int strLength = (int)this.ByteTransform.TransUInt16(read.Content, 0);
            return OperateResult.CreateSuccessResult<string>(encoding.GetString(read.Content, 2, strLength));
        }

        /// <inheritdoc />
        [EstMqttApi("WriteInt16Array", "")]
        public override OperateResult Write(string address, short[] values) => this.WriteTag(address, (ushort)195, this.ByteTransform.TransByte(values), values.Length);

        /// <inheritdoc />
        [EstMqttApi("WriteUInt16Array", "")]
        public override OperateResult Write(string address, ushort[] values) => this.WriteTag(address, (ushort)199, this.ByteTransform.TransByte(values), values.Length);

        /// <inheritdoc />
        [EstMqttApi("WriteInt32Array", "")]
        public override OperateResult Write(string address, int[] values) => this.WriteTag(address, (ushort)196, this.ByteTransform.TransByte(values), values.Length);

        /// <inheritdoc />
        [EstMqttApi("WriteUInt32Array", "")]
        public override OperateResult Write(string address, uint[] values) => this.WriteTag(address, (ushort)200, this.ByteTransform.TransByte(values), values.Length);

        /// <inheritdoc />
        [EstMqttApi("WriteFloatArray", "")]
        public override OperateResult Write(string address, float[] values) => this.WriteTag(address, (ushort)202, this.ByteTransform.TransByte(values), values.Length);

        /// <inheritdoc />
        [EstMqttApi("WriteInt64Array", "")]
        public override OperateResult Write(string address, long[] values) => this.WriteTag(address, (ushort)197, this.ByteTransform.TransByte(values), values.Length);

        /// <inheritdoc />
        [EstMqttApi("WriteUInt64Array", "")]
        public override OperateResult Write(string address, ulong[] values) => this.WriteTag(address, (ushort)201, this.ByteTransform.TransByte(values), values.Length);

        /// <inheritdoc />
        [EstMqttApi("WriteDoubleArray", "")]
        public override OperateResult Write(string address, double[] values) => this.WriteTag(address, (ushort)203, this.ByteTransform.TransByte(values), values.Length);

        /// <inheritdoc />
        [EstMqttApi("WriteString", "")]
        public override OperateResult Write(string address, string value)
        {
            byte[] numArray = string.IsNullOrEmpty(value) ? new byte[0] : Encoding.UTF8.GetBytes(value);
            return this.WriteTag(address, (ushort)208, SoftBasic.SpliceArray<byte>(BitConverter.GetBytes((ushort)numArray.Length), numArray));
        }

        /// <inheritdoc />
        [EstMqttApi("WriteBool", "")]
        public override OperateResult Write(string address, bool value)
        {
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
            return this.WriteTag(address1, (ushort)193, numArray);
        }

        /// <inheritdoc />
        [EstMqttApi("WriteByte", "")]
        public OperateResult Write(string address, byte value) => this.WriteTag(address, (ushort)194, new byte[1]
        {
      value
        });

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronConnectedCipNet.Write(System.String,System.Int16[])" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          short[] values)
        {
            OperateResult operateResult = await this.WriteTagAsync(address, (ushort)195, this.ByteTransform.TransByte(values), values.Length);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronConnectedCipNet.Write(System.String,System.UInt16[])" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          ushort[] values)
        {
            OperateResult operateResult = await this.WriteTagAsync(address, (ushort)199, this.ByteTransform.TransByte(values), values.Length);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronConnectedCipNet.Write(System.String,System.Int32[])" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          int[] values)
        {
            OperateResult operateResult = await this.WriteTagAsync(address, (ushort)196, this.ByteTransform.TransByte(values), values.Length);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronConnectedCipNet.Write(System.String,System.UInt32[])" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          uint[] values)
        {
            OperateResult operateResult = await this.WriteTagAsync(address, (ushort)200, this.ByteTransform.TransByte(values), values.Length);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronConnectedCipNet.Write(System.String,System.Single[])" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          float[] values)
        {
            OperateResult operateResult = await this.WriteTagAsync(address, (ushort)202, this.ByteTransform.TransByte(values), values.Length);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronConnectedCipNet.Write(System.String,System.Int64[])" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          long[] values)
        {
            OperateResult operateResult = await this.WriteTagAsync(address, (ushort)197, this.ByteTransform.TransByte(values), values.Length);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronConnectedCipNet.Write(System.String,System.UInt64[])" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          ulong[] values)
        {
            OperateResult operateResult = await this.WriteTagAsync(address, (ushort)201, this.ByteTransform.TransByte(values), values.Length);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronConnectedCipNet.Write(System.String,System.Double[])" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          double[] values)
        {
            OperateResult operateResult = await this.WriteTagAsync(address, (ushort)203, this.ByteTransform.TransByte(values), values.Length);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronConnectedCipNet.Write(System.String,System.String)" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          string value)
        {
            byte[] buffer = string.IsNullOrEmpty(value) ? new byte[0] : Encoding.UTF8.GetBytes(value);
            OperateResult operateResult = await this.WriteTagAsync(address, (ushort)208, SoftBasic.SpliceArray<byte>(BitConverter.GetBytes((ushort)buffer.Length), buffer));
            buffer = (byte[])null;
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronConnectedCipNet.Write(System.String,System.Boolean)" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          bool value)
        {
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
            OperateResult operateResult = await this.WriteTagAsync(address1, (ushort)193, numArray);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronConnectedCipNet.Write(System.String,System.Byte)" />
        public async Task<OperateResult> WriteAsync(string address, byte value)
        {
            OperateResult operateResult = await this.WriteTagAsync(address, (ushort)194, new byte[1]
            {
        value
            });
            return operateResult;
        }

        private byte[] GetLargeForwardOpen() => "00 00 00 00 00 00 02 00 00 00 00 00 b2 00 34 00\r\n5b 02 20 06 24 01 06 9c 02 00 00 80 01 00 fe 80\r\n02 00 1b 05 30 a7 2b 03 02 00 00 00 80 84 1e 00\r\ncc 07 00 42 80 84 1e 00 cc 07 00 42 a3 03 20 02\r\n24 01 2c 01".ToHexBytes();

        private byte[] GetAttributeAll() => "00 00 00 00 00 00 02 00 00 00 00 00 b2 00 06 00 01 02 20 01 24 01".ToHexBytes();

        /// <summary>从PLC反馈的数据解析</summary>
        /// <param name="response">PLC的反馈数据</param>
        /// <param name="isRead">是否是返回的操作</param>
        /// <returns>带有结果标识的最终数据</returns>
        public static OperateResult<byte[], ushort, bool> ExtractActualData(
          byte[] response,
          bool isRead)
        {
            List<byte> byteList = new List<byte>();
            int startIndex1 = 42;
            bool flag = false;
            ushort num1 = 0;
            ushort uint16_1 = BitConverter.ToUInt16(response, startIndex1);
            if (BitConverter.ToInt32(response, 46) == 138)
            {
                int startIndex2 = 50;
                int uint16_2 = (int)BitConverter.ToUInt16(response, startIndex2);
                for (int index1 = 0; index1 < uint16_2; ++index1)
                {
                    int num2 = (int)BitConverter.ToUInt16(response, startIndex2 + 2 + index1 * 2) + startIndex2;
                    int num3 = index1 == uint16_2 - 1 ? response.Length : (int)BitConverter.ToUInt16(response, startIndex2 + 4 + index1 * 2) + startIndex2;
                    ushort uint16_3 = BitConverter.ToUInt16(response, num2 + 2);
                    switch (uint16_3)
                    {
                        case 0:
                            if (isRead)
                            {
                                for (int index2 = num2 + 6; index2 < num3; ++index2)
                                    byteList.Add(response[index2]);
                                continue;
                            }
                            continue;
                        case 4:
                            OperateResult<byte[], ushort, bool> operateResult1 = new OperateResult<byte[], ushort, bool>();
                            operateResult1.ErrorCode = (int)uint16_3;
                            operateResult1.Message = StringResources.Language.AllenBradley04;
                            return operateResult1;
                        case 5:
                            OperateResult<byte[], ushort, bool> operateResult2 = new OperateResult<byte[], ushort, bool>();
                            operateResult2.ErrorCode = (int)uint16_3;
                            operateResult2.Message = StringResources.Language.AllenBradley05;
                            return operateResult2;
                        case 6:
                            if (response[startIndex2 + 2] == (byte)210 || response[startIndex2 + 2] == (byte)204)
                            {
                                OperateResult<byte[], ushort, bool> operateResult3 = new OperateResult<byte[], ushort, bool>();
                                operateResult3.ErrorCode = (int)uint16_3;
                                operateResult3.Message = StringResources.Language.AllenBradley06;
                                return operateResult3;
                            }
                            goto case 0;
                        case 10:
                            OperateResult<byte[], ushort, bool> operateResult4 = new OperateResult<byte[], ushort, bool>();
                            operateResult4.ErrorCode = (int)uint16_3;
                            operateResult4.Message = StringResources.Language.AllenBradley0A;
                            return operateResult4;
                        case 19:
                            OperateResult<byte[], ushort, bool> operateResult5 = new OperateResult<byte[], ushort, bool>();
                            operateResult5.ErrorCode = (int)uint16_3;
                            operateResult5.Message = StringResources.Language.AllenBradley13;
                            return operateResult5;
                        case 28:
                            OperateResult<byte[], ushort, bool> operateResult6 = new OperateResult<byte[], ushort, bool>();
                            operateResult6.ErrorCode = (int)uint16_3;
                            operateResult6.Message = StringResources.Language.AllenBradley1C;
                            return operateResult6;
                        case 30:
                            OperateResult<byte[], ushort, bool> operateResult7 = new OperateResult<byte[], ushort, bool>();
                            operateResult7.ErrorCode = (int)uint16_3;
                            operateResult7.Message = StringResources.Language.AllenBradley1E;
                            return operateResult7;
                        case 38:
                            OperateResult<byte[], ushort, bool> operateResult8 = new OperateResult<byte[], ushort, bool>();
                            operateResult8.ErrorCode = (int)uint16_3;
                            operateResult8.Message = StringResources.Language.AllenBradley26;
                            return operateResult8;
                        default:
                            OperateResult<byte[], ushort, bool> operateResult9 = new OperateResult<byte[], ushort, bool>();
                            operateResult9.ErrorCode = (int)uint16_3;
                            operateResult9.Message = StringResources.Language.UnknownError;
                            return operateResult9;
                    }
                }
            }
            else
            {
                byte num2 = response[startIndex1 + 6];
                switch (num2)
                {
                    case 0:
                        if (response[startIndex1 + 4] == (byte)205 || response[startIndex1 + 4] == (byte)211)
                            return OperateResult.CreateSuccessResult<byte[], ushort, bool>(byteList.ToArray(), num1, flag);
                        if (response[startIndex1 + 4] == (byte)204 || response[startIndex1 + 4] == (byte)210)
                        {
                            for (int index = startIndex1 + 10; index < startIndex1 + 2 + (int)uint16_1; ++index)
                                byteList.Add(response[index]);
                            num1 = BitConverter.ToUInt16(response, startIndex1 + 8);
                        }
                        else if (response[startIndex1 + 4] == (byte)213)
                        {
                            for (int index = startIndex1 + 8; index < startIndex1 + 2 + (int)uint16_1; ++index)
                                byteList.Add(response[index]);
                        }
                        break;
                    case 4:
                        OperateResult<byte[], ushort, bool> operateResult1 = new OperateResult<byte[], ushort, bool>();
                        operateResult1.ErrorCode = (int)num2;
                        operateResult1.Message = StringResources.Language.AllenBradley04;
                        return operateResult1;
                    case 5:
                        OperateResult<byte[], ushort, bool> operateResult2 = new OperateResult<byte[], ushort, bool>();
                        operateResult2.ErrorCode = (int)num2;
                        operateResult2.Message = StringResources.Language.AllenBradley05;
                        return operateResult2;
                    case 6:
                        flag = true;
                        goto case 0;
                    case 10:
                        OperateResult<byte[], ushort, bool> operateResult3 = new OperateResult<byte[], ushort, bool>();
                        operateResult3.ErrorCode = (int)num2;
                        operateResult3.Message = StringResources.Language.AllenBradley0A;
                        return operateResult3;
                    case 19:
                        OperateResult<byte[], ushort, bool> operateResult4 = new OperateResult<byte[], ushort, bool>();
                        operateResult4.ErrorCode = (int)num2;
                        operateResult4.Message = StringResources.Language.AllenBradley13;
                        return operateResult4;
                    case 28:
                        OperateResult<byte[], ushort, bool> operateResult5 = new OperateResult<byte[], ushort, bool>();
                        operateResult5.ErrorCode = (int)num2;
                        operateResult5.Message = StringResources.Language.AllenBradley1C;
                        return operateResult5;
                    case 30:
                        OperateResult<byte[], ushort, bool> operateResult6 = new OperateResult<byte[], ushort, bool>();
                        operateResult6.ErrorCode = (int)num2;
                        operateResult6.Message = StringResources.Language.AllenBradley1E;
                        return operateResult6;
                    case 38:
                        OperateResult<byte[], ushort, bool> operateResult7 = new OperateResult<byte[], ushort, bool>();
                        operateResult7.ErrorCode = (int)num2;
                        operateResult7.Message = StringResources.Language.AllenBradley26;
                        return operateResult7;
                    default:
                        OperateResult<byte[], ushort, bool> operateResult8 = new OperateResult<byte[], ushort, bool>();
                        operateResult8.ErrorCode = (int)num2;
                        operateResult8.Message = StringResources.Language.UnknownError;
                        return operateResult8;
                }
            }
            return OperateResult.CreateSuccessResult<byte[], ushort, bool>(byteList.ToArray(), num1, flag);
        }
    }
}
