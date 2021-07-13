/**********************************************************************
*******命名空间： EstMTramsit.Contracts
*******类 名 称： SubmitOrderConsumer
*******类 说 明： 
*******作    者： Easten
*******机器名称： DESKTOP-EC8U0GP
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/10/2021 7:24:10 PM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @easten company 2021-2022. All rights reserved ★ *********
***********************************************************************
 */
using MassTransit;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstMTramsit.Contracts
{
    public class SubmitOrderConsumer :
       IConsumer<SubmitOrder>
    {
        private readonly IOrderSubmitter _orderSubmitter;

        public SubmitOrderConsumer(IOrderSubmitter submitter)
            => _orderSubmitter = submitter;
        public async Task Consume(ConsumeContext<SubmitOrder> context)
        {
            await context.Publish<OrderSubmitted>(new
            {
                context.Message.Name
            });
        }
    }
    public class IOrderSubmitter { }
    public class SubmitOrder
    {
        public string Name { get; set; }
    }
    public class OrderSubmitted
    {
        public string test { get; set; }
    }
}
