using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using S.AddonsOverhaul.Core.Interfaces.Module;
using UnityEngine;

namespace S.AddonsOverhaul.Core.Modules.HarmonyLib
{
    internal class HarmonyModule : IModule
    {
        private static readonly List<Action<Harmony>> _patchers = new();
        private Harmony _harmony;
        public string LoadingCaption => "Initializing Harmony library...";

        public void Initialize()
        {
            _harmony = new Harmony("com.s.addonsoverhaul");
        }

        public IEnumerator Load()
        {
            AddonsLogger.Log("Patching game assembly using Harmony...");
            foreach (var plugin in LoaderManager.Instance.PluginLoader.LoadedPlugins)
            {
                AddonsLogger.Log($"Applying patches from assembly '{plugin.Assembly.FullName}'");
                _harmony.PatchAll(plugin.Assembly);
                yield return new WaitForEndOfFrame();
            }

            foreach (var patcher in _patchers)
            {
                AddonsLogger.Log($"Applying patches from assembly '{patcher.Target.GetType().Name}'");
                patcher(_harmony);
            }

            AddonsLogger.Log(
                $"Finished {LoaderManager.Instance.PluginLoader.LoadedPlugins.Count} patches to game assembly");
        }

        public void Shutdown()
        {
            _harmony.UnpatchAll();
            _harmony = null;
        }

        public static void RegisterPatcher(Action<Harmony> patcher)
        {
            _patchers.Add(patcher);
        }
    }
}