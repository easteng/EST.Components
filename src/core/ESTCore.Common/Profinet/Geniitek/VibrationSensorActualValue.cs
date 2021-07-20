// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Geniitek.VibrationSensorActualValue
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

namespace ESTCore.Common.Profinet.Geniitek
{
    /// <summary>振动传感器的加速度值</summary>
    public struct VibrationSensorActualValue
    {
        /// <summary>X轴的实时加速度</summary>
        public float AcceleratedSpeedX { get; set; }

        /// <summary>Y轴的实时加速度</summary>
        public float AcceleratedSpeedY { get; set; }

        /// <summary>Z轴的实时加速度</summary>
        public float AcceleratedSpeedZ { get; set; }

        /// <inheritdoc />
        public override string ToString() => string.Format("ActualValue[{0},{1},{2}]", (object)this.AcceleratedSpeedX, (object)this.AcceleratedSpeedY, (object)this.AcceleratedSpeedZ);
    }
}
