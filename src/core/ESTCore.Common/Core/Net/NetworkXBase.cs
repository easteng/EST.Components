// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Core.Net.NetworkXBase
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;

using Newtonsoft.Json.Linq;

using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ESTCore.Common.Core.Net
{
    /// <summary>
    /// 包含了主动异步接收的方法实现和文件类异步读写的实现<br />
    /// Contains the implementation of the active asynchronous receiving method and the implementation of asynchronous reading and writing of the file class
    /// </summary>
    public class NetworkXBase : NetworkBase
    {
        /// <summary>
        /// [自校验] 将文件数据发送至套接字，如果结果异常，则结束通讯<br />
        /// [Self-check] Send the file data to the socket. If the result is abnormal, the communication is ended.
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="filename">完整的文件路径</param>
        /// <param name="filelength">文件的长度</param>
        /// <param name="report">进度报告器</param>
        /// <returns>是否发送成功</returns>
        protected OperateResult SendFileStreamToSocket(
          Socket socket,
          string filename,
          long filelength,
          Action<long, long> report = null)
        {
            try
            {
                OperateResult operateResult = new OperateResult();
                using (FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                    operateResult = this.SendStreamToSocket(socket, (Stream)fileStream, filelength, report, true);
                return operateResult;
            }
            catch (Exception ex)
            {
                socket?.Close();
                this.LogNet?.WriteException(this.ToString(), ex);
                return new OperateResult(ex.Message);
            }
        }

        /// <summary>
        /// [自校验] 将文件数据发送至套接字，具体发送细节将在继承类中实现，如果结果异常，则结束通讯<br />
        /// [Self-checking] Send the file data to the socket. The specific sending details will be implemented in the inherited class. If the result is abnormal, the communication will end
        /// </summary>
        /// <param name="socket">套接字</param>
        /// <param name="filename">文件名称，文件必须存在</param>
        /// <param name="servername">远程端的文件名称</param>
        /// <param name="filetag">文件的额外标签</param>
        /// <param name="fileupload">文件的上传人</param>
        /// <param name="sendReport">发送进度报告</param>
        /// <returns>是否发送成功</returns>
        protected OperateResult SendFileAndCheckReceive(
          Socket socket,
          string filename,
          string servername,
          string filetag,
          string fileupload,
          Action<long, long> sendReport = null)
        {
            FileInfo fileInfo = new FileInfo(filename);
            if (!File.Exists(filename))
            {
                OperateResult operateResult = this.SendStringAndCheckReceive(socket, 0, "");
                if (!operateResult.IsSuccess)
                    return operateResult;
                socket?.Close();
                return new OperateResult(StringResources.Language.FileNotExist);
            }
            JObject jobject = new JObject()
      {
        {
          "FileName",
          (JToken) new JValue(servername)
        },
        {
          "FileSize",
          (JToken) new JValue(fileInfo.Length)
        },
        {
          "FileTag",
          (JToken) new JValue(filetag)
        },
        {
          "FileUpload",
          (JToken) new JValue(fileupload)
        }
      };
            OperateResult operateResult1 = this.SendStringAndCheckReceive(socket, 1, jobject.ToString());
            return !operateResult1.IsSuccess ? operateResult1 : this.SendFileStreamToSocket(socket, filename, fileInfo.Length, sendReport);
        }

        /// <summary>
        /// [自校验] 将流数据发送至套接字，具体发送细节将在继承类中实现，如果结果异常，则结束通讯<br />
        /// [Self-checking] Send stream data to the socket. The specific sending details will be implemented in the inherited class.
        /// If the result is abnormal, the communication will be terminated
        /// </summary>
        /// <param name="socket">套接字</param>
        /// <param name="stream">文件名称，文件必须存在</param>
        /// <param name="servername">远程端的文件名称</param>
        /// <param name="filetag">文件的额外标签</param>
        /// <param name="fileupload">文件的上传人</param>
        /// <param name="sendReport">发送进度报告</param>
        /// <returns>是否成功的结果对象</returns>
        protected OperateResult SendFileAndCheckReceive(
          Socket socket,
          Stream stream,
          string servername,
          string filetag,
          string fileupload,
          Action<long, long> sendReport = null)
        {
            JObject jobject = new JObject()
      {
        {
          "FileName",
          (JToken) new JValue(servername)
        },
        {
          "FileSize",
          (JToken) new JValue(stream.Length)
        },
        {
          "FileTag",
          (JToken) new JValue(filetag)
        },
        {
          "FileUpload",
          (JToken) new JValue(fileupload)
        }
      };
            OperateResult operateResult = this.SendStringAndCheckReceive(socket, 1, jobject.ToString());
            return !operateResult.IsSuccess ? operateResult : this.SendStreamToSocket(socket, stream, stream.Length, sendReport, true);
        }

        /// <summary>
        /// [自校验] 从套接字中接收文件头信息<br />
        /// [Self-checking] Receive file header information from socket
        /// </summary>
        /// <param name="socket">套接字的网络</param>
        /// <returns>包含文件信息的结果对象</returns>
        protected OperateResult<FileBaseInfo> ReceiveFileHeadFromSocket(
          Socket socket)
        {
            OperateResult<int, string> contentFromSocket = this.ReceiveStringContentFromSocket(socket);
            if (!contentFromSocket.IsSuccess)
                return OperateResult.CreateFailedResult<FileBaseInfo>((OperateResult)contentFromSocket);
            if (contentFromSocket.Content1 == 0)
            {
                socket?.Close();
                this.LogNet?.WriteWarn(this.ToString(), StringResources.Language.FileRemoteNotExist);
                return new OperateResult<FileBaseInfo>(StringResources.Language.FileNotExist);
            }
            OperateResult<FileBaseInfo> operateResult = new OperateResult<FileBaseInfo>()
            {
                Content = new FileBaseInfo()
            };
            try
            {
                JObject json = JObject.Parse(contentFromSocket.Content2);
                operateResult.Content.Name = SoftBasic.GetValueFromJsonObject<string>(json, "FileName", "");
                operateResult.Content.Size = SoftBasic.GetValueFromJsonObject<long>(json, "FileSize", 0L);
                operateResult.Content.Tag = SoftBasic.GetValueFromJsonObject<string>(json, "FileTag", "");
                operateResult.Content.Upload = SoftBasic.GetValueFromJsonObject<string>(json, "FileUpload", "");
                operateResult.IsSuccess = true;
            }
            catch (Exception ex)
            {
                socket?.Close();
                operateResult.Message = "Extra File Head Wrong:" + ex.Message;
            }
            return operateResult;
        }

        /// <summary>
        /// [自校验] 从网络中接收一个文件，如果结果异常，则结束通讯<br />
        /// [Self-checking] Receive a file from the network. If the result is abnormal, the communication ends.
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="savename">接收文件后保存的文件名</param>
        /// <param name="receiveReport">接收进度报告</param>
        /// <returns>包含文件信息的结果对象</returns>
        protected OperateResult<FileBaseInfo> ReceiveFileFromSocket(
          Socket socket,
          string savename,
          Action<long, long> receiveReport)
        {
            OperateResult<FileBaseInfo> fileHeadFromSocket = this.ReceiveFileHeadFromSocket(socket);
            if (!fileHeadFromSocket.IsSuccess)
                return fileHeadFromSocket;
            try
            {
                OperateResult result = (OperateResult)null;
                using (FileStream fileStream = new FileStream(savename, FileMode.Create, FileAccess.Write))
                    result = this.WriteStreamFromSocket(socket, (Stream)fileStream, fileHeadFromSocket.Content.Size, receiveReport, true);
                if (result.IsSuccess)
                    return fileHeadFromSocket;
                if (File.Exists(savename))
                    File.Delete(savename);
                return OperateResult.CreateFailedResult<FileBaseInfo>(result);
            }
            catch (Exception ex)
            {
                this.LogNet?.WriteException(this.ToString(), ex);
                socket?.Close();
                OperateResult<FileBaseInfo> operateResult = new OperateResult<FileBaseInfo>();
                operateResult.Message = ex.Message;
                return operateResult;
            }
        }

        /// <summary>
        /// [自校验] 从网络中接收一个文件，写入数据流，如果结果异常，则结束通讯，参数顺序文件名，文件大小，文件标识，上传人<br />
        /// [Self-checking] Receive a file from the network. If the result is abnormal, the communication ends.
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="stream">等待写入的数据流</param>
        /// <param name="receiveReport">接收进度报告</param>
        /// <returns>文件头结果</returns>
        protected OperateResult<FileBaseInfo> ReceiveFileFromSocket(
          Socket socket,
          Stream stream,
          Action<long, long> receiveReport)
        {
            OperateResult<FileBaseInfo> fileHeadFromSocket = this.ReceiveFileHeadFromSocket(socket);
            if (!fileHeadFromSocket.IsSuccess)
                return fileHeadFromSocket;
            try
            {
                this.WriteStreamFromSocket(socket, stream, fileHeadFromSocket.Content.Size, receiveReport, true);
                return fileHeadFromSocket;
            }
            catch (Exception ex)
            {
                this.LogNet?.WriteException(this.ToString(), ex);
                socket?.Close();
                OperateResult<FileBaseInfo> operateResult = new OperateResult<FileBaseInfo>();
                operateResult.Message = ex.Message;
                return operateResult;
            }
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkXBase.SendFileStreamToSocket(System.Net.Sockets.Socket,System.String,System.Int64,System.Action{System.Int64,System.Int64})" />
        protected async Task<OperateResult> SendFileStreamToSocketAsync(
          Socket socket,
          string filename,
          long filelength,
          Action<long, long> report = null)
        {
            try
            {
                OperateResult result = new OperateResult();
                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                    result = await this.SendStreamToSocketAsync(socket, (Stream)fs, filelength, report, true);
                return result;
            }
            catch (Exception ex)
            {
                socket?.Close();
                this.LogNet?.WriteException(this.ToString(), ex);
                return new OperateResult(ex.Message);
            }
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkXBase.SendFileAndCheckReceive(System.Net.Sockets.Socket,System.String,System.String,System.String,System.String,System.Action{System.Int64,System.Int64})" />
        protected async Task<OperateResult> SendFileAndCheckReceiveAsync(
          Socket socket,
          string filename,
          string servername,
          string filetag,
          string fileupload,
          Action<long, long> sendReport = null)
        {
            FileInfo info = new FileInfo(filename);
            if (!File.Exists(filename))
            {
                OperateResult stringResult = await this.SendStringAndCheckReceiveAsync(socket, 0, "");
                if (!stringResult.IsSuccess)
                    return stringResult;
                socket?.Close();
                return new OperateResult(StringResources.Language.FileNotExist);
            }
            JObject json = new JObject()
      {
        {
          "FileName",
          (JToken) new JValue(servername)
        },
        {
          "FileSize",
          (JToken) new JValue(info.Length)
        },
        {
          "FileTag",
          (JToken) new JValue(filetag)
        },
        {
          "FileUpload",
          (JToken) new JValue(fileupload)
        }
      };
            OperateResult sendResult = await this.SendStringAndCheckReceiveAsync(socket, 1, json.ToString());
            if (!sendResult.IsSuccess)
                return sendResult;
            OperateResult socketAsync = await this.SendFileStreamToSocketAsync(socket, filename, info.Length, sendReport);
            return socketAsync;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkXBase.SendFileAndCheckReceive(System.Net.Sockets.Socket,System.IO.Stream,System.String,System.String,System.String,System.Action{System.Int64,System.Int64})" />
        protected async Task<OperateResult> SendFileAndCheckReceiveAsync(
          Socket socket,
          Stream stream,
          string servername,
          string filetag,
          string fileupload,
          Action<long, long> sendReport = null)
        {
            JObject json = new JObject()
      {
        {
          "FileName",
          (JToken) new JValue(servername)
        },
        {
          "FileSize",
          (JToken) new JValue(stream.Length)
        },
        {
          "FileTag",
          (JToken) new JValue(filetag)
        },
        {
          "FileUpload",
          (JToken) new JValue(fileupload)
        }
      };
            OperateResult fileResult = await this.SendStringAndCheckReceiveAsync(socket, 1, json.ToString());
            if (!fileResult.IsSuccess)
                return fileResult;
            OperateResult socketAsync = await this.SendStreamToSocketAsync(socket, stream, stream.Length, sendReport, true);
            return socketAsync;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkXBase.ReceiveFileHeadFromSocket(System.Net.Sockets.Socket)" />
        protected async Task<OperateResult<FileBaseInfo>> ReceiveFileHeadFromSocketAsync(
          Socket socket)
        {
            OperateResult<int, string> receiveString = await this.ReceiveStringContentFromSocketAsync(socket);
            if (!receiveString.IsSuccess)
                return OperateResult.CreateFailedResult<FileBaseInfo>((OperateResult)receiveString);
            if (receiveString.Content1 == 0)
            {
                socket?.Close();
                this.LogNet?.WriteWarn(this.ToString(), StringResources.Language.FileRemoteNotExist);
                return new OperateResult<FileBaseInfo>(StringResources.Language.FileNotExist);
            }
            OperateResult<FileBaseInfo> result = new OperateResult<FileBaseInfo>()
            {
                Content = new FileBaseInfo()
            };
            try
            {
                JObject json = JObject.Parse(receiveString.Content2);
                result.Content.Name = SoftBasic.GetValueFromJsonObject<string>(json, "FileName", "");
                result.Content.Size = SoftBasic.GetValueFromJsonObject<long>(json, "FileSize", 0L);
                result.Content.Tag = SoftBasic.GetValueFromJsonObject<string>(json, "FileTag", "");
                result.Content.Upload = SoftBasic.GetValueFromJsonObject<string>(json, "FileUpload", "");
                result.IsSuccess = true;
                json = (JObject)null;
            }
            catch (Exception ex)
            {
                socket?.Close();
                result.Message = "Extra File Head Wrong:" + ex.Message;
            }
            return result;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkXBase.ReceiveFileFromSocket(System.Net.Sockets.Socket,System.String,System.Action{System.Int64,System.Int64})" />
        protected async Task<OperateResult<FileBaseInfo>> ReceiveFileFromSocketAsync(
          Socket socket,
          string savename,
          Action<long, long> receiveReport)
        {
            OperateResult<FileBaseInfo> fileResult = await this.ReceiveFileHeadFromSocketAsync(socket);
            if (!fileResult.IsSuccess)
                return fileResult;
            try
            {
                OperateResult write = (OperateResult)null;
                using (FileStream fs = new FileStream(savename, FileMode.Create, FileAccess.Write))
                    write = await this.WriteStreamFromSocketAsync(socket, (Stream)fs, fileResult.Content.Size, receiveReport, true);
                if (write.IsSuccess)
                    return fileResult;
                if (File.Exists(savename))
                    File.Delete(savename);
                return OperateResult.CreateFailedResult<FileBaseInfo>(write);
            }
            catch (Exception ex)
            {
                this.LogNet?.WriteException(this.ToString(), ex);
                socket?.Close();
                return new OperateResult<FileBaseInfo>(ex.Message);
            }
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkXBase.ReceiveFileFromSocket(System.Net.Sockets.Socket,System.IO.Stream,System.Action{System.Int64,System.Int64})" />
        protected async Task<OperateResult<FileBaseInfo>> ReceiveFileFromSocketAsync(
          Socket socket,
          Stream stream,
          Action<long, long> receiveReport)
        {
            OperateResult<FileBaseInfo> fileResult = await this.ReceiveFileHeadFromSocketAsync(socket);
            if (!fileResult.IsSuccess)
                return fileResult;
            try
            {
                OperateResult operateResult = await this.WriteStreamFromSocketAsync(socket, stream, fileResult.Content.Size, receiveReport, true);
                return fileResult;
            }
            catch (Exception ex)
            {
                this.LogNet?.WriteException(this.ToString(), ex);
                socket?.Close();
                return new OperateResult<FileBaseInfo>(ex.Message);
            }
        }

        /// <inheritdoc />
        public override string ToString() => nameof(NetworkXBase);
    }
}
