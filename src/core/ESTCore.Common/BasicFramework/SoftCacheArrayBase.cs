// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.BasicFramework.SoftCacheArrayBase
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Core;

using System;

namespace ESTCore.Common.BasicFramework
{
    /// <summary>内存队列的基类</summary>
    public abstract class SoftCacheArrayBase
    {
        /// <summary>字节数据流</summary>
        protected byte[] DataBytes = (byte[])null;
        /// <summary>数据数组变动时的数据锁</summary>
        protected SimpleHybirdLock HybirdLock = new SimpleHybirdLock();

        /// <summary>数据的长度</summary>
        public int ArrayLength { get; protected set; }

        /// <summary>用于从保存的数据对象初始化的</summary>
        /// <param name="dataSave"></param>
        /// <exception cref="T:System.NullReferenceException"></exception>
        public virtual void LoadFromBytes(byte[] dataSave)
        {
        }

        /// <summary>获取原本的数据字节</summary>
        /// <returns>字节数组</returns>
        public byte[] GetAllData()
        {
            byte[] numArray = new byte[this.DataBytes.Length];
            this.DataBytes.CopyTo((Array)numArray, 0);
            return numArray;
        }
    }
}
