/**********************************************************************
*******命名空间： ESTCore.Tcp.TcpClient
*******类 名 称： ESTModbusClient
*******类 说 明： modbus 客户端
*******作    者： Easten
*******机器名称： DESKTOP-EC8U0GP
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/29/2021 12:36:04 AM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @easten company 2021-2022. All rights reserved ★ *********
***********************************************************************
 */
using ESTCore.Common;
using ESTCore.Common.BasicFramework;
using ESTCore.Common.Core;
using ESTCore.Common.Core.Address;
using ESTCore.Common.ModBus;
using ESTCore.Common.Serial;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TcpClient = NetCoreServer.TcpClient;
namespace ESTCore.Tcp
{
    public class ESTModbusTcpClient: TcpClient
    {
        private Action<byte[]> currentReceiverDeleagte;
        /// <summary>
        /// 交互的混合锁，保证交互操作的安全性<br />
        /// Interactive hybrid locks to ensure the security of interactive operations
        /// </summary>
        protected SimpleHybirdLock InteractiveLock;
        private byte station = 1; //站点
        private byte operation = 0x03;
        private readonly SoftIncrementCount softIncrementCount; // 自增类
        IByteTransform ByteTransform;
        /// <summary>
        /// 重新连接时间，默认是1s
        /// </summary>
        public int ReConnectTime { get; set; } =1000;
        public bool Connected { get; set;  } // 是否连接到服务

        public ESTModbusTcpClient(IPEndPoint  iPEndPoint):base(iPEndPoint)
        {
            this.InteractiveLock = new SimpleHybirdLock();
            this.softIncrementCount = new SoftIncrementCount((long)ushort.MaxValue);
            this.station = (byte)1;
            this.ByteTransform = (IByteTransform)new ReverseWordTransform();
        }
        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            // 处理得到的数据
            if (buffer.Length > 5)
            {
                var list = buffer.ToList();
              
                if (list[0] != this.station && list[1] != this.operation)
                {
                    list.Remove(0);
                }
                int nLen = list[2];//数据域长度
                list = list.Take(nLen + 3 + 2).ToList();
                if (list.Count() < (nLen + 3 + 2)) return;
                // 进行数据校验
                byte[] buf_frame = new byte[nLen + 3 + 2];
                for (int i = 0; i < (nLen + 3 + 2); i++)
                {
                    buf_frame[i] = list[0];
                    list.RemoveAt(0);
                }
                var res = list.ToArray();

                Console.WriteLine(buf_frame.ToHexString());

            }
            this.currentReceiverDeleagte?.Invoke(buffer);
            base.OnReceived(buffer, offset, size);
        }

        /// <summary>
        /// 读取指定起始位，指定长度的数据
        /// </summary>
        /// <param name="adress">modbus 地址位</param>
        /// <param name="startIndex">数据读取起始位置</param>
        /// <param name="length">读取的树长度</param>
        /// <returns></returns>
        public OperateResult<byte[]> Read(byte adress,string startIndex,ushort length)
        {
            this.operation = (byte)3;
            // 实例化一个modbus 地址
            OperateResult<byte[][]> operateResult1 = ModbusInfo.BuildReadModbusCommand(startIndex, length, adress, true, (byte)3);
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult1);
            //this.Socket.se
            this.ReadFromCoreServer(ModbusInfo.PackCommandToRtu(operateResult1.Content[0]));
            return null;
        }

        public void ConnectServer()
        {
            this.Connect();
        }

        private string ReadFromCoreServer(byte[] command)
        {
            OperateResult<byte[]> operateResult = new OperateResult<byte[]>();
            this.InteractiveLock.Enter();
            var aaa = "01 03 00 00 00 02 C4 0B";
            var ccc = StringToHexByte(aaa);
            if (this.IsConnected)
            {
                // 连接到服务，执行命令发送
                this.SendAsync(ccc);
                this.currentReceiverDeleagte = res =>
                {
                    
                };
            }

            var ss = command.ToHexString();
            return ss;
        }
        public static byte[] StringToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if (hexString.Length % 2 != 0)
                hexString += " ";
            var returnBytes = new byte[hexString.Length / 2];
            for (var i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }
    }
}
