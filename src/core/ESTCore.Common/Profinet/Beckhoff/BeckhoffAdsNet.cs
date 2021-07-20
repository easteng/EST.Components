// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Beckhoff.BeckhoffAdsNet
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;
using ESTCore.Common.Core;
using ESTCore.Common.Core.IMessage;
using ESTCore.Common.Core.Net;
using ESTCore.Common.Reflection;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.Common.Profinet.Beckhoff
{
    /// <summary>
    /// 倍福的ADS协议，支持读取倍福的地址数据，关于端口号的选择，TwinCAT2，端口号801；TwinCAT3，端口号为851<br />
    /// Beckhoff ’s ADS protocol supports reading Beckhoff ’s address data. For the choice of port number, TwinCAT2, port number 801; TwinCAT3, port number 851
    /// </summary>
    /// <remarks>
    /// 支持的地址格式分三种，第一种是绝对的地址表示，比如M100，I100，Q100；第二种是字符串地址，采用s=aaaa;的表示方式；第三种是绝对内存地址采用i=1000000;的表示方式
    /// <br />
    /// <note type="important">
    /// 在实际的测试中，由于打开了VS软件对倍福PLC进行编程操作，会导致ESTCore.CommonDemo读取PLC发生间歇性读写失败的问题，此时需要关闭Visual Studio软件对倍福的
    /// 连接，之后ESTCore.CommonDemo就会读写成功，感谢QQ：1813782515 提供的解决思路。
    /// </note>
    /// </remarks>
    public class BeckhoffAdsNet : NetworkDeviceBase
    {
        private byte[] targetAMSNetId = new byte[8];
        private byte[] sourceAMSNetId = new byte[8];
        private string senderAMSNetId = string.Empty;
        private bool useTagCache = false;
        private readonly Dictionary<string, uint> tagCaches = new Dictionary<string, uint>();
        private readonly object tagLock = new object();
        private readonly SoftIncrementCount incrementCount = new SoftIncrementCount((long)int.MaxValue, 1L);

        /// <summary>
        /// 实例化一个默认的对象<br />
        /// Instantiate a default object
        /// </summary>
        public BeckhoffAdsNet()
        {
            this.WordLength = (ushort)2;
            this.targetAMSNetId[4] = (byte)1;
            this.targetAMSNetId[5] = (byte)1;
            this.targetAMSNetId[6] = (byte)33;
            this.targetAMSNetId[7] = (byte)3;
            this.sourceAMSNetId[4] = (byte)1;
            this.sourceAMSNetId[5] = (byte)1;
            this.ByteTransform = (IByteTransform)new RegularByteTransform();
        }

        /// <summary>
        /// 通过指定的ip地址以及端口号实例化一个默认的对象<br />
        /// Instantiate a default object with the specified IP address and port number
        /// </summary>
        /// <param name="ipAddress">IP地址信息</param>
        /// <param name="port">端口号</param>
        public BeckhoffAdsNet(string ipAddress, int port)
        {
            this.IpAddress = ipAddress;
            this.Port = port;
            this.WordLength = (ushort)2;
            this.targetAMSNetId[4] = (byte)1;
            this.targetAMSNetId[5] = (byte)1;
            this.targetAMSNetId[6] = (byte)33;
            this.targetAMSNetId[7] = (byte)3;
            this.sourceAMSNetId[4] = (byte)1;
            this.sourceAMSNetId[5] = (byte)1;
            this.ByteTransform = (IByteTransform)new RegularByteTransform();
        }

        /// <inheritdoc />
        protected override INetMessage GetNewNetMessage() => (INetMessage)new AdsNetMessage();

        /// <inheritdoc />
        [EstMqttApi(Description = "Get or set the IP address of the remote server. If it is a local test, then it needs to be set to 127.0.0.1", HttpMethod = "GET")]
        public override string IpAddress
        {
            get => base.IpAddress;
            set
            {
                base.IpAddress = value;
                string[] strArray = base.IpAddress.Split(new char[1]
                {
          '.'
                }, StringSplitOptions.RemoveEmptyEntries);
                for (int index = 0; index < strArray.Length; ++index)
                    this.targetAMSNetId[index] = byte.Parse(strArray[index]);
            }
        }

        /// <summary>
        /// 是否使用标签的名称缓存功能，默认为 <c>False</c><br />
        /// Whether to use tag name caching. The default is <c>False</c>
        /// </summary>
        public bool UseTagCache
        {
            get => this.useTagCache;
            set => this.useTagCache = value;
        }

        /// <summary>
        /// 目标的地址，举例 192.168.0.1.1.1；也可以是带端口号 192.168.0.1.1.1:801<br />
        /// The address of the destination, for example 192.168.0.1.1.1; it can also be the port number 192.168.0.1.1.1: 801
        /// </summary>
        /// <remarks>
        /// Port：1: AMS Router; 2: AMS Debugger; 800: Ring 0 TC2 PLC; 801: TC2 PLC Runtime System 1; 811: TC2 PLC Runtime System 2; <br />
        /// 821: TC2 PLC Runtime System 3; 831: TC2 PLC Runtime System 4; 850: Ring 0 TC3 PLC; 851: TC3 PLC Runtime System 1<br />
        /// 852: TC3 PLC Runtime System 2; 853: TC3 PLC Runtime System 3; 854: TC3 PLC Runtime System 4; ...
        /// </remarks>
        /// <param name="amsNetId">AMSNet Id地址</param>
        public void SetTargetAMSNetId(string amsNetId)
        {
            if (string.IsNullOrEmpty(amsNetId))
                return;
            BeckhoffAdsNet.StrToAMSNetId(amsNetId).CopyTo((Array)this.targetAMSNetId, 0);
        }

        /// <summary>
        /// 设置原目标地址 举例 192.168.0.100.1.1；也可以是带端口号 192.168.0.100.1.1:34567<br />
        /// Set the original destination address Example: 192.168.0.100.1.1; it can also be the port number 192.168.0.100.1.1: 34567
        /// </summary>
        /// <param name="amsNetId">原地址</param>
        public void SetSenderAMSNetId(string amsNetId)
        {
            if (string.IsNullOrEmpty(amsNetId))
                return;
            BeckhoffAdsNet.StrToAMSNetId(amsNetId).CopyTo((Array)this.sourceAMSNetId, 0);
            this.senderAMSNetId = amsNetId;
        }

        /// <inheritdoc />
        protected override OperateResult InitializationOnConnect(Socket socket)
        {
            if (string.IsNullOrEmpty(this.senderAMSNetId))
            {
                IPEndPoint localEndPoint = (IPEndPoint)socket.LocalEndPoint;
                this.sourceAMSNetId[6] = BitConverter.GetBytes(localEndPoint.Port)[0];
                this.sourceAMSNetId[7] = BitConverter.GetBytes(localEndPoint.Port)[1];
                localEndPoint.Address.GetAddressBytes().CopyTo((Array)this.sourceAMSNetId, 0);
            }
            return base.InitializationOnConnect(socket);
        }

        /// <inheritdoc />
        protected override async Task<OperateResult> InitializationOnConnectAsync(
          Socket socket)
        {
            if (string.IsNullOrEmpty(this.senderAMSNetId))
            {
                IPEndPoint iPEndPoint = (IPEndPoint)socket.LocalEndPoint;
                this.sourceAMSNetId[6] = BitConverter.GetBytes(iPEndPoint.Port)[0];
                this.sourceAMSNetId[7] = BitConverter.GetBytes(iPEndPoint.Port)[1];
                iPEndPoint.Address.GetAddressBytes().CopyTo((Array)this.sourceAMSNetId, 0);
                iPEndPoint = (IPEndPoint)null;
            }
            OperateResult operateResult = await base.InitializationOnConnectAsync(socket);
            return operateResult;
        }

        /// <summary>
        /// 根据当前标签的地址获取到内存偏移地址<br />
        /// Get the memory offset address based on the address of the current label
        /// </summary>
        /// <param name="address">带标签的地址信息，例如s=A,那么标签就是A</param>
        /// <returns>内存偏移地址</returns>
        public OperateResult<uint> ReadValueHandle(string address)
        {
            if (!address.StartsWith("s="))
                return new OperateResult<uint>(StringResources.Language.SAMAddressStartWrong);
            OperateResult<byte[]> operateResult1 = this.BuildReadWriteCommand(address, 4, false, BeckhoffAdsNet.StrToAdsBytes(address.Substring(2)));
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<uint>((OperateResult)operateResult1);
            OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
            if (!operateResult2.IsSuccess)
                return OperateResult.CreateFailedResult<uint>((OperateResult)operateResult2);
            OperateResult result = BeckhoffAdsNet.CheckResponse(operateResult2.Content);
            return !result.IsSuccess ? OperateResult.CreateFailedResult<uint>(result) : OperateResult.CreateSuccessResult<uint>(BitConverter.ToUInt32(operateResult2.Content, 46));
        }

        /// <summary>
        /// 将字符串的地址转换为内存的地址，其他地址则不操作<br />
        /// Converts the address of a string to the address of a memory, other addresses do not operate
        /// </summary>
        /// <param name="address">地址信息，s=A的地址转换为i=100000的形式</param>
        /// <returns>地址</returns>
        public OperateResult<string> TransValueHandle(string address)
        {
            if (!address.StartsWith("s="))
                return OperateResult.CreateSuccessResult<string>(address);
            if (this.useTagCache)
            {
                lock (this.tagLock)
                {
                    if (this.tagCaches.ContainsKey(address))
                        return OperateResult.CreateSuccessResult<string>(string.Format("i={0}", (object)this.tagCaches[address]));
                }
            }
            OperateResult<uint> operateResult = this.ReadValueHandle(address);
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<string>((OperateResult)operateResult);
            if (this.useTagCache)
            {
                lock (this.tagLock)
                {
                    if (!this.tagCaches.ContainsKey(address))
                        this.tagCaches.Add(address, operateResult.Content);
                }
            }
            return OperateResult.CreateSuccessResult<string>(string.Format("i={0}", (object)operateResult.Content));
        }

        /// <summary>
        /// 读取Ads设备的设备信息。主要是版本号，设备名称<br />
        /// Read the device information of the Ads device. Mainly version number, device name
        /// </summary>
        /// <returns>设备信息</returns>
        [EstMqttApi("ReadAdsDeviceInfo", "读取Ads设备的设备信息。主要是版本号，设备名称")]
        public OperateResult<AdsDeviceInfo> ReadAdsDeviceInfo()
        {
            OperateResult<byte[]> operateResult1 = this.BuildReadDeviceInfoCommand();
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<AdsDeviceInfo>((OperateResult)operateResult1);
            OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
            if (!operateResult2.IsSuccess)
                return OperateResult.CreateFailedResult<AdsDeviceInfo>((OperateResult)operateResult2);
            OperateResult result = BeckhoffAdsNet.CheckResponse(operateResult2.Content);
            return !result.IsSuccess ? OperateResult.CreateFailedResult<AdsDeviceInfo>(result) : OperateResult.CreateSuccessResult<AdsDeviceInfo>(new AdsDeviceInfo(operateResult2.Content.RemoveBegin<byte>(42)));
        }

        /// <summary>
        /// 读取Ads设备的状态信息，其中<see cref="P:ESTCore.Common.OperateResult`2.Content1" />是Ads State，<see cref="P:ESTCore.Common.OperateResult`2.Content2" />是Device State<br />
        /// Read the status information of the Ads device, where <see cref="P:ESTCore.Common.OperateResult`2.Content1" /> is the Ads State, and <see cref="P:ESTCore.Common.OperateResult`2.Content2" /> is the Device State
        /// </summary>
        /// <returns>设备状态信息</returns>
        public OperateResult<ushort, ushort> ReadAdsState()
        {
            OperateResult<byte[]> operateResult1 = this.BuildReadStateCommand();
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<ushort, ushort>((OperateResult)operateResult1);
            OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
            if (!operateResult2.IsSuccess)
                return OperateResult.CreateFailedResult<ushort, ushort>((OperateResult)operateResult2);
            OperateResult result = BeckhoffAdsNet.CheckResponse(operateResult2.Content);
            return !result.IsSuccess ? OperateResult.CreateFailedResult<ushort, ushort>(result) : OperateResult.CreateSuccessResult<ushort, ushort>(BitConverter.ToUInt16(operateResult2.Content, 42), BitConverter.ToUInt16(operateResult2.Content, 44));
        }

        /// <summary>
        /// 写入Ads的状态，可以携带数据信息，数据可以为空<br />
        /// Write the status of Ads, can carry data information, and the data can be empty
        /// </summary>
        /// <param name="state">ads state</param>
        /// <param name="deviceState">device state</param>
        /// <param name="data">数据信息</param>
        /// <returns>是否写入成功</returns>
        public OperateResult WriteAdsState(short state, short deviceState, byte[] data)
        {
            OperateResult<byte[]> operateResult1 = this.BuildWriteControlCommand(state, deviceState, data);
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
            if (!operateResult2.IsSuccess)
                return (OperateResult)operateResult2;
            OperateResult operateResult3 = BeckhoffAdsNet.CheckResponse(operateResult2.Content);
            return !operateResult3.IsSuccess ? operateResult3 : OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 释放当前的系统句柄，该句柄是通过<see cref="M:ESTCore.Common.Profinet.Beckhoff.BeckhoffAdsNet.ReadValueHandle(System.String)" />获取的
        /// </summary>
        /// <param name="handle">句柄</param>
        /// <returns>是否释放成功</returns>
        public OperateResult ReleaseSystemHandle(uint handle)
        {
            OperateResult<byte[]> operateResult1 = this.BuildReleaseSystemHandle(handle);
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
            if (!operateResult2.IsSuccess)
                return (OperateResult)operateResult2;
            OperateResult operateResult3 = BeckhoffAdsNet.CheckResponse(operateResult2.Content);
            return !operateResult3.IsSuccess ? operateResult3 : OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Beckhoff.BeckhoffAdsNet.ReadValueHandle(System.String)" />
        public async Task<OperateResult<uint>> ReadValueHandleAsync(string address)
        {
            if (!address.StartsWith("s="))
                return new OperateResult<uint>(StringResources.Language.SAMAddressStartWrong);
            OperateResult<byte[]> build = this.BuildReadWriteCommand(address, 4, false, BeckhoffAdsNet.StrToAdsBytes(address.Substring(2)));
            if (!build.IsSuccess)
                return OperateResult.CreateFailedResult<uint>((OperateResult)build);
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(build.Content);
            if (!read.IsSuccess)
                return OperateResult.CreateFailedResult<uint>((OperateResult)read);
            OperateResult check = BeckhoffAdsNet.CheckResponse(read.Content);
            return check.IsSuccess ? OperateResult.CreateSuccessResult<uint>(BitConverter.ToUInt32(read.Content, 46)) : OperateResult.CreateFailedResult<uint>(check);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Beckhoff.BeckhoffAdsNet.TransValueHandle(System.String)" />
        public async Task<OperateResult<string>> TransValueHandleAsync(string address)
        {
            if (!address.StartsWith("s="))
                return OperateResult.CreateSuccessResult<string>(address);
            if (this.useTagCache)
            {
                lock (this.tagLock)
                {
                    if (this.tagCaches.ContainsKey(address))
                        return OperateResult.CreateSuccessResult<string>(string.Format("i={0}", (object)this.tagCaches[address]));
                }
            }
            OperateResult<uint> read = await this.ReadValueHandleAsync(address);
            if (!read.IsSuccess)
                return OperateResult.CreateFailedResult<string>((OperateResult)read);
            if (this.useTagCache)
            {
                lock (this.tagLock)
                {
                    if (!this.tagCaches.ContainsKey(address))
                        this.tagCaches.Add(address, read.Content);
                }
            }
            return OperateResult.CreateSuccessResult<string>(string.Format("i={0}", (object)read.Content));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Beckhoff.BeckhoffAdsNet.ReadAdsDeviceInfo" />
        public async Task<OperateResult<AdsDeviceInfo>> ReadAdsDeviceInfoAsync()
        {
            OperateResult<byte[]> build = this.BuildReadDeviceInfoCommand();
            if (!build.IsSuccess)
                return OperateResult.CreateFailedResult<AdsDeviceInfo>((OperateResult)build);
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(build.Content);
            if (!read.IsSuccess)
                return OperateResult.CreateFailedResult<AdsDeviceInfo>((OperateResult)read);
            OperateResult check = BeckhoffAdsNet.CheckResponse(read.Content);
            return check.IsSuccess ? OperateResult.CreateSuccessResult<AdsDeviceInfo>(new AdsDeviceInfo(read.Content.RemoveBegin<byte>(42))) : OperateResult.CreateFailedResult<AdsDeviceInfo>(check);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Beckhoff.BeckhoffAdsNet.ReadAdsState" />
        public async Task<OperateResult<ushort, ushort>> ReadAdsStateAsync()
        {
            OperateResult<byte[]> build = this.BuildReadStateCommand();
            if (!build.IsSuccess)
                return OperateResult.CreateFailedResult<ushort, ushort>((OperateResult)build);
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(build.Content);
            if (!read.IsSuccess)
                return OperateResult.CreateFailedResult<ushort, ushort>((OperateResult)read);
            OperateResult check = BeckhoffAdsNet.CheckResponse(read.Content);
            return check.IsSuccess ? OperateResult.CreateSuccessResult<ushort, ushort>(BitConverter.ToUInt16(read.Content, 42), BitConverter.ToUInt16(read.Content, 44)) : OperateResult.CreateFailedResult<ushort, ushort>(check);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Beckhoff.BeckhoffAdsNet.WriteAdsState(System.Int16,System.Int16,System.Byte[])" />
        public async Task<OperateResult> WriteAdsStateAsync(
          short state,
          short deviceState,
          byte[] data)
        {
            OperateResult<byte[]> build = this.BuildWriteControlCommand(state, deviceState, data);
            if (!build.IsSuccess)
                return (OperateResult)build;
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(build.Content);
            if (!read.IsSuccess)
                return (OperateResult)read;
            OperateResult check = BeckhoffAdsNet.CheckResponse(read.Content);
            return check.IsSuccess ? OperateResult.CreateSuccessResult() : check;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Beckhoff.BeckhoffAdsNet.ReleaseSystemHandle(System.UInt32)" />
        public async Task<OperateResult> ReleaseSystemHandleAsync(uint handle)
        {
            OperateResult<byte[]> build = this.BuildReleaseSystemHandle(handle);
            if (!build.IsSuccess)
                return (OperateResult)build;
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(build.Content);
            if (!read.IsSuccess)
                return (OperateResult)read;
            OperateResult check = BeckhoffAdsNet.CheckResponse(read.Content);
            return check.IsSuccess ? OperateResult.CreateSuccessResult() : check;
        }

        /// <summary>
        /// 读取PLC的数据，地址共有三种格式，一：I,Q,M数据信息，举例M0,M100；二：内存地址，i=100000；三：标签地址，s=A<br />
        /// Read PLC data, there are three formats of address, one: I, Q, M data information, such as M0, M100; two: memory address, i = 100000; three: tag address, s = A
        /// </summary>
        /// <param name="address">地址信息，地址共有三种格式，一：I,Q,M数据信息，举例M0,M100；二：内存地址，i=100000；三：标签地址，s=A</param>
        /// <param name="length">长度</param>
        /// <returns>包含是否成功的结果对象</returns>
        [EstMqttApi("ReadByteArray", "")]
        public override OperateResult<byte[]> Read(string address, ushort length)
        {
            OperateResult<string> operateResult1 = this.TransValueHandle(address);
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult1);
            address = operateResult1.Content;
            OperateResult<byte[]> operateResult2 = this.BuildReadCommand(address, (int)length, false);
            if (!operateResult2.IsSuccess)
                return operateResult2;
            OperateResult<byte[]> operateResult3 = this.ReadFromCoreServer(operateResult2.Content);
            if (!operateResult3.IsSuccess)
                return operateResult3;
            OperateResult result = BeckhoffAdsNet.CheckResponse(operateResult3.Content);
            return !result.IsSuccess ? OperateResult.CreateFailedResult<byte[]>(result) : OperateResult.CreateSuccessResult<byte[]>(SoftBasic.ArrayRemoveBegin<byte>(operateResult3.Content, 46));
        }

        /// <summary>
        /// 写入PLC的数据，地址共有三种格式，一：I,Q,M数据信息，举例M0,M100；二：内存地址，i=100000；三：标签地址，s=A<br />
        /// There are three formats for the data written into the PLC. One: I, Q, M data information, such as M0, M100; two: memory address, i = 100000; three: tag address, s = A
        /// </summary>
        /// <param name="address">地址信息，地址共有三种格式，一：I,Q,M数据信息，举例M0,M100；二：内存地址，i=100000；三：标签地址，s=A</param>
        /// <param name="value">数据值</param>
        /// <returns>是否写入成功</returns>
        [EstMqttApi("WriteByteArray", "")]
        public override OperateResult Write(string address, byte[] value)
        {
            OperateResult<string> operateResult1 = this.TransValueHandle(address);
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            address = operateResult1.Content;
            OperateResult<byte[]> operateResult2 = this.BuildWriteCommand(address, value, false);
            if (!operateResult2.IsSuccess)
                return (OperateResult)operateResult2;
            OperateResult<byte[]> operateResult3 = this.ReadFromCoreServer(operateResult2.Content);
            if (!operateResult3.IsSuccess)
                return (OperateResult)operateResult3;
            OperateResult operateResult4 = BeckhoffAdsNet.CheckResponse(operateResult3.Content);
            return !operateResult4.IsSuccess ? operateResult4 : OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 读取PLC的数据，地址共有三种格式，一：I,Q,M数据信息，举例M0,M100；二：内存地址，i=100000；三：标签地址，s=A<br />
        /// Read PLC data, there are three formats of address, one: I, Q, M data information, such as M0, M100; two: memory address, i = 100000; three: tag address, s = A
        /// </summary>
        /// <param name="address">PLC的地址信息，例如 M10</param>
        /// <param name="length">数据长度</param>
        /// <returns>包含是否成功的结果对象</returns>
        [EstMqttApi("ReadBoolArray", "")]
        public override OperateResult<bool[]> ReadBool(string address, ushort length)
        {
            OperateResult<string> operateResult1 = this.TransValueHandle(address);
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult1);
            address = operateResult1.Content;
            OperateResult<byte[]> operateResult2 = this.BuildReadCommand(address, (int)length, true);
            if (!operateResult2.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult2);
            OperateResult<byte[]> operateResult3 = this.ReadFromCoreServer(operateResult2.Content);
            if (!operateResult3.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult3);
            OperateResult result = BeckhoffAdsNet.CheckResponse(operateResult3.Content);
            return !result.IsSuccess ? OperateResult.CreateFailedResult<bool[]>(result) : OperateResult.CreateSuccessResult<bool[]>(SoftBasic.ByteToBoolArray(SoftBasic.ArrayRemoveBegin<byte>(operateResult3.Content, 46)));
        }

        /// <summary>
        /// 写入PLC的数据，地址共有三种格式，一：I,Q,M数据信息，举例M0,M100；二：内存地址，i=100000；三：标签地址，s=A<br />
        /// There are three formats for the data written into the PLC. One: I, Q, M data information, such as M0, M100; two: memory address, i = 100000; three: tag address, s = A
        /// </summary>
        /// <param name="address">地址信息</param>
        /// <param name="value">数据值</param>
        /// <returns>是否写入成功</returns>
        [EstMqttApi("WriteBoolArray", "")]
        public override OperateResult Write(string address, bool[] value)
        {
            OperateResult<string> operateResult1 = this.TransValueHandle(address);
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            address = operateResult1.Content;
            OperateResult<byte[]> operateResult2 = this.BuildWriteCommand(address, value, true);
            if (!operateResult2.IsSuccess)
                return (OperateResult)operateResult2;
            OperateResult<byte[]> operateResult3 = this.ReadFromCoreServer(operateResult2.Content);
            if (!operateResult3.IsSuccess)
                return (OperateResult)operateResult3;
            OperateResult operateResult4 = BeckhoffAdsNet.CheckResponse(operateResult3.Content);
            return !operateResult4.IsSuccess ? operateResult4 : OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 读取PLC的数据，地址共有三种格式，一：I,Q,M数据信息，举例M0,M100；二：内存地址，i=100000；三：标签地址，s=A<br />
        /// Read PLC data, there are three formats of address, one: I, Q, M data information, such as M0, M100; two: memory address, i = 100000; three: tag address, s = A
        /// </summary>
        /// <param name="address">地址信息</param>
        /// <returns>包含是否成功的结果对象</returns>
        [EstMqttApi("ReadByte", "")]
        public OperateResult<byte> ReadByte(string address) => ByteTransformHelper.GetResultFromArray<byte>(this.Read(address, (ushort)1));

        /// <summary>
        /// 写入PLC的数据，地址共有三种格式，一：I,Q,M数据信息，举例M0,M100；二：内存地址，i=100000；三：标签地址，s=A<br />
        /// There are three formats for the data written into the PLC. One: I, Q, M data information, such as M0, M100; two: memory address, i = 100000; three: tag address, s = A
        /// </summary>
        /// <param name="address">地址信息</param>
        /// <param name="value">数据值</param>
        /// <returns>是否写入成功</returns>
        [EstMqttApi("WriteByte", "")]
        public OperateResult Write(string address, byte value) => this.Write(address, new byte[1]
        {
      value
        });

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Beckhoff.BeckhoffAdsNet.Read(System.String,System.UInt16)" />
        public override async Task<OperateResult<byte[]>> ReadAsync(
          string address,
          ushort length)
        {
            OperateResult<string> addressCheck = await this.TransValueHandleAsync(address);
            if (!addressCheck.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)addressCheck);
            address = addressCheck.Content;
            OperateResult<byte[]> build = this.BuildReadCommand(address, (int)length, false);
            if (!build.IsSuccess)
                return build;
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(build.Content);
            if (!read.IsSuccess)
                return read;
            OperateResult check = BeckhoffAdsNet.CheckResponse(read.Content);
            return check.IsSuccess ? OperateResult.CreateSuccessResult<byte[]>(SoftBasic.ArrayRemoveBegin<byte>(read.Content, 46)) : OperateResult.CreateFailedResult<byte[]>(check);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Beckhoff.BeckhoffAdsNet.Write(System.String,System.Byte[])" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          byte[] value)
        {
            OperateResult<string> addressCheck = await this.TransValueHandleAsync(address);
            if (!addressCheck.IsSuccess)
                return (OperateResult)addressCheck;
            address = addressCheck.Content;
            OperateResult<byte[]> build = this.BuildWriteCommand(address, value, false);
            if (!build.IsSuccess)
                return (OperateResult)build;
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(build.Content);
            if (!read.IsSuccess)
                return (OperateResult)read;
            OperateResult check = BeckhoffAdsNet.CheckResponse(read.Content);
            return check.IsSuccess ? OperateResult.CreateSuccessResult() : check;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Beckhoff.BeckhoffAdsNet.ReadBool(System.String,System.UInt16)" />
        public override async Task<OperateResult<bool[]>> ReadBoolAsync(
          string address,
          ushort length)
        {
            OperateResult<string> addressCheck = await this.TransValueHandleAsync(address);
            if (!addressCheck.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)addressCheck);
            address = addressCheck.Content;
            OperateResult<byte[]> build = this.BuildReadCommand(address, (int)length, true);
            if (!build.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)build);
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(build.Content);
            if (!read.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)read);
            OperateResult check = BeckhoffAdsNet.CheckResponse(read.Content);
            return check.IsSuccess ? OperateResult.CreateSuccessResult<bool[]>(SoftBasic.ByteToBoolArray(SoftBasic.ArrayRemoveBegin<byte>(read.Content, 46))) : OperateResult.CreateFailedResult<bool[]>(check);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Beckhoff.BeckhoffAdsNet.Write(System.String,System.Boolean[])" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          bool[] value)
        {
            OperateResult<string> addressCheck = await this.TransValueHandleAsync(address);
            if (!addressCheck.IsSuccess)
                return (OperateResult)addressCheck;
            address = addressCheck.Content;
            OperateResult<byte[]> build = this.BuildWriteCommand(address, value, true);
            if (!build.IsSuccess)
                return (OperateResult)build;
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(build.Content);
            if (!read.IsSuccess)
                return (OperateResult)read;
            OperateResult check = BeckhoffAdsNet.CheckResponse(read.Content);
            return check.IsSuccess ? OperateResult.CreateSuccessResult() : check;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Beckhoff.BeckhoffAdsNet.ReadByte(System.String)" />
        public async Task<OperateResult<byte>> ReadByteAsync(string address)
        {
            OperateResult<byte[]> result = await this.ReadAsync(address, (ushort)1);
            return ByteTransformHelper.GetResultFromArray<byte>(result);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Beckhoff.BeckhoffAdsNet.Write(System.String,System.Byte)" />
        public async Task<OperateResult> WriteAsync(string address, byte value)
        {
            OperateResult operateResult = await this.WriteAsync(address, new byte[1]
            {
        value
            });
            return operateResult;
        }

        /// <summary>根据命令码ID，消息ID，数据信息组成AMS的命令码</summary>
        /// <param name="commandId">命令码ID</param>
        /// <param name="data">数据内容</param>
        /// <returns>打包之后的数据信息，没有填写AMSNetId的Target和Source内容</returns>
        public byte[] BuildAmsHeaderCommand(ushort commandId, byte[] data)
        {
            if (data == null)
                data = new byte[0];
            uint currentValue = (uint)this.incrementCount.GetCurrentValue();
            byte[] command = new byte[32 + data.Length];
            this.targetAMSNetId.CopyTo((Array)command, 0);
            this.sourceAMSNetId.CopyTo((Array)command, 8);
            command[16] = BitConverter.GetBytes(commandId)[0];
            command[17] = BitConverter.GetBytes(commandId)[1];
            command[18] = (byte)4;
            command[19] = (byte)0;
            command[20] = BitConverter.GetBytes(data.Length)[0];
            command[21] = BitConverter.GetBytes(data.Length)[1];
            command[22] = BitConverter.GetBytes(data.Length)[2];
            command[23] = BitConverter.GetBytes(data.Length)[3];
            command[24] = (byte)0;
            command[25] = (byte)0;
            command[26] = (byte)0;
            command[27] = (byte)0;
            command[28] = BitConverter.GetBytes(currentValue)[0];
            command[29] = BitConverter.GetBytes(currentValue)[1];
            command[30] = BitConverter.GetBytes(currentValue)[2];
            command[31] = BitConverter.GetBytes(currentValue)[3];
            data.CopyTo((Array)command, 32);
            return BeckhoffAdsNet.PackAmsTcpHelper(command);
        }

        /// <summary>构建读取设备信息的命令报文</summary>
        /// <returns>报文信息</returns>
        public OperateResult<byte[]> BuildReadDeviceInfoCommand() => OperateResult.CreateSuccessResult<byte[]>(this.BuildAmsHeaderCommand((ushort)1, (byte[])null));

        /// <summary>构建读取状态的命令报文</summary>
        /// <returns>报文信息</returns>
        public OperateResult<byte[]> BuildReadStateCommand() => OperateResult.CreateSuccessResult<byte[]>(this.BuildAmsHeaderCommand((ushort)4, (byte[])null));

        /// <summary>构建写入状态的命令报文</summary>
        /// <param name="state">Ads state</param>
        /// <param name="deviceState">Device state</param>
        /// <param name="data">Data</param>
        /// <returns>报文信息</returns>
        public OperateResult<byte[]> BuildWriteControlCommand(
          short state,
          short deviceState,
          byte[] data)
        {
            if (data == null)
                data = new byte[0];
            byte[] numArray = new byte[8 + data.Length];
            return OperateResult.CreateSuccessResult<byte[]>(this.BuildAmsHeaderCommand((ushort)5, SoftBasic.SpliceArray<byte>(BitConverter.GetBytes(state), BitConverter.GetBytes(deviceState), BitConverter.GetBytes(data.Length), data)));
        }

        /// <summary>构建写入的指令信息</summary>
        /// <param name="address">地址信息</param>
        /// <param name="length">数据长度</param>
        /// <param name="isBit">是否是位信息</param>
        /// <returns>结果内容</returns>
        public OperateResult<byte[]> BuildReadCommand(
          string address,
          int length,
          bool isBit)
        {
            OperateResult<uint, uint> operateResult = BeckhoffAdsNet.AnalysisAddress(address, isBit);
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult);
            byte[] data = new byte[12];
            BitConverter.GetBytes(operateResult.Content1).CopyTo((Array)data, 0);
            BitConverter.GetBytes(operateResult.Content2).CopyTo((Array)data, 4);
            BitConverter.GetBytes(length).CopyTo((Array)data, 8);
            return OperateResult.CreateSuccessResult<byte[]>(this.BuildAmsHeaderCommand((ushort)2, data));
        }

        /// <summary>构建写入的指令信息</summary>
        /// <param name="address">地址信息</param>
        /// <param name="length">数据长度</param>
        /// <param name="isBit">是否是位信息</param>
        /// <param name="value">写入的数值</param>
        /// <returns>结果内容</returns>
        public OperateResult<byte[]> BuildReadWriteCommand(
          string address,
          int length,
          bool isBit,
          byte[] value)
        {
            OperateResult<uint, uint> operateResult = BeckhoffAdsNet.AnalysisAddress(address, isBit);
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult);
            byte[] data = new byte[16 + value.Length];
            BitConverter.GetBytes(operateResult.Content1).CopyTo((Array)data, 0);
            BitConverter.GetBytes(operateResult.Content2).CopyTo((Array)data, 4);
            BitConverter.GetBytes(length).CopyTo((Array)data, 8);
            BitConverter.GetBytes(value.Length).CopyTo((Array)data, 12);
            value.CopyTo((Array)data, 16);
            return OperateResult.CreateSuccessResult<byte[]>(this.BuildAmsHeaderCommand((ushort)9, data));
        }

        /// <summary>构建写入的指令信息</summary>
        /// <param name="address">地址信息</param>
        /// <param name="value">数据</param>
        /// <param name="isBit">是否是位信息</param>
        /// <returns>结果内容</returns>
        public OperateResult<byte[]> BuildWriteCommand(
          string address,
          byte[] value,
          bool isBit)
        {
            OperateResult<uint, uint> operateResult = BeckhoffAdsNet.AnalysisAddress(address, isBit);
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult);
            byte[] data = new byte[12 + value.Length];
            BitConverter.GetBytes(operateResult.Content1).CopyTo((Array)data, 0);
            BitConverter.GetBytes(operateResult.Content2).CopyTo((Array)data, 4);
            BitConverter.GetBytes(value.Length).CopyTo((Array)data, 8);
            value.CopyTo((Array)data, 12);
            return OperateResult.CreateSuccessResult<byte[]>(this.BuildAmsHeaderCommand((ushort)3, data));
        }

        /// <summary>构建写入的指令信息</summary>
        /// <param name="address">地址信息</param>
        /// <param name="value">数据</param>
        /// <param name="isBit">是否是位信息</param>
        /// <returns>结果内容</returns>
        public OperateResult<byte[]> BuildWriteCommand(
          string address,
          bool[] value,
          bool isBit)
        {
            OperateResult<uint, uint> operateResult = BeckhoffAdsNet.AnalysisAddress(address, isBit);
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult);
            byte[] numArray = SoftBasic.BoolArrayToByte(value);
            byte[] data = new byte[12 + numArray.Length];
            BitConverter.GetBytes(operateResult.Content1).CopyTo((Array)data, 0);
            BitConverter.GetBytes(operateResult.Content2).CopyTo((Array)data, 4);
            BitConverter.GetBytes(numArray.Length).CopyTo((Array)data, 8);
            numArray.CopyTo((Array)data, 12);
            return OperateResult.CreateSuccessResult<byte[]>(this.BuildAmsHeaderCommand((ushort)3, data));
        }

        /// <summary>构建释放句柄的报文信息，当获取了变量的句柄后，这个句柄就被释放</summary>
        /// <param name="handle">句柄信息</param>
        /// <returns>报文的结果内容</returns>
        public OperateResult<byte[]> BuildReleaseSystemHandle(uint handle)
        {
            byte[] data = new byte[16];
            BitConverter.GetBytes(61446).CopyTo((Array)data, 0);
            BitConverter.GetBytes(4).CopyTo((Array)data, 8);
            BitConverter.GetBytes(handle).CopyTo((Array)data, 12);
            return OperateResult.CreateSuccessResult<byte[]>(this.BuildAmsHeaderCommand((ushort)3, data));
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("BeckhoffAdsNet[{0}:{1}]", (object)this.IpAddress, (object)this.Port);

        /// <summary>检查从PLC的反馈的数据报文是否正确</summary>
        /// <param name="response">反馈报文</param>
        /// <returns>检查结果</returns>
        public static OperateResult CheckResponse(byte[] response)
        {
            try
            {
                int int32 = BitConverter.ToInt32(response, 38);
                if ((uint)int32 > 0U)
                    return new OperateResult(int32, StringResources.Language.UnknownError + " Source:" + response.ToHexString(' '));
            }
            catch (Exception ex)
            {
                return new OperateResult(ex.Message + " Source:" + response.ToHexString(' '));
            }
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>将实际的包含AMS头报文和数据报文的命令，打包成实际可发送的命令</summary>
        /// <param name="command">命令信息</param>
        /// <returns>结果信息</returns>
        public static byte[] PackAmsTcpHelper(byte[] command)
        {
            byte[] numArray = new byte[6 + command.Length];
            BitConverter.GetBytes(command.Length).CopyTo((Array)numArray, 2);
            command.CopyTo((Array)numArray, 6);
            return numArray;
        }

        /// <summary>分析当前的地址信息，根据结果信息进行解析出真实的偏移地址</summary>
        /// <param name="address">地址</param>
        /// <param name="isBit">是否位访问</param>
        /// <returns>结果内容</returns>
        public static OperateResult<uint, uint> AnalysisAddress(string address, bool isBit)
        {
            OperateResult<uint, uint> operateResult = new OperateResult<uint, uint>();
            try
            {
                if (address.StartsWith("i="))
                {
                    operateResult.Content1 = 61445U;
                    operateResult.Content2 = uint.Parse(address.Substring(2));
                }
                else if (address.StartsWith("s="))
                {
                    operateResult.Content1 = 61443U;
                    operateResult.Content2 = 0U;
                }
                else
                {
                    switch (address[0])
                    {
                        case 'I':
                        case 'i':
                            operateResult.Content1 = !isBit ? 61472U : 61473U;
                            break;
                        case 'M':
                        case 'm':
                            operateResult.Content1 = !isBit ? 16416U : 16417U;
                            break;
                        case 'Q':
                        case 'q':
                            operateResult.Content1 = !isBit ? 61488U : 61489U;
                            break;
                        default:
                            throw new Exception(StringResources.Language.NotSupportedDataType);
                    }
                    operateResult.Content2 = uint.Parse(address.Substring(1));
                }
            }
            catch (Exception ex)
            {
                operateResult.Message = ex.Message;
                return operateResult;
            }
            operateResult.IsSuccess = true;
            operateResult.Message = StringResources.Language.SuccessText;
            return operateResult;
        }

        /// <summary>将字符串名称转变为ADS协议可识别的字节数组</summary>
        /// <param name="value">值</param>
        /// <returns>字节数组</returns>
        public static byte[] StrToAdsBytes(string value) => SoftBasic.SpliceArray<byte>(Encoding.ASCII.GetBytes(value), new byte[1]);

        /// <summary>将字符串的信息转换为AMS目标的地址</summary>
        /// <param name="amsNetId">目标信息</param>
        /// <returns>字节数组</returns>
        public static byte[] StrToAMSNetId(string amsNetId)
        {
            string str = amsNetId;
            byte[] numArray;
            if (amsNetId.IndexOf(':') > 0)
            {
                numArray = new byte[8];
                string[] strArray = amsNetId.Split(new char[1]
                {
          ':'
                }, StringSplitOptions.RemoveEmptyEntries);
                str = strArray[0];
                numArray[6] = BitConverter.GetBytes(int.Parse(strArray[1]))[0];
                numArray[7] = BitConverter.GetBytes(int.Parse(strArray[1]))[1];
            }
            else
                numArray = new byte[6];
            string[] strArray1 = str.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            for (int index = 0; index < strArray1.Length; ++index)
                numArray[index] = byte.Parse(strArray1[index]);
            return numArray;
        }
    }
}
