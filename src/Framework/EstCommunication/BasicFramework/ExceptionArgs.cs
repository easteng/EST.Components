// Decompiled with JetBrains decompiler
// Type: EstCommunication.BasicFramework.ExceptionArgs
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using System;

namespace EstCommunication.BasicFramework
{
  /// <summary>异常消息基类</summary>
  [Serializable]
  public abstract class ExceptionArgs
  {
    /// <inheritdoc />
    public virtual string Message => string.Empty;
  }
}
