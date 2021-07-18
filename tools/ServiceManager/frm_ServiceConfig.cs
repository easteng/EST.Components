using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZYServiceCore;
using ZYServiceCore.Model;
using System.IO;
namespace ZYServiceTool
{
    public partial class frm_ServiceConfig :Form
    {
        public frm_ServiceConfig()
        {
            InitializeComponent();
        }
        private IDBProvider _resp;
        private bool checkService = false;

        private ManageModel folder;
        private void Frm_ServiceConfig_Load(object sender, EventArgs e)
        {

            _resp=Easten.Resolve<IDBProvider>();

            folder = _resp.Get<ManageModel>("Folder");

            if (folder == null||folder.Value=="")
                MessageBox.Show("请选择SmartCity文件路径");
        }

        private void C1FlexGrid1_Click(object sender, EventArgs e)
        {

        }

        private void C1Button1_Click(object sender, EventArgs e)
        {
            //选择文件夹路径
            var fileDialog = new FolderBrowserDialog();

            if (fileDialog.ShowDialog() == DialogResult.OK) { }
            else
                return;
        }

        private void Btn_check_Click(object sender, EventArgs e)
        {
            ////可执行文件验证
            //if (string.IsNullOrEmpty(textBox1.Text))
            //{
            //    MessageBox.Show("请选择SmartCity目录");
            //    return;
            //}
            //else
            //{
            //    if (CheckExefileExits())
            //    {
            //        checkService = true;
            //        btn_save.Enabled = checkService = true;
            //        btn_check.Enabled = false;
            //        btn_check.Text = "验证成功";
            //    }
            //    else
            //    {
            //        MessageBox.Show(ErrorMessage.ToString());
            //    }
                
            //}
        }
        private StringBuilder ErrorMessage = new StringBuilder();
        private bool CheckExefileExits()
        {
            try
            {
                //ErrorMessage.Clear();
                //var folder = textBox1.Text;
                //var bins = _resp.GetAllList<ServiceModel>();
                //bins?.ForEach(a =>
                //{
                //    var path = string.Format("{0}\\{1}", folder, a.BinPath);

                //    if (!File.Exists(path))
                //    {
                //        ErrorMessage.AppendLine($"服务：{a.ServiceName}的文件路径与实际SmartCity路径不符\r\n");
                //    }
                //});
                //if (ErrorMessage.Length > 0)
                //    return false;
                //else
         return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void Btn_save_Click(object sender, EventArgs e)
        {
            //保存路径
            //if (folder != null&&!string.IsNullOrEmpty(textBox1.Text))
            //{
            //    //更新
            //    folder.Value = textBox1.Text;
            //    _resp.Updata<ManageModel>(folder);
            //    MessageBox.Show("保存成功");
            //    this.DialogResult = DialogResult.OK;
            //}
        }
    }
}
