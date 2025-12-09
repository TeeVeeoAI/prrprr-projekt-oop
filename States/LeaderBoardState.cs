using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using prrprr_projekt_oop.Systems;
using prrprr_projekt_oop.Data;
using prrprr_projekt_oop.Enums;

namespace prrprr_projekt_oop.States
{
    public class LeaderBoardState : State
    {
        private enum ClearMode
        {
            None,
            EnteringIndex,
            EnteringPasswordForAll,
            EnteringPasswordForSingle
        }

        private ClearMode mode = ClearMode.None;
        private List<LeaderBoardEntry> entries;
        private int maxShow = 15;
        private int boxW = 800;
        private int boxH = 600;
        private Rectangle lederBoardBox;
        private Rectangle difficultyButtonRect;

        private string adminPassword =
            "011000010110010001101101011010010110111001110000011000010111001101110011";

        private string pass = "";
        private string clearIndex = "";
        private bool ignoreNextEnter = false;
        private Difficulty difficulty = Difficulty.Easy;

        public LeaderBoardState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content)
            : base(game, graphicsDevice, content)
        {
            entries = new List<LeaderBoardEntry>();
            BGcolor = Color.Black;

            lederBoardBox = new Rectangle(
                (int)Game1.ScreenSize.X / 2 - boxW / 2,
                (int)Game1.ScreenSize.Y / 2 - boxH / 2,
                boxW,
                boxH
            );
        }

        public override void LoadContent()
        {
            font = contentManager.Load<SpriteFont>("Fonts/MainFont");
            entries = LeaderBoardSystem.Load()
                .Where(e => e.Difficulty == difficulty)
                .ToList();

            difficultyButtonRect = new Rectangle(
                lederBoardBox.X + boxW - 150,
                lederBoardBox.Y + 10,
                130,
                font.LineSpacing + 8
            );
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (InputSystem.IsKeyPressed(Keys.Back) && !starting && mode == ClearMode.None)
            {
                game1.ChangeState(new MenuState(game1, graphicsDevice, contentManager));
            }

            if (mode == ClearMode.None && !starting)
            {
                if (InputSystem.IsKeyPressed(Keys.C))
                {
                    mode = ClearMode.EnteringPasswordForAll;
                    pass = "";
                    InputSystem.ClearTypedBuffer();
                }

                if (InputSystem.IsKeyPressed(Keys.X))
                {
                    mode = ClearMode.EnteringIndex;
                    clearIndex = "";
                    InputSystem.ClearTypedBuffer();
                }
                // cycle difficulty
                if (InputSystem.IsKeyPressed(Keys.D))
                {
                    CycleDifficulty();
                }
                // mouse click cycle
                if (InputSystem.IsLeftPressed())
                {
                    var mp = InputSystem.GetMousePosition();
                    if (difficultyButtonRect.Contains((int)mp.X, (int)mp.Y))
                    {
                        CycleDifficulty();
                    }
                }
            }

            if (mode == ClearMode.EnteringIndex)
            {
                char tc;

                while (InputSystem.TryGetTypedChar(out tc))
                {
                    if (char.IsControl(tc)) continue;
                    clearIndex += tc;
                    if (clearIndex.Length > 3)
                        clearIndex = clearIndex.Substring(0, 3);
                }

                if (InputSystem.IsKeyPressed(Keys.Back) && clearIndex.Length > 0)
                {
                    clearIndex = clearIndex.Substring(0, clearIndex.Length - 1);
                }

                if (InputSystem.IsKeyPressed(Keys.Enter))
                {
                    mode = ClearMode.EnteringPasswordForSingle;
                    pass = "";
                    InputSystem.ClearTypedBuffer();
                    ignoreNextEnter = true;
                }
            }

            if (mode == ClearMode.EnteringPasswordForAll ||
                mode == ClearMode.EnteringPasswordForSingle)
            {
                if (ignoreNextEnter)
                {
                    if (InputSystem.IsKeyPressed(Keys.Enter))
                        return;        // skip this frame

                    ignoreNextEnter = false;
                }

                char tc;

                while (InputSystem.TryGetTypedChar(out tc))
                {
                    if (char.IsControl(tc)) continue;
                    pass += tc;

                    if (pass.Length > 64)
                        pass = pass.Substring(0, 64);
                }

                if (InputSystem.IsKeyPressed(Keys.Back) && pass.Length > 0)
                {
                    pass = pass.Substring(0, pass.Length - 1);
                }

                if (InputSystem.IsKeyPressed(Keys.Enter))
                {
                    if (pass == DecodeBinaryPassword(adminPassword))
                    {
                        if (mode == ClearMode.EnteringPasswordForAll)
                        {
                            // Clear the entire leaderboard across all difficulties
                            LeaderBoardSystem.Save(new List<LeaderBoardEntry>());
                            entries = new List<LeaderBoardEntry>();
                        }
                        else if (mode == ClearMode.EnteringPasswordForSingle)
                        {
                            if (int.TryParse(clearIndex, out int idx))
                            {
                                idx -= 1; 
                                if (idx >= 0 && idx < entries.Count)
                                {
                                    // Remove the corresponding entry from the full list so other difficulties are preserved
                                    var toRemove = entries[idx];
                                    var all = LeaderBoardSystem.Load();
                                    var match = all.FirstOrDefault(e =>
                                        e.Name == toRemove.Name &&
                                        e.Score == toRemove.Score &&
                                        e.Level == toRemove.Level &&
                                        e.Date == toRemove.Date &&
                                        e.Difficulty == toRemove.Difficulty);

                                    if (match != null)
                                    {
                                        all.Remove(match);
                                        LeaderBoardSystem.Save(all);
                                    }

                                    // Reload filtered entries for the current difficultytjxv
                                    entries = LeaderBoardSystem.Load().Where(e => e.Difficulty == difficulty).ToList();
                                }
                            }
                        }
                    }

                    mode = ClearMode.None;
                    pass = "";
                    clearIndex = "";
                }
            }

            starting = false;
        }

        private void CycleDifficulty()
        {
            var vals = Enum.GetValues(typeof(Difficulty)).Cast<Difficulty>().ToArray();
            int idx = Array.IndexOf(vals, difficulty);
            idx = (idx + 1) % vals.Length;
            difficulty = vals[idx];
            entries = LeaderBoardSystem.Load().Where(e => e.Difficulty == difficulty).ToList();
        }

        private string DecodeBinaryPassword(string bin)
        {
            string result = "";

            for (int i = 0; i < bin.Length; i += 8)
            {
                string byteString = bin.Substring(i, 8);
                char c = Convert.ToChar(Convert.ToByte(byteString, 2));
                result += c;
            }

            return result;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(pixel, lederBoardBox, new Color(10, 10, 10, 220));

            string title = $"Leader Board | {difficulty}";
            spriteBatch.DrawString(
                font,
                title,
                new Vector2(Game1.ScreenSize.X / 2 - font.MeasureString(title).X / 2,
                lederBoardBox.Y + 10),
                Color.Gold
            );

            DrawDifficultyButton(spriteBatch);

            string[] header = { "#", "Name", "Score", "Level", "Date" };
            Rectangle[] columns =
            {
                new Rectangle(lederBoardBox.X + 20, lederBoardBox.Y + 50, 40, font.LineSpacing),
                new Rectangle(lederBoardBox.X + 60, lederBoardBox.Y + 50, 240, font.LineSpacing),
                new Rectangle(lederBoardBox.X + 310, lederBoardBox.Y + 50, 120, font.LineSpacing),
                new Rectangle(lederBoardBox.X + 440, lederBoardBox.Y + 50, 100, font.LineSpacing),
                new Rectangle(lederBoardBox.X + 540, lederBoardBox.Y + 50, 320, font.LineSpacing)
            };

            for (int i = 0; i < header.Length; i++)
                spriteBatch.DrawString(font, header[i], new Vector2(columns[i].X, columns[i].Y), Color.LightGray);

            int drawY = lederBoardBox.Y + 50 + font.LineSpacing + 8;
            int count = Math.Min(entries.Count, maxShow);

            if (count == 0)
            {
                spriteBatch.DrawString(font,
                    "No entries",
                    new Vector2(lederBoardBox.X + 20, drawY),
                    Color.White
                );
            }

            for (int i = 0; i < count; i++)
            {
                var e = entries[i];

                string name = e?.Name ?? "---";
                if (name.Length > 30)
                    name = name.Substring(0, 30) + "...";

                string[] line =
                {
                    $"{i + 1}.",
                    name,
                    e.Score.ToString(),
                    e.Level.ToString(),
                    $"{e.Date.ToShortTimeString()} {e.Date.ToShortDateString()}"
                };

                for (int j = 0; j < line.Length; j++)
                {
                    spriteBatch.DrawString(
                        font,
                        line[j],
                        new Vector2(columns[j].X, drawY + i * font.LineSpacing),
                        Color.White
                    );
                }
            }

            string instr = "Back: Back    C: Clear leaderboard    X: Clear specific entry";
            spriteBatch.DrawString(
                font,
                instr,
                new Vector2(Game1.ScreenSize.X / 2 - font.MeasureString(instr).X / 2,
                lederBoardBox.Y + boxH - font.LineSpacing - 10),
                Color.White
            );

            if (mode == ClearMode.EnteringPasswordForAll ||
                mode == ClearMode.EnteringPasswordForSingle)
            {
                DrawPasswordInput(spriteBatch);
            }

            if (mode == ClearMode.EnteringIndex)
            {
                DrawIndexInput(spriteBatch);
            }

            spriteBatch.End();
        }

        public void DrawDifficultyButton(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(pixel, difficultyButtonRect, new Color(30, 30, 30, 220));
            string diffText = difficulty.ToString();
            Vector2 dtSize = font.MeasureString(diffText);
            spriteBatch.DrawString(
                font,
                diffText,
                new Vector2(difficultyButtonRect.X + difficultyButtonRect.Width / 2 - dtSize.X / 2,
                    difficultyButtonRect.Y + difficultyButtonRect.Height / 2 - dtSize.Y / 2),
                Color.White
            );
        }

        private void DrawPasswordInput(SpriteBatch spriteBatch)
        {
            string prompt = "Enter admin password:";
            spriteBatch.DrawString(
                font,
                prompt,
                new Vector2(Game1.ScreenSize.X / 2 - font.MeasureString(prompt).X / 2,
                    lederBoardBox.Y + boxH - font.LineSpacing - 50),
                Color.Red
            );

            spriteBatch.DrawString(
                font,
                pass,
                new Vector2(Game1.ScreenSize.X / 2 - font.MeasureString(pass).X / 2,
                    lederBoardBox.Y + boxH - font.LineSpacing - 30),
                Color.Red
            );
        }

        private void DrawIndexInput(SpriteBatch spriteBatch)
        {
            string prompt = "Enter index of entry to clear:";
            spriteBatch.DrawString(
                font,
                prompt,
                new Vector2(Game1.ScreenSize.X / 2 - font.MeasureString(prompt).X / 2,
                    lederBoardBox.Y + boxH - font.LineSpacing - 50),
                Color.Red
            );

            spriteBatch.DrawString(
                font,
                clearIndex,
                new Vector2(Game1.ScreenSize.X / 2 - font.MeasureString(clearIndex).X / 2,
                    lederBoardBox.Y + boxH - font.LineSpacing - 30),
                Color.Red
            );
        }
    }
}
