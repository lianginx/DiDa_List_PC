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
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DiDa_List_PC
{
    public partial class FormMain : Form
    {
        #region 属性/字段

        private const int SHORTCUTKEY_ID = 100;

        public string[] StarArgs { get; }
        public bool IsWindowActivate { get; private set; }
        public ChromiumWebBrowser Browser { get; private set; }
        public string StartUrl { get; private set; }
        public List<TaskData> HistoryTasks { get; private set; }

        #endregion

        #region 方法

        public FormMain(string[] args)
        {
            InitializeComponent();
            SetControlValue(Settings.Default);
            InitializeCefSharp(StartUrl);
            StarArgs = args;
        }

        /// <summary>
        /// 初始化浏览器控件
        /// </summary>
        /// <param name="url">初始地址</param>
        private void InitializeCefSharp(string url)
        {
            // 初始化设置
            var settings = new CefSettings
            {
                CachePath = Application.StartupPath + @"\Cache",
                Locale = "zh-CN"
            };
            Cef.Initialize(settings);

            // 初始化浏览器
            Browser = new ChromiumWebBrowser(url)
            {
                MenuHandler = new MenuHandler(),
                DragHandler = new DragHandler()
            };
            Controls.Add(Browser);

            //加载完成后事件
            Browser.FrameLoadEnd += Browser_FrameLoadEnd;
        }

        private static List<TaskData> GetTaskDatas(string html)
        {
            var tasks = new List<TaskData>();
            const string taskListXPath = "//*[@id=\"reminders-modal\"]/ul/li";
            const string taskXPath = "/div/div[2]/div/div[1]/div[1]";
            const string contentXPath = "/div/div[2]/div/div[1]/div[2]";

            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var nodes = doc.DocumentNode.SelectNodes(taskListXPath);

            if (nodes == null) return null;

            foreach (var node in nodes)
            {
                var xpath = node.XPath;
                tasks.Add(new TaskData
                {
                    Title = node.SelectSingleNode($"{xpath}{taskXPath}").InnerText,
                    Content = node.SelectSingleNode($"{xpath}{contentXPath}").InnerText,
                    DateTime = DateTime.Now
                });
            }

            return tasks;
        }

        /// <summary>
        /// 对比新旧任务列是否重复，如未重复或时间大于通知周期则发送通知
        /// </summary>
        /// <param name="newTasks">刚获取的新任务集合</param>
        /// <param name="oldTasks">上次获取的任务集合</param>
        /// <param name="tick">通知周期（单位为：秒）</param>
        /// <returns>返回一个任务集合，传入的旧任务集合为null或与新集合不同或与新集合相同但时间大于对比周期，则返回新集合并发送通知，否则返回旧集合</returns>
        private List<TaskData> EqualsTasks(List<TaskData> newTasks, List<TaskData> oldTasks, int tick)
        {
            if (newTasks == null) return oldTasks;

            if (oldTasks == null)
            {
                foreach (var item in newTasks)
                {
                    notifyIcon1.ShowBalloonTip(0,
                                               item.Title,
                                               item.Content == "" ? item.Title : item.Content,
                                               ToolTipIcon.Info);
                }

                return newTasks;
            }

            var trigger = false;
            foreach (var newD in newTasks)
            {
                foreach (var oldD in oldTasks)
                {
                    if (newD.Title != oldD.Title && newD.Content != oldD.Content) continue;

                    if (!((newD.DateTime - oldD.DateTime).TotalSeconds > tick))
                    {
                        continue;
                    }
                    else
                    {
                        notifyIcon1.ShowBalloonTip(0,
                                                   newD.Title,
                                                   newD.Content == "" ? newD.Title : newD.Content,
                                                   ToolTipIcon.Info);
                        trigger = true;
                        break;
                    }
                }
            }

            return trigger ? newTasks : oldTasks;
        }

        /// <summary>
        /// 窗体未激活情况下，进行周期性同步操作
        /// </summary>
        /// <param name="isWindowActivate">窗体是否激活</param>
        private void UpdateData(bool isWindowActivate)
        {
            if (!isWindowActivate)
            {
                Browser.ExecuteScriptAsync(Resources.Update_JS);
            }
        }

        /// <summary>
        /// 如果窗体离屏幕边缘很近，则停靠在该边缘
        /// </summary>
        /// <param name="sideThickness">距边缘的距离</param>
        private void SideDock(int sideThickness)
        {
            //如果窗体离屏幕边缘很近，则自动停靠在该边缘
            if (Top <= sideThickness)
            {
                Top = 0;
                TopMost = false;
                Activate();
            }
            if (Left <= sideThickness)
            {
                Left = 0;
                TopMost = false;
                Activate();
            }
            if (Left >= Screen.PrimaryScreen.WorkingArea.Width - Width - sideThickness)
            {
                Left = Screen.PrimaryScreen.WorkingArea.Width - Width;
                TopMost = false;
                Activate();
            }
        }

        /// <summary>
        /// 侧边停靠，并隐藏至侧边缘
        /// </summary>
        /// <param name="sideThickness">边缘的厚度，窗体停靠在边缘隐藏后留出来的可见部分的厚度</param>
        private void SideHideOrShow(int sideThickness)
        {
            if (WindowState == FormWindowState.Minimized || WindowState == FormWindowState.Maximized) return;

            //如果鼠标在窗体内
            if (Cursor.Position.X >= Left && Cursor.Position.X < Right && Cursor.Position.Y >= Top && Cursor.Position.Y < Bottom)
            {
                SideDock(sideThickness);
            }
            //当鼠标离开窗体以后
            else
            {
                //隐藏到屏幕左边缘
                if (Left == 0)
                {
                    Left = sideThickness - Width;
                    TopMost = true;
                }
                //隐藏到屏幕右边缘
                else if (Left == Screen.PrimaryScreen.WorkingArea.Width - Width)
                {
                    Left = Screen.PrimaryScreen.WorkingArea.Width - sideThickness;
                    TopMost = true;
                }
                //隐藏到屏幕上边缘
                else if (Top == 0 && Left > 0 && Left < Screen.PrimaryScreen.WorkingArea.Width - Width)
                {
                    Top = sideThickness - Height;
                    TopMost = true;
                }
            }
        }

        /// <summary>
        /// 设置或取消开机启动
        /// </summary>
        /// <param name="isBoot">是否开机启动</param>
        private static void SetBoot(bool isBoot)
        {
            var subkey = Resources.Name;

            using (var rKey = Registry.CurrentUser.CreateSubKey(Resources.Registry_Subkey))
            {
                try
                {
                    if (isBoot && rKey?.GetValue(subkey) == null)
                    {
                        rKey?.SetValue(subkey, Application.ExecutablePath + " -m");
                    }
                    else
                    {
                        rKey?.DeleteValue(subkey, false);
                    }
                }
                finally
                {
                    rKey?.Close();
                }
            }
        }

        /// <summary>
        /// 读取配置文件设置控件参数
        /// </summary>
        private void SetControlValue(Settings settings)
        {
            // 开启mini模式，任务栏不显示窗体
            ShowInTaskbar = !settings.HideEdgeValue;

            // 窗体大小
            Size = ShowInTaskbar ? settings.WindowSize : settings.WindowSize_Mini;

            // MINI模式
            tsm_Mini.Checked = settings.HideEdgeValue;

            // 默认清单
            tscb_DefaultList.SelectedIndex = settings.Defaultlist;
            switch (tscb_DefaultList.SelectedIndex)
            {
                default:
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

            // 全局快捷键
            tsm_IsDisableShortcutKey.Checked = settings.IsSetShortcutKey;
        }

        /// <summary>
        /// 传入启动参数设置最小化启动
        /// </summary>
        /// <param name="args"></param>
        private void SetMinStar(string[] args)
        {
            if (args == null) return;

            foreach (var arg in args)
            {
                switch (arg)
                {
                    case "-m":
                        SetShowOrHideWindow(false);
                        break;
                }
            }
        }

        /// <summary>
        /// 根据参数注册热键或注销热键
        /// </summary>
        /// <param name="isEnable">是否注册热键</param>
        private void SetShortcutKey(bool isEnable)
        {
            if (isEnable)
            {
                HotKey.RegisterHotKey(Handle, SHORTCUTKEY_ID, HotKey.KeyModifiers.Ctrl | HotKey.KeyModifiers.Alt, Keys.D);
            }
            else
            {
                HotKey.UnregisteredHotKey(Handle, SHORTCUTKEY_ID);
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// 获取Windows消息，响应全局快捷键
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            const int hotKey = 0x0312;
            if (m.Msg == hotKey || m.HWnd.ToInt32() == SHORTCUTKEY_ID)
            {
                SetShowOrHideWindow(!IsWindowActivate);
            }
            base.WndProc(ref m);
        }

        /// <summary>
        /// 设置显示或隐藏窗体
        /// </summary>
        /// <param name="isShow">是否显示窗体</param>
        private void SetShowOrHideWindow(bool isShow)
        {
            if (isShow)
            {
                // 显示窗体
                Show();
                WindowState = FormWindowState.Normal;
                Activate();
            }
            else
            {
                // 隐藏窗体
                WindowState = FormWindowState.Minimized;
                Thread.Sleep(150);
                Hide();
            }
        }

        /// <summary>
        /// 禁止重复启动（只能放在Load事件中）
        /// </summary>
        private static void DisableDuplicateStartup()
        {
            var processName = Process.GetCurrentProcess().ProcessName;
            var processes = Process.GetProcessesByName(processName);
            if (processes.Length > 1)
            {
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// 获取版本更新
        /// </summary>
        private async Task GetVersionUpdate()
        {
            var githubUrl = Settings.Default.GithubUrl;
            var xpath = Settings.Default.GitHubVersionXPath;

            var text = await new WebClient().DownloadStringTaskAsync(githubUrl);
            var doc = new HtmlDocument();
            doc.LoadHtml(text);
            var githubTag = doc.DocumentNode.SelectSingleNode(xpath).InnerText.Trim();
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            if (version.Equals($@"{githubTag.TrimStart('v')}.0")) return;
            notifyIcon1.ShowBalloonTip(1, "更新提醒", $"GitHub 上有新的版本: {githubTag}", ToolTipIcon.Info);
        }

        #endregion

        #region 事件

        private async void Form1_Load(object sender, EventArgs e)
        {
            DisableDuplicateStartup();
            await GetVersionUpdate();
            SetShortcutKey(!tsm_IsDisableShortcutKey.Checked);
        }

        private void Form_Main_Shown(object sender, EventArgs e)
        {
            SetMinStar(StarArgs);
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            IsWindowActivate = true;
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            IsWindowActivate = false;
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

        private void Browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            t_TimedTask.Start(); // 启动周期事件
        }

        private void LeftClickToShow(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            SetShowOrHideWindow(true);
        }

        private void ClosingToHide(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing) return;
            e.Cancel = true;
            SetShowOrHideWindow(false);
        }

        private void MouseUpToExit(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            Application.Exit();
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

        private async void Timer1_Tick(object sender, EventArgs e)
        {
            // 任务通知
            var newTaskDatas = GetTaskDatas(await Browser.GetSourceAsync());
            HistoryTasks = EqualsTasks(newTaskDatas, HistoryTasks, 60);

            UpdateData(IsWindowActivate);
        }

        private void AutoSideHideOrShow_Tick(object sender, EventArgs e)
        {
            SideHideOrShow(Settings.Default.SideThicknessValue);
        }

        private void Tsm_test_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.HideEdgeValue = tsm_Mini.Checked;
            Settings.Default.Save();
            Application.Restart();
        }

        private void Tsm_IsDisableShortcutKey_CheckedChanged(object sender, EventArgs e)
        {
            // 是否禁用全局快捷键
            SetShortcutKey(!tsm_IsDisableShortcutKey.Checked);
        }

        private void Tsm_Reload_MouseUp(object sender, MouseEventArgs e)
        {
            Browser.GetBrowser().Reload();
        }

        #endregion

    }

    /// <summary>
    /// 任务数据
    /// </summary>
    public struct TaskData
    {
        public string Title;
        public string Content;
        public DateTime DateTime;
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
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
    /// 全局快捷键注册与注销
    /// </summary>
    public class HotKey
    {
        /// <summary>
        /// 绑定热键
        /// </summary>
        /// <param name="hWnd">要定义热键的窗口的句柄</param>
        /// <param name="id">定义热键ID（不能与其它ID重复） </param>
        /// <param name="fsModifiers">标识热键是否在按Alt、Ctrl、Shift、Windows等键时才会生效</param>
        /// <param name="vk">定义热键的内容</param>
        /// <returns>执行成功返回值不为0，失败返回值为0，要得到扩展错误信息，调用GetLastError</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(
            IntPtr hWnd,
            int id,
            KeyModifiers fsModifiers,
            Keys vk
            );

        /// <summary>
        /// 取消热键绑定
        /// </summary>
        /// <param name="hWnd">要取消热键的窗口的句柄</param>
        /// <param name="id">要取消热键的ID</param>
        /// <returns>执行成功返回值不为0，失败返回值为0，要得到扩展错误信息，调用GetLastError</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisteredHotKey(
            IntPtr hWnd,
            int id
            );

        /// <summary>
        /// 定义了辅助键的名称（将数字转变为字符以便于记忆，也可去除此枚举而直接使用数值）
        /// </summary>
        [Flags]
        public enum KeyModifiers
        {
            None = 0,
            Alt = 1,
            Ctrl = 2,
            Shift = 4,
            WindowsKey = 8
        }
    }
}