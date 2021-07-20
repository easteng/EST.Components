// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Enthernet.HttpServer
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;
using ESTCore.Common.LogNet;
using ESTCore.Common.MQTT;
using ESTCore.Common.Reflection;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ESTCore.Common.Enthernet
{
    /// <summary>
    /// 一个支持完全自定义的Http服务器，支持返回任意的数据信息，方便调试信息，详细的案例请查看API文档信息<br />
    /// A Http server that supports fully customized, supports returning arbitrary data information, which is convenient for debugging information. For detailed cases, please refer to the API documentation information
    /// </summary>
    /// <example>
    /// 我们先来看看一个最简单的例子，如何进行实例化的操作。
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Enthernet\HttpServerSample.cs" region="Sample1" title="基本的实例化" />
    /// 通常来说，基本的实例化，返回固定的数据并不能满足我们的需求，我们需要返回自定义的数据，有一个委托，我们需要自己指定方法.
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Enthernet\HttpServerSample.cs" region="Sample2" title="自定义返回" />
    /// 我们实际的需求可能会更加的复杂，不同的网址会返回不同的数据，所以接下来我们需要对网址信息进行判断。
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Enthernet\HttpServerSample.cs" region="Sample3" title="区分网址" />
    /// 如果我们想增加安全性的验证功能，比如我们的api接口需要增加用户名和密码的功能，那么我们也可以实现
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Enthernet\HttpServerSample.cs" region="Sample4" title="安全实现" />
    /// 当然了，如果我们想反回一个完整的html网页，也是可以实现的，甚至添加一些js的脚本，下面的例子就简单的说明了如何操作
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Enthernet\HttpServerSample.cs" region="Sample5" title="返回html" />
    /// 如果需要实现跨域的操作，可以将属性<see cref="P:ESTCore.Common.Enthernet.HttpServer.IsCrossDomain" /> 设置为<c>True</c>
    /// </example>
    public class HttpServer
    {
        private Dictionary<string, MqttRpcApiInfo> apiTopicServiceDict;
        private object rpcApiLock;
        private int receiveBufferSize = 2048;
        private int port = 80;
        private HttpListener listener;
        private ILogNet logNet;
        private Encoding encoding = Encoding.UTF8;
        private Func<HttpListenerRequest, HttpListenerResponse, string, string> handleRequestFunc;
        private LogStatisticsDict statisticsDict;

        /// <summary>
        /// 实例化一个默认的对象，当前的运行，需要使用管理员的模式运行<br />
        /// Instantiate a default object, the current operation, you need to use the administrator mode to run
        /// </summary>
        public HttpServer()
        {
            this.statisticsDict = new LogStatisticsDict(GenerateMode.ByEveryDay, 60);
            this.apiTopicServiceDict = new Dictionary<string, MqttRpcApiInfo>();
            this.rpcApiLock = new object();
        }

        /// <summary>
        /// 启动服务器，正常调用该方法时，应该使用try...catch...来捕获错误信息<br />
        /// Start the server and use try...catch... to capture the error message when calling this method normally
        /// </summary>
        /// <param name="port">端口号信息</param>
        /// <exception cref="T:System.Net.HttpListenerException"></exception>
        /// <exception cref="T:System.ObjectDisposedException"></exception>
        public void Start(int port)
        {
            this.port = port;
            this.listener = new HttpListener();
            this.listener.Prefixes.Add(string.Format("http://+:{0}/", (object)port));
            this.listener.Start();
            this.listener.BeginGetContext(new AsyncCallback(this.GetConnectCallBack), (object)this.listener);
            this.logNet?.WriteDebug(this.ToString(), "Server Started, wait for connections");
        }

        /// <summary>
        /// 关闭服务器<br />
        /// Shut down the server
        /// </summary>
        public void Close() => this.listener?.Close();

        private async void GetConnectCallBack(IAsyncResult ar)
        {
            if (!(ar.AsyncState is HttpListener listener))
            {
                listener = (HttpListener)null;
            }
            else
            {
                HttpListenerContext context = (HttpListenerContext)null;
                try
                {
                    context = listener.EndGetContext(ar);
                }
                catch (Exception ex)
                {
                    this.logNet?.WriteException(this.ToString(), ex);
                }
                int restartcount = 0;
                while (true)
                {
                    try
                    {
                        listener.BeginGetContext(new AsyncCallback(this.GetConnectCallBack), (object)listener);
                        break;
                    }
                    catch (Exception ex)
                    {
                        this.logNet?.WriteException(this.ToString(), ex);
                        ++restartcount;
                        if (restartcount >= 3)
                        {
                            ILogNet logNet = this.logNet;
                            if (logNet == null)
                            {
                                listener = (HttpListener)null;
                                return;
                            }
                            logNet.WriteError(this.ToString(), "ReGet Content Failed!");
                            listener = (HttpListener)null;
                            return;
                        }
                        Thread.Sleep(1000);
                    }
                }
                if (context == null)
                {
                    listener = (HttpListener)null;
                }
                else
                {
                    HttpListenerRequest request = context.Request;
                    HttpListenerResponse response = context.Response;
                    if (response != null)
                    {
                        try
                        {
                            if (this.IsCrossDomain)
                            {
                                context.Response.AppendHeader("Access-Control-Allow-Origin", request.Headers["Origin"]);
                                context.Response.AppendHeader("Access-Control-Allow-Headers", "*");
                                context.Response.AppendHeader("Access-Control-Allow-Method", "POST,GET,PUT,OPTIONS,DELETE");
                                context.Response.AppendHeader("Access-Control-Allow-Credentials", "true");
                                context.Response.AppendHeader("Access-Control-Max-Age", "3600");
                            }
                            context.Response.AddHeader("Content-type", "Content-Type: text/html; charset=utf-8");
                        }
                        catch (Exception ex)
                        {
                            this.logNet?.WriteError(this.ToString(), ex.Message);
                        }
                    }
                    string data = await this.GetDataFromRequestAsync(request);
                    response.StatusCode = 200;
                    try
                    {
                        string ret = this.HandleRequest(request, response, data);
                        using (Stream stream = response.OutputStream)
                        {
                            if (string.IsNullOrEmpty(ret))
                            {
                                await stream.WriteAsync(new byte[0], 0, 0);
                            }
                            else
                            {
                                byte[] buffer = this.encoding.GetBytes(ret);
                                await stream.WriteAsync(buffer, 0, buffer.Length);
                                buffer = (byte[])null;
                            }
                        }
                        ret = (string)null;
                    }
                    catch (Exception ex)
                    {
                        this.logNet?.WriteException(this.ToString(), "Handle Request[" + request.HttpMethod + "], " + request.RawUrl, ex);
                    }
                    context = (HttpListenerContext)null;
                    request = (HttpListenerRequest)null;
                    response = (HttpListenerResponse)null;
                    data = (string)null;
                    listener = (HttpListener)null;
                }
            }
        }

        private string GetDataFromRequest(HttpListenerRequest request)
        {
            try
            {
                List<byte> byteList = new List<byte>();
                byte[] buffer = new byte[this.receiveBufferSize];
                int count = 0;
                int length;
                do
                {
                    length = request.InputStream.Read(buffer, 0, buffer.Length);
                    count += length;
                    byteList.AddRange((IEnumerable<byte>)SoftBasic.ArraySelectBegin<byte>(buffer, length));
                }
                while ((uint)length > 0U);
                return this.encoding.GetString(byteList.ToArray(), 0, count);
            }
            catch
            {
                return string.Empty;
            }
        }

        private async Task<string> GetDataFromRequestAsync(HttpListenerRequest request)
        {
            try
            {
                List<byte> byteList = new List<byte>();
                byte[] byteArr = new byte[this.receiveBufferSize];
                int readLen = 0;
                int len = 0;
                do
                {
                    readLen = await request.InputStream.ReadAsync(byteArr, 0, byteArr.Length);
                    len += readLen;
                    byteList.AddRange((IEnumerable<byte>)SoftBasic.ArraySelectBegin<byte>(byteArr, readLen));
                }
                while ((uint)readLen > 0U);
                return this.encoding.GetString(byteList.ToArray(), 0, len);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 根据客户端的请求进行处理的核心方法，可以返回自定义的数据内容，只需要集成重写即可。<br />
        /// The core method of processing according to the client's request can return custom data content, and only needs to be integrated and rewritten.
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="response">回应</param>
        /// <param name="data">Body数据</param>
        /// <returns>返回的内容</returns>
        protected virtual string HandleRequest(
          HttpListenerRequest request,
          HttpListenerResponse response,
          string data)
        {
            if (request.HttpMethod == "HSL")
            {
                if (request.RawUrl.StartsWith("/Apis"))
                {
                    response.AddHeader("Content-type", "Content-Type: application/json; charset=utf-8");
                    return this.GetAllRpcApiInfo().ToJsonString();
                }
                if (request.RawUrl.StartsWith("/Logs"))
                {
                    response.AddHeader("Content-type", "Content-Type: application/json; charset=utf-8");
                    return request.RawUrl == "/Logs" || request.RawUrl == "/Logs/" ? this.LogStatistics.LogStat.GetStatisticsSnapshot().ToJsonString() : this.LogStatistics.GetStatisticsSnapshot(request.RawUrl.Substring(6)).ToJsonString();
                }
                response.AddHeader("Content-type", "Content-Type: application/json; charset=utf-8");
                return this.GetAllRpcApiInfo().ToJsonString();
            }
            if (request.HttpMethod == "OPTIONS")
                return "OK";
            MqttRpcApiInfo mqttRpcApiInfo = this.GetMqttRpcApiInfo(HttpServer.GetMethodName(HttpUtility.UrlDecode(request.RawUrl)));
            if (mqttRpcApiInfo == null)
                return this.HandleRequestFunc != null ? this.HandleRequestFunc(request, response, data) : "This is EstWebServer, Thank you for use!";
            response.AddHeader("Content-type", "Content-Type: application/json; charset=utf-8");
            DateTime now = DateTime.Now;
            string str = HttpServer.HandleObjectMethod(request, HttpUtility.UrlDecode(request.RawUrl), data, mqttRpcApiInfo);
            double num = Math.Round((DateTime.Now - now).TotalSeconds, 5);
            mqttRpcApiInfo.CalledCountAddOne((long)(num * 100000.0));
            this.statisticsDict.StatisticsAdd(mqttRpcApiInfo.ApiTopic);
            this.LogNet?.WriteDebug(this.ToString(), string.Format("[{0}] HttpRpc request:[{1}] Spend:[{2:F2} ms] Count:[{3}]", (object)request.RemoteEndPoint, (object)mqttRpcApiInfo.ApiTopic, (object)(num * 1000.0), (object)mqttRpcApiInfo.CalledCount));
            return str;
        }

        /// <summary>
        /// 获取当前的日志统计信息，可以获取到每个API的每天的调度次数信息，缓存60天数据，如果需要存储本地，需要调用<see cref="M:ESTCore.Common.LogNet.LogStatisticsDict.SaveToFile(System.String)" />方法。<br />
        /// Get the current log statistics, you can get the daily scheduling times information of each API, and cache 60-day data.
        /// If you need to store it locally, you need to call the <see cref="M:ESTCore.Common.LogNet.LogStatisticsDict.SaveToFile(System.String)" /> method.
        /// </summary>
        public LogStatisticsDict LogStatistics => this.statisticsDict;

        /// <inheritdoc cref="P:ESTCore.Common.Core.Net.NetworkBase.LogNet" />
        public ILogNet LogNet
        {
            get => this.logNet;
            set => this.logNet = value;
        }

        /// <summary>
        /// 获取或设置当前服务器的编码信息，默认为UTF8编码<br />
        /// Get or set the encoding information of the current server, the default is UTF8 encoding
        /// </summary>
        public Encoding ServerEncoding
        {
            get => this.encoding;
            set => this.encoding = value;
        }

        /// <summary>
        /// 获取或设置是否支持跨域操作<br />
        /// Get or set whether to support cross-domain operations
        /// </summary>
        public bool IsCrossDomain { get; set; }

        /// <summary>
        /// 获取或设置当前的自定义的处理信息，如果不想继承实现方法，可以使用本属性来关联你自定义的方法。<br />
        /// Get or set the current custom processing information. If you don't want to inherit the implementation method, you can use this attribute to associate your custom method.
        /// </summary>
        public Func<HttpListenerRequest, HttpListenerResponse, string, string> HandleRequestFunc
        {
            get => this.handleRequestFunc;
            set => this.handleRequestFunc = value;
        }

        /// <summary>
        /// 获取当前的端口号信息<br />
        /// Get current port number information
        /// </summary>
        public int Port => this.port;

        private MqttRpcApiInfo GetMqttRpcApiInfo(string apiTopic)
        {
            MqttRpcApiInfo mqttRpcApiInfo = (MqttRpcApiInfo)null;
            lock (this.rpcApiLock)
            {
                if (this.apiTopicServiceDict.ContainsKey(apiTopic))
                    mqttRpcApiInfo = this.apiTopicServiceDict[apiTopic];
            }
            return mqttRpcApiInfo;
        }

        /// <summary>
        /// 获取当前所有注册的RPC接口信息，将返回一个数据列表。<br />
        /// Get all currently registered RPC interface information, and a data list will be returned.
        /// </summary>
        /// <returns>信息列表</returns>
        public MqttRpcApiInfo[] GetAllRpcApiInfo()
        {
            MqttRpcApiInfo[] mqttRpcApiInfoArray = (MqttRpcApiInfo[])null;
            lock (this.rpcApiLock)
                mqttRpcApiInfoArray = this.apiTopicServiceDict.Values.ToArray<MqttRpcApiInfo>();
            return mqttRpcApiInfoArray;
        }

        /// <summary>
        /// 注册一个RPC的服务接口，可以指定当前的控制器名称，以及提供RPC服务的原始对象<br />
        /// Register an RPC service interface, you can specify the current controller name,
        /// and the original object that provides the RPC service
        /// </summary>
        /// <param name="api">前置的接口信息，可以理解为MVC模式的控制器</param>
        /// <param name="obj">原始对象信息</param>
        public void RegisterHttpRpcApi(string api, object obj)
        {
            lock (this.rpcApiLock)
            {
                foreach (MqttRpcApiInfo mqttRpcApiInfo in MqttHelper.GetSyncServicesApiInformationFromObject(api, obj))
                    this.apiTopicServiceDict.Add(mqttRpcApiInfo.ApiTopic, mqttRpcApiInfo);
            }
        }

        /// <inheritdoc cref="M:ESTCore.Common.Enthernet.HttpServer.RegisterHttpRpcApi(System.String,System.Object)" />
        public void RegisterHttpRpcApi(object obj)
        {
            lock (this.rpcApiLock)
            {
                foreach (MqttRpcApiInfo mqttRpcApiInfo in MqttHelper.GetSyncServicesApiInformationFromObject(obj))
                    this.apiTopicServiceDict.Add(mqttRpcApiInfo.ApiTopic, mqttRpcApiInfo);
            }
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("HttpServer[{0}]", (object)this.port);

        /// <summary>
        /// 使用指定的对象来返回网络的API接口，前提是传入的数据为json参数，返回的数据为json数据，详细参照说明<br />
        /// Use the specified object to return the API interface of the network,
        /// provided that the incoming data is json parameters and the returned data is json data,
        /// please refer to the description for details
        /// </summary>
        /// <param name="request">当前的请求信息</param>
        /// <param name="deceodeUrl">已经解码过的Url地址信息</param>
        /// <param name="json">json格式的参数信息</param>
        /// <param name="obj">等待解析的api解析的对象</param>
        /// <returns>等待返回客户的结果</returns>
        public static string HandleObjectMethod(
          HttpListenerRequest request,
          string deceodeUrl,
          string json,
          object obj)
        {
            string name = HttpServer.GetMethodName(deceodeUrl);
            if (name.LastIndexOf('/') >= 0)
                name = name.Substring(name.LastIndexOf('/') + 1);
            MethodInfo method = obj.GetType().GetMethod(name);
            if (method == (MethodInfo)null)
                return new OperateResult<string>("Current MqttSync Api ：[" + name + "] not exsist").ToJsonString();
            OperateResult<MqttRpcApiInfo> servicesApiFromMethod = MqttHelper.GetMqttSyncServicesApiFromMethod("", method, obj);
            return !servicesApiFromMethod.IsSuccess ? OperateResult.CreateFailedResult<string>((OperateResult)servicesApiFromMethod).ToJsonString() : HttpServer.HandleObjectMethod(request, deceodeUrl, json, servicesApiFromMethod.Content);
        }

        /// <summary>根据完整的地址获取当前的url地址信息</summary>
        /// <param name="url">地址信息</param>
        /// <returns>方法名称</returns>
        public static string GetMethodName(string url)
        {
            string empty = string.Empty;
            string str = url.IndexOf('?') <= 0 ? url : url.Substring(0, url.IndexOf('?'));
            if (str.EndsWith("/") || str.StartsWith("/"))
                str = str.Trim('/');
            return str;
        }

        /// <summary>
        /// 使用指定的对象来返回网络的API接口，前提是传入的数据为json参数，返回的数据为json数据，详细参照说明<br />
        /// Use the specified object to return the API interface of the network,
        /// provided that the incoming data is json parameters and the returned data is json data,
        /// please refer to the description for details
        /// </summary>
        /// <param name="request">当前的请求信息</param>
        /// <param name="deceodeUrl">已经解码过的Url地址信息</param>
        /// <param name="json">json格式的参数信息</param>
        /// <param name="apiInformation">等待解析的api解析的对象</param>
        /// <returns>等待返回客户的结果</returns>
        public static string HandleObjectMethod(
          HttpListenerRequest request,
          string deceodeUrl,
          string json,
          MqttRpcApiInfo apiInformation)
        {
            if (apiInformation.PermissionAttribute != null)
            {
                if (!ESTCore.Common.Authorization.asdniasnfaksndiqwhawfskhfaiw())
                    return new OperateResult<string>("Permission function need authorization ：" + StringResources.Language.InsufficientPrivileges).ToJsonString();
                try
                {
                    string[] values = request.Headers.GetValues("Authorization");
                    if (values == null || values.Length < 1 || string.IsNullOrEmpty(values[0]))
                        return new OperateResult<string>("Mqtt RPC Api ：[" + apiInformation.ApiTopic + "] has none Authorization information, access not permission").ToJsonString();
                    string[] strArray = Encoding.UTF8.GetString(Convert.FromBase64String(values[0].Split(new char[1]
                    {
            ' '
                    }, StringSplitOptions.RemoveEmptyEntries)[1])).Split(new char[1]
                    {
            ':'
                    }, StringSplitOptions.RemoveEmptyEntries);
                    if (strArray.Length < 1)
                        return new OperateResult<string>("Mqtt RPC Api ：[" + apiInformation.ApiTopic + "] has none Username information, access not permission").ToJsonString();
                    if (!apiInformation.PermissionAttribute.CheckUserName(strArray[0]))
                        return new OperateResult<string>("Mqtt RPC Api ：[" + apiInformation.ApiTopic + "] Check Username[" + strArray[0] + "] failed, access not permission").ToJsonString();
                }
                catch (Exception ex)
                {
                    return new OperateResult<string>("Mqtt RPC Api ：[" + apiInformation.ApiTopic + "] Check Username failed, access not permission, reason:" + ex.Message).ToJsonString();
                }
            }
            try
            {
                if (apiInformation.Method != (MethodInfo)null)
                {
                    MethodInfo method = apiInformation.Method;
                    string apiTopic = apiInformation.ApiTopic;
                    if (request.HttpMethod != apiInformation.HttpMethod)
                        return new OperateResult("Current Api ：" + apiTopic + " not support diffrent httpMethod").ToJsonString();
                    object[] parameters;
                    if (request.HttpMethod == "POST")
                    {
                        parameters = EstReflectionHelper.GetParametersFromJson(method.GetParameters(), json);
                    }
                    else
                    {
                        if (!(request.HttpMethod == "GET"))
                            return new OperateResult("Current Api ：" + apiTopic + " not support GET or POST").ToJsonString();
                        parameters = deceodeUrl.IndexOf('?') <= 0 ? EstReflectionHelper.GetParametersFromJson(method.GetParameters(), json) : EstReflectionHelper.GetParametersFromUrl(method.GetParameters(), deceodeUrl);
                    }
                    return method.Invoke(apiInformation.SourceObject, parameters).ToJsonString();
                }
                if (!(apiInformation.Property != (PropertyInfo)null))
                    return new OperateResult("Current Api ：" + deceodeUrl + " not supported").ToJsonString();
                string apiTopic1 = apiInformation.ApiTopic;
                if (request.HttpMethod != apiInformation.HttpMethod)
                    return new OperateResult("Current Api ：" + apiTopic1 + " not support diffrent httpMethod").ToJsonString();
                return request.HttpMethod != "GET" ? new OperateResult("Current Api ：" + apiTopic1 + " not support POST").ToJsonString() : apiInformation.Property.GetValue(apiInformation.SourceObject, (object[])null).ToJsonString();
            }
            catch (Exception ex)
            {
                return new OperateResult("Current Api ：" + deceodeUrl + " Wrong，Reason：" + ex.Message).ToJsonString();
            }
        }
    }
}
