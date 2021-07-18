/**********************************************************************
*******命名空间： ZYServiceCore
*******类 名 称： SqlLiteRepository
*******类 说 明： 
*******作    者： 东腾 Easten
*******机器名称： MF5SK7EXLTJZGSI
*******CLR 版本： 4.0.30319.42000
*******创建时间： 2019-07-31 23:21:37
***********************************************************************
******* ★ Copyright @Easten 2019-2020. All rights reserved ★ *********
***********************************************************************
 */


using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ZYServiceCore.Model;

namespace ZYServiceCore
{
    public class SqliteRepository : ISqlLiteRepository
    {
        private SqlSugarClient sqlClient;
        public SqliteRepository()
        {
            var dir = new DirectoryInfo(string.Format(@"{0}\\", Environment.CurrentDirectory));
            var path = dir.FullName;
            sqlClient = new SqlSugarClient(new ConnectionConfig() {
                
                ConnectionString= $"Data Source ={path}services.db",
                DbType=DbType.Sqlite,
                IsAutoCloseConnection = true,//自动释放数据务，如果存在事务，在事务结束后释放
                InitKeyType = InitKeyType.Attribute
            });
        }
        public List<T> GetAllList<T>(Expression<Func<T, bool>> predicate)
        {
            var list = sqlClient.Queryable<T>().Where(predicate).ToList();
            return list;
        }

        public T GetEntity<T>(string key) where T : class, new()
        {
            var entity = sqlClient.Queryable<T>().Where($"Key='{key}'").First();
            return entity;
        }

        public void Insert<T>(T t) where T : class, new()
        {
            sqlClient.Insertable(t).ExecuteCommand();
        }

        public bool Update<T>(T t) where T : class, new()
        {
            sqlClient.Updateable(t).ExecuteCommand();
            return true;
        }
    }
}
