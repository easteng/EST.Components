/**********************************************************************
*******命名空间： ESTCore.Message
*******类 名 称： ESTMessageModule
*******类 说 明： 消息模块
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/16/2021 11:41:25 AM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @Easten 2020-2021. All rights reserved ★ *********
***********************************************************************
 */
using Autofac;
using Autofac.Extensions.DependencyInjection;

using ESTCore.Message.Handler;

using Microsoft.Extensions.DependencyInjection;

using Silky.Lms.Core.Modularity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.Message
{
    /// <summary>
    ///  消息模块
    /// </summary>
    
    public class ESTMessageModule: LmsModule
    {
        protected override void RegisterServices(ContainerBuilder builder)
        {
            var services = new ServiceCollection();

            //注册服务端接收消息的事件帮助类
            services.AddSingleton< MessageCenterServerEventHandler>(new MessageCenterServerEventHandler());

            services.AddSingleton<ICommandSender, CommandSender>();
            // 注入服务
            services.AddTransient(typeof(ICommandSender<>),typeof(CommandSender<>));
            builder.Populate(services);
        }
    }
}
