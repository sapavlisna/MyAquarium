namespace Aquarium
{
    public interface IConfig
    {
        ConfigLIghts LightConfig { get; set; }
        bool LogInfo { get; set; }
    }
}