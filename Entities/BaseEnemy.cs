using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using prrprr_projekt_oop.Systems;

namespace prrprr_projekt_oop.Entities
{
    public class BaseEnemy : BaseEntity
    {
        protected bool damageByPlayer = false;
        public bool DamageByPlayer { get => damageByPlayer; }
        public BaseEnemy(Texture2D texture)
        : base(EnemySpawnerSystem.PickSpawnPos(), new Vector2(5, 5), new Vector2(50, 50), texture, 3, Color.Violet /* Is Purple because Player can be Red at 1 HP */)
        {
        }

        public override void Update(GameTime gameTime)
        {
            position.Y += velocity.Y;

            OutOfBoundsCheck();

            hitbox.Location = position.ToPoint();
        }

        public void OutOfBoundsCheck()
        {
            if (position.Y > Game1.ScreenSize.Y)
            {
                hp = 0; // Mark for removal
            }
        }

        public void TakeDamage(int damage, bool byPlayer = false)
        {
            hp -= damage;
            if (byPlayer)
            {
                damageByPlayer = true;
            }
        }

        public void Kill(bool byPlayer = false)
        {
            if (byPlayer)
            {
                damageByPlayer = true;
            }
            hp = 0;
        }

        public bool IsDead()
        {
            return hp <= 0; 
        }

    }
}