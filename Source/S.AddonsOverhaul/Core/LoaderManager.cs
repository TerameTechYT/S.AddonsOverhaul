using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Serialization;
using Assets.Scripts.UI;
using ImGuiNET;
using S.AddonsOverhaul.Core.Configs;
using S.AddonsOverhaul.Core.Interfaces.Log;
using S.AddonsOverhaul.Core.Interfaces.Module;
using S.AddonsOverhaul.Core.Interfaces.Settings;
using S.AddonsOverhaul.Core.Modules.Bundles;
using S.AddonsOverhaul.Core.Modules.HarmonyLib;
using S.AddonsOverhaul.Core.Modules.LiveReload;
using S.AddonsOverhaul.Core.Modules.Plugins;
using S.AddonsOverhaul.Core.Modules.VersionCheck;
using S.AddonsOverhaul.Core.Modules.Workshop;
using Steamworks;
using UnityEngine;

namespace S.AddonsOverhaul.Core
{
    internal class LoaderManager : MonoBehaviour
    {
        private static LoaderManager _instance;

        private readonly List<IModule> _modules = new();
        private bool _isLoading;

        private bool DidConsoleOpen;

        private bool needSave;

        private bool open;
        public List<AddonsSettingItem> Settings;

        public static LoaderManager Instance
        {
            get
            {
                if (_instance)
                    return _instance;

                var gameObject = new GameObject("ModLoader");
                _instance = gameObject.AddComponent<LoaderManager>();

                return _instance;
            }
        }

        public static bool IsDedicatedServer { get; private set; }

        public WorkshopModule Workshop { get; private set; }

        public VersionCheckModule VersionCheck { get; private set; }

        public PluginCompilerModule PluginCompiler { get; private set; }

        public BundleLoaderModule BundleLoader { get; private set; }

        public PluginLoaderModule PluginLoader { get; private set; }

        public HarmonyModule Harmony { get; private set; }

        public LiveReloadModule LiveReload { get; private set; }

        public bool IsLoaded { get; set; }

        public AddonsSettings AddonsSetting { get; private set; }

        public void Activate()
        {
            if (File.Exists(Constants.LogPath))
                File.Delete(Constants.LogPath);
            if (!Directory.Exists(Constants.DataPath))
                Directory.CreateDirectory(Constants.DataPath);
            if (!Directory.Exists(Constants.AddonPath))
                Directory.CreateDirectory(Constants.AddonPath);
            if (!Directory.Exists(Constants.AddonCachePath))
                Directory.CreateDirectory(Constants.AddonCachePath);

            AddonsLogger.InitializeLog();
            AddonsLogger.Log($"Loader Version {Constants.VersionString}", LogLevel.Info, true);
        }

        private void Awake()
        {
            if (!File.Exists(Constants.ConfigPath))
            {
                AddonsSetting = new AddonsSettings();
                SaveAddonsSettings(true);
            }
            else
            {
                AddonsSetting = XmlSerialization.Deserialize<AddonsSettings>(Constants.ConfigPath);
                if (AddonsSetting.FirstRun)
                {
                    AddonsSetting.FirstRun = false;
                    Functions.MessageBox(
                        "You can press F7 to open the configuration menu. This message will only appear once.",
                        "Information");
                    SaveAddonsSettings(true);
                }
            }

            Settings = new List<AddonsSettingItem>
            {
                new("Console", AddonsSetting.ConsoleEnabled),
                new("LiveReload", AddonsSetting.LiveReloadEnabled)
            };

            if (AddonsSetting.ConsoleEnabled) AddonsLogger.OpenConsole();

            DontDestroyOnLoad(gameObject);

            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
            Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
            Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.None);

            IsDedicatedServer =
                File.Exists(
                    "rocketstation_DedicatedServer.exe");
            if (IsDedicatedServer)
                AddonsLogger.Log("Running on a dedicated server!");

            if (!IsDedicatedServer)
                Workshop = InitializeModule<WorkshopModule>();

            VersionCheck = InitializeModule<VersionCheckModule>();
            PluginCompiler = InitializeModule<PluginCompilerModule>();
            BundleLoader = InitializeModule<BundleLoaderModule>();
            PluginLoader = InitializeModule<PluginLoaderModule>();
            Harmony = InitializeModule<HarmonyModule>();
            LiveReload = InitializeModule<LiveReloadModule>();

            ImGuiUn.Layout += OnLayout;
        }

        private void OnLayout()
        {
            if (!_isLoading)
                return;

            ImGuiLoadingScreen.ShowLoadingScreen(null, ImGuiLoadingScreen.Singleton.State, 0.1f);
        }

        private IEnumerator Start()
        {
            if (!IsDedicatedServer)
            {
                while (!SteamClient.IsValid) yield return null;

                _isLoading = true;

                yield return null;

                var numModules = _modules.Count;
                var moduleIdx = 0;
                foreach (var module in _modules)
                {
                    var progress = Mathf.Clamp01(numModules / (float)moduleIdx);

                    var uniTask = ImGuiLoadingScreen.Singleton.SetState(module.LoadingCaption);
                    yield return uniTask;

                    uniTask = ImGuiLoadingScreen.Singleton.SetProgress(Mathf.Lerp(0.35f, 1.0f, progress));
                    yield return uniTask;

                    yield return module.Load();
                    moduleIdx++;
                }

                _isLoading = false;
            }
            else
            {
                foreach (var module in _modules) yield return module.Load();
            }

            IsLoaded = true;
        }

        private void Update()
        {
            LiveReload?.Update();
        }

        private void OnDestroy()
        {
            ImGuiUn.Layout -= OnLayout;

            foreach (var module in _modules)
                module.Shutdown();
        }

        private void OnGUI()
        {
            if (KeyManager.GetButtonDown(KeyCode.F7)) open = !open;

            if (open)
            {
                ShowElements();
                var console = Settings.Find(x => x.Name == "Console");
                var livereload = Settings.Find(x => x.Name == "LiveReload");

                if (AddonsSetting.ConsoleEnabled && !console.DidChange)
                {
                    needSave = true;
                    console.DidChange = true;
                    console.Value = true;
                    AddonsLogger.Log("Console has been enabled!");
                    if (!DidConsoleOpen)
                    {
                        DidConsoleOpen = true;
                        AddonsLogger.Log("Console has been opened.");
                        AddonsLogger.OpenConsole();
                    }
                }
                else if (!AddonsSetting.ConsoleEnabled)
                {
                    needSave = true;
                    console.DidChange = false;
                    console.Value = false;
                }

                if (AddonsSetting.LiveReloadEnabled && !livereload.DidChange)
                {
                    needSave = true;
                    livereload.DidChange = true;
                    livereload.Value = true;
                    AddonsLogger.Log("Live reload enabled! Press CTRL+ALT+R to reload all plugins.");
                }
                else if (!AddonsSetting.LiveReloadEnabled)
                {
                    needSave = true;
                    livereload.DidChange = false;
                    livereload.Value = false;
                }

                if (ShowElement(AddonsSettingElement.OpenLog))
                    Functions.OpenLogFile();
            }

            SaveAddonsSettings();
        }

        private void ShowElements(AddonsSettingElement type = AddonsSettingElement.All)
        {
            switch (type)
            {
                case AddonsSettingElement.All:
                {
                    AddonsSetting.ConsoleEnabled = GUI.Toggle(new Rect(5.0f, 5.0f, Screen.width, 25.0f),
                        AddonsSetting.ConsoleEnabled, "Toggle Console");
                    AddonsSetting.LiveReloadEnabled = GUI.Toggle(new Rect(5.0f, 30.0f, Screen.width, 25.0f),
                        AddonsSetting.LiveReloadEnabled, "Toggle LiveReload");
                    GUI.Label(new Rect(5.0f, 80.0f, Screen.width, 25.0f),
                        "S.AddonsOverhaul (" + Constants.VersionString + ") - " + PluginLoader.NumLoadedPlugins +
                        " plugin(s) loaded");
                }
                    break;

                case AddonsSettingElement.Console:
                {
                    AddonsSetting.ConsoleEnabled = GUI.Toggle(new Rect(5.0f, 5.0f, Screen.width, 25.0f),
                        AddonsSetting.ConsoleEnabled, "Toggle Console");
                }
                    break;

                case AddonsSettingElement.LiveReload:
                {
                    AddonsSetting.LiveReloadEnabled = GUI.Toggle(new Rect(5.0f, 30.0f, Screen.width, 25.0f),
                        AddonsSetting.LiveReloadEnabled, "Toggle LiveReload");
                }
                    break;

                case AddonsSettingElement.Info:
                {
                    GUI.Label(new Rect(5.0f, 80.0f, Screen.width, 25.0f),
                        "S.AddonsOverhaul (" + Constants.VersionString + ") - " + PluginLoader.NumLoadedPlugins +
                        " plugin(s) loaded");
                }
                    break;
            }
        }

        private bool ShowElement(AddonsSettingElement type = AddonsSettingElement.None)
        {
            switch (type)
            {
                case AddonsSettingElement.Console:
                    return GUI.Toggle(new Rect(5.0f, 5.0f, Screen.width, 25.0f),
                        AddonsSetting.ConsoleEnabled, "Toggle Console");

                case AddonsSettingElement.LiveReload:
                    return GUI.Toggle(new Rect(5.0f, 30.0f, Screen.width, 25.0f),
                        AddonsSetting.LiveReloadEnabled, "Toggle LiveReload");

                case AddonsSettingElement.OpenLog:
                    return GUI.Button(new Rect(5.0f, 55.0f, 150f, 25.0f),
                        "Open Log File");

                case AddonsSettingElement.None:
                    return false;

                default:
                    return false;
            }
        }

        private TModuleType InitializeModule<TModuleType>() where TModuleType : IModule, new()
        {
            var moduleInstance = new TModuleType();
            _modules.Add(moduleInstance);
            moduleInstance.Initialize();
            return moduleInstance;
        }

        private void SaveAddonsSettings(bool force = false)
        {
            if (needSave || force)
            {
                if (!XmlSerialization.Serialize(AddonsSetting, Constants.ConfigPath))
                    AddonsLogger.Log("Cannot save AddonsSettings!", LogLevel.Error);
                needSave = false;
            }
        }
    }
}