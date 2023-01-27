// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

using S.AddonsOverhaul.Patcher.Core.Loggers;

namespace S.AddonsOverhaul.Patcher.Core
{
    public static class Logger
    {
        public static ILogger Current { get; private set; }

        public static void Init()
        {
            Current = new ConsoleLogger();
        }
    }
}