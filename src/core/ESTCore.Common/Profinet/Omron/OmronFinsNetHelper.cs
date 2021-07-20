// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Omron.OmronFinsNetHelper
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;
using ESTCore.Common.Core.Address;

using System;
using System.Collections.Generic;

namespace ESTCore.Common.Profinet.Omron
{
    /// <summary>
    /// Omron PLC的FINS协议相关的辅助类，主要是一些地址解析，读写的指令生成。<br />
    /// The auxiliary classes related to the FINS protocol of Omron PLC are mainly some address resolution and the generation of read and write instructions.
    /// </summary>
    public class OmronFinsNetHelper
    {
        /// <summary>
        /// 根据读取的地址，长度，是否位读取创建Fins协议的核心报文<br />
        /// According to the read address, length, whether to read the core message that creates the Fins protocol
        /// </summary>
        /// <param name="address">地址，具体格式请参照示例说明</param>
        /// <param name="length">读取的数据长度</param>
        /// <param name="isBit">是否使用位读取</param>
        /// <param name="splitLength">读取的长度切割，默认500</param>
        /// <returns>带有成功标识的Fins核心报文</returns>
        public static OperateResult<List<byte[]>> BuildReadCommand(
          string address,
          ushort length,
          bool isBit,
          int splitLength = 500)
        {
            OperateResult<OmronFinsAddress> from = OmronFinsAddress.ParseFrom(address, length);
            if (!from.IsSuccess)
                return OperateResult.CreateFailedResult<List<byte[]>>((OperateResult)from);
            List<byte[]> numArrayList = new List<byte[]>();
            int[] array = SoftBasic.SplitIntegerToArray((int)length, isBit ? int.MaxValue : splitLength);
            for (int index = 0; index < array.Length; ++index)
            {
                byte[] numArray = new byte[8]
                {
          (byte) 1,
          (byte) 1,
          !isBit ? from.Content.WordCode : from.Content.BitCode,
          (byte) (from.Content.AddressStart / 16 / 256),
          (byte) (from.Content.AddressStart / 16 % 256),
          (byte) (from.Content.AddressStart % 16),
          (byte) (array[index] / 256),
          (byte) (array[index] % 256)
                };
                numArrayList.Add(numArray);
                from.Content.AddressStart += isBit ? array[index] : array[index] * 16;
            }
            return OperateResult.CreateSuccessResult<List<byte[]>>(numArrayList);
        }

        /// <summary>
        /// 根据写入的地址，数据，是否位写入生成Fins协议的核心报文<br />
        /// According to the written address, data, whether the bit is written to generate the core message of the Fins protocol
        /// </summary>
        /// <param name="address">地址内容，具体格式请参照示例说明</param>
        /// <param name="value">实际的数据</param>
        /// <param name="isBit">是否位数据</param>
        /// <returns>带有成功标识的Fins核心报文</returns>
        public static OperateResult<byte[]> BuildWriteWordCommand(
          string address,
          byte[] value,
          bool isBit)
        {
            OperateResult<OmronFinsAddress> from = OmronFinsAddress.ParseFrom(address, (ushort)0);
            if (!from.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)from);
            byte[] numArray = new byte[8 + value.Length];
            numArray[0] = (byte)1;
            numArray[1] = (byte)2;
            numArray[2] = !isBit ? from.Content.WordCode : from.Content.BitCode;
            numArray[3] = (byte)(from.Content.AddressStart / 16 / 256);
            numArray[4] = (byte)(from.Content.AddressStart / 16 % 256);
            numArray[5] = (byte)(from.Content.AddressStart % 16);
            if (isBit)
            {
                numArray[6] = (byte)(value.Length / 256);
                numArray[7] = (byte)(value.Length % 256);
            }
            else
            {
                numArray[6] = (byte)(value.Length / 2 / 256);
                numArray[7] = (byte)(value.Length / 2 % 256);
            }
            value.CopyTo((Array)numArray, 8);
            return OperateResult.CreateSuccessResult<byte[]>(numArray);
        }

        /// <summary>
        /// 验证欧姆龙的Fins-TCP返回的数据是否正确的数据，如果正确的话，并返回所有的数据内容<br />
        /// Verify that the data returned by Omron's Fins-TCP is correct data, if correct, and return all data content
        /// </summary>
        /// <param name="response">来自欧姆龙返回的数据内容</param>
        /// <param name="isRead">是否读取</param>
        /// <returns>带有是否成功的结果对象</returns>
        public static OperateResult<byte[]> ResponseValidAnalysis(
          byte[] response,
          bool isRead)
        {
            if (response.Length < 16)
                return new OperateResult<byte[]>(StringResources.Language.OmronReceiveDataError);
            int int32 = BitConverter.ToInt32(new byte[4]
            {
        response[15],
        response[14],
        response[13],
        response[12]
            }, 0);
            return int32 > 0 ? new OperateResult<byte[]>(int32, OmronFinsNetHelper.GetStatusDescription(int32)) : OmronFinsNetHelper.UdpResponseValidAnalysis(response.RemoveBegin<byte>(16), isRead);
        }

        /// <summary>
        /// 验证欧姆龙的Fins-Udp返回的数据是否正确的数据，如果正确的话，并返回所有的数据内容<br />
        /// Verify that the data returned by Omron's Fins-Udp is correct data, if correct, and return all data content
        /// </summary>
        /// <param name="response">来自欧姆龙返回的数据内容</param>
        /// <param name="isRead">是否读取</param>
        /// <returns>带有是否成功的结果对象</returns>
        public static OperateResult<byte[]> UdpResponseValidAnalysis(
          byte[] response,
          bool isRead)
        {
            if (response.Length < 14)
                return new OperateResult<byte[]>(StringResources.Language.OmronReceiveDataError);
            int err = (int)response[12] * 256 + (int)response[13];
            if (!isRead)
            {
                OperateResult<byte[]> successResult = OperateResult.CreateSuccessResult<byte[]>(new byte[0]);
                successResult.ErrorCode = err;
                successResult.Message = OmronFinsNetHelper.GetStatusDescription(err) + " Received:" + SoftBasic.ByteToHexString(response, ' ');
                return successResult;
            }
            byte[] numArray = new byte[response.Length - 14];
            if ((uint)numArray.Length > 0U)
                Array.Copy((Array)response, 14, (Array)numArray, 0, numArray.Length);
            OperateResult<byte[]> successResult1 = OperateResult.CreateSuccessResult<byte[]>(numArray);
            if (numArray.Length == 0)
                successResult1.IsSuccess = false;
            successResult1.ErrorCode = err;
            successResult1.Message = OmronFinsNetHelper.GetStatusDescription(err) + " Received:" + SoftBasic.ByteToHexString(response, ' ');
            return successResult1;
        }

        /// <summary>
        /// 根据欧姆龙返回的错误码，获取错误信息的字符串描述文本<br />
        /// According to the error code returned by Omron, get the string description text of the error message
        /// </summary>
        /// <param name="err">错误码</param>
        /// <returns>文本描述</returns>
        public static string GetStatusDescription(int err)
        {
            switch (err)
            {
                case 0:
                    return StringResources.Language.OmronStatus0;
                case 1:
                    return StringResources.Language.OmronStatus1;
                case 2:
                    return StringResources.Language.OmronStatus2;
                case 3:
                    return StringResources.Language.OmronStatus3;
                case 32:
                    return StringResources.Language.OmronStatus20;
                case 33:
                    return StringResources.Language.OmronStatus21;
                case 34:
                    return StringResources.Language.OmronStatus22;
                case 35:
                    return StringResources.Language.OmronStatus23;
                case 36:
                    return StringResources.Language.OmronStatus24;
                case 37:
                    return StringResources.Language.OmronStatus25;
                default:
                    return StringResources.Language.UnknownError;
            }
        }
    }
}
