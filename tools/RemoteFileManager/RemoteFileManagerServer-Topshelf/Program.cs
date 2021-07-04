using System;

using Topshelf;
namespace RemoteFileManagerServer_Topshelf
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(config =>
            {
                config.Service<ServiceProvider>(host =>
                {
                    host.ConstructUsing(a => new ServiceProvider());
                    host.WhenStarted(a => a.Start());
                    host.WhenStopped(a => a.Stop());
                })
                .RunAsLocalService()// 启动服务
                .SetDescription("EST 项目远程管理服务器，用户项目系统文件远程更细使用。");
                config.SetDisplayName("EST.RemoteFileManagerService v1.0");
                config.SetServiceName("ESTRemoteFileManagerService");
                config.StartAutomaticallyDelayed().StartAutomaticallyDelayed()
                .SetStartTimeout(TimeSpan.FromSeconds(2000));
            });
            
        }
    }
}
