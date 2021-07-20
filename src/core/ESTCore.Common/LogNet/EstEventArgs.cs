// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.LogNet.EstEventArgs
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using System;

namespace ESTCore.Common.LogNet
{
    /// <summary>带有日志消息的事件</summary>
    public class EstEventArgs : EventArgs
    {
        /// <summary>消息信息</summary>
        public EstMessageItem EstMessage { get; set; }
    }
}
