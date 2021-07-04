// Decompiled with JetBrains decompiler
// Type: EstCommunication.Serial.SoftCRC16
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using System;

namespace EstCommunication.Serial
{
  /// <summary>
  /// 用于CRC16验证的类，提供了标准的验证方法，可以方便快速的对数据进行CRC校验<br />
  /// The class for CRC16 validation provides a standard validation method that makes it easy to CRC data quickly
  /// </summary>
  /// <remarks>
  /// 本类提供了几个静态的方法，用来进行CRC16码的计算和验证的，多项式码可以自己指定配置，但是预置的寄存器为0xFF 0xFF
  /// </remarks>
  /// <example>
  /// 先演示如何校验一串数据的CRC码
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Serial\SoftCRC16.cs" region="Example1" title="SoftCRC16示例" />
  /// 然后下面是如何生成你自己的CRC校验码
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Serial\SoftCRC16.cs" region="Example2" title="SoftCRC16示例" />
  /// </example>
  public class SoftCRC16
  {
    /// <summary>
    /// 来校验对应的接收数据的CRC校验码，默认多项式码为0xA001<br />
    /// To verify the CRC check code corresponding to the received data, the default polynomial code is 0xA001
    /// </summary>
    /// <param name="value">需要校验的数据，带CRC校验码</param>
    /// <returns>返回校验成功与否</returns>
    public static bool CheckCRC16(byte[] value) => SoftCRC16.CheckCRC16(value, (byte) 160, (byte) 1);

    /// <summary>
    /// 指定多项式码来校验对应的接收数据的CRC校验码<br />
    /// Specifies a polynomial code to validate the corresponding CRC check code for the received data
    /// </summary>
    /// <param name="value">需要校验的数据，带CRC校验码</param>
    /// <param name="CH">多项式码高位</param>
    /// <param name="CL">多项式码低位</param>
    /// <returns>返回校验成功与否</returns>
    public static bool CheckCRC16(byte[] value, byte CH, byte CL)
    {
      if (value == null || value.Length < 2)
        return false;
      int length = value.Length;
      byte[] numArray1 = new byte[length - 2];
      Array.Copy((Array) value, 0, (Array) numArray1, 0, numArray1.Length);
      byte[] numArray2 = SoftCRC16.CRC16(numArray1, CH, CL);
      return (int) numArray2[length - 2] == (int) value[length - 2] && (int) numArray2[length - 1] == (int) value[length - 1];
    }

    /// <summary>
    /// 获取对应的数据的CRC校验码，默认多项式码为0xA001<br />
    /// Get the CRC check code of the corresponding data, the default polynomial code is 0xA001
    /// </summary>
    /// <param name="value">需要校验的数据，不包含CRC字节</param>
    /// <returns>返回带CRC校验码的字节数组，可用于串口发送</returns>
    public static byte[] CRC16(byte[] value) => SoftCRC16.CRC16(value, (byte) 160, (byte) 1);

    /// <summary>
    /// 通过指定多项式码来获取对应的数据的CRC校验码<br />
    /// The CRC check code of the corresponding data is obtained by specifying the polynomial code
    /// </summary>
    /// <param name="value">需要校验的数据，不包含CRC字节</param>
    /// <param name="CL">多项式码地位</param>
    /// <param name="CH">多项式码高位</param>
    /// <param name="preH">预置的高位值</param>
    /// <param name="preL">预置的低位值</param>
    /// <returns>返回带CRC校验码的字节数组，可用于串口发送</returns>
    public static byte[] CRC16(byte[] value, byte CH, byte CL, byte preH = 255, byte preL = 255)
    {
      byte[] numArray = new byte[value.Length + 2];
      value.CopyTo((Array) numArray, 0);
      byte num1 = preL;
      byte num2 = preH;
      foreach (byte num3 in value)
      {
        num1 ^= num3;
        for (int index = 0; index <= 7; ++index)
        {
          byte num4 = num2;
          byte num5 = num1;
          num2 >>= 1;
          num1 >>= 1;
          if (((int) num4 & 1) == 1)
            num1 |= (byte) 128;
          if (((int) num5 & 1) == 1)
          {
            num2 ^= CH;
            num1 ^= CL;
          }
        }
      }
      numArray[numArray.Length - 2] = num1;
      numArray[numArray.Length - 1] = num2;
      return numArray;
    }
  }
}
