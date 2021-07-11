using Autofac;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;

namespace ESTCore.Test.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = CreateHost().Build();
            //Host.CreateDefaultBuilder()
            //.ConfigureWebHostDefaults
            //.RegisterServices(builder =>
            //{
            //    builder.Register(a => new CPlatformContainer(ServiceLocator.Current));
            //})
            //.Configure(builder =>
            //{
            //    builder
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
            //})
            //.UseServer(a => { })
            //.()                
            //.UseStartup<Startup>()
            //.Build();
            host.Run();
            System.Console.WriteLine("服务已启动");
            // Console.w
        }

        private static IHostBuilder CreateHost()
        {
            var host = Host
                .CreateDefaultBuilder()
                .UseConsoleLifetime()
                .ConfigureAppConfiguration((host,config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                })
                .RegisterLmsServices<TestModule>();  // 注册服务
            return host;
        }
    }
}
