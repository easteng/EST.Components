/**********************************************************************
*******命名空间： ESTCore.Message.Services
*******类 名 称： MessageCenterBuilder
*******类 说 明： 消息中心服务端构建
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/21/2021 5:20:24 PM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @Easten 2020-2021. All rights reserved ★ *********
***********************************************************************
 */
using ESTCore.Common.WebSocket;
using ESTCore.Message.Handler;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Silky.Lms.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.Message
{
    /// <summary>
    ///  消息中心服务端构建
    /// </summary>
    public class MessageCenterServerBuilder
    {
        private IServiceCollection services { get; }
        private WebSocketServer wsServer { get; set; }
        private MessageCenterServerEventHandler serverEventHandler;
        public MessageCenterServerBuilder(IServiceCollection serviceDescriptors = null, 
            MessageCenterServerEventHandler serverEventHandler = null)
        {
            this.services = serviceDescriptors;
            this.serverEventHandler = serverEventHandler;
        }

        /// <summary>
        /// 添加转发器，用于接收数据和转发数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void AddRepeater<T>(Action<AbstractMessage> message)
        {
            AbstractMessage msg = null;
            message.Invoke(msg);            
            services.AddSingleton(typeof(IMessageRepeaterHandler<>), typeof(T));
            services.AddSingleton(msg);
        }

        public void Build()
        {
            var config = EngineContext.Current.Resolve<IConfiguration>();
            try
            {
                wsServer = new WebSocketServer();
                wsServer.ServerStart(int.Parse(config["WebSocket:Port"]));
                wsServer.OnClientApplicationMessageReceive += this.serverEventHandler.MessageReveive;
                wsServer.OnClientConnected += this.serverEventHandler.ClientConencted;
                wsServer.OnClientDisConnected += this.serverEventHandler.ClientDisConencted;
                // 注册实例
                services.AddSingleton(wsServer);
                Console.WriteLine($"WebSocket 服务已启动：{wsServer.Port}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("服务启动异常", ex);
            }
        }
    }
}
