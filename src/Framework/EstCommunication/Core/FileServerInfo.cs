// Decompiled with JetBrains decompiler
// Type: EstCommunication.Core.FileServerInfo
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

namespace EstCommunication.Core
{
  /// <summary>文件在服务器上的信息</summary>
  public class FileServerInfo : FileBaseInfo
  {
    /// <summary>文件的真实路径</summary>
    public string ActualFileFullName { get; set; }
  }
}
