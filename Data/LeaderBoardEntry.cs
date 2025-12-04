using System;

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
        public LeaderBoardEntry(string name, int score, DateTime date, int level) 
        { 
            this.name = name; 
            this.score = score; 
            this.date = date; 
            this.level = level;
        }
    }
}