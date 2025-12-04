using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using prrprr_projekt_oop.Systems;

namespace prrprr_projekt_oop.States
{
    public class MenuState : State
    {
        private List<string> menuItems;
        private int selectedIndex;
        public MenuState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content)
            : base(game, graphicsDevice, content)
        {
            menuItems = new List<string>()
            {
                "Start Game",
                "Leader Board",
                "Exit"
            };
            selectedIndex = 0;
            BGcolor = Color.Black;
        }

        public override void LoadContent()
        {
            font = contentManager.Load<SpriteFont>("Fonts/MainFont");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (InputSystem.IsKeyPressed(Keys.Down))
                selectedIndex = (selectedIndex + 1) % menuItems.Count;
            if (InputSystem.IsKeyPressed(Keys.Up))
                selectedIndex = (selectedIndex - 1 + menuItems.Count) % menuItems.Count;

            if (InputSystem.IsKeyPressed(Keys.Enter) && !starting)
            {
                if (menuItems[selectedIndex] == "Start Game")
                    game1.ChangeState(new GameState(game1, graphicsDevice, contentManager));
                else if (menuItems[selectedIndex] == "Leader Board")
                    game1.ChangeState(new LeaderBoardState(game1, graphicsDevice, contentManager));
                else if (menuItems[selectedIndex] == "Exit")
                    game1.Exit();
            }

            starting = false;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(pixel,
                new Rectangle(
                    (int)Game1.ScreenSize.X / 2 - 100,
                    (int)Game1.ScreenSize.Y / 2 - 100,
                    200,
                    200
                ),
                Color.Black
            );
            spriteBatch.DrawString(
                font,
                "Menu:",
                new Vector2(
                    Game1.ScreenSize.X / 2 - font.MeasureString("Menu:").X / 2,
                    Game1.ScreenSize.Y / 2 - 90
                ),
                Color.White
            );
            for (int i = 0; i < menuItems.Count; i++)
            {
                Color color = (i == selectedIndex) ? Color.Green : Color.White;
                spriteBatch.Draw(
                    pixel,
                    new Rectangle(
                        (int)(Game1.ScreenSize.X / 2 - font.MeasureString(menuItems[i]).X / 2 - 10),
                        (int)Game1.ScreenSize.Y / 2 - 90 + font.LineSpacing * 2 + i * font.LineSpacing * 2 - 10,
                        (int)font.MeasureString(menuItems[i]).X + 20,
                        (int)font.LineSpacing * 2
                    ),
                    new Color(color.R / 2, color.G / 2, color.B / 2, (byte)20)
                );
                spriteBatch.DrawString(
                    font,
                    menuItems[i],
                    new Vector2(
                        Game1.ScreenSize.X / 2 - font.MeasureString(menuItems[i]).X / 2,
                        Game1.ScreenSize.Y / 2 - 90 + font.LineSpacing * 2 + i * font.LineSpacing * 2
                    ),
                    Color.WhiteSmoke
                );
            }

            spriteBatch.End();
        }
    }
}