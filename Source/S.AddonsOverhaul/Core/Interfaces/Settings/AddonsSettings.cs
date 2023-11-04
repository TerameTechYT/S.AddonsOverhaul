using System.Xml.Serialization;

namespace S.AddonsOverhaul.Core.Interfaces.Settings
{
    [XmlRoot]
    public class AddonsSettings
    {
        [XmlElement] public bool ConsoleEnabled { get; set; }

        [XmlElement] public bool LiveReloadEnabled { get; set; }

        [XmlElement] public bool FirstRun { get; set; } = true;
    }
}