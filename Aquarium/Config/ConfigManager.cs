﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Aquarium.Config.Model;
using Aquarium.Log;
using Config.Model.Config;
using Newtonsoft.Json;

namespace Aquarium
{
    public class ConfigManager : IConfigManager
    {
        public delegate void ConfigChangedHandler(object sender, EventArgs e);
        public event ConfigChangedHandler ConfigChanged;

        private const string configFileName = "config.json";
        private ILogger logger;
        private Config.Model.Config _config;

        public ConfigManager(ILogger logger)
        {
            this.logger = logger;
            _config = LoadConfig();
            SetConfigFileWatcher();
        }

        public Config.Model.Config GetConfig()
        {
            return _config;
        }

        private void SetConfigFileWatcher()
        {
            var watcher = new FileSystemWatcher();
            watcher.Path = GetConfigDirectory();

            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = configFileName;
            watcher.Changed += Watcher_Changed;

            watcher.EnableRaisingEvents = true;

        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            _config = LoadConfig();
            OnConfigChanged();
        }

        public void OnConfigChanged()
        {
            ConfigChanged(this, EventArgs.Empty);
        }

        private string LoadConfigFile()
        {
            logger.Write("Searching config",LoggerTypes.LogLevel.System);

            if (!File.Exists(GetConfigFullPath()))
            {
                logger.Write("Creating default config", LoggerTypes.LogLevel.Info);
                var defaultConfig = GetDefaultConfig();
                logger.Write("Saving default config", LoggerTypes.LogLevel.Info);
                SaveConfig(defaultConfig);
            }
            else
            {
                logger.Write($"Found config {this.GetConfigFullPath()}", LoggerTypes.LogLevel.System);
            }

            return File.ReadAllText(GetConfigFullPath());
        }

        private Config.Model.Config GetDefaultConfig()
        {
            logger.Write($"Fill config by default values", LoggerTypes.LogLevel.Info);
            var config = new Config.Model.Config
            {
                LogInfo = true,
                LightConfig = new ConfigLIghts
                {
                    FullTurningOnMinutes = 5,
                    LightMaxValue = 255,
                    LightMinValue = 0,
                    LightPinNumber = 9,
                    UseSlowTurnOn = true,
                    TurnedOn = true,
                    LightStates = new List<LightState>
                    {
                        new LightState
                        {
                            DayOfWeek = DayOfWeek.Sunday,
                            Time = DateTime.Now.AddSeconds(10).TimeOfDay,
                            LightIntensity = 50
                        },
                        new LightState
                        {
                            DayOfWeek = DayOfWeek.Sunday,
                            Time = DateTime.Now.AddMinutes(5).TimeOfDay,
                            LightIntensity = 50
                        },
                        new LightState
                        {
                            DayOfWeek = DayOfWeek.Sunday,
                            Time = DateTime.Now.AddMinutes(10).AddSeconds(15).TimeOfDay,
                            LightIntensity = 0
                        }
                    }
                },
                Temperature = new Temperature
                {
                    Interval = 1,
                    Pin = 4,
                    TurnedOn = true
                },
                LightIntensity = new LightIntensity
                {
                    TurnedOn = true,
                    Interval = 1,
                    Pins = new List<int> { 4 }
                },
                Surface = new Surface 
                {
                    TurnedOn = true,
                    Samples = 20,
                    TriggerPin = 12,
                    EchoPin = 11,
                    Interval = 1
                }
            };

            logger.Write($"Config created: {config}", LoggerTypes.LogLevel.Info);
            return config;
        }

        private void SaveConfigFile(string fileContent)
        {
            logger.Write($"Save config to path {GetConfigFullPath()}", LoggerTypes.LogLevel.Info);
            logger.Write($"Save config to path {fileContent}", LoggerTypes.LogLevel.Info);


            File.WriteAllText(GetConfigFullPath(), fileContent);
            //using (StreamWriter streamWriter = new StreamWriter(GetConfigFullPath()))
            //{
            //    Ilogger.Write($"Save config {fileContent}", ILoggerTypes.LogLevel.Info);
            //    streamWriter.Write(fileContent);

            //}

            logger.Write($"Config saved", LoggerTypes.LogLevel.Info);
        }

        public Config.Model.Config LoadConfig()
        {
            Config.Model.Config config = new Config.Model.Config();
            try
            {
                var loadedConfig = LoadConfigFile();

                logger.Write(loadedConfig, LoggerTypes.LogLevel.System);
                config = JsonConvert.DeserializeObject<Config.Model.Config>(loadedConfig);
            }
            catch (Exception e)
            {
                logger.Write(e);
            }

            return config;
        }

        public void SaveConfig(Config.Model.Config config)
        {
            logger.Write($"Serialize config", LoggerTypes.LogLevel.Info);

            var serialized = SerializeConfig(config);
            logger.Write($"Save serialized config", LoggerTypes.LogLevel.Info);
            SaveConfigFile(serialized);

        }

        public string SerializeConfig(Config.Model.Config config)
        {
            logger.Write("Going to serialize config", LoggerTypes.LogLevel.Info);
            var serialized = JsonConvert.SerializeObject(config, Formatting.Indented);
            return serialized;
        }

        private string GetConfigFullPath()
        {
            var configFilePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/" + configFileName;
            logger.Write($"ConfigFile path: {configFilePath}", LoggerTypes.LogLevel.Info);
            return configFilePath;
        }

        private string GetConfigDirectory()
        {
            return Directory.GetCurrentDirectory();
        }
    }
}
