using System.Collections;

namespace S.AddonsOverhaul.Core.Interfaces.Module
{
    internal interface IModule
    {
        string LoadingCaption { get; }

        void Initialize();

        IEnumerator Load();

        void Shutdown();
    }
}