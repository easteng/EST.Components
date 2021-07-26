/**********************************************************************
*******命名空间： ESTCore.Message.Handler
*******类 名 称： MessageCenterClientEventHandler
*******类 说 明： 消息中心客户端事件帮助类
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/23/2021 11:01:09 AM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @Easten 2020-2021. All rights reserved ★ *********
***********************************************************************
 */
using ESTCore.Common.WebSocket;
using ESTCore.Message.Client;
using ESTCore.Message.Message;

using Silky.Lms.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.Message.Handler
{
    /// <summary>
    ///  消息中心客户端事件帮助类
    /// </summary>
    public class MessageCenterClientEventHandler
    {
        public void OnClientApplicationMessageReceive(WebSocketMessage message)
        {
            // 接收到消息，进行处理，有可能消息，有可能是命令消息
            if (message != null)
            {
                // 处理线程报错问题
                try
                {
                    var messageInstance = BaseMessage.ResolveMessage(message.Payload);
                    // 获取转换机实例并发送消息
                    var repeater = EngineContext.Current.ResolveNamed<IMessageReceiverHandler>(messageInstance.Topic);
                    if (repeater != null)
                    {
                        repeater.Receive(messageInstance);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                
            }
        }

        public void OnNetworkError(object sender, EventArgs e)
        {
            // 断开服务器
            MessageClientState.IsSuccess=false;
        }
        public void OnClientConnected()
        {
            MessageClientState.IsSuccess = true;
        }
    }
}
