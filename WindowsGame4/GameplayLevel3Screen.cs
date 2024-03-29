﻿using System;
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
    class GameplayLevel3Screen : GameScreen
    {
        ContentManager content;
        SpriteFont gameFont;
        SpriteFont smallFont;
        SpriteBatch spriteBatch;
        Animation animatedSprite;
        Texture2D texture;
        KeyboardState currentKeyboardState;
        Background mainbackground;
        Texture2D enemySkullStone;
        Texture2D enemyCruncher;
        Texture2D enemyFlyingBone;
        Vector2 playerLocation;
        List<ForwardEnemy> enemiesCrucher;
        List<UpwardEnemy> enemiesSkullStone;
        List<ForwardEnemy> enemiesFlyingBone;
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
        public GameplayLevel3Screen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }
        public override void LoadContent()
        {
            projectiles = new List<Projectile>();
            playerLocation = new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X, (ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Y + ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Height / 2) + 45);
            enemiesCrucher = new List<ForwardEnemy>();
            enemiesFlyingBone = new List<ForwardEnemy>();
            enemiesSkullStone = new List<UpwardEnemy>();
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
            mainbackground.Initialize(content, "violet", ScreenManager.GraphicsDevice.Viewport.Width, -1);
            texture = content.Load<Texture2D>("kit_from_firefox");
            animatedSprite = new Animation(texture, 9, 3);
            enemyCruncher = content.Load<Texture2D>("Cruncher");
            enemySkullStone = content.Load<Texture2D>("SkullStone");
            enemyFlyingBone = content.Load<Texture2D>("FlyingBone");
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
            for (int i = 0; i < enemiesCrucher.Count; i++)
            {
                enemiesCrucher[i].Draw(spriteBatch);
            }
            for (int i = 0; i < enemiesSkullStone.Count; i++)
            {
                enemiesSkullStone[i].Draw(spriteBatch);
            }
            for (int i = 0; i < enemiesFlyingBone.Count; i++)
            {
                enemiesFlyingBone[i].Draw(spriteBatch);
            }
            for (int i = 0; i < projectiles.Count; i++)
            {
                projectiles[i].Draw(spriteBatch);
            }
            spriteBatch.Begin();
            spriteBatch.DrawString(smallFont, "Level 3", new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + 380, ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
            spriteBatch.DrawString(smallFont, "Goal : 7000", new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + 720, ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
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
            Vector2 positionCruncher = new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + (enemyCruncher.Width / 2) + 700, random.Next(0, ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Height - 200));
            ForwardEnemy enemy2 = new ForwardEnemy();
            enemy2.Initialize(enemyCruncher, positionCruncher, 4f, 5, 100);
            enemiesCrucher.Add(enemy2);
            Vector2 positionSkullStone = new Vector2(random.Next(0, ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + 700), ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Y + 500);
            UpwardEnemy enemy3 = new UpwardEnemy();
            enemy3.Initialize(enemySkullStone, positionSkullStone, 4f, 5, 150);
            enemiesSkullStone.Add(enemy3);
            Vector2 positionFlyingBone = new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + (enemyFlyingBone.Width / 2) + 700, random.Next(0, ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Height - 200));
            ForwardEnemy enemy4 = new ForwardEnemy();
            enemy4.Initialize(enemyFlyingBone, positionFlyingBone, 4f, 5, 200);
            enemiesFlyingBone.Add(enemy4);
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
            for (int i = enemiesFlyingBone.Count - 1; i >= 0; i--)
            {
                enemiesFlyingBone[i].Update();

                if (enemiesFlyingBone[i].Active == false)
                {
                    enemiesFlyingBone.RemoveAt(i);
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
            Rectangle enemyCruncherSpace;
            Rectangle enemySkullStoneSpace;
            Rectangle enemyFlyingBoneSpace;
            Rectangle projectile;
            player = new Rectangle((int)playerLocation.X, (int)playerLocation.Y, texture.Width / 3, texture.Height / 9);
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
            for (int x = 0; x < enemiesFlyingBone.Count; x++)
            {
                enemyFlyingBoneSpace = new Rectangle((int)enemiesFlyingBone[x].Position.X, (int)enemiesFlyingBone[x].Position.Y, enemiesFlyingBone[x].Width, enemiesFlyingBone[x].Height);
                if (player.Intersects(enemyFlyingBoneSpace))
                {
                    score += enemiesFlyingBone[x].Value;
                    animatedSprite.Health -= enemiesFlyingBone[x].Damage;
                    enemiesFlyingBone.RemoveAt(x);
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
            for (int x = 0; x < projectiles.Count; x++)
            {
                projectile = new Rectangle((int)projectiles[x].Position.X, (int)projectiles[x].Position.Y, projectiles[x].Width, projectiles[x].Height);
                for (int y = 0; y < enemiesSkullStone.Count; y++)
                {
                    enemySkullStoneSpace = new Rectangle((int)enemiesSkullStone[y].Position.X, (int)enemiesSkullStone[y].Position.Y, enemiesSkullStone[y].Width, enemiesSkullStone[y].Height);
                    if (projectile.Intersects(enemySkullStoneSpace))
                    {
                        score += enemiesSkullStone[y].Value;
                        enemiesSkullStone.RemoveAt(y);
                        projectiles.RemoveAt(x);
                        if (score >= 7000)
                        {
                            LoadingScreen.Load(ScreenManager, true, ControllingPlayer, new GameplayLevel4Screen());
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
                        if (score >= 7000)
                        {
                            LoadingScreen.Load(ScreenManager, true, ControllingPlayer, new GameplayLevel4Screen());
                        }
                    }
                }
                for (int y = 0; y < enemiesFlyingBone.Count; y++)
                {
                    enemyFlyingBoneSpace = new Rectangle((int)enemiesFlyingBone[y].Position.X, (int)enemiesFlyingBone[y].Position.Y, enemiesFlyingBone[y].Width, enemiesFlyingBone[y].Height);
                    if (projectile.Intersects(enemyFlyingBoneSpace))
                    {
                        score += enemiesFlyingBone[y].Value;
                        enemiesFlyingBone.RemoveAt(y);
                        projectiles.RemoveAt(x);
                        if (score >= 7000)
                        {
                            LoadingScreen.Load(ScreenManager, true, ControllingPlayer, new GameplayLevel4Screen());
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
