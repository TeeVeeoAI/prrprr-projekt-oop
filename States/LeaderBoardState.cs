using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using prrprr_projekt_oop.Systems;
using prrprr_projekt_oop.Data;

namespace prrprr_projekt_oop.States
{
    public class LeaderBoardState : State
    {
        private List<LeaderBoardEntry> entries;
        private int maxShow = 15;

        public LeaderBoardState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content)
            : base(game, graphicsDevice, content)
        {
            entries = new List<LeaderBoardEntry>();
            BGcolor = Color.Black;
        }

        public override void LoadContent()
        {
            font = contentManager.Load<SpriteFont>("Fonts/MainFont");
            
            entries = LeaderBoardSystem.Load();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Return to menu
            if (InputSystem.IsKeyPressed(Keys.Back) && !starting)
            {
                game1.ChangeState(new MenuState(game1, graphicsDevice, contentManager));
            }

            // Clear leaderboard
            if (InputSystem.IsKeyPressed(Keys.C) && !starting)
            {
                entries = new List<LeaderBoardEntry>();
                LeaderBoardSystem.Save(entries.Select(e => e).ToList());
            }

            starting = false;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            int boxW = 800;
            int boxH = 600;
            int boxX = (int)Game1.ScreenSize.X / 2 - boxW / 2;
            int boxY = (int)Game1.ScreenSize.Y / 2 - boxH / 2;

            spriteBatch.Draw(pixel, new Rectangle(boxX, boxY, boxW, boxH), new Color(10, 10, 10, 220));

            string title = "Leader Board";
            spriteBatch.DrawString(font, title, new Vector2(Game1.ScreenSize.X / 2 - font.MeasureString(title).X / 2, boxY + 10), Color.Gold);

            string header = "#   Name               Score";
            spriteBatch.DrawString(font, header, new Vector2(boxX + 20, boxY + 50), Color.LightGray);

            int drawY = boxY + 50 + font.LineSpacing + 8;
            int count = Math.Min(entries.Count, maxShow);
            for (int i = 0; i < count; i++)
            {
                var e = entries[i];
                string name = e.Name ?? "---";
                if (name.Length > 18) name = name.Substring(0, 18) + "...";
                string line = $"{i + 1,2}. {name,-18} {e.Score,6}";
                spriteBatch.DrawString(font, line, new Vector2(boxX + 20, drawY + i * font.LineSpacing), Color.White);
            }

            string instr = "Back: Back    C: Clear leaderboard";
            spriteBatch.DrawString(font, instr, new Vector2(Game1.ScreenSize.X / 2 - font.MeasureString(instr).X / 2, boxY + boxH - font.LineSpacing - 10), Color.White);

            spriteBatch.End();
        }
    }
}