// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.BasicFramework.SoftFileSaveBase
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Core;
using ESTCore.Common.LogNet;

using System;
using System.IO;
using System.Text;

namespace ESTCore.Common.BasicFramework
{
    /// <summary>文件存储功能的基类，包含了文件存储路径，存储方法等</summary>
    /// <remarks>
    /// 需要继承才能实现你想存储的数据，比较经典的例子就是存储你的应用程序的配置信息，通常的格式就是xml文件或是json文件。具体请看例子：
    /// </remarks>
    /// <example>
    /// 下面举例实现两个字段的普通数据存储
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftFileSaveBaseExample.cs" region="SoftFileSaveBase1" title="简单示例" />
    /// 然后怎么调用呢？
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftFileSaveBaseExample.cs" region="Example" title="调用示例" />
    /// 如果你想实现加密存储，这样就不用关心被用户看到了。
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\BasicFramework\SoftFileSaveBaseExample.cs" region="SoftFileSaveBase2" title="加密示例" />
    /// 如果还是担心被反编译获取数据，那么这个密钥就要来自服务器的数据，本地不做存储。
    /// </example>
    public class SoftFileSaveBase : ISoftFileSaveBase
    {
        private SimpleHybirdLock HybirdLock;

        /// <summary>实例化一个文件存储的基类</summary>
        public SoftFileSaveBase() => this.HybirdLock = new SimpleHybirdLock();

        /// <summary>在日志保存时的标记当前调用类的信息</summary>
        protected string LogHeaderText { get; set; }

        /// <inheritdoc cref="M:ESTCore.Common.BasicFramework.ISoftFileSaveBase.ToSaveString" />
        public virtual string ToSaveString() => string.Empty;

        /// <inheritdoc cref="M:ESTCore.Common.BasicFramework.ISoftFileSaveBase.LoadByString(System.String)" />
        public virtual void LoadByString(string content)
        {
        }

        /// <inheritdoc cref="M:ESTCore.Common.BasicFramework.ISoftFileSaveBase.LoadByFile" />
        public virtual void LoadByFile() => this.LoadByFile((Converter<string, string>)(m => m));

        /// <summary>使用用户自定义的解密方法从文件读取数据</summary>
        /// <param name="decrypt">用户自定义的解密方法</param>
        public void LoadByFile(Converter<string, string> decrypt)
        {
            if (!(this.FileSavePath != "") || !File.Exists(this.FileSavePath))
                return;
            this.HybirdLock.Enter();
            try
            {
                using (StreamReader streamReader = new StreamReader(this.FileSavePath, Encoding.Default))
                    this.LoadByString(decrypt(streamReader.ReadToEnd()));
            }
            catch (Exception ex)
            {
                this.ILogNet?.WriteException(this.LogHeaderText, StringResources.Language.FileLoadFailed, ex);
            }
            finally
            {
                this.HybirdLock.Leave();
            }
        }

        /// <inheritdoc cref="M:ESTCore.Common.BasicFramework.ISoftFileSaveBase.SaveToFile" />
        public virtual void SaveToFile() => this.SaveToFile((Converter<string, string>)(m => m));

        /// <summary>使用用户自定义的加密方法保存数据到文件</summary>
        /// <param name="encrypt">用户自定义的加密方法</param>
        public void SaveToFile(Converter<string, string> encrypt)
        {
            if (!(this.FileSavePath != ""))
                return;
            this.HybirdLock.Enter();
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(this.FileSavePath, false, Encoding.Default))
                {
                    streamWriter.Write(encrypt(this.ToSaveString()));
                    streamWriter.Flush();
                }
            }
            catch (Exception ex)
            {
                this.ILogNet?.WriteException(this.LogHeaderText, StringResources.Language.FileSaveFailed, ex);
            }
            finally
            {
                this.HybirdLock.Leave();
            }
        }

        /// <summary>文件存储的路径</summary>
        public string FileSavePath { get; set; }

        /// <summary>日志记录类</summary>
        public ILogNet ILogNet { get; set; }
    }
}
