// Decompiled with JetBrains decompiler
// Type: EstCommunication.Core.Net.NetworkDoubleBase
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using EstCommunication.Core.IMessage;
using EstCommunication.Reflection;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EstCommunication.Core.Net
{
  /// <summary>
  /// 支持长连接，短连接两个模式的通用客户端基类 <br />
  /// Universal client base class that supports long connections and short connections to two modes
  /// </summary>
  /// <example>无，请使用继承类实例化，然后进行数据交互，当前的类并没有具体的实现。</example>
  public class NetworkDoubleBase : NetworkBase, IDisposable
  {
    private IByteTransform byteTransform;
    private string ipAddress = "127.0.0.1";
    private int port = 10000;
    private int connectTimeOut = 10000;
    private string connectionId = string.Empty;
    private bool isUseSpecifiedSocket = false;
    /// <summary>
    /// 接收数据的超时时间，单位：毫秒
    /// <br />
    /// Timeout for receiving data, unit: millisecond
    /// </summary>
    protected int receiveTimeOut = 5000;
    /// <summary>
    /// 是否是长连接的状态<br />
    /// Whether it is a long connection state
    /// </summary>
    protected bool isPersistentConn = false;
    /// <summary>
    /// 交互的混合锁，保证交互操作的安全性<br />
    /// Interactive hybrid locks to ensure the security of interactive operations
    /// </summary>
    protected SimpleHybirdLock InteractiveLock;
    /// <summary>
    /// 指示长连接的套接字是否处于错误的状态<br />
    /// Indicates if the long-connected socket is in the wrong state
    /// </summary>
    protected bool IsSocketError = false;
    /// <summary>
    /// 设置日志记录报文是否二进制，如果为False，那就使用ASCII码<br />
    /// Set whether the log message is binary, if it is False, then use ASCII code
    /// </summary>
    protected bool LogMsgFormatBinary = true;
    /// <summary>
    /// 是否使用账号登录，这个账户登录的功能是<c>HSL</c>组件创建的服务器特有的功能。<br />
    /// Whether to log in using an account. The function of this account login is a server-specific function created by the <c> HSL </c> component.
    /// </summary>
    protected bool isUseAccountCertificate = false;
    private string userName = string.Empty;
    private string password = string.Empty;
    private bool disposedValue = false;
    private Lazy<Ping> ping = new Lazy<Ping>((Func<Ping>) (() => new Ping()));

    /// <summary>
    /// 默认的无参构造函数 <br />
    /// Default no-parameter constructor
    /// </summary>
    public NetworkDoubleBase()
    {
      this.InteractiveLock = new SimpleHybirdLock();
      this.connectionId = SoftBasic.GetUniqueStringByGuidAndRandom();
    }

    /// <summary>
    /// 获取一个新的消息对象的方法，需要在继承类里面进行重写<br />
    /// The method to get a new message object needs to be overridden in the inheritance class
    /// </summary>
    /// <returns>消息类对象</returns>
    protected virtual INetMessage GetNewNetMessage() => (INetMessage) null;

    /// <summary>
    /// 当前的数据变换机制，当你需要从字节数据转换类型数据的时候需要。<br />
    /// The current data transformation mechanism is required when you need to convert type data from byte data.
    /// </summary>
    /// <example>
    /// 主要是用来转换数据类型的，下面仅仅演示了2个方法，其他的类型转换，类似处理。
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDoubleBase.cs" region="ByteTransform" title="ByteTransform示例" />
    /// </example>
    public IByteTransform ByteTransform
    {
      get => this.byteTransform;
      set => this.byteTransform = value;
    }

    /// <summary>
    /// 获取或设置连接的超时时间，单位是毫秒 <br />
    /// Gets or sets the timeout for the connection, in milliseconds
    /// </summary>
    /// <example>
    /// 设置1秒的超时的示例
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDoubleBase.cs" region="ConnectTimeOutExample" title="ConnectTimeOut示例" />
    /// </example>
    /// <remarks>不适用于异形模式的连接。</remarks>
    [EstMqttApi(Description = "Gets or sets the timeout for the connection, in milliseconds", HttpMethod = "GET")]
    public virtual int ConnectTimeOut
    {
      get => this.connectTimeOut;
      set
      {
        if (value < 0)
          return;
        this.connectTimeOut = value;
      }
    }

    /// <summary>
    /// 获取或设置接收服务器反馈的时间，如果为负数，则不接收反馈 <br />
    /// Gets or sets the time to receive server feedback, and if it is a negative number, does not receive feedback
    /// </summary>
    /// <example>
    /// 设置1秒的接收超时的示例
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDoubleBase.cs" region="ReceiveTimeOutExample" title="ReceiveTimeOut示例" />
    /// </example>
    /// <remarks>超时的通常原因是服务器端没有配置好，导致访问失败，为了不卡死软件，所以有了这个超时的属性。</remarks>
    [EstMqttApi(Description = "Gets or sets the time to receive server feedback, and if it is a negative number, does not receive feedback", HttpMethod = "GET")]
    public int ReceiveTimeOut
    {
      get => this.receiveTimeOut;
      set => this.receiveTimeOut = value;
    }

    /// <summary>
    /// 获取或是设置远程服务器的IP地址，如果是本机测试，那么需要设置为127.0.0.1 <br />
    /// Get or set the IP address of the remote server. If it is a local test, then it needs to be set to 127.0.0.1
    /// </summary>
    /// <remarks>最好实在初始化的时候进行指定，当使用短连接的时候，支持动态更改，切换；当使用长连接后，无法动态更改</remarks>
    /// <example>
    /// 以下举例modbus-tcp的短连接及动态更改ip地址的示例
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDoubleBase.cs" region="IpAddressExample" title="IpAddress示例" />
    /// </example>
    [EstMqttApi(Description = "Get or set the IP address of the remote server. If it is a local test, then it needs to be set to 127.0.0.1", HttpMethod = "GET")]
    public virtual string IpAddress
    {
      get => this.ipAddress;
      set => this.ipAddress = EstHelper.GetIpAddressFromInput(value);
    }

    /// <summary>
    /// 获取或设置服务器的端口号，具体的值需要取决于对方的配置<br />
    /// Gets or sets the port number of the server. The specific value depends on the configuration of the other party.
    /// </summary>
    /// <remarks>最好实在初始化的时候进行指定，当使用短连接的时候，支持动态更改，切换；当使用长连接后，无法动态更改</remarks>
    /// <example>动态更改请参照IpAddress属性的更改。</example>
    [EstMqttApi(Description = "Gets or sets the port number of the server. The specific value depends on the configuration of the other party.", HttpMethod = "GET")]
    public virtual int Port
    {
      get => this.port;
      set => this.port = value;
    }

    /// <inheritdoc cref="P:EstCommunication.Core.IReadWriteNet.ConnectionId" />
    [EstMqttApi(Description = "The unique ID number of the current connection. The default is a 20-digit guid code plus a random number.", HttpMethod = "GET")]
    public string ConnectionId
    {
      get => this.connectionId;
      set => this.connectionId = value;
    }

    /// <summary>
    /// 获取或设置在正式接收对方返回数据前的时候，需要休息的时间，当设置为0的时候，不需要休息。<br />
    /// Get or set the time required to rest before officially receiving the data from the other party. When it is set to 0, no rest is required.
    /// </summary>
    [EstMqttApi(Description = "Get or set the time required to rest before officially receiving the data from the other party. When it is set to 0, no rest is required.", HttpMethod = "GET")]
    public int SleepTime { get; set; }

    /// <summary>
    /// 获取或设置绑定的本地的IP地址和端口号信息，如果端口设置为0，代表任何可用的端口<br />
    /// Get or set the bound local IP address and port number information, if the port is set to 0, it means any available port
    /// </summary>
    public IPEndPoint LocalBinding { get; set; }

    /// <summary>
    /// 当前的异形连接对象，如果设置了异形连接的话，仅用于异形模式的情况使用<br />
    /// The current alien connection object, if alien connection is set, is only used in the case of alien mode
    /// </summary>
    /// <remarks>具体的使用方法请参照Demo项目中的异形modbus实现。</remarks>
    public AlienSession AlienSession { get; set; }

    /// <summary>
    /// 在读取数据之前可以调用本方法将客户端设置为长连接模式，相当于跳过了ConnectServer的结果验证，对异形客户端无效，当第一次进行通信时再进行创建连接请求。<br />
    /// Before reading the data, you can call this method to set the client to the long connection mode, which is equivalent to skipping the result verification of ConnectServer,
    /// and it is invalid for the alien client. When the first communication is performed, the connection creation request is performed.
    /// </summary>
    /// <example>
    /// 以下的方式演示了另一种长连接的机制
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDoubleBase.cs" region="SetPersistentConnectionExample" title="SetPersistentConnection示例" />
    /// </example>
    public void SetPersistentConnection() => this.isPersistentConn = true;

    /// <summary>
    /// 对当前设备的IP地址进行PING的操作，返回PING的结果，正常来说，返回<see cref="F:System.Net.NetworkInformation.IPStatus.Success" /><br />
    /// PING the IP address of the current device and return the PING result. Normally, it returns <see cref="F:System.Net.NetworkInformation.IPStatus.Success" />
    /// </summary>
    /// <returns>返回PING的结果</returns>
    public IPStatus IpAddressPing() => this.ping.Value.Send(this.IpAddress).Status;

    /// <summary>
    /// 尝试连接远程的服务器，如果连接成功，就切换短连接模式到长连接模式，后面的每次请求都共享一个通道，使得通讯速度更快速<br />
    /// Try to connect to a remote server. If the connection is successful, switch the short connection mode to the long connection mode.
    /// Each subsequent request will share a channel, making the communication speed faster.
    /// </summary>
    /// <returns>返回连接结果，如果失败的话（也即IsSuccess为False），包含失败信息</returns>
    /// <example>
    ///   简单的连接示例，调用该方法后，连接设备，创建一个长连接的对象，后续的读写操作均公用一个连接对象。
    ///   <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDoubleBase.cs" region="Connect1" title="连接设备" />
    ///   如果想知道是否连接成功，请参照下面的代码。
    ///   <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDoubleBase.cs" region="Connect2" title="判断连接结果" />
    /// </example>
    public OperateResult ConnectServer()
    {
      this.isPersistentConn = true;
      this.CoreSocket?.Close();
      OperateResult<Socket> andInitialication = this.CreateSocketAndInitialication();
      if (!andInitialication.IsSuccess)
      {
        this.IsSocketError = true;
        andInitialication.Content = (Socket) null;
        return (OperateResult) andInitialication;
      }
      this.CoreSocket = andInitialication.Content;
      this.LogNet?.WriteDebug(this.ToString(), StringResources.Language.NetEngineStart);
      return (OperateResult) andInitialication;
    }

    /// <summary>
    /// 使用指定的套接字创建异形客户端，在异形客户端的模式下，网络通道需要被动创建。<br />
    /// Use the specified socket to create the alien client. In the alien client mode, the network channel needs to be created passively.
    /// </summary>
    /// <param name="session">异形客户端对象，查看<seealso cref="T:EstCommunication.Core.Net.NetworkAlienClient" />类型创建的客户端</param>
    /// <returns>通常都为成功</returns>
    /// <example>
    ///   简单的创建示例。
    ///   <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDoubleBase.cs" region="AlienConnect1" title="连接设备" />
    ///   如果想知道是否创建成功。通常都是成功。
    ///   <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDoubleBase.cs" region="AlienConnect2" title="判断连接结果" />
    /// </example>
    /// <remarks>不能和之前的长连接和短连接混用，详细参考 Demo程序</remarks>
    public OperateResult ConnectServer(AlienSession session)
    {
      this.isPersistentConn = true;
      this.isUseSpecifiedSocket = true;
      if (session != null)
      {
        this.AlienSession?.Socket?.Close();
        if (string.IsNullOrEmpty(this.ConnectionId))
          this.ConnectionId = session.DTU;
        if (this.ConnectionId == session.DTU)
        {
          this.CoreSocket = session.Socket;
          this.IsSocketError = !session.IsStatusOk;
          this.AlienSession = session;
          return session.IsStatusOk ? this.InitializationOnConnect(session.Socket) : new OperateResult();
        }
        this.IsSocketError = true;
        return new OperateResult();
      }
      this.IsSocketError = true;
      return new OperateResult();
    }

    /// <summary>
    /// 手动断开与远程服务器的连接，如果当前是长连接模式，那么就会切换到短连接模式<br />
    /// Manually disconnect from the remote server, if it is currently in long connection mode, it will switch to short connection mode
    /// </summary>
    /// <returns>关闭连接，不需要查看IsSuccess属性查看</returns>
    /// <example>
    /// 直接关闭连接即可，基本上是不需要进行成功的判定
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDoubleBase.cs" region="ConnectCloseExample" title="关闭连接结果" />
    /// </example>
    public OperateResult ConnectClose()
    {
      OperateResult operateResult1 = new OperateResult();
      this.isPersistentConn = false;
      this.InteractiveLock.Enter();
      OperateResult operateResult2;
      try
      {
        operateResult2 = this.ExtraOnDisconnect(this.CoreSocket);
        this.CoreSocket?.Close();
        this.CoreSocket = (Socket) null;
        this.InteractiveLock.Leave();
      }
      catch
      {
        this.InteractiveLock.Leave();
        throw;
      }
      this.LogNet?.WriteDebug(this.ToString(), StringResources.Language.NetEngineClose);
      return operateResult2;
    }

    /// <summary>
    /// 根据实际的协议选择是否重写本方法，有些协议在创建连接之后，需要进行一些初始化的信号握手，才能最终建立网络通道。<br />
    /// Whether to rewrite this method is based on the actual protocol. Some protocols require some initial signal handshake to establish a network channel after the connection is created.
    /// </summary>
    /// <param name="socket">网络套接字</param>
    /// <returns>是否初始化成功，依据具体的协议进行重写</returns>
    /// <example>
    /// 有些协议不需要握手信号，比如三菱的MC协议，Modbus协议，西门子和欧姆龙就存在握手信息，此处的例子是继承本类后重写的西门子的协议示例
    /// <code lang="cs" source="EstCommunication_Net45\Profinet\Siemens\SiemensS7Net.cs" region="NetworkDoubleBase Override" title="西门子重连示例" />
    /// </example>
    protected virtual OperateResult InitializationOnConnect(Socket socket) => OperateResult.CreateSuccessResult();

    /// <summary>
    /// 根据实际的协议选择是否重写本方法，有些协议在断开连接之前，需要发送一些报文来关闭当前的网络通道<br />
    /// Select whether to rewrite this method according to the actual protocol. Some protocols need to send some packets to close the current network channel before disconnecting.
    /// </summary>
    /// <param name="socket">网络套接字</param>
    /// <example>目前暂无相关的示例，组件支持的协议都不用实现这个方法。</example>
    /// <returns>当断开连接时额外的操作结果</returns>
    protected virtual OperateResult ExtraOnDisconnect(Socket socket) => OperateResult.CreateSuccessResult();

    /// <summary>
    /// 和服务器交互完成的时候调用的方法，可以根据读写结果进行一些额外的操作，具体的操作需要根据实际的需求来重写实现<br />
    /// The method called when the interaction with the server is completed can perform some additional operations based on the read and write results.
    /// The specific operations need to be rewritten according to actual needs.
    /// </summary>
    /// <param name="read">读取结果</param>
    protected virtual void ExtraAfterReadFromCoreServer(OperateResult read)
    {
    }

    /// <summary>
    /// 设置当前的登录的账户名和密码信息，并启用账户验证的功能，账户名为空时设置不生效<br />
    /// Set the current login account name and password information, and enable the account verification function. The account name setting will not take effect when it is empty
    /// </summary>
    /// <param name="userName">账户名</param>
    /// <param name="password">密码</param>
    public void SetLoginAccount(string userName, string password)
    {
      if (!string.IsNullOrEmpty(userName.Trim()))
      {
        this.isUseAccountCertificate = true;
        this.userName = userName;
        this.password = password;
      }
      else
        this.isUseAccountCertificate = false;
    }

    /// <summary>
    /// 认证账号，根据已经设置的用户名和密码，进行发送服务器进行账号认证。<br />
    /// Authentication account, according to the user name and password that have been set, sending server for account authentication.
    /// </summary>
    /// <param name="socket">套接字</param>
    /// <returns>认证结果</returns>
    protected OperateResult AccountCertificate(Socket socket)
    {
      OperateResult operateResult = this.SendAccountAndCheckReceive(socket, 1, this.userName, this.password);
      if (!operateResult.IsSuccess)
        return operateResult;
      OperateResult<int, string[]> contentFromSocket = this.ReceiveStringArrayContentFromSocket(socket);
      if (!contentFromSocket.IsSuccess)
        return (OperateResult) contentFromSocket;
      return contentFromSocket.Content1 == 0 ? new OperateResult(contentFromSocket.Content2[0]) : OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc cref="M:EstCommunication.Core.Net.NetworkDoubleBase.AccountCertificate(System.Net.Sockets.Socket)" />
    protected async Task<OperateResult> AccountCertificateAsync(Socket socket)
    {
      OperateResult send = await this.SendAccountAndCheckReceiveAsync(socket, 1, this.userName, this.password);
      if (!send.IsSuccess)
        return send;
      OperateResult<int, string[]> read = await this.ReceiveStringArrayContentFromSocketAsync(socket);
      return read.IsSuccess ? (read.Content1 != 0 ? OperateResult.CreateSuccessResult() : new OperateResult(read.Content2[0])) : (OperateResult) read;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.Net.NetworkDoubleBase.InitializationOnConnect(System.Net.Sockets.Socket)" />
    protected virtual async Task<OperateResult> InitializationOnConnectAsync(
      Socket socket)
    {
      return OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc cref="M:EstCommunication.Core.Net.NetworkDoubleBase.ExtraOnDisconnect(System.Net.Sockets.Socket)" />
    protected virtual async Task<OperateResult> ExtraOnDisconnectAsync(
      Socket socket)
    {
      return OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc cref="M:EstCommunication.Core.Net.NetworkDoubleBase.CreateSocketAndInitialication" />
    private async Task<OperateResult<Socket>> CreateSocketAndInitialicationAsync()
    {
      OperateResult<Socket> result = await this.CreateSocketAndConnectAsync(new IPEndPoint(IPAddress.Parse(this.ipAddress), this.port), this.connectTimeOut, this.LocalBinding);
      if (result.IsSuccess)
      {
        OperateResult initi = await this.InitializationOnConnectAsync(result.Content);
        if (!initi.IsSuccess)
        {
          result.Content?.Close();
          result.IsSuccess = initi.IsSuccess;
          result.CopyErrorFromOther<OperateResult>(initi);
        }
        initi = (OperateResult) null;
      }
      OperateResult<Socket> operateResult = result;
      result = (OperateResult<Socket>) null;
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.Net.NetworkDoubleBase.GetAvailableSocket" />
    protected async Task<OperateResult<Socket>> GetAvailableSocketAsync()
    {
      if (this.isPersistentConn)
      {
        if (this.isUseSpecifiedSocket)
          return !this.IsSocketError ? OperateResult.CreateSuccessResult<Socket>(this.CoreSocket) : new OperateResult<Socket>(StringResources.Language.ConnectionIsNotAvailable);
        if (!this.IsSocketError && this.CoreSocket != null)
          return OperateResult.CreateSuccessResult<Socket>(this.CoreSocket);
        OperateResult connect = await this.ConnectServerAsync();
        if (!connect.IsSuccess)
        {
          this.IsSocketError = true;
          return OperateResult.CreateFailedResult<Socket>(connect);
        }
        this.IsSocketError = false;
        return OperateResult.CreateSuccessResult<Socket>(this.CoreSocket);
      }
      OperateResult<Socket> initialicationAsync = await this.CreateSocketAndInitialicationAsync();
      return initialicationAsync;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.Net.NetworkDoubleBase.ConnectServer" />
    public async Task<OperateResult> ConnectServerAsync()
    {
      this.isPersistentConn = true;
      this.CoreSocket?.Close();
      OperateResult<Socket> rSocket = await this.CreateSocketAndInitialicationAsync();
      if (!rSocket.IsSuccess)
      {
        this.IsSocketError = true;
        rSocket.Content = (Socket) null;
        return (OperateResult) rSocket;
      }
      this.CoreSocket = rSocket.Content;
      this.LogNet?.WriteDebug(this.ToString(), StringResources.Language.NetEngineStart);
      return (OperateResult) rSocket;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.Net.NetworkDoubleBase.ConnectClose" />
    public async Task<OperateResult> ConnectCloseAsync()
    {
      OperateResult result = new OperateResult();
      this.isPersistentConn = false;
      this.InteractiveLock.Enter();
      try
      {
        result = await this.ExtraOnDisconnectAsync(this.CoreSocket);
        this.CoreSocket?.Close();
        this.CoreSocket = (Socket) null;
        this.InteractiveLock.Leave();
      }
      catch
      {
        this.InteractiveLock.Leave();
        throw;
      }
      this.LogNet?.WriteDebug(this.ToString(), StringResources.Language.NetEngineClose);
      OperateResult operateResult = result;
      result = (OperateResult) null;
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.Net.NetworkDoubleBase.ReadFromCoreServer(System.Net.Sockets.Socket,System.Byte[],System.Boolean,System.Boolean)" />
    public virtual async Task<OperateResult<byte[]>> ReadFromCoreServerAsync(
      Socket socket,
      byte[] send,
      bool hasResponseData = true,
      bool usePackHeader = true)
    {
      this.LogNet?.WriteDebug(this.ToString(), StringResources.Language.Send + " : " + (this.LogMsgFormatBinary ? send.ToHexString(' ') : Encoding.ASCII.GetString(send)));
      INetMessage netMessage = this.GetNewNetMessage();
      if (netMessage != null)
        netMessage.SendBytes = send;
      OperateResult sendResult = await this.SendAsync(socket, usePackHeader ? this.PackCommandWithHeader(send) : send);
      if (!sendResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>(sendResult);
      if (this.receiveTimeOut < 0)
        return OperateResult.CreateSuccessResult<byte[]>(new byte[0]);
      if (!hasResponseData)
        return OperateResult.CreateSuccessResult<byte[]>(new byte[0]);
      if (this.SleepTime > 0)
        Thread.Sleep(this.SleepTime);
      OperateResult<byte[]> resultReceive = await this.ReceiveByMessageAsync(socket, this.receiveTimeOut, netMessage);
      if (!resultReceive.IsSuccess)
        return resultReceive;
      this.LogNet?.WriteDebug(this.ToString(), StringResources.Language.Receive + " : " + (this.LogMsgFormatBinary ? resultReceive.Content.ToHexString(' ') : Encoding.ASCII.GetString(resultReceive.Content)));
      if (netMessage == null || netMessage.CheckHeadBytesLegal(this.Token.ToByteArray()))
        return OperateResult.CreateSuccessResult<byte[]>(resultReceive.Content);
      socket?.Close();
      return new OperateResult<byte[]>(StringResources.Language.CommandHeadCodeCheckFailed + Environment.NewLine + StringResources.Language.Send + ": " + SoftBasic.ByteToHexString(send, ' ') + Environment.NewLine + StringResources.Language.Receive + ": " + SoftBasic.ByteToHexString(resultReceive.Content, ' '));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.Net.NetworkDoubleBase.ReadFromCoreServer(System.Byte[],System.Boolean)" />
    public async Task<OperateResult<byte[]>> ReadFromCoreServerAsync(byte[] send)
    {
      OperateResult<byte[]> operateResult = await this.ReadFromCoreServerAsync(send, true);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.Net.NetworkDoubleBase.ReadFromCoreServer(System.Byte[],System.Boolean)" />
    public async Task<OperateResult<byte[]>> ReadFromCoreServerAsync(
      byte[] send,
      bool hasResponseData)
    {
      OperateResult<byte[]> result = new OperateResult<byte[]>();
      OperateResult<Socket> resultSocket = (OperateResult<Socket>) null;
      this.InteractiveLock.Enter();
      try
      {
        resultSocket = await this.GetAvailableSocketAsync();
        if (!resultSocket.IsSuccess)
        {
          this.IsSocketError = true;
          this.AlienSession?.Offline();
          this.InteractiveLock.Leave();
          result.CopyErrorFromOther<OperateResult<Socket>>(resultSocket);
          return result;
        }
        OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(resultSocket.Content, send, hasResponseData);
        if (read.IsSuccess)
        {
          this.IsSocketError = false;
          result.IsSuccess = read.IsSuccess;
          result.Content = read.Content;
          result.Message = StringResources.Language.SuccessText;
        }
        else
        {
          this.IsSocketError = true;
          this.AlienSession?.Offline();
          result.CopyErrorFromOther<OperateResult<byte[]>>(read);
        }
        this.ExtraAfterReadFromCoreServer((OperateResult) read);
        this.InteractiveLock.Leave();
        read = (OperateResult<byte[]>) null;
      }
      catch
      {
        this.InteractiveLock.Leave();
        throw;
      }
      if (!this.isPersistentConn)
        resultSocket?.Content?.Close();
      return result;
    }

    /// <summary>对当前的命令进行打包处理，通常是携带命令头内容，标记当前的命令的长度信息，需要进行重写，否则默认不打包</summary>
    /// <param name="command">发送的数据命令内容</param>
    /// <returns>打包之后的数据结果信息</returns>
    protected virtual byte[] PackCommandWithHeader(byte[] command) => command;

    /// <summary>
    /// 获取本次操作的可用的网络通道，如果是短连接，就重新生成一个新的网络通道，如果是长连接，就复用当前的网络通道。<br />
    /// Obtain the available network channels for this operation. If it is a short connection, a new network channel is regenerated.
    /// If it is a long connection, the current network channel is reused.
    /// </summary>
    /// <returns>是否成功，如果成功，使用这个套接字</returns>
    protected OperateResult<Socket> GetAvailableSocket()
    {
      if (!this.isPersistentConn)
        return this.CreateSocketAndInitialication();
      if (this.isUseSpecifiedSocket)
        return this.IsSocketError ? new OperateResult<Socket>(StringResources.Language.ConnectionIsNotAvailable) : OperateResult.CreateSuccessResult<Socket>(this.CoreSocket);
      if (!this.IsSocketError && this.CoreSocket != null)
        return OperateResult.CreateSuccessResult<Socket>(this.CoreSocket);
      OperateResult result = this.ConnectServer();
      if (!result.IsSuccess)
      {
        this.IsSocketError = true;
        return OperateResult.CreateFailedResult<Socket>(result);
      }
      this.IsSocketError = false;
      return OperateResult.CreateSuccessResult<Socket>(this.CoreSocket);
    }

    /// <summary>
    /// 尝试连接服务器，如果成功，并执行<see cref="M:EstCommunication.Core.Net.NetworkDoubleBase.InitializationOnConnect(System.Net.Sockets.Socket)" />的初始化方法，并返回最终的结果。<br />
    /// Attempt to connect to the server, if successful, and execute the initialization method of <see cref="M:EstCommunication.Core.Net.NetworkDoubleBase.InitializationOnConnect(System.Net.Sockets.Socket)" />, and return the final result.
    /// </summary>
    /// <returns>带有socket的结果对象</returns>
    private OperateResult<Socket> CreateSocketAndInitialication()
    {
      OperateResult<Socket> socketAndConnect = this.CreateSocketAndConnect(new IPEndPoint(IPAddress.Parse(this.ipAddress), this.port), this.connectTimeOut, this.LocalBinding);
      if (socketAndConnect.IsSuccess)
      {
        OperateResult result = this.InitializationOnConnect(socketAndConnect.Content);
        if (!result.IsSuccess)
        {
          socketAndConnect.Content?.Close();
          socketAndConnect.IsSuccess = result.IsSuccess;
          socketAndConnect.CopyErrorFromOther<OperateResult>(result);
        }
      }
      return socketAndConnect;
    }

    /// <summary>
    /// 将数据报文发送指定的网络通道上，根据当前指定的<see cref="T:EstCommunication.Core.IMessage.INetMessage" />类型，返回一条完整的数据指令<br />
    /// Sends a data message to the specified network channel, and returns a complete data command according to the currently specified <see cref="T:EstCommunication.Core.IMessage.INetMessage" /> type
    /// </summary>
    /// <param name="socket">指定的套接字</param>
    /// <param name="send">发送的完整的报文信息</param>
    /// <param name="hasResponseData">是否有等待的数据返回，默认为 true</param>
    /// <param name="usePackHeader">是否需要对命令重新打包，在重写<see cref="M:EstCommunication.Core.Net.NetworkDoubleBase.PackCommandWithHeader(System.Byte[])" />方法后才会有影响</param>
    /// <remarks>无锁的基于套接字直接进行叠加协议的操作。</remarks>
    /// <example>
    /// 假设你有一个自己的socket连接了设备，本组件可以直接基于该socket实现modbus读取，三菱读取，西门子读取等等操作，前提是该服务器支持多协议，虽然这个需求听上去比较变态，但本组件支持这样的操作。
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDoubleBase.cs" region="ReadFromCoreServerExample1" title="ReadFromCoreServer示例" />
    /// </example>
    /// <returns>接收的完整的报文信息</returns>
    public virtual OperateResult<byte[]> ReadFromCoreServer(
      Socket socket,
      byte[] send,
      bool hasResponseData = true,
      bool usePackHeader = true)
    {
      this.LogNet?.WriteDebug(this.ToString(), StringResources.Language.Send + " : " + (this.LogMsgFormatBinary ? send.ToHexString(' ') : Encoding.ASCII.GetString(send)));
      INetMessage newNetMessage = this.GetNewNetMessage();
      if (newNetMessage != null)
        newNetMessage.SendBytes = send;
      OperateResult result = this.Send(socket, usePackHeader ? this.PackCommandWithHeader(send) : send);
      if (!result.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>(result);
      if (this.receiveTimeOut < 0)
        return OperateResult.CreateSuccessResult<byte[]>(new byte[0]);
      if (!hasResponseData)
        return OperateResult.CreateSuccessResult<byte[]>(new byte[0]);
      if (this.SleepTime > 0)
        Thread.Sleep(this.SleepTime);
      OperateResult<byte[]> byMessage = this.ReceiveByMessage(socket, this.receiveTimeOut, newNetMessage);
      if (!byMessage.IsSuccess)
        return byMessage;
      this.LogNet?.WriteDebug(this.ToString(), StringResources.Language.Receive + " : " + (this.LogMsgFormatBinary ? byMessage.Content.ToHexString(' ') : Encoding.ASCII.GetString(byMessage.Content)));
      if (newNetMessage == null || newNetMessage.CheckHeadBytesLegal(this.Token.ToByteArray()))
        return OperateResult.CreateSuccessResult<byte[]>(byMessage.Content);
      socket?.Close();
      return new OperateResult<byte[]>(StringResources.Language.CommandHeadCodeCheckFailed + Environment.NewLine + StringResources.Language.Send + ": " + SoftBasic.ByteToHexString(send, ' ') + Environment.NewLine + StringResources.Language.Receive + ": " + SoftBasic.ByteToHexString(byMessage.Content, ' '));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.Net.NetworkDoubleBase.ReadFromCoreServer(System.Byte[],System.Boolean)" />
    public OperateResult<byte[]> ReadFromCoreServer(byte[] send) => this.ReadFromCoreServer(send, true);

    /// <summary>
    /// 将数据发送到当前的网络通道中，并从网络通道中接收一个<see cref="T:EstCommunication.Core.IMessage.INetMessage" />指定的完整的报文，网络通道将根据<see cref="M:EstCommunication.Core.Net.NetworkDoubleBase.GetAvailableSocket" />方法自动获取，本方法是线程安全的。<br />
    /// Send data to the current network channel and receive a complete message specified by <see cref="T:EstCommunication.Core.IMessage.INetMessage" /> from the network channel.
    /// The network channel will be automatically obtained according to the <see cref="M:EstCommunication.Core.Net.NetworkDoubleBase.GetAvailableSocket" /> method This method is thread-safe.
    /// </summary>
    /// <param name="send">发送的完整的报文信息</param>
    /// <param name="hasResponseData">是否有等待的数据返回，默认为 true</param>
    /// <returns>接收的完整的报文信息</returns>
    /// <remarks>
    /// 本方法用于实现本组件还未实现的一些报文功能，例如有些modbus服务器会有一些特殊的功能码支持，需要收发特殊的报文，详细请看示例
    /// </remarks>
    /// <example>
    /// 此处举例有个modbus服务器，有个特殊的功能码0x09，后面携带子数据0x01即可，发送字节为 0x00 0x00 0x00 0x00 0x00 0x03 0x01 0x09 0x01
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDoubleBase.cs" region="ReadFromCoreServerExample2" title="ReadFromCoreServer示例" />
    /// </example>
    public OperateResult<byte[]> ReadFromCoreServer(byte[] send, bool hasResponseData)
    {
      OperateResult<byte[]> operateResult = new OperateResult<byte[]>();
      this.InteractiveLock.Enter();
      OperateResult<Socket> availableSocket;
      try
      {
        availableSocket = this.GetAvailableSocket();
        if (!availableSocket.IsSuccess)
        {
          this.IsSocketError = true;
          this.AlienSession?.Offline();
          this.InteractiveLock.Leave();
          operateResult.CopyErrorFromOther<OperateResult<Socket>>(availableSocket);
          return operateResult;
        }
        OperateResult<byte[]> result = this.ReadFromCoreServer(availableSocket.Content, send, hasResponseData);
        if (result.IsSuccess)
        {
          this.IsSocketError = false;
          operateResult.IsSuccess = result.IsSuccess;
          operateResult.Content = result.Content;
          operateResult.Message = StringResources.Language.SuccessText;
        }
        else
        {
          this.IsSocketError = true;
          this.AlienSession?.Offline();
          operateResult.CopyErrorFromOther<OperateResult<byte[]>>(result);
        }
        this.ExtraAfterReadFromCoreServer((OperateResult) result);
        this.InteractiveLock.Leave();
      }
      catch
      {
        this.InteractiveLock.Leave();
        throw;
      }
      if (!this.isPersistentConn && availableSocket != null)
        availableSocket.Content?.Close();
      return operateResult;
    }

    /// <summary>释放当前的资源，并自动关闭长连接，如果设置了的话</summary>
    /// <param name="disposing">是否释放托管的资源信息</param>
    protected virtual void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      if (disposing)
      {
        this.ConnectClose();
        this.InteractiveLock?.Dispose();
      }
      this.disposedValue = true;
    }

    /// <summary>释放当前的资源</summary>
    public void Dispose() => this.Dispose(true);

    /// <inheritdoc />
    public override string ToString() => string.Format("NetworkDoubleBase<{0}, {1}>[{2}:{3}]", (object) this.GetNewNetMessage().GetType(), (object) this.ByteTransform.GetType(), (object) this.IpAddress, (object) this.Port);
  }
}
