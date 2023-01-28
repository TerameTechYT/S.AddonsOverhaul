using System.Collections.Generic;
using System.Xml.Serialization;

namespace S.AddonsOverhaul.Mod
{
    [XmlRoot("ModMetadata")]
    public class ModInfo
    {

        [XmlIgnore]
        public bool IsValid = true;
        public const string ROOT_NAME = "ModMetadata";
        [XmlElement]
        public string Name;
        [XmlElement]
        public string Author;
        [XmlElement]
        public string Version;
        [XmlElement]
        public string Description;
        [XmlElement]
        public string InGameDescription;
        [XmlElement]
        public ulong WorkshopHandle;
        [XmlArray("Tags")]
        [XmlArrayItem("Tag")]
        public List<string> Tags;
    }
}
