// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.AllenBradley.AllenBradleyHelper
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EstCommunication.Profinet.AllenBradley
{
  /// <summary>AB PLC的辅助类，用来辅助生成基本的指令信息</summary>
  public class AllenBradleyHelper
  {
    /// <summary>CIP命令中的读取数据的服务</summary>
    public const byte CIP_READ_DATA = 76;
    /// <summary>CIP命令中的写数据的服务</summary>
    public const int CIP_WRITE_DATA = 77;
    /// <summary>CIP命令中的读并写的数据服务</summary>
    public const int CIP_READ_WRITE_DATA = 78;
    /// <summary>CIP命令中的读片段的数据服务</summary>
    public const int CIP_READ_FRAGMENT = 82;
    /// <summary>CIP命令中的写片段的数据服务</summary>
    public const int CIP_WRITE_FRAGMENT = 83;
    /// <summary>CIP指令中读取数据的列表</summary>
    public const byte CIP_READ_LIST = 85;
    /// <summary>CIP命令中的对数据读取服务</summary>
    public const int CIP_MULTIREAD_DATA = 4096;
    /// <summary>bool型数据，一个字节长度</summary>
    public const ushort CIP_Type_Bool = 193;
    /// <summary>byte型数据，一个字节长度，SINT</summary>
    public const ushort CIP_Type_Byte = 194;
    /// <summary>整型，两个字节长度，INT</summary>
    public const ushort CIP_Type_Word = 195;
    /// <summary>长整型，四个字节长度，DINT</summary>
    public const ushort CIP_Type_DWord = 196;
    /// <summary>特长整型，8个字节，LINT</summary>
    public const ushort CIP_Type_LInt = 197;
    /// <summary>• Unsigned 8-bit integer, USINT</summary>
    public const ushort CIP_Type_USInt = 198;
    /// <summary>Unsigned 16-bit integer, UINT</summary>
    public const ushort CIP_Type_UInt = 199;
    /// <summary>Unsigned 32-bit integer, UDINT,</summary>
    public const ushort CIP_Type_UDint = 200;
    /// <summary>Unsigned 32-bit integer, ULINT,</summary>
    public const ushort CIP_Type_ULint = 201;
    /// <summary>实数数据，四个字节长度</summary>
    public const ushort CIP_Type_Real = 202;
    /// <summary>实数数据，八个字节的长度</summary>
    public const ushort CIP_Type_Double = 203;
    /// <summary>结构体数据，不定长度</summary>
    public const ushort CIP_Type_Struct = 204;
    /// <summary>字符串数据内容</summary>
    public const ushort CIP_Type_String = 208;
    /// <summary>Bit string, 8 bits, BYTE,</summary>
    public const ushort CIP_Type_D1 = 209;
    /// <summary>Bit string, 16-bits, WORD</summary>
    public const ushort CIP_Type_D2 = 210;
    /// <summary>Bit string, 32 bits, DWORD</summary>
    public const ushort CIP_Type_D3 = 211;
    /// <summary>二进制数据内容</summary>
    public const ushort CIP_Type_BitArray = 211;

    private static byte[] BuildRequestPathCommand(string address, bool isConnectedAddress = false)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        string[] strArray = address.Split(new char[1]{ '.' }, StringSplitOptions.RemoveEmptyEntries);
        for (int index = 0; index < strArray.Length; ++index)
        {
          string str1 = string.Empty;
          int length = strArray[index].IndexOf('[');
          int num = strArray[index].IndexOf(']');
          if (length > 0 && num > 0 && num > length)
          {
            str1 = strArray[index].Substring(length + 1, num - length - 1);
            strArray[index] = strArray[index].Substring(0, length);
          }
          memoryStream.WriteByte((byte) 145);
          byte[] bytes = Encoding.UTF8.GetBytes(strArray[index]);
          memoryStream.WriteByte((byte) bytes.Length);
          memoryStream.Write(bytes, 0, bytes.Length);
          if (bytes.Length % 2 == 1)
            memoryStream.WriteByte((byte) 0);
          if (!string.IsNullOrEmpty(str1))
          {
            string str2 = str1;
            char[] separator = new char[1]{ ',' };
            foreach (string str3 in str2.Split(separator, StringSplitOptions.RemoveEmptyEntries))
            {
              int int32 = Convert.ToInt32(str3);
              if (int32 < 256 && !isConnectedAddress)
              {
                memoryStream.WriteByte((byte) 40);
                memoryStream.WriteByte((byte) int32);
              }
              else
              {
                memoryStream.WriteByte((byte) 41);
                memoryStream.WriteByte((byte) 0);
                memoryStream.WriteByte(BitConverter.GetBytes(int32)[0]);
                memoryStream.WriteByte(BitConverter.GetBytes(int32)[1]);
              }
            }
          }
        }
        return memoryStream.ToArray();
      }
    }

    /// <summary>从生成的报文里面反解出实际的数据地址，不支持结构体嵌套，仅支持数据，一维数组，不支持多维数据</summary>
    /// <param name="pathCommand">地址路径报文</param>
    /// <returns>实际的地址</returns>
    public static string ParseRequestPathCommand(byte[] pathCommand)
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < pathCommand.Length; ++index)
      {
        if (pathCommand[index] == (byte) 145)
        {
          string str = Encoding.UTF8.GetString(pathCommand, index + 2, (int) pathCommand[index + 1]).TrimEnd(new char[1]);
          stringBuilder.Append(str);
          int num = 2 + str.Length;
          if (str.Length % 2 == 1)
            ++num;
          if (pathCommand.Length > num + index)
          {
            if (pathCommand[index + num] == (byte) 40)
              stringBuilder.Append(string.Format("[{0}]", (object) pathCommand[index + num + 1]));
            else if (pathCommand[index + num] == (byte) 41)
              stringBuilder.Append(string.Format("[{0}]", (object) BitConverter.ToUInt16(pathCommand, index + num + 2)));
          }
          stringBuilder.Append(".");
        }
      }
      if (stringBuilder[stringBuilder.Length - 1] == '.')
        stringBuilder.Remove(stringBuilder.Length - 1, 1);
      return stringBuilder.ToString();
    }

    /// <summary>获取枚举PLC数据信息的指令</summary>
    /// <param name="startInstance">实例的起始地址</param>
    /// <returns>结果数据</returns>
    public static byte[] GetEnumeratorCommand(ushort startInstance) => new byte[14]
    {
      (byte) 85,
      (byte) 3,
      (byte) 32,
      (byte) 107,
      (byte) 37,
      (byte) 0,
      BitConverter.GetBytes(startInstance)[0],
      BitConverter.GetBytes(startInstance)[1],
      (byte) 2,
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 2,
      (byte) 0
    };

    /// <summary>获取获得结构体句柄的命令</summary>
    /// <param name="symbolType">包含地址的信息</param>
    /// <returns>命令数据</returns>
    public static byte[] GetStructHandleCommand(ushort symbolType)
    {
      byte[] numArray = new byte[18];
      byte[] bytes = BitConverter.GetBytes(symbolType);
      bytes[1] = (byte) ((uint) bytes[1] & 15U);
      numArray[0] = (byte) 3;
      numArray[1] = (byte) 3;
      numArray[2] = (byte) 32;
      numArray[3] = (byte) 108;
      numArray[4] = (byte) 37;
      numArray[5] = (byte) 0;
      numArray[6] = bytes[0];
      numArray[7] = bytes[1];
      numArray[8] = (byte) 4;
      numArray[9] = (byte) 0;
      numArray[10] = (byte) 4;
      numArray[11] = (byte) 0;
      numArray[12] = (byte) 5;
      numArray[13] = (byte) 0;
      numArray[14] = (byte) 2;
      numArray[15] = (byte) 0;
      numArray[16] = (byte) 1;
      numArray[17] = (byte) 0;
      return numArray;
    }

    /// <summary>获取结构体内部数据结构的方法</summary>
    /// <param name="symbolType">地址</param>
    /// <param name="structHandle">句柄</param>
    /// <returns>指令</returns>
    public static byte[] GetStructItemNameType(ushort symbolType, AbStructHandle structHandle)
    {
      byte[] numArray = new byte[14];
      ushort num = (ushort) ((int) structHandle.TemplateObjectDefinitionSize * 4 - 21);
      byte[] bytes1 = BitConverter.GetBytes(symbolType);
      bytes1[1] = (byte) ((uint) bytes1[1] & 15U);
      byte[] bytes2 = BitConverter.GetBytes(0);
      byte[] bytes3 = BitConverter.GetBytes(num);
      numArray[0] = (byte) 76;
      numArray[1] = (byte) 3;
      numArray[2] = (byte) 32;
      numArray[3] = (byte) 108;
      numArray[4] = (byte) 37;
      numArray[5] = (byte) 0;
      numArray[6] = bytes1[0];
      numArray[7] = bytes1[1];
      numArray[8] = bytes2[0];
      numArray[9] = bytes2[1];
      numArray[10] = bytes2[2];
      numArray[11] = bytes2[3];
      numArray[12] = bytes3[0];
      numArray[13] = bytes3[1];
      return numArray;
    }

    /// <summary>将CommandSpecificData的命令，打包成可发送的数据指令</summary>
    /// <param name="command">实际的命令暗号</param>
    /// <param name="session">当前会话的id</param>
    /// <param name="commandSpecificData">CommandSpecificData命令</param>
    /// <returns>最终可发送的数据命令</returns>
    public static byte[] PackRequestHeader(
      ushort command,
      uint session,
      byte[] commandSpecificData)
    {
      byte[] numArray = new byte[commandSpecificData.Length + 24];
      Array.Copy((Array) commandSpecificData, 0, (Array) numArray, 24, commandSpecificData.Length);
      BitConverter.GetBytes(command).CopyTo((Array) numArray, 0);
      BitConverter.GetBytes(session).CopyTo((Array) numArray, 4);
      BitConverter.GetBytes((ushort) commandSpecificData.Length).CopyTo((Array) numArray, 2);
      return numArray;
    }

    /// <summary>打包生成一个请求读取数据的节点信息，CIP指令信息</summary>
    /// <param name="address">地址</param>
    /// <param name="length">指代数组的长度</param>
    /// <param name="isConnectedAddress">是否是连接模式下的地址，默认为false</param>
    /// <returns>CIP的指令信息</returns>
    public static byte[] PackRequsetRead(string address, int length, bool isConnectedAddress = false)
    {
      byte[] numArray1 = new byte[1024];
      int num1 = 0;
      byte[] numArray2 = numArray1;
      int index1 = num1;
      int num2 = index1 + 1;
      numArray2[index1] = (byte) 76;
      int index2 = num2 + 1;
      byte[] numArray3 = AllenBradleyHelper.BuildRequestPathCommand(address, isConnectedAddress);
      numArray3.CopyTo((Array) numArray1, index2);
      int num3 = index2 + numArray3.Length;
      numArray1[1] = (byte) ((num3 - 2) / 2);
      byte[] numArray4 = numArray1;
      int index3 = num3;
      int num4 = index3 + 1;
      int num5 = (int) BitConverter.GetBytes(length)[0];
      numArray4[index3] = (byte) num5;
      byte[] numArray5 = numArray1;
      int index4 = num4;
      int length1 = index4 + 1;
      int num6 = (int) BitConverter.GetBytes(length)[1];
      numArray5[index4] = (byte) num6;
      byte[] numArray6 = new byte[length1];
      Array.Copy((Array) numArray1, 0, (Array) numArray6, 0, length1);
      return numArray6;
    }

    /// <summary>打包生成一个请求读取数据片段的节点信息，CIP指令信息</summary>
    /// <param name="address">节点的名称 -&gt; Tag Name</param>
    /// <param name="startIndex">起始的索引位置，以字节为单位 -&gt; The initial index position, in bytes</param>
    /// <param name="length">读取的数据长度，一次通讯总计490个字节 -&gt; Length of read data, a total of 490 bytes of communication</param>
    /// <returns>CIP的指令信息</returns>
    public static byte[] PackRequestReadSegment(string address, int startIndex, int length)
    {
      byte[] numArray1 = new byte[1024];
      int num1 = 0;
      byte[] numArray2 = numArray1;
      int index1 = num1;
      int num2 = index1 + 1;
      numArray2[index1] = (byte) 82;
      int index2 = num2 + 1;
      byte[] numArray3 = AllenBradleyHelper.BuildRequestPathCommand(address);
      numArray3.CopyTo((Array) numArray1, index2);
      int num3 = index2 + numArray3.Length;
      numArray1[1] = (byte) ((num3 - 2) / 2);
      byte[] numArray4 = numArray1;
      int index3 = num3;
      int num4 = index3 + 1;
      int num5 = (int) BitConverter.GetBytes(length)[0];
      numArray4[index3] = (byte) num5;
      byte[] numArray5 = numArray1;
      int index4 = num4;
      int num6 = index4 + 1;
      int num7 = (int) BitConverter.GetBytes(length)[1];
      numArray5[index4] = (byte) num7;
      byte[] numArray6 = numArray1;
      int index5 = num6;
      int num8 = index5 + 1;
      int num9 = (int) BitConverter.GetBytes(startIndex)[0];
      numArray6[index5] = (byte) num9;
      byte[] numArray7 = numArray1;
      int index6 = num8;
      int num10 = index6 + 1;
      int num11 = (int) BitConverter.GetBytes(startIndex)[1];
      numArray7[index6] = (byte) num11;
      byte[] numArray8 = numArray1;
      int index7 = num10;
      int num12 = index7 + 1;
      int num13 = (int) BitConverter.GetBytes(startIndex)[2];
      numArray8[index7] = (byte) num13;
      byte[] numArray9 = numArray1;
      int index8 = num12;
      int length1 = index8 + 1;
      int num14 = (int) BitConverter.GetBytes(startIndex)[3];
      numArray9[index8] = (byte) num14;
      byte[] numArray10 = new byte[length1];
      Array.Copy((Array) numArray1, 0, (Array) numArray10, 0, length1);
      return numArray10;
    }

    /// <summary>根据指定的数据和类型，生成对应的数据</summary>
    /// <param name="address">地址信息</param>
    /// <param name="typeCode">数据类型</param>
    /// <param name="value">字节值</param>
    /// <param name="length">如果节点为数组，就是数组长度</param>
    /// <param name="isConnectedAddress">是否为连接模式的地址</param>
    /// <returns>CIP的指令信息</returns>
    public static byte[] PackRequestWrite(
      string address,
      ushort typeCode,
      byte[] value,
      int length = 1,
      bool isConnectedAddress = false)
    {
      byte[] numArray1 = new byte[1024];
      int num1 = 0;
      byte[] numArray2 = numArray1;
      int index1 = num1;
      int num2 = index1 + 1;
      numArray2[index1] = (byte) 77;
      int index2 = num2 + 1;
      byte[] numArray3 = AllenBradleyHelper.BuildRequestPathCommand(address, isConnectedAddress);
      numArray3.CopyTo((Array) numArray1, index2);
      int num3 = index2 + numArray3.Length;
      numArray1[1] = (byte) ((num3 - 2) / 2);
      byte[] numArray4 = numArray1;
      int index3 = num3;
      int num4 = index3 + 1;
      int num5 = (int) BitConverter.GetBytes(typeCode)[0];
      numArray4[index3] = (byte) num5;
      byte[] numArray5 = numArray1;
      int index4 = num4;
      int num6 = index4 + 1;
      int num7 = (int) BitConverter.GetBytes(typeCode)[1];
      numArray5[index4] = (byte) num7;
      byte[] numArray6 = numArray1;
      int index5 = num6;
      int num8 = index5 + 1;
      int num9 = (int) BitConverter.GetBytes(length)[0];
      numArray6[index5] = (byte) num9;
      byte[] numArray7 = numArray1;
      int index6 = num8;
      int index7 = index6 + 1;
      int num10 = (int) BitConverter.GetBytes(length)[1];
      numArray7[index6] = (byte) num10;
      value.CopyTo((Array) numArray1, index7);
      int length1 = index7 + value.Length;
      byte[] numArray8 = new byte[length1];
      Array.Copy((Array) numArray1, 0, (Array) numArray8, 0, length1);
      return numArray8;
    }

    /// <summary>分析地址数据信息里的位索引的信息，例如a[10]  返回 a 和 10 索引，如果没有指定索引，就默认为0</summary>
    /// <param name="address">数据地址</param>
    /// <param name="arrayIndex">位索引</param>
    /// <returns>地址信息</returns>
    public static string AnalysisArrayIndex(string address, out int arrayIndex)
    {
      arrayIndex = 0;
      if (!address.EndsWith("]"))
        return address;
      int length = address.LastIndexOf('[');
      if (length < 0)
        return address;
      address = address.Remove(address.Length - 1);
      arrayIndex = int.Parse(address.Substring(length + 1));
      address = address.Substring(0, length);
      return address;
    }

    /// <summary>写入Bool数据的基本指令信息</summary>
    /// <param name="address">地址</param>
    /// <param name="value">值</param>
    /// <returns>报文信息</returns>
    public static byte[] PackRequestWrite(string address, bool value)
    {
      int arrayIndex;
      address = AllenBradleyHelper.AnalysisArrayIndex(address, out arrayIndex);
      address = address + "[" + (arrayIndex / 32).ToString() + "]";
      int num1 = 0;
      int num2 = -1;
      if (value)
        num1 = 1 << arrayIndex;
      else
        num2 = ~(1 << arrayIndex);
      byte[] numArray1 = new byte[1024];
      int num3 = 0;
      byte[] numArray2 = numArray1;
      int index1 = num3;
      int num4 = index1 + 1;
      numArray2[index1] = (byte) 78;
      int index2 = num4 + 1;
      byte[] numArray3 = AllenBradleyHelper.BuildRequestPathCommand(address);
      numArray3.CopyTo((Array) numArray1, index2);
      int num5 = index2 + numArray3.Length;
      numArray1[1] = (byte) ((num5 - 2) / 2);
      byte[] numArray4 = numArray1;
      int index3 = num5;
      int num6 = index3 + 1;
      numArray4[index3] = (byte) 4;
      byte[] numArray5 = numArray1;
      int index4 = num6;
      int index5 = index4 + 1;
      numArray5[index4] = (byte) 0;
      BitConverter.GetBytes(num1).CopyTo((Array) numArray1, index5);
      int index6 = index5 + 4;
      BitConverter.GetBytes(num2).CopyTo((Array) numArray1, index6);
      int length = index6 + 4;
      byte[] numArray6 = new byte[length];
      Array.Copy((Array) numArray1, 0, (Array) numArray6, 0, length);
      return numArray6;
    }

    /// <summary>将所有的cip指定进行打包操作。</summary>
    /// <param name="portSlot">PLC所在的面板槽号</param>
    /// <param name="cips">所有的cip打包指令信息</param>
    /// <returns>包含服务的信息</returns>
    public static byte[] PackCommandService(byte[] portSlot, params byte[][] cips)
    {
      MemoryStream memoryStream = new MemoryStream();
      memoryStream.WriteByte((byte) 178);
      memoryStream.WriteByte((byte) 0);
      memoryStream.WriteByte((byte) 0);
      memoryStream.WriteByte((byte) 0);
      memoryStream.WriteByte((byte) 82);
      memoryStream.WriteByte((byte) 2);
      memoryStream.WriteByte((byte) 32);
      memoryStream.WriteByte((byte) 6);
      memoryStream.WriteByte((byte) 36);
      memoryStream.WriteByte((byte) 1);
      memoryStream.WriteByte((byte) 10);
      memoryStream.WriteByte((byte) 240);
      memoryStream.WriteByte((byte) 0);
      memoryStream.WriteByte((byte) 0);
      int num1 = 0;
      int num2;
      if (cips.Length == 1)
      {
        memoryStream.Write(cips[0], 0, cips[0].Length);
        num2 = num1 + cips[0].Length;
      }
      else
      {
        memoryStream.WriteByte((byte) 10);
        memoryStream.WriteByte((byte) 2);
        memoryStream.WriteByte((byte) 32);
        memoryStream.WriteByte((byte) 2);
        memoryStream.WriteByte((byte) 36);
        memoryStream.WriteByte((byte) 1);
        int num3 = num1 + 8;
        memoryStream.Write(BitConverter.GetBytes((ushort) cips.Length), 0, 2);
        ushort num4 = (ushort) (2 + 2 * cips.Length);
        num2 = num3 + 2 * cips.Length;
        for (int index = 0; index < cips.Length; ++index)
        {
          memoryStream.Write(BitConverter.GetBytes(num4), 0, 2);
          num4 += (ushort) cips[index].Length;
        }
        for (int index = 0; index < cips.Length; ++index)
        {
          memoryStream.Write(cips[index], 0, cips[index].Length);
          num2 += cips[index].Length;
        }
      }
      memoryStream.WriteByte((byte) ((portSlot.Length + 1) / 2));
      memoryStream.WriteByte((byte) 0);
      memoryStream.Write(portSlot, 0, portSlot.Length);
      if (portSlot.Length % 2 == 1)
        memoryStream.WriteByte((byte) 0);
      byte[] array = memoryStream.ToArray();
      memoryStream.Dispose();
      BitConverter.GetBytes((short) num2).CopyTo((Array) array, 12);
      BitConverter.GetBytes((short) (array.Length - 4)).CopyTo((Array) array, 2);
      return array;
    }

    /// <summary>将所有的cip指定进行打包操作。</summary>
    /// <param name="portSlot">PLC所在的面板槽号</param>
    /// <param name="cips">所有的cip打包指令信息</param>
    /// <returns>包含服务的信息</returns>
    public static byte[] PackCleanCommandService(byte[] portSlot, params byte[][] cips)
    {
      MemoryStream memoryStream = new MemoryStream();
      memoryStream.WriteByte((byte) 178);
      memoryStream.WriteByte((byte) 0);
      memoryStream.WriteByte((byte) 0);
      memoryStream.WriteByte((byte) 0);
      if (cips.Length == 1)
      {
        memoryStream.Write(cips[0], 0, cips[0].Length);
      }
      else
      {
        memoryStream.WriteByte((byte) 10);
        memoryStream.WriteByte((byte) 2);
        memoryStream.WriteByte((byte) 32);
        memoryStream.WriteByte((byte) 2);
        memoryStream.WriteByte((byte) 36);
        memoryStream.WriteByte((byte) 1);
        memoryStream.Write(BitConverter.GetBytes((ushort) cips.Length), 0, 2);
        ushort num = (ushort) (2 + 2 * cips.Length);
        for (int index = 0; index < cips.Length; ++index)
        {
          memoryStream.Write(BitConverter.GetBytes(num), 0, 2);
          num += (ushort) cips[index].Length;
        }
        for (int index = 0; index < cips.Length; ++index)
          memoryStream.Write(cips[index], 0, cips[index].Length);
      }
      memoryStream.WriteByte((byte) ((portSlot.Length + 1) / 2));
      memoryStream.WriteByte((byte) 0);
      memoryStream.Write(portSlot, 0, portSlot.Length);
      if (portSlot.Length % 2 == 1)
        memoryStream.WriteByte((byte) 0);
      byte[] array = memoryStream.ToArray();
      memoryStream.Dispose();
      BitConverter.GetBytes((short) (array.Length - 4)).CopyTo((Array) array, 2);
      return array;
    }

    /// <summary>打包一个读取所有特性数据的报文信息，需要传入slot</summary>
    /// <param name="portSlot">站号信息</param>
    /// <param name="sessionHandle">会话的ID信息</param>
    /// <returns>最终发送的报文数据</returns>
    public static byte[] PackCommandGetAttributesAll(byte[] portSlot, uint sessionHandle)
    {
      byte[] commandSpecificData = AllenBradleyHelper.PackCommandSpecificData(new byte[4], AllenBradleyHelper.PackCommandService(portSlot, new byte[6]
      {
        (byte) 1,
        (byte) 2,
        (byte) 32,
        (byte) 1,
        (byte) 36,
        (byte) 1
      }));
      return AllenBradleyHelper.PackRequestHeader((ushort) 111, sessionHandle, commandSpecificData);
    }

    /// <summary>根据数据创建反馈的数据信息</summary>
    /// <param name="data">反馈的数据信息</param>
    /// <param name="isRead">是否是读取</param>
    /// <returns>数据</returns>
    public static byte[] PackCommandResponse(byte[] data, bool isRead)
    {
      if (data == null)
      {
        byte[] numArray = new byte[6];
        numArray[2] = (byte) 4;
        return numArray;
      }
      byte[][] numArray1 = new byte[2][];
      byte[] numArray2 = new byte[6];
      numArray2[0] = isRead ? (byte) 204 : (byte) 205;
      numArray1[0] = numArray2;
      numArray1[1] = data;
      return SoftBasic.SpliceArray<byte>(numArray1);
    }

    /// <summary>生成读取直接节点数据信息的内容</summary>
    /// <param name="service">cip指令内容</param>
    /// <returns>最终的指令值</returns>
    public static byte[] PackCommandSpecificData(params byte[][] service)
    {
      MemoryStream memoryStream = new MemoryStream();
      memoryStream.WriteByte((byte) 0);
      memoryStream.WriteByte((byte) 0);
      memoryStream.WriteByte((byte) 0);
      memoryStream.WriteByte((byte) 0);
      memoryStream.WriteByte((byte) 1);
      memoryStream.WriteByte((byte) 0);
      memoryStream.WriteByte(BitConverter.GetBytes(service.Length)[0]);
      memoryStream.WriteByte(BitConverter.GetBytes(service.Length)[1]);
      for (int index = 0; index < service.Length; ++index)
        memoryStream.Write(service[index], 0, service[index].Length);
      byte[] array = memoryStream.ToArray();
      memoryStream.Dispose();
      return array;
    }

    /// <summary>将所有的cip指定进行打包操作。</summary>
    /// <param name="command">指令信息</param>
    /// <returns>包含服务的信息</returns>
    public static byte[] PackCommandSingleService(byte[] command)
    {
      if (command == null)
        command = new byte[0];
      byte[] numArray = new byte[4 + command.Length];
      numArray[0] = (byte) 178;
      numArray[1] = (byte) 0;
      numArray[2] = BitConverter.GetBytes(command.Length)[0];
      numArray[3] = BitConverter.GetBytes(command.Length)[1];
      command.CopyTo((Array) numArray, 4);
      return numArray;
    }

    /// <summary>
    /// 向PLC注册会话ID的报文<br />
    /// Register a message with the PLC for the session ID
    /// </summary>
    /// <returns>报文信息 -&gt; Message information </returns>
    public static byte[] RegisterSessionHandle() => AllenBradleyHelper.PackRequestHeader((ushort) 101, 0U, new byte[4]
    {
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 0
    });

    /// <summary>
    /// 获取卸载一个已注册的会话的报文<br />
    /// Get a message to uninstall a registered session
    /// </summary>
    /// <param name="sessionHandle">当前会话的ID信息</param>
    /// <returns>字节报文信息 -&gt; BYTE message information </returns>
    public static byte[] UnRegisterSessionHandle(uint sessionHandle) => AllenBradleyHelper.PackRequestHeader((ushort) 102, sessionHandle, new byte[0]);

    /// <summary>
    /// 初步检查返回的CIP协议的报文是否正确<br />
    /// Initially check whether the returned CIP protocol message is correct
    /// </summary>
    /// <param name="response">CIP的报文信息</param>
    /// <returns>是否正确的结果信息</returns>
    public static OperateResult CheckResponse(byte[] response)
    {
      try
      {
        int int32 = BitConverter.ToInt32(response, 8);
        if (int32 == 0)
          return OperateResult.CreateSuccessResult();
        string empty = string.Empty;
        string msg;
        switch (int32)
        {
          case 1:
            msg = StringResources.Language.AllenBradleySessionStatus01;
            break;
          case 2:
            msg = StringResources.Language.AllenBradleySessionStatus02;
            break;
          case 3:
            msg = StringResources.Language.AllenBradleySessionStatus03;
            break;
          case 100:
            msg = StringResources.Language.AllenBradleySessionStatus64;
            break;
          case 101:
            msg = StringResources.Language.AllenBradleySessionStatus65;
            break;
          case 105:
            msg = StringResources.Language.AllenBradleySessionStatus69;
            break;
          default:
            msg = StringResources.Language.UnknownError;
            break;
        }
        return new OperateResult(int32, msg);
      }
      catch (Exception ex)
      {
        return new OperateResult(ex.Message);
      }
    }

    /// <summary>从PLC反馈的数据解析</summary>
    /// <param name="response">PLC的反馈数据</param>
    /// <param name="isRead">是否是返回的操作</param>
    /// <returns>带有结果标识的最终数据</returns>
    public static OperateResult<byte[], ushort, bool> ExtractActualData(
      byte[] response,
      bool isRead)
    {
      List<byte> byteList = new List<byte>();
      int num1 = 38;
      bool flag = false;
      ushort num2 = 0;
      ushort uint16_1 = BitConverter.ToUInt16(response, 38);
      if (BitConverter.ToInt32(response, 40) == 138)
      {
        int startIndex = 44;
        int uint16_2 = (int) BitConverter.ToUInt16(response, startIndex);
        for (int index1 = 0; index1 < uint16_2; ++index1)
        {
          int num3 = (int) BitConverter.ToUInt16(response, startIndex + 2 + index1 * 2) + startIndex;
          int num4 = index1 == uint16_2 - 1 ? response.Length : (int) BitConverter.ToUInt16(response, startIndex + 4 + index1 * 2) + startIndex;
          ushort uint16_3 = BitConverter.ToUInt16(response, num3 + 2);
          switch (uint16_3)
          {
            case 0:
              if (isRead)
              {
                for (int index2 = num3 + 6; index2 < num4; ++index2)
                  byteList.Add(response[index2]);
                continue;
              }
              continue;
            case 4:
              OperateResult<byte[], ushort, bool> operateResult1 = new OperateResult<byte[], ushort, bool>();
              operateResult1.ErrorCode = (int) uint16_3;
              operateResult1.Message = StringResources.Language.AllenBradley04;
              return operateResult1;
            case 5:
              OperateResult<byte[], ushort, bool> operateResult2 = new OperateResult<byte[], ushort, bool>();
              operateResult2.ErrorCode = (int) uint16_3;
              operateResult2.Message = StringResources.Language.AllenBradley05;
              return operateResult2;
            case 6:
              if (response[startIndex + 2] == (byte) 210 || response[startIndex + 2] == (byte) 204)
              {
                OperateResult<byte[], ushort, bool> operateResult3 = new OperateResult<byte[], ushort, bool>();
                operateResult3.ErrorCode = (int) uint16_3;
                operateResult3.Message = StringResources.Language.AllenBradley06;
                return operateResult3;
              }
              goto case 0;
            case 10:
              OperateResult<byte[], ushort, bool> operateResult4 = new OperateResult<byte[], ushort, bool>();
              operateResult4.ErrorCode = (int) uint16_3;
              operateResult4.Message = StringResources.Language.AllenBradley0A;
              return operateResult4;
            case 19:
              OperateResult<byte[], ushort, bool> operateResult5 = new OperateResult<byte[], ushort, bool>();
              operateResult5.ErrorCode = (int) uint16_3;
              operateResult5.Message = StringResources.Language.AllenBradley13;
              return operateResult5;
            case 28:
              OperateResult<byte[], ushort, bool> operateResult6 = new OperateResult<byte[], ushort, bool>();
              operateResult6.ErrorCode = (int) uint16_3;
              operateResult6.Message = StringResources.Language.AllenBradley1C;
              return operateResult6;
            case 30:
              OperateResult<byte[], ushort, bool> operateResult7 = new OperateResult<byte[], ushort, bool>();
              operateResult7.ErrorCode = (int) uint16_3;
              operateResult7.Message = StringResources.Language.AllenBradley1E;
              return operateResult7;
            case 38:
              OperateResult<byte[], ushort, bool> operateResult8 = new OperateResult<byte[], ushort, bool>();
              operateResult8.ErrorCode = (int) uint16_3;
              operateResult8.Message = StringResources.Language.AllenBradley26;
              return operateResult8;
            default:
              OperateResult<byte[], ushort, bool> operateResult9 = new OperateResult<byte[], ushort, bool>();
              operateResult9.ErrorCode = (int) uint16_3;
              operateResult9.Message = StringResources.Language.UnknownError;
              return operateResult9;
          }
        }
      }
      else
      {
        byte num3 = response[num1 + 4];
        byte num4 = num3;
        if (num4 <= (byte) 10)
        {
          switch (num4)
          {
            case 0:
              if (response[num1 + 2] == (byte) 205 || response[num1 + 2] == (byte) 211)
                return OperateResult.CreateSuccessResult<byte[], ushort, bool>(byteList.ToArray(), num2, flag);
              if (response[num1 + 2] == (byte) 204 || response[num1 + 2] == (byte) 210)
              {
                for (int index = num1 + 8; index < num1 + 2 + (int) uint16_1; ++index)
                  byteList.Add(response[index]);
                num2 = BitConverter.ToUInt16(response, num1 + 6);
              }
              else if (response[num1 + 2] == (byte) 213)
              {
                for (int index = num1 + 6; index < num1 + 2 + (int) uint16_1; ++index)
                  byteList.Add(response[index]);
              }
              goto label_47;
            case 1:
            case 2:
            case 3:
              break;
            case 4:
              OperateResult<byte[], ushort, bool> operateResult1 = new OperateResult<byte[], ushort, bool>();
              operateResult1.ErrorCode = (int) num3;
              operateResult1.Message = StringResources.Language.AllenBradley04;
              return operateResult1;
            case 5:
              OperateResult<byte[], ushort, bool> operateResult2 = new OperateResult<byte[], ushort, bool>();
              operateResult2.ErrorCode = (int) num3;
              operateResult2.Message = StringResources.Language.AllenBradley05;
              return operateResult2;
            case 6:
              flag = true;
              goto case 0;
            default:
              if (num4 == (byte) 10)
              {
                OperateResult<byte[], ushort, bool> operateResult3 = new OperateResult<byte[], ushort, bool>();
                operateResult3.ErrorCode = (int) num3;
                operateResult3.Message = StringResources.Language.AllenBradley0A;
                return operateResult3;
              }
              break;
          }
        }
        else if (num4 != (byte) 19)
        {
          switch ((int) num4 - 28)
          {
            case 0:
              OperateResult<byte[], ushort, bool> operateResult4 = new OperateResult<byte[], ushort, bool>();
              operateResult4.ErrorCode = (int) num3;
              operateResult4.Message = StringResources.Language.AllenBradley1C;
              return operateResult4;
            case 1:
            case 3:
              break;
            case 2:
              OperateResult<byte[], ushort, bool> operateResult5 = new OperateResult<byte[], ushort, bool>();
              operateResult5.ErrorCode = (int) num3;
              operateResult5.Message = StringResources.Language.AllenBradley1E;
              return operateResult5;
            case 4:
              OperateResult<byte[], ushort, bool> operateResult6 = new OperateResult<byte[], ushort, bool>();
              operateResult6.ErrorCode = (int) num3;
              operateResult6.Message = StringResources.Language.AllenBradley20;
              return operateResult6;
            default:
              if (num4 == (byte) 38)
              {
                OperateResult<byte[], ushort, bool> operateResult3 = new OperateResult<byte[], ushort, bool>();
                operateResult3.ErrorCode = (int) num3;
                operateResult3.Message = StringResources.Language.AllenBradley26;
                return operateResult3;
              }
              break;
          }
        }
        else
        {
          OperateResult<byte[], ushort, bool> operateResult3 = new OperateResult<byte[], ushort, bool>();
          operateResult3.ErrorCode = (int) num3;
          operateResult3.Message = StringResources.Language.AllenBradley13;
          return operateResult3;
        }
        OperateResult<byte[], ushort, bool> operateResult7 = new OperateResult<byte[], ushort, bool>();
        operateResult7.ErrorCode = (int) num3;
        operateResult7.Message = StringResources.Language.UnknownError;
        return operateResult7;
      }
label_47:
      return OperateResult.CreateSuccessResult<byte[], ushort, bool>(byteList.ToArray(), num2, flag);
    }
  }
}
