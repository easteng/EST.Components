// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Melsec.MelsecA1EDataType
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

namespace ESTCore.Common.Profinet.Melsec
{
    /// <summary>三菱PLC的数据类型，此处包含了几个常用的类型</summary>
    public class MelsecA1EDataType
    {
        /// <summary>X输入寄存器</summary>
        public static readonly MelsecA1EDataType X = new MelsecA1EDataType(new byte[2]
        {
      (byte) 88,
      (byte) 32
        }, (byte)1, "X*", 16);
        /// <summary>Y输出寄存器</summary>
        public static readonly MelsecA1EDataType Y = new MelsecA1EDataType(new byte[2]
        {
      (byte) 89,
      (byte) 32
        }, (byte)1, "Y*", 16);
        /// <summary>M中间寄存器</summary>
        public static readonly MelsecA1EDataType M = new MelsecA1EDataType(new byte[2]
        {
      (byte) 77,
      (byte) 32
        }, (byte)1, "M*", 10);
        /// <summary>S状态寄存器</summary>
        public static readonly MelsecA1EDataType S = new MelsecA1EDataType(new byte[2]
        {
      (byte) 83,
      (byte) 32
        }, (byte)1, "S*", 10);
        /// <summary>F报警器</summary>
        public static readonly MelsecA1EDataType F = new MelsecA1EDataType(new byte[2]
        {
      (byte) 70,
      (byte) 32
        }, (byte)1, "F*", 10);
        /// <summary>B连接继电器</summary>
        public static readonly MelsecA1EDataType B = new MelsecA1EDataType(new byte[2]
        {
      (byte) 66,
      (byte) 32
        }, (byte)1, "B*", 16);
        /// <summary>TS定时器触点</summary>
        public static readonly MelsecA1EDataType TS = new MelsecA1EDataType(new byte[2]
        {
      (byte) 84,
      (byte) 83
        }, (byte)1, nameof(TS), 10);
        /// <summary>TC定时器线圈</summary>
        public static readonly MelsecA1EDataType TC = new MelsecA1EDataType(new byte[2]
        {
      (byte) 84,
      (byte) 67
        }, (byte)1, nameof(TC), 10);
        /// <summary>TN定时器当前值</summary>
        public static readonly MelsecA1EDataType TN = new MelsecA1EDataType(new byte[2]
        {
      (byte) 84,
      (byte) 78
        }, (byte)0, nameof(TN), 10);
        /// <summary>CS计数器触点</summary>
        public static readonly MelsecA1EDataType CS = new MelsecA1EDataType(new byte[2]
        {
      (byte) 67,
      (byte) 83
        }, (byte)1, nameof(CS), 10);
        /// <summary>CC计数器线圈</summary>
        public static readonly MelsecA1EDataType CC = new MelsecA1EDataType(new byte[2]
        {
      (byte) 67,
      (byte) 67
        }, (byte)1, nameof(CC), 10);
        /// <summary>CN计数器当前值</summary>
        public static readonly MelsecA1EDataType CN = new MelsecA1EDataType(new byte[2]
        {
      (byte) 67,
      (byte) 78
        }, (byte)0, nameof(CN), 10);
        /// <summary>D数据寄存器</summary>
        public static readonly MelsecA1EDataType D = new MelsecA1EDataType(new byte[2]
        {
      (byte) 68,
      (byte) 32
        }, (byte)0, "D*", 10);
        /// <summary>W链接寄存器</summary>
        public static readonly MelsecA1EDataType W = new MelsecA1EDataType(new byte[2]
        {
      (byte) 87,
      (byte) 32
        }, (byte)0, "W*", 16);
        /// <summary>R文件寄存器</summary>
        public static readonly MelsecA1EDataType R = new MelsecA1EDataType(new byte[2]
        {
      (byte) 82,
      (byte) 32
        }, (byte)0, "R*", 10);

        /// <summary>如果您清楚类型代号，可以根据值进行扩展</summary>
        /// <param name="code">数据类型的代号</param>
        /// <param name="type">0或1，默认为0</param>
        /// <param name="asciiCode">ASCII格式的类型信息</param>
        /// <param name="fromBase">指示地址的多少进制的，10或是16</param>
        public MelsecA1EDataType(byte[] code, byte type, string asciiCode, int fromBase)
        {
            this.DataCode = code;
            this.AsciiCode = asciiCode;
            this.FromBase = fromBase;
            if (type >= (byte)2)
                return;
            this.DataType = type;
        }

        /// <summary>类型的代号值（软元件代码，用于区分软元件类型，如：D，R）</summary>
        public byte[] DataCode { get; private set; } = new byte[2];

        /// <summary>数据的类型，0代表按字，1代表按位</summary>
        public byte DataType { get; private set; } = 0;

        /// <summary>当以ASCII格式通讯时的类型描述</summary>
        public string AsciiCode { get; private set; }

        /// <summary>指示地址是10进制，还是16进制的</summary>
        public int FromBase { get; private set; }
    }
}
