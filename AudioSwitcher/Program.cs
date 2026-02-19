using System;
using System.Windows.Forms;

namespace AudioSwitcher
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using var instanceManager = new SingleInstanceManager();
            
            if (args.Length > 0 && args[0] == "-s")
            {
                if (instanceManager.TryCreate())
                {
                    MessageBox.Show(
                        "程序未运行，请先启动程序",
                        "提示",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }
                
                if (SingleInstanceManager.NotifyRunningInstance())
                {
                    return;
                }
                
                MessageBox.Show(
                    "无法通知运行中的程序",
                    "错误",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            if (!instanceManager.TryCreate())
            {
                MessageBox.Show(
                    "程序已经在运行",
                    "提示",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            using var deviceManager = new AudioDeviceManager();
            var configManager = new ConfigManager();
            var autoStartManager = new AutoStartManager();

            var mainForm = new MainForm(
                deviceManager,
                configManager,
                autoStartManager,
                instanceManager);

            Application.Run(mainForm);
        }
    }
}
