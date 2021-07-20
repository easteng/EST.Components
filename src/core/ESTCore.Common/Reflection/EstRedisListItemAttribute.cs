// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Reflection.EstRedisListItemAttribute
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using System;

namespace ESTCore.Common.Reflection
{
    /// <summary>对应redis的一个列表信息的内容</summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class EstRedisListItemAttribute : Attribute
    {
        /// <summary>列表键值的名称</summary>
        public string ListKey { get; set; }

        /// <summary>当前的位置的索引</summary>
        public long Index { get; set; }

        /// <summary>根据键名来读取写入当前的列表中的单个信息</summary>
        /// <param name="listKey">列表键名</param>
        /// <param name="index">当前的索引位置</param>
        public EstRedisListItemAttribute(string listKey, long index)
        {
            this.ListKey = listKey;
            this.Index = index;
        }
    }
}
