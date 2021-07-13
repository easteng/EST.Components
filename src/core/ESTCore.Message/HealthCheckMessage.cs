/**********************************************************************
*******命名空间： ESTCore.Message
*******类 名 称： HealthCheckMessage
*******类 说 明： 健康检查
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/13/2021 2:17:07 PM
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
    ///  健康检查
    /// </summary>
    public class HealthCheckMessage:AbstractMessage
    {
        public override string Topic { get => "HealthCheck"; set => base.Topic = value; }
        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServerName { get; set; }
        /// <summary>
        /// 是否在线
        /// </summary>
        public bool Online { get; set; }
        /// <summary>
        /// 数据是否采集中
        /// </summary>
        public bool Operating { get; set; }
        /// <summary>
        /// 消息事件类型
        /// </summary>
        public MessageEventType EventType { get; set; }
    }
}
