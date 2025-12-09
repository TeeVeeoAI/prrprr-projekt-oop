using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using prrprr_projekt_oop.Enums;
using prrprr_projekt_oop.Systems;

namespace prrprr_projekt_oop.Entities
{
    public class ClassicEnemy : BaseEnemy
    {
        private Player targetPlayer;

        public ClassicEnemy(Texture2D texture, Player player = null, float difficultyMultiplier = 1,  int damage = 1)
            : base(EnemySpawnerSystem.PickSpawnPos(), new Vector2(50, 50), texture, Color.Violet, (int)(damage* difficultyMultiplier), 3)
        {
            this.targetPlayer = player;
            this.damage = (int)(this.damage *difficultyMultiplier);
            this.speed *= difficultyMultiplier;
            this.hp = (int)(this.hp * difficultyMultiplier);

        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (targetPlayer != null)
            {
                Vector2 playerCenter = targetPlayer.Position + new Vector2(targetPlayer.Hitbox.Width / 2f, targetPlayer.Hitbox.Height / 2f);
                Vector2 selfCenter = position + new Vector2(hitbox.Width / 2f, hitbox.Height / 2f);
                Vector2 dir = playerCenter - selfCenter;
                if (dir.LengthSquared() > 0.0001f)
                {
                    dir.Normalize();
                    position += dir * speed * deltaTime;
                }
            }
            else
            {
                position.Y += speed * deltaTime;
            }

            hitbox.Location = position.ToPoint();
        }
        
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 playerCenter = targetPlayer.Position + new Vector2(targetPlayer.Hitbox.Width / 2f, targetPlayer.Hitbox.Height / 2f);
            Vector2 selfCenter = position + new Vector2(hitbox.Width / 2f, hitbox.Height / 2f);
            Vector2 dir = playerCenter - selfCenter;
            spriteBatch.Draw(
                texture, 
                position + new Vector2(hitbox.Width / 2, hitbox.Height / 2), 
                null, 
                color, 
                (float)Math.Atan2(dir.Y, dir.X) + (float)Math.PI / 2f, 
                new Vector2(texture.Width / 2f, texture.Height / 2f), 
                hitbox.Size.ToVector2() / (texture.Bounds.Size.ToVector2()/2),
                SpriteEffects.None, 0f
            );
        }
    }
}