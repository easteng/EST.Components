// Decompiled with JetBrains decompiler
// Type: EstCommunication.Core.FileGroupInfo
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

namespace EstCommunication.Core
{
  /// <summary>文件的分类信息</summary>
  public class FileGroupInfo
  {
    /// <summary>命令码</summary>
    public int Command { get; set; }

    /// <summary>文件名</summary>
    public string FileName { get; set; }

    /// <summary>文件名列表</summary>
    public string[] FileNames { get; set; }

    /// <summary>第一级分类信息</summary>
    public string Factory { get; set; }

    /// <summary>第二级分类信息</summary>
    public string Group { get; set; }

    /// <summary>第三级分类信息</summary>
    public string Identify { get; set; }
  }
}
