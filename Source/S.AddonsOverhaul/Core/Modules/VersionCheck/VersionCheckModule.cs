using System.Collections;
using Assets.Scripts.UI;
using S.AddonsOverhaul.Core.Interfaces.Log;
using S.AddonsOverhaul.Core.Interfaces.Module;
using UnityEngine.Networking;

namespace S.AddonsOverhaul.Core.Modules.VersionCheck
{
    internal class VersionCheckModule : IModule
    {
        public void Initialize()
        {
        }

        public IEnumerator Load()
        {
            AddonsLogger.Log("Checking for new S.AddonsOverhaul version...");

            using var webRequest =
                UnityWebRequest.Get(
                    Constants.VersionUrl);

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                var data = webRequest.downloadHandler.text.Trim();

                AddonsLogger.Log(
                    $"Latest S.AddonsOverhaul version is {data}. Installed {Constants.VersionNumber}");

                if (int.TryParse(data, out var val) && Constants.VersionNumber >= val)
                    yield break;

                AddonsLogger.Log("New version of S.AddonsOverhaul is available!");

                AddonsLogger.Log($"New version of S.AddonsOverhaul ({data}) is available!\n");

                AlertPanel.Instance.ShowAlert($"New version of S.AddonsOverhaul ({data}) is available!",
                    AlertState.Alert);
            }
            else
            {
                AddonsLogger.Log(
                    $"Failed to request latest S.AddonsOverhaul version. Result: {webRequest.result} Error: '\"{webRequest.error}\"",
                    LogLevel.Error
                );
                AddonsLogger.Log("Failed to check latest S.AddonsOverhaul version!\n", LogLevel.Error);

                while (AlertPanel.Instance.AlertWindow.activeInHierarchy)
                    yield return null;
            }

            webRequest.Dispose();
        }

        public void Shutdown()
        {
        }

        public string LoadingCaption => "Checking for a new version...";
    }
}