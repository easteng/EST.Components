// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Core.Net.NetworkDataServerBase
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;
using ESTCore.Common.Reflection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESTCore.Common.Core.Net
{
    /// <summary>
    /// 所有虚拟的数据服务器的基类，提供了基本的数据读写，存储加载的功能方法，具体的字节读写需要继承重写。<br />
    /// The base class of all virtual data servers provides basic methods for reading and writing data and storing and loading.
    /// Specific byte reads and writes need to be inherited and override.
    /// </summary>
    public class NetworkDataServerBase : NetworkAuthenticationServerBase, IDisposable, IReadWriteNet
    {
        private List<string> TrustedClients = (List<string>)null;
        private bool IsTrustedClientsOnly = false;
        private SimpleHybirdLock lock_trusted_clients;
        private List<AppSession> listsOnlineClient;
        private object lockOnlineClient;
        private int onlineCount = 0;
        private System.Threading.Timer timerHeart;

        /// <summary>
        /// 实例化一个默认的数据服务器的对象<br />
        /// Instantiate an object of the default data server
        /// </summary>
        public NetworkDataServerBase()
        {
            this.ActiveTimeSpan = TimeSpan.FromHours(24.0);
            this.lock_trusted_clients = new SimpleHybirdLock();
            this.ConnectionId = SoftBasic.GetUniqueStringByGuidAndRandom();
            this.lockOnlineClient = new object();
            this.listsOnlineClient = new List<AppSession>();
            this.timerHeart = new System.Threading.Timer(new TimerCallback(this.ThreadTimerHeartCheck), (object)null, 2000, 10000);
        }

        /// <summary>
        /// 将本系统的数据池数据存储到指定的文件<br />
        /// Store the data pool data of this system to the specified file
        /// </summary>
        /// <param name="path">指定文件的路径</param>
        /// <exception cref="T:System.ArgumentException"></exception>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.IO.PathTooLongException"></exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException"></exception>
        /// <exception cref="T:System.IO.IOException"></exception>
        /// <exception cref="T:System.UnauthorizedAccessException"></exception>
        /// <exception cref="T:System.NotSupportedException"></exception>
        /// <exception cref="T:System.Security.SecurityException"></exception>
        public void SaveDataPool(string path)
        {
            byte[] bytes = this.SaveToBytes();
            System.IO.File.WriteAllBytes(path, bytes);
        }

        /// <summary>
        /// 从文件加载数据池信息<br />
        /// Load datapool information from a file
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <exception cref="T:System.ArgumentException"></exception>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.IO.PathTooLongException"></exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException"></exception>
        /// <exception cref="T:System.IO.IOException"></exception>
        /// <exception cref="T:System.UnauthorizedAccessException"></exception>
        /// <exception cref="T:System.NotSupportedException"></exception>
        /// <exception cref="T:System.Security.SecurityException"></exception>
        /// <exception cref="T:System.IO.FileNotFoundException"></exception>
        public void LoadDataPool(string path)
        {
            if (!System.IO.File.Exists(path))
                return;
            this.LoadFromBytes(System.IO.File.ReadAllBytes(path));
        }

        /// <summary>
        /// 从字节数据加载数据信息，需要进行重写方法<br />
        /// Loading data information from byte data requires rewriting method
        /// </summary>
        /// <param name="content">字节数据</param>
        protected virtual void LoadFromBytes(byte[] content)
        {
        }

        /// <summary>
        /// 将数据信息存储到字节数组去，需要进行重写方法<br />
        /// To store data information into a byte array, a rewrite method is required
        /// </summary>
        /// <returns>所有的内容</returns>
        protected virtual byte[] SaveToBytes() => new byte[0];

        /// <inheritdoc cref="P:ESTCore.Common.Core.Net.NetworkDoubleBase.ByteTransform" />
        public IByteTransform ByteTransform { get; set; }

        /// <inheritdoc cref="P:ESTCore.Common.Core.IReadWriteNet.ConnectionId" />
        public string ConnectionId { get; set; }

        /// <summary>
        /// 获取或设置当前的服务器是否允许远程客户端进行写入数据操作，默认为<c>True</c><br />
        /// Gets or sets whether the current server allows remote clients to write data, the default is <c>True</c>
        /// </summary>
        /// <remarks>
        /// 如果设置为<c>False</c>，那么所有远程客户端的操作都会失败，直接返回错误码或是关闭连接。
        /// </remarks>
        public bool EnableWrite { get; set; } = true;

        /// <summary>
        /// 接收到数据的时候就触发的事件，示例详细参考API文档信息<br />
        /// An event that is triggered when data is received
        /// </summary>
        /// <remarks>
        /// 事件共有三个参数，sender指服务器本地的对象，例如 <see cref="T:ESTCore.Common.ModBus.ModbusTcpServer" /> 对象，source 指会话对象，网口对象为 <see cref="T:ESTCore.Common.Core.Net.AppSession" />，
        /// 串口为<see cref="T:System.IO.Ports.SerialPort" /> 对象，需要根据实际判断，data 为收到的原始数据 byte[] 对象
        /// </remarks>
        /// <example>
        /// 我们以Modbus的Server为例子，其他的虚拟服务器同理，因为都集成自本服务器对象
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Core\NetworkDataServerBaseSample.cs" region="OnDataReceivedSample" title="数据接收触发的示例" />
        /// </example>
        public event NetworkDataServerBase.DataReceivedDelegate OnDataReceived;

        /// <summary>
        /// 触发一个数据接收的事件信息<br />
        /// Event information that triggers a data reception
        /// </summary>
        /// <param name="source">数据的发送方</param>
        /// <param name="receive">接收数据信息</param>
        protected void RaiseDataReceived(object source, byte[] receive)
        {
            NetworkDataServerBase.DataReceivedDelegate onDataReceived = this.OnDataReceived;
            if (onDataReceived == null)
                return;
            onDataReceived((object)this, source, receive);
        }

        /// <summary>
        /// 数据发送的时候就触发的事件<br />
        /// Events that are triggered when data is sent
        /// </summary>
        public event NetworkDataServerBase.DataSendDelegate OnDataSend;

        /// <summary>
        /// 获取或设置两次数据交互时的最小时间间隔，默认为24小时。<br />
        /// Get or set the minimum time interval between two data interactions, the default is 24 hours.
        /// </summary>
        public TimeSpan ActiveTimeSpan { get; set; }

        /// <summary>
        /// 触发一个数据发送的事件信息<br />
        /// Event information that triggers a data transmission
        /// </summary>
        /// <param name="send">数据内容</param>
        protected void RaiseDataSend(byte[] send)
        {
            NetworkDataServerBase.DataSendDelegate onDataSend = this.OnDataSend;
            if (onDataSend == null)
                return;
            onDataSend((object)this, send);
        }

        /// <inheritdoc cref="P:ESTCore.Common.Core.Net.NetworkDeviceBase.WordLength" />
        protected ushort WordLength { get; set; } = 1;

        /// <summary>
        /// 当客户端登录后，在Ip信息的过滤后，然后触发本方法，进行后续的数据接收，处理，并返回相关的数据信息<br />
        /// When the client logs in, after filtering the IP information, this method is then triggered to perform subsequent data reception,
        /// processing, and return related data information
        /// </summary>
        /// <param name="socket">网络套接字</param>
        /// <param name="endPoint">终端节点</param>
        protected virtual void ThreadPoolLoginAfterClientCheck(Socket socket, IPEndPoint endPoint)
        {
        }

        /// <summary>
        /// 当接收到了新的请求的时候执行的操作，此处进行账户的安全验证<br />
        /// The operation performed when a new request is received, and the account security verification is performed here
        /// </summary>
        /// <param name="socket">异步对象</param>
        /// <param name="endPoint">终结点</param>
        protected override void ThreadPoolLogin(Socket socket, IPEndPoint endPoint)
        {
            string ipAddress = endPoint.Address.ToString();
            if (this.IsTrustedClientsOnly && !this.CheckIpAddressTrusted(ipAddress))
            {
                this.LogNet?.WriteDebug(this.ToString(), string.Format(StringResources.Language.ClientDisableLogin, (object)endPoint));
                socket.Close();
            }
            else
            {
                if (!this.IsUseAccountCertificate)
                    this.LogNet?.WriteDebug(this.ToString(), string.Format(StringResources.Language.ClientOnlineInfo, (object)endPoint));
                this.ThreadPoolLoginAfterClientCheck(socket, endPoint);
            }
        }

        /// <summary>
        /// 设置并启动受信任的客户端登录并读写，如果为null，将关闭对客户端的ip验证<br />
        /// Set and start the trusted client login and read and write, if it is null, the client's IP verification will be turned off
        /// </summary>
        /// <param name="clients">受信任的客户端列表</param>
        public void SetTrustedIpAddress(List<string> clients)
        {
            this.lock_trusted_clients.Enter();
            if (clients != null)
            {
                this.TrustedClients = clients.Select<string, string>((Func<string, string>)(m => IPAddress.Parse(m).ToString())).ToList<string>();
                this.IsTrustedClientsOnly = true;
            }
            else
            {
                this.TrustedClients = new List<string>();
                this.IsTrustedClientsOnly = false;
            }
            this.lock_trusted_clients.Leave();
        }

        /// <summary>
        /// 检查该Ip地址是否是受信任的<br />
        /// Check if the IP address is trusted
        /// </summary>
        /// <param name="ipAddress">Ip地址信息</param>
        /// <returns>是受信任的返回<c>True</c>，否则返回<c>False</c></returns>
        private bool CheckIpAddressTrusted(string ipAddress)
        {
            if (!this.IsTrustedClientsOnly)
                return false;
            bool flag = false;
            this.lock_trusted_clients.Enter();
            for (int index = 0; index < this.TrustedClients.Count; ++index)
            {
                if (this.TrustedClients[index] == ipAddress)
                {
                    flag = true;
                    break;
                }
            }
            this.lock_trusted_clients.Leave();
            return flag;
        }

        /// <summary>
        /// 获取受信任的客户端列表<br />
        /// Get a list of trusted clients
        /// </summary>
        /// <returns>字符串数据信息</returns>
        public string[] GetTrustedClients()
        {
            string[] strArray = new string[0];
            this.lock_trusted_clients.Enter();
            if (this.TrustedClients != null)
                strArray = this.TrustedClients.ToArray();
            this.lock_trusted_clients.Leave();
            return strArray;
        }

        /// <summary>
        /// 获取在线的客户端的数量<br />
        /// Get the number of clients online
        /// </summary>
        public int OnlineCount => this.onlineCount;

        /// <summary>
        /// 获取当前所有在线的客户端信息，包括IP地址和端口号信息<br />
        /// Get all current online client information, including IP address and port number information
        /// </summary>
        public AppSession[] GetOnlineSessions
        {
            get
            {
                lock (this.lockOnlineClient)
                    return this.listsOnlineClient.ToArray();
            }
        }

        /// <summary>
        /// 新增一个在线的客户端信息<br />
        /// Add an online client information
        /// </summary>
        /// <param name="session">会话内容</param>
        protected void AddClient(AppSession session)
        {
            lock (this.lockOnlineClient)
            {
                this.listsOnlineClient.Add(session);
                ++this.onlineCount;
            }
        }

        /// <summary>
        /// 移除一个在线的客户端信息<br />
        /// Remove an online client message
        /// </summary>
        /// <param name="session">会话内容</param>
        /// <param name="reason">下线的原因</param>
        protected void RemoveClient(AppSession session, string reason = "")
        {
            lock (this.lockOnlineClient)
            {
                if (!this.listsOnlineClient.Remove(session))
                    return;
                this.LogNet?.WriteDebug(this.ToString(), string.Format(StringResources.Language.ClientOfflineInfo, (object)session.IpEndPoint) + " " + reason);
                session.WorkSocket?.Close();
                --this.onlineCount;
            }
        }

        /// <summary>关闭之后进行的操作</summary>
        protected override void CloseAction()
        {
            base.CloseAction();
            lock (this.lockOnlineClient)
            {
                for (int index = 0; index < this.listsOnlineClient.Count; ++index)
                    this.listsOnlineClient[index]?.WorkSocket?.Close();
                this.listsOnlineClient.Clear();
            }
        }

        private void ThreadTimerHeartCheck(object obj)
        {
            AppSession[] appSessionArray = (AppSession[])null;
            lock (this.lockOnlineClient)
                appSessionArray = this.listsOnlineClient.ToArray();
            if (appSessionArray == null || (uint)appSessionArray.Length <= 0U)
                return;
            for (int index = 0; index < appSessionArray.Length; ++index)
            {
                if (DateTime.Now - appSessionArray[index].HeartTime > this.ActiveTimeSpan)
                    this.RemoveClient(appSessionArray[index]);
            }
        }

        /// <summary>释放当前的对象</summary>
        /// <param name="disposing">是否托管对象</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                this.lock_trusted_clients?.Dispose();
            base.Dispose(disposing);
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("NetworkDataServerBase[{0}]", (object)this.Port);

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Read(System.String,System.UInt16)" />
        [EstMqttApi("ReadByteArray", "")]
        public virtual OperateResult<byte[]> Read(string address, ushort length) => new OperateResult<byte[]>(StringResources.Language.NotSupportedFunction);

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Write(System.String,System.Byte[])" />
        [EstMqttApi("WriteByteArray", "")]
        public virtual OperateResult Write(string address, byte[] value) => new OperateResult(StringResources.Language.NotSupportedFunction);

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadBool(System.String,System.UInt16)" />
        [EstMqttApi("ReadBoolArray", "")]
        public virtual OperateResult<bool[]> ReadBool(string address, ushort length) => new OperateResult<bool[]>(StringResources.Language.NotSupportedFunction);

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadBool(System.String)" />
        [EstMqttApi("ReadBool", "")]
        public virtual OperateResult<bool> ReadBool(string address) => ByteTransformHelper.GetResultFromArray<bool>(this.ReadBool(address, (ushort)1));

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Write(System.String,System.Boolean[])" />
        [EstMqttApi("WriteBoolArray", "")]
        public virtual OperateResult Write(string address, bool[] value) => new OperateResult(StringResources.Language.NotSupportedFunction);

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Write(System.String,System.Boolean)" />
        [EstMqttApi("WriteBool", "")]
        public virtual OperateResult Write(string address, bool value) => this.Write(address, new bool[1]
        {
      value
        });

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadCustomer``1(System.String)" />
        public OperateResult<T> ReadCustomer<T>(string address) where T : IDataTransfer, new()
        {
            OperateResult<T> operateResult1 = new OperateResult<T>();
            T obj = new T();
            OperateResult<byte[]> operateResult2 = this.Read(address, obj.ReadCount);
            if (operateResult2.IsSuccess)
            {
                obj.ParseSource(operateResult2.Content);
                operateResult1.Content = obj;
                operateResult1.IsSuccess = true;
            }
            else
            {
                operateResult1.ErrorCode = operateResult2.ErrorCode;
                operateResult1.Message = operateResult2.Message;
            }
            return operateResult1;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.WriteCustomer``1(System.String,``0)" />
        public OperateResult WriteCustomer<T>(string address, T data) where T : IDataTransfer, new() => this.Write(address, data.ToSource());

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Read``1" />
        public virtual OperateResult<T> Read<T>() where T : class, new() => EstReflectionHelper.Read<T>((IReadWriteNet)this);

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Write``1(``0)" />
        public virtual OperateResult Write<T>(T data) where T : class, new() => EstReflectionHelper.Write<T>(data, (IReadWriteNet)this);

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadInt16(System.String)" />
        [EstMqttApi("ReadInt16", "")]
        public OperateResult<short> ReadInt16(string address) => ByteTransformHelper.GetResultFromArray<short>(this.ReadInt16(address, (ushort)1));

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadInt16(System.String,System.UInt16)" />
        [EstMqttApi("ReadInt16Array", "")]
        public virtual OperateResult<short[]> ReadInt16(string address, ushort length) => ByteTransformHelper.GetResultFromBytes<short[]>(this.Read(address, (ushort)((uint)length * (uint)this.WordLength)), (Func<byte[], short[]>)(m => this.ByteTransform.TransInt16(m, 0, (int)length)));

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadUInt16(System.String)" />
        [EstMqttApi("ReadUInt16", "")]
        public OperateResult<ushort> ReadUInt16(string address) => ByteTransformHelper.GetResultFromArray<ushort>(this.ReadUInt16(address, (ushort)1));

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadUInt16(System.String,System.UInt16)" />
        [EstMqttApi("ReadUInt16Array", "")]
        public virtual OperateResult<ushort[]> ReadUInt16(string address, ushort length) => ByteTransformHelper.GetResultFromBytes<ushort[]>(this.Read(address, (ushort)((uint)length * (uint)this.WordLength)), (Func<byte[], ushort[]>)(m => this.ByteTransform.TransUInt16(m, 0, (int)length)));

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadInt32(System.String)" />
        [EstMqttApi("ReadInt32", "")]
        public OperateResult<int> ReadInt32(string address) => ByteTransformHelper.GetResultFromArray<int>(this.ReadInt32(address, (ushort)1));

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadInt32(System.String,System.UInt16)" />
        [EstMqttApi("ReadInt32Array", "")]
        public virtual OperateResult<int[]> ReadInt32(string address, ushort length) => ByteTransformHelper.GetResultFromBytes<int[]>(this.Read(address, (ushort)((int)length * (int)this.WordLength * 2)), (Func<byte[], int[]>)(m => this.ByteTransform.TransInt32(m, 0, (int)length)));

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadUInt32(System.String)" />
        [EstMqttApi("ReadUInt32", "")]
        public OperateResult<uint> ReadUInt32(string address) => ByteTransformHelper.GetResultFromArray<uint>(this.ReadUInt32(address, (ushort)1));

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadUInt32(System.String,System.UInt16)" />
        [EstMqttApi("ReadUInt32Array", "")]
        public virtual OperateResult<uint[]> ReadUInt32(string address, ushort length) => ByteTransformHelper.GetResultFromBytes<uint[]>(this.Read(address, (ushort)((int)length * (int)this.WordLength * 2)), (Func<byte[], uint[]>)(m => this.ByteTransform.TransUInt32(m, 0, (int)length)));

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadFloat(System.String)" />
        [EstMqttApi("ReadFloat", "")]
        public OperateResult<float> ReadFloat(string address) => ByteTransformHelper.GetResultFromArray<float>(this.ReadFloat(address, (ushort)1));

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadFloat(System.String,System.UInt16)" />
        [EstMqttApi("ReadFloatArray", "")]
        public virtual OperateResult<float[]> ReadFloat(string address, ushort length) => ByteTransformHelper.GetResultFromBytes<float[]>(this.Read(address, (ushort)((int)length * (int)this.WordLength * 2)), (Func<byte[], float[]>)(m => this.ByteTransform.TransSingle(m, 0, (int)length)));

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadInt64(System.String)" />
        [EstMqttApi("ReadInt64", "")]
        public OperateResult<long> ReadInt64(string address) => ByteTransformHelper.GetResultFromArray<long>(this.ReadInt64(address, (ushort)1));

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadInt64(System.String,System.UInt16)" />
        [EstMqttApi("ReadInt64Array", "")]
        public virtual OperateResult<long[]> ReadInt64(string address, ushort length) => ByteTransformHelper.GetResultFromBytes<long[]>(this.Read(address, (ushort)((int)length * (int)this.WordLength * 4)), (Func<byte[], long[]>)(m => this.ByteTransform.TransInt64(m, 0, (int)length)));

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadUInt64(System.String)" />
        [EstMqttApi("ReadUInt64", "")]
        public OperateResult<ulong> ReadUInt64(string address) => ByteTransformHelper.GetResultFromArray<ulong>(this.ReadUInt64(address, (ushort)1));

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadUInt64(System.String,System.UInt16)" />
        [EstMqttApi("ReadUInt64Array", "")]
        public virtual OperateResult<ulong[]> ReadUInt64(string address, ushort length) => ByteTransformHelper.GetResultFromBytes<ulong[]>(this.Read(address, (ushort)((int)length * (int)this.WordLength * 4)), (Func<byte[], ulong[]>)(m => this.ByteTransform.TransUInt64(m, 0, (int)length)));

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadDouble(System.String)" />
        [EstMqttApi("ReadDouble", "")]
        public OperateResult<double> ReadDouble(string address) => ByteTransformHelper.GetResultFromArray<double>(this.ReadDouble(address, (ushort)1));

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadDouble(System.String,System.UInt16)" />
        [EstMqttApi("ReadDoubleArray", "")]
        public virtual OperateResult<double[]> ReadDouble(string address, ushort length) => ByteTransformHelper.GetResultFromBytes<double[]>(this.Read(address, (ushort)((int)length * (int)this.WordLength * 4)), (Func<byte[], double[]>)(m => this.ByteTransform.TransDouble(m, 0, (int)length)));

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadString(System.String,System.UInt16)" />
        [EstMqttApi("ReadString", "")]
        public OperateResult<string> ReadString(string address, ushort length) => this.ReadString(address, length, Encoding.ASCII);

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadString(System.String,System.UInt16,System.Text.Encoding)" />
        public virtual OperateResult<string> ReadString(
          string address,
          ushort length,
          Encoding encoding)
        {
            return ByteTransformHelper.GetResultFromBytes<string>(this.Read(address, length), (Func<byte[], string>)(m => this.ByteTransform.TransString(m, 0, m.Length, encoding)));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Write(System.String,System.Int16[])" />
        [EstMqttApi("WriteInt16Array", "")]
        public virtual OperateResult Write(string address, short[] values) => this.Write(address, this.ByteTransform.TransByte(values));

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Write(System.String,System.Int16)" />
        [EstMqttApi("WriteInt16", "")]
        public virtual OperateResult Write(string address, short value) => this.Write(address, new short[1]
        {
      value
        });

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Write(System.String,System.UInt16[])" />
        [EstMqttApi("WriteUInt16Array", "")]
        public virtual OperateResult Write(string address, ushort[] values) => this.Write(address, this.ByteTransform.TransByte(values));

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Write(System.String,System.UInt16)" />
        [EstMqttApi("WriteUInt16", "")]
        public virtual OperateResult Write(string address, ushort value) => this.Write(address, new ushort[1]
        {
      value
        });

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Write(System.String,System.Int32[])" />
        [EstMqttApi("WriteInt32Array", "")]
        public virtual OperateResult Write(string address, int[] values) => this.Write(address, this.ByteTransform.TransByte(values));

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Write(System.String,System.Int32)" />
        [EstMqttApi("WriteInt32", "")]
        public OperateResult Write(string address, int value) => this.Write(address, new int[1]
        {
      value
        });

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Write(System.String,System.UInt32[])" />
        [EstMqttApi("WriteUInt32Array", "")]
        public virtual OperateResult Write(string address, uint[] values) => this.Write(address, this.ByteTransform.TransByte(values));

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Write(System.String,System.UInt32)" />
        [EstMqttApi("WriteUInt32", "")]
        public OperateResult Write(string address, uint value) => this.Write(address, new uint[1]
        {
      value
        });

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Write(System.String,System.Single[])" />
        [EstMqttApi("WriteFloatArray", "")]
        public virtual OperateResult Write(string address, float[] values) => this.Write(address, this.ByteTransform.TransByte(values));

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Write(System.String,System.Single)" />
        [EstMqttApi("WriteFloat", "")]
        public OperateResult Write(string address, float value) => this.Write(address, new float[1]
        {
      value
        });

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Write(System.String,System.Int64[])" />
        [EstMqttApi("WriteInt64Array", "")]
        public virtual OperateResult Write(string address, long[] values) => this.Write(address, this.ByteTransform.TransByte(values));

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Write(System.String,System.Int64)" />
        [EstMqttApi("WriteInt64", "")]
        public OperateResult Write(string address, long value) => this.Write(address, new long[1]
        {
      value
        });

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Write(System.String,System.UInt64[])" />
        [EstMqttApi("WriteUInt64Array", "")]
        public virtual OperateResult Write(string address, ulong[] values) => this.Write(address, this.ByteTransform.TransByte(values));

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Write(System.String,System.UInt64)" />
        [EstMqttApi("WriteUInt64", "")]
        public OperateResult Write(string address, ulong value) => this.Write(address, new ulong[1]
        {
      value
        });

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Write(System.String,System.Double[])" />
        [EstMqttApi("WriteDoubleArray", "")]
        public virtual OperateResult Write(string address, double[] values) => this.Write(address, this.ByteTransform.TransByte(values));

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Write(System.String,System.Double)" />
        [EstMqttApi("WriteDouble", "")]
        public OperateResult Write(string address, double value) => this.Write(address, new double[1]
        {
      value
        });

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Write(System.String,System.String)" />
        [EstMqttApi("WriteString", "")]
        public virtual OperateResult Write(string address, string value) => this.Write(address, value, Encoding.ASCII);

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Write(System.String,System.String,System.Int32)" />
        public virtual OperateResult Write(string address, string value, int length) => this.Write(address, value, length, Encoding.ASCII);

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Write(System.String,System.String,System.Text.Encoding)" />
        public virtual OperateResult Write(
          string address,
          string value,
          Encoding encoding)
        {
            byte[] data = this.ByteTransform.TransByte(value, encoding);
            if (this.WordLength == (ushort)1)
                data = SoftBasic.ArrayExpandToLengthEven<byte>(data);
            return this.Write(address, data);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Write(System.String,System.String,System.Int32,System.Text.Encoding)" />
        public virtual OperateResult Write(
          string address,
          string value,
          int length,
          Encoding encoding)
        {
            byte[] data = this.ByteTransform.TransByte(value, encoding);
            if (this.WordLength == (ushort)1)
                data = SoftBasic.ArrayExpandToLengthEven<byte>(data);
            byte[] length1 = SoftBasic.ArrayExpandToLength<byte>(data, length);
            return this.Write(address, length1);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Wait(System.String,System.Boolean,System.Int32,System.Int32)" />
        [EstMqttApi("WaitBool", "")]
        public OperateResult<TimeSpan> Wait(
          string address,
          bool waitValue,
          int readInterval = 100,
          int waitTimeout = -1)
        {
            return ReadWriteNetHelper.Wait((IReadWriteNet)this, address, waitValue, readInterval, waitTimeout);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Wait(System.String,System.Int16,System.Int32,System.Int32)" />
        [EstMqttApi("WaitInt16", "")]
        public OperateResult<TimeSpan> Wait(
          string address,
          short waitValue,
          int readInterval = 100,
          int waitTimeout = -1)
        {
            return ReadWriteNetHelper.Wait((IReadWriteNet)this, address, waitValue, readInterval, waitTimeout);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Wait(System.String,System.UInt16,System.Int32,System.Int32)" />
        [EstMqttApi("WaitUInt16", "")]
        public OperateResult<TimeSpan> Wait(
          string address,
          ushort waitValue,
          int readInterval = 100,
          int waitTimeout = -1)
        {
            return ReadWriteNetHelper.Wait((IReadWriteNet)this, address, waitValue, readInterval, waitTimeout);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Wait(System.String,System.Int32,System.Int32,System.Int32)" />
        [EstMqttApi("WaitInt32", "")]
        public OperateResult<TimeSpan> Wait(
          string address,
          int waitValue,
          int readInterval = 100,
          int waitTimeout = -1)
        {
            return ReadWriteNetHelper.Wait((IReadWriteNet)this, address, waitValue, readInterval, waitTimeout);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Wait(System.String,System.UInt32,System.Int32,System.Int32)" />
        [EstMqttApi("WaitUInt32", "")]
        public OperateResult<TimeSpan> Wait(
          string address,
          uint waitValue,
          int readInterval = 100,
          int waitTimeout = -1)
        {
            return ReadWriteNetHelper.Wait((IReadWriteNet)this, address, waitValue, readInterval, waitTimeout);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Wait(System.String,System.Int64,System.Int32,System.Int32)" />
        [EstMqttApi("WaitInt64", "")]
        public OperateResult<TimeSpan> Wait(
          string address,
          long waitValue,
          int readInterval = 100,
          int waitTimeout = -1)
        {
            return ReadWriteNetHelper.Wait((IReadWriteNet)this, address, waitValue, readInterval, waitTimeout);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Wait(System.String,System.UInt64,System.Int32,System.Int32)" />
        [EstMqttApi("WaitUInt64", "")]
        public OperateResult<TimeSpan> Wait(
          string address,
          ulong waitValue,
          int readInterval = 100,
          int waitTimeout = -1)
        {
            return ReadWriteNetHelper.Wait((IReadWriteNet)this, address, waitValue, readInterval, waitTimeout);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Wait(System.String,System.Boolean,System.Int32,System.Int32)" />
        public async Task<OperateResult<TimeSpan>> WaitAsync(
          string address,
          bool waitValue,
          int readInterval = 100,
          int waitTimeout = -1)
        {
            OperateResult<TimeSpan> operateResult = await ReadWriteNetHelper.WaitAsync((IReadWriteNet)this, address, waitValue, readInterval, waitTimeout);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Wait(System.String,System.Int16,System.Int32,System.Int32)" />
        public async Task<OperateResult<TimeSpan>> WaitAsync(
          string address,
          short waitValue,
          int readInterval = 100,
          int waitTimeout = -1)
        {
            OperateResult<TimeSpan> operateResult = await ReadWriteNetHelper.WaitAsync((IReadWriteNet)this, address, waitValue, readInterval, waitTimeout);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Wait(System.String,System.UInt16,System.Int32,System.Int32)" />
        public async Task<OperateResult<TimeSpan>> WaitAsync(
          string address,
          ushort waitValue,
          int readInterval = 100,
          int waitTimeout = -1)
        {
            OperateResult<TimeSpan> operateResult = await ReadWriteNetHelper.WaitAsync((IReadWriteNet)this, address, waitValue, readInterval, waitTimeout);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Wait(System.String,System.Int32,System.Int32,System.Int32)" />
        public async Task<OperateResult<TimeSpan>> WaitAsync(
          string address,
          int waitValue,
          int readInterval = 100,
          int waitTimeout = -1)
        {
            OperateResult<TimeSpan> operateResult = await ReadWriteNetHelper.WaitAsync((IReadWriteNet)this, address, waitValue, readInterval, waitTimeout);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Wait(System.String,System.UInt32,System.Int32,System.Int32)" />
        public async Task<OperateResult<TimeSpan>> WaitAsync(
          string address,
          uint waitValue,
          int readInterval = 100,
          int waitTimeout = -1)
        {
            OperateResult<TimeSpan> operateResult = await ReadWriteNetHelper.WaitAsync((IReadWriteNet)this, address, waitValue, readInterval, waitTimeout);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Wait(System.String,System.Int64,System.Int32,System.Int32)" />
        public async Task<OperateResult<TimeSpan>> WaitAsync(
          string address,
          long waitValue,
          int readInterval = 100,
          int waitTimeout = -1)
        {
            OperateResult<TimeSpan> operateResult = await ReadWriteNetHelper.WaitAsync((IReadWriteNet)this, address, waitValue, readInterval, waitTimeout);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Wait(System.String,System.UInt64,System.Int32,System.Int32)" />
        public async Task<OperateResult<TimeSpan>> WaitAsync(
          string address,
          ulong waitValue,
          int readInterval = 100,
          int waitTimeout = -1)
        {
            OperateResult<TimeSpan> operateResult = await ReadWriteNetHelper.WaitAsync((IReadWriteNet)this, address, waitValue, readInterval, waitTimeout);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadAsync(System.String,System.UInt16)" />
        public virtual async Task<OperateResult<byte[]>> ReadAsync(
          string address,
          ushort length)
        {
            OperateResult<byte[]> operateResult = await Task.Run<OperateResult<byte[]>>((Func<OperateResult<byte[]>>)(() => this.Read(address, length)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.WriteAsync(System.String,System.Byte[])" />
        public virtual async Task<OperateResult> WriteAsync(
          string address,
          byte[] value)
        {
            OperateResult operateResult = await Task.Run<OperateResult>((Func<OperateResult>)(() => this.Write(address, value)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadBoolAsync(System.String,System.UInt16)" />
        public virtual async Task<OperateResult<bool[]>> ReadBoolAsync(
          string address,
          ushort length)
        {
            OperateResult<bool[]> operateResult = await Task.Run<OperateResult<bool[]>>((Func<OperateResult<bool[]>>)(() => this.ReadBool(address, length)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadBoolAsync(System.String)" />
        public virtual async Task<OperateResult<bool>> ReadBoolAsync(string address)
        {
            OperateResult<bool[]> result = await this.ReadBoolAsync(address, (ushort)1);
            return ByteTransformHelper.GetResultFromArray<bool>(result);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.WriteAsync(System.String,System.Boolean[])" />
        public virtual async Task<OperateResult> WriteAsync(
          string address,
          bool[] value)
        {
            OperateResult operateResult = await Task.Run<OperateResult>((Func<OperateResult>)(() => this.Write(address, value)));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.WriteAsync(System.String,System.Boolean)" />
        public virtual async Task<OperateResult> WriteAsync(string address, bool value)
        {
            OperateResult operateResult = await this.WriteAsync(address, new bool[1]
            {
        value
            });
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.WriteCustomerAsync``1(System.String,``0)" />
        public async Task<OperateResult<T>> ReadCustomerAsync<T>(string address) where T : IDataTransfer, new()
        {
            OperateResult<T> result = new OperateResult<T>();
            T Content = new T();
            OperateResult<byte[]> read = await this.ReadAsync(address, Content.ReadCount);
            if (read.IsSuccess)
            {
                Content.ParseSource(read.Content);
                result.Content = Content;
                result.IsSuccess = true;
            }
            else
            {
                result.ErrorCode = read.ErrorCode;
                result.Message = read.Message;
            }
            OperateResult<T> operateResult = result;
            result = (OperateResult<T>)null;
            Content = default(T);
            read = (OperateResult<byte[]>)null;
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.WriteCustomerAsync``1(System.String,``0)" />
        public async Task<OperateResult> WriteCustomerAsync<T>(string address, T data) where T : IDataTransfer, new()
        {
            OperateResult operateResult = await this.WriteAsync(address, data.ToSource());
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadAsync``1" />
        public virtual async Task<OperateResult<T>> ReadAsync<T>() where T : class, new()
        {
            OperateResult<T> operateResult = await EstReflectionHelper.ReadAsync<T>((IReadWriteNet)this);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.WriteAsync``1(``0)" />
        public virtual async Task<OperateResult> WriteAsync<T>(T data) where T : class, new()
        {
            OperateResult operateResult = await EstReflectionHelper.WriteAsync<T>(data, (IReadWriteNet)this);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadInt16Async(System.String)" />
        public async Task<OperateResult<short>> ReadInt16Async(string address)
        {
            OperateResult<short[]> result = await this.ReadInt16Async(address, (ushort)1);
            return ByteTransformHelper.GetResultFromArray<short>(result);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadInt16Async(System.String,System.UInt16)" />
        public virtual async Task<OperateResult<short[]>> ReadInt16Async(
          string address,
          ushort length)
        {
            OperateResult<byte[]> result = await this.ReadAsync(address, (ushort)((uint)length * (uint)this.WordLength));
            return ByteTransformHelper.GetResultFromBytes<short[]>(result, (Func<byte[], short[]>)(m => this.ByteTransform.TransInt16(m, 0, (int)length)));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadUInt16Async(System.String)" />
        public async Task<OperateResult<ushort>> ReadUInt16Async(string address)
        {
            OperateResult<ushort[]> result = await this.ReadUInt16Async(address, (ushort)1);
            return ByteTransformHelper.GetResultFromArray<ushort>(result);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadUInt16Async(System.String,System.UInt16)" />
        public virtual async Task<OperateResult<ushort[]>> ReadUInt16Async(
          string address,
          ushort length)
        {
            OperateResult<byte[]> result = await this.ReadAsync(address, (ushort)((uint)length * (uint)this.WordLength));
            return ByteTransformHelper.GetResultFromBytes<ushort[]>(result, (Func<byte[], ushort[]>)(m => this.ByteTransform.TransUInt16(m, 0, (int)length)));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadInt32Async(System.String)" />
        public async Task<OperateResult<int>> ReadInt32Async(string address)
        {
            OperateResult<int[]> result = await this.ReadInt32Async(address, (ushort)1);
            return ByteTransformHelper.GetResultFromArray<int>(result);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadInt32Async(System.String,System.UInt16)" />
        public virtual async Task<OperateResult<int[]>> ReadInt32Async(
          string address,
          ushort length)
        {
            OperateResult<byte[]> result = await this.ReadAsync(address, (ushort)((int)length * (int)this.WordLength * 2));
            return ByteTransformHelper.GetResultFromBytes<int[]>(result, (Func<byte[], int[]>)(m => this.ByteTransform.TransInt32(m, 0, (int)length)));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadUInt32Async(System.String)" />
        public async Task<OperateResult<uint>> ReadUInt32Async(string address)
        {
            OperateResult<uint[]> result = await this.ReadUInt32Async(address, (ushort)1);
            return ByteTransformHelper.GetResultFromArray<uint>(result);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadUInt32Async(System.String,System.UInt16)" />
        public virtual async Task<OperateResult<uint[]>> ReadUInt32Async(
          string address,
          ushort length)
        {
            OperateResult<byte[]> result = await this.ReadAsync(address, (ushort)((int)length * (int)this.WordLength * 2));
            return ByteTransformHelper.GetResultFromBytes<uint[]>(result, (Func<byte[], uint[]>)(m => this.ByteTransform.TransUInt32(m, 0, (int)length)));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadFloatAsync(System.String)" />
        public async Task<OperateResult<float>> ReadFloatAsync(string address)
        {
            OperateResult<float[]> result = await this.ReadFloatAsync(address, (ushort)1);
            return ByteTransformHelper.GetResultFromArray<float>(result);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadFloatAsync(System.String,System.UInt16)" />
        public virtual async Task<OperateResult<float[]>> ReadFloatAsync(
          string address,
          ushort length)
        {
            OperateResult<byte[]> result = await this.ReadAsync(address, (ushort)((int)length * (int)this.WordLength * 2));
            return ByteTransformHelper.GetResultFromBytes<float[]>(result, (Func<byte[], float[]>)(m => this.ByteTransform.TransSingle(m, 0, (int)length)));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadInt64Async(System.String)" />
        public async Task<OperateResult<long>> ReadInt64Async(string address)
        {
            OperateResult<long[]> result = await this.ReadInt64Async(address, (ushort)1);
            return ByteTransformHelper.GetResultFromArray<long>(result);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadInt64Async(System.String,System.UInt16)" />
        public virtual async Task<OperateResult<long[]>> ReadInt64Async(
          string address,
          ushort length)
        {
            OperateResult<byte[]> result = await this.ReadAsync(address, (ushort)((int)length * (int)this.WordLength * 4));
            return ByteTransformHelper.GetResultFromBytes<long[]>(result, (Func<byte[], long[]>)(m => this.ByteTransform.TransInt64(m, 0, (int)length)));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadUInt64Async(System.String)" />
        public async Task<OperateResult<ulong>> ReadUInt64Async(string address)
        {
            OperateResult<ulong[]> result = await this.ReadUInt64Async(address, (ushort)1);
            return ByteTransformHelper.GetResultFromArray<ulong>(result);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadUInt64Async(System.String,System.UInt16)" />
        public virtual async Task<OperateResult<ulong[]>> ReadUInt64Async(
          string address,
          ushort length)
        {
            OperateResult<byte[]> result = await this.ReadAsync(address, (ushort)((int)length * (int)this.WordLength * 4));
            return ByteTransformHelper.GetResultFromBytes<ulong[]>(result, (Func<byte[], ulong[]>)(m => this.ByteTransform.TransUInt64(m, 0, (int)length)));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadDoubleAsync(System.String)" />
        public async Task<OperateResult<double>> ReadDoubleAsync(string address)
        {
            OperateResult<double[]> result = await this.ReadDoubleAsync(address, (ushort)1);
            return ByteTransformHelper.GetResultFromArray<double>(result);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadDoubleAsync(System.String,System.UInt16)" />
        public virtual async Task<OperateResult<double[]>> ReadDoubleAsync(
          string address,
          ushort length)
        {
            OperateResult<byte[]> result = await this.ReadAsync(address, (ushort)((int)length * (int)this.WordLength * 4));
            return ByteTransformHelper.GetResultFromBytes<double[]>(result, (Func<byte[], double[]>)(m => this.ByteTransform.TransDouble(m, 0, (int)length)));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadStringAsync(System.String,System.UInt16)" />
        public async Task<OperateResult<string>> ReadStringAsync(
          string address,
          ushort length)
        {
            OperateResult<string> operateResult = await this.ReadStringAsync(address, length, Encoding.ASCII);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.ReadStringAsync(System.String,System.UInt16,System.Text.Encoding)" />
        public virtual async Task<OperateResult<string>> ReadStringAsync(
          string address,
          ushort length,
          Encoding encoding)
        {
            OperateResult<byte[]> result = await this.ReadAsync(address, length);
            return ByteTransformHelper.GetResultFromBytes<string>(result, (Func<byte[], string>)(m => this.ByteTransform.TransString(m, 0, m.Length, encoding)));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.WriteAsync(System.String,System.Int16[])" />
        public virtual async Task<OperateResult> WriteAsync(
          string address,
          short[] values)
        {
            OperateResult operateResult = await this.WriteAsync(address, this.ByteTransform.TransByte(values));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.WriteAsync(System.String,System.Int16)" />
        public virtual async Task<OperateResult> WriteAsync(
          string address,
          short value)
        {
            OperateResult operateResult = await this.WriteAsync(address, new short[1]
            {
        value
            });
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.WriteAsync(System.String,System.UInt16[])" />
        public virtual async Task<OperateResult> WriteAsync(
          string address,
          ushort[] values)
        {
            OperateResult operateResult = await this.WriteAsync(address, this.ByteTransform.TransByte(values));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.WriteAsync(System.String,System.UInt16)" />
        public virtual async Task<OperateResult> WriteAsync(
          string address,
          ushort value)
        {
            OperateResult operateResult = await this.WriteAsync(address, new ushort[1]
            {
        value
            });
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.WriteAsync(System.String,System.Int32[])" />
        public virtual async Task<OperateResult> WriteAsync(
          string address,
          int[] values)
        {
            OperateResult operateResult = await this.WriteAsync(address, this.ByteTransform.TransByte(values));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.WriteAsync(System.String,System.Int32)" />
        public async Task<OperateResult> WriteAsync(string address, int value)
        {
            OperateResult operateResult = await this.WriteAsync(address, new int[1]
            {
        value
            });
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.WriteAsync(System.String,System.UInt32[])" />
        public virtual async Task<OperateResult> WriteAsync(
          string address,
          uint[] values)
        {
            OperateResult operateResult = await this.WriteAsync(address, this.ByteTransform.TransByte(values));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.WriteAsync(System.String,System.UInt32)" />
        public async Task<OperateResult> WriteAsync(string address, uint value)
        {
            OperateResult operateResult = await this.WriteAsync(address, new uint[1]
            {
        value
            });
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.WriteAsync(System.String,System.Single[])" />
        public virtual async Task<OperateResult> WriteAsync(
          string address,
          float[] values)
        {
            OperateResult operateResult = await this.WriteAsync(address, this.ByteTransform.TransByte(values));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.WriteAsync(System.String,System.Single)" />
        public async Task<OperateResult> WriteAsync(string address, float value)
        {
            OperateResult operateResult = await this.WriteAsync(address, new float[1]
            {
        value
            });
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.WriteAsync(System.String,System.Int64[])" />
        public virtual async Task<OperateResult> WriteAsync(
          string address,
          long[] values)
        {
            OperateResult operateResult = await this.WriteAsync(address, this.ByteTransform.TransByte(values));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.WriteAsync(System.String,System.Int64)" />
        public async Task<OperateResult> WriteAsync(string address, long value)
        {
            OperateResult operateResult = await this.WriteAsync(address, new long[1]
            {
        value
            });
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.WriteAsync(System.String,System.UInt64[])" />
        public virtual async Task<OperateResult> WriteAsync(
          string address,
          ulong[] values)
        {
            OperateResult operateResult = await this.WriteAsync(address, this.ByteTransform.TransByte(values));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.WriteAsync(System.String,System.UInt64)" />
        public async Task<OperateResult> WriteAsync(string address, ulong value)
        {
            OperateResult operateResult = await this.WriteAsync(address, new ulong[1]
            {
        value
            });
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.WriteAsync(System.String,System.Double[])" />
        public virtual async Task<OperateResult> WriteAsync(
          string address,
          double[] values)
        {
            OperateResult operateResult = await this.WriteAsync(address, this.ByteTransform.TransByte(values));
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.WriteAsync(System.String,System.Double)" />
        public async Task<OperateResult> WriteAsync(string address, double value)
        {
            OperateResult operateResult = await this.WriteAsync(address, new double[1]
            {
        value
            });
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.WriteAsync(System.String,System.String)" />
        public virtual async Task<OperateResult> WriteAsync(
          string address,
          string value)
        {
            OperateResult operateResult = await this.WriteAsync(address, value, Encoding.ASCII);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.WriteAsync(System.String,System.String,System.Text.Encoding)" />
        public virtual async Task<OperateResult> WriteAsync(
          string address,
          string value,
          Encoding encoding)
        {
            byte[] temp = this.ByteTransform.TransByte(value, encoding);
            if (this.WordLength == (ushort)1)
                temp = SoftBasic.ArrayExpandToLengthEven<byte>(temp);
            OperateResult operateResult = await this.WriteAsync(address, temp);
            temp = (byte[])null;
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.WriteAsync(System.String,System.String,System.Int32)" />
        public virtual async Task<OperateResult> WriteAsync(
          string address,
          string value,
          int length)
        {
            OperateResult operateResult = await this.WriteAsync(address, value, length, Encoding.ASCII);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.WriteAsync(System.String,System.String,System.Int32,System.Text.Encoding)" />
        public virtual async Task<OperateResult> WriteAsync(
          string address,
          string value,
          int length,
          Encoding encoding)
        {
            byte[] temp = this.ByteTransform.TransByte(value, encoding);
            if (this.WordLength == (ushort)1)
                temp = SoftBasic.ArrayExpandToLengthEven<byte>(temp);
            temp = SoftBasic.ArrayExpandToLength<byte>(temp, length);
            OperateResult operateResult = await this.WriteAsync(address, temp);
            temp = (byte[])null;
            return operateResult;
        }

        /// <summary>
        /// 当接收到来自客户的数据信息时触发的对象，该数据可能来自tcp或是串口<br />
        /// The object that is triggered when receiving data information from the customer, the data may come from tcp or serial port
        /// </summary>
        /// <param name="sender">触发的服务器对象</param>
        /// <param name="source">消息的来源对象</param>
        /// <param name="data">实际的数据信息</param>
        public delegate void DataReceivedDelegate(object sender, object source, byte[] data);

        /// <summary>
        /// 数据发送的时候委托<br />
        /// Show DataSend To PLC
        /// </summary>
        /// <param name="sender">数据发送对象</param>
        /// <param name="data">数据内容</param>
        public delegate void DataSendDelegate(object sender, byte[] data);
    }
}
