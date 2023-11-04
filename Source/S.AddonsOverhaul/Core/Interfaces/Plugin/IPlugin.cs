namespace S.AddonsOverhaul.Core.Interfaces.Plugin
{
    public interface IPlugin
    {
        void OnLoad();

        void OnUnload();
    }
}