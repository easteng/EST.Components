// Decompiled with JetBrains decompiler
// Type: EstCommunication.LogNet.LogStatisticsBase`1
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.Reflection;
using System;

namespace EstCommunication.LogNet
{
  /// <summary>
  /// 一个按照实际进行数据分割的辅助基类，可以用于实现对某个的API按照每天进行调用次数统计，也可以实现对某个设备数据按照天进行最大最小值均值分析，这些都是需要继承实现。<br />
  /// An auxiliary base class that divides the data according to the actual data can be used to implement statistics on the number of calls per day for a certain API, and it can also implement the maximum and minimum average analysis of a certain device data according to the day. These all need to be inherited. .
  /// </summary>
  /// <typeparam name="T">统计的数据类型</typeparam>
  public class LogStatisticsBase<T>
  {
    private T[] statistics = (T[]) null;
    /// <summary>当前的实际模式</summary>
    protected GenerateMode generateMode = GenerateMode.ByEveryDay;
    private int arrayLength = 30;
    private long lastDataMark = -1;
    private object lockStatistics;

    /// <summary>
    /// 实例化一个新的数据统计内容，需要指定当前的时间统计方式，按小时，按天，按月等等，还需要指定统计的数据数量，比如按天统计30天。<br />
    /// To instantiate a new data statistics content, you need to specify the current time statistics method, by hour, by day, by month, etc.,
    /// and also need to specify the number of statistics, such as 30 days by day.
    /// </summary>
    /// <param name="generateMode">时间的统计方式</param>
    /// <param name="arrayLength">数据的数量信息</param>
    public LogStatisticsBase(GenerateMode generateMode, int arrayLength)
    {
      this.generateMode = generateMode;
      this.arrayLength = arrayLength;
      this.statistics = new T[arrayLength];
      this.lastDataMark = this.GetDataMarkFromDateTime(DateTime.Now);
      this.lockStatistics = new object();
    }

    /// <summary>
    /// 获取当前的统计类信息时间统计规则<br />
    /// Get the current statistical information time statistics rule
    /// </summary>
    public GenerateMode GenerateMode => this.generateMode;

    /// <summary>
    /// 获取当前的统计类信息的数据总量<br />
    /// Get the total amount of current statistical information
    /// </summary>
    public int ArrayLength => this.arrayLength;

    /// <summary>
    /// 重置当前的统计信息，需要指定统计的数据内容，最后一个数据的标记信息，本方法主要用于还原统计信息<br />
    /// To reset the current statistical information, you need to specify the content of the statistical data,
    /// and the tag information of the last data. This method is mainly used to restore statistical information
    /// </summary>
    /// <param name="statistics">统计结果数据信息</param>
    /// <param name="lastDataMark">最后一次标记的内容</param>
    public void Reset(T[] statistics, long lastDataMark)
    {
      if (statistics.Length > this.arrayLength)
        Array.Copy((Array) statistics, statistics.Length - this.arrayLength, (Array) this.statistics, 0, this.arrayLength);
      else if (statistics.Length < this.arrayLength)
        Array.Copy((Array) statistics, 0, (Array) this.statistics, this.arrayLength - statistics.Length, statistics.Length);
      else
        this.statistics = statistics;
      this.arrayLength = statistics.Length;
      this.lastDataMark = lastDataMark;
    }

    /// <summary>
    /// 新增一个统计信息，将会根据当前的时间来决定插入数据位置，如果数据位置发生了变化，则数据向左发送移动。如果没有移动或是移动完成后，最后一个数进行自定义的数据操作<br />
    /// Adding a new statistical information will determine the position to insert the data according to the current time. If the data position changes,
    /// the data will be sent to the left. If there is no movement or after the movement is completed, Custom data operations on the last number
    /// </summary>
    /// <param name="newValue">增对最后一个数的自定义操作</param>
    protected void StatisticsCustomAction(Func<T, T> newValue)
    {
      lock (this.lockStatistics)
      {
        long markFromDateTime = this.GetDataMarkFromDateTime(DateTime.Now);
        if (this.lastDataMark != markFromDateTime)
        {
          this.statistics = this.GetLeftMoveTimes((int) (markFromDateTime - this.lastDataMark));
          this.lastDataMark = markFromDateTime;
        }
        this.statistics[this.statistics.Length - 1] = newValue(this.statistics[this.statistics.Length - 1]);
      }
    }

    /// <summary>
    /// 新增一个统计信息，将会根据当前的时间来决定插入数据位置，如果数据位置发生了变化，则数据向左发送移动。如果没有移动或是移动完成后，最后一个数进行自定义的数据操作<br />
    /// Adding a new statistical information will determine the position to insert the data according to the current time. If the data position changes,
    /// the data will be sent to the left. If there is no movement or after the movement is completed, Custom data operations on the last number
    /// </summary>
    /// <param name="newValue">增对最后一个数的自定义操作</param>
    /// <param name="time">增加的时间信息</param>
    protected void StatisticsCustomAction(Func<T, T> newValue, DateTime time)
    {
      lock (this.lockStatistics)
      {
        long markFromDateTime1 = this.GetDataMarkFromDateTime(DateTime.Now);
        if (this.lastDataMark != markFromDateTime1)
        {
          this.statistics = this.GetLeftMoveTimes((int) (markFromDateTime1 - this.lastDataMark));
          this.lastDataMark = markFromDateTime1;
        }
        long markFromDateTime2 = this.GetDataMarkFromDateTime(time);
        if (markFromDateTime2 > markFromDateTime1)
          return;
        int index = (int) (markFromDateTime2 - (markFromDateTime1 - (long) this.statistics.Length + 1L));
        if (index >= 0 && index < this.statistics.Length)
          this.statistics[index] = newValue(this.statistics[index]);
      }
    }

    /// <summary>
    /// 获取当前的统计信息的数据快照，这是数据的副本，修改了里面的值不影响<br />
    /// Get a data snapshot of the current statistics. This is a copy of the data. Modifying the value inside does not affect
    /// </summary>
    /// <returns>实际的统计数据信息</returns>
    [EstMqttApi(Description = "Get a data snapshot of the current statistics")]
    public T[] GetStatisticsSnapshot() => this.GetStatisticsSnapAndDataMark().Content2;

    /// <summary>
    /// 根据指定的时间范围来获取统计的数据信息快照，包含起始时间，包含结束时间，这是数据的副本，修改了里面的值不影响<br />
    /// Get a snapshot of statistical data information according to the specified time range, including the start time,
    /// also the end time. This is a copy of the data. Modifying the value inside does not affect
    /// </summary>
    /// <param name="start">起始时间</param>
    /// <param name="finish">结束时间</param>
    /// <returns>指定实际范围内的数据副本</returns>
    [EstMqttApi(Description = "Get a snapshot of statistical data information according to the specified time range")]
    public T[] GetStatisticsSnapshotByTime(DateTime start, DateTime finish)
    {
      if (finish <= start)
        return new T[0];
      lock (this.lockStatistics)
      {
        long markFromDateTime = this.GetDataMarkFromDateTime(DateTime.Now);
        if (this.lastDataMark != markFromDateTime)
        {
          this.statistics = this.GetLeftMoveTimes((int) (markFromDateTime - this.lastDataMark));
          this.lastDataMark = markFromDateTime;
        }
        long num1 = markFromDateTime - (long) this.statistics.Length + 1L;
        long num2 = this.GetDataMarkFromDateTime(start);
        long num3 = this.GetDataMarkFromDateTime(finish);
        if (num2 < num1)
          num2 = num1;
        if (num3 > markFromDateTime)
          num3 = markFromDateTime;
        int index1 = (int) (num2 - num1);
        int length = (int) (num3 - num2 + 1L);
        if (num2 == num3)
          return new T[1]{ this.statistics[index1] };
        T[] objArray = new T[length];
        for (int index2 = 0; index2 < length; ++index2)
          objArray[index2] = this.statistics[index1 + index2];
        return objArray;
      }
    }

    /// <summary>
    /// 获取当前的统计信息的数据快照，这是数据的副本，修改了里面的值不影响<br />
    /// Get a data snapshot of the current statistics. This is a copy of the data. Modifying the value inside does not affect
    /// </summary>
    /// <returns>实际的统计数据信息</returns>
    public OperateResult<long, T[]> GetStatisticsSnapAndDataMark()
    {
      lock (this.lockStatistics)
      {
        long markFromDateTime = this.GetDataMarkFromDateTime(DateTime.Now);
        if (this.lastDataMark != markFromDateTime)
        {
          this.statistics = this.GetLeftMoveTimes((int) (markFromDateTime - this.lastDataMark));
          this.lastDataMark = markFromDateTime;
        }
        return OperateResult.CreateSuccessResult<long, T[]>(markFromDateTime, this.statistics.CopyArray<T>());
      }
    }

    /// <summary>
    /// 根据当前数据统计的时间模式，获取最新的数据标记信息<br />
    /// Obtain the latest data mark information according to the time mode of current data statistics
    /// </summary>
    /// <returns>数据标记</returns>
    [EstMqttApi(Description = "Obtain the latest data mark information according to the time mode of current data statistics")]
    public long GetDataMarkFromTimeNow() => this.GetDataMarkFromDateTime(DateTime.Now);

    /// <summary>
    /// 根据指定的时间，获取到该时间指定的数据标记信息<br />
    /// According to the specified time, get the data mark information specified at that time
    /// </summary>
    /// <param name="dateTime">指定的时间</param>
    /// <returns>数据标记</returns>
    [EstMqttApi(Description = "According to the specified time, get the data mark information specified at that time")]
    public long GetDataMarkFromDateTime(DateTime dateTime)
    {
      switch (this.generateMode)
      {
        case GenerateMode.ByEveryMinute:
          return this.GetMinuteFromTime(dateTime);
        case GenerateMode.ByEveryHour:
          return this.GetHourFromTime(dateTime);
        case GenerateMode.ByEveryDay:
          return this.GetDayFromTime(dateTime);
        case GenerateMode.ByEveryWeek:
          return this.GetWeekFromTime(dateTime);
        case GenerateMode.ByEveryMonth:
          return this.GetMonthFromTime(dateTime);
        case GenerateMode.ByEverySeason:
          return this.GetSeasonFromTime(dateTime);
        case GenerateMode.ByEveryYear:
          return this.GetYearFromTime(dateTime);
        default:
          return this.GetDayFromTime(dateTime);
      }
    }

    private long GetMinuteFromTime(DateTime dateTime) => (long) (dateTime.Date - new DateTime(1970, 1, 1)).Days * 24L * 60L + (long) (dateTime.Hour * 60) + (long) dateTime.Minute;

    private long GetHourFromTime(DateTime dateTime) => (long) (dateTime.Date - new DateTime(1970, 1, 1)).Days * 24L + (long) dateTime.Hour;

    private long GetDayFromTime(DateTime dateTime) => (long) (dateTime.Date - new DateTime(1970, 1, 1)).Days;

    private long GetWeekFromTime(DateTime dateTime) => ((long) (dateTime.Date - new DateTime(1970, 1, 1)).Days + 3L) / 7L;

    private long GetMonthFromTime(DateTime dateTime) => (long) (dateTime.Year - 1970) * 12L + (long) (dateTime.Month - 1);

    private long GetSeasonFromTime(DateTime dateTime) => (long) (dateTime.Year - 1970) * 4L + (long) ((dateTime.Month - 1) / 3);

    private long GetYearFromTime(DateTime dateTime) => (long) (dateTime.Year - 1970);

    private T[] GetLeftMoveTimes(int times)
    {
      if (times >= this.statistics.Length)
        return new T[this.arrayLength];
      T[] objArray = new T[this.arrayLength];
      Array.Copy((Array) this.statistics, times, (Array) objArray, 0, this.statistics.Length - times);
      return objArray;
    }
  }
}
