using System.Xml.Serialization;

namespace S.AddonsOverhaul.API.Config
{
    [XmlRoot]
    public class ConfigurationElement
    {
        [XmlElement]
        public string Name { get; private set; }
        [XmlElement]
        public string Description { get; private set; }
        [XmlElement]
        public ConfigurationValue Value { get; private set; }

        public Configuration Parent { get; private set; }

        public FakeConfigurationElement FakeSelf { get; private set; }

        public ConfigurationElement(string name, string description, ConfigurationValue value, Configuration parent)
        {
            Name = name;
            Description = description;
            Value = value;
            Parent = parent;

            FakeConfigurationValue fakeValue = new();
            fakeValue.Type = value.Type;
            fakeValue.Value = value.Value;

            FakeSelf = new();
            FakeSelf.Name = name;
            FakeSelf.Description = description;
            FakeSelf.Value = fakeValue;
        }

       
        // <summary>
        /// Don't forget to save after you use this function!!!
        /// <summary/>

        public void SetValue(ConfigurationValue value)
        {
            Value = value;
            FakeSelf.Value.Value = value.Value;
        }
    }
}
