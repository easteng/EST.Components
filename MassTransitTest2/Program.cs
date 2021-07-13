
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using System;

namespace MassTransitTest2
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = CreateHost().Build();
            host.Run();

            //using (var bus = RabbitHutch.CreateBus("host=47.105.150.88;username=admin;password=zbd123456"))
            //{
            //    var input = String.Empty;
            //    Console.WriteLine("Enter a message. 'Quit' to quit.");
            //    while ((input = Console.ReadLine()) != "Quit")
            //    {
            //        bus.PubSub.Publish(new TextMessage { Text = input },"aaa");
            //        Console.WriteLine("Message published!");
            //    }
            //}


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
