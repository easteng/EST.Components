// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.Siemens.SiemensMPI
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using EstCommunication.Core;
using EstCommunication.Core.Address;
using EstCommunication.Serial;
using System;

namespace EstCommunication.Profinet.Siemens
{
  /// <summary>
  /// 西门子的MPI协议信息，注意：未测试通过，无法使用<br />
  /// Siemens MPI protocol information, note: it has not passed the test and cannot be used
  /// </summary>
  public class SiemensMPI : SerialDeviceBase
  {
    private byte station = 2;
    private byte[] readConfirm = new byte[15]
    {
      (byte) 104,
      (byte) 8,
      (byte) 8,
      (byte) 104,
      (byte) 130,
      (byte) 128,
      (byte) 92,
      (byte) 22,
      (byte) 2,
      (byte) 176,
      (byte) 7,
      (byte) 0,
      (byte) 45,
      (byte) 22,
      (byte) 229
    };
    private byte[] writeConfirm = new byte[15]
    {
      (byte) 104,
      (byte) 8,
      (byte) 8,
      (byte) 104,
      (byte) 130,
      (byte) 128,
      (byte) 124,
      (byte) 22,
      (byte) 2,
      (byte) 176,
      (byte) 7,
      (byte) 0,
      (byte) 77,
      (byte) 22,
      (byte) 229
    };

    /// <summary>
    /// 实例化一个西门子的MPI协议对象<br />
    /// Instantiate a Siemens MPI protocol object
    /// </summary>
    public SiemensMPI()
    {
      this.ByteTransform = (IByteTransform) new ReverseBytesTransform();
      this.WordLength = (ushort) 2;
    }

    /// <summary>
    /// 西门子PLC的站号信息<br />
    /// Siemens PLC station number information
    /// </summary>
    public byte Station
    {
      get => this.station;
      set
      {
        this.station = value;
        this.readConfirm[4] = (byte) ((uint) value + 128U);
        this.writeConfirm[4] = (byte) ((uint) value + 128U);
        int num1 = 0;
        int num2 = 0;
        for (int index = 4; index < 12; ++index)
        {
          num1 += (int) this.readConfirm[index];
          num2 += (int) this.writeConfirm[index];
        }
        this.readConfirm[12] = (byte) num1;
        this.writeConfirm[12] = (byte) num2;
      }
    }

    /// <summary>
    /// 与PLC进行握手<br />
    /// Handshake with PLC
    /// </summary>
    /// <returns>是否握手成功</returns>
    public OperateResult Handle()
    {
      OperateResult<byte[]> operateResult;
      OperateResult result1;
      while (true)
      {
        operateResult = this.SPReceived(this.sP_ReadData, true);
        if (operateResult.IsSuccess)
        {
          if (operateResult.Content[0] == (byte) 220 && operateResult.Content[1] == (byte) 2 && operateResult.Content[2] == (byte) 2)
          {
            result1 = this.SPSend(this.sP_ReadData, new byte[3]
            {
              (byte) 220,
              (byte) 0,
              (byte) 0
            });
            if (!result1.IsSuccess)
              goto label_4;
          }
          else if (operateResult.Content[0] == (byte) 220 && operateResult.Content[1] == (byte) 0 && operateResult.Content[2] == (byte) 2)
            goto label_6;
        }
        else
          break;
      }
      return (OperateResult) OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult);
label_4:
      return (OperateResult) OperateResult.CreateFailedResult<byte[]>(result1);
label_6:
      OperateResult result2 = this.SPSend(this.sP_ReadData, new byte[3]
      {
        (byte) 220,
        (byte) 2,
        (byte) 0
      });
      return !result2.IsSuccess ? (OperateResult) OperateResult.CreateFailedResult<byte[]>(result2) : OperateResult.CreateSuccessResult();
    }

    /// <summary>
    /// 从西门子的PLC中读取数据信息，地址为"M100","AI100","I0","Q0","V100","S100"等，详细请参照API文档<br />
    /// Read data information from Siemens PLC, the address is "M100", "AI100", "I0", "Q0", "V100", "S100", etc., please refer to the API documentation
    /// </summary>
    /// <param name="address">西门子的地址数据信息</param>
    /// <param name="length">数据长度</param>
    /// <returns>带返回结果的结果对象</returns>
    public override OperateResult<byte[]> Read(string address, ushort length)
    {
      OperateResult<byte[]> operateResult1 = SiemensMPI.BuildReadCommand(this.station, address, length, false);
      if (!operateResult1.IsSuccess)
        return operateResult1;
      if (this.IsClearCacheBeforeRead)
        this.ClearSerialCache();
      OperateResult result1 = this.SPSend(this.sP_ReadData, operateResult1.Content);
      if (!result1.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>(result1);
      OperateResult<byte[]> operateResult2 = this.SPReceived(this.sP_ReadData, true);
      if (!operateResult2.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult2);
      if (operateResult2.Content[14] != (byte) 229)
        return new OperateResult<byte[]>("PLC Receive Check Failed:" + SoftBasic.ByteToHexString(operateResult2.Content));
      OperateResult<byte[]> operateResult3 = this.SPReceived(this.sP_ReadData, true);
      if (!operateResult3.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult3);
      if (operateResult3.Content[19] > (byte) 0)
        return new OperateResult<byte[]>("PLC Receive Check Failed:" + operateResult3.Content[19].ToString());
      OperateResult result2 = this.SPSend(this.sP_ReadData, this.readConfirm);
      if (!result2.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>(result2);
      byte[] numArray = new byte[(int) length];
      if (operateResult3.Content[25] == byte.MaxValue && operateResult3.Content[26] == (byte) 4)
        Array.Copy((Array) operateResult3.Content, 29, (Array) numArray, 0, (int) length);
      return OperateResult.CreateSuccessResult<byte[]>(numArray);
    }

    /// <summary>
    /// 从西门子的PLC中读取bool数据信息，地址为"M100.0","AI100.1","I0.3","Q0.6","V100.4","S100"等，详细请参照API文档<br />
    /// Read the bool data information from Siemens PLC. The addresses are "M100.0", "AI100.1", "I0.3", "Q0.6", "V100.4", "S100", etc. For details, please Refer to API documentation
    /// </summary>
    /// <param name="address">西门子的地址数据信息</param>
    /// <param name="length">数据长度</param>
    /// <returns>带返回结果的结果对象</returns>
    public override OperateResult<bool[]> ReadBool(string address, ushort length)
    {
      OperateResult<byte[]> operateResult1 = SiemensMPI.BuildReadCommand(this.station, address, length, true);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult1);
      OperateResult result1 = this.SPSend(this.sP_ReadData, operateResult1.Content);
      if (!result1.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>(result1);
      OperateResult<byte[]> operateResult2 = this.SPReceived(this.sP_ReadData, true);
      if (!operateResult2.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult2);
      if (operateResult2.Content[14] != (byte) 229)
        return new OperateResult<bool[]>("PLC Receive Check Failed:" + SoftBasic.ByteToHexString(operateResult2.Content));
      OperateResult<byte[]> operateResult3 = this.SPReceived(this.sP_ReadData, true);
      if (!operateResult3.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult3);
      if (operateResult3.Content[19] > (byte) 0)
        return new OperateResult<bool[]>("PLC Receive Check Failed:" + operateResult3.Content[19].ToString());
      OperateResult result2 = this.SPSend(this.sP_ReadData, this.readConfirm);
      if (!result2.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>(result2);
      byte[] InBytes = new byte[operateResult3.Content.Length - 31];
      if (operateResult3.Content[21] == byte.MaxValue && operateResult3.Content[22] == (byte) 3)
        Array.Copy((Array) operateResult3.Content, 28, (Array) InBytes, 0, InBytes.Length);
      return OperateResult.CreateSuccessResult<bool[]>(SoftBasic.ByteToBoolArray(InBytes, (int) length));
    }

    /// <summary>
    /// 将字节数据写入到西门子PLC中，地址为"M100.0","AI100.1","I0.3","Q0.6","V100.4","S100"等，详细请参照API文档<br />
    /// Write byte data to Siemens PLC, the address is "M100.0", "AI100.1", "I0.3", "Q0.6", "V100.4", "S100", etc. Refer to API documentation
    /// </summary>
    /// <param name="address">西门子的地址数据信息</param>
    /// <param name="value">数据长度</param>
    /// <returns>带返回结果的结果对象</returns>
    public override OperateResult Write(string address, byte[] value)
    {
      OperateResult<byte[]> operateResult1 = SiemensMPI.BuildWriteCommand(this.station, address, value);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      if (this.IsClearCacheBeforeRead)
        this.ClearSerialCache();
      OperateResult result1 = this.SPSend(this.sP_ReadData, operateResult1.Content);
      if (!result1.IsSuccess)
        return (OperateResult) OperateResult.CreateFailedResult<byte[]>(result1);
      OperateResult<byte[]> operateResult2 = this.SPReceived(this.sP_ReadData, true);
      if (!operateResult2.IsSuccess)
        return (OperateResult) OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult2);
      if (operateResult2.Content[14] != (byte) 229)
        return (OperateResult) new OperateResult<byte[]>("PLC Receive Check Failed:" + SoftBasic.ByteToHexString(operateResult2.Content));
      OperateResult<byte[]> operateResult3 = this.SPReceived(this.sP_ReadData, true);
      if (!operateResult3.IsSuccess)
        return (OperateResult) OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult3);
      if (operateResult3.Content[25] != byte.MaxValue)
        return (OperateResult) new OperateResult<byte[]>("PLC Receive Check Failed:" + operateResult3.Content[25].ToString());
      OperateResult result2 = this.SPSend(this.sP_ReadData, this.writeConfirm);
      return !result2.IsSuccess ? (OperateResult) OperateResult.CreateFailedResult<byte[]>(result2) : OperateResult.CreateSuccessResult();
    }

    /// <summary>
    /// 从西门子的PLC中读取byte数据信息，地址为"M100.0","AI100.1","I0.3","Q0.6","V100.4","S100"等，详细请参照API文档<br />
    /// Read byte data information from Siemens PLC. The addresses are "M100.0", "AI100.1", "I0.3", "Q0.6", "V100.4", "S100", etc. For details, please Refer to API documentation
    /// </summary>
    /// <param name="address">西门子的地址数据信息</param>
    /// <returns>带返回结果的结果对象</returns>
    public OperateResult<byte> ReadByte(string address) => ByteTransformHelper.GetResultFromArray<byte>(this.Read(address, (ushort) 1));

    /// <summary>
    /// 将byte数据写入到西门子PLC中，地址为"M100.0","AI100.1","I0.3","Q0.6","V100.4","S100"等，详细请参照API文档<br />
    /// Write byte data to Siemens PLC, the address is "M100.0", "AI100.1", "I0.3", "Q0.6", "V100.4", "S100", etc. API documentation
    /// </summary>
    /// <param name="address">西门子的地址数据信息</param>
    /// <param name="value">数据长度</param>
    /// <returns>带返回结果的结果对象</returns>
    public OperateResult Write(string address, byte value) => this.Write(address, new byte[1]
    {
      value
    });

    /// <inheritdoc />
    public override string ToString() => string.Format("SiemensMPI[{0}:{1}]", (object) this.PortName, (object) this.BaudRate);

    /// <summary>
    /// 生成一个读取字数据指令头的通用方法<br />
    /// A general method for generating a command header to read a Word data
    /// </summary>
    /// <param name="station">设备的站号信息 -&gt; Station number information for the device</param>
    /// <param name="address">起始地址，例如M100，I0，Q0，V100 -&gt;
    /// Start address, such as M100,I0,Q0,V100</param>
    /// <param name="length">读取数据长度 -&gt; Read Data length</param>
    /// <param name="isBit">是否为位读取</param>
    /// <returns>包含结果对象的报文 -&gt; Message containing the result object</returns>
    public static OperateResult<byte[]> BuildReadCommand(
      byte station,
      string address,
      ushort length,
      bool isBit)
    {
      OperateResult<S7AddressData> from = S7AddressData.ParseFrom(address, length);
      if (!from.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) from);
      byte[] numArray = new byte[38];
      numArray[0] = (byte) 104;
      numArray[1] = BitConverter.GetBytes(numArray.Length - 6)[0];
      numArray[2] = BitConverter.GetBytes(numArray.Length - 6)[0];
      numArray[3] = (byte) 104;
      numArray[4] = (byte) ((uint) station + 128U);
      numArray[5] = (byte) 128;
      numArray[6] = (byte) 124;
      numArray[7] = (byte) 22;
      numArray[8] = (byte) 1;
      numArray[9] = (byte) 241;
      numArray[10] = (byte) 0;
      numArray[11] = (byte) 50;
      numArray[12] = (byte) 1;
      numArray[13] = (byte) 0;
      numArray[14] = (byte) 0;
      numArray[15] = (byte) 51;
      numArray[16] = (byte) 2;
      numArray[17] = (byte) 0;
      numArray[18] = (byte) 14;
      numArray[19] = (byte) 0;
      numArray[20] = (byte) 0;
      numArray[21] = (byte) 4;
      numArray[22] = (byte) 1;
      numArray[23] = (byte) 18;
      numArray[24] = (byte) 10;
      numArray[25] = (byte) 16;
      numArray[26] = isBit ? (byte) 1 : (byte) 2;
      numArray[27] = BitConverter.GetBytes(length)[1];
      numArray[28] = BitConverter.GetBytes(length)[0];
      numArray[29] = BitConverter.GetBytes(from.Content.DbBlock)[1];
      numArray[30] = BitConverter.GetBytes(from.Content.DbBlock)[0];
      numArray[31] = from.Content.DataCode;
      numArray[32] = BitConverter.GetBytes(from.Content.AddressStart)[2];
      numArray[33] = BitConverter.GetBytes(from.Content.AddressStart)[1];
      numArray[34] = BitConverter.GetBytes(from.Content.AddressStart)[0];
      int num = 0;
      for (int index = 4; index < 35; ++index)
        num += (int) numArray[index];
      numArray[35] = BitConverter.GetBytes(num)[0];
      numArray[36] = (byte) 22;
      numArray[37] = (byte) 229;
      return OperateResult.CreateSuccessResult<byte[]>(numArray);
    }

    /// <summary>
    /// 生成一个写入PLC数据信息的报文内容<br />
    /// Generate a message content to write PLC data information
    /// </summary>
    /// <param name="station">PLC的站号</param>
    /// <param name="address">地址</param>
    /// <param name="values">数据值</param>
    /// <returns>是否写入成功</returns>
    public static OperateResult<byte[]> BuildWriteCommand(
      byte station,
      string address,
      byte[] values)
    {
      OperateResult<S7AddressData> from = S7AddressData.ParseFrom(address);
      if (!from.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) from);
      int length = values.Length;
      byte[] numArray = new byte[42 + values.Length];
      numArray[0] = (byte) 104;
      numArray[1] = BitConverter.GetBytes(numArray.Length - 6)[0];
      numArray[2] = BitConverter.GetBytes(numArray.Length - 6)[0];
      numArray[3] = (byte) 104;
      numArray[4] = (byte) ((uint) station + 128U);
      numArray[5] = (byte) 128;
      numArray[6] = (byte) 92;
      numArray[7] = (byte) 22;
      numArray[8] = (byte) 2;
      numArray[9] = (byte) 241;
      numArray[10] = (byte) 0;
      numArray[11] = (byte) 50;
      numArray[12] = (byte) 1;
      numArray[13] = (byte) 0;
      numArray[14] = (byte) 0;
      numArray[15] = (byte) 67;
      numArray[16] = (byte) 2;
      numArray[17] = (byte) 0;
      numArray[18] = (byte) 14;
      numArray[19] = (byte) 0;
      numArray[20] = (byte) (values.Length + 4);
      numArray[21] = (byte) 5;
      numArray[22] = (byte) 1;
      numArray[23] = (byte) 18;
      numArray[24] = (byte) 10;
      numArray[25] = (byte) 16;
      numArray[26] = (byte) 2;
      numArray[27] = BitConverter.GetBytes(length)[0];
      numArray[28] = BitConverter.GetBytes(length)[1];
      numArray[29] = BitConverter.GetBytes(from.Content.DbBlock)[0];
      numArray[30] = BitConverter.GetBytes(from.Content.DbBlock)[1];
      numArray[31] = from.Content.DataCode;
      numArray[32] = BitConverter.GetBytes(from.Content.AddressStart)[2];
      numArray[33] = BitConverter.GetBytes(from.Content.AddressStart)[1];
      numArray[34] = BitConverter.GetBytes(from.Content.AddressStart)[0];
      numArray[35] = (byte) 0;
      numArray[36] = (byte) 4;
      numArray[37] = BitConverter.GetBytes(length * 8)[1];
      numArray[38] = BitConverter.GetBytes(length * 8)[0];
      values.CopyTo((Array) numArray, 39);
      int num = 0;
      for (int index = 4; index < numArray.Length - 3; ++index)
        num += (int) numArray[index];
      numArray[numArray.Length - 3] = BitConverter.GetBytes(num)[0];
      numArray[numArray.Length - 2] = (byte) 22;
      numArray[numArray.Length - 1] = (byte) 229;
      return OperateResult.CreateSuccessResult<byte[]>(numArray);
    }

    /// <summary>根据错误信息，获取到文本信息</summary>
    /// <param name="code">状态</param>
    /// <returns>消息文本</returns>
    public static string GetMsgFromStatus(byte code)
    {
      switch (code)
      {
        case 1:
          return "Hardware fault";
        case 3:
          return "Illegal object access";
        case 5:
          return "Invalid address(incorrent variable address)";
        case 6:
          return "Data type is not supported";
        case 10:
          return "Object does not exist or length error";
        case byte.MaxValue:
          return "No error";
        default:
          return StringResources.Language.UnknownError;
      }
    }

    /// <summary>根据错误信息，获取到文本信息</summary>
    /// <param name="errorClass">错误类型</param>
    /// <param name="errorCode">错误代码</param>
    /// <returns>错误信息</returns>
    public static string GetMsgFromStatus(byte errorClass, byte errorCode)
    {
      if (errorClass == (byte) 128 && errorCode == (byte) 1)
        return "Switch in wrong position for requested operation";
      if (errorClass == (byte) 129 && errorCode == (byte) 4)
        return "Miscellaneous structure error in command.  Command is not supportedby CPU";
      if (errorClass == (byte) 132 && errorCode == (byte) 4)
        return "CPU is busy processing an upload or download CPU cannot process command because of system fault condition";
      if (errorClass == (byte) 133 && errorCode == (byte) 0)
        return "Length fields are not correct or do not agree with the amount of data received";
      int num;
      switch (errorClass)
      {
        case 210:
          return "Error in upload or download command";
        case 214:
          return "Protection error(password)";
        case 220:
          num = errorCode == (byte) 1 ? 1 : 0;
          break;
        default:
          num = 0;
          break;
      }
      return num != 0 ? "Error in time-of-day clock data" : StringResources.Language.UnknownError;
    }
  }
}
