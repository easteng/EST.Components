/**********************************************************************
*******命名空间： ZYServiceCore
*******类 名 称： DBProvider
*******类 说 明： 数据处理类
*******作    者： 东腾 Easten
*******机器名称： MF5SK7EXLTJZGSI
*******CLR 版本： 4.0.30319.42000
*******创建时间： 2019-07-31 23:14:06
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
    public class DBProvider: IDBProvider
    {
        private ISqlLiteRepository _repository;
        public DBProvider(ISqlLiteRepository sqlLiteRepository)
        {
            _repository = sqlLiteRepository;
            //无参数构造
        }

        public T Get<T>(int id)
        {
            throw new NotImplementedException();
        }

        public T Get<T>(string key) where T:class,new()
        {
            return _repository.GetEntity<T>(key);
        }

        public List<T> GetAllList<T>(Expression<Func<T, bool>> func)
        {
           return _repository.GetAllList(func);
        }

        public List<T> GetAllList<T>()
        {
            return _repository.GetAllList<T>(a=>true);
        }

        public T Insert<T>(T model)
        {
            throw new NotImplementedException();
        }

        public void Updata<T>(T t) where T :class,new()
        {
            _repository.Update<T>(t);
        }
    }
}
