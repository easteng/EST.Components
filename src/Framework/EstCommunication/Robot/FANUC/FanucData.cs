// Decompiled with JetBrains decompiler
// Type: EstCommunication.Robot.FANUC.FanucData
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using EstCommunication.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EstCommunication.Robot.FANUC
{
  /// <summary>Fanuc机器人的所有的数据信息</summary>
  public class FanucData
  {
    private bool isIni = false;

    public FanucAlarm[] AlarmList { get; set; }

    public FanucAlarm AlarmCurrent { get; set; }

    public FanucAlarm AlarmPassword { get; set; }

    public FanucPose CurrentPose { get; set; }

    public FanucPose CurrentPoseUF { get; set; }

    public FanucPose CurrentPose2 { get; set; }

    public FanucPose CurrentPose3 { get; set; }

    public FanucPose CurrentPose4 { get; set; }

    public FanucPose CurrentPose5 { get; set; }

    public FanucTask Task { get; set; }

    public FanucTask TaskIgnoreMacro { get; set; }

    public FanucTask TaskIgnoreKarel { get; set; }

    public FanucTask TaskIgnoreMacroKarel { get; set; }

    public FanucPose[] PosRegGP1 { get; set; }

    public FanucPose[] PosRegGP2 { get; set; }

    public FanucPose[] PosRegGP3 { get; set; }

    public FanucPose[] PosRegGP4 { get; set; }

    public FanucPose[] PosRegGP5 { get; set; }

    public int FAST_CLOCK { get; set; }

    public int Timer10_TIMER_VAL { get; set; }

    public float MOR_GRP_CURRENT_ANG { get; set; }

    public float DUTY_TEMP { get; set; }

    public string TIMER10_COMMENT { get; set; }

    public string TIMER2_COMMENT { get; set; }

    public FanucPose MNUTOOL1_1 { get; set; }

    public string HTTPKCL_CMDS { get; set; }

    public int[] NumReg1 { get; set; }

    public float[] NumReg2 { get; set; }

    public FanucPose[] DataPosRegMG { get; set; }

    public string[] DIComment { get; set; }

    public string[] DOComment { get; set; }

    public string[] RIComment { get; set; }

    public string[] ROComment { get; set; }

    public string[] UIComment { get; set; }

    public string[] UOComment { get; set; }

    public string[] SIComment { get; set; }

    public string[] SOComment { get; set; }

    public string[] WIComment { get; set; }

    public string[] WOComment { get; set; }

    public string[] WSIComment { get; set; }

    public string[] AIComment { get; set; }

    public string[] AOComment { get; set; }

    public string[] GIComment { get; set; }

    public string[] GOComment { get; set; }

    public string[] STRREGComment { get; set; }

    public string[] STRREG_COMMENT_Comment { get; set; }

    /// <summary>从原始的数据内容加载数据</summary>
    /// <param name="content">原始的内容</param>
    public void LoadByContent(byte[] content)
    {
      IByteTransform byteTransform = (IByteTransform) new RegularByteTransform();
      Encoding encoding;
      try
      {
        encoding = Encoding.GetEncoding("shift_jis", EncoderFallback.ReplacementFallback, (DecoderFallback) new DecoderReplacementFallback());
      }
      catch
      {
        encoding = Encoding.UTF8;
      }
      string[] fanucCmds = FanucHelper.GetFanucCmds();
      int[] numArray1 = new int[fanucCmds.Length - 1];
      int[] numArray2 = new int[fanucCmds.Length - 1];
      for (int index = 1; index < fanucCmds.Length; ++index)
      {
        MatchCollection matchCollection = Regex.Matches(fanucCmds[index], "[0-9]+");
        numArray1[index - 1] = (int.Parse(matchCollection[0].Value) - 1) * 2;
        numArray2[index - 1] = int.Parse(matchCollection[1].Value) * 2;
      }
      this.AlarmList = FanucData.GetFanucAlarmArray(byteTransform, content, numArray1[0], 5, encoding);
      this.AlarmCurrent = FanucAlarm.PraseFrom(byteTransform, content, numArray1[1], encoding);
      this.AlarmPassword = FanucAlarm.PraseFrom(byteTransform, content, numArray1[2], encoding);
      this.CurrentPose = FanucPose.PraseFrom(byteTransform, content, numArray1[3]);
      this.CurrentPoseUF = FanucPose.PraseFrom(byteTransform, content, numArray1[4]);
      this.CurrentPose2 = FanucPose.PraseFrom(byteTransform, content, numArray1[5]);
      this.CurrentPose3 = FanucPose.PraseFrom(byteTransform, content, numArray1[6]);
      this.CurrentPose4 = FanucPose.PraseFrom(byteTransform, content, numArray1[7]);
      this.CurrentPose5 = FanucPose.PraseFrom(byteTransform, content, numArray1[8]);
      this.Task = FanucTask.PraseFrom(byteTransform, content, numArray1[9], encoding);
      this.TaskIgnoreMacro = FanucTask.PraseFrom(byteTransform, content, numArray1[10], encoding);
      this.TaskIgnoreKarel = FanucTask.PraseFrom(byteTransform, content, numArray1[11], encoding);
      this.TaskIgnoreMacroKarel = FanucTask.PraseFrom(byteTransform, content, numArray1[12], encoding);
      this.PosRegGP1 = FanucData.GetFanucPoseArray(byteTransform, content, numArray1[13], 10, encoding);
      this.PosRegGP2 = FanucData.GetFanucPoseArray(byteTransform, content, numArray1[14], 4, encoding);
      this.PosRegGP3 = FanucData.GetFanucPoseArray(byteTransform, content, numArray1[15], 10, encoding);
      this.PosRegGP4 = FanucData.GetFanucPoseArray(byteTransform, content, numArray1[16], 10, encoding);
      this.PosRegGP5 = FanucData.GetFanucPoseArray(byteTransform, content, numArray1[17], 10, encoding);
      this.FAST_CLOCK = BitConverter.ToInt32(content, numArray1[18]);
      this.Timer10_TIMER_VAL = BitConverter.ToInt32(content, numArray1[19]);
      this.MOR_GRP_CURRENT_ANG = BitConverter.ToSingle(content, numArray1[20]);
      this.DUTY_TEMP = BitConverter.ToSingle(content, numArray1[21]);
      this.TIMER10_COMMENT = encoding.GetString(content, numArray1[22], 80).Trim(new char[1]);
      this.TIMER2_COMMENT = encoding.GetString(content, numArray1[23], 80).Trim(new char[1]);
      this.MNUTOOL1_1 = FanucPose.PraseFrom(byteTransform, content, numArray1[24]);
      this.HTTPKCL_CMDS = encoding.GetString(content, numArray1[25], 80).Trim(new char[1]);
      this.NumReg1 = byteTransform.TransInt32(content, numArray1[26], 5);
      this.NumReg2 = byteTransform.TransSingle(content, numArray1[27], 5);
      this.DataPosRegMG = new FanucPose[10];
      for (int index = 0; index < this.DataPosRegMG.Length; ++index)
      {
        this.DataPosRegMG[index] = new FanucPose();
        this.DataPosRegMG[index].Xyzwpr = byteTransform.TransSingle(content, numArray1[29] + index * 50, 9);
        this.DataPosRegMG[index].Config = FanucPose.TransConfigStringArray(byteTransform.TransInt16(content, numArray1[29] + 36 + index * 50, 7));
        this.DataPosRegMG[index].Joint = byteTransform.TransSingle(content, numArray1[30] + index * 36, 9);
      }
      this.DIComment = FanucData.GetStringArray(content, numArray1[31], 80, 3, encoding);
      this.DOComment = FanucData.GetStringArray(content, numArray1[32], 80, 3, encoding);
      this.RIComment = FanucData.GetStringArray(content, numArray1[33], 80, 3, encoding);
      this.ROComment = FanucData.GetStringArray(content, numArray1[34], 80, 3, encoding);
      this.UIComment = FanucData.GetStringArray(content, numArray1[35], 80, 3, encoding);
      this.UOComment = FanucData.GetStringArray(content, numArray1[36], 80, 3, encoding);
      this.SIComment = FanucData.GetStringArray(content, numArray1[37], 80, 3, encoding);
      this.SOComment = FanucData.GetStringArray(content, numArray1[38], 80, 3, encoding);
      this.WIComment = FanucData.GetStringArray(content, numArray1[39], 80, 3, encoding);
      this.WOComment = FanucData.GetStringArray(content, numArray1[40], 80, 3, encoding);
      this.WSIComment = FanucData.GetStringArray(content, numArray1[41], 80, 3, encoding);
      this.AIComment = FanucData.GetStringArray(content, numArray1[42], 80, 3, encoding);
      this.AOComment = FanucData.GetStringArray(content, numArray1[43], 80, 3, encoding);
      this.GIComment = FanucData.GetStringArray(content, numArray1[44], 80, 3, encoding);
      this.GOComment = FanucData.GetStringArray(content, numArray1[45], 80, 3, encoding);
      this.STRREGComment = FanucData.GetStringArray(content, numArray1[46], 80, 3, encoding);
      this.STRREG_COMMENT_Comment = FanucData.GetStringArray(content, numArray1[47], 80, 3, encoding);
      this.isIni = true;
    }

    /// <inheritdoc />
    public override string ToString()
    {
      if (!this.isIni)
        return "NULL";
      StringBuilder sb1 = new StringBuilder();
      FanucData.AppendStringBuilder(sb1, "AlarmList", ((IEnumerable<FanucAlarm>) this.AlarmList).Select<FanucAlarm, string>((Func<FanucAlarm, string>) (m => m.ToString())).ToArray<string>());
      FanucData.AppendStringBuilder(sb1, "AlarmCurrent", this.AlarmCurrent.ToString());
      FanucData.AppendStringBuilder(sb1, "AlarmPassword", this.AlarmPassword.ToString());
      FanucData.AppendStringBuilder(sb1, "CurrentPose", this.CurrentPose.ToString());
      FanucData.AppendStringBuilder(sb1, "CurrentPoseUF", this.CurrentPoseUF.ToString());
      FanucData.AppendStringBuilder(sb1, "CurrentPose2", this.CurrentPose2.ToString());
      FanucData.AppendStringBuilder(sb1, "CurrentPose3", this.CurrentPose3.ToString());
      FanucData.AppendStringBuilder(sb1, "CurrentPose4", this.CurrentPose4.ToString());
      FanucData.AppendStringBuilder(sb1, "CurrentPose5", this.CurrentPose5.ToString());
      FanucData.AppendStringBuilder(sb1, "Task", this.Task.ToString());
      FanucData.AppendStringBuilder(sb1, "TaskIgnoreMacro", this.TaskIgnoreMacro.ToString());
      FanucData.AppendStringBuilder(sb1, "TaskIgnoreKarel", this.TaskIgnoreKarel.ToString());
      FanucData.AppendStringBuilder(sb1, "TaskIgnoreMacroKarel", this.TaskIgnoreMacroKarel.ToString());
      FanucData.AppendStringBuilder(sb1, "PosRegGP1", ((IEnumerable<FanucPose>) this.PosRegGP1).Select<FanucPose, string>((Func<FanucPose, string>) (m => m.ToString())).ToArray<string>());
      FanucData.AppendStringBuilder(sb1, "PosRegGP2", ((IEnumerable<FanucPose>) this.PosRegGP2).Select<FanucPose, string>((Func<FanucPose, string>) (m => m.ToString())).ToArray<string>());
      FanucData.AppendStringBuilder(sb1, "PosRegGP3", ((IEnumerable<FanucPose>) this.PosRegGP3).Select<FanucPose, string>((Func<FanucPose, string>) (m => m.ToString())).ToArray<string>());
      FanucData.AppendStringBuilder(sb1, "PosRegGP4", ((IEnumerable<FanucPose>) this.PosRegGP4).Select<FanucPose, string>((Func<FanucPose, string>) (m => m.ToString())).ToArray<string>());
      FanucData.AppendStringBuilder(sb1, "PosRegGP5", ((IEnumerable<FanucPose>) this.PosRegGP5).Select<FanucPose, string>((Func<FanucPose, string>) (m => m.ToString())).ToArray<string>());
      StringBuilder sb2 = sb1;
      int num = this.FAST_CLOCK;
      string str1 = num.ToString();
      FanucData.AppendStringBuilder(sb2, "FAST_CLOCK", str1);
      StringBuilder sb3 = sb1;
      num = this.Timer10_TIMER_VAL;
      string str2 = num.ToString();
      FanucData.AppendStringBuilder(sb3, "Timer10_TIMER_VAL", str2);
      FanucData.AppendStringBuilder(sb1, "MOR_GRP_CURRENT_ANG", this.MOR_GRP_CURRENT_ANG.ToString());
      FanucData.AppendStringBuilder(sb1, "DUTY_TEMP", this.DUTY_TEMP.ToString());
      FanucData.AppendStringBuilder(sb1, "TIMER10_COMMENT", this.TIMER10_COMMENT.ToString());
      FanucData.AppendStringBuilder(sb1, "TIMER2_COMMENT", this.TIMER2_COMMENT.ToString());
      FanucData.AppendStringBuilder(sb1, "MNUTOOL1_1", this.MNUTOOL1_1.ToString());
      FanucData.AppendStringBuilder(sb1, "HTTPKCL_CMDS", this.HTTPKCL_CMDS.ToString());
      FanucData.AppendStringBuilder(sb1, "NumReg1", SoftBasic.ArrayFormat<int>(this.NumReg1));
      FanucData.AppendStringBuilder(sb1, "NumReg2", SoftBasic.ArrayFormat<float>(this.NumReg2));
      FanucData.AppendStringBuilder(sb1, "DataPosRegMG", ((IEnumerable<FanucPose>) this.DataPosRegMG).Select<FanucPose, string>((Func<FanucPose, string>) (m => m.ToString())).ToArray<string>());
      FanucData.AppendStringBuilder(sb1, "DIComment", SoftBasic.ArrayFormat<string>(this.DIComment));
      FanucData.AppendStringBuilder(sb1, "DOComment", SoftBasic.ArrayFormat<string>(this.DOComment));
      FanucData.AppendStringBuilder(sb1, "RIComment", SoftBasic.ArrayFormat<string>(this.RIComment));
      FanucData.AppendStringBuilder(sb1, "ROComment", SoftBasic.ArrayFormat<string>(this.ROComment));
      FanucData.AppendStringBuilder(sb1, "UIComment", SoftBasic.ArrayFormat<string>(this.UIComment));
      FanucData.AppendStringBuilder(sb1, "UOComment", SoftBasic.ArrayFormat<string>(this.UOComment));
      FanucData.AppendStringBuilder(sb1, "SIComment", SoftBasic.ArrayFormat<string>(this.SIComment));
      FanucData.AppendStringBuilder(sb1, "SOComment", SoftBasic.ArrayFormat<string>(this.SOComment));
      FanucData.AppendStringBuilder(sb1, "WIComment", SoftBasic.ArrayFormat<string>(this.WIComment));
      FanucData.AppendStringBuilder(sb1, "WOComment", SoftBasic.ArrayFormat<string>(this.WOComment));
      FanucData.AppendStringBuilder(sb1, "WSIComment", SoftBasic.ArrayFormat<string>(this.WSIComment));
      FanucData.AppendStringBuilder(sb1, "AIComment", SoftBasic.ArrayFormat<string>(this.AIComment));
      FanucData.AppendStringBuilder(sb1, "AOComment", SoftBasic.ArrayFormat<string>(this.AOComment));
      FanucData.AppendStringBuilder(sb1, "GIComment", SoftBasic.ArrayFormat<string>(this.GIComment));
      FanucData.AppendStringBuilder(sb1, "GOComment", SoftBasic.ArrayFormat<string>(this.GOComment));
      FanucData.AppendStringBuilder(sb1, "STRREGComment", SoftBasic.ArrayFormat<string>(this.STRREGComment));
      FanucData.AppendStringBuilder(sb1, "STRREG_COMMENT_Comment", SoftBasic.ArrayFormat<string>(this.STRREG_COMMENT_Comment));
      return sb1.ToString();
    }

    /// <summary>从字节数组解析出fanuc的数据信息</summary>
    /// <param name="content">原始的字节数组</param>
    /// <returns>fanuc数据</returns>
    public static OperateResult<FanucData> PraseFrom(byte[] content)
    {
      FanucData fanucData = new FanucData();
      fanucData.LoadByContent(content);
      return OperateResult.CreateSuccessResult<FanucData>(fanucData);
    }

    private static void AppendStringBuilder(StringBuilder sb, string name, string value) => FanucData.AppendStringBuilder(sb, name, new string[1]
    {
      value
    });

    private static void AppendStringBuilder(StringBuilder sb, string name, string[] values)
    {
      sb.Append(name);
      sb.Append(":");
      if (values.Length > 1)
        sb.Append(Environment.NewLine);
      for (int index = 0; index < values.Length; ++index)
      {
        sb.Append(values[index]);
        sb.Append(Environment.NewLine);
      }
      if (values.Length <= 1)
        return;
      sb.Append(Environment.NewLine);
    }

    private static string[] GetStringArray(
      byte[] content,
      int index,
      int length,
      int arraySize,
      Encoding encoding)
    {
      string[] strArray = new string[arraySize];
      for (int index1 = 0; index1 < arraySize; ++index1)
        strArray[index1] = encoding.GetString(content, index + length * index1, length).TrimEnd(new char[1]);
      return strArray;
    }

    private static FanucPose[] GetFanucPoseArray(
      IByteTransform byteTransform,
      byte[] content,
      int index,
      int arraySize,
      Encoding encoding)
    {
      FanucPose[] fanucPoseArray = new FanucPose[arraySize];
      for (int index1 = 0; index1 < arraySize; ++index1)
        fanucPoseArray[index1] = FanucPose.PraseFrom(byteTransform, content, index + index1 * 100);
      return fanucPoseArray;
    }

    private static FanucAlarm[] GetFanucAlarmArray(
      IByteTransform byteTransform,
      byte[] content,
      int index,
      int arraySize,
      Encoding encoding)
    {
      FanucAlarm[] fanucAlarmArray = new FanucAlarm[arraySize];
      for (int index1 = 0; index1 < arraySize; ++index1)
        fanucAlarmArray[index1] = FanucAlarm.PraseFrom(byteTransform, content, index + 200 * index1, encoding);
      return fanucAlarmArray;
    }
  }
}
