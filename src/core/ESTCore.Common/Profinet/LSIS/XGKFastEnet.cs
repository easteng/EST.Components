// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.LSIS.XGKFastEnet
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;
using ESTCore.Common.Core;
using ESTCore.Common.Core.IMessage;
using ESTCore.Common.Core.Net;
using ESTCore.Common.Reflection;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.Common.Profinet.LSIS
{
    /// <summary>
    /// XGK Fast Enet I/F module supports open Ethernet. It provides network configuration that is to connect LSIS and other company PLC, PC on network
    /// </summary>
    /// <remarks>
    /// Address example likes the follow
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
    ///     <term>*</term>
    ///     <term>P</term>
    ///     <term>PX100,PB100,PW100,PD100,PL100</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>*</term>
    ///     <term>M</term>
    ///     <term>MX100,MB100,MW100,MD100,ML100</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>*</term>
    ///     <term>L</term>
    ///     <term>LX100,LB100,LW100,LD100,LL100</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>*</term>
    ///     <term>K</term>
    ///     <term>KX100,KB100,KW100,KD100,KL100</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term>*</term>
    ///     <term>F</term>
    ///     <term>FX100,FB100,FW100,FD100,FL100</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term></term>
    ///     <term>T</term>
    ///     <term>TX100,TB100,TW100,TD100,TL100</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term></term>
    ///     <term>C</term>
    ///     <term>CX100,CB100,CW100,CD100,CL100</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term></term>
    ///     <term>D</term>
    ///     <term>DX100,DB100,DW100,DD100,DL100</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term></term>
    ///     <term>S</term>
    ///     <term>SX100,SB100,SW100,SD100,SL100</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term></term>
    ///     <term>Q</term>
    ///     <term>QX100,QB100,QW100,QD100,QL100</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term></term>
    ///     <term>I</term>
    ///     <term>IX100,IB100,IW100,ID100,IL100</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term></term>
    ///     <term>N</term>
    ///     <term>NX100,NB100,NW100,ND100,NL100</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term></term>
    ///     <term>U</term>
    ///     <term>UX100,UB100,UW100,UD100,UL100</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term></term>
    ///     <term>Z</term>
    ///     <term>ZX100,ZB100,ZW100,ZD100,ZL100</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    ///   <item>
    ///     <term></term>
    ///     <term>R</term>
    ///     <term>RX100,RB100,RW100,RD100,RL100</term>
    ///     <term>10</term>
    ///     <term>√</term>
    ///     <term>√</term>
    ///     <term></term>
    ///   </item>
    /// </list>
    /// </remarks>
    public class XGKFastEnet : NetworkDeviceBase
    {
        private string CompanyID1 = "LSIS-XGT";
        private LSCpuInfo cpuInfo = LSCpuInfo.XGK;
        private byte baseNo = 0;
        private byte slotNo = 3;

        /// <summary>Instantiate a Default object</summary>
        public XGKFastEnet()
        {
            this.WordLength = (ushort)2;
            this.IpAddress = "127.0.0.1";
            this.Port = 2004;
            this.ByteTransform = (IByteTransform)new RegularByteTransform();
        }

        /// <summary>Instantiate a object by ipaddress and port</summary>
        /// <param name="ipAddress">the ip address of the plc</param>
        /// <param name="port">the port of the plc, default is 2004</param>
        public XGKFastEnet(string ipAddress, int port)
        {
            this.WordLength = (ushort)2;
            this.IpAddress = ipAddress;
            this.Port = port;
            this.ByteTransform = (IByteTransform)new RegularByteTransform();
        }

        /// <summary>
        /// Instantiate a object by ipaddress, port, cpuType, slotNo
        /// </summary>
        /// <param name="CpuType">CpuType</param>
        /// <param name="ipAddress">the ip address of the plc</param>
        /// <param name="port">he port of the plc, default is 2004</param>
        /// <param name="slotNo">slot number</param>
        public XGKFastEnet(string CpuType, string ipAddress, int port, byte slotNo)
        {
            this.SetCpuType = CpuType;
            this.WordLength = (ushort)2;
            this.IpAddress = ipAddress;
            this.Port = port;
            this.slotNo = slotNo;
            this.ByteTransform = (IByteTransform)new RegularByteTransform();
        }

        /// <inheritdoc />
        protected override INetMessage GetNewNetMessage() => (INetMessage)new LsisFastEnetMessage();

        /// <summary>set plc</summary>
        public string SetCpuType { get; set; }

        /// <summary>CPU TYPE</summary>
        public string CpuType { get; private set; }

        /// <summary>Cpu is error</summary>
        public bool CpuError { get; private set; }

        /// <summary>RUN, STOP, ERROR, DEBUG</summary>
        public LSCpuStatus LSCpuStatus { get; private set; }

        /// <summary>FEnet I/F module’s Base No.</summary>
        public byte BaseNo
        {
            get => this.baseNo;
            set => this.baseNo = value;
        }

        /// <summary>FEnet I/F module’s Slot No.</summary>
        public byte SlotNo
        {
            get => this.slotNo;
            set => this.slotNo = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public LSCpuInfo CpuInfo
        {
            get => this.cpuInfo;
            set => this.cpuInfo = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public string CompanyID
        {
            get => this.CompanyID1;
            set => this.CompanyID1 = value;
        }

        /// <inheritdoc />
        public override async Task<OperateResult<bool[]>> ReadBoolAsync(
          string address,
          ushort length)
        {
            OperateResult<byte[]> coreResult = (OperateResult<byte[]>)null;
            List<XGTAddressData> lstAdress = new List<XGTAddressData>();
            string[] ArrayAdress = address.Split(new string[2]
            {
        ";",
        ","
            }, StringSplitOptions.RemoveEmptyEntries);
            string[] strArray = ArrayAdress;
            for (int index = 0; index < strArray.Length; ++index)
            {
                string item = strArray[index];
                XGTAddressData addrData = new XGTAddressData();
                OperateResult<XGT_DataType, bool> DataTypeResult1 = XGKFastEnet.GetDataTypeToAddress(item);
                addrData.Address = !DataTypeResult1.Content2 ? item.Substring(2) : item.Substring(1);
                lstAdress.Add(addrData);
                addrData = (XGTAddressData)null;
                DataTypeResult1 = (OperateResult<XGT_DataType, bool>)null;
                item = (string)null;
            }
            strArray = (string[])null;
            OperateResult<XGT_MemoryType> analysisResult = XGKFastEnet.AnalysisAddress(ArrayAdress[0]);
            if (!analysisResult.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)analysisResult);
            OperateResult<XGT_DataType, bool> DataTypeResult = XGKFastEnet.GetDataTypeToAddress(ArrayAdress[0]);
            if (!DataTypeResult.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)DataTypeResult);
            coreResult = DataTypeResult.Content1 != XGT_DataType.Continue ? this.Read(DataTypeResult.Content1, lstAdress, analysisResult.Content, 1) : this.Read(DataTypeResult.Content1, lstAdress, analysisResult.Content, 1, (int)length);
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(coreResult.Content);
            if (!read.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)read);
            if (lstAdress.Count > 1)
            {
                OperateResult<bool[]> extract = this.ExtractActualDataBool(read.Content);
                return extract.IsSuccess ? OperateResult.CreateSuccessResult<bool[]>(extract.Content) : OperateResult.CreateFailedResult<bool[]>((OperateResult)extract);
            }
            OperateResult<byte[]> extract1 = this.ExtractActualData(read.Content);
            return extract1.IsSuccess ? OperateResult.CreateSuccessResult<bool[]>(SoftBasic.ByteToBoolArray(extract1.Content)) : OperateResult.CreateFailedResult<bool[]>((OperateResult)extract1);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.LSIS.XGKFastEnet.ReadCoil(System.String)" />
        public async Task<OperateResult<bool>> ReadCoilAsync(string address)
        {
            OperateResult<bool> operateResult = await this.ReadBoolAsync(address);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.LSIS.XGKFastEnet.ReadCoil(System.String,System.UInt16)" />
        public async Task<OperateResult<bool[]>> ReadCoilAsync(
          string address,
          ushort length)
        {
            OperateResult<bool[]> operateResult = await this.ReadBoolAsync(address, length);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.LSIS.XGKFastEnet.ReadByte(System.String)" />
        public async Task<OperateResult<byte>> ReadByteAsync(string address)
        {
            OperateResult<byte[]> result = await this.ReadAsync(address, (ushort)1);
            return ByteTransformHelper.GetResultFromArray<byte>(result);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.LSIS.XGKFastEnet.Write(System.String,System.Byte)" />
        public async Task<OperateResult> WriteAsync(string address, byte value)
        {
            OperateResult operateResult = await this.WriteAsync(address, new byte[1]
            {
        value
            });
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.LSIS.XGKFastEnet.WriteCoil(System.String,System.Boolean)" />
        public async Task<OperateResult> WriteCoilAsync(string address, bool value)
        {
            OperateResult operateResult = await this.WriteAsync(address, new byte[2]
            {
        value ? (byte) 1 : (byte) 0,
        (byte) 0
            });
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.LSIS.XGKFastEnet.Write(System.String,System.Boolean)" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          bool value)
        {
            OperateResult operateResult = await this.WriteCoilAsync(address, value);
            return operateResult;
        }

        /// <inheritdoc />
        [EstMqttApi("ReadByteArray", "")]
        public override OperateResult<byte[]> Read(string address, ushort length)
        {
            List<XGTAddressData> pAddress = new List<XGTAddressData>();
            string[] strArray = address.Split(new string[2]
            {
        ";",
        ","
            }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string address1 in strArray)
                pAddress.Add(new XGTAddressData()
                {
                    Address = !XGKFastEnet.GetDataTypeToAddress(address1).Content2 ? address1.Substring(2) : address1.Substring(1)
                });
            OperateResult<XGT_MemoryType> operateResult1 = XGKFastEnet.AnalysisAddress(strArray[0]);
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult1);
            OperateResult<XGT_DataType, bool> dataTypeToAddress = XGKFastEnet.GetDataTypeToAddress(strArray[0]);
            if (!dataTypeToAddress.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)dataTypeToAddress);
            OperateResult<byte[]> operateResult2 = dataTypeToAddress.Content1 != XGT_DataType.Continue ? this.Read(dataTypeToAddress.Content1, pAddress, operateResult1.Content, 1) : this.Read(dataTypeToAddress.Content1, pAddress, operateResult1.Content, 1, (int)length);
            if (!operateResult2.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult2);
            OperateResult<byte[]> operateResult3 = this.ReadFromCoreServer(operateResult2.Content);
            if (!operateResult3.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult3);
            return pAddress.Count > 1 ? this.ExtractActualDatabyte(operateResult3.Content) : this.ExtractActualData(operateResult3.Content);
        }

        /// <inheritdoc />
        [EstMqttApi("WriteByteArray", "")]
        public override OperateResult Write(string address, byte[] value)
        {
            List<XGTAddressData> pAddressList = new List<XGTAddressData>();
            string str = address;
            string[] separator = new string[2] { ";", "," };
            foreach (string address1 in str.Split(separator, StringSplitOptions.RemoveEmptyEntries))
                pAddressList.Add(new XGTAddressData()
                {
                    Address = !XGKFastEnet.GetDataTypeToAddress(address1).Content2 ? address1.Substring(2) : address1.Substring(1),
                    DataByteArray = value
                });
            OperateResult<XGT_MemoryType> operateResult1 = XGKFastEnet.AnalysisAddress(address);
            if (!operateResult1.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult1);
            OperateResult<XGT_DataType, bool> dataTypeToAddress = XGKFastEnet.GetDataTypeToAddress(address);
            if (!dataTypeToAddress.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<byte[]>((OperateResult)dataTypeToAddress);
            OperateResult<byte[]> operateResult2 = dataTypeToAddress.Content1 != XGT_DataType.Continue ? this.Write(dataTypeToAddress.Content1, pAddressList, operateResult1.Content, 1) : this.Write(dataTypeToAddress.Content1, pAddressList, operateResult1.Content, 1, value.Length);
            if (!operateResult2.IsSuccess)
                return (OperateResult)operateResult2;
            OperateResult<byte[]> operateResult3 = this.ReadFromCoreServer(operateResult2.Content);
            return !operateResult3.IsSuccess ? (OperateResult)OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult3) : (OperateResult)this.ExtractActualData(operateResult3.Content);
        }

        /// <inheritdoc />
        [EstMqttApi("ReadBoolArray", "")]
        public override OperateResult<bool[]> ReadBool(string address, ushort length)
        {
            List<XGTAddressData> pAddress = new List<XGTAddressData>();
            string[] strArray = address.Split(new string[2]
            {
        ";",
        ","
            }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string address1 in strArray)
                pAddress.Add(new XGTAddressData()
                {
                    Address = !XGKFastEnet.GetDataTypeToAddress(address1).Content2 ? address1.Substring(2) : address1.Substring(1)
                });
            OperateResult<XGT_MemoryType> operateResult1 = XGKFastEnet.AnalysisAddress(strArray[0]);
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult1);
            OperateResult<XGT_DataType, bool> dataTypeToAddress = XGKFastEnet.GetDataTypeToAddress(strArray[0]);
            if (!dataTypeToAddress.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)dataTypeToAddress);
            OperateResult<byte[]> operateResult2 = dataTypeToAddress.Content1 != XGT_DataType.Continue ? this.Read(dataTypeToAddress.Content1, pAddress, operateResult1.Content, 1) : this.Read(dataTypeToAddress.Content1, pAddress, operateResult1.Content, 1, (int)length);
            if (!operateResult2.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult2);
            OperateResult<byte[]> operateResult3 = this.ReadFromCoreServer(operateResult2.Content);
            if (!operateResult3.IsSuccess)
                return OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult3);
            if (pAddress.Count > 1)
            {
                OperateResult<bool[]> actualDataBool = this.ExtractActualDataBool(operateResult3.Content);
                return !actualDataBool.IsSuccess ? OperateResult.CreateFailedResult<bool[]>((OperateResult)actualDataBool) : OperateResult.CreateSuccessResult<bool[]>(actualDataBool.Content);
            }
            OperateResult<byte[]> actualData = this.ExtractActualData(operateResult3.Content);
            return !actualData.IsSuccess ? OperateResult.CreateFailedResult<bool[]>((OperateResult)actualData) : OperateResult.CreateSuccessResult<bool[]>(SoftBasic.ByteToBoolArray(actualData.Content));
        }

        /// <summary>ReadCoil</summary>
        /// <param name="address">Start address</param>
        /// <returns>Whether to read the successful</returns>
        public OperateResult<bool> ReadCoil(string address) => this.ReadBool(address);

        /// <summary>ReadCoil</summary>
        /// <param name="address">Start address</param>
        /// <param name="length">read address length</param>
        /// <returns>Whether to read the successful</returns>
        public OperateResult<bool[]> ReadCoil(string address, ushort length) => this.ReadBool(address, length);

        /// <summary>Read single byte value from plc</summary>
        /// <param name="address">Start address</param>
        /// <returns>Whether to write the successful</returns>
        [EstMqttApi("ReadByte", "")]
        public OperateResult<byte> ReadByte(string address) => ByteTransformHelper.GetResultFromArray<byte>(this.Read(address, (ushort)1));

        /// <summary>Write single byte value to plc</summary>
        /// <param name="address">Start address</param>
        /// <param name="value">value</param>
        /// <returns>Whether to write the successful</returns>
        [EstMqttApi("WriteByte", "")]
        public OperateResult Write(string address, byte value) => this.Write(address, new byte[1]
        {
      value
        });

        /// <summary>WriteCoil</summary>
        /// <param name="address">Start address</param>
        /// <param name="value">bool value</param>
        /// <returns>Whether to write the successful</returns>
        public OperateResult WriteCoil(string address, bool value) => this.Write(address, new byte[2]
        {
      value ? (byte) 1 : (byte) 0,
      (byte) 0
        });

        /// <summary>WriteCoil</summary>
        /// <param name="address">Start address</param>
        /// <param name="value">bool value</param>
        /// <returns>Whether to write the successful</returns>
        [EstMqttApi("WriteBool", "")]
        public override OperateResult Write(string address, bool value) => this.WriteCoil(address, value);

        /// <summary>Read</summary>
        /// <param name="pDataType"></param>
        /// <param name="pAddress"></param>
        /// <param name="pMemtype"></param>
        /// <param name="pInvokeID"></param>
        /// <param name="pDataCount"></param>
        /// <returns></returns>
        public OperateResult<byte[]> Read(
          XGT_DataType pDataType,
          List<XGTAddressData> pAddress,
          XGT_MemoryType pMemtype,
          int pInvokeID,
          int pDataCount = 0)
        {
            if (pAddress.Count > 16)
                return new OperateResult<byte[]>("You cannot read more than 16 pieces.");
            try
            {
                byte[] readDataFormat = this.CreateReadDataFormat(XGT_Request_Func.Read, pDataType, pAddress, pMemtype, pDataCount);
                byte[] header1 = this.CreateHeader(pInvokeID, readDataFormat.Length);
                byte[] header2 = new byte[header1.Length + readDataFormat.Length];
                int idx = 0;
                this.AddByte(header1, ref idx, ref header2);
                this.AddByte(readDataFormat, ref idx, ref header2);
                return OperateResult.CreateSuccessResult<byte[]>(header2);
            }
            catch (Exception ex)
            {
                return new OperateResult<byte[]>("ERROR:" + ex.Message.ToString());
            }
        }

        /// <summary>Write</summary>
        /// <param name="pDataType"></param>
        /// <param name="pAddressList"></param>
        /// <param name="pMemtype"></param>
        /// <param name="pInvokeID"></param>
        /// <param name="pDataCount"></param>
        /// <returns></returns>
        public OperateResult<byte[]> Write(
          XGT_DataType pDataType,
          List<XGTAddressData> pAddressList,
          XGT_MemoryType pMemtype,
          int pInvokeID,
          int pDataCount = 0)
        {
            try
            {
                byte[] writeDataFormat = this.CreateWriteDataFormat(XGT_Request_Func.Write, pDataType, pAddressList, pMemtype, pDataCount);
                byte[] header1 = this.CreateHeader(pInvokeID, writeDataFormat.Length);
                byte[] header2 = new byte[header1.Length + writeDataFormat.Length];
                int idx = 0;
                this.AddByte(header1, ref idx, ref header2);
                this.AddByte(writeDataFormat, ref idx, ref header2);
                return OperateResult.CreateSuccessResult<byte[]>(header2);
            }
            catch (Exception ex)
            {
                return new OperateResult<byte[]>("ERROR:" + ex.Message.ToString());
            }
        }

        /// <summary>CreateHeader</summary>
        /// <param name="pInvokeID"></param>
        /// <param name="pDataByteLenth"></param>
        /// <returns></returns>
        public byte[] CreateHeader(int pInvokeID, int pDataByteLenth)
        {
            byte[] bytes1 = Encoding.ASCII.GetBytes(this.CompanyID);
            byte[] bytes2 = BitConverter.GetBytes((short)0);
            byte[] bytes3 = BitConverter.GetBytes((short)0);
            byte[] numArray1 = new byte[1];
            switch (this.cpuInfo)
            {
                case LSCpuInfo.XGK:
                    numArray1[0] = (byte)160;
                    break;
                case LSCpuInfo.XGI:
                    numArray1[0] = (byte)164;
                    break;
                case LSCpuInfo.XGR:
                    numArray1[0] = (byte)168;
                    break;
                case LSCpuInfo.XGB_MK:
                    numArray1[0] = (byte)176;
                    break;
                case LSCpuInfo.XGB_IEC:
                    numArray1[0] = (byte)180;
                    break;
            }
            byte[] numArray2 = new byte[1] { (byte)51 };
            byte[] bytes4 = BitConverter.GetBytes((short)pInvokeID);
            byte[] bytes5 = BitConverter.GetBytes((short)pDataByteLenth);
            byte[] numArray3 = new byte[1]
            {
        (byte) ((uint) this.baseNo * 16U + (uint) this.slotNo)
            };
            byte[] numArray4 = new byte[1] { (byte)0 };
            byte[] header = new byte[bytes1.Length + bytes2.Length + bytes3.Length + numArray1.Length + numArray2.Length + bytes4.Length + bytes5.Length + numArray3.Length + numArray4.Length];
            int idx = 0;
            this.AddByte(bytes1, ref idx, ref header);
            this.AddByte(bytes2, ref idx, ref header);
            this.AddByte(bytes3, ref idx, ref header);
            this.AddByte(numArray1, ref idx, ref header);
            this.AddByte(numArray2, ref idx, ref header);
            this.AddByte(bytes4, ref idx, ref header);
            this.AddByte(bytes5, ref idx, ref header);
            this.AddByte(numArray3, ref idx, ref header);
            this.AddByte(numArray4, ref idx, ref header);
            return header;
        }

        private byte[] CreateReadDataFormat(
          XGT_Request_Func emFunc,
          XGT_DataType emDatatype,
          List<XGTAddressData> pAddressList,
          XGT_MemoryType emMemtype,
          int pDataCount)
        {
            List<XGTAddressData> xgtAddressDataList = new List<XGTAddressData>();
            byte[] bytes1 = BitConverter.GetBytes((short)emFunc);
            byte[] bytes2 = BitConverter.GetBytes((short)emDatatype);
            byte[] bytes3 = BitConverter.GetBytes((short)0);
            byte[] bytes4 = BitConverter.GetBytes((short)pAddressList.Count);
            int length = bytes1.Length + bytes2.Length + bytes3.Length + bytes4.Length;
            foreach (XGTAddressData pAddress in pAddressList)
            {
                string valueName = this.CreateValueName(emDatatype, emMemtype, pAddress.Address);
                XGTAddressData xgtAddressData = new XGTAddressData();
                xgtAddressData.AddressString = valueName;
                xgtAddressDataList.Add(xgtAddressData);
                length += xgtAddressData.AddressByteArray.Length + xgtAddressData.LengthByteArray.Length;
            }
            if (XGT_DataType.Continue == emDatatype && XGT_Request_Func.Read == emFunc)
                length += 2;
            byte[] header = new byte[length];
            int idx = 0;
            this.AddByte(bytes1, ref idx, ref header);
            this.AddByte(bytes2, ref idx, ref header);
            this.AddByte(bytes3, ref idx, ref header);
            this.AddByte(bytes4, ref idx, ref header);
            foreach (XGTAddressData xgtAddressData in xgtAddressDataList)
            {
                this.AddByte(xgtAddressData.LengthByteArray, ref idx, ref header);
                this.AddByte(xgtAddressData.AddressByteArray, ref idx, ref header);
            }
            if (XGT_DataType.Continue == emDatatype)
                this.AddByte(BitConverter.GetBytes((short)pDataCount), ref idx, ref header);
            return header;
        }

        private byte[] CreateWriteDataFormat(
          XGT_Request_Func emFunc,
          XGT_DataType emDatatype,
          List<XGTAddressData> pAddressList,
          XGT_MemoryType emMemtype,
          int pDataCount)
        {
            byte[] bytes1 = BitConverter.GetBytes((short)emFunc);
            byte[] bytes2 = BitConverter.GetBytes((short)emDatatype);
            byte[] bytes3 = BitConverter.GetBytes((short)0);
            byte[] bytes4 = BitConverter.GetBytes((short)pAddressList.Count);
            int length1 = bytes1.Length + bytes2.Length + bytes3.Length + bytes4.Length;
            List<XGTAddressData> xgtAddressDataList = new List<XGTAddressData>();
            foreach (XGTAddressData pAddress in pAddressList)
            {
                string valueName = this.CreateValueName(emDatatype, emMemtype, pAddress.Address);
                pAddress.AddressString = valueName;
                int length2 = pAddress.DataByteArray.Length;
                length1 += pAddress.AddressByteArray.Length + pAddress.LengthByteArray.Length + 2 + length2;
                xgtAddressDataList.Add(pAddress);
            }
            if (XGT_DataType.Continue == emDatatype && XGT_Request_Func.Read == emFunc)
                length1 += 2;
            byte[] header = new byte[length1];
            int idx = 0;
            this.AddByte(bytes1, ref idx, ref header);
            this.AddByte(bytes2, ref idx, ref header);
            this.AddByte(bytes3, ref idx, ref header);
            this.AddByte(bytes4, ref idx, ref header);
            foreach (XGTAddressData xgtAddressData in xgtAddressDataList)
            {
                this.AddByte(xgtAddressData.LengthByteArray, ref idx, ref header);
                this.AddByte(xgtAddressData.AddressByteArray, ref idx, ref header);
            }
            foreach (XGTAddressData xgtAddressData in xgtAddressDataList)
            {
                this.AddByte(BitConverter.GetBytes((short)xgtAddressData.DataByteArray.Length), ref idx, ref header);
                this.AddByte(xgtAddressData.DataByteArray, ref idx, ref header);
            }
            return header;
        }

        /// <summary>Create a memory address variable name.</summary>
        /// <param name="dataType">데이터타입</param>
        /// <param name="memType">메모리타입</param>
        /// <param name="pAddress">주소번지</param>
        /// <returns></returns>
        public string CreateValueName(XGT_DataType dataType, XGT_MemoryType memType, string pAddress)
        {
            string empty = string.Empty;
            string memTypeChar = this.GetMemTypeChar(memType);
            string typeChar = this.GetTypeChar(dataType);
            int num;
            if (dataType == XGT_DataType.Continue)
            {
                num = Convert.ToInt32(pAddress) * 2;
                pAddress = num.ToString();
            }
            if (dataType == XGT_DataType.Bit)
            {
                string str = pAddress.Substring(0, pAddress.Length - 1);
                int int32 = Convert.ToInt32(pAddress.Substring(pAddress.Length - 1), 16);
                num = Convert.ToInt32(str) * 16 + int32;
                pAddress = num.ToString();
            }
            return "%" + memTypeChar + typeChar + pAddress;
        }

        /// <summary>Char return according to data type</summary>
        /// <param name="type">데이터타입</param>
        /// <returns></returns>
        private string GetTypeChar(XGT_DataType type)
        {
            string empty = string.Empty;
            string str;
            switch (type)
            {
                case XGT_DataType.Bit:
                    str = "X";
                    break;
                case XGT_DataType.Byte:
                    str = "B";
                    break;
                case XGT_DataType.Word:
                    str = "W";
                    break;
                case XGT_DataType.DWord:
                    str = "D";
                    break;
                case XGT_DataType.LWord:
                    str = "L";
                    break;
                case XGT_DataType.Continue:
                    str = "B";
                    break;
                default:
                    str = "X";
                    break;
            }
            return str;
        }

        /// <summary>Char return according to memory type</summary>
        /// <param name="type">메모리타입</param>
        /// <returns></returns>
        private string GetMemTypeChar(XGT_MemoryType type)
        {
            string str = string.Empty;
            switch (type)
            {
                case XGT_MemoryType.IO:
                    str = "P";
                    break;
                case XGT_MemoryType.SubRelay:
                    str = "M";
                    break;
                case XGT_MemoryType.LinkRelay:
                    str = "L";
                    break;
                case XGT_MemoryType.KeepRelay:
                    str = "K";
                    break;
                case XGT_MemoryType.EtcRelay:
                    str = "F";
                    break;
                case XGT_MemoryType.Timer:
                    str = "T";
                    break;
                case XGT_MemoryType.Counter:
                    str = "C";
                    break;
                case XGT_MemoryType.DataRegister:
                    str = "D";
                    break;
                case XGT_MemoryType.ComDataRegister:
                    str = "N";
                    break;
                case XGT_MemoryType.FileDataRegister:
                    str = "R";
                    break;
                case XGT_MemoryType.StepRelay:
                    str = "S";
                    break;
                case XGT_MemoryType.SpecialRegister:
                    str = "U";
                    break;
            }
            return str;
        }

        /// <summary>바이트 합치기</summary>
        /// <param name="item">개별바이트</param>
        /// <param name="idx">전체바이트에 개별바이트를 합칠 인덱스</param>
        /// <param name="header">전체바이트</param>
        /// <returns>전체 바이트 </returns>
        private byte[] AddByte(byte[] item, ref int idx, ref byte[] header)
        {
            Array.Copy((Array)item, 0, (Array)header, idx, item.Length);
            idx += item.Length;
            return header;
        }

        /// <summary>AnalysisAddress XGT_MemoryType</summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static OperateResult<XGT_MemoryType> AnalysisAddress(
          string address)
        {
            XGT_MemoryType xgtMemoryType = XGT_MemoryType.IO;
            try
            {
                char[] chArray = new char[15]
                {
          'P',
          'M',
          'L',
          'K',
          'F',
          'T',
          'C',
          'D',
          'S',
          'Q',
          'I',
          'N',
          'U',
          'Z',
          'R'
                };
                bool flag = false;
                for (int index = 0; index < chArray.Length; ++index)
                {
                    if ((int)chArray[index] == (int)address[0])
                    {
                        switch (address[0])
                        {
                            case 'C':
                                xgtMemoryType = XGT_MemoryType.Counter;
                                break;
                            case 'D':
                                xgtMemoryType = XGT_MemoryType.DataRegister;
                                break;
                            case 'F':
                                xgtMemoryType = XGT_MemoryType.EtcRelay;
                                break;
                            case 'K':
                                xgtMemoryType = XGT_MemoryType.KeepRelay;
                                break;
                            case 'L':
                                xgtMemoryType = XGT_MemoryType.LinkRelay;
                                break;
                            case 'M':
                                xgtMemoryType = XGT_MemoryType.SubRelay;
                                break;
                            case 'N':
                                xgtMemoryType = XGT_MemoryType.ComDataRegister;
                                break;
                            case 'P':
                                xgtMemoryType = XGT_MemoryType.IO;
                                break;
                            case 'R':
                                xgtMemoryType = XGT_MemoryType.FileDataRegister;
                                break;
                            case 'S':
                                xgtMemoryType = XGT_MemoryType.StepRelay;
                                break;
                            case 'T':
                                xgtMemoryType = XGT_MemoryType.Timer;
                                break;
                            case 'U':
                                xgtMemoryType = XGT_MemoryType.SpecialRegister;
                                break;
                        }
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                    throw new Exception(StringResources.Language.NotSupportedDataType);
            }
            catch (Exception ex)
            {
                return new OperateResult<XGT_MemoryType>(ex.Message);
            }
            return OperateResult.CreateSuccessResult<XGT_MemoryType>(xgtMemoryType);
        }

        /// <summary>GetDataTypeToAddress</summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static OperateResult<XGT_DataType, bool> GetDataTypeToAddress(
          string address)
        {
            XGT_DataType xgtDataType = XGT_DataType.Bit;
            bool flag1 = false;
            try
            {
                char[] chArray = new char[15]
                {
          'P',
          'M',
          'L',
          'K',
          'F',
          'T',
          'C',
          'D',
          'S',
          'Q',
          'I',
          'N',
          'U',
          'Z',
          'R'
                };
                bool flag2 = false;
                for (int index = 0; index < chArray.Length; ++index)
                {
                    if ((int)chArray[index] == (int)address[0])
                    {
                        switch (address[1])
                        {
                            case 'B':
                                xgtDataType = XGT_DataType.Byte;
                                break;
                            case 'C':
                                xgtDataType = XGT_DataType.Continue;
                                break;
                            case 'D':
                                xgtDataType = XGT_DataType.DWord;
                                break;
                            case 'L':
                                xgtDataType = XGT_DataType.LWord;
                                break;
                            case 'W':
                                xgtDataType = XGT_DataType.Word;
                                break;
                            case 'X':
                                xgtDataType = XGT_DataType.Bit;
                                break;
                            default:
                                flag1 = true;
                                xgtDataType = XGT_DataType.Continue;
                                break;
                        }
                        flag2 = true;
                        break;
                    }
                }
                if (!flag2)
                    throw new Exception(StringResources.Language.NotSupportedDataType);
            }
            catch (Exception ex)
            {
                return new OperateResult<XGT_DataType, bool>(ex.Message);
            }
            return OperateResult.CreateSuccessResult<XGT_DataType, bool>(xgtDataType, flag1);
        }

        /// <summary>
        /// Returns true data content, supports read and write returns
        /// </summary>
        /// <param name="response">response data</param>
        /// <returns>real data</returns>
        public OperateResult<byte[]> ExtractActualData(byte[] response)
        {
            OperateResult<bool> cpuTypeToPlc = this.GetCpuTypeToPLC(response);
            if (!cpuTypeToPlc.IsSuccess)
                return new OperateResult<byte[]>(cpuTypeToPlc.Message);
            if (response[20] == (byte)89)
                return OperateResult.CreateSuccessResult<byte[]>(new byte[0]);
            if (response[20] != (byte)85)
                return new OperateResult<byte[]>(StringResources.Language.NotSupportedFunction);
            try
            {
                ushort uint16 = BitConverter.ToUInt16(response, 30);
                byte[] numArray = new byte[(int)uint16];
                Array.Copy((Array)response, 32, (Array)numArray, 0, (int)uint16);
                return OperateResult.CreateSuccessResult<byte[]>(numArray);
            }
            catch (Exception ex)
            {
                return new OperateResult<byte[]>(ex.Message);
            }
        }

        /// <summary>SetCpuType</summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public OperateResult<bool> GetCpuTypeToPLC(byte[] response)
        {
            try
            {
                if (response.Length < 20)
                    return new OperateResult<bool>("Length is less than 20:" + SoftBasic.ByteToHexString(response));
                ushort uint16 = BitConverter.ToUInt16(response, 10);
                BitArray bitArray = new BitArray(BitConverter.GetBytes(uint16));
                int num = (int)uint16 % 32;
                switch ((int)uint16 % 32)
                {
                    case 1:
                        this.CpuType = "XGK/R-CPUH";
                        break;
                    case 2:
                        this.CpuType = "XGK-CPUS";
                        break;
                    case 4:
                        this.CpuType = "XGK-CPUE";
                        break;
                    case 5:
                        this.CpuType = "XGK/R-CPUH";
                        break;
                    case 6:
                        this.CpuType = "XGB/XBCU";
                        break;
                }
                this.CpuError = bitArray[7];
                if (bitArray[8])
                    this.LSCpuStatus = LSCpuStatus.RUN;
                if (bitArray[9])
                    this.LSCpuStatus = LSCpuStatus.STOP;
                if (bitArray[10])
                    this.LSCpuStatus = LSCpuStatus.ERROR;
                if (bitArray[11])
                    this.LSCpuStatus = LSCpuStatus.DEBUG;
                if (response.Length < 28)
                    return new OperateResult<bool>("Length is less than 28:" + SoftBasic.ByteToHexString(response));
                if (BitConverter.ToUInt16(response, 26) > (ushort)0)
                    return new OperateResult<bool>((int)response[28], "Error:" + XGKFastEnet.GetErrorDesciption(response[28]));
            }
            catch (Exception ex)
            {
                return new OperateResult<bool>(ex.Message);
            }
            return OperateResult.CreateSuccessResult<bool>(true);
        }

        /// <summary>
        /// Returns true data content, supports read and write returns
        /// </summary>
        /// <param name="response">response data</param>
        /// <returns>real data</returns>
        public OperateResult<bool[]> ExtractActualDataBool(byte[] response)
        {
            OperateResult<bool> cpuTypeToPlc = this.GetCpuTypeToPLC(response);
            if (!cpuTypeToPlc.IsSuccess)
                return new OperateResult<bool[]>(cpuTypeToPlc.Message);
            if (response[20] == (byte)89)
                return OperateResult.CreateSuccessResult<bool[]>(new bool[0]);
            if (response[20] != (byte)85)
                return new OperateResult<bool[]>(StringResources.Language.NotSupportedFunction);
            int sourceIndex1 = 28;
            byte[] numArray1 = new byte[2];
            byte[] numArray2 = new byte[2];
            byte[] numArray3 = new byte[2];
            Array.Copy((Array)response, sourceIndex1, (Array)numArray1, 0, 2);
            int int16_1 = (int)BitConverter.ToInt16(numArray1, 0);
            List<bool> boolList = new List<bool>();
            int sourceIndex2 = sourceIndex1 + 2;
            try
            {
                for (int index = 0; index < int16_1; ++index)
                {
                    Array.Copy((Array)response, sourceIndex2, (Array)numArray2, 0, 2);
                    int int16_2 = (int)BitConverter.ToInt16(numArray2, 0);
                    int sourceIndex3 = sourceIndex2 + 2;
                    byte[] numArray4 = new byte[int16_2];
                    Array.Copy((Array)response, sourceIndex3, (Array)numArray4, 0, int16_2);
                    sourceIndex2 = sourceIndex3 + int16_2;
                    boolList.Add(BitConverter.ToBoolean(numArray4, 0));
                }
                return OperateResult.CreateSuccessResult<bool[]>(boolList.ToArray());
            }
            catch (Exception ex)
            {
                return new OperateResult<bool[]>(ex.Message);
            }
        }

        /// <summary>
        /// Returns true data content, supports read and write returns
        /// </summary>
        /// <param name="response">response data</param>
        /// <returns>real data</returns>
        public OperateResult<byte[]> ExtractActualDatabyte(byte[] response)
        {
            OperateResult<bool> cpuTypeToPlc = this.GetCpuTypeToPLC(response);
            if (!cpuTypeToPlc.IsSuccess)
                return new OperateResult<byte[]>(cpuTypeToPlc.Message);
            if (response[20] == (byte)89)
                return OperateResult.CreateSuccessResult<byte[]>(new byte[0]);
            if (response[20] != (byte)85)
                return new OperateResult<byte[]>(StringResources.Language.NotSupportedFunction);
            int sourceIndex1 = 28;
            byte[] numArray1 = new byte[2];
            byte[] numArray2 = new byte[2];
            byte[] numArray3 = new byte[2];
            Array.Copy((Array)response, sourceIndex1, (Array)numArray1, 0, 2);
            int int16_1 = (int)BitConverter.ToInt16(numArray1, 0);
            List<byte> byteList = new List<byte>();
            int sourceIndex2 = sourceIndex1 + 2;
            try
            {
                for (int index = 0; index < int16_1; ++index)
                {
                    Array.Copy((Array)response, sourceIndex2, (Array)numArray2, 0, 2);
                    int int16_2 = (int)BitConverter.ToInt16(numArray2, 0);
                    int sourceIndex3 = sourceIndex2 + 2;
                    Array.Copy((Array)response, sourceIndex3, (Array)numArray3, 0, int16_2);
                    sourceIndex2 = sourceIndex3 + int16_2;
                    byteList.AddRange((IEnumerable<byte>)numArray3);
                }
                return OperateResult.CreateSuccessResult<byte[]>(byteList.ToArray());
            }
            catch (Exception ex)
            {
                return new OperateResult<byte[]>(ex.Message);
            }
        }

        /// <summary>get the description of the error code meanning</summary>
        /// <param name="code">code value</param>
        /// <returns>string information</returns>
        public static string GetErrorDesciption(byte code)
        {
            switch (code)
            {
                case 0:
                    return "Normal";
                case 1:
                    return "Physical layer error (TX, RX unavailable)";
                case 3:
                    return "There is no identifier of Function Block to receive in communication channel";
                case 4:
                    return "Mismatch of data type";
                case 5:
                    return "Reset is received from partner station";
                case 6:
                    return "Communication instruction of partner station is not ready status";
                case 7:
                    return "Device status of remote station is not desirable status";
                case 8:
                    return "Access to some target is not available";
                case 9:
                    return "Can’ t deal with communication instruction of partner station by too many reception";
                case 10:
                    return "Time Out error";
                case 11:
                    return "Structure error";
                case 12:
                    return "Abort";
                case 13:
                    return "Reject(local/remote)";
                case 14:
                    return "Communication channel establishment error (Connect/Disconnect)";
                case 15:
                    return "High speed communication and connection service error";
                case 33:
                    return "Can’t find variable identifier";
                case 34:
                    return "Address error";
                case 50:
                    return "Response error";
                case 113:
                    return "Object Access Unsupported";
                case 187:
                    return "Unknown error code (communication code of other company) is received";
                default:
                    return "Unknown error";
            }
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("XGkFastEnet[{0}:{1}]", (object)this.IpAddress, (object)this.Port);
    }
}
