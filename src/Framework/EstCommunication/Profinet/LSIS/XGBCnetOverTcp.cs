// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.LSIS.XGBCnetOverTcp
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using EstCommunication.Core;
using EstCommunication.Core.Net;
using EstCommunication.Reflection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EstCommunication.Profinet.LSIS
{
  /// <summary>
  /// XGB Cnet I/F module supports Serial Port. On Tcp/ip implementation, The address can carry station number information, for example: s=2;D100
  /// </summary>
  /// <remarks>
  /// Address example likes the follow
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
  ///     <term>*</term>
  ///     <term>P</term>
  ///     <term>PX100,PB100,PW100,PD100,PL100</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>*</term>
  ///     <term>M</term>
  ///     <term>MX100,MB100,MW100,MD100,ML100</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>*</term>
  ///     <term>L</term>
  ///     <term>LX100,LB100,LW100,LD100,LL100</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>*</term>
  ///     <term>K</term>
  ///     <term>KX100,KB100,KW100,KD100,KL100</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term>*</term>
  ///     <term>F</term>
  ///     <term>FX100,FB100,FW100,FD100,FL100</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term></term>
  ///     <term>T</term>
  ///     <term>TX100,TB100,TW100,TD100,TL100</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term></term>
  ///     <term>C</term>
  ///     <term>CX100,CB100,CW100,CD100,CL100</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term></term>
  ///     <term>D</term>
  ///     <term>DX100,DB100,DW100,DD100,DL100</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term></term>
  ///     <term>S</term>
  ///     <term>SX100,SB100,SW100,SD100,SL100</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term></term>
  ///     <term>Q</term>
  ///     <term>QX100,QB100,QW100,QD100,QL100</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term></term>
  ///     <term>I</term>
  ///     <term>IX100,IB100,IW100,ID100,IL100</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term></term>
  ///     <term>N</term>
  ///     <term>NX100,NB100,NW100,ND100,NL100</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term></term>
  ///     <term>U</term>
  ///     <term>UX100,UB100,UW100,UD100,UL100</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term></term>
  ///     <term>Z</term>
  ///     <term>ZX100,ZB100,ZW100,ZD100,ZL100</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  ///   <item>
  ///     <term></term>
  ///     <term>R</term>
  ///     <term>RX100,RB100,RW100,RD100,RL100</term>
  ///     <term>10</term>
  ///     <term>√</term>
  ///     <term>√</term>
  ///     <term></term>
  ///   </item>
  /// </list>
  /// </remarks>
  public class XGBCnetOverTcp : NetworkDeviceBase
  {
    /// <summary>Instantiate a Default object</summary>
    public XGBCnetOverTcp()
    {
      this.WordLength = (ushort) 2;
      this.ByteTransform = (IByteTransform) new RegularByteTransform();
      this.SleepTime = 20;
    }

    /// <summary>Instantiate a Default object</summary>
    /// <param name="ipAddress">Ip Address</param>
    /// <param name="port">Ip port</param>
    public XGBCnetOverTcp(string ipAddress, int port)
      : this()
    {
      this.IpAddress = ipAddress;
      this.Port = port;
    }

    /// <summary>PLC Station No.</summary>
    public byte Station { get; set; } = 5;

    /// <summary>Read single byte value from plc</summary>
    /// <param name="address">Start address</param>
    /// <returns>result</returns>
    [EstMqttApi("ReadByte", "")]
    public OperateResult<byte> ReadByte(string address) => ByteTransformHelper.GetResultFromArray<byte>(this.Read(address, (ushort) 2));

    /// <summary>Write single byte value to plc</summary>
    /// <param name="address">Start address</param>
    /// <param name="value">value</param>
    /// <returns>Whether to write the successful</returns>
    [EstMqttApi("WriteByte", "")]
    public OperateResult Write(string address, byte value) => this.Write(address, new byte[1]
    {
      value
    });

    /// <summary>Read single byte value from plc</summary>
    /// <param name="address">Start address</param>
    /// <returns>read result</returns>
    public async Task<OperateResult<byte>> ReadByteAsync(string address)
    {
      OperateResult<byte[]> result = await this.ReadAsync(address, (ushort) 2);
      return ByteTransformHelper.GetResultFromArray<byte>(result);
    }

    /// <summary>Write single byte value to plc</summary>
    /// <param name="address">Start address</param>
    /// <param name="value">value</param>
    /// <returns>Whether to write the successful</returns>
    public async Task<OperateResult> WriteAsync(string address, byte value)
    {
      OperateResult operateResult = await this.WriteAsync(address, new byte[1]
      {
        value
      });
      return operateResult;
    }

    /// <inheritdoc />
    [EstMqttApi("ReadBoolArray", "")]
    public override OperateResult<bool[]> ReadBool(string address, ushort length)
    {
      OperateResult<byte[]> operateResult1 = XGBCnetOverTcp.BuildReadOneCommand((byte) EstHelper.ExtractParameter(ref address, "s", (int) this.Station), address, length);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult1);
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      return !operateResult2.IsSuccess ? OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult2) : OperateResult.CreateSuccessResult<bool[]>(SoftBasic.ByteToBoolArray(XGBCnetOverTcp.ExtractActualData(operateResult2.Content, true).Content, (int) length));
    }

    /// <summary>ReadCoil, same as ReadBool</summary>
    /// <param name="address">address, for example: MX100, PX100</param>
    /// <returns>Result</returns>
    public OperateResult<bool> ReadCoil(string address) => this.ReadBool(address);

    /// <summary>ReadCoil, same as ReadBool</summary>
    /// <param name="address">address, for example: MX100, PX100</param>
    /// <param name="length">array length</param>
    /// <returns>result</returns>
    public OperateResult<bool[]> ReadCoil(string address, ushort length) => this.ReadBool(address, length);

    /// <summary>WriteCoil</summary>
    /// <param name="address">Start Address</param>
    /// <param name="value">value for write</param>
    /// <returns>whether write is success</returns>
    public OperateResult WriteCoil(string address, bool value) => this.Write(address, value);

    /// <inheritdoc />
    [EstMqttApi("WriteBool", "")]
    public override OperateResult Write(string address, bool value) => this.Write(address, new byte[1]
    {
      value ? (byte) 1 : (byte) 0
    });

    /// <inheritdoc />
    public override async Task<OperateResult<bool[]>> ReadBoolAsync(
      string address,
      ushort length)
    {
      byte stat = (byte) EstHelper.ExtractParameter(ref address, "s", (int) this.Station);
      OperateResult<byte[]> command = XGBCnetOverTcp.BuildReadOneCommand(stat, address, length);
      if (!command.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) command);
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      return read.IsSuccess ? OperateResult.CreateSuccessResult<bool[]>(SoftBasic.ByteToBoolArray(XGBCnetOverTcp.ExtractActualData(read.Content, true).Content, (int) length)) : OperateResult.CreateFailedResult<bool[]>((OperateResult) read);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.LSIS.XGBCnetOverTcp.ReadCoil(System.String)" />
    public async Task<OperateResult<bool>> ReadCoilAsync(string address)
    {
      OperateResult<bool> operateResult = await this.ReadBoolAsync(address);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.LSIS.XGBCnetOverTcp.ReadCoil(System.String,System.UInt16)" />
    public async Task<OperateResult<bool[]>> ReadCoilAsync(
      string address,
      ushort length)
    {
      OperateResult<bool[]> operateResult = await this.ReadBoolAsync(address, length);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.LSIS.XGBCnetOverTcp.WriteCoil(System.String,System.Boolean)" />
    public async Task<OperateResult> WriteCoilAsync(string address, bool value)
    {
      OperateResult operateResult = await this.WriteAsync(address, value);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.LSIS.XGBCnetOverTcp.WriteCoil(System.String,System.Boolean)" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      bool value)
    {
      OperateResult operateResult = await this.WriteAsync(address, new byte[1]
      {
        value ? (byte) 1 : (byte) 0
      });
      return operateResult;
    }

    /// <inheritdoc />
    [EstMqttApi("ReadByteArray", "")]
    public override OperateResult<byte[]> Read(string address, ushort length)
    {
      OperateResult<byte[]> operateResult1 = XGBCnetOverTcp.BuildReadCommand((byte) EstHelper.ExtractParameter(ref address, "s", (int) this.Station), address, length);
      if (!operateResult1.IsSuccess)
        return operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      return !operateResult2.IsSuccess ? operateResult2 : XGBCnetOverTcp.ExtractActualData(operateResult2.Content, true);
    }

    /// <inheritdoc />
    [EstMqttApi("WriteByteArray", "")]
    public override OperateResult Write(string address, byte[] value)
    {
      OperateResult<byte[]> operateResult1 = XGBCnetOverTcp.BuildWriteCommand((byte) EstHelper.ExtractParameter(ref address, "s", (int) this.Station), address, value);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      return !operateResult2.IsSuccess ? (OperateResult) operateResult2 : (OperateResult) XGBCnetOverTcp.ExtractActualData(operateResult2.Content, false);
    }

    /// <inheritdoc />
    public override async Task<OperateResult<byte[]>> ReadAsync(
      string address,
      ushort length)
    {
      byte stat = (byte) EstHelper.ExtractParameter(ref address, "s", (int) this.Station);
      OperateResult<byte[]> command = XGBCnetOverTcp.BuildReadCommand(stat, address, length);
      if (!command.IsSuccess)
        return command;
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      return read.IsSuccess ? XGBCnetOverTcp.ExtractActualData(read.Content, true) : read;
    }

    /// <inheritdoc />
    public override async Task<OperateResult> WriteAsync(
      string address,
      byte[] value)
    {
      byte stat = (byte) EstHelper.ExtractParameter(ref address, "s", (int) this.Station);
      OperateResult<byte[]> command = XGBCnetOverTcp.BuildWriteCommand(stat, address, value);
      if (!command.IsSuccess)
        return (OperateResult) command;
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
      return read.IsSuccess ? (OperateResult) XGBCnetOverTcp.ExtractActualData(read.Content, false) : (OperateResult) read;
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("XGBCnetOverTcp[{0}:{1}]", (object) this.IpAddress, (object) this.Port);

    /// <summary>AnalysisAddress IX0.0.0 QX0.0.0  MW1.0  MB1.0</summary>
    /// <param name="address"></param>
    /// <param name="QI"></param>
    /// <returns></returns>
    public static int CalculateAddressStarted(string address, bool QI = false)
    {
      if (address.IndexOf('.') < 0)
        return Convert.ToInt32(address);
      string[] strArray = address.Split('.');
      return !QI ? Convert.ToInt32(strArray[0]) : Convert.ToInt32(strArray[2]);
    }

    /// <summary>NumberStyles HexNumber</summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static bool IsHex(string value)
    {
      if (string.IsNullOrEmpty(value))
        return false;
      bool flag = false;
      for (int index = 0; index < value.Length; ++index)
      {
        switch (value[index])
        {
          case 'A':
          case 'B':
          case 'C':
          case 'D':
          case 'E':
          case 'F':
          case 'a':
          case 'b':
          case 'c':
          case 'd':
          case 'e':
          case 'f':
            flag = true;
            break;
        }
      }
      return flag;
    }

    /// <summary>AnalysisAddress</summary>
    /// <param name="address">start address</param>
    /// <returns>analysis result</returns>
    public static OperateResult<string> AnalysisAddress(string address)
    {
      StringBuilder stringBuilder = new StringBuilder();
      try
      {
        stringBuilder.Append("%");
        char[] chArray = new char[15]
        {
          'P',
          'M',
          'L',
          'K',
          'F',
          'T',
          'C',
          'D',
          'S',
          'Q',
          'I',
          'N',
          'U',
          'Z',
          'R'
        };
        bool flag = false;
        for (int index = 0; index < chArray.Length; ++index)
        {
          if ((int) chArray[index] == (int) address[0])
          {
            stringBuilder.Append(chArray[index]);
            if (address[1] == 'X')
            {
              stringBuilder.Append("X");
              if (address[0] == 'I' || address[0] == 'Q')
                stringBuilder.Append(XGBCnetOverTcp.CalculateAddressStarted(address.Substring(2), true));
              else if (XGBCnetOverTcp.IsHex(address.Substring(2)))
                stringBuilder.Append(address.Substring(2));
              else
                stringBuilder.Append(XGBCnetOverTcp.CalculateAddressStarted(address.Substring(2)));
            }
            else
            {
              stringBuilder.Append("B");
              int num = 0;
              if (address[1] == 'B')
              {
                int addressStarted = XGBCnetOverTcp.CalculateAddressStarted(address.Substring(2));
                stringBuilder.Append(addressStarted == 0 ? addressStarted : (num = addressStarted * 2));
              }
              else if (address[1] == 'W')
              {
                int addressStarted = XGBCnetOverTcp.CalculateAddressStarted(address.Substring(2));
                stringBuilder.Append(addressStarted == 0 ? addressStarted : (num = addressStarted * 2));
              }
              else if (address[1] == 'D')
              {
                int addressStarted = XGBCnetOverTcp.CalculateAddressStarted(address.Substring(2));
                stringBuilder.Append(addressStarted == 0 ? addressStarted : (num = addressStarted * 4));
              }
              else if (address[1] == 'L')
              {
                int addressStarted = XGBCnetOverTcp.CalculateAddressStarted(address.Substring(2));
                stringBuilder.Append(addressStarted == 0 ? addressStarted : (num = addressStarted * 8));
              }
              else if (address[0] == 'I' || address[0] == 'Q')
                stringBuilder.Append(XGBCnetOverTcp.CalculateAddressStarted(address.Substring(1), true));
              else if (XGBCnetOverTcp.IsHex(address.Substring(1)))
                stringBuilder.Append(address.Substring(1));
              else
                stringBuilder.Append(XGBCnetOverTcp.CalculateAddressStarted(address.Substring(1)));
            }
            flag = true;
            break;
          }
        }
        if (!flag)
          throw new Exception(StringResources.Language.NotSupportedDataType);
      }
      catch (Exception ex)
      {
        return new OperateResult<string>(ex.Message);
      }
      return OperateResult.CreateSuccessResult<string>(stringBuilder.ToString());
    }

    /// <summary>reading address  Type of ReadByte</summary>
    /// <param name="station">plc station</param>
    /// <param name="address">address, for example: M100, D100, DW100</param>
    /// <param name="length">read length</param>
    /// <returns>command bytes</returns>
    public static OperateResult<byte[]> BuildReadByteCommand(
      byte station,
      string address,
      ushort length)
    {
      OperateResult<string> operateResult = XGBCnetOverTcp.AnalysisAddress(address);
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult);
      List<byte> byteList = new List<byte>();
      byteList.Add((byte) 5);
      byteList.AddRange((IEnumerable<byte>) SoftBasic.BuildAsciiBytesFrom(station));
      byteList.Add((byte) 114);
      byteList.Add((byte) 83);
      byteList.Add((byte) 66);
      byteList.AddRange((IEnumerable<byte>) SoftBasic.BuildAsciiBytesFrom((byte) operateResult.Content.Length));
      byteList.AddRange((IEnumerable<byte>) Encoding.ASCII.GetBytes(operateResult.Content));
      byteList.AddRange((IEnumerable<byte>) SoftBasic.BuildAsciiBytesFrom((byte) length));
      byteList.Add((byte) 4);
      int num = 0;
      for (int index = 0; index < byteList.Count; ++index)
        num += (int) byteList[index];
      byteList.AddRange((IEnumerable<byte>) SoftBasic.BuildAsciiBytesFrom((byte) num));
      return OperateResult.CreateSuccessResult<byte[]>(byteList.ToArray());
    }

    /// <summary>One reading address Type of ReadByte</summary>
    /// <param name="station">plc station</param>
    /// <param name="address">address, for example: MX100, PX100</param>
    /// <param name="length">read length</param>
    /// <returns></returns>
    public static OperateResult<byte[]> BuildReadOneCommand(
      byte station,
      string address,
      ushort length)
    {
      OperateResult<string> operateResult = XGBCnetOverTcp.AnalysisAddress(address);
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult);
      List<byte> byteList = new List<byte>();
      byteList.Add((byte) 5);
      byteList.AddRange((IEnumerable<byte>) SoftBasic.BuildAsciiBytesFrom(station));
      byteList.Add((byte) 114);
      byteList.Add((byte) 83);
      byteList.Add((byte) 83);
      byteList.Add((byte) 48);
      byteList.Add((byte) 49);
      byteList.AddRange((IEnumerable<byte>) SoftBasic.BuildAsciiBytesFrom((byte) operateResult.Content.Length));
      byteList.AddRange((IEnumerable<byte>) Encoding.ASCII.GetBytes(operateResult.Content));
      byteList.Add((byte) 4);
      int num = 0;
      for (int index = 0; index < byteList.Count; ++index)
        num += (int) byteList[index];
      byteList.AddRange((IEnumerable<byte>) SoftBasic.BuildAsciiBytesFrom((byte) num));
      return OperateResult.CreateSuccessResult<byte[]>(byteList.ToArray());
    }

    /// <summary>build read command.</summary>
    /// <param name="station">station</param>
    /// <param name="address">start address</param>
    /// <param name="length">address length</param>
    /// <returns> command</returns>
    public static OperateResult<byte[]> BuildReadCommand(
      byte station,
      string address,
      ushort length)
    {
      OperateResult<string> dataTypeToAddress = XGBFastEnet.GetDataTypeToAddress(address);
      if (!dataTypeToAddress.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) dataTypeToAddress);
      string content = dataTypeToAddress.Content;
      if (content == "Bit")
        return XGBCnetOverTcp.BuildReadOneCommand(station, address, length);
      return content == "Word" || content == "DWord" || (content == "LWord" || content == "Continuous") ? XGBCnetOverTcp.BuildReadByteCommand(station, address, length) : new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType);
    }

    /// <summary>write data to address  Type of ReadByte</summary>
    /// <param name="station">plc station</param>
    /// <param name="address">address, for example: M100, D100, DW100</param>
    /// <param name="value">source value</param>
    /// <returns>command bytes</returns>
    public static OperateResult<byte[]> BuildWriteByteCommand(
      byte station,
      string address,
      byte[] value)
    {
      OperateResult<string> operateResult = XGBCnetOverTcp.AnalysisAddress(address);
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult);
      List<byte> byteList = new List<byte>();
      byteList.Add((byte) 5);
      byteList.AddRange((IEnumerable<byte>) SoftBasic.BuildAsciiBytesFrom(station));
      byteList.Add((byte) 119);
      byteList.Add((byte) 83);
      byteList.Add((byte) 66);
      byteList.AddRange((IEnumerable<byte>) SoftBasic.BuildAsciiBytesFrom((byte) operateResult.Content.Length));
      byteList.AddRange((IEnumerable<byte>) Encoding.ASCII.GetBytes(operateResult.Content));
      byteList.AddRange((IEnumerable<byte>) SoftBasic.BuildAsciiBytesFrom((byte) value.Length));
      byteList.AddRange((IEnumerable<byte>) SoftBasic.BytesToAsciiBytes(value));
      byteList.Add((byte) 4);
      int num = 0;
      for (int index = 0; index < byteList.Count; ++index)
        num += (int) byteList[index];
      byteList.AddRange((IEnumerable<byte>) SoftBasic.BuildAsciiBytesFrom((byte) num));
      return OperateResult.CreateSuccessResult<byte[]>(byteList.ToArray());
    }

    /// <summary>write data to address  Type of One</summary>
    /// <param name="station">plc station</param>
    /// <param name="address">address, for example: M100, D100, DW100</param>
    /// <param name="value">source value</param>
    /// <returns>command bytes</returns>
    public static OperateResult<byte[]> BuildWriteOneCommand(
      byte station,
      string address,
      byte[] value)
    {
      OperateResult<string> operateResult = XGBCnetOverTcp.AnalysisAddress(address);
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult);
      List<byte> byteList = new List<byte>();
      byteList.Add((byte) 5);
      byteList.AddRange((IEnumerable<byte>) SoftBasic.BuildAsciiBytesFrom(station));
      byteList.Add((byte) 119);
      byteList.Add((byte) 83);
      byteList.Add((byte) 83);
      byteList.Add((byte) 48);
      byteList.Add((byte) 49);
      byteList.AddRange((IEnumerable<byte>) SoftBasic.BuildAsciiBytesFrom((byte) operateResult.Content.Length));
      byteList.AddRange((IEnumerable<byte>) Encoding.ASCII.GetBytes(operateResult.Content));
      byteList.AddRange((IEnumerable<byte>) SoftBasic.BytesToAsciiBytes(value));
      byteList.Add((byte) 4);
      int num = 0;
      for (int index = 0; index < byteList.Count; ++index)
        num += (int) byteList[index];
      byteList.AddRange((IEnumerable<byte>) SoftBasic.BuildAsciiBytesFrom((byte) num));
      return OperateResult.CreateSuccessResult<byte[]>(byteList.ToArray());
    }

    /// <summary>write data to address  Type of ReadByte</summary>
    /// <param name="station">plc station</param>
    /// <param name="address">address, for example: M100, D100, DW100</param>
    /// <param name="value">source value</param>
    /// <returns>command bytes</returns>
    public static OperateResult<byte[]> BuildWriteCommand(
      byte station,
      string address,
      byte[] value)
    {
      OperateResult<string> dataTypeToAddress = XGBFastEnet.GetDataTypeToAddress(address);
      if (!dataTypeToAddress.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) dataTypeToAddress);
      string content = dataTypeToAddress.Content;
      if (content == "Bit")
        return XGBCnetOverTcp.BuildWriteOneCommand(station, address, value);
      return content == "Word" || content == "DWord" || (content == "LWord" || content == "Continuous") ? XGBCnetOverTcp.BuildWriteByteCommand(station, address, value) : new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType);
    }

    /// <summary>Extract actual data form plc response</summary>
    /// <param name="response">response data</param>
    /// <param name="isRead">read</param>
    /// <returns>result</returns>
    public static OperateResult<byte[]> ExtractActualData(byte[] response, bool isRead)
    {
      try
      {
        if (isRead)
        {
          if (response[0] == (byte) 6)
          {
            byte[] inBytes = new byte[response.Length - 13];
            Array.Copy((Array) response, 10, (Array) inBytes, 0, inBytes.Length);
            return OperateResult.CreateSuccessResult<byte[]>(SoftBasic.AsciiBytesToBytes(inBytes));
          }
          byte[] inBytes1 = new byte[response.Length - 9];
          Array.Copy((Array) response, 6, (Array) inBytes1, 0, inBytes1.Length);
          return new OperateResult<byte[]>((int) BitConverter.ToUInt16(SoftBasic.AsciiBytesToBytes(inBytes1), 0), "Data:" + SoftBasic.ByteToHexString(response));
        }
        if (response[0] == (byte) 6)
          return OperateResult.CreateSuccessResult<byte[]>(new byte[0]);
        byte[] inBytes2 = new byte[response.Length - 9];
        Array.Copy((Array) response, 6, (Array) inBytes2, 0, inBytes2.Length);
        return new OperateResult<byte[]>((int) BitConverter.ToUInt16(SoftBasic.AsciiBytesToBytes(inBytes2), 0), "Data:" + SoftBasic.ByteToHexString(response));
      }
      catch (Exception ex)
      {
        return new OperateResult<byte[]>(ex.Message);
      }
    }
  }
}
