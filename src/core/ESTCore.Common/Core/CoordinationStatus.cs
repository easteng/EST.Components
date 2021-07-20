// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Core.CoordinationStatus
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

namespace ESTCore.Common.Core
{
    /// <summary>线程的协调逻辑状态</summary>
    internal enum CoordinationStatus
    {
        /// <summary>所有项完成</summary>
        AllDone,
        /// <summary>超时</summary>
        Timeout,
        /// <summary>任务取消</summary>
        Cancel,
    }
}
