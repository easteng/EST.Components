// Decompiled with JetBrains decompiler
// Type: EstCommunication.Reflection.EstMqttApiAttribute
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using System;

namespace EstCommunication.Reflection
{
  /// <summary>
  /// 可以指定方法变成对外公开的API接口，如果方法不实现该特性，将不对外公开方法，无法获取相关的接口权限<br />
  /// You can specify the method to become an externally public API interface. If the method does not implement this feature,
  /// the method will not be publicly disclosed, and the related interface permissions cannot be obtained
  /// </summary>
  [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false)]
  public class EstMqttApiAttribute : Attribute
  {
    /// <summary>
    /// 当前指定的ApiTopic信息，如果当前的方法接口不指定别名，那么就使用当前的方法名称<br />
    /// The currently specified ApiTopic information, if the current method interface does not specify an alias,
    /// then the current method name is used
    /// </summary>
    public string ApiTopic { get; set; }

    /// <summary>
    /// 当前方法的注释内容<br />
    /// The comment content of the current method
    /// </summary>
    public string Description { get; set; }

    /// <summary>当前的属性是否需要展开API信息，默认不展开</summary>
    public bool PropertyUnfold { get; set; } = false;

    /// <summary>如果当前的API接口是支持Http的请求方式，当前属性有效，例如GET,POST</summary>
    public string HttpMethod { get; set; } = "POST";

    /// <summary>指定描述内容来实例化一个的对象</summary>
    /// <param name="description">当前接口的描述信息</param>
    public EstMqttApiAttribute(string description) => this.Description = description;

    /// <summary>指定描述内容来实例化一个的对象</summary>
    /// <param name="apiTopic">指重新定当前接口的路由信息</param>
    /// <param name="description">当前接口的描述信息</param>
    public EstMqttApiAttribute(string apiTopic, string description)
    {
      this.ApiTopic = apiTopic;
      this.Description = description;
    }

    /// <summary>实例化一个默认的对象</summary>
    public EstMqttApiAttribute()
    {
    }
  }
}
