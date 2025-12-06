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
            // Confirm with Enter
            if (InputSystem.IsKeyPressed(Keys.Enter) && name.Length > 0)
            {
                pickedName = true;
                onScreenName = name.Length > 10 ? name.Substring(0, 10) + "..." : name;
                InputSystem.ClearTypedBuffer();
                return;
            }

            // Backspace handling
            if (InputSystem.IsKeyPressed(Keys.Back) && name.Length > 0)
            {
                name = name.Substring(0, name.Length - 1);
            }

            // Get typed characters
            char c;
            while (InputSystem.TryGetTypedChar(out c))
            {
                // Ignore control characters except allow space
                if (char.IsControl(c) && c != ' ') continue;

                // Allow letters, digits, punctuation and space
                if (char.IsLetterOrDigit(c) || char.IsPunctuation(c) || char.IsWhiteSpace(c))
                {
                    name += c;
                    if (name.Length > 20) // limit length
                        name = name.Substring(0, 20);
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