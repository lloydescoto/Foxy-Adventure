using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame4
{
    class GameFinish : MenuScreen
    {
        public GameFinish()
            : base("Congratulations")
        {
            MenuEntry restartGameMenuEntry = new MenuEntry("Restart Game");
            MenuEntry quitGameMenuEntry = new MenuEntry("Quit Game");
            restartGameMenuEntry.Selected += RestartGameMenuEntrySelected;
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;
            MenuEntries.Add(restartGameMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);
        }
        void QuitGameMenuEntrySelected(object sender, PlayerIndexEvent e)
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new MainMenuScreen());
        }

        void RestartGameMenuEntrySelected(object sender, PlayerIndexEvent e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new GameplayScreen());
        }
    }
}
