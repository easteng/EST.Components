/**********************************************************************
*******命名空间： ESTCore.Message.Handler
*******接口名称： IMessageReceiverHandler
*******接口说明： 消息接收机帮助类
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/23/2021 10:51:09 AM
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
    /// 消息接收机帮助接口，主要用于客户端使用，用来接收服务端发送来的消息以及消息命令使用
    /// </summary>
    public interface IMessageReceiverHandler
    {
        abstract Task Receive(BaseMessage message);
    }
}
