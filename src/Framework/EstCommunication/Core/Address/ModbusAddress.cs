// Decompiled with JetBrains decompiler
// Type: EstCommunication.Core.Address.ModbusAddress
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using System.Text;

namespace EstCommunication.Core.Address
{
  /// <summary>
  /// Modbus协议地址格式，可以携带站号，功能码，地址信息<br />
  /// Modbus protocol address format, can carry station number, function code, address information
  /// </summary>
  public class ModbusAddress : DeviceAddressBase
  {
    /// <summary>
    /// 实例化一个默认的对象<br />
    /// Instantiate a default object
    /// </summary>
    public ModbusAddress()
    {
      this.Station = -1;
      this.Function = -1;
      this.Address = (ushort) 0;
    }

    /// <summary>
    /// 实例化一个对象，使用指定的地址初始化<br />
    /// Instantiate an object, initialize with the specified address
    /// </summary>
    /// <param name="address">传入的地址信息，支持富地址，例如s=2;x=3;100</param>
    public ModbusAddress(string address)
    {
      this.Station = -1;
      this.Function = -1;
      this.Address = (ushort) 0;
      this.Parse(address);
    }

    /// <summary>
    /// 实例化一个对象，使用指定的地址及功能码初始化<br />
    /// Instantiate an object and initialize it with the specified address and function code
    /// </summary>
    /// <param name="address">传入的地址信息，支持富地址，例如s=2;x=3;100</param>
    /// <param name="function">默认的功能码信息</param>
    public ModbusAddress(string address, byte function)
    {
      this.Station = -1;
      this.Function = (int) function;
      this.Address = (ushort) 0;
      this.Parse(address);
    }

    /// <summary>
    /// 实例化一个对象，使用指定的地址，站号，功能码来初始化<br />
    /// Instantiate an object, use the specified address, station number, function code to initialize
    /// </summary>
    /// <param name="address">传入的地址信息，支持富地址，例如s=2;x=3;100</param>
    /// <param name="station">站号信息</param>
    /// <param name="function">默认的功能码信息</param>
    public ModbusAddress(string address, byte station, byte function)
    {
      this.Station = -1;
      this.Function = (int) function;
      this.Station = (int) station;
      this.Address = (ushort) 0;
      this.Parse(address);
    }

    /// <summary>
    /// 获取或设置当前地址的站号信息<br />
    /// Get or set the station number information of the current address
    /// </summary>
    public int Station { get; set; }

    /// <summary>
    /// 获取或设置当前地址携带的功能码<br />
    /// Get or set the function code carried by the current address
    /// </summary>
    public int Function { get; set; }

    /// <inheritdoc />
    public override void Parse(string address)
    {
      if (address.IndexOf(';') < 0)
      {
        this.Address = ushort.Parse(address);
      }
      else
      {
        string[] strArray = address.Split(';');
        for (int index = 0; index < strArray.Length; ++index)
        {
          if (strArray[index][0] == 's' || strArray[index][0] == 'S')
            this.Station = (int) byte.Parse(strArray[index].Substring(2));
          else if (strArray[index][0] == 'x' || strArray[index][0] == 'X')
            this.Function = (int) byte.Parse(strArray[index].Substring(2));
          else
            this.Address = ushort.Parse(strArray[index]);
        }
      }
    }

    /// <summary>
    /// 地址偏移指定的位置，返回一个新的地址对象<br />
    /// The address is offset by the specified position and a new address object is returned
    /// </summary>
    /// <param name="value">数据值信息</param>
    /// <returns>新增后的地址信息</returns>
    public ModbusAddress AddressAdd(int value)
    {
      ModbusAddress modbusAddress = new ModbusAddress();
      modbusAddress.Station = this.Station;
      modbusAddress.Function = this.Function;
      modbusAddress.Address = (ushort) ((uint) this.Address + (uint) value);
      return modbusAddress;
    }

    /// <summary>
    /// 地址偏移1，返回一个新的地址对象<br />
    /// The address is offset by 1 and a new address object is returned
    /// </summary>
    /// <returns>新增后的地址信息</returns>
    public ModbusAddress AddressAdd() => this.AddressAdd(1);

    /// <inheritdoc />
    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (this.Station >= 0)
        stringBuilder.Append("s=" + this.Station.ToString() + ";");
      if (this.Function >= 1)
        stringBuilder.Append("x=" + this.Function.ToString() + ";");
      stringBuilder.Append(this.Address.ToString());
      return stringBuilder.ToString();
    }
  }
}
