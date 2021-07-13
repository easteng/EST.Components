/**********************************************************************
*******命名空间： MassTransitTest1
*******类 名 称： TestService
*******类 说 明： 
*******作    者： Easten
*******机器名称： DESKTOP-EC8U0GP
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/10/2021 4:37:06 PM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @easten company 2021-2022. All rights reserved ★ *********
***********************************************************************
 */
using ESTCore.MassTransit;

using EstMTramsit.Contracts;

using GreenPipes;

using MassTransit;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransitTest1
{
    public class TestService : IHostedService
    {
        IBus bus;
        IConfiguration config;
        public TestService(IBus bus = null, IConfiguration config = null)
        {
            this.bus = bus;
            this.config = config;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            //throw new NotImplementedException();
            this.SendOrder();
           return Task.CompletedTask;
        }

        public async Task SendOrder()
        {
            var index = 0;
            while (true)
            {
                index++;
                var point =await bus.GetSendEndpoint(new Uri($"{config["Rabbitmq:Host"]}/message"));
                //await point.Send<IBaseMessage>(new BaseMessage() { Name = index.ToString() });
                await bus.Publish<IBaseMessage>(new {Name= index});
                Console.WriteLine(index.ToString());
                await Task.Delay(100);
            }
            // 发布事件
           
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
           return Task.CompletedTask;
            //throw new NotImplementedException();
        }
    }
}
