using EstCommunication.Core;

using Newtonsoft.Json;

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

namespace RemoteFileManagerServer
{
    public partial class MainForm : Form
    {


        private RemoteFileManagerHandler remoteFileManagerHandler;
        public MainForm()
        {
            InitializeComponent();
            this.remoteFileManagerHandler = new RemoteFileManagerHandler();
            this.Load += MainForm_Load;
            this.FormClosed += MainForm_FormClosed;
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.remoteFileManagerHandler.CloseServer();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.treeView1.NodeMouseClick += TreeView1_NodeMouseClick;
        }

        private void TreeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // 展示项目信息
            this.textBox3.Clear();
            if (e.Node.Tag == null) return;
            var dirf = new DirectoryInfo(e.Node.Tag?.ToString());
            var file = dirf.GetFiles().FirstOrDefault(a => a.Name == "readme.txt");

            if (file != null)
            {
                var content = File.ReadAllText(file.FullName);
                this.textBox3.Text = content;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var projectDirectory = Path.Combine(Directory.GetCurrentDirectory(), "ESTProjects");
            if (!Directory.Exists(projectDirectory))
            {
                Directory.CreateDirectory(projectDirectory);
            }
            //D3A3690E-3D1A-4EC2-9F7C-1496CF5811FD
            this.button1.Enabled=!this.remoteFileManagerHandler.StartServer(int.Parse(textBox1.Text), projectDirectory, this.textBox2.Text);
            this.CreateProjectTree();
        }

        private void CreateProjectTree()
        {
            this.treeView1.Nodes.Clear();
            var mainPath= Path.Combine(Directory.GetCurrentDirectory(), "ESTProjects");

            var topNode = new TreeNode();
            topNode.Text = "EST项目列表";
            topNode.ExpandAll();
            ReadDirectory(mainPath, topNode);

            this.treeView1.Nodes.Add(topNode);
        }
        private void ReadDirectory(string path,TreeNode node)
        {
            var dirs = new DirectoryInfo(path);
            var dic = dirs.GetDirectories();
            if (dic.Any())
            {
                foreach (var item in dic)
                {
                    var n = new TreeNode();
                    n.Text = item.Name;
                    n.Tag = item.FullName;
                    ReadDirectory(item.FullName, n);
                    this.ReadDirectoryFiles(item.FullName, n);
                    node.Nodes.Add(n);
                }
            }
        }
        private void ReadDirectoryFiles(string path, TreeNode node)
        {
            var jsonContent = this.remoteFileManagerHandler.GetFiles(path);
            var list = JsonConvert.DeserializeObject< List<GroupFileItem>>(jsonContent);
            //var files = new DirectoryInfo(path).GetFiles();
            if (list.Any())
            {
                foreach (var item in list)
                {
                    var n = new TreeNode(item.FileName);
                    node.Nodes.Add(n);
                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.CreateProjectTree();
        }
    }
}
