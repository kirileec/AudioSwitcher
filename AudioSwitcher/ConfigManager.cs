using System;
using System.IO;
using Newtonsoft.Json;
using AudioSwitcher.Models;

namespace AudioSwitcher
{
    public class ConfigManager
    {
        private static readonly string ConfigFileName = "config.json";
        private readonly string _configPath;
        private AppConfig _config;

        public AppConfig Config => _config;

        public ConfigManager()
        {
            _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFileName);
            Load();
        }

        public void Load()
        {
            if (File.Exists(_configPath))
            {
                try
                {
                    var json = File.ReadAllText(_configPath);
                    _config = JsonConvert.DeserializeObject<AppConfig>(json) ?? new AppConfig();
                }
                catch
                {
                    _config = new AppConfig();
                }
            }
            else
            {
                _config = new AppConfig();
            }
        }

        public void Save()
        {
            try
            {
                var json = JsonConvert.SerializeObject(_config, Formatting.Indented);
                File.WriteAllText(_configPath, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"保存配置失败: {ex.Message}");
            }
        }

        public void SetDevice1(string id, string name)
        {
            _config.Device1 = new DeviceConfig { Id = id, Name = name };
            Save();
        }

        public void SetDevice2(string id, string name)
        {
            _config.Device2 = new DeviceConfig { Id = id, Name = name };
            Save();
        }

        public void SetAutoStart(bool autoStart)
        {
            _config.AutoStart = autoStart;
            Save();
        }
    }
}
