using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VirtualGyro.Hook
{
    internal class MouseHook
    {
        public const int WH_MOUSE_LL = 14;
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONUP = 0x0202;
        public const int WM_RBUTTONDOWN = 0x0204;
        public const int WM_RBUTTONUP = 0x0205;
        public const int WM_MOUSEMOVE = 0x0200;

        public static event Func<int, IntPtr, IntPtr, IntPtr>? Callback;

        private static IntPtr hookId = IntPtr.Zero;

        private delegate IntPtr MouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        public static void Start()
        {
            if (hookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(hookId);
            }

            Process process = Process.GetCurrentProcess();
            ProcessModule module = process.MainModule;
            IntPtr hInstance = GetModuleHandle(module.ModuleName);
            MouseProc proc = HookCallback;
            hookId = SetWindowsHookEx(WH_MOUSE_LL, proc, hInstance, 0);
        }

        public static void Stop()
        {
            if (hookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(hookId);
                hookId = IntPtr.Zero;
            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            //if (nCode >= 0)  // nCode 表示钩子的事件类型，只有在代码大于等于0时才会触发
            //{
            //    if ((int)wParam == WM_LBUTTONDOWN)
            //        Console.WriteLine("Left Mouse Button Pressed");
            //    if ((int)wParam == WM_LBUTTONUP)
            //        Console.WriteLine("Left Mouse Button Released");
            //    if ((int)wParam == WM_RBUTTONDOWN)
            //        Console.WriteLine("Right Mouse Button Pressed");
            //    if ((int)wParam == WM_RBUTTONUP)
            //        Console.WriteLine("Right Mouse Button Released");
            //    if ((int)wParam == WM_MOUSEMOVE)
            //    {
            //        // 获取鼠标位置
            //        int x = Marshal.ReadInt32(lParam);
            //        int y = Marshal.ReadInt32(new IntPtr(lParam.ToInt64() + 4));
            //        Console.WriteLine($"Mouse Moved to: {x}, {y}");
            //    }
            //}

            Callback?.Invoke(nCode, wParam, lParam);
            if ((int)wParam == WM_MOUSEMOVE)
            {
                return (IntPtr)1;
            }

            return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, MouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
