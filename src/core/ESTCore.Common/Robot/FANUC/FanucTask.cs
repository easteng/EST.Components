// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Robot.FANUC.FanucTask
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Core;

using System;
using System.Text;

namespace ESTCore.Common.Robot.FANUC
{
    /// <summary>Fanuc机器人的任务类</summary>
    public class FanucTask
    {
        /// <summary>ProgramName</summary>
        public string ProgramName { get; set; }

        /// <summary>LineNumber</summary>
        public short LineNumber { get; set; }

        /// <summary>State</summary>
        public short State { get; set; }

        /// <summary>ParentProgramName</summary>
        public string ParentProgramName { get; set; }

        /// <summary>从原始的数据对象加载数据信息</summary>
        /// <param name="byteTransform">字节变换</param>
        /// <param name="content">原始的字节数据</param>
        /// <param name="index">索引信息</param>
        /// <param name="encoding">编码</param>
        public void LoadByContent(
          IByteTransform byteTransform,
          byte[] content,
          int index,
          Encoding encoding)
        {
            this.ProgramName = encoding.GetString(content, index, 16).Trim(new char[1]);
            this.LineNumber = BitConverter.ToInt16(content, index + 16);
            this.State = BitConverter.ToInt16(content, index + 18);
            this.ParentProgramName = encoding.GetString(content, index + 20, 16).Trim(new char[1]);
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("ProgramName[{0}] LineNumber[{1}] State[{2}] ParentProgramName[{3}]", (object)this.ProgramName, (object)this.LineNumber, (object)this.State, (object)this.ParentProgramName);

        /// <summary>从原始的数据信息初始化一个任务对象</summary>
        /// <param name="byteTransform">字节变换</param>
        /// <param name="content">原始的字节数据</param>
        /// <param name="index">索引信息</param>
        /// <param name="encoding">编码</param>
        /// <returns>任务对象</returns>
        public static FanucTask PraseFrom(
          IByteTransform byteTransform,
          byte[] content,
          int index,
          Encoding encoding)
        {
            FanucTask fanucTask = new FanucTask();
            fanucTask.LoadByContent(byteTransform, content, index, encoding);
            return fanucTask;
        }
    }
}
