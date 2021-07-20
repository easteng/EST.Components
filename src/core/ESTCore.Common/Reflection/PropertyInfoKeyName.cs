// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Reflection.PropertyInfoKeyName
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using System.Reflection;

namespace ESTCore.Common.Reflection
{
    internal class PropertyInfoKeyName
    {
        public PropertyInfoKeyName(PropertyInfo property, string key)
        {
            this.PropertyInfo = property;
            this.KeyName = key;
        }

        public PropertyInfoKeyName(PropertyInfo property, string key, string value)
        {
            this.PropertyInfo = property;
            this.KeyName = key;
            this.Value = value;
        }

        public PropertyInfo PropertyInfo { get; set; }

        public string KeyName { get; set; }

        public string Value { get; set; }
    }
}
