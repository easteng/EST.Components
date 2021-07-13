/**********************************************************************
*******命名空间： ESTCore.Message
*******类 名 称： CommandMessage
*******类 说 明： 命令消息  用于端点通信
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/13/2021 2:05:34 PM
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
    ///  命令消息体  用于端点通信
    /// </summary>
    public class CommandMessage: AbstractMessage
    {
        public override string Topic { get => "command"; set => base.Topic = value; }
        /// <summary>
        /// 是否开启远程监测
        /// </summary>
        public bool RemoteMonitor { get; set; }
    }
}
