/**********************************************************************
*******命名空间： ESTCore.Message.Client
*******类 名 称： ClientOption
*******类 说 明： 客户端配置项
*******作    者： Easten
*******机器名称： DESKTOP-EC8U0GP
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/23/2021 11:29:53 PM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @easten company 2021-2022. All rights reserved ★ *********
***********************************************************************
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.Message.Client
{
    /// <summary>
    /// 客户端配置项，包括定义接收机的名称及需要订阅的主题，如果主题为空，则将主题设置为接收机的名称
    /// </summary>
    public class ClientOption
    {
        public string Name { get; set; }
        public string Topic { get; set; }
    }
}
