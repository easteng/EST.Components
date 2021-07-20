using System;
using System.Threading.Tasks;

using Autofac;

using Silky.Lms.Core.Modularity;
namespace ESTCore.Net
{
    /// <summary>
    /// 网络服务模块  提供强大的网络服务封装
    /// </summary>
    public class ESTNetModule: LmsModule
    {
        public override Task Initialize(ApplicationContext applicationContext)
        {
            return base.Initialize(applicationContext); 
        }

        protected override void RegisterServices(ContainerBuilder builder)
        {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            
            // 注册 需要的服务
            base.RegisterServices(builder);
        }
    }
}
