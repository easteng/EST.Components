/**********************************************************************
*******命名空间： ESTCore.Message.Client
*******类 名 称： MessageClientProvider
*******类 说 明： 消息客户端实现
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/23/2021 11:31:48 AM
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

namespace ESTCore.Message.Client
{
    /// <summary>
    ///  消息客户端实现
    /// </summary>
    public class MessageClientProvider: IMessageClientProvider
    {
        readonly WebSocketClient webSocketClient;
        public MessageClientProvider(WebSocketClient webSocketClient = null)
        {
            this.webSocketClient = webSocketClient;
        }

        public Task SendCommand<T>(T Command) where T : AbstractCommand
        {
            if (webSocketClient != null)
            {
                webSocketClient.SendServer(Command.ToString());
            }

            return Task.CompletedTask;
        }

        public Task SendMessage<T>(T message) where T : AbstractMessage
        {
            try
            {
                if (webSocketClient != null)
                {
                    webSocketClient.SendServer(message.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据中心服务未连接，发送失败...");
            }
           
            return Task.CompletedTask;
        }
    }
}
