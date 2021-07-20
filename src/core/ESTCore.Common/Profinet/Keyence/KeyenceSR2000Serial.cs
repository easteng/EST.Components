// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Keyence.KeyenceSR2000Serial
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Reflection;
using ESTCore.Common.Serial;

using System;

namespace ESTCore.Common.Profinet.Keyence
{
    /// <inheritdoc cref="T:ESTCore.Common.Profinet.Keyence.KeyenceSR2000SeriesTcp" />
    public class KeyenceSR2000Serial : SerialBase, IKeyenceSR2000Series
    {
        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000SeriesTcp.#ctor" />
        public KeyenceSR2000Serial()
        {
            this.ReceiveTimeout = 10000;
            this.SleepTime = 20;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.ReadBarcode(System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        public OperateResult<string> ReadBarcode() => KeyenceSR2000Helper.ReadBarcode(new Func<byte[], OperateResult<byte[]>>(((SerialBase)this).ReadBase));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.Reset(System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        public OperateResult Reset() => KeyenceSR2000Helper.Reset(new Func<byte[], OperateResult<byte[]>>(((SerialBase)this).ReadBase));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.OpenIndicator(System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        public OperateResult OpenIndicator() => KeyenceSR2000Helper.OpenIndicator(new Func<byte[], OperateResult<byte[]>>(((SerialBase)this).ReadBase));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.CloseIndicator(System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        public OperateResult CloseIndicator() => KeyenceSR2000Helper.CloseIndicator(new Func<byte[], OperateResult<byte[]>>(((SerialBase)this).ReadBase));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.ReadVersion(System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        public OperateResult<string> ReadVersion() => KeyenceSR2000Helper.ReadVersion(new Func<byte[], OperateResult<byte[]>>(((SerialBase)this).ReadBase));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.ReadCommandState(System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        public OperateResult<string> ReadCommandState() => KeyenceSR2000Helper.ReadCommandState(new Func<byte[], OperateResult<byte[]>>(((SerialBase)this).ReadBase));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.ReadErrorState(System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        public OperateResult<string> ReadErrorState() => KeyenceSR2000Helper.ReadErrorState(new Func<byte[], OperateResult<byte[]>>(((SerialBase)this).ReadBase));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.CheckInput(System.Int32,System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        public OperateResult<bool> CheckInput(int number) => KeyenceSR2000Helper.CheckInput(number, new Func<byte[], OperateResult<byte[]>>(((SerialBase)this).ReadBase));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.SetOutput(System.Int32,System.Boolean,System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        public OperateResult SetOutput(int number, bool value) => KeyenceSR2000Helper.SetOutput(number, value, new Func<byte[], OperateResult<byte[]>>(((SerialBase)this).ReadBase));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.ReadRecord(System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        public OperateResult<int[]> ReadRecord() => KeyenceSR2000Helper.ReadRecord(new Func<byte[], OperateResult<byte[]>>(((SerialBase)this).ReadBase));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.Lock(System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        public OperateResult Lock() => KeyenceSR2000Helper.Lock(new Func<byte[], OperateResult<byte[]>>(((SerialBase)this).ReadBase));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.UnLock(System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        public OperateResult UnLock() => KeyenceSR2000Helper.UnLock(new Func<byte[], OperateResult<byte[]>>(((SerialBase)this).ReadBase));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.ReadCustomer(System.String,System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        public OperateResult<string> ReadCustomer(string command) => KeyenceSR2000Helper.ReadCustomer(command, new Func<byte[], OperateResult<byte[]>>(((SerialBase)this).ReadBase));

        /// <inheritdoc />
        public override string ToString() => string.Format("KeyenceSR2000Serial[{0}:{1}]", (object)this.PortName, (object)this.BaudRate);
    }
}
