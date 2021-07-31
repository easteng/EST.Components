/**********************************************************************
*******命名空间： ESTCore.Tcp
*******类 名 称： OperationCode
*******类 说 明： 
*******作    者： Easten
*******机器名称： EASTEN
*******CLR 版本： 4.0.30319.42000
*******创建时间： 7/29/2021 9:59:38 AM
*******联系方式： 1301485237@qq.com
***********************************************************************
******* ★ Copyright @Easten 2020-2021. All rights reserved ★ *********
***********************************************************************
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESTCore.Tcp
{
    /// <summary>
    ///  modbus 功能码定义
    /// </summary>
    public class ModbusFunctionCode
    {
        /// <summary>
        /// 读取寄存
        /// </summary>
        public static byte ReadRegister = 0x03;
        /// <summary>
        /// 写入单个寄存器
        /// </summary>
        public static byte WriteRegister = 0x06;
        /// <summary>
        /// 写入多个寄存器
        /// </summary>
        public static byte WriteMulitpeRegister = 0x10;
    }
}
