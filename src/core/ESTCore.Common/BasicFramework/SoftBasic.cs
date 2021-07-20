// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.BasicFramework.SoftBasic
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace ESTCore.Common.BasicFramework
{
    /// <summary>
    /// 一个软件基础类，提供常用的一些静态方法，比如字符串转换，字节转换的方法<br />
    /// A software-based class that provides some common static methods，Such as string conversion, byte conversion method
    /// </summary>
    public class SoftBasic
    {
        /// <summary>
        /// 获取文件的md5码<br />
        /// Get the MD5 code of the file
        /// </summary>
        /// <param name="filePath">文件的路径，既可以是完整的路径，也可以是相对的路径 -&gt; The path to the file</param>
        /// <returns>Md5字符串</returns>
        /// <example>
        /// 下面举例实现获取一个文件的md5码
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="CalculateFileMD5Example" title="CalculateFileMD5示例" />
        /// </example>
        public static string CalculateFileMD5(string filePath)
        {
            string str = string.Empty;
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                str = SoftBasic.CalculateStreamMD5((Stream)fileStream);
            return str;
        }

        /// <summary>
        /// 获取数据流的md5码<br />
        /// Get the MD5 code for the data stream
        /// </summary>
        /// <param name="stream">数据流，可以是内存流，也可以是文件流</param>
        /// <returns>Md5字符串</returns>
        /// <example>
        /// 下面举例实现获取一个流的md5码
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="CalculateStreamMD5Example1" title="CalculateStreamMD5示例" />
        /// </example>
        public static string CalculateStreamMD5(Stream stream)
        {
            byte[] numArray = (byte[])null;
            using (MD5 md5 = (MD5)new MD5CryptoServiceProvider())
                numArray = md5.ComputeHash(stream);
            return BitConverter.ToString(numArray).Replace("-", "");
        }

        /// <summary>
        /// 获取文本字符串信息的Md5码，编码为UTF8<br />
        /// Get the Md5 code of the text string information, using the utf-8 encoding
        /// </summary>
        /// <param name="data">文本数据信息</param>
        /// <returns>Md5字符串</returns>
        public static string CalculateStreamMD5(string data) => SoftBasic.CalculateStreamMD5(data, Encoding.UTF8);

        /// <summary>
        /// 获取文本字符串信息的Md5码，使用指定的编码<br />
        /// Get the Md5 code of the text string information, using the specified encoding
        /// </summary>
        /// <param name="data">文本数据信息</param>
        /// <param name="encode">编码信息</param>
        /// <returns>Md5字符串</returns>
        public static string CalculateStreamMD5(string data, Encoding encode)
        {
            string str = string.Empty;
            using (MD5 md5 = (MD5)new MD5CryptoServiceProvider())
                str = BitConverter.ToString(md5.ComputeHash(encode.GetBytes(data))).Replace("-", "");
            return str;
        }

        ///// <summary>
        ///// 获取内存图片的md5码<br />
        ///// Get the MD5 code of the memory picture
        ///// </summary>
        ///// <param name="bitmap">内存图片</param>
        ///// <returns>Md5字符串</returns>
        ///// <example>
        ///// 下面举例实现获取一个图像的md5码
        ///// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="CalculateStreamMD5Example2" title="CalculateStreamMD5示例" />
        ///// </example>
        //public static string CalculateStreamMD5(Bitmap bitmap)
        //{
        //  MemoryStream memoryStream = new MemoryStream();
        //  bitmap.Save((Stream) memoryStream, bitmap.RawFormat);
        //  byte[] InBytes = (byte[]) null;
        //  using (MD5 md5 = (MD5) new MD5CryptoServiceProvider())
        //    InBytes = md5.ComputeHash((Stream) memoryStream);
        //  memoryStream.Dispose();
        //  return SoftBasic.ByteToHexString(InBytes);
        //}

        /// <summary>
        /// 从一个字节大小返回带单位的描述，主要是用于显示操作<br />
        /// Returns a description with units from a byte size, mainly for display operations
        /// </summary>
        /// <param name="size">实际的大小值</param>
        /// <returns>最终的字符串值</returns>
        /// <example>
        /// 比如说我们获取了文件的长度，这个长度可以来自于本地，也可以来自于数据库查询
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="GetSizeDescriptionExample" title="GetSizeDescription示例" />
        /// </example>
        public static string GetSizeDescription(long size)
        {
            if (size < 1000L)
                return size.ToString() + " B";
            if (size < 1000000L)
                return ((float)size / 1024f).ToString("F2") + " Kb";
            return size < 1000000000L ? ((float)((double)size / 1024.0 / 1024.0)).ToString("F2") + " Mb" : ((float)((double)size / 1024.0 / 1024.0 / 1024.0)).ToString("F2") + " Gb";
        }

        /// <summary>
        /// 从一个时间差返回带单位的描述，主要是用于显示操作。<br />
        /// Returns a description with units from a time difference, mainly for display operations.
        /// </summary>
        /// <param name="ts">实际的时间差</param>
        /// <returns>最终的字符串值</returns>
        /// <example>
        /// 比如说我们获取了一个时间差信息
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="GetTimeSpanDescriptionExample" title="GetTimeSpanDescription示例" />
        /// </example>
        public static string GetTimeSpanDescription(TimeSpan ts)
        {
            if (ts.TotalSeconds <= 60.0)
                return ((int)ts.TotalSeconds).ToString() + StringResources.Language.TimeDescriptionSecond;
            if (ts.TotalMinutes <= 60.0)
                return ts.TotalMinutes.ToString("F1") + StringResources.Language.TimeDescriptionMinute;
            return ts.TotalHours <= 24.0 ? ts.TotalHours.ToString("F2") + StringResources.Language.TimeDescriptionHour : ts.TotalDays.ToString("F2") + StringResources.Language.TimeDescriptionDay;
        }

        /// <summary>
        /// 将数组格式化为显示的字符串的信息，支持所有的类型对象<br />
        /// Formats the array into the displayed string information, supporting all types of objects
        /// </summary>
        /// <typeparam name="T">数组的类型</typeparam>
        /// <param name="array">数组信息</param>
        /// <returns>最终显示的信息</returns>
        public static string ArrayFormat<T>(T[] array) => SoftBasic.ArrayFormat<T>(array, string.Empty);

        /// <summary>
        /// 将数组格式化为显示的字符串的信息，支持所有的类型对象<br />
        /// Formats the array into the displayed string information, supporting all types of objects
        /// </summary>
        /// <typeparam name="T">数组的类型</typeparam>
        /// <param name="array">数组信息</param>
        /// <param name="format">格式化的信息</param>
        /// <returns>最终显示的信息</returns>
        public static string ArrayFormat<T>(T[] array, string format)
        {
            if (array == null)
                return "NULL";
            StringBuilder stringBuilder = new StringBuilder("[");
            for (int index = 0; index < array.Length; ++index)
            {
                stringBuilder.Append(string.IsNullOrEmpty(format) ? array[index].ToString() : string.Format(format, (object)array[index]));
                if (index != array.Length - 1)
                    stringBuilder.Append(",");
            }
            stringBuilder.Append("]");
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 将数组格式化为显示的字符串的信息，支持所有的类型对象<br />
        /// Formats the array into the displayed string information, supporting all types of objects
        /// </summary>
        /// <typeparam name="T">数组的类型</typeparam>
        /// <param name="array">数组信息</param>
        /// <returns>最终显示的信息</returns>
        public static string ArrayFormat<T>(T array) => SoftBasic.ArrayFormat<T>(array, string.Empty);

        /// <summary>
        /// 将数组格式化为显示的字符串的信息，支持所有的类型对象<br />
        /// Formats the array into the displayed string information, supporting all types of objects
        /// </summary>
        /// <typeparam name="T">数组的类型</typeparam>
        /// <param name="array">数组信息</param>
        /// <param name="format">格式化的信息</param>
        /// <returns>最终显示的信息</returns>
        public static string ArrayFormat<T>(T array, string format)
        {
            StringBuilder stringBuilder = new StringBuilder("[");
            if (array is Array array1)
            {
                foreach (object obj in array1)
                {
                    stringBuilder.Append(string.IsNullOrEmpty(format) ? obj.ToString() : string.Format(format, obj));
                    stringBuilder.Append(",");
                }
                if (array1.Length > 0 && stringBuilder[stringBuilder.Length - 1] == ',')
                    stringBuilder.Remove(stringBuilder.Length - 1, 1);
            }
            else
                stringBuilder.Append(string.IsNullOrEmpty(format) ? array.ToString() : string.Format(format, (object)array));
            stringBuilder.Append("]");
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 一个通用的数组新增个数方法，会自动判断越界情况，越界的情况下，会自动的截断或是填充<br />
        /// A common array of new methods, will automatically determine the cross-border situation, in the case of cross-border, will be automatically truncated or filled
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="array">原数据</param>
        /// <param name="data">等待新增的数据</param>
        /// <param name="max">原数据的最大值</param>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="AddArrayDataExample" title="AddArrayData示例" />
        /// </example>
        public static void AddArrayData<T>(ref T[] array, T[] data, int max)
        {
            if (data == null || data.Length == 0)
                return;
            if (array.Length == max)
            {
                Array.Copy((Array)array, data.Length, (Array)array, 0, array.Length - data.Length);
                Array.Copy((Array)data, 0, (Array)array, array.Length - data.Length, data.Length);
            }
            else if (array.Length + data.Length > max)
            {
                T[] objArray = new T[max];
                for (int index = 0; index < max - data.Length; ++index)
                    objArray[index] = array[index + (array.Length - max + data.Length)];
                for (int index = 0; index < data.Length; ++index)
                    objArray[objArray.Length - data.Length + index] = data[index];
                array = objArray;
            }
            else
            {
                T[] objArray = new T[array.Length + data.Length];
                for (int index = 0; index < array.Length; ++index)
                    objArray[index] = array[index];
                for (int index = 0; index < data.Length; ++index)
                    objArray[objArray.Length - data.Length + index] = data[index];
                array = objArray;
            }
        }

        /// <summary>
        /// 将一个数组进行扩充到指定长度，或是缩短到指定长度<br />
        /// Extend an array to a specified length, or shorten to a specified length or fill
        /// </summary>
        /// <typeparam name="T">数组的类型</typeparam>
        /// <param name="data">原先数据的数据</param>
        /// <param name="length">新数组的长度</param>
        /// <returns>新数组长度信息</returns>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="ArrayExpandToLengthExample" title="ArrayExpandToLength示例" />
        /// </example>
        public static T[] ArrayExpandToLength<T>(T[] data, int length)
        {
            if (data == null)
                return new T[length];
            if (data.Length == length)
                return data;
            T[] objArray = new T[length];
            Array.Copy((Array)data, (Array)objArray, Math.Min(data.Length, objArray.Length));
            return objArray;
        }

        /// <summary>
        /// 将一个数组进行扩充到偶数长度<br />
        /// Extend an array to even lengths
        /// </summary>
        /// <typeparam name="T">数组的类型</typeparam>
        /// <param name="data">原先数据的数据</param>
        /// <returns>新数组长度信息</returns>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="ArrayExpandToLengthEvenExample" title="ArrayExpandToLengthEven示例" />
        /// </example>
        public static T[] ArrayExpandToLengthEven<T>(T[] data)
        {
            if (data == null)
                return new T[0];
            return data.Length % 2 == 1 ? SoftBasic.ArrayExpandToLength<T>(data, data.Length + 1) : data;
        }

        /// <summary>
        /// 将指定的数据按照指定长度进行分割，例如int[10]，指定长度4，就分割成int[4],int[4],int[2]，然后拼接list<br />
        /// Divide the specified data according to the specified length, such as int [10], and specify the length of 4 to divide into int [4], int [4], int [2], and then concatenate the list
        /// </summary>
        /// <typeparam name="T">数组的类型</typeparam>
        /// <param name="array">等待分割的数组</param>
        /// <param name="length">指定的长度信息</param>
        /// <returns>分割后结果内容</returns>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="ArraySplitByLengthExample" title="ArraySplitByLength示例" />
        /// </example>
        public static List<T[]> ArraySplitByLength<T>(T[] array, int length)
        {
            if (array == null)
                return new List<T[]>();
            List<T[]> objArrayList = new List<T[]>();
            int sourceIndex = 0;
            while (sourceIndex < array.Length)
            {
                if (sourceIndex + length < array.Length)
                {
                    T[] objArray = new T[length];
                    Array.Copy((Array)array, sourceIndex, (Array)objArray, 0, length);
                    sourceIndex += length;
                    objArrayList.Add(objArray);
                }
                else
                {
                    T[] objArray = new T[array.Length - sourceIndex];
                    Array.Copy((Array)array, sourceIndex, (Array)objArray, 0, objArray.Length);
                    sourceIndex += length;
                    objArrayList.Add(objArray);
                }
            }
            return objArrayList;
        }

        /// <summary>
        /// 将整数进行有效的拆分成数组，指定每个元素的最大值<br />
        /// Effectively split integers into arrays, specifying the maximum value for each element
        /// </summary>
        /// <param name="integer">整数信息</param>
        /// <param name="everyLength">单个的数组长度</param>
        /// <returns>拆分后的数组长度</returns>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="SplitIntegerToArrayExample" title="SplitIntegerToArray示例" />
        /// </example>
        public static int[] SplitIntegerToArray(int integer, int everyLength)
        {
            int[] numArray = new int[integer / everyLength + (integer % everyLength == 0 ? 0 : 1)];
            for (int index = 0; index < numArray.Length; ++index)
                numArray[index] = index != numArray.Length - 1 ? everyLength : (integer % everyLength == 0 ? everyLength : integer % everyLength);
            return numArray;
        }

        /// <summary>
        /// 判断两个字节的指定部分是否相同<br />
        /// Determines whether the specified portion of a two-byte is the same
        /// </summary>
        /// <param name="b1">第一个字节</param>
        /// <param name="start1">第一个字节的起始位置</param>
        /// <param name="b2">第二个字节</param>
        /// <param name="start2">第二个字节的起始位置</param>
        /// <param name="length">校验的长度</param>
        /// <returns>返回是否相等</returns>
        /// <exception cref="T:System.IndexOutOfRangeException"></exception>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="IsTwoBytesEquelExample1" title="IsTwoBytesEquel示例" />
        /// </example>
        public static bool IsTwoBytesEquel(byte[] b1, int start1, byte[] b2, int start2, int length)
        {
            if (b1 == null || b2 == null)
                return false;
            for (int index = 0; index < length; ++index)
            {
                if ((int)b1[index + start1] != (int)b2[index + start2])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 判断两个字节的指定部分是否相同<br />
        /// Determines whether the specified portion of a two-byte is the same
        /// </summary>
        /// <param name="b1">第一个字节</param>
        /// <param name="b2">第二个字节</param>
        /// <returns>返回是否相等</returns>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="IsTwoBytesEquelExample2" title="IsTwoBytesEquel示例" />
        /// </example>
        public static bool IsTwoBytesEquel(byte[] b1, byte[] b2) => b1 != null && b2 != null && b1.Length == b2.Length && SoftBasic.IsTwoBytesEquel(b1, 0, b2, 0, b1.Length);

        /// <summary>
        /// 判断两个数据的令牌是否相等<br />
        /// Determines whether the tokens of two data are equal
        /// </summary>
        /// <param name="head">字节数据</param>
        /// <param name="token">GUID数据</param>
        /// <returns>返回是否相等</returns>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="IsTwoTokenEquelExample" title="IsByteTokenEquel示例" />
        /// </example>
        public static bool IsByteTokenEquel(byte[] head, Guid token) => SoftBasic.IsTwoBytesEquel(head, 12, token.ToByteArray(), 0, 16);

        /// <summary>
        /// 判断两个数据的令牌是否相等<br />
        /// Determines whether the tokens of two data are equal
        /// </summary>
        /// <param name="token1">第一个令牌</param>
        /// <param name="token2">第二个令牌</param>
        /// <returns>返回是否相等</returns>
        public static bool IsTwoTokenEquel(Guid token1, Guid token2) => SoftBasic.IsTwoBytesEquel(token1.ToByteArray(), 0, token2.ToByteArray(), 0, 16);

        /// <summary>
        /// 获取一个枚举类型的所有枚举值，可直接应用于组合框数据<br />
        /// Gets all the enumeration values of an enumeration type that can be applied directly to the combo box data
        /// </summary>
        /// <typeparam name="TEnum">枚举的类型值</typeparam>
        /// <returns>枚举值数组</returns>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="GetEnumValuesExample" title="GetEnumValues示例" />
        /// </example>
        public static TEnum[] GetEnumValues<TEnum>() where TEnum : struct => (TEnum[])Enum.GetValues(typeof(TEnum));

        /// <summary>
        /// 从字符串的枚举值数据转换成真实的枚举值数据<br />
        /// Convert enumeration value data from strings to real enumeration value data
        /// </summary>
        /// <typeparam name="TEnum">枚举的类型值</typeparam>
        /// <param name="value">枚举的字符串的数据值</param>
        /// <returns>真实的枚举值</returns>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="GetEnumFromStringExample" title="GetEnumFromString示例" />
        /// </example>
        public static TEnum GetEnumFromString<TEnum>(string value) where TEnum : struct => (TEnum)Enum.Parse(typeof(TEnum), value);

        /// <summary>
        /// 一个泛型方法，提供json对象的数据读取<br />
        /// A generic method that provides data read for a JSON object
        /// </summary>
        /// <typeparam name="T">读取的泛型</typeparam>
        /// <param name="json">json对象</param>
        /// <param name="name">值名称</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>值对象</returns>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="GetValueFromJsonObjectExample" title="GetValueFromJsonObject示例" />
        /// </example>
        public static T GetValueFromJsonObject<T>(JObject json, string name, T defaultValue) => json.Property(name) != null ? json.Property(name).Value.Value<T>() : defaultValue;

        /// <summary>
        /// 一个泛型方法，提供json对象的数据写入<br />
        /// A generic method that provides data writing to a JSON object
        /// </summary>
        /// <typeparam name="T">写入的泛型</typeparam>
        /// <param name="json">json对象</param>
        /// <param name="property">值名称</param>
        /// <param name="value">值数据</param>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="JsonSetValueExample" title="JsonSetValue示例" />
        /// </example>
        public static void JsonSetValue<T>(JObject json, string property, T value)
        {
            if (json.Property(property) != null)
                json.Property(property).Value = (JToken)new JValue((object)value);
            else
                json.Add(property, (JToken)new JValue((object)value));
        }

        ///// <summary>
        ///// 显示一个完整的错误信息<br />
        ///// Displays a complete error message
        ///// </summary>
        ///// <param name="ex">异常对象</param>
        ///// <remarks>调用本方法可以显示一个异常的详细信息</remarks>
        ///// <exception cref="T:System.NullReferenceException"></exception>
        //public static void ShowExceptionMessage(Exception ex)
        //{
        //  int num = (int) MessageBox.Show(SoftBasic.GetExceptionMessage(ex));
        //}

        ///// <summary>
        ///// 显示一个完整的错误信息，和额外的字符串描述信息<br />
        ///// Displays a complete error message, and additional string description information
        ///// </summary>
        ///// <param name="extraMsg">额外的描述信息</param>
        ///// <remarks>调用本方法可以显示一个异常的详细信息</remarks>
        ///// <param name="ex">异常对象</param>
        ///// <exception cref="T:System.NullReferenceException"></exception>
        //public static void ShowExceptionMessage(string extraMsg, Exception ex)
        //{
        //  int num = (int) MessageBox.Show(SoftBasic.GetExceptionMessage(extraMsg, ex));
        //}

        /// <summary>
        /// 获取一个异常的完整错误信息<br />
        /// Gets the complete error message for an exception
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <returns>完整的字符串数据</returns>
        /// <remarks>获取异常的完整信息</remarks>
        /// <exception cref="T:System.NullReferenceException">ex不能为空</exception>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="GetExceptionMessageExample1" title="GetExceptionMessage示例" />
        /// </example>
        public static string GetExceptionMessage(Exception ex)
        {
            string[] strArray = new string[8]
            {
        StringResources.Language.ExceptionMessage,
        ex.Message,
        Environment.NewLine,
        StringResources.Language.ExceptionStackTrace,
        ex.StackTrace,
        Environment.NewLine,
        StringResources.Language.ExceptionTargetSite,
        null
            };
            MethodBase targetSite = ex.TargetSite;
            strArray[7] = (object)targetSite != null ? targetSite.ToString() : (string)null;
            return string.Concat(strArray);
        }

        /// <summary>
        /// 获取一个异常的完整错误信息，和额外的字符串描述信息<br />
        /// Gets the complete error message for an exception, and additional string description information
        /// </summary>
        /// <param name="extraMsg">额外的信息</param>
        /// <param name="ex">异常对象</param>
        /// <returns>完整的字符串数据</returns>
        /// <exception cref="T:System.NullReferenceException"></exception>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="GetExceptionMessageExample2" title="GetExceptionMessage示例" />
        /// </example>
        public static string GetExceptionMessage(string extraMsg, Exception ex) => string.IsNullOrEmpty(extraMsg) ? SoftBasic.GetExceptionMessage(ex) : extraMsg + Environment.NewLine + SoftBasic.GetExceptionMessage(ex);

        /// <summary>
        /// 字节数据转化成16进制表示的字符串<br />
        /// Byte data into a string of 16 binary representations
        /// </summary>
        /// <param name="InBytes">字节数组</param>
        /// <returns>返回的字符串</returns>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="ByteToHexStringExample1" title="ByteToHexString示例" />
        /// </example>
        public static string ByteToHexString(byte[] InBytes) => SoftBasic.ByteToHexString(InBytes, char.MinValue);

        /// <summary>
        /// 字节数据转化成16进制表示的字符串<br />
        /// Byte data into a string of 16 binary representations
        /// </summary>
        /// <param name="InBytes">字节数组</param>
        /// <param name="segment">分割符</param>
        /// <returns>返回的字符串</returns>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="ByteToHexStringExample2" title="ByteToHexString示例" />
        /// </example>
        public static string ByteToHexString(byte[] InBytes, char segment) => SoftBasic.ByteToHexString(InBytes, segment, 0);

        /// <summary>
        /// 字节数据转化成16进制表示的字符串<br />
        /// Byte data into a string of 16 binary representations
        /// </summary>
        /// <param name="InBytes">字节数组</param>
        /// <param name="segment">分割符</param>
        /// <param name="newLineCount">每隔指定数量的时候进行换行</param>
        /// <returns>返回的字符串</returns>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="ByteToHexStringExample2" title="ByteToHexString示例" />
        /// </example>
        public static string ByteToHexString(byte[] InBytes, char segment, int newLineCount)
        {
            if (InBytes == null)
                return string.Empty;
            StringBuilder stringBuilder = new StringBuilder();
            long num = 0;
            foreach (byte inByte in InBytes)
            {
                if (segment == char.MinValue)
                    stringBuilder.Append(string.Format("{0:X2}", (object)inByte));
                else
                    stringBuilder.Append(string.Format("{0:X2}{1}", (object)inByte, (object)segment));
                ++num;
                if (newLineCount > 0 && num >= (long)newLineCount)
                {
                    stringBuilder.Append(Environment.NewLine);
                    num = 0L;
                }
            }
            if (segment != char.MinValue && stringBuilder.Length > 1 && (int)stringBuilder[stringBuilder.Length - 1] == (int)segment)
                stringBuilder.Remove(stringBuilder.Length - 1, 1);
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 字符串数据转化成16进制表示的字符串<br />
        /// String data into a string of 16 binary representations
        /// </summary>
        /// <param name="InString">输入的字符串数据</param>
        /// <returns>返回的字符串</returns>
        /// <exception cref="T:System.NullReferenceException"></exception>
        public static string ByteToHexString(string InString) => SoftBasic.ByteToHexString(Encoding.Unicode.GetBytes(InString));

        private static int GetHexCharIndex(char ch)
        {
            switch (ch)
            {
                case '0':
                    return 0;
                case '1':
                    return 1;
                case '2':
                    return 2;
                case '3':
                    return 3;
                case '4':
                    return 4;
                case '5':
                    return 5;
                case '6':
                    return 6;
                case '7':
                    return 7;
                case '8':
                    return 8;
                case '9':
                    return 9;
                case 'A':
                case 'a':
                    return 10;
                case 'B':
                case 'b':
                    return 11;
                case 'C':
                case 'c':
                    return 12;
                case 'D':
                case 'd':
                    return 13;
                case 'E':
                case 'e':
                    return 14;
                case 'F':
                case 'f':
                    return 15;
                default:
                    return -1;
            }
        }

        /// <summary>
        /// 将16进制的字符串转化成Byte数据，将检测每2个字符转化，也就是说，中间可以是任意字符<br />
        /// Converts a 16-character string into byte data, which will detect every 2 characters converted, that is, the middle can be any character
        /// </summary>
        /// <param name="hex">十六进制的字符串，中间可以是任意的分隔符</param>
        /// <returns>转换后的字节数组</returns>
        /// <remarks>参数举例：AA 01 34 A8</remarks>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="HexStringToBytesExample" title="HexStringToBytes示例" />
        /// </example>
        public static byte[] HexStringToBytes(string hex)
        {
            MemoryStream memoryStream = new MemoryStream();
            for (int index = 0; index < hex.Length; ++index)
            {
                if (index + 1 < hex.Length && (SoftBasic.GetHexCharIndex(hex[index]) >= 0 && SoftBasic.GetHexCharIndex(hex[index + 1]) >= 0))
                {
                    memoryStream.WriteByte((byte)(SoftBasic.GetHexCharIndex(hex[index]) * 16 + SoftBasic.GetHexCharIndex(hex[index + 1])));
                    ++index;
                }
            }
            byte[] array = memoryStream.ToArray();
            memoryStream.Dispose();
            return array;
        }

        /// <summary>
        /// 将byte数组按照双字节进行反转，如果为单数的情况，则自动补齐<br />
        /// Reverses the byte array by double byte, or if the singular is the case, automatically
        /// </summary>
        /// <param name="inBytes">输入的字节信息</param>
        /// <returns>反转后的数据</returns>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="BytesReverseByWord" title="BytesReverseByWord示例" />
        /// </example>
        public static byte[] BytesReverseByWord(byte[] inBytes)
        {
            if (inBytes == null)
                return (byte[])null;
            if (inBytes.Length == 0)
                return new byte[0];
            byte[] lengthEven = SoftBasic.ArrayExpandToLengthEven<byte>(inBytes.CopyArray<byte>());
            for (int index = 0; index < lengthEven.Length / 2; ++index)
            {
                byte num = lengthEven[index * 2];
                lengthEven[index * 2] = lengthEven[index * 2 + 1];
                lengthEven[index * 2 + 1] = num;
            }
            return lengthEven;
        }

        /// <summary>
        /// 将原始的byte数组转换成ascii格式的byte数组<br />
        /// Converts the original byte array to an ASCII-formatted byte array
        /// </summary>
        /// <param name="inBytes">等待转换的byte数组</param>
        /// <returns>转换后的数组</returns>
        public static byte[] BytesToAsciiBytes(byte[] inBytes) => Encoding.ASCII.GetBytes(SoftBasic.ByteToHexString(inBytes));

        /// <summary>
        /// 将ascii格式的byte数组转换成原始的byte数组<br />
        /// Converts an ASCII-formatted byte array to the original byte array
        /// </summary>
        /// <param name="inBytes">等待转换的byte数组</param>
        /// <returns>转换后的数组</returns>
        public static byte[] AsciiBytesToBytes(byte[] inBytes) => SoftBasic.HexStringToBytes(Encoding.ASCII.GetString(inBytes));

        /// <summary>
        /// 从字节构建一个ASCII格式的数据内容<br />
        /// Build an ASCII-formatted data content from bytes
        /// </summary>
        /// <param name="value">数据</param>
        /// <returns>ASCII格式的字节数组</returns>
        public static byte[] BuildAsciiBytesFrom(byte value) => Encoding.ASCII.GetBytes(value.ToString("X2"));

        /// <summary>
        /// 从short构建一个ASCII格式的数据内容<br />
        /// Constructing an ASCII-formatted data content from a short
        /// </summary>
        /// <param name="value">数据</param>
        /// <returns>ASCII格式的字节数组</returns>
        public static byte[] BuildAsciiBytesFrom(short value) => Encoding.ASCII.GetBytes(value.ToString("X4"));

        /// <summary>
        /// 从ushort构建一个ASCII格式的数据内容<br />
        /// Constructing an ASCII-formatted data content from ushort
        /// </summary>
        /// <param name="value">数据</param>
        /// <returns>ASCII格式的字节数组</returns>
        public static byte[] BuildAsciiBytesFrom(ushort value) => Encoding.ASCII.GetBytes(value.ToString("X4"));

        /// <summary>
        /// 从uint构建一个ASCII格式的数据内容<br />
        /// Constructing an ASCII-formatted data content from uint
        /// </summary>
        /// <param name="value">数据</param>
        /// <returns>ASCII格式的字节数组</returns>
        public static byte[] BuildAsciiBytesFrom(uint value) => Encoding.ASCII.GetBytes(value.ToString("X8"));

        /// <summary>
        /// 从字节数组构建一个ASCII格式的数据内容<br />
        /// Byte array to construct an ASCII format data content
        /// </summary>
        /// <param name="value">字节信息</param>
        /// <returns>ASCII格式的地址</returns>
        public static byte[] BuildAsciiBytesFrom(byte[] value)
        {
            byte[] numArray = new byte[value.Length * 2];
            for (int index = 0; index < value.Length; ++index)
                SoftBasic.BuildAsciiBytesFrom(value[index]).CopyTo((Array)numArray, 2 * index);
            return numArray;
        }

        private static byte GetDataByBitIndex(int offset)
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
        /// 获取byte数据类型的第offset位，是否为True<br />
        /// Gets the index bit of the byte data type, whether it is True
        /// </summary>
        /// <param name="value">byte数值</param>
        /// <param name="offset">索引位置</param>
        /// <returns>结果</returns>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="BoolOnByteIndex" title="BoolOnByteIndex示例" />
        /// </example>
        public static bool BoolOnByteIndex(byte value, int offset)
        {
            byte dataByBitIndex = SoftBasic.GetDataByBitIndex(offset);
            return ((int)value & (int)dataByBitIndex) == (int)dataByBitIndex;
        }

        /// <summary>
        /// 设置取byte数据类型的第offset位，是否为True<br />
        /// Set the offset bit of the byte data type, whether it is True
        /// </summary>
        /// <param name="byt">byte数值</param>
        /// <param name="offset">索引位置</param>
        /// <param name="value">写入的结果值</param>
        /// <returns>结果</returns>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="SetBoolOnByteIndex" title="SetBoolOnByteIndex示例" />
        /// </example>
        public static byte SetBoolOnByteIndex(byte byt, int offset, bool value)
        {
            byte dataByBitIndex = SoftBasic.GetDataByBitIndex(offset);
            return value ? (byte)((uint)byt | (uint)dataByBitIndex) : (byte)((uint)byt & (uint)~dataByBitIndex);
        }

        /// <summary>
        /// 将bool数组转换到byte数组<br />
        /// Converting a bool array to a byte array
        /// </summary>
        /// <param name="array">bool数组</param>
        /// <returns>转换后的字节数组</returns>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="BoolArrayToByte" title="BoolArrayToByte示例" />
        /// </example>
        public static byte[] BoolArrayToByte(bool[] array)
        {
            if (array == null)
                return (byte[])null;
            byte[] numArray = new byte[array.Length % 8 == 0 ? array.Length / 8 : array.Length / 8 + 1];
            for (int index = 0; index < array.Length; ++index)
            {
                if (array[index])
                    numArray[index / 8] += SoftBasic.GetDataByBitIndex(index % 8);
            }
            return numArray;
        }

        /// <summary>
        /// 从Byte数组中提取位数组，length代表位数<br />
        /// Extracts a bit array from a byte array, length represents the number of digits
        /// </summary>
        /// <param name="InBytes">原先的字节数组</param>
        /// <param name="length">想要转换的长度，如果超出自动会缩小到数组最大长度</param>
        /// <returns>转换后的bool数组</returns>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="ByteToBoolArray" title="ByteToBoolArray示例" />
        /// </example>
        public static bool[] ByteToBoolArray(byte[] InBytes, int length)
        {
            if (InBytes == null)
                return (bool[])null;
            if (length > InBytes.Length * 8)
                length = InBytes.Length * 8;
            bool[] flagArray = new bool[length];
            for (int index = 0; index < length; ++index)
                flagArray[index] = SoftBasic.BoolOnByteIndex(InBytes[index / 8], index % 8);
            return flagArray;
        }

        /// <summary>
        /// 从Byte数组中提取所有的位数组<br />
        /// Extracts a bit array from a byte array, length represents the number of digits
        /// </summary>
        /// <param name="InBytes">原先的字节数组</param>
        /// <returns>转换后的bool数组</returns>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="ByteToBoolArray" title="ByteToBoolArray示例" />
        /// </example>
        public static bool[] ByteToBoolArray(byte[] InBytes) => InBytes != null ? SoftBasic.ByteToBoolArray(InBytes, InBytes.Length * 8) : (bool[])null;

        /// <summary>
        /// 将一个数组的前后移除指定位数，返回新的一个数组<br />
        /// Removes a array before and after the specified number of bits, returning a new array
        /// </summary>
        /// <param name="value">数组</param>
        /// <param name="leftLength">前面的位数</param>
        /// <param name="rightLength">后面的位数</param>
        /// <returns>新的数组</returns>
        /// <exception cref="T:System.RankException"></exception>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="ArrayRemoveDouble" title="ArrayRemoveDouble示例" />
        /// </example>
        public static T[] ArrayRemoveDouble<T>(T[] value, int leftLength, int rightLength)
        {
            if (value == null)
                return (T[])null;
            if (value.Length <= leftLength + rightLength)
                return new T[0];
            T[] objArray = new T[value.Length - leftLength - rightLength];
            Array.Copy((Array)value, leftLength, (Array)objArray, 0, objArray.Length);
            return objArray;
        }

        /// <summary>
        /// 将一个数组的前面指定位数移除，返回新的一个数组<br />
        /// Removes the preceding specified number of bits in a array, returning a new array
        /// </summary>
        /// <param name="value">数组</param>
        /// <param name="length">等待移除的长度</param>
        /// <returns>新的数组</returns>
        /// <exception cref="T:System.RankException"></exception>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="ArrayRemoveBegin" title="ArrayRemoveBegin示例" />
        /// </example>
        public static T[] ArrayRemoveBegin<T>(T[] value, int length) => SoftBasic.ArrayRemoveDouble<T>(value, length, 0);

        /// <summary>
        /// 将一个数组的后面指定位数移除，返回新的一个数组<br />
        /// Removes the specified number of digits after a array, returning a new array
        /// </summary>
        /// <param name="value">数组</param>
        /// <param name="length">等待移除的长度</param>
        /// <returns>新的数组</returns>
        /// <exception cref="T:System.RankException"></exception>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="ArrayRemoveLast" title="ArrayRemoveLast示例" />
        /// </example>
        public static T[] ArrayRemoveLast<T>(T[] value, int length) => SoftBasic.ArrayRemoveDouble<T>(value, 0, length);

        /// <summary>
        /// 获取到数组里面的中间指定长度的数组<br />
        /// Get an array of the specified length in the array
        /// </summary>
        /// <param name="value">数组</param>
        /// <param name="index">起始索引</param>
        /// <param name="length">数据的长度</param>
        /// <returns>新的数组值</returns>
        /// <exception cref="T:System.IndexOutOfRangeException"></exception>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="ArraySelectMiddle" title="ArraySelectMiddle示例" />
        /// </example>
        public static T[] ArraySelectMiddle<T>(T[] value, int index, int length)
        {
            if (value == null)
                return (T[])null;
            T[] objArray = new T[Math.Min(value.Length, length)];
            Array.Copy((Array)value, index, (Array)objArray, 0, objArray.Length);
            return objArray;
        }

        /// <summary>
        /// 选择一个数组的前面的几个数据信息<br />
        /// Select the begin few items of data information of a array
        /// </summary>
        /// <param name="value">数组</param>
        /// <param name="length">数据的长度</param>
        /// <returns>新的数组</returns>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="ArraySelectBegin" title="ArraySelectBegin示例" />
        /// </example>
        public static T[] ArraySelectBegin<T>(T[] value, int length)
        {
            T[] objArray = new T[Math.Min(value.Length, length)];
            if ((uint)objArray.Length > 0U)
                Array.Copy((Array)value, 0, (Array)objArray, 0, objArray.Length);
            return objArray;
        }

        /// <summary>
        /// 选择一个数组的后面的几个数据信息<br />
        /// Select the last few items of data information of a array
        /// </summary>
        /// <param name="value">数组</param>
        /// <param name="length">数据的长度</param>
        /// <returns>新的数组信息</returns>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="ArraySelectLast" title="ArraySelectLast示例" />
        /// </example>
        public static T[] ArraySelectLast<T>(T[] value, int length)
        {
            T[] objArray = new T[Math.Min(value.Length, length)];
            Array.Copy((Array)value, value.Length - length, (Array)objArray, 0, objArray.Length);
            return objArray;
        }

        /// <summary>
        /// 拼接任意个泛型数组为一个总的泛型数组对象，采用深度拷贝实现。<br />
        /// Splicing any number of generic arrays into a total generic array object is implemented using deep copy.
        /// </summary>
        /// <typeparam name="T">数组的类型信息</typeparam>
        /// <param name="arrays">任意个长度的数组</param>
        /// <returns>拼接之后的最终的结果对象</returns>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="SpliceByteArray" title="SpliceByteArray示例" />
        /// </example>
        public static T[] SpliceArray<T>(params T[][] arrays)
        {
            int length = 0;
            for (int index = 0; index < arrays.Length; ++index)
            {
                T[] array = arrays[index];
                if (array != null && (uint)array.Length > 0U)
                    length += arrays[index].Length;
            }
            int index1 = 0;
            T[] objArray = new T[length];
            for (int index2 = 0; index2 < arrays.Length; ++index2)
            {
                T[] array = arrays[index2];
                if (array != null && (uint)array.Length > 0U)
                {
                    arrays[index2].CopyTo((Array)objArray, index1);
                    index1 += arrays[index2].Length;
                }
            }
            return objArray;
        }

        /// <summary>
        /// 将一个<see cref="T:System.String" />的数组和多个<see cref="T:System.String" /> 类型的对象整合成一个数组<br />
        /// Combine an array of <see cref="T:System.String" /> and multiple objects of type <see cref="T:System.String" /> into an array
        /// </summary>
        /// <param name="first">第一个数组对象</param>
        /// <param name="array">字符串数组信息</param>
        /// <returns>总的数组对象</returns>
        public static string[] SpliceStringArray(string first, string[] array)
        {
            List<string> stringList = new List<string>();
            stringList.Add(first);
            stringList.AddRange((IEnumerable<string>)array);
            return stringList.ToArray();
        }

        /// <summary>
        /// 将两个<see cref="T:System.String" />的数组和多个<see cref="T:System.String" /> 类型的对象整合成一个数组<br />
        /// Combine two arrays of <see cref="T:System.String" /> and multiple objects of type <see cref="T:System.String" /> into one array
        /// </summary>
        /// <param name="first">第一个数据对象</param>
        /// <param name="second">第二个数据对象</param>
        /// <param name="array">字符串数组信息</param>
        /// <returns>总的数组对象</returns>
        public static string[] SpliceStringArray(string first, string second, string[] array)
        {
            List<string> stringList = new List<string>();
            stringList.Add(first);
            stringList.Add(second);
            stringList.AddRange((IEnumerable<string>)array);
            return stringList.ToArray();
        }

        /// <summary>
        /// 将两个<see cref="T:System.String" />的数组和多个<see cref="T:System.String" /> 类型的对象整合成一个数组<br />
        /// Combine two arrays of <see cref="T:System.String" /> and multiple objects of type <see cref="T:System.String" /> into one array
        /// </summary>
        /// <param name="first">第一个数据对象</param>
        /// <param name="second">第二个数据对象</param>
        /// <param name="third">第三个数据对象</param>
        /// <param name="array">字符串数组信息</param>
        /// <returns>总的数组对象</returns>
        public static string[] SpliceStringArray(
          string first,
          string second,
          string third,
          string[] array)
        {
            List<string> stringList = new List<string>();
            stringList.Add(first);
            stringList.Add(second);
            stringList.Add(third);
            stringList.AddRange((IEnumerable<string>)array);
            return stringList.ToArray();
        }

        /// <summary>
        /// 设置或获取系统框架的版本号<br />
        /// Set or get the version number of the system framework
        /// </summary>
        /// <remarks>当你要显示本组件框架的版本号的时候，就可以用这个属性来显示</remarks>
        public static SystemVersion FrameworkVersion => new SystemVersion("9.7.0");

        /// <summary>
        /// 使用序列化反序列化深度克隆一个对象，该对象需要支持序列化特性<br />
        /// Cloning an object with serialization deserialization depth that requires support for serialization attributes
        /// </summary>
        /// <param name="oringinal">源对象，支持序列化</param>
        /// <returns>新的一个实例化的对象</returns>
        /// <exception cref="T:System.NullReferenceException"></exception>
        /// <exception cref="T:System.NonSerializedAttribute"></exception>
        /// <remarks>
        /// <note type="warning">
        /// <paramref name="oringinal" /> 参数必须实现序列化的特性
        /// </note>
        /// </remarks>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="DeepClone" title="DeepClone示例" />
        /// </example>
        public static object DeepClone(object oringinal)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter()
                {
                    Context = new StreamingContext(StreamingContextStates.Clone)
                };
                binaryFormatter.Serialize((Stream)memoryStream, oringinal);
                memoryStream.Position = 0L;
                return binaryFormatter.Deserialize((Stream)memoryStream);
            }
        }

        /// <summary>
        /// 获取一串唯一的随机字符串，长度为20，由Guid码和4位数的随机数组成，保证字符串的唯一性<br />
        /// Gets a string of unique random strings with a length of 20, consisting of a GUID code and a 4-digit random number to guarantee the uniqueness of the string
        /// </summary>
        /// <returns>随机字符串数据</returns>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftBasicExample.cs" region="GetUniqueStringByGuidAndRandom" title="GetUniqueStringByGuidAndRandom示例" />
        /// </example>
        public static string GetUniqueStringByGuidAndRandom()
        {
            Random random = new Random();
            return Guid.NewGuid().ToString("N") + random.Next(1000, 10000).ToString();
        }
    }
}
