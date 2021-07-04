// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.Delta.DeltaHelper
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.Core;
using System;

namespace EstCommunication.Profinet.Delta
{
  /// <summary>
  /// 台达PLC的相关的帮助类，公共的地址解析的方法。<br />
  /// Delta PLC related help classes, public address resolution methods.
  /// </summary>
  public class DeltaHelper
  {
    /// <summary>
    /// 根据台达PLC的地址，解析出转换后的modbus协议信息，适用DVP系列，当前的地址仍然支持站号指定，例如s=2;D100<br />
    /// According to the address of Delta PLC, the converted modbus protocol information is parsed out, applicable to DVP series,
    /// the current address still supports station number designation, such as s=2;D100
    /// </summary>
    /// <param name="address">台达plc的地址信息</param>
    /// <param name="modbusCode">原始的对应的modbus信息</param>
    /// <returns>还原后的modbus地址</returns>
    public static OperateResult<string> PraseDeltaDvpAddress(
      string address,
      byte modbusCode)
    {
      try
      {
        string str = string.Empty;
        OperateResult<int> parameter = EstHelper.ExtractParameter(ref address, "s");
        if (parameter.IsSuccess)
          str = string.Format("s={0};", (object) parameter.Content);
        if (modbusCode == (byte) 1 || modbusCode == (byte) 15 || modbusCode == (byte) 5)
        {
          if (address.StartsWith("S") || address.StartsWith("s"))
            return OperateResult.CreateSuccessResult<string>(str + Convert.ToInt32(address.Substring(1)).ToString());
          if (address.StartsWith("X") || address.StartsWith("x"))
            return OperateResult.CreateSuccessResult<string>(str + "x=2;" + (Convert.ToInt32(address.Substring(1), 8) + 1024).ToString());
          if (address.StartsWith("Y") || address.StartsWith("y"))
            return OperateResult.CreateSuccessResult<string>(str + (Convert.ToInt32(address.Substring(1), 8) + 1280).ToString());
          if (address.StartsWith("T") || address.StartsWith("t"))
            return OperateResult.CreateSuccessResult<string>(str + (Convert.ToInt32(address.Substring(1)) + 1536).ToString());
          if (address.StartsWith("C") || address.StartsWith("c"))
            return OperateResult.CreateSuccessResult<string>(str + (Convert.ToInt32(address.Substring(1)) + 3584).ToString());
          if (address.StartsWith("M") || address.StartsWith("m"))
          {
            int int32 = Convert.ToInt32(address.Substring(1));
            return int32 >= 1536 ? OperateResult.CreateSuccessResult<string>(str + (int32 - 1536 + 45056).ToString()) : OperateResult.CreateSuccessResult<string>(str + (int32 + 2048).ToString());
          }
        }
        else
        {
          if (address.StartsWith("D") || address.StartsWith("d"))
          {
            int int32 = Convert.ToInt32(address.Substring(1));
            return int32 >= 4096 ? OperateResult.CreateSuccessResult<string>(str + (int32 - 4096 + 36864).ToString()) : OperateResult.CreateSuccessResult<string>(str + (int32 + 4096).ToString());
          }
          if (address.StartsWith("C") || address.StartsWith("c"))
          {
            int int32 = Convert.ToInt32(address.Substring(1));
            return int32 >= 200 ? OperateResult.CreateSuccessResult<string>(str + (int32 - 200 + 3784).ToString()) : OperateResult.CreateSuccessResult<string>(str + (int32 + 3584).ToString());
          }
          if (address.StartsWith("T") || address.StartsWith("t"))
            return OperateResult.CreateSuccessResult<string>(str + (Convert.ToInt32(address.Substring(1)) + 1536).ToString());
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
