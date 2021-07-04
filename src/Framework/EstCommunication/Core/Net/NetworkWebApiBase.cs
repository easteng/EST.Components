// Decompiled with JetBrains decompiler
// Type: EstCommunication.Core.Net.NetworkWebApiBase
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.LogNet;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EstCommunication.Core.Net
{
  /// <summary>
  /// 基于webapi的数据访问的基类，提供了基本的http接口的交互功能<br />
  /// A base class for data access based on webapi that provides basic HTTP interface interaction
  /// </summary>
  /// <remarks>
  /// 当前的基类在.net framework上存在问题，在.net framework4.5及.net standard上运行稳定而且正常
  /// </remarks>
  public class NetworkWebApiBase
  {
    private string ipAddress = "127.0.0.1";
    private int port = 80;
    private string name = string.Empty;
    private string password = string.Empty;
    private HttpClient httpClient;

    /// <summary>
    /// 使用指定的ip地址来初始化对象<br />
    /// Initializes the object using the specified IP address
    /// </summary>
    /// <param name="ipAddress">Ip地址信息</param>
    public NetworkWebApiBase(string ipAddress)
    {
      this.ipAddress = ipAddress;
      this.httpClient = new HttpClient();
    }

    /// <summary>
    /// 使用指定的ip地址及端口号来初始化对象<br />
    /// Initializes the object with the specified IP address and port number
    /// </summary>
    /// <param name="ipAddress">Ip地址信息</param>
    /// <param name="port">端口号信息</param>
    public NetworkWebApiBase(string ipAddress, int port)
    {
      this.ipAddress = ipAddress;
      this.port = port;
      this.httpClient = new HttpClient();
    }

    /// <summary>
    /// 使用指定的ip地址，端口号，用户名，密码来初始化对象<br />
    /// Initialize the object with the specified IP address, port number, username, and password
    /// </summary>
    /// <param name="ipAddress">Ip地址信息</param>
    /// <param name="port">端口号信息</param>
    /// <param name="name">用户名</param>
    /// <param name="password">密码</param>
    public NetworkWebApiBase(string ipAddress, int port, string name, string password)
    {
      this.ipAddress = EstHelper.GetIpAddressFromInput(ipAddress);
      this.port = port;
      this.name = name;
      this.password = password;
      if (!string.IsNullOrEmpty(name))
      {
        HttpClientHandler httpClientHandler = new HttpClientHandler()
        {
          Credentials = (ICredentials) new NetworkCredential(name, password)
        };
        httpClientHandler.Proxy = (IWebProxy) null;
        httpClientHandler.UseProxy = false;
        this.httpClient = new HttpClient((HttpMessageHandler) httpClientHandler);
      }
      else
        this.httpClient = new HttpClient();
    }

    /// <summary>
    /// 等待重写的额外的指令信息的支持。除了url的形式之外，还支持基于命令的数据交互<br />
    /// Additional instruction information waiting for rewriting is supported.In addition to the url format, command based data interaction is supported
    /// </summary>
    /// <param name="address">地址信息</param>
    /// <returns>是否读取成功的内容</returns>
    protected virtual OperateResult<string> ReadByAddress(string address) => new OperateResult<string>(StringResources.Language.NotSupportedFunction);

    /// <inheritdoc cref="M:EstCommunication.Core.Net.NetworkWebApiBase.ReadByAddress(System.String)" />
    protected virtual async Task<OperateResult<string>> ReadByAddressAsync(
      string address)
    {
      return new OperateResult<string>(StringResources.Language.NotSupportedFunction);
    }

    /// <summary>
    /// 读取对方信息的的数据信息，通常是针对GET的方法信息设计的。如果使用了url=开头，就表示是使用了原生的地址访问<br />
    /// Read the other side of the data information, usually designed for the GET method information.If you start with url=, you are using native address access
    /// </summary>
    /// <param name="address">无效参数</param>
    /// <returns>带有成功标识的byte[]数组</returns>
    public virtual OperateResult<byte[]> Read(string address)
    {
      OperateResult<string> operateResult = this.ReadString(address);
      return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult) : OperateResult.CreateSuccessResult<byte[]>(Encoding.UTF8.GetBytes(operateResult.Content));
    }

    /// <summary>
    /// 读取对方信息的的字符串数据信息，通常是针对GET的方法信息设计的。如果使用了url=开头，就表示是使用了原生的地址访问<br />
    /// The string data information that reads the other party information, usually designed for the GET method information.If you start with url=, you are using native address access
    /// </summary>
    /// <param name="address">地址信息</param>
    /// <returns>带有成功标识的字符串数据</returns>
    public virtual OperateResult<string> ReadString(string address)
    {
      if (!EstCommunication.Authorization.nzugaydgwadawdibbas())
        return new OperateResult<string>(StringResources.Language.AuthorizationFailed);
      if (!address.StartsWith("url=") && !address.StartsWith("URL="))
        return this.ReadByAddress(address);
      address = address.Substring(4);
      string requestUri = string.Format("http://{0}:{1}/{2}", (object) this.ipAddress, (object) this.port, address.StartsWith("/") ? (object) address.Substring(1) : (object) address);
      try
      {
        using (HttpResponseMessage result = this.httpClient.GetAsync(requestUri).Result)
        {
          using (HttpContent content = result.Content)
          {
            result.EnsureSuccessStatusCode();
            return OperateResult.CreateSuccessResult<string>(content.ReadAsStringAsync().Result);
          }
        }
      }
      catch (Exception ex)
      {
        return new OperateResult<string>(ex.Message);
      }
    }

    /// <summary>
    /// 使用POST的方式来向对方进行请求数据信息，需要使用url=开头，来表示是使用了原生的地址访问<br />
    /// Using POST to request data information from the other party, we need to start with url= to indicate that we are using native address access
    /// </summary>
    /// <param name="address">指定的地址信息，有些设备可能不支持</param>
    /// <param name="value">原始的字节数据信息</param>
    /// <returns>是否成功的写入</returns>
    public virtual OperateResult Write(string address, byte[] value) => this.Write(address, Encoding.Default.GetString(value));

    /// <summary>
    /// 使用POST的方式来向对方进行请求数据信息，需要使用url=开头，来表示是使用了原生的地址访问<br />
    /// Using POST to request data information from the other party, we need to start with url= to indicate that we are using native address access
    /// </summary>
    /// <param name="address">指定的地址信息</param>
    /// <param name="value">字符串的数据信息</param>
    /// <returns>是否成功的写入</returns>
    public virtual OperateResult Write(string address, string value)
    {
      if (!address.StartsWith("url=") && !address.StartsWith("URL="))
        return (OperateResult) new OperateResult<string>(StringResources.Language.NotSupportedFunction);
      address = address.Substring(4);
      string requestUri = string.Format("http://{0}:{1}/{2}", (object) this.ipAddress, (object) this.port, address.StartsWith("/") ? (object) address.Substring(1) : (object) address);
      try
      {
        using (StringContent stringContent = new StringContent(value))
        {
          using (HttpResponseMessage result = this.httpClient.PostAsync(requestUri, (HttpContent) stringContent).Result)
          {
            using (HttpContent content = result.Content)
            {
              result.EnsureSuccessStatusCode();
              return (OperateResult) OperateResult.CreateSuccessResult<string>(content.ReadAsStringAsync().Result);
            }
          }
        }
      }
      catch (Exception ex)
      {
        return (OperateResult) new OperateResult<string>(ex.Message);
      }
    }

    /// <inheritdoc cref="M:EstCommunication.Core.Net.NetworkWebApiBase.Read(System.String)" />
    public virtual async Task<OperateResult<byte[]>> ReadAsync(string address)
    {
      OperateResult<string> read = await this.ReadStringAsync(address);
      OperateResult<byte[]> operateResult = read.IsSuccess ? OperateResult.CreateSuccessResult<byte[]>(Encoding.UTF8.GetBytes(read.Content)) : OperateResult.CreateFailedResult<byte[]>((OperateResult) read);
      read = (OperateResult<string>) null;
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.Net.NetworkWebApiBase.ReadString(System.String)" />
    public virtual async Task<OperateResult<string>> ReadStringAsync(string address)
    {
      if (!EstCommunication.Authorization.nzugaydgwadawdibbas())
        return new OperateResult<string>(StringResources.Language.AuthorizationFailed);
      if (address.StartsWith("url=") || address.StartsWith("URL="))
      {
        address = address.Substring(4);
        string url = string.Format("http://{0}:{1}/{2}", (object) this.ipAddress, (object) this.port, address.StartsWith("/") ? (object) address.Substring(1) : (object) address);
        try
        {
          using (HttpResponseMessage response = await this.httpClient.GetAsync(url))
          {
            using (HttpContent content = response.Content)
            {
              response.EnsureSuccessStatusCode();
              string result = await content.ReadAsStringAsync();
              return OperateResult.CreateSuccessResult<string>(result);
            }
          }
        }
        catch (Exception ex)
        {
          return new OperateResult<string>(ex.Message);
        }
      }
      else
      {
        OperateResult<string> operateResult = await this.ReadByAddressAsync(address);
        return operateResult;
      }
    }

    /// <inheritdoc cref="M:EstCommunication.Core.Net.NetworkWebApiBase.Write(System.String,System.Byte[])" />
    public virtual async Task<OperateResult> WriteAsync(
      string address,
      byte[] value)
    {
      OperateResult operateResult = await this.WriteAsync(address, Encoding.Default.GetString(value));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.Net.NetworkWebApiBase.Write(System.String,System.String)" />
    public virtual async Task<OperateResult> WriteAsync(
      string address,
      string value)
    {
      if (!address.StartsWith("url=") && !address.StartsWith("URL="))
        return (OperateResult) new OperateResult<string>(StringResources.Language.NotSupportedFunction);
      address = address.Substring(4);
      string url = string.Format("http://{0}:{1}/{2}", (object) this.ipAddress, (object) this.port, address.StartsWith("/") ? (object) address.Substring(1) : (object) address);
      try
      {
        using (StringContent stringContent = new StringContent(value))
        {
          using (HttpResponseMessage response = await this.httpClient.PostAsync(url, (HttpContent) stringContent))
          {
            using (HttpContent content = response.Content)
            {
              response.EnsureSuccessStatusCode();
              string result = await content.ReadAsStringAsync();
              return (OperateResult) OperateResult.CreateSuccessResult<string>(result);
            }
          }
        }
      }
      catch (Exception ex)
      {
        return (OperateResult) new OperateResult<string>(ex.Message);
      }
    }

    /// <summary>
    /// 获取或设置远程服务器的IP地址<br />
    /// Gets or sets the IP address of the remote server
    /// </summary>
    public string IpAddress
    {
      get => this.ipAddress;
      set => this.ipAddress = value;
    }

    /// <summary>
    /// 获取或设置远程服务器的端口号信息<br />
    /// Gets or sets the port number information for the remote server
    /// </summary>
    public int Port
    {
      get => this.port;
      set => this.port = value;
    }

    /// <summary>
    /// 获取或设置当前的用户名<br />
    /// Get or set the current username
    /// </summary>
    public string UserName
    {
      get => this.name;
      set => this.name = value;
    }

    /// <summary>
    /// 获取或设置当前的密码<br />
    /// Get or set the current password
    /// </summary>
    public string Password
    {
      get => this.password;
      set => this.password = value;
    }

    /// <inheritdoc cref="P:EstCommunication.Core.Net.NetworkBase.LogNet" />
    public ILogNet LogNet { get; set; }

    /// <summary>获取当前的HttpClinet的客户端</summary>
    public HttpClient Client => this.httpClient;

    /// <inheritdoc />
    public override string ToString() => string.Format("NetworkWebApiBase[{0}:{1}]", (object) this.ipAddress, (object) this.port);
  }
}
