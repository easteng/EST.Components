// Decompiled with JetBrains decompiler
// Type: EstCommunication.ModBus.ModbusUdpNet
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using EstCommunication.Core;
using EstCommunication.Core.Net;
using EstCommunication.Reflection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EstCommunication.ModBus
{
  /// <summary>
  /// Modbus-Udp协议的客户端通讯类，方便的和服务器进行数据交互，支持标准的功能码，也支持扩展的功能码实现，地址采用富文本的形式，详细见备注说明<br />
  /// The client communication class of Modbus-Udp protocol is convenient for data interaction with the server. It supports standard function codes and also supports extended function codes.
  /// The address is in rich text. For details, see the remarks.
  /// </summary>
  /// <remarks>
  /// 本客户端支持的标准的modbus协议，Modbus-Tcp及Modbus-Udp内置的消息号会进行自增，地址支持富文本格式，具体参考示例代码。<br />
  /// 读取线圈，输入线圈，寄存器，输入寄存器的方法中的读取长度对商业授权用户不限制，内部自动切割读取，结果合并。
  /// </remarks>
  /// <example>
  /// <inheritdoc cref="T:EstCommunication.ModBus.ModbusTcpNet" path="example" />
  /// </example>
  public class ModbusUdpNet : NetworkUdpDeviceBase
  {
    private byte station = 1;
    private SoftIncrementCount softIncrementCount;
    private bool isAddressStartWithZero = true;

    /// <summary>
    /// 实例化一个MOdbus-Udp协议的客户端对象<br />
    /// Instantiate a client object of the MOdbus-Udp protocol
    /// </summary>
    public ModbusUdpNet()
    {
      this.ByteTransform = (IByteTransform) new ReverseWordTransform();
      this.softIncrementCount = new SoftIncrementCount((long) ushort.MaxValue);
      this.WordLength = (ushort) 1;
      this.station = (byte) 1;
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.#ctor(System.String,System.Int32,System.Byte)" />
    public ModbusUdpNet(string ipAddress, int port = 502, byte station = 1)
    {
      this.ByteTransform = (IByteTransform) new ReverseWordTransform();
      this.softIncrementCount = new SoftIncrementCount((long) ushort.MaxValue);
      this.IpAddress = ipAddress;
      this.Port = port;
      this.WordLength = (ushort) 1;
      this.station = station;
    }

    /// <inheritdoc cref="P:EstCommunication.ModBus.ModbusTcpNet.AddressStartWithZero" />
    public bool AddressStartWithZero
    {
      get => this.isAddressStartWithZero;
      set => this.isAddressStartWithZero = value;
    }

    /// <inheritdoc cref="P:EstCommunication.ModBus.ModbusTcpNet.Station" />
    public byte Station
    {
      get => this.station;
      set => this.station = value;
    }

    /// <inheritdoc cref="P:EstCommunication.ModBus.ModbusTcpNet.DataFormat" />
    public DataFormat DataFormat
    {
      get => this.ByteTransform.DataFormat;
      set => this.ByteTransform.DataFormat = value;
    }

    /// <inheritdoc cref="P:EstCommunication.ModBus.ModbusTcpNet.IsStringReverse" />
    public bool IsStringReverse
    {
      get => this.ByteTransform.IsStringReverseByteWord;
      set => this.ByteTransform.IsStringReverseByteWord = value;
    }

    /// <inheritdoc cref="P:EstCommunication.ModBus.ModbusTcpNet.MessageId" />
    public SoftIncrementCount MessageId => this.softIncrementCount;

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.ReadCoil(System.String)" />
    public OperateResult<bool> ReadCoil(string address) => this.ReadBool(address);

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.ReadCoil(System.String,System.UInt16)" />
    public OperateResult<bool[]> ReadCoil(string address, ushort length) => this.ReadBool(address, length);

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.ReadDiscrete(System.String)" />
    public OperateResult<bool> ReadDiscrete(string address) => ByteTransformHelper.GetResultFromArray<bool>(this.ReadDiscrete(address, (ushort) 1));

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusUdpNet.ReadDiscrete(System.String,System.UInt16)" />
    public OperateResult<bool[]> ReadDiscrete(string address, ushort length) => this.ReadBoolHelper(address, length, (byte) 2);

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.Read(System.String,System.UInt16)" />
    [EstMqttApi("ReadByteArray", "")]
    public override OperateResult<byte[]> Read(string address, ushort length)
    {
      OperateResult<byte[][]> operateResult1 = ModbusInfo.BuildReadModbusCommand(address, length, this.Station, this.AddressStartWithZero, (byte) 3);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult1);
      List<byte> byteList = new List<byte>();
      for (int index = 0; index < operateResult1.Content.Length; ++index)
      {
        OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(ModbusInfo.PackCommandToTcp(operateResult1.Content[index], (ushort) this.softIncrementCount.GetCurrentValue()));
        if (!operateResult2.IsSuccess)
          return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult2);
        OperateResult<byte[]> actualData = ModbusInfo.ExtractActualData(ModbusInfo.ExplodeTcpCommandToCore(operateResult2.Content));
        if (!actualData.IsSuccess)
          return OperateResult.CreateFailedResult<byte[]>((OperateResult) actualData);
        byteList.AddRange((IEnumerable<byte>) actualData.Content);
      }
      return OperateResult.CreateSuccessResult<byte[]>(byteList.ToArray());
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.Write(System.String,System.Byte[])" />
    [EstMqttApi("WriteByteArray", "")]
    public override OperateResult Write(string address, byte[] value)
    {
      OperateResult<byte[]> operateResult1 = ModbusInfo.BuildWriteWordModbusCommand(address, value, this.Station, this.AddressStartWithZero, (byte) 16);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(ModbusInfo.PackCommandToTcp(operateResult1.Content, (ushort) this.softIncrementCount.GetCurrentValue()));
      return !operateResult2.IsSuccess ? (OperateResult) operateResult2 : (OperateResult) ModbusInfo.ExtractActualData(ModbusInfo.ExplodeTcpCommandToCore(operateResult2.Content));
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.Write(System.String,System.Int16)" />
    [EstMqttApi("WriteInt16", "")]
    public override OperateResult Write(string address, short value)
    {
      OperateResult<byte[]> operateResult1 = ModbusInfo.BuildWriteWordModbusCommand(address, value, this.Station, this.AddressStartWithZero, (byte) 6);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(ModbusInfo.PackCommandToTcp(operateResult1.Content, (ushort) this.softIncrementCount.GetCurrentValue()));
      return !operateResult2.IsSuccess ? (OperateResult) operateResult2 : (OperateResult) ModbusInfo.ExtractActualData(ModbusInfo.ExplodeTcpCommandToCore(operateResult2.Content));
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.Write(System.String,System.UInt16)" />
    [EstMqttApi("WriteUInt16", "")]
    public override OperateResult Write(string address, ushort value)
    {
      OperateResult<byte[]> operateResult1 = ModbusInfo.BuildWriteWordModbusCommand(address, value, this.Station, this.AddressStartWithZero, (byte) 6);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(ModbusInfo.PackCommandToTcp(operateResult1.Content, (ushort) this.softIncrementCount.GetCurrentValue()));
      return !operateResult2.IsSuccess ? (OperateResult) operateResult2 : (OperateResult) ModbusInfo.ExtractActualData(ModbusInfo.ExplodeTcpCommandToCore(operateResult2.Content));
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.WriteMask(System.String,System.UInt16,System.UInt16)" />
    [EstMqttApi("WriteMask", "")]
    public OperateResult WriteMask(string address, ushort andMask, ushort orMask)
    {
      OperateResult<byte[]> operateResult1 = ModbusInfo.BuildWriteMaskModbusCommand(address, andMask, orMask, this.Station, this.AddressStartWithZero, (byte) 22);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(ModbusInfo.PackCommandToTcp(operateResult1.Content, (ushort) this.softIncrementCount.GetCurrentValue()));
      return !operateResult2.IsSuccess ? (OperateResult) operateResult2 : (OperateResult) ModbusInfo.ExtractActualData(ModbusInfo.ExplodeTcpCommandToCore(operateResult2.Content));
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.WriteOneRegister(System.String,System.Int16)" />
    public OperateResult WriteOneRegister(string address, short value) => this.Write(address, value);

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.WriteOneRegister(System.String,System.UInt16)" />
    public OperateResult WriteOneRegister(string address, ushort value) => this.Write(address, value);

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.ReadCoilAsync(System.String)" />
    public async Task<OperateResult<bool>> ReadCoilAsync(string address)
    {
      OperateResult<bool> operateResult = await Task.Run<OperateResult<bool>>((Func<OperateResult<bool>>) (() => this.ReadCoil(address)));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.ReadCoilAsync(System.String,System.UInt16)" />
    public async Task<OperateResult<bool[]>> ReadCoilAsync(
      string address,
      ushort length)
    {
      OperateResult<bool[]> operateResult = await Task.Run<OperateResult<bool[]>>((Func<OperateResult<bool[]>>) (() => this.ReadCoil(address, length)));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.ReadDiscreteAsync(System.String)" />
    public async Task<OperateResult<bool>> ReadDiscreteAsync(string address)
    {
      OperateResult<bool> operateResult = await Task.Run<OperateResult<bool>>((Func<OperateResult<bool>>) (() => this.ReadDiscrete(address)));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.ReadDiscreteAsync(System.String,System.UInt16)" />
    public async Task<OperateResult<bool[]>> ReadDiscreteAsync(
      string address,
      ushort length)
    {
      OperateResult<bool[]> operateResult = await Task.Run<OperateResult<bool[]>>((Func<OperateResult<bool[]>>) (() => this.ReadDiscrete(address, length)));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusUdpNet.Write(System.String,System.Int16)" />
    /// /param&gt;
    public override async Task<OperateResult> WriteAsync(
      string address,
      short value)
    {
      OperateResult operateResult = await Task.Run<OperateResult>((Func<OperateResult>) (() => this.Write(address, value)));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusUdpNet.Write(System.String,System.UInt16)" />
    /// /param&gt;
    public override async Task<OperateResult> WriteAsync(
      string address,
      ushort value)
    {
      OperateResult operateResult = await Task.Run<OperateResult>((Func<OperateResult>) (() => this.Write(address, value)));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.WriteOneRegister(System.String,System.Int16)" />
    public async Task<OperateResult> WriteOneRegisterAsync(
      string address,
      short value)
    {
      OperateResult operateResult = await Task.Run<OperateResult>((Func<OperateResult>) (() => this.WriteOneRegister(address, value)));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.WriteOneRegister(System.String,System.UInt16)" />
    public async Task<OperateResult> WriteOneRegisterAsync(
      string address,
      ushort value)
    {
      OperateResult operateResult = await Task.Run<OperateResult>((Func<OperateResult>) (() => this.WriteOneRegister(address, value)));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.WriteMask(System.String,System.UInt16,System.UInt16)" />
    public async Task<OperateResult> WriteMaskAsync(
      string address,
      ushort andMask,
      ushort orMask)
    {
      OperateResult operateResult = await Task.Run<OperateResult>((Func<OperateResult>) (() => this.WriteMask(address, andMask, orMask)));
      return operateResult;
    }

    private OperateResult<bool[]> ReadBoolHelper(
      string address,
      ushort length,
      byte function)
    {
      OperateResult<byte[][]> operateResult1 = ModbusInfo.BuildReadModbusCommand(address, length, this.Station, this.AddressStartWithZero, function);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult1);
      List<bool> boolList = new List<bool>();
      for (int index = 0; index < operateResult1.Content.Length; ++index)
      {
        OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(ModbusInfo.PackCommandToTcp(operateResult1.Content[index], (ushort) this.softIncrementCount.GetCurrentValue()));
        if (!operateResult2.IsSuccess)
          return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult2);
        OperateResult<byte[]> actualData = ModbusInfo.ExtractActualData(ModbusInfo.ExplodeTcpCommandToCore(operateResult2.Content));
        if (!actualData.IsSuccess)
          return OperateResult.CreateFailedResult<bool[]>((OperateResult) actualData);
        int length1 = (int) operateResult1.Content[index][4] * 256 + (int) operateResult1.Content[index][5];
        boolList.AddRange((IEnumerable<bool>) SoftBasic.ByteToBoolArray(actualData.Content, length1));
      }
      return OperateResult.CreateSuccessResult<bool[]>(boolList.ToArray());
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.ReadBool(System.String,System.UInt16)" />
    [EstMqttApi("ReadBoolArray", "")]
    public override OperateResult<bool[]> ReadBool(string address, ushort length) => this.ReadBoolHelper(address, length, (byte) 1);

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.Write(System.String,System.Boolean[])" />
    [EstMqttApi("WriteBoolArray", "")]
    public override OperateResult Write(string address, bool[] values)
    {
      OperateResult<byte[]> operateResult1 = ModbusInfo.BuildWriteBoolModbusCommand(address, values, this.Station, this.AddressStartWithZero, (byte) 15);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(ModbusInfo.PackCommandToTcp(operateResult1.Content, (ushort) this.softIncrementCount.GetCurrentValue()));
      return !operateResult2.IsSuccess ? (OperateResult) operateResult2 : (OperateResult) ModbusInfo.ExtractActualData(ModbusInfo.ExplodeTcpCommandToCore(operateResult2.Content));
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.Write(System.String,System.Boolean)" />
    [EstMqttApi("WriteBool", "")]
    public override OperateResult Write(string address, bool value)
    {
      OperateResult<byte[]> operateResult1 = ModbusInfo.BuildWriteBoolModbusCommand(address, value, this.Station, this.AddressStartWithZero, (byte) 5);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(ModbusInfo.PackCommandToTcp(operateResult1.Content, (ushort) this.softIncrementCount.GetCurrentValue()));
      return !operateResult2.IsSuccess ? (OperateResult) operateResult2 : (OperateResult) ModbusInfo.ExtractActualData(ModbusInfo.ExplodeTcpCommandToCore(operateResult2.Content));
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.WriteAsync(System.String,System.Boolean)" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      bool value)
    {
      OperateResult operateResult = await Task.Run<OperateResult>((Func<OperateResult>) (() => this.Write(address, value)));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadInt32(System.String,System.UInt16)" />
    [EstMqttApi("ReadInt32Array", "")]
    public override OperateResult<int[]> ReadInt32(string address, ushort length)
    {
      IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      return ByteTransformHelper.GetResultFromBytes<int[]>(this.Read(address, (ushort) ((int) length * (int) this.WordLength * 2)), (Func<byte[], int[]>) (m => transform.TransInt32(m, 0, (int) length)));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadUInt32(System.String,System.UInt16)" />
    [EstMqttApi("ReadUInt32Array", "")]
    public override OperateResult<uint[]> ReadUInt32(string address, ushort length)
    {
      IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      return ByteTransformHelper.GetResultFromBytes<uint[]>(this.Read(address, (ushort) ((int) length * (int) this.WordLength * 2)), (Func<byte[], uint[]>) (m => transform.TransUInt32(m, 0, (int) length)));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadFloat(System.String,System.UInt16)" />
    [EstMqttApi("ReadFloatArray", "")]
    public override OperateResult<float[]> ReadFloat(string address, ushort length)
    {
      IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      return ByteTransformHelper.GetResultFromBytes<float[]>(this.Read(address, (ushort) ((int) length * (int) this.WordLength * 2)), (Func<byte[], float[]>) (m => transform.TransSingle(m, 0, (int) length)));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadInt64(System.String,System.UInt16)" />
    [EstMqttApi("ReadInt64Array", "")]
    public override OperateResult<long[]> ReadInt64(string address, ushort length)
    {
      IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      return ByteTransformHelper.GetResultFromBytes<long[]>(this.Read(address, (ushort) ((int) length * (int) this.WordLength * 4)), (Func<byte[], long[]>) (m => transform.TransInt64(m, 0, (int) length)));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadUInt64(System.String,System.UInt16)" />
    [EstMqttApi("ReadUInt64Array", "")]
    public override OperateResult<ulong[]> ReadUInt64(string address, ushort length)
    {
      IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      return ByteTransformHelper.GetResultFromBytes<ulong[]>(this.Read(address, (ushort) ((int) length * (int) this.WordLength * 4)), (Func<byte[], ulong[]>) (m => transform.TransUInt64(m, 0, (int) length)));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadDouble(System.String,System.UInt16)" />
    [EstMqttApi("ReadDoubleArray", "")]
    public override OperateResult<double[]> ReadDouble(string address, ushort length)
    {
      IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      return ByteTransformHelper.GetResultFromBytes<double[]>(this.Read(address, (ushort) ((int) length * (int) this.WordLength * 4)), (Func<byte[], double[]>) (m => transform.TransDouble(m, 0, (int) length)));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.Int32[])" />
    [EstMqttApi("WriteInt32Array", "")]
    public override OperateResult Write(string address, int[] values)
    {
      IByteTransform transformParameter = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      return this.Write(address, transformParameter.TransByte(values));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.UInt32[])" />
    [EstMqttApi("WriteUInt32Array", "")]
    public override OperateResult Write(string address, uint[] values)
    {
      IByteTransform transformParameter = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      return this.Write(address, transformParameter.TransByte(values));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.Single[])" />
    [EstMqttApi("WriteFloatArray", "")]
    public override OperateResult Write(string address, float[] values)
    {
      IByteTransform transformParameter = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      return this.Write(address, transformParameter.TransByte(values));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.Int64[])" />
    [EstMqttApi("WriteInt64Array", "")]
    public override OperateResult Write(string address, long[] values)
    {
      IByteTransform transformParameter = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      return this.Write(address, transformParameter.TransByte(values));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.UInt64[])" />
    [EstMqttApi("WriteUInt64Array", "")]
    public override OperateResult Write(string address, ulong[] values)
    {
      IByteTransform transformParameter = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      return this.Write(address, transformParameter.TransByte(values));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.Double[])" />
    [EstMqttApi("WriteDoubleArray", "")]
    public override OperateResult Write(string address, double[] values)
    {
      IByteTransform transformParameter = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      return this.Write(address, transformParameter.TransByte(values));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadInt32Async(System.String,System.UInt16)" />
    public override async Task<OperateResult<int[]>> ReadInt32Async(
      string address,
      ushort length)
    {
      IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      OperateResult<byte[]> result = await this.ReadAsync(address, (ushort) ((int) length * (int) this.WordLength * 2));
      return ByteTransformHelper.GetResultFromBytes<int[]>(result, (Func<byte[], int[]>) (m => transform.TransInt32(m, 0, (int) length)));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadUInt32Async(System.String,System.UInt16)" />
    public override async Task<OperateResult<uint[]>> ReadUInt32Async(
      string address,
      ushort length)
    {
      IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      OperateResult<byte[]> result = await this.ReadAsync(address, (ushort) ((int) length * (int) this.WordLength * 2));
      return ByteTransformHelper.GetResultFromBytes<uint[]>(result, (Func<byte[], uint[]>) (m => transform.TransUInt32(m, 0, (int) length)));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadFloatAsync(System.String,System.UInt16)" />
    public override async Task<OperateResult<float[]>> ReadFloatAsync(
      string address,
      ushort length)
    {
      IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      OperateResult<byte[]> result = await this.ReadAsync(address, (ushort) ((int) length * (int) this.WordLength * 2));
      return ByteTransformHelper.GetResultFromBytes<float[]>(result, (Func<byte[], float[]>) (m => transform.TransSingle(m, 0, (int) length)));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadInt64Async(System.String,System.UInt16)" />
    public override async Task<OperateResult<long[]>> ReadInt64Async(
      string address,
      ushort length)
    {
      IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      OperateResult<byte[]> result = await this.ReadAsync(address, (ushort) ((int) length * (int) this.WordLength * 4));
      return ByteTransformHelper.GetResultFromBytes<long[]>(result, (Func<byte[], long[]>) (m => transform.TransInt64(m, 0, (int) length)));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadUInt64Async(System.String,System.UInt16)" />
    public override async Task<OperateResult<ulong[]>> ReadUInt64Async(
      string address,
      ushort length)
    {
      IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      OperateResult<byte[]> result = await this.ReadAsync(address, (ushort) ((int) length * (int) this.WordLength * 4));
      return ByteTransformHelper.GetResultFromBytes<ulong[]>(result, (Func<byte[], ulong[]>) (m => transform.TransUInt64(m, 0, (int) length)));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadDoubleAsync(System.String,System.UInt16)" />
    public override async Task<OperateResult<double[]>> ReadDoubleAsync(
      string address,
      ushort length)
    {
      IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      OperateResult<byte[]> result = await this.ReadAsync(address, (ushort) ((int) length * (int) this.WordLength * 4));
      return ByteTransformHelper.GetResultFromBytes<double[]>(result, (Func<byte[], double[]>) (m => transform.TransDouble(m, 0, (int) length)));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteAsync(System.String,System.Int32[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      int[] values)
    {
      IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      OperateResult operateResult = await this.WriteAsync(address, transform.TransByte(values));
      transform = (IByteTransform) null;
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteAsync(System.String,System.UInt32[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      uint[] values)
    {
      IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      OperateResult operateResult = await this.WriteAsync(address, transform.TransByte(values));
      transform = (IByteTransform) null;
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteAsync(System.String,System.Single[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      float[] values)
    {
      IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      OperateResult operateResult = await this.WriteAsync(address, transform.TransByte(values));
      transform = (IByteTransform) null;
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteAsync(System.String,System.Int64[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      long[] values)
    {
      IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      OperateResult operateResult = await this.WriteAsync(address, transform.TransByte(values));
      transform = (IByteTransform) null;
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteAsync(System.String,System.UInt64[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      ulong[] values)
    {
      IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      OperateResult operateResult = await this.WriteAsync(address, transform.TransByte(values));
      transform = (IByteTransform) null;
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteAsync(System.String,System.Double[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      double[] values)
    {
      IByteTransform transform = EstHelper.ExtractTransformParameter(ref address, this.ByteTransform);
      OperateResult operateResult = await this.WriteAsync(address, transform.TransByte(values));
      transform = (IByteTransform) null;
      return operateResult;
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("ModbusUdpNet[{0}:{1}]", (object) this.IpAddress, (object) this.Port);
  }
}
