// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.AllenBradley.AllenBradleyItemValue
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

namespace ESTCore.Common.Profinet.AllenBradley
{
    /// <summary>AB PLC的数据</summary>
    public class AllenBradleyItemValue
    {
        /// <summary>真实的数组缓存</summary>
        public byte[] Buffer { get; set; }

        /// <summary>是否是数组的数据</summary>
        public bool IsArray { get; set; }

        /// <summary>单个单位的数据长度信息</summary>
        public int TypeLength { get; set; } = 1;
    }
}
