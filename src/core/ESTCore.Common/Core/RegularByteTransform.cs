// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Core.RegularByteTransform
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

namespace ESTCore.Common.Core
{
    /// <summary>
    /// 常规的字节转换类<br />
    /// Regular byte conversion class
    /// </summary>
    public class RegularByteTransform : ByteTransformBase
    {
        /// <inheritdoc cref="M:ESTCore.Common.Core.ByteTransformBase.#ctor" />
        public RegularByteTransform()
        {
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.ByteTransformBase.#ctor(ESTCore.Common.Core.DataFormat)" />
        public RegularByteTransform(DataFormat dataFormat)
          : base(dataFormat)
        {
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IByteTransform.CreateByDateFormat(ESTCore.Common.Core.DataFormat)" />
        public override IByteTransform CreateByDateFormat(DataFormat dataFormat)
        {
            RegularByteTransform regularByteTransform = new RegularByteTransform(dataFormat);
            regularByteTransform.IsStringReverseByteWord = this.IsStringReverseByteWord;
            return (IByteTransform)regularByteTransform;
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("RegularByteTransform[{0}]", (object)this.DataFormat);
    }
}
