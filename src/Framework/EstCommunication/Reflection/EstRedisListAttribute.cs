// Decompiled with JetBrains decompiler
// Type: EstCommunication.Reflection.EstRedisListAttribute
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using System;

namespace EstCommunication.Reflection
{
  /// <summary>对应redis的一个列表信息的内容</summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public class EstRedisListAttribute : Attribute
  {
    /// <summary>列表键值的名称</summary>
    public string ListKey { get; set; }

    /// <summary>当前的位置的索引</summary>
    public long StartIndex { get; set; }

    /// <summary>当前位置的结束索引</summary>
    public long EndIndex { get; set; } = -1;

    /// <summary>根据键名来读取写入当前的列表中的多个信息</summary>
    /// <param name="listKey">列表键名</param>
    public EstRedisListAttribute(string listKey) => this.ListKey = listKey;

    /// <summary>根据键名来读取写入当前的列表中的多个信息</summary>
    /// <param name="listKey">列表键名</param>
    /// <param name="startIndex">开始的索引信息</param>
    public EstRedisListAttribute(string listKey, long startIndex)
    {
      this.ListKey = listKey;
      this.StartIndex = startIndex;
    }

    /// <summary>根据键名来读取写入当前的列表中的多个信息</summary>
    /// <param name="listKey">列表键名</param>
    /// <param name="startIndex">开始的索引信息</param>
    /// <param name="endIndex">结束的索引位置，-1为倒数第一个，以此类推。</param>
    public EstRedisListAttribute(string listKey, long startIndex, long endIndex)
    {
      this.ListKey = listKey;
      this.StartIndex = startIndex;
      this.EndIndex = endIndex;
    }
  }
}
