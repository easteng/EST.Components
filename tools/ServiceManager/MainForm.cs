using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZYServiceCore;

namespace ZYServiceTool
{
    /// <summary>
    /// 功能需求：
    /// 1、首先获取每一个服务的详细信息。
    /// 2、通过服务列表判断其可执行文件是否存在。
    /// 3、检查完可执行文件后判断当前系统服务中有无当前服务，若服务已存在，判断当前服务的路径与实际可执行文件的路径是否相同，
    ///    若服务名相同路径不动，则将服务的状态改为禁用，并修改备注信息为：服务路径有误，请重新安装
    /// </summary>
    public partial class MainForm :Form
    {
        private IDBProvider _resp;
        private string machineHostname;
        private bool s_installInprocess;
        public MainForm(string machine)
        {
            InitializeComponent();
            dataGridView.AutoGenerateColumns = false;
            machineHostname = machine;

        }
        private List<ViewModel> viewList;
        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadData();
           
            var timer2 = new System.Timers.Timer(60 * 1000);
            timer2.Elapsed += Timer2_Elapsed;
            timer2.Enabled = true;
        }

        private void Timer2_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            CheckService();
        }

        /// <summary>
        /// 定时刷新服务
        /// </summary>
        private void CheckService()
        {
            var services = LoadService();
            viewList?.ForEach(async act => act.CheckService(services,(a) =>
            {
                if (a) dataGridView.BeginInvoke((Action)(() => { dataGridView.Refresh(); }));
            }));
        }

        private void LoadData()
        {
            _resp = Easten.Resolve<IDBProvider>();
            var list = _resp.GetAllList<ServiceModel>();
            viewList = new List<ViewModel>();
            list.ForEach(a =>
            {
                viewList.Add(new ViewModel(a));
            });
            dataGridView.DataSource = viewList;
        }
        private void RibbonProgressBar1_ValueChanged(object sender, EventArgs e)
        {


        }



        private ServiceController[] LoadService()
        {
            return ServiceController.GetServices(machineHostname);
        }

        private void ChangeToolbarState(bool state)
        {
            this.toolStrip.Enabled = state;
            dataGridView.Refresh();
        }



        private void InstallationProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data))
                return;
        }


        private async Task PreformOperationWithCheck(Func<IReadOnlyCollection<ViewModel>, bool> check, Func<ViewModel, Task> actionToPreform)
        {
            var selectServices = dataGridView.SelectedRows.OfType<DataGridViewRow>().Select(g => g.DataBoundItem).OfType<ViewModel>().ToList();
            await PreformOperation(check, actionToPreform, selectServices, true);
        }
        private async Task PerformOperation(Func<ViewModel, Task> actionToPerform)
        {
            await PreformOperationWithCheck(i => true, actionToPerform);
        }
        private async Task PreformOperation(Func<IReadOnlyCollection<ViewModel>, bool> check, Func<ViewModel, Task> actionToPreform, IReadOnlyCollection<ViewModel> serviceToAction, bool disableToolstrip)
        {
            if (check(serviceToAction))
            {
                await PreformAction(async () =>
                 {
                     async Task WrappedFunction(ViewModel model)
                     {
                         try
                         {
                             await actionToPreform(model);
                         }
                         catch (Exception e)
                         {
                             MessageBox.Show("服务处理异常");
                         }
                     }

                     var tasks = new List<Task>();
                     foreach (var model in serviceToAction)
                     {
                         tasks.Add(WrappedFunction(model));
                     }
                     await Task.WhenAll(tasks);
                 }, disableToolstrip);
            }
        }

        private async Task PreformAction(Func<Task> actionToPreform, bool disableToolstrip = true)
        {
            if (disableToolstrip)
            {
                foreach (ToolStripItem toolStrip in toolStrip.Items)
                {
                    toolStrip.Enabled = false;
                }
            }

            await actionToPreform();

            if (disableToolstrip)
            {
                foreach (ToolStripItem toolStrip in toolStrip.Items)
                {
                    toolStrip.Enabled = true;
                }
            }
        }

        #region 工具栏菜单事件
        private void ToolStripButton1_Click(object sender, EventArgs e)
        {
            //服务配置
            var _frm = new frm_ServiceConfig();
            if (_frm.ShowDialog() != DialogResult.OK)
                return;
        }

        private void ToolStripRefreshButton_Click(object sender, EventArgs e)
        {
            //LoadData();
            //刷新所有服务
            //if (viewList.Any())
            //{
            //    viewList.ForEach(a =>
            //    {
            //        var task = new Task(() => { a.Refresh().Wait(); });
            //        task.Start();
            //    });
            //}
            CheckService();
        }

        private void ToolStripButton2_Click(object sender, EventArgs e)
        {
            ChangeToolbarState(false);
            //服务校验
            //首先根据已有的服务名跟系统服务进行比对，如果存在则改变对应的服务显示状态
            var services = LoadService();
            viewList?.ForEach(async a => a.CheckService(services, (act) =>
            {
                if (act) dataGridView.Refresh();
            }));
            ChangeToolbarState(true);
        }
        private async void ToolStripStartButton_Click(object sender, EventArgs e)
        {
            //启动服务
            await PerformOperation(async x => await x.Start());
            CheckService();
        }
        private async void ToolStripPauseButton_Click(object sender, EventArgs e)
        {
            //暂停服务
            await PerformOperation(async x => await x.Pause());
            CheckService();
        }
        private async void ToolStripStopButton_Click(object sender, EventArgs e)
        {
            //停止服务
            await PerformOperation(async a => await a.Stop());
            CheckService();
        }
        private async void ToolStripRestartButton_Click(object sender, EventArgs e)
        {
            //重启服务
            await PerformOperation(async a => await a.Restart());
            CheckService();
        }
        private async void ToolStripDeleteButton_Click(object sender, EventArgs e)
        {
            //删除服务
            await PreformOperationWithCheck(a =>
            {
                if (a.Count > 1)
                    return MessageBox.Show($@"已选择 {a.Count} 个服务，确定要删除吗？", @"确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes;
                var service = a.FirstOrDefault();
                if (service.SerivceState == 1)
                {
                    return MessageBox.Show($"{service.ServiceName}未安装，无法执行删除操作", "提示") == DialogResult.OK; ;
                }
                if (service != null)
                {
                    return MessageBox.Show($@"确定要从window中删除 '{service.ServiceName}'", @"确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes;
                }
                return false;
            }, async x =>
            {
                await x.Delete();
                Thread.Sleep(500);
                dataGridView.Refresh();
            });

            CheckService();
        }
        private async void ToolStripInstallButton_Click(object sender, EventArgs e)
        {
            //安装注册服务
            if (s_installInprocess)
            {
                //有正在注册的服务
                MessageBox.Show("有正在注册的服务，请稍等...", "温馨提示");
                return;
            }

            s_installInprocess = true;
            Enabled = true;
            Refresh();
            var viewModel = new ViewModel();
            toolStrip.Enabled = false;
            await PreformOperationWithCheck(a =>
            {
                if (a.Count > 1)
                {
                    return MessageBox.Show("安装服务时只能选择一个", "温馨提示") == DialogResult.OK;
                }
                var service = a.FirstOrDefault();

                if (service.SerivceState != 1)
                    return MessageBox.Show($"{service.ServiceName}已经安装", "温馨提示") == DialogResult.OK;
                if (service != null)
                {
                    viewModel = service;
                    var serviceName = service.ServiceName;
                    var startModel = "auto";
                    var binPath = service.binPath;
                    var displayName = service.ServiceName;

                    // todo  新增服务描述命令
                    //   string updateDescriptionCommand = $"sc \\\\{webIp} description {serviceName} \"{description}\" ";  //更新描述

                    //创建安装文件
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = "sc.exe",
                        Arguments = $"create \"{serviceName}\" start={startModel} binPath=\"{binPath}\" DisplayName=\"{displayName}\"",
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        UseShellExecute = false
                    };

                    //执行进行安装
                    try
                    {
                        using (var installationProcess = Process.Start(startInfo))
                        {
                            installationProcess.OutputDataReceived += InstallationProcess_OutputDataReceived;
                            installationProcess.BeginOutputReadLine();
                            installationProcess.WaitForExit();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "服务安装过程遇到了问题", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                return true;
            }, async x =>
            {
                await x.Install();
                Thread.Sleep(500);
                dataGridView.Refresh();
            });

            s_installInprocess = false;
            Refresh();
            toolStrip.Enabled = true;
            //刷新服务
            //var services = LoadService();
            //viewModel.CheckService(services, (act) => {
            //    if (act) dataGridView.Refresh();
            //});
            CheckService();
        }
        #endregion

        private async void DataGridView_DoubleClick(object sender, EventArgs e)
        {
            //双击服务，执行安装
            //安装注册服务
            if (s_installInprocess)
            {
                //有正在注册的服务
                MessageBox.Show("有正在注册的服务，请稍等...", "温馨提示");
                return;
            }
            var viewModel = new ViewModel();
            s_installInprocess = true;
            Enabled = true;
            toolStrip.Enabled = false;
            await PreformOperationWithCheck(a =>
            {
                if (a.Count > 1)
                {
                    return MessageBox.Show("安装服务时只能选择一个", "温馨提示") == DialogResult.OK;
                }
                var service = a.FirstOrDefault();

                if (service.SerivceState != 1)
                    return MessageBox.Show($"{service.ServiceName}已经安装", "温馨提示") == DialogResult.OK;
                if (service != null)
                {
                    viewModel = service;
                    var serviceName = service.ServiceName;
                    var startModel = "auto";
                    var binPath = service.binPath;
                    var displayName = service.ServiceName;

                    //创建安装文件
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = "sc.exe",
                        Arguments = $"create \"{serviceName}\" start={startModel} binPath=\"{binPath}\" DisplayName=\"{displayName}\"",
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        UseShellExecute = false
                    };

                    //执行进行安装
                    try
                    {
                        using (var installationProcess = Process.Start(startInfo))
                        {
                            installationProcess.OutputDataReceived += InstallationProcess_OutputDataReceived;
                            installationProcess.BeginOutputReadLine();
                            installationProcess.WaitForExit();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "服务安装过程遇到了问题", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                return true;
            }, async x =>
            {
                await x.Install();
                Thread.Sleep(500);
                dataGridView.Refresh();
            });

            s_installInprocess = false;
            Refresh();
            toolStrip.Enabled = true;

            //刷新服务
            var services = LoadService();
            viewModel.CheckService(services,(act)=> {
                if (act) dataGridView.Refresh();
            });
        }

        #region ContextMenuStrip 菜单事件

        private async void ContextMenuStartItem_Click(object sender, EventArgs e)
        {
            //启动
            await PerformOperation(async x => await x.Start());
            dataGridView.Refresh();
        }

        private async void ContextMenuStopItem_Click(object sender, EventArgs e)
        {
            //停止
            //停止服务
            await PerformOperation(async a => await a.Stop());
            dataGridView.Refresh();
        }

        private async void ContextMenuRestartItem_Click(object sender, EventArgs e)
        {
            //重启
            await PerformOperation(async a => await a.Restart());
            dataGridView.Refresh();
        }

        private async void ContextMenuDeleteItem_Click(object sender, EventArgs e)
        {
            //卸载
            await PreformOperationWithCheck(a =>
            {
                if (a.Count > 1)
                    return MessageBox.Show($@"已选择 {a.Count} 个服务，确定要删除吗？", @"确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes;
                var service = a.FirstOrDefault();
                if (service.SerivceState == 1)
                {
                    return MessageBox.Show($"{service.ServiceName}未安装，无法执行删除操作", "提示") == DialogResult.OK; ;
                }
                if (service != null)
                {
                    return MessageBox.Show($@"确定要从window中删除 '{service.ServiceName}'", @"确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes;
                }
                return false;
            }, async x =>
            {
                await x.Delete();
                Thread.Sleep(500);
                dataGridView.Refresh();
            });
        }

        private async void ContextStatupTypeAutomatic_Click(object sender, EventArgs e)
        {
            //自动
            await PerformOperation(async x => await x.SetStartupType(ServiceStartMode.Automatic));
        }

        private async void ContextStartupTypeManual_Click(object sender, EventArgs e)
        {
            //手动
            await PerformOperation(async x => await x.SetStartupType(ServiceStartMode.Manual));
        }

        private async void ContextStartupTypeDisabled_Click(object sender, EventArgs e)
        {
            //禁用
            await PerformOperation(async x => await x.SetStartupType(ServiceStartMode.Disabled));
        }

        private async void ContextMenuRefreshItem_Click(object sender, EventArgs e)
        {
            //刷新，只刷新当前的服务
            await PreformOperationWithCheck(a =>
            {
                var service = a.FirstOrDefault();
                if (service.SerivceState == 1)
                {
                    return MessageBox.Show($"{service.ServiceName}未安装，无法执行删除操作", "提示") == DialogResult.OK; ;
                }
                if (service != null)
                {
                    service.Refresh();
                }
                return false;
            }, async x =>
            {
                await x.Refresh();
                Thread.Sleep(500);
                dataGridView.Refresh();
            });
        }

        #endregion

        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Visible = true;
            this.notifyIcon1.Visible = true;
        }

        private void ToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            try
            {
                //直接关闭系统进程

                if (MessageBox.Show("确定要关闭正元服务管理工具吗？", "温馨提示", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    this.Close();
                    Application.Exit();
                    var process = Process.GetProcessesByName("ZYServiceTool").FirstOrDefault();
                    if (process != null)
                        process.Kill();
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            CheckService(); 
            
        }
    }
}
