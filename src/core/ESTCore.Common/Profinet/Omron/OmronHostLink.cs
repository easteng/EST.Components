// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Omron.OmronHostLink
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

namespace ESTCore.Common.Profinet.Omron
{
    /// <summary>
    /// 欧姆龙的HostLink协议的实现，地址支持示例 DM区:D100; CIO区:C100; Work区:W100; Holding区:H100; Auxiliary区: A100<br />
    /// Implementation of Omron's HostLink protocol, address support example DM area: D100; CIO area: C100; Work area: W100; Holding area: H100; Auxiliary area: A100
    /// </summary>
    /// <remarks>
    /// 感谢 深圳～拾忆 的测试，地址可以携带站号信息，例如 s=2;D100
    /// <br />
    /// <note type="important">
    /// 如果发现串口线和usb同时打开才能通信的情况，需要按照如下的操作：<br />
    /// 串口线不是标准的串口线，电脑的串口线的235引脚分别接PLC的329引脚，45线短接，就可以通讯，感谢 深圳-小君(QQ932507362)提供的解决方案。
    /// </note>
    /// </remarks>
    /// <example>
    /// <inheritdoc cref="T:ESTCore.Common.Profinet.Omron.OmronHostLinkOverTcp" path="example" />
    /// </example>
    public class OmronHostLink : SerialDeviceBase
    {
        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronFinsNet.#ctor" />
        public OmronHostLink()
        {
            this.ByteTransform = (IByteTransform)new ReverseWordTransform();
            this.WordLength = (ushort)1;
            this.ByteTransform.DataFormat = DataFormat.CDAB;
            this.ByteTransform.IsStringReverseByteWord = true;
        }

        /// <inheritdoc cref="P:ESTCore.Common.Profinet.Omron.OmronHostLinkOverTcp.ICF" />
        public byte ICF { get; set; } = 0;

        /// <inheritdoc cref="P:ESTCore.Common.Profinet.Omron.OmronHostLinkOverTcp.DA2" />
        public byte DA2 { get; set; } = 0;

        /// <inheritdoc cref="P:ESTCore.Common.Profinet.Omron.OmronHostLinkOverTcp.SA2" />
        public byte SA2 { get; set; }

        /// <inheritdoc cref="P:ESTCore.Common.Profinet.Omron.OmronHostLinkOverTcp.SID" />
        public byte SID { get; set; } = 0;

        /// <inheritdoc cref="P:ESTCore.Common.Profinet.Omron.OmronHostLinkOverTcp.ResponseWaitTime" />
        public byte ResponseWaitTime { get; set; } = 48;

        /// <inheritdoc cref="P:ESTCore.Common.Profinet.Omron.OmronHostLinkOverTcp.UnitNumber" />
        public byte UnitNumber { get; set; }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronFinsNet.Read(System.String,System.UInt16)" />
        [EstMqttApi("ReadByteArray", "")]
        public override OperateResult<byte[]> Read(string address, ushort length)
        {
            byte parameter = (byte)EstHelper.ExtractParameter(ref address, "s", (int)this.UnitNumber);
            OperateResult<List<byte[]>> operateResult1 = OmronFinsNetHelper.BuildReadCommand(address, length, false);
            if (!operateResult1.IsSuccess)
                return operateResult1.ConvertFailed<byte[]>();
            List<byte> byteList = new List<byte>();
            for (int index = 0; index < operateResult1.Content.Count; ++index)
            {
                OperateResult<byte[]> operateResult2 = this.ReadBase(this.PackCommand(parameter, operateResult1.Content[index]));
                if (!operateResult2.IsSuccess)
                    return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult2);
                OperateResult<byte[]> operateResult3 = OmronHostLinkOverTcp.ResponseValidAnalysis(operateResult2.Content, true);
                if (!operateResult3.IsSuccess)
                    return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult3);
                byteList.AddRange((IEnumerable<byte>)operateResult3.Content);
            }
            return OperateResult.CreateSuccessResult<byte[]>(byteList.ToArray());
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronFinsNet.Write(System.String,System.Byte[])" />
        [EstMqttApi("WriteByteArray", "")]
        public override OperateResult Write(string address, byte[] value)
        {
            byte parameter = (byte)EstHelper.ExtractParameter(ref address, "s", (int)this.UnitNumber);
            OperateResult<byte[]> operateResult1 = OmronFinsNetHelper.BuildWriteWordCommand(address, value, false);
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            OperateResult<byte[]> operateResult2 = this.ReadBase(this.PackCommand(parameter, operateResult1.Content));
            if (!operateResult2.IsSuccess)
                return (OperateResult)operateResult2;
            OperateResult<byte[]> operateResult3 = OmronHostLinkOverTcp.ResponseValidAnalysis(operateResult2.Content, false);
            return !operateResult3.IsSuccess ? (OperateResult)operateResult3 : OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronFinsNet.ReadBool(System.String,System.UInt16)" />
        [EstMqttApi("ReadBoolArray", "")]
        public override OperateResult<bool[]> ReadBool(string address, ushort length)
        {
            byte parameter = (byte)EstHelper.ExtractParameter(ref address, "s", (int)this.UnitNumber);
            OperateResult<List<byte[]>> operateResult1 = OmronFinsNetHelper.BuildReadCommand(address, length, true);
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult1);
            List<bool> boolList = new List<bool>();
            for (int index = 0; index < operateResult1.Content.Count; ++index)
            {
                OperateResult<byte[]> operateResult2 = this.ReadBase(this.PackCommand(parameter, operateResult1.Content[index]));
                if (!operateResult2.IsSuccess)
                    return OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult2);
                OperateResult<byte[]> operateResult3 = OmronHostLinkOverTcp.ResponseValidAnalysis(operateResult2.Content, true);
                if (!operateResult3.IsSuccess)
                    return OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult3);
                boolList.AddRange(((IEnumerable<byte>)operateResult3.Content).Select<byte, bool>((Func<byte, bool>)(m => m != (byte)0)));
            }
            return OperateResult.CreateSuccessResult<bool[]>(boolList.ToArray());
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronFinsNet.Write(System.String,System.Boolean[])" />
        [EstMqttApi("WriteBoolArray", "")]
        public override OperateResult Write(string address, bool[] values)
        {
            byte parameter = (byte)EstHelper.ExtractParameter(ref address, "s", (int)this.UnitNumber);
            OperateResult<byte[]> operateResult1 = OmronFinsNetHelper.BuildWriteWordCommand(address, ((IEnumerable<bool>)values).Select<bool, byte>((Func<bool, byte>)(m => !m ? (byte)0 : (byte)1)).ToArray<byte>(), true);
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            OperateResult<byte[]> operateResult2 = this.ReadBase(this.PackCommand(parameter, operateResult1.Content));
            if (!operateResult2.IsSuccess)
                return (OperateResult)operateResult2;
            OperateResult<byte[]> operateResult3 = OmronHostLinkOverTcp.ResponseValidAnalysis(operateResult2.Content, false);
            return !operateResult3.IsSuccess ? (OperateResult)operateResult3 : OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("OmronHostLink[{0}:{1}]", (object)this.PortName, (object)this.BaudRate);

        /// <summary>将普通的指令打包成完整的指令</summary>
        /// <param name="station">PLC的站号信息</param>
        /// <param name="cmd">fins指令</param>
        /// <returns>完整的质量</returns>
        private byte[] PackCommand(byte station, byte[] cmd)
        {
            cmd = SoftBasic.BytesToAsciiBytes(cmd);
            byte[] numArray = new byte[18 + cmd.Length];
            numArray[0] = (byte)64;
            numArray[1] = SoftBasic.BuildAsciiBytesFrom(station)[0];
            numArray[2] = SoftBasic.BuildAsciiBytesFrom(station)[1];
            numArray[3] = (byte)70;
            numArray[4] = (byte)65;
            numArray[5] = this.ResponseWaitTime;
            numArray[6] = SoftBasic.BuildAsciiBytesFrom(this.ICF)[0];
            numArray[7] = SoftBasic.BuildAsciiBytesFrom(this.ICF)[1];
            numArray[8] = SoftBasic.BuildAsciiBytesFrom(this.DA2)[0];
            numArray[9] = SoftBasic.BuildAsciiBytesFrom(this.DA2)[1];
            numArray[10] = SoftBasic.BuildAsciiBytesFrom(this.SA2)[0];
            numArray[11] = SoftBasic.BuildAsciiBytesFrom(this.SA2)[1];
            numArray[12] = SoftBasic.BuildAsciiBytesFrom(this.SID)[0];
            numArray[13] = SoftBasic.BuildAsciiBytesFrom(this.SID)[1];
            numArray[numArray.Length - 2] = (byte)42;
            numArray[numArray.Length - 1] = (byte)13;
            cmd.CopyTo((Array)numArray, 14);
            int num = (int)numArray[0];
            for (int index = 1; index < numArray.Length - 4; ++index)
                num ^= (int)numArray[index];
            numArray[numArray.Length - 4] = SoftBasic.BuildAsciiBytesFrom((byte)num)[0];
            numArray[numArray.Length - 3] = SoftBasic.BuildAsciiBytesFrom((byte)num)[1];
            return numArray;
        }
    }
}
