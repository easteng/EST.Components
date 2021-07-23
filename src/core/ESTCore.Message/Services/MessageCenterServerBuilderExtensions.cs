/**********************************************************************
*******命名空间： ESTCore.Message.Services
*******类 名 称： MessageCenterServerBuilderExtensions
*******类 说 明： 服务创建帮助类
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/21/2021 5:58:07 PM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @Easten 2020-2021. All rights reserved ★ *********
***********************************************************************
 */
using ESTCore.Message.Handler;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.Message.Services
{
    /// <summary>
    ///  服务创建帮助类
    /// </summary>
    public static class MessageCenterServerBuilderExtensions
    {
        /// <summary>
        /// 构建实例
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static MessageCenterServerBuilder Build(this MessageCenterServerBuilder builder)
        {
            builder.Build();
            return builder;
        }
    }
}
