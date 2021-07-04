// Decompiled with JetBrains decompiler
// Type: EstCommunication.BasicFramework.SharpList`1
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.Core;
using System;
using System.Collections.Generic;

namespace EstCommunication.BasicFramework
{
  /// <summary>
  /// 一个高效的数组管理类，用于高效控制固定长度的数组实现<br />
  /// An efficient array management class for efficient control of fixed-length array implementations
  /// </summary>
  /// <typeparam name="T">泛型类型</typeparam>
  public class SharpList<T>
  {
    private T[] array;
    private int capacity = 2048;
    private int count = 0;
    private int lastIndex = 0;
    private SimpleHybirdLock hybirdLock;

    /// <summary>实例化一个对象，需要指定数组的最大数据对象</summary>
    /// <param name="count">数据的个数</param>
    /// <param name="appendLast">是否从最后一个数添加</param>
    public SharpList(int count, bool appendLast = false)
    {
      if (count > 8192)
        this.capacity = 4096;
      this.array = new T[this.capacity + count];
      this.hybirdLock = new SimpleHybirdLock();
      this.count = count;
      if (!appendLast)
        return;
      this.lastIndex = count;
    }

    /// <summary>
    /// 获取数组的个数<br />
    /// Get the number of arrays
    /// </summary>
    public int Count => this.count;

    /// <summary>
    /// 新增一个数据值<br />
    /// Add a data value
    /// </summary>
    /// <param name="value">数据值</param>
    public void Add(T value)
    {
      this.hybirdLock.Enter();
      if (this.lastIndex < this.capacity + this.count)
      {
        this.array[this.lastIndex++] = value;
      }
      else
      {
        T[] objArray = new T[this.capacity + this.count];
        Array.Copy((Array) this.array, this.capacity, (Array) objArray, 0, this.count);
        this.array = objArray;
        this.lastIndex = this.count;
      }
      this.hybirdLock.Leave();
    }

    /// <summary>
    /// 批量的增加数据<br />
    /// Increase data in batches
    /// </summary>
    /// <param name="values">批量数据信息</param>
    public void Add(IEnumerable<T> values)
    {
      foreach (T obj in values)
        this.Add(obj);
    }

    /// <summary>
    /// 获取数据的数组值<br />
    /// Get array value of data
    /// </summary>
    /// <returns>数组值</returns>
    public T[] ToArray()
    {
      this.hybirdLock.Enter();
      T[] objArray;
      if (this.lastIndex < this.count)
      {
        objArray = new T[this.lastIndex];
        Array.Copy((Array) this.array, 0, (Array) objArray, 0, this.lastIndex);
      }
      else
      {
        objArray = new T[this.count];
        Array.Copy((Array) this.array, this.lastIndex - this.count, (Array) objArray, 0, this.count);
      }
      this.hybirdLock.Leave();
      return objArray;
    }

    /// <summary>
    /// 获取或设置指定索引的位置的数据<br />
    /// Gets or sets the data at the specified index
    /// </summary>
    /// <param name="index">索引位置</param>
    /// <returns>数据值</returns>
    public T this[int index]
    {
      get
      {
        if (index < 0)
          throw new IndexOutOfRangeException("Index must larger than zero");
        if (index >= this.count)
          throw new IndexOutOfRangeException("Index must smaller than array length");
        T obj1 = default (T);
        this.hybirdLock.Enter();
        T obj2 = this.lastIndex >= this.count ? this.array[index + this.lastIndex - this.count] : this.array[index];
        this.hybirdLock.Leave();
        return obj2;
      }
      set
      {
        if (index < 0)
          throw new IndexOutOfRangeException("Index must larger than zero");
        if (index >= this.count)
          throw new IndexOutOfRangeException("Index must smaller than array length");
        this.hybirdLock.Enter();
        if (this.lastIndex < this.count)
          this.array[index] = value;
        else
          this.array[index + this.lastIndex - this.count] = value;
        this.hybirdLock.Leave();
      }
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("SharpList<{0}>[{1}]", (object) typeof (T), (object) this.capacity);
  }
}
