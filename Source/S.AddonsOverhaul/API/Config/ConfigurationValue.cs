using System.Xml.Serialization;
using UnityEngine;

namespace S.AddonsOverhaul.API.Config
{
    [XmlRoot]
    public class ConfigurationValue
    {
        [XmlElement]
        public ConfigurationType Type { get; private set; }

        [XmlElement]
        public string Value { get; private set; }

        private int IntValue;
        private float FloatValue;
        private double DoubleValue;
        private string StringValue;
        private bool BoolValue;
        private Color ColorValue;

        public ConfigurationValue(int value)
        {
            IntValue = value;
            Value = value.ToString();
            Type = ConfigurationType.Integer;
        }

        public ConfigurationValue(float value)
        {
            FloatValue = value;
            Value = value.ToString();
            Type = ConfigurationType.Float;
        }

        public ConfigurationValue(double value)
        {
            DoubleValue = value;
            Value = value.ToString();
            Type = ConfigurationType.Double;
        }

        public ConfigurationValue(string value)
        {
            StringValue = value;
            Value = value.ToString();
            Type = ConfigurationType.String;
        }

        public ConfigurationValue(bool value)
        {
            BoolValue = value;
            Value = value.ToString();
            Type = ConfigurationType.Boolean;
        }

        public ConfigurationValue(Color value)
        {
            ColorValue = value;
            Value = value.ToString();
            Type = ConfigurationType.Color;
        }

        public void SetIntValue(int intValue)
        {
            IntValue = intValue;
            Value = intValue.ToString();
        }

        public void SetFloatValue(float floatValue)
        {
            FloatValue = floatValue;
            Value = floatValue.ToString();
        }

        public void SetDoubleValue(double doubleValue)
        {
            DoubleValue = doubleValue;
            Value = doubleValue.ToString();
        }

        public void SetStringValue(string stringValue)
        {
            StringValue = stringValue;
            Value = stringValue.ToString();
        }

        public void SetBoolValue(bool stringValue)
        {
            BoolValue = stringValue;
            Value = stringValue.ToString();
        }

        public void SetColorValue(Color colorValue)
        {
            ColorValue = colorValue;
            Value = colorValue.ToString();
        }

        public int GetIntValue()
        {
            return IntValue;
        }

        public float GetFloatValue()
        {
            return FloatValue;
        }

        public double GetDoubleValue()
        {
            return DoubleValue;
        }

        public string GetStringValue()
        {
            return StringValue;
        }

        public bool GetBoolValue()
        {
            return BoolValue;
        }

        public Color GetColorValue()
        {
            return ColorValue;
        }

        public string GetValueAsString()
        {
            return Value;
        }
    }

    public enum ConfigurationType
    {
        Integer,
        Float,
        Double,
        String,
        Boolean,
        Color,
    }
}
