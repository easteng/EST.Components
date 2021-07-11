///**********************************************************************
//*******命名空间： ESTCore.Test.Console
//*******类 名 称： Startup
//*******类 说 明： 
//*******作    者： Easten
//*******机器名称： DESKTOP-EC8U0GP
//*******CLR 版本： 4.0.30319.42000
//*******创建时间： 7/4/2021 11:22:04 AM
//*******联系方式： 1301485237@qq.com
//***********************************************************************
//******* ★ Copyright @easten company 2021-2022. All rights reserved ★ *********
//***********************************************************************
// */
//using Autofac;
//using Autofac.Extensions.DependencyInjection;

//using ESTCore.ORM.FreeSql;

//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;

//using Surging.Core.CPlatform;
//using Surging.Core.CPlatform.Utilities;

//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ESTCore.Test.Console
//{
//    public class Startup
//    {
//        public IConfiguration Configuration { get; }

//        private ContainerBuilder _builder;
//        public Startup(IConfigurationBuilder config)
//        {
//            Configuration = config.Build();
//        }

//        public IContainer ConfigureServices(ContainerBuilder builder)
//        {
//            var services = new ServiceCollection();
//            builder.RegisterModule<FreeSqlModule>();
//            services.AddSingleton<IConfiguration>(Configuration);
//            ConfigureLogging(services);
//            builder.Populate(services);
//            _builder = builder;
//            ServiceLocator.Current = builder.Build();
//            return ServiceLocator.Current;
//        }


//        public void Configure(IContainer app)
//        {
//            var freeSql=ServiceLocator.Current.Resolve<IFreeSql>();
//            var userResp=freeSql.GetRepository<User>();

//            var user = new User();
//            user.Name = "easten";
//            user.Password="password";


//            userResp.Insert(user);
//            // app.ComponentRegistry
//            //app.Resolve

//            ConfigureCache();
//        }



//        private void ConfigureLogging(IServiceCollection services)
//        {
//            //services.
//            //services.AddLogging();
//        }
//        private void ConfigureCache()
//        {
//            var freeSql = ServiceLocator.Current.Resolve<IFreeSql>();
//            var userResp = freeSql.GetRepository<User>();
//            var list= userResp.Where(a=>true).ToList();
//        }
//    }
//}
