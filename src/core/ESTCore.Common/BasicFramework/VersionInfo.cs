// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.BasicFramework.VersionInfo
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using System;
using System.Text;

namespace ESTCore.Common.BasicFramework
{
    /// <summary>版本信息类，用于展示版本发布信息</summary>
    public sealed class VersionInfo
    {
        /// <summary>版本的发行日期</summary>
        public DateTime ReleaseDate { get; set; } = DateTime.Now;

        /// <summary>版本的更新细节</summary>
        public StringBuilder UpdateDetails { get; set; } = new StringBuilder();

        /// <summary>版本号</summary>
        public SystemVersion VersionNum { get; set; } = new SystemVersion(1, 0, 0);

        /// <inheritdoc />
        public override string ToString() => this.VersionNum.ToString();
    }
}
