// Decompiled with JetBrains decompiler
// Type: EstCommunication.Reflection.EstMqttPermissionAttribute
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using System;

namespace EstCommunication.Reflection
{
  /// <summary>
  /// 可以指定方法的权限内容，可以限定MQTT会话的ClientID信息或是UserName内容<br />
  /// 
  /// </summary>
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
  public class EstMqttPermissionAttribute : Attribute
  {
    /// <summary>ClientId的限定内容</summary>
    public string ClientID { get; set; }

    /// <summary>UserName的限定内容</summary>
    public string UserName { get; set; }

    /// <summary>检查当前的客户端ID是否通过</summary>
    /// <param name="clientID">ID信息</param>
    /// <returns>是否检测成功</returns>
    public virtual bool CheckClientID(string clientID) => string.IsNullOrEmpty(this.ClientID) || this.ClientID == clientID;

    /// <summary>检查当前的用户名是否通过</summary>
    /// <param name="name">用户名</param>
    /// <returns>是否检测成功</returns>
    public virtual bool CheckUserName(string name) => string.IsNullOrEmpty(this.UserName) || this.UserName == name;
  }
}
