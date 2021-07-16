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
using System.Windows.Controls;
using System.Windows.Media;

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
            // var element = CustomDrawing.BuilderTempElement(style, name);
            var element = new TextBlock();
            element.Text = "ceshi";
            element.Foreground = Brushes.Red;
            element.FontSize = 17;
            element.Name = "est_text_002";
            element.HorizontalAlignment = HorizontalAlignment.Center;
            element.VerticalAlignment = VerticalAlignment.Center;
            var border = new Border();
            border.Margin = new Thickness(10);
            border.Width = 40;
            border.Height = 30;
            border.BorderBrush = Brushes.Red;
            border.Background = Brushes.Blue;
            border.BorderThickness =new Thickness(2);
            border.Name = "est_border_001";
            border.Child = element;

            custom.Point = point;
            custom.Element = border;
            custom.ValueTemplateStyle = style;
            return custom;
        }
    }
}
