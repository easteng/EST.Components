// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.Inovance.InovanceAMTcp
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.ModBus;
using EstCommunication.Reflection;
using System.Threading.Tasks;

namespace EstCommunication.Profinet.Inovance
{
  /// <summary>
  /// 汇川的网络通信协议，适用于AM400、 AM400_800、 AC800 等系列，底层走的是MODBUS-TCP协议，地址说明参见标记<br />
  /// Huichuan's network communication protocol is applicable to AM400, AM400_800, AC800 and other series. The bottom layer is MODBUS-TCP protocol. For the address description, please refer to mark
  /// </summary>
  /// <remarks>
  /// AM400_800 的元件有 Q 区，I 区，M 区这三种，分别都可以按位，按字节，按字和按双字进行访问，在本组件的条件下，仅支持按照位，字访问。<br />
  /// 对于AM400_800系列的地址表如下：
  /// <list type="table">
  ///   <listheader>
  ///     <term>地址名称</term>
  ///     <term>地址代号</term>
  ///     <term>地址范围</term>
  ///     <term>地址进制</term>
  ///     <term>备注</term>
  ///   </listheader>
  ///   <item>
  ///     <term>输出</term>
  ///     <term>Q</term>
  ///     <term>Q0.0-Q8191.7 或是 Q0-Q65535</term>
  ///     <term>8 或是 10</term>
  ///     <term>位读写</term>
  ///   </item>
  ///   <item>
  ///     <term>输入</term>
  ///     <term>I</term>
  ///     <term>IX0.0-IX8191.7 或是 I0-I65535</term>
  ///     <term>8 或是 10</term>
  ///     <term>位读写</term>
  ///   </item>
  ///   <item>
  ///     <term>M寄存器</term>
  ///     <term>M</term>
  ///     <term>MW0-MW65535</term>
  ///     <term>10</term>
  ///     <term>按照字访问的</term>
  ///   </item>
  /// </list>
  /// 针对AM600的TCP还支持下面的两种地址读写
  /// <list type="table">
  ///   <listheader>
  ///     <term>地址名称</term>
  ///     <term>地址代号</term>
  ///     <term>地址范围</term>
  ///     <term>地址进制</term>
  ///     <term>备注</term>
  ///   </listheader>
  ///   <item>
  ///     <term></term>
  ///     <term>SM</term>
  ///     <term>SM0.0-SM8191.7 或是 SM0-SM65535</term>
  ///     <term>10</term>
  ///     <term>位读写</term>
  ///   </item>
  ///   <item>
  ///     <term></term>
  ///     <term>SD</term>
  ///     <term>SDW0-SDW65535</term>
  ///     <term>10</term>
  ///     <term>字读写</term>
  ///   </item>
  /// </list>
  /// </remarks>
  public class InovanceAMTcp : ModbusTcpNet
  {
    /// <summary>
    /// 实例化一个安川AM400-AM800系列的网络通讯协议<br />
    /// Instantiate a network communication protocol of Yaskawa AM400-AM800 series
    /// </summary>
    public InovanceAMTcp()
    {
    }

    /// <summary>
    /// 指定服务器地址，端口号，客户端自己的站号来实例化一个安川AM400-AM800系列的网络通讯协议<br />
    /// Specify the server address, port number, and client's own station number to instantiate a Yaskawa AM400-AM800 series network communication protocol
    /// </summary>
    /// <param name="ipAddress">服务器的Ip地址</param>
    /// <param name="port">服务器的端口号</param>
    /// <param name="station">客户端自身的站号</param>
    public InovanceAMTcp(string ipAddress, int port = 502, byte station = 1)
      : base(ipAddress, port, station)
    {
    }

    /// <summary>
    /// 按字读取汇川PLC的数据信息，对于AM400_800的情况，可以输入MW0，MW100，对于AM600而言，还支持SDW0，SDW100地址数据<br />
    /// Read Huichuan PLC's data information by word. For the case of AM400_800, you can enter MW0 and MW100. For AM600, it also supports SDW0 and SDW100 address data.
    /// </summary>
    /// <param name="address">PLC的真实的地址信息，对于AM400_800的情况，可以输入MW0，MW100，对于AM600而言，还支持SDW0，SD2100地址数据</param>
    /// <param name="length">读取的数据的长度，按照字为单位</param>
    /// <returns>包含是否成功的结果对象信息</returns>
    [EstMqttApi("ReadByteArray", "")]
    public override OperateResult<byte[]> Read(string address, ushort length)
    {
      OperateResult<string> operateResult = InovanceHelper.PraseInovanceAMAddress(address, (byte) 3);
      return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult) : base.Read(operateResult.Content, length);
    }

    /// <summary>
    /// 按字写入汇川PLC的数据信息，对于AM400_800的情况，可以输入MW0，MW100，对于AM600而言，还支持SDW0，SDW100地址数据<br />
    /// Write the data information of Huichuan PLC by word. For the case of AM400_800, you can enter MW0 and MW100. For AM600, it also supports SDW0 and SDW100 address data.
    /// </summary>
    /// <param name="address">PLC的真实的地址信息，对于AM400_800的情况，可以输入MW0，MW100，对于AM600而言，还支持SDW0，SD2100地址数据</param>
    /// <param name="value">等待写入的原始数据，长度为2的倍数</param>
    /// <returns>是否写入成功的结果信息</returns>
    [EstMqttApi("WriteByteArray", "")]
    public override OperateResult Write(string address, byte[] value)
    {
      OperateResult<string> operateResult = InovanceHelper.PraseInovanceAMAddress(address, (byte) 16);
      return !operateResult.IsSuccess ? (OperateResult) OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult) : base.Write(operateResult.Content, value);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Inovance.InovanceAMTcp.Read(System.String,System.UInt16)" />
    public override async Task<OperateResult<byte[]>> ReadAsync(
      string address,
      ushort length)
    {
      OperateResult<string> transModbus = InovanceHelper.PraseInovanceAMAddress(address, (byte) 3);
      if (!transModbus.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) transModbus);
      OperateResult<byte[]> operateResult = await base.ReadAsync(transModbus.Content, length);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Inovance.InovanceAMTcp.Write(System.String,System.Byte[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      byte[] value)
    {
      OperateResult<string> transModbus = InovanceHelper.PraseInovanceAMAddress(address, (byte) 16);
      if (!transModbus.IsSuccess)
        return (OperateResult) OperateResult.CreateFailedResult<byte[]>((OperateResult) transModbus);
      OperateResult operateResult = await base.WriteAsync(transModbus.Content, value);
      return operateResult;
    }

    /// <summary>
    /// 按位读取汇川PLC的数据信息，对于AM400_800的情况，可以输入QX0.1，IX0.1，对于AM600而言，还支持SMX0.1地址数据<br />
    /// Read the data of Huichuan PLC bit by bit. For the case of AM400_800, you can enter QX0.1 and IX0.1. For AM600, it also supports SMX0.1 address data.
    /// </summary>
    /// <param name="address">汇川PLC的真实的位地址信息，对于AM400_800的情况，可以输入QX0.1，IX0.1，对于AM600而言，还支持SMX0.1地址数据</param>
    /// <param name="length">等待读取的长度，按照位为单位</param>
    /// <returns>包含是否成功的结果对象</returns>
    [EstMqttApi("ReadBoolArray", "")]
    public override OperateResult<bool[]> ReadBool(string address, ushort length)
    {
      OperateResult<string> operateResult = InovanceHelper.PraseInovanceAMAddress(address, (byte) 1);
      return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult) : base.ReadBool(operateResult.Content, length);
    }

    /// <summary>
    /// 按位写入汇川PLC的数据信息，对于AM400_800的情况，可以输入QX0.1，对于AM600而言，还支持SMX0.1地址数据<br />
    /// Write the data information of Huichuan PLC bit by bit. For AM400_800, you can enter QX0.1. For AM600, it also supports SMX0.1 address data
    /// </summary>
    /// <param name="address">汇川PLC的真实的位地址信息，对于AM400_800的情况，可以输入QX0.1，对于AM600而言，还支持SMX0.1地址数据</param>
    /// <param name="values">等待写入的原始数据</param>
    /// <returns>是否写入成功</returns>
    [EstMqttApi("WriteBoolArray", "")]
    public override OperateResult Write(string address, bool[] values)
    {
      OperateResult<string> operateResult = InovanceHelper.PraseInovanceAMAddress(address, (byte) 15);
      return !operateResult.IsSuccess ? (OperateResult) OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult) : base.Write(operateResult.Content, values);
    }

    /// <summary>
    /// 写入汇川PLC一个bool数据，对于AM400_800的情况，可以输入QX0.1，对于AM600而言，还支持SMX0.1地址数据<br />
    /// Write a bool data of Huichuan PLC. For the case of AM400_800, you can enter QX0.1. For AM600, it also supports SMX0.1 address data.
    /// </summary>
    /// <param name="address">汇川PLC的真实的位地址信息，对于AM400_800的情况，可以输入QX0.1，对于AM600而言，还支持SMX0.1地址数据</param>
    /// <param name="value">bool数据值</param>
    /// <returns>是否写入成功</returns>
    [EstMqttApi("WriteBool", "")]
    public override OperateResult Write(string address, bool value)
    {
      OperateResult<string> operateResult = InovanceHelper.PraseInovanceAMAddress(address, (byte) 5);
      return !operateResult.IsSuccess ? (OperateResult) OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult) : base.Write(operateResult.Content, value);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Inovance.InovanceAMTcp.ReadBool(System.String,System.UInt16)" />
    public override async Task<OperateResult<bool[]>> ReadBoolAsync(
      string address,
      ushort length)
    {
      OperateResult<string> transModbus = InovanceHelper.PraseInovanceAMAddress(address, (byte) 1);
      if (!transModbus.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) transModbus);
      OperateResult<bool[]> operateResult = await base.ReadBoolAsync(transModbus.Content, length);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Inovance.InovanceAMTcp.Write(System.String,System.Boolean[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      bool[] values)
    {
      OperateResult<string> transModbus = InovanceHelper.PraseInovanceAMAddress(address, (byte) 15);
      if (!transModbus.IsSuccess)
        return (OperateResult) OperateResult.CreateFailedResult<bool[]>((OperateResult) transModbus);
      OperateResult operateResult = await base.WriteAsync(transModbus.Content, values);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Inovance.InovanceAMTcp.Write(System.String,System.Boolean)" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      bool value)
    {
      OperateResult<string> transModbus = InovanceHelper.PraseInovanceAMAddress(address, (byte) 5);
      if (!transModbus.IsSuccess)
        return (OperateResult) OperateResult.CreateFailedResult<bool[]>((OperateResult) transModbus);
      OperateResult operateResult = await base.WriteAsync(transModbus.Content, value);
      return operateResult;
    }

    /// <summary>
    /// 写入汇川PLC的一个字数据，对于AM400_800的情况，可以输入MW0，MW100，对于AM600而言，还支持SDW0，SDW100地址数据<br />
    /// Write one word data of Huichuan PLC. For the case of AM400_800, you can enter MW0 and MW100. For AM600, it also supports SDW0 and SDW100 address data.
    /// </summary>
    /// <param name="address">汇川PLC的真实地址，对于AM400_800的情况，可以输入MW0，MW100，对于AM600而言，还支持SDW0，SD2100地址数据</param>
    /// <param name="value">short数据</param>
    /// <returns>是否写入成功</returns>
    [EstMqttApi("WriteInt16", "")]
    public override OperateResult Write(string address, short value)
    {
      OperateResult<string> operateResult = InovanceHelper.PraseInovanceAMAddress(address, (byte) 6);
      return !operateResult.IsSuccess ? (OperateResult) OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult) : base.Write(operateResult.Content, value);
    }

    /// <summary>
    /// 写入汇川PLC的一个字数据，对于AM400_800的情况，可以输入MW0，MW100，对于AM600而言，还支持SDW0，SDW100地址数据<br />
    /// Write one word data of Huichuan PLC. For the case of AM400_800, you can enter MW0 and MW100. For AM600, it also supports SDW0 and SDW100 address data.
    /// </summary>
    /// <param name="address">汇川PLC的真实地址，对于AM400_800的情况，可以输入MW0，MW100，对于AM600而言，还支持SDW0，SD2100地址数据</param>
    /// <param name="value">ushort数据</param>
    /// <returns>是否写入成功</returns>
    [EstMqttApi("WriteUInt16", "")]
    public override OperateResult Write(string address, ushort value)
    {
      OperateResult<string> operateResult = InovanceHelper.PraseInovanceAMAddress(address, (byte) 6);
      return !operateResult.IsSuccess ? (OperateResult) OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult) : base.Write(operateResult.Content, value);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Inovance.InovanceAMTcp.Write(System.String,System.Int16)" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      short value)
    {
      OperateResult<string> transModbus = InovanceHelper.PraseInovanceAMAddress(address, (byte) 6);
      if (!transModbus.IsSuccess)
        return (OperateResult) OperateResult.CreateFailedResult<bool[]>((OperateResult) transModbus);
      OperateResult operateResult = await base.WriteAsync(transModbus.Content, value);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Inovance.InovanceAMTcp.Write(System.String,System.UInt16)" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      ushort value)
    {
      OperateResult<string> transModbus = InovanceHelper.PraseInovanceAMAddress(address, (byte) 6);
      if (!transModbus.IsSuccess)
        return (OperateResult) OperateResult.CreateFailedResult<bool[]>((OperateResult) transModbus);
      OperateResult operateResult = await base.WriteAsync(transModbus.Content, value);
      return operateResult;
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("InovanceAMTcp[{0}:{1}]", (object) this.IpAddress, (object) this.Port);
  }
}
