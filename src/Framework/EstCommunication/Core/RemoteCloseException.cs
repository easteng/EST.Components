// Decompiled with JetBrains decompiler
// Type: EstCommunication.Core.RemoteCloseException
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using System;

namespace EstCommunication.Core
{
  /// <summary>
  /// 远程对象关闭的异常信息<br />
  /// Exception information of remote object close
  /// </summary>
  public class RemoteCloseException : Exception
  {
    /// <summary>实例化一个默认的对象</summary>
    public RemoteCloseException()
      : base("Remote Closed Exception")
    {
    }
  }
}
