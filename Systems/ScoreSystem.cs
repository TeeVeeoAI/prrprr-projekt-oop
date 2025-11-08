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
        private bool pickedName;
        public int Value { get => value; }
        public string Name { get => name; }
        public bool PickedName { get => pickedName; }

        public ScoreSystem()
        {
            this.value = 0;
            this.name = "";
            this.pickedName = false;
        }

        public void PickName() //Om jag vill ha leaderboard senare.
        {
            if (InputSystem.IsKeyPressed(Keys.Enter) && name.Length > 0)
            {
                pickedName = true;
                return;
            }

            var newKeys = InputSystem.GetPressedKeys().Except(InputSystem.OldState.GetPressedKeys());

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
            value += amount;
        }
        public void DecreaseScore(int amount)
        {
            value -= amount;
        }
    }
}