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

        public GameState(Game1 game1, GraphicsDevice graphicsDevice, ContentManager content)
            : base(game1, graphicsDevice, content)
        {
            score = new ScoreSystem();
            BGcolor = new Color(30, 25, 40);
            player = new Player(
                new Vector2(50, 50),
                pixel
            );
        }

        public override void LoadContent()
        {
            font = contentManager.Load<SpriteFont>("Fonts/MainFont");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!score.PickedName && !starting)
            {
                score.PickName();
            }
            else
            {
                player.Update(gameTime);
            }

            starting = false;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            DrawGame(gameTime, spriteBatch);

            DrawUi(spriteBatch);

            DrawPickName(spriteBatch);
            spriteBatch.End();
        }
        
        public void DrawGame(GameTime gameTime, SpriteBatch spriteBatch)
        {
            player.Draw(gameTime, spriteBatch);
        }

        public void DrawUi(SpriteBatch spriteBatch)
        {
            DrawScore(spriteBatch);
        }

        public void DrawScore(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(
                font,
                "Score: " + score.Value,
                new Vector2(10, 10),
                Color.White
            );
        }

        public void DrawPickName(SpriteBatch spriteBatch)
        {
            if (!score.PickedName)
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
                    "Space to confirm",
                    new Vector2(
                        Game1.ScreenSize.X / 2 - font.MeasureString("Space to confirm").X / 2,
                        Game1.ScreenSize.Y / 2 - font.LineSpacing * -2
                    ),
                    Color.White
                );
            }
        }
    }
}