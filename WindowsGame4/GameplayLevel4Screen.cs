using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Threading;

namespace WindowsGame4
{
    class GameplayLevel4Screen : GameScreen
    {
        ContentManager content;
        SpriteFont gameFont;
        SpriteFont smallFont;
        SpriteBatch spriteBatch;
        Animation animatedSprite;
        Texture2D texture;
        KeyboardState currentKeyboardState;
        Background mainbackground;
        Texture2D enemyRockStone;
        Texture2D enemySkullStone;
        Texture2D enemySpider;
        Texture2D enemyCruncher;
        Vector2 playerLocation;
        List<ForwardEnemy> enemiesRockStone;
        List<ForwardEnemy> enemiesCrucher;
        List<UpwardEnemy> enemiesSkullStone;
        List<DownwardEnemy> enemiesSpider;
        TimeSpan enemySpawnTime;
        TimeSpan previousSpawnTime;
        Song gameplayMusic;
        Texture2D projectileTexture;
        List<Projectile> projectiles;
        TimeSpan fireTime;
        TimeSpan previousFireTime;
        float playerMoveSpeed;
        int score;
        Random random;

        float pauseAlpha;
        public GameplayLevel4Screen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }
        public override void LoadContent()
        {
            projectiles = new List<Projectile>();
            playerLocation = new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X, (ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Y + ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Height / 2) + 45);
            enemiesRockStone = new List<ForwardEnemy>();
            enemiesCrucher = new List<ForwardEnemy>();
            enemiesSkullStone = new List<UpwardEnemy>();
            enemiesSpider = new List<DownwardEnemy>();
            previousSpawnTime = TimeSpan.Zero;
            enemySpawnTime = TimeSpan.FromSeconds(1.0f);
            fireTime = TimeSpan.FromSeconds(.03f);
            random = new Random();
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");
            projectileTexture = content.Load<Texture2D>("laser");
            gameplayMusic = content.Load<Song>("gameMusic");
            gameFont = content.Load<SpriteFont>("gamefont");
            smallFont = content.Load<SpriteFont>("smallFont");
            spriteBatch = new SpriteBatch(ScreenManager.GraphicsDevice);
            mainbackground = new Background();
            mainbackground.Initialize(content, "city", ScreenManager.GraphicsDevice.Viewport.Width, -1);
            texture = content.Load<Texture2D>("kit_from_firefox");
            animatedSprite = new Animation(texture, 9, 3);
            enemyRockStone = content.Load<Texture2D>("Rock");
            enemyCruncher = content.Load<Texture2D>("Cruncher");
            enemySkullStone = content.Load<Texture2D>("SkullStone");
            enemySpider = content.Load<Texture2D>("Spider");
            playerMoveSpeed = 6.0f;
            score = 0;
            Thread.Sleep(1000);
            ScreenManager.Game.ResetElapsedTime();
            PlayMusic(gameplayMusic);
        }
        public override void UnloadContent()
        {
            content.Unload();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);
            if (IsActive)
            {
                currentKeyboardState = Keyboard.GetState();
                animatedSprite.Update(gameTime, currentKeyboardState);
                mainbackground.Update();
                PlayerMovement(gameTime);
                UpdateEnemies(gameTime);
                UpdateProjectiles();
                UpdateCollison();
            }
        }
        public override void HandleInput(InputState input)
        {
            int playerIndex = (int)ControllingPlayer.Value;
            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            if (input.IsPauseGame(ControllingPlayer))
            {
                ScreenManager.AddScreen(new PauseScreen(), ControllingPlayer);
            }
        }
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(Color.CornflowerBlue);
            mainbackground.Draw(spriteBatch);
            animatedSprite.Draw(spriteBatch, playerLocation);
            for (int i = 0; i < enemiesRockStone.Count; i++)
            {
                enemiesRockStone[i].Draw(spriteBatch);
            }
            for (int i = 0; i < enemiesCrucher.Count; i++)
            {
                enemiesCrucher[i].Draw(spriteBatch);
            }
            for (int i = 0; i < enemiesSkullStone.Count; i++)
            {
                enemiesSkullStone[i].Draw(spriteBatch);
            }
            for (int i = 0; i < enemiesSpider.Count; i++)
            {
                enemiesSpider[i].Draw(spriteBatch);
            }
            for (int i = 0; i < projectiles.Count; i++)
            {
                projectiles[i].Draw(spriteBatch);
            }
            spriteBatch.Begin();
            spriteBatch.DrawString(smallFont, "Level 4", new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + 380, ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
            spriteBatch.DrawString(smallFont, "Goal : 10000", new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + 720, ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
            spriteBatch.DrawString(smallFont, "Health : " + animatedSprite.Health, new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
            spriteBatch.DrawString(smallFont, "Score : " + score, new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X, ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Y + 15), Color.White);
            spriteBatch.End();
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);
                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }
        private void PlayerMovement(GameTime gameTime)
        {
            if (currentKeyboardState.GetPressedKeys().Length == 0 || currentKeyboardState.IsKeyDown(Keys.Space))
            {
                playerLocation.Y += (playerMoveSpeed - 3f);
            }
            if (currentKeyboardState.IsKeyDown(Keys.Left) ||
            currentKeyboardState.IsKeyDown(Keys.A))
            {
                playerLocation.X -= playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Right) ||
            currentKeyboardState.IsKeyDown(Keys.D))
            {
                playerLocation.X += playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Up) ||
            currentKeyboardState.IsKeyDown(Keys.W))
            {
                playerLocation.Y -= playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Down) ||
            currentKeyboardState.IsKeyDown(Keys.S))
            {
                playerLocation.Y += playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Space))
            {
                if (gameTime.TotalGameTime - previousFireTime > fireTime)
                {
                    previousFireTime = gameTime.TotalGameTime;
                    AddProjectile(playerLocation + new Vector2(50, (texture.Height / 9) / 2));
                }
            }
            playerLocation.X = MathHelper.Clamp(playerLocation.X, 0, ScreenManager.GraphicsDevice.Viewport.Width - ((texture.Width / 3)));
            playerLocation.Y = MathHelper.Clamp(playerLocation.Y, 0, ScreenManager.GraphicsDevice.Viewport.Height - ((texture.Height / 9) + 120));
        }
        private void AddEnemy()
        {
            Vector2 position = new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + 800 + enemyRockStone.Width, (ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Y + ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Height / 2) + 70);
            ForwardEnemy enemy = new ForwardEnemy();
            enemy.Initialize(enemyRockStone, position, 5f, 5, 100);
            enemiesRockStone.Add(enemy);
            Vector2 positionCruncher = new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + (enemyCruncher.Width / 2) + 700, random.Next(0, ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Height - 200));
            ForwardEnemy enemy2 = new ForwardEnemy();
            enemy2.Initialize(enemyCruncher, positionCruncher, 5f, 5, 100);
            enemiesCrucher.Add(enemy2);
            Vector2 positionSkullStone = new Vector2(random.Next(0, ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + 700), ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Y + 500);
            UpwardEnemy enemy3 = new UpwardEnemy();
            enemy3.Initialize(enemySkullStone, positionSkullStone, 5f, 5, 100);
            enemiesSkullStone.Add(enemy3);
            Vector2 positionSpider = new Vector2(random.Next(0, ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + 700), ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Y - enemySpider.Height);
            DownwardEnemy enemy4 = new DownwardEnemy();
            enemy4.Initialize(enemySpider, positionSpider, 5f, 5, 100);
            enemiesSpider.Add(enemy4);
        }

        private void UpdateEnemies(GameTime gameTime)
        {
            if (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime)
            {
                previousSpawnTime = gameTime.TotalGameTime;
                AddEnemy();
            }
            for (int i = enemiesCrucher.Count - 1; i >= 0; i--)
            {
                enemiesCrucher[i].Update();

                if (enemiesCrucher[i].Active == false)
                {
                    enemiesCrucher.RemoveAt(i);
                }
            }
            for (int i = enemiesRockStone.Count - 1; i >= 0; i--)
            {
                enemiesRockStone[i].Update();

                if (enemiesRockStone[i].Active == false)
                {
                    enemiesRockStone.RemoveAt(i);
                }
            }
            for (int i = enemiesSkullStone.Count - 1; i >= 0; i--)
            {
                enemiesSkullStone[i].Update();

                if (enemiesSkullStone[i].Active == false)
                {
                    enemiesSkullStone.RemoveAt(i);
                }
            }
            for (int i = enemiesSpider.Count - 1; i >= 0; i--)
            {
                enemiesSpider[i].Update();

                if (enemiesSpider[i].Active == false)
                {
                    enemiesSpider.RemoveAt(i);
                }
            }
        }
        private void AddProjectile(Vector2 position)
        {
            Projectile projectile = new Projectile();
            projectile.Initialize(ScreenManager.GraphicsDevice.Viewport, projectileTexture, position);
            projectiles.Add(projectile);
        }
        private void UpdateProjectiles()
        {
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                projectiles[i].Update();

                if (projectiles[i].Active == false)
                {
                    projectiles.RemoveAt(i);
                }
            }
        }
        private void UpdateCollison()
        {
            Rectangle player;
            Rectangle enemyRockStoneSpace;
            Rectangle enemyCruncherSpace;
            Rectangle enemySkullStoneSpace;
            Rectangle enemySpiderSpace;
            Rectangle projectile;
            player = new Rectangle((int)playerLocation.X, (int)playerLocation.Y, texture.Width / 3, texture.Height / 9);
            for (int x = 0; x < enemiesRockStone.Count; x++)
            {
                enemyRockStoneSpace = new Rectangle((int)enemiesRockStone[x].Position.X, (int)enemiesRockStone[x].Position.Y, enemiesRockStone[x].Width, enemiesRockStone[x].Height);
                if (player.Intersects(enemyRockStoneSpace))
                {
                    score += enemiesRockStone[x].Value;
                    animatedSprite.Health -= enemiesRockStone[x].Damage;
                    enemiesRockStone.RemoveAt(x);
                    if (animatedSprite.Health == 0)
                    {
                        ScreenManager.AddScreen(new GameOverScreen(), ControllingPlayer);
                    }
                }
            }
            for (int x = 0; x < enemiesCrucher.Count; x++)
            {
                enemyCruncherSpace = new Rectangle((int)enemiesCrucher[x].Position.X, (int)enemiesCrucher[x].Position.Y, enemiesCrucher[x].Width, enemiesCrucher[x].Height);
                if (player.Intersects(enemyCruncherSpace))
                {
                    score += enemiesCrucher[x].Value;
                    animatedSprite.Health -= enemiesCrucher[x].Damage;
                    enemiesCrucher.RemoveAt(x);
                    if (animatedSprite.Health == 0)
                    {
                        ScreenManager.AddScreen(new GameOverScreen(), ControllingPlayer);
                    }
                }
            }
            for (int x = 0; x < enemiesSkullStone.Count; x++)
            {
                enemySkullStoneSpace = new Rectangle((int)enemiesSkullStone[x].Position.X, (int)enemiesSkullStone[x].Position.Y, enemiesSkullStone[x].Width, enemiesSkullStone[x].Height);
                if (player.Intersects(enemySkullStoneSpace))
                {
                    score += enemiesSkullStone[x].Value;
                    animatedSprite.Health -= enemiesSkullStone[x].Damage;
                    enemiesSkullStone.RemoveAt(x);
                    if (animatedSprite.Health == 0)
                    {
                        ScreenManager.AddScreen(new GameOverScreen(), ControllingPlayer);
                    }
                }
            }
            for (int x = 0; x < enemiesSpider.Count; x++)
            {
                enemySpiderSpace = new Rectangle((int)enemiesSpider[x].Position.X, (int)enemiesSpider[x].Position.Y, enemiesSpider[x].Width, enemiesSpider[x].Height);
                if (player.Intersects(enemySpiderSpace))
                {
                    score += enemiesSpider[x].Value;
                    animatedSprite.Health -= enemiesSpider[x].Damage;
                    enemiesSpider.RemoveAt(x);
                    if (animatedSprite.Health == 0)
                    {
                        ScreenManager.AddScreen(new GameOverScreen(), ControllingPlayer);
                    }
                }
            }
            for (int x = 0; x < projectiles.Count; x++)
            {
                projectile = new Rectangle((int)projectiles[x].Position.X, (int)projectiles[x].Position.Y, projectiles[x].Width, projectiles[x].Height);
                for (int y = 0; y < enemiesRockStone.Count; y++)
                {
                    enemyRockStoneSpace = new Rectangle((int)enemiesRockStone[y].Position.X, (int)enemiesRockStone[y].Position.Y, enemiesRockStone[y].Width, enemiesRockStone[y].Height);
                    if (projectile.Intersects(enemyRockStoneSpace))
                    {
                        score += enemiesRockStone[y].Value;
                        enemiesRockStone.RemoveAt(y);
                        projectiles.RemoveAt(x);
                        if (score >= 10000)
                        {
                            LoadingScreen.Load(ScreenManager, true, ControllingPlayer, new GameplayLevel5Screen());
                        }
                    }
                }
                for (int y = 0; y < enemiesSkullStone.Count; y++)
                {
                    enemySkullStoneSpace = new Rectangle((int)enemiesSkullStone[y].Position.X, (int)enemiesSkullStone[y].Position.Y, enemiesSkullStone[y].Width, enemiesSkullStone[y].Height);
                    if (projectile.Intersects(enemySkullStoneSpace))
                    {
                        score += enemiesSkullStone[y].Value;
                        enemiesSkullStone.RemoveAt(y);
                        projectiles.RemoveAt(x);
                        if (score >= 10000)
                        {
                            LoadingScreen.Load(ScreenManager, true, ControllingPlayer, new GameplayLevel5Screen());
                        }
                    }
                }
                for (int y = 0; y < enemiesCrucher.Count; y++)
                {
                    enemyCruncherSpace = new Rectangle((int)enemiesCrucher[y].Position.X, (int)enemiesCrucher[y].Position.Y, enemiesCrucher[y].Width, enemiesCrucher[y].Height);
                    if (projectile.Intersects(enemyCruncherSpace))
                    {
                        score += enemiesCrucher[y].Value;
                        enemiesCrucher.RemoveAt(y);
                        projectiles.RemoveAt(x);
                        if (score >= 10000)
                        {
                            LoadingScreen.Load(ScreenManager, true, ControllingPlayer, new GameplayLevel5Screen());
                        }
                    }
                }
                for (int y = 0; y < enemiesSpider.Count; y++)
                {
                    enemySpiderSpace = new Rectangle((int)enemiesSpider[y].Position.X, (int)enemiesSpider[y].Position.Y, enemiesSpider[y].Width, enemiesSpider[y].Height);
                    if (projectile.Intersects(enemySpiderSpace))
                    {
                        score += enemiesSpider[y].Value;
                        enemiesSpider.RemoveAt(y);
                        projectiles.RemoveAt(x);
                        if (score >= 10000)
                        {
                            LoadingScreen.Load(ScreenManager, true, ControllingPlayer, new GameplayLevel5Screen());
                        }
                    }
                }
            }
        }
        private void PlayMusic(Song song)
        {
            try
            {
                MediaPlayer.Play(song);
                MediaPlayer.IsRepeating = true;
            }
            catch { }
        }
    }
}

