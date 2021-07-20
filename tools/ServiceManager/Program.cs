using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommandLine;

using ZYServiceCore;

namespace ZYServiceTool
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] commandLine)
        {
            var process = System.Diagnostics.Process.GetProcessesByName("ZYServiceTool");
            if (process.Count()>1)
            {
                MessageBox.Show("服务管理工具已经打开，不要重复操作！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }
            Easten.Initialization();

            Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += (sender, args) => MessageBox.Show(args.Exception.Message, @"未知错误", MessageBoxButtons.OK);

            //判断当前启动是否是以管理员的形式启动
            var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);
            Parser.Default.ParseArguments<Options>(commandLine).WithParsed((options) =>
            {
                var machinName = options.Machine;
                if (string.IsNullOrEmpty(machinName))
                    machinName = Environment.MachineName;
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm(machinName));
            });
        }
    }

    public class Options
    {
        [Option('m', "machine", HelpText = "The machine to initially connect to.", Hidden = false, Required = false)]
        public string Machine { get; set; }
    }
}
