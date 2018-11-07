using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WindowsGame4
{
    class PlayerIndexEvent : EventArgs
    {
        PlayerIndex playerIndex;
        public PlayerIndexEvent(PlayerIndex playerIndex)
        {
            this.playerIndex = playerIndex;
        }
        public PlayerIndex PlayerIndex
        {
            get { return playerIndex; }
        }
    }
}
