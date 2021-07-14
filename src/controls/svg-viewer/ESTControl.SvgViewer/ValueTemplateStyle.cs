/**********************************************************************
*******命名空间： ESTControl.SvgViewer
*******类 名 称： ValueTemplateStyle
*******类 说 明： 
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/14/2021 4:23:18 PM
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

namespace ESTControl.SvgViewer
{
    /// <summary>
    ///  温度自定义样式
    /// </summary>
    public class ValueTemplateStyle
    {
        /// <summary>
        /// 是否显示温度边框
        /// </summary>
        public bool ShowBorder { get; set; }
        /// <summary>
        /// 背景颜色
        /// </summary>
        public string BackgroundColor { get; set; }

        /// <summary>
        /// 温度边框圆角
        /// </summary>
        public int Radius { get; set; }
        /// <summary>
        /// 边线宽度
        /// </summary>
        public int Thickeness { get; set; }
        /// <summary>
        /// 边线颜色
        /// </summary>
        public string BorderColor { get; set; }
        /// <summary>
        /// 温度框宽度
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// 温度框高度
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// 内边距
        /// </summary>
        public int[] Padding { get; set; }

        /// <summary>
        /// 温度值的大小
        /// </summary>
        public int ValueSize { get; set; }
        /// <summary>
        /// 正常值字体颜色
        /// </summary>
        public string NormalColor { get; set; }
        /// <summary>
        /// 预警值颜色
        /// </summary>
        public string WaringColor { get; set; }

        /// <summary>
        /// 报警时的颜色
        /// </summary>
        public string AlertColor { get; set; }
        /// <summary>
        /// 是否显示报警小图标
        /// </summary>
        public bool ShowBadge { get; set; }
    }
}
