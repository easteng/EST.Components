// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.Keyence.KeyenceDataType
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

namespace EstCommunication.Profinet.Keyence
{
  /// <summary>Keyence PLC的数据类型，此处包含了几个常用的类型</summary>
  public class KeyenceDataType
  {
    /// <summary>X输入继电器</summary>
    public static readonly KeyenceDataType X = new KeyenceDataType((byte) 156, (byte) 1, "X*", 16);
    /// <summary>Y输出继电器</summary>
    public static readonly KeyenceDataType Y = new KeyenceDataType((byte) 157, (byte) 1, "Y*", 16);
    /// <summary>链接继电器</summary>
    public static readonly KeyenceDataType B = new KeyenceDataType((byte) 160, (byte) 1, "B*", 16);
    /// <summary>内部辅助继电器</summary>
    public static readonly KeyenceDataType M = new KeyenceDataType((byte) 144, (byte) 1, "M*", 10);
    /// <summary>锁存继电器</summary>
    public static readonly KeyenceDataType L = new KeyenceDataType((byte) 146, (byte) 1, "L*", 10);
    /// <summary>控制继电器</summary>
    public static readonly KeyenceDataType SM = new KeyenceDataType((byte) 145, (byte) 1, nameof (SM), 10);
    /// <summary>控制存储器</summary>
    public static readonly KeyenceDataType SD = new KeyenceDataType((byte) 169, (byte) 0, nameof (SD), 10);
    /// <summary>数据存储器</summary>
    public static readonly KeyenceDataType D = new KeyenceDataType((byte) 168, (byte) 0, "D*", 10);
    /// <summary>文件寄存器</summary>
    public static readonly KeyenceDataType R = new KeyenceDataType((byte) 175, (byte) 0, "R*", 10);
    /// <summary>文件寄存器</summary>
    public static readonly KeyenceDataType ZR = new KeyenceDataType((byte) 176, (byte) 0, nameof (ZR), 16);
    /// <summary>链路寄存器</summary>
    public static readonly KeyenceDataType W = new KeyenceDataType((byte) 180, (byte) 0, "W*", 16);
    /// <summary>计时器（当前值）</summary>
    public static readonly KeyenceDataType TN = new KeyenceDataType((byte) 194, (byte) 0, nameof (TN), 10);
    /// <summary>计时器（接点）</summary>
    public static readonly KeyenceDataType TS = new KeyenceDataType((byte) 193, (byte) 1, nameof (TS), 10);
    /// <summary>计数器（当前值）</summary>
    public static readonly KeyenceDataType CN = new KeyenceDataType((byte) 197, (byte) 0, nameof (CN), 10);
    /// <summary>计数器（接点）</summary>
    public static readonly KeyenceDataType CS = new KeyenceDataType((byte) 196, (byte) 1, nameof (CS), 10);

    /// <summary>如果您清楚类型代号，可以根据值进行扩展</summary>
    /// <param name="code">数据类型的代号</param>
    /// <param name="type">0或1，默认为0</param>
    /// <param name="asciiCode">ASCII格式的类型信息</param>
    /// <param name="fromBase">指示地址的多少进制的，10或是16</param>
    public KeyenceDataType(byte code, byte type, string asciiCode, int fromBase)
    {
      this.DataCode = code;
      this.AsciiCode = asciiCode;
      this.FromBase = fromBase;
      if (type >= (byte) 2)
        return;
      this.DataType = type;
    }

    /// <summary>类型的代号值</summary>
    public byte DataCode { get; private set; } = 0;

    /// <summary>数据的类型，0代表按字，1代表按位</summary>
    public byte DataType { get; private set; } = 0;

    /// <summary>当以ASCII格式通讯时的类型描述</summary>
    public string AsciiCode { get; private set; }

    /// <summary>指示地址是10进制，还是16进制的</summary>
    public int FromBase { get; private set; }
  }
}
