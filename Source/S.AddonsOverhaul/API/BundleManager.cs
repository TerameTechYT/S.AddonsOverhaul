using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace S.AddonsOverhaul.API
{
    public static class BundleManager
    {
        internal static readonly List<AssetBundle> LoadedAssetBundles = new();

        public static AssetBundle GetAssetBundle(string name)
        {
            return LoadedAssetBundles.FirstOrDefault(x =>
                x.name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}