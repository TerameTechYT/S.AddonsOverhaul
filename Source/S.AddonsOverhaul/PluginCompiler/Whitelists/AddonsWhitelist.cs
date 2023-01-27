// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

using S.AddonsOverhaul.API;

namespace S.AddonsOverhaul.PluginCompiler.Whitelists
{
    internal sealed class AddonsWhitelist : IWhitelistRegistry
    {
        public void Register(PluginWhitelist whitelist)
        {
            whitelist.WhitelistTypes(
                typeof(IPlugin),
                typeof(Constants)
            );

            whitelist.WhitelistTypesNamespaces(
                typeof(BundleManager)
            );
            
            whitelist.BlacklistTypes();
        }
    }
}