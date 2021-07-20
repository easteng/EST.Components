// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Reflection.EstReflectionHelper
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Core;
using ESTCore.Common.Enthernet.Redis;

using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace ESTCore.Common.Reflection
{
    /// <summary>反射的辅助类</summary>
    public class EstReflectionHelper
    {
        /// <summary>
        /// 从设备里读取支持Est特性的数据内容，该特性为<see cref="T:ESTCore.Common.Reflection.EstDeviceAddressAttribute" />，详细参考论坛的操作说明。
        /// </summary>
        /// <typeparam name="T">自定义的数据类型对象</typeparam>
        /// <param name="readWrite">读写接口的实现</param>
        /// <returns>包含是否成功的结果对象</returns>
        public static OperateResult<T> Read<T>(IReadWriteNet readWrite) where T : class, new()
        {
            Type type = typeof(T);
            object instance = type.Assembly.CreateInstance(type.FullName);
            foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                object[] customAttributes = property.GetCustomAttributes(typeof(EstDeviceAddressAttribute), false);
                if (customAttributes != null)
                {
                    EstDeviceAddressAttribute addressAttribute1 = (EstDeviceAddressAttribute)null;
                    for (int index = 0; index < customAttributes.Length; ++index)
                    {
                        EstDeviceAddressAttribute addressAttribute2 = (EstDeviceAddressAttribute)customAttributes[index];
                        if (addressAttribute2.DeviceType != (Type)null && addressAttribute2.DeviceType == readWrite.GetType())
                        {
                            addressAttribute1 = addressAttribute2;
                            break;
                        }
                    }
                    if (addressAttribute1 == null)
                    {
                        for (int index = 0; index < customAttributes.Length; ++index)
                        {
                            EstDeviceAddressAttribute addressAttribute2 = (EstDeviceAddressAttribute)customAttributes[index];
                            if (addressAttribute2.DeviceType == (Type)null)
                            {
                                addressAttribute1 = addressAttribute2;
                                break;
                            }
                        }
                    }
                    if (addressAttribute1 != null)
                    {
                        Type propertyType = property.PropertyType;
                        if (propertyType == typeof(short))
                        {
                            OperateResult<short> operateResult = readWrite.ReadInt16(addressAttribute1.Address);
                            if (!operateResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)operateResult);
                            property.SetValue(instance, (object)operateResult.Content, (object[])null);
                        }
                        else if (propertyType == typeof(short[]))
                        {
                            OperateResult<short[]> operateResult = readWrite.ReadInt16(addressAttribute1.Address, addressAttribute1.Length > 0 ? (ushort)addressAttribute1.Length : (ushort)1);
                            if (!operateResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)operateResult);
                            property.SetValue(instance, (object)operateResult.Content, (object[])null);
                        }
                        else if (propertyType == typeof(ushort))
                        {
                            OperateResult<ushort> operateResult = readWrite.ReadUInt16(addressAttribute1.Address);
                            if (!operateResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)operateResult);
                            property.SetValue(instance, (object)operateResult.Content, (object[])null);
                        }
                        else if (propertyType == typeof(ushort[]))
                        {
                            OperateResult<ushort[]> operateResult = readWrite.ReadUInt16(addressAttribute1.Address, addressAttribute1.Length > 0 ? (ushort)addressAttribute1.Length : (ushort)1);
                            if (!operateResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)operateResult);
                            property.SetValue(instance, (object)operateResult.Content, (object[])null);
                        }
                        else if (propertyType == typeof(int))
                        {
                            OperateResult<int> operateResult = readWrite.ReadInt32(addressAttribute1.Address);
                            if (!operateResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)operateResult);
                            property.SetValue(instance, (object)operateResult.Content, (object[])null);
                        }
                        else if (propertyType == typeof(int[]))
                        {
                            OperateResult<int[]> operateResult = readWrite.ReadInt32(addressAttribute1.Address, addressAttribute1.Length > 0 ? (ushort)addressAttribute1.Length : (ushort)1);
                            if (!operateResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)operateResult);
                            property.SetValue(instance, (object)operateResult.Content, (object[])null);
                        }
                        else if (propertyType == typeof(uint))
                        {
                            OperateResult<uint> operateResult = readWrite.ReadUInt32(addressAttribute1.Address);
                            if (!operateResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)operateResult);
                            property.SetValue(instance, (object)operateResult.Content, (object[])null);
                        }
                        else if (propertyType == typeof(uint[]))
                        {
                            OperateResult<uint[]> operateResult = readWrite.ReadUInt32(addressAttribute1.Address, addressAttribute1.Length > 0 ? (ushort)addressAttribute1.Length : (ushort)1);
                            if (!operateResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)operateResult);
                            property.SetValue(instance, (object)operateResult.Content, (object[])null);
                        }
                        else if (propertyType == typeof(long))
                        {
                            OperateResult<long> operateResult = readWrite.ReadInt64(addressAttribute1.Address);
                            if (!operateResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)operateResult);
                            property.SetValue(instance, (object)operateResult.Content, (object[])null);
                        }
                        else if (propertyType == typeof(long[]))
                        {
                            OperateResult<long[]> operateResult = readWrite.ReadInt64(addressAttribute1.Address, addressAttribute1.Length > 0 ? (ushort)addressAttribute1.Length : (ushort)1);
                            if (!operateResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)operateResult);
                            property.SetValue(instance, (object)operateResult.Content, (object[])null);
                        }
                        else if (propertyType == typeof(ulong))
                        {
                            OperateResult<ulong> operateResult = readWrite.ReadUInt64(addressAttribute1.Address);
                            if (!operateResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)operateResult);
                            property.SetValue(instance, (object)operateResult.Content, (object[])null);
                        }
                        else if (propertyType == typeof(ulong[]))
                        {
                            OperateResult<ulong[]> operateResult = readWrite.ReadUInt64(addressAttribute1.Address, addressAttribute1.Length > 0 ? (ushort)addressAttribute1.Length : (ushort)1);
                            if (!operateResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)operateResult);
                            property.SetValue(instance, (object)operateResult.Content, (object[])null);
                        }
                        else if (propertyType == typeof(float))
                        {
                            OperateResult<float> operateResult = readWrite.ReadFloat(addressAttribute1.Address);
                            if (!operateResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)operateResult);
                            property.SetValue(instance, (object)operateResult.Content, (object[])null);
                        }
                        else if (propertyType == typeof(float[]))
                        {
                            OperateResult<float[]> operateResult = readWrite.ReadFloat(addressAttribute1.Address, addressAttribute1.Length > 0 ? (ushort)addressAttribute1.Length : (ushort)1);
                            if (!operateResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)operateResult);
                            property.SetValue(instance, (object)operateResult.Content, (object[])null);
                        }
                        else if (propertyType == typeof(double))
                        {
                            OperateResult<double> operateResult = readWrite.ReadDouble(addressAttribute1.Address);
                            if (!operateResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)operateResult);
                            property.SetValue(instance, (object)operateResult.Content, (object[])null);
                        }
                        else if (propertyType == typeof(double[]))
                        {
                            OperateResult<double[]> operateResult = readWrite.ReadDouble(addressAttribute1.Address, addressAttribute1.Length > 0 ? (ushort)addressAttribute1.Length : (ushort)1);
                            if (!operateResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)operateResult);
                            property.SetValue(instance, (object)operateResult.Content, (object[])null);
                        }
                        else if (propertyType == typeof(string))
                        {
                            OperateResult<string> operateResult = readWrite.ReadString(addressAttribute1.Address, addressAttribute1.Length > 0 ? (ushort)addressAttribute1.Length : (ushort)1);
                            if (!operateResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)operateResult);
                            property.SetValue(instance, (object)operateResult.Content, (object[])null);
                        }
                        else if (propertyType == typeof(byte[]))
                        {
                            OperateResult<byte[]> operateResult = readWrite.Read(addressAttribute1.Address, addressAttribute1.Length > 0 ? (ushort)addressAttribute1.Length : (ushort)1);
                            if (!operateResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)operateResult);
                            property.SetValue(instance, (object)operateResult.Content, (object[])null);
                        }
                        else if (propertyType == typeof(bool))
                        {
                            OperateResult<bool> operateResult = readWrite.ReadBool(addressAttribute1.Address);
                            if (!operateResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)operateResult);
                            property.SetValue(instance, (object)operateResult.Content, (object[])null);
                        }
                        else if (propertyType == typeof(bool[]))
                        {
                            OperateResult<bool[]> operateResult = readWrite.ReadBool(addressAttribute1.Address, addressAttribute1.Length > 0 ? (ushort)addressAttribute1.Length : (ushort)1);
                            if (!operateResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)operateResult);
                            property.SetValue(instance, (object)operateResult.Content, (object[])null);
                        }
                    }
                }
            }
            return OperateResult.CreateSuccessResult<T>((T)instance);
        }

        /// <summary>
        /// 从设备里读取支持Est特性的数据内容，该特性为<see cref="T:ESTCore.Common.Reflection.EstDeviceAddressAttribute" />，详细参考论坛的操作说明。
        /// </summary>
        /// <typeparam name="T">自定义的数据类型对象</typeparam>
        /// <param name="data">自定义的数据对象</param>
        /// <param name="readWrite">数据读写对象</param>
        /// <returns>包含是否成功的结果对象</returns>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        public static OperateResult Write<T>(T data, IReadWriteNet readWrite) where T : class, new()
        {
            if ((object)data == null)
                throw new ArgumentNullException(nameof(data));
            Type type = typeof(T);
            T obj = data;
            foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                object[] customAttributes = property.GetCustomAttributes(typeof(EstDeviceAddressAttribute), false);
                if (customAttributes != null)
                {
                    EstDeviceAddressAttribute addressAttribute1 = (EstDeviceAddressAttribute)null;
                    for (int index = 0; index < customAttributes.Length; ++index)
                    {
                        EstDeviceAddressAttribute addressAttribute2 = (EstDeviceAddressAttribute)customAttributes[index];
                        if (addressAttribute2.DeviceType != (Type)null && addressAttribute2.DeviceType == readWrite.GetType())
                        {
                            addressAttribute1 = addressAttribute2;
                            break;
                        }
                    }
                    if (addressAttribute1 == null)
                    {
                        for (int index = 0; index < customAttributes.Length; ++index)
                        {
                            EstDeviceAddressAttribute addressAttribute2 = (EstDeviceAddressAttribute)customAttributes[index];
                            if (addressAttribute2.DeviceType == (Type)null)
                            {
                                addressAttribute1 = addressAttribute2;
                                break;
                            }
                        }
                    }
                    if (addressAttribute1 != null)
                    {
                        Type propertyType = property.PropertyType;
                        if (propertyType == typeof(short))
                        {
                            short num = (short)property.GetValue((object)obj, (object[])null);
                            OperateResult operateResult = readWrite.Write(addressAttribute1.Address, num);
                            if (!operateResult.IsSuccess)
                                return operateResult;
                        }
                        else if (propertyType == typeof(short[]))
                        {
                            short[] values = (short[])property.GetValue((object)obj, (object[])null);
                            OperateResult operateResult = readWrite.Write(addressAttribute1.Address, values);
                            if (!operateResult.IsSuccess)
                                return operateResult;
                        }
                        else if (propertyType == typeof(ushort))
                        {
                            ushort num = (ushort)property.GetValue((object)obj, (object[])null);
                            OperateResult operateResult = readWrite.Write(addressAttribute1.Address, num);
                            if (!operateResult.IsSuccess)
                                return operateResult;
                        }
                        else if (propertyType == typeof(ushort[]))
                        {
                            ushort[] values = (ushort[])property.GetValue((object)obj, (object[])null);
                            OperateResult operateResult = readWrite.Write(addressAttribute1.Address, values);
                            if (!operateResult.IsSuccess)
                                return operateResult;
                        }
                        else if (propertyType == typeof(int))
                        {
                            int num = (int)property.GetValue((object)obj, (object[])null);
                            OperateResult operateResult = readWrite.Write(addressAttribute1.Address, num);
                            if (!operateResult.IsSuccess)
                                return operateResult;
                        }
                        else if (propertyType == typeof(int[]))
                        {
                            int[] values = (int[])property.GetValue((object)obj, (object[])null);
                            OperateResult operateResult = readWrite.Write(addressAttribute1.Address, values);
                            if (!operateResult.IsSuccess)
                                return operateResult;
                        }
                        else if (propertyType == typeof(uint))
                        {
                            uint num = (uint)property.GetValue((object)obj, (object[])null);
                            OperateResult operateResult = readWrite.Write(addressAttribute1.Address, num);
                            if (!operateResult.IsSuccess)
                                return operateResult;
                        }
                        else if (propertyType == typeof(uint[]))
                        {
                            uint[] values = (uint[])property.GetValue((object)obj, (object[])null);
                            OperateResult operateResult = readWrite.Write(addressAttribute1.Address, values);
                            if (!operateResult.IsSuccess)
                                return operateResult;
                        }
                        else if (propertyType == typeof(long))
                        {
                            long num = (long)property.GetValue((object)obj, (object[])null);
                            OperateResult operateResult = readWrite.Write(addressAttribute1.Address, num);
                            if (!operateResult.IsSuccess)
                                return operateResult;
                        }
                        else if (propertyType == typeof(long[]))
                        {
                            long[] values = (long[])property.GetValue((object)obj, (object[])null);
                            OperateResult operateResult = readWrite.Write(addressAttribute1.Address, values);
                            if (!operateResult.IsSuccess)
                                return operateResult;
                        }
                        else if (propertyType == typeof(ulong))
                        {
                            ulong num = (ulong)property.GetValue((object)obj, (object[])null);
                            OperateResult operateResult = readWrite.Write(addressAttribute1.Address, num);
                            if (!operateResult.IsSuccess)
                                return operateResult;
                        }
                        else if (propertyType == typeof(ulong[]))
                        {
                            ulong[] values = (ulong[])property.GetValue((object)obj, (object[])null);
                            OperateResult operateResult = readWrite.Write(addressAttribute1.Address, values);
                            if (!operateResult.IsSuccess)
                                return operateResult;
                        }
                        else if (propertyType == typeof(float))
                        {
                            float num = (float)property.GetValue((object)obj, (object[])null);
                            OperateResult operateResult = readWrite.Write(addressAttribute1.Address, num);
                            if (!operateResult.IsSuccess)
                                return operateResult;
                        }
                        else if (propertyType == typeof(float[]))
                        {
                            float[] values = (float[])property.GetValue((object)obj, (object[])null);
                            OperateResult operateResult = readWrite.Write(addressAttribute1.Address, values);
                            if (!operateResult.IsSuccess)
                                return operateResult;
                        }
                        else if (propertyType == typeof(double))
                        {
                            double num = (double)property.GetValue((object)obj, (object[])null);
                            OperateResult operateResult = readWrite.Write(addressAttribute1.Address, num);
                            if (!operateResult.IsSuccess)
                                return operateResult;
                        }
                        else if (propertyType == typeof(double[]))
                        {
                            double[] values = (double[])property.GetValue((object)obj, (object[])null);
                            OperateResult operateResult = readWrite.Write(addressAttribute1.Address, values);
                            if (!operateResult.IsSuccess)
                                return operateResult;
                        }
                        else if (propertyType == typeof(string))
                        {
                            string str = (string)property.GetValue((object)obj, (object[])null);
                            OperateResult operateResult = readWrite.Write(addressAttribute1.Address, str);
                            if (!operateResult.IsSuccess)
                                return operateResult;
                        }
                        else if (propertyType == typeof(byte[]))
                        {
                            byte[] numArray = (byte[])property.GetValue((object)obj, (object[])null);
                            OperateResult operateResult = readWrite.Write(addressAttribute1.Address, numArray);
                            if (!operateResult.IsSuccess)
                                return operateResult;
                        }
                        else if (propertyType == typeof(bool))
                        {
                            bool flag = (bool)property.GetValue((object)obj, (object[])null);
                            OperateResult operateResult = readWrite.Write(addressAttribute1.Address, flag);
                            if (!operateResult.IsSuccess)
                                return operateResult;
                        }
                        else if (propertyType == typeof(bool[]))
                        {
                            bool[] flagArray = (bool[])property.GetValue((object)obj, (object[])null);
                            OperateResult operateResult = readWrite.Write(addressAttribute1.Address, flagArray);
                            if (!operateResult.IsSuccess)
                                return operateResult;
                        }
                    }
                }
            }
            return (OperateResult)OperateResult.CreateSuccessResult<T>(obj);
        }

        /// <summary>使用表达式树的方式来给一个属性赋值</summary>
        /// <param name="propertyInfo">属性信息</param>
        /// <param name="obj">对象信息</param>
        /// <param name="objValue">实际的值</param>
        public static void SetPropertyExp<T, K>(PropertyInfo propertyInfo, T obj, K objValue)
        {
            ParameterExpression parameterExpression1 = Expression.Parameter(typeof(T), nameof(obj));
            ParameterExpression parameterExpression2 = Expression.Parameter(propertyInfo.PropertyType, nameof(objValue));
            Expression.Lambda<Action<T, K>>((Expression)Expression.Call((Expression)parameterExpression1, propertyInfo.GetSetMethod(), (Expression)parameterExpression2), parameterExpression1, parameterExpression2).Compile()(obj, objValue);
        }

        /// <summary>
        /// 从设备里读取支持Est特性的数据内容，该特性为<see cref="T:ESTCore.Common.Reflection.EstDeviceAddressAttribute" />，详细参考论坛的操作说明。
        /// </summary>
        /// <typeparam name="T">自定义的数据类型对象</typeparam>
        /// <param name="readWrite">读写接口的实现</param>
        /// <returns>包含是否成功的结果对象</returns>
        public static async Task<OperateResult<T>> ReadAsync<T>(IReadWriteNet readWrite) where T : class, new()
        {
            Type type = typeof(T);
            object obj = type.Assembly.CreateInstance(type.FullName);
            PropertyInfo[] propertyInfoArray = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            for (int index = 0; index < propertyInfoArray.Length; ++index)
            {
                PropertyInfo property = propertyInfoArray[index];
                object[] attribute = property.GetCustomAttributes(typeof(EstDeviceAddressAttribute), false);
                if (attribute != null)
                {
                    EstDeviceAddressAttribute hslAttribute = (EstDeviceAddressAttribute)null;
                    for (int i = 0; i < attribute.Length; ++i)
                    {
                        EstDeviceAddressAttribute tmp = (EstDeviceAddressAttribute)attribute[i];
                        if (tmp.DeviceType != (Type)null && tmp.DeviceType == readWrite.GetType())
                        {
                            hslAttribute = tmp;
                            break;
                        }
                        tmp = (EstDeviceAddressAttribute)null;
                    }
                    if (hslAttribute == null)
                    {
                        for (int i = 0; i < attribute.Length; ++i)
                        {
                            EstDeviceAddressAttribute tmp = (EstDeviceAddressAttribute)attribute[i];
                            if (tmp.DeviceType == (Type)null)
                            {
                                hslAttribute = tmp;
                                break;
                            }
                            tmp = (EstDeviceAddressAttribute)null;
                        }
                    }
                    if (hslAttribute != null)
                    {
                        Type propertyType = property.PropertyType;
                        if (propertyType == typeof(short))
                        {
                            OperateResult<short> valueResult = await readWrite.ReadInt16Async(hslAttribute.Address);
                            if (!valueResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)valueResult);
                            property.SetValue(obj, (object)valueResult.Content, (object[])null);
                            valueResult = (OperateResult<short>)null;
                        }
                        else if (propertyType == typeof(short[]))
                        {
                            OperateResult<short[]> valueResult = await readWrite.ReadInt16Async(hslAttribute.Address, hslAttribute.Length > 0 ? (ushort)hslAttribute.Length : (ushort)1);
                            if (!valueResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)valueResult);
                            property.SetValue(obj, (object)valueResult.Content, (object[])null);
                            valueResult = (OperateResult<short[]>)null;
                        }
                        else if (propertyType == typeof(ushort))
                        {
                            OperateResult<ushort> valueResult = await readWrite.ReadUInt16Async(hslAttribute.Address);
                            if (!valueResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)valueResult);
                            property.SetValue(obj, (object)valueResult.Content, (object[])null);
                            valueResult = (OperateResult<ushort>)null;
                        }
                        else if (propertyType == typeof(ushort[]))
                        {
                            OperateResult<ushort[]> valueResult = await readWrite.ReadUInt16Async(hslAttribute.Address, hslAttribute.Length > 0 ? (ushort)hslAttribute.Length : (ushort)1);
                            if (!valueResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)valueResult);
                            property.SetValue(obj, (object)valueResult.Content, (object[])null);
                            valueResult = (OperateResult<ushort[]>)null;
                        }
                        else if (propertyType == typeof(int))
                        {
                            OperateResult<int> valueResult = await readWrite.ReadInt32Async(hslAttribute.Address);
                            if (!valueResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)valueResult);
                            property.SetValue(obj, (object)valueResult.Content, (object[])null);
                            valueResult = (OperateResult<int>)null;
                        }
                        else if (propertyType == typeof(int[]))
                        {
                            OperateResult<int[]> valueResult = await readWrite.ReadInt32Async(hslAttribute.Address, hslAttribute.Length > 0 ? (ushort)hslAttribute.Length : (ushort)1);
                            if (!valueResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)valueResult);
                            property.SetValue(obj, (object)valueResult.Content, (object[])null);
                            valueResult = (OperateResult<int[]>)null;
                        }
                        else if (propertyType == typeof(uint))
                        {
                            OperateResult<uint> valueResult = await readWrite.ReadUInt32Async(hslAttribute.Address);
                            if (!valueResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)valueResult);
                            property.SetValue(obj, (object)valueResult.Content, (object[])null);
                            valueResult = (OperateResult<uint>)null;
                        }
                        else if (propertyType == typeof(uint[]))
                        {
                            OperateResult<uint[]> valueResult = await readWrite.ReadUInt32Async(hslAttribute.Address, hslAttribute.Length > 0 ? (ushort)hslAttribute.Length : (ushort)1);
                            if (!valueResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)valueResult);
                            property.SetValue(obj, (object)valueResult.Content, (object[])null);
                            valueResult = (OperateResult<uint[]>)null;
                        }
                        else if (propertyType == typeof(long))
                        {
                            OperateResult<long> valueResult = await readWrite.ReadInt64Async(hslAttribute.Address);
                            if (!valueResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)valueResult);
                            property.SetValue(obj, (object)valueResult.Content, (object[])null);
                            valueResult = (OperateResult<long>)null;
                        }
                        else if (propertyType == typeof(long[]))
                        {
                            OperateResult<long[]> valueResult = await readWrite.ReadInt64Async(hslAttribute.Address, hslAttribute.Length > 0 ? (ushort)hslAttribute.Length : (ushort)1);
                            if (!valueResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)valueResult);
                            property.SetValue(obj, (object)valueResult.Content, (object[])null);
                            valueResult = (OperateResult<long[]>)null;
                        }
                        else if (propertyType == typeof(ulong))
                        {
                            OperateResult<ulong> valueResult = await readWrite.ReadUInt64Async(hslAttribute.Address);
                            if (!valueResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)valueResult);
                            property.SetValue(obj, (object)valueResult.Content, (object[])null);
                            valueResult = (OperateResult<ulong>)null;
                        }
                        else if (propertyType == typeof(ulong[]))
                        {
                            OperateResult<ulong[]> valueResult = await readWrite.ReadUInt64Async(hslAttribute.Address, hslAttribute.Length > 0 ? (ushort)hslAttribute.Length : (ushort)1);
                            if (!valueResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)valueResult);
                            property.SetValue(obj, (object)valueResult.Content, (object[])null);
                            valueResult = (OperateResult<ulong[]>)null;
                        }
                        else if (propertyType == typeof(float))
                        {
                            OperateResult<float> valueResult = await readWrite.ReadFloatAsync(hslAttribute.Address);
                            if (!valueResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)valueResult);
                            property.SetValue(obj, (object)valueResult.Content, (object[])null);
                            valueResult = (OperateResult<float>)null;
                        }
                        else if (propertyType == typeof(float[]))
                        {
                            OperateResult<float[]> valueResult = await readWrite.ReadFloatAsync(hslAttribute.Address, hslAttribute.Length > 0 ? (ushort)hslAttribute.Length : (ushort)1);
                            if (!valueResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)valueResult);
                            property.SetValue(obj, (object)valueResult.Content, (object[])null);
                            valueResult = (OperateResult<float[]>)null;
                        }
                        else if (propertyType == typeof(double))
                        {
                            OperateResult<double> valueResult = await readWrite.ReadDoubleAsync(hslAttribute.Address);
                            if (!valueResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)valueResult);
                            property.SetValue(obj, (object)valueResult.Content, (object[])null);
                            valueResult = (OperateResult<double>)null;
                        }
                        else if (propertyType == typeof(double[]))
                        {
                            OperateResult<double[]> valueResult = await readWrite.ReadDoubleAsync(hslAttribute.Address, hslAttribute.Length > 0 ? (ushort)hslAttribute.Length : (ushort)1);
                            if (!valueResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)valueResult);
                            property.SetValue(obj, (object)valueResult.Content, (object[])null);
                            valueResult = (OperateResult<double[]>)null;
                        }
                        else if (propertyType == typeof(string))
                        {
                            OperateResult<string> valueResult = await readWrite.ReadStringAsync(hslAttribute.Address, hslAttribute.Length > 0 ? (ushort)hslAttribute.Length : (ushort)1);
                            if (!valueResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)valueResult);
                            property.SetValue(obj, (object)valueResult.Content, (object[])null);
                            valueResult = (OperateResult<string>)null;
                        }
                        else if (propertyType == typeof(byte[]))
                        {
                            OperateResult<byte[]> valueResult = await readWrite.ReadAsync(hslAttribute.Address, hslAttribute.Length > 0 ? (ushort)hslAttribute.Length : (ushort)1);
                            if (!valueResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)valueResult);
                            property.SetValue(obj, (object)valueResult.Content, (object[])null);
                            valueResult = (OperateResult<byte[]>)null;
                        }
                        else if (propertyType == typeof(bool))
                        {
                            OperateResult<bool> valueResult = await readWrite.ReadBoolAsync(hslAttribute.Address);
                            if (!valueResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)valueResult);
                            property.SetValue(obj, (object)valueResult.Content, (object[])null);
                            valueResult = (OperateResult<bool>)null;
                        }
                        else if (propertyType == typeof(bool[]))
                        {
                            OperateResult<bool[]> valueResult = await readWrite.ReadBoolAsync(hslAttribute.Address, hslAttribute.Length > 0 ? (ushort)hslAttribute.Length : (ushort)1);
                            if (!valueResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)valueResult);
                            property.SetValue(obj, (object)valueResult.Content, (object[])null);
                            valueResult = (OperateResult<bool[]>)null;
                        }
                        attribute = (object[])null;
                        hslAttribute = (EstDeviceAddressAttribute)null;
                        propertyType = (Type)null;
                        property = (PropertyInfo)null;
                    }
                }
            }
            propertyInfoArray = (PropertyInfo[])null;
            return OperateResult.CreateSuccessResult<T>((T)obj);
        }

        /// <summary>
        /// 从设备里读取支持Est特性的数据内容，该特性为<see cref="T:ESTCore.Common.Reflection.EstDeviceAddressAttribute" />，详细参考论坛的操作说明。
        /// </summary>
        /// <typeparam name="T">自定义的数据类型对象</typeparam>
        /// <param name="data">自定义的数据对象</param>
        /// <param name="readWrite">数据读写对象</param>
        /// <returns>包含是否成功的结果对象</returns>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        public static async Task<OperateResult> WriteAsync<T>(
          T data,
          IReadWriteNet readWrite)
          where T : class, new()
        {
            if ((object)(T)data == null)
                throw new ArgumentNullException(nameof(data));
            Type type = typeof(T);
            T obj = data;
            PropertyInfo[] propertyInfoArray = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            for (int index = 0; index < propertyInfoArray.Length; ++index)
            {
                PropertyInfo property = propertyInfoArray[index];
                object[] attribute = property.GetCustomAttributes(typeof(EstDeviceAddressAttribute), false);
                if (attribute != null)
                {
                    EstDeviceAddressAttribute hslAttribute = (EstDeviceAddressAttribute)null;
                    for (int i = 0; i < attribute.Length; ++i)
                    {
                        EstDeviceAddressAttribute tmp = (EstDeviceAddressAttribute)attribute[i];
                        if (tmp.DeviceType != (Type)null && tmp.DeviceType == readWrite.GetType())
                        {
                            hslAttribute = tmp;
                            break;
                        }
                        tmp = (EstDeviceAddressAttribute)null;
                    }
                    if (hslAttribute == null)
                    {
                        for (int i = 0; i < attribute.Length; ++i)
                        {
                            EstDeviceAddressAttribute tmp = (EstDeviceAddressAttribute)attribute[i];
                            if (tmp.DeviceType == (Type)null)
                            {
                                hslAttribute = tmp;
                                break;
                            }
                            tmp = (EstDeviceAddressAttribute)null;
                        }
                    }
                    if (hslAttribute != null)
                    {
                        Type propertyType = property.PropertyType;
                        if (propertyType == typeof(short))
                        {
                            short value = (short)property.GetValue((object)obj, (object[])null);
                            OperateResult writeResult = await readWrite.WriteAsync(hslAttribute.Address, value);
                            if (!writeResult.IsSuccess)
                                return writeResult;
                            writeResult = (OperateResult)null;
                        }
                        else if (propertyType == typeof(short[]))
                        {
                            short[] value = (short[])property.GetValue((object)obj, (object[])null);
                            OperateResult writeResult = await readWrite.WriteAsync(hslAttribute.Address, value);
                            if (!writeResult.IsSuccess)
                                return writeResult;
                            value = (short[])null;
                            writeResult = (OperateResult)null;
                        }
                        else if (propertyType == typeof(ushort))
                        {
                            ushort value = (ushort)property.GetValue((object)obj, (object[])null);
                            OperateResult writeResult = await readWrite.WriteAsync(hslAttribute.Address, value);
                            if (!writeResult.IsSuccess)
                                return writeResult;
                            writeResult = (OperateResult)null;
                        }
                        else if (propertyType == typeof(ushort[]))
                        {
                            ushort[] value = (ushort[])property.GetValue((object)obj, (object[])null);
                            OperateResult writeResult = await readWrite.WriteAsync(hslAttribute.Address, value);
                            if (!writeResult.IsSuccess)
                                return writeResult;
                            value = (ushort[])null;
                            writeResult = (OperateResult)null;
                        }
                        else if (propertyType == typeof(int))
                        {
                            int value = (int)property.GetValue((object)obj, (object[])null);
                            OperateResult writeResult = await readWrite.WriteAsync(hslAttribute.Address, value);
                            if (!writeResult.IsSuccess)
                                return writeResult;
                            writeResult = (OperateResult)null;
                        }
                        else if (propertyType == typeof(int[]))
                        {
                            int[] value = (int[])property.GetValue((object)obj, (object[])null);
                            OperateResult writeResult = await readWrite.WriteAsync(hslAttribute.Address, value);
                            if (!writeResult.IsSuccess)
                                return writeResult;
                            value = (int[])null;
                            writeResult = (OperateResult)null;
                        }
                        else if (propertyType == typeof(uint))
                        {
                            uint value = (uint)property.GetValue((object)obj, (object[])null);
                            OperateResult writeResult = await readWrite.WriteAsync(hslAttribute.Address, value);
                            if (!writeResult.IsSuccess)
                                return writeResult;
                            writeResult = (OperateResult)null;
                        }
                        else if (propertyType == typeof(uint[]))
                        {
                            uint[] value = (uint[])property.GetValue((object)obj, (object[])null);
                            OperateResult writeResult = await readWrite.WriteAsync(hslAttribute.Address, value);
                            if (!writeResult.IsSuccess)
                                return writeResult;
                            value = (uint[])null;
                            writeResult = (OperateResult)null;
                        }
                        else if (propertyType == typeof(long))
                        {
                            long value = (long)property.GetValue((object)obj, (object[])null);
                            OperateResult writeResult = await readWrite.WriteAsync(hslAttribute.Address, value);
                            if (!writeResult.IsSuccess)
                                return writeResult;
                            writeResult = (OperateResult)null;
                        }
                        else if (propertyType == typeof(long[]))
                        {
                            long[] value = (long[])property.GetValue((object)obj, (object[])null);
                            OperateResult writeResult = await readWrite.WriteAsync(hslAttribute.Address, value);
                            if (!writeResult.IsSuccess)
                                return writeResult;
                            value = (long[])null;
                            writeResult = (OperateResult)null;
                        }
                        else if (propertyType == typeof(ulong))
                        {
                            ulong value = (ulong)property.GetValue((object)obj, (object[])null);
                            OperateResult writeResult = await readWrite.WriteAsync(hslAttribute.Address, value);
                            if (!writeResult.IsSuccess)
                                return writeResult;
                            writeResult = (OperateResult)null;
                        }
                        else if (propertyType == typeof(ulong[]))
                        {
                            ulong[] value = (ulong[])property.GetValue((object)obj, (object[])null);
                            OperateResult writeResult = await readWrite.WriteAsync(hslAttribute.Address, value);
                            if (!writeResult.IsSuccess)
                                return writeResult;
                            value = (ulong[])null;
                            writeResult = (OperateResult)null;
                        }
                        else if (propertyType == typeof(float))
                        {
                            float value = (float)property.GetValue((object)obj, (object[])null);
                            OperateResult writeResult = await readWrite.WriteAsync(hslAttribute.Address, value);
                            if (!writeResult.IsSuccess)
                                return writeResult;
                            writeResult = (OperateResult)null;
                        }
                        else if (propertyType == typeof(float[]))
                        {
                            float[] value = (float[])property.GetValue((object)obj, (object[])null);
                            OperateResult writeResult = await readWrite.WriteAsync(hslAttribute.Address, value);
                            if (!writeResult.IsSuccess)
                                return writeResult;
                            value = (float[])null;
                            writeResult = (OperateResult)null;
                        }
                        else if (propertyType == typeof(double))
                        {
                            double value = (double)property.GetValue((object)obj, (object[])null);
                            OperateResult writeResult = await readWrite.WriteAsync(hslAttribute.Address, value);
                            if (!writeResult.IsSuccess)
                                return writeResult;
                            writeResult = (OperateResult)null;
                        }
                        else if (propertyType == typeof(double[]))
                        {
                            double[] value = (double[])property.GetValue((object)obj, (object[])null);
                            OperateResult writeResult = await readWrite.WriteAsync(hslAttribute.Address, value);
                            if (!writeResult.IsSuccess)
                                return writeResult;
                            value = (double[])null;
                            writeResult = (OperateResult)null;
                        }
                        else if (propertyType == typeof(string))
                        {
                            string value = (string)property.GetValue((object)obj, (object[])null);
                            OperateResult writeResult = await readWrite.WriteAsync(hslAttribute.Address, value);
                            if (!writeResult.IsSuccess)
                                return writeResult;
                            value = (string)null;
                            writeResult = (OperateResult)null;
                        }
                        else if (propertyType == typeof(byte[]))
                        {
                            byte[] value = (byte[])property.GetValue((object)obj, (object[])null);
                            OperateResult writeResult = await readWrite.WriteAsync(hslAttribute.Address, value);
                            if (!writeResult.IsSuccess)
                                return writeResult;
                            value = (byte[])null;
                            writeResult = (OperateResult)null;
                        }
                        else if (propertyType == typeof(bool))
                        {
                            bool value = (bool)property.GetValue((object)obj, (object[])null);
                            OperateResult writeResult = await readWrite.WriteAsync(hslAttribute.Address, value);
                            if (!writeResult.IsSuccess)
                                return writeResult;
                            writeResult = (OperateResult)null;
                        }
                        else if (propertyType == typeof(bool[]))
                        {
                            bool[] value = (bool[])property.GetValue((object)obj, (object[])null);
                            OperateResult writeResult = await readWrite.WriteAsync(hslAttribute.Address, value);
                            if (!writeResult.IsSuccess)
                                return writeResult;
                            value = (bool[])null;
                            writeResult = (OperateResult)null;
                        }
                        attribute = (object[])null;
                        hslAttribute = (EstDeviceAddressAttribute)null;
                        propertyType = (Type)null;
                        property = (PropertyInfo)null;
                    }
                }
            }
            propertyInfoArray = (PropertyInfo[])null;
            return (OperateResult)OperateResult.CreateSuccessResult<T>(obj);
        }

        internal static void SetPropertyObjectValue(PropertyInfo property, object obj, string value)
        {
            Type propertyType = property.PropertyType;
            if (propertyType == typeof(short))
                property.SetValue(obj, (object)short.Parse(value), (object[])null);
            else if (propertyType == typeof(ushort))
                property.SetValue(obj, (object)ushort.Parse(value), (object[])null);
            else if (propertyType == typeof(int))
                property.SetValue(obj, (object)int.Parse(value), (object[])null);
            else if (propertyType == typeof(uint))
                property.SetValue(obj, (object)uint.Parse(value), (object[])null);
            else if (propertyType == typeof(long))
                property.SetValue(obj, (object)long.Parse(value), (object[])null);
            else if (propertyType == typeof(ulong))
                property.SetValue(obj, (object)ulong.Parse(value), (object[])null);
            else if (propertyType == typeof(float))
                property.SetValue(obj, (object)float.Parse(value), (object[])null);
            else if (propertyType == typeof(double))
                property.SetValue(obj, (object)double.Parse(value), (object[])null);
            else if (propertyType == typeof(string))
                property.SetValue(obj, (object)value, (object[])null);
            else if (propertyType == typeof(byte))
                property.SetValue(obj, (object)byte.Parse(value), (object[])null);
            else if (propertyType == typeof(bool))
                property.SetValue(obj, (object)bool.Parse(value), (object[])null);
            else
                property.SetValue(obj, (object)value, (object[])null);
        }

        internal static void SetPropertyObjectValueArray(
          PropertyInfo property,
          object obj,
          string[] values)
        {
            Type propertyType = property.PropertyType;
            if (propertyType == typeof(short[]))
                property.SetValue(obj, (object)((IEnumerable<string>)values).Select<string, short>((Func<string, short>)(m => short.Parse(m))).ToArray<short>(), (object[])null);
            else if (propertyType == typeof(List<short>))
                property.SetValue(obj, (object)((IEnumerable<string>)values).Select<string, short>((Func<string, short>)(m => short.Parse(m))).ToList<short>(), (object[])null);
            else if (propertyType == typeof(ushort[]))
                property.SetValue(obj, (object)((IEnumerable<string>)values).Select<string, ushort>((Func<string, ushort>)(m => ushort.Parse(m))).ToArray<ushort>(), (object[])null);
            else if (propertyType == typeof(List<ushort>))
                property.SetValue(obj, (object)((IEnumerable<string>)values).Select<string, ushort>((Func<string, ushort>)(m => ushort.Parse(m))).ToList<ushort>(), (object[])null);
            else if (propertyType == typeof(int[]))
                property.SetValue(obj, (object)((IEnumerable<string>)values).Select<string, int>((Func<string, int>)(m => int.Parse(m))).ToArray<int>(), (object[])null);
            else if (propertyType == typeof(List<int>))
                property.SetValue(obj, (object)((IEnumerable<string>)values).Select<string, int>((Func<string, int>)(m => int.Parse(m))).ToList<int>(), (object[])null);
            else if (propertyType == typeof(uint[]))
                property.SetValue(obj, (object)((IEnumerable<string>)values).Select<string, uint>((Func<string, uint>)(m => uint.Parse(m))).ToArray<uint>(), (object[])null);
            else if (propertyType == typeof(List<uint>))
                property.SetValue(obj, (object)((IEnumerable<string>)values).Select<string, uint>((Func<string, uint>)(m => uint.Parse(m))).ToList<uint>(), (object[])null);
            else if (propertyType == typeof(long[]))
                property.SetValue(obj, (object)((IEnumerable<string>)values).Select<string, long>((Func<string, long>)(m => long.Parse(m))).ToArray<long>(), (object[])null);
            else if (propertyType == typeof(List<long>))
                property.SetValue(obj, (object)((IEnumerable<string>)values).Select<string, long>((Func<string, long>)(m => long.Parse(m))).ToList<long>(), (object[])null);
            else if (propertyType == typeof(ulong[]))
                property.SetValue(obj, (object)((IEnumerable<string>)values).Select<string, ulong>((Func<string, ulong>)(m => ulong.Parse(m))).ToArray<ulong>(), (object[])null);
            else if (propertyType == typeof(List<ulong>))
                property.SetValue(obj, (object)((IEnumerable<string>)values).Select<string, ulong>((Func<string, ulong>)(m => ulong.Parse(m))).ToList<ulong>(), (object[])null);
            else if (propertyType == typeof(float[]))
                property.SetValue(obj, (object)((IEnumerable<string>)values).Select<string, float>((Func<string, float>)(m => float.Parse(m))).ToArray<float>(), (object[])null);
            else if (propertyType == typeof(List<float>))
                property.SetValue(obj, (object)((IEnumerable<string>)values).Select<string, float>((Func<string, float>)(m => float.Parse(m))).ToList<float>(), (object[])null);
            else if (propertyType == typeof(double[]))
                property.SetValue(obj, (object)((IEnumerable<string>)values).Select<string, double>((Func<string, double>)(m => double.Parse(m))).ToArray<double>(), (object[])null);
            else if (propertyType == typeof(double[]))
                property.SetValue(obj, (object)((IEnumerable<string>)values).Select<string, double>((Func<string, double>)(m => double.Parse(m))).ToList<double>(), (object[])null);
            else if (propertyType == typeof(string[]))
                property.SetValue(obj, (object)values, (object[])null);
            else if (propertyType == typeof(List<string>))
                property.SetValue(obj, (object)new List<string>((IEnumerable<string>)values), (object[])null);
            else if (propertyType == typeof(byte[]))
                property.SetValue(obj, (object)((IEnumerable<string>)values).Select<string, byte>((Func<string, byte>)(m => byte.Parse(m))).ToArray<byte>(), (object[])null);
            else if (propertyType == typeof(List<byte>))
                property.SetValue(obj, (object)((IEnumerable<string>)values).Select<string, byte>((Func<string, byte>)(m => byte.Parse(m))).ToList<byte>(), (object[])null);
            else if (propertyType == typeof(bool[]))
                property.SetValue(obj, (object)((IEnumerable<string>)values).Select<string, bool>((Func<string, bool>)(m => bool.Parse(m))).ToArray<bool>(), (object[])null);
            else if (propertyType == typeof(List<bool>))
                property.SetValue(obj, (object)((IEnumerable<string>)values).Select<string, bool>((Func<string, bool>)(m => bool.Parse(m))).ToList<bool>(), (object[])null);
            else
                property.SetValue(obj, (object)values, (object[])null);
        }

        /// <summary>
        /// 从设备里读取支持Est特性的数据内容，
        /// 该特性为<see cref="T:ESTCore.Common.Reflection.EstRedisKeyAttribute" />，<see cref="T:ESTCore.Common.Reflection.EstRedisListItemAttribute" />，
        /// <see cref="T:ESTCore.Common.Reflection.EstRedisListAttribute" />，<see cref="T:ESTCore.Common.Reflection.EstRedisHashFieldAttribute" />
        /// 详细参考代码示例的操作说明。
        /// </summary>
        /// <typeparam name="T">自定义的数据类型对象</typeparam>
        /// <param name="redis">Redis的数据对象</param>
        /// <returns>包含是否成功的结果对象</returns>
        public static OperateResult<T> Read<T>(RedisClient redis) where T : class, new()
        {
            Type type = typeof(T);
            object instance = type.Assembly.CreateInstance(type.FullName);
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            List<PropertyInfoKeyName> source1 = new List<PropertyInfoKeyName>();
            List<PropertyInfoHashKeyName> source2 = new List<PropertyInfoHashKeyName>();
            foreach (PropertyInfo property in properties)
            {
                object[] customAttributes1 = property.GetCustomAttributes(typeof(EstRedisKeyAttribute), false);
                if (customAttributes1 != null && (uint)customAttributes1.Length > 0U)
                {
                    EstRedisKeyAttribute redisKeyAttribute = (EstRedisKeyAttribute)customAttributes1[0];
                    source1.Add(new PropertyInfoKeyName(property, redisKeyAttribute.KeyName));
                }
                else
                {
                    object[] customAttributes2 = property.GetCustomAttributes(typeof(EstRedisListItemAttribute), false);
                    if (customAttributes2 != null && (uint)customAttributes2.Length > 0U)
                    {
                        EstRedisListItemAttribute listItemAttribute = (EstRedisListItemAttribute)customAttributes2[0];
                        OperateResult<string> operateResult = redis.ReadListByIndex(listItemAttribute.ListKey, listItemAttribute.Index);
                        if (!operateResult.IsSuccess)
                            return OperateResult.CreateFailedResult<T>((OperateResult)operateResult);
                        EstReflectionHelper.SetPropertyObjectValue(property, instance, operateResult.Content);
                    }
                    else
                    {
                        object[] customAttributes3 = property.GetCustomAttributes(typeof(EstRedisListAttribute), false);
                        if (customAttributes3 != null && (uint)customAttributes3.Length > 0U)
                        {
                            EstRedisListAttribute redisListAttribute = (EstRedisListAttribute)customAttributes3[0];
                            OperateResult<string[]> operateResult = redis.ListRange(redisListAttribute.ListKey, redisListAttribute.StartIndex, redisListAttribute.EndIndex);
                            if (!operateResult.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)operateResult);
                            EstReflectionHelper.SetPropertyObjectValueArray(property, instance, operateResult.Content);
                        }
                        else
                        {
                            object[] customAttributes4 = property.GetCustomAttributes(typeof(EstRedisHashFieldAttribute), false);
                            if (customAttributes4 != null && (uint)customAttributes4.Length > 0U)
                            {
                                EstRedisHashFieldAttribute hashFieldAttribute = (EstRedisHashFieldAttribute)customAttributes4[0];
                                source2.Add(new PropertyInfoHashKeyName(property, hashFieldAttribute.HaskKey, hashFieldAttribute.Field));
                            }
                        }
                    }
                }
            }
            if (source1.Count > 0)
            {
                OperateResult<string[]> operateResult = redis.ReadKey(source1.Select<PropertyInfoKeyName, string>((Func<PropertyInfoKeyName, string>)(m => m.KeyName)).ToArray<string>());
                if (!operateResult.IsSuccess)
                    return OperateResult.CreateFailedResult<T>((OperateResult)operateResult);
                for (int index = 0; index < source1.Count; ++index)
                    EstReflectionHelper.SetPropertyObjectValue(source1[index].PropertyInfo, instance, operateResult.Content[index]);
            }
            if (source2.Count > 0)
            {
                foreach (var data in source2.GroupBy<PropertyInfoHashKeyName, string>((Func<PropertyInfoHashKeyName, string>)(m => m.KeyName)).Select(g => new
                {
                    Key = g.Key,
                    Values = g.ToArray<PropertyInfoHashKeyName>()
                }))
                {
                    if (data.Values.Length == 1)
                    {
                        OperateResult<string> operateResult = redis.ReadHashKey(data.Key, data.Values[0].Field);
                        if (!operateResult.IsSuccess)
                            return OperateResult.CreateFailedResult<T>((OperateResult)operateResult);
                        EstReflectionHelper.SetPropertyObjectValue(data.Values[0].PropertyInfo, instance, operateResult.Content);
                    }
                    else
                    {
                        OperateResult<string[]> operateResult = redis.ReadHashKey(data.Key, ((IEnumerable<PropertyInfoHashKeyName>)data.Values).Select<PropertyInfoHashKeyName, string>((Func<PropertyInfoHashKeyName, string>)(m => m.Field)).ToArray<string>());
                        if (!operateResult.IsSuccess)
                            return OperateResult.CreateFailedResult<T>((OperateResult)operateResult);
                        for (int index = 0; index < data.Values.Length; ++index)
                            EstReflectionHelper.SetPropertyObjectValue(data.Values[index].PropertyInfo, instance, operateResult.Content[index]);
                    }
                }
            }
            return OperateResult.CreateSuccessResult<T>((T)instance);
        }

        /// <summary>
        /// 从设备里写入支持Est特性的数据内容，
        /// 该特性为<see cref="T:ESTCore.Common.Reflection.EstRedisKeyAttribute" /> ，<see cref="T:ESTCore.Common.Reflection.EstRedisHashFieldAttribute" />
        /// 需要注意的是写入并不支持<see cref="T:ESTCore.Common.Reflection.EstRedisListAttribute" />，<see cref="T:ESTCore.Common.Reflection.EstRedisListItemAttribute" />特性，详细参考代码示例的操作说明。
        /// </summary>
        /// <typeparam name="T">自定义的数据类型对象</typeparam>
        /// <param name="data">等待写入的数据参数</param>
        /// <param name="redis">Redis的数据对象</param>
        /// <returns>包含是否成功的结果对象</returns>
        public static OperateResult Write<T>(T data, RedisClient redis) where T : class, new()
        {
            Type type = typeof(T);
            T obj = data;
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            List<PropertyInfoKeyName> source1 = new List<PropertyInfoKeyName>();
            List<PropertyInfoHashKeyName> source2 = new List<PropertyInfoHashKeyName>();
            foreach (PropertyInfo property in properties)
            {
                object[] customAttributes1 = property.GetCustomAttributes(typeof(EstRedisKeyAttribute), false);
                if (customAttributes1 != null && (uint)customAttributes1.Length > 0U)
                {
                    EstRedisKeyAttribute redisKeyAttribute = (EstRedisKeyAttribute)customAttributes1[0];
                    source1.Add(new PropertyInfoKeyName(property, redisKeyAttribute.KeyName, property.GetValue((object)obj, (object[])null).ToString()));
                }
                else
                {
                    object[] customAttributes2 = property.GetCustomAttributes(typeof(EstRedisHashFieldAttribute), false);
                    if (customAttributes2 != null && (uint)customAttributes2.Length > 0U)
                    {
                        EstRedisHashFieldAttribute hashFieldAttribute = (EstRedisHashFieldAttribute)customAttributes2[0];
                        source2.Add(new PropertyInfoHashKeyName(property, hashFieldAttribute.HaskKey, hashFieldAttribute.Field, property.GetValue((object)obj, (object[])null).ToString()));
                    }
                }
            }
            if (source1.Count > 0)
            {
                OperateResult operateResult = redis.WriteKey(source1.Select<PropertyInfoKeyName, string>((Func<PropertyInfoKeyName, string>)(m => m.KeyName)).ToArray<string>(), source1.Select<PropertyInfoKeyName, string>((Func<PropertyInfoKeyName, string>)(m => m.Value)).ToArray<string>());
                if (!operateResult.IsSuccess)
                    return operateResult;
            }
            if (source2.Count > 0)
            {
                foreach (var data1 in source2.GroupBy<PropertyInfoHashKeyName, string>((Func<PropertyInfoHashKeyName, string>)(m => m.KeyName)).Select(g => new
                {
                    Key = g.Key,
                    Values = g.ToArray<PropertyInfoHashKeyName>()
                }))
                {
                    if (data1.Values.Length == 1)
                    {
                        OperateResult operateResult = (OperateResult)redis.WriteHashKey(data1.Key, data1.Values[0].Field, data1.Values[0].Value);
                        if (!operateResult.IsSuccess)
                            return operateResult;
                    }
                    else
                    {
                        OperateResult operateResult = redis.WriteHashKey(data1.Key, ((IEnumerable<PropertyInfoHashKeyName>)data1.Values).Select<PropertyInfoHashKeyName, string>((Func<PropertyInfoHashKeyName, string>)(m => m.Field)).ToArray<string>(), ((IEnumerable<PropertyInfoHashKeyName>)data1.Values).Select<PropertyInfoHashKeyName, string>((Func<PropertyInfoHashKeyName, string>)(m => m.Value)).ToArray<string>());
                        if (!operateResult.IsSuccess)
                            return operateResult;
                    }
                }
            }
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 从设备里读取支持Est特性的数据内容，
        /// 该特性为<see cref="T:ESTCore.Common.Reflection.EstRedisKeyAttribute" />，<see cref="T:ESTCore.Common.Reflection.EstRedisListItemAttribute" />，
        /// <see cref="T:ESTCore.Common.Reflection.EstRedisListAttribute" />，<see cref="T:ESTCore.Common.Reflection.EstRedisHashFieldAttribute" />
        /// 详细参考代码示例的操作说明。
        /// </summary>
        /// <typeparam name="T">自定义的数据类型对象</typeparam>
        /// <param name="redis">Redis的数据对象</param>
        /// <returns>包含是否成功的结果对象</returns>
        public static async Task<OperateResult<T>> ReadAsync<T>(RedisClient redis) where T : class, new()
        {
            Type type = typeof(T);
            object obj = type.Assembly.CreateInstance(type.FullName);
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            List<PropertyInfoKeyName> keyPropertyInfos = new List<PropertyInfoKeyName>();
            List<PropertyInfoHashKeyName> propertyInfoHashKeys = new List<PropertyInfoHashKeyName>();
            PropertyInfo[] propertyInfoArray = properties;
            for (int index = 0; index < propertyInfoArray.Length; ++index)
            {
                PropertyInfo property = propertyInfoArray[index];
                object[] attributes = property.GetCustomAttributes(typeof(EstRedisKeyAttribute), false);
                object[] objArray1 = attributes;
                if (objArray1 != null && (uint)objArray1.Length > 0U)
                {
                    EstRedisKeyAttribute attribute = (EstRedisKeyAttribute)attributes[0];
                    keyPropertyInfos.Add(new PropertyInfoKeyName(property, attribute.KeyName));
                }
                else
                {
                    attributes = property.GetCustomAttributes(typeof(EstRedisListItemAttribute), false);
                    object[] objArray2 = attributes;
                    if (objArray2 != null && (uint)objArray2.Length > 0U)
                    {
                        EstRedisListItemAttribute attribute = (EstRedisListItemAttribute)attributes[0];
                        OperateResult<string> read = await redis.ReadListByIndexAsync(attribute.ListKey, attribute.Index);
                        if (!read.IsSuccess)
                            return OperateResult.CreateFailedResult<T>((OperateResult)read);
                        EstReflectionHelper.SetPropertyObjectValue(property, obj, read.Content);
                    }
                    else
                    {
                        attributes = property.GetCustomAttributes(typeof(EstRedisListAttribute), false);
                        object[] objArray3 = attributes;
                        if (objArray3 != null && (uint)objArray3.Length > 0U)
                        {
                            EstRedisListAttribute attribute = (EstRedisListAttribute)attributes[0];
                            OperateResult<string[]> read = await redis.ListRangeAsync(attribute.ListKey, attribute.StartIndex, attribute.EndIndex);
                            if (!read.IsSuccess)
                                return OperateResult.CreateFailedResult<T>((OperateResult)read);
                            EstReflectionHelper.SetPropertyObjectValueArray(property, obj, read.Content);
                        }
                        else
                        {
                            attributes = property.GetCustomAttributes(typeof(EstRedisHashFieldAttribute), false);
                            object[] objArray4 = attributes;
                            if (objArray4 != null && (uint)objArray4.Length > 0U)
                            {
                                EstRedisHashFieldAttribute attribute = (EstRedisHashFieldAttribute)attributes[0];
                                propertyInfoHashKeys.Add(new PropertyInfoHashKeyName(property, attribute.HaskKey, attribute.Field));
                            }
                            else
                            {
                                attributes = (object[])null;
                                property = (PropertyInfo)null;
                            }
                        }
                    }
                }
            }
            propertyInfoArray = (PropertyInfo[])null;
            if (keyPropertyInfos.Count > 0)
            {
                OperateResult<string[]> readKeys = await redis.ReadKeyAsync(keyPropertyInfos.Select<PropertyInfoKeyName, string>((Func<PropertyInfoKeyName, string>)(m => m.KeyName)).ToArray<string>());
                if (!readKeys.IsSuccess)
                    return OperateResult.CreateFailedResult<T>((OperateResult)readKeys);
                for (int i = 0; i < keyPropertyInfos.Count; ++i)
                    EstReflectionHelper.SetPropertyObjectValue(keyPropertyInfos[i].PropertyInfo, obj, readKeys.Content[i]);
                readKeys = (OperateResult<string[]>)null;
            }
            if (propertyInfoHashKeys.Count > 0)
            {
                var tmp = propertyInfoHashKeys.GroupBy<PropertyInfoHashKeyName, string>((Func<PropertyInfoHashKeyName, string>)(m => m.KeyName)).Select(g => new
                {
                    Key = g.Key,
                    Values = g.ToArray<PropertyInfoHashKeyName>()
                });
                foreach (var data in tmp)
                {
                    var item = data;
                    if (item.Values.Length == 1)
                    {
                        OperateResult<string> readKey = await redis.ReadHashKeyAsync(item.Key, item.Values[0].Field);
                        if (!readKey.IsSuccess)
                            return OperateResult.CreateFailedResult<T>((OperateResult)readKey);
                        EstReflectionHelper.SetPropertyObjectValue(item.Values[0].PropertyInfo, obj, readKey.Content);
                        readKey = (OperateResult<string>)null;
                    }
                    else
                    {
                        OperateResult<string[]> readKeys = await redis.ReadHashKeyAsync(item.Key, ((IEnumerable<PropertyInfoHashKeyName>)item.Values).Select<PropertyInfoHashKeyName, string>((Func<PropertyInfoHashKeyName, string>)(m => m.Field)).ToArray<string>());
                        if (!readKeys.IsSuccess)
                            return OperateResult.CreateFailedResult<T>((OperateResult)readKeys);
                        for (int i = 0; i < item.Values.Length; ++i)
                            EstReflectionHelper.SetPropertyObjectValue(item.Values[i].PropertyInfo, obj, readKeys.Content[i]);
                        readKeys = (OperateResult<string[]>)null;
                    }
                    item = null;
                }
                tmp = null;
            }
            return OperateResult.CreateSuccessResult<T>((T)obj);
        }

        /// <summary>
        /// 从设备里写入支持Est特性的数据内容，
        /// 该特性为<see cref="T:ESTCore.Common.Reflection.EstRedisKeyAttribute" /> ，<see cref="T:ESTCore.Common.Reflection.EstRedisHashFieldAttribute" />
        /// 需要注意的是写入并不支持<see cref="T:ESTCore.Common.Reflection.EstRedisListAttribute" />，<see cref="T:ESTCore.Common.Reflection.EstRedisListItemAttribute" />特性，详细参考代码示例的操作说明。
        /// </summary>
        /// <typeparam name="T">自定义的数据类型对象</typeparam>
        /// <param name="data">等待写入的数据参数</param>
        /// <param name="redis">Redis的数据对象</param>
        /// <returns>包含是否成功的结果对象</returns>
        public static async Task<OperateResult> WriteAsync<T>(
          T data,
          RedisClient redis)
          where T : class, new()
        {
            Type type = typeof(T);
            T obj = data;
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            List<PropertyInfoKeyName> keyPropertyInfos = new List<PropertyInfoKeyName>();
            List<PropertyInfoHashKeyName> propertyInfoHashKeys = new List<PropertyInfoHashKeyName>();
            PropertyInfo[] propertyInfoArray = properties;
            for (int index = 0; index < propertyInfoArray.Length; ++index)
            {
                PropertyInfo property = propertyInfoArray[index];
                object[] attributes = property.GetCustomAttributes(typeof(EstRedisKeyAttribute), false);
                object[] objArray1 = attributes;
                if (objArray1 != null && (uint)objArray1.Length > 0U)
                {
                    EstRedisKeyAttribute attribute = (EstRedisKeyAttribute)attributes[0];
                    keyPropertyInfos.Add(new PropertyInfoKeyName(property, attribute.KeyName, property.GetValue((object)obj, (object[])null).ToString()));
                }
                else
                {
                    attributes = property.GetCustomAttributes(typeof(EstRedisHashFieldAttribute), false);
                    object[] objArray2 = attributes;
                    if (objArray2 != null && (uint)objArray2.Length > 0U)
                    {
                        EstRedisHashFieldAttribute attribute = (EstRedisHashFieldAttribute)attributes[0];
                        propertyInfoHashKeys.Add(new PropertyInfoHashKeyName(property, attribute.HaskKey, attribute.Field, property.GetValue((object)obj, (object[])null).ToString()));
                    }
                    else
                    {
                        attributes = (object[])null;
                        property = (PropertyInfo)null;
                    }
                }
            }
            propertyInfoArray = (PropertyInfo[])null;
            if (keyPropertyInfos.Count > 0)
            {
                OperateResult writeResult = await redis.WriteKeyAsync(keyPropertyInfos.Select<PropertyInfoKeyName, string>((Func<PropertyInfoKeyName, string>)(m => m.KeyName)).ToArray<string>(), keyPropertyInfos.Select<PropertyInfoKeyName, string>((Func<PropertyInfoKeyName, string>)(m => m.Value)).ToArray<string>());
                if (!writeResult.IsSuccess)
                    return writeResult;
                writeResult = (OperateResult)null;
            }
            if (propertyInfoHashKeys.Count > 0)
            {
                var tmp = propertyInfoHashKeys.GroupBy<PropertyInfoHashKeyName, string>((Func<PropertyInfoHashKeyName, string>)(m => m.KeyName)).Select(g => new
                {
                    Key = g.Key,
                    Values = g.ToArray<PropertyInfoHashKeyName>()
                });
                foreach (var data1 in tmp)
                {
                    var item = data1;
                    if (item.Values.Length == 1)
                    {
                        OperateResult<int> operateResult = await redis.WriteHashKeyAsync(item.Key, item.Values[0].Field, item.Values[0].Value);
                        OperateResult writeResult = (OperateResult)operateResult;
                        operateResult = (OperateResult<int>)null;
                        if (!writeResult.IsSuccess)
                            return writeResult;
                        writeResult = (OperateResult)null;
                    }
                    else
                    {
                        OperateResult writeResult = await redis.WriteHashKeyAsync(item.Key, ((IEnumerable<PropertyInfoHashKeyName>)item.Values).Select<PropertyInfoHashKeyName, string>((Func<PropertyInfoHashKeyName, string>)(m => m.Field)).ToArray<string>(), ((IEnumerable<PropertyInfoHashKeyName>)item.Values).Select<PropertyInfoHashKeyName, string>((Func<PropertyInfoHashKeyName, string>)(m => m.Value)).ToArray<string>());
                        if (!writeResult.IsSuccess)
                            return writeResult;
                        writeResult = (OperateResult)null;
                    }
                    item = null;
                }
                tmp = null;
            }
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 从Json数据里解析出真实的数据信息，根据方法参数列表的类型进行反解析，然后返回实际的数据数组<br />
        /// Analyze the real data information from the Json data, perform de-analysis according to the type of the method parameter list,
        /// and then return the actual data array
        /// </summary>
        /// <param name="parameters">提供的参数列表信息</param>
        /// <param name="json">参数变量信息</param>
        /// <returns>已经填好的实际数据的参数数组对象</returns>
        public static object[] GetParametersFromJson(ParameterInfo[] parameters, string json)
        {
            JObject jobject = string.IsNullOrEmpty(json) ? new JObject() : JObject.Parse(json);
            object[] objArray = new object[parameters.Length];
            for (int index = 0; index < parameters.Length; ++index)
                objArray[index] = !(parameters[index].ParameterType == typeof(byte)) ? (!(parameters[index].ParameterType == typeof(short)) ? (!(parameters[index].ParameterType == typeof(ushort)) ? (!(parameters[index].ParameterType == typeof(int)) ? (!(parameters[index].ParameterType == typeof(uint)) ? (!(parameters[index].ParameterType == typeof(long)) ? (!(parameters[index].ParameterType == typeof(ulong)) ? (!(parameters[index].ParameterType == typeof(double)) ? (!(parameters[index].ParameterType == typeof(float)) ? (!(parameters[index].ParameterType == typeof(bool)) ? (!(parameters[index].ParameterType == typeof(string)) ? (!(parameters[index].ParameterType == typeof(DateTime)) ? (!(parameters[index].ParameterType == typeof(byte[])) ? (!(parameters[index].ParameterType == typeof(short[])) ? (!(parameters[index].ParameterType == typeof(ushort[])) ? (!(parameters[index].ParameterType == typeof(int[])) ? (!(parameters[index].ParameterType == typeof(uint[])) ? (!(parameters[index].ParameterType == typeof(long[])) ? (!(parameters[index].ParameterType == typeof(ulong[])) ? (!(parameters[index].ParameterType == typeof(float[])) ? (!(parameters[index].ParameterType == typeof(double[])) ? (!(parameters[index].ParameterType == typeof(bool[])) ? (!(parameters[index].ParameterType == typeof(string[])) ? (!(parameters[index].ParameterType == typeof(DateTime[])) ? jobject[parameters[index].Name].ToObject(parameters[index].ParameterType) : (object)((IEnumerable<JToken>)jobject[parameters[index].Name].ToArray<JToken>()).Select<JToken, DateTime>((Func<JToken, DateTime>)(m => m.Value<DateTime>())).ToArray<DateTime>()) : (object)((IEnumerable<JToken>)jobject[parameters[index].Name].ToArray<JToken>()).Select<JToken, string>((Func<JToken, string>)(m => m.Value<string>())).ToArray<string>()) : (object)((IEnumerable<JToken>)jobject[parameters[index].Name].ToArray<JToken>()).Select<JToken, bool>((Func<JToken, bool>)(m => m.Value<bool>())).ToArray<bool>()) : (object)((IEnumerable<JToken>)jobject[parameters[index].Name].ToArray<JToken>()).Select<JToken, double>((Func<JToken, double>)(m => m.Value<double>())).ToArray<double>()) : (object)((IEnumerable<JToken>)jobject[parameters[index].Name].ToArray<JToken>()).Select<JToken, float>((Func<JToken, float>)(m => m.Value<float>())).ToArray<float>()) : (object)((IEnumerable<JToken>)jobject[parameters[index].Name].ToArray<JToken>()).Select<JToken, ulong>((Func<JToken, ulong>)(m => m.Value<ulong>())).ToArray<ulong>()) : (object)((IEnumerable<JToken>)jobject[parameters[index].Name].ToArray<JToken>()).Select<JToken, long>((Func<JToken, long>)(m => m.Value<long>())).ToArray<long>()) : (object)((IEnumerable<JToken>)jobject[parameters[index].Name].ToArray<JToken>()).Select<JToken, uint>((Func<JToken, uint>)(m => m.Value<uint>())).ToArray<uint>()) : (object)((IEnumerable<JToken>)jobject[parameters[index].Name].ToArray<JToken>()).Select<JToken, int>((Func<JToken, int>)(m => m.Value<int>())).ToArray<int>()) : (object)((IEnumerable<JToken>)jobject[parameters[index].Name].ToArray<JToken>()).Select<JToken, ushort>((Func<JToken, ushort>)(m => m.Value<ushort>())).ToArray<ushort>()) : (object)((IEnumerable<JToken>)jobject[parameters[index].Name].ToArray<JToken>()).Select<JToken, short>((Func<JToken, short>)(m => m.Value<short>())).ToArray<short>()) : (object)jobject.Value<string>((object)parameters[index].Name).ToHexBytes()) : (object)jobject.Value<DateTime>((object)parameters[index].Name)) : (object)jobject.Value<string>((object)parameters[index].Name)) : (object)jobject.Value<bool>((object)parameters[index].Name)) : (object)jobject.Value<float>((object)parameters[index].Name)) : (object)jobject.Value<double>((object)parameters[index].Name)) : (object)jobject.Value<ulong>((object)parameters[index].Name)) : (object)jobject.Value<long>((object)parameters[index].Name)) : (object)jobject.Value<uint>((object)parameters[index].Name)) : (object)jobject.Value<int>((object)parameters[index].Name)) : (object)jobject.Value<ushort>((object)parameters[index].Name)) : (object)jobject.Value<short>((object)parameters[index].Name)) : (object)jobject.Value<byte>((object)parameters[index].Name);
            return objArray;
        }

        /// <summary>
        /// 从url数据里解析出真实的数据信息，根据方法参数列表的类型进行反解析，然后返回实际的数据数组<br />
        /// Analyze the real data information from the url data, perform de-analysis according to the type of the method parameter list,
        /// and then return the actual data array
        /// </summary>
        /// <param name="parameters">提供的参数列表信息</param>
        /// <param name="url">参数变量信息</param>
        /// <returns>已经填好的实际数据的参数数组对象</returns>
        public static object[] GetParametersFromUrl(ParameterInfo[] parameters, string url)
        {
            if (url.IndexOf('?') > 0)
                url = url.Substring(url.IndexOf('?') + 1);
            string[] strArray = url.Split(new char[1] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, string> dictionary = new Dictionary<string, string>(strArray.Length);
            for (int index = 0; index < strArray.Length; ++index)
            {
                if (!string.IsNullOrEmpty(strArray[index]) && strArray[index].IndexOf('=') > 0)
                    dictionary.Add(strArray[index].Substring(0, strArray[index].IndexOf('=')).Trim(' '), strArray[index].Substring(strArray[index].IndexOf('=') + 1));
            }
            object[] objArray = new object[parameters.Length];
            for (int index = 0; index < parameters.Length; ++index)
                objArray[index] = !(parameters[index].ParameterType == typeof(byte)) ? (!(parameters[index].ParameterType == typeof(short)) ? (!(parameters[index].ParameterType == typeof(ushort)) ? (!(parameters[index].ParameterType == typeof(int)) ? (!(parameters[index].ParameterType == typeof(uint)) ? (!(parameters[index].ParameterType == typeof(long)) ? (!(parameters[index].ParameterType == typeof(ulong)) ? (!(parameters[index].ParameterType == typeof(double)) ? (!(parameters[index].ParameterType == typeof(float)) ? (!(parameters[index].ParameterType == typeof(bool)) ? (!(parameters[index].ParameterType == typeof(string)) ? (!(parameters[index].ParameterType == typeof(DateTime)) ? (!(parameters[index].ParameterType == typeof(byte[])) ? (!(parameters[index].ParameterType == typeof(short[])) ? (!(parameters[index].ParameterType == typeof(ushort[])) ? (!(parameters[index].ParameterType == typeof(int[])) ? (!(parameters[index].ParameterType == typeof(uint[])) ? (!(parameters[index].ParameterType == typeof(long[])) ? (!(parameters[index].ParameterType == typeof(ulong[])) ? (!(parameters[index].ParameterType == typeof(float[])) ? (!(parameters[index].ParameterType == typeof(double[])) ? (!(parameters[index].ParameterType == typeof(bool[])) ? (!(parameters[index].ParameterType == typeof(string[])) ? (!(parameters[index].ParameterType == typeof(DateTime[])) ? JToken.Parse(dictionary[parameters[index].Name]).ToObject(parameters[index].ParameterType) : (object)dictionary[parameters[index].Name].ToStringArray<DateTime>()) : (object)dictionary[parameters[index].Name].ToStringArray<string>()) : (object)dictionary[parameters[index].Name].ToStringArray<bool>()) : (object)dictionary[parameters[index].Name].ToStringArray<double>()) : (object)dictionary[parameters[index].Name].ToStringArray<float>()) : (object)dictionary[parameters[index].Name].ToStringArray<ulong>()) : (object)dictionary[parameters[index].Name].ToStringArray<long>()) : (object)dictionary[parameters[index].Name].ToStringArray<uint>()) : (object)dictionary[parameters[index].Name].ToStringArray<int>()) : (object)dictionary[parameters[index].Name].ToStringArray<ushort>()) : (object)dictionary[parameters[index].Name].ToStringArray<short>()) : (object)dictionary[parameters[index].Name].ToHexBytes()) : (object)DateTime.Parse(dictionary[parameters[index].Name])) : (object)dictionary[parameters[index].Name]) : (object)bool.Parse(dictionary[parameters[index].Name])) : (object)float.Parse(dictionary[parameters[index].Name])) : (object)double.Parse(dictionary[parameters[index].Name])) : (object)ulong.Parse(dictionary[parameters[index].Name])) : (object)long.Parse(dictionary[parameters[index].Name])) : (object)uint.Parse(dictionary[parameters[index].Name])) : (object)int.Parse(dictionary[parameters[index].Name])) : (object)ushort.Parse(dictionary[parameters[index].Name])) : (object)short.Parse(dictionary[parameters[index].Name])) : (object)byte.Parse(dictionary[parameters[index].Name]);
            return objArray;
        }

        /// <summary>
        /// 从方法的参数列表里，提取出实际的示例参数信息，返回一个json对象，注意：该数据是示例的数据，具体参数的限制参照服务器返回的数据声明。<br />
        /// From the parameter list of the method, extract the actual example parameter information, and return a json object. Note: The data is the example data,
        /// and the specific parameter restrictions refer to the data declaration returned by the server.
        /// </summary>
        /// <param name="method">当前需要解析的方法名称</param>
        /// <returns>当前的参数对象信息</returns>
        public static JObject GetParametersFromJson(MethodInfo method)
        {
            ParameterInfo[] parameters = method.GetParameters();
            JObject jobject1 = new JObject();
            for (int index = 0; index < parameters.Length; ++index)
            {
                if (parameters[index].ParameterType == typeof(byte))
                    jobject1.Add(parameters[index].Name, (JToken)new JValue(parameters[index].HasDefaultValue ? (long)(byte)parameters[index].DefaultValue : 0L));
                else if (parameters[index].ParameterType == typeof(short))
                    jobject1.Add(parameters[index].Name, (JToken)new JValue(parameters[index].HasDefaultValue ? (long)(short)parameters[index].DefaultValue : 0L));
                else if (parameters[index].ParameterType == typeof(ushort))
                    jobject1.Add(parameters[index].Name, (JToken)new JValue(parameters[index].HasDefaultValue ? (long)(ushort)parameters[index].DefaultValue : 0L));
                else if (parameters[index].ParameterType == typeof(int))
                    jobject1.Add(parameters[index].Name, (JToken)new JValue(parameters[index].HasDefaultValue ? (long)(int)parameters[index].DefaultValue : 0L));
                else if (parameters[index].ParameterType == typeof(uint))
                    jobject1.Add(parameters[index].Name, (JToken)new JValue(parameters[index].HasDefaultValue ? (long)(uint)parameters[index].DefaultValue : 0L));
                else if (parameters[index].ParameterType == typeof(long))
                    jobject1.Add(parameters[index].Name, (JToken)new JValue(parameters[index].HasDefaultValue ? (long)parameters[index].DefaultValue : 0L));
                else if (parameters[index].ParameterType == typeof(ulong))
                    jobject1.Add(parameters[index].Name, (JToken)new JValue(parameters[index].HasDefaultValue ? (ulong)parameters[index].DefaultValue : 0UL));
                else if (parameters[index].ParameterType == typeof(double))
                    jobject1.Add(parameters[index].Name, (JToken)new JValue(parameters[index].HasDefaultValue ? (double)parameters[index].DefaultValue : 0.0));
                else if (parameters[index].ParameterType == typeof(float))
                    jobject1.Add(parameters[index].Name, (JToken)new JValue(parameters[index].HasDefaultValue ? (float)parameters[index].DefaultValue : 0.0f));
                else if (parameters[index].ParameterType == typeof(bool))
                    jobject1.Add(parameters[index].Name, (JToken)new JValue(parameters[index].HasDefaultValue && (bool)parameters[index].DefaultValue));
                else if (parameters[index].ParameterType == typeof(string))
                    jobject1.Add(parameters[index].Name, (JToken)new JValue(parameters[index].HasDefaultValue ? (string)parameters[index].DefaultValue : ""));
                else if (parameters[index].ParameterType == typeof(DateTime))
                    jobject1.Add(parameters[index].Name, (JToken)new JValue(parameters[index].HasDefaultValue ? ((DateTime)parameters[index].DefaultValue).ToString("yyyy-MM-dd HH:mm:ss") : DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                else if (parameters[index].ParameterType == typeof(byte[]))
                    jobject1.Add(parameters[index].Name, (JToken)new JValue(parameters[index].HasDefaultValue ? ((byte[])parameters[index].DefaultValue).ToHexString() : "00 1A 2B 3C 4D"));
                else if (parameters[index].ParameterType == typeof(short[]))
                {
                    JObject jobject2 = jobject1;
                    string name = parameters[index].Name;
                    short[] numArray;
                    if (!parameters[index].HasDefaultValue)
                        numArray = new short[3]
                        {
              (short) 1,
              (short) 2,
              (short) 3
                        };
                    else
                        numArray = (short[])parameters[index].DefaultValue;
                    JArray jarray = new JArray((object)numArray);
                    jobject2.Add(name, (JToken)jarray);
                }
                else if (parameters[index].ParameterType == typeof(ushort[]))
                {
                    JObject jobject2 = jobject1;
                    string name = parameters[index].Name;
                    ushort[] numArray;
                    if (!parameters[index].HasDefaultValue)
                        numArray = new ushort[3]
                        {
              (ushort) 1,
              (ushort) 2,
              (ushort) 3
                        };
                    else
                        numArray = (ushort[])parameters[index].DefaultValue;
                    JArray jarray = new JArray((object)numArray);
                    jobject2.Add(name, (JToken)jarray);
                }
                else if (parameters[index].ParameterType == typeof(int[]))
                {
                    JObject jobject2 = jobject1;
                    string name = parameters[index].Name;
                    int[] numArray;
                    if (!parameters[index].HasDefaultValue)
                        numArray = new int[3] { 1, 2, 3 };
                    else
                        numArray = (int[])parameters[index].DefaultValue;
                    JArray jarray = new JArray((object)numArray);
                    jobject2.Add(name, (JToken)jarray);
                }
                else if (parameters[index].ParameterType == typeof(uint[]))
                {
                    JObject jobject2 = jobject1;
                    string name = parameters[index].Name;
                    uint[] numArray;
                    if (!parameters[index].HasDefaultValue)
                        numArray = new uint[3] { 1U, 2U, 3U };
                    else
                        numArray = (uint[])parameters[index].DefaultValue;
                    JArray jarray = new JArray((object)numArray);
                    jobject2.Add(name, (JToken)jarray);
                }
                else if (parameters[index].ParameterType == typeof(long[]))
                {
                    JObject jobject2 = jobject1;
                    string name = parameters[index].Name;
                    long[] numArray;
                    if (!parameters[index].HasDefaultValue)
                        numArray = new long[3] { 1L, 2L, 3L };
                    else
                        numArray = (long[])parameters[index].DefaultValue;
                    JArray jarray = new JArray((object)numArray);
                    jobject2.Add(name, (JToken)jarray);
                }
                else if (parameters[index].ParameterType == typeof(ulong[]))
                {
                    JObject jobject2 = jobject1;
                    string name = parameters[index].Name;
                    ulong[] numArray;
                    if (!parameters[index].HasDefaultValue)
                        numArray = new ulong[3] { 1UL, 2UL, 3UL };
                    else
                        numArray = (ulong[])parameters[index].DefaultValue;
                    JArray jarray = new JArray((object)numArray);
                    jobject2.Add(name, (JToken)jarray);
                }
                else if (parameters[index].ParameterType == typeof(float[]))
                {
                    JObject jobject2 = jobject1;
                    string name = parameters[index].Name;
                    float[] numArray;
                    if (!parameters[index].HasDefaultValue)
                        numArray = new float[3] { 1f, 2f, 3f };
                    else
                        numArray = (float[])parameters[index].DefaultValue;
                    JArray jarray = new JArray((object)numArray);
                    jobject2.Add(name, (JToken)jarray);
                }
                else if (parameters[index].ParameterType == typeof(double[]))
                {
                    JObject jobject2 = jobject1;
                    string name = parameters[index].Name;
                    double[] numArray;
                    if (!parameters[index].HasDefaultValue)
                        numArray = new double[3] { 1.0, 2.0, 3.0 };
                    else
                        numArray = (double[])parameters[index].DefaultValue;
                    JArray jarray = new JArray((object)numArray);
                    jobject2.Add(name, (JToken)jarray);
                }
                else if (parameters[index].ParameterType == typeof(bool[]))
                {
                    JObject jobject2 = jobject1;
                    string name = parameters[index].Name;
                    bool[] flagArray;
                    if (!parameters[index].HasDefaultValue)
                        flagArray = new bool[3] { true, false, false };
                    else
                        flagArray = (bool[])parameters[index].DefaultValue;
                    JArray jarray = new JArray((object)flagArray);
                    jobject2.Add(name, (JToken)jarray);
                }
                else if (parameters[index].ParameterType == typeof(string[]))
                {
                    JObject jobject2 = jobject1;
                    string name = parameters[index].Name;
                    string[] strArray;
                    if (!parameters[index].HasDefaultValue)
                        strArray = new string[3] { "1", "2", "3" };
                    else
                        strArray = (string[])parameters[index].DefaultValue;
                    JArray jarray = new JArray((object[])strArray);
                    jobject2.Add(name, (JToken)jarray);
                }
                else if (parameters[index].ParameterType == typeof(DateTime[]))
                {
                    JObject jobject2 = jobject1;
                    string name = parameters[index].Name;
                    string[] strArray;
                    if (!parameters[index].HasDefaultValue)
                        strArray = new string[1]
                        {
              DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                        };
                    else
                        strArray = ((IEnumerable<DateTime>)(DateTime[])parameters[index].DefaultValue).Select<DateTime, string>((Func<DateTime, string>)(m => m.ToString("yyyy-MM-dd HH:mm:ss"))).ToArray<string>();
                    JArray jarray = new JArray((object[])strArray);
                    jobject2.Add(name, (JToken)jarray);
                }
                else
                    jobject1.Add(parameters[index].Name, JToken.FromObject(parameters[index].HasDefaultValue ? parameters[index].DefaultValue : Activator.CreateInstance(parameters[index].ParameterType)));
            }
            return jobject1;
        }

        /// <summary>
        /// 将一个对象转换成 <see cref="T:ESTCore.Common.OperateResult`1" /> 的string 类型的对象，用于远程RPC的数据交互
        /// </summary>
        /// <param name="obj">自定义的对象</param>
        /// <returns>转换之后的结果对象</returns>
        public static OperateResult<string> GetOperateResultJsonFromObj(object obj)
        {
            if (!(obj is OperateResult operateResult))
                return OperateResult.CreateSuccessResult<string>(obj == null ? string.Empty : obj.ToJsonString());
            OperateResult<string> operateResult1 = new OperateResult<string>();
            operateResult1.IsSuccess = operateResult.IsSuccess;
            operateResult1.ErrorCode = operateResult.ErrorCode;
            operateResult1.Message = operateResult.Message;
            if (!operateResult.IsSuccess)
                return operateResult1;
            PropertyInfo property1 = obj.GetType().GetProperty("Content");
            if (property1 != (PropertyInfo)null)
            {
                object obj1 = property1.GetValue(obj, (object[])null);
                if (obj1 != null)
                    operateResult1.Content = obj1.ToJsonString();
                return operateResult1;
            }
            PropertyInfo property2 = obj.GetType().GetProperty("Content1");
            if (property2 == (PropertyInfo)null)
                return operateResult1;
            PropertyInfo property3 = obj.GetType().GetProperty("Content2");
            if (property3 == (PropertyInfo)null)
            {
                operateResult1.Content = new
                {
                    Content1 = property2.GetValue(obj, (object[])null)
                }.ToJsonString();
                return operateResult1;
            }
            PropertyInfo property4 = obj.GetType().GetProperty("Content3");
            if (property4 == (PropertyInfo)null)
            {
                operateResult1.Content = new
                {
                    Content1 = property2.GetValue(obj, (object[])null),
                    Content2 = property3.GetValue(obj, (object[])null)
                }.ToJsonString();
                return operateResult1;
            }
            PropertyInfo property5 = obj.GetType().GetProperty("Content4");
            if (property5 == (PropertyInfo)null)
            {
                operateResult1.Content = new
                {
                    Content1 = property2.GetValue(obj, (object[])null),
                    Content2 = property3.GetValue(obj, (object[])null),
                    Content3 = property4.GetValue(obj, (object[])null)
                }.ToJsonString();
                return operateResult1;
            }
            PropertyInfo property6 = obj.GetType().GetProperty("Content5");
            if (property6 == (PropertyInfo)null)
            {
                operateResult1.Content = new
                {
                    Content1 = property2.GetValue(obj, (object[])null),
                    Content2 = property3.GetValue(obj, (object[])null),
                    Content3 = property4.GetValue(obj, (object[])null),
                    Content4 = property5.GetValue(obj, (object[])null)
                }.ToJsonString();
                return operateResult1;
            }
            PropertyInfo property7 = obj.GetType().GetProperty("Content6");
            if (property7 == (PropertyInfo)null)
            {
                operateResult1.Content = new
                {
                    Content1 = property2.GetValue(obj, (object[])null),
                    Content2 = property3.GetValue(obj, (object[])null),
                    Content3 = property4.GetValue(obj, (object[])null),
                    Content4 = property5.GetValue(obj, (object[])null),
                    Content5 = property6.GetValue(obj, (object[])null)
                }.ToJsonString();
                return operateResult1;
            }
            PropertyInfo property8 = obj.GetType().GetProperty("Content7");
            if (property8 == (PropertyInfo)null)
            {
                operateResult1.Content = new
                {
                    Content1 = property2.GetValue(obj, (object[])null),
                    Content2 = property3.GetValue(obj, (object[])null),
                    Content3 = property4.GetValue(obj, (object[])null),
                    Content4 = property5.GetValue(obj, (object[])null),
                    Content5 = property6.GetValue(obj, (object[])null),
                    Content6 = property7.GetValue(obj, (object[])null)
                }.ToJsonString();
                return operateResult1;
            }
            PropertyInfo property9 = obj.GetType().GetProperty("Content8");
            if (property9 == (PropertyInfo)null)
            {
                operateResult1.Content = new
                {
                    Content1 = property2.GetValue(obj, (object[])null),
                    Content2 = property3.GetValue(obj, (object[])null),
                    Content3 = property4.GetValue(obj, (object[])null),
                    Content4 = property5.GetValue(obj, (object[])null),
                    Content5 = property6.GetValue(obj, (object[])null),
                    Content6 = property7.GetValue(obj, (object[])null),
                    Content7 = property8.GetValue(obj, (object[])null)
                }.ToJsonString();
                return operateResult1;
            }
            PropertyInfo property10 = obj.GetType().GetProperty("Content9");
            if (property10 == (PropertyInfo)null)
            {
                operateResult1.Content = new
                {
                    Content1 = property2.GetValue(obj, (object[])null),
                    Content2 = property3.GetValue(obj, (object[])null),
                    Content3 = property4.GetValue(obj, (object[])null),
                    Content4 = property5.GetValue(obj, (object[])null),
                    Content5 = property6.GetValue(obj, (object[])null),
                    Content6 = property7.GetValue(obj, (object[])null),
                    Content7 = property8.GetValue(obj, (object[])null),
                    Content8 = property9.GetValue(obj, (object[])null)
                }.ToJsonString();
                return operateResult1;
            }
            PropertyInfo property11 = obj.GetType().GetProperty("Content10");
            if (property11 == (PropertyInfo)null)
            {
                operateResult1.Content = new
                {
                    Content1 = property2.GetValue(obj, (object[])null),
                    Content2 = property3.GetValue(obj, (object[])null),
                    Content3 = property4.GetValue(obj, (object[])null),
                    Content4 = property5.GetValue(obj, (object[])null),
                    Content5 = property6.GetValue(obj, (object[])null),
                    Content6 = property7.GetValue(obj, (object[])null),
                    Content7 = property8.GetValue(obj, (object[])null),
                    Content8 = property9.GetValue(obj, (object[])null),
                    Content9 = property10.GetValue(obj, (object[])null)
                }.ToJsonString();
                return operateResult1;
            }
            operateResult1.Content = new
            {
                Content1 = property2.GetValue(obj, (object[])null),
                Content2 = property3.GetValue(obj, (object[])null),
                Content3 = property4.GetValue(obj, (object[])null),
                Content4 = property5.GetValue(obj, (object[])null),
                Content5 = property6.GetValue(obj, (object[])null),
                Content6 = property7.GetValue(obj, (object[])null),
                Content7 = property8.GetValue(obj, (object[])null),
                Content8 = property9.GetValue(obj, (object[])null),
                Content9 = property10.GetValue(obj, (object[])null),
                Content10 = property11.GetValue(obj, (object[])null)
            }.ToJsonString();
            return operateResult1;
        }
    }
}
