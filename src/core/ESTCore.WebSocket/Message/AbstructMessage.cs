﻿/**********************************************************************
*******命名空间： ESTCore.WebSocket.Message
*******类 名 称： AbstructMessage
*******类 说 明： 
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/21/2021 3:51:16 PM
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

namespace ESTCore.WebSocket.Message
{
    /// <summary>
    ///  抽象消息
    /// </summary>
    public abstract class AbstructMessage
    {
        public abstract string Topic { get; set; }
    }
}
