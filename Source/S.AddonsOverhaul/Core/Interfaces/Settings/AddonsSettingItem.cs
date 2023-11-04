namespace S.AddonsOverhaul.Core.Interfaces.Settings
{
    internal class AddonsSettingItem
    {
        public AddonsSettingItem(string name, bool value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; private set; } = string.Empty;
        public bool Value { get; set; }
        public bool DidChange { get; set; } = false;
    }
}