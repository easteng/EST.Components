// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.LogNet.LogNetManagment
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using System;
using System.Reflection;
using System.Text;

namespace ESTCore.Common.LogNet
{
    /// <summary>
    /// 日志类的管理器，提供了基本的功能代码。<br />
    /// The manager of the log class provides the basic function code.
    /// </summary>
    public class LogNetManagment
    {
        /// <summary>日志文件的头标志</summary>
        internal const string LogFileHeadString = "Logs_";

        internal static string GetDegreeDescription(EstMessageDegree degree)
        {
            switch (degree)
            {
                case EstMessageDegree.None:
                    return StringResources.Language.LogNetAbandon;
                case EstMessageDegree.FATAL:
                    return StringResources.Language.LogNetFatal;
                case EstMessageDegree.ERROR:
                    return StringResources.Language.LogNetError;
                case EstMessageDegree.WARN:
                    return StringResources.Language.LogNetWarn;
                case EstMessageDegree.INFO:
                    return StringResources.Language.LogNetInfo;
                case EstMessageDegree.DEBUG:
                    return StringResources.Language.LogNetDebug;
                default:
                    return StringResources.Language.LogNetAbandon;
            }
        }

        /// <summary>
        /// 公开的一个静态变量，允许随意的设置<br />
        /// Public static variable, allowing arbitrary setting
        /// </summary>
        public static ILogNet LogNet { get; set; }

        /// <summary>
        /// 通过异常文本格式化成字符串用于保存或发送<br />
        /// Formatted as a string with exception text for saving or sending
        /// </summary>
        /// <param name="text">文本消息</param>
        /// <param name="ex">异常</param>
        /// <returns>异常最终信息</returns>
        public static string GetSaveStringFromException(string text, Exception ex)
        {
            StringBuilder stringBuilder1 = new StringBuilder(text);
            if (ex != null)
            {
                if (!string.IsNullOrEmpty(text))
                    stringBuilder1.Append(" : ");
                try
                {
                    stringBuilder1.Append(StringResources.Language.ExceptionMessage);
                    stringBuilder1.Append(ex.Message);
                    stringBuilder1.Append(Environment.NewLine);
                    stringBuilder1.Append(StringResources.Language.ExceptionSource);
                    stringBuilder1.Append(ex.Source);
                    stringBuilder1.Append(Environment.NewLine);
                    stringBuilder1.Append(StringResources.Language.ExceptionStackTrace);
                    stringBuilder1.Append(ex.StackTrace);
                    stringBuilder1.Append(Environment.NewLine);
                    stringBuilder1.Append(StringResources.Language.ExceptionType);
                    stringBuilder1.Append(ex.GetType().ToString());
                    stringBuilder1.Append(Environment.NewLine);
                    stringBuilder1.Append(StringResources.Language.ExceptionTargetSite);
                    StringBuilder stringBuilder2 = stringBuilder1;
                    MethodBase targetSite = ex.TargetSite;
                    string str = (object)targetSite != null ? targetSite.ToString() : (string)null;
                    stringBuilder2.Append(str);
                }
                catch
                {
                }
                stringBuilder1.Append(Environment.NewLine);
                stringBuilder1.Append("\x0002/=================================================[    Exception    ]================================================/");
            }
            return stringBuilder1.ToString();
        }
    }
}
