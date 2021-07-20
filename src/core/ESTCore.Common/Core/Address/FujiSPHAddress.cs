// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Core.Address.FujiSPHAddress
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using System;

namespace ESTCore.Common.Core.Address
{
    /// <summary>富士SPH地址类对象</summary>
    public class FujiSPHAddress : DeviceAddressDataBase
    {
        /// <summary>数据的类型代码</summary>
        public byte TypeCode { get; set; }

        /// <summary>当前地址的位索引信息</summary>
        public int BitIndex { get; set; }

        /// <summary>
        /// 从实际的Fuji的地址里面解析出地址对象<br />
        /// Resolve the address object from the actual Fuji address
        /// </summary>
        /// <param name="address">富士的地址数据信息</param>
        /// <returns>是否成功的结果对象</returns>
        public static OperateResult<FujiSPHAddress> ParseFrom(string address) => FujiSPHAddress.ParseFrom(address, (ushort)0);

        /// <summary>
        /// 从实际的Fuji的地址里面解析出地址对象<br />
        /// Resolve the address object from the actual Fuji address
        /// </summary>
        /// <param name="address">富士的地址数据信息</param>
        /// <param name="length">读取的数据长度</param>
        /// <returns>是否成功的结果对象</returns>
        public static OperateResult<FujiSPHAddress> ParseFrom(
          string address,
          ushort length)
        {
            FujiSPHAddress fujiSphAddress = new FujiSPHAddress();
            try
            {
                switch (address[0])
                {
                    case 'I':
                    case 'Q':
                    case 'i':
                    case 'q':
                        string[] strArray1 = address.SplitDot();
                        fujiSphAddress.TypeCode = (byte)1;
                        fujiSphAddress.AddressStart = Convert.ToInt32(strArray1[0].Substring(1));
                        if (strArray1.Length > 1)
                        {
                            fujiSphAddress.BitIndex = EstHelper.CalculateBitStartIndex(strArray1[1]);
                            break;
                        }
                        break;
                    case 'M':
                    case 'm':
                        string[] strArray2 = address.SplitDot();
                        switch (int.Parse(strArray2[0].Substring(1)))
                        {
                            case 1:
                                fujiSphAddress.TypeCode = (byte)2;
                                break;
                            case 3:
                                fujiSphAddress.TypeCode = (byte)4;
                                break;
                            case 10:
                                fujiSphAddress.TypeCode = (byte)8;
                                break;
                            default:
                                throw new Exception(StringResources.Language.NotSupportedDataType);
                        }
                        fujiSphAddress.AddressStart = Convert.ToInt32(strArray2[1]);
                        if (strArray2.Length > 2)
                        {
                            fujiSphAddress.BitIndex = EstHelper.CalculateBitStartIndex(strArray2[2]);
                            break;
                        }
                        break;
                    default:
                        throw new Exception(StringResources.Language.NotSupportedDataType);
                }
            }
            catch (Exception ex)
            {
                return new OperateResult<FujiSPHAddress>(ex.Message);
            }
            return OperateResult.CreateSuccessResult<FujiSPHAddress>(fujiSphAddress);
        }
    }
}
