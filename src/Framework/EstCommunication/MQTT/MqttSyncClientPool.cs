// Decompiled with JetBrains decompiler
// Type: EstCommunication.MQTT.MqttSyncClientPool
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using System;
using System.Threading.Tasks;

namespace EstCommunication.MQTT
{
  /// <summary>
  /// <b>[商业授权]</b> MqttSyncClient客户端的连接池类对象，用于共享当前的连接池，合理的动态调整连接对象，然后进行高效通信的操作，默认连接数无限大。<br />
  /// <b>[Authorization]</b> The connection pool class object of the MqttSyncClient is used to share the current connection pool,
  /// reasonably dynamically adjust the connection object, and then perform efficient communication operations,
  /// The default number of connections is unlimited
  /// </summary>
  /// <remarks>
  /// 本连接池用于提供高并发的读写性能，仅对商业授权用户开放。使用起来和<see cref="T:EstCommunication.MQTT.MqttSyncClient" />一致，但是更加的高性能，在密集型数据交互时，优势尤为明显。
  /// </remarks>
  public class MqttSyncClientPool
  {
    private MqttConnectionOptions connectionOptions;
    private EstCommunication.Algorithms.ConnectPool.ConnectPool<IMqttSyncConnector> mqttConnectPool;

    /// <summary>
    /// 通过MQTT连接参数实例化一个对象<br />
    /// Instantiate an object through MQTT connection parameters
    /// </summary>
    /// <param name="options">MQTT的连接参数信息</param>
    public MqttSyncClientPool(MqttConnectionOptions options)
    {
      this.connectionOptions = options;
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        throw new Exception(StringResources.Language.InsufficientPrivileges);
      this.mqttConnectPool = new EstCommunication.Algorithms.ConnectPool.ConnectPool<IMqttSyncConnector>((Func<IMqttSyncConnector>) (() => new IMqttSyncConnector(options)));
      this.mqttConnectPool.MaxConnector = int.MaxValue;
    }

    /// <summary>
    /// 通过MQTT连接参数以及自定义的初始化方法来实例化一个对象<br />
    /// Instantiate an object through MQTT connection parameters and custom initialization methods
    /// </summary>
    /// <param name="options">MQTT的连接参数信息</param>
    /// <param name="initialize">自定义的初始化方法</param>
    public MqttSyncClientPool(MqttConnectionOptions options, Action<MqttSyncClient> initialize)
    {
      this.connectionOptions = options;
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        throw new Exception(StringResources.Language.InsufficientPrivileges);
      this.mqttConnectPool = new EstCommunication.Algorithms.ConnectPool.ConnectPool<IMqttSyncConnector>((Func<IMqttSyncConnector>) (() =>
      {
        MqttSyncClient mqttSyncClient = new MqttSyncClient(options);
        initialize(mqttSyncClient);
        return new IMqttSyncConnector()
        {
          SyncClient = mqttSyncClient
        };
      }));
      this.mqttConnectPool.MaxConnector = int.MaxValue;
    }

    /// <summary>
    /// 获取当前的连接池管理对象信息<br />
    /// Get current connection pool management object information
    /// </summary>
    public EstCommunication.Algorithms.ConnectPool.ConnectPool<IMqttSyncConnector> GetMqttSyncConnectPool => this.mqttConnectPool;

    /// <inheritdoc cref="P:EstCommunication.Algorithms.ConnectPool.ConnectPool`1.MaxConnector" />
    public int MaxConnector
    {
      get => this.mqttConnectPool.MaxConnector;
      set => this.mqttConnectPool.MaxConnector = value;
    }

    private OperateResult<T> ConnectPoolExecute<T>(
      Func<MqttSyncClient, OperateResult<T>> exec)
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        throw new Exception(StringResources.Language.InsufficientPrivileges);
      IMqttSyncConnector availableConnector = this.mqttConnectPool.GetAvailableConnector();
      OperateResult<T> operateResult = exec(availableConnector.SyncClient);
      this.mqttConnectPool.ReturnConnector(availableConnector);
      return operateResult;
    }

    private OperateResult<T1, T2> ConnectPoolExecute<T1, T2>(
      Func<MqttSyncClient, OperateResult<T1, T2>> exec)
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        throw new Exception(StringResources.Language.InsufficientPrivileges);
      IMqttSyncConnector availableConnector = this.mqttConnectPool.GetAvailableConnector();
      OperateResult<T1, T2> operateResult = exec(availableConnector.SyncClient);
      this.mqttConnectPool.ReturnConnector(availableConnector);
      return operateResult;
    }

    private async Task<OperateResult<T>> ConnectPoolExecuteAsync<T>(
      Func<MqttSyncClient, Task<OperateResult<T>>> exec)
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        throw new Exception(StringResources.Language.InsufficientPrivileges);
      IMqttSyncConnector client = this.mqttConnectPool.GetAvailableConnector();
      OperateResult<T> result = await exec(client.SyncClient);
      this.mqttConnectPool.ReturnConnector(client);
      return result;
    }

    private async Task<OperateResult<T1, T2>> ConnectPoolExecuteAsync<T1, T2>(
      Func<MqttSyncClient, Task<OperateResult<T1, T2>>> execAsync)
    {
      if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
        throw new Exception(StringResources.Language.InsufficientPrivileges);
      IMqttSyncConnector client = this.mqttConnectPool.GetAvailableConnector();
      OperateResult<T1, T2> result = await execAsync(client.SyncClient);
      this.mqttConnectPool.ReturnConnector(client);
      return result;
    }

    /// <inheritdoc cref="M:EstCommunication.MQTT.MqttSyncClient.Read(System.String,System.Byte[],System.Action{System.Int64,System.Int64},System.Action{System.String,System.String},System.Action{System.Int64,System.Int64})" />
    public OperateResult<string, byte[]> Read(
      string topic,
      byte[] payload,
      Action<long, long> sendProgress = null,
      Action<string, string> handleProgress = null,
      Action<long, long> receiveProgress = null)
    {
      return this.ConnectPoolExecute<string, byte[]>((Func<MqttSyncClient, OperateResult<string, byte[]>>) (m => m.Read(topic, payload, sendProgress, handleProgress, receiveProgress)));
    }

    /// <inheritdoc cref="M:EstCommunication.MQTT.MqttSyncClient.ReadString(System.String,System.String,System.Action{System.Int64,System.Int64},System.Action{System.String,System.String},System.Action{System.Int64,System.Int64})" />
    public OperateResult<string, string> ReadString(
      string topic,
      string payload,
      Action<long, long> sendProgress = null,
      Action<string, string> handleProgress = null,
      Action<long, long> receiveProgress = null)
    {
      return this.ConnectPoolExecute<string, string>((Func<MqttSyncClient, OperateResult<string, string>>) (m => m.ReadString(topic, payload, sendProgress, handleProgress, receiveProgress)));
    }

    /// <inheritdoc cref="M:EstCommunication.MQTT.MqttSyncClient.ReadRpc``1(System.String,System.String)" />
    public OperateResult<T> ReadRpc<T>(string topic, string payload) => this.ConnectPoolExecute<T>((Func<MqttSyncClient, OperateResult<T>>) (m => m.ReadRpc<T>(topic, payload)));

    /// <inheritdoc cref="M:EstCommunication.MQTT.MqttSyncClient.ReadRpc``1(System.String,System.Object)" />
    public OperateResult<T> ReadRpc<T>(string topic, object payload) => this.ConnectPoolExecute<T>((Func<MqttSyncClient, OperateResult<T>>) (m => m.ReadRpc<T>(topic, payload)));

    /// <inheritdoc cref="M:EstCommunication.MQTT.MqttSyncClient.ReadRpcApis" />
    public OperateResult<MqttRpcApiInfo[]> ReadRpcApis() => this.ConnectPoolExecute<MqttRpcApiInfo[]>((Func<MqttSyncClient, OperateResult<MqttRpcApiInfo[]>>) (m => m.ReadRpcApis()));

    /// <inheritdoc cref="M:EstCommunication.MQTT.MqttSyncClient.ReadRpcApiLog(System.String)" />
    public OperateResult<long[]> ReadRpcApiLog(string api) => this.ConnectPoolExecute<long[]>((Func<MqttSyncClient, OperateResult<long[]>>) (m => m.ReadRpcApiLog(api)));

    /// <inheritdoc cref="M:EstCommunication.MQTT.MqttSyncClient.ReadRetainTopics" />
    public OperateResult<string[]> ReadRetainTopics() => this.ConnectPoolExecute<string[]>((Func<MqttSyncClient, OperateResult<string[]>>) (m => m.ReadRetainTopics()));

    /// <inheritdoc cref="M:EstCommunication.MQTT.MqttSyncClient.ReadTopicPayload(System.String,System.Action{System.Int64,System.Int64})" />
    public OperateResult<MqttClientApplicationMessage> ReadTopicPayload(
      string topic,
      Action<long, long> receiveProgress = null)
    {
      return this.ConnectPoolExecute<MqttClientApplicationMessage>((Func<MqttSyncClient, OperateResult<MqttClientApplicationMessage>>) (m => m.ReadTopicPayload(topic, receiveProgress)));
    }

    /// <inheritdoc cref="M:EstCommunication.MQTT.MqttSyncClientPool.Read(System.String,System.Byte[],System.Action{System.Int64,System.Int64},System.Action{System.String,System.String},System.Action{System.Int64,System.Int64})" />
    public async Task<OperateResult<string, byte[]>> ReadAsync(
      string topic,
      byte[] payload,
      Action<long, long> sendProgress = null,
      Action<string, string> handleProgress = null,
      Action<long, long> receiveProgress = null)
    {
      OperateResult<string, byte[]> operateResult = await this.ConnectPoolExecuteAsync<string, byte[]>((Func<MqttSyncClient, Task<OperateResult<string, byte[]>>>) (m => m.ReadAsync(topic, payload, sendProgress, handleProgress, receiveProgress)));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.MQTT.MqttSyncClientPool.ReadString(System.String,System.String,System.Action{System.Int64,System.Int64},System.Action{System.String,System.String},System.Action{System.Int64,System.Int64})" />
    public async Task<OperateResult<string, string>> ReadStringAsync(
      string topic,
      string payload,
      Action<long, long> sendProgress = null,
      Action<string, string> handleProgress = null,
      Action<long, long> receiveProgress = null)
    {
      OperateResult<string, string> operateResult = await this.ConnectPoolExecuteAsync<string, string>((Func<MqttSyncClient, Task<OperateResult<string, string>>>) (m => m.ReadStringAsync(topic, payload, sendProgress, handleProgress, receiveProgress)));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.MQTT.MqttSyncClientPool.ReadRpc``1(System.String,System.String)" />
    public async Task<OperateResult<T>> ReadRpcAsync<T>(
      string topic,
      string payload)
    {
      OperateResult<T> operateResult = await this.ConnectPoolExecuteAsync<T>((Func<MqttSyncClient, Task<OperateResult<T>>>) (m => m.ReadRpcAsync<T>(topic, payload)));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.MQTT.MqttSyncClientPool.ReadRpc``1(System.String,System.Object)" />
    public async Task<OperateResult<T>> ReadRpcAsync<T>(
      string topic,
      object payload)
    {
      OperateResult<T> operateResult = await this.ConnectPoolExecuteAsync<T>((Func<MqttSyncClient, Task<OperateResult<T>>>) (m => m.ReadRpcAsync<T>(topic, payload)));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.MQTT.MqttSyncClientPool.ReadRpcApis" />
    public async Task<OperateResult<MqttRpcApiInfo[]>> ReadRpcApisAsync()
    {
      OperateResult<MqttRpcApiInfo[]> operateResult = await this.ConnectPoolExecuteAsync<MqttRpcApiInfo[]>((Func<MqttSyncClient, Task<OperateResult<MqttRpcApiInfo[]>>>) (m => m.ReadRpcApisAsync()));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.MQTT.MqttSyncClientPool.ReadRpcApiLog(System.String)" />
    public async Task<OperateResult<long[]>> ReadRpcApiLogAsync(string api)
    {
      OperateResult<long[]> operateResult = await this.ConnectPoolExecuteAsync<long[]>((Func<MqttSyncClient, Task<OperateResult<long[]>>>) (m => m.ReadRpcApiLogAsync(api)));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.MQTT.MqttSyncClient.ReadRetainTopics" />
    public async Task<OperateResult<string[]>> ReadRetainTopicsAsync()
    {
      OperateResult<string[]> operateResult = await this.ConnectPoolExecuteAsync<string[]>((Func<MqttSyncClient, Task<OperateResult<string[]>>>) (m => m.ReadRetainTopicsAsync()));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.MQTT.MqttSyncClient.ReadTopicPayload(System.String,System.Action{System.Int64,System.Int64})" />
    public async Task<OperateResult<MqttClientApplicationMessage>> ReadTopicPayloadAsync(
      string topic,
      Action<long, long> receiveProgress = null)
    {
      OperateResult<MqttClientApplicationMessage> operateResult = await this.ConnectPoolExecuteAsync<MqttClientApplicationMessage>((Func<MqttSyncClient, Task<OperateResult<MqttClientApplicationMessage>>>) (m => m.ReadTopicPayloadAsync(topic, receiveProgress)));
      return operateResult;
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("MqttSyncClientPool[{0}]", (object) this.mqttConnectPool.MaxConnector);
  }
}
