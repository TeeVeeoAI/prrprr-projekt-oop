using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using prrprr_projekt_oop.Systems;

namespace prrprr_projekt_oop.Entities
{
    public abstract class BaseEnemy : BaseEntity
    {
        protected bool damageByPlayer = false;
        protected int damage;
        public int Damage { get => damage; }

        public bool DamageByPlayer { get => damageByPlayer; }

        protected BaseEnemy(Vector2 position, Vector2 size, Texture2D texture, Color color, int damage, int hp = 3, float speed = 200f)
            : base(position, speed, size, texture, hp, color)
        {
            this.damage = damage;
        }

        public virtual void TakeDamage(int damage, bool byPlayer = false)
        {
            hp -= damage;
            if (byPlayer)
            {
                damageByPlayer = true;
            }
        }

        public virtual void Kill(bool byPlayer = false)
        {
            if (byPlayer)
            {
                damageByPlayer = true;
            }
            hp = 0;
        }

        public virtual bool IsDead()
        {
            return hp <= 0;
        }

    }
}