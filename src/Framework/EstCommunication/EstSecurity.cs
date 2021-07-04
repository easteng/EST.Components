// Decompiled with JetBrains decompiler
// Type: EstCommunication.EstSecurity
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

namespace EstCommunication
{
  internal class EstSecurity
  {
    /// <summary>加密方法，只对当前的程序集开放</summary>
    /// <param name="enBytes">等待加密的数据</param>
    /// <returns>加密后的字节数据</returns>
    internal static byte[] ByteEncrypt(byte[] enBytes)
    {
      if (enBytes == null)
        return (byte[]) null;
      byte[] numArray = new byte[enBytes.Length];
      for (int index = 0; index < enBytes.Length; ++index)
        numArray[index] = (byte) ((uint) enBytes[index] ^ 181U);
      return numArray;
    }

    internal static void ByteEncrypt(byte[] enBytes, int offset, int count)
    {
      for (int index = offset; index < offset + count && index < enBytes.Length; ++index)
        enBytes[index] = (byte) ((uint) enBytes[index] ^ 181U);
    }

    /// <summary>解密方法，只对当前的程序集开放</summary>
    /// <param name="deBytes">等待解密的数据</param>
    /// <returns>解密后的字节数据</returns>
    internal static byte[] ByteDecrypt(byte[] deBytes) => EstSecurity.ByteEncrypt(deBytes);
  }
}
