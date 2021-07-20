// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Enthernet.NetSoftUpdateServer
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Core.Net;
using ESTCore.Common.LogNet;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ESTCore.Common.Enthernet
{
    /// <summary>
    /// 用于服务器支持软件全自动更新升级的类<br />
    /// Class for server support software full automatic update and upgrade
    /// </summary>
    public sealed class NetSoftUpdateServer : NetworkServerBase
    {
        private string m_FilePath = "C:\\ESTCore.Common";
        private string updateExeFileName;

        /// <summary>
        /// 实例化一个默认的对象<br />
        /// Instantiate a default object
        /// </summary>
        /// <param name="updateExeFileName">更新程序的名称</param>
        public NetSoftUpdateServer(string updateExeFileName = "软件自动更新.exe") => this.updateExeFileName = updateExeFileName;

        /// <summary>系统升级时客户端所在的目录，默认为C:\ESTCore.Common</summary>
        public string FileUpdatePath
        {
            get => this.m_FilePath;
            set => this.m_FilePath = value;
        }

        /// <inheritdoc />
        protected override async void ThreadPoolLogin(Socket socket, IPEndPoint endPoint)
        {
            try
            {
                OperateResult<byte[]> receive = await this.ReceiveAsync(socket, 4);
                if (!receive.IsSuccess)
                {
                    this.LogNet?.WriteError(this.ToString(), "Receive Failed: " + receive.Message);
                }
                else
                {
                    byte[] ReceiveByte = receive.Content;
                    int Protocol = BitConverter.ToInt32(ReceiveByte, 0);
                    if (Protocol == 4097 || Protocol == 4098)
                    {
                        if (Protocol == 4097)
                            this.LogNet?.WriteInfo(this.ToString(), StringResources.Language.SystemInstallOperater + ((IPEndPoint)socket.RemoteEndPoint).Address.ToString());
                        else
                            this.LogNet?.WriteInfo(this.ToString(), StringResources.Language.SystemUpdateOperater + ((IPEndPoint)socket.RemoteEndPoint).Address.ToString());
                        if (Directory.Exists(this.FileUpdatePath))
                        {
                            List<string> Files = NetSoftUpdateServer.GetAllFiles(this.FileUpdatePath, this.LogNet);
                            for (int i = Files.Count - 1; i >= 0; --i)
                            {
                                FileInfo finfo = new FileInfo(Files[i]);
                                if (finfo.Length > 200000000L)
                                    Files.RemoveAt(i);
                                if (Protocol == 4098 && finfo.Name == this.updateExeFileName)
                                    Files.RemoveAt(i);
                                finfo = (FileInfo)null;
                            }
                            string[] files = Files.ToArray();
                            socket.BeginReceive(new byte[4], 0, 4, SocketFlags.None, new AsyncCallback(this.ReceiveCallBack), (object)socket);
                            OperateResult operateResult1 = await this.SendAsync(socket, BitConverter.GetBytes(files.Length));
                            for (int i = 0; i < files.Length; ++i)
                            {
                                FileInfo finfo = new FileInfo(files[i]);
                                string fileName = finfo.FullName.Replace(this.m_FilePath, "");
                                if (fileName.StartsWith("\\"))
                                    fileName = fileName.Substring(1);
                                byte[] ByteName = Encoding.Unicode.GetBytes(fileName);
                                int First = 8 + ByteName.Length;
                                byte[] FirstSend = new byte[First];
                                FileStream fs = new FileStream(files[i], FileMode.Open, FileAccess.Read);
                                Array.Copy((Array)BitConverter.GetBytes(First), 0, (Array)FirstSend, 0, 4);
                                Array.Copy((Array)BitConverter.GetBytes((int)fs.Length), 0, (Array)FirstSend, 4, 4);
                                Array.Copy((Array)ByteName, 0, (Array)FirstSend, 8, ByteName.Length);
                                OperateResult operateResult2 = await this.SendAsync(socket, FirstSend);
                                Thread.Sleep(10);
                                byte[] buffer = new byte[40960];
                                int count;
                                for (int sended = 0; (long)sended < fs.Length; sended += count)
                                {
                                    count = await fs.ReadAsync(buffer, 0, buffer.Length);
                                    OperateResult operateResult3 = await this.SendAsync(socket, buffer, 0, count);
                                }
                                fs.Close();
                                fs.Dispose();
                                Thread.Sleep(20);
                                finfo = (FileInfo)null;
                                fileName = (string)null;
                                ByteName = (byte[])null;
                                FirstSend = (byte[])null;
                                fs = (FileStream)null;
                                buffer = (byte[])null;
                            }
                            Files = (List<string>)null;
                            files = (string[])null;
                        }
                        else
                        {
                            OperateResult operateResult = await this.SendAsync(socket, BitConverter.GetBytes(0));
                            socket?.Close();
                        }
                    }
                    else
                    {
                        OperateResult operateResult = await this.SendAsync(socket, BitConverter.GetBytes(10000f));
                        Thread.Sleep(20);
                        socket?.Close();
                    }
                    receive = (OperateResult<byte[]>)null;
                    ReceiveByte = (byte[])null;
                }
            }
            catch (Exception ex)
            {
                Thread.Sleep(20);
                socket?.Close();
                this.LogNet?.WriteException(this.ToString(), StringResources.Language.FileSendClientFailed, ex);
            }
        }

        private void ReceiveCallBack(IAsyncResult ir)
        {
            if (!(ir.AsyncState is Socket asyncState))
                return;
            try
            {
                asyncState.EndReceive(ir);
            }
            catch (Exception ex)
            {
                this.LogNet?.WriteException(this.ToString(), ex);
            }
            finally
            {
                asyncState?.Close();
            }
        }

        /// <summary>获取所有的文件信息</summary>
        /// <param name="dircPath">目标路径</param>
        /// <param name="logNet">日志信息</param>
        /// <returns>文件名的列表</returns>
        public static List<string> GetAllFiles(string dircPath, ILogNet logNet)
        {
            List<string> stringList = new List<string>();
            try
            {
                stringList.AddRange((IEnumerable<string>)Directory.GetFiles(dircPath));
            }
            catch (Exception ex)
            {
                logNet?.WriteWarn(nameof(GetAllFiles), ex.Message);
            }
            foreach (string directory in Directory.GetDirectories(dircPath))
                stringList.AddRange((IEnumerable<string>)NetSoftUpdateServer.GetAllFiles(directory, logNet));
            return stringList;
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("NetSoftUpdateServer[{0}]", (object)this.Port);
    }
}
