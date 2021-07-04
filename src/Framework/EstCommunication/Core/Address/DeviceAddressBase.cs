// Decompiled with JetBrains decompiler
// Type: EstCommunication.Core.Address.DeviceAddressBase
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

namespace EstCommunication.Core.Address
{
  /// <summary>
  /// 所有设备通信类的地址基础类<br />
  /// Address basic class of all device communication classes
  /// </summary>
  public class DeviceAddressBase
  {
    /// <summary>
    /// 获取或设置起始地址<br />
    /// Get or set the starting address
    /// </summary>
    public ushort Address { get; set; }

    /// <summary>
    /// 解析字符串的地址<br />
    /// Parse the address of the string
    /// </summary>
    /// <param name="address">地址信息</param>
    public virtual void Parse(string address) => this.Address = ushort.Parse(address);

    /// <inheritdoc />
    public override string ToString() => this.Address.ToString();
  }
}
