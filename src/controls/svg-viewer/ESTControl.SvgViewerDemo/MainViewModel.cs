/**********************************************************************
*******命名空间： ESTControl.SvgViewerDemo
*******类 名 称： MainViewModel
*******类 说 明： 
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/14/2021 3:20:56 PM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @Easten 2020-2021. All rights reserved ★ *********
***********************************************************************
 */
using ESTControl.SvgViewer;

using SharpVectors.Dom;
using SharpVectors.Dom.Svg;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTControl.SvgViewerDemo
{
    /// <summary>
    ///  
    /// </summary>
    public class MainViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private SvgViewModel viewerModel;

        public SvgViewModel ViewerModel
        {
            get { return viewerModel; }
            set {
                viewerModel = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SvgViewModel"));
                }
            }
        }

        internal void SetPoint(System.Windows.Point e)
        {
            this.PointStr = $"x:{e.X},y:{e.Y}";
        }

        // 选中的坐标值
        private string pointstr;    

        public string PointStr
        {
            get { return pointstr; }
            set { pointstr = value; if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("PointStr"));
                }
            }
        }

    }
}
