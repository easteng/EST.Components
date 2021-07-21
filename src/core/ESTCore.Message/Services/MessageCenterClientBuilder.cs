/**********************************************************************
*******命名空间： ESTCore.Message.Services
*******类 名 称： MessageCenterClientBuilder
*******类 说 明： 
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/21/2021 5:22:30 PM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @Easten 2020-2021. All rights reserved ★ *********
***********************************************************************
 */
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.Message.Services
{
    /// <summary>
    ///  消息中心客户端构建
    /// </summary>
    public class MessageCenterClientBuilder
    {
        private IServiceCollection services { get; }
        public MessageCenterClientBuilder(IServiceCollection serviceDescriptors = null)
        {
            this.services = serviceDescriptors;
        }
    }
}
