// Decompiled with JetBrains decompiler
// Type: EstCommunication.Core.Singleton
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using System.Threading;

namespace EstCommunication.Core
{
  /// <summary>一个双检锁的示例，适合一些占内存的静态数据对象，获取的时候才实例化真正的对象</summary>
  internal sealed class Singleton
  {
    private static object m_lock = new object();
    private static Singleton SValue = (Singleton) null;

    public static Singleton GetSingleton()
    {
      if (Singleton.SValue != null)
        return Singleton.SValue;
      Monitor.Enter(Singleton.m_lock);
      if (Singleton.SValue == null)
      {
        Singleton singleton = new Singleton();
        Volatile.Write<Singleton>(ref Singleton.SValue, singleton);
        Singleton.SValue = new Singleton();
      }
      Monitor.Exit(Singleton.m_lock);
      return Singleton.SValue;
    }
  }
}
