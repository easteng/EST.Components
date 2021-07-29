/**********************************************************************
*******命名空间： ESTCore.TcpTests.TcpClient
*******类 名 称： ESTModbusTcpClientTests
*******类 说 明： 
*******作    者： Easten
*******机器名称： DESKTOP-EC8U0GP
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/29/2021 1:12:41 AM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @easten company 2021-2022. All rights reserved ★ *********
***********************************************************************
 */
using ESTCore.Common;
using ESTCore.Common.ModBus;
using ESTCore.Tcp;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.TcpTests.TcpClient
{
    [TestClass]
    public class ESTModbusTcpClientTests
    {
       
        [TestMethod]

        public void Test()
        {
            var ipendPoint = new IPEndPoint(IPAddress.Parse("192.168.1.254"), 30003);
            ESTModbusTcpClient client = new ESTModbusTcpClient(ipendPoint);
            client.Connect();

            client.Read((byte)1, "00", 2);
        }

        public OperateResult<byte[]> Read(byte adress, string startIndex, ushort length)
        {
            // 实例化一个modbus 地址
            OperateResult<byte[][]> operateResult1 = ModbusInfo.BuildReadModbusCommand(startIndex, length, adress, true, (byte)3);
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult1);

            this.ReadFromCoreServer(ModbusInfo.PackCommandToRtu(operateResult1.Content[0]));

            return null;
        }

        private void ReadFromCoreServer(byte[] vs)
        {
           // throw new NotImplementedException();
        }
    }
}
