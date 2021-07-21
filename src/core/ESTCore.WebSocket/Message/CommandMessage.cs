/**********************************************************************
*******命名空间： ESTCore.WebSocket.Message
*******类 名 称： CommandMessage
*******类 说 明： 
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/21/2021 3:52:02 PM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @Easten 2020-2021. All rights reserved ★ *********
***********************************************************************
 */
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.WebSocket.Message
{
    /// <summary>
    ///  
    /// </summary>
    public class CommandMessage: AbstructMessage
    {
        public override string Topic { get; set; } = "command";
        public string Name { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

    }
}
