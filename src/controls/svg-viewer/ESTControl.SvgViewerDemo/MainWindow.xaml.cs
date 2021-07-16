using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Forms;
using ESTControl.SvgViewer;

namespace ESTControl.SvgViewerDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xamls
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel mainViewModel;
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = mainViewModel = new MainViewModel();
            this.svgContainer.ElementSelect += SvgContainer_ElementSelect;
            this.svgContainer.PointSelectedEvent += SvgContainer_PointSelectedEvent;
            this.svgContainer.ElementUpdate += SvgContainer_ElementUpdate;
        }

        /// <summary>
        /// 拖动改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SvgContainer_ElementUpdate(object sender, FrameworkElement e)
        {
            var x =(double) e.GetValue(Canvas.LeftProperty);
            var y =(double) e.GetValue(Canvas.TopProperty);
            var point = new Point(x, y);
            this.mainViewModel.SetPoint(point);
        }

        private void SvgContainer_PointSelectedEvent(object sender, Point e)
        {
            var style = new ValueTemplateStyle();
            style.NormalColor = "#4DA2FD";
            style.ValueSize = 18;
            style.ShowBadge = true;
            var ele = CustomUIElement.CreateCustomUIElement(style, $"name_{DateTime.Now.ToString("YYYYmmddHHmmss")}", e);
            this.mainViewModel.SetPoint(e);
            this.svgContainer.AddUIElement(ele);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // 打开图片
            OpenFileDialog open = new OpenFileDialog();
            open.ShowDialog();

            var file = open.FileName;
            _ = this.svgContainer.LoadDocument(file);
            
        }

        private void SvgContainer_ElementSelect(object sender, SharpVectors.Dom.Svg.SvgElement e)
        {
           
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // 开启编辑
            this.svgContainer.ViewerModel = SvgViewer.SvgViewModel.Edit;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            // 关闭编辑
            this.svgContainer.ViewerModel = SvgViewer.SvgViewModel.View;
        }
    }
}
