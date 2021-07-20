// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.BasicFramework.SoftMail
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using System;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace ESTCore.Common.BasicFramework
{
    /// <summary>软件的邮箱类，用于发送邮箱数据</summary>
    /// <remarks>
    /// 如果您想实现自己的邮件发送中心，就可以去对应的邮件服务器注册，如果是想快速实现邮件的发送，本系统提供了2个静态的已经注册好了的邮箱发送器。
    /// </remarks>
    /// <example>
    /// 以下的代码演示了通过静态的发送对象来发送邮件，包含了发送普通的邮件，和发送html邮件。
    /// <code lang="cs" source="TestProject\ESTCore.CommonDemo\FormMail.cs" region="SoftMail" title="SoftMail示例" />
    /// </example>
    public class SoftMail
    {
        /// <summary>系统提供一个默认的163邮箱发送账号，只要更改接收地址即可发送服务，可能会被拦截</summary>
        public static SoftMail MailSystem163 = new SoftMail((Action<SmtpClient>)(mail =>
       {
           mail.Host = "smtp.163.com";
           mail.UseDefaultCredentials = true;
           mail.EnableSsl = true;
           mail.Port = 25;
           mail.DeliveryMethod = SmtpDeliveryMethod.Network;
           mail.Credentials = (ICredentialsByHost)new NetworkCredential("softmailsendcenter", "zxcvbnm6789");
       }), "softmailsendcenter@163.com", "hsl200909@163.com");
        /// <summary>系统提供一个默认的QQ邮箱发送账号，只要更改接收地址即可发送服务，发送成功概率比较高</summary>
        public static SoftMail MailSystemQQ = new SoftMail((Action<SmtpClient>)(mail =>
       {
           mail.Host = "smtp.qq.com";
           mail.UseDefaultCredentials = true;
           mail.Port = 587;
           mail.EnableSsl = true;
           mail.DeliveryMethod = SmtpDeliveryMethod.Network;
           mail.Credentials = (ICredentialsByHost)new NetworkCredential("974856779", "tvnlczxdumutbbic");
       }), "974856779@qq.com", "hsl200909@163.com");

        /// <summary>系统连续发送失败的次数，为了不影响系统，连续三次失败就禁止发送</summary>
        private static long SoftMailSendFailedCount { get; set; } = 0;

        /// <summary>实例化一个邮箱发送类，需要指定初始化信息</summary>
        /// <param name="mailIni">初始化的方法</param>
        /// <param name="addr_From">发送地址，应该和账户匹配</param>
        /// <param name="addr_to">邮件接收地址</param>
        /// <remarks>初始化的方法比较复杂，需要参照示例代码。</remarks>
        /// <example>
        /// <code lang="cs" source="ESTCore.Common_Net45\BasicFramework\SoftMail.cs" region="Static Mail" title="SoftMail示例" />
        /// </example>
        public SoftMail(Action<SmtpClient> mailIni, string addr_From = "", string addr_to = "")
        {
            this.smtpClient = new SmtpClient();
            mailIni(this.smtpClient);
            this.MailFromAddress = addr_From;
            this.MailSendAddress = addr_to;
        }

        /// <summary>系统的邮件发送客户端</summary>
        private SmtpClient smtpClient { get; set; }

        /// <summary>发送邮件的地址</summary>
        private string MailFromAddress { get; set; } = "";

        /// <summary>邮件发送的地址</summary>
        public string MailSendAddress { get; set; } = "";

        private string GetExceptionMail(Exception ex) => StringResources.Language.Time + DateTime.Now.ToString() + Environment.NewLine + StringResources.Language.SoftWare + ex.Source + Environment.NewLine + StringResources.Language.ExceptionMessage + ex.Message + Environment.NewLine + StringResources.Language.ExceptionType + ex.GetType().ToString() + Environment.NewLine + StringResources.Language.ExceptionStackTrace + ex.StackTrace + Environment.NewLine + StringResources.Language.ExceptionTargetSite + ex.TargetSite.Name;

        /// <summary>发生BUG至邮件地址，需要提前指定发送地址，否则失败</summary>
        /// <param name="ex">异常的BUG，同样试用兼容类型</param>
        /// <returns>是否发送成功，内容不正确会被视为垃圾邮件</returns>
        public bool SendMail(Exception ex) => this.SendMail(ex, "");

        /// <summary>发送邮件至地址，需要提前指定发送地址，否则失败</summary>
        /// <param name="subject">邮件主题</param>
        /// <param name="body">邮件内容</param>
        /// <returns>是否发送成功，内容不正确会被视为垃圾邮件</returns>
        public bool SendMail(string subject, string body) => this.SendMail(this.MailSendAddress, subject, body);

        /// <summary>发送邮件至地址，需要提前指定发送地址，否则失败</summary>
        /// <param name="subject">邮件主题</param>
        /// <param name="body">邮件内容</param>
        /// <param name="isHtml">是否是html格式化文本</param>
        /// <returns>是否发送成功，内容不正确会被视为垃圾邮件</returns>
        public bool SendMail(string subject, string body, bool isHtml) => this.SendMail(this.MailSendAddress, subject, body, isHtml);

        /// <summary>发生BUG至邮件地址，需要提前指定发送地址，否则失败</summary>
        /// <param name="ex">异常的BUG，同样试用兼容类型</param>
        /// <param name="addtion">额外信息</param>
        /// <returns>是否发送成功，内容不正确会被视为垃圾邮件</returns>
        public bool SendMail(Exception ex, string addtion) => !string.IsNullOrEmpty(this.MailSendAddress) && this.SendMail(this.MailSendAddress, StringResources.Language.BugSubmit, string.IsNullOrEmpty(addtion) ? this.GetExceptionMail(ex) : "User：" + addtion + Environment.NewLine + this.GetExceptionMail(ex));

        /// <summary>发送邮件的方法，需要指定接收地址，主题及内容</summary>
        /// <param name="addr_to">接收地址</param>
        /// <param name="subject">邮件主题</param>
        /// <param name="body">邮件内容</param>
        /// <returns>是否发送成功，内容不正确会被视为垃圾邮件</returns>
        public bool SendMail(string addr_to, string subject, string body) => this.SendMail(addr_to, subject, body, false);

        /// <summary>发送邮件的方法，默认发送别名，优先级，是否HTML</summary>
        /// <param name="addr_to">接收地址</param>
        /// <param name="subject">邮件主题</param>
        /// <param name="body">邮件内容</param>
        /// <param name="isHtml">是否是html格式的内容</param>
        /// <returns>是否发送成功，内容不正确会被视为垃圾邮件</returns>
        public bool SendMail(string addr_to, string subject, string body, bool isHtml) => this.SendMail(this.MailFromAddress, StringResources.Language.MailServerCenter, new string[1]
        {
      addr_to
        }, subject, body, MailPriority.Normal, (isHtml ? 1 : 0) != 0);

        /// <summary>发送邮件的方法，需要提供完整的参数信息</summary>
        /// <param name="addr_from">发送地址</param>
        /// <param name="name">发送别名</param>
        /// <param name="addr_to">接收地址</param>
        /// <param name="subject">邮件主题</param>
        /// <param name="body">邮件内容</param>
        /// <param name="priority">优先级</param>
        /// <param name="isHtml">邮件内容是否是HTML语言</param>
        /// <returns>发生是否成功，内容不正确会被视为垃圾邮件</returns>
        public bool SendMail(
          string addr_from,
          string name,
          string[] addr_to,
          string subject,
          string body,
          MailPriority priority,
          bool isHtml)
        {
            if (SoftMail.SoftMailSendFailedCount > 10L)
            {
                ++SoftMail.SoftMailSendFailedCount;
                return true;
            }
            using (MailMessage message = new MailMessage())
            {
                try
                {
                    message.From = new MailAddress(addr_from, name, Encoding.UTF8);
                    foreach (string addresses in addr_to)
                        message.To.Add(addresses);
                    message.Subject = subject;
                    message.Body = body;
                    MailMessage mailMessage = message;
                    mailMessage.Body = mailMessage.Body + Environment.NewLine + Environment.NewLine + Environment.NewLine + StringResources.Language.MailSendTail;
                    message.SubjectEncoding = Encoding.UTF8;
                    message.BodyEncoding = Encoding.UTF8;
                    message.Priority = priority;
                    message.IsBodyHtml = isHtml;
                    this.smtpClient.Send(message);
                    SoftMail.SoftMailSendFailedCount = 0L;
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(SoftBasic.GetExceptionMessage(ex));
                    ++SoftMail.SoftMailSendFailedCount;
                    return false;
                }
            }
        }
    }
}
