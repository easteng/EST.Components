// Decompiled with JetBrains decompiler
// Type: EstCommunication.Robot.ABB.ABBWebApiClient
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.Core.Net;
using EstCommunication.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EstCommunication.Robot.ABB
{
  /// <summary>
  /// ABB机器人的web api接口的客户端，可以方便快速的获取到abb机器人的一些数据信息<br />
  /// The client of ABB robot's web API interface can easily and quickly obtain some data information of ABB robot
  /// </summary>
  /// <remarks>
  /// 参考的界面信息是：http://developercenter.robotstudio.com/webservice/api_reference
  /// 
  /// 关于额外的地址说明，如果想要查看，可以调用<see cref="M:EstCommunication.Robot.ABB.ABBWebApiClient.GetSelectStrings" /> 返回字符串列表来看看。
  /// </remarks>
  public class ABBWebApiClient : NetworkWebApiBase, IRobotNet
  {
    /// <summary>
    /// 使用指定的ip地址来初始化对象<br />
    /// Initializes the object using the specified IP address
    /// </summary>
    /// <param name="ipAddress">Ip地址信息</param>
    public ABBWebApiClient(string ipAddress)
      : base(ipAddress)
    {
    }

    /// <summary>
    /// 使用指定的ip地址和端口号来初始化对象<br />
    /// Initializes the object with the specified IP address and port number
    /// </summary>
    /// <param name="ipAddress">Ip地址信息</param>
    /// <param name="port">端口号信息</param>
    public ABBWebApiClient(string ipAddress, int port)
      : base(ipAddress, port)
    {
    }

    /// <summary>
    /// 使用指定的ip地址，端口号，用户名，密码来初始化对象<br />
    /// Initialize the object with the specified IP address, port number, username, and password
    /// </summary>
    /// <param name="ipAddress">Ip地址信息</param>
    /// <param name="port">端口号信息</param>
    /// <param name="name">用户名</param>
    /// <param name="password">密码</param>
    public ABBWebApiClient(string ipAddress, int port, string name, string password)
      : base(ipAddress, port, name, password)
    {
    }

    /// <inheritdoc />
    [EstMqttApi(ApiTopic = "ReadRobotByte", Description = "Read the other side of the data information, usually designed for the GET method information.If you start with url=, you are using native address access")]
    public override OperateResult<byte[]> Read(string address) => base.Read(address);

    /// <inheritdoc />
    [EstMqttApi(ApiTopic = "ReadRobotString", Description = "The string data information that reads the other party information, usually designed for the GET method information.If you start with url=, you are using native address access")]
    public override OperateResult<string> ReadString(string address) => base.ReadString(address);

    /// <inheritdoc />
    [EstMqttApi(ApiTopic = "WriteRobotByte", Description = "Using POST to request data information from the other party, we need to start with url= to indicate that we are using native address access")]
    public override OperateResult Write(string address, byte[] value) => base.Write(address, value);

    /// <inheritdoc />
    [EstMqttApi(ApiTopic = "WriteRobotString", Description = "Using POST to request data information from the other party, we need to start with url= to indicate that we are using native address access")]
    public override OperateResult Write(string address, string value) => base.Write(address, value);

    /// <inheritdoc />
    protected override OperateResult<string> ReadByAddress(string address)
    {
      if (address.ToUpper() == "ErrorState".ToUpper())
        return this.GetErrorState();
      if (address.ToUpper() == "jointtarget".ToUpper() || address.ToUpper() == "PhysicalJoints".ToUpper())
        return this.GetJointTarget();
      if (address.ToUpper() == "SpeedRatio".ToUpper())
        return this.GetSpeedRatio();
      if (address.ToUpper() == "OperationMode".ToUpper())
        return this.GetOperationMode();
      if (address.ToUpper() == "CtrlState".ToUpper())
        return this.GetCtrlState();
      if (address.ToUpper() == "ioin".ToUpper())
        return this.GetIOIn();
      if (address.ToUpper() == "ioout".ToUpper())
        return this.GetIOOut();
      if (address.ToUpper() == "io2in".ToUpper())
        return this.GetIO2In();
      if (address.ToUpper() == "io2out".ToUpper())
        return this.GetIO2Out();
      if (address.ToUpper().StartsWith("log".ToUpper()))
      {
        int result;
        return address.Length > 3 && int.TryParse(address.Substring(3), out result) ? this.GetLog(result) : this.GetLog();
      }
      if (address.ToUpper() == "system".ToUpper())
        return this.GetSystem();
      if (address.ToUpper() == "robtarget".ToUpper())
        return this.GetRobotTarget();
      if (address.ToUpper() == "ServoEnable".ToUpper())
        return this.GetServoEnable();
      if (address.ToUpper() == "RapidExecution".ToUpper())
        return this.GetRapidExecution();
      return address.ToUpper() == "RapidTasks".ToUpper() ? this.GetRapidTasks() : base.ReadByAddress(address);
    }

    /// <inheritdoc />
    protected override async Task<OperateResult<string>> ReadByAddressAsync(
      string address)
    {
      if (address.ToUpper() == "ErrorState".ToUpper())
      {
        OperateResult<string> errorStateAsync = await this.GetErrorStateAsync();
        return errorStateAsync;
      }
      if (address.ToUpper() == "jointtarget".ToUpper())
      {
        OperateResult<string> jointTargetAsync = await this.GetJointTargetAsync();
        return jointTargetAsync;
      }
      if (address.ToUpper() == "PhysicalJoints".ToUpper())
      {
        OperateResult<string> jointTargetAsync = await this.GetJointTargetAsync();
        return jointTargetAsync;
      }
      if (address.ToUpper() == "SpeedRatio".ToUpper())
      {
        OperateResult<string> speedRatioAsync = await this.GetSpeedRatioAsync();
        return speedRatioAsync;
      }
      if (address.ToUpper() == "OperationMode".ToUpper())
      {
        OperateResult<string> operationModeAsync = await this.GetOperationModeAsync();
        return operationModeAsync;
      }
      if (address.ToUpper() == "CtrlState".ToUpper())
      {
        OperateResult<string> ctrlStateAsync = await this.GetCtrlStateAsync();
        return ctrlStateAsync;
      }
      if (address.ToUpper() == "ioin".ToUpper())
      {
        OperateResult<string> ioInAsync = await this.GetIOInAsync();
        return ioInAsync;
      }
      if (address.ToUpper() == "ioout".ToUpper())
      {
        OperateResult<string> ioOutAsync = await this.GetIOOutAsync();
        return ioOutAsync;
      }
      if (address.ToUpper() == "io2in".ToUpper())
      {
        OperateResult<string> io2InAsync = await this.GetIO2InAsync();
        return io2InAsync;
      }
      if (address.ToUpper() == "io2out".ToUpper())
      {
        OperateResult<string> io2OutAsync = await this.GetIO2OutAsync();
        return io2OutAsync;
      }
      if (address.ToUpper().StartsWith("log".ToUpper()))
      {
        int length;
        if (address.Length > 3 && int.TryParse(address.Substring(3), out length))
        {
          OperateResult<string> logAsync = await this.GetLogAsync(length);
          return logAsync;
        }
        OperateResult<string> logAsync1 = await this.GetLogAsync();
        return logAsync1;
      }
      if (address.ToUpper() == "system".ToUpper())
      {
        OperateResult<string> systemAsync = await this.GetSystemAsync();
        return systemAsync;
      }
      if (address.ToUpper() == "robtarget".ToUpper())
      {
        OperateResult<string> robotTargetAsync = await this.GetRobotTargetAsync();
        return robotTargetAsync;
      }
      if (address.ToUpper() == "ServoEnable".ToUpper())
      {
        OperateResult<string> servoEnableAsync = await this.GetServoEnableAsync();
        return servoEnableAsync;
      }
      if (address.ToUpper() == "RapidExecution".ToUpper())
      {
        OperateResult<string> rapidExecutionAsync = await this.GetRapidExecutionAsync();
        return rapidExecutionAsync;
      }
      if (address.ToUpper() == "RapidTasks".ToUpper())
      {
        OperateResult<string> rapidTasksAsync = await this.GetRapidTasksAsync();
        return rapidTasksAsync;
      }
      OperateResult<string> operateResult = await base.ReadByAddressAsync(address);
      return operateResult;
    }

    /// <summary>
    /// 获取当前支持的读取的地址列表<br />
    /// Gets a list of addresses for currently supported reads
    /// </summary>
    /// <returns>数组信息</returns>
    public static List<string> GetSelectStrings() => new List<string>()
    {
      "ErrorState",
      "jointtarget",
      "PhysicalJoints",
      "SpeedRatio",
      "OperationMode",
      "CtrlState",
      "ioin",
      "ioout",
      "io2in",
      "io2out",
      "log",
      "system",
      "robtarget",
      "ServoEnable",
      "RapidExecution",
      "RapidTasks"
    };

    private OperateResult<string> AnalysisClassAttribute(string content, string[] atts)
    {
      JObject jobject = new JObject();
      for (int index = 0; index < atts.Length; ++index)
      {
        Match match = Regex.Match(content, "<span class=\"" + atts[index] + "\">[^<]*");
        if (!match.Success)
          return new OperateResult<string>(content);
        jobject.Add(atts[index], (JToken) new JValue(match.Value.Substring(15 + atts[index].Length)));
      }
      return OperateResult.CreateSuccessResult<string>(jobject.ToString());
    }

    private OperateResult<string> AnalysisSystem(string content) => this.AnalysisClassAttribute(content, new string[11]
    {
      "major",
      "minor",
      "build",
      "title",
      "type",
      "description",
      "date",
      "mctimestamp",
      "name",
      "sysid",
      "starttm"
    });

    private OperateResult<string> AnalysisRobotTarget(string content) => this.AnalysisClassAttribute(content, new string[6]
    {
      "x",
      "y",
      "z",
      "q1",
      "q2",
      "q3"
    });

    /// <summary>
    /// 获取当前的控制状态，Content属性就是机器人的控制信息<br />
    /// Get the current control state. The Content attribute is the control information of the robot
    /// </summary>
    /// <returns>带有状态信息的结果类对象</returns>
    [EstMqttApi(Description = "Get the current control state. The Content attribute is the control information of the robot")]
    public OperateResult<string> GetCtrlState()
    {
      OperateResult<string> operateResult = this.ReadString("url=/rw/panel/ctrlstate");
      if (!operateResult.IsSuccess)
        return operateResult;
      Match match = Regex.Match(operateResult.Content, "<span class=\"ctrlstate\">[^<]+");
      return !match.Success ? new OperateResult<string>(operateResult.Content) : OperateResult.CreateSuccessResult<string>(match.Value.Substring(24));
    }

    /// <summary>
    /// 获取当前的错误状态，Content属性就是机器人的状态信息<br />
    /// Gets the current error state. The Content attribute is the state information of the robot
    /// </summary>
    /// <returns>带有状态信息的结果类对象</returns>
    [EstMqttApi(Description = "Gets the current error state. The Content attribute is the state information of the robot")]
    public OperateResult<string> GetErrorState()
    {
      OperateResult<string> operateResult = this.ReadString("url=/rw/motionsystem/errorstate");
      if (!operateResult.IsSuccess)
        return operateResult;
      Match match = Regex.Match(operateResult.Content, "<span class=\"err-state\">[^<]+");
      return !match.Success ? new OperateResult<string>(operateResult.Content) : OperateResult.CreateSuccessResult<string>(match.Value.Substring(24));
    }

    /// <summary>
    /// 获取当前机器人的物理关节点信息，返回json格式的关节信息<br />
    /// Get the physical node information of the current robot and return the joint information in json format
    /// </summary>
    /// <returns>带有关节信息的结果类对象</returns>
    [EstMqttApi(Description = "Get the physical node information of the current robot and return the joint information in json format")]
    public OperateResult<string> GetJointTarget()
    {
      OperateResult<string> operateResult = this.ReadString("url=/rw/motionsystem/mechunits/ROB_1/jointtarget");
      if (!operateResult.IsSuccess)
        return operateResult;
      MatchCollection matchCollection = Regex.Matches(operateResult.Content, "<span class=\"rax[^<]*");
      if (matchCollection.Count != 6)
        return new OperateResult<string>(operateResult.Content);
      double[] numArray = new double[6];
      for (int i = 0; i < matchCollection.Count; ++i)
      {
        if (matchCollection[i].Length > 17)
          numArray[i] = double.Parse(matchCollection[i].Value.Substring(20));
      }
      return OperateResult.CreateSuccessResult<string>(JArray.FromObject((object) numArray).ToString(Formatting.None));
    }

    /// <summary>
    /// 获取当前机器人的速度配比信息<br />
    /// Get the speed matching information of the current robot
    /// </summary>
    /// <returns>带有速度信息的结果类对象</returns>
    [EstMqttApi(Description = "Get the speed matching information of the current robot")]
    public OperateResult<string> GetSpeedRatio()
    {
      OperateResult<string> operateResult = this.ReadString("url=/rw/panel/speedratio");
      if (!operateResult.IsSuccess)
        return operateResult;
      Match match = Regex.Match(operateResult.Content, "<span class=\"speedratio\">[^<]*");
      return !match.Success ? new OperateResult<string>(operateResult.Content) : OperateResult.CreateSuccessResult<string>(match.Value.Substring(25));
    }

    /// <summary>
    /// 获取当前机器人的工作模式<br />
    /// Gets the current working mode of the robot
    /// </summary>
    /// <returns>带有工作模式信息的结果类对象</returns>
    [EstMqttApi(Description = "Gets the current working mode of the robot")]
    public OperateResult<string> GetOperationMode()
    {
      OperateResult<string> operateResult = this.ReadString("url=/rw/panel/opmode");
      if (!operateResult.IsSuccess)
        return operateResult;
      Match match = Regex.Match(operateResult.Content, "<span class=\"opmode\">[^<]*");
      return !match.Success ? new OperateResult<string>(operateResult.Content) : OperateResult.CreateSuccessResult<string>(match.Value.Substring(21));
    }

    /// <summary>
    /// 获取当前机器人的本机的输入IO<br />
    /// Gets the input IO of the current robot's native
    /// </summary>
    /// <returns>带有IO信息的结果类对象</returns>
    [EstMqttApi(Description = "Gets the input IO of the current robot's native")]
    public OperateResult<string> GetIOIn()
    {
      OperateResult<string> operateResult = this.ReadString("url=/rw/iosystem/devices/D652_10");
      if (!operateResult.IsSuccess)
        return operateResult;
      Match match = Regex.Match(operateResult.Content, "<span class=\"indata\">[^<]*");
      return !match.Success ? new OperateResult<string>(operateResult.Content) : OperateResult.CreateSuccessResult<string>(match.Value.Substring(21));
    }

    /// <summary>
    /// 获取当前机器人的本机的输出IO<br />
    /// Gets the output IO of the current robot's native
    /// </summary>
    /// <returns>带有IO信息的结果类对象</returns>
    [EstMqttApi(Description = "Gets the output IO of the current robot's native")]
    public OperateResult<string> GetIOOut()
    {
      OperateResult<string> operateResult = this.ReadString("url=/rw/iosystem/devices/D652_10");
      if (!operateResult.IsSuccess)
        return operateResult;
      Match match = Regex.Match(operateResult.Content, "<span class=\"outdata\">[^<]*");
      return !match.Success ? new OperateResult<string>(operateResult.Content) : OperateResult.CreateSuccessResult<string>(match.Value.Substring(22));
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.ABB.ABBWebApiClient.GetIOIn" />
    [EstMqttApi(Description = "Gets the input IO2 of the current robot's native")]
    public OperateResult<string> GetIO2In()
    {
      OperateResult<string> operateResult = this.ReadString("url=/rw/iosystem/devices/BK5250");
      if (!operateResult.IsSuccess)
        return operateResult;
      Match match = Regex.Match(operateResult.Content, "<span class=\"indata\">[^<]*");
      return !match.Success ? new OperateResult<string>(operateResult.Content) : OperateResult.CreateSuccessResult<string>(match.Value.Substring(21));
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.ABB.ABBWebApiClient.GetIOOut" />
    [EstMqttApi(Description = "Gets the output IO2 of the current robot's native")]
    public OperateResult<string> GetIO2Out()
    {
      OperateResult<string> operateResult = this.ReadString("url=/rw/iosystem/devices/BK5250");
      if (!operateResult.IsSuccess)
        return operateResult;
      Match match = Regex.Match(operateResult.Content, "<span class=\"outdata\">[^<]*");
      return !match.Success ? new OperateResult<string>(operateResult.Content) : OperateResult.CreateSuccessResult<string>(match.Value.Substring(22));
    }

    /// <summary>
    /// 获取当前机器人的日志记录，默认记录为10条<br />
    /// Gets the log record for the current robot, which is 10 by default
    /// </summary>
    /// <param name="logCount">读取的最大的日志总数</param>
    /// <returns>带有IO信息的结果类对象</returns>
    [EstMqttApi(Description = "Gets the log record for the current robot, which is 10 by default")]
    public OperateResult<string> GetLog(int logCount = 10)
    {
      OperateResult<string> operateResult = this.ReadString("url=/rw/elog/0?lang=zh&amp;resource=title");
      if (!operateResult.IsSuccess)
        return operateResult;
      MatchCollection matchCollection = Regex.Matches(operateResult.Content, "<li class=\"elog-message-li\" title=\"/rw/elog/0/[0-9]+\">[\\S\\s]+?</li>");
      JArray jarray = new JArray();
      for (int i = 0; i < matchCollection.Count && i < logCount; ++i)
      {
        Match match = Regex.Match(matchCollection[i].Value, "[0-9]+\"");
        JObject jobject = new JObject();
        jobject["id"] = (JToken) match.Value.TrimEnd('"');
        foreach (XElement element in XElement.Parse(matchCollection[i].Value).Elements((XName) "span"))
          jobject[element.Attribute((XName) "class").Value] = (JToken) element.Value;
        jarray.Add((JToken) jobject);
      }
      return OperateResult.CreateSuccessResult<string>(jarray.ToString());
    }

    /// <summary>
    /// 获取当前机器人的系统信息，版本号，唯一ID等信息<br />
    /// Get the current robot's system information, version number, unique ID and other information
    /// </summary>
    /// <returns>系统的基本信息</returns>
    [EstMqttApi(Description = "Get the current robot's system information, version number, unique ID and other information")]
    public OperateResult<string> GetSystem()
    {
      OperateResult<string> operateResult = this.ReadString("url=/rw/system");
      return !operateResult.IsSuccess ? operateResult : this.AnalysisSystem(operateResult.Content);
    }

    /// <summary>
    /// 获取当前机器人的系统信息，版本号，唯一ID等信息<br />
    /// Get the current robot's system information, version number, unique ID and other information
    /// </summary>
    /// <returns>系统的基本信息</returns>
    [EstMqttApi(Description = "Get the current robot's system information, version number, unique ID and other information")]
    public OperateResult<string> GetRobotTarget()
    {
      OperateResult<string> operateResult = this.ReadString("url=/rw/motionsystem/mechunits/ROB_1/robtarget");
      return !operateResult.IsSuccess ? operateResult : this.AnalysisRobotTarget(operateResult.Content);
    }

    /// <summary>
    /// 获取当前机器人的伺服使能状态<br />
    /// Get the current robot servo enable state
    /// </summary>
    /// <returns>机器人的伺服使能状态</returns>
    [EstMqttApi(Description = "Get the current robot servo enable state")]
    public OperateResult<string> GetServoEnable()
    {
      OperateResult<string> operateResult = this.ReadString("url=/rw/iosystem/signals/Local/DRV_1/DRV1K1");
      if (!operateResult.IsSuccess)
        return operateResult;
      Match match = Regex.Match(operateResult.Content, "<li class=\"ios-signal\"[\\S\\s]+?</li>");
      if (!match.Success)
        return new OperateResult<string>(operateResult.Content);
      JObject jobject = new JObject();
      foreach (XElement element in XElement.Parse(match.Value).Elements((XName) "span"))
        jobject[element.Attribute((XName) "class").Value] = (JToken) element.Value;
      return OperateResult.CreateSuccessResult<string>(jobject.ToString());
    }

    /// <summary>
    /// 获取当前机器人的当前程序运行状态<br />
    /// Get the current program running status of the current robot
    /// </summary>
    /// <returns>机器人的当前的程序运行状态</returns>
    [EstMqttApi(Description = "Get the current program running status of the current robot")]
    public OperateResult<string> GetRapidExecution()
    {
      OperateResult<string> operateResult = this.ReadString("url=/rw/rapid/execution");
      if (!operateResult.IsSuccess)
        return operateResult;
      Match match = Regex.Match(operateResult.Content, "<li class=\"rap-execution\"[\\S\\s]+?</li>");
      if (!match.Success)
        return new OperateResult<string>(operateResult.Content);
      JObject jobject = new JObject();
      foreach (XElement element in XElement.Parse(match.Value).Elements((XName) "span"))
        jobject[element.Attribute((XName) "class").Value] = (JToken) element.Value;
      return OperateResult.CreateSuccessResult<string>(jobject.ToString());
    }

    /// <summary>
    /// 获取当前机器人的任务列表<br />
    /// Get the task list of the current robot
    /// </summary>
    /// <returns>任务信息的列表</returns>
    [EstMqttApi(Description = "Get the task list of the current robot")]
    public OperateResult<string> GetRapidTasks()
    {
      OperateResult<string> operateResult = this.ReadString("url=/rw/rapid/tasks");
      if (!operateResult.IsSuccess)
        return operateResult;
      MatchCollection matchCollection = Regex.Matches(operateResult.Content, "<li class=\"rap-task-li\" [\\S\\s]+?</li>");
      JArray jarray = new JArray();
      for (int i = 0; i < matchCollection.Count; ++i)
      {
        JObject jobject = new JObject();
        foreach (XElement element in XElement.Parse(matchCollection[i].Value).Elements((XName) "span"))
          jobject[element.Attribute((XName) "class").Value] = (JToken) element.Value;
        jarray.Add((JToken) jobject);
      }
      return OperateResult.CreateSuccessResult<string>(jarray.ToString());
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.ABB.ABBWebApiClient.GetCtrlState" />
    public async Task<OperateResult<string>> GetCtrlStateAsync()
    {
      OperateResult<string> read = await this.ReadStringAsync("url=/rw/panel/ctrlstate");
      if (!read.IsSuccess)
        return read;
      Match match = Regex.Match(read.Content, "<span class=\"ctrlstate\">[^<]+");
      return match.Success ? OperateResult.CreateSuccessResult<string>(match.Value.Substring(24)) : new OperateResult<string>(read.Content);
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.ABB.ABBWebApiClient.GetErrorState" />
    public async Task<OperateResult<string>> GetErrorStateAsync()
    {
      OperateResult<string> read = await this.ReadStringAsync("url=/rw/motionsystem/errorstate");
      if (!read.IsSuccess)
        return read;
      Match match = Regex.Match(read.Content, "<span class=\"err-state\">[^<]+");
      return match.Success ? OperateResult.CreateSuccessResult<string>(match.Value.Substring(24)) : new OperateResult<string>(read.Content);
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.ABB.ABBWebApiClient.GetJointTarget" />
    public async Task<OperateResult<string>> GetJointTargetAsync()
    {
      OperateResult<string> read = await this.ReadStringAsync("url=/rw/motionsystem/mechunits/ROB_1/jointtarget");
      if (!read.IsSuccess)
        return read;
      MatchCollection mc = Regex.Matches(read.Content, "<span class=\"rax[^<]*");
      if (mc.Count != 6)
        return new OperateResult<string>(read.Content);
      double[] joints = new double[6];
      for (int i = 0; i < mc.Count; ++i)
      {
        if (mc[i].Length > 17)
          joints[i] = double.Parse(mc[i].Value.Substring(20));
      }
      return OperateResult.CreateSuccessResult<string>(JArray.FromObject((object) joints).ToString(Formatting.None));
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.ABB.ABBWebApiClient.GetSpeedRatio" />
    public async Task<OperateResult<string>> GetSpeedRatioAsync()
    {
      OperateResult<string> read = await this.ReadStringAsync("url=/rw/panel/speedratio");
      if (!read.IsSuccess)
        return read;
      Match match = Regex.Match(read.Content, "<span class=\"speedratio\">[^<]*");
      return match.Success ? OperateResult.CreateSuccessResult<string>(match.Value.Substring(25)) : new OperateResult<string>(read.Content);
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.ABB.ABBWebApiClient.GetOperationMode" />
    public async Task<OperateResult<string>> GetOperationModeAsync()
    {
      OperateResult<string> read = await this.ReadStringAsync("url=/rw/panel/opmode");
      if (!read.IsSuccess)
        return read;
      Match match = Regex.Match(read.Content, "<span class=\"opmode\">[^<]*");
      return match.Success ? OperateResult.CreateSuccessResult<string>(match.Value.Substring(21)) : new OperateResult<string>(read.Content);
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.ABB.ABBWebApiClient.GetIOIn" />
    public async Task<OperateResult<string>> GetIOInAsync()
    {
      OperateResult<string> read = await this.ReadStringAsync("url=/rw/iosystem/devices/D652_10");
      if (!read.IsSuccess)
        return read;
      Match match = Regex.Match(read.Content, "<span class=\"indata\">[^<]*");
      return match.Success ? OperateResult.CreateSuccessResult<string>(match.Value.Substring(21)) : new OperateResult<string>(read.Content);
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.ABB.ABBWebApiClient.GetIOOut" />
    public async Task<OperateResult<string>> GetIOOutAsync()
    {
      OperateResult<string> read = await this.ReadStringAsync("url=/rw/iosystem/devices/D652_10");
      if (!read.IsSuccess)
        return read;
      Match match = Regex.Match(read.Content, "<span class=\"outdata\">[^<]*");
      return match.Success ? OperateResult.CreateSuccessResult<string>(match.Value.Substring(22)) : new OperateResult<string>(read.Content);
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.ABB.ABBWebApiClient.GetIOIn" />
    public async Task<OperateResult<string>> GetIO2InAsync()
    {
      OperateResult<string> read = await this.ReadStringAsync("url=/rw/iosystem/devices/BK5250");
      if (!read.IsSuccess)
        return read;
      Match match = Regex.Match(read.Content, "<span class=\"indata\">[^<]*");
      return match.Success ? OperateResult.CreateSuccessResult<string>(match.Value.Substring(21)) : new OperateResult<string>(read.Content);
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.ABB.ABBWebApiClient.GetIOOut" />
    public async Task<OperateResult<string>> GetIO2OutAsync()
    {
      OperateResult<string> read = await this.ReadStringAsync("url=/rw/iosystem/devices/BK5250");
      if (!read.IsSuccess)
        return read;
      Match match = Regex.Match(read.Content, "<span class=\"outdata\">[^<]*");
      return match.Success ? OperateResult.CreateSuccessResult<string>(match.Value.Substring(22)) : new OperateResult<string>(read.Content);
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.ABB.ABBWebApiClient.GetLog(System.Int32)" />
    public async Task<OperateResult<string>> GetLogAsync(int logCount = 10)
    {
      OperateResult<string> read = await this.ReadStringAsync("url=/rw/elog/0?lang=zh&amp;resource=title");
      if (!read.IsSuccess)
        return read;
      MatchCollection matchs = Regex.Matches(read.Content, "<li class=\"elog-message-li\" title=\"/rw/elog/0/[0-9]+\">[\\S\\s]+?</li>");
      JArray jArray = new JArray();
      for (int i = 0; i < matchs.Count && i < logCount; ++i)
      {
        Match id = Regex.Match(matchs[i].Value, "[0-9]+\"");
        JObject json = new JObject();
        json["id"] = (JToken) id.Value.TrimEnd('"');
        foreach (XElement element in XElement.Parse(matchs[i].Value).Elements((XName) "span"))
        {
          XElement item = element;
          json[item.Attribute((XName) "class").Value] = (JToken) item.Value;
          item = (XElement) null;
        }
        jArray.Add((JToken) json);
        id = (Match) null;
        json = (JObject) null;
      }
      return OperateResult.CreateSuccessResult<string>(jArray.ToString());
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.ABB.ABBWebApiClient.GetSystem" />
    public async Task<OperateResult<string>> GetSystemAsync()
    {
      OperateResult<string> read = await this.ReadStringAsync("url=/rw/system");
      OperateResult<string> operateResult = read.IsSuccess ? this.AnalysisSystem(read.Content) : read;
      read = (OperateResult<string>) null;
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.ABB.ABBWebApiClient.GetRobotTarget" />
    public async Task<OperateResult<string>> GetRobotTargetAsync()
    {
      OperateResult<string> read = await this.ReadStringAsync("url=/rw/motionsystem/mechunits/ROB_1/robtarget");
      OperateResult<string> operateResult = read.IsSuccess ? this.AnalysisRobotTarget(read.Content) : read;
      read = (OperateResult<string>) null;
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.ABB.ABBWebApiClient.GetServoEnable" />
    public async Task<OperateResult<string>> GetServoEnableAsync()
    {
      OperateResult<string> read = await this.ReadStringAsync("url=/rw/iosystem/signals/Local/DRV_1/DRV1K1");
      if (!read.IsSuccess)
        return read;
      Match match = Regex.Match(read.Content, "<li class=\"ios-signal\"[\\S\\s]+?</li>");
      if (!match.Success)
        return new OperateResult<string>(read.Content);
      JObject json = new JObject();
      foreach (XElement element in XElement.Parse(match.Value).Elements((XName) "span"))
      {
        XElement item = element;
        json[item.Attribute((XName) "class").Value] = (JToken) item.Value;
        item = (XElement) null;
      }
      return OperateResult.CreateSuccessResult<string>(json.ToString());
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.ABB.ABBWebApiClient.GetRapidExecution" />
    public async Task<OperateResult<string>> GetRapidExecutionAsync()
    {
      OperateResult<string> read = await this.ReadStringAsync("url=/rw/rapid/execution");
      if (!read.IsSuccess)
        return read;
      Match match = Regex.Match(read.Content, "<li class=\"rap-execution\"[\\S\\s]+?</li>");
      if (!match.Success)
        return new OperateResult<string>(read.Content);
      JObject json = new JObject();
      foreach (XElement element in XElement.Parse(match.Value).Elements((XName) "span"))
      {
        XElement item = element;
        json[item.Attribute((XName) "class").Value] = (JToken) item.Value;
        item = (XElement) null;
      }
      return OperateResult.CreateSuccessResult<string>(json.ToString());
    }

    /// <inheritdoc cref="M:EstCommunication.Robot.ABB.ABBWebApiClient.GetRapidTasks" />
    public async Task<OperateResult<string>> GetRapidTasksAsync()
    {
      OperateResult<string> read = await this.ReadStringAsync("url=/rw/rapid/tasks");
      if (!read.IsSuccess)
        return read;
      MatchCollection matchs = Regex.Matches(read.Content, "<li class=\"rap-task-li\" [\\S\\s]+?</li>");
      JArray jArray = new JArray();
      for (int i = 0; i < matchs.Count; ++i)
      {
        JObject json = new JObject();
        foreach (XElement element in XElement.Parse(matchs[i].Value).Elements((XName) "span"))
        {
          XElement item = element;
          json[item.Attribute((XName) "class").Value] = (JToken) item.Value;
          item = (XElement) null;
        }
        jArray.Add((JToken) json);
        json = (JObject) null;
      }
      return OperateResult.CreateSuccessResult<string>(jArray.ToString());
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("ABBWebApiClient[{0}:{1}]", (object) this.IpAddress, (object) this.Port);
  }
}
