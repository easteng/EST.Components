// Decompiled with JetBrains decompiler
// Type: EstCommunication.DTU.DTUServer
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.Core.Net;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EstCommunication.DTU
{
  /// <summary>
  /// DTU的服务器信息，本服务器支持任意的hsl支持的网络对象，包括plc信息，modbus设备等等，通过DTU来连接，
  /// 然后支持多个连接对象。如果需要支持非hsl的注册报文，需要重写相关的方法<br />
  /// DTU server information, the server supports any network objects supported by hsl,
  /// including plc information, modbus devices, etc., connected through DTU, and then supports multiple connection objects.
  /// If you need to support non-HSL registration messages, you need to rewrite the relevant methods
  /// </summary>
  /// <remarks>针对异形客户端进行扩展信息</remarks>
  public class DTUServer : NetworkAlienClient
  {
    private Dictionary<string, NetworkDeviceBase> devices;

    /// <summary>
    /// 根据配置的列表信息来实例化相关的DTU服务器<br />
    /// Instantiate the relevant DTU server according to the configured list information
    /// </summary>
    /// <param name="dTUSettings">DTU的配置信息</param>
    public DTUServer(List<DTUSettingType> dTUSettings)
    {
      this.devices = new Dictionary<string, NetworkDeviceBase>();
      this.SetTrustClients(dTUSettings.Select<DTUSettingType, string>((Func<DTUSettingType, string>) (m => m.DtuId)).ToArray<string>());
      for (int index = 0; index < dTUSettings.Count; ++index)
      {
        this.devices.Add(dTUSettings[index].DtuId, dTUSettings[index].GetClient());
        this.devices[dTUSettings[index].DtuId].ConnectServer(new AlienSession()
        {
          DTU = dTUSettings[index].DtuId,
          IsStatusOk = false
        });
      }
      this.OnClientConnected += new NetworkAlienClient.OnClientConnectedDelegate(this.DTUServer_OnClientConnected);
    }

    /// <summary>
    /// 根据配置的列表信息来实例化相关的DTU服务器<br />
    /// Instantiate the relevant DTU server according to the configured list information
    /// </summary>
    /// <param name="dtuId">Dtu信息</param>
    /// <param name="networkDevices">设备信息</param>
    public DTUServer(string[] dtuId, NetworkDeviceBase[] networkDevices)
    {
      this.devices = new Dictionary<string, NetworkDeviceBase>();
      this.SetTrustClients(dtuId);
      for (int index = 0; index < dtuId.Length; ++index)
      {
        this.devices.Add(dtuId[index], networkDevices[index]);
        this.devices[dtuId[index]].ConnectServer(new AlienSession()
        {
          DTU = dtuId[index],
          IsStatusOk = false
        });
      }
    }

    /// <inheritdoc />
    protected override void CloseAction()
    {
      foreach (KeyValuePair<string, NetworkDeviceBase> device in this.devices)
      {
        AlienSession alienSession = device.Value.AlienSession;
        if (alienSession != null)
        {
          alienSession.IsStatusOk = false;
          alienSession.Socket?.Close();
        }
      }
    }

    /// <inheritdoc />
    public override int IsClientOnline(AlienSession session) => this.devices[session.DTU].AlienSession == null || !this.devices[session.DTU].AlienSession.IsStatusOk ? 0 : 1;

    private void DTUServer_OnClientConnected(AlienSession session) => this.devices[session.DTU].ConnectServer(session);

    /// <summary>
    /// 根据DTU信息获取设备的连接对象<br />
    /// Obtain the connection object of the device according to the DTU information
    /// </summary>
    /// <param name="dtuId">设备的id信息</param>
    /// <returns>设备的对象</returns>
    public NetworkDeviceBase this[string dtuId] => !this.devices.ContainsKey(dtuId) ? (NetworkDeviceBase) null : this.devices[dtuId];

    /// <summary>
    /// 获取所有的会话信息，是否在线，上线的基本信息<br />
    /// Get all the session information, whether it is online, online basic information
    /// </summary>
    /// <returns>会话列表</returns>
    public AlienSession[] GetAlienSessions() => this.devices.Values.Select<NetworkDeviceBase, AlienSession>((Func<NetworkDeviceBase, AlienSession>) (m => m.AlienSession)).ToArray<AlienSession>();

    /// <summary>
    /// 获取所有的设备的信息，可以用来读写设备的数据信息<br />
    /// Get all device information, can be used to read and write device data information
    /// </summary>
    /// <returns>设备数组</returns>
    public NetworkDeviceBase[] GetDevices() => this.devices.Values.ToArray<NetworkDeviceBase>();

    /// <inheritdoc />
    public override string ToString() => string.Format("Dtu[{0}]", (object) this.Port);
  }
}
