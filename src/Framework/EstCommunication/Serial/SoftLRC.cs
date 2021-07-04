// Decompiled with JetBrains decompiler
// Type: EstCommunication.Serial.SoftLRC
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using System;

namespace EstCommunication.Serial
{
  /// <summary>
  /// 用于LRC验证的类，提供了标准的验证方法<br />
  /// The class used for LRC verification provides a standard verification method
  /// </summary>
  public class SoftLRC
  {
    /// <summary>
    /// 获取对应的数据的LRC校验码<br />
    /// Class for LRC validation that provides a standard validation method
    /// </summary>
    /// <param name="value">需要校验的数据，不包含LRC字节</param>
    /// <returns>返回带LRC校验码的字节数组，可用于串口发送</returns>
    public static byte[] LRC(byte[] value)
    {
      if (value == null)
        return (byte[]) null;
      int num = 0;
      for (int index = 0; index < value.Length; ++index)
        num += (int) value[index];
      byte[] numArray = new byte[1]
      {
        (byte) (256 - num % 256)
      };
      return SoftBasic.SpliceArray<byte>(value, numArray);
    }

    /// <summary>
    /// 检查数据是否符合LRC的验证<br />
    /// Check data for compliance with LRC validation
    /// </summary>
    /// <param name="value">等待校验的数据，是否正确</param>
    /// <returns>是否校验成功</returns>
    public static bool CheckLRC(byte[] value)
    {
      if (value == null)
        return false;
      int length = value.Length;
      byte[] numArray = new byte[length - 1];
      Array.Copy((Array) value, 0, (Array) numArray, 0, numArray.Length);
      return (int) SoftLRC.LRC(numArray)[length - 1] == (int) value[length - 1];
    }
  }
}
