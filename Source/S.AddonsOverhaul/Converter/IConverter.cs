using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S.AddonsOverhaul.Converter
{
    public interface IConverter
    {
        string ConvertScript(string script);
    }
}
