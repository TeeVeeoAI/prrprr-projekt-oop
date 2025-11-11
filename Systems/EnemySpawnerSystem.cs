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
        public static Rectangle SpawnArea = new Rectangle(0, -100, (int)Game1.ScreenSize.X-100, 100);
        public static Vector2 PickSpawnPos()
        {
            Random rand = new Random();
            int x = rand.Next(SpawnArea.Left, SpawnArea.Right);
            int y = SpawnArea.Top;
            return new Vector2(x, y);
        }

        public static BaseEnemy SpawnEnemy(Texture2D texture)
        {
            if (!spawnTimer.IsRunning)
            {
                spawnTimer.Start();
            }
            if (spawnTimer.ElapsedMilliseconds > spawnInterval)
            {
                spawnTimer.Restart();
                return new BaseEnemy(texture);
            }
            return null;
        }
    }
}