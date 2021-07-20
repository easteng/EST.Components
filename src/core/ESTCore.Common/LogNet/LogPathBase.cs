// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.LogNet.LogPathBase
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using System;
using System.Collections.Generic;
using System.IO;

namespace ESTCore.Common.LogNet
{
    /// <summary>
    /// 基于路径实现的日志类的基类，提供几个基础的方法信息。<br />
    /// The base class of the log class implemented based on the path provides several basic method information.
    /// </summary>
    public abstract class LogPathBase : LogNetBase
    {
        /// <summary>
        /// 当前正在存储的文件名<br />
        /// File name currently being stored
        /// </summary>
        protected string fileName = string.Empty;
        /// <summary>
        /// 存储文件的路径，如果设置为空，就不进行存储。<br />
        /// The path for storing the file. If it is set to empty, it will not be stored.
        /// </summary>
        protected string filePath = string.Empty;
        /// <summary>
        /// 控制文件的数量，小于1则不进行任何操作，当设置为10的时候，就限制文件数量为10。<br />
        /// Control the number of files. If it is less than 1, no operation is performed. When it is set to 10, the number of files is limited to 10.
        /// </summary>
        protected int controlFileQuantity = -1;

        /// <inheritdoc />
        protected override void OnWriteCompleted()
        {
            if (this.controlFileQuantity <= 1)
                return;
            try
            {
                string[] existLogFileNames = this.GetExistLogFileNames();
                if (existLogFileNames.Length > this.controlFileQuantity)
                {
                    List<FileInfo> fileInfoList = new List<FileInfo>();
                    for (int index = 0; index < existLogFileNames.Length; ++index)
                        fileInfoList.Add(new FileInfo(existLogFileNames[index]));
                    fileInfoList.Sort((Comparison<FileInfo>)((m, n) => m.CreationTime.CompareTo(n.CreationTime)));
                    for (int index = 0; index < fileInfoList.Count - this.controlFileQuantity; ++index)
                        File.Delete(fileInfoList[index].FullName);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// 返回所有的日志文件名称，返回一个列表<br />
        /// Returns all log file names, returns a list
        /// </summary>
        /// <returns>所有的日志文件信息</returns>
        public string[] GetExistLogFileNames() => !string.IsNullOrEmpty(this.filePath) ? Directory.GetFiles(this.filePath, "Logs_*.txt") : new string[0];

        /// <inheritdoc />
        public override string ToString() => nameof(LogPathBase);
    }
}
