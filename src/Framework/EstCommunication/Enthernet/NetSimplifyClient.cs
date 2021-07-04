// Decompiled with JetBrains decompiler
// Type: EstCommunication.Enthernet.NetSimplifyClient
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.Core;
using EstCommunication.Core.IMessage;
using EstCommunication.Core.Net;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EstCommunication.Enthernet
{
  /// <summary>同步访问数据的客户端类，用于向服务器请求一些确定的数据信息</summary>
  /// <remarks>
  /// 详细的使用说明，请参照博客<a href="http://www.cnblogs.com/dathlin/p/7697782.html">http://www.cnblogs.com/dathlin/p/7697782.html</a>
  /// </remarks>
  /// <example>
  /// 此处贴上了Demo项目的服务器配置的示例代码
  /// <code lang="cs" source="TestProject\EstCommunicationDemo\Est\FormSimplifyNet.cs" region="FormSimplifyNet" title="FormSimplifyNet示例" />
  /// </example>
  public class NetSimplifyClient : NetworkDoubleBase
  {
    /// <summary>实例化一个客户端的对象，用于和服务器通信</summary>
    /// <param name="ipAddress">服务器的ip地址</param>
    /// <param name="port">服务器的端口号</param>
    public NetSimplifyClient(string ipAddress, int port)
    {
      this.ByteTransform = (IByteTransform) new RegularByteTransform();
      this.IpAddress = ipAddress;
      this.Port = port;
    }

    /// <summary>实例化一个客户端的对象，用于和服务器通信</summary>
    /// <param name="ipAddress">服务器的ip地址</param>
    /// <param name="port">服务器的端口号</param>
    public NetSimplifyClient(IPAddress ipAddress, int port)
    {
      this.ByteTransform = (IByteTransform) new RegularByteTransform();
      this.IpAddress = ipAddress.ToString();
      this.Port = port;
    }

    /// <summary>实例化一个客户端对象，需要手动指定Ip地址和端口</summary>
    public NetSimplifyClient() => this.ByteTransform = (IByteTransform) new RegularByteTransform();

    /// <inheritdoc />
    protected override INetMessage GetNewNetMessage() => (INetMessage) new EstMessage();

    /// <inheritdoc />
    protected override OperateResult InitializationOnConnect(Socket socket) => this.isUseAccountCertificate ? this.AccountCertificate(socket) : OperateResult.CreateSuccessResult();

    /// <inheritdoc />
    protected override async Task<OperateResult> InitializationOnConnectAsync(
      Socket socket)
    {
      if (!this.isUseAccountCertificate)
        return OperateResult.CreateSuccessResult();
      OperateResult operateResult = await this.AccountCertificateAsync(socket);
      return operateResult;
    }

    /// <summary>客户端向服务器进行请求，请求字符串数据，忽略了自定义消息反馈</summary>
    /// <param name="customer">用户的指令头</param>
    /// <param name="send">发送数据</param>
    /// <returns>带返回消息的结果对象</returns>
    public OperateResult<string> ReadFromServer(NetHandle customer, string send)
    {
      OperateResult<byte[]> operateResult = this.ReadFromServerBase(EstProtocol.CommandBytes((int) customer, this.Token, send));
      return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<string>((OperateResult) operateResult) : OperateResult.CreateSuccessResult<string>(Encoding.Unicode.GetString(operateResult.Content));
    }

    /// <summary>客户端向服务器进行请求，请求字符串数组，忽略了自定义消息反馈</summary>
    /// <param name="customer">用户的指令头</param>
    /// <param name="send">发送数据</param>
    /// <returns>带返回消息的结果对象</returns>
    public OperateResult<string[]> ReadFromServer(NetHandle customer, string[] send)
    {
      OperateResult<byte[]> operateResult = this.ReadFromServerBase(EstProtocol.CommandBytes((int) customer, this.Token, send));
      return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<string[]>((OperateResult) operateResult) : OperateResult.CreateSuccessResult<string[]>(EstProtocol.UnPackStringArrayFromByte(operateResult.Content));
    }

    /// <summary>客户端向服务器进行请求，请求字节数据</summary>
    /// <param name="customer">用户的指令头</param>
    /// <param name="send">发送的字节内容</param>
    /// <returns>带返回消息的结果对象</returns>
    public OperateResult<byte[]> ReadFromServer(NetHandle customer, byte[] send) => this.ReadFromServerBase(EstProtocol.CommandBytes((int) customer, this.Token, send));

    /// <summary>客户端向服务器进行请求，请求字符串数据，并返回状态信息</summary>
    /// <param name="customer">用户的指令头</param>
    /// <param name="send">发送数据</param>
    /// <returns>带返回消息的结果对象</returns>
    public OperateResult<NetHandle, string> ReadCustomerFromServer(
      NetHandle customer,
      string send)
    {
      OperateResult<NetHandle, byte[]> operateResult = this.ReadCustomerFromServerBase(EstProtocol.CommandBytes((int) customer, this.Token, send));
      return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<NetHandle, string>((OperateResult) operateResult) : OperateResult.CreateSuccessResult<NetHandle, string>(operateResult.Content1, Encoding.Unicode.GetString(operateResult.Content2));
    }

    /// <summary>客户端向服务器进行请求，请求字符串数据，并返回状态信息</summary>
    /// <param name="customer">用户的指令头</param>
    /// <param name="send">发送数据</param>
    /// <returns>带返回消息的结果对象</returns>
    public OperateResult<NetHandle, string[]> ReadCustomerFromServer(
      NetHandle customer,
      string[] send)
    {
      OperateResult<NetHandle, byte[]> operateResult = this.ReadCustomerFromServerBase(EstProtocol.CommandBytes((int) customer, this.Token, send));
      return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<NetHandle, string[]>((OperateResult) operateResult) : OperateResult.CreateSuccessResult<NetHandle, string[]>(operateResult.Content1, EstProtocol.UnPackStringArrayFromByte(operateResult.Content2));
    }

    /// <summary>客户端向服务器进行请求，请求字符串数据，并返回状态信息</summary>
    /// <param name="customer">用户的指令头</param>
    /// <param name="send">发送数据</param>
    /// <returns>带返回消息的结果对象</returns>
    public OperateResult<NetHandle, byte[]> ReadCustomerFromServer(
      NetHandle customer,
      byte[] send)
    {
      return this.ReadCustomerFromServerBase(EstProtocol.CommandBytes((int) customer, this.Token, send));
    }

    /// <summary>需要发送的底层数据</summary>
    /// <param name="send">需要发送的底层数据</param>
    /// <returns>带返回消息的结果对象</returns>
    private OperateResult<byte[]> ReadFromServerBase(byte[] send)
    {
      OperateResult<NetHandle, byte[]> operateResult = this.ReadCustomerFromServerBase(send);
      return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult) : OperateResult.CreateSuccessResult<byte[]>(operateResult.Content2);
    }

    /// <summary>需要发送的底层数据</summary>
    /// <param name="send">需要发送的底层数据</param>
    /// <returns>带返回消息的结果对象</returns>
    private OperateResult<NetHandle, byte[]> ReadCustomerFromServerBase(
      byte[] send)
    {
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(send);
      return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<NetHandle, byte[]>((OperateResult) operateResult) : EstProtocol.ExtractEstData(operateResult.Content);
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.NetSimplifyClient.ReadFromServer(EstCommunication.NetHandle,System.String)" />
    public async Task<OperateResult<string>> ReadFromServerAsync(
      NetHandle customer,
      string send)
    {
      OperateResult<byte[]> read = await this.ReadFromServerBaseAsync(EstProtocol.CommandBytes((int) customer, this.Token, send));
      OperateResult<string> operateResult = read.IsSuccess ? OperateResult.CreateSuccessResult<string>(Encoding.Unicode.GetString(read.Content)) : OperateResult.CreateFailedResult<string>((OperateResult) read);
      read = (OperateResult<byte[]>) null;
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.NetSimplifyClient.ReadFromServer(EstCommunication.NetHandle,System.String[])" />
    public async Task<OperateResult<string[]>> ReadFromServerAsync(
      NetHandle customer,
      string[] send)
    {
      OperateResult<byte[]> read = await this.ReadFromServerBaseAsync(EstProtocol.CommandBytes((int) customer, this.Token, send));
      OperateResult<string[]> operateResult = read.IsSuccess ? OperateResult.CreateSuccessResult<string[]>(EstProtocol.UnPackStringArrayFromByte(read.Content)) : OperateResult.CreateFailedResult<string[]>((OperateResult) read);
      read = (OperateResult<byte[]>) null;
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.NetSimplifyClient.ReadFromServer(EstCommunication.NetHandle,System.Byte[])" />
    public async Task<OperateResult<byte[]>> ReadFromServerAsync(
      NetHandle customer,
      byte[] send)
    {
      OperateResult<byte[]> operateResult = await this.ReadFromServerBaseAsync(EstProtocol.CommandBytes((int) customer, this.Token, send));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.NetSimplifyClient.ReadCustomerFromServer(EstCommunication.NetHandle,System.String)" />
    public async Task<OperateResult<NetHandle, string>> ReadCustomerFromServerAsync(
      NetHandle customer,
      string send)
    {
      OperateResult<NetHandle, byte[]> read = await this.ReadCustomerFromServerBaseAsync(EstProtocol.CommandBytes((int) customer, this.Token, send));
      OperateResult<NetHandle, string> operateResult = read.IsSuccess ? OperateResult.CreateSuccessResult<NetHandle, string>(read.Content1, Encoding.Unicode.GetString(read.Content2)) : OperateResult.CreateFailedResult<NetHandle, string>((OperateResult) read);
      read = (OperateResult<NetHandle, byte[]>) null;
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.NetSimplifyClient.ReadCustomerFromServer(EstCommunication.NetHandle,System.String[])" />
    public async Task<OperateResult<NetHandle, string[]>> ReadCustomerFromServerAsync(
      NetHandle customer,
      string[] send)
    {
      OperateResult<NetHandle, byte[]> read = await this.ReadCustomerFromServerBaseAsync(EstProtocol.CommandBytes((int) customer, this.Token, send));
      OperateResult<NetHandle, string[]> operateResult = read.IsSuccess ? OperateResult.CreateSuccessResult<NetHandle, string[]>(read.Content1, EstProtocol.UnPackStringArrayFromByte(read.Content2)) : OperateResult.CreateFailedResult<NetHandle, string[]>((OperateResult) read);
      read = (OperateResult<NetHandle, byte[]>) null;
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.NetSimplifyClient.ReadCustomerFromServer(EstCommunication.NetHandle,System.Byte[])" />
    public async Task<OperateResult<NetHandle, byte[]>> ReadCustomerFromServerAsync(
      NetHandle customer,
      byte[] send)
    {
      OperateResult<NetHandle, byte[]> operateResult = await this.ReadCustomerFromServerBaseAsync(EstProtocol.CommandBytes((int) customer, this.Token, send));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.NetSimplifyClient.ReadFromServerBase(System.Byte[])" />
    private async Task<OperateResult<byte[]>> ReadFromServerBaseAsync(byte[] send)
    {
      OperateResult<NetHandle, byte[]> read = await this.ReadCustomerFromServerBaseAsync(send);
      OperateResult<byte[]> operateResult = read.IsSuccess ? OperateResult.CreateSuccessResult<byte[]>(read.Content2) : OperateResult.CreateFailedResult<byte[]>((OperateResult) read);
      read = (OperateResult<NetHandle, byte[]>) null;
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Enthernet.NetSimplifyClient.ReadCustomerFromServerBase(System.Byte[])" />
    private async Task<OperateResult<NetHandle, byte[]>> ReadCustomerFromServerBaseAsync(
      byte[] send)
    {
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(send);
      OperateResult<NetHandle, byte[]> operateResult = read.IsSuccess ? EstProtocol.ExtractEstData(read.Content) : OperateResult.CreateFailedResult<NetHandle, byte[]>((OperateResult) read);
      read = (OperateResult<byte[]>) null;
      return operateResult;
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("NetSimplifyClient[{0}:{1}]", (object) this.IpAddress, (object) this.Port);
  }
}
