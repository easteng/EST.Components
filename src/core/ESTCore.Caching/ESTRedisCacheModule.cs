/**********************************************************************
*******命名空间： ESTCore.Caching
*******类 名 称： ESTCacheModule
*******类 说 明： 缓存模式封装
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/13/2021 10:10:36 AM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @Easten 2020-2021. All rights reserved ★ *********
***********************************************************************
 */
using Autofac;

using Silky.Lms.Core;
using Silky.Lms.Core.Modularity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using EasyCaching.Redis;
using EasyCaching.Core;
using Autofac.Extensions.DependencyInjection;

namespace ESTCore.Caching
{
    /// <summary>
    ///  缓存模块
    /// </summary>
    public class ESTRedisCacheModule: LmsModule
    {
        public override Task Initialize(ApplicationContext applicationContext)
        {
            var provider = applicationContext.ServiceProvider.GetService<IEasyCachingProviderFactory>();
            return base.Initialize(applicationContext); 
        }

        protected override void RegisterServices(ContainerBuilder builder)
        {
            var config = EngineContext.Current.Resolve<IConfiguration>();
            var services = new ServiceCollection();
            services.AddEasyCaching(options =>
            {
                //options.UseInMemory("defaut");
                //options.UseInMemory(config =>
                //{
                //    config.DBConfig = new EasyCaching.InMemory.InMemoryCachingOptions
                //    {
                //        ExpirationScanFrequency = 60,
                //        SizeLimit = 100,
                //    };
                //    config.MaxRdSecond = 120;
                //    config.EnableLogging = false;
                //    config.LockMs = 5000;
                //    config.SleepMs = 300;
                //}, "m2");

                options.UseRedis(c =>
                {
                    RedisDBOptions option = new RedisDBOptions();

                    option.Endpoints.Add(new EasyCaching.Core.Configurations.ServerEndPoint()
                    {
                        Host = config["Redis:Host"],
                        Port = int.Parse(config["Redis:Port"])
                    });
                    option.Database = int.Parse(config["Redis:Database"]);
                    option.ConnectionTimeout = 5000;
                    option.Password = config["Redis:Password"];
                    option.AllowAdmin = true;
                    c.DBConfig = option;
                    Action<Newtonsoft.Json.JsonSerializerSettings> jsonNET = x =>
                    {
                        x.NullValueHandling = Newtonsoft.Json.NullValueHandling.Include;
                        x.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
                    };
                    options.WithJson(jsonNET, "json.net_setting");

                }, "easten");
            });
            //services.AddSingleton<IESTCachingProvider, ESTCachingProvider>();
            builder.Populate(services);
        }
    }
}
