using System;
using Microsoft.Win32;

namespace AudioSwitcher
{
    public class AutoStartManager
    {
        private const string AppName = "AudioSwitcher";
        private readonly string _appPath;

        public AutoStartManager()
        {
            _appPath = $"\"{Environment.ProcessPath}\"";
        }

        public bool IsAutoStartEnabled()
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", false);
            var value = key?.GetValue(AppName);
            return value != null && value.ToString().Equals(_appPath, StringComparison.OrdinalIgnoreCase);
        }

        public void EnableAutoStart()
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            key?.SetValue(AppName, _appPath);
        }

        public void DisableAutoStart()
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            key?.DeleteValue(AppName, false);
        }
    }
}
