/**********************************************************************
*******命名空间： ESTCore.Domain.Entity
*******类 名 称： AuditEntity
*******类 说 明： 
*******作    者： Easten
*******机器名称： DESKTOP-EC8U0GP
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/3/2021 10:24:15 PM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @easten company 2021-2022. All rights reserved ★ *********
***********************************************************************
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.Domain.Entity
{
    /// <summary>
    /// 审计实体基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AuditEntity<T> : BaseEntity<T>
    {
        public DateTime CreatorTime { get; set; }
        public DateTime EditorTime { get; set; }
    }
}
