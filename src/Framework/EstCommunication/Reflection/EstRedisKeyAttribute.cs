// Decompiled with JetBrains decompiler
// Type: EstCommunication.Reflection.EstRedisKeyAttribute
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using System;

namespace EstCommunication.Reflection
{
  /// <summary>对应redis的一个键值信息的内容</summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public class EstRedisKeyAttribute : Attribute
  {
    /// <summary>键值的名称</summary>
    public string KeyName { get; set; }

    /// <summary>根据键名来读取写入当前的数据信息</summary>
    /// <param name="key">键名</param>
    public EstRedisKeyAttribute(string key) => this.KeyName = key;
  }
}
