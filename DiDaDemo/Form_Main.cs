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
using CefSharp;
using CefSharp.Enums;
using CefSharp.WinForms;
using System.Threading;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using Microsoft.Win32;
using DiDa_List_PC.Properties;

namespace DiDa_List_PC
{
    public partial class Form_Main : Form
    {
        /// <summary>
        /// 启动参数
        /// </summary>
        string[] Args = null;

        ChromiumWebBrowser Browser;

        /// <summary>
        /// 空任务集合
        /// </summary>
        List<TaskData> HistoryTasks = new List<TaskData>() {
            new TaskData() {
                Title = string.Empty,
                Content = string.Empty,
                dateTime = DateTime.Now
            }
        };

        /// <summary>
        /// 判断窗体是否激活
        /// </summary>
        bool IsWindowActivate = false;

        /// <summary>
        /// 初始网页地址
        /// </summary>
        string StartUrl = string.Empty;

        public Form_Main(string[] _args)
        {
            InitializeComponent();

            SetControlValue(Settings.Default);
            InitializeCefSharp(StartUrl);
            Args = _args;
        }

        #region 方法

        /// <summary>
        /// 初始化浏览器控件
        /// </summary>
        /// <param name="_url">初始地址</param>
        private void InitializeCefSharp(string _url)
        {
            // 初始化设置
            CefSettings Settings = new CefSettings
            {
                CachePath = Application.StartupPath + @"\Cache",
                Locale = "zh-CN"
            };
            Cef.Initialize(Settings);

            // 初始化浏览器
            Browser = new ChromiumWebBrowser(_url)
            {
                MenuHandler = new MenuHandler(),
                DragHandler = new DragHandler()
            };
            Controls.Add(Browser);

            //加载完成后事件
            Browser.FrameLoadEnd += Browser_FrameLoadEnd;
        }

        private List<TaskData> GetTaskDatas(string _html)
        {
            var tasks = new List<TaskData>();
            var taskListXPath = "//*[@id=\"reminders-modal\"]/ul/li";
            var taskXPath = "/div/div[2]/div/div[1]/div[1]";
            var contentXPath = "/div/div[2]/div/div[1]/div[2]";

            var doc = new HtmlDocument();
            doc.LoadHtml(_html);
            var nodes = doc.DocumentNode.SelectNodes(taskListXPath);

            if (nodes == null) return null;

            foreach (var node in nodes)
            {
                var xpath = node.XPath;
                var title = node.SelectSingleNode($"{xpath}{taskXPath}").InnerText;
                var content = node.SelectSingleNode($"{xpath}{contentXPath}").InnerText;

                tasks.Add(new TaskData()
                {
                    Title = title,
                    Content = content,
                    dateTime = DateTime.Now
                });
            }
            return tasks;
        }

        /// <summary>
        /// 对比新旧任务列是否重复，如未重复或时间大于通知周期则发送通知
        /// </summary>
        /// <param name="_newTasks">刚获取的新任务集合</param>
        /// <param name="_oldTasks">上次获取的任务集合</param>
        /// <param name="_tick">通知周期（单位为：秒）</param>
        /// <returns>返回一个任务集合，传入的旧任务集合为null或与新集合不同或与新集合相同但时间大于对比周期，则返回新集合并发送通知，否则返回旧集合</returns>
        private List<TaskData> EqualsTasks(List<TaskData> _newTasks, List<TaskData> _oldTasks, int _tick)
        {
            if (_newTasks == null) return _oldTasks;

            var trigger = false;

            foreach (var newD in _newTasks)
            {
                foreach (var oldD in _oldTasks)
                {
                    if (newD.Title != oldD.Title || newD.Content != oldD.Content || (newD.dateTime - oldD.dateTime).TotalSeconds > _tick)
                    {
                        notifyIcon1.ShowBalloonTip(3000, newD.Title, newD.Content == "" ? newD.Title : newD.Content, ToolTipIcon.Info);
                        trigger = true;
                    }
                }
            }

            return trigger ? _newTasks : _oldTasks;
        }

        /// <summary>
        /// 窗体未激活情况下，进行周期性同步操作
        /// </summary>
        /// <param name="_isWindowActivate">窗体是否激活</param>
        private void UpdateData(bool _isWindowActivate)
        {
            if (!_isWindowActivate)
            {
                Browser.ExecuteScriptAsync(Resources.Update_JS);
            }
        }

        /// <summary>
        /// 如果窗体离屏幕边缘很近，则停靠在该边缘
        /// </summary>
        /// <param name="_sideThickness">距边缘的距离</param>
        private void SideDock(int _sideThickness)
        {
            //如果窗体离屏幕边缘很近，则自动停靠在该边缘
            if (Top <= _sideThickness)
            {
                Top = 0;
                TopMost = false;
                Activate();
            }
            if (Left <= _sideThickness)
            {
                Left = 0;
                TopMost = false;
                Activate();
            }
            if (Left >= Screen.PrimaryScreen.WorkingArea.Width - Width - _sideThickness)
            {
                Left = Screen.PrimaryScreen.WorkingArea.Width - Width;
                TopMost = false;
                Activate();
            }
        }

        /// <summary>
        /// 侧边停靠，并隐藏至侧边缘
        /// </summary>
        /// <param name="_sideThickness">边缘的厚度，窗体停靠在边缘隐藏后留出来的可见部分的厚度</param>
        void SideHideOrShow(int _sideThickness)
        {
            if (WindowState == FormWindowState.Minimized || WindowState == FormWindowState.Maximized) return;

            //如果鼠标在窗体内
            if (Cursor.Position.X >= Left && Cursor.Position.X < Right && Cursor.Position.Y >= Top && Cursor.Position.Y < Bottom)
            {
                SideDock(_sideThickness);
            }
            //当鼠标离开窗体以后
            else
            {
                //隐藏到屏幕左边缘
                if (Left == 0)
                {
                    Left = _sideThickness - Width;
                    TopMost = true;
                }
                //隐藏到屏幕右边缘
                else if (Left == Screen.PrimaryScreen.WorkingArea.Width - Width)
                {
                    Left = Screen.PrimaryScreen.WorkingArea.Width - _sideThickness;
                    TopMost = true;
                }
                //隐藏到屏幕上边缘
                else if (Top == 0 && Left > 0 && Left < Screen.PrimaryScreen.WorkingArea.Width - Width)
                {
                    Top = _sideThickness - Height;
                    TopMost = true;
                }
            }
        }

        /// <summary>
        /// 设置或取消开机启动
        /// </summary>
        /// <param name="_isBoot">是否开机启动</param>
        private void SetBoot(bool _isBoot)
        {
            var subkey = Resources.Name;

            RegistryKey RKey = Registry.CurrentUser.CreateSubKey(Resources.Registry_Subkey);
            try
            {
                if (_isBoot && RKey.GetValue(subkey) == null)
                {
                    // 添加到 当前登陆用户的 注册表启动项
                    RKey.SetValue(subkey, Application.ExecutablePath + " -m");
                }
                else
                {
                    RKey.DeleteValue(subkey, false);
                }
            }
            finally
            {
                RKey.Close();
            }
        }

        /// <summary>
        /// 读取配置文件设置控件参数
        /// </summary>
        private void SetControlValue(Settings _settings)
        {
            ShowInTaskbar = _settings.HideEdgeValue ? false : true; // 开启mini模式，任务栏不显示窗体
            Size = ShowInTaskbar ? _settings.WindowSize : _settings.WindowSize_Mini;
            tsm_Mini.Checked = _settings.HideEdgeValue;
            tscb_DefaultList.SelectedIndex = _settings.Defaultlist;
            switch (tscb_DefaultList.SelectedIndex)
            {
                default:
                case 0:
                    StartUrl = Resources.List_Tasks;
                    break;
                case 1:
                    StartUrl = Resources.List_Today;
                    break;
                case 2:
                    StartUrl = Resources.List_Tomorrow;
                    break;
                case 3:
                    StartUrl = Resources.List_Week;
                    break;
            }
        }

        /// <summary>
        /// 传入启动参数设置最小化启动
        /// </summary>
        /// <param name="_args"></param>
        private void SetMinStar(string[] _args)
        {
            if (_args == null) return;

            foreach (var arg in _args)
            {
                switch (arg)
                {
                    case "-m":
                        ClosingToHide(null, new FormClosingEventArgs(CloseReason.UserClosing, false));
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        #region 事件区

        private void Form1_Load(object sender, EventArgs e)
        {
            //SetControlValue(Settings.Default);
            //InitializeCefSharp(StartUrl);
        }

        private void Form_Main_Shown(object sender, EventArgs e)
        {
            SetMinStar(Args);
        }

        private async void Timer1_Tick(object sender, EventArgs e)
        {
            // 任务通知
            var newTaskDatas = GetTaskDatas(await Browser.GetSourceAsync());
            HistoryTasks = EqualsTasks(newTaskDatas, HistoryTasks, 60);

            UpdateData(IsWindowActivate);
        }

        private void Browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            t_TimedTask.Start(); // 启动周期事件
        }

        private void LeftClickToShow(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            Show();
            WindowState = FormWindowState.Normal;
        }

        private void ClosingToHide(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing) return;

            e.Cancel = true;
            WindowState = FormWindowState.Minimized;
            Thread.Sleep(150);
            Hide();
        }

        private void MouseUpToExit(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            Application.Exit();
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            IsWindowActivate = true;
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            IsWindowActivate = false;
        }

        private void ToolStripMenuItem2_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.BootValue = tsm_Boot.Checked;
            Settings.Default.Save();
            SetBoot(tsm_Boot.Checked);
        }

        private void ToolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Default.Defaultlist = tscb_DefaultList.SelectedIndex;
            Settings.Default.Save();
        }

        private void Form_Main_ResizeEnd(object sender, EventArgs e)
        {
            if (tsm_Mini.Checked)
            {
                Settings.Default.WindowSize_Mini = Size;
            }
            else
            {
                Settings.Default.WindowSize = Size;
            }
            Settings.Default.Save();
        }

        private void Tsm_test_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.HideEdgeValue = tsm_Mini.Checked;
            Settings.Default.Save();
            Application.Restart();
        }

        private void T_AutoSideHideOrShow_Tick(object sender, EventArgs e)
        {
            SideHideOrShow(Settings.Default.SideThicknessValue);
        }

        #endregion

    }

    /// <summary>
    /// 屏蔽右键菜单
    /// </summary>
    public class MenuHandler : IContextMenuHandler
    {
        public void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
        {
            model.Clear();
        }
        public bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
        {
            return false;
        }
        public void OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {
        }
        public bool RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
        {
            return false;
        }
    }

    /// <summary>
    /// 屏蔽拖拽放置
    /// </summary> 
    public class DragHandler : IDragHandler
    {
        public bool OnDragEnter(IWebBrowser browserControl, IBrowser browser, IDragData dragData, DragOperationsMask mask)
        {
            return true;// false;//throw new NotImplementedException();
        }
        public void OnDraggableRegionsChanged(IWebBrowser browserControl, IBrowser browser, IList<DraggableRegion> regions)
        {
            //throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 任务数据
    /// </summary>
    struct TaskData
    {
        public string Title;
        public string Content;
        public DateTime dateTime;
    }
}