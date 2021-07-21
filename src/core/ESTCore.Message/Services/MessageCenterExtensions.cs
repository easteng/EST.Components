/**********************************************************************
*******命名空间： ESTCore.Message.Services
*******类 名 称： MessageCenterExtensions
*******类 说 明： 消息中心扩展类
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/21/2021 5:20:46 PM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @Easten 2020-2021. All rights reserved ★ *********
***********************************************************************
 */
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.Message.Services
{
    /// <summary>
    ///  消息中心扩展类
    /// </summary>
    public static class MessageCenterExtensions
    {
        /// <summary>
        /// 使用消息中信息服务端组件，该服务将成为Http宿主服务 来接收客户端的订阅消息
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="builder"></param>
        public static void UseMessageCenterServer(this IServiceCollection serviceCollection,
          Action<MessageCenterServerBuilder> builder)
        {
            var messageCore = new MessageCenterServerBuilder(serviceCollection);
            builder.Invoke(messageCore);
        }

        /// <summary>
        /// 使用消息中心客户端 ，当前服务通过订阅服务端主题来获取消息内容
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="builder"></param>
        public static void UseMessageCenterClient(this IServiceCollection serviceCollection,
              Action<MessageCenterClientBuilder> builder)
        {
            var messageCore = new MessageCenterClientBuilder(serviceCollection);
            builder.Invoke(messageCore);
        }
    }
}
