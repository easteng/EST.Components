// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Core.Net.NetworkFileServerBase
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESTCore.Common.Core.Net
{
    /// <summary>
    /// 文件服务器类的基类，为直接映射文件模式和间接映射文件模式提供基础的方法支持，主要包含了对文件的一些操作的功能<br />
    /// The base class of the file server class, which provides basic method support for the direct mapping file mode and the indirect mapping file mode, and mainly includes the functions of some operations on files
    /// </summary>
    public class NetworkFileServerBase : NetworkServerBase
    {
        private readonly Dictionary<string, FileMarkId> dictionaryFilesMarks;
        private readonly object dictHybirdLock;
        private string m_FilesDirectoryPath = (string)null;

        /// <summary>实例化一个默认的对象</summary>
        public NetworkFileServerBase()
        {
            this.dictionaryFilesMarks = new Dictionary<string, FileMarkId>(100);
            this.dictHybirdLock = new object();
        }

        /// <summary>
        /// 获取当前文件的读写锁，如果没有会自动创建，文件名应该是guid文件名，例如 b35a11ec533147ca80c7f7d1713f015b7909<br />
        /// Acquire the read-write lock of the current file. If not, it will be created automatically.
        /// The file name should be the guid file name, for example, b35a11ec533147ca80c7f7d1713f015b7909
        /// </summary>
        /// <param name="fileName">完整的文件路径</param>
        /// <returns>返回携带文件信息的读写锁</returns>
        protected FileMarkId GetFileMarksFromDictionaryWithFileName(string fileName)
        {
            FileMarkId fileMarkId;
            lock (this.dictHybirdLock)
            {
                if (this.dictionaryFilesMarks.ContainsKey(fileName))
                {
                    fileMarkId = this.dictionaryFilesMarks[fileName];
                }
                else
                {
                    fileMarkId = new FileMarkId(this.LogNet, fileName);
                    this.dictionaryFilesMarks.Add(fileName, fileMarkId);
                }
            }
            return fileMarkId;
        }

        /// <summary>接收本次操作的信息头数据</summary>
        /// <param name="socket">网络套接字</param>
        /// <returns>是否成功的结果对象</returns>
        protected OperateResult<FileGroupInfo> ReceiveInformationHead(
          Socket socket)
        {
            FileGroupInfo fileGroupInfo = new FileGroupInfo();
            OperateResult<byte[], byte[]> andCheckBytes = this.ReceiveAndCheckBytes(socket, 30000);
            if (!andCheckBytes.IsSuccess)
                return OperateResult.CreateFailedResult<FileGroupInfo>((OperateResult)andCheckBytes);
            fileGroupInfo.Command = BitConverter.ToInt32(andCheckBytes.Content1, 4);
            switch (BitConverter.ToInt32(andCheckBytes.Content1, 0))
            {
                case 1001:
                    fileGroupInfo.FileName = Encoding.Unicode.GetString(andCheckBytes.Content2);
                    break;
                case 1005:
                    fileGroupInfo.FileNames = EstProtocol.UnPackStringArrayFromByte(andCheckBytes.Content2);
                    break;
            }
            OperateResult<int, string> contentFromSocket1 = this.ReceiveStringContentFromSocket(socket);
            if (!contentFromSocket1.IsSuccess)
                return OperateResult.CreateFailedResult<FileGroupInfo>((OperateResult)contentFromSocket1);
            fileGroupInfo.Factory = contentFromSocket1.Content2;
            OperateResult<int, string> contentFromSocket2 = this.ReceiveStringContentFromSocket(socket);
            if (!contentFromSocket2.IsSuccess)
                return OperateResult.CreateFailedResult<FileGroupInfo>((OperateResult)contentFromSocket2);
            fileGroupInfo.Group = contentFromSocket2.Content2;
            OperateResult<int, string> contentFromSocket3 = this.ReceiveStringContentFromSocket(socket);
            if (!contentFromSocket3.IsSuccess)
                return OperateResult.CreateFailedResult<FileGroupInfo>((OperateResult)contentFromSocket3);
            fileGroupInfo.Identify = contentFromSocket3.Content2;
            return OperateResult.CreateSuccessResult<FileGroupInfo>(fileGroupInfo);
        }

        /// <summary>接收本次操作的信息头数据</summary>
        /// <param name="socket">网络套接字</param>
        /// <returns>是否成功的结果对象</returns>
        protected async Task<OperateResult<FileGroupInfo>> ReceiveInformationHeadAsync(
          Socket socket)
        {
            FileGroupInfo ret = new FileGroupInfo();
            OperateResult<byte[], byte[]> receive = await this.ReceiveAndCheckBytesAsync(socket, 30000);
            if (!receive.IsSuccess)
                return OperateResult.CreateFailedResult<FileGroupInfo>((OperateResult)receive);
            ret.Command = BitConverter.ToInt32(receive.Content1, 4);
            int cmd = BitConverter.ToInt32(receive.Content1, 0);
            switch (cmd)
            {
                case 1001:
                    ret.FileName = Encoding.Unicode.GetString(receive.Content2);
                    break;
                case 1005:
                    ret.FileNames = EstProtocol.UnPackStringArrayFromByte(receive.Content2);
                    break;
            }
            OperateResult<int, string> factoryResult = await this.ReceiveStringContentFromSocketAsync(socket);
            if (!factoryResult.IsSuccess)
                return OperateResult.CreateFailedResult<FileGroupInfo>((OperateResult)factoryResult);
            ret.Factory = factoryResult.Content2;
            OperateResult<int, string> groupResult = await this.ReceiveStringContentFromSocketAsync(socket);
            if (!groupResult.IsSuccess)
                return OperateResult.CreateFailedResult<FileGroupInfo>((OperateResult)groupResult);
            ret.Group = groupResult.Content2;
            OperateResult<int, string> idResult = await this.ReceiveStringContentFromSocketAsync(socket);
            if (!idResult.IsSuccess)
                return OperateResult.CreateFailedResult<FileGroupInfo>((OperateResult)idResult);
            ret.Identify = idResult.Content2;
            return OperateResult.CreateSuccessResult<FileGroupInfo>(ret);
        }

        /// <summary>获取一个随机的文件名，由GUID码和随机数字组成</summary>
        /// <returns>文件名</returns>
        protected string CreateRandomFileName() => SoftBasic.GetUniqueStringByGuidAndRandom();

        /// <summary>
        /// 返回服务器的绝对路径，包含根目录的信息  [Root Dir][Factory][Group][Id] 信息
        /// </summary>
        /// <param name="factory">第一大类</param>
        /// <param name="group">第二大类</param>
        /// <param name="id">第三大类</param>
        /// <returns>是否成功的结果对象</returns>
        protected string ReturnAbsoluteFilePath(string factory, string group, string id)
        {
            string str = this.m_FilesDirectoryPath;
            if (!string.IsNullOrEmpty(factory))
                str = str + "\\" + factory;
            if (!string.IsNullOrEmpty(group))
                str = str + "\\" + group;
            if (!string.IsNullOrEmpty(id))
                str = str + "\\" + id;
            return str;
        }

        /// <summary>
        /// 返回服务器的绝对路径，包含根目录的信息  [Root Dir][Factory][Group][Id][FileName] 信息
        /// </summary>
        /// <param name="factory">第一大类</param>
        /// <param name="group">第二大类</param>
        /// <param name="id">第三大类</param>
        /// <param name="fileName">文件名</param>
        /// <returns>是否成功的结果对象</returns>
        protected string ReturnAbsoluteFileName(
          string factory,
          string group,
          string id,
          string fileName)
        {
            return this.ReturnAbsoluteFilePath(factory, group, id) + "\\" + fileName;
        }

        /// <summary>返回相对路径的名称</summary>
        /// <param name="factory">第一大类</param>
        /// <param name="group">第二大类</param>
        /// <param name="id">第三大类</param>
        /// <param name="fileName">文件名</param>
        /// <returns>是否成功的结果对象</returns>
        protected string GetRelativeFileName(string factory, string group, string id, string fileName)
        {
            string str = "";
            if (!string.IsNullOrEmpty(factory))
                str = str + factory + "\\";
            if (!string.IsNullOrEmpty(group))
                str = str + group + "\\";
            if (!string.IsNullOrEmpty(id))
                str = str + id + "\\";
            return str + fileName;
        }

        /// <summary>移动一个文件到新的文件去</summary>
        /// <param name="fileNameOld">旧的文件名称</param>
        /// <param name="fileNameNew">新的文件名称</param>
        /// <returns>是否成功</returns>
        protected bool MoveFileToNewFile(string fileNameOld, string fileNameNew)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(fileNameNew);
                if (!Directory.Exists(fileInfo.DirectoryName))
                    Directory.CreateDirectory(fileInfo.DirectoryName);
                if (File.Exists(fileNameNew))
                    File.Delete(fileNameNew);
                File.Move(fileNameOld, fileNameNew);
                return true;
            }
            catch (Exception ex)
            {
                this.LogNet?.WriteException(this.ToString(), "Move a file to new file failed: ", ex);
                return false;
            }
        }

        /// <summary>删除文件并回发确认信息，如果结果异常，则结束通讯</summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="fullname">完整路径的文件名称</param>
        /// <returns>是否成功的结果对象</returns>
        protected OperateResult DeleteFileAndCheck(Socket socket, string fullname)
        {
            int customer = 0;
            int num = 0;
            while (num < 3)
            {
                ++num;
                if (this.DeleteFileByName(fullname))
                {
                    customer = 1;
                    break;
                }
                Thread.Sleep(500);
            }
            return this.SendStringAndCheckReceive(socket, customer, StringResources.Language.SuccessText);
        }

        /// <summary>文件上传的事件，当文件上传的时候触发。</summary>
        public event NetworkFileServerBase.FileUploadDelegate OnFileUploadEvent;

        /// <summary>触发一个文件上传的事件。</summary>
        /// <param name="fileInfo">文件的基本信息</param>
        protected void OnFileUpload(FileServerInfo fileInfo)
        {
            NetworkFileServerBase.FileUploadDelegate onFileUploadEvent = this.OnFileUploadEvent;
            if (onFileUploadEvent == null)
                return;
            onFileUploadEvent(fileInfo);
        }

        /// <summary>服务器启动时的操作</summary>
        protected override void StartInitialization()
        {
            if (string.IsNullOrEmpty(this.FilesDirectoryPath))
                throw new ArgumentNullException("FilesDirectoryPath", "No saved path is specified");
            this.CheckFolderAndCreate();
            base.StartInitialization();
        }

        /// <summary>检查文件夹是否存在，不存在就创建</summary>
        protected virtual void CheckFolderAndCreate()
        {
            if (Directory.Exists(this.FilesDirectoryPath))
                return;
            Directory.CreateDirectory(this.FilesDirectoryPath);
        }

        /// <summary>文件所存储的路径</summary>
        public string FilesDirectoryPath
        {
            get => this.m_FilesDirectoryPath;
            set => this.m_FilesDirectoryPath = this.PreprocessFolderName(value);
        }

        /// <summary>
        /// 获取当前的文件标记的对象数量<br />
        /// Get the number of objects marked by the current file
        /// </summary>
        public int FileMarkIdCount => this.dictionaryFilesMarks.Count;

        /// <inheritdoc cref="F:ESTCore.Common.Core.Net.NetworkBase.fileCacheSize" />
        public int FileCacheSize
        {
            get => this.fileCacheSize;
            set => this.fileCacheSize = value;
        }

        /// <summary>获取文件夹的所有文件列表</summary>
        /// <param name="factory">第一大类</param>
        /// <param name="group">第二大类</param>
        /// <param name="id">第三大类</param>
        /// <returns>文件列表</returns>
        public virtual string[] GetDirectoryFiles(string factory, string group, string id)
        {
            if (string.IsNullOrEmpty(this.FilesDirectoryPath))
                return new string[0];
            string path = this.ReturnAbsoluteFilePath(factory, group, id);
            return !Directory.Exists(path) ? new string[0] : Directory.GetFiles(path);
        }

        /// <summary>获取文件夹的所有文件夹列表</summary>
        /// <param name="factory">第一大类</param>
        /// <param name="group">第二大类</param>
        /// <param name="id">第三大类</param>
        /// <returns>文件夹列表</returns>
        public string[] GetDirectories(string factory, string group, string id)
        {
            if (string.IsNullOrEmpty(this.FilesDirectoryPath))
                return new string[0];
            string path = this.ReturnAbsoluteFilePath(factory, group, id);
            return !Directory.Exists(path) ? new string[0] : Directory.GetDirectories(path);
        }

        /// <inheritdoc />
        public override string ToString() => nameof(NetworkFileServerBase);

        /// <summary>文件上传的委托</summary>
        /// <param name="fileInfo">文件的基本信息</param>
        public delegate void FileUploadDelegate(FileServerInfo fileInfo);
    }
}
