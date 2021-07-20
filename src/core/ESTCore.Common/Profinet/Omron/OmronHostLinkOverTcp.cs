// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Omron.OmronHostLinkOverTcp
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;
using ESTCore.Common.Core;
using ESTCore.Common.Core.Net;
using ESTCore.Common.Reflection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.Common.Profinet.Omron
{
    /// <summary>
    /// 欧姆龙的HostLink协议的实现，基于Tcp实现，地址支持示例 DM区:D100; CIO区:C100; Work区:W100; Holding区:H100; Auxiliary区: A100<br />
    /// Implementation of Omron's HostLink protocol, based on tcp protocol, address support example DM area: D100; CIO area: C100; Work area: W100; Holding area: H100; Auxiliary area: A100
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
    /// 欧姆龙的地址参考如下：
    /// 地址支持的列表如下：
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
    ///     <term>DM Area</term>
    ///     <term>D</term>
    ///     <term>D100,D200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>CIO Area</term>
    ///     <term>C</term>
    ///     <term>C100,C200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>Work Area</term>
    ///     <term>W</term>
    ///     <term>W100,W200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>Holding Bit Area</term>
    ///     <term>H</term>
    ///     <term>H100,H200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>Auxiliary Bit Area</term>
    ///     <term>A</term>
    ///     <term>A100,A200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    /// </list>
    /// </example>
    public class OmronHostLinkOverTcp : NetworkDeviceBase
    {
        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronFinsNet.#ctor" />
        public OmronHostLinkOverTcp()
        {
            this.ByteTransform = (IByteTransform)new ReverseWordTransform();
            this.WordLength = (ushort)1;
            this.ByteTransform.DataFormat = DataFormat.CDAB;
            this.SleepTime = 20;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronCipNet.#ctor(System.String,System.Int32)" />
        public OmronHostLinkOverTcp(string ipAddress, int port)
          : this()
        {
            this.IpAddress = ipAddress;
            this.Port = port;
        }

        /// <summary>
        /// Specifies whether or not there are network relays. Set “80” (ASCII: 38,30)
        /// when sending an FINS command to a CPU Unit on a network.Set “00” (ASCII: 30,30)
        /// when sending to a CPU Unit connected directly to the host computer.
        /// </summary>
        public byte ICF { get; set; } = 0;

        /// <inheritdoc cref="P:ESTCore.Common.Profinet.Omron.OmronFinsNet.DA2" />
        public byte DA2 { get; set; } = 0;

        /// <inheritdoc cref="P:ESTCore.Common.Profinet.Omron.OmronFinsNet.SA2" />
        public byte SA2 { get; set; }

        /// <inheritdoc cref="P:ESTCore.Common.Profinet.Omron.OmronFinsNet.SID" />
        public byte SID { get; set; } = 0;

        /// <summary>
        /// The response wait time sets the time from when the CPU Unit receives a command block until it starts
        /// to return a response.It can be set from 0 to F in hexadecimal, in units of 10 ms.
        /// If F(15) is set, the response will begin to be returned 150 ms (15 × 10 ms) after the command block was received.
        /// </summary>
        public byte ResponseWaitTime { get; set; } = 48;

        /// <summary>
        /// PLC设备的站号信息<br />
        /// PLC device station number information
        /// </summary>
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
            for (int index = 0; index < (int)length; ++index)
            {
                OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(this.PackCommand(parameter, operateResult1.Content[index]));
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
            OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(this.PackCommand(parameter, operateResult1.Content));
            if (!operateResult2.IsSuccess)
                return (OperateResult)operateResult2;
            OperateResult<byte[]> operateResult3 = OmronHostLinkOverTcp.ResponseValidAnalysis(operateResult2.Content, false);
            return !operateResult3.IsSuccess ? (OperateResult)operateResult3 : OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronFinsNet.Read(System.String,System.UInt16)" />
        public override async Task<OperateResult<byte[]>> ReadAsync(
          string address,
          ushort length)
        {
            byte station = (byte)EstHelper.ExtractParameter(ref address, "s", (int)this.UnitNumber);
            OperateResult<List<byte[]>> command = OmronFinsNetHelper.BuildReadCommand(address, length, false);
            if (!command.IsSuccess)
                return command.ConvertFailed<byte[]>();
            List<byte> contentArray = new List<byte>();
            for (int i = 0; i < (int)length; ++i)
            {
                OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(this.PackCommand(station, command.Content[i]));
                if (!read.IsSuccess)
                    return OperateResult.CreateFailedResult<byte[]>((OperateResult)read);
                OperateResult<byte[]> valid = OmronHostLinkOverTcp.ResponseValidAnalysis(read.Content, true);
                if (!valid.IsSuccess)
                    return OperateResult.CreateFailedResult<byte[]>((OperateResult)valid);
                contentArray.AddRange((IEnumerable<byte>)valid.Content);
                read = (OperateResult<byte[]>)null;
                valid = (OperateResult<byte[]>)null;
            }
            return OperateResult.CreateSuccessResult<byte[]>(contentArray.ToArray());
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronFinsNet.Write(System.String,System.Byte[])" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          byte[] value)
        {
            byte station = (byte)EstHelper.ExtractParameter(ref address, "s", (int)this.UnitNumber);
            OperateResult<byte[]> command = OmronFinsNetHelper.BuildWriteWordCommand(address, value, false);
            if (!command.IsSuccess)
                return (OperateResult)command;
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(this.PackCommand(station, command.Content));
            if (!read.IsSuccess)
                return (OperateResult)read;
            OperateResult<byte[]> valid = OmronHostLinkOverTcp.ResponseValidAnalysis(read.Content, false);
            return valid.IsSuccess ? OperateResult.CreateSuccessResult() : (OperateResult)valid;
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
                OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(this.PackCommand(parameter, operateResult1.Content[index]));
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
            OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(this.PackCommand(parameter, operateResult1.Content));
            if (!operateResult2.IsSuccess)
                return (OperateResult)operateResult2;
            OperateResult<byte[]> operateResult3 = OmronHostLinkOverTcp.ResponseValidAnalysis(operateResult2.Content, false);
            return !operateResult3.IsSuccess ? (OperateResult)operateResult3 : OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronFinsNet.ReadBool(System.String,System.UInt16)" />
        public override async Task<OperateResult<bool[]>> ReadBoolAsync(
          string address,
          ushort length)
        {
            byte station = (byte)EstHelper.ExtractParameter(ref address, "s", (int)this.UnitNumber);
            OperateResult<List<byte[]>> command = OmronFinsNetHelper.BuildReadCommand(address, length, true);
            if (!command.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)command);
            List<bool> contentArray = new List<bool>();
            for (int i = 0; i < command.Content.Count; ++i)
            {
                OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(this.PackCommand(station, command.Content[i]));
                if (!read.IsSuccess)
                    return OperateResult.CreateFailedResult<bool[]>((OperateResult)read);
                OperateResult<byte[]> valid = OmronHostLinkOverTcp.ResponseValidAnalysis(read.Content, true);
                if (!valid.IsSuccess)
                    return OperateResult.CreateFailedResult<bool[]>((OperateResult)valid);
                contentArray.AddRange(((IEnumerable<byte>)valid.Content).Select<byte, bool>((Func<byte, bool>)(m => m != (byte)0)));
                read = (OperateResult<byte[]>)null;
                valid = (OperateResult<byte[]>)null;
            }
            return OperateResult.CreateSuccessResult<bool[]>(contentArray.ToArray());
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronFinsNet.Write(System.String,System.Boolean[])" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          bool[] values)
        {
            byte station = (byte)EstHelper.ExtractParameter(ref address, "s", (int)this.UnitNumber);
            OperateResult<byte[]> command = OmronFinsNetHelper.BuildWriteWordCommand(address, ((IEnumerable<bool>)values).Select<bool, byte>((Func<bool, byte>)(m => !m ? (byte)0 : (byte)1)).ToArray<byte>(), true);
            if (!command.IsSuccess)
                return (OperateResult)command;
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(this.PackCommand(station, command.Content));
            if (!read.IsSuccess)
                return (OperateResult)read;
            OperateResult<byte[]> valid = OmronHostLinkOverTcp.ResponseValidAnalysis(read.Content, false);
            return valid.IsSuccess ? OperateResult.CreateSuccessResult() : (OperateResult)valid;
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("OmronHostLinkOverTcp[{0}:{1}]", (object)this.IpAddress, (object)this.Port);

        /// <summary>将普通的指令打包成完整的指令</summary>
        /// <param name="station">PLC的站号信息</param>
        /// <param name="cmd">fins指令</param>
        /// <returns>完整的质量</returns>
        private byte[] PackCommand(byte station, byte[] cmd)
        {
            cmd = SoftBasic.BytesToAsciiBytes(cmd);
            byte[] bytes = new byte[18 + cmd.Length];
            bytes[0] = (byte)64;
            bytes[1] = SoftBasic.BuildAsciiBytesFrom(station)[0];
            bytes[2] = SoftBasic.BuildAsciiBytesFrom(station)[1];
            bytes[3] = (byte)70;
            bytes[4] = (byte)65;
            bytes[5] = this.ResponseWaitTime;
            bytes[6] = SoftBasic.BuildAsciiBytesFrom(this.ICF)[0];
            bytes[7] = SoftBasic.BuildAsciiBytesFrom(this.ICF)[1];
            bytes[8] = SoftBasic.BuildAsciiBytesFrom(this.DA2)[0];
            bytes[9] = SoftBasic.BuildAsciiBytesFrom(this.DA2)[1];
            bytes[10] = SoftBasic.BuildAsciiBytesFrom(this.SA2)[0];
            bytes[11] = SoftBasic.BuildAsciiBytesFrom(this.SA2)[1];
            bytes[12] = SoftBasic.BuildAsciiBytesFrom(this.SID)[0];
            bytes[13] = SoftBasic.BuildAsciiBytesFrom(this.SID)[1];
            bytes[bytes.Length - 2] = (byte)42;
            bytes[bytes.Length - 1] = (byte)13;
            cmd.CopyTo((Array)bytes, 14);
            int num = (int)bytes[0];
            for (int index = 1; index < bytes.Length - 4; ++index)
                num ^= (int)bytes[index];
            bytes[bytes.Length - 4] = SoftBasic.BuildAsciiBytesFrom((byte)num)[0];
            bytes[bytes.Length - 3] = SoftBasic.BuildAsciiBytesFrom((byte)num)[1];
            Console.WriteLine(Encoding.ASCII.GetString(bytes));
            return bytes;
        }

        /// <summary>验证欧姆龙的Fins-TCP返回的数据是否正确的数据，如果正确的话，并返回所有的数据内容</summary>
        /// <param name="response">来自欧姆龙返回的数据内容</param>
        /// <param name="isRead">是否读取</param>
        /// <returns>带有是否成功的结果对象</returns>
        public static OperateResult<byte[]> ResponseValidAnalysis(
          byte[] response,
          bool isRead)
        {
            if (response.Length < 27)
                return new OperateResult<byte[]>(StringResources.Language.OmronReceiveDataError + " Source Data: " + response.ToHexString(' '));
            int result;
            if (int.TryParse(Encoding.ASCII.GetString(response, 19, 4), out result))
            {
                byte[] numArray = (byte[])null;
                if (response.Length > 27)
                    numArray = SoftBasic.HexStringToBytes(Encoding.ASCII.GetString(response, 23, response.Length - 27));
                if (result <= 0)
                    return OperateResult.CreateSuccessResult<byte[]>(numArray);
                OperateResult<byte[]> operateResult = new OperateResult<byte[]>();
                operateResult.ErrorCode = result;
                operateResult.Content = numArray;
                return operateResult;
            }
            return new OperateResult<byte[]>("Parse error code failed, [" + Encoding.ASCII.GetString(response, 19, 4) + "] " + Environment.NewLine + "Source Data: " + response.ToHexString(' '));
        }
    }
}
