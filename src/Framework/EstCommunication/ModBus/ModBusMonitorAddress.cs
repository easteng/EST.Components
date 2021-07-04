// Decompiled with JetBrains decompiler
// Type: EstCommunication.ModBus.ModBusMonitorAddress
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using System;

namespace EstCommunication.ModBus
{
  /// <summary>服务器端提供的数据监视服务</summary>
  public class ModBusMonitorAddress
  {
    /// <summary>本次数据监视的地址</summary>
    public ushort Address { get; set; }

    /// <summary>数据写入时触发的事件</summary>
    public event Action<ModBusMonitorAddress, short> OnWrite;

    /// <summary>数据改变时触发的事件</summary>
    public event Action<ModBusMonitorAddress, short, short> OnChange;

    /// <summary>强制设置触发事件</summary>
    /// <param name="value">数据值信息</param>
    public void SetValue(short value)
    {
      Action<ModBusMonitorAddress, short> onWrite = this.OnWrite;
      if (onWrite == null)
        return;
      onWrite(this, value);
    }

    /// <summary>强制设置触发值变更事件</summary>
    /// <param name="before">变更前的值</param>
    /// <param name="after">变更后的值</param>
    public void SetChangeValue(short before, short after)
    {
      if ((int) before == (int) after)
        return;
      Action<ModBusMonitorAddress, short, short> onChange = this.OnChange;
      if (onChange != null)
        onChange(this, before, after);
    }
  }
}
