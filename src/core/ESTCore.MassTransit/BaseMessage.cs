/**********************************************************************
*******命名空间： ESTCore.MassTransit
*******类 名 称： BaseMessage
*******类 说 明： 
*******作    者： Easten
*******机器名称： DESKTOP-EC8U0GP
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/10/2021 11:10:40 PM
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

namespace ESTCore.MassTransit
{
    public class BaseMessage:IBaseMessage1
    {
        public string Name { get; set; }
    }

    public interface IBaseMessage1
    {
        string Name { get; set; }
    }
}
