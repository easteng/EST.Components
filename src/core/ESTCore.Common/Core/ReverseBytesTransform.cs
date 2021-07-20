// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Core.ReverseBytesTransform
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using System;

namespace ESTCore.Common.Core
{
    /// <summary>
    /// 字节倒序的转换类<br />
    /// Byte reverse order conversion class
    /// </summary>
    public class ReverseBytesTransform : ByteTransformBase
    {
        /// <inheritdoc cref="M:ESTCore.Common.Core.ByteTransformBase.#ctor" />
        public ReverseBytesTransform()
        {
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.ByteTransformBase.#ctor(ESTCore.Common.Core.DataFormat)" />
        public ReverseBytesTransform(DataFormat dataFormat)
          : base(dataFormat)
        {
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IByteTransform.TransInt16(System.Byte[],System.Int32)" />
        public override short TransInt16(byte[] buffer, int index) => BitConverter.ToInt16(new byte[2]
        {
      buffer[1 + index],
      buffer[index]
        }, 0);

        /// <inheritdoc cref="M:ESTCore.Common.Core.IByteTransform.TransUInt16(System.Byte[],System.Int32)" />
        public override ushort TransUInt16(byte[] buffer, int index) => BitConverter.ToUInt16(new byte[2]
        {
      buffer[1 + index],
      buffer[index]
        }, 0);

        /// <inheritdoc cref="M:ESTCore.Common.Core.IByteTransform.TransInt32(System.Byte[],System.Int32)" />
        public override int TransInt32(byte[] buffer, int index) => BitConverter.ToInt32(this.ByteTransDataFormat4(new byte[4]
        {
      buffer[3 + index],
      buffer[2 + index],
      buffer[1 + index],
      buffer[index]
        }), 0);

        /// <inheritdoc cref="M:ESTCore.Common.Core.IByteTransform.TransUInt32(System.Byte[],System.Int32)" />
        public override uint TransUInt32(byte[] buffer, int index) => BitConverter.ToUInt32(this.ByteTransDataFormat4(new byte[4]
        {
      buffer[3 + index],
      buffer[2 + index],
      buffer[1 + index],
      buffer[index]
        }), 0);

        /// <inheritdoc cref="M:ESTCore.Common.Core.IByteTransform.TransInt64(System.Byte[],System.Int32)" />
        public override long TransInt64(byte[] buffer, int index) => BitConverter.ToInt64(this.ByteTransDataFormat8(new byte[8]
        {
      buffer[7 + index],
      buffer[6 + index],
      buffer[5 + index],
      buffer[4 + index],
      buffer[3 + index],
      buffer[2 + index],
      buffer[1 + index],
      buffer[index]
        }), 0);

        /// <inheritdoc cref="M:ESTCore.Common.Core.IByteTransform.TransUInt64(System.Byte[],System.Int32)" />
        public override ulong TransUInt64(byte[] buffer, int index) => BitConverter.ToUInt64(this.ByteTransDataFormat8(new byte[8]
        {
      buffer[7 + index],
      buffer[6 + index],
      buffer[5 + index],
      buffer[4 + index],
      buffer[3 + index],
      buffer[2 + index],
      buffer[1 + index],
      buffer[index]
        }), 0);

        /// <inheritdoc cref="M:ESTCore.Common.Core.IByteTransform.TransSingle(System.Byte[],System.Int32)" />
        public override float TransSingle(byte[] buffer, int index) => BitConverter.ToSingle(this.ByteTransDataFormat4(new byte[4]
        {
      buffer[3 + index],
      buffer[2 + index],
      buffer[1 + index],
      buffer[index]
        }), 0);

        /// <inheritdoc cref="M:ESTCore.Common.Core.IByteTransform.TransDouble(System.Byte[],System.Int32)" />
        public override double TransDouble(byte[] buffer, int index) => BitConverter.ToDouble(this.ByteTransDataFormat8(new byte[8]
        {
      buffer[7 + index],
      buffer[6 + index],
      buffer[5 + index],
      buffer[4 + index],
      buffer[3 + index],
      buffer[2 + index],
      buffer[1 + index],
      buffer[index]
        }), 0);

        /// <inheritdoc cref="M:ESTCore.Common.Core.IByteTransform.TransByte(System.Int16[])" />
        public override byte[] TransByte(short[] values)
        {
            if (values == null)
                return (byte[])null;
            byte[] numArray = new byte[values.Length * 2];
            for (int index = 0; index < values.Length; ++index)
            {
                byte[] bytes = BitConverter.GetBytes(values[index]);
                Array.Reverse((Array)bytes);
                bytes.CopyTo((Array)numArray, 2 * index);
            }
            return numArray;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IByteTransform.TransByte(System.UInt16[])" />
        public override byte[] TransByte(ushort[] values)
        {
            if (values == null)
                return (byte[])null;
            byte[] numArray = new byte[values.Length * 2];
            for (int index = 0; index < values.Length; ++index)
            {
                byte[] bytes = BitConverter.GetBytes(values[index]);
                Array.Reverse((Array)bytes);
                bytes.CopyTo((Array)numArray, 2 * index);
            }
            return numArray;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IByteTransform.TransByte(System.Int32[])" />
        public override byte[] TransByte(int[] values)
        {
            if (values == null)
                return (byte[])null;
            byte[] numArray = new byte[values.Length * 4];
            for (int index = 0; index < values.Length; ++index)
            {
                byte[] bytes = BitConverter.GetBytes(values[index]);
                Array.Reverse((Array)bytes);
                this.ByteTransDataFormat4(bytes).CopyTo((Array)numArray, 4 * index);
            }
            return numArray;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IByteTransform.TransByte(System.UInt32[])" />
        public override byte[] TransByte(uint[] values)
        {
            if (values == null)
                return (byte[])null;
            byte[] numArray = new byte[values.Length * 4];
            for (int index = 0; index < values.Length; ++index)
            {
                byte[] bytes = BitConverter.GetBytes(values[index]);
                Array.Reverse((Array)bytes);
                this.ByteTransDataFormat4(bytes).CopyTo((Array)numArray, 4 * index);
            }
            return numArray;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IByteTransform.TransByte(System.Int64[])" />
        public override byte[] TransByte(long[] values)
        {
            if (values == null)
                return (byte[])null;
            byte[] numArray = new byte[values.Length * 8];
            for (int index = 0; index < values.Length; ++index)
            {
                byte[] bytes = BitConverter.GetBytes(values[index]);
                Array.Reverse((Array)bytes);
                this.ByteTransDataFormat8(bytes).CopyTo((Array)numArray, 8 * index);
            }
            return numArray;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IByteTransform.TransByte(System.UInt64[])" />
        public override byte[] TransByte(ulong[] values)
        {
            if (values == null)
                return (byte[])null;
            byte[] numArray = new byte[values.Length * 8];
            for (int index = 0; index < values.Length; ++index)
            {
                byte[] bytes = BitConverter.GetBytes(values[index]);
                Array.Reverse((Array)bytes);
                this.ByteTransDataFormat8(bytes).CopyTo((Array)numArray, 8 * index);
            }
            return numArray;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IByteTransform.TransByte(System.Single[])" />
        public override byte[] TransByte(float[] values)
        {
            if (values == null)
                return (byte[])null;
            byte[] numArray = new byte[values.Length * 4];
            for (int index = 0; index < values.Length; ++index)
            {
                byte[] bytes = BitConverter.GetBytes(values[index]);
                Array.Reverse((Array)bytes);
                this.ByteTransDataFormat4(bytes).CopyTo((Array)numArray, 4 * index);
            }
            return numArray;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IByteTransform.TransByte(System.Double[])" />
        /// <returns>buffer数据</returns>
        public override byte[] TransByte(double[] values)
        {
            if (values == null)
                return (byte[])null;
            byte[] numArray = new byte[values.Length * 8];
            for (int index = 0; index < values.Length; ++index)
            {
                byte[] bytes = BitConverter.GetBytes(values[index]);
                Array.Reverse((Array)bytes);
                this.ByteTransDataFormat8(bytes).CopyTo((Array)numArray, 8 * index);
            }
            return numArray;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IByteTransform.CreateByDateFormat(ESTCore.Common.Core.DataFormat)" />
        public override IByteTransform CreateByDateFormat(DataFormat dataFormat)
        {
            ReverseBytesTransform reverseBytesTransform = new ReverseBytesTransform(dataFormat);
            reverseBytesTransform.IsStringReverseByteWord = this.IsStringReverseByteWord;
            return (IByteTransform)reverseBytesTransform;
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("ReverseBytesTransform[{0}]", (object)this.DataFormat);
    }
}
