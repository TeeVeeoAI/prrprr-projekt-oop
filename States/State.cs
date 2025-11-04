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

        public State(Game1 game, GraphicsDevice graphics, ContentManager content)
        {
            game1 = game;
            graphicsDevice = graphics;
            contentManager = content;
            pixel = new Texture2D(graphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            game1.SetBGColor(Color.Black);
        }

        public abstract void LoadContent();
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}