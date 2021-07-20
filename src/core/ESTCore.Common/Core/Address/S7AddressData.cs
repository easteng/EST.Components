// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Core.Address.S7AddressData
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using System;

namespace ESTCore.Common.Core.Address
{
    /// <summary>
    /// 西门子的地址数据信息，主要包含数据代码，DB块，偏移地址，当处于写入时，Length无效<br />
    /// Address data information of Siemens, mainly including data code, DB block, offset address, when writing, Length is invalid
    /// </summary>
    public class S7AddressData : DeviceAddressDataBase
    {
        /// <summary>
        /// 获取或设置等待读取的数据的代码<br />
        /// Get or set the code of the data waiting to be read
        /// </summary>
        public byte DataCode { get; set; }

        /// <summary>
        /// 获取或设置PLC的DB块数据信息<br />
        /// Get or set PLC DB data information
        /// </summary>
        public ushort DbBlock { get; set; }

        /// <summary>从指定的地址信息解析成真正的设备地址信息</summary>
        /// <param name="address">地址信息</param>
        /// <param name="length">数据长度</param>
        public override void Parse(string address, ushort length)
        {
            OperateResult<S7AddressData> from = S7AddressData.ParseFrom(address, length);
            if (!from.IsSuccess)
                return;
            this.AddressStart = from.Content.AddressStart;
            this.Length = from.Content.Length;
            this.DataCode = from.Content.DataCode;
            this.DbBlock = from.Content.DbBlock;
        }

        /// <summary>
        /// 计算特殊的地址信息<br />
        /// Calculate Special Address information
        /// </summary>
        /// <param name="address">字符串地址 -&gt; String address</param>
        /// <param name="isCT">是否是定时器和计数器的地址</param>
        /// <returns>实际值 -&gt; Actual value</returns>
        public static int CalculateAddressStarted(string address, bool isCT = false)
        {
            if (address.IndexOf('.') < 0)
                return isCT ? Convert.ToInt32(address) : Convert.ToInt32(address) * 8;
            string[] strArray = address.Split('.');
            return Convert.ToInt32(strArray[0]) * 8 + Convert.ToInt32(strArray[1]);
        }

        /// <summary>
        /// 从实际的西门子的地址里面解析出地址对象<br />
        /// Resolve the address object from the actual Siemens address
        /// </summary>
        /// <param name="address">西门子的地址数据信息</param>
        /// <returns>是否成功的结果对象</returns>
        public static OperateResult<S7AddressData> ParseFrom(string address) => S7AddressData.ParseFrom(address, (ushort)0);

        /// <summary>
        /// 从实际的西门子的地址里面解析出地址对象<br />
        /// Resolve the address object from the actual Siemens address
        /// </summary>
        /// <param name="address">西门子的地址数据信息</param>
        /// <param name="length">读取的数据长度</param>
        /// <returns>是否成功的结果对象</returns>
        public static OperateResult<S7AddressData> ParseFrom(
          string address,
          ushort length)
        {
            S7AddressData s7AddressData = new S7AddressData();
            try
            {
                s7AddressData.Length = length;
                s7AddressData.DbBlock = (ushort)0;
                if (address.StartsWith("AI") || address.StartsWith("ai"))
                {
                    s7AddressData.DataCode = (byte)6;
                    s7AddressData.AddressStart = S7AddressData.CalculateAddressStarted(address.Substring(2));
                }
                else if (address.StartsWith("AQ") || address.StartsWith("aq"))
                {
                    s7AddressData.DataCode = (byte)7;
                    s7AddressData.AddressStart = S7AddressData.CalculateAddressStarted(address.Substring(2));
                }
                else if (address[0] == 'I')
                {
                    s7AddressData.DataCode = (byte)129;
                    s7AddressData.AddressStart = S7AddressData.CalculateAddressStarted(address.Substring(1));
                }
                else if (address[0] == 'Q')
                {
                    s7AddressData.DataCode = (byte)130;
                    s7AddressData.AddressStart = S7AddressData.CalculateAddressStarted(address.Substring(1));
                }
                else if (address[0] == 'M')
                {
                    s7AddressData.DataCode = (byte)131;
                    s7AddressData.AddressStart = S7AddressData.CalculateAddressStarted(address.Substring(1));
                }
                else if (address[0] == 'D' || address.Substring(0, 2) == "DB")
                {
                    s7AddressData.DataCode = (byte)132;
                    string[] strArray = address.Split('.');
                    s7AddressData.DbBlock = address[1] != 'B' ? Convert.ToUInt16(strArray[0].Substring(1)) : Convert.ToUInt16(strArray[0].Substring(2));
                    string address1 = address.Substring(address.IndexOf('.') + 1);
                    if (address1.StartsWith("DBX") || address1.StartsWith("DBB") || address1.StartsWith("DBW") || address1.StartsWith("DBD"))
                        address1 = address1.Substring(3);
                    s7AddressData.AddressStart = S7AddressData.CalculateAddressStarted(address1);
                }
                else if (address[0] == 'T')
                {
                    s7AddressData.DataCode = (byte)31;
                    s7AddressData.AddressStart = S7AddressData.CalculateAddressStarted(address.Substring(1), true);
                }
                else if (address[0] == 'C')
                {
                    s7AddressData.DataCode = (byte)30;
                    s7AddressData.AddressStart = S7AddressData.CalculateAddressStarted(address.Substring(1), true);
                }
                else
                {
                    if (address[0] != 'V')
                        return new OperateResult<S7AddressData>(StringResources.Language.NotSupportedDataType);
                    s7AddressData.DataCode = (byte)132;
                    s7AddressData.DbBlock = (ushort)1;
                    s7AddressData.AddressStart = S7AddressData.CalculateAddressStarted(address.Substring(1));
                }
            }
            catch (Exception ex)
            {
                return new OperateResult<S7AddressData>(ex.Message);
            }
            return OperateResult.CreateSuccessResult<S7AddressData>(s7AddressData);
        }
    }
}
