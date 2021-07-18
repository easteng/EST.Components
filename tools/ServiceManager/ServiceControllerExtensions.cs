/**********************************************************************
*******命名空间： ZYServiceTool
*******类 名 称： ServiceControllerExtensions
*******类 说 明： 服务扩展帮助类
*******作    者： 东腾 Easten
*******机器名称： LAPTOP-SKVTCBH0
*******CLR 版本： 4.0.30319.42000
*******创建时间： 2019-08-06 15:24:27
***********************************************************************
******* ★ Copyright @Easten 2019-2020. All rights reserved ★ *********
***********************************************************************
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ZYServiceTool
{
    public static class ServiceControllerExtensions
    {
        
        private static ManagementObject GetNewWmiManagementObject(this ServiceController controller)
        {
            return new ManagementObject(new ManagementPath($"\\\\{controller.MachineName}\\root\\cimv2:Win32_Service.Name='{controller.ServiceName}'"));
        }
        public static void SetStartupType(this ServiceController controller, ServiceStartMode newType)
        {
            using (var vmiManagemantObj = controller.GetNewWmiManagementObject())
            {
                var parameter = new object[1];
                parameter[0] = newType.ToString();
                vmiManagemantObj.InvokeMethod("ChangeStartMode", parameter);
            }
        }

#if NET45
        //NET45 PolyFil as controller doesn't have StartType
        public static string GetStartupType(this ServiceController controller)
        {
            using (var wmiManagementObject = controller.GetNewWmiManagementObject())
            {
                return wmiManagementObject["StartMode"].ToString();
            }
        }
#elif NET461
        public static string GetStartupType(this ServiceController controller)
        {
            return controller.StartType.ToString();
        }
#endif
    }
}
