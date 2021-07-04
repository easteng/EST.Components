// Decompiled with JetBrains decompiler
// Type: EstCommunication.MQTT.MqttServer
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using EstCommunication.Core;
using EstCommunication.Core.Net;
using EstCommunication.LogNet;
using EstCommunication.Reflection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EstCommunication.MQTT
{
  /// <summary>
  /// 一个Mqtt的服务器类对象，本服务器支持发布订阅操作，支持从服务器强制推送数据，支持往指定的客户端推送，支持基于一问一答的远程过程调用（RPC）的数据交互，支持文件上传下载。根据这些功能从而定制化出满足各个场景的服务器，详细的使用说明可以参见代码api文档示例。<br />
  /// An Mqtt server class object. This server supports publish and subscribe operations, supports forced push data from the server,
  /// supports push to designated clients, supports data interaction based on one-question-one-answer remote procedure calls (RPC),
  /// and supports file upload and download . According to these functions, the server can be customized to meet various scenarios.
  /// For detailed instructions, please refer to the code api document example.
  /// </summary>
  /// <remarks>
  /// 本MQTT服务器功能丰富，可以同时实现，用户名密码验证，在线客户端的管理，数据订阅推送，单纯的数据收发，心跳检测，同步数据访问，文件上传，下载，删除，遍历，详细参照下面的示例说明
  /// </remarks>
  /// <example>
  /// 最简单的使用，就是实例化，启动服务即可
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\MQTT\MqttServerSample.cs" region="Sample1" title="简单的实例化" />
  /// 当然了，我们可以稍微的复杂一点，加一个功能，验证连接的客户端操作
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\MQTT\MqttServerSample.cs" region="Sample2" title="增加验证" />
  /// 我们可以对ClientID，用户名，密码进行验证，那么我们可以动态修改client id么？比如用户名密码验证成功后，client ID我想设置为权限等级。
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\MQTT\MqttServerSample.cs" region="Sample2_1" title="动态修改Client ID" />
  /// 如果我想强制该客户端不能主动发布主题，可以这么操作。
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\MQTT\MqttServerSample.cs" region="Sample2_2" title="禁止发布主题" />
  /// 你也可以对clientid进行过滤验证，只要结果返回不是0，就可以了。接下来我们实现一个功能，所有客户端的发布的消息在控制台打印出来,
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\MQTT\MqttServerSample.cs" region="Sample3" title="打印所有发布" />
  /// 捕获客户端刚刚上线的时候，方便我们进行一些额外的操作信息。下面的意思就是返回一个数据，将数据发送到指定的会话内容上去
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\MQTT\MqttServerSample.cs" region="Sample4" title="客户端上线信息" />
  /// 下面演示如何从服务器端发布数据信息，包括多种发布的方法，消息是否驻留，详细看说明即可
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\MQTT\MqttServerSample.cs" region="Sample5" title="服务器发布" />
  /// 下面演示如何支持同步网络访问，当客户端是同步网络访问时，协议内容会变成HUSL，即被视为同步客户端，进行相关的操作，主要进行远程调用RPC，以及查询MQTT的主题列表。
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\MQTT\MqttServerSample.cs" region="Sample6" title="同步访问支持" />
  /// 如果需要查看在线信息，可以随时获取<see cref="P:EstCommunication.MQTT.MqttServer.OnlineCount" />属性，如果需要查看报文信息，可以实例化日志，参考日志的说明即可。<br /><br />
  /// 针对上面同步网络访问，虽然比较灵活，但是什么都要自己控制，无疑增加了代码的复杂度，举个例子，当你的topic分类很多的时候，已经客户端协议多个参数的时候，需要大量的手动解析的代码，
  /// 影响代码美观，而且让代码更加的杂乱，除此之外，还有个巨大的麻烦，服务器提供了很多的topic处理程序（可以换个称呼，暴露的API接口），
  /// 客户端没法清晰的浏览到，需要查找服务器代码才能知晓，而且服务器更新了接口，客户端有需要同步查看服务器的代码才行，以及做权限控制也很麻烦。<br />
  /// 所以在Est里面的MQTT服务器，提供了注册API接口的功能，只需要一行注册代码，你的类的方法自动就会变为API解析，所有的参数都是同步解析的，如果你返回的是
  /// OperateResult&lt;T&gt;类型对象，还支持是否成功的结果报告，否则一律视为json字符串，返回给调用方。
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\MQTT\MqttServerSample.cs" region="Sample7" title="基于MQTT的RPC接口实现" />
  /// 如果需要查看在线信息，可以随时获取<see cref="P:EstCommunication.MQTT.MqttServer.OnlineCount" />属性，如果需要查看报文信息，可以实例化日志，参考日志的说明即可。<br /><br />
  /// 最后介绍一下文件管理服务是如何启动的，在启动了文件管理服务之后，其匹配的客户端 <see cref="T:EstCommunication.MQTT.MqttSyncClient" /> 就可以上传下载，遍历文件了。
  /// 而服务器端做的就是启用服务，如果你需要一些更加自由的权限控制，比如某个账户只能下载，不能其他操作，都是可以实现的。更加多的示例参考DEMO程序。
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\MQTT\MqttServerSample.cs" region="Sample8" title="基于MQTT的文件管理服务启动" />
  /// </example>
  public class MqttServer : NetworkServerBase
  {
    private Dictionary<string, MqttRpcApiInfo> apiTopicServiceDict;
    private object rpcApiLock;
    private readonly Dictionary<string, FileMarkId> dictionaryFilesMarks;
    private readonly object dictHybirdLock;
    private string filesDirectoryPath = (string) null;
    private bool fileServerEnabled = false;
    private Dictionary<string, GroupFileContainer> m_dictionary_group_marks = new Dictionary<string, GroupFileContainer>();
    private SimpleHybirdLock group_marks_lock = new SimpleHybirdLock();
    private MqttFileMonitor fileMonitor = new MqttFileMonitor();
    private readonly Dictionary<string, MqttClientApplicationMessage> retainKeys;
    private readonly object keysLock;
    private readonly List<MqttSession> mqttSessions = new List<MqttSession>();
    private readonly object sessionsLock = new object();
    private System.Threading.Timer timerHeart;
    private LogStatisticsDict statisticsDict;

    /// <summary>
    /// 实例化一个MQTT协议的服务器<br />
    /// Instantiate a MQTT protocol server
    /// </summary>
    public MqttServer()
    {
      this.statisticsDict = new LogStatisticsDict(GenerateMode.ByEveryDay, 60);
      this.retainKeys = new Dictionary<string, MqttClientApplicationMessage>();
      this.apiTopicServiceDict = new Dictionary<string, MqttRpcApiInfo>();
      this.keysLock = new object();
      this.rpcApiLock = new object();
      this.timerHeart = new System.Threading.Timer(new TimerCallback(this.ThreadTimerHeartCheck), (object) null, 2000, 10000);
      this.dictionaryFilesMarks = new Dictionary<string, FileMarkId>();
      this.dictHybirdLock = new object();
    }

    /// <inheritdoc />
    protected override async void ThreadPoolLogin(Socket socket, IPEndPoint endPoint)
    {
      OperateResult<byte, byte[]> readMqtt = await this.ReceiveMqttMessageAsync(socket, 10000);
      this.HandleMqttConnection(socket, endPoint, readMqtt);
      readMqtt = (OperateResult<byte, byte[]>) null;
    }

    private async void SocketReceiveCallback(IAsyncResult ar)
    {
      MqttSession mqttSession = ar.AsyncState as MqttSession;
      if (mqttSession == null)
        ;
      else
      {
        try
        {
          mqttSession.MqttSocket.EndReceive(ar);
        }
        catch (Exception ex)
        {
          this.RemoveAndCloseSession(mqttSession, "Socket EndReceive -> " + ex.Message);
          return;
        }
        if (mqttSession.Protocol == "FILE")
        {
          if (this.fileServerEnabled)
            await this.HandleFileMessageAsync(mqttSession);
          this.RemoveAndCloseSession(mqttSession, string.Empty);
        }
        else
        {
          OperateResult<byte, byte[]> readMqtt = (OperateResult<byte, byte[]>) null;
          if (mqttSession.Protocol == "MQTT")
            readMqtt = await this.ReceiveMqttMessageAsync(mqttSession.MqttSocket, 60000);
          else
            readMqtt = await this.ReceiveMqttMessageAsync(mqttSession.MqttSocket, 60000, (Action<long, long>) ((already, total) => this.SyncMqttReceiveProgressBack(mqttSession.MqttSocket, already, total)));
          this.HandleWithReceiveMqtt(mqttSession, readMqtt);
          readMqtt = (OperateResult<byte, byte[]>) null;
        }
      }
    }

    private void SyncMqttReceiveProgressBack(Socket socket, long already, long total)
    {
      string message = total > 0L ? (already * 100L / total).ToString() : "100";
      byte[] payLoad = new byte[16];
      BitConverter.GetBytes(already).CopyTo((Array) payLoad, 0);
      BitConverter.GetBytes(total).CopyTo((Array) payLoad, 8);
      this.Send(socket, MqttHelper.BuildMqttCommand((byte) 15, (byte) 0, MqttHelper.BuildSegCommandByString(message), payLoad).Content);
    }

    private void HandleMqttConnection(
      Socket socket,
      IPEndPoint endPoint,
      OperateResult<byte, byte[]> readMqtt)
    {
      if (!readMqtt.IsSuccess)
        return;
      OperateResult<int, MqttSession> operateResult = this.CheckMqttConnection(readMqtt.Content1, readMqtt.Content2, socket, endPoint);
      if (!operateResult.IsSuccess)
      {
        this.LogNet?.WriteInfo(this.ToString(), operateResult.Message);
        socket?.Close();
      }
      else if ((uint) operateResult.Content1 > 0U)
      {
        this.Send(socket, MqttHelper.BuildMqttCommand((byte) 2, (byte) 0, (byte[]) null, new byte[2]
        {
          (byte) 0,
          (byte) operateResult.Content1
        }).Content);
        socket?.Close();
      }
      else
      {
        this.Send(socket, MqttHelper.BuildMqttCommand((byte) 2, (byte) 0, (byte[]) null, new byte[2]).Content);
        try
        {
          socket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, new AsyncCallback(this.SocketReceiveCallback), (object) operateResult.Content2);
          this.AddMqttSession(operateResult.Content2);
        }
        catch (Exception ex)
        {
          this.LogNet?.WriteDebug(this.ToString(), "Client Online Exception : " + ex.Message);
          return;
        }
        if (!(operateResult.Content2.Protocol == "MQTT"))
          return;
        MqttServer.OnClientConnectedDelegate onClientConnected = this.OnClientConnected;
        if (onClientConnected == null)
          return;
        onClientConnected(operateResult.Content2);
      }
    }

    private OperateResult<int, MqttSession> CheckMqttConnection(
      byte mqttCode,
      byte[] content,
      Socket socket,
      IPEndPoint endPoint)
    {
      if ((int) mqttCode >> 4 != 1)
        return new OperateResult<int, MqttSession>("Client Send Faied, And Close!");
      if (content.Length < 10)
        return new OperateResult<int, MqttSession>("Receive Data Too Short:" + SoftBasic.ByteToHexString(content, ' '));
      string protocol = Encoding.ASCII.GetString(content, 2, 4);
      if (!(protocol == "MQTT") && !(protocol == "HUSL") && !(protocol == "FILE"))
        return new OperateResult<int, MqttSession>("Not Mqtt Client Connection");
      try
      {
        int index = 10;
        string clientId = MqttHelper.ExtraMsgFromBytes(content, ref index);
        string str1 = ((int) content[7] & 4) == 4 ? MqttHelper.ExtraMsgFromBytes(content, ref index) : string.Empty;
        string str2 = ((int) content[7] & 4) == 4 ? MqttHelper.ExtraMsgFromBytes(content, ref index) : string.Empty;
        string userName = ((int) content[7] & 128) == 128 ? MqttHelper.ExtraMsgFromBytes(content, ref index) : string.Empty;
        string passwrod = ((int) content[7] & 64) == 64 ? MqttHelper.ExtraMsgFromBytes(content, ref index) : string.Empty;
        int num1 = (int) content[8] * 256 + (int) content[9];
        MqttSession mqttSession = new MqttSession(endPoint, protocol)
        {
          MqttSocket = socket,
          ClientId = clientId,
          UserName = userName
        };
        int num2 = this.ClientVerification != null ? this.ClientVerification(mqttSession, clientId, userName, passwrod) : 0;
        if (num1 > 0)
          mqttSession.ActiveTimeSpan = TimeSpan.FromSeconds((double) num1);
        return OperateResult.CreateSuccessResult<int, MqttSession>(num2, mqttSession);
      }
      catch (Exception ex)
      {
        return new OperateResult<int, MqttSession>("Client Online Exception : " + ex.Message);
      }
    }

    private void HandleWithReceiveMqtt(
      MqttSession mqttSession,
      OperateResult<byte, byte[]> readMqtt)
    {
      if (!readMqtt.IsSuccess)
      {
        this.RemoveAndCloseSession(mqttSession, readMqtt.Message);
      }
      else
      {
        byte content1 = readMqtt.Content1;
        byte[] content2 = readMqtt.Content2;
        try
        {
          if ((int) content1 >> 4 != 14)
          {
            mqttSession.MqttSocket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, new AsyncCallback(this.SocketReceiveCallback), (object) mqttSession);
          }
          else
          {
            this.RemoveAndCloseSession(mqttSession, string.Empty);
            return;
          }
        }
        catch (Exception ex)
        {
          this.RemoveAndCloseSession(mqttSession, "HandleWithReceiveMqtt:" + ex.Message);
          return;
        }
        mqttSession.ActiveTime = DateTime.Now;
        if (mqttSession.Protocol != "MQTT")
          this.DealWithPublish(mqttSession, content1, content2);
        else if ((int) content1 >> 4 == 3)
          this.DealWithPublish(mqttSession, content1, content2);
        else if ((int) content1 >> 4 != 4 && (int) content1 >> 4 != 5)
        {
          if ((int) content1 >> 4 == 6)
            this.Send(mqttSession.MqttSocket, MqttHelper.BuildMqttCommand((byte) 7, (byte) 0, (byte[]) null, content2).Content);
          else if ((int) content1 >> 4 == 8)
            this.DealWithSubscribe(mqttSession, content1, content2);
          else if ((int) content1 >> 4 == 10)
            this.DealWithUnSubscribe(mqttSession, content1, content2);
          else if ((int) content1 >> 4 == 12)
            this.Send(mqttSession.MqttSocket, MqttHelper.BuildMqttCommand((byte) 13, (byte) 0, (byte[]) null, (byte[]) null).Content);
        }
      }
    }

    /// <inheritdoc />
    protected override void StartInitialization()
    {
    }

    /// <inheritdoc />
    protected override void CloseAction()
    {
      base.CloseAction();
      lock (this.sessionsLock)
      {
        for (int index = 0; index < this.mqttSessions.Count; ++index)
          this.mqttSessions[index].MqttSocket?.Close();
        this.mqttSessions.Clear();
      }
    }

    private void ThreadTimerHeartCheck(object obj)
    {
      MqttSession[] mqttSessionArray = (MqttSession[]) null;
      lock (this.sessionsLock)
        mqttSessionArray = this.mqttSessions.ToArray();
      if (mqttSessionArray == null || (uint) mqttSessionArray.Length <= 0U)
        return;
      for (int index = 0; index < mqttSessionArray.Length; ++index)
      {
        if (mqttSessionArray[index].Protocol == "MQTT" && DateTime.Now - mqttSessionArray[index].ActiveTime > mqttSessionArray[index].ActiveTimeSpan)
          this.RemoveAndCloseSession(mqttSessionArray[index], "Thread Timer Heart Check failed:" + SoftBasic.GetTimeSpanDescription(DateTime.Now - mqttSessionArray[index].ActiveTime));
      }
    }

    private void DealWithPublish(MqttSession session, byte code, byte[] data)
    {
      bool flag1 = ((int) code & 8) == 8;
      int num1 = (((int) code & 4) == 4 ? 2 : 0) + (((int) code & 2) == 2 ? 1 : 0);
      MqttQualityOfServiceLevel qualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce;
      switch (num1)
      {
        case 1:
          qualityOfServiceLevel = MqttQualityOfServiceLevel.AtLeastOnce;
          break;
        case 2:
          qualityOfServiceLevel = MqttQualityOfServiceLevel.ExactlyOnce;
          break;
        case 3:
          qualityOfServiceLevel = MqttQualityOfServiceLevel.OnlyTransfer;
          break;
      }
      bool flag2 = ((int) code & 1) == 1;
      int data1 = 0;
      int index = 0;
      string topic = MqttHelper.ExtraMsgFromBytes(data, ref index);
      if (num1 > 0)
        data1 = MqttHelper.ExtraIntFromBytes(data, ref index);
      byte[] payload = SoftBasic.ArrayRemoveBegin<byte>(data, index);
      MqttClientApplicationMessage applicationMessage = new MqttClientApplicationMessage();
      applicationMessage.ClientId = session.ClientId;
      applicationMessage.QualityOfServiceLevel = qualityOfServiceLevel;
      applicationMessage.Retain = flag2;
      applicationMessage.Topic = topic;
      applicationMessage.UserName = session.UserName;
      applicationMessage.Payload = payload;
      MqttClientApplicationMessage message = applicationMessage;
      if (session.Protocol == "MQTT")
      {
        switch (qualityOfServiceLevel)
        {
          case MqttQualityOfServiceLevel.AtLeastOnce:
            this.Send(session.MqttSocket, MqttHelper.BuildMqttCommand((byte) 4, (byte) 0, (byte[]) null, MqttHelper.BuildIntBytes(data1)).Content);
            break;
          case MqttQualityOfServiceLevel.ExactlyOnce:
            this.Send(session.MqttSocket, MqttHelper.BuildMqttCommand((byte) 5, (byte) 0, (byte[]) null, MqttHelper.BuildIntBytes(data1)).Content);
            break;
        }
        if (session.ForbidPublishTopic)
          return;
        MqttServer.OnClientApplicationMessageReceiveDelegate applicationMessageReceive = this.OnClientApplicationMessageReceive;
        if (applicationMessageReceive != null)
          applicationMessageReceive(session, message);
        if (qualityOfServiceLevel == MqttQualityOfServiceLevel.OnlyTransfer || message.IsCancelPublish)
          return;
        this.PublishTopicPayload(topic, payload, false);
        if (flag2)
          this.RetainTopicPayload(topic, message);
      }
      else if ((int) code >> 4 == 3)
      {
        MqttRpcApiInfo mqttRpcApiInfo = this.GetMqttRpcApiInfo(message.Topic.Trim('/'));
        if (mqttRpcApiInfo == null)
        {
          MqttServer.OnClientApplicationMessageReceiveDelegate applicationMessageReceive = this.OnClientApplicationMessageReceive;
          if (applicationMessageReceive != null)
            applicationMessageReceive(session, message);
        }
        else
        {
          DateTime now = DateTime.Now;
          OperateResult<string> result = MqttHelper.HandleObjectMethod(session, message, mqttRpcApiInfo);
          double num2 = Math.Round((DateTime.Now - now).TotalSeconds, 5);
          mqttRpcApiInfo.CalledCountAddOne((long) (num2 * 100000.0));
          this.statisticsDict.StatisticsAdd(mqttRpcApiInfo.ApiTopic);
          this.LogNet?.WriteDebug(this.ToString(), string.Format("{0} RPC: [{1}] Spend:[{2:F2} ms] Count:[{3}] Return:[{4}]", (object) session, (object) message.Topic, (object) (num2 * 1000.0), (object) mqttRpcApiInfo.CalledCount, (object) result.IsSuccess));
          this.ReportOperateResult(session, result);
        }
      }
      else if ((int) code >> 4 == 8)
        this.ReportOperateResult(session, OperateResult.CreateSuccessResult<string>(JArray.FromObject((object) this.GetAllMqttRpcApiInfo()).ToString()));
      else if ((int) code >> 4 == 4)
        this.PublishTopicPayload(session, "", EstProtocol.PackStringArrayToByte(this.GetAllRetainTopics()));
      else if ((int) code >> 4 == 6)
      {
        long[] numArray = string.IsNullOrEmpty(message.Topic) ? this.LogStatistics.LogStat.GetStatisticsSnapshot() : this.LogStatistics.GetStatisticsSnapshot(message.Topic);
        if (numArray == null)
          this.ReportOperateResult(session, new OperateResult<string>(string.Format("{0} RPC:{1} has no data or not exist.", (object) session, (object) message.Topic)));
        else
          this.ReportOperateResult(session, OperateResult.CreateSuccessResult<string>(numArray.ToArrayString<long>()));
      }
      else if ((int) code >> 4 == 5)
      {
        lock (this.keysLock)
        {
          if (this.retainKeys.ContainsKey(message.Topic))
            this.PublishTopicPayload(session, message.Topic, Encoding.UTF8.GetBytes(this.retainKeys[message.Topic].ToJsonString()));
          else
            this.ReportOperateResult(session, StringResources.Language.KeyIsNotExist);
        }
      }
    }

    /// <summary>将消息进行驻留到内存词典，方便进行其他的功能操作。</summary>
    /// <param name="topic">消息的主题</param>
    /// <param name="payload">当前的数据负载</param>
    private void RetainTopicPayload(string topic, byte[] payload)
    {
      MqttClientApplicationMessage applicationMessage1 = new MqttClientApplicationMessage();
      applicationMessage1.ClientId = nameof (MqttServer);
      applicationMessage1.QualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce;
      applicationMessage1.Retain = true;
      applicationMessage1.Topic = topic;
      applicationMessage1.UserName = nameof (MqttServer);
      applicationMessage1.Payload = payload;
      MqttClientApplicationMessage applicationMessage2 = applicationMessage1;
      lock (this.keysLock)
      {
        if (this.retainKeys.ContainsKey(topic))
          this.retainKeys[topic] = applicationMessage2;
        else
          this.retainKeys.Add(topic, applicationMessage2);
      }
    }

    /// <summary>将消息进行驻留到内存词典，方便进行其他的功能操作。</summary>
    /// <param name="topic">消息的主题</param>
    /// <param name="message">当前的Mqtt消息</param>
    private void RetainTopicPayload(string topic, MqttClientApplicationMessage message)
    {
      lock (this.keysLock)
      {
        if (this.retainKeys.ContainsKey(topic))
          this.retainKeys[topic] = message;
        else
          this.retainKeys.Add(topic, message);
      }
    }

    private void DealWithSubscribe(MqttSession session, byte code, byte[] data)
    {
      int index1 = 0;
      int data1 = MqttHelper.ExtraIntFromBytes(data, ref index1);
      List<string> stringList = new List<string>();
      while (index1 < data.Length - 1)
        stringList.Add(MqttHelper.ExtraSubscribeMsgFromBytes(data, ref index1));
      if (index1 < data.Length)
        this.Send(session.MqttSocket, MqttHelper.BuildMqttCommand((byte) 9, (byte) 0, MqttHelper.BuildIntBytes(data1), new byte[1]
        {
          data[index1]
        }).Content);
      else
        this.Send(session.MqttSocket, MqttHelper.BuildMqttCommand((byte) 9, (byte) 0, (byte[]) null, MqttHelper.BuildIntBytes(data1)).Content);
      lock (this.keysLock)
      {
        for (int index2 = 0; index2 < stringList.Count; ++index2)
        {
          if (this.retainKeys.ContainsKey(stringList[index2]))
            this.Send(session.MqttSocket, MqttHelper.BuildPublishMqttCommand(stringList[index2], this.retainKeys[stringList[index2]].Payload).Content);
        }
      }
      session.AddSubscribe(stringList.ToArray());
    }

    private void DealWithUnSubscribe(MqttSession session, byte code, byte[] data)
    {
      int index = 0;
      int data1 = MqttHelper.ExtraIntFromBytes(data, ref index);
      List<string> stringList = new List<string>();
      for (; index < data.Length; ++index)
        stringList.Add(MqttHelper.ExtraMsgFromBytes(data, ref index));
      this.Send(session.MqttSocket, MqttHelper.BuildMqttCommand((byte) 11, (byte) 0, (byte[]) null, MqttHelper.BuildIntBytes(data1)).Content);
      session.RemoveSubscribe(stringList.ToArray());
    }

    /// <summary>
    /// 向指定的客户端发送主题及负载数据<br />
    /// Sends the topic and payload data to the specified client
    /// </summary>
    /// <param name="session">会话内容</param>
    /// <param name="topic">主题</param>
    /// <param name="payload">消息内容</param>
    public void PublishTopicPayload(MqttSession session, string topic, byte[] payload)
    {
      OperateResult operateResult = this.Send(session.MqttSocket, MqttHelper.BuildPublishMqttCommand(topic, payload).Content);
      if (operateResult.IsSuccess)
        return;
      this.LogNet?.WriteError(this.ToString(), string.Format("{0} PublishTopicPayload Failed:", (object) session) + operateResult.Message);
    }

    /// <summary>
    /// 从服务器向订阅了指定的主题的客户端发送消息，默认消息不驻留<br />
    /// Sends a message from the server to a client that subscribes to the specified topic; the default message does not retain
    /// </summary>
    /// <param name="topic">主题</param>
    /// <param name="payload">消息内容</param>
    /// <param name="retain">指示消息是否驻留</param>
    public void PublishTopicPayload(string topic, byte[] payload, bool retain = true)
    {
      lock (this.sessionsLock)
      {
        for (int index = 0; index < this.mqttSessions.Count; ++index)
        {
          if (this.mqttSessions[index].IsClientSubscribe(topic) && this.mqttSessions[index].Protocol == "MQTT")
          {
            OperateResult operateResult = this.Send(this.mqttSessions[index].MqttSocket, MqttHelper.BuildPublishMqttCommand(topic, payload).Content);
            if (!operateResult.IsSuccess)
              this.LogNet?.WriteError(this.ToString(), string.Format("{0} PublishTopicPayload Failed:", (object) this.mqttSessions[index]) + operateResult.Message);
          }
        }
      }
      if (!retain)
        return;
      this.RetainTopicPayload(topic, payload);
    }

    /// <summary>
    /// 向所有的客户端强制发送主题及负载数据，默认消息不驻留<br />
    /// Send subject and payload data to all clients compulsively, and the default message does not retain
    /// </summary>
    /// <param name="topic">主题</param>
    /// <param name="payload">消息内容</param>
    /// <param name="retain">指示消息是否驻留</param>
    public void PublishAllClientTopicPayload(string topic, byte[] payload, bool retain = false)
    {
      lock (this.sessionsLock)
      {
        for (int index = 0; index < this.mqttSessions.Count; ++index)
        {
          if (this.mqttSessions[index].Protocol == "MQTT")
          {
            OperateResult operateResult = this.Send(this.mqttSessions[index].MqttSocket, MqttHelper.BuildPublishMqttCommand(topic, payload).Content);
            if (!operateResult.IsSuccess)
              this.LogNet?.WriteError(this.ToString(), string.Format("{0} PublishTopicPayload Failed:", (object) this.mqttSessions[index]) + operateResult.Message);
          }
        }
      }
      if (!retain)
        return;
      this.RetainTopicPayload(topic, payload);
    }

    /// <summary>
    /// 向指定的客户端ID强制发送消息，默认消息不驻留<br />
    /// Forces a message to the specified client ID, and the default message does not retain
    /// </summary>
    /// <param name="clientId">指定的客户端ID信息</param>
    /// <param name="topic">主题</param>
    /// <param name="payload">消息内容</param>
    /// <param name="retain">指示消息是否驻留</param>
    public void PublishTopicPayload(string clientId, string topic, byte[] payload, bool retain = false)
    {
      lock (this.sessionsLock)
      {
        for (int index = 0; index < this.mqttSessions.Count; ++index)
        {
          if (this.mqttSessions[index].ClientId == clientId && this.mqttSessions[index].Protocol == "MQTT")
          {
            OperateResult operateResult = this.Send(this.mqttSessions[index].MqttSocket, MqttHelper.BuildPublishMqttCommand(topic, payload).Content);
            if (!operateResult.IsSuccess)
              this.LogNet?.WriteError(this.ToString(), string.Format("{0} PublishTopicPayload Failed:", (object) this.mqttSessions[index]) + operateResult.Message);
          }
        }
      }
      if (!retain)
        return;
      this.RetainTopicPayload(topic, payload);
    }

    /// <summary>
    /// 向客户端发布一个进度报告的信息，仅用于同步网络的时候才支持进度报告，将进度及消息发送给客户端，比如你的服务器需要分成5个部分完成，可以按照百分比提示给客户端当前服务器发生了什么<br />
    /// Publish the information of a progress report to the client. The progress report is only supported when the network is synchronized.
    /// The progress and the message are sent to the client. For example, your server needs to be divided into 5 parts to complete.
    /// You can prompt the client according to the percentage. What happened to the server
    /// </summary>
    /// <param name="session">当前的网络会话</param>
    /// <param name="topic">回发客户端的关键数据，可以是百分比字符串，甚至是自定义的任意功能</param>
    /// <param name="payload">数据消息</param>
    public void ReportProgress(MqttSession session, string topic, string payload)
    {
      if (!(session.Protocol == "HUSL"))
        throw new Exception("ReportProgress only support sync communication");
      payload = payload ?? string.Empty;
      OperateResult operateResult = this.Send(session.MqttSocket, MqttHelper.BuildMqttCommand((byte) 15, (byte) 0, MqttHelper.BuildSegCommandByString(topic), Encoding.UTF8.GetBytes(payload)).Content);
      if (operateResult.IsSuccess)
        return;
      this.LogNet?.WriteError(this.ToString(), string.Format("{0} PublishTopicPayload Failed:", (object) session) + operateResult.Message);
    }

    /// <summary>
    /// 向客户端发布一个失败的操作信息，仅用于同步网络的时候反馈失败结果，将错误的信息反馈回客户端，客户端就知道服务器发生了什么，为什么反馈失败。<br />
    /// Publish a failed operation information to the client, which is only used to feed back the failure result when synchronizing the network.
    /// If the error information is fed back to the client, the client will know what happened to the server and why the feedback failed.
    /// </summary>
    /// <param name="session">当前的网络会话</param>
    /// <param name="message">错误的消息文本信息</param>
    public void ReportOperateResult(MqttSession session, string message) => this.ReportOperateResult(session, new OperateResult<string>(message));

    /// <summary>
    /// 向客户端发布一个操作结果的信息，仅用于同步网络的时候反馈操作结果，该操作可能成功，可能失败，客户端就知道服务器发生了什么，以及结果如何。<br />
    /// Publish an operation result information to the client, which is only used to feed back the operation result when synchronizing the network.
    /// The operation may succeed or fail, and the client knows what happened to the server and the result.
    /// </summary>
    /// <param name="session">当前的网络会话</param>
    /// <param name="result">结果对象内容</param>
    public void ReportOperateResult(MqttSession session, OperateResult<string> result)
    {
      if (!(session.Protocol == "HUSL"))
        throw new Exception("Report Result Message only support sync communication, client is MqttSyncClient");
      if (result.IsSuccess)
      {
        this.PublishTopicPayload(session, result.ErrorCode.ToString(), string.IsNullOrEmpty(result.Content) ? new byte[0] : Encoding.UTF8.GetBytes(result.Content));
      }
      else
      {
        OperateResult operateResult = this.Send(session.MqttSocket, MqttHelper.BuildMqttCommand((byte) 0, (byte) 0, MqttHelper.BuildSegCommandByString(result.ErrorCode.ToString()), string.IsNullOrEmpty(result.Message) ? new byte[0] : Encoding.UTF8.GetBytes(result.Message)).Content);
        if (!operateResult.IsSuccess)
          this.LogNet?.WriteError(this.ToString(), string.Format("{0} PublishTopicPayload Failed:", (object) session) + operateResult.Message);
      }
    }

    /// <summary>
    /// 使用指定的对象来返回网络的API接口，前提是传入的数据为json参数，返回的数据为 <c>OperateResult&lt;string&gt;</c> 数据，详细参照说明<br />
    /// Use the specified object to return the API interface of the network,
    /// provided that the incoming data is json parameters and the returned data is <c>OperateResult&lt;string&gt;</c> data,
    /// please refer to the description for details
    /// </summary>
    /// <param name="session">当前的会话内容</param>
    /// <param name="message">客户端发送的消息，其中的payload将会解析为一个json字符串，然后提取参数信息。</param>
    /// <param name="apiObject">当前的对象的内容信息</param>
    public void ReportObjectApiMethod(
      MqttSession session,
      MqttClientApplicationMessage message,
      object apiObject)
    {
      if (!(session.Protocol == "HUSL"))
        throw new Exception("Report Result Message only support sync communication, client is MqttSyncClient");
      this.ReportOperateResult(session, MqttHelper.HandleObjectMethod(session, message, apiObject));
    }

    private MqttRpcApiInfo GetMqttRpcApiInfo(string apiTopic)
    {
      MqttRpcApiInfo mqttRpcApiInfo = (MqttRpcApiInfo) null;
      lock (this.rpcApiLock)
      {
        if (this.apiTopicServiceDict.ContainsKey(apiTopic))
          mqttRpcApiInfo = this.apiTopicServiceDict[apiTopic];
      }
      return mqttRpcApiInfo;
    }

    /// <summary>
    /// 获取当前所有注册的RPC接口信息，将返回一个数据列表。<br />
    /// Get all currently registered RPC interface information, and a data list will be returned.
    /// </summary>
    /// <returns>信息列表</returns>
    public MqttRpcApiInfo[] GetAllMqttRpcApiInfo()
    {
      MqttRpcApiInfo[] mqttRpcApiInfoArray = (MqttRpcApiInfo[]) null;
      lock (this.rpcApiLock)
        mqttRpcApiInfoArray = this.apiTopicServiceDict.Values.ToArray<MqttRpcApiInfo>();
      return mqttRpcApiInfoArray;
    }

    /// <summary>
    /// 注册一个RPC的服务接口，可以指定当前的控制器名称，以及提供RPC服务的原始对象，指定统一的权限控制。<br />
    /// Register an RPC service interface, you can specify the current controller name,
    /// and the original object that provides the RPC service, Specify unified access control
    /// </summary>
    /// <param name="api">前置的接口信息，可以理解为MVC模式的控制器</param>
    /// <param name="obj">原始对象信息</param>
    /// <param name="permissionAttribute">统一的权限访问配置，将会覆盖单个方法的权限控制。</param>
    public void RegisterMqttRpcApi(
      string api,
      object obj,
      EstMqttPermissionAttribute permissionAttribute)
    {
      lock (this.rpcApiLock)
      {
        foreach (MqttRpcApiInfo mqttRpcApiInfo in MqttHelper.GetSyncServicesApiInformationFromObject(api, obj, permissionAttribute))
          this.apiTopicServiceDict.Add(mqttRpcApiInfo.ApiTopic, mqttRpcApiInfo);
      }
    }

    /// <summary>
    /// 注册一个RPC的服务接口，可以指定当前的控制器名称，以及提供RPC服务的原始对象<br />
    /// Register an RPC service interface, you can specify the current controller name,
    /// and the original object that provides the RPC service
    /// </summary>
    /// <param name="api">前置的接口信息，可以理解为MVC模式的控制器</param>
    /// <param name="obj">原始对象信息</param>
    public void RegisterMqttRpcApi(string api, object obj)
    {
      lock (this.rpcApiLock)
      {
        foreach (MqttRpcApiInfo mqttRpcApiInfo in MqttHelper.GetSyncServicesApiInformationFromObject(api, obj))
          this.apiTopicServiceDict.Add(mqttRpcApiInfo.ApiTopic, mqttRpcApiInfo);
      }
    }

    /// <inheritdoc cref="M:EstCommunication.MQTT.MqttServer.RegisterMqttRpcApi(System.String,System.Object)" />
    public void RegisterMqttRpcApi(object obj)
    {
      lock (this.rpcApiLock)
      {
        foreach (MqttRpcApiInfo mqttRpcApiInfo in MqttHelper.GetSyncServicesApiInformationFromObject(obj))
          this.apiTopicServiceDict.Add(mqttRpcApiInfo.ApiTopic, mqttRpcApiInfo);
      }
    }

    /// <summary>
    /// 启动文件服务功能，协议头为FILE，需要指定服务器存储的文件路径<br />
    /// Start the file service function, the protocol header is FILE, you need to specify the file path stored by the server
    /// </summary>
    /// <param name="filePath">文件的存储路径</param>
    public void UseFileServer(string filePath)
    {
      this.filesDirectoryPath = filePath;
      this.fileServerEnabled = true;
      this.CheckFolderAndCreate();
    }

    /// <summary>关闭文件服务功能</summary>
    public void CloseFileServer() => this.fileServerEnabled = false;

    /// <summary>
    /// 获取当前的针对文件夹的文件管理容器的数量<br />
    /// Get the current number of file management containers for the folder
    /// </summary>
    [EstMqttApi(Description = "Get the current number of file management containers for the folder")]
    public int GroupFileContainerCount() => this.m_dictionary_group_marks.Count;

    /// <summary>
    /// 获取当前实时的文件上传下载的监控信息，操作的客户端信息，文件分类，文件名，上传或下载的速度等<br />
    /// Obtain current real-time file upload and download monitoring information, operating client information, file classification, file name, upload or download speed, etc.
    /// </summary>
    /// <returns>文件的监控信息</returns>
    [EstMqttApi(Description = "Obtain current real-time file upload and download monitoring information, operating client information, file classification, file name, upload or download speed, etc.")]
    public MqttFileMonitorItem[] GetMonitorItemsSnapShoot() => this.fileMonitor.GetMonitorItemsSnapShoot();

    /// <summary>
    /// 当客户端进行文件操作时，校验客户端合法性的事件，操作码具体查看<seealso cref="T:EstCommunication.MQTT.MqttControlMessage" />的常量值<br />
    /// When client performing file operations, it is an event to verify the legitimacy of the client. For the operation code, check the constant value of <seealso cref="T:EstCommunication.MQTT.MqttControlMessage" />
    /// </summary>
    public event MqttServer.FileOperateVerificationDelegate FileOperateVerification;

    private bool CheckPathAndFilenameLegal(string input) => input.Contains(":") || input.Contains("?") || (input.Contains("*") || input.Contains("/")) || (input.Contains("\\") || input.Contains("\"") || (input.Contains("<") || input.Contains(">"))) || input.Contains("|");

    private async Task HandleFileMessageAsync(MqttSession session)
    {
      OperateResult<byte, byte[]> receiveGroupInfo = await this.ReceiveMqttMessageAsync(session.MqttSocket, 60000);
      string[] groupInfo;
      OperateResult<byte, byte[]> receiveFileNames;
      string[] fileNames;
      OperateResult opLegal;
      OperateResult sendLegal;
      string relativeName;
      if (!receiveGroupInfo.IsSuccess)
      {
        receiveGroupInfo = (OperateResult<byte, byte[]>) null;
        groupInfo = (string[]) null;
        receiveFileNames = (OperateResult<byte, byte[]>) null;
        fileNames = (string[]) null;
        opLegal = (OperateResult) null;
        sendLegal = (OperateResult) null;
        relativeName = (string) null;
      }
      else
      {
        groupInfo = EstProtocol.UnPackStringArrayFromByte(receiveGroupInfo.Content2);
        receiveFileNames = await this.ReceiveMqttMessageAsync(session.MqttSocket, 60000);
        if (!receiveFileNames.IsSuccess)
        {
          receiveGroupInfo = (OperateResult<byte, byte[]>) null;
          groupInfo = (string[]) null;
          receiveFileNames = (OperateResult<byte, byte[]>) null;
          fileNames = (string[]) null;
          opLegal = (OperateResult) null;
          sendLegal = (OperateResult) null;
          relativeName = (string) null;
        }
        else
        {
          fileNames = EstProtocol.UnPackStringArrayFromByte(receiveFileNames.Content2);
          for (int i = 0; i < groupInfo.Length; ++i)
          {
            if (this.CheckPathAndFilenameLegal(groupInfo[i]))
            {
              this.Send(session.MqttSocket, MqttHelper.BuildMqttCommand((byte) 0, (byte[]) null, EstHelper.GetUTF8Bytes("Path Invalid, not include ':', '?'")).Content);
              this.RemoveAndCloseSession(session, "CheckPathAndFilenameLegal:" + groupInfo[i]);
              receiveGroupInfo = (OperateResult<byte, byte[]>) null;
              groupInfo = (string[]) null;
              receiveFileNames = (OperateResult<byte, byte[]>) null;
              fileNames = (string[]) null;
              opLegal = (OperateResult) null;
              sendLegal = (OperateResult) null;
              relativeName = (string) null;
              return;
            }
          }
          for (int i = 0; i < fileNames.Length; ++i)
          {
            if (this.CheckPathAndFilenameLegal(fileNames[i]))
            {
              this.Send(session.MqttSocket, MqttHelper.BuildMqttCommand((byte) 0, (byte[]) null, EstHelper.GetUTF8Bytes("FileName Invalid, not include '\\/:*?\"<>|'")).Content);
              this.RemoveAndCloseSession(session, "CheckPathAndFilenameLegal:" + fileNames[i]);
              receiveGroupInfo = (OperateResult<byte, byte[]>) null;
              groupInfo = (string[]) null;
              receiveFileNames = (OperateResult<byte, byte[]>) null;
              fileNames = (string[]) null;
              opLegal = (OperateResult) null;
              sendLegal = (OperateResult) null;
              relativeName = (string) null;
              return;
            }
          }
          MqttServer.FileOperateVerificationDelegate operateVerification = this.FileOperateVerification;
          opLegal = (operateVerification != null ? operateVerification(session, receiveFileNames.Content1, groupInfo, fileNames) : (OperateResult) null) ?? OperateResult.CreateSuccessResult();
          sendLegal = await this.SendAsync(session.MqttSocket, MqttHelper.BuildMqttCommand(opLegal.IsSuccess ? (byte) 100 : (byte) 0, (byte[]) null, EstHelper.GetUTF8Bytes(opLegal.Message)).Content);
          if (!opLegal.IsSuccess)
          {
            this.RemoveAndCloseSession(session, "FileOperateVerification:" + opLegal.Message);
            receiveGroupInfo = (OperateResult<byte, byte[]>) null;
            groupInfo = (string[]) null;
            receiveFileNames = (OperateResult<byte, byte[]>) null;
            fileNames = (string[]) null;
            opLegal = (OperateResult) null;
            sendLegal = (OperateResult) null;
            relativeName = (string) null;
          }
          else if (!sendLegal.IsSuccess)
          {
            this.RemoveAndCloseSession(session, "FileOperate SendLegal:" + sendLegal.Message);
            receiveGroupInfo = (OperateResult<byte, byte[]>) null;
            groupInfo = (string[]) null;
            receiveFileNames = (OperateResult<byte, byte[]>) null;
            fileNames = (string[]) null;
            opLegal = (OperateResult) null;
            sendLegal = (OperateResult) null;
            relativeName = (string) null;
          }
          else
          {
            string[] groups = groupInfo;
            string[] strArray1 = fileNames;
            string fileName1 = (strArray1 != null ? ((uint) strArray1.Length > 0U ? 1 : 0) : 0) != 0 ? fileNames[0] : string.Empty;
            relativeName = this.GetRelativeFileName(groups, fileName1);
            if (receiveFileNames.Content1 == (byte) 101)
            {
              string fileName = fileNames[0];
              string guidName = this.TransformFactFileName(groupInfo, fileName);
              FileMarkId fileMarkId = this.GetFileMarksFromDictionaryWithFileName(guidName);
              fileMarkId.EnterReadOperator();
              DateTime dateTimeStart = DateTime.Now;
              MqttFileMonitorItem monitorItem = new MqttFileMonitorItem()
              {
                EndPoint = session.EndPoint,
                ClientId = session.ClientId,
                UserName = session.UserName,
                FileName = fileName,
                Operate = "Download",
                Groups = EstHelper.PathCombine(groupInfo)
              };
              this.fileMonitor.Add(monitorItem);
              OperateResult send = await this.SendMqttFileAsync(session.MqttSocket, this.ReturnAbsoluteFileName(groupInfo, guidName), fileName, "", new Action<long, long>(monitorItem.UpdateProgress));
              fileMarkId.LeaveReadOperator();
              this.fileMonitor.Remove(monitorItem.UniqueId);
              MqttServer.FileChangedDelegate fileChangedEvent = this.OnFileChangedEvent;
              if (fileChangedEvent != null)
                fileChangedEvent(session, new MqttFileOperateInfo()
                {
                  Groups = EstHelper.PathCombine(groupInfo),
                  FileNames = fileNames,
                  Operate = "Download",
                  TimeCost = DateTime.Now - dateTimeStart
                });
              if (!send.IsSuccess)
                this.LogNet?.WriteError(this.ToString(), string.Format("{0} {1} : {2} :{3} Name:{4}", (object) session, (object) StringResources.Language.FileDownloadFailed, (object) send.Message, (object) relativeName, (object) session.UserName) + " Spend:" + SoftBasic.GetTimeSpanDescription(DateTime.Now - dateTimeStart));
              else
                this.LogNet?.WriteInfo(this.ToString(), string.Format("{0} {1} : {2} Spend:{3}", (object) session, (object) StringResources.Language.FileDownloadSuccess, (object) relativeName, (object) SoftBasic.GetTimeSpanDescription(DateTime.Now - dateTimeStart)));
              fileName = (string) null;
              guidName = (string) null;
              fileMarkId = (FileMarkId) null;
              monitorItem = (MqttFileMonitorItem) null;
              send = (OperateResult) null;
              receiveGroupInfo = (OperateResult<byte, byte[]>) null;
              groupInfo = (string[]) null;
              receiveFileNames = (OperateResult<byte, byte[]>) null;
              fileNames = (string[]) null;
              opLegal = (OperateResult) null;
              sendLegal = (OperateResult) null;
              relativeName = (string) null;
            }
            else if (receiveFileNames.Content1 == (byte) 102)
            {
              string fileName = fileNames[0];
              string fullFileName = this.ReturnAbsoluteFileName(groupInfo, fileName);
              this.CheckFolderAndCreate();
              FileInfo info = new FileInfo(fullFileName);
              try
              {
                if (!Directory.Exists(info.DirectoryName))
                  Directory.CreateDirectory(info.DirectoryName);
              }
              catch (Exception ex)
              {
                ILogNet logNet = this.LogNet;
                if (logNet == null)
                {
                  receiveGroupInfo = (OperateResult<byte, byte[]>) null;
                  groupInfo = (string[]) null;
                  receiveFileNames = (OperateResult<byte, byte[]>) null;
                  fileNames = (string[]) null;
                  opLegal = (OperateResult) null;
                  sendLegal = (OperateResult) null;
                  relativeName = (string) null;
                  return;
                }
                logNet.WriteException(this.ToString(), StringResources.Language.FilePathCreateFailed + fullFileName, ex);
                receiveGroupInfo = (OperateResult<byte, byte[]>) null;
                groupInfo = (string[]) null;
                receiveFileNames = (OperateResult<byte, byte[]>) null;
                fileNames = (string[]) null;
                opLegal = (OperateResult) null;
                sendLegal = (OperateResult) null;
                relativeName = (string) null;
                return;
              }
              DateTime dateTimeStart = DateTime.Now;
              MqttFileMonitorItem monitorItem = new MqttFileMonitorItem()
              {
                EndPoint = session.EndPoint,
                ClientId = session.ClientId,
                UserName = session.UserName,
                FileName = fileName,
                Operate = "Upload",
                Groups = EstHelper.PathCombine(groupInfo)
              };
              this.fileMonitor.Add(monitorItem);
              OperateResult<FileBaseInfo> receive = await this.ReceiveMqttFileAndUpdateGroupAsync(session, info, new Action<long, long>(monitorItem.UpdateProgress));
              this.fileMonitor.Remove(monitorItem.UniqueId);
              if (receive.IsSuccess)
              {
                MqttServer.FileChangedDelegate fileChangedEvent = this.OnFileChangedEvent;
                if (fileChangedEvent != null)
                  fileChangedEvent(session, new MqttFileOperateInfo()
                  {
                    Groups = EstHelper.PathCombine(groupInfo),
                    FileNames = fileNames,
                    Operate = "Upload",
                    TimeCost = DateTime.Now - dateTimeStart
                  });
                this.LogNet?.WriteInfo(this.ToString(), string.Format("{0} {1}:{2} Spend:{3}", (object) session, (object) StringResources.Language.FileUploadSuccess, (object) relativeName, (object) SoftBasic.GetTimeSpanDescription(DateTime.Now - dateTimeStart)));
              }
              else
                this.LogNet?.WriteError(this.ToString(), string.Format("{0} {1}:{2} Spend:{3}", (object) session, (object) StringResources.Language.FileUploadFailed, (object) relativeName, (object) SoftBasic.GetTimeSpanDescription(DateTime.Now - dateTimeStart)));
              fileName = (string) null;
              fullFileName = (string) null;
              info = (FileInfo) null;
              monitorItem = (MqttFileMonitorItem) null;
              receive = (OperateResult<FileBaseInfo>) null;
              receiveGroupInfo = (OperateResult<byte, byte[]>) null;
              groupInfo = (string[]) null;
              receiveFileNames = (OperateResult<byte, byte[]>) null;
              fileNames = (string[]) null;
              opLegal = (OperateResult) null;
              sendLegal = (OperateResult) null;
              relativeName = (string) null;
            }
            else if (receiveFileNames.Content1 == (byte) 103)
            {
              DateTime dateTimeStart = DateTime.Now;
              string[] strArray = fileNames;
              for (int index = 0; index < strArray.Length; ++index)
              {
                string item = strArray[index];
                string fullFileName = this.ReturnAbsoluteFileName(groupInfo, item);
                FileInfo info = new FileInfo(fullFileName);
                GroupFileContainer fileManagment = this.GetGroupFromFilePath(info.DirectoryName);
                this.DeleteExsistingFile(info.DirectoryName, fileManagment.DeleteFile(info.Name));
                relativeName = this.GetRelativeFileName(groupInfo, item);
                this.LogNet?.WriteInfo(this.ToString(), string.Format("{0} {1}:{2}", (object) session, (object) StringResources.Language.FileDeleteSuccess, (object) relativeName));
                fullFileName = (string) null;
                info = (FileInfo) null;
                fileManagment = (GroupFileContainer) null;
                item = (string) null;
              }
              strArray = (string[]) null;
              OperateResult operateResult = await this.SendAsync(session.MqttSocket, MqttHelper.BuildMqttCommand((byte) 103, (byte[]) null, (byte[]) null).Content);
              MqttServer.FileChangedDelegate fileChangedEvent = this.OnFileChangedEvent;
              if (fileChangedEvent == null)
              {
                receiveGroupInfo = (OperateResult<byte, byte[]>) null;
                groupInfo = (string[]) null;
                receiveFileNames = (OperateResult<byte, byte[]>) null;
                fileNames = (string[]) null;
                opLegal = (OperateResult) null;
                sendLegal = (OperateResult) null;
                relativeName = (string) null;
              }
              else
              {
                fileChangedEvent(session, new MqttFileOperateInfo()
                {
                  Groups = EstHelper.PathCombine(groupInfo),
                  FileNames = fileNames,
                  Operate = "Delete",
                  TimeCost = DateTime.Now - dateTimeStart
                });
                receiveGroupInfo = (OperateResult<byte, byte[]>) null;
                groupInfo = (string[]) null;
                receiveFileNames = (OperateResult<byte, byte[]>) null;
                fileNames = (string[]) null;
                opLegal = (OperateResult) null;
                sendLegal = (OperateResult) null;
                relativeName = (string) null;
              }
            }
            else if (receiveFileNames.Content1 == (byte) 104)
            {
              DateTime dateTimeStart = DateTime.Now;
              string fullFileName = this.ReturnAbsoluteFileName(groupInfo, "123.txt");
              FileInfo info = new FileInfo(fullFileName);
              GroupFileContainer fileManagment = this.GetGroupFromFilePath(info.DirectoryName);
              this.DeleteExsistingFile(info.DirectoryName, fileManagment.ClearAllFiles());
              OperateResult operateResult = await this.SendAsync(session.MqttSocket, MqttHelper.BuildMqttCommand((byte) 104, (byte[]) null, (byte[]) null).Content);
              MqttServer.FileChangedDelegate fileChangedEvent = this.OnFileChangedEvent;
              if (fileChangedEvent != null)
                fileChangedEvent(session, new MqttFileOperateInfo()
                {
                  Groups = EstHelper.PathCombine(groupInfo),
                  FileNames = (string[]) null,
                  Operate = "DeleteFolder",
                  TimeCost = DateTime.Now - dateTimeStart
                });
              this.LogNet?.WriteInfo(this.ToString(), session.ToString() + "FolderDelete : " + relativeName);
              fullFileName = (string) null;
              info = (FileInfo) null;
              fileManagment = (GroupFileContainer) null;
              receiveGroupInfo = (OperateResult<byte, byte[]>) null;
              groupInfo = (string[]) null;
              receiveFileNames = (OperateResult<byte, byte[]>) null;
              fileNames = (string[]) null;
              opLegal = (OperateResult) null;
              sendLegal = (OperateResult) null;
              relativeName = (string) null;
            }
            else if (receiveFileNames.Content1 == (byte) 105)
            {
              GroupFileContainer fileManagment = this.GetGroupFromFilePath(this.ReturnAbsoluteFilePath(groupInfo));
              OperateResult operateResult = await this.SendAsync(session.MqttSocket, MqttHelper.BuildMqttCommand((byte) 104, (byte[]) null, Encoding.UTF8.GetBytes(fileManagment.JsonArrayContent)).Content);
              fileManagment = (GroupFileContainer) null;
              receiveGroupInfo = (OperateResult<byte, byte[]>) null;
              groupInfo = (string[]) null;
              receiveFileNames = (OperateResult<byte, byte[]>) null;
              fileNames = (string[]) null;
              opLegal = (OperateResult) null;
              sendLegal = (OperateResult) null;
              relativeName = (string) null;
            }
            else if (receiveFileNames.Content1 == (byte) 106)
            {
              List<string> folders = new List<string>();
              string[] strArray = this.GetDirectories(groupInfo);
              for (int index = 0; index < strArray.Length; ++index)
              {
                string m = strArray[index];
                DirectoryInfo directory = new DirectoryInfo(m);
                folders.Add(directory.Name);
                directory = (DirectoryInfo) null;
                m = (string) null;
              }
              strArray = (string[]) null;
              JArray jArray = JArray.FromObject((object) folders.ToArray());
              OperateResult operateResult = await this.SendAsync(session.MqttSocket, MqttHelper.BuildMqttCommand((byte) 106, (byte[]) null, Encoding.UTF8.GetBytes(jArray.ToString())).Content);
              folders = (List<string>) null;
              jArray = (JArray) null;
              receiveGroupInfo = (OperateResult<byte, byte[]>) null;
              groupInfo = (string[]) null;
              receiveFileNames = (OperateResult<byte, byte[]>) null;
              fileNames = (string[]) null;
              opLegal = (OperateResult) null;
              sendLegal = (OperateResult) null;
              relativeName = (string) null;
            }
            else if (receiveFileNames.Content1 != (byte) 107)
            {
              receiveGroupInfo = (OperateResult<byte, byte[]>) null;
              groupInfo = (string[]) null;
              receiveFileNames = (OperateResult<byte, byte[]>) null;
              fileNames = (string[]) null;
              opLegal = (OperateResult) null;
              sendLegal = (OperateResult) null;
              relativeName = (string) null;
            }
            else
            {
              string fileName = fileNames[0];
              string fullPath = this.ReturnAbsoluteFilePath(groupInfo);
              GroupFileContainer fileManagment = this.GetGroupFromFilePath(fullPath);
              bool isExists = fileManagment.FileExists(fileName);
              OperateResult operateResult = await this.SendAsync(session.MqttSocket, MqttHelper.BuildMqttCommand(isExists ? (byte) 1 : (byte) 0, (byte[]) null, Encoding.UTF8.GetBytes(StringResources.Language.FileNotExist)).Content);
              fileName = (string) null;
              fullPath = (string) null;
              fileManagment = (GroupFileContainer) null;
              receiveGroupInfo = (OperateResult<byte, byte[]>) null;
              groupInfo = (string[]) null;
              receiveFileNames = (OperateResult<byte, byte[]>) null;
              fileNames = (string[]) null;
              opLegal = (OperateResult) null;
              sendLegal = (OperateResult) null;
              relativeName = (string) null;
            }
          }
        }
      }
    }

    /// <summary>从套接字接收文件并保存，更新文件列表</summary>
    /// <param name="session">当前的会话信息</param>
    /// <param name="info">保存的信息</param>
    /// <param name="reportProgress">当前的委托信息</param>
    /// <returns>是否成功的结果对象</returns>
    private async Task<OperateResult<FileBaseInfo>> ReceiveMqttFileAndUpdateGroupAsync(
      MqttSession session,
      FileInfo info,
      Action<long, long> reportProgress)
    {
      string guidName = SoftBasic.GetUniqueStringByGuidAndRandom();
      string fileName = Path.Combine(info.DirectoryName, guidName);
      OperateResult<FileBaseInfo> receive = await this.ReceiveMqttFileAsync(session.MqttSocket, (object) fileName, reportProgress);
      if (!receive.IsSuccess)
      {
        this.DeleteFileByName(fileName);
        return receive;
      }
      GroupFileContainer fileManagment = this.GetGroupFromFilePath(info.DirectoryName);
      string oldName = fileManagment.UpdateFileMappingName(info.Name, receive.Content.Size, guidName, session.UserName, receive.Content.Tag);
      this.DeleteExsistingFile(info.DirectoryName, oldName);
      OperateResult sendBack = await this.SendAsync(session.MqttSocket, MqttHelper.BuildMqttCommand((byte) 100, (byte[]) null, Encoding.UTF8.GetBytes(StringResources.Language.SuccessText)).Content);
      return sendBack.IsSuccess ? OperateResult.CreateSuccessResult<FileBaseInfo>(receive.Content) : OperateResult.CreateFailedResult<FileBaseInfo>(sendBack);
    }

    /// <summary>返回相对路径的名称</summary>
    /// <param name="groups">文件的分类路径信息</param>
    /// <param name="fileName">文件名</param>
    /// <returns>是否成功的结果对象</returns>
    private string GetRelativeFileName(string[] groups, string fileName)
    {
      string path1 = "";
      for (int index = 0; index < groups.Length; ++index)
      {
        if (!string.IsNullOrEmpty(groups[index]))
          path1 = Path.Combine(path1, groups[index]);
      }
      return Path.Combine(path1, fileName);
    }

    /// <summary>返回服务器的绝对路径，包含根目录的信息  [Root Dir][A][B][C]... 信息</summary>
    /// <param name="groups">文件的路径分类信息</param>
    /// <returns>是否成功的结果对象</returns>
    private string ReturnAbsoluteFilePath(string[] groups) => Path.Combine(this.filesDirectoryPath, Path.Combine(groups));

    /// <summary>
    /// 返回服务器的绝对路径，包含根目录的信息  [Root Dir][A][B][C]...[FileName] 信息
    /// </summary>
    /// <param name="groups">路径分类信息</param>
    /// <param name="fileName">文件名</param>
    /// <returns>是否成功的结果对象</returns>
    protected string ReturnAbsoluteFileName(string[] groups, string fileName) => Path.Combine(this.ReturnAbsoluteFilePath(groups), fileName);

    /// <summary>
    /// 根据文件的显示名称转化为真实存储的名称，例如 123.txt 获取到在文件服务器里映射的文件名称，例如返回 b35a11ec533147ca80c7f7d1713f015b7909
    /// </summary>
    /// <param name="groups">文件的分类信息</param>
    /// <param name="fileName">文件显示名称</param>
    /// <returns>是否成功的结果对象</returns>
    private string TransformFactFileName(string[] groups, string fileName) => this.GetGroupFromFilePath(this.ReturnAbsoluteFilePath(groups)).GetCurrentFileMappingName(fileName);

    /// <summary>
    /// 获取当前目录的文件列表管理容器，如果没有会自动创建，通过该容器可以实现对当前目录的文件进行访问<br />
    /// Get the file list management container of the current directory. If not, it will be created automatically.
    /// Through this container, you can access files in the current directory.
    /// </summary>
    /// <param name="filePath">路径信息</param>
    /// <returns>文件管理容器信息</returns>
    private GroupFileContainer GetGroupFromFilePath(string filePath)
    {
      filePath = filePath.ToUpper();
      this.group_marks_lock.Enter();
      GroupFileContainer groupFileContainer;
      if (this.m_dictionary_group_marks.ContainsKey(filePath))
      {
        groupFileContainer = this.m_dictionary_group_marks[filePath];
      }
      else
      {
        groupFileContainer = new GroupFileContainer(this.LogNet, filePath);
        this.m_dictionary_group_marks.Add(filePath, groupFileContainer);
      }
      this.group_marks_lock.Leave();
      return groupFileContainer;
    }

    /// <summary>获取文件夹的所有文件夹列表</summary>
    /// <param name="groups">分类信息</param>
    /// <returns>文件夹列表</returns>
    private string[] GetDirectories(string[] groups)
    {
      if (string.IsNullOrEmpty(this.filesDirectoryPath))
        return new string[0];
      string path = this.ReturnAbsoluteFilePath(groups);
      return !Directory.Exists(path) ? new string[0] : Directory.GetDirectories(path);
    }

    /// <summary>
    /// 获取当前文件的读写锁，如果没有会自动创建，文件名应该是guid文件名，例如 b35a11ec533147ca80c7f7d1713f015b7909<br />
    /// Acquire the read-write lock of the current file. If not, it will be created automatically.
    /// The file name should be the guid file name, for example, b35a11ec533147ca80c7f7d1713f015b7909
    /// </summary>
    /// <param name="fileName">完整的文件路径</param>
    /// <returns>返回携带文件信息的读写锁</returns>
    private FileMarkId GetFileMarksFromDictionaryWithFileName(string fileName)
    {
      FileMarkId fileMarkId;
      lock (this.dictHybirdLock)
      {
        if (this.dictionaryFilesMarks.ContainsKey(fileName))
        {
          fileMarkId = this.dictionaryFilesMarks[fileName];
        }
        else
        {
          fileMarkId = new FileMarkId(this.LogNet, fileName);
          this.dictionaryFilesMarks.Add(fileName, fileMarkId);
        }
      }
      return fileMarkId;
    }

    /// <summary>检查文件夹是否存在，不存在就创建</summary>
    private void CheckFolderAndCreate()
    {
      if (Directory.Exists(this.filesDirectoryPath))
        return;
      Directory.CreateDirectory(this.filesDirectoryPath);
    }

    /// <summary>
    /// 删除已经存在的文件信息，文件的名称需要是guid名称，例如 b35a11ec533147ca80c7f7d1713f015b7909
    /// </summary>
    /// <param name="path">文件的路径</param>
    /// <param name="fileName">文件的guid名称，例如 b35a11ec533147ca80c7f7d1713f015b7909</param>
    private void DeleteExsistingFile(string path, string fileName) => this.DeleteExsistingFile(path, new List<string>()
    {
      fileName
    });

    /// <summary>
    /// 删除已经存在的文件信息，文件的名称需要是guid名称，例如 b35a11ec533147ca80c7f7d1713f015b7909
    /// </summary>
    /// <param name="path">文件的路径</param>
    /// <param name="fileNames">文件的guid名称，例如 b35a11ec533147ca80c7f7d1713f015b7909</param>
    private void DeleteExsistingFile(string path, List<string> fileNames)
    {
      foreach (string fileName in fileNames)
      {
        if (!string.IsNullOrEmpty(fileName))
        {
          string fileUltimatePath = Path.Combine(path, fileName);
          this.GetFileMarksFromDictionaryWithFileName(fileName).AddOperation((Action) (() =>
          {
            if (!this.DeleteFileByName(fileUltimatePath))
              this.LogNet?.WriteInfo(this.ToString(), StringResources.Language.FileDeleteFailed + fileUltimatePath);
            else
              this.LogNet?.WriteInfo(this.ToString(), StringResources.Language.FileDeleteSuccess + fileUltimatePath);
          }));
        }
      }
    }

    /// <summary>
    /// 文件变化的事件，当文件上传的时候，文件下载的时候，文件被删除的时候触发。<br />
    /// The file change event is triggered when the file is uploaded, when the file is downloaded, or when the file is deleted.
    /// </summary>
    public event MqttServer.FileChangedDelegate OnFileChangedEvent;

    private void AddMqttSession(MqttSession session)
    {
      lock (this.sessionsLock)
        this.mqttSessions.Add(session);
      this.LogNet?.WriteDebug(this.ToString(), string.Format("{0} Online", (object) session));
    }

    /// <summary>让MQTT客户端正常下线，调用本方法即可自由控制会话客户端强制下线操作。</summary>
    /// <param name="session">当前的会话信息</param>
    /// <param name="reason">当前下线的原因，如果没有，代表正常下线</param>
    public void RemoveAndCloseSession(MqttSession session, string reason)
    {
      lock (this.sessionsLock)
        this.mqttSessions.Remove(session);
      session.MqttSocket?.Close();
      this.LogNet?.WriteDebug(this.ToString(), string.Format("{0} Offline {1}", (object) session, (object) reason));
      if (!(session.Protocol == "MQTT"))
        return;
      MqttServer.OnClientConnectedDelegate clientDisConnected = this.OnClientDisConnected;
      if (clientDisConnected == null)
        return;
      clientDisConnected(session);
    }

    /// <summary>
    ///  当收到客户端发来的<see cref="T:EstCommunication.MQTT.MqttClientApplicationMessage" />消息时触发<br />
    ///  Triggered when a <see cref="T:EstCommunication.MQTT.MqttClientApplicationMessage" /> message is received from the client
    /// </summary>
    public event MqttServer.OnClientApplicationMessageReceiveDelegate OnClientApplicationMessageReceive;

    /// <summary>
    /// Mqtt的客户端连接上来时触发<br />
    /// Triggered when Mqtt client connects
    /// </summary>
    public event MqttServer.OnClientConnectedDelegate OnClientConnected;

    /// <summary>
    /// Mqtt的客户端下线时触发<br />
    /// Triggered when Mqtt client connects
    /// </summary>
    public event MqttServer.OnClientConnectedDelegate OnClientDisConnected;

    /// <summary>
    /// 当客户端连接时，触发的验证事件<br />
    /// Validation event triggered when the client connects
    /// </summary>
    public event MqttServer.ClientVerificationDelegate ClientVerification;

    /// <inheritdoc cref="P:EstCommunication.Enthernet.HttpServer.LogStatistics" />
    public LogStatisticsDict LogStatistics => this.statisticsDict;

    /// <summary>
    /// 获取当前的在线的客户端数量<br />
    /// Gets the number of clients currently online
    /// </summary>
    public int OnlineCount => this.mqttSessions.Count;

    /// <summary>
    /// 获得当前所有的在线的MQTT客户端信息，包括异步的客户端及同步请求的客户端。<br />
    /// Obtain all current online MQTT client information, including asynchronous client and synchronous request client.
    /// </summary>
    public MqttSession[] OnlineSessions
    {
      get
      {
        MqttSession[] mqttSessionArray = (MqttSession[]) null;
        lock (this.sessionsLock)
          mqttSessionArray = this.mqttSessions.ToArray();
        return mqttSessionArray;
      }
    }

    /// <summary>
    /// 获得当前异步客户端在线的MQTT客户端信息。<br />
    /// Get the MQTT client information of the current asynchronous client online.
    /// </summary>
    public MqttSession[] MqttOnlineSessions
    {
      get
      {
        MqttSession[] mqttSessionArray = (MqttSession[]) null;
        lock (this.sessionsLock)
          mqttSessionArray = this.mqttSessions.Where<MqttSession>((Func<MqttSession, bool>) (m => m.Protocol == "MQTT")).ToArray<MqttSession>();
        return mqttSessionArray;
      }
    }

    /// <summary>
    /// 获得当前同步客户端在线的MQTT客户端信息，如果客户端是短连接，将难以捕获在在线信息。<br />
    /// Obtain the MQTT client information of the current synchronization client online. If the client is a short connection, it will be difficult to capture the online information. <br />
    /// </summary>
    public MqttSession[] SyncOnlineSessions
    {
      get
      {
        MqttSession[] mqttSessionArray = (MqttSession[]) null;
        lock (this.sessionsLock)
          mqttSessionArray = this.mqttSessions.Where<MqttSession>((Func<MqttSession, bool>) (m => m.Protocol == "HUSL")).ToArray<MqttSession>();
        return mqttSessionArray;
      }
    }

    /// <summary>
    /// 删除服务器里的指定主题的驻留消息。<br />
    /// Delete the resident message of the specified topic in the server.
    /// </summary>
    /// <param name="topic">等待删除的主题关键字</param>
    public void DeleteRetainTopic(string topic)
    {
      lock (this.keysLock)
      {
        if (!this.retainKeys.ContainsKey(topic))
          return;
        this.retainKeys.Remove(topic);
      }
    }

    /// <summary>
    /// 获取所有的驻留的消息的主题，如果消息发布的时候没有使用Retain属性，就无法通过本方法查到<br />
    /// Get the subject of all resident messages. If the Retain attribute is not used when the message is published, it cannot be found by this method
    /// </summary>
    /// <returns>主题的数组</returns>
    public string[] GetAllRetainTopics()
    {
      string[] strArray = (string[]) null;
      lock (this.keysLock)
        strArray = this.retainKeys.Select<KeyValuePair<string, MqttClientApplicationMessage>, string>((Func<KeyValuePair<string, MqttClientApplicationMessage>, string>) (m => m.Key)).ToArray<string>();
      return strArray;
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("MqttServer[{0}]", (object) this.Port);

    /// <summary>
    /// 当客户端进行文件操作时，校验客户端合法性的委托，操作码具体查看<seealso cref="T:EstCommunication.MQTT.MqttControlMessage" />的常量值<br />
    /// When client performing file operations, verify the legitimacy of the client, and check the constant value of <seealso cref="T:EstCommunication.MQTT.MqttControlMessage" /> for the operation code.
    /// </summary>
    /// <param name="session">会话状态</param>
    /// <param name="code">操作码</param>
    /// <param name="groups">分类信息</param>
    /// <param name="fileNames">文件名</param>
    /// <returns>是否成功</returns>
    public delegate OperateResult FileOperateVerificationDelegate(
      MqttSession session,
      byte code,
      string[] groups,
      string[] fileNames);

    /// <summary>文件变化的委托信息</summary>
    /// <param name="session">当前的会话信息，包含用户的基本信息</param>
    /// <param name="operateInfo">当前的文件操作信息，具体指示上传，下载，删除操作</param>
    public delegate void FileChangedDelegate(MqttSession session, MqttFileOperateInfo operateInfo);

    /// <summary>Mqtt的消息收到委托</summary>
    /// <param name="session">当前会话的内容</param>
    /// <param name="message">Mqtt的消息</param>
    public delegate void OnClientApplicationMessageReceiveDelegate(
      MqttSession session,
      MqttClientApplicationMessage message);

    /// <summary>当前mqtt客户端连接上服务器的事件委托</summary>
    /// <param name="session">当前的会话对象</param>
    public delegate void OnClientConnectedDelegate(MqttSession session);

    /// <summary>验证的委托</summary>
    /// <param name="mqttSession">当前的MQTT的会话内容</param>
    /// <param name="clientId">客户端的id</param>
    /// <param name="userName">用户名</param>
    /// <param name="passwrod">密码</param>
    /// <returns>0则是通过，否则，就是连接失败</returns>
    public delegate int ClientVerificationDelegate(
      MqttSession mqttSession,
      string clientId,
      string userName,
      string passwrod);
  }
}
