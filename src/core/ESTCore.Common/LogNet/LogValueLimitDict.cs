// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.LogNet.LogValueLimitDict
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Core;
using ESTCore.Common.Reflection;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ESTCore.Common.LogNet
{
    /// <summary>
    /// <seealso cref="T:ESTCore.Common.LogNet.LogValueLimit" /> 的词典集合类，用于多个数据的统计信息，例如可以统计多个温度变量的最大值，最小值，平均值<br />
    /// <seealso cref="T:ESTCore.Common.LogNet.LogValueLimit" /> The dictionary collection class, used for statistical information of multiple data,
    /// for example, it can count the maximum, minimum, and average values of multiple temperature variables
    /// </summary>
    public class LogValueLimitDict
    {
        private GenerateMode generateMode = GenerateMode.ByEveryDay;
        private int arrayLength = 30;
        private Dictionary<string, LogValueLimit> dict;
        private object dictLock;
        private LogStatistics logStat;

        /// <summary>
        /// 根据指定的存储模式，数据个数来实例化一个对象<br />
        /// According to the specified storage mode, the number of data to instantiate an object
        /// </summary>
        /// <param name="generateMode">当前的数据存储模式</param>
        /// <param name="arrayLength">准备存储的数据总个数</param>
        public LogValueLimitDict(GenerateMode generateMode, int arrayLength)
        {
            this.generateMode = generateMode;
            this.arrayLength = arrayLength;
            this.dictLock = new object();
            this.dict = new Dictionary<string, LogValueLimit>(128);
            this.logStat = new LogStatistics(generateMode, arrayLength);
        }

        /// <summary>
        /// 根据给定的关键字信息，获取相关的 <see cref="T:ESTCore.Common.LogNet.LogValueLimit" /> 对象，进而执行很多的操作<br />
        /// According to the given keyword information, obtain related <see cref="T:ESTCore.Common.LogNet.LogValueLimit" /> objects, and then perform many operations
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns>日志对象，如果当前的日志对象不存在，就返回为NULL</returns>
        public LogValueLimit GetLogValueLimit(string key)
        {
            lock (this.dictLock)
                return this.dict.ContainsKey(key) ? this.dict[key] : (LogValueLimit)null;
        }

        /// <summary>
        /// 手动新增一个 <see cref="T:ESTCore.Common.LogNet.LogValueLimit" /> 对象，需要指定相关的关键字<br />
        /// Manually add a <see cref="T:ESTCore.Common.LogNet.LogValueLimit" /> object, you need to specify related keywords
        /// </summary>
        /// <param name="key">关键字信息</param>
        /// <param name="logValueLimit">日志数据分析对象</param>
        public void AddLogValueLimit(string key, LogValueLimit logValueLimit)
        {
            lock (this.dictLock)
            {
                if (this.dict.ContainsKey(key))
                    this.dict[key] = logValueLimit;
                else
                    this.dict.Add(key, logValueLimit);
            }
        }

        /// <summary>
        /// 移除一个<seealso cref="T:ESTCore.Common.LogNet.LogValueLimit" />对象，需要指定相关的关键字，如果关键字本来就存在，返回 <c>True</c>, 如果不存在，返回 <c>False</c> <br />
        /// To remove a <seealso cref="T:ESTCore.Common.LogNet.LogValueLimit" /> object, you need to specify the relevant keyword. If the keyword already exists, return <c>True</c>, if it does not exist, return <c>False</c>
        /// </summary>
        /// <param name="key">关键字信息</param>
        /// <returns>如果关键字本来就存在，返回 <c>True</c>, 如果不存在，返回 <c>False</c> </returns>
        public bool RemoveLogValueLimit(string key)
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
        /// 新增一个数据用于分析，将会根据当前的时间来决定插入数据位置，如果数据位置发生了变化，则数据向左发送移动。如果没有移动或是移动完成后，最后一个数进行数据更新，包括最大值，最小值，平均值。<br />
        /// Add a new data for analysis, and will determine the position to insert the data according to the current time. If the data position changes,
        /// the data will be sent to the left. If there is no movement or after the movement is completed, data update for the last number, including maximum, minimum, and average.
        /// </summary>
        /// <param name="key">当前的关键字</param>
        /// <param name="value">当前的新的数据值</param>
        [EstMqttApi(Description = "Add a new data for analysis")]
        public void AnalysisNewValue(string key, double value)
        {
            this.logStat.StatisticsAdd();
            LogValueLimit logValueLimit = this.GetLogValueLimit(key);
            if (logValueLimit == null)
            {
                lock (this.dictLock)
                {
                    if (!this.dict.ContainsKey(key))
                    {
                        logValueLimit = new LogValueLimit(this.generateMode, this.arrayLength);
                        this.dict.Add(key, logValueLimit);
                    }
                    else
                        logValueLimit = this.dict[key];
                }
            }
            logValueLimit?.AnalysisNewValue(value);
        }

        /// <summary>
        /// 新增一个数据用于分析，将会指定的时间来决定插入数据位置，如果数据位置发生了变化，则数据向左发送移动。如果没有移动或是移动完成后，最后一个数进行数据更新，包括最大值，最小值，平均值。<br />
        /// dd a new data for analysis, and will determine the position to insert the data according to the specified time. If the data position changes,
        /// the data will be sent to the left. If there is no movement or after the movement is completed, data update for the last number, including maximum, minimum, and average.
        /// </summary>
        /// <param name="key">关键字信息</param>
        /// <param name="value">当前的新的数据值</param>
        /// <param name="time">指定的时间信息</param>
        [EstMqttApi(Description = "Add a new data for analysis")]
        public void AnalysisNewValueByTime(string key, double value, DateTime time)
        {
            this.logStat.StatisticsAddByTime(1L, time);
            LogValueLimit logValueLimit = this.GetLogValueLimit(key);
            if (logValueLimit == null)
            {
                lock (this.dictLock)
                {
                    if (!this.dict.ContainsKey(key))
                    {
                        logValueLimit = new LogValueLimit(this.generateMode, this.arrayLength);
                        this.dict.Add(key, logValueLimit);
                    }
                    else
                        logValueLimit = this.dict[key];
                }
            }
            logValueLimit?.AnalysisNewValueByTime(value, time);
        }

        /// <summary>
        /// 获取当前的统计信息的数据快照，这是数据的副本，修改了里面的值不影响<br />
        /// Get a data snapshot of the current statistics. This is a copy of the data. Modifying the value inside does not affect
        /// </summary>
        /// <param name="key">当前的关键字的信息</param>
        /// <returns>实际的统计数据信息</returns>
        [EstMqttApi(Description = "Get a data snapshot of the current statistics")]
        public ValueLimit[] GetStatisticsSnapshot(string key) => this.GetLogValueLimit(key)?.GetStatisticsSnapshot();

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
        public ValueLimit[] GetStatisticsSnapshotByTime(
          string key,
          DateTime start,
          DateTime finish)
        {
            return this.GetLogValueLimit(key)?.GetStatisticsSnapshotByTime(start, finish);
        }

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
                BitConverter.GetBytes(305419906).CopyTo((Array)buffer, 0);
                BitConverter.GetBytes((ushort)buffer.Length).CopyTo((Array)buffer, 4);
                BitConverter.GetBytes((ushort)this.GenerateMode).CopyTo((Array)buffer, 6);
                string[] keys = this.GetKeys();
                BitConverter.GetBytes(keys.Length).CopyTo((Array)buffer, 8);
                fileStream.Write(buffer, 0, buffer.Length);
                foreach (string key in keys)
                {
                    LogValueLimit logValueLimit = this.GetLogValueLimit(key);
                    if (logValueLimit != null)
                    {
                        EstHelper.WriteStringToStream((Stream)fileStream, key);
                        EstHelper.WriteBinaryToStream((Stream)fileStream, logValueLimit.SaveToBinary());
                    }
                }
            }
        }

        /// <summary>
        /// 从指定的文件加载对应的统计信息，通常是调用<see cref="M:ESTCore.Common.LogNet.LogValueLimitDict.SaveToFile(System.String)" />方法存储的文件，如果文件不存在，将会跳过加载<br />
        /// Load the corresponding statistical information from the specified file, usually the file stored by calling the <see cref="M:ESTCore.Common.LogNet.LogValueLimitDict.SaveToFile(System.String)" /> method.
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
                byte[] numArray = EstHelper.ReadSpecifiedLengthFromStream((Stream)fileStream, 1024);
                this.generateMode = (GenerateMode)BitConverter.ToUInt16(numArray, 6);
                int int32 = BitConverter.ToInt32(numArray, 8);
                for (int index = 0; index < int32; ++index)
                {
                    string key = EstHelper.ReadStringFromStream((Stream)fileStream);
                    byte[] buffer = EstHelper.ReadBinaryFromStream((Stream)fileStream);
                    LogValueLimit logValueLimit = new LogValueLimit(this.generateMode, this.arrayLength);
                    logValueLimit.LoadFromBinary(buffer);
                    this.AddLogValueLimit(key, logValueLimit);
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
        /// 获取当前词典类自身的日志统计对象，统计所有的元素的数据分析次数<br />
        /// Get the log statistics object of the current dictionary class itself, count the data analysis times of all elements
        /// </summary>
        [EstMqttApi(Description = "Get the log statistics object of the current dictionary class itself, and count the statistics of all elements", PropertyUnfold = true)]
        public LogStatistics LogStat => this.logStat;
    }
}
