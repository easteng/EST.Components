// Decompiled with JetBrains decompiler
// Type: EstCommunication.Core.EstReadWriteLock
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace EstCommunication.Core
{
  /// <summary>一个高性能的读写锁，支持写锁定，读灵活，读时写锁定，写时读锁定</summary>
  public sealed class EstReadWriteLock : IDisposable
  {
    private const int c_lsStateStartBit = 0;
    private const int c_lsReadersReadingStartBit = 3;
    private const int c_lsReadersWaitingStartBit = 12;
    private const int c_lsWritersWaitingStartBit = 21;
    private const int c_lsStateMask = 7;
    private const int c_lsReadersReadingMask = 4088;
    private const int c_lsReadersWaitingMask = 2093056;
    private const int c_lsWritersWaitingMask = 1071644672;
    private const int c_lsAnyWaitingMask = 1073737728;
    private const int c_ls1ReaderReading = 8;
    private const int c_ls1ReaderWaiting = 4096;
    private const int c_ls1WriterWaiting = 2097152;
    private int m_LockState = 0;
    private Semaphore m_ReadersLock = new Semaphore(0, int.MaxValue);
    private Semaphore m_WritersLock = new Semaphore(0, int.MaxValue);
    private bool disposedValue = false;
    private bool m_exclusive;

    private static EstReadWriteLock.OneManyLockStates State(int ls) => (EstReadWriteLock.OneManyLockStates) (ls & 7);

    private static void SetState(ref int ls, EstReadWriteLock.OneManyLockStates newState) => ls = (int) ((EstReadWriteLock.OneManyLockStates) (ls & -8) | newState);

    private static int NumReadersReading(int ls) => (ls & 4088) >> 3;

    private static void AddReadersReading(ref int ls, int amount) => ls += 8 * amount;

    private static int NumReadersWaiting(int ls) => (ls & 2093056) >> 12;

    private static void AddReadersWaiting(ref int ls, int amount) => ls += 4096 * amount;

    private static int NumWritersWaiting(int ls) => (ls & 1071644672) >> 21;

    private static void AddWritersWaiting(ref int ls, int amount) => ls += 2097152 * amount;

    private static bool AnyWaiters(int ls) => (uint) (ls & 1073737728) > 0U;

    private static string DebugState(int ls) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "State={0}, RR={1}, RW={2}, WW={3}", (object) EstReadWriteLock.State(ls), (object) EstReadWriteLock.NumReadersReading(ls), (object) EstReadWriteLock.NumReadersWaiting(ls), (object) EstReadWriteLock.NumWritersWaiting(ls));

    /// <summary>返回本对象的描述字符串</summary>
    /// <returns>对象的描述字符串</returns>
    public override string ToString() => EstReadWriteLock.DebugState(this.m_LockState);

    private void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      if (!disposing)
        ;
      this.m_WritersLock.Close();
      this.m_WritersLock = (Semaphore) null;
      this.m_ReadersLock.Close();
      this.m_ReadersLock = (Semaphore) null;
      this.disposedValue = true;
    }

    /// <summary>释放资源</summary>
    public void Dispose() => this.Dispose(true);

    /// <summary>根据读写情况请求锁</summary>
    /// <param name="exclusive">True为写请求，False为读请求</param>
    public void Enter(bool exclusive)
    {
      if (exclusive)
      {
        while (EstReadWriteLock.WaitToWrite(ref this.m_LockState))
          this.m_WritersLock.WaitOne();
      }
      else
      {
        while (EstReadWriteLock.WaitToRead(ref this.m_LockState))
          this.m_ReadersLock.WaitOne();
      }
      this.m_exclusive = exclusive;
    }

    private static bool WaitToWrite(ref int target)
    {
      int num = target;
      int comparand;
      bool flag;
      do
      {
        comparand = num;
        int ls = comparand;
        flag = false;
        switch (EstReadWriteLock.State(ls))
        {
          case EstReadWriteLock.OneManyLockStates.Free:
          case EstReadWriteLock.OneManyLockStates.ReservedForWriter:
            EstReadWriteLock.SetState(ref ls, EstReadWriteLock.OneManyLockStates.OwnedByWriter);
            break;
          case EstReadWriteLock.OneManyLockStates.OwnedByWriter:
            EstReadWriteLock.AddWritersWaiting(ref ls, 1);
            flag = true;
            break;
          case EstReadWriteLock.OneManyLockStates.OwnedByReaders:
          case EstReadWriteLock.OneManyLockStates.OwnedByReadersAndWriterPending:
            EstReadWriteLock.SetState(ref ls, EstReadWriteLock.OneManyLockStates.OwnedByReadersAndWriterPending);
            EstReadWriteLock.AddWritersWaiting(ref ls, 1);
            flag = true;
            break;
          default:
            Debug.Assert(false, "Invalid Lock state");
            break;
        }
        num = Interlocked.CompareExchange(ref target, ls, comparand);
      }
      while (comparand != num);
      return flag;
    }

    /// <summary>释放锁，将根据锁状态自动区分读写锁</summary>
    public void Leave()
    {
      int releaseCount;
      if (this.m_exclusive)
      {
        Debug.Assert(EstReadWriteLock.State(this.m_LockState) == EstReadWriteLock.OneManyLockStates.OwnedByWriter && EstReadWriteLock.NumReadersReading(this.m_LockState) == 0);
        releaseCount = EstReadWriteLock.DoneWriting(ref this.m_LockState);
      }
      else
      {
        EstReadWriteLock.State(this.m_LockState);
        Debug.Assert(EstReadWriteLock.State(this.m_LockState) == EstReadWriteLock.OneManyLockStates.OwnedByReaders || EstReadWriteLock.State(this.m_LockState) == EstReadWriteLock.OneManyLockStates.OwnedByReadersAndWriterPending);
        releaseCount = EstReadWriteLock.DoneReading(ref this.m_LockState);
      }
      if (releaseCount == -1)
      {
        this.m_WritersLock.Release();
      }
      else
      {
        if (releaseCount <= 0)
          return;
        this.m_ReadersLock.Release(releaseCount);
      }
    }

    private static int DoneWriting(ref int target)
    {
      int num1 = target;
      int comparand;
      int num2;
      do
      {
        int ls = comparand = num1;
        if (!EstReadWriteLock.AnyWaiters(ls))
        {
          EstReadWriteLock.SetState(ref ls, EstReadWriteLock.OneManyLockStates.Free);
          num2 = 0;
        }
        else if (EstReadWriteLock.NumWritersWaiting(ls) > 0)
        {
          EstReadWriteLock.SetState(ref ls, EstReadWriteLock.OneManyLockStates.ReservedForWriter);
          EstReadWriteLock.AddWritersWaiting(ref ls, -1);
          num2 = -1;
        }
        else
        {
          num2 = EstReadWriteLock.NumReadersWaiting(ls);
          Debug.Assert(num2 > 0);
          EstReadWriteLock.SetState(ref ls, EstReadWriteLock.OneManyLockStates.OwnedByReaders);
          EstReadWriteLock.AddReadersWaiting(ref ls, -num2);
        }
        num1 = Interlocked.CompareExchange(ref target, ls, comparand);
      }
      while (comparand != num1);
      return num2;
    }

    private static bool WaitToRead(ref int target)
    {
      int num = target;
      int comparand;
      bool flag;
      do
      {
        int ls = comparand = num;
        flag = false;
        switch (EstReadWriteLock.State(ls))
        {
          case EstReadWriteLock.OneManyLockStates.Free:
            EstReadWriteLock.SetState(ref ls, EstReadWriteLock.OneManyLockStates.OwnedByReaders);
            EstReadWriteLock.AddReadersReading(ref ls, 1);
            break;
          case EstReadWriteLock.OneManyLockStates.OwnedByWriter:
          case EstReadWriteLock.OneManyLockStates.OwnedByReadersAndWriterPending:
          case EstReadWriteLock.OneManyLockStates.ReservedForWriter:
            EstReadWriteLock.AddReadersWaiting(ref ls, 1);
            flag = true;
            break;
          case EstReadWriteLock.OneManyLockStates.OwnedByReaders:
            EstReadWriteLock.AddReadersReading(ref ls, 1);
            break;
          default:
            Debug.Assert(false, "Invalid Lock state");
            break;
        }
        num = Interlocked.CompareExchange(ref target, ls, comparand);
      }
      while (comparand != num);
      return flag;
    }

    private static int DoneReading(ref int target)
    {
      int num1 = target;
      int comparand;
      int num2;
      do
      {
        int ls = comparand = num1;
        EstReadWriteLock.AddReadersReading(ref ls, -1);
        if (EstReadWriteLock.NumReadersReading(ls) > 0)
          num2 = 0;
        else if (!EstReadWriteLock.AnyWaiters(ls))
        {
          EstReadWriteLock.SetState(ref ls, EstReadWriteLock.OneManyLockStates.Free);
          num2 = 0;
        }
        else
        {
          Debug.Assert(EstReadWriteLock.NumWritersWaiting(ls) > 0);
          EstReadWriteLock.SetState(ref ls, EstReadWriteLock.OneManyLockStates.ReservedForWriter);
          EstReadWriteLock.AddWritersWaiting(ref ls, -1);
          num2 = -1;
        }
        num1 = Interlocked.CompareExchange(ref target, ls, comparand);
      }
      while (comparand != num1);
      return num2;
    }

    private enum OneManyLockStates
    {
      Free,
      OwnedByWriter,
      OwnedByReaders,
      OwnedByReadersAndWriterPending,
      ReservedForWriter,
    }
  }
}
