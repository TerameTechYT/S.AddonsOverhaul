using System;
using System.IO;
using System.Threading;
using Assets.Scripts;
using S.AddonsOverhaul.Core.Configs;
using S.AddonsOverhaul.Core.Interfaces.Log;

namespace S.AddonsOverhaul.Core
{
    internal static class AddonsLogger
    {
        public static void InitializeLog()
        {
            new DisposableFileStream(Constants.LogPath, FileMode.Create).Dispose();
        }

        public static void OpenConsole()
        {
            Functions.InitializeConsole("S.AddonsOverhaul Console");
        }

        public static void Log(string message, LogLevel level = LogLevel.Info, bool force = false)
        {
            var newmessage = FixMessage(message);
            ToLogFile(newmessage, level);
            if (level > LogLevel.Info || force)
                switch (level)
                {
                    case LogLevel.Debug:
                        break;
                    case LogLevel.Info:
                        ConsoleWindow.Print($"[S.AddonsOverhaul - {level}]: {newmessage}");
                        break;
                    case LogLevel.Warn:
                        ConsoleWindow.PrintAction($"[S.AddonsOverhaul - {level}]: {newmessage}");
                        break;
                    case LogLevel.Error:
                        ConsoleWindow.PrintError($"[S.AddonsOverhaul - {level}]: {newmessage}");
                        break;
                    case LogLevel.Fatal:
                        Functions.MessageBox($"[S.AddonsOverhaul - {level}]: {newmessage}", "[FATAL ERROR]");
                        Thread.Sleep(int.MaxValue);
                        break;
                }
        }

        private static void ToLogFile(string message, LogLevel level = LogLevel.Info)
        {
            if (File.Exists(Constants.LogPath))
                new DisposableStreamWriter(Constants.LogPath, true).WriteLineAndDispose(
                    $"[S.AddonsOverhaul - {level}]: {message}");

            Console.WriteLine($"[S.AddonsOverhaul - {level}]: {message}");
        }

        private static string FixMessage(string message)
        {
            return message.Replace("\n ", "\n\t").Replace("\n", "\n\t");
        }
    }

    internal class DisposableFileStream
    {
        public DisposableFileStream(FileStream stream)
        {
            Stream = stream;
        }

        public DisposableFileStream(string path, FileMode fileMode)
        {
            Stream = new FileStream(path, fileMode);
        }

        public FileStream Stream { get; private set; }

        public void WriteAndDispose(byte[] buffer, int offset, int count)
        {
            Write(buffer, offset, count);
            Dispose();
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            Stream.Write(buffer, offset, count);
        }

        public void Dispose()
        {
            Stream.Dispose();
            Stream = null;
        }
    }

    internal class DisposableStreamWriter
    {
        public DisposableStreamWriter(StreamWriter stream)
        {
            Stream = stream;
        }

        public DisposableStreamWriter(string path, bool append)
        {
            Stream = new StreamWriter(path, append);
        }

        public StreamWriter Stream { get; private set; }

        public void WriteLineAndDispose(string text)
        {
            WriteLine(text);
            Dispose();
        }

        public void WriteAndDispose(string text)
        {
            WriteLine(text);
            Dispose();
        }

        public void Write(string text)
        {
            Stream.Write(text);
        }

        public void WriteLine(string text)
        {
            Stream.WriteLine(text);
        }

        public void Dispose()
        {
            Stream.Dispose();
            Stream = null;
        }
    }

    internal class DisposableMemoryStream
    {
        public DisposableMemoryStream()
        {
            Stream = new MemoryStream();
        }

        public DisposableMemoryStream(MemoryStream stream)
        {
            Stream = stream;
        }

        public MemoryStream Stream { get; private set; }

        public void RunThenDispose(Action<MemoryStream> action)
        {
            Run(action);
            Dispose();
        }

        public void Run(Action<MemoryStream> action)
        {
            action(Stream);
        }

        public void Dispose()
        {
            Stream.Dispose();
            Stream = null;
        }
    }
}