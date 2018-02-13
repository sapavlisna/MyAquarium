namespace Aquarium
{
    public interface IConfigManager
    {
        Config GetConfig();
        Config LoadConfig();
        void SaveConfig(IConfig config);
        string SerializeConfig(IConfig config);
    }
}