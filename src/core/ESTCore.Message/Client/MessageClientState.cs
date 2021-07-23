/**********************************************************************
*******命名空间： ESTCore.Message.Client
*******类 名 称： MessageClientState
*******类 说 明： 消息客户端状态，
*******作    者： Easten
*******机器名称： DESKTOP-EC8U0GP
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/24/2021 12:04:17 AM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @easten company 2021-2022. All rights reserved ★ *********
***********************************************************************
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.Message.Client
{
    /// <summary>
    /// 消息客户端装，为静态类，用来记录当前客户端是否成功的连接服务
    /// </summary>
    public abstract class MessageClientState
    {
        /// <summary>
        /// 是否连接正常
        /// </summary>
        public static bool IsSuccess { get; set; } = false;
    }
}
