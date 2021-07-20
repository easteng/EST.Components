// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Core.Net.AlienSession
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;

using System;
using System.Net.Sockets;
using System.Text;

namespace ESTCore.Common.Core.Net
{
    /// <summary>异形客户端的连接对象</summary>
    public class AlienSession
    {
        /// <summary>实例化一个默认的参数</summary>
        public AlienSession()
        {
            this.IsStatusOk = true;
            this.OnlineTime = DateTime.Now;
            this.OfflineTime = DateTime.MinValue;
        }

        /// <summary>网络套接字</summary>
        public Socket Socket { get; set; }

        /// <summary>唯一的标识</summary>
        public string DTU { get; set; }

        /// <summary>密码信息</summary>
        public string Pwd { get; set; }

        /// <summary>指示当前的网络状态</summary>
        public bool IsStatusOk { get; set; }

        /// <summary>上线时间</summary>
        public DateTime OnlineTime { get; set; }

        /// <summary>最后一次下线的时间</summary>
        public DateTime OfflineTime { get; set; }

        /// <summary>进行下线操作</summary>
        public void Offline()
        {
            if (!this.IsStatusOk)
                return;
            this.IsStatusOk = false;
            this.OfflineTime = DateTime.Now;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("DtuSession[" + this.DTU + "] [" + (this.IsStatusOk ? "Online" : "Offline") + "]");
            if (this.IsStatusOk)
                stringBuilder.Append(" [" + SoftBasic.GetTimeSpanDescription(DateTime.Now - this.OnlineTime) + "]");
            else if (this.OfflineTime == DateTime.MinValue)
                stringBuilder.Append(" [----]");
            else
                stringBuilder.Append(" [" + SoftBasic.GetTimeSpanDescription(DateTime.Now - this.OfflineTime) + "]");
            return stringBuilder.ToString();
        }
    }
}
