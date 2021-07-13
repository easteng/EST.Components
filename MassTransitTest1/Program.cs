using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using System;
using System.IO;

namespace MassTransitTest1
{
    class Program
    {
        static void Main(string[] args)
        {
            //Host
            //    .CreateDefaultBuilder()
            //    .UseConsoleLifetime()
            //    .ConfigureAppConfiguration((host, config) =>
            //    {
            //        config.SetBasePath(Directory.GetCurrentDirectory());
            //        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            //    })
            //    .RegisterLmsServices<TestModule>().Build().Run();
            var host = CreateHost().Build();
            host.Run();
            Console.WriteLine("Hello World!");
        }

        private static IHostBuilder CreateHost()
        {
            var host = Host
                .CreateDefaultBuilder()
                .UseConsoleLifetime()
                .ConfigureAppConfiguration((host, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                })
                .RegisterLmsServices<TestModule>();  // 注册服务
            return host;
        }
    }
}
