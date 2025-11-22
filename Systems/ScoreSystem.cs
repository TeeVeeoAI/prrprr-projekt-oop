using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace prrprr_projekt_oop.Systems
{
    public class ScoreSystem
    {
        private int value;
        private string name;
        private string onScreenName;
        private bool pickedName;
        public int Value { get => value; }
        public string Name { get => name; }
        public string OnScreenName { get => onScreenName; }
        public bool PickedName { get => pickedName; }

        public ScoreSystem()
        {
            this.value = 0;
            this.name = "";
            this.onScreenName = "";
            this.pickedName = false;
        }

        public void PickName() //If i want a leaderboard later
        {
            if (InputSystem.IsKeyPressed(Keys.Enter) && name.Length > 0)
            {
                pickedName = true;
                onScreenName = name.Length > 10 ? $"{name[0]}{name[1]}{name[2]}{name[3]}{name[4]}{name[5]}{name[6]}{name[7]}{name[8]}{name[9]}...": name;
                return;
            }

            List<Keys> newKeys = InputSystem.GetPressedKeys().Except(InputSystem.OldState.GetPressedKeys()).ToList();

            foreach (Keys key in newKeys)
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
            value += amount;
        }
        public void DecreaseScore(int amount)
        {
            value -= amount;
        }
    }
}