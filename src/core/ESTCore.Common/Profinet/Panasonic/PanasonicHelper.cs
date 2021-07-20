// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Panasonic.PanasonicHelper
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;
using ESTCore.Common.Core;

using System;
using System.Text;

namespace ESTCore.Common.Profinet.Panasonic
{
    /// <summary>
    /// 松下PLC的辅助类，提供了基本的辅助方法，用于解析地址，计算校验和，创建报文<br />
    /// The auxiliary class of Panasonic PLC provides basic auxiliary methods for parsing addresses, calculating checksums, and creating messages
    /// </summary>
    public class PanasonicHelper
    {
        private static string CalculateCrc(StringBuilder sb)
        {
            byte num = (byte)sb[0];
            for (int index = 1; index < sb.Length; ++index)
                num ^= (byte)sb[index];
            return SoftBasic.ByteToHexString(new byte[1]
            {
        num
            });
        }

        /// <summary>
        /// 位地址转换方法，101等同于10.1等同于10*16+1=161<br />
        /// Bit address conversion method, 101 is equivalent to 10.1 is equivalent to 10 * 16 + 1 = 161
        /// </summary>
        /// <param name="address">地址信息</param>
        /// <returns>实际的位地址信息</returns>
        public static int CalculateComplexAddress(string address) => address.IndexOf(".") >= 0 ? Convert.ToInt32(address.Substring(0, address.IndexOf("."))) * 16 + EstHelper.CalculateBitStartIndex(address.Substring(address.IndexOf(".") + 1)) : (address.Length != 1 ? Convert.ToInt32(address.Substring(0, address.Length - 1)) * 16 + Convert.ToInt32(address.Substring(address.Length - 1), 16) : Convert.ToInt32(address, 16));

        /// <summary>
        /// 解析数据地址，解析出地址类型，起始地址<br />
        /// Parse the data address, resolve the address type, start address
        /// </summary>
        /// <param name="address">数据地址</param>
        /// <returns>解析出地址类型，起始地址</returns>
        public static OperateResult<string, int> AnalysisAddress(string address)
        {
            OperateResult<string, int> operateResult = new OperateResult<string, int>();
            try
            {
                operateResult.Content2 = 0;
                if (address.StartsWith("IX") || address.StartsWith("ix"))
                {
                    operateResult.Content1 = "IX";
                    operateResult.Content2 = int.Parse(address.Substring(2));
                }
                else if (address.StartsWith("IY") || address.StartsWith("iy"))
                {
                    operateResult.Content1 = "IY";
                    operateResult.Content2 = int.Parse(address.Substring(2));
                }
                else if (address.StartsWith("ID") || address.StartsWith("id"))
                {
                    operateResult.Content1 = "ID";
                    operateResult.Content2 = int.Parse(address.Substring(2));
                }
                else if (address.StartsWith("SR") || address.StartsWith("sr"))
                {
                    operateResult.Content1 = "SR";
                    operateResult.Content2 = PanasonicHelper.CalculateComplexAddress(address.Substring(2));
                }
                else if (address.StartsWith("LD") || address.StartsWith("ld"))
                {
                    operateResult.Content1 = "LD";
                    operateResult.Content2 = int.Parse(address.Substring(2));
                }
                else if (address[0] == 'X' || address[0] == 'x')
                {
                    operateResult.Content1 = "X";
                    operateResult.Content2 = PanasonicHelper.CalculateComplexAddress(address.Substring(1));
                }
                else if (address[0] == 'Y' || address[0] == 'y')
                {
                    operateResult.Content1 = "Y";
                    operateResult.Content2 = PanasonicHelper.CalculateComplexAddress(address.Substring(1));
                }
                else if (address[0] == 'R' || address[0] == 'r')
                {
                    operateResult.Content1 = "R";
                    operateResult.Content2 = PanasonicHelper.CalculateComplexAddress(address.Substring(1));
                }
                else if (address[0] == 'T' || address[0] == 't')
                {
                    operateResult.Content1 = "T";
                    operateResult.Content2 = int.Parse(address.Substring(1));
                }
                else if (address[0] == 'C' || address[0] == 'c')
                {
                    operateResult.Content1 = "C";
                    operateResult.Content2 = int.Parse(address.Substring(1));
                }
                else if (address[0] == 'L' || address[0] == 'l')
                {
                    operateResult.Content1 = "L";
                    operateResult.Content2 = PanasonicHelper.CalculateComplexAddress(address.Substring(1));
                }
                else if (address[0] == 'D' || address[0] == 'd')
                {
                    operateResult.Content1 = "D";
                    operateResult.Content2 = int.Parse(address.Substring(1));
                }
                else if (address[0] == 'F' || address[0] == 'f')
                {
                    operateResult.Content1 = "F";
                    operateResult.Content2 = int.Parse(address.Substring(1));
                }
                else if (address[0] == 'S' || address[0] == 's')
                {
                    operateResult.Content1 = "S";
                    operateResult.Content2 = int.Parse(address.Substring(1));
                }
                else
                {
                    if (address[0] != 'K' && address[0] != 'k')
                        throw new Exception(StringResources.Language.NotSupportedDataType);
                    operateResult.Content1 = "K";
                    operateResult.Content2 = int.Parse(address.Substring(1));
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
        /// 创建读取离散触点的报文指令<br />
        /// Create message instructions for reading discrete contacts
        /// </summary>
        /// <param name="station">站号信息</param>
        /// <param name="address">地址信息</param>
        /// <returns>包含是否成功的结果对象</returns>
        public static OperateResult<byte[]> BuildReadOneCoil(byte station, string address)
        {
            if (address == null)
                return new OperateResult<byte[]>("address is not allowed null");
            if (address.Length < 1 || address.Length > 8)
                return new OperateResult<byte[]>("length must be 1-8");
            StringBuilder sb = new StringBuilder("%");
            sb.Append(station.ToString("X2"));
            sb.Append("#RCS");
            OperateResult<string, int> operateResult = PanasonicHelper.AnalysisAddress(address);
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult);
            sb.Append(operateResult.Content1);
            if (operateResult.Content1 == "X" || operateResult.Content1 == "Y" || operateResult.Content1 == "R" || operateResult.Content1 == "L")
            {
                sb.Append((operateResult.Content2 / 16).ToString("D3"));
                sb.Append((operateResult.Content2 % 16).ToString("X1"));
            }
            else
            {
                if (!(operateResult.Content1 == "T") && !(operateResult.Content1 == "C"))
                    return new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType);
                sb.Append("0");
                sb.Append(operateResult.Content2.ToString("D3"));
            }
            sb.Append(PanasonicHelper.CalculateCrc(sb));
            sb.Append('\r');
            return OperateResult.CreateSuccessResult<byte[]>(Encoding.ASCII.GetBytes(sb.ToString()));
        }

        /// <summary>
        /// 创建写入离散触点的报文指令<br />
        /// Create message instructions to write discrete contacts
        /// </summary>
        /// <param name="station">站号信息</param>
        /// <param name="address">地址信息</param>
        /// <param name="value">bool值数组</param>
        /// <returns>包含是否成功的结果对象</returns>
        public static OperateResult<byte[]> BuildWriteOneCoil(
          byte station,
          string address,
          bool value)
        {
            StringBuilder sb = new StringBuilder("%");
            sb.Append(station.ToString("X2"));
            sb.Append("#WCS");
            OperateResult<string, int> operateResult = PanasonicHelper.AnalysisAddress(address);
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult);
            sb.Append(operateResult.Content1);
            if (operateResult.Content1 == "X" || operateResult.Content1 == "Y" || operateResult.Content1 == "R" || operateResult.Content1 == "L")
            {
                sb.Append((operateResult.Content2 / 16).ToString("D3"));
                sb.Append((operateResult.Content2 % 16).ToString("X1"));
            }
            else
            {
                if (!(operateResult.Content1 == "T") && !(operateResult.Content1 == "C"))
                    return new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType);
                sb.Append("0");
                sb.Append(operateResult.Content2.ToString("D3"));
            }
            sb.Append(value ? '1' : '0');
            sb.Append(PanasonicHelper.CalculateCrc(sb));
            sb.Append('\r');
            return OperateResult.CreateSuccessResult<byte[]>(Encoding.ASCII.GetBytes(sb.ToString()));
        }

        /// <summary>
        /// 创建批量读取触点的报文指令<br />
        /// Create message instructions for batch reading contacts
        /// </summary>
        /// <param name="station">站号信息</param>
        /// <param name="address">地址信息</param>
        /// <param name="length">数据长度</param>
        /// <returns>包含是否成功的结果对象</returns>
        public static OperateResult<byte[]> BuildReadCommand(
          byte station,
          string address,
          ushort length)
        {
            if (address == null)
                return new OperateResult<byte[]>(StringResources.Language.PanasonicAddressParameterCannotBeNull);
            OperateResult<string, int> operateResult = PanasonicHelper.AnalysisAddress(address);
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult);
            StringBuilder sb = new StringBuilder("%");
            sb.Append(station.ToString("X2"));
            sb.Append("#");
            if (operateResult.Content1 == "X" || operateResult.Content1 == "Y" || operateResult.Content1 == "R" || operateResult.Content1 == "L")
            {
                sb.Append("RCC");
                sb.Append(operateResult.Content1);
                int num1 = operateResult.Content2 / 16;
                int num2 = (operateResult.Content2 + (int)length - 1) / 16;
                sb.Append(num1.ToString("D4"));
                sb.Append(num2.ToString("D4"));
            }
            else if (operateResult.Content1 == "D" || operateResult.Content1 == "LD" || operateResult.Content1 == "F")
            {
                sb.Append("RD");
                sb.Append(operateResult.Content1.Substring(0, 1));
                sb.Append(operateResult.Content2.ToString("D5"));
                sb.Append((operateResult.Content2 + (int)length - 1).ToString("D5"));
            }
            else if (operateResult.Content1 == "IX" || operateResult.Content1 == "IY" || operateResult.Content1 == "ID")
            {
                sb.Append("RD");
                sb.Append(operateResult.Content1);
                sb.Append("000000000");
            }
            else
            {
                if (!(operateResult.Content1 == "C") && !(operateResult.Content1 == "T"))
                    return new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType);
                sb.Append("RS");
                sb.Append(operateResult.Content2.ToString("D4"));
                sb.Append((operateResult.Content2 + (int)length - 1).ToString("D4"));
            }
            sb.Append(PanasonicHelper.CalculateCrc(sb));
            sb.Append('\r');
            return OperateResult.CreateSuccessResult<byte[]>(Encoding.ASCII.GetBytes(sb.ToString()));
        }

        /// <summary>
        /// 创建批量读取触点的报文指令<br />
        /// Create message instructions for batch reading contacts
        /// </summary>
        /// <param name="station">设备站号</param>
        /// <param name="address">地址信息</param>
        /// <param name="values">数据值</param>
        /// <returns>包含是否成功的结果对象</returns>
        public static OperateResult<byte[]> BuildWriteCommand(
          byte station,
          string address,
          byte[] values)
        {
            if (address == null)
                return new OperateResult<byte[]>(StringResources.Language.PanasonicAddressParameterCannotBeNull);
            OperateResult<string, int> operateResult = PanasonicHelper.AnalysisAddress(address);
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult);
            values = SoftBasic.ArrayExpandToLengthEven<byte>(values);
            short num1 = (short)(values.Length / 2);
            StringBuilder sb = new StringBuilder("%");
            sb.Append(station.ToString("X2"));
            sb.Append("#");
            if (operateResult.Content1 == "X" || operateResult.Content1 == "Y" || operateResult.Content1 == "R" || operateResult.Content1 == "L")
            {
                sb.Append("WCC");
                sb.Append(operateResult.Content1);
                int num2 = operateResult.Content2 / 16;
                int num3 = (operateResult.Content2 + values.Length * 8 - 1) / 16;
                sb.Append(num2.ToString("D4"));
                sb.Append((num3 - num2 + 1).ToString("D4"));
            }
            else if (operateResult.Content1 == "D" || operateResult.Content1 == "LD" || operateResult.Content1 == "F")
            {
                sb.Append("WD");
                sb.Append(operateResult.Content1.Substring(0, 1));
                sb.Append(operateResult.Content2.ToString("D5"));
                sb.Append((operateResult.Content2 + (int)num1 - 1).ToString("D5"));
            }
            else if (operateResult.Content1 == "IX" || operateResult.Content1 == "IY" || operateResult.Content1 == "ID")
            {
                sb.Append("WD");
                sb.Append(operateResult.Content1);
                StringBuilder stringBuilder1 = sb;
                int num2 = operateResult.Content2;
                string str1 = num2.ToString("D9");
                stringBuilder1.Append(str1);
                StringBuilder stringBuilder2 = sb;
                num2 = operateResult.Content2 + (int)num1 - 1;
                string str2 = num2.ToString("D9");
                stringBuilder2.Append(str2);
            }
            else if (operateResult.Content1 == "C" || operateResult.Content1 == "T")
            {
                sb.Append("WS");
                sb.Append(operateResult.Content2.ToString("D4"));
                sb.Append((operateResult.Content2 + (int)num1 - 1).ToString("D4"));
            }
            sb.Append(SoftBasic.ByteToHexString(values));
            sb.Append(PanasonicHelper.CalculateCrc(sb));
            sb.Append('\r');
            return OperateResult.CreateSuccessResult<byte[]>(Encoding.ASCII.GetBytes(sb.ToString()));
        }

        /// <summary>
        /// 检查从PLC反馈的数据，并返回正确的数据内容<br />
        /// Check the data feedback from the PLC and return the correct data content
        /// </summary>
        /// <param name="response">反馈信号</param>
        /// <returns>是否成功的结果信息</returns>
        public static OperateResult<byte[]> ExtraActualData(byte[] response)
        {
            if (response.Length < 9)
                return new OperateResult<byte[]>(StringResources.Language.PanasonicReceiveLengthMustLargerThan9);
            if (response[3] == (byte)36)
            {
                byte[] bytes = new byte[response.Length - 9];
                if ((uint)bytes.Length > 0U)
                {
                    Array.Copy((Array)response, 6, (Array)bytes, 0, bytes.Length);
                    bytes = SoftBasic.HexStringToBytes(Encoding.ASCII.GetString(bytes));
                }
                return OperateResult.CreateSuccessResult<byte[]>(bytes);
            }
            if (response[3] != (byte)33)
                return new OperateResult<byte[]>(StringResources.Language.UnknownError);
            int err = int.Parse(Encoding.ASCII.GetString(response, 4, 2));
            return new OperateResult<byte[]>(err, PanasonicHelper.GetErrorDescription(err));
        }

        /// <summary>
        /// 检查从PLC反馈的数据，并返回正确的数据内容<br />
        /// Check the data feedback from the PLC and return the correct data content
        /// </summary>
        /// <param name="response">反馈信号</param>
        /// <returns>是否成功的结果信息</returns>
        public static OperateResult<bool> ExtraActualBool(byte[] response)
        {
            if (response.Length < 9)
                return new OperateResult<bool>(StringResources.Language.PanasonicReceiveLengthMustLargerThan9);
            if (response[3] == (byte)36)
                return OperateResult.CreateSuccessResult<bool>(response[6] == (byte)49);
            if (response[3] != (byte)33)
                return new OperateResult<bool>(StringResources.Language.UnknownError);
            int err = int.Parse(Encoding.ASCII.GetString(response, 4, 2));
            return new OperateResult<bool>(err, PanasonicHelper.GetErrorDescription(err));
        }

        /// <summary>
        /// 根据错误码获取到错误描述文本<br />
        /// Get the error description text according to the error code
        /// </summary>
        /// <param name="err">错误代码</param>
        /// <returns>字符信息</returns>
        public static string GetErrorDescription(int err)
        {
            switch (err)
            {
                case 20:
                    return StringResources.Language.PanasonicMewStatus20;
                case 21:
                    return StringResources.Language.PanasonicMewStatus21;
                case 22:
                    return StringResources.Language.PanasonicMewStatus22;
                case 23:
                    return StringResources.Language.PanasonicMewStatus23;
                case 24:
                    return StringResources.Language.PanasonicMewStatus24;
                case 25:
                    return StringResources.Language.PanasonicMewStatus25;
                case 26:
                    return StringResources.Language.PanasonicMewStatus26;
                case 27:
                    return StringResources.Language.PanasonicMewStatus27;
                case 28:
                    return StringResources.Language.PanasonicMewStatus28;
                case 29:
                    return StringResources.Language.PanasonicMewStatus29;
                case 30:
                    return StringResources.Language.PanasonicMewStatus30;
                case 40:
                    return StringResources.Language.PanasonicMewStatus40;
                case 41:
                    return StringResources.Language.PanasonicMewStatus41;
                case 42:
                    return StringResources.Language.PanasonicMewStatus42;
                case 43:
                    return StringResources.Language.PanasonicMewStatus43;
                case 50:
                    return StringResources.Language.PanasonicMewStatus50;
                case 51:
                    return StringResources.Language.PanasonicMewStatus51;
                case 52:
                    return StringResources.Language.PanasonicMewStatus52;
                case 53:
                    return StringResources.Language.PanasonicMewStatus53;
                case 60:
                    return StringResources.Language.PanasonicMewStatus60;
                case 61:
                    return StringResources.Language.PanasonicMewStatus61;
                case 62:
                    return StringResources.Language.PanasonicMewStatus62;
                case 63:
                    return StringResources.Language.PanasonicMewStatus63;
                case 65:
                    return StringResources.Language.PanasonicMewStatus65;
                case 66:
                    return StringResources.Language.PanasonicMewStatus66;
                case 67:
                    return StringResources.Language.PanasonicMewStatus67;
                default:
                    return StringResources.Language.UnknownError;
            }
        }
    }
}
