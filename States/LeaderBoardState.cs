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
        private int boxW = 800;
        private int boxH = 600;
        private Rectangle lederBoardBox;
        private string adminPassword = "011000010110010001101101011010010110111001110000011000010111001101110011"; //not very safe, but oh well
        private bool enteringPass = false;
        private string pass = "";

        public LeaderBoardState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content)
            : base(game, graphicsDevice, content)
        {
            entries = new List<LeaderBoardEntry>();
            BGcolor = Color.Black;
            lederBoardBox = new Rectangle((int)Game1.ScreenSize.X / 2 - boxW / 2, (int)Game1.ScreenSize.Y / 2 - boxH / 2, boxW, boxH);
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
            if (InputSystem.IsKeyPressed(Keys.Back) && !starting && !enteringPass)
            {
                game1.ChangeState(new MenuState(game1, graphicsDevice, contentManager));
            }

            // Clear leaderboard: start entering admin password
            if (InputSystem.IsKeyPressed(Keys.C) && !starting && !enteringPass)
            {
                enteringPass = true;
                pass = "";
                InputSystem.ClearTypedBuffer();
            }

            if (enteringPass)
            {
                // Collect typed characters (preserves case)
                char tc;
                while (InputSystem.TryGetTypedChar(out tc))
                {
                    if (char.IsControl(tc) && tc != ' ') continue;
                    pass += tc;
                    if (pass.Length > 64) pass = pass.Substring(0, 64);
                }

                // Backspace handling (TextInput doesn't provide backspace)
                if (InputSystem.IsKeyPressed(Keys.Back) && pass.Length > 0)
                {
                    pass = pass.Substring(0, pass.Length - 1);
                }

                // Submit password
                if (InputSystem.IsKeyPressed(Keys.Enter))
                {
                    if (passwordCheck(pass))
                    {
                        entries = new List<LeaderBoardEntry>();
                        LeaderBoardSystem.Save(entries.Select(e => e).ToList());
                    }
                    enteringPass = false;
                }
            }

            starting = false;
        }

        public bool passwordCheck(string password)
        {
            string aP = "";
            for (int i = 0; i < adminPassword.Length; i += 8)
            {
                string byteString = adminPassword.Substring(i, 8);
                char binaryChar = Convert.ToChar(Convert.ToByte(byteString, 2));
                aP += binaryChar;
            }
            return password == aP;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(pixel, lederBoardBox, new Color(10, 10, 10, 220));

            string title = "Leader Board";
            spriteBatch.DrawString(font, title, new Vector2(Game1.ScreenSize.X / 2 - font.MeasureString(title).X / 2, lederBoardBox.Y + 10), Color.Gold);

            string[] header = new string[] { "#", "Name", "Score", "Date" };
            Rectangle[] columns = new Rectangle[]
            {
                new Rectangle(lederBoardBox.X + 20, lederBoardBox.Y + 50, 40, font.LineSpacing),
                new Rectangle(lederBoardBox.X + 60, lederBoardBox.Y + 50, 240, font.LineSpacing),
                new Rectangle(lederBoardBox.X + 310, lederBoardBox.Y + 50, 120, font.LineSpacing),
                new Rectangle(lederBoardBox.X + 440, lederBoardBox.Y + 50, 320, font.LineSpacing)
            };

            for (int i = 0; i < header.Length; i++)
            {
                spriteBatch.DrawString(font, header[i], new Vector2(columns[i].X, columns[i].Y), Color.LightGray);
            }

            int drawY = lederBoardBox.Y + 50 + font.LineSpacing + 8;
            int count = Math.Min(entries.Count, maxShow);
            if (count == 0)
            {
                spriteBatch.DrawString(font, "No entries", new Vector2(lederBoardBox.X + 20, drawY), Color.White);
            }
            for (int i = 0; i < count; i++)
            {
                var e = entries[i];
                string name = e?.Name ?? "---";
                if (name.Length > 30) name = name.Substring(0, 30) + "...";
                string[] line = new string[] { $"{i + 1}.", name, e.Score.ToString(), e.Date.ToString("g") };
                for (int j = 0; j < line.Length; j++)
                {
                    spriteBatch.DrawString(font, line[j], new Vector2(columns[j].X, drawY + i * font.LineSpacing), Color.White);
                }
            }

            string instr = "Back: Back    C: Clear leaderboard";
            spriteBatch.DrawString(font, instr, new Vector2(Game1.ScreenSize.X / 2 - font.MeasureString(instr).X / 2, lederBoardBox.Y + boxH - font.LineSpacing - 10), Color.White);

            if (enteringPass)
            {
                DrawPasswordInput(spriteBatch);
            }

            spriteBatch.End();
        }

        public void DrawPasswordInput(SpriteBatch spriteBatch)
        {
            string prompt = "Enter admin password to clear leaderboard:";
            spriteBatch.DrawString(font, prompt, new Vector2(Game1.ScreenSize.X / 2 - font.MeasureString(prompt).X / 2, lederBoardBox.Y + boxH - font.LineSpacing - 50), Color.Red);
            spriteBatch.DrawString(font, pass, new Vector2(Game1.ScreenSize.X / 2 - font.MeasureString(pass).X / 2, lederBoardBox.Y + boxH - font.LineSpacing - 30), Color.Red);
        }
    }
}