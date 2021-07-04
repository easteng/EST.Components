// Decompiled with JetBrains decompiler
// Type: EstCommunication.MQTT.MqttCredential
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

namespace EstCommunication.MQTT
{
  /// <summary>
  /// Mqtt协议的验证对象，包含用户名和密码<br />
  /// Authentication object of Mqtt protocol, including username and password
  /// </summary>
  public class MqttCredential
  {
    /// <summary>
    /// 实例化一个默认的对象<br />
    /// Instantiate a default object
    /// </summary>
    public MqttCredential()
    {
    }

    /// <summary>
    /// 实例化指定的用户名和密码的对象<br />
    /// Instantiates an object with the specified username and password
    /// </summary>
    /// <param name="name">用户名</param>
    /// <param name="pwd">密码</param>
    public MqttCredential(string name, string pwd)
    {
      this.UserName = name;
      this.Password = pwd;
    }

    /// <summary>
    /// 获取或设置用户名<br />
    /// Get or set username
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// 获取或设置密码<br />
    /// Get or set password
    /// </summary>
    public string Password { get; set; }

    /// <inheritdoc />
    public override string ToString() => this.UserName;
  }
}
