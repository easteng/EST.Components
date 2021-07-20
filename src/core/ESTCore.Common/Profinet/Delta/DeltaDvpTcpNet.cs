// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Delta.DeltaDvpTcpNet
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Core;
using ESTCore.Common.ModBus;
using ESTCore.Common.Reflection;

using System.Threading.Tasks;

namespace ESTCore.Common.Profinet.Delta
{
    /// <summary>
    /// 台达PLC的网口通讯类，基于Modbus-Rtu协议开发，按照台达的地址进行实现。<br />
    /// The tcp communication class of Delta PLC is developed based on the Modbus-Tcp protocol and implemented according to Delta's address.
    /// </summary>
    /// <remarks>
    /// 适用于DVP-ES/EX/EC/SS型号，DVP-SA/SC/SX/EH型号，地址参考API文档，同时地址可以携带站号信息，举例：[s=2;D100],[s=3;M100]，可以动态修改当前报文的站号信息。<br />
    /// Suitable for DVP-ES/EX/EC/SS models, DVP-SA/SC/SX/EH models, the address refers to the API document, and the address can carry station number information,
    /// for example: [s=2;D100],[s= 3;M100], you can dynamically modify the station number information of the current message.
    /// </remarks>
    /// <example>
    /// 地址的格式如下：
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
    ///     <term></term>
    ///     <term>S</term>
    ///     <term>S0-S1023</term>
    ///     <term>10</term>
    ///     <term>×</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>输入继电器</term>
    ///     <term>X</term>
    ///     <term>X0-X377</term>
    ///     <term>8</term>
    ///     <term>×</term>
    ///     <term>√</term>
    ///     <term>只读</term>
    ///   </item>
    ///   <item>
    ///     <term>输出继电器</term>
    ///     <term>Y</term>
    ///     <term>Y0-Y377</term>
    ///     <term>8</term>
    ///     <term>×</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>定时器</term>
    ///     <term>T</term>
    ///     <term>T0-T255</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term>如果是读位，就是通断继电器，如果是读字，就是当前值</term>
    ///   </item>
    ///   <item>
    ///     <term>计数器</term>
    ///     <term>C</term>
    ///     <term>C0-C255</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term>如果是读位，就是通断继电器，如果是读字，就是当前值</term>
    ///   </item>
    ///   <item>
    ///     <term>内部继电器</term>
    ///     <term>M</term>
    ///     <term>M0-M4095</term>
    ///     <term>10</term>
    ///     <term>×</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>数据寄存器</term>
    ///     <term>D</term>
    ///     <term>D0-D9999</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>×</term>
    ///     <term></term>
    ///   </item>
    /// </list>
    /// </example>
    public class DeltaDvpTcpNet : ModbusTcpNet
    {
        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Delta.DeltaDvpSerial.#ctor" />
        public DeltaDvpTcpNet() => this.ByteTransform.DataFormat = DataFormat.CDAB;

        /// <inheritdoc cref="M:ESTCore.Common.ModBus.ModbusTcpNet.#ctor(System.String,System.Int32,System.Byte)" />
        public DeltaDvpTcpNet(string ipAddress, int port = 502, byte station = 1)
          : base(ipAddress, port, station)
          => this.ByteTransform.DataFormat = DataFormat.CDAB;

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Delta.DeltaDvpSerial.Read(System.String,System.UInt16)" />
        [EstMqttApi("ReadByteArray", "Read the original byte data content from the register, the address is mainly D, T, C")]
        public override OperateResult<byte[]> Read(string address, ushort length)
        {
            OperateResult<string> operateResult = DeltaHelper.PraseDeltaDvpAddress(address, (byte)3);
            return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult) : base.Read(operateResult.Content, length);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Delta.DeltaDvpSerial.Write(System.String,System.Byte[])" />
        [EstMqttApi("WriteByteArray", "Write the original byte data content to the register, the address is mainly D, T, C")]
        public override OperateResult Write(string address, byte[] value)
        {
            OperateResult<string> operateResult = DeltaHelper.PraseDeltaDvpAddress(address, (byte)16);
            return !operateResult.IsSuccess ? (OperateResult)OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult) : base.Write(operateResult.Content, value);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Delta.DeltaDvpSerial.Read(System.String,System.UInt16)" />
        public override async Task<OperateResult<byte[]>> ReadAsync(
          string address,
          ushort length)
        {
            OperateResult<string> transModbus = DeltaHelper.PraseDeltaDvpAddress(address, (byte)3);
            if (!transModbus.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)transModbus);
            OperateResult<byte[]> operateResult = await base.ReadAsync(transModbus.Content, length);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Delta.DeltaDvpSerial.Write(System.String,System.Byte[])" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          byte[] value)
        {
            OperateResult<string> transModbus = DeltaHelper.PraseDeltaDvpAddress(address, (byte)16);
            if (!transModbus.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<byte[]>((OperateResult)transModbus);
            OperateResult operateResult = await base.WriteAsync(transModbus.Content, value);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Delta.DeltaDvpSerial.ReadBool(System.String,System.UInt16)" />
        [EstMqttApi("ReadBoolArray", "Read the contents of bool data in batches from the coil, the address is mainly X, Y, S, M, T, C")]
        public override OperateResult<bool[]> ReadBool(string address, ushort length)
        {
            OperateResult<string> operateResult = DeltaHelper.PraseDeltaDvpAddress(address, (byte)1);
            return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult) : base.ReadBool(operateResult.Content, length);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Delta.DeltaDvpTcpNet.Write(System.String,System.Boolean)" />
        [EstMqttApi("WriteBoolArray", "Read the contents of bool data in batches from the coil, the address is mainly X, Y, S, M, T, C")]
        public override OperateResult Write(string address, bool[] values)
        {
            OperateResult<string> operateResult = DeltaHelper.PraseDeltaDvpAddress(address, (byte)15);
            return !operateResult.IsSuccess ? (OperateResult)OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult) : base.Write(operateResult.Content, values);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Delta.DeltaDvpSerial.Write(System.String,System.Boolean)" />
        [EstMqttApi("WriteBool", "Write bool data content to the coil, the address is mainly Y, S, M, T, C")]
        public override OperateResult Write(string address, bool value)
        {
            OperateResult<string> operateResult = DeltaHelper.PraseDeltaDvpAddress(address, (byte)5);
            return !operateResult.IsSuccess ? (OperateResult)OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult) : base.Write(operateResult.Content, value);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Delta.DeltaDvpSerial.ReadBool(System.String,System.UInt16)" />
        public override async Task<OperateResult<bool[]>> ReadBoolAsync(
          string address,
          ushort length)
        {
            OperateResult<string> transModbus = DeltaHelper.PraseDeltaDvpAddress(address, (byte)1);
            if (!transModbus.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)transModbus);
            OperateResult<bool[]> operateResult = await base.ReadBoolAsync(transModbus.Content, length);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Delta.DeltaDvpTcpNet.Write(System.String,System.Boolean)" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          bool[] values)
        {
            OperateResult<string> transModbus = DeltaHelper.PraseDeltaDvpAddress(address, (byte)15);
            if (!transModbus.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<bool[]>((OperateResult)transModbus);
            OperateResult operateResult = await base.WriteAsync(transModbus.Content, values);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Delta.DeltaDvpSerial.Write(System.String,System.Boolean)" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          bool value)
        {
            OperateResult<string> transModbus = DeltaHelper.PraseDeltaDvpAddress(address, (byte)5);
            if (!transModbus.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<bool[]>((OperateResult)transModbus);
            OperateResult operateResult = await base.WriteAsync(transModbus.Content, value);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Write(System.String,System.Int16)" />
        [EstMqttApi("WriteInt16", "Write short data, returns whether success")]
        public override OperateResult Write(string address, short value)
        {
            OperateResult<string> operateResult = DeltaHelper.PraseDeltaDvpAddress(address, (byte)6);
            return !operateResult.IsSuccess ? (OperateResult)OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult) : base.Write(operateResult.Content, value);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Write(System.String,System.UInt16)" />
        [EstMqttApi("WriteUInt16", "Write ushort data, return whether the write was successful")]
        public override OperateResult Write(string address, ushort value)
        {
            OperateResult<string> operateResult = DeltaHelper.PraseDeltaDvpAddress(address, (byte)6);
            return !operateResult.IsSuccess ? (OperateResult)OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult) : base.Write(operateResult.Content, value);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Write(System.String,System.Int16)" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          short value)
        {
            OperateResult<string> transModbus = DeltaHelper.PraseDeltaDvpAddress(address, (byte)6);
            if (!transModbus.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<bool[]>((OperateResult)transModbus);
            OperateResult operateResult = await base.WriteAsync(transModbus.Content, value);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Write(System.String,System.UInt16)" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          ushort value)
        {
            OperateResult<string> transModbus = DeltaHelper.PraseDeltaDvpAddress(address, (byte)6);
            if (!transModbus.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<bool[]>((OperateResult)transModbus);
            OperateResult operateResult = await base.WriteAsync(transModbus.Content, value);
            return operateResult;
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("DeltaDvpTcpNet[{0}:{1}]", (object)this.IpAddress, (object)this.Port);
    }
}
