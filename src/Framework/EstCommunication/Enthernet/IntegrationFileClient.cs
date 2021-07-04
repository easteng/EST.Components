// Decompiled with JetBrains decompiler
// Type: EstCommunication.Enthernet.IntegrationFileClient
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.Core;
using EstCommunication.Reflection;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace EstCommunication.Enthernet
{
  /// <summary>
  /// 与服务器文件引擎交互的客户端类，支持操作Advanced引擎和Ultimate引擎，用来上传，下载，删除服务器中的文件操作。<br />
  /// The client class that interacts with the server file engine, supports the operation of the Advanced engine and the Ultimate engine,
  /// and is used to upload, download, and delete file operations on the server.
  /// </summary>
  /// <remarks>
  /// 这里需要需要的是，本客户端支持Advanced引擎和Ultimate引擎文件服务器，服务的类型需要您根据自己的需求来选择。
  /// <note type="important">需要注意的是，三个分类信息，factory, group, id 的字符串是不区分大小写的。</note>
  /// </remarks>
  /// <example>
  /// 此处只演示创建实例，具体的上传，下载，删除的例子请参照对应的方法
  /// <code lang="cs" source="TestProject\EstCommunicationDemo\Est\FormFileClient.cs" region="Intergration File Client" title="IntegrationFileClient示例" />
  /// </example>
  public class IntegrationFileClient : FileClientBase
  {
    /// <summary>
    /// 实例化一个默认的对象，需要提前指定服务器的远程地址<br />
    /// Instantiate a default object, you need to specify the remote address of the server in advance
    /// </summary>
    public IntegrationFileClient()
    {
    }

    /// <summary>
    /// 通过指定的Ip地址及端口号实例化一个对象<br />
    /// Instantiate an object with the specified IP address and port number
    /// </summary>
    /// <param name="ipAddress">服务器的ip地址</param>
    /// <param name="port">端口号信息</param>
    public IntegrationFileClient(string ipAddress, int port) => this.ServerIpEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);

    /// <inheritdoc cref="F:EstCommunication.Core.Net.NetworkBase.fileCacheSize" />
    public int FileCacheSize
    {
      get => this.fileCacheSize;
      set => this.fileCacheSize = value;
    }

    /// <summary>
    /// 删除服务器的文件操作，需要指定文件名称，文件的三级分类信息<br />
    /// Delete the file operation of the server, you need to specify the file name and the three-level classification information of the file
    /// </summary>
    /// <param name="fileName">文件名称，带后缀</param>
    /// <param name="factory">第一大类</param>
    /// <param name="group">第二大类</param>
    /// <param name="id">第三大类</param>
    /// <returns>是否成功的结果对象</returns>
    [EstMqttApi(ApiTopic = "DeleteFileFactoryGroupId", Description = "Delete the file operation of the server, you need to specify the file name and the three-level classification information of the file")]
    public OperateResult DeleteFile(
      string fileName,
      string factory,
      string group,
      string id)
    {
      return this.DeleteFileBase(fileName, factory, group, id);
    }

    /// <summary>
    /// 删除服务器的文件操作，此处文件的分类为空<br />
    /// Delete the file operation of the server, the classification of the file is empty here
    /// </summary>
    /// <param name="fileName">文件名称，带后缀</param>
    /// <returns>是否成功的结果对象</returns>
    [EstMqttApi(Description = "Delete the file operation of the server, the classification of the file is empty here")]
    public OperateResult DeleteFile(string fileName) => this.DeleteFileBase(fileName, "", "", "");

    /// <summary>
    /// 删除服务器的文件数组操作，需要指定文件名称，文件的三级分类信息<br />
    /// Delete the file operation of the server, you need to specify the file names and the three-level classification information of the file
    /// </summary>
    /// <param name="fileNames">文件名称数组，带后缀</param>
    /// <param name="factory">第一大类</param>
    /// <param name="group">第二大类</param>
    /// <param name="id">第三大类</param>
    /// <returns>是否成功的结果对象</returns>
    [EstMqttApi(ApiTopic = "DeleteFilesFactoryGroupId", Description = "Delete the file operation of the server, you need to specify the file names and the three-level classification information of the file")]
    public OperateResult DeleteFile(
      string[] fileNames,
      string factory,
      string group,
      string id)
    {
      return this.DeleteFileBase(fileNames, factory, group, id);
    }

    /// <summary>
    /// 删除服务器的文件夹的所有文件操作，文件的三级分类信息<br />
    /// Delete all file operations of the server folder, the three-level classification information of the file
    /// </summary>
    /// <param name="factory">第一大类</param>
    /// <param name="group">第二大类</param>
    /// <param name="id">第三大类</param>
    /// <returns>是否成功的结果对象</returns>
    [EstMqttApi(Description = "Delete all file operations of the server folder, the three-level classification information of the file")]
    public OperateResult DeleteFolderFiles(string factory, string group, string id) => this.DeleteFolder(factory, group, id);

    /// <inheritdoc cref="M:EstCommunication.Enthernet.IntegrationFileClient.DeleteFile(System.String,System.String,System.String,System.String)" />
    public async Task<OperateResult> DeleteFileAsync(
      string fileName,
      string factory,
      string group,
      string id)
    {
      OperateResult operateResult = await this.DeleteFileBaseAsync(fileName, factory, group, id);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.IntegrationFileClient.DeleteFile(System.String)" />
    public async Task<OperateResult> DeleteFileAsync(string fileName)
    {
      OperateResult operateResult = await this.DeleteFileBaseAsync(fileName, "", "", "");
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.IntegrationFileClient.DeleteFile(System.String[],System.String,System.String,System.String)" />
    public async Task<OperateResult> DeleteFileAsync(
      string[] fileNames,
      string factory,
      string group,
      string id)
    {
      OperateResult operateResult = await this.DeleteFileBaseAsync(fileNames, factory, group, id);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.IntegrationFileClient.DeleteFolderFiles(System.String,System.String,System.String)" />
    public async Task<OperateResult> DeleteFolderFilesAsync(
      string factory,
      string group,
      string id)
    {
      OperateResult operateResult = await this.DeleteFolderAsync(factory, group, id);
      return operateResult;
    }

    /// <summary>
    /// 下载服务器的文件到本地的文件操作，需要指定下载的文件的名字，三级分类信息，本次保存的文件名，支持进度报告。<br />
    /// To download a file from the server to a local file, you need to specify the name of the downloaded file,
    /// the three-level classification information, the name of the file saved this time, and support for progress reports.
    /// </summary>
    /// <param name="fileName">文件名称，带后缀</param>
    /// <param name="factory">第一大类</param>
    /// <param name="group">第二大类</param>
    /// <param name="id">第三大类</param>
    /// <param name="processReport">下载的进度报告，第一个数据是已完成总接字节数，第二个数据是总字节数。</param>
    /// <param name="fileSaveName">准备本地保存的名称</param>
    /// <returns>是否成功的结果对象</returns>
    /// <remarks>
    /// 用于分类的参数<paramref name="factory" />，<paramref name="group" />，<paramref name="id" />中间不需要的可以为空，对应的是服务器上的路径系统。
    /// <br /><br />
    /// <note type="warning">
    /// 失败的原因大多数来自于网络的接收异常，或是服务器不存在文件。
    /// </note>
    /// </remarks>
    /// <example>
    /// <code lang="cs" source="TestProject\EstCommunicationDemo\Est\FormFileClient.cs" region="Download File" title="DownloadFile示例" />
    /// </example>
    [EstMqttApi(Description = "To download a file from the server to a local file, you need to specify the name of the downloaded file and three-level classification information")]
    public OperateResult DownloadFile(
      string fileName,
      string factory,
      string group,
      string id,
      Action<long, long> processReport,
      string fileSaveName)
    {
      return this.DownloadFileBase(factory, group, id, fileName, processReport, (object) fileSaveName);
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.IntegrationFileClient.DownloadFile(System.String,System.String,System.String,System.String,System.Action{System.Int64,System.Int64},System.String)" />
    public OperateResult DownloadFile(
      string fileName,
      string factory,
      string group,
      string id,
      Action<long, long> processReport,
      Stream stream)
    {
      return this.DownloadFileBase(factory, group, id, fileName, processReport, (object) stream);
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.IntegrationFileClient.DownloadFile(System.String,System.String,System.String,System.String,System.Action{System.Int64,System.Int64},System.String)" />
    public OperateResult<Bitmap> DownloadFile(
      string fileName,
      string factory,
      string group,
      string id,
      Action<long, long> processReport)
    {
      MemoryStream memoryStream = new MemoryStream();
      OperateResult result = this.DownloadFileBase(factory, group, id, fileName, processReport, (object) memoryStream);
      if (!result.IsSuccess)
      {
        memoryStream.Dispose();
        return OperateResult.CreateFailedResult<Bitmap>(result);
      }
      Bitmap bitmap = new Bitmap((Stream) memoryStream);
      memoryStream.Dispose();
      return OperateResult.CreateSuccessResult<Bitmap>(bitmap);
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.IntegrationFileClient.DownloadFile(System.String,System.String,System.String,System.String,System.Action{System.Int64,System.Int64},System.String)" />
    public async Task<OperateResult> DownloadFileAsync(
      string fileName,
      string factory,
      string group,
      string id,
      Action<long, long> processReport,
      string fileSaveName)
    {
      OperateResult operateResult = await this.DownloadFileBaseAsync(factory, group, id, fileName, processReport, (object) fileSaveName);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.IntegrationFileClient.DownloadFile(System.String,System.String,System.String,System.String,System.Action{System.Int64,System.Int64},System.IO.Stream)" />
    public async Task<OperateResult> DownloadFileAsync(
      string fileName,
      string factory,
      string group,
      string id,
      Action<long, long> processReport,
      Stream stream)
    {
      OperateResult operateResult = await this.DownloadFileBaseAsync(factory, group, id, fileName, processReport, (object) stream);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.IntegrationFileClient.DownloadFile(System.String,System.String,System.String,System.String,System.Action{System.Int64,System.Int64})" />
    public async Task<OperateResult<Bitmap>> DownloadFileAsync(
      string fileName,
      string factory,
      string group,
      string id,
      Action<long, long> processReport)
    {
      MemoryStream stream = new MemoryStream();
      Bitmap bitmap = (Bitmap) null;
      OperateResult result = await this.DownloadFileBaseAsync(factory, group, id, fileName, processReport, (object) stream);
      if (!result.IsSuccess)
      {
        stream.Dispose();
        return OperateResult.CreateFailedResult<Bitmap>(result);
      }
      bitmap = new Bitmap((Stream) stream);
      stream.Dispose();
      return OperateResult.CreateSuccessResult<Bitmap>(bitmap);
    }

    /// <summary>
    /// 上传本地的文件到服务器操作，如果该文件已经存在，那么就更新这个文件。<br />
    /// Upload a local file to the server. If the file already exists, update the file.
    /// </summary>
    /// <param name="fileName">本地的完整路径的文件名称</param>
    /// <param name="serverName">服务器存储的文件名称，带后缀，例如123.txt</param>
    /// <param name="factory">第一大类</param>
    /// <param name="group">第二大类</param>
    /// <param name="id">第三大类</param>
    /// <param name="fileTag">文件的额外描述</param>
    /// <param name="fileUpload">文件的上传人</param>
    /// <param name="processReport">上传的进度报告</param>
    /// <returns>是否成功的结果对象</returns>
    /// <remarks>
    /// 用于分类的参数<paramref name="factory" />，<paramref name="group" />，<paramref name="id" />中间不需要的可以为空，对应的是服务器上的路径系统。
    /// <br /><br />
    /// <note type="warning">
    /// 失败的原因大多数来自于网络的接收异常，或是客户端不存在文件。
    /// </note>
    /// </remarks>
    /// <example>
    /// <code lang="cs" source="TestProject\EstCommunicationDemo\Est\FormFileClient.cs" region="Upload File" title="UploadFile示例" />
    /// </example>
    [EstMqttApi(Description = "Upload a local file to the server. If the file already exists, update the file.")]
    public OperateResult UploadFile(
      string fileName,
      string serverName,
      string factory,
      string group,
      string id,
      string fileTag,
      string fileUpload,
      Action<long, long> processReport)
    {
      return !System.IO.File.Exists(fileName) ? new OperateResult(StringResources.Language.FileNotExist) : this.UploadFileBase((object) fileName, serverName, factory, group, id, fileTag, fileUpload, processReport);
    }

    /// <summary>上传本地的文件到服务器操作，服务器存储的文件名就是当前文件默认的名称</summary>
    /// <param name="fileName">本地的完整路径的文件名称</param>
    /// <param name="factory">第一大类</param>
    /// <param name="group">第二大类</param>
    /// <param name="id">第三大类</param>
    /// <param name="fileTag">文件的额外描述</param>
    /// <param name="fileUpload">文件的上传人</param>
    /// <param name="processReport">上传的进度报告</param>
    /// <returns>是否成功的结果对象</returns>
    public OperateResult UploadFile(
      string fileName,
      string factory,
      string group,
      string id,
      string fileTag,
      string fileUpload,
      Action<long, long> processReport)
    {
      if (!System.IO.File.Exists(fileName))
        return new OperateResult(StringResources.Language.FileNotExist);
      FileInfo fileInfo = new FileInfo(fileName);
      return this.UploadFileBase((object) fileName, fileInfo.Name, factory, group, id, fileTag, fileUpload, processReport);
    }

    /// <summary>上传本地的文件到服务器操作，服务器存储的文件名就是当前文件默认的名称</summary>
    /// <param name="fileName">本地的完整路径的文件名称</param>
    /// <param name="factory">第一大类</param>
    /// <param name="group">第二大类</param>
    /// <param name="id">第三大类</param>
    /// <param name="processReport">上传的进度报告</param>
    /// <returns>是否成功的结果对象</returns>
    public OperateResult UploadFile(
      string fileName,
      string factory,
      string group,
      string id,
      Action<long, long> processReport)
    {
      if (!System.IO.File.Exists(fileName))
        return new OperateResult(StringResources.Language.FileNotExist);
      FileInfo fileInfo = new FileInfo(fileName);
      return this.UploadFileBase((object) fileName, fileInfo.Name, factory, group, id, "", "", processReport);
    }

    /// <summary>上传本地的文件到服务器操作，服务器存储的文件名就是当前文件默认的名称，其余参数默认为空</summary>
    /// <param name="fileName">本地的完整路径的文件名称</param>
    /// <param name="processReport">上传的进度报告</param>
    /// <returns>是否成功的结果对象</returns>
    public OperateResult UploadFile(string fileName, Action<long, long> processReport)
    {
      if (!System.IO.File.Exists(fileName))
        return new OperateResult(StringResources.Language.FileNotExist);
      FileInfo fileInfo = new FileInfo(fileName);
      return this.UploadFileBase((object) fileName, fileInfo.Name, "", "", "", "", "", processReport);
    }

    /// <summary>上传数据流到服务器操作</summary>
    /// <param name="stream">数据流内容</param>
    /// <param name="serverName">服务器存储的文件名称，带后缀</param>
    /// <param name="factory">第一大类</param>
    /// <param name="group">第二大类</param>
    /// <param name="id">第三大类</param>
    /// <param name="fileTag">文件的额外描述</param>
    /// <param name="fileUpload">文件的上传人</param>
    /// <param name="processReport">上传的进度报告</param>
    /// <returns>是否成功的结果对象</returns>
    /// <remarks>
    /// 用于分类的参数<paramref name="factory" />，<paramref name="group" />，<paramref name="id" />中间不需要的可以为空，对应的是服务器上的路径系统。
    /// <br /><br />
    /// <note type="warning">
    /// 失败的原因大多数来自于网络的接收异常，或是客户端不存在文件。
    /// </note>
    /// </remarks>
    /// <example>
    /// <code lang="cs" source="TestProject\EstCommunicationDemo\Est\FormFileClient.cs" region="Upload File" title="UploadFile示例" />
    /// </example>
    public OperateResult UploadFile(
      Stream stream,
      string serverName,
      string factory,
      string group,
      string id,
      string fileTag,
      string fileUpload,
      Action<long, long> processReport)
    {
      return this.UploadFileBase((object) stream, serverName, factory, group, id, fileTag, fileUpload, processReport);
    }

    /// <summary>上传内存图片到服务器操作</summary>
    /// <param name="bitmap">内存图片，不能为空</param>
    /// <param name="serverName">服务器存储的文件名称，带后缀</param>
    /// <param name="factory">第一大类</param>
    /// <param name="group">第二大类</param>
    /// <param name="id">第三大类</param>
    /// <param name="fileTag">文件的额外描述</param>
    /// <param name="fileUpload">文件的上传人</param>
    /// <param name="processReport">上传的进度报告</param>
    /// <returns>是否成功的结果对象</returns>
    /// <exception cref="T:System.ArgumentNullException"></exception>
    /// <remarks>
    /// 用于分类的参数<paramref name="factory" />，<paramref name="group" />，<paramref name="id" />中间不需要的可以为空，对应的是服务器上的路径系统。
    /// <br /><br />
    /// <note type="warning">
    /// 失败的原因大多数来自于网络的接收异常，或是客户端不存在文件。
    /// </note>
    /// </remarks>
    /// <example>
    /// <code lang="cs" source="TestProject\EstCommunicationDemo\Est\FormFileClient.cs" region="Upload File" title="UploadFile示例" />
    /// </example>
    public OperateResult UploadFile(
      Bitmap bitmap,
      string serverName,
      string factory,
      string group,
      string id,
      string fileTag,
      string fileUpload,
      Action<long, long> processReport)
    {
      MemoryStream memoryStream = new MemoryStream();
      if (bitmap.RawFormat != null)
        bitmap.Save((Stream) memoryStream, bitmap.RawFormat);
      else
        bitmap.Save((Stream) memoryStream, ImageFormat.Bmp);
      OperateResult operateResult = this.UploadFileBase((object) memoryStream, serverName, factory, group, id, fileTag, fileUpload, processReport);
      memoryStream.Dispose();
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.IntegrationFileClient.UploadFile(System.String,System.String,System.String,System.String,System.String,System.String,System.String,System.Action{System.Int64,System.Int64})" />
    public async Task<OperateResult> UploadFileAsync(
      string fileName,
      string serverName,
      string factory,
      string group,
      string id,
      string fileTag,
      string fileUpload,
      Action<long, long> processReport)
    {
      if (!System.IO.File.Exists(fileName))
        return new OperateResult(StringResources.Language.FileNotExist);
      OperateResult operateResult = await this.UploadFileBaseAsync((object) fileName, serverName, factory, group, id, fileTag, fileUpload, processReport);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.IntegrationFileClient.UploadFile(System.String,System.String,System.String,System.String,System.String,System.String,System.Action{System.Int64,System.Int64})" />
    public async Task<OperateResult> UploadFileAsync(
      string fileName,
      string factory,
      string group,
      string id,
      string fileTag,
      string fileUpload,
      Action<long, long> processReport)
    {
      if (!System.IO.File.Exists(fileName))
        return new OperateResult(StringResources.Language.FileNotExist);
      FileInfo fileInfo = new FileInfo(fileName);
      OperateResult operateResult = await this.UploadFileBaseAsync((object) fileName, fileInfo.Name, factory, group, id, fileTag, fileUpload, processReport);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.IntegrationFileClient.UploadFile(System.String,System.String,System.String,System.String,System.Action{System.Int64,System.Int64})" />
    public async Task<OperateResult> UploadFileAsync(
      string fileName,
      string factory,
      string group,
      string id,
      Action<long, long> processReport)
    {
      if (!System.IO.File.Exists(fileName))
        return new OperateResult(StringResources.Language.FileNotExist);
      FileInfo fileInfo = new FileInfo(fileName);
      OperateResult operateResult = await this.UploadFileBaseAsync((object) fileName, fileInfo.Name, factory, group, id, "", "", processReport);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.IntegrationFileClient.UploadFile(System.String,System.Action{System.Int64,System.Int64})" />
    public async Task<OperateResult> UploadFileAsync(
      string fileName,
      Action<long, long> processReport)
    {
      if (!System.IO.File.Exists(fileName))
        return new OperateResult(StringResources.Language.FileNotExist);
      FileInfo fileInfo = new FileInfo(fileName);
      OperateResult operateResult = await this.UploadFileBaseAsync((object) fileName, fileInfo.Name, "", "", "", "", "", processReport);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.IntegrationFileClient.UploadFile(System.IO.Stream,System.String,System.String,System.String,System.String,System.String,System.String,System.Action{System.Int64,System.Int64})" />
    public async Task<OperateResult> UploadFileAsync(
      Stream stream,
      string serverName,
      string factory,
      string group,
      string id,
      string fileTag,
      string fileUpload,
      Action<long, long> processReport)
    {
      OperateResult operateResult = await this.UploadFileBaseAsync((object) stream, serverName, factory, group, id, fileTag, fileUpload, processReport);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.IntegrationFileClient.UploadFile(System.Drawing.Bitmap,System.String,System.String,System.String,System.String,System.String,System.String,System.Action{System.Int64,System.Int64})" />
    public async Task<OperateResult> UploadFileAsync(
      Bitmap bitmap,
      string serverName,
      string factory,
      string group,
      string id,
      string fileTag,
      string fileUpload,
      Action<long, long> processReport)
    {
      MemoryStream stream = new MemoryStream();
      if (bitmap.RawFormat != null)
        bitmap.Save((Stream) stream, bitmap.RawFormat);
      else
        bitmap.Save((Stream) stream, ImageFormat.Bmp);
      OperateResult result = await this.UploadFileBaseAsync((object) stream, serverName, factory, group, id, fileTag, fileUpload, processReport);
      stream.Dispose();
      OperateResult operateResult = result;
      stream = (MemoryStream) null;
      result = (OperateResult) null;
      return operateResult;
    }

    /// <summary>
    /// 获取指定路径下的所有的文档<br />
    /// Get all documents in the specified path
    /// </summary>
    /// <param name="factory">第一大类</param>
    /// <param name="group">第二大类</param>
    /// <param name="id">第三大类</param>
    /// <returns>是否成功的结果对象</returns>
    /// <remarks>
    /// 用于分类的参数<paramref name="factory" />，<paramref name="group" />，<paramref name="id" />中间不需要的可以为空，对应的是服务器上的路径系统。
    /// <br /><br />
    /// <note type="warning">
    /// 失败的原因大多数来自于网络的接收异常。
    /// </note>
    /// </remarks>
    /// <example>
    /// <code lang="cs" source="TestProject\EstCommunicationDemo\Est\FormFileClient.cs" region="DownloadPathFileNames" title="DownloadPathFileNames示例" />
    /// </example>
    [EstMqttApi(Description = "Get all documents in the specified path")]
    public OperateResult<GroupFileItem[]> DownloadPathFileNames(
      string factory,
      string group,
      string id)
    {
      return this.DownloadStringArrays<GroupFileItem>(2007, factory, group, id);
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.IntegrationFileClient.DownloadPathFileNames(System.String,System.String,System.String)" />
    public async Task<OperateResult<GroupFileItem[]>> DownloadPathFileNamesAsync(
      string factory,
      string group,
      string id)
    {
      OperateResult<GroupFileItem[]> operateResult = await this.DownloadStringArraysAsync<GroupFileItem>(2007, factory, group, id);
      return operateResult;
    }

    /// <summary>
    /// 获取指定路径下的所有的目录<br />
    /// Get all directories under the specified path
    /// </summary>
    /// <param name="factory">第一大类</param>
    /// <param name="group">第二大类</param>
    /// <param name="id">第三大类</param>
    /// <returns>是否成功的结果对象</returns>
    /// <remarks>
    /// 用于分类的参数<paramref name="factory" />，<paramref name="group" />，<paramref name="id" />中间不需要的可以为空，对应的是服务器上的路径系统。
    /// <br /><br />
    /// <note type="warning">
    /// 失败的原因大多数来自于网络的接收异常。
    /// </note>
    /// </remarks>
    /// <example>
    /// <code lang="cs" source="TestProject\EstCommunicationDemo\Est\FormFileClient.cs" region="DownloadPathFolders" title="DownloadPathFolders示例" />
    /// </example>
    [EstMqttApi(Description = "Get all directories under the specified path")]
    public OperateResult<string[]> DownloadPathFolders(
      string factory,
      string group,
      string id)
    {
      return this.DownloadStringArrays<string>(2008, factory, group, id);
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.IntegrationFileClient.DownloadPathFolders(System.String,System.String,System.String)" />
    public async Task<OperateResult<string[]>> DownloadPathFoldersAsync(
      string factory,
      string group,
      string id)
    {
      OperateResult<string[]> operateResult = await this.DownloadStringArraysAsync<string>(2008, factory, group, id);
      return operateResult;
    }

    /// <summary>
    /// 检查当前的文件是否在服务器端存在，列表中需要存在文件的名称，映射的文件也需要存在。<br />
    /// Check whether the current file exists on the server side, the name of the file must exist in the list, and the mapped file must also exist.
    /// </summary>
    /// <param name="fileName">当前的文件名称，举例123.txt</param>
    /// <param name="factory">第一级分类信息</param>
    /// <param name="group">第二级分类信息</param>
    /// <param name="id">第三级分类信息</param>
    /// <returns>是否存在，存在返回true, 否则，返回false</returns>
    public OperateResult<bool> IsFileExists(
      string fileName,
      string factory,
      string group,
      string id)
    {
      OperateResult<Socket> socketAndConnect = this.CreateSocketAndConnect(this.ServerIpEndPoint, this.ConnectTimeOut);
      if (!socketAndConnect.IsSuccess)
        return OperateResult.CreateFailedResult<bool>((OperateResult) socketAndConnect);
      OperateResult result1 = this.SendStringAndCheckReceive(socketAndConnect.Content, 2013, fileName);
      if (!result1.IsSuccess)
        return OperateResult.CreateFailedResult<bool>(result1);
      OperateResult result2 = this.SendFactoryGroupId(socketAndConnect.Content, factory, group, id);
      if (!result2.IsSuccess)
        return OperateResult.CreateFailedResult<bool>(result2);
      OperateResult<int, string> contentFromSocket = this.ReceiveStringContentFromSocket(socketAndConnect.Content);
      if (!contentFromSocket.IsSuccess)
        return OperateResult.CreateFailedResult<bool>((OperateResult) contentFromSocket);
      OperateResult<bool> successResult = OperateResult.CreateSuccessResult<bool>(contentFromSocket.Content1 == 1);
      socketAndConnect.Content?.Close();
      return successResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.IntegrationFileClient.IsFileExists(System.String,System.String,System.String,System.String)" />
    public async Task<OperateResult<bool>> IsFileExistsAsync(
      string fileName,
      string factory,
      string group,
      string id)
    {
      OperateResult<Socket> socketResult = await this.CreateSocketAndConnectAsync(this.ServerIpEndPoint, this.ConnectTimeOut);
      if (!socketResult.IsSuccess)
        return OperateResult.CreateFailedResult<bool>((OperateResult) socketResult);
      OperateResult sendString = await this.SendStringAndCheckReceiveAsync(socketResult.Content, 2013, fileName);
      if (!sendString.IsSuccess)
        return OperateResult.CreateFailedResult<bool>(sendString);
      OperateResult sendFileInfo = await this.SendFactoryGroupIdAsync(socketResult.Content, factory, group, id);
      if (!sendFileInfo.IsSuccess)
        return OperateResult.CreateFailedResult<bool>(sendFileInfo);
      OperateResult<int, string> receiveBack = await this.ReceiveStringContentFromSocketAsync(socketResult.Content);
      if (!receiveBack.IsSuccess)
        return OperateResult.CreateFailedResult<bool>((OperateResult) receiveBack);
      OperateResult<bool> result = OperateResult.CreateSuccessResult<bool>(receiveBack.Content1 == 1);
      socketResult.Content?.Close();
      return result;
    }

    /// <summary>获取指定路径下的所有的路径或是文档信息</summary>
    /// <param name="protocol">指令</param>
    /// <param name="factory">第一大类</param>
    /// <param name="group">第二大类</param>
    /// <param name="id">第三大类</param>
    /// <typeparam name="T">数组的类型</typeparam>
    /// <returns>是否成功的结果对象</returns>
    private OperateResult<T[]> DownloadStringArrays<T>(
      int protocol,
      string factory,
      string group,
      string id)
    {
      OperateResult<Socket> socketAndConnect = this.CreateSocketAndConnect(this.ServerIpEndPoint, this.ConnectTimeOut);
      if (!socketAndConnect.IsSuccess)
        return OperateResult.CreateFailedResult<T[]>((OperateResult) socketAndConnect);
      OperateResult result1 = this.SendStringAndCheckReceive(socketAndConnect.Content, protocol, "nosense");
      if (!result1.IsSuccess)
        return OperateResult.CreateFailedResult<T[]>(result1);
      OperateResult result2 = this.SendFactoryGroupId(socketAndConnect.Content, factory, group, id);
      if (!result2.IsSuccess)
        return OperateResult.CreateFailedResult<T[]>(result2);
      OperateResult<int, string> contentFromSocket = this.ReceiveStringContentFromSocket(socketAndConnect.Content);
      if (!contentFromSocket.IsSuccess)
        return OperateResult.CreateFailedResult<T[]>((OperateResult) contentFromSocket);
      socketAndConnect.Content?.Close();
      try
      {
        return OperateResult.CreateSuccessResult<T[]>(JArray.Parse(contentFromSocket.Content2).ToObject<T[]>());
      }
      catch (Exception ex)
      {
        return new OperateResult<T[]>(ex.Message);
      }
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.IntegrationFileClient.DownloadStringArrays``1(System.Int32,System.String,System.String,System.String)" />
    private async Task<OperateResult<T[]>> DownloadStringArraysAsync<T>(
      int protocol,
      string factory,
      string group,
      string id)
    {
      OperateResult<Socket> socketResult = await this.CreateSocketAndConnectAsync(this.ServerIpEndPoint, this.ConnectTimeOut);
      if (!socketResult.IsSuccess)
        return OperateResult.CreateFailedResult<T[]>((OperateResult) socketResult);
      OperateResult send = await this.SendStringAndCheckReceiveAsync(socketResult.Content, protocol, "nosense");
      if (!send.IsSuccess)
        return OperateResult.CreateFailedResult<T[]>(send);
      OperateResult sendClass = await this.SendFactoryGroupIdAsync(socketResult.Content, factory, group, id);
      if (!sendClass.IsSuccess)
        return OperateResult.CreateFailedResult<T[]>(sendClass);
      OperateResult<int, string> receive = await this.ReceiveStringContentFromSocketAsync(socketResult.Content);
      if (!receive.IsSuccess)
        return OperateResult.CreateFailedResult<T[]>((OperateResult) receive);
      socketResult.Content?.Close();
      try
      {
        return OperateResult.CreateSuccessResult<T[]>(JArray.Parse(receive.Content2).ToObject<T[]>());
      }
      catch (Exception ex)
      {
        return new OperateResult<T[]>(ex.Message);
      }
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("IntegrationFileClient[{0}]", (object) this.ServerIpEndPoint);
  }
}
