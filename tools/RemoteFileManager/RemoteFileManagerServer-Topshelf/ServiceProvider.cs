using EstCommunication.Enthernet;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteFileManagerServer_Topshelf
{
    public class ServiceProvider
    {
        /// <summary>
        /// 启动服务
        /// </summary>
        public void Start()
        {
            try
            {
                var projectDirectory = Path.Combine(Directory.GetCurrentDirectory(), "ESTProjects");
                if (!Directory.Exists(projectDirectory))
                {
                    Directory.CreateDirectory(projectDirectory);
                }
                this.ultimateFileServer = new UltimateFileServer();
                this.ultimateFileServer.FilesDirectoryPath = projectDirectory;
                this.ultimateFileServer.Token = Guid.Parse("D3A3690E-3D1A-4EC2-9F7C-1496CF5811FD");
                this.ultimateFileServer.ServerStart(5188);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        /// <summary>
        /// 服务停止
        /// </summary>
        public void Stop()
        {
            if (this.ultimateFileServer.IsStarted)
                this.ultimateFileServer.ServerClose();
        }

        private UltimateFileServer ultimateFileServer;
        /// <summary>
        /// 启动文件服务
        /// </summary>
        /// <param name="port"></param>
        /// <param name="mainpath"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool StartServer(int port, string mainpath, string token)
        {
            try
            {
                this.ultimateFileServer = new UltimateFileServer();
                ultimateFileServer.FilesDirectoryPath = mainpath;
                this.ultimateFileServer.Token = Guid.Parse(token);
                this.ultimateFileServer.ServerStart(port);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 关闭服务
        /// </summary>
        public void CloseServer()
        {
            if (this.ultimateFileServer != null && this.ultimateFileServer.IsStarted)
                this.ultimateFileServer.ServerClose();
        }

        public string GetFiles(string dic)
        {
            var dir = this.ultimateFileServer.GetGroupFromFilePath(dic);
            return dir.JsonArrayContent;
        }
    }
}
