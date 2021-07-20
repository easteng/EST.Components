// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.BasicFramework.Exception`1
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ESTCore.Common.BasicFramework
{
    /// <summary>一个自定义的支持序列化反序列化的异常类，具体用法参照第四版《CLR Via C#》P414</summary>
    /// <typeparam name="TExceptionArgs">泛型异常</typeparam>
    [Serializable]
    public sealed class Exception<TExceptionArgs> : Exception, ISerializable where TExceptionArgs : ExceptionArgs
    {
        /// <summary>用于反序列化的</summary>
        private const string c_args = "Args";
        private readonly TExceptionArgs m_args;

        /// <summary>消息</summary>
        public TExceptionArgs Args => this.m_args;

        /// <summary>实例化一个异常对象</summary>
        /// <param name="message">消息</param>
        /// <param name="innerException">内部异常类</param>
        public Exception(string message = null, Exception innerException = null)
          : this(default(TExceptionArgs), message, innerException)
        {
        }

        /// <summary>实例化一个异常对象</summary>
        /// <param name="args">异常消息</param>
        /// <param name="message">消息</param>
        /// <param name="innerException">内部异常类</param>
        public Exception(TExceptionArgs args, string message = null, Exception innerException = null)
          : base(message, innerException)
          => this.m_args = args;

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        private Exception(SerializationInfo info, StreamingContext context)
          : base(info, context)
          => this.m_args = (TExceptionArgs)info.GetValue(nameof(Args), typeof(TExceptionArgs));

        /// <summary>获取存储对象的序列化数据</summary>
        /// <param name="info">序列化的信息</param>
        /// <param name="context">流的上下文</param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Args", (object)this.m_args);
            base.GetObjectData(info, context);
        }

        /// <summary>获取描述当前异常的消息</summary>
        public override string Message
        {
            get
            {
                string message = base.Message;
                return (object)this.m_args == null ? message : message + " (" + this.m_args.Message + ")";
            }
        }

        /// <summary>确定指定的object是否等于当前的object</summary>
        /// <param name="obj">异常对象</param>
        /// <returns>是否一致</returns>
        public override bool Equals(object obj) => obj is Exception<TExceptionArgs> exception && (object.Equals((object)this.m_args, (object)exception.m_args) && base.Equals(obj));

        /// <inheritdoc />
        public override int GetHashCode() => base.GetHashCode();
    }
}
