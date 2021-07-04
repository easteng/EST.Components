// Decompiled with JetBrains decompiler
// Type: EstCommunication.Core.Address.McRAddressData
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.Profinet.Melsec;

namespace EstCommunication.Core.Address
{
  /// <summary>三菱R系列的PLC的地址表示对象</summary>
  public class McRAddressData : DeviceAddressDataBase
  {
    /// <summary>实例化一个默认的对象</summary>
    public McRAddressData() => this.McDataType = MelsecMcRDataType.D;

    /// <summary>三菱的数据类型及地址信息</summary>
    public MelsecMcRDataType McDataType { get; set; }

    /// <summary>从指定的地址信息解析成真正的设备地址信息，默认是三菱的地址</summary>
    /// <param name="address">地址信息</param>
    /// <param name="length">数据长度</param>
    public override void Parse(string address, ushort length)
    {
      OperateResult<McRAddressData> melsecRfrom = McRAddressData.ParseMelsecRFrom(address, length);
      if (!melsecRfrom.IsSuccess)
        return;
      this.AddressStart = melsecRfrom.Content.AddressStart;
      this.Length = melsecRfrom.Content.Length;
      this.McDataType = melsecRfrom.Content.McDataType;
    }

    /// <summary>解析出三菱R系列的地址信息</summary>
    /// <param name="address">三菱的地址信息</param>
    /// <param name="length">读取的长度，对写入无效</param>
    /// <returns>解析结果</returns>
    public static OperateResult<McRAddressData> ParseMelsecRFrom(
      string address,
      ushort length)
    {
      OperateResult<MelsecMcRDataType, int> operateResult = MelsecMcRNet.AnalysisAddress(address);
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<McRAddressData>((OperateResult) operateResult);
      McRAddressData mcRaddressData = new McRAddressData();
      mcRaddressData.McDataType = operateResult.Content1;
      mcRaddressData.AddressStart = operateResult.Content2;
      mcRaddressData.Length = length;
      return OperateResult.CreateSuccessResult<McRAddressData>(mcRaddressData);
    }
  }
}
