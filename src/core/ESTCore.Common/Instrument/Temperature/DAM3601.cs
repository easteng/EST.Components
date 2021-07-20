// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Instrument.Temperature.DAM3601
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.ModBus;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ESTCore.Common.Instrument.Temperature
{
    /// <summary>阿尔泰科技发展有限公司的DAM3601温度采集模块，基于ModbusRtu开发完成。</summary>
    /// <remarks>
    /// 该温度采集模块是基于modbus-rtu，但不是标准的modbus协议，存在一些小误差，需要重写实现，并且提供了基础的数据转换
    /// </remarks>
    public class DAM3601 : ModbusRtu
    {
        /// <summary>实例化一个默认的对象</summary>
        public DAM3601() => this.SleepTime = 200;

        /// <summary>使用站号实例化默认的对象</summary>
        /// <param name="station">站号信息</param>
        public DAM3601(byte station)
          : base(station)
          => this.SleepTime = 200;

        /// <summary>读取所有的温度数据，并转化成相关的信息</summary>
        /// <returns>结果数据对象</returns>
        public OperateResult<float[]> ReadAllTemperature()
        {
            string address = "x=4;1";
            if (this.AddressStartWithZero)
                address = "x=4;0";
            OperateResult<short[]> operateResult = this.ReadInt16(address, (ushort)128);
            return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<float[]>((OperateResult)operateResult) : OperateResult.CreateSuccessResult<float[]>(((IEnumerable<short>)operateResult.Content).Select<short, float>((Func<short, float>)(m => this.TransformValue(m))).ToArray<float>());
        }

        /// <summary>数据转换方法，将读取的值，</summary>
        /// <param name="value">读取的值</param>
        /// <returns>转换后的值</returns>
        private float TransformValue(short value) => ((int)value & 2048) > 0 ? (float)(((int)value & 4095 ^ 4095) + 1) * (-1f / 16f) : (float)((int)value & 2047) * (1f / 16f);

        /// <summary>从Modbus服务器批量读取寄存器的信息，需要指定起始地址，读取长度</summary>
        /// <param name="address">起始地址，格式为"1234"，或者是带功能码格式x=3;1234</param>
        /// <param name="length">读取的数量</param>
        /// <returns>带有成功标志的字节信息</returns>
        /// <example>
        /// 此处演示批量读取的示例
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Modbus\Modbus.cs" region="ReadExample2" title="Read示例" />
        /// </example>
        public override OperateResult<byte[]> Read(string address, ushort length)
        {
            OperateResult<byte[][]> operateResult = ModbusInfo.BuildReadModbusCommand(address, length, this.Station, this.AddressStartWithZero, (byte)16);
            return !operateResult.IsSuccess ? operateResult.ConvertFailed<byte[]>() : this.CheckModbusTcpResponse(operateResult.Content[0]);
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("DAM3601[{0}:{1}]", (object)this.PortName, (object)this.BaudRate);
    }
}
