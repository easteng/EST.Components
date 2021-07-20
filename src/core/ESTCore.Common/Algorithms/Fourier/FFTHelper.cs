// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Algorithms.Fourier.FFTHelper
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace ESTCore.Common.Algorithms.Fourier
{
    /// <summary>离散傅氏变换的快速算法，处理的信号，适合单周期信号数为2的N次方个，支持变换及逆变换</summary>
    public class FFTHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xreal"></param>
        /// <param name="ximag"></param>
        /// <param name="n"></param>
        private static void bitrp(double[] xreal, double[] ximag, int n)
        {
            int num1 = 1;
            int num2 = 0;
            for (; num1 < n; num1 *= 2)
                ++num2;
            for (int index1 = 0; index1 < n; ++index1)
            {
                int num3 = index1;
                int index2 = 0;
                for (int index3 = 0; index3 < num2; ++index3)
                {
                    index2 = index2 * 2 + num3 % 2;
                    num3 /= 2;
                }
                if (index2 > index1)
                {
                    double num4 = xreal[index1];
                    xreal[index1] = xreal[index2];
                    xreal[index2] = num4;
                    double num5 = ximag[index1];
                    ximag[index1] = ximag[index2];
                    ximag[index2] = num5;
                }
            }
        }

        /// <summary>快速傅立叶变换</summary>
        /// <param name="xreal">实数部分</param>
        /// <returns>变换后的数组值</returns>
        public static double[] FFT(double[] xreal) => FFTHelper.FFTValue(xreal, new double[xreal.Length]);

        /// <summary>获取FFT变换后的显示图形，需要指定图形的相关参数</summary>
        /// <param name="xreal">实数部分的值</param>
        /// <param name="width">图形的宽度</param>
        /// <param name="heigh">图形的高度</param>
        /// <param name="lineColor">线条颜色</param>
        /// <param name="isSqrtDouble">是否开两次根，显示的噪点信息会更新明显</param>
        /// <returns>等待呈现的图形</returns>
        /// <remarks>
        /// <note type="warning">.net standrard2.0 下不支持。</note>
        /// </remarks>
        //public static Bitmap GetFFTImage(
        //  double[] xreal,
        //  int width,
        //  int heigh,
        //  Color lineColor,
        //  bool isSqrtDouble = false)
        //{
        //    double[] ximag = new double[xreal.Length];
        //    double[] numArray = FFTHelper.FFTValue(xreal, ximag, isSqrtDouble);
        //    Bitmap bitmap = new Bitmap(width, heigh);
        //    Graphics graphics = Graphics.FromImage((Image)bitmap);
        //    graphics.SmoothingMode = SmoothingMode.HighQuality;
        //    graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        //    graphics.Clear(Color.White);
        //    Pen pen1 = new Pen(Color.DimGray, 1f);
        //    Pen pen2 = new Pen(Color.LightGray, 1f);
        //    Pen pen3 = new Pen(lineColor, 1f);
        //    pen2.DashPattern = new float[2] { 5f, 5f };
        //    pen2.DashStyle = DashStyle.Custom;
        //    Font defaultFont = SystemFonts.DefaultFont;
        //    StringFormat format1 = new StringFormat();
        //    format1.Alignment = StringAlignment.Far;
        //    format1.LineAlignment = StringAlignment.Center;
        //    StringFormat format2 = new StringFormat();
        //    format2.LineAlignment = StringAlignment.Center;
        //    format2.Alignment = StringAlignment.Center;
        //    int num1 = 20;
        //    int num2 = 49;
        //    int num3 = 49;
        //    int num4 = 30;
        //    int num5 = 9;
        //    float num6 = (float)(heigh - num1 - num4);
        //    float num7 = (float)(width - num2 - num3);
        //    if (numArray.Length > 1)
        //    {
        //        double num8 = ((IEnumerable<double>)numArray).Max();
        //        double num9 = ((IEnumerable<double>)numArray).Min();
        //        double num10 = num8 - num9 > 1.0 ? num8 : num9 + 1.0;
        //        double num11 = num10 - num9;
        //        List<float> floatList = new List<float>();
        //        if (numArray.Length >= 2)
        //        {
        //            if (numArray[0] > numArray[1])
        //                floatList.Add(0.0f);
        //            for (int index = 1; index < numArray.Length - 2; ++index)
        //            {
        //                if (numArray[index - 1] < numArray[index] && numArray[index] > numArray[index + 1])
        //                    floatList.Add((float)index);
        //            }
        //            if (numArray[numArray.Length - 1] > numArray[numArray.Length - 2])
        //                floatList.Add((float)(numArray.Length - 1));
        //        }
        //        for (int index = 0; index < num5; ++index)
        //        {
        //            RectangleF layoutRectangle = new RectangleF(-10f, (float)index / (float)(num5 - 1) * num6, (float)num2 + 8f, 20f);
        //            double num12 = (double)(num5 - 1 - index) * num11 / (double)(num5 - 1) + num9;
        //            graphics.DrawString(num12.ToString("F1"), defaultFont, Brushes.Black, layoutRectangle, format1);
        //            graphics.DrawLine(pen2, (float)(num2 - 3), num6 * (float)index / (float)(num5 - 1) + (float)num1, (float)(width - num3), num6 * (float)index / (float)(num5 - 1) + (float)num1);
        //        }
        //        float num13 = num7 / (float)numArray.Length;
        //        for (int index = 0; index < floatList.Count; ++index)
        //        {
        //            if (numArray[(int)floatList[index]] * 200.0 / num10 > 1.0)
        //            {
        //                graphics.DrawLine(pen2, (float)((double)floatList[index] * (double)num13 + (double)num2 + 1.0), (float)num1, (float)((double)floatList[index] * (double)num13 + (double)num2 + 1.0), (float)(heigh - num4));
        //                RectangleF layoutRectangle = new RectangleF((float)((double)floatList[index] * (double)num13 + (double)num2 + 1.0 - 40.0), (float)(heigh - num4 + 1), 80f, 20f);
        //                graphics.DrawString(floatList[index].ToString(), defaultFont, Brushes.DeepPink, layoutRectangle, format2);
        //            }
        //        }
        //        for (int index = 0; index < numArray.Length; ++index)
        //            graphics.DrawLine(Pens.Tomato, new PointF()
        //            {
        //                X = (float)((double)index * (double)num13 + (double)num2 + 1.0),
        //                Y = num6 - (float)((numArray[index] - num9) * (double)num6 / num11) + (float)num1
        //            }, new PointF()
        //            {
        //                X = (float)((double)index * (double)num13 + (double)num2 + 1.0),
        //                Y = num6 - (float)((num9 - num9) * (double)num6 / num11) + (float)num1
        //            });
        //    }
        //    else
        //    {
        //        double num8 = 100.0;
        //        double num9 = 0.0;
        //        double num10 = num8 - num9;
        //        for (int index = 0; index < num5; ++index)
        //        {
        //            RectangleF layoutRectangle = new RectangleF(-10f, (float)index / (float)(num5 - 1) * num6, (float)num2 + 8f, 20f);
        //            double num11 = (double)(num5 - 1 - index) * num10 / (double)(num5 - 1) + num9;
        //            graphics.DrawString(num11.ToString("F1"), defaultFont, Brushes.Black, layoutRectangle, format1);
        //            graphics.DrawLine(pen2, (float)(num2 - 3), num6 * (float)index / (float)(num5 - 1) + (float)num1, (float)(width - num3), num6 * (float)index / (float)(num5 - 1) + (float)num1);
        //        }
        //    }
        //    pen2.Dispose();
        //    pen1.Dispose();
        //    pen3.Dispose();
        //    defaultFont.Dispose();
        //    format1.Dispose();
        //    format2.Dispose();
        //    graphics.Dispose();
        //    return bitmap;
        //}

        /// <summary>快速傅立叶变换</summary>
        /// <param name="xreal">实数部分，数组长度最好为2的n次方</param>
        /// <param name="ximag">虚数部分，数组长度最好为2的n次方</param>
        /// <param name="isSqrtDouble">是否开两次根，显示的噪点信息会更新明显</param>
        /// <returns>变换后的数组值</returns>
        public static double[] FFTValue(double[] xreal, double[] ximag, bool isSqrtDouble = false)
        {
            int num1 = 2;
            while (num1 <= xreal.Length)
                num1 *= 2;
            int n = num1 / 2;
            double[] numArray1 = new double[n / 2];
            double[] numArray2 = new double[n / 2];
            FFTHelper.bitrp(xreal, ximag, n);
            double num2 = -2.0 * Math.PI / (double)n;
            double num3 = Math.Cos(num2);
            double num4 = Math.Sin(num2);
            numArray1[0] = 1.0;
            numArray2[0] = 0.0;
            for (int index = 1; index < n / 2; ++index)
            {
                numArray1[index] = numArray1[index - 1] * num3 - numArray2[index - 1] * num4;
                numArray2[index] = numArray1[index - 1] * num4 + numArray2[index - 1] * num3;
            }
            for (int index1 = 2; index1 <= n; index1 *= 2)
            {
                for (int index2 = 0; index2 < n; index2 += index1)
                {
                    for (int index3 = 0; index3 < index1 / 2; ++index3)
                    {
                        int index4 = index2 + index3;
                        int index5 = index4 + index1 / 2;
                        int index6 = n * index3 / index1;
                        double num5 = numArray1[index6] * xreal[index5] - numArray2[index6] * ximag[index5];
                        double num6 = numArray1[index6] * ximag[index5] + numArray2[index6] * xreal[index5];
                        double num7 = xreal[index4];
                        double num8 = ximag[index4];
                        xreal[index4] = num7 + num5;
                        ximag[index4] = num8 + num6;
                        xreal[index5] = num7 - num5;
                        ximag[index5] = num8 - num6;
                    }
                }
            }
            double[] numArray3 = new double[n];
            for (int index = 0; index < numArray3.Length; ++index)
            {
                numArray3[index] = Math.Sqrt(Math.Pow(xreal[index], 2.0) + Math.Pow(ximag[index], 2.0));
                if (isSqrtDouble)
                    numArray3[index] = Math.Sqrt(numArray3[index]);
            }
            return numArray3;
        }

        /// <summary>快速傅立叶变换</summary>
        /// <param name="xreal">实数部分，数组长度最好为2的n次方</param>
        /// <param name="ximag">虚数部分，数组长度最好为2的n次方</param>
        /// <returns>变换后的数组值</returns>
        public static int FFT(double[] xreal, double[] ximag) => FFTHelper.FFTValue(xreal, ximag).Length;

        /// <summary>快速傅立叶变换</summary>
        /// <param name="xreal">实数部分，数组长度最好为2的n次方</param>
        /// <param name="ximag">虚数部分，数组长度最好为2的n次方</param>
        /// <returns>变换后的数组值</returns>
        public static int FFT(float[] xreal, float[] ximag) => FFTHelper.FFT(((IEnumerable<float>)xreal).Select<float, double>((Func<float, double>)(m => (double)m)).ToArray<double>(), ((IEnumerable<float>)ximag).Select<float, double>((Func<float, double>)(m => (double)m)).ToArray<double>());

        /// <summary>快速傅立叶变换的逆变换</summary>
        /// <param name="xreal">实数部分，数组长度最好为2的n次方</param>
        /// <param name="ximag">虚数部分，数组长度最好为2的n次方</param>
        /// <returns>2的多少次方</returns>
        public static int IFFT(float[] xreal, float[] ximag) => FFTHelper.IFFT(((IEnumerable<float>)xreal).Select<float, double>((Func<float, double>)(m => (double)m)).ToArray<double>(), ((IEnumerable<float>)ximag).Select<float, double>((Func<float, double>)(m => (double)m)).ToArray<double>());

        /// <summary>快速傅立叶变换的逆变换</summary>
        /// <param name="xreal">实数部分，数组长度最好为2的n次方</param>
        /// <param name="ximag">虚数部分，数组长度最好为2的n次方</param>
        /// <returns>2的多少次方</returns>
        public static int IFFT(double[] xreal, double[] ximag)
        {
            int num1 = 2;
            while (num1 <= xreal.Length)
                num1 *= 2;
            int n = num1 / 2;
            double[] numArray1 = new double[n / 2];
            double[] numArray2 = new double[n / 2];
            FFTHelper.bitrp(xreal, ximag, n);
            double num2 = 2.0 * Math.PI / (double)n;
            double num3 = Math.Cos(num2);
            double num4 = Math.Sin(num2);
            numArray1[0] = 1.0;
            numArray2[0] = 0.0;
            for (int index = 1; index < n / 2; ++index)
            {
                numArray1[index] = numArray1[index - 1] * num3 - numArray2[index - 1] * num4;
                numArray2[index] = numArray1[index - 1] * num4 + numArray2[index - 1] * num3;
            }
            for (int index1 = 2; index1 <= n; index1 *= 2)
            {
                for (int index2 = 0; index2 < n; index2 += index1)
                {
                    for (int index3 = 0; index3 < index1 / 2; ++index3)
                    {
                        int index4 = index2 + index3;
                        int index5 = index4 + index1 / 2;
                        int index6 = n * index3 / index1;
                        double num5 = numArray1[index6] * xreal[index5] - numArray2[index6] * ximag[index5];
                        double num6 = numArray1[index6] * ximag[index5] + numArray2[index6] * xreal[index5];
                        double num7 = xreal[index4];
                        double num8 = ximag[index4];
                        xreal[index4] = num7 + num5;
                        ximag[index4] = num8 + num6;
                        xreal[index5] = num7 - num5;
                        ximag[index5] = num8 - num6;
                    }
                }
            }
            for (int index = 0; index < n; ++index)
            {
                xreal[index] /= (double)n;
                ximag[index] /= (double)n;
            }
            return n;
        }
    }
}
