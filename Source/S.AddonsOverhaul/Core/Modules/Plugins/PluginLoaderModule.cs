using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using S.AddonsOverhaul.Core.Interfaces.Log;
using S.AddonsOverhaul.Core.Interfaces.Module;
using S.AddonsOverhaul.Core.Interfaces.Plugin;
using UnityEngine;

namespace S.AddonsOverhaul.Core.Modules.Plugins
{
    internal class PluginLoaderModule : IModule
    {
        private readonly Dictionary<string, PluginInfo> _plugins = new();

        public Dictionary<string, PluginInfo>.ValueCollection LoadedPlugins => _plugins.Values;
        public int NumLoadedPlugins => _plugins.Count;
        public string LoadingCaption => "Starting up plugins...";

        public void Initialize()
        {
        }

        public IEnumerator Load()
        {
            AddonsLogger.Log("Loading plugin assemblies...");

            foreach (var compiledPlugin in PluginCompilerModule.CompiledPlugins)
            {
                AddonsLogger.Log($"Loading plugin assembly '{compiledPlugin.AssemblyFile}'");
                LoadPlugin(compiledPlugin.AddonName, compiledPlugin.AssemblyFile);
                yield return new WaitForEndOfFrame();
            }

            AddonsLogger.Log($"Loaded {_plugins.Count} plugins");
        }

        public void Shutdown()
        {
            UnloadAllPlugins();
        }

        public void LoadPlugin(string addonName, string pluginAssembly)
        {
            if (_plugins.TryGetValue(addonName, out var prevPlugin))
            {
                AddonsLogger.Log("Plugin '" + addonName + "' already loaded!", LogLevel.Error);
                foreach (var prevPluginPlugin in prevPlugin.Plugins)
                    try
                    {
                        prevPluginPlugin.OnUnload();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }

                _plugins.Remove(addonName);
            }

            var tempPluginFile = Path.GetFileNameWithoutExtension(pluginAssembly) + Guid.NewGuid() + ".dll";
            var tempPluginPath = Path.GetFullPath(Path.Combine(Constants.AddonCachePath, tempPluginFile));
            var split = pluginAssembly.Split("\\");
            File.Copy(Path.Combine(Constants.AddonPath, addonName, split[split.Length-1]), tempPluginPath);

            var assembly = Assembly.LoadFile(tempPluginPath);
            AddonsLogger.Log($"Plugin assembly {pluginAssembly} loaded from {tempPluginPath}");

            var plugins = new List<IPlugin>();
            foreach (var type in assembly.GetTypes())
                if (typeof(IPlugin).IsAssignableFrom(type))
                    try
                    {
                        var instance = (IPlugin)Activator.CreateInstance(type);
                        instance.OnLoad();
                        plugins.Add(instance);

                        AddonsLogger.Log("Activated plugin " + type);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }

            _plugins.Add(addonName, new PluginInfo
            {
                Assembly = assembly,
                Plugins = plugins.ToArray()
            });
        }

        public void UnloadAllPlugins()
        {
            foreach (var plugin in _plugins) UnloadPlugin(plugin.Value);

            _plugins.Clear();
        }

        private void UnloadPlugin(PluginInfo pluginInfo)
        {
            foreach (var plugin in pluginInfo.Plugins)
                plugin.OnUnload();
        }
    }
}