/**********************************************************************
*******命名空间： ESTCore.ORM.FreeSql
*******类 名 称： FreeSqlModule
*******类 说 明： 数据库模块
*******作    者： Easten
*******机器名称： DESKTOP-EC8U0GP
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/4/2021 9:46:58 AM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @easten company 2021-2022. All rights reserved ★ *********
***********************************************************************
 */

using FreeSql;
using Microsoft.Extensions.DependencyInjection;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Autofac;
using Silky.Lms.Core.Modularity;
using System.Threading.Tasks;
using Silky.Lms.Core;
using System;
using Silky.Lms.AutoMapper;

namespace ESTCore.ORM.FreeSql
{
    public class FreeSqlModule : LmsModule
    {
        public override Task Initialize(ApplicationContext applicationContext)
        {
            return base.Initialize(applicationContext);
        }

        protected override void RegisterServices(ContainerBuilder builder)
        {
            var service = new ServiceCollection();
            var config=EngineContext.Current.Resolve<IConfiguration>();
            var dataType = config["DataBase:Type"];
            var conn = config["DataBase:ConnectionString"];
            var freeSql = new FreeSqlBuilder()
              .UseConnectionString(GetType(dataType), conn)
              .UseAutoSyncStructure(true)
              .Build();
            service.AddSingleton<IFreeSql>(freeSql);
            service.AddFreeRepository(null, this.GetType().Assembly);
            builder.Populate(service);
        }

        private DataType GetType(string type)
        {
            if (type == "pg")
                return DataType.PostgreSQL;
            else if (type == "mssql")
                return DataType.SqlServer;
            else return DataType.Sqlite;
        }
    }
}
