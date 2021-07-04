// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.AllenBradley.AbStructHandle
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

namespace EstCommunication.Profinet.AllenBradley
{
  /// <summary>结构体的句柄信息</summary>
  public class AbStructHandle
  {
    /// <summary>返回项数</summary>
    public ushort Count { get; set; }

    /// <summary>结构体定义大小</summary>
    public uint TemplateObjectDefinitionSize { get; set; }

    /// <summary>使用读取标记服务读取结构时在线路上传输的字节数</summary>
    public uint TemplateStructureSize { get; set; }

    /// <summary>成员数量</summary>
    public ushort MemberCount { get; set; }

    /// <summary>结构体的handle</summary>
    public ushort StructureHandle { get; set; }
  }
}
