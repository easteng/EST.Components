/**********************************************************************
*******命名空间： ZY_ServiceTool
*******类 名 称： ViewModel
*******类 说 明： 表格绑定实体
*******作    者： 东腾 Easten
*******机器名称： MF5SK7EXLTJZGSI
*******CLR 版本： 4.0.30319.42000
*******创建时间： 2019-08-01 17:30:07
***********************************************************************
******* ★ Copyright @Easten 2019-2020. All rights reserved ★ *********
***********************************************************************
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;
using ZYServiceCore;
using System.Management;
using ZYServiceCore.Model;
using System.Diagnostics;

namespace ZYServiceTool
{
    public class ViewModel : INotifyPropertyChanged
    {

        private ServiceController _service { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ViewModel(ServiceController serviceController)
        {
            _service = serviceController;
        }
        public ServiceModel serviceModel { get; set; }
        public ViewModel()
        {
        }
        public ViewModel(ServiceModel serviceModel)
        {

            var _resp = Easten.Resolve<IDBProvider>();
            binPath = _resp.Get<ManageModel>("Folder").Value+"\\";
            Combindata(serviceModel);
        }
        /// <summary>
        /// 状态图标
        /// </summary>
        public Image ServiceIcon { get; set; }
        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; }
        /// <summary>
        /// 服务描述
        /// </summary>
        public string ServiceDesc { get; set; }
        /// <summary>
        /// 服务状态， 1:未安装。2：已停止。3：正在运行。4：禁用，5：启动中,6：停止中；
        /// </summary>
        public int SerivceState { get; set; }
        /// <summary>
        /// 状态描述，根据<see cref="SerivceState"/>的值进行判断
        /// </summary>
        public string StateDesc { get; set; }
        /// <summary>
        /// 服务的启动类型
        /// </summary>
        public string ServiceType { get; set; }

        /// <summary>
        /// 服务备注
        /// </summary>
        public string BZ { get; set; }

        public string binPath { get; set; }
        /// <summary>
        /// 数据处理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private ViewModel Combindata(ServiceModel model)
        {
            this.SerivceState = model.ServiceState;
            this.ServiceName = model.ServiceName;
            this.ServiceDesc = model.ServiceDesc;
            this.ServiceType = model.ServiceType;
            this.binPath = binPath+ model.BinPath;
            this.ServiceIcon = this.GetIcon(model.ServiceState);
            return this;
        }
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <returns></returns>
        public async Task Start()
        {
            if (_service.Status == ServiceControllerStatus.Stopped)
            {
                await Task.Run(() => _service.Start());
                var state = GetStateNum(_service);
                await Refresh(state, null);//刷新
            }else if (_service.Status == ServiceControllerStatus.Paused)
            {
                await Task.Run(() => _service.Continue());
                await Refresh(GetStateNum(_service), null);//刷新
            }
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <returns></returns>
        public async Task Stop()
        {
            try
            {
                if (_service.Status == ServiceControllerStatus.Running || _service.Status == ServiceControllerStatus.Paused)
                {
                    //停止前先获取改执行文件的进程信息,直接在进程中关闭
                    var process = Process.GetProcesses().ToList();
                    if (process != null)
                    {
                        process.ForEach(a =>
                        {
                            var pathName = "";
                            try
                            {
                                pathName = a.MainModule.FileName;
                            }
                            catch { }
                            if (pathName == binPath)
                            {
                                a.Kill();
                                _service.Refresh();
                            }
                        });
                    }
                    if (_service.CanStop)
                    {
                        await Task.Run(() => _service.Stop());
                    }
                    await Refresh(GetStateNum(_service), null);//刷新
                }
            }
            catch (Exception e)
            { 
            }
          
        }
        /// <summary>
        /// 暂停服务
        /// </summary>
        /// <returns></returns>
        public async Task Pause()
        {
            if (_service.Status == ServiceControllerStatus.Running)
            {
                if (_service.CanPauseAndContinue)
                {
                    await Task.Run(() => _service.Pause());
                    await Refresh(GetStateNum(_service), null);//刷新
                }
                else
                {
                    throw new Exception("服务无法暂停");
                }
            }
        }
        /// <summary>
        /// 重启服务
        /// </summary>
        /// <returns></returns>
        public async Task Restart()
        {
            if (_service.Status == ServiceControllerStatus.Running|| _service.Status == ServiceControllerStatus.Paused)
            {
                await Task.Run(() =>
                {
                    var process = Process.GetProcesses().ToList();
                    if (process != null)
                    {
                        process.ForEach(a =>
                        {
                            var pathName = "";
                            try
                            {
                                pathName = a.MainModule.FileName;
                            }
                            catch { }
                            if (pathName == binPath)
                            {
                                a.Kill();
                                _service.Refresh();
                            }
                        });
                    }
                    if (_service.CanStop)
                    {
                        _service.Stop();
                    }

                    _service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(60));

                    if (_service.Status != ServiceControllerStatus.Stopped)
                    {
                        throw new Exception("服务停止超时，请手动启动");
                    }
                    _service.Start();
                });
                await Refresh(GetStateNum(_service), null);//刷新
            }
        }

        /// <summary>
        /// 删除服务
        /// </summary>
        /// <returns></returns>
        public async Task Delete()
        {

            try
            {
                if (_service!= null&&(_service.Status == ServiceControllerStatus.Running|| _service.Status == ServiceControllerStatus.Stopped))
                {
                    try
                    {
                        var process = Process.GetProcesses().ToList();
                        if (process != null)
                        {
                            process.ForEach(a =>
                            {
                                var pathName = "";
                                try
                                {
                                    pathName = a.MainModule.FileName;
                                }
                                catch { }
                                if (pathName == binPath)
                                {
                                    a.Kill();
                                    _service.Refresh();
                                }
                            });
                        }
                        if (_service.CanStop)
                        {
                            await Task.Run(() => _service.Stop());
                        }
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                    await Task.Run(() => Process.Start("sc.exe", $"delete {this.ServiceName}"));

                    await Refresh(GetStateNum(_service), null);//刷新
                }
             
            }
            catch (Exception e)
            {
                throw e;
            }
           
        }
        /// <summary>
        /// 刷新服务
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task Refresh(string type = null,string bz=null)
        {
            var change=await Task.Run(() =>
            {
                var changedEvents = new List<string>();

                if (type != null)
                {
                    this.SerivceState = Convert.ToInt32(type);
                }

                if (bz != null)
                {
                    this.BZ = bz;
                }
                this.ServiceIcon = this.GetIcon(SerivceState);
                changedEvents.Add("SerivceState");
                changedEvents.Add("StateDesc");
                changedEvents.Add("BZ");
                return changedEvents;
            });

            change.ForEach(a =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(a));
            });
        }
        /// <summary>
        /// 安装服务
        /// </summary>
        /// <param name="type"></param>
        /// <param name="bz"></param>
        /// <returns></returns>
        public async Task Install(string type = null, string bz = null)
        {
            try
            {
                if (_service != null)
                {
                    _service.Refresh();
                }
            }
            catch (Exception ex)
            {

                throw new Exception("服务停止超时，请手动启动");
            }
           

        }
        /// <summary>
        /// 检查服务是否存在
        /// </summary>
        /// <returns></returns>
        public async void CheckService(ServiceController[] services,Action<bool> success)
        {
           await Task.Run(() =>
            {
                var currentSercice = services.FirstOrDefault(a => a.ServiceName == ServiceName);
                if (currentSercice == null)
                {
                    //服务不存在，修改状态为未安装
                    //更新状态类型
                    Refresh("1").Wait();
                }
                else
                {
                    _service = currentSercice;
                    //服务存在，进行判断路径是否相同
                    var path = GetprocessPathAsync(ServiceName);
                    var currentPath = this.binPath;
                    if (path.Contains(path))
                    {
                        //服务存在且路径相同，判断服务状态
                        var state = GetStateNum(_service);
                        Refresh(state).Wait();
                    }
                    else
                    {
                        //服务存在，但是路径不同，在备注字段中进行提示：服务路径与SmartCity路径不符。
                        // 判断当前的服务状态，并更新状态
                        var state = GetStateNum(_service);
                        Refresh(state, "服务路径与SmartCity路径不符").Wait();

                    }
                }

                success(true);
            });
           
        }

        private string GetServiceType(ServiceController service)
        {
            //if (service != null)
            //{
            //    //switch (service.ServiceStartMode)
            //    //{
            //    //    var a=ServiceType.
            //    //}
            //}

            return "";
        }
        /// <summary>
        /// 获取当前服务的状态类型
        /// </summary>
        /// <returns></returns>
        private string GetStateNum(ServiceController service)
        {
            if (service != null)
            {
                service.Refresh();
                switch (service.Status)
                {
                    case ServiceControllerStatus.Running:
                        return "3";
                    case ServiceControllerStatus.Stopped:
                        return "2";
                    case ServiceControllerStatus.StartPending:
                        return "5";
                    case ServiceControllerStatus.Paused:
                        return "4";
                    case ServiceControllerStatus.StopPending:
                        return "6";
                    default:
                        return "";
                }
            }
            return "1";
        }

        /// <summary>
        /// 设置启动模式
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public async Task SetStartupType(ServiceStartMode mode)
        {
            await Task.Run(() => _service.SetStartupType(mode));
            await Refresh(GetStateNum(_service), null);//刷新
        }
        public Image GetIcon(int state)
        {
            switch (state)
            {
                case 1:
                    this.StateDesc = "服务未安装";
                    return Properties.Resources.Running_State_Unknown;
                case 2:
                    this.StateDesc = "已停止";
                    return Properties.Resources.Running_State_Stopped;
                case 3:
                    this.StateDesc = "正在运行";
                    return Properties.Resources.Running_State_Running;
                case 4:
                    this.StateDesc = "已暂停";
                    return Properties.Resources.Running_State_Unknown;
                case 5:
                    this.StateDesc = "启动中...";
                    return Properties.Resources.Running_State_StartPending;
                case 6:
                    this.StateDesc = "停止中...";
                    return Properties.Resources.Running_State_StopPending;
                default:
                    this.StateDesc = "服务异常";
                    this.BZ = "服务已存在,单路径与SmartCity不符";
                    return Properties.Resources.Running_State_Unknown;
            }
        }

        private void UpdataState()
        {
            
        }

        private string GetprocessPathAsync(string serviceName)
        {
            try
            {
                var searcher = new ManagementObjectSearcher($"SELECT * FROM Win32_Service where Name='{serviceName}'");
                if (searcher.Get().Count > 0)
                {
                    var process = searcher.Get().Cast<ManagementObject>().FirstOrDefault();
                    return  process["PathName"].ToString();
                }
            }
            catch (Exception)
            {

            }
            return "";
        }
    }
}
