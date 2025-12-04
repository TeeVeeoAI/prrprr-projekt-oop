using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using prrprr_projekt_oop.Entities;
using prrprr_projekt_oop.Systems;
using prrprr_projekt_oop.Entities.Projectiles;
using prrprr_projekt_oop.Data;

namespace prrprr_projekt_oop.States
{
    public class GameState : State
    {
        private ScoreSystem score;
        private Player player;
        private List<BaseEnemy> enemies;
        private List<XpPickup> xpPickups;
        private Texture2D xpTexture;
        private Texture2D healthBar;
        private Texture2D playerTexture;
        private SoundEffect invincibilitySound;
        private bool gameOver = false;
        private LeaderBoardEntry currentLeaderBoardEntry;
        private bool uploadedToLeaderBoard = false;

        public GameState(Game1 game1, GraphicsDevice graphicsDevice, ContentManager content)
            : base(game1, graphicsDevice, content)
        {
            score = new ScoreSystem();
            BGcolor = new Color(30, 25, 40);
            currentLeaderBoardEntry = null;
        }

        public override void LoadContent()
        {
            font = contentManager.Load<SpriteFont>("Fonts/MainFont");
            healthBar = contentManager.Load<Texture2D>("Images/HealthBarV3");
            playerTexture = contentManager.Load<Texture2D>("Images/Ship_4");
            xpTexture = contentManager.Load<Texture2D>("Images/XP");

            // Load invincibility sound if available (silently ignore if missing)
            try
            {
                invincibilitySound = contentManager.Load<SoundEffect>("Sounds/Invincible");
            }
            catch
            {
                // If asset missing, generate a short placeholder beep (16-bit PCM mono)
                int sampleRate = 22050;
                float duration = 0.15f; // seconds
                int sampleCount = (int)(sampleRate * duration);
                byte[] soundData = new byte[sampleCount * 2]; // 16-bit samples
                double freq = 880.0; // A5
                for (int i = 0; i < sampleCount; i++)
                {
                    short sample = (short)(Math.Sin(2.0 * Math.PI * freq * i / sampleRate) * short.MaxValue * 0.25);
                    soundData[i * 2] = (byte)(sample & 0xFF);
                    soundData[i * 2 + 1] = (byte)((sample >> 8) & 0xFF);
                }
                invincibilitySound = new SoundEffect(soundData, sampleRate, AudioChannels.Mono);
            }

            player = new Player(
                new Vector2(50, 50),
                playerTexture,
                pixel
            );
            enemies = new List<BaseEnemy>();
            xpPickups = new List<XpPickup>();
        }

        #region Update

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (gameOver)
            {
                currentLeaderBoardEntry = new LeaderBoardEntry(
                    score.Name.Length > 0 ? score.Name : "---",
                    score.Value,
                    DateTime.UtcNow,
                    player.Level
                );
                if (!uploadedToLeaderBoard)
                {
                    LeaderBoardSystem.AddEntry(currentLeaderBoardEntry);
                    uploadedToLeaderBoard = true;
                }
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
                    BaseEnemy e = enemies[i];
                    e.Update(gameTime);
                }
                
                BaseEnemy newEnemy = EnemySpawnerSystem.SpawnEnemy(pixel, pixel, player);
                if (newEnemy != null)
                {
                    enemies.Add(newEnemy);
                }

                CollisionCheck(gameTime);

                for (int i = 0; i < player.Projectiles.Count; i++)
                {
                    Projectile p = player.Projectiles[i];
                    p.Update(gameTime);
                    // check collision with enemies
                    for (int j = 0; j < enemies.Count; j++)
                    {
                        BaseEnemy e = enemies[j];
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

                Vector2 playerCenter = new Vector2(player.Hitbox.Center.X, player.Hitbox.Center.Y);

                for (int i = 0; i < xpPickups.Count; i++)
                {
                    XpPickup xp = xpPickups[i];
                    xp.Update(gameTime);
                    if (xp.IsExpired)
                    {
                        xpPickups.RemoveAt(i);
                        i--;
                        continue;
                    }

                    // If pickup is within pull radius, move it toward player
                    float dist = Vector2.Distance(xp.Collider.Pos, playerCenter);
                    if (dist <= player.PullRadius)
                    {
                        xp.Attract(playerCenter, player.PullSpeed, dt);
                    }

                    if (CollisionSystem.CheckPlayerPickupCollision(player, xp.Collider))
                    {
                        player.AddXP(xp.XpAmount);
                        score.IncreaseScore(xp.XpAmount / 4);
                        xpPickups.RemoveAt(i);
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

                    // Spawn an XP pickup at enemy center
                    int amount = 10;
                    if (e is ShooterEnemy) amount = 15;
                    else if (e is BuffEnemy) amount = 25;
                    Vector2 center = new Vector2(e.Hitbox.Center.X, e.Hitbox.Center.Y);
                    xpPickups.Add(new XpPickup(center, amount, 12f, xpTexture));
                }

                enemies.RemoveAt(index);
                index--;
            }
        }

        public void CollisionCheck(GameTime gameTime)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                BaseEnemy e = enemies[i];
                if (CollisionSystem.CheckPlayerEnemyCollision(player, e))
                {
                    if (!player.IsInvincible())
                    {
                        player.TakeDamage(e.Damage);
                        player.ApplyInvincibility();
                        invincibilitySound?.Play();
                    }
                }
                if (e is ShooterEnemy shooterEnemy)
                    {
                        // Check Enemy projectiles
                        for (int j = 0; j < shooterEnemy.Projectiles.Count; j++)
                        {
                            Projectile p = shooterEnemy.Projectiles[j];
                            p.Update(gameTime);
                            
                            // Check collision with player
                            if (p.Hitbox.Intersects(player.Hitbox))
                            {
                                player.TakeDamage(p.Damage);
                                player.ApplyInvincibility();
                                invincibilitySound?.Play();
                                p.Expire();
                            }
                            
                            if (p.IsExpired)
                            {
                                shooterEnemy.Projectiles.RemoveAt(j);
                                j--;
                            }
                        }
                    }
            }
        }

        #endregion

        #region Draw

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            DrawGame(gameTime, spriteBatch);

            spriteBatch.End();

            spriteBatch.Begin();

            DrawUi(spriteBatch);

            if (!score.PickedName)
                DrawPickName(spriteBatch);

            if (gameOver)
                DrawGameOver(spriteBatch);

            spriteBatch.End();
        }

        public void DrawGame(GameTime gameTime, SpriteBatch spriteBatch)
        {
            DrawXPickups(gameTime, spriteBatch);

            player.Draw(gameTime, spriteBatch);
            foreach (BaseEnemy e in enemies)
            {
                e.Draw(gameTime, spriteBatch);
                
                // Draw ShooterEnemy projectiles
                if (e is ShooterEnemy shooterEnemy)
                {
                    foreach (Projectile p in shooterEnemy.Projectiles)
                    {
                        p.Draw(gameTime, spriteBatch);
                    }
                }
            }
            // draw projectiles
            foreach (Projectile p in player.Projectiles)
            {
                p.Draw(gameTime, spriteBatch);
            }
        }

        public void DrawXPickups(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw XP pickups
            foreach (XpPickup xp in xpPickups)
            {
                xp.Draw(gameTime, spriteBatch);
            }
        }

        public void DrawUi(SpriteBatch spriteBatch)
        {
            DrawPlayerInfo(spriteBatch);
            DrawHealthBar(spriteBatch);
            DrawScore(spriteBatch);
            DrawXPickupsInfo(spriteBatch);
        }

        public void DrawXPickupsInfo(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(
                font,
                $"XP: {player.XP} / {player.XPToNextLevel}",
                new Vector2(10, 10 + font.LineSpacing),
                Color.LightGreen
            );
            spriteBatch.DrawString(
                font,
                $"Level: {player.Level}",
                new Vector2(10, 10 + font.LineSpacing * 2),
                Color.LightGreen
            );
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