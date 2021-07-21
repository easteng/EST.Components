/**********************************************************************
*******命名空间： WebSocketTest1
*******类 名 称： CommandHandler
*******类 说 明： 
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/21/2021 3:26:37 PM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @Easten 2020-2021. All rights reserved ★ *********
***********************************************************************
 */
using ESTCore.WebSocket;
using ESTCore.WebSocket.Message;

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
    public class CommandHandler : IWSCommandProvider
    {
        public Task Execute(CommandMessage t)
        {
            Console.WriteLine(t.Name);
            return Task.CompletedTask;
        }
    }
}
