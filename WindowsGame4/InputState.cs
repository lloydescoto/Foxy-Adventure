using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace WindowsGame4
{
    public class InputState
    {
        public const int MaxInputs = 4;
        public readonly KeyboardState[] CurrentKeyboardStates;
        public readonly KeyboardState[] LastKeyboardStates;
        public InputState()
        {
            CurrentKeyboardStates = new KeyboardState[MaxInputs];
            LastKeyboardStates = new KeyboardState[MaxInputs];
        }
        public void Update()
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                LastKeyboardStates[i] = CurrentKeyboardStates[i];
                CurrentKeyboardStates[i] = Keyboard.GetState((PlayerIndex)i);
            }
        }
        public bool IsNewKeyPress(Keys key, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                playerIndex = controllingPlayer.Value;
                int i = (int)playerIndex;
                return (CurrentKeyboardStates[i].IsKeyDown(key) &&
                        LastKeyboardStates[i].IsKeyUp(key));
            }
            else
            {
                return (IsNewKeyPress(key, PlayerIndex.One, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Two, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Three, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Four, out playerIndex));
            }
        }
        public bool IsMenuSelect(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            return IsNewKeyPress(Keys.Space, controllingPlayer, out playerIndex) ||
                   IsNewKeyPress(Keys.Enter, controllingPlayer, out playerIndex);
        }
        public bool IsMenuCancel(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            return IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex);
        }
        public bool IsMenuUp(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;
            return IsNewKeyPress(Keys.Up, controllingPlayer, out playerIndex) ||
                   IsNewKeyPress(Keys.W, controllingPlayer, out playerIndex);
        }
        public bool IsMenuDown(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return IsNewKeyPress(Keys.Down, controllingPlayer, out playerIndex) ||
                   IsNewKeyPress(Keys.S, controllingPlayer, out playerIndex);
        }

        public bool IsPauseGame(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex) ||
                   IsNewKeyPress(Keys.P, controllingPlayer, out playerIndex);
        }
    }
}
