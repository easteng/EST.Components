/**********************************************************************
*******命名空间： WebSocketClientTest
*******类 名 称： TestMessage
*******类 说 明： 
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/21/2021 3:24:59 PM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @Easten 2020-2021. All rights reserved ★ *********
***********************************************************************
 */
using ESTCore.WebSocket;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.WebSocket
{
    public class TestMessage : IESTBaseCommand
    {
        public string Name { get; set; }
        public string Topic { get; set; } = "sss";

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
