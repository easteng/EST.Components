// Decompiled with JetBrains decompiler
// Type: EstCommunication.StringResources
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.Language;
using System.Globalization;

namespace EstCommunication
{
  /// <summary>
  /// 系统的字符串资源及多语言管理中心<br />
  /// System string resource and multi-language management Center
  /// </summary>
  public static class StringResources
  {
    /// <summary>
    /// 获取或设置系统的语言选项<br />
    /// Gets or sets the language options for the system
    /// </summary>
    public static DefaultLanguage Language = new DefaultLanguage();

    static StringResources()
    {
      if (CultureInfo.CurrentCulture.ToString().StartsWith("zh"))
        StringResources.SetLanguageChinese();
      else
        StringResources.SeteLanguageEnglish();
    }

    /// <summary>
    /// 将语言设置为中文<br />
    /// Set the language to Chinese
    /// </summary>
    public static void SetLanguageChinese() => StringResources.Language = new DefaultLanguage();

    /// <summary>
    /// 将语言设置为英文<br />
    /// Set the language to English
    /// </summary>
    public static void SeteLanguageEnglish() => StringResources.Language = (DefaultLanguage) new English();
  }
}
