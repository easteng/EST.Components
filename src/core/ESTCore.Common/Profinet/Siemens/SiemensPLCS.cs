// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Siemens.SiemensPLCS
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

namespace ESTCore.Common.Profinet.Siemens
{
    /// <summary>西门子的PLC类型，目前支持的访问类型</summary>
    public enum SiemensPLCS
    {
        /// <summary>1200系列</summary>
        S1200 = 1,
        /// <summary>300系列</summary>
        S300 = 2,
        /// <summary>400系列</summary>
        S400 = 3,
        /// <summary>1500系列PLC</summary>
        S1500 = 4,
        /// <summary>200的smart系列</summary>
        S200Smart = 5,
        /// <summary>200系统，需要额外配置以太网模块</summary>
        S200 = 6,
    }
}
