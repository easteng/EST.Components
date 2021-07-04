// Decompiled with JetBrains decompiler
// Type: EstCommunication.Core.Address.YokogawaLinkAddress
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using System;

namespace EstCommunication.Core.Address
{
  /// <summary>
  /// 横河PLC的地址表示类<br />
  /// Yokogawa PLC address display class
  /// </summary>
  public class YokogawaLinkAddress : DeviceAddressDataBase
  {
    /// <summary>
    /// 获取或设置等待读取的数据的代码<br />
    /// Get or set the code of the data waiting to be read
    /// </summary>
    public int DataCode { get; set; }

    /// <summary>
    /// 获取当前横河PLC的地址的二进制表述方式<br />
    /// Obtain the binary representation of the current Yokogawa PLC address
    /// </summary>
    /// <returns>二进制数据信息</returns>
    public byte[] GetAddressBinaryContent() => new byte[6]
    {
      BitConverter.GetBytes(this.DataCode)[1],
      BitConverter.GetBytes(this.DataCode)[0],
      BitConverter.GetBytes(this.AddressStart)[3],
      BitConverter.GetBytes(this.AddressStart)[2],
      BitConverter.GetBytes(this.AddressStart)[1],
      BitConverter.GetBytes(this.AddressStart)[0]
    };

    /// <inheritdoc />
    public override void Parse(string address, ushort length)
    {
      OperateResult<YokogawaLinkAddress> from = YokogawaLinkAddress.ParseFrom(address, length);
      if (!from.IsSuccess)
        return;
      this.AddressStart = from.Content.AddressStart;
      this.Length = from.Content.Length;
      this.DataCode = from.Content.DataCode;
    }

    /// <summary>从普通的PLC的地址转换为HSL标准的地址信息</summary>
    /// <param name="address">地址信息</param>
    /// <param name="length">数据长度</param>
    /// <returns>是否成功的地址结果</returns>
    public static OperateResult<YokogawaLinkAddress> ParseFrom(
      string address,
      ushort length)
    {
      try
      {
        int num1;
        int num2;
        if (address.StartsWith("CN") || address.StartsWith("cn"))
        {
          num1 = 49;
          num2 = int.Parse(address.Substring(2));
        }
        else if (address.StartsWith("TN") || address.StartsWith("tn"))
        {
          num1 = 33;
          num2 = int.Parse(address.Substring(2));
        }
        else if (address.StartsWith("X") || address.StartsWith("x"))
        {
          num1 = 24;
          num2 = int.Parse(address.Substring(1));
        }
        else if (address.StartsWith("Y") || address.StartsWith("y"))
        {
          num1 = 25;
          num2 = int.Parse(address.Substring(1));
        }
        else if (address.StartsWith("I") || address.StartsWith("i"))
        {
          num1 = 9;
          num2 = int.Parse(address.Substring(1));
        }
        else if (address.StartsWith("E") || address.StartsWith("e"))
        {
          num1 = 5;
          num2 = int.Parse(address.Substring(1));
        }
        else if (address.StartsWith("M") || address.StartsWith("m"))
        {
          num1 = 13;
          num2 = int.Parse(address.Substring(1));
        }
        else if (address.StartsWith("T") || address.StartsWith("t"))
        {
          num1 = 20;
          num2 = int.Parse(address.Substring(1));
        }
        else if (address.StartsWith("C") || address.StartsWith("c"))
        {
          num1 = 3;
          num2 = int.Parse(address.Substring(1));
        }
        else if (address.StartsWith("L") || address.StartsWith("l"))
        {
          num1 = 12;
          num2 = int.Parse(address.Substring(1));
        }
        else if (address.StartsWith("D") || address.StartsWith("d"))
        {
          num1 = 4;
          num2 = int.Parse(address.Substring(1));
        }
        else if (address.StartsWith("B") || address.StartsWith("b"))
        {
          num1 = 2;
          num2 = int.Parse(address.Substring(1));
        }
        else if (address.StartsWith("F") || address.StartsWith("f"))
        {
          num1 = 6;
          num2 = int.Parse(address.Substring(1));
        }
        else if (address.StartsWith("R") || address.StartsWith("r"))
        {
          num1 = 18;
          num2 = int.Parse(address.Substring(1));
        }
        else if (address.StartsWith("V") || address.StartsWith("v"))
        {
          num1 = 22;
          num2 = int.Parse(address.Substring(1));
        }
        else if (address.StartsWith("Z") || address.StartsWith("z"))
        {
          num1 = 26;
          num2 = int.Parse(address.Substring(1));
        }
        else
        {
          if (!address.StartsWith("W") && !address.StartsWith("w"))
            throw new Exception(StringResources.Language.NotSupportedDataType);
          num1 = 23;
          num2 = int.Parse(address.Substring(1));
        }
        YokogawaLinkAddress yokogawaLinkAddress = new YokogawaLinkAddress();
        yokogawaLinkAddress.DataCode = num1;
        yokogawaLinkAddress.AddressStart = num2;
        yokogawaLinkAddress.Length = length;
        return OperateResult.CreateSuccessResult<YokogawaLinkAddress>(yokogawaLinkAddress);
      }
      catch (Exception ex)
      {
        return new OperateResult<YokogawaLinkAddress>(ex.Message);
      }
    }
  }
}
