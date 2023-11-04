using System.Collections.Generic;
using System.IO;
using S.AddonsOverhaul.Core.Interfaces.Log;

namespace S.AddonsOverhaul.Core
{
    internal static class LocalMods
    {
        private static readonly string LocalModsDirectory = LoaderManager.IsDedicatedServer
            ? GetDedicatedServerModsDirectory()
            : Constants.ModsPath;

        public static IEnumerable<string> GetLocalModDirectories()
        {
            if (string.IsNullOrEmpty(LocalModsDirectory))
            {
                AddonsLogger.Log("Could not locate mods directory, no mods getting initialized.\n" + LocalModsDirectory,
                    LogLevel.Error);
                return new string[] { };
            }

            if (!Directory.Exists(LocalModsDirectory)) Directory.CreateDirectory(LocalModsDirectory);

            AddonsLogger.Log($"Trying to load mod from {LocalModsDirectory}");

            var directories = Directory.GetDirectories(LocalModsDirectory);
            var modDirectory = new List<string>();

            foreach (var directory in directories)
            {
                AddonsLogger.Log($"Found local mod {directory}");
                modDirectory.Add(directory);
            }

            return modDirectory.ToArray();
        }

        private static string GetDedicatedServerModsDirectory()
        {
            if (!File.Exists("default.ini"))
            {
                AddonsLogger.Log("default.ini file not found!", LogLevel.Warn);
                return null;
            }

            foreach (var line in File.ReadLines("default.ini"))
            {
                if (!line.Contains("MODPATH=")) continue;
                var modpath = line.Split("MODPATH=")[1];
                AddonsLogger.Log($"Found mod path: {modpath}");
                return modpath;
            }

            return null;
        }
    }
}