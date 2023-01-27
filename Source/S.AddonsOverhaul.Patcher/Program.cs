// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

using S.AddonsOverhaul.Patcher.Core;

namespace S.AddonsOverhaul.Patcher
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Initialize logger
            Logger.Init();

            // Patch the game if needed
            StandalonePatcher.Patch();
        }
    }
}