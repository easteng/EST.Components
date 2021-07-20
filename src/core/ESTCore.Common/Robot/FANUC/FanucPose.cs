// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Robot.FANUC.FanucPose
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;
using ESTCore.Common.Core;

using System;
using System.Text;

namespace ESTCore.Common.Robot.FANUC
{
    /// <summary>机器人的姿态数据</summary>
    public class FanucPose
    {
        /// <summary>Xyzwpr</summary>
        public float[] Xyzwpr { get; set; }

        /// <summary>Config</summary>
        public string[] Config { get; set; }

        /// <summary>Joint</summary>
        public float[] Joint { get; set; }

        /// <summary>UF</summary>
        public short UF { get; set; }

        /// <summary>UT</summary>
        public short UT { get; set; }

        /// <summary>ValidC</summary>
        public short ValidC { get; set; }

        /// <summary>ValidJ</summary>
        public short ValidJ { get; set; }

        /// <summary>从原始数据解析出当前的姿态数据</summary>
        /// <param name="byteTransform">字节变化内容</param>
        /// <param name="content">原始的内容</param>
        /// <param name="index">索引位置</param>
        public void LoadByContent(IByteTransform byteTransform, byte[] content, int index)
        {
            this.Xyzwpr = new float[9];
            for (int index1 = 0; index1 < this.Xyzwpr.Length; ++index1)
                this.Xyzwpr[index1] = BitConverter.ToSingle(content, index + 4 * index1);
            this.Config = FanucPose.TransConfigStringArray(byteTransform.TransInt16(content, index + 36, 7));
            this.Joint = new float[9];
            for (int index1 = 0; index1 < this.Joint.Length; ++index1)
                this.Joint[index1] = BitConverter.ToSingle(content, index + 52 + 4 * index1);
            this.ValidC = BitConverter.ToInt16(content, index + 50);
            this.ValidJ = BitConverter.ToInt16(content, index + 88);
            this.UF = BitConverter.ToInt16(content, index + 90);
            this.UT = BitConverter.ToInt16(content, index + 92);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder(string.Format("FanucPose UF={0} UT={1}", (object)this.UF, (object)this.UT));
            if ((uint)this.ValidC > 0U)
                stringBuilder.Append("\r\nXyzwpr=" + SoftBasic.ArrayFormat<float>(this.Xyzwpr) + "\r\nConfig=" + SoftBasic.ArrayFormat<string>(this.Config));
            if ((uint)this.ValidJ > 0U)
                stringBuilder.Append("\r\nJOINT=" + SoftBasic.ArrayFormat<float>(this.Joint));
            return stringBuilder.ToString();
        }

        /// <summary>从原始的字节数据创建一个新的姿态数据</summary>
        /// <param name="byteTransform"></param>
        /// <param name="content">原始的内容</param>
        /// <param name="index">索引位置</param>
        /// <returns>姿态数据</returns>
        public static FanucPose PraseFrom(
          IByteTransform byteTransform,
          byte[] content,
          int index)
        {
            FanucPose fanucPose = new FanucPose();
            fanucPose.LoadByContent(byteTransform, content, index);
            return fanucPose;
        }

        /// <summary>将short类型的config数组转换成string数组类型的config</summary>
        /// <param name="value">short数组的值</param>
        /// <returns>string数组的值</returns>
        public static string[] TransConfigStringArray(short[] value) => new string[7]
        {
      value[0] != (short) 0 ? "F" : "N",
      value[1] != (short) 0 ? "L" : "R",
      value[2] != (short) 0 ? "U" : "D",
      value[3] != (short) 0 ? "T" : "B",
      value[4].ToString(),
      value[5].ToString(),
      value[6].ToString()
        };
    }
}
