using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
/*using System.Numerics;*/
using System.Text;
using System.Threading.Tasks;

namespace MyGame
{
    internal class Sprite
    {
        public Texture2D texture;
        public Vector2 position;
        public int scaleWight = 100;
        public int scaleHeight = 100;
        public AnimationManager Animator;
        private bool IsAnimated = false;
        public Sprite(Vector2 position)
        {
            this.position = position;
        }

        public Sprite(Texture2D texture, Vector2 position)
        {
            this.texture = texture;
            this.position = position;
        }

        public Sprite(Texture2D texture, Vector2 pos, Vector2 size, int numFrames, int numColumns) : this(texture, pos)
        {
            Animator = new AnimationManager(numFrames, numColumns, size);
            IsAnimated = true;
            scaleWight = (int)size.X;
            scaleHeight = (int)size.Y;
        }

        public Rectangle Rect
        {
            get
            {
                return new Rectangle((int)position.X, (int)position.Y, scaleWight, scaleHeight);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            if (IsAnimated) Animator.Update();
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (IsAnimated)
                spriteBatch.Draw(texture, Rect, Animator.GetFrame(), Color.White);
            else
                spriteBatch.Draw(texture, Rect, Color.White);
        }
    }
}
