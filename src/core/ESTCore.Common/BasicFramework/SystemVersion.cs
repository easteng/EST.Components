// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.BasicFramework.SystemVersion
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using System;

namespace ESTCore.Common.BasicFramework
{
    /// <summary>
    /// 系统版本类，由三部分组成，包含了一个大版本，小版本，修订版，还有一个开发者维护的内部版<br />
    /// System version class, consisting of three parts, including a major version, minor version, revised version, and an internal version maintained by the developer
    /// </summary>
    [Serializable]
    public sealed class SystemVersion
    {
        private int m_MainVersion = 2;
        private int m_SecondaryVersion = 0;
        private int m_EditVersion = 0;
        private int m_InnerVersion = 0;

        /// <summary>
        /// 根据格式化字符串的版本号初始化，例如：1.0或1.0.0或1.0.0.0503<br />
        /// Initialize according to the version number of the formatted string, for example: 1.0 or 1.0.0 or 1.0.0.0503
        /// </summary>
        /// <param name="VersionString">格式化的字符串，例如：1.0或1.0.0或1.0.0.0503</param>
        public SystemVersion(string VersionString)
        {
            string[] strArray = VersionString.Split(new char[1]
            {
        '.'
            }, StringSplitOptions.RemoveEmptyEntries);
            if (strArray.Length >= 1)
                this.m_MainVersion = Convert.ToInt32(strArray[0]);
            if (strArray.Length >= 2)
                this.m_SecondaryVersion = Convert.ToInt32(strArray[1]);
            if (strArray.Length >= 3)
                this.m_EditVersion = Convert.ToInt32(strArray[2]);
            if (strArray.Length < 4)
                return;
            this.m_InnerVersion = Convert.ToInt32(strArray[3]);
        }

        /// <summary>
        /// 根据指定的主版本，次版本，修订版来实例化一个对象<br />
        /// Instantiate an object based on the specified major, minor, and revision
        /// </summary>
        /// <param name="main">主版本</param>
        /// <param name="sec">次版本</param>
        /// <param name="edit">修订版</param>
        public SystemVersion(int main, int sec, int edit)
        {
            this.m_MainVersion = main;
            this.m_SecondaryVersion = sec;
            this.m_EditVersion = edit;
        }

        /// <summary>
        /// 根据指定的主版本，次版本，修订版，内部版本来实例化一个对象<br />
        /// Instantiate an object based on the specified major, minor, revision, and build
        /// </summary>
        /// <param name="main">主版本</param>
        /// <param name="sec">次版本</param>
        /// <param name="edit">修订版</param>
        /// <param name="inner">内部版本号</param>
        public SystemVersion(int main, int sec, int edit, int inner)
        {
            this.m_MainVersion = main;
            this.m_SecondaryVersion = sec;
            this.m_EditVersion = edit;
            this.m_InnerVersion = inner;
        }

        /// <summary>主版本</summary>
        public int MainVersion => this.m_MainVersion;

        /// <summary>次版本</summary>
        public int SecondaryVersion => this.m_SecondaryVersion;

        /// <summary>修订版</summary>
        public int EditVersion => this.m_EditVersion;

        /// <summary>内部版本号，或者是版本号表示为年月份+内部版本的表示方式</summary>
        public int InnerVersion => this.m_InnerVersion;

        /// <summary>
        /// 根据格式化为支持返回的不同信息的版本号<br />
        /// C返回1.0.0.0<br />
        /// N返回1.0.0<br />
        /// S返回1.0
        /// </summary>
        /// <param name="format">格式化信息</param>
        /// <returns>版本号信息</returns>
        public string ToString(string format)
        {
            if (format == "C")
                return string.Format("{0}.{1}.{2}.{3}", (object)this.MainVersion, (object)this.SecondaryVersion, (object)this.EditVersion, (object)this.InnerVersion);
            if (format == "N")
                return string.Format("{0}.{1}.{2}", (object)this.MainVersion, (object)this.SecondaryVersion, (object)this.EditVersion);
            return format == "S" ? string.Format("{0}.{1}", (object)this.MainVersion, (object)this.SecondaryVersion) : this.ToString();
        }

        /// <summary>获取版本号的字符串形式，如果内部版本号为0，则显示时不携带</summary>
        /// <returns>版本号信息</returns>
        public override string ToString() => this.InnerVersion == 0 ? this.ToString("N") : this.ToString("C");

        /// <summary>判断两个实例是否相等</summary>
        /// <param name="obj">版本号</param>
        /// <returns>是否一致</returns>
        public override bool Equals(object obj) => base.Equals(obj);

        /// <summary>获取哈希值</summary>
        /// <returns>哈希值</returns>
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>判断是否相等</summary>
        /// <param name="SV1">第一个版本</param>
        /// <param name="SV2">第二个版本</param>
        /// <returns>是否相同</returns>
        public static bool operator ==(SystemVersion SV1, SystemVersion SV2) => SV1.MainVersion == SV2.MainVersion && SV1.SecondaryVersion == SV2.SecondaryVersion && (SV1.m_EditVersion == SV2.m_EditVersion && SV1.InnerVersion == SV2.InnerVersion);

        /// <summary>判断是否不相等</summary>
        /// <param name="SV1">第一个版本号</param>
        /// <param name="SV2">第二个版本号</param>
        /// <returns>是否相同</returns>
        public static bool operator !=(SystemVersion SV1, SystemVersion SV2) => SV1.MainVersion != SV2.MainVersion || SV1.SecondaryVersion != SV2.SecondaryVersion || (SV1.m_EditVersion != SV2.m_EditVersion || SV1.InnerVersion != SV2.InnerVersion);

        /// <summary>判断一个版本是否大于另一个版本</summary>
        /// <param name="SV1">第一个版本</param>
        /// <param name="SV2">第二个版本</param>
        /// <returns>是否相同</returns>
        public static bool operator >(SystemVersion SV1, SystemVersion SV2)
        {
            if (SV1.MainVersion > SV2.MainVersion)
                return true;
            if (SV1.MainVersion < SV2.MainVersion)
                return false;
            if (SV1.SecondaryVersion > SV2.SecondaryVersion)
                return true;
            if (SV1.SecondaryVersion < SV2.SecondaryVersion)
                return false;
            if (SV1.EditVersion > SV2.EditVersion)
                return true;
            if (SV1.EditVersion < SV2.EditVersion)
                return false;
            if (SV1.InnerVersion > SV2.InnerVersion)
                return true;
            return SV1.InnerVersion < SV2.InnerVersion && false;
        }

        /// <summary>判断第一个版本是否小于第二个版本</summary>
        /// <param name="SV1">第一个版本号</param>
        /// <param name="SV2">第二个版本号</param>
        /// <returns>是否小于</returns>
        public static bool operator <(SystemVersion SV1, SystemVersion SV2)
        {
            if (SV1.MainVersion < SV2.MainVersion)
                return true;
            if (SV1.MainVersion > SV2.MainVersion)
                return false;
            if (SV1.SecondaryVersion < SV2.SecondaryVersion)
                return true;
            if (SV1.SecondaryVersion > SV2.SecondaryVersion)
                return false;
            if (SV1.EditVersion < SV2.EditVersion)
                return true;
            if (SV1.EditVersion > SV2.EditVersion)
                return false;
            if (SV1.InnerVersion < SV2.InnerVersion)
                return true;
            return SV1.InnerVersion > SV2.InnerVersion && false;
        }
    }
}
