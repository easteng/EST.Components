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
using Autofac;
using Autofac.Extensions.DependencyInjection;

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
        private ContainerBuilder containerBuilder;
        public MessageCenterServerBuilder(IServiceCollection serviceDescriptors = null, ContainerBuilder containerBuilder = null)
        {
            this.services = serviceDescriptors;
            this.serverEventHandler = new MessageCenterServerEventHandler();
            this.containerBuilder = containerBuilder;
        }

        /// <summary>
        /// 添加转发器，用于接收数据和转发数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void AddRepeater<T>(string name)
        {
            this.containerBuilder
                .RegisterType(typeof(T))
                .SingleInstance()
                .AsSelf()
                .Named(name,typeof(IMessageRepeaterHandler));
        }

        public void Build()
        {
            var config = EngineContext.Current.Resolve<IConfiguration>();
            try
            {
                wsServer = new WebSocketServer();              
                wsServer.OnClientApplicationMessageReceive += this.serverEventHandler.MessageReveive;
                wsServer.OnClientConnected += this.serverEventHandler.ClientConencted;
                wsServer.OnClientDisConnected += this.serverEventHandler.ClientDisConencted;
                wsServer.ServerStart(int.Parse(config["WebHost:Port"]));
                // 注册实例
                services.AddSingleton(wsServer);

                this.containerBuilder.Populate(this.services); // 合并服务
                Console.WriteLine($"WebSocket 服务已启动：{wsServer.Port}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("服务启动异常", ex);
            }
        }
    }
}
