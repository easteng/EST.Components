// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.Inovance.InovanceH5USerial
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.ModBus;
using EstCommunication.Reflection;

namespace EstCommunication.Profinet.Inovance
{
  /// <summary>
  /// 汇川的串口通信协议，适用于H5U 系列，底层走的是MODBUS-RTU协议，地址说明参见标记<br />
  /// Huichuan's serial communication protocol is suitable for H5U series. The bottom layer is MODBUS-RTU protocol.
  /// For the address description, please refer to the mark
  /// </summary>
  /// <remarks>
  /// <inheritdoc cref="T:EstCommunication.Profinet.Inovance.InovanceH5UTcp" path="remarks" />
  /// </remarks>
  public class InovanceH5USerial : ModbusRtu
  {
    /// <summary>
    /// 实例化一个汇川串口协议的客户端对象<br />
    /// Instantiate a client object of Huichuan serial protocol
    /// </summary>
    public InovanceH5USerial()
    {
    }

    /// <summary>
    /// 指定客户端自己的站号来初始化<br />
    /// Specify the client's own station number to initialize
    /// </summary>
    /// <param name="station">客户端自身的站号</param>
    public InovanceH5USerial(byte station = 1)
      : base(station)
    {
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Inovance.InovanceH5UTcp.Read(System.String,System.UInt16)" />
    [EstMqttApi("ReadByteArray", "")]
    public override OperateResult<byte[]> Read(string address, ushort length)
    {
      OperateResult<string> operateResult = InovanceHelper.PraseInovanceH5UAddress(address, (byte) 3);
      return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult) : base.Read(operateResult.Content, length);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Inovance.InovanceH5UTcp.Write(System.String,System.Byte[])" />
    [EstMqttApi("WriteByteArray", "")]
    public override OperateResult Write(string address, byte[] value)
    {
      OperateResult<string> operateResult = InovanceHelper.PraseInovanceH5UAddress(address, (byte) 16);
      return !operateResult.IsSuccess ? (OperateResult) OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult) : base.Write(operateResult.Content, value);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Inovance.InovanceH5UTcp.ReadBool(System.String,System.UInt16)" />
    [EstMqttApi("ReadBoolArray", "")]
    public override OperateResult<bool[]> ReadBool(string address, ushort length)
    {
      OperateResult<string> operateResult = InovanceHelper.PraseInovanceH5UAddress(address, (byte) 1);
      return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult) : base.ReadBool(operateResult.Content, length);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Inovance.InovanceH5UTcp.Write(System.String,System.Boolean[])" />
    [EstMqttApi("WriteBoolArray", "")]
    public override OperateResult Write(string address, bool[] values)
    {
      OperateResult<string> operateResult = InovanceHelper.PraseInovanceH5UAddress(address, (byte) 15);
      return !operateResult.IsSuccess ? (OperateResult) OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult) : base.Write(operateResult.Content, values);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Inovance.InovanceH5UTcp.Write(System.String,System.Boolean)" />
    [EstMqttApi("WriteBool", "")]
    public override OperateResult Write(string address, bool value)
    {
      OperateResult<string> operateResult = InovanceHelper.PraseInovanceH5UAddress(address, (byte) 5);
      return !operateResult.IsSuccess ? (OperateResult) OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult) : base.Write(operateResult.Content, value);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Inovance.InovanceH5UTcp.Write(System.String,System.Int16)" />
    [EstMqttApi("WriteInt16", "")]
    public override OperateResult Write(string address, short value)
    {
      OperateResult<string> operateResult = InovanceHelper.PraseInovanceH5UAddress(address, (byte) 6);
      return !operateResult.IsSuccess ? (OperateResult) OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult) : base.Write(operateResult.Content, value);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Inovance.InovanceH5UTcp.Write(System.String,System.UInt16)" />
    [EstMqttApi("WriteUInt16", "")]
    public override OperateResult Write(string address, ushort value)
    {
      OperateResult<string> operateResult = InovanceHelper.PraseInovanceH5UAddress(address, (byte) 6);
      return !operateResult.IsSuccess ? (OperateResult) OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult) : base.Write(operateResult.Content, value);
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("InovanceH5USerial[{0}:{1}]", (object) this.PortName, (object) this.BaudRate);
  }
}
