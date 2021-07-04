// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.LSIS.FlagBit
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

namespace EstCommunication.Profinet.LSIS
{
  /// <summary>
  /// using FlagBit in Marker for Byte<br />
  /// M0.0=1;M0.1=2;M0.2=4;M0.3=8;==========================&gt;M0.7=128
  /// </summary>
  public enum FlagBit
  {
    Flag1 = 1,
    Flag2 = 2,
    Flag4 = 4,
    Flag8 = 8,
    Flag16 = 16, // 0x00000010
    Flag32 = 32, // 0x00000020
    Flag64 = 64, // 0x00000040
    Flag128 = 128, // 0x00000080
    Flag256 = 256, // 0x00000100
  }
}
