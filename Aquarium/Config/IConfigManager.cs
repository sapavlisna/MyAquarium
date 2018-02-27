using System;

namespace Aquarium
{
    public interface IConfigManager
    {
        event ConfigManager.ConfigChangedHandler ConfigChanged;

        Config GetConfig();
        Config LoadConfig();
        void SaveConfig(IConfig config);
        string SerializeConfig(IConfig config);
    }
}