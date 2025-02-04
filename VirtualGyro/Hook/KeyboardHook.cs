using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VirtualGyro.Hook
{
    internal class KeyboardHook
    {
        public const int WH_KEYBOARD_LL = 13;
        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x0101;
        public const int WM_SYSKEYDOWN = 0x0104;
        public const int WM_SYSKEYUP = 0x0105;
        public const int WM_CHAR = 0x0102;

        public static event Func<int, IntPtr, IntPtr, IntPtr>? Callback;

        private static IntPtr hookId;

        private delegate IntPtr KeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        public static void Start()
        {
            if (hookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(hookId);
            }

            Process process = Process.GetCurrentProcess();
            ProcessModule module = process.MainModule;
            IntPtr hInstance = GetModuleHandle(module.ModuleName);
            KeyboardProc proc = HookCallback;
            hookId = SetWindowsHookEx(WH_KEYBOARD_LL, proc, hInstance, 0);
        }

        public static void Stop()
        {
            if (hookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(hookId);

            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            //if (nCode >= 0)  // nCode 表示钩子的事件类型，只有在代码大于等于0时才会触发
            //{
            //    int keyCode = Marshal.ReadInt32(lParam);  // 获取按键的虚拟键码

            //    // 判断按下的键
            //    if ((int)wParam == WM_KEYDOWN || (int)wParam == WM_SYSKEYDOWN)
            //    {
            //        Console.WriteLine($"Key Down: {(Keys)keyCode}");
            //    }
            //    else if ((int)wParam == WM_KEYUP || (int)wParam == WM_SYSKEYUP)
            //    {
            //        Console.WriteLine($"Key Up: {(Keys)keyCode}");
            //    }
            //    else if ((int)wParam == WM_CHAR)
            //    {
            //        char character = (char)keyCode;
            //        Console.WriteLine($"Char: {character}");
            //    }
            //}

            Callback?.Invoke(nCode, wParam, lParam);

            return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, KeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
