// Decompiled with JetBrains decompiler
// Type: EstCommunication.BasicFramework.ISqlDataType
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using System.Data.SqlClient;

namespace EstCommunication.BasicFramework
{
  /// <summary>数据库对应类的读取接口</summary>
  public interface ISqlDataType
  {
    /// <summary>根据sdr对象初始化数据的方法</summary>
    /// <param name="sdr">数据库reader对象</param>
    void LoadBySqlDataReader(SqlDataReader sdr);
  }
}
