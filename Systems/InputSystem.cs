using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace prrprr_projekt_oop.Systems
{
    public static class InputSystem
    {
        private static KeyboardState kstateNew = Keyboard.GetState();
        private static KeyboardState kstateOld = kstateNew;

        public static void Initialize()
        {
            kstateNew = Keyboard.GetState();
            kstateOld = kstateNew;
        }

        public static void Update()
        {
            kstateOld = kstateNew;
            kstateNew = Keyboard.GetState();
        }

        public static KeyboardState NewState => kstateNew;
        public static KeyboardState OldState => kstateOld;

        public static bool IsKeyPressed(Keys key)
        {
            return kstateNew.IsKeyDown(key) && kstateOld.IsKeyUp(key);
        }

        public static bool IsKeyReleased(Keys key)
        {
            return kstateNew.IsKeyUp(key) && kstateOld.IsKeyDown(key);
        }

        public static bool IsKeyDown(Keys key)
        {
            return kstateNew.IsKeyDown(key);
        }

        public static bool IsKeyUp(Keys key)
        {
            return kstateNew.IsKeyUp(key);
        }

        public static Keys[] GetPressedKeys()
        {
            return kstateNew.GetPressedKeys();
        }
    }
}