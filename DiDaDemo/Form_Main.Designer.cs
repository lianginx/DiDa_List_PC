namespace DiDa_List_PC
{
    partial class FormMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ToolStripSeparator _4;
            System.Windows.Forms.ToolStripSeparator _1;
            System.Windows.Forms.ToolStripSeparator _2;
            System.Windows.Forms.ToolStripSeparator _3;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsm_Open = new System.Windows.Forms.ToolStripMenuItem();
            this.tsm_Reload = new System.Windows.Forms.ToolStripMenuItem();
            this.tscb_DefaultList = new System.Windows.Forms.ToolStripComboBox();
            this.tsm_Mini = new System.Windows.Forms.ToolStripMenuItem();
            this.tsm_Boot = new System.Windows.Forms.ToolStripMenuItem();
            this.tsm_IsDisableShortcutKey = new System.Windows.Forms.ToolStripMenuItem();
            this.tsm_Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.t_TimedTask = new System.Windows.Forms.Timer(this.components);
            this.t_AutoSideHideOrShow = new System.Windows.Forms.Timer(this.components);
            _4 = new System.Windows.Forms.ToolStripSeparator();
            _1 = new System.Windows.Forms.ToolStripSeparator();
            _2 = new System.Windows.Forms.ToolStripSeparator();
            _3 = new System.Windows.Forms.ToolStripSeparator();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _4
            // 
            _4.Name = "_4";
            _4.Size = new System.Drawing.Size(207, 6);
            // 
            // _1
            // 
            _1.Name = "_1";
            _1.Size = new System.Drawing.Size(207, 6);
            // 
            // _2
            // 
            _2.Name = "_2";
            _2.Size = new System.Drawing.Size(207, 6);
            // 
            // _3
            // 
            _3.Name = "_3";
            _3.Size = new System.Drawing.Size(207, 6);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = global::DiDa_List_PC.Properties.Settings.Default.Name;
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.LeftClickToShow);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsm_Open,
            _1,
            this.tsm_Reload,
            _2,
            this.tscb_DefaultList,
            this.tsm_Mini,
            this.tsm_Boot,
            _3,
            this.tsm_IsDisableShortcutKey,
            _4,
            this.tsm_Exit});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.ShowCheckMargin = true;
            this.contextMenuStrip1.ShowImageMargin = false;
            this.contextMenuStrip1.Size = new System.Drawing.Size(211, 232);
            // 
            // tsm_Open
            // 
            this.tsm_Open.Name = "tsm_Open";
            this.tsm_Open.ShortcutKeyDisplayString = "Ctrl+Alt+D";
            this.tsm_Open.Size = new System.Drawing.Size(210, 24);
            this.tsm_Open.Text = "打开";
            this.tsm_Open.MouseUp += new System.Windows.Forms.MouseEventHandler(this.LeftClickToShow);
            // 
            // tsm_Reload
            // 
            this.tsm_Reload.Name = "tsm_Reload";
            this.tsm_Reload.Size = new System.Drawing.Size(210, 24);
            this.tsm_Reload.Text = "刷新页面";
            this.tsm_Reload.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Tsm_Reload_MouseUp);
            // 
            // tscb_DefaultList
            // 
            this.tscb_DefaultList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tscb_DefaultList.Items.AddRange(new object[] {
            "所有",
            "今天",
            "明天",
            "最近七天"});
            this.tscb_DefaultList.Name = "tscb_DefaultList";
            this.tscb_DefaultList.Size = new System.Drawing.Size(121, 28);
            this.tscb_DefaultList.SelectedIndexChanged += new System.EventHandler(this.ToolStripComboBox1_SelectedIndexChanged);
            // 
            // tsm_Mini
            // 
            this.tsm_Mini.Checked = global::DiDa_List_PC.Properties.Settings.Default.HideEdgeValue;
            this.tsm_Mini.CheckOnClick = true;
            this.tsm_Mini.Name = "tsm_Mini";
            this.tsm_Mini.ShowShortcutKeys = false;
            this.tsm_Mini.Size = new System.Drawing.Size(210, 24);
            this.tsm_Mini.Text = "Mini模式";
            this.tsm_Mini.CheckedChanged += new System.EventHandler(this.Tsm_test_CheckedChanged);
            // 
            // tsm_Boot
            // 
            this.tsm_Boot.Checked = global::DiDa_List_PC.Properties.Settings.Default.BootValue;
            this.tsm_Boot.CheckOnClick = true;
            this.tsm_Boot.Name = "tsm_Boot";
            this.tsm_Boot.ShowShortcutKeys = false;
            this.tsm_Boot.Size = new System.Drawing.Size(210, 24);
            this.tsm_Boot.Text = "开机启动";
            this.tsm_Boot.CheckedChanged += new System.EventHandler(this.ToolStripMenuItem2_CheckedChanged);
            // 
            // tsm_IsDisableShortcutKey
            // 
            this.tsm_IsDisableShortcutKey.Checked = global::DiDa_List_PC.Properties.Settings.Default.IsSetShortcutKey;
            this.tsm_IsDisableShortcutKey.CheckOnClick = true;
            this.tsm_IsDisableShortcutKey.Name = "tsm_IsDisableShortcutKey";
            this.tsm_IsDisableShortcutKey.ShortcutKeyDisplayString = "";
            this.tsm_IsDisableShortcutKey.ShowShortcutKeys = false;
            this.tsm_IsDisableShortcutKey.Size = new System.Drawing.Size(210, 24);
            this.tsm_IsDisableShortcutKey.Text = "禁用快捷键";
            this.tsm_IsDisableShortcutKey.CheckedChanged += new System.EventHandler(this.Tsm_IsDisableShortcutKey_CheckedChanged);
            // 
            // tsm_Exit
            // 
            this.tsm_Exit.Name = "tsm_Exit";
            this.tsm_Exit.ShowShortcutKeys = false;
            this.tsm_Exit.Size = new System.Drawing.Size(210, 24);
            this.tsm_Exit.Text = "退出";
            this.tsm_Exit.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MouseUpToExit);
            // 
            // t_TimedTask
            // 
            this.t_TimedTask.Interval = global::DiDa_List_PC.Properties.Settings.Default.TimedTaskValue;
            this.t_TimedTask.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // t_AutoSideHideOrShow
            // 
            this.t_AutoSideHideOrShow.Enabled = global::DiDa_List_PC.Properties.Settings.Default.HideEdgeValue;
            this.t_AutoSideHideOrShow.Interval = global::DiDa_List_PC.Properties.Settings.Default.Timer_AutoSideHide_Interval;
            this.t_AutoSideHideOrShow.Tick += new System.EventHandler(this.AutoSideHideOrShow_Tick);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1282, 753);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::DiDa_List_PC.Properties.Settings.Default, "Name", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MaximumSize = global::DiDa_List_PC.Properties.Settings.Default.WindowMaxSize;
            this.MinimumSize = global::DiDa_List_PC.Properties.Settings.Default.WindowMinSize;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = global::DiDa_List_PC.Properties.Settings.Default.Name;
            this.Activated += new System.EventHandler(this.Form1_Activated);
            this.Deactivate += new System.EventHandler(this.Form1_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ClosingToHide);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form_Main_Shown);
            this.ResizeEnd += new System.EventHandler(this.Form_Main_ResizeEnd);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsm_Open;
        private System.Windows.Forms.ToolStripMenuItem tsm_Boot;
        private System.Windows.Forms.ToolStripMenuItem tsm_Exit;
        private System.Windows.Forms.Timer t_TimedTask;
        private System.Windows.Forms.ToolStripComboBox tscb_DefaultList;
        private System.Windows.Forms.ToolStripMenuItem tsm_Mini;
        private System.Windows.Forms.Timer t_AutoSideHideOrShow;
        private System.Windows.Forms.ToolStripMenuItem tsm_IsDisableShortcutKey;
        private System.Windows.Forms.ToolStripMenuItem tsm_Reload;
    }
}

