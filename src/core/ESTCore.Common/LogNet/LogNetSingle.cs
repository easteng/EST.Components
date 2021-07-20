// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.LogNet.LogNetSingle
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using System;
using System.IO;
using System.Text;

namespace ESTCore.Common.LogNet
{
    /// <summary>
    /// 单日志文件对象，所有的日志信息的记录都会写入一个文件里面去。文件名指定为空的时候，自动不存储文件。<br />
    /// Single log file object, all log information records will be written to a file. When the file name is specified as empty, the file is not stored automatically.
    /// </summary>
    /// <remarks>
    /// 此日志实例化需要指定一个完整的文件路径，当需要记录日志的时候调用方法，会使得日志越来越大，对于写入的性能没有太大影响，但是会影响文件读取。
    /// </remarks>
    /// <example>
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\LogNet\LogNetSingle.cs" region="Example1" title="单文件实例化" />
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\LogNet\LogNetSingle.cs" region="Example4" title="基本的使用" />
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\LogNet\LogNetSingle.cs" region="Example5" title="所有日志不存储" />
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\LogNet\LogNetSingle.cs" region="Example6" title="仅存储ERROR等级" />
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\LogNet\LogNetSingle.cs" region="Example7" title="不指定路径" />
    /// </example>
    public class LogNetSingle : LogNetBase, ILogNet, IDisposable
    {
        private readonly string fileName = string.Empty;

        /// <summary>
        /// 实例化一个单文件日志的对象，如果日志的路径为空，那么就不存储数据，只触发<see cref="E:ESTCore.Common.LogNet.LogNetBase.BeforeSaveToFile" />事件<br />
        /// Instantiate a single file log object. If the log path is empty, then no data is stored and only the <see cref="E:ESTCore.Common.LogNet.LogNetBase.BeforeSaveToFile" /> event is triggered.
        /// </summary>
        /// <param name="filePath">文件的路径</param>
        public LogNetSingle(string filePath)
        {
            this.fileName = filePath;
            this.LogSaveMode = LogSaveMode.SingleFile;
            if (string.IsNullOrEmpty(this.fileName))
                return;
            FileInfo fileInfo = new FileInfo(filePath);
            if (!Directory.Exists(fileInfo.DirectoryName))
                Directory.CreateDirectory(fileInfo.DirectoryName);
        }

        /// <summary>
        /// 单日志文件允许清空日志内容<br />
        /// Single log file allows clearing log contents
        /// </summary>
        public void ClearLog()
        {
            this.m_fileSaveLock.Enter();
            try
            {
                if (string.IsNullOrEmpty(this.fileName))
                    return;
                File.Create(this.fileName).Dispose();
            }
            catch
            {
                throw;
            }
            finally
            {
                this.m_fileSaveLock.Leave();
            }
        }

        /// <summary>
        /// 获取单日志文件的所有保存记录<br />
        /// Get all saved records of a single log file
        /// </summary>
        /// <returns>字符串信息</returns>
        public string GetAllSavedLog()
        {
            string str = string.Empty;
            this.m_fileSaveLock.Enter();
            try
            {
                if (!string.IsNullOrEmpty(this.fileName))
                {
                    if (File.Exists(this.fileName))
                    {
                        StreamReader streamReader = new StreamReader(this.fileName, Encoding.UTF8);
                        str = streamReader.ReadToEnd();
                        streamReader.Dispose();
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                this.m_fileSaveLock.Leave();
            }
            return str;
        }

        /// <summary>
        /// 获取所有的日志文件数组，对于单日志文件来说就只有一个<br />
        /// Get all log file arrays, only one for a single log file
        /// </summary>
        /// <returns>字符串数组，包含了所有的存在的日志数据</returns>
        public string[] GetExistLogFileNames() => new string[1]
        {
      this.fileName
        };

        /// <inheritdoc />
        protected override string GetFileSaveName() => this.fileName;

        /// <inheritdoc />
        public override string ToString() => "LogNetSingle[" + this.fileName + "]";
    }
}
