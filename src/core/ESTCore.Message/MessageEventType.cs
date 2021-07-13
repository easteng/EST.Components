/**********************************************************************
*******命名空间： ESTCore.Message
*******类 名 称： MessageEventType
*******类 说 明： 
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/13/2021 2:23:14 PM
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
    ///  消息时间类型
    /// </summary>
    public enum MessageEventType
    {
        /// <summary>
        /// 异常
        /// </summary>
        Exception,
        /// <summary>
        /// 心跳监测
        /// </summary>
        Heartbeat,
    }
}
