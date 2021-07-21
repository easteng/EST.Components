/**********************************************************************
*******命名空间： ESTCore.WebSocket.Config
*******类 名 称： ServerConfig
*******类 说 明： 
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/21/2021 10:43:00 AM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @Easten 2020-2021. All rights reserved ★ *********
***********************************************************************
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.WebSocket.Config
{
    /// <summary>
    ///  服务配置项
    /// </summary>
    public class SocketOption
    {
        public static string Section = "WebSocket";
        /// <summary>
        /// 服务IP地址
        /// </summary>
        public string ServerIp { get; set; }
        /// <summary>
        /// 服务端口  如果是客户端，则直接连接   如果是服务端，则直接启动
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 保持心跳的间隔，，单位毫秒
        /// </summary>
        public string KeepAlive { get; set; }
    }
}
