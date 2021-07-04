// Decompiled with JetBrains decompiler
// Type: EstCommunication.Robot.Hyundai.HyundaiData
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using System;
using System.Text;

namespace EstCommunication.Robot.Hyundai
{
  /// <summary>Hyundai的数据类对象</summary>
  public class HyundaiData
  {
    /// <summary>实例化一个默认的对象</summary>
    public HyundaiData() => this.Data = new double[6];

    /// <summary>通过缓存对象实例化一个</summary>
    /// <param name="buffer"></param>
    public HyundaiData(byte[] buffer) => this.LoadBy(buffer);

    /// <summary>命令码，从控制器发数据到PC和PC到控制器，两者的命令不一样</summary>
    public char Command { get; set; }

    /// <summary>虚标记</summary>
    public string CharDummy { get; set; }

    /// <summary>状态码</summary>
    public int State { get; set; }

    /// <summary>标记数据，从PLC发送给机器人的数据，原封不动的返回</summary>
    public int Count { get; set; }

    /// <summary>虚标记</summary>
    public int IntDummy { get; set; }

    /// <summary>关节坐标数据，包含X,Y,Z,W,P,R，三个位置数据，三个角度数据。</summary>
    public double[] Data { get; set; }

    /// <summary>从字节数组的指定索引开始加载现在机器人的数据</summary>
    /// <param name="buffer">原始的字节数据</param>
    /// <param name="index">起始的索引信息</param>
    public void LoadBy(byte[] buffer, int index = 0)
    {
      this.Command = (char) buffer[index];
      this.CharDummy = Encoding.ASCII.GetString(buffer, index + 1, 3);
      this.State = BitConverter.ToInt32(buffer, index + 4);
      this.Count = BitConverter.ToInt32(buffer, index + 8);
      this.IntDummy = BitConverter.ToInt32(buffer, index + 12);
      this.Data = new double[6];
      for (int index1 = 0; index1 < this.Data.Length; ++index1)
        this.Data[index1] = index1 >= 3 ? BitConverter.ToDouble(buffer, index + 16 + 8 * index1) * 180.0 / Math.PI : BitConverter.ToDouble(buffer, index + 16 + 8 * index1) * 1000.0;
    }

    /// <summary>将现代机器人的数据转换为字节数组</summary>
    /// <returns>字节数组</returns>
    public byte[] ToBytes()
    {
      byte[] numArray = new byte[64];
      numArray[0] = (byte) this.Command;
      if (!string.IsNullOrEmpty(this.CharDummy))
        Encoding.ASCII.GetBytes(this.CharDummy).CopyTo((Array) numArray, 1);
      BitConverter.GetBytes(this.State).CopyTo((Array) numArray, 4);
      BitConverter.GetBytes(this.Count).CopyTo((Array) numArray, 8);
      BitConverter.GetBytes(this.IntDummy).CopyTo((Array) numArray, 12);
      for (int index = 0; index < this.Data.Length; ++index)
      {
        if (index < 3)
          BitConverter.GetBytes(this.Data[index] / 1000.0).CopyTo((Array) numArray, 16 + 8 * index);
        else
          BitConverter.GetBytes(this.Data[index] * Math.PI / 180.0).CopyTo((Array) numArray, 16 + 8 * index);
      }
      return numArray;
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("HyundaiData:Cmd[{0},{1},{2},{3},{4}] Data:{5}", (object) this.Command, (object) this.CharDummy, (object) this.State, (object) this.Count, (object) this.IntDummy, (object) SoftBasic.ArrayFormat<double>(this.Data));
  }
}
