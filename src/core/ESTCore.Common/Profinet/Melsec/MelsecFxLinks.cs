// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Melsec.MelsecFxLinks
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;
using ESTCore.Common.Core;
using ESTCore.Common.Reflection;
using ESTCore.Common.Serial;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESTCore.Common.Profinet.Melsec
{
    /// <summary>
    /// 三菱计算机链接协议，适用FX3U系列，FX3G，FX3S等等系列，通常在PLC侧连接的是485的接线口<br />
    /// Mitsubishi Computer Link Protocol, suitable for FX3U series, FX3G, FX3S, etc., usually the 485 connection port is connected on the PLC side
    /// </summary>
    /// <remarks>
    /// <inheritdoc cref="T:ESTCore.Common.Profinet.Melsec.MelsecFxLinksOverTcp" path="remarks" />
    /// </remarks>
    public class MelsecFxLinks : SerialDeviceBase
    {
        private byte station = 0;
        private byte watiingTime = 0;
        private bool sumCheck = true;

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecFxLinksOverTcp.#ctor" />
        public MelsecFxLinks()
        {
            this.ByteTransform = (IByteTransform)new RegularByteTransform();
            this.WordLength = (ushort)1;
        }

        /// <inheritdoc cref="P:ESTCore.Common.Profinet.Melsec.MelsecFxLinksOverTcp.Station" />
        public byte Station
        {
            get => this.station;
            set => this.station = value;
        }

        /// <inheritdoc cref="P:ESTCore.Common.Profinet.Melsec.MelsecFxLinksOverTcp.WaittingTime" />
        public byte WaittingTime
        {
            get => this.watiingTime;
            set
            {
                if (this.watiingTime > (byte)15)
                    this.watiingTime = (byte)15;
                else
                    this.watiingTime = value;
            }
        }

        /// <inheritdoc cref="P:ESTCore.Common.Profinet.Melsec.MelsecFxLinksOverTcp.SumCheck" />
        public bool SumCheck
        {
            get => this.sumCheck;
            set => this.sumCheck = value;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecFxLinksOverTcp.Read(System.String,System.UInt16)" />
        [EstMqttApi("ReadByteArray", "")]
        public override OperateResult<byte[]> Read(string address, ushort length)
        {
            OperateResult<byte[]> operateResult1 = MelsecFxLinksOverTcp.BuildReadCommand((byte)EstHelper.ExtractParameter(ref address, "s", (int)this.station), address, length, false, this.sumCheck, this.watiingTime);
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult1);
            OperateResult<byte[]> operateResult2 = this.ReadBase(operateResult1.Content);
            if (!operateResult2.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult2);
            if (operateResult2.Content[0] != (byte)2)
                return new OperateResult<byte[]>((int)operateResult2.Content[0], "Read Faild:" + SoftBasic.ByteToHexString(operateResult2.Content, ' '));
            byte[] numArray = new byte[(int)length * 2];
            for (int index = 0; index < numArray.Length / 2; ++index)
                BitConverter.GetBytes(Convert.ToUInt16(Encoding.ASCII.GetString(operateResult2.Content, index * 4 + 5, 4), 16)).CopyTo((Array)numArray, index * 2);
            return OperateResult.CreateSuccessResult<byte[]>(numArray);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecFxLinksOverTcp.Write(System.String,System.Byte[])" />
        [EstMqttApi("WriteByteArray", "")]
        public override OperateResult Write(string address, byte[] value)
        {
            OperateResult<byte[]> operateResult1 = MelsecFxLinksOverTcp.BuildWriteByteCommand((byte)EstHelper.ExtractParameter(ref address, "s", (int)this.station), address, value, this.sumCheck, this.watiingTime);
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            OperateResult<byte[]> operateResult2 = this.ReadBase(operateResult1.Content);
            if (!operateResult2.IsSuccess)
                return (OperateResult)operateResult2;
            return operateResult2.Content[0] != (byte)6 ? new OperateResult((int)operateResult2.Content[0], "Write Faild:" + SoftBasic.ByteToHexString(operateResult2.Content, ' ')) : OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecFxLinksOverTcp.ReadBool(System.String,System.UInt16)" />
        [EstMqttApi("ReadBoolArray", "")]
        public override OperateResult<bool[]> ReadBool(string address, ushort length)
        {
            OperateResult<byte[]> operateResult1 = MelsecFxLinksOverTcp.BuildReadCommand((byte)EstHelper.ExtractParameter(ref address, "s", (int)this.station), address, length, true, this.sumCheck, this.watiingTime);
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult1);
            OperateResult<byte[]> operateResult2 = this.ReadBase(operateResult1.Content);
            if (!operateResult2.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult2);
            if (operateResult2.Content[0] != (byte)2)
                return new OperateResult<bool[]>((int)operateResult2.Content[0], "Read Faild:" + SoftBasic.ByteToHexString(operateResult2.Content, ' '));
            byte[] numArray = new byte[(int)length];
            Array.Copy((Array)operateResult2.Content, 5, (Array)numArray, 0, (int)length);
            return OperateResult.CreateSuccessResult<bool[]>(((IEnumerable<byte>)numArray).Select<byte, bool>((Func<byte, bool>)(m => m == (byte)49)).ToArray<bool>());
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecFxLinksOverTcp.Write(System.String,System.Boolean[])" />
        [EstMqttApi("WriteBoolArray", "")]
        public override OperateResult Write(string address, bool[] value)
        {
            OperateResult<byte[]> operateResult1 = MelsecFxLinksOverTcp.BuildWriteBoolCommand((byte)EstHelper.ExtractParameter(ref address, "s", (int)this.station), address, value, this.sumCheck, this.watiingTime);
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            OperateResult<byte[]> operateResult2 = this.ReadBase(operateResult1.Content);
            if (!operateResult2.IsSuccess)
                return (OperateResult)operateResult2;
            return operateResult2.Content[0] != (byte)6 ? new OperateResult((int)operateResult2.Content[0], "Write Faild:" + SoftBasic.ByteToHexString(operateResult2.Content, ' ')) : OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecFxLinksOverTcp.StartPLC(System.String)" />
        [EstMqttApi(Description = "Start the PLC operation, you can carry additional parameter information and specify the station number. Example: s=2; Note: The semicolon is required.")]
        public OperateResult StartPLC(string parameter = "")
        {
            OperateResult<byte[]> operateResult1 = MelsecFxLinksOverTcp.BuildStart((byte)EstHelper.ExtractParameter(ref parameter, "s", (int)this.station), this.sumCheck, this.watiingTime);
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            OperateResult<byte[]> operateResult2 = this.ReadBase(operateResult1.Content);
            if (!operateResult2.IsSuccess)
                return (OperateResult)operateResult2;
            return operateResult2.Content[0] != (byte)6 ? new OperateResult((int)operateResult2.Content[0], "Start Faild:" + SoftBasic.ByteToHexString(operateResult2.Content, ' ')) : OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecFxLinksOverTcp.StopPLC(System.String)" />
        [EstMqttApi(Description = "Stop PLC operation, you can carry additional parameter information and specify the station number. Example: s=2; Note: The semicolon is required.")]
        public OperateResult StopPLC(string parameter = "")
        {
            OperateResult<byte[]> operateResult1 = MelsecFxLinksOverTcp.BuildStop((byte)EstHelper.ExtractParameter(ref parameter, "s", (int)this.station), this.sumCheck, this.watiingTime);
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            OperateResult<byte[]> operateResult2 = this.ReadBase(operateResult1.Content);
            if (!operateResult2.IsSuccess)
                return (OperateResult)operateResult2;
            return operateResult2.Content[0] != (byte)6 ? new OperateResult((int)operateResult2.Content[0], "Stop Faild:" + SoftBasic.ByteToHexString(operateResult2.Content, ' ')) : OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecFxLinksOverTcp.ReadPlcType(System.String)" />
        [EstMqttApi(Description = "Read the PLC model information, you can carry additional parameter information, and specify the station number. Example: s=2; Note: The semicolon is required.")]
        public OperateResult<string> ReadPlcType(string parameter = "")
        {
            OperateResult<byte[]> operateResult1 = MelsecFxLinksOverTcp.BuildReadPlcType((byte)EstHelper.ExtractParameter(ref parameter, "s", (int)this.station), this.sumCheck, this.watiingTime);
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<string>((OperateResult)operateResult1);
            OperateResult<byte[]> operateResult2 = this.ReadBase(operateResult1.Content);
            if (!operateResult2.IsSuccess)
                return OperateResult.CreateFailedResult<string>((OperateResult)operateResult2);
            return operateResult2.Content[0] != (byte)6 ? new OperateResult<string>((int)operateResult2.Content[0], "ReadPlcType Faild:" + SoftBasic.ByteToHexString(operateResult2.Content, ' ')) : MelsecFxLinksOverTcp.GetPlcTypeFromCode(Encoding.ASCII.GetString(operateResult2.Content, 5, 2));
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("MelsecFxLinks[{0}:{1}]", (object)this.PortName, (object)this.BaudRate);
    }
}
