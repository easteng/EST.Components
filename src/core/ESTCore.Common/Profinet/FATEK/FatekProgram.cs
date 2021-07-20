// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.FATEK.FatekProgram
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

namespace ESTCore.Common.Profinet.FATEK
{
    /// <summary>
    /// 台湾永宏公司的编程口协议，具体的地址信息请查阅api文档信息，地址允许携带站号信息，例如：s=2;D100<br />
    /// The programming port protocol of Taiwan Yonghong company,
    /// please refer to the api document for specific address information, The address can carry station number information, such as s=2;D100
    /// </summary>
    /// <remarks>
    /// <inheritdoc cref="T:ESTCore.Common.Profinet.FATEK.FatekProgramOverTcp" path="remarks" />
    /// </remarks>
    /// <example>
    /// <inheritdoc cref="T:ESTCore.Common.Profinet.FATEK.FatekProgramOverTcp" path="example" />
    /// </example>
    public class FatekProgram : SerialDeviceBase
    {
        private byte station = 1;

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.FATEK.FatekProgramOverTcp.#ctor" />
        public FatekProgram()
        {
            this.ByteTransform = (IByteTransform)new RegularByteTransform();
            this.WordLength = (ushort)1;
        }

        /// <inheritdoc cref="P:ESTCore.Common.Profinet.FATEK.FatekProgramOverTcp.Station" />
        public byte Station
        {
            get => this.station;
            set => this.station = value;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.FATEK.FatekProgramOverTcp.Read(System.String,System.UInt16)" />
        [EstMqttApi("ReadByteArray", "")]
        public override OperateResult<byte[]> Read(string address, ushort length)
        {
            OperateResult<List<byte[]>> operateResult1 = FatekProgramOverTcp.BuildReadCommand(this.station, address, length, false);
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult1);
            List<byte> byteList = new List<byte>();
            int[] array = SoftBasic.SplitIntegerToArray((int)length, (int)byte.MaxValue);
            for (int index = 0; index < operateResult1.Content.Count; ++index)
            {
                OperateResult<byte[]> operateResult2 = this.ReadBase(operateResult1.Content[index]);
                if (!operateResult2.IsSuccess)
                    return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult2);
                OperateResult operateResult3 = FatekProgramOverTcp.CheckResponse(operateResult2.Content);
                if (!operateResult3.IsSuccess)
                    return operateResult3.ConvertFailed<byte[]>();
                byteList.AddRange((IEnumerable<byte>)FatekProgramOverTcp.ExtraResponse(operateResult2.Content, (ushort)array[index]));
            }
            return OperateResult.CreateSuccessResult<byte[]>(byteList.ToArray());
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.FATEK.FatekProgramOverTcp.Write(System.String,System.Byte[])" />
        [EstMqttApi("WriteByteArray", "")]
        public override OperateResult Write(string address, byte[] value)
        {
            OperateResult<byte[]> operateResult1 = FatekProgramOverTcp.BuildWriteByteCommand(this.station, address, value);
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            OperateResult<byte[]> operateResult2 = this.ReadBase(operateResult1.Content);
            if (!operateResult2.IsSuccess)
                return (OperateResult)operateResult2;
            OperateResult operateResult3 = FatekProgramOverTcp.CheckResponse(operateResult2.Content);
            return !operateResult3.IsSuccess ? operateResult3 : OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.FATEK.FatekProgramOverTcp.ReadBool(System.String,System.UInt16)" />
        [EstMqttApi("ReadBoolArray", "")]
        public override OperateResult<bool[]> ReadBool(string address, ushort length)
        {
            OperateResult<List<byte[]>> operateResult1 = FatekProgramOverTcp.BuildReadCommand(this.station, address, length, true);
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult1);
            List<bool> boolList = new List<bool>();
            int[] array = SoftBasic.SplitIntegerToArray((int)length, (int)byte.MaxValue);
            for (int index = 0; index < operateResult1.Content.Count; ++index)
            {
                OperateResult<byte[]> operateResult2 = this.ReadBase(operateResult1.Content[index]);
                if (!operateResult2.IsSuccess)
                    return OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult2);
                OperateResult operateResult3 = FatekProgramOverTcp.CheckResponse(operateResult2.Content);
                if (!operateResult3.IsSuccess)
                    return operateResult3.ConvertFailed<bool[]>();
                boolList.AddRange(((IEnumerable<byte>)operateResult2.Content.SelectMiddle<byte>(6, array[index])).Select<byte, bool>((Func<byte, bool>)(m => m == (byte)49)));
            }
            return OperateResult.CreateSuccessResult<bool[]>(boolList.ToArray());
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.FATEK.FatekProgramOverTcp.Write(System.String,System.Boolean[])" />
        [EstMqttApi("WriteBoolArray", "")]
        public override OperateResult Write(string address, bool[] value)
        {
            OperateResult<byte[]> operateResult1 = FatekProgramOverTcp.BuildWriteBoolCommand(this.station, address, value);
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            OperateResult<byte[]> operateResult2 = this.ReadBase(operateResult1.Content);
            if (!operateResult2.IsSuccess)
                return (OperateResult)operateResult2;
            OperateResult operateResult3 = FatekProgramOverTcp.CheckResponse(operateResult2.Content);
            return !operateResult3.IsSuccess ? operateResult3 : OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("FatekProgram[{0}:{1}]", (object)this.PortName, (object)this.BaudRate);
    }
}
