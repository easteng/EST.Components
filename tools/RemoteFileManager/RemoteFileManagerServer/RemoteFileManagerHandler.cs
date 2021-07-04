using EstCommunication.Enthernet;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteFileManagerServer
{
    public class RemoteFileManagerHandler
    {

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
            var dir= this.ultimateFileServer.GetGroupFromFilePath(dic);
            return dir.JsonArrayContent;
        }
    }
}
