using Masuit.Tools.Hardware;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
           
            long physicalMemory = SystemInfo.PhysicalMemory;// 获取物理内存总数
            long memoryAvailable = SystemInfo.MemoryAvailable;// 获取物理内存可用率
            double freePhysicalMemory = SystemInfo.GetFreePhysicalMemory();// 获取可用物理内存
            
            int cpuCount = SystemInfo.GetCpuCount();// 获取CPU核心数
            //IList<string> ipAddress = SystemInfo.GetIPAddress();// 获取本机所有IP地址
            //string localUsedIp = SystemInfo.GetLocalUsedIP();// 获取本机当前正在使用的IP地址
          //  IList<string> macAddress = SystemInfo.GetMacAddress();// 获取本机所有网卡mac地址
          //  string osVersion = SystemInfo.GetOsVersion();// 获取操作系统版本
            RamInfo ramInfo = SystemInfo.GetRamInfo();// 获取内存信息
            var cpuSN = SystemInfo.GetCpuInfo()[0].SerialNumber; // CPU序列号
                                                                 //var driveSN = SystemInfo.GetDiskInfo()[0].SerialNumber; // 硬盘序列号

            while (true)
            {
                double temperature = SystemInfo.GetCPUTemperature();// 获取CPU温度
                float load = SystemInfo.CpuLoad;// 获取CPU占用率
                Console.WriteLine(temperature);
                Thread.Sleep(1000);
            }
            Console.WriteLine("Hello World!");
            Console.Read();
        }
    }
}
