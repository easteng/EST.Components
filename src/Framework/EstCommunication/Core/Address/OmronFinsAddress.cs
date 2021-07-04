// Decompiled with JetBrains decompiler
// Type: EstCommunication.Core.Address.OmronFinsAddress
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.Profinet.Omron;
using System;

namespace EstCommunication.Core.Address
{
  /// <summary>欧姆龙的Fins协议的地址类对象</summary>
  public class OmronFinsAddress : DeviceAddressDataBase
  {
    /// <summary>进行位操作的指令</summary>
    public byte BitCode { get; set; }

    /// <summary>进行字操作的指令</summary>
    public byte WordCode { get; set; }

    /// <summary>从指定的地址信息解析成真正的设备地址信息</summary>
    /// <param name="address">地址信息</param>
    /// <param name="length">数据长度</param>
    public override void Parse(string address, ushort length)
    {
      OperateResult<OmronFinsAddress> from = OmronFinsAddress.ParseFrom(address, length);
      if (!from.IsSuccess)
        return;
      this.AddressStart = from.Content.AddressStart;
      this.Length = from.Content.Length;
      this.BitCode = from.Content.BitCode;
      this.WordCode = from.Content.WordCode;
    }

    /// <summary>
    /// 从实际的欧姆龙的地址里面解析出地址对象<br />
    /// Resolve the address object from the actual Omron address
    /// </summary>
    /// <param name="address">欧姆龙的地址数据信息</param>
    /// <returns>是否成功的结果对象</returns>
    public static OperateResult<OmronFinsAddress> ParseFrom(
      string address)
    {
      return OmronFinsAddress.ParseFrom(address, (ushort) 0);
    }

    /// <summary>
    /// 从实际的欧姆龙的地址里面解析出地址对象<br />
    /// Resolve the address object from the actual Omron address
    /// </summary>
    /// <param name="address">欧姆龙的地址数据信息</param>
    /// <param name="length">读取的数据长度</param>
    /// <returns>是否成功的结果对象</returns>
    public static OperateResult<OmronFinsAddress> ParseFrom(
      string address,
      ushort length)
    {
      OmronFinsAddress omronFinsAddress = new OmronFinsAddress();
      try
      {
        omronFinsAddress.Length = length;
        switch (address[0])
        {
          case 'A':
          case 'a':
            omronFinsAddress.BitCode = OmronFinsDataType.AR.BitCode;
            omronFinsAddress.WordCode = OmronFinsDataType.AR.WordCode;
            break;
          case 'C':
          case 'c':
            omronFinsAddress.BitCode = OmronFinsDataType.CIO.BitCode;
            omronFinsAddress.WordCode = OmronFinsDataType.CIO.WordCode;
            break;
          case 'D':
          case 'd':
            omronFinsAddress.BitCode = OmronFinsDataType.DM.BitCode;
            omronFinsAddress.WordCode = OmronFinsDataType.DM.WordCode;
            break;
          case 'E':
          case 'e':
            int int32 = Convert.ToInt32(address.Split(new char[1]
            {
              '.'
            }, StringSplitOptions.RemoveEmptyEntries)[0].Substring(1), 16);
            if (int32 < 16)
            {
              omronFinsAddress.BitCode = (byte) (32 + int32);
              omronFinsAddress.WordCode = (byte) (160 + int32);
              break;
            }
            omronFinsAddress.BitCode = (byte) (224 + int32 - 16);
            omronFinsAddress.WordCode = (byte) (96 + int32 - 16);
            break;
          case 'H':
          case 'h':
            omronFinsAddress.BitCode = OmronFinsDataType.HR.BitCode;
            omronFinsAddress.WordCode = OmronFinsDataType.HR.WordCode;
            break;
          case 'W':
          case 'w':
            omronFinsAddress.BitCode = OmronFinsDataType.WR.BitCode;
            omronFinsAddress.WordCode = OmronFinsDataType.WR.WordCode;
            break;
          default:
            throw new Exception(StringResources.Language.NotSupportedDataType);
        }
        if (address[0] == 'E' || address[0] == 'e')
        {
          string[] strArray = address.SplitDot();
          int num = (int) ushort.Parse(strArray[1]) * 16;
          if (strArray.Length > 2)
            num += EstHelper.CalculateBitStartIndex(strArray[2]);
          omronFinsAddress.AddressStart = num;
        }
        else
        {
          string[] strArray = address.Substring(1).SplitDot();
          int num = (int) ushort.Parse(strArray[0]) * 16;
          if (strArray.Length > 1)
            num += EstHelper.CalculateBitStartIndex(strArray[1]);
          omronFinsAddress.AddressStart = num;
        }
      }
      catch (Exception ex)
      {
        return new OperateResult<OmronFinsAddress>(ex.Message);
      }
      return OperateResult.CreateSuccessResult<OmronFinsAddress>(omronFinsAddress);
    }
  }
}
