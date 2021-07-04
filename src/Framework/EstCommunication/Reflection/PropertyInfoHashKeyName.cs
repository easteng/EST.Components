// Decompiled with JetBrains decompiler
// Type: EstCommunication.Reflection.PropertyInfoHashKeyName
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using System.Reflection;

namespace EstCommunication.Reflection
{
  internal class PropertyInfoHashKeyName : PropertyInfoKeyName
  {
    public PropertyInfoHashKeyName(PropertyInfo property, string key, string field)
      : base(property, key)
      => this.Field = field;

    public PropertyInfoHashKeyName(PropertyInfo property, string key, string field, string value)
      : base(property, key, value)
      => this.Field = field;

    public string Field { get; set; }
  }
}
