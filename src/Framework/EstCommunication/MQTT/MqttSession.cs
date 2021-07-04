// Decompiled with JetBrains decompiler
// Type: EstCommunication.MQTT.MqttSession
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EstCommunication.MQTT
{
  /// <summary>
  /// Mqtt的会话信息，包含了一些基本的信息内容，客户端的IP地址及端口，Client ID，用户名，活动时间，是否允许发布数据等等<br />
  /// Mqtt's session information includes some basic information content, the client's IP address and port, Client ID, user name, activity time, whether it is allowed to publish data, etc.
  /// </summary>
  public class MqttSession
  {
    private object objLock = new object();

    /// <summary>
    /// 实例化一个对象，指定ip地址及端口，以及协议内容<br />
    /// Instantiate an object, specify ip address and port, and protocol content
    /// </summary>
    /// <param name="endPoint">远程客户端的IP地址</param>
    /// <param name="protocol">协议信息</param>
    public MqttSession(IPEndPoint endPoint, string protocol)
    {
      this.Topics = new List<string>();
      this.ActiveTime = DateTime.Now;
      this.OnlineTime = DateTime.Now;
      this.ActiveTimeSpan = TimeSpan.FromSeconds(1000000.0);
      this.EndPoint = endPoint;
      this.Protocol = protocol;
    }

    /// <summary>
    /// 远程的ip地址端口信息<br />
    /// Remote ip address port information
    /// </summary>
    public IPEndPoint EndPoint { get; set; }

    /// <summary>
    /// 当前接收的客户端ID信息<br />
    /// Client ID information currently received
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// 当前客户端的激活时间<br />
    /// The activation time of the current client
    /// </summary>
    public DateTime ActiveTime { get; set; }

    /// <summary>
    /// 获取当前的客户端的上线时间<br />
    /// Get the online time of the current client
    /// </summary>
    public DateTime OnlineTime { get; private set; }

    /// <summary>
    /// 两次活动的最小时间间隔<br />
    /// Minimum time interval between two activities
    /// </summary>
    public TimeSpan ActiveTimeSpan { get; set; }

    /// <summary>当前客户端绑定的套接字对象</summary>
    internal Socket MqttSocket { get; set; }

    /// <summary>
    /// 当前客户端订阅的所有的Topic信息<br />
    /// All Topic information subscribed by the current client
    /// </summary>
    private List<string> Topics { get; set; }

    /// <summary>
    /// 当前的用户名<br />
    /// Current username
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// 当前的协议信息，一般为 "MQTT"，如果是同步客户端那么是 "HUSL"，如果是文件客户端就是 "FILE"<br />
    /// The current protocol information, generally "MQTT", if it is a synchronous client then it is "HUSL", if it is a file client it is "FILE"
    /// </summary>
    public string Protocol { get; private set; }

    /// <summary>
    /// 获取或设置当前的MQTT客户端是否允许发布消息，默认为False，如果设置为True，就是禁止发布消息，服务器不会触发收到消息的事件。<br />
    /// Gets or sets whether the current MQTT client is allowed to publish messages, the default is False,
    /// if set to True, it is forbidden to publish messages, The server does not trigger the event of receiving a message.
    /// </summary>
    public bool ForbidPublishTopic { get; set; }

    /// <summary>
    /// 检查当前的会话对象里是否订阅了指定的主题内容<br />
    /// Check whether the specified topic content is subscribed in the current session object
    /// </summary>
    /// <param name="topic">主题信息</param>
    /// <returns>如果订阅了，返回 True, 否则，返回 False</returns>
    public bool IsClientSubscribe(string topic)
    {
      bool flag = false;
      lock (this.objLock)
        flag = this.Topics.Contains(topic);
      return flag;
    }

    /// <summary>
    /// 获取当前客户端订阅的所有的Topic信息<br />
    /// Get all Topic information subscribed by the current client
    /// </summary>
    /// <returns>主题列表</returns>
    public string[] GetTopics()
    {
      string[] array;
      lock (this.objLock)
        array = this.Topics.ToArray();
      return array;
    }

    /// <summary>
    /// 当前的会话信息新增一个订阅的主题信息<br />
    /// The current session information adds a subscribed topic information
    /// </summary>
    /// <param name="topic">主题的信息</param>
    public void AddSubscribe(string topic)
    {
      lock (this.objLock)
      {
        if (this.Topics.Contains(topic))
          return;
        this.Topics.Add(topic);
      }
    }

    /// <summary>
    /// 当前的会话信息新增多个订阅的主题信息<br />
    /// The current session information adds multiple subscribed topic information
    /// </summary>
    /// <param name="topics">主题的信息</param>
    public void AddSubscribe(string[] topics)
    {
      if (topics == null)
        return;
      lock (this.objLock)
      {
        for (int index = 0; index < topics.Length; ++index)
        {
          if (!this.Topics.Contains(topics[index]))
            this.Topics.Add(topics[index]);
        }
      }
    }

    /// <summary>移除会话信息的一个订阅的主题</summary>
    /// <param name="topic">主题</param>
    public void RemoveSubscribe(string topic)
    {
      lock (this.objLock)
      {
        if (!this.Topics.Contains(topic))
          return;
        this.Topics.Remove(topic);
      }
    }

    /// <summary>
    /// 移除会话信息的一个订阅的主题<br />
    /// Remove a subscribed topic from session information
    /// </summary>
    /// <param name="topics">主题</param>
    public void RemoveSubscribe(string[] topics)
    {
      if (topics == null)
        return;
      lock (this.objLock)
      {
        for (int index = 0; index < topics.Length; ++index)
        {
          if (this.Topics.Contains(topics[index]))
            this.Topics.Remove(topics[index]);
        }
      }
    }

    /// <summary>
    /// 获取当前的会话信息，包含在线时间的信息<br />
    /// Get current session information, including online time information
    /// </summary>
    /// <returns>会话信息，包含在线时间</returns>
    public string GetSessionOnlineInfo()
    {
      StringBuilder stringBuilder = new StringBuilder(this.ToString());
      stringBuilder.Append(" [" + SoftBasic.GetTimeSpanDescription(DateTime.Now - this.OnlineTime) + "]");
      return stringBuilder.ToString();
    }

    /// <inheritdoc />
    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder(string.Format("{0} Session[IP:{1}]", (object) this.Protocol, (object) this.EndPoint));
      if (!string.IsNullOrEmpty(this.ClientId))
        stringBuilder.Append(" [ID:" + this.ClientId + "]");
      if (!string.IsNullOrEmpty(this.UserName))
        stringBuilder.Append(" [Name:" + this.UserName + "]");
      return stringBuilder.ToString();
    }
  }
}
