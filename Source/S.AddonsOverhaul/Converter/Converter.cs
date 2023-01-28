namespace S.AddonsOverhaul
{
    internal static class Converter
    {
        public static string ToOverhaulFomat(string script)
        {
            var str = script.Replace("using Stationeers.Addons.API;", "using S.AddonsOverhaul.API;");
            var str2 = str.Replace("using Stationeers.Addons.Core;", "using S.AddonsOverhaul.Core;");
            var str3 = str2.Replace("using Stationeers.Addons;", "using S.AddonsOverhaul;");
            return str3;
        }

        public static string FromOverhaulFomat(string script)
        {
            var str = script.Replace("using S.AddonsOverhaul.API;", "using Stationeers.Addons.API;");
            var str2 = str.Replace("using S.AddonsOverhaul.Core;", "using Stationeers.Addons.Core;");
            var str3 = str2.Replace("using S.AddonsOverhaul;", "using Stationeers.Addons;");
            return str3;
        }
    }
}
