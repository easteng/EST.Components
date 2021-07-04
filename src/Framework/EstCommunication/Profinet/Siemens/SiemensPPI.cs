// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.Siemens.SiemensPPI
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using EstCommunication.Core;
using EstCommunication.Reflection;
using EstCommunication.Serial;
using System;

namespace EstCommunication.Profinet.Siemens
{
  /// <summary>
  /// 西门子的PPI协议，适用于s7-200plc，注意，由于本类库的每次通讯分成2次操作，内部增加了一个同步锁，所以单次通信时间比较久，另外，地址支持携带站号，例如：s=2;M100<br />
  /// Siemens' PPI protocol is suitable for s7-200plc. Note that since each communication of this class library is divided into two operations,
  /// and a synchronization lock is added inside, the single communication time is relatively long. In addition,
  /// the address supports carrying the station number, for example : S=2;M100
  /// </summary>
  /// <remarks>
  /// 适用于西门子200的通信，非常感谢 合肥-加劲 的测试，让本类库圆满完成。注意：M地址范围有限 0-31地址<br />
  /// 在本类的<see cref="T:EstCommunication.Profinet.Siemens.SiemensPPIOverTcp" />实现类里，如果使用了Async的异步方法，没有增加同步锁，多线程调用可能会引发数据错乱的情况。<br />
  /// In the <see cref="T:EstCommunication.Profinet.Siemens.SiemensPPIOverTcp" /> implementation class of this class, if the asynchronous method of Async is used,
  /// the synchronization lock is not added, and multi-threaded calls may cause data disorder.
  /// </remarks>
  public class SiemensPPI : SerialDeviceBase
  {
    private byte station = 2;
    private object communicationLock;

    /// <summary>
    /// 实例化一个西门子的PPI协议对象<br />
    /// Instantiate a Siemens PPI protocol object
    /// </summary>
    public SiemensPPI()
    {
      this.ByteTransform = (IByteTransform) new ReverseBytesTransform();
      this.WordLength = (ushort) 2;
      this.communicationLock = new object();
    }

    /// <summary>
    /// 西门子PLC的站号信息<br />
    /// Siemens PLC station number information
    /// </summary>
    public byte Station
    {
      get => this.station;
      set => this.station = value;
    }

    /// <summary>
    /// 从西门子的PLC中读取数据信息，地址为"M100","AI100","I0","Q0","V100","S100"等<br />
    /// Read data information from Siemens PLC with addresses "M100", "AI100", "I0", "Q0", "V100", "S100", etc.
    /// </summary>
    /// <param name="address">西门子的地址数据信息</param>
    /// <param name="length">数据长度</param>
    /// <returns>带返回结果的结果对象</returns>
    [EstMqttApi("ReadByteArray", "")]
    public override OperateResult<byte[]> Read(string address, ushort length)
    {
      byte parameter = (byte) EstHelper.ExtractParameter(ref address, "s", (int) this.Station);
      OperateResult<byte[]> operateResult1 = SiemensPPIOverTcp.BuildReadCommand(parameter, address, length, false);
      if (!operateResult1.IsSuccess)
        return operateResult1;
      lock (this.communicationLock)
      {
        OperateResult<byte[]> operateResult2 = this.ReadBase(operateResult1.Content);
        if (!operateResult2.IsSuccess)
          return operateResult2;
        if (operateResult2.Content[0] != (byte) 229)
          return new OperateResult<byte[]>("PLC Receive Check Failed:" + SoftBasic.ByteToHexString(operateResult2.Content, ' '));
        OperateResult<byte[]> operateResult3 = this.ReadBase(SiemensPPIOverTcp.GetExecuteConfirm(parameter));
        if (!operateResult3.IsSuccess)
          return operateResult3;
        OperateResult result = SiemensPPIOverTcp.CheckResponse(operateResult3.Content);
        if (!result.IsSuccess)
          return OperateResult.CreateFailedResult<byte[]>(result);
        byte[] numArray = new byte[(int) length];
        if (operateResult3.Content[21] == byte.MaxValue && operateResult3.Content[22] == (byte) 4)
          Array.Copy((Array) operateResult3.Content, 25, (Array) numArray, 0, (int) length);
        return OperateResult.CreateSuccessResult<byte[]>(numArray);
      }
    }

    /// <summary>
    /// 从西门子的PLC中读取bool数据信息，地址为"M100.0","AI100.1","I0.3","Q0.6","V100.4","S100"等<br />
    /// Read bool data information from Siemens PLC, the addresses are "M100.0", "AI100.1", "I0.3", "Q0.6", "V100.4", "S100", etc.
    /// </summary>
    /// <param name="address">西门子的地址数据信息</param>
    /// <param name="length">数据长度</param>
    /// <returns>带返回结果的结果对象</returns>
    [EstMqttApi("ReadBoolArray", "")]
    public override OperateResult<bool[]> ReadBool(string address, ushort length)
    {
      byte parameter = (byte) EstHelper.ExtractParameter(ref address, "s", (int) this.Station);
      OperateResult<byte[]> operateResult1 = SiemensPPIOverTcp.BuildReadCommand(parameter, address, length, true);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult1);
      lock (this.communicationLock)
      {
        OperateResult<byte[]> operateResult2 = this.ReadBase(operateResult1.Content);
        if (!operateResult2.IsSuccess)
          return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult2);
        if (operateResult2.Content[0] != (byte) 229)
          return new OperateResult<bool[]>("PLC Receive Check Failed:" + SoftBasic.ByteToHexString(operateResult2.Content, ' '));
        OperateResult<byte[]> operateResult3 = this.ReadBase(SiemensPPIOverTcp.GetExecuteConfirm(parameter));
        if (!operateResult3.IsSuccess)
          return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult3);
        OperateResult result = SiemensPPIOverTcp.CheckResponse(operateResult3.Content);
        if (!result.IsSuccess)
          return OperateResult.CreateFailedResult<bool[]>(result);
        byte[] InBytes = new byte[operateResult3.Content.Length - 27];
        if (operateResult3.Content[21] == byte.MaxValue && operateResult3.Content[22] == (byte) 3)
          Array.Copy((Array) operateResult3.Content, 25, (Array) InBytes, 0, InBytes.Length);
        return OperateResult.CreateSuccessResult<bool[]>(SoftBasic.ByteToBoolArray(InBytes, (int) length));
      }
    }

    /// <summary>
    /// 将字节数据写入到西门子PLC中，地址为"M100.0","AI100.1","I0.3","Q0.6","V100.4","S100"等<br />
    /// Write byte data to Siemens PLC with addresses "M100.0", "AI100.1", "I0.3", "Q0.6", "V100.4", "S100", etc.
    /// </summary>
    /// <param name="address">西门子的地址数据信息</param>
    /// <param name="value">数据长度</param>
    /// <returns>带返回结果的结果对象</returns>
    [EstMqttApi("WriteByteArray", "")]
    public override OperateResult Write(string address, byte[] value)
    {
      byte parameter = (byte) EstHelper.ExtractParameter(ref address, "s", (int) this.Station);
      OperateResult<byte[]> operateResult1 = SiemensPPIOverTcp.BuildWriteCommand(parameter, address, value);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      lock (this.communicationLock)
      {
        OperateResult<byte[]> operateResult2 = this.ReadBase(operateResult1.Content);
        if (!operateResult2.IsSuccess)
          return (OperateResult) operateResult2;
        if (operateResult2.Content[0] != (byte) 229)
          return (OperateResult) new OperateResult<byte[]>("PLC Receive Check Failed:" + operateResult2.Content[0].ToString());
        OperateResult<byte[]> operateResult3 = this.ReadBase(SiemensPPIOverTcp.GetExecuteConfirm(parameter));
        if (!operateResult3.IsSuccess)
          return (OperateResult) operateResult3;
        OperateResult operateResult4 = SiemensPPIOverTcp.CheckResponse(operateResult3.Content);
        return !operateResult4.IsSuccess ? operateResult4 : OperateResult.CreateSuccessResult();
      }
    }

    /// <summary>
    /// 将bool数据写入到西门子PLC中，地址为"M100.0","AI100.1","I0.3","Q0.6","V100.4","S100"等<br />
    /// Write the bool data to Siemens PLC with the addresses "M100.0", "AI100.1", "I0.3", "Q0.6", "V100.4", "S100", etc.
    /// </summary>
    /// <param name="address">西门子的地址数据信息</param>
    /// <param name="value">数据长度</param>
    /// <returns>带返回结果的结果对象</returns>
    [EstMqttApi("WriteBoolArray", "")]
    public override OperateResult Write(string address, bool[] value)
    {
      byte parameter = (byte) EstHelper.ExtractParameter(ref address, "s", (int) this.Station);
      OperateResult<byte[]> operateResult1 = SiemensPPIOverTcp.BuildWriteCommand(parameter, address, value);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      lock (this.communicationLock)
      {
        OperateResult<byte[]> operateResult2 = this.ReadBase(operateResult1.Content);
        if (!operateResult2.IsSuccess)
          return (OperateResult) operateResult2;
        if (operateResult2.Content[0] != (byte) 229)
          return (OperateResult) new OperateResult<byte[]>("PLC Receive Check Failed:" + operateResult2.Content[0].ToString());
        OperateResult<byte[]> operateResult3 = this.ReadBase(SiemensPPIOverTcp.GetExecuteConfirm(parameter));
        if (!operateResult3.IsSuccess)
          return (OperateResult) operateResult3;
        OperateResult operateResult4 = SiemensPPIOverTcp.CheckResponse(operateResult3.Content);
        return !operateResult4.IsSuccess ? operateResult4 : OperateResult.CreateSuccessResult();
      }
    }

    /// <summary>
    /// 从西门子的PLC中读取byte数据信息，地址为"M100","AI100","I0","Q0","V100","S100"等，详细请参照API文档<br />
    /// Read byte data information from Siemens PLC. The addresses are "M100", "AI100", "I0", "Q0", "V100", "S100", etc. Please refer to the API documentation for details.
    /// </summary>
    /// <param name="address">西门子的地址数据信息</param>
    /// <returns>带返回结果的结果对象</returns>
    [EstMqttApi("ReadByte", "")]
    public OperateResult<byte> ReadByte(string address) => ByteTransformHelper.GetResultFromArray<byte>(this.Read(address, (ushort) 1));

    /// <summary>
    /// 向西门子的PLC中读取byte数据，地址为"M100","AI100","I0","Q0","V100","S100"等，详细请参照API文档<br />
    /// Read byte data from Siemens PLC with addresses "M100", "AI100", "I0", "Q0", "V100", "S100", etc. For details, please refer to the API documentation
    /// </summary>
    /// <param name="address">西门子的地址数据信息</param>
    /// <param name="value">数据长度</param>
    /// <returns>带返回结果的结果对象</returns>
    [EstMqttApi("Write", "")]
    public OperateResult Write(string address, byte value) => this.Write(address, new byte[1]
    {
      value
    });

    /// <summary>
    /// 启动西门子PLC为RUN模式，参数信息可以携带站号信息 "s=2;", 注意，分号是必须的。<br />
    /// Start Siemens PLC in RUN mode, parameter information can carry station number information "s=2;", note that the semicolon is required.
    /// </summary>
    /// <param name="parameter">额外的参数信息，例如可以携带站号信息 "s=2;", 注意，分号是必须的。</param>
    /// <returns>是否启动成功</returns>
    [EstMqttApi]
    public OperateResult Start(string parameter = "")
    {
      byte parameter1 = (byte) EstHelper.ExtractParameter(ref parameter, "s", (int) this.Station);
      byte[] send = new byte[39]
      {
        (byte) 104,
        (byte) 33,
        (byte) 33,
        (byte) 104,
        parameter1,
        (byte) 0,
        (byte) 108,
        (byte) 50,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 20,
        (byte) 0,
        (byte) 0,
        (byte) 40,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 253,
        (byte) 0,
        (byte) 0,
        (byte) 9,
        (byte) 80,
        (byte) 95,
        (byte) 80,
        (byte) 82,
        (byte) 79,
        (byte) 71,
        (byte) 82,
        (byte) 65,
        (byte) 77,
        (byte) 170,
        (byte) 22
      };
      lock (this.communicationLock)
      {
        OperateResult<byte[]> operateResult1 = this.ReadBase(send);
        if (!operateResult1.IsSuccess)
          return (OperateResult) operateResult1;
        if (operateResult1.Content[0] != (byte) 229)
          return (OperateResult) new OperateResult<byte[]>("PLC Receive Check Failed:" + operateResult1.Content[0].ToString());
        OperateResult<byte[]> operateResult2 = this.ReadBase(SiemensPPIOverTcp.GetExecuteConfirm(parameter1));
        return !operateResult2.IsSuccess ? (OperateResult) operateResult2 : OperateResult.CreateSuccessResult();
      }
    }

    /// <summary>
    /// 停止西门子PLC，切换为Stop模式，参数信息可以携带站号信息 "s=2;", 注意，分号是必须的。<br />
    /// Stop Siemens PLC and switch to Stop mode, parameter information can carry station number information "s=2;", note that the semicolon is required.
    /// </summary>
    /// <param name="parameter">额外的参数信息，例如可以携带站号信息 "s=2;", 注意，分号是必须的。</param>
    /// <returns>是否停止成功</returns>
    [EstMqttApi]
    public OperateResult Stop(string parameter = "")
    {
      byte parameter1 = (byte) EstHelper.ExtractParameter(ref parameter, "s", (int) this.Station);
      byte[] send = new byte[35]
      {
        (byte) 104,
        (byte) 29,
        (byte) 29,
        (byte) 104,
        parameter1,
        (byte) 0,
        (byte) 108,
        (byte) 50,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 16,
        (byte) 0,
        (byte) 0,
        (byte) 41,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 9,
        (byte) 80,
        (byte) 95,
        (byte) 80,
        (byte) 82,
        (byte) 79,
        (byte) 71,
        (byte) 82,
        (byte) 65,
        (byte) 77,
        (byte) 170,
        (byte) 22
      };
      lock (this.communicationLock)
      {
        OperateResult<byte[]> operateResult1 = this.ReadBase(send);
        if (!operateResult1.IsSuccess)
          return (OperateResult) operateResult1;
        if (operateResult1.Content[0] != (byte) 229)
          return (OperateResult) new OperateResult<byte[]>("PLC Receive Check Failed:" + operateResult1.Content[0].ToString());
        OperateResult<byte[]> operateResult2 = this.ReadBase(SiemensPPIOverTcp.GetExecuteConfirm(parameter1));
        return !operateResult2.IsSuccess ? (OperateResult) operateResult2 : OperateResult.CreateSuccessResult();
      }
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("SiemensPPI[{0}:{1}]", (object) this.PortName, (object) this.BaudRate);
  }
}
