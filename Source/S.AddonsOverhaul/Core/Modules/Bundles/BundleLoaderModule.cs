using System.Collections;
using System.IO;
using Assets.Scripts.Networking;
using Assets.Scripts.Networking.Transports;
using S.AddonsOverhaul.Core.Interfaces.Log;
using S.AddonsOverhaul.Core.Interfaces.Module;
using S.AddonsOverhaul.API;
using UnityEngine;

namespace S.AddonsOverhaul.Core.Modules.Bundles
{
    internal class BundleLoaderModule : IModule
    {
        public string LoadingCaption => "Loading custom content...";

        public void Initialize()
        {
        }

        public IEnumerator Load()
        {
            AddonsLogger.Log("Loading custom content bundles...");

            var query = NetworkManager.GetLocalAndWorkshopItems(SteamTransport.WorkshopType.Mod).GetAwaiter();

            while (!query.IsCompleted)
                yield return null;

            var result = query.GetResult();

            foreach (var itemWrap in result)
            {
                var modDirectory = itemWrap.DirectoryPath;
                if (modDirectory == null)
                {
                    AddonsLogger.Log($"Missing mod directory for mod with id={itemWrap.Id}", LogLevel.Warn);
                    continue;
                }

                yield return LoadBundleFromModDirectory(modDirectory);
            }
        }

        public void Shutdown()
        {
            AddonsLogger.Log("Unloading all custom content bundles...");

            foreach (var bundle in BundleManager.LoadedAssetBundles) bundle.Unload(true);
        }

        private IEnumerator LoadBundleFromModDirectory(string modDirectory)
        {
            var contentDirectory = Path.Combine(modDirectory, "Content");

            if (!Directory.Exists(contentDirectory)) yield break;

            AddonsLogger.Log(contentDirectory);

            var bundles = Directory.GetFiles(contentDirectory, "*.asset", SearchOption.TopDirectoryOnly);

            if (bundles.Length == 0) yield break;

            foreach (var bundleFile in bundles)
            {
                var bundle = AssetBundle.LoadFromFileAsync(bundleFile);

                yield return bundle;

                AddonsLogger.Log($"Loaded asset bundle '{bundle.assetBundle.name}'");

                BundleManager.LoadedAssetBundles.Add(bundle.assetBundle);
            }
        }
    }
}