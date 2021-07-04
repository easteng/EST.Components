// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.Melsec.MelsecFxLinksOverTcp
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using EstCommunication.Core;
using EstCommunication.Core.Net;
using EstCommunication.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstCommunication.Profinet.Melsec
{
  /// <summary>
  /// 三菱计算机链接协议的网口版本，适用FX3U系列，FX3G，FX3S等等系列，通常在PLC侧连接的是485的接线口<br />
  /// Network port version of Mitsubishi Computer Link Protocol, suitable for FX3U series, FX3G, FX3S, etc., usually the 485 connection port is connected on the PLC side
  /// </summary>
  /// <remarks>
  /// 支持的通讯的系列如下参考
  /// <list type="table">
  ///     <listheader>
  ///         <term>系列</term>
  ///         <term>是否支持</term>
  ///         <term>备注</term>
  ///     </listheader>
  ///     <item>
  ///         <description>FX3UC系列</description>
  ///         <description>支持</description>
  ///         <description></description>
  ///     </item>
  ///     <item>
  ///         <description>FX3U系列</description>
  ///         <description>支持</description>
  ///         <description></description>
  ///     </item>
  ///     <item>
  ///         <description>FX3GC系列</description>
  ///         <description>支持</description>
  ///         <description></description>
  ///     </item>
  ///     <item>
  ///         <description>FX3G系列</description>
  ///         <description>支持</description>
  ///         <description></description>
  ///     </item>
  ///     <item>
  ///         <description>FX3S系列</description>
  ///         <description>支持</description>
  ///         <description></description>
  ///     </item>
  ///     <item>
  ///         <description>FX2NC系列</description>
  ///         <description>支持</description>
  ///         <description></description>
  ///     </item>
  ///     <item>
  ///         <description>FX2N系列</description>
  ///         <description>部分支持(v1.06+)</description>
  ///         <description>通过监控D8001来确认版本号</description>
  ///     </item>
  ///     <item>
  ///         <description>FX1NC系列</description>
  ///         <description>支持</description>
  ///         <description></description>
  ///     </item>
  ///     <item>
  ///         <description>FX1N系列</description>
  ///         <description>支持</description>
  ///         <description></description>
  ///     </item>
  ///     <item>
  ///         <description>FX1S系列</description>
  ///         <description>支持</description>
  ///         <description></description>
  ///     </item>
  ///     <item>
  ///         <description>FX0N系列</description>
  ///         <description>部分支持(v1.20+)</description>
  ///         <description></description>
  ///     </item>
  ///     <item>
  ///         <description>FX0S系列</description>
  ///         <description>不支持</description>
  ///         <description></description>
  ///     </item>
  ///     <item>
  ///         <description>FX0系列</description>
  ///         <description>不支持</description>
  ///         <description></description>
  ///     </item>
  ///     <item>
  ///         <description>FX2C系列</description>
  ///         <description>部分支持(v3.30+)</description>
  ///         <description></description>
  ///     </item>
  ///     <item>
  ///         <description>FX2(FX)系列</description>
  ///         <description>部分支持(v3.30+)</description>
  ///         <description></description>
  ///     </item>
  ///     <item>
  ///         <description>FX1系列</description>
  ///         <description>不支持</description>
  ///         <description></description>
  ///     </item>
  /// </list>
  /// 数据地址支持的格式如下：
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
  ///     <term>内部继电器</term>
  ///     <term>M</term>
  ///     <term>M100,M200</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>输入继电器</term>
  ///     <term>X</term>
  ///     <term>X10,X20</term>
  ///     <term>8</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>输出继电器</term>
  ///     <term>Y</term>
  ///     <term>Y10,Y20</term>
  ///     <term>8</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>步进继电器</term>
  ///     <term>S</term>
  ///     <term>S100,S200</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>定时器的触点</term>
  ///     <term>TS</term>
  ///     <term>TS100,TS200</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>定时器的当前值</term>
  ///     <term>TN</term>
  ///     <term>TN100,TN200</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>计数器的触点</term>
  ///     <term>CS</term>
  ///     <term>CS100,CS200</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>计数器的当前</term>
  ///     <term>CN</term>
  ///     <term>CN100,CN200</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>数据寄存器</term>
  ///     <term>D</term>
  ///     <term>D1000,D2000</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>文件寄存器</term>
  ///     <term>R</term>
  ///     <term>R100,R200</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term></term>
  ///   </item>
  /// </list>
  /// </remarks>
  public class MelsecFxLinksOverTcp : NetworkDeviceBase
  {
    private byte station = 0;
    private byte watiingTime = 0;
    private bool sumCheck = true;

    /// <summary>
    /// 实例化默认的对象<br />
    /// Instantiate the default object
    /// </summary>
    public MelsecFxLinksOverTcp()
    {
      this.WordLength = (ushort) 1;
      this.ByteTransform = (IByteTransform) new RegularByteTransform();
      this.SleepTime = 20;
    }

    /// <summary>
    /// 指定ip地址和端口号来实例化默认的对象<br />
    /// Specify the IP address and port number to instantiate the default object
    /// </summary>
    /// <param name="ipAddress">Ip地址信息</param>
    /// <param name="port">端口号</param>
    public MelsecFxLinksOverTcp(string ipAddress, int port)
      : this()
    {
      this.IpAddress = ipAddress;
      this.Port = port;
    }

    /// <summary>
    /// PLC的当前的站号，需要根据实际的值来设定，默认是0<br />
    /// The current station number of the PLC needs to be set according to the actual value. The default is 0.
    /// </summary>
    public byte Station
    {
      get => this.station;
      set => this.station = value;
    }

    /// <summary>
    /// 报文等待时间，单位10ms，设置范围为0-15<br />
    /// Message waiting time, unit is 10ms, setting range is 0-15
    /// </summary>
    public byte WaittingTime
    {
      get => this.watiingTime;
      set
      {
        if (this.watiingTime > (byte) 15)
          this.watiingTime = (byte) 15;
        else
          this.watiingTime = value;
      }
    }

    /// <summary>
    /// 是否启动和校验<br />
    /// Whether to start and sum verify
    /// </summary>
    public bool SumCheck
    {
      get => this.sumCheck;
      set => this.sumCheck = value;
    }

    /// <summary>
    /// 批量读取PLC的数据，以字为单位，支持读取X,Y,M,S,D,T,C，具体的地址范围需要根据PLC型号来确认，地址支持动态指定站号，例如：s=2;D100<br />
    /// Read PLC data in batches, in units of words, supports reading X, Y, M, S, D, T, C.
    /// The specific address range needs to be confirmed according to the PLC model,
    /// The address supports dynamically specifying the station number, for example: s=2;D100
    /// </summary>
    /// <param name="address">地址信息</param>
    /// <param name="length">数据长度</param>
    /// <returns>读取结果信息</returns>
    [EstMqttApi("ReadByteArray", "Read PLC data in batches, in units of words, supports reading X, Y, M, S, D, T, C.")]
    public override OperateResult<byte[]> Read(string address, ushort length)
    {
      OperateResult<byte[]> operateResult1 = MelsecFxLinksOverTcp.BuildReadCommand((byte) EstHelper.ExtractParameter(ref address, "s", (int) this.station), address, length, false, this.sumCheck, this.watiingTime);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult1);
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult2);
      if (operateResult2.Content[0] != (byte) 2)
        return new OperateResult<byte[]>((int) operateResult2.Content[0], "Read Faild:" + SoftBasic.ByteToHexString(operateResult2.Content, ' '));
      byte[] numArray = new byte[(int) length * 2];
      for (int index = 0; index < numArray.Length / 2; ++index)
        BitConverter.GetBytes(Convert.ToUInt16(Encoding.ASCII.GetString(operateResult2.Content, index * 4 + 5, 4), 16)).CopyTo((Array) numArray, index * 2);
      return OperateResult.CreateSuccessResult<byte[]>(numArray);
    }

    /// <summary>
    /// 批量写入PLC的数据，以字为单位，也就是说最少2个字节信息，支持X,Y,M,S,D,T,C，具体的地址范围需要根据PLC型号来确认，地址支持动态指定站号，例如：s=2;D100<br />
    /// The data written to the PLC in batches is in units of words, that is, at least 2 bytes of information.
    /// It supports X, Y, M, S, D, T, and C. The specific address range needs to be confirmed according to the PLC model,
    /// The address supports dynamically specifying the station number, for example: s=2;D100
    /// </summary>
    /// <param name="address">地址信息</param>
    /// <param name="value">数据值</param>
    /// <returns>是否写入成功</returns>
    [EstMqttApi("WriteByteArray", "The data written to the PLC in batches is in units of words, that is, at least 2 bytes of information. It supports X, Y, M, S, D, T, and C. ")]
    public override OperateResult Write(string address, byte[] value)
    {
      OperateResult<byte[]> operateResult1 = MelsecFxLinksOverTcp.BuildWriteByteCommand((byte) EstHelper.ExtractParameter(ref address, "s", (int) this.station), address, value, this.sumCheck, this.watiingTime);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return (OperateResult) operateResult2;
      return operateResult2.Content[0] != (byte) 6 ? new OperateResult((int) operateResult2.Content[0], "Write Faild:" + SoftBasic.ByteToHexString(operateResult2.Content, ' ')) : OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecFxLinksOverTcp.Read(System.String,System.UInt16)" />
    public override async Task<OperateResult<byte[]>> ReadAsync(
      string address,
      ushort length)
    {
      byte stat = (byte) EstHelper.ExtractParameter(ref address, "s", (int) this.station);
      OperateResult<byte[]> command = MelsecFxLinksOverTcp.BuildReadCommand(stat, address, length, false, this.sumCheck, this.watiingTime);
      if (!command.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) command);
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) read);
      if (read.Content[0] != (byte) 2)
        return new OperateResult<byte[]>((int) read.Content[0], "Read Faild:" + SoftBasic.ByteToHexString(read.Content, ' '));
      byte[] Content = new byte[(int) length * 2];
      for (int i = 0; i < Content.Length / 2; ++i)
      {
        ushort tmp = Convert.ToUInt16(Encoding.ASCII.GetString(read.Content, i * 4 + 5, 4), 16);
        BitConverter.GetBytes(tmp).CopyTo((Array) Content, i * 2);
      }
      return OperateResult.CreateSuccessResult<byte[]>(Content);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecFxLinksOverTcp.Write(System.String,System.Byte[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      byte[] value)
    {
      byte stat = (byte) EstHelper.ExtractParameter(ref address, "s", (int) this.station);
      OperateResult<byte[]> command = MelsecFxLinksOverTcp.BuildWriteByteCommand(stat, address, value, this.sumCheck, this.watiingTime);
      if (!command.IsSuccess)
        return (OperateResult) command;
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      return read.IsSuccess ? (read.Content[0] == (byte) 6 ? OperateResult.CreateSuccessResult() : new OperateResult((int) read.Content[0], "Write Faild:" + SoftBasic.ByteToHexString(read.Content, ' '))) : (OperateResult) read;
    }

    /// <summary>
    /// 批量读取bool类型数据，支持的类型为X,Y,S,T,C，具体的地址范围取决于PLC的类型，地址支持动态指定站号，例如：s=2;D100<br />
    /// Read bool data in batches. The supported types are X, Y, S, T, C. The specific address range depends on the type of PLC,
    /// The address supports dynamically specifying the station number, for example: s=2;D100
    /// </summary>
    /// <param name="address">地址信息，比如X10,Y17，注意X，Y的地址是8进制的</param>
    /// <param name="length">读取的长度</param>
    /// <returns>读取结果信息</returns>
    [EstMqttApi("ReadBoolArray", "Read bool data in batches. The supported types are X, Y, S, T, C.")]
    public override OperateResult<bool[]> ReadBool(string address, ushort length)
    {
      OperateResult<byte[]> operateResult1 = MelsecFxLinksOverTcp.BuildReadCommand((byte) EstHelper.ExtractParameter(ref address, "s", (int) this.station), address, length, true, this.sumCheck, this.watiingTime);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult1);
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult2);
      if (operateResult2.Content[0] != (byte) 2)
        return new OperateResult<bool[]>((int) operateResult2.Content[0], "Read Faild:" + SoftBasic.ByteToHexString(operateResult2.Content, ' '));
      byte[] numArray = new byte[(int) length];
      Array.Copy((Array) operateResult2.Content, 5, (Array) numArray, 0, (int) length);
      return OperateResult.CreateSuccessResult<bool[]>(((IEnumerable<byte>) numArray).Select<byte, bool>((Func<byte, bool>) (m => m == (byte) 49)).ToArray<bool>());
    }

    /// <summary>
    /// 批量写入bool类型的数组，支持的类型为X,Y,S,T,C，具体的地址范围取决于PLC的类型，地址支持动态指定站号，例如：s=2;D100<br />
    /// Write arrays of type bool in batches. The supported types are X, Y, S, T, C. The specific address range depends on the type of PLC,
    /// The address supports dynamically specifying the station number, for example: s=2;D100
    /// </summary>
    /// <param name="address">PLC的地址信息</param>
    /// <param name="value">数据信息</param>
    /// <returns>是否写入成功</returns>
    [EstMqttApi("WriteBoolArray", "Write arrays of type bool in batches. The supported types are X, Y, S, T, C.")]
    public override OperateResult Write(string address, bool[] value)
    {
      OperateResult<byte[]> operateResult1 = MelsecFxLinksOverTcp.BuildWriteBoolCommand((byte) EstHelper.ExtractParameter(ref address, "s", (int) this.station), address, value, this.sumCheck, this.watiingTime);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return (OperateResult) operateResult2;
      return operateResult2.Content[0] != (byte) 6 ? new OperateResult((int) operateResult2.Content[0], "Write Faild:" + SoftBasic.ByteToHexString(operateResult2.Content, ' ')) : OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecFxLinksOverTcp.ReadBool(System.String,System.UInt16)" />
    public override async Task<OperateResult<bool[]>> ReadBoolAsync(
      string address,
      ushort length)
    {
      byte stat = (byte) EstHelper.ExtractParameter(ref address, "s", (int) this.station);
      OperateResult<byte[]> command = MelsecFxLinksOverTcp.BuildReadCommand(stat, address, length, true, this.sumCheck, this.watiingTime);
      if (!command.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) command);
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) read);
      if (read.Content[0] != (byte) 2)
        return new OperateResult<bool[]>((int) read.Content[0], "Read Faild:" + SoftBasic.ByteToHexString(read.Content, ' '));
      byte[] buffer = new byte[(int) length];
      Array.Copy((Array) read.Content, 5, (Array) buffer, 0, (int) length);
      return OperateResult.CreateSuccessResult<bool[]>(((IEnumerable<byte>) buffer).Select<byte, bool>((Func<byte, bool>) (m => m == (byte) 49)).ToArray<bool>());
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecFxLinksOverTcp.Write(System.String,System.Boolean[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      bool[] value)
    {
      byte stat = (byte) EstHelper.ExtractParameter(ref address, "s", (int) this.station);
      OperateResult<byte[]> command = MelsecFxLinksOverTcp.BuildWriteBoolCommand(stat, address, value, this.sumCheck, this.watiingTime);
      if (!command.IsSuccess)
        return (OperateResult) command;
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      return read.IsSuccess ? (read.Content[0] == (byte) 6 ? OperateResult.CreateSuccessResult() : new OperateResult((int) read.Content[0], "Write Faild:" + SoftBasic.ByteToHexString(read.Content, ' '))) : (OperateResult) read;
    }

    /// <summary>
    /// <b>[商业授权]</b> 启动PLC的操作，可以携带额外的参数信息，指定站号。举例：s=2; 注意：分号是必须的。<br />
    /// <b>[Authorization]</b> Start the PLC operation, you can carry additional parameter information and specify the station number. Example: s=2; Note: The semicolon is required.
    /// </summary>
    /// <param name="parameter">允许携带的参数信息，例如s=2; 也可以为空</param>
    /// <returns>是否启动成功</returns>
    [EstMqttApi(Description = "Start the PLC operation, you can carry additional parameter information and specify the station number. Example: s=2; Note: The semicolon is required.")]
    public OperateResult StartPLC(string parameter = "")
    {
      OperateResult<byte[]> operateResult1 = MelsecFxLinksOverTcp.BuildStart((byte) EstHelper.ExtractParameter(ref parameter, "s", (int) this.station), this.sumCheck, this.watiingTime);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return (OperateResult) operateResult2;
      return operateResult2.Content[0] != (byte) 6 ? new OperateResult((int) operateResult2.Content[0], "Start Faild:" + SoftBasic.ByteToHexString(operateResult2.Content, ' ')) : OperateResult.CreateSuccessResult();
    }

    /// <summary>
    /// <b>[商业授权]</b> 停止PLC的操作，可以携带额外的参数信息，指定站号。举例：s=2; 注意：分号是必须的。<br />
    /// <b>[Authorization]</b> Stop PLC operation, you can carry additional parameter information and specify the station number. Example: s=2; Note: The semicolon is required.
    /// </summary>
    /// <param name="parameter">允许携带的参数信息，例如s=2; 也可以为空</param>
    /// <returns>是否停止成功</returns>
    [EstMqttApi(Description = "Stop PLC operation, you can carry additional parameter information and specify the station number. Example: s=2; Note: The semicolon is required.")]
    public OperateResult StopPLC(string parameter = "")
    {
      OperateResult<byte[]> operateResult1 = MelsecFxLinksOverTcp.BuildStop((byte) EstHelper.ExtractParameter(ref parameter, "s", (int) this.station), this.sumCheck, this.watiingTime);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return (OperateResult) operateResult2;
      return operateResult2.Content[0] != (byte) 6 ? new OperateResult((int) operateResult2.Content[0], "Stop Faild:" + SoftBasic.ByteToHexString(operateResult2.Content, ' ')) : OperateResult.CreateSuccessResult();
    }

    /// <summary>
    /// <b>[商业授权]</b> 读取PLC的型号信息，可以携带额外的参数信息，指定站号。举例：s=2; 注意：分号是必须的。<br />
    /// <b>[Authorization]</b> Read the PLC model information, you can carry additional parameter information, and specify the station number. Example: s=2; Note: The semicolon is required.
    /// </summary>
    /// <param name="parameter">允许携带的参数信息，例如s=2; 也可以为空</param>
    /// <returns>带PLC型号的结果信息</returns>
    [EstMqttApi(Description = "Read the PLC model information, you can carry additional parameter information, and specify the station number. Example: s=2; Note: The semicolon is required.")]
    public OperateResult<string> ReadPlcType(string parameter = "")
    {
      OperateResult<byte[]> operateResult1 = MelsecFxLinksOverTcp.BuildReadPlcType((byte) EstHelper.ExtractParameter(ref parameter, "s", (int) this.station), this.sumCheck, this.watiingTime);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<string>((OperateResult) operateResult1);
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return OperateResult.CreateFailedResult<string>((OperateResult) operateResult2);
      return operateResult2.Content[0] != (byte) 6 ? new OperateResult<string>((int) operateResult2.Content[0], "ReadPlcType Faild:" + SoftBasic.ByteToHexString(operateResult2.Content, ' ')) : MelsecFxLinksOverTcp.GetPlcTypeFromCode(Encoding.ASCII.GetString(operateResult2.Content, 5, 2));
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecFxLinksOverTcp.StartPLC(System.String)" />
    public async Task<OperateResult> StartPLCAsync(string parameter = "")
    {
      byte stat = (byte) EstHelper.ExtractParameter(ref parameter, "s", (int) this.station);
      OperateResult<byte[]> command = MelsecFxLinksOverTcp.BuildStart(stat, this.sumCheck, this.watiingTime);
      if (!command.IsSuccess)
        return (OperateResult) command;
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      return read.IsSuccess ? (read.Content[0] == (byte) 6 ? OperateResult.CreateSuccessResult() : new OperateResult((int) read.Content[0], "Start Faild:" + SoftBasic.ByteToHexString(read.Content, ' '))) : (OperateResult) read;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecFxLinksOverTcp.StopPLC(System.String)" />
    public async Task<OperateResult> StopPLCAsync(string parameter = "")
    {
      byte stat = (byte) EstHelper.ExtractParameter(ref parameter, "s", (int) this.station);
      OperateResult<byte[]> command = MelsecFxLinksOverTcp.BuildStop(stat, this.sumCheck, this.watiingTime);
      if (!command.IsSuccess)
        return (OperateResult) command;
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      return read.IsSuccess ? (read.Content[0] == (byte) 6 ? OperateResult.CreateSuccessResult() : new OperateResult((int) read.Content[0], "Stop Faild:" + SoftBasic.ByteToHexString(read.Content, ' '))) : (OperateResult) read;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecFxLinksOverTcp.ReadPlcType(System.String)" />
    public async Task<OperateResult<string>> ReadPlcTypeAsync(string parameter = "")
    {
      byte stat = (byte) EstHelper.ExtractParameter(ref parameter, "s", (int) this.station);
      OperateResult<byte[]> command = MelsecFxLinksOverTcp.BuildReadPlcType(stat, this.sumCheck, this.watiingTime);
      if (!command.IsSuccess)
        return OperateResult.CreateFailedResult<string>((OperateResult) command);
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      return read.IsSuccess ? (read.Content[0] == (byte) 6 ? MelsecFxLinksOverTcp.GetPlcTypeFromCode(Encoding.ASCII.GetString(read.Content, 5, 2)) : new OperateResult<string>((int) read.Content[0], "ReadPlcType Faild:" + SoftBasic.ByteToHexString(read.Content, ' '))) : OperateResult.CreateFailedResult<string>((OperateResult) read);
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("MelsecFxLinksOverTcp[{0}:{1}]", (object) this.IpAddress, (object) this.Port);

    /// <summary>解析数据地址成不同的三菱地址类型</summary>
    /// <param name="address">数据地址</param>
    /// <returns>地址结果对象</returns>
    private static OperateResult<string> FxAnalysisAddress(string address)
    {
      OperateResult<string> operateResult = new OperateResult<string>();
      try
      {
        switch (address[0])
        {
          case 'C':
          case 'c':
            if (address[1] == 'S' || address[1] == 's')
            {
              operateResult.Content = "CS" + Convert.ToUInt16(address.Substring(1), 10).ToString("D3");
              break;
            }
            if (address[1] != 'N' && address[1] != 'n')
              throw new Exception(StringResources.Language.NotSupportedDataType);
            operateResult.Content = "CN" + Convert.ToUInt16(address.Substring(1), 10).ToString("D3");
            break;
          case 'D':
          case 'd':
            operateResult.Content = "D" + Convert.ToUInt16(address.Substring(1), 10).ToString("D4");
            break;
          case 'M':
          case 'm':
            operateResult.Content = "M" + Convert.ToUInt16(address.Substring(1), 10).ToString("D4");
            break;
          case 'R':
          case 'r':
            operateResult.Content = "R" + Convert.ToUInt16(address.Substring(1), 10).ToString("D4");
            break;
          case 'S':
          case 's':
            operateResult.Content = "S" + Convert.ToUInt16(address.Substring(1), 10).ToString("D4");
            break;
          case 'T':
          case 't':
            if (address[1] == 'S' || address[1] == 's')
            {
              operateResult.Content = "TS" + Convert.ToUInt16(address.Substring(1), 10).ToString("D3");
              break;
            }
            if (address[1] != 'N' && address[1] != 'n')
              throw new Exception(StringResources.Language.NotSupportedDataType);
            operateResult.Content = "TN" + Convert.ToUInt16(address.Substring(1), 10).ToString("D3");
            break;
          case 'X':
          case 'x':
            Convert.ToUInt16(address.Substring(1), 8);
            operateResult.Content = "X" + Convert.ToUInt16(address.Substring(1), 10).ToString("D4");
            break;
          case 'Y':
          case 'y':
            Convert.ToUInt16(address.Substring(1), 8);
            operateResult.Content = "Y" + Convert.ToUInt16(address.Substring(1), 10).ToString("D4");
            break;
          default:
            throw new Exception(StringResources.Language.NotSupportedDataType);
        }
      }
      catch (Exception ex)
      {
        operateResult.Message = ex.Message;
        return operateResult;
      }
      operateResult.IsSuccess = true;
      return operateResult;
    }

    /// <summary>计算指令的和校验码</summary>
    /// <param name="data">指令</param>
    /// <returns>校验之后的信息</returns>
    public static string CalculateAcc(string data)
    {
      byte[] bytes = Encoding.ASCII.GetBytes(data);
      int num = 0;
      for (int index = 0; index < bytes.Length; ++index)
        num += (int) bytes[index];
      return data + num.ToString("X4").Substring(2);
    }

    /// <summary>创建一条读取的指令信息，需要指定一些参数</summary>
    /// <param name="station">PLCd的站号</param>
    /// <param name="address">地址信息</param>
    /// <param name="length">数据长度</param>
    /// <param name="isBool">是否位读取</param>
    /// <param name="sumCheck">是否和校验</param>
    /// <param name="waitTime">等待时间</param>
    /// <returns>是否成功的结果对象</returns>
    public static OperateResult<byte[]> BuildReadCommand(
      byte station,
      string address,
      ushort length,
      bool isBool,
      bool sumCheck = true,
      byte waitTime = 0)
    {
      OperateResult<string> operateResult = MelsecFxLinksOverTcp.FxAnalysisAddress(address);
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult);
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(station.ToString("D2"));
      stringBuilder.Append("FF");
      if (isBool)
        stringBuilder.Append("BR");
      else
        stringBuilder.Append("WR");
      stringBuilder.Append(waitTime.ToString("X"));
      stringBuilder.Append(operateResult.Content);
      stringBuilder.Append(length.ToString("D2"));
      return OperateResult.CreateSuccessResult<byte[]>(SoftBasic.SpliceArray<byte>(new byte[1]
      {
        (byte) 5
      }, !sumCheck ? Encoding.ASCII.GetBytes(stringBuilder.ToString()) : Encoding.ASCII.GetBytes(MelsecFxLinksOverTcp.CalculateAcc(stringBuilder.ToString()))));
    }

    /// <summary>创建一条别入bool数据的指令信息，需要指定一些参数</summary>
    /// <param name="station">站号</param>
    /// <param name="address">地址</param>
    /// <param name="value">数组值</param>
    /// <param name="sumCheck">是否和校验</param>
    /// <param name="waitTime">等待时间</param>
    /// <returns>是否创建成功</returns>
    public static OperateResult<byte[]> BuildWriteBoolCommand(
      byte station,
      string address,
      bool[] value,
      bool sumCheck = true,
      byte waitTime = 0)
    {
      OperateResult<string> operateResult = MelsecFxLinksOverTcp.FxAnalysisAddress(address);
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult);
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(station.ToString("D2"));
      stringBuilder.Append("FF");
      stringBuilder.Append("BW");
      stringBuilder.Append(waitTime.ToString("X"));
      stringBuilder.Append(operateResult.Content);
      stringBuilder.Append(value.Length.ToString("D2"));
      for (int index = 0; index < value.Length; ++index)
        stringBuilder.Append(value[index] ? "1" : "0");
      return OperateResult.CreateSuccessResult<byte[]>(SoftBasic.SpliceArray<byte>(new byte[1]
      {
        (byte) 5
      }, !sumCheck ? Encoding.ASCII.GetBytes(stringBuilder.ToString()) : Encoding.ASCII.GetBytes(MelsecFxLinksOverTcp.CalculateAcc(stringBuilder.ToString()))));
    }

    /// <summary>创建一条别入byte数据的指令信息，需要指定一些参数，按照字单位</summary>
    /// <param name="station">站号</param>
    /// <param name="address">地址</param>
    /// <param name="value">数组值</param>
    /// <param name="sumCheck">是否和校验</param>
    /// <param name="waitTime">等待时间</param>
    /// <returns>命令报文的结果内容对象</returns>
    public static OperateResult<byte[]> BuildWriteByteCommand(
      byte station,
      string address,
      byte[] value,
      bool sumCheck = true,
      byte waitTime = 0)
    {
      OperateResult<string> operateResult = MelsecFxLinksOverTcp.FxAnalysisAddress(address);
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult);
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(station.ToString("D2"));
      stringBuilder.Append("FF");
      stringBuilder.Append("WW");
      stringBuilder.Append(waitTime.ToString("X"));
      stringBuilder.Append(operateResult.Content);
      stringBuilder.Append((value.Length / 2).ToString("D2"));
      byte[] bytes = new byte[value.Length * 2];
      for (int index = 0; index < value.Length / 2; ++index)
        SoftBasic.BuildAsciiBytesFrom(BitConverter.ToUInt16(value, index * 2)).CopyTo((Array) bytes, 4 * index);
      stringBuilder.Append(Encoding.ASCII.GetString(bytes));
      return OperateResult.CreateSuccessResult<byte[]>(SoftBasic.SpliceArray<byte>(new byte[1]
      {
        (byte) 5
      }, !sumCheck ? Encoding.ASCII.GetBytes(stringBuilder.ToString()) : Encoding.ASCII.GetBytes(MelsecFxLinksOverTcp.CalculateAcc(stringBuilder.ToString()))));
    }

    /// <summary>创建启动PLC的报文信息</summary>
    /// <param name="station">站号信息</param>
    /// <param name="sumCheck">是否和校验</param>
    /// <param name="waitTime">等待时间</param>
    /// <returns>命令报文的结果内容对象</returns>
    public static OperateResult<byte[]> BuildStart(
      byte station,
      bool sumCheck = true,
      byte waitTime = 0)
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(station.ToString("D2"));
      stringBuilder.Append("FF");
      stringBuilder.Append("RR");
      stringBuilder.Append(waitTime.ToString("X"));
      return OperateResult.CreateSuccessResult<byte[]>(SoftBasic.SpliceArray<byte>(new byte[1]
      {
        (byte) 5
      }, !sumCheck ? Encoding.ASCII.GetBytes(stringBuilder.ToString()) : Encoding.ASCII.GetBytes(MelsecFxLinksOverTcp.CalculateAcc(stringBuilder.ToString()))));
    }

    /// <summary>创建启动PLC的报文信息</summary>
    /// <param name="station">站号信息</param>
    /// <param name="sumCheck">是否和校验</param>
    /// <param name="waitTime">等待时间</param>
    /// <returns>命令报文的结果内容对象</returns>
    public static OperateResult<byte[]> BuildStop(
      byte station,
      bool sumCheck = true,
      byte waitTime = 0)
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(station.ToString("D2"));
      stringBuilder.Append("FF");
      stringBuilder.Append("RS");
      stringBuilder.Append(waitTime.ToString("X"));
      return OperateResult.CreateSuccessResult<byte[]>(SoftBasic.SpliceArray<byte>(new byte[1]
      {
        (byte) 5
      }, !sumCheck ? Encoding.ASCII.GetBytes(stringBuilder.ToString()) : Encoding.ASCII.GetBytes(MelsecFxLinksOverTcp.CalculateAcc(stringBuilder.ToString()))));
    }

    /// <summary>创建读取PLC类型的命令报文</summary>
    /// <param name="station">站号信息</param>
    /// <param name="sumCheck">是否进行和校验</param>
    /// <param name="waitTime">等待实际</param>
    /// <returns>命令报文的结果内容对象</returns>
    public static OperateResult<byte[]> BuildReadPlcType(
      byte station,
      bool sumCheck = true,
      byte waitTime = 0)
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(station.ToString("D2"));
      stringBuilder.Append("FF");
      stringBuilder.Append("PC");
      stringBuilder.Append(waitTime.ToString("X"));
      return OperateResult.CreateSuccessResult<byte[]>(SoftBasic.SpliceArray<byte>(new byte[1]
      {
        (byte) 5
      }, !sumCheck ? Encoding.ASCII.GetBytes(stringBuilder.ToString()) : Encoding.ASCII.GetBytes(MelsecFxLinksOverTcp.CalculateAcc(stringBuilder.ToString()))));
    }

    /// <summary>从编码中提取PLC的型号信息</summary>
    /// <param name="code">编码</param>
    /// <returns>PLC的型号信息</returns>
    public static OperateResult<string> GetPlcTypeFromCode(string code)
    {
      switch (code)
      {
        case "82":
          return OperateResult.CreateSuccessResult<string>("A2USCPU");
        case "83":
          return OperateResult.CreateSuccessResult<string>("A2CPU-S1/A2USCPU-S1");
        case "84":
          return OperateResult.CreateSuccessResult<string>("A3UCPU");
        case "85":
          return OperateResult.CreateSuccessResult<string>("A4UCPU");
        case "8B":
          return OperateResult.CreateSuccessResult<string>("AJ72LP25/BR15");
        case "8D":
          return OperateResult.CreateSuccessResult<string>("FX2/FX2C");
        case "8E":
          return OperateResult.CreateSuccessResult<string>("FX0N");
        case "92":
          return OperateResult.CreateSuccessResult<string>("A2ACPU");
        case "93":
          return OperateResult.CreateSuccessResult<string>("A2ACPU-S1");
        case "94":
          return OperateResult.CreateSuccessResult<string>("A3ACPU");
        case "98":
          return OperateResult.CreateSuccessResult<string>("A0J2HCPU");
        case "9A":
          return OperateResult.CreateSuccessResult<string>("A2CCPU");
        case "9D":
          return OperateResult.CreateSuccessResult<string>("FX2N/FX2NC");
        case "9E":
          return OperateResult.CreateSuccessResult<string>("FX1N/FX1NC");
        case "A1":
          return OperateResult.CreateSuccessResult<string>("A1CPU /A1NCPU");
        case "A2":
          return OperateResult.CreateSuccessResult<string>("A2CPU/A2NCPU/A2SCPU");
        case "A3":
          return OperateResult.CreateSuccessResult<string>("A3CPU/A3NCPU");
        case "A4":
          return OperateResult.CreateSuccessResult<string>("A3HCPU/A3MCPU");
        case "AB":
          return OperateResult.CreateSuccessResult<string>("AJ72P25/R25");
        case "F2":
          return OperateResult.CreateSuccessResult<string>("FX1S");
        case "F3":
          return OperateResult.CreateSuccessResult<string>("FX3U/FX3UC");
        case "F4":
          return OperateResult.CreateSuccessResult<string>("FX3G");
        default:
          return new OperateResult<string>(StringResources.Language.NotSupportedDataType + " Code:" + code);
      }
    }
  }
}
