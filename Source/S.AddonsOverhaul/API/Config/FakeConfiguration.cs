using System.Collections.Generic;
using System.Xml.Serialization;

namespace S.AddonsOverhaul.API.Config
{
    [XmlRoot]
    public class FakeConfiguration
    {
        [XmlElement]
        public string Name { get; set; }

        public string Description { get; set; }

        public List<FakeConfigurationElement> ConfigurationElements { get; set; }
    }
}
