using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using prrprr_projekt_oop.Data;
using prrprr_projekt_oop.Enums;

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

            // Format: { "difficultys": [ { "difficulty": "Easy", "entries": [ ... ] }, ... ] }
            var file = JsonSerializer.Deserialize<LeaderBoardFile>(json, opts);
            var list = new List<LeaderBoardEntry>();
            if (file?.difficultys != null)
            {
                foreach (var d in file.difficultys)
                {
                    if (d.entries == null) continue;
                    list.AddRange(d.entries);
                }
            }

            list = list.OrderByDescending(e => e.Score).ThenBy(e => e.Date).ToList();
            return list ?? new List<LeaderBoardEntry>();
        }

        public static void Save(List<LeaderBoardEntry> entries)
        {
            var dir = Path.GetDirectoryName(fileName);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            // Group entries by difficulty into the { "difficultys": [ ... ] } format
            var grouped = Enum.GetValues(typeof(Difficulty)).Cast<Difficulty>()
                .Select(d => new LeaderBoardDiff { difficulty = d, entries = new List<LeaderBoardEntry>() })
                .ToList();

            foreach (var e in entries.OrderByDescending(e => e.Score).ThenBy(e => e.Date).Take(maxEntries))
            {
                var bucket = grouped.FirstOrDefault(g => g.difficulty == e.Difficulty);
                if (bucket != null)
                    bucket.entries.Add(e);
            }

            var file = new LeaderBoardFile { difficultys = grouped };

            JsonSerializerOptions opts = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(file, opts);
            File.WriteAllText(fileName, json);
        }

        public static void Add(string name, int score, int level, Difficulty difficulty)
        {
            if (string.IsNullOrWhiteSpace(name)) name = "---";
            List<LeaderBoardEntry> entries = Load();
            entries.Add(new LeaderBoardEntry(name, score, DateTime.UtcNow, level, difficulty));
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

    // File model for new JSON format: { "diff": [ { "difficulty": "Easy", "entries": [ ... ] }, ... ] }
    public class LeaderBoardFile
    {
        public List<LeaderBoardDiff> difficultys { get; set; }
    }

    public class LeaderBoardDiff
    {
        public Difficulty difficulty { get; set; }
        public List<LeaderBoardEntry> entries { get; set; }
    }
}