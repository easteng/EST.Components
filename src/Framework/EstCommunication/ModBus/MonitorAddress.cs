// Decompiled with JetBrains decompiler
// Type: EstCommunication.ModBus.MonitorAddress
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

namespace EstCommunication.ModBus
{
  /// <summary>监视使用的数据缓存</summary>
  internal struct MonitorAddress
  {
    /// <summary>地址</summary>
    public ushort Address;
    /// <summary>原有的值</summary>
    public short ValueOrigin;
    /// <summary>新的值</summary>
    public short ValueNew;
  }
}
