using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace prrprr_projekt_oop.score
{
    public class Score
    {
        private int scoreValue;
        private string name;
        private bool pickedName;
        public int ScoreValue { get => scoreValue; }
        public string Name { get => name; }
        public bool PickedName { get => pickedName; }

        public Score()
        {
            this.scoreValue = 0;
            this.name = "";
            this.pickedName = false;
        }

        public void PickName(KeyboardState kstateNew, KeyboardState kstateOld) //Om jag vill ha leaderboard senare.
        {
            if (kstateNew.IsKeyDown(Keys.Enter) && kstateOld.IsKeyUp(Keys.Enter))
            {
                pickedName = true;
                return;
            }

            var newKeys = kstateNew.GetPressedKeys().Except(kstateOld.GetPressedKeys());

            foreach (var key in newKeys)
            {
                if (key == Keys.Back && name.Length > 0)
                {
                    name = name.Substring(0, name.Length - 1);
                }
                else if (key >= Keys.A && key <= Keys.Z)
                {
                    name += key.ToString();
                }
            }
        }
        public void IncreaseScore(int amount)
        {
            scoreValue += amount;
        }
        public void DecreaseScore(int amount)
        {
            scoreValue -= amount;
        }
    }
}