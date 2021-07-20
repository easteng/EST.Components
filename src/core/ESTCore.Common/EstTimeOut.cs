// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.EstTimeOut
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Reflection;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace ESTCore.Common
{
    /// <summary>
    /// 超时操作的类<br />
    /// a class use to indicate the time-out of the connection
    /// </summary>
    /// <remarks>本类自动启动一个静态线程来处理</remarks>
    public class EstTimeOut
    {
        private static long hslTimeoutId = 0;
        private static List<EstTimeOut> WaitHandleTimeOut = new List<EstTimeOut>(128);
        private static object listLock = new object();
        private static Thread threadCheckTimeOut;
        private static long threadUniqueId = 0;
        private static DateTime threadActiveTime;
        private static int activeDisableCount = 0;

        /// <summary>
        /// 实例化一个默认的对象<br />
        /// Instantiate a default object
        /// </summary>
        public EstTimeOut()
        {
            this.UniqueId = Interlocked.Increment(ref EstTimeOut.hslTimeoutId);
            this.StartTime = DateTime.Now;
            this.IsSuccessful = false;
            this.IsTimeout = false;
        }

        /// <summary>
        /// 当前超时对象的唯一ID信息，没实例化一个对象，id信息就会自增1<br />
        /// The unique ID information of the current timeout object. If an object is not instantiated, the id information will increase by 1
        /// </summary>
        public long UniqueId { get; private set; }

        /// <summary>
        /// 操作的开始时间<br />
        /// Start time of operation
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 操作是否成功，当操作完成的时候，需要设置为<c>True</c>，超时检测自动结束。如果一直为<c>False</c>，超时检测到超时，设置<see cref="P:ESTCore.Common.EstTimeOut.IsTimeout" />为<c>True</c><br />
        /// Whether the operation is successful, when the operation is completed, it needs to be set to <c>True</c>,
        /// and the timeout detection will automatically end. If it is always <c>False</c>,
        /// the timeout is detected by the timeout, set <see cref="P:ESTCore.Common.EstTimeOut.IsTimeout" /> to <c>True</c>
        /// </summary>
        public bool IsSuccessful { get; set; }

        /// <summary>
        /// 延时的时间，单位毫秒<br />
        /// Delay time, in milliseconds
        /// </summary>
        public int DelayTime { get; set; }

        /// <summary>
        /// 连接超时用的Socket，本超时对象主要针对套接字的连接，接收数据的超时检测，也可以设置为空，用作其他用途的超时检测。<br />
        /// Socket used for connection timeout. This timeout object is mainly for socket connection and timeout detection of received data.
        /// It can also be set to empty for other purposes.
        /// </summary>
        [JsonIgnore]
        public Socket WorkSocket { get; set; }

        /// <summary>
        /// 是否发生了超时的操作，当调用方因为异常结束的时候，需要对<see cref="P:ESTCore.Common.EstTimeOut.IsTimeout" />进行判断，是否因为发送了超时导致的异常<br />
        /// Whether a timeout operation has occurred, when the caller ends abnormally,
        /// it needs to judge <see cref="P:ESTCore.Common.EstTimeOut.IsTimeout" />, whether it is an exception caused by a timeout sent
        /// </summary>
        public bool IsTimeout { get; set; }

        /// <summary>
        /// 获取到目前为止所花费的时间<br />
        /// Get the time spent so far
        /// </summary>
        /// <returns>时间信息</returns>
        public TimeSpan GetConsumeTime() => DateTime.Now - this.StartTime;

        /// <inheritdoc />
        public override string ToString() => string.Format("EstTimeOut[{0}]", (object)this.DelayTime);

        /// <summary>
        /// 新增一个超时检测的对象，当操作完成的时候，需要自行标记<see cref="T:ESTCore.Common.EstTimeOut" />对象的<see cref="P:ESTCore.Common.EstTimeOut.IsSuccessful" />为<c>True</c><br />
        /// Add a new object for timeout detection. When the operation is completed,
        /// you need to mark the <see cref="P:ESTCore.Common.EstTimeOut.IsSuccessful" /> of the <see cref="T:ESTCore.Common.EstTimeOut" /> object as <c>True</c>
        /// </summary>
        /// <param name="timeOut">超时对象</param>
        public static void HandleTimeOutCheck(EstTimeOut timeOut)
        {
            lock (EstTimeOut.listLock)
            {
                if ((DateTime.Now - EstTimeOut.threadActiveTime).TotalSeconds > 60.0)
                {
                    EstTimeOut.threadActiveTime = DateTime.Now;
                    if (Interlocked.Increment(ref EstTimeOut.activeDisableCount) >= 2)
                        EstTimeOut.CreateTimeoutCheckThread();
                }
                EstTimeOut.WaitHandleTimeOut.Add(timeOut);
            }
        }

        /// <summary>
        /// 获取当前检查超时对象的个数<br />
        /// Get the number of current check timeout objects
        /// </summary>
        [EstMqttApi(Description = "Get the number of current check timeout objects", HttpMethod = "GET")]
        public static int TimeOutCheckCount => EstTimeOut.WaitHandleTimeOut.Count;

        /// <summary>
        /// 获取当前的所有的等待超时检查对象列表，请勿手动更改对象的属性值<br />
        /// Get the current list of all waiting timeout check objects, do not manually change the property value of the object
        /// </summary>
        /// <returns>EstTimeOut数组，请勿手动更改对象的属性值</returns>
        [EstMqttApi(Description = "Get the current list of all waiting timeout check objects, do not manually change the property value of the object", HttpMethod = "GET")]
        public static EstTimeOut[] GetEstTimeOutsSnapShoot()
        {
            lock (EstTimeOut.listLock)
                return EstTimeOut.WaitHandleTimeOut.ToArray();
        }

        /// <summary>
        /// 新增一个超时检测的对象，需要指定socket，超时时间，返回<see cref="T:ESTCore.Common.EstTimeOut" />对象，用作标记完成信息<br />
        /// Add a new object for timeout detection, you need to specify the socket, the timeout period,
        /// and return the <see cref="T:ESTCore.Common.EstTimeOut" /> object for marking completion information
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="timeout">超时时间，单位为毫秒<br />Timeout period, in milliseconds</param>
        public static EstTimeOut HandleTimeOutCheck(Socket socket, int timeout)
        {
            EstTimeOut timeOut = new EstTimeOut()
            {
                DelayTime = timeout,
                IsSuccessful = false,
                StartTime = DateTime.Now,
                WorkSocket = socket
            };
            if (timeout > 0)
                EstTimeOut.HandleTimeOutCheck(timeOut);
            return timeOut;
        }

        static EstTimeOut() => EstTimeOut.CreateTimeoutCheckThread();

        private static void CreateTimeoutCheckThread()
        {
            EstTimeOut.threadActiveTime = DateTime.Now;
            EstTimeOut.threadCheckTimeOut?.Abort();
            EstTimeOut.threadCheckTimeOut = new Thread(new ParameterizedThreadStart(EstTimeOut.CheckTimeOut));
            EstTimeOut.threadCheckTimeOut.IsBackground = true;
            EstTimeOut.threadCheckTimeOut.Priority = ThreadPriority.AboveNormal;
            EstTimeOut.threadCheckTimeOut.Start((object)Interlocked.Increment(ref EstTimeOut.threadUniqueId));
        }

        /// <summary>
        /// 整个ESTCore.Common的检测超时的核心方法，由一个单独的线程运行，线程的优先级很高，当前其他所有的超时信息都可以放到这里处理<br />
        /// The core method of detecting the timeout of th e entire ESTCore.Common is run by a separate thread.
        /// The priority of the thread is very high. All other timeout information can be processed here.
        /// </summary>
        /// <param name="obj">需要传入线程的id信息</param>
        private static void CheckTimeOut(object obj)
        {
            long num = (long)obj;
        label_17:
            Thread.Sleep(100);
            if (num != EstTimeOut.threadUniqueId)
                return;
            EstTimeOut.threadActiveTime = DateTime.Now;
            EstTimeOut.activeDisableCount = 0;
            lock (EstTimeOut.listLock)
            {
                for (int index = EstTimeOut.WaitHandleTimeOut.Count - 1; index >= 0; --index)
                {
                    EstTimeOut hslTimeOut = EstTimeOut.WaitHandleTimeOut[index];
                    if (hslTimeOut.IsSuccessful)
                        EstTimeOut.WaitHandleTimeOut.RemoveAt(index);
                    else if ((DateTime.Now - hslTimeOut.StartTime).TotalMilliseconds > (double)hslTimeOut.DelayTime)
                    {
                        if (!hslTimeOut.IsSuccessful)
                        {
                            hslTimeOut.WorkSocket?.Close();
                            hslTimeOut.IsTimeout = true;
                        }
                        EstTimeOut.WaitHandleTimeOut.RemoveAt(index);
                    }
                }
                goto label_17;
            }
        }
    }
}
