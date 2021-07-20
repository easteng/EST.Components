// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.MQTT.MqttControlMessage
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

namespace ESTCore.Common.MQTT
{
    /// <summary>定义了Mqtt的相关的控制报文的信息</summary>
    public class MqttControlMessage
    {
        /// <summary>操作失败的信息返回</summary>
        public const byte FAILED = 0;
        /// <summary>连接标识</summary>
        public const byte CONNECT = 1;
        /// <summary>连接返回的标识</summary>
        public const byte CONNACK = 2;
        /// <summary>发布消息</summary>
        public const byte PUBLISH = 3;
        /// <summary>QoS 1消息发布收到确认</summary>
        public const byte PUBACK = 4;
        /// <summary>发布收到（保证交付第一步）</summary>
        public const byte PUBREC = 5;
        /// <summary>发布释放（保证交付第二步）</summary>
        public const byte PUBREL = 6;
        /// <summary>QoS 2消息发布完成（保证交互第三步）</summary>
        public const byte PUBCOMP = 7;
        /// <summary>客户端订阅请求</summary>
        public const byte SUBSCRIBE = 8;
        /// <summary>订阅请求报文确认</summary>
        public const byte SUBACK = 9;
        /// <summary>客户端取消订阅请求</summary>
        public const byte UNSUBSCRIBE = 10;
        /// <summary>取消订阅报文确认</summary>
        public const byte UNSUBACK = 11;
        /// <summary>心跳请求</summary>
        public const byte PINGREQ = 12;
        /// <summary>心跳响应</summary>
        public const byte PINGRESP = 13;
        /// <summary>客户端断开连接</summary>
        public const byte DISCONNECT = 14;
        /// <summary>报告进度</summary>
        public const byte REPORTPROGRESS = 15;
        /// <summary>文件传输中没有意义的标记</summary>
        public const byte FileNoSense = 100;
        /// <summary>下载文件的命令，一次只能下载一个文件</summary>
        public const byte FileDownload = 101;
        /// <summary>上传文件的命令，一次只能上传一个文件</summary>
        public const byte FileUpload = 102;
        /// <summary>删除文件的命令，一次可以删除多个文件</summary>
        public const byte FileDelete = 103;
        /// <summary>删除目录的命令，目录下面的所有的文件都会被删除</summary>
        public const byte FileFolderDelete = 104;
        /// <summary>遍历指定目录下所有的文件信息</summary>
        public const byte FileFolderFiles = 105;
        /// <summary>遍历指定目录下所有的子目录信息</summary>
        public const byte FileFolderPaths = 106;
        /// <summary>文件是否存在</summary>
        public const byte FileExists = 107;
    }
}
