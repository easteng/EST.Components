// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.LSIS.XGBFastEnet
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using EstCommunication.Core;
using EstCommunication.Core.IMessage;
using EstCommunication.Core.Net;
using EstCommunication.Reflection;
using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;

namespace EstCommunication.Profinet.LSIS
{
  /// <summary>
  /// XGB Fast Enet I/F module supports open Ethernet. It provides network configuration that is to connect LSIS and other company PLC, PC on network
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
  public class XGBFastEnet : NetworkDeviceBase
  {
    private string CompanyID1 = "LSIS-XGT";
    private LSCpuInfo cpuInfo = LSCpuInfo.XGK;
    private byte baseNo = 0;
    private byte slotNo = 3;
    /// <summary>所有支持的地址信息</summary>
    public static string AddressTypes = "PMLKFTCDSQINUZR";

    /// <summary>Instantiate a Default object</summary>
    public XGBFastEnet()
    {
      this.WordLength = (ushort) 2;
      this.IpAddress = "127.0.0.1";
      this.Port = 2004;
      this.ByteTransform = (IByteTransform) new RegularByteTransform();
    }

    /// <summary>Instantiate a object by ipaddress and port</summary>
    /// <param name="ipAddress">the ip address of the plc</param>
    /// <param name="port">the port of the plc, default is 2004</param>
    public XGBFastEnet(string ipAddress, int port)
    {
      this.WordLength = (ushort) 2;
      this.IpAddress = ipAddress;
      this.Port = port;
      this.ByteTransform = (IByteTransform) new RegularByteTransform();
    }

    /// <summary>
    /// Instantiate a object by ipaddress, port, cpuType, slotNo
    /// </summary>
    /// <param name="CpuType">CpuType</param>
    /// <param name="ipAddress">the ip address of the plc</param>
    /// <param name="port">he port of the plc, default is 2004</param>
    /// <param name="slotNo">slot number</param>
    public XGBFastEnet(string CpuType, string ipAddress, int port, byte slotNo)
    {
      this.SetCpuType = CpuType;
      this.WordLength = (ushort) 2;
      this.IpAddress = ipAddress;
      this.Port = port;
      this.slotNo = slotNo;
      this.ByteTransform = (IByteTransform) new RegularByteTransform();
    }

    /// <inheritdoc />
    protected override INetMessage GetNewNetMessage() => (INetMessage) new LsisFastEnetMessage();

    /// <summary>set plc</summary>
    public string SetCpuType { get; set; }

    /// <summary>CPU TYPE</summary>
    public string CpuType { get; private set; }

    /// <summary>Cpu is error</summary>
    public bool CpuError { get; private set; }

    /// <summary>RUN, STOP, ERROR, DEBUG</summary>
    public LSCpuStatus LSCpuStatus { get; private set; }

    /// <summary>FEnet I/F module’s Base No.</summary>
    public byte BaseNo
    {
      get => this.baseNo;
      set => this.baseNo = value;
    }

    /// <summary>FEnet I/F module’s Slot No.</summary>
    public byte SlotNo
    {
      get => this.slotNo;
      set => this.slotNo = value;
    }

    /// <summary>
    /// 
    /// </summary>
    public LSCpuInfo CpuInfo
    {
      get => this.cpuInfo;
      set => this.cpuInfo = value;
    }

    /// <summary>
    /// 
    /// </summary>
    public string CompanyID
    {
      get => this.CompanyID1;
      set => this.CompanyID1 = value;
    }

    /// <inheritdoc />
    [EstMqttApi("ReadByteArray", "")]
    public override OperateResult<byte[]> Read(string address, ushort length)
    {
      OperateResult<byte[]> operateResult1 = XGBFastEnet.BuildReadByteCommand(address, length);
      if (!operateResult1.IsSuccess)
        return operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(this.PackCommand(operateResult1.Content));
      return !operateResult2.IsSuccess ? OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult2) : this.ExtractActualData(operateResult2.Content);
    }

    /// <inheritdoc />
    [EstMqttApi("WriteByteArray", "")]
    public override OperateResult Write(string address, byte[] value)
    {
      OperateResult<byte[]> operateResult1 = this.BuildWriteByteCommand(address, value);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(this.PackCommand(operateResult1.Content));
      return !operateResult2.IsSuccess ? (OperateResult) OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult2) : (OperateResult) this.ExtractActualData(operateResult2.Content);
    }

    /// <inheritdoc />
    public override async Task<OperateResult<byte[]>> ReadAsync(
      string address,
      ushort length)
    {
      OperateResult<byte[]> coreResult = XGBFastEnet.BuildReadByteCommand(address, length);
      if (!coreResult.IsSuccess)
        return coreResult;
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(this.PackCommand(coreResult.Content));
      return read.IsSuccess ? this.ExtractActualData(read.Content) : OperateResult.CreateFailedResult<byte[]>((OperateResult) read);
    }

    /// <inheritdoc />
    public override async Task<OperateResult> WriteAsync(
      string address,
      byte[] value)
    {
      OperateResult<byte[]> coreResult = this.BuildWriteByteCommand(address, value);
      if (!coreResult.IsSuccess)
        return (OperateResult) coreResult;
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(this.PackCommand(coreResult.Content));
      return read.IsSuccess ? (OperateResult) this.ExtractActualData(read.Content) : (OperateResult) OperateResult.CreateFailedResult<byte[]>((OperateResult) read);
    }

    /// <inheritdoc />
    [EstMqttApi("ReadBoolArray", "")]
    public override OperateResult<bool[]> ReadBool(string address, ushort length)
    {
      OperateResult<byte[]> operateResult1 = XGBFastEnet.BuildReadByteCommand(address, length);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult1);
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(this.PackCommand(operateResult1.Content));
      if (!operateResult2.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult2);
      OperateResult<byte[]> actualData = this.ExtractActualData(operateResult2.Content);
      return !actualData.IsSuccess ? OperateResult.CreateFailedResult<bool[]>((OperateResult) actualData) : OperateResult.CreateSuccessResult<bool[]>(SoftBasic.ByteToBoolArray(actualData.Content, (int) length));
    }

    /// <summary>ReadCoil</summary>
    /// <param name="address">Start address</param>
    /// <returns>Whether to read the successful</returns>
    public OperateResult<bool> ReadCoil(string address) => this.ReadBool(address);

    /// <summary>ReadCoil</summary>
    /// <param name="address">Start address</param>
    /// <param name="length">read address length</param>
    /// <returns>Whether to read the successful</returns>
    public OperateResult<bool[]> ReadCoil(string address, ushort length) => this.ReadBool(address, length);

    /// <summary>Read single byte value from plc</summary>
    /// <param name="address">Start address</param>
    /// <returns>Whether to write the successful</returns>
    [EstMqttApi("ReadByte", "")]
    public OperateResult<byte> ReadByte(string address) => ByteTransformHelper.GetResultFromArray<byte>(this.Read(address, (ushort) 1));

    /// <summary>Write single byte value to plc</summary>
    /// <param name="address">Start address</param>
    /// <param name="value">value</param>
    /// <returns>Whether to write the successful</returns>
    [EstMqttApi("WriteByte", "")]
    public OperateResult Write(string address, byte value) => this.Write(address, new byte[1]
    {
      value
    });

    /// <summary>WriteCoil</summary>
    /// <param name="address">Start address</param>
    /// <param name="value">bool value</param>
    /// <returns>Whether to write the successful</returns>
    public OperateResult WriteCoil(string address, bool value) => this.Write(address, new byte[2]
    {
      value ? (byte) 1 : (byte) 0,
      (byte) 0
    });

    /// <summary>WriteCoil</summary>
    /// <param name="address">Start address</param>
    /// <param name="value">bool value</param>
    /// <returns>Whether to write the successful</returns>
    [EstMqttApi("WriteBool", "")]
    public override OperateResult Write(string address, bool value) => this.WriteCoil(address, value);

    /// <inheritdoc />
    public override async Task<OperateResult<bool[]>> ReadBoolAsync(
      string address,
      ushort length)
    {
      OperateResult<byte[]> coreResult = XGBFastEnet.BuildReadByteCommand(address, length);
      if (!coreResult.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) coreResult);
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(this.PackCommand(coreResult.Content));
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) read);
      OperateResult<byte[]> extract = this.ExtractActualData(read.Content);
      return extract.IsSuccess ? OperateResult.CreateSuccessResult<bool[]>(SoftBasic.ByteToBoolArray(extract.Content, (int) length)) : OperateResult.CreateFailedResult<bool[]>((OperateResult) extract);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.LSIS.XGBFastEnet.ReadCoil(System.String)" />
    public async Task<OperateResult<bool>> ReadCoilAsync(string address)
    {
      OperateResult<bool> operateResult = await this.ReadBoolAsync(address);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.LSIS.XGBFastEnet.ReadCoil(System.String,System.UInt16)" />
    public async Task<OperateResult<bool[]>> ReadCoilAsync(
      string address,
      ushort length)
    {
      OperateResult<bool[]> operateResult = await this.ReadBoolAsync(address, length);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.LSIS.XGBFastEnet.ReadByte(System.String)" />
    public async Task<OperateResult<byte>> ReadByteAsync(string address)
    {
      OperateResult<byte[]> result = await this.ReadAsync(address, (ushort) 1);
      return ByteTransformHelper.GetResultFromArray<byte>(result);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.LSIS.XGBFastEnet.Write(System.String,System.Byte)" />
    public async Task<OperateResult> WriteAsync(string address, byte value)
    {
      OperateResult operateResult = await this.WriteAsync(address, new byte[1]
      {
        value
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.LSIS.XGBFastEnet.WriteCoil(System.String,System.Boolean)" />
    public async Task<OperateResult> WriteCoilAsync(string address, bool value)
    {
      OperateResult operateResult = await this.WriteAsync(address, new byte[2]
      {
        value ? (byte) 1 : (byte) 0,
        (byte) 0
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.LSIS.XGBFastEnet.Write(System.String,System.Boolean)" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      bool value)
    {
      OperateResult operateResult = await this.WriteCoilAsync(address, value);
      return operateResult;
    }

    private byte[] PackCommand(byte[] coreCommand)
    {
      byte[] InBytes = new byte[coreCommand.Length + 20];
      Encoding.ASCII.GetBytes(this.CompanyID).CopyTo((Array) InBytes, 0);
      switch (this.cpuInfo)
      {
        case LSCpuInfo.XGK:
          InBytes[12] = (byte) 160;
          break;
        case LSCpuInfo.XGI:
          InBytes[12] = (byte) 164;
          break;
        case LSCpuInfo.XGR:
          InBytes[12] = (byte) 168;
          break;
        case LSCpuInfo.XGB_MK:
          InBytes[12] = (byte) 176;
          break;
        case LSCpuInfo.XGB_IEC:
          InBytes[12] = (byte) 180;
          break;
      }
      InBytes[13] = (byte) 51;
      BitConverter.GetBytes((short) coreCommand.Length).CopyTo((Array) InBytes, 16);
      InBytes[18] = (byte) ((uint) this.baseNo * 16U + (uint) this.slotNo);
      int num = 0;
      for (int index = 0; index < 19; ++index)
        num += (int) InBytes[index];
      InBytes[19] = (byte) num;
      coreCommand.CopyTo((Array) InBytes, 20);
      SoftBasic.ByteToHexString(InBytes, ' ');
      return InBytes;
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("XGBFastEnet[{0}:{1}]", (object) this.IpAddress, (object) this.Port);

    /// <summary>
    /// 需要传入 MX100.2 的 100.2 部分，返回的是
    /// AnalysisAddress IX0.0.0 QX0.0.0  MW1.0  MB1.0
    /// </summary>
    /// <param name="address">start address</param>
    /// <param name="QI">is Q or I data</param>
    /// <returns>int address</returns>
    public static int CalculateAddressStarted(string address, bool QI = false)
    {
      if (address.IndexOf('.') < 0)
        return Convert.ToInt32(address);
      string[] strArray = address.Split('.');
      if (!QI)
        return Convert.ToInt32(strArray[0]);
      return strArray.Length >= 4 ? Convert.ToInt32(strArray[3]) : Convert.ToInt32(strArray[2]);
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
    /// <param name="IsReadWrite">is read or write operate</param>
    /// <returns>analysis result</returns>
    public static OperateResult<string> AnalysisAddress(
      string address,
      bool IsReadWrite)
    {
      StringBuilder stringBuilder = new StringBuilder();
      try
      {
        stringBuilder.Append("%");
        bool flag = false;
        if (IsReadWrite)
        {
          for (int index = 0; index < XGBFastEnet.AddressTypes.Length; ++index)
          {
            if ((int) XGBFastEnet.AddressTypes[index] == (int) address[0])
            {
              stringBuilder.Append(XGBFastEnet.AddressTypes[index]);
              if (address[1] == 'X')
              {
                stringBuilder.Append("X");
                if (address[0] == 'I' || address[0] == 'Q' || address[0] == 'U')
                  stringBuilder.Append(XGBFastEnet.CalculateAddressStarted(address.Substring(2), true));
                else if (XGBFastEnet.IsHex(address.Substring(2)))
                  stringBuilder.Append(address.Substring(2));
                else
                  stringBuilder.Append(XGBFastEnet.CalculateAddressStarted(address.Substring(2)));
              }
              else
              {
                stringBuilder.Append("B");
                int num1 = 0;
                if (address[1] == 'B')
                {
                  int num2 = address[0] != 'I' && address[0] != 'Q' && address[0] != 'U' ? XGBFastEnet.CalculateAddressStarted(address.Substring(2)) : XGBFastEnet.CalculateAddressStarted(address.Substring(2), true);
                  stringBuilder.Append(num2 == 0 ? num2 : (num1 = num2 * 2));
                }
                else if (address[1] == 'W')
                {
                  int num2 = address[0] != 'I' && address[0] != 'Q' && address[0] != 'U' ? XGBFastEnet.CalculateAddressStarted(address.Substring(2)) : XGBFastEnet.CalculateAddressStarted(address.Substring(2), true);
                  stringBuilder.Append(num2 == 0 ? num2 : (num1 = num2 * 2));
                }
                else if (address[1] == 'D')
                {
                  int addressStarted = XGBFastEnet.CalculateAddressStarted(address.Substring(2));
                  stringBuilder.Append(addressStarted == 0 ? addressStarted : (num1 = addressStarted * 4));
                }
                else if (address[1] == 'L')
                {
                  int addressStarted = XGBFastEnet.CalculateAddressStarted(address.Substring(2));
                  stringBuilder.Append(addressStarted == 0 ? addressStarted : (num1 = addressStarted * 8));
                }
                else if (address[0] == 'I' || address[0] == 'Q' || address[0] == 'U')
                  stringBuilder.Append(XGBFastEnet.CalculateAddressStarted(address.Substring(1), true));
                else if (XGBFastEnet.IsHex(address.Substring(1)))
                  stringBuilder.Append(address.Substring(1));
                else
                  stringBuilder.Append(XGBFastEnet.CalculateAddressStarted(address.Substring(1)));
              }
              flag = true;
              break;
            }
          }
        }
        else
        {
          stringBuilder.Append(address);
          flag = true;
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

    /// <summary>Get DataType to Address</summary>
    /// <param name="address">address</param>
    /// <returns>dataType</returns>
    public static OperateResult<string> GetDataTypeToAddress(string address)
    {
      string str = string.Empty;
      try
      {
        char[] chArray = new char[12]
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
          'R'
        };
        bool flag = false;
        for (int index = 0; index < chArray.Length; ++index)
        {
          if ((int) chArray[index] == (int) address[0])
          {
            switch (address[1])
            {
              case 'B':
                str = "Continuous";
                break;
              case 'D':
                str = "DWord";
                break;
              case 'L':
                str = "LWord";
                break;
              case 'W':
                str = "Word";
                break;
              case 'X':
                str = "Bit";
                break;
              default:
                str = "Continuous";
                break;
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
      return OperateResult.CreateSuccessResult<string>(str);
    }

    private static OperateResult<byte[]> BuildReadByteCommand(
      string address,
      ushort length)
    {
      OperateResult<string> operateResult = XGBFastEnet.AnalysisAddress(address, true);
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult);
      OperateResult<string> dataTypeToAddress = XGBFastEnet.GetDataTypeToAddress(address);
      if (!dataTypeToAddress.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) dataTypeToAddress);
      byte[] numArray = new byte[12 + operateResult.Content.Length];
      string content = dataTypeToAddress.Content;
      if (!(content == "Bit"))
      {
        if (content == "Word" || content == "DWord" || (content == "LWord" || content == "Continuous"))
          numArray[2] = (byte) 20;
      }
      else
        numArray[2] = (byte) 0;
      numArray[0] = (byte) 84;
      numArray[1] = (byte) 0;
      numArray[3] = (byte) 0;
      numArray[4] = (byte) 0;
      numArray[5] = (byte) 0;
      numArray[6] = (byte) 1;
      numArray[7] = (byte) 0;
      numArray[8] = (byte) operateResult.Content.Length;
      numArray[9] = (byte) 0;
      Encoding.ASCII.GetBytes(operateResult.Content).CopyTo((Array) numArray, 10);
      BitConverter.GetBytes(length).CopyTo((Array) numArray, numArray.Length - 2);
      return OperateResult.CreateSuccessResult<byte[]>(numArray);
    }

    private OperateResult<byte[]> BuildWriteByteCommand(string address, byte[] data)
    {
      string setCpuType = this.SetCpuType;
      OperateResult<string> operateResult = setCpuType == "XGK" ? XGBFastEnet.AnalysisAddress(address, true) : (setCpuType == "XGB" ? XGBFastEnet.AnalysisAddress(address, false) : XGBFastEnet.AnalysisAddress(address, true));
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult);
      OperateResult<string> dataTypeToAddress = XGBFastEnet.GetDataTypeToAddress(address);
      if (!dataTypeToAddress.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) dataTypeToAddress);
      byte[] numArray = new byte[12 + operateResult.Content.Length + data.Length];
      string content = dataTypeToAddress.Content;
      if (!(content == "Bit") && !(content == "Byte"))
      {
        if (!(content == "Word"))
        {
          if (!(content == "DWord"))
          {
            if (!(content == "LWord"))
            {
              if (content == "Continuous")
                numArray[2] = (byte) 20;
            }
            else
              numArray[2] = (byte) 4;
          }
          else
            numArray[2] = (byte) 3;
        }
        else
          numArray[2] = (byte) 2;
      }
      else
        numArray[2] = (byte) 1;
      numArray[0] = (byte) 88;
      numArray[1] = (byte) 0;
      numArray[3] = (byte) 0;
      numArray[4] = (byte) 0;
      numArray[5] = (byte) 0;
      numArray[6] = (byte) 1;
      numArray[7] = (byte) 0;
      numArray[8] = (byte) operateResult.Content.Length;
      numArray[9] = (byte) 0;
      Encoding.ASCII.GetBytes(operateResult.Content).CopyTo((Array) numArray, 10);
      BitConverter.GetBytes(data.Length).CopyTo((Array) numArray, numArray.Length - 2 - data.Length);
      data.CopyTo((Array) numArray, numArray.Length - data.Length);
      return OperateResult.CreateSuccessResult<byte[]>(numArray);
    }

    /// <summary>
    /// Returns true data content, supports read and write returns
    /// </summary>
    /// <param name="response">response data</param>
    /// <returns>real data</returns>
    public OperateResult<byte[]> ExtractActualData(byte[] response)
    {
      if (response.Length < 20)
        return new OperateResult<byte[]>("Length is less than 20:" + SoftBasic.ByteToHexString(response));
      ushort uint16_1 = BitConverter.ToUInt16(response, 10);
      BitArray bitArray = new BitArray(BitConverter.GetBytes(uint16_1));
      int num = (int) uint16_1 % 32;
      switch ((int) uint16_1 % 32)
      {
        case 1:
          this.CpuType = "XGK/R-CPUH";
          break;
        case 2:
          this.CpuType = "XGK-CPUS";
          break;
        case 4:
          this.CpuType = "XGK-CPUE";
          break;
        case 5:
          this.CpuType = "XGK/R-CPUH";
          break;
        case 6:
          this.CpuType = "XGB/XBCU";
          break;
      }
      this.CpuError = bitArray[7];
      if (bitArray[8])
        this.LSCpuStatus = LSCpuStatus.RUN;
      if (bitArray[9])
        this.LSCpuStatus = LSCpuStatus.STOP;
      if (bitArray[10])
        this.LSCpuStatus = LSCpuStatus.ERROR;
      if (bitArray[11])
        this.LSCpuStatus = LSCpuStatus.DEBUG;
      if (response.Length < 28)
        return new OperateResult<byte[]>("Length is less than 28:" + SoftBasic.ByteToHexString(response));
      if (BitConverter.ToUInt16(response, 26) > (ushort) 0)
        return new OperateResult<byte[]>((int) response[28], "Error:" + XGBFastEnet.GetErrorDesciption(response[28]));
      if (response[20] == (byte) 89)
        return OperateResult.CreateSuccessResult<byte[]>(new byte[0]);
      if (response[20] != (byte) 85)
        return new OperateResult<byte[]>(StringResources.Language.NotSupportedFunction);
      try
      {
        ushort uint16_2 = BitConverter.ToUInt16(response, 30);
        byte[] numArray = new byte[(int) uint16_2];
        Array.Copy((Array) response, 32, (Array) numArray, 0, (int) uint16_2);
        return OperateResult.CreateSuccessResult<byte[]>(numArray);
      }
      catch (Exception ex)
      {
        return new OperateResult<byte[]>(ex.Message);
      }
    }

    /// <summary>get the description of the error code meanning</summary>
    /// <param name="code">code value</param>
    /// <returns>string information</returns>
    public static string GetErrorDesciption(byte code)
    {
      switch (code)
      {
        case 0:
          return "Normal";
        case 1:
          return "Physical layer error (TX, RX unavailable)";
        case 3:
          return "There is no identifier of Function Block to receive in communication channel";
        case 4:
          return "Mismatch of data type";
        case 5:
          return "Reset is received from partner station";
        case 6:
          return "Communication instruction of partner station is not ready status";
        case 7:
          return "Device status of remote station is not desirable status";
        case 8:
          return "Access to some target is not available";
        case 9:
          return "Can’ t deal with communication instruction of partner station by too many reception";
        case 10:
          return "Time Out error";
        case 11:
          return "Structure error";
        case 12:
          return "Abort";
        case 13:
          return "Reject(local/remote)";
        case 14:
          return "Communication channel establishment error (Connect/Disconnect)";
        case 15:
          return "High speed communication and connection service error";
        case 33:
          return "Can’t find variable identifier";
        case 34:
          return "Address error";
        case 50:
          return "Response error";
        case 113:
          return "Object Access Unsupported";
        case 187:
          return "Unknown error code (communication code of other company) is received";
        default:
          return "Unknown error";
      }
    }
  }
}
