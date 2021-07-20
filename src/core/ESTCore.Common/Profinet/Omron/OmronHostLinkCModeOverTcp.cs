// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Omron.OmronHostLinkCModeOverTcp
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Core;
using ESTCore.Common.Core.Net;
using ESTCore.Common.Reflection;

using System;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.Common.Profinet.Omron
{
    /// <summary>
    /// 欧姆龙的HostLink的C-Mode实现形式，当前的类是通过以太网透传实现。地址支持携带站号信息，例如：s=2;D100<br />
    /// The C-Mode implementation form of Omron’s HostLink, the current class is realized through Ethernet transparent transmission.
    /// Address supports carrying station number information, for example: s=2;D100
    /// </summary>
    /// <remarks>暂时只支持的字数据的读写操作，不支持位的读写操作。</remarks>
    public class OmronHostLinkCModeOverTcp : NetworkDeviceBase
    {
        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronFinsNet.#ctor" />
        public OmronHostLinkCModeOverTcp()
        {
            this.ByteTransform = (IByteTransform)new ReverseWordTransform();
            this.WordLength = (ushort)1;
            this.ByteTransform.DataFormat = DataFormat.CDAB;
            this.ByteTransform.IsStringReverseByteWord = true;
            this.SleepTime = 20;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronCipNet.#ctor(System.String,System.Int32)" />
        public OmronHostLinkCModeOverTcp(string ipAddress, int port)
          : this()
        {
            this.IpAddress = ipAddress;
            this.Port = port;
        }

        /// <inheritdoc cref="P:ESTCore.Common.Profinet.Omron.OmronHostLinkOverTcp.UnitNumber" />
        public byte UnitNumber { get; set; }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronFinsNet.Read(System.String,System.UInt16)" />
        [EstMqttApi("ReadByteArray", "")]
        public override OperateResult<byte[]> Read(string address, ushort length)
        {
            byte parameter = (byte)EstHelper.ExtractParameter(ref address, "s", (int)this.UnitNumber);
            OperateResult<byte[]> operateResult1 = OmronHostLinkCMode.BuildReadCommand(address, length, false);
            if (!operateResult1.IsSuccess)
                return operateResult1;
            OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(OmronHostLinkCMode.PackCommand(operateResult1.Content, parameter));
            if (!operateResult2.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult2);
            OperateResult<byte[]> operateResult3 = OmronHostLinkCMode.ResponseValidAnalysis(operateResult2.Content, true);
            return !operateResult3.IsSuccess ? OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult3) : OperateResult.CreateSuccessResult<byte[]>(operateResult3.Content);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronFinsNet.Write(System.String,System.Byte[])" />
        [EstMqttApi("WriteByteArray", "")]
        public override OperateResult Write(string address, byte[] value)
        {
            byte parameter = (byte)EstHelper.ExtractParameter(ref address, "s", (int)this.UnitNumber);
            OperateResult<byte[]> operateResult1 = OmronHostLinkCMode.BuildWriteWordCommand(address, value);
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(OmronHostLinkCMode.PackCommand(operateResult1.Content, parameter));
            if (!operateResult2.IsSuccess)
                return (OperateResult)operateResult2;
            OperateResult<byte[]> operateResult3 = OmronHostLinkCMode.ResponseValidAnalysis(operateResult2.Content, false);
            return !operateResult3.IsSuccess ? (OperateResult)operateResult3 : OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronHostLinkCMode.Read(System.String,System.UInt16)" />
        public override async Task<OperateResult<byte[]>> ReadAsync(
          string address,
          ushort length)
        {
            byte station = (byte)EstHelper.ExtractParameter(ref address, "s", (int)this.UnitNumber);
            OperateResult<byte[]> command = OmronHostLinkCMode.BuildReadCommand(address, length, false);
            if (!command.IsSuccess)
                return command;
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(OmronHostLinkCMode.PackCommand(command.Content, station));
            if (!read.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)read);
            OperateResult<byte[]> valid = OmronHostLinkCMode.ResponseValidAnalysis(read.Content, true);
            return valid.IsSuccess ? OperateResult.CreateSuccessResult<byte[]>(valid.Content) : OperateResult.CreateFailedResult<byte[]>((OperateResult)valid);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronHostLinkCMode.Write(System.String,System.Byte[])" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          byte[] value)
        {
            byte station = (byte)EstHelper.ExtractParameter(ref address, "s", (int)this.UnitNumber);
            OperateResult<byte[]> command = OmronHostLinkCMode.BuildWriteWordCommand(address, value);
            if (!command.IsSuccess)
                return (OperateResult)command;
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(OmronHostLinkCMode.PackCommand(command.Content, station));
            if (!read.IsSuccess)
                return (OperateResult)read;
            OperateResult<byte[]> valid = OmronHostLinkCMode.ResponseValidAnalysis(read.Content, false);
            return valid.IsSuccess ? OperateResult.CreateSuccessResult() : (OperateResult)valid;
        }

        /// <summary>读取PLC的当前的型号信息</summary>
        /// <returns>型号</returns>
        [EstMqttApi]
        public OperateResult<string> ReadPlcModel()
        {
            OperateResult<byte[]> operateResult = this.ReadFromCoreServer(OmronHostLinkCMode.PackCommand(Encoding.ASCII.GetBytes("MM"), this.UnitNumber));
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<string>((OperateResult)operateResult);
            int int32 = Convert.ToInt32(Encoding.ASCII.GetString(operateResult.Content, 5, 2), 16);
            return int32 > 0 ? new OperateResult<string>(int32, "Unknown Error") : OmronHostLinkCMode.GetModelText(Encoding.ASCII.GetString(operateResult.Content, 7, 2));
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("OmronHostLinkCModeOverTcp[{0}:{1}]", (object)this.IpAddress, (object)this.Port);
    }
}
