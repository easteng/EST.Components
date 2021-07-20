// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Siemens.SiemensS7Net
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;
using ESTCore.Common.Core;
using ESTCore.Common.Core.Address;
using ESTCore.Common.Core.IMessage;
using ESTCore.Common.Core.Net;
using ESTCore.Common.Reflection;

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.Common.Profinet.Siemens
{
    /// <summary>
    /// 一个西门子的客户端类，使用S7协议来进行数据交互 <br />
    /// A Siemens client class that uses the S7 protocol for data interaction
    /// </summary>
    /// <remarks>
    /// 暂时不支持bool[]的批量写入操作，请使用 Write(string, byte[]) 替换。<br />
    /// <note type="important">对于200smartPLC的V区，就是DB1.X，例如，V100=DB1.100，当然了你也可以输入V100</note>
    /// </remarks>
    /// <example>
    /// 地址支持的列表如下：
    /// <list type="table">
    ///   <listheader>
    ///     <term>地址名称</term>
    ///     <term>地址代号</term>
    ///     <term>示例</term>
    ///     <term>地址进制</term>
    ///     <term>字操作</term>
    ///     <term>位操作</term>
    ///     <term>备注</term>
    ///   </listheader>
    ///   <item>
    ///     <term>中间寄存器</term>
    ///     <term>M</term>
    ///     <term>M100,M200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>输入寄存器</term>
    ///     <term>I</term>
    ///     <term>I100,I200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>输出寄存器</term>
    ///     <term>Q</term>
    ///     <term>Q100,Q200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>DB块寄存器</term>
    ///     <term>DB</term>
    ///     <term>DB1.100,DB1.200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>V寄存器</term>
    ///     <term>V</term>
    ///     <term>V100,V200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term>V寄存器本质就是DB块1</term>
    ///   </item>
    ///   <item>
    ///     <term>定时器的值</term>
    ///     <term>T</term>
    ///     <term>T100,T200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term>仅在200smart测试通过</term>
    ///   </item>
    ///   <item>
    ///     <term>计数器的值</term>
    ///     <term>C</term>
    ///     <term>C100,C200</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term>仅在200smart测试通过</term>
    ///   </item>
    /// </list>
    /// <note type="important">对于200smartPLC的V区，就是DB1.X，例如，V100=DB1.100</note>
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\SiemensS7Net.cs" region="Usage" title="简单的短连接使用" />
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\SiemensS7Net.cs" region="Usage2" title="简单的长连接使用" />
    /// 
    /// 假设起始地址为M100，M100存储了温度，100.6℃值为1006，M102存储了压力，1.23Mpa值为123，M104，M105，M106，M107存储了产量计数，读取如下：
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\SiemensS7Net.cs" region="ReadExample2" title="Read示例" />
    /// 以下是读取不同类型数据的示例
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\SiemensS7Net.cs" region="ReadExample1" title="Read示例" />
    /// 以下是一个复杂的读取示例
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\SiemensS7Net.cs" region="ReadExample3" title="Read示例" />
    /// </example>
    public class SiemensS7Net : NetworkDeviceBase
    {
        private byte[] plcHead1 = new byte[22]
        {
      (byte) 3,
      (byte) 0,
      (byte) 0,
      (byte) 22,
      (byte) 17,
      (byte) 224,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 192,
      (byte) 1,
      (byte) 10,
      (byte) 193,
      (byte) 2,
      (byte) 1,
      (byte) 2,
      (byte) 194,
      (byte) 2,
      (byte) 1,
      (byte) 0
        };
        private byte[] plcHead2 = new byte[25]
        {
      (byte) 3,
      (byte) 0,
      (byte) 0,
      (byte) 25,
      (byte) 2,
      (byte) 240,
      (byte) 128,
      (byte) 50,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 4,
      (byte) 0,
      (byte) 0,
      (byte) 8,
      (byte) 0,
      (byte) 0,
      (byte) 240,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 1,
      (byte) 1,
      (byte) 224
        };
        private byte[] plcOrderNumber = new byte[33]
        {
      (byte) 3,
      (byte) 0,
      (byte) 0,
      (byte) 33,
      (byte) 2,
      (byte) 240,
      (byte) 128,
      (byte) 50,
      (byte) 7,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 8,
      (byte) 0,
      (byte) 8,
      (byte) 0,
      (byte) 1,
      (byte) 18,
      (byte) 4,
      (byte) 17,
      (byte) 68,
      (byte) 1,
      (byte) 0,
      byte.MaxValue,
      (byte) 9,
      (byte) 0,
      (byte) 4,
      (byte) 0,
      (byte) 17,
      (byte) 0,
      (byte) 0
        };
        private SiemensPLCS CurrentPlc = SiemensPLCS.S1200;
        private byte[] plcHead1_200smart = new byte[22]
        {
      (byte) 3,
      (byte) 0,
      (byte) 0,
      (byte) 22,
      (byte) 17,
      (byte) 224,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 193,
      (byte) 2,
      (byte) 16,
      (byte) 0,
      (byte) 194,
      (byte) 2,
      (byte) 3,
      (byte) 0,
      (byte) 192,
      (byte) 1,
      (byte) 10
        };
        private byte[] plcHead2_200smart = new byte[25]
        {
      (byte) 3,
      (byte) 0,
      (byte) 0,
      (byte) 25,
      (byte) 2,
      (byte) 240,
      (byte) 128,
      (byte) 50,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 204,
      (byte) 193,
      (byte) 0,
      (byte) 8,
      (byte) 0,
      (byte) 0,
      (byte) 240,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 1,
      (byte) 3,
      (byte) 192
        };
        private byte[] plcHead1_200 = new byte[22]
        {
      (byte) 3,
      (byte) 0,
      (byte) 0,
      (byte) 22,
      (byte) 17,
      (byte) 224,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 193,
      (byte) 2,
      (byte) 77,
      (byte) 87,
      (byte) 194,
      (byte) 2,
      (byte) 77,
      (byte) 87,
      (byte) 192,
      (byte) 1,
      (byte) 9
        };
        private byte[] plcHead2_200 = new byte[25]
        {
      (byte) 3,
      (byte) 0,
      (byte) 0,
      (byte) 25,
      (byte) 2,
      (byte) 240,
      (byte) 128,
      (byte) 50,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 8,
      (byte) 0,
      (byte) 0,
      (byte) 240,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 1,
      (byte) 3,
      (byte) 192
        };
        private byte[] S7_STOP = new byte[33]
        {
      (byte) 3,
      (byte) 0,
      (byte) 0,
      (byte) 33,
      (byte) 2,
      (byte) 240,
      (byte) 128,
      (byte) 50,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 14,
      (byte) 0,
      (byte) 0,
      (byte) 16,
      (byte) 0,
      (byte) 0,
      (byte) 41,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 9,
      (byte) 80,
      (byte) 95,
      (byte) 80,
      (byte) 82,
      (byte) 79,
      (byte) 71,
      (byte) 82,
      (byte) 65,
      (byte) 77
        };
        private byte[] S7_HOT_START = new byte[37]
        {
      (byte) 3,
      (byte) 0,
      (byte) 0,
      (byte) 37,
      (byte) 2,
      (byte) 240,
      (byte) 128,
      (byte) 50,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 12,
      (byte) 0,
      (byte) 0,
      (byte) 20,
      (byte) 0,
      (byte) 0,
      (byte) 40,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 253,
      (byte) 0,
      (byte) 0,
      (byte) 9,
      (byte) 80,
      (byte) 95,
      (byte) 80,
      (byte) 82,
      (byte) 79,
      (byte) 71,
      (byte) 82,
      (byte) 65,
      (byte) 77
        };
        private byte[] S7_COLD_START = new byte[39]
        {
      (byte) 3,
      (byte) 0,
      (byte) 0,
      (byte) 39,
      (byte) 2,
      (byte) 240,
      (byte) 128,
      (byte) 50,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 15,
      (byte) 0,
      (byte) 0,
      (byte) 22,
      (byte) 0,
      (byte) 0,
      (byte) 40,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 253,
      (byte) 0,
      (byte) 2,
      (byte) 67,
      (byte) 32,
      (byte) 9,
      (byte) 80,
      (byte) 95,
      (byte) 80,
      (byte) 82,
      (byte) 79,
      (byte) 71,
      (byte) 82,
      (byte) 65,
      (byte) 77
        };
        private byte plc_rack = 0;
        private byte plc_slot = 0;
        private int pdu_length = 0;
        private const byte pduStart = 40;
        private const byte pduStop = 41;
        private const byte pduAlreadyStarted = 2;
        private const byte pduAlreadyStopped = 7;

        /// <summary>
        /// 实例化一个西门子的S7协议的通讯对象 <br />
        /// Instantiate a communication object for a Siemens S7 protocol
        /// </summary>
        /// <param name="siemens">指定西门子的型号</param>
        public SiemensS7Net(SiemensPLCS siemens) => this.Initialization(siemens, string.Empty);

        /// <summary>
        /// 实例化一个西门子的S7协议的通讯对象并指定Ip地址 <br />
        /// Instantiate a communication object for a Siemens S7 protocol and specify an IP address
        /// </summary>
        /// <param name="siemens">指定西门子的型号</param>
        /// <param name="ipAddress">Ip地址</param>
        public SiemensS7Net(SiemensPLCS siemens, string ipAddress) => this.Initialization(siemens, ipAddress);

        /// <inheritdoc />
        protected override INetMessage GetNewNetMessage() => (INetMessage)new S7Message();

        /// <summary>
        /// 初始化方法<br />
        /// Initialize method
        /// </summary>
        /// <param name="siemens">指定西门子的型号 -&gt; Designation of Siemens</param>
        /// <param name="ipAddress">Ip地址 -&gt; IpAddress</param>
        private void Initialization(SiemensPLCS siemens, string ipAddress)
        {
            this.WordLength = (ushort)2;
            this.IpAddress = ipAddress;
            this.Port = 102;
            this.CurrentPlc = siemens;
            this.ByteTransform = (IByteTransform)new ReverseBytesTransform();
            switch (siemens)
            {
                case SiemensPLCS.S1200:
                    this.plcHead1[21] = (byte)0;
                    break;
                case SiemensPLCS.S300:
                    this.plcHead1[21] = (byte)2;
                    break;
                case SiemensPLCS.S400:
                    this.plcHead1[21] = (byte)3;
                    this.plcHead1[17] = (byte)0;
                    break;
                case SiemensPLCS.S1500:
                    this.plcHead1[21] = (byte)0;
                    break;
                case SiemensPLCS.S200Smart:
                    this.plcHead1 = this.plcHead1_200smart;
                    this.plcHead2 = this.plcHead2_200smart;
                    break;
                case SiemensPLCS.S200:
                    this.plcHead1 = this.plcHead1_200;
                    this.plcHead2 = this.plcHead2_200;
                    break;
                default:
                    this.plcHead1[18] = (byte)0;
                    break;
            }
        }

        /// <summary>
        /// PLC的槽号，针对S7-400的PLC设置的<br />
        /// The slot number of PLC is set for PLC of s7-400
        /// </summary>
        public byte Slot
        {
            get => this.plc_slot;
            set
            {
                this.plc_slot = value;
                this.plcHead1[21] = (byte)((uint)this.plc_rack * 32U + (uint)this.plc_slot);
            }
        }

        /// <summary>
        /// PLC的机架号，针对S7-400的PLC设置的<br />
        /// The frame number of the PLC is set for the PLC of s7-400
        /// </summary>
        public byte Rack
        {
            get => this.plc_rack;
            set
            {
                this.plc_rack = value;
                this.plcHead1[21] = (byte)((uint)this.plc_rack * 32U + (uint)this.plc_slot);
            }
        }

        /// <summary>
        /// 获取或设置当前PLC的连接方式，PG: 0x01，OP: 0x02，S7Basic: 0x03...0x10<br />
        /// Get or set the current PLC connection mode, PG: 0x01, OP: 0x02, S7Basic: 0x03...0x10
        /// </summary>
        public byte ConnectionType
        {
            get => this.plcHead1[20];
            set => this.plcHead1[20] = value;
        }

        /// <summary>
        /// 西门子相关的一个参数信息<br />
        /// A parameter information related to Siemens
        /// </summary>
        public int LocalTSAP
        {
            get => (int)this.plcHead1[16] * 256 + (int)this.plcHead1[17];
            set
            {
                this.plcHead1[16] = BitConverter.GetBytes(value)[1];
                this.plcHead1[17] = BitConverter.GetBytes(value)[0];
            }
        }

        /// <summary>
        /// 获取当前西门子的PDU的长度信息，不同型号PLC的值会不一样。<br />
        /// Get the length information of the current Siemens PDU, the value of different types of PLC will be different.
        /// </summary>
        public int PDULength => this.pdu_length;

        /// <inheritdoc />
        public override OperateResult<byte[]> ReadFromCoreServer(
          Socket socket,
          byte[] send,
          bool hasResponseData = true,
          bool usePackHeader = true)
        {
            OperateResult<byte[]> operateResult;
            do
            {
                operateResult = base.ReadFromCoreServer(socket, send, hasResponseData, usePackHeader);
            }
            while (operateResult.IsSuccess && (int)operateResult.Content[2] * 256 + (int)operateResult.Content[3] == 7);
            return operateResult;
        }

        /// <inheritdoc />
        protected override OperateResult InitializationOnConnect(Socket socket)
        {
            OperateResult<byte[]> operateResult1 = this.ReadFromCoreServer(socket, this.plcHead1, true, true);
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(socket, this.plcHead2, true, true);
            if (!operateResult2.IsSuccess)
                return (OperateResult)operateResult2;
            this.pdu_length = (int)this.ByteTransform.TransUInt16(operateResult2.Content.SelectLast<byte>(2), 0) - 28;
            if (this.pdu_length < 200)
                this.pdu_length = 200;
            return OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc />
        public override async Task<OperateResult<byte[]>> ReadFromCoreServerAsync(
          Socket socket,
          byte[] send,
          bool hasResponseData = true,
          bool usePackHeader = true)
        {
            OperateResult<byte[]> read;
            while (true)
            {
                read = await base.ReadFromCoreServerAsync(socket, send, hasResponseData, usePackHeader);
                if (read.IsSuccess && (int)read.Content[2] * 256 + (int)read.Content[3] == 7)
                    read = (OperateResult<byte[]>)null;
                else
                    break;
            }
            return read;
        }

        /// <inheritdoc />
        protected override async Task<OperateResult> InitializationOnConnectAsync(
          Socket socket)
        {
            OperateResult<byte[]> read_first = await this.ReadFromCoreServerAsync(socket, this.plcHead1, true, true);
            if (!read_first.IsSuccess)
                return (OperateResult)read_first;
            OperateResult<byte[]> read_second = await this.ReadFromCoreServerAsync(socket, this.plcHead2, true, true);
            if (!read_second.IsSuccess)
                return (OperateResult)read_second;
            this.pdu_length = (int)this.ByteTransform.TransUInt16(read_second.Content.SelectLast<byte>(2), 0) - 28;
            if (this.pdu_length < 200)
                this.pdu_length = 200;
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 从PLC读取订货号信息<br />
        /// Reading order number information from PLC
        /// </summary>
        /// <returns>CPU的订货号信息 -&gt; Order number information for the CPU</returns>
        [EstMqttApi("ReadOrderNumber", "获取到PLC的订货号信息")]
        public OperateResult<string> ReadOrderNumber() => ByteTransformHelper.GetSuccessResultFromOther<string, byte[]>(this.ReadFromCoreServer(this.plcOrderNumber), (Func<byte[], string>)(m => Encoding.ASCII.GetString(m, 71, 20)));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Siemens.SiemensS7Net.ReadOrderNumber" />
        public async Task<OperateResult<string>> ReadOrderNumberAsync()
        {
            OperateResult<byte[]> result = await this.ReadFromCoreServerAsync(this.plcOrderNumber);
            return ByteTransformHelper.GetSuccessResultFromOther<string, byte[]>(result, (Func<byte[], string>)(m => Encoding.ASCII.GetString(m, 71, 20)));
        }

        private OperateResult CheckStartResult(byte[] content)
        {
            if (content.Length < 19)
                return new OperateResult("Receive error");
            if (content[19] != (byte)40)
                return new OperateResult("Can not start PLC");
            return content[20] != (byte)2 ? new OperateResult("Can not start PLC") : OperateResult.CreateSuccessResult();
        }

        private OperateResult CheckStopResult(byte[] content)
        {
            if (content.Length < 19)
                return new OperateResult("Receive error");
            if (content[19] != (byte)41)
                return new OperateResult("Can not stop PLC");
            return content[20] != (byte)7 ? new OperateResult("Can not stop PLC") : OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 对PLC进行热启动，目前仅适用于200smart型号<br />
        /// Hot start for PLC, currently only applicable to 200smart model
        /// </summary>
        /// <returns>是否启动成功的结果对象</returns>
        [EstMqttApi]
        public OperateResult HotStart() => ByteTransformHelper.GetResultFromOther<byte[]>(this.ReadFromCoreServer(this.S7_HOT_START), new Func<byte[], OperateResult>(this.CheckStartResult));

        /// <summary>
        /// 对PLC进行冷启动，目前仅适用于200smart型号<br />
        /// Cold start for PLC, currently only applicable to 200smart model
        /// </summary>
        /// <returns>是否启动成功的结果对象</returns>
        [EstMqttApi]
        public OperateResult ColdStart() => ByteTransformHelper.GetResultFromOther<byte[]>(this.ReadFromCoreServer(this.S7_COLD_START), new Func<byte[], OperateResult>(this.CheckStartResult));

        /// <summary>
        /// 对PLC进行停止，目前仅适用于200smart型号<br />
        /// Stop the PLC, currently only applicable to the 200smart model
        /// </summary>
        /// <returns>是否启动成功的结果对象</returns>
        [EstMqttApi]
        public OperateResult Stop() => ByteTransformHelper.GetResultFromOther<byte[]>(this.ReadFromCoreServer(this.S7_STOP), new Func<byte[], OperateResult>(this.CheckStopResult));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Siemens.SiemensS7Net.HotStart" />
        public async Task<OperateResult> HotStartAsync()
        {
            OperateResult<byte[]> result = await this.ReadFromCoreServerAsync(this.S7_HOT_START);
            return ByteTransformHelper.GetResultFromOther<byte[]>(result, new Func<byte[], OperateResult>(this.CheckStartResult));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Siemens.SiemensS7Net.ColdStart" />
        public async Task<OperateResult> ColdStartAsync()
        {
            OperateResult<byte[]> result = await this.ReadFromCoreServerAsync(this.S7_COLD_START);
            return ByteTransformHelper.GetResultFromOther<byte[]>(result, new Func<byte[], OperateResult>(this.CheckStartResult));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Siemens.SiemensS7Net.Stop" />
        public async Task<OperateResult> StopAsync()
        {
            OperateResult<byte[]> result = await this.ReadFromCoreServerAsync(this.S7_STOP);
            return ByteTransformHelper.GetResultFromOther<byte[]>(result, new Func<byte[], OperateResult>(this.CheckStopResult));
        }

        /// <summary>
        /// 从PLC读取原始的字节数据，地址格式为I100，Q100，DB20.100，M100，长度参数以字节为单位<br />
        /// Read the original byte data from the PLC, the address format is I100, Q100, DB20.100, M100, length parameters in bytes
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100<br />
        /// Starting address, formatted as I100,M100,Q100,DB20.100</param>
        /// <param name="length">读取的数量，以字节为单位<br />
        /// The number of reads, in bytes</param>
        /// <returns>
        /// 是否读取成功的结果对象 <br />
        /// Whether to read the successful result object</returns>
        /// <remarks>
        /// <inheritdoc cref="T:ESTCore.Common.Profinet.Siemens.SiemensS7Net" path="note" />
        /// </remarks>
        /// <example>
        /// 假设起始地址为M100，M100存储了温度，100.6℃值为1006，M102存储了压力，1.23Mpa值为123，M104，M105，M106，M107存储了产量计数，读取如下：
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\SiemensS7Net.cs" region="ReadExample2" title="Read示例" />
        /// 以下是读取不同类型数据的示例
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\SiemensS7Net.cs" region="ReadExample1" title="Read示例" />
        /// </example>
        [EstMqttApi("ReadByteArray", "")]
        public override OperateResult<byte[]> Read(string address, ushort length)
        {
            OperateResult<S7AddressData> from = S7AddressData.ParseFrom(address, length);
            if (!from.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)from);
            List<byte> byteList = new List<byte>();
            ushort num1 = 0;
            while ((int)num1 < (int)length)
            {
                ushort num2 = (ushort)Math.Min((int)length - (int)num1, this.pdu_length);
                from.Content.Length = num2;
                OperateResult<byte[]> operateResult = this.Read(new S7AddressData[1]
                {
          from.Content
                });
                if (!operateResult.IsSuccess)
                    return operateResult;
                byteList.AddRange((IEnumerable<byte>)operateResult.Content);
                num1 += num2;
                if (from.Content.DataCode == (byte)31 || from.Content.DataCode == (byte)30)
                    from.Content.AddressStart += (int)num2 / 2;
                else
                    from.Content.AddressStart += (int)num2 * 8;
            }
            return OperateResult.CreateSuccessResult<byte[]>(byteList.ToArray());
        }

        /// <summary>
        /// 从PLC读取数据，地址格式为I100，Q100，DB20.100，M100，以位为单位 -&gt;
        /// Read the data from the PLC, the address format is I100，Q100，DB20.100，M100, in bits units
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 -&gt;
        /// Starting address, formatted as I100,M100,Q100,DB20.100</param>
        /// <returns>是否读取成功的结果对象 -&gt; Whether to read the successful result object</returns>
        private OperateResult<byte[]> ReadBitFromPLC(string address)
        {
            OperateResult<byte[]> operateResult1 = SiemensS7Net.BuildBitReadCommand(address);
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult1);
            OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
            return !operateResult2.IsSuccess ? operateResult2 : SiemensS7Net.AnalysisReadBit(operateResult2.Content);
        }

        /// <summary>
        /// 一次性从PLC获取所有的数据，按照先后顺序返回一个统一的Buffer，需要按照顺序处理，两个数组长度必须一致，数组长度无限制<br />
        /// One-time from the PLC to obtain all the data, in order to return a unified buffer, need to be processed sequentially, two array length must be consistent
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100<br />
        /// Starting address, formatted as I100,M100,Q100,DB20.100</param>
        /// <param name="length">数据长度数组<br />
        /// Array of data Lengths</param>
        /// <returns>是否读取成功的结果对象 -&gt; Whether to read the successful result object</returns>
        /// <exception cref="T:System.NullReferenceException"></exception>
        /// <remarks>
        /// <note type="warning">原先的批量的长度为19，现在已经内部自动处理整合，目前的长度为任意和长度。</note>
        /// </remarks>
        /// <example>
        /// 以下是一个高级的读取示例
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\SiemensS7Net.cs" region="ReadExample3" title="Read示例" />
        /// </example>
        [EstMqttApi("ReadAddressArray", "一次性从PLC获取所有的数据，按照先后顺序返回一个统一的Buffer，需要按照顺序处理，两个数组长度必须一致，数组长度无限制")]
        public OperateResult<byte[]> Read(string[] address, ushort[] length)
        {
            S7AddressData[] s7Addresses = new S7AddressData[address.Length];
            for (int index = 0; index < address.Length; ++index)
            {
                OperateResult<S7AddressData> from = S7AddressData.ParseFrom(address[index], length[index]);
                if (!from.IsSuccess)
                    return OperateResult.CreateFailedResult<byte[]>((OperateResult)from);
                s7Addresses[index] = from.Content;
            }
            return this.Read(s7Addresses);
        }

        /// <summary>
        /// 读取西门子的地址数据信息，支持任意个数的数据读取<br />
        /// Read Siemens address data information, support any number of data reading
        /// </summary>
        /// <param name="s7Addresses">
        /// 西门子的数据地址<br />
        /// Siemens data address</param>
        /// <returns>返回的结果对象信息 -&gt; Whether to read the successful result object</returns>
        public OperateResult<byte[]> Read(S7AddressData[] s7Addresses)
        {
            if (s7Addresses.Length <= 19)
                return this.ReadS7AddressData(s7Addresses);
            List<byte> byteList = new List<byte>();
            List<S7AddressData[]> s7AddressDataArrayList = SoftBasic.ArraySplitByLength<S7AddressData>(s7Addresses, 19);
            for (int index = 0; index < s7AddressDataArrayList.Count; ++index)
            {
                OperateResult<byte[]> operateResult = this.Read(s7AddressDataArrayList[index]);
                if (!operateResult.IsSuccess)
                    return operateResult;
                byteList.AddRange((IEnumerable<byte>)operateResult.Content);
            }
            return OperateResult.CreateSuccessResult<byte[]>(byteList.ToArray());
        }

        /// <summary>单次的读取，只能读取最多19个数组的长度，所以不再对外公开该方法</summary>
        /// <param name="s7Addresses">西门子的地址对象</param>
        /// <returns>返回的结果对象信息</returns>
        private OperateResult<byte[]> ReadS7AddressData(S7AddressData[] s7Addresses)
        {
            OperateResult<byte[]> operateResult1 = SiemensS7Net.BuildReadCommand(s7Addresses);
            if (!operateResult1.IsSuccess)
                return operateResult1;
            OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
            return !operateResult2.IsSuccess ? operateResult2 : SiemensS7Net.AnalysisReadByte(s7Addresses, operateResult2.Content);
        }

        /// <summary>
        /// 基础的写入数据的操作支持<br />
        /// Operational support for the underlying write data
        /// </summary>
        /// <param name="entireValue">完整的字节数据 -&gt; Full byte data</param>
        /// <returns>是否写入成功的结果对象 -&gt; Whether to write a successful result object</returns>
        private OperateResult WriteBase(byte[] entireValue) => ByteTransformHelper.GetResultFromOther<byte[]>(this.ReadFromCoreServer(entireValue), new Func<byte[], OperateResult>(SiemensS7Net.AnalysisWrite));

        /// <summary>
        /// 将数据写入到PLC数据，地址格式为I100，Q100，DB20.100，M100，以字节为单位<br />
        /// Writes data to the PLC data, in the address format I100,Q100,DB20.100,M100, in bytes
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 -&gt;
        /// Starting address, formatted as I100,M100,Q100,DB20.100</param>
        /// <param name="value">写入的原始数据 -&gt; Raw data written to</param>
        /// <returns>是否写入成功的结果对象 -&gt; Whether to write a successful result object</returns>
        /// <example>
        /// 假设起始地址为M100，M100,M101存储了温度，100.6℃值为1006，M102,M103存储了压力，1.23Mpa值为123，M104-M107存储了产量计数，写入如下：
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\SiemensS7Net.cs" region="WriteExample2" title="Write示例" />
        /// 以下是写入不同类型数据的示例
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\SiemensS7Net.cs" region="WriteExample1" title="Write示例" />
        /// </example>
        [EstMqttApi("WriteByteArray", "")]
        public override OperateResult Write(string address, byte[] value)
        {
            OperateResult<S7AddressData> from = S7AddressData.ParseFrom(address);
            if (!from.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<byte[]>((OperateResult)from);
            int length = value.Length;
            ushort num1 = 0;
            while ((int)num1 < length)
            {
                ushort num2 = (ushort)Math.Min(length - (int)num1, this.pdu_length);
                byte[] data = this.ByteTransform.TransByte(value, (int)num1, (int)num2);
                OperateResult<byte[]> operateResult1 = SiemensS7Net.BuildWriteByteCommand(from, data);
                if (!operateResult1.IsSuccess)
                    return (OperateResult)operateResult1;
                OperateResult operateResult2 = this.WriteBase(operateResult1.Content);
                if (!operateResult2.IsSuccess)
                    return operateResult2;
                num1 += num2;
                from.Content.AddressStart += (int)num2 * 8;
            }
            return OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Siemens.SiemensS7Net.Read(System.String,System.UInt16)" />
        public override async Task<OperateResult<byte[]>> ReadAsync(
          string address,
          ushort length)
        {
            OperateResult<S7AddressData> addressResult = S7AddressData.ParseFrom(address, length);
            if (!addressResult.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)addressResult);
            List<byte> bytesContent = new List<byte>();
            ushort alreadyFinished = 0;
            while ((int)alreadyFinished < (int)length)
            {
                ushort readLength = (ushort)Math.Min((int)length - (int)alreadyFinished, 200);
                addressResult.Content.Length = readLength;
                OperateResult<byte[]> read = await this.ReadAsync(new S7AddressData[1]
                {
          addressResult.Content
                });
                if (!read.IsSuccess)
                    return read;
                bytesContent.AddRange((IEnumerable<byte>)read.Content);
                alreadyFinished += readLength;
                if (addressResult.Content.DataCode == (byte)31 || addressResult.Content.DataCode == (byte)30)
                    addressResult.Content.AddressStart += (int)readLength / 2;
                else
                    addressResult.Content.AddressStart += (int)readLength * 8;
                read = (OperateResult<byte[]>)null;
            }
            return OperateResult.CreateSuccessResult<byte[]>(bytesContent.ToArray());
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Siemens.SiemensS7Net.ReadBitFromPLC(System.String)" />
        private async Task<OperateResult<byte[]>> ReadBitFromPLCAsync(string address)
        {
            OperateResult<byte[]> command = SiemensS7Net.BuildBitReadCommand(address);
            if (!command.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)command);
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
            return read.IsSuccess ? SiemensS7Net.AnalysisReadBit(read.Content) : read;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Siemens.SiemensS7Net.Read(System.String[],System.UInt16[])" />
        public async Task<OperateResult<byte[]>> ReadAsync(
          string[] address,
          ushort[] length)
        {
            S7AddressData[] addressResult = new S7AddressData[address.Length];
            for (int i = 0; i < address.Length; ++i)
            {
                OperateResult<S7AddressData> tmp = S7AddressData.ParseFrom(address[i], length[i]);
                if (!tmp.IsSuccess)
                    return OperateResult.CreateFailedResult<byte[]>((OperateResult)tmp);
                addressResult[i] = tmp.Content;
                tmp = (OperateResult<S7AddressData>)null;
            }
            OperateResult<byte[]> operateResult = await this.ReadAsync(addressResult);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Siemens.SiemensS7Net.Read(ESTCore.Common.Core.Address.S7AddressData[])" />
        public async Task<OperateResult<byte[]>> ReadAsync(S7AddressData[] s7Addresses)
        {
            if (s7Addresses.Length > 19)
            {
                List<byte> bytes = new List<byte>();
                List<S7AddressData[]> groups = SoftBasic.ArraySplitByLength<S7AddressData>(s7Addresses, 19);
                for (int i = 0; i < groups.Count; ++i)
                {
                    OperateResult<byte[]> read = await this.ReadAsync(groups[i]);
                    if (!read.IsSuccess)
                        return read;
                    bytes.AddRange((IEnumerable<byte>)read.Content);
                    read = (OperateResult<byte[]>)null;
                }
                return OperateResult.CreateSuccessResult<byte[]>(bytes.ToArray());
            }
            OperateResult<byte[]> operateResult = await this.ReadS7AddressDataAsync(s7Addresses);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Siemens.SiemensS7Net.ReadS7AddressData(ESTCore.Common.Core.Address.S7AddressData[])" />
        private async Task<OperateResult<byte[]>> ReadS7AddressDataAsync(
          S7AddressData[] s7Addresses)
        {
            OperateResult<byte[]> command = SiemensS7Net.BuildReadCommand(s7Addresses);
            if (!command.IsSuccess)
                return command;
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
            return read.IsSuccess ? SiemensS7Net.AnalysisReadByte(s7Addresses, read.Content) : read;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Siemens.SiemensS7Net.WriteBase(System.Byte[])" />
        private async Task<OperateResult> WriteBaseAsync(byte[] entireValue)
        {
            OperateResult<byte[]> result = await this.ReadFromCoreServerAsync(entireValue);
            return ByteTransformHelper.GetResultFromOther<byte[]>(result, new Func<byte[], OperateResult>(SiemensS7Net.AnalysisWrite));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Siemens.SiemensS7Net.Write(System.String,System.Byte[])" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          byte[] value)
        {
            OperateResult<S7AddressData> analysis = S7AddressData.ParseFrom(address);
            if (!analysis.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<byte[]>((OperateResult)analysis);
            int length = value.Length;
            ushort alreadyFinished = 0;
            while ((int)alreadyFinished < length)
            {
                ushort writeLength = (ushort)Math.Min(length - (int)alreadyFinished, 200);
                byte[] buffer = this.ByteTransform.TransByte(value, (int)alreadyFinished, (int)writeLength);
                OperateResult<byte[]> command = SiemensS7Net.BuildWriteByteCommand(analysis, buffer);
                if (!command.IsSuccess)
                    return (OperateResult)command;
                OperateResult write = await this.WriteBaseAsync(command.Content);
                if (!write.IsSuccess)
                    return write;
                alreadyFinished += writeLength;
                analysis.Content.AddressStart += (int)writeLength * 8;
                buffer = (byte[])null;
                command = (OperateResult<byte[]>)null;
                write = (OperateResult)null;
            }
            return OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 读取指定地址的bool数据，地址格式为I100，M100，Q100，DB20.100<br />
        /// reads bool data for the specified address in the format I100，M100，Q100，DB20.100
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 -&gt;
        /// Starting address, formatted as I100,M100,Q100,DB20.100</param>
        /// <returns>是否读取成功的结果对象 -&gt; Whether to read the successful result object</returns>
        /// <remarks>
        /// <note type="important">
        /// 对于200smartPLC的V区，就是DB1.X，例如，V100=DB1.100
        /// </note>
        /// </remarks>
        /// <example>
        /// 假设读取M100.0的位是否通断
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\SiemensS7Net.cs" region="ReadBool" title="ReadBool示例" />
        /// </example>
        [EstMqttApi("ReadBool", "")]
        public override OperateResult<bool> ReadBool(string address) => ByteTransformHelper.GetResultFromBytes<bool>(this.ReadBitFromPLC(address), (Func<byte[], bool>)(m => m[0] > (byte)0));

        /// <summary>
        /// 读取指定地址的bool数组，地址格式为I100，M100，Q100，DB20.100<br />
        /// reads bool array data for the specified address in the format I100，M100，Q100，DB20.100
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 -&gt;
        /// Starting address, formatted as I100,M100,Q100,DB20.100</param>
        /// <param name="length">读取的长度信息</param>
        /// <returns>是否读取成功的结果对象 -&gt; Whether to read the successful result object</returns>
        /// <remarks>
        /// <note type="important">
        /// 对于200smartPLC的V区，就是DB1.X，例如，V100=DB1.100
        /// </note>
        /// </remarks>
        [EstMqttApi("ReadBoolArray", "")]
        public override OperateResult<bool[]> ReadBool(string address, ushort length)
        {
            OperateResult<S7AddressData> from = S7AddressData.ParseFrom(address);
            if (!from.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)from);
            int newStart;
            ushort byteLength;
            int offset;
            EstHelper.CalculateStartBitIndexAndLength(from.Content.AddressStart, length, out newStart, out byteLength, out offset);
            from.Content.AddressStart = newStart;
            from.Content.Length = byteLength;
            OperateResult<byte[]> operateResult = this.Read(new S7AddressData[1]
            {
        from.Content
            });
            return !operateResult.IsSuccess ? OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult) : OperateResult.CreateSuccessResult<bool[]>(operateResult.Content.ToBoolArray().SelectMiddle<bool>(offset, (int)length));
        }

        /// <summary>
        /// 写入PLC的一个位，例如"M100.6"，"I100.7"，"Q100.0"，"DB20.100.0"，如果只写了"M100"默认为"M100.0"<br />
        /// Write a bit of PLC, for example  "M100.6",  "I100.7",  "Q100.0",  "DB20.100.0", if only write  "M100" defaults to  "M100.0"
        /// </summary>
        /// <param name="address">起始地址，格式为"M100.6",  "I100.7",  "Q100.0",  "DB20.100.0" -&gt;
        /// Start address, format  "M100.6",  "I100.7",  "Q100.0",  "DB20.100.0"</param>
        /// <param name="value">写入的数据，True或是False -&gt; Writes the data, either True or False</param>
        /// <returns>是否写入成功的结果对象 -&gt; Whether to write a successful result object</returns>
        /// <example>
        /// 假设写入M100.0的位是否通断
        /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\SiemensS7Net.cs" region="WriteBool" title="WriteBool示例" />
        /// </example>
        [EstMqttApi("WriteBool", "")]
        public override OperateResult Write(string address, bool value)
        {
            OperateResult<byte[]> operateResult = SiemensS7Net.BuildWriteBitCommand(address, value);
            return !operateResult.IsSuccess ? (OperateResult)operateResult : this.WriteBase(operateResult.Content);
        }

        /// <summary>
        /// [危险] 向PLC中写入bool数组，比如你写入M100,那么data[0]对应M100.0<br />
        /// [Danger] Write the bool array to the PLC, for example, if you write M100, then data[0] corresponds to M100.0
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 -&gt; Starting address, formatted as I100,mM100,Q100,DB20.100</param>
        /// <param name="values">要写入的bool数组，长度为8的倍数 -&gt; The bool array to write, a multiple of 8 in length</param>
        /// <returns>是否写入成功的结果对象 -&gt; Whether to write a successful result object</returns>
        /// <remarks>
        /// <note type="warning">
        /// 批量写入bool数组存在一定的风险，原因是只能批量写入长度为8的倍数的数组，否则会影响其他的位的数据，请谨慎使用。<br />
        /// There is a certain risk in batch writing to bool arrays, because you can only batch write arrays whose length is a multiple of 8,
        /// otherwise it will affect other bit data. Please use it with caution.
        /// </note>
        /// </remarks>
        [EstMqttApi("WriteBoolArray", "")]
        public override OperateResult Write(string address, bool[] values) => this.Write(address, SoftBasic.BoolArrayToByte(values));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Siemens.SiemensS7Net.ReadBool(System.String)" />
        public override async Task<OperateResult<bool>> ReadBoolAsync(string address)
        {
            OperateResult<byte[]> result = await this.ReadBitFromPLCAsync(address);
            return ByteTransformHelper.GetResultFromBytes<bool>(result, (Func<byte[], bool>)(m => m[0] > (byte)0));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Siemens.SiemensS7Net.ReadBool(System.String,System.UInt16)" />
        public override async Task<OperateResult<bool[]>> ReadBoolAsync(
          string address,
          ushort length)
        {
            OperateResult<S7AddressData> analysis = S7AddressData.ParseFrom(address);
            if (!analysis.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)analysis);
            int newStart;
            ushort byteLength;
            int offset;
            EstHelper.CalculateStartBitIndexAndLength(analysis.Content.AddressStart, length, out newStart, out byteLength, out offset);
            analysis.Content.AddressStart = newStart;
            analysis.Content.Length = byteLength;
            OperateResult<byte[]> read = await this.ReadAsync(new S7AddressData[1]
            {
        analysis.Content
            });
            return read.IsSuccess ? OperateResult.CreateSuccessResult<bool[]>(read.Content.ToBoolArray().SelectMiddle<bool>(offset, (int)length)) : OperateResult.CreateFailedResult<bool[]>((OperateResult)read);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Siemens.SiemensS7Net.Write(System.String,System.Boolean)" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          bool value)
        {
            OperateResult<byte[]> command = SiemensS7Net.BuildWriteBitCommand(address, value);
            if (!command.IsSuccess)
                return (OperateResult)command;
            OperateResult operateResult = await this.WriteBaseAsync(command.Content);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Siemens.SiemensS7Net.Write(System.String,System.Boolean[])" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          bool[] values)
        {
            OperateResult operateResult = await this.WriteAsync(address, SoftBasic.BoolArrayToByte(values));
            return operateResult;
        }

        /// <summary>
        /// 读取指定地址的byte数据，地址格式I100，M100，Q100，DB20.100<br />
        /// Reads the byte data of the specified address, the address format I100,Q100,DB20.100,M100
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 -&gt;
        /// Starting address, formatted as I100,M100,Q100,DB20.100</param>
        /// <returns>是否读取成功的结果对象 -&gt; Whether to read the successful result object</returns>
        /// <example>参考<see cref="M:ESTCore.Common.Profinet.Siemens.SiemensS7Net.Read(System.String,System.UInt16)" />的注释</example>
        [EstMqttApi("ReadByte", "")]
        public OperateResult<byte> ReadByte(string address) => ByteTransformHelper.GetResultFromArray<byte>(this.Read(address, (ushort)1));

        /// <summary>
        /// 向PLC中写入byte数据，返回值说明<br />
        /// Write byte data to the PLC, return value description
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 -&gt; Starting address, formatted as I100,mM100,Q100,DB20.100</param>
        /// <param name="value">byte数据 -&gt; Byte data</param>
        /// <returns>是否写入成功的结果对象 -&gt; Whether to write a successful result object</returns>
        [EstMqttApi("WriteByte", "")]
        public OperateResult Write(string address, byte value) => this.Write(address, new byte[1]
        {
      value
        });

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Siemens.SiemensS7Net.ReadByte(System.String)" />
        public async Task<OperateResult<byte>> ReadByteAsync(string address)
        {
            OperateResult<byte[]> result = await this.ReadAsync(address, (ushort)1);
            return ByteTransformHelper.GetResultFromArray<byte>(result);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Siemens.SiemensS7Net.Write(System.String,System.Byte)" />
        public async Task<OperateResult> WriteAsync(string address, byte value)
        {
            OperateResult operateResult = await this.WriteAsync(address, new byte[1]
            {
        value
            });
            return operateResult;
        }

        /// <inheritdoc />
        public override OperateResult Write(
          string address,
          string value,
          Encoding encoding)
        {
            if (value == null)
                value = string.Empty;
            byte[] inBytes = encoding.GetBytes(value);
            if (encoding == Encoding.Unicode)
                inBytes = SoftBasic.BytesReverseByWord(inBytes);
            if (this.CurrentPlc != SiemensPLCS.S200Smart)
            {
                OperateResult<byte[]> operateResult = this.Read(address, (ushort)2);
                if (!operateResult.IsSuccess)
                    return (OperateResult)operateResult;
                if (operateResult.Content[0] == byte.MaxValue)
                    return (OperateResult)new OperateResult<string>("Value in plc is not string type");
                if (operateResult.Content[0] == (byte)0)
                    operateResult.Content[0] = (byte)254;
                if (value.Length > (int)operateResult.Content[0])
                    return (OperateResult)new OperateResult<string>("String length is too long than plc defined");
                return this.Write(address, SoftBasic.SpliceArray<byte>(new byte[2]
                {
          operateResult.Content[0],
          (byte) value.Length
                }, inBytes));
            }
            return this.Write(address, SoftBasic.SpliceArray<byte>(new byte[1]
            {
        (byte) value.Length
            }, inBytes));
        }

        /// <summary>
        /// 使用双字节编码的方式，将字符串以 Unicode 编码写入到PLC的地址里，可以使用中文。<br />
        /// Use the double-byte encoding method to write the character string to the address of the PLC in Unicode encoding. Chinese can be used.
        /// </summary>
        /// <param name="address">起始地址，格式为I100，M100，Q100，DB20.100 -&gt; Starting address, formatted as I100,mM100,Q100,DB20.100</param>
        /// <param name="value">字符串的值</param>
        /// <returns>是否写入成功的结果对象</returns>
        [EstMqttApi(ApiTopic = "WriteWString", Description = "写入unicode编码的字符串，支持中文")]
        public OperateResult WriteWString(string address, string value) => this.Write(address, value, Encoding.Unicode);

        /// <inheritdoc />
        public override OperateResult<string> ReadString(
          string address,
          ushort length,
          Encoding encoding)
        {
            return length == (ushort)0 ? this.ReadString(address) : base.ReadString(address, length, encoding);
        }

        /// <summary>
        /// 读取西门子的地址的字符串信息，这个信息是和西门子绑定在一起，长度随西门子的信息动态变化的<br />
        /// Read the Siemens address string information. This information is bound to Siemens and its length changes dynamically with the Siemens information
        /// </summary>
        /// <param name="address">数据地址，具体的格式需要参照类的说明文档</param>
        /// <returns>带有是否成功的字符串结果类对象</returns>
        [EstMqttApi("ReadS7String", "读取S7格式的字符串")]
        public OperateResult<string> ReadString(string address)
        {
            if (this.CurrentPlc != SiemensPLCS.S200Smart)
            {
                OperateResult<byte[]> operateResult1 = this.Read(address, (ushort)2);
                if (!operateResult1.IsSuccess)
                    return OperateResult.CreateFailedResult<string>((OperateResult)operateResult1);
                if (operateResult1.Content[0] == (byte)0 || operateResult1.Content[0] == byte.MaxValue)
                    return new OperateResult<string>("Value in plc is not string type");
                OperateResult<byte[]> operateResult2 = this.Read(address, (ushort)(2U + (uint)operateResult1.Content[1]));
                return !operateResult2.IsSuccess ? OperateResult.CreateFailedResult<string>((OperateResult)operateResult2) : OperateResult.CreateSuccessResult<string>(Encoding.ASCII.GetString(operateResult2.Content, 2, operateResult2.Content.Length - 2));
            }
            OperateResult<byte[]> operateResult3 = this.Read(address, (ushort)1);
            if (!operateResult3.IsSuccess)
                return OperateResult.CreateFailedResult<string>((OperateResult)operateResult3);
            OperateResult<byte[]> operateResult4 = this.Read(address, (ushort)(1U + (uint)operateResult3.Content[0]));
            return !operateResult4.IsSuccess ? OperateResult.CreateFailedResult<string>((OperateResult)operateResult4) : OperateResult.CreateSuccessResult<string>(Encoding.ASCII.GetString(operateResult4.Content, 1, operateResult4.Content.Length - 1));
        }

        /// <summary>
        /// 读取西门子的地址的字符串信息，这个信息是和西门子绑定在一起，长度随西门子的信息动态变化的<br />
        /// Read the Siemens address string information. This information is bound to Siemens and its length changes dynamically with the Siemens information
        /// </summary>
        /// <param name="address">数据地址，具体的格式需要参照类的说明文档</param>
        /// <returns>带有是否成功的字符串结果类对象</returns>
        [EstMqttApi("ReadWString", "读取S7格式的双字节字符串")]
        public OperateResult<string> ReadWString(string address)
        {
            if (this.CurrentPlc != SiemensPLCS.S200Smart)
            {
                OperateResult<byte[]> operateResult1 = this.Read(address, (ushort)2);
                if (!operateResult1.IsSuccess)
                    return OperateResult.CreateFailedResult<string>((OperateResult)operateResult1);
                if (operateResult1.Content[0] == (byte)0 || operateResult1.Content[0] == byte.MaxValue)
                    return new OperateResult<string>("Value in plc is not string type");
                OperateResult<byte[]> operateResult2 = this.Read(address, (ushort)(2 + (int)operateResult1.Content[1] * 2));
                return !operateResult2.IsSuccess ? OperateResult.CreateFailedResult<string>((OperateResult)operateResult2) : OperateResult.CreateSuccessResult<string>(Encoding.Unicode.GetString(SoftBasic.BytesReverseByWord(operateResult2.Content.RemoveBegin<byte>(2))));
            }
            OperateResult<byte[]> operateResult3 = this.Read(address, (ushort)1);
            if (!operateResult3.IsSuccess)
                return OperateResult.CreateFailedResult<string>((OperateResult)operateResult3);
            OperateResult<byte[]> operateResult4 = this.Read(address, (ushort)(1 + (int)operateResult3.Content[0] * 2));
            return !operateResult4.IsSuccess ? OperateResult.CreateFailedResult<string>((OperateResult)operateResult4) : OperateResult.CreateSuccessResult<string>(Encoding.Unicode.GetString(operateResult4.Content, 1, operateResult4.Content.Length - 1));
        }

        /// <inheritdoc />
        public override async Task<OperateResult> WriteAsync(
          string address,
          string value,
          Encoding encoding)
        {
            if (value == null)
                value = string.Empty;
            byte[] buffer = encoding.GetBytes(value);
            if (encoding == Encoding.Unicode)
                buffer = SoftBasic.BytesReverseByWord(buffer);
            if (this.CurrentPlc != SiemensPLCS.S200Smart)
            {
                OperateResult<byte[]> readLength = await this.ReadAsync(address, (ushort)2);
                if (!readLength.IsSuccess)
                    return (OperateResult)readLength;
                if (readLength.Content[0] == byte.MaxValue)
                    return (OperateResult)new OperateResult<string>("Value in plc is not string type");
                if (readLength.Content[0] == (byte)0)
                    readLength.Content[0] = (byte)254;
                if (value.Length > (int)readLength.Content[0])
                    return (OperateResult)new OperateResult<string>("String length is too long than plc defined");
                OperateResult operateResult = await this.WriteAsync(address, SoftBasic.SpliceArray<byte>(new byte[2]
                {
          readLength.Content[0],
          (byte) value.Length
                }, buffer));
                return operateResult;
            }
            OperateResult operateResult1 = await this.WriteAsync(address, SoftBasic.SpliceArray<byte>(new byte[1]
            {
        (byte) value.Length
            }, buffer));
            return operateResult1;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Siemens.SiemensS7Net.WriteWString(System.String,System.String)" />
        public async Task<OperateResult> WriteWStringAsync(
          string address,
          string value)
        {
            OperateResult operateResult = await this.WriteAsync(address, value, Encoding.Unicode);
            return operateResult;
        }

        /// <inheritdoc />
        public override async Task<OperateResult<string>> ReadStringAsync(
          string address,
          ushort length,
          Encoding encoding)
        {
            if (length == (ushort)0)
            {
                OperateResult<string> operateResult = await this.ReadStringAsync(address);
                return operateResult;
            }
            OperateResult<string> operateResult1 = await base.ReadStringAsync(address, length, encoding);
            return operateResult1;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Siemens.SiemensS7Net.ReadString(System.String)" />
        public async Task<OperateResult<string>> ReadStringAsync(string address)
        {
            if (this.CurrentPlc != SiemensPLCS.S200Smart)
            {
                OperateResult<byte[]> read = await this.ReadAsync(address, (ushort)2);
                if (!read.IsSuccess)
                    return OperateResult.CreateFailedResult<string>((OperateResult)read);
                if (read.Content[0] == (byte)0 || read.Content[0] == byte.MaxValue)
                    return new OperateResult<string>("Value in plc is not string type");
                OperateResult<byte[]> readString = await this.ReadAsync(address, (ushort)(2U + (uint)read.Content[1]));
                return readString.IsSuccess ? OperateResult.CreateSuccessResult<string>(Encoding.ASCII.GetString(readString.Content, 2, readString.Content.Length - 2)) : OperateResult.CreateFailedResult<string>((OperateResult)readString);
            }
            OperateResult<byte[]> read1 = await this.ReadAsync(address, (ushort)1);
            if (!read1.IsSuccess)
                return OperateResult.CreateFailedResult<string>((OperateResult)read1);
            OperateResult<byte[]> readString1 = await this.ReadAsync(address, (ushort)(1U + (uint)read1.Content[0]));
            return readString1.IsSuccess ? OperateResult.CreateSuccessResult<string>(Encoding.ASCII.GetString(readString1.Content, 1, readString1.Content.Length - 1)) : OperateResult.CreateFailedResult<string>((OperateResult)readString1);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Siemens.SiemensS7Net.ReadWString(System.String)" />
        public async Task<OperateResult<string>> ReadWStringAsync(string address)
        {
            if (this.CurrentPlc != SiemensPLCS.S200Smart)
            {
                OperateResult<byte[]> read = await this.ReadAsync(address, (ushort)2);
                if (!read.IsSuccess)
                    return OperateResult.CreateFailedResult<string>((OperateResult)read);
                if (read.Content[0] == (byte)0 || read.Content[0] == byte.MaxValue)
                    return new OperateResult<string>("Value in plc is not string type");
                OperateResult<byte[]> readString = await this.ReadAsync(address, (ushort)(2 + (int)read.Content[1] * 2));
                return readString.IsSuccess ? OperateResult.CreateSuccessResult<string>(Encoding.Unicode.GetString(SoftBasic.BytesReverseByWord(readString.Content.RemoveBegin<byte>(2)))) : OperateResult.CreateFailedResult<string>((OperateResult)readString);
            }
            OperateResult<byte[]> read1 = await this.ReadAsync(address, (ushort)1);
            if (!read1.IsSuccess)
                return OperateResult.CreateFailedResult<string>((OperateResult)read1);
            OperateResult<byte[]> readString1 = await this.ReadAsync(address, (ushort)(1 + (int)read1.Content[0] * 2));
            return readString1.IsSuccess ? OperateResult.CreateSuccessResult<string>(Encoding.Unicode.GetString(readString1.Content, 1, readString1.Content.Length - 1)) : OperateResult.CreateFailedResult<string>((OperateResult)readString1);
        }

        /// <summary>
        /// 从PLC中读取时间格式的数据<br />
        /// Read time format data from PLC
        /// </summary>
        /// <param name="address">地址</param>
        /// <returns>时间对象</returns>
        [EstMqttApi("ReadDateTime", "读取PLC的时间格式的数据，这个格式是s7格式的一种")]
        public OperateResult<DateTime> ReadDateTime(string address) => ByteTransformHelper.GetResultFromBytes<DateTime>(this.Read(address, (ushort)8), new Func<byte[], DateTime>(SiemensDateTime.FromByteArray));

        /// <summary>
        /// 向PLC中写入时间格式的数据<br />
        /// Writes data in time format to the PLC
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="dateTime">时间</param>
        /// <returns>是否写入成功</returns>
        [EstMqttApi("WriteDateTime", "写入PLC的时间格式的数据，这个格式是s7格式的一种")]
        public OperateResult Write(string address, DateTime dateTime) => this.Write(address, SiemensDateTime.ToByteArray(dateTime));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Siemens.SiemensS7Net.ReadDateTime(System.String)" />
        public async Task<OperateResult<DateTime>> ReadDateTimeAsync(
          string address)
        {
            OperateResult<byte[]> result = await this.ReadAsync(address, (ushort)8);
            return ByteTransformHelper.GetResultFromBytes<DateTime>(result, new Func<byte[], DateTime>(SiemensDateTime.FromByteArray));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Siemens.SiemensS7Net.Write(System.String,System.DateTime)" />
        public async Task<OperateResult> WriteAsync(string address, DateTime dateTime)
        {
            OperateResult operateResult = await this.WriteAsync(address, SiemensDateTime.ToByteArray(dateTime));
            return operateResult;
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("SiemensS7Net {0}[{1}:{2}]", (object)this.CurrentPlc, (object)this.IpAddress, (object)this.Port);

        /// <summary>
        /// A general method for generating a command header to read a Word data
        /// </summary>
        /// <param name="s7Addresses">siemens address</param>
        /// <returns>Message containing the result object</returns>
        public static OperateResult<byte[]> BuildReadCommand(S7AddressData[] s7Addresses)
        {
            if (s7Addresses == null)
                throw new NullReferenceException(nameof(s7Addresses));
            int num = s7Addresses.Length <= 19 ? s7Addresses.Length : throw new Exception(StringResources.Language.SiemensReadLengthCannotLargerThan19);
            byte[] numArray = new byte[19 + num * 12];
            numArray[0] = (byte)3;
            numArray[1] = (byte)0;
            numArray[2] = (byte)(numArray.Length / 256);
            numArray[3] = (byte)(numArray.Length % 256);
            numArray[4] = (byte)2;
            numArray[5] = (byte)240;
            numArray[6] = (byte)128;
            numArray[7] = (byte)50;
            numArray[8] = (byte)1;
            numArray[9] = (byte)0;
            numArray[10] = (byte)0;
            numArray[11] = (byte)0;
            numArray[12] = (byte)1;
            numArray[13] = (byte)((numArray.Length - 17) / 256);
            numArray[14] = (byte)((numArray.Length - 17) % 256);
            numArray[15] = (byte)0;
            numArray[16] = (byte)0;
            numArray[17] = (byte)4;
            numArray[18] = (byte)num;
            for (int index = 0; index < num; ++index)
            {
                numArray[19 + index * 12] = (byte)18;
                numArray[20 + index * 12] = (byte)10;
                numArray[21 + index * 12] = (byte)16;
                if (s7Addresses[index].DataCode == (byte)30 || s7Addresses[index].DataCode == (byte)31)
                {
                    numArray[22 + index * 12] = s7Addresses[index].DataCode;
                    numArray[23 + index * 12] = (byte)((int)s7Addresses[index].Length / 2 / 256);
                    numArray[24 + index * 12] = (byte)((int)s7Addresses[index].Length / 2 % 256);
                }
                else if (s7Addresses[index].DataCode == (byte)6 | s7Addresses[index].DataCode == (byte)7)
                {
                    numArray[22 + index * 12] = (byte)4;
                    numArray[23 + index * 12] = (byte)((int)s7Addresses[index].Length / 2 / 256);
                    numArray[24 + index * 12] = (byte)((int)s7Addresses[index].Length / 2 % 256);
                }
                else
                {
                    numArray[22 + index * 12] = (byte)2;
                    numArray[23 + index * 12] = (byte)((uint)s7Addresses[index].Length / 256U);
                    numArray[24 + index * 12] = (byte)((uint)s7Addresses[index].Length % 256U);
                }
                numArray[25 + index * 12] = (byte)((uint)s7Addresses[index].DbBlock / 256U);
                numArray[26 + index * 12] = (byte)((uint)s7Addresses[index].DbBlock % 256U);
                numArray[27 + index * 12] = s7Addresses[index].DataCode;
                numArray[28 + index * 12] = (byte)(s7Addresses[index].AddressStart / 256 / 256 % 256);
                numArray[29 + index * 12] = (byte)(s7Addresses[index].AddressStart / 256 % 256);
                numArray[30 + index * 12] = (byte)(s7Addresses[index].AddressStart % 256);
            }
            return OperateResult.CreateSuccessResult<byte[]>(numArray);
        }

        /// <summary>
        /// 生成一个位读取数据指令头的通用方法 -&gt;
        /// A general method for generating a bit-read-Data instruction header
        /// </summary>
        /// <param name="address">起始地址，例如M100.0，I0.1，Q0.1，DB2.100.2 -&gt;
        /// Start address, such as M100.0,I0.1,Q0.1,DB2.100.2
        /// </param>
        /// <returns>包含结果对象的报文 -&gt; Message containing the result object</returns>
        public static OperateResult<byte[]> BuildBitReadCommand(string address)
        {
            OperateResult<S7AddressData> from = S7AddressData.ParseFrom(address);
            if (!from.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)from);
            byte[] numArray = new byte[31];
            numArray[0] = (byte)3;
            numArray[1] = (byte)0;
            numArray[2] = (byte)(numArray.Length / 256);
            numArray[3] = (byte)(numArray.Length % 256);
            numArray[4] = (byte)2;
            numArray[5] = (byte)240;
            numArray[6] = (byte)128;
            numArray[7] = (byte)50;
            numArray[8] = (byte)1;
            numArray[9] = (byte)0;
            numArray[10] = (byte)0;
            numArray[11] = (byte)0;
            numArray[12] = (byte)1;
            numArray[13] = (byte)((numArray.Length - 17) / 256);
            numArray[14] = (byte)((numArray.Length - 17) % 256);
            numArray[15] = (byte)0;
            numArray[16] = (byte)0;
            numArray[17] = (byte)4;
            numArray[18] = (byte)1;
            numArray[19] = (byte)18;
            numArray[20] = (byte)10;
            numArray[21] = (byte)16;
            numArray[22] = (byte)1;
            numArray[23] = (byte)0;
            numArray[24] = (byte)1;
            numArray[25] = (byte)((uint)from.Content.DbBlock / 256U);
            numArray[26] = (byte)((uint)from.Content.DbBlock % 256U);
            numArray[27] = from.Content.DataCode;
            numArray[28] = (byte)(from.Content.AddressStart / 256 / 256 % 256);
            numArray[29] = (byte)(from.Content.AddressStart / 256 % 256);
            numArray[30] = (byte)(from.Content.AddressStart % 256);
            return OperateResult.CreateSuccessResult<byte[]>(numArray);
        }

        /// <summary>
        /// 生成一个写入字节数据的指令 -&gt; Generate an instruction to write byte data
        /// </summary>
        /// <param name="analysis">起始地址，示例M100,I100,Q100,DB1.100 -&gt; Start Address, example M100,I100,Q100,DB1.100</param>
        /// <param name="data">原始的字节数据 -&gt; Raw byte data</param>
        /// <returns>包含结果对象的报文 -&gt; Message containing the result object</returns>
        public static OperateResult<byte[]> BuildWriteByteCommand(
          OperateResult<S7AddressData> analysis,
          byte[] data)
        {
            byte[] numArray = new byte[35 + data.Length];
            numArray[0] = (byte)3;
            numArray[1] = (byte)0;
            numArray[2] = (byte)((35 + data.Length) / 256);
            numArray[3] = (byte)((35 + data.Length) % 256);
            numArray[4] = (byte)2;
            numArray[5] = (byte)240;
            numArray[6] = (byte)128;
            numArray[7] = (byte)50;
            numArray[8] = (byte)1;
            numArray[9] = (byte)0;
            numArray[10] = (byte)0;
            numArray[11] = (byte)0;
            numArray[12] = (byte)1;
            numArray[13] = (byte)0;
            numArray[14] = (byte)14;
            numArray[15] = (byte)((4 + data.Length) / 256);
            numArray[16] = (byte)((4 + data.Length) % 256);
            numArray[17] = (byte)5;
            numArray[18] = (byte)1;
            numArray[19] = (byte)18;
            numArray[20] = (byte)10;
            numArray[21] = (byte)16;
            if (analysis.Content.DataCode == (byte)6 || analysis.Content.DataCode == (byte)7)
            {
                numArray[22] = (byte)4;
                numArray[23] = (byte)(data.Length / 2 / 256);
                numArray[24] = (byte)(data.Length / 2 % 256);
            }
            else
            {
                numArray[22] = (byte)2;
                numArray[23] = (byte)(data.Length / 256);
                numArray[24] = (byte)(data.Length % 256);
            }
            numArray[25] = (byte)((uint)analysis.Content.DbBlock / 256U);
            numArray[26] = (byte)((uint)analysis.Content.DbBlock % 256U);
            numArray[27] = analysis.Content.DataCode;
            numArray[28] = (byte)(analysis.Content.AddressStart / 256 / 256 % 256);
            numArray[29] = (byte)(analysis.Content.AddressStart / 256 % 256);
            numArray[30] = (byte)(analysis.Content.AddressStart % 256);
            numArray[31] = (byte)0;
            numArray[32] = (byte)4;
            numArray[33] = (byte)(data.Length * 8 / 256);
            numArray[34] = (byte)(data.Length * 8 % 256);
            data.CopyTo((Array)numArray, 35);
            return OperateResult.CreateSuccessResult<byte[]>(numArray);
        }

        /// <summary>
        /// 生成一个写入位数据的指令 -&gt; Generate an instruction to write bit data
        /// </summary>
        /// <param name="address">起始地址，示例M100,I100,Q100,DB1.100 -&gt; Start Address, example M100,I100,Q100,DB1.100</param>
        /// <param name="data">是否通断 -&gt; Power on or off</param>
        /// <returns>包含结果对象的报文 -&gt; Message containing the result object</returns>
        public static OperateResult<byte[]> BuildWriteBitCommand(string address, bool data)
        {
            OperateResult<S7AddressData> from = S7AddressData.ParseFrom(address);
            if (!from.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)from);
            byte[] numArray1 = new byte[1]
            {
        data ? (byte) 1 : (byte) 0
            };
            byte[] numArray2 = new byte[35 + numArray1.Length];
            numArray2[0] = (byte)3;
            numArray2[1] = (byte)0;
            numArray2[2] = (byte)((35 + numArray1.Length) / 256);
            numArray2[3] = (byte)((35 + numArray1.Length) % 256);
            numArray2[4] = (byte)2;
            numArray2[5] = (byte)240;
            numArray2[6] = (byte)128;
            numArray2[7] = (byte)50;
            numArray2[8] = (byte)1;
            numArray2[9] = (byte)0;
            numArray2[10] = (byte)0;
            numArray2[11] = (byte)0;
            numArray2[12] = (byte)1;
            numArray2[13] = (byte)0;
            numArray2[14] = (byte)14;
            numArray2[15] = (byte)((4 + numArray1.Length) / 256);
            numArray2[16] = (byte)((4 + numArray1.Length) % 256);
            numArray2[17] = (byte)5;
            numArray2[18] = (byte)1;
            numArray2[19] = (byte)18;
            numArray2[20] = (byte)10;
            numArray2[21] = (byte)16;
            numArray2[22] = (byte)1;
            numArray2[23] = (byte)(numArray1.Length / 256);
            numArray2[24] = (byte)(numArray1.Length % 256);
            numArray2[25] = (byte)((uint)from.Content.DbBlock / 256U);
            numArray2[26] = (byte)((uint)from.Content.DbBlock % 256U);
            numArray2[27] = from.Content.DataCode;
            numArray2[28] = (byte)(from.Content.AddressStart / 256 / 256);
            numArray2[29] = (byte)(from.Content.AddressStart / 256);
            numArray2[30] = (byte)(from.Content.AddressStart % 256);
            if (from.Content.DataCode == (byte)28)
            {
                numArray2[31] = (byte)0;
                numArray2[32] = (byte)9;
            }
            else
            {
                numArray2[31] = (byte)0;
                numArray2[32] = (byte)3;
            }
            numArray2[33] = (byte)(numArray1.Length / 256);
            numArray2[34] = (byte)(numArray1.Length % 256);
            numArray1.CopyTo((Array)numArray2, 35);
            return OperateResult.CreateSuccessResult<byte[]>(numArray2);
        }

        private static OperateResult<byte[]> AnalysisReadByte(
          S7AddressData[] s7Addresses,
          byte[] content)
        {
            int length = 0;
            for (int index = 0; index < s7Addresses.Length; ++index)
            {
                if (s7Addresses[index].DataCode == (byte)31 || s7Addresses[index].DataCode == (byte)30)
                    length += (int)s7Addresses[index].Length * 2;
                else
                    length += (int)s7Addresses[index].Length;
            }
            if (content.Length < 21 || (int)content[20] != s7Addresses.Length)
                return new OperateResult<byte[]>(StringResources.Language.SiemensDataLengthCheckFailed + " Msg:" + SoftBasic.ByteToHexString(content, ' '));
            byte[] numArray = new byte[length];
            int index1 = 0;
            int destinationIndex = 0;
            for (int index2 = 21; index2 < content.Length; ++index2)
            {
                if (index2 + 1 < content.Length)
                {
                    if (content[index2] == byte.MaxValue && content[index2 + 1] == (byte)4)
                    {
                        Array.Copy((Array)content, index2 + 4, (Array)numArray, destinationIndex, (int)s7Addresses[index1].Length);
                        index2 += (int)s7Addresses[index1].Length + 3;
                        destinationIndex += (int)s7Addresses[index1].Length;
                        ++index1;
                    }
                    else if (content[index2] == byte.MaxValue && content[index2 + 1] == (byte)9)
                    {
                        int num = (int)content[index2 + 2] * 256 + (int)content[index2 + 3];
                        if (num % 3 == 0)
                        {
                            for (int index3 = 0; index3 < num / 3; ++index3)
                            {
                                Array.Copy((Array)content, index2 + 5 + 3 * index3, (Array)numArray, destinationIndex, 2);
                                destinationIndex += 2;
                            }
                        }
                        else
                        {
                            for (int index3 = 0; index3 < num / 5; ++index3)
                            {
                                Array.Copy((Array)content, index2 + 7 + 5 * index3, (Array)numArray, destinationIndex, 2);
                                destinationIndex += 2;
                            }
                        }
                        index2 += num + 4;
                        ++index1;
                    }
                    else
                    {
                        if (content[index2] == (byte)5 && content[index2 + 1] == (byte)0)
                            return new OperateResult<byte[]>((int)content[index2], StringResources.Language.SiemensReadLengthOverPlcAssign);
                        if (content[index2] == (byte)6 && content[index2 + 1] == (byte)0)
                            return new OperateResult<byte[]>((int)content[index2], StringResources.Language.SiemensError0006);
                        if (content[index2] == (byte)10 && content[index2 + 1] == (byte)0)
                            return new OperateResult<byte[]>((int)content[index2], StringResources.Language.SiemensError000A);
                    }
                }
            }
            return OperateResult.CreateSuccessResult<byte[]>(numArray);
        }

        private static OperateResult<byte[]> AnalysisReadBit(byte[] content)
        {
            int length = 1;
            if (content.Length < 21 || content[20] != (byte)1)
                return new OperateResult<byte[]>(StringResources.Language.SiemensDataLengthCheckFailed);
            byte[] numArray = new byte[length];
            if (22 < content.Length && (content[21] == byte.MaxValue && content[22] == (byte)3))
                numArray[0] = content[25];
            return OperateResult.CreateSuccessResult<byte[]>(numArray);
        }

        private static OperateResult AnalysisWrite(byte[] content)
        {
            byte num = content[content.Length - 1];
            return num != byte.MaxValue ? new OperateResult((int)num, StringResources.Language.SiemensWriteError + num.ToString() + " Msg:" + SoftBasic.ByteToHexString(content, ' ')) : OperateResult.CreateSuccessResult();
        }
    }
}
