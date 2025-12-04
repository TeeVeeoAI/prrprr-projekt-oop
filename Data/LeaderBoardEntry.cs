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
        public LeaderBoardEntry(string name, int score, DateTime date) 
        { 
            this.name = name; 
            this.score = score; 
            this.date = date; 
        }
    }
}