using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame4
{
    class MessageBox : GameScreen
    {
        string message;
        Texture2D backgroundTexture;
        public event EventHandler<PlayerIndexEvent> Accepted;
        public event EventHandler<PlayerIndexEvent> Cancelled;
        public MessageBox(string message)
          : this(message, true)
        { }
        public MessageBox(string message, bool includeUsageText)
        {
            const string usageText = "\nSpace, Enter = ok" +
                                     "\nEsc = cancel";
            if (includeUsageText)
                this.message = message + usageText;
            else
                this.message = message;
            IsPopup = true;
            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
        }
        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            backgroundTexture = content.Load<Texture2D>("black");
        }
        public override void HandleInput(InputState input)
        {
            PlayerIndex playerIndex;
            if (input.IsMenuSelect(ControllingPlayer, out playerIndex))
            {
                if (Accepted != null)
                    Accepted(this, new PlayerIndexEvent(playerIndex));

                ExitScreen();
            }
            else if (input.IsMenuCancel(ControllingPlayer, out playerIndex))
            {
                if (Cancelled != null)
                    Cancelled(this, new PlayerIndexEvent(playerIndex));

                ExitScreen();
            }
        }
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
            Vector2 textSize = font.MeasureString(message);
            Vector2 textPosition = (viewportSize - textSize) / 2;
            const int hPad = 32;
            const int vPad = 16;
            Rectangle backgroundRectangle = new Rectangle((int)textPosition.X - hPad, (int)textPosition.Y - vPad, (int)textSize.X + hPad * 2, (int)textSize.Y + vPad * 2);
            Color color = Color.White * TransitionAlpha;
            spriteBatch.Begin();
            spriteBatch.Draw(backgroundTexture, backgroundRectangle, Color.White);
            spriteBatch.DrawString(font, message, textPosition, color);
            spriteBatch.End();
        }
    }
}
