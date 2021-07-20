// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.MQTT.MqttHelper
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;
using ESTCore.Common.Reflection;

using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace ESTCore.Common.MQTT
{
    /// <summary>
    /// Mqtt协议的辅助类，提供了一些协议相关的基础方法，方便客户端和服务器端一起调用。<br />
    /// The auxiliary class of the Mqtt protocol provides some protocol-related basic methods for the client and server to call together.
    /// </summary>
    public class MqttHelper
    {
        /// <summary>
        /// 根据数据的总长度，计算出剩余的数据长度信息<br />
        /// According to the total length of the data, calculate the remaining data length information
        /// </summary>
        /// <param name="length">数据的总长度</param>
        /// <returns>计算结果</returns>
        public static OperateResult<byte[]> CalculateLengthToMqttLength(int length)
        {
            if (length > 268435455)
                return new OperateResult<byte[]>(StringResources.Language.MQTTDataTooLong);
            return length < 128 ? OperateResult.CreateSuccessResult<byte[]>(new byte[1]
            {
        (byte) length
            }) : (length < 16384 ? OperateResult.CreateSuccessResult<byte[]>(new byte[2]
            {
        (byte) (length % 128 + 128),
        (byte) (length / 128)
            }) : (length < 2097152 ? OperateResult.CreateSuccessResult<byte[]>(new byte[3]
            {
        (byte) (length % 128 + 128),
        (byte) (length / 128 % 128 + 128),
        (byte) (length / 128 / 128)
            }) : OperateResult.CreateSuccessResult<byte[]>(new byte[4]
            {
        (byte) (length % 128 + 128),
        (byte) (length / 128 % 128 + 128),
        (byte) (length / 128 / 128 % 128 + 128),
        (byte) (length / 128 / 128 / 128)
            })));
        }

        /// <summary>
        /// 将一个数据打包成一个mqtt协议的内容<br />
        /// Pack a piece of data into a mqtt protocol
        /// </summary>
        /// <param name="control">控制码</param>
        /// <param name="flags">标记</param>
        /// <param name="variableHeader">可变头的字节内容</param>
        /// <param name="payLoad">负载数据</param>
        /// <returns>带有是否成功的结果对象</returns>
        public static OperateResult<byte[]> BuildMqttCommand(
          byte control,
          byte flags,
          byte[] variableHeader,
          byte[] payLoad)
        {
            control <<= 4;
            return MqttHelper.BuildMqttCommand((byte)((uint)control | (uint)flags), variableHeader, payLoad);
        }

        /// <summary>
        /// 将一个数据打包成一个mqtt协议的内容<br />
        /// Pack a piece of data into a mqtt protocol
        /// </summary>
        /// <param name="head">控制码加标记码</param>
        /// <param name="variableHeader">可变头的字节内容</param>
        /// <param name="payLoad">负载数据</param>
        /// <returns>带有是否成功的结果对象</returns>
        public static OperateResult<byte[]> BuildMqttCommand(
          byte head,
          byte[] variableHeader,
          byte[] payLoad)
        {
            if (variableHeader == null)
                variableHeader = new byte[0];
            if (payLoad == null)
                payLoad = new byte[0];
            OperateResult<byte[]> lengthToMqttLength = MqttHelper.CalculateLengthToMqttLength(variableHeader.Length + payLoad.Length);
            if (!lengthToMqttLength.IsSuccess)
                return lengthToMqttLength;
            MemoryStream memoryStream = new MemoryStream();
            memoryStream.WriteByte(head);
            memoryStream.Write(lengthToMqttLength.Content, 0, lengthToMqttLength.Content.Length);
            if ((uint)variableHeader.Length > 0U)
                memoryStream.Write(variableHeader, 0, variableHeader.Length);
            if ((uint)payLoad.Length > 0U)
                memoryStream.Write(payLoad, 0, payLoad.Length);
            byte[] array = memoryStream.ToArray();
            memoryStream.Dispose();
            return OperateResult.CreateSuccessResult<byte[]>(array);
        }

        /// <summary>
        /// 将字符串打包成utf8编码，并且带有2个字节的表示长度的信息<br />
        /// Pack the string into utf8 encoding, and with 2 bytes of length information
        /// </summary>
        /// <param name="message">文本消息</param>
        /// <returns>打包之后的信息</returns>
        public static byte[] BuildSegCommandByString(string message)
        {
            byte[] numArray1 = string.IsNullOrEmpty(message) ? new byte[0] : Encoding.UTF8.GetBytes(message);
            byte[] numArray2 = new byte[numArray1.Length + 2];
            numArray1.CopyTo((Array)numArray2, 2);
            numArray2[0] = (byte)(numArray1.Length / 256);
            numArray2[1] = (byte)(numArray1.Length % 256);
            return numArray2;
        }

        /// <summary>
        /// 从MQTT的缓存信息里，提取文本信息<br />
        /// Extract text information from MQTT cache information
        /// </summary>
        /// <param name="buffer">Mqtt的报文</param>
        /// <param name="index">索引</param>
        /// <returns>值</returns>
        public static string ExtraMsgFromBytes(byte[] buffer, ref int index)
        {
            int num = index;
            int count = (int)buffer[index] * 256 + (int)buffer[index + 1];
            index = index + 2 + count;
            return Encoding.UTF8.GetString(buffer, num + 2, count);
        }

        /// <summary>
        /// 从MQTT的缓存信息里，提取文本信息<br />
        /// Extract text information from MQTT cache information
        /// </summary>
        /// <param name="buffer">Mqtt的报文</param>
        /// <param name="index">索引</param>
        /// <returns>值</returns>
        public static string ExtraSubscribeMsgFromBytes(byte[] buffer, ref int index)
        {
            int num = index;
            int count = (int)buffer[index] * 256 + (int)buffer[index + 1];
            index = index + 3 + count;
            return Encoding.UTF8.GetString(buffer, num + 2, count);
        }

        /// <summary>
        /// 从MQTT的缓存信息里，提取长度信息<br />
        /// Extract length information from MQTT cache information
        /// </summary>
        /// <param name="buffer">Mqtt的报文</param>
        /// <param name="index">索引</param>
        /// <returns>值</returns>
        public static int ExtraIntFromBytes(byte[] buffer, ref int index)
        {
            int num = (int)buffer[index] * 256 + (int)buffer[index + 1];
            index += 2;
            return num;
        }

        /// <summary>
        /// 从MQTT的缓存信息里，提取长度信息<br />
        /// Extract length information from MQTT cache information
        /// </summary>
        /// <param name="data">数据信息</param>
        /// <returns>值</returns>
        public static byte[] BuildIntBytes(int data) => new byte[2]
        {
      BitConverter.GetBytes(data)[1],
      BitConverter.GetBytes(data)[0]
        };

        /// <summary>
        /// 创建MQTT连接服务器的报文信息<br />
        /// Create MQTT connection server message information
        /// </summary>
        /// <param name="connectionOptions">连接配置</param>
        /// <param name="protocol">协议的内容</param>
        /// <returns>返回是否成功的信息</returns>
        public static OperateResult<byte[]> BuildConnectMqttCommand(
          MqttConnectionOptions connectionOptions,
          string protocol = "MQTT")
        {
            List<byte> byteList1 = new List<byte>();
            byteList1.AddRange((IEnumerable<byte>)new byte[2]
            {
        (byte) 0,
        (byte) 4
            });
            byteList1.AddRange((IEnumerable<byte>)Encoding.ASCII.GetBytes(protocol));
            byteList1.Add((byte)4);
            byte num = 0;
            if (connectionOptions.Credentials != null)
                num = (byte)((uint)(byte)((uint)num | 128U) | 64U);
            if (connectionOptions.CleanSession)
                num |= (byte)2;
            byteList1.Add(num);
            if (connectionOptions.KeepAlivePeriod.TotalSeconds < 1.0)
                connectionOptions.KeepAlivePeriod = TimeSpan.FromSeconds(1.0);
            byte[] bytes = BitConverter.GetBytes((int)connectionOptions.KeepAlivePeriod.TotalSeconds);
            byteList1.Add(bytes[1]);
            byteList1.Add(bytes[0]);
            List<byte> byteList2 = new List<byte>();
            byteList2.AddRange((IEnumerable<byte>)MqttHelper.BuildSegCommandByString(connectionOptions.ClientId));
            if (connectionOptions.Credentials != null)
            {
                byteList2.AddRange((IEnumerable<byte>)MqttHelper.BuildSegCommandByString(connectionOptions.Credentials.UserName));
                byteList2.AddRange((IEnumerable<byte>)MqttHelper.BuildSegCommandByString(connectionOptions.Credentials.Password));
            }
            return MqttHelper.BuildMqttCommand((byte)1, (byte)0, byteList1.ToArray(), byteList2.ToArray());
        }

        /// <summary>
        /// 根据服务器返回的信息判断当前的连接是否是可用的<br />
        /// According to the information returned by the server to determine whether the current connection is available
        /// </summary>
        /// <param name="code">功能码</param>
        /// <param name="data">数据内容</param>
        /// <returns>是否可用的连接</returns>
        public static OperateResult CheckConnectBack(byte code, byte[] data)
        {
            if ((int)code >> 4 != 2)
                return new OperateResult("MQTT Connection Back Is Wrong: " + code.ToString());
            if (data.Length < 2)
                return new OperateResult("MQTT Connection Data Is Short: " + SoftBasic.ByteToHexString(data, ' '));
            int num1 = (int)data[1];
            int num2 = (int)data[0];
            return num1 > 0 ? new OperateResult(num1, MqttHelper.GetMqttCodeText(num1)) : OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 获取当前的错误的描述信息<br />
        /// Get a description of the current error
        /// </summary>
        /// <param name="status">状态信息</param>
        /// <returns>描述信息</returns>
        public static string GetMqttCodeText(int status)
        {
            switch (status)
            {
                case 1:
                    return StringResources.Language.MQTTStatus01;
                case 2:
                    return StringResources.Language.MQTTStatus02;
                case 3:
                    return StringResources.Language.MQTTStatus03;
                case 4:
                    return StringResources.Language.MQTTStatus04;
                case 5:
                    return StringResources.Language.MQTTStatus05;
                default:
                    return StringResources.Language.UnknownError;
            }
        }

        /// <summary>
        /// 创建Mqtt发送消息的命令<br />
        /// Create Mqtt command to send messages
        /// </summary>
        /// <param name="message">封装后的消息内容</param>
        /// <returns>结果内容</returns>
        public static OperateResult<byte[]> BuildPublishMqttCommand(
          MqttPublishMessage message)
        {
            byte flags = 0;
            if (!message.IsSendFirstTime)
                flags |= (byte)8;
            if (message.Message.Retain)
                flags |= (byte)1;
            if (message.Message.QualityOfServiceLevel == MqttQualityOfServiceLevel.AtLeastOnce)
                flags |= (byte)2;
            else if (message.Message.QualityOfServiceLevel == MqttQualityOfServiceLevel.ExactlyOnce)
                flags |= (byte)4;
            else if (message.Message.QualityOfServiceLevel == MqttQualityOfServiceLevel.OnlyTransfer)
                flags |= (byte)6;
            List<byte> byteList = new List<byte>();
            byteList.AddRange((IEnumerable<byte>)MqttHelper.BuildSegCommandByString(message.Message.Topic));
            if ((uint)message.Message.QualityOfServiceLevel > 0U)
            {
                byteList.Add(BitConverter.GetBytes(message.Identifier)[1]);
                byteList.Add(BitConverter.GetBytes(message.Identifier)[0]);
            }
            return MqttHelper.BuildMqttCommand((byte)3, flags, byteList.ToArray(), message.Message.Payload);
        }

        /// <summary>
        /// 创建Mqtt发送消息的命令<br />
        /// Create Mqtt command to send messages
        /// </summary>
        /// <param name="topic">主题消息内容</param>
        /// <param name="payload">数据负载</param>
        /// <returns>结果内容</returns>
        public static OperateResult<byte[]> BuildPublishMqttCommand(
          string topic,
          byte[] payload)
        {
            return MqttHelper.BuildMqttCommand((byte)3, (byte)0, MqttHelper.BuildSegCommandByString(topic), payload);
        }

        /// <summary>
        /// 创建Mqtt订阅消息的命令<br />
        /// Command to create Mqtt subscription message
        /// </summary>
        /// <param name="message">订阅的主题</param>
        /// <returns>结果内容</returns>
        public static OperateResult<byte[]> BuildSubscribeMqttCommand(
          MqttSubscribeMessage message)
        {
            List<byte> byteList1 = new List<byte>();
            List<byte> byteList2 = new List<byte>();
            byteList1.Add(BitConverter.GetBytes(message.Identifier)[1]);
            byteList1.Add(BitConverter.GetBytes(message.Identifier)[0]);
            for (int index = 0; index < message.Topics.Length; ++index)
            {
                byteList2.AddRange((IEnumerable<byte>)MqttHelper.BuildSegCommandByString(message.Topics[index]));
                if (message.QualityOfServiceLevel == MqttQualityOfServiceLevel.AtMostOnce)
                    byteList2.AddRange((IEnumerable<byte>)new byte[1]);
                else if (message.QualityOfServiceLevel == MqttQualityOfServiceLevel.AtLeastOnce)
                    byteList2.AddRange((IEnumerable<byte>)new byte[1]
                    {
            (byte) 1
                    });
                else
                    byteList2.AddRange((IEnumerable<byte>)new byte[1]
                    {
            (byte) 2
                    });
            }
            return MqttHelper.BuildMqttCommand((byte)8, (byte)2, byteList1.ToArray(), byteList2.ToArray());
        }

        /// <summary>
        /// 创建Mqtt取消订阅消息的命令<br />
        /// Create Mqtt unsubscribe message command
        /// </summary>
        /// <param name="message">订阅的主题</param>
        /// <returns>结果内容</returns>
        public static OperateResult<byte[]> BuildUnSubscribeMqttCommand(
          MqttSubscribeMessage message)
        {
            List<byte> byteList1 = new List<byte>();
            List<byte> byteList2 = new List<byte>();
            byteList1.Add(BitConverter.GetBytes(message.Identifier)[1]);
            byteList1.Add(BitConverter.GetBytes(message.Identifier)[0]);
            for (int index = 0; index < message.Topics.Length; ++index)
                byteList2.AddRange((IEnumerable<byte>)MqttHelper.BuildSegCommandByString(message.Topics[index]));
            return MqttHelper.BuildMqttCommand((byte)10, (byte)2, byteList1.ToArray(), byteList2.ToArray());
        }

        /// <summary>
        /// 解析从MQTT接受的客户端信息，解析成实际的Topic数据及Payload数据<br />
        /// Parse the client information received from MQTT and parse it into actual Topic data and Payload data
        /// </summary>
        /// <param name="mqttCode">MQTT的命令码</param>
        /// <param name="data">接收的MQTT原始的消息内容</param>
        /// <returns>解析的数据结果信息</returns>
        public static OperateResult<string, byte[]> ExtraMqttReceiveData(
          byte mqttCode,
          byte[] data)
        {
            if (data.Length < 2)
                return new OperateResult<string, byte[]>(StringResources.Language.ReceiveDataLengthTooShort + data.Length.ToString());
            int count = (int)data[0] * 256 + (int)data[1];
            if (data.Length < 2 + count)
                return new OperateResult<string, byte[]>(string.Format("Code[{0:X2}] ExtraMqttReceiveData Error: {1}", (object)mqttCode, (object)SoftBasic.ByteToHexString(data, ' ')));
            string str = count > 0 ? Encoding.UTF8.GetString(data, 2, count) : string.Empty;
            byte[] numArray = new byte[data.Length - count - 2];
            Array.Copy((Array)data, count + 2, (Array)numArray, 0, numArray.Length);
            return OperateResult.CreateSuccessResult<string, byte[]>(str, numArray);
        }

        /// <summary>
        /// 使用指定的对象来返回网络的API接口，前提是传入的数据为json参数，返回的数据为json数据，详细参照说明<br />
        /// Use the specified object to return the API interface of the network,
        /// provided that the incoming data is json parameters and the returned data is json data,
        /// please refer to the description for details
        /// </summary>
        /// <param name="mqttSession">当前的对话状态</param>
        /// <param name="message">当前传入的消息内容</param>
        /// <param name="obj">等待解析的api解析的对象</param>
        /// <returns>等待返回客户的结果</returns>
        public static OperateResult<string> HandleObjectMethod(
          MqttSession mqttSession,
          MqttClientApplicationMessage message,
          object obj)
        {
            string name = message.Topic;
            if (name.LastIndexOf('/') >= 0)
                name = name.Substring(name.LastIndexOf('/') + 1);
            MethodInfo method = obj.GetType().GetMethod(name);
            if (method == (MethodInfo)null)
                return new OperateResult<string>("Current MqttSync Api ：[" + name + "] not exsist");
            OperateResult<MqttRpcApiInfo> servicesApiFromMethod = MqttHelper.GetMqttSyncServicesApiFromMethod("", method, obj);
            return !servicesApiFromMethod.IsSuccess ? OperateResult.CreateFailedResult<string>((OperateResult)servicesApiFromMethod) : MqttHelper.HandleObjectMethod(mqttSession, message, servicesApiFromMethod.Content);
        }

        /// <summary>
        /// 使用指定的对象来返回网络的API接口，前提是传入的数据为json参数，返回的数据为json数据，详细参照说明<br />
        /// Use the specified object to return the API interface of the network,
        /// provided that the incoming data is json parameters and the returned data is json data,
        /// please refer to the description for details
        /// </summary>
        /// <param name="mqttSession">当前的对话状态</param>
        /// <param name="message">当前传入的消息内容</param>
        /// <param name="apiInformation">当前已经解析好的Api内容对象</param>
        /// <returns>等待返回客户的结果</returns>
        public static OperateResult<string> HandleObjectMethod(
          MqttSession mqttSession,
          MqttClientApplicationMessage message,
          MqttRpcApiInfo apiInformation)
        {
            object obj = (object)null;
            if (apiInformation.PermissionAttribute != null)
            {
                if (!Authorization.asdniasnfaksndiqwhawfskhfaiw())
                    return new OperateResult<string>("Permission function need authorization ：" + StringResources.Language.InsufficientPrivileges);
                if (!apiInformation.PermissionAttribute.CheckClientID(mqttSession.ClientId))
                    return new OperateResult<string>("Mqtt RPC Api ：[" + apiInformation.ApiTopic + "] Check ClientID[" + mqttSession.ClientId + "] failed, access not permission");
                if (!apiInformation.PermissionAttribute.CheckUserName(mqttSession.UserName))
                    return new OperateResult<string>("Mqtt RPC Api ：[" + apiInformation.ApiTopic + "] Check Username[" + mqttSession.UserName + "] failed, access not permission");
            }
            try
            {
                if (apiInformation.Method != (MethodInfo)null)
                {
                    string json = Encoding.UTF8.GetString(message.Payload);
                    JObject jobject = string.IsNullOrEmpty(json) ? new JObject() : JObject.Parse(json);
                    object[] parametersFromJson = EstReflectionHelper.GetParametersFromJson(apiInformation.Method.GetParameters(), json);
                    obj = apiInformation.Method.Invoke(apiInformation.SourceObject, parametersFromJson);
                }
                else if (apiInformation.Property != (PropertyInfo)null)
                    obj = apiInformation.Property.GetValue(apiInformation.SourceObject, (object[])null);
            }
            catch (Exception ex)
            {
                return new OperateResult<string>("Mqtt RPC Api ：[" + apiInformation.ApiTopic + "] Wrong，Reason：" + ex.Message);
            }
            return EstReflectionHelper.GetOperateResultJsonFromObj(obj);
        }

        /// <inheritdoc cref="M:ESTCore.Common.MQTT.MqttHelper.GetSyncServicesApiInformationFromObject(System.String,System.Object,ESTCore.Common.Reflection.EstMqttPermissionAttribute)" />
        public static List<MqttRpcApiInfo> GetSyncServicesApiInformationFromObject(
          object obj)
        {
            return obj is Type type ? MqttHelper.GetSyncServicesApiInformationFromObject(type.Name, (object)type) : MqttHelper.GetSyncServicesApiInformationFromObject(obj.GetType().Name, obj);
        }

        /// <summary>
        /// 根据当前的对象定义的方法信息，获取到所有支持ApiTopic的方法列表信息，包含API名称，示例参数数据，描述信息。<br />
        /// According to the method information defined by the current object, the list information of all methods that support ApiTopic is obtained,
        /// including the API name, sample parameter data, and description information.
        /// </summary>
        /// <param name="api">指定的ApiTopic的前缀，可以理解为控制器，如果为空，就不携带控制器。</param>
        /// <param name="obj">实际的等待解析的对象</param>
        /// <param name="permissionAttribute">默认的权限特性</param>
        /// <returns>返回所有API说明的列表，类型为<see cref="T:ESTCore.Common.MQTT.MqttRpcApiInfo" /></returns>
        public static List<MqttRpcApiInfo> GetSyncServicesApiInformationFromObject(
          string api,
          object obj,
          EstMqttPermissionAttribute permissionAttribute = null)
        {
            Type type1;
            if (obj is Type type2)
            {
                type1 = type2;
                obj = (object)null;
            }
            else
                type1 = obj.GetType();
            MethodInfo[] methods = type1.GetMethods();
            List<MqttRpcApiInfo> mqttRpcApiInfoList = new List<MqttRpcApiInfo>();
            foreach (MethodInfo method in methods)
            {
                OperateResult<MqttRpcApiInfo> servicesApiFromMethod = MqttHelper.GetMqttSyncServicesApiFromMethod(api, method, obj, permissionAttribute);
                if (servicesApiFromMethod.IsSuccess)
                    mqttRpcApiInfoList.Add(servicesApiFromMethod.Content);
            }
            foreach (PropertyInfo property in type1.GetProperties())
            {
                OperateResult<EstMqttApiAttribute, MqttRpcApiInfo> servicesApiFromProperty = MqttHelper.GetMqttSyncServicesApiFromProperty(api, property, obj, permissionAttribute);
                if (servicesApiFromProperty.IsSuccess)
                {
                    if (!servicesApiFromProperty.Content1.PropertyUnfold)
                        mqttRpcApiInfoList.Add(servicesApiFromProperty.Content2);
                    else if (property.GetValue(obj, (object[])null) != null)
                    {
                        List<MqttRpcApiInfo> informationFromObject = MqttHelper.GetSyncServicesApiInformationFromObject(servicesApiFromProperty.Content2.ApiTopic, property.GetValue(obj, (object[])null), permissionAttribute);
                        mqttRpcApiInfoList.AddRange((IEnumerable<MqttRpcApiInfo>)informationFromObject);
                    }
                }
            }
            return mqttRpcApiInfoList;
        }

        /// <summary>
        /// 根据当前的方法的委托信息和类对象，生成<see cref="T:ESTCore.Common.MQTT.MqttRpcApiInfo" />的API对象信息。
        /// </summary>
        /// <param name="api">Api头信息</param>
        /// <param name="method">方法的委托</param>
        /// <param name="obj">当前注册的API的源对象</param>
        /// <param name="permissionAttribute">默认的权限特性</param>
        /// <returns>返回是否成功的结果对象</returns>
        public static OperateResult<MqttRpcApiInfo> GetMqttSyncServicesApiFromMethod(
          string api,
          MethodInfo method,
          object obj,
          EstMqttPermissionAttribute permissionAttribute = null)
        {
            object[] customAttributes1 = method.GetCustomAttributes(typeof(EstMqttApiAttribute), false);
            if (customAttributes1 == null || customAttributes1.Length == 0)
                return new OperateResult<MqttRpcApiInfo>(string.Format("Current Api ：[{0}] not support Api attribute", (object)method));
            EstMqttApiAttribute mqttApiAttribute = (EstMqttApiAttribute)customAttributes1[0];
            MqttRpcApiInfo mqttRpcApiInfo = new MqttRpcApiInfo();
            mqttRpcApiInfo.SourceObject = obj;
            mqttRpcApiInfo.Method = method;
            mqttRpcApiInfo.Description = mqttApiAttribute.Description;
            mqttRpcApiInfo.HttpMethod = mqttApiAttribute.HttpMethod.ToUpper();
            if (string.IsNullOrEmpty(mqttApiAttribute.ApiTopic))
                mqttApiAttribute.ApiTopic = method.Name;
            if (permissionAttribute == null)
            {
                object[] customAttributes2 = method.GetCustomAttributes(typeof(EstMqttPermissionAttribute), false);
                if (customAttributes2 != null && (uint)customAttributes2.Length > 0U)
                    mqttRpcApiInfo.PermissionAttribute = (EstMqttPermissionAttribute)customAttributes2[0];
            }
            else
                mqttRpcApiInfo.PermissionAttribute = permissionAttribute;
            mqttRpcApiInfo.ApiTopic = !string.IsNullOrEmpty(api) ? api + "/" + mqttApiAttribute.ApiTopic : mqttApiAttribute.ApiTopic;
            mqttRpcApiInfo.ExamplePayload = EstReflectionHelper.GetParametersFromJson(method).ToString();
            return OperateResult.CreateSuccessResult<MqttRpcApiInfo>(mqttRpcApiInfo);
        }

        /// <summary>
        /// 根据当前的方法的委托信息和类对象，生成<see cref="T:ESTCore.Common.MQTT.MqttRpcApiInfo" />的API对象信息。
        /// </summary>
        /// <param name="api">Api头信息</param>
        /// <param name="property">方法的委托</param>
        /// <param name="obj">当前注册的API的源对象</param>
        /// <param name="permissionAttribute">默认的权限特性</param>
        /// <returns>返回是否成功的结果对象</returns>
        public static OperateResult<EstMqttApiAttribute, MqttRpcApiInfo> GetMqttSyncServicesApiFromProperty(
          string api,
          PropertyInfo property,
          object obj,
          EstMqttPermissionAttribute permissionAttribute = null)
        {
            object[] customAttributes1 = property.GetCustomAttributes(typeof(EstMqttApiAttribute), false);
            if (customAttributes1 == null || customAttributes1.Length == 0)
                return new OperateResult<EstMqttApiAttribute, MqttRpcApiInfo>(string.Format("Current Api ：[{0}] not support Api attribute", (object)property));
            EstMqttApiAttribute mqttApiAttribute = (EstMqttApiAttribute)customAttributes1[0];
            MqttRpcApiInfo mqttRpcApiInfo = new MqttRpcApiInfo();
            mqttRpcApiInfo.SourceObject = obj;
            mqttRpcApiInfo.Property = property;
            mqttRpcApiInfo.Description = mqttApiAttribute.Description;
            mqttRpcApiInfo.HttpMethod = mqttApiAttribute.HttpMethod.ToUpper();
            if (string.IsNullOrEmpty(mqttApiAttribute.ApiTopic))
                mqttApiAttribute.ApiTopic = property.Name;
            if (permissionAttribute == null)
            {
                object[] customAttributes2 = property.GetCustomAttributes(typeof(EstMqttPermissionAttribute), false);
                if (customAttributes2 != null && (uint)customAttributes2.Length > 0U)
                    mqttRpcApiInfo.PermissionAttribute = (EstMqttPermissionAttribute)customAttributes2[0];
            }
            else
                mqttRpcApiInfo.PermissionAttribute = permissionAttribute;
            mqttRpcApiInfo.ApiTopic = !string.IsNullOrEmpty(api) ? api + "/" + mqttApiAttribute.ApiTopic : mqttApiAttribute.ApiTopic;
            mqttRpcApiInfo.ExamplePayload = string.Empty;
            return OperateResult.CreateSuccessResult<EstMqttApiAttribute, MqttRpcApiInfo>(mqttApiAttribute, mqttRpcApiInfo);
        }
    }
}
