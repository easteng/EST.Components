// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.GE.GeHelper
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using EstCommunication.Core.Address;
using System;
using System.Text;

namespace EstCommunication.Profinet.GE
{
  /// <summary>GE plc相关的辅助类对象</summary>
  public class GeHelper
  {
    /// <summary>
    /// 构建一个读取数据的报文信息，需要指定操作的数据代码，读取的参数信息<br />
    /// To construct a message information for reading data, you need to specify the data code of the operation and the parameter information to be read
    /// </summary>
    /// <param name="id">消息号</param>
    /// <param name="code">操作代码</param>
    /// <param name="data">数据参数</param>
    /// <returns>包含是否成功的报文信息</returns>
    public static OperateResult<byte[]> BuildReadCoreCommand(
      long id,
      byte code,
      byte[] data)
    {
      byte[] numArray = new byte[56];
      numArray[0] = (byte) 2;
      numArray[1] = (byte) 0;
      numArray[2] = BitConverter.GetBytes(id)[0];
      numArray[3] = BitConverter.GetBytes(id)[1];
      numArray[4] = (byte) 0;
      numArray[5] = (byte) 0;
      numArray[9] = (byte) 1;
      numArray[17] = (byte) 1;
      numArray[18] = (byte) 0;
      numArray[30] = (byte) 6;
      numArray[31] = (byte) 192;
      numArray[36] = (byte) 16;
      numArray[37] = (byte) 14;
      numArray[40] = (byte) 1;
      numArray[41] = (byte) 1;
      numArray[42] = code;
      data.CopyTo((Array) numArray, 43);
      return OperateResult.CreateSuccessResult<byte[]>(numArray);
    }

    /// <summary>
    /// 构建一个读取数据的报文命令，需要指定消息号，读取的 GE 地址信息<br />
    /// To construct a message command to read data, you need to specify the message number and read GE address information
    /// </summary>
    /// <param name="id">消息号</param>
    /// <param name="address">GE 的地址</param>
    /// <returns>包含是否成功的报文信息</returns>
    public static OperateResult<byte[]> BuildReadCommand(
      long id,
      GeSRTPAddress address)
    {
      if (address.DataCode == (byte) 10 || address.DataCode == (byte) 12 || address.DataCode == (byte) 8)
        address.Length /= (ushort) 2;
      byte[] data = new byte[5]
      {
        address.DataCode,
        BitConverter.GetBytes(address.AddressStart)[0],
        BitConverter.GetBytes(address.AddressStart)[1],
        BitConverter.GetBytes(address.Length)[0],
        BitConverter.GetBytes(address.Length)[1]
      };
      return GeHelper.BuildReadCoreCommand(id, (byte) 4, data);
    }

    /// <summary>
    /// 构建一个读取数据的报文命令，需要指定消息号，地址，长度，是否位读取，返回完整的报文信息。<br />
    /// To construct a message command to read data, you need to specify the message number,
    /// address, length, whether to read in bits, and return the complete message information.
    /// </summary>
    /// <param name="id">消息号</param>
    /// <param name="address">地址</param>
    /// <param name="length">读取的长度</param>
    /// <param name="isBit"></param>
    /// <returns>包含是否成功的报文对象</returns>
    public static OperateResult<byte[]> BuildReadCommand(
      long id,
      string address,
      ushort length,
      bool isBit)
    {
      OperateResult<GeSRTPAddress> from = GeSRTPAddress.ParseFrom(address, length, isBit);
      return !from.IsSuccess ? OperateResult.CreateFailedResult<byte[]>((OperateResult) from) : GeHelper.BuildReadCommand(id, from.Content);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.GE.GeHelper.BuildWriteCommand(System.Int64,System.String,System.Byte[])" />
    public static OperateResult<byte[]> BuildWriteCommand(
      long id,
      GeSRTPAddress address,
      byte[] value)
    {
      int length = (int) address.Length;
      if (address.DataCode == (byte) 10 || address.DataCode == (byte) 12 || address.DataCode == (byte) 8)
        length /= 2;
      byte[] numArray = new byte[56 + value.Length];
      numArray[0] = (byte) 2;
      numArray[1] = (byte) 0;
      numArray[2] = BitConverter.GetBytes(id)[0];
      numArray[3] = BitConverter.GetBytes(id)[1];
      numArray[4] = BitConverter.GetBytes(value.Length)[0];
      numArray[5] = BitConverter.GetBytes(value.Length)[1];
      numArray[9] = (byte) 2;
      numArray[17] = (byte) 2;
      numArray[18] = (byte) 0;
      numArray[30] = (byte) 9;
      numArray[31] = (byte) 128;
      numArray[36] = (byte) 16;
      numArray[37] = (byte) 14;
      numArray[40] = (byte) 1;
      numArray[41] = (byte) 1;
      numArray[42] = (byte) 2;
      numArray[48] = (byte) 1;
      numArray[49] = (byte) 1;
      numArray[50] = (byte) 7;
      numArray[51] = address.DataCode;
      numArray[52] = BitConverter.GetBytes(address.AddressStart)[0];
      numArray[53] = BitConverter.GetBytes(address.AddressStart)[1];
      numArray[54] = BitConverter.GetBytes(length)[0];
      numArray[55] = BitConverter.GetBytes(length)[1];
      value.CopyTo((Array) numArray, 56);
      return OperateResult.CreateSuccessResult<byte[]>(numArray);
    }

    /// <summary>
    /// 构建一个批量写入 byte 数组变量的报文，需要指定消息号，写入的地址，地址参照 <see cref="T:EstCommunication.Profinet.GE.GeSRTPNet" /> 说明。<br />
    /// To construct a message to be written into byte array variables in batches,
    /// you need to specify the message number and write address. For the address, refer to the description of <see cref="T:EstCommunication.Profinet.GE.GeSRTPNet" />.
    /// </summary>
    /// <param name="id">消息的序号</param>
    /// <param name="address">地址信息</param>
    /// <param name="value">byte数组的原始数据</param>
    /// <returns>包含结果信息的报文内容</returns>
    public static OperateResult<byte[]> BuildWriteCommand(
      long id,
      string address,
      byte[] value)
    {
      OperateResult<GeSRTPAddress> from = GeSRTPAddress.ParseFrom(address, (ushort) value.Length, false);
      return !from.IsSuccess ? OperateResult.CreateFailedResult<byte[]>((OperateResult) from) : GeHelper.BuildWriteCommand(id, from.Content, value);
    }

    /// <summary>
    /// 构建一个批量写入 bool 数组变量的报文，需要指定消息号，写入的地址，地址参照 <see cref="T:EstCommunication.Profinet.GE.GeSRTPNet" /> 说明。<br />
    /// To construct a message to be written into bool array variables in batches,
    /// you need to specify the message number and write address. For the address, refer to the description of <see cref="T:EstCommunication.Profinet.GE.GeSRTPNet" />.
    /// </summary>
    /// <param name="id">消息的序号</param>
    /// <param name="address">地址信息</param>
    /// <param name="value">bool数组</param>
    /// <returns>包含结果信息的报文内容</returns>
    public static OperateResult<byte[]> BuildWriteCommand(
      long id,
      string address,
      bool[] value)
    {
      OperateResult<GeSRTPAddress> from = GeSRTPAddress.ParseFrom(address, (ushort) value.Length, true);
      if (!from.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) from);
      bool[] array = new bool[from.Content.AddressStart % 8 + value.Length];
      value.CopyTo((Array) array, from.Content.AddressStart % 8);
      return GeHelper.BuildWriteCommand(id, from.Content, SoftBasic.BoolArrayToByte(array));
    }

    /// <summary>
    /// 从PLC返回的数据中，提取出实际的数据内容，最少6个字节的数据。超出实际的数据长度的部分没有任何意义。<br />
    /// From the data returned by the PLC, extract the actual data content, at least 6 bytes of data. The part beyond the actual data length has no meaning.
    /// </summary>
    /// <param name="content">PLC返回的数据信息</param>
    /// <returns>解析后的实际数据内容</returns>
    public static OperateResult<byte[]> ExtraResponseContent(byte[] content)
    {
      try
      {
        if (content[0] != (byte) 3)
          return new OperateResult<byte[]>((int) content[0], StringResources.Language.UnknownError + " Source:" + content.ToHexString(' '));
        if (content[31] == (byte) 212)
        {
          ushort uint16 = BitConverter.ToUInt16(content, 42);
          return uint16 > (ushort) 0 ? new OperateResult<byte[]>((int) uint16, StringResources.Language.UnknownError) : OperateResult.CreateSuccessResult<byte[]>(content.SelectMiddle<byte>(44, 6));
        }
        return content[31] == (byte) 148 ? OperateResult.CreateSuccessResult<byte[]>(content.RemoveBegin<byte>(56)) : new OperateResult<byte[]>("Extra Wrong:" + StringResources.Language.UnknownError + " Source:" + content.ToHexString(' '));
      }
      catch (Exception ex)
      {
        return new OperateResult<byte[]>("Extra Wrong:" + ex.Message + " Source:" + content.ToHexString(' '));
      }
    }

    /// <summary>
    /// 从实际的时间的字节数组里解析出C#格式的时间对象，这个时间可能是时区0的时间，需要自行转化本地时间。<br />
    /// Analyze the time object in C# format from the actual time byte array.
    /// This time may be the time in time zone 0, and you need to convert the local time yourself.
    /// </summary>
    /// <param name="content">字节数组</param>
    /// <returns>包含是否成功的结果对象</returns>
    public static OperateResult<DateTime> ExtraDateTime(byte[] content)
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return new OperateResult<DateTime>(StringResources.Language.InsufficientPrivileges);
      try
      {
        return OperateResult.CreateSuccessResult<DateTime>(new DateTime(int.Parse(content[5].ToString("X2")) + 2000, int.Parse(content[4].ToString("X2")), int.Parse(content[3].ToString("X2")), int.Parse(content[2].ToString("X2")), int.Parse(content[1].ToString("X2")), int.Parse(content[0].ToString("X2"))));
      }
      catch (Exception ex)
      {
        return new OperateResult<DateTime>(ex.Message + " Source:" + content.ToHexString(' '));
      }
    }

    /// <summary>
    /// 从实际的时间的字节数组里解析出PLC的程序的名称。<br />
    /// Parse the name of the PLC program from the actual time byte array
    /// </summary>
    /// <param name="content">字节数组</param>
    /// <returns>包含是否成功的结果对象</returns>
    public static OperateResult<string> ExtraProgramName(byte[] content)
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        return new OperateResult<string>(StringResources.Language.InsufficientPrivileges);
      try
      {
        return OperateResult.CreateSuccessResult<string>(Encoding.UTF8.GetString(content, 18, 16).Trim(new char[1]));
      }
      catch (Exception ex)
      {
        return new OperateResult<string>(ex.Message + " Source:" + content.ToHexString(' '));
      }
    }
  }
}
