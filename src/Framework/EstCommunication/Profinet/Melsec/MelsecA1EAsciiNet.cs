// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.Melsec.MelsecA1EAsciiNet
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using EstCommunication.Core;
using EstCommunication.Core.IMessage;
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
  /// 三菱PLC通讯协议，采用A兼容1E帧协议实现，使用ASCII码通讯，请根据实际型号来进行选取<br />
  /// Mitsubishi PLC communication protocol, implemented using A compatible 1E frame protocol, using ascii code communication, please choose according to the actual model
  /// </summary>
  /// <remarks>
  /// <inheritdoc cref="T:EstCommunication.Profinet.Melsec.MelsecA1ENet" path="remarks" />
  /// </remarks>
  public class MelsecA1EAsciiNet : NetworkDeviceBase
  {
    /// <summary>
    /// 实例化一个默认的对象<br />
    /// Instantiate a default object
    /// </summary>
    public MelsecA1EAsciiNet()
    {
      this.WordLength = (ushort) 1;
      this.LogMsgFormatBinary = false;
      this.ByteTransform = (IByteTransform) new RegularByteTransform();
    }

    /// <summary>
    /// 指定ip地址和端口来来实例化一个默认的对象<br />
    /// Specify the IP address and port to instantiate a default object
    /// </summary>
    /// <param name="ipAddress">PLC的Ip地址</param>
    /// <param name="port">PLC的端口</param>
    public MelsecA1EAsciiNet(string ipAddress, int port)
    {
      this.WordLength = (ushort) 1;
      this.IpAddress = ipAddress;
      this.Port = port;
      this.LogMsgFormatBinary = false;
      this.ByteTransform = (IByteTransform) new RegularByteTransform();
    }

    /// <inheritdoc />
    protected override INetMessage GetNewNetMessage() => (INetMessage) new MelsecA1EAsciiMessage();

    /// <inheritdoc cref="P:EstCommunication.Profinet.Melsec.MelsecA1ENet.PLCNumber" />
    public byte PLCNumber { get; set; } = byte.MaxValue;

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecA1ENet.Read(System.String,System.UInt16)" />
    [EstMqttApi("ReadByteArray", "")]
    public override OperateResult<byte[]> Read(string address, ushort length)
    {
      OperateResult<byte[]> operateResult1 = MelsecA1EAsciiNet.BuildReadCommand(address, length, false, this.PLCNumber);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult1);
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult2);
      OperateResult result = MelsecA1EAsciiNet.CheckResponseLegal(operateResult2.Content);
      return !result.IsSuccess ? OperateResult.CreateFailedResult<byte[]>(result) : MelsecA1EAsciiNet.ExtractActualData(operateResult2.Content, false);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecA1ENet.Write(System.String,System.Byte[])" />
    [EstMqttApi("WriteByteArray", "")]
    public override OperateResult Write(string address, byte[] value)
    {
      OperateResult<byte[]> operateResult1 = MelsecA1EAsciiNet.BuildWriteWordCommand(address, value, this.PLCNumber);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return (OperateResult) operateResult2;
      OperateResult result = MelsecA1EAsciiNet.CheckResponseLegal(operateResult2.Content);
      return !result.IsSuccess ? (OperateResult) OperateResult.CreateFailedResult<byte[]>(result) : OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecA1EAsciiNet.Read(System.String,System.UInt16)" />
    public override async Task<OperateResult<byte[]>> ReadAsync(
      string address,
      ushort length)
    {
      OperateResult<byte[]> command = MelsecA1EAsciiNet.BuildReadCommand(address, length, false, this.PLCNumber);
      if (!command.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) command);
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) read);
      OperateResult check = MelsecA1EAsciiNet.CheckResponseLegal(read.Content);
      return check.IsSuccess ? MelsecA1EAsciiNet.ExtractActualData(read.Content, false) : OperateResult.CreateFailedResult<byte[]>(check);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecA1EAsciiNet.Write(System.String,System.Byte[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      byte[] value)
    {
      OperateResult<byte[]> command = MelsecA1EAsciiNet.BuildWriteWordCommand(address, value, this.PLCNumber);
      if (!command.IsSuccess)
        return (OperateResult) command;
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      if (!read.IsSuccess)
        return (OperateResult) read;
      OperateResult check = MelsecA1EAsciiNet.CheckResponseLegal(read.Content);
      return check.IsSuccess ? OperateResult.CreateSuccessResult() : (OperateResult) OperateResult.CreateFailedResult<byte[]>(check);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecA1ENet.ReadBool(System.String,System.UInt16)" />
    [EstMqttApi("ReadBoolArray", "")]
    public override OperateResult<bool[]> ReadBool(string address, ushort length)
    {
      OperateResult<byte[]> operateResult1 = MelsecA1EAsciiNet.BuildReadCommand(address, length, true, this.PLCNumber);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult1);
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult2);
      OperateResult result = MelsecA1EAsciiNet.CheckResponseLegal(operateResult2.Content);
      if (!result.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>(result);
      OperateResult<byte[]> actualData = MelsecA1EAsciiNet.ExtractActualData(operateResult2.Content, true);
      return !actualData.IsSuccess ? OperateResult.CreateFailedResult<bool[]>((OperateResult) actualData) : OperateResult.CreateSuccessResult<bool[]>(((IEnumerable<byte>) actualData.Content).Select<byte, bool>((Func<byte, bool>) (m => m == (byte) 1)).Take<bool>((int) length).ToArray<bool>());
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecA1ENet.Write(System.String,System.Boolean[])" />
    [EstMqttApi("WriteBoolArray", "")]
    public override OperateResult Write(string address, bool[] values)
    {
      OperateResult<byte[]> operateResult1 = MelsecA1EAsciiNet.BuildWriteBoolCommand(address, values, this.PLCNumber);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      return !operateResult2.IsSuccess ? (OperateResult) operateResult2 : MelsecA1EAsciiNet.CheckResponseLegal(operateResult2.Content);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecA1EAsciiNet.ReadBool(System.String,System.UInt16)" />
    public override async Task<OperateResult<bool[]>> ReadBoolAsync(
      string address,
      ushort length)
    {
      OperateResult<byte[]> command = MelsecA1EAsciiNet.BuildReadCommand(address, length, true, this.PLCNumber);
      if (!command.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) command);
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) read);
      OperateResult check = MelsecA1EAsciiNet.CheckResponseLegal(read.Content);
      if (!check.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>(check);
      OperateResult<byte[]> extract = MelsecA1EAsciiNet.ExtractActualData(read.Content, true);
      return extract.IsSuccess ? OperateResult.CreateSuccessResult<bool[]>(((IEnumerable<byte>) extract.Content).Select<byte, bool>((Func<byte, bool>) (m => m == (byte) 1)).Take<bool>((int) length).ToArray<bool>()) : OperateResult.CreateFailedResult<bool[]>((OperateResult) extract);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecA1EAsciiNet.Write(System.String,System.Boolean[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      bool[] values)
    {
      OperateResult<byte[]> command = MelsecA1EAsciiNet.BuildWriteBoolCommand(address, values, this.PLCNumber);
      if (!command.IsSuccess)
        return (OperateResult) command;
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      return read.IsSuccess ? MelsecA1EAsciiNet.CheckResponseLegal(read.Content) : (OperateResult) read;
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("MelsecA1ENet[{0}:{1}]", (object) this.IpAddress, (object) this.Port);

    /// <summary>根据类型地址长度确认需要读取的指令头</summary>
    /// <param name="address">起始地址</param>
    /// <param name="length">长度</param>
    /// <param name="isBit">指示是否按照位成批的读出</param>
    /// <param name="plcNumber">PLC编号</param>
    /// <returns>带有成功标志的指令数据</returns>
    public static OperateResult<byte[]> BuildReadCommand(
      string address,
      ushort length,
      bool isBit,
      byte plcNumber)
    {
      OperateResult<MelsecA1EDataType, int> operateResult = MelsecHelper.McA1EAnalysisAddress(address);
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult);
      byte num = isBit ? (byte) 0 : (byte) 1;
      return OperateResult.CreateSuccessResult<byte[]>(new byte[24]
      {
        SoftBasic.BuildAsciiBytesFrom(num)[0],
        SoftBasic.BuildAsciiBytesFrom(num)[1],
        SoftBasic.BuildAsciiBytesFrom(plcNumber)[0],
        SoftBasic.BuildAsciiBytesFrom(plcNumber)[1],
        (byte) 48,
        (byte) 48,
        (byte) 48,
        (byte) 65,
        SoftBasic.BuildAsciiBytesFrom(operateResult.Content1.DataCode[0])[0],
        SoftBasic.BuildAsciiBytesFrom(operateResult.Content1.DataCode[0])[1],
        SoftBasic.BuildAsciiBytesFrom(operateResult.Content1.DataCode[1])[0],
        SoftBasic.BuildAsciiBytesFrom(operateResult.Content1.DataCode[1])[1],
        SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[3])[0],
        SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[3])[1],
        SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[2])[0],
        SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[2])[1],
        SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[1])[0],
        SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[1])[1],
        SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[0])[0],
        SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[0])[1],
        SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes((int) length % 256)[0])[0],
        SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes((int) length % 256)[0])[1],
        (byte) 48,
        (byte) 48
      });
    }

    /// <summary>根据类型地址以及需要写入的数据来生成指令头</summary>
    /// <param name="address">起始地址</param>
    /// <param name="value">数据值</param>
    /// <param name="plcNumber">PLC编号</param>
    /// <returns>带有成功标志的指令数据</returns>
    public static OperateResult<byte[]> BuildWriteWordCommand(
      string address,
      byte[] value,
      byte plcNumber)
    {
      OperateResult<MelsecA1EDataType, int> operateResult = MelsecHelper.McA1EAnalysisAddress(address);
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult);
      value = MelsecHelper.TransByteArrayToAsciiByteArray(value);
      byte[] numArray = new byte[24 + value.Length];
      numArray[0] = (byte) 48;
      numArray[1] = (byte) 51;
      numArray[2] = SoftBasic.BuildAsciiBytesFrom(plcNumber)[0];
      numArray[3] = SoftBasic.BuildAsciiBytesFrom(plcNumber)[1];
      numArray[4] = (byte) 48;
      numArray[5] = (byte) 48;
      numArray[6] = (byte) 48;
      numArray[7] = (byte) 65;
      numArray[8] = SoftBasic.BuildAsciiBytesFrom(operateResult.Content1.DataCode[0])[0];
      numArray[9] = SoftBasic.BuildAsciiBytesFrom(operateResult.Content1.DataCode[0])[1];
      numArray[10] = SoftBasic.BuildAsciiBytesFrom(operateResult.Content1.DataCode[1])[0];
      numArray[11] = SoftBasic.BuildAsciiBytesFrom(operateResult.Content1.DataCode[1])[1];
      numArray[12] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[3])[0];
      numArray[13] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[3])[1];
      numArray[14] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[2])[0];
      numArray[15] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[2])[1];
      numArray[16] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[1])[0];
      numArray[17] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[1])[1];
      numArray[18] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[0])[0];
      numArray[19] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[0])[1];
      numArray[20] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(value.Length / 4)[0])[0];
      numArray[21] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(value.Length / 4)[0])[1];
      numArray[22] = (byte) 48;
      numArray[23] = (byte) 48;
      value.CopyTo((Array) numArray, 24);
      return OperateResult.CreateSuccessResult<byte[]>(numArray);
    }

    /// <summary>根据类型地址以及需要写入的数据来生成指令头</summary>
    /// <param name="address">起始地址</param>
    /// <param name="value">数据值</param>
    /// <param name="plcNumber">PLC编号</param>
    /// <returns>带有成功标志的指令数据</returns>
    public static OperateResult<byte[]> BuildWriteBoolCommand(
      string address,
      bool[] value,
      byte plcNumber)
    {
      OperateResult<MelsecA1EDataType, int> operateResult = MelsecHelper.McA1EAnalysisAddress(address);
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult);
      byte[] numArray1 = ((IEnumerable<bool>) value).Select<bool, byte>((Func<bool, byte>) (m => !m ? (byte) 48 : (byte) 49)).ToArray<byte>();
      if (numArray1.Length % 2 == 1)
        numArray1 = SoftBasic.SpliceArray<byte>(numArray1, new byte[1]
        {
          (byte) 48
        });
      byte[] numArray2 = new byte[24 + numArray1.Length];
      numArray2[0] = (byte) 48;
      numArray2[1] = (byte) 50;
      numArray2[2] = SoftBasic.BuildAsciiBytesFrom(plcNumber)[0];
      numArray2[3] = SoftBasic.BuildAsciiBytesFrom(plcNumber)[1];
      numArray2[4] = (byte) 48;
      numArray2[5] = (byte) 48;
      numArray2[6] = (byte) 48;
      numArray2[7] = (byte) 65;
      numArray2[8] = SoftBasic.BuildAsciiBytesFrom(operateResult.Content1.DataCode[0])[0];
      numArray2[9] = SoftBasic.BuildAsciiBytesFrom(operateResult.Content1.DataCode[0])[1];
      numArray2[10] = SoftBasic.BuildAsciiBytesFrom(operateResult.Content1.DataCode[1])[0];
      numArray2[11] = SoftBasic.BuildAsciiBytesFrom(operateResult.Content1.DataCode[1])[1];
      numArray2[12] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[3])[0];
      numArray2[13] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[3])[1];
      numArray2[14] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[2])[0];
      numArray2[15] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[2])[1];
      numArray2[16] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[1])[0];
      numArray2[17] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[1])[1];
      numArray2[18] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[0])[0];
      numArray2[19] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(operateResult.Content2)[0])[1];
      numArray2[20] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(value.Length)[0])[0];
      numArray2[21] = SoftBasic.BuildAsciiBytesFrom(BitConverter.GetBytes(value.Length)[0])[1];
      numArray2[22] = (byte) 48;
      numArray2[23] = (byte) 48;
      numArray1.CopyTo((Array) numArray2, 24);
      return OperateResult.CreateSuccessResult<byte[]>(numArray2);
    }

    /// <summary>检测反馈的消息是否合法</summary>
    /// <param name="response">接收的报文</param>
    /// <returns>是否成功</returns>
    public static OperateResult CheckResponseLegal(byte[] response)
    {
      if (response.Length < 4)
        return new OperateResult(StringResources.Language.ReceiveDataLengthTooShort);
      if (response[2] == (byte) 48 && response[3] == (byte) 48)
        return OperateResult.CreateSuccessResult();
      return response[2] == (byte) 53 && response[3] == (byte) 66 ? new OperateResult(Convert.ToInt32(Encoding.ASCII.GetString(response, 4, 2), 16), StringResources.Language.MelsecPleaseReferToManualDocument) : new OperateResult(Convert.ToInt32(Encoding.ASCII.GetString(response, 2, 2), 16), StringResources.Language.MelsecPleaseReferToManualDocument);
    }

    /// <summary>从PLC反馈的数据中提取出实际的数据内容，需要传入反馈数据，是否位读取</summary>
    /// <param name="response">反馈的数据内容</param>
    /// <param name="isBit">是否位读取</param>
    /// <returns>解析后的结果对象</returns>
    public static OperateResult<byte[]> ExtractActualData(byte[] response, bool isBit) => isBit ? OperateResult.CreateSuccessResult<byte[]>(((IEnumerable<byte>) response.RemoveBegin<byte>(4)).Select<byte, byte>((Func<byte, byte>) (m => m != (byte) 48 ? (byte) 1 : (byte) 0)).ToArray<byte>()) : OperateResult.CreateSuccessResult<byte[]>(MelsecHelper.TransAsciiByteArrayToByteArray(response.RemoveBegin<byte>(4)));
  }
}
