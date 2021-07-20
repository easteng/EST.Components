// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Freedom.FreedomSerial
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Core;
using ESTCore.Common.Reflection;
using ESTCore.Common.Serial;

namespace ESTCore.Common.Profinet.Freedom
{
    /// <summary>
    /// 基于串口的自由协议，需要在地址里传入报文信息，也可以传入数据偏移信息，<see cref="P:ESTCore.Common.Serial.SerialDeviceBase.ByteTransform" />默认为<see cref="T:ESTCore.Common.Core.RegularByteTransform" />
    /// </summary>
    /// <example>
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\FreedomExample.cs" region="Sample5" title="实例化" />
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\FreedomExample.cs" region="Sample6" title="读取" />
    /// </example>
    public class FreedomSerial : SerialDeviceBase
    {
        /// <summary>实例化一个默认的对象</summary>
        public FreedomSerial() => this.ByteTransform = (IByteTransform)new RegularByteTransform();

        /// <inheritdoc />
        [EstMqttApi("ReadByteArray", "特殊的地址格式，需要采用解析包起始地址的报文，例如 modbus 协议为 stx=9;00 00 00 00 00 06 01 03 00 64 00 01")]
        public override OperateResult<byte[]> Read(string address, ushort length)
        {
            OperateResult<byte[], int> operateResult1 = FreedomTcpNet.AnalysisAddress(address);
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult1);
            OperateResult<byte[]> operateResult2 = this.ReadBase(operateResult1.Content1);
            if (!operateResult2.IsSuccess)
                return operateResult2;
            return operateResult1.Content2 >= operateResult2.Content.Length ? new OperateResult<byte[]>(StringResources.Language.ReceiveDataLengthTooShort) : OperateResult.CreateSuccessResult<byte[]>(operateResult2.Content.RemoveBegin<byte>(operateResult1.Content2));
        }

        /// <inheritdoc />
        public override OperateResult Write(string address, byte[] value) => (OperateResult)this.Read(address, (ushort)0);

        /// <inheritdoc />
        public override string ToString() => string.Format("FreedomSerial<{0}>[{1}:{2}]", (object)this.ByteTransform.GetType(), (object)this.PortName, (object)this.BaudRate);
    }
}
