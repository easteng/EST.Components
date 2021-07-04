// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.Freedom.FreedomUdpNet
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.Core;
using EstCommunication.Core.Net;
using EstCommunication.Reflection;

namespace EstCommunication.Profinet.Freedom
{
  /// <summary>
  /// 基于UDP/IP协议的自由协议，需要在地址里传入报文信息，也可以传入数据偏移信息，<see cref="P:EstCommunication.Core.Net.NetworkUdpDeviceBase.ByteTransform" />默认为<see cref="T:EstCommunication.Core.RegularByteTransform" />
  /// </summary>
  /// <example>
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\FreedomExample.cs" region="Sample3" title="实例化" />
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Profinet\FreedomExample.cs" region="Sample4" title="读取" />
  /// </example>
  public class FreedomUdpNet : NetworkUdpDeviceBase
  {
    /// <summary>实例化一个默认的对象</summary>
    public FreedomUdpNet() => this.ByteTransform = (IByteTransform) new RegularByteTransform();

    /// <summary>指定IP地址及端口号来实例化自由的TCP协议</summary>
    /// <param name="ipAddress">Ip地址</param>
    /// <param name="port">端口</param>
    public FreedomUdpNet(string ipAddress, int port)
    {
      this.IpAddress = ipAddress;
      this.Port = port;
      this.ByteTransform = (IByteTransform) new RegularByteTransform();
    }

    /// <inheritdoc />
    [EstMqttApi("ReadByteArray", "特殊的地址格式，需要采用解析包起始地址的报文，例如 modbus 协议为 stx=9;00 00 00 00 00 06 01 03 00 64 00 01")]
    public override OperateResult<byte[]> Read(string address, ushort length)
    {
      OperateResult<byte[], int> operateResult1 = FreedomTcpNet.AnalysisAddress(address);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<byte[]>((OperateResult) operateResult1);
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content1);
      if (!operateResult2.IsSuccess)
        return operateResult2;
      return operateResult1.Content2 >= operateResult2.Content.Length ? new OperateResult<byte[]>(StringResources.Language.ReceiveDataLengthTooShort) : OperateResult.CreateSuccessResult<byte[]>(operateResult2.Content.RemoveBegin<byte>(operateResult1.Content2));
    }

    /// <inheritdoc />
    public override OperateResult Write(string address, byte[] value) => (OperateResult) this.Read(address, (ushort) 0);

    /// <inheritdoc />
    public override string ToString() => string.Format("FreedomUdpNet<{0}>[{1}:{2}]", (object) this.ByteTransform.GetType(), (object) this.IpAddress, (object) this.Port);
  }
}
