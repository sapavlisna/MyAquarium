using System;
using Aquarium;
using Aquarium.Config.Model;

namespace Config.Model.Config
{
    public interface IConfigManager
    {
        event ConfigManager.ConfigChangedHandler ConfigChanged;

        Aquarium.Config.Model.Config GetConfig();
        Aquarium.Config.Model.Config LoadConfig();
        void SaveConfig(Aquarium.Config.Model.Config config);
        string SerializeConfig(Aquarium.Config.Model.Config config);
    }
}