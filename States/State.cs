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
    public abstract class State
    {
        protected Game1 game1;
        protected GraphicsDevice graphicsDevice;
        protected ContentManager contentManager;
        protected KeyboardState kstateNew, kstateOld;
        protected SpriteFont font;
        protected Texture2D pixel;
        protected bool starting = true;
        protected Color BGcolor;

        public State(Game1 game, GraphicsDevice graphics, ContentManager content)
        {
            game1 = game;
            graphicsDevice = graphics;
            contentManager = content;
            pixel = new Texture2D(graphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
        }

        public virtual void LoadContent()
        {
            game1.SetBGColor(BGcolor);
        }
        public virtual void Update(GameTime gameTime)
        {
            if (kstateNew.IsKeyDown(Keys.Escape) && kstateOld.IsKeyUp(Keys.Escape))
            {
                game1.Exit();
            }
        }
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}