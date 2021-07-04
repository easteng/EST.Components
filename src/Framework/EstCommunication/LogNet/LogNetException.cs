// Decompiled with JetBrains decompiler
// Type: EstCommunication.LogNet.LogNetException
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using System;

namespace EstCommunication.LogNet
{
  /// <summary>日志存储回调的异常信息</summary>
  public class LogNetException : Exception
  {
    /// <summary>使用其他的异常信息来初始化日志异常</summary>
    /// <param name="innerException">异常信息</param>
    public LogNetException(Exception innerException)
      : base(innerException.Message, innerException)
    {
    }
  }
}
