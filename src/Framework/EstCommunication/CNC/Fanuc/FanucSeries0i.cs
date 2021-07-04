// Decompiled with JetBrains decompiler
// Type: EstCommunication.CNC.Fanuc.FanucSeries0i
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using EstCommunication.Core;
using EstCommunication.Core.IMessage;
using EstCommunication.Core.Net;
using EstCommunication.Reflection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EstCommunication.CNC.Fanuc
{
  /// <summary>一个FANUC的机床通信类对象</summary>
  public class FanucSeries0i : NetworkDoubleBase
  {
    private Encoding encoding;

    /// <summary>根据IP及端口来实例化一个对象内容</summary>
    /// <param name="ipAddress">Ip地址信息</param>
    /// <param name="port">端口号</param>
    public FanucSeries0i(string ipAddress, int port = 8193)
    {
      this.IpAddress = ipAddress;
      this.Port = port;
      this.ByteTransform = (IByteTransform) new ReverseBytesTransform();
      this.encoding = Encoding.Default;
    }

    /// <inheritdoc />
    protected override INetMessage GetNewNetMessage() => (INetMessage) new CNCFanucSeriesMessage();

    /// <summary>
    /// 获取或设置当前的文本的字符编码信息，如果你不清楚，可以调用<see cref="M:EstCommunication.CNC.Fanuc.FanucSeries0i.ReadLanguage" />方法来自动匹配。<br />
    /// Get or set the character encoding information of the current text.
    /// If you are not sure, you can call the <see cref="M:EstCommunication.CNC.Fanuc.FanucSeries0i.ReadLanguage" /> method to automatically match.
    /// </summary>
    public Encoding TextEncoding
    {
      get => this.encoding;
      set => this.encoding = value;
    }

    /// <inheritdoc />
    protected override OperateResult InitializationOnConnect(Socket socket)
    {
      OperateResult<byte[]> operateResult1 = this.ReadFromCoreServer(socket, "a0 a0 a0 a0 00 01 01 01 00 02 00 02".ToHexBytes());
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(socket, "a0 a0 a0 a0 00 01 21 01 00 1e 00 01 00 1c 00 01 00 01 00 18 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00".ToHexBytes());
      return !operateResult2.IsSuccess ? (OperateResult) operateResult2 : OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc />
    protected override OperateResult ExtraOnDisconnect(Socket socket) => (OperateResult) this.ReadFromCoreServer(socket, "a0 a0 a0 a0 00 01 02 01 00 00".ToHexBytes());

    /// <inheritdoc />
    protected override async Task<OperateResult> InitializationOnConnectAsync(
      Socket socket)
    {
      OperateResult<byte[]> read1 = await this.ReadFromCoreServerAsync(socket, "a0 a0 a0 a0 00 01 01 01 00 02 00 02".ToHexBytes());
      if (!read1.IsSuccess)
        return (OperateResult) read1;
      OperateResult<byte[]> read2 = await this.ReadFromCoreServerAsync(socket, "a0 a0 a0 a0 00 01 21 01 00 1e 00 01 00 1c 00 01 00 01 00 18 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00".ToHexBytes());
      return read2.IsSuccess ? OperateResult.CreateSuccessResult() : (OperateResult) read2;
    }

    /// <inheritdoc />
    protected override async Task<OperateResult> ExtraOnDisconnectAsync(
      Socket socket)
    {
      OperateResult<byte[]> operateResult = await this.ReadFromCoreServerAsync(socket, "a0 a0 a0 a0 00 01 02 01 00 00".ToHexBytes());
      return (OperateResult) operateResult;
    }

    private double GetFanucDouble(byte[] content, int index) => this.GetFanucDouble(content, index, 1)[0];

    private double[] GetFanucDouble(byte[] content, int index, int length)
    {
      double[] numArray = new double[length];
      for (int index1 = 0; index1 < length; ++index1)
      {
        int num = this.ByteTransform.TransInt32(content, index + 8 * index1);
        int digits = (int) this.ByteTransform.TransInt16(content, index + 8 * index1 + 6);
        numArray[index1] = num != 0 ? Math.Round((double) num * Math.Pow(0.1, (double) digits), digits) : 0.0;
      }
      return numArray;
    }

    private byte[] CreateFromFanucDouble(double value)
    {
      byte[] numArray = new byte[8];
      this.ByteTransform.TransByte((int) (value * 1000.0)).CopyTo((Array) numArray, 0);
      numArray[5] = (byte) 10;
      numArray[7] = (byte) 3;
      return numArray;
    }

    private void ChangeTextEncoding(ushort code)
    {
      switch (code)
      {
        case 0:
          this.encoding = Encoding.Default;
          break;
        case 1:
        case 4:
          this.encoding = Encoding.GetEncoding("shift_jis", EncoderFallback.ReplacementFallback, (DecoderFallback) new DecoderReplacementFallback());
          break;
        case 6:
          this.encoding = Encoding.GetEncoding("ks_c_5601-1987");
          break;
        case 15:
          this.encoding = Encoding.Default;
          break;
        case 16:
          this.encoding = Encoding.GetEncoding("windows-1251");
          break;
        case 17:
          this.encoding = Encoding.GetEncoding("windows-1254");
          break;
      }
    }

    /// <summary>
    /// 主轴转速及进给倍率<br />
    /// Spindle speed and feedrate override
    /// </summary>
    /// <returns>主轴转速及进给倍率</returns>
    [EstMqttApi(Description = "Spindle speed and feedrate override")]
    public OperateResult<double, double> ReadSpindleSpeedAndFeedRate()
    {
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(this.BuildReadArray(this.BuildReadSingle((ushort) 164, 3, 0, 0, 0, 0), this.BuildReadSingle((ushort) 138, 1, 0, 0, 0, 0), this.BuildReadSingle((ushort) 136, 3, 0, 0, 0, 0), this.BuildReadSingle((ushort) 136, 4, 0, 0, 0, 0), this.BuildReadSingle((ushort) 36, 0, 0, 0, 0, 0), this.BuildReadSingle((ushort) 37, 0, 0, 0, 0, 0), this.BuildReadSingle((ushort) 164, 3, 0, 0, 0, 0)));
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<double, double>((OperateResult) operateResult);
      List<byte[]> numArrayList = this.ExtraContentArray(operateResult.Content.RemoveBegin<byte>(10));
      return OperateResult.CreateSuccessResult<double, double>(this.GetFanucDouble(numArrayList[5], 14), this.GetFanucDouble(numArrayList[4], 14));
    }

    /// <summary>
    /// 读取程序名及程序号<br />
    /// Read program name and program number
    /// </summary>
    /// <returns>程序名及程序号</returns>
    [EstMqttApi(Description = "Read program name and program number")]
    public OperateResult<string, int> ReadSystemProgramCurrent()
    {
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(this.BuildReadArray(this.BuildReadSingle((ushort) 207, 0, 0, 0, 0, 0)));
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<string, int>((OperateResult) operateResult);
      List<byte[]> numArrayList = this.ExtraContentArray(operateResult.Content.RemoveBegin<byte>(10));
      int num = this.ByteTransform.TransInt32(numArrayList[0], 14);
      return OperateResult.CreateSuccessResult<string, int>(this.encoding.GetString(numArrayList[0].SelectMiddle<byte>(18, 36)).TrimEnd(new char[1]), num);
    }

    /// <summary>
    /// 读取机床的语言设定信息，具体值的含义参照API文档说明<br />
    /// Read the language setting information of the machine tool, refer to the API documentation for the meaning of the specific values
    /// </summary>
    /// <remarks>此处举几个常用值 0: 英语 1: 日语 2: 德语 3: 法语 4: 中文繁体 6: 韩语 15: 中文简体 16: 俄语 17: 土耳其语</remarks>
    /// <returns>返回的语言代号</returns>
    [EstMqttApi(Description = "Read the language setting information of the machine tool")]
    public OperateResult<ushort> ReadLanguage()
    {
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(this.BuildReadArray(this.BuildReadSingle((ushort) 141, 3281, 3281, 0, 0, 0)));
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<ushort>((OperateResult) operateResult);
      ushort code = this.ByteTransform.TransUInt16(this.ExtraContentArray(operateResult.Content.RemoveBegin<byte>(10))[0], 24);
      this.ChangeTextEncoding(code);
      return OperateResult.CreateSuccessResult<ushort>(code);
    }

    /// <summary>
    /// 读取宏变量，可以用来读取刀具号<br />
    /// Read macro variable, can be used to read tool number
    /// </summary>
    /// <param name="number">刀具号</param>
    /// <returns>读宏变量信息</returns>
    [EstMqttApi(Description = "Read macro variable, can be used to read tool number")]
    public OperateResult<double> ReadSystemMacroValue(int number) => ByteTransformHelper.GetResultFromArray<double>(this.ReadSystemMacroValue(number, 1));

    /// <summary>
    /// 读取宏变量，可以用来读取刀具号<br />
    /// Read macro variable, can be used to read tool number
    /// </summary>
    /// <param name="number">宏变量地址</param>
    /// <param name="length">读取的长度信息</param>
    /// <returns>是否成功</returns>
    [EstMqttApi(ApiTopic = "ReadSystemMacroValueArray", Description = "Read macro variable, can be used to read tool number")]
    public OperateResult<double[]> ReadSystemMacroValue(int number, int length)
    {
      int[] array = SoftBasic.SplitIntegerToArray(length, 5);
      int a = number;
      List<byte> byteList = new List<byte>();
      for (int index = 0; index < array.Length; ++index)
      {
        OperateResult<byte[]> operateResult = this.ReadFromCoreServer(this.BuildReadArray(this.BuildReadSingle((ushort) 21, a, a + array[index] - 1, 0, 0, 0)));
        if (!operateResult.IsSuccess)
          return OperateResult.CreateFailedResult<double[]>((OperateResult) operateResult);
        byteList.AddRange((IEnumerable<byte>) this.ExtraContentArray(operateResult.Content.RemoveBegin<byte>(10))[0].RemoveBegin<byte>(14));
        a += array[index];
      }
      try
      {
        return OperateResult.CreateSuccessResult<double[]>(this.GetFanucDouble(byteList.ToArray(), 0, length));
      }
      catch (Exception ex)
      {
        return new OperateResult<double[]>(ex.Message + " Source:" + byteList.ToArray().ToHexString(' '));
      }
    }

    /// <summary>
    /// 写宏变量，需要指定地址及写入的数据<br />
    /// Write macro variable, need to specify the address and write data
    /// </summary>
    /// <param name="number">地址</param>
    /// <param name="values">数据值</param>
    /// <returns>是否成功</returns>
    [EstMqttApi(Description = "Write macro variable, need to specify the address and write data")]
    public OperateResult WriteSystemMacroValue(int number, double[] values)
    {
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(this.BuildReadArray(this.BuildWriteSingle((ushort) 22, number, number + values.Length - 1, 0, 0, values)));
      if (!operateResult.IsSuccess)
        return (OperateResult) OperateResult.CreateFailedResult<string, int>((OperateResult) operateResult);
      List<byte[]> numArrayList = this.ExtraContentArray(operateResult.Content.RemoveBegin<byte>(10));
      return this.ByteTransform.TransUInt16(numArrayList[0], 6) == (ushort) 0 ? OperateResult.CreateSuccessResult() : new OperateResult((int) this.ByteTransform.TransUInt16(numArrayList[0], 6), "Unknown Error");
    }

    /// <summary>
    /// 根据刀具号写入长度形状补偿，刀具号为1-24<br />
    /// Write length shape compensation according to the tool number, the tool number is 1-24
    /// </summary>
    /// <param name="cutter">刀具号，范围为1-24</param>
    /// <param name="offset">补偿值</param>
    /// <returns>是否写入成功</returns>
    [EstMqttApi(Description = "Write length shape compensation according to the tool number, the tool number is 1-24")]
    public OperateResult WriteCutterLengthShapeOffset(int cutter, double offset) => this.WriteSystemMacroValue(11000 + cutter, new double[1]
    {
      offset
    });

    /// <summary>
    /// 根据刀具号写入长度磨损补偿，刀具号为1-24<br />
    /// Write length wear compensation according to the tool number, the tool number is 1-24
    /// </summary>
    /// <param name="cutter">刀具号，范围为1-24</param>
    /// <param name="offset">补偿值</param>
    /// <returns>是否写入成功</returns>
    [EstMqttApi(Description = "Write length wear compensation according to the tool number, the tool number is 1-24")]
    public OperateResult WriteCutterLengthWearOffset(int cutter, double offset) => this.WriteSystemMacroValue(10000 + cutter, new double[1]
    {
      offset
    });

    /// <summary>
    /// 根据刀具号写入半径形状补偿，刀具号为1-24<br />
    /// Write radius shape compensation according to the tool number, the tool number is 1-24
    /// </summary>
    /// <param name="cutter">刀具号，范围为1-24</param>
    /// <param name="offset">补偿值</param>
    /// <returns>是否写入成功</returns>
    [EstMqttApi(Description = "Write radius shape compensation according to the tool number, the tool number is 1-24")]
    public OperateResult WriteCutterRadiusShapeOffset(int cutter, double offset) => this.WriteSystemMacroValue(13000 + cutter, new double[1]
    {
      offset
    });

    /// <summary>
    /// 根据刀具号写入半径磨损补偿，刀具号为1-24<br />
    /// Write radius wear compensation according to the tool number, the tool number is 1-24
    /// </summary>
    /// <param name="cutter">刀具号，范围为1-24</param>
    /// <param name="offset">补偿值</param>
    /// <returns>是否写入成功</returns>
    [EstMqttApi(Description = "Write radius wear compensation according to the tool number, the tool number is 1-24")]
    public OperateResult WriteCutterRadiusWearOffset(int cutter, double offset) => this.WriteSystemMacroValue(12000 + cutter, new double[1]
    {
      offset
    });

    /// <summary>
    /// 读取伺服负载<br />
    /// Read servo load
    /// </summary>
    /// <returns>轴负载</returns>
    [EstMqttApi(Description = "Read servo load")]
    public OperateResult<double[]> ReadFanucAxisLoad()
    {
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(this.BuildReadArray(this.BuildReadSingle((ushort) 164, 2, 0, 0, 0, 0), this.BuildReadSingle((ushort) 137, 0, 0, 0, 0, 0), this.BuildReadSingle((ushort) 86, 1, 0, 0, 0, 0), this.BuildReadSingle((ushort) 164, 2, 0, 0, 0, 0)));
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<double[]>((OperateResult) operateResult);
      List<byte[]> numArrayList = this.ExtraContentArray(operateResult.Content.RemoveBegin<byte>(10));
      int length = (int) this.ByteTransform.TransUInt16(numArrayList[0], 14);
      return OperateResult.CreateSuccessResult<double[]>(this.GetFanucDouble(numArrayList[2], 14, length));
    }

    /// <summary>
    /// 读取机床的坐标，包括机械坐标，绝对坐标，相对坐标<br />
    /// Read the coordinates of the machine tool, including mechanical coordinates, absolute coordinates, and relative coordinates
    /// </summary>
    /// <returns>数控机床的坐标信息，包括机械坐标，绝对坐标，相对坐标</returns>
    [EstMqttApi(Description = "Read the coordinates of the machine tool, including mechanical coordinates, absolute coordinates, and relative coordinates")]
    public OperateResult<SysAllCoors> ReadSysAllCoors()
    {
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(this.BuildReadArray(this.BuildReadSingle((ushort) 164, 0, 0, 0, 0, 0), this.BuildReadSingle((ushort) 137, -1, 0, 0, 0, 0), this.BuildReadSingle((ushort) 136, 1, 0, 0, 0, 0), this.BuildReadSingle((ushort) 136, 2, 0, 0, 0, 0), this.BuildReadSingle((ushort) 163, 0, -1, 0, 0, 0), this.BuildReadSingle((ushort) 38, 0, -1, 0, 0, 0), this.BuildReadSingle((ushort) 38, 1, -1, 0, 0, 0), this.BuildReadSingle((ushort) 38, 2, -1, 0, 0, 0), this.BuildReadSingle((ushort) 38, 3, -1, 0, 0, 0), this.BuildReadSingle((ushort) 164, 0, 0, 0, 0, 0)));
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<SysAllCoors>((OperateResult) operateResult);
      List<byte[]> numArrayList = this.ExtraContentArray(operateResult.Content.RemoveBegin<byte>(10));
      int length = (int) this.ByteTransform.TransUInt16(numArrayList[0], 14);
      return OperateResult.CreateSuccessResult<SysAllCoors>(new SysAllCoors()
      {
        Absolute = this.GetFanucDouble(numArrayList[5], 14, length),
        Machine = this.GetFanucDouble(numArrayList[6], 14, length),
        Relative = this.GetFanucDouble(numArrayList[7], 14, length)
      });
    }

    /// <summary>
    /// 读取报警信息<br />
    /// Read alarm information
    /// </summary>
    /// <returns>机床的当前的所有的报警信息</returns>
    [EstMqttApi(Description = "Read alarm information")]
    public OperateResult<SysAlarm[]> ReadSystemAlarm()
    {
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(this.BuildReadArray(this.BuildReadSingle((ushort) 35, -1, 10, 2, 64, 0)));
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<SysAlarm[]>((OperateResult) operateResult);
      List<byte[]> numArrayList = this.ExtraContentArray(operateResult.Content.RemoveBegin<byte>(10));
      if (this.ByteTransform.TransUInt16(numArrayList[0], 12) <= (ushort) 0)
        return OperateResult.CreateSuccessResult<SysAlarm[]>(new SysAlarm[0]);
      SysAlarm[] sysAlarmArray = new SysAlarm[(int) this.ByteTransform.TransUInt16(numArrayList[0], 12) / 80];
      for (int index = 0; index < sysAlarmArray.Length; ++index)
      {
        sysAlarmArray[index] = new SysAlarm();
        sysAlarmArray[index].AlarmId = this.ByteTransform.TransInt32(numArrayList[0], 14 + 80 * index);
        sysAlarmArray[index].Type = this.ByteTransform.TransInt16(numArrayList[0], 20 + 80 * index);
        sysAlarmArray[index].Axis = this.ByteTransform.TransInt16(numArrayList[0], 24 + 80 * index);
        ushort num = this.ByteTransform.TransUInt16(numArrayList[0], 28 + 80 * index);
        sysAlarmArray[index].Message = this.encoding.GetString(numArrayList[0], 30 + 80 * index, (int) num);
      }
      return OperateResult.CreateSuccessResult<SysAlarm[]>(sysAlarmArray);
    }

    /// <summary>
    /// 读取fanuc机床的时间，0是开机时间，1是运行时间，2是切割时间，3是循环时间，4是空闲时间，返回秒为单位的信息<br />
    /// Read the time of the fanuc machine tool, 0 is the boot time, 1 is the running time, 2 is the cutting time,
    /// 3 is the cycle time, 4 is the idle time, and returns the information in seconds.
    /// </summary>
    /// <param name="timeType">读取的时间类型</param>
    /// <returns>秒为单位的结果</returns>
    [EstMqttApi(Description = "Read the time of the fanuc machine tool, 0 is the boot time, 1 is the running time, 2 is the cutting time, 3 is the cycle time, 4 is the idle time, and returns the information in seconds.")]
    public OperateResult<long> ReadTimeData(int timeType)
    {
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(this.BuildReadArray(this.BuildReadSingle((ushort) 288, timeType, 0, 0, 0, 0)));
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<long>((OperateResult) operateResult);
      List<byte[]> numArrayList = this.ExtraContentArray(operateResult.Content.RemoveBegin<byte>(10));
      int num1 = this.ByteTransform.TransInt32(numArrayList[0], 18);
      long num2 = (long) this.ByteTransform.TransInt32(numArrayList[0], 14);
      if (num1 < 0 || num1 > 60000)
      {
        num1 = BitConverter.ToInt32(numArrayList[0], 18);
        num2 = (long) BitConverter.ToInt32(numArrayList[0], 14);
      }
      long num3 = (long) (num1 / 1000);
      return OperateResult.CreateSuccessResult<long>(num2 * 60L + num3);
    }

    /// <summary>
    /// 读取报警状态信息<br />
    /// Read alarm status information
    /// </summary>
    /// <returns>报警状态数据</returns>
    [EstMqttApi(Description = "Read alarm status information")]
    public OperateResult<int> ReadAlarmStatus()
    {
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(this.BuildReadArray(this.BuildReadSingle((ushort) 26, 0, 0, 0, 0, 0)));
      return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<int>((OperateResult) operateResult) : OperateResult.CreateSuccessResult<int>((int) this.ByteTransform.TransUInt16(this.ExtraContentArray(operateResult.Content.RemoveBegin<byte>(10))[0], 16));
    }

    /// <summary>
    /// 读取系统的基本信息状态，工作模式，运行状态，是否急停等等操作<br />
    /// Read the basic information status of the system, working mode, running status, emergency stop, etc.
    /// </summary>
    /// <returns>结果信息数据</returns>
    [EstMqttApi(Description = "Read the basic information status of the system, working mode, running status, emergency stop, etc.")]
    public OperateResult<SysStatusInfo> ReadSysStatusInfo()
    {
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(this.BuildReadArray(this.BuildReadSingle((ushort) 25, 0, 0, 0, 0, 0), this.BuildReadSingle((ushort) 225, 0, 0, 0, 0, 0), this.BuildReadSingle((ushort) 152, 0, 0, 0, 0, 0)));
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<SysStatusInfo>((OperateResult) operateResult);
      List<byte[]> numArrayList = this.ExtraContentArray(operateResult.Content.RemoveBegin<byte>(10));
      return OperateResult.CreateSuccessResult<SysStatusInfo>(new SysStatusInfo()
      {
        Dummy = this.ByteTransform.TransInt16(numArrayList[1], 14),
        TMMode = numArrayList[2].Length >= 16 ? this.ByteTransform.TransInt16(numArrayList[2], 14) : (short) 0,
        WorkMode = (CNCWorkMode) this.ByteTransform.TransInt16(numArrayList[0], 14),
        RunStatus = (CNCRunStatus) this.ByteTransform.TransInt16(numArrayList[0], 16),
        Motion = this.ByteTransform.TransInt16(numArrayList[0], 18),
        MSTB = this.ByteTransform.TransInt16(numArrayList[0], 20),
        Emergency = this.ByteTransform.TransInt16(numArrayList[0], 22),
        Alarm = this.ByteTransform.TransInt16(numArrayList[0], 24),
        Edit = this.ByteTransform.TransInt16(numArrayList[0], 26)
      });
    }

    /// <summary>
    /// 读取设备的程序列表<br />
    /// Read the program list of the device
    /// </summary>
    /// <returns>读取结果信息</returns>
    [EstMqttApi(Description = "Read the program list of the device")]
    public OperateResult<int[]> ReadProgramList()
    {
      OperateResult<byte[]> operateResult1 = this.ReadFromCoreServer(this.BuildReadArray(this.BuildReadSingle((ushort) 6, 1, 19, 0, 0, 0)));
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(this.BuildReadArray(this.BuildReadSingle((ushort) 6, 6667, 19, 0, 0, 0)));
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<int[]>((OperateResult) operateResult1);
      if (!operateResult2.IsSuccess)
        return OperateResult.CreateFailedResult<int[]>((OperateResult) operateResult1);
      List<byte[]> numArrayList = this.ExtraContentArray(operateResult1.Content.RemoveBegin<byte>(10));
      int length = (numArrayList[0].Length - 14) / 72;
      int[] numArray = new int[length];
      for (int index = 0; index < length; ++index)
        numArray[index] = this.ByteTransform.TransInt32(numArrayList[0], 14 + 72 * index);
      return OperateResult.CreateSuccessResult<int[]>(numArray);
    }

    /// <summary>
    /// 读取当前的刀具补偿信息<br />
    /// Read current tool compensation information
    /// </summary>
    /// <param name="cutterNumber">刀具数量</param>
    /// <returns>结果内容</returns>
    [EstMqttApi(Description = "Read current tool compensation information")]
    public OperateResult<CutterInfo[]> ReadCutterInfos(int cutterNumber = 24)
    {
      OperateResult<byte[]> operateResult1 = this.ReadFromCoreServer(this.BuildReadArray(this.BuildReadSingle((ushort) 8, 1, cutterNumber, 0, 0, 0)));
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<CutterInfo[]>((OperateResult) operateResult1);
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(this.BuildReadArray(this.BuildReadSingle((ushort) 8, 1, cutterNumber, 1, 0, 0)));
      if (!operateResult2.IsSuccess)
        return OperateResult.CreateFailedResult<CutterInfo[]>((OperateResult) operateResult2);
      OperateResult<byte[]> operateResult3 = this.ReadFromCoreServer(this.BuildReadArray(this.BuildReadSingle((ushort) 8, 1, cutterNumber, 2, 0, 0)));
      if (!operateResult3.IsSuccess)
        return OperateResult.CreateFailedResult<CutterInfo[]>((OperateResult) operateResult3);
      OperateResult<byte[]> operateResult4 = this.ReadFromCoreServer(this.BuildReadArray(this.BuildReadSingle((ushort) 8, 1, cutterNumber, 3, 0, 0)));
      if (!operateResult4.IsSuccess)
        return OperateResult.CreateFailedResult<CutterInfo[]>((OperateResult) operateResult4);
      List<byte[]> numArrayList1 = this.ExtraContentArray(operateResult1.Content.RemoveBegin<byte>(10));
      List<byte[]> numArrayList2 = this.ExtraContentArray(operateResult2.Content.RemoveBegin<byte>(10));
      List<byte[]> numArrayList3 = this.ExtraContentArray(operateResult3.Content.RemoveBegin<byte>(10));
      List<byte[]> numArrayList4 = this.ExtraContentArray(operateResult4.Content.RemoveBegin<byte>(10));
      CutterInfo[] cutterInfoArray = new CutterInfo[cutterNumber];
      for (int index = 0; index < cutterInfoArray.Length; ++index)
      {
        cutterInfoArray[index] = new CutterInfo();
        cutterInfoArray[index].LengthSharpOffset = this.GetFanucDouble(numArrayList1[0], 14 + 8 * index);
        cutterInfoArray[index].LengthWearOffset = this.GetFanucDouble(numArrayList2[0], 14 + 8 * index);
        cutterInfoArray[index].RadiusSharpOffset = this.GetFanucDouble(numArrayList3[0], 14 + 8 * index);
        cutterInfoArray[index].RadiusWearOffset = this.GetFanucDouble(numArrayList4[0], 14 + 8 * index);
      }
      return OperateResult.CreateSuccessResult<CutterInfo[]>(cutterInfoArray);
    }

    /// <summary>
    /// 读取R数据，需要传入起始地址和结束地址，返回byte[]数据信息<br />
    /// To read R data, you need to pass in the start address and end address, and return byte[] data information
    /// </summary>
    /// <param name="start">起始地址</param>
    /// <param name="end">结束地址</param>
    /// <returns>读取结果</returns>
    [EstMqttApi(Description = "To read R data, you need to pass in the start address and end address, and return byte[] data information")]
    public OperateResult<byte[]> ReadRData(int start, int end)
    {
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(this.BuildReadArray(this.BuildReadMulti((ushort) 2, (ushort) 32769, start, end, 5, 0, 0)));
      if (!operateResult.IsSuccess)
        return operateResult;
      List<byte[]> numArrayList = this.ExtraContentArray(operateResult.Content.RemoveBegin<byte>(10));
      int length = (int) this.ByteTransform.TransUInt16(numArrayList[0], 12);
      return OperateResult.CreateSuccessResult<byte[]>(numArrayList[0].SelectMiddle<byte>(14, length));
    }

    /// <summary>
    /// 读取工件尺寸<br />
    /// Read workpiece size
    /// </summary>
    /// <returns>结果数据信息</returns>
    [EstMqttApi(Description = "Read workpiece size")]
    public OperateResult<double[]> ReadDeviceWorkPiecesSize() => this.ReadSystemMacroValue(601, 20);

    /// <summary>读取指定的程序内容，目前还没有测试通</summary>
    /// <param name="program">程序号</param>
    /// <returns>程序内容</returns>
    [Obsolete]
    private OperateResult<string> ReadProgram(int program)
    {
      OperateResult<byte[]> operateResult1 = this.ReadFromCoreServer(this.BuildReadProgram(program));
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<string>((OperateResult) operateResult1);
      Console.WriteLine("等待第二次数据接收。");
      Thread.Sleep(100);
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer("a0 a0 a0 a0 00 01 18 04 00 08 00 00 00 00 00 00 00 00".ToHexBytes());
      return !operateResult2.IsSuccess ? OperateResult.CreateFailedResult<string>((OperateResult) operateResult2) : OperateResult.CreateSuccessResult<string>(Encoding.ASCII.GetString(operateResult2.Content, 10, operateResult2.Content.Length - 10));
    }

    /// <summary>
    /// 读取当前程序的前台路径<br />
    /// Read the foreground path of the current program
    /// </summary>
    /// <returns>程序的路径信息</returns>
    [EstMqttApi(Description = "Read the foreground path of the current program")]
    public OperateResult<string> ReadCurrentForegroundDir()
    {
      OperateResult<byte[]> operateResult = this.ReadFromCoreServer(this.BuildReadArray(this.BuildReadSingle((ushort) 176, 1, 0, 0, 0, 0)));
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<string>((OperateResult) operateResult);
      List<byte[]> numArrayList = this.ExtraContentArray(operateResult.Content.RemoveBegin<byte>(10));
      int num = 0;
      for (int index = 14; index < numArrayList[0].Length; ++index)
      {
        if (numArrayList[0][index] == (byte) 0)
        {
          num = index;
          break;
        }
      }
      if (num == 0)
        num = numArrayList[0].Length;
      return OperateResult.CreateSuccessResult<string>(Encoding.ASCII.GetString(numArrayList[0], 14, num - 14));
    }

    /// <summary>
    /// 设置指定路径为当前路径<br />
    /// Set the specified path as the current path
    /// </summary>
    /// <param name="programName">程序名</param>
    /// <returns>结果信息</returns>
    [EstMqttApi(Description = "Set the specified path as the current path")]
    public OperateResult SetDeviceProgsCurr(string programName)
    {
      OperateResult<string> operateResult1 = this.ReadCurrentForegroundDir();
      if (!operateResult1.IsSuccess)
        return (OperateResult) operateResult1;
      byte[] data = new byte[256];
      Encoding.ASCII.GetBytes(operateResult1.Content + programName).CopyTo((Array) data, 0);
      OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(this.BuildReadArray(this.BuildWriteSingle((ushort) 186, 0, 0, 0, 0, data)));
      if (!operateResult2.IsSuccess)
        return (OperateResult) OperateResult.CreateFailedResult<string>((OperateResult) operateResult2);
      List<byte[]> numArrayList = this.ExtraContentArray(operateResult2.Content.RemoveBegin<byte>(10));
      int err = (int) numArrayList[0][10] * 256 + (int) numArrayList[0][11];
      return err == 0 ? OperateResult.CreateSuccessResult() : new OperateResult(err, StringResources.Language.UnknownError);
    }

    /// <summary>
    /// 读取机床的当前时间信息<br />
    /// Read the current time information of the machine tool
    /// </summary>
    /// <returns>时间信息</returns>
    [EstMqttApi(Description = "Read the current time information of the machine tool")]
    public OperateResult<DateTime> ReadCurrentDateTime()
    {
      OperateResult<double> operateResult1 = this.ReadSystemMacroValue(3011);
      if (!operateResult1.IsSuccess)
        return OperateResult.CreateFailedResult<DateTime>((OperateResult) operateResult1);
      OperateResult<double> operateResult2 = this.ReadSystemMacroValue(3012);
      if (!operateResult2.IsSuccess)
        return OperateResult.CreateFailedResult<DateTime>((OperateResult) operateResult2);
      int int32 = Convert.ToInt32(operateResult1.Content);
      string str1 = int32.ToString();
      int32 = Convert.ToInt32(operateResult2.Content);
      string str2 = int32.ToString().PadLeft(6, '0');
      return OperateResult.CreateSuccessResult<DateTime>(new DateTime(int.Parse(str1.Substring(0, 4)), int.Parse(str1.Substring(4, 2)), int.Parse(str1.Substring(6)), int.Parse(str2.Substring(0, 2)), int.Parse(str2.Substring(2, 2)), int.Parse(str2.Substring(4))));
    }

    /// <summary>
    /// 读取当前的已加工的零件数量<br />
    /// Read the current number of processed parts
    /// </summary>
    /// <returns>已经加工的零件数量</returns>
    [EstMqttApi(Description = "Read the current number of processed parts")]
    public OperateResult<int> ReadCurrentProduceCount()
    {
      OperateResult<double> operateResult = this.ReadSystemMacroValue(3901);
      return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<int>((OperateResult) operateResult) : OperateResult.CreateSuccessResult<int>(Convert.ToInt32(operateResult.Content));
    }

    /// <summary>
    /// 读取期望的加工的零件数量<br />
    /// Read the expected number of processed parts
    /// </summary>
    /// <returns>期望的加工的零件数量</returns>
    [EstMqttApi(Description = "Read the expected number of processed parts")]
    public OperateResult<int> ReadExpectProduceCount()
    {
      OperateResult<double> operateResult = this.ReadSystemMacroValue(3902);
      return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<int>((OperateResult) operateResult) : OperateResult.CreateSuccessResult<int>(Convert.ToInt32(operateResult.Content));
    }

    /// <inheritdoc cref="M:EstCommunication.CNC.Fanuc.FanucSeries0i.ReadSpindleSpeedAndFeedRate" />
    public async Task<OperateResult<double, double>> ReadSpindleSpeedAndFeedRateAsync()
    {
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(this.BuildReadArray(this.BuildReadSingle((ushort) 164, 3, 0, 0, 0, 0), this.BuildReadSingle((ushort) 138, 1, 0, 0, 0, 0), this.BuildReadSingle((ushort) 136, 3, 0, 0, 0, 0), this.BuildReadSingle((ushort) 136, 4, 0, 0, 0, 0), this.BuildReadSingle((ushort) 36, 0, 0, 0, 0, 0), this.BuildReadSingle((ushort) 37, 0, 0, 0, 0, 0), this.BuildReadSingle((ushort) 164, 3, 0, 0, 0, 0)));
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<double, double>((OperateResult) read);
      List<byte[]> result = this.ExtraContentArray(read.Content.RemoveBegin<byte>(10));
      return OperateResult.CreateSuccessResult<double, double>(this.GetFanucDouble(result[5], 14), this.GetFanucDouble(result[4], 14));
    }

    /// <inheritdoc cref="M:EstCommunication.CNC.Fanuc.FanucSeries0i.ReadSystemProgramCurrent" />
    public async Task<OperateResult<string, int>> ReadSystemProgramCurrentAsync()
    {
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(this.BuildReadArray(this.BuildReadSingle((ushort) 207, 0, 0, 0, 0, 0)));
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<string, int>((OperateResult) read);
      List<byte[]> result = this.ExtraContentArray(read.Content.RemoveBegin<byte>(10));
      int number = this.ByteTransform.TransInt32(result[0], 14);
      string name = this.encoding.GetString(result[0].SelectMiddle<byte>(18, 36)).TrimEnd(new char[1]);
      return OperateResult.CreateSuccessResult<string, int>(name, number);
    }

    /// <inheritdoc cref="M:EstCommunication.CNC.Fanuc.FanucSeries0i.ReadLanguage" />
    public async Task<OperateResult<ushort>> ReadLanguageAsync()
    {
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(this.BuildReadArray(this.BuildReadSingle((ushort) 141, 3281, 3281, 0, 0, 0)));
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<ushort>((OperateResult) read);
      List<byte[]> result = this.ExtraContentArray(read.Content.RemoveBegin<byte>(10));
      ushort code = this.ByteTransform.TransUInt16(result[0], 24);
      this.ChangeTextEncoding(code);
      return OperateResult.CreateSuccessResult<ushort>(code);
    }

    /// <inheritdoc cref="M:EstCommunication.CNC.Fanuc.FanucSeries0i.ReadSystemMacroValue(System.Int32)" />
    public async Task<OperateResult<double>> ReadSystemMacroValueAsync(int number)
    {
      OperateResult<double[]> result = await this.ReadSystemMacroValueAsync(number, 1);
      return ByteTransformHelper.GetResultFromArray<double>(result);
    }

    /// <inheritdoc cref="M:EstCommunication.CNC.Fanuc.FanucSeries0i.ReadSystemMacroValue(System.Int32,System.Int32)" />
    public async Task<OperateResult<double[]>> ReadSystemMacroValueAsync(
      int number,
      int length)
    {
      int[] lenArray = SoftBasic.SplitIntegerToArray(length, 5);
      int index = number;
      List<byte> result = new List<byte>();
      for (int i = 0; i < lenArray.Length; ++i)
      {
        OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(this.BuildReadArray(this.BuildReadSingle((ushort) 21, index, index + lenArray[i] - 1, 0, 0, 0)));
        if (!read.IsSuccess)
          return OperateResult.CreateFailedResult<double[]>((OperateResult) read);
        result.AddRange((IEnumerable<byte>) this.ExtraContentArray(read.Content.RemoveBegin<byte>(10))[0].RemoveBegin<byte>(14));
        index += lenArray[i];
        read = (OperateResult<byte[]>) null;
      }
      try
      {
        return OperateResult.CreateSuccessResult<double[]>(this.GetFanucDouble(result.ToArray(), 0, length));
      }
      catch (Exception ex)
      {
        return new OperateResult<double[]>(ex.Message + " Source:" + result.ToArray().ToHexString(' '));
      }
    }

    /// <inheritdoc cref="M:EstCommunication.CNC.Fanuc.FanucSeries0i.WriteSystemMacroValue(System.Int32,System.Double[])" />
    public async Task<OperateResult> WriteSystemMacroValueAsync(
      int number,
      double[] values)
    {
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(this.BuildReadArray(this.BuildWriteSingle((ushort) 22, number, number + values.Length - 1, 0, 0, values)));
      if (!read.IsSuccess)
        return (OperateResult) OperateResult.CreateFailedResult<string, int>((OperateResult) read);
      List<byte[]> result = this.ExtraContentArray(read.Content.RemoveBegin<byte>(10));
      return this.ByteTransform.TransUInt16(result[0], 6) != (ushort) 0 ? new OperateResult((int) this.ByteTransform.TransUInt16(result[0], 6), "Unknown Error") : OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc cref="M:EstCommunication.CNC.Fanuc.FanucSeries0i.WriteCutterLengthShapeOffset(System.Int32,System.Double)" />
    public async Task<OperateResult> WriteCutterLengthSharpOffsetAsync(
      int cutter,
      double offset)
    {
      OperateResult operateResult = await this.WriteSystemMacroValueAsync(11000 + cutter, new double[1]
      {
        offset
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.CNC.Fanuc.FanucSeries0i.WriteCutterLengthWearOffset(System.Int32,System.Double)" />
    public async Task<OperateResult> WriteCutterLengthWearOffsetAsync(
      int cutter,
      double offset)
    {
      OperateResult operateResult = await this.WriteSystemMacroValueAsync(10000 + cutter, new double[1]
      {
        offset
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.CNC.Fanuc.FanucSeries0i.WriteCutterRadiusShapeOffset(System.Int32,System.Double)" />
    public async Task<OperateResult> WriteCutterRadiusSharpOffsetAsync(
      int cutter,
      double offset)
    {
      OperateResult operateResult = await this.WriteSystemMacroValueAsync(13000 + cutter, new double[1]
      {
        offset
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.CNC.Fanuc.FanucSeries0i.WriteCutterRadiusWearOffset(System.Int32,System.Double)" />
    public async Task<OperateResult> WriteCutterRadiusWearOffsetAsync(
      int cutter,
      double offset)
    {
      OperateResult operateResult = await this.WriteSystemMacroValueAsync(12000 + cutter, new double[1]
      {
        offset
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.CNC.Fanuc.FanucSeries0i.ReadFanucAxisLoad" />
    public async Task<OperateResult<double[]>> ReadFanucAxisLoadAsync()
    {
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(this.BuildReadArray(this.BuildReadSingle((ushort) 164, 2, 0, 0, 0, 0), this.BuildReadSingle((ushort) 137, 0, 0, 0, 0, 0), this.BuildReadSingle((ushort) 86, 1, 0, 0, 0, 0), this.BuildReadSingle((ushort) 164, 2, 0, 0, 0, 0)));
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<double[]>((OperateResult) read);
      List<byte[]> result = this.ExtraContentArray(read.Content.RemoveBegin<byte>(10));
      int length = (int) this.ByteTransform.TransUInt16(result[0], 14);
      return OperateResult.CreateSuccessResult<double[]>(this.GetFanucDouble(result[2], 14, length));
    }

    /// <inheritdoc cref="M:EstCommunication.CNC.Fanuc.FanucSeries0i.ReadSysAllCoors" />
    public async Task<OperateResult<SysAllCoors>> ReadSysAllCoorsAsync()
    {
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(this.BuildReadArray(this.BuildReadSingle((ushort) 164, 0, 0, 0, 0, 0), this.BuildReadSingle((ushort) 137, -1, 0, 0, 0, 0), this.BuildReadSingle((ushort) 136, 1, 0, 0, 0, 0), this.BuildReadSingle((ushort) 136, 2, 0, 0, 0, 0), this.BuildReadSingle((ushort) 163, 0, -1, 0, 0, 0), this.BuildReadSingle((ushort) 38, 0, -1, 0, 0, 0), this.BuildReadSingle((ushort) 38, 1, -1, 0, 0, 0), this.BuildReadSingle((ushort) 38, 2, -1, 0, 0, 0), this.BuildReadSingle((ushort) 38, 3, -1, 0, 0, 0), this.BuildReadSingle((ushort) 164, 0, 0, 0, 0, 0)));
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<SysAllCoors>((OperateResult) read);
      List<byte[]> result = this.ExtraContentArray(read.Content.RemoveBegin<byte>(10));
      int length = (int) this.ByteTransform.TransUInt16(result[0], 14);
      return OperateResult.CreateSuccessResult<SysAllCoors>(new SysAllCoors()
      {
        Absolute = this.GetFanucDouble(result[5], 14, length),
        Machine = this.GetFanucDouble(result[6], 14, length),
        Relative = this.GetFanucDouble(result[7], 14, length)
      });
    }

    /// <inheritdoc cref="M:EstCommunication.CNC.Fanuc.FanucSeries0i.ReadSystemAlarm" />
    public async Task<OperateResult<SysAlarm[]>> ReadSystemAlarmAsync()
    {
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(this.BuildReadArray(this.BuildReadSingle((ushort) 35, -1, 10, 2, 64, 0)));
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<SysAlarm[]>((OperateResult) read);
      List<byte[]> result = this.ExtraContentArray(read.Content.RemoveBegin<byte>(10));
      if (this.ByteTransform.TransUInt16(result[0], 12) <= (ushort) 0)
        return OperateResult.CreateSuccessResult<SysAlarm[]>(new SysAlarm[0]);
      int length = (int) this.ByteTransform.TransUInt16(result[0], 12) / 80;
      SysAlarm[] alarms = new SysAlarm[length];
      for (int i = 0; i < alarms.Length; ++i)
      {
        alarms[i] = new SysAlarm();
        alarms[i].AlarmId = this.ByteTransform.TransInt32(result[0], 14 + 80 * i);
        alarms[i].Type = this.ByteTransform.TransInt16(result[0], 20 + 80 * i);
        alarms[i].Axis = this.ByteTransform.TransInt16(result[0], 24 + 80 * i);
        ushort msgLength = this.ByteTransform.TransUInt16(result[0], 28 + 80 * i);
        alarms[i].Message = this.encoding.GetString(result[0], 30 + 80 * i, (int) msgLength);
      }
      return OperateResult.CreateSuccessResult<SysAlarm[]>(alarms);
    }

    /// <inheritdoc cref="M:EstCommunication.CNC.Fanuc.FanucSeries0i.ReadTimeData(System.Int32)" />
    public async Task<OperateResult<long>> ReadTimeDataAsync(int timeType)
    {
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(this.BuildReadArray(this.BuildReadSingle((ushort) 288, timeType, 0, 0, 0, 0)));
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<long>((OperateResult) read);
      List<byte[]> result = this.ExtraContentArray(read.Content.RemoveBegin<byte>(10));
      int millisecond = this.ByteTransform.TransInt32(result[0], 18);
      long munite = (long) this.ByteTransform.TransInt32(result[0], 14);
      if (millisecond < 0 || millisecond > 60000)
      {
        millisecond = BitConverter.ToInt32(result[0], 18);
        munite = (long) BitConverter.ToInt32(result[0], 14);
      }
      long seconds = (long) (millisecond / 1000);
      return OperateResult.CreateSuccessResult<long>(munite * 60L + seconds);
    }

    /// <inheritdoc cref="M:EstCommunication.CNC.Fanuc.FanucSeries0i.ReadAlarmStatus" />
    public async Task<OperateResult<int>> ReadAlarmStatusAsync()
    {
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(this.BuildReadArray(this.BuildReadSingle((ushort) 26, 0, 0, 0, 0, 0)));
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<int>((OperateResult) read);
      List<byte[]> result = this.ExtraContentArray(read.Content.RemoveBegin<byte>(10));
      return OperateResult.CreateSuccessResult<int>((int) this.ByteTransform.TransUInt16(result[0], 16));
    }

    /// <inheritdoc cref="M:EstCommunication.CNC.Fanuc.FanucSeries0i.ReadSysStatusInfo" />
    public async Task<OperateResult<SysStatusInfo>> ReadSysStatusInfoAsync()
    {
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(this.BuildReadArray(this.BuildReadSingle((ushort) 25, 0, 0, 0, 0, 0), this.BuildReadSingle((ushort) 225, 0, 0, 0, 0, 0), this.BuildReadSingle((ushort) 152, 0, 0, 0, 0, 0)));
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<SysStatusInfo>((OperateResult) read);
      List<byte[]> result = this.ExtraContentArray(read.Content.RemoveBegin<byte>(10));
      return OperateResult.CreateSuccessResult<SysStatusInfo>(new SysStatusInfo()
      {
        Dummy = this.ByteTransform.TransInt16(result[1], 14),
        TMMode = result[2].Length >= 16 ? this.ByteTransform.TransInt16(result[2], 14) : (short) 0,
        WorkMode = (CNCWorkMode) this.ByteTransform.TransInt16(result[0], 14),
        RunStatus = (CNCRunStatus) this.ByteTransform.TransInt16(result[0], 16),
        Motion = this.ByteTransform.TransInt16(result[0], 18),
        MSTB = this.ByteTransform.TransInt16(result[0], 20),
        Emergency = this.ByteTransform.TransInt16(result[0], 22),
        Alarm = this.ByteTransform.TransInt16(result[0], 24),
        Edit = this.ByteTransform.TransInt16(result[0], 26)
      });
    }

    /// <inheritdoc cref="M:EstCommunication.CNC.Fanuc.FanucSeries0i.ReadProgramList" />
    public async Task<OperateResult<int[]>> ReadProgramListAsync()
    {
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(this.BuildReadArray(this.BuildReadSingle((ushort) 6, 1, 19, 0, 0, 0)));
      OperateResult<byte[]> check = await this.ReadFromCoreServerAsync(this.BuildReadArray(this.BuildReadSingle((ushort) 6, 6667, 19, 0, 0, 0)));
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<int[]>((OperateResult) read);
      if (!check.IsSuccess)
        return OperateResult.CreateFailedResult<int[]>((OperateResult) read);
      List<byte[]> result = this.ExtraContentArray(read.Content.RemoveBegin<byte>(10));
      int length = (result[0].Length - 14) / 72;
      int[] programs = new int[length];
      for (int i = 0; i < length; ++i)
        programs[i] = this.ByteTransform.TransInt32(result[0], 14 + 72 * i);
      return OperateResult.CreateSuccessResult<int[]>(programs);
    }

    /// <inheritdoc cref="M:EstCommunication.CNC.Fanuc.FanucSeries0i.ReadCutterInfos(System.Int32)" />
    public async Task<OperateResult<CutterInfo[]>> ReadCutterInfosAsync(
      int cutterNumber = 24)
    {
      OperateResult<byte[]> read1 = await this.ReadFromCoreServerAsync(this.BuildReadArray(this.BuildReadSingle((ushort) 8, 1, cutterNumber, 0, 0, 0)));
      if (!read1.IsSuccess)
        return OperateResult.CreateFailedResult<CutterInfo[]>((OperateResult) read1);
      OperateResult<byte[]> read2 = await this.ReadFromCoreServerAsync(this.BuildReadArray(this.BuildReadSingle((ushort) 8, 1, cutterNumber, 1, 0, 0)));
      if (!read2.IsSuccess)
        return OperateResult.CreateFailedResult<CutterInfo[]>((OperateResult) read2);
      OperateResult<byte[]> read3 = await this.ReadFromCoreServerAsync(this.BuildReadArray(this.BuildReadSingle((ushort) 8, 1, cutterNumber, 2, 0, 0)));
      if (!read3.IsSuccess)
        return OperateResult.CreateFailedResult<CutterInfo[]>((OperateResult) read3);
      OperateResult<byte[]> read4 = await this.ReadFromCoreServerAsync(this.BuildReadArray(this.BuildReadSingle((ushort) 8, 1, cutterNumber, 3, 0, 0)));
      if (!read4.IsSuccess)
        return OperateResult.CreateFailedResult<CutterInfo[]>((OperateResult) read4);
      List<byte[]> result1 = this.ExtraContentArray(read1.Content.RemoveBegin<byte>(10));
      List<byte[]> result2 = this.ExtraContentArray(read2.Content.RemoveBegin<byte>(10));
      List<byte[]> result3 = this.ExtraContentArray(read3.Content.RemoveBegin<byte>(10));
      List<byte[]> result4 = this.ExtraContentArray(read4.Content.RemoveBegin<byte>(10));
      CutterInfo[] cutters = new CutterInfo[cutterNumber];
      for (int i = 0; i < cutters.Length; ++i)
      {
        cutters[i] = new CutterInfo();
        cutters[i].LengthSharpOffset = this.GetFanucDouble(result1[0], 14 + 8 * i);
        cutters[i].LengthWearOffset = this.GetFanucDouble(result2[0], 14 + 8 * i);
        cutters[i].RadiusSharpOffset = this.GetFanucDouble(result3[0], 14 + 8 * i);
        cutters[i].RadiusWearOffset = this.GetFanucDouble(result4[0], 14 + 8 * i);
      }
      return OperateResult.CreateSuccessResult<CutterInfo[]>(cutters);
    }

    /// <inheritdoc cref="M:EstCommunication.CNC.Fanuc.FanucSeries0i.ReadRData(System.Int32,System.Int32)" />
    public async Task<OperateResult<byte[]>> ReadRDataAsync(int start, int end)
    {
      OperateResult<byte[]> read1 = await this.ReadFromCoreServerAsync(this.BuildReadArray(this.BuildReadMulti((ushort) 2, (ushort) 32769, start, end, 5, 0, 0)));
      if (!read1.IsSuccess)
        return read1;
      List<byte[]> result = this.ExtraContentArray(read1.Content.RemoveBegin<byte>(10));
      int length = (int) this.ByteTransform.TransUInt16(result[0], 12);
      return OperateResult.CreateSuccessResult<byte[]>(result[0].SelectMiddle<byte>(14, length));
    }

    /// <inheritdoc cref="M:EstCommunication.CNC.Fanuc.FanucSeries0i.ReadDeviceWorkPiecesSize" />
    public async Task<OperateResult<double[]>> ReadDeviceWorkPiecesSizeAsync()
    {
      OperateResult<double[]> operateResult = await this.ReadSystemMacroValueAsync(601, 20);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.CNC.Fanuc.FanucSeries0i.ReadCurrentForegroundDir" />
    public async Task<OperateResult<string>> ReadCurrentForegroundDirAsync()
    {
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(this.BuildReadArray(this.BuildReadSingle((ushort) 176, 1, 0, 0, 0, 0)));
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<string>((OperateResult) read);
      List<byte[]> result = this.ExtraContentArray(read.Content.RemoveBegin<byte>(10));
      int index = 0;
      for (int i = 14; i < result[0].Length; ++i)
      {
        if (result[0][i] == (byte) 0)
        {
          index = i;
          break;
        }
      }
      if (index == 0)
        index = result[0].Length;
      return OperateResult.CreateSuccessResult<string>(Encoding.ASCII.GetString(result[0], 14, index - 14));
    }

    /// <inheritdoc cref="M:EstCommunication.CNC.Fanuc.FanucSeries0i.SetDeviceProgsCurr(System.String)" />
    public async Task<OperateResult> SetDeviceProgsCurrAsync(string programName)
    {
      OperateResult<string> path = await this.ReadCurrentForegroundDirAsync();
      if (!path.IsSuccess)
        return (OperateResult) path;
      byte[] buffer = new byte[256];
      Encoding.ASCII.GetBytes(path.Content + programName).CopyTo((Array) buffer, 0);
      OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(this.BuildReadArray(this.BuildWriteSingle((ushort) 186, 0, 0, 0, 0, buffer)));
      if (!read.IsSuccess)
        return (OperateResult) OperateResult.CreateFailedResult<string>((OperateResult) read);
      List<byte[]> result = this.ExtraContentArray(read.Content.RemoveBegin<byte>(10));
      int status = (int) result[0][10] * 256 + (int) result[0][11];
      return status != 0 ? new OperateResult(status, StringResources.Language.UnknownError) : OperateResult.CreateSuccessResult();
    }

    /// <inheritdoc cref="M:EstCommunication.CNC.Fanuc.FanucSeries0i.ReadCurrentDateTime" />
    public async Task<OperateResult<DateTime>> ReadCurrentDateTimeAsync()
    {
      OperateResult<double> read1 = await this.ReadSystemMacroValueAsync(3011);
      if (!read1.IsSuccess)
        return OperateResult.CreateFailedResult<DateTime>((OperateResult) read1);
      OperateResult<double> read2 = await this.ReadSystemMacroValueAsync(3012);
      if (!read2.IsSuccess)
        return OperateResult.CreateFailedResult<DateTime>((OperateResult) read2);
      string date = Convert.ToInt32(read1.Content).ToString();
      string time = Convert.ToInt32(read2.Content).ToString().PadLeft(6, '0');
      return OperateResult.CreateSuccessResult<DateTime>(new DateTime(int.Parse(date.Substring(0, 4)), int.Parse(date.Substring(4, 2)), int.Parse(date.Substring(6)), int.Parse(time.Substring(0, 2)), int.Parse(time.Substring(2, 2)), int.Parse(time.Substring(4))));
    }

    /// <inheritdoc cref="M:EstCommunication.CNC.Fanuc.FanucSeries0i.ReadCurrentProduceCount" />
    public async Task<OperateResult<int>> ReadCurrentProduceCountAsync()
    {
      OperateResult<double> read = await this.ReadSystemMacroValueAsync(3901);
      OperateResult<int> operateResult = read.IsSuccess ? OperateResult.CreateSuccessResult<int>(Convert.ToInt32(read.Content)) : OperateResult.CreateFailedResult<int>((OperateResult) read);
      read = (OperateResult<double>) null;
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.CNC.Fanuc.FanucSeries0i.ReadExpectProduceCount" />
    public async Task<OperateResult<int>> ReadExpectProduceCountAsync()
    {
      OperateResult<double> read = await this.ReadSystemMacroValueAsync(3902);
      OperateResult<int> operateResult = read.IsSuccess ? OperateResult.CreateSuccessResult<int>(Convert.ToInt32(read.Content)) : OperateResult.CreateFailedResult<int>((OperateResult) read);
      read = (OperateResult<double>) null;
      return operateResult;
    }

    /// <summary>构建读取一个命令的数据内容</summary>
    /// <param name="code">命令码</param>
    /// <param name="a">第一个参数内容</param>
    /// <param name="b">第二个参数内容</param>
    /// <param name="c">第三个参数内容</param>
    /// <param name="d">第四个参数内容</param>
    /// <param name="e">第五个参数内容</param>
    /// <returns>总报文信息</returns>
    public byte[] BuildReadSingle(ushort code, int a, int b, int c, int d, int e) => this.BuildReadMulti((ushort) 1, code, a, b, c, d, e);

    /// <summary>构建读取多个命令的数据内容</summary>
    /// <param name="mode">模式</param>
    /// <param name="code">命令码</param>
    /// <param name="a">第一个参数内容</param>
    /// <param name="b">第二个参数内容</param>
    /// <param name="c">第三个参数内容</param>
    /// <param name="d">第四个参数内容</param>
    /// <param name="e">第五个参数内容</param>
    /// <returns>总报文信息</returns>
    public byte[] BuildReadMulti(ushort mode, ushort code, int a, int b, int c, int d, int e)
    {
      byte[] numArray = new byte[28];
      numArray[1] = (byte) 28;
      this.ByteTransform.TransByte(mode).CopyTo((Array) numArray, 2);
      numArray[5] = (byte) 1;
      this.ByteTransform.TransByte(code).CopyTo((Array) numArray, 6);
      this.ByteTransform.TransByte(a).CopyTo((Array) numArray, 8);
      this.ByteTransform.TransByte(b).CopyTo((Array) numArray, 12);
      this.ByteTransform.TransByte(c).CopyTo((Array) numArray, 16);
      this.ByteTransform.TransByte(d).CopyTo((Array) numArray, 20);
      this.ByteTransform.TransByte(e).CopyTo((Array) numArray, 24);
      return numArray;
    }

    /// <summary>创建写入byte[]数组的报文信息</summary>
    /// <param name="code">命令码</param>
    /// <param name="a">第一个参数内容</param>
    /// <param name="b">第二个参数内容</param>
    /// <param name="c">第三个参数内容</param>
    /// <param name="d">第四个参数内容</param>
    /// <param name="data">等待写入的byte数组信息</param>
    /// <returns>总报文信息</returns>
    public byte[] BuildWriteSingle(ushort code, int a, int b, int c, int d, byte[] data)
    {
      byte[] numArray = new byte[28 + data.Length];
      this.ByteTransform.TransByte((ushort) numArray.Length).CopyTo((Array) numArray, 0);
      numArray[3] = (byte) 1;
      numArray[5] = (byte) 1;
      this.ByteTransform.TransByte(code).CopyTo((Array) numArray, 6);
      this.ByteTransform.TransByte(a).CopyTo((Array) numArray, 8);
      this.ByteTransform.TransByte(b).CopyTo((Array) numArray, 12);
      this.ByteTransform.TransByte(c).CopyTo((Array) numArray, 16);
      this.ByteTransform.TransByte(d).CopyTo((Array) numArray, 20);
      this.ByteTransform.TransByte(data.Length).CopyTo((Array) numArray, 24);
      if ((uint) data.Length > 0U)
        data.CopyTo((Array) numArray, 28);
      return numArray;
    }

    /// <summary>创建写入单个double数组的报文信息</summary>
    /// <param name="code">功能码</param>
    /// <param name="a">第一个参数内容</param>
    /// <param name="b">第二个参数内容</param>
    /// <param name="c">第三个参数内容</param>
    /// <param name="d">第四个参数内容</param>
    /// <param name="data">等待写入的double数组信息</param>
    /// <returns>总报文信息</returns>
    public byte[] BuildWriteSingle(ushort code, int a, int b, int c, int d, double[] data)
    {
      byte[] data1 = new byte[data.Length * 8];
      for (int index = 0; index < data.Length; ++index)
        this.CreateFromFanucDouble(data[index]).CopyTo((Array) data1, 0);
      return this.BuildWriteSingle(code, a, b, c, d, data1);
    }

    /// <summary>创建读取运行程序的报文信息</summary>
    /// <param name="program">程序号</param>
    /// <returns>总报文</returns>
    public byte[] BuildReadProgram(int program) => "\r\na0 a0 a0 a0 00 01 15 01 02 04 00 00 00 01 4f 36\r\n30 30 32 2d 4f 36 30 30 32 00 00 00 00 00 00 00\r\n00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00\r\n00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00\r\n00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00\r\n00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00\r\n00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00\r\n00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00\r\n00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00\r\n00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00\r\n00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00\r\n00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00\r\n00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00\r\n00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00\r\n00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00\r\n00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00\r\n00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00\r\n00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00\r\n00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00\r\n00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00\r\n00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00\r\n00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00\r\n00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00\r\n00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00\r\n00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00\r\n00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00\r\n00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00\r\n00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00\r\n00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00\r\n00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00\r\n00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00\r\n00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00\r\n00 00 00 00 00 00 00 00 00 00 00 00 00 00\r\n".ToHexBytes();

    /// <summary>创建多个命令报文的总报文信息</summary>
    /// <param name="commands">报文命令的数组</param>
    /// <returns>总报文信息</returns>
    public byte[] BuildReadArray(params byte[][] commands)
    {
      MemoryStream memoryStream = new MemoryStream();
      memoryStream.Write(new byte[10]
      {
        (byte) 160,
        (byte) 160,
        (byte) 160,
        (byte) 160,
        (byte) 0,
        (byte) 1,
        (byte) 33,
        (byte) 1,
        (byte) 0,
        (byte) 30
      }, 0, 10);
      memoryStream.Write(this.ByteTransform.TransByte((ushort) commands.Length), 0, 2);
      for (int index = 0; index < commands.Length; ++index)
        memoryStream.Write(commands[index], 0, commands[index].Length);
      byte[] array = memoryStream.ToArray();
      this.ByteTransform.TransByte((ushort) (array.Length - 10)).CopyTo((Array) array, 8);
      return array;
    }

    /// <summary>从机床返回的数据里解析出实际的数据内容，去除了一些多余的信息报文。</summary>
    /// <param name="content">返回的报文信息</param>
    /// <returns>解析之后的报文信息</returns>
    public List<byte[]> ExtraContentArray(byte[] content)
    {
      List<byte[]> numArrayList = new List<byte[]>();
      int num1 = (int) this.ByteTransform.TransUInt16(content, 0);
      int index1 = 2;
      for (int index2 = 0; index2 < num1; ++index2)
      {
        ushort num2 = this.ByteTransform.TransUInt16(content, index1);
        numArrayList.Add(content.SelectMiddle<byte>(index1 + 2, (int) num2 - 2));
        index1 += (int) num2;
      }
      return numArrayList;
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("FanucSeries0i[{0}:{1}]", (object) this.IpAddress, (object) this.Port);
  }
}
