/**********************************************************************
*******命名空间： ZYServiceCore
*******接口名称： IDBProvider
*******接口说明： 数据处理接口
*******作    者： 东腾 Easten
*******机器名称： MF5SK7EXLTJZGSI
*******CLR 版本： 4.0.30319.42000
*******创建时间： 2019-07-31 23:09:08
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
using ZYServiceCore.Model;

namespace ZYServiceCore
{
    public interface IT<T>
    {

    }
    public interface IDBProvider
    {
        T Insert<T>(T model);

        List<T> GetAllList<T>(Expression<Func<T,bool>> func);

        List<T> GetAllList<T>();

        void Updata<T>(T t) where T : class, new();

        T Get<T>(int id);
        T Get<T>(string key) where T : class, new();
    }
}
