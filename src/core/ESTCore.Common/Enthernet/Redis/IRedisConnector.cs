// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Enthernet.Redis.IRedisConnector
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Algorithms.ConnectPool;

using System;

namespace ESTCore.Common.Enthernet.Redis
{
    /// <summary>
    /// 关于Redis实现的接口<see cref="T:ESTCore.Common.Algorithms.ConnectPool.IConnector" />，从而实现了数据连接池的操作信息
    /// </summary>
    public class IRedisConnector : IConnector
    {
        /// <inheritdoc cref="P:ESTCore.Common.Algorithms.ConnectPool.IConnector.IsConnectUsing" />
        public bool IsConnectUsing { get; set; }

        /// <inheritdoc cref="P:ESTCore.Common.Algorithms.ConnectPool.IConnector.GuidToken" />
        public string GuidToken { get; set; }

        /// <inheritdoc cref="P:ESTCore.Common.Algorithms.ConnectPool.IConnector.LastUseTime" />
        public DateTime LastUseTime { get; set; }

        /// <summary>Redis的连接对象</summary>
        public RedisClient Redis { get; set; }

        /// <inheritdoc cref="M:ESTCore.Common.Algorithms.ConnectPool.IConnector.Close" />
        public void Close() => this.Redis?.ConnectClose();

        /// <inheritdoc cref="M:ESTCore.Common.Algorithms.ConnectPool.IConnector.Open" />
        public void Open() => this.Redis?.SetPersistentConnection();
    }
}
