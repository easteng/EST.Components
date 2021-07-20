/**********************************************************************
*******命名空间： ESTCore.Message
*******类 名 称： ServiceStatusMessage
*******类 说 明： 服务状态消息体
*******作    者： Easten
*******机器名称： DESKTOP-EC8U0GP
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/16/2021 9:39:31 PM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @easten company 2021-2022. All rights reserved ★ *********
***********************************************************************
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.Message
{
    /// <summary>
    /// 服务状态消息体
    /// </summary>
    public class ServiceStatusMessage:AbstractMessage 
    {
        public override string Topic { get => "ServiceState"; set => base.Topic = value; }
        public ServiceType ServiceType { get; set; }
        public ServiceStatus Status { get; set; }
        public DateTime Time { get; set; }
    }
    /// <summary>
    /// 服务状态枚举
    /// </summary>
    public enum ServiceStatus
    {
        [Display(Name = "服务启动")]
        [Description("服务启动")]
        Start,
        [Display(Name = "服务停止")]
        [Description("服务停止")]
        Stop,
        [Display(Name = "服务异常")]
        [Description("服务异常")]
        Exception,
        [Display(Name = "内部错误")]
        [Description("内部错误")]
        Error,
        [Display(Name = "服务运行中")]
        [Description("服务运行中")]
        Runting
    }

    public enum ServiceType
    {
        [Display(Name = "数据存储服务")]
        [Description("数据存储服务")]
        /// <summary>
        /// 储存服务
        /// </summary>
        Storage,
        [Display(Name = "短信发送服务")]
        [Description("短信发送服务")]
        /// <summary>
        /// 短信服务
        /// </summary>
        Sms,
        [Display(Name = "WTR20A协议采集服务")]
        [Description("WTR20A协议采集服务")]
        /// <summary>
        /// 数据采集服务
        /// </summary>
        CollectionWTR20A,
        [Display(Name = "WTR31协议采集服务")]
        [Description("WTR31协议采集服务")]
        /// <summary>
        /// 数据采集服务
        /// </summary>
        CollectionWTR31

    }
}
