// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.Omron.OmronCipNet
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using EstCommunication.Core;
using EstCommunication.Profinet.AllenBradley;
using EstCommunication.Reflection;
using System;
using System.Text;
using System.Threading.Tasks;

namespace EstCommunication.Profinet.Omron
{
  /// <summary>
  /// 欧姆龙PLC的CIP协议的类，支持NJ,NX,NY系列PLC，支持tag名的方式读写数据，假设你读取的是局部变量，那么使用 Program:MainProgram.变量名<br />
  /// Omron PLC's CIP protocol class, support NJ, NX, NY series PLC, support tag name read and write data, assuming you read local variables, then use Program: MainProgram. Variable name
  /// </summary>
  public class OmronCipNet : AllenBradleyNet
  {
    /// <summary>
    /// Instantiate a communication object for a OmronCipNet PLC protocol
    /// </summary>
    public OmronCipNet()
    {
    }

    /// <summary>
    /// Specify the IP address and port to instantiate a communication object for a OmronCipNet PLC protocol
    /// </summary>
    /// <param name="ipAddress">PLC IpAddress</param>
    /// <param name="port">PLC Port</param>
    public OmronCipNet(string ipAddress, int port = 44818)
      : base(ipAddress, port)
    {
    }

    /// <inheritdoc />
    [EstMqttApi("ReadByteArray", "")]
    public override OperateResult<byte[]> Read(string address, ushort length) => length > (ushort) 1 ? this.Read(new string[1]
    {
      address
    }, new int[1]{ 1 }) : this.Read(new string[1]{ address }, new int[1]
    {
      (int) length
    });

    /// <inheritdoc />
    [EstMqttApi("ReadInt16Array", "")]
    public override OperateResult<short[]> ReadInt16(string address, ushort length)
    {
      if (length == (ushort) 1)
        return ByteTransformHelper.GetResultFromBytes<short[]>(this.Read(address, (ushort) 1), (Func<byte[], short[]>) (m => this.ByteTransform.TransInt16(m, 0, (int) length)));
      int startIndex = EstHelper.ExtractStartIndex(ref address);
      return ByteTransformHelper.GetResultFromBytes<short[]>(this.Read(address, (ushort) 1), (Func<byte[], short[]>) (m => this.ByteTransform.TransInt16(m, startIndex < 0 ? 0 : startIndex * 2, (int) length)));
    }

    /// <inheritdoc />
    [EstMqttApi("ReadUInt16Array", "")]
    public override OperateResult<ushort[]> ReadUInt16(string address, ushort length)
    {
      if (length == (ushort) 1)
        return ByteTransformHelper.GetResultFromBytes<ushort[]>(this.Read(address, (ushort) 1), (Func<byte[], ushort[]>) (m => this.ByteTransform.TransUInt16(m, 0, (int) length)));
      int startIndex = EstHelper.ExtractStartIndex(ref address);
      return ByteTransformHelper.GetResultFromBytes<ushort[]>(this.Read(address, (ushort) 1), (Func<byte[], ushort[]>) (m => this.ByteTransform.TransUInt16(m, startIndex < 0 ? 0 : startIndex * 2, (int) length)));
    }

    /// <inheritdoc />
    [EstMqttApi("ReadInt32Array", "")]
    public override OperateResult<int[]> ReadInt32(string address, ushort length)
    {
      if (length == (ushort) 1)
        return ByteTransformHelper.GetResultFromBytes<int[]>(this.Read(address, (ushort) 1), (Func<byte[], int[]>) (m => this.ByteTransform.TransInt32(m, 0, (int) length)));
      int startIndex = EstHelper.ExtractStartIndex(ref address);
      return ByteTransformHelper.GetResultFromBytes<int[]>(this.Read(address, (ushort) 1), (Func<byte[], int[]>) (m => this.ByteTransform.TransInt32(m, startIndex < 0 ? 0 : startIndex * 4, (int) length)));
    }

    /// <inheritdoc />
    [EstMqttApi("ReadUInt32Array", "")]
    public override OperateResult<uint[]> ReadUInt32(string address, ushort length)
    {
      if (length == (ushort) 1)
        return ByteTransformHelper.GetResultFromBytes<uint[]>(this.Read(address, (ushort) 1), (Func<byte[], uint[]>) (m => this.ByteTransform.TransUInt32(m, 0, (int) length)));
      int startIndex = EstHelper.ExtractStartIndex(ref address);
      return ByteTransformHelper.GetResultFromBytes<uint[]>(this.Read(address, (ushort) 1), (Func<byte[], uint[]>) (m => this.ByteTransform.TransUInt32(m, startIndex < 0 ? 0 : startIndex * 4, (int) length)));
    }

    /// <inheritdoc />
    [EstMqttApi("ReadFloatArray", "")]
    public override OperateResult<float[]> ReadFloat(string address, ushort length)
    {
      if (length == (ushort) 1)
        return ByteTransformHelper.GetResultFromBytes<float[]>(this.Read(address, (ushort) 1), (Func<byte[], float[]>) (m => this.ByteTransform.TransSingle(m, 0, (int) length)));
      int startIndex = EstHelper.ExtractStartIndex(ref address);
      return ByteTransformHelper.GetResultFromBytes<float[]>(this.Read(address, (ushort) 1), (Func<byte[], float[]>) (m => this.ByteTransform.TransSingle(m, startIndex < 0 ? 0 : startIndex * 4, (int) length)));
    }

    /// <inheritdoc />
    [EstMqttApi("ReadInt64Array", "")]
    public override OperateResult<long[]> ReadInt64(string address, ushort length)
    {
      if (length == (ushort) 1)
        return ByteTransformHelper.GetResultFromBytes<long[]>(this.Read(address, (ushort) 1), (Func<byte[], long[]>) (m => this.ByteTransform.TransInt64(m, 0, (int) length)));
      int startIndex = EstHelper.ExtractStartIndex(ref address);
      return ByteTransformHelper.GetResultFromBytes<long[]>(this.Read(address, (ushort) 1), (Func<byte[], long[]>) (m => this.ByteTransform.TransInt64(m, startIndex < 0 ? 0 : startIndex * 8, (int) length)));
    }

    /// <inheritdoc />
    [EstMqttApi("ReadUInt64Array", "")]
    public override OperateResult<ulong[]> ReadUInt64(string address, ushort length)
    {
      if (length == (ushort) 1)
        return ByteTransformHelper.GetResultFromBytes<ulong[]>(this.Read(address, (ushort) 1), (Func<byte[], ulong[]>) (m => this.ByteTransform.TransUInt64(m, 0, (int) length)));
      int startIndex = EstHelper.ExtractStartIndex(ref address);
      return ByteTransformHelper.GetResultFromBytes<ulong[]>(this.Read(address, (ushort) 1), (Func<byte[], ulong[]>) (m => this.ByteTransform.TransUInt64(m, startIndex < 0 ? 0 : startIndex * 8, (int) length)));
    }

    /// <inheritdoc />
    [EstMqttApi("ReadDoubleArray", "")]
    public override OperateResult<double[]> ReadDouble(string address, ushort length)
    {
      if (length == (ushort) 1)
        return ByteTransformHelper.GetResultFromBytes<double[]>(this.Read(address, (ushort) 1), (Func<byte[], double[]>) (m => this.ByteTransform.TransDouble(m, 0, (int) length)));
      int startIndex = EstHelper.ExtractStartIndex(ref address);
      return ByteTransformHelper.GetResultFromBytes<double[]>(this.Read(address, (ushort) 1), (Func<byte[], double[]>) (m => this.ByteTransform.TransDouble(m, startIndex < 0 ? 0 : startIndex * 8, (int) length)));
    }

    /// <inheritdoc />
    public override OperateResult<string> ReadString(
      string address,
      ushort length,
      Encoding encoding)
    {
      OperateResult<byte[]> operateResult = this.Read(address, length);
      if (!operateResult.IsSuccess)
        return OperateResult.CreateFailedResult<string>((OperateResult) operateResult);
      int count = (int) this.ByteTransform.TransUInt16(operateResult.Content, 0);
      return OperateResult.CreateSuccessResult<string>(encoding.GetString(operateResult.Content, 2, count));
    }

    /// <inheritdoc />
    [EstMqttApi("WriteString", "")]
    public override OperateResult Write(string address, string value)
    {
      if (string.IsNullOrEmpty(value))
        value = string.Empty;
      byte[] numArray = SoftBasic.SpliceArray<byte>(new byte[2], SoftBasic.ArrayExpandToLengthEven<byte>(Encoding.ASCII.GetBytes(value)));
      numArray[0] = BitConverter.GetBytes(numArray.Length - 2)[0];
      numArray[1] = BitConverter.GetBytes(numArray.Length - 2)[1];
      return base.WriteTag(address, (ushort) 208, numArray);
    }

    /// <inheritdoc />
    [EstMqttApi("WriteByte", "")]
    public override OperateResult Write(string address, byte value) => this.WriteTag(address, (ushort) 209, new byte[2]
    {
      value,
      (byte) 0
    }, 1);

    /// <inheritdoc />
    public override OperateResult WriteTag(
      string address,
      ushort typeCode,
      byte[] value,
      int length = 1)
    {
      return base.WriteTag(address, typeCode, value);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Omron.OmronCipNet.Read(System.String,System.UInt16)" />
    public override async Task<OperateResult<byte[]>> ReadAsync(
      string address,
      ushort length)
    {
      if (length > (ushort) 1)
      {
        OperateResult<byte[]> operateResult = await this.ReadAsync(new string[1]
        {
          address
        }, new int[1]{ 1 });
        return operateResult;
      }
      OperateResult<byte[]> operateResult1 = await this.ReadAsync(new string[1]
      {
        address
      }, new int[1]{ (int) length });
      return operateResult1;
    }

    /// <inheritdoc />
    public override async Task<OperateResult<short[]>> ReadInt16Async(
      string address,
      ushort length)
    {
      if (length == (ushort) 1)
      {
        OperateResult<byte[]> result = await this.ReadAsync(address, (ushort) 1);
        return ByteTransformHelper.GetResultFromBytes<short[]>(result, (Func<byte[], short[]>) (m => this.ByteTransform.TransInt16(m, 0, (int) length)));
      }
      int startIndex = EstHelper.ExtractStartIndex(ref address);
      OperateResult<byte[]> result1 = await this.ReadAsync(address, (ushort) 1);
      return ByteTransformHelper.GetResultFromBytes<short[]>(result1, (Func<byte[], short[]>) (m => this.ByteTransform.TransInt16(m, startIndex < 0 ? 0 : startIndex * 2, (int) length)));
    }

    /// <inheritdoc />
    public override async Task<OperateResult<ushort[]>> ReadUInt16Async(
      string address,
      ushort length)
    {
      if (length == (ushort) 1)
      {
        OperateResult<byte[]> result = await this.ReadAsync(address, (ushort) 1);
        return ByteTransformHelper.GetResultFromBytes<ushort[]>(result, (Func<byte[], ushort[]>) (m => this.ByteTransform.TransUInt16(m, 0, (int) length)));
      }
      int startIndex = EstHelper.ExtractStartIndex(ref address);
      OperateResult<byte[]> result1 = await this.ReadAsync(address, (ushort) 1);
      return ByteTransformHelper.GetResultFromBytes<ushort[]>(result1, (Func<byte[], ushort[]>) (m => this.ByteTransform.TransUInt16(m, startIndex < 0 ? 0 : startIndex * 2, (int) length)));
    }

    /// <inheritdoc />
    public override async Task<OperateResult<int[]>> ReadInt32Async(
      string address,
      ushort length)
    {
      if (length == (ushort) 1)
      {
        OperateResult<byte[]> result = await this.ReadAsync(address, (ushort) 1);
        return ByteTransformHelper.GetResultFromBytes<int[]>(result, (Func<byte[], int[]>) (m => this.ByteTransform.TransInt32(m, 0, (int) length)));
      }
      int startIndex = EstHelper.ExtractStartIndex(ref address);
      OperateResult<byte[]> result1 = await this.ReadAsync(address, (ushort) 1);
      return ByteTransformHelper.GetResultFromBytes<int[]>(result1, (Func<byte[], int[]>) (m => this.ByteTransform.TransInt32(m, startIndex < 0 ? 0 : startIndex * 4, (int) length)));
    }

    /// <inheritdoc />
    public override async Task<OperateResult<uint[]>> ReadUInt32Async(
      string address,
      ushort length)
    {
      if (length == (ushort) 1)
      {
        OperateResult<byte[]> result = await this.ReadAsync(address, (ushort) 1);
        return ByteTransformHelper.GetResultFromBytes<uint[]>(result, (Func<byte[], uint[]>) (m => this.ByteTransform.TransUInt32(m, 0, (int) length)));
      }
      int startIndex = EstHelper.ExtractStartIndex(ref address);
      OperateResult<byte[]> result1 = await this.ReadAsync(address, (ushort) 1);
      return ByteTransformHelper.GetResultFromBytes<uint[]>(result1, (Func<byte[], uint[]>) (m => this.ByteTransform.TransUInt32(m, startIndex < 0 ? 0 : startIndex * 4, (int) length)));
    }

    /// <inheritdoc />
    public override async Task<OperateResult<float[]>> ReadFloatAsync(
      string address,
      ushort length)
    {
      if (length == (ushort) 1)
      {
        OperateResult<byte[]> result = await this.ReadAsync(address, (ushort) 1);
        return ByteTransformHelper.GetResultFromBytes<float[]>(result, (Func<byte[], float[]>) (m => this.ByteTransform.TransSingle(m, 0, (int) length)));
      }
      int startIndex = EstHelper.ExtractStartIndex(ref address);
      OperateResult<byte[]> result1 = await this.ReadAsync(address, (ushort) 1);
      return ByteTransformHelper.GetResultFromBytes<float[]>(result1, (Func<byte[], float[]>) (m => this.ByteTransform.TransSingle(m, startIndex < 0 ? 0 : startIndex * 4, (int) length)));
    }

    /// <inheritdoc />
    public override async Task<OperateResult<long[]>> ReadInt64Async(
      string address,
      ushort length)
    {
      if (length == (ushort) 1)
      {
        OperateResult<byte[]> result = await this.ReadAsync(address, (ushort) 1);
        return ByteTransformHelper.GetResultFromBytes<long[]>(result, (Func<byte[], long[]>) (m => this.ByteTransform.TransInt64(m, 0, (int) length)));
      }
      int startIndex = EstHelper.ExtractStartIndex(ref address);
      OperateResult<byte[]> result1 = await this.ReadAsync(address, (ushort) 1);
      return ByteTransformHelper.GetResultFromBytes<long[]>(result1, (Func<byte[], long[]>) (m => this.ByteTransform.TransInt64(m, startIndex < 0 ? 0 : startIndex * 8, (int) length)));
    }

    /// <inheritdoc />
    public override async Task<OperateResult<ulong[]>> ReadUInt64Async(
      string address,
      ushort length)
    {
      if (length == (ushort) 1)
      {
        OperateResult<byte[]> result = await this.ReadAsync(address, (ushort) 1);
        return ByteTransformHelper.GetResultFromBytes<ulong[]>(result, (Func<byte[], ulong[]>) (m => this.ByteTransform.TransUInt64(m, 0, (int) length)));
      }
      int startIndex = EstHelper.ExtractStartIndex(ref address);
      OperateResult<byte[]> result1 = await this.ReadAsync(address, (ushort) 1);
      return ByteTransformHelper.GetResultFromBytes<ulong[]>(result1, (Func<byte[], ulong[]>) (m => this.ByteTransform.TransUInt64(m, startIndex < 0 ? 0 : startIndex * 8, (int) length)));
    }

    /// <inheritdoc />
    public override async Task<OperateResult<double[]>> ReadDoubleAsync(
      string address,
      ushort length)
    {
      if (length == (ushort) 1)
      {
        OperateResult<byte[]> result = await this.ReadAsync(address, (ushort) 1);
        return ByteTransformHelper.GetResultFromBytes<double[]>(result, (Func<byte[], double[]>) (m => this.ByteTransform.TransDouble(m, 0, (int) length)));
      }
      int startIndex = EstHelper.ExtractStartIndex(ref address);
      OperateResult<byte[]> result1 = await this.ReadAsync(address, (ushort) 1);
      return ByteTransformHelper.GetResultFromBytes<double[]>(result1, (Func<byte[], double[]>) (m => this.ByteTransform.TransDouble(m, startIndex < 0 ? 0 : startIndex * 8, (int) length)));
    }

    /// <inheritdoc />
    public override async Task<OperateResult<string>> ReadStringAsync(
      string address,
      ushort length,
      Encoding encoding)
    {
      OperateResult<byte[]> read = await this.ReadAsync(address, length);
      if (!read.IsSuccess)
        return OperateResult.CreateFailedResult<string>((OperateResult) read);
      int strLen = (int) this.ByteTransform.TransUInt16(read.Content, 0);
      return OperateResult.CreateSuccessResult<string>(encoding.GetString(read.Content, 2, strLen));
    }

    /// <inheritdoc />
    public override async Task<OperateResult> WriteAsync(
      string address,
      string value)
    {
      if (string.IsNullOrEmpty(value))
        value = string.Empty;
      byte[] data = SoftBasic.SpliceArray<byte>(new byte[2], SoftBasic.ArrayExpandToLengthEven<byte>(Encoding.ASCII.GetBytes(value)));
      data[0] = BitConverter.GetBytes(data.Length - 2)[0];
      data[1] = BitConverter.GetBytes(data.Length - 2)[1];
      OperateResult operateResult = await this.WriteTagAsync(address, (ushort) 208, data, 1);
      data = (byte[]) null;
      return operateResult;
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Omron.OmronCipNet.Write(System.String,System.Byte)" />
    public override async Task<OperateResult> WriteAsync(
      string address,
      byte value)
    {
      OperateResult operateResult = await this.WriteTagAsync(address, (ushort) 209, new byte[2]
      {
        value,
        (byte) 0
      }, 1);
      return operateResult;
    }

    /// <inheritdoc />
    public override async Task<OperateResult> WriteTagAsync(
      string address,
      ushort typeCode,
      byte[] value,
      int length = 1)
    {
      OperateResult operateResult = await base.WriteTagAsync(address, typeCode, value);
      return operateResult;
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("OmronCipNet[{0}:{1}]", (object) this.IpAddress, (object) this.Port);
  }
}
