/**********************************************************************
*******命名空间： ESTCore.Test.Console
*******类 名 称： TestService
*******类 说 明： 
*******作    者： Easten
*******机器名称： DESKTOP-EC8U0GP
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/5/2021 8:17:51 PM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @easten company 2021-2022. All rights reserved ★ *********
***********************************************************************
 */
using Autofac;

using FreeSql;

using Microsoft.Extensions.Hosting;

using Silky.Lms.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESTCore.Test.Console
{
    public class TestService : IHostedService
    {
        readonly IBaseRepository<User,Guid> baseRepository;
        public TestService(IBaseRepository<User, Guid> baseRepository = null)
        {
            this.baseRepository = baseRepository;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            //var ser = EngineContext.Current.Resolve<IFreeSql>();
            //var user=ser.GetRepository<User>();
            var list = baseRepository.Where(a => true).ToList();

            //throw new NotImplementedException();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
            // throw new NotImplementedException();
        }
    }
}
