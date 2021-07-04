// Decompiled with JetBrains decompiler
// Type: EstCommunication.Algorithms.ConnectPool.ConnectPool`1
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using System;
using System.Collections.Generic;
using System.Threading;

namespace EstCommunication.Algorithms.ConnectPool
{
  /// <summary>
  /// 一个连接池管理器，负责维护多个可用的连接，并且自动清理，扩容，用于快速读写服务器或是PLC时使用。<br />
  /// A connection pool manager is responsible for maintaining multiple available connections,
  /// and automatically cleans up, expands, and is used to quickly read and write servers or PLCs.
  /// </summary>
  /// <typeparam name="TConnector">管理的连接类，需要支持IConnector接口</typeparam>
  /// <remarks>
  /// 需要先实现 <see cref="T:EstCommunication.Algorithms.ConnectPool.IConnector" /> 接口的对象，然后就可以实现真正的连接池了，理论上可以实现任意的连接对象，包括modbus连接对象，各种PLC连接对象，数据库连接对象，redis连接对象，SimplifyNet连接对象等等。下面的示例就是modbus-tcp的实现
  /// <note type="warning">要想真正的支持连接池访问，还需要服务器支持一个端口的多连接操作，三菱PLC的端口就不支持，如果要测试示例代码的连接池对象，需要使用本组件的<see cref="T:EstCommunication.ModBus.ModbusTcpServer" />来创建服务器对象</note>
  /// </remarks>
  /// <example>
  /// 下面举例实现一个modbus的连接池对象，先实现接口化的操作
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Algorithms\ConnectPool.cs" region="IConnector Example" title="IConnector示例" />
  /// 然后就可以实现真正的连接池了
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Algorithms\ConnectPool.cs" region="ConnectPoolExample" title="ConnectPool示例" />
  /// </example>
  public class ConnectPool<TConnector> where TConnector : IConnector
  {
    private Func<TConnector> CreateConnector = (Func<TConnector>) null;
    private int maxConnector = 10;
    private int usedConnector = 0;
    private int usedConnectorMax = 0;
    private int expireTime = 30;
    private bool canGetConnector = true;
    private Timer timerCheck = (Timer) null;
    private object listLock;
    private List<TConnector> connectors = (List<TConnector>) null;

    /// <summary>
    /// 实例化一个连接池对象，需要指定如果创建新实例的方法<br />
    /// To instantiate a connection pool object, you need to specify how to create a new instance
    /// </summary>
    /// <param name="createConnector">创建连接对象的委托</param>
    public ConnectPool(Func<TConnector> createConnector)
    {
      this.CreateConnector = createConnector;
      this.listLock = new object();
      this.connectors = new List<TConnector>();
      this.timerCheck = new Timer(new TimerCallback(this.TimerCheckBackground), (object) null, 10000, 30000);
    }

    /// <summary>
    /// 获取一个可用的连接对象，如果已经达到上限，就进行阻塞等待。当使用完连接对象的时候，需要调用<see cref="M:EstCommunication.Algorithms.ConnectPool.ConnectPool`1.ReturnConnector(`0)" />方法归还连接对象。<br />
    /// Get an available connection object, if the upper limit has been reached, block waiting. When the connection object is used up,
    /// you need to call the <see cref="M:EstCommunication.Algorithms.ConnectPool.ConnectPool`1.ReturnConnector(`0)" /> method to return the connection object.
    /// </summary>
    /// <returns>可用的连接对象</returns>
    public TConnector GetAvailableConnector()
    {
      while (!this.canGetConnector)
        Thread.Sleep(20);
      TConnector connector1 = default (TConnector);
      lock (this.listLock)
      {
        for (int index = 0; index < this.connectors.Count; ++index)
        {
          TConnector connector2 = this.connectors[index];
          if (!connector2.IsConnectUsing)
          {
            connector2 = this.connectors[index];
            connector2.IsConnectUsing = true;
            connector1 = this.connectors[index];
            break;
          }
        }
        if ((object) connector1 == null)
        {
          connector1 = this.CreateConnector();
          connector1.IsConnectUsing = true;
          connector1.LastUseTime = DateTime.Now;
          connector1.Open();
          this.connectors.Add(connector1);
          this.usedConnector = this.connectors.Count;
          if (this.usedConnector > this.usedConnectorMax)
            this.usedConnectorMax = this.usedConnector;
          if (this.usedConnector == this.maxConnector)
            this.canGetConnector = false;
        }
        connector1.LastUseTime = DateTime.Now;
      }
      return connector1;
    }

    /// <summary>
    /// 使用完之后需要通知连接池的管理器，本方法调用之前需要获取到连接对象信息。<br />
    /// After using it, you need to notify the manager of the connection pool, and you need to get the connection object information before calling this method.
    /// </summary>
    /// <param name="connector">连接对象</param>
    public void ReturnConnector(TConnector connector)
    {
      lock (this.listLock)
      {
        int index = this.connectors.IndexOf(connector);
        if (index == -1)
          return;
        this.connectors[index].IsConnectUsing = false;
      }
    }

    /// <summary>
    /// 将目前连接中的所有对象进行关闭，然后移除队列。<br />
    /// Close all objects in the current connection, and then remove the queue.
    /// </summary>
    public void ResetAllConnector()
    {
      lock (this.listLock)
      {
        for (int index = this.connectors.Count - 1; index >= 0; --index)
        {
          this.connectors[index].Close();
          this.connectors.RemoveAt(index);
        }
      }
    }

    /// <summary>
    /// 获取或设置最大的连接数，当实际的连接数超过最大的连接数的时候，就会进行阻塞，直到有新的连接对象为止。<br />
    /// Get or set the maximum number of connections. When the actual number of connections exceeds the maximum number of connections,
    /// it will block until there is a new connection object.
    /// </summary>
    public int MaxConnector
    {
      get => this.maxConnector;
      set => this.maxConnector = value;
    }

    /// <summary>
    /// 获取或设置当前连接过期的时间，单位秒，默认30秒，也就是说，当前的连接在设置的时间段内未被使用，就进行释放连接，减少内存消耗。<br />
    /// Get or set the expiration time of the current connection, in seconds, the default is 30 seconds, that is,
    /// if the current connection is not used within the set time period, the connection will be released to reduce memory consumption.
    /// </summary>
    public int ConectionExpireTime
    {
      get => this.expireTime;
      set => this.expireTime = value;
    }

    /// <summary>
    /// 当前已经使用的连接数，会根据使用的频繁程度进行动态的变化。<br />
    /// The number of currently used connections will dynamically change according to the frequency of use.
    /// </summary>
    public int UsedConnector => this.usedConnector;

    /// <summary>
    /// 当前已经使用的连接数的峰值，可以用来衡量当前系统的适用的连接池上限。<br />
    /// The current peak value of the number of connections used can be used to measure the upper limit of the applicable connection pool of the current system.
    /// </summary>
    public int UseConnectorMax => this.usedConnectorMax;

    private void TimerCheckBackground(object obj)
    {
      lock (this.listLock)
      {
        for (int index = this.connectors.Count - 1; index >= 0; --index)
        {
          DateTime now = DateTime.Now;
          TConnector connector = this.connectors[index];
          DateTime lastUseTime = connector.LastUseTime;
          int num;
          if ((now - lastUseTime).TotalSeconds > (double) this.expireTime)
          {
            connector = this.connectors[index];
            num = !connector.IsConnectUsing ? 1 : 0;
          }
          else
            num = 0;
          if (num != 0)
          {
            connector = this.connectors[index];
            connector.Close();
            this.connectors.RemoveAt(index);
          }
        }
        this.usedConnector = this.connectors.Count;
        if (this.usedConnector >= this.MaxConnector)
          return;
        this.canGetConnector = true;
      }
    }
  }
}
