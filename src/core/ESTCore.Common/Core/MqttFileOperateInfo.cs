// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Core.MqttFileOperateInfo
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using System;

namespace ESTCore.Common.Core
{
    /// <summary>
    /// 当前MQTT服务器文件操作的对象类<br />
    /// Object class of current MQTT server file operation
    /// </summary>
    public class MqttFileOperateInfo
    {
        /// <summary>
        /// 指示上传还是下载的操作，Upload上传，Download下载，Delete删除文件，DeleteFolder删除目录<br />
        /// Indicate upload or download operation, "Upload": upload, "Download": download, "Delete": delete file, "DeleteFolder": delete directory
        /// </summary>
        public string Operate { get; set; }

        /// <summary>文件上传或是下载的类别</summary>
        public string Groups { get; set; }

        /// <summary>
        /// 上传,下载或是删除的文件名<br />
        /// File name uploaded, downloaded or deleted
        /// </summary>
        public string[] FileNames { get; set; }

        /// <summary>
        /// 当前操作消耗的时间<br />
        /// Time consumed by current operation
        /// </summary>
        public TimeSpan TimeCost { get; set; }
    }
}
