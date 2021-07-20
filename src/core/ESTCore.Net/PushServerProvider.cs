/**********************************************************************
*******命名空间： ESTCore.Net
*******类 名 称： PushServerProvider
*******类 说 明： 
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/20/2021 7:08:02 PM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @Easten 2020-2021. All rights reserved ★ *********
***********************************************************************
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESTCore.Common.Enthernet;
using Microsoft.Extensions.Configuration;
namespace ESTCore.Net
{
    /// <summary>
    ///  服务提供者数据实现
    /// </summary>
    public class PushServerProvider: IPushServerProvider
    {
        private NetPushServer netPushServer;
        private readonly IConfiguration configuration;

        public PushServerProvider(IConfiguration configuration = null)
        {
            this.configuration = configuration;
            netPushServer = new NetPushServer();
            var hostPort = this.configuration["Server:Port"];
            if (string.IsNullOrEmpty(hostPort)) return;
            netPushServer.Port = int.Parse(hostPort);
            ServerStartAsync();
        }

        /// <summary>
        /// 启动推送服务
        /// </summary>
        /// <returns></returns>
        public Task ServerStartAsync()
        {
            try
            {
               return Task.Run(() =>
                {
                    if (this.netPushServer.IsStarted)
                    {
                        Console.WriteLine("服务正在运行");
                    }
                    else
                    {
                        netPushServer.ServerStart();
                        Console.WriteLine("服务启动");
                        Console.WriteLine($"监测地址：localhost:{netPushServer.Port}");
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"服务异常:{ex}");
                return Task.CompletedTask;
            }
        }

        /// <summary>
        /// 关闭推送服务
        /// </summary>
        /// <returns></returns>
        public Task ServerStop()
        {
            return Task.Run(() =>
            {
                if (this.netPushServer != null && this.netPushServer.IsStarted)
                {
                    this.netPushServer.ServerClose();
                }
            });
        }
    }
}
