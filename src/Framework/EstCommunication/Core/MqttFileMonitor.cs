// Decompiled with JetBrains decompiler
// Type: EstCommunication.Core.MqttFileMonitor
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using System.Collections.Generic;
using System.Linq;

namespace EstCommunication.Core
{
  /// <summary>监控上传和下载文件的信息</summary>
  public class MqttFileMonitor
  {
    private Dictionary<long, MqttFileMonitorItem> fileMonitors;
    private object dicLock;

    /// <summary>实例化一个默认的对象</summary>
    public MqttFileMonitor()
    {
      this.dicLock = new object();
      this.fileMonitors = new Dictionary<long, MqttFileMonitorItem>();
    }

    /// <summary>增加一个文件监控的对象信息</summary>
    /// <param name="monitorItem">文件监控对象</param>
    public void Add(MqttFileMonitorItem monitorItem)
    {
      lock (this.dicLock)
      {
        if (this.fileMonitors.ContainsKey(monitorItem.UniqueId))
          this.fileMonitors[monitorItem.UniqueId] = monitorItem;
        else
          this.fileMonitors.Add(monitorItem.UniqueId, monitorItem);
      }
    }

    /// <summary>根据唯一的ID信息，移除相关的文件监控对象</summary>
    /// <param name="uniqueId"></param>
    public void Remove(long uniqueId)
    {
      lock (this.dicLock)
      {
        if (!this.fileMonitors.ContainsKey(uniqueId))
          return;
        this.fileMonitors.Remove(uniqueId);
      }
    }

    /// <summary>获取当前所有的监控文件数据的快照</summary>
    /// <returns>文件监控列表</returns>
    public MqttFileMonitorItem[] GetMonitorItemsSnapShoot()
    {
      lock (this.dicLock)
        return this.fileMonitors.Values.ToArray<MqttFileMonitorItem>();
    }
  }
}
