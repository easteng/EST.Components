// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.Melsec.MelsecMcDataType
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

namespace EstCommunication.Profinet.Melsec
{
  /// <summary>
  /// 三菱PLC的数据类型，此处包含了几个常用的类型<br />
  /// Data types of Mitsubishi PLC, here contains several commonly used types
  /// </summary>
  public class MelsecMcDataType
  {
    /// <summary>X输入继电器</summary>
    public static readonly MelsecMcDataType X = new MelsecMcDataType((byte) 156, (byte) 1, "X*", 16);
    /// <summary>Y输出继电器</summary>
    public static readonly MelsecMcDataType Y = new MelsecMcDataType((byte) 157, (byte) 1, "Y*", 16);
    /// <summary>M内部继电器</summary>
    public static readonly MelsecMcDataType M = new MelsecMcDataType((byte) 144, (byte) 1, "M*", 10);
    /// <summary>SM特殊继电器</summary>
    public static readonly MelsecMcDataType SM = new MelsecMcDataType((byte) 145, (byte) 1, nameof (SM), 10);
    /// <summary>S步进继电器</summary>
    public static readonly MelsecMcDataType S = new MelsecMcDataType((byte) 152, (byte) 1, "S*", 10);
    /// <summary>L锁存继电器</summary>
    public static readonly MelsecMcDataType L = new MelsecMcDataType((byte) 146, (byte) 1, "L*", 10);
    /// <summary>F报警器</summary>
    public static readonly MelsecMcDataType F = new MelsecMcDataType((byte) 147, (byte) 1, "F*", 10);
    /// <summary>V边沿继电器</summary>
    public static readonly MelsecMcDataType V = new MelsecMcDataType((byte) 148, (byte) 1, "V*", 10);
    /// <summary>B链接继电器</summary>
    public static readonly MelsecMcDataType B = new MelsecMcDataType((byte) 160, (byte) 1, "B*", 16);
    /// <summary>SB特殊链接继电器</summary>
    public static readonly MelsecMcDataType SB = new MelsecMcDataType((byte) 161, (byte) 1, nameof (SB), 16);
    /// <summary>DX直接访问输入</summary>
    public static readonly MelsecMcDataType DX = new MelsecMcDataType((byte) 162, (byte) 1, nameof (DX), 16);
    /// <summary>DY直接访问输出</summary>
    public static readonly MelsecMcDataType DY = new MelsecMcDataType((byte) 163, (byte) 1, nameof (DY), 16);
    /// <summary>D数据寄存器</summary>
    public static readonly MelsecMcDataType D = new MelsecMcDataType((byte) 168, (byte) 0, "D*", 10);
    /// <summary>特殊链接存储器</summary>
    public static readonly MelsecMcDataType SD = new MelsecMcDataType((byte) 169, (byte) 0, nameof (SD), 10);
    /// <summary>W链接寄存器</summary>
    public static readonly MelsecMcDataType W = new MelsecMcDataType((byte) 180, (byte) 0, "W*", 16);
    /// <summary>SW特殊链接寄存器</summary>
    public static readonly MelsecMcDataType SW = new MelsecMcDataType((byte) 181, (byte) 0, nameof (SW), 16);
    /// <summary>R文件寄存器</summary>
    public static readonly MelsecMcDataType R = new MelsecMcDataType((byte) 175, (byte) 0, "R*", 10);
    /// <summary>变址寄存器</summary>
    public static readonly MelsecMcDataType Z = new MelsecMcDataType((byte) 204, (byte) 0, "Z*", 10);
    /// <summary>文件寄存器ZR区</summary>
    public static readonly MelsecMcDataType ZR = new MelsecMcDataType((byte) 176, (byte) 0, nameof (ZR), 10);
    /// <summary>定时器的当前值</summary>
    public static readonly MelsecMcDataType TN = new MelsecMcDataType((byte) 194, (byte) 0, nameof (TN), 10);
    /// <summary>定时器的触点</summary>
    public static readonly MelsecMcDataType TS = new MelsecMcDataType((byte) 193, (byte) 1, nameof (TS), 10);
    /// <summary>定时器的线圈</summary>
    public static readonly MelsecMcDataType TC = new MelsecMcDataType((byte) 192, (byte) 1, nameof (TC), 10);
    /// <summary>累计定时器的触点</summary>
    public static readonly MelsecMcDataType SS = new MelsecMcDataType((byte) 199, (byte) 1, nameof (SS), 10);
    /// <summary>累计定时器的线圈</summary>
    public static readonly MelsecMcDataType SC = new MelsecMcDataType((byte) 198, (byte) 1, nameof (SC), 10);
    /// <summary>累计定时器的当前值</summary>
    public static readonly MelsecMcDataType SN = new MelsecMcDataType((byte) 200, (byte) 0, nameof (SN), 100);
    /// <summary>计数器的当前值</summary>
    public static readonly MelsecMcDataType CN = new MelsecMcDataType((byte) 197, (byte) 0, nameof (CN), 10);
    /// <summary>计数器的触点</summary>
    public static readonly MelsecMcDataType CS = new MelsecMcDataType((byte) 196, (byte) 1, nameof (CS), 10);
    /// <summary>计数器的线圈</summary>
    public static readonly MelsecMcDataType CC = new MelsecMcDataType((byte) 195, (byte) 1, nameof (CC), 10);
    /// <summary>X输入继电器</summary>
    public static readonly MelsecMcDataType Keyence_X = new MelsecMcDataType((byte) 156, (byte) 1, "X*", 16);
    /// <summary>Y输出继电器</summary>
    public static readonly MelsecMcDataType Keyence_Y = new MelsecMcDataType((byte) 157, (byte) 1, "Y*", 16);
    /// <summary>链接继电器</summary>
    public static readonly MelsecMcDataType Keyence_B = new MelsecMcDataType((byte) 160, (byte) 1, "B*", 16);
    /// <summary>内部辅助继电器</summary>
    public static readonly MelsecMcDataType Keyence_M = new MelsecMcDataType((byte) 144, (byte) 1, "M*", 10);
    /// <summary>锁存继电器</summary>
    public static readonly MelsecMcDataType Keyence_L = new MelsecMcDataType((byte) 146, (byte) 1, "L*", 10);
    /// <summary>控制继电器</summary>
    public static readonly MelsecMcDataType Keyence_SM = new MelsecMcDataType((byte) 145, (byte) 1, nameof (SM), 10);
    /// <summary>控制存储器</summary>
    public static readonly MelsecMcDataType Keyence_SD = new MelsecMcDataType((byte) 169, (byte) 0, nameof (SD), 10);
    /// <summary>数据存储器</summary>
    public static readonly MelsecMcDataType Keyence_D = new MelsecMcDataType((byte) 168, (byte) 0, "D*", 10);
    /// <summary>文件寄存器</summary>
    public static readonly MelsecMcDataType Keyence_R = new MelsecMcDataType((byte) 175, (byte) 0, "R*", 10);
    /// <summary>文件寄存器</summary>
    public static readonly MelsecMcDataType Keyence_ZR = new MelsecMcDataType((byte) 176, (byte) 0, nameof (ZR), 10);
    /// <summary>链路寄存器</summary>
    public static readonly MelsecMcDataType Keyence_W = new MelsecMcDataType((byte) 180, (byte) 0, "W*", 16);
    /// <summary>计时器（当前值）</summary>
    public static readonly MelsecMcDataType Keyence_TN = new MelsecMcDataType((byte) 194, (byte) 0, nameof (TN), 10);
    /// <summary>计时器（接点）</summary>
    public static readonly MelsecMcDataType Keyence_TS = new MelsecMcDataType((byte) 193, (byte) 1, nameof (TS), 10);
    /// <summary>计时器（线圈）</summary>
    public static readonly MelsecMcDataType Keyence_TC = new MelsecMcDataType((byte) 192, (byte) 1, nameof (TC), 10);
    /// <summary>计数器（当前值）</summary>
    public static readonly MelsecMcDataType Keyence_CN = new MelsecMcDataType((byte) 197, (byte) 0, nameof (CN), 10);
    /// <summary>计数器（接点）</summary>
    public static readonly MelsecMcDataType Keyence_CS = new MelsecMcDataType((byte) 196, (byte) 1, nameof (CS), 10);
    /// <summary>计数器（线圈）</summary>
    public static readonly MelsecMcDataType Keyence_CC = new MelsecMcDataType((byte) 195, (byte) 1, nameof (CC), 10);
    /// <summary>输入继电器</summary>
    public static readonly MelsecMcDataType Panasonic_X = new MelsecMcDataType((byte) 156, (byte) 1, "X*", 10);
    /// <summary>输出继电器</summary>
    public static readonly MelsecMcDataType Panasonic_Y = new MelsecMcDataType((byte) 157, (byte) 1, "Y*", 10);
    /// <summary>链接继电器</summary>
    public static readonly MelsecMcDataType Panasonic_L = new MelsecMcDataType((byte) 160, (byte) 1, "L*", 10);
    /// <summary>内部继电器</summary>
    public static readonly MelsecMcDataType Panasonic_R = new MelsecMcDataType((byte) 144, (byte) 1, "R*", 10);
    /// <summary>数据存储器</summary>
    public static readonly MelsecMcDataType Panasonic_DT = new MelsecMcDataType((byte) 168, (byte) 0, "D*", 10);
    /// <summary>链接存储器</summary>
    public static readonly MelsecMcDataType Panasonic_LD = new MelsecMcDataType((byte) 180, (byte) 0, "W*", 10);
    /// <summary>计时器（当前值）</summary>
    public static readonly MelsecMcDataType Panasonic_TN = new MelsecMcDataType((byte) 194, (byte) 0, nameof (TN), 10);
    /// <summary>计时器（接点）</summary>
    public static readonly MelsecMcDataType Panasonic_TS = new MelsecMcDataType((byte) 193, (byte) 1, nameof (TS), 10);
    /// <summary>计数器（当前值）</summary>
    public static readonly MelsecMcDataType Panasonic_CN = new MelsecMcDataType((byte) 197, (byte) 0, nameof (CN), 10);
    /// <summary>计数器（接点）</summary>
    public static readonly MelsecMcDataType Panasonic_CS = new MelsecMcDataType((byte) 196, (byte) 1, nameof (CS), 10);
    /// <summary>特殊链接继电器</summary>
    public static readonly MelsecMcDataType Panasonic_SM = new MelsecMcDataType((byte) 145, (byte) 1, nameof (SM), 10);
    /// <summary>特殊链接存储器</summary>
    public static readonly MelsecMcDataType Panasonic_SD = new MelsecMcDataType((byte) 169, (byte) 0, nameof (SD), 10);

    /// <summary>
    /// 实例化一个三菱数据类型对象，如果您清楚类型代号，可以根据值进行扩展<br />
    /// Instantiate a Mitsubishi data type object, if you know the type code, you can expand according to the value
    /// </summary>
    /// <param name="code">数据类型的代号</param>
    /// <param name="type">0或1，默认为0</param>
    /// <param name="asciiCode">ASCII格式的类型信息</param>
    /// <param name="fromBase">指示地址的多少进制的，10或是16</param>
    public MelsecMcDataType(byte code, byte type, string asciiCode, int fromBase)
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
