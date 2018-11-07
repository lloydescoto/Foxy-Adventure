using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame4
{
    class DownwardEnemy
    {
        public Texture2D EnemyTexture { get; set; }
        public Vector2 Position;
        public bool Active;
        public int Value;
        public int Damage;
        float enemyMoveSpeed;
        public int Width { get { return EnemyTexture.Width; } }
        public int Height { get { return EnemyTexture.Height; } }
        public void Initialize(Texture2D texture, Vector2 position, float speed, int damage, int value)
        {
            EnemyTexture = texture;
            Position = position;
            enemyMoveSpeed = speed;
            Value = value;
            Damage = damage;
            Active = true;
        }
        public void Update()
        {
            Position.Y += enemyMoveSpeed;

            if (Position.Y < -Height)
            {
                Active = false;
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(EnemyTexture, Position, Color.White);
            spriteBatch.End();
        }
    }
}
