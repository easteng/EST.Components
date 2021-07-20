// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Freedom.FreedomTcpNet
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Core;
using ESTCore.Common.Core.Net;
using ESTCore.Common.Reflection;

using System;
using System.Threading.Tasks;

namespace ESTCore.Common.Profinet.Freedom
{
    /// <summary>
    /// 基于TCP/IP协议的自由协议，需要在地址里传入报文信息，也可以传入数据偏移信息，<see cref="P:ESTCore.Common.Core.Net.NetworkDoubleBase.ByteTransform" />默认为<see cref="T:ESTCore.Common.Core.RegularByteTransform" />
    /// </summary>
    /// <example>
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\FreedomExample.cs" region="Sample1" title="实例化" />
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\FreedomExample.cs" region="Sample2" title="连接及读取" />
    /// </example>
    public class FreedomTcpNet : NetworkDeviceBase
    {
        /// <summary>实例化一个默认的对象</summary>
        public FreedomTcpNet() => this.ByteTransform = (IByteTransform)new RegularByteTransform();

        /// <summary>指定IP地址及端口号来实例化自由的TCP协议</summary>
        /// <param name="ipAddress">Ip地址</param>
        /// <param name="port">端口</param>
        public FreedomTcpNet(string ipAddress, int port)
        {
            this.IpAddress = ipAddress;
            this.Port = port;
            this.ByteTransform = (IByteTransform)new RegularByteTransform();
        }

        /// <inheritdoc />
        [EstMqttApi("ReadByteArray", "特殊的地址格式，需要采用解析包起始地址的报文，例如 modbus 协议为 stx=9;00 00 00 00 00 06 01 03 00 64 00 01")]
        public override OperateResult<byte[]> Read(string address, ushort length)
        {
            OperateResult<byte[], int> operateResult1 = FreedomTcpNet.AnalysisAddress(address);
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult1);
            OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content1);
            if (!operateResult2.IsSuccess)
                return operateResult2;
            return operateResult1.Content2 >= operateResult2.Content.Length ? new OperateResult<byte[]>(StringResources.Language.ReceiveDataLengthTooShort) : OperateResult.CreateSuccessResult<byte[]>(operateResult2.Content.RemoveBegin<byte>(operateResult1.Content2));
        }

        /// <inheritdoc />
        public override OperateResult Write(string address, byte[] value) => (OperateResult)this.Read(address, (ushort)0);

        /// <inheritdoc />
        public override async Task<OperateResult<byte[]>> ReadAsync(
          string address,
          ushort length)
        {
            OperateResult<byte[], int> analysis = FreedomTcpNet.AnalysisAddress(address);
            if (!analysis.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)analysis);
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(analysis.Content1);
            return read.IsSuccess ? (analysis.Content2 < read.Content.Length ? OperateResult.CreateSuccessResult<byte[]>(read.Content.RemoveBegin<byte>(analysis.Content2)) : new OperateResult<byte[]>(StringResources.Language.ReceiveDataLengthTooShort)) : read;
        }

        /// <inheritdoc />
        public override async Task<OperateResult> WriteAsync(
          string address,
          byte[] value)
        {
            OperateResult<byte[]> operateResult = await this.ReadAsync(address, (ushort)0);
            return (OperateResult)operateResult;
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("FreedomTcpNet<{0}>[{1}:{2}]", (object)this.ByteTransform.GetType(), (object)this.IpAddress, (object)this.Port);

        /// <summary>分析地址的方法，会转换成一个数据报文和数据结果偏移的信息</summary>
        /// <param name="address">地址信息</param>
        /// <returns>报文结果内容</returns>
        public static OperateResult<byte[], int> AnalysisAddress(string address)
        {
            try
            {
                int num = 0;
                byte[] numArray = (byte[])null;
                if (address.IndexOf(';') > 0)
                {
                    string[] strArray = address.Split(new char[1]
                    {
            ';'
                    }, StringSplitOptions.RemoveEmptyEntries);
                    for (int index = 0; index < strArray.Length; ++index)
                    {
                        if (strArray[index].StartsWith("stx="))
                            num = Convert.ToInt32(strArray[index].Substring(4));
                        else
                            numArray = strArray[index].ToHexBytes();
                    }
                }
                else
                    numArray = address.ToHexBytes();
                return OperateResult.CreateSuccessResult<byte[], int>(numArray, num);
            }
            catch (Exception ex)
            {
                return new OperateResult<byte[], int>(ex.Message);
            }
        }
    }
}
