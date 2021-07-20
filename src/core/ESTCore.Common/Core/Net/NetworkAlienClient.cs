// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Core.Net.NetworkAlienClient
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Core.IMessage;
using ESTCore.Common.LogNet;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ESTCore.Common.Core.Net
{
    /// <summary>
    /// 异形客户端的基类，提供了基础的异形操作<br />
    /// The base class of the profiled client provides the basic profiled operation
    /// </summary>
    public class NetworkAlienClient : NetworkServerBase
    {
        private byte[] password;
        private List<string> trustOnline;
        private SimpleHybirdLock trustLock;
        private bool isResponseAck = true;
        private bool isCheckPwd = true;
        /// <summary>状态登录成功</summary>
        public const byte StatusOk = 0;
        /// <summary>重复登录</summary>
        public const byte StatusLoginRepeat = 1;
        /// <summary>禁止登录</summary>
        public const byte StatusLoginForbidden = 2;
        /// <summary>密码错误</summary>
        public const byte StatusPasswodWrong = 3;

        /// <summary>
        /// 默认的无参构造方法<br />
        /// The default parameterless constructor
        /// </summary>
        public NetworkAlienClient()
        {
            this.password = new byte[6];
            this.trustOnline = new List<string>();
            this.trustLock = new SimpleHybirdLock();
        }

        /// <summary>
        /// 当接收到了新的请求的时候执行的操作<br />
        /// An action performed when a new request is received
        /// </summary>
        /// <param name="socket">异步对象</param>
        /// <param name="endPoint">终结点</param>
        protected override async void ThreadPoolLogin(Socket socket, IPEndPoint endPoint)
        {
            OperateResult<byte[]> check = await this.ReceiveByMessageAsync(socket, 5000, (INetMessage)new AlienMessage());
            string dtu;
            AlienSession session;
            if (!check.IsSuccess)
            {
                check = (OperateResult<byte[]>)null;
                dtu = (string)null;
                session = (AlienSession)null;
            }
            else
            {
                byte[] content = check.Content;
                if (content != null && content.Length < 22)
                {
                    Socket socket1 = socket;
                    if (socket1 == null)
                    {
                        check = (OperateResult<byte[]>)null;
                        dtu = (string)null;
                        session = (AlienSession)null;
                    }
                    else
                    {
                        socket1.Close();
                        check = (OperateResult<byte[]>)null;
                        dtu = (string)null;
                        session = (AlienSession)null;
                    }
                }
                else
                {
                    dtu = Encoding.ASCII.GetString(check.Content, 5, 11).Trim(char.MinValue, ' ');
                    bool isPasswrodRight = true;
                    if (this.isCheckPwd)
                    {
                        for (int i = 0; i < this.password.Length; ++i)
                        {
                            if ((int)check.Content[16 + i] != (int)this.password[i])
                            {
                                isPasswrodRight = false;
                                break;
                            }
                        }
                    }
                    if (!isPasswrodRight)
                    {
                        if (this.isResponseAck)
                        {
                            OperateResult send = this.Send(socket, this.GetResponse((byte)3));
                            if (send.IsSuccess)
                                socket?.Close();
                            send = (OperateResult)null;
                        }
                        else
                            socket?.Close();
                        ILogNet logNet = this.LogNet;
                        if (logNet == null)
                        {
                            check = (OperateResult<byte[]>)null;
                            dtu = (string)null;
                            session = (AlienSession)null;
                        }
                        else
                        {
                            logNet.WriteWarn(this.ToString(), "Login Password Wrong, Id:" + dtu);
                            check = (OperateResult<byte[]>)null;
                            dtu = (string)null;
                            session = (AlienSession)null;
                        }
                    }
                    else
                    {
                        session = new AlienSession()
                        {
                            DTU = dtu,
                            Socket = socket,
                            IsStatusOk = true,
                            Pwd = check.Content.SelectMiddle<byte>(16, 6).ToHexString()
                        };
                        if (!this.IsClientPermission(session))
                        {
                            if (this.isResponseAck)
                            {
                                OperateResult send = this.Send(socket, this.GetResponse((byte)2));
                                if (send.IsSuccess)
                                    socket?.Close();
                                send = (OperateResult)null;
                            }
                            else
                                socket?.Close();
                            ILogNet logNet = this.LogNet;
                            if (logNet == null)
                            {
                                check = (OperateResult<byte[]>)null;
                                dtu = (string)null;
                                session = (AlienSession)null;
                            }
                            else
                            {
                                logNet.WriteWarn(this.ToString(), "Login Forbidden, Id:" + session.DTU);
                                check = (OperateResult<byte[]>)null;
                                dtu = (string)null;
                                session = (AlienSession)null;
                            }
                        }
                        else
                        {
                            int status = this.IsClientOnline(session);
                            if ((uint)status > 0U)
                            {
                                if (this.isResponseAck)
                                {
                                    OperateResult send = this.Send(socket, this.GetResponse((byte)1));
                                    if (send.IsSuccess)
                                        socket?.Close();
                                    send = (OperateResult)null;
                                }
                                else
                                    socket?.Close();
                                ILogNet logNet = this.LogNet;
                                if (logNet == null)
                                {
                                    check = (OperateResult<byte[]>)null;
                                    dtu = (string)null;
                                    session = (AlienSession)null;
                                }
                                else
                                {
                                    logNet.WriteWarn(this.ToString(), NetworkAlienClient.GetMsgFromCode(session.DTU, status));
                                    check = (OperateResult<byte[]>)null;
                                    dtu = (string)null;
                                    session = (AlienSession)null;
                                }
                            }
                            else
                            {
                                if (this.isResponseAck)
                                {
                                    OperateResult send = this.Send(socket, this.GetResponse((byte)0));
                                    if (!send.IsSuccess)
                                    {
                                        check = (OperateResult<byte[]>)null;
                                        dtu = (string)null;
                                        session = (AlienSession)null;
                                        return;
                                    }
                                    send = (OperateResult)null;
                                }
                                this.LogNet?.WriteWarn(this.ToString(), NetworkAlienClient.GetMsgFromCode(session.DTU, status));
                                NetworkAlienClient.OnClientConnectedDelegate onClientConnected = this.OnClientConnected;
                                if (onClientConnected == null)
                                {
                                    check = (OperateResult<byte[]>)null;
                                    dtu = (string)null;
                                    session = (AlienSession)null;
                                }
                                else
                                {
                                    onClientConnected(session);
                                    check = (OperateResult<byte[]>)null;
                                    dtu = (string)null;
                                    session = (AlienSession)null;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 是否返回响应，默认为 <c>True</c><br />
        /// The default is <c>True</c>
        /// </summary>
        public bool IsResponseAck
        {
            get => this.isResponseAck;
            set => this.isResponseAck = value;
        }

        /// <summary>
        /// 是否统一检查密码，如果每个会话需要自己检查密码，就需要设置为false<br />
        /// Whether to check the password uniformly, if each session needs to check the password by itself, it needs to be set to false
        /// </summary>
        public bool IsCheckPwd
        {
            get => this.isCheckPwd;
            set => this.isCheckPwd = value;
        }

        /// <summary>
        /// 当有服务器连接上来的时候触发<br />
        /// Triggered when a server is connected
        /// </summary>
        public event NetworkAlienClient.OnClientConnectedDelegate OnClientConnected = null;

        /// <summary>获取返回的命令信息</summary>
        /// <param name="status">状态</param>
        /// <returns>回发的指令信息</returns>
        private byte[] GetResponse(byte status) => new byte[6]
        {
      (byte) 72,
      (byte) 115,
      (byte) 110,
      (byte) 0,
      (byte) 1,
      status
        };

        /// <summary>检测当前的DTU是否在线</summary>
        /// <param name="session">当前的会话信息</param>
        /// <returns>当前的会话是否在线</returns>
        public virtual int IsClientOnline(AlienSession session) => 0;

        /// <summary>检测当前的dtu是否允许登录</summary>
        /// <param name="session">当前的会话信息</param>
        /// <returns>当前的id是否可允许登录</returns>
        private bool IsClientPermission(AlienSession session)
        {
            bool flag = false;
            this.trustLock.Enter();
            if (this.trustOnline.Count == 0)
            {
                flag = true;
            }
            else
            {
                for (int index = 0; index < this.trustOnline.Count; ++index)
                {
                    if (this.trustOnline[index] == session.DTU)
                    {
                        flag = true;
                        break;
                    }
                }
            }
            this.trustLock.Leave();
            return flag;
        }

        /// <summary>
        /// 设置密码，需要传入长度为6的字节数组<br />
        /// To set the password, you need to pass in an array of bytes of length 6
        /// </summary>
        /// <param name="password">密码信息</param>
        public void SetPassword(byte[] password)
        {
            if (password == null || password.Length != 6)
                return;
            password.CopyTo((Array)this.password, 0);
        }

        /// <summary>
        /// 设置可信任的客户端列表，传入一个DTU的列表信息<br />
        /// Set up the list of trusted clients, passing in the list information for a DTU
        /// </summary>
        /// <param name="clients">客户端列表</param>
        public void SetTrustClients(string[] clients) => this.trustOnline = new List<string>((IEnumerable<string>)clients);

        /// <inheritdoc />
        public override string ToString() => "NetworkAlienBase";

        /// <summary>获取错误的描述信息</summary>
        /// <param name="dtu">dtu信息</param>
        /// <param name="code">错误码</param>
        /// <returns>错误信息</returns>
        public static string GetMsgFromCode(string dtu, int code)
        {
            switch (code)
            {
                case 0:
                    return "Login Success, Id:" + dtu;
                case 1:
                    return "Login Repeat, Id:" + dtu;
                case 2:
                    return "Login Forbidden, Id:" + dtu;
                case 3:
                    return "Login Passwod Wrong, Id:" + dtu;
                default:
                    return "Login Unknow reason, Id:" + dtu;
            }
        }

        /// <summary>客户上线的委托事件</summary>
        /// <param name="session">异形客户端的会话信息</param>
        public delegate void OnClientConnectedDelegate(AlienSession session);
    }
}
