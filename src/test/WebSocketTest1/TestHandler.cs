/**********************************************************************
*******命名空间： WebSocketTest1
*******类 名 称： TestHandler
*******类 说 明： 
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/21/2021 2:26:51 PM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @Easten 2020-2021. All rights reserved ★ *********
***********************************************************************
 */
using ESTCore.WebSocket.Handlers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketTest1
{
    /// <summary>
    ///  
    /// </summary>
    public class TestHandler : IWSServerMessageBase
    {
        public Task Receive(string data)
        {
            // 接收
            Console.WriteLine("接收到客户端数据");

            return Task.CompletedTask;
        }
    }
}
