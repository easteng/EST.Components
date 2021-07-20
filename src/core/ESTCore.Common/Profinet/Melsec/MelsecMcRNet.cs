// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Melsec.MelsecMcRNet
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
using System.Threading.Tasks;

namespace ESTCore.Common.Profinet.Melsec
{
    /// <summary>
    /// 三菱的R系列的MC协议，支持的地址类型和 <see cref="T:ESTCore.Common.Profinet.Melsec.MelsecMcNet" /> 有区别，详细请查看对应的API文档说明
    /// </summary>
    public class MelsecMcRNet : NetworkDeviceBase
    {
        /// <summary>
        /// 实例化三菱R系列的Qna兼容3E帧协议的通讯对象<br />
        /// Instantiate the communication object of Mitsubishi's Qna compatible 3E frame protocol
        /// </summary>
        public MelsecMcRNet()
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
        public MelsecMcRNet(string ipAddress, int port)
        {
            this.WordLength = (ushort)1;
            this.IpAddress = ipAddress;
            this.Port = port;
            this.ByteTransform = (IByteTransform)new RegularByteTransform();
        }

        /// <inheritdoc />
        protected override INetMessage GetNewNetMessage() => (INetMessage)new MelsecQnA3EBinaryMessage();

        /// <inheritdoc cref="P:ESTCore.Common.Profinet.Melsec.MelsecMcNet.NetworkNumber" />
        public byte NetworkNumber { get; set; } = 0;

        /// <inheritdoc cref="P:ESTCore.Common.Profinet.Melsec.MelsecMcNet.NetworkStationNumber" />
        public byte NetworkStationNumber { get; set; } = 0;

        /// <inheritdoc />
        [EstMqttApi("ReadByteArray", "")]
        public override OperateResult<byte[]> Read(string address, ushort length)
        {
            OperateResult<McRAddressData> melsecRfrom = McRAddressData.ParseMelsecRFrom(address, length);
            if (!melsecRfrom.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)melsecRfrom);
            List<byte> byteList = new List<byte>();
            ushort num1 = 0;
            while ((int)num1 < (int)length)
            {
                ushort num2 = (ushort)Math.Min((int)length - (int)num1, 900);
                melsecRfrom.Content.Length = num2;
                OperateResult<byte[]> operateResult = this.ReadAddressData(melsecRfrom.Content, false);
                if (!operateResult.IsSuccess)
                    return operateResult;
                byteList.AddRange((IEnumerable<byte>)operateResult.Content);
                num1 += num2;
                if (melsecRfrom.Content.McDataType.DataType == (byte)0)
                    melsecRfrom.Content.AddressStart += (int)num2;
                else
                    melsecRfrom.Content.AddressStart += (int)num2 * 16;
            }
            return OperateResult.CreateSuccessResult<byte[]>(byteList.ToArray());
        }

        private OperateResult<byte[]> ReadAddressData(McRAddressData address, bool isBit)
        {
            OperateResult<byte[]> operateResult = this.ReadFromCoreServer(MelsecMcNet.PackMcCommand(MelsecMcRNet.BuildReadMcCoreCommand(address, isBit), this.NetworkNumber, this.NetworkStationNumber));
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult);
            OperateResult result = MelsecMcNet.CheckResponseContent(operateResult.Content);
            return !result.IsSuccess ? OperateResult.CreateFailedResult<byte[]>(result) : MelsecMcNet.ExtractActualData(operateResult.Content.RemoveBegin<byte>(11), isBit);
        }

        /// <inheritdoc />
        [EstMqttApi("WriteByteArray", "")]
        public override OperateResult Write(string address, byte[] value)
        {
            OperateResult<McRAddressData> melsecRfrom = McRAddressData.ParseMelsecRFrom(address, (ushort)0);
            return !melsecRfrom.IsSuccess ? (OperateResult)OperateResult.CreateFailedResult<byte[]>((OperateResult)melsecRfrom) : this.WriteAddressData(melsecRfrom.Content, value);
        }

        private OperateResult WriteAddressData(McRAddressData addressData, byte[] value)
        {
            OperateResult<byte[]> operateResult = this.ReadFromCoreServer(MelsecMcNet.PackMcCommand(MelsecMcRNet.BuildWriteWordCoreCommand(addressData, value), this.NetworkNumber, this.NetworkStationNumber));
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
            OperateResult<McRAddressData> addressResult = McRAddressData.ParseMelsecRFrom(address, length);
            if (!addressResult.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)addressResult);
            List<byte> bytesContent = new List<byte>();
            ushort alreadyFinished = 0;
            while ((int)alreadyFinished < (int)length)
            {
                ushort readLength = (ushort)Math.Min((int)length - (int)alreadyFinished, 900);
                addressResult.Content.Length = readLength;
                OperateResult<byte[]> read = await this.ReadAddressDataAsync(addressResult.Content, false);
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
          McRAddressData address,
          bool isBit)
        {
            byte[] coreResult = MelsecMcRNet.BuildReadMcCoreCommand(address, isBit);
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(MelsecMcNet.PackMcCommand(coreResult, this.NetworkNumber, this.NetworkStationNumber));
            if (!read.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)read);
            OperateResult check = MelsecMcNet.CheckResponseContent(read.Content);
            return check.IsSuccess ? MelsecMcNet.ExtractActualData(read.Content.RemoveBegin<byte>(11), isBit) : OperateResult.CreateFailedResult<byte[]>(check);
        }

        /// <inheritdoc />
        public override async Task<OperateResult> WriteAsync(
          string address,
          byte[] value)
        {
            OperateResult<McRAddressData> addressResult = McRAddressData.ParseMelsecRFrom(address, (ushort)0);
            if (!addressResult.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<byte[]>((OperateResult)addressResult);
            OperateResult operateResult = await this.WriteAddressDataAsync(addressResult.Content, value);
            return operateResult;
        }

        private async Task<OperateResult> WriteAddressDataAsync(
          McRAddressData addressData,
          byte[] value)
        {
            byte[] coreResult = MelsecMcRNet.BuildWriteWordCoreCommand(addressData, value);
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(MelsecMcNet.PackMcCommand(coreResult, this.NetworkNumber, this.NetworkStationNumber));
            if (!read.IsSuccess)
                return (OperateResult)read;
            OperateResult check = MelsecMcNet.CheckResponseContent(read.Content);
            return check.IsSuccess ? OperateResult.CreateSuccessResult() : (OperateResult)OperateResult.CreateFailedResult<byte[]>(check);
        }

        /// <inheritdoc />
        [EstMqttApi("ReadBoolArray", "")]
        public override OperateResult<bool[]> ReadBool(string address, ushort length)
        {
            OperateResult<McRAddressData> melsecRfrom = McRAddressData.ParseMelsecRFrom(address, length);
            if (!melsecRfrom.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)melsecRfrom);
            OperateResult<byte[]> operateResult = this.ReadFromCoreServer(MelsecMcNet.PackMcCommand(MelsecMcRNet.BuildReadMcCoreCommand(melsecRfrom.Content, true), this.NetworkNumber, this.NetworkStationNumber));
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult);
            OperateResult result = MelsecMcNet.CheckResponseContent(operateResult.Content);
            if (!result.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>(result);
            OperateResult<byte[]> actualData = MelsecMcNet.ExtractActualData(operateResult.Content.RemoveBegin<byte>(11), true);
            return !actualData.IsSuccess ? OperateResult.CreateFailedResult<bool[]>((OperateResult)actualData) : OperateResult.CreateSuccessResult<bool[]>(((IEnumerable<byte>)actualData.Content).Select<byte, bool>((Func<byte, bool>)(m => m == (byte)1)).Take<bool>((int)length).ToArray<bool>());
        }

        /// <inheritdoc />
        [EstMqttApi("WriteBoolArray", "")]
        public override OperateResult Write(string address, bool[] values)
        {
            OperateResult<McRAddressData> melsecRfrom = McRAddressData.ParseMelsecRFrom(address, (ushort)0);
            if (!melsecRfrom.IsSuccess)
                return (OperateResult)melsecRfrom;
            OperateResult<byte[]> operateResult1 = this.ReadFromCoreServer(MelsecMcNet.PackMcCommand(MelsecMcRNet.BuildWriteBitCoreCommand(melsecRfrom.Content, values), this.NetworkNumber, this.NetworkStationNumber));
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            OperateResult operateResult2 = MelsecMcNet.CheckResponseContent(operateResult1.Content);
            return !operateResult2.IsSuccess ? operateResult2 : OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc />
        public override async Task<OperateResult<bool[]>> ReadBoolAsync(
          string address,
          ushort length)
        {
            OperateResult<McRAddressData> addressResult = McRAddressData.ParseMelsecRFrom(address, length);
            if (!addressResult.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)addressResult);
            byte[] coreResult = MelsecMcRNet.BuildReadMcCoreCommand(addressResult.Content, true);
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(MelsecMcNet.PackMcCommand(coreResult, this.NetworkNumber, this.NetworkStationNumber));
            if (!read.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)read);
            OperateResult check = MelsecMcNet.CheckResponseContent(read.Content);
            if (!check.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>(check);
            OperateResult<byte[]> extract = MelsecMcNet.ExtractActualData(read.Content.RemoveBegin<byte>(11), true);
            return extract.IsSuccess ? OperateResult.CreateSuccessResult<bool[]>(((IEnumerable<byte>)extract.Content).Select<byte, bool>((Func<byte, bool>)(m => m == (byte)1)).Take<bool>((int)length).ToArray<bool>()) : OperateResult.CreateFailedResult<bool[]>((OperateResult)extract);
        }

        /// <inheritdoc />
        public override async Task<OperateResult> WriteAsync(
          string address,
          bool[] values)
        {
            OperateResult<McRAddressData> addressResult = McRAddressData.ParseMelsecRFrom(address, (ushort)0);
            if (!addressResult.IsSuccess)
                return (OperateResult)addressResult;
            byte[] coreResult = MelsecMcRNet.BuildWriteBitCoreCommand(addressResult.Content, values);
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(MelsecMcNet.PackMcCommand(coreResult, this.NetworkNumber, this.NetworkStationNumber));
            if (!read.IsSuccess)
                return (OperateResult)read;
            OperateResult check = MelsecMcNet.CheckResponseContent(read.Content);
            return check.IsSuccess ? OperateResult.CreateSuccessResult() : check;
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("MelsecMcRNet[{0}:{1}]", (object)this.IpAddress, (object)this.Port);

        /// <summary>分析三菱R系列的地址，并返回解析后的数据对象</summary>
        /// <param name="address">字符串地址</param>
        /// <returns>是否解析成功</returns>
        public static OperateResult<MelsecMcRDataType, int> AnalysisAddress(
          string address)
        {
            try
            {
                if (address.StartsWith("LSTS"))
                    return OperateResult.CreateSuccessResult<MelsecMcRDataType, int>(MelsecMcRDataType.LSTS, Convert.ToInt32(address.Substring(4), MelsecMcRDataType.LSTS.FromBase));
                if (address.StartsWith("LSTC"))
                    return OperateResult.CreateSuccessResult<MelsecMcRDataType, int>(MelsecMcRDataType.LSTC, Convert.ToInt32(address.Substring(4), MelsecMcRDataType.LSTC.FromBase));
                if (address.StartsWith("LSTN"))
                    return OperateResult.CreateSuccessResult<MelsecMcRDataType, int>(MelsecMcRDataType.LSTN, Convert.ToInt32(address.Substring(4), MelsecMcRDataType.LSTN.FromBase));
                if (address.StartsWith("STS"))
                    return OperateResult.CreateSuccessResult<MelsecMcRDataType, int>(MelsecMcRDataType.STS, Convert.ToInt32(address.Substring(3), MelsecMcRDataType.STS.FromBase));
                if (address.StartsWith("STC"))
                    return OperateResult.CreateSuccessResult<MelsecMcRDataType, int>(MelsecMcRDataType.STC, Convert.ToInt32(address.Substring(3), MelsecMcRDataType.STC.FromBase));
                if (address.StartsWith("STN"))
                    return OperateResult.CreateSuccessResult<MelsecMcRDataType, int>(MelsecMcRDataType.STN, Convert.ToInt32(address.Substring(3), MelsecMcRDataType.STN.FromBase));
                if (address.StartsWith("LTS"))
                    return OperateResult.CreateSuccessResult<MelsecMcRDataType, int>(MelsecMcRDataType.LTS, Convert.ToInt32(address.Substring(3), MelsecMcRDataType.LTS.FromBase));
                if (address.StartsWith("LTC"))
                    return OperateResult.CreateSuccessResult<MelsecMcRDataType, int>(MelsecMcRDataType.LTC, Convert.ToInt32(address.Substring(3), MelsecMcRDataType.LTC.FromBase));
                if (address.StartsWith("LTN"))
                    return OperateResult.CreateSuccessResult<MelsecMcRDataType, int>(MelsecMcRDataType.LTN, Convert.ToInt32(address.Substring(3), MelsecMcRDataType.LTN.FromBase));
                if (address.StartsWith("LCS"))
                    return OperateResult.CreateSuccessResult<MelsecMcRDataType, int>(MelsecMcRDataType.LCS, Convert.ToInt32(address.Substring(3), MelsecMcRDataType.LCS.FromBase));
                if (address.StartsWith("LCC"))
                    return OperateResult.CreateSuccessResult<MelsecMcRDataType, int>(MelsecMcRDataType.LCC, Convert.ToInt32(address.Substring(3), MelsecMcRDataType.LCC.FromBase));
                if (address.StartsWith("LCN"))
                    return OperateResult.CreateSuccessResult<MelsecMcRDataType, int>(MelsecMcRDataType.LCN, Convert.ToInt32(address.Substring(3), MelsecMcRDataType.LCN.FromBase));
                if (address.StartsWith("TS"))
                    return OperateResult.CreateSuccessResult<MelsecMcRDataType, int>(MelsecMcRDataType.TS, Convert.ToInt32(address.Substring(2), MelsecMcRDataType.TS.FromBase));
                if (address.StartsWith("TC"))
                    return OperateResult.CreateSuccessResult<MelsecMcRDataType, int>(MelsecMcRDataType.TC, Convert.ToInt32(address.Substring(2), MelsecMcRDataType.TC.FromBase));
                if (address.StartsWith("TN"))
                    return OperateResult.CreateSuccessResult<MelsecMcRDataType, int>(MelsecMcRDataType.TN, Convert.ToInt32(address.Substring(2), MelsecMcRDataType.TN.FromBase));
                if (address.StartsWith("CS"))
                    return OperateResult.CreateSuccessResult<MelsecMcRDataType, int>(MelsecMcRDataType.CS, Convert.ToInt32(address.Substring(2), MelsecMcRDataType.CS.FromBase));
                if (address.StartsWith("CC"))
                    return OperateResult.CreateSuccessResult<MelsecMcRDataType, int>(MelsecMcRDataType.CC, Convert.ToInt32(address.Substring(2), MelsecMcRDataType.CC.FromBase));
                if (address.StartsWith("CN"))
                    return OperateResult.CreateSuccessResult<MelsecMcRDataType, int>(MelsecMcRDataType.CN, Convert.ToInt32(address.Substring(2), MelsecMcRDataType.CN.FromBase));
                if (address.StartsWith("SM"))
                    return OperateResult.CreateSuccessResult<MelsecMcRDataType, int>(MelsecMcRDataType.SM, Convert.ToInt32(address.Substring(2), MelsecMcRDataType.SM.FromBase));
                if (address.StartsWith("SB"))
                    return OperateResult.CreateSuccessResult<MelsecMcRDataType, int>(MelsecMcRDataType.SB, Convert.ToInt32(address.Substring(2), MelsecMcRDataType.SB.FromBase));
                if (address.StartsWith("DX"))
                    return OperateResult.CreateSuccessResult<MelsecMcRDataType, int>(MelsecMcRDataType.DX, Convert.ToInt32(address.Substring(2), MelsecMcRDataType.DX.FromBase));
                if (address.StartsWith("DY"))
                    return OperateResult.CreateSuccessResult<MelsecMcRDataType, int>(MelsecMcRDataType.DY, Convert.ToInt32(address.Substring(2), MelsecMcRDataType.DY.FromBase));
                if (address.StartsWith("SD"))
                    return OperateResult.CreateSuccessResult<MelsecMcRDataType, int>(MelsecMcRDataType.SD, Convert.ToInt32(address.Substring(2), MelsecMcRDataType.SD.FromBase));
                if (address.StartsWith("SW"))
                    return OperateResult.CreateSuccessResult<MelsecMcRDataType, int>(MelsecMcRDataType.SW, Convert.ToInt32(address.Substring(2), MelsecMcRDataType.SW.FromBase));
                if (address.StartsWith("X"))
                    return OperateResult.CreateSuccessResult<MelsecMcRDataType, int>(MelsecMcRDataType.X, Convert.ToInt32(address.Substring(1), MelsecMcRDataType.X.FromBase));
                if (address.StartsWith("Y"))
                    return OperateResult.CreateSuccessResult<MelsecMcRDataType, int>(MelsecMcRDataType.Y, Convert.ToInt32(address.Substring(1), MelsecMcRDataType.Y.FromBase));
                if (address.StartsWith("M"))
                    return OperateResult.CreateSuccessResult<MelsecMcRDataType, int>(MelsecMcRDataType.M, Convert.ToInt32(address.Substring(1), MelsecMcRDataType.M.FromBase));
                if (address.StartsWith("L"))
                    return OperateResult.CreateSuccessResult<MelsecMcRDataType, int>(MelsecMcRDataType.L, Convert.ToInt32(address.Substring(1), MelsecMcRDataType.L.FromBase));
                if (address.StartsWith("F"))
                    return OperateResult.CreateSuccessResult<MelsecMcRDataType, int>(MelsecMcRDataType.F, Convert.ToInt32(address.Substring(1), MelsecMcRDataType.F.FromBase));
                if (address.StartsWith("V"))
                    return OperateResult.CreateSuccessResult<MelsecMcRDataType, int>(MelsecMcRDataType.V, Convert.ToInt32(address.Substring(1), MelsecMcRDataType.V.FromBase));
                if (address.StartsWith("S"))
                    return OperateResult.CreateSuccessResult<MelsecMcRDataType, int>(MelsecMcRDataType.S, Convert.ToInt32(address.Substring(1), MelsecMcRDataType.S.FromBase));
                if (address.StartsWith("B"))
                    return OperateResult.CreateSuccessResult<MelsecMcRDataType, int>(MelsecMcRDataType.B, Convert.ToInt32(address.Substring(1), MelsecMcRDataType.B.FromBase));
                if (address.StartsWith("D"))
                    return OperateResult.CreateSuccessResult<MelsecMcRDataType, int>(MelsecMcRDataType.D, Convert.ToInt32(address.Substring(1), MelsecMcRDataType.D.FromBase));
                if (address.StartsWith("W"))
                    return OperateResult.CreateSuccessResult<MelsecMcRDataType, int>(MelsecMcRDataType.W, Convert.ToInt32(address.Substring(1), MelsecMcRDataType.W.FromBase));
                if (address.StartsWith("R"))
                    return OperateResult.CreateSuccessResult<MelsecMcRDataType, int>(MelsecMcRDataType.R, Convert.ToInt32(address.Substring(1), MelsecMcRDataType.R.FromBase));
                return address.StartsWith("Z") ? OperateResult.CreateSuccessResult<MelsecMcRDataType, int>(MelsecMcRDataType.Z, Convert.ToInt32(address.Substring(1), MelsecMcRDataType.Z.FromBase)) : new OperateResult<MelsecMcRDataType, int>(StringResources.Language.NotSupportedDataType);
            }
            catch (Exception ex)
            {
                return new OperateResult<MelsecMcRDataType, int>(ex.Message);
            }
        }

        /// <summary>从三菱地址，是否位读取进行创建读取的MC的核心报文</summary>
        /// <param name="address">地址数据</param>
        /// <param name="isBit">是否进行了位读取操作</param>
        /// <returns>带有成功标识的报文对象</returns>
        public static byte[] BuildReadMcCoreCommand(McRAddressData address, bool isBit) => new byte[12]
        {
      (byte) 1,
      (byte) 4,
      isBit ? (byte) 1 : (byte) 0,
      (byte) 0,
      BitConverter.GetBytes(address.AddressStart)[0],
      BitConverter.GetBytes(address.AddressStart)[1],
      BitConverter.GetBytes(address.AddressStart)[2],
      BitConverter.GetBytes(address.AddressStart)[3],
      address.McDataType.DataCode[0],
      address.McDataType.DataCode[1],
      (byte) ((uint) address.Length % 256U),
      (byte) ((uint) address.Length / 256U)
        };

        /// <summary>以字为单位，创建数据写入的核心报文</summary>
        /// <param name="address">三菱的数据地址</param>
        /// <param name="value">实际的原始数据信息</param>
        /// <returns>带有成功标识的报文对象</returns>
        public static byte[] BuildWriteWordCoreCommand(McRAddressData address, byte[] value)
        {
            if (value == null)
                value = new byte[0];
            byte[] numArray = new byte[12 + value.Length];
            numArray[0] = (byte)1;
            numArray[1] = (byte)20;
            numArray[2] = (byte)0;
            numArray[3] = (byte)0;
            numArray[4] = BitConverter.GetBytes(address.AddressStart)[0];
            numArray[5] = BitConverter.GetBytes(address.AddressStart)[1];
            numArray[6] = BitConverter.GetBytes(address.AddressStart)[2];
            numArray[7] = BitConverter.GetBytes(address.AddressStart)[3];
            numArray[8] = address.McDataType.DataCode[0];
            numArray[9] = address.McDataType.DataCode[1];
            numArray[10] = (byte)(value.Length / 2 % 256);
            numArray[11] = (byte)(value.Length / 2 / 256);
            value.CopyTo((Array)numArray, 12);
            return numArray;
        }

        /// <summary>以位为单位，创建数据写入的核心报文</summary>
        /// <param name="address">三菱的地址信息</param>
        /// <param name="value">原始的bool数组数据</param>
        /// <returns>带有成功标识的报文对象</returns>
        public static byte[] BuildWriteBitCoreCommand(McRAddressData address, bool[] value)
        {
            if (value == null)
                value = new bool[0];
            byte[] byteData = MelsecHelper.TransBoolArrayToByteData(value);
            byte[] numArray = new byte[12 + byteData.Length];
            numArray[0] = (byte)1;
            numArray[1] = (byte)20;
            numArray[2] = (byte)1;
            numArray[3] = (byte)0;
            numArray[4] = BitConverter.GetBytes(address.AddressStart)[0];
            numArray[5] = BitConverter.GetBytes(address.AddressStart)[1];
            numArray[6] = BitConverter.GetBytes(address.AddressStart)[2];
            numArray[7] = BitConverter.GetBytes(address.AddressStart)[3];
            numArray[8] = address.McDataType.DataCode[0];
            numArray[9] = address.McDataType.DataCode[1];
            numArray[10] = (byte)(value.Length % 256);
            numArray[11] = (byte)(value.Length / 256);
            byteData.CopyTo((Array)numArray, 12);
            return numArray;
        }
    }
}
