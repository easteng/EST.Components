// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.WebSocket.WebSocketSession
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace ESTCore.Common.WebSocket
{
    /// <summary>websocket 的会话客户端</summary>
    public class WebSocketSession
    {
        private object objLock = new object();

        /// <summary>实例化一个默认的对象</summary>
        public WebSocketSession()
        {
            this.Topics = new List<string>();
            this.ActiveTime = DateTime.Now;
            this.OnlineTime = DateTime.Now;
        }

        /// <summary>当前客户端的激活时间</summary>
        public DateTime ActiveTime { get; set; }

        /// <summary>获取当前的客户端的上线时间</summary>
        public DateTime OnlineTime { get; private set; }

        /// <summary>当前客户端绑定的套接字对象</summary>
        internal Socket WsSocket { get; set; }

        /// <summary>当前客户端订阅的所有的Topic信息</summary>
        public List<string> Topics { get; set; }

        /// <summary>远程的客户端的ip及端口信息</summary>
        public IPEndPoint Remote { get; set; }

        /// <summary>当前的会话是否是问答客户端，如果是问答客户端的话，数据的推送是无效的。</summary>
        public bool IsQASession { get; set; }

        /// <summary>客户端请求的url信息，可能携带一些参数信息</summary>
        public string Url { get; set; }

        /// <summary>检查当前的连接对象是否在</summary>
        /// <param name="topic">主题信息</param>
        /// <returns>是否包含的结果信息</returns>
        public bool IsClientSubscribe(string topic)
        {
            bool flag = false;
            lock (this.objLock)
                flag = this.Topics.Contains(topic);
            return flag;
        }

        /// <summary>动态增加一个订阅的信息</summary>
        /// <param name="topic">订阅的主题</param>
        public void AddTopic(string topic)
        {
            lock (this.objLock)
            {
                if (this.Topics.Contains(topic))
                    return;
                this.Topics.Add(topic);
            }
        }

        /// <summary>动态移除一个订阅的信息</summary>
        /// <param name="topic">订阅的主题</param>
        public bool RemoveTopic(string topic)
        {
            bool flag = false;
            lock (this.objLock)
                flag = this.Topics.Remove(topic);
            return flag;
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("WebSocketSession[{0}][{1}]", (object)this.Remote, (object)SoftBasic.GetTimeSpanDescription(DateTime.Now - this.OnlineTime));
    }
}
