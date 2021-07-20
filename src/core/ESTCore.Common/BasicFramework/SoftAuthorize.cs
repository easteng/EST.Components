// Decompiled with JetBrains decompiler
// Type: ESTCore.Common.BasicFramework.SoftAuthorize
// Assembly: ESTCore.Common, Version=9.7.0.0, Culture=neutral, PublicKeyToken=cdb2261fa039ed67
// MVID: 5E8BF708-20B8-4DD6-9DCA-9D9885AC7B2C
// Assembly location: F:\开发\工具开发\hls2\ESTCore.Common.dll

using Newtonsoft.Json.Linq;

using System;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace ESTCore.Common.BasicFramework
{
    /// <summary>软件授权类</summary>
    public class SoftAuthorize : SoftFileSaveBase
    {
        /// <summary>注册码描述文本</summary>
        public static readonly string TextCode = "Code";
        private string machine_code = "";
        private const uint DFP_GET_VERSION = 475264;
        private const uint DFP_SEND_DRIVE_COMMAND = 508036;
        private const uint DFP_RECEIVE_DRIVE_DATA = 508040;
        private const uint GENERIC_READ = 2147483648;
        private const uint GENERIC_WRITE = 1073741824;
        private const uint FILE_SHARE_READ = 1;
        private const uint FILE_SHARE_WRITE = 2;
        private const uint CREATE_NEW = 1;
        private const uint OPEN_EXISTING = 3;

        /// <summary>实例化一个软件授权类</summary>
        /// <param name="UseAdmin">是否使用管理员模式</param>
        public SoftAuthorize(bool UseAdmin = false)
        {
            this.machine_code = SoftAuthorize.GetInfo(UseAdmin);
            this.LogHeaderText = nameof(SoftAuthorize);
        }

        /// <summary>最终的注册秘钥信息，注意是只读的。</summary>
        /// <remarks>时间：2018年9月1日 23:01:54，来自 洛阳-LYG 的建议，公开了本属性信息，只读。</remarks>
        public string FinalCode { get; private set; } = "";

        /// <summary>是否正式发行版，是的话就取消授权</summary>
        public bool IsReleaseVersion { get; set; } = false;

        /// <summary>指示是否加载过文件信息</summary>
        private bool HasLoadByFile { get; set; } = false;

        /// <summary>指示系统是否处于试用运行</summary>
        public bool IsSoftTrial { get; set; } = false;

        /// <summary>获取本机的机器码</summary>
        /// <returns>机器码字符串</returns>
        public string GetMachineCodeString() => this.machine_code;

        /// <summary>获取需要保存的数据内容</summary>
        /// <returns>实际保存的内容</returns>
        public override string ToSaveString() => new JObject()
    {
      {
        SoftAuthorize.TextCode,
        (JToken) new JValue(this.FinalCode)
      }
    }.ToString();

        /// <summary>从字符串加载数据</summary>
        /// <param name="content">文件存储的数据</param>
        public override void LoadByString(string content)
        {
            this.FinalCode = SoftBasic.GetValueFromJsonObject<string>(JObject.Parse(content), SoftAuthorize.TextCode, this.FinalCode);
            this.HasLoadByFile = true;
        }

        /// <summary>使用特殊加密算法加密数据</summary>
        public override void SaveToFile() => this.SaveToFile((Converter<string, string>)(m => SoftSecurity.MD5Encrypt(m)));

        /// <summary>使用特殊解密算法解密数据</summary>
        public override void LoadByFile() => this.LoadByFile((Converter<string, string>)(m => SoftSecurity.MD5Decrypt(m)));

        /// <summary>检查该注册码是否是正确的注册码</summary>
        /// <param name="code">注册码信息</param>
        /// <param name="encrypt">数据加密的方法，必须用户指定</param>
        /// <returns>是否注册成功</returns>
        public bool CheckAuthorize(string code, Func<string, string> encrypt)
        {
            if (code != encrypt(this.GetMachineCodeString()))
                return false;
            this.FinalCode = code;
            this.SaveToFile();
            return true;
        }

        /// <summary>检测授权是否成功</summary>
        /// <param name="encrypt">数据加密的方法，必须用户指定</param>
        /// <returns>是否成功授权</returns>
        public bool IsAuthorizeSuccess(Func<string, string> encrypt)
        {
            if (this.IsReleaseVersion || encrypt(this.GetMachineCodeString()) == this.FinalCode)
                return true;
            this.FinalCode = "";
            this.SaveToFile();
            return false;
        }

        /// <summary>获取本计算机唯一的机器码</summary>
        /// <returns>字符串形式的机器码</returns>
        public static string GetInfo(bool UseAdmin)
        {
            string str1 = "" + SoftAuthorize.HWID.BIOS + "|" + SoftAuthorize.HWID.CPU + "|" + SoftAuthorize.HWID.HDD + "|" + SoftAuthorize.HWID.BaseBoard + "|";
            if (SoftAuthorize.HWID.IsServer)
                str1 = str1 + SoftAuthorize.HWID.SCSI + "|";
            ManagementObject managementObject = new ManagementObject("win32_logicaldisk.deviceid=" + ("\"" + Environment.GetEnvironmentVariable("systemdrive") + "\""));
            managementObject.Get();
            string str2 = str1 + managementObject["VolumeSerialNumber"].ToString() + "|";
            using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = new ManagementClass("Win32_ComputerSystemProduct").GetInstances().GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    ManagementObject current = (ManagementObject)enumerator.Current;
                    str2 += (string)current.Properties["UUID"].Value;
                }
            }
            string s = str2 + "|";
            if (UseAdmin)
            {
                if (new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
                {
                    SoftAuthorize.HardDiskInfo hddInfo = SoftAuthorize.GetHddInfo();
                    s += hddInfo.SerialNumber;
                }
                else
                {
                    //Process.Start(new ProcessStartInfo()
                    //{
                    //  FileName = Application.ExecutablePath,
                    //  Verb = "runas"
                    //});
                    //Application.Exit();
                }
            }
            return SoftBasic.ByteToHexString(new SHA1CryptoServiceProvider().ComputeHash(Encoding.Unicode.GetBytes(s))).Substring(0, 25);
        }

        private static string GetWMIIdent(string Class, string Property)
        {
            string str = "";
            foreach (ManagementBaseObject instance in new ManagementClass(Class).GetInstances())
            {
                if ((str = instance.GetPropertyValue(Property) as string) != "")
                    break;
            }
            return str;
        }

        private static string GetWMIIdent(string Class, params string[] Propertys)
        {
            string ident = "";
            Array.ForEach<string>(Propertys, (Action<string>)(prop => ident = ident + SoftAuthorize.GetWMIIdent(Class, prop) + " "));
            return ident;
        }

        /// <summary>获得硬盘信息</summary>
        /// <param name="driveIndex">硬盘序号</param>
        /// <returns>硬盘信息</returns>
        /// <remarks>
        /// by sunmast for everyone
        /// thanks lu0 for his great works
        /// 在Windows Array8/ME中，S.M.A.R.T并不缺省安装，请将SMARTVSD.VXD拷贝到%SYSTEM%＼IOSUBSYS目录下。
        /// 在Windows 2000/2003下，需要Administrators组的权限。
        /// </remarks>
        /// <example>AtapiDevice.GetHddInfo()</example>
        public static SoftAuthorize.HardDiskInfo GetHddInfo(byte driveIndex = 0)
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32S:
                    throw new NotSupportedException("Win32s is not supported.");
                case PlatformID.Win32Windows:
                    return SoftAuthorize.GetHddInfoArrayx(driveIndex);
                case PlatformID.Win32NT:
                    return SoftAuthorize.GetHddInfoNT(driveIndex);
                case PlatformID.WinCE:
                    throw new NotSupportedException("WinCE is not supported.");
                default:
                    throw new NotSupportedException("Unknown Platform.");
            }
        }

        private static SoftAuthorize.HardDiskInfo GetHardDiskInfo(
          SoftAuthorize.IdSector phdinfo)
        {
            SoftAuthorize.HardDiskInfo hardDiskInfo = new SoftAuthorize.HardDiskInfo();
            SoftAuthorize.ChangeByteOrder(phdinfo.sModelNumber);
            hardDiskInfo.ModuleNumber = Encoding.ASCII.GetString(phdinfo.sModelNumber).Trim();
            SoftAuthorize.ChangeByteOrder(phdinfo.sFirmwareRev);
            hardDiskInfo.Firmware = Encoding.ASCII.GetString(phdinfo.sFirmwareRev).Trim();
            SoftAuthorize.ChangeByteOrder(phdinfo.sSerialNumber);
            hardDiskInfo.SerialNumber = Encoding.ASCII.GetString(phdinfo.sSerialNumber).Trim();
            hardDiskInfo.Capacity = phdinfo.ulTotalAddressableSectors / 2U / 1024U;
            return hardDiskInfo;
        }

        private static SoftAuthorize.HardDiskInfo GetHddInfoArrayx(byte driveIndex)
        {
            SoftAuthorize.GetVersionOutParams lpOutBuffer1 = new SoftAuthorize.GetVersionOutParams();
            SoftAuthorize.SendCmdInParams lpInBuffer = new SoftAuthorize.SendCmdInParams();
            SoftAuthorize.SendCmdOutParams lpOutBuffer2 = new SoftAuthorize.SendCmdOutParams();
            uint lpBytesReturned = 0;
            IntPtr file = SoftAuthorize.CreateFile("＼＼.＼Smartvsd", FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, FileAttributes.Normal, IntPtr.Zero);
            if (file == IntPtr.Zero)
                throw new Exception("Open smartvsd.vxd failed.");
            if (SoftAuthorize.DeviceIoControl(file, 475264U, IntPtr.Zero, 0U, ref lpOutBuffer1, (uint)Marshal.SizeOf<SoftAuthorize.GetVersionOutParams>(lpOutBuffer1), ref lpBytesReturned, IntPtr.Zero) == 0)
            {
                SoftAuthorize.CloseHandle(file);
                throw new Exception("DeviceIoControl failed:DFP_GET_VERSION");
            }
            if (((int)lpOutBuffer1.fCapabilities & 1) == 0)
            {
                SoftAuthorize.CloseHandle(file);
                throw new Exception("Error: IDE identify command not supported.");
            }
            lpInBuffer.irDriveRegs.bDriveHeadReg = ((uint)driveIndex & 1U) <= 0U ? (byte)160 : (byte)176;
            if (((ulong)lpOutBuffer1.fCapabilities & (ulong)(16 >> (int)driveIndex)) > 0UL)
            {
                SoftAuthorize.CloseHandle(file);
                throw new Exception(string.Format("Drive {0} is a ATAPI device, we don’t detect it", (object)((int)driveIndex + 1)));
            }
            lpInBuffer.irDriveRegs.bCommandReg = (byte)236;
            lpInBuffer.bDriveNumber = driveIndex;
            lpInBuffer.irDriveRegs.bSectorCountReg = (byte)1;
            lpInBuffer.irDriveRegs.bSectorNumberReg = (byte)1;
            lpInBuffer.cBufferSize = 512U;
            if (SoftAuthorize.DeviceIoControl(file, 508040U, ref lpInBuffer, (uint)Marshal.SizeOf<SoftAuthorize.SendCmdInParams>(lpInBuffer), ref lpOutBuffer2, (uint)Marshal.SizeOf<SoftAuthorize.SendCmdOutParams>(lpOutBuffer2), ref lpBytesReturned, IntPtr.Zero) == 0)
            {
                SoftAuthorize.CloseHandle(file);
                throw new Exception("DeviceIoControl failed: DFP_RECEIVE_DRIVE_DATA");
            }
            SoftAuthorize.CloseHandle(file);
            return SoftAuthorize.GetHardDiskInfo(lpOutBuffer2.bBuffer);
        }

        private static SoftAuthorize.HardDiskInfo GetHddInfoNT(byte driveIndex)
        {
            SoftAuthorize.GetVersionOutParams lpOutBuffer1 = new SoftAuthorize.GetVersionOutParams();
            SoftAuthorize.SendCmdInParams lpInBuffer = new SoftAuthorize.SendCmdInParams();
            SoftAuthorize.SendCmdOutParams lpOutBuffer2 = new SoftAuthorize.SendCmdOutParams();
            uint lpBytesReturned = 0;
            IntPtr file = SoftAuthorize.CreateFile("\\\\.\\PhysicalDrive0", FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, FileAttributes.Normal, IntPtr.Zero);
            if (file == IntPtr.Zero)
                throw new Exception("CreateFile faild.");
            if (SoftAuthorize.DeviceIoControl(file, 475264U, IntPtr.Zero, 0U, ref lpOutBuffer1, (uint)Marshal.SizeOf<SoftAuthorize.GetVersionOutParams>(lpOutBuffer1), ref lpBytesReturned, IntPtr.Zero) == 0)
            {
                SoftAuthorize.CloseHandle(file);
                throw new Exception(string.Format("Drive {0} may not exists.", (object)((int)driveIndex + 1)));
            }
            if (((int)lpOutBuffer1.fCapabilities & 1) == 0)
            {
                SoftAuthorize.CloseHandle(file);
                throw new Exception("Error: IDE identify command not supported.");
            }
            lpInBuffer.irDriveRegs.bDriveHeadReg = ((uint)driveIndex & 1U) <= 0U ? (byte)160 : (byte)176;
            if (((ulong)lpOutBuffer1.fCapabilities & (ulong)(16 >> (int)driveIndex)) > 0UL)
            {
                SoftAuthorize.CloseHandle(file);
                throw new Exception(string.Format("Drive {0} is a ATAPI device, we don’t detect it.", (object)((int)driveIndex + 1)));
            }
            lpInBuffer.irDriveRegs.bCommandReg = (byte)236;
            lpInBuffer.bDriveNumber = driveIndex;
            lpInBuffer.irDriveRegs.bSectorCountReg = (byte)1;
            lpInBuffer.irDriveRegs.bSectorNumberReg = (byte)1;
            lpInBuffer.cBufferSize = 512U;
            if (SoftAuthorize.DeviceIoControl(file, 508040U, ref lpInBuffer, (uint)Marshal.SizeOf<SoftAuthorize.SendCmdInParams>(lpInBuffer), ref lpOutBuffer2, (uint)Marshal.SizeOf<SoftAuthorize.SendCmdOutParams>(lpOutBuffer2), ref lpBytesReturned, IntPtr.Zero) == 0)
            {
                SoftAuthorize.CloseHandle(file);
                throw new Exception("DeviceIoControl failed: DFP_RECEIVE_DRIVE_DATA");
            }
            SoftAuthorize.CloseHandle(file);
            return SoftAuthorize.GetHardDiskInfo(lpOutBuffer2.bBuffer);
        }

        private static void ChangeByteOrder(byte[] charArray)
        {
            for (int index = 0; index < charArray.Length; index += 2)
            {
                byte num = charArray[index];
                charArray[index] = charArray[index + 1];
                charArray[index + 1] = num;
            }
        }

        /// <summary>执行打开/建立资源的功能。</summary>
        /// <param name="lpFileName">指定要打开的设备或文件的名称。</param>
        /// <param name="dwDesiredAccess">
        /// <para>Win32 常量，用于控制对设备的读访问、写访问或读/写访问的常数。内容如下表：
        /// <p><list type="table">
        /// <listheader>
        /// <term>名称</term>
        /// <description>说明</description>
        /// </listheader>
        /// <item>
        /// <term>GENERIC_READ</term><description>指定对设备进行读取访问。</description>
        /// </item>
        /// <item>
        /// <term>GENERIC_WRITE</term><description>指定对设备进行写访问。</description>
        /// </item>
        /// <item><term><b>0</b></term><description>如果值为零，则表示只允许获取与一个设备有关的信息。</description></item>
        /// </list></p>
        /// </para>
        /// </param>
        /// <param name="dwShareMode">指定打开设备时的文件共享模式</param>
        /// <param name="lpSecurityAttributes"></param>
        /// <param name="dwCreationDisposition">Win32 常量，指定操作系统打开文件的方式。内容如下表：
        /// <para><p>
        /// <list type="table">
        /// <listheader><term>名称</term><description>说明</description></listheader>
        /// <item>
        /// <term>CREATE_NEW</term>
        /// <description>指定操作系统应创建新文件。如果文件存在，则抛出 <see cref="T:System.IO.IOException" /> 异常。</description>
        /// </item>
        /// <item><term>CREATE_ALWAYS</term><description>指定操作系统应创建新文件。如果文件已存在，它将被改写。</description></item>
        /// </list>
        /// </p></para>
        /// </param>
        /// <param name="dwFlagsAndAttributes"></param>
        /// <param name="hTemplateFile"></param>
        /// <returns>使用函数打开的设备的句柄。</returns>
        /// <remarks>
        /// 本函数可以执行打开或建立文件、文件流、目录/文件夹、物理磁盘、卷、系统控制的缓冲区、磁带设备、
        /// 通信资源、邮件系统和命名管道。
        /// </remarks>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr CreateFile(
          string lpFileName,
          [MarshalAs(UnmanagedType.U4)] FileAccess dwDesiredAccess,
          [MarshalAs(UnmanagedType.U4)] FileShare dwShareMode,
          IntPtr lpSecurityAttributes,
          [MarshalAs(UnmanagedType.U4)] FileMode dwCreationDisposition,
          [MarshalAs(UnmanagedType.U4)] FileAttributes dwFlagsAndAttributes,
          IntPtr hTemplateFile);

        /// <summary>关闭一个指定的指针对象指向的设备。。</summary>
        /// <param name="hObject">要关闭的句柄 <see cref="T:System.IntPtr" /> 对象。</param>
        /// <returns>成功返回 <b>0</b> ，不成功返回非零值。</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int CloseHandle(IntPtr hObject);

        /// <summary>对设备执行指定的操作。</summary>
        /// <param name="hDevice">要执行操作的设备句柄。</param>
        /// <param name="dwIoControlCode">Win32 API 常数，输入的是以 <b>FSCTL_</b> 为前缀的常数，定义在
        /// <b>WinIoCtl.h</b> 文件内，执行此重载方法必须输入 <b>SMART_GET_VERSION</b> 。</param>
        /// <param name="lpInBuffer">当参数为指针时，默认的输入值是 <b>0</b> 。</param>
        /// <param name="nInBufferSize">输入缓冲区的字节数量。</param>
        /// <param name="lpOutBuffer">一个 <b>GetVersionOutParams</b> ，表示执行函数后输出的设备检查。</param>
        /// <param name="nOutBufferSize">输出缓冲区的字节数量。</param>
        /// <param name="lpBytesReturned">实际装载到输出缓冲区的字节数量。</param>
        /// <param name="lpOverlapped">同步操作控制，一般不使用，默认值为 <b>0</b> 。</param>
        /// <returns>非零表示成功，零表示失败。</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int DeviceIoControl(
          IntPtr hDevice,
          uint dwIoControlCode,
          IntPtr lpInBuffer,
          uint nInBufferSize,
          ref SoftAuthorize.GetVersionOutParams lpOutBuffer,
          uint nOutBufferSize,
          ref uint lpBytesReturned,
          [Out] IntPtr lpOverlapped);

        /// <summary>对设备执行指定的操作。</summary>
        /// <param name="hDevice">要执行操作的设备句柄。</param>
        /// <param name="dwIoControlCode">Win32 API 常数，输入的是以 <b>FSCTL_</b> 为前缀的常数，定义在
        /// <b>WinIoCtl.h</b> 文件内，执行此重载方法必须输入 <b>SMART_SEND_DRIVE_COMMAND</b> 或 <b>SMART_RCV_DRIVE_DATA</b> 。</param>
        /// <param name="lpInBuffer">一个 <b>SendCmdInParams</b> 结构，它保存向系统发送的查询要求具体命令的数据结构。</param>
        /// <param name="nInBufferSize">输入缓冲区的字节数量。</param>
        /// <param name="lpOutBuffer">一个 <b>SendCmdOutParams</b> 结构，它保存系统根据命令返回的设备相信信息二进制数据。</param>
        /// <param name="nOutBufferSize">输出缓冲区的字节数量。</param>
        /// <param name="lpBytesReturned">实际装载到输出缓冲区的字节数量。</param>
        /// <param name="lpOverlapped">同步操作控制，一般不使用，默认值为 <b>0</b> 。</param>
        /// <returns>非零表示成功，零表示失败。</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int DeviceIoControl(
          IntPtr hDevice,
          uint dwIoControlCode,
          ref SoftAuthorize.SendCmdInParams lpInBuffer,
          uint nInBufferSize,
          ref SoftAuthorize.SendCmdOutParams lpOutBuffer,
          uint nOutBufferSize,
          ref uint lpBytesReturned,
          [Out] IntPtr lpOverlapped);

        private class HWID
        {
            public static string BIOS => SoftAuthorize.GetWMIIdent("Win32_BIOS", "Manufacturer", "SerialNumber", "SMBIOSBIOSVersion", "IdentificationCode");

            public static string CPU => SoftAuthorize.GetWMIIdent("Win32_Processor", "ProcessorId", "UniqueId", "Name");

            public static string HDD => SoftAuthorize.GetWMIIdent("Win32_DiskDrive", "Model", "TotalHeads");

            public static string GPU => SoftAuthorize.GetWMIIdent("Win32_VideoController", "DriverVersion", "Name");

            public static string MAC => SoftAuthorize.GetWMIIdent("Win32_NetworkAdapterConfiguration", "MACAddress");

            public static string OS => SoftAuthorize.GetWMIIdent("Win32_OperatingSystem", "SerialNumber", "Name");

            public static string SCSI => SoftAuthorize.GetWMIIdent("Win32_SCSIController", "DeviceID", "Name");

            public static string BaseBoard => SoftAuthorize.GetWMIIdent("Win32_BaseBoard", "SerialNumber", "PartNumber");

            public static bool IsServer => SoftAuthorize.HWID.HDD.Contains("SCSI");
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct SendCmdInParams
        {
            public uint cBufferSize;
            public SoftAuthorize.IdeRegs irDriveRegs;
            public byte bDriveNumber;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] bReserved;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] dwReserved;
            public byte bBuffer;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct DriverStatus
        {
            public byte bDriverError;
            public byte bIDEStatus;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] bReserved;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public uint[] dwReserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct SendCmdOutParams
        {
            public uint cBufferSize;
            public SoftAuthorize.DriverStatus DriverStatus;
            public SoftAuthorize.IdSector bBuffer;
        }

        /// <summary>硬盘信息</summary>
        [Serializable]
        public struct HardDiskInfo
        {
            /// <summary>型号</summary>
            public string ModuleNumber;
            /// <summary>固件版本</summary>
            public string Firmware;
            /// <summary>序列号</summary>
            public string SerialNumber;
            /// <summary>容量，以M为单位</summary>
            public uint Capacity;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct GetVersionOutParams
        {
            public byte bVersion;
            public byte bRevision;
            public byte bReserved;
            public byte bIDEDeviceMap;
            public uint fCapabilities;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint[] dwReserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct IdeRegs
        {
            public byte bFeaturesReg;
            public byte bSectorCountReg;
            public byte bSectorNumberReg;
            public byte bCylLowReg;
            public byte bCylHighReg;
            public byte bDriveHeadReg;
            public byte bCommandReg;
            public byte bReserved;
        }

        [StructLayout(LayoutKind.Sequential, Size = 512, Pack = 1)]
        internal struct IdSector
        {
            public ushort wGenConfig;
            public ushort wNumCyls;
            public ushort wReserved;
            public ushort wNumHeads;
            public ushort wBytesPerTrack;
            public ushort wBytesPerSector;
            public ushort wSectorsPerTrack;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public ushort[] wVendorUnique;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] sSerialNumber;
            public ushort wBufferType;
            public ushort wBufferSize;
            public ushort wECCSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] sFirmwareRev;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
            public byte[] sModelNumber;
            public ushort wMoreVendorUnique;
            public ushort wDoubleWordIO;
            public ushort wCapabilities;
            public ushort wReserved1;
            public ushort wPIOTiming;
            public ushort wDMATiming;
            public ushort wBS;
            public ushort wNumCurrentCyls;
            public ushort wNumCurrentHeads;
            public ushort wNumCurrentSectorsPerTrack;
            public uint ulCurrentSectorCapacity;
            public ushort wMultSectorStuff;
            public uint ulTotalAddressableSectors;
            public ushort wSingleWordDMA;
            public ushort wMultiWordDMA;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public byte[] bReserved;
        }
    }
}
