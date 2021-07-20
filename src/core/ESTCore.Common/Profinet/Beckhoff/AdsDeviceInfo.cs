// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Beckhoff.AdsDeviceInfo
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using System;
using System.Text;

namespace ESTCore.Common.Profinet.Beckhoff
{
    /// <summary>
    /// Ads设备的相关信息，主要是版本号，设备名称<br />
    /// Information about Ads devices, primarily version numbers, device names.
    /// </summary>
    public class AdsDeviceInfo
    {
        /// <summary>
        /// 实例化一个默认的对象<br />
        /// Instantiate a default object
        /// </summary>
        public AdsDeviceInfo()
        {
        }

        /// <summary>
        /// 根据原始的数据内容来实例化一个对象<br />
        /// Instantiate an object based on the original data content
        /// </summary>
        /// <param name="data">原始的数据内容</param>
        public AdsDeviceInfo(byte[] data)
        {
            this.Major = data[0];
            this.Minor = data[1];
            this.Build = BitConverter.ToUInt16(data, 2);
            this.DeviceName = Encoding.ASCII.GetString(data.RemoveBegin<byte>(4)).Trim(char.MinValue, ' ');
        }

        /// <summary>
        /// 主版本号<br />
        /// Main Version
        /// </summary>
        public byte Major { get; set; }

        /// <summary>
        /// 次版本号<br />
        /// Minor Version
        /// </summary>
        public byte Minor { get; set; }

        /// <summary>
        /// 构建版本号<br />
        /// Build version
        /// </summary>
        public ushort Build { get; set; }

        /// <summary>
        /// 设备的名字<br />
        /// Device Name
        /// </summary>
        public string DeviceName { get; set; }
    }
}
