// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Panasonic.PanasonicMewtocol
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;
using ESTCore.Common.Core;
using ESTCore.Common.Reflection;
using ESTCore.Common.Serial;

using System;
using System.Threading.Tasks;

namespace ESTCore.Common.Profinet.Panasonic
{
    /// <summary>
    /// 松下PLC的数据交互协议，采用Mewtocol协议通讯，支持的地址列表参考api文档<br />
    /// The data exchange protocol of Panasonic PLC adopts Mewtocol protocol for communication. For the list of supported addresses, refer to the api document.
    /// </summary>
    /// <remarks>地址支持携带站号的访问方式，例如：s=2;D100</remarks>
    /// <example>
    /// <inheritdoc cref="T:ESTCore.Common.Profinet.Panasonic.PanasonicMewtocolOverTcp" path="example" />
    /// </example>
    public class PanasonicMewtocol : SerialDeviceBase
    {
        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Panasonic.PanasonicMewtocolOverTcp.#ctor(System.Byte)" />
        public PanasonicMewtocol(byte station = 238)
        {
            this.ByteTransform = (IByteTransform)new RegularByteTransform();
            this.Station = station;
            this.ByteTransform.DataFormat = DataFormat.DCBA;
        }

        /// <inheritdoc cref="P:ESTCore.Common.Profinet.Panasonic.PanasonicMewtocolOverTcp.Station" />
        public byte Station { get; set; }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Panasonic.PanasonicMewtocolOverTcp.Read(System.String,System.UInt16)" />
        [EstMqttApi("ReadByteArray", "")]
        public override OperateResult<byte[]> Read(string address, ushort length)
        {
            OperateResult<byte[]> operateResult1 = PanasonicHelper.BuildReadCommand((byte)EstHelper.ExtractParameter(ref address, "s", (int)this.Station), address, length);
            if (!operateResult1.IsSuccess)
                return operateResult1;
            OperateResult<byte[]> operateResult2 = this.ReadBase(operateResult1.Content);
            return !operateResult2.IsSuccess ? operateResult2 : PanasonicHelper.ExtraActualData(operateResult2.Content);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Panasonic.PanasonicMewtocolOverTcp.Write(System.String,System.Byte[])" />
        [EstMqttApi("WriteByteArray", "")]
        public override OperateResult Write(string address, byte[] value)
        {
            OperateResult<byte[]> operateResult1 = PanasonicHelper.BuildWriteCommand((byte)EstHelper.ExtractParameter(ref address, "s", (int)this.Station), address, value);
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            OperateResult<byte[]> operateResult2 = this.ReadBase(operateResult1.Content);
            return !operateResult2.IsSuccess ? (OperateResult)operateResult2 : (OperateResult)PanasonicHelper.ExtraActualData(operateResult2.Content);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Panasonic.PanasonicMewtocolOverTcp.ReadBool(System.String,System.UInt16)" />
        [EstMqttApi("ReadBoolArray", "")]
        public override OperateResult<bool[]> ReadBool(string address, ushort length)
        {
            byte parameter = (byte)EstHelper.ExtractParameter(ref address, "s", (int)this.Station);
            OperateResult<string, int> operateResult1 = PanasonicHelper.AnalysisAddress(address);
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult1);
            OperateResult<byte[]> operateResult2 = PanasonicHelper.BuildReadCommand(parameter, address, length);
            if (!operateResult2.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult2);
            OperateResult<byte[]> operateResult3 = this.ReadBase(operateResult2.Content);
            if (!operateResult3.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult3);
            OperateResult<byte[]> operateResult4 = PanasonicHelper.ExtraActualData(operateResult3.Content);
            return !operateResult4.IsSuccess ? OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult4) : OperateResult.CreateSuccessResult<bool[]>(SoftBasic.ByteToBoolArray(operateResult4.Content).SelectMiddle<bool>(operateResult1.Content2 % 16, (int)length));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Panasonic.PanasonicMewtocolOverTcp.ReadBool(System.String)" />
        [EstMqttApi("ReadBool", "")]
        public override OperateResult<bool> ReadBool(string address)
        {
            OperateResult<byte[]> operateResult1 = PanasonicHelper.BuildReadOneCoil((byte)EstHelper.ExtractParameter(ref address, "s", (int)this.Station), address);
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<bool>((OperateResult)operateResult1);
            OperateResult<byte[]> operateResult2 = this.ReadBase(operateResult1.Content);
            return !operateResult2.IsSuccess ? OperateResult.CreateFailedResult<bool>((OperateResult)operateResult2) : PanasonicHelper.ExtraActualBool(operateResult2.Content);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Panasonic.PanasonicMewtocolOverTcp.Write(System.String,System.Boolean[])" />
        [EstMqttApi("WriteBoolArray", "")]
        public override OperateResult Write(string address, bool[] values)
        {
            byte parameter = (byte)EstHelper.ExtractParameter(ref address, "s", (int)this.Station);
            OperateResult<string, int> operateResult1 = PanasonicHelper.AnalysisAddress(address);
            if (!operateResult1.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult1);
            if ((uint)(operateResult1.Content2 % 16) > 0U)
                return new OperateResult(StringResources.Language.PanasonicAddressBitStartMulti16);
            if ((uint)(values.Length % 16) > 0U)
                return new OperateResult(StringResources.Language.PanasonicBoolLengthMulti16);
            byte[] values1 = SoftBasic.BoolArrayToByte(values);
            OperateResult<byte[]> operateResult2 = PanasonicHelper.BuildWriteCommand(parameter, address, values1);
            if (!operateResult2.IsSuccess)
                return (OperateResult)operateResult2;
            OperateResult<byte[]> operateResult3 = this.ReadBase(operateResult2.Content);
            return !operateResult3.IsSuccess ? (OperateResult)operateResult3 : (OperateResult)PanasonicHelper.ExtraActualData(operateResult3.Content);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Panasonic.PanasonicMewtocolOverTcp.Write(System.String,System.Boolean)" />
        [EstMqttApi("WriteBool", "")]
        public override OperateResult Write(string address, bool value)
        {
            OperateResult<byte[]> operateResult1 = PanasonicHelper.BuildWriteOneCoil((byte)EstHelper.ExtractParameter(ref address, "s", (int)this.Station), address, value);
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            OperateResult<byte[]> operateResult2 = this.ReadBase(operateResult1.Content);
            return !operateResult2.IsSuccess ? (OperateResult)operateResult2 : (OperateResult)PanasonicHelper.ExtraActualData(operateResult2.Content);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Panasonic.PanasonicMewtocol.ReadBool(System.String)" />
        public override async Task<OperateResult<bool>> ReadBoolAsync(string address)
        {
            OperateResult<bool> operateResult = await Task.Run<OperateResult<bool>>((Func<OperateResult<bool>>)(() => this.ReadBool(address)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Panasonic.PanasonicMewtocol.Write(System.String,System.Boolean)" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          bool value)
        {
            OperateResult operateResult = await Task.Run<OperateResult>((Func<OperateResult>)(() => this.Write(address, value)));
            return operateResult;
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("Panasonic Mewtocol[{0}:{1}]", (object)this.PortName, (object)this.BaudRate);
    }
}
