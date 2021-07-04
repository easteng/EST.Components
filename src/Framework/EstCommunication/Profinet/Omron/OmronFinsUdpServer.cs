// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.Omron.OmronFinsUdpServer
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using EstCommunication.Core.Net;
using System;
using System.Net;
using System.Net.Sockets;

namespace EstCommunication.Profinet.Omron
{
  /// <inheritdoc cref="T:EstCommunication.Profinet.Omron.OmronFinsServer" />
  public class OmronFinsUdpServer : OmronFinsServer
  {
    /// <summary>
    /// 实例化一个默认的对象<br />
    /// Instantiate a default object
    /// </summary>
    public OmronFinsUdpServer() => this.headLength = 0;

    /// <summary>获取或设置一次接收时的数据长度，默认2KB数据长度</summary>
    public int ReceiveCacheLength { get; set; } = 2048;

    /// <inheritdoc />
    public override void ServerStart(int port)
    {
      if (this.IsStarted)
        return;
      this.StartInitialization();
      this.CoreSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
      this.CoreSocket.Bind((EndPoint) new IPEndPoint(IPAddress.Any, port));
      this.RefreshReceive();
      this.IsStarted = true;
      this.Port = port;
      this.LogNet?.WriteInfo(this.ToString(), StringResources.Language.NetEngineStart);
    }

    /// <inheritdoc />
    protected override byte[] PackCommand(int status, byte[] data)
    {
      if (data == null)
        data = new byte[0];
      byte[] numArray = new byte[14 + data.Length];
      SoftBasic.HexStringToBytes("00 00 00 00 00 00 00 00 00 00 00 00 00 00").CopyTo((Array) numArray, 0);
      if ((uint) data.Length > 0U)
        data.CopyTo((Array) numArray, 14);
      numArray[12] = BitConverter.GetBytes(status)[1];
      numArray[13] = BitConverter.GetBytes(status)[0];
      return numArray;
    }

    /// <summary>重新开始接收数据</summary>
    /// <exception cref="T:System.ArgumentNullException"></exception>
    private void RefreshReceive()
    {
      AppSession appSession = new AppSession();
      appSession.WorkSocket = this.CoreSocket;
      appSession.UdpEndPoint = (EndPoint) new IPEndPoint(IPAddress.Any, 0);
      appSession.BytesContent = new byte[this.ReceiveCacheLength];
      this.CoreSocket.BeginReceiveFrom(appSession.BytesContent, 0, this.ReceiveCacheLength, SocketFlags.None, ref appSession.UdpEndPoint, new System.AsyncCallback(this.AsyncCallback), (object) appSession);
    }

    private void AsyncCallback(IAsyncResult ar)
    {
      if (!(ar.AsyncState is AppSession asyncState))
        return;
      try
      {
        int from = asyncState.WorkSocket.EndReceiveFrom(ar, ref asyncState.UdpEndPoint);
        if (!EstCommunication.Authorization.asdniasnfaksndiqwhawfskhfaiw())
        {
          this.RemoveClient(asyncState);
        }
        else
        {
          this.RefreshReceive();
          byte[] numArray1 = new byte[from];
          Array.Copy((Array) asyncState.BytesContent, 0, (Array) numArray1, 0, from);
          this.LogNet?.WriteDebug(this.ToString(), "Udp " + StringResources.Language.Receive + "：" + numArray1.ToHexString(' '));
          byte[] numArray2 = this.ReadFromFinsCore(numArray1);
          if (numArray2 != null)
          {
            asyncState.WorkSocket.SendTo(numArray2, numArray2.Length, SocketFlags.None, asyncState.UdpEndPoint);
            this.LogNet?.WriteDebug(this.ToString(), "Udp " + StringResources.Language.Send + "：" + numArray2.ToHexString(' '));
            this.RaiseDataReceived((object) asyncState, numArray1);
          }
          else
            this.RemoveClient(asyncState);
        }
      }
      catch (ObjectDisposedException ex)
      {
      }
      catch (Exception ex)
      {
        this.LogNet?.WriteException(this.ToString(), StringResources.Language.SocketEndReceiveException, ex);
        this.RefreshReceive();
      }
      finally
      {
      }
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("OmronFinsUdpServer[{0}]", (object) this.Port);
  }
}
