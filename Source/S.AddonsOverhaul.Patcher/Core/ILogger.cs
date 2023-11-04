namespace S.AddonsOverhaul.Patcher.Core
{
    internal interface ILogger
    {
        void Log(string message);

        void LogWarning(string message);

        void LogError(string message);

        void LogFatal(string message);
    }
}