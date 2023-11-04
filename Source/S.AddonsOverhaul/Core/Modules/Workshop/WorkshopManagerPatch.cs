using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Assets.Scripts.Networking.Transports;
using CSharpUtilities.Core;
using JetBrains.Annotations;
using S.AddonsOverhaul.Core.Interfaces.Log;
using UnityEngine.UI;
using Util.Commands;

namespace S.AddonsOverhaul.Core.Modules.Workshop
{
    internal static class WorkshopManagerPatch
    {
        private static readonly Regex[] ValidFileNames =
        {
            new(@".*\.cs$"),
            new(@".*\.xml$"),
            new(@".*\.png$"),
            new(@".*\.asset$"),
            new(@"^LICENSE$")
        };

        private static readonly Regex[] ValidDirectoryNames =
        {
            new(@"^About$"),
            new(@"^Content$"),
            new(@"^GameData$"),
            new(@"^Scripts$")
        };

        private static WorkshopModListItem _currentMod;

        private static bool changed;

        private static WorkshopModListItem GetSelectedModData(WorkshopMenu __instance)
        {
            return ReflectionHelper.ReadPrivateField<WorkshopModListItem>(__instance.GetType(), "_selectedModItem",
                __instance);
        }

        [UsedImplicitly]
        public static void RefreshButtons(WorkshopMenu __instance)
        {
            var selectedMod = GetSelectedModData(__instance);

            if (!selectedMod.Data.IsLocal) return;

            __instance.SelectedModButtonRight.GetComponent<Button>().onClick.RemoveAllListeners();
            __instance.SelectedModButtonRight.GetComponent<Button>().onClick
                .AddListener(() => PublishModOverride(__instance));
        }

        private static void PublishModOverride(WorkshopMenu __instance)
        {
            _currentMod = GetSelectedModData(__instance);
            var modData = _currentMod.Data;

            AddonsLogger.Log($"Publishing mod '{modData.GetAboutData().Name}' using Stationeers.Addon filter");

            var origItemContentPath = modData.LocalPath;
            var tempItemContentPath = origItemContentPath + "_temp";

            if (Directory.Exists(tempItemContentPath)) Directory.CreateDirectory(tempItemContentPath);

            foreach (var itemFilePath in Directory.GetFiles(origItemContentPath))
            {
                var fileName = new FileInfo(itemFilePath).Name;

                var validFile = ValidFileNames.Any(regex => regex.IsMatch(fileName));

                if (validFile)
                    File.Copy(itemFilePath, tempItemContentPath + Path.GetFileName(itemFilePath), true);
            }

            foreach (var itemFolderPath in Directory.GetDirectories(origItemContentPath))
            {
                var dirName = new FileInfo(itemFolderPath).Name;

                var validDir = ValidDirectoryNames.Any(regex => regex.IsMatch(dirName));

                if (validDir)
                    DirectoryHelper.CopyFiles(itemFolderPath,
                        tempItemContentPath + Path.DirectorySeparatorChar + dirName);
            }

            modData.LocalPath = tempItemContentPath;
            _currentMod.SetData(modData);

            AddonsLogger.Log("Created temporary workshop item directory " + tempItemContentPath);

            Publish();
        }

        private static async void Publish()
        {
            var mod = _currentMod.Data;
            var aboutData = mod.GetAboutData();
            var localPath = mod.LocalPath;

            AddonsLogger.Log("Uploading mod from " + localPath);

            var itemDetail = new SteamTransport.WorkShopItemDetail
            {
                Title = aboutData.Name,
                Path = localPath,
                PreviewPath = localPath + "\\About\\thumb.png",
                Description = aboutData.Description,
                PublishedFileId = aboutData.WorkshopHandle,
                Type = SteamTransport.WorkshopType.Mod,
                CustomTags = aboutData.Tags
            };

            var (success, fileId) = await SteamTransport.Workshop_PublishItemAsync(itemDetail);

            if (!success)
            {
                AddonsLogger.Log("Failed to publish mod to Steam Workshop! If error is 'FileNotFound', " +
                                 "mod has been deleted from workshop or you do not have access to it." +
                                 "Remove WorkshopHandle tag from About.xml file", LogLevel.Error);
                Cleanup();
                return;
            }

            Cleanup();

            itemDetail.PublishedFileId = fileId;


            ReflectionHelper.InvokePrivateMethod(typeof(WorkshopMenu), "SaveWorkShopFileHandle", null,
                new object[] { itemDetail, mod });
        }

        private static void Cleanup()
        {
            var modData = _currentMod.Data;
            var tempItemContentPath = modData.LocalPath;
            var origItemContentPath = tempItemContentPath.Replace("_temp", "");

            AddonsLogger.Log("Checking for temporary workshop item directory " + tempItemContentPath);
            if (Directory.Exists(tempItemContentPath) && tempItemContentPath.Contains("_temp"))
            {
                AddonsLogger.Log("Cleared temporary workshop item directory " + tempItemContentPath);
                Directory.Delete(tempItemContentPath, true);
            }

            modData.LocalPath = origItemContentPath;
            _currentMod.SetData(modData);
        }

        public static void OnDisable(WorkshopMenu __instance)
        {
            if (changed) new RestartCommand().Execute(new string[] { });
        }

        public static void OnEnable(WorkshopMenu __instance)
        {
            changed = false;
        }

        public static void MoveUp(WorkshopMenu __instance)
        {
            changed = true;
        }

        public static void MoveDown(WorkshopMenu __instance)
        {
            changed = true;
        }
    }
}