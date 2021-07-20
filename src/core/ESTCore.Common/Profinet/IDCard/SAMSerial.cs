// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.IDCard.SAMSerial
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;
using ESTCore.Common.Reflection;
using ESTCore.Common.Serial;

using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace ESTCore.Common.Profinet.IDCard
{
    /// <summary>
    /// 基于SAM协议的串口通信类，支持读取身份证的数据信息，详细参见API文档<br />
    /// Network class implemented by Tcp based on the SAM protocol, which supports reading ID card data information,
    /// see API documentation for details
    /// </summary>
    /// <example>
    /// 在使用之前需要实例化当前的对象，然后根据实际的情况填写好串口的信息，否则连接不上去。
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\SAMSerialSample.cs" region="Sample1" title="实例化操作" />
    /// 在实际的读取，我们一般放在后台进行循环扫描的操作，参见下面的代码
    /// <code lang="cs" source="ESTCore.Common_Net45.Test\Documentation\Samples\Profinet\SAMSerialSample.cs" region="Sample2" title="基本的读取操作" />
    /// </example>
    public class SAMSerial : SerialBase
    {
        /// <inheritdoc />
        protected override OperateResult<byte[]> SPReceived(
          SerialPort serialPort,
          bool awaitData)
        {
            List<byte> input = new List<byte>();
            OperateResult<byte[]> operateResult;
            do
            {
                operateResult = base.SPReceived(serialPort, awaitData);
                if (operateResult.IsSuccess)
                    input.AddRange((IEnumerable<byte>)operateResult.Content);
                else
                    goto label_1;
            }
            while (!SAMSerial.CheckADSCommandCompletion(input));
            goto label_3;
        label_1:
            return operateResult;
        label_3:
            return OperateResult.CreateSuccessResult<byte[]>(input.ToArray());
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.IDCard.SAMTcpNet.ReadSafeModuleNumber" />
        [EstMqttApi]
        public OperateResult<string> ReadSafeModuleNumber()
        {
            OperateResult<byte[]> operateResult = this.ReadBase(SAMSerial.PackToSAMCommand(SAMSerial.BuildReadCommand((byte)18, byte.MaxValue, (byte[])null)));
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<string>((OperateResult)operateResult);
            OperateResult result = SAMSerial.CheckADSCommandAndSum(operateResult.Content);
            return !result.IsSuccess ? OperateResult.CreateFailedResult<string>(result) : SAMSerial.ExtractSafeModuleNumber(operateResult.Content);
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.IDCard.SAMTcpNet.CheckSafeModuleStatus" />
        [EstMqttApi]
        public OperateResult CheckSafeModuleStatus()
        {
            OperateResult<byte[]> operateResult = this.ReadBase(SAMSerial.PackToSAMCommand(SAMSerial.BuildReadCommand((byte)18, byte.MaxValue, (byte[])null)));
            if (!operateResult.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<string>((OperateResult)operateResult);
            OperateResult result = SAMSerial.CheckADSCommandAndSum(operateResult.Content);
            if (!result.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<string>(result);
            return operateResult.Content[9] != (byte)144 ? new OperateResult(SAMSerial.GetErrorDescription((int)operateResult.Content[9])) : OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.IDCard.SAMTcpNet.SearchCard" />
        [EstMqttApi]
        public OperateResult SearchCard()
        {
            OperateResult<byte[]> operateResult = this.ReadBase(SAMSerial.PackToSAMCommand(SAMSerial.BuildReadCommand((byte)32, (byte)1, (byte[])null)));
            if (!operateResult.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<string>((OperateResult)operateResult);
            OperateResult result = SAMSerial.CheckADSCommandAndSum(operateResult.Content);
            if (!result.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<string>(result);
            return operateResult.Content[9] != (byte)159 ? new OperateResult(SAMSerial.GetErrorDescription((int)operateResult.Content[9])) : OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.IDCard.SAMTcpNet.SelectCard" />
        [EstMqttApi]
        public OperateResult SelectCard()
        {
            OperateResult<byte[]> operateResult = this.ReadBase(SAMSerial.PackToSAMCommand(SAMSerial.BuildReadCommand((byte)32, (byte)2, (byte[])null)));
            if (!operateResult.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<string>((OperateResult)operateResult);
            OperateResult result = SAMSerial.CheckADSCommandAndSum(operateResult.Content);
            if (!result.IsSuccess)
                return (OperateResult)OperateResult.CreateFailedResult<string>(result);
            return operateResult.Content[9] != (byte)144 ? new OperateResult(SAMSerial.GetErrorDescription((int)operateResult.Content[9])) : OperateResult.CreateSuccessResult();
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.IDCard.SAMTcpNet.ReadCard" />
        [EstMqttApi]
        public OperateResult<IdentityCard> ReadCard()
        {
            OperateResult<byte[]> operateResult = this.ReadBase(SAMSerial.PackToSAMCommand(SAMSerial.BuildReadCommand((byte)48, (byte)1, (byte[])null)));
            if (!operateResult.IsSuccess)
                return OperateResult.CreateFailedResult<IdentityCard>((OperateResult)operateResult);
            OperateResult result = SAMSerial.CheckADSCommandAndSum(operateResult.Content);
            return !result.IsSuccess ? OperateResult.CreateFailedResult<IdentityCard>(result) : SAMSerial.ExtractIdentityCard(operateResult.Content);
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("SAMSerial[{0}:{1}]", (object)this.PortName, (object)this.BaudRate);

        /// <summary>将指令进行打包成可以发送的数据对象</summary>
        /// <param name="command">命令信息</param>
        /// <returns>字节数组</returns>
        public static byte[] PackToSAMCommand(byte[] command)
        {
            byte[] numArray = new byte[command.Length + 8];
            numArray[0] = (byte)170;
            numArray[1] = (byte)170;
            numArray[2] = (byte)170;
            numArray[3] = (byte)150;
            numArray[4] = (byte)105;
            numArray[5] = BitConverter.GetBytes(numArray.Length - 7)[1];
            numArray[6] = BitConverter.GetBytes(numArray.Length - 7)[0];
            command.CopyTo((Array)numArray, 7);
            int num = 0;
            for (int index = 5; index < numArray.Length - 1; ++index)
                num ^= (int)numArray[index];
            numArray[numArray.Length - 1] = (byte)num;
            return numArray;
        }

        /// <summary>根据SAM的实际的指令，来生成实际的指令信息</summary>
        /// <param name="cmd">命令码</param>
        /// <param name="para">参数信息</param>
        /// <param name="data">数据内容</param>
        /// <returns>字符串的结果信息</returns>
        public static byte[] BuildReadCommand(byte cmd, byte para, byte[] data)
        {
            if (data == null)
                data = new byte[0];
            byte[] numArray = new byte[2 + data.Length];
            numArray[0] = cmd;
            numArray[1] = para;
            data.CopyTo((Array)numArray, 2);
            return numArray;
        }

        /// <summary>检查当前的接收数据信息是否一条完整的数据信息</summary>
        /// <param name="input">输入的信息</param>
        /// <returns>是否接收完成</returns>
        public static bool CheckADSCommandCompletion(List<byte> input) => (input == null || (input.Count) >= 8) && (int)input[5] * 256 + (int)input[6] <= input.Count - 7;

        /// <summary>检查当前的指令是否是正确的</summary>
        /// <param name="input">输入的指令信息</param>
        /// <returns>是否校验成功</returns>
        public static OperateResult CheckADSCommandAndSum(byte[] input)
        {
            if (input != null && input.Length < 8)
                return new OperateResult(StringResources.Language.SAMReceiveLengthMustLargerThan8);
            if (input[0] != (byte)170 || input[1] != (byte)170 || (input[2] != (byte)170 || input[3] != (byte)150) || input[4] != (byte)105)
                return new OperateResult(StringResources.Language.SAMHeadCheckFailed);
            if ((int)input[5] * 256 + (int)input[6] != input.Length - 7)
                return new OperateResult(StringResources.Language.SAMLengthCheckFailed);
            int num = 0;
            for (int index = 5; index < input.Length - 1; ++index)
                num ^= (int)input[index];
            return num != (int)input[input.Length - 1] ? new OperateResult(StringResources.Language.SAMSumCheckFailed) : OperateResult.CreateSuccessResult();
        }

        /// <summary>提炼安全的模块数据信息</summary>
        /// <param name="data">数据</param>
        /// <returns>结果对象</returns>
        public static OperateResult<string> ExtractSafeModuleNumber(byte[] data)
        {
            try
            {
                if (data[9] != (byte)144)
                    return new OperateResult<string>(SAMSerial.GetErrorDescription((int)data[9]));
                StringBuilder stringBuilder1 = new StringBuilder();
                stringBuilder1.Append(data[10].ToString("D2"));
                stringBuilder1.Append(".");
                stringBuilder1.Append(data[12].ToString("D2"));
                stringBuilder1.Append("-");
                stringBuilder1.Append(BitConverter.ToInt32(data, 14).ToString());
                stringBuilder1.Append("-");
                StringBuilder stringBuilder2 = stringBuilder1;
                int int32 = BitConverter.ToInt32(data, 18);
                string str1 = int32.ToString("D9");
                stringBuilder2.Append(str1);
                stringBuilder1.Append("-");
                StringBuilder stringBuilder3 = stringBuilder1;
                int32 = BitConverter.ToInt32(data, 22);
                string str2 = int32.ToString("D9");
                stringBuilder3.Append(str2);
                return OperateResult.CreateSuccessResult<string>(stringBuilder1.ToString());
            }
            catch (Exception ex)
            {
                return new OperateResult<string>("Error:" + ex.Message + "  Source Data: " + SoftBasic.ByteToHexString(data));
            }
        }

        /// <summary>从数据中提取出真实的身份证信息</summary>
        /// <param name="data">原始数据内容</param>
        /// <returns>包含结果对象的身份证数据</returns>
        public static OperateResult<IdentityCard> ExtractIdentityCard(
          byte[] data)
        {
            try
            {
                if (data[9] != (byte)144)
                    return new OperateResult<IdentityCard>(SAMSerial.GetErrorDescription((int)data[9]));
                string str = Encoding.Unicode.GetString(data, 14, 256);
                byte[] numArray = SoftBasic.ArraySelectMiddle<byte>(data, 270, 1024);
                return OperateResult.CreateSuccessResult<IdentityCard>(new IdentityCard()
                {
                    Name = str.Substring(0, 15),
                    Sex = str.Substring(15, 1) == "1" ? "男" : (str.Substring(15, 1) == "2" ? "女" : "未知"),
                    Nation = SAMSerial.GetNationText(Convert.ToInt32(str.Substring(16, 2))),
                    Birthday = new DateTime(int.Parse(str.Substring(18, 4)), int.Parse(str.Substring(22, 2)), int.Parse(str.Substring(24, 2))),
                    Address = str.Substring(26, 35),
                    Id = str.Substring(61, 18),
                    Organ = str.Substring(79, 15),
                    ValidityStartDate = new DateTime(int.Parse(str.Substring(94, 4)), int.Parse(str.Substring(98, 2)), int.Parse(str.Substring(100, 2))),
                    ValidityEndDate = new DateTime(int.Parse(str.Substring(102, 4)), int.Parse(str.Substring(106, 2)), int.Parse(str.Substring(108, 2))),
                    Portrait = numArray
                });
            }
            catch (Exception ex)
            {
                return new OperateResult<IdentityCard>(ex.Message);
            }
        }

        /// <summary>根据民族的代号来获取到民族的文本描述信息</summary>
        /// <param name="nation">民族代码</param>
        /// <returns>民族的文本信息</returns>
        public static string GetNationText(int nation)
        {
            switch (nation)
            {
                case 1:
                    return "汉";
                case 2:
                    return "蒙古";
                case 3:
                    return "回";
                case 4:
                    return "藏";
                case 5:
                    return "维吾尔";
                case 6:
                    return "苗";
                case 7:
                    return "彝";
                case 8:
                    return "壮";
                case 9:
                    return "布依";
                case 10:
                    return "朝鲜";
                case 11:
                    return "满";
                case 12:
                    return "侗";
                case 13:
                    return "瑶";
                case 14:
                    return "白";
                case 15:
                    return "土家";
                case 16:
                    return "哈尼";
                case 17:
                    return "哈萨克";
                case 18:
                    return "傣";
                case 19:
                    return "黎";
                case 20:
                    return "傈僳";
                case 21:
                    return "佤";
                case 22:
                    return "畲";
                case 23:
                    return "高山";
                case 24:
                    return "拉祜";
                case 25:
                    return "水";
                case 26:
                    return "东乡";
                case 27:
                    return "纳西";
                case 28:
                    return "景颇";
                case 29:
                    return "柯尔克孜";
                case 30:
                    return "土";
                case 31:
                    return "达斡尔";
                case 32:
                    return "仫佬";
                case 33:
                    return "羌";
                case 34:
                    return "布朗";
                case 35:
                    return "撒拉";
                case 36:
                    return "毛南";
                case 37:
                    return "仡佬";
                case 38:
                    return "锡伯";
                case 39:
                    return "阿昌";
                case 40:
                    return "普米";
                case 41:
                    return "塔吉克";
                case 42:
                    return "怒";
                case 43:
                    return "乌孜别克";
                case 44:
                    return "俄罗斯";
                case 45:
                    return "鄂温克";
                case 46:
                    return "德昂";
                case 47:
                    return "保安";
                case 48:
                    return "裕固";
                case 49:
                    return "京";
                case 50:
                    return "塔塔尔";
                case 51:
                    return "独龙";
                case 52:
                    return "鄂伦春";
                case 53:
                    return "赫哲";
                case 54:
                    return "门巴";
                case 55:
                    return "珞巴";
                case 56:
                    return "基诺";
                case 97:
                    return "其他";
                case 98:
                    return "外国血统中国籍人士";
                default:
                    return string.Empty;
            }
        }

        /// <summary>枚举当前的所有的民族信息，共计五十六个民族</summary>
        /// <returns>枚举信息</returns>
        public static IEnumerator<string> GetNationEnumerator()
        {
            for (int i = 1; i < 57; ++i)
                yield return SAMSerial.GetNationText(i);
        }

        /// <summary>获取错误的文本信息</summary>
        /// <param name="err">错误号</param>
        /// <returns>错误信息</returns>
        public static string GetErrorDescription(int err)
        {
            switch (err)
            {
                case 16:
                    return StringResources.Language.SAMStatus10;
                case 17:
                    return StringResources.Language.SAMStatus11;
                case 33:
                    return StringResources.Language.SAMStatus21;
                case 35:
                    return StringResources.Language.SAMStatus23;
                case 36:
                    return StringResources.Language.SAMStatus24;
                case 49:
                    return StringResources.Language.SAMStatus31;
                case 50:
                    return StringResources.Language.SAMStatus32;
                case 51:
                    return StringResources.Language.SAMStatus33;
                case 64:
                    return StringResources.Language.SAMStatus40;
                case 65:
                    return StringResources.Language.SAMStatus41;
                case 71:
                    return StringResources.Language.SAMStatus47;
                case 96:
                    return StringResources.Language.SAMStatus60;
                case 102:
                    return StringResources.Language.SAMStatus66;
                case 128:
                    return StringResources.Language.SAMStatus80;
                case 129:
                    return StringResources.Language.SAMStatus81;
                case 145:
                    return StringResources.Language.SAMStatus91;
                default:
                    return StringResources.Language.UnknownError;
            }
        }
    }
}
