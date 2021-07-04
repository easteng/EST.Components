// Decompiled with JetBrains decompiler
// Type: EstCommunication.ModBus.ModbusInfo
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using EstCommunication.Core;
using EstCommunication.Core.Address;
using EstCommunication.Serial;
using System;
using System.Collections.Generic;

namespace EstCommunication.ModBus
{
  /// <summary>
  /// Modbus协议相关的一些信息，包括功能码定义，报文的生成的定义等等信息<br />
  /// Some information related to Modbus protocol, including function code definition, definition of message generation, etc.
  /// </summary>
  public class ModbusInfo
  {
    /// <summary>读取线圈</summary>
    public const byte ReadCoil = 1;
    /// <summary>读取离散量</summary>
    public const byte ReadDiscrete = 2;
    /// <summary>读取寄存器</summary>
    public const byte ReadRegister = 3;
    /// <summary>读取输入寄存器</summary>
    public const byte ReadInputRegister = 4;
    /// <summary>写单个线圈</summary>
    public const byte WriteOneCoil = 5;
    /// <summary>写单个寄存器</summary>
    public const byte WriteOneRegister = 6;
    /// <summary>写多个线圈</summary>
    public const byte WriteCoil = 15;
    /// <summary>写多个寄存器</summary>
    public const byte WriteRegister = 16;
    /// <summary>使用掩码的方式写入寄存器</summary>
    public const byte WriteMaskRegister = 22;
    /// <summary>不支持该功能码</summary>
    public const byte FunctionCodeNotSupport = 1;
    /// <summary>该地址越界</summary>
    public const byte FunctionCodeOverBound = 2;
    /// <summary>读取长度超过最大值</summary>
    public const byte FunctionCodeQuantityOver = 3;
    /// <summary>读写异常</summary>
    public const byte FunctionCodeReadWriteException = 4;

    private static void CheckModbusAddressStart(ModbusAddress mAddress, bool isStartWithZero)
    {
      if (isStartWithZero)
        return;
      if (mAddress.Address < (ushort) 1)
        throw new Exception(StringResources.Language.ModbusAddressMustMoreThanOne);
      --mAddress.Address;
    }

    /// <summary>
    /// 构建Modbus读取数据的核心报文，需要指定地址，长度，站号，是否起始地址0，默认的功能码应该根据bool或是字来区分<br />
    /// To construct the core message of Modbus reading data, you need to specify the address, length, station number,
    /// whether the starting address is 0, and the default function code should be distinguished according to bool or word
    /// </summary>
    /// <param name="address">Modbus的富文本地址</param>
    /// <param name="length">读取的数据长度</param>
    /// <param name="station">默认的站号信息</param>
    /// <param name="isStartWithZero">起始地址是否从0开始</param>
    /// <param name="defaultFunction">默认的功能码</param>
    /// <returns>包含最终命令的结果对象</returns>
    public static OperateResult<byte[][]> BuildReadModbusCommand(
      string address,
      ushort length,
      byte station,
      bool isStartWithZero,
      byte defaultFunction)
    {
      try
      {
        ModbusAddress mAddress = new ModbusAddress(address, station, defaultFunction);
        ModbusInfo.CheckModbusAddressStart(mAddress, isStartWithZero);
        return ModbusInfo.BuildReadModbusCommand(mAddress, length);
      }
      catch (Exception ex)
      {
        return new OperateResult<byte[][]>(ex.Message);
      }
    }

    /// <summary>
    /// 构建Modbus读取数据的核心报文，需要指定地址，长度，站号，是否起始地址0，默认的功能码应该根据bool或是字来区分<br />
    /// To construct the core message of Modbus reading data, you need to specify the address, length, station number,
    /// whether the starting address is 0, and the default function code should be distinguished according to bool or word
    /// </summary>
    /// <param name="mAddress">Modbus的富文本地址</param>
    /// <param name="length">读取的数据长度</param>
    /// <returns>包含最终命令的结果对象</returns>
    public static OperateResult<byte[][]> BuildReadModbusCommand(
      ModbusAddress mAddress,
      ushort length)
    {
      List<byte[]> numArrayList = new List<byte[]>();
      if (mAddress.Function == 1 || mAddress.Function == 2 || (mAddress.Function == 3 || mAddress.Function == 4) || Authorization.asdniasnfaksndiqwhawfskhfaiw())
      {
        OperateResult<int[], int[]> operateResult = EstHelper.SplitReadLength((int) mAddress.Address, length, mAddress.Function == 1 || mAddress.Function == 2 ? (ushort) 2000 : (ushort) 120);
        for (int index = 0; index < operateResult.Content1.Length; ++index)
        {
          byte[] numArray = new byte[6]
          {
            (byte) mAddress.Station,
            (byte) mAddress.Function,
            BitConverter.GetBytes(operateResult.Content1[index])[1],
            BitConverter.GetBytes(operateResult.Content1[index])[0],
            BitConverter.GetBytes(operateResult.Content2[index])[1],
            BitConverter.GetBytes(operateResult.Content2[index])[0]
          };
          numArrayList.Add(numArray);
        }
      }
      else
      {
        byte[] numArray = new byte[6]
        {
          (byte) mAddress.Station,
          (byte) mAddress.Function,
          BitConverter.GetBytes(mAddress.Address)[1],
          BitConverter.GetBytes(mAddress.Address)[0],
          BitConverter.GetBytes(length)[1],
          BitConverter.GetBytes(length)[0]
        };
        numArrayList.Add(numArray);
      }
      return OperateResult.CreateSuccessResult<byte[][]>(numArrayList.ToArray());
    }

    /// <summary>
    /// 构建Modbus写入bool数据的核心报文，需要指定地址，长度，站号，是否起始地址0，默认的功能码<br />
    /// To construct the core message that Modbus writes to bool data, you need to specify the address, length,
    /// station number, whether the starting address is 0, and the default function code
    /// </summary>
    /// <param name="address">Modbus的富文本地址</param>
    /// <param name="values">bool数组的信息</param>
    /// <param name="station">默认的站号信息</param>
    /// <param name="isStartWithZero">起始地址是否从0开始</param>
    /// <param name="defaultFunction">默认的功能码</param>
    /// <returns>包含最终命令的结果对象</returns>
    public static OperateResult<byte[]> BuildWriteBoolModbusCommand(
      string address,
      bool[] values,
      byte station,
      bool isStartWithZero,
      byte defaultFunction)
    {
      try
      {
        ModbusAddress mAddress = new ModbusAddress(address, station, defaultFunction);
        ModbusInfo.CheckModbusAddressStart(mAddress, isStartWithZero);
        return ModbusInfo.BuildWriteBoolModbusCommand(mAddress, values);
      }
      catch (Exception ex)
      {
        return new OperateResult<byte[]>(ex.Message);
      }
    }

    /// <summary>
    /// 构建Modbus写入bool数据的核心报文，需要指定地址，长度，站号，是否起始地址0，默认的功能码<br />
    /// To construct the core message that Modbus writes to bool data, you need to specify the address, length, station number, whether the starting address is 0, and the default function code
    /// </summary>
    /// <param name="address">Modbus的富文本地址</param>
    /// <param name="value">bool的信息</param>
    /// <param name="station">默认的站号信息</param>
    /// <param name="isStartWithZero">起始地址是否从0开始</param>
    /// <param name="defaultFunction">默认的功能码</param>
    /// <returns>包含最终命令的结果对象</returns>
    public static OperateResult<byte[]> BuildWriteBoolModbusCommand(
      string address,
      bool value,
      byte station,
      bool isStartWithZero,
      byte defaultFunction)
    {
      try
      {
        if (address.IndexOf('.') <= 0)
        {
          ModbusAddress mAddress = new ModbusAddress(address, station, defaultFunction);
          ModbusInfo.CheckModbusAddressStart(mAddress, isStartWithZero);
          return ModbusInfo.BuildWriteBoolModbusCommand(mAddress, value);
        }
        if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
          return new OperateResult<byte[]>(StringResources.Language.InsufficientPrivileges);
        int int32 = Convert.ToInt32(address.Substring(address.IndexOf('.') + 1));
        if (int32 < 0 || int32 > 15)
          return new OperateResult<byte[]>(StringResources.Language.ModbusBitIndexOverstep);
        int num1 = 1 << int32;
        int num2 = ~num1;
        if (!value)
          num1 = 0;
        return ModbusInfo.BuildWriteMaskModbusCommand(address.Substring(0, address.IndexOf('.')), (ushort) num2, (ushort) num1, station, isStartWithZero, (byte) 22);
      }
      catch (Exception ex)
      {
        return new OperateResult<byte[]>(ex.Message);
      }
    }

    /// <summary>
    /// 构建Modbus写入bool数组的核心报文，需要指定地址，长度，站号，是否起始地址0，默认的功能码<br />
    /// To construct the core message that Modbus writes to the bool array, you need to specify the address, length,
    /// station number, whether the starting address is 0, and the default function code
    /// </summary>
    /// <param name="mAddress">Modbus的富文本地址</param>
    /// <param name="values">bool数组的信息</param>
    /// <returns>包含最终命令的结果对象</returns>
    public static OperateResult<byte[]> BuildWriteBoolModbusCommand(
      ModbusAddress mAddress,
      bool[] values)
    {
      try
      {
        byte[] numArray1 = SoftBasic.BoolArrayToByte(values);
        byte[] numArray2 = new byte[7 + numArray1.Length];
        numArray2[0] = (byte) mAddress.Station;
        numArray2[1] = (byte) mAddress.Function;
        numArray2[2] = BitConverter.GetBytes(mAddress.Address)[1];
        numArray2[3] = BitConverter.GetBytes(mAddress.Address)[0];
        numArray2[4] = (byte) (values.Length / 256);
        numArray2[5] = (byte) (values.Length % 256);
        numArray2[6] = (byte) numArray1.Length;
        numArray1.CopyTo((Array) numArray2, 7);
        return OperateResult.CreateSuccessResult<byte[]>(numArray2);
      }
      catch (Exception ex)
      {
        return new OperateResult<byte[]>(ex.Message);
      }
    }

    /// <summary>
    /// 构建Modbus写入bool数据的核心报文，需要指定地址，长度，站号，是否起始地址0，默认的功能码<br />
    /// To construct the core message that Modbus writes to bool data, you need to specify the address, length, station number, whether the starting address is 0, and the default function code
    /// </summary>
    /// <param name="mAddress">Modbus的富文本地址</param>
    /// <param name="value">bool数据的信息</param>
    /// <returns>包含最终命令的结果对象</returns>
    public static OperateResult<byte[]> BuildWriteBoolModbusCommand(
      ModbusAddress mAddress,
      bool value)
    {
      byte[] numArray = new byte[6]
      {
        (byte) mAddress.Station,
        (byte) mAddress.Function,
        BitConverter.GetBytes(mAddress.Address)[1],
        BitConverter.GetBytes(mAddress.Address)[0],
        (byte) 0,
        (byte) 0
      };
      if (value)
      {
        numArray[4] = byte.MaxValue;
        numArray[5] = (byte) 0;
      }
      else
      {
        numArray[4] = (byte) 0;
        numArray[5] = (byte) 0;
      }
      return OperateResult.CreateSuccessResult<byte[]>(numArray);
    }

    /// <summary>
    /// 构建Modbus写入字数据的核心报文，需要指定地址，长度，站号，是否起始地址0，默认的功能码<br />
    /// To construct the core message of Modbus writing word data, you need to specify the address, length,
    /// station number, whether the starting address is 0, and the default function code
    /// </summary>
    /// <param name="address">Modbus的富文本地址</param>
    /// <param name="values">bool数组的信息</param>
    /// <param name="station">默认的站号信息</param>
    /// <param name="isStartWithZero">起始地址是否从0开始</param>
    /// <param name="defaultFunction">默认的功能码</param>
    /// <returns>包含最终命令的结果对象</returns>
    public static OperateResult<byte[]> BuildWriteWordModbusCommand(
      string address,
      byte[] values,
      byte station,
      bool isStartWithZero,
      byte defaultFunction)
    {
      try
      {
        ModbusAddress mAddress = new ModbusAddress(address, station, defaultFunction);
        if (mAddress.Function == 3)
          mAddress.Function = (int) defaultFunction;
        ModbusInfo.CheckModbusAddressStart(mAddress, isStartWithZero);
        return ModbusInfo.BuildWriteWordModbusCommand(mAddress, values);
      }
      catch (Exception ex)
      {
        return new OperateResult<byte[]>(ex.Message);
      }
    }

    /// <summary>
    /// 构建Modbus写入字数据的核心报文，需要指定地址，长度，站号，是否起始地址0，默认的功能码<br />
    /// To construct the core message of Modbus writing word data, you need to specify the address, length,
    /// station number, whether the starting address is 0, and the default function code
    /// </summary>
    /// <param name="address">Modbus的富文本地址</param>
    /// <param name="value">short数据信息</param>
    /// <param name="station">默认的站号信息</param>
    /// <param name="isStartWithZero">起始地址是否从0开始</param>
    /// <param name="defaultFunction">默认的功能码</param>
    /// <returns>包含最终命令的结果对象</returns>
    public static OperateResult<byte[]> BuildWriteWordModbusCommand(
      string address,
      short value,
      byte station,
      bool isStartWithZero,
      byte defaultFunction)
    {
      try
      {
        ModbusAddress mAddress = new ModbusAddress(address, station, defaultFunction);
        if (mAddress.Function == 3)
          mAddress.Function = (int) defaultFunction;
        ModbusInfo.CheckModbusAddressStart(mAddress, isStartWithZero);
        return ModbusInfo.BuildWriteOneRegisterModbusCommand(mAddress, value);
      }
      catch (Exception ex)
      {
        return new OperateResult<byte[]>(ex.Message);
      }
    }

    /// <summary>
    /// 构建Modbus写入字数据的核心报文，需要指定地址，长度，站号，是否起始地址0，默认的功能码<br />
    /// To construct the core message of Modbus writing word data, you need to specify the address, length,
    /// station number, whether the starting address is 0, and the default function code
    /// </summary>
    /// <param name="address">Modbus的富文本地址</param>
    /// <param name="value">bool数组的信息</param>
    /// <param name="station">默认的站号信息</param>
    /// <param name="isStartWithZero">起始地址是否从0开始</param>
    /// <param name="defaultFunction">默认的功能码</param>
    /// <returns>包含最终命令的结果对象</returns>
    public static OperateResult<byte[]> BuildWriteWordModbusCommand(
      string address,
      ushort value,
      byte station,
      bool isStartWithZero,
      byte defaultFunction)
    {
      try
      {
        ModbusAddress mAddress = new ModbusAddress(address, station, defaultFunction);
        if (mAddress.Function == 3)
          mAddress.Function = (int) defaultFunction;
        ModbusInfo.CheckModbusAddressStart(mAddress, isStartWithZero);
        return ModbusInfo.BuildWriteOneRegisterModbusCommand(mAddress, value);
      }
      catch (Exception ex)
      {
        return new OperateResult<byte[]>(ex.Message);
      }
    }

    /// <summary>
    /// 构建Modbus写入掩码的核心报文，需要指定地址，长度，站号，是否起始地址0，默认的功能码<br />
    /// To construct the Modbus write mask core message, you need to specify the address, length,
    /// station number, whether the starting address is 0, and the default function code
    /// </summary>
    /// <param name="address">Modbus的富文本地址</param>
    /// <param name="andMask">进行与操作的掩码信息</param>
    /// <param name="orMask">进行或操作的掩码信息</param>
    /// <param name="station">默认的站号信息</param>
    /// <param name="isStartWithZero">起始地址是否从0开始</param>
    /// <param name="defaultFunction">默认的功能码</param>
    /// <returns>包含最终命令的结果对象</returns>
    public static OperateResult<byte[]> BuildWriteMaskModbusCommand(
      string address,
      ushort andMask,
      ushort orMask,
      byte station,
      bool isStartWithZero,
      byte defaultFunction)
    {
      try
      {
        ModbusAddress mAddress = new ModbusAddress(address, station, defaultFunction);
        if (mAddress.Function == 3)
          mAddress.Function = (int) defaultFunction;
        ModbusInfo.CheckModbusAddressStart(mAddress, isStartWithZero);
        return ModbusInfo.BuildWriteMaskModbusCommand(mAddress, andMask, orMask);
      }
      catch (Exception ex)
      {
        return new OperateResult<byte[]>(ex.Message);
      }
    }

    /// <summary>
    /// 构建Modbus写入字数据的核心报文，需要指定地址，长度，站号，是否起始地址0，默认的功能码<br />
    /// To construct the core message of Modbus writing word data, you need to specify the address, length,
    /// station number, whether the starting address is 0, and the default function code
    /// </summary>
    /// <param name="mAddress">Modbus的富文本地址</param>
    /// <param name="values">bool数组的信息</param>
    /// <returns>包含最终命令的结果对象</returns>
    public static OperateResult<byte[]> BuildWriteWordModbusCommand(
      ModbusAddress mAddress,
      byte[] values)
    {
      byte[] numArray = new byte[7 + values.Length];
      numArray[0] = (byte) mAddress.Station;
      numArray[1] = (byte) mAddress.Function;
      numArray[2] = BitConverter.GetBytes(mAddress.Address)[1];
      numArray[3] = BitConverter.GetBytes(mAddress.Address)[0];
      numArray[4] = (byte) (values.Length / 2 / 256);
      numArray[5] = (byte) (values.Length / 2 % 256);
      numArray[6] = (byte) values.Length;
      values.CopyTo((Array) numArray, 7);
      return OperateResult.CreateSuccessResult<byte[]>(numArray);
    }

    /// <summary>
    /// 构建Modbus写入掩码数据的核心报文，需要指定地址，长度，站号，是否起始地址0，默认的功能码<br />
    /// To construct the core message of Modbus writing mask data, you need to specify the address, length,
    /// station number, whether the starting address is 0, and the default function code
    /// </summary>
    /// <param name="mAddress">Modbus的富文本地址</param>
    /// <param name="andMask">等待进行与操作的掩码</param>
    /// <param name="orMask">等待进行或操作的掩码</param>
    /// <returns>包含最终命令的结果对象</returns>
    public static OperateResult<byte[]> BuildWriteMaskModbusCommand(
      ModbusAddress mAddress,
      ushort andMask,
      ushort orMask)
    {
      return OperateResult.CreateSuccessResult<byte[]>(new byte[8]
      {
        (byte) mAddress.Station,
        (byte) mAddress.Function,
        BitConverter.GetBytes(mAddress.Address)[1],
        BitConverter.GetBytes(mAddress.Address)[0],
        BitConverter.GetBytes(andMask)[1],
        BitConverter.GetBytes(andMask)[0],
        BitConverter.GetBytes(orMask)[1],
        BitConverter.GetBytes(orMask)[0]
      });
    }

    /// <summary>
    /// 构建Modbus写入字数据的核心报文，需要指定地址，长度，站号，是否起始地址0，默认的功能码<br />
    /// To construct the core message of Modbus writing word data, you need to specify the address, length,
    /// station number, whether the starting address is 0, and the default function code
    /// </summary>
    /// <param name="mAddress">Modbus的富文本地址</param>
    /// <param name="value">short的值</param>
    /// <returns>包含最终命令的结果对象</returns>
    public static OperateResult<byte[]> BuildWriteOneRegisterModbusCommand(
      ModbusAddress mAddress,
      short value)
    {
      return OperateResult.CreateSuccessResult<byte[]>(new byte[6]
      {
        (byte) mAddress.Station,
        (byte) mAddress.Function,
        BitConverter.GetBytes(mAddress.Address)[1],
        BitConverter.GetBytes(mAddress.Address)[0],
        BitConverter.GetBytes(value)[1],
        BitConverter.GetBytes(value)[0]
      });
    }

    /// <summary>
    /// 构建Modbus写入字数据的核心报文，需要指定地址，长度，站号，是否起始地址0，默认的功能码<br />
    /// To construct the core message of Modbus writing word data, you need to specify the address, length,
    /// station number, whether the starting address is 0, and the default function code
    /// </summary>
    /// <param name="mAddress">Modbus的富文本地址</param>
    /// <param name="value">ushort的值</param>
    /// <returns>包含最终命令的结果对象</returns>
    public static OperateResult<byte[]> BuildWriteOneRegisterModbusCommand(
      ModbusAddress mAddress,
      ushort value)
    {
      return OperateResult.CreateSuccessResult<byte[]>(new byte[6]
      {
        (byte) mAddress.Station,
        (byte) mAddress.Function,
        BitConverter.GetBytes(mAddress.Address)[1],
        BitConverter.GetBytes(mAddress.Address)[0],
        BitConverter.GetBytes(value)[1],
        BitConverter.GetBytes(value)[0]
      });
    }

    /// <summary>
    /// 从返回的modbus的书内容中，提取出真实的数据，适用于写入和读取操作<br />
    /// Extract real data from the content of the returned modbus book, suitable for writing and reading operations
    /// </summary>
    /// <param name="response">返回的核心modbus报文信息</param>
    /// <returns>结果数据内容</returns>
    public static OperateResult<byte[]> ExtractActualData(byte[] response)
    {
      try
      {
        if (response[1] >= (byte) 128)
          return new OperateResult<byte[]>(ModbusInfo.GetDescriptionByErrorCode(response[2]));
        return response.Length > 3 ? OperateResult.CreateSuccessResult<byte[]>(SoftBasic.ArrayRemoveBegin<byte>(response, 3)) : OperateResult.CreateSuccessResult<byte[]>(new byte[0]);
      }
      catch (Exception ex)
      {
        return new OperateResult<byte[]>(ex.Message);
      }
    }

    /// <summary>
    /// 将modbus指令打包成Modbus-Tcp指令，需要指定ID信息来添加6个字节的报文头<br />
    /// Pack the Modbus command into Modbus-Tcp command, you need to specify the ID information to add a 6-byte message header
    /// </summary>
    /// <param name="modbus">Modbus核心指令</param>
    /// <param name="id">消息的序号</param>
    /// <returns>Modbus-Tcp指令</returns>
    public static byte[] PackCommandToTcp(byte[] modbus, ushort id)
    {
      byte[] numArray = new byte[modbus.Length + 6];
      numArray[0] = BitConverter.GetBytes(id)[1];
      numArray[1] = BitConverter.GetBytes(id)[0];
      numArray[4] = BitConverter.GetBytes(modbus.Length)[1];
      numArray[5] = BitConverter.GetBytes(modbus.Length)[0];
      modbus.CopyTo((Array) numArray, 6);
      return numArray;
    }

    /// <summary>
    /// 将modbus-tcp的报文数据重新还原成modbus指令，移除6个字节的报文头数据<br />
    /// Re-modify the message data of modbus-tcp into the modbus command, remove the 6-byte message header data
    /// </summary>
    /// <param name="modbusTcp">modbus-tcp的报文</param>
    /// <returns>modbus数据报文</returns>
    public static byte[] ExplodeTcpCommandToCore(byte[] modbusTcp) => modbusTcp.RemoveBegin<byte>(6);

    /// <summary>
    /// 将modbus-rtu的数据重新还原成modbus数据，移除CRC校验的内容<br />
    /// Restore the data of modbus-rtu to modbus data again, remove the content of CRC check
    /// </summary>
    /// <param name="modbusRtu">modbus-rtu的报文</param>
    /// <returns>modbus数据报文</returns>
    public static byte[] ExplodeRtuCommandToCore(byte[] modbusRtu) => modbusRtu.RemoveLast<byte>(2);

    /// <summary>
    /// 将modbus指令打包成Modbus-Rtu指令，在报文的末尾添加CRC16的校验码<br />
    /// Pack the modbus instruction into Modbus-Rtu instruction, add CRC16 check code at the end of the message
    /// </summary>
    /// <param name="modbus">Modbus指令</param>
    /// <returns>Modbus-Rtu指令</returns>
    public static byte[] PackCommandToRtu(byte[] modbus) => SoftCRC16.CRC16(modbus);

    /// <summary>
    /// 将一个modbus核心的数据报文，转换成modbus-ascii的数据报文，增加LRC校验，增加首尾标记数据<br />
    /// Convert a Modbus core data message into a Modbus-ascii data message, add LRC check, and add head and tail tag data
    /// </summary>
    /// <param name="modbus">modbus-rtu的完整报文，携带相关的校验码</param>
    /// <returns>可以用于直接发送的modbus-ascii的报文</returns>
    public static byte[] TransModbusCoreToAsciiPackCommand(byte[] modbus) => SoftBasic.SpliceArray<byte>(new byte[1]
    {
      (byte) 58
    }, SoftBasic.BytesToAsciiBytes(SoftLRC.LRC(modbus)), new byte[2]
    {
      (byte) 13,
      (byte) 10
    });

    /// <summary>
    /// 将一个modbus-ascii的数据报文，转换成的modbus核心数据报文，移除首尾标记，移除LRC校验<br />
    /// Convert a Modbus-ascii data message into a Modbus core data message, remove the first and last tags, and remove the LRC check
    /// </summary>
    /// <param name="modbusAscii">modbus-ascii的完整报文，携带相关的校验码</param>
    /// <returns>可以用于直接发送的modbus的报文</returns>
    public static OperateResult<byte[]> TransAsciiPackCommandToCore(byte[] modbusAscii)
    {
      try
      {
        if (modbusAscii[0] != (byte) 58 || modbusAscii[modbusAscii.Length - 2] != (byte) 13 || modbusAscii[modbusAscii.Length - 1] != (byte) 10)
        {
          OperateResult<byte[]> operateResult = new OperateResult<byte[]>();
          operateResult.Message = StringResources.Language.ModbusAsciiFormatCheckFailed + modbusAscii.ToHexString(' ');
          return operateResult;
        }
        byte[] bytes = SoftBasic.AsciiBytesToBytes(modbusAscii.RemoveDouble<byte>(1, 2));
        if (SoftLRC.CheckLRC(bytes))
          return OperateResult.CreateSuccessResult<byte[]>(bytes.RemoveLast<byte>(1));
        OperateResult<byte[]> operateResult1 = new OperateResult<byte[]>();
        operateResult1.Message = StringResources.Language.ModbusLRCCheckFailed + bytes.ToHexString(' ');
        return operateResult1;
      }
      catch (Exception ex)
      {
        OperateResult<byte[]> operateResult = new OperateResult<byte[]>();
        operateResult.Message = ex.Message + modbusAscii.ToHexString(' ');
        return operateResult;
      }
    }

    /// <summary>
    /// 分析Modbus协议的地址信息，该地址适应于tcp及rtu模式<br />
    /// Analysis of the address information of Modbus protocol, the address is adapted to tcp and rtu mode
    /// </summary>
    /// <param name="address">带格式的地址，比如"100"，"x=4;100"，"s=1;100","s=1;x=4;100"</param>
    /// <param name="defaultStation">默认的站号信息</param>
    /// <param name="isStartWithZero">起始地址是否从0开始</param>
    /// <param name="defaultFunction">默认的功能码信息</param>
    /// <returns>转换后的地址信息</returns>
    public static OperateResult<ModbusAddress> AnalysisAddress(
      string address,
      byte defaultStation,
      bool isStartWithZero,
      byte defaultFunction)
    {
      try
      {
        ModbusAddress modbusAddress = new ModbusAddress(address, defaultStation, defaultFunction);
        if (!isStartWithZero)
        {
          if (modbusAddress.Address < (ushort) 1)
            throw new Exception(StringResources.Language.ModbusAddressMustMoreThanOne);
          --modbusAddress.Address;
        }
        return OperateResult.CreateSuccessResult<ModbusAddress>(modbusAddress);
      }
      catch (Exception ex)
      {
        OperateResult<ModbusAddress> operateResult = new OperateResult<ModbusAddress>();
        operateResult.Message = ex.Message;
        return operateResult;
      }
    }

    /// <summary>
    /// 通过错误码来获取到对应的文本消息<br />
    /// Get the corresponding text message through the error code
    /// </summary>
    /// <param name="code">错误码</param>
    /// <returns>错误的文本描述</returns>
    public static string GetDescriptionByErrorCode(byte code)
    {
      switch (code)
      {
        case 1:
          return StringResources.Language.ModbusTcpFunctionCodeNotSupport;
        case 2:
          return StringResources.Language.ModbusTcpFunctionCodeOverBound;
        case 3:
          return StringResources.Language.ModbusTcpFunctionCodeQuantityOver;
        case 4:
          return StringResources.Language.ModbusTcpFunctionCodeReadWriteException;
        default:
          return StringResources.Language.UnknownError;
      }
    }
  }
}
