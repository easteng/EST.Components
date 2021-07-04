// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.FATEK.FatekProgramOverTcp
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using EstCommunication.Core;
using EstCommunication.Core.Address;
using EstCommunication.Core.Net;
using EstCommunication.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstCommunication.Profinet.FATEK
{
  /// <summary>
  /// 台湾永宏公司的编程口协议，此处是基于tcp的实现，地址信息请查阅api文档信息，地址可以携带站号信息，例如 s=2;D100<br />
  /// The programming port protocol of Taiwan Yonghong company, here is the implementation based on TCP,
  /// please refer to the API information for the address information, The address can carry station number information, such as s=2;D100
  /// </summary>
  /// <remarks>
  /// 支持位访问：M,X,Y,S,T(触点),C(触点)，字访问：RT(当前值),RC(当前值)，D，R；具体参照API文档
  /// </remarks>
  /// <example>
  /// 其所支持的地址形式如下：
  /// <list type="table">
  ///   <listheader>
  ///     <term>地址名称</term>
  ///     <term>地址代号</term>
  ///     <term>示例</term>
  ///     <term>地址进制</term>
  ///     <term>字操作</term>
  ///     <term>位操作</term>
  ///     <term>备注</term>
  ///   </listheader>
  ///   <item>
  ///     <term>内部继电器</term>
  ///     <term>M</term>
  ///     <term>M100,M200</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>输入继电器</term>
  ///     <term>X</term>
  ///     <term>X10,X20</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>输出继电器</term>
  ///     <term>Y</term>
  ///     <term>Y10,Y20</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>步进继电器</term>
  ///     <term>S</term>
  ///     <term>S100,S200</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>定时器的触点</term>
  ///     <term>T</term>
  ///     <term>T100,T200</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>定时器的当前值</term>
  ///     <term>RT</term>
  ///     <term>RT100,RT200</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>计数器的触点</term>
  ///     <term>C</term>
  ///     <term>C100,C200</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>计数器的当前</term>
  ///     <term>RC</term>
  ///     <term>RC100,RC200</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>数据寄存器</term>
  ///     <term>D</term>
  ///     <term>D1000,D2000</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>文件寄存器</term>
  ///     <term>R</term>
  ///     <term>R100,R200</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>×</term>
  ///     <term></term>
  ///   </item>
  /// </list>
  /// </example>
  public class FatekProgramOverTcp : NetworkDeviceBase
  {
    private byte station = 1;

    /// <summary>
    /// 实例化默认的构造方法<br />
    /// Instantiate the default constructor
    /// </summary>
    public FatekProgramOverTcp()
    {
      this.WordLength = (ushort) 1;
      this.ByteTransform = (IByteTransform) new RegularByteTransform();
      this.SleepTime = 20;
    }

    /// <summary>
    /// 使用指定的ip地址和端口来实例化一个对象<br />
    /// Instantiate an object with the specified IP address and port
    /// </summary>
    /// <param name="ipAddress">设备的Ip地址</param>
    /// <param name="port">设备的端口号</param>
    public FatekProgramOverTcp(string ipAddress, int port)
      : this()
    {
      this.IpAddress = ipAddress;
      this.Port = port;
    }

    /// <summary>
    /// PLC的站号信息，需要和实际的设置值一致，默认为1<br />
    /// The station number information of the PLC needs to be consistent with the actual setting value. The default is 1.
    /// </summary>
    public byte Station
    {
      get => this.station;
      set => this.station = value;
    }

    /// <summary>
    /// 批量读取PLC的字节数据，以字为单位，支持读取X,Y,M,S,D,T,C,R,RT,RC具体的地址范围需要根据PLC型号来确认，地址可以携带站号信息，例如 s=2;D100<br />
    /// Read PLC byte data in batches, in word units. Supports reading X, Y, M, S, D, T, C, R, RT, RC.
    /// The specific address range needs to be confirmed according to the PLC model, The address can carry station number information, such as s=2;D100
    /// </summary>
    /// <param name="address">地址信息</param>
    /// <param name="length">数据长度</param>
    /// <returns>读取结果信息</returns>
    [EstMqttApi("ReadByteArray", "")]
    public override OperateResult<byte[]> Read(string address, ushort length)
    {
      OperateResult<List<byte[]>> operateResult1 = FatekProgramOverTcp.BuildReadCommand(this.station, address, length, false);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult1);
      List<byte> byteList = new List<byte>();
      int[] array = SoftBasic.SplitIntegerToArray((int) length, (int) byte.MaxValue);
      for (int index = 0; index < operateResult1.Content.Count; ++index)
      {
        OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content[index]);
        if (!operateResult2.IsSuccess)
          return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult2);
        OperateResult operateResult3 = FatekProgramOverTcp.CheckResponse(operateResult2.Content);
        if (!operateResult3.IsSuccess)
          return operateResult3.ConvertFailed<byte[]>();
        byteList.AddRange((IEnumerable<byte>) FatekProgramOverTcp.ExtraResponse(operateResult2.Content, (ushort) array[index]));
      }
      return OperateResult.CreateSuccessResult<byte[]>(byteList.ToArray());
    }

    /// <summary>
    /// 批量写入PLC的数据，以字为单位，也就是说最少2个字节信息，支持X,Y,M,S,D,T,C,R,RT,RC具体的地址范围需要根据PLC型号来确认，地址可以携带站号信息，例如 s=2;D100<br />
    /// The data written to the PLC in batches, in units of words, that is, at least 2 bytes of information,
    /// supporting X, Y, M, S, D, T, C, R, RT, and RC. The specific address range needs to be based on the PLC model To confirm, The address can carry station number information, such as s=2;D100
    /// </summary>
    /// <param name="address">地址信息，举例，D100，R200，RC100，RT200</param>
    /// <param name="value">数据值</param>
    /// <returns>是否写入成功</returns>
    [EstMqttApi("WriteByteArray", "")]
    public override OperateResult Write(string address, byte[] value)
    {
      OperateResult<byte[]> operateResult1 = FatekProgramOverTcp.BuildWriteByteCommand(this.station, address, value);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return (OperateResult) operateResult2;
      OperateResult operateResult3 = FatekProgramOverTcp.CheckResponse(operateResult2.Content);
      return !operateResult3.IsSuccess ? operateResult3 : OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.FATEK.FatekProgramOverTcp.Read(System.String,System.UInt16)" />
    public override async Task<OperateResult<byte[]>> ReadAsync(
      string address,
      ushort length)
    {
      OperateResult<List<byte[]>> command = FatekProgramOverTcp.BuildReadCommand(this.station, address, length, false);
      if (!command.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) command);
      List<byte> content = new List<byte>();
      int[] splits = SoftBasic.SplitIntegerToArray((int) length, (int) byte.MaxValue);
      for (int i = 0; i < command.Content.Count; ++i)
      {
        OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content[i]);
        if (!read.IsSuccess)
          return OperateResult.CreateFailedResult<byte[]>((OperateResult) read);
        OperateResult check = FatekProgramOverTcp.CheckResponse(read.Content);
        if (!check.IsSuccess)
          return check.ConvertFailed<byte[]>();
        content.AddRange((IEnumerable<byte>) FatekProgramOverTcp.ExtraResponse(read.Content, (ushort) splits[i]));
        read = (OperateResult<byte[]>) null;
        check = (OperateResult) null;
      }
      return OperateResult.CreateSuccessResult<byte[]>(content.ToArray());
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.FATEK.FatekProgramOverTcp.Write(System.String,System.Byte[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      byte[] value)
    {
      OperateResult<byte[]> command = FatekProgramOverTcp.BuildWriteByteCommand(this.station, address, value);
      if (!command.IsSuccess)
        return (OperateResult) command;
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      if (!read.IsSuccess)
        return (OperateResult) read;
      OperateResult check = FatekProgramOverTcp.CheckResponse(read.Content);
      return check.IsSuccess ? OperateResult.CreateSuccessResult() : check;
    }

    /// <summary>
    /// 批量读取bool类型数据，支持的类型为X,Y,M,S,T,C，具体的地址范围取决于PLC的类型，地址可以携带站号信息，例如 s=2;M100<br />
    /// Read bool data in batches. The supported types are X, Y, M, S, T, C. The specific address range depends on the type of PLC,
    /// The address can carry station number information, such as s=2;M100
    /// </summary>
    /// <param name="address">地址信息，比如X10，Y17，M100</param>
    /// <param name="length">读取的长度</param>
    /// <returns>读取结果信息</returns>
    [EstMqttApi("ReadBoolArray", "")]
    public override OperateResult<bool[]> ReadBool(string address, ushort length)
    {
      OperateResult<List<byte[]>> operateResult1 = FatekProgramOverTcp.BuildReadCommand(this.station, address, length, true);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult1);
      List<bool> boolList = new List<bool>();
      int[] array = SoftBasic.SplitIntegerToArray((int) length, (int) byte.MaxValue);
      for (int index = 0; index < operateResult1.Content.Count; ++index)
      {
        OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content[index]);
        if (!operateResult2.IsSuccess)
          return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult2);
        OperateResult operateResult3 = FatekProgramOverTcp.CheckResponse(operateResult2.Content);
        if (!operateResult3.IsSuccess)
          return operateResult3.ConvertFailed<bool[]>();
        boolList.AddRange(((IEnumerable<byte>) operateResult2.Content.SelectMiddle<byte>(6, array[index])).Select<byte, bool>((Func<byte, bool>) (m => m == (byte) 49)));
      }
      return OperateResult.CreateSuccessResult<bool[]>(boolList.ToArray());
    }

    /// <summary>
    /// 批量写入bool类型的数组，支持的类型为X,Y,M,S,T,C，具体的地址范围取决于PLC的类型，地址可以携带站号信息，例如 s=2;M100<br />
    /// Write arrays of type bool in batches. The supported types are X, Y, M, S, T, C. The specific address range depends on the type of PLC,
    /// The address can carry station number information, such as s=2;M100
    /// </summary>
    /// <param name="address">PLC的地址信息</param>
    /// <param name="value">数据信息</param>
    /// <returns>是否写入成功</returns>
    [EstMqttApi("WriteBoolArray", "")]
    public override OperateResult Write(string address, bool[] value)
    {
      OperateResult<byte[]> operateResult1 = FatekProgramOverTcp.BuildWriteBoolCommand(this.station, address, value);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return (OperateResult) operateResult2;
      OperateResult operateResult3 = FatekProgramOverTcp.CheckResponse(operateResult2.Content);
      return !operateResult3.IsSuccess ? operateResult3 : OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.FATEK.FatekProgramOverTcp.ReadBool(System.String,System.UInt16)" />
    public override async Task<OperateResult<bool[]>> ReadBoolAsync(
      string address,
      ushort length)
    {
      OperateResult<List<byte[]>> command = FatekProgramOverTcp.BuildReadCommand(this.station, address, length, true);
      if (!command.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) command);
      List<bool> content = new List<bool>();
      int[] splits = SoftBasic.SplitIntegerToArray((int) length, (int) byte.MaxValue);
      for (int i = 0; i < command.Content.Count; ++i)
      {
        OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content[i]);
        if (!read.IsSuccess)
          return OperateResult.CreateFailedResult<bool[]>((OperateResult) read);
        OperateResult check = FatekProgramOverTcp.CheckResponse(read.Content);
        if (!check.IsSuccess)
          return check.ConvertFailed<bool[]>();
        content.AddRange(((IEnumerable<byte>) read.Content.SelectMiddle<byte>(6, splits[i])).Select<byte, bool>((Func<byte, bool>) (m => m == (byte) 49)));
        read = (OperateResult<byte[]>) null;
        check = (OperateResult) null;
      }
      return OperateResult.CreateSuccessResult<bool[]>(content.ToArray());
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.FATEK.FatekProgramOverTcp.Write(System.String,System.Boolean[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      bool[] value)
    {
      OperateResult<byte[]> command = FatekProgramOverTcp.BuildWriteBoolCommand(this.station, address, value);
      if (!command.IsSuccess)
        return (OperateResult) command;
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      if (!read.IsSuccess)
        return (OperateResult) read;
      OperateResult check = FatekProgramOverTcp.CheckResponse(read.Content);
      return check.IsSuccess ? OperateResult.CreateSuccessResult() : check;
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("FatekProgramOverTcp[{0}:{1}]", (object) this.IpAddress, (object) this.Port);

    /// <summary>计算指令的和校验码</summary>
    /// <param name="data">指令</param>
    /// <returns>校验之后的信息</returns>
    public static string CalculateAcc(string data)
    {
      byte[] bytes = Encoding.ASCII.GetBytes(data);
      int num = 0;
      for (int index = 0; index < bytes.Length; ++index)
        num += (int) bytes[index];
      return num.ToString("X4").Substring(2);
    }

    /// <summary>创建一条读取的指令信息，需要指定一些参数</summary>
    /// <param name="station">PLCd的站号</param>
    /// <param name="address">地址信息</param>
    /// <param name="length">数据长度</param>
    /// <param name="isBool">是否位读取</param>
    /// <returns>是否成功的结果对象</returns>
    public static OperateResult<List<byte[]>> BuildReadCommand(
      byte station,
      string address,
      ushort length,
      bool isBool)
    {
      station = (byte) EstHelper.ExtractParameter(ref address, "s", (int) station);
      OperateResult<FatekProgramAddress> from = FatekProgramAddress.ParseFrom(address, length);
      if (!from.IsSuccess)
        return from.ConvertFailed<List<byte[]>>();
      List<byte[]> numArrayList = new List<byte[]>();
      int[] array = SoftBasic.SplitIntegerToArray((int) length, (int) byte.MaxValue);
      for (int index = 0; index < array.Length; ++index)
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append('\x0002');
        stringBuilder.Append(station.ToString("X2"));
        if (isBool)
        {
          stringBuilder.Append("44");
          stringBuilder.Append(array[index].ToString("X2"));
        }
        else
        {
          stringBuilder.Append("46");
          stringBuilder.Append(array[index].ToString("X2"));
          if (from.Content.DataCode.StartsWith("X") || from.Content.DataCode.StartsWith("Y") || (from.Content.DataCode.StartsWith("M") || from.Content.DataCode.StartsWith("S")) || from.Content.DataCode.StartsWith("T") || from.Content.DataCode.StartsWith("C"))
            stringBuilder.Append("W");
        }
        stringBuilder.Append(from.Content.ToString());
        stringBuilder.Append(FatekProgramOverTcp.CalculateAcc(stringBuilder.ToString()));
        stringBuilder.Append('\x0003');
        numArrayList.Add(Encoding.ASCII.GetBytes(stringBuilder.ToString()));
        from.Content.AddressStart += array[index];
      }
      return OperateResult.CreateSuccessResult<List<byte[]>>(numArrayList);
    }

    /// <summary>提取当前的结果数据信息，针对的是字单位的方式</summary>
    /// <param name="response">PLC返回的数据信息</param>
    /// <param name="length">读取的长度内容</param>
    /// <returns>结果数组</returns>
    public static byte[] ExtraResponse(byte[] response, ushort length)
    {
      byte[] numArray = new byte[(int) length * 2];
      for (int index = 0; index < numArray.Length / 2; ++index)
        BitConverter.GetBytes(Convert.ToUInt16(Encoding.ASCII.GetString(response, index * 4 + 6, 4), 16)).CopyTo((Array) numArray, index * 2);
      return numArray;
    }

    /// <summary>创建一条别入bool数据的指令信息，需要指定一些参数</summary>
    /// <param name="station">站号</param>
    /// <param name="address">地址</param>
    /// <param name="value">数组值</param>
    /// <returns>是否创建成功</returns>
    public static OperateResult<byte[]> BuildWriteBoolCommand(
      byte station,
      string address,
      bool[] value)
    {
      station = (byte) EstHelper.ExtractParameter(ref address, "s", (int) station);
      OperateResult<FatekProgramAddress> from = FatekProgramAddress.ParseFrom(address, (ushort) 0);
      if (!from.IsSuccess)
        return from.ConvertFailed<byte[]>();
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append('\x0002');
      stringBuilder.Append(station.ToString("X2"));
      stringBuilder.Append("45");
      stringBuilder.Append(value.Length.ToString("X2"));
      stringBuilder.Append(from.Content.ToString());
      for (int index = 0; index < value.Length; ++index)
        stringBuilder.Append(value[index] ? "1" : "0");
      stringBuilder.Append(FatekProgramOverTcp.CalculateAcc(stringBuilder.ToString()));
      stringBuilder.Append('\x0003');
      return OperateResult.CreateSuccessResult<byte[]>(Encoding.ASCII.GetBytes(stringBuilder.ToString()));
    }

    /// <summary>创建一条别入byte数据的指令信息，需要指定一些参数，按照字单位</summary>
    /// <param name="station">站号</param>
    /// <param name="address">地址</param>
    /// <param name="value">数组值</param>
    /// <returns>是否创建成功</returns>
    public static OperateResult<byte[]> BuildWriteByteCommand(
      byte station,
      string address,
      byte[] value)
    {
      station = (byte) EstHelper.ExtractParameter(ref address, "s", (int) station);
      OperateResult<FatekProgramAddress> from = FatekProgramAddress.ParseFrom(address, (ushort) 0);
      if (!from.IsSuccess)
        return from.ConvertFailed<byte[]>();
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append('\x0002');
      stringBuilder.Append(station.ToString("X2"));
      stringBuilder.Append("47");
      stringBuilder.Append((value.Length / 2).ToString("X2"));
      if (from.Content.DataCode.StartsWith("X") || from.Content.DataCode.StartsWith("Y") || (from.Content.DataCode.StartsWith("M") || from.Content.DataCode.StartsWith("S")) || from.Content.DataCode.StartsWith("T") || from.Content.DataCode.StartsWith("C"))
        stringBuilder.Append("W");
      stringBuilder.Append(from.Content.ToString());
      byte[] bytes = new byte[value.Length * 2];
      for (int index = 0; index < value.Length / 2; ++index)
        SoftBasic.BuildAsciiBytesFrom(BitConverter.ToUInt16(value, index * 2)).CopyTo((Array) bytes, 4 * index);
      stringBuilder.Append(Encoding.ASCII.GetString(bytes));
      stringBuilder.Append(FatekProgramOverTcp.CalculateAcc(stringBuilder.ToString()));
      stringBuilder.Append('\x0003');
      return OperateResult.CreateSuccessResult<byte[]>(Encoding.ASCII.GetBytes(stringBuilder.ToString()));
    }

    /// <summary>检查PLC反馈的报文是否正确，如果不正确，返回错误消息</summary>
    /// <param name="content">PLC反馈的报文信息</param>
    /// <returns>反馈的报文是否正确</returns>
    public static OperateResult CheckResponse(byte[] content)
    {
      if (content[0] != (byte) 2)
        return new OperateResult((int) content[0], "Write Faild:" + SoftBasic.ByteToHexString(content, ' '));
      return content[5] != (byte) 48 ? new OperateResult((int) content[5], FatekProgramOverTcp.GetErrorDescriptionFromCode((char) content[5])) : OperateResult.CreateSuccessResult();
    }

    /// <summary>根据错误码获取到真实的文本信息</summary>
    /// <param name="code">错误码</param>
    /// <returns>错误的文本描述</returns>
    public static string GetErrorDescriptionFromCode(char code)
    {
      switch (code)
      {
        case '2':
          return StringResources.Language.FatekStatus02;
        case '3':
          return StringResources.Language.FatekStatus03;
        case '4':
          return StringResources.Language.FatekStatus04;
        case '5':
          return StringResources.Language.FatekStatus05;
        case '6':
          return StringResources.Language.FatekStatus06;
        case '7':
          return StringResources.Language.FatekStatus07;
        case '9':
          return StringResources.Language.FatekStatus09;
        case 'A':
          return StringResources.Language.FatekStatus10;
        default:
          return StringResources.Language.UnknownError;
      }
    }
  }
}
