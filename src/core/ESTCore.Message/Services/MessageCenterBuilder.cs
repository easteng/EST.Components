/**********************************************************************
*******命名空间： ESTCore.Message.Services
*******类 名 称： MessageCenterBuilder
*******类 说 明： 创建消息中心
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/23/2021 4:59:22 PM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @Easten 2020-2021. All rights reserved ★ *********
***********************************************************************
 */
using Autofac;
using Autofac.Extensions.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.Message.Services
{
    /// <summary>
    ///  消息中心注册
    /// </summary>
    public class MessageCenterBuilder
    {
        readonly ContainerBuilder builder;
        public MessageCenterBuilder(ContainerBuilder builder = null)
        {
            this.builder = builder;
        }
        /// <summary>
        /// 配置客户端信息
        /// </summary>
        /// <param name="options"></param>
        public void OptionClient(Action<MessageCenterClientBuilder> options)
        {
            var services = new ServiceCollection();
            var clientBuilder = new MessageCenterClientBuilder(services,this.builder);
            options.Invoke(clientBuilder);
        }
        /// <summary>
        /// 配置服务端信息
        /// </summary>
        /// <param name="options"></param>
        public void OptionServer(Action<MessageCenterServerBuilder> options)
        {
            var services = new ServiceCollection();
            var serverBuilder = new MessageCenterServerBuilder(services, this.builder);
            options.Invoke(serverBuilder);
        }
    }
}
