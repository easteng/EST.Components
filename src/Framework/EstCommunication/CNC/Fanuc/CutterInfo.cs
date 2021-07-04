// Decompiled with JetBrains decompiler
// Type: EstCommunication.CNC.Fanuc.CutterInfo
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

namespace EstCommunication.CNC.Fanuc
{
  /// <summary>刀具信息</summary>
  public class CutterInfo
  {
    /// <summary>长度形状补偿</summary>
    public double LengthSharpOffset { get; set; }

    /// <summary>长度磨损补偿</summary>
    public double LengthWearOffset { get; set; }

    /// <summary>半径形状补偿</summary>
    public double RadiusSharpOffset { get; set; }

    /// <summary>半径磨损补偿</summary>
    public double RadiusWearOffset { get; set; }

    /// <inheritdoc />
    public override string ToString() => string.Format("LengthSharpOffset:{0:10} LengthWearOffset:{1:10} RadiusSharpOffset:{2:10} RadiusWearOffset:{3:10}", (object) this.LengthSharpOffset, (object) this.LengthWearOffset, (object) this.RadiusSharpOffset, (object) this.RadiusWearOffset);
  }
}
