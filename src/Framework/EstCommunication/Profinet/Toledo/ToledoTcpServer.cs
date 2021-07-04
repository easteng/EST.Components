// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.Toledo.ToledoTcpServer
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.Core.Net;
using System;
using System.Net;
using System.Net.Sockets;

namespace EstCommunication.Profinet.Toledo
{
  /// <summary>托利多电子秤的TCP服务器，启动服务器后，等待电子秤的数据连接。</summary>
  public class ToledoTcpServer : NetworkServerBase
  {
    /// <summary>获取或设置当前的报文否是含有校验的，默认为含有校验</summary>
    public bool HasChk { get; set; } = true;

    /// <inheritdoc />
    protected override void ThreadPoolLogin(Socket socket, IPEndPoint endPoint)
    {
      AppSession appSession = new AppSession();
      appSession.WorkSocket = socket;
      this.LogNet?.WriteDebug(this.ToString(), string.Format(StringResources.Language.ClientOnlineInfo, (object) appSession.IpEndPoint));
      if (appSession.WorkSocket.BeginReceiveResult(new AsyncCallback(this.ReceiveCallBack), (object) appSession).IsSuccess)
        return;
      this.LogNet?.WriteError(this.ToString(), StringResources.Language.NetClientLoginFailed);
    }

    private async void ReceiveCallBack(IAsyncResult ar)
    {
      if (!(ar.AsyncState is AppSession appSession))
        appSession = (AppSession) null;
      else if (!appSession.WorkSocket.EndReceiveResult(ar).IsSuccess)
      {
        appSession = (AppSession) null;
      }
      else
      {
        OperateResult<byte[]> read = await this.ReceiveAsync(appSession.WorkSocket, this.HasChk ? 18 : 17);
        if (!read.IsSuccess)
        {
          this.LogNet?.WriteDebug(this.ToString(), string.Format(StringResources.Language.ClientOfflineInfo, (object) appSession.IpEndPoint));
          Socket workSocket = appSession.WorkSocket;
          if (workSocket == null)
          {
            appSession = (AppSession) null;
          }
          else
          {
            workSocket.Close();
            appSession = (AppSession) null;
          }
        }
        else
        {
          ToledoTcpServer.ToledoStandardDataReceivedDelegate standardDataReceived = this.OnToledoStandardDataReceived;
          if (standardDataReceived != null)
            standardDataReceived((object) this, new ToledoStandardData(read.Content));
          if (!appSession.WorkSocket.BeginReceiveResult(new AsyncCallback(this.ReceiveCallBack), (object) appSession).IsSuccess)
            this.LogNet?.WriteDebug(this.ToString(), string.Format(StringResources.Language.ClientOfflineInfo, (object) appSession.IpEndPoint));
          read = (OperateResult<byte[]>) null;
          appSession = (AppSession) null;
        }
      }
    }

    /// <summary>当接收到一条新的托利多的数据的时候触发</summary>
    public event ToledoTcpServer.ToledoStandardDataReceivedDelegate OnToledoStandardDataReceived;

    /// <inheritdoc />
    public override string ToString() => string.Format("ToledoTcpServer[{0}]", (object) this.Port);

    /// <summary>托利多数据接收时的委托</summary>
    /// <param name="sender">数据发送对象</param>
    /// <param name="toledoStandardData">数据对象</param>
    public delegate void ToledoStandardDataReceivedDelegate(
      object sender,
      ToledoStandardData toledoStandardData);
  }
}
