/**********************************************************************
*******命名空间： ESTCore.Message
*******类 名 称： MessageCenter
*******类 说 明： 消息中心
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/16/2021 10:42:58 AM
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
    ///  消息中心
    /// </summary>
    public class MessageCenter
    {
        /// <summary>
        /// 初始化健康检查消息体
        /// </summary>
        /// <param name="name"></param>
        public static void InitHealtMessage(string name)
        {
            HealthCheckMessage = new HealthCheckMessage(name);
        }
        public static HealthCheckMessage HealthCheckMessage { get; set; }
    }
}
