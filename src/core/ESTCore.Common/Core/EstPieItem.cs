// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Core.EstPieItem
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using System.Drawing;

namespace ESTCore.Common.Core
{
    /// <summary>饼图的基本元素</summary>
    public class EstPieItem
    {
        /// <summary>实例化一个饼图基本元素的对象</summary>
        public EstPieItem() => this.Back = Color.DodgerBlue;

        /// <summary>名称</summary>
        public string Name { get; set; }

        /// <summary>值</summary>
        public int Value { get; set; }

        /// <summary>背景颜色</summary>
        public Color Back { get; set; }
    }
}
