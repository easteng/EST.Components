/**********************************************************************
*******命名空间： ESTCore.WebSocket.Services
*******类 名 称： ESTWebSocketServerBuilder
*******类 说 明： 
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/21/2021 11:38:21 AM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @Easten 2020-2021. All rights reserved ★ *********
***********************************************************************
 */
using ESTCore.Common.WebSocket;
using ESTCore.WebSocket.Config;
using ESTCore.WebSocket.Handlers;
using ESTCore.WebSocket.Message;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Silky.Lms.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.WebSocket.Services
{
    /// <summary>
    ///  web socket 服务服务创建
    /// </summary>
    public class ESTWebSocketServerBuilder
    {
        private WebSocketServer wsServer;
        private IServiceCollection services { get; }
        private SocketInstanceType socketInstanceType { get; set; }
        public ESTWebSocketServerBuilder(IServiceCollection serviceDescriptors = null)
        {
            this.services = serviceDescriptors;
        }
        
        public void AddConfig(Action<SocketInstanceType> type)
        {
            type.Invoke(socketInstanceType);
        }

        public void AddWSServerHandler<THandler>()
        {
            services.AddSingleton(typeof(IWSServerMessageBase), typeof(THandler));
        }
        /// <summary>
        /// 添加socket 命令 用于有权限的客户端向服务器发送命令，服务再次转发的作用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void AddWSCommand<T>()        
        {
            services.AddSingleton(typeof(IWSCommandProvider),typeof(T));
        }

        public void Build()
        {
            var config = EngineContext.Current.Resolve<IConfiguration>();
            if (socketInstanceType == SocketInstanceType.Server)
            {
                //  opt.ServerIp = config["WebSocket:ServerIp"];
                // 创建 sockert 服务
                try
                {
                    wsServer = new WebSocketServer();
                    wsServer.ServerStart(int.Parse(config["WebSocket:Port"]));
                    wsServer.OnClientApplicationMessageReceive += WsServer_OnClientApplicationMessageReceive;
                    wsServer.OnClientConnected += WsServer_OnClientConnected;
                    wsServer.OnClientDisConnected += WsServer_OnClientDisConnected;
                    services.AddSingleton<WebSocketServer>(wsServer);
                    Console.WriteLine($"WebSocket 服务已启动：{wsServer.Port}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("服务启动异常", ex);
                }
               
            }
        }

        private void WsServer_OnClientDisConnected(WebSocketSession session)
        {
            //throw new NotImplementedException();
        }

        private void WsServer_OnClientConnected(WebSocketSession session)
        {
            //throw new NotImplementedException();
        }

        private void WsServer_OnClientApplicationMessageReceive(WebSocketSession session, WebSocketMessage message)
        {
            // 接受到数据
            // 对数据进行判断处理
            // 如果 是明码，说明是操作客户端向服务端发送消息，服务端转发到对应数据采集服务，执行操作。
            // 如果 是暗码，说明消息是定向传播，要根据主题进行相关操作
            if (message.HasMask)
            {
                // 存在掩码
            }
            else
            {
                // 不存在掩码  指命令得发送，要解析成具体的命令进行发送
                var msg = message.Payload.GetMessage<CommandMessage>();
                var wsProvicer = EngineContext.Current.ServiceProvider.GetService<IWSCommandProvider>();
                wsProvicer.Execute(msg);
            }


            //var msg = message..GetMessage();
           
            //wsProvicer.Receive("123");
        }
    }
}
