/**********************************************************************
*******命名空间： ESTCore.Message
*******类 名 称： CheckMessageStatus
*******类 说 明： 检查消息状态  是否被消费 没有消费，则不发送数据
*******作    者： Easten
*******机器名称： DESKTOP-EC8U0GP
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/17/2021 9:11:39 AM
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

namespace ESTCore.Message
{
    public interface CheckMessageStatus
    {
        bool Ok { get;set;  }
    }
}
