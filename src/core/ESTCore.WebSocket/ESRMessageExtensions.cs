/**********************************************************************
*******命名空间： ESTCore.WebSocket
*******类 名 称： ESRMessageExtensions
*******类 说 明： 扩展帮助类
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/21/2021 2:58:17 PM
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

namespace ESTCore.WebSocket
{
    /// <summary>
    ///  
    /// </summary>
    public static class ESRMessageExtensions
    {
        public static T GetMessage<T>(this byte[] data)
        {
            try
            {
                var str = Encoding.Default.GetString(data);
                return JsonConvert.DeserializeObject<T>(str);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("参数有问题，请检查");
            }
        }
    }
}
