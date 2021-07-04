// Decompiled with JetBrains decompiler
// Type: EstCommunication.CNC.Fanuc.SysAllCoors
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

namespace EstCommunication.CNC.Fanuc
{
  /// <summary>系统的坐标信息</summary>
  public class SysAllCoors
  {
    /// <summary>绝对坐标</summary>
    public double[] Absolute { get; set; }

    /// <summary>机械坐标</summary>
    public double[] Machine { get; set; }

    /// <summary>相对坐标</summary>
    public double[] Relative { get; set; }
  }
}
