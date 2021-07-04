/**********************************************************************
*******命名空间： ESRCore.Platform.Utilties
*******类 名 称： ServiceLocator
*******类 说 明： 
*******作    者： Easten
*******机器名称： DESKTOP-EC8U0GP
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/3/2021 11:38:20 PM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @easten company 2021-2022. All rights reserved ★ *********
***********************************************************************
 */
using Autofac;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESRCore.Platform.Utilties
{
    public class ServiceLocator
    {
        public static IContainer Current { get; set;  }
        public static T GetServie<T>()
        {
            return Current.Resolve<T>();
        }
        public static bool IsRegistered<T>()
        {
            return Current.IsRegistered<T>();
        }
        public static bool IsRegistered<T>(string key)
        {
            return Current.IsRegisteredWithKey<T>(key);
        }
        public static bool IsRegistered(Type type)
        {
            return Current.IsRegistered(type);
        }

        public static bool IsRegistered(string key,Type type)
        {
            return Current.IsRegisteredWithKey(key,type);
        }

        public static T GetService<T>(string key)
        {
            return Current.ResolveKeyed<T>(key);
        }

        public static object GetService(Type type)
        {
            return Current.ResolveKeyed<object>(type);
        }

        public static object GetService(string key,Type type)
        {
            return Current.ResolveKeyed(key, type);
        }

    }
}
