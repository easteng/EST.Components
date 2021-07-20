// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Melsec.MelsecMcAsciiUdp
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Core;
using ESTCore.Common.Core.Address;
using ESTCore.Common.Core.Net;
using ESTCore.Common.Reflection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESTCore.Common.Profinet.Melsec
{
    /// <summary>
    /// 三菱PLC通讯类，采用UDP的协议实现，采用Qna兼容3E帧协议实现，需要在PLC侧先的以太网模块先进行配置，必须为ascii通讯<br />
    /// Mitsubishi PLC communication class is implemented using UDP protocol and Qna compatible 3E frame protocol.
    /// The Ethernet module needs to be configured first on the PLC side, and it must be ascii communication.
    /// </summary>
    /// <remarks>
    /// <inheritdoc cref="T:ESTCore.Common.Profinet.Melsec.MelsecMcNet" path="remarks" />
    /// </remarks>
    /// <example>
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\MelsecAscii.cs" region="Usage" title="简单的短连接使用" />
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\MelsecAscii.cs" region="Usage2" title="简单的长连接使用" />
    /// </example>
    public class MelsecMcAsciiUdp : NetworkUdpDeviceBase
    {
        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecMcNet.#ctor" />
        public MelsecMcAsciiUdp()
        {
            this.WordLength = (ushort)1;
            this.ByteTransform = (IByteTransform)new RegularByteTransform();
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecMcNet.#ctor(System.String,System.Int32)" />
        public MelsecMcAsciiUdp(string ipAddress, int port)
        {
            this.WordLength = (ushort)1;
            this.IpAddress = ipAddress;
            this.Port = port;
            this.ByteTransform = (IByteTransform)new RegularByteTransform();
        }

        /// <inheritdoc cref="P:ESTCore.Common.Profinet.Melsec.MelsecMcNet.NetworkNumber" />
        public byte NetworkNumber { get; set; } = 0;

        /// <inheritdoc cref="P:ESTCore.Common.Profinet.Melsec.MelsecMcNet.NetworkStationNumber" />
        public byte NetworkStationNumber { get; set; } = 0;

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecMcNet.McAnalysisAddress(System.String,System.UInt16)" />
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
            OperateResult<McAddressData> operateResult1 = this.McAnalysisAddress(address, length);
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult1);
            List<byte> byteList = new List<byte>();
            ushort num1 = 0;
            while ((int)num1 < (int)length)
            {
                ushort num2 = (ushort)Math.Min((int)length - (int)num1, 450);
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
            OperateResult<byte[]> operateResult = this.ReadFromCoreServer(MelsecMcAsciiNet.PackMcCommand(MelsecHelper.BuildAsciiReadMcCoreCommand(addressData, false), this.NetworkNumber, this.NetworkStationNumber));
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult);
            OperateResult result = MelsecMcAsciiNet.CheckResponseContent(operateResult.Content);
            return !result.IsSuccess ? OperateResult.CreateFailedResult<byte[]>(result) : MelsecMcAsciiNet.ExtractActualData(operateResult.Content, false);
        }

        /// <inheritdoc />
        [EstMqttApi("WriteByteArray", "")]
        public override OperateResult Write(string address, byte[] value)
        {
            OperateResult<McAddressData> operateResult1 = this.McAnalysisAddress(address, (ushort)0);
            if (!operateResult1.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult1);
            OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(MelsecMcAsciiNet.PackMcCommand(MelsecHelper.BuildAsciiWriteWordCoreCommand(operateResult1.Content, value), this.NetworkNumber, this.NetworkStationNumber));
            if (!operateResult2.IsSuccess)
                return (OperateResult)operateResult2;
            OperateResult operateResult3 = MelsecMcAsciiNet.CheckResponseContent(operateResult2.Content);
            return !operateResult3.IsSuccess ? operateResult3 : OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecMcNet.ReadRandom(System.String[])" />
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
            OperateResult<byte[]> operateResult = this.ReadFromCoreServer(MelsecMcAsciiNet.PackMcCommand(MelsecHelper.BuildAsciiReadRandomWordCommand(address1), this.NetworkNumber, this.NetworkStationNumber));
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult);
            OperateResult result = MelsecMcAsciiNet.CheckResponseContent(operateResult.Content);
            return !result.IsSuccess ? OperateResult.CreateFailedResult<byte[]>(result) : MelsecMcAsciiNet.ExtractActualData(operateResult.Content, false);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecMcNet.ReadRandom(System.String[],System.UInt16[])" />
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
            OperateResult<byte[]> operateResult = this.ReadFromCoreServer(MelsecMcAsciiNet.PackMcCommand(MelsecHelper.BuildAsciiReadRandomCommand(address1), this.NetworkNumber, this.NetworkStationNumber));
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult);
            OperateResult result = MelsecMcAsciiNet.CheckResponseContent(operateResult.Content);
            return !result.IsSuccess ? OperateResult.CreateFailedResult<byte[]>(result) : MelsecMcAsciiNet.ExtractActualData(operateResult.Content, false);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecMcAsciiUdp.ReadRandomInt16(System.String[])" />
        public OperateResult<short[]> ReadRandomInt16(string[] address)
        {
            OperateResult<byte[]> operateResult = this.ReadRandom(address);
            return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<short[]>((OperateResult)operateResult) : OperateResult.CreateSuccessResult<short[]>(this.ByteTransform.TransInt16(operateResult.Content, 0, address.Length));
        }

        /// <inheritdoc />
        [EstMqttApi("ReadBoolArray", "")]
        public override OperateResult<bool[]> ReadBool(string address, ushort length)
        {
            OperateResult<McAddressData> operateResult1 = this.McAnalysisAddress(address, length);
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult1);
            OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(MelsecMcAsciiNet.PackMcCommand(MelsecHelper.BuildAsciiReadMcCoreCommand(operateResult1.Content, true), this.NetworkNumber, this.NetworkStationNumber));
            if (!operateResult2.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult2);
            OperateResult result = MelsecMcAsciiNet.CheckResponseContent(operateResult2.Content);
            if (!result.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>(result);
            OperateResult<byte[]> actualData = MelsecMcAsciiNet.ExtractActualData(operateResult2.Content, true);
            return !actualData.IsSuccess ? OperateResult.CreateFailedResult<bool[]>((OperateResult)actualData) : OperateResult.CreateSuccessResult<bool[]>(((IEnumerable<byte>)actualData.Content).Select<byte, bool>((Func<byte, bool>)(m => m == (byte)1)).Take<bool>((int)length).ToArray<bool>());
        }

        /// <inheritdoc />
        [EstMqttApi("WriteBoolArray", "")]
        public override OperateResult Write(string address, bool[] values)
        {
            OperateResult<McAddressData> operateResult1 = this.McAnalysisAddress(address, (ushort)0);
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(MelsecMcAsciiNet.PackMcCommand(MelsecHelper.BuildAsciiWriteBitCoreCommand(operateResult1.Content, values), this.NetworkNumber, this.NetworkStationNumber));
            if (!operateResult2.IsSuccess)
                return (OperateResult)operateResult2;
            OperateResult operateResult3 = MelsecMcAsciiNet.CheckResponseContent(operateResult2.Content);
            return !operateResult3.IsSuccess ? operateResult3 : OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecMcNet.RemoteRun(System.Boolean)" />
        [EstMqttApi]
        public OperateResult RemoteRun()
        {
            OperateResult<byte[]> operateResult1 = this.ReadFromCoreServer(MelsecMcAsciiNet.PackMcCommand(Encoding.ASCII.GetBytes("1001000000010000"), this.NetworkNumber, this.NetworkStationNumber));
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            OperateResult operateResult2 = MelsecMcAsciiNet.CheckResponseContent(operateResult1.Content);
            return !operateResult2.IsSuccess ? operateResult2 : OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecMcNet.RemoteStop" />
        [EstMqttApi]
        public OperateResult RemoteStop()
        {
            OperateResult<byte[]> operateResult1 = this.ReadFromCoreServer(MelsecMcAsciiNet.PackMcCommand(Encoding.ASCII.GetBytes("100200000001"), this.NetworkNumber, this.NetworkStationNumber));
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            OperateResult operateResult2 = MelsecMcAsciiNet.CheckResponseContent(operateResult1.Content);
            return !operateResult2.IsSuccess ? operateResult2 : OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecMcNet.RemoteReset" />
        [EstMqttApi]
        public OperateResult RemoteReset()
        {
            OperateResult<byte[]> operateResult1 = this.ReadFromCoreServer(MelsecMcAsciiNet.PackMcCommand(Encoding.ASCII.GetBytes("100600000001"), this.NetworkNumber, this.NetworkStationNumber));
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            OperateResult operateResult2 = MelsecMcAsciiNet.CheckResponseContent(operateResult1.Content);
            return !operateResult2.IsSuccess ? operateResult2 : OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecMcNet.ReadPlcType" />
        [EstMqttApi]
        public OperateResult<string> ReadPlcType()
        {
            OperateResult<byte[]> operateResult = this.ReadFromCoreServer(MelsecMcAsciiNet.PackMcCommand(Encoding.ASCII.GetBytes("01010000"), this.NetworkNumber, this.NetworkStationNumber));
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<string>((OperateResult)operateResult);
            OperateResult result = MelsecMcAsciiNet.CheckResponseContent(operateResult.Content);
            return !result.IsSuccess ? OperateResult.CreateFailedResult<string>(result) : OperateResult.CreateSuccessResult<string>(Encoding.ASCII.GetString(operateResult.Content, 22, 16).TrimEnd());
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecMcNet.ErrorStateReset" />
        [EstMqttApi]
        public OperateResult ErrorStateReset()
        {
            OperateResult<byte[]> operateResult1 = this.ReadFromCoreServer(MelsecMcAsciiNet.PackMcCommand(Encoding.ASCII.GetBytes("01010000"), this.NetworkNumber, this.NetworkStationNumber));
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            OperateResult operateResult2 = MelsecMcAsciiNet.CheckResponseContent(operateResult1.Content);
            return !operateResult2.IsSuccess ? operateResult2 : OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("MelsecMcAsciiUdp[{0}:{1}]", (object)this.IpAddress, (object)this.Port);
    }
}
