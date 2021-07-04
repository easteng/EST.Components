// Decompiled with JetBrains decompiler
// Type: EstCommunication.BasicFramework.SoftPainting
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;

namespace EstCommunication.BasicFramework
{
  /// <summary>静态类，包含了几个常用的画图方法，获取字符串，绘制小三角等</summary>
  public static class SoftPainting
  {
    /// <summary>获取一个直方图</summary>
    /// <param name="array">数据数组</param>
    /// <param name="width">宽度</param>
    /// <param name="height">高度</param>
    /// <param name="degree">刻度划分等级</param>
    /// <param name="lineColor">线条颜色</param>
    /// <returns></returns>
    public static Bitmap GetGraphicFromArray(
      int[] array,
      int width,
      int height,
      int degree,
      Color lineColor)
    {
      if (width < 10 && height < 10)
        throw new ArgumentException("长宽不能小于等于10");
      int max = ((IEnumerable<int>) array).Max();
      int min = 0;
      int length = array.Length;
      StringFormat sf = new StringFormat();
      sf.Alignment = StringAlignment.Far;
      Pen penDash = new Pen(Color.LightGray, 1f);
      penDash.DashStyle = DashStyle.Custom;
      penDash.DashPattern = new float[2]{ 5f, 5f };
      Font font = new Font("宋体", 9f);
      Bitmap bitmap = new Bitmap(width, height);
      Graphics g = Graphics.FromImage((Image) bitmap);
      g.SmoothingMode = SmoothingMode.AntiAlias;
      g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
      g.Clear(Color.White);
      int left = 60;
      int right = 8;
      int num1 = 8;
      int down = 8;
      int num2 = width - left - right;
      int height1 = height - num1 - down;
      Rectangle rectangle = new Rectangle(left - 1, num1 - 1, num2 + 1, height1 + 1);
      g.DrawLine(Pens.Gray, left - 1, num1, left + num2 + 1, num1);
      g.DrawLine(Pens.Gray, left - 1, num1 + height1 + 1, left + num2 + 1, num1 + height1 + 1);
      g.DrawLine(Pens.Gray, left - 1, num1 - 1, left - 1, num1 + height1 + 1);
      SoftPainting.PaintCoordinateDivide(g, Pens.DimGray, penDash, font, Brushes.DimGray, sf, degree, max, min, width, height, left, right, num1, down);
      PointF[] points = new PointF[array.Length];
      for (int index = 0; index < array.Length; ++index)
      {
        points[index].X = (float) num2 * 1f / (float) (array.Length - 1) * (float) index + (float) left;
        points[index].Y = (float) ((double) SoftPainting.ComputePaintLocationY(max, min, height1, array[index]) + (double) num1 + 1.0);
      }
      Pen pen = new Pen(lineColor);
      g.DrawLines(pen, points);
      pen.Dispose();
      penDash.Dispose();
      font.Dispose();
      sf.Dispose();
      g.Dispose();
      return bitmap;
    }

    /// <summary>计算绘图时的相对偏移值</summary>
    /// <param name="max">0-100分的最大值，就是指准备绘制的最大值</param>
    /// <param name="min">0-100分的最小值，就是指准备绘制的最小值</param>
    /// <param name="height">实际绘图区域的高度</param>
    /// <param name="value">需要绘制数据的当前值</param>
    /// <returns>相对于0的位置，还需要增加上面的偏值</returns>
    public static float ComputePaintLocationY(int max, int min, int height, int value) => (float) height - (float) (value - min) * 1f / (float) (max - min) * (float) height;

    /// <summary>计算绘图时的相对偏移值</summary>
    /// <param name="max">0-100分的最大值，就是指准备绘制的最大值</param>
    /// <param name="min">0-100分的最小值，就是指准备绘制的最小值</param>
    /// <param name="height">实际绘图区域的高度</param>
    /// <param name="value">需要绘制数据的当前值</param>
    /// <returns>相对于0的位置，还需要增加上面的偏值</returns>
    public static float ComputePaintLocationY(float max, float min, int height, float value) => (float) height - (float) (((double) value - (double) min) / ((double) max - (double) min)) * (float) height;

    /// <summary>绘制坐标系中的刻度线</summary>
    /// <param name="g"></param>
    /// <param name="penLine"></param>
    /// <param name="penDash"></param>
    /// <param name="font"></param>
    /// <param name="brush"></param>
    /// <param name="sf"></param>
    /// <param name="degree"></param>
    /// <param name="max"></param>
    /// <param name="min"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <param name="up"></param>
    /// <param name="down"></param>
    public static void PaintCoordinateDivide(
      Graphics g,
      Pen penLine,
      Pen penDash,
      Font font,
      Brush brush,
      StringFormat sf,
      int degree,
      int max,
      int min,
      int width,
      int height,
      int left = 60,
      int right = 8,
      int up = 8,
      int down = 8)
    {
      for (int index = 0; index <= degree; ++index)
      {
        int num1 = (max - min) * index / degree + min;
        int num2 = (int) SoftPainting.ComputePaintLocationY(max, min, height - up - down, num1) + up + 1;
        g.DrawLine(penLine, left - 1, num2, left - 4, num2);
        if ((uint) index > 0U)
          g.DrawLine(penDash, left, num2, width - right, num2);
        g.DrawString(num1.ToString(), font, brush, (RectangleF) new Rectangle(-5, num2 - font.Height / 2, left, font.Height), sf);
      }
    }

    /// <summary>根据指定的方向绘制一个箭头</summary>
    /// <param name="g"></param>
    /// <param name="brush"></param>
    /// <param name="point"></param>
    /// <param name="size"></param>
    /// <param name="direction"></param>
    public static void PaintTriangle(
      Graphics g,
      Brush brush,
      Point point,
      int size,
      GraphDirection direction)
    {
      Point[] points = new Point[4];
      switch (direction)
      {
        case GraphDirection.Upward:
          points[0] = new Point(point.X - size, point.Y);
          points[1] = new Point(point.X + size, point.Y);
          points[2] = new Point(point.X, point.Y - 2 * size);
          break;
        case GraphDirection.Ledtward:
          points[0] = new Point(point.X, point.Y - size);
          points[1] = new Point(point.X, point.Y + size);
          points[2] = new Point(point.X - 2 * size, point.Y);
          break;
        case GraphDirection.Rightward:
          points[0] = new Point(point.X, point.Y - size);
          points[1] = new Point(point.X, point.Y + size);
          points[2] = new Point(point.X + 2 * size, point.Y);
          break;
        default:
          points[0] = new Point(point.X - size, point.Y);
          points[1] = new Point(point.X + size, point.Y);
          points[2] = new Point(point.X, point.Y + 2 * size);
          break;
      }
      points[3] = points[0];
      g.FillPolygon(brush, points);
    }

    /// <summary>根据数据生成一个可视化的图形</summary>
    /// <param name="array">数据集合</param>
    /// <param name="width">需要绘制图形的宽度</param>
    /// <param name="height">需要绘制图形的高度</param>
    /// <param name="graphic">指定绘制成什么样子的图形</param>
    /// <returns>返回一个bitmap对象</returns>
    public static Bitmap GetGraphicFromArray(
      Paintdata[] array,
      int width,
      int height,
      GraphicRender graphic)
    {
      if (width < 10 && height < 10)
        throw new ArgumentException("长宽不能小于等于10");
      ((IEnumerable<Paintdata>) array).Max<Paintdata>((Func<Paintdata, int>) (m => m.Count));
      Action<Paintdata[], GraphicRender, Graphics> action = (Action<Paintdata[], GraphicRender, Graphics>) ((array1, graphic1, g) => {});
      return (Bitmap) null;
    }
  }
}
