/**********************************************************************
*******命名空间： ZYServiceCore
*******类 名 称： Easten
*******类 说 明： 服务管理类
*******作    者： 东腾 Easten
*******机器名称： MF5SK7EXLTJZGSI
*******CLR 版本： 4.0.30319.42000
*******创建时间： 2019-07-31 18:02:40
***********************************************************************
******* ★ Copyright @Easten 2019-2020. All rights reserved ★ *********
***********************************************************************
 */
using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZYServiceCore.Model;

namespace ZYServiceCore
{
    public class Easten
    {
        private static IContainer container;
        /// <summary>
        /// 数据引擎初始化
        /// </summary>
        /// <returns></returns>
        public static bool Initialization()
        {
            var build = new ContainerBuilder();
            build.RegisterType<SqliteRepository>().As<ISqlLiteRepository>();
            build.RegisterType<DBProvider>().As<IDBProvider>();
            
            container = build.Build();
            container.BeginLifetimeScope();
            //初始化容器
            return true;
        }

        public static T Resolve<T>()
        {
            return container.Resolve<T>();
        }
    }
}
