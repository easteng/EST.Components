// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.Profinet.Fuji.FujiSPHNet
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using ESTCore.Common.BasicFramework;
using ESTCore.Common.Core;
using ESTCore.Common.Core.Address;
using ESTCore.Common.Core.IMessage;
using ESTCore.Common.Core.Net;
using ESTCore.Common.Reflection;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESTCore.Common.Profinet.Fuji
{
    /// <summary>
    /// 富士PLC的SPH通信协议，可以和富士PLC进行通信，<see cref="P:ESTCore.Common.Profinet.Fuji.FujiSPHNet.ConnectionID" />默认CPU0，需要根据实际进行调整。
    /// </summary>
    public class FujiSPHNet : NetworkDeviceBase
    {
        /// <summary>
        /// 实例化一个默认的对象<br />
        /// Instantiate a default object
        /// </summary>
        public FujiSPHNet()
        {
            this.ByteTransform = (IByteTransform)new RegularByteTransform();
            this.WordLength = (ushort)1;
        }

        /// <summary>
        /// 指定IP地址和端口号来实例化一个对象<br />
        /// Specify the IP address and port number to instantiate an object
        /// </summary>
        /// <param name="ipAddress">Ip地址</param>
        /// <param name="port">端口号</param>
        public FujiSPHNet(string ipAddress, int port = 18245)
          : this()
        {
            this.IpAddress = ipAddress;
            this.Port = port;
        }

        /// <inheritdoc />
        protected override INetMessage GetNewNetMessage() => (INetMessage)new FujiSPHMessage();

        /// <summary>
        /// 对于 CPU0-CPU7来说是CPU的站号，分为对应 0xFE-0xF7，对于P/PE link, FL-net是模块站号，分别对应0xF6-0xEF<br />
        /// CPU0 to CPU7: SX bus station No. of destination CPU (FEh to F7h); P/PE link, FL-net: SX bus station No. of destination module (F6H to EFH)
        /// </summary>
        public byte ConnectionID { get; set; } = 254;

        private OperateResult<byte[]> ReadFujiSPHAddress(
          FujiSPHAddress address,
          ushort length)
        {
            OperateResult<List<byte[]>> operateResult1 = FujiSPHNet.BuildReadCommand(this.ConnectionID, address, length);
            if (!operateResult1.IsSuccess)
                return operateResult1.ConvertFailed<byte[]>();
            List<byte> byteList = new List<byte>();
            for (int index = 0; index < operateResult1.Content.Count; ++index)
            {
                OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content[index]);
                if (!operateResult2.IsSuccess)
                    return operateResult2;
                OperateResult<byte[]> operateResult3 = FujiSPHNet.ExtraResponseContent(operateResult2.Content);
                if (!operateResult3.IsSuccess)
                    return operateResult3;
                byteList.AddRange((IEnumerable<byte>)operateResult3.Content);
            }
            return OperateResult.CreateSuccessResult<byte[]>(byteList.ToArray());
        }

        /// <summary>
        /// 批量读取PLC的地址数据，长度单位为字。地址支持M1.1000，M3.1000，M10.1000，返回读取的原始字节数组。<br />
        /// Read PLC address data in batches, the length unit is words. The address supports M1.1000, M3.1000, M10.1000,
        /// and returns the original byte array read.
        /// </summary>
        /// <param name="address">PLC的地址，支持M1.1000，M3.1000，M10.1000</param>
        /// <param name="length">读取的长度信息，按照字为单位</param>
        /// <returns>包含byte[]的原始字节数据内容</returns>
        [EstMqttApi("ReadByteArray", "")]
        public override OperateResult<byte[]> Read(string address, ushort length)
        {
            OperateResult<FujiSPHAddress> from = FujiSPHAddress.ParseFrom(address);
            return !from.IsSuccess ? from.ConvertFailed<byte[]>() : this.ReadFujiSPHAddress(from.Content, length);
        }

        /// <summary>
        /// 批量写入字节数组到PLC的地址里，地址支持M1.1000，M3.1000，M10.1000，返回是否写入成功。<br />
        /// Batch write byte array to PLC address, the address supports M1.1000, M3.1000, M10.1000,
        /// and return whether the writing is successful.
        /// </summary>
        /// <param name="address">PLC的地址，支持M1.1000，M3.1000，M10.1000</param>
        /// <param name="value">写入的原始字节数据</param>
        /// <returns>是否写入成功</returns>
        [EstMqttApi("WriteByteArray", "")]
        public override OperateResult Write(string address, byte[] value)
        {
            OperateResult<byte[]> operateResult1 = FujiSPHNet.BuildWriteCommand(this.ConnectionID, address, value);
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1.ConvertFailed<byte[]>();
            OperateResult<byte[]> operateResult2 = this.ReadFromCoreServer(operateResult1.Content);
            if (!operateResult2.IsSuccess)
                return (OperateResult)operateResult2;
            OperateResult<byte[]> operateResult3 = FujiSPHNet.ExtraResponseContent(operateResult2.Content);
            return !operateResult3.IsSuccess ? (OperateResult)operateResult3 : OperateResult.CreateSuccessResult();
        }

        /// <summary>
        /// 批量读取位数据的方法，需要传入位地址，读取的位长度，地址示例：M1.100.5，M3.1000.12，M10.1000.0<br />
        /// To read the bit data in batches, you need to pass in the bit address, the length of the read bit, address examples: M1.100.5, M3.1000.12, M10.1000.0
        /// </summary>
        /// <param name="address">PLC的地址，示例：M1.100.5，M3.1000.12，M10.1000.0</param>
        /// <param name="length">读取的bool长度信息</param>
        /// <returns>包含bool[]的结果对象</returns>
        [EstMqttApi("ReadBoolArray", "")]
        public override OperateResult<bool[]> ReadBool(string address, ushort length)
        {
            OperateResult<FujiSPHAddress> from = FujiSPHAddress.ParseFrom(address);
            if (!from.IsSuccess)
                return from.ConvertFailed<bool[]>();
            int num1 = from.Content.BitIndex + (int)length;
            int num2 = num1 % 16 == 0 ? num1 / 16 : num1 / 16 + 1;
            OperateResult<byte[]> operateResult = this.ReadFujiSPHAddress(from.Content, (ushort)num2);
            return !operateResult.IsSuccess ? operateResult.ConvertFailed<bool[]>() : OperateResult.CreateSuccessResult<bool[]>(operateResult.Content.ToBoolArray().SelectMiddle<bool>(from.Content.BitIndex, (int)length));
        }

        /// <summary>
        /// 批量写入位数据的方法，需要传入位地址，等待写入的boo[]数据，地址示例：M1.100.5，M3.1000.12，M10.1000.0<br />
        /// To write bit data in batches, you need to pass in the bit address and wait for the boo[] data to be written. Examples of addresses: M1.100.5, M3.1000.12, M10.1000.0
        /// </summary>
        /// <remarks>
        /// [警告] 由于协议没有提供位写入的命令，所有通过字写入间接实现，先读取字数据，修改中间的位，然后写入字数据，所以本质上不是安全的，确保相关的地址只有上位机可以写入。
        /// [Warning] Since the protocol does not provide commands for bit writing, all are implemented indirectly through word writing. First read the word data,
        /// modify the bits in the middle, and then write the word data, so it is inherently not safe. Make sure that the relevant address is only The host computer can write.
        /// </remarks>
        /// <param name="address">PLC的地址，示例：M1.100.5，M3.1000.12，M10.1000.0</param>
        /// <param name="value">等待写入的bool数组</param>
        /// <returns>是否写入成功的结果对象</returns>
        [EstMqttApi("WriteBoolArray", "")]
        public override OperateResult Write(string address, bool[] value)
        {
            OperateResult<FujiSPHAddress> from = FujiSPHAddress.ParseFrom(address);
            if (!from.IsSuccess)
                return (OperateResult)from.ConvertFailed<bool[]>();
            int num1 = from.Content.BitIndex + value.Length;
            int num2 = num1 % 16 == 0 ? num1 / 16 : num1 / 16 + 1;
            OperateResult<byte[]> operateResult1 = this.ReadFujiSPHAddress(from.Content, (ushort)num2);
            if (!operateResult1.IsSuccess)
                return (OperateResult)operateResult1.ConvertFailed<bool[]>();
            bool[] boolArray = operateResult1.Content.ToBoolArray();
            value.CopyTo((Array)boolArray, from.Content.BitIndex);
            OperateResult<byte[]> operateResult2 = FujiSPHNet.BuildWriteCommand(this.ConnectionID, address, boolArray.ToByteArray());
            if (!operateResult2.IsSuccess)
                return (OperateResult)operateResult2.ConvertFailed<byte[]>();
            OperateResult<byte[]> operateResult3 = this.ReadFromCoreServer(operateResult2.Content);
            if (!operateResult3.IsSuccess)
                return (OperateResult)operateResult3;
            OperateResult<byte[]> operateResult4 = FujiSPHNet.ExtraResponseContent(operateResult3.Content);
            return !operateResult4.IsSuccess ? (OperateResult)operateResult4 : OperateResult.CreateSuccessResult();
        }

        private async Task<OperateResult<byte[]>> ReadFujiSPHAddressAsync(
          FujiSPHAddress address,
          ushort length)
        {
            OperateResult<List<byte[]>> command = FujiSPHNet.BuildReadCommand(this.ConnectionID, address, length);
            if (!command.IsSuccess)
                return command.ConvertFailed<byte[]>();
            List<byte> array = new List<byte>();
            for (int i = 0; i < command.Content.Count; ++i)
            {
                OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content[i]);
                if (!read.IsSuccess)
                    return read;
                OperateResult<byte[]> extra = FujiSPHNet.ExtraResponseContent(read.Content);
                if (!extra.IsSuccess)
                    return extra;
                array.AddRange((IEnumerable<byte>)extra.Content);
                read = (OperateResult<byte[]>)null;
                extra = (OperateResult<byte[]>)null;
            }
            return OperateResult.CreateSuccessResult<byte[]>(array.ToArray());
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Fuji.FujiSPHNet.Read(System.String,System.UInt16)" />
        public override async Task<OperateResult<byte[]>> ReadAsync(
          string address,
          ushort length)
        {
            OperateResult<FujiSPHAddress> analysis = FujiSPHAddress.ParseFrom(address);
            if (!analysis.IsSuccess)
                return analysis.ConvertFailed<byte[]>();
            OperateResult<byte[]> operateResult = await this.ReadFujiSPHAddressAsync(analysis.Content, length);
            return operateResult;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Fuji.FujiSPHNet.Write(System.String,System.Byte[])" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          byte[] value)
        {
            OperateResult<byte[]> command = FujiSPHNet.BuildWriteCommand(this.ConnectionID, address, value);
            if (!command.IsSuccess)
                return (OperateResult)command.ConvertFailed<byte[]>();
            OperateResult<byte[]> read = await this.ReadFromCoreServerAsync(command.Content);
            if (!read.IsSuccess)
                return (OperateResult)read;
            OperateResult<byte[]> extra = FujiSPHNet.ExtraResponseContent(read.Content);
            return extra.IsSuccess ? OperateResult.CreateSuccessResult() : (OperateResult)extra;
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Fuji.FujiSPHNet.ReadBool(System.String,System.UInt16)" />
        public override async Task<OperateResult<bool[]>> ReadBoolAsync(
          string address,
          ushort length)
        {
            OperateResult<FujiSPHAddress> analysis = FujiSPHAddress.ParseFrom(address);
            if (!analysis.IsSuccess)
                return analysis.ConvertFailed<bool[]>();
            int bitCount = analysis.Content.BitIndex + (int)length;
            int wordLength = bitCount % 16 == 0 ? bitCount / 16 : bitCount / 16 + 1;
            OperateResult<byte[]> read = await this.ReadFujiSPHAddressAsync(analysis.Content, (ushort)wordLength);
            return read.IsSuccess ? OperateResult.CreateSuccessResult<bool[]>(read.Content.ToBoolArray().SelectMiddle<bool>(analysis.Content.BitIndex, (int)length)) : read.ConvertFailed<bool[]>();
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Fuji.FujiSPHNet.Write(System.String,System.Boolean[])" />
        public override async Task<OperateResult> WriteAsync(
          string address,
          bool[] value)
        {
            OperateResult<FujiSPHAddress> analysis = FujiSPHAddress.ParseFrom(address);
            if (!analysis.IsSuccess)
                return (OperateResult)analysis.ConvertFailed<bool[]>();
            int bitCount = analysis.Content.BitIndex + value.Length;
            int wordLength = bitCount % 16 == 0 ? bitCount / 16 : bitCount / 16 + 1;
            OperateResult<byte[]> read = await this.ReadFujiSPHAddressAsync(analysis.Content, (ushort)wordLength);
            if (!read.IsSuccess)
                return (OperateResult)read.ConvertFailed<bool[]>();
            bool[] writeBoolArray = read.Content.ToBoolArray();
            value.CopyTo((Array)writeBoolArray, analysis.Content.BitIndex);
            OperateResult<byte[]> command = FujiSPHNet.BuildWriteCommand(this.ConnectionID, address, writeBoolArray.ToByteArray());
            if (!command.IsSuccess)
                return (OperateResult)command.ConvertFailed<byte[]>();
            OperateResult<byte[]> write = await this.ReadFromCoreServerAsync(command.Content);
            if (!write.IsSuccess)
                return (OperateResult)write;
            OperateResult<byte[]> extra = FujiSPHNet.ExtraResponseContent(write.Content);
            return extra.IsSuccess ? OperateResult.CreateSuccessResult() : (OperateResult)extra;
        }

        /// <summary>
        /// This command is used to start all the CPUs that exist in a configuration in a batch.
        /// Each CPU is cold-started or warm-started,depending on its condition. If a CPU is already started up,
        /// or if the key switch is set at "RUN" position, the CPU does not perform processing for startup,
        /// which, however, does not result in an error, and a response is returned normally
        /// </summary>
        /// <returns>是否启动成功</returns>
        [EstMqttApi]
        public OperateResult CpuBatchStart() => (OperateResult)this.ReadFromCoreServer(FujiSPHNet.PackCommand(this.ConnectionID, (byte)4, (byte)0, (byte[])null)).Check(new Func<byte[], OperateResult>(FujiSPHNet.ExtraResponseContent));

        /// <summary>
        /// This command is used to initialize and start all the CPUs that exist in a configuration in a batch. Each CPU is cold-started.
        /// If a CPU is already started up, or if the key switch is set at "RUN" position, the CPU does not perform processing for initialization
        /// and startup, which, however, does not result in an error, and a response is returned normally.
        /// </summary>
        /// <returns>是否启动成功</returns>
        [EstMqttApi]
        public OperateResult CpuBatchInitializeAndStart() => (OperateResult)this.ReadFromCoreServer(FujiSPHNet.PackCommand(this.ConnectionID, (byte)4, (byte)1, (byte[])null)).Check(new Func<byte[], OperateResult>(FujiSPHNet.ExtraResponseContent));

        /// <summary>
        /// This command is used to stop all the CPUs that exist in a configuration in a batch.
        /// If a CPU is already stopped, or if the key switch is set at "RUN" position, the CPU does not perform processing for stop, which,
        /// however, does not result in an error, and a response is returned normally.
        /// </summary>
        /// <returns>是否停止成功</returns>
        [EstMqttApi]
        public OperateResult CpuBatchStop() => (OperateResult)this.ReadFromCoreServer(FujiSPHNet.PackCommand(this.ConnectionID, (byte)4, (byte)2, (byte[])null)).Check(new Func<byte[], OperateResult>(FujiSPHNet.ExtraResponseContent));

        /// <summary>
        /// This command is used to stop all the CPUs that exist in a configuration in a batch.
        /// If a CPU is already stopped, or if the key switch is set at "RUN" position, the CPU does not perform processing for stop, which,
        /// however, does not result in an error, and a response is returned normally.
        /// </summary>
        /// <returns>是否复位成功</returns>
        [EstMqttApi]
        public OperateResult CpuBatchReset() => (OperateResult)this.ReadFromCoreServer(FujiSPHNet.PackCommand(this.ConnectionID, (byte)4, (byte)3, (byte[])null)).Check(new Func<byte[], OperateResult>(FujiSPHNet.ExtraResponseContent));

        /// <summary>
        /// This command is used to start an arbitrary CPU existing in a configuration by specifying it. The CPU may be cold-started or
        /// warm-started, depending on its condition. An error occurs if the CPU is already started. A target CPU is specified by a connection
        /// mode and connection ID.
        /// </summary>
        /// <returns>是否启动成功</returns>
        [EstMqttApi]
        public OperateResult CpuIndividualStart() => (OperateResult)this.ReadFromCoreServer(FujiSPHNet.PackCommand(this.ConnectionID, (byte)4, (byte)4, (byte[])null)).Check(new Func<byte[], OperateResult>(FujiSPHNet.ExtraResponseContent));

        /// <summary>
        /// This command is used to initialize and start an arbitrary CPU existing in a configuration by specifying it. The CPU is cold-started.
        /// An error occurs if the CPU is already started or if the key switch is set at "RUN" or "STOP" position. A target CPU is specified by
        /// a connection mode and connection ID.
        /// </summary>
        /// <returns>是否启动成功</returns>
        [EstMqttApi]
        public OperateResult CpuIndividualInitializeAndStart() => (OperateResult)this.ReadFromCoreServer(FujiSPHNet.PackCommand(this.ConnectionID, (byte)4, (byte)5, (byte[])null)).Check(new Func<byte[], OperateResult>(FujiSPHNet.ExtraResponseContent));

        /// <summary>
        /// This command is used to stop an arbitrary CPU existing in a configuration by specifying it. An error occurs if the CPU is already
        /// stopped or if the key switch is set at "RUN" or "STOP" position. A target CPU is specified by a connection mode and connection ID.
        /// </summary>
        /// <returns>是否停止成功</returns>
        [EstMqttApi]
        public OperateResult CpuIndividualStop() => (OperateResult)this.ReadFromCoreServer(FujiSPHNet.PackCommand(this.ConnectionID, (byte)4, (byte)6, (byte[])null)).Check(new Func<byte[], OperateResult>(FujiSPHNet.ExtraResponseContent));

        /// <summary>
        /// This command is used to reset an arbitrary CPU existing in a configuration by specifying it. An error occurs if the key switch is
        /// set at "RUN" or "STOP" position. A target CPU is specified by a connection mode and connection ID.
        /// </summary>
        /// <returns>是否复位成功</returns>
        [EstMqttApi]
        public OperateResult CpuIndividualReset() => (OperateResult)this.ReadFromCoreServer(FujiSPHNet.PackCommand(this.ConnectionID, (byte)4, (byte)7, (byte[])null)).Check(new Func<byte[], OperateResult>(FujiSPHNet.ExtraResponseContent));

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Fuji.FujiSPHNet.CpuBatchStart" />
        public async Task<OperateResult> CpuBatchStartAsync()
        {
            OperateResult<byte[]> operateResult = await this.ReadFromCoreServerAsync(FujiSPHNet.PackCommand(this.ConnectionID, (byte)4, (byte)0, (byte[])null));
            return (OperateResult)operateResult.Check(new Func<byte[], OperateResult>(FujiSPHNet.ExtraResponseContent));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Fuji.FujiSPHNet.CpuBatchInitializeAndStart" />
        public async Task<OperateResult> CpuBatchInitializeAndStartAsync()
        {
            OperateResult<byte[]> operateResult = await this.ReadFromCoreServerAsync(FujiSPHNet.PackCommand(this.ConnectionID, (byte)4, (byte)1, (byte[])null));
            return (OperateResult)operateResult.Check(new Func<byte[], OperateResult>(FujiSPHNet.ExtraResponseContent));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Fuji.FujiSPHNet.CpuBatchStop" />
        public async Task<OperateResult> CpuBatchStopAsync()
        {
            OperateResult<byte[]> operateResult = await this.ReadFromCoreServerAsync(FujiSPHNet.PackCommand(this.ConnectionID, (byte)4, (byte)2, (byte[])null));
            return (OperateResult)operateResult.Check(new Func<byte[], OperateResult>(FujiSPHNet.ExtraResponseContent));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Fuji.FujiSPHNet.CpuBatchReset" />
        public async Task<OperateResult> CpuBatchResetAsync()
        {
            OperateResult<byte[]> operateResult = await this.ReadFromCoreServerAsync(FujiSPHNet.PackCommand(this.ConnectionID, (byte)4, (byte)3, (byte[])null));
            return (OperateResult)operateResult.Check(new Func<byte[], OperateResult>(FujiSPHNet.ExtraResponseContent));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Fuji.FujiSPHNet.CpuIndividualStart" />
        public async Task<OperateResult> CpuIndividualStartAsync()
        {
            OperateResult<byte[]> operateResult = await this.ReadFromCoreServerAsync(FujiSPHNet.PackCommand(this.ConnectionID, (byte)4, (byte)4, (byte[])null));
            return (OperateResult)operateResult.Check(new Func<byte[], OperateResult>(FujiSPHNet.ExtraResponseContent));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Fuji.FujiSPHNet.CpuIndividualInitializeAndStartAsync" />
        public async Task<OperateResult> CpuIndividualInitializeAndStartAsync()
        {
            OperateResult<byte[]> operateResult = await this.ReadFromCoreServerAsync(FujiSPHNet.PackCommand(this.ConnectionID, (byte)4, (byte)5, (byte[])null));
            return (OperateResult)operateResult.Check(new Func<byte[], OperateResult>(FujiSPHNet.ExtraResponseContent));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Fuji.FujiSPHNet.CpuIndividualStop" />
        public async Task<OperateResult> CpuIndividualStopAsync()
        {
            OperateResult<byte[]> operateResult = await this.ReadFromCoreServerAsync(FujiSPHNet.PackCommand(this.ConnectionID, (byte)4, (byte)6, (byte[])null));
            return (OperateResult)operateResult.Check(new Func<byte[], OperateResult>(FujiSPHNet.ExtraResponseContent));
        }

        /// <inheritdoc cref="M:ESTCore.Common.Profinet.Fuji.FujiSPHNet.CpuIndividualReset" />
        public async Task<OperateResult> CpuIndividualResetAsync()
        {
            OperateResult<byte[]> operateResult = await this.ReadFromCoreServerAsync(FujiSPHNet.PackCommand(this.ConnectionID, (byte)4, (byte)7, (byte[])null));
            return (OperateResult)operateResult.Check(new Func<byte[], OperateResult>(FujiSPHNet.ExtraResponseContent));
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("FujiSPHNet[{0}:{1}]", (object)this.IpAddress, (object)this.Port);

        /// <summary>根据错误代号获取详细的错误描述信息</summary>
        /// <param name="code">错误码</param>
        /// <returns>错误的描述文本</returns>
        public static string GetErrorDescription(byte code)
        {
            switch (code)
            {
                case 16:
                    return "Command cannot be executed because an error occurred in the CPU.";
                case 17:
                    return "Command cannot be executed because the CPU is running.";
                case 18:
                    return "Command cannot be executed due to the key switch condition of the CPU.";
                case 32:
                    return "CPU received undefined command or mode.";
                case 34:
                    return "Setting error was found in command header part.";
                case 35:
                    return "Transmission is interlocked by a command from another device.";
                case 40:
                    return "Requested command cannot be executed because another command is now being executed.";
                case 43:
                    return "Requested command cannot be executed because the loader is now performing another processing( including program change).";
                case 47:
                    return "Requested command cannot be executed because the system is now being initialized.";
                case 64:
                    return "Invalid data type or number was specified.";
                case 65:
                    return "Specified data cannot be found.";
                case 68:
                    return "Specified address exceeds the valid range.";
                case 69:
                    return "Address + the number of read/write words exceed the valid range.";
                case 160:
                    return "No module exists at specified destination station No.";
                case 162:
                    return "No response data is returned from the destination module.";
                case 164:
                    return "Command cannot be communicated because an error occurred in the SX bus.";
                case 165:
                    return "Command cannot be communicated because NAK occurred while sending data via the SX bus.";
                default:
                    return StringResources.Language.UnknownError;
            }
        }

        private static byte[] PackCommand(byte connectionId, byte command, byte mode, byte[] data)
        {
            if (data == null)
                data = new byte[0];
            byte[] numArray = new byte[20 + data.Length];
            numArray[0] = (byte)251;
            numArray[1] = (byte)128;
            numArray[2] = (byte)128;
            numArray[3] = (byte)0;
            numArray[4] = byte.MaxValue;
            numArray[5] = (byte)123;
            numArray[6] = connectionId;
            numArray[7] = (byte)0;
            numArray[8] = (byte)17;
            numArray[9] = (byte)0;
            numArray[10] = (byte)0;
            numArray[11] = (byte)0;
            numArray[12] = (byte)0;
            numArray[13] = (byte)0;
            numArray[14] = command;
            numArray[15] = mode;
            numArray[16] = (byte)0;
            numArray[17] = (byte)1;
            numArray[18] = BitConverter.GetBytes(data.Length)[0];
            numArray[19] = BitConverter.GetBytes(data.Length)[1];
            if ((uint)data.Length > 0U)
                data.CopyTo((Array)numArray, 20);
            return numArray;
        }

        /// <summary>构建读取数据的命令报文</summary>
        /// <param name="connectionId">连接ID</param>
        /// <param name="address">读取的PLC的地址</param>
        /// <param name="length">读取的长度信息，按照字为单位</param>
        /// <returns>构建成功的读取报文命令</returns>
        public static OperateResult<List<byte[]>> BuildReadCommand(
          byte connectionId,
          string address,
          ushort length)
        {
            OperateResult<FujiSPHAddress> from = FujiSPHAddress.ParseFrom(address);
            return !from.IsSuccess ? from.ConvertFailed<List<byte[]>>() : FujiSPHNet.BuildReadCommand(connectionId, from.Content, length);
        }

        /// <summary>构建读取数据的命令报文</summary>
        /// <param name="connectionId">连接ID</param>
        /// <param name="address">读取的PLC的地址</param>
        /// <param name="length">读取的长度信息，按照字为单位</param>
        /// <returns>构建成功的读取报文命令</returns>
        public static OperateResult<List<byte[]>> BuildReadCommand(
          byte connectionId,
          FujiSPHAddress address,
          ushort length)
        {
            List<byte[]> numArrayList = new List<byte[]>();
            int[] array = SoftBasic.SplitIntegerToArray((int)length, int.MaxValue);
            for (int index = 0; index < array.Length; ++index)
            {
                byte[] data = new byte[6]
                {
          address.TypeCode,
          BitConverter.GetBytes(address.AddressStart)[0],
          BitConverter.GetBytes(address.AddressStart)[1],
          BitConverter.GetBytes(address.AddressStart)[2],
          BitConverter.GetBytes(array[index])[0],
          BitConverter.GetBytes(array[index])[1]
                };
                numArrayList.Add(FujiSPHNet.PackCommand(connectionId, (byte)0, (byte)0, data));
                address.AddressStart += array[index];
            }
            return OperateResult.CreateSuccessResult<List<byte[]>>(numArrayList);
        }

        /// <summary>构建写入数据的命令报文</summary>
        /// <param name="connectionId">连接ID</param>
        /// <param name="address">写入的PLC的地址</param>
        /// <param name="data">原始数据内容</param>
        /// <returns>报文信息</returns>
        public static OperateResult<byte[]> BuildWriteCommand(
          byte connectionId,
          string address,
          byte[] data)
        {
            OperateResult<FujiSPHAddress> from = FujiSPHAddress.ParseFrom(address);
            if (!from.IsSuccess)
                return from.ConvertFailed<byte[]>();
            int num = data.Length / 2;
            byte[] data1 = new byte[6 + data.Length];
            data1[0] = from.Content.TypeCode;
            data1[1] = BitConverter.GetBytes(from.Content.AddressStart)[0];
            data1[2] = BitConverter.GetBytes(from.Content.AddressStart)[1];
            data1[3] = BitConverter.GetBytes(from.Content.AddressStart)[2];
            data1[4] = BitConverter.GetBytes(num)[0];
            data1[5] = BitConverter.GetBytes(num)[1];
            data.CopyTo((Array)data1, 6);
            return OperateResult.CreateSuccessResult<byte[]>(FujiSPHNet.PackCommand(connectionId, (byte)1, (byte)0, data1));
        }

        /// <summary>从PLC返回的报文里解析出实际的数据内容，如果发送了错误，则返回失败信息</summary>
        /// <param name="response">PLC返回的报文信息</param>
        /// <returns>是否成功的结果对象</returns>
        public static OperateResult<byte[]> ExtraResponseContent(byte[] response)
        {
            try
            {
                if (response[4] > (byte)0)
                    return new OperateResult<byte[]>((int)response[4], FujiSPHNet.GetErrorDescription(response[4]));
                return response.Length > 26 ? OperateResult.CreateSuccessResult<byte[]>(response.RemoveBegin<byte>(26)) : OperateResult.CreateSuccessResult<byte[]>(new byte[0]);
            }
            catch (Exception ex)
            {
                return new OperateResult<byte[]>(ex.Message + " Source: " + response.ToHexString(' '));
            }
        }
    }
}
