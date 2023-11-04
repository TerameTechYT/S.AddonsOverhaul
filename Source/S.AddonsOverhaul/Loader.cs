using JetBrains.Annotations;
using S.AddonsOverhaul.Core;

namespace S.AddonsOverhaul
{
    public class Loader
    {
        [UsedImplicitly]
        public void Load()
        {
            LoaderManager.Instance.Activate();
        }
    }
}