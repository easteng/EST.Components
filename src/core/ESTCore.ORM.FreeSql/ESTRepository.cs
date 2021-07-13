/**********************************************************************
*******命名空间： ESTCore.ORM.FreeSql
*******类 名 称： ESTRepository
*******类 说 明： 
*******作    者： Easten
*******机器名称： DESKTOP-EC8U0GP
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/11/2021 5:47:00 PM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @easten company 2021-2022. All rights reserved ★ *********
***********************************************************************
 */
using FreeSql;

using Silky.Lms.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.ORM.FreeSql
{
    public class ESTRepository
    {
        public static IBaseRepository<T,Tkey> Builder<T, Tkey>() where T : class
        {
            var free = EngineContext.Current.Resolve<IFreeSql>();
            return free.GetRepository<T, Tkey>();
        }
    }
}
