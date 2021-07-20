// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Omron.OmronFinsDataType
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

namespace ESTCore.Common.Profinet.Omron
{
    /// <summary>欧姆龙的Fins协议的数据类型</summary>
    public class OmronFinsDataType
    {
        /// <summary>DM Area</summary>
        public static readonly OmronFinsDataType DM = new OmronFinsDataType((byte)2, (byte)130);
        /// <summary>CIO Area</summary>
        public static readonly OmronFinsDataType CIO = new OmronFinsDataType((byte)48, (byte)176);
        /// <summary>Work Area</summary>
        public static readonly OmronFinsDataType WR = new OmronFinsDataType((byte)49, (byte)177);
        /// <summary>Holding Bit Area</summary>
        public static readonly OmronFinsDataType HR = new OmronFinsDataType((byte)50, (byte)178);
        /// <summary>Auxiliary Bit Area</summary>
        public static readonly OmronFinsDataType AR = new OmronFinsDataType((byte)51, (byte)179);
        /// <summary>TIM Area</summary>
        public static readonly OmronFinsDataType TIM = new OmronFinsDataType((byte)9, (byte)137);

        /// <summary>实例化一个Fins的数据类型</summary>
        /// <param name="bitCode">进行位操作的指令</param>
        /// <param name="wordCode">进行字操作的指令</param>
        public OmronFinsDataType(byte bitCode, byte wordCode)
        {
            this.BitCode = bitCode;
            this.WordCode = wordCode;
        }

        /// <summary>进行位操作的指令</summary>
        public byte BitCode { get; private set; }

        /// <summary>进行字操作的指令</summary>
        public byte WordCode { get; private set; }
    }
}
