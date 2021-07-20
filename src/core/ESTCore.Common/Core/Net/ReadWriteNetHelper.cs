// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Core.Net.ReadWriteNetHelper
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace ESTCore.Common.Core.Net
{
    /// <summary>读写网络的辅助类</summary>
    public class ReadWriteNetHelper
    {
        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Wait(System.String,System.Boolean,System.Int32,System.Int32)" />
        /// <param name="readWriteNet">通信对象</param>
        public static OperateResult<TimeSpan> Wait(
          IReadWriteNet readWriteNet,
          string address,
          bool waitValue,
          int readInterval,
          int waitTimeout)
        {
            DateTime now = DateTime.Now;
            OperateResult<bool> operateResult;
            while (true)
            {
                operateResult = readWriteNet.ReadBool(address);
                if (operateResult.IsSuccess)
                {
                    if (operateResult.Content != waitValue)
                    {
                        if (waitTimeout <= 0 || (DateTime.Now - now).TotalMilliseconds <= (double)waitTimeout)
                            Thread.Sleep(readInterval);
                        else
                            goto label_5;
                    }
                    else
                        goto label_3;
                }
                else
                    break;
            }
            return OperateResult.CreateFailedResult<TimeSpan>((OperateResult)operateResult);
        label_3:
            return OperateResult.CreateSuccessResult<TimeSpan>(DateTime.Now - now);
        label_5:
            return new OperateResult<TimeSpan>(StringResources.Language.CheckDataTimeout + waitTimeout.ToString());
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Wait(System.String,System.Int16,System.Int32,System.Int32)" />
        /// <param name="readWriteNet">通信对象</param>
        public static OperateResult<TimeSpan> Wait(
          IReadWriteNet readWriteNet,
          string address,
          short waitValue,
          int readInterval,
          int waitTimeout)
        {
            DateTime now = DateTime.Now;
            OperateResult<short> operateResult;
            while (true)
            {
                operateResult = readWriteNet.ReadInt16(address);
                if (operateResult.IsSuccess)
                {
                    if ((int)operateResult.Content != (int)waitValue)
                    {
                        if (waitTimeout <= 0 || (DateTime.Now - now).TotalMilliseconds <= (double)waitTimeout)
                            Thread.Sleep(readInterval);
                        else
                            goto label_5;
                    }
                    else
                        goto label_3;
                }
                else
                    break;
            }
            return OperateResult.CreateFailedResult<TimeSpan>((OperateResult)operateResult);
        label_3:
            return OperateResult.CreateSuccessResult<TimeSpan>(DateTime.Now - now);
        label_5:
            return new OperateResult<TimeSpan>(StringResources.Language.CheckDataTimeout + waitTimeout.ToString());
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Wait(System.String,System.UInt16,System.Int32,System.Int32)" />
        /// <param name="readWriteNet">通信对象</param>
        public static OperateResult<TimeSpan> Wait(
          IReadWriteNet readWriteNet,
          string address,
          ushort waitValue,
          int readInterval,
          int waitTimeout)
        {
            DateTime now = DateTime.Now;
            OperateResult<ushort> operateResult;
            while (true)
            {
                operateResult = readWriteNet.ReadUInt16(address);
                if (operateResult.IsSuccess)
                {
                    if ((int)operateResult.Content != (int)waitValue)
                    {
                        if (waitTimeout <= 0 || (DateTime.Now - now).TotalMilliseconds <= (double)waitTimeout)
                            Thread.Sleep(readInterval);
                        else
                            goto label_5;
                    }
                    else
                        goto label_3;
                }
                else
                    break;
            }
            return OperateResult.CreateFailedResult<TimeSpan>((OperateResult)operateResult);
        label_3:
            return OperateResult.CreateSuccessResult<TimeSpan>(DateTime.Now - now);
        label_5:
            return new OperateResult<TimeSpan>(StringResources.Language.CheckDataTimeout + waitTimeout.ToString());
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Wait(System.String,System.Int32,System.Int32,System.Int32)" />
        /// <param name="readWriteNet">通信对象</param>
        public static OperateResult<TimeSpan> Wait(
          IReadWriteNet readWriteNet,
          string address,
          int waitValue,
          int readInterval,
          int waitTimeout)
        {
            DateTime now = DateTime.Now;
            OperateResult<int> operateResult;
            while (true)
            {
                operateResult = readWriteNet.ReadInt32(address);
                if (operateResult.IsSuccess)
                {
                    if (operateResult.Content != waitValue)
                    {
                        if (waitTimeout <= 0 || (DateTime.Now - now).TotalMilliseconds <= (double)waitTimeout)
                            Thread.Sleep(readInterval);
                        else
                            goto label_5;
                    }
                    else
                        goto label_3;
                }
                else
                    break;
            }
            return OperateResult.CreateFailedResult<TimeSpan>((OperateResult)operateResult);
        label_3:
            return OperateResult.CreateSuccessResult<TimeSpan>(DateTime.Now - now);
        label_5:
            return new OperateResult<TimeSpan>(StringResources.Language.CheckDataTimeout + waitTimeout.ToString());
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Wait(System.String,System.UInt32,System.Int32,System.Int32)" />
        /// <param name="readWriteNet">通信对象</param>
        public static OperateResult<TimeSpan> Wait(
          IReadWriteNet readWriteNet,
          string address,
          uint waitValue,
          int readInterval,
          int waitTimeout)
        {
            DateTime now = DateTime.Now;
            OperateResult<uint> operateResult;
            while (true)
            {
                operateResult = readWriteNet.ReadUInt32(address);
                if (operateResult.IsSuccess)
                {
                    if ((int)operateResult.Content != (int)waitValue)
                    {
                        if (waitTimeout <= 0 || (DateTime.Now - now).TotalMilliseconds <= (double)waitTimeout)
                            Thread.Sleep(readInterval);
                        else
                            goto label_5;
                    }
                    else
                        goto label_3;
                }
                else
                    break;
            }
            return OperateResult.CreateFailedResult<TimeSpan>((OperateResult)operateResult);
        label_3:
            return OperateResult.CreateSuccessResult<TimeSpan>(DateTime.Now - now);
        label_5:
            return new OperateResult<TimeSpan>(StringResources.Language.CheckDataTimeout + waitTimeout.ToString());
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Wait(System.String,System.Int64,System.Int32,System.Int32)" />
        /// <param name="readWriteNet">通信对象</param>
        public static OperateResult<TimeSpan> Wait(
          IReadWriteNet readWriteNet,
          string address,
          long waitValue,
          int readInterval,
          int waitTimeout)
        {
            DateTime now = DateTime.Now;
            OperateResult<long> operateResult;
            while (true)
            {
                operateResult = readWriteNet.ReadInt64(address);
                if (operateResult.IsSuccess)
                {
                    if (operateResult.Content != waitValue)
                    {
                        if (waitTimeout <= 0 || (DateTime.Now - now).TotalMilliseconds <= (double)waitTimeout)
                            Thread.Sleep(readInterval);
                        else
                            goto label_5;
                    }
                    else
                        goto label_3;
                }
                else
                    break;
            }
            return OperateResult.CreateFailedResult<TimeSpan>((OperateResult)operateResult);
        label_3:
            return OperateResult.CreateSuccessResult<TimeSpan>(DateTime.Now - now);
        label_5:
            return new OperateResult<TimeSpan>(StringResources.Language.CheckDataTimeout + waitTimeout.ToString());
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Wait(System.String,System.UInt64,System.Int32,System.Int32)" />
        /// <param name="readWriteNet">通信对象</param>
        public static OperateResult<TimeSpan> Wait(
          IReadWriteNet readWriteNet,
          string address,
          ulong waitValue,
          int readInterval,
          int waitTimeout)
        {
            DateTime now = DateTime.Now;
            OperateResult<ulong> operateResult;
            while (true)
            {
                operateResult = readWriteNet.ReadUInt64(address);
                if (operateResult.IsSuccess)
                {
                    if ((long)operateResult.Content != (long)waitValue)
                    {
                        if (waitTimeout <= 0 || (DateTime.Now - now).TotalMilliseconds <= (double)waitTimeout)
                            Thread.Sleep(readInterval);
                        else
                            goto label_5;
                    }
                    else
                        goto label_3;
                }
                else
                    break;
            }
            return OperateResult.CreateFailedResult<TimeSpan>((OperateResult)operateResult);
        label_3:
            return OperateResult.CreateSuccessResult<TimeSpan>(DateTime.Now - now);
        label_5:
            return new OperateResult<TimeSpan>(StringResources.Language.CheckDataTimeout + waitTimeout.ToString());
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Wait(System.String,System.Boolean,System.Int32,System.Int32)" />
        /// <param name="readWriteNet">通信对象</param>
        public static async Task<OperateResult<TimeSpan>> WaitAsync(
          IReadWriteNet readWriteNet,
          string address,
          bool waitValue,
          int readInterval,
          int waitTimeout)
        {
            DateTime start = DateTime.Now;
            OperateResult<bool> read;
            while (true)
            {
                read = await readWriteNet.ReadBoolAsync(address);
                if (read.IsSuccess)
                {
                    if (read.Content != waitValue)
                    {
                        if (waitTimeout <= 0 || (DateTime.Now - start).TotalMilliseconds <= (double)waitTimeout)
                        {
                            await Task.Delay(readInterval);
                            read = (OperateResult<bool>)null;
                        }
                        else
                            goto label_6;
                    }
                    else
                        goto label_4;
                }
                else
                    break;
            }
            return OperateResult.CreateFailedResult<TimeSpan>((OperateResult)read);
        label_4:
            return OperateResult.CreateSuccessResult<TimeSpan>(DateTime.Now - start);
        label_6:
            return new OperateResult<TimeSpan>(StringResources.Language.CheckDataTimeout + waitTimeout.ToString());
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Wait(System.String,System.Int16,System.Int32,System.Int32)" />
        /// <param name="readWriteNet">通信对象</param>
        public static async Task<OperateResult<TimeSpan>> WaitAsync(
          IReadWriteNet readWriteNet,
          string address,
          short waitValue,
          int readInterval,
          int waitTimeout)
        {
            DateTime start = DateTime.Now;
            OperateResult<short> read;
            while (true)
            {
                read = await readWriteNet.ReadInt16Async(address);
                if (read.IsSuccess)
                {
                    if ((int)read.Content != (int)waitValue)
                    {
                        if (waitTimeout <= 0 || (DateTime.Now - start).TotalMilliseconds <= (double)waitTimeout)
                        {
                            await Task.Delay(readInterval);
                            read = (OperateResult<short>)null;
                        }
                        else
                            goto label_6;
                    }
                    else
                        goto label_4;
                }
                else
                    break;
            }
            return OperateResult.CreateFailedResult<TimeSpan>((OperateResult)read);
        label_4:
            return OperateResult.CreateSuccessResult<TimeSpan>(DateTime.Now - start);
        label_6:
            return new OperateResult<TimeSpan>(StringResources.Language.CheckDataTimeout + waitTimeout.ToString());
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Wait(System.String,System.UInt16,System.Int32,System.Int32)" />
        /// <param name="readWriteNet">通信对象</param>
        public static async Task<OperateResult<TimeSpan>> WaitAsync(
          IReadWriteNet readWriteNet,
          string address,
          ushort waitValue,
          int readInterval,
          int waitTimeout)
        {
            DateTime start = DateTime.Now;
            OperateResult<ushort> read;
            while (true)
            {
                read = await readWriteNet.ReadUInt16Async(address);
                if (read.IsSuccess)
                {
                    if ((int)read.Content != (int)waitValue)
                    {
                        if (waitTimeout <= 0 || (DateTime.Now - start).TotalMilliseconds <= (double)waitTimeout)
                        {
                            await Task.Delay(readInterval);
                            read = (OperateResult<ushort>)null;
                        }
                        else
                            goto label_6;
                    }
                    else
                        goto label_4;
                }
                else
                    break;
            }
            return OperateResult.CreateFailedResult<TimeSpan>((OperateResult)read);
        label_4:
            return OperateResult.CreateSuccessResult<TimeSpan>(DateTime.Now - start);
        label_6:
            return new OperateResult<TimeSpan>(StringResources.Language.CheckDataTimeout + waitTimeout.ToString());
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Wait(System.String,System.Int32,System.Int32,System.Int32)" />
        /// <param name="readWriteNet">通信对象</param>
        public static async Task<OperateResult<TimeSpan>> WaitAsync(
          IReadWriteNet readWriteNet,
          string address,
          int waitValue,
          int readInterval,
          int waitTimeout)
        {
            DateTime start = DateTime.Now;
            OperateResult<int> read;
            while (true)
            {
                read = await readWriteNet.ReadInt32Async(address);
                if (read.IsSuccess)
                {
                    if (read.Content != waitValue)
                    {
                        if (waitTimeout <= 0 || (DateTime.Now - start).TotalMilliseconds <= (double)waitTimeout)
                        {
                            await Task.Delay(readInterval);
                            read = (OperateResult<int>)null;
                        }
                        else
                            goto label_6;
                    }
                    else
                        goto label_4;
                }
                else
                    break;
            }
            return OperateResult.CreateFailedResult<TimeSpan>((OperateResult)read);
        label_4:
            return OperateResult.CreateSuccessResult<TimeSpan>(DateTime.Now - start);
        label_6:
            return new OperateResult<TimeSpan>(StringResources.Language.CheckDataTimeout + waitTimeout.ToString());
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Wait(System.String,System.UInt32,System.Int32,System.Int32)" />
        /// <param name="readWriteNet">通信对象</param>
        public static async Task<OperateResult<TimeSpan>> WaitAsync(
          IReadWriteNet readWriteNet,
          string address,
          uint waitValue,
          int readInterval,
          int waitTimeout)
        {
            DateTime start = DateTime.Now;
            OperateResult<uint> read;
            while (true)
            {
                read = readWriteNet.ReadUInt32(address);
                if (read.IsSuccess)
                {
                    if ((int)read.Content != (int)waitValue)
                    {
                        if (waitTimeout <= 0 || (DateTime.Now - start).TotalMilliseconds <= (double)waitTimeout)
                        {
                            await Task.Delay(readInterval);
                            read = (OperateResult<uint>)null;
                        }
                        else
                            goto label_5;
                    }
                    else
                        goto label_3;
                }
                else
                    break;
            }
            return OperateResult.CreateFailedResult<TimeSpan>((OperateResult)read);
        label_3:
            return OperateResult.CreateSuccessResult<TimeSpan>(DateTime.Now - start);
        label_5:
            return new OperateResult<TimeSpan>(StringResources.Language.CheckDataTimeout + waitTimeout.ToString());
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Wait(System.String,System.Int64,System.Int32,System.Int32)" />
        /// <param name="readWriteNet">通信对象</param>
        public static async Task<OperateResult<TimeSpan>> WaitAsync(
          IReadWriteNet readWriteNet,
          string address,
          long waitValue,
          int readInterval,
          int waitTimeout)
        {
            DateTime start = DateTime.Now;
            OperateResult<long> read;
            while (true)
            {
                read = readWriteNet.ReadInt64(address);
                if (read.IsSuccess)
                {
                    if (read.Content != waitValue)
                    {
                        if (waitTimeout <= 0 || (DateTime.Now - start).TotalMilliseconds <= (double)waitTimeout)
                        {
                            await Task.Delay(readInterval);
                            read = (OperateResult<long>)null;
                        }
                        else
                            goto label_5;
                    }
                    else
                        goto label_3;
                }
                else
                    break;
            }
            return OperateResult.CreateFailedResult<TimeSpan>((OperateResult)read);
        label_3:
            return OperateResult.CreateSuccessResult<TimeSpan>(DateTime.Now - start);
        label_5:
            return new OperateResult<TimeSpan>(StringResources.Language.CheckDataTimeout + waitTimeout.ToString());
        }

        /// <inheritdoc cref="M:ESTCore.Common.Core.IReadWriteNet.Wait(System.String,System.UInt64,System.Int32,System.Int32)" />
        /// <param name="readWriteNet">通信对象</param>
        public static async Task<OperateResult<TimeSpan>> WaitAsync(
          IReadWriteNet readWriteNet,
          string address,
          ulong waitValue,
          int readInterval,
          int waitTimeout)
        {
            DateTime start = DateTime.Now;
            OperateResult<ulong> read;
            while (true)
            {
                read = readWriteNet.ReadUInt64(address);
                if (read.IsSuccess)
                {
                    if ((long)read.Content != (long)waitValue)
                    {
                        if (waitTimeout <= 0 || (DateTime.Now - start).TotalMilliseconds <= (double)waitTimeout)
                        {
                            await Task.Delay(readInterval);
                            read = (OperateResult<ulong>)null;
                        }
                        else
                            goto label_5;
                    }
                    else
                        goto label_3;
                }
                else
                    break;
            }
            return OperateResult.CreateFailedResult<TimeSpan>((OperateResult)read);
        label_3:
            return OperateResult.CreateSuccessResult<TimeSpan>(DateTime.Now - start);
        label_5:
            return new OperateResult<TimeSpan>(StringResources.Language.CheckDataTimeout + waitTimeout.ToString());
        }
    }
}
