/**********************************************************************
*******命名空间： ZYServiceCore.Model
*******类 名 称： ServiceModel
*******类 说 明： 服务信息实体
*******作    者： 东腾 Easten
*******机器名称： MF5SK7EXLTJZGSI
*******CLR 版本： 4.0.30319.42000
*******创建时间： 2019-07-31 17:38:45
***********************************************************************
******* ★ Copyright @Easten 2019-2020. All rights reserved ★ *********
***********************************************************************
 */
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZYServiceCore
{
    /// <summary>
    /// 
    /// </summary>
    [SugarTable("zy_service")]
    public class ServiceModel
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)] //是主键, 还是标识列
        public int Id { get; set; }
        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// 服务描述
        /// </summary>
        public string ServiceDesc { get; set; }

        /// <summary>
        /// 是否安装
        /// </summary>
        public bool IsInstall { get; set; }

        /// <summary>
        /// 执行文件所在目录
        /// </summary>
        public string BinPath { get; set; }

        /// <summary>
        /// 服务启动状态，自动，手动，禁用
        /// </summary>
        public string ServiceType { get; set; }

        /// <summary>
        /// 服务状态，表示该服务时候可用
        /// </summary>
        public int ServiceState { get; set; }

        /// <summary>
        /// 服务运行状态，正在运行、未安装、已停止
        /// </summary>
        public string RuningState { get; set; }

    }
}
