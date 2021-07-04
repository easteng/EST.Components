// Decompiled with JetBrains decompiler
// Type: EstCommunication.Core.Address.DeviceAddressDataBase
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

namespace EstCommunication.Core.Address
{
  /// <summary>
  /// 设备地址数据的信息，通常包含起始地址，数据类型，长度<br />
  /// Device address data information, usually including the starting address, data type, length
  /// </summary>
  public class DeviceAddressDataBase
  {
    /// <summary>
    /// 数字的起始地址，也就是偏移地址<br />
    /// The starting address of the number, which is the offset address
    /// </summary>
    public int AddressStart { get; set; }

    /// <summary>
    /// 读取的数据长度，单位是字节还是字取决于设备方<br />
    /// The length of the data read, the unit is byte or word depends on the device side
    /// </summary>
    public ushort Length { get; set; }

    /// <summary>
    /// 从指定的地址信息解析成真正的设备地址信息<br />
    /// Parsing from the specified address information into real device address information
    /// </summary>
    /// <param name="address">地址信息</param>
    /// <param name="length">数据长度</param>
    public virtual void Parse(string address, ushort length)
    {
      this.AddressStart = int.Parse(address);
      this.Length = length;
    }

    /// <inheritdoc />
    public override string ToString() => this.AddressStart.ToString();
  }
}
