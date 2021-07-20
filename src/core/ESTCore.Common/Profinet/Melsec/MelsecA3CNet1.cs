// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Melsec.MelsecA3CNet1
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Core;
using ESTCore.Common.Reflection;
using ESTCore.Common.Serial;

using System;

namespace ESTCore.Common.Profinet.Melsec
{
    /// <summary>
    /// 基于Qna 兼容3C帧的格式一的通讯，具体的地址需要参照三菱的基本地址<br />
    /// Based on Qna-compatible 3C frame format one communication, the specific address needs to refer to the basic address of Mitsubishi.
    /// </summary>
    /// <remarks>
    /// <inheritdoc cref="T:ESTCore.Common.Profinet.Melsec.MelsecA3CNet1OverTcp" path="remarks" />
    /// </remarks>
    /// <example>
    /// <inheritdoc cref="T:ESTCore.Common.Profinet.Melsec.MelsecA3CNet1OverTcp" path="example" />
    /// </example>
    public class MelsecA3CNet1 : SerialDeviceBase
    {
        private byte station = 0;

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecA3CNet1OverTcp.#ctor" />
        public MelsecA3CNet1()
        {
            this.ByteTransform = (IByteTransform)new RegularByteTransform();
            this.WordLength = (ushort)1;
        }

        /// <inheritdoc cref="P:ESTCore.Common.Profinet.Melsec.MelsecA3CNet1OverTcp.Station" />
        public byte Station
        {
            get => this.station;
            set => this.station = value;
        }

        private OperateResult<byte[]> ReadWithPackCommand(byte[] command, byte station) => this.ReadBase(MelsecA3CNet1OverTcp.PackCommand(command, station));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecA3CNet1OverTcp.Read(System.String,System.UInt16)" />
        [EstMqttApi("ReadByteArray", "")]
        public override OperateResult<byte[]> Read(string address, ushort length)
        {
            byte parameter = (byte)EstHelper.ExtractParameter(ref address, "s", (int)this.station);
            return MelsecA3CNet1OverTcp.ReadHelper(address, length, parameter, new Func<byte[], byte, OperateResult<byte[]>>(this.ReadWithPackCommand));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecA3CNet1OverTcp.Write(System.String,System.Byte[])" />
        [EstMqttApi("WriteByteArray", "")]
        public override OperateResult Write(string address, byte[] value)
        {
            byte parameter = (byte)EstHelper.ExtractParameter(ref address, "s", (int)this.station);
            return MelsecA3CNet1OverTcp.WriteHelper(address, value, parameter, new Func<byte[], byte, OperateResult<byte[]>>(this.ReadWithPackCommand));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecA3CNet1OverTcp.ReadBool(System.String,System.UInt16)" />
        [EstMqttApi("ReadBoolArray", "")]
        public override OperateResult<bool[]> ReadBool(string address, ushort length)
        {
            byte parameter = (byte)EstHelper.ExtractParameter(ref address, "s", (int)this.station);
            return MelsecA3CNet1OverTcp.ReadBoolHelper(address, length, parameter, new Func<byte[], byte, OperateResult<byte[]>>(this.ReadWithPackCommand));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecA3CNet1OverTcp.Write(System.String,System.Boolean[])" />
        [EstMqttApi("WriteBoolArray", "")]
        public override OperateResult Write(string address, bool[] value)
        {
            byte parameter = (byte)EstHelper.ExtractParameter(ref address, "s", (int)this.station);
            return MelsecA3CNet1OverTcp.WriteHelper(address, value, parameter, new Func<byte[], byte, OperateResult<byte[]>>(this.ReadWithPackCommand));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecA3CNet1OverTcp.RemoteRun" />
        [EstMqttApi]
        public OperateResult RemoteRun() => MelsecA3CNet1OverTcp.RemoteRunHelper(this.station, new Func<byte[], byte, OperateResult<byte[]>>(this.ReadWithPackCommand));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecA3CNet1OverTcp.RemoteStop" />
        [EstMqttApi]
        public OperateResult RemoteStop() => MelsecA3CNet1OverTcp.RemoteStopHelper(this.station, new Func<byte[], byte, OperateResult<byte[]>>(this.ReadWithPackCommand));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecA3CNet1OverTcp.ReadPlcType" />
        [EstMqttApi]
        public OperateResult<string> ReadPlcType() => MelsecA3CNet1OverTcp.ReadPlcTypeHelper(this.station, new Func<byte[], byte, OperateResult<byte[]>>(this.ReadWithPackCommand));

        /// <inheritdoc />
        public override string ToString() => string.Format("MelsecA3CNet1[{0}:{1}]", (object)this.PortName, (object)this.BaudRate);
    }
}
