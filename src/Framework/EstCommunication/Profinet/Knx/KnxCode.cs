// Decompiled with JetBrains decompiler
// Type: EstCommunication.Profinet.Knx.KnxCode
// Assembly: EstCommunication, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\EstCommunication.dll

using System;
using System.Net;

namespace EstCommunication.Profinet.Knx
{
  /// <summary>Knx协议</summary>
  public class KnxCode
  {
    private bool is_fresh = false;

    /// <summary>返回需要写入KNX总线的应答报文（应答数据）</summary>
    public event KnxCode.ReturnData Return_data_msg;

    /// <summary>返回需要写入的KNX系统的报文（写入数据）</summary>
    public event KnxCode.ReturnData Set_knx_data;

    /// <summary>返回从knx系统得到的数据</summary>
    public event KnxCode.GetData GetData_msg;

    /// <summary>序号计数</summary>
    public byte SequenceCounter { get; set; }

    /// <summary>通道</summary>
    public byte Channel { get; set; }

    /// <summary>连接状态</summary>
    public bool IsConnect { get; private set; }

    /// <summary>关闭KNX连接</summary>
    /// <param name="channel">通道号</param>
    /// <param name="IP_PROT">本机IP</param>
    /// <returns></returns>
    public byte[] Disconnect_knx(byte channel, IPEndPoint IP_PROT)
    {
      byte[] addressBytes = IP_PROT.Address.GetAddressBytes();
      byte[] bytes = BitConverter.GetBytes(IP_PROT.Port);
      return new byte[16]
      {
        (byte) 6,
        (byte) 16,
        (byte) 2,
        (byte) 9,
        (byte) 0,
        (byte) 16,
        channel,
        (byte) 0,
        (byte) 8,
        (byte) 1,
        addressBytes[0],
        addressBytes[1],
        addressBytes[2],
        addressBytes[3],
        bytes[1],
        bytes[0]
      };
    }

    /// <summary>返回握手报文</summary>
    /// <param name="IP_PROT">本机ip地址</param>
    /// <returns></returns>
    public byte[] Handshake(IPEndPoint IP_PROT)
    {
      byte[] addressBytes = IP_PROT.Address.GetAddressBytes();
      byte[] bytes = BitConverter.GetBytes(IP_PROT.Port);
      return new byte[26]
      {
        (byte) 6,
        (byte) 16,
        (byte) 2,
        (byte) 5,
        (byte) 0,
        (byte) 26,
        (byte) 8,
        (byte) 1,
        addressBytes[0],
        addressBytes[1],
        addressBytes[2],
        addressBytes[3],
        bytes[1],
        bytes[0],
        (byte) 8,
        (byte) 1,
        addressBytes[0],
        addressBytes[1],
        addressBytes[2],
        addressBytes[3],
        bytes[1],
        bytes[0],
        (byte) 4,
        (byte) 4,
        (byte) 2,
        (byte) 0
      };
    }

    /// <summary>KNX报文解析</summary>
    /// <param name="in_data"></param>
    public void KNX_check(byte[] in_data)
    {
      switch (in_data[2])
      {
        case 2:
          this.KNX_serverOF_2(in_data);
          break;
        case 4:
          this.KNX_serverOF_4(in_data);
          break;
      }
    }

    /// <summary>写入数据到KNX系统</summary>
    /// <param name="addr">地址</param>
    /// <param name="len">长度</param>
    /// <param name="data">数据</param>
    public void Knx_Write(short addr, byte len, byte[] data)
    {
      byte[] bytes1 = BitConverter.GetBytes(addr);
      byte[] data1 = new byte[20 + (int) len];
      byte[] bytes2 = BitConverter.GetBytes(data1.Length);
      if ((int) this.SequenceCounter + 1 <= (int) byte.MaxValue)
      {
        if (this.is_fresh)
          ++this.SequenceCounter;
        else
          this.is_fresh = true;
      }
      else
        this.SequenceCounter = (byte) 0;
      data1[0] = (byte) 6;
      data1[1] = (byte) 16;
      data1[2] = (byte) 4;
      data1[3] = (byte) 32;
      data1[4] = bytes2[1];
      data1[5] = bytes2[0];
      data1[6] = (byte) 4;
      data1[7] = this.Channel;
      data1[8] = this.SequenceCounter;
      data1[9] = (byte) 0;
      data1[10] = (byte) 17;
      data1[11] = (byte) 0;
      data1[12] = (byte) 188;
      data1[13] = (byte) 224;
      data1[14] = (byte) 0;
      data1[15] = (byte) 0;
      data1[16] = bytes1[1];
      data1[17] = bytes1[0];
      data1[18] = len;
      data1[19] = (byte) 0;
      if (len == (byte) 1)
      {
        byte[] bytes3 = BitConverter.GetBytes((int) data[0] & 63 | 128);
        data1[20] = bytes3[0];
      }
      else
      {
        data1[20] = (byte) 128;
        for (int index = 2; index <= (int) len; ++index)
          data1[(int) len - 1 + 20] = data[index - 2];
      }
      if (this.Set_knx_data == null)
        return;
      this.Set_knx_data(data1);
    }

    /// <summary>从KNX获取数据</summary>
    /// <param name="addr"></param>
    public void Knx_Resd_step1(short addr)
    {
      byte[] bytes1 = BitConverter.GetBytes(addr);
      byte[] data = new byte[21];
      byte[] bytes2 = BitConverter.GetBytes(data.Length);
      if ((int) this.SequenceCounter + 1 <= (int) byte.MaxValue)
      {
        if (this.is_fresh)
          ++this.SequenceCounter;
        else
          this.is_fresh = true;
      }
      else
        this.SequenceCounter = (byte) 0;
      data[0] = (byte) 6;
      data[1] = (byte) 16;
      data[2] = (byte) 4;
      data[3] = (byte) 32;
      data[4] = bytes2[1];
      data[5] = bytes2[0];
      data[6] = (byte) 4;
      data[7] = this.Channel;
      data[8] = this.SequenceCounter;
      data[9] = (byte) 0;
      data[10] = (byte) 17;
      data[11] = (byte) 0;
      data[12] = (byte) 188;
      data[13] = (byte) 224;
      data[14] = (byte) 0;
      data[15] = (byte) 0;
      data[16] = bytes1[1];
      data[17] = bytes1[0];
      data[18] = (byte) 1;
      data[19] = (byte) 0;
      data[20] = (byte) 0;
      if (this.Set_knx_data == null)
        return;
      this.Return_data_msg(data);
    }

    /// <summary>连接保持（每隔1s发送一次到设备）</summary>
    /// <param name="IP_PROT"></param>
    public void knx_server_is_real(IPEndPoint IP_PROT)
    {
      byte[] data = new byte[16];
      byte[] addressBytes = IP_PROT.Address.GetAddressBytes();
      byte[] bytes = BitConverter.GetBytes(IP_PROT.Port);
      data[0] = (byte) 6;
      data[1] = (byte) 16;
      data[2] = (byte) 2;
      data[3] = (byte) 7;
      data[4] = (byte) 0;
      data[5] = (byte) 16;
      data[6] = this.Channel;
      data[7] = (byte) 0;
      data[8] = (byte) 8;
      data[9] = (byte) 1;
      data[10] = addressBytes[0];
      data[11] = addressBytes[1];
      data[12] = addressBytes[2];
      data[13] = addressBytes[3];
      data[14] = bytes[1];
      data[15] = bytes[0];
      if (this.Return_data_msg == null)
        return;
      this.Return_data_msg(data);
    }

    /// <summary>暂时没有注释</summary>
    /// <param name="addr"></param>
    /// <param name="is_ok"></param>
    /// <returns></returns>
    public short Get_knx_addr(string addr, out bool is_ok)
    {
      short num1 = 0;
      string[] strArray = addr.Split('\\');
      if (strArray.Length == 3)
      {
        int num2 = int.Parse(strArray[0]);
        int num3 = int.Parse(strArray[1]);
        int num4 = int.Parse(strArray[2]);
        if (num2 > 31 || num3 > 7 || num4 > (int) byte.MaxValue || (num2 < 0 || num3 < 0 || num4 < 0))
        {
          Console.WriteLine("地址不合法");
          is_ok = false;
          return num1;
        }
        short num5 = (short) (num2 << 11 | num3 << 8 | num4);
        is_ok = true;
        return num5;
      }
      Console.WriteLine("地址不合法");
      is_ok = false;
      return num1;
    }

    private void KNX_serverOF_2(byte[] in_data)
    {
      switch (in_data[3])
      {
        case 6:
          this.Extraction_of_Channel(in_data);
          break;
        case 7:
          this.Return_status();
          break;
      }
    }

    /// <summary>返回连接状态</summary>
    private void Return_status()
    {
      byte[] data = new byte[8]
      {
        (byte) 6,
        (byte) 16,
        (byte) 2,
        (byte) 8,
        (byte) 0,
        (byte) 8,
        this.Channel,
        (byte) 0
      };
      if (this.Return_data_msg == null)
        return;
      this.Return_data_msg(data);
    }

    /// <summary>从握手回复报文获取通道号</summary>
    /// <param name="in_data"></param>
    private void Extraction_of_Channel(byte[] in_data)
    {
      this.Channel = in_data[6];
      if (in_data[5] == (byte) 8 & in_data[7] == (byte) 37)
        this.IsConnect = false;
      if (this.Channel <= (byte) 0)
        return;
      this.IsConnect = true;
    }

    private void KNX_serverOF_4(byte[] in_data)
    {
      switch (in_data[3])
      {
        case 32:
          this.Read_com_CEMI(in_data);
          break;
      }
    }

    /// <summary>解析控制包头和CEMI</summary>
    private void Read_com_CEMI(byte[] in_data) => this.Read_CEMI(in_data);

    /// <summary>具体解析CEMI</summary>
    private void Read_CEMI(byte[] in_data)
    {
      if (in_data.Length <= 11)
        return;
      switch (in_data[10])
      {
        case 41:
          this.Read_CEMI_29(in_data);
          break;
        case 46:
          this.Read_CEMI_2e(in_data);
          break;
      }
    }

    private void Read_CEMI_2e(byte[] in_data)
    {
      byte[] data = new byte[10]
      {
        (byte) 6,
        (byte) 16,
        (byte) 4,
        (byte) 33,
        (byte) 0,
        (byte) 10,
        (byte) 4,
        this.Channel,
        in_data[8],
        (byte) 0
      };
      if (this.Set_knx_data == null)
        return;
      this.Return_data_msg(data);
    }

    private void Read_CEMI_29(byte[] in_data)
    {
      short int16 = BitConverter.ToInt16(new byte[2]
      {
        in_data[17],
        in_data[16]
      }, 0);
      byte[] data;
      if (in_data[18] > (byte) 1)
      {
        data = new byte[(int) in_data[17]];
        for (int index = 0; index < (int) in_data[18] - 1; ++index)
          data[index] = in_data[21 + index];
      }
      else
        data = BitConverter.GetBytes((int) in_data[20] & 63);
      if (this.GetData_msg != null)
        this.GetData_msg(int16, in_data[18], data);
      this.Read_setp6(in_data);
    }

    private void Read_setp6(byte[] in_data)
    {
      byte[] data = new byte[10]
      {
        (byte) 6,
        (byte) 16,
        (byte) 4,
        (byte) 33,
        (byte) 0,
        (byte) 10,
        (byte) 4,
        this.Channel,
        in_data[8],
        (byte) 0
      };
      if (this.Return_data_msg == null)
        return;
      this.Return_data_msg(data);
    }

    /// <summary>返回数据的委托</summary>
    /// <param name="data"></param>
    public delegate void ReturnData(byte[] data);

    /// <summary>获取数据的委托</summary>
    /// <param name="addr"></param>
    /// <param name="len"></param>
    /// <param name="data"></param>
    public delegate void GetData(short addr, byte len, byte[] data);
  }
}
