// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.MQTT.MqttRpcApiInfo
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Reflection;

using Newtonsoft.Json;

using System.Reflection;
using System.Threading;

namespace ESTCore.Common.MQTT
{
    /// <summary>
    /// Mqtt的同步网络服务的单Api信息描述类<br />
    /// Single Api information description class of Mqtt's synchronous network service
    /// </summary>
    public class MqttRpcApiInfo
    {
        private long calledCount = 0;
        private long spendTotalTime = 0;
        private MethodInfo method;
        private PropertyInfo property;

        /// <summary>当前的Api的路由信息，对于注册服务来说，是类名/方法名</summary>
        public string ApiTopic { get; set; }

        /// <summary>当前的Api的路由说明</summary>
        public string Description { get; set; }

        /// <summary>当前Api的调用次数</summary>
        public long CalledCount
        {
            get => this.calledCount;
            set => this.calledCount = value;
        }

        /// <summary>示例的</summary>
        public string ExamplePayload { get; set; }

        /// <summary>当前Api的调用总耗时，单位是秒</summary>
        public double SpendTotalTime
        {
            get => (double)this.spendTotalTime / 100000.0;
            set => this.spendTotalTime = (long)(value * 100000.0);
        }

        /// <summary>当前Api是否为方法，如果是方法，就为true，否则为false</summary>
        public bool IsMethodApi { get; set; }

        /// <summary>如果当前的API接口是支持Http的请求方式，当前属性有效，例如GET,POST</summary>
        public string HttpMethod { get; set; } = "GET";

        /// <summary>当前的Api的方法是否是异步的Task类型</summary>
        [JsonIgnore]
        public bool IsOperateResultApi { get; set; }

        /// <summary>当前的Api关联的方法反射，本属性在JSON中将会忽略</summary>
        [JsonIgnore]
        public MethodInfo Method
        {
            get => this.method;
            set
            {
                this.method = value;
                this.IsMethodApi = true;
            }
        }

        /// <summary>当前的Api关联的方法反射，本属性在JSON中将会忽略</summary>
        [JsonIgnore]
        public PropertyInfo Property
        {
            get => this.property;
            set
            {
                this.property = value;
                this.IsMethodApi = false;
            }
        }

        /// <summary>当前Api的方法的权限访问反射，本属性在JSON中将会忽略</summary>
        [JsonIgnore]
        public EstMqttPermissionAttribute PermissionAttribute { get; set; }

        /// <summary>当前Api绑定的对象的，实际的接口请求，将会从对象进行调用，本属性在JSON中将会忽略</summary>
        [JsonIgnore]
        public object SourceObject { get; set; }

        /// <summary>使用原子的操作增加一次调用次数的数据信息，需要传入当前的消耗的时间，单位为100倍毫秒</summary>
        /// <param name="timeSpend">当前调用花费的时间，单位为100倍毫秒</param>
        public void CalledCountAddOne(long timeSpend)
        {
            Interlocked.Increment(ref this.calledCount);
            Interlocked.Add(ref this.spendTotalTime, timeSpend);
        }

        /// <inheritdoc />
        public override string ToString() => this.ApiTopic;
    }
}
