// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.MQTT.IMqttSyncConnector
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Algorithms.ConnectPool;

using System;

namespace ESTCore.Common.MQTT
{
    /// <summary>
    /// 关于MqttSyncClient实现的接口<see cref="T:ESTCore.Common.Algorithms.ConnectPool.IConnector" />，从而实现了数据连接池的操作信息
    /// </summary>
    public class IMqttSyncConnector : IConnector
    {
        private MqttConnectionOptions connectionOptions;

        /// <summary>
        /// 根据连接的MQTT参数，实例化一个默认的对象<br />
        /// According to the connected MQTT parameters, instantiate a default object
        /// </summary>
        /// <param name="options">连接的参数信息</param>
        public IMqttSyncConnector(MqttConnectionOptions options)
        {
            this.connectionOptions = options;
            this.SyncClient = new MqttSyncClient(options);
        }

        /// <summary>
        /// 实例化一个默认的对象<br />
        /// Instantiate a default object
        /// </summary>
        public IMqttSyncConnector()
        {
        }

        /// <inheritdoc cref="P:ESTCore.Common.Algorithms.ConnectPool.IConnector.IsConnectUsing" />
        public bool IsConnectUsing { get; set; }

        /// <inheritdoc cref="P:ESTCore.Common.Algorithms.ConnectPool.IConnector.GuidToken" />
        public string GuidToken { get; set; }

        /// <inheritdoc cref="P:ESTCore.Common.Algorithms.ConnectPool.IConnector.LastUseTime" />
        public DateTime LastUseTime { get; set; }

        /// <summary>MQTT的连接对象</summary>
        public MqttSyncClient SyncClient { get; set; }

        /// <inheritdoc cref="M:ESTCore.Common.Algorithms.ConnectPool.IConnector.Close" />
        public void Close() => this.SyncClient?.ConnectClose();

        /// <inheritdoc cref="M:ESTCore.Common.Algorithms.ConnectPool.IConnector.Open" />
        public void Open() => this.SyncClient?.SetPersistentConnection();
    }
}
