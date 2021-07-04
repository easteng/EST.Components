// Decompiled with JetBrains decompiler
// Type: EstCommunication.Instrument.DLT.DLT645OverTcp
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using EstCommunication.Core;
using EstCommunication.Core.IMessage;
using EstCommunication.Core.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstCommunication.Instrument.DLT
{
  /// <summary>
  /// 基于多功能电能表通信协议实现的通讯类，参考的文档是DLT645-2007，主要实现了对电表数据的读取和一些功能方法，
  /// 在点对点模式下，需要在连接后调用 <see cref="M:EstCommunication.Instrument.DLT.DLT645OverTcp.ReadAddress" /> 方法，数据标识格式为 00-00-00-00，具体参照文档手册。<br />
  /// The communication type based on the communication protocol of the multifunctional electric energy meter.
  /// The reference document is DLT645-2007, which mainly realizes the reading of the electric meter data and some functional methods.
  /// In the point-to-point mode, you need to call <see cref="M:EstCommunication.Instrument.DLT.DLT645OverTcp.ReadAddress" /> method after connect the device.
  /// the data identification format is 00-00-00-00, refer to the documentation manual for details.
  /// </summary>
  /// <remarks>
  /// 如果一对多的模式，地址可以携带地址域访问，例如 "s=2;00-00-00-00"，主要使用 <see cref="M:EstCommunication.Instrument.DLT.DLT645OverTcp.ReadDouble(System.String,System.UInt16)" /> 方法来读取浮点数，
  /// <see cref="M:EstCommunication.Core.Net.NetworkDeviceBase.ReadString(System.String,System.UInt16)" /> 方法来读取字符串
  /// </remarks>
  public class DLT645OverTcp : NetworkDeviceBase
  {
    private string station = "1";
    private string password = "00000000";
    private string opCode = "00000000";

    /// <summary>
    /// 指定IP地址，端口，地址域，密码，操作者代码来实例化一个对象<br />
    /// Specify the IP address, port, address field, password, and operator code to instantiate an object
    /// </summary>
    /// <param name="ipAddress">TcpServer的IP地址</param>
    /// <param name="port">TcpServer的端口</param>
    /// <param name="station">设备的站号信息</param>
    /// <param name="password">密码，写入的时候进行验证的信息</param>
    /// <param name="opCode">操作者代码</param>
    public DLT645OverTcp(
      string ipAddress,
      int port = 502,
      string station = "1",
      string password = "",
      string opCode = "")
    {
      this.IpAddress = ipAddress;
      this.Port = port;
      this.WordLength = (ushort) 1;
      this.ByteTransform = (IByteTransform) new ReverseWordTransform();
      this.station = station;
      this.password = string.IsNullOrEmpty(password) ? "00000000" : password;
      this.opCode = string.IsNullOrEmpty(opCode) ? "00000000" : opCode;
    }

    /// <inheritdoc />
    protected override INetMessage GetNewNetMessage() => (INetMessage) new DLT645Message();

    /// <inheritdoc cref="M:EstCommunication.Instrument.DLT.DLT645.ActiveDeveice" />
    public OperateResult ActiveDeveice() => (OperateResult) this.ReadFromCoreServer(new byte[4]
    {
      (byte) 254,
      (byte) 254,
      (byte) 254,
      (byte) 254
    }, false);

    private OperateResult<byte[]> ReadWithAddress(string address, byte[] dataArea)
    {
      OperateResult<byte[]> operateResult1 = DLT645.BuildEntireCommand(address, (byte) 17, dataArea);
      if (!operateResult1.IsSuccess)
        return operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return operateResult2;
      OperateResult result = DLT645.CheckResponse(operateResult2.Content);
      if (!result.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>(result);
      return operateResult2.Content.Length < 16 ? OperateResult.CreateSuccessResult<byte[]>(new byte[0]) : OperateResult.CreateSuccessResult<byte[]>(operateResult2.Content.SelectMiddle<byte>(14, operateResult2.Content.Length - 16));
    }

    /// <inheritdoc cref="M:EstCommunication.Instrument.DLT.DLT645.Read(System.String,System.UInt16)" />
    public override OperateResult<byte[]> Read(string address, ushort length)
    {
      OperateResult<string, byte[]> operateResult = DLT645.AnalysisBytesAddress(address, this.station, length);
      return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult) : this.ReadWithAddress(operateResult.Content1, operateResult.Content2);
    }

    /// <inheritdoc />
    public override OperateResult<double[]> ReadDouble(string address, ushort length)
    {
      OperateResult<string, byte[]> operateResult1 = DLT645.AnalysisBytesAddress(address, this.station, length);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<double[]>((OperateResult) operateResult1);
      OperateResult<byte[]> operateResult2 = this.ReadWithAddress(operateResult1.Content1, operateResult1.Content2);
      return !operateResult2.IsSuccess ? OperateResult.CreateFailedResult<double[]>((OperateResult) operateResult2) : DLTTransform.TransDoubleFromDLt(operateResult2.Content, length, DLT645.GetFormatWithDataArea(operateResult1.Content2));
    }

    /// <inheritdoc />
    public override OperateResult<string> ReadString(
      string address,
      ushort length,
      Encoding encoding)
    {
      OperateResult<byte[]> operateResult = this.Read(address, (ushort) 1);
      return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<string>((OperateResult) operateResult) : DLTTransform.TransStringFromDLt(operateResult.Content, length);
    }

    /// <inheritdoc cref="M:EstCommunication.Instrument.DLT.DLT645.ActiveDeveice" />
    public async Task<OperateResult> ActiveDeveiceAsync()
    {
      OperateResult<byte[]> operateResult = await this.ReadFromCoreServerAsync(new byte[4]
      {
        (byte) 254,
        (byte) 254,
        (byte) 254,
        (byte) 254
      }, false);
      return (OperateResult) operateResult;
    }

    private async Task<OperateResult<byte[]>> ReadWithAddressAsync(
      string address,
      byte[] dataArea)
    {
      OperateResult<byte[]> command = DLT645.BuildEntireCommand(address, (byte) 17, dataArea);
      if (!command.IsSuccess)
        return command;
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      if (!read.IsSuccess)
        return read;
      OperateResult check = DLT645.CheckResponse(read.Content);
      return check.IsSuccess ? (read.Content.Length >= 16 ? OperateResult.CreateSuccessResult<byte[]>(read.Content.SelectMiddle<byte>(14, read.Content.Length - 16)) : OperateResult.CreateSuccessResult<byte[]>(new byte[0])) : OperateResult.CreateFailedResult<byte[]>(check);
    }

    /// <inheritdoc cref="M:EstCommunication.Instrument.DLT.DLT645.Read(System.String,System.UInt16)" />
    public override async Task<OperateResult<byte[]>> ReadAsync(
      string address,
      ushort length)
    {
      OperateResult<string, byte[]> analysis = DLT645.AnalysisBytesAddress(address, this.station, length);
      if (!analysis.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) analysis);
      OperateResult<byte[]> operateResult = await this.ReadWithAddressAsync(analysis.Content1, analysis.Content2);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Instrument.DLT.DLT645OverTcp.ReadDouble(System.String,System.UInt16)" />
    public override async Task<OperateResult<double[]>> ReadDoubleAsync(
      string address,
      ushort length)
    {
      OperateResult<string, byte[]> analysis = DLT645.AnalysisBytesAddress(address, this.station, length);
      if (!analysis.IsSuccess)
        return OperateResult.CreateFailedResult<double[]>((OperateResult) analysis);
      OperateResult<byte[]> read = await this.ReadWithAddressAsync(analysis.Content1, analysis.Content2);
      return read.IsSuccess ? DLTTransform.TransDoubleFromDLt(read.Content, length, DLT645.GetFormatWithDataArea(analysis.Content2)) : OperateResult.CreateFailedResult<double[]>((OperateResult) read);
    }

    /// <inheritdoc />
    public override async Task<OperateResult<string>> ReadStringAsync(
      string address,
      ushort length,
      Encoding encoding)
    {
      OperateResult<byte[]> read = await this.ReadAsync(address, (ushort) 1);
      OperateResult<string> operateResult = read.IsSuccess ? DLTTransform.TransStringFromDLt(read.Content, length) : OperateResult.CreateFailedResult<string>((OperateResult) read);
      read = (OperateResult<byte[]>) null;
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Instrument.DLT.DLT645.Write(System.String,System.Byte[])" />
    public override OperateResult Write(string address, byte[] value)
    {
      OperateResult<string, byte[]> operateResult1 = DLT645.AnalysisBytesAddress(address, this.station);
      if (!operateResult1.IsSuccess)
        return (OperateResult) OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult1);
      byte[] dataArea = SoftBasic.SpliceArray<byte>(operateResult1.Content2, this.password.ToHexBytes(), this.opCode.ToHexBytes(), value);
      OperateResult<byte[]> operateResult2 = DLT645.BuildEntireCommand(operateResult1.Content1, (byte) 21, dataArea);
      if (!operateResult2.IsSuccess)
        return (OperateResult) operateResult2;
      OperateResult<byte[]> operateResult3 = this.ReadFromCoreServer(operateResult2.Content);
      return !operateResult3.IsSuccess ? (OperateResult) operateResult3 : DLT645.CheckResponse(operateResult3.Content);
    }

    /// <inheritdoc cref="M:EstCommunication.Instrument.DLT.DLT645.ReadAddress" />
    public OperateResult<string> ReadAddress()
    {
      OperateResult<byte[]> operateResult1 = DLT645.BuildEntireCommand("AAAAAAAAAAAA", (byte) 19, (byte[]) null);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<string>((OperateResult) operateResult1);
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return OperateResult.CreateFailedResult<string>((OperateResult) operateResult2);
      OperateResult result = DLT645.CheckResponse(operateResult2.Content);
      if (!result.IsSuccess)
        return OperateResult.CreateFailedResult<string>(result);
      this.station = ((IEnumerable<byte>) operateResult2.Content.SelectMiddle<byte>(1, 6)).Reverse<byte>().ToArray<byte>().ToHexString();
      return OperateResult.CreateSuccessResult<string>(((IEnumerable<byte>) operateResult2.Content.SelectMiddle<byte>(1, 6)).Reverse<byte>().ToArray<byte>().ToHexString());
    }

    /// <inheritdoc cref="M:EstCommunication.Instrument.DLT.DLT645.WriteAddress(System.String)" />
    public OperateResult WriteAddress(string address)
    {
      OperateResult<byte[]> addressByteFromString = DLT645.GetAddressByteFromString(address);
      if (!addressByteFromString.IsSuccess)
        return (OperateResult) addressByteFromString;
      OperateResult<byte[]> operateResult1 = DLT645.BuildEntireCommand("AAAAAAAAAAAA", (byte) 21, addressByteFromString.Content);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      OperateResult operateResult3 = DLT645.CheckResponse(operateResult2.Content);
      if (!operateResult3.IsSuccess)
        return operateResult3;
      return SoftBasic.IsTwoBytesEquel(operateResult2.Content.SelectMiddle<byte>(1, 6), DLT645.GetAddressByteFromString(address).Content) ? OperateResult.CreateSuccessResult() : new OperateResult(StringResources.Language.DLTErrorWriteReadCheckFailed);
    }

    /// <inheritdoc cref="M:EstCommunication.Instrument.DLT.DLT645.BroadcastTime(System.DateTime)" />
    public OperateResult BroadcastTime(DateTime dateTime)
    {
      OperateResult<byte[]> operateResult = DLT645.BuildEntireCommand("999999999999", (byte) 8, string.Format("{0:D2}{1:D2}{2:D2}{3:D2}{4:D2}{5:D2}", (object) dateTime.Second, (object) dateTime.Minute, (object) dateTime.Hour, (object) dateTime.Day, (object) dateTime.Month, (object) (dateTime.Year % 100)).ToHexBytes());
      return !operateResult.IsSuccess ? (OperateResult) operateResult : (OperateResult) this.ReadFromCoreServer(operateResult.Content, false);
    }

    /// <inheritdoc cref="M:EstCommunication.Instrument.DLT.DLT645.FreezeCommand(System.String)" />
    public OperateResult FreezeCommand(string dataArea)
    {
      OperateResult<string, byte[]> operateResult1 = DLT645.AnalysisBytesAddress(dataArea, this.station);
      if (!operateResult1.IsSuccess)
        return (OperateResult) OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult1);
      OperateResult<byte[]> operateResult2 = DLT645.BuildEntireCommand(operateResult1.Content1, (byte) 22, operateResult1.Content2);
      if (!operateResult2.IsSuccess)
        return (OperateResult) operateResult2;
      if (operateResult1.Content1 == "999999999999")
        return (OperateResult) this.ReadFromCoreServer(operateResult2.Content, false);
      OperateResult<byte[]> operateResult3 = this.ReadFromCoreServer(operateResult2.Content);
      return !operateResult3.IsSuccess ? (OperateResult) operateResult3 : DLT645.CheckResponse(operateResult3.Content);
    }

    /// <inheritdoc cref="M:EstCommunication.Instrument.DLT.DLT645.ChangeBaudRate(System.String)" />
    public OperateResult ChangeBaudRate(string baudRate)
    {
      OperateResult<string, int> operateResult1 = DLT645.AnalysisIntegerAddress(baudRate, this.station);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      byte num;
      switch (operateResult1.Content2)
      {
        case 600:
          num = (byte) 2;
          break;
        case 1200:
          num = (byte) 4;
          break;
        case 2400:
          num = (byte) 8;
          break;
        case 4800:
          num = (byte) 16;
          break;
        case 9600:
          num = (byte) 32;
          break;
        case 19200:
          num = (byte) 64;
          break;
        default:
          return new OperateResult(StringResources.Language.NotSupportedFunction);
      }
      OperateResult<byte[]> operateResult2 = DLT645.BuildEntireCommand(operateResult1.Content1, (byte) 23, new byte[1]
      {
        num
      });
      if (!operateResult2.IsSuccess)
        return (OperateResult) operateResult2;
      OperateResult<byte[]> operateResult3 = this.ReadFromCoreServer(operateResult2.Content);
      if (!operateResult3.IsSuccess)
        return (OperateResult) operateResult3;
      OperateResult operateResult4 = DLT645.CheckResponse(operateResult3.Content);
      if (!operateResult4.IsSuccess)
        return operateResult4;
      return (int) operateResult3.Content[10] == (int) num ? OperateResult.CreateSuccessResult() : new OperateResult(StringResources.Language.DLTErrorWriteReadCheckFailed);
    }

    /// <inheritdoc cref="M:EstCommunication.Instrument.DLT.DLT645OverTcp.Write(System.String,System.Byte[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      byte[] value)
    {
      OperateResult<string, byte[]> analysis = DLT645.AnalysisBytesAddress(address, this.station);
      if (!analysis.IsSuccess)
        return (OperateResult) OperateResult.CreateFailedResult<byte[]>((OperateResult) analysis);
      byte[] content = SoftBasic.SpliceArray<byte>(analysis.Content2, this.password.ToHexBytes(), this.opCode.ToHexBytes(), value);
      OperateResult<byte[]> command = DLT645.BuildEntireCommand(analysis.Content1, (byte) 21, content);
      if (!command.IsSuccess)
        return (OperateResult) command;
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      return read.IsSuccess ? DLT645.CheckResponse(read.Content) : (OperateResult) read;
    }

    /// <inheritdoc cref="M:EstCommunication.Instrument.DLT.DLT645.ReadAddress" />
    public async Task<OperateResult<string>> ReadAddressAsync()
    {
      OperateResult<byte[]> command = DLT645.BuildEntireCommand("AAAAAAAAAAAA", (byte) 19, (byte[]) null);
      if (!command.IsSuccess)
        return OperateResult.CreateFailedResult<string>((OperateResult) command);
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<string>((OperateResult) read);
      OperateResult check = DLT645.CheckResponse(read.Content);
      if (!check.IsSuccess)
        return OperateResult.CreateFailedResult<string>(check);
      this.station = ((IEnumerable<byte>) read.Content.SelectMiddle<byte>(1, 6)).Reverse<byte>().ToArray<byte>().ToHexString();
      return OperateResult.CreateSuccessResult<string>(((IEnumerable<byte>) read.Content.SelectMiddle<byte>(1, 6)).Reverse<byte>().ToArray<byte>().ToHexString());
    }

    /// <inheritdoc cref="M:EstCommunication.Instrument.DLT.DLT645.WriteAddress(System.String)" />
    public async Task<OperateResult> WriteAddressAsync(string address)
    {
      OperateResult<byte[]> add = DLT645.GetAddressByteFromString(address);
      if (!add.IsSuccess)
        return (OperateResult) add;
      OperateResult<byte[]> command = DLT645.BuildEntireCommand("AAAAAAAAAAAA", (byte) 21, add.Content);
      if (!command.IsSuccess)
        return (OperateResult) command;
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      OperateResult check = DLT645.CheckResponse(read.Content);
      return check.IsSuccess ? (!SoftBasic.IsTwoBytesEquel(read.Content.SelectMiddle<byte>(1, 6), DLT645.GetAddressByteFromString(address).Content) ? new OperateResult(StringResources.Language.DLTErrorWriteReadCheckFailed) : OperateResult.CreateSuccessResult()) : check;
    }

    /// <inheritdoc cref="M:EstCommunication.Instrument.DLT.DLT645.BroadcastTime(System.DateTime)" />
    public async Task<OperateResult> BroadcastTimeAsync(DateTime dateTime)
    {
      string hex = string.Format("{0:D2}{1:D2}{2:D2}{3:D2}{4:D2}{5:D2}", (object) dateTime.Second, (object) dateTime.Minute, (object) dateTime.Hour, (object) dateTime.Day, (object) dateTime.Month, (object) (dateTime.Year % 100));
      OperateResult<byte[]> command = DLT645.BuildEntireCommand("999999999999", (byte) 8, hex.ToHexBytes());
      if (!command.IsSuccess)
        return (OperateResult) command;
      OperateResult<byte[]> operateResult = await this.ReadFromCoreServerAsync(command.Content, false);
      return (OperateResult) operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Instrument.DLT.DLT645.FreezeCommand(System.String)" />
    public async Task<OperateResult> FreezeCommandAsync(string dataArea)
    {
      OperateResult<string, byte[]> analysis = DLT645.AnalysisBytesAddress(dataArea, this.station);
      if (!analysis.IsSuccess)
        return (OperateResult) OperateResult.CreateFailedResult<byte[]>((OperateResult) analysis);
      OperateResult<byte[]> command = DLT645.BuildEntireCommand(analysis.Content1, (byte) 22, analysis.Content2);
      if (!command.IsSuccess)
        return (OperateResult) command;
      if (analysis.Content1 == "999999999999")
      {
        OperateResult<byte[]> operateResult = await this.ReadFromCoreServerAsync(command.Content, false);
        return (OperateResult) operateResult;
      }
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      return read.IsSuccess ? DLT645.CheckResponse(read.Content) : (OperateResult) read;
    }

    /// <inheritdoc cref="M:EstCommunication.Instrument.DLT.DLT645.ChangeBaudRate(System.String)" />
    public async Task<OperateResult> ChangeBaudRateAsync(string baudRate)
    {
      OperateResult<string, int> analysis = DLT645.AnalysisIntegerAddress(baudRate, this.station);
      if (!analysis.IsSuccess)
        return (OperateResult) analysis;
      byte code = 0;
      int content2 = analysis.Content2;
      switch (content2)
      {
        case 600:
          code = (byte) 2;
          break;
        case 1200:
          code = (byte) 4;
          break;
        case 2400:
          code = (byte) 8;
          break;
        case 4800:
          code = (byte) 16;
          break;
        case 9600:
          code = (byte) 32;
          break;
        case 19200:
          code = (byte) 64;
          break;
        default:
          return new OperateResult(StringResources.Language.NotSupportedFunction);
      }
      OperateResult<byte[]> command = DLT645.BuildEntireCommand(analysis.Content1, (byte) 23, new byte[1]
      {
        code
      });
      if (!command.IsSuccess)
        return (OperateResult) command;
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      if (!read.IsSuccess)
        return (OperateResult) read;
      OperateResult check = DLT645.CheckResponse(read.Content);
      return check.IsSuccess ? ((int) read.Content[10] != (int) code ? new OperateResult(StringResources.Language.DLTErrorWriteReadCheckFailed) : OperateResult.CreateSuccessResult()) : check;
    }

    /// <inheritdoc cref="P:EstCommunication.Instrument.DLT.DLT645.Station" />
    public string Station
    {
      get => this.station;
      set => this.station = value;
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("DLT645OverTcp[{0}:{1}]", (object) this.IpAddress, (object) this.Port);
  }
}
