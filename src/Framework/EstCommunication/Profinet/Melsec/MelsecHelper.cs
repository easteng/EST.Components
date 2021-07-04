// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.Melsec.MelsecHelper
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using EstCommunication.Core.Address;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EstCommunication.Profinet.Melsec
{
  /// <summary>
  /// 所有三菱通讯类的通用辅助工具类，包含了一些通用的静态方法，可以使用本类来获取一些原始的报文信息。详细的操作参见例子<br />
  /// All general auxiliary tool classes of Mitsubishi communication class include some general static methods.
  /// You can use this class to get some primitive message information. See the example for detailed operation
  /// </summary>
  public class MelsecHelper
  {
    /// <summary>
    /// 解析A1E协议数据地址<br />
    /// Parse A1E protocol data address
    /// </summary>
    /// <param name="address">数据地址</param>
    /// <returns>结果对象</returns>
    public static OperateResult<MelsecA1EDataType, int> McA1EAnalysisAddress(
      string address)
    {
      OperateResult<MelsecA1EDataType, int> operateResult = new OperateResult<MelsecA1EDataType, int>();
      try
      {
        switch (address[0])
        {
          case 'B':
          case 'b':
            operateResult.Content1 = MelsecA1EDataType.B;
            operateResult.Content2 = Convert.ToInt32(address.Substring(1), MelsecA1EDataType.B.FromBase);
            break;
          case 'C':
          case 'c':
            if (address[1] == 'S' || address[1] == 's')
            {
              operateResult.Content1 = MelsecA1EDataType.CS;
              operateResult.Content2 = Convert.ToInt32(address.Substring(2), MelsecA1EDataType.CS.FromBase);
              break;
            }
            if (address[1] == 'C' || address[1] == 'c')
            {
              operateResult.Content1 = MelsecA1EDataType.CC;
              operateResult.Content2 = Convert.ToInt32(address.Substring(2), MelsecA1EDataType.CC.FromBase);
              break;
            }
            if (address[1] != 'N' && address[1] != 'n')
              throw new Exception(StringResources.Language.NotSupportedDataType);
            operateResult.Content1 = MelsecA1EDataType.CN;
            operateResult.Content2 = Convert.ToInt32(address.Substring(2), MelsecA1EDataType.CN.FromBase);
            break;
          case 'D':
          case 'd':
            operateResult.Content1 = MelsecA1EDataType.D;
            operateResult.Content2 = Convert.ToInt32(address.Substring(1), MelsecA1EDataType.D.FromBase);
            break;
          case 'F':
          case 'f':
            operateResult.Content1 = MelsecA1EDataType.F;
            operateResult.Content2 = Convert.ToInt32(address.Substring(1), MelsecA1EDataType.F.FromBase);
            break;
          case 'M':
          case 'm':
            operateResult.Content1 = MelsecA1EDataType.M;
            operateResult.Content2 = Convert.ToInt32(address.Substring(1), MelsecA1EDataType.M.FromBase);
            break;
          case 'R':
          case 'r':
            operateResult.Content1 = MelsecA1EDataType.R;
            operateResult.Content2 = Convert.ToInt32(address.Substring(1), MelsecA1EDataType.R.FromBase);
            break;
          case 'S':
          case 's':
            operateResult.Content1 = MelsecA1EDataType.S;
            operateResult.Content2 = Convert.ToInt32(address.Substring(1), MelsecA1EDataType.S.FromBase);
            break;
          case 'T':
          case 't':
            if (address[1] == 'S' || address[1] == 's')
            {
              operateResult.Content1 = MelsecA1EDataType.TS;
              operateResult.Content2 = Convert.ToInt32(address.Substring(2), MelsecA1EDataType.TS.FromBase);
              break;
            }
            if (address[1] == 'C' || address[1] == 'c')
            {
              operateResult.Content1 = MelsecA1EDataType.TC;
              operateResult.Content2 = Convert.ToInt32(address.Substring(2), MelsecA1EDataType.TC.FromBase);
              break;
            }
            if (address[1] != 'N' && address[1] != 'n')
              throw new Exception(StringResources.Language.NotSupportedDataType);
            operateResult.Content1 = MelsecA1EDataType.TN;
            operateResult.Content2 = Convert.ToInt32(address.Substring(2), MelsecA1EDataType.TN.FromBase);
            break;
          case 'W':
          case 'w':
            operateResult.Content1 = MelsecA1EDataType.W;
            operateResult.Content2 = Convert.ToInt32(address.Substring(1), MelsecA1EDataType.W.FromBase);
            break;
          case 'X':
          case 'x':
            operateResult.Content1 = MelsecA1EDataType.X;
            address = address.Substring(1);
            operateResult.Content2 = !address.StartsWith("0") ? Convert.ToInt32(address, MelsecA1EDataType.X.FromBase) : Convert.ToInt32(address, 8);
            break;
          case 'Y':
          case 'y':
            operateResult.Content1 = MelsecA1EDataType.Y;
            address = address.Substring(1);
            operateResult.Content2 = !address.StartsWith("0") ? Convert.ToInt32(address, MelsecA1EDataType.Y.FromBase) : Convert.ToInt32(address, 8);
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

    /// <summary>
    /// 从三菱地址，是否位读取进行创建读取的MC的核心报文<br />
    /// From the Mitsubishi address, whether to read the core message of the MC for creating and reading
    /// </summary>
    /// <param name="isBit">是否进行了位读取操作</param>
    /// <param name="addressData">三菱Mc协议的数据地址</param>
    /// <returns>带有成功标识的报文对象</returns>
    public static byte[] BuildReadMcCoreCommand(McAddressData addressData, bool isBit) => new byte[10]
    {
      (byte) 1,
      (byte) 4,
      isBit ? (byte) 1 : (byte) 0,
      (byte) 0,
      BitConverter.GetBytes(addressData.AddressStart)[0],
      BitConverter.GetBytes(addressData.AddressStart)[1],
      BitConverter.GetBytes(addressData.AddressStart)[2],
      addressData.McDataType.DataCode,
      (byte) ((uint) addressData.Length % 256U),
      (byte) ((uint) addressData.Length / 256U)
    };

    /// <summary>从三菱地址，是否位读取进行创建读取Ascii格式的MC的核心报文</summary>
    /// <param name="addressData">三菱Mc协议的数据地址</param>
    /// <param name="isBit">是否进行了位读取操作</param>
    /// <returns>带有成功标识的报文对象</returns>
    public static byte[] BuildAsciiReadMcCoreCommand(McAddressData addressData, bool isBit) => new byte[20]
    {
      (byte) 48,
      (byte) 52,
      (byte) 48,
      (byte) 49,
      (byte) 48,
      (byte) 48,
      (byte) 48,
      isBit ? (byte) 49 : (byte) 48,
      Encoding.ASCII.GetBytes(addressData.McDataType.AsciiCode)[0],
      Encoding.ASCII.GetBytes(addressData.McDataType.AsciiCode)[1],
      MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[0],
      MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[1],
      MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[2],
      MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[3],
      MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[4],
      MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[5],
      SoftBasic.BuildAsciiBytesFrom(addressData.Length)[0],
      SoftBasic.BuildAsciiBytesFrom(addressData.Length)[1],
      SoftBasic.BuildAsciiBytesFrom(addressData.Length)[2],
      SoftBasic.BuildAsciiBytesFrom(addressData.Length)[3]
    };

    /// <summary>以字为单位，创建数据写入的核心报文</summary>
    /// <param name="addressData">三菱Mc协议的数据地址</param>
    /// <param name="value">实际的原始数据信息</param>
    /// <returns>带有成功标识的报文对象</returns>
    public static byte[] BuildWriteWordCoreCommand(McAddressData addressData, byte[] value)
    {
      if (value == null)
        value = new byte[0];
      byte[] numArray = new byte[10 + value.Length];
      numArray[0] = (byte) 1;
      numArray[1] = (byte) 20;
      numArray[2] = (byte) 0;
      numArray[3] = (byte) 0;
      numArray[4] = BitConverter.GetBytes(addressData.AddressStart)[0];
      numArray[5] = BitConverter.GetBytes(addressData.AddressStart)[1];
      numArray[6] = BitConverter.GetBytes(addressData.AddressStart)[2];
      numArray[7] = addressData.McDataType.DataCode;
      numArray[8] = (byte) (value.Length / 2 % 256);
      numArray[9] = (byte) (value.Length / 2 / 256);
      value.CopyTo((Array) numArray, 10);
      return numArray;
    }

    /// <summary>以字为单位，创建ASCII数据写入的核心报文</summary>
    /// <param name="addressData">三菱Mc协议的数据地址</param>
    /// <param name="value">实际的原始数据信息</param>
    /// <returns>带有成功标识的报文对象</returns>
    public static byte[] BuildAsciiWriteWordCoreCommand(McAddressData addressData, byte[] value)
    {
      value = MelsecHelper.TransByteArrayToAsciiByteArray(value);
      byte[] numArray = new byte[20 + value.Length];
      numArray[0] = (byte) 49;
      numArray[1] = (byte) 52;
      numArray[2] = (byte) 48;
      numArray[3] = (byte) 49;
      numArray[4] = (byte) 48;
      numArray[5] = (byte) 48;
      numArray[6] = (byte) 48;
      numArray[7] = (byte) 48;
      numArray[8] = Encoding.ASCII.GetBytes(addressData.McDataType.AsciiCode)[0];
      numArray[9] = Encoding.ASCII.GetBytes(addressData.McDataType.AsciiCode)[1];
      numArray[10] = MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[0];
      numArray[11] = MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[1];
      numArray[12] = MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[2];
      numArray[13] = MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[3];
      numArray[14] = MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[4];
      numArray[15] = MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[5];
      numArray[16] = SoftBasic.BuildAsciiBytesFrom((ushort) (value.Length / 4))[0];
      numArray[17] = SoftBasic.BuildAsciiBytesFrom((ushort) (value.Length / 4))[1];
      numArray[18] = SoftBasic.BuildAsciiBytesFrom((ushort) (value.Length / 4))[2];
      numArray[19] = SoftBasic.BuildAsciiBytesFrom((ushort) (value.Length / 4))[3];
      value.CopyTo((Array) numArray, 20);
      return numArray;
    }

    /// <summary>以位为单位，创建数据写入的核心报文</summary>
    /// <param name="addressData">三菱Mc协议的数据地址</param>
    /// <param name="value">原始的bool数组数据</param>
    /// <returns>带有成功标识的报文对象</returns>
    public static byte[] BuildWriteBitCoreCommand(McAddressData addressData, bool[] value)
    {
      if (value == null)
        value = new bool[0];
      byte[] byteData = MelsecHelper.TransBoolArrayToByteData(value);
      byte[] numArray = new byte[10 + byteData.Length];
      numArray[0] = (byte) 1;
      numArray[1] = (byte) 20;
      numArray[2] = (byte) 1;
      numArray[3] = (byte) 0;
      numArray[4] = BitConverter.GetBytes(addressData.AddressStart)[0];
      numArray[5] = BitConverter.GetBytes(addressData.AddressStart)[1];
      numArray[6] = BitConverter.GetBytes(addressData.AddressStart)[2];
      numArray[7] = addressData.McDataType.DataCode;
      numArray[8] = (byte) (value.Length % 256);
      numArray[9] = (byte) (value.Length / 256);
      byteData.CopyTo((Array) numArray, 10);
      return numArray;
    }

    /// <summary>以位为单位，创建ASCII数据写入的核心报文</summary>
    /// <param name="addressData">三菱Mc协议的数据地址</param>
    /// <param name="value">原始的bool数组数据</param>
    /// <returns>带有成功标识的报文对象</returns>
    public static byte[] BuildAsciiWriteBitCoreCommand(McAddressData addressData, bool[] value)
    {
      if (value == null)
        value = new bool[0];
      byte[] array = ((IEnumerable<bool>) value).Select<bool, byte>((Func<bool, byte>) (m => !m ? (byte) 48 : (byte) 49)).ToArray<byte>();
      byte[] numArray = new byte[20 + array.Length];
      numArray[0] = (byte) 49;
      numArray[1] = (byte) 52;
      numArray[2] = (byte) 48;
      numArray[3] = (byte) 49;
      numArray[4] = (byte) 48;
      numArray[5] = (byte) 48;
      numArray[6] = (byte) 48;
      numArray[7] = (byte) 49;
      numArray[8] = Encoding.ASCII.GetBytes(addressData.McDataType.AsciiCode)[0];
      numArray[9] = Encoding.ASCII.GetBytes(addressData.McDataType.AsciiCode)[1];
      numArray[10] = MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[0];
      numArray[11] = MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[1];
      numArray[12] = MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[2];
      numArray[13] = MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[3];
      numArray[14] = MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[4];
      numArray[15] = MelsecHelper.BuildBytesFromAddress(addressData.AddressStart, addressData.McDataType)[5];
      numArray[16] = SoftBasic.BuildAsciiBytesFrom((ushort) value.Length)[0];
      numArray[17] = SoftBasic.BuildAsciiBytesFrom((ushort) value.Length)[1];
      numArray[18] = SoftBasic.BuildAsciiBytesFrom((ushort) value.Length)[2];
      numArray[19] = SoftBasic.BuildAsciiBytesFrom((ushort) value.Length)[3];
      array.CopyTo((Array) numArray, 20);
      return numArray;
    }

    /// <summary>从三菱扩展地址，是否位读取进行创建读取的MC的核心报文</summary>
    /// <param name="isBit">是否进行了位读取操作</param>
    /// <param name="extend">扩展指定</param>
    /// <param name="addressData">三菱Mc协议的数据地址</param>
    /// <returns>带有成功标识的报文对象</returns>
    public static byte[] BuildReadMcCoreExtendCommand(
      McAddressData addressData,
      ushort extend,
      bool isBit)
    {
      return new byte[17]
      {
        (byte) 1,
        (byte) 4,
        isBit ? (byte) 129 : (byte) 128,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        BitConverter.GetBytes(addressData.AddressStart)[0],
        BitConverter.GetBytes(addressData.AddressStart)[1],
        BitConverter.GetBytes(addressData.AddressStart)[2],
        addressData.McDataType.DataCode,
        (byte) 0,
        (byte) 0,
        BitConverter.GetBytes(extend)[0],
        BitConverter.GetBytes(extend)[1],
        (byte) 249,
        (byte) ((uint) addressData.Length % 256U),
        (byte) ((uint) addressData.Length / 256U)
      };
    }

    /// <summary>按字为单位随机读取的指令创建</summary>
    /// <param name="address">地址数组</param>
    /// <returns>指令</returns>
    public static byte[] BuildReadRandomWordCommand(McAddressData[] address)
    {
      byte[] numArray = new byte[6 + address.Length * 4];
      numArray[0] = (byte) 3;
      numArray[1] = (byte) 4;
      numArray[2] = (byte) 0;
      numArray[3] = (byte) 0;
      numArray[4] = (byte) address.Length;
      numArray[5] = (byte) 0;
      for (int index = 0; index < address.Length; ++index)
      {
        numArray[index * 4 + 6] = BitConverter.GetBytes(address[index].AddressStart)[0];
        numArray[index * 4 + 7] = BitConverter.GetBytes(address[index].AddressStart)[1];
        numArray[index * 4 + 8] = BitConverter.GetBytes(address[index].AddressStart)[2];
        numArray[index * 4 + 9] = address[index].McDataType.DataCode;
      }
      return numArray;
    }

    /// <summary>随机读取的指令创建</summary>
    /// <param name="address">地址数组</param>
    /// <returns>指令</returns>
    public static byte[] BuildReadRandomCommand(McAddressData[] address)
    {
      byte[] numArray = new byte[6 + address.Length * 6];
      numArray[0] = (byte) 6;
      numArray[1] = (byte) 4;
      numArray[2] = (byte) 0;
      numArray[3] = (byte) 0;
      numArray[4] = (byte) address.Length;
      numArray[5] = (byte) 0;
      for (int index = 0; index < address.Length; ++index)
      {
        numArray[index * 6 + 6] = BitConverter.GetBytes(address[index].AddressStart)[0];
        numArray[index * 6 + 7] = BitConverter.GetBytes(address[index].AddressStart)[1];
        numArray[index * 6 + 8] = BitConverter.GetBytes(address[index].AddressStart)[2];
        numArray[index * 6 + 9] = address[index].McDataType.DataCode;
        numArray[index * 6 + 10] = (byte) ((uint) address[index].Length % 256U);
        numArray[index * 6 + 11] = (byte) ((uint) address[index].Length / 256U);
      }
      return numArray;
    }

    /// <summary>按字为单位随机读取的指令创建</summary>
    /// <param name="address">地址数组</param>
    /// <returns>指令</returns>
    public static byte[] BuildAsciiReadRandomWordCommand(McAddressData[] address)
    {
      byte[] numArray = new byte[12 + address.Length * 8];
      numArray[0] = (byte) 48;
      numArray[1] = (byte) 52;
      numArray[2] = (byte) 48;
      numArray[3] = (byte) 51;
      numArray[4] = (byte) 48;
      numArray[5] = (byte) 48;
      numArray[6] = (byte) 48;
      numArray[7] = (byte) 48;
      numArray[8] = SoftBasic.BuildAsciiBytesFrom((byte) address.Length)[0];
      numArray[9] = SoftBasic.BuildAsciiBytesFrom((byte) address.Length)[1];
      numArray[10] = (byte) 48;
      numArray[11] = (byte) 48;
      for (int index = 0; index < address.Length; ++index)
      {
        numArray[index * 8 + 12] = Encoding.ASCII.GetBytes(address[index].McDataType.AsciiCode)[0];
        numArray[index * 8 + 13] = Encoding.ASCII.GetBytes(address[index].McDataType.AsciiCode)[1];
        numArray[index * 8 + 14] = MelsecHelper.BuildBytesFromAddress(address[index].AddressStart, address[index].McDataType)[0];
        numArray[index * 8 + 15] = MelsecHelper.BuildBytesFromAddress(address[index].AddressStart, address[index].McDataType)[1];
        numArray[index * 8 + 16] = MelsecHelper.BuildBytesFromAddress(address[index].AddressStart, address[index].McDataType)[2];
        numArray[index * 8 + 17] = MelsecHelper.BuildBytesFromAddress(address[index].AddressStart, address[index].McDataType)[3];
        numArray[index * 8 + 18] = MelsecHelper.BuildBytesFromAddress(address[index].AddressStart, address[index].McDataType)[4];
        numArray[index * 8 + 19] = MelsecHelper.BuildBytesFromAddress(address[index].AddressStart, address[index].McDataType)[5];
      }
      return numArray;
    }

    /// <summary>随机读取的指令创建</summary>
    /// <param name="address">地址数组</param>
    /// <returns>指令</returns>
    public static byte[] BuildAsciiReadRandomCommand(McAddressData[] address)
    {
      byte[] numArray = new byte[12 + address.Length * 12];
      numArray[0] = (byte) 48;
      numArray[1] = (byte) 52;
      numArray[2] = (byte) 48;
      numArray[3] = (byte) 54;
      numArray[4] = (byte) 48;
      numArray[5] = (byte) 48;
      numArray[6] = (byte) 48;
      numArray[7] = (byte) 48;
      numArray[8] = SoftBasic.BuildAsciiBytesFrom((byte) address.Length)[0];
      numArray[9] = SoftBasic.BuildAsciiBytesFrom((byte) address.Length)[1];
      numArray[10] = (byte) 48;
      numArray[11] = (byte) 48;
      for (int index = 0; index < address.Length; ++index)
      {
        numArray[index * 12 + 12] = Encoding.ASCII.GetBytes(address[index].McDataType.AsciiCode)[0];
        numArray[index * 12 + 13] = Encoding.ASCII.GetBytes(address[index].McDataType.AsciiCode)[1];
        numArray[index * 12 + 14] = MelsecHelper.BuildBytesFromAddress(address[index].AddressStart, address[index].McDataType)[0];
        numArray[index * 12 + 15] = MelsecHelper.BuildBytesFromAddress(address[index].AddressStart, address[index].McDataType)[1];
        numArray[index * 12 + 16] = MelsecHelper.BuildBytesFromAddress(address[index].AddressStart, address[index].McDataType)[2];
        numArray[index * 12 + 17] = MelsecHelper.BuildBytesFromAddress(address[index].AddressStart, address[index].McDataType)[3];
        numArray[index * 12 + 18] = MelsecHelper.BuildBytesFromAddress(address[index].AddressStart, address[index].McDataType)[4];
        numArray[index * 12 + 19] = MelsecHelper.BuildBytesFromAddress(address[index].AddressStart, address[index].McDataType)[5];
        numArray[index * 12 + 20] = SoftBasic.BuildAsciiBytesFrom(address[index].Length)[0];
        numArray[index * 12 + 21] = SoftBasic.BuildAsciiBytesFrom(address[index].Length)[1];
        numArray[index * 12 + 22] = SoftBasic.BuildAsciiBytesFrom(address[index].Length)[2];
        numArray[index * 12 + 23] = SoftBasic.BuildAsciiBytesFrom(address[index].Length)[3];
      }
      return numArray;
    }

    /// <summary>创建批量读取标签的报文数据信息</summary>
    /// <param name="tags">标签名</param>
    /// <param name="lengths">长度信息</param>
    /// <returns>报文名称</returns>
    public static byte[] BuildReadTag(string[] tags, ushort[] lengths)
    {
      if (tags.Length != lengths.Length)
        throw new Exception(StringResources.Language.TwoParametersLengthIsNotSame);
      MemoryStream memoryStream = new MemoryStream();
      memoryStream.WriteByte((byte) 26);
      memoryStream.WriteByte((byte) 4);
      memoryStream.WriteByte((byte) 0);
      memoryStream.WriteByte((byte) 0);
      memoryStream.WriteByte(BitConverter.GetBytes(tags.Length)[0]);
      memoryStream.WriteByte(BitConverter.GetBytes(tags.Length)[1]);
      memoryStream.WriteByte((byte) 0);
      memoryStream.WriteByte((byte) 0);
      for (int index = 0; index < tags.Length; ++index)
      {
        byte[] bytes = Encoding.Unicode.GetBytes(tags[index]);
        memoryStream.WriteByte(BitConverter.GetBytes(bytes.Length / 2)[0]);
        memoryStream.WriteByte(BitConverter.GetBytes(bytes.Length / 2)[1]);
        memoryStream.Write(bytes, 0, bytes.Length);
        memoryStream.WriteByte((byte) 1);
        memoryStream.WriteByte((byte) 0);
        memoryStream.WriteByte(BitConverter.GetBytes((int) lengths[index] * 2)[0]);
        memoryStream.WriteByte(BitConverter.GetBytes((int) lengths[index] * 2)[1]);
      }
      byte[] array = memoryStream.ToArray();
      memoryStream.Dispose();
      return array;
    }

    /// <summary>解析出标签读取的数据内容</summary>
    /// <param name="content">返回的数据信息</param>
    /// <returns>解析结果</returns>
    public static OperateResult<byte[]> ExtraTagData(byte[] content)
    {
      try
      {
        int uint16_1 = (int) BitConverter.ToUInt16(content, 0);
        int num = 2;
        List<byte> byteList = new List<byte>(20);
        for (int index = 0; index < uint16_1; ++index)
        {
          int uint16_2 = (int) BitConverter.ToUInt16(content, num + 2);
          byteList.AddRange((IEnumerable<byte>) SoftBasic.ArraySelectMiddle<byte>(content, num + 4, uint16_2));
          num += 4 + uint16_2;
        }
        return OperateResult.CreateSuccessResult<byte[]>(byteList.ToArray());
      }
      catch (Exception ex)
      {
        return new OperateResult<byte[]>(ex.Message + " Source:" + SoftBasic.ByteToHexString(content, ' '));
      }
    }

    /// <summary>读取本站缓冲寄存器的数据信息，需要指定寄存器的地址，和读取的长度</summary>
    /// <param name="address">寄存器的地址</param>
    /// <param name="length">数据长度</param>
    /// <returns>结果内容</returns>
    public static OperateResult<byte[]> BuildReadMemoryCommand(
      string address,
      ushort length)
    {
      try
      {
        uint num = uint.Parse(address);
        return OperateResult.CreateSuccessResult<byte[]>(new byte[10]
        {
          (byte) 19,
          (byte) 6,
          (byte) 0,
          (byte) 0,
          BitConverter.GetBytes(num)[0],
          BitConverter.GetBytes(num)[1],
          BitConverter.GetBytes(num)[2],
          BitConverter.GetBytes(num)[3],
          (byte) ((uint) length % 256U),
          (byte) ((uint) length / 256U)
        });
      }
      catch (Exception ex)
      {
        return new OperateResult<byte[]>(ex.Message);
      }
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecHelper.BuildReadMemoryCommand(System.String,System.UInt16)" />
    public static OperateResult<byte[]> BuildAsciiReadMemoryCommand(
      string address,
      ushort length)
    {
      try
      {
        uint num = uint.Parse(address);
        byte[] numArray = new byte[20];
        numArray[0] = (byte) 48;
        numArray[1] = (byte) 54;
        numArray[2] = (byte) 49;
        numArray[3] = (byte) 51;
        numArray[4] = (byte) 48;
        numArray[5] = (byte) 48;
        numArray[6] = (byte) 48;
        numArray[7] = (byte) 48;
        SoftBasic.BuildAsciiBytesFrom(num).CopyTo((Array) numArray, 8);
        SoftBasic.BuildAsciiBytesFrom(length).CopyTo((Array) numArray, 16);
        return OperateResult.CreateSuccessResult<byte[]>(numArray);
      }
      catch (Exception ex)
      {
        return new OperateResult<byte[]>(ex.Message);
      }
    }

    /// <summary>构建读取智能模块的命令，需要指定模块编号，起始地址，读取的长度，注意，该长度以字节为单位。</summary>
    /// <param name="module">模块编号</param>
    /// <param name="address">智能模块的起始地址</param>
    /// <param name="length">读取的字长度</param>
    /// <returns>报文的结果内容</returns>
    public static OperateResult<byte[]> BuildReadSmartModule(
      ushort module,
      string address,
      ushort length)
    {
      try
      {
        uint num = uint.Parse(address);
        return OperateResult.CreateSuccessResult<byte[]>(new byte[12]
        {
          (byte) 1,
          (byte) 6,
          (byte) 0,
          (byte) 0,
          BitConverter.GetBytes(num)[0],
          BitConverter.GetBytes(num)[1],
          BitConverter.GetBytes(num)[2],
          BitConverter.GetBytes(num)[3],
          (byte) ((uint) length % 256U),
          (byte) ((uint) length / 256U),
          BitConverter.GetBytes(module)[0],
          BitConverter.GetBytes(module)[1]
        });
      }
      catch (Exception ex)
      {
        return new OperateResult<byte[]>(ex.Message);
      }
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Melsec.MelsecHelper.BuildReadSmartModule(System.UInt16,System.String,System.UInt16)" />
    public static OperateResult<byte[]> BuildAsciiReadSmartModule(
      ushort module,
      string address,
      ushort length)
    {
      try
      {
        uint num = uint.Parse(address);
        byte[] numArray = new byte[24];
        numArray[0] = (byte) 48;
        numArray[1] = (byte) 54;
        numArray[2] = (byte) 48;
        numArray[3] = (byte) 49;
        numArray[4] = (byte) 48;
        numArray[5] = (byte) 48;
        numArray[6] = (byte) 48;
        numArray[7] = (byte) 48;
        SoftBasic.BuildAsciiBytesFrom(num).CopyTo((Array) numArray, 8);
        SoftBasic.BuildAsciiBytesFrom(length).CopyTo((Array) numArray, 16);
        SoftBasic.BuildAsciiBytesFrom(module).CopyTo((Array) numArray, 20);
        return OperateResult.CreateSuccessResult<byte[]>(numArray);
      }
      catch (Exception ex)
      {
        return new OperateResult<byte[]>(ex.Message);
      }
    }

    /// <summary>根据三菱的错误码去查找对象描述信息</summary>
    /// <param name="code">错误码</param>
    /// <returns>描述信息</returns>
    public static string GetErrorDescription(int code)
    {
      switch (code)
      {
        case 2:
          return StringResources.Language.MelsecError02;
        case 81:
          return StringResources.Language.MelsecError51;
        case 82:
          return StringResources.Language.MelsecError52;
        case 84:
          return StringResources.Language.MelsecError54;
        case 85:
          return StringResources.Language.MelsecError55;
        case 86:
          return StringResources.Language.MelsecError56;
        case 88:
          return StringResources.Language.MelsecError58;
        case 89:
          return StringResources.Language.MelsecError59;
        case 49229:
          return StringResources.Language.MelsecErrorC04D;
        case 49232:
          return StringResources.Language.MelsecErrorC050;
        case 49233:
        case 49234:
        case 49235:
        case 49236:
          return StringResources.Language.MelsecErrorC051_54;
        case 49237:
          return StringResources.Language.MelsecErrorC055;
        case 49238:
          return StringResources.Language.MelsecErrorC056;
        case 49239:
          return StringResources.Language.MelsecErrorC057;
        case 49240:
          return StringResources.Language.MelsecErrorC058;
        case 49241:
          return StringResources.Language.MelsecErrorC059;
        case 49242:
        case 49243:
          return StringResources.Language.MelsecErrorC05A_B;
        case 49244:
          return StringResources.Language.MelsecErrorC05C;
        case 49245:
          return StringResources.Language.MelsecErrorC05D;
        case 49246:
          return StringResources.Language.MelsecErrorC05E;
        case 49247:
          return StringResources.Language.MelsecErrorC05F;
        case 49248:
          return StringResources.Language.MelsecErrorC060;
        case 49249:
          return StringResources.Language.MelsecErrorC061;
        case 49250:
          return StringResources.Language.MelsecErrorC062;
        case 49264:
          return StringResources.Language.MelsecErrorC070;
        case 49266:
          return StringResources.Language.MelsecErrorC072;
        case 49268:
          return StringResources.Language.MelsecErrorC074;
        default:
          return StringResources.Language.MelsecPleaseReferToManualDocument;
      }
    }

    /// <summary>从三菱的地址中构建MC协议的6字节的ASCII格式的地址</summary>
    /// <param name="address">三菱地址</param>
    /// <param name="type">三菱的数据类型</param>
    /// <returns>6字节的ASCII格式的地址</returns>
    internal static byte[] BuildBytesFromAddress(int address, MelsecMcDataType type) => Encoding.ASCII.GetBytes(address.ToString(type.FromBase == 10 ? "D6" : "X6"));

    /// <summary>将0，1，0，1的字节数组压缩成三菱格式的字节数组来表示开关量的</summary>
    /// <param name="value">原始的数据字节</param>
    /// <returns>压缩过后的数据字节</returns>
    internal static byte[] TransBoolArrayToByteData(byte[] value) => MelsecHelper.TransBoolArrayToByteData(((IEnumerable<byte>) value).Select<byte, bool>((Func<byte, bool>) (m => m > (byte) 0)).ToArray<bool>());

    /// <summary>将bool的组压缩成三菱格式的字节数组来表示开关量的</summary>
    /// <param name="value">原始的数据字节</param>
    /// <returns>压缩过后的数据字节</returns>
    internal static byte[] TransBoolArrayToByteData(bool[] value)
    {
      int length = (value.Length + 1) / 2;
      byte[] numArray = new byte[length];
      for (int index = 0; index < length; ++index)
      {
        if (value[index * 2])
          numArray[index] += (byte) 16;
        if (index * 2 + 1 < value.Length && value[index * 2 + 1])
          ++numArray[index];
      }
      return numArray;
    }

    internal static byte[] TransByteArrayToAsciiByteArray(byte[] value)
    {
      if (value == null)
        return new byte[0];
      byte[] numArray = new byte[value.Length * 2];
      for (int index = 0; index < value.Length / 2; ++index)
        SoftBasic.BuildAsciiBytesFrom(BitConverter.ToUInt16(value, index * 2)).CopyTo((Array) numArray, 4 * index);
      return numArray;
    }

    internal static byte[] TransAsciiByteArrayToByteArray(byte[] value)
    {
      byte[] numArray = new byte[value.Length / 2];
      for (int index = 0; index < numArray.Length / 2; ++index)
        BitConverter.GetBytes(Convert.ToUInt16(Encoding.ASCII.GetString(value, index * 4, 4), 16)).CopyTo((Array) numArray, index * 2);
      return numArray;
    }

    /// <summary>计算Fx协议指令的和校验信息</summary>
    /// <param name="data">字节数据</param>
    /// <returns>校验之后的数据</returns>
    internal static byte[] FxCalculateCRC(byte[] data)
    {
      int num = 0;
      for (int index = 1; index < data.Length - 2; ++index)
        num += (int) data[index];
      return SoftBasic.BuildAsciiBytesFrom((byte) num);
    }

    /// <summary>检查指定的和校验是否是正确的</summary>
    /// <param name="data">字节数据</param>
    /// <returns>是否成功</returns>
    internal static bool CheckCRC(byte[] data)
    {
      byte[] crc = MelsecHelper.FxCalculateCRC(data);
      return (int) crc[0] == (int) data[data.Length - 2] && (int) crc[1] == (int) data[data.Length - 1];
    }
  }
}
