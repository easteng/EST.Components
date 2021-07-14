/**********************************************************************
*******命名空间： ESTControl.SvgViewer
*******类 名 称： CustomDrawing
*******类 说 明： 自定义绘图类
*******作    者： Easten
*******机器名称： DESKTOP-EC8U0GP
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/1/2021 10:23:00 PM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @easten company 2021-2022. All rights reserved ★ *********
***********************************************************************
 */
using HandyControl.Controls;

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
    public class CustomDrawing
    {
        public static FrameworkElement BuilderTempElement(ValueTemplateStyle style,string name)
        {
            //  创建边框
            var border = new Border();
            border.BorderBrush = BrushConverter(style.BorderColor);
            border.BorderThickness = new System.Windows.Thickness(style.Thickeness);
            border.Height = style.Height;
            border.Width = style.Width;
            border.Background = BrushConverter(style.BackgroundColor);
            if (style.Padding == null)
            {
                style.Padding = new int[] { 10, 2 ,2,2};
            }
            border.Padding = new System.Windows.Thickness(style.Padding[0], style.Padding[1], style.Padding[2], style.Padding[3]);
            border.Name = name;
            // 创建字体

            var text = new TextBlock();
            text.Name = $"{name}_value";
            text.Foreground = BrushConverter(style.NormalColor);
            text.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            text.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            text.Text = "00.00";
            if (style.ShowBadge)
            {
                var badge = new Badge();
                badge.Status = HandyControl.Data.BadgeStatus.Processing;
                //badge.Style=StaticResource
                badge.Content = text;
                border.Child = badge;
            }
            else
            {
                border.Child = text;
            }

            return border;
        }
        private static Brush BrushConverter(string color)
        {
            if (color == null) return Brushes.White;
            var converter = new System.Windows.Media.BrushConverter();
            var brush = (Brush)converter.ConvertFromString(color);
            return brush;
        }
    }
}
