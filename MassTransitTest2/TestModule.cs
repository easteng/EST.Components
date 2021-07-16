﻿/**********************************************************************
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

using ESTCore.MassTransit;
using ESTCore.ORM.FreeSql;

using EstMTramsit.Contracts;

using MassTransit;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Silky.Lms.Core;
using Silky.Lms.Core.Modularity;

using System;
using System.Threading.Tasks;

namespace MassTransitTest2
{
    [DependsOn(typeof(FreeSqlModule))]
    public class TestModule: StartUpModule
    {
        public override Task Initialize(ApplicationContext applicationContext)
        {
            return base.Initialize(applicationContext);
        }

        protected override void RegisterServices(ContainerBuilder builder)
        {
            var service = new ServiceCollection();
            var config = EngineContext.Current.Resolve<IConfiguration>();
            service.AddMassTransit(conf =>
            {
              //  conf.AddConsumer<MessageConsumer>();
              //  conf.AddConsumer<UpdateOrderStatusConsumer>();
                conf.UsingRabbitMq((context, cif) =>
                {
                    cif.Host(config["Rabbitmq:Host"], c =>
                    {
                        c.Username(config["Rabbitmq:Username"]);
                        c.Password(config["Rabbitmq:Password"]);
                    });
                   
                    
                    cif.ReceiveEndpoint("server1", e =>
                    {
                        // e.Handler<IBaseMessage>()
                        e.Handler<IBaseMessage>(async context =>
                        {
                            await Task.Run(() =>
                            {
                                Console.WriteLine("Received By Handler:{0}", context.Message.Name);
                            });
                        },cc=> {
                            cc.UseConcurrentMessageLimit(200);
                        });
                       // e.Observer(new MessageConsumer());
                        // e.Consumer<UpdateOrderStatusConsumer>(context);
                    });
                });

            });
            //service.AddHostedService<TestService>();
            service.AddMassTransitHostedService();
            builder.Populate(service);
           // base.RegisterServices(builder);
        }
    }
}
