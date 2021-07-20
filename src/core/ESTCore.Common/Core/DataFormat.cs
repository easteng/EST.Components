// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Core.DataFormat
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

namespace ESTCore.Common.Core
{
    /// <summary>
    /// 应用于多字节数据的解析或是生成格式<br />
    /// Parsing or generating format for multibyte data
    /// </summary>
    public enum DataFormat
    {
        /// <summary>按照顺序排序</summary>
        ABCD,
        /// <summary>按照单字反转</summary>
        BADC,
        /// <summary>按照双字反转</summary>
        CDAB,
        /// <summary>按照倒序排序</summary>
        DCBA,
    }
}
