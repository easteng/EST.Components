// Decompiled with JetBrains decompiler
// Type: EstCommunication.EstExtension
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace EstCommunication
{
  /// <summary>扩展的辅助类方法</summary>
  public static class EstExtension
  {
    /// <inheritdoc cref="M:EstCommunication.BasicFramework.SoftBasic.ByteToHexString(System.Byte[])" />
    public static string ToHexString(this byte[] InBytes) => SoftBasic.ByteToHexString(InBytes);

    /// <inheritdoc cref="M:EstCommunication.BasicFramework.SoftBasic.ByteToHexString(System.Byte[],System.Char)" />
    public static string ToHexString(this byte[] InBytes, char segment) => SoftBasic.ByteToHexString(InBytes, segment);

    /// <inheritdoc cref="M:EstCommunication.BasicFramework.SoftBasic.ByteToHexString(System.Byte[],System.Char,System.Int32)" />
    public static string ToHexString(this byte[] InBytes, char segment, int newLineCount) => SoftBasic.ByteToHexString(InBytes, segment, newLineCount);

    /// <inheritdoc cref="M:EstCommunication.BasicFramework.SoftBasic.HexStringToBytes(System.String)" />
    public static byte[] ToHexBytes(this string value) => SoftBasic.HexStringToBytes(value);

    /// <inheritdoc cref="M:EstCommunication.BasicFramework.SoftBasic.BoolOnByteIndex(System.Byte,System.Int32)" />
    public static bool GetBoolOnIndex(this byte value, int offset) => SoftBasic.BoolOnByteIndex(value, offset);

    /// <inheritdoc cref="M:EstCommunication.BasicFramework.SoftBasic.BoolArrayToByte(System.Boolean[])" />
    public static byte[] ToByteArray(this bool[] array) => SoftBasic.BoolArrayToByte(array);

    /// <inheritdoc cref="M:EstCommunication.BasicFramework.SoftBasic.ByteToBoolArray(System.Byte[],System.Int32)" />
    public static bool[] ToBoolArray(this byte[] InBytes, int length) => SoftBasic.ByteToBoolArray(InBytes, length);

    /// <summary>
    /// 获取当前数组的倒序数组，这是一个新的实例，不改变原来的数组值<br />
    /// Get the reversed array of the current byte array, this is a new instance, does not change the original array value
    /// </summary>
    /// <param name="value">输入的原始数组</param>
    /// <returns>反转之后的数组信息</returns>
    public static T[] ReverseNew<T>(this T[] value)
    {
      T[] objArray = value.CopyArray<T>();
      Array.Reverse((Array) objArray);
      return objArray;
    }

    /// <inheritdoc cref="M:EstCommunication.BasicFramework.SoftBasic.ByteToBoolArray(System.Byte[])" />
    public static bool[] ToBoolArray(this byte[] InBytes) => SoftBasic.ByteToBoolArray(InBytes);

    /// <summary>
    /// 获取Byte数组的第 bytIndex 个位置的，boolIndex偏移的bool值<br />
    /// Get the bool value of the bytIndex position of the Byte array and the boolIndex offset
    /// </summary>
    /// <param name="bytes">字节数组信息</param>
    /// <param name="bytIndex">字节的偏移位置</param>
    /// <param name="boolIndex">指定字节的位偏移</param>
    /// <returns>bool值</returns>
    public static bool GetBoolValue(this byte[] bytes, int bytIndex, int boolIndex) => SoftBasic.BoolOnByteIndex(bytes[bytIndex], boolIndex);

    /// <summary>
    /// 获取Byte数组的第 boolIndex 偏移的bool值，这个偏移值可以为 10，就是第 1 个字节的 第3位 <br />
    /// Get the bool value of the boolIndex offset of the Byte array. The offset value can be 10, which is the third bit of the first byte
    /// </summary>
    /// <param name="bytes">字节数组信息</param>
    /// <param name="boolIndex">指定字节的位偏移</param>
    /// <returns>bool值</returns>
    public static bool GetBoolByIndex(this byte[] bytes, int boolIndex) => SoftBasic.BoolOnByteIndex(bytes[boolIndex / 8], boolIndex % 8);

    /// <summary>
    /// 获取Byte的第 boolIndex 偏移的bool值，比如3，就是第4位 <br />
    /// Get the bool value of Byte's boolIndex offset, such as 3, which is the 4th bit
    /// </summary>
    /// <param name="byt">字节信息</param>
    /// <param name="boolIndex">指定字节的位偏移</param>
    /// <returns>bool值</returns>
    public static bool GetBoolByIndex(this byte byt, int boolIndex) => SoftBasic.BoolOnByteIndex(byt, boolIndex % 8);

    /// <summary>
    /// 设置Byte的第 boolIndex 位的bool值，可以强制为 true 或是 false, 不影响其他的位<br />
    /// Set the bool value of the boolIndex bit of Byte, which can be forced to true or false, without affecting other bits
    /// </summary>
    /// <param name="byt">字节信息</param>
    /// <param name="boolIndex">指定字节的位偏移</param>
    /// <param name="value">bool的值</param>
    /// <returns>修改之后的byte值</returns>
    public static byte SetBoolByIndex(this byte byt, int boolIndex, bool value) => SoftBasic.SetBoolOnByteIndex(byt, boolIndex, value);

    /// <inheritdoc cref="M:EstCommunication.BasicFramework.SoftBasic.ArrayRemoveDouble``1(``0[],System.Int32,System.Int32)" />
    public static T[] RemoveDouble<T>(this T[] value, int leftLength, int rightLength) => SoftBasic.ArrayRemoveDouble<T>(value, leftLength, rightLength);

    /// <inheritdoc cref="M:EstCommunication.BasicFramework.SoftBasic.ArrayRemoveBegin``1(``0[],System.Int32)" />
    public static T[] RemoveBegin<T>(this T[] value, int length) => SoftBasic.ArrayRemoveBegin<T>(value, length);

    /// <inheritdoc cref="M:EstCommunication.BasicFramework.SoftBasic.ArrayRemoveLast``1(``0[],System.Int32)" />
    public static T[] RemoveLast<T>(this T[] value, int length) => SoftBasic.ArrayRemoveLast<T>(value, length);

    /// <inheritdoc cref="M:EstCommunication.BasicFramework.SoftBasic.ArraySelectMiddle``1(``0[],System.Int32,System.Int32)" />
    public static T[] SelectMiddle<T>(this T[] value, int index, int length) => SoftBasic.ArraySelectMiddle<T>(value, index, length);

    /// <inheritdoc cref="M:EstCommunication.BasicFramework.SoftBasic.ArraySelectBegin``1(``0[],System.Int32)" />
    public static T[] SelectBegin<T>(this T[] value, int length) => SoftBasic.ArraySelectBegin<T>(value, length);

    /// <inheritdoc cref="M:EstCommunication.BasicFramework.SoftBasic.ArraySelectLast``1(``0[],System.Int32)" />
    public static T[] SelectLast<T>(this T[] value, int length) => SoftBasic.ArraySelectLast<T>(value, length);

    /// <inheritdoc cref="M:EstCommunication.BasicFramework.SoftBasic.GetValueFromJsonObject``1(Newtonsoft.Json.Linq.JObject,System.String,``0)" />
    public static T GetValueOrDefault<T>(JObject jObject, string name, T defaultValue) => SoftBasic.GetValueFromJsonObject<T>(jObject, name, defaultValue);

    /// <inheritdoc cref="M:EstCommunication.BasicFramework.SoftBasic.SpliceArray``1(``0[][])" />
    public static T[] SpliceArray<T>(this T[] value, params T[][] arrays)
    {
      List<T[]> objArrayList = new List<T[]>(arrays.Length + 1);
      objArrayList.Add(value);
      objArrayList.AddRange((IEnumerable<T[]>) arrays);
      return SoftBasic.SpliceArray<T>(objArrayList.ToArray());
    }

    /// <summary>将指定的数据添加到数组的每个元素上去，使用表达式树的形式实现，将会修改原数组。不适用byte类型</summary>
    /// <typeparam name="T">数组的类型</typeparam>
    /// <param name="array">原始数据</param>
    /// <param name="value">数据值</param>
    /// <returns>返回的结果信息</returns>
    //public static T[] IncreaseBy<T>(this T[] array, T value)
    //{
    //  // ISSUE: unable to decompile the method.
    //}

    /// <summary>拷贝当前的实例数组，是基于引用层的浅拷贝，如果类型为值类型，那就是深度拷贝，如果类型为引用类型，就是浅拷贝</summary>
    /// <typeparam name="T">类型对象</typeparam>
    /// <param name="value">数组对象</param>
    /// <returns>拷贝的结果内容</returns>
    public static T[] CopyArray<T>(this T[] value)
    {
      if (value == null)
        return (T[]) null;
      T[] objArray = new T[value.Length];
      Array.Copy((Array) value, (Array) objArray, value.Length);
      return objArray;
    }

    /// <inheritdoc cref="M:EstCommunication.BasicFramework.SoftBasic.ArrayFormat``1(``0[])" />
    public static string ToArrayString<T>(this T[] value) => SoftBasic.ArrayFormat<T>(value);

    /// <inheritdoc cref="M:EstCommunication.BasicFramework.SoftBasic.ArrayFormat``1(``0,System.String)" />
    public static string ToArrayString<T>(this T[] value, string format) => SoftBasic.ArrayFormat<T>(value, format);

    /// <summary>
    /// 将字符串数组转换为实际的数据数组。例如字符串格式[1,2,3,4,5]，可以转成实际的数组对象<br />
    /// Converts a string array into an actual data array. For example, the string format [1,2,3,4,5] can be converted into an actual array object
    /// </summary>
    /// <typeparam name="T">类型对象</typeparam>
    /// <param name="value">字符串数据</param>
    /// <param name="selector">转换方法</param>
    /// <returns>实际的数组</returns>
    public static T[] ToStringArray<T>(this string value, Func<string, T> selector)
    {
      if (value.IndexOf('[') >= 0)
        value = value.Replace("[", "");
      if (value.IndexOf(']') >= 0)
        value = value.Replace("]", "");
      return ((IEnumerable<string>) value.Split(new char[2]
      {
        ',',
        ';'
      }, StringSplitOptions.RemoveEmptyEntries)).Select<string, T>(selector).ToArray<T>();
    }

    /// <summary>
    /// 将字符串数组转换为实际的数据数组。支持byte,sbyte,bool,short,ushort,int,uint,long,ulong,float,double，使用默认的十进制，例如字符串格式[1,2,3,4,5]，可以转成实际的数组对象<br />
    /// Converts a string array into an actual data array. Support byte, sbyte, bool, short, ushort, int, uint, long, ulong, float, double, use the default decimal,
    /// such as the string format [1,2,3,4,5], which can be converted into an actual array Object
    /// </summary>
    /// <typeparam name="T">类型对象</typeparam>
    /// <param name="value">字符串数据</param>
    /// <returns>实际的数组</returns>
    public static T[] ToStringArray<T>(this string value)
    {
      Type type = typeof (T);
      //if (type == typeof (byte))
      //  return (T[]) value.ToStringArray<byte>(a=>a.ToString());
      //if (type == typeof (sbyte))
      //  return (T[]) value.ToStringArray<sbyte>(new Func<string, sbyte>(sbpar));
      //if (type == typeof (bool))
      //  return (T[]) value.ToStringArray<bool>(new Func<string, bool>(bool.Parse));
      //if (type == typeof (short))
      //  return (T[]) value.ToStringArray<short>(new Func<string, short>(short.Parse));
      //if (type == typeof (ushort))
      //  return (T[]) value.ToStringArray<ushort>(new Func<string, ushort>(ushort.Parse));
      //if (type == typeof (int))
      //  return (T[]) value.ToStringArray<int>(new Func<string, int>(int.Parse));
      //if (type == typeof (uint))
      //  return (T[]) value.ToStringArray<uint>(new Func<string, uint>(uint.Parse));
      //if (type == typeof (long))
      //  return (T[]) value.ToStringArray<long>(new Func<string, long>(long.Parse));
      //if (type == typeof (ulong))
      //  return (T[]) value.ToStringArray<ulong>(new Func<string, ulong>(ulong.Parse));
      //if (type == typeof (float))
      //  return (T[]) value.ToStringArray<float>(new Func<string, float>(float.Parse));
      //if (type == typeof (double))
      //  return (T[]) value.ToStringArray<double>(new Func<string, double>(double.Parse));
      //if (type == typeof (DateTime))
      //  return (T[]) value.ToStringArray<DateTime>(new Func<string, DateTime>(DateTime.Parse));
      //if (type == typeof (string))
      //  return (T[]) value.ToStringArray<string>((Func<string, string>) (m => m));
      throw new Exception("use ToArray<T>(Func<string,T>) method instead");
    }

    /// <summary>
    /// 启动接收数据，需要传入回调方法，传递对象<br />
    /// To start receiving data, you need to pass in a callback method and pass an object
    /// </summary>
    /// <param name="socket">socket对象</param>
    /// <param name="callback">回调方法</param>
    /// <param name="obj">数据对象</param>
    /// <returns>是否启动成功</returns>
    public static OperateResult BeginReceiveResult(
      this Socket socket,
      AsyncCallback callback,
      object obj)
    {
      try
      {
        socket.BeginReceive(new byte[0], 0, 0, SocketFlags.None, callback, obj);
        return OperateResult.CreateSuccessResult();
      }
      catch (Exception ex)
      {
        socket?.Close();
        return new OperateResult(ex.Message);
      }
    }

    /// <summary>
    /// 启动接收数据，需要传入回调方法，传递对象默认为socket本身<br />
    /// To start receiving data, you need to pass in a callback method. The default object is the socket itself.
    /// </summary>
    /// <param name="socket">socket对象</param>
    /// <param name="callback">回调方法</param>
    /// <returns>是否启动成功</returns>
    public static OperateResult BeginReceiveResult(
      this Socket socket,
      AsyncCallback callback)
    {
      return socket.BeginReceiveResult(callback, (object) socket);
    }

    /// <summary>
    /// 结束挂起的异步读取，返回读取的字节数，如果成功的情况。<br />
    /// Ends the pending asynchronous read and returns the number of bytes read, if successful.
    /// </summary>
    /// <param name="socket">socket对象</param>
    /// <param name="ar">回调方法</param>
    /// <returns>是否启动成功</returns>
    public static OperateResult<int> EndReceiveResult(
      this Socket socket,
      IAsyncResult ar)
    {
      try
      {
        return OperateResult.CreateSuccessResult<int>(socket.EndReceive(ar));
      }
      catch (Exception ex)
      {
        socket?.Close();
        return new OperateResult<int>(ex.Message);
      }
    }

    /// <summary>
    /// 根据英文小数点进行切割字符串，去除空白的字符<br />
    /// Cut the string according to the English decimal point and remove the blank characters
    /// </summary>
    /// <param name="str">字符串本身</param>
    /// <returns>切割好的字符串数组，例如输入 "100.5"，返回 "100", "5"</returns>
    public static string[] SplitDot(this string str) => str.Split(new char[1]
    {
      '.'
    }, StringSplitOptions.RemoveEmptyEntries);

    /// <summary>
    /// 获取当前对象的JSON格式表示的字符串。<br />
    /// Gets the string represented by the JSON format of the current object.
    /// </summary>
    /// <returns>字符串对象</returns>
    public static string ToJsonString(this object obj, Formatting formatting = Formatting.Indented) => JsonConvert.SerializeObject(obj, formatting);
  }
}
