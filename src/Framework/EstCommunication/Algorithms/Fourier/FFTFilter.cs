// Decompiled with JetBrains decompiler
// Type: EstCommunication.Algorithms.Fourier.FFTFilter
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace EstCommunication.Algorithms.Fourier
{
  /// <summary>一个基于傅立叶变换的一个滤波算法</summary>
  /// <remarks>非常感谢来自北京的monk网友，提供了完整的解决方法。</remarks>
  public class FFTFilter
  {
    /// <summary>对指定的数据进行填充，方便的进行傅立叶计算</summary>
    /// <typeparam name="T">数据的数据类型</typeparam>
    /// <param name="source">数据源</param>
    /// <param name="putLength">输出的长度</param>
    /// <returns>填充结果</returns>
    public static List<T> FillDataArray<T>(List<T> source, out int putLength)
    {
      int num = (int) (Math.Pow(2.0, (double) Convert.ToString(source.Count, 2).Length) - (double) source.Count) / 2 + 1;
      putLength = num;
      T obj1 = source[0];
      T obj2 = source[source.Count - 1];
      for (int index = 0; index < num; ++index)
        source.Insert(0, obj1);
      for (int index = 0; index < num; ++index)
        source.Add(obj2);
      return source;
    }

    /// <summary>对指定的原始数据进行滤波，并返回成功的数据值</summary>
    /// <param name="source">数据源，数组的长度需要为2的n次方。</param>
    /// <param name="filter">滤波值：最大值为1，不能低于0，越接近1，滤波强度越强，也可能会导致失去真实信号，为0时没有滤波效果。</param>
    /// <returns>滤波后的数据值</returns>
    public static double[] FilterFFT(double[] source, double filter)
    {
      double[] numArray1 = new double[source.Length];
      int putLength;
      float[] numArray2 = FFTFilter.Filter(FFTFilter.FillDataArray<double>(new List<double>((IEnumerable<double>) source), out putLength).ToArray(), filter);
      for (int index = 0; index < numArray1.Length; ++index)
        numArray1[index] = (double) numArray2[index + putLength];
      return numArray1;
    }

    /// <summary>对指定的原始数据进行滤波，并返回成功的数据值</summary>
    /// <param name="source">数据源，数组的长度需要为2的n次方。</param>
    /// <param name="filter">滤波值：最大值为1，不能低于0，越接近1，滤波强度越强，也可能会导致失去真实信号，为0时没有滤波效果。</param>
    /// <returns>滤波后的数据值</returns>
    public static float[] FilterFFT(float[] source, double filter)
    {
      float[] numArray1 = new float[source.Length];
      int putLength;
      float[] numArray2 = FFTFilter.Filter(FFTFilter.FillDataArray<float>(new List<float>((IEnumerable<float>) source), out putLength).ToArray(), filter);
      for (int index = 0; index < numArray1.Length; ++index)
        numArray1[index] = numArray2[index + putLength];
      return numArray1;
    }

    /// <summary>对指定的原始数据进行滤波，并返回成功的数据值</summary>
    /// <param name="source">数据源，数组的长度需要为2的n次方。</param>
    /// <param name="filter">滤波值：最大值为1，不能低于0，越接近1，滤波强度越强，也可能会导致失去真实信号，为0时没有滤波效果。</param>
    /// <returns>滤波后的数据值</returns>
    private static float[] Filter(float[] source, double filter) => FFTFilter.Filter(((IEnumerable<float>) source).Select<float, double>((Func<float, double>) (m => (double) m)).ToArray<double>(), filter);

    /// <summary>对指定的原始数据进行滤波，并返回成功的数据值</summary>
    /// <param name="source">数据源，数组的长度需要为2的n次方。</param>
    /// <param name="filter">滤波值：最大值为1，不能低于0，越接近1，滤波强度越强，也可能会导致失去真实信号，为0时没有滤波效果。</param>
    /// <returns>滤波后的数据值</returns>
    private static float[] Filter(double[] source, double filter)
    {
      if (filter > 1.0)
        filter = 1.0;
      if (filter < 0.0)
        filter = 0.0;
      double[] xreal = new double[source.Length];
      double[] ximag = new double[source.Length];
      List<double> source1 = new List<double>();
      for (int index = 0; index < source.Length; ++index)
      {
        xreal[index] = source[index];
        ximag[index] = 0.0;
      }
      double[] numArray = FFTHelper.FFTValue(xreal, ximag);
      int length = numArray.Length;
      double num1 = ((IEnumerable<double>) numArray).Max();
      for (int index = 0; index < numArray.Length; ++index)
      {
        if (numArray[index] < num1 * filter)
        {
          xreal[index] = 0.0;
          ximag[index] = 0.0;
        }
      }
      int num2 = FFTHelper.IFFT(xreal, ximag);
      for (int index = 0; index < num2; ++index)
        source1.Add(Math.Sqrt(xreal[index] * xreal[index] + ximag[index] * ximag[index]));
      return source1.Select<double, float>((Func<double, float>) (m => (float) m)).ToArray<float>();
    }
  }
}
