/**********************************************************************
*******命名空间： ESTControl.SvgViewer
*******类 名 称： ZoomPanMouseHandlingMode
*******类 说 明： 鼠标拖动帮助类
*******作    者： Easten
*******机器名称： DESKTOP-EC8U0GP
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/1/2021 9:31:43 PM
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

namespace ESTControl.SvgViewer
{
    public enum ZoomPanMouseHandlingMode
    {
        None,
        Panning,
        Zooming,
        DragZooming,
        SelectPoint,
        SelectRectangle
    }
}