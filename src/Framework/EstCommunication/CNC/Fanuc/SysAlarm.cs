// Decompiled with JetBrains decompiler
// Type: EstCommunication.CNC.Fanuc.SysAlarm
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

namespace EstCommunication.CNC.Fanuc
{
  /// <summary>当前机床的报警信息</summary>
  public class SysAlarm
  {
    /// <summary>当前报警的ID信息</summary>
    public int AlarmId { get; set; }

    /// <summary>当前的报警类型</summary>
    public short Type { get; set; }

    /// <summary>报警的轴信息</summary>
    public short Axis { get; set; }

    /// <summary>报警的消息</summary>
    public string Message { get; set; }
  }
}
