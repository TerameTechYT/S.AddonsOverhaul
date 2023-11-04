using System.Reflection;

namespace S.AddonsOverhaul.Core.Interfaces.Plugin
{
    internal struct PluginInfo
    {
        public Assembly Assembly { get; set; }
        public IPlugin[] Plugins { get; set; }
    }
}