using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace S.AddonsOverhaul.Core.Configs
{
    internal class Functions
    {
        public static void InitializeConsole(string title, bool alwaysCreateNewConsole = true)
        {
            var consoleAttached = true;
            if (alwaysCreateNewConsole
                || (DllImports.AttachConsole(DllImports.ATTACH_PARRENT) == 0
                    && Marshal.GetLastWin32Error() != DllImports.ERROR_ACCESS_DENIED))
                consoleAttached = DllImports.AllocConsole() != 0;

            if (consoleAttached)
            {
                InitializeOutStream();
                InitializeInStream();
                DllImports.SetConsoleTitle(title);
            }
        }

        public static int MessageBox(string msg, string title)
        {
            return DllImports.MessageBox(IntPtr.Zero, msg, title, 0);
        }

        public static void OpenLogFile()
        {
            ProcessStartInfo info = new()
            {
                Arguments = Constants.LogPath,
                FileName = "notepad.exe",
                UseShellExecute = false
            };
            Process.Start(info);
        }

        private static void InitializeOutStream()
        {
            var fs = CreateFileStream("CONOUT$", DllImports.GENERIC_WRITE, DllImports.FILE_SHARE_WRITE,
                FileAccess.Write);
            if (fs != null)
            {
                var writer = new StreamWriter(fs) { AutoFlush = true };
                Console.SetOut(writer);
                Console.SetError(writer);
            }
        }

        private static void InitializeInStream()
        {
            var fs = CreateFileStream("CONIN$", DllImports.GENERIC_READ, DllImports.FILE_SHARE_READ, FileAccess.Read);
            if (fs != null) Console.SetIn(new StreamReader(fs));
        }

        private static FileStream CreateFileStream(string name, uint win32DesiredAccess, uint win32ShareMode,
            FileAccess dotNetFileAccess)
        {
            var file = new SafeFileHandle(
                DllImports.CreateFileW(name, win32DesiredAccess, win32ShareMode, IntPtr.Zero, DllImports.OPEN_EXISTING,
                    DllImports.FILE_ATTRIBUTE_NORMAL, IntPtr.Zero), true);
            if (!file.IsInvalid)
            {
                var fs = new FileStream(file, dotNetFileAccess);
                return fs;
            }

            return null;
        }
    }
}