// Decompiled with JetBrains decompiler
// Type: EstCommunication.Instrument.DLT.DLTTransform
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EstCommunication.Instrument.DLT
{
  /// <summary>DTL数据转换</summary>
  public class DLTTransform
  {
    /// <summary>Byte[]转ToHexString</summary>
    /// <param name="content">原始的字节内容</param>
    /// <param name="length">长度信息</param>
    /// <returns></returns>
    public static OperateResult<string> TransStringFromDLt(
      byte[] content,
      ushort length)
    {
      OperateResult<string> operateResult;
      try
      {
        string empty = string.Empty;
        byte[] array = ((IEnumerable<byte>) content.SelectBegin<byte>((int) length)).Reverse<byte>().ToArray<byte>();
        for (int index = 0; index < array.Length; ++index)
          array[index] = (byte) ((uint) array[index] - 51U);
        operateResult = OperateResult.CreateSuccessResult<string>(Encoding.ASCII.GetString(array));
      }
      catch (Exception ex)
      {
        operateResult = new OperateResult<string>(ex.Message + " Reason: " + content.ToHexString(' '));
      }
      return operateResult;
    }

    /// <summary>Byte[]转Dlt double[]</summary>
    /// <param name="content">原始的字节数据</param>
    /// <param name="length">需要转换的数据长度</param>
    /// <param name="format">当前数据的解析格式</param>
    /// <returns>结果内容</returns>
    public static OperateResult<double[]> TransDoubleFromDLt(
      byte[] content,
      ushort length,
      string format = "XXXXXX.XX")
    {
      try
      {
        format = format.ToUpper();
        int length1 = format.Count<char>((Func<char, bool>) (m => m == 'X')) / 2;
        int num = format.IndexOf('.') >= 0 ? format.Length - format.IndexOf('.') - 1 : 0;
        double[] numArray = new double[(int) length];
        for (int index1 = 0; index1 < numArray.Length; ++index1)
        {
          byte[] array = ((IEnumerable<byte>) content.SelectMiddle<byte>(index1 * length1, length1)).Reverse<byte>().ToArray<byte>();
          for (int index2 = 0; index2 < array.Length; ++index2)
            array[index2] = (byte) ((uint) array[index2] - 51U);
          numArray[index1] = Convert.ToDouble(array.ToHexString()) / Math.Pow(10.0, (double) num);
        }
        return OperateResult.CreateSuccessResult<double[]>(numArray);
      }
      catch (Exception ex)
      {
        return new OperateResult<double[]>(ex.Message);
      }
    }
  }
}
