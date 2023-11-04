using System.Collections;
using Assets.Scripts.UI;
using ImGuiNET;
using S.AddonsOverhaul.Core.Interfaces.Log;
using S.AddonsOverhaul.Core.Interfaces.Module;
using UnityEngine;

namespace S.AddonsOverhaul.Core.Modules.LiveReload
{
    internal class LiveReloadModule : IModule
    {
        private bool _isRecompiling;

        private bool _liveReloadEnabled;

        public string LoadingCaption => "Initializing live reload module...";

        public void Initialize()
        {
            ImGuiUn.Layout += OnLayout;
        }

        public IEnumerator Load()
        {
            _liveReloadEnabled = LoaderManager.Instance.AddonsSetting.LiveReloadEnabled;

            yield break;
        }

        public void Shutdown()
        {
            ImGuiUn.Layout -= OnLayout;
        }

        private void OnLayout()
        {
            if (!_isRecompiling)
                return;

            ImGuiLoadingScreen.ShowLoadingScreen(null, ImGuiLoadingScreen.Singleton.State, 0.1f);
        }

        public void Update()
        {
            _liveReloadEnabled = LoaderManager.Instance.AddonsSetting.LiveReloadEnabled;
            if (!_liveReloadEnabled || !LoaderManager.Instance.IsLoaded) return;

            if (Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt))
                LoaderManager.Instance.StartCoroutine(Reload());
        }

        internal IEnumerator Reload()
        {
            if (_isRecompiling)
            {
                AddonsLogger.Log("Already recompiling!", LogLevel.Warn);
                yield break;
            }

            _isRecompiling = true;

            yield return null;

            var uniTask = ImGuiLoadingScreen.Singleton.SetState("Live Reload - Unloading plugins...");
            yield return uniTask;
            uniTask = ImGuiLoadingScreen.Singleton.SetProgress(0.1f);
            yield return uniTask;

            yield return new WaitForSeconds(0.1f);

            AddonsLogger.Log("Unloading plugins");

            LoaderManager.Instance.Harmony.Shutdown();
            LoaderManager.Instance.Harmony.Initialize();
            LoaderManager.Instance.PluginLoader.UnloadAllPlugins();

            uniTask = ImGuiLoadingScreen.Singleton.SetState("Live Reload - Recompiling plugins...");
            yield return uniTask;
            uniTask = ImGuiLoadingScreen.Singleton.SetProgress(0.25f);
            yield return uniTask;

            AddonsLogger.Log("Recompiling plugins");
            yield return new WaitForSeconds(0.1f);
            yield return LoaderManager.Instance.PluginCompiler.Load();

            uniTask = ImGuiLoadingScreen.Singleton.SetState("Live Reload - Reloading plugins...");
            yield return uniTask;
            uniTask = ImGuiLoadingScreen.Singleton.SetProgress(0.50f);
            yield return uniTask;

            AddonsLogger.Log("Reloading plugins");
            yield return new WaitForSeconds(0.1f);
            yield return LoaderManager.Instance.PluginLoader.Load();

            uniTask = ImGuiLoadingScreen.Singleton.SetState("Live Reload - Patching plugins...");
            yield return uniTask;
            uniTask = ImGuiLoadingScreen.Singleton.SetProgress(0.50f);
            yield return uniTask;

            AddonsLogger.Log("Re-patching game using harmony");
            yield return new WaitForSeconds(0.1f);
            yield return LoaderManager.Instance.Harmony.Load();

            AddonsLogger.Log("Recompilation done");

            _isRecompiling = false;
        }
    }
}