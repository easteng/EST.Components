// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.BasicFramework.SoftSecurity
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ESTCore.Common.BasicFramework
{
    /// <summary>字符串加密解密相关的自定义类</summary>
    public static class SoftSecurity
    {
        /// <summary>加密数据，采用对称加密的方式</summary>
        /// <param name="pToEncrypt">待加密的数据</param>
        /// <returns>加密后的数据</returns>
        internal static string MD5Encrypt(string pToEncrypt) => SoftSecurity.MD5Encrypt(pToEncrypt, "zxcvBNMM");

        /// <summary>加密数据，采用对称加密的方式</summary>
        /// <param name="pToEncrypt">待加密的数据</param>
        /// <param name="Password">密钥，长度为8，英文或数字</param>
        /// <returns>加密后的数据</returns>
        public static string MD5Encrypt(string pToEncrypt, string Password)
        {
            string s = Password;
            DESCryptoServiceProvider cryptoServiceProvider = new DESCryptoServiceProvider();
            byte[] bytes = Encoding.Default.GetBytes(pToEncrypt);
            cryptoServiceProvider.Key = Encoding.ASCII.GetBytes(s);
            cryptoServiceProvider.IV = Encoding.ASCII.GetBytes(s);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, cryptoServiceProvider.CreateEncryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(bytes, 0, bytes.Length);
            cryptoStream.FlushFinalBlock();
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte num in memoryStream.ToArray())
                stringBuilder.AppendFormat("{0:X2}", (object)num);
            stringBuilder.ToString();
            return stringBuilder.ToString();
        }

        /// <summary>解密过程，使用的是对称的加密</summary>
        /// <param name="pToDecrypt">等待解密的字符</param>
        /// <returns>返回原密码，如果解密失败，返回‘解密失败’</returns>
        internal static string MD5Decrypt(string pToDecrypt) => SoftSecurity.MD5Decrypt(pToDecrypt, "zxcvBNMM");

        /// <summary>解密过程，使用的是对称的加密</summary>
        /// <param name="pToDecrypt">等待解密的字符</param>
        /// <param name="password">密钥，长度为8，英文或数字</param>
        /// <returns>返回原密码，如果解密失败，返回‘解密失败’</returns>
        public static string MD5Decrypt(string pToDecrypt, string password)
        {
            if (pToDecrypt == "")
                return pToDecrypt;
            string s = password;
            DESCryptoServiceProvider cryptoServiceProvider = new DESCryptoServiceProvider();
            byte[] buffer = new byte[pToDecrypt.Length / 2];
            for (int index = 0; index < pToDecrypt.Length / 2; ++index)
            {
                int int32 = Convert.ToInt32(pToDecrypt.Substring(index * 2, 2), 16);
                buffer[index] = (byte)int32;
            }
            cryptoServiceProvider.Key = Encoding.ASCII.GetBytes(s);
            cryptoServiceProvider.IV = Encoding.ASCII.GetBytes(s);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, cryptoServiceProvider.CreateDecryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(buffer, 0, buffer.Length);
            cryptoStream.FlushFinalBlock();
            cryptoStream.Dispose();
            return Encoding.Default.GetString(memoryStream.ToArray());
        }
    }
}
