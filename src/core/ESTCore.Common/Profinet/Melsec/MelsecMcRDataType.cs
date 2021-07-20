// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Melsec.MelsecMcRDataType
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

namespace ESTCore.Common.Profinet.Melsec
{
    /// <summary>三菱R系列的PLC的数据类型</summary>
    public class MelsecMcRDataType
    {
        /// <summary>X输入继电器</summary>
        public static readonly MelsecMcRDataType X = new MelsecMcRDataType(new byte[2]
        {
      (byte) 156,
      (byte) 0
        }, (byte)1, "X***", 16);
        /// <summary>Y输入继电器</summary>
        public static readonly MelsecMcRDataType Y = new MelsecMcRDataType(new byte[2]
        {
      (byte) 157,
      (byte) 0
        }, (byte)1, "Y***", 16);
        /// <summary>M内部继电器</summary>
        public static readonly MelsecMcRDataType M = new MelsecMcRDataType(new byte[2]
        {
      (byte) 144,
      (byte) 0
        }, (byte)1, "M***", 10);
        /// <summary>特殊继电器</summary>
        public static readonly MelsecMcRDataType SM = new MelsecMcRDataType(new byte[2]
        {
      (byte) 145,
      (byte) 0
        }, (byte)1, "SM**", 10);
        /// <summary>锁存继电器</summary>
        public static readonly MelsecMcRDataType L = new MelsecMcRDataType(new byte[2]
        {
      (byte) 146,
      (byte) 0
        }, (byte)1, "L***", 10);
        /// <summary>报警器</summary>
        public static readonly MelsecMcRDataType F = new MelsecMcRDataType(new byte[2]
        {
      (byte) 147,
      (byte) 0
        }, (byte)1, "F***", 10);
        /// <summary>变址继电器</summary>
        public static readonly MelsecMcRDataType V = new MelsecMcRDataType(new byte[2]
        {
      (byte) 148,
      (byte) 0
        }, (byte)1, "V***", 10);
        /// <summary>S步进继电器</summary>
        public static readonly MelsecMcRDataType S = new MelsecMcRDataType(new byte[2]
        {
      (byte) 152,
      (byte) 0
        }, (byte)1, "S***", 10);
        /// <summary>链接继电器</summary>
        public static readonly MelsecMcRDataType B = new MelsecMcRDataType(new byte[2]
        {
      (byte) 160,
      (byte) 0
        }, (byte)1, "B***", 16);
        /// <summary>特殊链接继电器</summary>
        public static readonly MelsecMcRDataType SB = new MelsecMcRDataType(new byte[2]
        {
      (byte) 161,
      (byte) 0
        }, (byte)1, "SB**", 16);
        /// <summary>直接访问输入继电器</summary>
        public static readonly MelsecMcRDataType DX = new MelsecMcRDataType(new byte[2]
        {
      (byte) 162,
      (byte) 0
        }, (byte)1, "DX**", 16);
        /// <summary>直接访问输出继电器</summary>
        public static readonly MelsecMcRDataType DY = new MelsecMcRDataType(new byte[2]
        {
      (byte) 163,
      (byte) 0
        }, (byte)1, "DY**", 16);
        /// <summary>数据寄存器</summary>
        public static readonly MelsecMcRDataType D = new MelsecMcRDataType(new byte[2]
        {
      (byte) 168,
      (byte) 0
        }, (byte)0, "D***", 10);
        /// <summary>特殊数据寄存器</summary>
        public static readonly MelsecMcRDataType SD = new MelsecMcRDataType(new byte[2]
        {
      (byte) 169,
      (byte) 0
        }, (byte)0, "SD**", 10);
        /// <summary>链接寄存器</summary>
        public static readonly MelsecMcRDataType W = new MelsecMcRDataType(new byte[2]
        {
      (byte) 180,
      (byte) 0
        }, (byte)0, "W***", 16);
        /// <summary>特殊链接寄存器</summary>
        public static readonly MelsecMcRDataType SW = new MelsecMcRDataType(new byte[2]
        {
      (byte) 181,
      (byte) 0
        }, (byte)0, "SW**", 16);
        /// <summary>文件寄存器</summary>
        public static readonly MelsecMcRDataType R = new MelsecMcRDataType(new byte[2]
        {
      (byte) 175,
      (byte) 0
        }, (byte)0, "R***", 10);
        /// <summary>变址寄存器</summary>
        public static readonly MelsecMcRDataType Z = new MelsecMcRDataType(new byte[2]
        {
      (byte) 204,
      (byte) 0
        }, (byte)0, "Z***", 10);
        /// <summary>长累计定时器触点</summary>
        public static readonly MelsecMcRDataType LSTS = new MelsecMcRDataType(new byte[2]
        {
      (byte) 89,
      (byte) 0
        }, (byte)1, nameof(LSTS), 10);
        /// <summary>长累计定时器线圈</summary>
        public static readonly MelsecMcRDataType LSTC = new MelsecMcRDataType(new byte[2]
        {
      (byte) 88,
      (byte) 0
        }, (byte)1, nameof(LSTC), 10);
        /// <summary>长累计定时器当前值</summary>
        public static readonly MelsecMcRDataType LSTN = new MelsecMcRDataType(new byte[2]
        {
      (byte) 90,
      (byte) 0
        }, (byte)0, nameof(LSTN), 10);
        /// <summary>累计定时器触点</summary>
        public static readonly MelsecMcRDataType STS = new MelsecMcRDataType(new byte[2]
        {
      (byte) 199,
      (byte) 0
        }, (byte)1, "STS*", 10);
        /// <summary>累计定时器线圈</summary>
        public static readonly MelsecMcRDataType STC = new MelsecMcRDataType(new byte[2]
        {
      (byte) 198,
      (byte) 0
        }, (byte)1, "STC*", 10);
        /// <summary>累计定时器当前值</summary>
        public static readonly MelsecMcRDataType STN = new MelsecMcRDataType(new byte[2]
        {
      (byte) 200,
      (byte) 0
        }, (byte)0, "STN*", 10);
        /// <summary>长定时器触点</summary>
        public static readonly MelsecMcRDataType LTS = new MelsecMcRDataType(new byte[2]
        {
      (byte) 81,
      (byte) 0
        }, (byte)1, "LTS*", 10);
        /// <summary>长定时器线圈</summary>
        public static readonly MelsecMcRDataType LTC = new MelsecMcRDataType(new byte[2]
        {
      (byte) 80,
      (byte) 0
        }, (byte)1, "LTC*", 10);
        /// <summary>长定时器当前值</summary>
        public static readonly MelsecMcRDataType LTN = new MelsecMcRDataType(new byte[2]
        {
      (byte) 82,
      (byte) 0
        }, (byte)0, "LTN*", 10);
        /// <summary>定时器触点</summary>
        public static readonly MelsecMcRDataType TS = new MelsecMcRDataType(new byte[2]
        {
      (byte) 193,
      (byte) 0
        }, (byte)1, "TS**", 10);
        /// <summary>定时器线圈</summary>
        public static readonly MelsecMcRDataType TC = new MelsecMcRDataType(new byte[2]
        {
      (byte) 192,
      (byte) 0
        }, (byte)1, "TC**", 10);
        /// <summary>定时器当前值</summary>
        public static readonly MelsecMcRDataType TN = new MelsecMcRDataType(new byte[2]
        {
      (byte) 194,
      (byte) 0
        }, (byte)0, "TN**", 10);
        /// <summary>长计数器触点</summary>
        public static readonly MelsecMcRDataType LCS = new MelsecMcRDataType(new byte[2]
        {
      (byte) 85,
      (byte) 0
        }, (byte)1, "LCS*", 10);
        /// <summary>长计数器线圈</summary>
        public static readonly MelsecMcRDataType LCC = new MelsecMcRDataType(new byte[2]
        {
      (byte) 84,
      (byte) 0
        }, (byte)1, "LCC*", 10);
        /// <summary>长计数器当前值</summary>
        public static readonly MelsecMcRDataType LCN = new MelsecMcRDataType(new byte[2]
        {
      (byte) 86,
      (byte) 0
        }, (byte)0, "LCN*", 10);
        /// <summary>计数器触点</summary>
        public static readonly MelsecMcRDataType CS = new MelsecMcRDataType(new byte[2]
        {
      (byte) 196,
      (byte) 0
        }, (byte)1, "CS**", 10);
        /// <summary>计数器线圈</summary>
        public static readonly MelsecMcRDataType CC = new MelsecMcRDataType(new byte[2]
        {
      (byte) 195,
      (byte) 0
        }, (byte)1, "CC**", 10);
        /// <summary>计数器当前值</summary>
        public static readonly MelsecMcRDataType CN = new MelsecMcRDataType(new byte[2]
        {
      (byte) 197,
      (byte) 0
        }, (byte)0, "CN**", 10);

        /// <summary>如果您清楚类型代号，可以根据值进行扩展</summary>
        /// <param name="code">数据类型的代号</param>
        /// <param name="type">0或1，默认为0</param>
        /// <param name="asciiCode">ASCII格式的类型信息</param>
        /// <param name="fromBase">指示地址的多少进制的，10或是16</param>
        public MelsecMcRDataType(byte[] code, byte type, string asciiCode, int fromBase)
        {
            this.DataCode = code;
            this.AsciiCode = asciiCode;
            this.FromBase = fromBase;
            if (type >= (byte)2)
                return;
            this.DataType = type;
        }

        /// <summary>类型的代号值</summary>
        public byte[] DataCode { get; private set; } = new byte[2];

        /// <summary>数据的类型，0代表按字，1代表按位</summary>
        public byte DataType { get; private set; } = 0;

        /// <summary>当以ASCII格式通讯时的类型描述</summary>
        public string AsciiCode { get; private set; }

        /// <summary>指示地址是10进制，还是16进制的</summary>
        public int FromBase { get; private set; }
    }
}
