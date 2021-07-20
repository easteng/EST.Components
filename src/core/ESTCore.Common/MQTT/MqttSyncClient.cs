// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.MQTT.MqttSyncClient
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;
using ESTCore.Common.Core;
using ESTCore.Common.Core.Net;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.Common.MQTT
{
    /// <summary>
    /// 基于MQTT协议的同步访问的客户端程序，支持以同步的方式访问服务器的数据信息，并及时的反馈结果，当服务器启动文件功能时，也支持文件的上传，下载，删除操作等。<br />
    /// The client program based on MQTT protocol for synchronous access supports synchronous access to the server's data information and timely feedback of results,
    /// When the server starts the file function, it also supports file upload, download, and delete operations.
    /// </summary>
    /// <example>
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\MQTT\MqttSyncClientSample.cs" region="Test" title="简单的实例化" />
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\MQTT\MqttSyncClientSample.cs" region="Test2" title="带用户名密码的实例化" />
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\MQTT\MqttSyncClientSample.cs" region="Test3" title="连接示例" />
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\MQTT\MqttSyncClientSample.cs" region="Test4" title="读取数据示例" />
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\MQTT\MqttSyncClientSample.cs" region="Test5" title="带进度报告示例" />
    /// 当MqttServer注册了远程RPC接口的时候，例如将一个plc对象注册是接口对象
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\MQTT\MqttSyncClientSample.cs" region="Test11" title="RPC接口读取" />
    /// 下面演示文件部分的功能的接口方法，主要包含，上传，下载，删除，遍历操作
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\MQTT\MqttSyncClientSample.cs" region="Test6" title="下载文件功能" />
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\MQTT\MqttSyncClientSample.cs" region="Test7" title="上传文件功能" />
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\MQTT\MqttSyncClientSample.cs" region="Test8" title="删除文件功能" />
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\MQTT\MqttSyncClientSample.cs" region="Test9" title="遍历指定目录的文件名功能" />
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\MQTT\MqttSyncClientSample.cs" region="Test10" title="遍历指定目录的所有子目录" />
    /// 上述的两个遍历的方法，就可以遍历出服务器的所有目录和文件了，具体可以参考 Demo 的MQTT文件客户端的演示界面。
    /// </example>
    public class MqttSyncClient : NetworkDoubleBase
    {
        private SoftIncrementCount incrementCount;
        private MqttConnectionOptions connectionOptions;
        private Encoding stringEncoding = Encoding.UTF8;

        /// <summary>
        /// 实例化一个MQTT的同步客户端<br />
        /// Instantiate an MQTT synchronization client
        /// </summary>
        public MqttSyncClient(MqttConnectionOptions options)
        {
            this.ByteTransform = (IByteTransform)new RegularByteTransform();
            this.connectionOptions = options;
            this.IpAddress = options.IpAddress;
            this.Port = options.Port;
            this.incrementCount = new SoftIncrementCount((long)ushort.MaxValue, 1L);
            this.ConnectTimeOut = options.ConnectTimeout;
            this.receiveTimeOut = 60000;
        }

        /// <summary>
        /// 通过指定的ip地址及端口来实例化一个同步的MQTT客户端<br />
        /// Instantiate a synchronized MQTT client with the specified IP address and port
        /// </summary>
        /// <param name="ipAddress">IP地址信息</param>
        /// <param name="port">端口号信息</param>
        public MqttSyncClient(string ipAddress, int port)
        {
            this.connectionOptions = new MqttConnectionOptions()
            {
                IpAddress = ipAddress,
                Port = port
            };
            this.ByteTransform = (IByteTransform)new RegularByteTransform();
            this.IpAddress = ipAddress;
            this.Port = port;
            this.incrementCount = new SoftIncrementCount((long)ushort.MaxValue, 1L);
            this.receiveTimeOut = 60000;
        }

        /// <summary>
        /// 通过指定的ip地址及端口来实例化一个同步的MQTT客户端<br />
        /// Instantiate a synchronized MQTT client with the specified IP address and port
        /// </summary>
        /// <param name="ipAddress">IP地址信息</param>
        /// <param name="port">端口号信息</param>
        public MqttSyncClient(IPAddress ipAddress, int port)
        {
            this.connectionOptions = new MqttConnectionOptions()
            {
                IpAddress = ipAddress.ToString(),
                Port = port
            };
            this.ByteTransform = (IByteTransform)new RegularByteTransform();
            this.IpAddress = ipAddress.ToString();
            this.Port = port;
            this.incrementCount = new SoftIncrementCount((long)ushort.MaxValue, 1L);
        }

        /// <inheritdoc />
        protected override OperateResult InitializationOnConnect(Socket socket)
        {
            OperateResult<byte[]> operateResult1 = MqttHelper.BuildConnectMqttCommand(this.connectionOptions, "HUSL");
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            OperateResult operateResult2 = this.Send(socket, operateResult1.Content);
            if (!operateResult2.IsSuccess)
                return operateResult2;
            OperateResult<byte, byte[]> mqttMessage = this.ReceiveMqttMessage(socket, this.ReceiveTimeOut);
            if (!mqttMessage.IsSuccess)
                return (OperateResult)mqttMessage;
            OperateResult operateResult3 = MqttHelper.CheckConnectBack(mqttMessage.Content1, mqttMessage.Content2);
            if (!operateResult3.IsSuccess)
            {
                socket?.Close();
                return operateResult3;
            }
            this.incrementCount.ResetCurrentValue();
            return OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc />
        protected override async Task<OperateResult> InitializationOnConnectAsync(
          Socket socket)
        {
            OperateResult<byte[]> command = MqttHelper.BuildConnectMqttCommand(this.connectionOptions, "HUSL");
            if (!command.IsSuccess)
                return (OperateResult)command;
            OperateResult send = await this.SendAsync(socket, command.Content);
            if (!send.IsSuccess)
                return send;
            OperateResult<byte, byte[]> receive = await this.ReceiveMqttMessageAsync(socket, this.ReceiveTimeOut);
            if (!receive.IsSuccess)
                return (OperateResult)receive;
            OperateResult check = MqttHelper.CheckConnectBack(receive.Content1, receive.Content2);
            if (!check.IsSuccess)
            {
                socket?.Close();
                return check;
            }
            this.incrementCount.ResetCurrentValue();
            return OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc />
        public override OperateResult<byte[]> ReadFromCoreServer(
          Socket socket,
          byte[] send,
          bool hasResponseData = true,
          bool usePackHeader = true)
        {
            OperateResult<byte, byte[]> operateResult = this.ReadMqttFromCoreServer(socket, send, (Action<long, long>)null, (Action<string, string>)null, (Action<long, long>)null);
            return operateResult.IsSuccess ? OperateResult.CreateSuccessResult<byte[]>(operateResult.Content2) : OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult);
        }

        private OperateResult<byte, byte[]> ReadMqttFromCoreServer(
          Socket socket,
          byte[] send,
          Action<long, long> sendProgress,
          Action<string, string> handleProgress,
          Action<long, long> receiveProgress)
        {
            OperateResult result = this.Send(socket, send);
            if (!result.IsSuccess)
                return OperateResult.CreateFailedResult<byte, byte[]>(result);
            OperateResult<byte, byte[]> mqttMessage1;
            OperateResult<string, byte[]> data1;
            long int64_1;
            long int64_2;
            do
            {
                mqttMessage1 = this.ReceiveMqttMessage(socket, this.ReceiveTimeOut);
                if (mqttMessage1.IsSuccess)
                {
                    data1 = MqttHelper.ExtraMqttReceiveData(mqttMessage1.Content1, mqttMessage1.Content2);
                    if (data1.IsSuccess)
                    {
                        if (data1.Content2.Length == 16)
                        {
                            int64_1 = BitConverter.ToInt64(data1.Content2, 0);
                            int64_2 = BitConverter.ToInt64(data1.Content2, 8);
                            if (sendProgress != null)
                                sendProgress(int64_1, int64_2);
                        }
                        else
                            goto label_6;
                    }
                    else
                        goto label_4;
                }
                else
                    goto label_2;
            }
            while (int64_1 != int64_2);
            goto label_17;
        label_2:
            return mqttMessage1;
        label_4:
            return OperateResult.CreateFailedResult<byte, byte[]>((OperateResult)data1);
        label_6:
            return new OperateResult<byte, byte[]>(StringResources.Language.ReceiveDataLengthTooShort);
        label_17:
            OperateResult<byte, byte[]> mqttMessage2;
            while (true)
            {
                mqttMessage2 = this.ReceiveMqttMessage(socket, this.ReceiveTimeOut, receiveProgress);
                if (mqttMessage2.IsSuccess)
                {
                    if ((int)mqttMessage2.Content1 >> 4 == 15)
                    {
                        OperateResult<string, byte[]> data2 = MqttHelper.ExtraMqttReceiveData(mqttMessage2.Content1, mqttMessage2.Content2);
                        if (handleProgress != null)
                            handleProgress(data2.Content1, Encoding.UTF8.GetString(data2.Content2));
                    }
                    else
                        goto label_15;
                }
                else
                    break;
            }
            return mqttMessage2;
        label_15:
            return OperateResult.CreateSuccessResult<byte, byte[]>(mqttMessage2.Content1, mqttMessage2.Content2);
        }

        private OperateResult<byte[]> ReadMqttFromCoreServer(
          byte[] send,
          Action<long, long> sendProgress,
          Action<string, string> handleProgress,
          Action<long, long> receiveProgress)
        {
            OperateResult<byte[]> operateResult = new OperateResult<byte[]>();
            this.InteractiveLock.Enter();
            OperateResult<Socket> availableSocket;
            try
            {
                availableSocket = this.GetAvailableSocket();
                if (!availableSocket.IsSuccess)
                {
                    this.IsSocketError = true;
                    this.AlienSession?.Offline();
                    this.InteractiveLock.Leave();
                    operateResult.CopyErrorFromOther<OperateResult<Socket>>(availableSocket);
                    return operateResult;
                }
                OperateResult<byte, byte[]> result = this.ReadMqttFromCoreServer(availableSocket.Content, send, sendProgress, handleProgress, receiveProgress);
                if (result.IsSuccess)
                {
                    this.IsSocketError = false;
                    if ((int)result.Content1 >> 4 == 0)
                    {
                        OperateResult<string, byte[]> data = MqttHelper.ExtraMqttReceiveData(result.Content1, result.Content2);
                        operateResult.IsSuccess = false;
                        operateResult.ErrorCode = int.Parse(data.Content1);
                        operateResult.Message = Encoding.UTF8.GetString(data.Content2);
                    }
                    else
                    {
                        operateResult.IsSuccess = result.IsSuccess;
                        operateResult.Content = result.Content2;
                        operateResult.Message = StringResources.Language.SuccessText;
                    }
                }
                else
                {
                    this.IsSocketError = true;
                    this.AlienSession?.Offline();
                    operateResult.CopyErrorFromOther<OperateResult<byte, byte[]>>(result);
                }
                this.ExtraAfterReadFromCoreServer((OperateResult)result);
                this.InteractiveLock.Leave();
            }
            catch
            {
                this.InteractiveLock.Leave();
                throw;
            }
            if (!this.isPersistentConn && availableSocket != null)
                availableSocket.Content?.Close();
            return operateResult;
        }

        /// <inheritdoc />
        public override async Task<OperateResult<byte[]>> ReadFromCoreServerAsync(
          Socket socket,
          byte[] send,
          bool hasResponseData = true,
          bool usePackHeader = true)
        {
            OperateResult<byte, byte[]> read = await this.ReadMqttFromCoreServerAsync(socket, send, (Action<long, long>)null, (Action<string, string>)null, (Action<long, long>)null);
            OperateResult<byte[]> operateResult = !read.IsSuccess ? OperateResult.CreateFailedResult<byte[]>((OperateResult)read) : OperateResult.CreateSuccessResult<byte[]>(read.Content2);
            read = (OperateResult<byte, byte[]>)null;
            return operateResult;
        }

        private async Task<OperateResult<byte, byte[]>> ReadMqttFromCoreServerAsync(
          Socket socket,
          byte[] send,
          Action<long, long> sendProgress,
          Action<string, string> handleProgress,
          Action<long, long> receiveProgress)
        {
            OperateResult sendResult = await this.SendAsync(socket, send);
            if (!sendResult.IsSuccess)
                return OperateResult.CreateFailedResult<byte, byte[]>(sendResult);
            OperateResult<byte, byte[]> server_receive;
            OperateResult<string, byte[]> server_back;
            while (true)
            {
                server_receive = await this.ReceiveMqttMessageAsync(socket, this.ReceiveTimeOut);
                if (server_receive.IsSuccess)
                {
                    server_back = MqttHelper.ExtraMqttReceiveData(server_receive.Content1, server_receive.Content2);
                    if (server_back.IsSuccess)
                    {
                        if (server_back.Content2.Length == 16)
                        {
                            long already = BitConverter.ToInt64(server_back.Content2, 0);
                            long total = BitConverter.ToInt64(server_back.Content2, 8);
                            Action<long, long> action = sendProgress;
                            if (action != null)
                                action(already, total);
                            if (already != total)
                            {
                                server_receive = (OperateResult<byte, byte[]>)null;
                                server_back = (OperateResult<string, byte[]>)null;
                            }
                            else
                                goto label_21;
                        }
                        else
                            goto label_8;
                    }
                    else
                        goto label_6;
                }
                else
                    break;
            }
            return server_receive;
        label_6:
            return OperateResult.CreateFailedResult<byte, byte[]>((OperateResult)server_back);
        label_8:
            return new OperateResult<byte, byte[]>(StringResources.Language.ReceiveDataLengthTooShort);
        label_21:
            OperateResult<byte, byte[]> receive;
            while (true)
            {
                receive = await this.ReceiveMqttMessageAsync(socket, this.ReceiveTimeOut, receiveProgress);
                if (receive.IsSuccess)
                {
                    if ((int)receive.Content1 >> 4 == 15)
                    {
                        OperateResult<string, byte[]> extra = MqttHelper.ExtraMqttReceiveData(receive.Content1, receive.Content2);
                        Action<string, string> action = handleProgress;
                        if (action != null)
                            action(extra.Content1, Encoding.UTF8.GetString(extra.Content2));
                        extra = (OperateResult<string, byte[]>)null;
                        receive = (OperateResult<byte, byte[]>)null;
                    }
                    else
                        goto label_20;
                }
                else
                    break;
            }
            return receive;
        label_20:
            return OperateResult.CreateSuccessResult<byte, byte[]>(receive.Content1, receive.Content2);
        }

        private async Task<OperateResult<byte[]>> ReadMqttFromCoreServerAsync(
          byte[] send,
          Action<long, long> sendProgress,
          Action<string, string> handleProgress,
          Action<long, long> receiveProgress)
        {
            OperateResult<byte[]> result = new OperateResult<byte[]>();
            OperateResult<Socket> resultSocket = (OperateResult<Socket>)null;
            this.InteractiveLock.Enter();
            try
            {
                resultSocket = await this.GetAvailableSocketAsync();
                if (!resultSocket.IsSuccess)
                {
                    this.IsSocketError = true;
                    this.AlienSession?.Offline();
                    this.InteractiveLock.Leave();
                    result.CopyErrorFromOther<OperateResult<Socket>>(resultSocket);
                    return result;
                }
                OperateResult<byte, byte[]> read = await this.ReadMqttFromCoreServerAsync(resultSocket.Content, send, sendProgress, handleProgress, receiveProgress);
                if (read.IsSuccess)
                {
                    this.IsSocketError = false;
                    if ((int)read.Content1 >> 4 == 0)
                    {
                        OperateResult<string, byte[]> extra = MqttHelper.ExtraMqttReceiveData(read.Content1, read.Content2);
                        result.IsSuccess = false;
                        result.ErrorCode = int.Parse(extra.Content1);
                        result.Message = Encoding.UTF8.GetString(extra.Content2);
                        extra = (OperateResult<string, byte[]>)null;
                    }
                    else
                    {
                        result.IsSuccess = read.IsSuccess;
                        result.Content = read.Content2;
                        result.Message = StringResources.Language.SuccessText;
                    }
                }
                else
                {
                    this.IsSocketError = true;
                    this.AlienSession?.Offline();
                    result.CopyErrorFromOther<OperateResult<byte, byte[]>>(read);
                }
                this.ExtraAfterReadFromCoreServer((OperateResult)read);
                this.InteractiveLock.Leave();
                read = (OperateResult<byte, byte[]>)null;
            }
            catch
            {
                this.InteractiveLock.Leave();
                throw;
            }
            if (!this.isPersistentConn)
                resultSocket?.Content?.Close();
            return result;
        }

        /// <summary>
        /// 从MQTT服务器同步读取数据，将payload发送到服务器，然后从服务器返回相关的数据，支持数据发送进度报告，服务器执行进度报告，接收数据进度报告操作<br />
        /// Synchronously read data from the MQTT server, send the payload to the server, and then return relevant data from the server,
        /// support data transmission progress report, the server executes the progress report, and receives the data progress report
        /// </summary>
        /// <remarks>
        /// 进度报告可以实现一个比较有意思的功能，可以用来数据的上传和下载，提供一个友好的进度条，因为网络的好坏通常是不确定的。
        /// </remarks>
        /// <param name="topic">主题信息</param>
        /// <param name="payload">负载数据</param>
        /// <param name="sendProgress">发送数据给服务器时的进度报告，第一个参数为已发送数据，第二个参数为总发送数据</param>
        /// <param name="handleProgress">服务器处理数据的进度报告，第一个参数Topic自定义，通常用来传送操作百分比，第二个参数自定义，通常用来表示服务器消息</param>
        /// <param name="receiveProgress">从服务器接收数据的进度报告，第一个参数为已接收数据，第二个参数为总接收数据</param>
        /// <returns>服务器返回的数据信息</returns>
        public OperateResult<string, byte[]> Read(
          string topic,
          byte[] payload,
          Action<long, long> sendProgress = null,
          Action<string, string> handleProgress = null,
          Action<long, long> receiveProgress = null)
        {
            OperateResult<byte[]> operateResult1 = MqttHelper.BuildPublishMqttCommand(topic, payload);
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<string, byte[]>((OperateResult)operateResult1);
            OperateResult<byte[]> operateResult2 = this.ReadMqttFromCoreServer(operateResult1.Content, sendProgress, handleProgress, receiveProgress);
            return !operateResult2.IsSuccess ? OperateResult.CreateFailedResult<string, byte[]>((OperateResult)operateResult2) : MqttHelper.ExtraMqttReceiveData((byte)3, operateResult2.Content);
        }

        /// <summary>
        /// 从MQTT服务器同步读取数据，将指定编码的字符串payload发送到服务器，然后从服务器返回相关的数据，并转换为指定编码的字符串，支持数据发送进度报告，服务器执行进度报告，接收数据进度报告操作<br />
        /// Synchronously read data from the MQTT server, send the specified encoded string payload to the server,
        /// and then return the data from the server, and convert it to the specified encoded string,
        /// support data transmission progress report, the server executes the progress report, and receives the data progress report
        /// </summary>
        /// <param name="topic">主题信息</param>
        /// <param name="payload">负载数据</param>
        /// <param name="sendProgress">发送数据给服务器时的进度报告，第一个参数为已发送数据，第二个参数为总发送数据</param>
        /// <param name="handleProgress">服务器处理数据的进度报告，第一个参数Topic自定义，通常用来传送操作百分比，第二个参数自定义，通常用来表示服务器消息</param>
        /// <param name="receiveProgress">从服务器接收数据的进度报告，第一个参数为已接收数据，第二个参数为总接收数据</param>
        /// <returns>服务器返回的数据信息</returns>
        public OperateResult<string, string> ReadString(
          string topic,
          string payload,
          Action<long, long> sendProgress = null,
          Action<string, string> handleProgress = null,
          Action<long, long> receiveProgress = null)
        {
            OperateResult<string, byte[]> operateResult = this.Read(topic, string.IsNullOrEmpty(payload) ? (byte[])null : this.stringEncoding.GetBytes(payload), sendProgress, handleProgress, receiveProgress);
            return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<string, string>((OperateResult)operateResult) : OperateResult.CreateSuccessResult<string, string>(operateResult.Content1, this.stringEncoding.GetString(operateResult.Content2));
        }

        /// <summary>
        /// 读取MQTT服务器注册的RPC接口，忽略返回的Topic数据，直接将结果转换为泛型对象，如果JSON转换失败，将返回错误，参数传递主题和数据负载，
        /// 数据负载示例："{\"address\": \"100\",\"length\": 10}" 本质是一个字符串。<br />
        /// Read the RPC interface registered by the MQTT server, ignore the returned Topic data, and directly convert the result into a generic object.
        /// If the JSON conversion fails, an error will be returned. The parameter passes the topic and the data payload.
        /// The data payload example: "{\"address\ ": \"100\",\"length\": 10}" is essentially a string.
        /// </summary>
        /// <typeparam name="T">泛型对象，需要和返回的数据匹配，如果返回的是 int 数组，那么这里就是 int[]</typeparam>
        /// <param name="topic">主题信息，也是服务器的 RPC 接口信息</param>
        /// <param name="payload">传递的参数信息，示例："{\"address\": \"100\",\"length\": 10}" 本质是一个字符串。</param>
        /// <returns>服务器返回的数据信息</returns>
        public OperateResult<T> ReadRpc<T>(string topic, string payload)
        {
            OperateResult<string, string> operateResult = this.ReadString(topic, payload);
            if (!operateResult.IsSuccess)
                return operateResult.ConvertFailed<T>();
            try
            {
                return OperateResult.CreateSuccessResult<T>(JsonConvert.DeserializeObject<T>(operateResult.Content2));
            }
            catch (Exception ex)
            {
                return new OperateResult<T>("JSON failed: " + ex.Message);
            }
        }

        /// <summary>
        /// 读取MQTT服务器注册的RPC接口，忽略返回的Topic数据，直接将结果转换为泛型对象，如果JSON转换失败，将返回错误，参数传递主题和数据负载，
        /// 数据负载示例：new { address = "", length = 0 } 本质是一个匿名对象。<br />
        /// Read the RPC interface registered by the MQTT server, ignore the returned Topic data, and directly convert the result into a generic object.
        /// If the JSON conversion fails, an error will be returned. The parameter passes the topic and the data payload.
        /// The data payload example: new { address = "", length = 0 } is essentially an anonymous object.
        /// </summary>
        /// <typeparam name="T">泛型对象，需要和返回的数据匹配，如果返回的是 int 数组，那么这里就是 int[]</typeparam>
        /// <param name="topic">主题信息，也是服务器的 RPC 接口信息</param>
        /// <param name="payload">传递的参数信息，示例：new { address = "", length = 0 } 本质是一个匿名对象。</param>
        /// <returns>服务器返回的数据信息</returns>
        public OperateResult<T> ReadRpc<T>(string topic, object payload) => this.ReadRpc<T>(topic, payload == null ? "{}" : payload.ToJsonString());

        /// <summary>
        /// 读取服务器的已经注册的API信息列表，将返回API的主题路径，注释信息，示例的传入的数据信息。<br />
        /// Read the registered API information list of the server, and return the API subject path, annotation information, and sample incoming data information.
        /// </summary>
        /// <returns>包含是否成功的api信息的列表</returns>
        public OperateResult<MqttRpcApiInfo[]> ReadRpcApis()
        {
            OperateResult<byte[]> operateResult1 = MqttHelper.BuildMqttCommand((byte)8, (byte)0, MqttHelper.BuildSegCommandByString(""), (byte[])null);
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<MqttRpcApiInfo[]>((OperateResult)operateResult1);
            OperateResult<byte[]> operateResult2 = this.ReadMqttFromCoreServer(operateResult1.Content, (Action<long, long>)null, (Action<string, string>)null, (Action<long, long>)null);
            if (!operateResult2.IsSuccess)
                return OperateResult.CreateFailedResult<MqttRpcApiInfo[]>((OperateResult)operateResult2);
            OperateResult<string, byte[]> data = MqttHelper.ExtraMqttReceiveData((byte)3, operateResult2.Content);
            return !data.IsSuccess ? OperateResult.CreateFailedResult<MqttRpcApiInfo[]>((OperateResult)data) : OperateResult.CreateSuccessResult<MqttRpcApiInfo[]>(JArray.Parse(Encoding.UTF8.GetString(data.Content2)).ToObject<MqttRpcApiInfo[]>());
        }

        /// <summary>
        /// 读取服务器的指定的API接口的每天的调用次数，如果API接口不存在，或是还没有调用数据，则返回失败。<br />
        /// Read the number of calls per day of the designated API interface of the server.
        /// If the API interface does not exist or the data has not been called yet, it returns a failure.
        /// </summary>
        /// <remarks>如果api的参数为空字符串，就是请求所有的接口的调用的统计信息。</remarks>
        /// <param name="api">等待请求的API的接口信息，如果为空，就是请求所有的接口的调用的统计信息。</param>
        /// <returns>最近几日的连续的调用情况，例如[1,2,3]，表示前提调用1次，昨天调用2次，今天3次</returns>
        public OperateResult<long[]> ReadRpcApiLog(string api)
        {
            OperateResult<byte[]> operateResult1 = MqttHelper.BuildMqttCommand((byte)6, (byte)0, MqttHelper.BuildSegCommandByString(api), (byte[])null);
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<long[]>((OperateResult)operateResult1);
            OperateResult<byte[]> operateResult2 = this.ReadMqttFromCoreServer(operateResult1.Content, (Action<long, long>)null, (Action<string, string>)null, (Action<long, long>)null);
            if (!operateResult2.IsSuccess)
                return OperateResult.CreateFailedResult<long[]>((OperateResult)operateResult2);
            OperateResult<string, byte[]> data = MqttHelper.ExtraMqttReceiveData((byte)3, operateResult2.Content);
            return !data.IsSuccess ? OperateResult.CreateFailedResult<long[]>((OperateResult)data) : OperateResult.CreateSuccessResult<long[]>(Encoding.UTF8.GetString(data.Content2).ToStringArray<long>());
        }

        /// <summary>
        /// 读取服务器的已经驻留的所有消息的主题列表<br />
        /// Read the topic list of all messages that have resided on the server
        /// </summary>
        /// <returns>消息列表对象</returns>
        public OperateResult<string[]> ReadRetainTopics()
        {
            OperateResult<byte[]> operateResult1 = MqttHelper.BuildMqttCommand((byte)4, (byte)0, MqttHelper.BuildSegCommandByString(""), (byte[])null);
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<string[]>((OperateResult)operateResult1);
            OperateResult<byte[]> operateResult2 = this.ReadMqttFromCoreServer(operateResult1.Content, (Action<long, long>)null, (Action<string, string>)null, (Action<long, long>)null);
            if (!operateResult2.IsSuccess)
                return OperateResult.CreateFailedResult<string[]>((OperateResult)operateResult2);
            OperateResult<string, byte[]> data = MqttHelper.ExtraMqttReceiveData((byte)3, operateResult2.Content);
            return !data.IsSuccess ? OperateResult.CreateFailedResult<string[]>((OperateResult)data) : OperateResult.CreateSuccessResult<string[]>(EstProtocol.UnPackStringArrayFromByte(data.Content2));
        }

        /// <summary>
        /// 读取服务器的已经驻留的指定主题的消息内容<br />
        /// Read the topic list of all messages that have resided on the server
        /// </summary>
        /// <param name="topic">指定的主题消息</param>
        /// <param name="receiveProgress">结果进度报告</param>
        /// <returns>消息列表对象</returns>
        public OperateResult<MqttClientApplicationMessage> ReadTopicPayload(
          string topic,
          Action<long, long> receiveProgress = null)
        {
            OperateResult<byte[]> operateResult1 = MqttHelper.BuildMqttCommand((byte)5, (byte)0, MqttHelper.BuildSegCommandByString(topic), (byte[])null);
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<MqttClientApplicationMessage>((OperateResult)operateResult1);
            OperateResult<byte[]> operateResult2 = this.ReadMqttFromCoreServer(operateResult1.Content, (Action<long, long>)null, (Action<string, string>)null, receiveProgress);
            if (!operateResult2.IsSuccess)
                return OperateResult.CreateFailedResult<MqttClientApplicationMessage>((OperateResult)operateResult2);
            OperateResult<string, byte[]> data = MqttHelper.ExtraMqttReceiveData((byte)3, operateResult2.Content);
            return !data.IsSuccess ? OperateResult.CreateFailedResult<MqttClientApplicationMessage>((OperateResult)data) : OperateResult.CreateSuccessResult<MqttClientApplicationMessage>(JObject.Parse(Encoding.UTF8.GetString(data.Content2)).ToObject<MqttClientApplicationMessage>());
        }

        /// <inheritdoc cref="M:ESTCore.Common.MQTT.MqttSyncClient.Read(System.String,System.Byte[],System.Action{System.Int64,System.Int64},System.Action{System.String,System.String},System.Action{System.Int64,System.Int64})" />
        public async Task<OperateResult<string, byte[]>> ReadAsync(
          string topic,
          byte[] payload,
          Action<long, long> sendProgress = null,
          Action<string, string> handleProgress = null,
          Action<long, long> receiveProgress = null)
        {
            OperateResult<byte[]> command = MqttHelper.BuildPublishMqttCommand(topic, payload);
            if (!command.IsSuccess)
                return OperateResult.CreateFailedResult<string, byte[]>((OperateResult)command);
            OperateResult<byte[]> read = await this.ReadMqttFromCoreServerAsync(command.Content, sendProgress, handleProgress, receiveProgress);
            return read.IsSuccess ? MqttHelper.ExtraMqttReceiveData((byte)3, read.Content) : OperateResult.CreateFailedResult<string, byte[]>((OperateResult)read);
        }

        /// <inheritdoc cref="M:ESTCore.Common.MQTT.MqttSyncClient.ReadString(System.String,System.String,System.Action{System.Int64,System.Int64},System.Action{System.String,System.String},System.Action{System.Int64,System.Int64})" />
        public async Task<OperateResult<string, string>> ReadStringAsync(
          string topic,
          string payload,
          Action<long, long> sendProgress = null,
          Action<string, string> handleProgress = null,
          Action<long, long> receiveProgress = null)
        {
            OperateResult<string, byte[]> read = await this.ReadAsync(topic, string.IsNullOrEmpty(payload) ? (byte[])null : this.stringEncoding.GetBytes(payload), sendProgress, handleProgress, receiveProgress);
            OperateResult<string, string> operateResult = read.IsSuccess ? OperateResult.CreateSuccessResult<string, string>(read.Content1, this.stringEncoding.GetString(read.Content2)) : OperateResult.CreateFailedResult<string, string>((OperateResult)read);
            read = (OperateResult<string, byte[]>)null;
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.MQTT.MqttSyncClient.ReadRpc``1(System.String,System.String)" />
        public async Task<OperateResult<T>> ReadRpcAsync<T>(
          string topic,
          string payload)
        {
            OperateResult<string, string> read = await this.ReadStringAsync(topic, payload);
            if (!read.IsSuccess)
                return read.ConvertFailed<T>();
            try
            {
                return OperateResult.CreateSuccessResult<T>(JsonConvert.DeserializeObject<T>(read.Content2));
            }
            catch (Exception ex)
            {
                return new OperateResult<T>("JSON failed: " + ex.Message);
            }
        }

        /// <inheritdoc cref="M:ESTCore.Common.MQTT.MqttSyncClient.ReadRpc``1(System.String,System.Object)" />
        public async Task<OperateResult<T>> ReadRpcAsync<T>(
          string topic,
          object payload)
        {
            OperateResult<T> operateResult = await this.ReadRpcAsync<T>(topic, payload == null ? "{}" : payload.ToJsonString());
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.MQTT.MqttSyncClient.ReadRpcApis" />
        public async Task<OperateResult<MqttRpcApiInfo[]>> ReadRpcApisAsync()
        {
            OperateResult<byte[]> command = MqttHelper.BuildMqttCommand((byte)8, (byte)0, MqttHelper.BuildSegCommandByString(""), (byte[])null);
            if (!command.IsSuccess)
                return OperateResult.CreateFailedResult<MqttRpcApiInfo[]>((OperateResult)command);
            OperateResult<byte[]> read = await this.ReadMqttFromCoreServerAsync(command.Content, (Action<long, long>)null, (Action<string, string>)null, (Action<long, long>)null);
            if (!read.IsSuccess)
                return OperateResult.CreateFailedResult<MqttRpcApiInfo[]>((OperateResult)read);
            OperateResult<string, byte[]> mqtt = MqttHelper.ExtraMqttReceiveData((byte)3, read.Content);
            return mqtt.IsSuccess ? OperateResult.CreateSuccessResult<MqttRpcApiInfo[]>(JArray.Parse(Encoding.UTF8.GetString(mqtt.Content2)).ToObject<MqttRpcApiInfo[]>()) : OperateResult.CreateFailedResult<MqttRpcApiInfo[]>((OperateResult)mqtt);
        }

        /// <inheritdoc cref="M:ESTCore.Common.MQTT.MqttSyncClient.ReadRpcApiLog(System.String)" />
        public async Task<OperateResult<long[]>> ReadRpcApiLogAsync(string api)
        {
            OperateResult<byte[]> command = MqttHelper.BuildMqttCommand((byte)6, (byte)0, MqttHelper.BuildSegCommandByString(api), (byte[])null);
            if (!command.IsSuccess)
                return OperateResult.CreateFailedResult<long[]>((OperateResult)command);
            OperateResult<byte[]> read = await this.ReadMqttFromCoreServerAsync(command.Content, (Action<long, long>)null, (Action<string, string>)null, (Action<long, long>)null);
            if (!read.IsSuccess)
                return OperateResult.CreateFailedResult<long[]>((OperateResult)read);
            OperateResult<string, byte[]> mqtt = MqttHelper.ExtraMqttReceiveData((byte)3, read.Content);
            if (!mqtt.IsSuccess)
                return OperateResult.CreateFailedResult<long[]>((OperateResult)mqtt);
            string content = Encoding.UTF8.GetString(mqtt.Content2);
            return OperateResult.CreateSuccessResult<long[]>(content.ToStringArray<long>());
        }

        /// <inheritdoc cref="M:ESTCore.Common.MQTT.MqttSyncClient.ReadRetainTopics" />
        public async Task<OperateResult<string[]>> ReadRetainTopicsAsync()
        {
            OperateResult<byte[]> command = MqttHelper.BuildMqttCommand((byte)4, (byte)0, MqttHelper.BuildSegCommandByString(""), (byte[])null);
            if (!command.IsSuccess)
                return OperateResult.CreateFailedResult<string[]>((OperateResult)command);
            OperateResult<byte[]> read = await this.ReadMqttFromCoreServerAsync(command.Content, (Action<long, long>)null, (Action<string, string>)null, (Action<long, long>)null);
            if (!read.IsSuccess)
                return OperateResult.CreateFailedResult<string[]>((OperateResult)read);
            OperateResult<string, byte[]> mqtt = MqttHelper.ExtraMqttReceiveData((byte)3, read.Content);
            return mqtt.IsSuccess ? OperateResult.CreateSuccessResult<string[]>(EstProtocol.UnPackStringArrayFromByte(mqtt.Content2)) : OperateResult.CreateFailedResult<string[]>((OperateResult)mqtt);
        }

        /// <inheritdoc cref="M:ESTCore.Common.MQTT.MqttSyncClient.ReadTopicPayload(System.String,System.Action{System.Int64,System.Int64})" />
        public async Task<OperateResult<MqttClientApplicationMessage>> ReadTopicPayloadAsync(
          string topic,
          Action<long, long> receiveProgress = null)
        {
            OperateResult<byte[]> command = MqttHelper.BuildMqttCommand((byte)5, (byte)0, MqttHelper.BuildSegCommandByString(topic), (byte[])null);
            if (!command.IsSuccess)
                return OperateResult.CreateFailedResult<MqttClientApplicationMessage>((OperateResult)command);
            OperateResult<byte[]> read = await this.ReadMqttFromCoreServerAsync(command.Content, (Action<long, long>)null, (Action<string, string>)null, receiveProgress);
            if (!read.IsSuccess)
                return OperateResult.CreateFailedResult<MqttClientApplicationMessage>((OperateResult)read);
            OperateResult<string, byte[]> mqtt = MqttHelper.ExtraMqttReceiveData((byte)3, read.Content);
            return mqtt.IsSuccess ? OperateResult.CreateSuccessResult<MqttClientApplicationMessage>(JObject.Parse(Encoding.UTF8.GetString(mqtt.Content2)).ToObject<MqttClientApplicationMessage>()) : OperateResult.CreateFailedResult<MqttClientApplicationMessage>((OperateResult)mqtt);
        }

        private OperateResult<Socket> ConnectFileServer(
          byte code,
          string groups,
          string[] fileNames)
        {
            OperateResult<Socket> socketAndConnect = this.CreateSocketAndConnect(this.IpAddress, this.Port, this.ConnectTimeOut);
            if (!socketAndConnect.IsSuccess)
                return socketAndConnect;
            OperateResult<byte[]> operateResult = MqttHelper.BuildConnectMqttCommand(this.connectionOptions, "FILE");
            if (!operateResult.IsSuccess)
            {
                socketAndConnect.Content?.Close();
                return OperateResult.CreateFailedResult<Socket>((OperateResult)operateResult);
            }
            OperateResult result1 = this.Send(socketAndConnect.Content, operateResult.Content);
            if (!result1.IsSuccess)
                return OperateResult.CreateFailedResult<Socket>(result1);
            OperateResult<byte, byte[]> mqttMessage1 = this.ReceiveMqttMessage(socketAndConnect.Content, this.ReceiveTimeOut);
            if (!mqttMessage1.IsSuccess)
            {
                socketAndConnect.Content?.Close();
                return OperateResult.CreateFailedResult<Socket>((OperateResult)mqttMessage1);
            }
            OperateResult result2 = MqttHelper.CheckConnectBack(mqttMessage1.Content1, mqttMessage1.Content2);
            if (!result2.IsSuccess)
            {
                socketAndConnect.Content?.Close();
                return OperateResult.CreateFailedResult<Socket>(result2);
            }
            Socket content1 = socketAndConnect.Content;
            int num = (int)code;
            string[] data;
            if (!string.IsNullOrEmpty(groups))
                data = groups.Split(new char[2] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
            else
                data = (string[])null;
            byte[] payLoad = EstProtocol.PackStringArrayToByte(data);
            byte[] content2 = MqttHelper.BuildMqttCommand((byte)num, (byte[])null, payLoad).Content;
            OperateResult result3 = this.Send(content1, content2);
            if (!result3.IsSuccess)
                return OperateResult.CreateFailedResult<Socket>(result3);
            OperateResult result4 = this.Send(socketAndConnect.Content, MqttHelper.BuildMqttCommand(code, (byte[])null, EstProtocol.PackStringArrayToByte(fileNames)).Content);
            if (!result4.IsSuccess)
                return OperateResult.CreateFailedResult<Socket>(result4);
            OperateResult<byte, byte[]> mqttMessage2 = this.ReceiveMqttMessage(socketAndConnect.Content, 60000);
            if (!mqttMessage2.IsSuccess)
                return OperateResult.CreateFailedResult<Socket>((OperateResult)mqttMessage2);
            if (mqttMessage2.Content1 != (byte)0)
                return OperateResult.CreateSuccessResult<Socket>(socketAndConnect.Content);
            socketAndConnect.Content?.Close();
            return new OperateResult<Socket>(Encoding.UTF8.GetString(mqttMessage2.Content2));
        }

        private OperateResult DownloadFileBase(
          string groups,
          string fileName,
          Action<long, long> processReport,
          object source)
        {
            OperateResult<Socket> operateResult = this.ConnectFileServer((byte)101, groups, new string[1]
            {
        fileName
            });
            if (!operateResult.IsSuccess)
                return (OperateResult)operateResult;
            OperateResult mqttFile = (OperateResult)this.ReceiveMqttFile(operateResult.Content, source, processReport);
            if (!mqttFile.IsSuccess)
                return mqttFile;
            operateResult.Content?.Close();
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 从远程服务器下载一个文件到本地，需要指定文件类别，文件名，进度报告，本地保存的文件名<br />
        /// To download a file from a remote server to the local, you need to specify the file category, file name, progress report, and file name saved locally
        /// </summary>
        /// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分 </param>
        /// <param name="fileName">文件名称，例如 123.txt</param>
        /// <param name="processReport">进度报告，第一个参数是已完成字节数，第二个参数是总字节数</param>
        /// <param name="fileSaveName">本地保存的文件名</param>
        /// <returns>是否下载成功</returns>
        public OperateResult DownloadFile(
          string groups,
          string fileName,
          Action<long, long> processReport,
          string fileSaveName)
        {
            return this.DownloadFileBase(groups, fileName, processReport, (object)fileSaveName);
        }

        /// <summary>
        /// 从远程服务器下载一个文件到流中，需要指定文件类别，文件名，进度报告，本地保存的文件名<br />
        /// To download a file from a remote server to the stream, you need to specify the file category, file name, progress report, and file name saved locally
        /// </summary>
        /// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分 </param>
        /// <param name="fileName">文件名称，例如 123.txt</param>
        /// <param name="processReport">进度报告，第一个参数是已完成字节数，第二个参数是总字节数</param>
        /// <param name="stream">数据流</param>
        /// <returns>是否下载成功</returns>
        public OperateResult DownloadFile(
          string groups,
          string fileName,
          Action<long, long> processReport,
          Stream stream)
        {
            return this.DownloadFileBase(groups, fileName, processReport, (object)stream);
        }

        /// <summary>
        /// 从远程服务器下载一个文件，生成一个Bitmap图片对象，需要指定文件类别，文件名，进度报告，可用于用户头像的存储<br />
        /// Download a file from a remote server and generate a Bitmap image object. You need to specify the file category, file name, and progress report, which can be used to store the user's avatar
        /// </summary>
        /// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
        /// <param name="fileName">文件名称，例如 123.txt</param>
        /// <param name="processReport">进度报告，第一个参数是已完成字节数，第二个参数是总字节数</param>
        /// <returns>如果下载成功，则携带图片资源对象</returns>
        //public OperateResult<Bitmap> DownloadBitmap(
        //  string groups,
        //  string fileName,
        //  Action<long, long> processReport)
        //{
        //  MemoryStream memoryStream = new MemoryStream();
        //  OperateResult result = this.DownloadFileBase(groups, fileName, processReport, (object) memoryStream);
        //  if (!result.IsSuccess)
        //  {
        //    memoryStream.Dispose();
        //    return OperateResult.CreateFailedResult<Bitmap>(result);
        //  }
        //  Bitmap bitmap = new Bitmap((Stream) memoryStream);
        //  memoryStream.Dispose();
        //  return OperateResult.CreateSuccessResult<Bitmap>(bitmap);
        //}

        /// <summary>
        /// 上传一个Bitmap图片对象到服务器指定的分类下面，需要指定分类信息，服务器保存的文件名，描述信息，支持进度报告<br />
        /// Upload a Bitmap image object to the category specified by the server, you need to specify the category information,
        /// the file name saved by the server, description information, and support for progress reports
        /// </summary>
        /// <param name="bitmap">图片对象</param>
        /// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
        /// <param name="serverName">在服务器保存的文件名称</param>
        /// <param name="fileTag">文件的额外的描述信息</param>
        /// <param name="processReport">进度报告，第一个参数是已完成字节数，第二个参数是总字节数</param>
        /// <returns>是否上传成功</returns>
        //public OperateResult UploadFile(
        //  Bitmap bitmap,
        //  string groups,
        //  string serverName,
        //  string fileTag,
        //  Action<long, long> processReport)
        //{
        //  MemoryStream memoryStream = new MemoryStream();
        //  if (bitmap.RawFormat != null)
        //    bitmap.Save((Stream) memoryStream, bitmap.RawFormat);
        //  else
        //    bitmap.Save((Stream) memoryStream, ImageFormat.Bmp);
        //  OperateResult operateResult = this.UploadFileBase((object) memoryStream, groups, serverName, fileTag, processReport);
        //  memoryStream.Dispose();
        //  return operateResult;
        //}

        /// <summary>
        /// 上传文件给服务器，需要指定上传的数据内容，上传到服务器的分类信息，支持进度汇报功能。<br />
        /// To upload files to the server, you need to specify the content of the uploaded data,
        /// the classification information uploaded to the server, and support the progress report function.
        /// </summary>
        /// <param name="source">数据源，可以是文件名，也可以是数据流</param>
        /// <param name="serverName">在服务器保存的文件名，不包含驱动器路径</param>
        /// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
        /// <param name="fileTag">文件的额外的描述信息</param>
        /// <param name="processReport">进度报告，第一个参数是已完成字节数，第二个参数是总字节数</param>
        /// <returns>是否成功的结果对象</returns>
        private OperateResult UploadFileBase(
          object source,
          string groups,
          string serverName,
          string fileTag,
          Action<long, long> processReport)
        {
            OperateResult<Socket> operateResult1 = this.ConnectFileServer((byte)102, groups, new string[1]
            {
        serverName
            });
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            switch (source)
            {
                case string filename:
                    OperateResult operateResult2 = this.SendMqttFile(operateResult1.Content, filename, serverName, fileTag, processReport);
                    if (!operateResult2.IsSuccess)
                        return operateResult2;
                    break;
                case Stream stream:
                    OperateResult operateResult3 = this.SendMqttFile(operateResult1.Content, stream, serverName, fileTag, processReport);
                    if (!operateResult3.IsSuccess)
                        return operateResult3;
                    break;
                default:
                    operateResult1.Content?.Close();
                    this.LogNet?.WriteError(this.ToString(), StringResources.Language.DataSourceFormatError);
                    return new OperateResult(StringResources.Language.DataSourceFormatError);
            }
            OperateResult<byte, byte[]> mqttMessage = this.ReceiveMqttMessage(operateResult1.Content, 60000);
            if (!mqttMessage.IsSuccess)
                return (OperateResult)mqttMessage;
            operateResult1.Content?.Close();
            return mqttMessage.Content1 != (byte)0 ? OperateResult.CreateSuccessResult() : new OperateResult(Encoding.UTF8.GetString(mqttMessage.Content2));
        }

        /// <summary>
        /// 上传文件给服务器，需要指定上传文件的路径信息，服务器保存的名字，以及上传到服务器的分类信息，支持进度汇报功能。<br />
        /// To upload a file to the server, you need to specify the path information of the uploaded file, the name saved by the server,
        /// and the classification information uploaded to the server to support the progress report function.
        /// </summary>
        /// <param name="fileName">文件名，需要指定完整的路径信息，文件必须存在，否则发送失败</param>
        /// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
        /// <param name="serverName">服务器端保存的文件名</param>
        /// <param name="fileTag">文件的额外的描述信息</param>
        /// <param name="processReport">进度报告，第一个参数是已完成字节数，第二个参数是总字节数</param>
        /// <returns>是否上传成功的结果对象</returns>
        public OperateResult UploadFile(
          string fileName,
          string groups,
          string serverName,
          string fileTag,
          Action<long, long> processReport)
        {
            return !System.IO.File.Exists(fileName) ? new OperateResult(StringResources.Language.FileNotExist) : this.UploadFileBase((object)fileName, groups, serverName, fileTag, processReport);
        }

        /// <summary>
        /// 上传文件给服务器，需要指定上传文件的路径信息(服务器保存的名称就是文件名)，以及上传到服务器的分类信息，支持进度汇报功能。<br />
        /// To upload a file to the server, you need to specify the path information of the uploaded file (the name saved by the server is the file name),
        /// as well as the classification information uploaded to the server, to support the progress report function.
        /// </summary>
        /// <param name="fileName">文件名，需要指定完整的路径信息，文件必须存在，否则发送失败</param>
        /// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
        /// <param name="fileTag">文件的额外的描述信息</param>
        /// <param name="processReport">进度报告，第一个参数是已完成字节数，第二个参数是总字节数</param>
        /// <returns>是否上传成功的结果对象</returns>
        public OperateResult UploadFile(
          string fileName,
          string groups,
          string fileTag,
          Action<long, long> processReport)
        {
            if (!System.IO.File.Exists(fileName))
                return new OperateResult(StringResources.Language.FileNotExist);
            FileInfo fileInfo = new FileInfo(fileName);
            return this.UploadFileBase((object)fileName, groups, fileInfo.Name, fileTag, processReport);
        }

        private OperateResult<T[]> DownloadStringArrays<T>(
          byte protocol,
          string groups,
          string[] fileNames)
        {
            OperateResult<Socket> operateResult = this.ConnectFileServer(protocol, groups, fileNames);
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<T[]>((OperateResult)operateResult);
            OperateResult<byte, byte[]> mqttMessage = this.ReceiveMqttMessage(operateResult.Content, 60000);
            if (!mqttMessage.IsSuccess)
                return OperateResult.CreateFailedResult<T[]>((OperateResult)mqttMessage);
            operateResult.Content?.Close();
            try
            {
                return OperateResult.CreateSuccessResult<T[]>(JArray.Parse(Encoding.UTF8.GetString(mqttMessage.Content2)).ToObject<T[]>());
            }
            catch (Exception ex)
            {
                return new OperateResult<T[]>(ex.Message);
            }
        }

        /// <summary>
        /// 下载指定分类信息的所有的文件描述信息，需要指定分类信息，例如：Files/Personal/Admin<br />
        /// To download all the file description information of the specified classification information,
        /// you need to specify the classification information, for example: Files/Personal/Admin
        /// </summary>
        /// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
        /// <returns>当前分类下所有的文件描述信息</returns>
        public OperateResult<GroupFileItem[]> DownloadPathFileNames(
          string groups)
        {
            return this.DownloadStringArrays<GroupFileItem>((byte)105, groups, (string[])null);
        }

        /// <summary>
        /// 下载指定分类信息的全部子分类信息<br />
        /// Download all sub-category information of the specified category information
        /// </summary>
        /// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
        /// <returns>当前分类下所有的子分类信息</returns>
        public OperateResult<string[]> DownloadPathFolders(string groups) => this.DownloadStringArrays<string>((byte)106, groups, (string[])null);

        /// <summary>
        /// 请求服务器指定分类是否存在指定的文件名，需要指定分类信息，文件名<br />
        /// Request the server to specify whether the specified file name exists in the specified category, need to specify the category information, file name
        /// </summary>
        /// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
        /// <param name="fileName">文件名信息，例如 123.txt</param>
        /// <returns>Content为True表示存在，否则为不存在</returns>
        public OperateResult<bool> IsFileExists(string groups, string fileName)
        {
            OperateResult<Socket> operateResult = this.ConnectFileServer((byte)107, groups, new string[1]
            {
        fileName
            });
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<bool>((OperateResult)operateResult);
            OperateResult<byte, byte[]> mqttMessage = this.ReceiveMqttMessage(operateResult.Content, 60000);
            if (!mqttMessage.IsSuccess)
                return OperateResult.CreateFailedResult<bool>((OperateResult)mqttMessage);
            OperateResult<bool> successResult = OperateResult.CreateSuccessResult<bool>(mqttMessage.Content1 == (byte)1);
            operateResult.Content?.Close();
            return successResult;
        }

        /// <summary>
        /// 删除服务器的指定的文件名，需要指定分类信息，文件名<br />
        /// Delete the specified file name of the server, need to specify the classification information, file name
        /// </summary>
        /// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
        /// <param name="fileName">文件名信息</param>
        /// <returns>是否删除成功</returns>
        public OperateResult DeleteFile(string groups, string fileName) => this.DeleteFile(groups, new string[1]
        {
      fileName
        });

        /// <inheritdoc cref="M:ESTCore.Common.MQTT.MqttSyncClient.DeleteFile(System.String,System.String)" />
        public OperateResult DeleteFile(string groups, string[] fileNames)
        {
            OperateResult<Socket> operateResult = this.ConnectFileServer((byte)103, groups, fileNames);
            if (!operateResult.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<bool>((OperateResult)operateResult);
            OperateResult<byte, byte[]> mqttMessage = this.ReceiveMqttMessage(operateResult.Content, 60000);
            if (!mqttMessage.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<bool>((OperateResult)mqttMessage);
            operateResult.Content?.Close();
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 删除服务器上指定的分类信息及管理的所有的文件，包含所有的子分类信息，不可逆操作，谨慎操作。<br />
        /// Delete the specified classification information and all files managed on the server,
        /// including all sub-classification information, irreversible operation, and careful operation.
        /// </summary>
        /// <param name="groups">文件的类别，例如 Files/Personal/Admin 按照斜杠来区分</param>
        /// <returns>是否删除成功</returns>
        public OperateResult DeleteFolderFiles(string groups)
        {
            OperateResult<Socket> operateResult = this.ConnectFileServer((byte)104, groups, (string[])null);
            if (!operateResult.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<bool>((OperateResult)operateResult);
            OperateResult<byte, byte[]> mqttMessage = this.ReceiveMqttMessage(operateResult.Content, 60000);
            if (!mqttMessage.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<bool>((OperateResult)mqttMessage);
            operateResult.Content?.Close();
            return OperateResult.CreateSuccessResult();
        }

        private async Task<OperateResult<Socket>> ConnectFileServerAsync(
          byte code,
          string groups,
          string[] fileNames)
        {
            OperateResult<Socket> socketResult = await this.CreateSocketAndConnectAsync(this.IpAddress, this.Port, this.ConnectTimeOut);
            if (!socketResult.IsSuccess)
                return socketResult;
            OperateResult<byte[]> command = MqttHelper.BuildConnectMqttCommand(this.connectionOptions, "FILE");
            if (!command.IsSuccess)
            {
                socketResult.Content?.Close();
                return OperateResult.CreateFailedResult<Socket>((OperateResult)command);
            }
            OperateResult send = await this.SendAsync(socketResult.Content, command.Content);
            if (!send.IsSuccess)
                return OperateResult.CreateFailedResult<Socket>(send);
            OperateResult<byte, byte[]> receive = await this.ReceiveMqttMessageAsync(socketResult.Content, this.ReceiveTimeOut);
            if (!receive.IsSuccess)
            {
                socketResult.Content?.Close();
                return OperateResult.CreateFailedResult<Socket>((OperateResult)receive);
            }
            OperateResult check = MqttHelper.CheckConnectBack(receive.Content1, receive.Content2);
            if (!check.IsSuccess)
            {
                socketResult.Content?.Close();
                return OperateResult.CreateFailedResult<Socket>(check);
            }
            Socket content1 = socketResult.Content;
            int num = (int)code;
            string[] data;
            if (!string.IsNullOrEmpty(groups))
                data = groups.Split(new char[2] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
            else
                data = (string[])null;
            byte[] payLoad = EstProtocol.PackStringArrayToByte(data);
            byte[] content2 = MqttHelper.BuildMqttCommand((byte)num, (byte[])null, payLoad).Content;
            OperateResult sendClass = await this.SendAsync(content1, content2);
            if (!sendClass.IsSuccess)
                return OperateResult.CreateFailedResult<Socket>(sendClass);
            OperateResult sendString = await this.SendAsync(socketResult.Content, MqttHelper.BuildMqttCommand(code, (byte[])null, EstProtocol.PackStringArrayToByte(fileNames)).Content);
            if (!sendString.IsSuccess)
                return OperateResult.CreateFailedResult<Socket>(sendString);
            OperateResult<byte, byte[]> legal = await this.ReceiveMqttMessageAsync(socketResult.Content, 60000);
            if (!legal.IsSuccess)
                return OperateResult.CreateFailedResult<Socket>((OperateResult)legal);
            if (legal.Content1 != (byte)0)
                return OperateResult.CreateSuccessResult<Socket>(socketResult.Content);
            socketResult.Content?.Close();
            return new OperateResult<Socket>(Encoding.UTF8.GetString(legal.Content2));
        }

        private async Task<OperateResult> DownloadFileBaseAsync(
          string groups,
          string fileName,
          Action<long, long> processReport,
          object source)
        {
            OperateResult<Socket> socketResult = await this.ConnectFileServerAsync((byte)101, groups, new string[1]
            {
        fileName
            });
            if (!socketResult.IsSuccess)
                return (OperateResult)socketResult;
            OperateResult<FileBaseInfo> operateResult = await this.ReceiveMqttFileAsync(socketResult.Content, source, processReport);
            OperateResult result = (OperateResult)operateResult;
            operateResult = (OperateResult<FileBaseInfo>)null;
            if (!result.IsSuccess)
                return result;
            socketResult.Content?.Close();
            return OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc cref="M:ESTCore.Common.MQTT.MqttSyncClient.DownloadFile(System.String,System.String,System.Action{System.Int64,System.Int64},System.String)" />
        public async Task<OperateResult> DownloadFileAsync(
          string groups,
          string fileName,
          Action<long, long> processReport,
          string fileSaveName)
        {
            OperateResult operateResult = await this.DownloadFileBaseAsync(groups, fileName, processReport, (object)fileSaveName);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.MQTT.MqttSyncClient.DownloadFile(System.String,System.String,System.Action{System.Int64,System.Int64},System.IO.Stream)" />
        public async Task<OperateResult> DownloadFileAsync(
          string groups,
          string fileName,
          Action<long, long> processReport,
          Stream stream)
        {
            OperateResult operateResult = await this.DownloadFileBaseAsync(groups, fileName, processReport, (object)stream);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.MQTT.MqttSyncClient.DownloadBitmap(System.String,System.String,System.Action{System.Int64,System.Int64})" />
        //public async Task<OperateResult<Bitmap>> DownloadBitmapAsync(
        //  string groups,
        //  string fileName,
        //  Action<long, long> processReport)
        //{
        //  MemoryStream stream = new MemoryStream();
        //  Bitmap bitmap = (Bitmap) null;
        //  OperateResult result = await this.DownloadFileBaseAsync(groups, fileName, processReport, (object) stream);
        //  if (!result.IsSuccess)
        //  {
        //    stream.Dispose();
        //    return OperateResult.CreateFailedResult<Bitmap>(result);
        //  }
        //  bitmap = new Bitmap((Stream) stream);
        //  stream.Dispose();
        //  return OperateResult.CreateSuccessResult<Bitmap>(bitmap);
        //}

        ///// <inheritdoc cref="M:ESTCore.Common.MQTT.MqttSyncClient.UploadFile(System.Drawing.Bitmap,System.String,System.String,System.String,System.Action{System.Int64,System.Int64})" />
        //public async Task<OperateResult> UploadFileAsync(
        //  Bitmap bitmap,
        //  string groups,
        //  string serverName,
        //  string fileTag,
        //  Action<long, long> processReport)
        //{
        //  MemoryStream stream = new MemoryStream();
        //  if (bitmap.RawFormat != null)
        //    bitmap.Save((Stream) stream, bitmap.RawFormat);
        //  else
        //    bitmap.Save((Stream) stream, ImageFormat.Bmp);
        //  OperateResult result = await this.UploadFileBaseAsync((object) stream, groups, serverName, fileTag, processReport);
        //  stream.Dispose();
        //  OperateResult operateResult = result;
        //  stream = (MemoryStream) null;
        //  result = (OperateResult) null;
        //  return operateResult;
        //}

        /// <inheritdoc cref="M:ESTCore.Common.MQTT.MqttSyncClient.UploadFileBase(System.Object,System.String,System.String,System.String,System.Action{System.Int64,System.Int64})" />
        private async Task<OperateResult> UploadFileBaseAsync(
          object source,
          string groups,
          string serverName,
          string fileTag,
          Action<long, long> processReport)
        {
            OperateResult<Socket> socketResult = await this.ConnectFileServerAsync((byte)102, groups, new string[1]
            {
        serverName
            });
            if (!socketResult.IsSuccess)
                return (OperateResult)socketResult;
            switch (source)
            {
                case string fileName:
                    OperateResult result1 = await this.SendMqttFileAsync(socketResult.Content, fileName, serverName, fileTag, processReport);
                    if (!result1.IsSuccess)
                        return result1;
                    result1 = (OperateResult)null;
                    break;
                case Stream stream:
                    OperateResult result2 = await this.SendMqttFileAsync(socketResult.Content, stream, serverName, fileTag, processReport);
                    if (!result2.IsSuccess)
                        return result2;
                    result2 = (OperateResult)null;
                    stream = (Stream)null;
                    break;
                default:
                    socketResult.Content?.Close();
                    this.LogNet?.WriteError(this.ToString(), StringResources.Language.DataSourceFormatError);
                    return new OperateResult(StringResources.Language.DataSourceFormatError);
            }
            OperateResult<byte, byte[]> resultCheck = await this.ReceiveMqttMessageAsync(socketResult.Content, 60000);
            if (!resultCheck.IsSuccess)
                return (OperateResult)resultCheck;
            socketResult.Content?.Close();
            return resultCheck.Content1 != (byte)0 ? OperateResult.CreateSuccessResult() : new OperateResult(Encoding.UTF8.GetString(resultCheck.Content2));
        }

        /// <inheritdoc cref="M:ESTCore.Common.MQTT.MqttSyncClient.UploadFile(System.String,System.String,System.String,System.String,System.Action{System.Int64,System.Int64})" />
        public async Task<OperateResult> UploadFileAsync(
          string fileName,
          string groups,
          string serverName,
          string fileTag,
          Action<long, long> processReport)
        {
            if (!System.IO.File.Exists(fileName))
                return new OperateResult(StringResources.Language.FileNotExist);
            OperateResult operateResult = await this.UploadFileBaseAsync((object)fileName, groups, serverName, fileTag, processReport);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.MQTT.MqttSyncClient.UploadFile(System.String,System.String,System.String,System.Action{System.Int64,System.Int64})" />
        public async Task<OperateResult> UploadFileAsync(
          string fileName,
          string groups,
          string fileTag,
          Action<long, long> processReport)
        {
            if (!System.IO.File.Exists(fileName))
                return new OperateResult(StringResources.Language.FileNotExist);
            FileInfo fileInfo = new FileInfo(fileName);
            OperateResult operateResult = await this.UploadFileBaseAsync((object)fileName, groups, fileInfo.Name, fileTag, processReport);
            return operateResult;
        }

        private async Task<OperateResult<T[]>> DownloadStringArraysAsync<T>(
          byte protocol,
          string groups,
          string[] fileNames)
        {
            OperateResult<Socket> socketResult = await this.ConnectFileServerAsync(protocol, groups, fileNames);
            if (!socketResult.IsSuccess)
                return OperateResult.CreateFailedResult<T[]>((OperateResult)socketResult);
            OperateResult<byte, byte[]> receive = await this.ReceiveMqttMessageAsync(socketResult.Content, 60000);
            if (!receive.IsSuccess)
                return OperateResult.CreateFailedResult<T[]>((OperateResult)receive);
            socketResult.Content?.Close();
            try
            {
                return OperateResult.CreateSuccessResult<T[]>(JArray.Parse(Encoding.UTF8.GetString(receive.Content2)).ToObject<T[]>());
            }
            catch (Exception ex)
            {
                return new OperateResult<T[]>(ex.Message);
            }
        }

        /// <inheritdoc cref="M:ESTCore.Common.MQTT.MqttSyncClient.DownloadPathFileNames(System.String)" />
        public async Task<OperateResult<GroupFileItem[]>> DownloadPathFileNamesAsync(
          string groups)
        {
            OperateResult<GroupFileItem[]> operateResult = await this.DownloadStringArraysAsync<GroupFileItem>((byte)105, groups, (string[])null);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.MQTT.MqttSyncClient.DownloadPathFolders(System.String)" />
        public async Task<OperateResult<string[]>> DownloadPathFoldersAsync(
          string groups)
        {
            OperateResult<string[]> operateResult = await this.DownloadStringArraysAsync<string>((byte)106, groups, (string[])null);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.MQTT.MqttSyncClient.IsFileExists(System.String,System.String)" />
        public async Task<OperateResult<bool>> IsFileExistsAsync(
          string groups,
          string fileName)
        {
            OperateResult<Socket> socketResult = await this.ConnectFileServerAsync((byte)107, groups, new string[1]
            {
        fileName
            });
            if (!socketResult.IsSuccess)
                return OperateResult.CreateFailedResult<bool>((OperateResult)socketResult);
            OperateResult<byte, byte[]> receiveBack = await this.ReceiveMqttMessageAsync(socketResult.Content, 60000);
            if (!receiveBack.IsSuccess)
                return OperateResult.CreateFailedResult<bool>((OperateResult)receiveBack);
            OperateResult<bool> result = OperateResult.CreateSuccessResult<bool>(receiveBack.Content1 == (byte)1);
            socketResult.Content?.Close();
            return result;
        }

        /// <inheritdoc cref="M:ESTCore.Common.MQTT.MqttSyncClient.DeleteFile(System.String,System.String)" />
        public async Task<OperateResult> DeleteFileAsync(
          string groups,
          string fileName)
        {
            OperateResult operateResult = await this.DeleteFileAsync(groups, new string[1]
            {
        fileName
            });
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.MQTT.MqttSyncClient.DeleteFile(System.String,System.String[])" />
        public async Task<OperateResult> DeleteFileAsync(
          string groups,
          string[] fileNames)
        {
            OperateResult<Socket> socketResult = await this.ConnectFileServerAsync((byte)103, groups, fileNames);
            if (!socketResult.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<bool>((OperateResult)socketResult);
            OperateResult<byte, byte[]> receiveBack = await this.ReceiveMqttMessageAsync(socketResult.Content, 60000);
            if (!receiveBack.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<bool>((OperateResult)receiveBack);
            socketResult.Content?.Close();
            return OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc cref="M:ESTCore.Common.MQTT.MqttSyncClient.DeleteFolderFiles(System.String)" />
        public async Task<OperateResult> DeleteFolderFilesAsync(string groups)
        {
            OperateResult<Socket> socketResult = await this.ConnectFileServerAsync((byte)104, groups, (string[])null);
            if (!socketResult.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<bool>((OperateResult)socketResult);
            OperateResult<byte, byte[]> receiveBack = await this.ReceiveMqttMessageAsync(socketResult.Content, 60000);
            if (!receiveBack.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<bool>((OperateResult)receiveBack);
            socketResult.Content?.Close();
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 获取或设置当前的连接信息，客户端将根据这个连接配置进行连接服务器，在连接之前需要设置相关的信息才有效。<br />
        /// To obtain or set the current connection information, the client will connect to the server according to this connection configuration.
        /// Before connecting, the relevant information needs to be set to be effective.
        /// </summary>
        public MqttConnectionOptions ConnectionOptions
        {
            get => this.connectionOptions;
            set => this.connectionOptions = value;
        }

        /// <summary>
        /// 获取或设置使用字符串访问的时候，使用的编码信息，默认为UT8编码<br />
        /// Get or set the encoding information used when accessing with a string, the default is UT8 encoding
        /// </summary>
        public Encoding StringEncoding
        {
            get => this.stringEncoding;
            set => this.stringEncoding = value;
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("MqttSyncClient[{0}:{1}]", (object)this.connectionOptions.IpAddress, (object)this.connectionOptions.Port);
    }
}
