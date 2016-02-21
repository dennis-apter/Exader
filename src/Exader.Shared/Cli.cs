#if !SILVERLIGHT
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using JetBrains.Annotations;

namespace Exader
{
    /// <summary>
    /// Command-line interface utility.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class Cli
    {
        private const int SW_MINIMIZE = 6;
        private const int SW_MAXIMIZE = 3;
        private const int SW_RESTORE = 9;
        private const int SW_SHOWNORMAL = 1;
        private const int SW_HIDE = 0;

        private static bool? HasWindow;

        public static void CheckConsoleWindow()
        {
            FindConsoleWindow();
        }

        public static void MaximizeWindow()
        {
            TryShowWindow(FindConsoleWindow(), SW_MAXIMIZE);
        }

        public static void MinimizeWindow()
        {
            TryShowWindow(FindConsoleWindow(), SW_MINIMIZE);
        }

        public static void MinimizeMaximizedWindow()
        {
            var wnd = FindConsoleWindow();
            TryShowWindow(wnd, SW_MAXIMIZE);
            TryShowWindow(wnd, SW_MINIMIZE);
        }

        public static void RestoreWindow()
        {
            TryShowWindow(FindConsoleWindow(), SW_RESTORE);
        }

        public static bool WindowVisible
        {
            get
            {
                if (HasWindow == null)
                {
                    CheckConsoleWindow();
                }

                return HasWindow == true;
            }
            set
            {
                TryShowWindow(FindConsoleWindow(), value ? SW_SHOWNORMAL : SW_HIDE);
            }
        }

        private static void TryShowWindow(IntPtr hWnd, int nCmdShow)
        {
            if (hWnd != IntPtr.Zero) ShowWindow(hWnd, nCmdShow);
        }

        private static IntPtr FindConsoleWindow()
        {
            if (Environment.UserInteractive)
            {
                IntPtr hWnd = Process.GetCurrentProcess().MainWindowHandle;
                if (hWnd != IntPtr.Zero)
                {
                    HasWindow = true;
                    return hWnd;
                }

                // Далее для тех редких случаев,
                // когда окно консоли отображается одновременно с обычными окнами
                // и прочей экзотики …

                string originalTitle;
                try
                {
                    originalTitle = Console.Title;
                }
                catch (IOException)
                {
                    return IntPtr.Zero;
                }

                try
                {
                    string uniqueTitle = Guid.NewGuid().ToString();
                    Console.Title = uniqueTitle;

                    Thread.Sleep(50);

                    IntPtr handle = FindWindowByCaption(IntPtr.Zero, uniqueTitle);
                    if (handle != IntPtr.Zero)
                    {
                        HasWindow = true;
                        return handle;
                    }
                    else
                    {
                        HasWindow = false;
                    }
                }
                finally
                {
                    Console.Title = originalTitle;
                }
            }

            return IntPtr.Zero;
        }

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        private static extern IntPtr FindWindowByCaption(IntPtr zeroOnly, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    }
}
#endif
