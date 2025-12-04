using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using prrprr_projekt_oop.Data;

namespace prrprr_projekt_oop.Systems
{
    public static class LeaderBoardSystem
    {
        private static readonly string fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "leaderboard.json");
        private static readonly int maxEntries = 50;

        public static List<LeaderBoardEntry> Load()
        {
            if (!File.Exists(fileName))
                return new List<LeaderBoardEntry>();

            string json = File.ReadAllText(fileName);
            JsonSerializerOptions opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            List<LeaderBoardEntry> list = JsonSerializer.Deserialize<List<LeaderBoardEntry>>(json, opts);
            return list ?? new List<LeaderBoardEntry>();
        }

        public static void Save(List<LeaderBoardEntry> entries)
        {
            var dir = Path.GetDirectoryName(fileName);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            JsonSerializerOptions opts = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(entries.Take(maxEntries).ToList(), opts);
            File.WriteAllText(fileName, json);
        }

        public static void Add(string name, int score, int level)
        {
            if (string.IsNullOrWhiteSpace(name)) name = "---";
            List<LeaderBoardEntry> entries = Load();
            entries.Add(new LeaderBoardEntry(name, score, DateTime.UtcNow, level));
            entries = entries.OrderByDescending(e => e.Score).ThenBy(e => e.Date).ToList();
            Save(entries);
        }

        public static void AddEntry(LeaderBoardEntry entry)
        {
            List<LeaderBoardEntry> entries = Load();
            entries.Add(entry);
            entries = entries.OrderByDescending(e => e.Score).ThenBy(e => e.Date).ToList();
            Save(entries);
        }
    }
}