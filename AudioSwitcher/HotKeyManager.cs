using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AudioSwitcher
{
    public class HotKeyManager : IDisposable
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int MOD_ALT = 0x1;
        private const int MOD_WIN = 0x8;
        private const int WM_HOTKEY = 0x312;
        private const int HOTKEY_ID = 9000;

        private readonly IntPtr _windowHandle;
        private bool _isRegistered;

        public event EventHandler HotKeyPressed;

        public HotKeyManager(IntPtr windowHandle)
        {
            _windowHandle = windowHandle;
        }

        public bool Register()
        {
            if (_isRegistered)
                return true;

            int modifiers = MOD_ALT | MOD_WIN;
            int key = (int)Keys.Q;

            _isRegistered = RegisterHotKey(_windowHandle, HOTKEY_ID, modifiers, key);
            return _isRegistered;
        }

        public void Unregister()
        {
            if (_isRegistered)
            {
                UnregisterHotKey(_windowHandle, HOTKEY_ID);
                _isRegistered = false;
            }
        }

        public bool HandleMessage(Message m)
        {
            if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_ID)
            {
                HotKeyPressed?.Invoke(this, EventArgs.Empty);
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            Unregister();
        }
    }
}
