// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Keyence.KeyenceSR2000SeriesTcp
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Core.Net;
using ESTCore.Common.Reflection;

using System;

namespace ESTCore.Common.Profinet.Keyence
{
    /// <summary>基恩士的SR2000的扫码设备，可以进行简单的交互</summary>
    /// <remarks>
    /// 当使用 "LON","LOFF","PRON","PROFF" 命令时，在发送时和发生错误时，将不会收到扫码设备的回发命令，而是输出读取结果。
    /// 如果也希望获取上述命令的响应时，请在以下位置进行设置。[设置列表]-[其他]-"指定基本命令的响应字符串"
    /// </remarks>
    public class KeyenceSR2000SeriesTcp : NetworkDoubleBase, IKeyenceSR2000Series
    {
        /// <summary>
        /// 实例化基恩士的SR2000的扫码设备通讯对象<br />
        /// Instantiate keyence's SR2000 scan code device communication object
        /// </summary>
        public KeyenceSR2000SeriesTcp()
        {
            this.receiveTimeOut = 10000;
            this.SleepTime = 20;
        }

        /// <summary>
        /// 指定ip地址及端口号来实例化一个基恩士的SR2000的扫码设备通讯对象<br />
        /// Specify the ip address and port number to instantiate a keyence SR2000 scan code device communication object
        /// </summary>
        /// <param name="ipAddress">PLC的Ip地址</param>
        /// <param name="port">PLC的端口</param>
        public KeyenceSR2000SeriesTcp(string ipAddress, int port)
        {
            this.IpAddress = ipAddress;
            this.Port = port;
            this.receiveTimeOut = 10000;
            this.SleepTime = 20;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.ReadBarcode(System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        public OperateResult<string> ReadBarcode() => KeyenceSR2000Helper.ReadBarcode(new Func<byte[], OperateResult<byte[]>>(((NetworkDoubleBase)this).ReadFromCoreServer));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.Reset(System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        public OperateResult Reset() => KeyenceSR2000Helper.Reset(new Func<byte[], OperateResult<byte[]>>(((NetworkDoubleBase)this).ReadFromCoreServer));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.OpenIndicator(System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        public OperateResult OpenIndicator() => KeyenceSR2000Helper.OpenIndicator(new Func<byte[], OperateResult<byte[]>>(((NetworkDoubleBase)this).ReadFromCoreServer));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.CloseIndicator(System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        public OperateResult CloseIndicator() => KeyenceSR2000Helper.CloseIndicator(new Func<byte[], OperateResult<byte[]>>(((NetworkDoubleBase)this).ReadFromCoreServer));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.ReadVersion(System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        public OperateResult<string> ReadVersion() => KeyenceSR2000Helper.ReadVersion(new Func<byte[], OperateResult<byte[]>>(((NetworkDoubleBase)this).ReadFromCoreServer));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.ReadCommandState(System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        public OperateResult<string> ReadCommandState() => KeyenceSR2000Helper.ReadCommandState(new Func<byte[], OperateResult<byte[]>>(((NetworkDoubleBase)this).ReadFromCoreServer));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.ReadErrorState(System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        public OperateResult<string> ReadErrorState() => KeyenceSR2000Helper.ReadErrorState(new Func<byte[], OperateResult<byte[]>>(((NetworkDoubleBase)this).ReadFromCoreServer));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.CheckInput(System.Int32,System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        public OperateResult<bool> CheckInput(int number) => KeyenceSR2000Helper.CheckInput(number, new Func<byte[], OperateResult<byte[]>>(((NetworkDoubleBase)this).ReadFromCoreServer));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.SetOutput(System.Int32,System.Boolean,System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        public OperateResult SetOutput(int number, bool value) => KeyenceSR2000Helper.SetOutput(number, value, new Func<byte[], OperateResult<byte[]>>(((NetworkDoubleBase)this).ReadFromCoreServer));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.ReadRecord(System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        public OperateResult<int[]> ReadRecord() => KeyenceSR2000Helper.ReadRecord(new Func<byte[], OperateResult<byte[]>>(((NetworkDoubleBase)this).ReadFromCoreServer));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.Lock(System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        public OperateResult Lock() => KeyenceSR2000Helper.Lock(new Func<byte[], OperateResult<byte[]>>(((NetworkDoubleBase)this).ReadFromCoreServer));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.UnLock(System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        public OperateResult UnLock() => KeyenceSR2000Helper.UnLock(new Func<byte[], OperateResult<byte[]>>(((NetworkDoubleBase)this).ReadFromCoreServer));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.ReadCustomer(System.String,System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        public OperateResult<string> ReadCustomer(string command) => KeyenceSR2000Helper.ReadCustomer(command, new Func<byte[], OperateResult<byte[]>>(((NetworkDoubleBase)this).ReadFromCoreServer));

        /// <inheritdoc />
        public override string ToString() => string.Format("KeyenceSR2000SeriesTcp[{0}:{1}]", (object)this.IpAddress, (object)this.Port);
    }
}
