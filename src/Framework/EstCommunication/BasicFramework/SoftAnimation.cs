// Decompiled with JetBrains decompiler
// Type: EstCommunication.BasicFramework.SoftAnimation
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace EstCommunication.BasicFramework
{
  /// <summary>系统框架支持的一些常用的动画特效</summary>
  public class SoftAnimation
  {
    /// <summary>最小的时间片段</summary>
    private static int TimeFragment { get; set; } = 20;

    /// <summary>调整控件背景色，采用了线性的颜色插补方式，实现了控件的背景色渐变，需要指定控件，颜色，以及渐变的时间</summary>
    /// <param name="control">控件</param>
    /// <param name="color">设置的颜色</param>
    /// <param name="time">时间</param>
    public static void BeginBackcolorAnimation(Control control, Color color, int time)
    {
      if (!(control.BackColor != color))
        return;
      Func<Control, Color> func = (Func<Control, Color>) (m => m.BackColor);
      Action<Control, Color> action = (Action<Control, Color>) ((m, n) => m.BackColor = n);
      ThreadPool.QueueUserWorkItem(new WaitCallback(SoftAnimation.ThreadPoolColorAnimation), (object) new object[5]
      {
        (object) control,
        (object) color,
        (object) time,
        (object) func,
        (object) action
      });
    }

    private static byte GetValue(byte Start, byte End, int i, int count) => (int) Start == (int) End ? Start : (byte) ((uint) (((int) End - (int) Start) * i / count) + (uint) Start);

    private static float GetValue(float Start, float End, int i, int count) => (double) Start == (double) End ? Start : (End - Start) * (float) i / (float) count + Start;

    private static void ThreadPoolColorAnimation(object obj)
    {
      object[] objArray = obj as object[];
      Control control = objArray[0] as Control;
      Color color = (Color) objArray[1];
      int num = (int) objArray[2];
      Func<Control, Color> func = (Func<Control, Color>) objArray[3];
      Action<Control, Color> setcolor = (Action<Control, Color>) objArray[4];
      int count = (num + SoftAnimation.TimeFragment - 1) / SoftAnimation.TimeFragment;
      Color color_old = func(control);
      try
      {
        for (int i = 0; i < count; i++)
        {
          control.Invoke((Action) (() => setcolor(control, Color.FromArgb((int) SoftAnimation.GetValue(color_old.R, color.R, i, count), (int) SoftAnimation.GetValue(color_old.G, color.G, i, count), (int) SoftAnimation.GetValue(color_old.B, color.B, i, count)))));
          Thread.Sleep(SoftAnimation.TimeFragment);
        }
        control?.Invoke((Action) (() => setcolor(control, color)));
      }
      catch
      {
      }
    }

    private static void ThreadPoolFloatAnimation(object obj)
    {
      object[] objArray = obj as object[];
      Control control = objArray[0] as Control;
      lock (control)
      {
        float value = (float) objArray[1];
        int num = (int) objArray[2];
        Func<Control, float> func = (Func<Control, float>) objArray[3];
        Action<Control, float> setValue = (Action<Control, float>) objArray[4];
        int count = (num + SoftAnimation.TimeFragment - 1) / SoftAnimation.TimeFragment;
        float value_old = func(control);
        for (int i = 0; i < count; i++)
        {
          if (!control.IsHandleCreated || control.IsDisposed)
            return;
          control.Invoke((Action) (() => setValue(control, SoftAnimation.GetValue(value_old, value, i, count))));
          Thread.Sleep(SoftAnimation.TimeFragment);
        }
        if (!control.IsHandleCreated || control.IsDisposed)
          return;
        control.Invoke((Action) (() => setValue(control, value)));
      }
    }
  }
}
