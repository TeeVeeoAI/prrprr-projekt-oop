using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using prrprr_projekt_oop.Entities;
using prrprr_projekt_oop.Enums;

namespace prrprr_projekt_oop.Systems
{
    public static class EnemySpawnerSystem
    {
        private static Random rand = new Random();
        private static Stopwatch spawnTimer = new Stopwatch();
        public static int spawnInterval = 2000; // in milliseconds
        public static float ShooterEnemyWeight = 0.3f;
        public static float BuffEnemyWeight = 0.1f;
        public static Difficulty difficulty = Difficulty.Medium;
        private static float[] difficultyMultiplier = new float[] 
        { 
            0.8f, // Easy
            1.0f, // Medium
            1.2f  // Hard
        };


        public static Vector2 PickSpawnPos()
        {
            int w = (int)Game1.ScreenSize.X;
            int h = (int)Game1.ScreenSize.Y;
            int off = 100; // how far off-screen to spawn

            int side = rand.Next(4); // 0=top,1=bottom,2=left,3=right
            int x = 0, y = 0;

            switch (side)
            {
                case 0: // top
                    x = rand.Next(-off, w + off);
                    y = -off;
                    break;
                case 1: // bottom
                    x = rand.Next(-off, w + off);
                    y = h + off;
                    break;
                case 2: // left
                    x = -off;
                    y = rand.Next(-off, h + off);
                    break;
                default: // right
                    x = w + off;
                    y = rand.Next(-off, h + off);
                    break;
            }

            return new Vector2(x, y);
        }

        public static BaseEnemy SpawnEnemy(Texture2D texture, Texture2D projectileTexture = null, Entities.Player player = null)
        {
            if (!spawnTimer.IsRunning)
            {
                spawnTimer.Start();
            }
            if (spawnTimer.ElapsedMilliseconds > spawnInterval)
            {
                spawnTimer.Restart();

                Dictionary<string, float> candidates = new Dictionary<string, float>();

                // ClassicEnemy base weight
                candidates["classic"] = 1f;

                // ShooterEnemy only if are is a projectile texture
                if (projectileTexture != null)
                    candidates["shooter"] = ShooterEnemyWeight;

                // BuffEnemy
                candidates["buff"] = BuffEnemyWeight;

                // Sum weights
                float total = candidates.Values.Sum();
                if (total <= 0f)
                {
                    spawnInterval -= 10;
                    return new ClassicEnemy(texture, player, difficultyMultiplier[(int)difficulty]);
                }

                double r = rand.NextDouble() * total;
                float cumulative = 0f;
                string chosen = "classic";
                foreach (KeyValuePair<string, float> kv in candidates)
                {
                    cumulative += kv.Value;
                    if (r <= cumulative)
                    {
                        chosen = kv.Key;
                        break;
                    }
                }

                switch (chosen)
                {
                    case "shooter":
                        spawnInterval -= 10;
                        return new ShooterEnemy(texture, projectileTexture, player, difficultyMultiplier[(int)difficulty]);
                    case "buff":
                        spawnInterval -= 10;
                        return new BuffEnemy(texture, player, difficultyMultiplier[(int)difficulty]);
                    default:
                        spawnInterval -= 10;
                        return new ClassicEnemy(texture, player, difficultyMultiplier[(int)difficulty]);
                }
            }
            return null;
        }
    }
}