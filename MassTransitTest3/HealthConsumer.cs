/**********************************************************************
*******命名空间： MassTransitTest3
*******类 名 称： HealthConsumer
*******类 说 明： 
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/16/2021 12:01:22 PM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @Easten 2020-2021. All rights reserved ★ *********
***********************************************************************
 */
using ESTCore.Message;

using MassTransit;

using Silky.Lms.Core.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassTransitTest3
{
    /// <summary>
    ///  
    /// </summary>
    //public class HealthConsumer : IConsumer<ServiceStatusMessage>
    //{
    //    public async Task Consume(ConsumeContext<ServiceStatusMessage> context)
    //    {
    //        var message = context.Message;
    //        Console.WriteLine($"{message.ServiceType.GetDisplay()}:{message.Status.GetDisplay()}：{message.Time}");
    //        await context.RespondAsync<CheckMessageStatus>(new { Ok = true });  // 相应
    //    }
    //}
}
