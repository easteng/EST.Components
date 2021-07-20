// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.LSIS.XGKCnet
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;
using ESTCore.Common.Core;
using ESTCore.Common.Reflection;
using ESTCore.Common.Serial;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.Common.Profinet.LSIS
{
    /// <summary>XGk Cnet I/F module supports Serial Port.</summary>
    /// <remarks>
    /// </remarks>
    public class XGKCnet : SerialDeviceBase
    {
        /// <summary>Instantiate a Default object</summary>
        public XGKCnet()
        {
            this.ByteTransform = (IByteTransform)new RegularByteTransform();
            this.WordLength = (ushort)2;
        }

        /// <inheritdoc cref="P:ESTCore.Common.Profinet.LSIS.XGBCnetOverTcp.Station" />
        public byte Station { get; set; } = 5;

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.LSIS.XGBCnetOverTcp.ReadByte(System.String)" />
        [EstMqttApi("ReadByte", "")]
        public OperateResult<byte> ReadByte(string address) => ByteTransformHelper.GetResultFromArray<byte>(this.Read(address, (ushort)2));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.LSIS.XGBCnetOverTcp.Write(System.String,System.Byte)" />
        [EstMqttApi("WriteByte", "")]
        public OperateResult Write(string address, byte value) => this.Write(address, new byte[1]
        {
      value
        });

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
            OperateResult<byte[]> operateResult2 = this.ReadBase(this.Read(dataTypeToAddress.Content1, pAddress, operateResult1.Content, 1).Content);
            return !operateResult2.IsSuccess ? OperateResult.CreateFailedResult<bool[]>((OperateResult)operateResult2) : OperateResult.CreateSuccessResult<bool[]>(SoftBasic.ByteToBoolArray(XGKCnet.ExtractActualData(operateResult2.Content, true).Content, (int)length));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.LSIS.XGBCnetOverTcp.ReadCoil(System.String)" />
        public OperateResult<bool> ReadCoil(string address) => this.ReadBool(address);

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.LSIS.XGBCnetOverTcp.ReadCoil(System.String,System.UInt16)" />
        public OperateResult<bool[]> ReadCoil(string address, ushort length) => this.ReadBool(address, length);

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.LSIS.XGKCnet.WriteCoil(System.String,System.Boolean)" />
        public OperateResult WriteCoil(string address, bool value) => this.Write(address, value);

        /// <inheritdoc />
        [EstMqttApi("WriteBool", "")]
        public override OperateResult Write(string address, bool value) => this.Write(address, new byte[1]
        {
      value ? (byte) 1 : (byte) 0
        });

        /// <inheritdoc />
        public override async Task<OperateResult> WriteAsync(
          string address,
          bool value)
        {
            OperateResult operateResult = await this.WriteAsync(address, new byte[1]
            {
        value ? (byte) 1 : (byte) 0
            });
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
            OperateResult<byte[]> operateResult2 = this.Read(dataTypeToAddress.Content1, pAddress, operateResult1.Content, (int)length);
            if (!operateResult2.IsSuccess)
                return operateResult2;
            OperateResult<byte[]> operateResult3 = this.ReadBase(operateResult2.Content);
            return !operateResult3.IsSuccess ? operateResult3 : XGKCnet.ExtractActualData(operateResult3.Content, true);
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
            OperateResult<byte[]> operateResult2 = this.Write(dataTypeToAddress.Content1, pAddressList, operateResult1.Content);
            if (!operateResult2.IsSuccess)
                return (OperateResult)operateResult2;
            OperateResult<byte[]> operateResult3 = this.ReadBase(operateResult2.Content);
            return !operateResult3.IsSuccess ? (OperateResult)operateResult3 : (OperateResult)XGKCnet.ExtractActualData(operateResult3.Content, false);
        }

        /// <summary>Read</summary>
        /// <param name="pDataType"></param>
        /// <param name="pAddress"></param>
        /// <param name="pMemtype"></param>
        /// <param name="pDataCount"></param>
        /// <returns></returns>
        public OperateResult<byte[]> Read(
          XGT_DataType pDataType,
          List<XGTAddressData> pAddress,
          XGT_MemoryType pMemtype,
          int pDataCount = 0)
        {
            if (pAddress.Count > 16)
                return new OperateResult<byte[]>("You cannot read more than 16 pieces.");
            try
            {
                return OperateResult.CreateSuccessResult<byte[]>(this.CreateReadDataFormat(this.Station, XGT_Request_Func.Read, pDataType, pAddress, pMemtype, pDataCount));
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
        /// <param name="pDataCount"></param>
        /// <returns></returns>
        public OperateResult<byte[]> Write(
          XGT_DataType pDataType,
          List<XGTAddressData> pAddressList,
          XGT_MemoryType pMemtype,
          int pDataCount = 0)
        {
            try
            {
                return OperateResult.CreateSuccessResult<byte[]>(this.CreateWriteDataFormat(this.Station, XGT_Request_Func.Write, pDataType, pAddressList, pMemtype, pDataCount));
            }
            catch (Exception ex)
            {
                return new OperateResult<byte[]>("ERROR:" + ex.Message.ToString());
            }
        }

        private byte[] CreateReadDataFormat(
          byte station,
          XGT_Request_Func emFunc,
          XGT_DataType emDatatype,
          List<XGTAddressData> pAddressList,
          XGT_MemoryType emMemtype,
          int pDataCount)
        {
            List<XGTAddressData> xgtAddressDataList = new List<XGTAddressData>();
            foreach (XGTAddressData pAddress in pAddressList)
            {
                string valueName = new XGKFastEnet().CreateValueName(emDatatype, emMemtype, pAddress.Address);
                xgtAddressDataList.Add(new XGTAddressData()
                {
                    AddressString = valueName
                });
            }
            List<byte> byteList = new List<byte>();
            if (XGT_DataType.Continue == emDatatype && XGT_Request_Func.Read == emFunc)
            {
                byteList.Add((byte)5);
                byteList.AddRange((IEnumerable<byte>)SoftBasic.BuildAsciiBytesFrom(station));
                byteList.Add((byte)114);
                byteList.Add((byte)83);
                byteList.Add((byte)66);
                foreach (XGTAddressData xgtAddressData in xgtAddressDataList)
                {
                    byteList.AddRange((IEnumerable<byte>)SoftBasic.BuildAsciiBytesFrom((byte)xgtAddressData.AddressString.Length));
                    byteList.AddRange((IEnumerable<byte>)Encoding.ASCII.GetBytes(xgtAddressData.AddressString));
                    byteList.AddRange((IEnumerable<byte>)SoftBasic.BuildAsciiBytesFrom((byte)pDataCount));
                }
                byteList.Add((byte)4);
                int num = 0;
                for (int index = 0; index < byteList.Count; ++index)
                    num += (int)byteList[index];
                byteList.AddRange((IEnumerable<byte>)SoftBasic.BuildAsciiBytesFrom((byte)num));
            }
            else
            {
                byteList.Add((byte)5);
                byteList.AddRange((IEnumerable<byte>)SoftBasic.BuildAsciiBytesFrom(station));
                byteList.Add((byte)114);
                byteList.Add((byte)83);
                byteList.Add((byte)83);
                byteList.Add((byte)48);
                byteList.Add((byte)49);
                foreach (XGTAddressData xgtAddressData in xgtAddressDataList)
                {
                    byteList.AddRange((IEnumerable<byte>)SoftBasic.BuildAsciiBytesFrom((byte)xgtAddressData.AddressString.Length));
                    byteList.AddRange((IEnumerable<byte>)Encoding.ASCII.GetBytes(xgtAddressData.AddressString));
                }
                byteList.Add((byte)4);
                int num = 0;
                for (int index = 0; index < byteList.Count; ++index)
                    num += (int)byteList[index];
                byteList.AddRange((IEnumerable<byte>)SoftBasic.BuildAsciiBytesFrom((byte)num));
            }
            return byteList.ToArray();
        }

        private byte[] CreateWriteDataFormat(
          byte station,
          XGT_Request_Func emFunc,
          XGT_DataType emDatatype,
          List<XGTAddressData> pAddressList,
          XGT_MemoryType emMemtype,
          int pDataCount)
        {
            List<XGTAddressData> xgtAddressDataList = new List<XGTAddressData>();
            foreach (XGTAddressData pAddress in pAddressList)
            {
                string valueName = new XGKFastEnet().CreateValueName(emDatatype, emMemtype, pAddress.Address);
                pAddress.AddressString = valueName;
                xgtAddressDataList.Add(pAddress);
            }
            List<byte> byteList = new List<byte>();
            if (XGT_DataType.Continue == emDatatype && XGT_Request_Func.Write == emFunc)
            {
                byteList.Add((byte)5);
                byteList.AddRange((IEnumerable<byte>)SoftBasic.BuildAsciiBytesFrom(station));
                byteList.Add((byte)119);
                byteList.Add((byte)83);
                byteList.Add((byte)66);
                foreach (XGTAddressData xgtAddressData in xgtAddressDataList)
                {
                    byteList.AddRange((IEnumerable<byte>)SoftBasic.BuildAsciiBytesFrom((byte)xgtAddressData.AddressString.Length));
                    byteList.AddRange((IEnumerable<byte>)Encoding.ASCII.GetBytes(xgtAddressData.AddressString));
                    byteList.AddRange((IEnumerable<byte>)SoftBasic.BuildAsciiBytesFrom((byte)xgtAddressData.AddressByteArray.Length));
                    byteList.AddRange((IEnumerable<byte>)SoftBasic.BytesToAsciiBytes(xgtAddressData.AddressByteArray));
                }
                byteList.Add((byte)4);
                int num = 0;
                for (int index = 0; index < byteList.Count; ++index)
                    num += (int)byteList[index];
                byteList.AddRange((IEnumerable<byte>)SoftBasic.BuildAsciiBytesFrom((byte)num));
            }
            else
            {
                byteList.Add((byte)5);
                byteList.AddRange((IEnumerable<byte>)SoftBasic.BuildAsciiBytesFrom(station));
                byteList.Add((byte)119);
                byteList.Add((byte)83);
                byteList.Add((byte)83);
                byteList.Add((byte)48);
                byteList.Add((byte)49);
                foreach (XGTAddressData xgtAddressData in xgtAddressDataList)
                {
                    byteList.AddRange((IEnumerable<byte>)SoftBasic.BuildAsciiBytesFrom((byte)xgtAddressData.AddressString.Length));
                    byteList.AddRange((IEnumerable<byte>)Encoding.ASCII.GetBytes(xgtAddressData.AddressString));
                    byteList.AddRange((IEnumerable<byte>)SoftBasic.BytesToAsciiBytes(xgtAddressData.AddressByteArray));
                }
                byteList.Add((byte)4);
                int num = 0;
                for (int index = 0; index < byteList.Count; ++index)
                    num += (int)byteList[index];
                byteList.AddRange((IEnumerable<byte>)SoftBasic.BuildAsciiBytesFrom((byte)num));
            }
            return byteList.ToArray();
        }

        /// <summary>Extract actual data form plc response</summary>
        /// <param name="response">response data</param>
        /// <param name="isRead">read</param>
        /// <returns>result</returns>
        public static OperateResult<byte[]> ExtractActualData(byte[] response, bool isRead)
        {
            try
            {
                if (isRead)
                {
                    if (response[0] == (byte)6)
                    {
                        byte[] inBytes = new byte[response.Length - 13];
                        Array.Copy((Array)response, 10, (Array)inBytes, 0, inBytes.Length);
                        return OperateResult.CreateSuccessResult<byte[]>(SoftBasic.AsciiBytesToBytes(inBytes));
                    }
                    byte[] inBytes1 = new byte[response.Length - 9];
                    Array.Copy((Array)response, 6, (Array)inBytes1, 0, inBytes1.Length);
                    return new OperateResult<byte[]>((int)BitConverter.ToUInt16(SoftBasic.AsciiBytesToBytes(inBytes1), 0), "Data:" + SoftBasic.ByteToHexString(response));
                }
                if (response[0] == (byte)6)
                    return OperateResult.CreateSuccessResult<byte[]>(new byte[0]);
                byte[] inBytes2 = new byte[response.Length - 9];
                Array.Copy((Array)response, 6, (Array)inBytes2, 0, inBytes2.Length);
                return new OperateResult<byte[]>((int)BitConverter.ToUInt16(SoftBasic.AsciiBytesToBytes(inBytes2), 0), "Data:" + SoftBasic.ByteToHexString(response));
            }
            catch (Exception ex)
            {
                return new OperateResult<byte[]>(ex.Message);
            }
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("XGkCnet[{0}:{1}]", (object)this.PortName, (object)this.BaudRate);
    }
}
