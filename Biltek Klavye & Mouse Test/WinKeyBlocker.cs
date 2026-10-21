using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace Biltek.KlavyeTest
{
    public class WinKeyBlocker : IDisposable
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x100, WM_KEYUP = 0x101;
        private const int WM_SYSKEYDOWN = 0x104, WM_SYSKEYUP = 0x105;
        private const int VK_LWIN = 0x5B, VK_RWIN = 0x5C;

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);
        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private readonly LowLevelKeyboardProc proc;
        private IntPtr hook = IntPtr.Zero;

        public bool Active { get; set; }

        public event Action<Key, bool> WinKeyChanged;

        public WinKeyBlocker()
        {
            proc = HookCallback;
            using (Process p = Process.GetCurrentProcess())
                hook = SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(p.MainModule.ModuleName), 0);
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && Active)
            {
                int vk = Marshal.ReadInt32(lParam);
                if ((vk == VK_LWIN || vk == VK_RWIN) && IsOurAppForeground())
                {
                    int msg = (int)wParam;
                    bool down = msg == WM_KEYDOWN || msg == WM_SYSKEYDOWN;
                    bool up = msg == WM_KEYUP || msg == WM_SYSKEYUP;

                    if (down || up)
                    {
                        var h = WinKeyChanged;
                        if (h != null) h(vk == VK_LWIN ? Key.LWin : Key.RWin, down);
                    }
                    return (IntPtr)1;
                }
            }
            return CallNextHookEx(hook, nCode, wParam, lParam);
        }

        private static bool IsOurAppForeground()
        {
            uint pid;
            GetWindowThreadProcessId(GetForegroundWindow(), out pid);
            return pid == (uint)Process.GetCurrentProcess().Id;
        }

        public void Dispose()
        {
            if (hook != IntPtr.Zero)
            {
                UnhookWindowsHookEx(hook);
                hook = IntPtr.Zero;
            }
        }
    }
}
