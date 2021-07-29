// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Core.NetSupport
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using System;
using System.Net.Sockets;

namespace ESTCore.Common.Core
{
    /// <summary>
    /// 静态的方法支持类，提供一些网络的静态支持，支持从套接字从同步接收指定长度的字节数据，并支持报告进度。<br />
    /// The static method support class provides some static support for the network, supports receiving byte data of a specified length from the socket from synchronization, and supports reporting progress.
    /// </summary>
    /// <remarks>
    /// 在接收指定数量的字节数据的时候，如果一直接收不到，就会发生假死的状态。接收的数据时保存在内存里的，不适合大数据块的接收。
    /// </remarks>
    public static class NetSupport
    {
        /// <summary>
        /// Socket传输中的缓冲池大小<br />
        /// Buffer pool size in socket transmission
        /// </summary>
        internal const int SocketBufferSize = 16384;

        /// <summary>
        /// 从socket的网络中读取数据内容，需要指定数据长度和超时的时间，为了防止数据太大导致接收失败，所以此处接收到新的数据之后就更新时间。<br />
        /// To read the data content from the socket network, you need to specify the data length and timeout period. In order to prevent the data from being too large and cause the reception to fail, the time is updated after new data is received here.
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="receive">接收的长度</param>
        /// <param name="reportProgress">当前接收数据的进度报告，有些协议支持传输非常大的数据内容，可以给与进度提示的功能</param>
        /// <returns>最终接收的指定长度的byte[]数据</returns>
        public static byte[] ReadBytesFromSocket(
          Socket socket,
          int receive,
          Action<long, long> reportProgress = null)
        {
            byte[] buffer = new byte[receive];
            int offset = 0;
            while (offset < receive)
            {
                int size = Math.Min(receive - offset, 16384);
                int num = socket.Receive(buffer, offset, size, SocketFlags.None);
                offset += num;
                if (num == 0)
                    throw new RemoteCloseException();
                if (reportProgress != null)
                    reportProgress((long)offset, (long)receive);
            }
            return buffer;
        }
    }
}
