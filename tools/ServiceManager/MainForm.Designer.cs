namespace ZYServiceTool
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripStartButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripPauseButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripStopButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripRestartButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripDeleteButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripInstallButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripRefreshButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripFilterIcon = new System.Windows.Forms.ToolStripLabel();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridStatusIcon = new System.Windows.Forms.DataGridViewImageColumn();
            this.ServiceName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ServiceDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ServiceType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ServiceState = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BZ = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.contextMenuStartItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStopItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuRestartItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuSpacer1 = new System.Windows.Forms.ToolStripSeparator();
            this.contextMenuDeleteItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStartupTypeItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextStatupTypeAutomatic = new System.Windows.Forms.ToolStripMenuItem();
            this.contextStartupTypeManual = new System.Windows.Forms.ToolStripMenuItem();
            this.contextStartupTypeDisabled = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuSpacer2 = new System.Windows.Forms.ToolStripSeparator();
            this.contextMenuRefreshItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuSpacer3 = new System.Windows.Forms.ToolStripSeparator();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.contextMenu.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.SuspendLayout();


            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // toolStrip
            // 
            this.toolStrip.AllowMerge = false;
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(25, 25);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripSeparator3,
            this.toolStripStartButton,
            this.toolStripPauseButton,
            this.toolStripStopButton,
            this.toolStripRestartButton,
            this.toolStripSeparator1,
            this.toolStripDeleteButton,
            this.toolStripInstallButton,
            this.toolStripSeparator2,
            this.toolStripRefreshButton,
            this.toolStripFilterIcon});
            this.toolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.toolStrip.Location = new System.Drawing.Point(0, 24);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolStrip.Size = new System.Drawing.Size(1125, 32);
            this.toolStrip.Stretch = true;
            this.toolStrip.TabIndex = 12;
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = global::ZYServiceTool.Properties.Resources.outlook_view_setting;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(61, 29);
            this.toolStripButton1.Text = "配置";
            this.toolStripButton1.Click += new System.EventHandler(this.ToolStripButton1_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.Image = global::ZYServiceTool.Properties.Resources.Info;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(61, 29);
            this.toolStripButton2.Text = "校验";
            this.toolStripButton2.Click += new System.EventHandler(this.ToolStripButton2_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.AutoSize = false;
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 29);
            // 
            // toolStripStartButton
            // 
            this.toolStripStartButton.Image = global::ZYServiceTool.Properties.Resources.Start;
            this.toolStripStartButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripStartButton.Name = "toolStripStartButton";
            this.toolStripStartButton.Size = new System.Drawing.Size(61, 29);
            this.toolStripStartButton.Text = "启动";
            this.toolStripStartButton.Click += new System.EventHandler(this.ToolStripStartButton_Click);
            // 
            // toolStripPauseButton
            // 
            this.toolStripPauseButton.Image = global::ZYServiceTool.Properties.Resources.Pause;
            this.toolStripPauseButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripPauseButton.Name = "toolStripPauseButton";
            this.toolStripPauseButton.Size = new System.Drawing.Size(61, 29);
            this.toolStripPauseButton.Text = "暂停";
            this.toolStripPauseButton.Click += new System.EventHandler(this.ToolStripPauseButton_Click);
            // 
            // toolStripStopButton
            // 
            this.toolStripStopButton.Image = global::ZYServiceTool.Properties.Resources.Stop;
            this.toolStripStopButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripStopButton.Name = "toolStripStopButton";
            this.toolStripStopButton.Size = new System.Drawing.Size(61, 29);
            this.toolStripStopButton.Text = "停止";
            this.toolStripStopButton.Click += new System.EventHandler(this.ToolStripStopButton_Click);
            // 
            // toolStripRestartButton
            // 
            this.toolStripRestartButton.Image = global::ZYServiceTool.Properties.Resources.Restart;
            this.toolStripRestartButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripRestartButton.Name = "toolStripRestartButton";
            this.toolStripRestartButton.Size = new System.Drawing.Size(61, 29);
            this.toolStripRestartButton.Text = "重启";
            this.toolStripRestartButton.ToolTipText = "Stop, wait, then start";
            this.toolStripRestartButton.Click += new System.EventHandler(this.ToolStripRestartButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.AutoSize = false;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 29);
            // 
            // toolStripDeleteButton
            // 
            this.toolStripDeleteButton.Image = global::ZYServiceTool.Properties.Resources.delete;
            this.toolStripDeleteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDeleteButton.Name = "toolStripDeleteButton";
            this.toolStripDeleteButton.Size = new System.Drawing.Size(61, 29);
            this.toolStripDeleteButton.Text = "卸载";
            this.toolStripDeleteButton.Click += new System.EventHandler(this.ToolStripDeleteButton_Click);
            // 
            // toolStripInstallButton
            // 
            this.toolStripInstallButton.Image = global::ZYServiceTool.Properties.Resources.Install;
            this.toolStripInstallButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripInstallButton.Name = "toolStripInstallButton";
            this.toolStripInstallButton.Size = new System.Drawing.Size(61, 29);
            this.toolStripInstallButton.Text = "安装";
            this.toolStripInstallButton.Click += new System.EventHandler(this.ToolStripInstallButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.AutoSize = false;
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 29);
            // 
            // toolStripRefreshButton
            // 
            this.toolStripRefreshButton.Image = global::ZYServiceTool.Properties.Resources.Refresh;
            this.toolStripRefreshButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripRefreshButton.Name = "toolStripRefreshButton";
            this.toolStripRefreshButton.Size = new System.Drawing.Size(61, 29);
            this.toolStripRefreshButton.Text = "刷新";
            this.toolStripRefreshButton.Click += new System.EventHandler(this.ToolStripRefreshButton_Click);
            // 
            // toolStripFilterIcon
            // 
            this.toolStripFilterIcon.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripFilterIcon.Name = "toolStripFilterIcon";
            this.toolStripFilterIcon.Size = new System.Drawing.Size(0, 0);
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToOrderColumns = true;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridStatusIcon,
            this.ServiceName,
            this.ServiceDesc,
            this.ServiceType,
            this.ServiceState,
            this.BZ});
            this.dataGridView.ContextMenuStrip = this.contextMenu;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dataGridView.Location = new System.Drawing.Point(0, 56);
            this.dataGridView.Margin = new System.Windows.Forms.Padding(7);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(2);
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.RowTemplate.Height = 25;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(1125, 564);
            this.dataGridView.TabIndex = 13;
            this.dataGridView.DoubleClick += new System.EventHandler(this.DataGridView_DoubleClick);
            // 
            // dataGridStatusIcon
            // 
            this.dataGridStatusIcon.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dataGridStatusIcon.DataPropertyName = "ServiceIcon";
            this.dataGridStatusIcon.FillWeight = 50.76142F;
            this.dataGridStatusIcon.HeaderText = "";
            this.dataGridStatusIcon.MinimumWidth = 30;
            this.dataGridStatusIcon.Name = "dataGridStatusIcon";
            this.dataGridStatusIcon.ReadOnly = true;
            this.dataGridStatusIcon.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridStatusIcon.Width = 30;
            // 
            // ServiceName
            // 
            this.ServiceName.DataPropertyName = "ServiceName";
            this.ServiceName.FillWeight = 71.85628F;
            this.ServiceName.HeaderText = "服务名称";
            this.ServiceName.Name = "ServiceName";
            this.ServiceName.ReadOnly = true;
            // 
            // ServiceDesc
            // 
            this.ServiceDesc.DataPropertyName = "ServiceDesc";
            this.ServiceDesc.FillWeight = 203.2609F;
            this.ServiceDesc.HeaderText = "服务描述";
            this.ServiceDesc.Name = "ServiceDesc";
            this.ServiceDesc.ReadOnly = true;
            // 
            // ServiceType
            // 
            this.ServiceType.DataPropertyName = "ServiceType";
            this.ServiceType.FillWeight = 55.6082F;
            this.ServiceType.HeaderText = "启动类型";
            this.ServiceType.Name = "ServiceType";
            this.ServiceType.ReadOnly = true;
            // 
            // ServiceState
            // 
            this.ServiceState.DataPropertyName = "StateDesc";
            this.ServiceState.FillWeight = 69.27463F;
            this.ServiceState.HeaderText = "状态";
            this.ServiceState.Name = "ServiceState";
            this.ServiceState.ReadOnly = true;
            // 
            // BZ
            // 
            this.BZ.DataPropertyName = "BZ";
            this.BZ.HeaderText = "备注";
            this.BZ.Name = "BZ";
            this.BZ.ReadOnly = true;
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contextMenuStartItem,
            this.contextMenuStopItem,
            this.contextMenuRestartItem,
            this.contextMenuSpacer1,
            this.contextMenuDeleteItem,
            this.contextMenuStartupTypeItem,
            this.contextMenuSpacer2,
            this.contextMenuRefreshItem,
            this.contextMenuSpacer3});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(125, 154);
            // 
            // contextMenuStartItem
            // 
            this.contextMenuStartItem.Image = global::ZYServiceTool.Properties.Resources.Start;
            this.contextMenuStartItem.Name = "contextMenuStartItem";
            this.contextMenuStartItem.Size = new System.Drawing.Size(124, 22);
            this.contextMenuStartItem.Text = "启动";
            this.contextMenuStartItem.Click += new System.EventHandler(this.ContextMenuStartItem_Click);
            // 
            // contextMenuStopItem
            // 
            this.contextMenuStopItem.Image = global::ZYServiceTool.Properties.Resources.Stop;
            this.contextMenuStopItem.Name = "contextMenuStopItem";
            this.contextMenuStopItem.Size = new System.Drawing.Size(124, 22);
            this.contextMenuStopItem.Text = "停止";
            this.contextMenuStopItem.Click += new System.EventHandler(this.ContextMenuStopItem_Click);
            // 
            // contextMenuRestartItem
            // 
            this.contextMenuRestartItem.Image = global::ZYServiceTool.Properties.Resources.Restart;
            this.contextMenuRestartItem.Name = "contextMenuRestartItem";
            this.contextMenuRestartItem.Size = new System.Drawing.Size(124, 22);
            this.contextMenuRestartItem.Text = "重启";
            this.contextMenuRestartItem.Click += new System.EventHandler(this.ContextMenuRestartItem_Click);
            // 
            // contextMenuSpacer1
            // 
            this.contextMenuSpacer1.Name = "contextMenuSpacer1";
            this.contextMenuSpacer1.Size = new System.Drawing.Size(121, 6);
            // 
            // contextMenuDeleteItem
            // 
            this.contextMenuDeleteItem.Image = global::ZYServiceTool.Properties.Resources.delete;
            this.contextMenuDeleteItem.Name = "contextMenuDeleteItem";
            this.contextMenuDeleteItem.Size = new System.Drawing.Size(124, 22);
            this.contextMenuDeleteItem.Text = "卸载";
            this.contextMenuDeleteItem.Click += new System.EventHandler(this.ContextMenuDeleteItem_Click);
            // 
            // contextMenuStartupTypeItem
            // 
            this.contextMenuStartupTypeItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contextStatupTypeAutomatic,
            this.contextStartupTypeManual,
            this.contextStartupTypeDisabled});
            this.contextMenuStartupTypeItem.Image = global::ZYServiceTool.Properties.Resources.Startup;
            this.contextMenuStartupTypeItem.Name = "contextMenuStartupTypeItem";
            this.contextMenuStartupTypeItem.Size = new System.Drawing.Size(124, 22);
            this.contextMenuStartupTypeItem.Text = "启动类型";
            // 
            // contextStatupTypeAutomatic
            // 
            this.contextStatupTypeAutomatic.Name = "contextStatupTypeAutomatic";
            this.contextStatupTypeAutomatic.Size = new System.Drawing.Size(100, 22);
            this.contextStatupTypeAutomatic.Text = "自动";
            this.contextStatupTypeAutomatic.Click += new System.EventHandler(this.ContextStatupTypeAutomatic_Click);
            // 
            // contextStartupTypeManual
            // 
            this.contextStartupTypeManual.Name = "contextStartupTypeManual";
            this.contextStartupTypeManual.Size = new System.Drawing.Size(100, 22);
            this.contextStartupTypeManual.Text = "手动";
            this.contextStartupTypeManual.Click += new System.EventHandler(this.ContextStartupTypeManual_Click);
            // 
            // contextStartupTypeDisabled
            // 
            this.contextStartupTypeDisabled.Name = "contextStartupTypeDisabled";
            this.contextStartupTypeDisabled.Size = new System.Drawing.Size(100, 22);
            this.contextStartupTypeDisabled.Text = "禁用";
            this.contextStartupTypeDisabled.Click += new System.EventHandler(this.ContextStartupTypeDisabled_Click);
            // 
            // contextMenuSpacer2
            // 
            this.contextMenuSpacer2.Name = "contextMenuSpacer2";
            this.contextMenuSpacer2.Size = new System.Drawing.Size(121, 6);
            // 
            // contextMenuRefreshItem
            // 
            this.contextMenuRefreshItem.Image = global::ZYServiceTool.Properties.Resources.Refresh;
            this.contextMenuRefreshItem.Name = "contextMenuRefreshItem";
            this.contextMenuRefreshItem.Size = new System.Drawing.Size(124, 22);
            this.contextMenuRefreshItem.Text = "刷新";
            this.contextMenuRefreshItem.Click += new System.EventHandler(this.ContextMenuRefreshItem_Click);
            // 
            // contextMenuSpacer3
            // 
            this.contextMenuSpacer3.Name = "contextMenuSpacer3";
            this.contextMenuSpacer3.Size = new System.Drawing.Size(121, 6);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon1.BalloonTipText = "物联网服务监控管理工具";
            this.notifyIcon1.BalloonTipTitle = "正元地理信息";
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip2;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "正元物联网服务\r\n管理工具";
            this.notifyIcon1.Visible = true;
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripSeparator4,
            this.toolStripMenuItem5,
            this.toolStripSeparator5,
            this.toolStripSeparator6});
            this.contextMenuStrip2.Name = "contextMenu";
            this.contextMenuStrip2.Size = new System.Drawing.Size(125, 66);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Image = global::ZYServiceTool.Properties.Resources.Start;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(124, 22);
            this.toolStripMenuItem1.Text = "打开工具";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.ToolStripMenuItem1_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(121, 6);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Image = global::ZYServiceTool.Properties.Resources.Startup;
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(124, 22);
            this.toolStripMenuItem5.Text = "关闭进程";
            this.toolStripMenuItem5.Click += new System.EventHandler(this.ToolStripMenuItem5_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(121, 6);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(121, 6);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Gear 1.ico");
            this.imageList1.Images.SetKeyName(1, "Gear 2.ico");
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1125, 643);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.toolStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "正元物联网服务管理工具";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.contextMenu.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

     
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripStartButton;
        private System.Windows.Forms.ToolStripButton toolStripPauseButton;
        private System.Windows.Forms.ToolStripButton toolStripStopButton;
        private System.Windows.Forms.ToolStripButton toolStripRestartButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripDeleteButton;
        private System.Windows.Forms.ToolStripButton toolStripInstallButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripRefreshButton;
        private System.Windows.Forms.ToolStripLabel toolStripFilterIcon;
        private System.Windows.Forms.ToolStripButton toolStripButton1;

        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem contextMenuStartItem;
        private System.Windows.Forms.ToolStripMenuItem contextMenuStopItem;
        private System.Windows.Forms.ToolStripMenuItem contextMenuRestartItem;
        private System.Windows.Forms.ToolStripSeparator contextMenuSpacer1;
        private System.Windows.Forms.ToolStripMenuItem contextMenuDeleteItem;
        private System.Windows.Forms.ToolStripMenuItem contextMenuStartupTypeItem;
        private System.Windows.Forms.ToolStripMenuItem contextStatupTypeAutomatic;
        private System.Windows.Forms.ToolStripMenuItem contextStartupTypeManual;
        private System.Windows.Forms.ToolStripMenuItem contextStartupTypeDisabled;
        private System.Windows.Forms.ToolStripSeparator contextMenuSpacer2;
        private System.Windows.Forms.ToolStripMenuItem contextMenuRefreshItem;
        private System.Windows.Forms.ToolStripSeparator contextMenuSpacer3;
        private System.Windows.Forms.DataGridViewImageColumn dataGridStatusIcon;
        private System.Windows.Forms.DataGridViewTextBoxColumn ServiceName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ServiceDesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn ServiceType;
        private System.Windows.Forms.DataGridViewTextBoxColumn ServiceState;
        private System.Windows.Forms.DataGridViewTextBoxColumn BZ;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
    }
}