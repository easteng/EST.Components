// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Core.SimpleHybirdLock
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using System;
using System.Threading;

namespace ESTCore.Common.Core
{
    /// <summary>
    /// 一个简单的混合线程同步锁，采用了基元用户加基元内核同步构造实现<br />
    /// A simple hybrid thread editing lock, implemented by the base user plus the element kernel synchronization.
    /// </summary>
    /// <remarks>当前的锁适用于，竞争频率比较低，锁部分的代码运行时间比较久的情况，当前的简单混合锁可以达到最大性能。</remarks>
    /// <example>
    /// 以下演示常用的锁的使用方式，还包含了如何优雅的处理异常锁
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Core\ThreadLock.cs" region="SimpleHybirdLockExample1" title="SimpleHybirdLock示例" />
    /// </example>
    public sealed class SimpleHybirdLock : IDisposable
    {
        private bool disposedValue = false;
        /// <summary>基元用户模式构造同步锁</summary>
        private int m_waiters = 0;
        /// <summary>基元内核模式构造同步锁</summary>
        private readonly Lazy<AutoResetEvent> m_waiterLock = new Lazy<AutoResetEvent>((Func<AutoResetEvent>)(() => new AutoResetEvent(false)));
        private static long simpleHybirdLockCount;
        private static long simpleHybirdLockWaitCount;

        private void Dispose(bool disposing)
        {
            if (this.disposedValue)
                return;
            if (!disposing)
                ;
            this.m_waiterLock.Value.Close();
            this.disposedValue = true;
        }

        /// <inheritdoc cref="M:System.IDisposable.Dispose" />
        public void Dispose() => this.Dispose(true);

        /// <summary>获取锁</summary>
        public void Enter()
        {
            Interlocked.Increment(ref SimpleHybirdLock.simpleHybirdLockCount);
            if (Interlocked.Increment(ref this.m_waiters) == 1)
                return;
            Interlocked.Increment(ref SimpleHybirdLock.simpleHybirdLockWaitCount);
            this.m_waiterLock.Value.WaitOne();
        }

        /// <summary>离开锁</summary>
        public void Leave()
        {
            Interlocked.Decrement(ref SimpleHybirdLock.simpleHybirdLockCount);
            if (Interlocked.Decrement(ref this.m_waiters) == 0)
                return;
            Interlocked.Decrement(ref SimpleHybirdLock.simpleHybirdLockWaitCount);
            this.m_waiterLock.Value.Set();
        }

        /// <summary>获取当前锁是否在等待当中</summary>
        public bool IsWaitting => (uint)this.m_waiters > 0U;

        /// <summary>
        /// 获取当前总的所有进入锁的信息<br />
        /// Get the current total information of all access locks
        /// </summary>
        public static long SimpleHybirdLockCount => SimpleHybirdLock.simpleHybirdLockCount;

        /// <summary>当前正在等待的锁的统计信息，此时已经发生了竞争了</summary>
        public static long SimpleHybirdLockWaitCount => SimpleHybirdLock.simpleHybirdLockWaitCount;
    }
}
