// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.LogNet.LogNetBase
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Core;
using ESTCore.Common.Reflection;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace ESTCore.Common.LogNet
{
    /// <summary>日志存储类的基类，提供一些基础的服务</summary>
    /// <remarks>
    /// 基于此类可以实现任意的规则的日志存储规则，欢迎大家补充实现，本组件实现了3个日志类
    /// <list type="number">
    /// <item>单文件日志类 <see cref="T:ESTCore.Common.LogNet.LogNetSingle" /></item>
    /// <item>根据文件大小的类 <see cref="T:ESTCore.Common.LogNet.LogNetFileSize" /></item>
    /// <item>根据时间进行存储的类 <see cref="T:ESTCore.Common.LogNet.LogNetDateTime" /></item>
    /// </list>
    /// </remarks>
    public abstract class LogNetBase : IDisposable
    {
        /// <summary>文件存储的锁</summary>
        protected SimpleHybirdLock m_fileSaveLock;
        private EstMessageDegree m_messageDegree = EstMessageDegree.DEBUG;
        private Queue<EstMessageItem> m_WaitForSave;
        private SimpleHybirdLock m_simpleHybirdLock;
        private int m_SaveStatus = 0;
        private List<string> filtrateKeyword;
        private SimpleHybirdLock filtrateLock;
        private bool disposedValue = false;

        /// <summary>
        /// 实例化一个日志对象<br />
        /// Instantiate a log object
        /// </summary>
        public LogNetBase()
        {
            this.m_fileSaveLock = new SimpleHybirdLock();
            this.m_simpleHybirdLock = new SimpleHybirdLock();
            this.m_WaitForSave = new Queue<EstMessageItem>();
            this.filtrateKeyword = new List<string>();
            this.filtrateLock = new SimpleHybirdLock();
        }

        /// <inheritdoc cref="E:ESTCore.Common.LogNet.ILogNet.BeforeSaveToFile" />
        public event EventHandler<EstEventArgs> BeforeSaveToFile = null;

        private void OnBeforeSaveToFile(EstEventArgs args)
        {
            EventHandler<EstEventArgs> beforeSaveToFile = this.BeforeSaveToFile;
            if (beforeSaveToFile == null)
                return;
            beforeSaveToFile((object)this, args);
        }

        /// <inheritdoc cref="P:ESTCore.Common.LogNet.ILogNet.LogSaveMode" />
        public LogSaveMode LogSaveMode { get; protected set; }

        /// <inheritdoc cref="P:ESTCore.Common.LogNet.ILogNet.LogNetStatistics" />
        public LogStatistics LogNetStatistics { get; set; }

        /// <inheritdoc cref="P:ESTCore.Common.LogNet.ILogNet.ConsoleOutput" />
        public bool ConsoleOutput { get; set; }

        /// <inheritdoc cref="M:ESTCore.Common.LogNet.ILogNet.WriteDebug(System.String)" />
        [EstMqttApi]
        public void WriteDebug(string text) => this.WriteDebug(string.Empty, text);

        /// <inheritdoc cref="M:ESTCore.Common.LogNet.ILogNet.WriteDebug(System.String,System.String)" />
        [EstMqttApi(ApiTopic = "WriteDebugKeyWord")]
        public void WriteDebug(string keyWord, string text) => this.RecordMessage(EstMessageDegree.DEBUG, keyWord, text);

        /// <inheritdoc cref="M:ESTCore.Common.LogNet.ILogNet.WriteInfo(System.String)" />
        [EstMqttApi]
        public void WriteInfo(string text) => this.WriteInfo(string.Empty, text);

        /// <inheritdoc cref="M:ESTCore.Common.LogNet.ILogNet.WriteInfo(System.String,System.String)" />
        [EstMqttApi(ApiTopic = "WriteInfoKeyWord")]
        public void WriteInfo(string keyWord, string text) => this.RecordMessage(EstMessageDegree.INFO, keyWord, text);

        /// <inheritdoc cref="M:ESTCore.Common.LogNet.ILogNet.WriteWarn(System.String)" />
        [EstMqttApi]
        public void WriteWarn(string text) => this.WriteWarn(string.Empty, text);

        /// <inheritdoc cref="M:ESTCore.Common.LogNet.ILogNet.WriteWarn(System.String,System.String)" />
        [EstMqttApi(ApiTopic = "WriteWarnKeyWord")]
        public void WriteWarn(string keyWord, string text) => this.RecordMessage(EstMessageDegree.WARN, keyWord, text);

        /// <inheritdoc cref="M:ESTCore.Common.LogNet.ILogNet.WriteError(System.String)" />
        [EstMqttApi]
        public void WriteError(string text) => this.WriteError(string.Empty, text);

        /// <inheritdoc cref="M:ESTCore.Common.LogNet.ILogNet.WriteError(System.String,System.String)" />
        [EstMqttApi(ApiTopic = "WriteErrorKeyWord")]
        public void WriteError(string keyWord, string text) => this.RecordMessage(EstMessageDegree.ERROR, keyWord, text);

        /// <inheritdoc cref="M:ESTCore.Common.LogNet.ILogNet.WriteFatal(System.String)" />
        [EstMqttApi]
        public void WriteFatal(string text) => this.WriteFatal(string.Empty, text);

        /// <inheritdoc cref="M:ESTCore.Common.LogNet.ILogNet.WriteFatal(System.String,System.String)" />
        [EstMqttApi(ApiTopic = "WriteFatalKeyWord")]
        public void WriteFatal(string keyWord, string text) => this.RecordMessage(EstMessageDegree.FATAL, keyWord, text);

        /// <inheritdoc cref="M:ESTCore.Common.LogNet.ILogNet.WriteException(System.String,System.Exception)" />
        public void WriteException(string keyWord, Exception ex) => this.WriteException(keyWord, string.Empty, ex);

        /// <inheritdoc cref="M:ESTCore.Common.LogNet.ILogNet.WriteException(System.String,System.String,System.Exception)" />
        public void WriteException(string keyWord, string text, Exception ex) => this.RecordMessage(EstMessageDegree.FATAL, keyWord, LogNetManagment.GetSaveStringFromException(text, ex));

        /// <inheritdoc cref="M:ESTCore.Common.LogNet.ILogNet.RecordMessage(ESTCore.Common.LogNet.EstMessageDegree,System.String,System.String)" />
        public void RecordMessage(EstMessageDegree degree, string keyWord, string text) => this.WriteToFile(degree, keyWord, text);

        /// <inheritdoc cref="M:ESTCore.Common.LogNet.ILogNet.WriteDescrition(System.String)" />
        [EstMqttApi]
        public void WriteDescrition(string description)
        {
            if (string.IsNullOrEmpty(description))
                return;
            StringBuilder sb = new StringBuilder("\x0002");
            sb.Append(Environment.NewLine);
            sb.Append("\x0002/");
            int num = 118 - this.CalculateStringOccupyLength(description);
            if (num >= 8)
            {
                int count = (num - 8) / 2;
                this.AppendCharToStringBuilder(sb, '*', count);
                sb.Append("   ");
                sb.Append(description);
                sb.Append("   ");
                if (num % 2 == 0)
                    this.AppendCharToStringBuilder(sb, '*', count);
                else
                    this.AppendCharToStringBuilder(sb, '*', count + 1);
            }
            else if (num >= 2)
            {
                int count = (num - 2) / 2;
                this.AppendCharToStringBuilder(sb, '*', count);
                sb.Append(description);
                if (num % 2 == 0)
                    this.AppendCharToStringBuilder(sb, '*', count);
                else
                    this.AppendCharToStringBuilder(sb, '*', count + 1);
            }
            else
                sb.Append(description);
            sb.Append("/");
            sb.Append(Environment.NewLine);
            this.RecordMessage(EstMessageDegree.None, string.Empty, sb.ToString());
        }

        /// <inheritdoc cref="M:ESTCore.Common.LogNet.ILogNet.WriteAnyString(System.String)" />
        [EstMqttApi]
        public void WriteAnyString(string text) => this.RecordMessage(EstMessageDegree.None, string.Empty, text);

        /// <inheritdoc cref="M:ESTCore.Common.LogNet.ILogNet.WriteNewLine" />
        [EstMqttApi]
        public void WriteNewLine() => this.RecordMessage(EstMessageDegree.None, string.Empty, "\x0002" + Environment.NewLine);

        /// <inheritdoc cref="M:ESTCore.Common.LogNet.ILogNet.SetMessageDegree(ESTCore.Common.LogNet.EstMessageDegree)" />
        public void SetMessageDegree(EstMessageDegree degree) => this.m_messageDegree = degree;

        /// <inheritdoc cref="M:ESTCore.Common.LogNet.ILogNet.FiltrateKeyword(System.String)" />
        [EstMqttApi]
        public void FiltrateKeyword(string keyword)
        {
            this.filtrateLock.Enter();
            if (!this.filtrateKeyword.Contains(keyword))
                this.filtrateKeyword.Add(keyword);
            this.filtrateLock.Leave();
        }

        /// <inheritdoc cref="M:ESTCore.Common.LogNet.ILogNet.RemoveFiltrate(System.String)" />
        [EstMqttApi]
        public void RemoveFiltrate(string keyword)
        {
            this.filtrateLock.Enter();
            if (this.filtrateKeyword.Contains(keyword))
                this.filtrateKeyword.Remove(keyword);
            this.filtrateLock.Leave();
        }

        private void WriteToFile(EstMessageDegree degree, string keyword, string text)
        {
            if (degree > this.m_messageDegree)
                return;
            this.AddItemToCache(this.GetEstMessageItem(degree, keyword, text));
        }

        private void AddItemToCache(EstMessageItem item)
        {
            this.m_simpleHybirdLock.Enter();
            this.m_WaitForSave.Enqueue(item);
            this.m_simpleHybirdLock.Leave();
            this.StartSaveFile();
        }

        private void StartSaveFile()
        {
            if (Interlocked.CompareExchange(ref this.m_SaveStatus, 1, 0) != 0)
                return;
            ThreadPool.QueueUserWorkItem(new WaitCallback(this.ThreadPoolSaveFile), (object)null);
        }

        private EstMessageItem GetAndRemoveLogItem()
        {
            this.m_simpleHybirdLock.Enter();
            EstMessageItem hslMessageItem = this.m_WaitForSave.Count > 0 ? this.m_WaitForSave.Dequeue() : (EstMessageItem)null;
            this.m_simpleHybirdLock.Leave();
            return hslMessageItem;
        }

        private void ConsoleWriteLog(EstMessageItem log)
        {
            Console.ForegroundColor = log.Degree != EstMessageDegree.DEBUG ? (log.Degree != EstMessageDegree.INFO ? (log.Degree != EstMessageDegree.WARN ? (log.Degree != EstMessageDegree.ERROR ? (log.Degree != EstMessageDegree.FATAL ? ConsoleColor.White : ConsoleColor.DarkRed) : ConsoleColor.Red) : ConsoleColor.Yellow) : ConsoleColor.White) : ConsoleColor.DarkGray;
            Console.WriteLine(log.ToString());
        }

        private void ThreadPoolSaveFile(object obj)
        {
            EstMessageItem andRemoveLogItem = this.GetAndRemoveLogItem();
            this.m_fileSaveLock.Enter();
            string fileSaveName = this.GetFileSaveName();
            if (!string.IsNullOrEmpty(fileSaveName))
            {
                StreamWriter streamWriter = (StreamWriter)null;
                try
                {
                    streamWriter = new StreamWriter(fileSaveName, true, Encoding.UTF8);
                    for (; andRemoveLogItem != null; andRemoveLogItem = this.GetAndRemoveLogItem())
                    {
                        if (this.ConsoleOutput)
                            this.ConsoleWriteLog(andRemoveLogItem);
                        this.OnBeforeSaveToFile(new EstEventArgs()
                        {
                            EstMessage = andRemoveLogItem
                        });
                        this.LogNetStatistics?.StatisticsAdd();
                        this.filtrateLock.Enter();
                        bool flag = !this.filtrateKeyword.Contains(andRemoveLogItem.KeyWord);
                        this.filtrateLock.Leave();
                        if (andRemoveLogItem.Cancel)
                            flag = false;
                        if (flag)
                        {
                            streamWriter.Write(this.EstMessageFormate(andRemoveLogItem));
                            streamWriter.Write(Environment.NewLine);
                            streamWriter.Flush();
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.AddItemToCache(andRemoveLogItem);
                    this.AddItemToCache(new EstMessageItem()
                    {
                        Degree = EstMessageDegree.FATAL,
                        Text = LogNetManagment.GetSaveStringFromException("LogNetSelf", ex)
                    });
                }
                finally
                {
                    streamWriter?.Dispose();
                }
            }
            else
            {
                for (; andRemoveLogItem != null; andRemoveLogItem = this.GetAndRemoveLogItem())
                {
                    if (this.ConsoleOutput)
                        this.ConsoleWriteLog(andRemoveLogItem);
                    this.OnBeforeSaveToFile(new EstEventArgs()
                    {
                        EstMessage = andRemoveLogItem
                    });
                }
            }
            this.m_fileSaveLock.Leave();
            Interlocked.Exchange(ref this.m_SaveStatus, 0);
            this.OnWriteCompleted();
            if (this.m_WaitForSave.Count <= 0)
                return;
            this.StartSaveFile();
        }

        private string EstMessageFormate(EstMessageItem hslMessage)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (hslMessage.Degree != EstMessageDegree.None)
            {
                stringBuilder.Append("\x0002");
                stringBuilder.Append("[");
                stringBuilder.Append(LogNetManagment.GetDegreeDescription(hslMessage.Degree));
                stringBuilder.Append("] ");
                stringBuilder.Append(hslMessage.Time.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                stringBuilder.Append(" thread:[");
                stringBuilder.Append(hslMessage.ThreadId.ToString("D3"));
                stringBuilder.Append("] ");
                if (!string.IsNullOrEmpty(hslMessage.KeyWord))
                {
                    stringBuilder.Append(hslMessage.KeyWord);
                    stringBuilder.Append(" : ");
                }
            }
            stringBuilder.Append(hslMessage.Text);
            return stringBuilder.ToString();
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("LogNetBase[{0}]", (object)this.LogSaveMode);

        /// <inheritdoc />
        protected virtual string GetFileSaveName() => string.Empty;

        /// <summary>
        /// 当写入文件完成的时候触发，这时候已经释放了文件的句柄了。<br />
        /// Triggered when writing to the file is complete, and the file handle has been released.
        /// </summary>
        protected virtual void OnWriteCompleted()
        {
        }

        private EstMessageItem GetEstMessageItem(
          EstMessageDegree degree,
          string keyWord,
          string text)
        {
            return new EstMessageItem()
            {
                KeyWord = keyWord,
                Degree = degree,
                Text = text,
                ThreadId = Thread.CurrentThread.ManagedThreadId
            };
        }

        private int CalculateStringOccupyLength(string str)
        {
            if (string.IsNullOrEmpty(str))
                return 0;
            int num = 0;
            for (int index = 0; index < str.Length; ++index)
            {
                if (str[index] >= '一' && str[index] <= '龻')
                    num += 2;
                else
                    ++num;
            }
            return num;
        }

        private void AppendCharToStringBuilder(StringBuilder sb, char c, int count)
        {
            for (int index = 0; index < count; ++index)
                sb.Append(c);
        }

        /// <summary>释放资源</summary>
        /// <param name="disposing">是否初次调用</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposedValue)
                return;
            if (disposing)
            {
                this.m_simpleHybirdLock.Dispose();
                this.m_WaitForSave.Clear();
                this.m_fileSaveLock.Dispose();
            }
            this.disposedValue = true;
        }

        /// <inheritdoc cref="M:System.IDisposable.Dispose" />
        public void Dispose() => this.Dispose(true);
    }
}
