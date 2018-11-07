using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WindowsGame4
{
    class MainMenuScreen : MenuScreen
    {
        public MainMenuScreen() : base("Foxy Adventures")
        {
            MenuEntry playGameMenuEntry = new MenuEntry("Play Game");
            MenuEntry controlsMenuEntry = new MenuEntry("Controls");
            MenuEntry exitMenuEntry = new MenuEntry("Exit");
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            controlsMenuEntry.Selected += ControlsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;
            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(controlsMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }
        void PlayGameMenuEntrySelected(object sender, PlayerIndexEvent e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new GameplayScreen());
        }
        void ControlsMenuEntrySelected(object sender, PlayerIndexEvent e)
        {
            ScreenManager.AddScreen(new ControlsMenuScreen(), e.PlayerIndex);
        }
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            const string message = "Are you sure you want to exit this game?";
            MessageBox confirmExitMessageBox = new MessageBox(message);
            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;
            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEvent e)
        {
            ScreenManager.Game.Exit();
        }
    }
}
