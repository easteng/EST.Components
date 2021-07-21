/**********************************************************************
*******命名空间： ESTCore.Message.Handler
*******类 名 称： ServerContext
*******类 说 明： 
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/21/2021 6:16:11 PM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @Easten 2020-2021. All rights reserved ★ *********
***********************************************************************
 */
using ESTCore.Common.WebSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.Message.Handler
{
    /// <summary>
    ///  服务上下文
    /// </summary>
    public class ServerContext
    {
        readonly WebSocketServer webSocketServer;
        public ServerContext(WebSocketServer webSocketServer = null)
        {
            this.webSocketServer = webSocketServer;
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="topic"></param>
        /// <param name="message"></param>
        public void Publish<T>(string topic,T message) where T:AbstractMessage
        {
            this.webSocketServer.PublishClientPayload(topic, message.ToString());
        }
    }
}
