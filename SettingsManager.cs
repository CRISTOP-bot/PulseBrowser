using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;

namespace PulseBrowser
{
    public class SettingsManager
    {
        Dictionary<string, string> settings = new Dictionary<string, string>();
        string filePath;

        public SettingsManager()
        {
            filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "PulseBrowser", "settings.json");
            Load();
        }

        public string Get(string key, string defaultValue = "")
        {
            return settings.TryGetValue(key, out var val) ? val : defaultValue;
        }

        public void Set(string key, string value)
        {
            settings[key] = value;
            Save();
        }

        public Dictionary<string, string> GetAll() => new Dictionary<string, string>(settings);

        void Load()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var json = File.ReadAllText(filePath);
                    settings = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
                }
            }
            catch { settings = new Dictionary<string, string>(); }
        }

        void Save()
        {
            try
            {
                var dir = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                var json = new JavaScriptSerializer().Serialize(settings);
                File.WriteAllText(filePath, json);
            }
            catch { }
        }
    }
}
