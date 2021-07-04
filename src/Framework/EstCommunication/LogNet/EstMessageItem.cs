// Decompiled with JetBrains decompiler
// Type: EstCommunication.LogNet.EstMessageItem
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using System;
using System.Threading;

namespace EstCommunication.LogNet
{
  /// <summary>
  /// 单条日志的记录信息，包含了消息等级，线程号，关键字，文本信息<br />
  /// Record information of a single log, including message level, thread number, keywords, text information
  /// </summary>
  public class EstMessageItem
  {
    private static long IdNumber;

    /// <summary>
    /// 实例化一个默认的对象<br />
    /// Instantiate a default object
    /// </summary>
    public EstMessageItem()
    {
      this.Id = Interlocked.Increment(ref EstMessageItem.IdNumber);
      this.Time = DateTime.Now;
    }

    /// <summary>
    /// 单个记录信息的标识ID，程序重新运行时清空，代表程序从运行以来的日志计数，不管存储的或是未存储的<br />
    /// The ID of a single record of information. It is cleared when the program is re-run.
    /// It represents the log count of the program since it was run, whether stored or unstored.
    /// </summary>
    public long Id { get; private set; }

    /// <summary>
    /// 消息的等级，包括DEBUG，INFO，WARN，ERROR，FATAL，NONE共计六个等级<br />
    /// Message levels, including DEBUG, INFO, WARN, ERROR, FATAL, NONE total six levels
    /// </summary>
    public EstMessageDegree Degree { get; set; } = EstMessageDegree.DEBUG;

    /// <summary>
    /// 线程ID，发生异常时的线程号<br />
    /// Thread ID, the thread number when the exception occurred
    /// </summary>
    public int ThreadId { get; set; }

    /// <summary>
    /// 消息文本，记录日志的时候给定<br />
    /// Message text, given when logging
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// 记录日志的时间，而非存储日志的时间<br />
    /// The time the log was recorded, not the time it was stored
    /// </summary>
    public DateTime Time { get; set; }

    /// <summary>
    /// 消息的关键字<br />
    /// Keyword of the message
    /// </summary>
    public string KeyWord { get; set; }

    /// <summary>
    /// 是否取消写入到文件中去，在事件 <see cref="E:EstCommunication.LogNet.LogNetBase.BeforeSaveToFile" /> 触发的时候捕获即可设置。<br />
    /// Whether to cancel writing to the file, can be set when the event <see cref="E:EstCommunication.LogNet.LogNetBase.BeforeSaveToFile" /> is triggered.
    /// </summary>
    public bool Cancel { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
      if (this.Degree == EstMessageDegree.None)
        return this.Text;
      return string.IsNullOrEmpty(this.KeyWord) ? string.Format("[{0}] {1:yyyy-MM-dd HH:mm:ss.fff} Thread [{2:D3}] {3}", (object) LogNetManagment.GetDegreeDescription(this.Degree), (object) this.Time, (object) this.ThreadId, (object) this.Text) : string.Format("[{0}] {1:yyyy-MM-dd HH:mm:ss.fff} Thread [{2:D3}] {3} : {4}", (object) LogNetManagment.GetDegreeDescription(this.Degree), (object) this.Time, (object) this.ThreadId, (object) this.KeyWord, (object) this.Text);
    }

    /// <summary>
    /// 返回表示当前对象的字符串，剔除了关键字<br />
    /// Returns a string representing the current object, excluding keywords
    /// </summary>
    /// <returns>字符串信息</returns>
    public string ToStringWithoutKeyword()
    {
      if (this.Degree == EstMessageDegree.None)
        return this.Text;
      return string.Format("[{0}] {1:yyyy-MM-dd HH:mm:ss.fff} Thread [{2:D3}] {3}", (object) LogNetManagment.GetDegreeDescription(this.Degree), (object) this.Time, (object) this.ThreadId, (object) this.Text);
    }
  }
}
