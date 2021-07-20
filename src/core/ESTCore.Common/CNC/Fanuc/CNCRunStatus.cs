// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.CNC.Fanuc.CNCRunStatus
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

namespace ESTCore.Common.CNC.Fanuc
{
    /// <summary>CNC的运行状态</summary>
    public enum CNCRunStatus
    {
        /// <summary>重置</summary>
        RESET,
        /// <summary>停止</summary>
        STOP,
        /// <summary>等待</summary>
        HOLD,
        /// <summary>启动</summary>
        START,
        /// <summary>MSTR</summary>
        MSTR,
    }
}
