///**********************************************************************
//*******命名空间： MassTransitTest2
//*******类 名 称： MessageConsumer
//*******类 说 明： 
//*******作    者： Easten
//*******机器名称： DESKTOP-EC8U0GP
//*******CLR 版本： 4.0.30319.42000
//*******创建时间： 7/10/2021 11:14:25 PM
//*******联系方式： 1301485237@qq.com
//***********************************************************************
//******* ★ Copyright @easten company 2021-2022. All rights reserved ★ *********
//***********************************************************************
// */
//using ESTCore.MassTransit;


using ESTCore.Message;

using MassTransit;

using Silky.Lms.Core.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//namespace MassTransitTest3
//{
//    //public class MessageConsumer : IConsumer<IBaseMessage>
public class MessageConsumer : IObserver<ConsumeContext<ServiceStatusMessage>>
{
    public Task Consume(MassTransit.ConsumeContext<ServiceStatusMessage> context)
    {
        return Task.Run(async () =>
        {
                // var config = EngineContext.Current.Resolve<IConfiguration>();
                var message = context.Message;
          
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

    public void OnCompleted()
    {
        throw new NotImplementedException();
    }

    public void OnError(Exception error)
    {
        throw new NotImplementedException();
    }
    public void OnNext(ConsumeContext<ServiceStatusMessage> value)
    {
        var message = value.Message;    
        Console.WriteLine($"{message.ServiceType.GetDisplay()}:{message.Status.GetDisplay()}:{message.Time}");
    }
}