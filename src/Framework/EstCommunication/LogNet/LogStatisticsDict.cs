// Decompiled with JetBrains decompiler
// Type: EstCommunication.LogNet.LogStatisticsDict
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.Core;
using EstCommunication.Reflection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EstCommunication.LogNet
{
  /// <summary>
  /// <seealso cref="T:EstCommunication.LogNet.LogStatistics" />的词典集合类，用于多个数据的统计信息，例如可以统计多个规格的产量信息，统计多个方法的调用次数信息<br />
  /// The dictionary collection class of <seealso cref="T:EstCommunication.LogNet.LogStatistics" /> is used for the statistical information of multiple data, for example,
  /// the output information of multiple specifications can be counted, and the number of calls of multiple methods can be counted.
  /// </summary>
  public class LogStatisticsDict
  {
    private GenerateMode generateMode = GenerateMode.ByEveryDay;
    private int arrayLength = 30;
    private Dictionary<string, LogStatistics> dict;
    private object dictLock;
    private LogStatistics logStat;

    /// <summary>
    /// 根据指定的存储模式，数据个数来实例化一个对象<br />
    /// According to the specified storage mode, the number of data to instantiate an object
    /// </summary>
    /// <param name="generateMode">当前的数据存储模式</param>
    /// <param name="arrayLength">准备存储的数据总个数</param>
    public LogStatisticsDict(GenerateMode generateMode, int arrayLength)
    {
      this.generateMode = generateMode;
      this.arrayLength = arrayLength;
      this.dictLock = new object();
      this.dict = new Dictionary<string, LogStatistics>(128);
      this.logStat = new LogStatistics(generateMode, arrayLength);
    }

    /// <summary>
    /// 根据给定的关键字信息，获取相关的 <seealso cref="T:EstCommunication.LogNet.LogStatistics" /> 对象，进而执行很多的操作<br />
    /// According to the given keyword information, obtain related <seealso cref="T:EstCommunication.LogNet.LogStatistics" /> objects, and then perform many operations
    /// </summary>
    /// <param name="key">关键字</param>
    /// <returns>日志对象，如果当前的日志对象不存在，就返回为NULL</returns>
    public LogStatistics GetLogStatistics(string key)
    {
      lock (this.dictLock)
        return this.dict.ContainsKey(key) ? this.dict[key] : (LogStatistics) null;
    }

    /// <summary>
    /// 手动新增一个<seealso cref="T:EstCommunication.LogNet.LogStatistics" />对象，需要指定相关的关键字<br />
    /// Manually add a <seealso cref="T:EstCommunication.LogNet.LogStatistics" /> object, you need to specify related keywords
    /// </summary>
    /// <param name="key">关键字信息</param>
    /// <param name="logStatistics">日志统计对象</param>
    public void AddLogStatistics(string key, LogStatistics logStatistics)
    {
      lock (this.dictLock)
      {
        if (this.dict.ContainsKey(key))
          this.dict[key] = logStatistics;
        else
          this.dict.Add(key, logStatistics);
      }
    }

    /// <summary>
    /// 移除一个<seealso cref="T:EstCommunication.LogNet.LogStatistics" />对象，需要指定相关的关键字，如果关键字本来就存在，返回 <c>True</c>, 如果不存在，返回 <c>False</c> <br />
    /// To remove a <seealso cref="T:EstCommunication.LogNet.LogStatistics" /> object, you need to specify the relevant keyword. If the keyword already exists, return <c>True</c>, if it does not exist, return <c>False</c>
    /// </summary>
    /// <param name="key">关键字信息</param>
    /// <returns>如果关键字本来就存在，返回 <c>True</c>, 如果不存在，返回 <c>False</c> </returns>
    public bool RemoveLogStatistics(string key)
    {
      lock (this.dictLock)
      {
        if (!this.dict.ContainsKey(key))
          return false;
        this.dict.Remove(key);
        return true;
      }
    }

    /// <summary>
    /// 新增一个统计信息，将会根据当前的时间来决定插入数据位置，如果数据位置发生了变化，则数据向左发送移动。如果没有移动或是移动完成后，最后一个数进行新增数据 frequency 次<br />
    /// Adding a new statistical information will determine the position to insert the data according to the current time. If the data position changes,
    /// the data will be sent to the left. If there is no movement or after the movement is completed, add data to the last number frequency times
    /// </summary>
    /// <param name="key">当前选择的关键字</param>
    /// <param name="frequency">新增的次数信息，默认为1</param>
    [EstMqttApi(Description = "Adding a new statistical information will determine the position to insert the data according to the current time")]
    public void StatisticsAdd(string key, long frequency = 1)
    {
      this.logStat.StatisticsAdd(frequency);
      LogStatistics logStatistics = this.GetLogStatistics(key);
      if (logStatistics == null)
      {
        lock (this.dictLock)
        {
          if (!this.dict.ContainsKey(key))
          {
            logStatistics = new LogStatistics(this.generateMode, this.arrayLength);
            this.dict.Add(key, logStatistics);
          }
          else
            logStatistics = this.dict[key];
        }
      }
      logStatistics?.StatisticsAdd(frequency);
    }

    /// <summary>
    /// 新增一个统计信息，将会根据当前的时间来决定插入数据位置，如果数据位置发生了变化，则数据向左发送移动。如果没有移动或是移动完成后，最后一个数进行新增数据 frequency 次<br />
    /// Adding a new statistical information will determine the position to insert the data according to the current time. If the data position changes,
    /// the data will be sent to the left. If there is no movement or after the movement is completed, add data to the last number frequency times
    /// </summary>
    /// <param name="key">当前的关键字</param>
    /// <param name="frequency">新增的次数信息</param>
    /// <param name="time">新增的次数的时间</param>
    [EstMqttApi(Description = "Adding a new statistical information will determine the position to insert the data according to the specified time")]
    public void StatisticsAddByTime(string key, long frequency, DateTime time)
    {
      this.logStat.StatisticsAddByTime(frequency, time);
      LogStatistics logStatistics = this.GetLogStatistics(key);
      if (logStatistics == null)
      {
        lock (this.dictLock)
        {
          if (!this.dict.ContainsKey(key))
          {
            logStatistics = new LogStatistics(this.generateMode, this.arrayLength);
            this.dict.Add(key, logStatistics);
          }
          else
            logStatistics = this.dict[key];
        }
      }
      logStatistics?.StatisticsAddByTime(frequency, time);
    }

    /// <summary>
    /// 获取当前的统计信息的数据快照，这是数据的副本，修改了里面的值不影响<br />
    /// Get a data snapshot of the current statistics. This is a copy of the data. Modifying the value inside does not affect
    /// </summary>
    /// <param name="key">当前的关键字的信息</param>
    /// <returns>实际的统计数据信息</returns>
    [EstMqttApi(Description = "Get a data snapshot of the current statistics")]
    public long[] GetStatisticsSnapshot(string key) => this.GetLogStatistics(key)?.GetStatisticsSnapshot();

    /// <summary>
    /// 根据指定的时间范围来获取统计的数据信息快照，包含起始时间，包含结束时间，这是数据的副本，修改了里面的值不影响<br />
    /// Get a snapshot of statistical data information according to the specified time range, including the start time,
    /// also the end time. This is a copy of the data. Modifying the value inside does not affect
    /// </summary>
    /// <param name="key">当前的关键字信息</param>
    /// <param name="start">起始时间</param>
    /// <param name="finish">结束时间</param>
    /// <returns>指定实际范围内的数据副本</returns>
    [EstMqttApi(Description = "Get a snapshot of statistical data information according to the specified time range")]
    public long[] GetStatisticsSnapshotByTime(string key, DateTime start, DateTime finish) => this.GetLogStatistics(key)?.GetStatisticsSnapshotByTime(start, finish);

    /// <summary>
    /// 获取所有的关键字的数据信息<br />
    /// Get data information of all keywords
    /// </summary>
    /// <returns>字符串数组</returns>
    [EstMqttApi(Description = "Get data information of all keywords")]
    public string[] GetKeys()
    {
      lock (this.dictLock)
        return this.dict.Keys.ToArray<string>();
    }

    /// <summary>
    /// 将当前的统计信息及数据内容写入到指定的文件里面，需要指定文件的路径名称<br />
    /// Write the current statistical information and data content to the specified file, you need to specify the path name of the file
    /// </summary>
    /// <param name="fileName">文件的完整的路径名称</param>
    public void SaveToFile(string fileName)
    {
      using (FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
      {
        byte[] buffer = new byte[1024];
        BitConverter.GetBytes(305419906).CopyTo((Array) buffer, 0);
        BitConverter.GetBytes((ushort) buffer.Length).CopyTo((Array) buffer, 4);
        BitConverter.GetBytes((ushort) this.GenerateMode).CopyTo((Array) buffer, 6);
        string[] keys = this.GetKeys();
        BitConverter.GetBytes(keys.Length).CopyTo((Array) buffer, 8);
        fileStream.Write(buffer, 0, buffer.Length);
        foreach (string key in keys)
        {
          LogStatistics logStatistics = this.GetLogStatistics(key);
          if (logStatistics != null)
          {
            EstHelper.WriteStringToStream((Stream) fileStream, key);
            EstHelper.WriteBinaryToStream((Stream) fileStream, logStatistics.SaveToBinary());
          }
        }
      }
    }

    /// <summary>
    /// 从指定的文件加载对应的统计信息，通常是调用<see cref="M:EstCommunication.LogNet.LogStatisticsDict.SaveToFile(System.String)" />方法存储的文件，如果文件不存在，将会跳过加载<br />
    /// Load the corresponding statistical information from the specified file, usually the file stored by calling the <see cref="M:EstCommunication.LogNet.LogStatisticsDict.SaveToFile(System.String)" /> method.
    /// If the file does not exist, the loading will be skipped
    /// </summary>
    /// <param name="fileName">文件的完整的路径名称</param>
    /// <exception cref="T:System.Exception">当文件的模式和当前的模式设置不一样的时候，会引发异常</exception>
    public void LoadFromFile(string fileName)
    {
      if (!File.Exists(fileName))
        return;
      using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
      {
        byte[] numArray = EstHelper.ReadSpecifiedLengthFromStream((Stream) fileStream, 1024);
        this.generateMode = (GenerateMode) BitConverter.ToUInt16(numArray, 6);
        int int32 = BitConverter.ToInt32(numArray, 8);
        for (int index = 0; index < int32; ++index)
        {
          string key = EstHelper.ReadStringFromStream((Stream) fileStream);
          byte[] buffer = EstHelper.ReadBinaryFromStream((Stream) fileStream);
          LogStatistics logStatistics = new LogStatistics(this.generateMode, this.arrayLength);
          logStatistics.LoadFromBinary(buffer);
          this.AddLogStatistics(key, logStatistics);
        }
      }
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
    [EstMqttApi(Description = "Get the total amount of current statistical information", HttpMethod = "GET")]
    public int ArrayLength => this.arrayLength;

    /// <summary>
    /// 获取当前词典类自身的日志统计对象，统计所有的元素的统计信息<br />
    /// Get the log statistics object of the current dictionary class itself, and count the statistics of all elements
    /// </summary>
    [EstMqttApi(Description = "Get the log statistics object of the current dictionary class itself, and count the statistics of all elements", PropertyUnfold = true)]
    public LogStatistics LogStat => this.logStat;
  }
}
