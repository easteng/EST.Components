// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Enthernet.PushGroupClient
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Core;
using ESTCore.Common.Core.Net;

using System;
using System.Collections.Generic;
using System.Threading;

namespace ESTCore.Common.Enthernet
{
    /// <summary>订阅分类的核心组织对象</summary>
    public class PushGroupClient : IDisposable
    {
        private List<AppSession> appSessions;
        private SimpleHybirdLock simpleHybird;
        private long pushTimesCount = 0;
        private bool disposedValue = false;

        /// <summary>实例化一个默认的对象</summary>
        public PushGroupClient()
        {
            this.appSessions = new List<AppSession>();
            this.simpleHybird = new SimpleHybirdLock();
        }

        /// <summary>新增一个订阅的会话</summary>
        /// <param name="session">会话</param>
        public void AddPushClient(AppSession session)
        {
            this.simpleHybird.Enter();
            this.appSessions.Add(session);
            this.simpleHybird.Leave();
        }

        /// <summary>移除一个订阅的会话</summary>
        /// <param name="clientID">客户端唯一的ID信息</param>
        public bool RemovePushClient(string clientID)
        {
            bool flag = false;
            this.simpleHybird.Enter();
            for (int index = 0; index < this.appSessions.Count; ++index)
            {
                if (this.appSessions[index].ClientUniqueID == clientID)
                {
                    this.appSessions[index].WorkSocket?.Close();
                    this.appSessions.RemoveAt(index);
                    flag = true;
                    break;
                }
            }
            this.simpleHybird.Leave();
            return flag;
        }

        /// <summary>使用固定的发送方法将数据发送出去</summary>
        /// <param name="content">数据内容</param>
        /// <param name="send">指定的推送方法</param>
        public void PushString(string content, Action<AppSession, string> send)
        {
            this.simpleHybird.Enter();
            Interlocked.Increment(ref this.pushTimesCount);
            for (int index = 0; index < this.appSessions.Count; ++index)
                send(this.appSessions[index], content);
            this.simpleHybird.Leave();
        }

        /// <summary>移除并关闭所有的客户端</summary>
        public int RemoveAllClient()
        {
            this.simpleHybird.Enter();
            for (int index = 0; index < this.appSessions.Count; ++index)
                this.appSessions[index].WorkSocket?.Close();
            int count = this.appSessions.Count;
            this.appSessions.Clear();
            this.simpleHybird.Leave();
            return count;
        }

        /// <summary>获取是否推送过数据</summary>
        /// <returns>True代表有，False代表没有</returns>
        public bool HasPushedContent() => this.pushTimesCount > 0L;

        /// <summary>释放当前的程序所占用的资源</summary>
        /// <param name="disposing">是否释放资源</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposedValue)
                return;
            if (!disposing)
                ;
            this.simpleHybird.Enter();
            this.appSessions.ForEach((Action<AppSession>)(m => m.WorkSocket?.Close()));
            this.appSessions.Clear();
            this.simpleHybird.Leave();
            this.simpleHybird.Dispose();
            this.disposedValue = true;
        }

        /// <summary>释放当前的对象所占用的资源</summary>
        public void Dispose() => this.Dispose(true);

        /// <inheritdoc />
        public override string ToString() => nameof(PushGroupClient);
    }
}
