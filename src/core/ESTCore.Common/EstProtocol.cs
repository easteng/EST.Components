// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.EstProtocol
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;

using System;
using System.Collections.Generic;
using System.Text;

namespace ESTCore.Common
{
    /// <summary>用于本程序集访问通信的暗号说明</summary>
    internal class EstProtocol
    {
        /// <summary>规定所有的网络传输指令头都为32字节</summary>
        internal const int HeadByteLength = 32;
        /// <summary>所有网络通信中的缓冲池数据信息</summary>
        internal const int ProtocolBufferSize = 1024;
        /// <summary>用于心跳程序的暗号信息</summary>
        internal const int ProtocolCheckSecends = 1;
        /// <summary>客户端退出消息</summary>
        internal const int ProtocolClientQuit = 2;
        /// <summary>因为客户端达到上限而拒绝登录</summary>
        internal const int ProtocolClientRefuseLogin = 3;
        /// <summary>允许客户端登录到服务器</summary>
        internal const int ProtocolClientAllowLogin = 4;
        /// <summary>客户端登录的暗号信息</summary>
        internal const int ProtocolAccountLogin = 5;
        /// <summary>客户端拒绝登录的暗号信息</summary>
        internal const int ProtocolAccountRejectLogin = 6;
        /// <summary>说明发送的只是文本信息</summary>
        internal const int ProtocolUserString = 1001;
        /// <summary>发送的数据就是普通的字节数组</summary>
        internal const int ProtocolUserBytes = 1002;
        /// <summary>发送的数据就是普通的图片数据</summary>
        internal const int ProtocolUserBitmap = 1003;
        /// <summary>发送的数据是一条异常的数据，字符串为异常消息</summary>
        internal const int ProtocolUserException = 1004;
        /// <summary>说明发送的数据是字符串的数组</summary>
        internal const int ProtocolUserStringArray = 1005;
        /// <summary>请求文件下载的暗号</summary>
        internal const int ProtocolFileDownload = 2001;
        /// <summary>请求文件上传的暗号</summary>
        internal const int ProtocolFileUpload = 2002;
        /// <summary>请求删除文件的暗号</summary>
        internal const int ProtocolFileDelete = 2003;
        /// <summary>文件校验成功</summary>
        internal const int ProtocolFileCheckRight = 2004;
        /// <summary>文件校验失败</summary>
        internal const int ProtocolFileCheckError = 2005;
        /// <summary>文件保存失败</summary>
        internal const int ProtocolFileSaveError = 2006;
        /// <summary>请求文件列表的暗号</summary>
        internal const int ProtocolFileDirectoryFiles = 2007;
        /// <summary>请求子文件的列表暗号</summary>
        internal const int ProtocolFileDirectories = 2008;
        /// <summary>进度返回暗号</summary>
        internal const int ProtocolProgressReport = 2009;
        /// <summary>返回的错误信息</summary>
        internal const int ProtocolErrorMsg = 2010;
        /// <summary>请求删除多个文件的暗号</summary>
        internal const int ProtocolFilesDelete = 2011;
        /// <summary>请求删除文件夹的暗号</summary>
        internal const int ProtocolFolderDelete = 2012;
        /// <summary>请求当前的文件是否存在</summary>
        internal const int ProtocolFileExists = 2013;
        /// <summary>不压缩数据字节</summary>
        internal const int ProtocolNoZipped = 3001;
        /// <summary>压缩数据字节</summary>
        internal const int ProtocolZipped = 3002;

        /// <summary>生成终极传送指令的方法，所有的数据均通过该方法出来</summary>
        /// <param name="command">命令头</param>
        /// <param name="customer">自用自定义</param>
        /// <param name="token">令牌</param>
        /// <param name="data">字节数据</param>
        /// <returns>包装后的数据信息</returns>
        internal static byte[] CommandBytes(int command, int customer, Guid token, byte[] data)
        {
            int num1 = 3001;
            int num2 = data == null ? 0 : data.Length;
            byte[] enBytes = new byte[32 + num2];
            BitConverter.GetBytes(command).CopyTo((Array)enBytes, 0);
            BitConverter.GetBytes(customer).CopyTo((Array)enBytes, 4);
            BitConverter.GetBytes(num1).CopyTo((Array)enBytes, 8);
            token.ToByteArray().CopyTo((Array)enBytes, 12);
            if (num2 > 0)
            {
                BitConverter.GetBytes(num2).CopyTo((Array)enBytes, 28);
                Array.Copy((Array)data, 0, (Array)enBytes, 32, num2);
                EstSecurity.ByteEncrypt(enBytes, 32, num2);
            }
            return enBytes;
        }

        /// <summary>解析接收到数据，先解压缩后进行解密</summary>
        /// <param name="head">指令头</param>
        /// <param name="content">指令的内容</param>
        /// <return>真实的数据内容</return>
        internal static byte[] CommandAnalysis(byte[] head, byte[] content)
        {
            if (content == null)
                return (byte[])null;
            if (BitConverter.ToInt32(head, 8) == 3002)
                content = SoftZipped.Decompress(content);
            return EstSecurity.ByteDecrypt(content);
        }

        /// <summary>获取发送字节数据的实际数据，带指令头</summary>
        /// <param name="customer">用户数据</param>
        /// <param name="token">令牌</param>
        /// <param name="data">字节信息</param>
        /// <returns>包装后的指令信息</returns>
        internal static byte[] CommandBytes(int customer, Guid token, byte[] data) => EstProtocol.CommandBytes(1002, customer, token, data);

        /// <summary>获取发送字节数据的实际数据，带指令头</summary>
        /// <param name="customer">用户数据</param>
        /// <param name="token">令牌</param>
        /// <param name="data">字符串数据信息</param>
        /// <returns>包装后的指令信息</returns>
        internal static byte[] CommandBytes(int customer, Guid token, string data) => data == null ? EstProtocol.CommandBytes(1001, customer, token, (byte[])null) : EstProtocol.CommandBytes(1001, customer, token, Encoding.Unicode.GetBytes(data));

        /// <summary>获取发送字节数据的实际数据，带指令头</summary>
        /// <param name="customer">用户数据</param>
        /// <param name="token">令牌</param>
        /// <param name="data">字符串数据信息</param>
        /// <returns>包装后的指令信息</returns>
        internal static byte[] CommandBytes(int customer, Guid token, string[] data) => EstProtocol.CommandBytes(1005, customer, token, EstProtocol.PackStringArrayToByte(data));

        /// <inheritdoc cref="M:ESTCore.Common.EstProtocol.PackStringArrayToByte(System.String[])" />
        internal static byte[] PackStringArrayToByte(string data) => EstProtocol.PackStringArrayToByte(new string[1]
        {
      data
        });

        /// <summary>将字符串打包成字节数组内容</summary>
        /// <param name="data">字符串数组</param>
        /// <returns>打包后的原始数据内容</returns>
        internal static byte[] PackStringArrayToByte(string[] data)
        {
            if (data == null)
                data = new string[0];
            List<byte> byteList = new List<byte>();
            byteList.AddRange((IEnumerable<byte>)BitConverter.GetBytes(data.Length));
            for (int index = 0; index < data.Length; ++index)
            {
                if (!string.IsNullOrEmpty(data[index]))
                {
                    byte[] bytes = Encoding.Unicode.GetBytes(data[index]);
                    byteList.AddRange((IEnumerable<byte>)BitConverter.GetBytes(bytes.Length));
                    byteList.AddRange((IEnumerable<byte>)bytes);
                }
                else
                    byteList.AddRange((IEnumerable<byte>)BitConverter.GetBytes(0));
            }
            return byteList.ToArray();
        }

        /// <summary>将字节数组还原成真实的字符串数组</summary>
        /// <param name="content">原始字节数组</param>
        /// <returns>解析后的字符串内容</returns>
        internal static string[] UnPackStringArrayFromByte(byte[] content)
        {
            if (content != null && content.Length < 4)
                return (string[])null;
            int startIndex1 = 0;
            int int32_1 = BitConverter.ToInt32(content, startIndex1);
            string[] strArray = new string[int32_1];
            int startIndex2 = startIndex1 + 4;
            for (int index1 = 0; index1 < int32_1; ++index1)
            {
                int int32_2 = BitConverter.ToInt32(content, startIndex2);
                int index2 = startIndex2 + 4;
                strArray[index1] = int32_2 <= 0 ? string.Empty : Encoding.Unicode.GetString(content, index2, int32_2);
                startIndex2 = index2 + int32_2;
            }
            return strArray;
        }

        /// <summary>从接收的数据内容提取出用户的暗号和数据内容</summary>
        /// <param name="content">数据内容</param>
        /// <returns>包含结果对象的信息</returns>
        public static OperateResult<NetHandle, byte[]> ExtractEstData(
          byte[] content)
        {
            if (content.Length == 0)
                return OperateResult.CreateSuccessResult<NetHandle, byte[]>((NetHandle)0, new byte[0]);
            byte[] head = new byte[32];
            byte[] numArray = new byte[content.Length - 32];
            Array.Copy((Array)content, 0, (Array)head, 0, 32);
            if ((uint)numArray.Length > 0U)
                Array.Copy((Array)content, 32, (Array)numArray, 0, content.Length - 32);
            if (BitConverter.ToInt32(head, 0) == 2010)
                return new OperateResult<NetHandle, byte[]>(Encoding.Unicode.GetString(numArray));
            int int32_1 = BitConverter.ToInt32(head, 0);
            int int32_2 = BitConverter.ToInt32(head, 4);
            byte[] bytes = EstProtocol.CommandAnalysis(head, numArray);
            return int32_1 == 6 ? new OperateResult<NetHandle, byte[]>(Encoding.Unicode.GetString(bytes)) : OperateResult.CreateSuccessResult<NetHandle, byte[]>((NetHandle)int32_2, bytes);
        }
    }
}
