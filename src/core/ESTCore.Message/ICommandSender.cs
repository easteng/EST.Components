/**********************************************************************
*******命名空间： ESTCore.Message
*******接口名称： ICommandSender
*******接口说明： 命令发送者接口定义
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/16/2021 10:55:06 AM
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

namespace ESTCore.Message
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICommandSender
    {
        Task Send<TMessage>(TMessage message) where TMessage : IBaseMessage;
    }
}
