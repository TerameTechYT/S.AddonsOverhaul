using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S.AddonsOverhaul.Converter
{
    public class Converter
    {
        public static string ConvertScript(string scriptToConv)
        {
            var str = scriptToConv.Replace("using Stationeers.Addons.API;", "using S.AddonsOverhaul.API;");
            var str2 = str.Replace("using Stationeers.Addons.Core;", "using S.AddonsOverhaul.Core;");
            var str3 = str2.Replace("using Stationeers.Addons;", "using S.AddonsOverhaul;");
            var str4 = str3.Replace("using Stationeers.Addons.API; ", "using S.AddonsOverhaul.API;");
            var str5 = str4.Replace("using Stationeers.Addons.Core; ", "using S.AddonsOverhaul.Core;");
            var str6 = str5.Replace("using Stationeers.Addons; ", "using S.AddonsOverhaul;");
            return str6;
        }
    }
}
