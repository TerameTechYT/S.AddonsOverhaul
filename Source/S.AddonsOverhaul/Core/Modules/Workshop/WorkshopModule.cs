using System;
using System.Collections;
using Assets.Scripts.UI;
using CSharpUtilities.Core;
using HarmonyLib;
using S.AddonsOverhaul.Core.Interfaces.Log;
using S.AddonsOverhaul.Core.Interfaces.Module;
using S.AddonsOverhaul.Core.Modules.HarmonyLib;

namespace S.AddonsOverhaul.Core.Modules.Workshop
{
    internal class WorkshopModule : IModule
    {
        public string LoadingCaption => "Loading Workshop...";

        public void Initialize()
        {
            HarmonyModule.RegisterPatcher(harmony =>
            {
                AddonsLogger.Log("Patching WorkshopManager using Harmony...");
                try
                {
                    harmony.Patch(ReflectionHelper.GetPrivateMethod(typeof(WorkshopMenu), "RefreshButtons"), null,
                        new HarmonyMethod(ReflectionHelper.GetMethod(typeof(WorkshopManagerPatch),
                            nameof(WorkshopManagerPatch.RefreshButtons))));
                    harmony.Patch(ReflectionHelper.GetPrivateMethod(typeof(WorkshopMenu), "MoveUp"), null,
                        new HarmonyMethod(ReflectionHelper.GetMethod(typeof(WorkshopManagerPatch),
                            nameof(WorkshopManagerPatch.MoveUp))));
                    harmony.Patch(ReflectionHelper.GetPrivateMethod(typeof(WorkshopMenu), "MoveDown"), null,
                        new HarmonyMethod(ReflectionHelper.GetMethod(typeof(WorkshopManagerPatch),
                            nameof(WorkshopManagerPatch.MoveDown))));
                    harmony.Patch(ReflectionHelper.GetPrivateMethod(typeof(WorkshopMenu), "OnEnable"), null,
                        new HarmonyMethod(ReflectionHelper.GetMethod(typeof(WorkshopManagerPatch),
                            nameof(WorkshopManagerPatch.OnEnable))));
                    harmony.Patch(ReflectionHelper.GetPrivateMethod(typeof(WorkshopMenu), "OnDisable"), null,
                        new HarmonyMethod(ReflectionHelper.GetMethod(typeof(WorkshopManagerPatch),
                            nameof(WorkshopManagerPatch.OnDisable))));
                }
                catch (Exception ex)
                {
                    AlertPanel.Instance.ShowAlert("Failed to initialize workshop patch!\n", AlertState.Alert);
                    AddonsLogger.Log($"Failed to initialize workshop patch. Exception:\n{ex}", LogLevel.Error);
                }
            });
        }

        public IEnumerator Load()
        {
            yield return null;
            AddonsLogger.Log("Workshop loaded!");
        }

        public void Shutdown()
        {
        }
    }
}