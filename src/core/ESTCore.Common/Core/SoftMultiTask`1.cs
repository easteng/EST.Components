// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Core.SoftMultiTask`1
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using System;
using System.Threading;

namespace ESTCore.Common.Core
{
    /// <summary>一个用于多线程并发处理数据的模型类，适用于处理数据量非常庞大的情况</summary>
    /// <typeparam name="T">等待处理的数据类型</typeparam>
    public sealed class SoftMultiTask<T>
    {
        /// <summary>操作总数，判定操作是否完成</summary>
        private int m_opCount = 0;
        /// <summary>判断是否所有的线程是否处理完成</summary>
        private int m_opThreadCount = 1;
        /// <summary>准备启动的处理数据的线程数量</summary>
        private int m_threadCount = 10;
        /// <summary>指示多线程处理是否在运行中，防止冗余调用</summary>
        private int m_runStatus = 0;
        /// <summary>列表数据</summary>
        private T[] m_dataList = (T[])null;
        /// <summary>需要操作的方法</summary>
        private Func<T, bool> m_operater = (Func<T, bool>)null;
        /// <summary>已处理完成数量，无论是否异常</summary>
        private int m_finishCount = 0;
        /// <summary>处理完成并实现操作数量</summary>
        private int m_successCount = 0;
        /// <summary>处理过程中异常数量</summary>
        private int m_failedCount = 0;
        /// <summary>用于触发事件的混合线程锁</summary>
        private SimpleHybirdLock HybirdLock = new SimpleHybirdLock();
        /// <summary>指示处理状态是否为暂停状态</summary>
        private bool m_isRunningStop = false;
        /// <summary>指示系统是否需要强制退出</summary>
        private bool m_isQuit = false;
        /// <summary>在发生错误的时候是否强制退出后续的操作</summary>
        private bool m_isQuitAfterException = false;

        /// <summary>实例化一个数据处理对象</summary>
        /// <param name="dataList">数据处理列表</param>
        /// <param name="operater">数据操作方法，应该是相对耗时的任务</param>
        /// <param name="threadCount">需要使用的线程数</param>
        public SoftMultiTask(T[] dataList, Func<T, bool> operater, int threadCount = 10)
        {
            this.m_dataList = dataList ?? throw new ArgumentNullException(nameof(dataList));
            this.m_operater = operater ?? throw new ArgumentNullException(nameof(operater));
            this.m_threadCount = threadCount >= 1 ? threadCount : throw new ArgumentException("threadCount can not less than 1", nameof(threadCount));
            Interlocked.Add(ref this.m_opCount, dataList.Length);
            Interlocked.Add(ref this.m_opThreadCount, threadCount);
        }

        /// <summary>异常发生时事件</summary>
        public event SoftMultiTask<T>.MultiInfo OnExceptionOccur;

        /// <summary>报告处理进度时发生</summary>
        public event SoftMultiTask<T>.MultiInfoTwo OnReportProgress;

        /// <summary>启动多线程进行数据处理</summary>
        public void StartOperater()
        {
            if (Interlocked.CompareExchange(ref this.m_runStatus, 0, 1) != 0)
                return;
            for (int index = 0; index < this.m_threadCount; ++index)
                new Thread(new ThreadStart(this.ThreadBackground))
                {
                    IsBackground = true
                }.Start();
            this.JustEnded();
        }

        /// <summary>暂停当前的操作</summary>
        public void StopOperater()
        {
            if (this.m_runStatus != 1)
                return;
            this.m_isRunningStop = true;
        }

        /// <summary>恢复暂停的操作</summary>
        public void ResumeOperater() => this.m_isRunningStop = false;

        /// <summary>直接手动强制结束操作</summary>
        public void EndedOperater()
        {
            if (this.m_runStatus != 1)
                return;
            this.m_isQuit = true;
        }

        /// <summary>在发生错误的时候是否强制退出后续的操作</summary>
        public bool IsQuitAfterException
        {
            get => this.m_isQuitAfterException;
            set => this.m_isQuitAfterException = value;
        }

        private void ThreadBackground()
        {
            while (true)
            {
                do
                    ;
                while (this.m_isRunningStop);
                int index = Interlocked.Decrement(ref this.m_opCount);
                if (index >= 0)
                {
                    T data = this.m_dataList[index];
                    bool flag1 = false;
                    bool flag2 = false;
                    try
                    {
                        if (!this.m_isQuit)
                            flag1 = this.m_operater(data);
                    }
                    catch (Exception ex)
                    {
                        flag2 = true;
                        SoftMultiTask<T>.MultiInfo onExceptionOccur = this.OnExceptionOccur;
                        if (onExceptionOccur != null)
                            onExceptionOccur(data, ex);
                        if (this.m_isQuitAfterException)
                            this.EndedOperater();
                    }
                    finally
                    {
                        this.HybirdLock.Enter();
                        if (flag1)
                            ++this.m_successCount;
                        if (flag2)
                            ++this.m_failedCount;
                        ++this.m_finishCount;
                        SoftMultiTask<T>.MultiInfoTwo onReportProgress = this.OnReportProgress;
                        if (onReportProgress != null)
                            onReportProgress(this.m_finishCount, this.m_dataList.Length, this.m_successCount, this.m_failedCount);
                        this.HybirdLock.Leave();
                    }
                }
                else
                    break;
            }
            this.JustEnded();
        }

        private void JustEnded()
        {
            if (Interlocked.Decrement(ref this.m_opThreadCount) != 0)
                return;
            this.m_finishCount = 0;
            this.m_failedCount = 0;
            this.m_successCount = 0;
            Interlocked.Exchange(ref this.m_opCount, this.m_dataList.Length);
            Interlocked.Exchange(ref this.m_opThreadCount, this.m_threadCount + 1);
            Interlocked.Exchange(ref this.m_runStatus, 0);
            this.m_isRunningStop = false;
            this.m_isQuit = false;
        }

        /// <summary>一个双参数委托</summary>
        /// <param name="item"></param>
        /// <param name="ex"></param>
        public delegate void MultiInfo(T item, Exception ex);

        /// <summary>用于报告进度的委托，当finish等于count时，任务完成</summary>
        /// <param name="finish">已完成操作数量</param>
        /// <param name="count">总数量</param>
        /// <param name="success">成功数量</param>
        /// <param name="failed">失败数量</param>
        public delegate void MultiInfoTwo(int finish, int count, int success, int failed);
    }
}
