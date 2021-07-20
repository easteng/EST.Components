// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.XINJE.XinJEHelper
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Core;

using System;

namespace ESTCore.Common.Profinet.XINJE
{
    /// <summary>信捷PLC的相关辅助类</summary>
    public class XinJEHelper
    {
        private static int CalculateXinJEStartAddress(string address)
        {
            if (address.IndexOf('.') < 0)
                return Convert.ToInt32(address, 8);
            string[] strArray = address.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            return Convert.ToInt32(strArray[0], 8) * 8 + int.Parse(strArray[1]);
        }

        /// <summary>根据信捷PLC的地址，解析出转换后的modbus协议信息，适用XC系列</summary>
        /// <param name="address">安川plc的地址信息</param>
        /// <param name="modbusCode">原始的对应的modbus信息</param>
        /// <returns>还原后的modbus地址</returns>
        public static OperateResult<string> PraseXinJEXCAddress(
          string address,
          byte modbusCode)
        {
            try
            {
                string str = string.Empty;
                OperateResult<int> parameter = EstHelper.ExtractParameter(ref address, "s");
                if (parameter.IsSuccess)
                    str = string.Format("s={0};", (object)parameter.Content);
                if (modbusCode == (byte)1 || modbusCode == (byte)15 || modbusCode == (byte)5)
                {
                    if (address.StartsWith("X") || address.StartsWith("x"))
                        return OperateResult.CreateSuccessResult<string>(str + (XinJEHelper.CalculateXinJEStartAddress(address.Substring(1)) + 16384).ToString());
                    if (address.StartsWith("Y") || address.StartsWith("y"))
                        return OperateResult.CreateSuccessResult<string>(str + (XinJEHelper.CalculateXinJEStartAddress(address.Substring(1)) + 18432).ToString());
                    if (address.StartsWith("S") || address.StartsWith("s"))
                        return OperateResult.CreateSuccessResult<string>(str + (Convert.ToInt32(address.Substring(1)) + 20480).ToString());
                    if (address.StartsWith("T") || address.StartsWith("t"))
                        return OperateResult.CreateSuccessResult<string>(str + (Convert.ToInt32(address.Substring(1)) + 25600).ToString());
                    if (address.StartsWith("C") || address.StartsWith("c"))
                        return OperateResult.CreateSuccessResult<string>(str + (Convert.ToInt32(address.Substring(1)) + 27648).ToString());
                    if (address.StartsWith("M") || address.StartsWith("m"))
                    {
                        int int32 = Convert.ToInt32(address.Substring(1));
                        return int32 >= 8000 ? OperateResult.CreateSuccessResult<string>(str + (int32 - 8000 + 24576).ToString()) : OperateResult.CreateSuccessResult<string>(str + int32.ToString());
                    }
                }
                else
                {
                    if (address.StartsWith("D") || address.StartsWith("d"))
                    {
                        int int32 = Convert.ToInt32(address.Substring(1));
                        return int32 >= 8000 ? OperateResult.CreateSuccessResult<string>(str + (int32 - 8000 + 16384).ToString()) : OperateResult.CreateSuccessResult<string>(str + int32.ToString());
                    }
                    if (address.StartsWith("F") || address.StartsWith("f"))
                    {
                        int int32 = Convert.ToInt32(address.Substring(1));
                        return int32 >= 8000 ? OperateResult.CreateSuccessResult<string>(str + (int32 - 8000 + 26624).ToString()) : OperateResult.CreateSuccessResult<string>(str + (int32 + 18432).ToString());
                    }
                    if (address.StartsWith("E") || address.StartsWith("e"))
                        return OperateResult.CreateSuccessResult<string>(str + (Convert.ToInt32(address.Substring(1)) + 28672).ToString());
                    if (address.StartsWith("T") || address.StartsWith("t"))
                        return OperateResult.CreateSuccessResult<string>(str + (Convert.ToInt32(address.Substring(1)) + 12288).ToString());
                    if (address.StartsWith("C") || address.StartsWith("c"))
                        return OperateResult.CreateSuccessResult<string>(str + (Convert.ToInt32(address.Substring(1)) + 14336).ToString());
                }
                return new OperateResult<string>(StringResources.Language.NotSupportedDataType);
            }
            catch (Exception ex)
            {
                return new OperateResult<string>(ex.Message);
            }
        }
    }
}
