using Autofac;

using Microsoft.Extensions.Configuration;

using Surging.Core.CPlatform;
using Surging.Core.CPlatform.Utilities;
using Surging.Core.ServiceHosting;
using Surging.Core.ServiceHosting.Internal;
using Surging.Core.ServiceHosting.Internal.Implementation;

using System;
using System.IO;

namespace ESTCore.Test.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new ServiceHostBuilder()
                .RegisterServices(builder =>
                {
                    builder.Register(a => new CPlatformContainer(ServiceLocator.Current));
                })
                .Configure(builder =>
                {
                    builder
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
                })
                .UseStartup<Startup>()
                .Build();
            host.Run();
            // Console.w
        }
    }
}
