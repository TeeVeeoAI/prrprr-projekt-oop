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
        private string adminPassword = "011000010110010001101101011010010110111001110000011000010111001101110011";
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

            // Clear leaderboard
            if (InputSystem.IsKeyPressed(Keys.C) && !starting && !enteringPass)
            {
                enteringPass = true;
                pass = "";
            }

            if (enteringPass){
                List<Keys> newKeys = InputSystem.GetPressedKeys().Except(InputSystem.OldState.GetPressedKeys()).ToList();

                foreach (Keys key in newKeys)
                {
                    if (key == Keys.Back && pass.Length > 0)
                    {
                        pass = pass.Substring(0, pass.Length - 1);
                    }
                    else if (key >= Keys.A && key <= Keys.Z)
                    {
                        pass += key.ToString().ToLower();
                    }
                }
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

            string[] header = ["#", "Name:", "Score:", "Level:"];
            Rectangle[] columns = new Rectangle[]
            {
                new Rectangle(lederBoardBox.X + 20, lederBoardBox.Y + 50, 20, font.LineSpacing),
                new Rectangle(lederBoardBox.X + 40, lederBoardBox.Y + 50, 180, font.LineSpacing),
                new Rectangle(lederBoardBox.X + 220, lederBoardBox.Y + 50, 140, font.LineSpacing),
                new Rectangle(lederBoardBox.X + 360, lederBoardBox.Y + 50, 100, font.LineSpacing)
            };
            for (int i = 0; i < header.Length; i++){
                spriteBatch.DrawString(font, header[i], new Vector2((float)columns[i].X, (float)columns[i].Y), Color.LightGray);
            }

            int drawY = lederBoardBox.Y + 50 + font.LineSpacing + 8;
            int count = Math.Min(entries.Count, maxShow);
            for (int i = 0; i < count; i++)
            {
                var e = entries[i];
                string name = e.Name;
                if (name.Length > 18) name = name.Substring(0, 18) + "...";
                string[] line = [$"{i+ 1}.", $"{name}", $"{e.Score}", $"{e.Level}"];
                for (int j = 0; j < line.Length; j++){
                    spriteBatch.DrawString(font, line[j], new Vector2((float)columns[j].X, (float)(drawY + i * font.LineSpacing)), Color.White);
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