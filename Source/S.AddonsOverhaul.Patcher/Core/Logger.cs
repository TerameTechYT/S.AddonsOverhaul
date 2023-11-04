using S.AddonsOverhaul.Patcher.Core.Loggers;

namespace S.AddonsOverhaul.Patcher.Core
{
    internal static class Logger
    {
        public static ILogger Current { get; private set; }

        public static void Init()
        {
            Current = new ConsoleLogger();
        }
    }
}