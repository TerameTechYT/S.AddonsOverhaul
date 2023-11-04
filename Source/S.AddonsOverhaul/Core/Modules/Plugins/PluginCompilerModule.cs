using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Networking;
using Assets.Scripts.Networking.Transports;
using Assets.Scripts.Serialization;
using S.AddonsOverhaul.Core.Compilation;
using S.AddonsOverhaul.Core.Interfaces.Log;
using S.AddonsOverhaul.Core.Interfaces.Module;
using S.AddonsOverhaul.Core.Interfaces.Plugin;
using UnityEngine;

namespace S.AddonsOverhaul.Core.Modules.Plugins
{
    internal class PluginCompilerModule : IModule
    {
        internal static readonly List<AddonPlugin> CompiledPlugins = new();

        public string LoadingCaption => "Compiling plugins...";

        public void Initialize()
        {
        }

        public IEnumerator Load()
        {
            CompiledPlugins.Clear();

            AddonsLogger.Log("Starting plugins compilation...");

            if (!Directory.Exists(Constants.AddonCachePath))
                Directory.CreateDirectory(Constants.AddonCachePath);

            yield return LoadLocalPlugins();

            if (!LoaderManager.IsDedicatedServer) yield return LoadWorkshopPlugins();
        }

        public void Shutdown()
        {
        }

        private IEnumerator LoadLocalPlugins()
        {
            var localPluginDirectories = LocalMods.GetLocalModDirectories();

            foreach (var localPluginDirectory in localPluginDirectories)
            {
                try
                {
                    var addonDirectory = localPluginDirectory;
                    var addonName = XmlSerialization
                        .Deserialize<ModAbout>(Path.Combine(addonDirectory, "About", "About.xml"),
                            "ModMetadata").Name;
                    var addonAuthor = XmlSerialization
                        .Deserialize<ModAbout>(Path.Combine(addonDirectory, "About", "About.xml"),
                            "ModMetadata").Author;

                    var assemblyName =
                        addonName.Trim() +
                        " - AssemblyCSharp";
                    var assemblyFile = Path.Combine(Constants.AddonCachePath, assemblyName + ".dll");

                    if (!Directory.Exists(addonDirectory))
                    {
                        AddonsLogger.Log(
                            $"Could not load addon plugin '{addonName}' because directory '{addonDirectory}' does not exist!",
                            LogLevel.Warn);
                        continue;
                    }

                    var addonScripts = Directory.GetFiles(addonDirectory, "*.cs", SearchOption.AllDirectories);

                    var sourceFilesList = new List<string>(addonScripts);

                    sourceFilesList.RemoveAll(x =>
                        x.Contains(Path.DirectorySeparatorChar + "Properties" + Path.DirectorySeparatorChar));
                    sourceFilesList.RemoveAll(x =>
                        x.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar));
                    sourceFilesList.RemoveAll(x =>
                        x.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar));

                    addonScripts = sourceFilesList.ToArray();

                    if (addonScripts.Length == 0)
                    {
                        if (!Directory.Exists(addonDirectory = "Scripts"))
                            AddonsLogger.Log($"Skipping compilation of, '{addonName}' (No 'Scripts' folder)!",
                                LogLevel.Warn);
                        else
                            AddonsLogger.Log($"No scripts found in addon '{addonName}'!", LogLevel.Warn);
                        continue;
                    }

                    foreach (var file in addonScripts)
                        if (!File.Exists(file))
                            AddonsLogger.Log($"Could not find source file '{file}'.", LogLevel.Error);

                    if (File.Exists(assemblyFile))
                    {
                        AddonsLogger.Log($"Removing old plugin assembly file '{assemblyFile}'");
                        File.Delete(assemblyFile);
                    }

                    if (!Compiler.Compile(addonName, addonAuthor, addonScripts))
                    {
                        AddonsLogger.Log($"Could not compile addon plugin '{addonName}'!", LogLevel.Error);
                        continue;
                    }

                    CompiledPlugins.Add(new AddonPlugin(addonName, assemblyFile));
                }
                catch (Exception ex)
                {
                    AddonsLogger.Log($"Failed to compile plugin from '{localPluginDirectory}'. Exception:\n{ex}",
                        LogLevel.Error);
                }

                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator LoadWorkshopPlugins()
        {
            yield return null;

            var query = NetworkManager.GetLocalAndWorkshopItems(SteamTransport.WorkshopType.Mod).GetAwaiter();
            while (!query.IsCompleted)
                yield return null;

            var result = query.GetResult();

            foreach (var itemWrap in result)
            {
                if (itemWrap.IsLocal()) continue;

                try
                {
                    var addonDirectory = itemWrap.DirectoryPath;
                    var addonName = XmlSerialization
                        .Deserialize<ModAbout>(Path.Combine(addonDirectory, "About", "About.xml"),
                            "ModMetadata").Name;
                    var addonAuthor = XmlSerialization
                        .Deserialize<ModAbout>(Path.Combine(addonDirectory, "About", "About.xml"),
                            "ModMetadata").Author;

                    var assemblyName =
                        addonName.Trim() +
                        " - AssemblyCSharp";
                    var assemblyFile = Path.Combine(Constants.AddonPath, addonName, assemblyName + ".dll");

                    if (!Directory.Exists(addonDirectory))
                    {
                        AddonsLogger.Log(
                            $"Could not load addon plugin '{addonName}' because directory '{addonDirectory}' does not exist!",
                            LogLevel.Warn);
                        continue;
                    }

                    var addonScripts = Directory.GetFiles(addonDirectory, "*.cs", SearchOption.AllDirectories);

                    if (addonScripts.Length == 0) continue;
                    foreach (var file in addonScripts)
                        if (!File.Exists(file))
                            AddonsLogger.Log($"Could not find source file '{file}'.", LogLevel.Error);

                    if (File.Exists(assemblyFile))
                    {
                        AddonsLogger.Log($"Removing old plugin assembly file '{assemblyFile}'");
                        File.Delete(assemblyFile);
                    }

                    if (!Compiler.Compile(addonName, addonAuthor, addonScripts))
                    {
                        AddonsLogger.Log($"Could not compile addon plugin '{addonName}'!", LogLevel.Error);
                        continue;
                    }

                    CompiledPlugins.Add(new AddonPlugin(addonName, assemblyFile));
                }
                catch (Exception ex)
                {
                    AddonsLogger.Log($"Failed to compile plugin from '{itemWrap.DirectoryPath}'. Exception:\n{ex}",
                        LogLevel.Error);
                }
            }
        }
    }
}