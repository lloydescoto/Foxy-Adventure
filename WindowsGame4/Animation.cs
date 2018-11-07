using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WindowsGame4
{
    class Animation
    {
        public Texture2D Texture { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        private int currentFrame;
        private int totalFrames;
        public int Health;
        public Animation(Texture2D texture, int rows, int columns)
        {
            Texture = texture;
            Rows = rows;
            Columns = columns;
            currentFrame = 0;
            totalFrames = Rows * Columns;
            Health = 100;
        }
        public void Update(GameTime gametime, KeyboardState currentKeyboardState)
        {
            currentKeyboardState = Keyboard.GetState();
            if(currentKeyboardState.GetPressedKeys().Length == 0)
            {
                currentFrame = 0;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Up) || currentKeyboardState.IsKeyDown(Keys.W))
            {
                currentFrame = 6;
                currentFrame++;
                if (currentFrame == 9)
                    currentFrame = 6;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Right) || currentKeyboardState.IsKeyDown(Keys.S))
            {
                currentFrame = 12;
                currentFrame++;
                if (currentFrame == 15)
                    currentFrame = 12;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Left) || currentKeyboardState.IsKeyDown(Keys.A))
            {
                currentFrame = 3;
                currentFrame++;
                if (currentFrame == 6)
                    currentFrame = 3;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Down) || currentKeyboardState.IsKeyDown(Keys.D))
            {
                currentFrame = 9;
                currentFrame++;
                if (currentFrame == 12)
                    currentFrame = 9;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Space))
            {
                currentFrame = 11;
                currentFrame++;
                if (currentFrame == 12)
                    currentFrame = 11;
            }
        }
        public void Draw(SpriteBatch spriteBatch, Vector2 location)
        {
            int width = Texture.Width / Columns;
            int height = Texture.Height / Rows;
            int row = (int)((float)currentFrame / (float)Columns);
            int column = currentFrame % Columns;

            Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
            Rectangle destinationRectangle = new Rectangle((int)location.X, (int)location.Y, width, height);

            spriteBatch.Begin();
            spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.White);
            spriteBatch.End();
        }
    }
}
