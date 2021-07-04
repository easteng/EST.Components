// Decompiled with JetBrains decompiler
// Type: EstCommunication.Core.ValueLimit
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

namespace EstCommunication.Core
{
  /// <summary>
  /// 一个数据范围管理的的类型对象，可以方便的管理一个数据值的最大值，最小值，平均值信息。<br />
  /// A type object managed by a data range can conveniently manage the maximum, minimum, and average information of a data value.
  /// </summary>
  public struct ValueLimit
  {
    /// <summary>
    /// 当前的数据的最大值<br />
    /// The maximum value of the current data
    /// </summary>
    public double MaxValue { get; set; }

    /// <summary>
    /// 当前的数据的最小值<br />
    /// The minimum value of the current data
    /// </summary>
    public double MinValue { get; set; }

    /// <summary>
    /// 当前的数据的平均值<br />
    /// Average value of current data
    /// </summary>
    public double Average { get; set; }

    /// <summary>
    /// 当前的数据的起始值<br />
    /// The starting value of the current data
    /// </summary>
    public double StartValue { get; set; }

    /// <summary>
    /// 当前的数据的当前值，也是最后一次更新的数据值<br />
    /// The current value of the current data is also the last updated data value
    /// </summary>
    public double Current { get; set; }

    /// <summary>
    /// 当前的数据的更新总个数<br />
    /// The total number of current data updates
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// 重新设置当前的最新值，然后计算新的最大值，最小值，平均值等信息<br />
    /// Reset the current latest value, and then calculate the new maximum, minimum, average and other information
    /// </summary>
    /// <param name="value">新的数据值信息</param>
    /// <returns>新的值对象</returns>
    public ValueLimit SetNewValue(double value)
    {
      if (!double.IsNaN(value))
      {
        if (this.Count == 0)
        {
          this.MaxValue = value;
          this.MinValue = value;
          this.Count = 1;
          this.Current = value;
          this.Average = value;
          this.StartValue = value;
        }
        else
        {
          if (value < this.MinValue)
            this.MinValue = value;
          if (value > this.MaxValue)
            this.MaxValue = value;
          this.Current = value;
          this.Average = ((double) this.Count * this.Average + value) / (double) (this.Count + 1);
          ++this.Count;
        }
      }
      return this;
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("Avg[{0}]", (object) this.Current);

    /// <summary>判断是否相等</summary>
    /// <param name="value1">第一个数据值</param>
    /// <param name="value2">第二个数据值</param>
    /// <returns>是否相同</returns>
    public static bool operator ==(ValueLimit value1, ValueLimit value2) => value1.Count == value2.Count && value1.MaxValue == value2.MaxValue && (value1.MinValue == value2.MinValue && value1.Current == value2.Current) && (value1.Average == value2.Average && value1.StartValue == value2.StartValue);

    /// <summary>判断是否相等</summary>
    /// <param name="value1">第一个数据值</param>
    /// <param name="value2">第二个数据值</param>
    /// <returns>是否相同</returns>
    public static bool operator !=(ValueLimit value1, ValueLimit value2) => !(value1 == value2);

    /// <inheritdoc />
    public override bool Equals(object obj) => obj is ValueLimit valueLimit && this == valueLimit;

    /// <inheritdoc />
    public override int GetHashCode() => base.GetHashCode();
  }
}
