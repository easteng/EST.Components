// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.Geniitek.VibrationSensorPeekValue
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

namespace EstCommunication.Profinet.Geniitek
{
  /// <summary>振动传感器的峰值数据类</summary>
  public class VibrationSensorPeekValue
  {
    /// <summary>X轴的加速度，单位 m/s2</summary>
    public float AcceleratedSpeedX { get; set; }

    /// <summary>Y轴的加速度，单位 m/s2</summary>
    public float AcceleratedSpeedY { get; set; }

    /// <summary>Z轴的加速度，单位 m/s2</summary>
    public float AcceleratedSpeedZ { get; set; }

    /// <summary>X轴的速度，单位 mm/s</summary>
    public float SpeedX { get; set; }

    /// <summary>Y轴的速度，单位 mm/s</summary>
    public float SpeedY { get; set; }

    /// <summary>Z轴的速度，单位 mm/s</summary>
    public float SpeedZ { get; set; }

    /// <summary>X轴的位置，单位 um</summary>
    public int OffsetX { get; set; }

    /// <summary>Y轴的位移，单位 um</summary>
    public int OffsetY { get; set; }

    /// <summary>Z轴的位移，单位 um</summary>
    public int OffsetZ { get; set; }

    /// <summary>温度，单位 摄氏度</summary>
    public float Temperature { get; set; }

    /// <summary>电压，单位 伏特</summary>
    public float Voltage { get; set; }

    /// <summary>数据的发送间隔，单位秒</summary>
    public int SendingInterval { get; set; }
  }
}
