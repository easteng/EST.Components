using EstCommunication;
using EstCommunication.Core;
using EstCommunication.Enthernet;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemoteFileUpdate
{
    public partial class UpdateForm : Form
    {
        private string _projectName;
        private string _programName;
        private string _ip;
        private int _port;
        private string _token;

        private IntegrationFileClient integrationFileClient;
        public UpdateForm(string title,string projectName,string programName,string token,string ip,int port)
        {
            InitializeComponent();
            this.Text = title;
            this._programName = programName;
            this._projectName = projectName;
            this._ip = ip;
            this._token = token;
            this._port = port;
        }
       
        private void UpdateForm_Load(object sender, EventArgs e)
        {
            integrationFileClient = new IntegrationFileClient
            {
                ConnectTimeOut = 5000,
                ServerIpEndPoint = new System.Net.IPEndPoint(IPAddress.Parse(_ip),_port),
                Token = Guid.Parse(_token)
            };

            this.CheckFiles();
        }
        private string versionFilePath = Directory.GetCurrentDirectory() + "\\version.txt";
        // 检查当前的版本是不是最新的
        private void CheckFiles()
        {
            
            var files = this.integrationFileClient.DownloadPathFileNames("ESTDirectory", this._projectName, this._programName);

            // 根据当前程序运行目录下的文件名称和服务器端的比较
           
            if (File.Exists(versionFilePath))
            {
                // 文件存在，只对比和下载已经存在的文件
                var mapNames = files.Content.Select(a => a.MappingName).ToList();
                localItems = File.ReadAllLines(versionFilePath).Select(a => a).ToList();
                if (mapNames.All(localItems.Contains))
                {
                    this.panel1.Visible = false;
                    this.label1.Text = "当前已是最新版本！";
                }
                else
                {
                    // 有不同的版本，需要更新
                    var uploads = files.Content.Where(a => !localItems.Contains(a.MappingName)).ToArray();
                    // 删除已经不存在的id 信息
                    localItems = localItems.Where(a => mapNames.Contains(a)).ToList();
                    this.DownloadFile(uploads);
                }
            }
            else
            {
                // 文件不存在，直接下载
                this.DownloadFile(files.Content);
            }
        }
        List<string> localItems;
        private void DownloadFile(GroupFileItem[] files)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    if (localItems == null) localItems = new List<string>();
                    var total = files.Length;
                    var index = 0;
                    var versionStr = new StringBuilder();
                    var results = new List<OperateResult>();
                    foreach (var item in files)
                    {
                        label2.Invoke((Action)(() =>
                        {
                            label2.Text = $"({++index}/{total})";
                        }));

                        localItems.Add(item.MappingName);
                        var result = integrationFileClient.DownloadFile(
                            item.FileName,
                            "ESTDirectory",
                            this._projectName,
                            this._programName,
                            DownloadReportProgress,
                            Path.Combine(Directory.GetCurrentDirectory(),item.FileName)
                            );
                        results.Add(result);
                    }
                    Task.WaitAll();
                    // 切换ui
                    Invoke(new Action<List<OperateResult>>(a =>
                    {
                        if (results.Count() == files.Count())
                        {                           
                            File.WriteAllText(versionFilePath, string.Join("\r", localItems));
                            MessageBox.Show("更新完成！");
                            this.Close();
                            Application.Exit();
                        }
                    }), results);
                }
                catch (Exception ex)
                {
                }
              
             
            });
        }

        private void DownloadReportProgress(long receive, long totle)
        {
            BeginInvoke((Action)(() =>
            {
                int value = (int)(receive * 100L / totle);
                progressBar1.Value = value;
            }));
        }
    }
}
