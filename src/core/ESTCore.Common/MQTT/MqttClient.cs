// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.MQTT.MqttClient
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;
using ESTCore.Common.Core.Net;
using ESTCore.Common.LogNet;

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ESTCore.Common.MQTT
{
    /// <summary>
    /// Mqtt协议的客户端实现，支持订阅消息，发布消息，详细的使用例子参考api文档<br />
    /// The client implementation of the Mqtt protocol supports subscription messages and publishing messages. For detailed usage examples, refer to the api documentation.
    /// </summary>
    /// <remarks>
    /// 这是一个MQTT的客户端实现，参照MQTT协议的3.1.1版本设计实现的。服务器可以是其他的组件提供的，其他的可以参考示例<br />
    /// This is an MQTT client implementation, designed and implemented with reference to version 3.1.1 of the MQTT protocol. The server can be provided by other components.
    /// </remarks>
    /// <example>
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\MQTT\MQTTClient.cs" region="Test" title="简单的实例化" />
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\MQTT\MQTTClient.cs" region="Test2" title="带用户名密码的实例化" />
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\MQTT\MQTTClient.cs" region="Test3" title="连接示例" />
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\MQTT\MQTTClient.cs" region="Test4" title="发布示例" />
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\MQTT\MQTTClient.cs" region="Test5" title="订阅示例" />
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\MQTT\MQTTClient.cs" region="Test8" title="网络重连示例" />
    /// </example>
    public class MqttClient : NetworkXBase
    {
        private DateTime activeTime;
        private int isReConnectServer = 0;
        private List<MqttPublishMessage> publishMessages;
        private object listLock;
        private List<string> subcribeTopics;
        private object connectLock;
        private object subcribeLock;
        private SoftIncrementCount incrementCount;
        private bool closed = false;
        private MqttConnectionOptions connectionOptions;
        private Timer timerCheck;

        /// <summary>实例化一个默认的对象</summary>
        /// <param name="options">配置信息</param>
        public MqttClient(MqttConnectionOptions options)
        {
            this.connectionOptions = options;
            this.incrementCount = new SoftIncrementCount((long)ushort.MaxValue, 1L);
            this.listLock = new object();
            this.publishMessages = new List<MqttPublishMessage>();
            this.subcribeTopics = new List<string>();
            this.activeTime = DateTime.Now;
            this.subcribeLock = new object();
            this.connectLock = new object();
        }

        /// <summary>
        /// 连接服务器，如果连接失败，请稍候重试。<br />
        /// Connect to the server. If the connection fails, try again later.
        /// </summary>
        /// <returns>连接是否成功</returns>
        public OperateResult ConnectServer()
        {
            if (this.connectionOptions == null)
                return new OperateResult("Optines is null");
            this.CoreSocket?.Close();
            OperateResult<Socket> socketAndConnect = this.CreateSocketAndConnect(this.connectionOptions.IpAddress, this.connectionOptions.Port, this.connectionOptions.ConnectTimeout);
            if (!socketAndConnect.IsSuccess)
                return (OperateResult)socketAndConnect;
            this.CoreSocket = socketAndConnect.Content;
            OperateResult<byte[]> operateResult1 = MqttHelper.BuildConnectMqttCommand(this.connectionOptions);
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            OperateResult operateResult2 = this.Send(this.CoreSocket, operateResult1.Content);
            if (!operateResult2.IsSuccess)
                return operateResult2;
            OperateResult<byte, byte[]> mqttMessage = this.ReceiveMqttMessage(this.CoreSocket, 30000);
            if (!mqttMessage.IsSuccess)
                return (OperateResult)mqttMessage;
            OperateResult operateResult3 = MqttHelper.CheckConnectBack(mqttMessage.Content1, mqttMessage.Content2);
            if (!operateResult3.IsSuccess)
            {
                this.CoreSocket?.Close();
                return operateResult3;
            }
            this.incrementCount.ResetCurrentValue();
            this.closed = false;
            try
            {
                this.CoreSocket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, new AsyncCallback(this.ReceiveAsyncCallback), (object)this.CoreSocket);
            }
            catch (Exception ex)
            {
                return new OperateResult(ex.Message);
            }
            MqttClient.OnClientConnectedDelegate onClientConnected = this.OnClientConnected;
            if (onClientConnected != null)
                onClientConnected(this);
            this.timerCheck?.Dispose();
            this.activeTime = DateTime.Now;
            TimeSpan aliveSendInterval = this.connectionOptions.KeepAliveSendInterval;
            if ((int)aliveSendInterval.TotalMilliseconds > 0)
            {
                TimerCallback callback = new TimerCallback(this.TimerCheckServer);
                aliveSendInterval = this.connectionOptions.KeepAliveSendInterval;
                int totalMilliseconds = (int)aliveSendInterval.TotalMilliseconds;
                this.timerCheck = new Timer(callback, (object)null, 2000, totalMilliseconds);
            }
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 关闭Mqtt服务器的连接。<br />
        /// Close the connection to the Mqtt server.
        /// </summary>
        public void ConnectClose()
        {
            lock (this.connectLock)
                this.closed = true;
            OperateResult<byte[]> operateResult = MqttHelper.BuildMqttCommand((byte)14, (byte)0, (byte[])null, (byte[])null);
            if (operateResult.IsSuccess)
                this.Send(this.CoreSocket, operateResult.Content);
            this.timerCheck?.Dispose();
            Thread.Sleep(20);
            this.CoreSocket?.Close();
        }

        /// <inheritdoc cref="M:ESTCore.Common.MQTT.MqttClient.ConnectServer" />
        public async Task<OperateResult> ConnectServerAsync()
        {
            if (this.connectionOptions == null)
                return new OperateResult("Optines is null");
            this.CoreSocket?.Close();
            OperateResult<Socket> connect = await this.CreateSocketAndConnectAsync(this.connectionOptions.IpAddress, this.connectionOptions.Port, this.connectionOptions.ConnectTimeout);
            if (!connect.IsSuccess)
                return (OperateResult)connect;
            this.CoreSocket = connect.Content;
            OperateResult<byte[]> command = MqttHelper.BuildConnectMqttCommand(this.connectionOptions);
            if (!command.IsSuccess)
                return (OperateResult)command;
            OperateResult send = await this.SendAsync(this.CoreSocket, command.Content);
            if (!send.IsSuccess)
                return send;
            OperateResult<byte, byte[]> receive = await this.ReceiveMqttMessageAsync(this.CoreSocket, 30000);
            if (!receive.IsSuccess)
                return (OperateResult)receive;
            OperateResult check = MqttHelper.CheckConnectBack(receive.Content1, receive.Content2);
            if (!check.IsSuccess)
            {
                this.CoreSocket?.Close();
                return check;
            }
            this.incrementCount.ResetCurrentValue();
            this.closed = false;
            try
            {
                this.CoreSocket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, new AsyncCallback(this.ReceiveAsyncCallback), (object)this.CoreSocket);
            }
            catch (Exception ex)
            {
                return new OperateResult(ex.Message);
            }
            MqttClient.OnClientConnectedDelegate onClientConnected = this.OnClientConnected;
            if (onClientConnected != null)
                onClientConnected(this);
            this.timerCheck?.Dispose();
            this.activeTime = DateTime.Now;
            TimeSpan aliveSendInterval = this.connectionOptions.KeepAliveSendInterval;
            if ((int)aliveSendInterval.TotalMilliseconds > 0)
            {
                TimerCallback callback = new TimerCallback(this.TimerCheckServer);
                aliveSendInterval = this.connectionOptions.KeepAliveSendInterval;
                int totalMilliseconds = (int)aliveSendInterval.TotalMilliseconds;
                this.timerCheck = new Timer(callback, (object)null, 2000, totalMilliseconds);
            }
            return OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc cref="M:ESTCore.Common.MQTT.MqttClient.ConnectClose" />
        public async Task ConnectCloseAsync()
        {
            lock (this.connectLock)
                this.closed = true;
            OperateResult<byte[]> command = MqttHelper.BuildMqttCommand((byte)14, (byte)0, (byte[])null, (byte[])null);
            if (command.IsSuccess)
            {
                OperateResult operateResult = await this.SendAsync(this.CoreSocket, command.Content);
            }
            this.timerCheck?.Dispose();
            Thread.Sleep(20);
            Socket coreSocket = this.CoreSocket;
            if (coreSocket == null)
            {
                command = (OperateResult<byte[]>)null;
            }
            else
            {
                coreSocket.Close();
                command = (OperateResult<byte[]>)null;
            }
        }

        /// <summary>
        /// 发布一个MQTT协议的消息到服务器。该消息包含主题，负载数据，消息等级，是否保留信息。<br />
        /// Publish an MQTT protocol message to the server. The message contains the subject, payload data, message level, and whether to retain information.
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns>发布结果</returns>
        /// <example>
        /// 参照 <see cref="T:ESTCore.Common.MQTT.MqttClient" /> 的示例说明。
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\MQTT\MQTTClient.cs" region="Test" title="简单的实例化" />
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\MQTT\MQTTClient.cs" region="Test4" title="发布示例" />
        /// </example>
        public OperateResult PublishMessage(MqttApplicationMessage message)
        {
            MqttPublishMessage mqttPublishMessage = new MqttPublishMessage()
            {
                Identifier = message.QualityOfServiceLevel == MqttQualityOfServiceLevel.AtMostOnce ? 0 : (int)this.incrementCount.GetCurrentValue(),
                Message = message
            };
            OperateResult<byte[]> operateResult = MqttHelper.BuildPublishMqttCommand(mqttPublishMessage);
            if (!operateResult.IsSuccess)
                return (OperateResult)operateResult;
            if (message.QualityOfServiceLevel == MqttQualityOfServiceLevel.AtMostOnce)
                return this.Send(this.CoreSocket, operateResult.Content);
            this.AddPublishMessage(mqttPublishMessage);
            return this.Send(this.CoreSocket, operateResult.Content);
        }

        /// <inheritdoc cref="M:ESTCore.Common.MQTT.MqttClient.PublishMessage(ESTCore.Common.MQTT.MqttApplicationMessage)" />
        public async Task<OperateResult> PublishMessageAsync(
          MqttApplicationMessage message)
        {
            MqttPublishMessage publishMessage = new MqttPublishMessage()
            {
                Identifier = message.QualityOfServiceLevel == MqttQualityOfServiceLevel.AtMostOnce ? 0 : (int)this.incrementCount.GetCurrentValue(),
                Message = message
            };
            OperateResult<byte[]> command = MqttHelper.BuildPublishMqttCommand(publishMessage);
            if (!command.IsSuccess)
                return (OperateResult)command;
            if (message.QualityOfServiceLevel == MqttQualityOfServiceLevel.AtMostOnce)
            {
                OperateResult operateResult = await this.SendAsync(this.CoreSocket, command.Content);
                return operateResult;
            }
            this.AddPublishMessage(publishMessage);
            OperateResult operateResult1 = await this.SendAsync(this.CoreSocket, command.Content);
            return operateResult1;
        }

        /// <summary>
        /// 从服务器订阅一个或多个主题信息<br />
        /// Subscribe to one or more topics from the server
        /// </summary>
        /// <param name="topic">主题信息</param>
        /// <returns>订阅结果</returns>
        /// <example>
        /// 参照 <see cref="T:ESTCore.Common.MQTT.MqttClient" /> 的示例说明。
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\MQTT\MQTTClient.cs" region="Test" title="简单的实例化" />
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\MQTT\MQTTClient.cs" region="Test5" title="订阅示例" />
        /// </example>
        public OperateResult SubscribeMessage(string topic) => this.SubscribeMessage(new string[1]
        {
      topic
        });

        /// <inheritdoc cref="M:ESTCore.Common.MQTT.MqttClient.SubscribeMessage(System.String)" />
        public OperateResult SubscribeMessage(string[] topics) => this.SubscribeMessage(new MqttSubscribeMessage()
        {
            Identifier = (int)this.incrementCount.GetCurrentValue(),
            Topics = topics
        });

        /// <summary>
        /// 向服务器订阅一个主题消息，可以指定订阅的主题数组，订阅的质量等级，还有消息标识符<br />
        /// To subscribe to a topic message from the server, you can specify the subscribed topic array,
        /// the subscription quality level, and the message identifier
        /// </summary>
        /// <param name="subcribeMessage">订阅的消息本体</param>
        /// <returns>是否订阅成功</returns>
        public OperateResult SubscribeMessage(MqttSubscribeMessage subcribeMessage)
        {
            if (subcribeMessage.Topics == null || subcribeMessage.Topics.Length == 0)
                return OperateResult.CreateSuccessResult();
            OperateResult<byte[]> operateResult1 = MqttHelper.BuildSubscribeMqttCommand(subcribeMessage);
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            OperateResult operateResult2 = this.Send(this.CoreSocket, operateResult1.Content);
            if (!operateResult2.IsSuccess)
                return operateResult2;
            this.AddSubTopics(subcribeMessage.Topics);
            return OperateResult.CreateSuccessResult();
        }

        private void AddSubTopics(string[] topics)
        {
            lock (this.subcribeLock)
            {
                for (int index = 0; index < topics.Length; ++index)
                {
                    if (!this.subcribeTopics.Contains(topics[index]))
                        this.subcribeTopics.Add(topics[index]);
                }
            }
        }

        /// <summary>
        /// 取消订阅多个主题信息，取消之后，当前的订阅数据就不在接收到，除非服务器强制推送。<br />
        /// Unsubscribe from multiple topic information. After cancellation, the current subscription data will not be received unless the server forces it to push it.
        /// </summary>
        /// <param name="topics">主题信息</param>
        /// <returns>取消订阅结果</returns>
        /// <example>
        /// 参照 <see cref="T:ESTCore.Common.MQTT.MqttClient" /> 的示例说明。
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\MQTT\MQTTClient.cs" region="Test" title="简单的实例化" />
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\MQTT\MQTTClient.cs" region="Test7" title="订阅示例" />
        /// </example>
        public OperateResult UnSubscribeMessage(string[] topics)
        {
            OperateResult<byte[]> operateResult1 = MqttHelper.BuildUnSubscribeMqttCommand(new MqttSubscribeMessage()
            {
                Identifier = (int)this.incrementCount.GetCurrentValue(),
                Topics = topics
            });
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            OperateResult operateResult2 = this.Send(this.CoreSocket, operateResult1.Content);
            if (!operateResult2.IsSuccess)
                return operateResult2;
            this.RemoveSubTopics(topics);
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>取消订阅置顶的主题信息</summary>
        /// <param name="topic">主题信息</param>
        /// <returns>取消订阅结果</returns>
        /// <example>
        /// 参照 <see cref="T:ESTCore.Common.MQTT.MqttClient" /> 的示例说明。
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\MQTT\MQTTClient.cs" region="Test" title="简单的实例化" />
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\MQTT\MQTTClient.cs" region="Test7" title="订阅示例" />
        /// </example>
        public OperateResult UnSubscribeMessage(string topic) => this.UnSubscribeMessage(new string[1]
        {
      topic
        });

        private void RemoveSubTopics(string[] topics)
        {
            lock (this.subcribeLock)
            {
                for (int index = 0; index < topics.Length; ++index)
                {
                    if (this.subcribeTopics.Contains(topics[index]))
                        this.subcribeTopics.Remove(topics[index]);
                }
            }
        }

        /// <inheritdoc cref="M:ESTCore.Common.MQTT.MqttClient.SubscribeMessage(System.String)" />
        public async Task<OperateResult> SubscribeMessageAsync(string topic)
        {
            OperateResult operateResult = await this.SubscribeMessageAsync(new string[1]
            {
        topic
            });
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.MQTT.MqttClient.SubscribeMessage(System.String[])" />
        public async Task<OperateResult> SubscribeMessageAsync(string[] topics)
        {
            if (topics == null || topics.Length == 0)
                return OperateResult.CreateSuccessResult();
            MqttSubscribeMessage subcribeMessage = new MqttSubscribeMessage()
            {
                Identifier = (int)this.incrementCount.GetCurrentValue(),
                Topics = topics
            };
            OperateResult<byte[]> command = MqttHelper.BuildSubscribeMqttCommand(subcribeMessage);
            if (!command.IsSuccess)
                return (OperateResult)command;
            this.AddSubTopics(topics);
            OperateResult operateResult = await this.SendAsync(this.CoreSocket, command.Content);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.MQTT.MqttClient.UnSubscribeMessage(System.String[])" />
        public async Task<OperateResult> UnSubscribeMessageAsync(string[] topics)
        {
            MqttSubscribeMessage subcribeMessage = new MqttSubscribeMessage()
            {
                Identifier = (int)this.incrementCount.GetCurrentValue(),
                Topics = topics
            };
            OperateResult<byte[]> command = MqttHelper.BuildUnSubscribeMqttCommand(subcribeMessage);
            this.RemoveSubTopics(topics);
            OperateResult operateResult = await this.SendAsync(this.CoreSocket, command.Content);
            subcribeMessage = (MqttSubscribeMessage)null;
            command = (OperateResult<byte[]>)null;
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.MQTT.MqttClient.UnSubscribeMessage(System.String)" />
        public async Task<OperateResult> UnSubscribeMessageAsync(string topic)
        {
            OperateResult operateResult = await this.UnSubscribeMessageAsync(new string[1]
            {
        topic
            });
            return operateResult;
        }

        private void OnMqttNetworkError()
        {
            if (this.closed)
            {
                this.LogNet?.WriteDebug(this.ToString(), "Closed");
            }
            else
            {
                if (Interlocked.CompareExchange(ref this.isReConnectServer, 1, 0) != 0)
                    return;
                try
                {
                    if (this.OnNetworkError == null)
                    {
                        this.LogNet?.WriteInfo(this.ToString(), "The network is abnormal, and the system is ready to automatically reconnect after 10 seconds.");
                    label_33:
                        for (int index = 0; index < 10; ++index)
                        {
                            Thread.Sleep(1000);
                            this.LogNet?.WriteInfo(this.ToString(), string.Format("Wait for {0} second to connect to the server ...", (object)(10 - index)));
                            if (this.closed)
                            {
                                this.LogNet?.WriteDebug(this.ToString(), "Closed");
                                return;
                            }
                        }
                        lock (this.connectLock)
                        {
                            if (this.closed)
                            {
                                this.LogNet?.WriteDebug(this.ToString(), "Closed");
                                return;
                            }
                            if (this.ConnectServer().IsSuccess)
                            {
                                this.LogNet?.WriteInfo(this.ToString(), "Successfully connected to the server!");
                            }
                            else
                            {
                                this.LogNet?.WriteInfo(this.ToString(), "The connection failed. Prepare to reconnect after 10 seconds.");
                                if (this.closed)
                                {
                                    this.LogNet?.WriteDebug(this.ToString(), "Closed");
                                    return;
                                }
                                goto label_33;
                            }
                        }
                    }
                    else
                    {
                        EventHandler onNetworkError = this.OnNetworkError;
                        if (onNetworkError != null)
                            onNetworkError((object)this, new EventArgs());
                    }
                    Interlocked.Exchange(ref this.isReConnectServer, 0);
                }
                catch
                {
                    Interlocked.Exchange(ref this.isReConnectServer, 0);
                    throw;
                }
            }
        }

        private async void ReceiveAsyncCallback(IAsyncResult ar)
        {
            if (!(ar.AsyncState is Socket socket))
            {
                socket = (Socket)null;
            }
            else
            {
                try
                {
                    socket.EndReceive(ar);
                }
                catch (ObjectDisposedException ex)
                {
                    socket?.Close();
                    ILogNet logNet = this.LogNet;
                    if (logNet == null)
                    {
                        socket = (Socket)null;
                        return;
                    }
                    logNet.WriteDebug(this.ToString(), "Closed");
                    socket = (Socket)null;
                    return;
                }
                catch (Exception ex)
                {
                    socket?.Close();
                    this.LogNet?.WriteDebug(this.ToString(), "ReceiveCallback Failed:" + ex.Message);
                    this.OnMqttNetworkError();
                    socket = (Socket)null;
                    return;
                }
                if (this.closed)
                {
                    ILogNet logNet = this.LogNet;
                    if (logNet == null)
                    {
                        socket = (Socket)null;
                    }
                    else
                    {
                        logNet.WriteDebug(this.ToString(), "Closed");
                        socket = (Socket)null;
                    }
                }
                else
                {
                    OperateResult<byte, byte[]> read = await this.ReceiveMqttMessageAsync(socket, 30000);
                    if (!read.IsSuccess)
                    {
                        this.OnMqttNetworkError();
                        socket = (Socket)null;
                    }
                    else
                    {
                        byte mqttCode = read.Content1;
                        byte[] data = read.Content2;
                        if ((int)mqttCode >> 4 == 4)
                            this.LogNet?.WriteDebug(this.ToString(), string.Format("Code[{0:X2}] Publish Ack: {1}", (object)mqttCode, (object)SoftBasic.ByteToHexString(data, ' ')));
                        else if ((int)mqttCode >> 4 == 5)
                        {
                            this.Send(socket, MqttHelper.BuildMqttCommand((byte)6, (byte)2, data, new byte[0]).Content);
                            this.LogNet?.WriteDebug(this.ToString(), string.Format("Code[{0:X2}] Publish Rec: {1}", (object)mqttCode, (object)SoftBasic.ByteToHexString(data, ' ')));
                        }
                        else if ((int)mqttCode >> 4 == 7)
                            this.LogNet?.WriteDebug(this.ToString(), string.Format("Code[{0:X2}] Publish Complete: {1}", (object)mqttCode, (object)SoftBasic.ByteToHexString(data, ' ')));
                        else if ((int)mqttCode >> 4 == 13)
                        {
                            this.activeTime = DateTime.Now;
                            this.LogNet?.WriteDebug(this.ToString(), "Heart Code Check!");
                        }
                        else if ((int)mqttCode >> 4 == 3)
                            this.ExtraPublishData(mqttCode, data);
                        else if ((int)mqttCode >> 4 == 9)
                            this.LogNet?.WriteDebug(this.ToString(), string.Format("Code[{0:X2}] Subscribe Ack: {1}", (object)mqttCode, (object)SoftBasic.ByteToHexString(data, ' ')));
                        else if ((int)mqttCode >> 4 == 11)
                            this.LogNet?.WriteDebug(this.ToString(), string.Format("Code[{0:X2}] UnSubscribe Ack: {1}", (object)mqttCode, (object)SoftBasic.ByteToHexString(data, ' ')));
                        else
                            this.LogNet?.WriteDebug(this.ToString(), string.Format("Code[{0:X2}] {1}", (object)mqttCode, (object)SoftBasic.ByteToHexString(data, ' ')));
                        try
                        {
                            socket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, new AsyncCallback(this.ReceiveAsyncCallback), (object)socket);
                        }
                        catch (Exception ex)
                        {
                            socket?.Close();
                            this.LogNet?.WriteDebug(this.ToString(), "BeginReceive Failed:" + ex.Message);
                            this.OnMqttNetworkError();
                        }
                        read = (OperateResult<byte, byte[]>)null;
                        data = (byte[])null;
                        socket = (Socket)null;
                    }
                }
            }
        }

        private void ExtraPublishData(byte mqttCode, byte[] data)
        {
            this.activeTime = DateTime.Now;
            OperateResult<string, byte[]> data1 = MqttHelper.ExtraMqttReceiveData(mqttCode, data);
            if (!data1.IsSuccess)
            {
                this.LogNet?.WriteDebug(this.ToString(), data1.Message);
            }
            else
            {
                MqttClient.MqttMessageReceiveDelegate mqttMessageReceived = this.OnMqttMessageReceived;
                if (mqttMessageReceived == null)
                    return;
                mqttMessageReceived(data1.Content1, data1.Content2);
            }
        }

        private void TimerCheckServer(object obj)
        {
            if (this.CoreSocket == null)
                return;
            if ((DateTime.Now - this.activeTime).TotalSeconds > this.connectionOptions.KeepAliveSendInterval.TotalSeconds * 3.0)
                this.OnMqttNetworkError();
            else if (!this.Send(this.CoreSocket, MqttHelper.BuildMqttCommand((byte)12, (byte)0, new byte[0], new byte[0]).Content).IsSuccess)
                this.OnMqttNetworkError();
        }

        private void AddPublishMessage(MqttPublishMessage publishMessage)
        {
        }

        /// <summary>当接收到Mqtt订阅的信息的时候触发</summary>
        public event MqttClient.MqttMessageReceiveDelegate OnMqttMessageReceived;

        /// <summary>当网络发生异常的时候触发的事件，用户应该在事件里进行重连服务器</summary>
        public event EventHandler OnNetworkError;

        /// <summary>
        /// 当客户端连接成功触发事件，就算是重新连接服务器后，也是会触发的<br />
        /// The event is triggered when the client is connected successfully, even after reconnecting to the server.
        /// </summary>
        public event MqttClient.OnClientConnectedDelegate OnClientConnected;

        /// <inheritdoc />
        public override string ToString() => string.Format("MqttClient[{0}:{1}]", (object)this.connectionOptions.IpAddress, (object)this.connectionOptions.Port);

        /// <summary>
        /// 当接收到Mqtt订阅的信息的时候触发<br />
        /// Triggered when receiving Mqtt subscription information
        /// </summary>
        /// <param name="topic">主题信息</param>
        /// <param name="payload">负载数据</param>
        public delegate void MqttMessageReceiveDelegate(string topic, byte[] payload);

        /// <summary>
        /// 连接服务器成功的委托<br />
        /// Connection server successfully delegated
        /// </summary>
        public delegate void OnClientConnectedDelegate(MqttClient client);
    }
}
