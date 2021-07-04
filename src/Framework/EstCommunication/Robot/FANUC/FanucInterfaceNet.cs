// Decompiled with JetBrains decompiler
// Type: EstCommunication.Robot.FANUC.FanucInterfaceNet
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using EstCommunication.Core;
using EstCommunication.Core.IMessage;
using EstCommunication.Core.Net;
using EstCommunication.Reflection;
using Newtonsoft.Json;
using System;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EstCommunication.Robot.FANUC
{
  /// <summary>
  /// Fanuc机器人的PC Interface实现，在R-30iB mate plus型号上测试通过，支持读写任意的数据，写入操作务必谨慎调用，写入数据不当造成生命财产损失，作者概不负责。读写任意的地址见api文档信息<br />
  /// The Fanuc robot's PC Interface implementation has been tested on R-30iB mate plus models. It supports reading and writing arbitrary data. The writing operation must be called carefully.
  /// Improper writing of data will cause loss of life and property. The author is not responsible. Read and write arbitrary addresses see api documentation information
  /// </summary>
  /// <remarks>
  /// 如果使用绝对地址进行访问的话，支持的地址格式如下：
  /// <list type="table">
  ///   <listheader>
  ///     <term>地址名称</term>
  ///     <term>地址代号</term>
  ///     <term>示例</term>
  ///     <term>地址进制</term>
  ///     <term>字操作</term>
  ///     <term>位操作</term>
  ///     <term>备注</term>
  ///   </listheader>
  ///   <item>
  ///     <term>数据寄存器</term>
  ///     <term>D</term>
  ///     <term>D100,D200</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>输入寄存器</term>
  ///     <term>AI</term>
  ///     <term>AI100,AI200</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>输出寄存器</term>
  ///     <term>AQ</term>
  ///     <term>AQ100,Q200</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>输入继电器</term>
  ///     <term>I</term>
  ///     <term>I100,I200</term>
  ///     <term>10</term>
  ///     <term>×</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>输出继电器</term>
  ///     <term>Q</term>
  ///     <term>Q100,Q200</term>
  ///     <term>10</term>
  ///     <term>×</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>中间继电器</term>
  ///     <term>M</term>
  ///     <term>M100,M200</term>
  ///     <term>10</term>
  ///     <term>×</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  /// </list>
  /// </remarks>
  /// <example>
  /// 我们先来看看简单的情况
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Robot\FANUC\FanucInterfaceNetSample.cs" region="Sample1" title="简单的读取" />
  /// 读取fanuc部分数据
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Robot\FANUC\FanucInterfaceNetSample.cs" region="Sample2" title="属性读取" />
  /// 最后是比较高级的任意数据读写
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Robot\FANUC\FanucInterfaceNetSample.cs" region="Sample3" title="复杂读取" />
  /// </example>
  public class FanucInterfaceNet : NetworkDeviceBase, IRobotNet, IReadWriteNet
  {
    private FanucData fanucDataRetain = (FanucData) null;
    private DateTime fanucDataRefreshTime = DateTime.Now.AddSeconds(-10.0);
    private PropertyInfo[] fanucDataPropertyInfo = typeof (FanucData).GetProperties();
    private byte[] connect_req = new byte[56];
    private byte[] session_req = new byte[56]
    {
      (byte) 8,
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 192,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 16,
      (byte) 14,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 1,
      (byte) 79,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0
    };

    /// <summary>
    /// 实例化一个默认的对象<br />
    /// Instantiate a default object
    /// </summary>
    public FanucInterfaceNet()
    {
      this.WordLength = (ushort) 1;
      this.ByteTransform = (IByteTransform) new RegularByteTransform();
    }

    /// <summary>
    /// 指定ip及端口来实例化一个默认的对象，端口默认60008<br />
    /// Specify the IP and port to instantiate a default object, the port defaults to 60008
    /// </summary>
    /// <param name="ipAddress">ip地址</param>
    /// <param name="port">端口号</param>
    public FanucInterfaceNet(string ipAddress, int port = 60008)
    {
      this.WordLength = (ushort) 1;
      this.IpAddress = ipAddress;
      this.Port = port;
      this.ByteTransform = (IByteTransform) new RegularByteTransform();
    }

    /// <inheritdoc />
    protected override INetMessage GetNewNetMessage() => (INetMessage) new FanucRobotMessage();

    /// <summary>
    /// 获取或设置当前客户端的ID信息，默认为1024<br />
    /// Gets or sets the ID information of the current client. The default is 1024.
    /// </summary>
    public int ClientId { get; private set; } = 1024;

    /// <summary>
    /// 获取或设置缓存的Fanuc数据的有效时间，对<see cref="M:EstCommunication.Robot.FANUC.FanucInterfaceNet.ReadString(System.String)" />方法有效，默认为100，单位毫秒。也即是在100ms内频繁读取机器人的属性数据的时候，优先读取缓存值，提高读取效率。<br />
    /// Gets or sets the valid time of the cached Fanuc data. It is valid for the <see cref="M:EstCommunication.Robot.FANUC.FanucInterfaceNet.ReadString(System.String)" /> method. The default is 100, in milliseconds.
    /// That is, when the attribute data of the robot is frequently read within 100ms, the cache value is preferentially read to improve the reading efficiency.
    /// </summary>
    public int FanucDataRetainTime { get; set; } = 100;

    private OperateResult ReadCommandFromRobot(Socket socket, string[] cmds)
    {
      for (int index = 0; index < cmds.Length; ++index)
      {
        byte[] bytes = Encoding.ASCII.GetBytes(cmds[index]);
        OperateResult<byte[]> operateResult = this.ReadFromCoreServer(socket, FanucHelper.BuildWriteData((byte) 56, (ushort) 1, bytes, bytes.Length));
        if (!operateResult.IsSuccess)
          return (OperateResult) operateResult;
      }
      return OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc />
    protected override OperateResult InitializationOnConnect(Socket socket)
    {
      BitConverter.GetBytes(this.ClientId).CopyTo((Array) this.connect_req, 1);
      OperateResult<byte[]> operateResult1 = this.ReadFromCoreServer(socket, this.connect_req);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(socket, this.session_req);
      return !operateResult2.IsSuccess ? (OperateResult) operateResult2 : this.ReadCommandFromRobot(socket, FanucHelper.GetFanucCmds());
    }

    private async Task<OperateResult> ReadCommandFromRobotAsync(
      Socket socket,
      string[] cmds)
    {
      for (int i = 0; i < cmds.Length; ++i)
      {
        byte[] buffer = Encoding.ASCII.GetBytes(cmds[i]);
        OperateResult<byte[]> write = await this.ReadFromCoreServerAsync(socket, FanucHelper.BuildWriteData((byte) 56, (ushort) 1, buffer, buffer.Length));
        if (!write.IsSuccess)
          return (OperateResult) write;
        buffer = (byte[]) null;
        write = (OperateResult<byte[]>) null;
      }
      return OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc />
    protected override async Task<OperateResult> InitializationOnConnectAsync(
      Socket socket)
    {
      BitConverter.GetBytes(this.ClientId).CopyTo((Array) this.connect_req, 1);
      OperateResult<byte[]> receive = await this.ReadFromCoreServerAsync(socket, this.connect_req);
      if (!receive.IsSuccess)
        return (OperateResult) receive;
      receive = await this.ReadFromCoreServerAsync(socket, this.session_req);
      if (!receive.IsSuccess)
        return (OperateResult) receive;
      OperateResult operateResult = await this.ReadCommandFromRobotAsync(socket, FanucHelper.GetFanucCmds());
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.Net.IRobotNet.Read(System.String)" />
    [EstMqttApi(ApiTopic = "ReadRobotByte", Description = "Read the robot's original byte data information according to the address")]
    public OperateResult<byte[]> Read(string address) => this.Read((byte) 8, (ushort) 1, (ushort) 6130);

    /// <inheritdoc cref="M:EstCommunication.Core.Net.IRobotNet.ReadString(System.String)" />
    [EstMqttApi(ApiTopic = "ReadRobotString", Description = "Read the string data information of the robot based on the address")]
    public OperateResult<string> ReadString(string address)
    {
      if (string.IsNullOrEmpty(address))
      {
        OperateResult<FanucData> operateResult = this.ReadFanucData();
        if (!operateResult.IsSuccess)
          return OperateResult.CreateFailedResult<string>((OperateResult) operateResult);
        this.fanucDataRetain = operateResult.Content;
        this.fanucDataRefreshTime = DateTime.Now;
        return OperateResult.CreateSuccessResult<string>(JsonConvert.SerializeObject((object) operateResult.Content, Formatting.Indented));
      }
      if ((DateTime.Now - this.fanucDataRefreshTime).TotalMilliseconds > (double) this.FanucDataRetainTime || this.fanucDataRetain == null)
      {
        OperateResult<FanucData> operateResult = this.ReadFanucData();
        if (!operateResult.IsSuccess)
          return OperateResult.CreateFailedResult<string>((OperateResult) operateResult);
        this.fanucDataRetain = operateResult.Content;
        this.fanucDataRefreshTime = DateTime.Now;
      }
      foreach (PropertyInfo propertyInfo in this.fanucDataPropertyInfo)
      {
        if (propertyInfo.Name == address)
          return OperateResult.CreateSuccessResult<string>(JsonConvert.SerializeObject(propertyInfo.GetValue((object) this.fanucDataRetain, (object[]) null), Formatting.Indented));
      }
      return new OperateResult<string>(StringResources.Language.NotSupportedDataType);
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.FANUC.FanucInterfaceNet.Read(System.String)" />
    public async Task<OperateResult<byte[]>> ReadAsync(string address)
    {
      OperateResult<byte[]> operateResult = await this.ReadAsync((byte) 8, (ushort) 1, (ushort) 6130);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.FANUC.FanucInterfaceNet.ReadString(System.String)" />
    public async Task<OperateResult<string>> ReadStringAsync(string address)
    {
      if (string.IsNullOrEmpty(address))
      {
        OperateResult<FanucData> read = await this.ReadFanucDataAsync();
        if (!read.IsSuccess)
          return OperateResult.CreateFailedResult<string>((OperateResult) read);
        this.fanucDataRetain = read.Content;
        this.fanucDataRefreshTime = DateTime.Now;
        return OperateResult.CreateSuccessResult<string>(JsonConvert.SerializeObject((object) read.Content, Formatting.Indented));
      }
      if ((DateTime.Now - this.fanucDataRefreshTime).TotalMilliseconds > (double) this.FanucDataRetainTime || this.fanucDataRetain == null)
      {
        OperateResult<FanucData> read = await this.ReadFanucDataAsync();
        if (!read.IsSuccess)
          return OperateResult.CreateFailedResult<string>((OperateResult) read);
        this.fanucDataRetain = read.Content;
        this.fanucDataRefreshTime = DateTime.Now;
        read = (OperateResult<FanucData>) null;
      }
      PropertyInfo[] propertyInfoArray = this.fanucDataPropertyInfo;
      for (int index = 0; index < propertyInfoArray.Length; ++index)
      {
        PropertyInfo item = propertyInfoArray[index];
        if (item.Name == address)
          return OperateResult.CreateSuccessResult<string>(JsonConvert.SerializeObject(item.GetValue((object) this.fanucDataRetain, (object[]) null), Formatting.Indented));
        item = (PropertyInfo) null;
      }
      propertyInfoArray = (PropertyInfo[]) null;
      return new OperateResult<string>(StringResources.Language.NotSupportedDataType);
    }

    /// <summary>
    /// 按照字为单位批量读取设备的原始数据，需要指定地址及长度，地址示例：D1，AI1，AQ1，共计3个区的数据，注意地址的起始为1<br />
    /// Read the raw data of the device in batches in units of words. You need to specify the address and length. Example addresses: D1, AI1, AQ1, a total of 3 areas of data. Note that the start of the address is 1.
    /// </summary>
    /// <param name="address">起始地址，地址示例：D1，AI1，AQ1，共计3个区的数据，注意起始的起始为1</param>
    /// <param name="length">读取的长度，字为单位</param>
    /// <returns>返回的数据信息结果</returns>
    [EstMqttApi("ReadByteArray", "")]
    public override OperateResult<byte[]> Read(string address, ushort length)
    {
      OperateResult<byte, ushort> operateResult = FanucHelper.AnalysisFanucAddress(address);
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult);
      return operateResult.Content1 == (byte) 8 || operateResult.Content1 == (byte) 10 || operateResult.Content1 == (byte) 12 ? this.Read(operateResult.Content1, operateResult.Content2, length) : new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType);
    }

    /// <summary>
    /// 写入原始的byte数组数据到指定的地址，返回是否写入成功，地址示例：D1，AI1，AQ1，共计3个区的数据，注意起始的起始为1<br />
    /// Write the original byte array data to the specified address, and return whether the write was successful. Example addresses: D1, AI1, AQ1, a total of 3 areas of data. Note that the start of the address is 1.
    /// </summary>
    /// <param name="address">起始地址，地址示例：D1，AI1，AQ1，共计3个区的数据，注意起始的起始为1</param>
    /// <param name="value">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    [EstMqttApi("WriteByteArray", "")]
    public override OperateResult Write(string address, byte[] value)
    {
      OperateResult<byte, ushort> operateResult = FanucHelper.AnalysisFanucAddress(address);
      if (!operateResult.IsSuccess)
        return (OperateResult) OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult);
      return operateResult.Content1 == (byte) 8 || operateResult.Content1 == (byte) 10 || operateResult.Content1 == (byte) 12 ? this.Write(operateResult.Content1, operateResult.Content2, value) : (OperateResult) new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType);
    }

    /// <summary>
    /// 按照位为单位批量读取设备的原始数据，需要指定地址及长度，地址示例：M1，I1，Q1，共计3个区的数据，注意地址的起始为1<br />
    /// Read the raw data of the device in batches in units of boolean. You need to specify the address and length. Example addresses: M1，I1，Q1, a total of 3 areas of data. Note that the start of the address is 1.
    /// </summary>
    /// <param name="address">起始地址，地址示例：M1，I1，Q1，共计3个区的数据，注意地址的起始为1</param>
    /// <param name="length">读取的长度，位为单位</param>
    /// <returns>返回的数据信息结果</returns>
    [EstMqttApi("ReadBoolArray", "")]
    public override OperateResult<bool[]> ReadBool(string address, ushort length)
    {
      OperateResult<byte, ushort> operateResult = FanucHelper.AnalysisFanucAddress(address);
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult);
      return operateResult.Content1 == (byte) 70 || operateResult.Content1 == (byte) 72 || operateResult.Content1 == (byte) 76 ? this.ReadBool(operateResult.Content1, operateResult.Content2, length) : new OperateResult<bool[]>(StringResources.Language.NotSupportedDataType);
    }

    /// <summary>
    /// 批量写入<see cref="T:System.Boolean" />数组数据，返回是否写入成功，需要指定起始地址，地址示例：M1，I1，Q1，共计3个区的数据，注意地址的起始为1<br />
    /// Write <see cref="T:System.Boolean" /> array data in batches. If the write success is returned, you need to specify the starting address. Example address: M1, I1, Q1, a total of 3 areas of data. Note that the starting address is 1.
    /// </summary>
    /// <param name="address">起始地址，地址示例：M1，I1，Q1，共计3个区的数据，注意地址的起始为1</param>
    /// <param name="value">等待写入的数据值</param>
    /// <returns>是否写入成功</returns>
    [EstMqttApi("WriteBoolArray", "")]
    public override OperateResult Write(string address, bool[] value)
    {
      OperateResult<byte, ushort> operateResult = FanucHelper.AnalysisFanucAddress(address);
      if (!operateResult.IsSuccess)
        return (OperateResult) operateResult;
      return operateResult.Content1 == (byte) 70 || operateResult.Content1 == (byte) 72 || operateResult.Content1 == (byte) 76 ? this.WriteBool(operateResult.Content1, operateResult.Content2, value) : new OperateResult(StringResources.Language.NotSupportedDataType);
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.FANUC.FanucInterfaceNet.Read(System.String,System.UInt16)" />
    public override async Task<OperateResult<byte[]>> ReadAsync(
      string address,
      ushort length)
    {
      OperateResult<byte, ushort> analysis = FanucHelper.AnalysisFanucAddress(address);
      if (!analysis.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) analysis);
      if (analysis.Content1 != (byte) 8 && analysis.Content1 != (byte) 10 && analysis.Content1 != (byte) 12)
        return new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType);
      OperateResult<byte[]> operateResult = await this.ReadAsync(analysis.Content1, analysis.Content2, length);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.FANUC.FanucInterfaceNet.Write(System.String,System.Byte[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      byte[] value)
    {
      OperateResult<byte, ushort> analysis = FanucHelper.AnalysisFanucAddress(address);
      if (!analysis.IsSuccess)
        return (OperateResult) OperateResult.CreateFailedResult<byte[]>((OperateResult) analysis);
      if (analysis.Content1 != (byte) 8 && analysis.Content1 != (byte) 10 && analysis.Content1 != (byte) 12)
        return (OperateResult) new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType);
      OperateResult operateResult = await this.WriteAsync(analysis.Content1, analysis.Content2, value);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.FANUC.FanucInterfaceNet.ReadBool(System.String,System.UInt16)" />
    public override async Task<OperateResult<bool[]>> ReadBoolAsync(
      string address,
      ushort length)
    {
      OperateResult<byte, ushort> analysis = FanucHelper.AnalysisFanucAddress(address);
      if (!analysis.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) analysis);
      if (analysis.Content1 != (byte) 70 && analysis.Content1 != (byte) 72 && analysis.Content1 != (byte) 76)
        return new OperateResult<bool[]>(StringResources.Language.NotSupportedDataType);
      OperateResult<bool[]> operateResult = await this.ReadBoolAsync(analysis.Content1, analysis.Content2, length);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.FANUC.FanucInterfaceNet.Write(System.String,System.Boolean[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      bool[] value)
    {
      OperateResult<byte, ushort> analysis = FanucHelper.AnalysisFanucAddress(address);
      if (!analysis.IsSuccess)
        return (OperateResult) analysis;
      if (analysis.Content1 != (byte) 70 && analysis.Content1 != (byte) 72 && analysis.Content1 != (byte) 76)
        return new OperateResult(StringResources.Language.NotSupportedDataType);
      OperateResult operateResult = await this.WriteBoolAsync(analysis.Content1, analysis.Content2, value);
      return operateResult;
    }

    /// <summary>
    /// 按照字为单位批量读取设备的原始数据，需要指定数据块地址，偏移地址及长度，主要针对08, 10, 12的数据块，注意地址的起始为1<br />
    /// Read the raw data of the device in batches in units of words. You need to specify the data block address, offset address, and length. It is mainly for data blocks of 08, 10, and 12. Note that the start of the address is 1.
    /// </summary>
    /// <param name="select">数据块信息</param>
    /// <param name="address">偏移地址</param>
    /// <param name="length">读取的长度，字为单位</param>
    public OperateResult<byte[]> Read(byte select, ushort address, ushort length)
    {
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(FanucHelper.BulidReadData(select, address, length));
      if (!operateResult.IsSuccess)
        return operateResult;
      if (operateResult.Content[31] == (byte) 148)
        return OperateResult.CreateSuccessResult<byte[]>(SoftBasic.ArrayRemoveBegin<byte>(operateResult.Content, 56));
      return operateResult.Content[31] == (byte) 212 ? OperateResult.CreateSuccessResult<byte[]>(SoftBasic.ArraySelectMiddle<byte>(operateResult.Content, 44, (int) length * 2)) : new OperateResult<byte[]>((int) operateResult.Content[31], "Error");
    }

    /// <summary>
    /// 写入原始的byte数组数据到指定的地址，返回是否写入成功，，需要指定数据块地址，偏移地址，主要针对08, 10, 12的数据块，注意起始的起始为1<br />
    /// Write the original byte array data to the specified address, and return whether the writing is successful. You need to specify the data block address and offset address,
    /// which are mainly for the data blocks of 08, 10, and 12. Note that the start of the start is 1.
    /// </summary>
    /// <param name="select">数据块信息</param>
    /// <param name="address">偏移地址</param>
    /// <param name="value">原始数据内容</param>
    public OperateResult Write(byte select, ushort address, byte[] value)
    {
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(FanucHelper.BuildWriteData(select, address, value, value.Length / 2));
      if (!operateResult.IsSuccess)
        return (OperateResult) operateResult;
      return operateResult.Content[31] == (byte) 212 ? OperateResult.CreateSuccessResult() : (OperateResult) new OperateResult<byte[]>((int) operateResult.Content[31], "Error");
    }

    /// <summary>
    /// 按照位为单位批量读取设备的原始数据，需要指定数据块地址，偏移地址及长度，主要针对70, 72, 76的数据块，注意地址的起始为1<br />
    /// </summary>
    /// <param name="select">数据块信息</param>
    /// <param name="address">偏移地址</param>
    /// <param name="length">读取的长度，字为单位</param>
    public OperateResult<bool[]> ReadBool(byte select, ushort address, ushort length)
    {
      int num = (int) address - 1 - ((int) address - 1) % 8 + 1;
      int length1 = ((((int) address + (int) length - 1) % 8 == 0 ? (int) address + (int) length - 1 : ((int) address + (int) length - 1) / 8 * 8 + 8) - num + 1) / 8;
      byte[] numArray = FanucHelper.BulidReadData(select, address, (ushort) (length1 * 8));
      this.LogNet?.WriteDebug(this.ToString(), StringResources.Language.Send + SoftBasic.ByteToHexString(numArray));
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(numArray);
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult);
      this.LogNet?.WriteDebug(this.ToString(), StringResources.Language.Receive + SoftBasic.ByteToHexString(operateResult.Content));
      if (operateResult.Content[31] == (byte) 148)
      {
        bool[] boolArray = SoftBasic.ByteToBoolArray(SoftBasic.ArrayRemoveBegin<byte>(operateResult.Content, 56));
        bool[] flagArray = new bool[(int) length];
        Array.Copy((Array) boolArray, (int) address - num, (Array) flagArray, 0, (int) length);
        return OperateResult.CreateSuccessResult<bool[]>(flagArray);
      }
      if (operateResult.Content[31] != (byte) 212)
        return new OperateResult<bool[]>((int) operateResult.Content[31], "Error");
      bool[] boolArray1 = SoftBasic.ByteToBoolArray(SoftBasic.ArraySelectMiddle<byte>(operateResult.Content, 44, length1));
      bool[] flagArray1 = new bool[(int) length];
      Array.Copy((Array) boolArray1, (int) address - num, (Array) flagArray1, 0, (int) length);
      return OperateResult.CreateSuccessResult<bool[]>(flagArray1);
    }

    /// <summary>
    /// 批量写入<see cref="T:System.Boolean" />数组数据，返回是否写入成功，需要指定数据块地址，偏移地址，主要针对70, 72, 76的数据块，注意起始的起始为1
    /// </summary>
    /// <param name="select">数据块信息</param>
    /// <param name="address">偏移地址</param>
    /// <param name="value">原始的数据内容</param>
    /// <returns>是否写入成功</returns>
    public OperateResult WriteBool(byte select, ushort address, bool[] value)
    {
      int num = (int) address - 1 - ((int) address - 1) % 8 + 1;
      bool[] values = new bool[((((int) address + value.Length - 1) % 8 == 0 ? (int) address + value.Length - 1 : ((int) address + value.Length - 1) / 8 * 8 + 8) - num + 1) / 8 * 8];
      Array.Copy((Array) value, 0, (Array) values, (int) address - num, value.Length);
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(FanucHelper.BuildWriteData(select, address, this.ByteTransform.TransByte(values), value.Length));
      if (!operateResult.IsSuccess)
        return (OperateResult) OperateResult.CreateFailedResult<short[]>((OperateResult) operateResult);
      return operateResult.Content[31] == (byte) 212 ? OperateResult.CreateSuccessResult() : new OperateResult();
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.FANUC.FanucInterfaceNet.Read(System.Byte,System.UInt16,System.UInt16)" />
    public async Task<OperateResult<byte[]>> ReadAsync(
      byte select,
      ushort address,
      ushort length)
    {
      byte[] send = FanucHelper.BulidReadData(select, address, length);
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(send);
      OperateResult<byte[]> operateResult = read.IsSuccess ? (read.Content[31] != (byte) 148 ? (read.Content[31] != (byte) 212 ? new OperateResult<byte[]>((int) read.Content[31], "Error") : OperateResult.CreateSuccessResult<byte[]>(SoftBasic.ArraySelectMiddle<byte>(read.Content, 44, (int) length * 2))) : OperateResult.CreateSuccessResult<byte[]>(SoftBasic.ArrayRemoveBegin<byte>(read.Content, 56))) : read;
      send = (byte[]) null;
      read = (OperateResult<byte[]>) null;
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.FANUC.FanucInterfaceNet.Write(System.Byte,System.UInt16,System.Byte[])" />
    public async Task<OperateResult> WriteAsync(
      byte select,
      ushort address,
      byte[] value)
    {
      byte[] send = FanucHelper.BuildWriteData(select, address, value, value.Length / 2);
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(send);
      OperateResult operateResult = read.IsSuccess ? (read.Content[31] != (byte) 212 ? (OperateResult) new OperateResult<byte[]>((int) read.Content[31], "Error") : OperateResult.CreateSuccessResult()) : (OperateResult) read;
      send = (byte[]) null;
      read = (OperateResult<byte[]>) null;
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.FANUC.FanucInterfaceNet.ReadBool(System.Byte,System.UInt16,System.UInt16)" />
    public async Task<OperateResult<bool[]>> ReadBoolAsync(
      byte select,
      ushort address,
      ushort length)
    {
      int byteStartIndex = (int) address - 1 - ((int) address - 1) % 8 + 1;
      int byteEndIndex = ((int) address + (int) length - 1) % 8 == 0 ? (int) address + (int) length - 1 : ((int) address + (int) length - 1) / 8 * 8 + 8;
      int byteLength = (byteEndIndex - byteStartIndex + 1) / 8;
      byte[] send = FanucHelper.BulidReadData(select, address, (ushort) (byteLength * 8));
      this.LogNet?.WriteDebug(this.ToString(), StringResources.Language.Send + SoftBasic.ByteToHexString(send));
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(send);
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) read);
      this.LogNet?.WriteDebug(this.ToString(), StringResources.Language.Receive + SoftBasic.ByteToHexString(read.Content));
      if (read.Content[31] == (byte) 148)
      {
        bool[] array = SoftBasic.ByteToBoolArray(SoftBasic.ArrayRemoveBegin<byte>(read.Content, 56));
        bool[] buffer = new bool[(int) length];
        Array.Copy((Array) array, (int) address - byteStartIndex, (Array) buffer, 0, (int) length);
        return OperateResult.CreateSuccessResult<bool[]>(buffer);
      }
      if (read.Content[31] != (byte) 212)
        return new OperateResult<bool[]>((int) read.Content[31], "Error");
      bool[] array1 = SoftBasic.ByteToBoolArray(SoftBasic.ArraySelectMiddle<byte>(read.Content, 44, byteLength));
      bool[] buffer1 = new bool[(int) length];
      Array.Copy((Array) array1, (int) address - byteStartIndex, (Array) buffer1, 0, (int) length);
      return OperateResult.CreateSuccessResult<bool[]>(buffer1);
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.FANUC.FanucInterfaceNet.WriteBool(System.Byte,System.UInt16,System.Boolean[])" />
    public async Task<OperateResult> WriteBoolAsync(
      byte select,
      ushort address,
      bool[] value)
    {
      int byteStartIndex = (int) address - 1 - ((int) address - 1) % 8 + 1;
      int byteEndIndex = ((int) address + value.Length - 1) % 8 == 0 ? (int) address + value.Length - 1 : ((int) address + value.Length - 1) / 8 * 8 + 8;
      int byteLength = (byteEndIndex - byteStartIndex + 1) / 8;
      bool[] buffer = new bool[byteLength * 8];
      Array.Copy((Array) value, 0, (Array) buffer, (int) address - byteStartIndex, value.Length);
      byte[] send = FanucHelper.BuildWriteData(select, address, this.ByteTransform.TransByte(buffer), value.Length);
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(send);
      OperateResult operateResult = read.IsSuccess ? (read.Content[31] != (byte) 212 ? new OperateResult() : OperateResult.CreateSuccessResult()) : (OperateResult) OperateResult.CreateFailedResult<short[]>((OperateResult) read);
      buffer = (bool[]) null;
      send = (byte[]) null;
      read = (OperateResult<byte[]>) null;
      return operateResult;
    }

    /// <summary>
    /// 读取机器人的详细信息，返回解析后的数据类型<br />
    /// Read the details of the robot and return the resolved data type
    /// </summary>
    /// <returns>结果数据信息</returns>
    [EstMqttApi(Description = "Read the details of the robot and return the resolved data type")]
    public OperateResult<FanucData> ReadFanucData()
    {
      OperateResult<byte[]> operateResult = this.Read("");
      return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<FanucData>((OperateResult) operateResult) : FanucData.PraseFrom(operateResult.Content);
    }

    /// <summary>
    /// 读取机器人的SDO信息<br />
    /// Read the SDO information of the robot
    /// </summary>
    /// <param name="address">偏移地址</param>
    /// <param name="length">读取的长度</param>
    /// <returns>结果数据</returns>
    [EstMqttApi(Description = "Read the SDO information of the robot")]
    public OperateResult<bool[]> ReadSDO(ushort address, ushort length) => address < (ushort) 11001 ? this.ReadBool((byte) 70, address, length) : this.ReadPMCR2((ushort) ((uint) address - 11000U), length);

    /// <summary>
    /// 写入机器人的SDO信息<br />
    /// Write the SDO information of the robot
    /// </summary>
    /// <param name="address">偏移地址</param>
    /// <param name="value">数据值</param>
    /// <returns>是否写入成功</returns>
    [EstMqttApi(Description = "Write the SDO information of the robot")]
    public OperateResult WriteSDO(ushort address, bool[] value) => address < (ushort) 11001 ? this.WriteBool((byte) 70, address, value) : this.WritePMCR2((ushort) ((uint) address - 11000U), value);

    /// <summary>
    /// 读取机器人的SDI信息<br />
    /// Read the SDI information of the robot
    /// </summary>
    /// <param name="address">偏移地址</param>
    /// <param name="length">读取长度</param>
    /// <returns>结果内容</returns>
    [EstMqttApi(Description = "Read the SDI information of the robot")]
    public OperateResult<bool[]> ReadSDI(ushort address, ushort length) => this.ReadBool((byte) 72, address, length);

    /// <summary>
    /// 写入机器人的SDI信息<br />
    /// Write the SDI information of the robot
    /// </summary>
    /// <param name="address">偏移地址</param>
    /// <param name="value">数据值</param>
    /// <returns>是否写入成功</returns>
    [EstMqttApi(Description = "Write the SDI information of the robot")]
    public OperateResult WriteSDI(ushort address, bool[] value) => this.WriteBool((byte) 72, address, value);

    /// <summary>读取机器人的RDI信息</summary>
    /// <param name="address">偏移地址</param>
    /// <param name="length">读取长度</param>
    /// <returns>结果信息</returns>
    [EstMqttApi]
    public OperateResult<bool[]> ReadRDI(ushort address, ushort length) => this.ReadBool((byte) 72, (ushort) ((uint) address + 5000U), length);

    /// <summary>写入机器人的RDI信息</summary>
    /// <param name="address">偏移地址</param>
    /// <param name="value">数据值</param>
    /// <returns>是否写入成功</returns>
    [EstMqttApi]
    public OperateResult WriteRDI(ushort address, bool[] value) => this.WriteBool((byte) 72, (ushort) ((uint) address + 5000U), value);

    /// <summary>读取机器人的UI信息</summary>
    /// <param name="address">偏移地址</param>
    /// <param name="length">读取长度</param>
    /// <returns>结果信息</returns>
    [EstMqttApi]
    public OperateResult<bool[]> ReadUI(ushort address, ushort length) => this.ReadBool((byte) 72, (ushort) ((uint) address + 6000U), length);

    /// <summary>读取机器人的UO信息</summary>
    /// <param name="address">偏移地址</param>
    /// <param name="length">读取长度</param>
    /// <returns>结果信息</returns>
    [EstMqttApi]
    public OperateResult<bool[]> ReadUO(ushort address, ushort length) => this.ReadBool((byte) 70, (ushort) ((uint) address + 6000U), length);

    /// <summary>写入机器人的UO信息</summary>
    /// <param name="address">偏移地址</param>
    /// <param name="value">数据值</param>
    /// <returns>是否写入成功</returns>
    [EstMqttApi]
    public OperateResult WriteUO(ushort address, bool[] value) => this.WriteBool((byte) 70, (ushort) ((uint) address + 6000U), value);

    /// <summary>读取机器人的SI信息</summary>
    /// <param name="address">偏移地址</param>
    /// <param name="length">读取长度</param>
    /// <returns>结果信息</returns>
    [EstMqttApi]
    public OperateResult<bool[]> ReadSI(ushort address, ushort length) => this.ReadBool((byte) 72, (ushort) ((uint) address + 7000U), length);

    /// <summary>读取机器人的SO信息</summary>
    /// <param name="address">偏移地址</param>
    /// <param name="length">读取长度</param>
    /// <returns>结果信息</returns>
    [EstMqttApi]
    public OperateResult<bool[]> ReadSO(ushort address, ushort length) => this.ReadBool((byte) 70, (ushort) ((uint) address + 7000U), length);

    /// <summary>写入机器人的SO信息</summary>
    /// <param name="address">偏移地址</param>
    /// <param name="value">数据值</param>
    /// <returns>是否写入成功</returns>
    [EstMqttApi]
    public OperateResult WriteSO(ushort address, bool[] value) => this.WriteBool((byte) 70, (ushort) ((uint) address + 7000U), value);

    /// <summary>读取机器人的GI信息</summary>
    /// <param name="address">偏移地址</param>
    /// <param name="length">数据长度</param>
    /// <returns>结果信息</returns>
    [EstMqttApi]
    public OperateResult<ushort[]> ReadGI(ushort address, ushort length) => ByteTransformHelper.GetSuccessResultFromOther<ushort[], byte[]>(this.Read((byte) 12, address, length), (Func<byte[], ushort[]>) (m => this.ByteTransform.TransUInt16(m, 0, (int) length)));

    /// <summary>写入机器人的GI信息</summary>
    /// <param name="address">偏移地址</param>
    /// <param name="value">数据值</param>
    /// <returns>是否写入成功</returns>
    [EstMqttApi]
    public OperateResult WriteGI(ushort address, ushort[] value) => this.Write((byte) 12, address, this.ByteTransform.TransByte(value));

    /// <summary>读取机器人的GO信息</summary>
    /// <param name="address">偏移地址</param>
    /// <param name="length">读取长度</param>
    /// <returns>结果信息</returns>
    [EstMqttApi]
    public OperateResult<ushort[]> ReadGO(ushort address, ushort length)
    {
      if (address >= (ushort) 10001)
        address -= (ushort) 6000;
      return ByteTransformHelper.GetSuccessResultFromOther<ushort[], byte[]>(this.Read((byte) 10, address, length), (Func<byte[], ushort[]>) (m => this.ByteTransform.TransUInt16(m, 0, (int) length)));
    }

    /// <summary>写入机器人的GO信息</summary>
    /// <param name="address">偏移地址</param>
    /// <param name="value">数据值</param>
    /// <returns>写入结果</returns>
    [EstMqttApi]
    public OperateResult WriteGO(ushort address, ushort[] value)
    {
      if (address >= (ushort) 10001)
        address -= (ushort) 6000;
      return this.Write((byte) 10, address, this.ByteTransform.TransByte(value));
    }

    /// <summary>读取机器人的PMCR2信息</summary>
    /// <param name="address">偏移地址</param>
    /// <param name="length">读取长度</param>
    /// <returns>结果信息</returns>
    [EstMqttApi]
    public OperateResult<bool[]> ReadPMCR2(ushort address, ushort length) => this.ReadBool((byte) 76, address, length);

    /// <summary>写入机器人的PMCR2信息</summary>
    /// <param name="address">偏移信息</param>
    /// <param name="value">数据值</param>
    /// <returns>是否写入成功</returns>
    [EstMqttApi]
    public OperateResult WritePMCR2(ushort address, bool[] value) => this.WriteBool((byte) 76, address, value);

    /// <summary>读取机器人的RDO信息</summary>
    /// <param name="address">偏移地址</param>
    /// <param name="length">读取长度</param>
    /// <returns>结果信息</returns>
    [EstMqttApi]
    public OperateResult<bool[]> ReadRDO(ushort address, ushort length) => this.ReadBool((byte) 70, (ushort) ((uint) address + 5000U), length);

    /// <summary>写入机器人的RDO信息</summary>
    /// <param name="address">偏移地址</param>
    /// <param name="value">数据值</param>
    /// <returns>是否写入成功</returns>
    [EstMqttApi]
    public OperateResult WriteRDO(ushort address, bool[] value) => this.WriteBool((byte) 70, (ushort) ((uint) address + 5000U), value);

    /// <summary>写入机器人的Rxyzwpr信息，谨慎调用，</summary>
    /// <param name="Address">偏移地址</param>
    /// <param name="Xyzwpr">姿态信息</param>
    /// <param name="Config">设置信息</param>
    /// <param name="UserFrame">参考系</param>
    /// <param name="UserTool">工具</param>
    /// <returns>是否写入成功</returns>
    [EstMqttApi]
    public OperateResult WriteRXyzwpr(
      ushort Address,
      float[] Xyzwpr,
      short[] Config,
      short UserFrame,
      short UserTool)
    {
      byte[] numArray = new byte[Xyzwpr.Length * 4 + Config.Length * 2 + 2];
      this.ByteTransform.TransByte(Xyzwpr).CopyTo((Array) numArray, 0);
      this.ByteTransform.TransByte(Config).CopyTo((Array) numArray, 36);
      OperateResult operateResult1 = this.Write((byte) 8, Address, numArray);
      if (!operateResult1.IsSuccess)
        return operateResult1;
      if ((short) 0 <= UserFrame && UserFrame <= (short) 15)
      {
        if ((short) 0 <= UserTool && UserTool <= (short) 15)
        {
          OperateResult operateResult2 = this.Write((byte) 8, (ushort) ((uint) Address + 45U), this.ByteTransform.TransByte(new short[2]
          {
            UserFrame,
            UserTool
          }));
          if (!operateResult2.IsSuccess)
            return operateResult2;
        }
        else
        {
          OperateResult operateResult2 = this.Write((byte) 8, (ushort) ((uint) Address + 45U), this.ByteTransform.TransByte(new short[1]
          {
            UserFrame
          }));
          if (!operateResult2.IsSuccess)
            return operateResult2;
        }
      }
      else if ((short) 0 <= UserTool && UserTool <= (short) 15)
      {
        OperateResult operateResult2 = this.Write((byte) 8, (ushort) ((uint) Address + 46U), this.ByteTransform.TransByte(new short[1]
        {
          UserTool
        }));
        if (!operateResult2.IsSuccess)
          return operateResult2;
      }
      return OperateResult.CreateSuccessResult();
    }

    /// <summary>写入机器人的Joint信息</summary>
    /// <param name="address">偏移地址</param>
    /// <param name="joint">关节坐标</param>
    /// <param name="UserFrame">参考系</param>
    /// <param name="UserTool">工具</param>
    /// <returns>是否写入成功</returns>
    [EstMqttApi]
    public OperateResult WriteRJoint(
      ushort address,
      float[] joint,
      short UserFrame,
      short UserTool)
    {
      OperateResult operateResult1 = this.Write((byte) 8, (ushort) ((uint) address + 26U), this.ByteTransform.TransByte(joint));
      if (!operateResult1.IsSuccess)
        return operateResult1;
      if ((short) 0 <= UserFrame && UserFrame <= (short) 15)
      {
        if ((short) 0 <= UserTool && UserTool <= (short) 15)
        {
          OperateResult operateResult2 = this.Write((byte) 8, (ushort) ((uint) address + 44U), this.ByteTransform.TransByte(new short[3]
          {
            (short) 0,
            UserFrame,
            UserTool
          }));
          if (!operateResult2.IsSuccess)
            return operateResult2;
        }
        else
        {
          OperateResult operateResult2 = this.Write((byte) 8, (ushort) ((uint) address + 44U), this.ByteTransform.TransByte(new short[2]
          {
            (short) 0,
            UserFrame
          }));
          if (!operateResult2.IsSuccess)
            return operateResult2;
        }
      }
      else
      {
        OperateResult operateResult2 = this.Write((byte) 8, (ushort) ((uint) address + 44U), this.ByteTransform.TransByte(new short[1]));
        if (!operateResult2.IsSuccess)
          return operateResult2;
        if ((short) 0 <= UserTool && UserTool <= (short) 15)
        {
          OperateResult operateResult3 = this.Write((byte) 8, (ushort) ((uint) address + 44U), this.ByteTransform.TransByte(new short[2]
          {
            (short) 0,
            UserTool
          }));
          if (!operateResult3.IsSuccess)
            return operateResult3;
        }
      }
      return OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.FANUC.FanucInterfaceNet.ReadFanucData" />
    public async Task<OperateResult<FanucData>> ReadFanucDataAsync()
    {
      OperateResult<byte[]> read = await this.ReadAsync("");
      OperateResult<FanucData> operateResult = read.IsSuccess ? FanucData.PraseFrom(read.Content) : OperateResult.CreateFailedResult<FanucData>((OperateResult) read);
      read = (OperateResult<byte[]>) null;
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.FANUC.FanucInterfaceNet.ReadSDO(System.UInt16,System.UInt16)" />
    public async Task<OperateResult<bool[]>> ReadSDOAsync(
      ushort address,
      ushort length)
    {
      if (address < (ushort) 11001)
        return this.ReadBool((byte) 70, address, length);
      OperateResult<bool[]> operateResult = await this.ReadPMCR2Async((ushort) ((uint) address - 11000U), length);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.FANUC.FanucInterfaceNet.WriteSDO(System.UInt16,System.Boolean[])" />
    public async Task<OperateResult> WriteSDOAsync(ushort address, bool[] value)
    {
      if (address < (ushort) 11001)
        return this.WriteBool((byte) 70, address, value);
      OperateResult operateResult = await this.WritePMCR2Async((ushort) ((uint) address - 11000U), value);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.FANUC.FanucInterfaceNet.ReadSDI(System.UInt16,System.UInt16)" />
    public async Task<OperateResult<bool[]>> ReadSDIAsync(
      ushort address,
      ushort length)
    {
      OperateResult<bool[]> operateResult = await this.ReadBoolAsync((byte) 72, address, length);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.FANUC.FanucInterfaceNet.WriteSDI(System.UInt16,System.Boolean[])" />
    public async Task<OperateResult> WriteSDIAsync(ushort address, bool[] value)
    {
      OperateResult operateResult = await this.WriteBoolAsync((byte) 72, address, value);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.FANUC.FanucInterfaceNet.ReadRDI(System.UInt16,System.UInt16)" />
    public async Task<OperateResult<bool[]>> ReadRDIAsync(
      ushort address,
      ushort length)
    {
      OperateResult<bool[]> operateResult = await this.ReadBoolAsync((byte) 72, (ushort) ((uint) address + 5000U), length);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.FANUC.FanucInterfaceNet.WriteRDI(System.UInt16,System.Boolean[])" />
    public async Task<OperateResult> WriteRDIAsync(ushort address, bool[] value)
    {
      OperateResult operateResult = await this.WriteBoolAsync((byte) 72, (ushort) ((uint) address + 5000U), value);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.FANUC.FanucInterfaceNet.ReadUI(System.UInt16,System.UInt16)" />
    public async Task<OperateResult<bool[]>> ReadUIAsync(
      ushort address,
      ushort length)
    {
      OperateResult<bool[]> operateResult = await this.ReadBoolAsync((byte) 72, (ushort) ((uint) address + 6000U), length);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.FANUC.FanucInterfaceNet.ReadUO(System.UInt16,System.UInt16)" />
    public async Task<OperateResult<bool[]>> ReadUOAsync(
      ushort address,
      ushort length)
    {
      OperateResult<bool[]> operateResult = await this.ReadBoolAsync((byte) 70, (ushort) ((uint) address + 6000U), length);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.FANUC.FanucInterfaceNet.WriteUO(System.UInt16,System.Boolean[])" />
    public async Task<OperateResult> WriteUOAsync(ushort address, bool[] value)
    {
      OperateResult operateResult = await this.WriteBoolAsync((byte) 70, (ushort) ((uint) address + 6000U), value);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.FANUC.FanucInterfaceNet.ReadSI(System.UInt16,System.UInt16)" />
    public async Task<OperateResult<bool[]>> ReadSIAsync(
      ushort address,
      ushort length)
    {
      OperateResult<bool[]> operateResult = await this.ReadBoolAsync((byte) 72, (ushort) ((uint) address + 7000U), length);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.FANUC.FanucInterfaceNet.ReadSO(System.UInt16,System.UInt16)" />
    public async Task<OperateResult<bool[]>> ReadSOAsync(
      ushort address,
      ushort length)
    {
      OperateResult<bool[]> operateResult = await this.ReadBoolAsync((byte) 70, (ushort) ((uint) address + 7000U), length);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.FANUC.FanucInterfaceNet.WriteSO(System.UInt16,System.Boolean[])" />
    public async Task<OperateResult> WriteSOAsync(ushort address, bool[] value)
    {
      OperateResult operateResult = await this.WriteBoolAsync((byte) 70, (ushort) ((uint) address + 7000U), value);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.FANUC.FanucInterfaceNet.ReadGI(System.UInt16,System.UInt16)" />
    public async Task<OperateResult<ushort[]>> ReadGIAsync(
      ushort address,
      ushort length)
    {
      OperateResult<byte[]> result = await this.ReadAsync((byte) 12, address, length);
      return ByteTransformHelper.GetSuccessResultFromOther<ushort[], byte[]>(result, (Func<byte[], ushort[]>) (m => this.ByteTransform.TransUInt16(m, 0, (int) length)));
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.FANUC.FanucInterfaceNet.WriteGI(System.UInt16,System.UInt16[])" />
    public async Task<OperateResult> WriteGIAsync(ushort address, ushort[] value)
    {
      OperateResult operateResult = await this.WriteAsync((byte) 12, address, this.ByteTransform.TransByte(value));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.FANUC.FanucInterfaceNet.ReadGO(System.UInt16,System.UInt16)" />
    public async Task<OperateResult<ushort[]>> ReadGOAsync(
      ushort address,
      ushort length)
    {
      if (address >= (ushort) 10001)
        address -= (ushort) 6000;
      OperateResult<byte[]> result = await this.ReadAsync((byte) 10, address, length);
      return ByteTransformHelper.GetSuccessResultFromOther<ushort[], byte[]>(result, (Func<byte[], ushort[]>) (m => this.ByteTransform.TransUInt16(m, 0, (int) length)));
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.FANUC.FanucInterfaceNet.WriteGO(System.UInt16,System.UInt16[])" />
    public async Task<OperateResult> WriteGOAsync(ushort address, ushort[] value)
    {
      if (address >= (ushort) 10001)
        address -= (ushort) 6000;
      OperateResult operateResult = await this.WriteAsync((byte) 10, address, this.ByteTransform.TransByte(value));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.FANUC.FanucInterfaceNet.ReadPMCR2(System.UInt16,System.UInt16)" />
    public async Task<OperateResult<bool[]>> ReadPMCR2Async(
      ushort address,
      ushort length)
    {
      OperateResult<bool[]> operateResult = await this.ReadBoolAsync((byte) 76, address, length);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.FANUC.FanucInterfaceNet.WritePMCR2(System.UInt16,System.Boolean[])" />
    public async Task<OperateResult> WritePMCR2Async(ushort address, bool[] value)
    {
      OperateResult operateResult = await this.WriteBoolAsync((byte) 76, address, value);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.FANUC.FanucInterfaceNet.ReadRDO(System.UInt16,System.UInt16)" />
    public async Task<OperateResult<bool[]>> ReadRDOAsync(
      ushort address,
      ushort length)
    {
      OperateResult<bool[]> operateResult = await this.ReadBoolAsync((byte) 70, (ushort) ((uint) address + 5000U), length);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.FANUC.FanucInterfaceNet.WriteRDO(System.UInt16,System.Boolean[])" />
    public async Task<OperateResult> WriteRDOAsync(ushort address, bool[] value)
    {
      OperateResult operateResult = await this.WriteBoolAsync((byte) 70, (ushort) ((uint) address + 5000U), value);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.FANUC.FanucInterfaceNet.WriteRXyzwpr(System.UInt16,System.Single[],System.Int16[],System.Int16,System.Int16)" />
    public async Task<OperateResult> WriteRXyzwprAsync(
      ushort Address,
      float[] Xyzwpr,
      short[] Config,
      short UserFrame,
      short UserTool)
    {
      int num = Xyzwpr.Length * 4 + Config.Length * 2 + 2;
      byte[] robotBuffer = new byte[num];
      this.ByteTransform.TransByte(Xyzwpr).CopyTo((Array) robotBuffer, 0);
      this.ByteTransform.TransByte(Config).CopyTo((Array) robotBuffer, 36);
      OperateResult write = await this.WriteAsync((byte) 8, Address, robotBuffer);
      if (!write.IsSuccess)
        return write;
      if ((short) 0 <= UserFrame && UserFrame <= (short) 15)
      {
        if ((short) 0 <= UserTool && UserTool <= (short) 15)
        {
          write = await this.WriteAsync((byte) 8, (ushort) ((uint) Address + 45U), this.ByteTransform.TransByte(new short[2]
          {
            UserFrame,
            UserTool
          }));
          if (!write.IsSuccess)
            return write;
        }
        else
        {
          write = await this.WriteAsync((byte) 8, (ushort) ((uint) Address + 45U), this.ByteTransform.TransByte(new short[1]
          {
            UserFrame
          }));
          if (!write.IsSuccess)
            return write;
        }
      }
      else if ((short) 0 <= UserTool && UserTool <= (short) 15)
      {
        write = await this.WriteAsync((byte) 8, (ushort) ((uint) Address + 46U), this.ByteTransform.TransByte(new short[1]
        {
          UserTool
        }));
        if (!write.IsSuccess)
          return write;
      }
      return OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.FANUC.FanucInterfaceNet.WriteRJoint(System.UInt16,System.Single[],System.Int16,System.Int16)" />
    public async Task<OperateResult> WriteRJointAsync(
      ushort address,
      float[] joint,
      short UserFrame,
      short UserTool)
    {
      OperateResult write = await this.WriteAsync((byte) 8, (ushort) ((uint) address + 26U), this.ByteTransform.TransByte(joint));
      if (!write.IsSuccess)
        return write;
      if ((short) 0 <= UserFrame && UserFrame <= (short) 15)
      {
        if ((short) 0 <= UserTool && UserTool <= (short) 15)
        {
          write = await this.WriteAsync((byte) 8, (ushort) ((uint) address + 44U), this.ByteTransform.TransByte(new short[3]
          {
            (short) 0,
            UserFrame,
            UserTool
          }));
          if (!write.IsSuccess)
            return write;
        }
        else
        {
          write = await this.WriteAsync((byte) 8, (ushort) ((uint) address + 44U), this.ByteTransform.TransByte(new short[2]
          {
            (short) 0,
            UserFrame
          }));
          if (!write.IsSuccess)
            return write;
        }
      }
      else
      {
        write = await this.WriteAsync((byte) 8, (ushort) ((uint) address + 44U), this.ByteTransform.TransByte(new short[1]));
        if (!write.IsSuccess)
          return write;
        if ((short) 0 <= UserTool && UserTool <= (short) 15)
        {
          write = await this.WriteAsync((byte) 8, (ushort) ((uint) address + 44U), this.ByteTransform.TransByte(new short[2]
          {
            (short) 0,
            UserTool
          }));
          if (!write.IsSuccess)
            return write;
        }
      }
      return OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("FanucInterfaceNet[{0}:{1}]", (object) this.IpAddress, (object) this.Port);
  }
}
