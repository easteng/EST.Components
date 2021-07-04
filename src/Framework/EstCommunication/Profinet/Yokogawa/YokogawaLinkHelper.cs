// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.Yokogawa.YokogawaLinkHelper
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

namespace EstCommunication.Profinet.Yokogawa
{
  /// <summary>横河PLC的通信辅助类。</summary>
  public class YokogawaLinkHelper
  {
    /// <summary>获取横河PLC的错误的具体描述信息</summary>
    /// <param name="code">错误码</param>
    /// <returns></returns>
    public static string GetErrorMsg(byte code)
    {
      switch (code)
      {
        case 1:
          return StringResources.Language.YokogawaLinkError01;
        case 2:
          return StringResources.Language.YokogawaLinkError02;
        case 3:
          return StringResources.Language.YokogawaLinkError03;
        case 4:
          return StringResources.Language.YokogawaLinkError04;
        case 5:
          return StringResources.Language.YokogawaLinkError05;
        case 6:
          return StringResources.Language.YokogawaLinkError06;
        case 7:
          return StringResources.Language.YokogawaLinkError07;
        case 8:
          return StringResources.Language.YokogawaLinkError08;
        case 65:
          return StringResources.Language.YokogawaLinkError41;
        case 66:
          return StringResources.Language.YokogawaLinkError42;
        case 67:
          return StringResources.Language.YokogawaLinkError43;
        case 68:
          return StringResources.Language.YokogawaLinkError44;
        case 81:
          return StringResources.Language.YokogawaLinkError51;
        case 82:
          return StringResources.Language.YokogawaLinkError52;
        case 241:
          return StringResources.Language.YokogawaLinkErrorF1;
        default:
          return StringResources.Language.UnknownError;
      }
    }
  }
}
