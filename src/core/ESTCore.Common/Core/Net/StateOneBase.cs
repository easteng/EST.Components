// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Core.Net.StateOneBase
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using System.Threading;

namespace ESTCore.Common.Core.Net
{
    /// <summary>异步消息的对象</summary>
    internal class StateOneBase
    {
        /// <summary>本次接收或是发送的数据长度</summary>
        public int DataLength { get; set; } = 32;

        /// <summary>已经处理的字节长度</summary>
        public int AlreadyDealLength { get; set; }

        /// <summary>操作完成的信号</summary>
        public ManualResetEvent WaitDone { get; set; }

        /// <summary>缓存器</summary>
        public byte[] Buffer { get; set; }

        /// <summary>是否发生了错误</summary>
        public bool IsError { get; set; }

        /// <summary>错误消息</summary>
        public string ErrerMsg { get; set; }
    }
}
