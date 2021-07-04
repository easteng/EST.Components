// Decompiled with JetBrains decompiler
// Type: EstCommunication.LogNet.LogSaveMode
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

namespace EstCommunication.LogNet
{
  /// <summary>日志文件的存储模式</summary>
  public enum LogSaveMode
  {
    /// <summary>单个文件的存储模式</summary>
    SingleFile = 1,
    /// <summary>根据文件的大小来存储，固定一个大小，不停的生成文件</summary>
    FileFixedSize = 2,
    /// <summary>根据时间来存储，可以设置年，季，月，日，小时等等</summary>
    Time = 3,
  }
}
