/**********************************************************************
*******命名空间： ESTCore.Message
*******类 名 称： CommandSender
*******类 说 明： 
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/16/2021 11:32:03 AM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @Easten 2020-2021. All rights reserved ★ *********
***********************************************************************
 */
using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.Message
{
    /// <summary>
    ///  命令发送者
    /// </summary>
    public class CommandSender : ICommandSender
    {
        readonly IConfiguration config;
        public CommandSender(IConfiguration config = null)
        {
            this.config = config;
        }
        /// <summary>
        /// 发送命令指定的消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>

        public async Task Send<TMessage>(TMessage message) where TMessage : IBaseMessage
        {
            var url = new Uri($"{config["Rabbitmq:Host"]}/{message.Topic}");
        }
    }

    public class CommandSender<T> : ICommandSender<T> where T : class, IBaseMessage
    {
        readonly IConfiguration config;
        public CommandSender( IConfiguration config = null)
        {
            this.config = config;
        }
        /// <summary>
        /// 推送消息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task Push(ServiceType type, ServiceStatus status)
        {
            var messsage = MessageCenter.CreateMessage<T>(type, status);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task Send(ServiceType type, ServiceStatus status)
        {
            var messsage = MessageCenter.CreateMessage<T>(type, status);
            var url = new Uri($"{config["Rabbitmq:Host"]}/{messsage.Topic}");
        }
    }
}
