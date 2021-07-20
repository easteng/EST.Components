// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Robot.FANUC.FanucAlarm
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Core;

using System;
using System.Text;

namespace ESTCore.Common.Robot.FANUC
{
    /// <summary>Fanuc机器人的报警对象</summary>
    public class FanucAlarm
    {
        /// <summary>AlarmID</summary>
        public short AlarmID { get; set; }

        /// <summary>AlarmNumber</summary>
        public short AlarmNumber { get; set; }

        /// <summary>CauseAlarmID</summary>
        public short CauseAlarmID { get; set; }

        /// <summary>CauseAlarmNumber</summary>
        public short CauseAlarmNumber { get; set; }

        /// <summary>Severity</summary>
        public short Severity { get; set; }

        /// <summary>Time</summary>
        public DateTime Time { get; set; }

        /// <summary>AlarmMessage</summary>
        public string AlarmMessage { get; set; }

        /// <summary>CauseAlarmMessage</summary>
        public string CauseAlarmMessage { get; set; }

        /// <summary>SeverityMessage</summary>
        public string SeverityMessage { get; set; }

        /// <summary>从字节数据加载真实的信息</summary>
        /// <param name="byteTransform">字节变换</param>
        /// <param name="content">原始的字节内容</param>
        /// <param name="index">索引</param>
        /// <param name="encoding">编码</param>
        public void LoadByContent(
          IByteTransform byteTransform,
          byte[] content,
          int index,
          Encoding encoding)
        {
            this.AlarmID = BitConverter.ToInt16(content, index);
            this.AlarmNumber = BitConverter.ToInt16(content, index + 2);
            this.CauseAlarmID = BitConverter.ToInt16(content, index + 4);
            this.CauseAlarmNumber = BitConverter.ToInt16(content, index + 6);
            this.Severity = BitConverter.ToInt16(content, index + 8);
            if (BitConverter.ToInt16(content, index + 10) > (short)0)
                this.Time = new DateTime((int)BitConverter.ToInt16(content, index + 10), (int)BitConverter.ToInt16(content, index + 12), (int)BitConverter.ToInt16(content, index + 14), (int)BitConverter.ToInt16(content, index + 16), (int)BitConverter.ToInt16(content, index + 18), (int)BitConverter.ToInt16(content, index + 20));
            this.AlarmMessage = encoding.GetString(content, index + 22, 80).Trim(new char[1]);
            this.CauseAlarmMessage = encoding.GetString(content, index + 102, 80).Trim(new char[1]);
            this.SeverityMessage = encoding.GetString(content, index + 182, 18).Trim(new char[1]);
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("FanucAlarm ID[{0},{1},{2},{3},{4}]{5}{6}{7}{8}{9}{10}", (object)this.AlarmID, (object)this.AlarmNumber, (object)this.CauseAlarmID, (object)this.CauseAlarmNumber, (object)this.Severity, (object)Environment.NewLine, (object)this.AlarmMessage, (object)Environment.NewLine, (object)this.CauseAlarmMessage, (object)Environment.NewLine, (object)this.SeverityMessage);

        /// <summary>从数据内容创建报警信息</summary>
        /// <param name="byteTransform">字节变换</param>
        /// <param name="content">原始的字节内容</param>
        /// <param name="index">索引</param>
        /// <param name="encoding">编码</param>
        /// <returns>报警信息</returns>
        public static FanucAlarm PraseFrom(
          IByteTransform byteTransform,
          byte[] content,
          int index,
          Encoding encoding)
        {
            FanucAlarm fanucAlarm = new FanucAlarm();
            fanucAlarm.LoadByContent(byteTransform, content, index, encoding);
            return fanucAlarm;
        }
    }
}
