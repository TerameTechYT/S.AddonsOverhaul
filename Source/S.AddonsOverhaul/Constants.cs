using System;
using System.IO;

namespace S.AddonsOverhaul
{
    public static class Constants
    {
        public static string VersionString => "v0.1.6";
        public static int VersionNumber => 4;

        public static string VersionUrl =>
            "https://raw.githubusercontent.com/TerameTechYT/S.AddonsOverhaul/master/VERSION";

        public static string GamePath => AppContext.BaseDirectory;
        public static string ManagerPath => Path.Combine(GamePath, "AddonManager");
        public static string DocumentsPath => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static string MyGamesPath => Path.Combine(DocumentsPath, "My Games");
        public static string StationeersDocumentsPath => Path.Combine(MyGamesPath, "Stationeers");
        public static string ModsPath => Path.Combine(StationeersDocumentsPath, "mods");
        public static string DataPath => Path.Combine(StationeersDocumentsPath, "addons");
        public static string ConfigPath => Path.Combine(DataPath, "settings.xml");
        public static string LogPath => Path.Combine(DataPath, "log.log");
        public static string AddonPath => Path.Combine(DataPath, "plugins");
        public static string AddonCachePath => Path.Combine(DataPath, "temp");
    }
}