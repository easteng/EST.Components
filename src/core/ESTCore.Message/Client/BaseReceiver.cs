/**********************************************************************
*******命名空间： ESTCore.Message.Client
*******类 名 称： BaseReceiver
*******类 说 明： 基础接收者定义
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/23/2021 2:25:43 PM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @Easten 2020-2021. All rights reserved ★ *********
***********************************************************************
 */
using ESTCore.Message.Handler;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.Message.Client
{
    /// <summary>
    ///  
    /// </summary>
    public class BaseReceiver<T> : IMessageReceiverHandler<T> where T : AbstractMessage
    {
        public virtual Task Receive(StandardMessage<T> message, ServerContext context)
        {
            return Task.CompletedTask;
        }
    }
}
