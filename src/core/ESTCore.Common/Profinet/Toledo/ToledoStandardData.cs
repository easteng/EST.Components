// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Toledo.ToledoStandardData
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;

using Newtonsoft.Json;

using System.Text;

namespace ESTCore.Common.Profinet.Toledo
{
    /// <summary>托利多标准格式的数据类对象</summary>
    public class ToledoStandardData
    {
        /// <summary>实例化一个默认的对象</summary>
        public ToledoStandardData()
        {
        }

        /// <summary>从缓存里加载一个标准格式的对象</summary>
        /// <param name="buffer">缓存</param>
        public ToledoStandardData(byte[] buffer)
        {
            this.Weight = float.Parse(Encoding.ASCII.GetString(buffer, 4, 6));
            this.Tare = float.Parse(Encoding.ASCII.GetString(buffer, 10, 6));
            switch ((int)buffer[1] & 7)
            {
                case 0:
                    this.Weight *= 100f;
                    this.Tare *= 100f;
                    break;
                case 1:
                    this.Weight *= 10f;
                    this.Tare *= 10f;
                    break;
                case 3:
                    this.Weight /= 10f;
                    this.Tare /= 10f;
                    break;
                case 4:
                    this.Weight /= 100f;
                    this.Tare /= 100f;
                    break;
                case 5:
                    this.Weight /= 1000f;
                    this.Tare /= 1000f;
                    break;
                case 6:
                    this.Weight /= 10000f;
                    this.Tare /= 10000f;
                    break;
                case 7:
                    this.Weight /= 100000f;
                    this.Tare /= 100000f;
                    break;
            }
            this.Suttle = SoftBasic.BoolOnByteIndex(buffer[2], 0);
            this.Symbol = SoftBasic.BoolOnByteIndex(buffer[2], 1);
            this.BeyondScope = SoftBasic.BoolOnByteIndex(buffer[2], 2);
            this.DynamicState = SoftBasic.BoolOnByteIndex(buffer[2], 3);
            switch ((int)buffer[3] & 7)
            {
                case 0:
                    this.Unit = SoftBasic.BoolOnByteIndex(buffer[2], 4) ? "kg" : "lb";
                    break;
                case 1:
                    this.Unit = "g";
                    break;
                case 2:
                    this.Unit = "t";
                    break;
                case 3:
                    this.Unit = "oz";
                    break;
                case 4:
                    this.Unit = "ozt";
                    break;
                case 5:
                    this.Unit = "dwt";
                    break;
                case 6:
                    this.Unit = "ton";
                    break;
                case 7:
                    this.Unit = "newton";
                    break;
            }
            this.IsPrint = SoftBasic.BoolOnByteIndex(buffer[3], 3);
            this.IsTenExtend = SoftBasic.BoolOnByteIndex(buffer[3], 4);
            this.SourceData = buffer;
        }

        /// <summary>为 True 则是净重，为 False 则为毛重</summary>
        public bool Suttle { get; set; }

        /// <summary>为 True 则是正，为 False 则为负</summary>
        public bool Symbol { get; set; }

        /// <summary>是否在范围之外</summary>
        public bool BeyondScope { get; set; }

        /// <summary>是否为动态，为 True 则是动态，为 False 则为稳态</summary>
        public bool DynamicState { get; set; }

        /// <summary>单位</summary>
        public string Unit { get; set; }

        /// <summary>是否打印</summary>
        public bool IsPrint { get; set; }

        /// <summary>是否10被扩展</summary>
        public bool IsTenExtend { get; set; }

        /// <summary>重量</summary>
        public float Weight { get; set; }

        /// <summary>皮重</summary>
        public float Tare { get; set; }

        /// <summary>解析数据的原始字节</summary>
        [JsonIgnore]
        public byte[] SourceData { get; set; }

        /// <inheritdoc />
        public override string ToString() => string.Format("ToledoStandardData[{0}]", (object)this.Weight);
    }
}
