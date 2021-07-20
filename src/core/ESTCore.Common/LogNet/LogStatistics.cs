// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.LogNet.LogStatistics
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Core;
using ESTCore.Common.Reflection;

using System;
using System.IO;
using System.Threading;

namespace ESTCore.Common.LogNet
{
    /// <summary>
    /// 一个统计次数的辅助类，可用于实现分析一些次数统计信息，比如统计某个API最近每天的访问次数，
    /// 统计日志组件最近每天访问的次数，调用者只需要关心统计方式和数据个数，详细参照API文档。<br />
    /// An auxiliary class for counting the number of times, which can be used to realize the analysis of some number of times statistical information,
    /// such as counting the number of daily visits of an API, and counting the number of daily visits of the log component.
    /// The caller only needs to care about the statistical method and the number of data. Refer to details API documentation.
    /// </summary>
    /// <example>
    /// 我们来举个例子：我有个方法，AAA需要记录一下连续60天的调用次数信息
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\LogNet\LogStatisticsSample.cs" region="Sample1" title="简单的记录调用次数" />
    /// 因为这个数据是保存在内存里的，程序重新运行就丢失了，如果希望让这个数据一直在程序的话，在软件退出的时候需要存储文件，在软件启动的时候，加载文件数据
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\LogNet\LogStatisticsSample.cs" region="Sample2" title="存储与加载" />
    /// </example>
    public class LogStatistics : LogStatisticsBase<long>
    {
        private RegularByteTransform byteTransform;
        private long totalSum = 0;

        /// <inheritdoc cref="M:ESTCore.Common.LogNet.LogStatisticsBase`1.#ctor(ESTCore.Common.LogNet.GenerateMode,System.Int32)" />
        public LogStatistics(GenerateMode generateMode, int dataCount)
          : base(generateMode, dataCount)
          => this.byteTransform = new RegularByteTransform();

        /// <summary>
        /// 新增一个统计信息，将会根据当前的时间来决定插入数据位置，如果数据位置发生了变化，则数据向左发送移动。如果没有移动或是移动完成后，最后一个数进行新增数据 frequency 次<br />
        /// Adding a new statistical information will determine the position to insert the data according to the current time. If the data position changes,
        /// the data will be sent to the left. If there is no movement or after the movement is completed, add data to the last number frequency times
        /// </summary>
        /// <param name="frequency">新增的次数信息，默认为1</param>
        [EstMqttApi(Description = "Adding a new statistical information will determine the position to insert the data according to the current time.")]
        public void StatisticsAdd(long frequency = 1)
        {
            Interlocked.Add(ref this.totalSum, frequency);
            this.StatisticsCustomAction((Func<long, long>)(m => m + frequency));
        }

        /// <summary>
        /// 新增一个统计信息，将会根据指定的时间来决定插入数据位置，如果数据位置发生了变化，则数据向左发送移动。如果没有移动或是移动完成后，最后一个数进行新增数据 frequency 次<br />
        /// Adding a new statistical information will determine the position to insert the data according to the specified time. If the data position changes,
        /// the data will be sent to the left. If there is no movement or after the movement is completed, add data to the last number frequency times
        /// </summary>
        /// <param name="frequency">新增的次数信息</param>
        /// <param name="time">新增的次数的时间</param>
        [EstMqttApi(Description = "Adding a new statistical information will determine the position to insert the data according to the specified time.")]
        public void StatisticsAddByTime(long frequency, DateTime time)
        {
            Interlocked.Add(ref this.totalSum, frequency);
            this.StatisticsCustomAction((Func<long, long>)(m => m + frequency), time);
        }

        /// <summary>
        /// 将当前所有的数据都写入到二进制的内存里去，可以用来写入文件或是网络发送。<br />
        /// Write all current data into binary memory, which can be used to write files or send over the network.
        /// </summary>
        /// <returns>二进制的byte数组</returns>
        public byte[] SaveToBinary()
        {
            OperateResult<long, long[]> statisticsSnapAndDataMark = this.GetStatisticsSnapAndDataMark();
            int num = 1024;
            byte[] numArray = new byte[statisticsSnapAndDataMark.Content2.Length * 8 + num];
            BitConverter.GetBytes(305419896).CopyTo((Array)numArray, 0);
            BitConverter.GetBytes((ushort)num).CopyTo((Array)numArray, 4);
            BitConverter.GetBytes((ushort)this.GenerateMode).CopyTo((Array)numArray, 6);
            BitConverter.GetBytes(statisticsSnapAndDataMark.Content2.Length).CopyTo((Array)numArray, 8);
            BitConverter.GetBytes(statisticsSnapAndDataMark.Content1).CopyTo((Array)numArray, 12);
            BitConverter.GetBytes(this.TotalSum).CopyTo((Array)numArray, 20);
            for (int index = 0; index < statisticsSnapAndDataMark.Content2.Length; ++index)
                BitConverter.GetBytes(statisticsSnapAndDataMark.Content2[index]).CopyTo((Array)numArray, num + index * 8);
            return numArray;
        }

        /// <summary>
        /// 将当前的统计信息及数据内容写入到指定的文件里面，需要指定文件的路径名称<br />
        /// Write the current statistical information and data content to the specified file, you need to specify the path name of the file
        /// </summary>
        /// <param name="fileName">文件的完整的路径名称</param>
        public void SaveToFile(string fileName) => File.WriteAllBytes(fileName, this.SaveToBinary());

        /// <summary>
        /// 从二进制的数据内容加载，会对数据的合法性进行检查，如果数据不匹配，会报异常<br />
        /// Loading from the binary data content will check the validity of the data. If the data does not match, an exception will be reported
        /// </summary>
        /// <param name="buffer">等待加载的二进制数据</param>
        /// <exception cref="T:System.Exception"></exception>
        public void LoadFromBinary(byte[] buffer)
        {
            int index = BitConverter.ToInt32(buffer, 0) == 305419896 ? (int)BitConverter.ToUInt16(buffer, 4) : throw new Exception("File is not LogStatistics file, can't load data.");
            GenerateMode uint16 = (GenerateMode)BitConverter.ToUInt16(buffer, 6);
            int int32 = BitConverter.ToInt32(buffer, 8);
            long int64_1 = BitConverter.ToInt64(buffer, 12);
            long int64_2 = BitConverter.ToInt64(buffer, 20);
            this.generateMode = uint16;
            this.totalSum = int64_2;
            this.Reset(this.byteTransform.TransInt64(buffer, index, int32), int64_1);
        }

        /// <summary>
        /// 从指定的文件加载对应的统计信息，通常是调用<see cref="M:ESTCore.Common.LogNet.LogStatistics.SaveToFile(System.String)" />方法存储的文件，如果文件不存在，将会跳过加载<br />
        /// Load the corresponding statistical information from the specified file, usually the file stored by calling the <see cref="M:ESTCore.Common.LogNet.LogStatistics.SaveToFile(System.String)" /> method.
        /// If the file does not exist, the loading will be skipped
        /// </summary>
        /// <param name="fileName">文件的完整的路径名称</param>
        /// <exception cref="T:System.Exception">当文件的模式和当前的模式设置不一样的时候，会引发异常</exception>
        public void LoadFromFile(string fileName)
        {
            if (!File.Exists(fileName))
                return;
            this.LoadFromBinary(File.ReadAllBytes(fileName));
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("LogStatistics[{0}:{1}]", (object)this.GenerateMode, (object)this.ArrayLength);

        /// <summary>
        /// 获取当前的所有的值的总和<br />
        /// Get the sum of all current values
        /// </summary>
        [EstMqttApi(Description = "Get the sum of all current values", HttpMethod = "GET")]
        public long TotalSum => this.totalSum;
    }
}
