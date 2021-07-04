// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.Delta.DeltaDvpSerial
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.Core;
using EstCommunication.ModBus;
using EstCommunication.Reflection;

namespace EstCommunication.Profinet.Delta
{
  /// <summary>
  /// 台达PLC的串口通讯类，基于Modbus-Rtu协议开发，按照台达的地址进行实现。<br />
  /// The serial communication class of Delta PLC is developed based on the Modbus-Rtu protocol and implemented according to Delta's address.
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
  /// 除此之外，地址可以携带站号信息，例如 s=2;D100，也是支持的。
  /// </example>
  public class DeltaDvpSerial : ModbusRtu
  {
    /// <summary>
    /// 实例化一个默认的对象<br />
    /// Instantiate a default object
    /// </summary>
    public DeltaDvpSerial() => this.ByteTransform.DataFormat = DataFormat.CDAB;

    /// <summary>
    /// 指定客户端自己的站号来初始化<br />
    /// Specify the client's own station number to initialize
    /// </summary>
    /// <param name="station">客户端自身的站号</param>
    public DeltaDvpSerial(byte station = 1)
      : base(station)
      => this.ByteTransform.DataFormat = DataFormat.CDAB;

    /// <summary>
    /// 从寄存器里读取原始的字节数据内容，地址主要是 D,T,C，每个类型的地址范围不一样，具体参考API文档<br />
    /// Read the original byte data content from the register, the address is mainly D, T, C,
    /// the address range of each type is different, please refer to the API document for details
    /// </summary>
    /// <param name="address">D,T,C 类型地址，举例：D100, C100</param>
    /// <param name="length">读取的地址长度，一个地址是2个字节</param>
    /// <returns>带有成功标志的字节数据</returns>
    [EstMqttApi("ReadByteArray", "Read the original byte data content from the register, the address is mainly D, T, C")]
    public override OperateResult<byte[]> Read(string address, ushort length)
    {
      OperateResult<string> operateResult = DeltaHelper.PraseDeltaDvpAddress(address, (byte) 3);
      return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult) : base.Read(operateResult.Content, length);
    }

    /// <summary>
    /// 向寄存器里写入原始的字节数据内容，地址主要是 D,T,C，每个类型的地址范围不一样，具体参考API文档<br />
    /// Write the original byte data content to the register, the address is mainly D, T, C,
    /// the address range of each type is different, please refer to the API document for details
    /// </summary>
    /// <param name="address">D,T,C 类型地址，举例：D100, C100</param>
    /// <param name="value">等待写入的原始字节值</param>
    /// <returns>返回写入结果</returns>
    [EstMqttApi("WriteByteArray", "Write the original byte data content to the register, the address is mainly D, T, C")]
    public override OperateResult Write(string address, byte[] value)
    {
      OperateResult<string> operateResult = DeltaHelper.PraseDeltaDvpAddress(address, (byte) 16);
      return !operateResult.IsSuccess ? (OperateResult) OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult) : base.Write(operateResult.Content, value);
    }

    /// <summary>
    /// 从线圈里批量读取bool数据内容，地址主要是 X,Y,S,M,T,C，其中X,Y的地址是8进制的，X0-X377，Y0-Y377<br />
    /// Read the contents of bool data in batches from the coil, the address is mainly X, Y, S, M, T, C, where X, Y address is in octal, X0-X377, Y0-Y377
    /// </summary>
    /// <param name="address">X,Y,S,M,T,C，其中X,Y的地址是8进制的，X0-X377，Y0-Y377</param>
    /// <param name="length">数据长度</param>
    /// <returns>带有成功标识的bool[]数组</returns>
    [EstMqttApi("ReadBoolArray", "Read the contents of bool data in batches from the coil, the address is mainly X, Y, S, M, T, C")]
    public override OperateResult<bool[]> ReadBool(string address, ushort length)
    {
      OperateResult<string> operateResult = DeltaHelper.PraseDeltaDvpAddress(address, (byte) 1);
      return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult) : base.ReadBool(operateResult.Content, length);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Delta.DeltaDvpSerial.Write(System.String,System.Boolean)" />
    [EstMqttApi("WriteBoolArray", "Read the contents of bool data in batches from the coil, the address is mainly X, Y, S, M, T, C")]
    public override OperateResult Write(string address, bool[] values)
    {
      OperateResult<string> operateResult = DeltaHelper.PraseDeltaDvpAddress(address, (byte) 15);
      return !operateResult.IsSuccess ? (OperateResult) OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult) : base.Write(operateResult.Content, values);
    }

    /// <summary>
    /// 向线圈里写入bool数据内容，地址主要是 Y,S,M,T,C，其中Y的地址是8进制的，Y0-Y377，注意 X 地址不能写入！<br />
    /// Write bool data content to the coil, the address is mainly Y, S, M, T, C, where the address of Y is in octal, Y0-Y377, note that X address cannot be written!
    /// </summary>
    /// <param name="address">X,Y,S,M,T,C，其中X,Y的地址是8进制的，X0-X1037，Y0-Y1037</param>
    /// <param name="value">等待写入的Bool值</param>
    /// <returns>返回写入结果</returns>
    [EstMqttApi("WriteBool", "Write bool data content to the coil, the address is mainly Y, S, M, T, C")]
    public override OperateResult Write(string address, bool value)
    {
      OperateResult<string> operateResult = DeltaHelper.PraseDeltaDvpAddress(address, (byte) 5);
      return !operateResult.IsSuccess ? (OperateResult) OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult) : base.Write(operateResult.Content, value);
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.Int16)" />
    [EstMqttApi("WriteInt16", "Write short data, returns whether success")]
    public override OperateResult Write(string address, short value)
    {
      OperateResult<string> operateResult = DeltaHelper.PraseDeltaDvpAddress(address, (byte) 6);
      return !operateResult.IsSuccess ? (OperateResult) OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult) : base.Write(operateResult.Content, value);
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.UInt16)" />
    [EstMqttApi("WriteUInt16", "Write ushort data, return whether the write was successful")]
    public override OperateResult Write(string address, ushort value)
    {
      OperateResult<string> operateResult = DeltaHelper.PraseDeltaDvpAddress(address, (byte) 6);
      return !operateResult.IsSuccess ? (OperateResult) OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult) : base.Write(operateResult.Content, value);
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("DeltaDvpSerial[{0}:{1}]", (object) this.PortName, (object) this.BaudRate);
  }
}
