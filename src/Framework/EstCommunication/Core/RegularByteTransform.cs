// Decompiled with JetBrains decompiler
// Type: EstCommunication.Core.RegularByteTransform
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

namespace EstCommunication.Core
{
  /// <summary>
  /// 常规的字节转换类<br />
  /// Regular byte conversion class
  /// </summary>
  public class RegularByteTransform : ByteTransformBase
  {
    /// <inheritdoc cref="M:EstCommunication.Core.ByteTransformBase.#ctor" />
    public RegularByteTransform()
    {
    }

    /// <inheritdoc cref="M:EstCommunication.Core.ByteTransformBase.#ctor(EstCommunication.Core.DataFormat)" />
    public RegularByteTransform(DataFormat dataFormat)
      : base(dataFormat)
    {
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.CreateByDateFormat(EstCommunication.Core.DataFormat)" />
    public override IByteTransform CreateByDateFormat(DataFormat dataFormat)
    {
      RegularByteTransform regularByteTransform = new RegularByteTransform(dataFormat);
      regularByteTransform.IsStringReverseByteWord = this.IsStringReverseByteWord;
      return (IByteTransform) regularByteTransform;
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("RegularByteTransform[{0}]", (object) this.DataFormat);
  }
}
