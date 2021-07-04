// Decompiled with JetBrains decompiler
// Type: EstCommunication.Core.Net.NetworkDeviceBase
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using EstCommunication.Reflection;
using System;
using System.Text;
using System.Threading.Tasks;

namespace EstCommunication.Core.Net
{
  /// <summary>
  /// 设备交互类的基类，实现了<see cref="T:EstCommunication.Core.IReadWriteNet" />接口的基础方法方法，需要使用继承重写来实现字节读写，bool读写操作。<br />
  /// The base class of the device interaction class, which implements the basic methods of the <see cref="T:EstCommunication.Core.IReadWriteNet" /> interface,
  /// requires inheritance rewriting to implement byte read and write, and bool read and write operations.
  /// </summary>
  /// <remarks>需要继承实现采用使用。</remarks>
  public class NetworkDeviceBase : NetworkDoubleBase, IReadWriteNet
  {
    /// <summary>
    /// 一个字单位的数据表示的地址长度，西门子为2，三菱，欧姆龙，modbusTcp就为1，AB PLC无效<br />
    /// The address length represented by one word of data, Siemens is 2, Mitsubishi, Omron, modbusTcp is 1, AB PLC is invalid
    /// </summary>
    /// <remarks>对设备来说，一个地址的数据对应的字节数，或是1个字节或是2个字节，通常是这两个选择</remarks>
    protected ushort WordLength { get; set; } = 1;

    /// <inheritdoc />
    public override string ToString() => string.Format("NetworkDeviceBase<{0}, {1}>[{2}:{3}]", (object) this.GetNewNetMessage().GetType(), (object) this.ByteTransform.GetType(), (object) this.IpAddress, (object) this.Port);

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Read(System.String,System.UInt16)" />
    [EstMqttApi("ReadByteArray", "")]
    public virtual OperateResult<byte[]> Read(string address, ushort length) => new OperateResult<byte[]>(StringResources.Language.NotSupportedFunction);

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.Byte[])" />
    [EstMqttApi("WriteByteArray", "")]
    public virtual OperateResult Write(string address, byte[] value) => new OperateResult(StringResources.Language.NotSupportedFunction);

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadBool(System.String,System.UInt16)" />
    [EstMqttApi("ReadBoolArray", "")]
    public virtual OperateResult<bool[]> ReadBool(string address, ushort length) => new OperateResult<bool[]>(StringResources.Language.NotSupportedFunction);

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadBool(System.String)" />
    [EstMqttApi("ReadBool", "")]
    public virtual OperateResult<bool> ReadBool(string address) => ByteTransformHelper.GetResultFromArray<bool>(this.ReadBool(address, (ushort) 1));

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.Boolean[])" />
    [EstMqttApi("WriteBoolArray", "")]
    public virtual OperateResult Write(string address, bool[] value) => new OperateResult(StringResources.Language.NotSupportedFunction);

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.Boolean)" />
    [EstMqttApi("WriteBool", "")]
    public virtual OperateResult Write(string address, bool value) => this.Write(address, new bool[1]
    {
      value
    });

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadCustomer``1(System.String)" />
    public OperateResult<T> ReadCustomer<T>(string address) where T : IDataTransfer, new()
    {
      OperateResult<T> operateResult1 = new OperateResult<T>();
      T obj = new T();
      OperateResult<byte[]> operateResult2 = this.Read(address, obj.ReadCount);
      if (operateResult2.IsSuccess)
      {
        obj.ParseSource(operateResult2.Content);
        operateResult1.Content = obj;
        operateResult1.IsSuccess = true;
      }
      else
      {
        operateResult1.ErrorCode = operateResult2.ErrorCode;
        operateResult1.Message = operateResult2.Message;
      }
      return operateResult1;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteCustomer``1(System.String,``0)" />
    public OperateResult WriteCustomer<T>(string address, T data) where T : IDataTransfer, new() => this.Write(address, data.ToSource());

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Read``1" />
    public virtual OperateResult<T> Read<T>() where T : class, new() => EstReflectionHelper.Read<T>((IReadWriteNet) this);

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write``1(``0)" />
    public virtual OperateResult Write<T>(T data) where T : class, new() => EstReflectionHelper.Write<T>(data, (IReadWriteNet) this);

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadInt16(System.String)" />
    [EstMqttApi("ReadInt16", "")]
    public OperateResult<short> ReadInt16(string address) => ByteTransformHelper.GetResultFromArray<short>(this.ReadInt16(address, (ushort) 1));

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadInt16(System.String,System.UInt16)" />
    [EstMqttApi("ReadInt16Array", "")]
    public virtual OperateResult<short[]> ReadInt16(string address, ushort length) => ByteTransformHelper.GetResultFromBytes<short[]>(this.Read(address, (ushort) ((uint) length * (uint) this.WordLength)), (Func<byte[], short[]>) (m => this.ByteTransform.TransInt16(m, 0, (int) length)));

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadUInt16(System.String)" />
    [EstMqttApi("ReadUInt16", "")]
    public OperateResult<ushort> ReadUInt16(string address) => ByteTransformHelper.GetResultFromArray<ushort>(this.ReadUInt16(address, (ushort) 1));

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadUInt16(System.String,System.UInt16)" />
    [EstMqttApi("ReadUInt16Array", "")]
    public virtual OperateResult<ushort[]> ReadUInt16(string address, ushort length) => ByteTransformHelper.GetResultFromBytes<ushort[]>(this.Read(address, (ushort) ((uint) length * (uint) this.WordLength)), (Func<byte[], ushort[]>) (m => this.ByteTransform.TransUInt16(m, 0, (int) length)));

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadInt32(System.String)" />
    [EstMqttApi("ReadInt32", "")]
    public OperateResult<int> ReadInt32(string address) => ByteTransformHelper.GetResultFromArray<int>(this.ReadInt32(address, (ushort) 1));

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadInt32(System.String,System.UInt16)" />
    [EstMqttApi("ReadInt32Array", "")]
    public virtual OperateResult<int[]> ReadInt32(string address, ushort length) => ByteTransformHelper.GetResultFromBytes<int[]>(this.Read(address, (ushort) ((int) length * (int) this.WordLength * 2)), (Func<byte[], int[]>) (m => this.ByteTransform.TransInt32(m, 0, (int) length)));

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadUInt32(System.String)" />
    [EstMqttApi("ReadUInt32", "")]
    public OperateResult<uint> ReadUInt32(string address) => ByteTransformHelper.GetResultFromArray<uint>(this.ReadUInt32(address, (ushort) 1));

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadUInt32(System.String,System.UInt16)" />
    [EstMqttApi("ReadUInt32Array", "")]
    public virtual OperateResult<uint[]> ReadUInt32(string address, ushort length) => ByteTransformHelper.GetResultFromBytes<uint[]>(this.Read(address, (ushort) ((int) length * (int) this.WordLength * 2)), (Func<byte[], uint[]>) (m => this.ByteTransform.TransUInt32(m, 0, (int) length)));

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadFloat(System.String)" />
    [EstMqttApi("ReadFloat", "")]
    public OperateResult<float> ReadFloat(string address) => ByteTransformHelper.GetResultFromArray<float>(this.ReadFloat(address, (ushort) 1));

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadFloat(System.String,System.UInt16)" />
    [EstMqttApi("ReadFloatArray", "")]
    public virtual OperateResult<float[]> ReadFloat(string address, ushort length) => ByteTransformHelper.GetResultFromBytes<float[]>(this.Read(address, (ushort) ((int) length * (int) this.WordLength * 2)), (Func<byte[], float[]>) (m => this.ByteTransform.TransSingle(m, 0, (int) length)));

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadInt64(System.String)" />
    [EstMqttApi("ReadInt64", "")]
    public OperateResult<long> ReadInt64(string address) => ByteTransformHelper.GetResultFromArray<long>(this.ReadInt64(address, (ushort) 1));

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadInt64(System.String,System.UInt16)" />
    [EstMqttApi("ReadInt64Array", "")]
    public virtual OperateResult<long[]> ReadInt64(string address, ushort length) => ByteTransformHelper.GetResultFromBytes<long[]>(this.Read(address, (ushort) ((int) length * (int) this.WordLength * 4)), (Func<byte[], long[]>) (m => this.ByteTransform.TransInt64(m, 0, (int) length)));

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadUInt64(System.String)" />
    [EstMqttApi("ReadUInt64", "")]
    public OperateResult<ulong> ReadUInt64(string address) => ByteTransformHelper.GetResultFromArray<ulong>(this.ReadUInt64(address, (ushort) 1));

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadUInt64(System.String,System.UInt16)" />
    [EstMqttApi("ReadUInt64Array", "")]
    public virtual OperateResult<ulong[]> ReadUInt64(string address, ushort length) => ByteTransformHelper.GetResultFromBytes<ulong[]>(this.Read(address, (ushort) ((int) length * (int) this.WordLength * 4)), (Func<byte[], ulong[]>) (m => this.ByteTransform.TransUInt64(m, 0, (int) length)));

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadDouble(System.String)" />
    [EstMqttApi("ReadDouble", "")]
    public OperateResult<double> ReadDouble(string address) => ByteTransformHelper.GetResultFromArray<double>(this.ReadDouble(address, (ushort) 1));

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadDouble(System.String,System.UInt16)" />
    [EstMqttApi("ReadDoubleArray", "")]
    public virtual OperateResult<double[]> ReadDouble(string address, ushort length) => ByteTransformHelper.GetResultFromBytes<double[]>(this.Read(address, (ushort) ((int) length * (int) this.WordLength * 4)), (Func<byte[], double[]>) (m => this.ByteTransform.TransDouble(m, 0, (int) length)));

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadString(System.String,System.UInt16)" />
    [EstMqttApi("ReadString", "")]
    public virtual OperateResult<string> ReadString(string address, ushort length) => this.ReadString(address, length, Encoding.ASCII);

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadString(System.String,System.UInt16,System.Text.Encoding)" />
    public virtual OperateResult<string> ReadString(
      string address,
      ushort length,
      Encoding encoding)
    {
      return ByteTransformHelper.GetResultFromBytes<string>(this.Read(address, length), (Func<byte[], string>) (m => this.ByteTransform.TransString(m, 0, m.Length, encoding)));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.Int16[])" />
    [EstMqttApi("WriteInt16Array", "")]
    public virtual OperateResult Write(string address, short[] values) => this.Write(address, this.ByteTransform.TransByte(values));

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.Int16)" />
    [EstMqttApi("WriteInt16", "")]
    public virtual OperateResult Write(string address, short value) => this.Write(address, new short[1]
    {
      value
    });

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.UInt16[])" />
    [EstMqttApi("WriteUInt16Array", "")]
    public virtual OperateResult Write(string address, ushort[] values) => this.Write(address, this.ByteTransform.TransByte(values));

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.UInt16)" />
    [EstMqttApi("WriteUInt16", "")]
    public virtual OperateResult Write(string address, ushort value) => this.Write(address, new ushort[1]
    {
      value
    });

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.Int32[])" />
    [EstMqttApi("WriteInt32Array", "")]
    public virtual OperateResult Write(string address, int[] values) => this.Write(address, this.ByteTransform.TransByte(values));

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.Int32)" />
    [EstMqttApi("WriteInt32", "")]
    public OperateResult Write(string address, int value) => this.Write(address, new int[1]
    {
      value
    });

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.UInt32[])" />
    [EstMqttApi("WriteUInt32Array", "")]
    public virtual OperateResult Write(string address, uint[] values) => this.Write(address, this.ByteTransform.TransByte(values));

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.UInt32)" />
    [EstMqttApi("WriteUInt32", "")]
    public OperateResult Write(string address, uint value) => this.Write(address, new uint[1]
    {
      value
    });

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.Single[])" />
    [EstMqttApi("WriteFloatArray", "")]
    public virtual OperateResult Write(string address, float[] values) => this.Write(address, this.ByteTransform.TransByte(values));

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.Single)" />
    [EstMqttApi("WriteFloat", "")]
    public OperateResult Write(string address, float value) => this.Write(address, new float[1]
    {
      value
    });

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.Int64[])" />
    [EstMqttApi("WriteInt64Array", "")]
    public virtual OperateResult Write(string address, long[] values) => this.Write(address, this.ByteTransform.TransByte(values));

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.Int64)" />
    [EstMqttApi("WriteInt64", "")]
    public OperateResult Write(string address, long value) => this.Write(address, new long[1]
    {
      value
    });

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.UInt64[])" />
    [EstMqttApi("WriteUInt64Array", "")]
    public virtual OperateResult Write(string address, ulong[] values) => this.Write(address, this.ByteTransform.TransByte(values));

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.UInt64)" />
    [EstMqttApi("WriteUInt64", "")]
    public OperateResult Write(string address, ulong value) => this.Write(address, new ulong[1]
    {
      value
    });

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.Double[])" />
    [EstMqttApi("WriteDoubleArray", "")]
    public virtual OperateResult Write(string address, double[] values) => this.Write(address, this.ByteTransform.TransByte(values));

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.Double)" />
    [EstMqttApi("WriteDouble", "")]
    public OperateResult Write(string address, double value) => this.Write(address, new double[1]
    {
      value
    });

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.String)" />
    [EstMqttApi("WriteString", "")]
    public virtual OperateResult Write(string address, string value) => this.Write(address, value, Encoding.ASCII);

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.String,System.Int32)" />
    public virtual OperateResult Write(string address, string value, int length) => this.Write(address, value, length, Encoding.ASCII);

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.String,System.Text.Encoding)" />
    public virtual OperateResult Write(
      string address,
      string value,
      Encoding encoding)
    {
      byte[] data = this.ByteTransform.TransByte(value, encoding);
      if (this.WordLength == (ushort) 1)
        data = SoftBasic.ArrayExpandToLengthEven<byte>(data);
      return this.Write(address, data);
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.String,System.Int32,System.Text.Encoding)" />
    public virtual OperateResult Write(
      string address,
      string value,
      int length,
      Encoding encoding)
    {
      byte[] data = this.ByteTransform.TransByte(value, encoding);
      if (this.WordLength == (ushort) 1)
        data = SoftBasic.ArrayExpandToLengthEven<byte>(data);
      byte[] length1 = SoftBasic.ArrayExpandToLength<byte>(data, length);
      return this.Write(address, length1);
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Wait(System.String,System.Boolean,System.Int32,System.Int32)" />
    [EstMqttApi("WaitBool", "")]
    public OperateResult<TimeSpan> Wait(
      string address,
      bool waitValue,
      int readInterval = 100,
      int waitTimeout = -1)
    {
      return ReadWriteNetHelper.Wait((IReadWriteNet) this, address, waitValue, readInterval, waitTimeout);
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Wait(System.String,System.Int16,System.Int32,System.Int32)" />
    [EstMqttApi("WaitInt16", "")]
    public OperateResult<TimeSpan> Wait(
      string address,
      short waitValue,
      int readInterval = 100,
      int waitTimeout = -1)
    {
      return ReadWriteNetHelper.Wait((IReadWriteNet) this, address, waitValue, readInterval, waitTimeout);
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Wait(System.String,System.UInt16,System.Int32,System.Int32)" />
    [EstMqttApi("WaitUInt16", "")]
    public OperateResult<TimeSpan> Wait(
      string address,
      ushort waitValue,
      int readInterval = 100,
      int waitTimeout = -1)
    {
      return ReadWriteNetHelper.Wait((IReadWriteNet) this, address, waitValue, readInterval, waitTimeout);
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Wait(System.String,System.Int32,System.Int32,System.Int32)" />
    [EstMqttApi("WaitInt32", "")]
    public OperateResult<TimeSpan> Wait(
      string address,
      int waitValue,
      int readInterval = 100,
      int waitTimeout = -1)
    {
      return ReadWriteNetHelper.Wait((IReadWriteNet) this, address, waitValue, readInterval, waitTimeout);
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Wait(System.String,System.UInt32,System.Int32,System.Int32)" />
    [EstMqttApi("WaitUInt32", "")]
    public OperateResult<TimeSpan> Wait(
      string address,
      uint waitValue,
      int readInterval = 100,
      int waitTimeout = -1)
    {
      return ReadWriteNetHelper.Wait((IReadWriteNet) this, address, waitValue, readInterval, waitTimeout);
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Wait(System.String,System.Int64,System.Int32,System.Int32)" />
    [EstMqttApi("WaitInt64", "")]
    public OperateResult<TimeSpan> Wait(
      string address,
      long waitValue,
      int readInterval = 100,
      int waitTimeout = -1)
    {
      return ReadWriteNetHelper.Wait((IReadWriteNet) this, address, waitValue, readInterval, waitTimeout);
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Wait(System.String,System.UInt64,System.Int32,System.Int32)" />
    [EstMqttApi("WaitUInt64", "")]
    public OperateResult<TimeSpan> Wait(
      string address,
      ulong waitValue,
      int readInterval = 100,
      int waitTimeout = -1)
    {
      return ReadWriteNetHelper.Wait((IReadWriteNet) this, address, waitValue, readInterval, waitTimeout);
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Wait(System.String,System.Boolean,System.Int32,System.Int32)" />
    public async Task<OperateResult<TimeSpan>> WaitAsync(
      string address,
      bool waitValue,
      int readInterval = 100,
      int waitTimeout = -1)
    {
      OperateResult<TimeSpan> operateResult = await ReadWriteNetHelper.WaitAsync((IReadWriteNet) this, address, waitValue, readInterval, waitTimeout);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Wait(System.String,System.Int16,System.Int32,System.Int32)" />
    public async Task<OperateResult<TimeSpan>> WaitAsync(
      string address,
      short waitValue,
      int readInterval = 100,
      int waitTimeout = -1)
    {
      OperateResult<TimeSpan> operateResult = await ReadWriteNetHelper.WaitAsync((IReadWriteNet) this, address, waitValue, readInterval, waitTimeout);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Wait(System.String,System.UInt16,System.Int32,System.Int32)" />
    public async Task<OperateResult<TimeSpan>> WaitAsync(
      string address,
      ushort waitValue,
      int readInterval = 100,
      int waitTimeout = -1)
    {
      OperateResult<TimeSpan> operateResult = await ReadWriteNetHelper.WaitAsync((IReadWriteNet) this, address, waitValue, readInterval, waitTimeout);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Wait(System.String,System.Int32,System.Int32,System.Int32)" />
    public async Task<OperateResult<TimeSpan>> WaitAsync(
      string address,
      int waitValue,
      int readInterval = 100,
      int waitTimeout = -1)
    {
      OperateResult<TimeSpan> operateResult = await ReadWriteNetHelper.WaitAsync((IReadWriteNet) this, address, waitValue, readInterval, waitTimeout);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Wait(System.String,System.UInt32,System.Int32,System.Int32)" />
    public async Task<OperateResult<TimeSpan>> WaitAsync(
      string address,
      uint waitValue,
      int readInterval = 100,
      int waitTimeout = -1)
    {
      OperateResult<TimeSpan> operateResult = await ReadWriteNetHelper.WaitAsync((IReadWriteNet) this, address, waitValue, readInterval, waitTimeout);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Wait(System.String,System.Int64,System.Int32,System.Int32)" />
    public async Task<OperateResult<TimeSpan>> WaitAsync(
      string address,
      long waitValue,
      int readInterval = 100,
      int waitTimeout = -1)
    {
      OperateResult<TimeSpan> operateResult = await ReadWriteNetHelper.WaitAsync((IReadWriteNet) this, address, waitValue, readInterval, waitTimeout);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Wait(System.String,System.UInt64,System.Int32,System.Int32)" />
    public async Task<OperateResult<TimeSpan>> WaitAsync(
      string address,
      ulong waitValue,
      int readInterval = 100,
      int waitTimeout = -1)
    {
      OperateResult<TimeSpan> operateResult = await ReadWriteNetHelper.WaitAsync((IReadWriteNet) this, address, waitValue, readInterval, waitTimeout);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadAsync(System.String,System.UInt16)" />
    public virtual async Task<OperateResult<byte[]>> ReadAsync(
      string address,
      ushort length)
    {
      OperateResult<byte[]> operateResult = await Task.Run<OperateResult<byte[]>>((Func<OperateResult<byte[]>>) (() => this.Read(address, length)));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteAsync(System.String,System.Byte[])" />
    public virtual async Task<OperateResult> WriteAsync(
      string address,
      byte[] value)
    {
      OperateResult operateResult = await Task.Run<OperateResult>((Func<OperateResult>) (() => this.Write(address, value)));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadBoolAsync(System.String,System.UInt16)" />
    public virtual async Task<OperateResult<bool[]>> ReadBoolAsync(
      string address,
      ushort length)
    {
      OperateResult<bool[]> operateResult = await Task.Run<OperateResult<bool[]>>((Func<OperateResult<bool[]>>) (() => this.ReadBool(address, length)));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadBoolAsync(System.String)" />
    public virtual async Task<OperateResult<bool>> ReadBoolAsync(string address)
    {
      OperateResult<bool[]> result = await this.ReadBoolAsync(address, (ushort) 1);
      return ByteTransformHelper.GetResultFromArray<bool>(result);
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteAsync(System.String,System.Boolean[])" />
    public virtual async Task<OperateResult> WriteAsync(
      string address,
      bool[] value)
    {
      OperateResult operateResult = await Task.Run<OperateResult>((Func<OperateResult>) (() => this.Write(address, value)));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteAsync(System.String,System.Boolean)" />
    public virtual async Task<OperateResult> WriteAsync(string address, bool value)
    {
      OperateResult operateResult = await this.WriteAsync(address, new bool[1]
      {
        value
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteCustomerAsync``1(System.String,``0)" />
    public async Task<OperateResult<T>> ReadCustomerAsync<T>(string address) where T : IDataTransfer, new()
    {
      OperateResult<T> result = new OperateResult<T>();
      T Content = new T();
      OperateResult<byte[]> read = await this.ReadAsync(address, Content.ReadCount);
      if (read.IsSuccess)
      {
        Content.ParseSource(read.Content);
        result.Content = Content;
        result.IsSuccess = true;
      }
      else
      {
        result.ErrorCode = read.ErrorCode;
        result.Message = read.Message;
      }
      OperateResult<T> operateResult = result;
      result = (OperateResult<T>) null;
      Content = default (T);
      read = (OperateResult<byte[]>) null;
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteCustomerAsync``1(System.String,``0)" />
    public async Task<OperateResult> WriteCustomerAsync<T>(string address, T data) where T : IDataTransfer, new()
    {
      OperateResult operateResult = await this.WriteAsync(address, data.ToSource());
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadAsync``1" />
    public virtual async Task<OperateResult<T>> ReadAsync<T>() where T : class, new()
    {
      OperateResult<T> operateResult = await EstReflectionHelper.ReadAsync<T>((IReadWriteNet) this);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteAsync``1(``0)" />
    public virtual async Task<OperateResult> WriteAsync<T>(T data) where T : class, new()
    {
      OperateResult operateResult = await EstReflectionHelper.WriteAsync<T>(data, (IReadWriteNet) this);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadInt16Async(System.String)" />
    public async Task<OperateResult<short>> ReadInt16Async(string address)
    {
      OperateResult<short[]> result = await this.ReadInt16Async(address, (ushort) 1);
      return ByteTransformHelper.GetResultFromArray<short>(result);
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadInt16Async(System.String,System.UInt16)" />
    public virtual async Task<OperateResult<short[]>> ReadInt16Async(
      string address,
      ushort length)
    {
      OperateResult<byte[]> result = await this.ReadAsync(address, (ushort) ((uint) length * (uint) this.WordLength));
      return ByteTransformHelper.GetResultFromBytes<short[]>(result, (Func<byte[], short[]>) (m => this.ByteTransform.TransInt16(m, 0, (int) length)));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadUInt16Async(System.String)" />
    public async Task<OperateResult<ushort>> ReadUInt16Async(string address)
    {
      OperateResult<ushort[]> result = await this.ReadUInt16Async(address, (ushort) 1);
      return ByteTransformHelper.GetResultFromArray<ushort>(result);
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadUInt16Async(System.String,System.UInt16)" />
    public virtual async Task<OperateResult<ushort[]>> ReadUInt16Async(
      string address,
      ushort length)
    {
      OperateResult<byte[]> result = await this.ReadAsync(address, (ushort) ((uint) length * (uint) this.WordLength));
      return ByteTransformHelper.GetResultFromBytes<ushort[]>(result, (Func<byte[], ushort[]>) (m => this.ByteTransform.TransUInt16(m, 0, (int) length)));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadInt32Async(System.String)" />
    public async Task<OperateResult<int>> ReadInt32Async(string address)
    {
      OperateResult<int[]> result = await this.ReadInt32Async(address, (ushort) 1);
      return ByteTransformHelper.GetResultFromArray<int>(result);
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadInt32Async(System.String,System.UInt16)" />
    public virtual async Task<OperateResult<int[]>> ReadInt32Async(
      string address,
      ushort length)
    {
      OperateResult<byte[]> result = await this.ReadAsync(address, (ushort) ((int) length * (int) this.WordLength * 2));
      return ByteTransformHelper.GetResultFromBytes<int[]>(result, (Func<byte[], int[]>) (m => this.ByteTransform.TransInt32(m, 0, (int) length)));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadUInt32Async(System.String)" />
    public async Task<OperateResult<uint>> ReadUInt32Async(string address)
    {
      OperateResult<uint[]> result = await this.ReadUInt32Async(address, (ushort) 1);
      return ByteTransformHelper.GetResultFromArray<uint>(result);
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadUInt32Async(System.String,System.UInt16)" />
    public virtual async Task<OperateResult<uint[]>> ReadUInt32Async(
      string address,
      ushort length)
    {
      OperateResult<byte[]> result = await this.ReadAsync(address, (ushort) ((int) length * (int) this.WordLength * 2));
      return ByteTransformHelper.GetResultFromBytes<uint[]>(result, (Func<byte[], uint[]>) (m => this.ByteTransform.TransUInt32(m, 0, (int) length)));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadFloatAsync(System.String)" />
    public async Task<OperateResult<float>> ReadFloatAsync(string address)
    {
      OperateResult<float[]> result = await this.ReadFloatAsync(address, (ushort) 1);
      return ByteTransformHelper.GetResultFromArray<float>(result);
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadFloatAsync(System.String,System.UInt16)" />
    public virtual async Task<OperateResult<float[]>> ReadFloatAsync(
      string address,
      ushort length)
    {
      OperateResult<byte[]> result = await this.ReadAsync(address, (ushort) ((int) length * (int) this.WordLength * 2));
      return ByteTransformHelper.GetResultFromBytes<float[]>(result, (Func<byte[], float[]>) (m => this.ByteTransform.TransSingle(m, 0, (int) length)));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadInt64Async(System.String)" />
    public async Task<OperateResult<long>> ReadInt64Async(string address)
    {
      OperateResult<long[]> result = await this.ReadInt64Async(address, (ushort) 1);
      return ByteTransformHelper.GetResultFromArray<long>(result);
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadInt64Async(System.String,System.UInt16)" />
    public virtual async Task<OperateResult<long[]>> ReadInt64Async(
      string address,
      ushort length)
    {
      OperateResult<byte[]> result = await this.ReadAsync(address, (ushort) ((int) length * (int) this.WordLength * 4));
      return ByteTransformHelper.GetResultFromBytes<long[]>(result, (Func<byte[], long[]>) (m => this.ByteTransform.TransInt64(m, 0, (int) length)));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadUInt64Async(System.String)" />
    public async Task<OperateResult<ulong>> ReadUInt64Async(string address)
    {
      OperateResult<ulong[]> result = await this.ReadUInt64Async(address, (ushort) 1);
      return ByteTransformHelper.GetResultFromArray<ulong>(result);
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadUInt64Async(System.String,System.UInt16)" />
    public virtual async Task<OperateResult<ulong[]>> ReadUInt64Async(
      string address,
      ushort length)
    {
      OperateResult<byte[]> result = await this.ReadAsync(address, (ushort) ((int) length * (int) this.WordLength * 4));
      return ByteTransformHelper.GetResultFromBytes<ulong[]>(result, (Func<byte[], ulong[]>) (m => this.ByteTransform.TransUInt64(m, 0, (int) length)));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadDoubleAsync(System.String)" />
    public async Task<OperateResult<double>> ReadDoubleAsync(string address)
    {
      OperateResult<double[]> result = await this.ReadDoubleAsync(address, (ushort) 1);
      return ByteTransformHelper.GetResultFromArray<double>(result);
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadDoubleAsync(System.String,System.UInt16)" />
    public virtual async Task<OperateResult<double[]>> ReadDoubleAsync(
      string address,
      ushort length)
    {
      OperateResult<byte[]> result = await this.ReadAsync(address, (ushort) ((int) length * (int) this.WordLength * 4));
      return ByteTransformHelper.GetResultFromBytes<double[]>(result, (Func<byte[], double[]>) (m => this.ByteTransform.TransDouble(m, 0, (int) length)));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadStringAsync(System.String,System.UInt16)" />
    public virtual async Task<OperateResult<string>> ReadStringAsync(
      string address,
      ushort length)
    {
      OperateResult<string> operateResult = await this.ReadStringAsync(address, length, Encoding.ASCII);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadStringAsync(System.String,System.UInt16,System.Text.Encoding)" />
    public virtual async Task<OperateResult<string>> ReadStringAsync(
      string address,
      ushort length,
      Encoding encoding)
    {
      OperateResult<byte[]> result = await this.ReadAsync(address, length);
      return ByteTransformHelper.GetResultFromBytes<string>(result, (Func<byte[], string>) (m => this.ByteTransform.TransString(m, 0, m.Length, encoding)));
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteAsync(System.String,System.Int16[])" />
    public virtual async Task<OperateResult> WriteAsync(
      string address,
      short[] values)
    {
      OperateResult operateResult = await this.WriteAsync(address, this.ByteTransform.TransByte(values));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteAsync(System.String,System.Int16)" />
    public virtual async Task<OperateResult> WriteAsync(
      string address,
      short value)
    {
      OperateResult operateResult = await this.WriteAsync(address, new short[1]
      {
        value
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteAsync(System.String,System.UInt16[])" />
    public virtual async Task<OperateResult> WriteAsync(
      string address,
      ushort[] values)
    {
      OperateResult operateResult = await this.WriteAsync(address, this.ByteTransform.TransByte(values));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteAsync(System.String,System.UInt16)" />
    public virtual async Task<OperateResult> WriteAsync(
      string address,
      ushort value)
    {
      OperateResult operateResult = await this.WriteAsync(address, new ushort[1]
      {
        value
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteAsync(System.String,System.Int32[])" />
    public virtual async Task<OperateResult> WriteAsync(
      string address,
      int[] values)
    {
      OperateResult operateResult = await this.WriteAsync(address, this.ByteTransform.TransByte(values));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteAsync(System.String,System.Int32)" />
    public async Task<OperateResult> WriteAsync(string address, int value)
    {
      OperateResult operateResult = await this.WriteAsync(address, new int[1]
      {
        value
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteAsync(System.String,System.UInt32[])" />
    public virtual async Task<OperateResult> WriteAsync(
      string address,
      uint[] values)
    {
      OperateResult operateResult = await this.WriteAsync(address, this.ByteTransform.TransByte(values));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteAsync(System.String,System.UInt32)" />
    public async Task<OperateResult> WriteAsync(string address, uint value)
    {
      OperateResult operateResult = await this.WriteAsync(address, new uint[1]
      {
        value
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteAsync(System.String,System.Single[])" />
    public virtual async Task<OperateResult> WriteAsync(
      string address,
      float[] values)
    {
      OperateResult operateResult = await this.WriteAsync(address, this.ByteTransform.TransByte(values));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteAsync(System.String,System.Single)" />
    public async Task<OperateResult> WriteAsync(string address, float value)
    {
      OperateResult operateResult = await this.WriteAsync(address, new float[1]
      {
        value
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteAsync(System.String,System.Int64[])" />
    public virtual async Task<OperateResult> WriteAsync(
      string address,
      long[] values)
    {
      OperateResult operateResult = await this.WriteAsync(address, this.ByteTransform.TransByte(values));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteAsync(System.String,System.Int64)" />
    public async Task<OperateResult> WriteAsync(string address, long value)
    {
      OperateResult operateResult = await this.WriteAsync(address, new long[1]
      {
        value
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteAsync(System.String,System.UInt64[])" />
    public virtual async Task<OperateResult> WriteAsync(
      string address,
      ulong[] values)
    {
      OperateResult operateResult = await this.WriteAsync(address, this.ByteTransform.TransByte(values));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteAsync(System.String,System.UInt64)" />
    public async Task<OperateResult> WriteAsync(string address, ulong value)
    {
      OperateResult operateResult = await this.WriteAsync(address, new ulong[1]
      {
        value
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteAsync(System.String,System.Double[])" />
    public virtual async Task<OperateResult> WriteAsync(
      string address,
      double[] values)
    {
      OperateResult operateResult = await this.WriteAsync(address, this.ByteTransform.TransByte(values));
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteAsync(System.String,System.Double)" />
    public async Task<OperateResult> WriteAsync(string address, double value)
    {
      OperateResult operateResult = await this.WriteAsync(address, new double[1]
      {
        value
      });
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteAsync(System.String,System.String)" />
    public virtual async Task<OperateResult> WriteAsync(
      string address,
      string value)
    {
      OperateResult operateResult = await this.WriteAsync(address, value, Encoding.ASCII);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteAsync(System.String,System.String,System.Text.Encoding)" />
    public virtual async Task<OperateResult> WriteAsync(
      string address,
      string value,
      Encoding encoding)
    {
      byte[] temp = this.ByteTransform.TransByte(value, encoding);
      if (this.WordLength == (ushort) 1)
        temp = SoftBasic.ArrayExpandToLengthEven<byte>(temp);
      OperateResult operateResult = await this.WriteAsync(address, temp);
      temp = (byte[]) null;
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteAsync(System.String,System.String,System.Int32)" />
    public virtual async Task<OperateResult> WriteAsync(
      string address,
      string value,
      int length)
    {
      OperateResult operateResult = await this.WriteAsync(address, value, length, Encoding.ASCII);
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.WriteAsync(System.String,System.String,System.Int32,System.Text.Encoding)" />
    public virtual async Task<OperateResult> WriteAsync(
      string address,
      string value,
      int length,
      Encoding encoding)
    {
      byte[] temp = this.ByteTransform.TransByte(value, encoding);
      if (this.WordLength == (ushort) 1)
        temp = SoftBasic.ArrayExpandToLengthEven<byte>(temp);
      temp = SoftBasic.ArrayExpandToLength<byte>(temp, length);
      OperateResult operateResult = await this.WriteAsync(address, temp);
      temp = (byte[]) null;
      return operateResult;
    }
  }
}
