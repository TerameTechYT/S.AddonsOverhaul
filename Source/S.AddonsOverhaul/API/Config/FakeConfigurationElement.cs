using System.Xml.Serialization;

namespace S.AddonsOverhaul.API.Config
{
    [XmlRoot]
    public class FakeConfigurationElement
    {
        [XmlElement]
        public string Name { get; set; }
        public string Description { get; set; }
        [XmlElement]
        public FakeConfigurationValue Value { get; set; }
    }
}
