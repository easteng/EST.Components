/**********************************************************************
*******命名空间： ESTCore.Message.Handler
*******类 名 称： MessageRepeaterHandler
*******类 说 明： 消息接收机内容
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/21/2021 5:32:57 PM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @Easten 2020-2021. All rights reserved ★ *********
***********************************************************************
 */
using ESTCore.Message.Message;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.Message.Handler
{
    /// <summary>
    /// 消息转换器帮助类，主要用于服务端使用，用来接收处理消息，并做转发使用的
    /// </summary>
    public interface IMessageRepeaterHandler
    {
        abstract Task Repeater(BaseMessage message);
    }
}
