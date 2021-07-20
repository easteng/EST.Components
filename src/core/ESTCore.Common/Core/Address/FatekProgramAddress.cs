// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Core.Address.FatekProgramAddress
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using System;

namespace ESTCore.Common.Core.Address
{
    /// <summary>永宏编程口的地址类对象</summary>
    public class FatekProgramAddress : DeviceAddressDataBase
    {
        /// <summary>数据的类型</summary>
        public string DataCode { get; set; }

        /// <inheritdoc />
        public override void Parse(string address, ushort length)
        {
            OperateResult<FatekProgramAddress> from = FatekProgramAddress.ParseFrom(address, length);
            if (!from.IsSuccess)
                return;
            this.AddressStart = from.Content.AddressStart;
            this.Length = from.Content.Length;
            this.DataCode = from.Content.DataCode;
        }

        /// <inheritdoc />
        public override string ToString() => this.DataCode == "X" || this.DataCode == "Y" || (this.DataCode == "M" || this.DataCode == "S") || (this.DataCode == "T" || this.DataCode == "C" || this.DataCode == "RT") || this.DataCode == "RC" ? this.DataCode + this.AddressStart.ToString("D4") : this.DataCode + this.AddressStart.ToString("D5");

        /// <summary>从普通的PLC的地址转换为HSL标准的地址信息</summary>
        /// <param name="address">地址信息</param>
        /// <param name="length">数据长度</param>
        /// <returns>是否成功的地址结果</returns>
        public static OperateResult<FatekProgramAddress> ParseFrom(
          string address,
          ushort length)
        {
            try
            {
                FatekProgramAddress fatekProgramAddress = new FatekProgramAddress();
                switch (address[0])
                {
                    case 'C':
                    case 'c':
                        fatekProgramAddress.DataCode = "C";
                        fatekProgramAddress.AddressStart = (int)Convert.ToUInt16(address.Substring(1), 10);
                        break;
                    case 'D':
                    case 'd':
                        fatekProgramAddress.DataCode = "D";
                        fatekProgramAddress.AddressStart = (int)Convert.ToUInt16(address.Substring(1), 10);
                        break;
                    case 'M':
                    case 'm':
                        fatekProgramAddress.DataCode = "M";
                        fatekProgramAddress.AddressStart = (int)Convert.ToUInt16(address.Substring(1), 10);
                        break;
                    case 'R':
                    case 'r':
                        if (address[1] == 'T' || address[1] == 't')
                        {
                            fatekProgramAddress.DataCode = "RT";
                            fatekProgramAddress.AddressStart = (int)Convert.ToUInt16(address.Substring(2), 10);
                            break;
                        }
                        if (address[1] == 'C' || address[1] == 'c')
                        {
                            fatekProgramAddress.DataCode = "RC";
                            fatekProgramAddress.AddressStart = (int)Convert.ToUInt16(address.Substring(2), 10);
                            break;
                        }
                        fatekProgramAddress.DataCode = "R";
                        fatekProgramAddress.AddressStart = (int)Convert.ToUInt16(address.Substring(1), 10);
                        break;
                    case 'S':
                    case 's':
                        fatekProgramAddress.DataCode = "S";
                        fatekProgramAddress.AddressStart = (int)Convert.ToUInt16(address.Substring(1), 10);
                        break;
                    case 'T':
                    case 't':
                        fatekProgramAddress.DataCode = "T";
                        fatekProgramAddress.AddressStart = (int)Convert.ToUInt16(address.Substring(1), 10);
                        break;
                    case 'X':
                    case 'x':
                        fatekProgramAddress.DataCode = "X";
                        fatekProgramAddress.AddressStart = (int)Convert.ToUInt16(address.Substring(1), 10);
                        break;
                    case 'Y':
                    case 'y':
                        fatekProgramAddress.DataCode = "Y";
                        fatekProgramAddress.AddressStart = (int)Convert.ToUInt16(address.Substring(1), 10);
                        break;
                    default:
                        throw new Exception(StringResources.Language.NotSupportedDataType);
                }
                return OperateResult.CreateSuccessResult<FatekProgramAddress>(fatekProgramAddress);
            }
            catch (Exception ex)
            {
                return new OperateResult<FatekProgramAddress>(ex.Message);
            }
        }
    }
}
