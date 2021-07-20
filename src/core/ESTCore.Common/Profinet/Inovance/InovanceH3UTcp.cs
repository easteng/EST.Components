// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Inovance.InovanceH3UTcp
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.ModBus;
using ESTCore.Common.Reflection;

using System.Threading.Tasks;

namespace ESTCore.Common.Profinet.Inovance
{
    /// <summary>
    /// 汇川的网络通信协议，适用于H3U, XP 等系列，底层走的是MODBUS-TCP协议，地址说明参见标记<br />
    /// Huichuan's network communication protocol is suitable for H3U, XP and other series. The bottom layer is MODBUS-TCP protocol. For the address description, please refer to the mark
    /// </summary>
    /// <remarks>
    /// 本组件适用的系列为H3U 系列和 XP 系列 PLC（H2UXP、 H2SXP、 H1UXP、 H1SXP 等系列） 其中XP 系列控制器不支持以太网通讯<br />
    /// H3U 系列控制器支持 M/SM/S/T/C/X/Y 等 bit 型变量（也称线圈） 的访问、 D/SD/R/T/C 等 word 型变量的访问；<br /><br />
    /// <c>我们先来看看H3U系列</c><br />
    /// 线圈、 位元件、位变量地址定义
    /// <list type="table">
    ///   <listheader>
    ///     <term>地址名称</term>
    ///     <term>地址代号</term>
    ///     <term>地址范围</term>
    ///     <term>地址进制</term>
    ///     <term>备注</term>
    ///   </listheader>
    ///   <item>
    ///     <term>中间寄电器</term>
    ///     <term>M</term>
    ///     <term>M0-M7679，M8000-M8511</term>
    ///     <term>10</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term></term>
    ///     <term>SM</term>
    ///     <term>SM0-SM1023</term>
    ///     <term>10</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term></term>
    ///     <term>S</term>
    ///     <term>S0-S4095</term>
    ///     <term>10</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>定时器</term>
    ///     <term>T</term>
    ///     <term>T0-T511</term>
    ///     <term>10</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>计数器</term>
    ///     <term>C</term>
    ///     <term>C0-C255</term>
    ///     <term>10</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>输入</term>
    ///     <term>X</term>
    ///     <term>X0-X377 或者X0.0-X37.7</term>
    ///     <term>8</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>输出</term>
    ///     <term>Y</term>
    ///     <term>Y0-Y377 或者Y0.0-Y37.7</term>
    ///     <term>8</term>
    ///     <term></term>
    ///   </item>
    /// </list>
    /// 寄存器、 字元件、字变量地址定义：
    /// <list type="table">
    ///   <listheader>
    ///     <term>地址名称</term>
    ///     <term>地址代号</term>
    ///     <term>地址范围</term>
    ///     <term>地址进制</term>
    ///     <term>备注</term>
    ///   </listheader>
    ///   <item>
    ///     <term>数据寄存器</term>
    ///     <term>D</term>
    ///     <term>D0-D8511</term>
    ///     <term>10</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>数据寄存器</term>
    ///     <term>SD</term>
    ///     <term>SD0-SD1023</term>
    ///     <term>10</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term></term>
    ///     <term>R</term>
    ///     <term>R0-R32767</term>
    ///     <term>10</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>定时器</term>
    ///     <term>T</term>
    ///     <term>T0-T511</term>
    ///     <term>10</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>计数器</term>
    ///     <term>C</term>
    ///     <term>C0-C199,C200-C255</term>
    ///     <term>10</term>
    ///     <term>其实C200-C255的计数器是32位的</term>
    ///   </item>
    /// </list>
    /// <c>我们再来看看XP系列，就是少了一点访问的数据类型，然后，地址范围也不一致</c><br />
    /// 线圈、 位元件、位变量地址定义
    /// <list type="table">
    ///   <listheader>
    ///     <term>地址名称</term>
    ///     <term>地址代号</term>
    ///     <term>地址范围</term>
    ///     <term>地址进制</term>
    ///     <term>备注</term>
    ///   </listheader>
    ///   <item>
    ///     <term>中间寄电器</term>
    ///     <term>M</term>
    ///     <term>M0-M3071，M8000-M8511</term>
    ///     <term>10</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term></term>
    ///     <term>S</term>
    ///     <term>S0-S999</term>
    ///     <term>10</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>定时器</term>
    ///     <term>T</term>
    ///     <term>T0-T255</term>
    ///     <term>10</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>计数器</term>
    ///     <term>C</term>
    ///     <term>C0-C255</term>
    ///     <term>10</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>输入</term>
    ///     <term>X</term>
    ///     <term>X0-X377 或者X0.0-X37.7</term>
    ///     <term>8</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>输出</term>
    ///     <term>Y</term>
    ///     <term>Y0-Y377 或者Y0.0-Y37.7</term>
    ///     <term>8</term>
    ///     <term></term>
    ///   </item>
    /// </list>
    /// 寄存器、 字元件、字变量地址定义：
    /// <list type="table">
    ///   <listheader>
    ///     <term>地址名称</term>
    ///     <term>地址代号</term>
    ///     <term>地址范围</term>
    ///     <term>地址进制</term>
    ///     <term>备注</term>
    ///   </listheader>
    ///   <item>
    ///     <term>数据寄存器</term>
    ///     <term>D</term>
    ///     <term>D0-D8511</term>
    ///     <term>10</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>定时器</term>
    ///     <term>T</term>
    ///     <term>T0-T255</term>
    ///     <term>10</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>计数器</term>
    ///     <term>C</term>
    ///     <term>C0-C199,C200-C255</term>
    ///     <term>10</term>
    ///     <term>其实C200-C255的计数器是32位的</term>
    ///   </item>
    /// </list>
    /// </remarks>
    public class InovanceH3UTcp : ModbusTcpNet
    {
        /// <summary>
        /// 实例化一个汇川H3U, XP 系列的网络通讯协议<br />
        /// Instantiate a network communication protocol of HuiChuan H3U, XP series
        /// </summary>
        public InovanceH3UTcp()
        {
        }

        /// <summary>
        /// 指定服务器地址，端口号，客户端自己的站号来实例化一个汇川H3U, XP 系列的网络通讯协议<br />
        /// Specify the server address, port number, and client's own station number to instantiate a network communication protocol of Huichuan H3U, XP series
        /// </summary>
        /// <param name="ipAddress">服务器的Ip地址</param>
        /// <param name="port">服务器的端口号</param>
        /// <param name="station">客户端自身的站号</param>
        public InovanceH3UTcp(string ipAddress, int port = 502, byte station = 1)
          : base(ipAddress, port, station)
        {
        }

        /// <summary>
        /// 按字读取汇川PLC的数据信息，对于H3U情况，可以输入D0,SD0,R0,T0,C0类型地址，对于XP系列而言，可以输入D0,T0,C0<br />
        /// Read Huichuan PLC's data information by word. For H3U, you can enter D0, SD0, R0, T0, C0 type addresses. For XP series, you can enter D0, T0, C0.
        /// </summary>
        /// <param name="address">PLC的真实的地址信息，对于H3U情况，可以输入D0,SD0,R0,T0,C0类型地址，对于XP系列而言，可以输入D0,T0,C0</param>
        /// <param name="length">读取的数据的长度，按照字为单位</param>
        /// <returns>包含是否成功的结果对象信息</returns>
        [EstMqttApi("ReadByteArray", "")]
        public override OperateResult<byte[]> Read(string address, ushort length)
        {
            OperateResult<string> operateResult = InovanceHelper.PraseInovanceH3UAddress(address, (byte)3);
            return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult) : base.Read(operateResult.Content, length);
        }

        /// <summary>
        /// 按字写入汇川PLC的数据信息，对于H3U情况，可以输入D0,SD0,R0,T0,C0类型地址，对于XP系列而言，可以输入D0,T0,C0<br />
        /// Write the data information of Huichuan PLC by word. For H3U, you can enter D0, SD0, R0, T0, and C0 type addresses. For XP series, you can enter D0, T0, and C0.
        /// </summary>
        /// <param name="address">PLC的真实的地址信息，对于H3U情况，可以输入D0,SD0,R0,T0,C0类型地址，对于XP系列而言，可以输入D0,T0,C0</param>
        /// <param name="value">等待写入的原始数据，长度为2的倍数</param>
        /// <returns>是否写入成功的结果信息</returns>
        [EstMqttApi("WriteByteArray", "")]
        public override OperateResult Write(string address, byte[] value)
        {
            OperateResult<string> operateResult = InovanceHelper.PraseInovanceH3UAddress(address, (byte)16);
            return !operateResult.IsSuccess ? (OperateResult)OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult) : base.Write(operateResult.Content, value);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Inovance.InovanceH3UTcp.Read(System.String,System.UInt16)" />
        public override async Task<OperateResult<byte[]>> ReadAsync(
          string address,
          ushort length)
        {
            OperateResult<string> transModbus = InovanceHelper.PraseInovanceH3UAddress(address, (byte)3);
            if (!transModbus.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)transModbus);
            OperateResult<byte[]> operateResult = await base.ReadAsync(transModbus.Content, length);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Inovance.InovanceH3UTcp.Write(System.String,System.Byte[])" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          byte[] value)
        {
            OperateResult<string> transModbus = InovanceHelper.PraseInovanceH3UAddress(address, (byte)16);
            if (!transModbus.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<byte[]>((OperateResult)transModbus);
            OperateResult operateResult = await base.WriteAsync(transModbus.Content, value);
            return operateResult;
        }

        /// <summary>
        /// 按位读取汇川PLC的数据信息，对于H3U情况，可以输入M0,SM0,S0,T0,C0,X0,Y0类型地址，对于XP系列而言，可以输入M0,S0,T0,C0,X0,Y0<br />
        /// Read the data information of Huichuan PLC bit by bit. For H3U, you can enter M0, SM0, S0, T0, C0, X0, Y0 type addresses. For XP series, you can enter M0, S0, T0, C0, X0. , Y0
        /// </summary>
        /// <param name="address">汇川PLC的真实的位地址信息，对于H3U情况，可以输入M0,SM0,S0,T0,C0,X0,Y0类型地址，对于XP系列而言，可以输入M0,S0,T0,C0,X0,Y0</param>
        /// <param name="length">等待读取的长度，按照位为单位</param>
        /// <returns>包含是否成功的结果对象</returns>
        [EstMqttApi("ReadBoolArray", "")]
        public override OperateResult<bool[]> ReadBool(string address, ushort length)
        {
            OperateResult<string> operateResult = InovanceHelper.PraseInovanceH3UAddress(address, (byte)1);
            return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult) : base.ReadBool(operateResult.Content, length);
        }

        /// <summary>
        /// 按位写入汇川PLC的数据信息，对于H3U情况，可以输入M0,SM0,S0,T0,C0,X0,Y0类型地址，对于XP系列而言，可以输入M0,S0,T0,C0,X0,Y0<br />
        /// Write the data information of Huichuan PLC bit by bit. For H3U, you can enter M0, SM0, S0, T0, C0, X0, Y0 type addresses. For XP series, you can enter M0, S0, T0, C0, X0 , Y0
        /// </summary>
        /// <param name="address">汇川PLC的真实的位地址信息，对于H3U情况，可以输入M0,SM0,S0,T0,C0,X0,Y0类型地址，对于XP系列而言，可以输入M0,S0,T0,C0,X0,Y0</param>
        /// <param name="values">等待写入的原始数据</param>
        /// <returns>是否写入成功</returns>
        [EstMqttApi("WriteBoolArray", "")]
        public override OperateResult Write(string address, bool[] values)
        {
            OperateResult<string> operateResult = InovanceHelper.PraseInovanceH3UAddress(address, (byte)15);
            return !operateResult.IsSuccess ? (OperateResult)OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult) : base.Write(operateResult.Content, values);
        }

        /// <summary>
        /// 写入汇川PLC一个bool数据，对于H3U情况，可以输入M0,SM0,S0,T0,C0,X0,Y0类型地址，对于XP系列而言，可以输入M0,S0,T0,C0,X0,Y0<br />
        /// Write a bool data to Huichuan PLC. For H3U, you can enter M0, SM0, S0, T0, C0, X0, Y0 type address. For XP series, you can enter M0, S0, T0, C0, X0, Y0.
        /// </summary>
        /// <param name="address">汇川PLC的真实的位地址信息，对于H3U情况，可以输入M0,SM0,S0,T0,C0,X0,Y0类型地址，对于XP系列而言，可以输入M0,S0,T0,C0,X0,Y0</param>
        /// <param name="value">bool数据值</param>
        /// <returns>是否写入成功</returns>
        [EstMqttApi("WriteBool", "")]
        public override OperateResult Write(string address, bool value)
        {
            OperateResult<string> operateResult = InovanceHelper.PraseInovanceH3UAddress(address, (byte)5);
            return !operateResult.IsSuccess ? (OperateResult)OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult) : base.Write(operateResult.Content, value);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Inovance.InovanceH3UTcp.ReadBool(System.String,System.UInt16)" />
        public override async Task<OperateResult<bool[]>> ReadBoolAsync(
          string address,
          ushort length)
        {
            OperateResult<string> transModbus = InovanceHelper.PraseInovanceH3UAddress(address, (byte)1);
            if (!transModbus.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)transModbus);
            OperateResult<bool[]> operateResult = await base.ReadBoolAsync(transModbus.Content, length);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Inovance.InovanceH3UTcp.Write(System.String,System.Boolean[])" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          bool[] values)
        {
            OperateResult<string> transModbus = InovanceHelper.PraseInovanceH3UAddress(address, (byte)15);
            if (!transModbus.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<bool[]>((OperateResult)transModbus);
            OperateResult operateResult = await base.WriteAsync(transModbus.Content, values);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Inovance.InovanceH3UTcp.Write(System.String,System.Boolean)" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          bool value)
        {
            OperateResult<string> transModbus = InovanceHelper.PraseInovanceH3UAddress(address, (byte)5);
            if (!transModbus.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<bool[]>((OperateResult)transModbus);
            OperateResult operateResult = await base.WriteAsync(transModbus.Content, value);
            return operateResult;
        }

        /// <summary>
        /// 写入汇川PLC的一个字数据，对于H3U情况，可以输入D0,SD0,R0,T0,C0类型地址，对于XP系列而言，可以输入D0,T0,C0<br />
        /// Write one word data of Huichuan PLC. For H3U, you can enter D0, SD0, R0, T0, C0 type address. For XP series, you can enter D0, T0, C0
        /// </summary>
        /// <param name="address">汇川PLC的真实地址，对于H3U情况，可以输入D0,SD0,R0,T0,C0类型地址，对于XP系列而言，可以输入D0,T0,C0</param>
        /// <param name="value">short数据</param>
        /// <returns>是否写入成功</returns>
        [EstMqttApi("WriteInt16", "")]
        public override OperateResult Write(string address, short value)
        {
            OperateResult<string> operateResult = InovanceHelper.PraseInovanceH3UAddress(address, (byte)6);
            return !operateResult.IsSuccess ? (OperateResult)OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult) : base.Write(operateResult.Content, value);
        }

        /// <summary>
        /// 写入汇川PLC的一个字数据，对于H3U情况，可以输入D0,SD0,R0,T0,C0类型地址，对于XP系列而言，可以输入D0,T0,C0<br />
        /// Write one word data of Huichuan PLC. For H3U, you can enter D0, SD0, R0, T0, C0 type address. For XP series, you can enter D0, T0, C0
        /// </summary>
        /// <param name="address">汇川PLC的真实地址，对于H3U情况，可以输入D0,SD0,R0,T0,C0类型地址，对于XP系列而言，可以输入D0,T0,C0</param>
        /// <param name="value">ushort数据</param>
        /// <returns>是否写入成功</returns>
        [EstMqttApi("WriteUInt16", "")]
        public override OperateResult Write(string address, ushort value)
        {
            OperateResult<string> operateResult = InovanceHelper.PraseInovanceH3UAddress(address, (byte)6);
            return !operateResult.IsSuccess ? (OperateResult)OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult) : base.Write(operateResult.Content, value);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Inovance.InovanceH3UTcp.Write(System.String,System.Int16)" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          short value)
        {
            OperateResult<string> transModbus = InovanceHelper.PraseInovanceH3UAddress(address, (byte)6);
            if (!transModbus.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<bool[]>((OperateResult)transModbus);
            OperateResult operateResult = await base.WriteAsync(transModbus.Content, value);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Inovance.InovanceH3UTcp.Write(System.String,System.UInt16)" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          ushort value)
        {
            OperateResult<string> transModbus = InovanceHelper.PraseInovanceH3UAddress(address, (byte)6);
            if (!transModbus.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<bool[]>((OperateResult)transModbus);
            OperateResult operateResult = await base.WriteAsync(transModbus.Content, value);
            return operateResult;
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("InovanceH3UTcp[{0}:{1}]", (object)this.IpAddress, (object)this.Port);
    }
}
