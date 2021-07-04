// Decompiled with JetBrains decompiler
// Type: EstCommunication.Core.IReadWriteNet
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.LogNet;
using System;
using System.Text;
using System.Threading.Tasks;

namespace EstCommunication.Core
{
  /// <summary>
  /// 所有的和设备或是交互类统一读写标准，公开了如何读写对方的一些api接口，并支持基于特性的读写操作<br />
  /// All unified read and write standards for devices and interaction classes,
  /// expose how to read and write some API interfaces of each other, and support feature-based read and write operations
  /// </summary>
  /// <remarks>Modbus类，PLC类均实现了本接口，可以基于本接口实现统一所有的不同种类的设备的数据交互</remarks>
  /// <example>
  /// 此处举例实现modbus，三菱，西门子三种设备的统一的数据交互
  /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\IReadWriteNet.cs" region="IReadWriteNetExample" title="IReadWriteNet示例" />
  /// </example>
  public interface IReadWriteNet
  {
    /// <summary>
    /// 组件的日志工具，支持日志记录，只要实例化后，当前网络的基本信息，就以<see cref="F:EstCommunication.LogNet.EstMessageDegree.DEBUG" />等级进行输出<br />
    /// The component's logging tool supports logging. As long as the instantiation of the basic network information, the output will be output at <see cref="F:EstCommunication.LogNet.EstMessageDegree.DEBUG" />
    /// </summary>
    /// <remarks>
    /// 只要实例化即可以记录日志，实例化的对象需要实现接口 <see cref="T:EstCommunication.LogNet.ILogNet" /> ，本组件提供了三个日志记录类，你可以实现基于 <see cref="T:EstCommunication.LogNet.ILogNet" />  的对象。</remarks>
    /// <example>
    /// 如下的实例化适用于所有的Network及其派生类，以下举两个例子，三菱的设备类及服务器类
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkBase.cs" region="LogNetExample1" title="LogNet示例" />
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkBase.cs" region="LogNetExample2" title="LogNet示例" />
    /// </example>
    ILogNet LogNet { get; set; }

    /// <summary>
    /// 当前连接的唯一ID号，默认为长度20的guid码加随机数组成，方便列表管理，也可以自己指定<br />
    /// The unique ID number of the current connection. The default is a 20-digit guid code plus a random number.
    /// </summary>
    string ConnectionId { get; set; }

    /// <summary>
    /// 批量读取字节数组信息，需要指定地址和长度，返回原始的字节数组<br />
    /// Batch read byte array information, need to specify the address and length, return the original byte array
    /// </summary>
    /// <param name="address">数据地址</param>
    /// <param name="length">数据长度</param>
    /// <returns>带有成功标识的byte[]数组</returns>
    OperateResult<byte[]> Read(string address, ushort length);

    /// <summary>
    /// 写入原始的byte数组数据到指定的地址，返回是否写入成功<br />
    /// Write the original byte array data to the specified address, and return whether the write was successful
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="value">写入值</param>
    /// <returns>是否写入成功</returns>
    OperateResult Write(string address, byte[] value);

    /// <summary>
    /// 批量读取<see cref="T:System.Boolean" />数组信息，需要指定地址和长度，返回<see cref="T:System.Boolean" /> 数组<br />
    /// Batch read <see cref="T:System.Boolean" /> array information, need to specify the address and length, return <see cref="T:System.Boolean" /> array
    /// </summary>
    /// <param name="address">数据地址</param>
    /// <param name="length">数据长度</param>
    /// <returns>带有成功标识的 bool[] 数组</returns>
    OperateResult<bool[]> ReadBool(string address, ushort length);

    /// <summary>
    /// 读取单个的<see cref="T:System.Boolean" />数据信息<br />
    /// Read a single <see cref="T:System.Boolean" /> data message
    /// </summary>
    /// <param name="address">数据地址</param>
    /// <returns>带有成功标识的 bool 值</returns>
    OperateResult<bool> ReadBool(string address);

    /// <summary>
    /// 批量写入<see cref="T:System.Boolean" />数组数据，返回是否成功<br />
    /// Batch write <see cref="T:System.Boolean" /> array data, return whether the write was successful
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="value">写入值</param>
    /// <returns>是否写入成功</returns>
    OperateResult Write(string address, bool[] value);

    /// <summary>
    /// 写入单个的<see cref="T:System.Boolean" />数据，返回是否成功<br />
    /// Write a single <see cref="T:System.Boolean" /> data, and return whether the write was successful
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="value">写入值</param>
    /// <returns>是否写入成功</returns>
    OperateResult Write(string address, bool value);

    /// <summary>
    /// 读取16位的有符号的整型数据<br />
    /// Read 16-bit signed integer data
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <returns>带有成功标识的short数据</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadInt16" title="Int16类型示例" />
    /// </example>
    OperateResult<short> ReadInt16(string address);

    /// <summary>
    /// 读取16位的有符号整型数组<br />
    /// Read 16-bit signed integer array
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="length">读取的数组长度</param>
    /// <returns>带有成功标识的short数组</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadInt16Array" title="Int16类型示例" />
    /// </example>
    OperateResult<short[]> ReadInt16(string address, ushort length);

    /// <summary>
    /// 读取16位的无符号整型<br />
    /// Read 16-bit unsigned integer
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <returns>带有成功标识的ushort数据</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadUInt16" title="UInt16类型示例" />
    /// </example>
    OperateResult<ushort> ReadUInt16(string address);

    /// <summary>
    /// 读取16位的无符号整型数组<br />
    /// Read 16-bit unsigned integer array
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="length">读取的数组长度</param>
    /// <returns>带有成功标识的ushort数组</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadUInt16Array" title="UInt16类型示例" />
    /// </example>
    OperateResult<ushort[]> ReadUInt16(string address, ushort length);

    /// <summary>
    /// 读取32位的有符号整型<br />
    /// Read 32-bit signed integer
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <returns>带有成功标识的int数据</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadInt32" title="Int32类型示例" />
    /// </example>
    OperateResult<int> ReadInt32(string address);

    /// <summary>
    /// 读取32位有符号整型数组<br />
    /// Read 32-bit signed integer array
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="length">数组长度</param>
    /// <returns>带有成功标识的int数组</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadInt32Array" title="Int32类型示例" />
    /// </example>
    OperateResult<int[]> ReadInt32(string address, ushort length);

    /// <summary>
    /// 读取32位的无符号整型<br />
    /// Read 32-bit unsigned integer
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <returns>带有成功标识的uint数据</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadUInt32" title="UInt32类型示例" />
    /// </example>
    OperateResult<uint> ReadUInt32(string address);

    /// <summary>
    /// 读取32位的无符号整型数组<br />
    /// Read 32-bit unsigned integer array
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="length">数组长度</param>
    /// <returns>带有成功标识的uint数组</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadUInt32Array" title="UInt32类型示例" />
    /// </example>
    OperateResult<uint[]> ReadUInt32(string address, ushort length);

    /// <summary>
    /// 读取64位的有符号整型<br />
    /// Read 64-bit signed integer
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <returns>带有成功标识的long数据</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadInt64" title="Int64类型示例" />
    /// </example>
    OperateResult<long> ReadInt64(string address);

    /// <summary>
    /// 读取64位的有符号整型数组<br />
    /// Read 64-bit signed integer array
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="length">数组长度</param>
    /// <returns>带有成功标识的long数组</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadInt64Array" title="Int64类型示例" />
    /// </example>
    OperateResult<long[]> ReadInt64(string address, ushort length);

    /// <summary>
    /// 读取64位的无符号整型<br />
    /// Read 64-bit unsigned integer
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <returns>带有成功标识的ulong数据</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadUInt64" title="UInt64类型示例" />
    /// </example>
    OperateResult<ulong> ReadUInt64(string address);

    /// <summary>
    /// 读取64位的无符号整型的数组<br />
    /// Read 64-bit unsigned integer array
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="length">数组长度</param>
    /// <returns>带成功标志的结果数据对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadUInt64Array" title="UInt64类型示例" />
    /// </example>
    OperateResult<ulong[]> ReadUInt64(string address, ushort length);

    /// <summary>
    /// 读取单浮点数据<br />
    /// Read single floating point data
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <returns>带有成功标识的float数据</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadFloat" title="Float类型示例" />
    /// </example>
    OperateResult<float> ReadFloat(string address);

    /// <summary>
    /// 读取单浮点精度的数组<br />
    /// Read single floating point array
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="length">数组长度</param>
    /// <returns>带有成功标识的float数组</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadFloatArray" title="Float类型示例" />
    /// </example>
    OperateResult<float[]> ReadFloat(string address, ushort length);

    /// <summary>
    /// 读取双浮点的数据<br />
    /// Read double floating point data
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <returns>带有成功标识的double数据</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadDouble" title="Double类型示例" />
    /// </example>
    OperateResult<double> ReadDouble(string address);

    /// <summary>
    /// 读取双浮点数据的数组<br />
    /// Read double floating point data array
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="length">数组长度</param>
    /// <returns>带有成功标识的double数组</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadDoubleArray" title="Double类型示例" />
    /// </example>
    OperateResult<double[]> ReadDouble(string address, ushort length);

    /// <summary>
    /// 读取字符串数据，默认为最常见的ASCII编码<br />
    /// Read string data, default is the most common ASCII encoding
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="length">数据长度</param>
    /// <returns>带有成功标识的string数据</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadString" title="String类型示例" />
    /// </example>
    OperateResult<string> ReadString(string address, ushort length);

    /// <summary>
    /// 使用指定的编码，读取字符串数据<br />
    /// Reads string data using the specified encoding
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="length">数据长度</param>
    /// <param name="encoding">指定的自定义的编码</param>
    /// <returns>带有成功标识的string数据</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadStringEncoding" title="String类型示例" />
    /// </example>
    OperateResult<string> ReadString(string address, ushort length, Encoding encoding);

    /// <summary>
    /// 等待指定地址的<see cref="T:System.Boolean" />值为指定的值，可以指定刷新数据的频率，等待的超时时间，如果超时时间为-1的话，则是无期限等待。<br />
    /// Waiting for the <see cref="T:System.Boolean" /> value of the specified address to be the specified value, you can specify the frequency of refreshing the data,
    /// and the timeout time to wait. If the timeout time is -1, it is an indefinite wait.
    /// </summary>
    /// <param name="address">其实地址</param>
    /// <param name="waitValue">等待检测是值</param>
    /// <param name="readInterval">读取的频率</param>
    /// <param name="waitTimeout">等待的超时时间，如果超时时间为-1的话，则是无期限等待。</param>
    /// <returns>是否等待成功的结果对象，一旦通信失败，或是等待超时就返回失败。否则返回成功，并告知调用方等待了多久。</returns>
    OperateResult<TimeSpan> Wait(
      string address,
      bool waitValue,
      int readInterval,
      int waitTimeout);

    /// <summary>
    /// 等待指定地址的<see cref="T:System.Int16" />值为指定的值，可以指定刷新数据的频率，等待的超时时间，如果超时时间为-1的话，则是无期限等待。<br />
    /// Waiting for the <see cref="T:System.Int16" /> value of the specified address to be the specified value, you can specify the frequency of refreshing the data,
    /// and the timeout time to wait. If the timeout time is -1, it is an indefinite wait.
    /// </summary>
    /// <param name="address">其实地址</param>
    /// <param name="waitValue">等待检测是值</param>
    /// <param name="readInterval">读取的频率</param>
    /// <param name="waitTimeout">等待的超时时间，如果超时时间为-1的话，则是无期限等待。</param>
    /// <returns>是否等待成功的结果对象，一旦通信失败，或是等待超时就返回失败。否则返回成功，并告知调用方等待了多久。</returns>
    OperateResult<TimeSpan> Wait(
      string address,
      short waitValue,
      int readInterval,
      int waitTimeout);

    /// <summary>
    /// 等待指定地址的<see cref="T:System.UInt16" />值为指定的值，可以指定刷新数据的频率，等待的超时时间，如果超时时间为-1的话，则是无期限等待。<br />
    /// Waiting for the <see cref="T:System.UInt16" /> value of the specified address to be the specified value, you can specify the frequency of refreshing the data,
    /// and the timeout time to wait. If the timeout time is -1, it is an indefinite wait.
    /// </summary>
    /// <param name="address">其实地址</param>
    /// <param name="waitValue">等待检测是值</param>
    /// <param name="readInterval">读取的频率</param>
    /// <param name="waitTimeout">等待的超时时间，如果超时时间为-1的话，则是无期限等待。</param>
    /// <returns>是否等待成功的结果对象，一旦通信失败，或是等待超时就返回失败。否则返回成功，并告知调用方等待了多久。</returns>
    OperateResult<TimeSpan> Wait(
      string address,
      ushort waitValue,
      int readInterval,
      int waitTimeout);

    /// <summary>
    /// 等待指定地址的<see cref="T:System.Int32" />值为指定的值，可以指定刷新数据的频率，等待的超时时间，如果超时时间为-1的话，则是无期限等待。<br />
    /// Waiting for the <see cref="T:System.Int32" /> value of the specified address to be the specified value, you can specify the frequency of refreshing the data,
    /// and the timeout time to wait. If the timeout time is -1, it is an indefinite wait.
    /// </summary>
    /// <param name="address">其实地址</param>
    /// <param name="waitValue">等待检测是值</param>
    /// <param name="readInterval">读取的频率</param>
    /// <param name="waitTimeout">等待的超时时间，如果超时时间为-1的话，则是无期限等待。</param>
    /// <returns>是否等待成功的结果对象，一旦通信失败，或是等待超时就返回失败。否则返回成功，并告知调用方等待了多久。</returns>
    OperateResult<TimeSpan> Wait(
      string address,
      int waitValue,
      int readInterval,
      int waitTimeout);

    /// <summary>
    /// 等待指定地址的<see cref="T:System.UInt32" />值为指定的值，可以指定刷新数据的频率，等待的超时时间，如果超时时间为-1的话，则是无期限等待。<br />
    /// Waiting for the <see cref="T:System.UInt32" /> value of the specified address to be the specified value, you can specify the frequency of refreshing the data,
    /// and the timeout time to wait. If the timeout time is -1, it is an indefinite wait.
    /// </summary>
    /// <param name="address">其实地址</param>
    /// <param name="waitValue">等待检测是值</param>
    /// <param name="readInterval">读取的频率</param>
    /// <param name="waitTimeout">等待的超时时间，如果超时时间为-1的话，则是无期限等待。</param>
    /// <returns>是否等待成功的结果对象，一旦通信失败，或是等待超时就返回失败。否则返回成功，并告知调用方等待了多久。</returns>
    OperateResult<TimeSpan> Wait(
      string address,
      uint waitValue,
      int readInterval,
      int waitTimeout);

    /// <summary>
    /// 等待指定地址的<see cref="T:System.Int64" />值为指定的值，可以指定刷新数据的频率，等待的超时时间，如果超时时间为-1的话，则是无期限等待。<br />
    /// Waiting for the <see cref="T:System.Int64" /> value of the specified address to be the specified value, you can specify the frequency of refreshing the data,
    /// and the timeout time to wait. If the timeout time is -1, it is an indefinite wait.
    /// </summary>
    /// <param name="address">其实地址</param>
    /// <param name="waitValue">等待检测是值</param>
    /// <param name="readInterval">读取的频率</param>
    /// <param name="waitTimeout">等待的超时时间，如果超时时间为-1的话，则是无期限等待。</param>
    /// <returns>是否等待成功的结果对象，一旦通信失败，或是等待超时就返回失败。否则返回成功，并告知调用方等待了多久。</returns>
    OperateResult<TimeSpan> Wait(
      string address,
      long waitValue,
      int readInterval,
      int waitTimeout);

    /// <summary>
    /// 等待指定地址的<see cref="T:System.UInt64" />值为指定的值，可以指定刷新数据的频率，等待的超时时间，如果超时时间为-1的话，则是无期限等待。<br />
    /// Waiting for the <see cref="T:System.UInt64" /> value of the specified address to be the specified value, you can specify the frequency of refreshing the data,
    /// and the timeout time to wait. If the timeout time is -1, it is an indefinite wait.
    /// </summary>
    /// <param name="address">其实地址</param>
    /// <param name="waitValue">等待检测是值</param>
    /// <param name="readInterval">读取的频率</param>
    /// <param name="waitTimeout">等待的超时时间，如果超时时间为-1的话，则是无期限等待。</param>
    /// <returns>是否等待成功的结果对象，一旦通信失败，或是等待超时就返回失败。否则返回成功，并告知调用方等待了多久。</returns>
    OperateResult<TimeSpan> Wait(
      string address,
      ulong waitValue,
      int readInterval,
      int waitTimeout);

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Wait(System.String,System.Boolean,System.Int32,System.Int32)" />
    Task<OperateResult<TimeSpan>> WaitAsync(
      string address,
      bool waitValue,
      int readInterval,
      int waitTimeout);

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Wait(System.String,System.Int16,System.Int32,System.Int32)" />
    Task<OperateResult<TimeSpan>> WaitAsync(
      string address,
      short waitValue,
      int readInterval,
      int waitTimeout);

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Wait(System.String,System.UInt16,System.Int32,System.Int32)" />
    Task<OperateResult<TimeSpan>> WaitAsync(
      string address,
      ushort waitValue,
      int readInterval,
      int waitTimeout);

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Wait(System.String,System.Int32,System.Int32,System.Int32)" />
    Task<OperateResult<TimeSpan>> WaitAsync(
      string address,
      int waitValue,
      int readInterval,
      int waitTimeout);

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Wait(System.String,System.UInt32,System.Int32,System.Int32)" />
    Task<OperateResult<TimeSpan>> WaitAsync(
      string address,
      uint waitValue,
      int readInterval,
      int waitTimeout);

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Wait(System.String,System.Int64,System.Int32,System.Int32)" />
    Task<OperateResult<TimeSpan>> WaitAsync(
      string address,
      long waitValue,
      int readInterval,
      int waitTimeout);

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Wait(System.String,System.UInt64,System.Int32,System.Int32)" />
    Task<OperateResult<TimeSpan>> WaitAsync(
      string address,
      ulong waitValue,
      int readInterval,
      int waitTimeout);

    /// <summary>
    /// 写入short数据，返回是否成功<br />
    /// Write short data, returns whether success
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="value">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteInt16" title="Int16类型示例" />
    /// </example>
    OperateResult Write(string address, short value);

    /// <summary>
    /// 写入short数组，返回是否成功<br />
    /// Write short array, return whether the write was successful
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="values">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteInt16Array" title="Int16类型示例" />
    /// </example>
    OperateResult Write(string address, short[] values);

    /// <summary>
    /// 写入ushort数据，返回是否成功<br />
    /// Write ushort data, return whether the write was successful
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="value">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteUInt16" title="UInt16类型示例" />
    /// </example>
    OperateResult Write(string address, ushort value);

    /// <summary>
    /// 写入ushort数组，返回是否成功<br />
    /// Write ushort array, return whether the write was successful
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="values">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteUInt16Array" title="UInt16类型示例" />
    /// </example>
    OperateResult Write(string address, ushort[] values);

    /// <summary>
    /// 写入int数据，返回是否成功<br />
    /// Write int data, return whether the write was successful
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="value">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteInt32" title="Int32类型示例" />
    /// </example>
    OperateResult Write(string address, int value);

    /// <summary>
    /// 写入int[]数组，返回是否成功<br />
    /// Write int array, return whether the write was successful
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="values">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteInt32Array" title="Int32类型示例" />
    /// </example>
    OperateResult Write(string address, int[] values);

    /// <summary>
    /// 写入uint数据，返回是否成功<br />
    /// Write uint data, return whether the write was successful
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="value">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteUInt32" title="UInt32类型示例" />
    /// </example>
    OperateResult Write(string address, uint value);

    /// <summary>
    /// 写入uint[]数组，返回是否成功<br />
    /// Write uint array, return whether the write was successful
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="values">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteUInt32Array" title="UInt32类型示例" />
    /// </example>
    OperateResult Write(string address, uint[] values);

    /// <summary>
    /// 写入long数据，返回是否成功<br />
    /// Write long data, return whether the write was successful
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="value">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteInt64" title="Int64类型示例" />
    /// </example>
    OperateResult Write(string address, long value);

    /// <summary>
    /// 写入long数组，返回是否成功<br />
    /// Write long array, return whether the write was successful
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="values">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteInt64Array" title="Int64类型示例" />
    /// </example>
    OperateResult Write(string address, long[] values);

    /// <summary>
    /// 写入ulong数据，返回是否成功<br />
    /// Write ulong data, return whether the write was successful
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="value">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteUInt64" title="UInt64类型示例" />
    /// </example>
    OperateResult Write(string address, ulong value);

    /// <summary>
    /// 写入ulong数组，返回是否成功<br />
    /// Write ulong array, return whether the write was successful
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="values">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteUInt64Array" title="UInt64类型示例" />
    /// </example>
    OperateResult Write(string address, ulong[] values);

    /// <summary>
    /// 写入float数据，返回是否成功<br />
    /// Write float data, return whether the write was successful
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="value">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteFloat" title="Float类型示例" />
    /// </example>
    OperateResult Write(string address, float value);

    /// <summary>
    /// 写入float数组，返回是否成功<br />
    /// Write float array, return whether the write was successful
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="values">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteFloatArray" title="Float类型示例" />
    /// </example>
    OperateResult Write(string address, float[] values);

    /// <summary>
    /// 写入double数据，返回是否成功<br />
    /// Write double data, return whether the write was successful
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="value">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteDouble" title="Double类型示例" />
    /// </example>
    OperateResult Write(string address, double value);

    /// <summary>
    /// 写入double数组，返回是否成功<br />
    /// Write double array, return whether the write was successful
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="values">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteDoubleArray" title="Double类型示例" />
    /// </example>
    OperateResult Write(string address, double[] values);

    /// <summary>
    /// 写入字符串信息，编码为ASCII<br />
    /// Write string information, encoded as ASCII
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="value">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteString" title="String类型示例" />
    /// </example>
    OperateResult Write(string address, string value);

    /// <summary>
    /// 写入字符串信息，需要指定的编码信息<br />
    /// Write string information, need to specify the encoding information
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="value">写入值</param>
    /// <param name="encoding">指定的编码信息</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteString" title="String类型示例" />
    /// </example>
    OperateResult Write(string address, string value, Encoding encoding);

    /// <summary>
    /// 写入指定长度的字符串信息，如果超出，就截断字符串，如果长度不足，那就补0操作，编码为ASCII<br />
    /// Write string information of the specified length. If it exceeds the value, the string is truncated.
    /// If the length is not enough, it is filled with 0 and the encoding is ASCII.
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="value">写入值</param>
    /// <param name="length">字符串的长度</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteString2" title="String类型示例" />
    /// </example>
    OperateResult Write(string address, string value, int length);

    /// <summary>
    /// 写入指定长度的字符串信息，如果超出，就截断字符串，如果长度不足，那就补0操作，编码为指定的编码信息<br />
    /// Write string information of the specified length. If it exceeds the value, the string is truncated. If the length is not enough,
    /// then the operation is complemented with 0 , you should specified the encoding information
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="value">写入值</param>
    /// <param name="length">字符串的长度</param>
    /// <param name="encoding">指定的编码信息</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteString2" title="String类型示例" />
    /// </example>
    OperateResult Write(string address, string value, int length, Encoding encoding);

    /// <summary>
    /// 读取自定义的数据类型，需要继承自IDataTransfer接口<br />
    /// Read custom data types, need to inherit from IDataTransfer interface
    /// </summary>
    /// <typeparam name="T">自定义的类型</typeparam>
    /// <param name="address">起始地址</param>
    /// <returns>带有成功标识的自定义类型数据</returns>
    /// <remarks>需要是定义一个类，选择好相对于的ByteTransform实例，才能调用该方法。</remarks>
    /// <example>
    /// 此处演示三菱的读取示例，先定义一个类，实现<see cref="T:EstCommunication.IDataTransfer" />接口
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="IDataTransfer Example" title="DataMy示例" />
    /// 接下来就可以实现数据的读取了
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadCustomerExample" title="ReadCustomer示例" />
    /// </example>
    OperateResult<T> ReadCustomer<T>(string address) where T : IDataTransfer, new();

    /// <summary>
    /// 写入自定义类型的数据，该类型必须继承自IDataTransfer接口<br />
    /// Write data of a custom type, which must inherit from the IDataTransfer interface
    /// </summary>
    /// <typeparam name="T">类型对象</typeparam>
    /// <param name="address">起始地址</param>
    /// <param name="value">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <remarks>
    /// 需要是定义一个类，选择好相对于的<see cref="T:EstCommunication.IDataTransfer" />实例，才能调用该方法。
    /// </remarks>
    /// <example>
    /// 此处演示三菱的读取示例，先定义一个类，实现<see cref="T:EstCommunication.IDataTransfer" />接口
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="IDataTransfer Example" title="DataMy示例" />
    /// 接下来就可以实现数据的读取了
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteCustomerExample" title="WriteCustomer示例" />
    /// </example>
    OperateResult WriteCustomer<T>(string address, T value) where T : IDataTransfer, new();

    /// <summary>
    /// 读取支持Est特性的数据内容，该特性为<see cref="T:EstCommunication.Reflection.EstDeviceAddressAttribute" />，详细参考api文档说明<br />
    /// Read the data content of the Est attribute. The attribute is <see cref="T:EstCommunication.Reflection.EstDeviceAddressAttribute" />, please refer to the api documentation for details.
    /// </summary>
    /// <typeparam name="T">自定义的数据类型对象</typeparam>
    /// <returns>包含是否成功的结果对象</returns>
    /// <example>
    /// 此处演示西门子的读取示例，先定义一个类，重点是将需要读取的数据，写入到属性的特性中去。
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ObjectDefineExample" title="特性实现示例" />
    /// 接下来就可以实现数据的读取了
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadObjectExample" title="ReadObject示例" />
    /// </example>
    OperateResult<T> Read<T>() where T : class, new();

    /// <summary>
    /// 写入支持Est特性的数据，返回是否写入成功，该特性为<see cref="T:EstCommunication.Reflection.EstDeviceAddressAttribute" />，详细参考api文档说明<br />
    /// Write data that supports the Est attribute, and return whether the write was successful. The attribute is <see cref="T:EstCommunication.Reflection.EstDeviceAddressAttribute" />, please refer to the api documentation for details.
    /// </summary>
    /// <typeparam name="T">自定义的数据类型对象</typeparam>
    /// <returns>包含是否成功的结果对象</returns>
    /// <example>
    /// 此处演示西门子的读取示例，先定义一个类，重点是将需要读取的数据，写入到属性的特性中去。
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ObjectDefineExample" title="特性实现示例" />
    /// 接下来就可以实现数据的写入了
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteObjectExample" title="WriteObject示例" />
    /// </example>
    /// <exception cref="T:System.ArgumentNullException"></exception>
    OperateResult Write<T>(T data) where T : class, new();

    /// <summary>
    /// 异步批量读取字节数组信息，需要指定地址和长度，返回原始的字节数组<br />
    /// Asynchronous batch read byte array information, need to specify the address and length, return the original byte array
    /// </summary>
    /// <param name="address">数据地址</param>
    /// <param name="length">数据长度</param>
    /// <returns>带有成功标识的byte[]数组</returns>
    Task<OperateResult<byte[]>> ReadAsync(string address, ushort length);

    /// <summary>
    /// 异步写入原始的byte数组数据到指定的地址，返回是否写入成功<br />
    /// Asynchronously writes the original byte array data to the specified address, and returns whether the write was successful
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="value">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteAsync" title="bytes类型示例" />
    /// </example>
    Task<OperateResult> WriteAsync(string address, byte[] value);

    /// <summary>
    /// 异步批量读取<see cref="T:System.Boolean" />数组信息，需要指定地址和长度，返回<see cref="T:System.Boolean" /> 数组<br />
    /// Asynchronously batch read <see cref="T:System.Boolean" /> array information, need to specify the address and length, return <see cref="T:System.Boolean" /> array
    /// </summary>
    /// <param name="address">数据地址</param>
    /// <param name="length">数据长度</param>
    /// <returns>带有成功标识的byte[]数组</returns>
    Task<OperateResult<bool[]>> ReadBoolAsync(string address, ushort length);

    /// <summary>
    /// 异步读取单个的<see cref="T:System.Boolean" />数据信息<br />
    /// Asynchronously read a single <see cref="T:System.Boolean" /> data message
    /// </summary>
    /// <param name="address">数据地址</param>
    /// <returns>带有成功标识的byte[]数组</returns>
    Task<OperateResult<bool>> ReadBoolAsync(string address);

    /// <summary>
    /// 异步批量写入<see cref="T:System.Boolean" />数组数据，返回是否成功<br />
    /// Asynchronously batch write <see cref="T:System.Boolean" /> array data, return success
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="value">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    Task<OperateResult> WriteAsync(string address, bool[] value);

    /// <summary>
    /// 异步批量写入<see cref="T:System.Boolean" />数组数据，返回是否成功<br />
    /// Asynchronously batch write <see cref="T:System.Boolean" /> array data, return success
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="value">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    Task<OperateResult> WriteAsync(string address, bool value);

    /// <summary>
    /// 异步读取16位的有符号的整型数据<br />
    /// Asynchronously read 16-bit signed integer data
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <returns>带有成功标识的short数据</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadInt16Async" title="Int16类型示例" />
    /// </example>
    Task<OperateResult<short>> ReadInt16Async(string address);

    /// <summary>
    /// 异步读取16位的有符号整型数组<br />
    /// Asynchronously read 16-bit signed integer array
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="length">读取的数组长度</param>
    /// <returns>带有成功标识的short数组</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadInt16ArrayAsync" title="Int16类型示例" />
    /// </example>
    Task<OperateResult<short[]>> ReadInt16Async(string address, ushort length);

    /// <summary>
    /// 异步读取16位的无符号整型<br />
    /// Asynchronously read 16-bit unsigned integer
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <returns>带有成功标识的ushort数据</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadUInt16Async" title="UInt16类型示例" />
    /// </example>
    Task<OperateResult<ushort>> ReadUInt16Async(string address);

    /// <summary>
    /// 异步读取16位的无符号整型数组<br />
    /// Asynchronously read 16-bit unsigned integer array
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="length">读取的数组长度</param>
    /// <returns>带有成功标识的ushort数组</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadUInt16ArrayAsync" title="UInt16类型示例" />
    /// </example>
    Task<OperateResult<ushort[]>> ReadUInt16Async(string address, ushort length);

    /// <summary>
    /// 异步读取32位的有符号整型<br />
    /// Asynchronously read 32-bit signed integer
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <returns>带有成功标识的int数据</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadInt32Async" title="Int32类型示例" />
    /// </example>
    Task<OperateResult<int>> ReadInt32Async(string address);

    /// <summary>
    /// 异步读取32位有符号整型数组<br />
    /// Asynchronously read 32-bit signed integer array
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="length">数组长度</param>
    /// <returns>带有成功标识的int数组</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadInt32ArrayAsync" title="Int32类型示例" />
    /// </example>
    Task<OperateResult<int[]>> ReadInt32Async(string address, ushort length);

    /// <summary>
    /// 异步读取32位的无符号整型<br />
    /// Asynchronously read 32-bit unsigned integer
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <returns>带有成功标识的uint数据</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadUInt32Async" title="UInt32类型示例" />
    /// </example>
    Task<OperateResult<uint>> ReadUInt32Async(string address);

    /// <summary>
    /// 异步读取32位的无符号整型数组<br />
    /// Asynchronously read 32-bit unsigned integer array
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="length">数组长度</param>
    /// <returns>带有成功标识的uint数组</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadUInt32ArrayAsync" title="UInt32类型示例" />
    /// </example>
    Task<OperateResult<uint[]>> ReadUInt32Async(string address, ushort length);

    /// <summary>
    /// 异步读取64位的有符号整型<br />
    /// Asynchronously read 64-bit signed integer
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <returns>带有成功标识的long数据</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadInt64Async" title="Int64类型示例" />
    /// </example>
    Task<OperateResult<long>> ReadInt64Async(string address);

    /// <summary>
    /// 异步读取64位的有符号整型数组<br />
    /// Asynchronously read 64-bit signed integer array
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="length">数组长度</param>
    /// <returns>带有成功标识的long数组</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadInt64ArrayAsync" title="Int64类型示例" />
    /// </example>
    Task<OperateResult<long[]>> ReadInt64Async(string address, ushort length);

    /// <summary>
    /// 异步读取64位的无符号整型<br />
    /// Asynchronously read 64-bit unsigned integer
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <returns>带有成功标识的ulong数据</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadUInt64Async" title="UInt64类型示例" />
    /// </example>
    Task<OperateResult<ulong>> ReadUInt64Async(string address);

    /// <summary>
    /// 异步读取64位的无符号整型的数组<br />
    /// Asynchronously read 64-bit unsigned integer array
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="length">数组长度</param>
    /// <returns>带有成功标识的ulong数组</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadUInt64ArrayAsync" title="UInt64类型示例" />
    /// </example>
    Task<OperateResult<ulong[]>> ReadUInt64Async(string address, ushort length);

    /// <summary>
    /// 异步读取单浮点数据<br />
    /// Asynchronously read single floating point data
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <returns>带有成功标识的float数据</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadFloatAsync" title="Float类型示例" />
    /// </example>
    Task<OperateResult<float>> ReadFloatAsync(string address);

    /// <summary>
    /// 异步读取单浮点精度的数组<br />
    /// Asynchronously read single floating point array
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="length">数组长度</param>
    /// <returns>带有成功标识的float数组</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadFloatArrayAsync" title="Float类型示例" />
    /// </example>
    Task<OperateResult<float[]>> ReadFloatAsync(string address, ushort length);

    /// <summary>
    /// 异步读取双浮点的数据<br />
    /// Asynchronously read double floating point data
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <returns>带有成功标识的double数据</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadDoubleAsync" title="Double类型示例" />
    /// </example>
    Task<OperateResult<double>> ReadDoubleAsync(string address);

    /// <summary>
    /// 异步读取双浮点数据的数组<br />
    /// Asynchronously read double floating point data array
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="length">数组长度</param>
    /// <returns>带有成功标识的double数组</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadDoubleArrayAsync" title="Double类型示例" />
    /// </example>
    Task<OperateResult<double[]>> ReadDoubleAsync(string address, ushort length);

    /// <summary>
    /// 异步读取字符串数据，默认为最常见的ASCII编码<br />
    /// Asynchronously read string data, default is the most common ASCII encoding
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="length">数据长度</param>
    /// <returns>带有成功标识的string数据</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadStringAsync" title="String类型示例" />
    /// </example>
    Task<OperateResult<string>> ReadStringAsync(string address, ushort length);

    /// <summary>
    /// 异步使用指定的编码，读取字符串数据<br />
    /// Asynchronously reads string data using the specified encoding
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="length">数据长度</param>
    /// <param name="encoding">指定的自定义的编码</param>
    /// <returns>带有成功标识的string数据</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadStringEncoding" title="String类型示例" />
    /// </example>
    Task<OperateResult<string>> ReadStringAsync(
      string address,
      ushort length,
      Encoding encoding);

    /// <summary>
    /// 异步写入short数据，返回是否成功<br />
    /// Asynchronously write short data, returns whether success
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="value">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteInt16Async" title="Int16类型示例" />
    /// </example>
    Task<OperateResult> WriteAsync(string address, short value);

    /// <summary>
    /// 异步写入short数组，返回是否成功<br />
    /// Asynchronously write short array, return whether the write was successful
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="values">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteInt16ArrayAsync" title="Int16类型示例" />
    /// </example>
    Task<OperateResult> WriteAsync(string address, short[] values);

    /// <summary>
    /// 异步写入ushort数据，返回是否成功<br />
    /// Asynchronously write ushort data, return whether the write was successful
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="value">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteUInt16Async" title="UInt16类型示例" />
    /// </example>
    Task<OperateResult> WriteAsync(string address, ushort value);

    /// <summary>
    /// 异步写入ushort数组，返回是否成功<br />
    /// Asynchronously write ushort array, return whether the write was successful
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="values">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteUInt16ArrayAsync" title="UInt16类型示例" />
    /// </example>
    Task<OperateResult> WriteAsync(string address, ushort[] values);

    /// <summary>
    /// 异步写入int数据，返回是否成功<br />
    /// Asynchronously write int data, return whether the write was successful
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="value">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteInt32Async" title="Int32类型示例" />
    /// </example>
    Task<OperateResult> WriteAsync(string address, int value);

    /// <summary>
    /// 异步写入int[]数组，返回是否成功<br />
    /// Asynchronously write int array, return whether the write was successful
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="values">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteInt32ArrayAsync" title="Int32类型示例" />
    /// </example>
    Task<OperateResult> WriteAsync(string address, int[] values);

    /// <summary>
    /// 异步写入uint数据，返回是否成功<br />
    /// Asynchronously write uint data, return whether the write was successful
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="value">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteUInt32Async" title="UInt32类型示例" />
    /// </example>
    Task<OperateResult> WriteAsync(string address, uint value);

    /// <summary>
    /// 异步写入uint[]数组，返回是否成功<br />
    /// Asynchronously write uint array, return whether the write was successful
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="values">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteUInt32ArrayAsync" title="UInt32类型示例" />
    /// </example>
    Task<OperateResult> WriteAsync(string address, uint[] values);

    /// <summary>
    /// 异步写入long数据，返回是否成功<br />
    /// Asynchronously write long data, return whether the write was successful
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="value">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteInt64Async" title="Int64类型示例" />
    /// </example>
    Task<OperateResult> WriteAsync(string address, long value);

    /// <summary>
    /// 异步写入long数组，返回是否成功<br />
    /// Asynchronously write long array, return whether the write was successful
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="values">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteInt64ArrayAsync" title="Int64类型示例" />
    /// </example>
    Task<OperateResult> WriteAsync(string address, long[] values);

    /// <summary>
    /// 异步写入ulong数据，返回是否成功<br />
    /// Asynchronously write ulong data, return whether the write was successful
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="value">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteUInt64Async" title="UInt64类型示例" />
    /// </example>
    Task<OperateResult> WriteAsync(string address, ulong value);

    /// <summary>
    /// 异步写入ulong数组，返回是否成功<br />
    /// Asynchronously write ulong array, return whether the write was successful
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="values">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteUInt64ArrayAsync" title="UInt64类型示例" />
    /// </example>
    Task<OperateResult> WriteAsync(string address, ulong[] values);

    /// <summary>
    /// 异步写入float数据，返回是否成功<br />
    /// Asynchronously write float data, return whether the write was successful
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="value">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteFloatAsync" title="Float类型示例" />
    /// </example>
    Task<OperateResult> WriteAsync(string address, float value);

    /// <summary>
    /// 异步写入float数组，返回是否成功<br />
    /// Asynchronously write float array, return whether the write was successful
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="values">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteFloatArrayAsync" title="Float类型示例" />
    /// </example>
    Task<OperateResult> WriteAsync(string address, float[] values);

    /// <summary>
    /// 异步写入double数据，返回是否成功<br />
    /// Asynchronously write double data, return whether the write was successful
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="value">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteDoubleAsync" title="Double类型示例" />
    /// </example>
    Task<OperateResult> WriteAsync(string address, double value);

    /// <summary>
    /// 异步写入double数组，返回是否成功<br />
    /// Asynchronously write double array, return whether the write was successful
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="values">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteDoubleArrayAsync" title="Double类型示例" />
    /// </example>
    Task<OperateResult> WriteAsync(string address, double[] values);

    /// <summary>
    /// 异步写入字符串信息，编码为ASCII<br />
    /// Asynchronously write string information, encoded as ASCII
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="value">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteStringAsync" title="String类型示例" />
    /// </example>
    Task<OperateResult> WriteAsync(string address, string value);

    /// <summary>
    /// 异步写入字符串信息，需要指定的编码信息<br />
    /// Asynchronously write string information, need to specify the encoding information
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="value">写入值</param>
    /// <param name="encoding">指定的编码信息</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <example>
    /// 以下为三菱的连接对象示例，其他的设备读写情况参照下面的代码：
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteStringAsync" title="String类型示例" />
    /// </example>
    Task<OperateResult> WriteAsync(
      string address,
      string value,
      Encoding encoding);

    /// <summary>
    /// 异步写入指定长度的字符串信息，如果超出，就截断字符串，如果长度不足，那就补0操作，编码为ASCII<br />
    /// Asynchronously write string information of the specified length. If it exceeds the value, the string is truncated.
    /// If the length is not enough, it is filled with 0 and the encoding is ASCII.
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="value">写入值</param>
    /// <param name="length">字符串的长度</param>
    /// <returns>带有成功标识的结果类对象</returns>
    Task<OperateResult> WriteAsync(string address, string value, int length);

    /// <summary>
    /// 异步写入指定长度的字符串信息，如果超出，就截断字符串，如果长度不足，那就补0操作，编码为指定的编码信息<br />
    /// Asynchronously write string information of the specified length. If it exceeds the value, the string is truncated. If the length is not enough,
    /// then the operation is complemented with 0 , you should specified the encoding information
    /// </summary>
    /// <param name="address">起始地址</param>
    /// <param name="value">写入值</param>
    /// <param name="length">字符串的长度</param>
    /// <param name="encoding">指定的编码信息</param>
    /// <returns>带有成功标识的结果类对象</returns>
    Task<OperateResult> WriteAsync(
      string address,
      string value,
      int length,
      Encoding encoding);

    /// <summary>
    /// 异步读取自定义的数据类型，需要继承自IDataTransfer接口<br />
    /// Asynchronously read custom data types, need to inherit from IDataTransfer interface
    /// </summary>
    /// <typeparam name="T">自定义的类型</typeparam>
    /// <param name="address">起始地址</param>
    /// <returns>带有成功标识的自定义类型数据</returns>
    /// <remarks>需要是定义一个类，选择好相对于的ByteTransform实例，才能调用该方法。</remarks>
    /// <example>
    /// 此处演示三菱的读取示例，先定义一个类，实现<see cref="T:EstCommunication.IDataTransfer" />接口
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="IDataTransfer Example" title="DataMy示例" />
    /// 接下来就可以实现数据的读取了
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadCustomerAsyncExample" title="ReadCustomerAsync示例" />
    /// </example>
    Task<OperateResult<T>> ReadCustomerAsync<T>(string address) where T : IDataTransfer, new();

    /// <summary>
    /// 异步写入自定义类型的数据，该类型必须继承自IDataTransfer接口<br />
    /// Asynchronously write data of a custom type, which must inherit from the IDataTransfer interface
    /// </summary>
    /// <typeparam name="T">类型对象</typeparam>
    /// <param name="address">起始地址</param>
    /// <param name="value">写入值</param>
    /// <returns>带有成功标识的结果类对象</returns>
    /// <remarks>
    /// 需要是定义一个类，选择好相对于的<see cref="T:EstCommunication.IDataTransfer" />实例，才能调用该方法。
    /// </remarks>
    /// <example>
    /// 此处演示三菱的读取示例，先定义一个类，实现<see cref="T:EstCommunication.IDataTransfer" />接口
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="IDataTransfer Example" title="DataMy示例" />
    /// 接下来就可以实现数据的读取了
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteCustomerAsyncExample" title="WriteCustomerAsync示例" />
    /// </example>
    Task<OperateResult> WriteCustomerAsync<T>(string address, T value) where T : IDataTransfer, new();

    /// <summary>
    /// 异步读取支持Est特性的数据内容，该特性为<see cref="T:EstCommunication.Reflection.EstDeviceAddressAttribute" />，详细参考api文档说明<br />
    /// Asynchronously read the data content of the Est attribute. The attribute is <see cref="T:EstCommunication.Reflection.EstDeviceAddressAttribute" />, please refer to the api documentation for details.
    /// </summary>
    /// <typeparam name="T">自定义的数据类型对象</typeparam>
    /// <returns>包含是否成功的结果对象</returns>
    /// <example>
    /// 此处演示西门子的读取示例，先定义一个类，重点是将需要读取的数据，写入到属性的特性中去。
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ObjectDefineExample" title="特性实现示例" />
    /// 接下来就可以实现数据的读取了
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ReadObjectAsyncExample" title="ReadObjectAsync示例" />
    /// </example>
    Task<OperateResult<T>> ReadAsync<T>() where T : class, new();

    /// <summary>
    /// 异步写入支持Est特性的数据，返回是否写入成功，该特性为<see cref="T:EstCommunication.Reflection.EstDeviceAddressAttribute" />，详细参考api文档说明<br />
    /// Asynchronously write data that supports the Est attribute, and return whether the write was successful. The attribute is <see cref="T:EstCommunication.Reflection.EstDeviceAddressAttribute" />, please refer to the api documentation for details.
    /// </summary>
    /// <typeparam name="T">自定义的数据类型对象</typeparam>
    /// <returns>包含是否成功的结果对象</returns>
    /// <example>
    /// 此处演示西门子的读取示例，先定义一个类，重点是将需要读取的数据，写入到属性的特性中去。
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="ObjectDefineExample" title="特性实现示例" />
    /// 接下来就可以实现数据的写入了
    /// <code lang="cs" source="EstCommunication_Net45.Test\Documentation\Samples\Core\NetworkDeviceBase.cs" region="WriteObjectAsyncExample" title="WriteObjectAsync示例" />
    /// </example>
    /// <exception cref="T:System.ArgumentNullException"></exception>
    Task<OperateResult> WriteAsync<T>(T data) where T : class, new();
  }
}
