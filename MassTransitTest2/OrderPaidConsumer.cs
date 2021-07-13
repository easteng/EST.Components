/**********************************************************************
*******命名空间： MassTransitTest2
*******类 名 称： OrderPaidConsumer
*******类 说 明： 
*******作    者： Easten
*******机器名称： DESKTOP-EC8U0GP
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/10/2021 8:29:47 PM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @easten company 2021-2022. All rights reserved ★ *********
***********************************************************************
 */
using EstMTramsit.Contracts;

using MassTransit;

using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

using Silky.Lms.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassTransitTest2
{
    public class OrderPaidConsumer : MassTransit.IConsumer<IOrderPaid>
    {
     
        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Consume(ConsumeContext<IOrderPaid> context)
        {
            return Task.Run(async () =>
            {
                var config=EngineContext.Current.Resolve<IConfiguration>();
                var message = context.Message;
                Console.WriteLine($"收到消息:{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
                Console.WriteLine(JsonConvert.SerializeObject(message));
                //// 发送命令
                //var endPoint = await context.GetSendEndpoint(new Uri($"{config["Rabbitmq:Host"]}/test/order-queue"));
                //await endPoint.Send<IUpdateOrderStatus>(new
                //{
                //    CommandId = Guid.NewGuid(),
                //    Text = "修改订单状态",
                //    Date = DateTime.Now
                //});
            });
        }
    }
}
