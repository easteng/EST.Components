// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Core.Net.NetworkBase
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;
using ESTCore.Common.Core.IMessage;
using ESTCore.Common.Enthernet.Redis;
using ESTCore.Common.LogNet;
using ESTCore.Common.MQTT;
using ESTCore.Common.WebSocket;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESTCore.Common.Core.Net
{
    /// <summary>
    /// 本系统所有网络类的基类，该类为抽象类，无法进行实例化，如果想使用里面的方法来实现自定义的网络通信，请通过继承使用。<br />
    /// The base class of all network classes in this system. This class is an abstract class and cannot be instantiated.
    /// If you want to use the methods inside to implement custom network communication, please use it through inheritance.
    /// </summary>
    /// <remarks>
    /// 本类提供了丰富的底层数据的收发支持，包含<see cref="T:ESTCore.Common.Core.IMessage.INetMessage" />消息的接收，<c>MQTT</c>以及<c>Redis</c>,<c>websocket</c>协议的实现
    /// </remarks>
    public abstract class NetworkBase
    {
        /// <summary>
        /// 对客户端而言是的通讯用的套接字，对服务器来说是用于侦听的套接字<br />
        /// A communication socket for the client, or a listening socket for the server
        /// </summary>
        protected Socket CoreSocket = (Socket)null;
        /// <summary>
        /// 文件传输的时候的缓存大小，直接影响传输的速度，值越大，传输速度越快，越占内存，默认为100K大小<br />
        /// The size of the cache during file transfer directly affects the speed of the transfer. The larger the value, the faster the transfer speed and the more memory it takes. The default size is 100K.
        /// </summary>
        protected int fileCacheSize = 102400;
        private int connectErrorCount = 0;

        /// <summary>
        /// 实例化一个NetworkBase对象，令牌的默认值为空，都是0x00<br />
        /// Instantiate a NetworkBase object, the default value of the token is empty, both are 0x00
        /// </summary>
        public NetworkBase()
        {
            this.Token = Guid.Empty;
            ESTCore.Common.Authorization.oasjodaiwfsodopsdjpasjpf();
        }

        /// <summary>
        /// 组件的日志工具，支持日志记录，只要实例化后，当前网络的基本信息，就以<see cref="F:ESTCore.Common.LogNet.EstMessageDegree.DEBUG" />等级进行输出<br />
        /// The component's logging tool supports logging. As long as the instantiation of the basic network information, the output will be output at <see cref="F:ESTCore.Common.LogNet.EstMessageDegree.DEBUG" />
        /// </summary>
        /// <remarks>
        /// 只要实例化即可以记录日志，实例化的对象需要实现接口 <see cref="T:ESTCore.Common.LogNet.ILogNet" /> ，本组件提供了三个日志记录类，你可以实现基于 <see cref="T:ESTCore.Common.LogNet.ILogNet" />  的对象。</remarks>
        /// <example>
        /// 如下的实例化适用于所有的Network及其派生类，以下举两个例子，三菱的设备类及服务器类
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Core\NetworkBase.cs" region="LogNetExample1" title="LogNet示例" />
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Core\NetworkBase.cs" region="LogNetExample2" title="LogNet示例" />
        /// </example>
        public ILogNet LogNet { get; set; }

        /// <summary>
        /// 网络类的身份令牌，在hsl协议的模式下会有效，在和设备进行通信的时候是无效的<br />
        /// Network-type identity tokens will be valid in the hsl protocol mode and will not be valid when communicating with the device
        /// </summary>
        /// <remarks>适用于Est协议相关的网络通信类，不适用于设备交互类。</remarks>
        /// <example>
        /// 此处以 <see cref="T:ESTCore.Common.Enthernet.NetSimplifyServer" /> 服务器类及 <see cref="T:ESTCore.Common.Enthernet.NetSimplifyClient" /> 客户端类的令牌设置举例
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Core\NetworkBase.cs" region="TokenClientExample" title="Client示例" />
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Core\NetworkBase.cs" region="TokenServerExample" title="Server示例" />
        /// </example>
        public Guid Token { get; set; }

        /// <summary>
        /// 接收固定长度的字节数组，允许指定超时时间，默认为60秒，当length大于0时，接收固定长度的数据内容，当length小于0时，接收不大于2048长度的随机数据信息<br />
        /// Receiving a fixed-length byte array, allowing a specified timeout time. The default is 60 seconds. When length is greater than 0,
        /// fixed-length data content is received. When length is less than 0, random data information of a length not greater than 2048 is received.
        /// </summary>
        /// <param name="socket">网络通讯的套接字<br />Network communication socket</param>
        /// <param name="length">准备接收的数据长度，当length大于0时，接收固定长度的数据内容，当length小于0时，接收不大于1024长度的随机数据信息</param>
        /// <param name="timeOut">单位：毫秒，超时时间，默认为60秒，如果设置小于0，则不检查超时时间</param>
        /// <param name="reportProgress">当前接收数据的进度报告，有些协议支持传输非常大的数据内容，可以给与进度提示的功能</param>
        /// <returns>包含了字节数据的结果类</returns>
        protected OperateResult<byte[]> Receive(
          Socket socket,
          int length,
          int timeOut = 60000,
          Action<long, long> reportProgress = null)
        {
            if (length == 0)
                return OperateResult.CreateSuccessResult<byte[]>(new byte[0]);
            if (!ESTCore.Common.Authorization.nzugaydgwadawdibbas())
                return new OperateResult<byte[]>(StringResources.Language.AuthorizationFailed);
            try
            {
                socket.ReceiveTimeout = timeOut;
                if (length > 0)
                    return OperateResult.CreateSuccessResult<byte[]>(NetSupport.ReadBytesFromSocket(socket, length, reportProgress));
                byte[] buffer = new byte[2048];
                int length1 = socket.Receive(buffer);
                return length1 != 0 ? OperateResult.CreateSuccessResult<byte[]>(SoftBasic.ArraySelectBegin<byte>(buffer, length1)) : throw new RemoteCloseException();
            }
            catch (RemoteCloseException ex)
            {
                socket?.Close();
                if (this.connectErrorCount < 1000000000)
                    ++this.connectErrorCount;
                return new OperateResult<byte[]>(-this.connectErrorCount, "Socket Exception -> " + StringResources.Language.RemoteClosedConnection);
            }
            catch (Exception ex)
            {
                socket?.Close();
                if (this.connectErrorCount < 1000000000)
                    ++this.connectErrorCount;
                return new OperateResult<byte[]>(-this.connectErrorCount, "Socket Exception -> " + ex.Message);
            }
        }

        /// <summary>
        /// 接收一行命令数据，需要自己指定这个结束符，默认超时时间为60秒，也即是60000，单位是毫秒<br />
        /// To receive a line of command data, you need to specify the terminator yourself. The default timeout is 60 seconds, which is 60,000, in milliseconds.
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="endCode">结束符信息</param>
        /// <param name="timeout">超时时间，默认为60000，单位为毫秒，也就是60秒</param>
        /// <returns>带有结果对象的数据信息</returns>
        protected OperateResult<byte[]> ReceiveCommandLineFromSocket(
          Socket socket,
          byte endCode,
          int timeout = 2147483647)
        {
            List<byte> byteList = new List<byte>();
            try
            {
                DateTime now = DateTime.Now;
                bool flag = false;
                while ((DateTime.Now - now).TotalMilliseconds < (double)timeout)
                {
                    if (socket.Poll(timeout, SelectMode.SelectRead))
                    {
                        OperateResult<byte[]> operateResult = this.Receive(socket, 1);
                        if (!operateResult.IsSuccess)
                            return operateResult;
                        byteList.AddRange((IEnumerable<byte>)operateResult.Content);
                        if ((int)operateResult.Content[0] == (int)endCode)
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                return !flag ? new OperateResult<byte[]>(StringResources.Language.ReceiveDataTimeout) : OperateResult.CreateSuccessResult<byte[]>(byteList.ToArray());
            }
            catch (Exception ex)
            {
                socket?.Close();
                return new OperateResult<byte[]>(ex.Message);
            }
        }

        /// <summary>
        /// 接收一行命令数据，需要自己指定这个结束符，默认超时时间为60秒，也即是60000，单位是毫秒<br />
        /// To receive a line of command data, you need to specify the terminator yourself. The default timeout is 60 seconds, which is 60,000, in milliseconds.
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="endCode1">结束符1信息</param>
        /// <param name="endCode2">结束符2信息</param>
        /// 
        ///             /// <param name="timeout">超时时间，默认无穷大，单位毫秒</param>
        /// <returns>带有结果对象的数据信息</returns>
        protected OperateResult<byte[]> ReceiveCommandLineFromSocket(
          Socket socket,
          byte endCode1,
          byte endCode2,
          int timeout = 60000)
        {
            List<byte> byteList = new List<byte>();
            try
            {
                DateTime now = DateTime.Now;
                bool flag = false;
                while ((DateTime.Now - now).TotalMilliseconds < (double)timeout)
                {
                    if (socket.Poll(timeout, SelectMode.SelectRead))
                    {
                        OperateResult<byte[]> operateResult = this.Receive(socket, 1);
                        if (!operateResult.IsSuccess)
                            return operateResult;
                        byteList.AddRange((IEnumerable<byte>)operateResult.Content);
                        if ((int)operateResult.Content[0] == (int)endCode2 && (byteList.Count > 1 && (int)byteList[byteList.Count - 2] == (int)endCode1))
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                return !flag ? new OperateResult<byte[]>(StringResources.Language.ReceiveDataTimeout) : OperateResult.CreateSuccessResult<byte[]>(byteList.ToArray());
            }
            catch (Exception ex)
            {
                socket?.Close();
                return new OperateResult<byte[]>(ex.Message);
            }
        }

        /// <summary>
        /// 接收一条完整的 <seealso cref="T:ESTCore.Common.Core.IMessage.INetMessage" /> 数据内容，需要指定超时时间，单位为毫秒。 <br />
        /// Receive a complete <seealso cref="T:ESTCore.Common.Core.IMessage.INetMessage" /> data content, Need to specify a timeout period in milliseconds
        /// </summary>
        /// <param name="socket">网络的套接字</param>
        /// <param name="timeOut">超时时间，单位：毫秒</param>
        /// <param name="netMessage">消息的格式定义</param>
        /// <param name="reportProgress">接收消息的时候的进度报告</param>
        /// <returns>带有是否成功的byte数组对象</returns>
        protected OperateResult<byte[]> ReceiveByMessage(
          Socket socket,
          int timeOut,
          INetMessage netMessage,
          Action<long, long> reportProgress = null)
        {
            if (netMessage == null)
                return this.Receive(socket, -1, timeOut);
            OperateResult<byte[]> operateResult1 = this.Receive(socket, netMessage.ProtocolHeadBytesLength, timeOut);
            if (!operateResult1.IsSuccess)
                return operateResult1;
            netMessage.HeadBytes = operateResult1.Content;
            int lengthByHeadBytes = netMessage.GetContentLengthByHeadBytes();
            OperateResult<byte[]> operateResult2 = this.Receive(socket, lengthByHeadBytes, timeOut, reportProgress);
            if (!operateResult2.IsSuccess)
                return operateResult2;
            netMessage.ContentBytes = operateResult2.Content;
            return OperateResult.CreateSuccessResult<byte[]>(SoftBasic.SpliceArray<byte>(operateResult1.Content, operateResult2.Content));
        }

        /// <summary>
        /// 发送消息给套接字，直到完成的时候返回，经过测试，本方法是线程安全的。<br />
        /// Send a message to the socket until it returns when completed. After testing, this method is thread-safe.
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="data">字节数据</param>
        /// <returns>发送是否成功的结果</returns>
        protected OperateResult Send(Socket socket, byte[] data) => data == null ? OperateResult.CreateSuccessResult() : this.Send(socket, data, 0, data.Length);

        /// <summary>
        /// 发送消息给套接字，直到完成的时候返回，经过测试，本方法是线程安全的。<br />
        /// Send a message to the socket until it returns when completed. After testing, this method is thread-safe.
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="data">字节数据</param>
        /// <param name="offset">偏移的位置信息</param>
        /// <param name="size">发送的数据总数</param>
        /// <returns>发送是否成功的结果</returns>
        protected OperateResult Send(Socket socket, byte[] data, int offset, int size)
        {
            if (data == null)
                return OperateResult.CreateSuccessResult();
            try
            {
                int num1 = 0;
                do
                {
                    int num2 = (int)(socket?.Send(data, offset, size - num1, SocketFlags.None));
                    num1 += num2;
                    offset += num2;
                }
                while (num1 < size);
                return OperateResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                socket?.Close();
                if (this.connectErrorCount < 1000000000)
                    ++this.connectErrorCount;
                return (OperateResult)new OperateResult<byte[]>(-this.connectErrorCount, ex.Message);
            }
        }

        /// <summary>
        /// 创建一个新的socket对象并连接到远程的地址，默认超时时间为10秒钟，需要指定ip地址以及端口号信息<br />
        /// Create a new socket object and connect to the remote address. The default timeout is 10 seconds. You need to specify the IP address and port number.
        /// </summary>
        /// <param name="ipAddress">Ip地址</param>
        /// <param name="port">端口号</param>
        /// <returns>返回套接字的封装结果对象</returns>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Core\NetworkBase.cs" region="CreateSocketAndConnectExample" title="创建连接示例" />
        /// </example>
        protected OperateResult<Socket> CreateSocketAndConnect(
          string ipAddress,
          int port)
        {
            return this.CreateSocketAndConnect(new IPEndPoint(IPAddress.Parse(ipAddress), port), 10000);
        }

        /// <summary>
        /// 创建一个新的socket对象并连接到远程的地址，需要指定ip地址以及端口号信息，还有超时时间，单位是毫秒<br />
        /// To create a new socket object and connect to a remote address, you need to specify the IP address and port number information, and the timeout period in milliseconds
        /// </summary>
        /// <param name="ipAddress">Ip地址</param>
        /// <param name="port">端口号</param>
        /// <param name="timeOut">连接的超时时间</param>
        /// <returns>返回套接字的封装结果对象</returns>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Core\NetworkBase.cs" region="CreateSocketAndConnectExample" title="创建连接示例" />
        /// </example>
        protected OperateResult<Socket> CreateSocketAndConnect(
          string ipAddress,
          int port,
          int timeOut)
        {
            return this.CreateSocketAndConnect(new IPEndPoint(IPAddress.Parse(ipAddress), port), timeOut);
        }

        /// <summary>
        /// 创建一个新的socket对象并连接到远程的地址，需要指定远程终结点，超时时间（单位是毫秒），如果需要绑定本地的IP或是端口，传入 local对象<br />
        /// To create a new socket object and connect to the remote address, you need to specify the remote endpoint,
        /// the timeout period (in milliseconds), if you need to bind the local IP or port, pass in the local object
        /// </summary>
        /// <param name="endPoint">连接的目标终结点</param>
        /// <param name="timeOut">连接的超时时间</param>
        /// <param name="local">如果需要绑定本地的IP地址，就需要设置当前的对象</param>
        /// <returns>返回套接字的封装结果对象</returns>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Core\NetworkBase.cs" region="CreateSocketAndConnectExample" title="创建连接示例" />
        /// </example>
        protected OperateResult<Socket> CreateSocketAndConnect(
          IPEndPoint endPoint,
          int timeOut,
          IPEndPoint local = null)
        {
            int num = 0;
            while (true)
            {
                ++num;
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                EstTimeOut hslTimeOut = EstTimeOut.HandleTimeOutCheck(socket, timeOut);
                try
                {
                    if (local != null)
                        socket.Bind((EndPoint)local);
                    socket.Connect((EndPoint)endPoint);
                    this.connectErrorCount = 0;
                    hslTimeOut.IsSuccessful = true;
                    return OperateResult.CreateSuccessResult<Socket>(socket);
                }
                catch (Exception ex)
                {
                    socket?.Close();
                    hslTimeOut.IsSuccessful = true;
                    if (this.connectErrorCount < 1000000000)
                        ++this.connectErrorCount;
                    if (hslTimeOut.GetConsumeTime() < TimeSpan.FromMilliseconds(500.0) && num < 2)
                        Thread.Sleep(100);
                    else
                        return hslTimeOut.IsTimeout ? new OperateResult<Socket>(-this.connectErrorCount, string.Format(StringResources.Language.ConnectTimeout, (object)endPoint, (object)timeOut) + " ms") : new OperateResult<Socket>(-this.connectErrorCount, string.Format("Socket Connect {0} Exception -> ", (object)endPoint) + ex.Message);
                }
            }
        }

        /// <summary>
        /// 读取流中的数据到缓存区，读取的长度需要按照实际的情况来判断<br />
        /// Read the data in the stream to the buffer area. The length of the read needs to be determined according to the actual situation.
        /// </summary>
        /// <param name="stream">数据流</param>
        /// <param name="buffer">缓冲区</param>
        /// <returns>带有成功标志的读取数据长度</returns>
        protected OperateResult<int> ReadStream(Stream stream, byte[] buffer)
        {
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            FileStateObject fileStateObject1 = new FileStateObject();
            fileStateObject1.WaitDone = manualResetEvent;
            fileStateObject1.Stream = stream;
            fileStateObject1.DataLength = buffer.Length;
            fileStateObject1.Buffer = buffer;
            FileStateObject fileStateObject2 = fileStateObject1;
            try
            {
                stream.BeginRead(buffer, 0, fileStateObject2.DataLength, new AsyncCallback(this.ReadStreamCallBack), (object)fileStateObject2);
            }
            catch (Exception ex)
            {
                manualResetEvent.Close();
                return new OperateResult<int>("stream.BeginRead Exception -> " + ex.Message);
            }
            manualResetEvent.WaitOne();
            manualResetEvent.Close();
            return fileStateObject2.IsError ? new OperateResult<int>(fileStateObject2.ErrerMsg) : OperateResult.CreateSuccessResult<int>(fileStateObject2.AlreadyDealLength);
        }

        private void ReadStreamCallBack(IAsyncResult ar)
        {
            if (!(ar.AsyncState is FileStateObject asyncState))
                return;
            try
            {
                asyncState.AlreadyDealLength += asyncState.Stream.EndRead(ar);
                asyncState.WaitDone.Set();
            }
            catch (Exception ex)
            {
                asyncState.IsError = true;
                asyncState.ErrerMsg = ex.Message;
                asyncState.WaitDone.Set();
            }
        }

        /// <summary>
        /// 将缓冲区的数据写入到流里面去<br />
        /// Write the buffer data to the stream
        /// </summary>
        /// <param name="stream">数据流</param>
        /// <param name="buffer">缓冲区</param>
        /// <returns>是否写入成功</returns>
        protected OperateResult WriteStream(Stream stream, byte[] buffer)
        {
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            FileStateObject fileStateObject1 = new FileStateObject();
            fileStateObject1.WaitDone = manualResetEvent;
            fileStateObject1.Stream = stream;
            FileStateObject fileStateObject2 = fileStateObject1;
            try
            {
                stream.BeginWrite(buffer, 0, buffer.Length, new AsyncCallback(this.WriteStreamCallBack), (object)fileStateObject2);
            }
            catch (Exception ex)
            {
                manualResetEvent.Close();
                return new OperateResult("stream.BeginWrite Exception -> " + ex.Message);
            }
            manualResetEvent.WaitOne();
            manualResetEvent.Close();
            if (!fileStateObject2.IsError)
                return OperateResult.CreateSuccessResult();
            return new OperateResult()
            {
                Message = fileStateObject2.ErrerMsg
            };
        }

        private void WriteStreamCallBack(IAsyncResult ar)
        {
            if (!(ar.AsyncState is FileStateObject asyncState))
                return;
            try
            {
                asyncState.Stream.EndWrite(ar);
            }
            catch (Exception ex)
            {
                asyncState.IsError = true;
                asyncState.ErrerMsg = ex.Message;
            }
            finally
            {
                asyncState.WaitDone.Set();
            }
        }

        /// <summary>
        /// 检查当前的头子节信息的令牌是否是正确的，仅用于某些特殊的协议实现<br />
        /// Check whether the token of the current header subsection information is correct, only for some special protocol implementations
        /// </summary>
        /// <param name="headBytes">头子节数据</param>
        /// <returns>令牌是验证成功</returns>
        protected bool CheckRemoteToken(byte[] headBytes) => SoftBasic.IsByteTokenEquel(headBytes, this.Token);

        /// <summary>
        /// [自校验] 发送字节数据并确认对方接收完成数据，如果结果异常，则结束通讯<br />
        /// [Self-check] Send the byte data and confirm that the other party has received the completed data. If the result is abnormal, the communication ends.
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="headCode">头指令</param>
        /// <param name="customer">用户指令</param>
        /// <param name="send">发送的数据</param>
        /// <returns>是否发送成功</returns>
        protected OperateResult SendBaseAndCheckReceive(
          Socket socket,
          int headCode,
          int customer,
          byte[] send)
        {
            send = EstProtocol.CommandBytes(headCode, customer, this.Token, send);
            OperateResult operateResult1 = this.Send(socket, send);
            if (!operateResult1.IsSuccess)
                return operateResult1;
            OperateResult<long> operateResult2 = this.ReceiveLong(socket);
            if (!operateResult2.IsSuccess || operateResult2.Content == (long)send.Length)
                return (OperateResult)operateResult2;
            socket?.Close();
            return new OperateResult(StringResources.Language.CommandLengthCheckFailed);
        }

        /// <summary>
        /// [自校验] 发送字节数据并确认对方接收完成数据，如果结果异常，则结束通讯<br />
        /// [Self-check] Send the byte data and confirm that the other party has received the completed data. If the result is abnormal, the communication ends.
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="customer">用户指令</param>
        /// <param name="send">发送的数据</param>
        /// <returns>是否发送成功</returns>
        protected OperateResult SendBytesAndCheckReceive(
          Socket socket,
          int customer,
          byte[] send)
        {
            return this.SendBaseAndCheckReceive(socket, 1002, customer, send);
        }

        /// <summary>
        /// [自校验] 直接发送字符串数据并确认对方接收完成数据，如果结果异常，则结束通讯<br />
        /// [Self-checking] Send string data directly and confirm that the other party has received the completed data. If the result is abnormal, the communication ends.
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="customer">用户指令</param>
        /// <param name="send">发送的数据</param>
        /// <returns>是否发送成功</returns>
        protected OperateResult SendStringAndCheckReceive(
          Socket socket,
          int customer,
          string send)
        {
            byte[] send1 = string.IsNullOrEmpty(send) ? (byte[])null : Encoding.Unicode.GetBytes(send);
            return this.SendBaseAndCheckReceive(socket, 1001, customer, send1);
        }

        /// <summary>
        /// [自校验] 直接发送字符串数组并确认对方接收完成数据，如果结果异常，则结束通讯<br />
        /// [Self-check] Send string array directly and confirm that the other party has received the completed data. If the result is abnormal, the communication ends.
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="customer">用户指令</param>
        /// <param name="sends">发送的字符串数组</param>
        /// <returns>是否发送成功</returns>
        protected OperateResult SendStringAndCheckReceive(
          Socket socket,
          int customer,
          string[] sends)
        {
            return this.SendBaseAndCheckReceive(socket, 1005, customer, EstProtocol.PackStringArrayToByte(sends));
        }

        /// <summary>
        /// [自校验] 直接发送字符串数组并确认对方接收完成数据，如果结果异常，则结束通讯<br />
        /// [Self-check] Send string array directly and confirm that the other party has received the completed data. If the result is abnormal, the communication ends.
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="customer">用户指令</param>
        /// <param name="name">用户名</param>
        /// <param name="pwd">密码</param>
        /// <returns>是否发送成功</returns>
        protected OperateResult SendAccountAndCheckReceive(
          Socket socket,
          int customer,
          string name,
          string pwd)
        {
            return this.SendBaseAndCheckReceive(socket, 5, customer, EstProtocol.PackStringArrayToByte(new string[2]
            {
        name,
        pwd
            }));
        }

        /// <summary>
        /// [自校验] 接收一条完整的同步数据，包含头子节和内容字节，基础的数据，如果结果异常，则结束通讯<br />
        /// [Self-checking] Receive a complete synchronization data, including header subsection and content bytes, basic data, if the result is abnormal, the communication ends
        /// </summary>
        /// <param name="socket">套接字</param>
        /// <param name="timeOut">超时时间设置，如果为负数，则不检查超时</param>
        /// <returns>包含是否成功的结果对象</returns>
        /// <exception cref="T:System.ArgumentNullException">result</exception>
        protected OperateResult<byte[], byte[]> ReceiveAndCheckBytes(
          Socket socket,
          int timeOut)
        {
            OperateResult<byte[]> operateResult1 = this.Receive(socket, 32, timeOut);
            if (!operateResult1.IsSuccess)
                return operateResult1.ConvertFailed<byte[], byte[]>();
            if (!this.CheckRemoteToken(operateResult1.Content))
            {
                socket?.Close();
                return new OperateResult<byte[], byte[]>(StringResources.Language.TokenCheckFailed);
            }
            int int32 = BitConverter.ToInt32(operateResult1.Content, 28);
            OperateResult<byte[]> operateResult2 = this.Receive(socket, int32, timeOut);
            if (!operateResult2.IsSuccess)
                return operateResult2.ConvertFailed<byte[], byte[]>();
            OperateResult operateResult3 = this.SendLong(socket, (long)(32 + int32));
            if (!operateResult3.IsSuccess)
                return operateResult3.ConvertFailed<byte[], byte[]>();
            byte[] content1 = operateResult1.Content;
            byte[] content2 = operateResult2.Content;
            byte[] numArray = EstProtocol.CommandAnalysis(content1, content2);
            return OperateResult.CreateSuccessResult<byte[], byte[]>(content1, numArray);
        }

        /// <summary>
        /// [自校验] 从网络中接收一个字符串数据，如果结果异常，则结束通讯<br />
        /// [Self-checking] Receive a string of data from the network. If the result is abnormal, the communication ends.
        /// </summary>
        /// <param name="socket">套接字</param>
        /// <param name="timeOut">接收数据的超时时间</param>
        /// <returns>包含是否成功的结果对象</returns>
        protected OperateResult<int, string> ReceiveStringContentFromSocket(
          Socket socket,
          int timeOut = 30000)
        {
            OperateResult<byte[], byte[]> andCheckBytes = this.ReceiveAndCheckBytes(socket, timeOut);
            if (!andCheckBytes.IsSuccess)
                return OperateResult.CreateFailedResult<int, string>((OperateResult)andCheckBytes);
            if (BitConverter.ToInt32(andCheckBytes.Content1, 0) != 1001)
            {
                socket?.Close();
                return new OperateResult<int, string>(StringResources.Language.CommandHeadCodeCheckFailed);
            }
            if (andCheckBytes.Content2 == null)
                andCheckBytes.Content2 = new byte[0];
            return OperateResult.CreateSuccessResult<int, string>(BitConverter.ToInt32(andCheckBytes.Content1, 4), Encoding.Unicode.GetString(andCheckBytes.Content2));
        }

        /// <summary>
        /// [自校验] 从网络中接收一个字符串数组，如果结果异常，则结束通讯<br />
        /// [Self-check] Receive an array of strings from the network. If the result is abnormal, the communication ends.
        /// </summary>
        /// <param name="socket">套接字</param>
        /// <param name="timeOut">接收数据的超时时间</param>
        /// <returns>包含是否成功的结果对象</returns>
        protected OperateResult<int, string[]> ReceiveStringArrayContentFromSocket(
          Socket socket,
          int timeOut = 30000)
        {
            OperateResult<byte[], byte[]> andCheckBytes = this.ReceiveAndCheckBytes(socket, timeOut);
            if (!andCheckBytes.IsSuccess)
                return OperateResult.CreateFailedResult<int, string[]>((OperateResult)andCheckBytes);
            if (BitConverter.ToInt32(andCheckBytes.Content1, 0) != 1005)
            {
                socket?.Close();
                return new OperateResult<int, string[]>(StringResources.Language.CommandHeadCodeCheckFailed);
            }
            if (andCheckBytes.Content2 == null)
                andCheckBytes.Content2 = new byte[4];
            return OperateResult.CreateSuccessResult<int, string[]>(BitConverter.ToInt32(andCheckBytes.Content1, 4), EstProtocol.UnPackStringArrayFromByte(andCheckBytes.Content2));
        }

        /// <summary>
        /// [自校验] 从网络中接收一串字节数据，如果结果异常，则结束通讯<br />
        /// [Self-checking] Receive a string of byte data from the network. If the result is abnormal, the communication ends.
        /// </summary>
        /// <param name="socket">套接字的网络</param>
        /// <param name="timeout">超时时间</param>
        /// <returns>包含是否成功的结果对象</returns>
        protected OperateResult<int, byte[]> ReceiveBytesContentFromSocket(
          Socket socket,
          int timeout = 30000)
        {
            OperateResult<byte[], byte[]> andCheckBytes = this.ReceiveAndCheckBytes(socket, timeout);
            if (!andCheckBytes.IsSuccess)
                return OperateResult.CreateFailedResult<int, byte[]>((OperateResult)andCheckBytes);
            if (BitConverter.ToInt32(andCheckBytes.Content1, 0) == 1002)
                return OperateResult.CreateSuccessResult<int, byte[]>(BitConverter.ToInt32(andCheckBytes.Content1, 4), andCheckBytes.Content2);
            socket?.Close();
            return new OperateResult<int, byte[]>(StringResources.Language.CommandHeadCodeCheckFailed);
        }

        /// <summary>
        /// 从网络中接收Long数据<br />
        /// Receive Long data from the network
        /// </summary>
        /// <param name="socket">套接字网络</param>
        /// <returns>long数据结果</returns>
        private OperateResult<long> ReceiveLong(Socket socket)
        {
            OperateResult<byte[]> operateResult = this.Receive(socket, 8, -1);
            return operateResult.IsSuccess ? OperateResult.CreateSuccessResult<long>(BitConverter.ToInt64(operateResult.Content, 0)) : OperateResult.CreateFailedResult<long>((OperateResult)operateResult);
        }

        /// <summary>
        /// 将long数据发送到套接字<br />
        /// Send long data to the socket
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="value">long数据</param>
        /// <returns>是否发送成功</returns>
        private OperateResult SendLong(Socket socket, long value) => this.Send(socket, BitConverter.GetBytes(value));

        /// <summary>
        /// 发送一个流的所有数据到指定的网络套接字，需要指定发送的数据长度，支持按照百分比的进度报告<br />
        /// Send all the data of a stream to the specified network socket. You need to specify the length of the data to be sent. It supports the progress report in percentage.
        /// </summary>
        /// <param name="socket">套接字</param>
        /// <param name="stream">内存流</param>
        /// <param name="receive">发送的数据长度</param>
        /// <param name="report">进度报告的委托</param>
        /// <param name="reportByPercent">进度报告是否按照百分比报告</param>
        /// <returns>是否成功的结果对象</returns>
        protected OperateResult SendStreamToSocket(
          Socket socket,
          Stream stream,
          long receive,
          Action<long, long> report,
          bool reportByPercent)
        {
            byte[] buffer = new byte[this.fileCacheSize];
            long num1 = 0;
            long num2 = 0;
            stream.Position = 0L;
            while (num1 < receive)
            {
                OperateResult<int> operateResult1 = this.ReadStream(stream, buffer);
                if (!operateResult1.IsSuccess)
                {
                    socket?.Close();
                    return (OperateResult)operateResult1;
                }
                num1 += (long)operateResult1.Content;
                byte[] send = new byte[operateResult1.Content];
                Array.Copy((Array)buffer, 0, (Array)send, 0, send.Length);
                OperateResult operateResult2 = this.SendBytesAndCheckReceive(socket, operateResult1.Content, send);
                if (!operateResult2.IsSuccess)
                {
                    socket?.Close();
                    return operateResult2;
                }
                if (reportByPercent)
                {
                    long num3 = num1 * 100L / receive;
                    if (num2 != num3)
                    {
                        num2 = num3;
                        if (report != null)
                            report(num1, receive);
                    }
                }
                else if (report != null)
                    report(num1, receive);
            }
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 从套接字中接收所有的数据然后写入到指定的流当中去，需要指定数据的长度，支持按照百分比进行进度报告<br />
        /// Receives all data from the socket and writes it to the specified stream. The length of the data needs to be specified, and progress reporting is supported in percentage.
        /// </summary>
        /// <param name="socket">套接字</param>
        /// <param name="stream">数据流</param>
        /// <param name="totalLength">所有数据的长度</param>
        /// <param name="report">进度报告</param>
        /// <param name="reportByPercent">进度报告是否按照百分比</param>
        /// <returns>是否成功的结果对象</returns>
        protected OperateResult WriteStreamFromSocket(
          Socket socket,
          Stream stream,
          long totalLength,
          Action<long, long> report,
          bool reportByPercent)
        {
            long num1 = 0;
            long num2 = 0;
            while (num1 < totalLength)
            {
                OperateResult<int, byte[]> contentFromSocket = this.ReceiveBytesContentFromSocket(socket, 60000);
                if (!contentFromSocket.IsSuccess)
                    return (OperateResult)contentFromSocket;
                num1 += (long)contentFromSocket.Content1;
                OperateResult operateResult = this.WriteStream(stream, contentFromSocket.Content2);
                if (!operateResult.IsSuccess)
                {
                    socket?.Close();
                    return operateResult;
                }
                if (reportByPercent)
                {
                    long num3 = num1 * 100L / totalLength;
                    if (num2 != num3)
                    {
                        num2 = num3;
                        if (report != null)
                            report(num1, totalLength);
                    }
                }
                else if (report != null)
                    report(num1, totalLength);
            }
            return OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkBase.CreateSocketAndConnect(System.Net.IPEndPoint,System.Int32,System.Net.IPEndPoint)" />
        protected async Task<OperateResult<Socket>> CreateSocketAndConnectAsync(
          IPEndPoint endPoint,
          int timeOut,
          IPEndPoint local = null)
        {
            int connectCount = 0;
            EstTimeOut connectTimeout;
            Exception ex;
            while (true)
            {
                ++connectCount;
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                connectTimeout = EstTimeOut.HandleTimeOutCheck(socket, timeOut);
                int num = 0;
                object obj;
                try
                {
                    if (local != null)
                        socket.Bind((EndPoint)local);
                    await Task.Factory.FromAsync(socket.BeginConnect((EndPoint)endPoint, (AsyncCallback)null, (object)socket), new Action<IAsyncResult>(socket.EndConnect));
                    this.connectErrorCount = 0;
                    connectTimeout.IsSuccessful = true;
                    return OperateResult.CreateSuccessResult<Socket>(socket);
                }
                catch (Exception ex1)
                {
                    obj = (object)ex1;
                    num = 1;
                }
                if (num == 1)
                {
                    ex = (Exception)obj;
                    connectTimeout.IsSuccessful = true;
                    socket?.Close();
                    if (this.connectErrorCount < 1000000000)
                        ++this.connectErrorCount;
                    if (connectTimeout.GetConsumeTime() < TimeSpan.FromMilliseconds(500.0) && connectCount < 2)
                        await Task.Delay(100);
                    else
                        break;
                }
                else
                {
                    obj = (object)null;
                    socket = (Socket)null;
                    connectTimeout = (EstTimeOut)null;
                }
            }
            return !connectTimeout.IsTimeout ? new OperateResult<Socket>(-this.connectErrorCount, "Socket Exception -> " + ex.Message) : new OperateResult<Socket>(-this.connectErrorCount, string.Format(StringResources.Language.ConnectTimeout, (object)endPoint, (object)timeOut) + " ms");
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkBase.CreateSocketAndConnect(System.String,System.Int32)" />
        protected async Task<OperateResult<Socket>> CreateSocketAndConnectAsync(
          string ipAddress,
          int port)
        {
            OperateResult<Socket> socketAndConnectAsync = await this.CreateSocketAndConnectAsync(new IPEndPoint(IPAddress.Parse(ipAddress), port), 10000);
            return socketAndConnectAsync;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkBase.CreateSocketAndConnect(System.String,System.Int32,System.Int32)" />
        protected async Task<OperateResult<Socket>> CreateSocketAndConnectAsync(
          string ipAddress,
          int port,
          int timeOut)
        {
            OperateResult<Socket> socketAndConnectAsync = await this.CreateSocketAndConnectAsync(new IPEndPoint(IPAddress.Parse(ipAddress), port), timeOut);
            return socketAndConnectAsync;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkBase.Receive(System.Net.Sockets.Socket,System.Int32,System.Int32,System.Action{System.Int64,System.Int64})" />
        protected async Task<OperateResult<byte[]>> ReceiveAsync(
          Socket socket,
          int length,
          int timeOut = 60000,
          Action<long, long> reportProgress = null)
        {
            if (length == 0)
                return OperateResult.CreateSuccessResult<byte[]>(new byte[0]);
            if (!ESTCore.Common.Authorization.nzugaydgwadawdibbas())
            {
                OperateResult<byte[]> operateResult = new OperateResult<byte[]>(StringResources.Language.AuthorizationFailed);
            }
            EstTimeOut hslTimeOut = EstTimeOut.HandleTimeOutCheck(socket, timeOut);
            try
            {
                if (length > 0)
                {
                    byte[] buffer = new byte[length];
                    int alreadyCount = 0;
                    do
                    {
                        int currentReceiveLength = length - alreadyCount > 16384 ? 16384 : length - alreadyCount;
                        int count = await Task.Factory.FromAsync<int>(socket.BeginReceive(buffer, alreadyCount, currentReceiveLength, SocketFlags.None, (AsyncCallback)null, (object)socket), new Func<IAsyncResult, int>(socket.EndReceive));
                        alreadyCount += count;
                        if (count > 0)
                        {
                            hslTimeOut.StartTime = DateTime.Now;
                            Action<long, long> action = reportProgress;
                            if (action != null)
                                action((long)alreadyCount, (long)length);
                        }
                        else
                            goto label_9;
                    }
                    while (alreadyCount < length);
                    goto label_13;
                label_9:
                    throw new RemoteCloseException();
                label_13:
                    hslTimeOut.IsSuccessful = true;
                    return OperateResult.CreateSuccessResult<byte[]>(buffer);
                }
                byte[] buffer1 = new byte[2048];
                int count1 = await Task.Factory.FromAsync<int>(socket.BeginReceive(buffer1, 0, buffer1.Length, SocketFlags.None, (AsyncCallback)null, (object)socket), new Func<IAsyncResult, int>(socket.EndReceive));
                hslTimeOut.IsSuccessful = true;
                return OperateResult.CreateSuccessResult<byte[]>(SoftBasic.ArraySelectBegin<byte>(buffer1, count1));
            }
            catch (RemoteCloseException ex)
            {
                socket?.Close();
                if (this.connectErrorCount < 1000000000)
                    ++this.connectErrorCount;
                hslTimeOut.IsSuccessful = true;
                return new OperateResult<byte[]>(-this.connectErrorCount, StringResources.Language.RemoteClosedConnection);
            }
            catch (Exception ex)
            {
                socket?.Close();
                hslTimeOut.IsSuccessful = true;
                if (this.connectErrorCount < 1000000000)
                    ++this.connectErrorCount;
                return !hslTimeOut.IsTimeout ? new OperateResult<byte[]>(-this.connectErrorCount, "Socket Exception -> " + ex.Message) : new OperateResult<byte[]>(-this.connectErrorCount, StringResources.Language.ReceiveDataTimeout + hslTimeOut.DelayTime.ToString());
            }
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkBase.ReceiveCommandLineFromSocket(System.Net.Sockets.Socket,System.Byte,System.Int32)" />
        protected async Task<OperateResult<byte[]>> ReceiveCommandLineFromSocketAsync(
          Socket socket,
          byte endCode,
          int timeout = 2147483647)
        {
            List<byte> bufferArray = new List<byte>();
            try
            {
                DateTime st = DateTime.Now;
                bool bOK = false;
                while ((DateTime.Now - st).TotalMilliseconds < (double)timeout)
                {
                    if (socket.Poll(timeout, SelectMode.SelectRead))
                    {
                        OperateResult<byte[]> headResult = await this.ReceiveAsync(socket, 1, 5000);
                        if (!headResult.IsSuccess)
                            return headResult;
                        bufferArray.AddRange((IEnumerable<byte>)headResult.Content);
                        if ((int)headResult.Content[0] == (int)endCode)
                        {
                            bOK = true;
                            break;
                        }
                        headResult = (OperateResult<byte[]>)null;
                    }
                }
                return bOK ? OperateResult.CreateSuccessResult<byte[]>(bufferArray.ToArray()) : new OperateResult<byte[]>(StringResources.Language.ReceiveDataTimeout);
            }
            catch (Exception ex)
            {
                socket?.Close();
                return new OperateResult<byte[]>(ex.Message);
            }
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkBase.ReceiveCommandLineFromSocket(System.Net.Sockets.Socket,System.Byte,System.Byte,System.Int32)" />
        protected async Task<OperateResult<byte[]>> ReceiveCommandLineFromSocketAsync(
          Socket socket,
          byte endCode1,
          byte endCode2,
          int timeout = 60000)
        {
            List<byte> bufferArray = new List<byte>();
            try
            {
                DateTime st = DateTime.Now;
                bool bOK = false;
                while ((DateTime.Now - st).TotalMilliseconds < (double)timeout)
                {
                    if (socket.Poll(timeout, SelectMode.SelectRead))
                    {
                        OperateResult<byte[]> headResult = await this.ReceiveAsync(socket, 1, timeout);
                        if (!headResult.IsSuccess)
                            return headResult;
                        bufferArray.AddRange((IEnumerable<byte>)headResult.Content);
                        if ((int)headResult.Content[0] == (int)endCode2 && (bufferArray.Count > 1 && (int)bufferArray[bufferArray.Count - 2] == (int)endCode1))
                        {
                            bOK = true;
                            break;
                        }
                        headResult = (OperateResult<byte[]>)null;
                    }
                }
                return bOK ? OperateResult.CreateSuccessResult<byte[]>(bufferArray.ToArray()) : new OperateResult<byte[]>(StringResources.Language.ReceiveDataTimeout);
            }
            catch (Exception ex)
            {
                socket?.Close();
                return new OperateResult<byte[]>(ex.Message);
            }
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkBase.Send(System.Net.Sockets.Socket,System.Byte[])" />
        protected async Task<OperateResult> SendAsync(Socket socket, byte[] data)
        {
            if (data == null)
                return OperateResult.CreateSuccessResult();
            OperateResult operateResult = await this.SendAsync(socket, data, 0, data.Length);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkBase.Send(System.Net.Sockets.Socket,System.Byte[],System.Int32,System.Int32)" />
        protected async Task<OperateResult> SendAsync(
          Socket socket,
          byte[] data,
          int offset,
          int size)
        {
            if (data == null)
                return OperateResult.CreateSuccessResult();
            if (!ESTCore.Common.Authorization.nzugaydgwadawdibbas())
                return new OperateResult(StringResources.Language.AuthorizationFailed);
            int alreadyCount = 0;
            try
            {
                do
                {
                    int count = await Task.Factory.FromAsync<int>(socket.BeginSend(data, offset, size - alreadyCount, SocketFlags.None, (AsyncCallback)null, (object)socket), new Func<IAsyncResult, int>(socket.EndSend));
                    alreadyCount += count;
                    offset += count;
                }
                while (alreadyCount < size);
                return OperateResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                socket?.Close();
                if (this.connectErrorCount < 1000000000)
                    ++this.connectErrorCount;
                return (OperateResult)new OperateResult<byte[]>(-this.connectErrorCount, ex.Message);
            }
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkBase.ReceiveByMessage(System.Net.Sockets.Socket,System.Int32,ESTCore.Common.Core.IMessage.INetMessage,System.Action{System.Int64,System.Int64})" />
        protected async Task<OperateResult<byte[]>> ReceiveByMessageAsync(
          Socket socket,
          int timeOut,
          INetMessage netMessage,
          Action<long, long> reportProgress = null)
        {
            if (netMessage == null)
            {
                OperateResult<byte[]> async = await this.ReceiveAsync(socket, -1, timeOut);
                return async;
            }
            OperateResult<byte[]> headResult = await this.ReceiveAsync(socket, netMessage.ProtocolHeadBytesLength, timeOut);
            if (!headResult.IsSuccess)
                return headResult;
            netMessage.HeadBytes = headResult.Content;
            int contentLength = netMessage.GetContentLengthByHeadBytes();
            OperateResult<byte[]> contentResult = await this.ReceiveAsync(socket, contentLength, timeOut, reportProgress);
            if (!contentResult.IsSuccess)
                return contentResult;
            netMessage.ContentBytes = contentResult.Content;
            return OperateResult.CreateSuccessResult<byte[]>(SoftBasic.SpliceArray<byte>(headResult.Content, contentResult.Content));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkBase.ReadStream(System.IO.Stream,System.Byte[])" />
        protected async Task<OperateResult<int>> ReadStreamAsync(
          Stream stream,
          byte[] buffer)
        {
            int num = 0;
            if (num != 0 && !ESTCore.Common.Authorization.nzugaydgwadawdibbas())
                return new OperateResult<int>(StringResources.Language.AuthorizationFailed);
            try
            {
                int count = await stream.ReadAsync(buffer, 0, buffer.Length);
                return OperateResult.CreateSuccessResult<int>(count);
            }
            catch (Exception ex)
            {
                stream?.Close();
                return new OperateResult<int>(ex.Message);
            }
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkBase.WriteStream(System.IO.Stream,System.Byte[])" />
        protected async Task<OperateResult> WriteStreamAsync(
          Stream stream,
          byte[] buffer)
        {
            if (!ESTCore.Common.Authorization.nzugaydgwadawdibbas())
                return new OperateResult(StringResources.Language.AuthorizationFailed);
            int alreadyCount = 0;
            try
            {
                await stream.WriteAsync(buffer, alreadyCount, buffer.Length - alreadyCount);
                return (OperateResult)OperateResult.CreateSuccessResult<int>(alreadyCount);
            }
            catch (Exception ex)
            {
                stream?.Close();
                return (OperateResult)new OperateResult<int>(ex.Message);
            }
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkBase.ReceiveLong(System.Net.Sockets.Socket)" />
        private async Task<OperateResult<long>> ReceiveLongAsync(Socket socket)
        {
            OperateResult<byte[]> read = await this.ReceiveAsync(socket, 8, -1);
            OperateResult<long> operateResult = !read.IsSuccess ? OperateResult.CreateFailedResult<long>((OperateResult)read) : OperateResult.CreateSuccessResult<long>(BitConverter.ToInt64(read.Content, 0));
            read = (OperateResult<byte[]>)null;
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkBase.SendLong(System.Net.Sockets.Socket,System.Int64)" />
        private async Task<OperateResult> SendLongAsync(Socket socket, long value)
        {
            OperateResult operateResult = await this.SendAsync(socket, BitConverter.GetBytes(value));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkBase.SendBaseAndCheckReceive(System.Net.Sockets.Socket,System.Int32,System.Int32,System.Byte[])" />
        protected async Task<OperateResult> SendBaseAndCheckReceiveAsync(
          Socket socket,
          int headCode,
          int customer,
          byte[] send)
        {
            send = EstProtocol.CommandBytes(headCode, customer, this.Token, send);
            OperateResult sendResult = await this.SendAsync(socket, send);
            if (!sendResult.IsSuccess)
                return sendResult;
            OperateResult<long> checkResult = await this.ReceiveLongAsync(socket);
            if (!checkResult.IsSuccess || checkResult.Content == (long)send.Length)
                return (OperateResult)checkResult;
            socket?.Close();
            return new OperateResult(StringResources.Language.CommandLengthCheckFailed);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkBase.SendBytesAndCheckReceive(System.Net.Sockets.Socket,System.Int32,System.Byte[])" />
        protected async Task<OperateResult> SendBytesAndCheckReceiveAsync(
          Socket socket,
          int customer,
          byte[] send)
        {
            OperateResult async = await this.SendBaseAndCheckReceiveAsync(socket, 1002, customer, send);
            return async;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkBase.SendStringAndCheckReceive(System.Net.Sockets.Socket,System.Int32,System.String)" />
        protected async Task<OperateResult> SendStringAndCheckReceiveAsync(
          Socket socket,
          int customer,
          string send)
        {
            byte[] data = string.IsNullOrEmpty(send) ? (byte[])null : Encoding.Unicode.GetBytes(send);
            OperateResult async = await this.SendBaseAndCheckReceiveAsync(socket, 1001, customer, data);
            data = (byte[])null;
            return async;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkBase.SendStringAndCheckReceive(System.Net.Sockets.Socket,System.Int32,System.String[])" />
        protected async Task<OperateResult> SendStringAndCheckReceiveAsync(
          Socket socket,
          int customer,
          string[] sends)
        {
            OperateResult async = await this.SendBaseAndCheckReceiveAsync(socket, 1005, customer, EstProtocol.PackStringArrayToByte(sends));
            return async;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkBase.SendAccountAndCheckReceive(System.Net.Sockets.Socket,System.Int32,System.String,System.String)" />
        protected async Task<OperateResult> SendAccountAndCheckReceiveAsync(
          Socket socket,
          int customer,
          string name,
          string pwd)
        {
            OperateResult async = await this.SendBaseAndCheckReceiveAsync(socket, 5, customer, EstProtocol.PackStringArrayToByte(new string[2]
            {
        name,
        pwd
            }));
            return async;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkBase.ReceiveAndCheckBytes(System.Net.Sockets.Socket,System.Int32)" />
        protected async Task<OperateResult<byte[], byte[]>> ReceiveAndCheckBytesAsync(
          Socket socket,
          int timeout)
        {
            OperateResult<byte[]> headResult = await this.ReceiveAsync(socket, 32, timeout);
            if (!headResult.IsSuccess)
                return OperateResult.CreateFailedResult<byte[], byte[]>((OperateResult)headResult);
            if (!this.CheckRemoteToken(headResult.Content))
            {
                socket?.Close();
                return new OperateResult<byte[], byte[]>(StringResources.Language.TokenCheckFailed);
            }
            int contentLength = BitConverter.ToInt32(headResult.Content, 28);
            OperateResult<byte[]> contentResult = await this.ReceiveAsync(socket, contentLength, timeout);
            if (!contentResult.IsSuccess)
                return OperateResult.CreateFailedResult<byte[], byte[]>((OperateResult)contentResult);
            OperateResult checkResult = await this.SendLongAsync(socket, (long)(32 + contentLength));
            if (!checkResult.IsSuccess)
                return OperateResult.CreateFailedResult<byte[], byte[]>(checkResult);
            byte[] head = headResult.Content;
            byte[] content = contentResult.Content;
            content = EstProtocol.CommandAnalysis(head, content);
            return OperateResult.CreateSuccessResult<byte[], byte[]>(head, content);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkBase.ReceiveStringContentFromSocket(System.Net.Sockets.Socket,System.Int32)" />
        protected async Task<OperateResult<int, string>> ReceiveStringContentFromSocketAsync(
          Socket socket,
          int timeOut = 30000)
        {
            OperateResult<byte[], byte[]> receive = await this.ReceiveAndCheckBytesAsync(socket, timeOut);
            if (!receive.IsSuccess)
                return OperateResult.CreateFailedResult<int, string>((OperateResult)receive);
            if (BitConverter.ToInt32(receive.Content1, 0) != 1001)
            {
                socket?.Close();
                return new OperateResult<int, string>(StringResources.Language.CommandHeadCodeCheckFailed);
            }
            if (receive.Content2 == null)
                receive.Content2 = new byte[0];
            return OperateResult.CreateSuccessResult<int, string>(BitConverter.ToInt32(receive.Content1, 4), Encoding.Unicode.GetString(receive.Content2));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkBase.ReceiveStringArrayContentFromSocket(System.Net.Sockets.Socket,System.Int32)" />
        protected async Task<OperateResult<int, string[]>> ReceiveStringArrayContentFromSocketAsync(
          Socket socket,
          int timeOut = 30000)
        {
            OperateResult<byte[], byte[]> receive = await this.ReceiveAndCheckBytesAsync(socket, timeOut);
            if (!receive.IsSuccess)
                return OperateResult.CreateFailedResult<int, string[]>((OperateResult)receive);
            if (BitConverter.ToInt32(receive.Content1, 0) != 1005)
            {
                socket?.Close();
                return new OperateResult<int, string[]>(StringResources.Language.CommandHeadCodeCheckFailed);
            }
            if (receive.Content2 == null)
                receive.Content2 = new byte[4];
            return OperateResult.CreateSuccessResult<int, string[]>(BitConverter.ToInt32(receive.Content1, 4), EstProtocol.UnPackStringArrayFromByte(receive.Content2));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkBase.ReceiveBytesContentFromSocket(System.Net.Sockets.Socket,System.Int32)" />
        protected async Task<OperateResult<int, byte[]>> ReceiveBytesContentFromSocketAsync(
          Socket socket,
          int timeout = 30000)
        {
            OperateResult<byte[], byte[]> receive = await this.ReceiveAndCheckBytesAsync(socket, timeout);
            if (!receive.IsSuccess)
                return OperateResult.CreateFailedResult<int, byte[]>((OperateResult)receive);
            if (BitConverter.ToInt32(receive.Content1, 0) == 1002)
                return OperateResult.CreateSuccessResult<int, byte[]>(BitConverter.ToInt32(receive.Content1, 4), receive.Content2);
            socket?.Close();
            return new OperateResult<int, byte[]>(StringResources.Language.CommandHeadCodeCheckFailed);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkBase.SendStreamToSocket(System.Net.Sockets.Socket,System.IO.Stream,System.Int64,System.Action{System.Int64,System.Int64},System.Boolean)" />
        protected async Task<OperateResult> SendStreamToSocketAsync(
          Socket socket,
          Stream stream,
          long receive,
          Action<long, long> report,
          bool reportByPercent)
        {
            byte[] buffer = new byte[this.fileCacheSize];
            long SendTotal = 0;
            long percent = 0;
            stream.Position = 0L;
            while (SendTotal < receive)
            {
                OperateResult<int> read = await this.ReadStreamAsync(stream, buffer);
                if (!read.IsSuccess)
                {
                    socket?.Close();
                    return (OperateResult)read;
                }
                SendTotal += (long)read.Content;
                byte[] newBuffer = new byte[read.Content];
                Array.Copy((Array)buffer, 0, (Array)newBuffer, 0, newBuffer.Length);
                OperateResult write = await this.SendBytesAndCheckReceiveAsync(socket, read.Content, newBuffer);
                if (!write.IsSuccess)
                {
                    socket?.Close();
                    return write;
                }
                if (reportByPercent)
                {
                    long percentCurrent = SendTotal * 100L / receive;
                    if (percent != percentCurrent)
                    {
                        percent = percentCurrent;
                        Action<long, long> action = report;
                        if (action != null)
                            action(SendTotal, receive);
                    }
                }
                else
                {
                    Action<long, long> action = report;
                    if (action != null)
                        action(SendTotal, receive);
                }
                read = (OperateResult<int>)null;
                newBuffer = (byte[])null;
                write = (OperateResult)null;
            }
            return OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkBase.WriteStreamFromSocket(System.Net.Sockets.Socket,System.IO.Stream,System.Int64,System.Action{System.Int64,System.Int64},System.Boolean)" />
        protected async Task<OperateResult> WriteStreamFromSocketAsync(
          Socket socket,
          Stream stream,
          long totalLength,
          Action<long, long> report,
          bool reportByPercent)
        {
            long count_receive = 0;
            long percent = 0;
            while (count_receive < totalLength)
            {
                OperateResult<int, byte[]> read = await this.ReceiveBytesContentFromSocketAsync(socket, 60000);
                if (!read.IsSuccess)
                    return (OperateResult)read;
                count_receive += (long)read.Content1;
                OperateResult write = await this.WriteStreamAsync(stream, read.Content2);
                if (!write.IsSuccess)
                {
                    socket?.Close();
                    return write;
                }
                if (reportByPercent)
                {
                    long percentCurrent = count_receive * 100L / totalLength;
                    if (percent != percentCurrent)
                    {
                        percent = percentCurrent;
                        Action<long, long> action = report;
                        if (action != null)
                            action(count_receive, totalLength);
                    }
                }
                else
                {
                    Action<long, long> action = report;
                    if (action != null)
                        action(count_receive, totalLength);
                }
                read = (OperateResult<int, byte[]>)null;
                write = (OperateResult)null;
            }
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 从socket接收一条完整的websocket数据，返回<see cref="T:ESTCore.Common.WebSocket.WebSocketMessage" />的数据信息<br />
        /// Receive a complete websocket data from the socket, return the data information of the <see cref="T:ESTCore.Common.WebSocket.WebSocketMessage" />
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <returns>包含websocket消息的结果内容</returns>
        protected OperateResult<WebSocketMessage> ReceiveWebSocketPayload(
          Socket socket)
        {
            List<byte> byteList = new List<byte>();
            OperateResult<WebSocketMessage, bool> webSocketPayload;
            do
            {
                webSocketPayload = this.ReceiveFrameWebSocketPayload(socket);
                if (webSocketPayload.IsSuccess)
                    byteList.AddRange((IEnumerable<byte>)webSocketPayload.Content1.Payload);
                else
                    goto label_1;
            }
            while (!webSocketPayload.Content2);
            goto label_3;
        label_1:
            return OperateResult.CreateFailedResult<WebSocketMessage>((OperateResult)webSocketPayload);
        label_3:
            return OperateResult.CreateSuccessResult<WebSocketMessage>(new WebSocketMessage()
            {
                HasMask = webSocketPayload.Content1.HasMask,
                OpCode = webSocketPayload.Content1.OpCode,
                Payload = byteList.ToArray()
            });
        }

        /// <summary>
        /// 从socket接收一条<see cref="T:ESTCore.Common.WebSocket.WebSocketMessage" />片段数据，返回<see cref="T:ESTCore.Common.WebSocket.WebSocketMessage" />的数据信息和是否最后一条数据内容<br />
        /// Receive a piece of <see cref="T:ESTCore.Common.WebSocket.WebSocketMessage" /> fragment data from the socket, return the data information of <see cref="T:ESTCore.Common.WebSocket.WebSocketMessage" /> and whether the last data content
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <returns>包含websocket消息的结果内容</returns>
        protected OperateResult<WebSocketMessage, bool> ReceiveFrameWebSocketPayload(
          Socket socket)
        {
            OperateResult<byte[]> operateResult1 = this.Receive(socket, 2, 5000);
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<WebSocketMessage, bool>((OperateResult)operateResult1);
            bool flag1 = ((int)operateResult1.Content[0] & 128) == 128;
            bool flag2 = ((int)operateResult1.Content[1] & 128) == 128;
            int num = (int)operateResult1.Content[0] & 15;
            byte[] numArray = (byte[])null;
            int length = (int)operateResult1.Content[1] & (int)sbyte.MaxValue;
            switch (length)
            {
                case 126:
                    OperateResult<byte[]> operateResult2 = this.Receive(socket, 2, 5000);
                    if (!operateResult2.IsSuccess)
                        return OperateResult.CreateFailedResult<WebSocketMessage, bool>((OperateResult)operateResult2);
                    Array.Reverse((Array)operateResult2.Content);
                    length = (int)BitConverter.ToUInt16(operateResult2.Content, 0);
                    break;
                case (int)sbyte.MaxValue:
                    OperateResult<byte[]> operateResult3 = this.Receive(socket, 8, 5000);
                    if (!operateResult3.IsSuccess)
                        return OperateResult.CreateFailedResult<WebSocketMessage, bool>((OperateResult)operateResult3);
                    Array.Reverse((Array)operateResult3.Content);
                    length = (int)BitConverter.ToUInt64(operateResult3.Content, 0);
                    break;
            }
            if (flag2)
            {
                OperateResult<byte[]> operateResult4 = this.Receive(socket, 4, 5000);
                if (!operateResult4.IsSuccess)
                    return OperateResult.CreateFailedResult<WebSocketMessage, bool>((OperateResult)operateResult4);
                numArray = operateResult4.Content;
            }
            OperateResult<byte[]> operateResult5 = this.Receive(socket, length);
            if (!operateResult5.IsSuccess)
                return OperateResult.CreateFailedResult<WebSocketMessage, bool>((OperateResult)operateResult5);
            if (flag2)
            {
                for (int index = 0; index < operateResult5.Content.Length; ++index)
                    operateResult5.Content[index] = (byte)((uint)operateResult5.Content[index] ^ (uint)numArray[index % 4]);
            }
            return OperateResult.CreateSuccessResult<WebSocketMessage, bool>(new WebSocketMessage()
            {
                HasMask = flag2,
                OpCode = num,
                Payload = operateResult5.Content
            }, flag1);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkBase.ReceiveWebSocketPayload(System.Net.Sockets.Socket)" />
        protected async Task<OperateResult<WebSocketMessage>> ReceiveWebSocketPayloadAsync(
          Socket socket)
        {
            List<byte> data = new List<byte>();
            OperateResult<WebSocketMessage, bool> read;
            while (true)
            {
                read = await this.ReceiveFrameWebSocketPayloadAsync(socket);
                if (read.IsSuccess)
                {
                    data.AddRange((IEnumerable<byte>)read.Content1.Payload);
                    if (!read.Content2)
                        read = (OperateResult<WebSocketMessage, bool>)null;
                    else
                        goto label_4;
                }
                else
                    break;
            }
            return OperateResult.CreateFailedResult<WebSocketMessage>((OperateResult)read);
        label_4:
            return OperateResult.CreateSuccessResult<WebSocketMessage>(new WebSocketMessage()
            {
                HasMask = read.Content1.HasMask,
                OpCode = read.Content1.OpCode,
                Payload = data.ToArray()
            });
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkBase.ReceiveFrameWebSocketPayload(System.Net.Sockets.Socket)" />
        protected async Task<OperateResult<WebSocketMessage, bool>> ReceiveFrameWebSocketPayloadAsync(
          Socket socket)
        {
            OperateResult<byte[]> head = await this.ReceiveAsync(socket, 2, 10000);
            if (!head.IsSuccess)
                return OperateResult.CreateFailedResult<WebSocketMessage, bool>((OperateResult)head);
            bool isEof = ((int)head.Content[0] & 128) == 128;
            bool hasMask = ((int)head.Content[1] & 128) == 128;
            int opCode = (int)head.Content[0] & 15;
            byte[] mask = (byte[])null;
            int length = (int)head.Content[1] & (int)sbyte.MaxValue;
            switch (length)
            {
                case 126:
                    OperateResult<byte[]> extended1 = await this.ReceiveAsync(socket, 2, 30000);
                    if (!extended1.IsSuccess)
                        return OperateResult.CreateFailedResult<WebSocketMessage, bool>((OperateResult)extended1);
                    Array.Reverse((Array)extended1.Content);
                    length = (int)BitConverter.ToUInt16(extended1.Content, 0);
                    extended1 = (OperateResult<byte[]>)null;
                    break;
                case (int)sbyte.MaxValue:
                    OperateResult<byte[]> extended2 = await this.ReceiveAsync(socket, 8, 30000);
                    if (!extended2.IsSuccess)
                        return OperateResult.CreateFailedResult<WebSocketMessage, bool>((OperateResult)extended2);
                    Array.Reverse((Array)extended2.Content);
                    length = (int)BitConverter.ToUInt64(extended2.Content, 0);
                    extended2 = (OperateResult<byte[]>)null;
                    break;
            }
            if (hasMask)
            {
                OperateResult<byte[]> maskResult = await this.ReceiveAsync(socket, 4, 30000);
                if (!maskResult.IsSuccess)
                    return OperateResult.CreateFailedResult<WebSocketMessage, bool>((OperateResult)maskResult);
                mask = maskResult.Content;
                maskResult = (OperateResult<byte[]>)null;
            }
            OperateResult<byte[]> payload = await this.ReceiveAsync(socket, length);
            if (!payload.IsSuccess)
                return OperateResult.CreateFailedResult<WebSocketMessage, bool>((OperateResult)payload);
            if (hasMask)
            {
                for (int i = 0; i < payload.Content.Length; ++i)
                    payload.Content[i] = (byte)((uint)payload.Content[i] ^ (uint)mask[i % 4]);
            }
            return OperateResult.CreateSuccessResult<WebSocketMessage, bool>(new WebSocketMessage()
            {
                HasMask = hasMask,
                OpCode = opCode,
                Payload = payload.Content
            }, isEof);
        }

        /// <summary>
        /// 基于MQTT协议，从网络套接字中接收剩余的数据长度<br />
        /// Receives the remaining data length from the network socket based on the MQTT protocol
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <returns>网络中剩余的长度数据</returns>
        private OperateResult<int> ReceiveMqttRemainingLength(Socket socket)
        {
            List<byte> byteList = new List<byte>();
            OperateResult<byte[]> operateResult;
            do
            {
                operateResult = this.Receive(socket, 1, 5000);
                if (operateResult.IsSuccess)
                    byteList.Add(operateResult.Content[0]);
                else
                    goto label_1;
            }
            while (operateResult.Content[0] >= (byte)128 && byteList.Count < 4);
            goto label_4;
        label_1:
            return OperateResult.CreateFailedResult<int>((OperateResult)operateResult);
        label_4:
            if (byteList.Count > 4)
                return new OperateResult<int>("Receive Length is too long!");
            if (byteList.Count == 1)
                return OperateResult.CreateSuccessResult<int>((int)byteList[0]);
            if (byteList.Count == 2)
                return OperateResult.CreateSuccessResult<int>((int)byteList[0] - 128 + (int)byteList[1] * 128);
            return byteList.Count == 3 ? OperateResult.CreateSuccessResult<int>((int)byteList[0] - 128 + ((int)byteList[1] - 128) * 128 + (int)byteList[2] * 128 * 128) : OperateResult.CreateSuccessResult<int>((int)byteList[0] - 128 + ((int)byteList[1] - 128) * 128 + ((int)byteList[2] - 128) * 128 * 128 + (int)byteList[3] * 128 * 128 * 128);
        }

        /// <summary>
        /// 接收一条完整的MQTT协议的报文信息，包含控制码和负载数据<br />
        /// Receive a message of a completed MQTT protocol, including control code and payload data
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="reportProgress">进度报告，第一个参数是已完成的字节数量，第二个参数是总字节数量。</param>
        /// <returns>结果数据内容</returns>
        protected OperateResult<byte, byte[]> ReceiveMqttMessage(
          Socket socket,
          int timeOut,
          Action<long, long> reportProgress = null)
        {
            OperateResult<byte[]> operateResult1 = this.Receive(socket, 1, timeOut);
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<byte, byte[]>((OperateResult)operateResult1);
            OperateResult<int> mqttRemainingLength = this.ReceiveMqttRemainingLength(socket);
            if (!mqttRemainingLength.IsSuccess)
                return OperateResult.CreateFailedResult<byte, byte[]>((OperateResult)mqttRemainingLength);
            if ((int)operateResult1.Content[0] >> 4 == 15)
                reportProgress = (Action<long, long>)null;
            if ((int)operateResult1.Content[0] >> 4 == 0)
                reportProgress = (Action<long, long>)null;
            OperateResult<byte[]> operateResult2 = this.Receive(socket, mqttRemainingLength.Content, reportProgress: reportProgress);
            return !operateResult2.IsSuccess ? OperateResult.CreateFailedResult<byte, byte[]>((OperateResult)operateResult2) : OperateResult.CreateSuccessResult<byte, byte[]>(operateResult1.Content[0], operateResult2.Content);
        }

        /// <summary>
        /// 使用MQTT协议从socket接收指定长度的字节数组，然后全部写入到流中，可以指定进度报告<br />
        /// Use the MQTT protocol to receive a byte array of specified length from the socket, and then write all of them to the stream, and you can specify a progress report
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="stream">数据流</param>
        /// <param name="fileSize">数据大小</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="reportProgress">进度报告，第一个参数是已完成的字节数量，第二个参数是总字节数量。</param>
        /// <returns>是否操作成功</returns>
        protected OperateResult ReceiveMqttStream(
          Socket socket,
          Stream stream,
          long fileSize,
          int timeOut,
          Action<long, long> reportProgress = null)
        {
            long num = 0;
            while (num < fileSize)
            {
                OperateResult<byte, byte[]> mqttMessage = this.ReceiveMqttMessage(socket, timeOut);
                if (!mqttMessage.IsSuccess)
                    return (OperateResult)mqttMessage;
                if (mqttMessage.Content1 == (byte)0)
                {
                    socket?.Close();
                    return new OperateResult(Encoding.UTF8.GetString(mqttMessage.Content2));
                }
                OperateResult operateResult1 = this.WriteStream(stream, mqttMessage.Content2);
                if (!operateResult1.IsSuccess)
                    return operateResult1;
                num += (long)mqttMessage.Content2.Length;
                byte[] payLoad = new byte[16];
                BitConverter.GetBytes(num).CopyTo((Array)payLoad, 0);
                BitConverter.GetBytes(fileSize).CopyTo((Array)payLoad, 8);
                OperateResult operateResult2 = this.Send(socket, MqttHelper.BuildMqttCommand((byte)100, (byte[])null, payLoad).Content);
                if (!operateResult2.IsSuccess)
                    return operateResult2;
                if (reportProgress != null)
                    reportProgress(num, fileSize);
            }
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 使用MQTT协议将流中的数据读取到字节数组，然后都写入到socket里面，可以指定进度报告，主要用于将文件发送到网络。<br />
        /// Use the MQTT protocol to read the data in the stream into a byte array, and then write them all into the socket.
        /// You can specify a progress report, which is mainly used to send files to the network.
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="stream">流</param>
        /// <param name="fileSize">总的数据大小</param>
        /// <param name="timeOut">超时信息</param>
        /// <param name="reportProgress">进度报告，第一个参数是已完成的字节数量，第二个参数是总字节数量。</param>
        /// <returns>是否操作成功</returns>
        protected OperateResult SendMqttStream(
          Socket socket,
          Stream stream,
          long fileSize,
          int timeOut,
          Action<long, long> reportProgress = null)
        {
            byte[] buffer = new byte[this.fileCacheSize];
            long num = 0;
            stream.Position = 0L;
            while (num < fileSize)
            {
                OperateResult<int> operateResult1 = this.ReadStream(stream, buffer);
                if (!operateResult1.IsSuccess)
                {
                    socket?.Close();
                    return (OperateResult)operateResult1;
                }
                num += (long)operateResult1.Content;
                OperateResult operateResult2 = this.Send(socket, MqttHelper.BuildMqttCommand((byte)100, (byte[])null, buffer.SelectBegin<byte>(operateResult1.Content)).Content);
                if (!operateResult2.IsSuccess)
                {
                    socket?.Close();
                    return operateResult2;
                }
                OperateResult<byte, byte[]> mqttMessage = this.ReceiveMqttMessage(socket, timeOut);
                if (!mqttMessage.IsSuccess)
                    return (OperateResult)mqttMessage;
                if (reportProgress != null)
                    reportProgress(num, fileSize);
            }
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 使用MQTT协议将一个文件发送到网络上去，需要指定文件名，保存的文件名，可选指定文件描述信息，进度报告<br />
        /// To send a file to the network using the MQTT protocol, you need to specify the file name, the saved file name,
        /// optionally specify the file description information, and the progress report
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="filename">文件名称</param>
        /// <param name="servername">对方接收后保存的文件名</param>
        /// <param name="filetag">文件的描述信息</param>
        /// <param name="reportProgress">进度报告，第一个参数是已完成的字节数量，第二个参数是总字节数量。</param>
        /// <returns>是否操作成功</returns>
        protected OperateResult SendMqttFile(
          Socket socket,
          string filename,
          string servername,
          string filetag,
          Action<long, long> reportProgress = null)
        {
            FileInfo fileInfo = new FileInfo(filename);
            if (!System.IO.File.Exists(filename))
            {
                OperateResult operateResult = this.Send(socket, MqttHelper.BuildMqttCommand((byte)0, (byte[])null, Encoding.UTF8.GetBytes(StringResources.Language.FileNotExist)).Content);
                if (!operateResult.IsSuccess)
                    return operateResult;
                socket?.Close();
                return new OperateResult(StringResources.Language.FileNotExist);
            }
            string[] data = new string[3]
            {
        servername,
        fileInfo.Length.ToString(),
        filetag
            };
            OperateResult operateResult1 = this.Send(socket, MqttHelper.BuildMqttCommand((byte)100, (byte[])null, EstProtocol.PackStringArrayToByte(data)).Content);
            if (!operateResult1.IsSuccess)
                return operateResult1;
            OperateResult<byte, byte[]> mqttMessage = this.ReceiveMqttMessage(socket, 60000);
            if (!mqttMessage.IsSuccess)
                return (OperateResult)mqttMessage;
            if (mqttMessage.Content1 == (byte)0)
            {
                socket?.Close();
                return new OperateResult(Encoding.UTF8.GetString(mqttMessage.Content2));
            }
            try
            {
                OperateResult operateResult2 = new OperateResult();
                using (FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                    operateResult2 = this.SendMqttStream(socket, (Stream)fileStream, fileInfo.Length, 60000, reportProgress);
                return operateResult2;
            }
            catch (Exception ex)
            {
                socket?.Close();
                return new OperateResult("SendMqttStream Exception -> " + ex.Message);
            }
        }

        /// <summary>
        /// 使用MQTT协议将一个数据流发送到网络上去，需要保存的文件名，可选指定文件描述信息，进度报告<br />
        /// Use the MQTT protocol to send a data stream to the network, the file name that needs to be saved, optional file description information, progress report
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="stream">数据流</param>
        /// <param name="servername">对方接收后保存的文件名</param>
        /// <param name="filetag">文件的描述信息</param>
        /// <param name="reportProgress">进度报告，第一个参数是已完成的字节数量，第二个参数是总字节数量。</param>
        /// <returns>是否操作成功</returns>
        protected OperateResult SendMqttFile(
          Socket socket,
          Stream stream,
          string servername,
          string filetag,
          Action<long, long> reportProgress = null)
        {
            string[] data = new string[3]
            {
        servername,
        stream.Length.ToString(),
        filetag
            };
            OperateResult operateResult = this.Send(socket, MqttHelper.BuildMqttCommand((byte)100, (byte[])null, EstProtocol.PackStringArrayToByte(data)).Content);
            if (!operateResult.IsSuccess)
                return operateResult;
            OperateResult<byte, byte[]> mqttMessage = this.ReceiveMqttMessage(socket, 60000);
            if (!mqttMessage.IsSuccess)
                return (OperateResult)mqttMessage;
            if (mqttMessage.Content1 == (byte)0)
            {
                socket?.Close();
                return new OperateResult(Encoding.UTF8.GetString(mqttMessage.Content2));
            }
            try
            {
                return this.SendMqttStream(socket, stream, stream.Length, 60000, reportProgress);
            }
            catch (Exception ex)
            {
                socket?.Close();
                return new OperateResult("SendMqttStream Exception -> " + ex.Message);
            }
        }

        /// <summary>
        /// 使用MQTT协议从网络接收字节数组，然后写入文件或流中，支持进度报告<br />
        /// Use MQTT protocol to receive byte array from the network, and then write it to file or stream, support progress report
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="source">文件名或是流</param>
        /// <param name="reportProgress">进度报告</param>
        /// <returns>是否操作成功，如果成功，携带文件基本信息</returns>
        protected OperateResult<FileBaseInfo> ReceiveMqttFile(
          Socket socket,
          object source,
          Action<long, long> reportProgress = null)
        {
            OperateResult<byte, byte[]> mqttMessage = this.ReceiveMqttMessage(socket, 60000);
            if (!mqttMessage.IsSuccess)
                return OperateResult.CreateFailedResult<FileBaseInfo>((OperateResult)mqttMessage);
            if (mqttMessage.Content1 == (byte)0)
            {
                socket?.Close();
                return new OperateResult<FileBaseInfo>(Encoding.UTF8.GetString(mqttMessage.Content2));
            }
            FileBaseInfo fileBaseInfo = new FileBaseInfo();
            string[] strArray = EstProtocol.UnPackStringArrayFromByte(mqttMessage.Content2);
            fileBaseInfo.Name = strArray[0];
            fileBaseInfo.Size = long.Parse(strArray[1]);
            fileBaseInfo.Tag = strArray[2];
            this.Send(socket, MqttHelper.BuildMqttCommand((byte)100, (byte[])null, (byte[])null).Content);
            try
            {
                OperateResult result = (OperateResult)null;
                switch (source)
                {
                    case string path:
                        using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                            result = this.ReceiveMqttStream(socket, (Stream)fileStream, fileBaseInfo.Size, 60000, reportProgress);
                        if (!result.IsSuccess)
                        {
                            if (System.IO.File.Exists(path))
                                System.IO.File.Delete(path);
                            return OperateResult.CreateFailedResult<FileBaseInfo>(result);
                        }
                        break;
                    case Stream stream:
                        this.ReceiveMqttStream(socket, stream, fileBaseInfo.Size, 60000, reportProgress);
                        break;
                    default:
                        throw new Exception("Not Supported Type");
                }
                return OperateResult.CreateSuccessResult<FileBaseInfo>(fileBaseInfo);
            }
            catch (Exception ex)
            {
                socket?.Close();
                return new OperateResult<FileBaseInfo>(ex.Message);
            }
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkBase.ReceiveMqttRemainingLength(System.Net.Sockets.Socket)" />
        private async Task<OperateResult<int>> ReceiveMqttRemainingLengthAsync(
          Socket socket)
        {
            List<byte> buffer = new List<byte>();
            OperateResult<byte[]> rece;
            while (true)
            {
                rece = await this.ReceiveAsync(socket, 1, 5000);
                if (rece.IsSuccess)
                {
                    buffer.Add(rece.Content[0]);
                    if (rece.Content[0] >= (byte)128 && buffer.Count < 4)
                        rece = (OperateResult<byte[]>)null;
                    else
                        goto label_6;
                }
                else
                    break;
            }
            return OperateResult.CreateFailedResult<int>((OperateResult)rece);
        label_6:
            return buffer.Count <= 4 ? (buffer.Count != 1 ? (buffer.Count != 2 ? (buffer.Count != 3 ? OperateResult.CreateSuccessResult<int>((int)buffer[0] - 128 + ((int)buffer[1] - 128) * 128 + ((int)buffer[2] - 128) * 128 * 128 + (int)buffer[3] * 128 * 128 * 128) : OperateResult.CreateSuccessResult<int>((int)buffer[0] - 128 + ((int)buffer[1] - 128) * 128 + (int)buffer[2] * 128 * 128)) : OperateResult.CreateSuccessResult<int>((int)buffer[0] - 128 + (int)buffer[1] * 128)) : OperateResult.CreateSuccessResult<int>((int)buffer[0])) : new OperateResult<int>("Receive Length is too long!");
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkBase.ReceiveMqttMessage(System.Net.Sockets.Socket,System.Int32,System.Action{System.Int64,System.Int64})" />
        protected async Task<OperateResult<byte, byte[]>> ReceiveMqttMessageAsync(
          Socket socket,
          int timeOut,
          Action<long, long> reportProgress = null)
        {
            OperateResult<byte[]> readCode = await this.ReceiveAsync(socket, 1, timeOut);
            if (!readCode.IsSuccess)
                return OperateResult.CreateFailedResult<byte, byte[]>((OperateResult)readCode);
            OperateResult<int> readContentLength = await this.ReceiveMqttRemainingLengthAsync(socket);
            if (!readContentLength.IsSuccess)
                return OperateResult.CreateFailedResult<byte, byte[]>((OperateResult)readContentLength);
            if ((int)readCode.Content[0] >> 4 == 15)
                reportProgress = (Action<long, long>)null;
            if ((int)readCode.Content[0] >> 4 == 0)
                reportProgress = (Action<long, long>)null;
            OperateResult<byte[]> readContent = await this.ReceiveAsync(socket, readContentLength.Content, timeOut, reportProgress);
            return readContent.IsSuccess ? OperateResult.CreateSuccessResult<byte, byte[]>(readCode.Content[0], readContent.Content) : OperateResult.CreateFailedResult<byte, byte[]>((OperateResult)readContent);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkBase.ReceiveMqttStream(System.Net.Sockets.Socket,System.IO.Stream,System.Int64,System.Int32,System.Action{System.Int64,System.Int64})" />
        protected async Task<OperateResult> ReceiveMqttStreamAsync(
          Socket socket,
          Stream stream,
          long fileSize,
          int timeOut,
          Action<long, long> reportProgress = null)
        {
            long already = 0;
            while (already < fileSize)
            {
                OperateResult<byte, byte[]> receive = await this.ReceiveMqttMessageAsync(socket, timeOut);
                if (!receive.IsSuccess)
                    return (OperateResult)receive;
                if (receive.Content1 == (byte)0)
                {
                    socket?.Close();
                    return new OperateResult(Encoding.UTF8.GetString(receive.Content2));
                }
                OperateResult write = await this.WriteStreamAsync(stream, receive.Content2);
                if (!write.IsSuccess)
                    return write;
                already += (long)receive.Content2.Length;
                byte[] ack = new byte[16];
                BitConverter.GetBytes(already).CopyTo((Array)ack, 0);
                BitConverter.GetBytes(fileSize).CopyTo((Array)ack, 8);
                OperateResult send = await this.SendAsync(socket, MqttHelper.BuildMqttCommand((byte)100, (byte[])null, ack).Content);
                if (!send.IsSuccess)
                    return send;
                Action<long, long> action = reportProgress;
                if (action != null)
                    action(already, fileSize);
                receive = (OperateResult<byte, byte[]>)null;
                write = (OperateResult)null;
                ack = (byte[])null;
                send = (OperateResult)null;
            }
            return OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkBase.SendMqttStream(System.Net.Sockets.Socket,System.IO.Stream,System.Int64,System.Int32,System.Action{System.Int64,System.Int64})" />
        protected async Task<OperateResult> SendMqttStreamAsync(
          Socket socket,
          Stream stream,
          long fileSize,
          int timeOut,
          Action<long, long> reportProgress = null)
        {
            byte[] buffer = new byte[this.fileCacheSize];
            long already = 0;
            stream.Position = 0L;
            while (already < fileSize)
            {
                OperateResult<int> read = await this.ReadStreamAsync(stream, buffer);
                if (!read.IsSuccess)
                {
                    socket?.Close();
                    return (OperateResult)read;
                }
                already += (long)read.Content;
                OperateResult write = await this.SendAsync(socket, MqttHelper.BuildMqttCommand((byte)100, (byte[])null, buffer.SelectBegin<byte>(read.Content)).Content);
                if (!write.IsSuccess)
                {
                    socket?.Close();
                    return write;
                }
                OperateResult<byte, byte[]> receive = await this.ReceiveMqttMessageAsync(socket, timeOut);
                if (!receive.IsSuccess)
                    return (OperateResult)receive;
                Action<long, long> action = reportProgress;
                if (action != null)
                    action(already, fileSize);
                read = (OperateResult<int>)null;
                write = (OperateResult)null;
                receive = (OperateResult<byte, byte[]>)null;
            }
            return OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkBase.SendMqttFile(System.Net.Sockets.Socket,System.String,System.String,System.String,System.Action{System.Int64,System.Int64})" />
        protected async Task<OperateResult> SendMqttFileAsync(
          Socket socket,
          string filename,
          string servername,
          string filetag,
          Action<long, long> reportProgress = null)
        {
            FileInfo info = new FileInfo(filename);
            if (!System.IO.File.Exists(filename))
            {
                OperateResult notFoundResult = await this.SendAsync(socket, MqttHelper.BuildMqttCommand((byte)0, (byte[])null, Encoding.UTF8.GetBytes(StringResources.Language.FileNotExist)).Content);
                if (!notFoundResult.IsSuccess)
                    return notFoundResult;
                socket?.Close();
                return new OperateResult(StringResources.Language.FileNotExist);
            }
            string[] array = new string[3]
            {
        servername,
        info.Length.ToString(),
        filetag
            };
            OperateResult sendResult = await this.SendAsync(socket, MqttHelper.BuildMqttCommand((byte)100, (byte[])null, EstProtocol.PackStringArrayToByte(array)).Content);
            if (!sendResult.IsSuccess)
                return sendResult;
            OperateResult<byte, byte[]> check = await this.ReceiveMqttMessageAsync(socket, 60000);
            if (!check.IsSuccess)
                return (OperateResult)check;
            if (check.Content1 == (byte)0)
            {
                socket?.Close();
                return new OperateResult(Encoding.UTF8.GetString(check.Content2));
            }
            try
            {
                OperateResult result = new OperateResult();
                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                    result = await this.SendMqttStreamAsync(socket, (Stream)fs, info.Length, 60000, reportProgress);
                return result;
            }
            catch (Exception ex)
            {
                socket?.Close();
                return new OperateResult("SendMqttStreamAsync Exception -> " + ex.Message);
            }
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkBase.SendMqttFile(System.Net.Sockets.Socket,System.IO.Stream,System.String,System.String,System.Action{System.Int64,System.Int64})" />
        protected async Task<OperateResult> SendMqttFileAsync(
          Socket socket,
          Stream stream,
          string servername,
          string filetag,
          Action<long, long> reportProgress = null)
        {
            string[] array = new string[3]
            {
        servername,
        stream.Length.ToString(),
        filetag
            };
            OperateResult sendResult = await this.SendAsync(socket, MqttHelper.BuildMqttCommand((byte)100, (byte[])null, EstProtocol.PackStringArrayToByte(array)).Content);
            if (!sendResult.IsSuccess)
                return sendResult;
            OperateResult<byte, byte[]> check = await this.ReceiveMqttMessageAsync(socket, 60000);
            if (!check.IsSuccess)
                return (OperateResult)check;
            if (check.Content1 == (byte)0)
            {
                socket?.Close();
                return new OperateResult(Encoding.UTF8.GetString(check.Content2));
            }
            try
            {
                OperateResult operateResult = await this.SendMqttStreamAsync(socket, stream, stream.Length, 60000, reportProgress);
                return operateResult;
            }
            catch (Exception ex)
            {
                socket?.Close();
                return new OperateResult("SendMqttStreamAsync Exception -> " + ex.Message);
            }
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkBase.ReceiveMqttFile(System.Net.Sockets.Socket,System.Object,System.Action{System.Int64,System.Int64})" />
        protected async Task<OperateResult<FileBaseInfo>> ReceiveMqttFileAsync(
          Socket socket,
          object source,
          Action<long, long> reportProgress = null)
        {
            OperateResult<byte, byte[]> receiveFileInfo = await this.ReceiveMqttMessageAsync(socket, 60000);
            if (!receiveFileInfo.IsSuccess)
                return OperateResult.CreateFailedResult<FileBaseInfo>((OperateResult)receiveFileInfo);
            if (receiveFileInfo.Content1 == (byte)0)
            {
                socket?.Close();
                return new OperateResult<FileBaseInfo>(Encoding.UTF8.GetString(receiveFileInfo.Content2));
            }
            FileBaseInfo fileBaseInfo = new FileBaseInfo();
            string[] array = EstProtocol.UnPackStringArrayFromByte(receiveFileInfo.Content2);
            fileBaseInfo.Name = array[0];
            fileBaseInfo.Size = long.Parse(array[1]);
            fileBaseInfo.Tag = array[2];
            OperateResult operateResult = await this.SendAsync(socket, MqttHelper.BuildMqttCommand((byte)100, (byte[])null, (byte[])null).Content);
            try
            {
                OperateResult write = (OperateResult)null;
                switch (source)
                {
                    case string savename:
                        using (FileStream fs = new FileStream(savename, FileMode.Create, FileAccess.Write))
                            write = await this.ReceiveMqttStreamAsync(socket, (Stream)fs, fileBaseInfo.Size, 60000, reportProgress);
                        if (!write.IsSuccess)
                        {
                            if (System.IO.File.Exists(savename))
                                System.IO.File.Delete(savename);
                            return OperateResult.CreateFailedResult<FileBaseInfo>(write);
                        }
                        break;
                    case Stream stream:
                        write = await this.ReceiveMqttStreamAsync(socket, stream, fileBaseInfo.Size, 60000, reportProgress);
                        stream = (Stream)null;
                        break;
                    default:
                        throw new Exception("Not Supported Type");
                }
                return OperateResult.CreateSuccessResult<FileBaseInfo>(fileBaseInfo);
            }
            catch (Exception ex)
            {
                socket?.Close();
                return new OperateResult<FileBaseInfo>(ex.Message);
            }
        }

        /// <summary>
        /// 接收一行基于redis协议的字符串的信息，需要指定固定的长度<br />
        /// Receive a line of information based on the redis protocol string, you need to specify a fixed length
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="length">字符串的长度</param>
        /// <returns>带有结果对象的数据信息</returns>
        protected OperateResult<byte[]> ReceiveRedisCommandString(
          Socket socket,
          int length)
        {
            List<byte> byteList = new List<byte>();
            OperateResult<byte[]> operateResult = this.Receive(socket, length);
            if (!operateResult.IsSuccess)
                return operateResult;
            byteList.AddRange((IEnumerable<byte>)operateResult.Content);
            OperateResult<byte[]> commandLineFromSocket = this.ReceiveCommandLineFromSocket(socket, (byte)10);
            if (!commandLineFromSocket.IsSuccess)
                return commandLineFromSocket;
            byteList.AddRange((IEnumerable<byte>)commandLineFromSocket.Content);
            return OperateResult.CreateSuccessResult<byte[]>(byteList.ToArray());
        }

        /// <summary>
        /// 从网络接收一条完整的redis报文的消息<br />
        /// Receive a complete redis message from the network
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <returns>接收的结果对象</returns>
        protected OperateResult<byte[]> ReceiveRedisCommand(Socket socket)
        {
            List<byte> byteList = new List<byte>();
            OperateResult<byte[]> commandLineFromSocket = this.ReceiveCommandLineFromSocket(socket, (byte)10);
            if (!commandLineFromSocket.IsSuccess)
                return commandLineFromSocket;
            byteList.AddRange((IEnumerable<byte>)commandLineFromSocket.Content);
            if (commandLineFromSocket.Content[0] == (byte)43 || commandLineFromSocket.Content[0] == (byte)45 || commandLineFromSocket.Content[0] == (byte)58)
                return OperateResult.CreateSuccessResult<byte[]>(byteList.ToArray());
            if (commandLineFromSocket.Content[0] == (byte)36)
            {
                OperateResult<int> numberFromCommandLine = RedisHelper.GetNumberFromCommandLine(commandLineFromSocket.Content);
                if (!numberFromCommandLine.IsSuccess)
                    return OperateResult.CreateFailedResult<byte[]>((OperateResult)numberFromCommandLine);
                if (numberFromCommandLine.Content < 0)
                    return OperateResult.CreateSuccessResult<byte[]>(byteList.ToArray());
                OperateResult<byte[]> redisCommandString = this.ReceiveRedisCommandString(socket, numberFromCommandLine.Content);
                if (!redisCommandString.IsSuccess)
                    return redisCommandString;
                byteList.AddRange((IEnumerable<byte>)redisCommandString.Content);
                return OperateResult.CreateSuccessResult<byte[]>(byteList.ToArray());
            }
            if (commandLineFromSocket.Content[0] != (byte)42)
                return new OperateResult<byte[]>("Not Supported HeadCode: " + commandLineFromSocket.Content[0].ToString());
            OperateResult<int> numberFromCommandLine1 = RedisHelper.GetNumberFromCommandLine(commandLineFromSocket.Content);
            if (!numberFromCommandLine1.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)numberFromCommandLine1);
            for (int index = 0; index < numberFromCommandLine1.Content; ++index)
            {
                OperateResult<byte[]> redisCommand = this.ReceiveRedisCommand(socket);
                if (!redisCommand.IsSuccess)
                    return redisCommand;
                byteList.AddRange((IEnumerable<byte>)redisCommand.Content);
            }
            return OperateResult.CreateSuccessResult<byte[]>(byteList.ToArray());
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkBase.ReceiveRedisCommandString(System.Net.Sockets.Socket,System.Int32)" />
        protected async Task<OperateResult<byte[]>> ReceiveRedisCommandStringAsync(
          Socket socket,
          int length)
        {
            List<byte> bufferArray = new List<byte>();
            OperateResult<byte[]> receive = await this.ReceiveAsync(socket, length);
            if (!receive.IsSuccess)
                return receive;
            bufferArray.AddRange((IEnumerable<byte>)receive.Content);
            OperateResult<byte[]> commandTail = await this.ReceiveCommandLineFromSocketAsync(socket, (byte)10);
            if (!commandTail.IsSuccess)
                return commandTail;
            bufferArray.AddRange((IEnumerable<byte>)commandTail.Content);
            return OperateResult.CreateSuccessResult<byte[]>(bufferArray.ToArray());
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkBase.ReceiveRedisCommand(System.Net.Sockets.Socket)" />
        protected async Task<OperateResult<byte[]>> ReceiveRedisCommandAsync(
          Socket socket)
        {
            List<byte> bufferArray = new List<byte>();
            OperateResult<byte[]> readCommandLine = await this.ReceiveCommandLineFromSocketAsync(socket, (byte)10);
            if (!readCommandLine.IsSuccess)
                return readCommandLine;
            bufferArray.AddRange((IEnumerable<byte>)readCommandLine.Content);
            if (readCommandLine.Content[0] == (byte)43 || readCommandLine.Content[0] == (byte)45 || readCommandLine.Content[0] == (byte)58)
                return OperateResult.CreateSuccessResult<byte[]>(bufferArray.ToArray());
            if (readCommandLine.Content[0] == (byte)36)
            {
                OperateResult<int> lengthResult = RedisHelper.GetNumberFromCommandLine(readCommandLine.Content);
                if (!lengthResult.IsSuccess)
                    return OperateResult.CreateFailedResult<byte[]>((OperateResult)lengthResult);
                if (lengthResult.Content < 0)
                    return OperateResult.CreateSuccessResult<byte[]>(bufferArray.ToArray());
                OperateResult<byte[]> receiveContent = await this.ReceiveRedisCommandStringAsync(socket, lengthResult.Content);
                if (!receiveContent.IsSuccess)
                    return receiveContent;
                bufferArray.AddRange((IEnumerable<byte>)receiveContent.Content);
                return OperateResult.CreateSuccessResult<byte[]>(bufferArray.ToArray());
            }
            if (readCommandLine.Content[0] != (byte)42)
                return new OperateResult<byte[]>("Not Supported HeadCode: " + readCommandLine.Content[0].ToString());
            OperateResult<int> lengthResult1 = RedisHelper.GetNumberFromCommandLine(readCommandLine.Content);
            if (!lengthResult1.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)lengthResult1);
            for (int i = 0; i < lengthResult1.Content; ++i)
            {
                OperateResult<byte[]> receiveCommand = await this.ReceiveRedisCommandAsync(socket);
                if (!receiveCommand.IsSuccess)
                    return receiveCommand;
                bufferArray.AddRange((IEnumerable<byte>)receiveCommand.Content);
                receiveCommand = (OperateResult<byte[]>)null;
            }
            return OperateResult.CreateSuccessResult<byte[]>(bufferArray.ToArray());
        }

        /// <summary>
        /// 接收一条hsl协议的数据信息，自动解析，解压，解码操作，获取最后的实际的数据，接收结果依次为暗号，用户码，负载数据<br />
        /// Receive a piece of hsl protocol data information, automatically parse, decompress, and decode operations to obtain the last actual data.
        /// The result is a opCode, user code, and payload data in order.
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <returns>接收结果，依次为暗号，用户码，负载数据</returns>
        protected OperateResult<int, int, byte[]> ReceiveEstMessage(Socket socket)
        {
            OperateResult<byte[]> operateResult1 = this.Receive(socket, 32, 10000);
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<int, int, byte[]>((OperateResult)operateResult1);
            int int32 = BitConverter.ToInt32(operateResult1.Content, operateResult1.Content.Length - 4);
            OperateResult<byte[]> operateResult2 = this.Receive(socket, int32);
            if (!operateResult2.IsSuccess)
                return OperateResult.CreateFailedResult<int, int, byte[]>((OperateResult)operateResult2);
            byte[] numArray = EstProtocol.CommandAnalysis(operateResult1.Content, operateResult2.Content);
            return OperateResult.CreateSuccessResult<int, int, byte[]>(BitConverter.ToInt32(operateResult1.Content, 0), BitConverter.ToInt32(operateResult1.Content, 4), numArray);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.Net.NetworkBase.ReceiveEstMessage(System.Net.Sockets.Socket)" />
        protected async Task<OperateResult<int, int, byte[]>> ReceiveEstMessageAsync(
          Socket socket)
        {
            OperateResult<byte[]> receiveHead = await this.ReceiveAsync(socket, 32, 10000);
            if (!receiveHead.IsSuccess)
                return OperateResult.CreateFailedResult<int, int, byte[]>((OperateResult)receiveHead);
            int receive_length = BitConverter.ToInt32(receiveHead.Content, receiveHead.Content.Length - 4);
            OperateResult<byte[]> receiveContent = await this.ReceiveAsync(socket, receive_length);
            if (!receiveContent.IsSuccess)
                return OperateResult.CreateFailedResult<int, int, byte[]>((OperateResult)receiveContent);
            byte[] Content = EstProtocol.CommandAnalysis(receiveHead.Content, receiveContent.Content);
            int protocol = BitConverter.ToInt32(receiveHead.Content, 0);
            int customer = BitConverter.ToInt32(receiveHead.Content, 4);
            return OperateResult.CreateSuccessResult<int, int, byte[]>(protocol, customer, Content);
        }

        /// <summary>
        /// 删除文件的操作<br />
        /// Delete file operation
        /// </summary>
        /// <param name="filename">完整的真实的文件路径</param>
        /// <returns>是否删除成功</returns>
        protected bool DeleteFileByName(string filename)
        {
            try
            {
                if (!System.IO.File.Exists(filename))
                    return true;
                System.IO.File.Delete(filename);
                return true;
            }
            catch (Exception ex)
            {
                this.LogNet?.WriteException(this.ToString(), "delete file failed:" + filename, ex);
                return false;
            }
        }

        /// <summary>
        /// 预处理文件夹的名称，除去文件夹名称最后一个'\'或'/'，如果有的话<br />
        /// Preprocess the name of the folder, removing the last '\' or '/' in the folder name
        /// </summary>
        /// <param name="folder">文件夹名称</param>
        /// <returns>返回处理之后的名称</returns>
        protected string PreprocessFolderName(string folder) => folder.EndsWith("\\") || folder.EndsWith("/") ? folder.Substring(0, folder.Length - 1) : folder;

        /// <inheritdoc />
        public override string ToString() => nameof(NetworkBase);

        /// <summary>
        /// 通过主机名或是IP地址信息，获取到真实的IP地址信息<br />
        /// Obtain the real IP address information through the host name or IP address information
        /// </summary>
        /// <param name="hostName">主机名或是IP地址</param>
        /// <returns>IP地址信息</returns>
        public static string GetIpAddressHostName(string hostName) => Dns.GetHostEntry(hostName).AddressList[0].ToString();
    }
}
