// Decompiled with JetBrains decompiler
// Type: EstCommunication.BasicFramework.SoftIncrementCount
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.Core;
using System;

namespace EstCommunication.BasicFramework
{
  /// <summary>
  /// 一个简单的不持久化的序号自增类，采用线程安全实现，并允许指定最大数字，将包含该最大值，到达后清空从指定数开始<br />
  /// A simple non-persistent serial number auto-increment class, which is implemented with thread safety, and allows the maximum number to be specified, which will contain the maximum number, and will be cleared from the specified number upon arrival.
  /// </summary>
  /// <example>
  /// 先来看看一个简单的应用的
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftIncrementCountSample.cs" region="Sample1" title="简单示例" />
  /// 再来看看一些复杂的情况
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftIncrementCountSample.cs" region="Sample2" title="复杂示例" />
  /// 其他一些特殊的设定
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftIncrementCountSample.cs" region="Sample3" title="其他示例" />
  /// </example>
  public sealed class SoftIncrementCount : IDisposable
  {
    private long start = 0;
    private long current = 0;
    private long max = long.MaxValue;
    private SimpleHybirdLock hybirdLock;
    private bool disposedValue = false;

    /// <summary>
    /// 实例化一个自增信息的对象，包括最大值，初始值，增量值<br />
    /// Instantiate an object with incremental information, including the maximum value and initial value, IncreaseTick
    /// </summary>
    /// <param name="max">数据的最大值，必须指定</param>
    /// <param name="start">数据的起始值，默认为0</param>
    /// <param name="tick">每次的增量值</param>
    public SoftIncrementCount(long max, long start = 0, int tick = 1)
    {
      this.start = start;
      this.max = max;
      this.current = start;
      this.IncreaseTick = tick;
      this.hybirdLock = new SimpleHybirdLock();
    }

    /// <summary>
    /// 获取自增信息，获得数据之后，下一次获取将会自增，如果自增后大于最大值，则会重置为最小值，如果小于最小值，则会重置为最大值。<br />
    /// Get the auto-increment information. After getting the data, the next acquisition will auto-increase.
    /// If the auto-increment is greater than the maximum value, it will reset to the minimum value.
    /// If the auto-increment is smaller than the minimum value, it will reset to the maximum value.
    /// </summary>
    /// <returns>计数自增后的值</returns>
    public long GetCurrentValue()
    {
      this.hybirdLock.Enter();
      long current = this.current;
      this.current += (long) this.IncreaseTick;
      if (this.current > this.max)
        this.current = this.start;
      else if (this.current < this.start)
        this.current = this.max;
      this.hybirdLock.Leave();
      return current;
    }

    /// <summary>
    /// 重置当前序号的最大值，最大值应该大于初始值，如果当前值大于最大值，则当前值被重置为最大值<br />
    /// Reset the maximum value of the current serial number. The maximum value should be greater than the initial value.
    /// If the current value is greater than the maximum value, the current value is reset to the maximum value.
    /// </summary>
    /// <param name="max">最大值</param>
    public void ResetMaxValue(long max)
    {
      this.hybirdLock.Enter();
      if (max > this.start)
      {
        if (max < this.current)
          this.current = this.start;
        this.max = max;
      }
      this.hybirdLock.Leave();
    }

    /// <summary>
    /// 重置当前序号的初始值，需要小于最大值，如果当前值小于初始值，则当前值被重置为初始值。<br />
    /// To reset the initial value of the current serial number, it must be less than the maximum value.
    /// If the current value is less than the initial value, the current value is reset to the initial value.
    /// </summary>
    /// <param name="start">初始值</param>
    public void ResetStartValue(long start)
    {
      this.hybirdLock.Enter();
      if (start < this.max)
      {
        if (this.current < start)
          this.current = start;
        this.start = start;
      }
      this.hybirdLock.Leave();
    }

    /// <summary>
    /// 将当前的值重置为初始值。<br />
    /// Reset the current value to the initial value.
    /// </summary>
    public void ResetCurrentValue()
    {
      this.hybirdLock.Enter();
      this.current = this.start;
      this.hybirdLock.Leave();
    }

    /// <summary>
    /// 将当前的值重置为指定值，该值不能大于max，如果大于max值，就会自动设置为max<br />
    /// Reset the current value to the specified value. The value cannot be greater than max. If it is greater than max, it will be automatically set to max.
    /// </summary>
    /// <param name="value">指定的数据值</param>
    public void ResetCurrentValue(long value)
    {
      this.hybirdLock.Enter();
      this.current = value <= this.max ? (value >= this.start ? value : this.start) : this.max;
      this.hybirdLock.Leave();
    }

    /// <summary>
    /// 增加的单元，如果设置为0，就是不增加。如果为小于0，那就是减少，会变成负数的可能。<br />
    /// Increased units, if set to 0, do not increase. If it is less than 0, it is a decrease and it may become a negative number.
    /// </summary>
    public int IncreaseTick { get; set; } = 1;

    /// <summary>
    /// 获取当前的计数器的最大的设置值。<br />
    /// Get the maximum setting value of the current counter.
    /// </summary>
    public long MaxValue => this.max;

    /// <inheritdoc />
    public override string ToString() => string.Format("SoftIncrementCount[{0}]", (object) this.current);

    private void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      if (disposing)
        this.hybirdLock.Dispose();
      this.disposedValue = true;
    }

    /// <inheritdoc cref="M:System.IDisposable.Dispose" />
    public void Dispose() => this.Dispose(true);
  }
}
