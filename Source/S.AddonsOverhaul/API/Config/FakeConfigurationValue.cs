using System.Xml.Serialization;

namespace S.AddonsOverhaul.API.Config
{
    [XmlRoot]
    public class FakeConfigurationValue
    {
        [XmlElement]
        public ConfigurationType Type { get; set; }

        [XmlElement]
        public string Value { get; set; }
    }
}
