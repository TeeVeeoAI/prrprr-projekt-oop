using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace prrprr_projekt_oop.States
{
    public class MenuState : State
    {
        private List<string> _menuItems;
        private int _selectedIndex;
        public MenuState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content)
            : base(game, graphicsDevice, content)
        {
            _menuItems = new List<string>()
            {
                "Start Game",
                "Exit"
            };
            _selectedIndex = 0;
        }

        public override void LoadContent()
        {
            font = contentManager.Load<SpriteFont>("Fonts/MainFont");
        }

        public override void Update(GameTime gameTime)
        {
            kstateNew = Keyboard.GetState();

            if (kstateNew.IsKeyDown(Keys.Down) && kstateOld.IsKeyUp(Keys.Down))
                _selectedIndex = (_selectedIndex + 1) % _menuItems.Count;
            if (kstateNew.IsKeyDown(Keys.Up) && kstateOld.IsKeyUp(Keys.Up))
                _selectedIndex = (_selectedIndex - 1 + _menuItems.Count) % _menuItems.Count;

            if (kstateNew.IsKeyDown(Keys.Enter) && kstateOld.IsKeyUp(Keys.Enter) && !starting)
            {
                if (_menuItems[_selectedIndex] == "Start Game")
                    game1.ChangeState(new GameState(game1, graphicsDevice, contentManager));
                else if (_menuItems[_selectedIndex] == "Exit")
                    game1.Exit();
            }

            kstateOld = kstateNew;
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
            for (int i = 0; i < _menuItems.Count; i++)
            {
                var color = (i == _selectedIndex) ? Color.Green : Color.White;
                spriteBatch.Draw(
                    pixel,
                    new Rectangle(
                        (int)(Game1.ScreenSize.X / 2 - font.MeasureString(_menuItems[i]).X / 2 - 10),
                        (int)Game1.ScreenSize.Y / 2 - 90 + font.LineSpacing * 2 + i * font.LineSpacing * 2 - 10,
                        (int)font.MeasureString(_menuItems[i]).X + 20,
                        (int)font.LineSpacing * 2
                    ),
                    new Color(color.R / 2, color.G / 2, color.B / 2, (byte)20)
                );
                spriteBatch.DrawString(
                    font,
                    _menuItems[i],
                    new Vector2(
                        Game1.ScreenSize.X / 2 - font.MeasureString(_menuItems[i]).X / 2,
                        Game1.ScreenSize.Y / 2 - 90 + font.LineSpacing * 2 + i * font.LineSpacing * 2
                    ),
                    Color.WhiteSmoke
                );
            }

            spriteBatch.End();
        }
    }
}