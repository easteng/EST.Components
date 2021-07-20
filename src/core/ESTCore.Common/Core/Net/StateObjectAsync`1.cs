// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Core.Net.StateObjectAsync`1
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using System.Threading.Tasks;

namespace ESTCore.Common.Core.Net
{
    /// <summary>携带TaskCompletionSource属性的异步对象</summary>
    /// <typeparam name="T">类型</typeparam>
    internal class StateObjectAsync<T> : StateObject
    {
        /// <summary>实例化一个对象</summary>
        public StateObjectAsync()
        {
        }

        /// <summary>实例化一个对象，指定接收或是发送的数据长度</summary>
        /// <param name="length">数据长度</param>
        public StateObjectAsync(int length)
          : base(length)
        {
        }

        public TaskCompletionSource<T> Tcs { get; set; }
    }
}
