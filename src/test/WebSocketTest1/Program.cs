using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using System;

namespace WebSocketTest1
{
    class Program
    {
        static void Main(string[] args)
        {
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
