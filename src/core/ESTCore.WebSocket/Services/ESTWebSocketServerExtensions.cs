/**********************************************************************
*******命名空间： ESTCore.WebSocket.Services
*******类 名 称： ESTWebSocketServerExtensions
*******类 说 明： 服务端扩展方法
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/21/2021 10:53:18 AM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @Easten 2020-2021. All rights reserved ★ *********
***********************************************************************
 */
using ESTCore.Common.WebSocket;
using ESTCore.WebSocket.Config;
using ESTCore.WebSocket.Services;

using Microsoft.Extensions.DependencyInjection;

using Silky.Lms.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.WebSocket
{
    /// <summary>
    ///  服务端扩展方法
    /// </summary>
    public static class ESTWebSocketServerExtensions
    {

        public static void AddESTWebSocket(this IServiceCollection serviceCollection,
           Action<ESTWebSocketServerBuilder> builder)
        {
            var messageCore = new ESTWebSocketServerBuilder(serviceCollection);
            builder.Invoke(messageCore);
        }
    }
}
