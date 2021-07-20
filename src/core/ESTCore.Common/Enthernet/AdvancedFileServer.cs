// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Enthernet.AdvancedFileServer
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Core;
using ESTCore.Common.Core.Net;
using ESTCore.Common.LogNet;

using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ESTCore.Common.Enthernet
{
    /// <summary>文件管理类服务器，负责服务器所有分类文件的管理，特点是不支持文件附加数据，但是支持直接访问文件名</summary>
    /// <remarks>
    /// 本文件的服务器不支持存储文件携带的额外信息，是直接将文件存放在服务器指定目录下的，文件名不更改，特点是服务器查看方便。
    /// </remarks>
    /// <example>
    /// 以下的示例来自Demo项目，创建了一个简单的服务器对象。
    /// <code lang="cs" source="TestProject\FileNetServer\FormFileServer.cs" region="Advanced Server" title="AdvancedFileServer示例" />
    /// </example>
    public class AdvancedFileServer : NetworkFileServerBase
    {
        private string m_FilesDirectoryPathTemp = (string)null;

        /// <inheritdoc />
        protected override async void ThreadPoolLogin(Socket socket, IPEndPoint endPoint)
        {
            OperateResult result = new OperateResult();
            string IpAddress = endPoint.Address.ToString();
            OperateResult<FileGroupInfo> infoResult = await this.ReceiveInformationHeadAsync(socket);
            string Factory;
            string Group;
            string Identify;
            string fileName;
            string relativeName;
            if (!infoResult.IsSuccess)
            {
                result = (OperateResult)null;
                IpAddress = (string)null;
                infoResult = (OperateResult<FileGroupInfo>)null;
                Factory = (string)null;
                Group = (string)null;
                Identify = (string)null;
                fileName = (string)null;
                relativeName = (string)null;
            }
            else
            {
                int customer = infoResult.Content.Command;
                Factory = infoResult.Content.Factory;
                Group = infoResult.Content.Group;
                Identify = infoResult.Content.Identify;
                fileName = infoResult.Content.FileName;
                relativeName = this.GetRelativeFileName(Factory, Group, Identify, fileName);
                switch (customer)
                {
                    case 2001:
                        string fullFileName1 = this.ReturnAbsoluteFileName(Factory, Group, Identify, fileName);
                        OperateResult sendFile = await this.SendFileAndCheckReceiveAsync(socket, fullFileName1, fileName, "", "");
                        if (!sendFile.IsSuccess)
                        {
                            ILogNet logNet = this.LogNet;
                            if (logNet == null)
                            {
                                result = (OperateResult)null;
                                IpAddress = (string)null;
                                infoResult = (OperateResult<FileGroupInfo>)null;
                                Factory = (string)null;
                                Group = (string)null;
                                Identify = (string)null;
                                fileName = (string)null;
                                relativeName = (string)null;
                                break;
                            }
                            logNet.WriteError(this.ToString(), StringResources.Language.FileDownloadFailed + ":" + relativeName + " ip:" + IpAddress + " reason：" + sendFile.Message);
                            result = (OperateResult)null;
                            IpAddress = (string)null;
                            infoResult = (OperateResult<FileGroupInfo>)null;
                            Factory = (string)null;
                            Group = (string)null;
                            Identify = (string)null;
                            fileName = (string)null;
                            relativeName = (string)null;
                            break;
                        }
                        socket?.Close();
                        this.LogNet?.WriteInfo(this.ToString(), StringResources.Language.FileDownloadSuccess + ":" + relativeName);
                        fullFileName1 = (string)null;
                        sendFile = (OperateResult)null;
                        result = (OperateResult)null;
                        IpAddress = (string)null;
                        infoResult = (OperateResult<FileGroupInfo>)null;
                        Factory = (string)null;
                        Group = (string)null;
                        Identify = (string)null;
                        fileName = (string)null;
                        relativeName = (string)null;
                        break;
                    case 2002:
                        string tempFileName = Path.Combine(this.FilesDirectoryPathTemp, this.CreateRandomFileName());
                        string fullFileName2 = this.ReturnAbsoluteFileName(Factory, Group, Identify, fileName);
                        this.CheckFolderAndCreate();
                        try
                        {
                            FileInfo info = new FileInfo(fullFileName2);
                            if (!Directory.Exists(info.DirectoryName))
                                Directory.CreateDirectory(info.DirectoryName);
                            info = (FileInfo)null;
                        }
                        catch (Exception ex)
                        {
                            this.LogNet?.WriteException(this.ToString(), StringResources.Language.FilePathCreateFailed + fullFileName2, ex);
                            Socket socket1 = socket;
                            if (socket1 == null)
                            {
                                result = (OperateResult)null;
                                IpAddress = (string)null;
                                infoResult = (OperateResult<FileGroupInfo>)null;
                                Factory = (string)null;
                                Group = (string)null;
                                Identify = (string)null;
                                fileName = (string)null;
                                relativeName = (string)null;
                                break;
                            }
                            socket1.Close();
                            result = (OperateResult)null;
                            IpAddress = (string)null;
                            infoResult = (OperateResult<FileGroupInfo>)null;
                            Factory = (string)null;
                            Group = (string)null;
                            Identify = (string)null;
                            fileName = (string)null;
                            relativeName = (string)null;
                            break;
                        }
                        OperateResult<FileBaseInfo> receiveFile = await this.ReceiveFileFromSocketAndMoveFileAsync(socket, tempFileName, fullFileName2);
                        if (receiveFile.IsSuccess)
                        {
                            socket?.Close();
                            FileServerInfo fileInfo = new FileServerInfo();
                            fileInfo.ActualFileFullName = fullFileName2;
                            fileInfo.Name = receiveFile.Content.Name;
                            fileInfo.Size = receiveFile.Content.Size;
                            fileInfo.Tag = receiveFile.Content.Tag;
                            fileInfo.Upload = receiveFile.Content.Upload;
                            this.OnFileUpload(fileInfo);
                            this.LogNet?.WriteInfo(this.ToString(), StringResources.Language.FileUploadSuccess + ":" + relativeName);
                        }
                        else
                            this.LogNet?.WriteInfo(this.ToString(), StringResources.Language.FileUploadFailed + ":" + relativeName + " " + StringResources.Language.TextDescription + receiveFile.Message);
                        tempFileName = (string)null;
                        fullFileName2 = (string)null;
                        receiveFile = (OperateResult<FileBaseInfo>)null;
                        result = (OperateResult)null;
                        IpAddress = (string)null;
                        infoResult = (OperateResult<FileGroupInfo>)null;
                        Factory = (string)null;
                        Group = (string)null;
                        Identify = (string)null;
                        fileName = (string)null;
                        relativeName = (string)null;
                        break;
                    case 2003:
                        string fullFileName3 = this.ReturnAbsoluteFileName(Factory, Group, Identify, fileName);
                        bool deleteResult1 = this.DeleteFileByName(fullFileName3);
                        OperateResult operateResult1 = await this.SendStringAndCheckReceiveAsync(socket, deleteResult1 ? 1 : 0, deleteResult1 ? StringResources.Language.FileDeleteSuccess : StringResources.Language.FileDeleteFailed);
                        if (operateResult1.IsSuccess)
                        {
                            operateResult1 = (OperateResult)null;
                            socket?.Close();
                        }
                        if (deleteResult1)
                            this.LogNet?.WriteInfo(this.ToString(), StringResources.Language.FileDeleteSuccess + ":" + relativeName);
                        fullFileName3 = (string)null;
                        result = (OperateResult)null;
                        IpAddress = (string)null;
                        infoResult = (OperateResult<FileGroupInfo>)null;
                        Factory = (string)null;
                        Group = (string)null;
                        Identify = (string)null;
                        fileName = (string)null;
                        relativeName = (string)null;
                        break;
                    case 2007:
                        List<GroupFileItem> fileNames = new List<GroupFileItem>();
                        string[] strArray1 = this.GetDirectoryFiles(Factory, Group, Identify);
                        for (int index = 0; index < strArray1.Length; ++index)
                        {
                            string m = strArray1[index];
                            FileInfo fileInfo = new FileInfo(m);
                            fileNames.Add(new GroupFileItem()
                            {
                                FileName = fileInfo.Name,
                                FileSize = fileInfo.Length
                            });
                            fileInfo = (FileInfo)null;
                            m = (string)null;
                        }
                        strArray1 = (string[])null;
                        JArray jArray1 = JArray.FromObject((object)fileNames.ToArray());
                        OperateResult operateResult2 = await this.SendStringAndCheckReceiveAsync(socket, 2007, jArray1.ToString());
                        if (operateResult2.IsSuccess)
                        {
                            operateResult2 = (OperateResult)null;
                            socket?.Close();
                        }
                        fileNames = (List<GroupFileItem>)null;
                        jArray1 = (JArray)null;
                        result = (OperateResult)null;
                        IpAddress = (string)null;
                        infoResult = (OperateResult<FileGroupInfo>)null;
                        Factory = (string)null;
                        Group = (string)null;
                        Identify = (string)null;
                        fileName = (string)null;
                        relativeName = (string)null;
                        break;
                    case 2008:
                        List<string> folders = new List<string>();
                        string[] strArray2 = this.GetDirectories(Factory, Group, Identify);
                        for (int index = 0; index < strArray2.Length; ++index)
                        {
                            string m = strArray2[index];
                            DirectoryInfo directory = new DirectoryInfo(m);
                            folders.Add(directory.Name);
                            directory = (DirectoryInfo)null;
                            m = (string)null;
                        }
                        strArray2 = (string[])null;
                        JArray jArray2 = JArray.FromObject((object)folders.ToArray());
                        OperateResult operateResult3 = await this.SendStringAndCheckReceiveAsync(socket, 2007, jArray2.ToString());
                        if (operateResult3.IsSuccess)
                        {
                            operateResult3 = (OperateResult)null;
                            socket?.Close();
                        }
                        folders = (List<string>)null;
                        jArray2 = (JArray)null;
                        result = (OperateResult)null;
                        IpAddress = (string)null;
                        infoResult = (OperateResult<FileGroupInfo>)null;
                        Factory = (string)null;
                        Group = (string)null;
                        Identify = (string)null;
                        fileName = (string)null;
                        relativeName = (string)null;
                        break;
                    case 2011:
                        bool deleteResult2 = true;
                        string[] strArray3 = infoResult.Content.FileNames;
                        for (int index = 0; index < strArray3.Length; ++index)
                        {
                            string item = strArray3[index];
                            string fullFileName4 = this.ReturnAbsoluteFileName(Factory, Group, Identify, item);
                            deleteResult2 = this.DeleteFileByName(fullFileName4);
                            if (deleteResult2)
                            {
                                this.LogNet?.WriteInfo(this.ToString(), StringResources.Language.FileDeleteSuccess + ":" + relativeName);
                                fullFileName4 = (string)null;
                                item = (string)null;
                            }
                            else
                            {
                                deleteResult2 = false;
                                break;
                            }
                        }
                        strArray3 = (string[])null;
                        OperateResult operateResult4 = await this.SendStringAndCheckReceiveAsync(socket, deleteResult2 ? 1 : 0, deleteResult2 ? StringResources.Language.FileDeleteSuccess : StringResources.Language.FileDeleteFailed);
                        if (!operateResult4.IsSuccess)
                        {
                            result = (OperateResult)null;
                            IpAddress = (string)null;
                            infoResult = (OperateResult<FileGroupInfo>)null;
                            Factory = (string)null;
                            Group = (string)null;
                            Identify = (string)null;
                            fileName = (string)null;
                            relativeName = (string)null;
                            break;
                        }
                        operateResult4 = (OperateResult)null;
                        socket?.Close();
                        result = (OperateResult)null;
                        IpAddress = (string)null;
                        infoResult = (OperateResult<FileGroupInfo>)null;
                        Factory = (string)null;
                        Group = (string)null;
                        Identify = (string)null;
                        fileName = (string)null;
                        relativeName = (string)null;
                        break;
                    case 2012:
                        string fullPath = this.ReturnAbsoluteFileName(Factory, Group, Identify, string.Empty);
                        DirectoryInfo info1 = new DirectoryInfo(fullPath);
                        bool deleteResult3 = false;
                        try
                        {
                            if (info1.Exists)
                                info1.Delete(true);
                            deleteResult3 = true;
                        }
                        catch (Exception ex)
                        {
                            this.LogNet?.WriteInfo(this.ToString(), StringResources.Language.FileDeleteFailed + " [" + fullPath + "] " + ex.Message);
                        }
                        OperateResult operateResult5 = await this.SendStringAndCheckReceiveAsync(socket, deleteResult3 ? 1 : 0, deleteResult3 ? StringResources.Language.FileDeleteSuccess : StringResources.Language.FileDeleteFailed);
                        if (operateResult5.IsSuccess)
                        {
                            operateResult5 = (OperateResult)null;
                            socket?.Close();
                        }
                        if (deleteResult3)
                            this.LogNet?.WriteInfo(this.ToString(), StringResources.Language.FileDeleteSuccess + ":" + fullPath);
                        fullPath = (string)null;
                        info1 = (DirectoryInfo)null;
                        result = (OperateResult)null;
                        IpAddress = (string)null;
                        infoResult = (OperateResult<FileGroupInfo>)null;
                        Factory = (string)null;
                        Group = (string)null;
                        Identify = (string)null;
                        fileName = (string)null;
                        relativeName = (string)null;
                        break;
                    case 2013:
                        string fullFileName5 = this.ReturnAbsoluteFileName(Factory, Group, Identify, fileName);
                        bool isExists = System.IO.File.Exists(fullFileName5);
                        OperateResult operateResult6 = await this.SendStringAndCheckReceiveAsync(socket, isExists ? 1 : 0, StringResources.Language.FileNotExist);
                        if (operateResult6.IsSuccess)
                        {
                            operateResult6 = (OperateResult)null;
                            socket?.Close();
                        }
                        fullFileName5 = (string)null;
                        result = (OperateResult)null;
                        IpAddress = (string)null;
                        infoResult = (OperateResult<FileGroupInfo>)null;
                        Factory = (string)null;
                        Group = (string)null;
                        Identify = (string)null;
                        fileName = (string)null;
                        relativeName = (string)null;
                        break;
                    default:
                        socket?.Close();
                        result = (OperateResult)null;
                        IpAddress = (string)null;
                        infoResult = (OperateResult<FileGroupInfo>)null;
                        Factory = (string)null;
                        Group = (string)null;
                        Identify = (string)null;
                        fileName = (string)null;
                        relativeName = (string)null;
                        break;
                }
            }
        }

        /// <inheritdoc />
        protected override void StartInitialization()
        {
            if (string.IsNullOrEmpty(this.FilesDirectoryPathTemp))
                throw new ArgumentNullException("FilesDirectoryPathTemp", "No saved path is specified");
            base.StartInitialization();
        }

        /// <inheritdoc />
        protected override void CheckFolderAndCreate()
        {
            if (!Directory.Exists(this.FilesDirectoryPathTemp))
                Directory.CreateDirectory(this.FilesDirectoryPathTemp);
            base.CheckFolderAndCreate();
        }

        /// <summary>从网络套接字接收文件并移动到目标的文件夹中，如果结果异常，则结束通讯</summary>
        /// <param name="socket"></param>
        /// <param name="savename"></param>
        /// <param name="fileNameNew"></param>
        /// <returns></returns>
        private OperateResult<FileBaseInfo> ReceiveFileFromSocketAndMoveFile(
          Socket socket,
          string savename,
          string fileNameNew)
        {
            OperateResult<FileBaseInfo> fileFromSocket = this.ReceiveFileFromSocket(socket, savename, (Action<long, long>)null);
            if (!fileFromSocket.IsSuccess)
            {
                this.DeleteFileByName(savename);
                return OperateResult.CreateFailedResult<FileBaseInfo>((OperateResult)fileFromSocket);
            }
            int customer = 0;
            int num = 0;
            while (num < 3)
            {
                ++num;
                if (this.MoveFileToNewFile(savename, fileNameNew))
                {
                    customer = 1;
                    break;
                }
                Thread.Sleep(500);
            }
            if (customer == 0)
                this.DeleteFileByName(savename);
            OperateResult result = this.SendStringAndCheckReceive(socket, customer, "success");
            return !result.IsSuccess ? OperateResult.CreateFailedResult<FileBaseInfo>(result) : OperateResult.CreateSuccessResult<FileBaseInfo>(fileFromSocket.Content);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.AdvancedFileServer.ReceiveFileFromSocketAndMoveFile(System.Net.Sockets.Socket,System.String,System.String)" />
        private async Task<OperateResult<FileBaseInfo>> ReceiveFileFromSocketAndMoveFileAsync(
          Socket socket,
          string savename,
          string fileNameNew)
        {
            OperateResult<FileBaseInfo> fileInfo = await this.ReceiveFileFromSocketAsync(socket, savename, (Action<long, long>)null);
            if (!fileInfo.IsSuccess)
            {
                this.DeleteFileByName(savename);
                return OperateResult.CreateFailedResult<FileBaseInfo>((OperateResult)fileInfo);
            }
            int customer = 0;
            int times = 0;
            while (times < 3)
            {
                ++times;
                if (this.MoveFileToNewFile(savename, fileNameNew))
                {
                    customer = 1;
                    break;
                }
                Thread.Sleep(500);
            }
            if (customer == 0)
                this.DeleteFileByName(savename);
            OperateResult sendString = await this.SendStringAndCheckReceiveAsync(socket, customer, "success");
            return sendString.IsSuccess ? OperateResult.CreateSuccessResult<FileBaseInfo>(fileInfo.Content) : OperateResult.CreateFailedResult<FileBaseInfo>(sendString);
        }

        /// <summary>
        /// 用于接收上传文件时的临时文件夹，临时文件使用结束后会被删除<br />
        /// Used to receive the temporary folder when uploading files. The temporary files will be deleted after use
        /// </summary>
        public string FilesDirectoryPathTemp
        {
            get => this.m_FilesDirectoryPathTemp;
            set => this.m_FilesDirectoryPathTemp = this.PreprocessFolderName(value);
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("AdvancedFileServer[{0}]", (object)this.Port);
    }
}
