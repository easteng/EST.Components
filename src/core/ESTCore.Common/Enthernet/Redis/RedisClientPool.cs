// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Enthernet.Redis.RedisClientPool
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using System;
using System.Threading.Tasks;

namespace ESTCore.Common.Enthernet.Redis
{
    /// <summary>
    /// <b>[商业授权]</b> Redis客户端的连接池类对象，用于共享当前的连接池，合理的动态调整连接对象，然后进行高效通信的操作，默认连接数无限大。<br />
    /// <b>[Authorization]</b> The connection pool class object of the Redis client is used to share the current connection pool,
    /// reasonably dynamically adjust the connection object, and then perform efficient communication operations,
    /// The default number of connections is unlimited
    /// </summary>
    /// <remarks>
    /// 本连接池的实现仅对商业授权用户开放，用于提供服务器端的与Redis的并发读写能力。使用上和普通的 <see cref="T:ESTCore.Common.Enthernet.Redis.RedisClient" /> 没有区别，
    /// 但是在高并发上却高性能的多，占用的连接也更少，这一切都是连接池自动实现的。
    /// </remarks>
    public class RedisClientPool
    {
        private ESTCore.Common.Algorithms.ConnectPool.ConnectPool<IRedisConnector> redisConnectPool;

        /// <summary>
        /// 实例化一个默认的客户端连接池对象，需要指定实例Redis对象时的IP，端口，密码信息<br />
        /// To instantiate a default client connection pool object, you need to specify the IP, port, and password information when the Redis object is instantiated
        /// </summary>
        /// <param name="ipAddress">IP地址信息</param>
        /// <param name="port">端口号信息</param>
        /// <param name="password">密码，如果没有，请输入空字符串</param>
        public RedisClientPool(string ipAddress, int port, string password)
        {
            if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
                throw new Exception(StringResources.Language.InsufficientPrivileges);
            this.redisConnectPool = new ESTCore.Common.Algorithms.ConnectPool.ConnectPool<IRedisConnector>((Func<IRedisConnector>)(() => new IRedisConnector()
            {
                Redis = new RedisClient(ipAddress, port, password)
            }));
            this.redisConnectPool.MaxConnector = int.MaxValue;
        }

        /// <summary>
        /// 实例化一个默认的客户端连接池对象，需要指定实例Redis对象时的IP，端口，密码信息，以及可以指定额外的初始化操作<br />
        /// To instantiate a default client connection pool object, you need to specify the IP, port,
        /// and password information when the Redis object is instantiated, and you can specify additional initialization operations
        /// </summary>
        /// <param name="ipAddress">IP地址信息</param>
        /// <param name="port">端口号信息</param>
        /// <param name="password">密码，如果没有，请输入空字符串</param>
        /// <param name="initialize">额外的初始化信息，比如修改db块的信息。</param>
        public RedisClientPool(
          string ipAddress,
          int port,
          string password,
          Action<RedisClient> initialize)
        {
            if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
                throw new Exception(StringResources.Language.InsufficientPrivileges);
            this.redisConnectPool = new ESTCore.Common.Algorithms.ConnectPool.ConnectPool<IRedisConnector>((Func<IRedisConnector>)(() =>
           {
               RedisClient redisClient = new RedisClient(ipAddress, port, password);
               initialize(redisClient);
               return new IRedisConnector() { Redis = redisClient };
           }));
            this.redisConnectPool.MaxConnector = int.MaxValue;
        }

        /// <summary>
        /// 获取当前的连接池管理对象信息<br />
        /// Get current connection pool management object information
        /// </summary>
        public ESTCore.Common.Algorithms.ConnectPool.ConnectPool<IRedisConnector> GetRedisConnectPool => this.redisConnectPool;

        /// <inheritdoc cref="P:ESTCore.Common.Algorithms.ConnectPool.ConnectPool`1.MaxConnector" />
        public int MaxConnector
        {
            get => this.redisConnectPool.MaxConnector;
            set => this.redisConnectPool.MaxConnector = value;
        }

        private OperateResult<T> ConnectPoolExecute<T>(
          Func<RedisClient, OperateResult<T>> exec)
        {
            if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
                throw new Exception(StringResources.Language.InsufficientPrivileges);
            IRedisConnector availableConnector = this.redisConnectPool.GetAvailableConnector();
            OperateResult<T> operateResult = exec(availableConnector.Redis);
            this.redisConnectPool.ReturnConnector(availableConnector);
            return operateResult;
        }

        private OperateResult ConnectPoolExecute(Func<RedisClient, OperateResult> exec)
        {
            if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
                throw new Exception(StringResources.Language.InsufficientPrivileges);
            IRedisConnector availableConnector = this.redisConnectPool.GetAvailableConnector();
            OperateResult operateResult = exec(availableConnector.Redis);
            this.redisConnectPool.ReturnConnector(availableConnector);
            return operateResult;
        }

        private async Task<OperateResult<T>> ConnectPoolExecuteAsync<T>(
          Func<RedisClient, Task<OperateResult<T>>> execAsync)
        {
            if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
                throw new Exception(StringResources.Language.InsufficientPrivileges);
            IRedisConnector client = this.redisConnectPool.GetAvailableConnector();
            OperateResult<T> result = await execAsync(client.Redis);
            this.redisConnectPool.ReturnConnector(client);
            return result;
        }

        private async Task<OperateResult> ConnectPoolExecuteAsync(
          Func<RedisClient, Task<OperateResult>> execAsync)
        {
            if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
                throw new Exception(StringResources.Language.InsufficientPrivileges);
            IRedisConnector client = this.redisConnectPool.GetAvailableConnector();
            OperateResult result = await execAsync(client.Redis);
            this.redisConnectPool.ReturnConnector(client);
            return result;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.DeleteKey(System.String[])" />
        public OperateResult<int> DeleteKey(string[] keys) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.DeleteKey(keys)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.DeleteKey(System.String)" />
        public OperateResult<int> DeleteKey(string key) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.DeleteKey(key)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ExistsKey(System.String)" />
        public OperateResult<int> ExistsKey(string key) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.ExistsKey(key)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ExpireKey(System.String,System.Int32)" />
        public OperateResult<int> ExpireKey(string key, int seconds) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.ExpireKey(key, seconds)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ReadAllKeys(System.String)" />
        public OperateResult<string[]> ReadAllKeys(string pattern) => this.ConnectPoolExecute<string[]>((Func<RedisClient, OperateResult<string[]>>)(m => m.ReadAllKeys(pattern)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.MoveKey(System.String,System.Int32)" />
        public OperateResult MoveKey(string key, int db) => this.ConnectPoolExecute((Func<RedisClient, OperateResult>)(m => m.MoveKey(key, db)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.PersistKey(System.String)" />
        public OperateResult<int> PersistKey(string key) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.PersistKey(key)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ReadRandomKey" />
        public OperateResult<string> ReadRandomKey() => this.ConnectPoolExecute<string>((Func<RedisClient, OperateResult<string>>)(m => m.ReadRandomKey()));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.RenameKey(System.String,System.String)" />
        public OperateResult RenameKey(string key1, string key2) => this.ConnectPoolExecute((Func<RedisClient, OperateResult>)(m => m.RenameKey(key1, key2)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ReadKeyType(System.String)" />
        public OperateResult<string> ReadKeyType(string key) => this.ConnectPoolExecute<string>((Func<RedisClient, OperateResult<string>>)(m => m.ReadKeyType(key)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ReadKeyTTL(System.String)" />
        public OperateResult<int> ReadKeyTTL(string key) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.ReadKeyTTL(key)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.DeleteKey(System.String[])" />
        public async Task<OperateResult<int>> DeleteKeyAsync(string[] keys)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.DeleteKeyAsync(keys)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.DeleteKey(System.String)" />
        public async Task<OperateResult<int>> DeleteKeyAsync(string key)
        {
            OperateResult<int> operateResult = await this.DeleteKeyAsync(new string[1]
            {
        key
            });
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ExistsKey(System.String)" />
        public async Task<OperateResult<int>> ExistsKeyAsync(string key)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.ExistsKeyAsync(key)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ExpireKey(System.String,System.Int32)" />
        public async Task<OperateResult<int>> ExpireKeyAsync(string key, int seconds)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.ExpireKeyAsync(key, seconds)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ReadAllKeys(System.String)" />
        public async Task<OperateResult<string[]>> ReadAllKeysAsync(string pattern)
        {
            OperateResult<string[]> operateResult = await this.ConnectPoolExecuteAsync<string[]>((Func<RedisClient, Task<OperateResult<string[]>>>)(m => m.ReadAllKeysAsync(pattern)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.MoveKey(System.String,System.Int32)" />
        public async Task<OperateResult> MoveKeyAsync(string key, int db)
        {
            OperateResult operateResult = await this.ConnectPoolExecuteAsync((Func<RedisClient, Task<OperateResult>>)(m => m.MoveKeyAsync(key, db)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.PersistKey(System.String)" />
        public async Task<OperateResult<int>> PersistKeyAsync(string key)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.PersistKeyAsync(key)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ReadRandomKey" />
        public async Task<OperateResult<string>> ReadRandomKeyAsync()
        {
            OperateResult<string> operateResult = await this.ConnectPoolExecuteAsync<string>((Func<RedisClient, Task<OperateResult<string>>>)(m => m.ReadRandomKeyAsync()));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.RenameKey(System.String,System.String)" />
        public async Task<OperateResult> RenameKeyAsync(string key1, string key2)
        {
            OperateResult operateResult = await this.ConnectPoolExecuteAsync((Func<RedisClient, Task<OperateResult>>)(m => m.RenameKeyAsync(key1, key2)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ReadKeyType(System.String)" />
        public async Task<OperateResult<string>> ReadKeyTypeAsync(string key)
        {
            OperateResult<string> operateResult = await this.ConnectPoolExecuteAsync<string>((Func<RedisClient, Task<OperateResult<string>>>)(m => m.ReadKeyTypeAsync(key)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ReadKeyTTL(System.String)" />
        public async Task<OperateResult<int>> ReadKeyTTLAsync(string key)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.ReadKeyTTLAsync(key)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.AppendKey(System.String,System.String)" />
        public OperateResult<int> AppendKey(string key, string value) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.AppendKey(key, value)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.DecrementKey(System.String)" />
        public OperateResult<long> DecrementKey(string key) => this.ConnectPoolExecute<long>((Func<RedisClient, OperateResult<long>>)(m => m.DecrementKey(key)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.DecrementKey(System.String,System.Int64)" />
        public OperateResult<long> DecrementKey(string key, long value) => this.ConnectPoolExecute<long>((Func<RedisClient, OperateResult<long>>)(m => m.DecrementKey(key, value)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ReadKey(System.String)" />
        public OperateResult<string> ReadKey(string key) => this.ConnectPoolExecute<string>((Func<RedisClient, OperateResult<string>>)(m => m.ReadKey(key)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ReadKeyRange(System.String,System.Int32,System.Int32)" />
        public OperateResult<string> ReadKeyRange(string key, int start, int end) => this.ConnectPoolExecute<string>((Func<RedisClient, OperateResult<string>>)(m => m.ReadKeyRange(key, start, end)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ReadAndWriteKey(System.String,System.String)" />
        public OperateResult<string> ReadAndWriteKey(string key, string value) => this.ConnectPoolExecute<string>((Func<RedisClient, OperateResult<string>>)(m => m.ReadAndWriteKey(key, value)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.IncrementKey(System.String)" />
        public OperateResult<long> IncrementKey(string key) => this.ConnectPoolExecute<long>((Func<RedisClient, OperateResult<long>>)(m => m.IncrementKey(key)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.IncrementKey(System.String,System.Int64)" />
        public OperateResult<long> IncrementKey(string key, long value) => this.ConnectPoolExecute<long>((Func<RedisClient, OperateResult<long>>)(m => m.IncrementKey(key, value)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.IncrementKey(System.String,System.Single)" />
        public OperateResult<string> IncrementKey(string key, float value) => this.ConnectPoolExecute<string>((Func<RedisClient, OperateResult<string>>)(m => m.IncrementKey(key, value)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ReadKey(System.String[])" />
        public OperateResult<string[]> ReadKey(string[] keys) => this.ConnectPoolExecute<string[]>((Func<RedisClient, OperateResult<string[]>>)(m => m.ReadKey(keys)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.WriteKey(System.String[],System.String[])" />
        public OperateResult WriteKey(string[] keys, string[] values) => this.ConnectPoolExecute((Func<RedisClient, OperateResult>)(m => m.WriteKey(keys, values)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.WriteKey(System.String,System.String)" />
        public OperateResult WriteKey(string key, string value) => this.ConnectPoolExecute((Func<RedisClient, OperateResult>)(m => m.WriteKey(key, value)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.WriteAndPublishKey(System.String,System.String)" />
        public OperateResult WriteAndPublishKey(string key, string value) => this.ConnectPoolExecute((Func<RedisClient, OperateResult>)(m => m.WriteAndPublishKey(key, value)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.WriteExpireKey(System.String,System.String,System.Int64)" />
        public OperateResult WriteExpireKey(string key, string value, long seconds) => this.ConnectPoolExecute((Func<RedisClient, OperateResult>)(m => m.WriteExpireKey(key, value, seconds)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.WriteKeyIfNotExists(System.String,System.String)" />
        public OperateResult<int> WriteKeyIfNotExists(string key, string value) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.WriteKeyIfNotExists(key, value)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.WriteKeyRange(System.String,System.String,System.Int32)" />
        public OperateResult<int> WriteKeyRange(string key, string value, int offset) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.WriteKeyRange(key, value, offset)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ReadKeyLength(System.String)" />
        public OperateResult<int> ReadKeyLength(string key) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.ReadKeyLength(key)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.AppendKey(System.String,System.String)" />
        public async Task<OperateResult<int>> AppendKeyAsync(string key, string value)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.AppendKeyAsync(key, value)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.DecrementKey(System.String)" />
        public async Task<OperateResult<long>> DecrementKeyAsync(string key)
        {
            OperateResult<long> operateResult = await this.ConnectPoolExecuteAsync<long>((Func<RedisClient, Task<OperateResult<long>>>)(m => m.DecrementKeyAsync(key)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.DecrementKey(System.String,System.Int64)" />
        public async Task<OperateResult<long>> DecrementKeyAsync(
          string key,
          long value)
        {
            OperateResult<long> operateResult = await this.ConnectPoolExecuteAsync<long>((Func<RedisClient, Task<OperateResult<long>>>)(m => m.DecrementKeyAsync(key, value)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ReadKey(System.String)" />
        public async Task<OperateResult<string>> ReadKeyAsync(string key)
        {
            OperateResult<string> operateResult = await this.ConnectPoolExecuteAsync<string>((Func<RedisClient, Task<OperateResult<string>>>)(m => m.ReadKeyAsync(key)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ReadKeyRange(System.String,System.Int32,System.Int32)" />
        public async Task<OperateResult<string>> ReadKeyRangeAsync(
          string key,
          int start,
          int end)
        {
            OperateResult<string> operateResult = await this.ConnectPoolExecuteAsync<string>((Func<RedisClient, Task<OperateResult<string>>>)(m => m.ReadKeyRangeAsync(key, start, end)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ReadAndWriteKey(System.String,System.String)" />
        public async Task<OperateResult<string>> ReadAndWriteKeyAsync(
          string key,
          string value)
        {
            OperateResult<string> operateResult = await this.ConnectPoolExecuteAsync<string>((Func<RedisClient, Task<OperateResult<string>>>)(m => m.ReadAndWriteKeyAsync(key, value)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.IncrementKey(System.String)" />
        public async Task<OperateResult<long>> IncrementKeyAsync(string key)
        {
            OperateResult<long> operateResult = await this.ConnectPoolExecuteAsync<long>((Func<RedisClient, Task<OperateResult<long>>>)(m => m.IncrementKeyAsync(key)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.IncrementKey(System.String,System.Int64)" />
        public async Task<OperateResult<long>> IncrementKeyAsync(
          string key,
          long value)
        {
            OperateResult<long> operateResult = await this.ConnectPoolExecuteAsync<long>((Func<RedisClient, Task<OperateResult<long>>>)(m => m.IncrementKeyAsync(key, value)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.IncrementKey(System.String,System.Single)" />
        public async Task<OperateResult<string>> IncrementKeyAsync(
          string key,
          float value)
        {
            OperateResult<string> operateResult = await this.ConnectPoolExecuteAsync<string>((Func<RedisClient, Task<OperateResult<string>>>)(m => m.IncrementKeyAsync(key, value)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ReadKey(System.String[])" />
        public async Task<OperateResult<string[]>> ReadKeyAsync(string[] keys)
        {
            OperateResult<string[]> operateResult = await this.ConnectPoolExecuteAsync<string[]>((Func<RedisClient, Task<OperateResult<string[]>>>)(m => m.ReadKeyAsync(keys)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.WriteKey(System.String[],System.String[])" />
        public async Task<OperateResult> WriteKeyAsync(string[] keys, string[] values)
        {
            OperateResult operateResult = await this.ConnectPoolExecuteAsync((Func<RedisClient, Task<OperateResult>>)(m => m.WriteKeyAsync(keys, values)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.WriteKey(System.String,System.String)" />
        public async Task<OperateResult> WriteKeyAsync(string key, string value)
        {
            OperateResult operateResult = await this.ConnectPoolExecuteAsync((Func<RedisClient, Task<OperateResult>>)(m => m.WriteKeyAsync(key, value)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.WriteAndPublishKey(System.String,System.String)" />
        public async Task<OperateResult> WriteAndPublishKeyAsync(
          string key,
          string value)
        {
            OperateResult operateResult = await this.ConnectPoolExecuteAsync((Func<RedisClient, Task<OperateResult>>)(m => m.WriteAndPublishKeyAsync(key, value)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.WriteExpireKey(System.String,System.String,System.Int64)" />
        public async Task<OperateResult> WriteExpireKeyAsync(
          string key,
          string value,
          long seconds)
        {
            OperateResult operateResult = await this.ConnectPoolExecuteAsync((Func<RedisClient, Task<OperateResult>>)(m => m.WriteExpireKeyAsync(key, value, seconds)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.WriteKeyIfNotExists(System.String,System.String)" />
        public async Task<OperateResult<int>> WriteKeyIfNotExistsAsync(
          string key,
          string value)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.WriteKeyIfNotExistsAsync(key, value)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.WriteKeyRange(System.String,System.String,System.Int32)" />
        public async Task<OperateResult<int>> WriteKeyRangeAsync(
          string key,
          string value,
          int offset)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.WriteKeyRangeAsync(key, value, offset)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ReadKeyLength(System.String)" />
        public async Task<OperateResult<int>> ReadKeyLengthAsync(string key)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.ReadKeyLengthAsync(key)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ListInsertBefore(System.String,System.String,System.String)" />
        public OperateResult<int> ListInsertBefore(string key, string value, string pivot) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.ListInsertBefore(key, value, pivot)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ListInsertAfter(System.String,System.String,System.String)" />
        public OperateResult<int> ListInsertAfter(string key, string value, string pivot) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.ListInsertAfter(key, value, pivot)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.GetListLength(System.String)" />
        public OperateResult<int> GetListLength(string key) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.GetListLength(key)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ReadListByIndex(System.String,System.Int64)" />
        public OperateResult<string> ReadListByIndex(string key, long index) => this.ConnectPoolExecute<string>((Func<RedisClient, OperateResult<string>>)(m => m.ReadListByIndex(key, index)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ListLeftPop(System.String)" />
        public OperateResult<string> ListLeftPop(string key) => this.ConnectPoolExecute<string>((Func<RedisClient, OperateResult<string>>)(m => m.ListLeftPop(key)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ListLeftPush(System.String,System.String)" />
        public OperateResult<int> ListLeftPush(string key, string value) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.ListLeftPush(key, value)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ListLeftPush(System.String,System.String[])" />
        public OperateResult<int> ListLeftPush(string key, string[] values) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.ListLeftPush(key, values)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ListLeftPushX(System.String,System.String)" />
        public OperateResult<int> ListLeftPushX(string key, string value) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.ListLeftPushX(key, value)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ListRange(System.String,System.Int64,System.Int64)" />
        public OperateResult<string[]> ListRange(string key, long start, long stop) => this.ConnectPoolExecute<string[]>((Func<RedisClient, OperateResult<string[]>>)(m => m.ListRange(key, start, stop)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ListRemoveElementMatch(System.String,System.Int64,System.String)" />
        public OperateResult<int> ListRemoveElementMatch(
          string key,
          long count,
          string value)
        {
            return this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.ListRemoveElementMatch(key, count, value)));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ListSet(System.String,System.Int64,System.String)" />
        public OperateResult ListSet(string key, long index, string value) => this.ConnectPoolExecute((Func<RedisClient, OperateResult>)(m => m.ListSet(key, index, value)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ListTrim(System.String,System.Int64,System.Int64)" />
        public OperateResult ListTrim(string key, long start, long end) => this.ConnectPoolExecute((Func<RedisClient, OperateResult>)(m => m.ListTrim(key, start, end)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ListRightPop(System.String)" />
        public OperateResult<string> ListRightPop(string key) => this.ConnectPoolExecute<string>((Func<RedisClient, OperateResult<string>>)(m => m.ListRightPop(key)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ListRightPopLeftPush(System.String,System.String)" />
        public OperateResult<string> ListRightPopLeftPush(string key1, string key2) => this.ConnectPoolExecute<string>((Func<RedisClient, OperateResult<string>>)(m => m.ListRightPopLeftPush(key1, key2)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ListRightPush(System.String,System.String)" />
        public OperateResult<int> ListRightPush(string key, string value) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.ListRightPush(key, value)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ListRightPush(System.String,System.String[])" />
        public OperateResult<int> ListRightPush(string key, string[] values) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.ListRightPush(key, values)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ListRightPushX(System.String,System.String)" />
        public OperateResult<int> ListRightPushX(string key, string value) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.ListRightPushX(key, value)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ListInsertBefore(System.String,System.String,System.String)" />
        public async Task<OperateResult<int>> ListInsertBeforeAsync(
          string key,
          string value,
          string pivot)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.ListInsertBeforeAsync(key, value, pivot)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ListInsertAfter(System.String,System.String,System.String)" />
        public async Task<OperateResult<int>> ListInsertAfterAsync(
          string key,
          string value,
          string pivot)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.ListInsertAfterAsync(key, value, pivot)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.GetListLength(System.String)" />
        public async Task<OperateResult<int>> GetListLengthAsync(string key)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.GetListLengthAsync(key)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ReadListByIndex(System.String,System.Int64)" />
        public async Task<OperateResult<string>> ReadListByIndexAsync(
          string key,
          long index)
        {
            OperateResult<string> operateResult = await this.ConnectPoolExecuteAsync<string>((Func<RedisClient, Task<OperateResult<string>>>)(m => m.ReadListByIndexAsync(key, index)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ListLeftPop(System.String)" />
        public async Task<OperateResult<string>> ListLeftPopAsync(string key)
        {
            OperateResult<string> operateResult = await this.ConnectPoolExecuteAsync<string>((Func<RedisClient, Task<OperateResult<string>>>)(m => m.ListLeftPopAsync(key)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ListLeftPush(System.String,System.String)" />
        public async Task<OperateResult<int>> ListLeftPushAsync(
          string key,
          string value)
        {
            OperateResult<int> operateResult = await this.ListLeftPushAsync(key, new string[1]
            {
        value
            });
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ListLeftPush(System.String,System.String[])" />
        public async Task<OperateResult<int>> ListLeftPushAsync(
          string key,
          string[] values)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.ListLeftPushAsync(key, values)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ListLeftPushX(System.String,System.String)" />
        public async Task<OperateResult<int>> ListLeftPushXAsync(
          string key,
          string value)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.ListLeftPushXAsync(key, value)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ListRange(System.String,System.Int64,System.Int64)" />
        public async Task<OperateResult<string[]>> ListRangeAsync(
          string key,
          long start,
          long stop)
        {
            OperateResult<string[]> operateResult = await this.ConnectPoolExecuteAsync<string[]>((Func<RedisClient, Task<OperateResult<string[]>>>)(m => m.ListRangeAsync(key, start, stop)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ListRemoveElementMatch(System.String,System.Int64,System.String)" />
        public async Task<OperateResult<int>> ListRemoveElementMatchAsync(
          string key,
          long count,
          string value)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.ListRemoveElementMatchAsync(key, count, value)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ListSet(System.String,System.Int64,System.String)" />
        public async Task<OperateResult> ListSetAsync(
          string key,
          long index,
          string value)
        {
            OperateResult operateResult = await this.ConnectPoolExecuteAsync((Func<RedisClient, Task<OperateResult>>)(m => m.ListSetAsync(key, index, value)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ListTrim(System.String,System.Int64,System.Int64)" />
        public async Task<OperateResult> ListTrimAsync(
          string key,
          long start,
          long end)
        {
            OperateResult operateResult = await this.ConnectPoolExecuteAsync((Func<RedisClient, Task<OperateResult>>)(m => m.ListTrimAsync(key, start, end)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ListRightPop(System.String)" />
        public async Task<OperateResult<string>> ListRightPopAsync(string key)
        {
            OperateResult<string> operateResult = await this.ConnectPoolExecuteAsync<string>((Func<RedisClient, Task<OperateResult<string>>>)(m => m.ListRightPopAsync(key)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ListRightPopLeftPush(System.String,System.String)" />
        public async Task<OperateResult<string>> ListRightPopLeftPushAsync(
          string key1,
          string key2)
        {
            OperateResult<string> operateResult = await this.ConnectPoolExecuteAsync<string>((Func<RedisClient, Task<OperateResult<string>>>)(m => m.ListRightPopLeftPushAsync(key1, key2)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ListRightPush(System.String,System.String)" />
        public async Task<OperateResult<int>> ListRightPushAsync(
          string key,
          string value)
        {
            OperateResult<int> operateResult = await this.ListRightPushAsync(key, new string[1]
            {
        value
            });
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ListRightPush(System.String,System.String[])" />
        public async Task<OperateResult<int>> ListRightPushAsync(
          string key,
          string[] values)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.ListRightPushAsync(key, values)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ListRightPushX(System.String,System.String)" />
        public async Task<OperateResult<int>> ListRightPushXAsync(
          string key,
          string value)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.ListRightPushXAsync(key, value)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.DeleteHashKey(System.String,System.String)" />
        public OperateResult<int> DeleteHashKey(string key, string field) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.DeleteHashKey(key, field)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.DeleteHashKey(System.String,System.String[])" />
        public OperateResult<int> DeleteHashKey(string key, string[] fields) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.DeleteHashKey(key, fields)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ExistsHashKey(System.String,System.String)" />
        public OperateResult<int> ExistsHashKey(string key, string field) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.ExistsHashKey(key, field)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ReadHashKey(System.String,System.String)" />
        public OperateResult<string> ReadHashKey(string key, string field) => this.ConnectPoolExecute<string>((Func<RedisClient, OperateResult<string>>)(m => m.ReadHashKey(key, field)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ReadHashKeyAll(System.String)" />
        public OperateResult<string[]> ReadHashKeyAll(string key) => this.ConnectPoolExecute<string[]>((Func<RedisClient, OperateResult<string[]>>)(m => m.ReadHashKeyAll(key)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.IncrementHashKey(System.String,System.String,System.Int64)" />
        public OperateResult<long> IncrementHashKey(string key, string field, long value) => this.ConnectPoolExecute<long>((Func<RedisClient, OperateResult<long>>)(m => m.IncrementHashKey(key, field, value)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.IncrementHashKey(System.String,System.String,System.Single)" />
        public OperateResult<string> IncrementHashKey(
          string key,
          string field,
          float value)
        {
            return this.ConnectPoolExecute<string>((Func<RedisClient, OperateResult<string>>)(m => m.IncrementHashKey(key, field, value)));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ReadHashKeys(System.String)" />
        public OperateResult<string[]> ReadHashKeys(string key) => this.ConnectPoolExecute<string[]>((Func<RedisClient, OperateResult<string[]>>)(m => m.ReadHashKeys(key)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ReadHashKeyLength(System.String)" />
        public OperateResult<int> ReadHashKeyLength(string key) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.ReadHashKeyLength(key)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ReadHashKey(System.String,System.String[])" />
        public OperateResult<string[]> ReadHashKey(string key, string[] fields) => this.ConnectPoolExecute<string[]>((Func<RedisClient, OperateResult<string[]>>)(m => m.ReadHashKey(key, fields)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.WriteHashKey(System.String,System.String,System.String)" />
        public OperateResult<int> WriteHashKey(string key, string field, string value) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.WriteHashKey(key, field, value)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.WriteHashKey(System.String,System.String[],System.String[])" />
        public OperateResult WriteHashKey(string key, string[] fields, string[] values) => this.ConnectPoolExecute((Func<RedisClient, OperateResult>)(m => m.WriteHashKey(key, fields, values)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.WriteHashKeyNx(System.String,System.String,System.String)" />
        public OperateResult<int> WriteHashKeyNx(string key, string field, string value) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.WriteHashKeyNx(key, field, value)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ReadHashValues(System.String)" />
        public OperateResult<string[]> ReadHashValues(string key) => this.ConnectPoolExecute<string[]>((Func<RedisClient, OperateResult<string[]>>)(m => m.ReadHashValues(key)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.DeleteHashKey(System.String,System.String)" />
        public async Task<OperateResult<int>> DeleteHashKeyAsync(
          string key,
          string field)
        {
            OperateResult<int> operateResult = await this.DeleteHashKeyAsync(key, new string[1]
            {
        field
            });
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.DeleteHashKey(System.String,System.String[])" />
        public async Task<OperateResult<int>> DeleteHashKeyAsync(
          string key,
          string[] fields)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.DeleteHashKeyAsync(key, fields)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ExistsHashKey(System.String,System.String)" />
        public async Task<OperateResult<int>> ExistsHashKeyAsync(
          string key,
          string field)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.ExistsHashKeyAsync(key, field)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ReadHashKey(System.String,System.String)" />
        public async Task<OperateResult<string>> ReadHashKeyAsync(
          string key,
          string field)
        {
            OperateResult<string> operateResult = await this.ConnectPoolExecuteAsync<string>((Func<RedisClient, Task<OperateResult<string>>>)(m => m.ReadHashKeyAsync(key, field)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ReadHashKeyAll(System.String)" />
        public async Task<OperateResult<string[]>> ReadHashKeyAllAsync(string key)
        {
            OperateResult<string[]> operateResult = await this.ConnectPoolExecuteAsync<string[]>((Func<RedisClient, Task<OperateResult<string[]>>>)(m => m.ReadHashKeyAllAsync(key)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.IncrementHashKey(System.String,System.String,System.Int64)" />
        public async Task<OperateResult<long>> IncrementHashKeyAsync(
          string key,
          string field,
          long value)
        {
            OperateResult<long> operateResult = await this.ConnectPoolExecuteAsync<long>((Func<RedisClient, Task<OperateResult<long>>>)(m => m.IncrementHashKeyAsync(key, field, value)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.IncrementHashKey(System.String,System.String,System.Single)" />
        public async Task<OperateResult<string>> IncrementHashKeyAsync(
          string key,
          string field,
          float value)
        {
            OperateResult<string> operateResult = await this.ConnectPoolExecuteAsync<string>((Func<RedisClient, Task<OperateResult<string>>>)(m => m.IncrementHashKeyAsync(key, field, value)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ReadHashKeys(System.String)" />
        public async Task<OperateResult<string[]>> ReadHashKeysAsync(string key)
        {
            OperateResult<string[]> operateResult = await this.ConnectPoolExecuteAsync<string[]>((Func<RedisClient, Task<OperateResult<string[]>>>)(m => m.ReadHashKeysAsync(key)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ReadHashKeyLength(System.String)" />
        public async Task<OperateResult<int>> ReadHashKeyLengthAsync(string key)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.ReadHashKeyLengthAsync(key)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ReadHashKey(System.String,System.String[])" />
        public async Task<OperateResult<string[]>> ReadHashKeyAsync(
          string key,
          string[] fields)
        {
            OperateResult<string[]> operateResult = await this.ConnectPoolExecuteAsync<string[]>((Func<RedisClient, Task<OperateResult<string[]>>>)(m => m.ReadHashKeyAsync(key, fields)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.WriteHashKey(System.String,System.String,System.String)" />
        public async Task<OperateResult<int>> WriteHashKeyAsync(
          string key,
          string field,
          string value)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.WriteHashKeyAsync(key, field, value)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.WriteHashKey(System.String,System.String[],System.String[])" />
        public async Task<OperateResult> WriteHashKeyAsync(
          string key,
          string[] fields,
          string[] values)
        {
            OperateResult operateResult = await this.ConnectPoolExecuteAsync((Func<RedisClient, Task<OperateResult>>)(m => m.WriteHashKeyAsync(key, fields, values)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.WriteHashKeyNx(System.String,System.String,System.String)" />
        public async Task<OperateResult<int>> WriteHashKeyNxAsync(
          string key,
          string field,
          string value)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.WriteHashKeyNxAsync(key, field, value)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ReadHashValues(System.String)" />
        public async Task<OperateResult<string[]>> ReadHashValuesAsync(string key)
        {
            OperateResult<string[]> operateResult = await this.ConnectPoolExecuteAsync<string[]>((Func<RedisClient, Task<OperateResult<string[]>>>)(m => m.ReadHashValuesAsync(key)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.SetAdd(System.String,System.String)" />
        public OperateResult<int> SetAdd(string key, string member) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.SetAdd(key, member)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.SetAdd(System.String,System.String[])" />
        public OperateResult<int> SetAdd(string key, string[] members) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.SetAdd(key, members)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.SetCard(System.String)" />
        public OperateResult<int> SetCard(string key) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.SetCard(key)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.SetDiff(System.String,System.String)" />
        public OperateResult<string[]> SetDiff(string key, string diffKey) => this.ConnectPoolExecute<string[]>((Func<RedisClient, OperateResult<string[]>>)(m => m.SetDiff(key, diffKey)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.SetDiff(System.String,System.String[])" />
        public OperateResult<string[]> SetDiff(string key, string[] diffKeys) => this.ConnectPoolExecute<string[]>((Func<RedisClient, OperateResult<string[]>>)(m => m.SetDiff(key, diffKeys)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.SetDiffStore(System.String,System.String,System.String)" />
        public OperateResult<int> SetDiffStore(
          string destination,
          string key,
          string diffKey)
        {
            return this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.SetDiffStore(destination, key, diffKey)));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.SetDiffStore(System.String,System.String,System.String[])" />
        public OperateResult<int> SetDiffStore(
          string destination,
          string key,
          string[] diffKeys)
        {
            return this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.SetDiffStore(destination, key, diffKeys)));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.SetInter(System.String,System.String)" />
        public OperateResult<string[]> SetInter(string key, string interKey) => this.ConnectPoolExecute<string[]>((Func<RedisClient, OperateResult<string[]>>)(m => m.SetInter(key, interKey)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.SetInter(System.String,System.String[])" />
        public OperateResult<string[]> SetInter(string key, string[] interKeys) => this.ConnectPoolExecute<string[]>((Func<RedisClient, OperateResult<string[]>>)(m => m.SetInter(key, interKeys)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.SetInterStore(System.String,System.String,System.String)" />
        public OperateResult<int> SetInterStore(
          string destination,
          string key,
          string interKey)
        {
            return this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.SetInterStore(destination, key, interKey)));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.SetInterStore(System.String,System.String,System.String[])" />
        public OperateResult<int> SetInterStore(
          string destination,
          string key,
          string[] interKeys)
        {
            return this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.SetInterStore(destination, key, interKeys)));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.SetIsMember(System.String,System.String)" />
        public OperateResult<int> SetIsMember(string key, string member) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.SetIsMember(key, member)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.SetMembers(System.String)" />
        public OperateResult<string[]> SetMembers(string key) => this.ConnectPoolExecute<string[]>((Func<RedisClient, OperateResult<string[]>>)(m => m.SetMembers(key)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.SetMove(System.String,System.String,System.String)" />
        public OperateResult<int> SetMove(
          string source,
          string destination,
          string member)
        {
            return this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.SetMove(source, destination, member)));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.SetPop(System.String)" />
        public OperateResult<string> SetPop(string key) => this.ConnectPoolExecute<string>((Func<RedisClient, OperateResult<string>>)(m => m.SetPop(key)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.SetRandomMember(System.String)" />
        public OperateResult<string> SetRandomMember(string key) => this.ConnectPoolExecute<string>((Func<RedisClient, OperateResult<string>>)(m => m.SetRandomMember(key)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.SetRandomMember(System.String,System.Int32)" />
        public OperateResult<string[]> SetRandomMember(string key, int count) => this.ConnectPoolExecute<string[]>((Func<RedisClient, OperateResult<string[]>>)(m => m.SetRandomMember(key, count)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.SetRemove(System.String,System.String)" />
        public OperateResult<int> SetRemove(string key, string member) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.SetRemove(key, member)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.SetRemove(System.String,System.String[])" />
        public OperateResult<int> SetRemove(string key, string[] members) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.SetRemove(key, members)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.SetUnion(System.String,System.String)" />
        public OperateResult<string[]> SetUnion(string key, string unionKey) => this.ConnectPoolExecute<string[]>((Func<RedisClient, OperateResult<string[]>>)(m => m.SetUnion(key, unionKey)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.SetUnion(System.String,System.String[])" />
        public OperateResult<string[]> SetUnion(string key, string[] unionKeys) => this.ConnectPoolExecute<string[]>((Func<RedisClient, OperateResult<string[]>>)(m => m.SetUnion(key, unionKeys)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.SetUnionStore(System.String,System.String,System.String)" />
        public OperateResult<int> SetUnionStore(
          string destination,
          string key,
          string unionKey)
        {
            return this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.SetUnionStore(destination, key, unionKey)));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.SetUnionStore(System.String,System.String,System.String[])" />
        public OperateResult<int> SetUnionStore(
          string destination,
          string key,
          string[] unionKeys)
        {
            return this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.SetUnionStore(destination, key, unionKeys)));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.SetAdd(System.String,System.String)" />
        public async Task<OperateResult<int>> SetAddAsync(string key, string member)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.SetAddAsync(key, member)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.SetAdd(System.String,System.String[])" />
        public async Task<OperateResult<int>> SetAddAsync(string key, string[] members)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.SetAddAsync(key, members)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.SetCard(System.String)" />
        public async Task<OperateResult<int>> SetCardAsync(string key)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.SetCardAsync(key)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.SetDiff(System.String,System.String)" />
        public async Task<OperateResult<string[]>> SetDiffAsync(
          string key,
          string diffKey)
        {
            OperateResult<string[]> operateResult = await this.ConnectPoolExecuteAsync<string[]>((Func<RedisClient, Task<OperateResult<string[]>>>)(m => m.SetDiffAsync(key, diffKey)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.SetDiff(System.String,System.String[])" />
        public async Task<OperateResult<string[]>> SetDiffAsync(
          string key,
          string[] diffKeys)
        {
            OperateResult<string[]> operateResult = await this.ConnectPoolExecuteAsync<string[]>((Func<RedisClient, Task<OperateResult<string[]>>>)(m => m.SetDiffAsync(key, diffKeys)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.SetDiffStore(System.String,System.String,System.String)" />
        public async Task<OperateResult<int>> SetDiffStoreAsync(
          string destination,
          string key,
          string diffKey)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.SetDiffStoreAsync(destination, key, diffKey)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.SetDiffStore(System.String,System.String,System.String[])" />
        public async Task<OperateResult<int>> SetDiffStoreAsync(
          string destination,
          string key,
          string[] diffKeys)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.SetDiffStoreAsync(destination, key, diffKeys)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.SetInter(System.String,System.String)" />
        public async Task<OperateResult<string[]>> SetInterAsync(
          string key,
          string interKey)
        {
            OperateResult<string[]> operateResult = await this.ConnectPoolExecuteAsync<string[]>((Func<RedisClient, Task<OperateResult<string[]>>>)(m => m.SetInterAsync(key, interKey)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.SetInter(System.String,System.String[])" />
        public async Task<OperateResult<string[]>> SetInterAsync(
          string key,
          string[] interKeys)
        {
            OperateResult<string[]> operateResult = await this.ConnectPoolExecuteAsync<string[]>((Func<RedisClient, Task<OperateResult<string[]>>>)(m => m.SetInterAsync(key, interKeys)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.SetInterStore(System.String,System.String,System.String)" />
        public async Task<OperateResult<int>> SetInterStoreAsync(
          string destination,
          string key,
          string interKey)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.SetInterStoreAsync(destination, key, interKey)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.SetInterStore(System.String,System.String,System.String[])" />
        public async Task<OperateResult<int>> SetInterStoreAsync(
          string destination,
          string key,
          string[] interKeys)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.SetInterStoreAsync(destination, key, interKeys)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.SetIsMember(System.String,System.String)" />
        public async Task<OperateResult<int>> SetIsMemberAsync(
          string key,
          string member)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.SetIsMemberAsync(key, member)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.SetMembers(System.String)" />
        public async Task<OperateResult<string[]>> SetMembersAsync(string key)
        {
            OperateResult<string[]> operateResult = await this.ConnectPoolExecuteAsync<string[]>((Func<RedisClient, Task<OperateResult<string[]>>>)(m => m.SetMembersAsync(key)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.SetMove(System.String,System.String,System.String)" />
        public async Task<OperateResult<int>> SetMoveAsync(
          string source,
          string destination,
          string member)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.SetMoveAsync(source, destination, member)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.SetPop(System.String)" />
        public async Task<OperateResult<string>> SetPopAsync(string key)
        {
            OperateResult<string> operateResult = await this.ConnectPoolExecuteAsync<string>((Func<RedisClient, Task<OperateResult<string>>>)(m => m.SetPopAsync(key)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.SetRandomMember(System.String)" />
        public async Task<OperateResult<string>> SetRandomMemberAsync(string key)
        {
            OperateResult<string> operateResult = await this.ConnectPoolExecuteAsync<string>((Func<RedisClient, Task<OperateResult<string>>>)(m => m.SetRandomMemberAsync(key)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.SetRandomMember(System.String,System.Int32)" />
        public async Task<OperateResult<string[]>> SetRandomMemberAsync(
          string key,
          int count)
        {
            OperateResult<string[]> operateResult = await this.ConnectPoolExecuteAsync<string[]>((Func<RedisClient, Task<OperateResult<string[]>>>)(m => m.SetRandomMemberAsync(key, count)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.SetRemove(System.String,System.String)" />
        public async Task<OperateResult<int>> SetRemoveAsync(string key, string member)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.SetRemoveAsync(key, member)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.SetRemove(System.String,System.String[])" />
        public async Task<OperateResult<int>> SetRemoveAsync(
          string key,
          string[] members)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.SetRemoveAsync(key, members)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.SetUnion(System.String,System.String)" />
        public async Task<OperateResult<string[]>> SetUnionAsync(
          string key,
          string unionKey)
        {
            OperateResult<string[]> operateResult = await this.ConnectPoolExecuteAsync<string[]>((Func<RedisClient, Task<OperateResult<string[]>>>)(m => m.SetUnionAsync(key, unionKey)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.SetUnion(System.String,System.String[])" />
        public async Task<OperateResult<string[]>> SetUnionAsync(
          string key,
          string[] unionKeys)
        {
            OperateResult<string[]> operateResult = await this.ConnectPoolExecuteAsync<string[]>((Func<RedisClient, Task<OperateResult<string[]>>>)(m => m.SetUnionAsync(key, unionKeys)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.SetUnionStore(System.String,System.String,System.String)" />
        public async Task<OperateResult<int>> SetUnionStoreAsync(
          string destination,
          string key,
          string unionKey)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.SetUnionStoreAsync(destination, key, unionKey)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.SetUnionStore(System.String,System.String,System.String[])" />
        public async Task<OperateResult<int>> SetUnionStoreAsync(
          string destination,
          string key,
          string[] unionKeys)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.SetUnionStoreAsync(destination, key, unionKeys)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ZSetAdd(System.String,System.String,System.Double)" />
        public OperateResult<int> ZSetAdd(string key, string member, double score) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.ZSetAdd(key, member, score)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ZSetAdd(System.String,System.String[],System.Double[])" />
        public OperateResult<int> ZSetAdd(string key, string[] members, double[] scores) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.ZSetAdd(key, members, scores)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ZSetCard(System.String)" />
        public OperateResult<int> ZSetCard(string key) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.ZSetCard(key)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ZSetCount(System.String,System.Double,System.Double)" />
        public OperateResult<int> ZSetCount(string key, double min, double max) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.ZSetCount(key, min, max)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ZSetIncreaseBy(System.String,System.String,System.Double)" />
        public OperateResult<string> ZSetIncreaseBy(
          string key,
          string member,
          double increment)
        {
            return this.ConnectPoolExecute<string>((Func<RedisClient, OperateResult<string>>)(m => m.ZSetIncreaseBy(key, member, increment)));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ZSetRange(System.String,System.Int32,System.Int32,System.Boolean)" />
        public OperateResult<string[]> ZSetRange(
          string key,
          int start,
          int stop,
          bool withScore = false)
        {
            return this.ConnectPoolExecute<string[]>((Func<RedisClient, OperateResult<string[]>>)(m => m.ZSetRange(key, start, stop, withScore)));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ZSetRangeByScore(System.String,System.String,System.String,System.Boolean)" />
        public OperateResult<string[]> ZSetRangeByScore(
          string key,
          string min,
          string max,
          bool withScore = false)
        {
            return this.ConnectPoolExecute<string[]>((Func<RedisClient, OperateResult<string[]>>)(m => m.ZSetRangeByScore(key, min, max, withScore)));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ZSetRank(System.String,System.String)" />
        public OperateResult<int> ZSetRank(string key, string member) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.ZSetRank(key, member)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ZSetRemove(System.String,System.String)" />
        public OperateResult<int> ZSetRemove(string key, string member) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.ZSetRemove(key, member)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ZSetRemove(System.String,System.String[])" />
        public OperateResult<int> ZSetRemove(string key, string[] members) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.ZSetRemove(key, members)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ZSetRemoveRangeByRank(System.String,System.Int32,System.Int32)" />
        public OperateResult<int> ZSetRemoveRangeByRank(string key, int start, int stop) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.ZSetRemoveRangeByRank(key, start, stop)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ZSetRemoveRangeByScore(System.String,System.String,System.String)" />
        public OperateResult<int> ZSetRemoveRangeByScore(
          string key,
          string min,
          string max)
        {
            return this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.ZSetRemoveRangeByScore(key, min, max)));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ZSetReverseRange(System.String,System.Int32,System.Int32,System.Boolean)" />
        public OperateResult<string[]> ZSetReverseRange(
          string key,
          int start,
          int stop,
          bool withScore = false)
        {
            return this.ConnectPoolExecute<string[]>((Func<RedisClient, OperateResult<string[]>>)(m => m.ZSetReverseRange(key, start, stop, withScore)));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ZSetReverseRangeByScore(System.String,System.String,System.String,System.Boolean)" />
        public OperateResult<string[]> ZSetReverseRangeByScore(
          string key,
          string max,
          string min,
          bool withScore = false)
        {
            return this.ConnectPoolExecute<string[]>((Func<RedisClient, OperateResult<string[]>>)(m => m.ZSetReverseRangeByScore(key, max, min, withScore)));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ZSetReverseRank(System.String,System.String)" />
        public OperateResult<int> ZSetReverseRank(string key, string member) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.ZSetReverseRank(key, member)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ZSetScore(System.String,System.String)" />
        public OperateResult<string> ZSetScore(string key, string member) => this.ConnectPoolExecute<string>((Func<RedisClient, OperateResult<string>>)(m => m.ZSetScore(key, member)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ZSetAdd(System.String,System.String,System.Double)" />
        public async Task<OperateResult<int>> ZSetAddAsync(
          string key,
          string member,
          double score)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.ZSetAddAsync(key, member, score)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ZSetAdd(System.String,System.String[],System.Double[])" />
        public async Task<OperateResult<int>> ZSetAddAsync(
          string key,
          string[] members,
          double[] scores)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.ZSetAddAsync(key, members, scores)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ZSetCard(System.String)" />
        public async Task<OperateResult<int>> ZSetCardAsync(string key)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.ZSetCardAsync(key)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ZSetCount(System.String,System.Double,System.Double)" />
        public async Task<OperateResult<int>> ZSetCountAsync(
          string key,
          double min,
          double max)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.ZSetCountAsync(key, min, max)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ZSetIncreaseBy(System.String,System.String,System.Double)" />
        public async Task<OperateResult<string>> ZSetIncreaseByAsync(
          string key,
          string member,
          double increment)
        {
            OperateResult<string> operateResult = await this.ConnectPoolExecuteAsync<string>((Func<RedisClient, Task<OperateResult<string>>>)(m => m.ZSetIncreaseByAsync(key, member, increment)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ZSetRange(System.String,System.Int32,System.Int32,System.Boolean)" />
        public async Task<OperateResult<string[]>> ZSetRangeAsync(
          string key,
          int start,
          int stop,
          bool withScore = false)
        {
            OperateResult<string[]> operateResult = await this.ConnectPoolExecuteAsync<string[]>((Func<RedisClient, Task<OperateResult<string[]>>>)(m => m.ZSetRangeAsync(key, start, stop, withScore)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ZSetRangeByScore(System.String,System.String,System.String,System.Boolean)" />
        public async Task<OperateResult<string[]>> ZSetRangeByScoreAsync(
          string key,
          string min,
          string max,
          bool withScore = false)
        {
            OperateResult<string[]> operateResult = await this.ConnectPoolExecuteAsync<string[]>((Func<RedisClient, Task<OperateResult<string[]>>>)(m => m.ZSetRangeByScoreAsync(key, min, max, withScore)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ZSetRank(System.String,System.String)" />
        public async Task<OperateResult<int>> ZSetRankAsync(string key, string member)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.ZSetRankAsync(key, member)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ZSetRemove(System.String,System.String)" />
        public async Task<OperateResult<int>> ZSetRemoveAsync(
          string key,
          string member)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.ZSetRemoveAsync(key, member)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ZSetRemove(System.String,System.String[])" />
        public async Task<OperateResult<int>> ZSetRemoveAsync(
          string key,
          string[] members)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.ZSetRemoveAsync(key, members)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ZSetRemoveRangeByRank(System.String,System.Int32,System.Int32)" />
        public async Task<OperateResult<int>> ZSetRemoveRangeByRankAsync(
          string key,
          int start,
          int stop)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.ZSetRemoveRangeByRankAsync(key, start, stop)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ZSetRemoveRangeByScore(System.String,System.String,System.String)" />
        public async Task<OperateResult<int>> ZSetRemoveRangeByScoreAsync(
          string key,
          string min,
          string max)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.ZSetRemoveRangeByScoreAsync(key, min, max)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ZSetReverseRange(System.String,System.Int32,System.Int32,System.Boolean)" />
        public async Task<OperateResult<string[]>> ZSetReverseRangeAsync(
          string key,
          int start,
          int stop,
          bool withScore = false)
        {
            OperateResult<string[]> operateResult = await this.ConnectPoolExecuteAsync<string[]>((Func<RedisClient, Task<OperateResult<string[]>>>)(m => m.ZSetReverseRangeAsync(key, start, stop, withScore)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ZSetReverseRangeByScore(System.String,System.String,System.String,System.Boolean)" />
        public async Task<OperateResult<string[]>> ZSetReverseRangeByScoreAsync(
          string key,
          string max,
          string min,
          bool withScore = false)
        {
            OperateResult<string[]> operateResult = await this.ConnectPoolExecuteAsync<string[]>((Func<RedisClient, Task<OperateResult<string[]>>>)(m => m.ZSetReverseRangeByScoreAsync(key, max, min, withScore)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ZSetReverseRank(System.String,System.String)" />
        public async Task<OperateResult<int>> ZSetReverseRankAsync(
          string key,
          string member)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.ZSetReverseRankAsync(key, member)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ZSetScore(System.String,System.String)" />
        public async Task<OperateResult<string>> ZSetScoreAsync(
          string key,
          string member)
        {
            OperateResult<string> operateResult = await this.ConnectPoolExecuteAsync<string>((Func<RedisClient, Task<OperateResult<string>>>)(m => m.ZSetScoreAsync(key, member)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.Read``1" />
        public OperateResult<T> Read<T>() where T : class, new() => this.ConnectPoolExecute<T>((Func<RedisClient, OperateResult<T>>)(m => m.Read<T>()));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.Write``1(``0)" />
        public OperateResult Write<T>(T data) where T : class, new() => this.ConnectPoolExecute((Func<RedisClient, OperateResult>)(m => m.Write<T>(data)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.Read``1" />
        public async Task<OperateResult<T>> ReadAsync<T>() where T : class, new()
        {
            OperateResult<T> operateResult = await this.ConnectPoolExecuteAsync<T>((Func<RedisClient, Task<OperateResult<T>>>)(m => m.ReadAsync<T>()));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.Write``1(``0)" />
        public async Task<OperateResult> WriteAsync<T>(T data) where T : class, new()
        {
            OperateResult operateResult = await this.ConnectPoolExecuteAsync((Func<RedisClient, Task<OperateResult>>)(m => m.WriteAsync<T>(data)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.Save" />
        public OperateResult Save() => this.ConnectPoolExecute((Func<RedisClient, OperateResult>)(m => m.Save()));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.SaveAsync" />
        public OperateResult SaveAsync() => this.ConnectPoolExecute((Func<RedisClient, OperateResult>)(m => m.SaveAsync()));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ReadServerTime" />
        public OperateResult<DateTime> ReadServerTime() => this.ConnectPoolExecute<DateTime>((Func<RedisClient, OperateResult<DateTime>>)(m => m.ReadServerTime()));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.Ping" />
        public OperateResult Ping() => this.ConnectPoolExecute((Func<RedisClient, OperateResult>)(m => m.Ping()));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.DBSize" />
        public OperateResult<long> DBSize() => this.ConnectPoolExecute<long>((Func<RedisClient, OperateResult<long>>)(m => m.DBSize()));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.FlushDB" />
        public OperateResult FlushDB() => this.ConnectPoolExecute((Func<RedisClient, OperateResult>)(m => m.FlushDB()));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.ChangePassword(System.String)" />
        public OperateResult ChangePassword(string password) => this.ConnectPoolExecute((Func<RedisClient, OperateResult>)(m => m.ChangePassword(password)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ReadServerTime" />
        public async Task<OperateResult<DateTime>> ReadServerTimeAsync()
        {
            OperateResult<DateTime> operateResult = await this.ConnectPoolExecuteAsync<DateTime>((Func<RedisClient, Task<OperateResult<DateTime>>>)(m => m.ReadServerTimeAsync()));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.Ping" />
        public async Task<OperateResult> PingAsync()
        {
            OperateResult operateResult = await this.ConnectPoolExecuteAsync((Func<RedisClient, Task<OperateResult>>)(m => m.PingAsync()));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.DBSize" />
        public async Task<OperateResult<long>> DBSizeAsync()
        {
            OperateResult<long> operateResult = await this.ConnectPoolExecuteAsync<long>((Func<RedisClient, Task<OperateResult<long>>>)(m => m.DBSizeAsync()));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.FlushDB" />
        public async Task<OperateResult> FlushDBAsync()
        {
            OperateResult operateResult = await this.ConnectPoolExecuteAsync((Func<RedisClient, Task<OperateResult>>)(m => m.FlushDBAsync()));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.ChangePassword(System.String)" />
        public async Task<OperateResult> ChangePasswordAsync(string password)
        {
            OperateResult operateResult = await this.ConnectPoolExecuteAsync((Func<RedisClient, Task<OperateResult>>)(m => m.ChangePasswordAsync(password)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.Publish(System.String,System.String)" />
        public OperateResult<int> Publish(string channel, string message) => this.ConnectPoolExecute<int>((Func<RedisClient, OperateResult<int>>)(m => m.Publish(channel, message)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.Publish(System.String,System.String)" />
        public async Task<OperateResult<int>> PublishAsync(
          string channel,
          string message)
        {
            OperateResult<int> operateResult = await this.ConnectPoolExecuteAsync<int>((Func<RedisClient, Task<OperateResult<int>>>)(m => m.PublishAsync(channel, message)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClient.SelectDB(System.Int32)" />
        public OperateResult SelectDB(int db) => this.ConnectPoolExecute((Func<RedisClient, OperateResult>)(m => m.SelectDB(db)));

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.Redis.RedisClientPool.SelectDB(System.Int32)" />
        public async Task<OperateResult> SelectDBAsync(int db)
        {
            OperateResult operateResult = await this.ConnectPoolExecuteAsync((Func<RedisClient, Task<OperateResult>>)(m => m.SelectDBAsync(db)));
            return operateResult;
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("RedisConnectPool[{0}]", (object)this.redisConnectPool.MaxConnector);
    }
}
