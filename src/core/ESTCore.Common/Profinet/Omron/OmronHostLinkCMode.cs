// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Omron.OmronHostLinkCMode
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;
using ESTCore.Common.Core;
using ESTCore.Common.Reflection;
using ESTCore.Common.Serial;

using System;
using System.Text;

namespace ESTCore.Common.Profinet.Omron
{
    /// <summary>
    /// 欧姆龙的HostLink的C-Mode实现形式，地址支持携带站号信息，例如：s=2;D100<br />
    /// Omron's HostLink C-Mode implementation form, the address supports carrying station number information, for example: s=2;D100
    /// </summary>
    /// <remarks>暂时只支持的字数据的读写操作，不支持位的读写操作。</remarks>
    public class OmronHostLinkCMode : SerialDeviceBase
    {
        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronFinsNet.#ctor" />
        public OmronHostLinkCMode()
        {
            this.ByteTransform = (IByteTransform)new ReverseWordTransform();
            this.WordLength = (ushort)1;
            this.ByteTransform.DataFormat = DataFormat.CDAB;
            this.ByteTransform.IsStringReverseByteWord = true;
        }

        /// <inheritdoc cref="P:ESTCore.Common.Profinet.Omron.OmronHostLinkOverTcp.UnitNumber" />
        public byte UnitNumber { get; set; }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronFinsNet.Read(System.String,System.UInt16)" />
        [EstMqttApi("ReadByteArray", "")]
        public override OperateResult<byte[]> Read(string address, ushort length)
        {
            byte parameter = (byte)EstHelper.ExtractParameter(ref address, "s", (int)this.UnitNumber);
            OperateResult<byte[]> operateResult1 = OmronHostLinkCMode.BuildReadCommand(address, length, false);
            if (!operateResult1.IsSuccess)
                return operateResult1;
            OperateResult<byte[]> operateResult2 = this.ReadBase(OmronHostLinkCMode.PackCommand(operateResult1.Content, parameter));
            if (!operateResult2.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult2);
            OperateResult<byte[]> operateResult3 = OmronHostLinkCMode.ResponseValidAnalysis(operateResult2.Content, true);
            return !operateResult3.IsSuccess ? OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult3) : OperateResult.CreateSuccessResult<byte[]>(operateResult3.Content);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Omron.OmronFinsNet.Write(System.String,System.Byte[])" />
        [EstMqttApi("WriteByteArray", "")]
        public override OperateResult Write(string address, byte[] value)
        {
            byte parameter = (byte)EstHelper.ExtractParameter(ref address, "s", (int)this.UnitNumber);
            OperateResult<byte[]> operateResult1 = OmronHostLinkCMode.BuildWriteWordCommand(address, value);
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1;
            OperateResult<byte[]> operateResult2 = this.ReadBase(OmronHostLinkCMode.PackCommand(operateResult1.Content, parameter));
            if (!operateResult2.IsSuccess)
                return (OperateResult)operateResult2;
            OperateResult<byte[]> operateResult3 = OmronHostLinkCMode.ResponseValidAnalysis(operateResult2.Content, false);
            return !operateResult3.IsSuccess ? (OperateResult)operateResult3 : OperateResult.CreateSuccessResult();
        }

        /// <summary>读取PLC的当前的型号信息</summary>
        /// <returns>型号</returns>
        [EstMqttApi]
        public OperateResult<string> ReadPlcModel()
        {
            OperateResult<byte[]> operateResult = this.ReadBase(OmronHostLinkCMode.PackCommand(Encoding.ASCII.GetBytes("MM"), this.UnitNumber));
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<string>((OperateResult)operateResult);
            int int32 = Convert.ToInt32(Encoding.ASCII.GetString(operateResult.Content, 5, 2), 16);
            return int32 > 0 ? new OperateResult<string>(int32, "Unknown Error") : OmronHostLinkCMode.GetModelText(Encoding.ASCII.GetString(operateResult.Content, 7, 2));
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("OmronHostLinkCMode[{0}:{1}]", (object)this.PortName, (object)this.BaudRate);

        /// <summary>
        /// 解析欧姆龙的数据地址，参考来源是Omron手册第188页，比如D100， E1.100<br />
        /// Analyze Omron's data address, the reference source is page 188 of the Omron manual, such as D100, E1.100
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <param name="isBit">是否是位地址</param>
        /// <param name="isRead">是否读取</param>
        /// <returns>解析后的结果地址对象</returns>
        public static OperateResult<string, string> AnalysisAddress(
          string address,
          bool isBit,
          bool isRead)
        {
            OperateResult<string, string> operateResult = new OperateResult<string, string>();
            try
            {
                switch (address[0])
                {
                    case 'A':
                    case 'a':
                        operateResult.Content1 = isRead ? "RJ" : "WJ";
                        break;
                    case 'C':
                    case 'c':
                        operateResult.Content1 = isRead ? "RR" : "WR";
                        break;
                    case 'D':
                    case 'd':
                        operateResult.Content1 = isRead ? "RD" : "WD";
                        break;
                    case 'E':
                    case 'e':
                        int int32 = Convert.ToInt32(address.Split(new char[1]
                        {
              '.'
                        }, StringSplitOptions.RemoveEmptyEntries)[0].Substring(1), 16);
                        operateResult.Content1 = (isRead ? "RE" : "WE") + Encoding.ASCII.GetString(SoftBasic.BuildAsciiBytesFrom((byte)int32));
                        break;
                    case 'H':
                    case 'h':
                        operateResult.Content1 = isRead ? "RH" : "WH";
                        break;
                    default:
                        throw new Exception(StringResources.Language.NotSupportedDataType);
                }
                if (address[0] == 'E' || address[0] == 'e')
                {
                    string[] strArray = address.Split(new char[1]
                    {
            '.'
                    }, StringSplitOptions.RemoveEmptyEntries);
                    if (!isBit)
                    {
                        ushort num = ushort.Parse(strArray[1]);
                        operateResult.Content2 = num.ToString("D4");
                    }
                }
                else if (!isBit)
                {
                    ushort num = ushort.Parse(address.Substring(1));
                    operateResult.Content2 = num.ToString("D4");
                }
            }
            catch (Exception ex)
            {
                operateResult.Message = ex.Message;
                return operateResult;
            }
            operateResult.IsSuccess = true;
            return operateResult;
        }

        /// <summary>
        /// 根据读取的地址，长度，是否位读取创建Fins协议的核心报文<br />
        /// According to the read address, length, whether to read the core message that creates the Fins protocol
        /// </summary>
        /// <param name="address">地址，具体格式请参照示例说明</param>
        /// <param name="length">读取的数据长度</param>
        /// <param name="isBit">是否使用位读取</param>
        /// <returns>带有成功标识的Fins核心报文</returns>
        public static OperateResult<byte[]> BuildReadCommand(
          string address,
          ushort length,
          bool isBit)
        {
            OperateResult<string, string> operateResult = OmronHostLinkCMode.AnalysisAddress(address, isBit, true);
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(operateResult.Content1);
            stringBuilder.Append(operateResult.Content2);
            stringBuilder.Append(length.ToString("D4"));
            return OperateResult.CreateSuccessResult<byte[]>(Encoding.ASCII.GetBytes(stringBuilder.ToString()));
        }

        /// <summary>
        /// 根据读取的地址，长度，是否位读取创建Fins协议的核心报文<br />
        /// According to the read address, length, whether to read the core message that creates the Fins protocol
        /// </summary>
        /// <param name="address">地址，具体格式请参照示例说明</param>
        /// <param name="value">等待写入的数据</param>
        /// <returns>带有成功标识的Fins核心报文</returns>
        public static OperateResult<byte[]> BuildWriteWordCommand(
          string address,
          byte[] value)
        {
            OperateResult<string, string> operateResult = OmronHostLinkCMode.AnalysisAddress(address, false, false);
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(operateResult.Content1);
            stringBuilder.Append(operateResult.Content2);
            for (int index = 0; index < value.Length / 2; ++index)
                stringBuilder.Append(BitConverter.ToUInt16(value, index * 2).ToString("X4"));
            return OperateResult.CreateSuccessResult<byte[]>(Encoding.ASCII.GetBytes(stringBuilder.ToString()));
        }

        /// <summary>验证欧姆龙的Fins-TCP返回的数据是否正确的数据，如果正确的话，并返回所有的数据内容</summary>
        /// <param name="response">来自欧姆龙返回的数据内容</param>
        /// <param name="isRead">是否读取</param>
        /// <returns>带有是否成功的结果对象</returns>
        public static OperateResult<byte[]> ResponseValidAnalysis(
          byte[] response,
          bool isRead)
        {
            if (response.Length < 11)
                return new OperateResult<byte[]>(StringResources.Language.OmronReceiveDataError);
            int int32 = Convert.ToInt32(Encoding.ASCII.GetString(response, 5, 2), 16);
            byte[] numArray1 = (byte[])null;
            if (response.Length > 11)
            {
                byte[] numArray2 = new byte[(response.Length - 11) / 2];
                for (int index = 0; index < numArray2.Length / 2; ++index)
                    BitConverter.GetBytes(Convert.ToUInt16(Encoding.ASCII.GetString(response, 7 + 4 * index, 4), 16)).CopyTo((Array)numArray2, index * 2);
                numArray1 = numArray2;
            }
            if (int32 <= 0)
                return OperateResult.CreateSuccessResult<byte[]>(numArray1);
            OperateResult<byte[]> operateResult = new OperateResult<byte[]>();
            operateResult.ErrorCode = int32;
            operateResult.Content = numArray1;
            return operateResult;
        }

        /// <summary>将普通的指令打包成完整的指令</summary>
        /// <param name="cmd">fins指令</param>
        /// <param name="unitNumber">站号信息</param>
        /// <returns>完整的质量</returns>
        public static byte[] PackCommand(byte[] cmd, byte unitNumber)
        {
            byte[] bytes = new byte[7 + cmd.Length];
            bytes[0] = (byte)64;
            bytes[1] = SoftBasic.BuildAsciiBytesFrom(unitNumber)[0];
            bytes[2] = SoftBasic.BuildAsciiBytesFrom(unitNumber)[1];
            bytes[bytes.Length - 2] = (byte)42;
            bytes[bytes.Length - 1] = (byte)13;
            cmd.CopyTo((Array)bytes, 3);
            int num = (int)bytes[0];
            for (int index = 1; index < bytes.Length - 4; ++index)
                num ^= (int)bytes[index];
            bytes[bytes.Length - 4] = SoftBasic.BuildAsciiBytesFrom((byte)num)[0];
            bytes[bytes.Length - 3] = SoftBasic.BuildAsciiBytesFrom((byte)num)[1];
            Console.WriteLine(Encoding.ASCII.GetString(bytes));
            return bytes;
        }

        /// <summary>获取model的字符串描述信息</summary>
        /// <param name="model">型号代码</param>
        /// <returns>是否解析成功</returns>
        public static OperateResult<string> GetModelText(string model)
        {
            switch (model)
            {
                case "01":
                    return OperateResult.CreateSuccessResult<string>("C250");
                case "02":
                    return OperateResult.CreateSuccessResult<string>("C500");
                case "03":
                    return OperateResult.CreateSuccessResult<string>("C120/C50");
                case "09":
                    return OperateResult.CreateSuccessResult<string>("C250F");
                case "0A":
                    return OperateResult.CreateSuccessResult<string>("C500F");
                case "0B":
                    return OperateResult.CreateSuccessResult<string>("C120F");
                case "0E":
                    return OperateResult.CreateSuccessResult<string>("C2000");
                case "10":
                    return OperateResult.CreateSuccessResult<string>("C1000H");
                case "11":
                    return OperateResult.CreateSuccessResult<string>("C2000H/CQM1/CPM1");
                case "12":
                    return OperateResult.CreateSuccessResult<string>("C20H/C28H/C40H, C200H, C200HS, C200HX/HG/HE (-ZE)");
                case "20":
                    return OperateResult.CreateSuccessResult<string>("CV500");
                case "21":
                    return OperateResult.CreateSuccessResult<string>("CV1000");
                case "22":
                    return OperateResult.CreateSuccessResult<string>("CV2000");
                case "30":
                    return OperateResult.CreateSuccessResult<string>("CS/CJ");
                case "40":
                    return OperateResult.CreateSuccessResult<string>("CVM1-CPU01-E");
                case "41":
                    return OperateResult.CreateSuccessResult<string>("CVM1-CPU11-E");
                case "42":
                    return OperateResult.CreateSuccessResult<string>("CVM1-CPU21-E");
                default:
                    return new OperateResult<string>("Unknown model, model code:" + model);
            }
        }
    }
}
