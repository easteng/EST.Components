// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.Panasonic.PanasonicMcNet
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.Core.Address;
using EstCommunication.Profinet.Melsec;

namespace EstCommunication.Profinet.Panasonic
{
  /// <summary>
  /// 松下PLC的数据读写类，基于MC协议的实现，具体的地址格式请参考备注说明<br />
  /// Data reading and writing of Panasonic PLC, based on the implementation of the MC protocol, please refer to the note for specific address format
  /// </summary>
  /// <remarks>
  /// 地址的输入的格式说明如下：
  /// <list type="table">
  ///   <listheader>
  ///     <term>地址名称</term>
  ///     <term>地址代号</term>
  ///     <term>地址示例一</term>
  ///     <term>地址范围</term>
  ///     <term>字操作</term>
  ///     <term>位操作</term>
  ///   </listheader>
  ///   <item>
  ///     <term>输入继电器</term>
  ///     <term>X</term>
  ///     <term>X0000,X100F</term>
  ///     <term>X0000～X109F</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///   </item>
  ///   <item>
  ///     <term>输出继电器</term>
  ///     <term>Y</term>
  ///     <term>Y0000,Y100F</term>
  ///     <term>Y0000～Y109F</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///   </item>
  ///   <item>
  ///     <term>链接继电器</term>
  ///     <term>L</term>
  ///     <term>L0000,L100F</term>
  ///     <term>L0000～L0127F</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///   </item>
  ///   <item>
  ///     <term>内部继电器</term>
  ///     <term>R</term>
  ///     <term>R0000,R100F</term>
  ///     <term>R0000～R511F,R9000～R951F</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///   </item>
  ///   <item>
  ///     <term>数据存储器</term>
  ///     <term>D</term>
  ///     <term>D100,D200</term>
  ///     <term>D0～D65532,D90000～D90999</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///   </item>
  ///   <item>
  ///     <term>链路寄存器</term>
  ///     <term>LD</term>
  ///     <term>LD0,LD100</term>
  ///     <term>LD0～LD255</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///   </item>
  ///   <item>
  ///     <term>定时器（当前值）</term>
  ///     <term>TN</term>
  ///     <term>TN100,TN200</term>
  ///     <term>TN0～TN1023</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///   </item>
  ///   <item>
  ///     <term>定时器（接点）</term>
  ///     <term>TS</term>
  ///     <term>TS100,TS200</term>
  ///     <term>TS0～TS1023</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///   </item>
  ///   <item>
  ///     <term>计数器（当前值）</term>
  ///     <term>CN</term>
  ///     <term>CN100,CN200</term>
  ///     <term>CN0～CN1023</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///   </item>
  ///   <item>
  ///     <term>计数器（接点）</term>
  ///     <term>CS</term>
  ///     <term>CS100,CS200</term>
  ///     <term>CS0～CS1023</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///   </item>
  /// </list>
  /// </remarks>
  public class PanasonicMcNet : MelsecMcNet
  {
    /// <summary>
    /// 实例化松下的的Qna兼容3E帧协议的通讯对象<br />
    /// Instantiate Panasonic's Qna compatible 3E frame protocol communication object
    /// </summary>
    public PanasonicMcNet()
    {
    }

    /// <summary>
    /// 指定ip地址及端口号来实例化一个基恩士的Qna兼容3E帧协议的通讯对象<br />
    /// Specify an IP address and port number to instantiate a Keynes Qna compatible 3E frame protocol communication object
    /// </summary>
    /// <param name="ipAddress">PLC的Ip地址</param>
    /// <param name="port">PLC的端口</param>
    public PanasonicMcNet(string ipAddress, int port)
      : base(ipAddress, port)
    {
    }

    /// <inheritdoc />
    protected override OperateResult<McAddressData> McAnalysisAddress(
      string address,
      ushort length)
    {
      return McAddressData.ParsePanasonicFrom(address, length);
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("PanasonicMcNet[{0}:{1}]", (object) this.IpAddress, (object) this.Port);
  }
}
