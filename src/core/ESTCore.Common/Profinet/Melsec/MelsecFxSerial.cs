// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Melsec.MelsecFxSerial
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Core;
using ESTCore.Common.Reflection;
using ESTCore.Common.Serial;

using System;
using System.Threading.Tasks;

namespace ESTCore.Common.Profinet.Melsec
{
    /// <summary>
    /// 三菱的串口通信的对象，适用于读取FX系列的串口数据，支持的类型参考文档说明<br />
    /// Mitsubishi's serial communication object is suitable for reading serial data of the FX series. Refer to the documentation for the supported types.
    /// </summary>
    /// <remarks>
    /// <inheritdoc cref="T:ESTCore.Common.Profinet.Melsec.MelsecFxSerialOverTcp" path="remarks" />
    /// </remarks>
    /// <example>
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\MelsecFxSerial.cs" region="Usage" title="简单的使用" />
    /// </example>
    public class MelsecFxSerial : SerialDeviceBase
    {
        /// <summary>实例化一个默认的对象</summary>
        public MelsecFxSerial()
        {
            this.ByteTransform = (IByteTransform)new RegularByteTransform();
            this.WordLength = (ushort)1;
            this.IsNewVersion = true;
            this.ByteTransform.IsStringReverseByteWord = true;
        }

        /// <inheritdoc cref="P:ESTCore.Common.Profinet.Melsec.MelsecFxSerialOverTcp.IsNewVersion" />
        public bool IsNewVersion { get; set; }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecFxSerialOverTcp.Read(System.String,System.UInt16)" />
        [EstMqttApi("ReadByteArray", "")]
        public override OperateResult<byte[]> Read(string address, ushort length) => MelsecFxSerialOverTcp.ReadHelper(address, length, new Func<byte[], OperateResult<byte[]>>(((SerialBase)this).ReadBase), this.IsNewVersion);

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecFxSerialOverTcp.Write(System.String,System.Byte[])" />
        [EstMqttApi("WriteByteArray", "")]
        public override OperateResult Write(string address, byte[] value) => MelsecFxSerialOverTcp.WriteHelper(address, value, new Func<byte[], OperateResult<byte[]>>(((SerialBase)this).ReadBase), this.IsNewVersion);

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecFxSerialOverTcp.ReadBool(System.String,System.UInt16)" />
        [EstMqttApi("ReadBoolArray", "")]
        public override OperateResult<bool[]> ReadBool(string address, ushort length) => MelsecFxSerialOverTcp.ReadBoolHelper(address, length, new Func<byte[], OperateResult<byte[]>>(((SerialBase)this).ReadBase));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecFxSerialOverTcp.Write(System.String,System.Boolean)" />
        [EstMqttApi("WriteBool", "")]
        public override OperateResult Write(string address, bool value) => MelsecFxSerialOverTcp.WriteHelper(address, value, new Func<byte[], OperateResult<byte[]>>(((SerialBase)this).ReadBase));

        /// <inheritdoc />
        public override async Task<OperateResult> WriteAsync(
          string address,
          bool value)
        {
            OperateResult operateResult = await Task.Run<OperateResult>((Func<OperateResult>)(() => this.Write(address, value)));
            return operateResult;
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("MelsecFxSerial[{0}:{1}]", (object)this.PortName, (object)this.BaudRate);
    }
}
