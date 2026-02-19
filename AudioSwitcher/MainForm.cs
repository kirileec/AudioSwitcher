using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using AudioSwitcher.Models;
using CoreAudio;

namespace AudioSwitcher
{
    public class MainForm : Form
    {
        private NotifyIcon _notifyIcon;
        private ContextMenuStrip _contextMenu;
        private ToolStripMenuItem _menuSettings;
        private ToolStripMenuItem _menuAutoStart;
        private ToolStripMenuItem _menuAbout;
        private ToolStripMenuItem _menuExit;

        private readonly AudioDeviceManager _deviceManager;
        private readonly ConfigManager _configManager;
        private readonly AutoStartManager _autoStartManager;
        private readonly SingleInstanceManager _instanceManager;
        private HotKeyManager _hotKeyManager;

        public MainForm(
            AudioDeviceManager deviceManager,
            ConfigManager configManager,
            AutoStartManager autoStartManager,
            SingleInstanceManager instanceManager)
        {
            _deviceManager = deviceManager;
            _configManager = configManager;
            _autoStartManager = autoStartManager;
            _instanceManager = instanceManager;
           
            
            InitializeComponents();
            _hotKeyManager = new HotKeyManager(this.Handle);
            SetupEventHandlers();
            _hotKeyManager.Register();
            _instanceManager.SwitchRequested += OnSwitchRequested;
            _instanceManager.StartServer();



            UpdateAutoStartMenu();
        }

        private void InitializeComponents()
        {
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;
            this.FormClosing += OnFormClosing;

            _contextMenu = new ContextMenuStrip();

            _menuSettings = new ToolStripMenuItem("设置输出设备");
            _menuAutoStart = new ToolStripMenuItem("开机自动启动");
            _menuAbout = new ToolStripMenuItem("关于");
            _menuExit = new ToolStripMenuItem("退出");

            _contextMenu.Items.AddRange(new ToolStripItem[]
            {
                _menuSettings,
                _menuAutoStart,
                new ToolStripSeparator(),
                _menuAbout,
                new ToolStripSeparator(),
                _menuExit
            });

            _notifyIcon = new NotifyIcon
            {
                Icon = LoadIcon(),
                Text = "声音输出设备切换工具",
                Visible = true,
                ContextMenuStrip = _contextMenu
            };
        }

        private void SetupEventHandlers()
        {
            _menuSettings.Click += OnMenuSettingsClick;
            _menuAutoStart.Click += OnMenuAutoStartClick;
            _menuAbout.Click += OnMenuAboutClick;
            _menuExit.Click += OnMenuExitClick;
            _notifyIcon.DoubleClick += OnNotifyIconDoubleClick;
            _hotKeyManager.HotKeyPressed += OnHotKeyPressed;
        }

        private void OnMenuSettingsClick(object sender, EventArgs e)
        {
            using var form = new SettingsForm(_deviceManager, _configManager);
            form.ShowDialog();
        }

        private void OnMenuAutoStartClick(object sender, EventArgs e)
        {
            if (_autoStartManager.IsAutoStartEnabled())
            {
                _autoStartManager.DisableAutoStart();
                _configManager.SetAutoStart(false);
            }
            else
            {
                _autoStartManager.EnableAutoStart();
                _configManager.SetAutoStart(true);
            }
            
            UpdateAutoStartMenu();
        }

        private void OnMenuAboutClick(object sender, EventArgs e)
        {
            MessageBox.Show(
                "声音输出设备快速切换工具\n\n版本: 1.0.0\n\n快捷键: Win + Alt + Q\n\n用于快速切换预设的声音输出设备",
                "关于",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void OnMenuExitClick(object sender, EventArgs e)
        {
            _notifyIcon.Visible = false;
            Application.Exit();
        }

        private void OnNotifyIconDoubleClick(object sender, EventArgs e)
        {
            SwitchDevice();
        }

        private void OnHotKeyPressed(object sender, EventArgs e)
        {
            SwitchDevice();
        }

        private void OnSwitchRequested(object sender, EventArgs e)
        {
            BeginInvoke(new Action(SwitchDevice));
        }

        private void SwitchDevice()
        {
            var config = _configManager.Config;
            MMDevice targetDevice = null;

            if (config.Device1 == null || config.Device2 == null)
            {
                ShowNotification("提示", "请先设置输出设备");
                return;
            }
           
            MMDevice defaultDevice = _deviceManager.GetDefaultOutputDevice();
            string deviceId = "";
            if (defaultDevice.ID == config.Device1.Id)
            {
                deviceId = config.Device2.Id;
            }
            else if (defaultDevice.ID == config.Device2.Id) { 
               deviceId = config.Device1.Id;
            }


            targetDevice = _deviceManager.GetDeviceById(deviceId);

            if (targetDevice != null)
            {
                if (_deviceManager.SetDefaultOutputDevice(targetDevice))
                {
                    ShowNotification("设备已切换", $"{targetDevice.DeviceFriendlyName}");
                }
                else
                {
                    ShowNotification("切换失败", "无法切换到指定设备");
                }
            }
        }

        private void ShowNotification(string title, string text)
        {
            var toast = new ToastForm(title, text);
            toast.Show();
        }

        private void UpdateAutoStartMenu()
        {
            if (_autoStartManager.IsAutoStartEnabled())
            {
                _menuAutoStart.Text = "√ 开机自动启动";
            }
            else
            {
                _menuAutoStart.Text = "开机自动启动";
            }
        }

        private Icon LoadIcon()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("AudioSwitcher.Assets.icon.ico");
            return stream != null ? new Icon(stream) : SystemIcons.Application;
        }

        protected override void WndProc(ref Message m)
        {
            if (_hotKeyManager!=null && _hotKeyManager.HandleMessage(m))
                return;
            
            base.WndProc(ref m);
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            _notifyIcon.Visible = false;
            _hotKeyManager.Dispose();
        }
    }
}
