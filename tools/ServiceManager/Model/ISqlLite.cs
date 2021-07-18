/**********************************************************************
*******命名空间： ZYServiceCore
*******接口名称： ISqlLite
*******接口说明： sql lite 数据库实现
*******作    者： 东腾 Easten
*******机器名称： MF5SK7EXLTJZGSI
*******CLR 版本： 4.0.30319.42000
*******创建时间： 2019-07-31 23:17:50
***********************************************************************
******* ★ Copyright @Easten 2019-2020. All rights reserved ★ *********
***********************************************************************
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ZYServiceCore
{
    public interface ISqlLiteRepository
    {
        void Insert<T>(T t) where T : class, new();

        List<T> GetAllList<T>(Expression<Func<T, bool>> predicate);

        bool Update<T>(T t) where T : class, new();

        T GetEntity<T>(string key) where T : class, new();

    }
}
