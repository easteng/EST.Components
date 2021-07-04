// Decompiled with JetBrains decompiler
// Type: EstCommunication.MQTT.MqttClientApplicationMessage
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using System;

namespace EstCommunication.MQTT
{
  /// <summary>
  /// 来自客户端的一次消息的内容，当前类主要是在MQTT的服务端进行使用<br />
  /// The content of a message from the client. The current class is mainly used on the MQTT server
  /// </summary>
  public class MqttClientApplicationMessage : MqttApplicationMessage
  {
    /// <summary>
    /// 实例化一个默认的对象<br />
    /// Instantiate a default object
    /// </summary>
    public MqttClientApplicationMessage() => this.CreateTime = DateTime.Now;

    /// <summary>
    /// 客户端的Id信息<br />
    /// Client Id information
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// 当前的客户端的用户名<br />
    /// Username of the current client
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// 获取或设置当前的消息是否取消发布，默认False，也就是发布出去<br />
    /// Get or set whether the current message is unpublished, the default is False, which means it is published
    /// </summary>
    public bool IsCancelPublish { get; set; } = false;

    /// <summary>
    /// 当前消息的生成时间<br />
    /// The generation time of the current message
    /// </summary>
    public DateTime CreateTime { get; set; }
  }
}
