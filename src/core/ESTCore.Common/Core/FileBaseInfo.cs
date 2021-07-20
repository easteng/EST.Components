// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Core.FileBaseInfo
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

namespace ESTCore.Common.Core
{
    /// <summary>文件的基础信息</summary>
    public class FileBaseInfo
    {
        /// <summary>文件名称</summary>
        public string Name { get; set; }

        /// <summary>文件大小</summary>
        public long Size { get; set; }

        /// <summary>文件的标识，注释</summary>
        public string Tag { get; set; }

        /// <summary>文件上传人的名称</summary>
        public string Upload { get; set; }
    }
}
