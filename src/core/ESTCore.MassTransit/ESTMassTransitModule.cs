/**********************************************************************
*******命名空间： ESTCore.MassTransit
*******类 名 称： ESTMassTransitModule
*******类 说 明： 
*******作    者： Easten
*******机器名称： DESKTOP-EC8U0GP
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/10/2021 4:42:36 PM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @easten company 2021-2022. All rights reserved ★ *********
***********************************************************************
 */
using Autofac;
using Autofac.Extensions.DependencyInjection;

using MassTransit;
using MassTransit.MultiBus;
using MassTransit.RabbitMqTransport;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Silky.Lms.Core;
using Silky.Lms.Core.Modularity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.MassTransit
{
    public class ESTMassTransitModule : LmsModule
    {
        public override Task Initialize(ApplicationContext applicationContext)
        {
            return base.Initialize(applicationContext);
        }
        protected override void RegisterServices(ContainerBuilder builder)
        {
            base.RegisterServices(builder);
        }
    }
}
