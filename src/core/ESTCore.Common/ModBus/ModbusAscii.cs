// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.ModBus.ModbusAscii
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

namespace ESTCore.Common.ModBus
{
    /// <summary>
    /// Modbus-Ascii通讯协议的类库，基于rtu类库完善过来，支持标准的功能码，也支持扩展的功能码实现，地址采用富文本的形式，详细见备注说明<br />
    /// The client communication class of Modbus-Ascii protocol is convenient for data interaction with the server. It supports standard function codes and also supports extended function codes.
    /// The address is in rich text. For details, see the remarks.
    /// </summary>
    /// <remarks>
    /// 本客户端支持的标准的modbus协议，Modbus-Tcp及Modbus-Udp内置的消息号会进行自增，地址支持富文本格式，具体参考示例代码。<br />
    /// 读取线圈，输入线圈，寄存器，输入寄存器的方法中的读取长度对商业授权用户不限制，内部自动切割读取，结果合并。
    /// </remarks>
    /// <example>
    /// 基本的用法请参照下面的代码示例，初始化部分的代码省略
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Modbus\ModbusAsciiExample.cs" region="Example" title="Modbus示例" />
    /// 复杂的读取数据的代码示例如下：
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Modbus\ModbusAsciiExample.cs" region="ReadExample" title="read示例" />
    /// 写入数据的代码如下：
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Modbus\ModbusAsciiExample.cs" region="WriteExample" title="write示例" />
    /// </example>
    public class ModbusAscii : ModbusRtu
    {
        /// <summary>
        /// 实例化一个Modbus-ascii协议的客户端对象<br />
        /// Instantiate a client object of the Modbus-ascii protocol
        /// </summary>
        public ModbusAscii() => this.LogMsgFormatBinary = false;

        /// <inheritdoc cref="M:ESTCore.Common.ModBus.ModbusRtu.#ctor(System.Byte)" />
        public ModbusAscii(byte station = 1)
          : base(station)
          => this.LogMsgFormatBinary = false;

        /// <inheritdoc />
        protected override OperateResult<byte[]> CheckModbusTcpResponse(byte[] send)
        {
            OperateResult<byte[]> operateResult = this.ReadBase(ModbusInfo.TransModbusCoreToAsciiPackCommand(send));
            if (!operateResult.IsSuccess)
                return operateResult;
            OperateResult<byte[]> core = ModbusInfo.TransAsciiPackCommandToCore(operateResult.Content);
            if (!core.IsSuccess)
                return core;
            return (int)send[1] + 128 == (int)core.Content[1] ? new OperateResult<byte[]>((int)core.Content[2], ModbusInfo.GetDescriptionByErrorCode(core.Content[2])) : ModbusInfo.ExtractActualData(core.Content);
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("ModbusAscii[{0}:{1}]", (object)this.PortName, (object)this.BaudRate);
    }
}
