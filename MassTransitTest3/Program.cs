

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using System;

namespace MassTransitTest3
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = CreateHost().Build();
            host.Run();
            Console.WriteLine("Hello World!");
            //using (var bus = RabbitHutch.CreateBus("host=47.105.150.88;username=admin;password=zbd123456"))
            //{
            //    bus.PubSub.Subscribe<TextMessage>("test", HandleTextMessage,a=>a.WithTopic("aaa"));
            //    Console.WriteLine("Listening for messages. Hit <return> to quit.");
            //    Console.ReadLine();
            //}
        }
        //static void HandleTextMessage(TextMessage textMessage)
        //{
        //    Console.ForegroundColor = ConsoleColor.Red;
        //    Console.WriteLine("Got message: {0}", textMessage.Text);
        //    Console.ResetColor();
        //}
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
