/**********************************************************************
*******命名空间： ESTCore.WebSocket.Config
*******接口名称： MessageTopic
*******接口说明： 设置消息主题
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/21/2021 3:04:44 PM
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

namespace ESTCore.WebSocket.Config
{
    /// <summary>
    /// 
    /// </summary>
    public class MessageTopic
    {
        /// <summary>
        /// 数据生产之，一般指采集服务创建得客户端
        /// </summary>
        public static readonly string DataProducer = "DataProducer";
        /// <summary>
        /// 数据消费者，一般指客户端  服务端定向推送给客户端
        /// </summary>
        public static readonly string DataConsumer = "Consumer";
    }
}
