// Decompiled with JetBrains decompiler
// Type: EstCommunication.Core.AdvancedHybirdLock
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using System;
using System.Threading;

namespace EstCommunication.Core
{
  /// <summary>一个高级的混合线程同步锁，采用了基元用户加基元内核同步构造实现，并包含了自旋和线程所有权</summary>
  /// <remarks>当竞争的频率很高的时候，锁的时间很短的时候，当前的锁可以获得最大性能。</remarks>
  internal sealed class AdvancedHybirdLock : IDisposable
  {
    private bool disposedValue = false;
    private int m_waiters = 0;
    private readonly Lazy<AutoResetEvent> m_waiterLock = new Lazy<AutoResetEvent>((Func<AutoResetEvent>) (() => new AutoResetEvent(false)));
    private int m_spincount = 1000;
    private int m_owningThreadId = 0;
    private int m_recursion = 0;

    private void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      if (!disposing)
        ;
      this.m_waiterLock.Value.Close();
      this.disposedValue = true;
    }

    /// <summary>释放资源</summary>
    public void Dispose() => this.Dispose(true);

    /// <summary>
    /// 自旋锁的自旋周期，当竞争频率小，就要设置小，当竞争频率大，就要设置大，锁时间长就设置小，锁时间短就设置大，这样才能达到真正的高性能，默认为1000
    /// </summary>
    public int SpinCount
    {
      get => this.m_spincount;
      set => this.m_spincount = value;
    }

    /// <summary>获取锁</summary>
    public void Enter()
    {
      if (Thread.CurrentThread.ManagedThreadId == this.m_owningThreadId)
      {
        ++this.m_recursion;
      }
      else
      {
        SpinWait spinWait = new SpinWait();
        for (int index = 0; index < this.m_spincount; ++index)
        {
          if (Interlocked.CompareExchange(ref this.m_waiters, 1, 0) == 0)
          {
            this.m_owningThreadId = Thread.CurrentThread.ManagedThreadId;
            this.m_recursion = 1;
            return;
          }
          spinWait.SpinOnce();
        }
        if (Interlocked.Increment(ref this.m_waiters) > 1)
          this.m_waiterLock.Value.WaitOne();
        this.m_owningThreadId = Thread.CurrentThread.ManagedThreadId;
        this.m_recursion = 1;
      }
    }

    /// <summary>离开锁</summary>
    public void Leave()
    {
      if (Thread.CurrentThread.ManagedThreadId != this.m_owningThreadId)
        throw new SynchronizationLockException("Current Thread have not the owning thread.");
      if (--this.m_recursion > 0)
        return;
      this.m_owningThreadId = 0;
      if (Interlocked.Decrement(ref this.m_waiters) == 0)
        return;
      this.m_waiterLock.Value.Set();
    }
  }
}
