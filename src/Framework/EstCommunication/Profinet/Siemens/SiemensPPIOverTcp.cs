// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.Siemens.SiemensPPIOverTcp
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using EstCommunication.Core;
using EstCommunication.Core.Address;
using EstCommunication.Core.Net;
using EstCommunication.Reflection;
using System;
using System.Threading.Tasks;

namespace EstCommunication.Profinet.Siemens
{
  /// <inheritdoc cref="T:EstCommunication.Profinet.Siemens.SiemensPPI" />
  public class SiemensPPIOverTcp : NetworkDeviceBase
  {
    private byte station = 2;
    private object communicationLock;

    /// <inheritdoc cref="M:EstCommunication.Profinet.Siemens.SiemensPPI.#ctor" />
    public SiemensPPIOverTcp()
    {
      this.WordLength = (ushort) 2;
      this.ByteTransform = (IByteTransform) new ReverseBytesTransform();
      this.communicationLock = new object();
      this.SleepTime = 20;
    }

    /// <summary>
    /// 使用指定的ip地址和端口号来实例化对象<br />
    /// Instantiate the object with the specified IP address and port number
    /// </summary>
    /// <param name="ipAddress">Ip地址信息</param>
    /// <param name="port">端口号信息</param>
    public SiemensPPIOverTcp(string ipAddress, int port)
      : this()
    {
      this.IpAddress = ipAddress;
      this.Port = port;
    }

    /// <inheritdoc cref="P:EstCommunication.Profinet.Siemens.SiemensPPI.Station" />
    public byte Station
    {
      get => this.station;
      set => this.station = value;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Siemens.SiemensPPI.Read(System.String,System.UInt16)" />
    [EstMqttApi("ReadByteArray", "")]
    public override OperateResult<byte[]> Read(string address, ushort length)
    {
      byte parameter = (byte) EstHelper.ExtractParameter(ref address, "s", (int) this.Station);
      OperateResult<byte[]> operateResult1 = SiemensPPIOverTcp.BuildReadCommand(parameter, address, length, false);
      if (!operateResult1.IsSuccess)
        return operateResult1;
      lock (this.communicationLock)
      {
        OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
        if (!operateResult2.IsSuccess)
          return operateResult2;
        if (operateResult2.Content[0] != (byte) 229)
          return new OperateResult<byte[]>("PLC Receive Check Failed:" + SoftBasic.ByteToHexString(operateResult2.Content, ' '));
        OperateResult<byte[]> operateResult3 = this.ReadFromCoreServer(SiemensPPIOverTcp.GetExecuteConfirm(parameter));
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

    /// <inheritdoc cref="M:EstCommunication.Profinet.Siemens.SiemensPPI.ReadBool(System.String,System.UInt16)" />
    [EstMqttApi("ReadBoolArray", "")]
    public override OperateResult<bool[]> ReadBool(string address, ushort length)
    {
      byte parameter = (byte) EstHelper.ExtractParameter(ref address, "s", (int) this.Station);
      OperateResult<byte[]> operateResult1 = SiemensPPIOverTcp.BuildReadCommand(parameter, address, length, true);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult1);
      lock (this.communicationLock)
      {
        OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
        if (!operateResult2.IsSuccess)
          return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult2);
        if (operateResult2.Content[0] != (byte) 229)
          return new OperateResult<bool[]>("PLC Receive Check Failed:" + SoftBasic.ByteToHexString(operateResult2.Content, ' '));
        OperateResult<byte[]> operateResult3 = this.ReadFromCoreServer(SiemensPPIOverTcp.GetExecuteConfirm(parameter));
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

    /// <inheritdoc cref="M:EstCommunication.Profinet.Siemens.SiemensPPI.Write(System.String,System.Byte[])" />
    [EstMqttApi("WriteByteArray", "")]
    public override OperateResult Write(string address, byte[] value)
    {
      byte parameter = (byte) EstHelper.ExtractParameter(ref address, "s", (int) this.Station);
      OperateResult<byte[]> operateResult1 = SiemensPPIOverTcp.BuildWriteCommand(parameter, address, value);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      lock (this.communicationLock)
      {
        OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
        if (!operateResult2.IsSuccess)
          return (OperateResult) operateResult2;
        if (operateResult2.Content[0] != (byte) 229)
          return (OperateResult) new OperateResult<byte[]>("PLC Receive Check Failed:" + operateResult2.Content[0].ToString());
        OperateResult<byte[]> operateResult3 = this.ReadFromCoreServer(SiemensPPIOverTcp.GetExecuteConfirm(parameter));
        if (!operateResult3.IsSuccess)
          return (OperateResult) operateResult3;
        OperateResult operateResult4 = SiemensPPIOverTcp.CheckResponse(operateResult3.Content);
        return !operateResult4.IsSuccess ? operateResult4 : OperateResult.CreateSuccessResult();
      }
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Siemens.SiemensPPI.Write(System.String,System.Boolean[])" />
    [EstMqttApi("WriteBoolArray", "")]
    public override OperateResult Write(string address, bool[] value)
    {
      byte parameter = (byte) EstHelper.ExtractParameter(ref address, "s", (int) this.Station);
      OperateResult<byte[]> operateResult1 = SiemensPPIOverTcp.BuildWriteCommand(parameter, address, value);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      lock (this.communicationLock)
      {
        OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
        if (!operateResult2.IsSuccess)
          return (OperateResult) operateResult2;
        if (operateResult2.Content[0] != (byte) 229)
          return (OperateResult) new OperateResult<byte[]>("PLC Receive Check Failed:" + operateResult2.Content[0].ToString());
        OperateResult<byte[]> operateResult3 = this.ReadFromCoreServer(SiemensPPIOverTcp.GetExecuteConfirm(parameter));
        if (!operateResult3.IsSuccess)
          return (OperateResult) operateResult3;
        OperateResult operateResult4 = SiemensPPIOverTcp.CheckResponse(operateResult3.Content);
        return !operateResult4.IsSuccess ? operateResult4 : OperateResult.CreateSuccessResult();
      }
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Siemens.SiemensPPIOverTcp.Read(System.String,System.UInt16)" />
    public override async Task<OperateResult<byte[]>> ReadAsync(
      string address,
      ushort length)
    {
      byte stat = (byte) EstHelper.ExtractParameter(ref address, "s", (int) this.Station);
      OperateResult<byte[]> command = SiemensPPIOverTcp.BuildReadCommand(stat, address, length, false);
      if (!command.IsSuccess)
        return command;
      OperateResult<byte[]> read1 = await this.ReadFromCoreServerAsync(command.Content);
      if (!read1.IsSuccess)
        return read1;
      if (read1.Content[0] != (byte) 229)
        return new OperateResult<byte[]>("PLC Receive Check Failed:" + SoftBasic.ByteToHexString(read1.Content, ' '));
      OperateResult<byte[]> read2 = await this.ReadFromCoreServerAsync(SiemensPPIOverTcp.GetExecuteConfirm(stat));
      if (!read2.IsSuccess)
        return read2;
      OperateResult check = SiemensPPIOverTcp.CheckResponse(read2.Content);
      if (!check.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>(check);
      byte[] buffer = new byte[(int) length];
      if (read2.Content[21] == byte.MaxValue && read2.Content[22] == (byte) 4)
        Array.Copy((Array) read2.Content, 25, (Array) buffer, 0, (int) length);
      return OperateResult.CreateSuccessResult<byte[]>(buffer);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Siemens.SiemensPPIOverTcp.ReadBool(System.String,System.UInt16)" />
    public override async Task<OperateResult<bool[]>> ReadBoolAsync(
      string address,
      ushort length)
    {
      byte stat = (byte) EstHelper.ExtractParameter(ref address, "s", (int) this.Station);
      OperateResult<byte[]> command = SiemensPPIOverTcp.BuildReadCommand(stat, address, length, true);
      if (!command.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) command);
      OperateResult<byte[]> read1 = await this.ReadFromCoreServerAsync(command.Content);
      if (!read1.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) read1);
      if (read1.Content[0] != (byte) 229)
        return new OperateResult<bool[]>("PLC Receive Check Failed:" + SoftBasic.ByteToHexString(read1.Content, ' '));
      OperateResult<byte[]> read2 = await this.ReadFromCoreServerAsync(SiemensPPIOverTcp.GetExecuteConfirm(stat));
      if (!read2.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) read2);
      OperateResult check = SiemensPPIOverTcp.CheckResponse(read2.Content);
      if (!check.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>(check);
      byte[] buffer = new byte[read2.Content.Length - 27];
      if (read2.Content[21] == byte.MaxValue && read2.Content[22] == (byte) 3)
        Array.Copy((Array) read2.Content, 25, (Array) buffer, 0, buffer.Length);
      return OperateResult.CreateSuccessResult<bool[]>(SoftBasic.ByteToBoolArray(buffer, (int) length));
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Siemens.SiemensPPIOverTcp.Write(System.String,System.Byte[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      byte[] value)
    {
      byte stat = (byte) EstHelper.ExtractParameter(ref address, "s", (int) this.Station);
      OperateResult<byte[]> command = SiemensPPIOverTcp.BuildWriteCommand(stat, address, value);
      if (!command.IsSuccess)
        return (OperateResult) command;
      OperateResult<byte[]> read1 = await this.ReadFromCoreServerAsync(command.Content);
      if (!read1.IsSuccess)
        return (OperateResult) read1;
      if (read1.Content[0] != (byte) 229)
        return (OperateResult) new OperateResult<byte[]>("PLC Receive Check Failed:" + read1.Content[0].ToString());
      OperateResult<byte[]> read2 = await this.ReadFromCoreServerAsync(SiemensPPIOverTcp.GetExecuteConfirm(stat));
      if (!read2.IsSuccess)
        return (OperateResult) read2;
      OperateResult check = SiemensPPIOverTcp.CheckResponse(read2.Content);
      return check.IsSuccess ? OperateResult.CreateSuccessResult() : check;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Siemens.SiemensPPIOverTcp.Write(System.String,System.Boolean[])" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      bool[] value)
    {
      byte stat = (byte) EstHelper.ExtractParameter(ref address, "s", (int) this.Station);
      OperateResult<byte[]> command = SiemensPPIOverTcp.BuildWriteCommand(stat, address, value);
      if (!command.IsSuccess)
        return (OperateResult) command;
      OperateResult<byte[]> read1 = await this.ReadFromCoreServerAsync(command.Content);
      if (!read1.IsSuccess)
        return (OperateResult) read1;
      if (read1.Content[0] != (byte) 229)
        return (OperateResult) new OperateResult<byte[]>("PLC Receive Check Failed:" + read1.Content[0].ToString());
      OperateResult<byte[]> read2 = await this.ReadFromCoreServerAsync(SiemensPPIOverTcp.GetExecuteConfirm(stat));
      if (!read2.IsSuccess)
        return (OperateResult) read2;
      OperateResult check = SiemensPPIOverTcp.CheckResponse(read2.Content);
      return check.IsSuccess ? OperateResult.CreateSuccessResult() : check;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Siemens.SiemensPPI.ReadByte(System.String)" />
    [EstMqttApi("ReadByte", "")]
    public OperateResult<byte> ReadByte(string address) => ByteTransformHelper.GetResultFromArray<byte>(this.Read(address, (ushort) 1));

    /// <inheritdoc cref="M:EstCommunication.Profinet.Siemens.SiemensPPI.Write(System.String,System.Byte)" />
    [EstMqttApi("WriteByte", "")]
    public OperateResult Write(string address, byte value) => this.Write(address, new byte[1]
    {
      value
    });

    /// <inheritdoc cref="M:EstCommunication.Profinet.Siemens.SiemensPPIOverTcp.ReadByte(System.String)" />
    public async Task<OperateResult<byte>> ReadByteAsync(string address)
    {
      OperateResult<byte[]> result = await this.ReadAsync(address, (ushort) 1);
      return ByteTransformHelper.GetResultFromArray<byte>(result);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Siemens.SiemensPPIOverTcp.Write(System.String,System.Byte)" />
    public async Task<OperateResult> WriteAsync(string address, byte value)
    {
      OperateResult operateResult = await this.WriteAsync(address, new byte[1]
      {
        value
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Siemens.SiemensPPI.Start(System.String)" />
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
        this.station,
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
        OperateResult<byte[]> operateResult1 = this.ReadFromCoreServer(send);
        if (!operateResult1.IsSuccess)
          return (OperateResult) operateResult1;
        if (operateResult1.Content[0] != (byte) 229)
          return (OperateResult) new OperateResult<byte[]>("PLC Receive Check Failed:" + operateResult1.Content[0].ToString());
        OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(SiemensPPIOverTcp.GetExecuteConfirm(parameter1));
        return !operateResult2.IsSuccess ? (OperateResult) operateResult2 : OperateResult.CreateSuccessResult();
      }
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Siemens.SiemensPPI.Stop(System.String)" />
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
        this.station,
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
        OperateResult<byte[]> operateResult1 = this.ReadFromCoreServer(send);
        if (!operateResult1.IsSuccess)
          return (OperateResult) operateResult1;
        if (operateResult1.Content[0] != (byte) 229)
          return (OperateResult) new OperateResult<byte[]>("PLC Receive Check Failed:" + operateResult1.Content[0].ToString());
        OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(SiemensPPIOverTcp.GetExecuteConfirm(parameter1));
        return !operateResult2.IsSuccess ? (OperateResult) operateResult2 : OperateResult.CreateSuccessResult();
      }
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Siemens.SiemensPPI.Start(System.String)" />
    public async Task<OperateResult> StartAsync(string parameter = "")
    {
      byte stat = (byte) EstHelper.ExtractParameter(ref parameter, "s", (int) this.Station);
      byte[] cmd = new byte[39]
      {
        (byte) 104,
        (byte) 33,
        (byte) 33,
        (byte) 104,
        this.station,
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
      OperateResult<byte[]> read1 = await this.ReadFromCoreServerAsync(cmd);
      if (!read1.IsSuccess)
        return (OperateResult) read1;
      if (read1.Content[0] != (byte) 229)
        return (OperateResult) new OperateResult<byte[]>("PLC Receive Check Failed:" + read1.Content[0].ToString());
      OperateResult<byte[]> read2 = await this.ReadFromCoreServerAsync(SiemensPPIOverTcp.GetExecuteConfirm(stat));
      return read2.IsSuccess ? OperateResult.CreateSuccessResult() : (OperateResult) read2;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Siemens.SiemensPPI.Stop(System.String)" />
    public async Task<OperateResult> StopAsync(string parameter = "")
    {
      byte stat = (byte) EstHelper.ExtractParameter(ref parameter, "s", (int) this.Station);
      byte[] cmd = new byte[35]
      {
        (byte) 104,
        (byte) 29,
        (byte) 29,
        (byte) 104,
        this.station,
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
      OperateResult<byte[]> read1 = await this.ReadFromCoreServerAsync(cmd);
      if (!read1.IsSuccess)
        return (OperateResult) read1;
      if (read1.Content[0] != (byte) 229)
        return (OperateResult) new OperateResult<byte[]>("PLC Receive Check Failed:" + read1.Content[0].ToString());
      OperateResult<byte[]> read2 = await this.ReadFromCoreServerAsync(SiemensPPIOverTcp.GetExecuteConfirm(stat));
      return read2.IsSuccess ? OperateResult.CreateSuccessResult() : (OperateResult) read2;
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("SiemensPPIOverTcp[{0}:{1}]", (object) this.IpAddress, (object) this.Port);

    /// <summary>
    /// 解析数据地址，解析出地址类型，起始地址，DB块的地址<br />
    /// Parse data address, parse out address type, start address, db block address
    /// </summary>
    /// <param name="address">起始地址，例如M100，I0，Q0，V100 -&gt;
    /// Start address, such as M100,I0,Q0,V100</param>
    /// <returns>解析数据地址，解析出地址类型，起始地址，DB块的地址 -&gt;
    /// Parse data address, parse out address type, start address, db block address</returns>
    public static OperateResult<byte, int, ushort> AnalysisAddress(string address)
    {
      OperateResult<byte, int, ushort> operateResult = new OperateResult<byte, int, ushort>();
      try
      {
        operateResult.Content3 = (ushort) 0;
        if (address.Substring(0, 2) == "AI")
        {
          operateResult.Content1 = (byte) 6;
          operateResult.Content2 = S7AddressData.CalculateAddressStarted(address.Substring(2));
        }
        else if (address.Substring(0, 2) == "AQ")
        {
          operateResult.Content1 = (byte) 7;
          operateResult.Content2 = S7AddressData.CalculateAddressStarted(address.Substring(2));
        }
        else if (address[0] == 'T')
        {
          operateResult.Content1 = (byte) 31;
          operateResult.Content2 = S7AddressData.CalculateAddressStarted(address.Substring(1));
        }
        else if (address[0] == 'C')
        {
          operateResult.Content1 = (byte) 30;
          operateResult.Content2 = S7AddressData.CalculateAddressStarted(address.Substring(1));
        }
        else if (address.Substring(0, 2) == "SM")
        {
          operateResult.Content1 = (byte) 5;
          operateResult.Content2 = S7AddressData.CalculateAddressStarted(address.Substring(2));
        }
        else if (address[0] == 'S')
        {
          operateResult.Content1 = (byte) 4;
          operateResult.Content2 = S7AddressData.CalculateAddressStarted(address.Substring(1));
        }
        else if (address[0] == 'I')
        {
          operateResult.Content1 = (byte) 129;
          operateResult.Content2 = S7AddressData.CalculateAddressStarted(address.Substring(1));
        }
        else if (address[0] == 'Q')
        {
          operateResult.Content1 = (byte) 130;
          operateResult.Content2 = S7AddressData.CalculateAddressStarted(address.Substring(1));
        }
        else if (address[0] == 'M')
        {
          operateResult.Content1 = (byte) 131;
          operateResult.Content2 = S7AddressData.CalculateAddressStarted(address.Substring(1));
        }
        else if (address[0] == 'D' || address.Substring(0, 2) == "DB")
        {
          operateResult.Content1 = (byte) 132;
          string[] strArray = address.Split('.');
          operateResult.Content3 = address[1] != 'B' ? Convert.ToUInt16(strArray[0].Substring(1)) : Convert.ToUInt16(strArray[0].Substring(2));
          operateResult.Content2 = S7AddressData.CalculateAddressStarted(address.Substring(address.IndexOf('.') + 1));
        }
        else if (address[0] == 'V')
        {
          operateResult.Content1 = (byte) 132;
          operateResult.Content3 = (ushort) 1;
          operateResult.Content2 = S7AddressData.CalculateAddressStarted(address.Substring(1));
        }
        else
        {
          operateResult.Message = StringResources.Language.NotSupportedDataType;
          operateResult.Content1 = (byte) 0;
          operateResult.Content2 = 0;
          operateResult.Content3 = (ushort) 0;
          return operateResult;
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
      OperateResult<byte, int, ushort> operateResult = SiemensPPIOverTcp.AnalysisAddress(address);
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult);
      byte[] numArray = new byte[33];
      numArray[0] = (byte) 104;
      numArray[1] = BitConverter.GetBytes(numArray.Length - 6)[0];
      numArray[2] = BitConverter.GetBytes(numArray.Length - 6)[0];
      numArray[3] = (byte) 104;
      numArray[4] = station;
      numArray[5] = (byte) 0;
      numArray[6] = (byte) 108;
      numArray[7] = (byte) 50;
      numArray[8] = (byte) 1;
      numArray[9] = (byte) 0;
      numArray[10] = (byte) 0;
      numArray[11] = (byte) 0;
      numArray[12] = (byte) 0;
      numArray[13] = (byte) 0;
      numArray[14] = (byte) 14;
      numArray[15] = (byte) 0;
      numArray[16] = (byte) 0;
      numArray[17] = (byte) 4;
      numArray[18] = (byte) 1;
      numArray[19] = (byte) 18;
      numArray[20] = (byte) 10;
      numArray[21] = (byte) 16;
      numArray[22] = isBit ? (byte) 1 : (byte) 2;
      numArray[23] = (byte) 0;
      numArray[24] = BitConverter.GetBytes(length)[0];
      numArray[25] = BitConverter.GetBytes(length)[1];
      numArray[26] = (byte) operateResult.Content3;
      numArray[27] = operateResult.Content1;
      numArray[28] = BitConverter.GetBytes(operateResult.Content2)[2];
      numArray[29] = BitConverter.GetBytes(operateResult.Content2)[1];
      numArray[30] = BitConverter.GetBytes(operateResult.Content2)[0];
      int num = 0;
      for (int index = 4; index < 31; ++index)
        num += (int) numArray[index];
      numArray[31] = BitConverter.GetBytes(num)[0];
      numArray[32] = (byte) 22;
      return OperateResult.CreateSuccessResult<byte[]>(numArray);
    }

    /// <summary>生成一个写入PLC数据信息的报文内容</summary>
    /// <param name="station">PLC的站号</param>
    /// <param name="address">地址</param>
    /// <param name="values">数据值</param>
    /// <returns>是否写入成功</returns>
    public static OperateResult<byte[]> BuildWriteCommand(
      byte station,
      string address,
      byte[] values)
    {
      OperateResult<byte, int, ushort> operateResult = SiemensPPIOverTcp.AnalysisAddress(address);
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult);
      int length = values.Length;
      byte[] numArray = new byte[37 + values.Length];
      numArray[0] = (byte) 104;
      numArray[1] = BitConverter.GetBytes(numArray.Length - 6)[0];
      numArray[2] = BitConverter.GetBytes(numArray.Length - 6)[0];
      numArray[3] = (byte) 104;
      numArray[4] = station;
      numArray[5] = (byte) 0;
      numArray[6] = (byte) 124;
      numArray[7] = (byte) 50;
      numArray[8] = (byte) 1;
      numArray[9] = (byte) 0;
      numArray[10] = (byte) 0;
      numArray[11] = (byte) 0;
      numArray[12] = (byte) 0;
      numArray[13] = (byte) 0;
      numArray[14] = (byte) 14;
      numArray[15] = (byte) 0;
      numArray[16] = (byte) (values.Length + 4);
      numArray[17] = (byte) 5;
      numArray[18] = (byte) 1;
      numArray[19] = (byte) 18;
      numArray[20] = (byte) 10;
      numArray[21] = (byte) 16;
      numArray[22] = (byte) 2;
      numArray[23] = (byte) 0;
      numArray[24] = BitConverter.GetBytes(length)[0];
      numArray[25] = BitConverter.GetBytes(length)[1];
      numArray[26] = (byte) operateResult.Content3;
      numArray[27] = operateResult.Content1;
      numArray[28] = BitConverter.GetBytes(operateResult.Content2)[2];
      numArray[29] = BitConverter.GetBytes(operateResult.Content2)[1];
      numArray[30] = BitConverter.GetBytes(operateResult.Content2)[0];
      numArray[31] = (byte) 0;
      numArray[32] = (byte) 4;
      numArray[33] = BitConverter.GetBytes(length * 8)[1];
      numArray[34] = BitConverter.GetBytes(length * 8)[0];
      values.CopyTo((Array) numArray, 35);
      int num = 0;
      for (int index = 4; index < numArray.Length - 2; ++index)
        num += (int) numArray[index];
      numArray[numArray.Length - 2] = BitConverter.GetBytes(num)[0];
      numArray[numArray.Length - 1] = (byte) 22;
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

    /// <summary>创建写入PLC的bool类型数据报文指令</summary>
    /// <param name="station">PLC的站号信息</param>
    /// <param name="address">地址信息</param>
    /// <param name="values">bool[]数据值</param>
    /// <returns>带有成功标识的结果对象</returns>
    public static OperateResult<byte[]> BuildWriteCommand(
      byte station,
      string address,
      bool[] values)
    {
      OperateResult<byte, int, ushort> operateResult = SiemensPPIOverTcp.AnalysisAddress(address);
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult);
      byte[] numArray1 = SoftBasic.BoolArrayToByte(values);
      byte[] numArray2 = new byte[37 + numArray1.Length];
      numArray2[0] = (byte) 104;
      numArray2[1] = BitConverter.GetBytes(numArray2.Length - 6)[0];
      numArray2[2] = BitConverter.GetBytes(numArray2.Length - 6)[0];
      numArray2[3] = (byte) 104;
      numArray2[4] = station;
      numArray2[5] = (byte) 0;
      numArray2[6] = (byte) 124;
      numArray2[7] = (byte) 50;
      numArray2[8] = (byte) 1;
      numArray2[9] = (byte) 0;
      numArray2[10] = (byte) 0;
      numArray2[11] = (byte) 0;
      numArray2[12] = (byte) 0;
      numArray2[13] = (byte) 0;
      numArray2[14] = (byte) 14;
      numArray2[15] = (byte) 0;
      numArray2[16] = (byte) 5;
      numArray2[17] = (byte) 5;
      numArray2[18] = (byte) 1;
      numArray2[19] = (byte) 18;
      numArray2[20] = (byte) 10;
      numArray2[21] = (byte) 16;
      numArray2[22] = (byte) 1;
      numArray2[23] = (byte) 0;
      numArray2[24] = BitConverter.GetBytes(values.Length)[0];
      numArray2[25] = BitConverter.GetBytes(values.Length)[1];
      numArray2[26] = (byte) operateResult.Content3;
      numArray2[27] = operateResult.Content1;
      numArray2[28] = BitConverter.GetBytes(operateResult.Content2)[2];
      numArray2[29] = BitConverter.GetBytes(operateResult.Content2)[1];
      numArray2[30] = BitConverter.GetBytes(operateResult.Content2)[0];
      numArray2[31] = (byte) 0;
      numArray2[32] = (byte) 3;
      numArray2[33] = BitConverter.GetBytes(values.Length)[1];
      numArray2[34] = BitConverter.GetBytes(values.Length)[0];
      numArray1.CopyTo((Array) numArray2, 35);
      int num = 0;
      for (int index = 4; index < numArray2.Length - 2; ++index)
        num += (int) numArray2[index];
      numArray2[numArray2.Length - 2] = BitConverter.GetBytes(num)[0];
      numArray2[numArray2.Length - 1] = (byte) 22;
      return OperateResult.CreateSuccessResult<byte[]>(numArray2);
    }

    /// <summary>检查西门子PLC的返回的数据和合法性，对反馈的数据进行初步的校验</summary>
    /// <param name="content">服务器返回的原始的数据内容</param>
    /// <returns>是否校验成功</returns>
    public static OperateResult CheckResponse(byte[] content)
    {
      if (content.Length < 21)
        return new OperateResult(10000, "Failed, data too short:" + SoftBasic.ByteToHexString(content, ' '));
      if (content[17] != (byte) 0 || content[18] > (byte) 0)
        return new OperateResult((int) content[19], SiemensPPIOverTcp.GetMsgFromStatus(content[18], content[19]));
      return content[21] != byte.MaxValue ? new OperateResult((int) content[21], SiemensPPIOverTcp.GetMsgFromStatus(content[21])) : OperateResult.CreateSuccessResult();
    }

    /// <summary>根据站号信息获取命令二次确认的报文信息</summary>
    /// <param name="station">站号信息</param>
    /// <returns>二次命令确认的报文</returns>
    public static byte[] GetExecuteConfirm(byte station)
    {
      byte[] numArray = new byte[6]
      {
        (byte) 16,
        station,
        (byte) 0,
        (byte) 92,
        (byte) 94,
        (byte) 22
      };
      int num = 0;
      for (int index = 1; index < 4; ++index)
        num += (int) numArray[index];
      numArray[4] = (byte) num;
      return numArray;
    }
  }
}
