/**********************************************************************
*******命名空间： ESTTool.Excel
*******类 名 称： ExcelFileType
*******类 说 明： 
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/26/2021 5:52:09 PM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @Easten 2020-2021. All rights reserved ★ *********
***********************************************************************
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTTool.Excel
{
    /// <summary>
    ///  excel 格式类型
    /// </summary>
    public enum ExcelFileType
    {
        /// <summary>
        /// 2007以上版本
        /// </summary>
        Xlsx,
        /// <summary>
        /// 2003以前版本
        /// </summary>
        Xls
    }
}
