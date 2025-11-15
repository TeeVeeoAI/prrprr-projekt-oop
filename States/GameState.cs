using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using prrprr_projekt_oop.Entities;
using prrprr_projekt_oop.Systems;

namespace prrprr_projekt_oop.States
{
    public class GameState : State
    {
        private ScoreSystem score;
        private Player player;
        private List<BaseEnemy> enemies;
        private Texture2D healthBar;
        private bool gameOver = false;

        public GameState(Game1 game1, GraphicsDevice graphicsDevice, ContentManager content)
            : base(game1, graphicsDevice, content)
        {
            score = new ScoreSystem();
            BGcolor = new Color(30, 25, 40);
            player = new Player(
                new Vector2(50, 50),
                pixel,
                pixel
            );
            enemies = new List<BaseEnemy>();
        }

        public override void LoadContent()
        {
            font = contentManager.Load<SpriteFont>("Fonts/MainFont");
            healthBar = contentManager.Load<Texture2D>("Images/HealthBarV3");
            
        }

        #region Update

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (gameOver)
            {
                if (InputSystem.IsKeyPressed(Keys.Enter) && !starting)
                {
                    game1.ChangeState(new MenuState(game1, graphicsDevice, contentManager));
                }
                return;
            }

            if (!score.PickedName && !starting)
            {
                score.PickName();
            }
            else
            {
                player.Update(gameTime);
                for (int i = 0; i < enemies.Count; i++)
                {
                    var e = enemies[i];
                    e.Update(gameTime);
                }
                CollisionCheck();
                
                var newEnemy = EnemySpawnerSystem.SpawnEnemy(pixel, player);
                if (newEnemy != null)
                {
                    enemies.Add(newEnemy);
                }

                

                for (int i = 0; i < player.Projectiles.Count; i++)
                {
                    var p = player.Projectiles[i];
                    p.Update(gameTime);
                    // check collision with enemies
                    for (int j = 0; j < enemies.Count; j++)
                    {
                        var e = enemies[j];
                        if (p.Owner == e) continue; // don't hit owner
                        if (p.Hitbox.Intersects(e.Hitbox))
                        {
                            e.TakeDamage(p.Damage, true);
                            p.Expire();
                            ReamoveDeadEnemy(e, ref j);
                        }
                    }

                    if (p.IsExpired)
                    {
                        player.Projectiles.RemoveAt(i);
                        i--;
                    }
                }

                if (player.IsDead())
                {
                    gameOver = true;
                }

            }

            starting = false;
        }

        public void ReamoveDeadEnemy(BaseEnemy e, ref int index)
        {
            if (e.IsDead())
            {
                if (e.DamageByPlayer)
                {
                    score.IncreaseScore(100);
                }
                enemies.RemoveAt(index);
                index--;
            }
        }

        public void CollisionCheck()
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                var e = enemies[i];
                if (CollisionSystem.CheckPlayerEnemyCollision(player, e))
                {
                    player.TakeDamage(1);
                    e.Kill(true);
                    ReamoveDeadEnemy(e, ref i);
                }
            }
        }

        #endregion

        #region Draw

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            DrawGame(gameTime, spriteBatch);

            DrawUi(spriteBatch);

            if (!score.PickedName)
                DrawPickName(spriteBatch);

            if (gameOver)
                DrawGameOver(spriteBatch);

            spriteBatch.End();
        }

        public void DrawGame(GameTime gameTime, SpriteBatch spriteBatch)
        {
            player.Draw(gameTime, spriteBatch);
            foreach (BaseEnemy e in enemies)
            {
                e.Draw(gameTime, spriteBatch);
            }
            // draw projectiles
            foreach (var p in player.Projectiles)
            {
                p.Draw(gameTime, spriteBatch);
            }
        }

        public void DrawUi(SpriteBatch spriteBatch)
        {
            DrawPlayerInfo(spriteBatch);
            DrawHealthBar(spriteBatch);
            DrawScore(spriteBatch);
        }

        public void DrawPlayerInfo(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(
                font,
                $"{score.OnScreenName} | ",
                new Vector2(10, 10),
                Color.White
            );
        }

        public void DrawScore(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(
                font,
                $"{score.Value}p",
                new Vector2(10 + font.MeasureString($"{score.OnScreenName} | ").X, 10),
                Color.White
            );
        }

        public void DrawHealthBar(SpriteBatch spriteBatch)
        {
            int barHeight = healthBar.Height / 6;
            int barWidth = healthBar.Width;
            int sourceY = (5 - (player.HP >= 0 ? player.HP : 0)) * barHeight; // Each bar is 50 pixels tall

            spriteBatch.Draw(
                healthBar,
                new Rectangle((int)(Game1.ScreenSize.X - barWidth), (int)(Game1.ScreenSize.Y - barHeight), barWidth, barHeight),
                new Rectangle(0, sourceY, barWidth, barHeight),
                Color.White
            );
        }

        public void DrawGameOver(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(
                font,
                "Game Over!",
                new Vector2(
                    Game1.ScreenSize.X / 2 - font.MeasureString("Game Over!").X / 2,
                    Game1.ScreenSize.Y / 2 - font.LineSpacing
                ),
                Color.White
            );
            spriteBatch.DrawString(
                font,
                "Press ENTER to return to Menu",
                new Vector2(
                    Game1.ScreenSize.X / 2 - font.MeasureString("Press ENTER to return to Menu").X / 2,
                    Game1.ScreenSize.Y / 2 + font.LineSpacing
                ),
                Color.White
            );
        }

        public void DrawPickName(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                pixel,
                new Rectangle(0, 0, (int)Game1.ScreenSize.X, (int)Game1.ScreenSize.Y),
                new Color(10, 10, 10)
            );

            string displayText = score.Name.Length > 0 ? score.Name : "_";
            spriteBatch.DrawString(
                font,
                "Enter your name:",
                new Vector2(
                    Game1.ScreenSize.X / 2 - font.MeasureString("Enter your name:").X / 2,
                    Game1.ScreenSize.Y / 2 - font.LineSpacing * 3
                ),
                Color.White
            );
            spriteBatch.DrawString(
                font,
                displayText,
                new Vector2(
                    Game1.ScreenSize.X / 2 - font.MeasureString(displayText).X / 2,
                    Game1.ScreenSize.Y / 2 - font.LineSpacing
                ),
                Color.White
            );
            spriteBatch.DrawString(
                font,
                "ENTER to confirm",
                new Vector2(
                    Game1.ScreenSize.X / 2 - font.MeasureString("ENTER to confirm").X / 2,
                    Game1.ScreenSize.Y / 2 - font.LineSpacing * -2
                ),
                Color.White
            );
        }
        #endregion
    }
}