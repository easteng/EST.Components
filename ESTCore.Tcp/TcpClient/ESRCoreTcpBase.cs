/**********************************************************************
*******命名空间： ESTCore.Tcp.TcpClient
*******类 名 称： ESRCoreTcpBase
*******类 说 明： 
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/29/2021 8:58:18 AM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @Easten 2020-2021. All rights reserved ★ *********
***********************************************************************
 */
using ESTCore.Common;
using ESTCore.Common.Core.Net;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESTCore.Tcp
{
    /// <summary>
    ///  tcp 客户端
    /// </summary>
    public class ESTcpClientBase
    {
        public ESTcpClientBase()
        {

        }
        protected  ESTModbusTcpClient tcpClient;
        private int connectErrorCount = 0;
        /// <summary>
        /// 创建一个服务连接
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="timeOut"></param>
        /// <param name="local"></param>
        /// <returns></returns>

        public OperateResult<ESTModbusTcpClient> CreateSocketAndConnect(
         IPEndPoint endPoint,
         int timeOut)
        {
            int num = 0;
            while (true)
            {
                ++num;
                tcpClient = new ESTModbusTcpClient(endPoint);
                EstTimeOut hslTimeOut = EstTimeOut.HandleTimeOutCheck(tcpClient.Socket, timeOut);
                try
                {
                    this.tcpClient.OptionKeepAlive = true;
                    this.tcpClient.Connect();
                    this.connectErrorCount = 0;
                   // hslTimeOut.IsSuccessful = true;
                    return OperateResult.CreateSuccessResult<ESTModbusTcpClient>(tcpClient);
                }
                catch (Exception ex)
                {
                    tcpClient?.Disconnect();
                   // hslTimeOut.IsSuccessful = true;
                    if (this.connectErrorCount < 1000000000)
                        ++this.connectErrorCount;
                    if (hslTimeOut.GetConsumeTime() < TimeSpan.FromMilliseconds(500.0) && num < 2)
                        Thread.Sleep(100);
                    else
                        return hslTimeOut.IsTimeout ? 
                            new OperateResult<ESTModbusTcpClient>(-this.connectErrorCount, string.Format(StringResources.Language.ConnectTimeout, (object)endPoint, (object)timeOut) + " ms") :
                            new OperateResult<ESTModbusTcpClient>(-this.connectErrorCount, string.Format("tcp Connect {0} Exception -> ", (object)endPoint) + ex.Message);
                }
            }
        }

    }
}
