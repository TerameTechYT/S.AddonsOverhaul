namespace S.AddonsOverhaul.Core.Interfaces.Plugin
{
    internal readonly struct AddonPlugin
    {
        public readonly string AddonName;
        public readonly string AssemblyFile;

        public AddonPlugin(string addonName, string assemblyFile)
        {
            AddonName = addonName;
            AssemblyFile = assemblyFile;
        }
    }
}