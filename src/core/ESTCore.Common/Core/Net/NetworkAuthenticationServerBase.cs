// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Core.Net.NetworkAuthenticationServerBase
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace ESTCore.Common.Core.Net
{
    /// <summary>
    /// 带登录认证的服务器类，可以对连接的客户端进行筛选，放行用户名密码正确的连接<br />
    /// Server class with login authentication, which can filter connected clients and allow connections with correct username and password
    /// </summary>
    public class NetworkAuthenticationServerBase : NetworkServerBase, IDisposable
    {
        private Dictionary<string, string> accounts = new Dictionary<string, string>();
        private SimpleHybirdLock lockLoginAccount = new SimpleHybirdLock();
        private bool disposedValue = false;

        /// <summary>
        /// 当客户端的socket登录的时候额外检查的信息，检查当前会话的用户名和密码<br />
        /// Additional check information when the client's socket logs in, check the username and password of the current session
        /// </summary>
        /// <param name="socket">套接字</param>
        /// <param name="endPoint">终结点</param>
        /// <returns>验证的结果</returns>
        protected override OperateResult SocketAcceptExtraCheck(
          Socket socket,
          IPEndPoint endPoint)
        {
            if (this.IsUseAccountCertificate)
            {
                OperateResult<byte[], byte[]> andCheckBytes = this.ReceiveAndCheckBytes(socket, 2000);
                if (!andCheckBytes.IsSuccess)
                    return new OperateResult(string.Format("Client login failed[{0}]", (object)endPoint));
                if (BitConverter.ToInt32(andCheckBytes.Content1, 0) != 5)
                {
                    this.LogNet?.WriteError(this.ToString(), StringResources.Language.NetClientAccountTimeout);
                    socket?.Close();
                    return new OperateResult(string.Format("Authentication failed[{0}]", (object)endPoint));
                }
                string[] strArray = EstProtocol.UnPackStringArrayFromByte(andCheckBytes.Content2);
                string str = this.CheckAccountLegal(strArray);
                this.SendStringAndCheckReceive(socket, str == "success" ? 1 : 0, new string[1]
                {
          str
                });
                if (str != "success")
                    return new OperateResult(string.Format("Client login failed[{0}]:{1} {2}", (object)endPoint, (object)str, (object)SoftBasic.ArrayFormat<string>(strArray)));
                this.LogNet?.WriteDebug(this.ToString(), string.Format("Account Login:{0} Endpoint:[{1}]", (object)strArray[0], (object)endPoint));
            }
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 获取或设置是否对客户端启动账号认证<br />
        /// Gets or sets whether to enable account authentication on the client
        /// </summary>
        public bool IsUseAccountCertificate { get; set; }

        /// <summary>
        /// 新增账户，如果想要启动账户登录，必须将<see cref="P:ESTCore.Common.Core.Net.NetworkAuthenticationServerBase.IsUseAccountCertificate" />设置为<c>True</c>。<br />
        /// Add an account. If you want to activate account login, you must set <see cref="P:ESTCore.Common.Core.Net.NetworkAuthenticationServerBase.IsUseAccountCertificate" /> to <c> True </c>
        /// </summary>
        /// <param name="userName">账户名称</param>
        /// <param name="password">账户名称</param>
        public void AddAccount(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName))
                return;
            this.lockLoginAccount.Enter();
            if (this.accounts.ContainsKey(userName))
                this.accounts[userName] = password;
            else
                this.accounts.Add(userName, password);
            this.lockLoginAccount.Leave();
        }

        /// <summary>
        /// 删除一个账户的信息<br />
        /// Delete an account's information
        /// </summary>
        /// <param name="userName">账户名称</param>
        public void DeleteAccount(string userName)
        {
            this.lockLoginAccount.Enter();
            if (this.accounts.ContainsKey(userName))
                this.accounts.Remove(userName);
            this.lockLoginAccount.Leave();
        }

        private string CheckAccountLegal(string[] infos)
        {
            if (infos != null && infos.Length < 2)
                return "User Name input wrong";
            this.lockLoginAccount.Enter();
            string str = this.accounts.ContainsKey(infos[0]) ? (!(this.accounts[infos[0]] != infos[1]) ? "success" : "Password is not corrent") : "User Name input wrong";
            this.lockLoginAccount.Leave();
            return str;
        }

        /// <summary>释放当前的对象</summary>
        /// <param name="disposing">是否托管对象</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposedValue)
                return;
            if (disposing)
            {
                this.ServerClose();
                this.lockLoginAccount?.Dispose();
            }
            this.disposedValue = true;
        }

        /// <inheritdoc cref="M:System.IDisposable.Dispose" />
        public void Dispose() => this.Dispose(true);

        /// <inheritdoc />
        public override string ToString() => string.Format("NetworkAuthenticationServerBase[{0}]", (object)this.Port);
    }
}
