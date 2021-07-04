// Decompiled with JetBrains decompiler
// Type: EstCommunication.BasicFramework.SoftMsgQueue`1
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace EstCommunication.BasicFramework
{
  /// <summary>一个简单通用的消息队列</summary>
  /// <typeparam name="T">类型</typeparam>
  public class SoftMsgQueue<T> : SoftFileSaveBase
  {
    /// <summary>所有临时存储的数据</summary>
    private Queue<T> all_items = new Queue<T>();
    private int m_Max_Cache = 200;
    /// <summary>将集合进行锁定</summary>
    private object lock_queue = new object();

    /// <summary>实例化一个对象</summary>
    public SoftMsgQueue() => this.LogHeaderText = "SoftMsgQueue<" + typeof (T).ToString() + ">";

    /// <summary>临时消息存储的最大条数，必须大于10</summary>
    public int MaxCache
    {
      get => this.m_Max_Cache;
      set
      {
        if (value <= 10)
          return;
        this.m_Max_Cache = value;
      }
    }

    /// <summary>获取最新添加进去的数据</summary>
    public T CurrentItem => this.all_items.Count > 0 ? this.all_items.Peek() : default (T);

    /// <summary>新增一条数据</summary>
    public void AddNewItem(T item)
    {
      lock (this.lock_queue)
      {
        while (this.all_items.Count >= this.m_Max_Cache)
          this.all_items.Dequeue();
        this.all_items.Enqueue(item);
      }
    }

    /// <summary>获取存储字符串</summary>
    /// <returns></returns>
    public override string ToSaveString() => JArray.FromObject((object) this.all_items).ToString();

    /// <summary>获取加载字符串</summary>
    /// <param name="content"></param>
    public override void LoadByString(string content) => this.all_items = (Queue<T>) JArray.Parse(content).ToObject(typeof (Queue<T>));
  }
}
