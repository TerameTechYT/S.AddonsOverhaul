using S.AddonsOverhaul.Patcher.Core;

namespace S.AddonsOverhaul.Patcher
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Logger.Init();

            StandalonePatcher.Patch();
        }
    }
}