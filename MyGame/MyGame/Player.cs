using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace MyGame
{
    internal class Player : Sprite
    {
        List<Sprite> collisionGroup;
        public bool IsAttacking = false;
        private int hp = 3;
        private int invulTime = 0;
        private Vector2 startPos = Vector2.Zero;
        public Rectangle collisionRect
        {
            // 28 - высота, занимаемая огнём от ракеты
            get { return new Rectangle((int)position.X, (int)position.Y, scaleWight, scaleHeight - 28); }
        } 
        public Rectangle hitBox
        {
            get { return new Rectangle((int)position.X + 20, (int)position.Y + 28, 44, 36); }
        }
        public int HP { get { return hp; } }
        public void GetHP(int num)
        {
            hp += num;
        }
        public Player(Texture2D texture, Vector2 pos, Vector2 size, int numFrames, int numColumns, List<Sprite> cG, List<Sprite> allS)
            : base(texture, pos, size, numFrames, numColumns)
        {
            this.collisionGroup = cG;
            startPos = new Vector2(pos.X, pos.Y);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            invulTime--;

            // атакует ли игрок
            if (Keyboard.GetState().IsKeyDown(Keys.Z)) IsAttacking = true;
            else IsAttacking = false;

            // скорость
            int speed = 7;
            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift)) speed = 4;

            // управление + коллизии 
            int changeX = 0;
            if (Keyboard.GetState().IsKeyDown(Keys.Right) && !Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                changeX += speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                changeX -= speed;
            }
            position.X += changeX;
            foreach (var sprite in collisionGroup)
            {
                if (sprite.Rect.Intersects(collisionRect))
                {
                    if (collisionRect.X - changeX >= sprite.position.X + sprite.scaleWight)
                        position.X = sprite.position.X + sprite.scaleWight;
                    else position.X = sprite.position.X - scaleWight;
                }
            }

            int changeY = 0;
            if (Keyboard.GetState().IsKeyDown(Keys.Up) && !Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                changeY -= speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                changeY += speed;
            }
            position.Y += changeY;
            foreach (var sprite in collisionGroup)
            {
                if (sprite.Rect.Intersects(collisionRect))
                {
                    if (collisionRect.Y - changeY >= sprite.position.Y + sprite.scaleHeight)
                        position.Y = sprite.position.Y + sprite.scaleHeight;
                    else position.Y = sprite.position.Y - scaleHeight + 28;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (invulTime <= 0 || invulTime % 20 > 10) base.Draw(spriteBatch);
        }

        public void GetDamage()
        {
            if (invulTime <= 0)
            {
                hp--;
                invulTime = 100;
                position.X = startPos.X;
                position.Y = startPos.Y;
            }
        }
    }
}
