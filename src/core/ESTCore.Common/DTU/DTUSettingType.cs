// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.DTU.DTUSettingType
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.Core.Net;
using ESTCore.Common.ModBus;
using ESTCore.Common.Profinet.AllenBradley;
using ESTCore.Common.Profinet.Melsec;
using ESTCore.Common.Profinet.Omron;
using ESTCore.Common.Profinet.Siemens;

using Newtonsoft.Json.Linq;

using System;

namespace ESTCore.Common.DTU
{
    /// <summary>DTU的类型设置器</summary>
    public class DTUSettingType
    {
        /// <summary>设备的唯一ID信息</summary>
        public string DtuId { get; set; }

        /// <summary>当前的设备的类型</summary>
        public string DtuType { get; set; } = "ModbusRtuOverTcp";

        /// <summary>额外的参数都存放在json里面</summary>
        public string JsonParameter { get; set; } = "{}";

        /// <summary>根据类型，获取连接对象</summary>
        /// <returns>获取设备的连接对象</returns>
        public virtual NetworkDeviceBase GetClient()
        {
            JObject jobject = JObject.Parse(this.JsonParameter);
            if (this.DtuType == "ModbusRtuOverTcp")
            {
                ModbusRtuOverTcp modbusRtuOverTcp = new ModbusRtuOverTcp("127.0.0.1", station: jobject["Station"].Value<byte>());
                modbusRtuOverTcp.ConnectionId = this.DtuId;
                return (NetworkDeviceBase)modbusRtuOverTcp;
            }
            if (this.DtuType == "ModbusTcpNet")
            {
                ModbusTcpNet modbusTcpNet = new ModbusTcpNet("127.0.0.1", station: jobject["Station"].Value<byte>());
                modbusTcpNet.ConnectionId = this.DtuId;
                return (NetworkDeviceBase)modbusTcpNet;
            }
            if (this.DtuType == "MelsecMcNet")
            {
                MelsecMcNet melsecMcNet = new MelsecMcNet("127.0.0.1", 5000);
                melsecMcNet.ConnectionId = this.DtuId;
                return (NetworkDeviceBase)melsecMcNet;
            }
            if (this.DtuType == "MelsecMcAsciiNet")
            {
                MelsecMcAsciiNet melsecMcAsciiNet = new MelsecMcAsciiNet("127.0.0.1", 5000);
                melsecMcAsciiNet.ConnectionId = this.DtuId;
                return (NetworkDeviceBase)melsecMcAsciiNet;
            }
            if (this.DtuType == "MelsecA1ENet")
            {
                MelsecA1ENet melsecA1Enet = new MelsecA1ENet("127.0.0.1", 5000);
                melsecA1Enet.ConnectionId = this.DtuId;
                return (NetworkDeviceBase)melsecA1Enet;
            }
            if (this.DtuType == "MelsecA1EAsciiNet")
            {
                MelsecA1EAsciiNet melsecA1EasciiNet = new MelsecA1EAsciiNet("127.0.0.1", 5000);
                melsecA1EasciiNet.ConnectionId = this.DtuId;
                return (NetworkDeviceBase)melsecA1EasciiNet;
            }
            if (this.DtuType == "MelsecA3CNet1OverTcp")
            {
                MelsecA3CNet1OverTcp melsecA3Cnet1OverTcp = new MelsecA3CNet1OverTcp("127.0.0.1", 5000);
                melsecA3Cnet1OverTcp.ConnectionId = this.DtuId;
                return (NetworkDeviceBase)melsecA3Cnet1OverTcp;
            }
            if (this.DtuType == "MelsecFxLinksOverTcp")
            {
                MelsecFxLinksOverTcp melsecFxLinksOverTcp = new MelsecFxLinksOverTcp("127.0.0.1", 5000);
                melsecFxLinksOverTcp.ConnectionId = this.DtuId;
                return (NetworkDeviceBase)melsecFxLinksOverTcp;
            }
            if (this.DtuType == "MelsecFxSerialOverTcp")
            {
                MelsecFxSerialOverTcp melsecFxSerialOverTcp = new MelsecFxSerialOverTcp("127.0.0.1", 5000);
                melsecFxSerialOverTcp.ConnectionId = this.DtuId;
                return (NetworkDeviceBase)melsecFxSerialOverTcp;
            }
            if (this.DtuType == "SiemensS7Net")
            {
                SiemensS7Net siemensS7Net = new SiemensS7Net((SiemensPLCS)Enum.Parse(typeof(SiemensPLCS), jobject["SiemensPLCS"].Value<string>()));
                siemensS7Net.ConnectionId = this.DtuId;
                return (NetworkDeviceBase)siemensS7Net;
            }
            if (this.DtuType == "SiemensFetchWriteNet")
            {
                SiemensFetchWriteNet siemensFetchWriteNet = new SiemensFetchWriteNet("127.0.0.1", 5000);
                siemensFetchWriteNet.ConnectionId = this.DtuId;
                return (NetworkDeviceBase)siemensFetchWriteNet;
            }
            if (this.DtuType == "SiemensPPIOverTcp")
            {
                SiemensPPIOverTcp siemensPpiOverTcp = new SiemensPPIOverTcp("127.0.0.1", 5000);
                siemensPpiOverTcp.ConnectionId = this.DtuId;
                return (NetworkDeviceBase)siemensPpiOverTcp;
            }
            if (this.DtuType == "OmronFinsNet")
            {
                OmronFinsNet omronFinsNet = new OmronFinsNet("127.0.0.1", 5000);
                omronFinsNet.ConnectionId = this.DtuId;
                return (NetworkDeviceBase)omronFinsNet;
            }
            if (this.DtuType == "OmronHostLinkOverTcp")
            {
                OmronHostLinkOverTcp omronHostLinkOverTcp = new OmronHostLinkOverTcp("127.0.0.1", 5000);
                omronHostLinkOverTcp.ConnectionId = this.DtuId;
                return (NetworkDeviceBase)omronHostLinkOverTcp;
            }
            if (!(this.DtuType == "AllenBradleyNet"))
                throw new NotImplementedException();
            AllenBradleyNet allenBradleyNet = new AllenBradleyNet("127.0.0.1", 5000);
            allenBradleyNet.ConnectionId = this.DtuId;
            return (NetworkDeviceBase)allenBradleyNet;
        }
    }
}
