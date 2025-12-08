using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using prrprr_projekt_oop.Systems;

namespace prrprr_projekt_oop.Entities
{
    public class BuffEnemy : ClassicEnemy
    {
        public BuffEnemy(Texture2D texture, Player player = null, float difficultyMultiplier = 1, float speed = 180f)
            : base(texture, player, difficultyMultiplier, 2)
        {
            this.speed = speed * difficultyMultiplier;
            hp = 6 * (int)difficultyMultiplier;
            color = Color.Gold;
        }
    }
}
