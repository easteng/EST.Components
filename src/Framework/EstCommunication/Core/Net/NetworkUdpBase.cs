// Decompiled with JetBrains decompiler
// Type: EstCommunication.Core.Net.NetworkUdpBase
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace EstCommunication.Core.Net
{
  /// <summary>
  /// 基于Udp的应答式通信类<br />
  /// Udp - based responsive communication class
  /// </summary>
  public class NetworkUdpBase : NetworkBase
  {
    private SimpleHybirdLock hybirdLock = (SimpleHybirdLock) null;
    private int connectErrorCount = 0;
    private string ipAddress = "127.0.0.1";

    /// <summary>
    /// 实例化一个默认的方法<br />
    /// Instantiate a default method
    /// </summary>
    public NetworkUdpBase()
    {
      this.hybirdLock = new SimpleHybirdLock();
      this.ReceiveTimeout = 5000;
      this.ConnectionId = SoftBasic.GetUniqueStringByGuidAndRandom();
    }

    /// <inheritdoc cref="P:EstCommunication.Core.Net.NetworkDoubleBase.IpAddress" />
    public virtual string IpAddress
    {
      get => this.ipAddress;
      set => this.ipAddress = EstHelper.GetIpAddressFromInput(value);
    }

    /// <inheritdoc cref="P:EstCommunication.Core.Net.NetworkDoubleBase.Port" />
    public virtual int Port { get; set; }

    /// <inheritdoc cref="P:EstCommunication.Core.Net.NetworkDoubleBase.ReceiveTimeOut" />
    public int ReceiveTimeout { get; set; }

    /// <inheritdoc cref="P:EstCommunication.Core.Net.NetworkDoubleBase.ConnectionId" />
    public string ConnectionId { get; set; }

    /// <summary>
    /// 获取或设置一次接收时的数据长度，默认2KB数据长度，特殊情况的时候需要调整<br />
    /// Gets or sets the length of data received at a time. The default length is 2KB
    /// </summary>
    public int ReceiveCacheLength { get; set; } = 2048;

    /// <inheritdoc cref="P:EstCommunication.Core.Net.NetworkDoubleBase.LocalBinding" />
    public IPEndPoint LocalBinding { get; set; }

    /// <summary>
    /// 核心的数据交互读取，发数据发送到串口上去，然后从串口上接收返回的数据<br />
    /// The core data is read interactively, the data is sent to the serial port, and the returned data is received from the serial port
    /// </summary>
    /// <param name="value">完整的报文内容</param>
    /// <returns>是否成功的结果对象</returns>
    public virtual OperateResult<byte[]> ReadFromCoreServer(byte[] value)
    {
      if (!EstCommunication.Authorization.nzugaydgwadawdibbas())
        return new OperateResult<byte[]>(StringResources.Language.AuthorizationFailed);
      this.LogNet?.WriteDebug(this.ToString(), StringResources.Language.Send + " : " + SoftBasic.ByteToHexString(value));
      this.hybirdLock.Enter();
      try
      {
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(this.IpAddress), this.Port);
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        if (this.LocalBinding != null)
          socket.Bind((EndPoint) this.LocalBinding);
        socket.SendTo(value, value.Length, SocketFlags.None, (EndPoint) ipEndPoint);
        if (this.ReceiveTimeout < 0)
        {
          this.hybirdLock.Leave();
          return OperateResult.CreateSuccessResult<byte[]>(new byte[0]);
        }
        socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, this.ReceiveTimeout);
        EndPoint remoteEP = (EndPoint) new IPEndPoint(IPAddress.Any, 0);
        byte[] buffer = new byte[this.ReceiveCacheLength];
        int from = socket.ReceiveFrom(buffer, ref remoteEP);
        byte[] array = ((IEnumerable<byte>) buffer).Take<byte>(from).ToArray<byte>();
        this.hybirdLock.Leave();
        this.LogNet?.WriteDebug(this.ToString(), StringResources.Language.Receive + " : " + SoftBasic.ByteToHexString(array));
        this.connectErrorCount = 0;
        return OperateResult.CreateSuccessResult<byte[]>(array);
      }
      catch (Exception ex)
      {
        this.hybirdLock.Leave();
        if (this.connectErrorCount < 100000000)
          ++this.connectErrorCount;
        return new OperateResult<byte[]>(-this.connectErrorCount, ex.Message);
      }
    }

    /// <inheritdoc cref="M:EstCommunication.Core.Net.NetworkDoubleBase.IpAddressPing" />
    public IPStatus IpAddressPing() => new Ping().Send(this.IpAddress).Status;

    /// <inheritdoc />
    public override string ToString() => string.Format("NetworkUdpBase[{0}:{1}]", (object) this.IpAddress, (object) this.Port);
  }
}
