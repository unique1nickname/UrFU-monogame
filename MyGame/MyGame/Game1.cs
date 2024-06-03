using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace MyGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        List<Sprite> activeSprites;
        List<Enemy> enemySprites;
        List<PlayerBullet> playerBullets;
        List<Sprite> enemyBullets;
        List<Sprite> collisionList;
        Player player;

        Texture2D mob1;
        Sprite gameover;
        Sprite background;
        Sprite heart;
        Song song;

        private SpriteFont ArialFont;
        Random rnd = new Random();
        int score = 0;
        int bestScore = 0;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // списки
            activeSprites = new();
            enemySprites = new();
            playerBullets = new();
            enemyBullets = new();
            collisionList = new();

            //фон
            Texture2D backgroundTexture = Content.Load<Texture2D>("background");
            background = new Sprite(backgroundTexture, Vector2.Zero);
            background.scaleWight = 1280;
            background.scaleHeight = 720;
            activeSprites.Add(background);

            // музыка
            song = Content.Load<Song>("Remaster-Arrange Bad Apple");
            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = true;

            // стены вокруг окна
            var wallUp = new Sprite(new Vector2(0, -5));
            wallUp.scaleWight = 1280;
            wallUp.scaleHeight = 5;
            collisionList.Add(wallUp);
            var wallDown = new Sprite(new Vector2(0, 720));
            wallDown.scaleWight = 1280;
            wallDown.scaleHeight = 5;
            collisionList.Add(wallDown);
            var wallLeft = new Sprite(new Vector2(-5, 0));
            wallLeft.scaleWight = 5;
            wallLeft.scaleHeight = 720;
            collisionList.Add(wallLeft);
            var wallRight = new Sprite(new Vector2(1280, 0));
            wallRight.scaleWight = 5;
            wallRight.scaleHeight = 720;
            collisionList.Add(wallRight);


            // создание моба (1)
            mob1 = Content.Load<Texture2D>("mob1");
            /*
            Enemy mob1S = new Enemy(mob1, new Vector2(600, 200), new Vector2(12, 11), 3, 3);
            mob1S.scaleHeight = 44;
            mob1S.scaleWight = 48;
            activeSprites.Add(mob1S);
            enemySprites.Add(mob1S);
            */

            /*
             // создание блока (удалить)
             Texture2D block = Content.Load<Texture2D>("block");
             Sprite blockS = new Sprite(block, new Vector2(400, 200));
             activeSprites.Add(blockS);
             collisionList.Add(blockS); // to delate
            */

            // создание игрока
            Texture2D spaceship = Content.Load<Texture2D>("spaceship");
            player = new Player(spaceship, new Vector2(600, 600), new Vector2(21, 24), 3, 3, collisionList, activeSprites);
            player.scaleHeight = 84;
            player.scaleWight = 96;
            activeSprites.Add(player);

            // шрифт
            ArialFont = Content.Load<SpriteFont>("Arial");

            // экран проигрыша 
            Texture2D gameoverTexture = Content.Load<Texture2D>("gameover");
            gameover = new Sprite(gameoverTexture, new Vector2(265, 200));
            gameover.scaleWight = 750;
            gameover.scaleHeight = 300;

            // спрайт для HP
            Texture2D heartTexture = Content.Load<Texture2D>("heart");
            heart = new Sprite(heartTexture, new Vector2(10, 10));
            heart.scaleWight = 44;
            heart.scaleHeight = 40;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // после определённого времени не получается нормально просчитывать выстрелы
            // поэтому после 3х минут время обнуляется
            if ((int)gameTime.TotalGameTime.TotalMinutes > 3) gameTime.TotalGameTime = TimeSpan.Zero;

            if (player.HP > 0)
            {
                //счёт игрока (за время выживания)
                score++;

                // обновление спрайтов
                foreach (var sprite in activeSprites)
                {
                    sprite.Update(gameTime);
                }

                // создание пуль игрока
                if (player.IsAttacking && (int)gameTime.TotalGameTime.TotalMilliseconds % 15 == 0)
                    MakePlayerBullet();

                // создание пуль врага
                foreach (var enemy in enemySprites)
                {
                    if (enemy.AttackStatus())
                        MakeEnemyBullet(enemy.position, enemy.type);
                }

                // логика пуль врага
                List<Sprite> enemyBulletsKillList = new();
                foreach (var bullet in enemyBullets)
                {
                    if (bullet.Rect.Intersects(player.hitBox))
                    {
                        player.GetDamage();
                        enemyBulletsKillList.Add(bullet);
                    }
                    if (bullet.position.Y > 770 || bullet.position.Y < -25 || bullet.position.X < 0 || bullet.position.X > 1330)
                        enemyBulletsKillList.Add(bullet);
                }

                // получение урона игроком
                foreach (var enemy in enemySprites)
                {
                    if (enemy.Rect.Intersects(player.hitBox))
                    {
                        player.GetDamage();
                    }
                }

                // логика пуль игрока + логика врагов
                List<Enemy> enemyKillList = new();
                List<PlayerBullet> playerBulletsKillList = new();
                foreach (var pBullet in playerBullets)
                {
                    if (pBullet.position.Y < -50) playerBulletsKillList.Add(pBullet);
                    foreach (var enemy in enemySprites)
                    {
                        if (enemy.Rect.Intersects(pBullet.Rect))
                        {
                            score += 100;
                            enemyKillList.Add(enemy);
                            playerBulletsKillList.Add(pBullet);
                        }
                        // удаление врага, если он за пределами экрана
                        if (enemy.position.Y > 720 || enemy.position.X < 0 || enemy.position.X > 1330)
                            enemyBulletsKillList.Add(enemy);
                    }
                }

                foreach (var pBullet in playerBulletsKillList)
                {
                    activeSprites.Remove(pBullet);
                    playerBullets.Remove(pBullet);
                }
                foreach (var enemy in enemyKillList)
                {
                    activeSprites.Remove(enemy);
                    enemySprites.Remove(enemy);
                }
                if ((int)gameTime.TotalGameTime.TotalSeconds % 2 == 0 && (int)gameTime.TotalGameTime.TotalMilliseconds % 55 == 0)
                    GetEnemy();
            }
            else 
            {
                activeSprites.Add(gameover);
                if (bestScore < score) bestScore = score;
                if (Keyboard.GetState().IsKeyDown(Keys.R))
                {
                    player.GetHP(3);
                    enemySprites.Clear();
                    enemyBullets.Clear();
                    playerBullets.Clear();
                    activeSprites.Clear();
                    activeSprites.Add(background);
                    activeSprites.Add(player);
                    score = 0;
                }
                
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin(samplerState: SamplerState.PointWrap);

            // прорисовка спрайтов
            foreach (var sprite in activeSprites)
            {
                sprite.Draw(_spriteBatch);
            }

            // текст счёта
            _spriteBatch.DrawString(ArialFont, $"Score: {score}", new Vector2(15, 60), Color.Black);

            // HP
            Vector2 heartPosition = new Vector2(15, 15);
            Vector2 heartInterval = new Vector2(59, 0);
            for (int i = 0; i < player.HP; i++)
            {
                heart.position = heartPosition + heartInterval * i;
                heart.Draw(_spriteBatch);
            }

            // рисует счёт при проигрыше
            if (player.HP <= 0)
            {
                _spriteBatch.DrawString(
                    ArialFont,
                    $"Score: {score}\nBest: {bestScore}", 
                    new Vector2(550, 390), 
                    Color.White);
            }

            // обучение
            if (gameTime.TotalGameTime.TotalSeconds < 10)
            {
                _spriteBatch.DrawString(
                    ArialFont,
                    $"({10-(int)gameTime.TotalGameTime.TotalSeconds}) Control: Left Up Down Right;   Press Shift to slow down;   Press Z to shoot",
                    new Vector2(20, 675),
                    Color.Black);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        public void MakePlayerBullet()
        {
            // нужно оптимизировать !!!
            Texture2D playerBullet1 = Content.Load<Texture2D>("PlayerBullet1");
            PlayerBullet bullet1 = new PlayerBullet(playerBullet1, player.position);
            bullet1.scaleWight = 16;
            bullet1.scaleHeight = 36;
            activeSprites.Add(bullet1);
            playerBullets.Add(bullet1);

            PlayerBullet bullet2 = new PlayerBullet(playerBullet1, player.position + new Vector2(78, 0));
            bullet2.scaleWight = 16;
            bullet2.scaleHeight = 36;
            activeSprites.Add(bullet2);
            playerBullets.Add(bullet2);
        }

        public void MakeEnemyBullet(Vector2 pos, EnemyBulletType type)
        {
            switch (type)
            {
                case EnemyBulletType.First:
                    Texture2D enemyBullet1 = Content.Load<Texture2D>("EnemyBullet1");
                    EnemyBullet1 bullet1 = new EnemyBullet1(enemyBullet1, pos);
                    bullet1.scaleWight = 36;
                    bullet1.scaleHeight = 36;
                    activeSprites.Add(bullet1);
                    enemyBullets.Add(bullet1);
                    break;
                case EnemyBulletType.SecondRight:
                    Texture2D enemyBullet2 = Content.Load<Texture2D>("EnemyBullet2");
                    EnemyBullet2 bullet2 = new EnemyBullet2(enemyBullet2, pos);
                    bullet2.scaleWight = 32;
                    bullet2.scaleHeight = 32;
                    activeSprites.Add(bullet2);
                    enemyBullets.Add(bullet2);
                    break;
                case EnemyBulletType.SecondLeft:
                    Texture2D enemyBullet3 = Content.Load<Texture2D>("EnemyBullet3");
                    EnemyBullet3 bullet3 = new EnemyBullet3(enemyBullet3, pos);
                    bullet3.scaleWight = 32;
                    bullet3.scaleHeight = 32;
                    activeSprites.Add(bullet3);
                    enemyBullets.Add(bullet3);
                    break;
            }
        }

        public void GetEnemy()
        {
            int enemyNumber = rnd.Next(3, 6);
            for (int i = 0; i < enemyNumber; i++)
            {
                int type = rnd.Next(0, 3);
                int valueX = rnd.Next(80, 1200);
                int valueY = rnd.Next(-110, -39);
                Enemy mobS = new Enemy(mob1, new Vector2(valueX, valueY), new Vector2(12, 11), 3, 3);
                mobS.scaleHeight = 44;
                mobS.scaleWight = 48;
                mobS.type = (EnemyBulletType)type;
                activeSprites.Add(mobS);
                enemySprites.Add(mobS);
                
            }
            
        }
    }
}