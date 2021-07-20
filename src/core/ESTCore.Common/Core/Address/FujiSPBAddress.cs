// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Core.Address.FujiSPBAddress
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Profinet.Fuji;

using System;

namespace ESTCore.Common.Core.Address
{
    /// <summary>FujiSPB的地址信息，可以携带数据类型，起始地址操作</summary>
    public class FujiSPBAddress : DeviceAddressBase
    {
        /// <summary>数据的类型代码</summary>
        public string TypeCode { get; set; }

        /// <summary>当是位地址的时候，用于标记的信息</summary>
        public int BitIndex { get; set; }

        /// <summary>获取读写字数据的时候的地址信息内容</summary>
        /// <returns>报文信息</returns>
        public string GetWordAddress() => this.TypeCode + FujiSPBOverTcp.AnalysisIntegerAddress((int)this.Address);

        /// <summary>获取命令，写入字地址的某一位的命令内容</summary>
        /// <returns>报文信息</returns>
        public string GetWriteBoolAddress()
        {
            int address = (int)this.Address * 2;
            int bitIndex = this.BitIndex;
            if (bitIndex >= 8)
            {
                ++address;
                bitIndex -= 8;
            }
            return string.Format("{0}{1}{2:X2}", (object)this.TypeCode, (object)FujiSPBOverTcp.AnalysisIntegerAddress(address), (object)bitIndex);
        }

        /// <summary>按照位为单位获取相关的索引信息</summary>
        /// <returns>位数据信息</returns>
        public int GetBitIndex() => (int)this.Address * 16 + this.BitIndex;

        /// <summary>
        /// 从实际的Fuji的地址里面解析出地址对象<br />
        /// Resolve the address object from the actual Fuji address
        /// </summary>
        /// <param name="address">富士的地址数据信息</param>
        /// <returns>是否成功的结果对象</returns>
        public static OperateResult<FujiSPBAddress> ParseFrom(string address) => FujiSPBAddress.ParseFrom(address, (ushort)0);

        /// <summary>
        /// 从实际的Fuji的地址里面解析出地址对象<br />
        /// Resolve the address object from the actual Fuji address
        /// </summary>
        /// <param name="address">富士的地址数据信息</param>
        /// <param name="length">读取的数据长度</param>
        /// <returns>是否成功的结果对象</returns>
        public static OperateResult<FujiSPBAddress> ParseFrom(
          string address,
          ushort length)
        {
            FujiSPBAddress fujiSpbAddress = new FujiSPBAddress();
            try
            {
                fujiSpbAddress.BitIndex = EstHelper.GetBitIndexInformation(ref address);
                switch (address[0])
                {
                    case 'C':
                    case 'c':
                        if (address[1] == 'N' || address[1] == 'n')
                        {
                            fujiSpbAddress.TypeCode = "0B";
                            fujiSpbAddress.Address = Convert.ToUInt16(address.Substring(2), 10);
                            break;
                        }
                        if (address[1] != 'C' && address[1] != 'c')
                            throw new Exception(StringResources.Language.NotSupportedDataType);
                        fujiSpbAddress.TypeCode = "05";
                        fujiSpbAddress.Address = Convert.ToUInt16(address.Substring(2), 10);
                        break;
                    case 'D':
                    case 'd':
                        fujiSpbAddress.TypeCode = "0C";
                        fujiSpbAddress.Address = Convert.ToUInt16(address.Substring(1), 10);
                        break;
                    case 'L':
                    case 'l':
                        fujiSpbAddress.TypeCode = "03";
                        fujiSpbAddress.Address = Convert.ToUInt16(address.Substring(1), 10);
                        break;
                    case 'M':
                    case 'm':
                        fujiSpbAddress.TypeCode = "02";
                        fujiSpbAddress.Address = Convert.ToUInt16(address.Substring(1), 10);
                        break;
                    case 'R':
                    case 'r':
                        fujiSpbAddress.TypeCode = "0D";
                        fujiSpbAddress.Address = Convert.ToUInt16(address.Substring(1), 10);
                        break;
                    case 'T':
                    case 't':
                        if (address[1] == 'N' || address[1] == 'n')
                        {
                            fujiSpbAddress.TypeCode = "0A";
                            fujiSpbAddress.Address = Convert.ToUInt16(address.Substring(2), 10);
                            break;
                        }
                        if (address[1] != 'C' && address[1] != 'c')
                            throw new Exception(StringResources.Language.NotSupportedDataType);
                        fujiSpbAddress.TypeCode = "04";
                        fujiSpbAddress.Address = Convert.ToUInt16(address.Substring(2), 10);
                        break;
                    case 'W':
                    case 'w':
                        fujiSpbAddress.TypeCode = "0E";
                        fujiSpbAddress.Address = Convert.ToUInt16(address.Substring(1), 10);
                        break;
                    case 'X':
                    case 'x':
                        fujiSpbAddress.TypeCode = "01";
                        fujiSpbAddress.Address = Convert.ToUInt16(address.Substring(1), 10);
                        break;
                    case 'Y':
                    case 'y':
                        fujiSpbAddress.TypeCode = "00";
                        fujiSpbAddress.Address = Convert.ToUInt16(address.Substring(1), 10);
                        break;
                    default:
                        throw new Exception(StringResources.Language.NotSupportedDataType);
                }
            }
            catch (Exception ex)
            {
                return new OperateResult<FujiSPBAddress>(ex.Message);
            }
            return OperateResult.CreateSuccessResult<FujiSPBAddress>(fujiSpbAddress);
        }
    }
}
