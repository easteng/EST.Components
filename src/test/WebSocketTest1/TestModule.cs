/**********************************************************************
*******命名空间： ESTCore.Test.Console
*******类 名 称： TestModule
*******类 说 明： 
*******作    者： Easten
*******机器名称： DESKTOP-EC8U0GP
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/10/2021 11:49:12 AM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @easten company 2021-2022. All rights reserved ★ *********
***********************************************************************
 */
using Autofac;
using Autofac.Extensions.DependencyInjection;

using ESTCore.WebSocket;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Silky.Lms.Core;
using Silky.Lms.Core.Modularity;

using System;
using System.Threading.Tasks;

namespace WebSocketTest1
{
    [DependsOn(typeof(ESTWebSocketModule))]
    public class TestModule: StartUpModule
    {
        public override Task Initialize(ApplicationContext applicationContext)
        {
            return base.Initialize(applicationContext);
        }

        protected override void RegisterServices(ContainerBuilder builder)
        {
            //service.AddMassTransit(conf =>
            //{
            //    //  conf.AddConsumer<MessageConsumer>();
            //    //  conf.AddConsumer<UpdateOrderStatusConsumer>();
            //    conf.UsingRabbitMq((context, cif) =>
            //    {
            //        cif.Host(config["Rabbitmq:Host"], c =>
            //        {
            //            c.Username(config["Rabbitmq:Username"]);
            //            c.Password(config["Rabbitmq:Password"]);
            //        });
            //        cif.ReceiveEndpoint("ServiceState", e =>
            //        {
            //            e.Instance(new HealthConsumer());
            //            // e.Handler<IBaseMessage>(new );
            //            //e.BindDeadLetterQueue("ServiceState");
            //        });
            //    });

            //    conf.AddRequestClient<CheckMessageStatus>();// 添加相应客户端

            //});
            //service.AddMassTransitHostedService();
            //builder.Populate(service);
            var service = new ServiceCollection();
            var config = EngineContext.Current.Resolve<IConfiguration>();
            service.AddESTWebSocket(builder=> {
                builder.AddConfig(a =>
                {
                    a = ESTCore.WebSocket.Config.SocketInstanceType.Server;
                });
              //  builder.AddWSServerHandler<TestHandler>();
                builder.AddWSCommand<CommandHandler>();
                builder.Build();
                
            });
            builder.Populate(service);
           // base.RegisterServices(builder);
        }
    }
}
