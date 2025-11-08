using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;

namespace prprpr_projekt_oop.Systems
{
    public static class SoundSystem
    {
        private static Dictionary<string, SoundEffect> sounds = new();

        public static void Load(string name, SoundEffect effect)
        {
            sounds[name] = effect;
        }   

        public static void Play(string name)
        {
            if (sounds.ContainsKey(name))
            {
                sounds[name].Play();
            }
        }
    }
}
