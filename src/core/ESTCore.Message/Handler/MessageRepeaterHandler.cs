/**********************************************************************
*******命名空间： ESTCore.Message.Handler
*******类 名 称： MessageRepeaterHandler
*******类 说 明： 
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/21/2021 5:32:57 PM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @Easten 2020-2021. All rights reserved ★ *********
***********************************************************************
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.Message.Handler
{
    /// <summary>
    ///  消息转发器 用于解说不同客户端的消息，并转发掉
    /// </summary>
    public class MessageRepeaterHandler
    {

    }

    public interface IMessageRepeaterHandler<T> where T:class
    {
        Task Repeater(StandardMessage<T> message, ServerContext context);
    }
}
