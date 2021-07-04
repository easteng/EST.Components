// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.XINJE.XinJEXCSerial
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.ModBus;
using EstCommunication.Reflection;

namespace EstCommunication.Profinet.XINJE
{
  /// <summary>
  /// 信捷PLC的XC系列的串口通讯类，适用于XC1/XC2/XC3/XC5/XCM/XCC系列，线圈支持X,Y,S,M,T,C，寄存器支持D,F,E,T,C，各个地址的范围不一样，详细参考API文档<br />
  /// XC series serial communication of Xinje PLC is applicable to XC1/XC2/XC3/XC5/XCM/XCC series, coil supports X, Y, S, M, T, C,
  /// and registers support D, F, E, T, C, the range of each address is different, please refer to the API documentation for details
  /// </summary>
  /// <remarks>
  /// 本类适用于XC1/XC2/XC3/XC5/XCM/XCC系列，线圈支持X,Y,S,M,T,C，寄存器支持D,F,E,T,C
  /// 线圈、 位元件、位变量地址定义，但是具体某个型号支持的可能又有区别
  /// </remarks>
  /// <example>
  /// <list type="table">
  ///   <listheader>
  ///     <term>地址名称</term>
  ///     <term>地址代号</term>
  ///     <term>地址范围</term>
  ///     <term>地址进制</term>
  ///     <term>备注</term>
  ///   </listheader>
  ///   <item>
  ///     <term>内部继电器</term>
  ///     <term>M</term>
  ///     <term>M0-M7999，M8000-M8511</term>
  ///     <term>10</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>流程继电器</term>
  ///     <term>S</term>
  ///     <term>S0-S1023</term>
  ///     <term>10</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>定时器</term>
  ///     <term>T</term>
  ///     <term>T0-T618</term>
  ///     <term>10</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>计数器</term>
  ///     <term>C</term>
  ///     <term>C0-C634</term>
  ///     <term>10</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>输入</term>
  ///     <term>X</term>
  ///     <term>X0-X1037 或者X0.0-X103.7</term>
  ///     <term>8</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>输出</term>
  ///     <term>Y</term>
  ///     <term>Y0-Y1037 或者Y0.0-Y103.7</term>
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
  ///     <term>Flash寄存器</term>
  ///     <term>F</term>
  ///     <term>F0-F5000;F8000-F8511</term>
  ///     <term>10</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>扩展内部寄存器</term>
  ///     <term>E</term>
  ///     <term>E0-E36863</term>
  ///     <term>10</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>定时器</term>
  ///     <term>T</term>
  ///     <term>T0-T618</term>
  ///     <term>10</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>计数器</term>
  ///     <term>C</term>
  ///     <term>C0-C634</term>
  ///     <term>10</term>
  ///     <term></term>
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
  /// </example>
  public class XinJEXCSerial : ModbusRtu
  {
    /// <summary>实例化一个默认的对象</summary>
    public XinJEXCSerial()
    {
    }

    /// <summary>
    /// 指定客户端自己的站号来初始化<br />
    /// Specify the client's own station number to initialize
    /// </summary>
    /// <param name="station">客户端自身的站号</param>
    public XinJEXCSerial(byte station = 1)
      : base(station)
    {
    }

    /// <summary>
    /// 从寄存器里读取原始的字节数据内容，地址主要是 D,F,E,T,C，每个类型的地址范围不一样，具体参考API文档<br />
    /// Read the original byte data content from the register, the address is mainly D, F, E, T, C,
    /// the address range of each type is different, please refer to the API document for details
    /// </summary>
    /// <param name="address">D,F,E,T,C 类型地址，举例：D100, F100</param>
    /// <param name="length">读取的地址长度，一个地址是2个字节</param>
    /// <returns>带有成功标志的字节数据</returns>
    [EstMqttApi("ReadByteArray", "Read the original byte data content from the register, the address is mainly D, F, E, T, C")]
    public override OperateResult<byte[]> Read(string address, ushort length)
    {
      OperateResult<string> operateResult = XinJEHelper.PraseXinJEXCAddress(address, (byte) 3);
      return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult) : base.Read(operateResult.Content, length);
    }

    /// <summary>
    /// 向寄存器里写入原始的字节数据内容，地址主要是 D,F,E,T,C，每个类型的地址范围不一样，具体参考API文档<br />
    /// Write the original byte data content to the register, the address is mainly D, F, E, T, C,
    /// the address range of each type is different, please refer to the API document for details
    /// </summary>
    /// <param name="address">D,F,E,T,C 类型地址，举例：D100, F100</param>
    /// <param name="value">等待写入的原始字节值</param>
    /// <returns>返回写入结果</returns>
    [EstMqttApi("WriteByteArray", "Write the original byte data content to the register, the address is mainly D, F, E, T, C")]
    public override OperateResult Write(string address, byte[] value)
    {
      OperateResult<string> operateResult = XinJEHelper.PraseXinJEXCAddress(address, (byte) 16);
      return !operateResult.IsSuccess ? (OperateResult) OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult) : base.Write(operateResult.Content, value);
    }

    /// <summary>
    /// 从线圈里批量读取bool数据内容，地址主要是 X,Y,S,M,T,C，其中X,Y的地址是8进制的，X0-X1037，Y0-Y1037<br />
    /// Read the contents of bool data in batches from the coil, the address is mainly X, Y, S, M, T, C, where X, Y address is in octal, X0-X1037, Y0-Y1037
    /// </summary>
    /// <param name="address">X,Y,S,M,T,C，其中X,Y的地址是8进制的，X0-X1037，Y0-Y1037</param>
    /// <param name="length">数据长度</param>
    /// <returns>带有成功标识的bool[]数组</returns>
    [EstMqttApi("ReadBoolArray", "Read the contents of bool data in batches from the coil, the address is mainly X, Y, S, M, T, C")]
    public override OperateResult<bool[]> ReadBool(string address, ushort length)
    {
      OperateResult<string> operateResult = XinJEHelper.PraseXinJEXCAddress(address, (byte) 1);
      return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult) : base.ReadBool(operateResult.Content, length);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.XINJE.XinJEXCSerial.Write(System.String,System.Boolean)" />
    [EstMqttApi("WriteBoolArray", "Read the contents of bool data in batches from the coil, the address is mainly X, Y, S, M, T, C")]
    public override OperateResult Write(string address, bool[] values)
    {
      OperateResult<string> operateResult = XinJEHelper.PraseXinJEXCAddress(address, (byte) 15);
      return !operateResult.IsSuccess ? (OperateResult) OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult) : base.Write(operateResult.Content, values);
    }

    /// <summary>
    /// 向线圈里写入bool数据内容，地址主要是 X,Y,S,M,T,C，其中X,Y的地址是8进制的，X0-X1037，Y0-Y1037<br />
    /// Write bool data content to the coil, the address is mainly X, Y, S, M, T, C, where X, Y address is in octal, X0-X1037, Y0-Y1037
    /// </summary>
    /// <param name="address">X,Y,S,M,T,C，其中X,Y的地址是8进制的，X0-X1037，Y0-Y1037</param>
    /// <param name="value">等待写入的Bool值</param>
    /// <returns>返回写入结果</returns>
    [EstMqttApi("WriteBool", "Write bool data content to the coil, the address is mainly X, Y, S, M, T, C")]
    public override OperateResult Write(string address, bool value)
    {
      OperateResult<string> operateResult = XinJEHelper.PraseXinJEXCAddress(address, (byte) 5);
      return !operateResult.IsSuccess ? (OperateResult) OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult) : base.Write(operateResult.Content, value);
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.Int16)" />
    [EstMqttApi("WriteInt16", "Write short data, returns whether success")]
    public override OperateResult Write(string address, short value)
    {
      OperateResult<string> operateResult = XinJEHelper.PraseXinJEXCAddress(address, (byte) 6);
      return !operateResult.IsSuccess ? (OperateResult) OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult) : base.Write(operateResult.Content, value);
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.UInt16)" />
    [EstMqttApi("WriteUInt16", "Write ushort data, return whether the write was successful")]
    public override OperateResult Write(string address, ushort value)
    {
      OperateResult<string> operateResult = XinJEHelper.PraseXinJEXCAddress(address, (byte) 6);
      return !operateResult.IsSuccess ? (OperateResult) OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult) : base.Write(operateResult.Content, value);
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("XinJEXCSerial[{0}:{1}]", (object) this.PortName, (object) this.BaudRate);
  }
}
