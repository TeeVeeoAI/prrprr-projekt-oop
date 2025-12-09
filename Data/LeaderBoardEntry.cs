using System;
using prrprr_projekt_oop.Enums;

namespace prrprr_projekt_oop.Data
{
    public class LeaderBoardEntry
    {
        private string name;
        public string Name { get => name; }
        private int score;
        public int Score { get => score; }
        private DateTime date;
        public DateTime Date { get => date; }
        private int level;
        public int Level { get => level; }
        private Difficulty difficulty;
        public Difficulty Difficulty { get => difficulty; }
        public LeaderBoardEntry(string name, int score, DateTime date, int level, Difficulty difficulty) 
        { 
            this.name = name; 
            this.score = score; 
            this.date = date; 
            this.level = level;
            this.difficulty = difficulty;
        }
    }
}