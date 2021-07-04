using EstCommunication;
using EstCommunication.Enthernet;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteFilePusher
{
    public class RemoteFileManagerHandler
    {
        private IntegrationFileClient integrationFileClient;

        public bool ConnectionServer(string ip,int port,string token)
        {
            try
            {
                integrationFileClient = new IntegrationFileClient
                {
                    ConnectTimeOut = 5000,
                    ServerIpEndPoint = new System.Net.IPEndPoint(IPAddress.Parse(ip), port),
                    Token = Guid.Parse(token)
                };
                return true;
            }
            catch (Exception ex)
            {
                return false;
                
            }
        }

        public string[] GetRemoteFolders(string projectName="",string program="")
        {
            var arr=this.integrationFileClient.DownloadPathFolders("ESTDirectory", projectName, program);
            return arr.Content;
        }

        public event EventHandler<UpdateInfo> UpdateProgress;
        
        /// <summary>
        /// 上传文件 
        /// </summary>
        /// <param name="fileInfos">需要上传的文件</param>
        /// <param name="factory"></param>
        /// <param name="group">文件分组</param>
        /// <param name="id">文件的id</param>
        /// <returns></returns>
        public bool UploadFile(List<FileInfo> fileInfos,
            string projectName,
            string program,
            string desc="",
            string user="Easten"
            )
        {
            var task=Task.Factory.StartNew(() =>
            {
                foreach (var file in fileInfos)
                {
                    this.integrationFileClient.UploadFile(
                        file.FullName,// 文件名称
                        file.Name,//服务器上存储的路径
                        "ESTDirectory",// 一级目录
                        projectName,// 二级目录
                        program, // 三级目录
                        desc, // 描述文本
                        user,
                        UpdateReportProgress
                        );
                }
            });

            return task.IsCompleted;
        }
        /// <summary>
        /// 文件上传进度
        /// </summary>
        /// <param name="sended"></param>
        /// <param name="totle"></param>
        private void UpdateReportProgress(long sended,long totle)
        {
            this.UpdateProgress?.Invoke(null, new UpdateInfo(sended,totle));
        }
    }
    public class UpdateInfo
    {
        public UpdateInfo(long s,long t)
        {
            this.Sended = s;
            this.Totle = t;
        }
        public long Sended { get; set; }
        public long Totle{ get; set; }
    }
}
