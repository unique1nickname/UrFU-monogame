using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame
{
    public enum EnemyBulletType
    {
        First, 
        SecondRight,
        SecondLeft
    }
    internal class Enemy : Sprite
    {
        private bool IsAttacking = true;
        public int attackTimer = 100;
        public int attackPause = 50;
        public int count;
        public EnemyBulletType type;
        Random rnd = new Random();
        int direction;
        public Enemy(Texture2D texture, Vector2 pos, Vector2 size, int numFrames, int numColumns)
            : base(texture, pos, size, numFrames, numColumns)
        {
            type = EnemyBulletType.SecondLeft;
            direction = rnd.Next(-1, 2);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (IsAttacking)
            {
                count++;
                if (count == attackTimer)
                {
                    IsAttacking = false;
                    count = 0;
                }
            }
            else
            {
                count++;
                if (count == attackPause)
                {
                    IsAttacking = true;
                    count = 0;
                }
            }
            position.Y ++;
            position.X += direction;
        }

        public bool AttackStatus()
        {
            if (IsAttacking) 
            {
                if (type == EnemyBulletType.First && count % 50 == 0) return true; 
                if ((type == EnemyBulletType.SecondRight || type == EnemyBulletType.SecondLeft)
                    && count % 60 == 0)
                    return true;
            }
            return false;
        }
    }
}
