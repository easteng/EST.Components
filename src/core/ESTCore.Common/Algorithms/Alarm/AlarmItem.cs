// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Algorithms.Alarm.AlarmItem
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using System;
using System.Threading;

namespace ESTCore.Common.Algorithms.Alarm
{
    /// <summary>单次报警的信息内容</summary>
    public class AlarmItem
    {
        private long uniqueId = 0;
        private int alarmCode = 0;
        private int userId = 0;
        private DateTime startTime = DateTime.Now;
        private DateTime finishTime = DateTime.Now;
        private string alarmDescription = string.Empty;
        private bool isChecked = false;
        private bool isViewed = false;
        private string checkName = string.Empty;
        private AlarmDegree alarmDegree = AlarmDegree.Hint;
        private static long AlarmIdCurrent;

        /// <summary>实例化一个默认的对象</summary>
        public AlarmItem() => this.uniqueId = Interlocked.Increment(ref AlarmItem.AlarmIdCurrent);

        /// <summary>使用默认的用户id和报警描述信息来初始化报警</summary>
        /// <param name="userId">用户的自身的id标识信息</param>
        /// <param name="alarmDescription">报警的描述信息</param>
        public AlarmItem(int userId, string alarmDescription)
        {
            this.uniqueId = Interlocked.Increment(ref AlarmItem.AlarmIdCurrent);
            this.userId = userId;
            this.alarmDescription = alarmDescription;
        }

        /// <summary>使用默认的用户id和报警描述信息来初始化报警</summary>
        /// <param name="alarmCode">报警的代号</param>
        /// <param name="userId">用户的自身的id标识信息</param>
        /// <param name="alarmDescription">报警的描述信息</param>
        public AlarmItem(int alarmCode, int userId, string alarmDescription)
        {
            this.uniqueId = Interlocked.Increment(ref AlarmItem.AlarmIdCurrent);
            this.alarmCode = alarmCode;
            this.userId = userId;
            this.alarmDescription = alarmDescription;
        }

        /// <summary>本次系统运行的唯一报警信息，用来标识操作的信息的</summary>
        public long UniqueId => this.uniqueId;

        /// <summary>报警的ID信息</summary>
        public int AlarmCode
        {
            get => this.alarmCode;
            set => this.alarmCode = value;
        }

        /// <summary>用户自带的标记信息，可以用来区分不同的设备的情况</summary>
        public int UserId
        {
            get => this.userId;
            set => this.userId = value;
        }
    }
}
