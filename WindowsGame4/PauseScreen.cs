using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame4
{
    class PauseScreen : MenuScreen
    {
        public PauseScreen() : base("Paused")
        {
            MenuEntry resumeGameMenuEntry = new MenuEntry("Resume Game");
            MenuEntry quitGameMenuEntry = new MenuEntry("Quit Game");
            resumeGameMenuEntry.Selected += OnCancel;
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;
            MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);
        }
        void QuitGameMenuEntrySelected(object sender, PlayerIndexEvent e)
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new MainMenuScreen());
        }
    }
}
