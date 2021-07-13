/**********************************************************************
*******命名空间： ESTCore.Message
*******类 名 称： StandardMessage
*******类 说 明： 标准数据消息体
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/13/2021 2:09:21 PM
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
    ///  标准数据消息体
    /// </summary>
    public class StandardMessage<T>:AbstractMessage<T> where T:class
    {
        public override string Topic { get => "Standard"; set => base.Topic = value; }
    }
}
