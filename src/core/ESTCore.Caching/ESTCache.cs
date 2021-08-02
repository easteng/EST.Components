/**********************************************************************
*******命名空间： ESTCore.Caching
*******类 名 称： ESTCache
*******类 说 明： 
*******作    者： Easten
*******机器名称： DESKTOP-EC8U0GP
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/30/2021 11:35:45 PM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @easten company 2021-2022. All rights reserved ★ *********
***********************************************************************
 */
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.Caching
{
    public static class ESTCache
    {
        public static List<T> GetList<T>(string caching)
        {
            if (string.IsNullOrEmpty(caching)) return null;
            return JsonConvert.DeserializeObject<List<T>>(caching, GetDefaultSettings());
        }

        public static T Get<T>(string caching)
        {
            if (string.IsNullOrEmpty(caching)) return default(T);
            return JsonConvert.DeserializeObject<T>(caching, GetDefaultSettings());  
        }

        public static string GetCacheString<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, GetDefaultSettings());
        }
        public static JsonSerializerSettings GetDefaultSettings()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
            };
          //  settings.Converters.Add(new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.RoundtripKind });
            return settings;
        }
    }
}
