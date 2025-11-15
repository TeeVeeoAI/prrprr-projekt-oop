using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using prrprr_projekt_oop.Systems;

namespace prrprr_projekt_oop.Entities
{
    public class BuffEnemy : ClassicEnemy
    {
        public BuffEnemy(Texture2D texture, Player player = null, float speed = 180f)
            : base(texture, player, 2)
        {
            this.speed = speed;
            hp = 6;
            color = Color.Gold;
        }
    }
}
