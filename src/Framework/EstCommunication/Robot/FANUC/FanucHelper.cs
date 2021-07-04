// Decompiled with JetBrains decompiler
// Type: EstCommunication.Robot.FANUC.FanucHelper
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using System;

namespace EstCommunication.Robot.FANUC
{
  /// <summary>Fanuc的辅助方法信息</summary>
  public class FanucHelper
  {
    /// <summary>Q区数据</summary>
    public const byte SELECTOR_Q = 72;
    /// <summary>I区数据</summary>
    public const byte SELECTOR_I = 70;
    /// <summary>AQ区数据</summary>
    public const byte SELECTOR_AQ = 12;
    /// <summary>AI区数据</summary>
    public const byte SELECTOR_AI = 10;
    /// <summary>M区数据</summary>
    public const byte SELECTOR_M = 76;
    /// <summary>D区数据</summary>
    public const byte SELECTOR_D = 8;
    /// <summary>命令数据</summary>
    public const byte SELECTOR_G = 56;

    /// <summary>
    /// 从FANUC机器人地址进行解析数据信息，地址为D,I,Q,M,AI,AQ区<br />
    /// Parse data information from FANUC robot address, the address is D, I, Q, M, AI, AQ area
    /// </summary>
    /// <param name="address">fanuc机器人的地址信息</param>
    /// <returns>解析结果</returns>
    public static OperateResult<byte, ushort> AnalysisFanucAddress(string address)
    {
      try
      {
        if (address.StartsWith("aq") || address.StartsWith("AQ"))
          return OperateResult.CreateSuccessResult<byte, ushort>((byte) 12, ushort.Parse(address.Substring(2)));
        if (address.StartsWith("ai") || address.StartsWith("AI"))
          return OperateResult.CreateSuccessResult<byte, ushort>((byte) 10, ushort.Parse(address.Substring(2)));
        if (address.StartsWith("i") || address.StartsWith("I"))
          return OperateResult.CreateSuccessResult<byte, ushort>((byte) 70, ushort.Parse(address.Substring(1)));
        if (address.StartsWith("q") || address.StartsWith("Q"))
          return OperateResult.CreateSuccessResult<byte, ushort>((byte) 72, ushort.Parse(address.Substring(1)));
        if (address.StartsWith("m") || address.StartsWith("M"))
          return OperateResult.CreateSuccessResult<byte, ushort>((byte) 76, ushort.Parse(address.Substring(1)));
        return address.StartsWith("d") || address.StartsWith("D") ? OperateResult.CreateSuccessResult<byte, ushort>((byte) 8, ushort.Parse(address.Substring(1))) : new OperateResult<byte, ushort>(StringResources.Language.NotSupportedDataType);
      }
      catch (Exception ex)
      {
        return new OperateResult<byte, ushort>(ex.Message);
      }
    }

    /// <summary>构建读取数据的报文内容</summary>
    /// <param name="sel">数据类别</param>
    /// <param name="address">偏移地址</param>
    /// <param name="length">长度</param>
    /// <returns>报文内容</returns>
    public static byte[] BulidReadData(byte sel, ushort address, ushort length) => new byte[56]
    {
      (byte) 2,
      (byte) 0,
      (byte) 6,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 6,
      (byte) 192,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 16,
      (byte) 14,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 1,
      (byte) 4,
      sel,
      BitConverter.GetBytes((int) address - 1)[0],
      BitConverter.GetBytes((int) address - 1)[1],
      BitConverter.GetBytes(length)[0],
      BitConverter.GetBytes(length)[1],
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0
    };

    /// <summary>构建读取返回的数据信息</summary>
    /// <param name="data">数据</param>
    /// <returns>结果</returns>
    public static byte[] BuildReadResponseData(byte[] data)
    {
      byte[] bytes = SoftBasic.HexStringToBytes("\r\n03 00 06 00 e4 2f 00 00 00 01 00 00 00 00 00 00\r\n00 01 00 00 00 00 00 00 00 00 00 00 00 00 06 94\r\n10 0e 00 00 30 3a 00 00 01 01 00 00 00 00 00 00\r\n01 01 ff 04 00 00 7c 21");
      if (data.Length > 6)
      {
        byte[] numArray = SoftBasic.SpliceArray<byte>(bytes, data);
        numArray[4] = BitConverter.GetBytes(data.Length)[0];
        numArray[5] = BitConverter.GetBytes(data.Length)[1];
        return numArray;
      }
      bytes[4] = (byte) 0;
      bytes[5] = (byte) 0;
      bytes[31] = (byte) 212;
      data.CopyTo((Array) bytes, 44);
      return bytes;
    }

    /// <summary>构建写入的数据报文，需要指定相关的参数信息</summary>
    /// <param name="sel">数据类别</param>
    /// <param name="address">偏移地址</param>
    /// <param name="value">原始数据内容</param>
    /// <param name="length">写入的数据长度</param>
    /// <returns>报文内容</returns>
    public static byte[] BuildWriteData(byte sel, ushort address, byte[] value, int length)
    {
      if (value == null)
        value = new byte[0];
      if (value.Length > 6)
      {
        byte[] numArray = new byte[56 + value.Length];
        new byte[56]
        {
          (byte) 2,
          (byte) 0,
          (byte) 9,
          (byte) 0,
          (byte) 50,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 2,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 2,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 9,
          (byte) 128,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 16,
          (byte) 14,
          (byte) 0,
          (byte) 0,
          (byte) 1,
          (byte) 1,
          (byte) 50,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 0,
          (byte) 1,
          (byte) 1,
          (byte) 7,
          (byte) 8,
          (byte) 49,
          (byte) 0,
          (byte) 25,
          (byte) 0
        }.CopyTo((Array) numArray, 0);
        value.CopyTo((Array) numArray, 56);
        numArray[4] = BitConverter.GetBytes(value.Length)[0];
        numArray[5] = BitConverter.GetBytes(value.Length)[1];
        numArray[51] = sel;
        numArray[52] = BitConverter.GetBytes((int) address - 1)[0];
        numArray[53] = BitConverter.GetBytes((int) address - 1)[1];
        numArray[54] = BitConverter.GetBytes(length)[0];
        numArray[55] = BitConverter.GetBytes(length)[1];
        return numArray;
      }
      byte[] numArray1 = new byte[56]
      {
        (byte) 2,
        (byte) 0,
        (byte) 8,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 8,
        (byte) 192,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 0,
        (byte) 16,
        (byte) 14,
        (byte) 0,
        (byte) 0,
        (byte) 1,
        (byte) 1,
        (byte) 7,
        sel,
        BitConverter.GetBytes((int) address - 1)[0],
        BitConverter.GetBytes((int) address - 1)[1],
        BitConverter.GetBytes(length)[0],
        BitConverter.GetBytes(length)[1],
        (byte) 1,
        (byte) 0,
        (byte) 2,
        (byte) 0,
        (byte) 3,
        (byte) 0,
        (byte) 4,
        (byte) 0
      };
      value.CopyTo((Array) numArray1, 48);
      return numArray1;
    }

    /// <summary>
    /// 获取所有的命令信息<br />
    /// Get all command information
    /// </summary>
    /// <returns>命令数组</returns>
    public static string[] GetFanucCmds() => new string[60]
    {
      "CLRASG",
      "SETASG 1 500 ALM[E1] 1",
      "SETASG 501 100 ALM[1] 1",
      "SETASG 601 100 ALM[P1] 1",
      "SETASG 701 50 POS[15] 0.0",
      "SETASG 751 50 POS[15] 0.0",
      "SETASG 801 50 POS[G2: 15] 0.0",
      "SETASG 851 50 POS[G3: 0] 0.0",
      "SETASG 901 50 POS[G4:0] 0.0",
      "SETASG 951 50 POS[G5:0] 0.0",
      "SETASG 1001 18 PRG[1] 1",
      "SETASG 1019 18 PRG[M1] 1",
      "SETASG 1037 18 PRG[K1] 1",
      "SETASG 1055 18 PRG[MK1] 1",
      "SETASG 1073 500 PR[1] 0.0",
      "SETASG 1573 200 PR[G2:1] 0.0",
      "SETASG 1773 500 PR[G3:1] 0.0",
      "SETASG 2273 500 PR[G4: 1] 0.0",
      "SETASG 2773 500 PR[G5: 1] 0.0",
      "SETASG 3273 2 $FAST_CLOCK 1",
      "SETASG 3275 2 $TIMER[10].$TIMER_VAL 1",
      "SETASG 3277 2 $MOR_GRP[1].$CURRENT_ANG[1] 0",
      "SETASG 3279 2 $DUTY_TEMP 0",
      "SETASG 3281 40 $TIMER[10].$COMMENT 1",
      "SETASG 3321 40 $TIMER[2].$COMMENT 1",
      "SETASG 3361 50 $MNUTOOL[1,1] 0.0",
      "SETASG 3411 40 $[HTTPKCL]CMDS[1] 1",
      "SETASG 3451 10 R[1] 1.0",
      "SETASG 3461 10 R[6] 0",
      "SETASG 3471 250 PR[1]@1.25 0.0",
      "SETASG 3721 250 PR[1]@1.25 0.0",
      "SETASG 3971 120 PR[G2:1]@27.12 0.0",
      "SETASG 4091 120 DI[C1] 1",
      "SETASG 4211 120 DO[C1] 1",
      "SETASG 4331 120 RI[C1] 1",
      "SETASG 4451 120 RO[C1] 1",
      "SETASG 4571 120 UI[C1] 1",
      "SETASG 4691 120 UO[C1] 1",
      "SETASG 4811 120 SI[C1] 1",
      "SETASG 4931 120 SO[C1] 1",
      "SETASG 5051 120 WI[C1] 1",
      "SETASG 5171 120 WO[C1] 1",
      "SETASG 5291 120 WSI[C1] 1",
      "SETASG 5411 120 AI[C1] 1",
      "SETASG 5531 120 AO[C1] 1",
      "SETASG 5651 120 GI[C1] 1",
      "SETASG 5771 120 GO[C1] 1",
      "SETASG 5891 120 SR[1] 1",
      "SETASG 6011 120 SR[C1] 1",
      "SETASG 6131 10 R[1] 1.0",
      "SETASG 6141 2 $TIMER[1].$TIMER_VAL 1",
      "SETASG 6143 2 $TIMER[2].$TIMER_VAL 1",
      "SETASG 6145 2 $TIMER[3].$TIMER_VAL 1",
      "SETASG 6147 2 $TIMER[4].$TIMER_VAL 1",
      "SETASG 6149 2 $TIMER[5].$TIMER_VAL 1",
      "SETASG 6151 2 $TIMER[6].$TIMER_VAL 1",
      "SETASG 6153 2 $TIMER[7].$TIMER_VAL 1",
      "SETASG 6155 2 $TIMER[8].$TIMER_VAL 1",
      "SETASG 6157 2 $TIMER[9].$TIMER_VAL 1",
      "SETASG 6159 2 $TIMER[10].$TIMER_VAL 1"
    };
  }
}
