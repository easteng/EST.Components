// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Enthernet.UltimateFileServer
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Core;
using ESTCore.Common.Core.Net;
using ESTCore.Common.LogNet;
using ESTCore.Common.Reflection;

using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ESTCore.Common.Enthernet
{
    /// <summary>
    /// 一个终极文件管理服务器，可以实现对所有的文件分类管理，本服务器支持读写分离，支持同名文件，
    /// 客户端使用<see cref="T:ESTCore.Common.Enthernet.IntegrationFileClient" />进行访问，支持上传，下载，删除，请求文件列表，校验文件是否存在操作。<br />
    /// An ultimate file management server, which can realize classified management of all files. This server supports read-write separation,
    /// supports files with the same name, and the client uses <see cref="T:ESTCore.Common.Enthernet.IntegrationFileClient" /> to access,
    /// supports upload, download, delete, and request files List, check whether the file exists operation.
    /// </summary>
    /// <remarks>
    /// 本文件的服务器支持存储文件携带上传人的信息，备注信息，文件名被映射成了新的名称，无法在服务器直接查看文件信息。
    /// </remarks>
    /// <example>
    /// 以下的示例来自Demo项目，创建了一个简单的服务器对象。
    /// <code lang="cs" source="TestProject\FileNetServer\FormFileServer.cs" region="Ultimate Server" title="UltimateFileServer示例" />
    /// </example>
    public class UltimateFileServer : NetworkFileServerBase
    {
        /// <summary>所有文件组操作的词典锁</summary>
        internal Dictionary<string, GroupFileContainer> m_dictionary_group_marks = new Dictionary<string, GroupFileContainer>();
        /// <summary>词典的锁</summary>
        private SimpleHybirdLock hybirdLock = new SimpleHybirdLock();

        /// <summary>
        /// 获取当前的针对文件夹的文件管理容器的数量<br />
        /// Get the current number of file management containers for the folder
        /// </summary>
        [EstMqttApi(Description = "Get the current number of file management containers for the folder")]
        public int GroupFileContainerCount() => this.m_dictionary_group_marks.Count;

        /// <summary>
        /// 获取当前目录的文件列表管理容器，如果没有会自动创建，通过该容器可以实现对当前目录的文件进行访问<br />
        /// Get the file list management container of the current directory. If not, it will be created automatically.
        /// Through this container, you can access files in the current directory.
        /// </summary>
        /// <param name="filePath">路径信息</param>
        /// <returns>文件管理容器信息</returns>
        public GroupFileContainer GetGroupFromFilePath(string filePath)
        {
            filePath = filePath.ToUpper();
            this.hybirdLock.Enter();
            GroupFileContainer groupFileContainer;
            if (this.m_dictionary_group_marks.ContainsKey(filePath))
            {
                groupFileContainer = this.m_dictionary_group_marks[filePath];
            }
            else
            {
                groupFileContainer = new GroupFileContainer(this.LogNet, filePath);
                this.m_dictionary_group_marks.Add(filePath, groupFileContainer);
            }
            this.hybirdLock.Leave();
            return groupFileContainer;
        }

        /// <summary>从套接字接收文件并保存，更新文件列表</summary>
        /// <param name="socket">套接字</param>
        /// <param name="savename">保存的文件名</param>
        /// <returns>是否成功的结果对象</returns>
        private OperateResult<FileBaseInfo> ReceiveFileFromSocketAndUpdateGroup(
          Socket socket,
          string savename)
        {
            FileInfo fileInfo = new FileInfo(savename);
            string randomFileName = this.CreateRandomFileName();
            string str = Path.Combine(fileInfo.DirectoryName, randomFileName);
            OperateResult<FileBaseInfo> fileFromSocket = this.ReceiveFileFromSocket(socket, str, (Action<long, long>)null);
            if (!fileFromSocket.IsSuccess)
            {
                this.DeleteFileByName(str);
                return fileFromSocket;
            }
            string fileName = this.GetGroupFromFilePath(fileInfo.DirectoryName).UpdateFileMappingName(fileInfo.Name, fileFromSocket.Content.Size, randomFileName, fileFromSocket.Content.Upload, fileFromSocket.Content.Tag);
            this.DeleteExsistingFile(fileInfo.DirectoryName, fileName);
            OperateResult result = this.SendStringAndCheckReceive(socket, 1, StringResources.Language.SuccessText);
            return !result.IsSuccess ? OperateResult.CreateFailedResult<FileBaseInfo>(result) : OperateResult.CreateSuccessResult<FileBaseInfo>(fileFromSocket.Content);
        }

        /// <summary>从套接字接收文件并保存，更新文件列表</summary>
        /// <param name="socket">套接字</param>
        /// <param name="savename">保存的文件名</param>
        /// <returns>是否成功的结果对象</returns>
        private async Task<OperateResult<FileBaseInfo>> ReceiveFileFromSocketAndUpdateGroupAsync(
          Socket socket,
          string savename)
        {
            FileInfo info = new FileInfo(savename);
            string guidName = this.CreateRandomFileName();
            string fileName = Path.Combine(info.DirectoryName, guidName);
            OperateResult<FileBaseInfo> receive = await this.ReceiveFileFromSocketAsync(socket, fileName, (Action<long, long>)null);
            if (!receive.IsSuccess)
            {
                this.DeleteFileByName(fileName);
                return receive;
            }
            GroupFileContainer fileManagment = this.GetGroupFromFilePath(info.DirectoryName);
            string oldName = fileManagment.UpdateFileMappingName(info.Name, receive.Content.Size, guidName, receive.Content.Upload, receive.Content.Tag);
            this.DeleteExsistingFile(info.DirectoryName, oldName);
            OperateResult sendBack = await this.SendStringAndCheckReceiveAsync(socket, 1, StringResources.Language.SuccessText);
            return sendBack.IsSuccess ? OperateResult.CreateSuccessResult<FileBaseInfo>(receive.Content) : OperateResult.CreateFailedResult<FileBaseInfo>(sendBack);
        }

        /// <summary>
        /// 根据文件的显示名称转化为真实存储的名称，例如 123.txt 获取到在文件服务器里映射的文件名称，例如返回 b35a11ec533147ca80c7f7d1713f015b7909
        /// </summary>
        /// <param name="factory">第一大类</param>
        /// <param name="group">第二大类</param>
        /// <param name="id">第三大类</param>
        /// <param name="fileName">文件显示名称</param>
        /// <returns>是否成功的结果对象</returns>
        private string TransformFactFileName(string factory, string group, string id, string fileName) => this.GetGroupFromFilePath(this.ReturnAbsoluteFilePath(factory, group, id)).GetCurrentFileMappingName(fileName);

        /// <summary>
        /// 删除已经存在的文件信息，文件的名称需要是guid名称，例如 b35a11ec533147ca80c7f7d1713f015b7909
        /// </summary>
        /// <param name="path">文件的路径</param>
        /// <param name="fileName">文件的guid名称，例如 b35a11ec533147ca80c7f7d1713f015b7909</param>
        private void DeleteExsistingFile(string path, string fileName) => this.DeleteExsistingFile(path, new List<string>()
    {
      fileName
    });

        /// <summary>
        /// 删除已经存在的文件信息，文件的名称需要是guid名称，例如 b35a11ec533147ca80c7f7d1713f015b7909
        /// </summary>
        /// <param name="path">文件的路径</param>
        /// <param name="fileNames">文件的guid名称，例如 b35a11ec533147ca80c7f7d1713f015b7909</param>
        private void DeleteExsistingFile(string path, List<string> fileNames)
        {
            foreach (string fileName in fileNames)
            {
                if (!string.IsNullOrEmpty(fileName))
                {
                    string fileUltimatePath = Path.Combine(path, fileName);
                    this.GetFileMarksFromDictionaryWithFileName(fileName).AddOperation((Action)(() =>
                   {
                       if (!this.DeleteFileByName(fileUltimatePath))
                           this.LogNet?.WriteInfo(this.ToString(), StringResources.Language.FileDeleteFailed + fileUltimatePath);
                       else
                           this.LogNet?.WriteInfo(this.ToString(), StringResources.Language.FileDeleteSuccess + fileUltimatePath);
                   }));
                }
            }
        }

        /// <inheritdoc />
        protected override async void ThreadPoolLogin(Socket socket, IPEndPoint endPoint)
        {
            string IpAddress = endPoint.Address.ToString();
            OperateResult<FileGroupInfo> infoResult = await this.ReceiveInformationHeadAsync(socket);
            string Factory;
            string Group;
            string Identify;
            string fileName;
            string relativeName;
            if (!infoResult.IsSuccess)
            {
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
                        string guidName = this.TransformFactFileName(Factory, Group, Identify, fileName);
                        FileMarkId fileMarkId = this.GetFileMarksFromDictionaryWithFileName(guidName);
                        fileMarkId.EnterReadOperator();
                        OperateResult send = await this.SendFileAndCheckReceiveAsync(socket, this.ReturnAbsoluteFileName(Factory, Group, Identify, guidName), fileName, "", "");
                        if (!send.IsSuccess)
                        {
                            fileMarkId.LeaveReadOperator();
                            ILogNet logNet = this.LogNet;
                            if (logNet == null)
                            {
                                IpAddress = (string)null;
                                infoResult = (OperateResult<FileGroupInfo>)null;
                                Factory = (string)null;
                                Group = (string)null;
                                Identify = (string)null;
                                fileName = (string)null;
                                relativeName = (string)null;
                                break;
                            }
                            logNet.WriteError(this.ToString(), StringResources.Language.FileDownloadFailed + " : " + send.Message + " :" + relativeName + " ip:" + IpAddress);
                            IpAddress = (string)null;
                            infoResult = (OperateResult<FileGroupInfo>)null;
                            Factory = (string)null;
                            Group = (string)null;
                            Identify = (string)null;
                            fileName = (string)null;
                            relativeName = (string)null;
                            break;
                        }
                        this.LogNet?.WriteInfo(this.ToString(), StringResources.Language.FileDownloadSuccess + ":" + relativeName);
                        fileMarkId.LeaveReadOperator();
                        socket?.Close();
                        guidName = (string)null;
                        fileMarkId = (FileMarkId)null;
                        send = (OperateResult)null;
                        IpAddress = (string)null;
                        infoResult = (OperateResult<FileGroupInfo>)null;
                        Factory = (string)null;
                        Group = (string)null;
                        Identify = (string)null;
                        fileName = (string)null;
                        relativeName = (string)null;
                        break;
                    case 2002:
                        string fullFileName1 = this.ReturnAbsoluteFileName(Factory, Group, Identify, fileName);
                        this.CheckFolderAndCreate();
                        FileInfo info1 = new FileInfo(fullFileName1);
                        try
                        {
                            if (!Directory.Exists(info1.DirectoryName))
                                Directory.CreateDirectory(info1.DirectoryName);
                        }
                        catch (Exception ex)
                        {
                            this.LogNet?.WriteException(this.ToString(), StringResources.Language.FilePathCreateFailed + fullFileName1, ex);
                            Socket socket1 = socket;
                            if (socket1 == null)
                            {
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
                            IpAddress = (string)null;
                            infoResult = (OperateResult<FileGroupInfo>)null;
                            Factory = (string)null;
                            Group = (string)null;
                            Identify = (string)null;
                            fileName = (string)null;
                            relativeName = (string)null;
                            break;
                        }
                        OperateResult<FileBaseInfo> receive = await this.ReceiveFileFromSocketAndUpdateGroupAsync(socket, fullFileName1);
                        if (receive.IsSuccess)
                        {
                            socket?.Close();
                            FileServerInfo fileInfo = new FileServerInfo();
                            fileInfo.ActualFileFullName = fullFileName1;
                            fileInfo.Name = receive.Content.Name;
                            fileInfo.Size = receive.Content.Size;
                            fileInfo.Tag = receive.Content.Tag;
                            fileInfo.Upload = receive.Content.Upload;
                            this.OnFileUpload(fileInfo);
                            this.LogNet?.WriteInfo(this.ToString(), StringResources.Language.FileUploadSuccess + ":" + relativeName);
                        }
                        else
                            this.LogNet?.WriteInfo(this.ToString(), StringResources.Language.FileUploadFailed + ":" + relativeName);
                        fullFileName1 = (string)null;
                        info1 = (FileInfo)null;
                        receive = (OperateResult<FileBaseInfo>)null;
                        IpAddress = (string)null;
                        infoResult = (OperateResult<FileGroupInfo>)null;
                        Factory = (string)null;
                        Group = (string)null;
                        Identify = (string)null;
                        fileName = (string)null;
                        relativeName = (string)null;
                        break;
                    case 2003:
                        string fullFileName2 = this.ReturnAbsoluteFileName(Factory, Group, Identify, fileName);
                        FileInfo info2 = new FileInfo(fullFileName2);
                        GroupFileContainer fileManagment1 = this.GetGroupFromFilePath(info2.DirectoryName);
                        this.DeleteExsistingFile(info2.DirectoryName, fileManagment1.DeleteFile(info2.Name));
                        OperateResult operateResult1 = await this.SendStringAndCheckReceiveAsync(socket, 1, "success");
                        if (operateResult1.IsSuccess)
                        {
                            operateResult1 = (OperateResult)null;
                            socket?.Close();
                        }
                        this.LogNet?.WriteInfo(this.ToString(), StringResources.Language.FileDeleteSuccess + ":" + relativeName);
                        fullFileName2 = (string)null;
                        info2 = (FileInfo)null;
                        fileManagment1 = (GroupFileContainer)null;
                        IpAddress = (string)null;
                        infoResult = (OperateResult<FileGroupInfo>)null;
                        Factory = (string)null;
                        Group = (string)null;
                        Identify = (string)null;
                        fileName = (string)null;
                        relativeName = (string)null;
                        break;
                    case 2007:
                        GroupFileContainer fileManagment2 = this.GetGroupFromFilePath(this.ReturnAbsoluteFilePath(Factory, Group, Identify));
                        OperateResult operateResult2 = await this.SendStringAndCheckReceiveAsync(socket, 2007, fileManagment2.JsonArrayContent);
                        if (operateResult2.IsSuccess)
                        {
                            operateResult2 = (OperateResult)null;
                            socket?.Close();
                        }
                        fileManagment2 = (GroupFileContainer)null;
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
                        string[] strArray1 = this.GetDirectories(Factory, Group, Identify);
                        for (int index = 0; index < strArray1.Length; ++index)
                        {
                            string m = strArray1[index];
                            DirectoryInfo directory = new DirectoryInfo(m);
                            folders.Add(directory.Name);
                            directory = (DirectoryInfo)null;
                            m = (string)null;
                        }
                        strArray1 = (string[])null;
                        JArray jArray = JArray.FromObject((object)folders.ToArray());
                        OperateResult operateResult3 = await this.SendStringAndCheckReceiveAsync(socket, 2007, jArray.ToString());
                        if (operateResult3.IsSuccess)
                        {
                            operateResult3 = (OperateResult)null;
                            socket?.Close();
                        }
                        folders = (List<string>)null;
                        jArray = (JArray)null;
                        IpAddress = (string)null;
                        infoResult = (OperateResult<FileGroupInfo>)null;
                        Factory = (string)null;
                        Group = (string)null;
                        Identify = (string)null;
                        fileName = (string)null;
                        relativeName = (string)null;
                        break;
                    case 2011:
                        string[] strArray2 = infoResult.Content.FileNames;
                        for (int index = 0; index < strArray2.Length; ++index)
                        {
                            string item = strArray2[index];
                            string fullFileName3 = this.ReturnAbsoluteFileName(Factory, Group, Identify, item);
                            FileInfo info3 = new FileInfo(fullFileName3);
                            GroupFileContainer fileManagment3 = this.GetGroupFromFilePath(info3.DirectoryName);
                            this.DeleteExsistingFile(info3.DirectoryName, fileManagment3.DeleteFile(info3.Name));
                            relativeName = this.GetRelativeFileName(Factory, Group, Identify, fileName);
                            this.LogNet?.WriteInfo(this.ToString(), StringResources.Language.FileDeleteSuccess + ":" + relativeName);
                            fullFileName3 = (string)null;
                            info3 = (FileInfo)null;
                            fileManagment3 = (GroupFileContainer)null;
                            item = (string)null;
                        }
                        strArray2 = (string[])null;
                        OperateResult operateResult4 = await this.SendStringAndCheckReceiveAsync(socket, 1, "success");
                        if (!operateResult4.IsSuccess)
                        {
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
                        Socket socket2 = socket;
                        if (socket2 == null)
                        {
                            IpAddress = (string)null;
                            infoResult = (OperateResult<FileGroupInfo>)null;
                            Factory = (string)null;
                            Group = (string)null;
                            Identify = (string)null;
                            fileName = (string)null;
                            relativeName = (string)null;
                            break;
                        }
                        socket2.Close();
                        IpAddress = (string)null;
                        infoResult = (OperateResult<FileGroupInfo>)null;
                        Factory = (string)null;
                        Group = (string)null;
                        Identify = (string)null;
                        fileName = (string)null;
                        relativeName = (string)null;
                        break;
                    case 2012:
                        string fullFileName4 = this.ReturnAbsoluteFileName(Factory, Group, Identify, "123.txt");
                        FileInfo info4 = new FileInfo(fullFileName4);
                        GroupFileContainer fileManagment4 = this.GetGroupFromFilePath(info4.DirectoryName);
                        this.DeleteExsistingFile(info4.DirectoryName, fileManagment4.ClearAllFiles());
                        OperateResult operateResult5 = await this.SendStringAndCheckReceiveAsync(socket, 1, "success");
                        if (operateResult5.IsSuccess)
                        {
                            operateResult5 = (OperateResult)null;
                            socket?.Close();
                        }
                        this.LogNet?.WriteInfo(this.ToString(), "FolderDelete : " + relativeName);
                        fullFileName4 = (string)null;
                        info4 = (FileInfo)null;
                        fileManagment4 = (GroupFileContainer)null;
                        IpAddress = (string)null;
                        infoResult = (OperateResult<FileGroupInfo>)null;
                        Factory = (string)null;
                        Group = (string)null;
                        Identify = (string)null;
                        fileName = (string)null;
                        relativeName = (string)null;
                        break;
                    case 2013:
                        string fullPath = this.ReturnAbsoluteFilePath(Factory, Group, Identify);
                        GroupFileContainer fileManagment5 = this.GetGroupFromFilePath(fullPath);
                        bool isExists = fileManagment5.FileExists(fileName);
                        OperateResult operateResult6 = await this.SendStringAndCheckReceiveAsync(socket, isExists ? 1 : 0, StringResources.Language.FileNotExist);
                        if (operateResult6.IsSuccess)
                        {
                            operateResult6 = (OperateResult)null;
                            socket?.Close();
                        }
                        fullPath = (string)null;
                        fileManagment5 = (GroupFileContainer)null;
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
        public override string ToString() => string.Format("UltimateFileServer[{0}]", (object)this.Port);
    }
}
