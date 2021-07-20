/**********************************************************************
*******命名空间： ZYServiceCore.Model
*******类 名 称： ManageModel
*******类 说 明： 系统配置实体
*******作    者： 东腾 Easten
*******机器名称： MF5SK7EXLTJZGSI
*******CLR 版本： 4.0.30319.42000
*******创建时间： 2019-08-01 15:15:24
***********************************************************************
******* ★ Copyright @Easten 2019-2020. All rights reserved ★ *********
***********************************************************************
 */
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZYServiceCore.Model
{
    [SugarTable("zy_manage")]
    public class ManageModel
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)] //是主键, 还是标识列
        public int Id { get; set; }
        public string Key { get; set; }

        public string Value { get; set; }
    }
}
