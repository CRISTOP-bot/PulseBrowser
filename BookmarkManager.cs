using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;

namespace PulseBrowser
{
    public class Bookmark
    {
        public string Title { get; set; } = "";
        public string Url { get; set; } = "";
        public DateTime Added { get; set; } = DateTime.Now;
    }

    public class BookmarkManager
    {
        List<Bookmark> bookmarks = new List<Bookmark>();
        string filePath;

        public BookmarkManager()
        {
            filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "PulseBrowser", "bookmarks.json");
            Load();
        }

        public List<Bookmark> GetAll() => bookmarks.OrderBy(b => b.Title).ToList();

        public void Add(string title, string url)
        {
            if (Contains(url)) return;
            bookmarks.Add(new Bookmark
            {
                Title = string.IsNullOrEmpty(title) ? url : title,
                Url = url,
                Added = DateTime.Now
            });
            Save();
        }

        public void Remove(string url)
        {
            bookmarks.RemoveAll(b => b.Url == url);
            Save();
        }

        public void Remove(Bookmark bm)
        {
            bookmarks.Remove(bm);
            Save();
        }

        public bool Contains(string url)
        {
            return bookmarks.Any(b => b.Url == url);
        }

        public void Update(Bookmark old, string newTitle, string newUrl)
        {
            var existing = bookmarks.FirstOrDefault(b => b.Url == old.Url);
            if (existing != null)
            {
                existing.Title = newTitle;
                existing.Url = newUrl;
            }
            Save();
        }

        public void Load()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var json = File.ReadAllText(filePath);
                    bookmarks = new JavaScriptSerializer().Deserialize<List<Bookmark>>(json) ?? new List<Bookmark>();
                }
            }
            catch
            {
                bookmarks = new List<Bookmark>();
            }
        }

        public void Save()
        {
            try
            {
                var dir = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                var json = new JavaScriptSerializer().Serialize(bookmarks);
                File.WriteAllText(filePath, json);
            }
            catch { }
        }
    }
}
