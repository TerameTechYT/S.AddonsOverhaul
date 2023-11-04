using System;
using System.Runtime.InteropServices;

namespace S.AddonsOverhaul.Core.Configs
{
    internal class DllImports
    {
        public const uint GENERIC_WRITE = 0x40000000;
        public const uint GENERIC_READ = 0x80000000;
        public const uint FILE_SHARE_READ = 0x00000001;
        public const uint FILE_SHARE_WRITE = 0x00000002;
        public const uint OPEN_EXISTING = 0x00000003;
        public const uint FILE_ATTRIBUTE_NORMAL = 0x80;
        public const uint ERROR_ACCESS_DENIED = 5;
        public const uint ATTACH_PARRENT = 0xFFFFFFFF;

        [DllImport("kernel32.dll",
            EntryPoint = "AllocConsole",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int AllocConsole();

        [DllImport("kernel32.dll",
            EntryPoint = "AttachConsole",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern uint AttachConsole(uint dwProcessId);

        [DllImport("kernel32.dll",
            EntryPoint = "CreateFileW",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr CreateFileW(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile
        );

        [DllImport("kernel32.dll")]
        public static extern bool SetConsoleTitle(
            string lpConsoleTitle
        );

        [DllImport("User32.dll",
            CharSet = CharSet.Unicode)]
        public static extern int MessageBox(
            IntPtr h,
            string m,
            string c,
            int type
        );
    }
}