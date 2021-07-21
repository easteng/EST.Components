/**********************************************************************
*******命名空间： ESTCore.WebSocket
*******接口名称： IESTBaseCommand
*******接口说明： 
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/21/2021 3:18:59 PM
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

namespace ESTCore.WebSocket
{
    /// <summary>
    /// 
    /// </summary>
    public interface IESTBaseCommand<T>
    {
        
    }

    public interface IESTBaseCommand
    {
        string Topic { get; set; }
    }
}
