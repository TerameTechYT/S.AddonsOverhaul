using System;
using System.IO;
using S.AddonsOverhaul.Patcher.Core.Patchers;

namespace S.AddonsOverhaul.Patcher.Core
{
    internal static class StandalonePatcher
    {
        public static void Patch()
        {
            Logger.Current.Log("Startup");

            string installInstance;

            if (File.Exists(Constants.GameExe))
            {
                installInstance = Constants.GameExe;
            }
            else if (File.Exists(Constants.ServerExe))
            {
                installInstance = Constants.ServerExe;
            }
            else
            {
                installInstance = null;
                Logger.Current.LogFatal(
                    $"Could not find executable file '{Constants.GameExe}' or {Constants.ServerResourcesDir}!");
            }

            var patcher = new MonoPatcher();

            try
            {
                patcher.Load(installInstance);

                if (!patcher.IsPatched())
                    patcher.Patch();
            }
            catch (Exception e)
            {
                Logger.Current.LogFatal(e.ToString());
            }

            patcher.Dispose();
        }
    }
}