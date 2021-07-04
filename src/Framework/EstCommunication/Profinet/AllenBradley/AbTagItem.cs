// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.AllenBradley.AbTagItem
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

namespace EstCommunication.Profinet.AllenBradley
{
  /// <summary>AB PLC的每个的数据标签情况</summary>
  public class AbTagItem
  {
    private ushort symbolType = 0;

    /// <summary>实例ID</summary>
    public uint InstanceID { get; set; }

    /// <summary>名字</summary>
    public string Name { get; set; }

    /// <summary>TAG类型（有时候可作为实例ID使用）</summary>
    public ushort SymbolType
    {
      get => this.symbolType;
      set
      {
        this.symbolType = value;
        this.ArrayDimension = ((int) this.symbolType & 16384) == 16384 ? 2 : (((int) this.symbolType & 8192) == 8192 ? 1 : 0);
        this.IsStruct = ((int) this.symbolType & 32768) == 32768;
      }
    }

    /// <summary>数据的维度信息，默认是0，标量数据，1代表1为数组</summary>
    public int ArrayDimension { get; set; }

    /// <summary>是否结构体数据</summary>
    public bool IsStruct { get; set; }
  }
}
