/**********************************************************************
*******命名空间： ESTCore.MassTransit
*******类 名 称： ESRMassTransitBuilderExtensions
*******类 说 明： 
*******作    者： Easten
*******机器名称： DESKTOP-EC8U0GP
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/10/2021 5:47:08 PM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @easten company 2021-2022. All rights reserved ★ *********
***********************************************************************
 */
using Autofac;
using Autofac.Extensions.DependencyInjection;

using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.RabbitMqTransport;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Silky.Lms.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.MassTransit
{
    public static class ESRMassTransitBuilderExtensions
    {
        public static void UseMassTransit<T>(this ContainerBuilder build,string chanl) where T :class, IConsumer
        {
            var config = EngineContext.Current.Resolve<IConfiguration>();
            var service = new ServiceCollection();
            service.AddMassTransit(conf =>
            {
                conf.AddConsumer<T>();
                conf.UsingRabbitMq((context, cif) =>
                {
                    cif.Host(config["Rabbitmq:Host"], c =>
                    {
                        c.Username(config["Rabbitmq:Usename"]);
                        c.Password(config["Rabbitmq:Password"]);
                    });
                    cif.ReceiveEndpoint(chanl, a =>
                    {
                        a.ConfigureConsumer<T>(context);
                    });
                });

            });
            build.Populate(service);
           

        }
    }
}
