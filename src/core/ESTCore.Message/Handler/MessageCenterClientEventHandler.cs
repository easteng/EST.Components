/**********************************************************************
*******命名空间： ESTCore.Message.Handler
*******类 名 称： MessageCenterClientEventHandler
*******类 说 明： 消息中心客户端事件帮助类
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/23/2021 11:01:09 AM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @Easten 2020-2021. All rights reserved ★ *********
***********************************************************************
 */
using ESTCore.Common.WebSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.Message.Handler
{
    /// <summary>
    ///  消息中心客户端事件帮助类
    /// </summary>
    public class MessageCenterClientEventHandler
    {
        public static void OnClientApplicationMessageReceive(WebSocketMessage message)
        {

        }

        public static void OnNetworkError(object sender, EventArgs e)
        {

        }
        public static void OnClientConnected()
        {

        }
    }
}
