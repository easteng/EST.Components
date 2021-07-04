// Decompiled with JetBrains decompiler
// Type: EstCommunication.Reflection.EstRedisHashFieldAttribute
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using System;

namespace EstCommunication.Reflection
{
  /// <summary>对应redis的一个哈希信息的内容</summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public class EstRedisHashFieldAttribute : Attribute
  {
    /// <summary>哈希键值的名称</summary>
    public string HaskKey { get; set; }

    /// <summary>当前的哈希域名称</summary>
    public string Field { get; set; }

    /// <summary>根据键名来读取写入当前的哈希的单个信息</summary>
    /// <param name="hashKey">哈希键名</param>
    /// <param name="filed">哈希域名称</param>
    public EstRedisHashFieldAttribute(string hashKey, string filed)
    {
      this.HaskKey = hashKey;
      this.Field = filed;
    }
  }
}
