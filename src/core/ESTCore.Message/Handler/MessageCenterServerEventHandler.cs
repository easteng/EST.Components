/**********************************************************************
*******命名空间： ESTCore.Message.Handler
*******类 名 称： MessageCenterServerEventHandler
*******类 说 明： 
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/21/2021 5:40:23 PM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @Easten 2020-2021. All rights reserved ★ *********
***********************************************************************
 */
using ESTCore.Common.WebSocket;
using ESTCore.Message.Message;

using Newtonsoft.Json;

using Silky.Lms.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.Message.Handler
{
    /// <summary>
    ///  消息中心服务端 事件接收器
    /// </summary>
    public class MessageCenterServerEventHandler
    {
        WebSocketServer webSocket;
        private ServerContext serverContext;
        public MessageCenterServerEventHandler(WebSocketServer webSocket = null)
        {
            this.webSocket = webSocket;
            this.serverContext = new ServerContext(this.webSocket);
        }
        /// <summary>
        /// 接收消息信息
        /// </summary>
        /// <param name="session"></param>
        /// <param name="message"></param>
        public void MessageReveive(WebSocketSession session, WebSocketMessage message)
        {
            // 对消息进行处理 
            if (message != null)
            {
                var messageInstance = new BaseMessage(message.Payload);
                // 获取转换机实例并发送消息
                var repeater = EngineContext.Current.ResolveNamed<IMessageRepeaterHandler>(messageInstance.Topic);
                if (repeater != null)
                {
                    repeater.Repeater(messageInstance, this.serverContext);
                }
            }
        }

        /// <summary>
        /// 客户端连接事件
        /// </summary>
        /// <param name="session"></param>
        public void ClientConencted(WebSocketSession session)
        {

        }

        /// <summary>
        /// 客户端断开事件
        /// </summary>
        /// <param name="session"></param>
        public void ClientDisConencted(WebSocketSession session)
        {

        }
    }
}
