// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.BasicFramework.SoftCacheArrayLong
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using System;

namespace ESTCore.Common.BasicFramework
{
    /// <summary>一个内存队列缓存的类，数据类型为Int64</summary>
    public sealed class SoftCacheArrayLong : SoftCacheArrayBase
    {
        /// <summary>数据的本身面貌</summary>
        private long[] DataArray = (long[])null;

        /// <summary>实例化一个数据对象</summary>
        /// <param name="capacity"></param>
        /// <param name="defaultValue"></param>
        public SoftCacheArrayLong(int capacity, int defaultValue)
        {
            if (capacity < 10)
                capacity = 10;
            this.ArrayLength = capacity;
            this.DataArray = new long[capacity];
            this.DataBytes = new byte[capacity * 8];
            if ((uint)defaultValue <= 0U)
                return;
            for (int index = 0; index < capacity; ++index)
                this.DataArray[index] = (long)defaultValue;
        }

        /// <summary>用于从保存的数据对象初始化的</summary>
        /// <param name="dataSave"></param>
        /// <exception cref="T:System.NullReferenceException"></exception>
        public override void LoadFromBytes(byte[] dataSave)
        {
            int length = dataSave.Length / 8;
            this.ArrayLength = length;
            this.DataArray = new long[length];
            this.DataBytes = new byte[length * 8];
            for (int index = 0; index < length; ++index)
                this.DataArray[index] = BitConverter.ToInt64(dataSave, index * 8);
        }

        /// <summary>线程安全的添加数据</summary>
        /// <param name="value">值</param>
        public void AddValue(long value)
        {
            this.HybirdLock.Enter();
            for (int index = 0; index < this.ArrayLength - 1; ++index)
                this.DataArray[index] = this.DataArray[index + 1];
            this.DataArray[this.ArrayLength - 1] = value;
            for (int index = 0; index < this.ArrayLength; ++index)
                BitConverter.GetBytes(this.DataArray[index]).CopyTo((Array)this.DataBytes, 8 * index);
            this.HybirdLock.Leave();
        }
    }
}
