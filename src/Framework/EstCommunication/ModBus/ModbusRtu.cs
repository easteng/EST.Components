// Decompiled with JetBrains decompiler
// Type: EstCommunication.ModBus.ModbusRtu
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using EstCommunication.Core;
using EstCommunication.Reflection;
using EstCommunication.Serial;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EstCommunication.ModBus
{
  /// <summary>
  /// Modbus-Rtu通讯协议的类库，多项式码0xA001，支持标准的功能码，也支持扩展的功能码实现，地址采用富文本的形式，详细见备注说明<br />
  /// Modbus-Rtu communication protocol class library, polynomial code 0xA001, supports standard function codes,
  /// and also supports extended function code implementation. The address is in rich text. For details, see the remark
  /// </summary>
  /// <remarks>
  /// 本客户端支持的标准的modbus协议，Modbus-Tcp及Modbus-Udp内置的消息号会进行自增，地址支持富文本格式，具体参考示例代码。<br />
  /// 读取线圈，输入线圈，寄存器，输入寄存器的方法中的读取长度对商业授权用户不限制，内部自动切割读取，结果合并。
  /// </remarks>
  /// <example>
  /// <inheritdoc cref="T:EstCommunication.ModBus.ModbusTcpNet" path="example" />
  /// </example>
  public class ModbusRtu : SerialDeviceBase
  {
    private byte station = 1;
    private bool isAddressStartWithZero = true;

    /// <summary>
    /// 实例化一个Modbus-Rtu协议的客户端对象<br />
    /// Instantiate a client object of the Modbus-Rtu protocol
    /// </summary>
    public ModbusRtu() => this.ByteTransform = (IByteTransform) new ReverseWordTransform();

    /// <summary>
    /// 指定客户端自己的站号来初始化<br />
    /// Specify the client's own station number to initialize
    /// </summary>
    /// <param name="station">客户端自身的站号</param>
    public ModbusRtu(byte station = 1)
    {
      this.station = station;
      this.ByteTransform = (IByteTransform) new ReverseWordTransform();
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

    /// <summary>
    /// 检查当前的Modbus-Rtu响应是否是正确的<br />
    /// Check if the current Modbus-Rtu response is correct
    /// </summary>
    /// <param name="send">发送的数据信息</param>
    /// <returns>带是否成功的结果数据</returns>
    protected virtual OperateResult<byte[]> CheckModbusTcpResponse(byte[] send)
    {
      send = ModbusInfo.PackCommandToRtu(send);
      OperateResult<byte[]> operateResult = this.ReadBase(send);
      if (!operateResult.IsSuccess)
        return operateResult;
      if (operateResult.Content.Length < 5)
        return new OperateResult<byte[]>(StringResources.Language.ReceiveDataLengthTooShort + "5");
      if (!SoftCRC16.CheckCRC16(operateResult.Content))
        return new OperateResult<byte[]>(StringResources.Language.ModbusCRCCheckFailed + SoftBasic.ByteToHexString(operateResult.Content, ' '));
      if ((int) send[1] + 128 == (int) operateResult.Content[1])
        return new OperateResult<byte[]>((int) operateResult.Content[2], ModbusInfo.GetDescriptionByErrorCode(operateResult.Content[2]));
      return (int) send[1] != (int) operateResult.Content[1] ? new OperateResult<byte[]>((int) operateResult.Content[1], "Receive Command Check Failed: ") : ModbusInfo.ExtractActualData(ModbusInfo.ExplodeRtuCommandToCore(operateResult.Content));
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.ReadCoil(System.String)" />
    public OperateResult<bool> ReadCoil(string address) => this.ReadBool(address);

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.ReadCoil(System.String,System.UInt16)" />
    public OperateResult<bool[]> ReadCoil(string address, ushort length) => this.ReadBool(address, length);

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.ReadDiscrete(System.String)" />
    public OperateResult<bool> ReadDiscrete(string address) => ByteTransformHelper.GetResultFromArray<bool>(this.ReadDiscrete(address, (ushort) 1));

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.ReadDiscrete(System.String,System.UInt16)" />
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
        OperateResult<byte[]> operateResult2 = this.CheckModbusTcpResponse(operateResult1.Content[index]);
        if (!operateResult2.IsSuccess)
          return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult2);
        byteList.AddRange((IEnumerable<byte>) operateResult2.Content);
      }
      return OperateResult.CreateSuccessResult<byte[]>(byteList.ToArray());
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.Write(System.String,System.Byte[])" />
    [EstMqttApi("WriteByteArray", "")]
    public override OperateResult Write(string address, byte[] value)
    {
      OperateResult<byte[]> operateResult = ModbusInfo.BuildWriteWordModbusCommand(address, value, this.Station, this.AddressStartWithZero, (byte) 16);
      return !operateResult.IsSuccess ? (OperateResult) operateResult : (OperateResult) this.CheckModbusTcpResponse(operateResult.Content);
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.Write(System.String,System.Int16)" />
    [EstMqttApi("WriteInt16", "")]
    public override OperateResult Write(string address, short value)
    {
      OperateResult<byte[]> operateResult = ModbusInfo.BuildWriteWordModbusCommand(address, value, this.Station, this.AddressStartWithZero, (byte) 6);
      return !operateResult.IsSuccess ? (OperateResult) operateResult : (OperateResult) this.CheckModbusTcpResponse(operateResult.Content);
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.Write(System.String,System.UInt16)" />
    [EstMqttApi("WriteUInt16", "")]
    public override OperateResult Write(string address, ushort value)
    {
      OperateResult<byte[]> operateResult = ModbusInfo.BuildWriteWordModbusCommand(address, value, this.Station, this.AddressStartWithZero, (byte) 6);
      return !operateResult.IsSuccess ? (OperateResult) operateResult : (OperateResult) this.CheckModbusTcpResponse(operateResult.Content);
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.WriteMask(System.String,System.UInt16,System.UInt16)" />
    [EstMqttApi("WriteMask", "")]
    public OperateResult WriteMask(string address, ushort andMask, ushort orMask)
    {
      OperateResult<byte[]> operateResult = ModbusInfo.BuildWriteMaskModbusCommand(address, andMask, orMask, this.Station, this.AddressStartWithZero, (byte) 22);
      return !operateResult.IsSuccess ? (OperateResult) operateResult : (OperateResult) this.CheckModbusTcpResponse(operateResult.Content);
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusRtu.Write(System.String,System.Int16)" />
    public OperateResult WriteOneRegister(string address, short value) => this.Write(address, value);

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusRtu.Write(System.String,System.UInt16)" />
    public OperateResult WriteOneRegister(string address, ushort value) => this.Write(address, value);

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusRtu.Write(System.String,System.Int16)" />
    /// /param&gt;
    public override async Task<OperateResult> WriteAsync(
      string address,
      short value)
    {
      OperateResult operateResult = await Task.Run<OperateResult>((Func<OperateResult>) (() => this.Write(address, value)));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusRtu.Write(System.String,System.UInt16)" />
    /// /param&gt;
    public override async Task<OperateResult> WriteAsync(
      string address,
      ushort value)
    {
      OperateResult operateResult = await Task.Run<OperateResult>((Func<OperateResult>) (() => this.Write(address, value)));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusRtu.ReadCoil(System.String)" />
    public async Task<OperateResult<bool>> ReadCoilAsync(string address)
    {
      OperateResult<bool> operateResult = await Task.Run<OperateResult<bool>>((Func<OperateResult<bool>>) (() => this.ReadCoil(address)));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusRtu.ReadCoil(System.String,System.UInt16)" />
    public async Task<OperateResult<bool[]>> ReadCoilAsync(
      string address,
      ushort length)
    {
      OperateResult<bool[]> operateResult = await Task.Run<OperateResult<bool[]>>((Func<OperateResult<bool[]>>) (() => this.ReadCoil(address, length)));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusRtu.ReadDiscrete(System.String)" />
    public async Task<OperateResult<bool>> ReadDiscreteAsync(string address)
    {
      OperateResult<bool> operateResult = await Task.Run<OperateResult<bool>>((Func<OperateResult<bool>>) (() => this.ReadDiscrete(address)));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusRtu.ReadDiscrete(System.String,System.UInt16)" />
    public async Task<OperateResult<bool[]>> ReadDiscreteAsync(
      string address,
      ushort length)
    {
      OperateResult<bool[]> operateResult = await Task.Run<OperateResult<bool[]>>((Func<OperateResult<bool[]>>) (() => this.ReadDiscrete(address, length)));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusRtu.WriteOneRegister(System.String,System.Int16)" />
    public async Task<OperateResult> WriteOneRegisterAsync(
      string address,
      short value)
    {
      OperateResult operateResult = await Task.Run<OperateResult>((Func<OperateResult>) (() => this.WriteOneRegister(address, value)));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusRtu.WriteOneRegister(System.String,System.UInt16)" />
    public async Task<OperateResult> WriteOneRegisterAsync(
      string address,
      ushort value)
    {
      OperateResult operateResult = await Task.Run<OperateResult>((Func<OperateResult>) (() => this.WriteOneRegister(address, value)));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusRtu.WriteMask(System.String,System.UInt16,System.UInt16)" />
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
        OperateResult<byte[]> operateResult2 = this.CheckModbusTcpResponse(operateResult1.Content[index]);
        if (!operateResult2.IsSuccess)
          return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult2);
        int length1 = (int) operateResult1.Content[index][4] * 256 + (int) operateResult1.Content[index][5];
        boolList.AddRange((IEnumerable<bool>) SoftBasic.ByteToBoolArray(operateResult2.Content, length1));
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
      OperateResult<byte[]> operateResult = ModbusInfo.BuildWriteBoolModbusCommand(address, values, this.Station, this.AddressStartWithZero, (byte) 15);
      return !operateResult.IsSuccess ? (OperateResult) operateResult : (OperateResult) this.CheckModbusTcpResponse(operateResult.Content);
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusTcpNet.Write(System.String,System.Boolean)" />
    [EstMqttApi("WriteBool", "")]
    public override OperateResult Write(string address, bool value)
    {
      OperateResult<byte[]> operateResult = ModbusInfo.BuildWriteBoolModbusCommand(address, value, this.Station, this.AddressStartWithZero, (byte) 5);
      return !operateResult.IsSuccess ? (OperateResult) operateResult : (OperateResult) this.CheckModbusTcpResponse(operateResult.Content);
    }

    /// <inheritdoc cref="M:EstCommunication.ModBus.ModbusRtu.Write(System.String,System.Boolean)" />
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
    public override string ToString() => string.Format("ModbusRtu[{0}:{1}]", (object) this.PortName, (object) this.BaudRate);
  }
}
