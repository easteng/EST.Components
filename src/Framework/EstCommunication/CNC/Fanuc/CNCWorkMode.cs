// Decompiled with JetBrains decompiler
// Type: EstCommunication.CNC.Fanuc.CNCWorkMode
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

namespace EstCommunication.CNC.Fanuc
{
  /// <summary>设备的工作模式</summary>
  public enum CNCWorkMode
  {
    /// <summary>手动输入</summary>
    MDI = 0,
    /// <summary>自动循环</summary>
    AUTO = 1,
    /// <summary>程序编辑</summary>
    EDIT = 3,
    /// <summary>×100</summary>
    HANDLE = 4,
    /// <summary>连续进给</summary>
    JOG = 5,
    /// <summary>???</summary>
    TeachInJOG = 6,
    /// <summary>示教</summary>
    TeachInHandle = 7,
    /// <summary>???</summary>
    INCfeed = 8,
    /// <summary>机床回零</summary>
    REFerence = 9,
    /// <summary>???</summary>
    ReMoTe = 10, // 0x0000000A
  }
}
