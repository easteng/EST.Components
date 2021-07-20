// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.AllenBradley.AllenBradleyDF1Serial
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;
using ESTCore.Common.Core;
using ESTCore.Common.Serial;

using System;
using System.IO;

namespace ESTCore.Common.Profinet.AllenBradley
{
    /// <summary>
    /// AB-PLC的DF1通信协议，基于串口实现，通信机制为半双工，目前适用于 Micro-Logix1000,SLC500,SLC 5/03,SLC 5/04，地址示例：N7:1
    /// </summary>
    public class AllenBradleyDF1Serial : SerialDeviceBase
    {
        private SoftIncrementCount incrementCount;

        /// <summary>
        /// Instantiate a communication object for a Allenbradley PLC protocol
        /// </summary>
        public AllenBradleyDF1Serial()
        {
            this.WordLength = (ushort)2;
            this.ByteTransform = (IByteTransform)new RegularByteTransform();
            this.incrementCount = new SoftIncrementCount((long)ushort.MaxValue);
            this.CheckType = CheckType.CRC16;
        }

        /// <summary>站号信息</summary>
        public byte Station { get; set; }

        /// <summary>目标节点号</summary>
        public byte DstNode { get; set; }

        /// <summary>源节点号</summary>
        public byte SrcNode { get; set; }

        /// <summary>校验方式</summary>
        public CheckType CheckType { get; set; }

        /// <summary>
        /// 读取PLC的原始数据信息，地址示例：N7:0  可以携带站号 s=2;N7:0, 携带 dst 和 src 信息，例如 dst=1;src=2;N7:0
        /// </summary>
        /// <param name="address">PLC的地址信息，支持的类型见类型注释说明</param>
        /// <param name="length">读取的长度，单位，字节</param>
        /// <returns>是否读取成功的结果对象</returns>
        public override OperateResult<byte[]> Read(string address, ushort length)
        {
            byte parameter = (byte)EstHelper.ExtractParameter(ref address, "s", (int)this.Station);
            OperateResult<byte[]> operateResult1 = AllenBradleyDF1Serial.BuildProtectedTypedLogicalRead((byte)EstHelper.ExtractParameter(ref address, "dst", (int)this.DstNode), (byte)EstHelper.ExtractParameter(ref address, "src", (int)this.SrcNode), (int)this.incrementCount.GetCurrentValue(), address, length);
            if (!operateResult1.IsSuccess)
                return operateResult1;
            OperateResult<byte[]> operateResult2 = this.ReadBase(this.PackCommand(parameter, operateResult1.Content));
            return !operateResult2.IsSuccess ? operateResult2 : AllenBradleyDF1Serial.ExtraResponseContent(operateResult2.Content);
        }

        /// <summary>
        /// 写入PLC的原始数据信息，地址示例：N7:0  可以携带站号 s=2;N7:0, 携带 dst 和 src 信息，例如 dst=1;src=2;N7:0
        /// </summary>
        /// <param name="address">PLC的地址信息，支持的类型见类型注释说明</param>
        /// <param name="value">原始的数据值</param>
        /// <returns>是否写入成功</returns>
        public override OperateResult Write(string address, byte[] value)
        {
            byte parameter = (byte)EstHelper.ExtractParameter(ref address, "s", (int)this.Station);
            OperateResult<byte[]> operateResult1 = AllenBradleyDF1Serial.BuildProtectedTypedLogicalWrite((byte)EstHelper.ExtractParameter(ref address, "dst", (int)this.DstNode), (byte)EstHelper.ExtractParameter(ref address, "src", (int)this.SrcNode), (int)this.incrementCount.GetCurrentValue(), address, value);
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            OperateResult<byte[]> operateResult2 = this.ReadBase(this.PackCommand(parameter, operateResult1.Content));
            return !operateResult2.IsSuccess ? (OperateResult)operateResult2 : (OperateResult)AllenBradleyDF1Serial.ExtraResponseContent(operateResult2.Content);
        }

        private byte[] CalculateCheckResult(byte station, byte[] command)
        {
            if (this.CheckType == CheckType.BCC)
            {
                int num = (int)station;
                for (int index = 0; index < command.Length; ++index)
                    num += (int)command[index];
                return new byte[1] { (byte)((int)(byte)~num + 1) };
            }
            return SoftCRC16.CRC16(SoftBasic.SpliceArray<byte>(new byte[1]
            {
        station
            }, new byte[1] { (byte)2 }, command, new byte[1]
            {
        (byte) 3
            }), (byte)160, (byte)1, (byte)0, (byte)0).SelectLast<byte>(2);
        }

        /// <summary>打包命令的操作，加站号进行打包成完整的数据内容，命令内容为原始命令，打包后会自动补充0x10的值</summary>
        /// <param name="station">站号信息</param>
        /// <param name="command">等待发送的命令</param>
        /// <returns>打包之后的数据内容</returns>
        public byte[] PackCommand(byte station, byte[] command)
        {
            byte[] checkResult = this.CalculateCheckResult(station, command);
            MemoryStream memoryStream = new MemoryStream();
            memoryStream.WriteByte((byte)16);
            memoryStream.WriteByte((byte)1);
            memoryStream.WriteByte(station);
            if (station == (byte)16)
                memoryStream.WriteByte(station);
            memoryStream.WriteByte((byte)16);
            memoryStream.WriteByte((byte)2);
            for (int index = 0; index < command.Length; ++index)
            {
                memoryStream.WriteByte(command[index]);
                if (command[index] == (byte)16)
                    memoryStream.WriteByte(command[index]);
            }
            memoryStream.WriteByte((byte)16);
            memoryStream.WriteByte((byte)3);
            memoryStream.Write(checkResult, 0, checkResult.Length);
            byte[] array = memoryStream.ToArray();
            memoryStream.Dispose();
            return array;
        }

        /// <summary>
        /// 构建0F-A2命令码的报文读取指令，用来读取文件数据。适用 Micro-Logix1000,SLC500,SLC 5/03,SLC 5/04，地址示例：N7:1<br />
        /// Construct a message read instruction of 0F-A2 command code to read file data. Applicable to Micro-Logix1000, SLC500, SLC 5/03, SLC 5/04, address example: N7:1
        /// </summary>
        /// <param name="dstNode">目标节点号</param>
        /// <param name="srcNode">原节点号</param>
        /// <param name="tns">消息号</param>
        /// <param name="address">PLC的地址信息</param>
        /// <param name="length">读取的数据长度</param>
        /// <returns>初步的报文信息</returns>
        /// <remarks>
        /// 对于SLC 5/01或SLC 5/02而言，一次最多读取82个字节。对于 03 或是 04 为225，236字节取决于是否应用DF1驱动
        /// </remarks>
        public static OperateResult<byte[]> BuildProtectedTypedLogicalRead(
          byte dstNode,
          byte srcNode,
          int tns,
          string address,
          ushort length)
        {
            OperateResult<byte, byte, ushort> operateResult = AllenBradleySLCNet.AnalysisAddress(address);
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult);
            return OperateResult.CreateSuccessResult<byte[]>(new byte[12]
            {
        dstNode,
        srcNode,
        (byte) 15,
        (byte) 0,
        BitConverter.GetBytes(tns)[0],
        BitConverter.GetBytes(tns)[1],
        (byte) 162,
        BitConverter.GetBytes(length)[0],
        operateResult.Content2,
        operateResult.Content1,
        BitConverter.GetBytes(operateResult.Content3)[0],
        BitConverter.GetBytes(operateResult.Content3)[1]
            });
        }

        /// <summary>
        /// 构建0F-AA命令码的写入读取指令，用来写入文件数据。适用 Micro-Logix1000,SLC500,SLC 5/03,SLC 5/04，地址示例：N7:1<br />
        /// Construct a write and read command of 0F-AA command code to write file data. Applicable to Micro-Logix1000, SLC500, SLC 5/03, SLC 5/04, address example: N7:1
        /// </summary>
        /// <param name="dstNode">目标节点号</param>
        /// <param name="srcNode">原节点号</param>
        /// <param name="tns">消息号</param>
        /// <param name="address">PLC的地址信息</param>
        /// <param name="data">写入的数据内容</param>
        /// <returns>初步的报文信息</returns>
        /// <remarks>
        /// 对于SLC 5/01或SLC 5/02而言，一次最多读取82个字节。对于 03 或是 04 为225，236字节取决于是否应用DF1驱动
        /// </remarks>
        public static OperateResult<byte[]> BuildProtectedTypedLogicalWrite(
          byte dstNode,
          byte srcNode,
          int tns,
          string address,
          byte[] data)
        {
            OperateResult<byte, byte, ushort> operateResult = AllenBradleySLCNet.AnalysisAddress(address);
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult);
            byte[] numArray = new byte[12 + data.Length];
            numArray[0] = dstNode;
            numArray[1] = srcNode;
            numArray[2] = (byte)15;
            numArray[3] = (byte)0;
            numArray[4] = BitConverter.GetBytes(tns)[0];
            numArray[5] = BitConverter.GetBytes(tns)[1];
            numArray[6] = (byte)170;
            numArray[7] = BitConverter.GetBytes(data.Length)[0];
            numArray[8] = operateResult.Content2;
            numArray[9] = operateResult.Content1;
            numArray[10] = BitConverter.GetBytes(operateResult.Content3)[0];
            numArray[11] = BitConverter.GetBytes(operateResult.Content3)[1];
            data.CopyTo((Array)numArray, 12);
            return OperateResult.CreateSuccessResult<byte[]>(numArray);
        }

        /// <summary>
        /// 提取返回报文的数据内容，将其转换成实际的数据内容，如果PLC返回了错误信息，则结果对象为失败。<br />
        /// Extract the data content of the returned message and convert it into the actual data content. If the PLC returns an error message, the result object is a failure.
        /// </summary>
        /// <param name="content">PLC返回的报文信息</param>
        /// <returns>结果对象内容</returns>
        public static OperateResult<byte[]> ExtraResponseContent(byte[] content)
        {
            try
            {
                int num = -1;
                for (int index = 0; index < content.Length; ++index)
                {
                    if (content[index] == (byte)16 && content[index + 1] == (byte)2)
                    {
                        num = index + 2;
                        break;
                    }
                }
                if (num < 0 || num >= content.Length - 6)
                    return new OperateResult<byte[]>("Message must start with '10 02', source: " + content.ToHexString(' '));
                MemoryStream memoryStream = new MemoryStream();
                for (int index = num; index < content.Length - 1; ++index)
                {
                    if (content[index] == (byte)16 && content[index + 1] == (byte)16)
                    {
                        memoryStream.WriteByte(content[index]);
                        ++index;
                    }
                    else if (content[index] != (byte)16 || content[index + 1] != (byte)3)
                        memoryStream.WriteByte(content[index]);
                    else
                        break;
                }
                content = memoryStream.ToArray();
                memoryStream.Dispose();
                if (content[3] == (byte)240)
                    return new OperateResult<byte[]>(AllenBradleyDF1Serial.GetExtStatusDescription(content[6]));
                if (content[3] > (byte)0)
                    return new OperateResult<byte[]>(AllenBradleyDF1Serial.GetStatusDescription(content[3]));
                return content.Length > 6 ? OperateResult.CreateSuccessResult<byte[]>(content.RemoveBegin<byte>(6)) : OperateResult.CreateSuccessResult<byte[]>(new byte[0]);
            }
            catch (Exception ex)
            {
                return new OperateResult<byte[]>(ex.Message + " Source:" + content.ToHexString(' '));
            }
        }

        /// <summary>根据错误代码，来获取错误的具体描述文本</summary>
        /// <param name="code">错误的代码，非0</param>
        /// <returns>错误的描述文本信息</returns>
        public static string GetStatusDescription(byte code)
        {
            byte num1 = (byte)((uint)code & 15U);
            byte num2 = (byte)((uint)code & 240U);
            switch (num1)
            {
                case 1:
                    return "DST node is out of buffer space";
                case 2:
                    return "Cannot guarantee delivery: link layer(The remote node specified does not ACK command.)";
                case 3:
                    return "Duplicate token holder detected";
                case 4:
                    return "Local port is disconnected";
                case 5:
                    return "Application layer timed out waiting for a response";
                case 6:
                    return "Duplicate node detected";
                case 7:
                    return "Station is offline";
                case 8:
                    return "Hardware fault";
                default:
                    switch (num2)
                    {
                        case 16:
                            return "Illegal command or format";
                        case 32:
                            return "Host has a problem and will not communicate";
                        case 48:
                            return "Remote node host is missing, disconnected, or shut down";
                        case 64:
                            return "Host could not complete function due to hardware fault";
                        case 80:
                            return "Addressing problem or memory protect rungs";
                        case 96:
                            return "Function not allowed due to command protection selection";
                        case 112:
                            return "Processor is in Program mode";
                        case 128:
                            return "Compatibility mode file missing or communication zone problem";
                        case 144:
                            return "Remote node cannot buffer command";
                        case 160:
                            return "Wait ACK (1775\x0006KA buffer full)";
                        case 176:
                            return "Remote node problem due to download";
                        case 192:
                            return "Wait ACK (1775\x0006KA buffer full)";
                        case 240:
                            return "Error code in the EXT STS byte";
                        default:
                            return StringResources.Language.UnknownError;
                    }
            }
        }

        /// <summary>根据错误代码，来获取错误的具体描述文本</summary>
        /// <param name="code">错误的代码，非0</param>
        /// <returns>错误的描述文本信息</returns>
        public static string GetExtStatusDescription(byte code)
        {
            switch (code)
            {
                case 1:
                    return "A field has an illegal value";
                case 2:
                    return "Less levels specified in address than minimum for any address";
                case 3:
                    return "More levels specified in address than system supports";
                case 4:
                    return "Symbol not found";
                case 5:
                    return "Symbol is of improper format";
                case 6:
                    return "Address doesn’t point to something usable";
                case 7:
                    return "File is wrong size";
                case 8:
                    return "Cannot complete request, situation has changed since the start of the command";
                case 9:
                    return "Data or file is too large";
                case 10:
                    return "Transaction size plus word address is too large";
                case 11:
                    return "Access denied, improper privilege";
                case 12:
                    return "Condition cannot be generated \x0006 resource is not available";
                case 13:
                    return "Condition already exists \x0006 resource is already available";
                case 14:
                    return "Command cannot be executed";
                case 15:
                    return "Histogram overflow";
                case 16:
                    return "No access";
                case 17:
                    return "Illegal data type";
                case 18:
                    return "Invalid parameter or invalid data";
                case 19:
                    return "Address reference exists to deleted area";
                case 20:
                    return "Command execution failure for unknown reason; possible PLC\x00063 histogram overflow";
                case 21:
                    return "Data conversion error";
                case 22:
                    return "Scanner not able to communicate with 1771 rack adapter";
                case 23:
                    return "Type mismatch";
                case 24:
                    return "1771 module response was not valid";
                case 25:
                    return "Duplicated label";
                case 26:
                    return "File is open; another node owns it";
                case 27:
                    return "Another node is the program owner";
                case 28:
                    return "Reserved";
                case 29:
                    return "Reserved";
                case 30:
                    return "Data table element protection violation";
                case 31:
                    return "Temporary internal problem";
                case 34:
                    return "Remote rack fault";
                case 35:
                    return "Timeout";
                case 36:
                    return "Unknown error";
                default:
                    return StringResources.Language.UnknownError;
            }
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("AllenBradleyDF1Serial[{0}:{1}]", (object)this.PortName, (object)this.BaudRate);
    }
}
