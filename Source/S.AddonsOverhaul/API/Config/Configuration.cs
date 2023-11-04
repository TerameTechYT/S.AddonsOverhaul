using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using Assets.Scripts.Serialization;
using S.AddonsOverhaul.Core;
using S.AddonsOverhaul.Core.Interfaces.Log;

namespace S.AddonsOverhaul.API.Config
{
    [XmlRoot]
    public class Configuration
    {
        [XmlElement]
        public string Name { get; private set; }

        [XmlElement]
        public string Description { get; private set; }

        [XmlElement("ConfigurationElements")]
        private List<ConfigurationElement> _configurationElements;
        public List<ConfigurationElement> ConfigurationElements
        {
            get { return _configurationElements; }
            set { _configurationElements = value; }
        }

        private FakeConfiguration _fakeSelf;

        public Configuration(string name, string description)
        {
            Name = name;
            Description = description;
            _configurationElements = new();

            _fakeSelf = new();
            _fakeSelf.Name = name;
            _fakeSelf.Description = description;
            _fakeSelf.ConfigurationElements = new();
        }

        public void AddConfigurationElement(ConfigurationElement element)
        {
            _configurationElements.Add(element);
            _fakeSelf.ConfigurationElements.Add(element.FakeSelf);
        }

        public void RemoveConfigurationElement(ConfigurationElement element)
        {
            _configurationElements.Remove(element);
            _fakeSelf.ConfigurationElements.Remove(element.FakeSelf);
        }

        public bool ContainsConfigurationElement(ConfigurationElement element)
        {
            return _configurationElements.Contains(element);
        }

        public ConfigurationElement GetConfigurationElement(string name)
        {
            return ConfigurationElements.Find(element => element.Name == name);
        }

        public void SaveConfiguration(Assembly pluginAssembly)
        {
            string path = pluginAssembly.Location;
            var f = path.Split("\\");
            var filename = f[f.Length - 1];
            var pluginname = filename.Split(" - ");
            if (!XmlSerialization.Serialize(_fakeSelf, Path.Combine(Constants.AddonPath, pluginname[0], pluginname[0] + " - AssemblyCSharp.dll.xml")))
                AddonsLogger.Log("Cant save configuration!", LogLevel.Error);
        }

        public FakeConfiguration GetConfiguration(Assembly pluginAssembly)
        {
            string path = pluginAssembly.Location;
            var f = path.Split("\\");
            var filename = f[f.Length - 1];
            var pluginname = filename.Split(" - ");

            var file = Path.Combine(Constants.AddonPath, pluginname[0], pluginname[0] + " - AssemblyCSharp.dll.xml");
            if (File.Exists(file))
                return XmlSerialization.Deserialize<FakeConfiguration>(file);
            return null;
        }
    }
}
