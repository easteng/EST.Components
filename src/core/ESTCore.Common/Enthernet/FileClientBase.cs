// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Enthernet.FileClientBase
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Core;
using ESTCore.Common.Core.Net;

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ESTCore.Common.Enthernet
{
    /// <summary>
    /// 文件传输客户端基类，提供上传，下载，删除的基础服务<br />
    /// File transfer client base class, providing basic services for uploading, downloading, and deleting
    /// </summary>
    public abstract class FileClientBase : NetworkXBase
    {
        private IPEndPoint m_ipEndPoint = (IPEndPoint)null;

        /// <summary>
        /// 文件管理服务器的ip地址及端口<br />
        /// IP address and port of the file management server
        /// </summary>
        public IPEndPoint ServerIpEndPoint
        {
            get => this.m_ipEndPoint;
            set => this.m_ipEndPoint = value;
        }

        /// <summary>
        /// 获取或设置连接的超时时间，默认10秒<br />
        /// Gets or sets the connection timeout time. The default is 10 seconds.
        /// </summary>
        public int ConnectTimeOut { get; set; } = 10000;

        /// <summary>
        /// 发送三个文件分类信息到服务器端，方便后续开展其他的操作。<br />
        /// Send the three file classification information to the server to facilitate subsequent operations.
        /// </summary>
        /// <param name="socket">套接字对象</param>
        /// <param name="factory">一级分类</param>
        /// <param name="group">二级分类</param>
        /// <param name="id">三级分类</param>
        /// <returns>是否成功的结果对象</returns>
        protected OperateResult SendFactoryGroupId(
          Socket socket,
          string factory,
          string group,
          string id)
        {
            OperateResult operateResult1 = this.SendStringAndCheckReceive(socket, 1, factory);
            if (!operateResult1.IsSuccess)
                return operateResult1;
            OperateResult operateResult2 = this.SendStringAndCheckReceive(socket, 2, group);
            if (!operateResult2.IsSuccess)
                return operateResult2;
            OperateResult operateResult3 = this.SendStringAndCheckReceive(socket, 3, id);
            return !operateResult3.IsSuccess ? operateResult3 : OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.FileClientBase.SendFactoryGroupId(System.Net.Sockets.Socket,System.String,System.String,System.String)" />
        protected async Task<OperateResult> SendFactoryGroupIdAsync(
          Socket socket,
          string factory,
          string group,
          string id)
        {
            OperateResult factoryResult = await this.SendStringAndCheckReceiveAsync(socket, 1, factory);
            if (!factoryResult.IsSuccess)
                return factoryResult;
            OperateResult groupResult = await this.SendStringAndCheckReceiveAsync(socket, 2, group);
            if (!groupResult.IsSuccess)
                return groupResult;
            OperateResult idResult = await this.SendStringAndCheckReceiveAsync(socket, 3, id);
            return idResult.IsSuccess ? OperateResult.CreateSuccessResult() : idResult;
        }

        /// <summary>
        /// 删除服务器上的文件，需要传入文件信息，以及文件绑定的分类信息。<br />
        /// To delete a file on the server, you need to pass in the file information and the classification information of the file binding.
        /// </summary>
        /// <param name="fileName">文件的名称</param>
        /// <param name="factory">一级分类</param>
        /// <param name="group">二级分类</param>
        /// <param name="id">三级分类</param>
        /// <returns>是否成功的结果对象</returns>
        protected OperateResult DeleteFileBase(
          string fileName,
          string factory,
          string group,
          string id)
        {
            OperateResult<Socket> socketAndConnect = this.CreateSocketAndConnect(this.ServerIpEndPoint, this.ConnectTimeOut);
            if (!socketAndConnect.IsSuccess)
                return (OperateResult)socketAndConnect;
            OperateResult operateResult1 = this.SendStringAndCheckReceive(socketAndConnect.Content, 2003, fileName);
            if (!operateResult1.IsSuccess)
                return operateResult1;
            OperateResult operateResult2 = this.SendFactoryGroupId(socketAndConnect.Content, factory, group, id);
            if (!operateResult2.IsSuccess)
                return operateResult2;
            OperateResult<int, string> contentFromSocket = this.ReceiveStringContentFromSocket(socketAndConnect.Content);
            if (!contentFromSocket.IsSuccess)
                return (OperateResult)contentFromSocket;
            OperateResult operateResult3 = new OperateResult();
            if (contentFromSocket.Content1 == 1)
                operateResult3.IsSuccess = true;
            operateResult3.Message = contentFromSocket.Message;
            socketAndConnect.Content?.Close();
            return operateResult3;
        }

        /// <summary>
        /// 删除服务器上的文件列表，需要传入文件信息，以及文件绑定的分类信息。<br />
        /// To delete a file on the server, you need to pass in the file information and the classification information of the file binding.
        /// </summary>
        /// <param name="fileNames">所有等待删除的文件的名称</param>
        /// <param name="factory">一级分类</param>
        /// <param name="group">二级分类</param>
        /// <param name="id">三级分类</param>
        /// <returns>是否成功的结果对象</returns>
        protected OperateResult DeleteFileBase(
          string[] fileNames,
          string factory,
          string group,
          string id)
        {
            OperateResult<Socket> socketAndConnect = this.CreateSocketAndConnect(this.ServerIpEndPoint, this.ConnectTimeOut);
            if (!socketAndConnect.IsSuccess)
                return (OperateResult)socketAndConnect;
            OperateResult operateResult1 = this.SendStringAndCheckReceive(socketAndConnect.Content, 2011, fileNames);
            if (!operateResult1.IsSuccess)
                return operateResult1;
            OperateResult operateResult2 = this.SendFactoryGroupId(socketAndConnect.Content, factory, group, id);
            if (!operateResult2.IsSuccess)
                return operateResult2;
            OperateResult<int, string> contentFromSocket = this.ReceiveStringContentFromSocket(socketAndConnect.Content);
            if (!contentFromSocket.IsSuccess)
                return (OperateResult)contentFromSocket;
            OperateResult operateResult3 = new OperateResult();
            if (contentFromSocket.Content1 == 1)
                operateResult3.IsSuccess = true;
            operateResult3.Message = contentFromSocket.Message;
            socketAndConnect.Content?.Close();
            return operateResult3;
        }

        /// <summary>
        /// 删除服务器上的指定目录的所有文件，需要传入分类信息。<br />
        /// To delete all files in the specified directory on the server, you need to input classification information
        /// </summary>
        /// <param name="factory">一级分类</param>
        /// <param name="group">二级分类</param>
        /// <param name="id">三级分类</param>
        /// <returns>是否成功的结果对象</returns>
        protected OperateResult DeleteFolder(string factory, string group, string id)
        {
            OperateResult<Socket> socketAndConnect = this.CreateSocketAndConnect(this.ServerIpEndPoint, this.ConnectTimeOut);
            if (!socketAndConnect.IsSuccess)
                return (OperateResult)socketAndConnect;
            OperateResult operateResult1 = this.SendStringAndCheckReceive(socketAndConnect.Content, 2012, "");
            if (!operateResult1.IsSuccess)
                return operateResult1;
            OperateResult operateResult2 = this.SendFactoryGroupId(socketAndConnect.Content, factory, group, id);
            if (!operateResult2.IsSuccess)
                return operateResult2;
            OperateResult<int, string> contentFromSocket = this.ReceiveStringContentFromSocket(socketAndConnect.Content);
            if (!contentFromSocket.IsSuccess)
                return (OperateResult)contentFromSocket;
            OperateResult operateResult3 = new OperateResult();
            if (contentFromSocket.Content1 == 1)
                operateResult3.IsSuccess = true;
            operateResult3.Message = contentFromSocket.Message;
            socketAndConnect.Content?.Close();
            return operateResult3;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.FileClientBase.DeleteFileBase(System.String,System.String,System.String,System.String)" />
        protected async Task<OperateResult> DeleteFileBaseAsync(
          string fileName,
          string factory,
          string group,
          string id)
        {
            OperateResult<Socket> socketResult = await this.CreateSocketAndConnectAsync(this.ServerIpEndPoint, this.ConnectTimeOut);
            if (!socketResult.IsSuccess)
                return (OperateResult)socketResult;
            OperateResult sendString = await this.SendStringAndCheckReceiveAsync(socketResult.Content, 2003, fileName);
            if (!sendString.IsSuccess)
                return sendString;
            OperateResult sendFileInfo = await this.SendFactoryGroupIdAsync(socketResult.Content, factory, group, id);
            if (!sendFileInfo.IsSuccess)
                return sendFileInfo;
            OperateResult<int, string> receiveBack = await this.ReceiveStringContentFromSocketAsync(socketResult.Content);
            if (!receiveBack.IsSuccess)
                return (OperateResult)receiveBack;
            OperateResult result = new OperateResult();
            if (receiveBack.Content1 == 1)
                result.IsSuccess = true;
            result.Message = receiveBack.Message;
            socketResult.Content?.Close();
            return result;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.FileClientBase.DeleteFileBase(System.String[],System.String,System.String,System.String)" />
        protected async Task<OperateResult> DeleteFileBaseAsync(
          string[] fileNames,
          string factory,
          string group,
          string id)
        {
            OperateResult<Socket> socketResult = await this.CreateSocketAndConnectAsync(this.ServerIpEndPoint, this.ConnectTimeOut);
            if (!socketResult.IsSuccess)
                return (OperateResult)socketResult;
            OperateResult sendString = await this.SendStringAndCheckReceiveAsync(socketResult.Content, 2011, fileNames);
            if (!sendString.IsSuccess)
                return sendString;
            OperateResult sendFileInfo = await this.SendFactoryGroupIdAsync(socketResult.Content, factory, group, id);
            if (!sendFileInfo.IsSuccess)
                return sendFileInfo;
            OperateResult<int, string> receiveBack = await this.ReceiveStringContentFromSocketAsync(socketResult.Content);
            if (!receiveBack.IsSuccess)
                return (OperateResult)receiveBack;
            OperateResult result = new OperateResult();
            if (receiveBack.Content1 == 1)
                result.IsSuccess = true;
            result.Message = receiveBack.Message;
            socketResult.Content?.Close();
            return result;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.FileClientBase.DeleteFolder(System.String,System.String,System.String)" />
        protected async Task<OperateResult> DeleteFolderAsync(
          string factory,
          string group,
          string id)
        {
            OperateResult<Socket> socketResult = await this.CreateSocketAndConnectAsync(this.ServerIpEndPoint, this.ConnectTimeOut);
            if (!socketResult.IsSuccess)
                return (OperateResult)socketResult;
            OperateResult sendString = await this.SendStringAndCheckReceiveAsync(socketResult.Content, 2012, "");
            if (!sendString.IsSuccess)
                return sendString;
            OperateResult sendFileInfo = await this.SendFactoryGroupIdAsync(socketResult.Content, factory, group, id);
            if (!sendFileInfo.IsSuccess)
                return sendFileInfo;
            OperateResult<int, string> receiveBack = await this.ReceiveStringContentFromSocketAsync(socketResult.Content);
            if (!receiveBack.IsSuccess)
                return (OperateResult)receiveBack;
            OperateResult result = new OperateResult();
            if (receiveBack.Content1 == 1)
                result.IsSuccess = true;
            result.Message = receiveBack.Message;
            socketResult.Content?.Close();
            return result;
        }

        /// <summary>
        /// 下载服务器的文件数据，并且存储到对应的内容里去。<br />
        /// Download the file data of the server and store it in the corresponding content.
        /// </summary>
        /// <param name="factory">一级分类</param>
        /// <param name="group">二级分类</param>
        /// <param name="id">三级分类</param>
        /// <param name="fileName">服务器的文件名称</param>
        /// <param name="processReport">下载的进度报告，第一个数据是已完成总接字节数，第二个数据是总字节数。</param>
        /// <param name="source">数据源信息，决定最终存储到哪里去</param>
        /// <returns>是否成功的结果对象</returns>
        protected OperateResult DownloadFileBase(
          string factory,
          string group,
          string id,
          string fileName,
          Action<long, long> processReport,
          object source)
        {
            OperateResult<Socket> socketAndConnect = this.CreateSocketAndConnect(this.ServerIpEndPoint, this.ConnectTimeOut);
            if (!socketAndConnect.IsSuccess)
                return (OperateResult)socketAndConnect;
            OperateResult operateResult1 = this.SendStringAndCheckReceive(socketAndConnect.Content, 2001, fileName);
            if (!operateResult1.IsSuccess)
                return operateResult1;
            OperateResult operateResult2 = this.SendFactoryGroupId(socketAndConnect.Content, factory, group, id);
            if (!operateResult2.IsSuccess)
                return operateResult2;
            switch (source)
            {
                case string savename:
                    OperateResult fileFromSocket1 = (OperateResult)this.ReceiveFileFromSocket(socketAndConnect.Content, savename, processReport);
                    if (!fileFromSocket1.IsSuccess)
                        return fileFromSocket1;
                    break;
                case Stream stream:
                    OperateResult fileFromSocket2 = (OperateResult)this.ReceiveFileFromSocket(socketAndConnect.Content, stream, processReport);
                    if (!fileFromSocket2.IsSuccess)
                        return fileFromSocket2;
                    break;
                default:
                    socketAndConnect.Content?.Close();
                    this.LogNet?.WriteError(this.ToString(), StringResources.Language.NotSupportedDataType);
                    return new OperateResult(StringResources.Language.NotSupportedDataType);
            }
            socketAndConnect.Content?.Close();
            return OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.FileClientBase.DownloadFileBase(System.String,System.String,System.String,System.String,System.Action{System.Int64,System.Int64},System.Object)" />
        protected async Task<OperateResult> DownloadFileBaseAsync(
          string factory,
          string group,
          string id,
          string fileName,
          Action<long, long> processReport,
          object source)
        {
            OperateResult<Socket> socketResult = await this.CreateSocketAndConnectAsync(this.ServerIpEndPoint, this.ConnectTimeOut);
            if (!socketResult.IsSuccess)
                return (OperateResult)socketResult;
            OperateResult sendString = await this.SendStringAndCheckReceiveAsync(socketResult.Content, 2001, fileName);
            if (!sendString.IsSuccess)
                return sendString;
            OperateResult sendClass = await this.SendFactoryGroupIdAsync(socketResult.Content, factory, group, id);
            if (!sendClass.IsSuccess)
                return sendClass;
            switch (source)
            {
                case string fileSaveName:
                    OperateResult<FileBaseInfo> operateResult1 = await this.ReceiveFileFromSocketAsync(socketResult.Content, fileSaveName, processReport);
                    OperateResult result1 = (OperateResult)operateResult1;
                    operateResult1 = (OperateResult<FileBaseInfo>)null;
                    if (!result1.IsSuccess)
                        return result1;
                    result1 = (OperateResult)null;
                    break;
                case Stream stream:
                    OperateResult<FileBaseInfo> operateResult2 = await this.ReceiveFileFromSocketAsync(socketResult.Content, stream, processReport);
                    OperateResult result2 = (OperateResult)operateResult2;
                    operateResult2 = (OperateResult<FileBaseInfo>)null;
                    if (!result2.IsSuccess)
                        return result2;
                    result2 = (OperateResult)null;
                    stream = (Stream)null;
                    break;
                default:
                    socketResult.Content?.Close();
                    this.LogNet?.WriteError(this.ToString(), StringResources.Language.NotSupportedDataType);
                    return new OperateResult(StringResources.Language.NotSupportedDataType);
            }
            socketResult.Content?.Close();
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 上传文件给服务器，需要指定上传的数据内容，上传到服务器的分类信息，支持进度汇报功能。<br />
        /// To upload files to the server, you need to specify the content of the uploaded data,
        /// the classification information uploaded to the server, and support the progress report function.
        /// </summary>
        /// <param name="source">数据源，可以是文件名，也可以是数据流</param>
        /// <param name="serverName">在服务器保存的文件名，不包含驱动器路径</param>
        /// <param name="factory">一级分类</param>
        /// <param name="group">二级分类</param>
        /// <param name="id">三级分类</param>
        /// <param name="fileTag">文件的描述</param>
        /// <param name="fileUpload">文件的上传人</param>
        /// <param name="processReport">汇报进度，第一个数据是已完成总接字节数，第二个数据是总字节数。</param>
        /// <returns>是否成功的结果对象</returns>
        protected OperateResult UploadFileBase(
          object source,
          string serverName,
          string factory,
          string group,
          string id,
          string fileTag,
          string fileUpload,
          Action<long, long> processReport)
        {
            OperateResult<Socket> socketAndConnect = this.CreateSocketAndConnect(this.ServerIpEndPoint, this.ConnectTimeOut);
            if (!socketAndConnect.IsSuccess)
                return (OperateResult)socketAndConnect;
            OperateResult operateResult1 = this.SendStringAndCheckReceive(socketAndConnect.Content, 2002, serverName);
            if (!operateResult1.IsSuccess)
                return operateResult1;
            OperateResult operateResult2 = this.SendFactoryGroupId(socketAndConnect.Content, factory, group, id);
            if (!operateResult2.IsSuccess)
                return operateResult2;
            switch (source)
            {
                case string filename:
                    OperateResult operateResult3 = this.SendFileAndCheckReceive(socketAndConnect.Content, filename, serverName, fileTag, fileUpload, processReport);
                    if (!operateResult3.IsSuccess)
                        return operateResult3;
                    break;
                case Stream stream:
                    OperateResult operateResult4 = this.SendFileAndCheckReceive(socketAndConnect.Content, stream, serverName, fileTag, fileUpload, processReport);
                    if (!operateResult4.IsSuccess)
                        return operateResult4;
                    break;
                default:
                    socketAndConnect.Content?.Close();
                    this.LogNet?.WriteError(this.ToString(), StringResources.Language.DataSourceFormatError);
                    return new OperateResult(StringResources.Language.DataSourceFormatError);
            }
            OperateResult<int, string> contentFromSocket = this.ReceiveStringContentFromSocket(socketAndConnect.Content);
            return !contentFromSocket.IsSuccess ? (OperateResult)contentFromSocket : (contentFromSocket.Content1 == 1 ? OperateResult.CreateSuccessResult() : new OperateResult(StringResources.Language.ServerFileCheckFailed));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.FileClientBase.UploadFileBase(System.Object,System.String,System.String,System.String,System.String,System.String,System.String,System.Action{System.Int64,System.Int64})" />
        protected async Task<OperateResult> UploadFileBaseAsync(
          object source,
          string serverName,
          string factory,
          string group,
          string id,
          string fileTag,
          string fileUpload,
          Action<long, long> processReport)
        {
            OperateResult<Socket> socketResult = await this.CreateSocketAndConnectAsync(this.ServerIpEndPoint, this.ConnectTimeOut);
            if (!socketResult.IsSuccess)
                return (OperateResult)socketResult;
            OperateResult sendString = await this.SendStringAndCheckReceiveAsync(socketResult.Content, 2002, serverName);
            if (!sendString.IsSuccess)
                return sendString;
            OperateResult sendClass = await this.SendFactoryGroupIdAsync(socketResult.Content, factory, group, id);
            if (!sendClass.IsSuccess)
                return sendClass;
            switch (source)
            {
                case string fileName:
                    OperateResult result1 = await this.SendFileAndCheckReceiveAsync(socketResult.Content, fileName, serverName, fileTag, fileUpload, processReport);
                    if (!result1.IsSuccess)
                        return result1;
                    result1 = (OperateResult)null;
                    break;
                case Stream stream:
                    OperateResult result2 = await this.SendFileAndCheckReceiveAsync(socketResult.Content, stream, serverName, fileTag, fileUpload, processReport);
                    if (!result2.IsSuccess)
                        return result2;
                    result2 = (OperateResult)null;
                    stream = (Stream)null;
                    break;
                default:
                    socketResult.Content?.Close();
                    this.LogNet?.WriteError(this.ToString(), StringResources.Language.DataSourceFormatError);
                    return new OperateResult(StringResources.Language.DataSourceFormatError);
            }
            OperateResult<int, string> resultCheck = await this.ReceiveStringContentFromSocketAsync(socketResult.Content);
            return resultCheck.IsSuccess ? (resultCheck.Content1 != 1 ? new OperateResult(StringResources.Language.ServerFileCheckFailed) : OperateResult.CreateSuccessResult()) : (OperateResult)resultCheck;
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("FileClientBase[{0}]", (object)this.m_ipEndPoint);
    }
}
