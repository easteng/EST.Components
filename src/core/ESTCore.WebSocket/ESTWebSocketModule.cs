/**********************************************************************
*******命名空间： ESTCore.WebSocket
*******类 名 称： ESTWebSocketModule
*******类 说 明： 消息服务模块
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/21/2021 10:41:36 AM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @Easten 2020-2021. All rights reserved ★ *********
***********************************************************************
 */
using Autofac;
using Autofac.Extensions.DependencyInjection;

using ESTCore.WebSocket.Config;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Silky.Lms.Core;
using Silky.Lms.Core.Modularity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.WebSocket
{
    /// <summary>
    ///  消息服务模块
    /// </summary>
    public class ESTWebSocketModule: LmsModule
    {
        public override Task Initialize(ApplicationContext applicationContext)
        {
           var aa= applicationContext.ServiceProvider.GetService<IConfigurationProvider>();
            return base.Initialize(applicationContext);
        }

        protected override void RegisterServices(ContainerBuilder builder)
        {
            var config = EngineContext.Current.Resolve<IConfiguration>();
            var services = new ServiceCollection();
            services.Configure<SocketOption>(opt=>
            {
                opt.Port = int.Parse(config["WebSocket:Port"]);
                opt.ServerIp = config["WebSocket:ServerIp"];
            });
            builder.Populate(services);
           base.RegisterServices(builder); 
        }
    }
}
