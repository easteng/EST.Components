/**********************************************************************
*******命名空间： ESTControl.SvgViewer
*******类 名 称： CustomUIElement
*******类 说 明： 
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/14/2021 5:39:27 PM
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
using System.Windows;

namespace ESTControl.SvgViewer
{
    /// <summary>
    ///  自定义的点数据，用于创建温度标记
    /// </summary>
    public class CustomUIElement
    {
        
        private CustomUIElement() { }
        public FrameworkElement Element { get; set; }
        /// <summary>
        ///  当前的样式
        /// </summary>
        public ValueTemplateStyle ValueTemplateStyle { get; set; }
        public Point Point { get; set; }

        /// <summary>
        /// 根据组件name 样式值创建的及点坐标创建绘图
        /// </summary>
        /// <param name="style"></param>
        /// <param name="name"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static CustomUIElement CreateCustomUIElement(ValueTemplateStyle style,string name,Point point)
        {
            var custom = new CustomUIElement();
            var element = CustomDrawing.BuilderTempElement(style, name);
            custom.Point = point;
            custom.Element = element;
            custom.ValueTemplateStyle = style;
            return custom;
        }
    }
}
