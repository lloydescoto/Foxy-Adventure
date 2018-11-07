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

namespace WindowsGame4
{
    class Background
    {
        Texture2D texture;
        Vector2[] positions;
        int speed;
        public void Initialize(ContentManager content, String texturePath, int screenWidth, int speed)
        {
            texture = content.Load<Texture2D>(texturePath);
            this.speed = speed;
            positions = new Vector2[screenWidth / texture.Width + 1];
            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = new Vector2(i * texture.Width, 0);
            }
        }
        public void Update()
        {
            for (int i = 0; i < positions.Length; i++)
            {
                positions[i].X += speed;
                if (speed <= 0)
                {
                    if (positions[i].X <= -texture.Width)
                    {
                        positions[i].X = texture.Width * (positions.Length - 1);
                    }
                }
                else
                {
                    if (positions[i].X >= texture.Width * (positions.Length - 1))
                    {
                        positions[i].X = -texture.Width;
                    }
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(texture, positions[i], new Rectangle(0,0,800,600), Color.White);
                spriteBatch.End();
            }
        }
    }
}
