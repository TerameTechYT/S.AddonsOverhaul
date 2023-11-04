namespace S.AddonsOverhaul.Core.Compilation
{
    internal static class Converter
    {
        public static string ToOverhaulFormat(string script)
        {
            var newScript = script.Replace("Stationeers.Addons", "S.AddonsOverhaul");

            if (!newScript.Contains("using S.AddonsOverhaul.Core.Interfaces.Plugin;"))
                newScript = newScript.Insert(0, "using S.AddonsOverhaul.Core.Interfaces.Plugin;");
            return newScript;
        }

        public static string FromOverhaulFormat(string script)
        {
            return script.Replace("S.AddonsOverhaul", "Stationeers.Addons")
                .Replace("using S.AddonsOverhaul.Core.Interfaces.Plugin;", "");
        }
    }
}