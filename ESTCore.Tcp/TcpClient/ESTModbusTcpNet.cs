/**********************************************************************
*******命名空间： ESTCore.Tcp.TcpClient
*******类 名 称： ESTcpNet
*******类 说 明： 
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/29/2021 9:43:31 AM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @Easten 2020-2021. All rights reserved ★ *********
***********************************************************************
 */
using ESTCore.Common;
using ESTCore.Common.ModBus;
using ESTCore.Common.Profinet.Freedom;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.Tcp
{
    /// <summary>
    ///  
    /// </summary>
    public class ESTModbusTcpNet
    {
        /// <summary>
        /// 站号，指采集器在485上的地址位  默认是1；
        /// </summary>
        public byte Station { get; set; }
        private FreedomTcpNet freedomTcp;
        byte functionCode = 0x01;  // 操作码
        public ESTModbusTcpNet()
        {
            this.Station = 0x01;
        }

        public void CreateAndConnectTcpClient(string ipAddress,int port)
        {
            this.freedomTcp = new FreedomTcpNet(ipAddress,port);
            this.freedomTcp.ConnectTimeOut = 200;
            this.freedomTcp.ConnectServer();
        }

        public OperateResult<byte[]> Read(string addr, ushort length)
        {
            this.functionCode = ModbusFunctionCode.ReadRegister;
            // 实例化一个modbus 地址
            OperateResult<byte[][]> operateResult1 = ModbusInfo.BuildReadModbusCommand(addr, length, this.Station, true, this.functionCode);
            if (!operateResult1.IsSuccess)
                return OperateResult.CreateFailedResult<byte[]>((OperateResult)operateResult1);
            // 添加校验
            var command = ModbusInfo.PackCommandToRtu(operateResult1.Content[0]);
            return this.freedomTcp.Read(command.ToHexString(),0);
        }
        //public OperateResult<byte[]> Write(string)
    }
}
