// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Enthernet.Redis.RedisHelper
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using System;
using System.Collections.Generic;
using System.Text;

namespace ESTCore.Common.Enthernet.Redis
{
    /// <summary>提供了redis辅助类的一些方法</summary>
    public class RedisHelper
    {
        /// <summary>将字符串数组打包成一个redis的报文信息</summary>
        /// <param name="commands">字节数据信息</param>
        /// <returns>结果报文信息</returns>
        public static byte[] PackStringCommand(string[] commands)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append('*');
            stringBuilder.Append(commands.Length.ToString());
            stringBuilder.Append("\r\n");
            for (int index = 0; index < commands.Length; ++index)
            {
                stringBuilder.Append('$');
                stringBuilder.Append(Encoding.UTF8.GetBytes(commands[index]).Length.ToString());
                stringBuilder.Append("\r\n");
                stringBuilder.Append(commands[index]);
                stringBuilder.Append("\r\n");
            }
            return Encoding.UTF8.GetBytes(stringBuilder.ToString());
        }

        /// <summary>生成一个订阅多个主题的报文信息</summary>
        /// <param name="topics">多个的主题信息</param>
        /// <returns>结果报文信息</returns>
        public static byte[] PackSubscribeCommand(string[] topics)
        {
            List<string> stringList = new List<string>();
            stringList.Add("SUBSCRIBE");
            stringList.AddRange((IEnumerable<string>)topics);
            return RedisHelper.PackStringCommand(stringList.ToArray());
        }

        /// <summary>生成一个取消订阅多个主题的报文信息</summary>
        /// <param name="topics">多个的主题信息</param>
        /// <returns>结果报文信息</returns>
        public static byte[] PackUnSubscribeCommand(string[] topics)
        {
            List<string> stringList = new List<string>();
            stringList.Add("UNSUBSCRIBE");
            stringList.AddRange((IEnumerable<string>)topics);
            return RedisHelper.PackStringCommand(stringList.ToArray());
        }

        /// <summary>从原始的结果数据对象中提取出数字数据</summary>
        /// <param name="commandLine">原始的字节数据</param>
        /// <returns>带有结果对象的数据信息</returns>
        public static OperateResult<int> GetNumberFromCommandLine(byte[] commandLine)
        {
            try
            {
                return OperateResult.CreateSuccessResult<int>(Convert.ToInt32(Encoding.UTF8.GetString(commandLine).TrimEnd('\r', '\n').Substring(1)));
            }
            catch (Exception ex)
            {
                return new OperateResult<int>(ex.Message);
            }
        }

        /// <summary>从原始的结果数据对象中提取出数字数据</summary>
        /// <param name="commandLine">原始的字节数据</param>
        /// <returns>带有结果对象的数据信息</returns>
        public static OperateResult<long> GetLongNumberFromCommandLine(byte[] commandLine)
        {
            try
            {
                return OperateResult.CreateSuccessResult<long>(Convert.ToInt64(Encoding.UTF8.GetString(commandLine).TrimEnd('\r', '\n').Substring(1)));
            }
            catch (Exception ex)
            {
                return new OperateResult<long>(ex.Message);
            }
        }

        /// <summary>从结果的数据对象里提取字符串的信息</summary>
        /// <param name="commandLine">原始的字节数据</param>
        /// <returns>带有结果对象的数据信息</returns>
        public static OperateResult<string> GetStringFromCommandLine(byte[] commandLine)
        {
            try
            {
                if (commandLine[0] != (byte)36)
                    return new OperateResult<string>(Encoding.UTF8.GetString(commandLine));
                int num1 = -1;
                int num2 = -1;
                for (int index = 0; index < commandLine.Length; ++index)
                {
                    if (commandLine[index] == (byte)13 || commandLine[index] == (byte)10)
                        num1 = index;
                    if (commandLine[index] == (byte)10)
                    {
                        num2 = index;
                        break;
                    }
                }
                int int32 = Convert.ToInt32(Encoding.UTF8.GetString(commandLine, 1, num1 - 1));
                return int32 < 0 ? new OperateResult<string>("(nil) None Value") : OperateResult.CreateSuccessResult<string>(Encoding.UTF8.GetString(commandLine, num2 + 1, int32));
            }
            catch (Exception ex)
            {
                return new OperateResult<string>(ex.Message);
            }
        }

        /// <summary>从redis的结果数据中分析出所有的字符串信息</summary>
        /// <param name="commandLine">结果数据</param>
        /// <returns>带有结果对象的数据信息</returns>
        public static OperateResult<string[]> GetStringsFromCommandLine(byte[] commandLine)
        {
            try
            {
                List<string> stringList = new List<string>();
                if (commandLine[0] != (byte)42)
                    return new OperateResult<string[]>(Encoding.UTF8.GetString(commandLine));
                int index1 = 0;
                for (int index2 = 0; index2 < commandLine.Length; ++index2)
                {
                    if (commandLine[index2] == (byte)13 || commandLine[index2] == (byte)10)
                    {
                        index1 = index2;
                        break;
                    }
                }
                int int32_1 = Convert.ToInt32(Encoding.UTF8.GetString(commandLine, 1, index1 - 1));
                for (int index2 = 0; index2 < int32_1; ++index2)
                {
                    int num1 = -1;
                    for (int index3 = index1; index3 < commandLine.Length; ++index3)
                    {
                        if (commandLine[index3] == (byte)10)
                        {
                            num1 = index3;
                            break;
                        }
                    }
                    index1 = num1 + 1;
                    if (commandLine[index1] == (byte)36)
                    {
                        int num2 = -1;
                        for (int index3 = index1; index3 < commandLine.Length; ++index3)
                        {
                            if (commandLine[index3] == (byte)13 || commandLine[index3] == (byte)10)
                            {
                                num2 = index3;
                                break;
                            }
                        }
                        int int32_2 = Convert.ToInt32(Encoding.UTF8.GetString(commandLine, index1 + 1, num2 - index1 - 1));
                        if (int32_2 >= 0)
                        {
                            for (int index3 = index1; index3 < commandLine.Length; ++index3)
                            {
                                if (commandLine[index3] == (byte)10)
                                {
                                    num1 = index3;
                                    break;
                                }
                            }
                            int index4 = num1 + 1;
                            stringList.Add(Encoding.UTF8.GetString(commandLine, index4, int32_2));
                            index1 = index4 + int32_2;
                        }
                        else
                            stringList.Add((string)null);
                    }
                    else
                    {
                        int num2 = -1;
                        for (int index3 = index1; index3 < commandLine.Length; ++index3)
                        {
                            if (commandLine[index3] == (byte)13 || commandLine[index3] == (byte)10)
                            {
                                num2 = index3;
                                break;
                            }
                        }
                        stringList.Add(Encoding.UTF8.GetString(commandLine, index1, num2 - index1 - 1));
                    }
                }
                return OperateResult.CreateSuccessResult<string[]>(stringList.ToArray());
            }
            catch (Exception ex)
            {
                return new OperateResult<string[]>(ex.Message);
            }
        }
    }
}
