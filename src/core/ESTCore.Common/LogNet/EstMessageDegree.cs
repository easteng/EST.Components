// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.LogNet.EstMessageDegree
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

namespace ESTCore.Common.LogNet
{
    /// <summary>记录消息的等级</summary>
    public enum EstMessageDegree
    {
        /// <summary>一条消息都不记录</summary>
        None = 1,
        /// <summary>记录致命等级及以上日志的消息</summary>
        FATAL = 2,
        /// <summary>记录异常等级及以上日志的消息</summary>
        ERROR = 3,
        /// <summary>记录警告等级及以上日志的消息</summary>
        WARN = 4,
        /// <summary>记录信息等级及以上日志的消息</summary>
        INFO = 5,
        /// <summary>记录调试等级及以上日志的信息</summary>
        DEBUG = 6,
    }
}
