/**********************************************************************
*******命名空间： ESTCore.MassTransit
*******类 名 称： ESTMasssTransitConsumerFilter
*******类 说 明： 消费者过滤器
*******作    者： Easten
*******机器名称： DESKTOP-EC8U0GP
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/11/2021 10:30:52 AM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @easten company 2021-2022. All rights reserved ★ *********
***********************************************************************
 */
using GreenPipes;

using MassTransit;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.MassTransit
{
    public class ESTMasssTransitConsumerFilter<T> : IFilter<ConsumeContext<T>> where T:class
    {
        public void Probe(ProbeContext context)
        {
            //throw new NotImplementedException();
        }

        public Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            // 对数据进行过滤
            next.Send(context);
            return Task.CompletedTask;
        }
    }
}
