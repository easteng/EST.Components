// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.Yokogawa.YokogawaLinkTcp
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using EstCommunication.Core;
using EstCommunication.Core.Address;
using EstCommunication.Core.IMessage;
using EstCommunication.Core.Net;
using EstCommunication.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EstCommunication.Profinet.Yokogawa
{
  /// <summary>
  /// 横河PLC的二进制通信类，支持X,Y,I,E,M,T,C,L继电器类型的数据读写，支持D,B,F,R,V,Z,W,TN,CN寄存器类型的数据读写，还支持一些高级的信息读写接口，详细参考API文档。<br />
  /// Yokogawa PLC's binary communication type, supports X, Y, I, E, M, T, C, L relay type data read and write,
  /// supports D, B, F, R, V, Z, W, TN, CN registers Types of data reading and writing, and some advanced information reading and writing interfaces are also supported.
  /// Please refer to the API documentation for details.
  /// </summary>
  /// <remarks>
  /// 基础的数据读写面向VIP用户开放，高级的读写随机数据，启动停止命令，读取程序状态，
  /// 系统信息，PLC时间，读写特殊的模块数据需要商业用户授权，读取的数据长度，读取的随机地址长度，在商业授权下，长度不受限制，可以无限大。
  /// </remarks>
  /// <example>
  /// 地址示例如下：
  /// <list type="table">
  ///   <listheader>
  ///     <term>地址名称</term>
  ///     <term>地址代号</term>
  ///     <term>示例</term>
  ///     <term>字操作</term>
  ///     <term>位操作</term>
  ///     <term>备注</term>
  ///   </listheader>
  ///   <item>
  ///     <term>Input relay</term>
  ///     <term>X</term>
  ///     <term>X100,X200</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term>只能读，不能写</term>
  ///   </item>
  ///   <item>
  ///     <term>Output relay</term>
  ///     <term>Y</term>
  ///     <term>Y100,Y200</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>Internal relay</term>
  ///     <term>I</term>
  ///     <term>I100,I200</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>Share relay</term>
  ///     <term>E</term>
  ///     <term>E100,E200</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>Special relay</term>
  ///     <term>M</term>
  ///     <term>M100,M200</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>Time relay</term>
  ///     <term>T</term>
  ///     <term>T100,T200</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>Counter relay</term>
  ///     <term>C</term>
  ///     <term>C100,C200</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>link relay</term>
  ///     <term>L</term>
  ///     <term>L100, L200</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>Data register</term>
  ///     <term>D</term>
  ///     <term>D100,D200</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>File register</term>
  ///     <term>B</term>
  ///     <term>B100,B200</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term>Only available for sequence CPU modules F3SP22, F3SP25, F3SP28, F3SP35, F3SP38, F3SP53, F3SP58, F3SP59, F3SP66, F3SP67, F3SP71 and F3SP76</term>
  ///   </item>
  ///   <item>
  ///     <term>Cache register</term>
  ///     <term>F</term>
  ///     <term>F100,F200</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term>Only available for sequence CPU modules F3SP71 and F3SP76</term>
  ///   </item>
  ///   <item>
  ///     <term>Shared register</term>
  ///     <term>R</term>
  ///     <term>R100,R200</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>Index register</term>
  ///     <term>V</term>
  ///     <term>V100,V200</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>Special register</term>
  ///     <term>Z</term>
  ///     <term>Z100,Z200</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>Link register</term>
  ///     <term>W</term>
  ///     <term>W100,W200</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>Timer current value</term>
  ///     <term>TN</term>
  ///     <term>TN100,TN200</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>Counter current value</term>
  ///     <term>CN</term>
  ///     <term>CN100,CN200</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term></term>
  ///   </item>
  /// </list>
  /// 例如我们正常读取一个D100的数据如下：
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\YokogawaLinkTcpSample.cs" region="Sample1" title="Read示例" />
  /// 我们在读取的时候可以动态的变更cpu信息，参考下面的代码
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\YokogawaLinkTcpSample.cs" region="Sample2" title="Read示例" />
  /// 关于随机读写的代码示例，可以读写地址分布很散的地址，参考下面的代码
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\YokogawaLinkTcpSample.cs" region="Sample3" title="Read示例" />
  /// 最后看一下读取特殊模块的数据，可以读取基本的字节数据，也可以使用富文本的地址读取
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\YokogawaLinkTcpSample.cs" region="Sample4" title="Read示例" />
  /// </example>
  public class YokogawaLinkTcp : NetworkDeviceBase
  {
    /// <summary>
    /// 实例化一个默认的对象<br />
    /// Instantiate a default object
    /// </summary>
    public YokogawaLinkTcp()
    {
      this.ByteTransform = (IByteTransform) new ReverseWordTransform();
      this.ByteTransform.DataFormat = DataFormat.CDAB;
      this.CpuNumber = (byte) 1;
    }

    /// <summary>
    /// 指定IP地址和端口号来实例化一个对象<br />
    /// Specify the IP address and port number to instantiate an object
    /// </summary>
    /// <param name="ipAddress">Ip地址</param>
    /// <param name="port">端口号</param>
    public YokogawaLinkTcp(string ipAddress, int port)
    {
      this.ByteTransform = (IByteTransform) new ReverseWordTransform();
      this.ByteTransform.DataFormat = DataFormat.CDAB;
      this.IpAddress = ipAddress;
      this.Port = port;
      this.CpuNumber = (byte) 1;
    }

    /// <inheritdoc />
    protected override INetMessage GetNewNetMessage() => (INetMessage) new YokogawaLinkBinaryMessage();

    /// <summary>
    /// 获取或设置当前的CPU Number，默认值为1<br />
    /// Get or set the current CPU Number, the default value is 1
    /// </summary>
    public byte CpuNumber { get; set; }

    /// <inheritdoc />
    /// <remarks>
    /// 读取的线圈地址支持X,Y,I,E,M,T,C,L，寄存器地址支持D,B,F,R,V,Z,W,TN,CN，举例：D100；也可以携带CPU进行访问，举例：cpu=2;D100<br />
    /// <b>[商业授权]</b> 如果想要读取特殊模块的数据，需要使用 <b>Special:</b> 开头标记，举例：Special:unit=0;slot=1;100<br />
    /// The read coil address supports X, Y, I, E, M, T, C, L, and the register address supports D, B, F, R, V, Z, W, TN, CN, for example: D100;
    /// it can also be carried CPU access, for example: cpu=2;D100. <br />
    /// <b>[Authorization]</b> If you want to read the data of a special module, you need to use the <b>Special:</b> beginning tag, for example: Special:unit=0;slot=1;100
    /// </remarks>
    [EstMqttApi("ReadByteArray", "Supports X,Y,I,E,M,T,C,L,D,B,F,R,V,Z,W,TN,CN, for example: D100; or cpu=2;D100 or Special:unit=0;slot=1;100")]
    public override OperateResult<byte[]> Read(string address, ushort length)
    {
      OperateResult<List<byte[]>> operateResult1;
      if (address.StartsWith("Special:") || address.StartsWith("special:"))
      {
        if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
          return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
        operateResult1 = YokogawaLinkTcp.BuildReadSpecialModule(this.CpuNumber, address, length);
      }
      else
        operateResult1 = YokogawaLinkTcp.BuildReadCommand(this.CpuNumber, address, length, false);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult1);
      List<byte> byteList = new List<byte>();
      for (int index = 0; index < operateResult1.Content.Count; ++index)
      {
        OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content[index]);
        if (!operateResult2.IsSuccess)
          return operateResult2;
        OperateResult<byte[]> operateResult3 = YokogawaLinkTcp.CheckContent(operateResult2.Content);
        byteList.AddRange((IEnumerable<byte>) operateResult3.Content);
      }
      return OperateResult.CreateSuccessResult<byte[]>(byteList.ToArray());
    }

    /// <inheritdoc />
    /// <remarks>
    /// 写入的线圈地址支持Y,I,E,M,T,C,L，寄存器地址支持D,B,F,R,V,Z,W,TN,CN，举例：D100；也可以携带CPU进行访问，举例：cpu=2;D100<br />
    /// 如果想要写入特殊模块的数据，需要使用 <b>Special:</b> 开头标记，举例：Special:unit=0;slot=1;100<br />
    /// The read coil address supports Y, I, E, M, T, C, L, and the register address supports D, B, F, R, V, Z, W, TN, CN, for example: D100;
    /// it can also be carried CPU access, for example: cpu=2;D100.
    /// If you want to read the data of a special module, you need to use the <b>Special:</b> beginning tag, for example: Special:unit=0;slot=1;100
    /// </remarks>
    [EstMqttApi("WriteByteArray", "Supports Y,I,E,M,T,C,L,D,B,F,R,V,Z,W,TN,CN, for example: D100; or cpu=2;D100 or Special:unit=0;slot=1;100")]
    public override OperateResult Write(string address, byte[] value)
    {
      OperateResult<byte[]> operateResult1;
      if (address.StartsWith("Special:") || address.StartsWith("special:"))
      {
        if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
          return new OperateResult(StringResources.Language.InsufficientPrivileges);
        operateResult1 = YokogawaLinkTcp.BuildWriteSpecialModule(this.CpuNumber, address, value);
      }
      else
        operateResult1 = YokogawaLinkTcp.BuildWriteWordCommand(this.CpuNumber, address, value);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      return !operateResult2.IsSuccess ? (OperateResult) operateResult2 : (OperateResult) YokogawaLinkTcp.CheckContent(operateResult2.Content);
    }

    /// <inheritdoc />
    /// <remarks>
    /// 读取的线圈地址支持X,Y,I,E,M,T,C,L，举例：Y100；也可以携带CPU进行访问，举例：cpu=2;Y100<br />
    /// The read coil address supports X, Y, I, E, M, T, C, L, for example: Y100; you can also carry the CPU for access, for example: cpu=2;Y100
    /// </remarks>
    [EstMqttApi("ReadBoolArray", "Read coil address supports X, Y, I, E, M, T, C, L, for example: Y100; or cpu=2;Y100")]
    public override OperateResult<bool[]> ReadBool(string address, ushort length)
    {
      OperateResult<List<byte[]>> operateResult1 = YokogawaLinkTcp.BuildReadCommand(this.CpuNumber, address, length, true);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult1);
      List<byte> byteList = new List<byte>();
      for (int index = 0; index < operateResult1.Content.Count; ++index)
      {
        OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content[index]);
        if (!operateResult2.IsSuccess)
          return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult2);
        OperateResult<byte[]> operateResult3 = YokogawaLinkTcp.CheckContent(operateResult2.Content);
        if (!operateResult3.IsSuccess)
          return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult3);
        byteList.AddRange((IEnumerable<byte>) operateResult3.Content);
      }
      return OperateResult.CreateSuccessResult<bool[]>(((IEnumerable<byte>) byteList.ToArray()).Select<byte, bool>((Func<byte, bool>) (m => m > (byte) 0)).ToArray<bool>());
    }

    /// <inheritdoc />
    /// <remarks>
    /// 写入的线圈地址支持Y,I,E,M,T,C,L，举例：Y100；也可以携带CPU进行访问，举例：cpu=2;Y100<br />
    /// The write coil address supports Y, I, E, M, T, C, L, for example: Y100; you can also carry the CPU for access, for example: cpu=2;Y100
    /// </remarks>
    [EstMqttApi("WriteBoolArray", "The write coil address supports Y, I, E, M, T, C, L, for example: Y100; or cpu=2;Y100")]
    public override OperateResult Write(string address, bool[] value)
    {
      OperateResult<byte[]> operateResult1 = YokogawaLinkTcp.BuildWriteBoolCommand(this.CpuNumber, address, value);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      return !operateResult2.IsSuccess ? (OperateResult) operateResult2 : (OperateResult) YokogawaLinkTcp.CheckContent(operateResult2.Content);
    }

    /// <summary>
    /// <b>[商业授权]</b> 随机读取<see cref="T:System.Boolean" />数组信息，主需要出传入<see cref="T:System.Boolean" />数组地址信息，就可以返回批量<see cref="T:System.Boolean" />值<br />
    /// <b>[Authorization]</b> Random read <see cref="T:System.Boolean" /> array information, the master needs to pass in the <see cref="T:System.Boolean" /> array address information, and then the batch can be returned to <see cref="T:System.Boolean" /> value
    /// </summary>
    /// <param name="address">批量地址信息</param>
    /// <remarks>
    /// 读取的线圈地址支持X,Y,I,E,M,T,C,L，举例：Y100；也可以携带CPU进行访问，举例：cpu=2;Y100<br />
    /// The read coil address supports X, Y, I, E, M, T, C, L, for example: Y100; you can also carry the CPU for access, for example: cpu=2;Y100
    /// </remarks>
    /// <returns>带有成功标志的Bool数组信息</returns>
    [EstMqttApi(Description = "Read random relay, supports X, Y, I, E, M, T, C, L, for example: Y100;")]
    public OperateResult<bool[]> ReadRandomBool(string[] address)
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return new OperateResult<bool[]>(StringResources.Language.InsufficientPrivileges);
      OperateResult<List<byte[]>> operateResult1 = YokogawaLinkTcp.BuildReadRandomCommand(this.CpuNumber, address, true);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult1);
      List<bool> boolList = new List<bool>();
      foreach (byte[] send in operateResult1.Content)
      {
        OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(send);
        if (!operateResult2.IsSuccess)
          return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult2);
        OperateResult<byte[]> operateResult3 = YokogawaLinkTcp.CheckContent(operateResult2.Content);
        if (!operateResult3.IsSuccess)
          return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult3);
        boolList.AddRange(((IEnumerable<byte>) operateResult3.Content).Select<byte, bool>((Func<byte, bool>) (m => m > (byte) 0)));
      }
      return OperateResult.CreateSuccessResult<bool[]>(boolList.ToArray());
    }

    /// <summary>
    /// <b>[商业授权]</b> 随机写入<see cref="T:System.Boolean" />数组信息，主需要出传入<see cref="T:System.Boolean" />数组地址信息，以及对应的<see cref="T:System.Boolean" />数组值<br />
    /// <b>[Authorization]</b> Randomly write the <see cref="T:System.Boolean" /> array information, the main need to pass in the <see cref="T:System.Boolean" /> array address information,
    /// and the corresponding <see cref="T:System.Boolean" /> array value
    /// </summary>
    /// <param name="address">批量地址信息</param>
    /// <param name="value">批量的数据值信息</param>
    /// <remarks>
    /// 写入的线圈地址支持Y,I,E,M,T,C,L，举例：Y100；也可以携带CPU进行访问，举例：cpu=2;Y100<br />
    /// The write coil address supports Y, I, E, M, T, C, L, for example: Y100; you can also carry the CPU for access, for example: cpu=2;Y100
    /// </remarks>
    /// <returns>是否写入成功</returns>
    [EstMqttApi(Description = "Write random relay, supports Y, I, E, M, T, C, L, for example: Y100;")]
    public OperateResult WriteRandomBool(string[] address, bool[] value)
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return (OperateResult) new OperateResult<bool[]>(StringResources.Language.InsufficientPrivileges);
      if (address.Length != value.Length)
        return new OperateResult(StringResources.Language.TwoParametersLengthIsNotSame);
      OperateResult<byte[]> operateResult1 = YokogawaLinkTcp.BuildWriteRandomBoolCommand(this.CpuNumber, address, value);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return (OperateResult) operateResult2;
      OperateResult<byte[]> operateResult3 = YokogawaLinkTcp.CheckContent(operateResult2.Content);
      return !operateResult3.IsSuccess ? (OperateResult) operateResult3 : OperateResult.CreateSuccessResult();
    }

    /// <summary>
    /// <b>[商业授权]</b> 随机读取<see cref="T:System.Byte" />数组信息，主需要出传入<see cref="T:System.Byte" />数组地址信息，就可以返回批量<see cref="T:System.Byte" />值<br />
    /// <b>[Authorization]</b> Random read <see cref="T:System.Byte" /> array information, the master needs to pass in the <see cref="T:System.Byte" /> array address information, and then the batch can be returned to <see cref="T:System.Byte" /> value
    /// </summary>
    /// <param name="address">批量地址信息</param>
    /// <remarks>
    /// </remarks>
    /// <returns>带有成功标志的Bool数组信息</returns>
    [EstMqttApi(Description = "Read random register, supports D,B,F,R,V,Z,W,TN,CN，example: D100")]
    public OperateResult<byte[]> ReadRandom(string[] address)
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
      OperateResult<List<byte[]>> operateResult1 = YokogawaLinkTcp.BuildReadRandomCommand(this.CpuNumber, address, false);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult1);
      List<byte> byteList = new List<byte>();
      foreach (byte[] send in operateResult1.Content)
      {
        OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(send);
        if (!operateResult2.IsSuccess)
          return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult2);
        OperateResult<byte[]> operateResult3 = YokogawaLinkTcp.CheckContent(operateResult2.Content);
        if (!operateResult3.IsSuccess)
          return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult3);
        byteList.AddRange((IEnumerable<byte>) operateResult3.Content);
      }
      return OperateResult.CreateSuccessResult<byte[]>(byteList.ToArray());
    }

    /// <summary>
    /// <b>[商业授权]</b> 随机读取<see cref="T:System.Int16" />数组信息，主需要出传入<see cref="T:System.Int16" />数组地址信息，就可以返回批量<see cref="T:System.Int16" />值<br />
    /// <b>[Authorization]</b> Random read <see cref="T:System.Int16" /> array information, the master needs to pass in the <see cref="T:System.Int16" /> array address information, and then the batch can be returned to <see cref="T:System.Int16" /> value
    /// </summary>
    /// <param name="address">批量地址信息</param>
    /// <returns>带有成功标志的Bool数组信息</returns>
    [EstMqttApi(Description = "Read random register, and get short array, supports D, B, F, R, V, Z, W, TN, CN，example: D100")]
    public OperateResult<short[]> ReadRandomInt16(string[] address) => this.ReadRandom(address).Then<short[]>((Func<byte[], OperateResult<short[]>>) (m => OperateResult.CreateSuccessResult<short[]>(this.ByteTransform.TransInt16(m, 0, address.Length))));

    /// <summary>
    /// <b>[商业授权]</b> 随机读取<see cref="T:System.UInt16" />数组信息，主需要出传入<see cref="T:System.UInt16" />数组地址信息，就可以返回批量<see cref="T:System.UInt16" />值<br />
    /// <b>[Authorization]</b> Random read <see cref="T:System.UInt16" /> array information, the master needs to pass in the <see cref="T:System.UInt16" /> array address information, and then the batch can be returned to <see cref="T:System.UInt16" /> value
    /// </summary>
    /// <param name="address">批量地址信息</param>
    /// <returns>带有成功标志的Bool数组信息</returns>
    [EstMqttApi(Description = "Read random register, and get ushort array, supports D, B, F, R, V, Z, W, TN, CN，example: D100")]
    public OperateResult<ushort[]> ReadRandomUInt16(string[] address) => this.ReadRandom(address).Then<ushort[]>((Func<byte[], OperateResult<ushort[]>>) (m => OperateResult.CreateSuccessResult<ushort[]>(this.ByteTransform.TransUInt16(m, 0, address.Length))));

    /// <summary>
    /// <b>[商业授权]</b> 随机写入<see cref="T:System.Byte" />数组信息，主需要出传入<see cref="T:System.Byte" />数组地址信息，以及对应的<see cref="T:System.Byte" />数组值<br />
    /// <b>[Authorization]</b> Randomly write the <see cref="T:System.Byte" /> array information, the main need to pass in the <see cref="T:System.Byte" /> array address information,
    /// and the corresponding <see cref="T:System.Byte" /> array value
    /// </summary>
    /// <param name="address">批量地址信息</param>
    /// <param name="value">批量的数据值信息</param>
    /// <returns>是否写入成功</returns>
    [EstMqttApi(ApiTopic = "WriteRandom", Description = "Randomly write the byte array information, the main need to pass in the byte array address information")]
    public OperateResult WriteRandom(string[] address, byte[] value)
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return (OperateResult) new OperateResult<bool[]>(StringResources.Language.InsufficientPrivileges);
      if (address.Length * 2 != value.Length)
        return new OperateResult(StringResources.Language.TwoParametersLengthIsNotSame);
      OperateResult<byte[]> operateResult1 = YokogawaLinkTcp.BuildWriteRandomWordCommand(this.CpuNumber, address, value);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return (OperateResult) operateResult2;
      OperateResult<byte[]> operateResult3 = YokogawaLinkTcp.CheckContent(operateResult2.Content);
      return !operateResult3.IsSuccess ? (OperateResult) operateResult3 : OperateResult.CreateSuccessResult();
    }

    /// <summary>
    /// <b>[商业授权]</b> 随机写入<see cref="T:System.Int16" />数组信息，主需要出传入<see cref="T:System.Int16" />数组地址信息，以及对应的<see cref="T:System.Int16" />数组值<br />
    /// <b>[Authorization]</b> Randomly write the <see cref="T:System.Int16" /> array information, the main need to pass in the <see cref="T:System.Int16" /> array address information,
    /// and the corresponding <see cref="T:System.Int16" /> array value
    /// </summary>
    /// <param name="address">批量地址信息</param>
    /// <param name="value">批量的数据值信息</param>
    /// <returns>是否写入成功</returns>
    [EstMqttApi(ApiTopic = "WriteRandomInt16", Description = "Randomly write the short array information, the main need to pass in the short array address information")]
    public OperateResult WriteRandom(string[] address, short[] value) => this.WriteRandom(address, this.ByteTransform.TransByte(value));

    /// <summary>
    /// <b>[商业授权]</b> 随机写入<see cref="T:System.UInt16" />数组信息，主需要出传入<see cref="T:System.UInt16" />数组地址信息，以及对应的<see cref="T:System.UInt16" />数组值<br />
    /// <b>[Authorization]</b> Randomly write the <see cref="T:System.UInt16" /> array information, the main need to pass in the <see cref="T:System.UInt16" /> array address information,
    /// and the corresponding <see cref="T:System.UInt16" /> array value
    /// </summary>
    /// <param name="address">批量地址信息</param>
    /// <param name="value">批量的数据值信息</param>
    /// <returns>是否写入成功</returns>
    [EstMqttApi(ApiTopic = "WriteRandomUInt16", Description = "Randomly write the ushort array information, the main need to pass in the ushort array address information")]
    public OperateResult WriteRandom(string[] address, ushort[] value) => this.WriteRandom(address, this.ByteTransform.TransByte(value));

    /// <inheritdoc />
    public override async Task<OperateResult<byte[]>> ReadAsync(
      string address,
      ushort length)
    {
      OperateResult<List<byte[]>> command;
      if (address.StartsWith("Special:") || address.StartsWith("special:"))
      {
        if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
          return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
        command = YokogawaLinkTcp.BuildReadSpecialModule(this.CpuNumber, address, length);
      }
      else
        command = YokogawaLinkTcp.BuildReadCommand(this.CpuNumber, address, length, false);
      if (!command.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) command);
      List<byte> content = new List<byte>();
      for (int i = 0; i < command.Content.Count; ++i)
      {
        OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content[i]);
        if (!read.IsSuccess)
          return read;
        OperateResult<byte[]> check = YokogawaLinkTcp.CheckContent(read.Content);
        content.AddRange((IEnumerable<byte>) check.Content);
        read = (OperateResult<byte[]>) null;
        check = (OperateResult<byte[]>) null;
      }
      return OperateResult.CreateSuccessResult<byte[]>(content.ToArray());
    }

    /// <inheritdoc />
    public override async Task<OperateResult> WriteAsync(
      string address,
      byte[] value)
    {
      OperateResult<byte[]> command;
      if (address.StartsWith("Special:") || address.StartsWith("special:"))
      {
        if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
          return new OperateResult(StringResources.Language.InsufficientPrivileges);
        command = YokogawaLinkTcp.BuildWriteSpecialModule(this.CpuNumber, address, value);
      }
      else
        command = YokogawaLinkTcp.BuildWriteWordCommand(this.CpuNumber, address, value);
      if (!command.IsSuccess)
        return (OperateResult) command;
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      return read.IsSuccess ? (OperateResult) YokogawaLinkTcp.CheckContent(read.Content) : (OperateResult) read;
    }

    /// <inheritdoc />
    public override async Task<OperateResult<bool[]>> ReadBoolAsync(
      string address,
      ushort length)
    {
      OperateResult<List<byte[]>> command = YokogawaLinkTcp.BuildReadCommand(this.CpuNumber, address, length, true);
      if (!command.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) command);
      List<byte> content = new List<byte>();
      for (int i = 0; i < command.Content.Count; ++i)
      {
        OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content[i]);
        if (!read.IsSuccess)
          return OperateResult.CreateFailedResult<bool[]>((OperateResult) read);
        OperateResult<byte[]> check = YokogawaLinkTcp.CheckContent(read.Content);
        if (!check.IsSuccess)
          return OperateResult.CreateFailedResult<bool[]>((OperateResult) check);
        content.AddRange((IEnumerable<byte>) check.Content);
        read = (OperateResult<byte[]>) null;
        check = (OperateResult<byte[]>) null;
      }
      return OperateResult.CreateSuccessResult<bool[]>(((IEnumerable<byte>) content.ToArray()).Select<byte, bool>((Func<byte, bool>) (m => m > (byte) 0)).ToArray<bool>());
    }

    /// <inheritdoc />
    public override async Task<OperateResult> WriteAsync(
      string address,
      bool[] value)
    {
      OperateResult<byte[]> command = YokogawaLinkTcp.BuildWriteBoolCommand(this.CpuNumber, address, value);
      if (!command.IsSuccess)
        return (OperateResult) command;
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      return read.IsSuccess ? (OperateResult) YokogawaLinkTcp.CheckContent(read.Content) : (OperateResult) read;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Yokogawa.YokogawaLinkTcp.ReadRandomBool(System.String[])" />
    public async Task<OperateResult<bool[]>> ReadRandomBoolAsync(string[] address)
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return new OperateResult<bool[]>(StringResources.Language.InsufficientPrivileges);
      OperateResult<List<byte[]>> command = YokogawaLinkTcp.BuildReadRandomCommand(this.CpuNumber, address, true);
      if (!command.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) command);
      List<bool> lists = new List<bool>();
      foreach (byte[] numArray in command.Content)
      {
        byte[] content = numArray;
        OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(content);
        if (!read.IsSuccess)
          return OperateResult.CreateFailedResult<bool[]>((OperateResult) read);
        OperateResult<byte[]> check = YokogawaLinkTcp.CheckContent(read.Content);
        if (!check.IsSuccess)
          return OperateResult.CreateFailedResult<bool[]>((OperateResult) check);
        lists.AddRange(((IEnumerable<byte>) check.Content).Select<byte, bool>((Func<byte, bool>) (m => m > (byte) 0)));
        read = (OperateResult<byte[]>) null;
        check = (OperateResult<byte[]>) null;
        content = (byte[]) null;
      }
      return OperateResult.CreateSuccessResult<bool[]>(lists.ToArray());
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Yokogawa.YokogawaLinkTcp.WriteRandomBool(System.String[],System.Boolean[])" />
    public async Task<OperateResult> WriteRandomBoolAsync(
      string[] address,
      bool[] value)
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return (OperateResult) new OperateResult<bool[]>(StringResources.Language.InsufficientPrivileges);
      if (address.Length != value.Length)
        return new OperateResult(StringResources.Language.TwoParametersLengthIsNotSame);
      OperateResult<byte[]> command = YokogawaLinkTcp.BuildWriteRandomBoolCommand(this.CpuNumber, address, value);
      if (!command.IsSuccess)
        return (OperateResult) command;
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      if (!read.IsSuccess)
        return (OperateResult) read;
      OperateResult<byte[]> check = YokogawaLinkTcp.CheckContent(read.Content);
      return check.IsSuccess ? OperateResult.CreateSuccessResult() : (OperateResult) check;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Yokogawa.YokogawaLinkTcp.ReadRandom(System.String[])" />
    public async Task<OperateResult<byte[]>> ReadRandomAsync(string[] address)
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
      OperateResult<List<byte[]>> command = YokogawaLinkTcp.BuildReadRandomCommand(this.CpuNumber, address, false);
      if (!command.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) command);
      List<byte> lists = new List<byte>();
      foreach (byte[] numArray in command.Content)
      {
        byte[] content = numArray;
        OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(content);
        if (!read.IsSuccess)
          return OperateResult.CreateFailedResult<byte[]>((OperateResult) read);
        OperateResult<byte[]> check = YokogawaLinkTcp.CheckContent(read.Content);
        if (!check.IsSuccess)
          return OperateResult.CreateFailedResult<byte[]>((OperateResult) check);
        lists.AddRange((IEnumerable<byte>) check.Content);
        read = (OperateResult<byte[]>) null;
        check = (OperateResult<byte[]>) null;
        content = (byte[]) null;
      }
      return OperateResult.CreateSuccessResult<byte[]>(lists.ToArray());
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Yokogawa.YokogawaLinkTcp.ReadRandomInt16(System.String[])" />
    public async Task<OperateResult<short[]>> ReadRandomInt16Async(string[] address)
    {
      OperateResult<byte[]> operateResult = await this.ReadRandomAsync(address);
      return operateResult.Then<short[]>((Func<byte[], OperateResult<short[]>>) (m => OperateResult.CreateSuccessResult<short[]>(this.ByteTransform.TransInt16(m, 0, address.Length))));
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Yokogawa.YokogawaLinkTcp.ReadRandomUInt16(System.String[])" />
    public async Task<OperateResult<ushort[]>> ReadRandomUInt16Async(
      string[] address)
    {
      OperateResult<byte[]> operateResult = await this.ReadRandomAsync(address);
      return operateResult.Then<ushort[]>((Func<byte[], OperateResult<ushort[]>>) (m => OperateResult.CreateSuccessResult<ushort[]>(this.ByteTransform.TransUInt16(m, 0, address.Length))));
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Yokogawa.YokogawaLinkTcp.WriteRandom(System.String[],System.Byte[])" />
    public async Task<OperateResult> WriteRandomAsync(
      string[] address,
      byte[] value)
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return (OperateResult) new OperateResult<bool[]>(StringResources.Language.InsufficientPrivileges);
      if (address.Length * 2 != value.Length)
        return new OperateResult(StringResources.Language.TwoParametersLengthIsNotSame);
      OperateResult<byte[]> command = YokogawaLinkTcp.BuildWriteRandomWordCommand(this.CpuNumber, address, value);
      if (!command.IsSuccess)
        return (OperateResult) command;
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      if (!read.IsSuccess)
        return (OperateResult) read;
      OperateResult<byte[]> check = YokogawaLinkTcp.CheckContent(read.Content);
      return check.IsSuccess ? OperateResult.CreateSuccessResult() : (OperateResult) check;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Yokogawa.YokogawaLinkTcp.WriteRandom(System.String[],System.Int16[])" />
    public async Task<OperateResult> WriteRandomAsync(
      string[] address,
      short[] value)
    {
      OperateResult operateResult = await this.WriteRandomAsync(address, this.ByteTransform.TransByte(value));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Yokogawa.YokogawaLinkTcp.WriteRandom(System.String[],System.UInt16[])" />
    public async Task<OperateResult> WriteRandomAsync(
      string[] address,
      ushort[] value)
    {
      OperateResult operateResult = await this.WriteRandomAsync(address, this.ByteTransform.TransByte(value));
      return operateResult;
    }

    /// <summary>
    /// <b>[商业授权]</b> 如果未执行程序，则开始执行程序<br />
    /// <b>[Authorization]</b> Starts executing a program if it is not being executed
    /// </summary>
    /// <remarks>
    /// This command will be ignored if it is executed while a program is being executed.<br />
    /// Refer to the users manual for the individual modules for the response formats that are used at error times.
    /// </remarks>
    /// <returns>是否启动成功</returns>
    [EstMqttApi(Description = "Starts executing a program if it is not being executed")]
    public OperateResult Start()
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return new OperateResult(StringResources.Language.InsufficientPrivileges);
      OperateResult<byte[]> operateResult1 = YokogawaLinkTcp.BuildStartCommand(this.CpuNumber);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return (OperateResult) operateResult2;
      OperateResult<byte[]> operateResult3 = YokogawaLinkTcp.CheckContent(operateResult2.Content);
      return !operateResult3.IsSuccess ? (OperateResult) operateResult3 : OperateResult.CreateSuccessResult();
    }

    /// <summary>
    /// <b>[商业授权]</b> 停止当前正在执行程序<br />
    /// <b>[Authorization]</b> Stops the executing program.
    /// </summary>
    /// <remarks>
    /// This command will be ignored if it is executed when no program is being executed.<br />
    /// Refer to the users manual for the individual modules for the response formats that are used at error times.
    /// </remarks>
    /// <returns>是否启动成功</returns>
    [EstMqttApi(Description = "Stops the executing program.")]
    public OperateResult Stop()
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return new OperateResult(StringResources.Language.InsufficientPrivileges);
      OperateResult<byte[]> operateResult1 = YokogawaLinkTcp.BuildStopCommand(this.CpuNumber);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return (OperateResult) operateResult2;
      OperateResult<byte[]> operateResult3 = YokogawaLinkTcp.CheckContent(operateResult2.Content);
      return !operateResult3.IsSuccess ? (OperateResult) operateResult3 : OperateResult.CreateSuccessResult();
    }

    /// <summary>
    /// <b>[商业授权]</b> 重置当前的模块，当前打开的连接被强制关闭。 模块中所做的设置也将被清除。然后当前对象需要重连PLC。<br />
    /// <b>[Authorization]</b> When this command is executed via an Ethernet interface module or an Ethernet connection of an F3SP66, F3SP67,
    /// F3SP71 or F3SP76 sequence CPU module, the connection which is currently open is forced to close.
    /// The settings made in the modules are also cleared. Then the current object needs to reconnect to the PLC.
    /// </summary>
    /// <returns>是否重置成功</returns>
    [EstMqttApi(Description = "Reset the connection which is currently open is forced to close")]
    public OperateResult ModuleReset()
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return new OperateResult(StringResources.Language.InsufficientPrivileges);
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(new byte[4]
      {
        (byte) 97,
        this.CpuNumber,
        (byte) 0,
        (byte) 0
      }, false);
      return !operateResult.IsSuccess ? (OperateResult) operateResult : OperateResult.CreateSuccessResult();
    }

    /// <summary>
    /// <b>[商业授权]</b> 读取当前PLC的程序状态，返回1：RUN；2：Stop；3：Debug；255：ROM writer<br />
    /// <b>[Authorization]</b> Read the program status. return code 1:RUN; 2:Stop; 3:Debug; 255:ROM writer
    /// </summary>
    /// <returns>当前PLC的程序状态，返回1：RUN；2：Stop；3：Debug；255：ROM writer</returns>
    [EstMqttApi(Description = "Read the program status. return code 1:RUN; 2:Stop; 3:Debug; 255:ROM writer")]
    public OperateResult<int> ReadProgramStatus()
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return new OperateResult<int>(StringResources.Language.InsufficientPrivileges);
      OperateResult<byte[]> operateResult1 = this.ReadFromCoreServer(new byte[6]
      {
        (byte) 98,
        this.CpuNumber,
        (byte) 0,
        (byte) 2,
        (byte) 0,
        (byte) 1
      });
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<int>((OperateResult) operateResult1);
      OperateResult<byte[]> operateResult2 = YokogawaLinkTcp.CheckContent(operateResult1.Content);
      return !operateResult2.IsSuccess ? OperateResult.CreateFailedResult<int>((OperateResult) operateResult2) : OperateResult.CreateSuccessResult<int>((int) operateResult2.Content[1]);
    }

    /// <summary>
    /// <b>[商业授权]</b> 读取当前PLC的系统状态，系统的ID，CPU类型，程序大小信息<br />
    /// <b>[Authorization]</b> Read current PLC system status, system ID, CPU type, program size information
    /// </summary>
    /// <returns>系统信息的结果对象</returns>
    [EstMqttApi(Description = "Read current PLC system status, system ID, CPU type, program size information")]
    public OperateResult<YokogawaSystemInfo> ReadSystemInfo()
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return new OperateResult<YokogawaSystemInfo>(StringResources.Language.InsufficientPrivileges);
      OperateResult<byte[]> operateResult1 = this.ReadFromCoreServer(new byte[6]
      {
        (byte) 98,
        this.CpuNumber,
        (byte) 0,
        (byte) 2,
        (byte) 0,
        (byte) 2
      });
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<YokogawaSystemInfo>((OperateResult) operateResult1);
      OperateResult<byte[]> operateResult2 = YokogawaLinkTcp.CheckContent(operateResult1.Content);
      return !operateResult2.IsSuccess ? OperateResult.CreateFailedResult<YokogawaSystemInfo>((OperateResult) operateResult2) : YokogawaSystemInfo.Prase(operateResult2.Content);
    }

    /// <summary>
    /// <b>[商业授权]</b> 读取当前PLC的时间信息，包含年月日时分秒<br />
    /// <b>[Authorization]</b> Read current PLC time information, including year, month, day, hour, minute, and second
    /// </summary>
    /// <returns>PLC的当前的时间信息</returns>
    [EstMqttApi(Description = "Read current PLC time information, including year, month, day, hour, minute, and second")]
    public OperateResult<DateTime> ReadDateTime()
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return new OperateResult<DateTime>(StringResources.Language.InsufficientPrivileges);
      OperateResult<byte[]> operateResult1 = this.ReadFromCoreServer(new byte[4]
      {
        (byte) 99,
        this.CpuNumber,
        (byte) 0,
        (byte) 0
      });
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<DateTime>((OperateResult) operateResult1);
      OperateResult<byte[]> operateResult2 = YokogawaLinkTcp.CheckContent(operateResult1.Content);
      return !operateResult2.IsSuccess ? OperateResult.CreateFailedResult<DateTime>((OperateResult) operateResult2) : OperateResult.CreateSuccessResult<DateTime>(new DateTime(2000 + (int) this.ByteTransform.TransUInt16(operateResult2.Content, 0), (int) this.ByteTransform.TransUInt16(operateResult2.Content, 2), (int) this.ByteTransform.TransUInt16(operateResult2.Content, 4), (int) this.ByteTransform.TransUInt16(operateResult2.Content, 6), (int) this.ByteTransform.TransUInt16(operateResult2.Content, 8), (int) this.ByteTransform.TransUInt16(operateResult2.Content, 10)));
    }

    /// <summary>
    /// <b>[商业授权]</b> 读取特殊模块的数据信息，需要指定模块单元号，模块站号，数据地址，长度信息。<br />
    /// <b>[Authorization]</b> To read the data information of a special module, you need to specify the module unit number, module slot number, data address, and length information.
    /// </summary>
    /// <param name="moduleUnit">模块的单元号</param>
    /// <param name="moduleSlot">模块的站号</param>
    /// <param name="dataPosition">模块的数据地址</param>
    /// <param name="length">长度信息</param>
    /// <returns>带有成功标识的byte[]，可以自行解析出所需要的各种类型的数据</returns>
    [EstMqttApi(Description = "Read the data information of a special module, you need to specify the module unit number, module slot number, data address, and length information.")]
    public OperateResult<byte[]> ReadSpecialModule(
      byte moduleUnit,
      byte moduleSlot,
      ushort dataPosition,
      ushort length)
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
      List<byte> byteList = new List<byte>();
      List<byte[]> numArrayList = YokogawaLinkTcp.BuildReadSpecialModule(this.CpuNumber, moduleUnit, moduleSlot, dataPosition, length);
      for (int index = 0; index < numArrayList.Count; ++index)
      {
        OperateResult<byte[]> operateResult1 = this.ReadFromCoreServer(numArrayList[index]);
        if (!operateResult1.IsSuccess)
          return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult1);
        OperateResult<byte[]> operateResult2 = YokogawaLinkTcp.CheckContent(operateResult1.Content);
        if (!operateResult2.IsSuccess)
          return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult2);
        byteList.AddRange((IEnumerable<byte>) operateResult2.Content);
      }
      return OperateResult.CreateSuccessResult<byte[]>(byteList.ToArray());
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Yokogawa.YokogawaLinkTcp.Start" />
    public async Task<OperateResult> StartAsync()
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return new OperateResult(StringResources.Language.InsufficientPrivileges);
      OperateResult<byte[]> command = YokogawaLinkTcp.BuildStartCommand(this.CpuNumber);
      if (!command.IsSuccess)
        return (OperateResult) command;
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      if (!read.IsSuccess)
        return (OperateResult) read;
      OperateResult<byte[]> check = YokogawaLinkTcp.CheckContent(read.Content);
      return check.IsSuccess ? OperateResult.CreateSuccessResult() : (OperateResult) check;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Yokogawa.YokogawaLinkTcp.Stop" />
    public async Task<OperateResult> StopAsync()
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return new OperateResult(StringResources.Language.InsufficientPrivileges);
      OperateResult<byte[]> command = YokogawaLinkTcp.BuildStopCommand(this.CpuNumber);
      if (!command.IsSuccess)
        return (OperateResult) command;
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      if (!read.IsSuccess)
        return (OperateResult) read;
      OperateResult<byte[]> check = YokogawaLinkTcp.CheckContent(read.Content);
      return check.IsSuccess ? OperateResult.CreateSuccessResult() : (OperateResult) check;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Yokogawa.YokogawaLinkTcp.ModuleReset" />
    public async Task<OperateResult> ModuleResetAsync()
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return new OperateResult(StringResources.Language.InsufficientPrivileges);
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(new byte[4]
      {
        (byte) 97,
        this.CpuNumber,
        (byte) 0,
        (byte) 0
      }, false);
      return read.IsSuccess ? OperateResult.CreateSuccessResult() : (OperateResult) read;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Yokogawa.YokogawaLinkTcp.ReadProgramStatus" />
    public async Task<OperateResult<int>> ReadProgramStatusAsync()
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return new OperateResult<int>(StringResources.Language.InsufficientPrivileges);
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(new byte[6]
      {
        (byte) 98,
        this.CpuNumber,
        (byte) 0,
        (byte) 2,
        (byte) 0,
        (byte) 1
      });
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<int>((OperateResult) read);
      OperateResult<byte[]> check = YokogawaLinkTcp.CheckContent(read.Content);
      return check.IsSuccess ? OperateResult.CreateSuccessResult<int>((int) check.Content[1]) : OperateResult.CreateFailedResult<int>((OperateResult) check);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Yokogawa.YokogawaLinkTcp.ReadSystemInfo" />
    public async Task<OperateResult<YokogawaSystemInfo>> ReadSystemInfoAsync()
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return new OperateResult<YokogawaSystemInfo>(StringResources.Language.InsufficientPrivileges);
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(new byte[6]
      {
        (byte) 98,
        this.CpuNumber,
        (byte) 0,
        (byte) 2,
        (byte) 0,
        (byte) 2
      });
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<YokogawaSystemInfo>((OperateResult) read);
      OperateResult<byte[]> check = YokogawaLinkTcp.CheckContent(read.Content);
      return check.IsSuccess ? YokogawaSystemInfo.Prase(check.Content) : OperateResult.CreateFailedResult<YokogawaSystemInfo>((OperateResult) check);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Yokogawa.YokogawaLinkTcp.ReadDateTime" />
    public async Task<OperateResult<DateTime>> ReadDateTimeAsync()
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return new OperateResult<DateTime>(StringResources.Language.InsufficientPrivileges);
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(new byte[4]
      {
        (byte) 99,
        this.CpuNumber,
        (byte) 0,
        (byte) 0
      });
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<DateTime>((OperateResult) read);
      OperateResult<byte[]> check = YokogawaLinkTcp.CheckContent(read.Content);
      return check.IsSuccess ? OperateResult.CreateSuccessResult<DateTime>(new DateTime(2000 + (int) this.ByteTransform.TransUInt16(check.Content, 0), (int) this.ByteTransform.TransUInt16(check.Content, 2), (int) this.ByteTransform.TransUInt16(check.Content, 4), (int) this.ByteTransform.TransUInt16(check.Content, 6), (int) this.ByteTransform.TransUInt16(check.Content, 8), (int) this.ByteTransform.TransUInt16(check.Content, 10))) : OperateResult.CreateFailedResult<DateTime>((OperateResult) check);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Yokogawa.YokogawaLinkTcp.ReadSpecialModule(System.Byte,System.Byte,System.UInt16,System.UInt16)" />
    public async Task<OperateResult<byte[]>> ReadSpecialModuleAsync(
      byte moduleUnit,
      byte moduleSlot,
      ushort dataPosition,
      ushort length)
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
      List<byte> content = new List<byte>();
      List<byte[]> commands = YokogawaLinkTcp.BuildReadSpecialModule(this.CpuNumber, moduleUnit, moduleSlot, dataPosition, length);
      for (int i = 0; i < commands.Count; ++i)
      {
        OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(commands[i]);
        if (!read.IsSuccess)
          return OperateResult.CreateFailedResult<byte[]>((OperateResult) read);
        OperateResult<byte[]> check = YokogawaLinkTcp.CheckContent(read.Content);
        if (!check.IsSuccess)
          return OperateResult.CreateFailedResult<byte[]>((OperateResult) check);
        content.AddRange((IEnumerable<byte>) check.Content);
        read = (OperateResult<byte[]>) null;
        check = (OperateResult<byte[]>) null;
      }
      return OperateResult.CreateSuccessResult<byte[]>(content.ToArray());
    }

    /// <summary>
    /// 检查当前的反馈内容，如果没有发生错误，就解析出实际的数据内容。<br />
    /// Check the current feedback content, if there is no error, parse out the actual data content.
    /// </summary>
    /// <param name="content">原始的数据内容</param>
    /// <returns>解析之后的数据内容</returns>
    public static OperateResult<byte[]> CheckContent(byte[] content)
    {
      if (content[1] > (byte) 0)
        return new OperateResult<byte[]>(YokogawaLinkHelper.GetErrorMsg(content[1]));
      return content.Length > 4 ? OperateResult.CreateSuccessResult<byte[]>(content.RemoveBegin<byte>(4)) : OperateResult.CreateSuccessResult<byte[]>(new byte[0]);
    }

    /// <summary>构建读取命令的原始报文信息</summary>
    /// <param name="cpu">Cpu Number</param>
    /// <param name="address">地址数据信息</param>
    /// <param name="length">数据长度信息</param>
    /// <param name="isBit">是否位访问</param>
    /// <returns>实际的读取的报文信息</returns>
    public static OperateResult<List<byte[]>> BuildReadCommand(
      byte cpu,
      string address,
      ushort length,
      bool isBit)
    {
      cpu = (byte) EstHelper.ExtractParameter(ref address, nameof (cpu), (int) cpu);
      OperateResult<YokogawaLinkAddress> from = YokogawaLinkAddress.ParseFrom(address, length);
      if (!from.IsSuccess)
        return OperateResult.CreateFailedResult<List<byte[]>>((OperateResult) from);
      OperateResult<int[], int[]> operateResult = !Authorization.asdniasnfaksndiqwhawfskhfaiw() ? EstHelper.SplitReadLength(from.Content.AddressStart, length, ushort.MaxValue) : (!isBit ? EstHelper.SplitReadLength(from.Content.AddressStart, length, (ushort) 64) : EstHelper.SplitReadLength(from.Content.AddressStart, length, (ushort) 256));
      List<byte[]> numArrayList = new List<byte[]>();
      for (int index = 0; index < operateResult.Content1.Length; ++index)
      {
        from.Content.AddressStart = operateResult.Content1[index];
        byte[] numArray = new byte[12];
        numArray[0] = isBit ? (byte) 1 : (byte) 17;
        numArray[1] = cpu;
        numArray[2] = (byte) 0;
        numArray[3] = (byte) 8;
        from.Content.GetAddressBinaryContent().CopyTo((Array) numArray, 4);
        numArray[10] = BitConverter.GetBytes(operateResult.Content2[index])[1];
        numArray[11] = BitConverter.GetBytes(operateResult.Content2[index])[0];
        numArrayList.Add(numArray);
      }
      return OperateResult.CreateSuccessResult<List<byte[]>>(numArrayList);
    }

    /// <summary>构建随机读取的原始报文的初始命令</summary>
    /// <param name="cpu">Cpu Number</param>
    /// <param name="address">实际的数据地址信息</param>
    /// <param name="isBit">是否是位读取</param>
    /// <returns>实际的读取的报文信息</returns>
    public static OperateResult<List<byte[]>> BuildReadRandomCommand(
      byte cpu,
      string[] address,
      bool isBit)
    {
      List<string[]> strArrayList = SoftBasic.ArraySplitByLength<string>(address, 32);
      List<byte[]> numArrayList = new List<byte[]>();
      foreach (string[] strArray in strArrayList)
      {
        byte[] numArray = new byte[6 + 6 * strArray.Length];
        numArray[0] = isBit ? (byte) 4 : (byte) 20;
        numArray[1] = cpu;
        numArray[2] = BitConverter.GetBytes(numArray.Length - 4)[1];
        numArray[3] = BitConverter.GetBytes(numArray.Length - 4)[0];
        numArray[4] = BitConverter.GetBytes(strArray.Length)[1];
        numArray[5] = BitConverter.GetBytes(strArray.Length)[0];
        for (int index = 0; index < strArray.Length; ++index)
        {
          numArray[1] = (byte) EstHelper.ExtractParameter(ref strArray[index], nameof (cpu), (int) cpu);
          OperateResult<YokogawaLinkAddress> from = YokogawaLinkAddress.ParseFrom(strArray[index], (ushort) 1);
          if (!from.IsSuccess)
            return OperateResult.CreateFailedResult<List<byte[]>>((OperateResult) from);
          from.Content.GetAddressBinaryContent().CopyTo((Array) numArray, 6 * index + 6);
        }
        numArrayList.Add(numArray);
      }
      return OperateResult.CreateSuccessResult<List<byte[]>>(numArrayList);
    }

    /// <summary>构建批量写入Bool数组的命令，需要指定CPU Number信息和设备地址信息</summary>
    /// <param name="cpu">Cpu Number</param>
    /// <param name="address">设备地址数据</param>
    /// <param name="value">实际的bool数组</param>
    /// <returns>构建的写入指令</returns>
    public static OperateResult<byte[]> BuildWriteBoolCommand(
      byte cpu,
      string address,
      bool[] value)
    {
      cpu = (byte) EstHelper.ExtractParameter(ref address, nameof (cpu), (int) cpu);
      OperateResult<YokogawaLinkAddress> from = YokogawaLinkAddress.ParseFrom(address, (ushort) 0);
      if (!from.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) from);
      byte[] numArray = new byte[12 + value.Length];
      numArray[0] = (byte) 2;
      numArray[1] = cpu;
      numArray[2] = (byte) 0;
      numArray[3] = (byte) (8 + value.Length);
      from.Content.GetAddressBinaryContent().CopyTo((Array) numArray, 4);
      numArray[10] = BitConverter.GetBytes(value.Length)[1];
      numArray[11] = BitConverter.GetBytes(value.Length)[0];
      for (int index = 0; index < value.Length; ++index)
        numArray[12 + index] = value[index] ? (byte) 1 : (byte) 0;
      return OperateResult.CreateSuccessResult<byte[]>(numArray);
    }

    /// <summary>构建批量随机写入Bool数组的命令，需要指定CPU Number信息和设备地址信息</summary>
    /// <param name="cpu">Cpu Number</param>
    /// <param name="address">设备地址数据</param>
    /// <param name="value">实际的bool数组</param>
    /// <returns>构建的写入指令</returns>
    public static OperateResult<byte[]> BuildWriteRandomBoolCommand(
      byte cpu,
      string[] address,
      bool[] value)
    {
      if (address.Length != value.Length)
        return new OperateResult<byte[]>(StringResources.Language.TwoParametersLengthIsNotSame);
      byte[] numArray = new byte[6 + address.Length * 8 - 1];
      numArray[0] = (byte) 5;
      numArray[1] = cpu;
      numArray[2] = BitConverter.GetBytes(numArray.Length - 4)[1];
      numArray[3] = BitConverter.GetBytes(numArray.Length - 4)[0];
      numArray[4] = BitConverter.GetBytes(address.Length)[1];
      numArray[5] = BitConverter.GetBytes(address.Length)[0];
      for (int index = 0; index < address.Length; ++index)
      {
        numArray[1] = (byte) EstHelper.ExtractParameter(ref address[index], nameof (cpu), (int) cpu);
        OperateResult<YokogawaLinkAddress> from = YokogawaLinkAddress.ParseFrom(address[index], (ushort) 0);
        if (!from.IsSuccess)
          return OperateResult.CreateFailedResult<byte[]>((OperateResult) from);
        from.Content.GetAddressBinaryContent().CopyTo((Array) numArray, 6 + 8 * index);
        numArray[12 + 8 * index] = value[index] ? (byte) 1 : (byte) 0;
      }
      return OperateResult.CreateSuccessResult<byte[]>(numArray);
    }

    /// <summary>构建字写入的命令报文信息，需要指定设备地址</summary>
    /// <param name="cpu">Cpu Number</param>
    /// <param name="address">地址</param>
    /// <param name="value">原始的数据值</param>
    /// <returns>原始的报文命令</returns>
    public static OperateResult<byte[]> BuildWriteWordCommand(
      byte cpu,
      string address,
      byte[] value)
    {
      cpu = (byte) EstHelper.ExtractParameter(ref address, nameof (cpu), (int) cpu);
      OperateResult<YokogawaLinkAddress> from = YokogawaLinkAddress.ParseFrom(address, (ushort) 0);
      if (!from.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) from);
      byte[] numArray = new byte[12 + value.Length];
      numArray[0] = (byte) 18;
      numArray[1] = cpu;
      numArray[2] = (byte) 0;
      numArray[3] = (byte) (8 + value.Length);
      from.Content.GetAddressBinaryContent().CopyTo((Array) numArray, 4);
      numArray[10] = BitConverter.GetBytes(value.Length / 2)[1];
      numArray[11] = BitConverter.GetBytes(value.Length / 2)[0];
      value.CopyTo((Array) numArray, 12);
      return OperateResult.CreateSuccessResult<byte[]>(numArray);
    }

    /// <summary>构建随机写入字的命令的报文</summary>
    /// <param name="cpu">Cpu Number</param>
    /// <param name="address">地址</param>
    /// <param name="value">原始的数据值</param>
    /// <returns>原始的报文命令</returns>
    public static OperateResult<byte[]> BuildWriteRandomWordCommand(
      byte cpu,
      string[] address,
      byte[] value)
    {
      if (address.Length * 2 != value.Length)
        return new OperateResult<byte[]>(StringResources.Language.TwoParametersLengthIsNotSame);
      byte[] numArray = new byte[6 + address.Length * 8];
      numArray[0] = (byte) 21;
      numArray[1] = cpu;
      numArray[2] = BitConverter.GetBytes(numArray.Length - 4)[1];
      numArray[3] = BitConverter.GetBytes(numArray.Length - 4)[0];
      numArray[4] = BitConverter.GetBytes(address.Length)[1];
      numArray[5] = BitConverter.GetBytes(address.Length)[0];
      for (int index = 0; index < address.Length; ++index)
      {
        numArray[1] = (byte) EstHelper.ExtractParameter(ref address[index], nameof (cpu), (int) cpu);
        OperateResult<YokogawaLinkAddress> from = YokogawaLinkAddress.ParseFrom(address[index], (ushort) 0);
        if (!from.IsSuccess)
          return OperateResult.CreateFailedResult<byte[]>((OperateResult) from);
        from.Content.GetAddressBinaryContent().CopyTo((Array) numArray, 6 + 8 * index);
        numArray[12 + 8 * index] = value[index * 2];
        numArray[13 + 8 * index] = value[index * 2 + 1];
      }
      return OperateResult.CreateSuccessResult<byte[]>(numArray);
    }

    /// <summary>构建启动PLC的命令报文</summary>
    /// <param name="cpu">Cpu Number</param>
    /// <returns>原始的报文命令</returns>
    public static OperateResult<byte[]> BuildStartCommand(byte cpu) => OperateResult.CreateSuccessResult<byte[]>(new byte[4]
    {
      (byte) 69,
      cpu,
      (byte) 0,
      (byte) 0
    });

    /// <summary>构建停止PLC的命令报文</summary>
    /// <param name="cpu">Cpu Number</param>
    /// <returns>原始的报文命令</returns>
    public static OperateResult<byte[]> BuildStopCommand(byte cpu) => OperateResult.CreateSuccessResult<byte[]>(new byte[4]
    {
      (byte) 70,
      cpu,
      (byte) 0,
      (byte) 0
    });

    /// <summary>构建读取特殊模块的命令报文</summary>
    /// <param name="cpu">Cpu Number</param>
    /// <param name="moduleUnit">模块单元号</param>
    /// <param name="moduleSlot">模块站号</param>
    /// <param name="dataPosition">数据位置</param>
    /// <param name="length">长度信息</param>
    /// <returns>原始的报文命令</returns>
    public static List<byte[]> BuildReadSpecialModule(
      byte cpu,
      byte moduleUnit,
      byte moduleSlot,
      ushort dataPosition,
      ushort length)
    {
      List<byte[]> numArrayList = new List<byte[]>();
      OperateResult<int[], int[]> operateResult = EstHelper.SplitReadLength((int) dataPosition, length, (ushort) 64);
      for (int index = 0; index < operateResult.Content1.Length; ++index)
      {
        byte[] numArray = new byte[10];
        numArray[0] = (byte) 49;
        numArray[1] = cpu;
        numArray[2] = BitConverter.GetBytes(numArray.Length - 4)[1];
        numArray[3] = BitConverter.GetBytes(numArray.Length - 4)[0];
        numArray[4] = moduleUnit;
        numArray[5] = moduleSlot;
        numArray[6] = BitConverter.GetBytes(operateResult.Content1[index])[1];
        numArray[7] = BitConverter.GetBytes(operateResult.Content1[index])[0];
        numArray[8] = BitConverter.GetBytes(operateResult.Content2[index])[1];
        numArray[9] = BitConverter.GetBytes(operateResult.Content2[index])[0];
        numArrayList.Add(numArray);
      }
      return numArrayList;
    }

    /// <summary>
    /// 构建读取特殊模块的命令报文，需要传入高级地址，必须以 <b>Special:</b> 开头表示特殊模块地址，示例：Special:cpu=1;unit=0;slot=1;100<br />
    /// To construct a command message to read a special module, the advanced address needs to be passed in.
    /// It must start with <b>Special:</b> to indicate the address of the special module, for example: Special:cpu=1;unit=0;slot=1;100
    /// </summary>
    /// <param name="cpu">Cpu Number</param>
    /// <param name="address">高级的混合地址，除了Cpu可以不携带，例如：Special:unit=0;slot=1;100</param>
    /// <param name="length">长度信息</param>
    /// <returns>原始的报文命令</returns>
    public static OperateResult<List<byte[]>> BuildReadSpecialModule(
      byte cpu,
      string address,
      ushort length)
    {
      if (!address.StartsWith("Special:") && !address.StartsWith("special:"))
        return new OperateResult<List<byte[]>>("Special module address must start with Special:");
      address = address.Substring(8);
      cpu = (byte) EstHelper.ExtractParameter(ref address, nameof (cpu), (int) cpu);
      OperateResult<int> parameter1 = EstHelper.ExtractParameter(ref address, "unit");
      if (!parameter1.IsSuccess)
        return OperateResult.CreateFailedResult<List<byte[]>>((OperateResult) parameter1);
      OperateResult<int> parameter2 = EstHelper.ExtractParameter(ref address, "slot");
      if (!parameter2.IsSuccess)
        return OperateResult.CreateFailedResult<List<byte[]>>((OperateResult) parameter2);
      try
      {
        return OperateResult.CreateSuccessResult<List<byte[]>>(YokogawaLinkTcp.BuildReadSpecialModule(cpu, (byte) parameter1.Content, (byte) parameter2.Content, ushort.Parse(address), length));
      }
      catch (Exception ex)
      {
        return new OperateResult<List<byte[]>>("Address format wrong: " + ex.Message);
      }
    }

    /// <summary>构建读取特殊模块的命令报文</summary>
    /// <param name="cpu">Cpu Number</param>
    /// <param name="moduleUnit">模块单元号</param>
    /// <param name="moduleSlot">模块站号</param>
    /// <param name="dataPosition">数据位置</param>
    /// <param name="data">数据内容</param>
    /// <returns>原始的报文命令</returns>
    public static byte[] BuildWriteSpecialModule(
      byte cpu,
      byte moduleUnit,
      byte moduleSlot,
      ushort dataPosition,
      byte[] data)
    {
      byte[] numArray = new byte[10 + data.Length];
      numArray[0] = (byte) 50;
      numArray[1] = cpu;
      numArray[2] = BitConverter.GetBytes(numArray.Length - 4)[1];
      numArray[3] = BitConverter.GetBytes(numArray.Length - 4)[0];
      numArray[4] = moduleUnit;
      numArray[5] = moduleSlot;
      numArray[6] = BitConverter.GetBytes(dataPosition)[1];
      numArray[7] = BitConverter.GetBytes(dataPosition)[0];
      numArray[8] = BitConverter.GetBytes(data.Length / 2)[1];
      numArray[9] = BitConverter.GetBytes(data.Length / 2)[0];
      data.CopyTo((Array) numArray, 10);
      return numArray;
    }

    /// <summary>
    /// 构建写入特殊模块的命令报文，需要传入高级地址，必须以 <b>Special:</b> 开头表示特殊模块地址，示例：Special:cpu=1;unit=0;slot=1;100<br />
    /// To construct a command message to write a special module, the advanced address needs to be passed in.
    /// It must start with <b>Special:</b> to indicate the address of the special module, for example: Special:cpu=1;unit=0;slot=1;100
    /// </summary>
    /// <param name="cpu">Cpu Number</param>
    /// <param name="address">高级的混合地址，除了Cpu可以不携带，例如：Special:unit=0;slot=1;100</param>
    /// <param name="data">写入的原始数据内容</param>
    /// <returns>原始的报文命令</returns>
    public static OperateResult<byte[]> BuildWriteSpecialModule(
      byte cpu,
      string address,
      byte[] data)
    {
      OperateResult<List<byte[]>> operateResult = YokogawaLinkTcp.BuildReadSpecialModule(cpu, address, (ushort) 0);
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult);
      byte[] numArray = new byte[10 + data.Length];
      numArray[0] = (byte) 50;
      numArray[1] = operateResult.Content[0][1];
      numArray[2] = BitConverter.GetBytes(numArray.Length - 4)[1];
      numArray[3] = BitConverter.GetBytes(numArray.Length - 4)[0];
      numArray[4] = operateResult.Content[0][4];
      numArray[5] = operateResult.Content[0][5];
      numArray[6] = operateResult.Content[0][6];
      numArray[7] = operateResult.Content[0][7];
      numArray[8] = BitConverter.GetBytes(data.Length / 2)[1];
      numArray[9] = BitConverter.GetBytes(data.Length / 2)[0];
      data.CopyTo((Array) numArray, 10);
      return OperateResult.CreateSuccessResult<byte[]>(numArray);
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("YokogawaLinkTcp[{0}:{1}]", (object) this.IpAddress, (object) this.Port);
  }
}
