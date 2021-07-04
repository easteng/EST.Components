// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.Keyence.KeyenceNanoServer
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using EstCommunication.BasicFramework;
using EstCommunication.Core;
using EstCommunication.Core.Net;
using EstCommunication.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace EstCommunication.Profinet.Keyence
{
  /// <summary>基恩士的上位链路协议的虚拟服务器</summary>
  public class KeyenceNanoServer : NetworkDataServerBase
  {
    private SoftBuffer rBuffer;
    private SoftBuffer bBuffer;
    private SoftBuffer mrBuffer;
    private SoftBuffer lrBuffer;
    private SoftBuffer crBuffer;
    private SoftBuffer vbBuffer;
    private SoftBuffer dmBuffer;
    private SoftBuffer emBuffer;
    private SoftBuffer wBuffer;
    private SoftBuffer atBuffer;
    private const int DataPoolLength = 65536;

    /// <summary>
    /// 实例化一个基于上位链路协议的虚拟的基恩士PLC对象，可以用来和<see cref="T:EstCommunication.Profinet.Keyence.KeyenceNanoSerialOverTcp" />进行通信测试。
    /// </summary>
    public KeyenceNanoServer()
    {
      this.rBuffer = new SoftBuffer(65536);
      this.bBuffer = new SoftBuffer(65536);
      this.mrBuffer = new SoftBuffer(65536);
      this.lrBuffer = new SoftBuffer(65536);
      this.crBuffer = new SoftBuffer(65536);
      this.vbBuffer = new SoftBuffer(65536);
      this.dmBuffer = new SoftBuffer(131072);
      this.emBuffer = new SoftBuffer(131072);
      this.wBuffer = new SoftBuffer(131072);
      this.atBuffer = new SoftBuffer(65536);
      this.ByteTransform = (IByteTransform) new RegularByteTransform();
      this.WordLength = (ushort) 1;
    }

    /// <inheritdoc />
    protected override byte[] SaveToBytes()
    {
      byte[] numArray = new byte[851968];
      this.rBuffer.GetBytes().CopyTo((Array) numArray, 0);
      this.bBuffer.GetBytes().CopyTo((Array) numArray, 65536);
      this.mrBuffer.GetBytes().CopyTo((Array) numArray, 131072);
      this.lrBuffer.GetBytes().CopyTo((Array) numArray, 196608);
      this.crBuffer.GetBytes().CopyTo((Array) numArray, 262144);
      this.vbBuffer.GetBytes().CopyTo((Array) numArray, 327680);
      this.dmBuffer.GetBytes().CopyTo((Array) numArray, 393216);
      this.emBuffer.GetBytes().CopyTo((Array) numArray, 524288);
      this.wBuffer.GetBytes().CopyTo((Array) numArray, 655360);
      this.atBuffer.GetBytes().CopyTo((Array) numArray, 786432);
      return numArray;
    }

    /// <inheritdoc />
    protected override void LoadFromBytes(byte[] content)
    {
      if (content.Length < 851968)
        throw new Exception("File is not correct");
      this.rBuffer.SetBytes(content, 0, 65536);
      this.bBuffer.SetBytes(content, 65536, 65536);
      this.mrBuffer.SetBytes(content, 131072, 65536);
      this.lrBuffer.SetBytes(content, 196608, 65536);
      this.crBuffer.SetBytes(content, 262144, 65536);
      this.vbBuffer.SetBytes(content, 327680, 65536);
      this.dmBuffer.SetBytes(content, 393216, 131072);
      this.emBuffer.SetBytes(content, 524288, 131072);
      this.wBuffer.SetBytes(content, 655360, 131072);
      this.atBuffer.SetBytes(content, 786432, 65536);
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Keyence.KeyenceNanoSerialOverTcp.Read(System.String,System.UInt16)" />
    [EstMqttApi("ReadByteArray", "")]
    public override OperateResult<byte[]> Read(string address, ushort length)
    {
      try
      {
        if (address.StartsWith("DM"))
          return OperateResult.CreateSuccessResult<byte[]>(this.dmBuffer.GetBytes(int.Parse(address.Substring(2)) * 2, (int) length * 2));
        if (address.StartsWith("EM"))
          return OperateResult.CreateSuccessResult<byte[]>(this.emBuffer.GetBytes(int.Parse(address.Substring(2)) * 2, (int) length * 2));
        if (address.StartsWith("W"))
          return OperateResult.CreateSuccessResult<byte[]>(this.wBuffer.GetBytes(int.Parse(address.Substring(1)) * 2, (int) length * 2));
        return address.StartsWith("AT") ? OperateResult.CreateSuccessResult<byte[]>(this.atBuffer.GetBytes(int.Parse(address.Substring(2)) * 4, (int) length * 4)) : new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType);
      }
      catch (Exception ex)
      {
        return new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType + " Reason:" + ex.Message);
      }
    }

    /// <inheritdoc cref="M:EstCommunication.Profinet.Keyence.KeyenceNanoSerialOverTcp.Write(System.String,System.Byte[])" />
    [EstMqttApi("WriteByteArray", "")]
    public override OperateResult Write(string address, byte[] value)
    {
      try
      {
        if (address.StartsWith("DM"))
          this.dmBuffer.SetBytes(value, int.Parse(address.Substring(2)) * 2);
        else if (address.StartsWith("EM"))
          this.emBuffer.SetBytes(value, int.Parse(address.Substring(2)) * 2);
        else if (address.StartsWith("W"))
        {
          this.wBuffer.SetBytes(value, int.Parse(address.Substring(1)) * 2);
        }
        else
        {
          if (!address.StartsWith("AT"))
            return (OperateResult) new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType);
          this.atBuffer.SetBytes(value, int.Parse(address.Substring(2)) * 4);
        }
        return OperateResult.CreateSuccessResult();
      }
      catch (Exception ex)
      {
        return (OperateResult) new OperateResult<byte[]>(StringResources.Language.NotSupportedDataType + " Reason:" + ex.Message);
      }
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.ReadBool(System.String,System.UInt16)" />
    [EstMqttApi("ReadBoolArray", "")]
    public override OperateResult<bool[]> ReadBool(string address, ushort length)
    {
      try
      {
        if (address.StartsWith("R"))
          return OperateResult.CreateSuccessResult<bool[]>(((IEnumerable<byte>) this.rBuffer.GetBytes(int.Parse(address.Substring(1)), (int) length)).Select<byte, bool>((Func<byte, bool>) (m => m > (byte) 0)).ToArray<bool>());
        if (address.StartsWith("B"))
          return OperateResult.CreateSuccessResult<bool[]>(((IEnumerable<byte>) this.bBuffer.GetBytes(int.Parse(address.Substring(1)), (int) length)).Select<byte, bool>((Func<byte, bool>) (m => m > (byte) 0)).ToArray<bool>());
        if (address.StartsWith("MR"))
          return OperateResult.CreateSuccessResult<bool[]>(((IEnumerable<byte>) this.mrBuffer.GetBytes(int.Parse(address.Substring(2)), (int) length)).Select<byte, bool>((Func<byte, bool>) (m => m > (byte) 0)).ToArray<bool>());
        if (address.StartsWith("LR"))
          return OperateResult.CreateSuccessResult<bool[]>(((IEnumerable<byte>) this.lrBuffer.GetBytes(int.Parse(address.Substring(2)), (int) length)).Select<byte, bool>((Func<byte, bool>) (m => m > (byte) 0)).ToArray<bool>());
        if (address.StartsWith("CR"))
          return OperateResult.CreateSuccessResult<bool[]>(((IEnumerable<byte>) this.crBuffer.GetBytes(int.Parse(address.Substring(2)), (int) length)).Select<byte, bool>((Func<byte, bool>) (m => m > (byte) 0)).ToArray<bool>());
        return address.StartsWith("VB") ? OperateResult.CreateSuccessResult<bool[]>(((IEnumerable<byte>) this.vbBuffer.GetBytes(int.Parse(address.Substring(2)), (int) length)).Select<byte, bool>((Func<byte, bool>) (m => m > (byte) 0)).ToArray<bool>()) : new OperateResult<bool[]>(StringResources.Language.NotSupportedDataType);
      }
      catch (Exception ex)
      {
        return new OperateResult<bool[]>(StringResources.Language.NotSupportedDataType + " Reason:" + ex.Message);
      }
    }

    /// <inheritdoc cref="M:EstCommunication.Core.IReadWriteNet.Write(System.String,System.Boolean[])" />
    [EstMqttApi("WriteBoolArray", "")]
    public override OperateResult Write(string address, bool[] value)
    {
      try
      {
        byte[] array = ((IEnumerable<bool>) value).Select<bool, byte>((Func<bool, byte>) (m => !m ? (byte) 0 : (byte) 1)).ToArray<byte>();
        if (address.StartsWith("R"))
          this.rBuffer.SetBytes(array, int.Parse(address.Substring(1)));
        else if (address.StartsWith("B"))
          this.bBuffer.SetBytes(array, int.Parse(address.Substring(1)));
        else if (address.StartsWith("MR"))
          this.mrBuffer.SetBytes(array, int.Parse(address.Substring(2)));
        else if (address.StartsWith("LR"))
          this.lrBuffer.SetBytes(array, int.Parse(address.Substring(2)));
        else if (address.StartsWith("CR"))
        {
          this.crBuffer.SetBytes(array, int.Parse(address.Substring(2)));
        }
        else
        {
          if (!address.StartsWith("VB"))
            return (OperateResult) new OperateResult<bool[]>(StringResources.Language.NotSupportedDataType);
          this.vbBuffer.SetBytes(array, int.Parse(address.Substring(2)));
        }
        return OperateResult.CreateSuccessResult();
      }
      catch (Exception ex)
      {
        return (OperateResult) new OperateResult<bool[]>(StringResources.Language.NotSupportedDataType + " Reason:" + ex.Message);
      }
    }

    /// <inheritdoc />
    protected override void ThreadPoolLoginAfterClientCheck(Socket socket, IPEndPoint endPoint)
    {
      OperateResult<byte[]> commandLineFromSocket = this.ReceiveCommandLineFromSocket(socket, (byte) 13, 5000);
      if (!commandLineFromSocket.IsSuccess)
        socket?.Close();
      else if (!Encoding.ASCII.GetString(commandLineFromSocket.Content).StartsWith("CR"))
      {
        socket?.Close();
      }
      else
      {
        if (!this.Send(socket, Encoding.ASCII.GetBytes("CC\r\n")).IsSuccess)
          return;
        AppSession session = new AppSession();
        session.IpEndPoint = endPoint;
        session.WorkSocket = socket;
        if (socket.BeginReceiveResult(new AsyncCallback(this.SocketAsyncCallBack), (object) session).IsSuccess)
          this.AddClient(session);
        else
          this.LogNet?.WriteDebug(this.ToString(), string.Format(StringResources.Language.ClientOfflineInfo, (object) endPoint));
      }
    }

    private async void SocketAsyncCallBack(IAsyncResult ar)
    {
      if (!(ar.AsyncState is AppSession session))
        session = (AppSession) null;
      else if (!session.WorkSocket.EndReceiveResult(ar).IsSuccess)
      {
        this.RemoveClient(session);
        session = (AppSession) null;
      }
      else
      {
        OperateResult<byte[]> read1 = await this.ReceiveCommandLineFromSocketAsync(session.WorkSocket, (byte) 13, 5000);
        if (!read1.IsSuccess)
        {
          this.RemoveClient(session);
          session = (AppSession) null;
        }
        else if (!EstCommunication.Authorization.asdniasnfaksndiqwhawfskhfaiw())
        {
          this.RemoveClient(session);
          session = (AppSession) null;
        }
        else
        {
          this.LogNet?.WriteDebug(this.ToString(), string.Format("[{0}] Tcp {1}：{2}", (object) session.IpEndPoint, (object) StringResources.Language.Receive, (object) read1.Content.ToHexString(' ')));
          byte[] back = this.ReadFromNanoCore(read1.Content);
          if (back == null)
          {
            this.RemoveClient(session);
            session = (AppSession) null;
          }
          else if (!this.Send(session.WorkSocket, back).IsSuccess)
          {
            this.RemoveClient(session);
            session = (AppSession) null;
          }
          else
          {
            this.LogNet?.WriteDebug(this.ToString(), string.Format("[{0}] Tcp {1}：{2}", (object) session.IpEndPoint, (object) StringResources.Language.Send, (object) back.ToHexString(' ')));
            session.HeartTime = DateTime.Now;
            this.RaiseDataReceived((object) session, read1.Content);
            if (!session.WorkSocket.BeginReceiveResult(new AsyncCallback(this.SocketAsyncCallBack), (object) session).IsSuccess)
              this.RemoveClient(session);
            read1 = (OperateResult<byte[]>) null;
            back = (byte[]) null;
            session = (AppSession) null;
          }
        }
      }
    }

    private byte[] GetBoolResponseData(byte[] data)
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < data.Length; ++index)
      {
        stringBuilder.Append(data[index]);
        if (index != data.Length - 1)
          stringBuilder.Append(" ");
      }
      stringBuilder.Append("\r\n");
      return Encoding.ASCII.GetBytes(stringBuilder.ToString());
    }

    private byte[] GetWordResponseData(byte[] data)
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < data.Length / 2; ++index)
      {
        stringBuilder.Append(BitConverter.ToUInt16(data, index * 2));
        if (index != data.Length / 2 - 1)
          stringBuilder.Append(" ");
      }
      stringBuilder.Append("\r\n");
      return Encoding.ASCII.GetBytes(stringBuilder.ToString());
    }

    private byte[] GetDoubleWordResponseData(byte[] data)
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < data.Length / 4; ++index)
      {
        stringBuilder.Append(BitConverter.ToUInt32(data, index * 4));
        if (index != data.Length / 4 - 1)
          stringBuilder.Append(" ");
      }
      stringBuilder.Append("\r\n");
      return Encoding.ASCII.GetBytes(stringBuilder.ToString());
    }

    private byte[] ReadFromNanoCore(byte[] receive)
    {
      string[] command = Encoding.ASCII.GetString(receive).Trim('\r', '\n').Split(new char[1]
      {
        ' '
      }, StringSplitOptions.RemoveEmptyEntries);
      if (command[0] == "ER")
        return Encoding.ASCII.GetBytes("OK\r\n");
      if (command[0] == "RD" || command[0] == "RDS")
        return this.ReadByCommand(command);
      if (command[0] == "WR" || command[0] == "WRS")
        return this.WriteByCommand(command);
      if (command[0] == "ST")
        return this.WriteByCommand(new string[4]
        {
          "WRS",
          command[1],
          "1",
          "1"
        });
      if (command[0] == "RS")
        return this.WriteByCommand(new string[4]
        {
          "WRS",
          command[1],
          "1",
          "0"
        });
      if (command[0] == "?K")
        return Encoding.ASCII.GetBytes("53\r\n");
      return command[0] == "?M" ? Encoding.ASCII.GetBytes("1\r\n") : Encoding.ASCII.GetBytes("E0\r\n");
    }

    private byte[] ReadByCommand(string[] command)
    {
      try
      {
        if (command[1].EndsWith(".U") || command[1].EndsWith(".S") || (command[1].EndsWith(".D") || command[1].EndsWith(".L")) || command[1].EndsWith(".H"))
          command[1] = command[1].Remove(command[1].Length - 2);
        int length = command.Length > 2 ? int.Parse(command[2]) : 1;
        if (length > 256)
          return Encoding.ASCII.GetBytes("E0\r\n");
        if (Regex.IsMatch(command[1], "^[0-9]+$"))
          command[1] = "R" + command[1];
        if (command[1].StartsWith("R"))
          return this.GetBoolResponseData(this.rBuffer.GetBytes(int.Parse(command[1].Substring(1)), length));
        if (command[1].StartsWith("B"))
          return this.GetBoolResponseData(this.bBuffer.GetBytes(int.Parse(command[1].Substring(1)), length));
        if (command[1].StartsWith("MR"))
          return this.GetBoolResponseData(this.mrBuffer.GetBytes(int.Parse(command[1].Substring(2)), length));
        if (command[1].StartsWith("LR"))
          return this.GetBoolResponseData(this.lrBuffer.GetBytes(int.Parse(command[1].Substring(2)), length));
        if (command[1].StartsWith("CR"))
          return this.GetBoolResponseData(this.crBuffer.GetBytes(int.Parse(command[1].Substring(2)), length));
        if (command[1].StartsWith("VB"))
          return this.GetBoolResponseData(this.vbBuffer.GetBytes(int.Parse(command[1].Substring(2)), length));
        if (command[1].StartsWith("DM"))
          return this.GetWordResponseData(this.dmBuffer.GetBytes(int.Parse(command[1].Substring(2)) * 2, length * 2));
        if (command[1].StartsWith("EM"))
          return this.GetWordResponseData(this.emBuffer.GetBytes(int.Parse(command[1].Substring(2)) * 2, length * 2));
        if (command[1].StartsWith("W"))
          return this.GetWordResponseData(this.wBuffer.GetBytes(int.Parse(command[1].Substring(1)) * 2, length * 2));
        return command[1].StartsWith("AT") ? this.GetDoubleWordResponseData(this.atBuffer.GetBytes(int.Parse(command[1].Substring(2)) * 4, length * 4)) : Encoding.ASCII.GetBytes("E0\r\n");
      }
      catch
      {
        return Encoding.ASCII.GetBytes("E1\r\n");
      }
    }

    private byte[] WriteByCommand(string[] command)
    {
      if (!this.EnableWrite)
        return Encoding.ASCII.GetBytes("E4\r\n");
      try
      {
        if (command[1].EndsWith(".U") || command[1].EndsWith(".S") || (command[1].EndsWith(".D") || command[1].EndsWith(".L")) || command[1].EndsWith(".H"))
          command[1] = command[1].Remove(command[1].Length - 2);
        if ((command[0] == "WRS" ? int.Parse(command[2]) : 1) > 256)
          return Encoding.ASCII.GetBytes("E0\r\n");
        if (Regex.IsMatch(command[1], "^[0-9]+$"))
          command[1] = "R" + command[1];
        if (command[1].StartsWith("R") || command[1].StartsWith("B") || (command[1].StartsWith("MR") || command[1].StartsWith("LR")) || command[1].StartsWith("CR") || command[1].StartsWith("VB"))
        {
          byte[] array = ((IEnumerable<string>) command.RemoveBegin<string>(command[0] == "WRS" ? 3 : 2)).Select<string, byte>((Func<string, byte>) (m => byte.Parse(m))).ToArray<byte>();
          if (command[1].StartsWith("R"))
            this.rBuffer.SetBytes(array, int.Parse(command[1].Substring(1)));
          else if (command[1].StartsWith("B"))
            this.bBuffer.SetBytes(array, int.Parse(command[1].Substring(1)));
          else if (command[1].StartsWith("MR"))
            this.mrBuffer.SetBytes(array, int.Parse(command[1].Substring(2)));
          else if (command[1].StartsWith("LR"))
            this.lrBuffer.SetBytes(array, int.Parse(command[1].Substring(2)));
          else if (command[1].StartsWith("CR"))
          {
            this.crBuffer.SetBytes(array, int.Parse(command[1].Substring(2)));
          }
          else
          {
            if (!command[1].StartsWith("VB"))
              return Encoding.ASCII.GetBytes("E0\r\n");
            this.vbBuffer.SetBytes(array, int.Parse(command[1].Substring(2)));
          }
        }
        else
        {
          byte[] data = this.ByteTransform.TransByte(((IEnumerable<string>) command.RemoveBegin<string>(command[0] == "WRS" ? 3 : 2)).Select<string, ushort>((Func<string, ushort>) (m => ushort.Parse(m))).ToArray<ushort>());
          if (command[1].StartsWith("DM"))
            this.dmBuffer.SetBytes(data, int.Parse(command[1].Substring(2)) * 2);
          else if (command[1].StartsWith("EM"))
            this.emBuffer.SetBytes(data, int.Parse(command[1].Substring(2)) * 2);
          else if (command[1].StartsWith("W"))
          {
            this.wBuffer.SetBytes(data, int.Parse(command[1].Substring(1)) * 2);
          }
          else
          {
            if (!command[1].StartsWith("AT"))
              return Encoding.ASCII.GetBytes("E0\r\n");
            this.atBuffer.SetBytes(data, int.Parse(command[1].Substring(2)) * 4);
          }
        }
        return Encoding.ASCII.GetBytes("OK\r\n");
      }
      catch
      {
        return Encoding.ASCII.GetBytes("E1\r\n");
      }
    }

    /// <inheritdoc />
    public override string ToString() => string.Format("KeyenceNanoServer[{0}]", (object) this.Port);
  }
}
