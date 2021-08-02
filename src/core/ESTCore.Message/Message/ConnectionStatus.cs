/**********************************************************************
*******命名空间： ESTCore.Message.Message
*******类 名 称： ConnectionStatus
*******类 说 明： 
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 8/2/2021 11:51:41 AM
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

namespace ESTCore.Message.Message
{
    /// <summary>
    ///  
    /// </summary>
    public class ConnectionStatus:AbstractMessage
    {
        public override string Topic { get => "Status"; set => base.Topic = value; }
        public bool Success { get; set; }
    }
}
