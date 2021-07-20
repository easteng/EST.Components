// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Core.EstAsyncCoordinator
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using System;
using System.Threading;

namespace ESTCore.Common.Core
{
    /// <summary>一个用于高性能，乐观并发模型控制操作的类，允许一个方法(隔离方法)的安全单次执行</summary>
    public sealed class EstAsyncCoordinator
    {
        private Action action = (Action)null;
        private int OperaterStatus = 0;
        private long Target = 0;

        /// <summary>实例化一个对象，需要传入隔离执行的方法</summary>
        /// <param name="operater">隔离执行的方法</param>
        public EstAsyncCoordinator(Action operater) => this.action = operater;

        /// <summary>启动线程池执行隔离方法</summary>
        public void StartOperaterInfomation()
        {
            Interlocked.Increment(ref this.Target);
            if (Interlocked.CompareExchange(ref this.OperaterStatus, 1, 0) != 0)
                return;
            ThreadPool.QueueUserWorkItem(new WaitCallback(this.ThreadPoolOperater), (object)null);
        }

        private void ThreadPoolOperater(object obj)
        {
            long num1 = this.Target;
            long num2 = 0;
            long comparand;
            do
            {
                comparand = num1;
                Action action = this.action;
                if (action != null)
                    action();
                num1 = Interlocked.CompareExchange(ref this.Target, num2, comparand);
            }
            while (comparand != num1);
            Interlocked.Exchange(ref this.OperaterStatus, 0);
            if (this.Target == num2)
                return;
            this.StartOperaterInfomation();
        }

        /// <inheritdoc />
        public override string ToString() => nameof(EstAsyncCoordinator);
    }
}
