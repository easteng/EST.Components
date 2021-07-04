// Decompiled with JetBrains decompiler
// Type: EstCommunication.BasicFramework.SoftBuffer
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.Core;
using System;

namespace EstCommunication.BasicFramework
{
  /// <summary>
  /// 一个线程安全的缓存数据块，支持批量动态修改，添加，并获取快照<br />
  /// A thread-safe cache data block that supports batch dynamic modification, addition, and snapshot acquisition
  /// </summary>
  /// <remarks>
  /// 这个类可以实现什么功能呢，就是你有一个大的数组，作为你的应用程序的中间数据池，允许你往byte[]数组里存放指定长度的子byte[]数组，也允许从里面拿数据，
  /// 这些操作都是线程安全的，当然，本类扩展了一些额外的方法支持，也可以直接赋值或获取基本的数据类型对象。
  /// </remarks>
  /// <example>
  /// 此处举例一些数据的读写说明，可以此处的数据示例。
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\BasicFramework\SoftBufferExample.cs" region="SoftBufferExample1" title="SoftBuffer示例" />
  /// </example>
  public class SoftBuffer : IDisposable
  {
    private int capacity = 10;
    private byte[] buffer;
    private SimpleHybirdLock hybirdLock;
    private IByteTransform byteTransform;
    private bool isBoolReverseByWord = false;
    private bool disposedValue = false;

    /// <summary>
    /// 使用默认的大小初始化缓存空间<br />
    /// Initialize cache space with default size
    /// </summary>
    public SoftBuffer()
    {
      this.buffer = new byte[this.capacity];
      this.hybirdLock = new SimpleHybirdLock();
      this.byteTransform = (IByteTransform) new RegularByteTransform();
    }

    /// <summary>
    /// 使用指定的容量初始化缓存数据块<br />
    /// Initialize the cache data block with the specified capacity
    /// </summary>
    /// <param name="capacity">初始化的容量</param>
    public SoftBuffer(int capacity)
    {
      this.buffer = new byte[capacity];
      this.capacity = capacity;
      this.hybirdLock = new SimpleHybirdLock();
      this.byteTransform = (IByteTransform) new RegularByteTransform();
    }

    /// <summary>
    /// 设置指定的位置bool值，如果超出，则丢弃数据，该位置是指按照位为单位排序的<br />
    /// Set the bool value at the specified position, if it is exceeded,
    /// the data is discarded, the position refers to sorting in units of bits
    /// </summary>
    /// <param name="value">bool值</param>
    /// <param name="destIndex">目标存储的索引</param>
    /// <exception cref="T:System.IndexOutOfRangeException"></exception>
    public void SetBool(bool value, int destIndex) => this.SetBool(new bool[1]
    {
      value
    }, destIndex);

    /// <summary>
    /// 设置指定的位置的bool数组，如果超出，则丢弃数据，该位置是指按照位为单位排序的<br />
    /// Set the bool array at the specified position, if it is exceeded,
    /// the data is discarded, the position refers to sorting in units of bits
    /// </summary>
    /// <param name="value">bool数组值</param>
    /// <param name="destIndex">目标存储的索引</param>
    /// <exception cref="T:System.IndexOutOfRangeException"></exception>
    public void SetBool(bool[] value, int destIndex)
    {
      if (value == null)
        return;
      try
      {
        this.hybirdLock.Enter();
        for (int index1 = 0; index1 < value.Length; ++index1)
        {
          int index2 = (destIndex + index1) / 8;
          int offset = (destIndex + index1) % 8;
          if (this.isBoolReverseByWord)
          {
            if (index2 % 2 == 0)
              ++index2;
            else
              --index2;
          }
          this.buffer[index2] = !value[index1] ? (byte) ((uint) this.buffer[index2] & (uint) this.getAndByte(offset)) : (byte) ((uint) this.buffer[index2] | (uint) this.getOrByte(offset));
        }
        this.hybirdLock.Leave();
      }
      catch
      {
        this.hybirdLock.Leave();
        throw;
      }
    }

    /// <summary>
    /// 获取指定的位置的bool值，如果超出，则引发异常<br />
    /// Get the bool value at the specified position, if it exceeds, an exception is thrown
    /// </summary>
    /// <param name="destIndex">目标存储的索引</param>
    /// <returns>获取索引位置的bool数据值</returns>
    /// <exception cref="T:System.IndexOutOfRangeException"></exception>
    public bool GetBool(int destIndex) => this.GetBool(destIndex, 1)[0];

    /// <summary>
    /// 获取指定位置的bool数组值，如果超过，则引发异常<br />
    /// Get the bool array value at the specified position, if it exceeds, an exception is thrown
    /// </summary>
    /// <param name="destIndex">目标存储的索引</param>
    /// <param name="length">读取的数组长度</param>
    /// <exception cref="T:System.IndexOutOfRangeException"></exception>
    /// <returns>bool数组值</returns>
    public bool[] GetBool(int destIndex, int length)
    {
      bool[] flagArray = new bool[length];
      try
      {
        this.hybirdLock.Enter();
        for (int index1 = 0; index1 < length; ++index1)
        {
          int index2 = (destIndex + index1) / 8;
          int offset = (destIndex + index1) % 8;
          if (this.isBoolReverseByWord)
          {
            if (index2 % 2 == 0)
              ++index2;
            else
              --index2;
          }
          flagArray[index1] = ((int) this.buffer[index2] & (int) this.getOrByte(offset)) == (int) this.getOrByte(offset);
        }
        this.hybirdLock.Leave();
      }
      catch
      {
        this.hybirdLock.Leave();
        throw;
      }
      return flagArray;
    }

    private byte getAndByte(int offset)
    {
      switch (offset)
      {
        case 0:
          return 254;
        case 1:
          return 253;
        case 2:
          return 251;
        case 3:
          return 247;
        case 4:
          return 239;
        case 5:
          return 223;
        case 6:
          return 191;
        case 7:
          return 127;
        default:
          return byte.MaxValue;
      }
    }

    private byte getOrByte(int offset)
    {
      switch (offset)
      {
        case 0:
          return 1;
        case 1:
          return 2;
        case 2:
          return 4;
        case 3:
          return 8;
        case 4:
          return 16;
        case 5:
          return 32;
        case 6:
          return 64;
        case 7:
          return 128;
        default:
          return 0;
      }
    }

    /// <summary>
    /// 设置指定的位置的数据块，如果超出，则丢弃数据<br />
    /// Set the data block at the specified position, if it is exceeded, the data is discarded
    /// </summary>
    /// <param name="data">数据块信息</param>
    /// <param name="destIndex">目标存储的索引</param>
    public void SetBytes(byte[] data, int destIndex)
    {
      if (destIndex >= this.capacity || destIndex < 0 || data == null)
        return;
      this.hybirdLock.Enter();
      if (data.Length + destIndex > this.buffer.Length)
        Array.Copy((Array) data, 0, (Array) this.buffer, destIndex, this.buffer.Length - destIndex);
      else
        data.CopyTo((Array) this.buffer, destIndex);
      this.hybirdLock.Leave();
    }

    /// <summary>
    /// 设置指定的位置的数据块，如果超出，则丢弃数据
    /// Set the data block at the specified position, if it is exceeded, the data is discarded
    /// </summary>
    /// <param name="data">数据块信息</param>
    /// <param name="destIndex">目标存储的索引</param>
    /// <param name="length">准备拷贝的数据长度</param>
    public void SetBytes(byte[] data, int destIndex, int length)
    {
      if (destIndex >= this.capacity || destIndex < 0 || data == null)
        return;
      if (length > data.Length)
        length = data.Length;
      this.hybirdLock.Enter();
      if (length + destIndex > this.buffer.Length)
        Array.Copy((Array) data, 0, (Array) this.buffer, destIndex, this.buffer.Length - destIndex);
      else
        Array.Copy((Array) data, 0, (Array) this.buffer, destIndex, length);
      this.hybirdLock.Leave();
    }

    /// <summary>
    /// 设置指定的位置的数据块，如果超出，则丢弃数据<br />
    /// Set the data block at the specified position, if it is exceeded, the data is discarded
    /// </summary>
    /// <param name="data">数据块信息</param>
    /// <param name="sourceIndex">Data中的起始位置</param>
    /// <param name="destIndex">目标存储的索引</param>
    /// <param name="length">准备拷贝的数据长度</param>
    /// <exception cref="T:System.IndexOutOfRangeException"></exception>
    public void SetBytes(byte[] data, int sourceIndex, int destIndex, int length)
    {
      if (destIndex >= this.capacity || destIndex < 0 || data == null)
        return;
      if (length > data.Length)
        length = data.Length;
      this.hybirdLock.Enter();
      Array.Copy((Array) data, sourceIndex, (Array) this.buffer, destIndex, length);
      this.hybirdLock.Leave();
    }

    /// <summary>
    /// 获取内存指定长度的数据信息<br />
    /// Get data information of specified length in memory
    /// </summary>
    /// <param name="index">起始位置</param>
    /// <param name="length">数组长度</param>
    /// <returns>返回实际的数据信息</returns>
    public byte[] GetBytes(int index, int length)
    {
      byte[] numArray = new byte[length];
      if (length > 0)
      {
        this.hybirdLock.Enter();
        if (index >= 0 && index + length <= this.buffer.Length)
          Array.Copy((Array) this.buffer, index, (Array) numArray, 0, length);
        this.hybirdLock.Leave();
      }
      return numArray;
    }

    /// <summary>
    /// 获取内存所有的数据信息<br />
    /// Get all data information in memory
    /// </summary>
    /// <returns>实际的数据信息</returns>
    public byte[] GetBytes() => this.GetBytes(0, this.capacity);

    /// <summary>
    /// 设置byte类型的数据到缓存区<br />
    /// Set byte type data to the cache area
    /// </summary>
    /// <param name="value">byte数值</param>
    /// <param name="index">索引位置</param>
    public void SetValue(byte value, int index) => this.SetBytes(new byte[1]
    {
      value
    }, index);

    /// <summary>
    /// 设置short数组的数据到缓存区<br />
    /// Set short array data to the cache area
    /// </summary>
    /// <param name="values">short数组</param>
    /// <param name="index">索引位置</param>
    public void SetValue(short[] values, int index) => this.SetBytes(this.byteTransform.TransByte(values), index);

    /// <summary>
    /// 设置short类型的数据到缓存区<br />
    /// Set short type data to the cache area
    /// </summary>
    /// <param name="value">short数值</param>
    /// <param name="index">索引位置</param>
    public void SetValue(short value, int index) => this.SetValue(new short[1]
    {
      value
    }, index);

    /// <summary>
    /// 设置ushort数组的数据到缓存区<br />
    /// Set ushort array data to the cache area
    /// </summary>
    /// <param name="values">ushort数组</param>
    /// <param name="index">索引位置</param>
    public void SetValue(ushort[] values, int index) => this.SetBytes(this.byteTransform.TransByte(values), index);

    /// <summary>
    /// 设置ushort类型的数据到缓存区<br />
    /// Set ushort type data to the cache area
    /// </summary>
    /// <param name="value">ushort数值</param>
    /// <param name="index">索引位置</param>
    public void SetValue(ushort value, int index) => this.SetValue(new ushort[1]
    {
      value
    }, index);

    /// <summary>
    /// 设置int数组的数据到缓存区<br />
    /// Set int array data to the cache area
    /// </summary>
    /// <param name="values">int数组</param>
    /// <param name="index">索引位置</param>
    public void SetValue(int[] values, int index) => this.SetBytes(this.byteTransform.TransByte(values), index);

    /// <summary>
    /// 设置int类型的数据到缓存区<br />
    /// Set int type data to the cache area
    /// </summary>
    /// <param name="value">int数值</param>
    /// <param name="index">索引位置</param>
    public void SetValue(int value, int index) => this.SetValue(new int[1]
    {
      value
    }, index);

    /// <summary>
    /// 设置uint数组的数据到缓存区<br />
    /// Set uint array data to the cache area
    /// </summary>
    /// <param name="values">uint数组</param>
    /// <param name="index">索引位置</param>
    public void SetValue(uint[] values, int index) => this.SetBytes(this.byteTransform.TransByte(values), index);

    /// <summary>
    /// 设置uint类型的数据到缓存区<br />
    /// Set uint byte data to the cache area
    /// </summary>
    /// <param name="value">uint数值</param>
    /// <param name="index">索引位置</param>
    public void SetValue(uint value, int index) => this.SetValue(new uint[1]
    {
      value
    }, index);

    /// <summary>
    /// 设置float数组的数据到缓存区<br />
    /// Set float array data to the cache area
    /// </summary>
    /// <param name="values">float数组</param>
    /// <param name="index">索引位置</param>
    public void SetValue(float[] values, int index) => this.SetBytes(this.byteTransform.TransByte(values), index);

    /// <summary>
    /// 设置float类型的数据到缓存区<br />
    /// Set float type data to the cache area
    /// </summary>
    /// <param name="value">float数值</param>
    /// <param name="index">索引位置</param>
    public void SetValue(float value, int index) => this.SetValue(new float[1]
    {
      value
    }, index);

    /// <summary>
    /// 设置long数组的数据到缓存区<br />
    /// Set long array data to the cache area
    /// </summary>
    /// <param name="values">long数组</param>
    /// <param name="index">索引位置</param>
    public void SetValue(long[] values, int index) => this.SetBytes(this.byteTransform.TransByte(values), index);

    /// <summary>
    /// 设置long类型的数据到缓存区<br />
    /// Set long type data to the cache area
    /// </summary>
    /// <param name="value">long数值</param>
    /// <param name="index">索引位置</param>
    public void SetValue(long value, int index) => this.SetValue(new long[1]
    {
      value
    }, index);

    /// <summary>
    /// 设置ulong数组的数据到缓存区<br />
    /// Set long array data to the cache area
    /// </summary>
    /// <param name="values">ulong数组</param>
    /// <param name="index">索引位置</param>
    public void SetValue(ulong[] values, int index) => this.SetBytes(this.byteTransform.TransByte(values), index);

    /// <summary>
    /// 设置ulong类型的数据到缓存区<br />
    /// Set ulong byte data to the cache area
    /// </summary>
    /// <param name="value">ulong数值</param>
    /// <param name="index">索引位置</param>
    public void SetValue(ulong value, int index) => this.SetValue(new ulong[1]
    {
      value
    }, index);

    /// <summary>
    /// 设置double数组的数据到缓存区<br />
    /// Set double array data to the cache area
    /// </summary>
    /// <param name="values">double数组</param>
    /// <param name="index">索引位置</param>
    public void SetValue(double[] values, int index) => this.SetBytes(this.byteTransform.TransByte(values), index);

    /// <summary>
    /// 设置double类型的数据到缓存区<br />
    /// Set double type data to the cache area
    /// </summary>
    /// <param name="value">double数值</param>
    /// <param name="index">索引位置</param>
    public void SetValue(double value, int index) => this.SetValue(new double[1]
    {
      value
    }, index);

    /// <summary>
    /// 获取byte类型的数据<br />
    /// Get byte data
    /// </summary>
    /// <param name="index">索引位置</param>
    /// <returns>byte数值</returns>
    public byte GetByte(int index) => this.GetBytes(index, 1)[0];

    /// <summary>
    /// 获取short类型的数组到缓存区<br />
    /// Get short type array to cache
    /// </summary>
    /// <param name="index">索引位置</param>
    /// <param name="length">数组长度</param>
    /// <returns>short数组</returns>
    public short[] GetInt16(int index, int length) => this.byteTransform.TransInt16(this.GetBytes(index, length * 2), 0, length);

    /// <summary>
    /// 获取short类型的数据到缓存区<br />
    /// Get short data to the cache
    /// </summary>
    /// <param name="index">索引位置</param>
    /// <returns>short数据</returns>
    public short GetInt16(int index) => this.GetInt16(index, 1)[0];

    /// <summary>
    /// 获取ushort类型的数组到缓存区<br />
    /// Get ushort type array to cache
    /// </summary>
    /// <param name="index">索引位置</param>
    /// <param name="length">数组长度</param>
    /// <returns>ushort数组</returns>
    public ushort[] GetUInt16(int index, int length) => this.byteTransform.TransUInt16(this.GetBytes(index, length * 2), 0, length);

    /// <summary>
    /// 获取ushort类型的数据到缓存区<br />
    /// Get ushort type data to cache
    /// </summary>
    /// <param name="index">索引位置</param>
    /// <returns>ushort数据</returns>
    public ushort GetUInt16(int index) => this.GetUInt16(index, 1)[0];

    /// <summary>
    /// 获取int类型的数组到缓存区<br />
    /// Get int type array to cache
    /// </summary>
    /// <param name="index">索引位置</param>
    /// <param name="length">数组长度</param>
    /// <returns>int数组</returns>
    public int[] GetInt32(int index, int length) => this.byteTransform.TransInt32(this.GetBytes(index, length * 4), 0, length);

    /// <summary>
    /// 获取int类型的数据到缓存区<br />
    /// Get int type data to cache
    /// </summary>
    /// <param name="index">索引位置</param>
    /// <returns>int数据</returns>
    public int GetInt32(int index) => this.GetInt32(index, 1)[0];

    /// <summary>
    /// 获取uint类型的数组到缓存区<br />
    /// Get uint type array to cache
    /// </summary>
    /// <param name="index">索引位置</param>
    /// <param name="length">数组长度</param>
    /// <returns>uint数组</returns>
    public uint[] GetUInt32(int index, int length) => this.byteTransform.TransUInt32(this.GetBytes(index, length * 4), 0, length);

    /// <summary>
    /// 获取uint类型的数据到缓存区<br />
    /// Get uint type data to cache
    /// </summary>
    /// <param name="index">索引位置</param>
    /// <returns>uint数据</returns>
    public uint GetUInt32(int index) => this.GetUInt32(index, 1)[0];

    /// <summary>
    /// 获取float类型的数组到缓存区<br />
    /// Get float type array to cache
    /// </summary>
    /// <param name="index">索引位置</param>
    /// <param name="length">数组长度</param>
    /// <returns>float数组</returns>
    public float[] GetSingle(int index, int length) => this.byteTransform.TransSingle(this.GetBytes(index, length * 4), 0, length);

    /// <summary>
    /// 获取float类型的数据到缓存区<br />
    /// Get float type data to cache
    /// </summary>
    /// <param name="index">索引位置</param>
    /// <returns>float数据</returns>
    public float GetSingle(int index) => this.GetSingle(index, 1)[0];

    /// <summary>
    /// 获取long类型的数组到缓存区<br />
    /// Get long type array to cache
    /// </summary>
    /// <param name="index">索引位置</param>
    /// <param name="length">数组长度</param>
    /// <returns>long数组</returns>
    public long[] GetInt64(int index, int length) => this.byteTransform.TransInt64(this.GetBytes(index, length * 8), 0, length);

    /// <summary>
    /// 获取long类型的数据到缓存区<br />
    /// Get long type data to cache
    /// </summary>
    /// <param name="index">索引位置</param>
    /// <returns>long数据</returns>
    public long GetInt64(int index) => this.GetInt64(index, 1)[0];

    /// <summary>
    /// 获取ulong类型的数组到缓存区<br />
    /// Get ulong type array to cache
    /// </summary>
    /// <param name="index">索引位置</param>
    /// <param name="length">数组长度</param>
    /// <returns>ulong数组</returns>
    public ulong[] GetUInt64(int index, int length) => this.byteTransform.TransUInt64(this.GetBytes(index, length * 8), 0, length);

    /// <summary>
    /// 获取ulong类型的数据到缓存区<br />
    /// Get ulong type data to cache
    /// </summary>
    /// <param name="index">索引位置</param>
    /// <returns>ulong数据</returns>
    public ulong GetUInt64(int index) => this.GetUInt64(index, 1)[0];

    /// <summary>
    /// 获取double类型的数组到缓存区<br />
    /// Get double type array to cache
    /// </summary>
    /// <param name="index">索引位置</param>
    /// <param name="length">数组长度</param>
    /// <returns>double数组</returns>
    public double[] GetDouble(int index, int length) => this.byteTransform.TransDouble(this.GetBytes(index, length * 8), 0, length);

    /// <summary>
    /// 获取double类型的数据到缓存区<br />
    /// Get double type data to cache
    /// </summary>
    /// <param name="index">索引位置</param>
    /// <returns>double数据</returns>
    public double GetDouble(int index) => this.GetDouble(index, 1)[0];

    /// <summary>
    /// 读取自定义类型的数据，需要规定解析规则<br />
    /// Read custom types of data, need to specify the parsing rules
    /// </summary>
    /// <typeparam name="T">类型名称</typeparam>
    /// <param name="index">起始索引</param>
    /// <returns>自定义的数据类型</returns>
    public T GetCustomer<T>(int index) where T : IDataTransfer, new()
    {
      T obj = new T();
      byte[] bytes = this.GetBytes(index, (int) obj.ReadCount);
      obj.ParseSource(bytes);
      return obj;
    }

    /// <summary>
    /// 写入自定义类型的数据到缓存中去，需要规定生成字节的方法<br />
    /// Write custom type data to the cache, need to specify the method of generating bytes
    /// </summary>
    /// <typeparam name="T">自定义类型</typeparam>
    /// <param name="data">实例对象</param>
    /// <param name="index">起始地址</param>
    public void SetCustomer<T>(T data, int index) where T : IDataTransfer, new() => this.SetBytes(data.ToSource(), index);

    /// <inheritdoc cref="P:EstCommunication.Core.Net.NetworkDoubleBase.ByteTransform" />
    public IByteTransform ByteTransform
    {
      get => this.byteTransform;
      set => this.byteTransform = value;
    }

    /// <summary>
    /// 获取或设置当前的bool操作是否按照字节反转<br />
    /// Gets or sets whether the current bool operation is reversed by bytes
    /// </summary>
    public bool IsBoolReverseByWord
    {
      get => this.isBoolReverseByWord;
      set => this.isBoolReverseByWord = value;
    }

    /// <summary>释放当前的对象</summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      if (disposing)
      {
        this.hybirdLock?.Dispose();
        this.buffer = (byte[]) null;
      }
      this.disposedValue = true;
    }

    /// <inheritdoc cref="M:System.IDisposable.Dispose" />
    public void Dispose() => this.Dispose(true);

    /// <inheritdoc />
    public override string ToString() => string.Format("SoftBuffer[{0}][{1}]", (object) this.capacity, (object) this.ByteTransform);
  }
}
