/**********************************************************************
*******命名空间： ESTCore.Message.Services
*******类 名 称： MessageCenterClientBuilder
*******类 说 明： 
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/21/2021 5:22:30 PM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @Easten 2020-2021. All rights reserved ★ *********
***********************************************************************
 */
using Autofac;
using Autofac.Extensions.DependencyInjection;

using Castle.Core.Internal;

using ESTCore.Common.WebSocket;
using ESTCore.Message.Client;
using ESTCore.Message.Handler;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Silky.Lms.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESTCore.Message.Services
{
    /// <summary>
    ///  消息中心客户端构建
    /// </summary>
    public class MessageCenterClientBuilder:IDisposable
    {
        private IServiceCollection services { get; }
        private WebSocketClient client { get; set; }
        private ContainerBuilder containerBuilder;
        private MessageCenterClientEventHandler messageCenterClientEvent; 
        private bool IsDispose { get; set; }
        /// <summary>
        /// 是否成功连接服务
        /// </summary>
        private bool IsConnect { get; set; }
        /// <summary>
        /// 客户端需要订阅的主题
        /// </summary>
        public List<string> Topic { get; set; }=new List<string>(); 
        /// <summary>
        /// 服务连接失败重试的事件间隔
        /// </summary>
        public TimeSpan ReConnectTimeSpan { get; set; }

        public MessageCenterClientBuilder(IServiceCollection serviceDescriptors = null, ContainerBuilder containerBuilder = null)
        {
            this.services = serviceDescriptors;
            this.containerBuilder = containerBuilder;
            this.messageCenterClientEvent = new MessageCenterClientEventHandler();
        }
        /// <summary>
        /// 添加数据接收机
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void AddReceiver<T>(Action<ClientOption> option)
        {
            var opt = new ClientOption();
            option.Invoke(opt);
            containerBuilder.RegisterType<T>()
                .OwnedByLifetimeScope()
                .AsSelf()
                .Named<IMessageReceiverHandler>(opt.Name);

            if (opt.Topic.IsNullOrEmpty())
                this.Topic.Add(opt.Name);
            else
                this.Topic.Add(opt.Topic);
        }


        public void Build()
        {
            var config = EngineContext.Current.Resolve<IConfiguration>();
            try
            {
                var ip = config["WebHost:Ip"];
                var port = int.Parse(config["WebHost:port"]);
                client = new WebSocketClient(ip,port);
                client.OnClientApplicationMessageReceive += messageCenterClientEvent.OnClientApplicationMessageReceive;
                client.OnClientConnected += messageCenterClientEvent.OnClientConnected;
                client.OnNetworkError += messageCenterClientEvent.OnNetworkError;
                services.AddSingleton(client); // 注册服务
                services.AddSingleton<IMessageClientProvider, MessageClientProvider>(); // 注册消息提供
                this.containerBuilder.Populate(services);
                var res=client.ConnectServer(Topic.ToArray());
                if (res.IsSuccess)
                {
                    MessageClientState.IsSuccess = true;
                    Console.WriteLine("数据中心服务连接成功！");
                }
                Console.WriteLine(res);
                // 启动服务重连线程
                StartReconnectServer();
            }
            catch (Exception ex)
            {
                Console.WriteLine("数据中心连接失败");
            }
        }

        /// <summary>
        /// 启动重新连接服务的线程
        /// </summary>
        private void StartReconnectServer()
        {
            var thread = new Thread(() =>
              {
                  if (client != null)
                  {
                      while (!IsDispose)
                      {
                          if (!MessageClientState.IsSuccess)
                          {
                              var res = client.ConnectServer(Topic.ToArray());
                              if (res.IsSuccess)
                              {
                                  MessageClientState.IsSuccess = true;
                                  Console.WriteLine("数据中心服务连接成功！");
                              }
                              else
                              {
                                  Console.WriteLine("数据中心服务重连中....");
                              }
                          }
                          Thread.Sleep(ReConnectTimeSpan);
                      }
                  }
              });
            thread.Start();
        }

        public void Dispose()
        {
            IsDispose = true;
        }
    }
}
