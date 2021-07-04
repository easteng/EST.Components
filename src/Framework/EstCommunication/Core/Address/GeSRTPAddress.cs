// Decompiled with JetBrains decompiler
// Type: EstCommunication.Core.Address.GeSRTPAddress
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using System;

namespace EstCommunication.Core.Address
{
  /// <summary>
  /// GE的SRTP协议的地址内容，主要包含一个数据代码信息，还有静态的解析地址的方法<br />
  /// The address content of GE's SRTP protocol mainly includes a data code information, as well as a static method of address resolution
  /// </summary>
  public class GeSRTPAddress : DeviceAddressDataBase
  {
    /// <summary>
    /// 获取或设置等待读取的数据的代码<br />
    /// Get or set the code of the data waiting to be read
    /// </summary>
    public byte DataCode { get; set; }

    /// <inheritdoc />
    public override void Parse(string address, ushort length)
    {
      OperateResult<GeSRTPAddress> from = GeSRTPAddress.ParseFrom(address, length, false);
      if (!from.IsSuccess)
        return;
      this.AddressStart = from.Content.AddressStart;
      this.Length = from.Content.Length;
      this.DataCode = from.Content.DataCode;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.Address.GeSRTPAddress.ParseFrom(System.String,System.UInt16,System.Boolean)" />
    public static OperateResult<GeSRTPAddress> ParseFrom(
      string address,
      bool isBit)
    {
      return GeSRTPAddress.ParseFrom(address, (ushort) 0, isBit);
    }

    /// <summary>
    /// 从GE的地址里，解析出实际的带数据码的 <see cref="T:EstCommunication.Core.Address.GeSRTPAddress" /> 地址信息，起始地址会自动减一，和实际的地址相匹配
    /// </summary>
    /// <param name="address">实际的地址数据</param>
    /// <param name="length">读取的长度信息</param>
    /// <param name="isBit">是否位操作</param>
    /// <returns>是否成功的GE地址对象</returns>
    public static OperateResult<GeSRTPAddress> ParseFrom(
      string address,
      ushort length,
      bool isBit)
    {
      GeSRTPAddress geSrtpAddress = new GeSRTPAddress();
      try
      {
        geSrtpAddress.Length = length;
        if (address.StartsWith("AI") || address.StartsWith("ai"))
        {
          if (isBit)
            return new OperateResult<GeSRTPAddress>(StringResources.Language.GeSRTPNotSupportBitReadWrite);
          geSrtpAddress.DataCode = (byte) 10;
          geSrtpAddress.AddressStart = Convert.ToInt32(address.Substring(2));
        }
        else if (address.StartsWith("AQ") || address.StartsWith("aq"))
        {
          if (isBit)
            return new OperateResult<GeSRTPAddress>(StringResources.Language.GeSRTPNotSupportBitReadWrite);
          geSrtpAddress.DataCode = (byte) 12;
          geSrtpAddress.AddressStart = Convert.ToInt32(address.Substring(2));
        }
        else if (address.StartsWith("R") || address.StartsWith("r"))
        {
          if (isBit)
            return new OperateResult<GeSRTPAddress>(StringResources.Language.GeSRTPNotSupportBitReadWrite);
          geSrtpAddress.DataCode = (byte) 8;
          geSrtpAddress.AddressStart = Convert.ToInt32(address.Substring(1));
        }
        else if (address.StartsWith("SA") || address.StartsWith("sa"))
        {
          geSrtpAddress.DataCode = isBit ? (byte) 78 : (byte) 24;
          geSrtpAddress.AddressStart = Convert.ToInt32(address.Substring(2));
        }
        else if (address.StartsWith("SB") || address.StartsWith("sb"))
        {
          geSrtpAddress.DataCode = isBit ? (byte) 80 : (byte) 26;
          geSrtpAddress.AddressStart = Convert.ToInt32(address.Substring(2));
        }
        else if (address.StartsWith("SC") || address.StartsWith("sc"))
        {
          geSrtpAddress.DataCode = isBit ? (byte) 82 : (byte) 28;
          geSrtpAddress.AddressStart = Convert.ToInt32(address.Substring(2));
        }
        else
        {
          if (address[0] == 'I' || address[0] == 'i')
            geSrtpAddress.DataCode = isBit ? (byte) 70 : (byte) 16;
          else if (address[0] == 'Q' || address[0] == 'q')
            geSrtpAddress.DataCode = isBit ? (byte) 72 : (byte) 18;
          else if (address[0] == 'M' || address[0] == 'm')
            geSrtpAddress.DataCode = isBit ? (byte) 76 : (byte) 22;
          else if (address[0] == 'T' || address[0] == 't')
            geSrtpAddress.DataCode = isBit ? (byte) 74 : (byte) 20;
          else if (address[0] == 'S' || address[0] == 's')
          {
            geSrtpAddress.DataCode = isBit ? (byte) 84 : (byte) 30;
          }
          else
          {
            if (address[0] != 'G' && address[0] != 'g')
              throw new Exception(StringResources.Language.NotSupportedDataType);
            geSrtpAddress.DataCode = isBit ? (byte) 86 : (byte) 56;
          }
          geSrtpAddress.AddressStart = Convert.ToInt32(address.Substring(1));
        }
      }
      catch (Exception ex)
      {
        return new OperateResult<GeSRTPAddress>(ex.Message);
      }
      if (geSrtpAddress.AddressStart == 0)
        return new OperateResult<GeSRTPAddress>(StringResources.Language.GeSRTPAddressCannotBeZero);
      if (geSrtpAddress.AddressStart > 0)
        --geSrtpAddress.AddressStart;
      return OperateResult.CreateSuccessResult<GeSRTPAddress>(geSrtpAddress);
    }
  }
}
