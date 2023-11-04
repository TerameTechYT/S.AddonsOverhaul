using System;

namespace S.AddonsOverhaul.Patcher.Core
{
    internal interface IGamePatcher : IDisposable
    {
        void Load(string gameExe);

        bool IsPatched();

        void Patch();
    }
}