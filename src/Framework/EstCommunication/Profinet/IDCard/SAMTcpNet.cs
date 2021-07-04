// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.IDCard.SAMTcpNet
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.Core;
using EstCommunication.Core.IMessage;
using EstCommunication.Core.Net;
using EstCommunication.Reflection;
using System.Threading.Tasks;

namespace EstCommunication.Profinet.IDCard
{
  /// <summary>
  /// 基于SAM协议的Tcp实现的网络类，支持读取身份证的数据信息，通过透传的形式实现，除了初始化和串口类不一致，调用方法是几乎一模一样的，详细参见API文档<br />
  /// The network class implemented by Tcp based on the SAM protocol supports reading ID card data information and is implemented in the form of transparent transmission.
  /// Except for the inconsistency between the initialization and the serial port class, the calling method is almost the same.
  /// See the API documentation for details
  /// </summary>
  /// <example>
  /// 在使用之前需要实例化当前的对象，然后根据实际的情况填写好串口的信息，否则连接不上去。
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\SAMSerialSample.cs" region="Sample3" title="实例化操作" />
  /// 在实际的读取，我们一般放在后台进行循环扫描的操作，参见下面的代码
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\SAMSerialSample.cs" region="Sample4" title="基本的读取操作" />
  /// 当然也支持全异步的操作了，就是方法的名称改改
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\SAMSerialSample.cs" region="Sample5" title="实例化操作" />
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\SAMSerialSample.cs" region="Sample6" title="基本的读取操作" />
  /// </example>
  public class SAMTcpNet : NetworkDoubleBase
  {
    /// <summary>
    /// 实例化一个默认的对象<br />
    /// Instantiate a default object
    /// </summary>
    public SAMTcpNet() => this.ByteTransform = (IByteTransform) new RegularByteTransform();

    /// <summary>
    /// 通过指定的ip地址以及端口来实例化对象<br />
    /// Instantiate the object with the specified IP address and port
    /// </summary>
    /// <param name="ipAddress">ip地址</param>
    /// <param name="port">端口号</param>
    public SAMTcpNet(string ipAddress, int port)
    {
      this.IpAddress = ipAddress;
      this.Port = port;
      this.ByteTransform = (IByteTransform) new RegularByteTransform();
    }

    /// <inheritdoc />
    protected override INetMessage GetNewNetMessage() => (INetMessage) new SAMMessage();

    /// <summary>
    /// 读取身份证设备的安全模块号<br />
    /// Read the security module number of the ID device
    /// </summary>
    /// <returns>结果数据内容</returns>
    [EstMqttApi]
    public OperateResult<string> ReadSafeModuleNumber()
    {
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(SAMSerial.PackToSAMCommand(SAMSerial.BuildReadCommand((byte) 18, byte.MaxValue, (byte[]) null)));
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<string>((OperateResult) operateResult);
      OperateResult result = SAMSerial.CheckADSCommandAndSum(operateResult.Content);
      return !result.IsSuccess ? OperateResult.CreateFailedResult<string>(result) : SAMSerial.ExtractSafeModuleNumber(operateResult.Content);
    }

    /// <summary>
    /// 检测安全模块状态<br />
    /// Detecting Security Module Status
    /// </summary>
    /// <returns>返回是否检测成功</returns>
    [EstMqttApi]
    public OperateResult CheckSafeModuleStatus()
    {
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(SAMSerial.PackToSAMCommand(SAMSerial.BuildReadCommand((byte) 18, byte.MaxValue, (byte[]) null)));
      if (!operateResult.IsSuccess)
        return (OperateResult) OperateResult.CreateFailedResult<string>((OperateResult) operateResult);
      OperateResult result = SAMSerial.CheckADSCommandAndSum(operateResult.Content);
      if (!result.IsSuccess)
        return (OperateResult) OperateResult.CreateFailedResult<string>(result);
      return operateResult.Content[9] != (byte) 144 ? new OperateResult(SAMSerial.GetErrorDescription((int) operateResult.Content[9])) : OperateResult.CreateSuccessResult();
    }

    /// <summary>
    /// 寻找卡片，并返回是否成功<br />
    /// Find cards and return success
    /// </summary>
    /// <returns>是否寻找成功</returns>
    [EstMqttApi]
    public OperateResult SearchCard()
    {
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(SAMSerial.PackToSAMCommand(SAMSerial.BuildReadCommand((byte) 32, (byte) 1, (byte[]) null)));
      if (!operateResult.IsSuccess)
        return (OperateResult) OperateResult.CreateFailedResult<string>((OperateResult) operateResult);
      OperateResult result = SAMSerial.CheckADSCommandAndSum(operateResult.Content);
      if (!result.IsSuccess)
        return (OperateResult) OperateResult.CreateFailedResult<string>(result);
      return operateResult.Content[9] != (byte) 159 ? new OperateResult(SAMSerial.GetErrorDescription((int) operateResult.Content[9])) : OperateResult.CreateSuccessResult();
    }

    /// <summary>
    /// 选择卡片，并返回是否成功<br />
    /// Select card and return success
    /// </summary>
    /// <returns>是否寻找成功</returns>
    [EstMqttApi]
    public OperateResult SelectCard()
    {
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(SAMSerial.PackToSAMCommand(SAMSerial.BuildReadCommand((byte) 32, (byte) 2, (byte[]) null)));
      if (!operateResult.IsSuccess)
        return (OperateResult) OperateResult.CreateFailedResult<string>((OperateResult) operateResult);
      OperateResult result = SAMSerial.CheckADSCommandAndSum(operateResult.Content);
      if (!result.IsSuccess)
        return (OperateResult) OperateResult.CreateFailedResult<string>(result);
      return operateResult.Content[9] != (byte) 144 ? new OperateResult(SAMSerial.GetErrorDescription((int) operateResult.Content[9])) : OperateResult.CreateSuccessResult();
    }

    /// <summary>
    /// 读取卡片，如果成功的话，就返回身份证的所有的信息<br />
    /// Read the card, if successful, return all the information of the ID cards
    /// </summary>
    /// <returns>是否寻找成功</returns>
    [EstMqttApi]
    public OperateResult<IdentityCard> ReadCard()
    {
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(SAMSerial.PackToSAMCommand(SAMSerial.BuildReadCommand((byte) 48, (byte) 1, (byte[]) null)));
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<IdentityCard>((OperateResult) operateResult);
      OperateResult result = SAMSerial.CheckADSCommandAndSum(operateResult.Content);
      return !result.IsSuccess ? OperateResult.CreateFailedResult<IdentityCard>(result) : SAMSerial.ExtractIdentityCard(operateResult.Content);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.IDCard.SAMTcpNet.ReadSafeModuleNumber" />
    public async Task<OperateResult<string>> ReadSafeModuleNumberAsync()
    {
      byte[] command = SAMSerial.PackToSAMCommand(SAMSerial.BuildReadCommand((byte) 18, byte.MaxValue, (byte[]) null));
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command);
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<string>((OperateResult) read);
      OperateResult check = SAMSerial.CheckADSCommandAndSum(read.Content);
      return check.IsSuccess ? SAMSerial.ExtractSafeModuleNumber(read.Content) : OperateResult.CreateFailedResult<string>(check);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.IDCard.SAMTcpNet.CheckSafeModuleStatus" />
    public async Task<OperateResult> CheckSafeModuleStatusAsync()
    {
      byte[] command = SAMSerial.PackToSAMCommand(SAMSerial.BuildReadCommand((byte) 18, byte.MaxValue, (byte[]) null));
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command);
      if (!read.IsSuccess)
        return (OperateResult) OperateResult.CreateFailedResult<string>((OperateResult) read);
      OperateResult check = SAMSerial.CheckADSCommandAndSum(read.Content);
      return check.IsSuccess ? (read.Content[9] == (byte) 144 ? OperateResult.CreateSuccessResult() : new OperateResult(SAMSerial.GetErrorDescription((int) read.Content[9]))) : (OperateResult) OperateResult.CreateFailedResult<string>(check);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.IDCard.SAMTcpNet.SearchCard" />
    public async Task<OperateResult> SearchCardAsync()
    {
      byte[] command = SAMSerial.PackToSAMCommand(SAMSerial.BuildReadCommand((byte) 32, (byte) 1, (byte[]) null));
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command);
      if (!read.IsSuccess)
        return (OperateResult) OperateResult.CreateFailedResult<string>((OperateResult) read);
      OperateResult check = SAMSerial.CheckADSCommandAndSum(read.Content);
      return check.IsSuccess ? (read.Content[9] == (byte) 159 ? OperateResult.CreateSuccessResult() : new OperateResult(SAMSerial.GetErrorDescription((int) read.Content[9]))) : (OperateResult) OperateResult.CreateFailedResult<string>(check);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.IDCard.SAMTcpNet.SelectCard" />
    public async Task<OperateResult> SelectCardAsync()
    {
      byte[] command = SAMSerial.PackToSAMCommand(SAMSerial.BuildReadCommand((byte) 32, (byte) 2, (byte[]) null));
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command);
      if (!read.IsSuccess)
        return (OperateResult) OperateResult.CreateFailedResult<string>((OperateResult) read);
      OperateResult check = SAMSerial.CheckADSCommandAndSum(read.Content);
      return check.IsSuccess ? (read.Content[9] == (byte) 144 ? OperateResult.CreateSuccessResult() : new OperateResult(SAMSerial.GetErrorDescription((int) read.Content[9]))) : (OperateResult) OperateResult.CreateFailedResult<string>(check);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.IDCard.SAMTcpNet.ReadCard" />
    public async Task<OperateResult<IdentityCard>> ReadCardAsync()
    {
      byte[] command = SAMSerial.PackToSAMCommand(SAMSerial.BuildReadCommand((byte) 48, (byte) 1, (byte[]) null));
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command);
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<IdentityCard>((OperateResult) read);
      OperateResult check = SAMSerial.CheckADSCommandAndSum(read.Content);
      return check.IsSuccess ? SAMSerial.ExtractIdentityCard(read.Content) : OperateResult.CreateFailedResult<IdentityCard>(check);
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("SAMTcpNet[{0}:{1}]", (object) this.IpAddress, (object) this.Port);
  }
}
