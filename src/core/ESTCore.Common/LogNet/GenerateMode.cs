// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.LogNet.GenerateMode
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

namespace ESTCore.Common.LogNet
{
    /// <summary>日志文件输出模式</summary>
    public enum GenerateMode
    {
        /// <summary>按每分钟生成日志文件</summary>
        ByEveryMinute = 1,
        /// <summary>按每个小时生成日志文件</summary>
        ByEveryHour = 2,
        /// <summary>按每天生成日志文件</summary>
        ByEveryDay = 3,
        /// <summary>按每个周生成日志文件</summary>
        ByEveryWeek = 4,
        /// <summary>按每个月生成日志文件</summary>
        ByEveryMonth = 5,
        /// <summary>按每季度生成日志文件</summary>
        ByEverySeason = 6,
        /// <summary>按每年生成日志文件</summary>
        ByEveryYear = 7,
    }
}
