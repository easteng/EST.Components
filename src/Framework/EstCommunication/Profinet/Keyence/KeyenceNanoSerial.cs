// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.Keyence.KeyenceNanoSerial
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using EstCommunication.Core;
using EstCommunication.Reflection;
using EstCommunication.Serial;
using System;
using System.Threading.Tasks;

namespace EstCommunication.Profinet.Keyence
{
  /// <summary>
  /// 基恩士KV上位链路串口通信的对象,适用于Nano系列串口数据,KV1000以及L20V通信模块，地址格式参考api文档<br />
  /// Keyence KV upper link serial communication object, suitable for Nano series serial data, and L20V communication module, please refer to api document for address format
  /// </summary>
  /// <remarks>
  /// <inheritdoc cref="T:EstCommunication.Profinet.Keyence.KeyenceNanoSerialOverTcp" path="remarks" />
  /// </remarks>
  /// <example>
  /// <inheritdoc cref="T:EstCommunication.Profinet.Keyence.KeyenceNanoSerialOverTcp" path="example" />
  /// </example>
  public class KeyenceNanoSerial : SerialDeviceBase
  {
    /// <summary>
    /// 实例化基恩士的串口协议的通讯对象<br />
    /// Instantiate the communication object of Keyence's serial protocol
    /// </summary>
    public KeyenceNanoSerial()
    {
      this.ByteTransform = (IByteTransform) new RegularByteTransform();
      this.WordLength = (ushort) 1;
    }

    /// <inheritdoc />
    protected override OperateResult InitializationOnOpen()
    {
      OperateResult<byte[]> operateResult = this.ReadBase(KeyenceNanoSerialOverTcp.GetConnectCmd(this.Station));
      if (!operateResult.IsSuccess)
        return (OperateResult) operateResult;
      return operateResult.Content.Length > 2 && (operateResult.Content[0] == (byte) 67 && operateResult.Content[1] == (byte) 67) ? OperateResult.CreateSuccessResult() : new OperateResult("Check Failed: " + SoftBasic.ByteToHexString(operateResult.Content, ' '));
    }

    /// <inheritdoc />
    protected override OperateResult ExtraOnClose()
    {
      OperateResult<byte[]> operateResult = this.ReadBase(KeyenceNanoSerialOverTcp.GetDisConnectCmd(this.Station));
      if (!operateResult.IsSuccess)
        return (OperateResult) operateResult;
      return operateResult.Content.Length > 2 && (operateResult.Content[0] == (byte) 67 && operateResult.Content[1] == (byte) 70) ? OperateResult.CreateSuccessResult() : new OperateResult("Check Failed: " + SoftBasic.ByteToHexString(operateResult.Content, ' '));
    }

    /// <inheritdoc cref="P:EstCommunication.Profinet.Keyence.KeyenceNanoSerialOverTcp.Station" />
    public byte Station { get; set; }

    /// <inheritdoc />
    [EstMqttApi("ReadByteArray", "")]
    public override OperateResult<byte[]> Read(string address, ushort length)
    {
      if (address.StartsWith("unit="))
      {
        byte parameter = (byte) EstHelper.ExtractParameter(ref address, "unit", 0);
        return !ushort.TryParse(address, out ushort _) ? new OperateResult<byte[]>("Address is not right, convert ushort wrong!") : this.ReadExpansionMemory(parameter, ushort.Parse(address), length);
      }
      OperateResult<byte[]> operateResult1 = KeyenceNanoSerialOverTcp.BuildReadCommand(address, length);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult1);
      OperateResult<byte[]> operateResult2 = this.ReadBase(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult2);
      OperateResult result = KeyenceNanoSerialOverTcp.CheckPlcReadResponse(operateResult2.Content);
      if (!result.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>(result);
      OperateResult<string, int> operateResult3 = KeyenceNanoSerialOverTcp.KvAnalysisAddress(address);
      return !operateResult3.IsSuccess ? OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult3) : KeyenceNanoSerialOverTcp.ExtractActualData(operateResult3.Content1, operateResult2.Content);
    }

    /// <inheritdoc />
    [EstMqttApi("WriteByteArray", "")]
    public override OperateResult Write(string address, byte[] value)
    {
      OperateResult<byte[]> operateResult1 = KeyenceNanoSerialOverTcp.BuildWriteCommand(address, value);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadBase(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return (OperateResult) operateResult2;
      OperateResult operateResult3 = KeyenceNanoSerialOverTcp.CheckPlcWriteResponse(operateResult2.Content);
      return !operateResult3.IsSuccess ? operateResult3 : OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc />
    [EstMqttApi("ReadBoolArray", "")]
    public override OperateResult<bool[]> ReadBool(string address, ushort length)
    {
      OperateResult<byte[]> operateResult1 = KeyenceNanoSerialOverTcp.BuildReadCommand(address, length);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult1);
      OperateResult<byte[]> operateResult2 = this.ReadBase(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult2);
      OperateResult result = KeyenceNanoSerialOverTcp.CheckPlcReadResponse(operateResult2.Content);
      if (!result.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>(result);
      OperateResult<string, int> operateResult3 = KeyenceNanoSerialOverTcp.KvAnalysisAddress(address);
      return !operateResult3.IsSuccess ? OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult3) : KeyenceNanoSerialOverTcp.ExtractActualBoolData(operateResult3.Content1, operateResult2.Content);
    }

    /// <inheritdoc />
    [EstMqttApi("WriteBool", "")]
    public override OperateResult Write(string address, bool value)
    {
      OperateResult<byte[]> operateResult1 = KeyenceNanoSerialOverTcp.BuildWriteCommand(address, value);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadBase(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return (OperateResult) operateResult2;
      OperateResult operateResult3 = KeyenceNanoSerialOverTcp.CheckPlcWriteResponse(operateResult2.Content);
      return !operateResult3.IsSuccess ? operateResult3 : OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc />
    [EstMqttApi("WriteBoolArray", "")]
    public override OperateResult Write(string address, bool[] value)
    {
      OperateResult<byte[]> operateResult1 = KeyenceNanoSerialOverTcp.BuildWriteCommand(address, value);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadBase(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return (OperateResult) operateResult2;
      OperateResult operateResult3 = KeyenceNanoSerialOverTcp.CheckPlcWriteResponse(operateResult2.Content);
      return !operateResult3.IsSuccess ? operateResult3 : OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Keyence.KeyenceNanoSerial.Write(System.String,System.Boolean)" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      bool value)
    {
      OperateResult operateResult = await Task.Run<OperateResult>((Func<OperateResult>) (() => this.Write(address, value)));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Keyence.KeyenceNanoSerialOverTcp.ReadPlcType" />
    [EstMqttApi("查询PLC的型号信息")]
    public OperateResult<KeyencePLCS> ReadPlcType() => KeyenceNanoSerialOverTcp.ReadPlcTypeHelper(new Func<byte[], OperateResult<byte[]>>(((SerialBase) this).ReadBase));

    /// <inheritdoc cref="M:EstCommunication.Profinet.Keyence.KeyenceNanoSerialOverTcp.ReadPlcMode" />
    [EstMqttApi("读取当前PLC的模式，如果是0，代表 PROG模式或者梯形图未登录，如果为1，代表RUN模式")]
    public OperateResult<int> ReadPlcMode() => KeyenceNanoSerialOverTcp.ReadPlcModeHelper(new Func<byte[], OperateResult<byte[]>>(((SerialBase) this).ReadBase));

    /// <inheritdoc cref="M:EstCommunication.Profinet.Keyence.KeyenceNanoSerialOverTcp.SetPlcDateTime(System.DateTime)" />
    [EstMqttApi("设置PLC的时间")]
    public OperateResult SetPlcDateTime(DateTime dateTime) => KeyenceNanoSerialOverTcp.SetPlcDateTimeHelper(new Func<byte[], OperateResult<byte[]>>(((SerialBase) this).ReadBase), dateTime);

    /// <inheritdoc cref="M:EstCommunication.Profinet.Keyence.KeyenceNanoSerialOverTcp.ReadAddressAnnotation(System.String)" />
    [EstMqttApi("读取指定软元件的注释信息")]
    public OperateResult<string> ReadAddressAnnotation(string address) => KeyenceNanoSerialOverTcp.ReadAddressAnnotationHelper(new Func<byte[], OperateResult<byte[]>>(((SerialBase) this).ReadBase), address);

    /// <inheritdoc cref="M:EstCommunication.Profinet.Keyence.KeyenceNanoSerialOverTcp.ReadExpansionMemory(System.Byte,System.UInt16,System.UInt16)" />
    [EstMqttApi("从扩展单元缓冲存储器连续读取指定个数的数据，单位为字")]
    public OperateResult<byte[]> ReadExpansionMemory(
      byte unit,
      ushort address,
      ushort length)
    {
      return KeyenceNanoSerialOverTcp.ReadExpansionMemoryHelper(new Func<byte[], OperateResult<byte[]>>(((SerialBase) this).ReadBase), unit, address, length);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Keyence.KeyenceNanoSerialOverTcp.WriteExpansionMemory(System.Byte,System.UInt16,System.Byte[])" />
    [EstMqttApi("将原始字节数据写入到扩展的缓冲存储器，需要指定单元编号，偏移地址，写入的数据")]
    public OperateResult WriteExpansionMemory(byte unit, ushort address, byte[] value) => KeyenceNanoSerialOverTcp.WriteExpansionMemoryHelper(new Func<byte[], OperateResult<byte[]>>(((SerialBase) this).ReadBase), unit, address, value);

    /// <inheritdoc />
    public override string ToString() => string.Format("KeyenceNanoSerial[{0}:{1}]", (object) this.PortName, (object) this.BaudRate);
  }
}
