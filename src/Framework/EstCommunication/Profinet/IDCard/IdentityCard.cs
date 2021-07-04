// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.IDCard.IdentityCard
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using System;
using System.Text;

namespace EstCommunication.Profinet.IDCard
{
    /// <summary>身份证的信息类</summary>
    public class IdentityCard
    {
        /// <summary>名字</summary>
        public string Name { get; set; }

        /// <summary>性别</summary>
        public string Sex { get; set; }

        /// <summary>身份证号</summary>
        public string Id { get; set; }

        /// <summary>民族</summary>
        public string Nation { get; set; }

        /// <summary>生日</summary>
        public DateTime Birthday { get; set; }

        /// <summary>地址</summary>
        public string Address { get; set; }

        /// <summary>发证机关</summary>
        public string Organ { get; set; }

        /// <summary>有效期日期的起始日期</summary>
        public DateTime ValidityStartDate { get; set; }

        /// <summary>有效期日期的结束日期</summary>
        public DateTime ValidityEndDate { get; set; }

        /// <summary>头像信息</summary>
        public byte[] Portrait { get; set; }

        /// <summary>返回表示当前对象的字符串</summary>
        /// <returns>字符串</returns>
        public override string ToString()
        {
            StringBuilder stringBuilder1 = new StringBuilder();
            stringBuilder1.Append("姓名：" + this.Name);
            stringBuilder1.Append(Environment.NewLine);
            stringBuilder1.Append("性别：" + this.Sex);
            stringBuilder1.Append(Environment.NewLine);
            stringBuilder1.Append("民族：" + this.Nation);
            stringBuilder1.Append(Environment.NewLine);
            stringBuilder1.Append("身份证号：" + this.Id);
            stringBuilder1.Append(Environment.NewLine);
            StringBuilder stringBuilder2 = stringBuilder1;
            DateTime birthday = this.Birthday;
            // ISSUE: variable of a boxed type
            var year = (ValueType)birthday.Year;
            birthday = this.Birthday;
            // ISSUE: variable of a boxed type
            var month = (ValueType)birthday.Month;
            birthday = this.Birthday;
            // ISSUE: variable of a boxed type
            var day = (ValueType)birthday.Day;
            string str = string.Format("出身日期：{0}年{1}月{2}日", (object)year, (object)month, (object)day);
            stringBuilder2.Append(str);
            stringBuilder1.Append(Environment.NewLine);
            stringBuilder1.Append("地址：" + this.Address);
            stringBuilder1.Append(Environment.NewLine);
            stringBuilder1.Append("发证机关：" + this.Organ);
            stringBuilder1.Append(Environment.NewLine);
            stringBuilder1.Append(string.Format("有效日期：{0}年{1}月{2}日 - {3}年{4}月{5}日", (object)this.ValidityStartDate.Year, (object)this.ValidityStartDate.Month, (object)this.ValidityStartDate.Day, (object)this.ValidityEndDate.Year, (object)this.ValidityEndDate.Month, (object)this.ValidityEndDate.Day));
            stringBuilder1.Append(Environment.NewLine);
            return stringBuilder1.ToString();
        }
    }
}
