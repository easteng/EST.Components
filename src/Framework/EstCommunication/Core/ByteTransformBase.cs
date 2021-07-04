// Decompiled with JetBrains decompiler
// Type: EstCommunication.Core.ByteTransformBase
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using System;
using System.Text;

namespace EstCommunication.Core
{
  /// <summary>
  /// 数据转换类的基础，提供了一些基础的方法实现.<br />
  /// The basis of the data conversion class provides some basic method implementations.
  /// </summary>
  public class ByteTransformBase : IByteTransform
  {
    /// <summary>
    /// 实例化一个默认的对象<br />
    /// Instantiate a default object
    /// </summary>
    public ByteTransformBase() => this.DataFormat = DataFormat.DCBA;

    /// <summary>
    /// 使用指定的数据解析来实例化对象<br />
    /// Instantiate the object using the specified data parsing
    /// </summary>
    /// <param name="dataFormat">数据规则</param>
    public ByteTransformBase(DataFormat dataFormat) => this.DataFormat = dataFormat;

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransBool(System.Byte[],System.Int32)" />
    public virtual bool TransBool(byte[] buffer, int index) => ((int) buffer[index] & 1) == 1;

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransBool(System.Byte[],System.Int32,System.Int32)" />
    public bool[] TransBool(byte[] buffer, int index, int length)
    {
      byte[] InBytes = new byte[length];
      Array.Copy((Array) buffer, index, (Array) InBytes, 0, length);
      return SoftBasic.ByteToBoolArray(InBytes, length * 8);
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransByte(System.Byte[],System.Int32)" />
    public virtual byte TransByte(byte[] buffer, int index) => buffer[index];

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransByte(System.Byte[],System.Int32,System.Int32)" />
    public virtual byte[] TransByte(byte[] buffer, int index, int length)
    {
      byte[] numArray = new byte[length];
      Array.Copy((Array) buffer, index, (Array) numArray, 0, length);
      return numArray;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransInt16(System.Byte[],System.Int32)" />
    public virtual short TransInt16(byte[] buffer, int index) => BitConverter.ToInt16(buffer, index);

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransInt16(System.Byte[],System.Int32,System.Int32)" />
    public virtual short[] TransInt16(byte[] buffer, int index, int length)
    {
      short[] numArray = new short[length];
      for (int index1 = 0; index1 < length; ++index1)
        numArray[index1] = this.TransInt16(buffer, index + 2 * index1);
      return numArray;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransUInt16(System.Byte[],System.Int32)" />
    public virtual ushort TransUInt16(byte[] buffer, int index) => BitConverter.ToUInt16(buffer, index);

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransUInt16(System.Byte[],System.Int32,System.Int32)" />
    public virtual ushort[] TransUInt16(byte[] buffer, int index, int length)
    {
      ushort[] numArray = new ushort[length];
      for (int index1 = 0; index1 < length; ++index1)
        numArray[index1] = this.TransUInt16(buffer, index + 2 * index1);
      return numArray;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransInt32(System.Byte[],System.Int32)" />
    public virtual int TransInt32(byte[] buffer, int index) => BitConverter.ToInt32(this.ByteTransDataFormat4(buffer, index), 0);

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransInt32(System.Byte[],System.Int32,System.Int32)" />
    public virtual int[] TransInt32(byte[] buffer, int index, int length)
    {
      int[] numArray = new int[length];
      for (int index1 = 0; index1 < length; ++index1)
        numArray[index1] = this.TransInt32(buffer, index + 4 * index1);
      return numArray;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransUInt32(System.Byte[],System.Int32)" />
    public virtual uint TransUInt32(byte[] buffer, int index) => BitConverter.ToUInt32(this.ByteTransDataFormat4(buffer, index), 0);

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransUInt32(System.Byte[],System.Int32,System.Int32)" />
    public virtual uint[] TransUInt32(byte[] buffer, int index, int length)
    {
      uint[] numArray = new uint[length];
      for (int index1 = 0; index1 < length; ++index1)
        numArray[index1] = this.TransUInt32(buffer, index + 4 * index1);
      return numArray;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransInt64(System.Byte[],System.Int32)" />
    public virtual long TransInt64(byte[] buffer, int index) => BitConverter.ToInt64(this.ByteTransDataFormat8(buffer, index), 0);

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransInt64(System.Byte[],System.Int32,System.Int32)" />
    public virtual long[] TransInt64(byte[] buffer, int index, int length)
    {
      long[] numArray = new long[length];
      for (int index1 = 0; index1 < length; ++index1)
        numArray[index1] = this.TransInt64(buffer, index + 8 * index1);
      return numArray;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransUInt64(System.Byte[],System.Int32)" />
    public virtual ulong TransUInt64(byte[] buffer, int index) => BitConverter.ToUInt64(this.ByteTransDataFormat8(buffer, index), 0);

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransUInt64(System.Byte[],System.Int32,System.Int32)" />
    public virtual ulong[] TransUInt64(byte[] buffer, int index, int length)
    {
      ulong[] numArray = new ulong[length];
      for (int index1 = 0; index1 < length; ++index1)
        numArray[index1] = this.TransUInt64(buffer, index + 8 * index1);
      return numArray;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransSingle(System.Byte[],System.Int32)" />
    public virtual float TransSingle(byte[] buffer, int index) => BitConverter.ToSingle(this.ByteTransDataFormat4(buffer, index), 0);

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransSingle(System.Byte[],System.Int32,System.Int32)" />
    public virtual float[] TransSingle(byte[] buffer, int index, int length)
    {
      float[] numArray = new float[length];
      for (int index1 = 0; index1 < length; ++index1)
        numArray[index1] = this.TransSingle(buffer, index + 4 * index1);
      return numArray;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransDouble(System.Byte[],System.Int32)" />
    public virtual double TransDouble(byte[] buffer, int index) => BitConverter.ToDouble(this.ByteTransDataFormat8(buffer, index), 0);

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransDouble(System.Byte[],System.Int32,System.Int32)" />
    public virtual double[] TransDouble(byte[] buffer, int index, int length)
    {
      double[] numArray = new double[length];
      for (int index1 = 0; index1 < length; ++index1)
        numArray[index1] = this.TransDouble(buffer, index + 8 * index1);
      return numArray;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransString(System.Byte[],System.Int32,System.Int32,System.Text.Encoding)" />
    public virtual string TransString(byte[] buffer, int index, int length, Encoding encoding)
    {
      byte[] numArray = this.TransByte(buffer, index, length);
      return this.IsStringReverseByteWord ? encoding.GetString(SoftBasic.BytesReverseByWord(numArray)) : encoding.GetString(numArray);
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransString(System.Byte[],System.Text.Encoding)" />
    public virtual string TransString(byte[] buffer, Encoding encoding) => encoding.GetString(buffer);

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransByte(System.Boolean)" />
    public virtual byte[] TransByte(bool value) => this.TransByte(new bool[1]
    {
      value
    });

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransByte(System.Boolean[])" />
    public virtual byte[] TransByte(bool[] values) => values != null ? SoftBasic.BoolArrayToByte(values) : (byte[]) null;

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransByte(System.Byte)" />
    public virtual byte[] TransByte(byte value) => new byte[1]
    {
      value
    };

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransByte(System.Int16)" />
    public virtual byte[] TransByte(short value) => this.TransByte(new short[1]
    {
      value
    });

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransByte(System.Int16[])" />
    public virtual byte[] TransByte(short[] values)
    {
      if (values == null)
        return (byte[]) null;
      byte[] numArray = new byte[values.Length * 2];
      for (int index = 0; index < values.Length; ++index)
        BitConverter.GetBytes(values[index]).CopyTo((Array) numArray, 2 * index);
      return numArray;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransByte(System.UInt16)" />
    public virtual byte[] TransByte(ushort value) => this.TransByte(new ushort[1]
    {
      value
    });

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransByte(System.UInt16[])" />
    public virtual byte[] TransByte(ushort[] values)
    {
      if (values == null)
        return (byte[]) null;
      byte[] numArray = new byte[values.Length * 2];
      for (int index = 0; index < values.Length; ++index)
        BitConverter.GetBytes(values[index]).CopyTo((Array) numArray, 2 * index);
      return numArray;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransByte(System.Int32)" />
    public virtual byte[] TransByte(int value) => this.TransByte(new int[1]
    {
      value
    });

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransByte(System.Int32[])" />
    public virtual byte[] TransByte(int[] values)
    {
      if (values == null)
        return (byte[]) null;
      byte[] numArray = new byte[values.Length * 4];
      for (int index = 0; index < values.Length; ++index)
        this.ByteTransDataFormat4(BitConverter.GetBytes(values[index])).CopyTo((Array) numArray, 4 * index);
      return numArray;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransByte(System.UInt32)" />
    public virtual byte[] TransByte(uint value) => this.TransByte(new uint[1]
    {
      value
    });

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransByte(System.UInt32[])" />
    public virtual byte[] TransByte(uint[] values)
    {
      if (values == null)
        return (byte[]) null;
      byte[] numArray = new byte[values.Length * 4];
      for (int index = 0; index < values.Length; ++index)
        this.ByteTransDataFormat4(BitConverter.GetBytes(values[index])).CopyTo((Array) numArray, 4 * index);
      return numArray;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransByte(System.Int64)" />
    public virtual byte[] TransByte(long value) => this.TransByte(new long[1]
    {
      value
    });

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransByte(System.Int64[])" />
    public virtual byte[] TransByte(long[] values)
    {
      if (values == null)
        return (byte[]) null;
      byte[] numArray = new byte[values.Length * 8];
      for (int index = 0; index < values.Length; ++index)
        this.ByteTransDataFormat8(BitConverter.GetBytes(values[index])).CopyTo((Array) numArray, 8 * index);
      return numArray;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransByte(System.UInt64)" />
    public virtual byte[] TransByte(ulong value) => this.TransByte(new ulong[1]
    {
      value
    });

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransByte(System.UInt64[])" />
    public virtual byte[] TransByte(ulong[] values)
    {
      if (values == null)
        return (byte[]) null;
      byte[] numArray = new byte[values.Length * 8];
      for (int index = 0; index < values.Length; ++index)
        this.ByteTransDataFormat8(BitConverter.GetBytes(values[index])).CopyTo((Array) numArray, 8 * index);
      return numArray;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransByte(System.Single)" />
    public virtual byte[] TransByte(float value) => this.TransByte(new float[1]
    {
      value
    });

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransByte(System.Single[])" />
    public virtual byte[] TransByte(float[] values)
    {
      if (values == null)
        return (byte[]) null;
      byte[] numArray = new byte[values.Length * 4];
      for (int index = 0; index < values.Length; ++index)
        this.ByteTransDataFormat4(BitConverter.GetBytes(values[index])).CopyTo((Array) numArray, 4 * index);
      return numArray;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransByte(System.Double)" />
    public virtual byte[] TransByte(double value) => this.TransByte(new double[1]
    {
      value
    });

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransByte(System.Double[])" />
    public virtual byte[] TransByte(double[] values)
    {
      if (values == null)
        return (byte[]) null;
      byte[] numArray = new byte[values.Length * 8];
      for (int index = 0; index < values.Length; ++index)
        this.ByteTransDataFormat8(BitConverter.GetBytes(values[index])).CopyTo((Array) numArray, 8 * index);
      return numArray;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransByte(System.String,System.Text.Encoding)" />
    public virtual byte[] TransByte(string value, Encoding encoding)
    {
      if (value == null)
        return (byte[]) null;
      byte[] bytes = encoding.GetBytes(value);
      return this.IsStringReverseByteWord ? SoftBasic.BytesReverseByWord(bytes) : bytes;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.TransByte(System.String,System.Int32,System.Text.Encoding)" />
    public virtual byte[] TransByte(string value, int length, Encoding encoding)
    {
      if (value == null)
        return (byte[]) null;
      byte[] bytes = encoding.GetBytes(value);
      return this.IsStringReverseByteWord ? SoftBasic.ArrayExpandToLength<byte>(SoftBasic.BytesReverseByWord(bytes), length) : SoftBasic.ArrayExpandToLength<byte>(bytes, length);
    }

    /// <summary>反转多字节的数据信息</summary>
    /// <param name="value">数据字节</param>
    /// <param name="index">起始索引，默认值为0</param>
    /// <returns>实际字节信息</returns>
    protected byte[] ByteTransDataFormat4(byte[] value, int index = 0)
    {
      byte[] numArray = new byte[4];
      switch (this.DataFormat)
      {
        case DataFormat.ABCD:
          numArray[0] = value[index + 3];
          numArray[1] = value[index + 2];
          numArray[2] = value[index + 1];
          numArray[3] = value[index];
          break;
        case DataFormat.BADC:
          numArray[0] = value[index + 2];
          numArray[1] = value[index + 3];
          numArray[2] = value[index];
          numArray[3] = value[index + 1];
          break;
        case DataFormat.CDAB:
          numArray[0] = value[index + 1];
          numArray[1] = value[index];
          numArray[2] = value[index + 3];
          numArray[3] = value[index + 2];
          break;
        case DataFormat.DCBA:
          numArray[0] = value[index];
          numArray[1] = value[index + 1];
          numArray[2] = value[index + 2];
          numArray[3] = value[index + 3];
          break;
      }
      return numArray;
    }

    /// <summary>反转多字节的数据信息</summary>
    /// <param name="value">数据字节</param>
    /// <param name="index">起始索引，默认值为0</param>
    /// <returns>实际字节信息</returns>
    protected byte[] ByteTransDataFormat8(byte[] value, int index = 0)
    {
      byte[] numArray = new byte[8];
      switch (this.DataFormat)
      {
        case DataFormat.ABCD:
          numArray[0] = value[index + 7];
          numArray[1] = value[index + 6];
          numArray[2] = value[index + 5];
          numArray[3] = value[index + 4];
          numArray[4] = value[index + 3];
          numArray[5] = value[index + 2];
          numArray[6] = value[index + 1];
          numArray[7] = value[index];
          break;
        case DataFormat.BADC:
          numArray[0] = value[index + 6];
          numArray[1] = value[index + 7];
          numArray[2] = value[index + 4];
          numArray[3] = value[index + 5];
          numArray[4] = value[index + 2];
          numArray[5] = value[index + 3];
          numArray[6] = value[index];
          numArray[7] = value[index + 1];
          break;
        case DataFormat.CDAB:
          numArray[0] = value[index + 1];
          numArray[1] = value[index];
          numArray[2] = value[index + 3];
          numArray[3] = value[index + 2];
          numArray[4] = value[index + 5];
          numArray[5] = value[index + 4];
          numArray[6] = value[index + 7];
          numArray[7] = value[index + 6];
          break;
        case DataFormat.DCBA:
          numArray[0] = value[index];
          numArray[1] = value[index + 1];
          numArray[2] = value[index + 2];
          numArray[3] = value[index + 3];
          numArray[4] = value[index + 4];
          numArray[5] = value[index + 5];
          numArray[6] = value[index + 6];
          numArray[7] = value[index + 7];
          break;
      }
      return numArray;
    }

    /// <inheritdoc cref="P:EstCommunication.Core.IByteTransform.DataFormat" />
    public DataFormat DataFormat { get; set; }

    /// <inheritdoc cref="P:EstCommunication.Core.IByteTransform.IsStringReverseByteWord" />
    public bool IsStringReverseByteWord { get; set; }

    /// <inheritdoc cref="M:EstCommunication.Core.IByteTransform.CreateByDateFormat(EstCommunication.Core.DataFormat)" />
    public virtual IByteTransform CreateByDateFormat(DataFormat dataFormat) => (IByteTransform) this;

    /// <inheritdoc />
    public override string ToString() => string.Format("ByteTransformBase[{0}]", (object) this.DataFormat);
  }
}
