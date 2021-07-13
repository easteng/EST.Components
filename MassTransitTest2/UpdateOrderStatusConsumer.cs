/**********************************************************************
*******命名空间： MassTransitTest2
*******类 名 称： UpdateOrderStatusConsumer
*******类 说 明： 
*******作    者： Easten
*******机器名称： DESKTOP-EC8U0GP
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/10/2021 8:47:42 PM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @easten company 2021-2022. All rights reserved ★ *********
***********************************************************************
 */
using EstMTramsit.Contracts;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassTransitTest2
{
    public class UpdateOrderStatusConsumer : MassTransit.IConsumer<IUpdateOrderStatus>
    {
        public Task Consume(MassTransit.ConsumeContext<IUpdateOrderStatus> context)
        {
            return Task.CompletedTask;
        }
    }
}
