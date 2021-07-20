// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Melsec.MelsecCipNet
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Profinet.AllenBradley;
using ESTCore.Common.Reflection;

using System.Threading.Tasks;

namespace ESTCore.Common.Profinet.Melsec
{
    /// <summary>三菱PLC的EIP协议的实现，当PLC使用了 QJ71EIP71 模块时就需要使用本类来访问</summary>
    public class MelsecCipNet : AllenBradleyNet
    {
        /// <inheritdoc cref="M:ESTCore.Common.Profinet.AllenBradley.AllenBradleyNet.#ctor" />
        public MelsecCipNet()
        {
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.AllenBradley.AllenBradleyNet.#ctor(System.String,System.Int32)" />
        public MelsecCipNet(string ipAddress, int port = 44818)
          : base(ipAddress, port)
        {
        }

        /// <summary>
        /// Read data information, data length for read array length information
        /// </summary>
        /// <param name="address">Address format of the node</param>
        /// <param name="length">In the case of arrays, the length of the array </param>
        /// <returns>Result data with result object </returns>
        [EstMqttApi("ReadByteArray", "")]
        public override OperateResult<byte[]> Read(string address, ushort length) => this.Read(new string[1]
        {
      address
        }, new int[1] { (int)length });

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Melsec.MelsecCipNet.Read(System.String,System.UInt16)" />
        public override async Task<OperateResult<byte[]>> ReadAsync(
          string address,
          ushort length)
        {
            OperateResult<byte[]> operateResult = await this.ReadAsync(new string[1]
            {
        address
            }, new int[1] { (int)length });
            return operateResult;
        }
    }
}
