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

//using EstMTramsit.Contracts;

//using MassTransit;

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace MassTransitTest2
//{
//    public class MessageConsumer : IObserver<ConsumeContext<>>
//    {
//        //public Task Consume(MassTransit.ConsumeContext<IBaseMessage> context)
//        //{
//        //    return Task.Run(async () =>
//        //    {
//        //       // var config = EngineContext.Current.Resolve<IConfiguration>();
//        //        var message = context.Message;
//        //        Console.WriteLine($"收到消息:{message.Name}");
//        //        //// 发送命令
//        //        //var endPoint = await context.GetSendEndpoint(new Uri($"{config["Rabbitmq:Host"]}/test/order-queue"));
//        //        //await endPoint.Send<IUpdateOrderStatus>(new
//        //        //{
//        //        //    CommandId = Guid.NewGuid(),
//        //        //    Text = "修改订单状态",
//        //        //    Date = DateTime.Now
//        //        //});
//        //    });
//        //}
//        public void OnCompleted()
//        {
//            throw new NotImplementedException();
//        }

//        public void OnError(Exception error)
//        {
//            throw new NotImplementedException();
//        }

//        public void OnNext(ConsumeContext<> value)
//        {
//            Console.WriteLine("Received By Observer:{0}", value.Message.Name);
//        }
//    }
//}
