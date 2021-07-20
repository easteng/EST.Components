// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Core.ReverseWordTransform
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;

namespace ESTCore.Common.Core
{
    /// <summary>
    /// 按照字节错位的数据转换类<br />
    /// Data conversion class according to byte misalignment
    /// </summary>
    public class ReverseWordTransform : ByteTransformBase
    {
        /// <inheritdoc cref="M:ESTCore.Common.Core.ByteTransformBase.#ctor" />
        public ReverseWordTransform() => this.DataFormat = DataFormat.ABCD;

        /// <inheritdoc cref="M:ESTCore.Common.Core.ByteTransformBase.#ctor(ESTCore.Common.Core.DataFormat)" />
        public ReverseWordTransform(DataFormat dataFormat)
          : base(dataFormat)
        {
        }

        /// <summary>按照字节错位的方法</summary>
        /// <param name="buffer">实际的字节数据</param>
        /// <param name="index">起始字节位置</param>
        /// <param name="length">数据长度</param>
        /// <returns>处理过的数据信息</returns>
        private byte[] ReverseBytesByWord(byte[] buffer, int index, int length) => buffer == null ? (byte[])null : SoftBasic.BytesReverseByWord(buffer.SelectMiddle<byte>(index, length));

        /// <inheritdoc cref="M:ESTCore.Common.Core.IByteTransform.TransInt16(System.Byte[],System.Int32)" />
        public override short TransInt16(byte[] buffer, int index) => base.TransInt16(this.ReverseBytesByWord(buffer, index, 2), 0);

        /// <inheritdoc cref="M:ESTCore.Common.Core.IByteTransform.TransUInt16(System.Byte[],System.Int32)" />
        public override ushort TransUInt16(byte[] buffer, int index) => base.TransUInt16(this.ReverseBytesByWord(buffer, index, 2), 0);

        /// <inheritdoc cref="M:ESTCore.Common.Core.IByteTransform.TransByte(System.Int16[])" />
        public override byte[] TransByte(short[] values) => SoftBasic.BytesReverseByWord(base.TransByte(values));

        /// <inheritdoc cref="M:ESTCore.Common.Core.IByteTransform.TransByte(System.UInt16[])" />
        public override byte[] TransByte(ushort[] values) => SoftBasic.BytesReverseByWord(base.TransByte(values));

        /// <inheritdoc cref="M:ESTCore.Common.Core.IByteTransform.CreateByDateFormat(ESTCore.Common.Core.DataFormat)" />
        public override IByteTransform CreateByDateFormat(DataFormat dataFormat)
        {
            ReverseWordTransform reverseWordTransform = new ReverseWordTransform(dataFormat);
            reverseWordTransform.IsStringReverseByteWord = this.IsStringReverseByteWord;
            return (IByteTransform)reverseWordTransform;
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("ReverseWordTransform[{0}]", (object)this.DataFormat);
    }
}
