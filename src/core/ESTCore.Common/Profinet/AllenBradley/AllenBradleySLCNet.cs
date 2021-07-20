// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.AllenBradley.AllenBradleySLCNet
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Core;
using ESTCore.Common.Core.IMessage;
using ESTCore.Common.Core.Net;
using ESTCore.Common.Reflection;

using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ESTCore.Common.Profinet.AllenBradley
{
    /// <summary>
    /// AllenBradley品牌的PLC，针对SLC系列的通信的实现，测试PLC为1747。<br />
    /// AllenBradley brand PLC, for the realization of SLC series communication, the test PLC is 1747.
    /// </summary>
    /// <remarks>
    /// 地址格式如下：
    /// <list type="table">
    ///   <listheader>
    ///     <term>地址代号</term>
    ///     <term>字操作</term>
    ///     <term>位操作</term>
    ///     <term>备注</term>
    ///   </listheader>
    ///   <item>
    ///     <term>A</term>
    ///     <term>A9:0</term>
    ///     <term>A9:0/1 或 A9:0.1</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>B</term>
    ///     <term>B9:0</term>
    ///     <term>B9:0/1 或 B9:0.1</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>N</term>
    ///     <term>N9:0</term>
    ///     <term>N9:0/1 或 N9:0.1</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>F</term>
    ///     <term>F9:0</term>
    ///     <term>F9:0/1 或 F9:0.1</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>S</term>
    ///     <term>S:0</term>
    ///     <term>S:0/1 或 S:0.1</term>
    ///     <term>S:0 等同于 S2:0</term>
    ///   </item>
    ///   <item>
    ///     <term>C</term>
    ///     <term>C9:0</term>
    ///     <term>C9:0/1 或 C9:0.1</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>I</term>
    ///     <term>I9:0</term>
    ///     <term>I9:0/1 或 I9:0.1</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>O</term>
    ///     <term>O9:0</term>
    ///     <term>O9:0/1 或 O9:0.1</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>R</term>
    ///     <term>R9:0</term>
    ///     <term>R9:0/1 或 R9:0.1</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>T</term>
    ///     <term>T9:0</term>
    ///     <term>T9:0/1 或 T9:0.1</term>
    ///     <term></term>
    ///   </item>
    /// </list>
    /// 感谢 seedee 的测试支持。
    /// </remarks>
    public class AllenBradleySLCNet : NetworkDeviceBase
    {
        /// <summary>
        /// Instantiate a communication object for a Allenbradley PLC protocol
        /// </summary>
        public AllenBradleySLCNet()
        {
            this.WordLength = (ushort)2;
            this.ByteTransform = (IByteTransform)new RegularByteTransform();
        }

        /// <summary>
        /// Instantiate a communication object for a Allenbradley PLC protocol
        /// </summary>
        /// <param name="ipAddress">PLC IpAddress</param>
        /// <param name="port">PLC Port</param>
        public AllenBradleySLCNet(string ipAddress, int port = 44818)
        {
            this.WordLength = (ushort)2;
            this.IpAddress = ipAddress;
            this.Port = port;
            this.ByteTransform = (IByteTransform)new RegularByteTransform();
        }

        /// <inheritdoc />
        protected override INetMessage GetNewNetMessage() => (INetMessage)new AllenBradleySLCMessage();

        /// <summary>
        /// The current session handle, which is determined by the PLC when communicating with the PLC handshake
        /// </summary>
        public uint SessionHandle { get; protected set; }

        /// <inheritdoc />
        protected override OperateResult InitializationOnConnect(Socket socket)
        {
            OperateResult<byte[]> operateResult = this.ReadFromCoreServer(socket, "01 01 00 00 00 00 00 00 00 00 00 00 00 04 00 05 00 00 00 00 00 00 00 00 00 00 00 00".ToHexBytes());
            if (!operateResult.IsSuccess)
                return (OperateResult)operateResult;
            this.SessionHandle = this.ByteTransform.TransUInt32(operateResult.Content, 4);
            return OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc />
        protected override async Task<OperateResult> InitializationOnConnectAsync(
          Socket socket)
        {
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(socket, "01 01 00 00 00 00 00 00 00 00 00 00 00 04 00 05 00 00 00 00 00 00 00 00 00 00 00 00".ToHexBytes());
            if (!read.IsSuccess)
                return (OperateResult)read;
            this.SessionHandle = this.ByteTransform.TransUInt32(read.Content, 4);
            return OperateResult.CreateSuccessResult();
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
            OperateResult<byte[]> operateResult1 = AllenBradleySLCNet.BuildReadCommand(address, length);
            if (!operateResult1.IsSuccess)
                return operateResult1;
            OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(this.PackCommand(operateResult1.Content));
            if (!operateResult2.IsSuccess)
                return operateResult2;
            OperateResult<byte[]> operateResult3 = AllenBradleySLCNet.ExtraActualContent(operateResult2.Content);
            return !operateResult3.IsSuccess ? operateResult3 : OperateResult.CreateSuccessResult<byte[]>(operateResult3.Content);
        }

        /// <inheritdoc />
        [EstMqttApi("WriteByteArray", "")]
        public override OperateResult Write(string address, byte[] value)
        {
            OperateResult<byte[]> operateResult1 = AllenBradleySLCNet.BuildWriteCommand(address, value);
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(this.PackCommand(operateResult1.Content));
            if (!operateResult2.IsSuccess)
                return (OperateResult)operateResult2;
            OperateResult<byte[]> operateResult3 = AllenBradleySLCNet.ExtraActualContent(operateResult2.Content);
            return !operateResult3.IsSuccess ? (OperateResult)operateResult3 : (OperateResult)OperateResult.CreateSuccessResult<byte[]>(operateResult3.Content);
        }

        /// <inheritdoc />
        [EstMqttApi("ReadBool", "")]
        public override OperateResult<bool> ReadBool(string address)
        {
            int bitIndex;
            address = AllenBradleySLCNet.AnalysisBitIndex(address, out bitIndex);
            OperateResult<byte[]> operateResult = this.Read(address, (ushort)1);
            return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<bool>((OperateResult)operateResult) : OperateResult.CreateSuccessResult<bool>(operateResult.Content.ToBoolArray()[bitIndex]);
        }

        /// <inheritdoc />
        [EstMqttApi("WriteBool", "")]
        public override OperateResult Write(string address, bool value)
        {
            OperateResult<byte[]> operateResult1 = AllenBradleySLCNet.BuildWriteCommand(address, value);
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(this.PackCommand(operateResult1.Content));
            if (!operateResult2.IsSuccess)
                return (OperateResult)operateResult2;
            OperateResult<byte[]> operateResult3 = AllenBradleySLCNet.ExtraActualContent(operateResult2.Content);
            return !operateResult3.IsSuccess ? (OperateResult)operateResult3 : (OperateResult)OperateResult.CreateSuccessResult<byte[]>(operateResult3.Content);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.AllenBradley.AllenBradleySLCNet.Read(System.String,System.UInt16)" />
        public override async Task<OperateResult<byte[]>> ReadAsync(
          string address,
          ushort length)
        {
            OperateResult<byte[]> command = AllenBradleySLCNet.BuildReadCommand(address, length);
            if (!command.IsSuccess)
                return command;
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(this.PackCommand(command.Content));
            if (!read.IsSuccess)
                return read;
            OperateResult<byte[]> extra = AllenBradleySLCNet.ExtraActualContent(read.Content);
            return extra.IsSuccess ? OperateResult.CreateSuccessResult<byte[]>(extra.Content) : extra;
        }

        /// <inheritdoc />
        public override async Task<OperateResult> WriteAsync(
          string address,
          byte[] value)
        {
            OperateResult<byte[]> command = AllenBradleySLCNet.BuildWriteCommand(address, value);
            if (!command.IsSuccess)
                return (OperateResult)command;
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(this.PackCommand(command.Content));
            if (!read.IsSuccess)
                return (OperateResult)read;
            OperateResult<byte[]> extra = AllenBradleySLCNet.ExtraActualContent(read.Content);
            return extra.IsSuccess ? (OperateResult)OperateResult.CreateSuccessResult<byte[]>(extra.Content) : (OperateResult)extra;
        }

        /// <inheritdoc />
        public override async Task<OperateResult<bool>> ReadBoolAsync(string address)
        {
            int bitIndex;
            address = AllenBradleySLCNet.AnalysisBitIndex(address, out bitIndex);
            OperateResult<byte[]> read = await this.ReadAsync(address, (ushort)1);
            OperateResult<bool> operateResult = read.IsSuccess ? OperateResult.CreateSuccessResult<bool>(read.Content.ToBoolArray()[bitIndex]) : OperateResult.CreateFailedResult<bool>((OperateResult)read);
            read = (OperateResult<byte[]>)null;
            return operateResult;
        }

        /// <inheritdoc />
        public override async Task<OperateResult> WriteAsync(
          string address,
          bool value)
        {
            OperateResult<byte[]> command = AllenBradleySLCNet.BuildWriteCommand(address, value);
            if (!command.IsSuccess)
                return (OperateResult)command;
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(this.PackCommand(command.Content));
            if (!read.IsSuccess)
                return (OperateResult)read;
            OperateResult<byte[]> extra = AllenBradleySLCNet.ExtraActualContent(read.Content);
            return extra.IsSuccess ? (OperateResult)OperateResult.CreateSuccessResult<byte[]>(extra.Content) : (OperateResult)extra;
        }

        private byte[] PackCommand(byte[] coreCmd)
        {
            byte[] numArray = new byte[28 + coreCmd.Length];
            numArray[0] = (byte)1;
            numArray[1] = (byte)7;
            numArray[2] = (byte)(coreCmd.Length / 256);
            numArray[3] = (byte)(coreCmd.Length % 256);
            BitConverter.GetBytes(this.SessionHandle).CopyTo((Array)numArray, 4);
            coreCmd.CopyTo((Array)numArray, 28);
            return numArray;
        }

        /// <summary>分析地址数据信息里的位索引的信息</summary>
        /// <param name="address">数据地址</param>
        /// <param name="bitIndex">位索引</param>
        /// <returns>地址信息</returns>
        public static string AnalysisBitIndex(string address, out int bitIndex)
        {
            bitIndex = 0;
            int length = address.IndexOf('/');
            if (length < 0)
                length = address.IndexOf('.');
            if (length > 0)
            {
                bitIndex = int.Parse(address.Substring(length + 1));
                address = address.Substring(0, length);
            }
            return address;
        }

        /// <summary>
        /// 分析当前的地址信息，返回类型代号，区块号，起始地址<br />
        /// Analyze the current address information, return the type code, block number, and actual address
        /// </summary>
        /// <param name="address">地址信息</param>
        /// <returns>结果内容对象</returns>
        public static OperateResult<byte, byte, ushort> AnalysisAddress(string address)
        {
            if (!address.Contains(":"))
                return new OperateResult<byte, byte, ushort>("Address can't find ':', example : A9:0");
            string[] strArray = address.Split(':');
            try
            {
                OperateResult<byte, byte, ushort> operateResult = new OperateResult<byte, byte, ushort>();
                switch (strArray[0][0])
                {
                    case 'A':
                        operateResult.Content1 = (byte)142;
                        break;
                    case 'B':
                        operateResult.Content1 = (byte)133;
                        break;
                    case 'C':
                        operateResult.Content1 = (byte)135;
                        break;
                    case 'F':
                        operateResult.Content1 = (byte)138;
                        break;
                    case 'I':
                        operateResult.Content1 = (byte)131;
                        break;
                    case 'N':
                        operateResult.Content1 = (byte)137;
                        break;
                    case 'O':
                        operateResult.Content1 = (byte)130;
                        break;
                    case 'R':
                        operateResult.Content1 = (byte)136;
                        break;
                    case 'S':
                        operateResult.Content1 = (byte)132;
                        break;
                    case 'T':
                        operateResult.Content1 = (byte)134;
                        break;
                    default:
                        throw new Exception("Address code wrong, must be A,B,N,F,S,C,I,O,R,T");
                }
                operateResult.Content2 = operateResult.Content1 != (byte)132 ? byte.Parse(strArray[0].Substring(1)) : (byte)2;
                operateResult.Content3 = ushort.Parse(strArray[1]);
                operateResult.IsSuccess = true;
                operateResult.Message = StringResources.Language.SuccessText;
                return operateResult;
            }
            catch (Exception ex)
            {
                return new OperateResult<byte, byte, ushort>("Wrong Address formate: " + ex.Message);
            }
        }

        /// <summary>构建读取的指令信息</summary>
        /// <param name="address">地址信息，举例：A9:0</param>
        /// <param name="length">读取的长度信息</param>
        /// <returns>是否成功</returns>
        public static OperateResult<byte[]> BuildReadCommand(string address, ushort length)
        {
            OperateResult<byte, byte, ushort> operateResult = AllenBradleySLCNet.AnalysisAddress(address);
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult);
            if (length < (ushort)2)
                length = (ushort)2;
            if (operateResult.Content1 == (byte)142)
                operateResult.Content3 /= (ushort)2;
            byte[] numArray = new byte[14]
            {
        (byte) 0,
        (byte) 5,
        (byte) 0,
        (byte) 0,
        (byte) 15,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 162,
        (byte) length,
        operateResult.Content2,
        operateResult.Content1,
        (byte) 0,
        (byte) 0
            };
            BitConverter.GetBytes(operateResult.Content3).CopyTo((Array)numArray, 12);
            return OperateResult.CreateSuccessResult<byte[]>(numArray);
        }

        /// <summary>构建写入的报文内容，变成实际的数据</summary>
        /// <param name="address">地址信息</param>
        /// <param name="value">数据值</param>
        /// <returns>是否成功的结果对象</returns>
        public static OperateResult<byte[]> BuildWriteCommand(string address, byte[] value)
        {
            OperateResult<byte, byte, ushort> operateResult = AllenBradleySLCNet.AnalysisAddress(address);
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult);
            if (operateResult.Content1 == (byte)142)
                operateResult.Content3 /= (ushort)2;
            byte[] numArray = new byte[18 + value.Length];
            numArray[0] = (byte)0;
            numArray[1] = (byte)5;
            numArray[2] = (byte)0;
            numArray[3] = (byte)0;
            numArray[4] = (byte)15;
            numArray[5] = (byte)0;
            numArray[6] = (byte)0;
            numArray[7] = (byte)1;
            numArray[8] = (byte)171;
            numArray[9] = byte.MaxValue;
            numArray[10] = BitConverter.GetBytes(value.Length)[0];
            numArray[11] = BitConverter.GetBytes(value.Length)[1];
            numArray[12] = operateResult.Content2;
            numArray[13] = operateResult.Content1;
            BitConverter.GetBytes(operateResult.Content3).CopyTo((Array)numArray, 14);
            numArray[16] = byte.MaxValue;
            numArray[17] = byte.MaxValue;
            value.CopyTo((Array)numArray, 18);
            return OperateResult.CreateSuccessResult<byte[]>(numArray);
        }

        /// <summary>构建写入的报文内容，变成实际的数据</summary>
        /// <param name="address">地址信息</param>
        /// <param name="value">数据值</param>
        /// <returns>是否成功的结果对象</returns>
        public static OperateResult<byte[]> BuildWriteCommand(string address, bool value)
        {
            int bitIndex;
            address = AllenBradleySLCNet.AnalysisBitIndex(address, out bitIndex);
            OperateResult<byte, byte, ushort> operateResult = AllenBradleySLCNet.AnalysisAddress(address);
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult);
            if (operateResult.Content1 == (byte)142)
                operateResult.Content3 /= (ushort)2;
            int num = 1 << bitIndex;
            byte[] numArray = new byte[20]
            {
        (byte) 0,
        (byte) 5,
        (byte) 0,
        (byte) 0,
        (byte) 15,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 171,
        byte.MaxValue,
        (byte) 2,
        (byte) 0,
        operateResult.Content2,
        operateResult.Content1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0
            };
            BitConverter.GetBytes(operateResult.Content3).CopyTo((Array)numArray, 14);
            numArray[16] = BitConverter.GetBytes(num)[0];
            numArray[17] = BitConverter.GetBytes(num)[1];
            if (value)
            {
                numArray[18] = BitConverter.GetBytes(num)[0];
                numArray[19] = BitConverter.GetBytes(num)[1];
            }
            return OperateResult.CreateSuccessResult<byte[]>(numArray);
        }

        /// <summary>解析当前的实际报文内容，变成数据内容</summary>
        /// <param name="content">报文内容</param>
        /// <returns>是否成功</returns>
        public static OperateResult<byte[]> ExtraActualContent(byte[] content) => content.Length < 36 ? new OperateResult<byte[]>(StringResources.Language.ReceiveDataLengthTooShort + content.ToHexString(' ')) : OperateResult.CreateSuccessResult<byte[]>(content.RemoveBegin<byte>(36));
    }
}
