// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Keyence.IKeyenceSR2000Series
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Reflection;

namespace ESTCore.Common.Profinet.Keyence
{
    /// <summary>基恩士SR2000系列扫码设备的通用接口</summary>
    internal interface IKeyenceSR2000Series
    {
        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.ReadBarcode(System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        OperateResult<string> ReadBarcode();

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.Reset(System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        OperateResult Reset();

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.OpenIndicator(System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        OperateResult OpenIndicator();

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.CloseIndicator(System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        OperateResult CloseIndicator();

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.ReadVersion(System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        OperateResult<string> ReadVersion();

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.ReadCommandState(System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        OperateResult<string> ReadCommandState();

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.ReadErrorState(System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        OperateResult<string> ReadErrorState();

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.CheckInput(System.Int32,System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        OperateResult<bool> CheckInput(int number);

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.SetOutput(System.Int32,System.Boolean,System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        OperateResult SetOutput(int number, bool value);

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.ReadRecord(System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        OperateResult<int[]> ReadRecord();

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.Lock(System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        OperateResult Lock();

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.UnLock(System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        OperateResult UnLock();

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Keyence.KeyenceSR2000Helper.ReadCustomer(System.String,System.Func{System.Byte[],ESTCore.Common.OperateResult{System.Byte[]}})" />
        [EstMqttApi]
        OperateResult<string> ReadCustomer(string command);
    }
}
