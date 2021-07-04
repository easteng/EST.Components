using EstCommunication;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemoteFilePusher
{
    public partial class MainForm : Form
    {
        private RemoteFileManagerHandler remoteFileManagerHandler;
        public MainForm()
        {
            InitializeComponent();
            this.remoteFileManagerHandler = new RemoteFileManagerHandler();
            this.remoteFileManagerHandler.UpdateProgress += RemoteFileManagerHandler_UpdateProgress;
            this.Load += MainForm_Load;
            this.treeView1.NodeMouseClick += TreeView1_NodeMouseClick;
        }

        private void TreeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var node = e.Node;
            if ((int)node.Tag == 1)
            {
                this.textBox4.Text = node.Text;
            }
            if ((int)node.Tag == 2)
            {
                this.textBox6.Text = node.Text;
            }
        }

        private void RemoteFileManagerHandler_UpdateProgress(object sender, UpdateInfo e)
        {
            // 此处代码是安全的
            int value = (int)(e.Sended * 100L / e.Totle);
            BeginInvoke((Action)(() =>
            {
                progressBar1.Value = value;
            }));

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.button1.Enabled = !this.remoteFileManagerHandler.ConnectionServer(this.textBox3.Text, int.Parse(this.textBox1.Text), this.textBox2.Text);
            Refresh();
        }

        private void Refresh()
        {
            this.treeView1.Nodes.Clear();
            // 获取远程服务器文件列表
            if (this.remoteFileManagerHandler != null)
            {
                var node = new TreeNode("所有项目");
                this.GetRemoteFolder(node);
                node.ExpandAll();
                this.treeView1.Nodes.Add(node);
            }
        }
        /// <summary>
        /// 获取远程项目
        /// </summary>
        /// <param name="name"></param>
        /// <param name="node"></param>
        private void GetRemoteFolder(TreeNode node, string project = "", string program = "")
        {
            try
            {
                var folders = this.remoteFileManagerHandler.GetRemoteFolders(project, program);
                if (folders.Any())
                {
                    this.treeView1.Nodes.Clear();
                    foreach (var folder in folders)
                    {
                        var n = new TreeNode(folder);
                        node.Nodes.Add(n);
                        GetRemoteFolder(n, project, folder);
                    }
                    node.Tag = 1;
                }
                else
                {
                    node.Tag = 2;
                }

            }
            catch (Exception ex)
            {

                Console.Write("运行出错");
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
        }
        private List<FileInfo> UploadFiles = new List<FileInfo>();
        private void button3_Click(object sender, EventArgs e)
        {
            // 选择文件
            var dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                foreach (var file in dialog.FileNames)
                {
                    var fileInfo = new FileInfo(file);
                    this.richTextBox2.AppendText(fileInfo.Name + "\n");
                    UploadFiles.Add(fileInfo);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // 上传文件
            if (UploadFiles.Any())
            {
                if (this.remoteFileManagerHandler.UploadFile(this.UploadFiles, textBox4.Text, textBox6.Text))
                {
                    MessageBox.Show("文件上传成功");
                };
            }
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            this.Refresh();
            this.UploadFiles.Clear();
            this.richTextBox2.Clear();
        }
    }
}
