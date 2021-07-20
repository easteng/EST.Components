// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.LSIS.LSCpuStatus
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

namespace ESTCore.Common.Profinet.LSIS
{
    /// <summary>Cpu status</summary>
    public enum LSCpuStatus
    {
        /// <summary>运行中</summary>
        RUN = 1,
        /// <summary>运行停止</summary>
        STOP = 2,
        /// <summary>错误状态</summary>
        ERROR = 3,
        /// <summary>调试模式</summary>
        DEBUG = 4,
    }
}
