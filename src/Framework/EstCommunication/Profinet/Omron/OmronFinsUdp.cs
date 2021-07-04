// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.Omron.OmronFinsUdp
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.Core;
using EstCommunication.Core.Net;
using EstCommunication.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EstCommunication.Profinet.Omron
{
  /// <summary>
  /// 欧姆龙的Udp协议的实现类，地址类型和Fins-TCP一致，无连接的实现，可靠性不如<see cref="T:EstCommunication.Profinet.Omron.OmronFinsNet" /><br />
  /// Omron's Udp protocol implementation class, the address type is the same as Fins-TCP,
  /// and the connectionless implementation is not as reliable as <see cref="T:EstCommunication.Profinet.Omron.OmronFinsNet" />
  /// </summary>
  /// <remarks>
  /// <inheritdoc cref="T:EstCommunication.Profinet.Omron.OmronFinsNet" path="remarks" />
  /// </remarks>
  public class OmronFinsUdp : NetworkUdpDeviceBase
  {
    /// <inheritdoc cref="M:EstCommunication.Profinet.Omron.OmronFinsNet.#ctor(System.String,System.Int32)" />
    public OmronFinsUdp(string ipAddress, int port)
    {
      this.WordLength = (ushort) 1;
      this.IpAddress = ipAddress;
      this.Port = port;
      this.ByteTransform = (IByteTransform) new ReverseWordTransform();
      this.ByteTransform.DataFormat = DataFormat.CDAB;
      this.ByteTransform.IsStringReverseByteWord = true;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Omron.OmronFinsNet.#ctor" />
    public OmronFinsUdp()
    {
      this.WordLength = (ushort) 1;
      this.ByteTransform = (IByteTransform) new ReverseWordTransform();
      this.ByteTransform.DataFormat = DataFormat.CDAB;
      this.ByteTransform.IsStringReverseByteWord = true;
    }

    /// <inheritdoc />
    public override string IpAddress
    {
      get => base.IpAddress;
      set
      {
        base.IpAddress = value;
        this.DA1 = Convert.ToByte(base.IpAddress.Substring(base.IpAddress.LastIndexOf(".") + 1));
      }
    }

    /// <inheritdoc cref="P:EstCommunication.Profinet.Omron.OmronFinsNet.ICF" />
    public byte ICF { get; set; } = 128;

    /// <inheritdoc cref="P:EstCommunication.Profinet.Omron.OmronFinsNet.RSV" />
    public byte RSV { get; private set; } = 0;

    /// <inheritdoc cref="P:EstCommunication.Profinet.Omron.OmronFinsNet.GCT" />
    public byte GCT { get; set; } = 2;

    /// <inheritdoc cref="P:EstCommunication.Profinet.Omron.OmronFinsNet.DNA" />
    public byte DNA { get; set; } = 0;

    /// <inheritdoc cref="P:EstCommunication.Profinet.Omron.OmronFinsNet.DA1" />
    public byte DA1 { get; set; } = 19;

    /// <inheritdoc cref="P:EstCommunication.Profinet.Omron.OmronFinsNet.DA2" />
    public byte DA2 { get; set; } = 0;

    /// <inheritdoc cref="P:EstCommunication.Profinet.Omron.OmronFinsNet.SNA" />
    public byte SNA { get; set; } = 0;

    /// <inheritdoc cref="P:EstCommunication.Profinet.Omron.OmronFinsNet.SA1" />
    public byte SA1 { get; set; } = 13;

    /// <inheritdoc cref="P:EstCommunication.Profinet.Omron.OmronFinsNet.SA2" />
    public byte SA2 { get; set; }

    /// <inheritdoc cref="P:EstCommunication.Profinet.Omron.OmronFinsNet.SID" />
    public byte SID { get; set; } = 0;

    /// <inheritdoc cref="P:EstCommunication.Profinet.Omron.OmronFinsNet.ReadSplits" />
    public int ReadSplits { get; set; } = 500;

    /// <inheritdoc cref="M:EstCommunication.Profinet.Omron.OmronFinsNet.PackCommand(System.Byte[])" />
    private byte[] PackCommand(byte[] cmd)
    {
      byte[] numArray = new byte[10 + cmd.Length];
      numArray[0] = this.ICF;
      numArray[1] = this.RSV;
      numArray[2] = this.GCT;
      numArray[3] = this.DNA;
      numArray[4] = this.DA1;
      numArray[5] = this.DA2;
      numArray[6] = this.SNA;
      numArray[7] = this.SA1;
      numArray[8] = this.SA2;
      numArray[9] = this.SID;
      cmd.CopyTo((Array) numArray, 10);
      return numArray;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Omron.OmronFinsNet.BuildReadCommand(System.String,System.UInt16,System.Boolean)" />
    public OperateResult<List<byte[]>> BuildReadCommand(
      string address,
      ushort length,
      bool isBit)
    {
      OperateResult<List<byte[]>> operateResult = OmronFinsNetHelper.BuildReadCommand(address, length, isBit, this.ReadSplits);
      return !operateResult.IsSuccess ? operateResult : OperateResult.CreateSuccessResult<List<byte[]>>(operateResult.Content.Select<byte[], byte[]>((Func<byte[], byte[]>) (m => this.PackCommand(m))).ToList<byte[]>());
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Omron.OmronFinsNet.BuildWriteCommand(System.String,System.Byte[],System.Boolean)" />
    public OperateResult<byte[]> BuildWriteCommand(
      string address,
      byte[] value,
      bool isBit)
    {
      OperateResult<byte[]> operateResult = OmronFinsNetHelper.BuildWriteWordCommand(address, value, isBit);
      return !operateResult.IsSuccess ? operateResult : OperateResult.CreateSuccessResult<byte[]>(this.PackCommand(operateResult.Content));
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Omron.OmronFinsNet.Read(System.String,System.UInt16)" />
    [EstMqttApi("ReadByteArray", "")]
    public override OperateResult<byte[]> Read(string address, ushort length)
    {
      OperateResult<List<byte[]>> operateResult1 = this.BuildReadCommand(address, length, false);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult1);
      List<byte> byteList = new List<byte>();
      for (int index = 0; index < operateResult1.Content.Count; ++index)
      {
        OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content[index]);
        if (!operateResult2.IsSuccess)
          return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult2);
        OperateResult<byte[]> operateResult3 = OmronFinsNetHelper.UdpResponseValidAnalysis(operateResult2.Content, true);
        if (!operateResult3.IsSuccess)
          return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult3);
        byteList.AddRange((IEnumerable<byte>) operateResult3.Content);
      }
      return OperateResult.CreateSuccessResult<byte[]>(byteList.ToArray());
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Omron.OmronFinsNet.Write(System.String,System.Byte[])" />
    [EstMqttApi("WriteByteArray", "")]
    public override OperateResult Write(string address, byte[] value)
    {
      OperateResult<byte[]> operateResult1 = this.BuildWriteCommand(address, value, false);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return (OperateResult) operateResult2;
      OperateResult<byte[]> operateResult3 = OmronFinsNetHelper.UdpResponseValidAnalysis(operateResult2.Content, false);
      return !operateResult3.IsSuccess ? (OperateResult) operateResult3 : OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Omron.OmronFinsNet.ReadBool(System.String,System.UInt16)" />
    [EstMqttApi("ReadBoolArray", "")]
    public override OperateResult<bool[]> ReadBool(string address, ushort length)
    {
      OperateResult<List<byte[]>> operateResult1 = this.BuildReadCommand(address, length, true);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult1);
      List<bool> boolList = new List<bool>();
      for (int index = 0; index < operateResult1.Content.Count; ++index)
      {
        OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content[index]);
        if (!operateResult2.IsSuccess)
          return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult2);
        OperateResult<byte[]> operateResult3 = OmronFinsNetHelper.UdpResponseValidAnalysis(operateResult2.Content, true);
        if (!operateResult3.IsSuccess)
          return OperateResult.CreateFailedResult<bool[]>((OperateResult) operateResult3);
        boolList.AddRange(((IEnumerable<byte>) operateResult3.Content).Select<byte, bool>((Func<byte, bool>) (m => m != (byte) 0)));
      }
      return OperateResult.CreateSuccessResult<bool[]>(boolList.ToArray());
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Omron.OmronFinsNet.Write(System.String,System.Boolean[])" />
    [EstMqttApi("WriteBoolArray", "")]
    public override OperateResult Write(string address, bool[] values)
    {
      OperateResult<byte[]> operateResult1 = this.BuildWriteCommand(address, ((IEnumerable<bool>) values).Select<bool, byte>((Func<bool, byte>) (m => !m ? (byte) 0 : (byte) 1)).ToArray<byte>(), true);
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
      if (!operateResult2.IsSuccess)
        return (OperateResult) operateResult2;
      OperateResult<byte[]> operateResult3 = OmronFinsNetHelper.UdpResponseValidAnalysis(operateResult2.Content, false);
      return !operateResult3.IsSuccess ? (OperateResult) operateResult3 : OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("OmronFinsUdp[{0}:{1}]", (object) this.IpAddress, (object) this.Port);
  }
}
