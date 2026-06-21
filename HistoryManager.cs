using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;

namespace PulseBrowser
{
    public class HistoryEntry
    {
        public string Title { get; set; } = "";
        public string Url { get; set; } = "";
        public DateTime Visited { get; set; } = DateTime.Now;
    }

    public class HistoryManager
    {
        List<HistoryEntry> entries = new List<HistoryEntry>();
        string filePath;
        const int MaxEntries = 1000;

        public HistoryManager()
        {
            filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "PulseBrowser", "history.json");
            Load();
        }

        public void Add(string title, string url)
        {
            entries.Add(new HistoryEntry
            {
                Title = string.IsNullOrEmpty(title) ? url : title,
                Url = url,
                Visited = DateTime.Now
            });

            if (entries.Count > MaxEntries)
                entries.RemoveAt(0);

            Save();
        }

        public List<HistoryEntry> GetAll() => entries.OrderByDescending(e => e.Visited).ToList();

        public List<HistoryEntry> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return GetAll();
            return entries
                .Where(e => e.Title.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0
                         || e.Url.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                .OrderByDescending(e => e.Visited)
                .ToList();
        }

        public void Clear()
        {
            entries.Clear();
            Save();
        }

        void Load()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var json = File.ReadAllText(filePath);
                    entries = new JavaScriptSerializer().Deserialize<List<HistoryEntry>>(json) ?? new List<HistoryEntry>();
                }
            }
            catch { entries = new List<HistoryEntry>(); }
        }

        void Save()
        {
            try
            {
                var dir = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                var json = new JavaScriptSerializer().Serialize(entries);
                File.WriteAllText(filePath, json);
            }
            catch { }
        }
    }
}
