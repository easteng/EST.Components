// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Inovance.InovanceHelper
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Core;

using System;

namespace ESTCore.Common.Profinet.Inovance
{
    /// <summary>
    /// 汇川PLC的辅助类，提供一些地址解析的方法<br />
    /// Auxiliary class of Yaskawa robot, providing some methods of address resolution
    /// </summary>
    public class InovanceHelper
    {
        private static int CalculateStartAddress(string address)
        {
            if (address.IndexOf('.') < 0)
                return int.Parse(address);
            string[] strArray = address.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            return int.Parse(strArray[0]) * 8 + int.Parse(strArray[1]);
        }

        /// <summary>根据汇川PLC的地址，解析出转换后的modbus协议信息</summary>
        /// <param name="address">安川plc的地址信息</param>
        /// <param name="modbusCode">原始的对应的modbus信息</param>
        /// <returns>还原后的modbus地址</returns>
        public static OperateResult<string> PraseInovanceAMAddress(
          string address,
          byte modbusCode)
        {
            try
            {
                string str = string.Empty;
                OperateResult<int> parameter = EstHelper.ExtractParameter(ref address, "s");
                if (parameter.IsSuccess)
                    str = string.Format("s={0};", (object)parameter.Content);
                if (address.StartsWith("QX") || address.StartsWith("qx"))
                    return OperateResult.CreateSuccessResult<string>(str + InovanceHelper.CalculateStartAddress(address.Substring(2)).ToString());
                if (address.StartsWith("Q") || address.StartsWith("q"))
                    return OperateResult.CreateSuccessResult<string>(str + InovanceHelper.CalculateStartAddress(address.Substring(1)).ToString());
                if (address.StartsWith("IX") || address.StartsWith("ix"))
                    return OperateResult.CreateSuccessResult<string>(str + "x=2;" + InovanceHelper.CalculateStartAddress(address.Substring(2)).ToString());
                if (address.StartsWith("I") || address.StartsWith("i"))
                    return OperateResult.CreateSuccessResult<string>(str + "x=2;" + InovanceHelper.CalculateStartAddress(address.Substring(1)).ToString());
                if (address.StartsWith("MW") || address.StartsWith("mw"))
                    return OperateResult.CreateSuccessResult<string>(str + address.Substring(2));
                if (address.StartsWith("M") || address.StartsWith("m"))
                    return OperateResult.CreateSuccessResult<string>(str + address.Substring(1));
                if (modbusCode == (byte)1 || modbusCode == (byte)15 || modbusCode == (byte)5)
                {
                    if (address.StartsWith("SMX") || address.StartsWith("smx"))
                        return OperateResult.CreateSuccessResult<string>(str + string.Format("x={0};", (object)((int)modbusCode + 48)) + InovanceHelper.CalculateStartAddress(address.Substring(3)).ToString());
                    if (address.StartsWith("SM") || address.StartsWith("sm"))
                        return OperateResult.CreateSuccessResult<string>(str + string.Format("x={0};", (object)((int)modbusCode + 48)) + InovanceHelper.CalculateStartAddress(address.Substring(2)).ToString());
                }
                else
                {
                    if (address.StartsWith("SDW") || address.StartsWith("sdw"))
                        return OperateResult.CreateSuccessResult<string>(str + string.Format("x={0};", (object)((int)modbusCode + 48)) + address.Substring(3));
                    if (address.StartsWith("SD") || address.StartsWith("sd"))
                        return OperateResult.CreateSuccessResult<string>(str + string.Format("x={0};", (object)((int)modbusCode + 48)) + address.Substring(2));
                }
                return new OperateResult<string>(StringResources.Language.NotSupportedDataType);
            }
            catch (Exception ex)
            {
                return new OperateResult<string>(ex.Message);
            }
        }

        private static int CalculateH3UStartAddress(string address)
        {
            if (address.IndexOf('.') < 0)
                return Convert.ToInt32(address, 8);
            string[] strArray = address.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            return Convert.ToInt32(strArray[0], 8) * 8 + int.Parse(strArray[1]);
        }

        /// <summary>根据安川PLC的地址，解析出转换后的modbus协议信息，适用H3U系列</summary>
        /// <param name="address">安川plc的地址信息</param>
        /// <param name="modbusCode">原始的对应的modbus信息</param>
        /// <returns>还原后的modbus地址</returns>
        public static OperateResult<string> PraseInovanceH3UAddress(
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
                        return OperateResult.CreateSuccessResult<string>(str + (InovanceHelper.CalculateH3UStartAddress(address.Substring(1)) + 63488).ToString());
                    if (address.StartsWith("Y") || address.StartsWith("y"))
                        return OperateResult.CreateSuccessResult<string>(str + (InovanceHelper.CalculateH3UStartAddress(address.Substring(1)) + 64512).ToString());
                    if (address.StartsWith("SM") || address.StartsWith("sm"))
                        return OperateResult.CreateSuccessResult<string>(str + (Convert.ToInt32(address.Substring(2)) + 9216).ToString());
                    if (address.StartsWith("S") || address.StartsWith("s"))
                        return OperateResult.CreateSuccessResult<string>(str + (Convert.ToInt32(address.Substring(1)) + 57344).ToString());
                    if (address.StartsWith("T") || address.StartsWith("t"))
                        return OperateResult.CreateSuccessResult<string>(str + (Convert.ToInt32(address.Substring(1)) + 61440).ToString());
                    if (address.StartsWith("C") || address.StartsWith("c"))
                        return OperateResult.CreateSuccessResult<string>(str + (Convert.ToInt32(address.Substring(1)) + 62464).ToString());
                    if (address.StartsWith("M") || address.StartsWith("m"))
                    {
                        int int32 = Convert.ToInt32(address.Substring(1));
                        return int32 >= 8000 ? OperateResult.CreateSuccessResult<string>(str + (int32 - 8000 + 8000).ToString()) : OperateResult.CreateSuccessResult<string>(str + int32.ToString());
                    }
                }
                else
                {
                    if (address.StartsWith("D") || address.StartsWith("d"))
                        return OperateResult.CreateSuccessResult<string>(str + Convert.ToInt32(address.Substring(1)).ToString());
                    if (address.StartsWith("SD") || address.StartsWith("sd"))
                        return OperateResult.CreateSuccessResult<string>(str + (Convert.ToInt32(address.Substring(2)) + 9216).ToString());
                    if (address.StartsWith("R") || address.StartsWith("r"))
                        return OperateResult.CreateSuccessResult<string>(str + (Convert.ToInt32(address.Substring(1)) + 12288).ToString());
                    if (address.StartsWith("T") || address.StartsWith("t"))
                        return OperateResult.CreateSuccessResult<string>(str + (Convert.ToInt32(address.Substring(1)) + 61440).ToString());
                    if (address.StartsWith("C") || address.StartsWith("c"))
                    {
                        int int32 = Convert.ToInt32(address.Substring(1));
                        return int32 >= 200 ? OperateResult.CreateSuccessResult<string>(str + ((int32 - 200) * 2 + 63232).ToString()) : OperateResult.CreateSuccessResult<string>(str + (int32 + 62464).ToString());
                    }
                }
                return new OperateResult<string>(StringResources.Language.NotSupportedDataType);
            }
            catch (Exception ex)
            {
                return new OperateResult<string>(ex.Message);
            }
        }

        /// <summary>根据安川PLC的地址，解析出转换后的modbus协议信息，适用H5U系列</summary>
        /// <param name="address">安川plc的地址信息</param>
        /// <param name="modbusCode">原始的对应的modbus信息</param>
        /// <returns>还原后的modbus地址</returns>
        public static OperateResult<string> PraseInovanceH5UAddress(
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
                        return OperateResult.CreateSuccessResult<string>(str + (InovanceHelper.CalculateH3UStartAddress(address.Substring(1)) + 63488).ToString());
                    if (address.StartsWith("Y") || address.StartsWith("y"))
                        return OperateResult.CreateSuccessResult<string>(str + (InovanceHelper.CalculateH3UStartAddress(address.Substring(1)) + 64512).ToString());
                    if (address.StartsWith("S") || address.StartsWith("s"))
                        return OperateResult.CreateSuccessResult<string>(str + (Convert.ToInt32(address.Substring(1)) + 57344).ToString());
                    if (address.StartsWith("B") || address.StartsWith("b"))
                        return OperateResult.CreateSuccessResult<string>(str + (Convert.ToInt32(address.Substring(1)) + 12288).ToString());
                    if (address.StartsWith("M") || address.StartsWith("m"))
                        return OperateResult.CreateSuccessResult<string>(str + Convert.ToInt32(address.Substring(1)).ToString());
                }
                else
                {
                    if (address.StartsWith("D") || address.StartsWith("d"))
                        return OperateResult.CreateSuccessResult<string>(str + Convert.ToInt32(address.Substring(1)).ToString());
                    if (address.StartsWith("R") || address.StartsWith("r"))
                        return OperateResult.CreateSuccessResult<string>(str + (Convert.ToInt32(address.Substring(1)) + 12288).ToString());
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
