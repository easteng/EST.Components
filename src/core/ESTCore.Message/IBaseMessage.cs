/**********************************************************************
*******命名空间： ESTCore.Message
*******接口名称： IBaseMessage
*******接口说明： 基础接口
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/13/2021 2:00:53 PM
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

namespace ESTCore.Message
{
    /// <summary>
    /// 基础消息接口
    /// </summary>
    public interface IBaseMessage
    {
        /// <summary>
        /// 消息主题
        /// </summary>
        string Topic { get; set; }
    }
}
