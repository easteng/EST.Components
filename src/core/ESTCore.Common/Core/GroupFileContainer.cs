// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Core.GroupFileContainer
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.LogNet;

using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ESTCore.Common.Core
{
    /// <summary>
    /// 文件集容器，绑定一个文件夹的文件信息组，提供了文件夹的文件信息的获取，更新接口<br />
    /// File set container, which binds the file information group of a folder, provides the file information acquisition and update interface of the folder
    /// </summary>
    public class GroupFileContainer
    {
        private string dirPath = string.Empty;
        private const string FileListResources = "list.txt";
        private ILogNet LogNet;
        private string jsonArrayContent = "[]";
        private int filesCount = 0;
        private object hybirdLock = new object();
        private EstAsyncCoordinator coordinatorCacheJsonArray;
        private List<GroupFileItem> groupFileItems;
        private string fileFolderPath;
        private string fileFullPath;

        /// <summary>
        /// 实例化一个新的指定目录的文件管理容器<br />
        /// Instantiates a new file management container for the specified directory
        /// </summary>
        /// <param name="logNet">日志记录对象，可以为空</param>
        /// <param name="path">文件的路径</param>
        public GroupFileContainer(ILogNet logNet, string path)
        {
            this.LogNet = logNet;
            this.dirPath = path;
            if (string.IsNullOrEmpty(path))
                return;
            this.LoadByPath(path);
        }

        /// <summary>
        /// 包含所有文件列表信息的json文本缓存<br />
        /// JSON text cache containing all file list information
        /// </summary>
        public string JsonArrayContent => this.jsonArrayContent;

        /// <summary>
        /// 获取文件的数量<br />
        /// Get the number of files
        /// </summary>
        public int FileCount => this.filesCount;

        /// <summary>当前的目录信息</summary>
        public string DirectoryPath => this.dirPath;

        /// <summary>
        /// 当文件数量发生变化的时候触发的事件<br />
        /// Event triggered when the number of files changes
        /// </summary>
        public event GroupFileContainer.FileCountChangedDelegate FileCountChanged;

        /// <summary>
        /// 下载文件时调用，根据当前的文件名称，例如 123.txt 获取到在文件服务器里映射的文件名称，例如返回 b35a11ec533147ca80c7f7d1713f015b7909<br />
        /// Called when downloading a file. Get the file name mapped in the file server according to the current file name, such as 123.txt.
        /// For example, return b35a11ec533147ca80c7f7d1713f015b7909.
        /// </summary>
        /// <param name="fileName">文件的实际名称</param>
        /// <returns>文件名映射过去的实际的文件名字</returns>
        public string GetCurrentFileMappingName(string fileName)
        {
            string str = string.Empty;
            lock (this.hybirdLock)
            {
                for (int index = 0; index < this.groupFileItems.Count; ++index)
                {
                    if (this.groupFileItems[index].FileName == fileName)
                    {
                        str = this.groupFileItems[index].MappingName;
                        ++this.groupFileItems[index].DownloadTimes;
                    }
                }
            }
            this.coordinatorCacheJsonArray.StartOperaterInfomation();
            return str;
        }

        /// <summary>
        /// 上传文件时掉用，通过比对现有的文件列表，如果没有，就重新创建列表信息<br />
        /// Used when uploading files, by comparing existing file lists, if not, re-creating list information
        /// </summary>
        /// <param name="fileName">文件名，带后缀，不带任何的路径</param>
        /// <param name="fileSize">文件的大小</param>
        /// <param name="mappingName">文件映射名称</param>
        /// <param name="owner">文件的拥有者</param>
        /// <param name="description">文件的额外描述</param>
        /// <returns>映射的文件名称</returns>
        public string UpdateFileMappingName(
          string fileName,
          long fileSize,
          string mappingName,
          string owner,
          string description)
        {
            string str = string.Empty;
            lock (this.hybirdLock)
            {
                for (int index = 0; index < this.groupFileItems.Count; ++index)
                {
                    if (this.groupFileItems[index].FileName == fileName)
                    {
                        str = this.groupFileItems[index].MappingName;
                        this.groupFileItems[index].MappingName = mappingName;
                        this.groupFileItems[index].Description = description;
                        this.groupFileItems[index].FileSize = fileSize;
                        this.groupFileItems[index].Owner = owner;
                        this.groupFileItems[index].UploadTime = DateTime.Now;
                        break;
                    }
                }
                if (string.IsNullOrEmpty(str))
                    this.groupFileItems.Add(new GroupFileItem()
                    {
                        FileName = fileName,
                        FileSize = fileSize,
                        DownloadTimes = 0L,
                        Description = description,
                        Owner = owner,
                        MappingName = mappingName,
                        UploadTime = DateTime.Now
                    });
            }
            this.coordinatorCacheJsonArray.StartOperaterInfomation();
            return str;
        }

        /// <summary>
        /// 删除一个文件信息，传入文件实际的名称，例如 123.txt 返回被删除的文件的guid名称，例如返回 b35a11ec533147ca80c7f7d1713f015b7909   此方法存在同名文件删除的风险<br />
        /// Delete a file information. Pass in the actual name of the file. For example, 123.txt returns the guid name of the deleted file. For example, it returns b35a11ec533147ca80c7f7d1713f015b7909. There is a risk of deleting the file with the same name
        /// </summary>
        /// <param name="fileName">实际的文件名称，如果 123.txt</param>
        /// <returns>映射之后的文件名，例如 b35a11ec533147ca80c7f7d1713f015b7909</returns>
        public string DeleteFile(string fileName)
        {
            string str = string.Empty;
            lock (this.hybirdLock)
            {
                for (int index = 0; index < this.groupFileItems.Count; ++index)
                {
                    if (this.groupFileItems[index].FileName == fileName)
                    {
                        str = this.groupFileItems[index].MappingName;
                        this.groupFileItems.RemoveAt(index);
                        break;
                    }
                }
            }
            this.coordinatorCacheJsonArray.StartOperaterInfomation();
            return str;
        }

        /// <summary>
        /// 判断当前的文件名是否在文件的列表里，传入文件实际的名称，例如 123.txt，如果文件存在，返回 true, 如果不存在，返回 false<br />
        /// Determine whether the current file name is in the file list, and pass in the actual file name, such as 123.txt,
        /// if it exists, return true, if it does not exist, return false
        /// </summary>
        /// <param name="fileName">实际的文件名称，如果 123.txt</param>
        /// <returns>如果文件存在，返回 true, 如果不存在，返回 false</returns>
        public bool FileExists(string fileName)
        {
            bool flag = false;
            lock (this.hybirdLock)
            {
                for (int index = 0; index < this.groupFileItems.Count; ++index)
                {
                    if (this.groupFileItems[index].FileName == fileName)
                    {
                        flag = true;
                        if (!File.Exists(Path.Combine(this.dirPath, this.groupFileItems[index].MappingName)))
                        {
                            flag = false;
                            this.LogNet?.WriteError("File Check exist failed, find file in list, but mapping file not found");
                            break;
                        }
                        break;
                    }
                }
            }
            return flag;
        }

        /// <summary>
        /// 删除一个文件信息，传入文件唯一的guid的名称，例如 b35a11ec533147ca80c7f7d1713f015b7909 返回被删除的文件的guid名称<br />
        /// Delete a file information, pass in the unique GUID name of the file, for example b35a11ec533147ca80c7f7d1713f015b7909 return the GUID name of the deleted file
        /// </summary>
        /// <param name="guidName">实际的文件名称，如果 b35a11ec533147ca80c7f7d1713f015b7909</param>
        /// <returns>映射之后的文件名，例如 b35a11ec533147ca80c7f7d1713f015b7909</returns>
        public string DeleteFileByGuid(string guidName)
        {
            string str = string.Empty;
            lock (this.hybirdLock)
            {
                for (int index = 0; index < this.groupFileItems.Count; ++index)
                {
                    if (this.groupFileItems[index].MappingName == guidName)
                    {
                        str = this.groupFileItems[index].MappingName;
                        this.groupFileItems.RemoveAt(index);
                        break;
                    }
                }
            }
            this.coordinatorCacheJsonArray.StartOperaterInfomation();
            return str;
        }

        /// <summary>
        /// 删除当前目录下所有的文件信息，返回等待被删除的文件列表，是映射文件名：b35a11ec533147ca80c7f7d1713f015b7909
        /// </summary>
        /// <returns>映射之后的文件列表，例如 b35a11ec533147ca80c7f7d1713f015b7909</returns>
        public List<string> ClearAllFiles()
        {
            List<string> stringList = new List<string>();
            lock (this.hybirdLock)
            {
                for (int index = 0; index < this.groupFileItems.Count; ++index)
                    stringList.Add(this.groupFileItems[index].MappingName);
                this.groupFileItems.Clear();
            }
            this.coordinatorCacheJsonArray.StartOperaterInfomation();
            return stringList;
        }

        /// <summary>
        /// 缓存JSON文本的方法，该机制使用乐观并发模型完成<br />
        /// Method for caching JSON text, which is done using an optimistic concurrency model
        /// </summary>
        private void CacheJsonArrayContent()
        {
            lock (this.hybirdLock)
            {
                this.filesCount = this.groupFileItems.Count;
                try
                {
                    this.jsonArrayContent = JArray.FromObject((object)this.groupFileItems).ToString();
                    using (StreamWriter streamWriter = new StreamWriter(this.fileFullPath, false, Encoding.UTF8))
                    {
                        streamWriter.Write(this.jsonArrayContent);
                        streamWriter.Flush();
                    }
                }
                catch (Exception ex)
                {
                    this.LogNet?.WriteException(nameof(CacheJsonArrayContent), ex);
                }
            }
            GroupFileContainer.FileCountChangedDelegate fileCountChanged = this.FileCountChanged;
            if (fileCountChanged == null)
                return;
            fileCountChanged(this, this.filesCount);
        }

        /// <summary>
        /// 从目录进行加载数据，必须实例化的时候加载，加载失败会导致系统异常，旧的文件丢失<br />
        /// Load data from the directory, it must be loaded when instantiating. Failure to load will cause system exceptions and old files will be lost
        /// </summary>
        /// <param name="path">当前的文件夹路径信息</param>
        private void LoadByPath(string path)
        {
            this.fileFolderPath = path;
            this.fileFullPath = Path.Combine(path, "list.txt");
            if (!Directory.Exists(this.fileFolderPath))
                Directory.CreateDirectory(this.fileFolderPath);
            if (File.Exists(this.fileFullPath))
            {
                try
                {
                    using (StreamReader streamReader = new StreamReader(this.fileFullPath, Encoding.UTF8))
                        this.groupFileItems = JArray.Parse(streamReader.ReadToEnd()).ToObject<List<GroupFileItem>>();
                }
                catch (Exception ex)
                {
                    this.LogNet?.WriteException(nameof(GroupFileContainer), "Load files txt failed,", ex);
                }
            }
            if (this.groupFileItems == null)
                this.groupFileItems = new List<GroupFileItem>();
            this.coordinatorCacheJsonArray = new EstAsyncCoordinator(new Action(this.CacheJsonArrayContent));
            this.CacheJsonArrayContent();
        }

        /// <inheritdoc />
        public override string ToString() => "GroupFileContainer[" + this.dirPath + "]";

        /// <summary>文件数量变化的委托信息</summary>
        /// <param name="container">文件列表容器</param>
        /// <param name="fileCount">文件的数量</param>
        public delegate void FileCountChangedDelegate(GroupFileContainer container, int fileCount);
    }
}
