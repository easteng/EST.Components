/**********************************************************************
*******命名空间： CachingTest
*******类 名 称： TestService
*******类 说 明： 
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/13/2021 10:58:20 AM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @Easten 2020-2021. All rights reserved ★ *********
***********************************************************************
 */
using EasyCaching.Core;

using ESTCore.Caching;

using Microsoft.Extensions.Hosting;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CachingTest
{
    /// <summary>
    ///  
    /// </summary>
    public class TestService : IHostedService
    {
        IRedisCachingProvider caching;
        public TestService(IRedisCachingProvider chching = null)
        {
            this.caching = chching;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            

            var item = new CacheItem()
            {
                Name = "水水水水",
                Value = 123123
            };
            //caching.LSet("test", 1, item);

            var list = new List<CacheItem>();
            list.Add(new CacheItem { Name = "11", Value = 11 });
            list.Add(new CacheItem { Name = "22", Value = 22 });
            list.Add(new CacheItem { Value = 33 });
            list.Add(new CacheItem { Name = "44" });

            caching.KeyDel("test:data");

            caching.StringSet("test:data:config",JsonConvert.SerializeObject(list));
            caching.StringSet("test:data:monitor",JsonConvert.SerializeObject(list));
            caching.StringSet("test:data:sensor",JsonConvert.SerializeObject(list));

            var data = caching.StringGet("test2");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
           // throw new NotImplementedException();
        }
    }
    [Serializable]
    public class CacheItem
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }
}
