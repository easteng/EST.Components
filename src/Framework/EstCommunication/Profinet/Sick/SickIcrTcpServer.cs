// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.Sick.SickIcrTcpServer
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.Core.Net;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace EstCommunication.Profinet.Sick
{
  /// <summary>
  /// Sick的扫码器的服务器信息，只要启动服务器之后，扫码器配置将条码发送到PC的指定端口上来即可，就可以持续的接收条码信息，同样也适用于海康，基恩士，DATELOGIC 。<br />
  /// The server information of Sick's code scanner, as long as the server is started, the code scanner is configured to send the barcode to the designated port of the PC, and it can continuously receive the barcode information.
  /// </summary>
  public class SickIcrTcpServer : NetworkServerBase
  {
    private int clientCount = 0;
    private List<AppSession> initiativeClients;
    private object lockClients;

    /// <summary>
    /// 实例化一个默认的服务器对象<br />
    /// Instantiate a default server object
    /// </summary>
    public SickIcrTcpServer()
    {
      this.initiativeClients = new List<AppSession>();
      this.lockClients = new object();
    }

    /// <summary>
    /// 当接收到条码数据的时候触发<br />
    /// Triggered when barcode data is received
    /// </summary>
    public event SickIcrTcpServer.ReceivedBarCodeDelegate OnReceivedBarCode;

    /// <inheritdoc />
    protected override void ThreadPoolLogin(Socket socket, IPEndPoint endPoint)
    {
      AppSession session = new AppSession();
      session.IpEndPoint = endPoint;
      session.IpAddress = endPoint.Address.ToString();
      session.WorkSocket = socket;
      try
      {
        socket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, new AsyncCallback(this.SocketAsyncCallBack), (object) session);
        this.AddClient(session);
      }
      catch
      {
        socket.Close();
        this.LogNet?.WriteDebug(this.ToString(), string.Format(StringResources.Language.ClientOfflineInfo, (object) endPoint));
      }
    }

    private void SocketAsyncCallBack(IAsyncResult ar)
    {
      if (!(ar.AsyncState is AppSession asyncState))
        return;
      try
      {
        asyncState.WorkSocket.EndReceive(ar);
        byte[] buffer = new byte[1024];
        int length = asyncState.WorkSocket.Receive(buffer);
        if (length > 0)
        {
          byte[] bytes = new byte[length];
          Array.Copy((Array) buffer, 0, (Array) bytes, 0, length);
          asyncState.WorkSocket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, new AsyncCallback(this.SocketAsyncCallBack), (object) asyncState);
          if (EstCommunication.Authorization.nzugaydgwadawdibbas())
          {
            SickIcrTcpServer.ReceivedBarCodeDelegate onReceivedBarCode = this.OnReceivedBarCode;
            if (onReceivedBarCode != null)
              onReceivedBarCode(asyncState.IpAddress, this.TranslateCode(Encoding.ASCII.GetString(bytes)));
          }
        }
        else
        {
          asyncState.WorkSocket?.Close();
          this.LogNet?.WriteDebug(this.ToString(), string.Format(StringResources.Language.ClientOfflineInfo, (object) asyncState.IpEndPoint));
          this.RemoveClient(asyncState);
        }
      }
      catch
      {
        asyncState.WorkSocket?.Close();
        this.LogNet?.WriteDebug(this.ToString(), string.Format(StringResources.Language.ClientOfflineInfo, (object) asyncState.IpEndPoint));
        this.RemoveClient(asyncState);
      }
    }

    private string TranslateCode(string code)
    {
      StringBuilder stringBuilder = new StringBuilder("");
      for (int index = 0; index < code.Length; ++index)
      {
        if (char.IsLetterOrDigit(code, index))
          stringBuilder.Append(code[index]);
      }
      return stringBuilder.ToString();
    }

    /// <summary>
    /// 新增一个主动连接的请求，将不会收到是否连接成功的信息，当网络中断及奔溃之后，会自动重新连接。<br />
    /// A new active connection request will not receive a message whether the connection is successful. When the network is interrupted and crashed, it will automatically reconnect.
    /// </summary>
    /// <param name="ipAddress">对方的Ip地址</param>
    /// <param name="port">端口号</param>
    public void AddConnectBarcodeScan(string ipAddress, int port)
    {
      IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
      ThreadPool.QueueUserWorkItem(new WaitCallback(this.ConnectBarcodeScan), (object) new AppSession()
      {
        IpEndPoint = ipEndPoint,
        IpAddress = ipEndPoint.Address.ToString()
      });
    }

    private void ConnectBarcodeScan(object obj)
    {
      if (!(obj is AppSession session))
        return;
      OperateResult<Socket> socketAndConnect = this.CreateSocketAndConnect(session.IpEndPoint, 5000);
      if (!socketAndConnect.IsSuccess)
      {
        Thread.Sleep(1000);
        ThreadPool.QueueUserWorkItem(new WaitCallback(this.ConnectBarcodeScan), (object) session);
      }
      else
      {
        session.WorkSocket = socketAndConnect.Content;
        try
        {
          session.WorkSocket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, new AsyncCallback(this.InitiativeSocketAsyncCallBack), (object) session);
          this.AddClient(session);
        }
        catch
        {
          session.WorkSocket.Close();
          ThreadPool.QueueUserWorkItem(new WaitCallback(this.ConnectBarcodeScan), (object) session);
        }
      }
    }

    private void InitiativeSocketAsyncCallBack(IAsyncResult ar)
    {
      if (!(ar.AsyncState is AppSession asyncState))
        return;
      try
      {
        asyncState.WorkSocket.EndReceive(ar);
        byte[] buffer = new byte[1024];
        int length = asyncState.WorkSocket.Receive(buffer);
        if (length > 0)
        {
          byte[] bytes = new byte[length];
          Array.Copy((Array) buffer, 0, (Array) bytes, 0, length);
          asyncState.WorkSocket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, new AsyncCallback(this.InitiativeSocketAsyncCallBack), (object) asyncState);
          if (EstCommunication.Authorization.nzugaydgwadawdibbas())
          {
            SickIcrTcpServer.ReceivedBarCodeDelegate onReceivedBarCode = this.OnReceivedBarCode;
            if (onReceivedBarCode != null)
              onReceivedBarCode(asyncState.IpAddress, this.TranslateCode(Encoding.ASCII.GetString(bytes)));
          }
        }
        else
        {
          asyncState.WorkSocket?.Close();
          this.LogNet?.WriteDebug(this.ToString(), string.Format(StringResources.Language.ClientOfflineInfo, (object) asyncState.IpEndPoint));
          this.RemoveClient(asyncState);
        }
      }
      catch
      {
        asyncState.WorkSocket?.Close();
        this.LogNet?.WriteDebug(this.ToString(), string.Format(StringResources.Language.ClientOfflineInfo, (object) asyncState.IpEndPoint));
        this.RemoveClient(asyncState);
        if (this.IsStarted)
          this.ConnectBarcodeScan((object) asyncState);
      }
    }

    /// <summary>
    /// 获取当前在线的客户端数量<br />
    /// Get the number of clients currently online
    /// </summary>
    public int OnlineCount => this.clientCount;

    private void AddClient(AppSession session)
    {
      lock (this.lockClients)
      {
        ++this.clientCount;
        this.initiativeClients.Add(session);
      }
    }

    private void RemoveClient(AppSession session)
    {
      lock (this.lockClients)
      {
        --this.clientCount;
        this.initiativeClients.Remove(session);
      }
    }

    /// <inheritdoc />
    protected override void CloseAction()
    {
      lock (this.lockClients)
      {
        for (int index = 0; index < this.initiativeClients.Count; ++index)
          this.initiativeClients[index].WorkSocket?.Close();
        this.initiativeClients.Clear();
      }
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("SickIcrTcpServer[{0}]", (object) this.Port);

    /// <summary>
    /// 接收条码数据的委托信息<br />
    /// Entrusted information to receive barcode data
    /// </summary>
    /// <param name="ipAddress">Ip地址信息</param>
    /// <param name="barCode">条码信息</param>
    public delegate void ReceivedBarCodeDelegate(string ipAddress, string barCode);
  }
}
