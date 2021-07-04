// Decompiled with JetBrains decompiler
// Type: EstCommunication.BasicFramework.SoftZipped
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using System;
using System.IO;
using System.IO.Compression;

namespace EstCommunication.BasicFramework
{
  /// <summary>一个负责压缩解压数据字节的类</summary>
  public class SoftZipped
  {
    /// <summary>压缩字节数据</summary>
    /// <param name="bytes">等待被压缩的数据</param>
    /// <exception cref="T:System.ArgumentNullException"></exception>
    /// <returns>压缩之后的字节数据</returns>
    public static byte[] CompressBytes(byte[] bytes)
    {
      if (bytes == null)
        throw new ArgumentNullException(nameof (bytes));
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (GZipStream gzipStream = new GZipStream((Stream) memoryStream, CompressionMode.Compress))
          gzipStream.Write(bytes, 0, bytes.Length);
        return memoryStream.ToArray();
      }
    }

    /// <summary>解压压缩后的数据</summary>
    /// <param name="bytes">压缩后的数据</param>
    /// <exception cref="T:System.ArgumentNullException"></exception>
    /// <returns>压缩前的原始字节数据</returns>
    public static byte[] Decompress(byte[] bytes)
    {
      if (bytes == null)
        throw new ArgumentNullException(nameof (bytes));
      using (MemoryStream memoryStream1 = new MemoryStream(bytes))
      {
        using (GZipStream gzipStream = new GZipStream((Stream) memoryStream1, CompressionMode.Decompress))
        {
          using (MemoryStream memoryStream2 = new MemoryStream())
          {
            int count1 = 1024;
            byte[] buffer = new byte[count1];
            int count2;
            while ((count2 = gzipStream.Read(buffer, 0, count1)) > 0)
              memoryStream2.Write(buffer, 0, count2);
            return memoryStream2.ToArray();
          }
        }
      }
    }
  }
}
