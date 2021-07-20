// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.OpenProtocol.OpenProtocolNet
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Core;
using ESTCore.Common.Core.IMessage;
using ESTCore.Common.Core.Net;

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ESTCore.Common.Profinet.OpenProtocol
{
    /// <summary>
    /// 开放以太网协议，仍然在开发中<br />
    /// Open Ethernet protocol, still under development
    /// </summary>
    public class OpenProtocolNet : NetworkDoubleBase
    {
        /// <summary>
        /// 实例化一个默认的对象<br />
        /// Instantiate a default object
        /// </summary>
        public OpenProtocolNet() => this.ByteTransform = (IByteTransform)new RegularByteTransform();

        /// <summary>
        /// 使用指定的IP地址及端口来初始化对象<br />
        /// Use the specified IP address and port to initialize the object
        /// </summary>
        /// <param name="ipAddress">Ip地址</param>
        /// <param name="port">端口号</param>
        public OpenProtocolNet(string ipAddress, int port)
        {
            this.IpAddress = ipAddress;
            this.Port = port;
            this.ByteTransform = (IByteTransform)new RegularByteTransform();
        }

        /// <inheritdoc />
        protected override INetMessage GetNewNetMessage() => (INetMessage)new OpenProtocolMessage();

        /// <inheritdoc />
        protected override OperateResult InitializationOnConnect(Socket socket)
        {
            OperateResult<string> operateResult = this.ReadCustomer(1, 0, 0, 0, (List<string>)null);
            if (!operateResult.IsSuccess)
                return (OperateResult)operateResult;
            return operateResult.Content.Substring(4, 4) == "0002" ? OperateResult.CreateSuccessResult() : new OperateResult("Failed:" + operateResult.Content.Substring(4, 4));
        }

        /// <summary>自定义的命令读取</summary>
        /// <param name="mid"></param>
        /// <param name="revison"></param>
        /// <param name="stationId"></param>
        /// <param name="spindleId"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public OperateResult<string> ReadCustomer(
          int mid,
          int revison,
          int stationId,
          int spindleId,
          List<string> parameters)
        {
            if (parameters != null)
                parameters = new List<string>();
            OperateResult<byte[]> operateResult1 = OpenProtocolNet.BuildReadCommand(mid, revison, stationId, spindleId, parameters);
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<string>((OperateResult)operateResult1);
            OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
            return !operateResult2.IsSuccess ? OperateResult.CreateFailedResult<string>((OperateResult)operateResult2) : OperateResult.CreateSuccessResult<string>(Encoding.ASCII.GetString(operateResult2.Content));
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("OpenProtocolNet[{0}:{1}]", (object)this.IpAddress, (object)this.Port);

        /// <summary>构建一个读取的初始报文</summary>
        /// <param name="mid"></param>
        /// <param name="revison"></param>
        /// <param name="stationId"></param>
        /// <param name="spindleId"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static OperateResult<byte[]> BuildReadCommand(
          int mid,
          int revison,
          int stationId,
          int spindleId,
          List<string> parameters)
        {
            if (mid < 0 || mid > 9999)
                return new OperateResult<byte[]>("Mid must be between 0 - 9999");
            if (revison < 0 || revison > 999)
                return new OperateResult<byte[]>("revison must be between 0 - 999");
            if (stationId < 0 || stationId > 9)
                return new OperateResult<byte[]>("stationId must be between 0 - 9");
            if (spindleId < 0 || spindleId > 99)
                return new OperateResult<byte[]>("spindleId must be between 0 - 99");
            int count = 0;
            parameters?.ForEach((Action<string>)(m => count += m.Length));
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append((20 + count).ToString("D4"));
            stringBuilder.Append(mid.ToString("D4"));
            stringBuilder.Append(revison.ToString("D3"));
            stringBuilder.Append(char.MinValue);
            stringBuilder.Append(stationId.ToString("D1"));
            stringBuilder.Append(spindleId.ToString("D2"));
            stringBuilder.Append(char.MinValue);
            stringBuilder.Append(char.MinValue);
            stringBuilder.Append(char.MinValue);
            stringBuilder.Append(char.MinValue);
            stringBuilder.Append(char.MinValue);
            if (parameters != null)
            {
                for (int index = 0; index < parameters.Count; ++index)
                    stringBuilder.Append(parameters[index]);
            }
            stringBuilder.Append(char.MinValue);
            return OperateResult.CreateSuccessResult<byte[]>(Encoding.ASCII.GetBytes(stringBuilder.ToString()));
        }
    }
}
