using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using prrprr_projekt_oop.Entities;

namespace prrprr_projekt_oop.Systems
{
    public static class EnemySpawnerSystem
    {
        public static Stopwatch spawnTimer = new Stopwatch();
        public static int spawnInterval = 2000; // in milliseconds
        private static Random rand = new Random();
        public static float ShooterEnemyWeight = 0.3f;
        public static float BuffEnemyWeight = 0.1f;

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

                // Build dictionary of candidate enemy types with their weights
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
                    return new ClassicEnemy(texture, player);
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
                        return new ShooterEnemy(texture, projectileTexture, player);
                    case "buff":
                        return new BuffEnemy(texture, player);
                    default:
                        return new ClassicEnemy(texture, player);
                }
            }
            return null;
        }
    }
}