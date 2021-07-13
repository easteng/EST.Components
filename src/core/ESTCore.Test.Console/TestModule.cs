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

using ESTCore.ORM.FreeSql;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Silky.Lms.Core.Modularity;
using System.Threading.Tasks;

namespace ESTCore.Test.Console
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
            service.AddHostedService<TestService>();
            builder.Populate(service);
            //base.RegisterServices(builder);
        }
    }
}
