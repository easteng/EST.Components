// Decompiled with JetBrains decompiler
// Type: EstCommunication.Core.Types.CertificateDegree
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

namespace EstCommunication.Core.Types
{
  /// <summary>证书等级</summary>
  public enum CertificateDegree
  {
    /// <summary>只允许读取数据的等级</summary>
    Read = 1,
    /// <summary>允许同时读写数据的等级</summary>
    ReadWrite = 2,
  }
}
