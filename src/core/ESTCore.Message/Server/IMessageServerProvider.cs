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
using ESTCore.Message.Message;

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
    public interface IMessageServerProvider
    {
        Task Publish(string topic, BaseMessage message);
    }

    public class MessageServerProvider : IMessageServerProvider
    {
        readonly WebSocketServer webSocketServer;
        public MessageServerProvider(WebSocketServer webSocketServer = null)
        {
            this.webSocketServer = webSocketServer;
        }
        /// <summary>
        /// 向指定主题发送消息数据
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task Publish(string topic, BaseMessage message)
        {
            return Task.Run(() => this.webSocketServer.PublishClientPayload(topic, message.ToString()));
        }
    }


}
